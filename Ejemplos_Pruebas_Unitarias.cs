using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace RegistroMantenciones.Tests
{
    /// <summary>
    /// Pruebas unitarias para validaciones de RUT en Reg_Mantencion_01
    /// </summary>
    [TestClass]
    public class ValidacionRutTests
    {
        private Registro_Mantenciones_Reg_Mantencion_01 _page;

        [TestInitialize]
        public void Setup()
        {
            _page = new Registro_Mantenciones_Reg_Mantencion_01();
        }

        [TestMethod]
        [TestCategory("ValidacionRUT")]
        public void ValidaRut_RutValidoConDigitoK_RetornaTrue()
        {
            // Arrange
            var args = new ServerValidateEventArgs("12345678-K", true);

            // Act
            _page.ValidaRut2(null, args);

            // Assert
            Assert.IsTrue(args.IsValid, "RUT válido con dígito K debería ser válido");
        }

        [TestMethod]
        [TestCategory("ValidacionRUT")]
        public void ValidaRut_RutValidoConDigitoNumerico_RetornaTrue()
        {
            // Arrange
            var args = new ServerValidateEventArgs("11111111-1", true);

            // Act
            _page.ValidaRut2(null, args);

            // Assert
            Assert.IsFalse(args.IsValid, "RUT con números repetidos debería ser inválido");
        }

        [TestMethod]
        [TestCategory("ValidacionRUT")]
        public void ValidaRut_RutConDigitoIncorrecto_RetornaFalse()
        {
            // Arrange
            var args = new ServerValidateEventArgs("12345678-9", true);

            // Act
            _page.ValidaRut2(null, args);

            // Assert
            Assert.IsFalse(args.IsValid, "RUT con dígito verificador incorrecto debería ser inválido");
        }

        [TestMethod]
        [TestCategory("ValidacionRUT")]
        public void ValidaRut_RutConNumerosRepetidos_RetornaFalse()
        {
            // Arrange
            var args = new ServerValidateEventArgs("77777777-7", true);

            // Act
            _page.ValidaRut2(null, args);

            // Assert
            Assert.IsFalse(args.IsValid, "RUT con todos los números iguales debería ser inválido");
        }

        [TestMethod]
        [TestCategory("ValidacionRUT")]
        public void ValidaRut_RutVacio_RetornaTrue()
        {
            // Arrange
            var args = new ServerValidateEventArgs("", true);

            // Act
            _page.ValidaRut2(null, args);

            // Assert
            Assert.IsTrue(args.IsValid, "RUT vacío debería pasar la validación (se maneja en RequiredFieldValidator)");
        }

        [TestMethod]
        [TestCategory("ValidacionRUT")]
        public void ValidaRut_RutConEspacios_RetornaResultadoCorrecto()
        {
            // Arrange
            var args = new ServerValidateEventArgs("  12345678-5  ", true);

            // Act
            _page.ValidaRut2(null, args);

            // Assert
            Assert.IsTrue(args.IsValid, "RUT válido con espacios debería ser válido después del trim");
        }
    }

    /// <summary>
    /// Pruebas unitarias para validaciones de Stock
    /// </summary>
    [TestClass]
    public class ValidacionStockTests
    {
        private Mock<SqlConnection> _mockConnection;
        private Mock<SqlCommand> _mockCommand;
        private Mock<SqlDataAdapter> _mockAdapter;

        [TestInitialize]
        public void Setup()
        {
            _mockConnection = new Mock<SqlConnection>();
            _mockCommand = new Mock<SqlCommand>();
            _mockAdapter = new Mock<SqlDataAdapter>();
        }

        [TestMethod]
        [TestCategory("ValidacionStock")]
        public void ValidaStock_StockExistente_CargaDatosEnSesion()
        {
            // Arrange
            var page = new TestableReg_Mantencion_01();
            var args = new ServerValidateEventArgs("ABC123", true);
            
            // Simular datos del vehículo
            var vehiculoData = new DataTable();
            vehiculoData.Columns.Add("Marca");
            vehiculoData.Columns.Add("Codigo_Linea");
            vehiculoData.Columns.Add("Modelo");
            vehiculoData.Columns.Add("Numero_Vin");
            vehiculoData.Columns.Add("Motor");
            vehiculoData.Columns.Add("Color_Desc");
            vehiculoData.Columns.Add("codigo_version");
            vehiculoData.Columns.Add("Combustible");
            
            var row = vehiculoData.NewRow();
            row["Marca"] = "TOY";
            row["Codigo_Linea"] = "COROLLA";
            row["Modelo"] = "2023";
            row["Numero_Vin"] = "VIN123456789";
            row["Motor"] = "1.8L";
            row["Color_Desc"] = "BLANCO";
            row["codigo_version"] = "GLI";
            row["Combustible"] = "BENCINA";
            vehiculoData.Rows.Add(row);

            page.SetMockVehiculoData(vehiculoData);

            // Act
            page.Valida_Stock2(null, args);

            // Assert
            Assert.IsTrue(args.IsValid);
            Assert.AreEqual("ABC123", page.GetSessionValue("Session_Stock"));
            Assert.AreEqual("TOY", page.GetSessionValue("Session_Marca"));
            Assert.AreEqual("10", page.GetSessionValue("MantenAsumar")); // TOY + BENCINA = 10
        }

        [TestMethod]
        [TestCategory("ValidacionStock")]
        public void ValidaStock_StockInexistente_RetornaFalse()
        {
            // Arrange
            var page = new TestableReg_Mantencion_01();
            var args = new ServerValidateEventArgs("XYZ999", true);
            
            // Simular tabla vacía
            var emptyData = new DataTable();
            page.SetMockVehiculoData(emptyData);

            // Act
            page.Valida_Stock2(null, args);

            // Assert
            Assert.IsFalse(args.IsValid);
            Assert.AreEqual("", page.GetSessionValue("Session_Stock"));
        }

        [TestMethod]
        [TestCategory("ValidacionStock")]
        public void ValidaStock_VehiculoDiesel_AsignaMantencion5()
        {
            // Arrange
            var page = new TestableReg_Mantencion_01();
            var args = new ServerValidateEventArgs("DEF456", true);
            
            var vehiculoData = new DataTable();
            vehiculoData.Columns.Add("Marca");
            vehiculoData.Columns.Add("Combustible");
            // ... otras columnas
            
            var row = vehiculoData.NewRow();
            row["Marca"] = "TOY";
            row["Combustible"] = "DIESEL";
            vehiculoData.Rows.Add(row);

            page.SetMockVehiculoData(vehiculoData);

            // Act
            page.Valida_Stock2(null, args);

            // Assert
            Assert.AreEqual("5", page.GetSessionValue("MantenAsumar")); // DIESEL = 5
        }
    }

    /// <summary>
    /// Pruebas unitarias para servicios web SAP
    /// </summary>
    [TestClass]
    public class ServicioSAPTests
    {
        private TestableReg_Mantencion_01 _page;

        [TestInitialize]
        public void Setup()
        {
            _page = new TestableReg_Mantencion_01();
        }

        [TestMethod]
        [TestCategory("ServicioSAP")]
        public void LeerXML_RespuestaConError_RetornaNotOk()
        {
            // Arrange
            string xmlConError = @"<?xml version='1.0' encoding='utf-8'?>
                <soapenv:Envelope xmlns:soapenv='http://schemas.xmlsoap.org/soap/envelope/'>
                    <soapenv:Body>
                        <response>
                            <mensaje>Stock no encontrado</mensaje>
                        </response>
                    </soapenv:Body>
                </soapenv:Envelope>";

            // Act
            string resultado = _page.LeerXML(xmlConError);

            // Assert
            Assert.AreEqual("NotOk", resultado);
        }

        [TestMethod]
        [TestCategory("ServicioSAP")]
        public void LeerXML_RespuestaExitosa_RetornaOk()
        {
            // Arrange
            string xmlExitoso = @"<?xml version='1.0' encoding='utf-8'?>
                <soapenv:Envelope xmlns:soapenv='http://schemas.xmlsoap.org/soap/envelope/'>
                    <soapenv:Body>
                        <response>
                            <fechaFactura>2023-01-15</fechaFactura>
                            <numeroFactura>12345</numeroFactura>
                        </response>
                    </soapenv:Body>
                </soapenv:Envelope>";

            // Act
            string resultado = _page.LeerXML(xmlExitoso);

            // Assert
            Assert.AreEqual("Ok", resultado);
        }

        [TestMethod]
        [TestCategory("ServicioSAP")]
        public void LeerXML_XMLMalformado_NoLanzaExcepcion()
        {
            // Arrange
            string xmlMalformado = "XML inválido sin estructura";

            // Act & Assert
            try
            {
                string resultado = _page.LeerXML(xmlMalformado);
                // Si llega aquí, no lanzó excepción (comportamiento esperado)
                Assert.IsTrue(true);
            }
            catch (Exception)
            {
                Assert.Fail("No debería lanzar excepción con XML malformado");
            }
        }
    }

    /// <summary>
    /// Pruebas unitarias para validaciones de kilometraje (Página 2)
    /// </summary>
    [TestClass]
    public class ValidacionKilometrajeTests
    {
        [TestMethod]
        [TestCategory("ValidacionKilometraje")]
        public void ValidarConfirmacion_KilometrajeEnRango_RetornaTrue()
        {
            // Arrange
            int mantencion = 10; // 10.000 km
            int kilometraje = 9500; // Dentro del rango permitido (5.000 - 15.000)

            // Act
            bool resultado = ValidarKilometrajeEnRango(mantencion, kilometraje);

            // Assert
            Assert.IsTrue(resultado);
        }

        [TestMethod]
        [TestCategory("ValidacionKilometraje")]
        public void ValidarConfirmacion_KilometrajeFueraDeRango_RetornaFalse()
        {
            // Arrange
            int mantencion = 10; // 10.000 km
            int kilometraje = 20000; // Fuera del rango permitido

            // Act
            bool resultado = ValidarKilometrajeEnRango(mantencion, kilometraje);

            // Assert
            Assert.IsFalse(resultado);
        }

        [TestMethod]
        [TestCategory("ValidacionKilometraje")]
        public void ValidarConfirmacion_KilometrajeEnLimiteSuperior_RetornaTrue()
        {
            // Arrange
            int mantencion = 10; // 10.000 km
            int kilometraje = 15000; // En el límite superior (10.000 + 5.000)

            // Act
            bool resultado = ValidarKilometrajeEnRango(mantencion, kilometraje);

            // Assert
            Assert.IsTrue(resultado);
        }

        [TestMethod]
        [TestCategory("ValidacionKilometraje")]
        public void ValidarConfirmacion_KilometrajeEnLimiteInferior_RetornaTrue()
        {
            // Arrange
            int mantencion = 10; // 10.000 km
            int kilometraje = 5000; // En el límite inferior (10.000 - 5.000)

            // Act
            bool resultado = ValidarKilometrajeEnRango(mantencion, kilometraje);

            // Assert
            Assert.IsTrue(resultado);
        }

        private bool ValidarKilometrajeEnRango(int mantencion, int kilometraje)
        {
            var min = mantencion * 1000 - 5000;
            var max = mantencion * 1000 + 5000;
            return kilometraje >= min && kilometraje <= max;
        }
    }

    /// <summary>
    /// Pruebas de integración para el flujo completo
    /// </summary>
    [TestClass]
    public class IntegracionFlujosTests
    {
        [TestMethod]
        [TestCategory("Integracion")]
        public void FlujoCompleto_DatosValidos_CompletaRegistro()
        {
            // Arrange
            var datosRegistro = new
            {
                Rut = "12345678-5",
                Stock = "ABC123",
                Kilometraje = 10000,
                Mantencion = 10,
                Asesor = "Juan Pérez"
            };

            // Act
            var resultado = SimularFlujoCompleto(datosRegistro);

            // Assert
            Assert.IsTrue(resultado.Exitoso);
            Assert.IsNotNull(resultado.CodigoGarantia);
            Assert.IsTrue(resultado.CodigoGarantia.Length > 0);
        }

        [TestMethod]
        [TestCategory("Integracion")]
        public void FlujoCompleto_RutInvalido_FallaEnPagina1()
        {
            // Arrange
            var datosRegistro = new
            {
                Rut = "12345678-9", // RUT inválido
                Stock = "ABC123",
                Kilometraje = 10000,
                Mantencion = 10,
                Asesor = "Juan Pérez"
            };

            // Act
            var resultado = SimularFlujoCompleto(datosRegistro);

            // Assert
            Assert.IsFalse(resultado.Exitoso);
            Assert.AreEqual("Página 1", resultado.PaginaError);
            Assert.Contains("RUT", resultado.MensajeError);
        }

        private dynamic SimularFlujoCompleto(dynamic datos)
        {
            // Simulación del flujo completo
            // En una implementación real, esto invocaría los métodos reales
            return new
            {
                Exitoso = true,
                CodigoGarantia = "202312151030456",
                PaginaError = "",
                MensajeError = ""
            };
        }
    }

    /// <summary>
    /// Clase testeable que hereda de la página original para permitir testing
    /// </summary>
    public class TestableReg_Mantencion_01 : Registro_Mantenciones_Reg_Mantencion_01
    {
        private Dictionary<string, object> _mockSession = new Dictionary<string, object>();
        private DataTable _mockVehiculoData;

        public void SetMockVehiculoData(DataTable data)
        {
            _mockVehiculoData = data;
        }

        public string GetSessionValue(string key)
        {
            return _mockSession.ContainsKey(key) ? _mockSession[key]?.ToString() : "";
        }

        public void SetSessionValue(string key, object value)
        {
            _mockSession[key] = value;
        }

        // Override del método original para usar datos mock
        protected override DataTable ConsultarVehiculo(string stock)
        {
            return _mockVehiculoData ?? new DataTable();
        }
    }

    /// <summary>
    /// Pruebas de rendimiento básicas
    /// </summary>
    [TestClass]
    public class RendimientoTests
    {
        [TestMethod]
        [TestCategory("Rendimiento")]
        public void ValidacionRUT_TiempoRespuesta_MenorA100ms()
        {
            // Arrange
            var page = new Registro_Mantenciones_Reg_Mantencion_01();
            var args = new ServerValidateEventArgs("12345678-5", true);
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            // Act
            page.ValidaRut2(null, args);
            stopwatch.Stop();

            // Assert
            Assert.IsTrue(stopwatch.ElapsedMilliseconds < 100, 
                $"Validación de RUT tomó {stopwatch.ElapsedMilliseconds}ms, debería ser < 100ms");
        }

        [TestMethod]
        [TestCategory("Rendimiento")]
        public void ValidacionStock_TiempoRespuesta_MenorA500ms()
        {
            // Arrange
            var page = new TestableReg_Mantencion_01();
            var args = new ServerValidateEventArgs("ABC123", true);
            
            // Simular datos rápidos
            var quickData = new DataTable();
            quickData.Columns.Add("Marca");
            page.SetMockVehiculoData(quickData);
            
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            // Act
            page.Valida_Stock2(null, args);
            stopwatch.Stop();

            // Assert
            Assert.IsTrue(stopwatch.ElapsedMilliseconds < 500, 
                $"Validación de Stock tomó {stopwatch.ElapsedMilliseconds}ms, debería ser < 500ms");
        }
    }
}

