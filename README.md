# PonteFit

## Run database for development

```bash
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=abc1234." -p 1433:1433 -d mcr.microsoft.com/mssql/server:2022-latest

dotnet ef database update --project Server
```