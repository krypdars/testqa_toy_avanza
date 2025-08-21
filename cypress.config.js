const { defineConfig } = require('cypress')

module.exports = defineConfig({
  e2e: {
    // URL base de la aplicación - usar variable de entorno o fallback
    baseUrl: process.env.CYPRESS_baseUrl || 'http://portalqas:91',
    projectId: "f2kkta",
    
    // Configuración de viewport
    viewportWidth: 1280,
    viewportHeight: 720,
    
    // Timeouts
    defaultCommandTimeout: 10000,
    requestTimeout: 10000,
    responseTimeout: 10000,
    pageLoadTimeout: 30000,
    
    // Configuración de video y screenshots
    video: true,
    screenshotOnRunFailure: true,
    
    // Configuración de archivos
    fixturesFolder: 'cypress/fixtures',
    screenshotsFolder: 'cypress/screenshots',
    videosFolder: 'cypress/videos',
    downloadsFolder: 'cypress/downloads',
    
    // Patrones de archivos de prueba
    specPattern: 'cypress/e2e/**/*.cy.{js,jsx,ts,tsx}',
    
    // Configuración del navegador
    chromeWebSecurity: false,
    
    // Variables de entorno específicas para Registro de Mantenciones
    env: {
      // URLs específicas
      loginUrl: '/default.aspx',
      registroUrl: '/Registro_Mantenciones/Reg_Mantencion_01.aspx',
      
      // Credenciales de prueba - usar variables de entorno
      testUser: process.env.CYPRESS_testUser || 'USERPRUEBA2',
      testPassword: process.env.CYPRESS_testPassword || '1234',
      
      // Configuración de base de datos
      dbHost: process.env.CYPRESS_dbHost || 'localhost',
      dbName: 'RegistroMantenciones_Test',
      
      // Datos de prueba válidos - usar variables de entorno
      validRut: process.env.CYPRESS_validRut || '8249882-6',
      validStock: process.env.CYPRESS_validStock || '0000906403',
      validClient: 'CLIENTE_PRUEBA',
      
      // Configuración de funcionalidades
      enableFileUpload: true,
      enableSAPIntegration: process.env.CI ? false : true, // Desactivar SAP en CI
      
      // Timeouts específicos
      sapTimeout: 5000,
      dbTimeout: 3000,
      
      // Configuración de organización
      clienteOrganizacion: 'TOYOTA_TEST',
      clienteSector: 'VENTAS',
      canalDistribucion: 'DIRECTO',
      
      // Configuración específica para CI/CD
      isCI: !!process.env.CI,
      retries: process.env.CI ? 2 : 0
    },
    
    // Configurar reintentos para CI
    retries: {
      runMode: 2,    // 2 reintentos en modo headless (CI)
      openMode: 0    // 0 reintentos en modo interactivo
    },
    
    setupNodeEvents(on, config) {
      // Configurar plugins
      require('cypress-mochawesome-reporter/plugin')(on);
      
      // Configurar tareas personalizadas para Registro de Mantenciones
      on('task', {
        // Tarea para limpiar base de datos de pruebas
        clearDatabase() {
          console.log('Limpiando base de datos de pruebas...');
          // Aquí implementarías la limpieza real de BD
          // Ejemplo: DELETE FROM Vehiculo_Mantencion WHERE test_data = 1
          return null;
        },
        
        // Tarea para insertar datos de prueba
        seedDatabase(data) {
          console.log('Insertando datos de prueba:', data);
          // Aquí implementarías la inserción de datos de prueba
          return null;
        },
        
        // Tarea para generar RUT válido chileno
        generateValidRut() {
          const rut = Math.floor(Math.random() * 89999999) + 10000000;
          const dv = calculateDV(rut);
          return `${rut}-${dv}`;
        },
        
        // Tarea para generar Stock válido
        generateValidStock() {
          const letters = 'ABCDEFGHIJKLMNOPQRSTUVWXYZ';
          const numbers = '0123456789';
          let stock = '';
          
          // Generar 3 letras
          for (let i = 0; i < 3; i++) {
            stock += letters.charAt(Math.floor(Math.random() * letters.length));
          }
          
          // Generar 3 números
          for (let i = 0; i < 3; i++) {
            stock += numbers.charAt(Math.floor(Math.random() * numbers.length));
          }
          
          return stock;
        },
        
        // Tarea para validar RUT
        validateRut(rut) {
          if (!rut || typeof rut !== 'string') return false;
          
          const parts = rut.split('-');
          if (parts.length !== 2) return false;
          
          const rutNumber = parseInt(parts[0]);
          const dv = parts[1].toUpperCase();
          
          if (isNaN(rutNumber)) return false;
          
          const calculatedDV = calculateDV(rutNumber);
          return dv === calculatedDV;
        },
        
        // Tarea para crear datos de vehículo de prueba
        createTestVehicle(vehicleData) {
          console.log('Creando vehículo de prueba:', vehicleData);
          // Implementar inserción en BD
          return {
            stock: vehicleData.stock,
            created: true,
            timestamp: new Date().toISOString()
          };
        },
        
        // Tarea para crear cliente de prueba
        createTestClient(clientData) {
          console.log('Creando cliente de prueba:', clientData);
          // Implementar inserción en BD
          return {
            rut: clientData.rut,
            created: true,
            timestamp: new Date().toISOString()
          };
        },
        
        // Tarea para simular respuesta SAP
        mockSAPResponse(stockNumber) {
          console.log('Simulando respuesta SAP para stock:', stockNumber);
          
          // Simular diferentes respuestas según el stock
          if (stockNumber.includes('ERROR')) {
            return {
              success: false,
              message: 'Stock no encontrado en SAP'
            };
          }
          
          return {
            success: true,
            fechaFactura: '2023-01-15',
            numeroFactura: '12345',
            cliente: 'CLIENTE_PRUEBA'
          };
        },
        
        // Tarea para logs personalizados
        log(message) {
          console.log(`[CYPRESS LOG] ${new Date().toISOString()}: ${message}`);
          return null;
        }
      });
      
      // Configurar interceptores globales
      on('before:browser:launch', (browser = {}, launchOptions) => {
        if (browser.name === 'chrome') {
          launchOptions.args.push('--disable-dev-shm-usage');
          launchOptions.args.push('--no-sandbox');
          launchOptions.args.push('--disable-gpu');
        }
        
        return launchOptions;
      });
      
      return config;
    },
  },
  
  component: {
    devServer: {
      framework: 'create-react-app',
      bundler: 'webpack',
    },
  },
})

// Función auxiliar para calcular dígito verificador de RUT chileno
function calculateDV(rut) {
  let contador = 2;
  let acumulador = 0;
  let rutTemp = rut;
  
  while (rutTemp !== 0) {
    const multiplo = (rutTemp % 10) * contador;
    acumulador += multiplo;
    rutTemp = Math.floor(rutTemp / 10);
    contador++;
    if (contador === 8) contador = 2;
  }
  
  const digito = 11 - (acumulador % 11);
  if (digito === 10) return 'K';
  if (digito === 11) return '0';
  return digito.toString();
}
