using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Data.SqlClient;
using System.Net;
using System.Xml;
using System.IO;
public partial class Registro_Mantenciones_Reg_Mantencion_01 : System.Web.UI.Page
{
    string ipCliente;
    protected void Page_Load(object sender, EventArgs e)
    {
        /*  control autentificacion */
        if (Session["usuario"] == null) Response.Redirect("../default.aspx");
        /**/
        if (Request.QueryString["Cliente_Organizacion"] != null)
        {

            Session["RPCliente_Organizacion"] = Convert.ToString(Request.QueryString["Cliente_Organizacion"]);
            Session["RPCliente_Sector"] = Convert.ToString(Request.QueryString["Cliente_Sector"]);
            Session["RPCanal_Distribucion"] = Convert.ToString(Request.QueryString["Canal_Distribucion"]);
            ipCliente = Request.ServerVariables["REMOTE_ADDR"];
            Session["Ud_Esta"] = "Ud. Esta en : " + Convert.ToString(Session["RPCliente_Organizacion"]) + ".";
        }
        Label4.Text = Convert.ToString(Session["Ud_Esta"]);
        TxtRut.Focus();
        if (Request.QueryString["rut"] != null && TxtRut.Text.Trim().Length == 0)
        {
            TxtRut.Text = Request.QueryString["rut"] + "-" + Request.QueryString["dv"];
            TxtStock.Text = Request.QueryString["stock"];
        }
    }
    protected void ValidaRut2(object source, ServerValidateEventArgs args)
    {
        if (Convert.ToString(args.Value).Trim() != "")
        {
            string[] partes = args.Value.Trim().Split('-');

            Int32 rut = Convert.ToInt32(partes[0]);
            string dv = Convert.ToString(partes[1]);

            int Digito;
            int Contador;
            int Multiplo;
            int Acumulador;
            string RutDigito;
            Contador = 2;
            Acumulador = 0;
            while (rut != 0)
            {
                Multiplo = (rut % 10) * Contador;
                Acumulador = Acumulador + Multiplo;
                rut = rut / 10;
                Contador = Contador + 1;
                if (Contador == 8)
                {
                    Contador = 2;
                }

            }
            Digito = 11 - (Acumulador % 11);
            RutDigito = Digito.ToString().Trim();
            if (Digito == 10)
            { RutDigito = "K"; }
            if (Digito == 11)
            { RutDigito = "0"; }

            if (dv == RutDigito)
            {
                args.IsValid = true;
            }
            else
            {
                args.IsValid = false;
            }
            //
            try
            {
                if (Convert.ToInt32(partes[0]) == 0 && Convert.ToInt16(dv) == 0) args.IsValid = false;

                //double RutNoNumeroRepetido = Convert.ToInt32(partes[0]) % Convert.ToInt16(dv);
                Int32 RutNoNumeroRepetido = 0;
                string car1 = partes[0].Substring(0, 1);

                for (Int32 i = 0; i <= partes[0].Length - 1; i++)
                {
                    string carComp = partes[0].Substring(i, 1);
                    if (car1 == carComp)
                    {
                        RutNoNumeroRepetido++;
                    }

                }

                if (RutNoNumeroRepetido == partes[0].Length) args.IsValid = false;

            }
            catch (Exception ex)
            {

            }
            //
        }

    }
    protected void Valida_Stock2(object source, ServerValidateEventArgs args)
    {
        if (Convert.ToString(args.Value).Trim() != "" && MiValidadorRUT.IsValid)
        {
            SqlConnection conexion1 = new SqlConnection(ConfigurationManager.ConnectionStrings["SAP_WEBConnectionString"].ConnectionString);

            try
            {
                SqlCommand selQRY = new SqlCommand();
                selQRY.Connection = conexion1;
                selQRY.CommandText = "Sp_Lst_Vehiculo_Only_One";
                selQRY.CommandType = CommandType.StoredProcedure;
                selQRY.Parameters.Add("@stock", SqlDbType.NVarChar).Value = Convert.ToString(args.Value).Trim();
                SqlDataAdapter adaptador1 = new SqlDataAdapter(selQRY);
                DataTable table = new DataTable();
                table = new DataTable("myVeh");
                adaptador1.Fill(table);
                if (table.Rows.Count == 0)
                {
                    args.IsValid = false;
                    Session["Session_Stock"] = "";
                    Session["Session_Linea"] = "";
                    Session["Session_Modelo"] = "";
                    Session["Session_Vin"] = "";
                    Session["Session_Version"] = "";
                    Session["Session_Motor"] = "";
                    Session["Session_Color"] = "";
                    Session["MantenAsumar"] = "";
                    ValidaStockReg.ErrorMessage = "N° Stock no Existe o no Corresponde a la organización";
                }
                else
                {
                    args.IsValid = true;
                    conexion1 = new SqlConnection(ConfigurationManager.ConnectionStrings["SAP_WEBConnectionString"].ConnectionString);
                    selQRY = new SqlCommand();
                    selQRY.Connection = conexion1;
                    selQRY.CommandText = "Sp_Lst_Vehiculo_Reg_Garantia_One";
                    selQRY.CommandType = CommandType.StoredProcedure;
                    selQRY.Parameters.Add("@stock", SqlDbType.NVarChar).Value = Convert.ToString(args.Value).Trim();
                    adaptador1 = new SqlDataAdapter(selQRY);
                    DataTable table2 = new DataTable("myVeh");
                    adaptador1.Fill(table2);
                    DateTime fecventa = DateTime.Now;
                    #region valida registro venta
                    if (table2.Rows.Count == 0)
                    {
                        Boolean VentaSap = busca_Garantia(Convert.ToString(args.Value).Trim());
                        if (VentaSap == false)
                        {
                            Session["Mensaje_SinRegistroVenta"] = "Observación: N° Stock no registra ingreso de venta.";
                            Session["Mensaje_SinRegistroVenta"] = "";
                            Session["Mensaje_SinRegistroVenta"] = "";
                            Session["Rut_Cliente_Final"] = "";
                            Session["Dv_Cliente_Final"] = "";
                            fecventa = DateTime.Now;
                            //     args.IsValid = false;
                            string texto = "";
                            texto = "N° Stock no registra ingreso de venta.";
                            ValidaStockReg.ErrorMessage = texto;
                        }
                    }
                    else
                    {
                        Session["Mensaje_SinRegistroVenta"] = "";
                        Session["Rut_Cliente_Final"] = Convert.ToString(table2.Rows[0]["CustRut"]);
                        Session["Dv_Cliente_Final"] = Convert.ToString(table2.Rows[0]["CustDV"]);
                        Session["Session_Patente"] = Convert.ToString(table2.Rows[0]["Patente"]);
                        fecventa = Convert.ToDateTime(table2.Rows[0]["garfecent"]);
                    }
                    #endregion
                    Session["Session_Stock"] = Convert.ToString(args.Value).Trim();
                    Session["Session_Marca"] = Convert.ToString(table.Rows[0]["Marca"]);
                    Session["Session_Linea"] = Convert.ToString(table.Rows[0]["Codigo_Linea"]);
                    Session["Session_Modelo"] = Convert.ToString(table.Rows[0]["Modelo"]);
                    Session["Session_Vin"] = Convert.ToString(table.Rows[0]["Numero_Vin"]);
                    Session["Session_Motor"] = Convert.ToString(table.Rows[0]["Motor"]);
                    Session["Session_Color"] = Convert.ToString(table.Rows[0]["Color_Desc"]);

                    Session["Session_Version"] = Convert.ToString(table.Rows[0]["codigo_version"]).Trim();
                    Session["Session_Combustible"] = Convert.ToString(table.Rows[0]["Combustible"]);
                    if (Session["Session_Combustible"] == null)
                    {
                        Session["Session_Combustible"] = "";
                    }
                    //
                    string m = Convert.ToString(table.Rows[0]["Marca"]);
                    string c = Convert.ToString(Session["Session_Combustible"]).Trim().ToUpper();
                    //si marca en TOY y Bencina se suma 10 sino siempre 5
                    if ((Convert.ToString(Session["Session_Combustible"]).Trim().ToUpper() == "BENCINA" || Convert.ToString(Session["Session_Combustible"]).Trim() == "Híbrido") && Convert.ToString(Session["Session_Marca"]).Trim().ToUpper() == "TOY")
                    {
                        Session["MantenAsumar"] = "10";
                    }
                    else
                    {
                        Session["MantenAsumar"] = "5";
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Write("<font color=red size=3>Favor enviar este error a vuestro supervisor.<br> <b>" + Page.Page.AppRelativeVirtualPath + ex.Message + "</b></font>");
                Response.End();
            }
            finally
            {
                if (conexion1 != null)
                    conexion1.Close();
            }
        }
    }

    protected void Button1_Click1(object sender, EventArgs e)
    {
        if (ValidaStockReg.IsValid && MiValidadorRUT.IsValid && MiValidadorRUT2.IsValid)
        {
            //rut del ingreso
            string[] partes = TxtRut.Text.Trim().Split('-');
            Int32 rut = Convert.ToInt32(partes[0]);
            string dv = Convert.ToString(partes[1]);
            Session["Rut_Cliente_Final"] = Convert.ToString(rut);
            Session["Dv_Cliente_Final"] = Convert.ToString(dv);
            Session["Session_Inicio"] = TxtInicio.Text;

            Response.Redirect("~/Registro_Mantenciones/Reg_Mantencion_02.aspx");
        }
    }
    public bool busca_Garantia(string _stock)
    {
        bool retorno = false;
        string mensaje = InvokeService(_stock);
        if (mensaje == "Ok")
            retorno = true;

        return retorno;
    }
    public string InvokeService(string _stock)
    {
        string retorno = "";
        try
        {
            //Calling CreateSOAPWebRequest method    
            HttpWebRequest request = CreateSOAPWebRequest();

            XmlDocument SOAPReqBody = new XmlDocument();
            string Soap = "<?xml version='1.0' encoding='utf-8'?> " +
            "<soapenv:Envelope xmlns:soapenv='http://schemas.xmlsoap.org/soap/envelope/' xmlns:un='un:vision:sap_sender:sync:SD_ZSD0011'>" +
            "   <soapenv:Header/>" +
            "   <soapenv:Body>" +
            "      <un:VehiculoRequest>" +
            "         <NumeroStock>" + _stock + "</NumeroStock>" +
            "      </un:VehiculoRequest>" +
            "   </soapenv:Body>" +
            "</soapenv:Envelope>";

            //SOAP Body Request
            SOAPReqBody.LoadXml(@Soap);

            using (Stream stream = request.GetRequestStream())
            {
                SOAPReqBody.Save(stream);
            }
            //Geting response from request    
            using (WebResponse Serviceres = request.GetResponse())
            {
                using (StreamReader rd = new StreamReader
                    (Serviceres.GetResponseStream()))
                {
                    //reading stream    
                    string ServiceResult = rd.ReadToEnd();
                    retorno = LeerXML(ServiceResult);
                    //Label2.Text = mensaje;
                }
            }
        }
        catch (WebException ex)
        {
        }
        return retorno;
    }
    public HttpWebRequest CreateSOAPWebRequest()
    {
        string Url;
        Url = "http://192.168.55.12:50000/XISOAPAdapter/MessageServlet?senderParty=&senderService=SOAP_Sender&receiverParty=&receiverService=&interface=RegistroVentasSolicitar_Out&interfaceNamespace=un:vision:sap_sender:sync:SD_ZSD0011";
        HttpWebRequest Req = (HttpWebRequest)WebRequest.Create(Url);
        Req.Headers.Add(@"SOAPAction:http://tempuri.org/SGT_ActualizaCualificadores_Req_MT");
        ICredentials credentials = new NetworkCredential("RFCUSER", "tcl.rfc$prd");
        Req.Credentials = credentials;
        Req.ContentType = "text/xml;charset=\"utf-8\"";
        Req.Accept = "text/xml";
        Req.Method = "POST";
        return Req;
    }
    public string LeerXML(string _XML)
    {
        string mensaje = "";
        XmlDocument xmldoc = new XmlDocument();
        xmldoc.LoadXml(_XML);
        XmlNodeList nodeList;

        nodeList = xmldoc.GetElementsByTagName("mensaje");

        if (nodeList.Count > 0)
        {
            foreach (XmlNode node in nodeList)
            {
                mensaje = "NotOk";
            }
        }
        else if (nodeList.Count == 0)
        {
            nodeList = xmldoc.GetElementsByTagName("fechaFactura");
            foreach (XmlNode node in nodeList)
            {
                mensaje = "Ok";
            }
        }
        return mensaje;
    }

    protected void inserta_prepago(string _stock, int IdTipoPrePago)
    {
        SqlConnection conSQL = new SqlConnection(ConfigurationManager.ConnectionStrings["SAP_WEBConnectionString"].ConnectionString);
        try
        {
            conSQL.Open();
            SqlCommand selQRY = new SqlCommand();
            selQRY.Connection = conSQL;
            selQRY.CommandText = "Sp_Lst_Toyota_Avanza_Registro_OT_MPP";
            selQRY.CommandType = CommandType.StoredProcedure;
            selQRY.Parameters.Add("@stock", SqlDbType.NVarChar).Value = _stock;
            int cant = Convert.ToInt32(selQRY.ExecuteScalar());
            if (cant == 0)
            {
                SqlCommand insQRY = new SqlCommand();
                insQRY.Connection = conSQL;
                insQRY.CommandText = "Sp_Ins_Mantencion_Prepago";
                insQRY.CommandType = CommandType.StoredProcedure;
                insQRY.Parameters.Add("@Numero_Stock", SqlDbType.NVarChar).Value = _stock;
                insQRY.Parameters.Add("@IdTipo_PrePago", SqlDbType.Int).Value = IdTipoPrePago;
                insQRY.Parameters.Add("@Prepago1", SqlDbType.Int).Value = 1;
                insQRY.Parameters.Add("@Prepago2", SqlDbType.Int).Value = 1;
                insQRY.Parameters.Add("@Prepago3", SqlDbType.Int).Value = 1;
                insQRY.Parameters.Add("@Prepago4", SqlDbType.Int).Value = 0;
                insQRY.Parameters.Add("@Prepago5", SqlDbType.Int).Value = 0;
                insQRY.Parameters.Add("@valor_prepago1", SqlDbType.Int).Value = 0;
                insQRY.Parameters.Add("@valor_prepago2", SqlDbType.Int).Value = 0;
                insQRY.Parameters.Add("@valor_prepago3", SqlDbType.Int).Value = 0;
                insQRY.Parameters.Add("@valor_prepago4", SqlDbType.Int).Value = 0;
                insQRY.Parameters.Add("@valor_prepago5", SqlDbType.Int).Value = 0;
                insQRY.ExecuteNonQuery();
            }
        }
        catch (Exception ex)
        {
            if (conSQL != null) conSQL.Close();
        }
        finally
        {
            if (conSQL != null) conSQL.Close();
        }
    }
    protected void TxtStock_TextChanged(object sender, EventArgs e)
    {

    }
    protected void ValidaRut3(object source, ServerValidateEventArgs args)
    {
        if (MiValidadorRUT.IsValid)
        {
            if (Convert.ToString(args.Value).Trim() != "")
            {
                string[] partes = args.Value.Trim().Split('-');

                Int32 rut = Convert.ToInt32(partes[0]);
                string dv = Convert.ToString(partes[1]);

                SqlConnection conexion1 = new SqlConnection(ConfigurationManager.ConnectionStrings["SAP_WEBConnectionString"].ConnectionString);
                try
                {
                    SqlCommand selQRY = new SqlCommand();
                    selQRY.Connection = conexion1;
                    selQRY.CommandText = "Sp_Lst_CLIENTE_Portal_One";
                    selQRY.CommandType = CommandType.StoredProcedure;
                    selQRY.Parameters.Add("@rut", SqlDbType.Int).Value = Convert.ToString(rut).Trim();
                    SqlDataAdapter adaptador1 = new SqlDataAdapter(selQRY);
                    DataTable table = new DataTable();
                    table = new DataTable("myVeh");
                    adaptador1.Fill(table);
                    if (table.Rows.Count == 0)
                    {
                        args.IsValid = false;
                        string texto = "";
                        texto = "R.U.T. Ingresado no figura como cliente. ";
                        texto = texto + "Para registrarlo presione <a href=../include/Ingresar_Clientes.aspx";
                        texto = texto + "?url_origen=Registro_Mantenciones/Reg_Mantencion_01.aspx";
                        texto = texto + "&rut=" + Convert.ToString(rut);
                        texto = texto + "&dv=" + Convert.ToString(dv);
                        texto = texto + "&stock=" + Convert.ToString(TxtStock.Text.Trim());
                        texto = texto + " target=_self>Aquí</a><br>";
                        MiValidadorRUT2.ErrorMessage = texto;
                    }
                    else
                    {
                        args.IsValid = true;
                    }
                }
                catch (Exception ex)
                {
                    Response.Write("<font color=red size=3>Favor enviar este error a vuestro supervisor.<br> <b>" + Page.Page.AppRelativeVirtualPath + ex.Message + "</b></font>");

                    Response.End();
                }
                finally
                {
                    if (conexion1 != null)
                        conexion1.Close();
                }
            }
        }
    }
}