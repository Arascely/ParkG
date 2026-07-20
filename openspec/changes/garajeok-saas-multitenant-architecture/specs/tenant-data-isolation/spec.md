## ADDED Requirements

### Requirement: Aislamiento estricto por TenantId
El sistema MUST aplicar aislamiento de datos por `TenantId` en todas las entidades de negocio mediante filtros globales de EF Core para evitar exposición cruzada.

#### Scenario: Consulta de datos operativos
- **WHEN** un operador autenticado realiza una consulta de cualquier entidad de negocio
- **THEN** el sistema retorna solo registros cuyo `TenantId` coincide con el claim `tenant_id` del JWT

### Requirement: Persistencia con inyección obligatoria de TenantId
El sistema SHALL inyectar `TenantId` de forma obligatoria al crear o actualizar registros de negocio.

#### Scenario: Creación de entidad sin TenantId explícito de payload
- **WHEN** el cliente envía una solicitud de creación de entidad de negocio
- **THEN** el sistema asigna internamente `TenantId` desde el contexto autenticado y no desde datos manipulables por el cliente