# Bruno Vehicle Hire

My submission for the intermediate level movement assessment. It implements the
Vehicle entity end to end: a .NET 10 Web API behind Clean Architecture, and a
React + TypeScript frontend that consumes it.

I picked Vehicles over Customers because the vehicle rules gave me more to
demonstrate: a unique registration number I can enforce at database level, and
soft delete, which means the read path has to actively exclude deleted rows.

## What it does

- Paginated list of vehicles
- Look up a single vehicle by registration number
- Add a vehicle
- Edit make / model / year
- Soft delete (sets `IsDeleted`, row stays in the table)

Registration numbers are unique, normalised to uppercase on the way in, and
soft-deleted vehicles disappear from both the list and the registration lookup.
Every endpoint needs an `X-API-Key` header.

## How it is put together

Four projects under `src/`, plus one test project:

| Project | Holds |
| --- | --- |
| `Domain` | The `Vehicle` entity and the repository interface. No packages, no EF Core. |
| `Application` | Commands, queries, handlers, validators, DTOs. MediatR + FluentValidation. |
| `Infrastructure` | EF Core `DbContext`, SQLite, the repository implementation, migrations. |
| `Api` | Controllers, API key auth, OpenAPI, exception handling, DI wiring. |

References only ever point inward:

```
Domain  <-  Application  <-  Infrastructure
                         <-  Api
```

Domain has no project references at all, which is the check I would run first if
someone asked me to prove the layering. `Api` is the composition root: it is the
only project that knows about HTTP, configuration, and how everything is wired.

Controllers do no business work. They build a command or query, hand it to
MediatR, and turn the result into a status code. Validation runs in a MediatR
pipeline behaviour, so it protects the use case rather than just the controller
model.

The frontend is Vite + React + TypeScript, split into `models/`, `services/`,
`components/`, `pages/` and `routes/`. Pages call services, services call one
shared HTTP client. No component contains a URL.

## Running it locally

You need the .NET 10 SDK, Node with npm, and one API key string that you will
use in two places (the API and the frontend must match).

Everything below runs from this folder.

**1. Set the API key for the API.**

```powershell
dotnet user-secrets set "Security:ApiKey" "your-local-api-key" --project src/BrunoVehicleHire.Api
```

It has to be user secrets or an environment variable. There is no key in
`appsettings.json` and no default to fall back on, so if you skip this step the
API still starts, but every request comes back 401 and the log records a
critical message saying the key is not configured.

**2. Create the database.**

```powershell
dotnet ef database update --project src/BrunoVehicleHire.Infrastructure --startup-project src/BrunoVehicleHire.Api
```

SQLite, so this just creates `bruno-vehicle-hire.db` inside the API project
folder. Delete that file and re-run this command if you want a clean slate. If
`dotnet ef` is missing: `dotnet tool install --global dotnet-ef --version 10.0.9`.

**3. Start the API.**

```powershell
dotnet run --project src/BrunoVehicleHire.Api --launch-profile https
```

That listens on `https://localhost:7027` (and `http://localhost:5269`). Swagger
UI is at `https://localhost:7027/swagger` in Development only. You can authorise
in Swagger with the same key from step 1.

**4. Create the frontend environment file before starting it.**

`frontend/.env.local`:

```env
VITE_API_BASE_URL=https://localhost:7027
VITE_API_KEY=your-local-api-key
```

There is a `.env.example` to copy. This step comes before `npm run dev` on
purpose: the frontend validates both variables when it loads and throws
immediately if either is missing, rather than silently sending `undefined` and
giving you a confusing 401.

**5. Start the frontend.**

```powershell
cd frontend
npm install
npm run dev
```

Then open `http://localhost:5173`.

## Endpoints

```
GET    /api/vehicles?pageNumber=1&pageSize=10
GET    /api/vehicles/registration/{registrationNumber}
POST   /api/vehicles
PUT    /api/vehicles/{id}
DELETE /api/vehicles/{id}
```

Lookup is by registration number because the brief asks for it and because that
is the value a user actually knows. Update and delete take the `Id`, since the
Id is the stable identity and the registration number cannot change after
create. Delete returns 204 and soft deletes. A duplicate registration on POST
returns 409 with a problem-details body.

Failures come back as `application/problem+json`: 400 for validation (with the
field errors), 404 for a missing vehicle, 409 for a duplicate registration.

## Tests and checks

```powershell
dotnet build --configuration Release
dotnet test
dotnet format BrunoVehicleHire.slnx --verify-no-changes --no-restore
```

