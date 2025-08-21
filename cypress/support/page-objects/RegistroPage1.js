/**
 * Page Object para la Página 1 del Registro de Mantenciones
 * Maneja RUT del cliente y N° Stock del vehículo
 */
class RegistroPage1 {
  // Selectores de elementos
  get rutInput() { 
    return cy.get('#TxtRut'); 
  }
  
  get stockInput() { 
    return cy.get('#TxtStock'); 
  }
  
  get continueButton() { 
    return cy.get('#Button1'); 
  }
  
  get menuButton() { 
    return cy.get('#Button3'); 
  }
  
  get validationSummary() { 
    return cy.get('#valSumario'); 
  }
  
  get helpButton() { 
    return cy.get('input[value="Ayuda"]'); 
  }
  
  get helpContent() { 
    return cy.get('#LblAyuda'); 
  }
  
  get pageTitle() { 
    return cy.get('#Label1'); 
  }
  
  get organizationLabel() { 
    return cy.get('#Label4'); 
  }
  
  // Validadores específicos
  get rutRequiredValidator() { 
    return cy.get('#RequiredFieldValidator3'); 
  }
  
  get rutFormatValidator() { 
    return cy.get('#RegularExpressionValidator1'); 
  }
  
  get rutCustomValidator() { 
    return cy.get('#MiValidadorRUT'); 
  }
  
  get rutClientValidator() { 
    return cy.get('#MiValidadorRUT2'); 
  }
  
  get stockRequiredValidator() { 
    return cy.get('#RequiredFieldValidator1'); 
  }
  
  get stockCustomValidator() { 
    return cy.get('#ValidaStockReg'); 
  }

  // Métodos de navegación
  visit() {
    cy.visit(Cypress.env('registroUrl'));
    cy.waitForPageLoad();
    return this;
  }

  visitWithParams(rut, dv, stock) {
    const url = `${Cypress.env('registroUrl')}?rut=${rut}&dv=${dv}&stock=${stock}`;
    cy.visit(url);
    cy.waitForPageLoad();
    return this;
  }

  // Métodos de interacción básica
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

  clickMenu() {
    this.menuButton.click();
    return this;
  }

  clickHelp() {
    this.helpButton.click();
    return this;
  }

  // Métodos de acción completa
  fillFormAndContinue(rut, stock, options = {}) {
    const { waitForValidation = true, shouldSucceed = true } = options;
    
    this.fillRut(rut);
    
    if (waitForValidation) {
      this.rutInput.blur(); // Trigger RUT validation
      cy.wait(1000);
    }
    
    this.fillStock(stock);
    
    if (waitForValidation) {
      this.stockInput.blur(); // Trigger Stock validation
      cy.wait(2000); // Stock validation takes longer (DB + SAP)
    }
    
    this.clickContinue();
    
    if (shouldSucceed) {
      this.verifyPageTransition();
    }
    
    return this;
  }

  fillValidForm() {
    const validRut = Cypress.env('validRut');
    const validStock = Cypress.env('validStock');
    return this.fillFormAndContinue(validRut, validStock);
  }

  // Métodos de verificación
  verifyPageElements() {
    this.pageTitle.should('be.visible').and('contain.text', 'Registro de Mantenciones');
    this.rutInput.should('be.visible');
    this.stockInput.should('be.visible');
    this.continueButton.should('be.visible');
    this.menuButton.should('be.visible');
    return this;
  }

  verifyPageTransition() {
    cy.url({ timeout: 15000 }).should('contain', 'Reg_Mantencion_02.aspx');
    return this;
  }

  verifyStillOnPage1() {
    cy.url().should('contain', 'Reg_Mantencion_01.aspx');
    return this;
  }

  verifyValidationError(message = null) {
    this.validationSummary.should('be.visible');
    
    if (message) {
      this.validationSummary.should('contain.text', message);
    }
    
    return this;
  }

