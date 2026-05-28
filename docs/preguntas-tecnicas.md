# Preguntas Técnicas - Análisis y Soluciones

Documento de respuestas a problemas dirigidos a lenjuaje .NET . Incluye diagramas, tablas de diagnóstico, fragmentos de código y justificación técnica y de ingeniería.

---

## Pregunta 1: Latencia en integración con mensajería instantánea al final de la tarde

### Diagnóstico

| Fase | Acción | Herramientas |
|---|---|---|
| **Monitoreo** | Identificar si es problema de recursos (CPU, RAM, IO, red) o de lógica | `dotnet-counters`, PerfMon, Azure Metrics |
| **Trazas distribuidas** | Rastrear tiempo de cada llamada: API → BD → servicio de mensajería | OpenTelemetry, Application Insights, Serilog + Seq |
| **Análisis de contención** | Detectar bloqueos por hilos / base de datos al final del día | `dotnet-dump`, WinDbg, SQL Server Activity Monitor |
| **Logging diferencial** | Comparar logs de la mañana vs. 5-6 PM | Elastic Stack (ELK), structured logging con Serilog |
| **Prueba de carga** | Replicar el pico de la tarde en ambiente controlado | k6, NBomber, JMeter |

```mermaid
flowchart TD
    A[Síntoma: integración falla al final de la tarde] --> B{¿Cuál es la causa raíz?}
    B -->|Timeout por alta concurrencia| C["Pool de conexiones agotado<br/>→ Usar HttpClientFactory +<br/>conexiones asíncronas con timeout"]
    B -->|Contención en BD| D["Reportes pesados bloqueando<br/>transacciones → Separar<br/>lecturas (Dapper / Read replicas)"]
    B -->|Rate limiting del proveedor| E["El servicio de mensajería<br/>limita requests → Implementar<br/>retry policy + circuit breaker<br/>(Polly)"]
    B -->|GC de alto impacto| F["Promoción a Gen 2 por<br/>objetos grandes → Usar<br/>ArrayPool&lt;T&gt;, evitar LOH"]
```

### Solución técnica

```csharp
// Patrón: Circuit Breaker + Retry + Timeout con Polly
services.AddHttpClient<IMessagingClient, MessagingClient>()
    .AddPolicyHandler(Policy
        .Handle<TimeoutException>()
        .OrTransientHttpError()
        .CircuitBreakerAsync(
            handledEventsAllowedBeforeBreaking: 3,
            durationOfBreak: TimeSpan.FromSeconds(30)))
    .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(5));
```

### Vista de ingeniería

Implementar **Bulkhead pattern** para aislar la integración de mensajería del resto del sistema, y **Queue-based load leveling** usando Azure Service Bus o RabbitMQ para desacoplar el envío síncrono. Esto evita que picos de mensajería afecten las transacciones principales y viceversa.

---

## Pregunta 2: Reporte matutino colapsa la base de datos

### Diagnóstico

```mermaid
flowchart LR
    subgraph "Problema"
        A["Query de reporte<br/>millones de registros<br/>JOINs masivos"]
    end
    subgraph "Diagnóstico"
        B["Plan de ejecución<br/>SQL Server Profiler<br/>DMV: sys.dm_exec_query_stats"]
    end
    subgraph "Soluciones"
        C["Materialized View"]
        D["Columnstore Index"]
        E["ETL nocturna a<br/>Reporting DB"]
    end
    A --> B --> C & D & E
```

### Acciones inmediatas (corto plazo)

| # | Acción | Impacto |
|---|---|---|
| 1 | Habilitar `SNAPSHOT ISOLATION` para evitar bloqueos en tablas transaccionales | Medio |
| 2 | Crear **índices cubrientes** para las tablas más consultadas en el reporte | Alto |
| 3 | Mover la ejecución del reporte a un **AlwaysOn Readable Secondary** | Alto |

### Solución definitiva (mediano plazo)

```mermaid
graph LR
    subgraph "Noche (ETL)"
        OP["BD Operacional"] -->|SQL Agent Job<br/>02:00 AM| DW["Data Warehouse<br/>Tablas resumen<br/>+ Columnstore"]
    end
    subgraph "Mañana (reporte)"
        DW -->|SELECT directo<br/>sin JOINs masivos| RPT["Reporte listo<br/>en segundos"]
    end
```

### Vista de ingeniería

Implementar un modelo **CQRS** donde el reporte no consulta la base operacional sino un **Data Warehouse** actualizado por un job nocturno. Las tablas de hecho usan **Columnstore Index** para compresión y velocidad analítica. Las consultas se hacen con **Dapper** en vez de EF Core para evitar overhead de materialización.

