# Mini CRM — Seguimiento Comercial por Bernabé Díaz Alvillos

Aplicación full stack para que un asesor administre clientes, registre gestiones y consulte cuándo debe volver a contactarlos.

Desarrollada como prueba técnica para Zoco. Prioriza las funcionalidades obligatorias, reglas de negocio claras y poder ejecutarla desde cero sin SQL Server ni servicios externos.

## Stack

**Backend:** .NET 8 / ASP.NET Core Web API, Entity Framework Core (Code-First), SQLite, Swagger, xUnit + FluentAssertions.

**Frontend:** React 18, Vite, TypeScript, Tailwind CSS, React Router.

## Requisitos previos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js](https://nodejs.org/) 18 o superior
- Git

## Cómo ejecutar
Primero descargar el repo desde: `https://github.com/bernadiaz/PruebaTecnicaFullStackZoco`
Despues dirigirse a la carpeta descargada: cd ./PruebaTecnicaFullStackZoco
Para levantar el proyecto localmente seguir los siguientes pasos:

### Backend

```bash
cd backend
dotnet restore
dotnet run --project src/MiniCrm.Api
```

Al iniciar, la API aplica migraciones, carga el seed si la base está vacía y queda en `http://localhost:5088`.

- Swagger: http://localhost:5088/swagger

### Frontend

En otra terminal:

```bash
cd frontend
npm install 
npm run dev
```

UI: http://localhost:5173. Vite proxea `/api` hacia `http://localhost:5088`.

Opcional:

```bash
VITE_API_URL=http://localhost:5088/api
```

### Base de datos

SQLite. Cadena de conexión en `backend/src/MiniCrm.Api/appsettings.json`. Al arrancar, el API resuelve el archivo contra su Content Root:

`backend/src/MiniCrm.Api/minicrm.db`

No se versiona. En Cursor o Visual Studio no aparece en el árbol porque `*.db` está en `.gitignore`. Se ve en el Explorador de Windows o activando archivos excluidos.

Para regenerar el seed, borrar `minicrm.db` (y `-wal`/`-shm` si existen) y volver a iniciar la API.

### Pruebas

```bash
cd backend
dotnet test
```

Cubren CUIT duplicado (con o sin guiones), actualización de estado y próximo contacto al registrar una gestión, y detección de seguimientos vencidos.

## Estructura

```
backend/
  src/MiniCrm.Api            # Controladores, Swagger, ProblemDetails, Program.cs
  src/MiniCrm.Application    # Servicios, DTOs, validaciones de negocio
  src/MiniCrm.Domain         # Entidades y enums
  src/MiniCrm.Infrastructure # EF Core, migraciones, seed
  tests/MiniCrm.Tests
frontend/
docs/requests.http
```

Los controladores no contienen reglas: delegan en servicios (`ClienteService`, `GestionService`, `DashboardService`, `AsesorService`).

## API

| Método | Ruta | Descripción |
| --- | --- | --- |
| GET | `/api/clientes` | Listado paginado (5 por página). Query: `search`, `estado`, `asesorId`, `page`. Orden por próximo contacto |
| GET | `/api/clientes/{id}` | Detalle. 404 si no existe |
| POST | `/api/clientes` | Alta |
| PUT | `/api/clientes/{id}` | Edición |
| GET | `/api/clientes/{id}/gestiones` | Historial, más reciente primero |
| POST | `/api/clientes/{id}/gestiones` | Nueva gestión y actualización del cliente |
| GET | `/api/dashboard/resumen` | Totales del panel |
| GET | `/api/asesores` | Catálogo para el selector (no hay ABM de asesores) |

### Errores (`ProblemDetails`)

Los errores salen en formato RFC 7807 (`title`, `status`, `detail`, y `errors` si hay campos inválidos):

| HTTP | Cuándo |
| --- | --- |
| 400 | Validación (nombre/CUIT vacíos, CUIT con formato inválido, email inválido, estado no permitido) |
| 404 | Cliente inexistente |
| 409 | CUIT duplicado (también si cambia el formato: `20-28333444-5` o `20283334445`) |

Ejemplo 409:

```json
{
  "title": "Conflicto",
  "status": 409,
  "detail": "Ya existe un cliente con el mismo CUIT.",
  "instance": "/api/clientes"
}
```

## Decisiones técnicas

- Arquitectura en capas (`Api` / `Application` / `Domain` / `Infrastructure`) para separar transporte, negocio y persistencia, sin Clean Architecture pesada.
- SQLite para clonar y correr sin instalar un motor de base.
- Asesor como catálogo (`Asesores`) + `GET /api/asesores`, no texto libre. No hay alta/edición de asesores.
- CUIT visible como lo escribe el usuario; unicidad sobre `CuitNormalizado` (solo dígitos).
- Gestiones de solo inserción: el historial no se edita ni se borra. Al registrar una, se actualizan estado, próximo contacto (si vino) y fecha de actualización del cliente.
- Seguimiento vencido: `ProximoContacto` anterior a hoy. Hoy no cuenta como vencido.
- `ProximoContacto` no se carga en el alta/edición del cliente: solo cambia al registrar una gestión.

## Funcionalidades

### Completadas 

#### Obligatorias

- Listado con búsqueda (nombre, CUIT, teléfono), filtro por estado y por asesor, orden por próximo contacto y paginación de 5 registros
- Alerta visual de seguimientos vencidos
- Alta y edición de clientes
- Registro de gestiones e historial cronológico
- Panel: total, prospectos, interesados, vencidos
- Seed (7 clientes, 4 asesores, gestiones y al menos un vencido)
- Tests de reglas de negocio
- Swagger y `docs/requests.http`

#### Opcionales
- Paginación
- Filtro de Clientes por Asesor responsable

### Pendientes (opcionales)

- Autenticación, Kanban, edición/baja de gestiones, Docker / deploy

## Problemas conocidos

- La API corre por HTTP en el puerto 5088 para no pedir el certificado de desarrollo.
- El seed solo corre si la base está vacía.

## Uso de inteligencia artificial

- **Herramientas:** Cursor (agente de código).
- **Para qué se usó:** scaffolding de la solución, boilerplate, formularios React y redacción del README.
- **Partes asistidas:** estructura de proyectos, DTOs, seed de prueba y estilos Tailwind.
- **Revisión personal:** modelo de datos, reglas de CUIT y gestiones, contratos de la API, validaciones, pruebas, zona horaria y flujo de pantallas.
