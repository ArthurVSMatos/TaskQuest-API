# =========================
# BUILD
# =========================

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /src

COPY *.csproj ./

RUN dotnet restore

COPY . .

RUN dotnet publish -c Release -o /app/publish


# =========================
# RUNTIME
# =========================

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final

WORKDIR /app

# Desabilita o monitoramento de alterações dos arquivos
# de configuração em produção.
ENV DOTNET_HOSTBUILDER__RELOADCONFIGONCHANGE=false

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "TaskQuest.API.dll"]