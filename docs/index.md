---
_layout: landing
---

<div class="hero-section" style="background: linear-gradient(135deg, #25262a 0%, #3a3b40 100%); color: white; padding: 64px 32px; text-align: center; border-radius: 12px; margin-bottom: 48px;">
  <div style="margin-bottom: 16px;">
    <img src="https://img.shields.io/badge/.NET-8.0-purple" style="margin: 4px;" />
    <img src="https://img.shields.io/badge/status-finished-success" style="margin: 4px;" />
    <img src="https://img.shields.io/badge/license-MIT-green" style="margin: 4px;" />
  </div>
  <h1 style="font-size: 2.8rem; font-weight: 800; margin: 0 0 12px 0;">ServiceFlow</h1>
  <p style="font-size: 1.1rem; color: #c9cdd4; max-width: 600px; margin: 0 auto;">
    Sistema de Gestión de Solicitudes Internas — Help Desk desarrollado con ASP.NET Core MVC (.NET 8)
  </p>
</div>

## ¿Qué es ServiceFlow?

ServiceFlow es una aplicación web de tipo Help Desk diseñada para gestionar solicitudes internas en organizaciones como empresas, universidades o instituciones públicas. Permite registrar, asignar, dar seguimiento y resolver tickets de soporte de forma estructurada, con tres niveles de acceso: administrador, agente y usuario.

---

## Características principales

<div style="display: grid; grid-template-columns: repeat(auto-fit, minmax(260px, 1fr)); gap: 16px; margin: 24px 0;">

<div style="background: #f8f9fa; border-radius: 10px; padding: 20px; border-left: 4px solid #25262a;">
<strong>🔐 Autenticación</strong><br/>
ASP.NET Identity + OAuth con Google. Verificación de correo por código de 6 dígitos y recuperación de contraseña.
</div>

<div style="background: #f8f9fa; border-radius: 10px; padding: 20px; border-left: 4px solid #25262a;">
<strong>👥 Roles y permisos</strong><br/>
Tres roles: <code>Admin</code>, <code>Agent</code> y <code>User</code>, cada uno con acceso y funcionalidades diferenciadas.
</div>

<div style="background: #f8f9fa; border-radius: 10px; padding: 20px; border-left: 4px solid #25262a;">
<strong>🎫 Gestión de solicitudes</strong><br/>
Crear, editar, comentar, asignar y resolver tickets. Filtros por estado, prioridad y categoría.
</div>

<div style="background: #f8f9fa; border-radius: 10px; padding: 20px; border-left: 4px solid #25262a;">
<strong>📊 Panel de control</strong><br/>
Estadísticas por rol, paginación en listados y sidebar de navegación persistente.
</div>

<div style="background: #f8f9fa; border-radius: 10px; padding: 20px; border-left: 4px solid #25262a;">
<strong>⚙️ Administración</strong><br/>
Gestión completa de usuarios y categorías exclusiva para el rol Admin.
</div>

<div style="background: #f8f9fa; border-radius: 10px; padding: 20px; border-left: 4px solid #25262a;">
<strong>📧 Notificaciones por correo</strong><br/>
Integración con MailKit y Mailtrap para envío de correos en desarrollo.
</div>

</div>

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

## Roles del sistema

| Rol | Permisos |
|---|---|
| `Admin` | Acceso total: gestión de usuarios, categorías, solicitudes y asignación de agentes |
| `Agent` | Ver y gestionar solicitudes asignadas, cambiar estado |
| `User` | Crear y dar seguimiento a sus propias solicitudes |

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

```bash
cd ServiceFlow.Web
dotnet ef database update
```

Esto creará la base de datos, todas las tablas y los roles `Admin`, `Agent` y `User` automáticamente.

---

## Usuario administrador por defecto

Al iniciar la aplicación por primera vez se crea automáticamente un usuario administrador:

| Campo | Valor |
|---|---|
| Email | `admin@serviceflow.com` |
| Password | `Admin123*` |

> ⚠️ Se recomienda cambiar estas credenciales al primer inicio.

---

## Equipo de desarrollo

<div style="display: flex; gap: 24px; margin: 16px 0;">
  <div style="text-align: center;">
    <a href="https://github.com/floreslog">
      <img src="https://github.com/floreslog.png" width="72" style="border-radius: 50%; border: 3px solid #25262a;" /><br/>
      <strong>floreslog</strong>
    </a>
  </div>
  <div style="text-align: center;">
    <a href="https://github.com/nicolasdluna01">
      <img src="https://github.com/nicolasdluna01.png" width="72" style="border-radius: 50%; border: 3px solid #25262a;" /><br/>
      <strong>nicolasdluna01</strong>
    </a>
  </div>
</div>

---

## Licencia

Distribuido bajo licencia **MIT**. Consulta `LICENSE` para más información.