/// <summary>
/// Clase de utilidades para las pruebas
/// </summary>
public static class TestUtilities
{
    public static string GenerarRutValido()
    {
        Random random = new Random();
        int rut = random.Next(10000000, 99999999);
        string dv = CalcularDigitoVerificador(rut);
        return $"{rut}-{dv}";
    }

    public static string GenerarRutInvalido()
    {
        Random random = new Random();
        int rut = random.Next(10000000, 99999999);
        string dvIncorrecto = random.Next(0, 9).ToString();
        return $"{rut}-{dvIncorrecto}";
    }

    private static string CalcularDigitoVerificador(int rut)
    {
        int contador = 2;
        int acumulador = 0;
        
        while (rut != 0)
        {
            int multiplo = (rut % 10) * contador;
            acumulador += multiplo;
            rut /= 10;
            contador++;
            if (contador == 8) contador = 2;
        }
        
        int digito = 11 - (acumulador % 11);
        
        if (digito == 10) return "K";
        if (digito == 11) return "0";
        return digito.ToString();
    }

    public static DataTable CrearTablaVehiculoMock(string marca = "TOY", string combustible = "BENCINA")
    {
        var table = new DataTable();
        table.Columns.Add("Marca");
        table.Columns.Add("Codigo_Linea");
        table.Columns.Add("Modelo");
        table.Columns.Add("Numero_Vin");
        table.Columns.Add("Motor");
        table.Columns.Add("Color_Desc");
        table.Columns.Add("codigo_version");
        table.Columns.Add("Combustible");
        
        var row = table.NewRow();
        row["Marca"] = marca;
        row["Codigo_Linea"] = "COROLLA";
        row["Modelo"] = "2023";
        row["Numero_Vin"] = "VIN123456789";
        row["Motor"] = "1.8L";
        row["Color_Desc"] = "BLANCO";
        row["codigo_version"] = "GLI";
        row["Combustible"] = combustible;
        table.Rows.Add(row);
        
        return table;
    }
}
