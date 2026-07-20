## Why

GarajeOk necesita una base arquitectónica SaaS multi-tenant robusta para operar múltiples estacionamientos independientes en una sola plataforma sin fugas de datos ni conflictos de concurrencia. Este cambio es prioritario porque define los contratos funcionales y técnicos críticos (aislamiento por tenant, facturación dinámica con IGV, validaciones peruanas y transacciones ACID) antes de implementar el sistema en .NET 9.

## What Changes

- Definir arquitectura técnica MVC nativa en .NET 9 para GarajeOk con capas de controladores, servicios de dominio y acceso a datos con EF Core.
- Diseñar modelo relacional PostgreSQL (compatible Neon) con aislamiento estricto por `TenantId` en tablas de negocio.
- Especificar aprovisionamiento SaaS atómico de tenant con inventario inicial de espacios y tarifas base (`carro`, `camion`, `trailer`).
- Especificar autenticación y autorización con JWT incluyendo claim obligatorio `TenantId` para scoping de consultas.
- Definir flujos transaccionales de ingreso/salida con bloqueo pesimista para evitar doble asignación de casilleros.
- Definir validaciones de entrada mediante FluentValidation (DNI, placa peruana, normalización de tipo de vehículo).
- Definir algoritmo de cobro mixto por horas/días con desglose financiero de IGV 18% usando precisión `decimal`.
- Definir contratos API para `/api/auth/register`, `/api/auth/login`, `/api/parking/ingreso`, `/api/parking/salida`.
- Definir estrategia TDD con pruebas unitarias e integrales (xUnit, Moq, `Microsoft.AspNetCore.Mvc.Testing`) como base de implementación.

## Capabilities

### New Capabilities
- `tenant-provisioning`: Alta SaaS de empresa de estacionamiento con bootstrap transaccional de datos operativos iniciales.
- `tenant-auth-access`: Autenticación JWT multi-tenant y gestión de operadores/roles por tenant.
- `tenant-data-isolation`: Aislamiento estricto de datos por `TenantId` mediante filtros globales EF Core.
- `parking-operations`: Registro seguro de ingreso/salida con control de aforo, validación de casilleros y concurrencia transaccional.
- `dynamic-pricing-billing`: Tarifas configurables por tenant y cálculo mixto horas/días con desglose de IGV.
- `api-contracts-garajeok`: Contratos formales de endpoints críticos para auth y operación de estacionamiento.
- `quality-tdd-strategy`: Estrategia de pruebas unitarias e integrales para capacidades críticas del dominio.

### Modified Capabilities

- Ninguna (no existen capacidades previas en `openspec/specs`).

## Impact

- Sistema afectado: backend MVC .NET 9 de GarajeOk.
- Persistencia: PostgreSQL en esquema compartido con columnas `TenantId` en tablas de negocio.
- Seguridad: JWT Bearer con claim `TenantId` y autorización por roles.
- Librerías y dependencias: `Npgsql.EntityFrameworkCore.PostgreSQL`, `FluentValidation.AspNetCore`, `Microsoft.AspNetCore.Authentication.JwtBearer`, `xunit`, `Moq`, `Microsoft.AspNetCore.Mvc.Testing`.
- APIs impactadas: nuevos endpoints de registro/login y flujo operativo de parking.
- Riesgos mitigados por diseño: fuga de datos entre tenants, condiciones de carrera en asignación de casilleros y errores de facturación por precisión decimal.

### Sistema de Diseño Visual (Tokens de UI)
- **Decisión**: El Frontend implementará un diseño adaptativo de doble propósito basado en variables nativas de CSS.
  - *Modo Claro (Editorial)*: Fondo crema, textos oscuros, acentos magenta profundo (`#8C2F63`) y verde medio (`#2F6B4F`).
  - *Modo Oscuro (Consola HUD)*: Interfaz tipo terminal de monitoreo con fondo oscuro profundo (`#080C0A`), bordes y textos en verde neón (`#3CFFA0`) y cian (`#55D6FF`).
- **Rationale**: El cambio de tema mantiene la misma identidad de marca a través del logotipo y el marco "visor" de esquinas, garantizando accesibilidad y un aspecto moderno de cuadro de mandos (Dashboard).