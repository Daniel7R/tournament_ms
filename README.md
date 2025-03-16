# TournamentMs
Este proyecto gestiona todo el tema de administracion de torneos, como son la obtencion, creacion de torneos/partidos, cambio de fechas de los torneos/partidos, creacion de roles, cambio de estado del torneo. 

## Requisitos Previos
- .NET 8 SDK
- PostgreSQL
- RabbitMQ Server


## Estructura

- **API/Controllers/**: Contiene los controladores de la API.
- **Domain/Entities/**: Contiene los modelos o entidades de datos.
- **Application/Services/**: Contiene los servicios de la aplicación.
- **Infrastructure/Data/**: Contiene el contexto de la base de datos.
- **Infrastructure/EventBus/**: Contiene la config del event bus/rabbitmq.
- **Program.cs**: Punto de entrada del proyecto.

# Instrucciones de Ejecución
Para ejecutar el proyecto UsersAuthorization, sigue estos pasos:

- Asegúrate de tener una base de datos PostgreSQL en funcionamiento.
- Configura las variables de entorno necesarias o modifica los archivos appsettings.json, según sea necesario.
- Navega al directorio del proyecto UsersAuthorization.
- Ejecuta el siguiente comando para aplicar las migraciones de la base de datos: `dotnet ef database update`
- Ejecuta el siguiente comando para iniciar el proyecto: `dotnet run`

Esto iniciará el proyecto y estará listo para poder ser usado.

Generar Documentación con Swagger
Swagger automáticamente genera la documentación de la API. Para ver la documentación generada, inicia la aplicación y navega a http://localhost:<puerto>/swagger.

En la solucion, se hace un metodo de extension el cual va a implementar la validacion de los tokens para endpoints protegidos, los tokens se generan en el MS de Usuarios y Autorizacion, llamando al metodo de inicio de sesion, son los tokens validos dentro del sistema, en caso de que no sea proporcionado, se rechaza la peticion con un codigo de estado `401`.
