## 1. DbContext y Modelo de Datos

- [x] 1.1 Agregar los paquetes base del backend y configurar `Npgsql.EntityFrameworkCore.PostgreSQL` para Neon/PostgreSQL.
- [x] 1.2 Crear `GarajeDbContext` con `DbSet` para `Tenant`, `Operador`, `EspacioParking`, `TarifaTenant`, `Estadia`, `Comprobante` y catálogos.
- [x] 1.3 Implementar las configuraciones Fluent API de EF Core para claves primarias, relaciones, índices únicos y restricciones por tenant.
- [x] 1.4 Agregar `TenantId` a todas las entidades de negocio y preparar el `TenantContext` para inyectarlo en cada operación.
- [x] 1.5 Aplicar filtros globales `HasQueryFilter` para aislar datos por tenant en consultas EF Core.
- [x] 1.6 Crear la migración inicial con semillas de `roles`, `tipos_vehiculo` y `metodos_pago`.

## 2. Controladores y Flujo de Negocio

- [x] 2.1 Crear `AuthController` con `POST /api/auth/register` y `POST /api/auth/login`.
- [x] 2.2 Crear `ParkingController` con `POST /api/parking/ingreso` y `POST /api/parking/salida`.
- [x] 2.3 Implementar `TenantProvisioningService` para registrar tenant, owner y tarifas base en una transacción atómica.
- [x] 2.4 Implementar `AuthService` y `JwtTokenService` para emitir JWT con claim obligatorio `tenant_id` y rol.
- [x] 2.5 Implementar `ParkingIngresoService` para validar placa, DNI, tipo de vehículo y compatibilidad del espacio.
- [x] 2.6 Implementar `ParkingSalidaService` para localizar la estadía abierta, cerrarla y liberar el espacio.
- [x] 2.7 Implementar `BillingService` para calcular minutos, horas, días, subtotal neto, IGV y total.
- [x] 2.8 Agregar manejo uniforme de errores de dominio para `400`, `401`, `403`, `404` y `409`.

## 3. Pruebas

- [x] 3.1 Crear pruebas unitarias de `BillingService` para estadías menores a 24h, mayores a 24h y cálculo de IGV.
- [x] 3.2 Crear pruebas unitarias de `AuthService` para emisión de JWT con `tenant_id` y rol.
- [x] 3.3 Crear pruebas unitarias de `ParkingIngresoService` para validar tipo de vehículo, placa peruana y DNI.
- [x] 3.4 Crear pruebas unitarias de aislamiento multi-tenant verificando que el filtro global no expone datos de otros tenants.
- [x] 3.5 Crear pruebas de integración con `Microsoft.AspNetCore.Mvc.Testing` para register/login/ingreso/salida.
- [x] 3.6 Crear prueba de concurrencia para el ingreso simultáneo al mismo espacio y verificar que solo una transacción confirma.
- [x] 3.7 Ejecutar la suite completa de pruebas y corregir fallos antes de continuar con UI.

## 4. UI MVC

- [x] 4.1 Crear `HomeController` o redirigir el arranque a la pantalla de login según el estado de autenticación.
- [x] 4.2 Diseñar la vista de login para operadores con layout MVC compartido y mensajes de error claros.
- [x] 4.3 Crear la vista de registro SaaS para alta de tenant con RUC, nombre comercial y credenciales del owner.
- [x] 4.4 Crear las vistas operativas de ingreso y salida de vehículos con formularios simples y validación visual.
- [ ] 4.5 Crear la vista de configuración de tarifas por tenant para editar precio por hora y por día.
- [x] 4.6 Implementar el sistema visual dual definido en el diseño mediante variables CSS en `wwwroot/css/site.css`.
- [x] 4.7 Adaptar el `Shared/_Layout.cshtml` para navegación básica, estado de sesión y cambio de tema claro/oscuro.
- [x] 4.8 Ajustar estilos responsivos para escritorio y móvil, priorizando uso operativo rápido en garaje.