# 1. Etapa de compilación (SDK de .NET 8)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiamos el archivo de proyecto apuntando a la carpeta exacta
COPY ["AhorraYa.Client/AhorraYa.Client.csproj", "AhorraYa.Client/"]
RUN dotnet restore "AhorraYa.Client/AhorraYa.Client.csproj"

# Copiamos absolutamente todo el monorepo
COPY . .
WORKDIR "/src/AhorraYa.Client"
RUN dotnet publish "AhorraYa.Client.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 2. Etapa de ejecución (Runtime de .NET 8)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

COPY --from=build /app/publish .

# Comando de arranque apuntando a la DLL del cliente
ENTRYPOINT ["dotnet", "AhorraYa.Client.dll"]