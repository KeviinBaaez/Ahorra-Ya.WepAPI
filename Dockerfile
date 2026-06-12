# 1. Etapa de compilación (Web API)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiamos todo el contenido del repositorio
COPY . .

# Restauramos las dependencias de toda la solución
RUN dotnet restore "AhorraYa/"

# Compilamos apuntando al archivo exacto del proyecto (sin la barra al final)
RUN dotnet publish "AhorraYa/AhorraYa.WebApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 2. Etapa de ejecución (Runtime de .NET 8)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Copiamos el resultado compilado
COPY --from=build /app/publish .

# Arrancamos la DLL de la API
ENTRYPOINT ["dotnet", "AhorraYa.WebApi.dll"]