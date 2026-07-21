# Guía de Sujetos de Prueba Manuales

Esta guía documenta los sujetos mínimos que QA debe revisar manualmente antes de liberar cambios sensibles en autenticación, aislamiento multi-tenant y facturación.

## 1. Autenticación JWT

### Sujeto de prueba: Login de operador activo

- **Objetivo:** Verificar que un operador activo pueda autenticarse correctamente.
- **Datos de entrada:** RUC válido, usuario activo y contraseña correcta.
- **Resultado esperado:** El sistema emite un JWT utilizable y la sesión queda autenticada.
- **Criterio de aprobación:** El token permite continuar con operaciones protegidas sin errores de autorización.

### Sujeto de prueba: Rechazo de credenciales inválidas

- **Objetivo:** Verificar que el sistema bloquee accesos incorrectos.
- **Datos de entrada:** RUC válido, usuario inexistente o contraseña incorrecta.
- **Resultado esperado:** El sistema rechaza el acceso y no entrega token.
- **Criterio de aprobación:** No se genera una sesión válida ni se habilitan operaciones privadas.

## 2. Aislamiento Multi-Tenant

### Sujeto de prueba: Separación de datos entre tenants

- **Objetivo:** Comprobar que cada operador solo observe sus propios datos.
- **Datos de entrada:** Dos usuarios pertenecientes a tenants distintos.
- **Resultado esperado:** Cada sesión ve únicamente sus espacios, tarifas y operaciones.
- **Criterio de aprobación:** No aparecen registros cruzados entre tenants.

### Sujeto de prueba: Contexto tenant activo

- **Objetivo:** Confirmar que el contexto autenticado gobierna la visibilidad de datos.
- **Datos de entrada:** Sesión autenticada dentro de un tenant específico.
- **Resultado esperado:** Las operaciones consultadas corresponden al tenant de la sesión.
- **Criterio de aprobación:** El operador no puede inferir datos de otro tenant.

## 3. Facturación Dinámica

### Sujeto de prueba: Estadía menor a 24 horas

- **Objetivo:** Validar el cobro por horas redondeadas.
- **Datos de entrada:** Estadía simulada inferior a 1440 minutos.
- **Resultado esperado:** La liquidación calcula el cobro por horas y muestra IGV con precisión decimal.
- **Criterio de aprobación:** El total y el desglose coinciden con la tarifa configurada.

### Sujeto de prueba: Estadía de 24 horas o más

- **Objetivo:** Validar el cobro mixto por días y horas remanentes.
- **Datos de entrada:** Estadía simulada de 1440 minutos o más.
- **Resultado esperado:** La liquidación combina días completos, horas restantes e IGV con redondeo consistente.
- **Criterio de aprobación:** El total mostrado coincide con el cálculo esperado para el tenant.

## 4. Criterios de Uso

- Ejecutar estos sujetos antes de liberar cambios en autenticación, billing o aislamiento tenant.
- Si un sujeto falla, detener la liberación hasta corregir la causa raíz.
- Mantener esta guía alineada con los contratos y pruebas automatizadas del sistema.