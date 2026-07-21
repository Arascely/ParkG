## ADDED Requirements

### Requirement: Contrato de API para registro de tenant
El sistema MUST exponer `POST /api/auth/register` para aprovisionamiento SaaS y devolver identificadores de tenant y operador propietario al completar el bootstrap.

#### Scenario: Respuesta de registro exitoso
- **WHEN** el cliente invoca `POST /api/auth/register` con payload válido
- **THEN** la API responde `201` con `tenantId`, `ownerOperatorId` y tarifas base iniciales

### Requirement: Contrato de API para login de operador
El sistema MUST exponer `POST /api/auth/login` y responder un JWT que incluya `tenant_id` y rol cuando las credenciales son válidas.

#### Scenario: Login correcto
- **WHEN** el cliente invoca `POST /api/auth/login` con credenciales válidas
- **THEN** la API responde `200` con `accessToken`, `expiresIn`, `tenantId` y `role`

### Requirement: Contrato de API para ingreso de vehículo
El sistema SHALL exponer `POST /api/parking/ingreso` protegido con JWT y devolver ticket digital al confirmar una entrada.

#### Scenario: Ingreso confirmado
- **WHEN** un operador autenticado invoca `POST /api/parking/ingreso` con datos válidos
- **THEN** la API responde `201` con `ticketId`, `fechaIngreso` y `espacioCodigo`

### Requirement: Contrato de API para salida y liquidación
El sistema SHALL exponer `POST /api/parking/salida` protegido con JWT y devolver la liquidación completa de pago.

#### Scenario: Salida liquidada
- **WHEN** un operador autenticado invoca `POST /api/parking/salida` para una placa con estadía abierta
- **THEN** la API responde `200` con `minutosTotales`, `subtotalNeto`, `igv` y `total`