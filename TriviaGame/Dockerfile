FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY TriviaGame/TriviaGame.csproj TriviaGame/
COPY Services/Services.csproj Services/
COPY CoreLibrary/CoreLibrary.csproj CoreLibrary/
COPY domain/Domain.csproj domain/
RUN dotnet restore TriviaGame/TriviaGame.csproj

COPY . .
RUN dotnet publish TriviaGame/TriviaGame.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
EXPOSE 8080
COPY --from=build /app .
ENTRYPOINT ["dotnet", "TriviaGame.dll"]
