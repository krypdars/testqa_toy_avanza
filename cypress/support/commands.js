// ***********************************************
// Comandos personalizados para Registro de Mantenciones
// ***********************************************

import 'cypress-real-events';
import 'cypress-file-upload';

// Comando para login en el sistema
Cypress.Commands.add('login', (username, password) => {
  username = username || Cypress.env('testUser');
  password = password || Cypress.env('testPassword');
  
  cy.log(`Iniciando sesión con usuario: ${username}`);
  
  cy.visit(Cypress.env('loginUrl'));
  cy.waitForPageLoad();
  
  // Llenar formulario de login
  cy.get('#txtUsuario', { timeout: 10000 }).should('be.visible').clear().type(username);
  cy.get('#txtPassword').should('be.visible').clear().type(password);
  cy.get('#btnLogin').should('be.visible').click();
  
  // Verificar que el login fue exitoso
  cy.url({ timeout: 15000 }).should('not.contain', 'default.aspx');
  cy.log('Login exitoso');
});

// Comando para logout
Cypress.Commands.add('logout', () => {
  cy.log('Cerrando sesión');
  
  // Buscar botón de logout o limpiar sesión
  cy.window().then((win) => {
    win.sessionStorage.clear();
    win.localStorage.clear();
  });
  
  cy.visit(Cypress.env('loginUrl'));
});

// Comando para completar página 1 del registro
Cypress.Commands.add('completarPagina1', (rut, stock, options = {}) => {
  const { shouldSucceed = true, waitForValidation = true } = options;
  
  cy.log(`Completando página 1 - RUT: ${rut}, Stock: ${stock}`);
  
  cy.visit(Cypress.env('registroUrl'));
  cy.waitForPageLoad();
  
  if (rut) {
    cy.get('#TxtRut').should('be.visible').clear().type(rut);
    
    if (waitForValidation) {
      cy.get('#TxtRut').blur(); // Trigger validation
      cy.wait(1000); // Esperar validación
    }
  }
  
  if (stock) {
    cy.get('#TxtStock').should('be.visible').clear().type(stock);
    
    if (waitForValidation) {
      cy.get('#TxtStock').blur(); // Trigger validation
      cy.wait(2000); // Esperar validación de BD
    }
  }
  
  cy.get('#Button1').should('be.visible').click();
  
  if (shouldSucceed) {
    cy.url({ timeout: 15000 }).should('contain', 'Reg_Mantencion_02.aspx');
    cy.log('Página 1 completada exitosamente');
  }
});

// Comando para completar página 2 del registro
Cypress.Commands.add('completarPagina2', (mantencion, kilometraje, asesor, options = {}) => {
  const { shouldSucceed = true, uploadFile = false } = options;
  
  cy.log(`Completando página 2 - Mantención: ${mantencion}, KM: ${kilometraje}`);
  
  // Verificar que estamos en página 2
  cy.url().should('contain', 'Reg_Mantencion_02.aspx');
  cy.waitForPageLoad();
  
  if (mantencion) {
    cy.get('#Mantencion').should('be.visible').select(mantencion.toString());
  }
  
  if (kilometraje) {
    cy.get('#TxtKM').should('be.visible').clear().type(kilometraje.toString());
  }
  
  if (asesor) {
    cy.get('#CmbAsesor').should('be.visible').select(asesor);
  }
  
  // Subir archivo si es necesario
  if (uploadFile) {
    cy.fixture('test-document.pdf', 'base64').then(fileContent => {
      cy.get('#FileUpload1').attachFile({
        fileContent,
        fileName: 'test-document.pdf',
        mimeType: 'application/pdf'
      });
    });
  }
  
  cy.get('#Button1').should('be.visible').click();
  
  if (shouldSucceed) {
    cy.url({ timeout: 15000 }).should('contain', 'Reg_Mantencion_03.aspx');
    cy.log('Página 2 completada exitosamente');
  }
});

// Comando para completar página 3 (confirmación)
Cypress.Commands.add('completarPagina3', (options = {}) => {
  const { shouldSucceed = true } = options;
  
  cy.log('Completando página 3 - Confirmación');
  
  // Verificar que estamos en página 3
  cy.url().should('contain', 'Reg_Mantencion_03.aspx');
  cy.waitForPageLoad();
  
  // Verificar que los datos se muestran
  cy.get('body').should('contain', 'Confirmación');
  
  cy.get('#Button1').should('be.visible').click();
  
  if (shouldSucceed) {
    cy.url({ timeout: 15000 }).should('contain', 'Reg_Mantencion_04.aspx');
    cy.log('Página 3 completada exitosamente');
  }
});

// Comando para generar RUT válido
Cypress.Commands.add('generateValidRut', () => {
  return cy.task('generateValidRut');
});

// Comando para generar Stock válido
Cypress.Commands.add('generateValidStock', () => {
  return cy.task('generateValidStock');
});

// Comando para validar RUT
Cypress.Commands.add('validateRut', (rut) => {
  return cy.task('validateRut', rut);
});

