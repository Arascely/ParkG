## ADDED Requirements

### Requirement: Cobertura unitaria de reglas críticas
El sistema MUST definir pruebas unitarias con xUnit y Moq para servicios de aislamiento por tenant, autenticación y facturación dinámica antes de implementación completa.

#### Scenario: Suite unitaria de billing
- **WHEN** se ejecutan pruebas de `BillingService`
- **THEN** se validan de forma aislada los casos de horas redondeadas, días completos, remanentes e IGV

#### Scenario: Suite unitaria de tenant/auth
- **WHEN** se ejecutan pruebas de autenticación y contexto tenant
- **THEN** se verifica la inclusión de claim `tenant_id` y el scoping de consultas

### Requirement: Cobertura de integración para flujos operativos
El sistema SHALL definir pruebas de integración con `Microsoft.AspNetCore.Mvc.Testing` para validar registro de tenant, login, ingreso y salida end-to-end.

#### Scenario: Flujo integrado de ingreso y salida
- **WHEN** una prueba de integración registra tenant, autentica operador, ingresa y retira un vehículo
- **THEN** el sistema confirma estado consistente de espacio, estadía y comprobante de pago