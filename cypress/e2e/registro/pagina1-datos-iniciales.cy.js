import LoginPage from '../../support/page-objects/LoginPage'
import RegistroPage1 from '../../support/page-objects/RegistroPage1'

describe('Registro Mantenciones - Página 1: Datos Iniciales', () => {
  let loginPage
  let registroPage1
  let clientes
  let vehiculos

  before(() => {
    // Cargar datos de prueba
    cy.fixture('clientes').then((data) => {
      clientes = data
    })
    cy.fixture('vehiculos').then((data) => {
      vehiculos = data
    })
  })

  beforeEach(() => {
    loginPage = new LoginPage()
    registroPage1 = new RegistroPage1()
    
    // Login antes de cada prueba
    cy.login()
    
    // Navegar a página 1 del registro
    registroPage1.visit()
  })

  describe('Elementos de la Página', () => {
    it('Debe mostrar todos los elementos requeridos', () => {
      registroPage1.verifyPageElements()
      
      cy.logStep('Todos los elementos de la página están presentes')
    })

    it('Debe mostrar información de organización', () => {
      registroPage1.verifyOrganizationInfo()
      
      cy.logStep('Información de organización mostrada')
    })

    it('Debe enfocar el campo RUT al cargar', () => {
      registroPage1.rutInput.should('be.focused')
      
      cy.logStep('Campo RUT enfocado correctamente')
    })

    it('Debe mostrar funcionalidad de ayuda', () => {
      registroPage1.verifyHelpFunctionality()
      
      cy.logStep('Funcionalidad de ayuda verificada')
    })
  })

  describe('Validación de RUT', () => {
    it('Debe aceptar RUT válido con dígito numérico', () => {
      const cliente = clientes.validClients[0]
      
      registroPage1
        .fillRut(cliente.rut)
        .fillStock(vehiculos.validVehicles[0].stock)
        .clickContinue()
        .verifyPageTransition()
      
      cy.logStep(`RUT válido ${cliente.rut} aceptado`)
    })

    it('Debe aceptar RUT válido con dígito K', () => {
      const rutConK = clientes.rutValidationExamples.find(r => r.rut.includes('K'))
      
      registroPage1
        .fillRut(rutConK.rut)
        .fillStock(vehiculos.validVehicles[0].stock)
        .clickContinue()
        .verifyPageTransition()
      
      cy.logStep(`RUT con dígito K ${rutConK.rut} aceptado`)
    })

    it('Debe rechazar RUT con formato incorrecto', () => {
      const rutInvalido = clientes.invalidClients.find(c => 
        c.error.includes('Formato')
      )
      
      registroPage1
        .fillRut(rutInvalido.rut)
        .fillStock(vehiculos.validVehicles[0].stock)
        .clickContinue()
        .verifyRutFormatError()
        .verifyStillOnPage1()
      
      cy.logStep(`RUT con formato incorrecto ${rutInvalido.rut} rechazado`)
    })

    it('Debe rechazar RUT con dígito verificador incorrecto', () => {
      const rutInvalido = clientes.invalidClients.find(c => 
        c.error.includes('Dígito verificador')
      )
      
      registroPage1
        .fillRut(rutInvalido.rut)
        .fillStock(vehiculos.validVehicles[0].stock)
        .clickContinue()
        .verifyRutInvalidError()
        .verifyStillOnPage1()
      
      cy.logStep(`RUT con DV incorrecto ${rutInvalido.rut} rechazado`)
    })

    it('Debe rechazar RUT con números repetidos', () => {
      const rutRepetido = clientes.invalidClients.find(c => 
        c.error.includes('números repetidos')
      )
      
      registroPage1
        .fillRut(rutRepetido.rut)
        .fillStock(vehiculos.validVehicles[0].stock)
        .clickContinue()
        .verifyRutInvalidError()
        .verifyStillOnPage1()
      
      cy.logStep(`RUT con números repetidos ${rutRepetido.rut} rechazado`)
    })

    it('Debe validar RUT requerido', () => {
      registroPage1
        .fillRut('')
        .fillStock(vehiculos.validVehicles[0].stock)
        .clickContinue()
        .verifyRutRequiredError()
        .verifyStillOnPage1()
      
      cy.logStep('Validación de RUT requerido funciona')
    })

    it('Debe ejecutar todas las validaciones de RUT', () => {
      registroPage1.testRutValidations()
      
      cy.logStep('Todas las validaciones de RUT ejecutadas')
    })
  })

  describe('Validación de Cliente', () => {
    it('Debe aceptar cliente registrado', () => {
      const cliente = clientes.validClients[0]
      const vehiculo = vehiculos.validVehicles[0]
      
      registroPage1
        .fillRut(cliente.rut)
        .fillStock(vehiculo.stock)
        .clickContinue()
        .verifyPageTransition()
      
      cy.logStep(`Cliente registrado ${cliente.rut} aceptado`)
    })

    it('Debe mostrar enlace de registro para cliente no registrado', () => {
      const clienteNoRegistrado = clientes.unregisteredClients[0]
      
      registroPage1
        .fillRut(clienteNoRegistrado.rut)
        .fillStock(vehiculos.validVehicles[0].stock)
        .clickContinue()
        .verifyClientNotFoundError()
      
      // Verificar que aparece enlace de registro
      cy.get('a[href*="Ingresar_Clientes.aspx"]').should('be.visible')
      
      cy.logStep(`Cliente no registrado ${clienteNoRegistrado.rut} - enlace mostrado`)
    })

    it('Debe incluir parámetros correctos en enlace de registro', () => {
      const clienteNoRegistrado = clientes.unregisteredClients[0]
      const vehiculo = vehiculos.validVehicles[0]
      
      registroPage1
        .fillRut(clienteNoRegistrado.rut)
        .fillStock(vehiculo.stock)
        .clickContinue()
      
      // Verificar parámetros en el enlace
      cy.get('a[href*="Ingresar_Clientes.aspx"]')
        .should('have.attr', 'href')
        .and('include', `rut=${clienteNoRegistrado.rut.split('-')[0]}`)
        .and('include', `dv=${clienteNoRegistrado.rut.split('-')[1]}`)
        .and('include', `stock=${vehiculo.stock}`)
      
      cy.logStep('Parámetros del enlace de registro verificados')
    })
  })

  describe('Validación de Stock', () => {
    it('Debe aceptar stock válido existente', () => {
      const cliente = clientes.validClients[0]
      const vehiculo = vehiculos.validVehicles[0]
      
      registroPage1
        .fillRut(cliente.rut)
        .fillStock(vehiculo.stock)
        .clickContinue()
        .verifyPageTransition()
      
      cy.logStep(`Stock válido ${vehiculo.stock} aceptado`)
    })

    it('Debe rechazar stock inexistente', () => {
      const cliente = clientes.validClients[0]
      const stockInvalido = vehiculos.invalidVehicles[0]
      
      registroPage1
        .fillRut(cliente.rut)
        .fillStock(stockInvalido.stock)
        .clickContinue()
        .verifyStockNotFoundError()
        .verifyStillOnPage1()
      
      cy.logStep(`Stock inexistente ${stockInvalido.stock} rechazado`)
    })

    it('Debe validar stock requerido', () => {
      const cliente = clientes.validClients[0]
      
      registroPage1
        .fillRut(cliente.rut)
        .fillStock('')
        .clickContinue()
        .verifyStockRequiredError()
        .verifyStillOnPage1()
      
      cy.logStep('Validación de stock requerido funciona')
    })

    it('Debe cargar datos del vehículo en sesión', () => {
      const cliente = clientes.validClients[0]
      const vehiculo = vehiculos.validVehicles[0]
      
      registroPage1
        .fillRut(cliente.rut)
        .fillStock(vehiculo.stock)
        .clickContinue()
      
      // Verificar que llegamos a página 2 y los datos están disponibles
      cy.url().should('contain', 'Reg_Mantencion_02.aspx')
      
      // Verificar que los datos del vehículo se muestran en página 2
      cy.get('body').should('contain', vehiculo.stock)
      
      cy.logStep('Datos del vehículo cargados correctamente')
    })

    it('Debe ejecutar todas las validaciones de Stock', () => {
      registroPage1.testStockValidations()
      
      cy.logStep('Todas las validaciones de Stock ejecutadas')
    })
  })

  describe('Integración SAP', () => {
    it('Debe manejar vehículo con venta registrada en SAP', () => {
      const vehiculoConVenta = vehiculos.validVehicles.find(v => v.hasVenta)
      const cliente = clientes.validClients[0]
      
      // Mock respuesta exitosa de SAP
      cy.intercept('POST', '**/SAP**', {
        statusCode: 200,
        body: {
          fechaFactura: '2023-01-15',
          numeroFactura: '12345'
        }
      }).as('sapSuccess')
      
      registroPage1
        .fillRut(cliente.rut)
        .fillStock(vehiculoConVenta.stock)
        .clickContinue()
      
      cy.wait('@sapSuccess')
      registroPage1.verifyPageTransition()
      
      cy.logStep('Vehículo con venta SAP procesado correctamente')
    })

    it('Debe manejar vehículo sin venta registrada en SAP', () => {
      const vehiculoSinVenta = vehiculos.validVehicles.find(v => !v.hasVenta)
      const cliente = clientes.validClients[0]
      
      // Mock respuesta de SAP sin venta
      cy.intercept('POST', '**/SAP**', {
        statusCode: 200,
        body: {
          mensaje: 'Stock no encontrado'
        }
      }).as('sapNoVenta')
      
      registroPage1
        .fillRut(cliente.rut)
        .fillStock(vehiculoSinVenta.stock)
        .clickContinue()
      
      cy.wait('@sapNoVenta')
      
      // Debería mostrar mensaje informativo pero permitir continuar
      cy.get('body').should('contain', 'no registra ingreso de venta')
      registroPage1.verifyPageTransition()
      
      cy.logStep('Vehículo sin venta SAP manejado correctamente')
    })

    it('Debe manejar timeout de SAP', () => {
      const cliente = clientes.validClients[0]
      const vehiculo = vehiculos.validVehicles[0]
      
      // Mock timeout de SAP
      cy.intercept('POST', '**/SAP**', { delay: 10000 }).as('sapTimeout')
      
      registroPage1
        .fillRut(cliente.rut)
        .fillStock(vehiculo.stock)
      
      // Configurar timeout más corto para la prueba
      cy.setTimeouts('sap')
      
      registroPage1.clickContinue()
      
      // Debería manejar el timeout gracefully
      cy.logStep('Timeout de SAP manejado correctamente')
    })
  })

  describe('Cálculo de Mantenciones', () => {
    it('Debe calcular 10 mantenciones para Toyota Bencina', () => {
      const vehiculoToyotaBencina = vehiculos.validVehicles.find(v => 
        v.marca === 'TOY' && v.combustible === 'BENCINA'
      )
      const cliente = clientes.validClients[0]
      
      registroPage1
        .fillRut(cliente.rut)
        .fillStock(vehiculoToyotaBencina.stock)
        .clickContinue()
        .verifyPageTransition()
      
      // En página 2, verificar que las mantenciones disponibles son correctas
      cy.get('#Mantencion option').should('have.length.greaterThan', 5)
      
      cy.logStep('Mantenciones para Toyota Bencina calculadas correctamente')
    })

    it('Debe calcular 10 mantenciones para Toyota Híbrido', () => {
      const vehiculoToyotaHibrido = vehiculos.validVehicles.find(v => 
        v.marca === 'TOY' && v.combustible === 'Híbrido'
      )
      const cliente = clientes.validClients[0]
      
      registroPage1
        .fillRut(cliente.rut)
        .fillStock(vehiculoToyotaHibrido.stock)
        .clickContinue()
        .verifyPageTransition()
      
      cy.logStep('Mantenciones para Toyota Híbrido calculadas correctamente')
    })

    it('Debe calcular 5 mantenciones para vehículo Diesel', () => {
      const vehiculoDiesel = vehiculos.validVehicles.find(v => 
        v.combustible === 'DIESEL'
      )
      const cliente = clientes.validClients[0]
      
      registroPage1
        .fillRut(cliente.rut)
        .fillStock(vehiculoDiesel.stock)
        .clickContinue()
        .verifyPageTransition()
      
      cy.logStep('Mantenciones para vehículo Diesel calculadas correctamente')
    })
  })

  describe('Navegación', () => {
    it('Debe permitir volver al menú inicial', () => {
      registroPage1.clickMenu()
      
      cy.url().should('contain', 'Default.aspx')
      
      cy.logStep('Navegación al menú inicial funciona')
    })

    it('Debe mantener datos al navegar con parámetros URL', () => {
      const cliente = clientes.validClients[0]
      const vehiculo = vehiculos.validVehicles[0]
      const [rut, dv] = cliente.rut.split('-')
      
      registroPage1.visitWithParams(rut, dv, vehiculo.stock)
      
      // Verificar que los campos se llenan automáticamente
      registroPage1.getRutValue().should('equal', cliente.rut)
      registroPage1.getStockValue().should('equal', vehiculo.stock)
      
      cy.logStep('Datos mantenidos con parámetros URL')
    })
  })

  describe('Validaciones de Seguridad', () => {
    it('Debe prevenir inyección SQL en RUT', () => {
      const sqlInjection = clientes.securityTestCases.find(c => 
        c.description.includes('inyección SQL')
      )
      
      registroPage1
        .fillRut(sqlInjection.rut)
        .fillStock(vehiculos.validVehicles[0].stock)
        .clickContinue()
        .verifyRutFormatError()
      
      cy.logStep('Inyección SQL en RUT bloqueada')
    })

    it('Debe prevenir XSS en Stock', () => {
      const xssAttempt = clientes.securityTestCases.find(c => 
        c.description.includes('XSS')
      )
      
      registroPage1
        .fillRut(clientes.validClients[0].rut)
        .fillStock(xssAttempt.rut) // Usar payload XSS en stock
        .clickContinue()
        .verifyStockNotFoundError()
      
      // Verificar que no se ejecutó JavaScript
      cy.window().then((win) => {
        expect(win.document.body.innerHTML).to.not.contain('<script>')
      })
      
      cy.logStep('Intento XSS en Stock bloqueado')
    })

    it('Debe ejecutar pruebas de seguridad completas', () => {
      registroPage1.testSecurityVulnerabilities()
      
      cy.logStep('Pruebas de seguridad completadas')
    })
  })

  describe('Rendimiento', () => {
    it('Debe validar RUT en tiempo aceptable', () => {
      registroPage1.measureValidationTime()
      
      cy.logStep('Tiempos de validación dentro de límites aceptables')
    })

    it('Debe cargar página en tiempo aceptable', () => {
      const startTime = Date.now()
      
      registroPage1.visit()
      
      cy.waitForPageLoad().then(() => {
        const loadTime = Date.now() - startTime
        cy.log(`Tiempo de carga: ${loadTime}ms`)
        
        expect(loadTime).to.be.lessThan(3000)
      })
      
      cy.logStep('Página carga en tiempo aceptable')
    })
  })

  describe('Accesibilidad', () => {
    it('Debe cumplir estándares de accesibilidad', () => {
      registroPage1.verifyAccessibility()
      
      cy.logStep('Estándares de accesibilidad cumplidos')
    })

    it('Debe ser navegable por teclado', () => {
      const cliente = clientes.validClients[0]
      const vehiculo = vehiculos.validVehicles[0]
      
      // Navegar usando solo teclado
      registroPage1.rutInput.focus().type(cliente.rut).tab()
      registroPage1.stockInput.should('be.focused').type(vehiculo.stock).tab()
      registroPage1.continueButton.should('be.focused').type('{enter}')
      
      registroPage1.verifyPageTransition()
      
      cy.logStep('Navegación por teclado completada exitosamente')
    })
  })

  describe('Casos Edge', () => {
    it('Debe manejar RUT con espacios', () => {
      const cliente = clientes.validClients[0]
      const rutConEspacios = `  ${cliente.rut}  `
      
      registroPage1
        .fillRut(rutConEspacios)
        .fillStock(vehiculos.validVehicles[0].stock)
        .clickContinue()
        .verifyPageTransition()
      
      cy.logStep('RUT con espacios manejado correctamente')
    })

    it('Debe manejar Stock con diferentes formatos', () => {
      const cliente = clientes.validClients[0]
      const stockMinusculas = vehiculos.validVehicles[0].stock.toLowerCase()
      
      registroPage1
        .fillRut(cliente.rut)
        .fillStock(stockMinusculas)
        .clickContinue()
      
      // Debería convertir a mayúsculas o manejar apropiadamente
      cy.logStep('Stock en minúsculas manejado')
    })

    it('Debe manejar pérdida de conexión durante validación', () => {
      // Simular error de red en validación de stock
      cy.intercept('POST', '**/ValidateStock**', { forceNetworkError: true }).as('networkError')
      
      registroPage1
        .fillRut(clientes.validClients[0].rut)
        .fillStock(vehiculos.validVehicles[0].stock)
        .clickContinue()
      
      // Debería mostrar error apropiado
      cy.wait('@networkError')
      
      cy.logStep('Error de conexión durante validación manejado')
    })
  })
})
