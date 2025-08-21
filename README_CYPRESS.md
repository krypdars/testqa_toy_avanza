# 🚗 Pruebas Automatizadas con Cypress - Registro de Mantenciones Toyota

## 📋 Tabla de Contenidos
- [Descripción del Proyecto](#descripción-del-proyecto)
- [Instalación y Configuración](#instalación-y-configuración)
- [Estructura del Proyecto](#estructura-del-proyecto)
- [Ejecución de Pruebas](#ejecución-de-pruebas)
- [Tipos de Pruebas](#tipos-de-pruebas)
- [Configuración del Entorno](#configuración-del-entorno)
- [Datos de Prueba](#datos-de-prueba)
- [Reportes y Resultados](#reportes-y-resultados)
- [CI/CD](#cicd)
- [Troubleshooting](#troubleshooting)

---

## 📖 Descripción del Proyecto

Este proyecto contiene las pruebas automatizadas E2E (End-to-End) para el sistema de **Registro de Mantenciones** de Toyota Chile, desarrolladas con **Cypress**.

### Funcionalidades Cubiertas
- ✅ **Autenticación y Login**
- ✅ **Validación de RUT chileno**
- ✅ **Validación de Stock de vehículos**
- ✅ **Registro completo de mantenciones**
- ✅ **Integración con servicios SAP**
- ✅ **Flujos de navegación**
- ✅ **Validaciones de seguridad**
- ✅ **Pruebas de rendimiento**

---

## 🚀 Instalación y Configuración

### Prerrequisitos
```bash
# Verificar versiones requeridas
node --version    # >= 16.0.0
npm --version     # >= 8.0.0
```

### Paso 1: Instalar Dependencias
```bash
# Navegar al directorio del proyecto
cd Registro_Mantenciones

# Instalar dependencias
npm install

# Verificar instalación de Cypress
npx cypress verify
```

### Paso 2: Configurar Variables de Entorno
Editar `cypress.config.js` con la configuración de tu entorno:

```javascript
// cypress.config.js
module.exports = defineConfig({
  e2e: {
    baseUrl: 'http://localhost:8080', // 👈 Cambiar por tu URL
    env: {
      testUser: 'tu_usuario',         // 👈 Usuario de prueba
      testPassword: 'tu_password',    // 👈 Contraseña de prueba
      validRut: '12345678-5',         // 👈 RUT válido de prueba
      validStock: 'ABC123'            // 👈 Stock válido de prueba
    }
  }
})
```

### Paso 3: Configurar Servidor Web
```bash
# Para IIS (Windows)
# 1. Configurar sitio web en IIS apuntando a la carpeta del proyecto
# 2. Configurar puerto (ej: 8080)
# 3. Verificar que la aplicación funciona: http://localhost:8080

# Para desarrollo con Visual Studio
# 1. Abrir proyecto en Visual Studio
# 2. Ejecutar con IIS Express
# 3. Anotar la URL generada
```

---

## 📁 Estructura del Proyecto

```
Registro_Mantenciones/
├── cypress/
│   ├── e2e/                          # Pruebas E2E
│   │   ├── auth/                     # Pruebas de autenticación
│   │   │   ├── login.cy.js          # Login y logout
│   │   │   └── session.cy.js        # Manejo de sesiones
│   │   ├── registro/                 # Pruebas por página
│   │   │   ├── pagina1-datos-iniciales.cy.js
│   │   │   ├── pagina2-datos-mantencion.cy.js
│   │   │   ├── pagina3-confirmacion.cy.js
│   │   │   └── pagina4-finalizacion.cy.js
│   │   ├── flujos/                   # Flujos completos
│   │   │   ├── flujo-completo-exitoso.cy.js
│   │   │   ├── flujo-con-errores.cy.js
│   │   │   └── flujos-edge-cases.cy.js
│   │   └── validaciones/             # Validaciones específicas
│   │       ├── validacion-rut.cy.js
│   │       ├── validacion-stock.cy.js
│   │       └── validacion-kilometraje.cy.js
│   ├── fixtures/                     # Datos de prueba
│   │   ├── usuarios.json            # Usuarios de prueba
│   │   ├── vehiculos.json           # Vehículos de prueba
│   │   └── clientes.json            # Clientes de prueba
│   ├── support/                      # Archivos de soporte
│   │   ├── commands.js              # Comandos personalizados
│   │   ├── e2e.js                   # Configuración global
│   │   └── page-objects/            # Page Objects
│   │       ├── LoginPage.js
│   │       ├── RegistroPage1.js
│   │       ├── RegistroPage2.js
│   │       ├── RegistroPage3.js
│   │       └── RegistroPage4.js
│   └── downloads/                    # Archivos descargados
├── cypress.config.js                 # Configuración principal
├── package.json                      # Dependencias y scripts
└── README_CYPRESS.md                 # Esta documentación
```

---

## ▶️ Ejecución de Pruebas

### Modo Interactivo (Desarrollo)
```bash
# Abrir Cypress Test Runner
npm run cy:open

# Seleccionar navegador y ejecutar pruebas individualmente
```

### Modo Headless (CI/CD)
```bash
# Ejecutar todas las pruebas
npm run cy:run

# Ejecutar con navegador específico
npm run cy:run:chrome
npm run cy:run:firefox
npm run cy:run:edge

# Ejecutar con cabeza visible (para debugging)
npm run cy:run:headed
```

### Ejecución por Categorías
```bash
# Pruebas de autenticación
npm run test:auth

# Pruebas de registro por páginas
npm run test:registro

# Flujos completos
npm run test:flujos

# Validaciones específicas
npm run test:validaciones

# Pruebas críticas (smoke tests)
npm run test:smoke

# Suite completa de regresión
npm run test:regression
```

### Ejecución por Página Específica
```bash
# Página 1 - Datos iniciales
npm run test:pagina1

# Página 2 - Datos de mantención
npm run test:pagina2

# Página 3 - Confirmación
npm run test:pagina3

# Página 4 - Finalización
npm run test:pagina4
```

### Ejecución por Validación Específica
```bash
# Validaciones de RUT
npm run test:rut

# Validaciones de Stock
npm run test:stock

# Validaciones de Kilometraje
npm run test:kilometraje
```

---

## 🧪 Tipos de Pruebas

### 1. Pruebas de Autenticación
- **Login exitoso** con diferentes roles
- **Login fallido** con credenciales inválidas
- **Validaciones de seguridad** (SQL Injection, XSS)
- **Manejo de sesiones**

### 2. Pruebas de Validación de RUT
- **Formato correcto** (########-#)
- **Dígito verificador** válido
- **Números repetidos** (rechazados)
- **Cliente registrado** en sistema

### 3. Pruebas de Validación de Stock
- **Stock existente** en base de datos
- **Correspondencia** con organización
- **Integración SAP** para verificar venta
- **Carga de datos** del vehículo

### 4. Pruebas de Flujo Completo
- **Registro exitoso** completo
- **Diferentes tipos** de vehículos (Toyota Bencina, Híbrido, Diesel)
- **Diferentes mantenciones** (30 días, 5K, 10K, etc.)
- **Navegación** entre páginas

### 5. Pruebas de Casos Especiales
- **Vehículos sin venta** registrada
- **Vehículos en campaña**
- **Vehículos con prepagos**
- **Programa T10**

### 6. Pruebas de Seguridad
- **Inyección SQL** en campos de entrada
- **Cross-Site Scripting (XSS)**
- **Validación server-side**
- **Control de acceso**

### 7. Pruebas de Rendimiento
- **Tiempo de carga** de páginas
- **Tiempo de validaciones**
- **Tiempo de flujo completo**
- **Carga concurrente**

---

## ⚙️ Configuración del Entorno

### Base de Datos de Pruebas
```sql
-- Crear base de datos de pruebas
CREATE DATABASE RegistroMantenciones_Test;

-- Configurar connection string en web.config
<connectionStrings>
    <add name="SAP_WEBConnectionString" 
         connectionString="Server=localhost;Database=RegistroMantenciones_Test;Integrated Security=true;" 
         providerName="System.Data.SqlClient" />
</connectionStrings>
```

### Datos de Prueba Requeridos
```sql
-- Insertar usuarios de prueba
INSERT INTO Usuarios (username, password, role) VALUES 
('admin_toyota', 'password123', 'admin'),
('asesor_ventas', 'asesor123', 'asesor');

-- Insertar clientes de prueba
INSERT INTO Clientes (rut, dv, nombre, apellido) VALUES 
(12345678, '5', 'Juan Carlos', 'Pérez González'),
(87654321, '0', 'María Elena', 'González Silva');

-- Insertar vehículos de prueba
INSERT INTO Vehiculos (stock, marca, linea, modelo, combustible) VALUES 
('ABC123', 'TOY', 'COROLLA', '2023', 'BENCINA'),
('DEF456', 'TOY', 'RAV4', '2023', 'DIESEL'),
('GHI789', 'TOY', 'PRIUS', '2023', 'Híbrido');
```

### Variables de Entorno por Ambiente

#### Desarrollo Local
```javascript
// cypress.config.js
env: {
  baseUrl: 'http://localhost:8080',
  testUser: 'admin_toyota',
  testPassword: 'password123',
  enableSAPIntegration: false
}
```

#### Testing
```javascript
env: {
  baseUrl: 'http://test-server:8080',
  testUser: 'test_admin',
  testPassword: 'test123',
  enableSAPIntegration: true
}
```

#### Staging
```javascript
env: {
  baseUrl: 'http://staging-server:8080',
  testUser: 'staging_admin',
  testPassword: 'staging123',
  enableSAPIntegration: true
}
```

---

## 📊 Datos de Prueba

### Usuarios Disponibles
| Usuario | Contraseña | Rol | Descripción |
|---------|------------|-----|-------------|
| admin_toyota | password123 | admin | Administrador principal |
| asesor_ventas | asesor123 | asesor | Asesor de ventas |
| supervisor_taller | supervisor123 | supervisor | Supervisor de taller |

### RUTs de Prueba
| RUT | Estado | Descripción |
|-----|--------|-------------|
| 12345678-5 | ✅ Válido | Cliente registrado |
| 87654321-0 | ✅ Válido | Cliente con múltiples vehículos |
| 99888777-6 | ❌ No registrado | Cliente válido pero no en sistema |
| 11111111-1 | ❌ Inválido | Números repetidos |
| 12345678-9 | ❌ Inválido | Dígito verificador incorrecto |

### Stocks de Prueba
| Stock | Marca | Modelo | Combustible | Estado |
|-------|-------|--------|-------------|--------|
| ABC123 | TOY | COROLLA | BENCINA | ✅ Con venta |
| DEF456 | TOY | RAV4 | DIESEL | ✅ Con venta |
| GHI789 | TOY | PRIUS | Híbrido | ✅ Con venta |
| JKL012 | TOY | HILUX | DIESEL | ❌ Sin venta |
| XYZ999 | - | - | - | ❌ No existe |

---

## 📈 Reportes y Resultados

### Generar Reportes
```bash
# Ejecutar pruebas con reporte
npm run test:ci

# Generar reporte HTML
npm run report:generate

# Abrir reporte en navegador
npm run report:open
```

### Ubicación de Resultados
```
cypress/
├── screenshots/          # Screenshots de fallos
├── videos/              # Videos de ejecución
├── reports/             # Reportes HTML/JSON
│   ├── report.html     # Reporte principal
│   └── report.json     # Datos del reporte
└── downloads/           # Archivos descargados
```

### Métricas Incluidas
- ✅ **Pruebas ejecutadas** y resultados
- ⏱️ **Tiempos de ejecución** por prueba
- 📊 **Porcentaje de éxito**
- 🐛 **Errores y fallos** detallados
- 📸 **Screenshots** de fallos
- 🎥 **Videos** de ejecución

---

## 🔄 CI/CD

### GitHub Actions
Crear archivo `.github/workflows/cypress.yml`:

```yaml
name: Cypress Tests

on:
  push:
    branches: [ main, develop ]
  pull_request:
    branches: [ main ]

jobs:
  cypress-run:
    runs-on: windows-latest
    
    steps:
      - name: Checkout
        uses: actions/checkout@v3
      
      - name: Setup Node.js
        uses: actions/setup-node@v3
        with:
          node-version: '18'
          cache: 'npm'
      
      - name: Install dependencies
        run: npm ci
      
      - name: Run Cypress tests
        uses: cypress-io/github-action@v5
        with:
          browser: chrome
          record: true
        env:
          CYPRESS_RECORD_KEY: ${{ secrets.CYPRESS_RECORD_KEY }}
```

### Azure DevOps
Crear archivo `azure-pipelines.yml`:

```yaml
trigger:
- main
- develop

pool:
  vmImage: 'windows-latest'

steps:
- task: NodeTool@0
  inputs:
    versionSpec: '18.x'

- script: npm ci
  displayName: 'Install dependencies'

- script: npm run test:ci
  displayName: 'Run Cypress tests'

- task: PublishTestResults@2
  inputs:
    testResultsFormat: 'JUnit'
    testResultsFiles: 'cypress/reports/*.xml'
```

### Configuración de Secrets
```bash
# Variables de entorno para CI/CD
CYPRESS_baseUrl=http://test-server:8080
CYPRESS_testUser=ci_user
CYPRESS_testPassword=ci_password
CYPRESS_RECORD_KEY=your-cypress-dashboard-key
```

---

## 🔧 Troubleshooting

### Problemas Comunes

#### 1. Error: "Cypress cannot connect to application"
```bash
# Verificar que la aplicación esté ejecutándose
curl http://localhost:8080

# Verificar configuración en cypress.config.js
baseUrl: 'http://localhost:8080'  # ← Verificar URL correcta
```

#### 2. Error: "Element not found"
```javascript
// Aumentar timeout para elementos lentos
cy.get('#elemento', { timeout: 10000 }).should('be.visible')

// Usar esperas explícitas
cy.waitForPageLoad()
cy.waitForElement('#elemento')
```

#### 3. Error: "Network request failed"
```javascript
// Configurar interceptores para APIs externas
cy.intercept('POST', '**/SAP**', { fixture: 'sap-response.json' })

// Manejar timeouts de red
cy.setTimeouts('sap')  // Configurar timeout específico para SAP
```

#### 4. Error: "Database connection failed"
```bash
# Verificar connection string en web.config
# Verificar que SQL Server esté ejecutándose
# Verificar permisos de base de datos
```

#### 5. Pruebas flaky (intermitentes)
```javascript
// Usar esperas más robustas
cy.get('#elemento').should('be.visible').and('not.be.disabled')

// Configurar reintentos
Cypress.env('retries', { runMode: 2, openMode: 0 })

// Usar comandos más estables
cy.waitForPageLoad()
cy.waitForElement('#elemento')
```

### Debug y Logs

#### Habilitar logs detallados
```javascript
// En cypress.config.js
env: {
  DEBUG: 'cypress:*'
}

// En pruebas individuales
cy.debug()  // Pausar ejecución
cy.pause()  // Pausar con interfaz
```

#### Tomar screenshots manuales
```javascript
cy.screenshot('nombre-descriptivo')
cy.screenshotWithName('paso-especifico')
```

#### Logs personalizados
```javascript
cy.logStep('Descripción del paso actual')
cy.task('log', 'Mensaje para logs del servidor')
```

### Configuración de Timeouts

```javascript
// Timeouts por tipo de operación
cy.setTimeouts('validation')  // 5 segundos
cy.setTimeouts('database')    // 8 segundos  
cy.setTimeouts('sap')         // 15 segundos

// Timeout global
Cypress.config('defaultCommandTimeout', 10000)
```

---

## 📞 Soporte

### Contactos
- **Equipo QA**: qa-team@toyota.cl
- **Desarrollo**: dev-team@toyota.cl
- **Infraestructura**: infra-team@toyota.cl

### Recursos Adicionales
- [Documentación Cypress](https://docs.cypress.io/)
- [Guía de Best Practices](https://docs.cypress.io/guides/references/best-practices)
- [Cypress Dashboard](https://dashboard.cypress.io/)

### Reportar Issues
1. Crear issue en repositorio del proyecto
2. Incluir logs de error
3. Adjuntar screenshots/videos si es necesible
4. Especificar entorno y configuración

---

## 🎯 Próximos Pasos

### Mejoras Planificadas
- [ ] **Integración con Allure** para reportes avanzados
- [ ] **Pruebas de accesibilidad** con cypress-axe
- [ ] **Pruebas visuales** con cypress-image-diff
- [ ] **Pruebas de API** complementarias
- [ ] **Paralelización** de pruebas
- [ ] **Integración con Jira** para tracking de defectos

### Expansión de Cobertura
- [ ] **Más casos edge** y escenarios complejos
- [ ] **Pruebas de carga** con múltiples usuarios
- [ ] **Pruebas cross-browser** automatizadas
- [ ] **Pruebas móviles** con viewport responsive

---

**¡Listo para comenzar! 🚀**

Ejecuta `npm run cy:open` para abrir Cypress y comenzar a ejecutar las pruebas.
