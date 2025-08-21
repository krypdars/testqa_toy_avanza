# Plan de Quality Assurance (QA) - Sistema Registro de Mantenciones

## 1. Análisis del Sistema

### 1.1 Descripción General
El sistema "Registro de Mantenciones" es una aplicación web ASP.NET que permite registrar mantenciones de vehículos Toyota. La aplicación consta de 4 páginas principales que forman un flujo secuencial de registro.

### 1.2 Arquitectura y Tecnologías
- **Framework**: ASP.NET Web Forms (.NET Framework)
- **Lenguaje**: C# (Code-behind)
- **Base de Datos**: SQL Server
- **Controles**: ASP.NET Web Controls, controles personalizados (.ascx)
- **Validación**: Server-side y Client-side validation
- **Integración**: Web Services SOAP para consultas SAP

### 1.3 Flujo de Páginas

#### Página 1: `Reg_Mantencion_01.aspx` - Datos Iniciales
**Funcionalidades:**
- Validación de autenticación de usuario
- Ingreso y validación de RUT del cliente
- Ingreso y validación de N° Stock del vehículo
- Validación de existencia del cliente en base de datos
- Validación de existencia del vehículo
- Consulta a servicio SAP para verificar registro de venta
- Carga de datos del vehículo en sesión

**Validaciones Implementadas:**
- RUT: Formato, dígito verificador, números repetidos
- Stock: Existencia en BD, correspondencia con organización
- Cliente: Existencia en sistema

#### Página 2: `Reg_Mantencion_02.aspx` - Datos de Mantención
**Funcionalidades:**
- Selección de tipo de mantención
- Ingreso de kilometraje
- Selección de asesor
- Validación de coherencia kilometraje vs mantención
- Manejo de prepagos y campañas
- Carga de archivos adjuntos (PDF)
- Validación de programa T10

#### Página 3: `Reg_Mantencion_03.aspx` - Confirmación
**Funcionalidades:**
- Revisión de datos ingresados
- Confirmación de información
- Preparación para grabación final

#### Página 4: `Reg_Mantencion_04.aspx` - Finalización
**Funcionalidades:**
- Grabación final en base de datos
- Generación de código de garantía
- Confirmación de registro exitoso

## 2. Plan de Pruebas

### 2.1 Pruebas Unitarias

#### 2.1.1 Validaciones de RUT (`Reg_Mantencion_01.aspx.cs`)

```csharp
[TestClass]
public class ValidacionRutTests
{
    [TestMethod]
    public void ValidaRut_RutValido_RetornaTrue()
    {
        // Arrange
        var page = new Registro_Mantenciones_Reg_Mantencion_01();
        var args = new ServerValidateEventArgs("12345678-5", true);
        
        // Act
        page.ValidaRut2(null, args);
        
        // Assert
        Assert.IsTrue(args.IsValid);
    }
    
    [TestMethod]
    public void ValidaRut_RutInvalido_RetornaFalse()
    {
        // Arrange
        var page = new Registro_Mantenciones_Reg_Mantencion_01();
        var args = new ServerValidateEventArgs("12345678-9", true);
        
        // Act
        page.ValidaRut2(null, args);
        
        // Assert
        Assert.IsFalse(args.IsValid);
    }
    
    [TestMethod]
    public void ValidaRut_NumerosRepetidos_RetornaFalse()
    {
        // Arrange
        var page = new Registro_Mantenciones_Reg_Mantencion_01();
        var args = new ServerValidateEventArgs("11111111-1", true);
        
        // Act
        page.ValidaRut2(null, args);
        
        // Assert
        Assert.IsFalse(args.IsValid);
    }
    
    [TestMethod]
    public void ValidaRut_FormatoIncorrecto_RetornaFalse()
    {
        // Arrange
        var page = new Registro_Mantenciones_Reg_Mantencion_01();
        var args = new ServerValidateEventArgs("123456789", true);
        
        // Act
        page.ValidaRut2(null, args);
        
        // Assert
        Assert.IsFalse(args.IsValid);
    }
}
```

#### 2.1.2 Validaciones de Stock

```csharp
[TestClass]
public class ValidacionStockTests
{
    private Mock<IDbConnection> _mockConnection;
    private Mock<IDbCommand> _mockCommand;
    
    [TestInitialize]
    public void Setup()
    {
        _mockConnection = new Mock<IDbConnection>();
        _mockCommand = new Mock<IDbCommand>();
    }
    
    [TestMethod]
    public void ValidaStock_StockExistente_RetornaTrue()
    {
        // Arrange
        var page = new Registro_Mantenciones_Reg_Mantencion_01();
        var args = new ServerValidateEventArgs("ABC123", true);
        
        // Mock database response
        var mockDataTable = new DataTable();
        mockDataTable.Rows.Add(/* datos del vehículo */);
        
        // Act
        page.Valida_Stock2(null, args);
        
        // Assert
        Assert.IsTrue(args.IsValid);
    }
    
    [TestMethod]
    public void ValidaStock_StockInexistente_RetornaFalse()
    {
        // Arrange
        var page = new Registro_Mantenciones_Reg_Mantencion_01();
        var args = new ServerValidateEventArgs("XYZ999", true);
        
        // Mock empty database response
        var mockDataTable = new DataTable();
        
        // Act
        page.Valida_Stock2(null, args);
        
        // Assert
        Assert.IsFalse(args.IsValid);
    }
}
```

