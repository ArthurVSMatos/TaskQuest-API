# Estágio de Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia e restaura dependências
COPY *.csproj ./
RUN dotnet restore

# Copia o código restante e faz o publish
COPY . ./
RUN dotnet publish -c Release -o /app/out

# Estágio de Execução (Runtime)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/out .

# Define a porta padrão de execução
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "TaskQuest.API.dll"]