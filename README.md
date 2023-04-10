# Mientreno

Controla f�cilmente tus entrenamientos.

## Configuraci�n

### Base de datos

En la configuraci�n del programa, a�adir una Connection String con nombre `Database`. Por ejemplo, en un `secrets.json` ser�a:

```json
{
	/* ... */
	"ConnectionStrings:Database": "Data Source=EJEMPLO;Initial Catalog=Mientreno;Integrated Security=True; Trust Server Certificate=True"
}
```

### Correo electr�nico

Por ahora, solo se puede configurar Azure Email Communication Services como sistema de env�o de correo. Por tanto, hay que configurar la cadena de conexi�n `AzureEmailCS` con la ConnectionString que proporciona Azure; y el valor `EmailFrom` con el correo electr�nico que se usar� como remitente. Por ejemplo, en un `secrets.json`, ser�a tal que:

```json
{
	/* ... */
	"ConnectionStrings:AzureCS": "CADENA QUE TE PROPORCIONA AZURE COMMUNICATION SERVICES",
	"EmailFrom": "ladireccionqueconfigureenazure@midominioconfigurado.com"
}
```

## Desarrollo

### Base de datos de desarrollo

Ejecutar con:

```bash
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=abc1234." -p 1433:1433 -d mcr.microsoft.com/mssql/server:2022-latest
```

### Cadena de conexi�n
```bash
Server=localhost;Database=Mientreno;User Id=sa;Password=abc1234.;Trust Server Certificate=True;
```

### Migraciones
```bash
dotnet ef database update --project Server
```
