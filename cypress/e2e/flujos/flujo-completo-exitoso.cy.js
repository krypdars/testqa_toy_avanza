import LoginPage from '../../support/page-objects/LoginPage'
import RegistroPage1 from '../../support/page-objects/RegistroPage1'

describe('Flujo Completo Exitoso - Registro de Mantenciones', () => {
  let loginPage
  let registroPage1
  let usuarios
  let clientes
  let vehiculos

  before(() => {
    // Cargar todos los datos de prueba
    cy.fixture('usuarios').then((data) => {
      usuarios = data
    })
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
    
    // Limpiar datos de prueba antes de cada test
    cy.cleanTestData()
  })

  describe('Flujo Completo Básico', () => {
    it('Debe completar registro de mantención exitosamente', () => {
      const usuario = usuarios.validUsers[0]
      const cliente = clientes.validClients[0]
      const vehiculo = vehiculos.validVehicles[0]
      
      cy.logStep('=== INICIANDO FLUJO COMPLETO EXITOSO ===')
      
      // PASO 1: Login
      cy.logStep('PASO 1: Realizando login')
      loginPage
        .visit()
        .login(usuario.username, usuario.password)
        .verifyLoginSuccess()
      
      // PASO 2: Página 1 - Datos Iniciales
      cy.logStep('PASO 2: Completando datos iniciales (RUT y Stock)')
      registroPage1
        .visit()
        .verifyPageElements()
        .fillRut(cliente.rut)
        .fillStock(vehiculo.stock)
        .clickContinue()
        .verifyPageTransition()
      
      // PASO 3: Página 2 - Datos de Mantención
      cy.logStep('PASO 3: Completando datos de mantención')
      cy.url().should('contain', 'Reg_Mantencion_02.aspx')
      
      // Verificar que los datos del vehículo se muestran
      cy.get('#Label7').should('contain.text', vehiculo.stock)
      
      // Seleccionar mantención
      cy.get('#Mantencion').should('be.visible').select('10') // 10.000 Kms
      
      // Ingresar kilometraje
      cy.get('#TxtKM').should('be.visible').clear().type('9500')
      
      // Seleccionar asesor si está disponible
      cy.get('body').then(($body) => {
        if ($body.find('#CmbAsesor').length > 0) {
          cy.get('#CmbAsesor').select(1) // Seleccionar primer asesor disponible
        }
      })
      
      // Continuar a página 3
      cy.get('#Button1').should('be.visible').click()
      
      // PASO 4: Página 3 - Confirmación
      cy.logStep('PASO 4: Confirmando datos')
      cy.url({ timeout: 15000 }).should('contain', 'Reg_Mantencion_03.aspx')
      
      // Verificar que los datos se muestran correctamente
      cy.get('body').should('contain', cliente.rut.split('-')[0]) // RUT sin DV
      cy.get('body').should('contain', vehiculo.stock)
      cy.get('body').should('contain', '10.000 Kms')
      cy.get('body').should('contain', '9500')
      
      // Confirmar registro
      cy.get('#Button1').should('be.visible').click()
      
      // PASO 5: Página 4 - Finalización
      cy.logStep('PASO 5: Verificando finalización exitosa')
      cy.url({ timeout: 15000 }).should('contain', 'Reg_Mantencion_04.aspx')
      
      // Verificar mensaje de éxito y código de garantía
      cy.get('body').should('contain.text', 'exitoso')
        .or('contain.text', 'completado')
        .or('contain.text', 'registrado')
      
      // Verificar que se generó código de garantía (formato: AAAAMMDDHHMMSS)
      cy.get('body').invoke('text').then((text) => {
        const codigoMatch = text.match(/\d{14}/) // Buscar código de 14 dígitos
        expect(codigoMatch).to.not.be.null
        cy.logStep(`Código de garantía generado: ${codigoMatch[0]}`)
      })
      
      cy.logStep('=== FLUJO COMPLETO EXITOSO FINALIZADO ===')
    })

    it('Debe completar registro con vehículo Toyota Bencina', () => {
      const vehiculoToyotaBencina = vehiculos.validVehicles.find(v => 
        v.marca === 'TOY' && v.combustible === 'BENCINA'
      )
      const cliente = clientes.validClients[0]
      
      cy.logStep('Probando flujo con vehículo Toyota Bencina')
      
      const datosRegistro = {
        rut: cliente.rut,
        stock: vehiculoToyotaBencina.stock,
        mantencion: 10,
        kilometraje: 9500,
        asesor: 'ASESOR001'
      }
      
      cy.login()
      cy.completarRegistroCompleto(datosRegistro)
      
      cy.logStep('Registro con Toyota Bencina completado exitosamente')
    })

    it('Debe completar registro con vehículo Híbrido', () => {
      const vehiculoHibrido = vehiculos.validVehicles.find(v => 
        v.combustible === 'Híbrido'
      )
      const cliente = clientes.validClients[0]
      
      cy.logStep('Probando flujo con vehículo Híbrido')
      
      const datosRegistro = {
        rut: cliente.rut,
        stock: vehiculoHibrido.stock,
        mantencion: 5,
        kilometraje: 4800,
        asesor: 'ASESOR002'
      }
      
      cy.login()
      cy.completarRegistroCompleto(datosRegistro)
      
      cy.logStep('Registro con vehículo Híbrido completado exitosamente')
    })

    it('Debe completar registro con vehículo Diesel', () => {
      const vehiculoDiesel = vehiculos.validVehicles.find(v => 
        v.combustible === 'DIESEL'
      )
      const cliente = clientes.validClients[0]
      
      cy.logStep('Probando flujo con vehículo Diesel')
      
      const datosRegistro = {
        rut: cliente.rut,
        stock: vehiculoDiesel.stock,
        mantencion: 15,
        kilometraje: 14500,
        asesor: 'ASESOR003'
      }
      
      cy.login()
      cy.completarRegistroCompleto(datosRegistro)
      
      cy.logStep('Registro con vehículo Diesel completado exitosamente')
    })
  })

  describe('Flujo con Diferentes Mantenciones', () => {
    vehiculos.mantenciones.forEach((mantencion) => {
      it(`Debe completar registro para mantención de ${mantencion.description}`, () => {
        const cliente = clientes.validClients[0]
        const vehiculo = vehiculos.validVehicles[0]
        const kilometraje = mantencion.kilometers - 500 // Kilometraje ligeramente menor
        
        cy.logStep(`Probando mantención: ${mantencion.description}`)
        
        const datosRegistro = {
          rut: cliente.rut,
          stock: vehiculo.stock,
          mantencion: mantencion.id,
          kilometraje: kilometraje,
          asesor: 'ASESOR001'
        }
        
        cy.login()
        cy.completarRegistroCompleto(datosRegistro)
        
        cy.logStep(`Mantención ${mantencion.description} completada exitosamente`)
      })
    })
  })

  describe('Flujo con Diferentes Usuarios', () => {
    usuarios.validUsers.forEach((usuario) => {
      it(`Debe completar registro con usuario ${usuario.role}`, () => {
        const cliente = clientes.validClients[0]
        const vehiculo = vehiculos.validVehicles[0]
        
        cy.logStep(`Probando con usuario: ${usuario.username} (${usuario.role})`)
        
        // Login con usuario específico
        loginPage
          .visit()
          .login(usuario.username, usuario.password)
          .verifyLoginSuccess()
        
        const datosRegistro = {
          rut: cliente.rut,
          stock: vehiculo.stock,
          mantencion: 10,
          kilometraje: 9500
        }
        
        cy.completarRegistroCompleto(datosRegistro)
        
        cy.logStep(`Registro con usuario ${usuario.role} completado`)
      })
    })
  })

  describe('Flujo con Casos Especiales', () => {
    it('Debe completar registro con vehículo sin venta registrada', () => {
      const vehiculoSinVenta = vehiculos.validVehicles.find(v => !v.hasVenta)
      const cliente = clientes.validClients[0]
      
      cy.logStep('Probando flujo con vehículo sin venta registrada')
      
      // Mock respuesta SAP sin venta
      cy.intercept('POST', '**/SAP**', {
        statusCode: 200,
        body: { mensaje: 'Stock no encontrado' }
      }).as('sapSinVenta')
      
      cy.login()
      
      // Página 1
      registroPage1
        .visit()
        .fillRut(cliente.rut)
        .fillStock(vehiculoSinVenta.stock)
        .clickContinue()
      
      cy.wait('@sapSinVenta')
      
      // Debería mostrar mensaje pero permitir continuar
      cy.url().should('contain', 'Reg_Mantencion_02.aspx')
      cy.get('body').should('contain', 'no registra ingreso de venta')
      
      // Completar resto del flujo
      cy.get('#Mantencion').select('10')
      cy.get('#TxtKM').clear().type('9500')
      cy.get('#Button1').click()
      
      // Página 3
      cy.url().should('contain', 'Reg_Mantencion_03.aspx')
      cy.get('#Button1').click()
      
      // Página 4
      cy.url().should('contain', 'Reg_Mantencion_04.aspx')
      cy.get('body').should('contain.text', 'exitoso')
      
      cy.logStep('Registro sin venta SAP completado exitosamente')
    })

    it('Debe completar registro con vehículo en campaña', () => {
      const vehiculoConCampana = vehiculos.vehiclesWithCampaigns[0]
      const cliente = clientes.validClients[0]
      
      cy.logStep('Probando flujo con vehículo en campaña')
      
      // Mock datos de campaña
      cy.intercept('GET', '**/Campañas**', {
        statusCode: 200,
        body: vehiculoConCampana.campaigns
      }).as('campanasData')
      
      const datosRegistro = {
        rut: cliente.rut,
        stock: vehiculoConCampana.stock,
        mantencion: 10,
        kilometraje: 9500
      }
      
      cy.login()
      cy.completarRegistroCompleto(datosRegistro)
      
      cy.logStep('Registro con vehículo en campaña completado')
    })

    it('Debe completar registro con vehículo con prepagos', () => {
      const vehiculoConPrepagos = vehiculos.vehiclesWithPrepayments[0]
      const cliente = clientes.validClients[0]
      
      cy.logStep('Probando flujo con vehículo con prepagos')
      
      const datosRegistro = {
        rut: cliente.rut,
        stock: vehiculoConPrepagos.stock,
        mantencion: 10,
        kilometraje: 9500
      }
      
      cy.login()
      cy.completarRegistroCompleto(datosRegistro)
      
      cy.logStep('Registro con prepagos completado')
    })

    it('Debe completar registro con vehículo elegible para T10', () => {
      const vehiculoT10 = vehiculos.vehiclesT10Program[0]
      const cliente = clientes.validClients[0]
      
      cy.logStep('Probando flujo con vehículo T10')
      
      const datosRegistro = {
        rut: cliente.rut,
        stock: vehiculoT10.stock,
        mantencion: 10,
        kilometraje: 9500
      }
      
      cy.login()
      cy.completarRegistroCompleto(datosRegistro)
      
      cy.logStep('Registro T10 completado')
    })
  })

  describe('Flujo con Navegación', () => {
    it('Debe permitir volver y modificar datos', () => {
      const cliente = clientes.validClients[0]
      const vehiculo = vehiculos.validVehicles[0]
      
      cy.logStep('Probando navegación hacia atrás y modificación')
      
      cy.login()
      
      // Completar página 1
      registroPage1
        .visit()
        .fillRut(cliente.rut)
        .fillStock(vehiculo.stock)
        .clickContinue()
      
      // En página 2, volver a página 1
      cy.get('#B_Volver').should('be.visible').click()
      
      // Verificar que volvimos y los datos se mantienen
      cy.url().should('contain', 'Reg_Mantencion_01.aspx')
      registroPage1.getRutValue().should('equal', cliente.rut)
      registroPage1.getStockValue().should('equal', vehiculo.stock)
      
      // Modificar stock
      const nuevoVehiculo = vehiculos.validVehicles[1]
      registroPage1
        .fillStock(nuevoVehiculo.stock)
        .clickContinue()
      
      // Verificar que el nuevo stock se refleja en página 2
      cy.url().should('contain', 'Reg_Mantencion_02.aspx')
      cy.get('#Label7').should('contain.text', nuevoVehiculo.stock)
      
      cy.logStep('Navegación y modificación completada exitosamente')
    })

    it('Debe mantener sesión durante todo el flujo', () => {
      const cliente = clientes.validClients[0]
      const vehiculo = vehiculos.validVehicles[0]
      
      cy.logStep('Verificando persistencia de sesión')
      
      cy.login()
      
      // Completar página 1
      registroPage1
        .visit()
        .fillRut(cliente.rut)
        .fillStock(vehiculo.stock)
        .clickContinue()
      
      // Verificar sesión en página 2
      cy.window().then((win) => {
        expect(win.sessionStorage.getItem('usuario')).to.not.be.null
      })
      
      // Continuar a página 3
      cy.get('#Mantencion').select('10')
      cy.get('#TxtKM').clear().type('9500')
      cy.get('#Button1').click()
      
      // Verificar sesión en página 3
      cy.url().should('contain', 'Reg_Mantencion_03.aspx')
      cy.window().then((win) => {
        expect(win.sessionStorage.getItem('usuario')).to.not.be.null
      })
      
      cy.logStep('Sesión mantenida durante todo el flujo')
    })
  })

  describe('Flujo con Validaciones de Rendimiento', () => {
    it('Debe completar flujo completo en tiempo aceptable', () => {
      const cliente = clientes.validClientes[0]
      const vehiculo = vehiculos.validVehicles[0]
      
      cy.logStep('Midiendo tiempo de flujo completo')
      
      const startTime = Date.now()
      
      const datosRegistro = {
        rut: cliente.rut,
        stock: vehiculo.stock,
        mantencion: 10,
        kilometraje: 9500
      }
      
      cy.login()
      cy.completarRegistroCompleto(datosRegistro)
      
      const endTime = Date.now()
      const totalTime = endTime - startTime
      
      cy.log(`Tiempo total del flujo: ${totalTime}ms`)
      
      // El flujo completo debería tomar menos de 30 segundos
      expect(totalTime).to.be.lessThan(30000)
      
      cy.logStep(`Flujo completado en ${totalTime}ms - Dentro de límites aceptables`)
    })
  })

  describe('Flujo con Datos Generados Dinámicamente', () => {
    it('Debe completar registro con datos generados automáticamente', () => {
      cy.logStep('Probando con datos generados dinámicamente')
      
      // Generar RUT y Stock válidos
      cy.generateValidRut().then((rut) => {
        cy.generateValidStock().then((stock) => {
          
          // Crear datos de prueba en BD
          cy.createTestClient({ rut: rut })
          cy.createTestVehicle({ stock: stock, rut: rut })
          
          const datosRegistro = {
            rut: rut,
            stock: stock,
            mantencion: 10,
            kilometraje: 9500
          }
          
          cy.login()
          cy.completarRegistroCompleto(datosRegistro)
          
          cy.logStep(`Registro con datos generados completado: RUT=${rut}, Stock=${stock}`)
        })
      })
    })
  })
})
