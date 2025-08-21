# Guía Completa: Configuración de Cypress para Registro de Mantenciones

## 📋 Índice
1. [Prerrequisitos](#prerrequisitos)
2. [Configuración del Entorno](#configuración-del-entorno)
3. [Instalación de Cypress](#instalación-de-cypress)
4. [Estructura del Proyecto](#estructura-del-proyecto)
5. [Configuración de Cypress](#configuración-de-cypress)
6. [Creación de Pruebas](#creación-de-pruebas)
7. [Ejecución de Pruebas](#ejecución-de-pruebas)
8. [Integración CI/CD](#integración-cicd)
9. [Mejores Prácticas](#mejores-prácticas)

---

## 1. Prerrequisitos

### Software Requerido
- **Node.js** (versión 14 o superior)
- **npm** o **yarn**
- **Git**
- **Servidor web** (IIS, Apache, o servidor de desarrollo)
- **Base de datos** SQL Server con datos de prueba

### Verificar Instalaciones
```bash
# Verificar Node.js
node --version

# Verificar npm
npm --version

# Verificar Git
git --version
```

---

## 2. Configuración del Entorno

### Paso 1: Clonar/Preparar el Repositorio
```bash
# Si el repo está en Git
git clone <url-del-repositorio>
cd Registro_Mantenciones

# O si ya tienes la carpeta local
cd ./Registro_Mantenciones
```

### Paso 2: Configurar el Servidor Web
```bash
# Para IIS (Windows)
# 1. Abrir IIS Manager
# 2. Crear nuevo sitio web
# 3. Apuntar a la carpeta del proyecto
# 4. Configurar puerto (ej: 8080)

# Para desarrollo local con Visual Studio
# 1. Abrir proyecto en Visual Studio
# 2. Configurar IIS Express
# 3. Anotar la URL (ej: http://localhost:44301)
```

### Paso 3: Configurar Base de Datos de Pruebas
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

---

## 3. Instalación de Cypress

### Paso 1: Inicializar Proyecto Node.js
```bash
# Crear package.json si no existe
npm init -y
```

### Paso 2: Instalar Cypress
```bash
# Instalar Cypress como dependencia de desarrollo
npm install --save-dev cypress

# O con yarn
yarn add --dev cypress
```

### Paso 3: Instalar Dependencias Adicionales
```bash
# Instalar plugins útiles
npm install --save-dev @cypress/code-coverage
npm install --save-dev cypress-mochawesome-reporter
npm install --save-dev cypress-real-events
npm install --save-dev cypress-file-upload
npm install --save-dev cypress-xpath

# Para manejo de datos
npm install --save-dev faker
```

### Paso 4: Verificar Instalación
```bash
# Abrir Cypress por primera vez
npx cypress open
```

---

## 4. Estructura del Proyecto

### Estructura Recomendada
```
Registro_Mantenciones/
├── cypress/
│   ├── e2e/
│   │   ├── auth/
│   │   │   ├── login.cy.js
│   │   │   └── session.cy.js
│   │   ├── registro/
│   │   │   ├── pagina1-datos-iniciales.cy.js
│   │   │   ├── pagina2-datos-mantencion.cy.js
│   │   │   ├── pagina3-confirmacion.cy.js
│   │   │   └── pagina4-finalizacion.cy.js
│   │   ├── flujos/
│   │   │   ├── flujo-completo-exitoso.cy.js
│   │   │   ├── flujo-con-errores.cy.js
│   │   │   └── flujos-edge-cases.cy.js
│   │   └── validaciones/
│   │       ├── validacion-rut.cy.js
│   │       ├── validacion-stock.cy.js
│   │       └── validacion-kilometraje.cy.js
│   ├── fixtures/
│   │   ├── usuarios.json
│   │   ├── vehiculos.json
│   │   ├── clientes.json
│   │   └── test-data.json
│   ├── support/
│   │   ├── commands.js
│   │   ├── e2e.js
│   │   ├── page-objects/
│   │   │   ├── LoginPage.js
│   │   │   ├── RegistroPage1.js
│   │   │   ├── RegistroPage2.js
│   │   │   ├── RegistroPage3.js
│   │   │   └── RegistroPage4.js
│   │   └── utils/
│   │       ├── database-helpers.js
│   │       ├── data-generators.js
│   │       └── test-helpers.js
│   └── downloads/
├── cypress.config.js
├── package.json
└── README.md
```

### Crear Estructura de Carpetas
```bash
# Crear estructura básica
mkdir -p cypress/e2e/{auth,registro,flujos,validaciones}
mkdir -p cypress/fixtures
mkdir -p cypress/support/{page-objects,utils}
mkdir -p cypress/downloads
```

---

## 5. Configuración de Cypress

### Archivo: `cypress.config.js`
```javascript
const { defineConfig } = require('cypress')

module.exports = defineConfig({
  e2e: {
    // URL base de la aplicación
    baseUrl: 'http://localhost:8080', // Ajustar según tu configuración
    
    // Configuración de viewport
    viewportWidth: 1280,
    viewportHeight: 720,
    
    // Timeouts
    defaultCommandTimeout: 10000,
    requestTimeout: 10000,
    responseTimeout: 10000,
    pageLoadTimeout: 30000,
    
    // Configuración de video y screenshots
    video: true,
    screenshotOnRunFailure: true,
    
    // Configuración de archivos
    fixturesFolder: 'cypress/fixtures',
    screenshotsFolder: 'cypress/screenshots',
    videosFolder: 'cypress/videos',
    downloadsFolder: 'cypress/downloads',
    
    // Patrones de archivos de prueba
    specPattern: 'cypress/e2e/**/*.cy.{js,jsx,ts,tsx}',
    
    // Configuración del navegador
    chromeWebSecurity: false,
    
    // Variables de entorno
    env: {
      // URLs específicas
      loginUrl: '/default.aspx',
      registroUrl: '/Registro_Mantenciones/Reg_Mantencion_01.aspx',
      
      // Credenciales de prueba
      testUser: 'admin_toyota',
      testPassword: 'password123',
      
      // Configuración de base de datos
      dbHost: 'localhost',
      dbName: 'RegistroMantenciones_Test',
      
      // Configuración de API
      apiUrl: 'http://localhost:8080/api',
      
      // Flags de funcionalidades
      enableFileUpload: true,
      enableSAPIntegration: false // Para pruebas sin SAP
    },
    
    setupNodeEvents(on, config) {
      // Configurar plugins
      require('cypress-mochawesome-reporter/plugin')(on);
      
      // Configurar tareas personalizadas
      on('task', {
        // Tarea para limpiar base de datos
        clearDatabase() {
          // Implementar limpieza de BD
          return null;
        },
        
        // Tarea para insertar datos de prueba
        seedDatabase(data) {
          // Implementar inserción de datos
          return null;
        },
        
        // Tarea para generar datos de prueba
        generateTestData(type) {
          const faker = require('faker');
          
          switch(type) {
            case 'rut':
              return generateValidRut();
            case 'stock':
              return faker.random.alphaNumeric(6).toUpperCase();
            default:
              return null;
          }
        }
      });
      
      return config;
    },
  },
  
  component: {
    devServer: {
      framework: 'create-react-app',
      bundler: 'webpack',
    },
  },
})

// Función auxiliar para generar RUT válido
function generateValidRut() {
  const rut = Math.floor(Math.random() * 99999999) + 10000000;
  const dv = calculateDV(rut);
  return `${rut}-${dv}`;
}

function calculateDV(rut) {
  let contador = 2;
  let acumulador = 0;
  let rutTemp = rut;
  
  while (rutTemp !== 0) {
    const multiplo = (rutTemp % 10) * contador;
    acumulador += multiplo;
    rutTemp = Math.floor(rutTemp / 10);
    contador++;
    if (contador === 8) contador = 2;
  }
  
  const digito = 11 - (acumulador % 11);
  if (digito === 10) return 'K';
  if (digito === 11) return '0';
  return digito.toString();
}
```

### Archivo: `package.json` (scripts)
```json
{
  "name": "registro-mantenciones-cypress",
  "version": "1.0.0",
  "description": "Pruebas automatizadas con Cypress para Registro de Mantenciones",
  "scripts": {
    "cy:open": "cypress open",
    "cy:run": "cypress run",
    "cy:run:chrome": "cypress run --browser chrome",
    "cy:run:firefox": "cypress run --browser firefox",
    "cy:run:edge": "cypress run --browser edge",
    "cy:run:headed": "cypress run --headed",
    "cy:run:record": "cypress run --record --key YOUR_RECORD_KEY",
    "test:auth": "cypress run --spec 'cypress/e2e/auth/**'",
    "test:registro": "cypress run --spec 'cypress/e2e/registro/**'",
    "test:flujos": "cypress run --spec 'cypress/e2e/flujos/**'",
    "test:validaciones": "cypress run --spec 'cypress/e2e/validaciones/**'",
    "test:smoke": "cypress run --spec 'cypress/e2e/flujos/flujo-completo-exitoso.cy.js'",
    "test:regression": "cypress run --spec 'cypress/e2e/**'",
    "report:generate": "npx mochawesome-merge cypress/reports/*.json > cypress/reports/report.json && npx marge cypress/reports/report.json --reportDir cypress/reports --inline",
    "test:ci": "cypress run --browser chrome --headless --reporter mochawesome"
  },
  "devDependencies": {
    "cypress": "^13.0.0",
    "@cypress/code-coverage": "^3.10.0",
    "cypress-mochawesome-reporter": "^3.5.1",
    "cypress-real-events": "^1.9.1",
    "cypress-file-upload": "^5.0.8",
    "cypress-xpath": "^2.0.1",
    "faker": "^5.5.3"
  }
}
```

---

## 6. Creación de Pruebas

### Configurar Comandos Personalizados
**Archivo: `cypress/support/commands.js`**
```javascript
// Comando para login
Cypress.Commands.add('login', (username, password) => {
  username = username || Cypress.env('testUser');
  password = password || Cypress.env('testPassword');
  
  cy.visit(Cypress.env('loginUrl'));
  cy.get('#txtUsuario').type(username);
  cy.get('#txtPassword').type(password);
  cy.get('#btnLogin').click();
  
  // Verificar que el login fue exitoso
  cy.url().should('not.contain', 'default.aspx');
});

// Comando para completar página 1
Cypress.Commands.add('completarPagina1', (rut, stock) => {
  cy.visit(Cypress.env('registroUrl'));
  
  if (rut) {
    cy.get('#TxtRut').clear().type(rut);
  }
  
  if (stock) {
    cy.get('#TxtStock').clear().type(stock);
  }
  
  cy.get('#Button1').click();
});

// Comando para generar RUT válido
Cypress.Commands.add('generateValidRut', () => {
  return cy.task('generateTestData', 'rut');
});

// Comando para generar Stock válido
Cypress.Commands.add('generateValidStock', () => {
  return cy.task('generateTestData', 'stock');
});

// Comando para esperar carga de página
Cypress.Commands.add('waitForPageLoad', () => {
  cy.window().should('have.property', 'document');
  cy.document().should('have.property', 'readyState', 'complete');
});

// Comando para tomar screenshot con nombre personalizado
Cypress.Commands.add('screenshotWithName', (name) => {
  const timestamp = new Date().toISOString().replace(/[:.]/g, '-');
  cy.screenshot(`${name}-${timestamp}`);
});

// Comando para validar elementos de página
Cypress.Commands.add('validatePageElements', (elements) => {
  elements.forEach(element => {
    cy.get(element.selector).should(element.assertion || 'exist');
  });
});

// Comando para limpiar datos de prueba
Cypress.Commands.add('cleanTestData', () => {
  cy.task('clearDatabase');
});

// Comando para insertar datos de prueba
Cypress.Commands.add('seedTestData', (data) => {
  cy.task('seedDatabase', data);
});
```

### Page Objects
**Archivo: `cypress/support/page-objects/LoginPage.js`**
```javascript
class LoginPage {
  // Selectores
  get usernameInput() { return cy.get('#txtUsuario'); }
  get passwordInput() { return cy.get('#txtPassword'); }
  get loginButton() { return cy.get('#btnLogin'); }
  get errorMessage() { return cy.get('.error-message'); }
  
  // Métodos
  visit() {
    cy.visit(Cypress.env('loginUrl'));
    return this;
  }
  
  login(username, password) {
    this.usernameInput.clear().type(username);
    this.passwordInput.clear().type(password);
    this.loginButton.click();
    return this;
  }
  
  verifyLoginSuccess() {
    cy.url().should('not.contain', 'default.aspx');
    return this;
  }
  
  verifyLoginError() {
    this.errorMessage.should('be.visible');
    return this;
  }
}

export default LoginPage;
```

**Archivo: `cypress/support/page-objects/RegistroPage1.js`**
```javascript
class RegistroPage1 {
  // Selectores
  get rutInput() { return cy.get('#TxtRut'); }
  get stockInput() { return cy.get('#TxtStock'); }
  get continueButton() { return cy.get('#Button1'); }
  get menuButton() { return cy.get('#Button3'); }
  get validationSummary() { return cy.get('#valSumario'); }
  get helpButton() { return cy.get('input[value="Ayuda"]'); }
  
  // Métodos
  visit() {
    cy.visit(Cypress.env('registroUrl'));
    return this;
  }
  
  fillRut(rut) {
    this.rutInput.clear().type(rut);
    return this;
  }
  
  fillStock(stock) {
    this.stockInput.clear().type(stock);
    return this;
  }
  
  clickContinue() {
    this.continueButton.click();
    return this;
  }
  
  verifyValidationError(message) {
    this.validationSummary.should('contain.text', message);
    return this;
  }
  
  verifyPageTransition() {
    cy.url().should('contain', 'Reg_Mantencion_02.aspx');
    return this;
  }
  
  fillFormAndContinue(rut, stock) {
    this.fillRut(rut);
    this.fillStock(stock);
    this.clickContinue();
    return this;
  }
}

export default RegistroPage1;
```

### Datos de Prueba (Fixtures)
**Archivo: `cypress/fixtures/usuarios.json`**
```json
{
  "validUsers": [
    {
      "username": "admin_toyota",
      "password": "password123",
      "role": "admin"
    },
    {
      "username": "asesor_ventas",
      "password": "asesor123",
      "role": "asesor"
    }
  ],
  "invalidUsers": [
    {
      "username": "usuario_inexistente",
      "password": "password_incorrecto",
      "expectedError": "Credenciales inválidas"
    }
  ]
}
```

**Archivo: `cypress/fixtures/vehiculos.json`**
```json
{
  "validVehicles": [
    {
      "stock": "ABC123",
      "rut": "12345678-5",
      "marca": "TOY",
      "modelo": "COROLLA",
      "combustible": "BENCINA",
      "expectedMaintenance": "10"
    },
    {
      "stock": "DEF456",
      "rut": "87654321-0",
      "marca": "TOY",
      "modelo": "RAV4",
      "combustible": "DIESEL",
      "expectedMaintenance": "5"
    }
  ],
  "invalidVehicles": [
    {
      "stock": "XYZ999",
      "rut": "11111111-1",
      "expectedError": "Stock no existe"
    }
  ]
}
```

---

## 7. Ejecución de Pruebas

### Comandos Básicos
```bash
# Abrir Cypress en modo interactivo
npm run cy:open

# Ejecutar todas las pruebas en modo headless
npm run cy:run

# Ejecutar pruebas específicas
npm run test:auth
npm run test:registro
npm run test:flujos

# Ejecutar pruebas de smoke
npm run test:smoke

# Ejecutar con navegador específico
npm run cy:run:chrome
npm run cy:run:firefox

# Ejecutar con cabeza visible
npm run cy:run:headed
```

### Ejecución por Categorías
```bash
# Pruebas de autenticación
cypress run --spec "cypress/e2e/auth/**"

# Pruebas de validaciones
cypress run --spec "cypress/e2e/validaciones/**"

# Pruebas de flujo completo
cypress run --spec "cypress/e2e/flujos/**"

# Prueba específica
cypress run --spec "cypress/e2e/registro/pagina1-datos-iniciales.cy.js"
```

### Configuración de Reportes
```bash
# Generar reporte HTML
npm run report:generate

# Ejecutar con reporte para CI
npm run test:ci
```

---

## 8. Integración CI/CD

### GitHub Actions
**Archivo: `.github/workflows/cypress.yml`**
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
      
      - name: Setup IIS
        run: |
          # Configurar IIS para pruebas
          Enable-WindowsOptionalFeature -Online -FeatureName IIS-WebServerRole
          Enable-WindowsOptionalFeature -Online -FeatureName IIS-WebServer
          Enable-WindowsOptionalFeature -Online -FeatureName IIS-CommonHttpFeatures
          Enable-WindowsOptionalFeature -Online -FeatureName IIS-HttpErrors
          Enable-WindowsOptionalFeature -Online -FeatureName IIS-HttpRedirect
          Enable-WindowsOptionalFeature -Online -FeatureName IIS-ApplicationDevelopment
          Enable-WindowsOptionalFeature -Online -FeatureName IIS-NetFx45
          Enable-WindowsOptionalFeature -Online -FeatureName IIS-ASPNET45
      
      - name: Deploy application
        run: |
          # Copiar archivos a IIS
          xcopy /E /I /Y . "C:\inetpub\wwwroot\RegistroMantenciones"
      
      - name: Run Cypress tests
        uses: cypress-io/github-action@v5
        with:
          start: echo "Application already running"
          wait-on: 'http://localhost'
          browser: chrome
          record: true
        env:
          CYPRESS_RECORD_KEY: ${{ secrets.CYPRESS_RECORD_KEY }}
          GITHUB_TOKEN: ${{ secrets.GITHUB_TOKEN }}
      
      - name: Upload screenshots
        uses: actions/upload-artifact@v3
        if: failure()
        with:
          name: cypress-screenshots
          path: cypress/screenshots
      
      - name: Upload videos
        uses: actions/upload-artifact@v3
        if: always()
        with:
          name: cypress-videos
          path: cypress/videos
```

### Azure DevOps Pipeline
**Archivo: `azure-pipelines.yml`**
```yaml
trigger:
- main
- develop

pool:
  vmImage: 'windows-latest'

variables:
  buildConfiguration: 'Release'

steps:
- task: NodeTool@0
  inputs:
    versionSpec: '18.x'
  displayName: 'Install Node.js'

- script: |
    npm ci
  displayName: 'Install dependencies'

- task: IISWebAppManagementOnMachineGroup@0
  inputs:
    EnableIIS: true
    IISDeploymentType: 'IISWebsite'
    ActionIISWebsite: 'CreateOrUpdateWebsite'
    WebsiteName: 'RegistroMantenciones'
    WebsitePhysicalPath: '$(System.DefaultWorkingDirectory)'
    WebsitePort: '8080'
  displayName: 'Deploy to IIS'

- script: |
    npm run cy:run -- --reporter mochawesome
  displayName: 'Run Cypress tests'

- task: PublishTestResults@2
  inputs:
    testResultsFormat: 'JUnit'
    testResultsFiles: 'cypress/reports/*.xml'
  displayName: 'Publish test results'

- task: PublishHtmlReport@1
  inputs:
    reportDir: 'cypress/reports'
  displayName: 'Publish HTML report'
```

---

## 9. Mejores Prácticas

### Organización de Pruebas
1. **Usar Page Objects** para mantener código reutilizable
2. **Separar datos de prueba** en fixtures
3. **Crear comandos personalizados** para acciones comunes
4. **Agrupar pruebas** por funcionalidad
5. **Usar tags** para categorizar pruebas

### Manejo de Datos
```javascript
// Usar beforeEach para preparar datos
beforeEach(() => {
  cy.cleanTestData();
  cy.seedTestData({
    users: ['admin_toyota'],
    vehicles: ['ABC123', 'DEF456']
  });
});

// Usar afterEach para limpiar
afterEach(() => {
  cy.cleanTestData();
});
```

### Esperas Inteligentes
```javascript
// Esperar elementos específicos
cy.get('#TxtRut').should('be.visible');

// Esperar respuestas de API
cy.intercept('POST', '/api/validate-stock').as('validateStock');
cy.wait('@validateStock');

// Esperar condiciones personalizadas
cy.waitUntil(() => 
  cy.window().then(win => win.document.readyState === 'complete')
);
```

### Manejo de Errores
```javascript
// Capturar errores de aplicación
Cypress.on('uncaught:exception', (err, runnable) => {
  // Ignorar errores específicos de la aplicación
  if (err.message.includes('Script error')) {
    return false;
  }
});

// Reintentos automáticos
cy.get('#Button1', { timeout: 10000 }).click();
```

---

## 10. Comandos de Inicio Rápido

### Setup Inicial Completo
```bash
# 1. Clonar repositorio
git clone <repo-url>
cd Registro_Mantenciones

# 2. Instalar dependencias
npm install

# 3. Configurar aplicación web
# (Seguir pasos de configuración IIS/servidor)

# 4. Ejecutar primera prueba
npm run cy:open

# 5. Ejecutar suite completa
npm run cy:run
```

### Verificación de Setup
```bash
# Verificar que Cypress funciona
npx cypress verify

# Ejecutar prueba de ejemplo
npx cypress run --spec "cypress/e2e/auth/login.cy.js"

# Verificar configuración
npx cypress info
```

---

## 📞 Soporte y Troubleshooting

### Problemas Comunes

1. **Error de conexión a la aplicación**
   - Verificar que el servidor web esté ejecutándose
   - Confirmar la URL en `cypress.config.js`

2. **Elementos no encontrados**
   - Verificar selectores en las páginas
   - Usar `cy.debug()` para inspeccionar

3. **Timeouts**
   - Aumentar timeouts en configuración
   - Usar esperas explícitas

4. **Problemas de base de datos**
   - Verificar connection string
   - Confirmar permisos de BD

### Logs y Debug
```javascript
// Habilitar logs detallados
Cypress.config('defaultCommandTimeout', 10000);
Cypress.config('requestTimeout', 10000);

// Debug en pruebas
cy.debug();
cy.pause();
```

Esta guía te permitirá configurar completamente Cypress para el proyecto de Registro de Mantenciones. ¿Te gustaría que cree ahora los archivos específicos de pruebas de Cypress?