#### 2.1.3 Servicios Web SAP

```csharp
[TestClass]
public class ServicioSAPTests
{
    [TestMethod]
    public void InvokeService_StockConVenta_RetornaOk()
    {
        // Arrange
        var page = new Registro_Mantenciones_Reg_Mantencion_01();
        string stock = "ABC123";
        
        // Act
        string resultado = page.InvokeService(stock);
        
        // Assert
        Assert.AreEqual("Ok", resultado);
    }
    
    [TestMethod]
    public void InvokeService_StockSinVenta_RetornaNotOk()
    {
        // Arrange
        var page = new Registro_Mantenciones_Reg_Mantencion_01();
        string stock = "XYZ999";
        
        // Act
        string resultado = page.InvokeService(stock);
        
        // Assert
        Assert.AreEqual("NotOk", resultado);
    }
    
    [TestMethod]
    public void LeerXML_XMLConMensaje_RetornaNotOk()
    {
        // Arrange
        var page = new Registro_Mantenciones_Reg_Mantencion_01();
        string xml = @"<?xml version='1.0'?>
                      <response>
                          <mensaje>Error</mensaje>
                      </response>";
        
        // Act
        string resultado = page.LeerXML(xml);
        
        // Assert
        Assert.AreEqual("NotOk", resultado);
    }
}
```

### 2.2 Pruebas Funcionales

#### 2.2.1 Casos de Prueba - Login y Autenticación

| ID | Descripción | Pasos | Resultado Esperado |
|----|-------------|-------|-------------------|
| F001 | Login exitoso | 1. Acceder a la aplicación<br>2. Ingresar credenciales válidas | Redirección a página principal |
| F002 | Login fallido | 1. Acceder a la aplicación<br>2. Ingresar credenciales inválidas | Mensaje de error, permanece en login |
| F003 | Sesión expirada | 1. Estar logueado<br>2. Esperar timeout de sesión<br>3. Intentar acceder a página protegida | Redirección a página de login |
| F004 | Acceso directo sin login | 1. Acceder directamente a URL protegida sin estar logueado | Redirección a página de login |

#### 2.2.2 Casos de Prueba - Registro de Mantenciones

| ID | Descripción | Pasos | Resultado Esperado |
|----|-------------|-------|-------------------|
| F005 | Registro completo exitoso | 1. Ingresar RUT válido<br>2. Ingresar Stock válido<br>3. Completar datos de mantención<br>4. Confirmar registro | Registro guardado exitosamente |
| F006 | RUT inválido | 1. Ingresar RUT con formato incorrecto<br>2. Intentar continuar | Mensaje de validación, no permite continuar |
| F007 | Stock inexistente | 1. Ingresar RUT válido<br>2. Ingresar Stock que no existe<br>3. Intentar continuar | Mensaje de error, no permite continuar |
| F008 | Cliente no registrado | 1. Ingresar RUT válido pero no registrado<br>2. Ingresar Stock válido | Mensaje con enlace para registrar cliente |
| F009 | Kilometraje inconsistente | 1. Completar datos correctamente<br>2. Ingresar kilometraje muy diferente a mantención<br>3. Intentar continuar | Mensaje de confirmación |
| F010 | Archivo adjunto inválido | 1. Completar datos correctamente<br>2. Adjuntar archivo no PDF<br>3. Intentar continuar | Mensaje de error, no permite continuar |

#### 2.2.3 Casos de Prueba - Validaciones de Negocio

| ID | Descripción | Pasos | Resultado Esperado |
|----|-------------|-------|-------------------|
| F011 | Vehículo sin registro de venta | 1. Ingresar Stock sin venta registrada | Mensaje informativo, permite continuar |
| F012 | Vehículo en campaña | 1. Ingresar Stock que está en campaña | Mensaje informativo sobre campañas |
| F013 | Programa T10 | 1. Ingresar vehículo elegible para T10 | Opciones de T10 habilitadas |
| F014 | Prepagos contratados | 1. Ingresar vehículo con prepagos | Información de prepagos disponibles |

### 2.3 Pruebas de Regresión

#### 2.3.1 Suite de Regresión Básica

