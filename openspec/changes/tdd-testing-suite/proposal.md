## Why

El núcleo SaaS multi-tenant ya está implementado, pero la base de pruebas todavía necesita formalizarse como una capacidad de primera clase. Sin una suite estándar de xUnit para aislamiento multi-tenant, lógica financiera y autenticación JWT, el riesgo de regresiones aumenta justo cuando el sistema entra en una fase de evolución más rápida.

## What Changes

- Se crea una suite de pruebas `ParkG.Tests` basada en xUnit y Moq como proyecto formal del repo.
- Se añaden pruebas unitarias para validar el aislamiento multi-tenant a través de `TenantContext` y el scoping asociado.
- Se añaden pruebas unitarias para la lógica financiera de cobro, cubriendo cálculo de horas, días y precisión decimal del IGV.
- Se añaden pruebas de integración para `AuthService` y la emisión de tokens JWT con claims de tenant y rol.

## Capabilities

### New Capabilities
- `testing-suite`: cobertura unitaria e integración para el núcleo SaaS multi-tenant, incluyendo `TenantContext`, reglas financieras y autenticación JWT.

### Modified Capabilities

## Impact

- Proyecto nuevo `ParkG.Tests` con xUnit, Moq y soporte para integración sobre la solución `ParkG`.
- Servicios y componentes afectados por el contrato de pruebas: `TenantContext`, `BillingService`, `AuthService` y la infraestructura JWT.
- Se refuerza la confiabilidad de cambios futuros en autenticación, cálculo de cobro y aislamiento por tenant.