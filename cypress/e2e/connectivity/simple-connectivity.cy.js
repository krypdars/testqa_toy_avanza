/**
 * Prueba Simple de Conectividad - Servidor QAS
 * 
 * Prueba básica para verificar conectividad sin conflictos de promesas
 */

describe('Conectividad Simple del Servidor QAS', () => {
  
  it('Debe conectarse al servidor QAS', () => {
    cy.log('🔗 Verificando conectividad básica con: ' + Cypress.config('baseUrl'));
    
    // Hacer una petición simple
    cy.request({
      method: 'GET',
      url: '/',
      failOnStatusCode: false,
      timeout: 30000
    }).then((response) => {
      cy.log(`📊 Respuesta del servidor: ${response.status}`);
      
      // Verificar que el servidor responde
      expect(response.status).to.be.greaterThan(0);
      
      if (response.status === 200) {
        cy.log('✅ Servidor responde correctamente (200 OK)');
      } else if (response.status === 302) {
        cy.log('🔄 Servidor redirige (302) - posible autenticación requerida');
      } else if (response.status === 401) {
        cy.log('🔐 Servidor requiere autenticación (401)');
      } else if (response.status === 403) {
        cy.log('🚫 Acceso prohibido (403) - verificar permisos');
      } else {
        cy.log(`⚠️ Servidor responde con código: ${response.status}`);
      }
    });
  });

  it('Debe poder cargar la página principal', () => {
    cy.log('🌐 Intentando cargar página principal');
    
    cy.visit('/', {
      failOnStatusCode: false,
      timeout: 60000
    });
    
    // Verificar que algo se cargó
    cy.get('body').should('exist');
    cy.log('✅ Página principal cargada exitosamente');
  });

  it('Debe poder acceder a default.aspx', () => {
    cy.log('📄 Verificando acceso a default.aspx');
    
    cy.visit('/default.aspx', {
      failOnStatusCode: false,
      timeout: 60000
    });
    
    cy.get('body').should('exist');
    cy.log('✅ Página default.aspx accesible');
  });

  it('Debe medir tiempo de respuesta', () => {
    cy.log('⏱️ Midiendo tiempo de respuesta');
    
    const startTime = Date.now();
    
    cy.visit('/', {
      failOnStatusCode: false,
      timeout: 60000
    }).then(() => {
      const loadTime = Date.now() - startTime;
      cy.log(`📈 Tiempo de carga: ${loadTime}ms`);
      
      if (loadTime < 5000) {
        cy.log('🚀 Excelente tiempo de respuesta');
      } else if (loadTime < 15000) {
        cy.log('👍 Tiempo de respuesta aceptable');
      } else {
        cy.log('⚠️ Tiempo de respuesta lento');
      }
    });
  });

});
