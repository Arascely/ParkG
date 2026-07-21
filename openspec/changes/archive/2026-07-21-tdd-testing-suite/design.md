## Context

El sistema SaaS multi-tenant ya tiene autenticación JWT, `TenantContext`, persistencia con `TenantId` y lógica financiera operativa. El siguiente paso es formalizar una suite de pruebas que proteja estos contratos críticos y permita evolucionar el backend con menor riesgo de regresión.

La base técnica del repo ya incluye `xunit`, `Moq`, `Microsoft.AspNetCore.Mvc.Testing` y soporte para una aplicación ASP.NET Core con DI completa, por lo que el trabajo principal es estructurar las pruebas y consolidar escenarios representativos.

## Goals / Non-Goals

**Goals:**
- Establecer una suite de pruebas `ParkG.Tests` coherente y mantenible.
- Cubrir el aislamiento multi-tenant mediante pruebas unitarias centradas en `TenantContext`.
- Cubrir la lógica financiera de cobro con pruebas unitarias deterministas.
- Cubrir la integración de autenticación y emisión de JWT con pruebas de integración sobre el host real de la aplicación.

**Non-Goals:**
- No se modificarán reglas de negocio productivas del dominio.
- No se introducirán nuevos proveedores de identidad ni un nuevo esquema de autorización.
- No se reemplazará el stack de pruebas existente por otro framework.
- No se expandirá el alcance a los controladores de parking o a la UI en esta fase.

## Decisions

- **Usar xUnit como framework base de ejecución de pruebas.** Es el estándar ya alineado con el repo y reduce fricción frente a la solución actual. Alternativas como NUnit o MSTest se descartan para evitar una migración innecesaria.
- **Usar Moq para aislar dependencias de servicios.** Permite simular contextos, repositorios y collaborators sin acoplar las pruebas a infraestructura real. Alternativa: stubs manuales, pero aumentan el costo de mantenimiento.
- **Separar claramente pruebas unitarias e integración dentro del mismo proyecto de tests.** Mantiene una superficie simple para CI/CD y evita multiplicar proyectos sin necesidad. Alternativa: proyectos separados por tipo de prueba, pero eso agrega mantenimiento sin un beneficio claro en este tamaño de solución.
- **Ejecutar integración sobre el host ASP.NET Core real con la configuración de prueba.** Esto valida wiring de DI, JWT y servicios reales, mientras se conserva aislamiento mediante una base de datos de pruebas. Alternativa: mockear el contenedor completo, pero eso dejaría de verificar los contratos de integración que se quieren proteger.
- **Mantener los tests deterministas y herméticos.** Las pruebas financieras usarán fechas y montos explícitos; las de auth validarán claims y expiración sin depender de reloj del sistema más allá de lo necesario. Alternativa: tests basados en tiempo real, pero introducen flakes.

## Risks / Trade-offs

- **Riesgo de tests frágiles por infraestructura compartida** → Mitigar usando datos de prueba mínimos, nombres únicos y setup/teardown controlado por fixture.
- **Riesgo de divergencia entre unit tests e integración** → Mitigar alineando los escenarios con los contratos de negocio existentes y usando los mismos valores de referencia donde sea posible.
- **Riesgo de aumentar el tiempo de ejecución de la suite** → Mitigar dejando la mayoría de escenarios como unitarios y reservando integración para los flujos JWT críticos.
- **Riesgo de falsa confianza si solo se prueba el happy path** → Mitigar incluyendo validaciones de aislamiento, precisión decimal y emisión correcta de claims como casos obligatorios.