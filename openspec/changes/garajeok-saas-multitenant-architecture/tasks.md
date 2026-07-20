## 1. Foundation y Dependencias

- [ ] 1.1 Agregar paquetes NuGet requeridos (`Npgsql.EntityFrameworkCore.PostgreSQL`, `FluentValidation.AspNetCore`, `Microsoft.AspNetCore.Authentication.JwtBearer`, `xunit`, `Moq`, `Microsoft.AspNetCore.Mvc.Testing`).
- [ ] 1.2 Configurar conexión PostgreSQL (Neon-compatible) y pipeline de migraciones EF Core.
- [ ] 1.3 Crear estructura de capas (`Controllers`, `Application`, `Domain`, `Infrastructure`, `Validation`).

## 2. Modelo de Datos Multi-Tenant

- [ ] 2.1 Implementar entidades y configuraciones EF Core para `Tenant`, `Operador`, `EspacioParking`, `TarifaTenant`, `Estadia`, `Comprobante` y catálogos.
- [ ] 2.2 Añadir columna `TenantId` en tablas de negocio, índices compuestos y restricciones de unicidad por tenant.
- [ ] 2.3 Generar migración inicial con seeds globales (`roles`, `tipos_vehiculo`, `metodos_pago`).

## 3. Seguridad y Contexto Tenant

- [ ] 3.1 Implementar `AuthService` y `JwtTokenService` con claim obligatorio `tenant_id`.
- [ ] 3.2 Implementar `TenantContext` por request y middleware/helper para resolver `TenantId` desde JWT.
- [ ] 3.3 Aplicar `HasQueryFilter` global por `TenantId` en todas las entidades de negocio.

## 4. Aprovisionamiento SaaS

- [ ] 4.1 Implementar `POST /api/auth/register` con validación de RUC y creación atómica de tenant.
- [ ] 4.2 Implementar bootstrap transaccional de operador owner, espacios iniciales y tarifas base (`carro`, `camion`, `trailer`).
- [ ] 4.3 Implementar manejo de conflicto por RUC duplicado y rollback total ante fallos parciales.

## 5. Operación de Parking con Concurrencia

- [ ] 5.1 Implementar `POST /api/parking/ingreso` con FluentValidation (DNI, placa, tipoVehiculo normalizado).
- [ ] 5.2 Implementar validación cruzada `tipo_vehiculo` vs `tipo_vehiculo_permitido` y errores 400.
- [ ] 5.3 Implementar transacción explícita y bloqueo pesimista (`FOR UPDATE`) para evitar doble asignación de casillero.
- [ ] 5.4 Implementar `POST /api/parking/salida` con cierre transaccional de estadía y liberación de espacio.

## 6. Facturación Dinámica e IGV

- [ ] 6.1 Implementar `BillingService` con algoritmo mixto por minutos/horas/días según reglas del dominio.
- [ ] 6.2 Implementar cálculo financiero en `decimal` para `subtotal_neto`, `igv` y `total` con tasa 18%.
- [ ] 6.3 Persistir comprobante de salida y devolver desglose completo en la respuesta API.

## 7. Contratos API y Manejo de Errores

- [ ] 7.1 Implementar controladores `AuthController` y `ParkingController` con contratos definidos en spec.
- [ ] 7.2 Estandarizar respuestas de error (`400`, `401`, `403`, `404`, `409`) con payload consistente.
- [ ] 7.3 Documentar contratos HTTP y ejemplos de request/response para consumo interno.

## 8. TDD y Validación Integral

- [ ] 8.1 Crear pruebas unitarias de `BillingService` para horas redondeadas, días completos y remanentes.
- [ ] 8.2 Crear pruebas unitarias de autenticación e inyección de claim `tenant_id`.
- [ ] 8.3 Crear pruebas unitarias de aislamiento multi-tenant sobre consultas EF Core.
- [ ] 8.4 Crear pruebas de integración (`Microsoft.AspNetCore.Mvc.Testing`) para register/login/ingreso/salida.
- [ ] 8.5 Ejecutar suite completa en CI y corregir fallos antes de pasar a implementación final.