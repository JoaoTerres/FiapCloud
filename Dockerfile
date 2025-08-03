# Etapa 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia os arquivos da solução
COPY *.sln .
COPY FiapCloud.Api/FiapCloud.Api.csproj FiapCloud.Api/
COPY FIapCloud.App/FIapCloud.App.csproj FIapCloud.App/
COPY FiapCloud.Infra/FiapCloud.Infra.csproj FiapCloud.Infra/
COPY FiapCLoud.Domain/FiapCLoud.Domain.csproj FiapCLoud.Domain/
COPY FiapCloud.Tests/FiapCloud.Tests.csproj FiapCloud.Tests/

# Restaura os pacotes
RUN dotnet restore

# Copia todo o restante
COPY . .

# Publica a aplicação
WORKDIR /src/FiapCloud.Api
RUN dotnet publish -c Release -o /app/publish

# Etapa 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "FiapCloud.Api.dll"]
