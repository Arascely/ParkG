## Context

GarajeOk se implementará como plataforma SaaS multi-tenant sobre ASP.NET Core MVC (.NET 9), EF Core y PostgreSQL (Neon-compatible). El problema principal es garantizar aislamiento estricto entre empresas dentro de una base compartida, junto con operaciones concurrentes seguras para ingreso/salida de vehículos y cálculo financiero exacto de cobros con IGV peruano.

El estado actual del proyecto es una base MVC inicial sin dominio de parking multi-tenant. Se requiere una arquitectura con seguridad por claim `TenantId`, filtros globales de EF Core, transacciones explícitas y validaciones de entrada antes de ejecutar lógica de negocio.

## Goals / Non-Goals

**Goals:**
- Diseñar arquitectura MVC por capas (Controllers, Application Services, Domain/Entities, Infrastructure/EF Core) para el dominio de estacionamientos SaaS.
- Definir modelo relacional multi-tenant con `TenantId` en todas las tablas de negocio y restricciones de integridad.
- Definir contratos API formales para `register`, `login`, `ingreso`, `salida`.
- Definir estrategia de concurrencia con transacciones ACID y bloqueo pesimista (`SELECT ... FOR UPDATE`) en asignación de casilleros.
- Definir cálculo de cobro mixto horas/días y desglose de IGV con precisión `decimal`.
- Definir estrategia TDD para pruebas unitarias e integración.

**Non-Goals:**
- Integración con pasarelas de pago externas en esta fase.
- Multi-región, sharding por tenant o particionamiento físico avanzado.
- Interfaz frontend final de producción (solo contratos y arquitectura backend).

## Decisions

### 1) Arquitectura por capas MVC nativa
- **Decisión**: usar ASP.NET Core MVC con controladores API en capa de presentación, servicios de aplicación para casos de uso, y repositorios/DbContext en infraestructura.
- **Rationale**: mantiene claridad de responsabilidades, testabilidad y adopción nativa del stack .NET.
- **Alternativas**:
  - Minimal APIs: menor estructura para dominios complejos.
  - Arquitectura puramente monolítica sin servicios: dificulta TDD y mantenibilidad.

### 2) Aislamiento multi-tenant por columna y filtro global
- **Decisión**: incluir `TenantId` en todas las tablas de negocio y aplicar `HasQueryFilter(e => e.TenantId == _tenantContext.TenantId)` en cada entidad.
- **Rationale**: control transversal y defensivo ante consultas accidentalmente sin filtro.
- **Alternativas**:
  - Esquema por tenant: mayor complejidad operativa/migraciones.
  - Base por tenant: coste operativo superior para etapa inicial.

### 3) Seguridad JWT con claim obligatorio `TenantId`
- **Decisión**: emitir JWT con claims `sub`, `role`, `tenant_id`; inyectar `tenant_id` en `TenantContext` por request.
- **Rationale**: permite scoping automático en EF Core y autorización por rol.
- **Alternativas**:
  - Header custom de tenant: manipulable por cliente y menos seguro.

### 4) Concurrencia con transacciones explícitas + bloqueo pesimista
- **Decisión**: para ingreso/salida usar `BeginTransactionAsync(IsolationLevel.ReadCommitted)` y lecturas críticas con `FOR UPDATE` sobre `EspaciosParking` y `Estadias` abiertas.
- **Rationale**: evita condición de carrera al asignar casilleros o cerrar tickets simultáneamente.
- **Alternativas**:
  - Optimistic concurrency únicamente: mayor probabilidad de reintentos y colisiones bajo alta carga operativa.

### 5) Validación y normalización con FluentValidation
- **Decisión**: validar DTOs antes del controlador usando `FluentValidation.AspNetCore`; normalizar `tipo_vehiculo` a minúscula previo a persistir/comparar.
- **Rationale**: evita datos inválidos y errores de matching por casing.

### 6) Algoritmo de cobro mixto con `decimal`
- **Decisión**: cálculo por minutos reales; si `< 1440` cobrar horas redondeadas hacia arriba; si `>= 1440` cobrar días completos + horas remanentes. Desglose IGV por división inversa (`neto = total / 1.18m`, `igv = total - neto`).
- **Rationale**: precisión financiera y cumplimiento tributario local.

### 7) TDD como puerta de calidad
- **Decisión**: definir primero pruebas unitarias de servicios críticos y pruebas de integración de endpoints antes de implementación full.
- **Rationale**: minimiza regresiones en reglas de negocio sensibles (aforo, tenant isolation, billing).

## Esquema Relacional Multi-Tenant (PostgreSQL DDL Conceptual)

