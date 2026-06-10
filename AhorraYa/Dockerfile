# Etapa de compilación
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Copiar los archivos de solución y restaurar las dependencias
COPY *.sln ./
COPY AhorraYa.WebApi/*.csproj ./AhorraYa.WebApi/
# Si tu API depende de tu capa de datos (ej: DbDataAccess), copiala también acá abajo:
# COPY AhorraYa.DbDataAccess/*.csproj ./AhorraYa.DbDataAccess/

RUN dotnet restore

# Copiar todo el código restante y compilar
COPY . ./
RUN dotnet publish AhorraYa.WebApi/*.csproj -c Release -o out

# Etapa de ejecución
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/out .

# Exponer el puerto que usa Render por defecto
ENV ASPNETCORE_URLS=http://+:10000
EXPOSE 10000

ENTRYPOINT ["dotnet", "AhorraYa.WebApi.dll"]