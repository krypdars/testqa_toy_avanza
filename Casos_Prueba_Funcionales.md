# Casos de Prueba Funcionales - Sistema Registro de Mantenciones

## 1. Casos de Prueba - Autenticación y Sesión

### TC_AUTH_001: Login Exitoso
**Objetivo:** Verificar que un usuario con credenciales válidas puede acceder al sistema
**Precondiciones:** Usuario registrado en el sistema
**Pasos:**
1. Navegar a la página de login
2. Ingresar usuario válido
3. Ingresar contraseña válida
4. Hacer clic en "Ingresar"

**Resultado Esperado:** 
- Usuario es redirigido a la página principal
- Sesión se establece correctamente
- Variable Session["usuario"] contiene datos del usuario

**Datos de Prueba:**
- Usuario: admin_toyota
- Contraseña: password123

---

### TC_AUTH_002: Login con Credenciales Inválidas
**Objetivo:** Verificar que el sistema rechaza credenciales incorrectas
**Precondiciones:** Ninguna
**Pasos:**
1. Navegar a la página de login
2. Ingresar usuario inválido o contraseña incorrecta
3. Hacer clic en "Ingresar"

**Resultado Esperado:** 
- Mensaje de error mostrado
- Usuario permanece en página de login
- No se establece sesión

**Datos de Prueba:**
- Usuario: usuario_inexistente
- Contraseña: password_incorrecto

---

### TC_AUTH_003: Acceso Directo Sin Autenticación
**Objetivo:** Verificar que páginas protegidas redirigen al login
**Precondiciones:** Usuario no autenticado
**Pasos:**
1. Acceder directamente a URL: `/Registro_Mantenciones/Reg_Mantencion_01.aspx`

**Resultado Esperado:** 
- Redirección automática a `/default.aspx`
- No se permite acceso a la página protegida

---

### TC_AUTH_004: Expiración de Sesión
**Objetivo:** Verificar manejo de sesión expirada
**Precondiciones:** Usuario autenticado
**Pasos:**
1. Estar logueado en el sistema
2. Esperar timeout de sesión (o simular eliminando Session["usuario"])
3. Intentar acceder a cualquier página del sistema

**Resultado Esperado:** 
- Redirección automática al login
- Mensaje informativo sobre sesión expirada

---

## 2. Casos de Prueba - Página 1: Datos Iniciales

### TC_REG01_001: Ingreso de RUT Válido
**Objetivo:** Verificar validación correcta de RUT válido
**Precondiciones:** Usuario autenticado, cliente registrado en sistema
**Pasos:**
1. Acceder a Reg_Mantencion_01.aspx
2. Ingresar RUT válido: "12345678-5"
3. Ingresar Stock válido: "ABC123"
4. Hacer clic en "Continuar"

**Resultado Esperado:** 
- Validaciones pasan correctamente
- Redirección a Reg_Mantencion_02.aspx
- Datos cargados en sesión

**Datos de Prueba:**
- RUT: 12345678-5 (cliente registrado)
- Stock: ABC123 (vehículo existente)

---

### TC_REG01_002: RUT con Formato Incorrecto
**Objetivo:** Verificar validación de formato de RUT
**Precondiciones:** Usuario autenticado
**Pasos:**
1. Acceder a Reg_Mantencion_01.aspx
2. Ingresar RUT con formato incorrecto: "123456789"
3. Intentar continuar

**Resultado Esperado:** 
- Mensaje de validación: "Formato Rut no valido (Ej: 10981017-K)"
- No permite continuar
- Campo RUT marcado con error

**Datos de Prueba:**
- RUT inválido: 123456789, 12.345.678-5, 12345678K

---

### TC_REG01_003: RUT con Dígito Verificador Incorrecto
**Objetivo:** Verificar cálculo de dígito verificador
**Precondiciones:** Usuario autenticado
**Pasos:**
1. Acceder a Reg_Mantencion_01.aspx
2. Ingresar RUT con DV incorrecto: "12345678-9"
3. Intentar continuar

