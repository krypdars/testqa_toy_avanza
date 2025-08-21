using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Support.Extensions;

namespace RegistroMantenciones.AutomationTests
{
    /// <summary>
    /// Pruebas automatizadas con Selenium para el sistema de Registro de Mantenciones
    /// </summary>
    [TestClass]
    public class RegistroMantencionesSeleniumTests
    {
        private IWebDriver driver;
        private WebDriverWait wait;
        private string baseUrl = "http://localhost/RegistroMantenciones"; // Ajustar según entorno

        [TestInitialize]
        public void SetUp()
        {
            // Configurar Chrome driver
            var options = new ChromeOptions();
            options.AddArgument("--headless"); // Ejecutar sin interfaz gráfica para CI/CD
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-dev-shm-usage");
            
            driver = new ChromeDriver(options);
            driver.Manage().Window.Maximize();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
        }

        [TestCleanup]
        public void TearDown()
        {
            driver?.Quit();
        }

        /// <summary>
        /// Método auxiliar para realizar login
        /// </summary>
        private void RealizarLogin(string usuario = "admin_toyota", string password = "password123")
        {
            driver.Navigate().GoToUrl($"{baseUrl}/default.aspx");
            
            var txtUsuario = wait.Until(d => d.FindElement(By.Id("txtUsuario")));
            var txtPassword = driver.FindElement(By.Id("txtPassword"));
            var btnLogin = driver.FindElement(By.Id("btnLogin"));

            txtUsuario.Clear();
            txtUsuario.SendKeys(usuario);
            
            txtPassword.Clear();
            txtPassword.SendKeys(password);
            
            btnLogin.Click();

            // Verificar que el login fue exitoso
            wait.Until(d => d.Url.Contains("Default.aspx") || d.Url.Contains("Menu"));
        }

        [TestMethod]
        [TestCategory("Selenium_Login")]
        public void Test_Login_Exitoso()
        {
            // Act
            RealizarLogin();

            // Assert
            Assert.IsFalse(driver.Url.Contains("login"), "Debería haber salido de la página de login");
            
            // Verificar que hay elementos del menú principal
            var menuElements = driver.FindElements(By.TagName("a"));
            Assert.IsTrue(menuElements.Count > 0, "Debería mostrar elementos del menú");
        }

        [TestMethod]
        [TestCategory("Selenium_Login")]
        public void Test_Login_Credenciales_Invalidas()
        {
            // Arrange
            driver.Navigate().GoToUrl($"{baseUrl}/default.aspx");

            // Act
            var txtUsuario = wait.Until(d => d.FindElement(By.Id("txtUsuario")));
            var txtPassword = driver.FindElement(By.Id("txtPassword"));
            var btnLogin = driver.FindElement(By.Id("btnLogin"));

            txtUsuario.SendKeys("usuario_invalido");
            txtPassword.SendKeys("password_invalido");
            btnLogin.Click();

            // Assert
            Thread.Sleep(2000); // Esperar respuesta del servidor
            
            // Verificar que permanece en login o muestra error
            Assert.IsTrue(driver.Url.Contains("default.aspx") || 
                         driver.PageSource.Contains("error") || 
                         driver.PageSource.Contains("inválido"),
                         "Debería mostrar error o permanecer en login");
        }

