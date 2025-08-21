/**
 * Page Object para la página de Login del sistema
 */
class LoginPage {
  // Selectores de elementos
  get usernameInput() { 
    return cy.get('#txtUsuario'); 
  }
  
  get passwordInput() { 
    return cy.get('#txtPassword'); 
  }
  
  get loginButton() { 
    return cy.get('#btnLogin'); 
  }
  
  get errorMessage() { 
    return cy.get('.error-message, .validation-summary-errors'); 
  }
  
  get forgotPasswordLink() { 
    return cy.get('a[href*="forgot"]'); 
  }
  
  get rememberMeCheckbox() { 
    return cy.get('#chkRememberMe'); 
  }

  // Métodos de navegación
  visit() {
    cy.visit(Cypress.env('loginUrl'));
    cy.waitForPageLoad();
    return this;
  }

  // Métodos de interacción
  fillUsername(username) {
    this.usernameInput.clear().type(username);
    return this;
  }

  fillPassword(password) {
    this.passwordInput.clear().type(password);
    return this;
  }

  clickLogin() {
    this.loginButton.click();
    return this;
  }

  toggleRememberMe() {
    this.rememberMeCheckbox.click();
    return this;
  }

  // Métodos de acción completa
  login(username, password) {
    this.fillUsername(username);
    this.fillPassword(password);
    this.clickLogin();
    return this;
  }

  loginWithValidCredentials() {
    const username = Cypress.env('testUser');
    const password = Cypress.env('testPassword');
    return this.login(username, password);
  }

  loginWithInvalidCredentials() {
    return this.login('usuario_invalido', 'password_invalido');
  }

  // Métodos de verificación
  verifyLoginSuccess() {
    cy.url({ timeout: 15000 }).should('not.contain', 'default.aspx');
    cy.url().should('not.contain', 'login');
    return this;
  }

  verifyLoginError(expectedMessage = null) {
    if (expectedMessage) {
      this.errorMessage.should('be.visible').and('contain.text', expectedMessage);
    } else {
      this.errorMessage.should('be.visible');
    }
    return this;
  }

  verifyStillOnLoginPage() {
    cy.url().should('contain', 'default.aspx');
    return this;
  }

  verifyPageElements() {
    this.usernameInput.should('be.visible');
    this.passwordInput.should('be.visible');
    this.loginButton.should('be.visible');
    return this;
  }

  verifyUsernameFieldFocused() {
    this.usernameInput.should('be.focused');
    return this;
  }

  // Métodos de validación de campos
  verifyUsernameRequired() {
    this.usernameInput.clear();
    this.passwordInput.type('password123');
    this.clickLogin();
    
    // Verificar que muestra error de campo requerido
    cy.get('span[data-valmsg-for="txtUsuario"]').should('be.visible');
    return this;
  }

  verifyPasswordRequired() {
    this.usernameInput.type('admin');
    this.passwordInput.clear();
    this.clickLogin();
    
    // Verificar que muestra error de campo requerido
    cy.get('span[data-valmsg-for="txtPassword"]').should('be.visible');
    return this;
  }

  // Métodos de utilidad
  clearForm() {
    this.usernameInput.clear();
    this.passwordInput.clear();
    return this;
  }

  getLoginButtonText() {
    return this.loginButton.invoke('val');
  }

  isLoginButtonEnabled() {
    return this.loginButton.should('not.be.disabled');
  }

  // Métodos para testing de seguridad
  attemptSQLInjection() {
    const sqlInjection = "'; DROP TABLE Users; --";
    this.fillUsername(sqlInjection);
    this.fillPassword('password');
    this.clickLogin();
    return this;
  }

  attemptXSSAttack() {
    const xssPayload = '<script>alert("XSS")</script>';
    this.fillUsername(xssPayload);
    this.fillPassword('password');
    this.clickLogin();
    return this;
  }

  // Métodos para testing de performance
  measureLoginTime() {
    const startTime = Date.now();
    
    this.loginWithValidCredentials();
    this.verifyLoginSuccess();
    
    const endTime = Date.now();
    const loginTime = endTime - startTime;
    
    cy.log(`Tiempo de login: ${loginTime}ms`);
    
    // Verificar que el login toma menos de 5 segundos
    expect(loginTime).to.be.lessThan(5000);
    
    return this;
  }

  // Métodos para diferentes escenarios de prueba
  loginScenario(scenario) {
    switch (scenario) {
      case 'valid':
        return this.loginWithValidCredentials();
      
      case 'invalid':
        return this.loginWithInvalidCredentials();
      
      case 'empty':
        return this.login('', '');
      
      case 'username_only':
        return this.login(Cypress.env('testUser'), '');
      
      case 'password_only':
        return this.login('', Cypress.env('testPassword'));
      
      case 'sql_injection':
        return this.attemptSQLInjection();
      
      case 'xss':
        return this.attemptXSSAttack();
      
      default:
        throw new Error(`Escenario de login no reconocido: ${scenario}`);
    }
  }

  // Método para verificar accesibilidad
  verifyAccessibility() {
    // Verificar labels
    this.usernameInput.should('have.attr', 'id');
    this.passwordInput.should('have.attr', 'id');
    
    cy.get('label[for="txtUsuario"]').should('exist');
    cy.get('label[for="txtPassword"]').should('exist');
    
    // Verificar navegación por teclado
    this.usernameInput.focus().tab();
    this.passwordInput.should('be.focused');
    
    this.passwordInput.tab();
    this.loginButton.should('be.focused');
    
    return this;
  }

  // Método para testing responsive
  verifyResponsiveDesign() {
    // Desktop
    cy.viewport(1280, 720);
    this.verifyPageElements();
    
    // Tablet
    cy.viewport(768, 1024);
    this.verifyPageElements();
    
    // Mobile
    cy.viewport(375, 667);
    this.verifyPageElements();
    
    return this;
  }
}

export default LoginPage;