**Resultado Esperado:** 
- Mensaje de validación: "Rut ingresado no es valido"
- No permite continuar

**Datos de Prueba:**
- RUT con DV incorrecto: 12345678-9, 11111111-2

---

### TC_REG01_004: RUT con Números Repetidos
**Objetivo:** Verificar validación de RUTs con números repetidos
**Precondiciones:** Usuario autenticado
**Pasos:**
1. Acceder a Reg_Mantencion_01.aspx
2. Ingresar RUT con números repetidos: "11111111-1"
3. Intentar continuar

**Resultado Esperado:** 
- Mensaje de validación: "Rut ingresado no es valido"
- No permite continuar

**Datos de Prueba:**
- RUTs repetidos: 11111111-1, 22222222-2, 77777777-7

---

### TC_REG01_005: Cliente No Registrado
**Objetivo:** Verificar manejo de cliente no registrado
**Precondiciones:** Usuario autenticado, RUT válido pero no registrado
**Pasos:**
1. Acceder a Reg_Mantencion_01.aspx
2. Ingresar RUT válido pero no registrado: "87654321-0"
3. Ingresar Stock válido
4. Intentar continuar

**Resultado Esperado:** 
- Mensaje: "R.U.T. Ingresado no figura como cliente"
- Enlace para registrar cliente
- No permite continuar hasta registrar

**Datos de Prueba:**
- RUT no registrado: 87654321-0

---

### TC_REG01_006: Stock Inexistente
**Objetivo:** Verificar validación de stock inexistente
**Precondiciones:** Usuario autenticado, RUT válido
**Pasos:**
1. Acceder a Reg_Mantencion_01.aspx
2. Ingresar RUT válido: "12345678-5"
3. Ingresar Stock inexistente: "XYZ999"
4. Intentar continuar

**Resultado Esperado:** 
- Mensaje: "N° Stock no Existe o no Corresponde a la organización"
- No permite continuar
- Variables de sesión del vehículo se limpian

**Datos de Prueba:**
- Stock inexistente: XYZ999, NOEXISTE, 123456

---

### TC_REG01_007: Stock Sin Registro de Venta
**Objetivo:** Verificar manejo de vehículo sin venta registrada
**Precondiciones:** Usuario autenticado, RUT y Stock válidos, sin venta en SAP
**Pasos:**
1. Acceder a Reg_Mantencion_01.aspx
2. Ingresar RUT válido: "12345678-5"
3. Ingresar Stock sin venta: "DEF456"
4. Continuar

**Resultado Esperado:** 
- Mensaje informativo: "N° Stock no registra ingreso de venta"
- Permite continuar (no es bloqueante)
- Datos del vehículo se cargan normalmente

**Datos de Prueba:**
- Stock sin venta: DEF456

---

### TC_REG01_008: Vehículo Toyota Bencina
**Objetivo:** Verificar cálculo correcto de mantenciones para Toyota bencina
**Precondiciones:** Usuario autenticado, vehículo Toyota con combustible bencina
**Pasos:**
1. Ingresar datos de vehículo Toyota bencina
2. Verificar variable Session["MantenAsumar"]

**Resultado Esperado:** 
- Session["MantenAsumar"] = "10"
- Datos del vehículo cargados correctamente

**Datos de Prueba:**
- Marca: TOY, Combustible: BENCINA

---

### TC_REG01_009: Vehículo Toyota Híbrido
**Objetivo:** Verificar cálculo para vehículo híbrido
**Precondiciones:** Usuario autenticado, vehículo Toyota híbrido
**Pasos:**
1. Ingresar datos de vehículo Toyota híbrido
2. Verificar variable Session["MantenAsumar"]

**Resultado Esperado:** 
- Session["MantenAsumar"] = "10"
- Datos del vehículo cargados correctamente

**Datos de Prueba:**
- Marca: TOY, Combustible: Híbrido

---

### TC_REG01_010: Vehículo Diesel
**Objetivo:** Verificar cálculo para vehículo diesel
**Precondiciones:** Usuario autenticado, vehículo diesel
**Pasos:**
1. Ingresar datos de vehículo diesel
2. Verificar variable Session["MantenAsumar"]