        [TestMethod]
        [TestCategory("Selenium_Flujo_Completo")]
        public void Test_Flujo_Completo_Registro_Exitoso()
        {
            // Arrange
            RealizarLogin();

            // Act & Assert - Página 1
            driver.Navigate().GoToUrl($"{baseUrl}/Registro_Mantenciones/Reg_Mantencion_01.aspx");
            
            // Verificar que llegamos a la página 1
            wait.Until(d => d.FindElement(By.Id("TxtRut")));
            
            var txtRut = driver.FindElement(By.Id("TxtRut"));
            var txtStock = driver.FindElement(By.Id("TxtStock"));
            var btnContinuar = driver.FindElement(By.Id("Button1"));

            // Ingresar datos válidos
            txtRut.Clear();
            txtRut.SendKeys("12345678-5");
            
            txtStock.Clear();
            txtStock.SendKeys("ABC123");
            
            btnContinuar.Click();

            // Verificar transición a página 2
            wait.Until(d => d.Url.Contains("Reg_Mantencion_02.aspx"));
            Assert.IsTrue(driver.Url.Contains("Reg_Mantencion_02.aspx"), "Debería estar en página 2");

            // Act & Assert - Página 2
            var cmbMantencion = wait.Until(d => d.FindElement(By.Id("Mantencion")));
            var txtKM = driver.FindElement(By.Id("TxtKM"));
            var btnContinuar2 = driver.FindElement(By.Id("Button1"));

            // Seleccionar mantención
            var selectMantencion = new SelectElement(cmbMantencion);
            selectMantencion.SelectByValue("10"); // 10000 Kms

            // Ingresar kilometraje
            txtKM.Clear();
            txtKM.SendKeys("9500");

            btnContinuar2.Click();

            // Verificar transición a página 3
            wait.Until(d => d.Url.Contains("Reg_Mantencion_03.aspx"));
            Assert.IsTrue(driver.Url.Contains("Reg_Mantencion_03.aspx"), "Debería estar en página 3");

            // Act & Assert - Página 3 (Confirmación)
            var btnConfirmar = wait.Until(d => d.FindElement(By.Id("Button1")));
            
            // Verificar que los datos se muestran correctamente
            Assert.IsTrue(driver.PageSource.Contains("12345678-5"), "Debería mostrar el RUT ingresado");
            Assert.IsTrue(driver.PageSource.Contains("ABC123"), "Debería mostrar el Stock ingresado");
            
            btnConfirmar.Click();

            // Verificar transición a página 4
            wait.Until(d => d.Url.Contains("Reg_Mantencion_04.aspx"));
            Assert.IsTrue(driver.Url.Contains("Reg_Mantencion_04.aspx"), "Debería estar en página 4");

            // Verificar mensaje de éxito
            Assert.IsTrue(driver.PageSource.Contains("exitoso") || 
                         driver.PageSource.Contains("completado") ||
                         driver.PageSource.Contains("registrado"),
                         "Debería mostrar mensaje de éxito");
        }

        [TestMethod]
        [TestCategory("Selenium_Validaciones")]
        public void Test_Validacion_RUT_Formato_Incorrecto()
        {
            // Arrange
            RealizarLogin();
            driver.Navigate().GoToUrl($"{baseUrl}/Registro_Mantenciones/Reg_Mantencion_01.aspx");

            // Act
            var txtRut = wait.Until(d => d.FindElement(By.Id("TxtRut")));
            var txtStock = driver.FindElement(By.Id("TxtStock"));
            var btnContinuar = driver.FindElement(By.Id("Button1"));

            txtRut.Clear();
            txtRut.SendKeys("123456789"); // Formato incorrecto
            
            txtStock.Clear();
            txtStock.SendKeys("ABC123");
            
            btnContinuar.Click();

            // Assert
            Thread.Sleep(2000); // Esperar validación
            
            // Verificar que muestra error de validación
            var validationSummary = driver.FindElements(By.Id("valSumario"));
            if (validationSummary.Count > 0)
            {
                Assert.IsTrue(validationSummary[0].Displayed, "Debería mostrar resumen de validación");
            }

            // Verificar que no avanza a la siguiente página
            Assert.IsTrue(driver.Url.Contains("Reg_Mantencion_01.aspx"), "Debería permanecer en página 1");
        }

        [TestMethod]
        [TestCategory("Selenium_Validaciones")]
        public void Test_Validacion_Stock_Inexistente()
        {
            // Arrange
            RealizarLogin();
            driver.Navigate().GoToUrl($"{baseUrl}/Registro_Mantenciones/Reg_Mantencion_01.aspx");

            // Act
            var txtRut = wait.Until(d => d.FindElement(By.Id("TxtRut")));
            var txtStock = driver.FindElement(By.Id("TxtStock"));
            var btnContinuar = driver.FindElement(By.Id("Button1"));

            txtRut.Clear();
            txtRut.SendKeys("12345678-5");
            
            txtStock.Clear();
            txtStock.SendKeys("STOCKINEXISTENTE");
            
            btnContinuar.Click();

            // Assert
            Thread.Sleep(3000); // Esperar validación de BD
            
            // Verificar mensaje de error
            Assert.IsTrue(driver.PageSource.Contains("no Existe") || 
                         driver.PageSource.Contains("no Corresponde"),
                         "Debería mostrar mensaje de stock inexistente");

            // Verificar que no avanza
            Assert.IsTrue(driver.Url.Contains("Reg_Mantencion_01.aspx"), "Debería permanecer en página 1");
        }

