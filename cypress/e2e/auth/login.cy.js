import LoginPage from '../../support/page-objects/LoginPage'

describe('Autenticación - Login', () => {
  let loginPage
  let usuarios

  before(() => {
    // Cargar datos de prueba
    cy.fixture('usuarios').then((data) => {
      usuarios = data
    })
  })

  beforeEach(() => {
    loginPage = new LoginPage()
    
    // Limpiar datos de sesión
    cy.clearCookies()
    cy.clearLocalStorage()
    
    // Visitar página de login
    loginPage.visit()
  })

  describe('Login Exitoso', () => {
    it('Debe permitir login con credenciales válidas', () => {
      const usuario = usuarios.validUsers[0]
      
      loginPage
        .login(usuario.username, usuario.password)
        .verifyLoginSuccess()
      
      cy.logStep('Login exitoso completado')
    })

    it('Debe mantener sesión después del login', () => {
      const usuario = usuarios.validUsers[0]
      
      loginPage.loginWithValidCredentials()
      
      // Verificar que la sesión se mantiene al navegar
      cy.visit(Cypress.env('registroUrl'))
      cy.url().should('contain', 'Reg_Mantencion_01.aspx')
      
      cy.logStep('Sesión mantenida correctamente')
    })

    it('Debe mostrar información de organización después del login', () => {
      const usuario = usuarios.validUsers[0]
      
      loginPage.loginWithValidCredentials()
      
      // Navegar a página de registro para verificar organización
      cy.visit(Cypress.env('registroUrl'))
      cy.get('#Label4').should('be.visible').and('contain.text', 'Ud. Esta en')
    })

    it('Debe permitir login con diferentes roles de usuario', () => {
      usuarios.validUsers.forEach((usuario) => {
        cy.logStep(`Probando login con usuario: ${usuario.username} (${usuario.role})`)
        
        loginPage
          .visit()
          .login(usuario.username, usuario.password)
          .verifyLoginSuccess()
        
        // Logout para siguiente iteración
        cy.logout()
      })
    })
  })

  describe('Login Fallido', () => {
    it('Debe rechazar credenciales inválidas', () => {
      const usuario = usuarios.invalidUsers[0]
      
      loginPage
        .login(usuario.username, usuario.password)
        .verifyLoginError()
        .verifyStillOnLoginPage()
      
      cy.logStep('Credenciales inválidas rechazadas correctamente')
    })

    it('Debe rechazar usuario válido con contraseña incorrecta', () => {
      const validUser = usuarios.validUsers[0]
      
      loginPage
        .login(validUser.username, 'password_incorrecto')
        .verifyLoginError()
        .verifyStillOnLoginPage()
    })

    it('Debe rechazar usuario inexistente', () => {
      loginPage
        .login('usuario_que_no_existe', 'cualquier_password')
        .verifyLoginError()
        .verifyStillOnLoginPage()
    })

    it('Debe validar campos requeridos', () => {
      // Test usuario vacío
      loginPage
        .login('', 'password123')
        .verifyUsernameRequired()
      
      // Test contraseña vacía
      loginPage
        .clearForm()
        .login('admin', '')
        .verifyPasswordRequired()
      
      // Test ambos campos vacíos
      loginPage
        .clearForm()
        .login('', '')
        .verifyUsernameRequired()
    })
  })

  describe('Validaciones de Seguridad', () => {
    it('Debe prevenir inyección SQL en username', () => {
      const sqlInjection = usuarios.securityTestUsers.find(u => 
        u.description.includes('inyección SQL en username')
      )
      
      loginPage
        .login(sqlInjection.username, sqlInjection.password)
        .verifyLoginError(sqlInjection.expectedError)
      
      cy.logStep('Inyección SQL en username bloqueada')
    })

    it('Debe prevenir inyección SQL en password', () => {
      const sqlInjection = usuarios.securityTestUsers.find(u => 
        u.description.includes('inyección SQL en password')
      )
      
      loginPage
        .login(sqlInjection.username, sqlInjection.password)
        .verifyLoginError()
      
      cy.logStep('Inyección SQL en password bloqueada')
    })

    it('Debe prevenir XSS en campos de entrada', () => {
      const xssAttempt = usuarios.securityTestUsers.find(u => 
        u.description.includes('XSS')
      )
      
      loginPage
        .login(xssAttempt.username, xssAttempt.password)
        .verifyLoginError()
      
      // Verificar que no se ejecutó JavaScript malicioso
      cy.window().then((win) => {
        expect(win.document.body.innerHTML).to.not.contain('<script>')
      })
      
      cy.logStep('Intento de XSS bloqueado')
    })

    it('Debe limpiar datos sensibles del DOM', () => {
      const usuario = usuarios.validUsers[0]
      
      loginPage.login(usuario.username, usuario.password)
      
      // Verificar que la contraseña no queda en el DOM
      cy.get('#txtPassword').should('have.value', '')
      
      cy.logStep('Datos sensibles limpiados del DOM')
    })
  })

  describe('Funcionalidad de Interfaz', () => {
    it('Debe mostrar todos los elementos de la página', () => {
      loginPage.verifyPageElements()
      
      cy.logStep('Todos los elementos de la página están presentes')
    })

    it('Debe enfocar el campo de usuario al cargar', () => {
      loginPage.verifyUsernameFieldFocused()
      
      cy.logStep('Campo de usuario enfocado correctamente')
    })

    it('Debe permitir navegación por teclado', () => {
      loginPage.verifyAccessibility()
      
      cy.logStep('Navegación por teclado funciona correctamente')
    })

    it('Debe ser responsive en diferentes tamaños de pantalla', () => {
      loginPage.verifyResponsiveDesign()
      
      cy.logStep('Diseño responsive verificado')
    })

    it('Debe mostrar/ocultar contraseña si está implementado', () => {
      // Verificar si existe botón de mostrar/ocultar contraseña
      cy.get('body').then(($body) => {
        if ($body.find('[data-toggle="password"]').length > 0) {
          cy.get('[data-toggle="password"]').click()
          cy.get('#txtPassword').should('have.attr', 'type', 'text')
          
          cy.get('[data-toggle="password"]').click()
          cy.get('#txtPassword').should('have.attr', 'type', 'password')
          
          cy.logStep('Funcionalidad mostrar/ocultar contraseña verificada')
        } else {
          cy.logStep('Funcionalidad mostrar/ocultar contraseña no implementada')
        }
      })
    })
  })

  describe('Rendimiento', () => {
    it('Debe completar login en tiempo aceptable', () => {
      loginPage.measureLoginTime()
      
      cy.logStep('Tiempo de login dentro de límites aceptables')
    })

    it('Debe cargar página de login rápidamente', () => {
      const startTime = Date.now()
      
      loginPage.visit()
      
      cy.waitForPageLoad().then(() => {
        const loadTime = Date.now() - startTime
        cy.log(`Tiempo de carga de página: ${loadTime}ms`)
        
        expect(loadTime).to.be.lessThan(3000)
      })
      
      cy.logStep('Página de login carga en tiempo aceptable')
    })
  })

  describe('Casos Edge', () => {
    it('Debe manejar caracteres especiales en credenciales', () => {
      const specialChars = 'áéíóúñ@#$%&'
      
      loginPage
        .login(specialChars, specialChars)
        .verifyLoginError()
      
      cy.logStep('Caracteres especiales manejados correctamente')
    })

    it('Debe manejar credenciales muy largas', () => {
      const longString = 'a'.repeat(1000)
      
      loginPage
        .login(longString, longString)
        .verifyLoginError()
      
      cy.logStep('Credenciales largas manejadas correctamente')
    })

    it('Debe manejar múltiples intentos de login fallidos', () => {
      const invalidUser = usuarios.invalidUsers[0]
      
      // Intentar login fallido múltiples veces
      for (let i = 0; i < 3; i++) {
        loginPage
          .clearForm()
          .login(invalidUser.username, invalidUser.password)
          .verifyLoginError()
        
        cy.wait(1000) // Esperar entre intentos
      }
      
      cy.logStep('Múltiples intentos fallidos manejados correctamente')
    })

    it('Debe manejar pérdida de conexión durante login', () => {
      // Simular error de red
      cy.intercept('POST', '**/login**', { forceNetworkError: true }).as('loginError')
      
      loginPage.loginWithValidCredentials()
      
      // Verificar manejo del error
      cy.wait('@loginError')
      
      // La página debería mostrar algún tipo de error o permanecer en login
      cy.url().should('contain', 'default.aspx')
      
      cy.logStep('Error de conexión manejado correctamente')
    })
  })

  describe('Accesibilidad', () => {
    it('Debe cumplir estándares básicos de accesibilidad', () => {
      loginPage.verifyAccessibility()
      
      // Verificar contraste de colores (básico)
      cy.get('#txtUsuario').should('have.css', 'color')
      cy.get('#txtPassword').should('have.css', 'color')
      
      // Verificar que los elementos tienen roles apropiados
      cy.get('#btnLogin').should('have.attr', 'type', 'submit')
      
      cy.logStep('Estándares básicos de accesibilidad cumplidos')
    })

    it('Debe ser navegable solo con teclado', () => {
      // Navegar usando solo Tab y Enter
      cy.get('body').tab()
      cy.focused().should('have.id', 'txtUsuario')
      
      cy.focused().type(usuarios.validUsers[0].username).tab()
      cy.focused().should('have.id', 'txtPassword')
      
      cy.focused().type(usuarios.validUsers[0].password).tab()
      cy.focused().should('have.id', 'btnLogin')
      
      cy.focused().type('{enter}')
      
      loginPage.verifyLoginSuccess()
      
      cy.logStep('Navegación por teclado completada exitosamente')
    })
  })
})