**Resultado Esperado:** 
- Session["MantenAsumar"] = "5"
- Datos del vehículo cargados correctamente

**Datos de Prueba:**
- Combustible: DIESEL

---

## 3. Casos de Prueba - Página 2: Datos de Mantención

### TC_REG02_001: Selección de Mantención Válida
**Objetivo:** Verificar selección correcta de tipo de mantención
**Precondiciones:** Datos de página 1 completados correctamente
**Pasos:**
1. Acceder a Reg_Mantencion_02.aspx
2. Seleccionar mantención: "10000 Kms"
3. Ingresar kilometraje: "9500"
4. Seleccionar asesor
5. Continuar

**Resultado Esperado:** 
- Mantención seleccionada correctamente
- Kilometraje dentro del rango aceptable
- Continúa a página 3

**Datos de Prueba:**
- Mantención: 10 (10000 Kms)
- Kilometraje: 9500

---

### TC_REG02_002: Kilometraje Fuera de Rango
**Objetivo:** Verificar validación de kilometraje inconsistente
**Precondiciones:** Datos de página 1 completados
**Pasos:**
1. Acceder a Reg_Mantencion_02.aspx
2. Seleccionar mantención: "10000 Kms"
3. Ingresar kilometraje fuera de rango: "20000"
4. Intentar continuar

**Resultado Esperado:** 
- Mensaje de confirmación: "Kilometraje ingresado no coincide con mantención seleccionada ¿desea continuar?"
- Opción de confirmar o cancelar

**Datos de Prueba:**
- Mantención: 10 (10000 Kms)
- Kilometraje: 20000 (fuera del rango 5000-15000)

---

### TC_REG02_003: Kilometraje No Numérico
**Objetivo:** Verificar validación de entrada no numérica en kilometraje
**Precondiciones:** Datos de página 1 completados
**Pasos:**
1. Acceder a Reg_Mantencion_02.aspx
2. Seleccionar mantención válida
3. Ingresar texto en kilometraje: "abc"
4. Intentar continuar

**Resultado Esperado:** 
- Mensaje: "Kilometraje debe ingresar valores numéricos válidos"
- No permite continuar

**Datos de Prueba:**
- Kilometraje inválido: abc, 10.5, 10,000

---

### TC_REG02_004: Sin Seleccionar Mantención
**Objetivo:** Verificar validación de mantención obligatoria
**Precondiciones:** Datos de página 1 completados
**Pasos:**
1. Acceder a Reg_Mantencion_02.aspx
2. No seleccionar mantención
3. Ingresar kilometraje válido
4. Intentar continuar

**Resultado Esperado:** 
- Mensaje: "Debe seleccionar Mantencion"
- No permite continuar

---

### TC_REG02_005: Carga de Archivo PDF Válido
**Objetivo:** Verificar carga correcta de archivo PDF
**Precondiciones:** Datos completados, función de adjuntos habilitada
**Pasos:**
1. Completar datos de mantención
2. Seleccionar archivo PDF válido
3. Verificar habilitación del botón continuar

**Resultado Esperado:** 
- Archivo se carga correctamente
- Botón "Continuar" se habilita
- Validación JavaScript pasa

**Datos de Prueba:**
- Archivo: documento.pdf (válido)

---

### TC_REG02_006: Carga de Archivo No PDF
**Objetivo:** Verificar rechazo de archivos no PDF
**Precondiciones:** Datos completados, función de adjuntos habilitada
**Pasos:**
1. Completar datos de mantención
2. Intentar cargar archivo no PDF (ej: .doc, .jpg)
3. Verificar validación

**Resultado Esperado:** 
- Archivo rechazado
- Botón "Continuar" permanece deshabilitado
- Mensaje de error sobre formato

**Datos de Prueba:**
- Archivos inválidos: documento.doc, imagen.jpg, archivo.txt

---