        [TestMethod]
        [TestCategory("Selenium_Validaciones")]
        public void Test_Validacion_Kilometraje_Fuera_Rango()
        {
            // Arrange - Completar página 1 primero
            RealizarLogin();
            CompletarPagina1Exitosamente();

            // Act
            var cmbMantencion = wait.Until(d => d.FindElement(By.Id("Mantencion")));
            var txtKM = driver.FindElement(By.Id("TxtKM"));
            var btnContinuar = driver.FindElement(By.Id("Button1"));

            // Seleccionar mantención de 10000 km
            var selectMantencion = new SelectElement(cmbMantencion);
            selectMantencion.SelectByValue("10");

            // Ingresar kilometraje muy fuera de rango
            txtKM.Clear();
            txtKM.SendKeys("25000");

            btnContinuar.Click();

            // Assert
            Thread.Sleep(2000);
            
            // Debería aparecer confirmación JavaScript
            try
            {
                var alert = driver.SwitchTo().Alert();
                Assert.IsTrue(alert.Text.Contains("no coincide"), "Debería mostrar alerta de confirmación");
                alert.Dismiss(); // Cancelar para probar que no avanza
            }
            catch (NoAlertPresentException)
            {
                // Si no hay alerta, verificar que hay mensaje en la página
                Assert.IsTrue(driver.PageSource.Contains("coincide") || 
                             driver.PageSource.Contains("rango"),
                             "Debería mostrar mensaje de kilometraje fuera de rango");
            }
        }

        [TestMethod]
        [TestCategory("Selenium_Archivos")]
        public void Test_Carga_Archivo_PDF_Valido()
        {
            // Arrange
            RealizarLogin();
            CompletarPagina1Exitosamente();

            // Crear archivo PDF temporal para prueba
            string tempPdfPath = System.IO.Path.GetTempFileName() + ".pdf";
            System.IO.File.WriteAllText(tempPdfPath, "%PDF-1.4 dummy content"); // PDF mínimo

            try
            {
                // Act
                var fileUpload = wait.Until(d => d.FindElement(By.Id("FileUpload1")));
                
                if (fileUpload.Displayed && fileUpload.Enabled)
                {
                    fileUpload.SendKeys(tempPdfPath);
                    
                    Thread.Sleep(1000); // Esperar procesamiento
                    
                    // Assert
                    var btnContinuar = driver.FindElement(By.Id("Button1"));
                    Assert.IsTrue(btnContinuar.Enabled, "Botón continuar debería habilitarse con PDF válido");
                }
            }
            finally
            {
                // Cleanup
                if (System.IO.File.Exists(tempPdfPath))
                    System.IO.File.Delete(tempPdfPath);
            }
        }

        [TestMethod]
        [TestCategory("Selenium_Navegacion")]
        public void Test_Navegacion_Volver_Pagina_Anterior()
        {
            // Arrange
            RealizarLogin();
            CompletarPagina1Exitosamente();

            // Verificar que estamos en página 2
            Assert.IsTrue(driver.Url.Contains("Reg_Mantencion_02.aspx"));

            // Act
            var btnVolver = driver.FindElement(By.Id("B_Volver"));
            btnVolver.Click();

            // Assert
            wait.Until(d => d.Url.Contains("Reg_Mantencion_01.aspx"));
            Assert.IsTrue(driver.Url.Contains("Reg_Mantencion_01.aspx"), "Debería volver a página 1");

            // Verificar que los datos se mantienen
            var txtRut = driver.FindElement(By.Id("TxtRut"));
            var txtStock = driver.FindElement(By.Id("TxtStock"));
            
            Assert.AreEqual("12345678-5", txtRut.GetAttribute("value"), "RUT debería mantenerse");
            Assert.AreEqual("ABC123", txtStock.GetAttribute("value"), "Stock debería mantenerse");
        }

