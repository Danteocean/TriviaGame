using Domain.Querys.Interface;
using infrastructure;
using Microservice.core;

var builder = WebApplication.CreateBuilder(args);

// Cargar controllers
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Core
builder.Services.AddCoreLayer();

// Infraestructura: EF + repositorios
builder.Services.AddDbContexts(builder.Configuration);
builder.Services.AddRepository();

builder.Services.AddScoped<IQueryService>(sp =>
new QueryService(builder.Configuration.GetConnectionString("DefaultConnection")
?? throw new InvalidOperationException("Missing DefaultConnection")));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowApp",
        policy =>
        {
            policy.WithOrigins("https://localhost:44300")
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowAnyOrigin();
        });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowApp");
app.UseAuthorization();
app.MapControllers();
app.Run();