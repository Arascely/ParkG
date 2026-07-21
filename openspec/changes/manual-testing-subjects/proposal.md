## Why

La suite automatizada ya cubre el núcleo crítico, pero el equipo necesita una referencia explícita de qué sujetos funcionales deben validarse manualmente antes de liberar cambios sensibles. Esto reduce ambigüedad durante QA, mejora la repetibilidad de pruebas exploratorias y ayuda a verificar rápidamente autenticación, aislamiento tenant y cálculo de cobros.

## What Changes

- Se documentan sujetos de prueba manuales para el contexto tenant, la autenticación JWT y la facturación dinámica.
- Se definen escenarios manuales mínimos para validar el comportamiento esperado de cada sujeto de prueba.
- Se establece una referencia de QA que puede reutilizarse para smoke tests, UAT y verificación previa a despliegues.

## Capabilities

### New Capabilities
- `manual-testing-subjects`: especificación de los sujetos de prueba manuales para autenticación, aislamiento multi-tenant y lógica financiera.

### Modified Capabilities

## Impact

- Equipo de QA y desarrollo que valida cambios antes de releases.
- Procedimientos manuales de verificación sobre `TenantContext`, `AuthService` y `BillingService`.
- Mayor trazabilidad entre la suite automatizada existente y las pruebas manuales exploratorias.