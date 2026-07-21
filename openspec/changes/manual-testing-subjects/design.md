## Context

La base funcional del SaaS multi-tenant ya está implementada y validada con pruebas automatizadas. Falta un contrato explícito para QA manual que indique qué sujetos del sistema deben revisarse antes de liberar cambios sensibles, especialmente en autenticación, aislamiento tenant y cálculo de cobros.

El objetivo no es redefinir el comportamiento del sistema, sino normalizar qué se considera un sujeto de prueba manual y qué resultado debe observar el evaluador para validar que el producto sigue operando correctamente.

## Goals / Non-Goals

**Goals:**
- Definir sujetos de prueba manuales accionables para QA y smoke testing.
- Cubrir los tres ejes críticos del dominio: autenticación JWT, aislamiento multi-tenant y facturación.
- Hacer que cada sujeto tenga resultados observables y fáciles de verificar por una persona.

**Non-Goals:**
- No se introducen nuevas reglas de negocio.
- No se reemplazan ni amplían las pruebas automatizadas existentes.
- No se agregan flujos de UI nuevos ni cambios de API funcionales.

## Decisions

- **Separar esta capacidad como documentación de QA y no como cambio funcional.** El problema a resolver es de verificabilidad humana, no de implementación. Alternativa: añadirlo como parte de la suite automatizada, pero eso no ayuda al flujo manual de liberación.
- **Centrarse en sujetos y criterios observables en lugar de casos exhaustivos.** Para QA manual es mejor un set pequeño y repetible. Alternativa: documentar todos los casos posibles, pero haría el proceso más pesado sin mejorar proporcionalmente la cobertura.
- **Agrupar los sujetos por dominio crítico.** Permite que QA trabaje por sesión: autenticación, tenant y billing. Alternativa: una lista única plana, pero dificulta priorización y trazabilidad.

## Risks / Trade-offs

- [Riesgo] Los sujetos manuales pueden quedarse desactualizados frente a la implementación real → [Mitigación] Mantenerlos alineados con los contratos ya validados por automatización.
- [Riesgo] La documentación puede resultar demasiado genérica para QA → [Mitigación] Incluir criterios observables y resultados esperados concretos.
- [Riesgo] El alcance podría confundirse con pruebas automatizadas → [Mitigación] Aclarar explícitamente que esta capacidad documenta verificación manual.

## Migration Plan

1. Crear el spec de sujetos de prueba manuales.
2. Definir tareas de documentación y revisión con QA.
3. Validar que la terminología coincida con los contratos ya implementados.
4. Archivar el change una vez aceptado el material de referencia.

## Open Questions

- ¿El equipo de QA quiere que estos sujetos se publiquen como checklist operativo o solo como referencia de especificación?
- ¿Se necesita una versión reducida para smoke tests de preproducción además de la versión completa?