### TC_REG02_007: Vehículo con Prepagos Contratados
**Objetivo:** Verificar visualización de prepagos disponibles
**Precondiciones:** Vehículo con prepagos contratados
**Pasos:**
1. Acceder con vehículo que tiene prepagos
2. Verificar información de prepagos mostrada

**Resultado Esperado:** 
- Información de prepagos visible
- Opciones de prepago disponibles
- Datos correctos según base de datos

---

### TC_REG02_008: Vehículo en Campaña
**Objetivo:** Verificar notificación de campañas activas
**Precondiciones:** Vehículo incluido en campaña
**Pasos:**
1. Acceder con vehículo en campaña
2. Verificar mensaje de campaña

**Resultado Esperado:** 
- Mensaje: "Este stock esta en X campaña(s)"
- Información visible para el usuario

---

### TC_REG02_009: Programa T10 Elegible
**Objetivo:** Verificar habilitación de opciones T10
**Precondiciones:** Vehículo elegible para programa T10
**Pasos:**
1. Acceder con vehículo elegible T10
2. Verificar opciones T10 disponibles

**Resultado Esperado:** 
- Opciones T10 habilitadas
- Información del programa visible

---

## 4. Casos de Prueba - Página 3: Confirmación

### TC_REG03_001: Revisión de Datos Correctos
**Objetivo:** Verificar visualización correcta de datos para confirmación
**Precondiciones:** Páginas 1 y 2 completadas correctamente
**Pasos:**
1. Acceder a Reg_Mantencion_03.aspx
2. Verificar todos los datos mostrados
3. Confirmar información

**Resultado Esperado:** 
- Todos los datos se muestran correctamente
- Información coincide con lo ingresado
- Permite continuar a página final

---

### TC_REG03_002: Modificación de Datos
**Objetivo:** Verificar posibilidad de volver y modificar datos
**Precondiciones:** En página de confirmación
**Pasos:**
1. Estar en página de confirmación
2. Hacer clic en "Volver" o "Modificar"
3. Cambiar datos en páginas anteriores
4. Volver a confirmación

**Resultado Esperado:** 
- Permite volver a páginas anteriores
- Cambios se reflejan en confirmación
- Datos actualizados correctamente

---

## 5. Casos de Prueba - Página 4: Finalización

### TC_REG04_001: Registro Exitoso
**Objetivo:** Verificar grabación exitosa del registro
**Precondiciones:** Todas las páginas anteriores completadas
**Pasos:**
1. Llegar a página de finalización
2. Verificar grabación en base de datos
3. Verificar generación de código de garantía

**Resultado Esperado:** 
- Registro guardado en base de datos
- Código de garantía generado (formato: AAAAMMDDHHMMSS)
- Mensaje de confirmación mostrado

---

### TC_REG04_002: Error en Grabación
**Objetivo:** Verificar manejo de errores en grabación
**Precondiciones:** Simular error de base de datos
**Pasos:**
1. Intentar completar registro con error de BD
2. Verificar manejo del error

**Resultado Esperado:** 
- Error capturado correctamente
- Mensaje de error informativo
- No se corrompen datos

---

## 6. Casos de Prueba - Flujos Completos

### TC_FLOW_001: Flujo Completo Exitoso
**Objetivo:** Verificar flujo completo de registro sin errores
**Precondiciones:** Datos válidos disponibles
**Pasos:**
1. Login exitoso
2. Completar página 1 con datos válidos
3. Completar página 2 con datos válidos
4. Confirmar en página 3
5. Finalizar en página 4

**Resultado Esperado:** 
- Flujo completo sin errores
- Registro guardado exitosamente
- Usuario puede iniciar nuevo registro

**Datos de Prueba:**
- RUT: 12345678-5
- Stock: ABC123
- Mantención: 10000 Kms
- Kilometraje: 9500

---

### TC_FLOW_002: Flujo con Validaciones
**Objetivo:** Verificar manejo de errores en flujo completo
**Pasos:**
1. Intentar con RUT inválido (corregir)
2. Intentar con Stock inválido (corregir)
3. Completar correctamente

**Resultado Esperado:** 
- Validaciones funcionan correctamente
- Permite corrección de errores
- Flujo se completa tras correcciones

