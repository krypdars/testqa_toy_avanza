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
    cy.screenshot(`FAILED-${testName}`)
  }
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
  
  // No fallar la prueba por estos errores
  return false
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
  ]
})

// Utilidades globales disponibles en todas las pruebas
Cypress.Commands.add('getTestData', (key) => {
  return Cypress.env('testData')[key]
})

// Configuración de reportes - no modificar configuraciones de solo lectura
// Las configuraciones de video y screenshots se manejan en cypress.config.js

// Configuración específica para ASP.NET
Cypress.on('window:before:load', (win) => {
  // Stub de funciones ASP.NET que pueden causar problemas
  win.__doPostBack = cy.stub()
  win.WebForm_DoPostBackWithOptions = cy.stub()
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
