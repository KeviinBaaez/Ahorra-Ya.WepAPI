# 1. Etapa de compilación (API)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY . .
RUN dotnet restore "AhorraYa/AhorraYa.csproj"

WORKDIR "/src/AhorraYa"
RUN dotnet publish "AhorraYa.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 2. Etapa de ejecución
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "AhorraYa.dll"]