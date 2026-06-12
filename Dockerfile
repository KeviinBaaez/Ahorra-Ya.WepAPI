# 1. Etapa de compilación (Web API)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiamos todo el contenido del repositorio
COPY . .

# Restauramos las dependencias buscando cualquier .csproj dentro de la carpeta de la API
RUN dotnet restore "AhorraYa/"

# Compilamos y publicamos el proyecto de la API en modo Release
RUN dotnet publish "AhorraYa.WebApi.csproj/" -c Release -o /app/publish /p:UseAppHost=false

# 2. Etapa de ejecución (Runtime de .NET 8)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Copiamos el resultado compilado
COPY --from=build /app/publish .

# Arrancamos dinámicamente el archivo .dll que se haya generado para la API
ENTRYPOINT ["sh", "-c", "dotnet $(ls AhorraYa*.dll | head -n 1)"]