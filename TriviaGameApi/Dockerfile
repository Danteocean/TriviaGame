FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY TriviaGameApi/TriviaGameApi.csproj TriviaGameApi/
COPY CoreLibrary/CoreLibrary.csproj CoreLibrary/
COPY domain/Domain.csproj domain/
COPY infrastructure/infrastructure/infrastructure.csproj infrastructure/infrastructure/
RUN dotnet restore TriviaGameApi/TriviaGameApi.csproj

COPY . .
RUN dotnet publish TriviaGameApi/TriviaGameApi.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
EXPOSE 8080
COPY --from=build /app .
ENTRYPOINT ["dotnet", "TriviaGameApi.dll"]