  verifyNoValidationErrors() {
    this.validationSummary.should('not.be.visible');
    return this;
  }

  verifyOrganizationInfo(expectedOrg = null) {
    this.organizationLabel.should('be.visible');
    
    if (expectedOrg) {
      this.organizationLabel.should('contain.text', expectedOrg);
    }
    
    return this;
  }

  // Métodos de validación específica de RUT
  verifyRutFormatError() {
    this.rutFormatValidator.should('be.visible');
    this.verifyValidationError('Formato Rut no valido');
    return this;
  }

  verifyRutInvalidError() {
    this.rutCustomValidator.should('be.visible');
    this.verifyValidationError('Rut ingresado no es valido');
    return this;
  }

  verifyRutRequiredError() {
    this.rutRequiredValidator.should('be.visible');
    this.verifyValidationError('Rut es obligatorio');
    return this;
  }

  verifyClientNotFoundError() {
    this.rutClientValidator.should('be.visible');
    this.verifyValidationError('R.U.T. Ingresado no figura como cliente');
    return this;
  }

  // Métodos de validación específica de Stock
  verifyStockRequiredError() {
    this.stockRequiredValidator.should('be.visible');
    this.verifyValidationError('Stock es obligatorio');
    return this;
  }

  verifyStockNotFoundError() {
    this.stockCustomValidator.should('be.visible');
    this.verifyValidationError('N° Stock no Existe');
    return this;
  }

  // Métodos de testing de validaciones
  testRutValidations() {
    // Test RUT requerido
    this.fillRut('');
    this.fillStock('ABC123');
    this.clickContinue();
    this.verifyRutRequiredError();
    
    // Test formato incorrecto
    this.fillRut('123456789');
    this.clickContinue();
    this.verifyRutFormatError();
    
    // Test dígito verificador incorrecto
    this.fillRut('12345678-9');
    this.clickContinue();
    this.verifyRutInvalidError();
    
    // Test números repetidos
    this.fillRut('11111111-1');
    this.clickContinue();
    this.verifyRutInvalidError();
    
    return this;
  }

  testStockValidations() {
    const validRut = Cypress.env('validRut');
    
    // Test Stock requerido
    this.fillRut(validRut);
    this.fillStock('');
    this.clickContinue();
    this.verifyStockRequiredError();
    
    // Test Stock inexistente
    this.fillStock('STOCKINEXISTENTE');
    this.clickContinue();
    this.verifyStockNotFoundError();
    
    return this;
  }

  // Métodos para diferentes escenarios de RUT
  testRutScenarios() {
    const scenarios = [
      { rut: '', expected: 'required' },
      { rut: '123456789', expected: 'format' },
      { rut: '12345678-9', expected: 'invalid' },
      { rut: '11111111-1', expected: 'invalid' },
      { rut: '87654321-0', expected: 'not_found' }, // Cliente no registrado
      { rut: '12345678-5', expected: 'valid' }
    ];
    
    scenarios.forEach(scenario => {
      cy.log(`Testing RUT scenario: ${scenario.rut} (${scenario.expected})`);
      
      this.fillRut(scenario.rut);
      this.fillStock('ABC123');
      this.rutInput.blur();
      cy.wait(1000);
      
      switch (scenario.expected) {
        case 'required':
          this.verifyRutRequiredError();
          break;
        case 'format':
          this.verifyRutFormatError();
          break;
        case 'invalid':
          this.verifyRutInvalidError();
          break;
        case 'not_found':
          this.verifyClientNotFoundError();
          break;
        case 'valid':
          this.verifyNoValidationErrors();
          break;
      }
      
      // Limpiar para siguiente escenario
      this.rutInput.clear();
    });
    
    return this;
  }

