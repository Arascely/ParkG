#   ParkG: Motor SaaS Multi-Tenant para Estacionamientos

![.NET 9](https://img.shields.io/badge/.NET-9.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white)
![ASP.NET Core MVC](https://img.shields.io/badge/ASP.NET_MVC-0072C6?style=for-the-badge&logo=microsoft&logoColor=white)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-316192?style=for-the-badge&logo=postgresql&logoColor=white)
![OpenSpec](https://img.shields.io/badge/OpenSpec-SDD-FF6B6B?style=for-the-badge)
![Render](https://img.shields.io/badge/Render-46E3B7?style=for-the-badge&logo=render&logoColor=white)

##  Descripción del Proyecto

**ParkG** (anteriormente conocido conceptualmente como GarajeOk) es un sistema web transaccional bajo el modelo **Software as a Service (SaaS) Multi-Tenant**. Está diseñado para operar múltiples empresas de estacionamiento independientes dentro de una misma plataforma unificada, garantizando un aislamiento estricto de datos, seguridad transaccional y alta concurrencia.

##  Cómo se construyó (Ingeniería y Metodología)

Este proyecto no fue programado de manera improvisada; sigue un ciclo de vida de desarrollo de software (SDLC) altamente estructurado:

1. **Spec-Driven Development (SDD) con OpenSpec:** Todo el código escrito fue precedido por especificaciones formales (`proposal.md`, `design.md`, `tasks.md`). Las reglas de negocio, la arquitectura y los contratos de las APIs se documentaron antes de escribir la primera línea de código C#, utilizando asistentes de IA para validar la lógica.
2. **Arquitectura Limpia en Capas:** Se separó estrictamente el Dominio (`Entities`), la Infraestructura (`ParkGDbContext`), la Aplicación (`Services`) y la Presentación (`Controllers` y `Views`), garantizando mantenibilidad a largo plazo.
3. **Evolución de Datos (Code-First):** La base de datos no se creó a mano. Se utilizó **Entity Framework Core 9** con el enfoque *Code-First*. El diseño relacional nació de clases en C#, iterando localmente con **SQLite** para un desarrollo ágil, y utilizando migraciones automatizadas (`dotnet ef migrations`) para reflejar los cambios en producción.
4. **Test-Driven Development (TDD):** (En proceso) Integración de pruebas unitarias con `xUnit` y `Moq` para blindar la lógica financiera (cálculo de IGV, cobro por horas/días) y evitar regresiones.

##  Patrones de Diseño y Reglas de Negocio

* **Aislamiento de Datos (Tenant Isolation):** Implementación de Filtros Globales de Consulta en EF Core. El `ParkGDbContext` inyecta automáticamente la cláusula `WHERE TenantId = @current` en cada operación a través de un `TenantContext` de alcance local (Scoped), previniendo fugas de datos entre garajes.
* **Concurrencia Segura:** Uso de transacciones ACID y bloqueo pesimista (`SELECT ... FOR UPDATE`) para evitar condiciones de carrera (ej. dos operadores intentando asignar el mismo casillero en el mismo milisegundo).
* **Seguridad API:** Autenticación basada en **JWT Bearer** con claims obligatorios de `TenantId`.
* **Validaciones Defensivas:** Integración de `FluentValidation` para asegurar reglas peruanas (DNI de 8 dígitos, RUC de 11 dígitos, normalización de placas).

##  Módulos Principales

1.  **Aprovisionamiento Multi-Tenant:** Alta atómica de empresas con inventario inicial de casilleros y tarifas (`carro`, `camion`, `trailer`).
2.  **Operaciones de Parking:** Registro de ingresos, liberación de casilleros y cierre de estancias en tiempo real.
3.  **Facturación Dinámica:** Cálculo mixto (horas/días) con precisión `decimal(12,2)` para desgloses exactos de IGV (18%) sin pérdida de céntimos.
4.  **Sistema de Diseño Visual Dual:**
    * *Modo Claro (Editorial):* Interfaz administrativa corporativa.
    * *Modo Oscuro (Consola HUD):* Interfaz de alto contraste tipo terminal para operadores nocturnos.

##  QA Manual

Para validaciones humanas previas a liberación, consulta la guía en [docs/qa/README.md](docs/qa/README.md).

##  Requisitos Previos (Instalación Local)

* [.NET 9 SDK](https://dotnet.microsoft.com/download) instalado.
* Visual Studio 2022 o Visual Studio Code.

##  Configuración y Ejecución Rápida

1.  **Clonar el repositorio:**
    ```bash
    git clone [https://github.com/TuUsuario/ParkG.git](https://github.com/TuUsuario/ParkG.git)
    cd ParkG/ParkG
    ```
2.  **Asegurar la Conexión Local (SQLite):**
    Verifica que tu `appsettings.json` apunte al entorno de desarrollo:
    ```json
    "ConnectionStrings": {
      "DefaultConnection": "Data Source=ParkGLocal.db"
    }
    ```
3.  **Aplicar Migraciones:**
    ```bash
    dotnet ef database update
    ```
4.  **Ejecutar el proyecto:**
    ```bash
    dotnet run
    ```
    Navega a `https://localhost:5001`.

##  Entorno de Producción

Para producción, el sistema cambia dinámicamente su cadena de conexión hacia **Neon (PostgreSQL Serverless)**, garantizando escalabilidad elástica. El despliegue continuo del backend MVC está proyectado para infraestructuras PaaS como **Render** o **Azure App Services**.