```powershell
cd frontend
npm run lint
npm run build
```

The test project covers the create command handler (success and duplicate) and
the paginated query handler, using a hand-written in-memory repository rather
than a mocking library. I wanted the test double to be something I could read.

## Decisions worth explaining

**SQLite instead of SQL Server.** The brief allows any SQL database. SQLite
means anyone can clone this and run it without installing a server, and code
first migrations still demonstrate the same thing. The provider is only
referenced in `Infrastructure`, so switching is a one-line change plus a new
migration.

**Soft delete via a global query filter.** `HasQueryFilter` on the entity means
every normal query excludes deleted rows automatically, instead of me
remembering a `Where` clause in each repository method. The one place that
deliberately bypasses it is the duplicate-registration check, which uses
`IgnoreQueryFilters()`, because the unique index covers the whole table
including deleted rows. If it did not bypass the filter, re-using a deleted
plate would pass the check and then blow up on the insert.

**Both an application check and a database constraint.** The handler checks
first so the user gets a clean 409, and the unique index is the real guarantee.
The check is for the message, the index is for correctness.

**`fetch` rather than Axios.** Everything I needed from Axios here (base URL,
default headers, error normalisation) is about fifteen lines in
`services/apiClient.ts`, and it saves a dependency. I would use Axios if I
needed interceptors or retries.

**No state management library.** There is no shared state across pages. Each
page owns what it fetches, and the only cross-page state (which vehicle is being
edited) lives in the URL, which is also what makes the edit page survive a
refresh.

## Known gaps

Things I know about and would fix next, in order:

1. **Concurrent duplicate creates.** Two simultaneous POSTs with the same
   registration can both pass the existence check. The database unique index
   still stops the bad write, so the data stays correct, but the second request
   currently surfaces as a 500 instead of a 409. The fix is to translate that
   specific constraint failure at the persistence boundary.
2. **The API key is not a real secret on the frontend.** Vite substitutes
   `VITE_API_KEY` into the bundle at build time, so anyone can read it in
   devtools. It satisfies the header-based API key requirement and blocks casual
   unauthenticated calls, but it cannot identify a user. A real design would use
   proper user authentication, or a backend-for-frontend holding the secret
   server side. CORS does not help here either, since it only constrains
   browsers.
3. **No frontend tests.** `validateVehicleForm` is a pure function and would be
   the obvious first one.
4. **No integration tests.** The unit tests prove the handlers. They do not
   prove the auth handler, the 401 path, the problem-details mapping, or the
   database constraint. One `WebApplicationFactory` group over SQLite would
   cover all of it.
5. **Delete uses `window.confirm`.** Fine for this, but a real product needs a
   proper focus-trapped dialog.

## Where to look first

- `src/BrunoVehicleHire.Api/Controllers/VehiclesController.cs` for how thin the
  controllers are
- `src/BrunoVehicleHire.Application/Vehicles` for the commands, queries and
  validators
- `src/BrunoVehicleHire.Application/Common/Behaviors/ValidationBehavior.cs` for
  where validation actually runs
- `src/BrunoVehicleHire.Infrastructure/Persistence/BrunoVehicleHireDbContext.cs`
  for the unique index and the soft-delete filter
- `frontend/src/services/apiClient.ts` for the single place HTTP happens
- `frontend/src/pages/VehiclesPage.tsx` for loading, empty, error and data states

## Troubleshooting

**Everything returns 401.** The key in user secrets and the key in
`frontend/.env.local` are different, or the API was started before the secret
was set.

**The frontend cannot reach the API and the browser console mentions the
certificate.** The .NET dev certificate is not trusted. Run
`dotnet dev-certs https --trust`, or start the API with `--launch-profile http`
and change `VITE_API_BASE_URL` to `http://localhost:5269` to match. The two have
to agree or CORS will reject the request.

**The API will not start and complains about the CORS origin.** The allowed
origin is only configured in `appsettings.Development.json`. Running outside
Development needs `Cors__AllowedOrigin` supplied as an environment variable.
Startup fails deliberately rather than defaulting to something permissive.

**The frontend throws on load about a missing environment value.**
`frontend/.env.local` does not exist yet, or is missing one of the two
variables. Vite also needs a restart after that file changes.

**A route 404s after a hard refresh in a deployed build.** `BrowserRouter` needs
whatever serves the built files to fall back to `index.html` for unknown paths.
The Vite dev server does this for you, so it only shows up once it is hosted
somewhere.
