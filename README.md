![.NET](https://img.shields.io/badge/.NET-8.0-purple)
![Status](https://img.shields.io/badge/status-finished-success)
![License](https://img.shields.io/badge/license-MIT-green)
# ServiceFlow — Sistema de Gestión de Solicitudes Internas

ServiceFlow es una aplicación web de tipo Help Desk desarrollada con **ASP.NET Core MVC (.NET 8)**, diseñada para gestionar solicitudes internas en organizaciones como empresas, universidades o instituciones públicas. Permite registrar, asignar, dar seguimiento y resolver tickets de soporte de forma estructurada.

---

## Características principales

- Autenticación con ASP.NET Identity + OAuth con Google
- Verificación de correo al registrarse mediante código de 6 dígitos
- Recuperación de contraseña por correo
- Tres roles: `Admin`, `Agent`, `User`
- Gestión completa de solicitudes (crear, editar, eliminar, comentar)
- Filtros por estado, prioridad y categoría
- Paginación en listados
- Panel de control con estadísticas por rol
- Gestión de usuarios y categorías (solo Admin)
- Sidebar de navegación persistente
- Banner de consentimiento de cookies
- Página 404 personalizada

---

## Tecnologías utilizadas

| Capa | Tecnología |
|---|---|
| Backend | ASP.NET Core MVC (.NET 8) |
| ORM | Entity Framework Core (Code First) |
| Base de datos | SQL Server |
| Autenticación | ASP.NET Identity + Google OAuth |
| Email | MailKit + Mailtrap |
| Frontend | Bootstrap 5 + Bootstrap Icons |
| Arquitectura | MVC en capas |

---

## Estructura del proyecto

```text
ServiceFlow/
├── ServiceFlow.Class/                 # Lógica de negocio y acceso a datos
│   ├── Data/                          # DbContext (ServiceFlowDB)
│   ├── Models/                        # Entidades: RequestModel, CategoryModel, etc.
│   └── Repositories/                  # IRepository<T>, Repository<T>, RequestRepository
│
└── ServiceFlow.Web/                   # Capa de presentación
    ├── Controllers/                   # AccountController, RequestController, etc.
    ├── Services/                      # EmailService
    ├── ViewModels/                    # ViewModels por vista
    ├── Views/                         # Vistas Razor organizadas por controlador
    └── wwwroot/                       # CSS, JS, imágenes y archivos estáticos
```
---

## Requisitos previos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8)
- SQL Server (local o remoto)
- Cuenta en [Mailtrap](https://mailtrap.io) (para envío de correos en desarrollo)
- Credenciales de Google OAuth ([Google Cloud Console](https://console.cloud.google.com))

---

## Configuración

### 1. Clonar el repositorio

```bash
git clone https://github.com/floreslog/serviceflow-sistema-de-solicitudes.git
cd serviceflow-sistema-de-solicitudes
```

### 2. Configurar `appsettings.json`

En `ServiceFlow.Web/appsettings.json` reemplaza los valores necesarios:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=TU_SERVIDOR;Database=ServiceFlowDB;Trusted_Connection=True;TrustServerCertificate=True"
  },
  "Authentication": {
    "Google": {
      "ClientId": "TU_GOOGLE_CLIENT_ID",
      "ClientSecret": "TU_GOOGLE_CLIENT_SECRET"
    }
  },
  "EmailSettings": {
    "Host": "sandbox.smtp.mailtrap.io",
    "Port": 587,
    "Username": "TU_MAILTRAP_USERNAME",
    "Password": "TU_MAILTRAP_PASSWORD",
    "FromEmail": "noreply@serviceflow.com",
    "FromName": "ServiceFlow"
  }
}
```

### 3. Aplicar la migración

La migración inicial ya está incluida en el proyecto. Solo ejecuta:

```bash
cd ServiceFlow.Web
dotnet ef database update
```

Esto creará la base de datos y todas las tablas automáticamente, incluyendo los roles `Admin`, `Agent` y `User` que se inicializan al arrancar la aplicación.

---

## Roles del sistema

| Rol | Permisos |
|---|---|
| `Admin` | Acceso total: gestión de usuarios, categorías, solicitudes y asignación de agentes |
| `Agent` | Ver y gestionar solicitudes asignadas, cambiar estado |
| `User` | Crear y dar seguimiento a sus propias solicitudes |

---

## Usuario administrador por defecto

Al iniciar la aplicación por primera vez, se crean automáticamente los roles del sistema y un usuario administrador inicial:

- Email: `admin@serviceflow.com`
- Password: `Admin123*`

> Se recomienda cambiar estas credenciales al primer inicio.

---

## Credenciales de Google OAuth

1. Ve a [Google Cloud Console](https://console.cloud.google.com)
2. Crea un proyecto y activa la API de OAuth
3. En **Credenciales → ID de cliente OAuth**, agrega:
   - Origen autorizado: `https://localhost:PUERTO`
   - URI de redirección: `https://localhost:PUERTO/signin-google`
4. Copia el `Client ID` y `Client Secret` en `appsettings.json`

---

## Envío de correos (Mailtrap)

El sistema usa Mailtrap en modo sandbox para desarrollo. Los correos no llegan a destinatarios reales, se interceptan y se muestran en el inbox de Mailtrap.

1. Crea una cuenta en [mailtrap.io](https://mailtrap.io)
2. Ve a **Email Testing → Inboxes → SMTP Settings**
3. Selecciona **.NET** y copia las credenciales en `appsettings.json`

---

## Equipo de desarrollo

<table>
  <tr>
    <td align="center">
      <a href="https://github.com/floreslog">
        <img src="https://github.com/floreslog.png" width="64" style="border-radius:50%"/><br/>
        <sub><b>floreslog</b></sub>
      </a>
    </td>
    <td align="center">
      <a href="https://github.com/nicolasdluna01">
        <img src="https://github.com/nicolasdluna01.png" width="64" style="border-radius:50%"/><br/>
        <sub><b>nicolasdluna01</b></sub>
      </a>
    </td>
  </tr>
</table>

## Licencia

Distribuido bajo licencia MIT. Consulta `LICENSE` para más información.