  // Métodos para diferentes escenarios de Stock
  testStockScenarios() {
    const validRut = Cypress.env('validRut');
    
    const scenarios = [
      { stock: '', expected: 'required' },
      { stock: 'STOCKINEXISTENTE', expected: 'not_found' },
      { stock: 'ABC123', expected: 'valid' }
    ];
    
    scenarios.forEach(scenario => {
      cy.log(`Testing Stock scenario: ${scenario.stock} (${scenario.expected})`);
      
      this.fillRut(validRut);
      this.fillStock(scenario.stock);
      this.stockInput.blur();
      cy.wait(2000); // Stock validation takes longer
      
      switch (scenario.expected) {
        case 'required':
          this.verifyStockRequiredError();
          break;
        case 'not_found':
          this.verifyStockNotFoundError();
          break;
        case 'valid':
          this.verifyNoValidationErrors();
          break;
      }
      
      // Limpiar para siguiente escenario
      this.stockInput.clear();
    });
    
    return this;
  }

  // Métodos de utilidad
  clearForm() {
    this.rutInput.clear();
    this.stockInput.clear();
    return this;
  }

  getRutValue() {
    return this.rutInput.invoke('val');
  }

  getStockValue() {
    return this.stockInput.invoke('val');
  }

  isContinueButtonEnabled() {
    return this.continueButton.should('not.be.disabled');
  }

  // Métodos para testing de ayuda
  verifyHelpFunctionality() {
    this.clickHelp();
    this.helpContent.should('be.visible');
    
    // Verificar que se puede cerrar la ayuda
    this.clickHelp();
    this.helpContent.should('not.be.visible');
    
    return this;
  }

  // Métodos para testing de accesibilidad
  verifyAccessibility() {
    // Verificar labels
    cy.get('label[for="TxtRut"]').should('exist');
    cy.get('label[for="TxtStock"]').should('exist');
    
    // Verificar navegación por teclado
    this.rutInput.focus().tab();
    this.stockInput.should('be.focused');
    
    this.stockInput.tab();
    this.continueButton.should('be.focused');
    
    return this;
  }

  // Métodos para testing de performance
  measureValidationTime() {
    const validRut = Cypress.env('validRut');
    const validStock = Cypress.env('validStock');
    
    // Medir tiempo de validación de RUT
    const rutStartTime = Date.now();
    this.fillRut(validRut);
    this.rutInput.blur();
    
    cy.wait(1000).then(() => {
      const rutEndTime = Date.now();
      const rutValidationTime = rutEndTime - rutStartTime;
      cy.log(`Tiempo validación RUT: ${rutValidationTime}ms`);
      expect(rutValidationTime).to.be.lessThan(2000);
    });
    
    // Medir tiempo de validación de Stock
    const stockStartTime = Date.now();
    this.fillStock(validStock);
    this.stockInput.blur();
    
    cy.wait(3000).then(() => {
      const stockEndTime = Date.now();
      const stockValidationTime = stockEndTime - stockStartTime;
      cy.log(`Tiempo validación Stock: ${stockValidationTime}ms`);
      expect(stockValidationTime).to.be.lessThan(5000);
    });
    
    return this;
  }

  // Métodos para testing de seguridad
  testSecurityVulnerabilities() {
    // Test SQL Injection en RUT
    this.fillRut("'; DROP TABLE Clientes; --");
    this.fillStock('ABC123');
    this.clickContinue();
    this.verifyRutInvalidError(); // Debería fallar la validación, no ejecutar SQL
    
    // Test XSS en Stock
    this.fillRut('12345678-5');
    this.fillStock('<script>alert("XSS")</script>');
    this.clickContinue();
    this.verifyStockNotFoundError(); // Debería fallar la validación, no ejecutar script
    
    return this;
  }

  // Método para generar datos de prueba dinámicos
  fillWithGeneratedData() {
    cy.generateValidRut().then(rut => {
      this.fillRut(rut);
    });
    
    cy.generateValidStock().then(stock => {
      this.fillStock(stock);
    });
    
    return this;
  }
}

export default RegistroPage1;