// Comando para esperar carga completa de página
Cypress.Commands.add('waitForPageLoad', () => {
  cy.window().should('have.property', 'document');
  cy.document().should('have.property', 'readyState', 'complete');
  
  // Esperar que jQuery esté disponible si se usa
  cy.window().then((win) => {
    if (win.jQuery) {
      cy.wrap(null).should(() => {
        expect(win.jQuery.active).to.equal(0);
      });
    }
  });
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

// Comando para crear vehículo de prueba
Cypress.Commands.add('createTestVehicle', (vehicleData) => {
  return cy.task('createTestVehicle', vehicleData);
});

// Comando para crear cliente de prueba
Cypress.Commands.add('createTestClient', (clientData) => {
  return cy.task('createTestClient', clientData);
});

// Comando para manejar alertas JavaScript
Cypress.Commands.add('handleAlert', (action = 'accept') => {
  cy.window().then((win) => {
    cy.stub(win, 'alert').as('windowAlert');
    cy.stub(win, 'confirm').returns(action === 'accept');
  });
});

// Comando para esperar elemento específico
Cypress.Commands.add('waitForElement', (selector, timeout = 10000) => {
  cy.get(selector, { timeout }).should('exist').and('be.visible');
});

// Comando para scroll hasta elemento
Cypress.Commands.add('scrollToElement', (selector) => {
  cy.get(selector).scrollIntoView();
  cy.wait(500); // Esperar que termine el scroll
});

// Comando para verificar validación de campo
Cypress.Commands.add('verifyFieldValidation', (fieldSelector, errorMessage) => {
  cy.get(fieldSelector).should('have.class', 'error').or('have.attr', 'aria-invalid', 'true');
  
  if (errorMessage) {
    cy.get('#valSumario').should('contain.text', errorMessage);
  }
});

// Comando para llenar formulario completo de registro
Cypress.Commands.add('completarRegistroCompleto', (datosRegistro) => {
  const {
    rut,
    stock,
    mantencion,
    kilometraje,
    asesor,
    uploadFile = false
  } = datosRegistro;
  
  cy.log('Iniciando registro completo');
  
  // Página 1
  cy.completarPagina1(rut, stock);
  
  // Página 2
  cy.completarPagina2(mantencion, kilometraje, asesor, { uploadFile });
  
  // Página 3
  cy.completarPagina3();
  
  // Verificar página 4 (finalización)
  cy.url().should('contain', 'Reg_Mantencion_04.aspx');
  cy.get('body').should('contain.text', 'exitoso').or('contain.text', 'completado');
  
  cy.log('Registro completo finalizado exitosamente');
});

// Comando para interceptar llamadas AJAX/API
Cypress.Commands.add('interceptValidations', () => {
  // Interceptar validación de RUT
  cy.intercept('POST', '**/ValidateRut**').as('validateRut');
  
  // Interceptar validación de Stock
  cy.intercept('POST', '**/ValidateStock**').as('validateStock');
  
  // Interceptar llamadas SAP
  cy.intercept('POST', '**/SAP**').as('sapCall');
});

// Comando para esperar validaciones
Cypress.Commands.add('waitForValidations', () => {
  cy.wait('@validateRut', { timeout: 5000 }).then((interception) => {
    expect(interception.response.statusCode).to.equal(200);
  });
  
  cy.wait('@validateStock', { timeout: 8000 }).then((interception) => {
    expect(interception.response.statusCode).to.equal(200);
  });
});

// Comando para simular errores de red
Cypress.Commands.add('simulateNetworkError', (url) => {
  cy.intercept('POST', url, { forceNetworkError: true }).as('networkError');
});

// Comando para verificar accesibilidad básica
Cypress.Commands.add('checkA11y', (selector = null) => {
  // Verificaciones básicas de accesibilidad
  if (selector) {
    cy.get(selector).should('have.attr', 'aria-label').or('have.attr', 'title');
  } else {
    cy.get('input[type="text"]').each(($input) => {
      cy.wrap($input).should('have.attr', 'id');
      cy.get(`label[for="${$input.attr('id')}"]`).should('exist');
    });
  }
});

// Comando para performance testing básico
Cypress.Commands.add('measurePageLoad', (pageName) => {
  cy.window().then((win) => {
    const startTime = win.performance.now();
    
    cy.waitForPageLoad().then(() => {
      const endTime = win.performance.now();
      const loadTime = endTime - startTime;
      
      cy.log(`${pageName} cargó en ${loadTime.toFixed(2)}ms`);
      
      // Assertion de performance (menos de 3 segundos)
      expect(loadTime).to.be.lessThan(3000);
    });
  });
});

// Comando para logs personalizados
Cypress.Commands.add('logStep', (message) => {
  cy.task('log', message);
  cy.log(message);
});

// Sobrescribir comando visit para agregar logs
Cypress.Commands.overwrite('visit', (originalFn, url, options) => {
  cy.logStep(`Navegando a: ${url}`);
  return originalFn(url, options);
});

// Sobrescribir comando click para agregar logs
Cypress.Commands.overwrite('click', (originalFn, subject, options) => {
  cy.logStep(`Haciendo click en elemento`);
  return originalFn(subject, options);
});
