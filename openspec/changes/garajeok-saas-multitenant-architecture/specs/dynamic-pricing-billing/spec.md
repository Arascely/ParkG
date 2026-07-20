## ADDED Requirements

### Requirement: Tarifas configurables por tenant y tipo de vehículo
El sistema SHALL permitir que cada tenant defina y actualice `tarifa_hora` y `tarifa_dia` por tipo de vehículo sin depender de valores hardcodeados.

#### Scenario: Actualización de tarifas
- **WHEN** un administrador modifica tarifas para `camion`
- **THEN** las nuevas operaciones de salida usan las tarifas vigentes del tenant

### Requirement: Cálculo mixto por minutos, horas y días
El sistema MUST calcular el cobro según minutos reales transcurridos: si es menor a 1440 minutos cobra horas redondeadas hacia arriba; si es mayor o igual a 1440 cobra días completos y horas remanentes.

#### Scenario: Estadía menor a 24 horas
- **WHEN** una estadía dura menos de 1440 minutos
- **THEN** el sistema calcula el total usando `ceil(minutos/60) * tarifa_hora`

#### Scenario: Estadía mayor o igual a 24 horas
- **WHEN** una estadía dura 1 día o más
- **THEN** el sistema calcula `dias * tarifa_dia + horasRemanentesRedondeadas * tarifa_hora`

### Requirement: Desglose de IGV con precisión decimal
El sistema MUST generar `subtotal_neto`, `igv` y `total` usando tipo `decimal` y la regla de división inversa con tasa de 18%.

#### Scenario: Generación de comprobante
- **WHEN** se procesa una salida con total calculado
- **THEN** el sistema desglosa `neto = total / 1.18` e `igv = total - neto` con redondeo financiero consistente