```sql
CREATE TABLE tenants (
  id UUID PRIMARY KEY,
  ruc CHAR(11) NOT NULL UNIQUE,
  nombre_comercial VARCHAR(200) NOT NULL,
  estado VARCHAR(20) NOT NULL DEFAULT 'activo',
  creado_en TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE TABLE roles (
  id SMALLSERIAL PRIMARY KEY,
  codigo VARCHAR(30) NOT NULL UNIQUE -- owner, admin, operador
);

CREATE TABLE operadores (
  id UUID PRIMARY KEY,
  tenant_id UUID NOT NULL REFERENCES tenants(id),
  username VARCHAR(80) NOT NULL,
  password_hash VARCHAR(300) NOT NULL,
  role_id SMALLINT NOT NULL REFERENCES roles(id),
  activo BOOLEAN NOT NULL DEFAULT TRUE,
  creado_en TIMESTAMPTZ NOT NULL DEFAULT NOW(),
  UNIQUE (tenant_id, username)
);

CREATE TABLE tipos_vehiculo (
  id SMALLSERIAL PRIMARY KEY,
  codigo VARCHAR(20) NOT NULL UNIQUE -- carro, camion, trailer
);

CREATE TABLE espacios_parking (
  id UUID PRIMARY KEY,
  tenant_id UUID NOT NULL REFERENCES tenants(id),
  codigo VARCHAR(20) NOT NULL,
  tipo_vehiculo_permitido VARCHAR(20) NOT NULL,
  estado VARCHAR(20) NOT NULL DEFAULT 'libre', -- libre, ocupado, bloqueado
  version BIGINT NOT NULL DEFAULT 0,
  UNIQUE (tenant_id, codigo)
);

CREATE TABLE tarifas_tenant (
  id UUID PRIMARY KEY,
  tenant_id UUID NOT NULL REFERENCES tenants(id),
  tipo_vehiculo VARCHAR(20) NOT NULL,
  tarifa_hora NUMERIC(12,2) NOT NULL CHECK (tarifa_hora >= 0),
  tarifa_dia NUMERIC(12,2) NOT NULL CHECK (tarifa_dia >= 0),
  vigente_desde TIMESTAMPTZ NOT NULL DEFAULT NOW(),
  vigente_hasta TIMESTAMPTZ NULL,
  UNIQUE (tenant_id, tipo_vehiculo, vigente_desde)
);

CREATE TABLE estadias (
  id UUID PRIMARY KEY,
  tenant_id UUID NOT NULL REFERENCES tenants(id),
  placa VARCHAR(12) NOT NULL,
  tipo_vehiculo VARCHAR(20) NOT NULL,
  dni_cliente CHAR(8) NOT NULL,
  espacio_id UUID NOT NULL REFERENCES espacios_parking(id),
  operador_ingreso_id UUID NOT NULL REFERENCES operadores(id),
  operador_salida_id UUID NULL REFERENCES operadores(id),
  fecha_ingreso TIMESTAMPTZ NOT NULL,
  fecha_salida TIMESTAMPTZ NULL,
  estado VARCHAR(20) NOT NULL DEFAULT 'abierta', -- abierta, cerrada
  UNIQUE (tenant_id, placa, estado)
);

CREATE TABLE comprobantes (
  id UUID PRIMARY KEY,
  tenant_id UUID NOT NULL REFERENCES tenants(id),
  estadia_id UUID NOT NULL UNIQUE REFERENCES estadias(id),
  minutos_totales INT NOT NULL,
  subtotal_neto NUMERIC(12,2) NOT NULL,
  igv NUMERIC(12,2) NOT NULL,
  total NUMERIC(12,2) NOT NULL,
  creado_en TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE TABLE metodos_pago (
  id SMALLSERIAL PRIMARY KEY,
  codigo VARCHAR(30) NOT NULL UNIQUE
);
```

## Estructura C# (Controladores, Entidades y Servicios)

```text
ParkG/
  Controllers/
    AuthController.cs
    ParkingController.cs
  Application/
    Services/
      TenantProvisioningService.cs
      AuthService.cs
      ParkingIngresoService.cs
      ParkingSalidaService.cs
      BillingService.cs
    DTOs/
      RegisterTenantRequest.cs
      LoginRequest.cs
      IngresoRequest.cs
      SalidaRequest.cs
  Domain/
    Entities/
      Tenant.cs
      Operador.cs
      EspacioParking.cs
      TarifaTenant.cs
      Estadia.cs
      Comprobante.cs
  Infrastructure/
    Data/
      GarajeDbContext.cs
      Configurations/*.cs
    Security/
      JwtTokenService.cs
      TenantContext.cs
  Validation/
    RegisterTenantRequestValidator.cs
    IngresoRequestValidator.cs
    SalidaRequestValidator.cs
```

