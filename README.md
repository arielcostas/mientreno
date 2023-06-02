# Mientreno

Controla fácilmente tus entrenamientos.

## Configuración de apps (appsettings.json, user-secrets...)

### Base de datos (Server)

En la configuración del programa, añadir una Connection String con nombre `Database`. Por ejemplo, en un `secrets.json`
sería:

```json5
{
	"ConnectionStrings:Database": "Data Source=EJEMPLO;Initial Catalog=Mientreno;Integrated Security=True; Trust Server Certificate=True"
}
```

### Correo electrónico (QueueWorker)

Por ahora, solo se puede configurar Azure Email Communication Services como sistema de envío de correo. Por tanto, hay
que configurar la cadena de conexión `AzureEmailCS` con la ConnectionString que proporciona Azure; y el
valor `EmailFrom` con el correo electrónico que se usará como remitente. Por ejemplo, en un `secrets.json`, sería tal
que:

```json5
{
	"ConnectionStrings:AzureCS": "CADENA QUE TE PROPORCIONA AZURE COMMUNICATION SERVICES",
	"EmailFrom": "ladireccionqueconfigureenazure@midominioconfigurado.com"
}
```

### RabbitMQ (Server y QueueWorker)

```json5
{
	"ConnectionStrings:RabbitMQ": "amqp://guest:guest@localhost:5672/"
}
```

### Monitorización con Sentry (Server y QueueWorker)

```json5
{
	"ConnectionStrings:Sentry": "CADENA DE CONEXION DE SENTRY.IO"
}
```

### Guardado de imágenes local (Server y QueueWorker)

```json5
{
	"FileBase": "C:\\Users\\you\\Desktop\\MientrenoPics"
}
```

### Procesador de pagos (Server)

```json5
{
	"Stripe": {
		"Publishable": "sk_test_51N0V4dKk3b9U3IGl6anM7TsSjZLMJTdmyjgKo7Ucn83XKMkJKIhnX29sQ1F8BwERf2oColu165b9oWGBJazYjo0w00NjffAixa",
		"Secret": "sk_test_51N0V4dKk3b9U3IGl6anM7TsSjZLMJTdmyjgKo7Ucn83XKMkJKIhnX29sQ1F8BwERf2oColu165b9oWGBJazYjo0w00NjffAixa",
		"Webhook": ""
	}
}
```

## Desarrollo

### Base de datos de desarrollo

Ejecutar con:

```sh
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=abc1234." -p 1433:1433 -d mcr.microsoft.com/mssql/server:2022-latest
```

### Cadena de conexión

```
Server=localhost;Database=Mientreno;User Id=sa;Password=abc1234.;Trust Server Certificate=True;
```

### Migraciones

```sh
dotnet ef database update --project Server
```
