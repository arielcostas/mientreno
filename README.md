# Mientreno

Controla fácilmente tus entrenamientos.

## Historia

Este proyecto fue desarrollado por Ariel Costas Guerrero como proyecto final de ciclo superior de Desarrollo de
Aplicaciones Multiplataforma (DAM) en el IES A Pinguela, Monforte de Lemos, Lugo. El proyecto fue presentado y evaluado
en junio de 2023.
En enero de 2025 decidió reabrir el proyecto, actualizarlo a .NET 8 y empezar a trabajar en mejoras y nuevas
funcionalidades.

## Arquitectura

El proyecto está implementado en .NET 8 y utiliza una arquitectura monolítica, con planes para separar algunos
componentes (como los "workers" de generación de perfiles y envío de correos). Para la base de datos se utiliza MySQL.
Para la comunicación entre servicios se utiliza RabbitMQ con el SDK oficial (se planea cambiar a MassTransit en el
futuro). Para el almacenamiento de imágenes se utiliza el sistema de archivos local (se planea cambiar a Azure Blob y
S3-compatible en el futuro).

## Desarrollo local

Para ejecutar el proyecto en local, se necesita tener instalado .NET 8 y Docker, con soporte para Docker Compose. Se
ejecuta el comando `docker compose up -d` en la raíz del proyecto para levantar la base de datos, el servidor RabbitMQ
y el servidor de email (para desarrollo). Luego se ejecuta el proyecto con `dotnet run --project Server`.

## Contribuciones

Las contribuciones son bienvenidas. Por favor, crea un fork del proyecto, haz tus cambios y abre un pull request. Si
quieres abrir un issue, por favor, asegúrate de que no esté ya abierto.

## Licencia

Este proyecto está bajo la licencia GNU Affero General Public License v3.0. Puedes leer la licencia en el archivo LICENSE
que se incluye en el proyecto. Esencialmente, puedes usar, modificar y distribuir el proyecto como quieras, pero si
lo distribuyes o alojas como servicio de red (el caso de uso principal de este proyecto), debes hacer públicos los
cambios que hagas al código fuente así como el código original.

Este software se ofrece tal cual, sin garantías de ningún tipo. El autor no se hace responsable de ningún daño que pueda
causar el uso de este software. Si usas este software, aceptas estos términos.