```csharp
[TestClass]
public class RegresionBasicaTests
{
    [TestMethod]
    public void Regresion_FlujoCompletoBasico()
    {
        // Simula el flujo completo básico
        // Página 1 -> Página 2 -> Página 3 -> Página 4
    }
    
    [TestMethod]
    public void Regresion_ValidacionesEsenciales()
    {
        // Verifica que todas las validaciones críticas funcionen
    }
    
    [TestMethod]
    public void Regresion_IntegracionSAP()
    {
        // Verifica que la integración con SAP funcione
    }
}
```

#### 2.3.2 Casos de Regresión por Módulo

**Módulo de Validaciones:**
- Validación de RUT con diferentes formatos
- Validación de Stock con diferentes escenarios
- Validación de Cliente existente/no existente

**Módulo de Integración SAP:**
- Consulta de venta exitosa
- Consulta de venta fallida
- Timeout de servicio
- Respuesta malformada

**Módulo de Sesión:**
- Manejo correcto de variables de sesión
- Limpieza de sesión al logout
- Persistencia de datos entre páginas

### 2.4 Pruebas de Rendimiento

#### 2.4.1 Pruebas de Carga
- Simular 50 usuarios concurrentes
- Medir tiempo de respuesta de validaciones
- Medir tiempo de respuesta de consultas a BD
- Medir tiempo de respuesta de servicios SAP

#### 2.4.2 Pruebas de Estrés
- Incrementar usuarios hasta encontrar punto de quiebre
- Probar con base de datos con gran volumen de datos
- Probar timeout de servicios externos

### 2.5 Pruebas de Seguridad

#### 2.5.1 Autenticación y Autorización
- Verificar que todas las páginas requieren autenticación
- Probar inyección SQL en campos de entrada
- Verificar manejo seguro de sesiones

#### 2.5.2 Validación de Entrada
- Probar XSS en campos de texto
- Verificar validación server-side además de client-side
- Probar carga de archivos maliciosos

## 3. Herramientas y Configuración

### 3.1 Herramientas de Testing
- **Unit Testing**: MSTest o NUnit
- **Mocking**: Moq Framework
- **UI Testing**: Selenium WebDriver
- **Performance Testing**: JMeter o LoadRunner
- **Code Coverage**: Visual Studio Code Coverage

### 3.2 Configuración del Entorno de Pruebas
- Base de datos de pruebas con datos de test
- Mock de servicios SAP para pruebas unitarias
- Configuración de CI/CD para ejecución automática

### 3.3 Datos de Prueba
- RUTs válidos e inválidos
- Stocks existentes e inexistentes
- Clientes registrados y no registrados
- Vehículos con y sin venta registrada

## 4. Criterios de Aceptación

### 4.1 Cobertura de Código
- Mínimo 80% de cobertura en métodos críticos
- 100% de cobertura en validaciones de negocio

### 4.2 Criterios de Calidad
- Todas las pruebas unitarias deben pasar
- Todas las pruebas funcionales críticas deben pasar
- Tiempo de respuesta menor a 3 segundos para operaciones normales
- No debe haber vulnerabilidades de seguridad críticas

### 4.3 Criterios de Regresión
- Suite de regresión debe ejecutarse en cada deploy
- No debe haber regresiones en funcionalidades existentes
- Nuevas funcionalidades deben incluir pruebas correspondientes

## 5. Cronograma de Implementación

### Fase 1: Pruebas Unitarias (2 semanas)
- Implementar pruebas de validaciones
- Implementar pruebas de servicios
- Configurar framework de testing

### Fase 2: Pruebas Funcionales (2 semanas)
- Implementar casos de prueba críticos
- Configurar Selenium para UI testing
- Crear datos de prueba

### Fase 3: Pruebas de Regresión (1 semana)
- Implementar suite de regresión
- Configurar ejecución automática
- Documentar casos de prueba

### Fase 4: Pruebas de Rendimiento y Seguridad (1 semana)
- Configurar herramientas de performance
- Ejecutar pruebas de seguridad
- Documentar resultados

## 6. Riesgos y Mitigaciones

### 6.1 Riesgos Identificados
- **Dependencia de servicios SAP**: Puede afectar pruebas
  - *Mitigación*: Implementar mocks y stubs
- **Base de datos compartida**: Puede causar conflictos
  - *Mitigación*: Usar base de datos dedicada para pruebas
- **Datos sensibles**: RUTs y datos de clientes reales
  - *Mitigación*: Usar datos sintéticos para pruebas

### 6.2 Plan de Contingencia
- Backup de datos de prueba
- Rollback automático en caso de fallas críticas
- Notificación automática de fallas en CI/CD

## 7. Métricas y Reportes

### 7.1 Métricas de Calidad
- Porcentaje de pruebas que pasan
- Cobertura de código
- Número de defectos encontrados por fase
- Tiempo promedio de ejecución de pruebas

### 7.2 Reportes
- Reporte diario de ejecución de pruebas
- Reporte semanal de cobertura
- Reporte mensual de tendencias de calidad

---

*Este plan debe ser revisado y actualizado regularmente conforme evolucione el sistema y se identifiquen nuevos requerimientos de testing.*
