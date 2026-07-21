## ADDED Requirements

### Requirement: Ingreso transaccional con bloqueo de casillero
El sistema MUST registrar ingresos bajo transacción explícita y bloquear el espacio de parking a nivel de fila para evitar doble asignación concurrente.

#### Scenario: Dos operadores intentan asignar el mismo espacio
- **WHEN** dos solicitudes de ingreso compiten por un mismo casillero al mismo tiempo
- **THEN** solo una transacción confirma la asignación y la otra recibe conflicto de concurrencia

### Requirement: Validación cruzada de tipo de vehículo y espacio
El sistema MUST validar que el tipo de vehículo solicitado sea compatible con `tipo_vehiculo_permitido` del espacio antes de confirmar el ingreso.

#### Scenario: Incompatibilidad de categoría
- **WHEN** se intenta registrar un `trailer` en un espacio configurado para `carro`
- **THEN** el sistema cancela la transacción y responde error de validación de negocio

### Requirement: Validación y normalización de entrada operativa
El sistema MUST validar DNI de 8 dígitos numéricos, placa peruana con regex oficial y normalizar `tipo_vehiculo` a minúsculas antes de evaluar reglas.

#### Scenario: Payload con formato inválido
- **WHEN** la solicitud de ingreso contiene DNI o placa inválidos
- **THEN** el sistema rechaza la solicitud en la capa de validación sin ejecutar lógica de persistencia