        [TestMethod]
        [TestCategory("Selenium_Sesion")]
        public void Test_Acceso_Sin_Autenticacion()
        {
            // Act - Intentar acceder directamente sin login
            driver.Navigate().GoToUrl($"{baseUrl}/Registro_Mantenciones/Reg_Mantencion_01.aspx");

            // Assert
            wait.Until(d => d.Url.Contains("default.aspx"));
            Assert.IsTrue(driver.Url.Contains("default.aspx"), "Debería redirigir al login");
        }

        [TestMethod]
        [TestCategory("Selenium_Rendimiento")]
        public void Test_Tiempo_Respuesta_Validaciones()
        {
            // Arrange
            RealizarLogin();
            driver.Navigate().GoToUrl($"{baseUrl}/Registro_Mantenciones/Reg_Mantencion_01.aspx");

            var txtRut = wait.Until(d => d.FindElement(By.Id("TxtRut")));
            var txtStock = driver.FindElement(By.Id("TxtStock"));

            // Act & Assert - Medir tiempo de validación de RUT
            var startTime = DateTime.Now;
            
            txtRut.Clear();
            txtRut.SendKeys("12345678-5");
            txtRut.SendKeys(Keys.Tab); // Trigger blur event
            
            Thread.Sleep(500); // Esperar validación
            
            var rutValidationTime = DateTime.Now - startTime;
            Assert.IsTrue(rutValidationTime.TotalMilliseconds < 2000, 
                         $"Validación de RUT tomó {rutValidationTime.TotalMilliseconds}ms, debería ser < 2000ms");

            // Act & Assert - Medir tiempo de validación de Stock
            startTime = DateTime.Now;
            
            txtStock.Clear();
            txtStock.SendKeys("ABC123");
            txtStock.SendKeys(Keys.Tab); // Trigger blur event
            
            Thread.Sleep(2000); // Esperar validación de BD
            
            var stockValidationTime = DateTime.Now - startTime;
            Assert.IsTrue(stockValidationTime.TotalMilliseconds < 5000, 
                         $"Validación de Stock tomó {stockValidationTime.TotalMilliseconds}ms, debería ser < 5000ms");
        }

        /// <summary>
        /// Método auxiliar para completar página 1 exitosamente
        /// </summary>
        private void CompletarPagina1Exitosamente()
        {
            driver.Navigate().GoToUrl($"{baseUrl}/Registro_Mantenciones/Reg_Mantencion_01.aspx");
            
            var txtRut = wait.Until(d => d.FindElement(By.Id("TxtRut")));
            var txtStock = driver.FindElement(By.Id("TxtStock"));
            var btnContinuar = driver.FindElement(By.Id("Button1"));

            txtRut.Clear();
            txtRut.SendKeys("12345678-5");
            
            txtStock.Clear();
            txtStock.SendKeys("ABC123");
            
            btnContinuar.Click();

            wait.Until(d => d.Url.Contains("Reg_Mantencion_02.aspx"));
        }

