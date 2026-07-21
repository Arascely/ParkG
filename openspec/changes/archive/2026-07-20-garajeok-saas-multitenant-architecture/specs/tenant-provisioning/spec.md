## ADDED Requirements

### Requirement: Aprovisionamiento atómico de tenant
El sistema MUST permitir registrar una empresa de estacionamiento creando de forma atómica el `Tenant`, el operador propietario inicial, el inventario base de espacios y las tarifas base para `carro`, `camion` y `trailer`.

#### Scenario: Registro exitoso de nuevo tenant
- **WHEN** un usuario envía `ruc` válido de 11 dígitos y `nombreComercial` junto con credenciales del propietario
- **THEN** el sistema crea el tenant y todos los registros iniciales en una sola transacción confirmada

#### Scenario: Fallo parcial durante bootstrap
- **WHEN** ocurre un error al crear alguno de los datos iniciales del tenant
- **THEN** el sistema revierte toda la transacción y no persiste registros huérfanos

### Requirement: Unicidad de empresa por RUC
El sistema MUST impedir el registro de dos tenants con el mismo RUC.

#### Scenario: RUC duplicado
- **WHEN** se intenta registrar un tenant con un RUC ya existente
- **THEN** el sistema responde conflicto de negocio y no crea un nuevo tenant