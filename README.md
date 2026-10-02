# Mini CRM — Seguimiento Comercial

Aplicación full stack para que un asesor administre clientes, registre gestiones y consulte cuándo debe volver a contactarlos.

Fue desarrollada como prueba técnica para Zoco, priorizando las funcionalidades obligatorias, reglas de negocio claras y una ejecución simple desde cero.

## Stack

### Backend

- .NET 8 / ASP.NET Core Web API
- Entity Framework Core (Code-First)
- SQLite
- Swagger / OpenAPI
- xUnit + FluentAssertions

### Frontend

- React 18 + Vite
- TypeScript
- Tailwind CSS
- React Router

## Requisitos previos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js](https://nodejs.org/) 18 o superior
- Git

No hace falta instalar SQL Server. La base SQLite se crea sola al iniciar la API.

## Instalación y ejecución

### 1. Clonar el repositorio

```bash
git clone <URL_DEL_REPOSITORIO>
cd <NOMBRE_DEL_REPOSITORIO>
```

### 2. Backend

```bash
cd backend
dotnet restore
dotnet run --project src/MiniCrm.Api
```

Al iniciar, la API:

- aplica las migraciones
- carga el seed si la base está vacía
- queda disponible en `http://localhost:5088`

Swagger: [http://localhost:5088/swagger](http://localhost:5088/swagger)

La cadena de conexión está en `backend/src/MiniCrm.Api/appsettings.json`:

```json
"ConnectionStrings": {
  "Default": "Data Source=minicrm.db"
}
```

El archivo `minicrm.db` se genera en la carpeta de ejecución de la API y no se versiona.

### 3. Frontend

En otra terminal:

```bash
cd frontend
npm install
npm run dev
```

La UI queda en [http://localhost:5173](http://localhost:5173). Vite proxea `/api` hacia `http://localhost:5088`.

Si se quiere apuntar a otra URL:

```bash
VITE_API_URL=http://localhost:5088/api
```

## Pruebas

```bash
cd backend
dotnet test
```

Las pruebas cubren:

- rechazo de CUIT duplicado, aunque el formato cambie (`20-28333444-5` vs `20283334445`)
- actualización del estado del cliente al registrar una gestión
- actualización de la fecha de próximo contacto
- detección de seguimientos vencidos

## API

| Método | Ruta | Descripción |
| --- | --- | --- |
| GET | `/api/clientes` | Listado con `search` y `estado`. Ordena por próximo contacto |
| GET | `/api/clientes/{id}` | Detalle. 404 si no existe |
| POST | `/api/clientes` | Alta con validación de CUIT |
| PUT | `/api/clientes/{id}` | Edición |
| GET | `/api/clientes/{id}/gestiones` | Historial, más reciente primero |
| POST | `/api/clientes/{id}/gestiones` | Nueva gestión y actualización del cliente |
| GET | `/api/dashboard/resumen` | Totales del panel |
| GET | `/api/asesores` | Catálogo para el selector |

Errores en formato `ProblemDetails` (RFC 7807): 400 validación, 404 no encontrado, 409 CUIT duplicado.

## Decisiones técnicas

- Arquitectura en 3 capas (`Api`, `Application`, `Domain`, `Infrastructure`). Los controladores no contienen reglas de negocio.
- SQLite para que el evaluador pueda clonar y correr sin servicios externos.
- El asesor es un catálogo (`Asesores`) y no texto libre, para que el alta/edición use un selector. No hay ABM de asesores.
- El CUIT se guarda como lo escribe el usuario y se normaliza a dígitos (`CuitNormalizado`) para el índice único.
- Las gestiones son solo inserción. Nunca se editan ni se borran.
- Un seguimiento está vencido si `ProximoContacto` es anterior a la fecha de hoy. El día de hoy no cuenta como vencido.
- `ProximoContacto` no se carga en el alta/edición del cliente: solo cambia al registrar una gestión.

## Funcionalidades completadas

- Listado de clientes con búsqueda, filtro por estado y orden por próximo contacto
- Alerta visual de seguimientos vencidos
- Alta y edición de clientes
- Registro de gestiones e historial cronológico
- Panel de indicadores
- Seed inicial (7 clientes, 4 asesores, gestiones y al menos un vencido)
- Tests de reglas de negocio
- Swagger

## Funcionalidades pendientes

Quedaron fuera a propósito, porque el enunciado las marca como opcionales:

- Paginación
- Autenticación y roles
- Kanban
- Edición o baja de gestiones
- Docker / deploy

## Problemas conocidos

- La API se expone por HTTP en el puerto 5088 para evitar el paso extra del certificado de desarrollo.
- El seed solo corre cuando la base está vacía. Si se quiere regenerar, hay que borrar `minicrm.db` y volver a iniciar la API.

## Uso de inteligencia artificial

- **Herramientas:** Cursor (agente de código).
- **Para qué se usó:** scaffolding de la solución, generación de boilerplate, formularios React y redacción del README.
- **Partes asistidas:** estructura de proyectos, mapeo de DTOs, seed de datos de prueba y estilos Tailwind.
- **Revisión personal:** modelo de datos, reglas de CUIT/gestiones, contratos de la API, validaciones, pruebas y flujo de pantallas.

Durante la presentación se puede explicar cualquier parte del código entregado.
