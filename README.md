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


## Endpoints Controllers

### Category

- [GET] `/api/v1/categories`: obtiene las categorias disponibles dentro del sistema

***Headers***:
    - **Authorization**: Bearer some-token

***Response***:
```
{
    "result": [
        {
            "idCategory": 0,
            "name": "Carreras",
            "code": "0235",
            "alias": "Racing"
        },
        {
            "idCategory": 0,
            "name": "Estrategia",
            "code": "0236",
            "alias": "Strategy"
        }
    ],
    "message": "Successfully requested"
}
```
### Games

- [GET] `/api/v1/Games`: obtiene los juegos disponibles dentro del sistema

***Headers***:
    - **Authorization**: Bearer some-token

***Response***:
```
{
    "result": [
        {
            "idGame": 0,
            "name": "Need For Speed",
            "players": 10
        },
        {
            "idGame": 0,
            "name": "League Of Legends",
            "players": 10
        }
    ],
    "message": "Successfully requested"
}
```

### Tournament

- [POST] `/api/v1/tournaments`: esta metodo se encarga de crear un nuevo torneo en el sistema, los torneos son gratis o de pago, pero hay una restriccion en el sistema, de que un usuario solo puede crear 1 torneo gratuito como maximo dentro del sistema, para destacar, idGame y idCategory son deben ser validos(la fecha del torneo tambien debe ser valida, no debe ser inferior a la fecha y hora actual)

***Headers***:
    - **Authorization**: Bearer some-token

***Body***:
```
{
    "name": "Need For Speed LEGENDS",
    "description": "This is the Need For Speed tournament",
    "idCategory": 1,
    "idGame": 1,
    "isFree": false,
    "startDate": "2025-03-29T00:27:00.128Z",
    "endDate": "2025-03-29T00:28:00.128Z",
    "prize": {
        "description": "this is the prize",
        "total": 100000
    }
}
```

***Respuesta***:
```
{
    "result": {
        "id": 2,
        "name": "Need For Speed LEGENDS",
        "categoryName": "Carreras",
        "gameName": "Need For Speed",
        "maxPlayers": 10,
        "isFree": false,
        "createdAt": "0001-01-01T00:00:00",
        "startDate": "2025-03-29T00:27:00.128Z",
        "endDate": "2025-03-29T00:28:00.128Z"
    },
    "message": "Tournament successfully created"
}
```

**NOTA**: una vez creado el torneo, se manda un evento de creacion de tickets de participantes, y se envia correo de notificacion de creacion del torneo a todos los usuarios

- [GET] `/api/v1/tournaments`: obtiene los torneos del sistema de acuerdo al estado que se pase como query param (pueden ser multiples estados de torneos), los estados disponibles son un enum con los siguientes estados: `PENDING`, `ONGOING`, `FINISHED` y `CANCELED`

***Params***
    - **statuses**: `{STATUS}`

**URI Example**: `/api/v1/tournaments?statuses=ONGOING&statuses=PENDING`

***Respuesta***: 
```
{
    "result": [
        {
            "id": 2,
            "name": "Need For Speed LEGENDS",
            "categoryName": "Carreras",
            "prizeDescription": "this is the prize",
            "maxPlayers": 10,
            "totalPrize": 100000,
            "gameName": "Need For Speed",
            "isFree": false,
            "startDate": "2025-03-29T00:27:00.128",
            "endDate": "2025-03-29T00:28:00.128"
        },
        {
            "id": 1,
            "name": "Need For Speed",
            "categoryName": "Carreras",
            "prizeDescription": "this is the prize",
            "maxPlayers": 10,
            "totalPrize": 100000,
            "gameName": "Need For Speed",
            "isFree": false,
            "startDate": "2025-03-29T00:27:00.128",
            "endDate": "2025-03-29T00:28:00.128"
        }
    ],
    "message": "Successfully requested"
}
```

- [PATCH] `/api/v1/tournaments/{idTournament}/date`: se usa para cambiar la fecha de un torneo, de acuerdo a un estado proporcionado, solamente si el usuario tiene un rol valido, que seria `ADMIN` o `SUBADMIN`

***URI***: `/api/v1/tournaments/2/date`

***Headers***:
    - **Authorization**: Bearer some-token

***Body***:
```
{
  "startDate": "2025-03-16T22:17:00.128Z",
  "endDate": "2025-03-17T00:28:00.128Z"
}
```

***Respuesta***:
```
{
    "result": null,
    "message": "Tournament dates successfully changed"
}
```


- [PATCH] `/api/v1/tournaments/{idTournament}/status`: se usa para cambiar el estado del torneo por uno nuevo, solamente si el usuario cuenta con un rol valido para hacerlo (`ADMIN`,`SUBADMIN`)

***URI***: `/api/v1/tournaments/2/status`

***Headers***:
    - **Authorization**: Bearer some-token

***Body***:
```
{
  "newStatus": "PENDING"
}
```

***Respuesta***:
```
{
    "result": true,
    "message": "Status changed successfully"
}
```


- [POST] `/api/v1/role/subadmin`: agrega un nuevo subadministrador al torneo, para auidar a administrar el torneo y los eventos subyacentes(si el torneo es gratuito no puede tener mas de 2 partidos)

***Headers***:
    - **Authorization**: Bearer some-token

***Body***:
```
{
  "idUser": 3,
  "idTournament": 4
}
```


