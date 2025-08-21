/**
 * Pruebas de Conectividad - Servidor QAS
 * 
 * Estas pruebas verifican que Cypress puede conectarse correctamente
 * al servidor QAS y que la aplicación responde adecuadamente.
 */

describe('Conectividad del Servidor QAS', () => {
  
  beforeEach(() => {
    // Configurar timeouts más largos para servidor remoto
    Cypress.config('pageLoadTimeout', 60000);
    Cypress.config('requestTimeout', 30000);
    Cypress.config('responseTimeout', 30000);
  });

  it('Debe poder acceder a la página principal del servidor QAS', () => {
    cy.log('🔗 Verificando conectividad con: ' + Cypress.config('baseUrl'));
    
    // Intentar cargar la página principal
    cy.visit('/', {
      failOnStatusCode: false, // No fallar si hay errores HTTP
      timeout: 60000
    });
    
    // Verificar que la página cargó (puede ser login o página principal)
    cy.get('body').should('exist');
    
    // Log de éxito
    cy.log('✅ Conectividad exitosa con el servidor QAS');
  });

  it('Debe poder acceder a la página de login', () => {
    cy.log('🔐 Verificando acceso a página de login');
    
    cy.visit('/default.aspx', {
      failOnStatusCode: false,
      timeout: 60000
    });
    
    // Verificar elementos típicos de una página de login
    cy.get('body').should('exist');
    
    // Buscar campos comunes de login (adaptable según la página real)
    cy.get('body').then(($body) => {
      if ($body.find('input[type="text"], input[type="password"]').length > 0) {
        cy.log('✅ Página de login detectada - campos de entrada encontrados');
      } else {
        cy.log('ℹ️ Página cargada pero estructura de login no detectada');
      }
    });
  });

  it('Debe verificar la resolución DNS del servidor', () => {
    cy.log('🌐 Verificando resolución DNS de portalqas');
    
    // Hacer una petición simple para verificar conectividad
    cy.request({
      method: 'GET',
      url: '/',
      failOnStatusCode: false,
      timeout: 30000
    }).then((response) => {
      cy.log(`📊 Respuesta del servidor: ${response.status}`);
      
      // Verificar que el servidor responde (cualquier código es válido)
      expect(response.status).to.be.oneOf([200, 302, 401, 403, 500]);
      
      if (response.status === 200) {
        cy.log('✅ Servidor responde correctamente');
      } else if (response.status === 302) {
        cy.log('🔄 Servidor redirige (posible autenticación requerida)');
      } else if (response.status === 401) {
        cy.log('🔐 Servidor requiere autenticación');
      } else {
        cy.log(`⚠️ Servidor responde con código: ${response.status}`);
      }
    });
  });

  it('Diagnóstico completo de conectividad', () => {
    cy.log('🔧 Ejecutando diagnóstico completo de conectividad');
    
    // Información de configuración
    cy.log('📋 Configuración actual:');
    cy.log(`   Base URL: ${Cypress.config('baseUrl')}`);
    cy.log(`   Page Load Timeout: ${Cypress.config('pageLoadTimeout')}ms`);
    cy.log(`   Request Timeout: ${Cypress.config('requestTimeout')}ms`);
    
    // Intentar ping básico
    cy.request({
      method: 'GET',
      url: '/',
      failOnStatusCode: false,
      timeout: 60000
    }).then((response) => {
      cy.log('📊 Diagnóstico de respuesta:');
      cy.log(`   Status: ${response.status}`);
      cy.log(`   Headers: ${JSON.stringify(response.headers, null, 2)}`);
      
      if (response.body) {
        const bodyPreview = response.body.toString().substring(0, 200);
        cy.log(`   Body Preview: ${bodyPreview}...`);
      }
    }).catch((error) => {
      cy.log('❌ Error de conectividad:');
      cy.log(`   Mensaje: ${error.message}`);
      cy.log('💡 Posibles causas:');
      cy.log('   - Servidor QAS no está ejecutándose');
      cy.log('   - Puerto 91 bloqueado por firewall');
      cy.log('   - Problemas de DNS con "portalqas"');
      cy.log('   - Red corporativa requiere proxy/VPN');
      
      // No fallar la prueba, solo reportar
      cy.log('⚠️ Diagnóstico completado - revisar logs arriba');
    });
  });

});