---

## 7. Casos de Prueba - Rendimiento

### TC_PERF_001: Tiempo de Respuesta Validaciones
**Objetivo:** Verificar tiempo de respuesta de validaciones
**Criterio:** Validaciones < 2 segundos
**Pasos:**
1. Medir tiempo de validación de RUT
2. Medir tiempo de validación de Stock
3. Medir tiempo de consulta SAP

**Resultado Esperado:** 
- Validación RUT < 100ms
- Validación Stock < 500ms
- Consulta SAP < 2000ms

---

### TC_PERF_002: Carga Concurrente
**Objetivo:** Verificar comportamiento con múltiples usuarios
**Criterio:** 20 usuarios concurrentes sin degradación
**Pasos:**
1. Simular 20 usuarios registrando mantenciones
2. Medir tiempos de respuesta
3. Verificar integridad de datos

**Resultado Esperado:** 
- Tiempos de respuesta aceptables
- No hay conflictos de datos
- Todos los registros se completan

---

## 8. Casos de Prueba - Seguridad

### TC_SEC_001: Inyección SQL
**Objetivo:** Verificar protección contra inyección SQL
**Pasos:**
1. Intentar inyección en campo RUT: "'; DROP TABLE --"
2. Intentar inyección en campo Stock
3. Verificar que no se ejecuten comandos maliciosos

**Resultado Esperado:** 
- Comandos maliciosos no se ejecutan
- Datos se validan correctamente
- Base de datos permanece íntegra

---

### TC_SEC_002: XSS (Cross-Site Scripting)
**Objetivo:** Verificar protección contra XSS
**Pasos:**
1. Intentar insertar script en campos: "<script>alert('XSS')</script>"
2. Verificar que no se ejecute código JavaScript

**Resultado Esperado:** 
- Scripts no se ejecutan
- Contenido se escapa correctamente
- No hay vulnerabilidades XSS

---

### TC_SEC_003: Acceso No Autorizado
**Objetivo:** Verificar control de acceso
**Pasos:**
1. Intentar acceder sin autenticación
2. Intentar manipular variables de sesión
3. Intentar acceso directo a páginas

**Resultado Esperado:** 
- Acceso denegado sin autenticación
- Variables de sesión protegidas
- Redirección correcta al login

---

## 9. Matriz de Trazabilidad

| Funcionalidad | Casos de Prueba | Prioridad | Estado |
|---------------|-----------------|-----------|---------|
| Autenticación | TC_AUTH_001-004 | Alta | Pendiente |
| Validación RUT | TC_REG01_002-004 | Alta | Pendiente |
| Validación Stock | TC_REG01_006-010 | Alta | Pendiente |
| Validación Kilometraje | TC_REG02_002-003 | Media | Pendiente |
| Carga Archivos | TC_REG02_005-006 | Media | Pendiente |
| Flujo Completo | TC_FLOW_001-002 | Alta | Pendiente |
| Rendimiento | TC_PERF_001-002 | Media | Pendiente |
| Seguridad | TC_SEC_001-003 | Alta | Pendiente |

---

## 10. Criterios de Aceptación

### Criterios Funcionales
- ✅ Todas las validaciones de negocio funcionan correctamente
- ✅ Flujo completo se ejecuta sin errores
- ✅ Datos se guardan correctamente en base de datos
- ✅ Integración con SAP funciona correctamente

### Criterios de Rendimiento
- ✅ Validaciones responden en < 2 segundos
- ✅ Sistema soporta 20 usuarios concurrentes
- ✅ Consultas a base de datos optimizadas

### Criterios de Seguridad
- ✅ No hay vulnerabilidades de inyección SQL
- ✅ No hay vulnerabilidades XSS
- ✅ Control de acceso funciona correctamente
- ✅ Sesiones se manejan de forma segura

### Criterios de Usabilidad
- ✅ Mensajes de error son claros y útiles
- ✅ Flujo de navegación es intuitivo
- ✅ Validaciones en tiempo real funcionan
- ✅ Interfaz es responsive y accesible