## Contratos Formales de API (OpenSpec Source-of-Truth)

### POST /api/auth/register
- Request:
  - `ruc` string(11), requerido, numérico
  - `nombreComercial` string(1..200), requerido
  - `ownerUsername` string, requerido
  - `ownerPassword` string, requerido
- Response `201`:
  - `tenantId` uuid
  - `ownerOperatorId` uuid
  - `defaultTariffs` array (`carro`, `camion`, `trailer`)
- Errores: `400` validación, `409` ruc duplicado

### POST /api/auth/login
- Request:
  - `ruc` string(11), requerido
  - `username` string, requerido
  - `password` string, requerido
- Response `200`:
  - `accessToken` jwt
  - `expiresIn` int
  - `tenantId` uuid
  - `role` string
- Errores: `401` credenciales inválidas, `403` operador inactivo

### POST /api/parking/ingreso
- Security: Bearer JWT con claim `tenant_id`
- Request:
  - `placa` string regex `^[A-Z0-9]{2,4}-[A-Z0-9]{3,4}$`
  - `tipoVehiculo` enum-string (`carro|camion|trailer`)
  - `espacioCodigo` string
  - `dniCliente` string(8) numérico
- Response `201`:
  - `ticketId` uuid
  - `fechaIngreso` datetime
  - `espacioCodigo` string
- Errores: `400` incompatibilidad tipo/casillero o validación, `409` espacio ocupado

### POST /api/parking/salida
- Security: Bearer JWT con claim `tenant_id`
- Request:
  - `placa` string, requerido
- Response `200`:
  - `ticketId` uuid
  - `minutosTotales` int
  - `subtotalNeto` decimal(12,2)
  - `igv` decimal(12,2)
  - `total` decimal(12,2)
- Errores: `404` estadía abierta no encontrada, `409` conflicto de concurrencia

## Estrategia TDD

- Unit tests (`xUnit`, `Moq`):
  - `BillingServiceTests`: reglas de minutos/horas/días e IGV.
  - `TenantIsolationQueryFilterTests`: scoping por `TenantId`.
  - `ParkingIngresoServiceTests`: validación cruzada tipo vehículo-espacio.
  - `AuthServiceTests`: emisión JWT con claim `tenant_id`.
- Integration tests (`Microsoft.AspNetCore.Mvc.Testing`):
  - `AuthRegisterFlowTests`: alta atómica tenant + bootstrap.
  - `ParkingIngresoConcurrencyTests`: doble intento mismo espacio.
  - `ParkingSalidaFlowTests`: cierre y facturación.

## Risks / Trade-offs

- [Error humano en consultas SQL manuales sin filtro tenant] → Mitigación: prohibir accesos directos no filtrados y centralizar en repositorios + `TenantContext`.
- [Bloqueos prolongados bajo alta concurrencia] → Mitigación: transacciones cortas, índices por `(tenant_id, estado)` y reintentos controlados.
- [Cambios tributarios (IGV)] → Mitigación: parametrizar tasa y versionar reglas de cálculo.
- [Neon cold starts/latencia] → Mitigación: connection pooling y timeouts resilientes.

## Migration Plan

1. Crear migración inicial de esquema multi-tenant y semillas globales (`roles`, `metodos_pago`, `tipos_vehiculo`).
2. Implementar `TenantContext`, autenticación JWT y filtros globales EF Core.
3. Implementar flujo de aprovisionamiento atómico.
4. Implementar ingreso/salida transaccional con bloqueos.
5. Implementar cálculo de cobro y comprobantes.
6. Ejecutar suites TDD unitarias e integración en CI.

Rollback:
- Revertir despliegue de API.
- Ejecutar down migration si no hay datos productivos o usar migración compensatoria.

## Decisiones Finales (Resoluciones)

1. **Numeración Fiscal:** Para la Fase 1 (MVP SaaS), se utilizará únicamente numeración de Ticket Interno (Ej. `TK-20260719-001`). La facturación electrónica con la SUNAT (XML/CDR) queda fuera del alcance actual y se abordará en la Fase 2 como un módulo adicional para los Tenants.
2. **Tolerancia de Gracia (Salida):** Queda postergado para la siguiente iteración para no añadir complejidad a la lógica de tiempo actual. En el futuro, se agregará como una columna `minutos_tolerancia` en la tabla `tarifas_tenant`.
3. **Auditoría Inmutable (Event Sourcing):** No se implementará por ahora. El diseño relacional actual cumple con los requisitos de trazabilidad básicos al registrar `operador_ingreso_id` y `operador_salida_id` en la tabla `estadias`.