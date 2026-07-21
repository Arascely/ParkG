## ADDED Requirements

### Requirement: Proyecto de pruebas estándar para el backend
El sistema SHALL disponer de un proyecto de pruebas `ParkG.Tests` basado en xUnit y Moq para validar comportamiento crítico del backend.

#### Scenario: Estructura mínima del proyecto
- **WHEN** se crea o valida la suite de pruebas del repositorio
- **THEN** el proyecto de tests puede referenciar la aplicación principal y ejecutar pruebas unitarias e integración sin mezclar dependencias de producción

### Requirement: Aislamiento multi-tenant verificable por pruebas unitarias
El sistema MUST incluir pruebas unitarias que validen que `TenantContext` resuelve el `TenantId` y el `OperatorId` desde el contexto autenticado y no desde datos del cliente.

#### Scenario: Resolución de tenant desde claims
- **WHEN** una prueba construye un contexto HTTP con claims `tenant_id` y `sub`
- **THEN** `TenantContext` expone esos identificadores como el contexto efectivo de la solicitud

#### Scenario: Contexto sin autenticación
- **WHEN** el contexto HTTP no contiene identidad autenticada
- **THEN** `TenantContext` no expone un tenant u operador válidos

### Requirement: Cobertura financiera determinista por pruebas unitarias
El sistema MUST incluir pruebas unitarias que validen la lógica de cobro para minutos, horas, días y precisión decimal del IGV.

#### Scenario: Cobro por horas menores a 24h
- **WHEN** una estadía dura menos de 1440 minutos
- **THEN** el cálculo usa horas redondeadas hacia arriba y conserva precisión decimal en el resultado

#### Scenario: Cobro por días completos y remanentes
- **WHEN** una estadía dura 1440 minutos o más
- **THEN** el cálculo combina días completos, horas remanentes y reglas financieras del IGV sin pérdida de precisión

### Requirement: Integración de autenticación y emisión JWT
El sistema SHALL incluir pruebas de integración para `AuthService` y la emisión de tokens JWT con `tenant_id`, `sub` y `role`.

#### Scenario: Emisión de token válido
- **WHEN** las credenciales del operador son válidas y el tenant está activo
- **THEN** el servicio emite un token JWT utilizable por la aplicación y con claims de tenant y rol

#### Scenario: Rechazo de credenciales inválidas
- **WHEN** las credenciales no son válidas o el operador está inactivo
- **THEN** el servicio rechaza la autenticación y no emite token