        /// <summary>
        /// Método auxiliar para tomar screenshot en caso de error
        /// </summary>
        private void TakeScreenshot(string testName)
        {
            try
            {
                var screenshot = ((ITakesScreenshot)driver).GetScreenshot();
                var fileName = $"screenshot_{testName}_{DateTime.Now:yyyyMMdd_HHmmss}.png";
                screenshot.SaveAsFile(fileName, ScreenshotImageFormat.Png);
                Console.WriteLine($"Screenshot guardado: {fileName}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al tomar screenshot: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// Pruebas de carga y estrés usando Selenium Grid
    /// </summary>
    [TestClass]
    public class PruebasCargaSelenium
    {
        [TestMethod]
        [TestCategory("Selenium_Carga")]
        public void Test_Carga_Multiples_Usuarios()
        {
            const int numeroUsuarios = 5; // Reducido para pruebas locales
            var tasks = new System.Threading.Tasks.Task[numeroUsuarios];

            for (int i = 0; i < numeroUsuarios; i++)
            {
                int userId = i;
                tasks[i] = System.Threading.Tasks.Task.Run(() => SimularUsuario(userId));
            }

            // Esperar que todos los usuarios completen
            System.Threading.Tasks.Task.WaitAll(tasks, TimeSpan.FromMinutes(5));

            // Verificar que todos completaron exitosamente
            foreach (var task in tasks)
            {
                Assert.IsTrue(task.IsCompletedSuccessfully, "Todos los usuarios deberían completar exitosamente");
            }
        }

        private void SimularUsuario(int userId)
        {
            IWebDriver driver = null;
            try
            {
                var options = new ChromeOptions();
                options.AddArgument("--headless");
                options.AddArgument($"--user-data-dir=/tmp/chrome_user_{userId}");
                
                driver = new ChromeDriver(options);
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));

                // Simular flujo completo de usuario
                var test = new RegistroMantencionesSeleniumTests();
                
                // Usar reflection para acceder a métodos privados si es necesario
                var loginMethod = typeof(RegistroMantencionesSeleniumTests)
                    .GetMethod("RealizarLogin", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                
                if (loginMethod != null)
                {
                    loginMethod.Invoke(test, new object[] { $"user_{userId}", "password123" });
                }

                // Completar registro
                driver.Navigate().GoToUrl("http://localhost/RegistroMantenciones/Registro_Mantenciones/Reg_Mantencion_01.aspx");
                
                var txtRut = wait.Until(d => d.FindElement(By.Id("TxtRut")));
                txtRut.SendKeys($"1234567{userId}-5");
                
                var txtStock = driver.FindElement(By.Id("TxtStock"));
                txtStock.SendKeys($"ABC12{userId}");
                
                driver.FindElement(By.Id("Button1")).Click();

                Thread.Sleep(2000); // Simular tiempo de usuario
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en usuario {userId}: {ex.Message}");
                throw;
            }
            finally
            {
                driver?.Quit();
            }
        }
    }

    /// <summary>
    /// Configuración para ejecución en CI/CD
    /// </summary>
    [TestClass]
    public class ConfiguracionCI
    {
        [AssemblyInitialize]
        public static void AssemblyInit(TestContext context)
        {
            // Configuración para CI/CD
            if (Environment.GetEnvironmentVariable("CI") == "true")
            {
                // Configurar para ejecución en servidor de CI
                Console.WriteLine("Ejecutando en entorno CI/CD");
                
                // Verificar que Chrome está disponible
                try
                {
                    using (var driver = new ChromeDriver(new ChromeOptions { BinaryLocation = "/usr/bin/google-chrome" }))
                    {
                        Console.WriteLine("Chrome driver configurado correctamente");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error configurando Chrome: {ex.Message}");
                    throw;
                }
            }
        }

        [AssemblyCleanup]
        public static void AssemblyCleanup()
        {
            // Limpieza después de todas las pruebas
            Console.WriteLine("Limpieza de pruebas completada");
        }
    }
}

/// <summary>
/// Clase de utilidades para las pruebas Selenium
/// </summary>
public static class SeleniumUtilities
{
    public static void WaitForPageLoad(IWebDriver driver, int timeoutSeconds = 30)
    {
        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutSeconds));
        wait.Until(d => ((IJavaScriptExecutor)d).ExecuteScript("return document.readyState").Equals("complete"));
    }

    public static void WaitForAjax(IWebDriver driver, int timeoutSeconds = 30)
    {
        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutSeconds));
        wait.Until(d => (bool)((IJavaScriptExecutor)d).ExecuteScript("return jQuery.active == 0"));
    }

    public static void ScrollToElement(IWebDriver driver, IWebElement element)
    {
        ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", element);
        Thread.Sleep(500); // Esperar que termine el scroll
    }

    public static void HighlightElement(IWebDriver driver, IWebElement element)
    {
        var jsExecutor = (IJavaScriptExecutor)driver;
        jsExecutor.ExecuteScript("arguments[0].style.border='3px solid red'", element);
    }

    public static void ClearHighlight(IWebDriver driver, IWebElement element)
    {
        var jsExecutor = (IJavaScriptExecutor)driver;
        jsExecutor.ExecuteScript("arguments[0].style.border=''", element);
    }
}