***Respuesta***:
```
{
    "result": null,
    "message": "Successfully added"
}
```



### Teams

- [GET] `/api/v1/teams`: obtiene los equipos que hay de acuerdo a un id de torneo que se proporcione por un query param

***Query Param***:
    - `idTournament`: number

***URI***: `/api/v1/teams?idTournament=2`


***Respuesta***:

```
{
    "result": [
        {
            "idTournament": 2,
            "idTeam": 11,
            "teamName": "Team 1",
            "maxMembers": 1,
            "currentMembers": 0,
            "members": []
        },
        {
            "idTournament": 2,
            "idTeam": 12,
            "teamName": "Team 2",
            "maxMembers": 1,
            "currentMembers": 0,
            "members": []
        },
        {
            "idTournament": 2,
            "idTeam": 13,
            "teamName": "Team 3",
            "maxMembers": 1,
            "currentMembers": 0,
            "members": []
        },
        {
            "idTournament": 2,
            "idTeam": 14,
            "teamName": "Team 4",
            "maxMembers": 1,
            "currentMembers": 0,
            "members": []
        },
        {
            "idTournament": 2,
            "idTeam": 15,
            "teamName": "Team 5",
            "maxMembers": 1,
            "currentMembers": 0,
            "members": []
        },
        {
            "idTournament": 2,
            "idTeam": 16,
            "teamName": "Team 6",
            "maxMembers": 1,
            "currentMembers": 0,
            "members": []
        },
        {
            "idTournament": 2,
            "idTeam": 17,
            "teamName": "Team 7",
            "maxMembers": 1,
            "currentMembers": 0,
            "members": []
        },
        {
            "idTournament": 2,
            "idTeam": 18,
            "teamName": "Team 8",
            "maxMembers": 1,
            "currentMembers": 0,
            "members": []
        },
        {
            "idTournament": 2,
            "idTeam": 19,
            "teamName": "Team 9",
            "maxMembers": 1,
            "currentMembers": 0,
            "members": []
        },
        {
            "idTournament": 2,
            "idTeam": 20,
            "teamName": "Team 10",
            "maxMembers": 1,
            "currentMembers": 0,
            "members": []
        }
    ],
    "message": ""
}
```

### Matches

- [POST] `/api/v1/matches`: crea un partido para un torneo, si el usuario tiene un rol valido dentro de l torneo(`ADMIN`, `SUBADMIN`), ademas, se hacen diferentes validaciones con respecto a los equipos del partido, el torneo y los equipos correspondan, ademas, de que la fecha del partido este, dentro del rango de la fecha del torneo


***Headers***:
    - **Authorization**: Bearer some-token

***Body***:
```
{
  "idTournament": 2,
  "name": "match 3",
  "matchDate": "2025-03-17T00:26:29.128Z",
  "idTeams": [
   11,12,13
  ]
}
```

***Respuesta***:
```
{
    "result": {
        "idMatch": 3,
        "name": "match 3",
        "date": "2025-03-17T00:26:29.128Z",
        "matchStatus": "PENDING"
    },
    "message": "Match created"
}
```

- [GET] `/api/v1/matches`: obtiene los partidos por torneo, haciendo uso de un query param con el id del torneo

***Query Param***
    - `idTournament`: number

***URI***: `/api/v1/matches?idTournament=2`

***Respuesta***:
```
{
    "result": [
        {
            "idMatch": 3,
            "name": "match 3",
            "date": "2025-03-17T00:26:29.128",
            "matchStatus": "PENDING"
        }
    ],
    "message": "Successfully requested"
}
```


- [PATCH] `/api/v1/matches/winner`: establece el ganador dentro de un partido, despues de hacer las respectivas validaciones de roles  y reglas dentro del torneo y partido

***Headers***:
    - **Authorization**: Bearer some-token


***Body***:
```
{
  "idWinner": 13,
  "idMatch": 3
}
```

***Respuesta***:
```
{
    "result": true,
    "message": "Winner has been set"
}
```

- [PATCH] `/api/v1/matches/date`: cambia la fecha del partido en caso de ser requerido, haciendo validaciones del rol del usuario y validando que la fecha este entre el rango que dura el torneo

***Headers***:
    - **Authorization**: Bearer some-token

***Body***:
```
{
  "idMatch": 3,
  "date": "2025-03-16T22:46:32.841Z"
}
```

***Respuesta***:
```
{
    "result": true,
    "message": "Date has been changed"
}
```

## RabbitMQ/LavinMQ

### Colas de procesamiento sincrono(request/reply): 

- `match.role.user`: se usa para retornar la validacion de rol del usuario para el torneo o al evento a partcipar

- `tournament.by_id`: obtiene el torneo por id y retorna info basica respecto a este

- `match.belongs.tournament`: para validar que un partido pertenece a un torneo

- `tournament.bulk.names`: retorna los torneos con info por lotes, para evitar hacer uso de peticiones individuales

- `tournament.validate`: valida si el torneo es gratis o no para temas de validacion de tickets y unirse a streams

- `match.info`: retorna info basica sobre el partido/evento para validar su existencia

### Colas de procesamiento asincrono:


- `team.assign`: se usa para procesar el evento de asignacion de miembros de equipo, una vez un cliente realiza la compra de un ticket de tipo participante

- `viewer.role`: asigna el rol de espectador, cuando se realiza la compra de un ticket de este tipo
