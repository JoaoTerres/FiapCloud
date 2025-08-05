# Etapa 1: Build da aplicação
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

# Copia todo o restante do código
COPY . .

# Publica a aplicação
WORKDIR /src/FiapCloud.Api
RUN dotnet publish -c Release -o /app/publish

# Etapa 2: Runtime com agente New Relic
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

# Instala o agente New Relic
RUN apt-get update && apt-get install -y wget ca-certificates gnupg \
    && echo 'deb [signed-by=/usr/share/keyrings/newrelic-apt.gpg] http://apt.newrelic.com/debian/ newrelic non-free' | tee /etc/apt/sources.list.d/newrelic.list \
    && wget -O- https://download.newrelic.com/NEWRELIC_APT_2DAD550E.public | gpg --import --batch --no-default-keyring --keyring /usr/share/keyrings/newrelic-apt.gpg \
    && apt-get update \
    && apt-get install -y newrelic-dotnet-agent \
    && rm -rf /var/lib/apt/lists/*

# Configura as variáveis de ambiente para ativar o agente
ENV CORECLR_ENABLE_PROFILING=1 \
    CORECLR_PROFILER={36032161-FFC0-4B61-B559-F6C5D41BAE5A} \
    CORECLR_NEWRELIC_HOME=/usr/local/newrelic-dotnet-agent \
    CORECLR_PROFILER_PATH=/usr/local/newrelic-dotnet-agent/libNewRelicProfiler.so

# Essas variáveis são definidas no Azure App Service (não aqui):
# ENV NEW_RELIC_LICENSE_KEY=<SUA_LICENÇA>
# ENV NEW_RELIC_APP_NAME=<NOME_DA_APP>

# Define a pasta de trabalho e copia os arquivos publicados
WORKDIR /app
COPY --from=build /app/publish .

# Expõe a porta 8080 (caso use no Azure ou localmente)
EXPOSE 8080

# Inicia a aplicação
ENTRYPOINT ["dotnet", "FiapCloud.Api.dll"]
