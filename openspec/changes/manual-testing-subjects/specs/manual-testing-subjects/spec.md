## ADDED Requirements

### Requirement: Sujetos manuales para autenticación JWT
El sistema SHALL documentar sujetos de prueba manuales que permitan verificar si el flujo de autenticación emite credenciales válidas y utilizables para la sesión del operador.

#### Scenario: Validación manual de login
- **WHEN** un evaluador ingresa credenciales válidas de un operador activo
- **THEN** puede observar un token JWT utilizable y una sesión autenticada para continuar con pruebas operativas

#### Scenario: Validación manual de credenciales inválidas
- **WHEN** un evaluador usa credenciales incorrectas o de un operador inactivo
- **THEN** el sistema rechaza el acceso y no deja una sesión válida abierta

### Requirement: Sujetos manuales para aislamiento multi-tenant
El sistema SHALL documentar sujetos de prueba manuales que permitan verificar que cada operador solo visualiza y opera sobre los datos de su propio tenant.

#### Scenario: Separación de datos entre tenants
- **WHEN** un evaluador alterna entre dos usuarios pertenecientes a tenants distintos
- **THEN** cada sesión observa únicamente sus propios espacios, tarifas y operaciones asociadas

#### Scenario: Verificación de contexto tenant activo
- **WHEN** un evaluador revisa una operación autenticada dentro de un tenant
- **THEN** los resultados visibles corresponden al tenant de la sesión y no muestran datos cruzados

### Requirement: Sujetos manuales para facturación y cobro
El sistema SHALL documentar sujetos de prueba manuales para validar que la lógica de cobro respeta horas, días y precisión decimal del IGV.

#### Scenario: Cobro manual por estadía corta
- **WHEN** un evaluador simula una estadía inferior a 24 horas
- **THEN** la liquidación observada refleja el redondeo por horas esperado y el desglose tributario correcto

#### Scenario: Cobro manual por estadía larga
- **WHEN** un evaluador simula una estadía de 24 horas o más
- **THEN** la liquidación observada combina días completos y horas remanentes con precisión decimal consistente