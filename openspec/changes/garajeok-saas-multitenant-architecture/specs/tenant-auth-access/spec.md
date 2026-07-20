## ADDED Requirements

### Requirement: Autenticación JWT con contexto de tenant
El sistema SHALL autenticar operadores por credenciales y emitir un JWT que incluya el claim obligatorio `tenant_id` y el rol del operador.

#### Scenario: Login exitoso
- **WHEN** un operador activo envía credenciales válidas para su tenant
- **THEN** el sistema devuelve un token JWT con `tenant_id`, `sub` y `role`

#### Scenario: Operador inactivo
- **WHEN** un operador inactivo intenta iniciar sesión
- **THEN** el sistema rechaza la autenticación y no emite token

### Requirement: Autorización por roles
El sistema MUST permitir acceso a operaciones según rol (`owner`, `admin`, `operador`) en el contexto de su tenant.

#### Scenario: Operador intenta operación administrativa
- **WHEN** un usuario con rol `operador` invoca un endpoint restringido a `admin`
- **THEN** el sistema responde acceso denegado