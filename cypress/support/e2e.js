// ***********************************************************
// Configuración global para pruebas E2E de Registro de Mantenciones
// ***********************************************************

// Importar comandos personalizados
import './commands'

// Importar plugins adicionales
import 'cypress-real-events/support'
import 'cypress-file-upload'
import 'cypress-mochawesome-reporter/register'

// Configuración global antes de cada prueba
beforeEach(() => {
  // Limpiar cookies y storage
  cy.clearCookies()
  cy.clearLocalStorage()
  
  // Configurar interceptores globales
  cy.interceptValidations()
  
  // Configurar viewport estándar
  cy.viewport(1280, 720)
  
  // Configurar timeouts específicos para la aplicación
  Cypress.config('defaultCommandTimeout', 10000)
  Cypress.config('requestTimeout', 10000)
  Cypress.config('responseTimeout', 10000)
})

// Configuración después de cada prueba
afterEach(() => {
  // Tomar screenshot si la prueba falló
  if (Cypress.currentTest.state === 'failed') {
    const testName = Cypress.currentTest.title.replace(/\s+/g, '-')
    cy.screenshotWithName(`FAILED-${testName}`)
  }
  
  // Limpiar datos de prueba si es necesario
  cy.cleanTestData()
})

// Manejo global de errores no capturados
Cypress.on('uncaught:exception', (err, runnable) => {
  // Ignorar errores específicos de la aplicación ASP.NET
  if (err.message.includes('Script error')) {
    return false
  }
  
  if (err.message.includes('ResizeObserver loop limit exceeded')) {
    return false
  }
  
  if (err.message.includes('Non-Error promise rejection captured')) {
    return false
  }
  
  // Ignorar errores de validación de ASP.NET que son esperados
  if (err.message.includes('WebForm_PostBackOptions')) {
    return false
  }
  
  // Log del error para debugging
  cy.task('log', `Error no capturado: ${err.message}`)
  
  // No fallar la prueba por estos errores
  return false
})

// Configuración para manejo de promesas rechazadas
Cypress.on('fail', (err, runnable) => {
  // Log detallado del error
  cy.task('log', `Prueba falló: ${err.message}`)
  
  // Tomar screenshot adicional
  cy.screenshotWithName('ERROR-DETAIL')
  
  throw err
})

// Configuración de logs personalizados
Cypress.on('log:added', (attrs, log) => {
  if (attrs.name === 'request') {
    cy.task('log', `Request: ${attrs.message}`)
  }
})

// Configuración específica para el entorno de pruebas
before(() => {
  cy.task('log', '=== INICIANDO SUITE DE PRUEBAS REGISTRO MANTENCIONES ===')
  
  // Verificar que la aplicación esté disponible
  cy.request({
    url: Cypress.config('baseUrl'),
    failOnStatusCode: false
  }).then((response) => {
    if (response.status !== 200) {
      throw new Error(`Aplicación no disponible. Status: ${response.status}`)
    }
    cy.task('log', 'Aplicación disponible y lista para pruebas')
  })
})

after(() => {
  cy.task('log', '=== FINALIZANDO SUITE DE PRUEBAS ===')
})

// Configuración de datos de prueba globales
Cypress.env('testData', {
  validRuts: [
    '12345678-5',
    '98765432-1',
    '11222333-4'
  ],
  invalidRuts: [
    '12345678-9',
    '11111111-1',
    '123456789'
  ],
  validStocks: [
    'ABC123',
    'DEF456',
    'GHI789'
  ],
  invalidStocks: [
    'XYZ999',
    'NOEXISTE',
    '123456'
  ],
  testUsers: {
    admin: {
      username: 'admin_toyota',
      password: 'password123'
    },
    asesor: {
      username: 'asesor_ventas',
      password: 'asesor123'
    }
  }
})

// Utilidades globales disponibles en todas las pruebas
Cypress.Commands.add('getTestData', (key) => {
  return Cypress.env('testData')[key]
})

// Configuración de reportes
if (Cypress.config('isInteractive')) {
  // Configuración para modo interactivo
  Cypress.config('video', false)
} else {
  // Configuración para modo headless/CI
  Cypress.config('video', true)
  Cypress.config('screenshotOnRunFailure', true)
}

// Configuración de retry para pruebas flaky
Cypress.env('retries', {
  runMode: 2,
  openMode: 0
})

// Configuración específica para ASP.NET
Cypress.on('window:before:load', (win) => {
  // Stub de funciones ASP.NET que pueden causar problemas
  win.__doPostBack = cy.stub()
  win.WebForm_DoPostBackWithOptions = cy.stub()
  
  // Configurar console para capturar logs de la aplicación
  const originalLog = win.console.log
  win.console.log = (...args) => {
    cy.task('log', `App Log: ${args.join(' ')}`)
    originalLog.apply(win.console, args)
  }
})

// Configuración de custom matchers
chai.use((chai, utils) => {
  chai.Assertion.addMethod('validRut', function () {
    const obj = this._obj
    const isValid = /^\d{7,8}[-][0-9kK]{1}$/.test(obj)
    
    this.assert(
      isValid,
      'expected #{this} to be a valid RUT format',
      'expected #{this} not to be a valid RUT format'
    )
  })
})

// Configuración de timeouts específicos por tipo de operación
Cypress.Commands.add('setTimeouts', (type) => {
  switch (type) {
    case 'validation':
      Cypress.config('defaultCommandTimeout', 5000)
      break
    case 'database':
      Cypress.config('defaultCommandTimeout', 8000)
      break
    case 'sap':
      Cypress.config('defaultCommandTimeout', 15000)
      break
    default:
      Cypress.config('defaultCommandTimeout', 10000)
  }
})
