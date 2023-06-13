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

### Scaleway (Server)

Datos de Scaleway.

```json5
{
  "Scaleway": {
    "AccessKey": "",
    "SecretKey": "",
    "ProjectId": ""
  }
}
```

### RabbitMQ (Server)

```json5
{
	"ConnectionStrings:RabbitMQ": "amqp://guest:guest@localhost:5672/"
}
```

### Guardado de imágenes local (Server)

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

### Workers

Si ejecutar o no los workers de fondo que corren en segundo plano.

```json5
{
	"Workers": {
		"RunProfileGenerator": true,
		"RunEmailSender": true
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
