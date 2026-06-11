# 1. Etapa de compilación (SDK de .NET 8)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiamos los archivos del proyecto y restauramos las dependencias
COPY ["AhorraYa.WebClient.csproj", "./"]
RUN dotnet restore "./AhorraYa.WebClient.csproj"

# Copiamos el resto del código y lo compilamos en modo Release
COPY . .
RUN dotnet publish "AhorraYa.WebClient.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 2. Etapa de ejecución (Runtime de .NET 8)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Copiamos lo compilado desde la etapa anterior
COPY --from=build /app/publish .

# Comando para arrancar la aplicación
ENTRYPOINT ["dotnet", "AhorraYa.WebClient.dll"]