```sql
-- Tabla resumen creada por job nocturno
CREATE TABLE dbo.ResumenDiarioRegistraduria (
    Fecha DATE NOT NULL,
    CodigoRegistraduria VARCHAR(10) NOT NULL,
    TotalTransacciones INT,
    TotalCiudadanos INT,
    TiempoPromedioSegundos DECIMAL(10,2),
    INDEX IX_Columnstore CLUSTERED COLUMNSTORE
);
```

---

## Pregunta 3: Sistema contable multi-país con diferentes normas fiscales

### Arquitectura propuesta

```mermaid
graph TB
    subgraph "Core (invariante)"
        CORE["Core.Contracts<br/>ICountryTaxCalculator"]
        CORE2["Core.Entities<br/>Invoice, TaxLine"]
        CORE3["Core.Services<br/>InvoiceProcessor"]
    end
    subgraph "Plugins por país"
        P1["ColombiaPlugin"]
        P2["MéxicoPlugin"]
        P3["ArgentinaPlugin"]
        P4["BrasilPlugin"]
    end
    subgraph "Resolución"
        F["CountryTaxCalculatorFactory<br/>(DI + Strategy)"]
        CFG["appsettings.json<br/>País activo"]
    end

    CORE3 --> CORE
    CORE3 --> F
    F --> P1 & P2 & P3 & P4
    CFG --> F
```

### Patrones aplicados

| Patrón | Propósito |
|---|---|
| **Strategy Pattern** | Cada país implementa `ICountryTaxCalculator` con su lógica de impuestos |
| **Plugin Architecture** | Módulos por país como assemblies separados, cargados vía `AssemblyLoadContext` |
| **Factory + DI** | `CountryTaxCalculatorFactory` resuelve el calculador según configuración |
| **Chain of Responsibility** | Impuestos en cadena (IVA → retención → ICA, etc.) |
| **Feature Flags** | Activar/desactivar normas sin desplegar código |

### Solución técnica

```csharp
// Core - invariante
public interface ICountryTaxCalculator
{
    string CountryCode { get; }
    Task<TaxResult> CalculateAsync(Invoice invoice);
}

// Plugin Colombia
public class ColombiaTaxCalculator : ICountryTaxCalculator
{
    public string CountryCode => "CO";
    public async Task<TaxResult> CalculateAsync(Invoice invoice)
    {
        // Lógica específica: IVA 19%, Retefuente, ICA
    }
}

// Factory con DI dinámica
public class CountryTaxCalculatorFactory
{
    private readonly IEnumerable<ICountryTaxCalculator> _calculators;
    
    public ICountryTaxCalculator GetCalculator(string countryCode)
        => _calculators.First(c => c.CountryCode == countryCode);
}
```

### Vista de ingeniería

Implementar **modular monolith** donde cada país es un módulo independiente en su propio assembly. Si la escala lo justifica (20+ países), migrar a **microservicios** con un servicio contable por país y un API Gateway (Recomendable usar Ocelot). Usar **Database-per-tenant** con esquema por país para flexibilidad fiscal total sin acoplar estructuras de datos.

---

## Pregunta 4: 80+ clientes on-premise + Azure con parches costosos

### Arquitectura de despliegue

```mermaid
flowchart TD
    subgraph "Desarrollo"
        GIT["Azure Repos / GitHub"]
        CI["Azure DevOps CI<br/>Build + Tests + Security Scan"]
        REGISTRY["Container Registry /<br/>NuGet Feed"]
    end
    subgraph "Artefactos"
        A1["SQL Scripts<br/>(FluentMigrator)"]
        A2["Web App .NET<br/>(self-contained)"]
        A3["Docker Image"]
    end
    subgraph "Despliegue"
        D1["Clientes Azure:<br/>App Service + Deploy Slot"]
        D2["Clientes On-Premise con internet:<br/>Azure Arc + agente self-hosted"]
        D3["Clientes On-Premise air-gap:<br/>USB/NAS + script PowerShell"]
    end
    GIT --> CI --> REGISTRY
    REGISTRY --> A1 & A2 & A3
    A1 & A2 & A3 --> D1 & D2 & D3
```

### Estrategia por tipo de cliente

| Escenario | Despliegue | Actualización BD | SSL |
|---|---|---|---|
| **Azure con internet** | Azure DevOps → Deploy Slot (zero-downtime) | Migraciones automáticas en startup | Managed Certificate + Key Vault |
| **On-premise con internet** | Agente self-hosted o Azure Arc | Ejecución vía script PowerShell | Let's Encrypt o CA interna |
| **On-premise sin internet** | USB/NAS cifrado con paquete NuGet + script | Script SQL versionado manual | Certificado auto-firmado CA distribuido por GPO |

### Solución técnica: migraciones de base de datos versionadas

```csharp
// FluentMigrator - funciona sin EF Core
[Migration(20250101)]
public class AddSecurityPatch : Migration
{
    public override void Up()
    {
        Execute.Sql(@"
            ALTER TABLE Users ADD MFAEnabled BIT NOT NULL DEFAULT 0;
            UPDATE Users SET MFAEnabled = 1 WHERE Role = 'Admin';
        ");
    }
    public override void Down() { }
}
```

