# Mientreno

Controla fácilmente tus entrenamientos.

## Desarrollo

### Base de datos de desarrollo

Ejecutar con:

```bash
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=abc1234." -p 1433:1433 -d mcr.microsoft.com/mssql/server:2022-latest
```

### Cadena de conexión
```bash
Server=localhost;Database=Mientreno;User Id=sa;Password=abc1234.;Trust Server Certificate=True;
```

### Migraciones
```bash
dotnet ef database update --project Server
```