```mermaid
graph LR
    subgraph "Cliente On-Premise Air-Gap"
        USB["USB / NAS<br/>cifrado BitLocker"] -->|"Ejecuta"| PS["PowerShell Update.ps1"]
        PS -->|"1. Detiene servicio"| IIS
        PS -->|"2. Ejecuta migraciones"| BD
        PS -->|"3. Reemplaza binarios"| IIS
        PS -->|"4. Inicia servicio"| IIS
    end
```

### Vista de ingeniería

Migrar la arquitectura a **contenedores** (Docker + K3s para on-premise ligero) para que el despliegue sea idéntico en todos los entornos. Usar **GitOps** (Flux/ArgoCD) para entornos con conectividad y distribuir imágenes Docker comprimidas en tarball para entornos air-gapped. Centralizar la telemetría con **Azure Arc** para tener visibilidad de todos los clientes sin importar su infraestructura.

---

## Pregunta 5: Kiosko con transacciones incompletas por intermitencia de red

### Estrategia: Saga Pattern + Idempotency Key + Offline-First

```mermaid
sequenceDiagram
    participant U as Usuario
    participant K as Kiosko
    participant Q as Cola Local<br/>(SQLite)
    participant CRM as CRM API
    participant DB as BD Kiosko

    U->>K: Inicia trámite
    K->>K: Genera IdempotencyKey (GUID)
    
    loop Reintento hasta completar
        K->>Q: Guarda estado PASO_1_INICIADO
        K->>CRM: Valida datos (con retry)
        CRM-->>K: OK
        K->>Q: Actualiza estado PASO_2_VALIDADO
        
        K->>K: Escanea documentos
        K->>DB: Guarda documentos localmente
        K->>Q: Actualiza estado PASO_3_DOCUMENTOS
        
        K->>CRM: Crear caso con IdempotencyKey
        CRM-->>K: Número de radicado
        K->>Q: Actualiza estado PASO_4_COMPLETADO
    end

    K->>U: Muestra radicado
    K->>Q: Limpia sesión
```

### Componentes técnicos

| Componente | Tecnología | Propósito |
|---|---|---|
| **Idempotency Key** | `Guid` generado al inicio del trámite | El CRM rechaza duplicados con la misma key |
| **Cola de persistencia local** | SQLite embebido en el kiosko | Almacena el estado del trámite ante caídas de red |
| **Saga Orchestrator** | Máquina de estados con pasos compensables | Si falla el paso final, reintenta; si falla uno intermedio, compensa |
| **Outbox Pattern** | Tabla local `Outbox` con eventos | Cuando hay red, drena los eventos pendientes al CRM |

### Solución técnica: máquina de estados

```csharp
public enum TramiteStep
{
    INICIADO,
    VALIDADO_CRM,
    DOCUMENTOS_ESCANEADOS,
    CASO_CREADO,
    RADICADO_ENTREGADO
}

public class TramiteOrchestrator
{
    private readonly ILocalQueue _queue;
    private readonly ICrmClient _crm;
    
    public async Task<TramiteResult> ExecuteAsync(TramiteRequest request)
    {
        var key = Guid.NewGuid();
        
        // Verificar si existe un trámite en progreso
        var existing = await _queue.GetByKeyAsync(key);
        if (existing != null)
            return await ResumeAsync(existing); // Reanudar
        
        return await StartNewAsync(request, key);
    }
    
    private async Task<TramiteResult> ResumeAsync(TramiteState state)
    {
        switch (state.CurrentStep)
        {
            case TramiteStep.VALIDADO_CRM:
                return await EscanearDocumentos(state);
            case TramiteStep.DOCUMENTOS_ESCANEADOS:
                return await CrearCasoCRM(state);
            // ...
        }
    }
}
```

### Vista de ingeniería

```mermaid
flowchart LR
    subgraph "Kiosko (offline-first)"
        K1["SQLite Local"]
        K2["Cola de eventos"]
    end
    subgraph "Cloud"
        C1["CRM API"]
        C2["Event Grid"]
        C3["Base central"]
    end
    K1 --- K2
    K2 -->|"Sincroniza<br/>cuando hay red"| C2 --> C1 --> C3
```

Implementar **Offline-first architecture** donde el kiosko funciona completamente desconectado y sincroniza en segundo plano cuando recupera conectividad. Usar **Azure IoT Hub** para gestionar los dispositivos y **Event Grid** para el enrutamiento de eventos. La **Idempotency Key** es la pieza clave: al reanudar un trámite, el kiosko retoma desde el último paso persistido localmente, y el CRM rechaza duplicados porque ya procesó esa key.
