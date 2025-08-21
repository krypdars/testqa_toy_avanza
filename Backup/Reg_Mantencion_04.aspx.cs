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

public partial class Registro_Mantenciones_Reg_Mantencion_04 : System.Web.UI.Page
{
    string ipCliente;
    protected void Page_Load(object sender, EventArgs e)
    {
        /*  control autentificacion */
        if (Session["usuario"] == null) Response.Redirect("../default.aspx");
        /**/
        #region Obtiene Codigo Garantia
        ipCliente = Request.ServerVariables["REMOTE_ADDR"];
        Label4.Text = Convert.ToString(Session["Ud_Esta"]);
        //traigo numero aleatorio
        string se = Convert.ToString(DateTime.Now.Second).PadLeft(2, '0');
        string mi = Convert.ToString(DateTime.Now.Minute).PadLeft(2, '0');
        string ho = Convert.ToString(DateTime.Now.Hour).PadLeft(2, '0');
        string di = Convert.ToString(DateTime.Now.Day).PadLeft(2, '0');
        string me = Convert.ToString(DateTime.Now.Month).PadLeft(2, '0');
        string an = Convert.ToString(DateTime.Now.Year).PadLeft(4, '0');

        string codigo = an + me + di + ho + mi + se;
        #endregion
        #region Graba Mantencion
        SqlConnection conexion1 = new SqlConnection(ConfigurationManager.ConnectionStrings["SAP_WEBConnectionString"].ConnectionString);
        try
        {
            SqlCommand insQRY = new SqlCommand();
            insQRY.Connection = conexion1;
            insQRY.CommandText = "Sp_Ins_Vehiculo_Mantencion";
            insQRY.CommandType = CommandType.StoredProcedure;
            insQRY.Parameters.Add("@GarNumero", SqlDbType.Int).Value = 0;
            insQRY.Parameters.Add("@VehSto", SqlDbType.NVarChar).Value = Convert.ToString(Session["Session_Stock"]);
            insQRY.Parameters.Add("@RevNumero", SqlDbType.Int).Value = Convert.ToInt32(Session["myMantencion"]);
            insQRY.Parameters.Add("@RevOrt", SqlDbType.Int).Value = Convert.ToInt32(Session["Session_Ort"]);
            insQRY.Parameters.Add("@RevKms", SqlDbType.Int).Value = Convert.ToInt32(Session["Session_KM"]);
            insQRY.Parameters.Add("@RevSat", SqlDbType.NVarChar).Value = Convert.ToString(Session["Cliente_Numero"]);
            insQRY.Parameters.Add("@CustRut", SqlDbType.Int).Value = Convert.ToInt32(Session["Rut_Cliente_Final"]);
            insQRY.Parameters.Add("@CustDV", SqlDbType.NVarChar).Value = Convert.ToString(Session["Dv_Cliente_Final"]);
            insQRY.Parameters.Add("@RevTip", SqlDbType.NVarChar).Value = Convert.ToString(Session["Session_Inicio"]).Trim();
            insQRY.Parameters.Add("@GarClave", SqlDbType.NVarChar).Value = Convert.ToString(codigo);
            insQRY.Parameters.Add("@Usuario", SqlDbType.NVarChar).Value = Convert.ToString(Session["usuario"]);
            insQRY.Parameters.Add("@interno", SqlDbType.Int).Value = Convert.ToInt32(Session["Session_Interno"]);
            insQRY.Parameters.Add("@ip", SqlDbType.NVarChar).Value = Convert.ToString(ipCliente);
            insQRY.Parameters.Add("@RevFec", SqlDbType.NVarChar).Value = Convert.ToString(Session["Session_FecMantecion"]);
            insQRY.Parameters.Add("@cita", SqlDbType.NVarChar).Value = Convert.ToString(Session["Cita"]);
            insQRY.Parameters.Add("@Retiro_domicilio", SqlDbType.Bit).Value = Convert.ToByte(Session["Session_Retiro"]);
            insQRY.Parameters.Add("@Entrega_Domicilio", SqlDbType.Bit).Value = Convert.ToByte(Session["Session_Entrega"]);
            insQRY.Parameters.Add("@Proveedor_Servicio_Domicilio", SqlDbType.NVarChar).Value = Convert.ToString(Session["Session_ProveedorServicio"]);
            insQRY.Parameters.Add("@Taller_Movil", SqlDbType.NVarChar).Value = Convert.ToString(Session["Taller_Movil"]);
            insQRY.Parameters.Add("@idasesor", SqlDbType.Int).Value = Convert.ToInt32(Session["AsesorID"]);
            insQRY.Parameters.Add("@Arbol", SqlDbType.NVarChar).Value = "";
            insQRY.Parameters.Add("@LavadoSustetable", SqlDbType.NVarChar).Value = Convert.ToString(Session["LavadoSustetable"]);
            insQRY.Parameters.Add("@Hibrido", SqlDbType.NVarChar).Value = Convert.ToString(Session["Hibrido"]);
            insQRY.Parameters.Add("@Origen", SqlDbType.NVarChar).Value = Convert.ToString(Session["OrigenMantencion"]);
            insQRY.Parameters.Add("@AnclajePiso", SqlDbType.NVarChar).Value = Convert.ToString(Session["AnclajePiso"]).ToUpper();
            insQRY.Parameters.Add("@PisoInstaladoOK", SqlDbType.NVarChar).Value = Convert.ToString(Session["PisoInstaladoOK"]).ToUpper();
            //
            insQRY.Parameters.Add("@descto_mpp", SqlDbType.NVarChar).Value = Convert.ToString(Session["descto_mpp"]).ToUpper();
            Int32 precio_mpp = 0;
            if (Convert.ToString(Session["precio_mpp"]) != "")
            {
                precio_mpp = Convert.ToInt32(Session["precio_mpp"]);
            }
            insQRY.Parameters.Add("@precio_mpp", SqlDbType.Int).Value = precio_mpp;

            // afecto Programa T10
            string Check_T10 = "";

            switch (Session["Flujo_Programa_T10"].ToString())
            {
                case "APROBACION AUTOMATICA":
                    Check_T10 = "X";
                    break;
                case "PROGRAMA T10 ACTIVO":
                    Check_T10 = "X";
                    break;
                case "APROBACION MANUAL":
                    if (Session["estadoT10"] != null && Session["estadoT10"].ToString() == "Aprobar")
                    {
                        Check_T10 = "X";
                    }
                    break;
            }
            insQRY.Parameters.Add("@Check_T10", SqlDbType.NVarChar).Value = Check_T10;

            conexion1.Open();
            insQRY.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            string err = ex.Message + "<br>" + ex.StackTrace + "<br>" + ex.Source + "<br>" + ex.Data + "<br>" + ex.InnerException;
            if (conexion1 != null)
                conexion1.Close();
            Response.Write("<font color=red size=3>(A)Favor enviar este error a vuestro supervisor.<br> <b>" + Page.Page.AppRelativeVirtualPath + "<br>" + err + "</b></font>");
            Response.End();
        }
        finally
        {
            if (conexion1 != null)
                conexion1.Close();
        }
        #endregion
        #region Graba Proxima Mantencion
        //GENERO REV PROX
        MyRevisionNo1000 Rv = new MyRevisionNo1000(Convert.ToString(Session["Session_Stock"]), Convert.ToString(Session["Session_Combustible"]), Convert.ToString(Session["Session_Marca"]));
        Rv.GeneraProximaRevision();
        string miFecha = Rv.FecRevisionProxima.ToString("dd/MM/yyyy");
        string miHora = Convert.ToString(Rv.FecRevisionProxima.Hour).PadLeft(2, '0') + Convert.ToString(Rv.FecRevisionProxima.Minute).PadLeft(2, '0');
        try
        {
            conexion1 = new SqlConnection(ConfigurationManager.ConnectionStrings["SAP_WEB_GRUPOCRConnectionString"].ConnectionString);
            SqlCommand insQRY = new SqlCommand();
            insQRY.Connection = conexion1;
            insQRY.CommandText = "Sp_InsUpd_Vehiculo_Contacto_Mantencion";
            insQRY.CommandType = CommandType.StoredProcedure;
            insQRY.Parameters.Add("@stock", SqlDbType.NVarChar).Value = Convert.ToString(Session["Session_Stock"]);
            insQRY.Parameters.Add("@revProx", SqlDbType.Int).Value = Convert.ToString(Rv.RevisionProxima);
            insQRY.Parameters.Add("@fechaFutura", SqlDbType.DateTime).Value = miFecha;
            insQRY.Parameters.Add("@horaFutura", SqlDbType.NVarChar).Value = miHora;
            insQRY.Parameters.Add("@conce", SqlDbType.NVarChar).Value = Convert.ToString(Session["Codigo_antiguo"]);
            insQRY.Parameters.Add("@rut", SqlDbType.Int).Value = Convert.ToString(Session["Rut_Cliente_Final"]);
            insQRY.Parameters.Add("@usuario", SqlDbType.NVarChar).Value = Convert.ToString(Session["usuario"]);
            conexion1.Open();
            insQRY.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            if (conexion1 != null)
                conexion1.Close();
            Response.Write("<font color=red size=2>(B)Favor enviar este error a vuestro supervisor (" + Page.Page.AppRelativeVirtualPath + ") <br><b>" + ex.Message + "</b><br><br><u>Información Adicional</u><br>" + ex.StackTrace + "</font><br><br>");
            Response.End();
        }
        finally
        {
            if (conexion1 != null)
                conexion1.Close();
        }
        //grabo rev prox
        SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["SAP_WEBConnectionString"].ConnectionString);
        SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = "Sp_Ins_Garantia_GarantiaRevProx";
        command.CommandType = CommandType.StoredProcedure;

        // Add the input parameter and set its properties.
        SqlParameter parameter = new SqlParameter();
        parameter.ParameterName = "@GarNumero";
        parameter.SqlDbType = SqlDbType.Int;
        parameter.Direction = ParameterDirection.Input;
        parameter.Value = 0;
        command.Parameters.Add(parameter); ;
        //
        parameter = new SqlParameter();
        parameter.ParameterName = "@VehSto";
        parameter.SqlDbType = SqlDbType.NVarChar;
        parameter.Direction = ParameterDirection.Input;
        parameter.Value = Convert.ToString(Session["Session_Stock"]);
        command.Parameters.Add(parameter);
        //
        parameter = new SqlParameter();
        parameter.ParameterName = "@RevSat";
        parameter.SqlDbType = SqlDbType.Char;
        parameter.Direction = ParameterDirection.Input;
        parameter.Value = Convert.ToString(Session["Cliente_Numero"]);
        command.Parameters.Add(parameter);
        //
        parameter = new SqlParameter();
        parameter.ParameterName = "@RevNumero";
        parameter.SqlDbType = SqlDbType.Int;
        parameter.Direction = ParameterDirection.Input;
        parameter.Value = Convert.ToInt32(Rv.Revision);
        command.Parameters.Add(parameter);
        //
        parameter = new SqlParameter();
        parameter.ParameterName = "@RevProx";
        parameter.SqlDbType = SqlDbType.Int;
        parameter.Direction = ParameterDirection.Input;
        parameter.Value = Convert.ToInt32(Rv.RevisionProxima);
        command.Parameters.Add(parameter);
        //
        parameter = new SqlParameter();
        parameter.ParameterName = "@RevProxFec";
        parameter.SqlDbType = SqlDbType.DateTime;
        parameter.Direction = ParameterDirection.Input;
        parameter.Value = Convert.ToDateTime(Rv.FecRevisionProxima);
        command.Parameters.Add(parameter);
        //
        parameter = new SqlParameter();
        parameter.ParameterName = "@Usuario";
        parameter.SqlDbType = SqlDbType.NVarChar;
        parameter.Direction = ParameterDirection.Input;
        parameter.Value = Convert.ToString(Session["usuario"]);
        command.Parameters.Add(parameter);
        // SE AÑADE RUT DEL CLIENTE EN EL REGISTRO DE PROXIMA MANTENCION SP3277
        parameter = new SqlParameter();
        parameter.ParameterName = "@CustRut";
        parameter.SqlDbType = SqlDbType.Int;
        parameter.Direction = ParameterDirection.Input;
        parameter.Value = Convert.ToInt32(Session["Rut_Cliente_Final"]);
        // Add the parameter to the Parameters collection. 
        command.Parameters.Add(parameter);
        //
        parameter = new SqlParameter();
        parameter.ParameterName = "@CustDV";
        parameter.SqlDbType = SqlDbType.NVarChar;
        parameter.Direction = ParameterDirection.Input;
        parameter.Value = Convert.ToString(Session["Dv_Cliente_Final"]);
        command.Parameters.Add(parameter);
        try
        {
            connection.Open();
            command.CommandTimeout = 0;
            command.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            if (connection != null)
                connection.Close();
            Response.Write("<font color=red size=3>(C)Favor enviar este error a vuestro supervisor.<br> <b>" + Page.Page.AppRelativeVirtualPath + ex.Message + "</b></font>");
            Response.End();
        }
        finally
        {
            if (connection != null)
                connection.Close();
        }
        #endregion
        #region Graba Seguimineto PostServicio
        try
        {
            conexion1 = new SqlConnection(ConfigurationManager.ConnectionStrings["SAP_WEB_GRUPOCRConnectionString"].ConnectionString);
            SqlCommand insQRY = new SqlCommand();
            insQRY.Connection = conexion1;
            insQRY.CommandText = "Sp_InsUpd_Vehiculo_Contacto_Seguimiento_PostServicio";
            insQRY.CommandType = CommandType.StoredProcedure;
            insQRY.Parameters.Add("@stock", SqlDbType.NVarChar).Value = Convert.ToString(Session["Session_Stock"]);
            insQRY.Parameters.Add("@revision", SqlDbType.Int).Value = Convert.ToString(Rv.Revision);
            insQRY.Parameters.Add("@fechaFutura", SqlDbType.DateTime).Value = Convert.ToString(Session["Session_FecMantecion"]);
            insQRY.Parameters.Add("@horaFutura", SqlDbType.NVarChar).Value = "'0000'";
            insQRY.Parameters.Add("@conce", SqlDbType.NVarChar).Value = Convert.ToString(Session["Codigo_antiguo"]);
            insQRY.Parameters.Add("@rut", SqlDbType.Int).Value = Convert.ToString(Session["Rut_Cliente_Final"]);
            insQRY.Parameters.Add("@usuario", SqlDbType.NVarChar).Value = Convert.ToString(Session["usuario"]);
            insQRY.Parameters.Add("@OT", SqlDbType.NVarChar).Value = Convert.ToString(Session["Session_Ort"]);
            conexion1.Open();
            insQRY.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            if (conexion1 != null)
                conexion1.Close();
            Response.Write("<font color=red size=2>(D)Favor enviar este error a vuestro supervisor (" + Page.Page.AppRelativeVirtualPath + ") <br><b>" + ex.Message + "</b><br><br><u>Información Adicional</u><br>" + ex.StackTrace + "</font><br><br>");
            Response.End();
        }
        finally
        {
            if (conexion1 != null)
                conexion1.Close();
        }
        #endregion
        #region MPP
        string nombre = "", apellido = "", rut = "";
        conexion1 = new SqlConnection(ConfigurationManager.ConnectionStrings["SAP_WEBConnectionString"].ConnectionString);
        try
        {
            SqlCommand selQRY = new SqlCommand();
            selQRY.Connection = conexion1;
            selQRY.CommandText = "Sp_Lst_Toyota_Avanza_Consulta_Vehiculo_Cliente";
            selQRY.CommandType = CommandType.StoredProcedure;
            selQRY.Parameters.Add("@rut", SqlDbType.Int).Value = Convert.ToString(Session["Rut_Cliente_Final"]);
            SqlDataAdapter ad = new SqlDataAdapter(selQRY);
            DataTable table = new DataTable();
            table = new DataTable("myCliente");
            ad.Fill(table);
            if (table.Rows.Count > 0)
            {
                rut = Convert.ToString(table.Rows[0]["MYRUT"]);
                nombre = Convert.ToString(table.Rows[0]["CustNombre1"]).Trim().ToUpper() + " " + Convert.ToString(table.Rows[0]["CustNombre2"]).Trim().ToUpper();
                apellido = Convert.ToString(table.Rows[0]["CustApater"]).Trim().ToUpper() + " " + Convert.ToString(table.Rows[0]["CustAmater"]).Trim().ToUpper();
            }
            else
            {
                Response.Write("cliente no existe");
                Response.End();
            }
        }
        catch (Exception)
        {
            if (conexion1 != null) conexion1.Close();
        }
        finally
        {
            if (conexion1 != null) conexion1.Close();
        }
        //
        HyperLink1.NavigateUrl = "~/Consulta_Vehiculo/Consulta_Vehiculo_Stock_Link.aspx?stock=" + Convert.ToString(Session["Session_Stock"]);
        LblCodigo.Text = codigo;
        LblNOm.Text = nombre.ToUpper() + " " + apellido.ToUpper();
        LblRut.Text = rut;
        if (Convert.ToString(Session["IsSEQUOIA"]) == "N")
        {
            if (Convert.ToString(Session["myMantencion"]) == "1")
                LblTexto.Text = "Ha realizado su mantención de: 30 Días. ";
            else
                LblTexto.Text = "Ha realizado su mantención de: " + Convert.ToString(Session["myMantencion"]) + ".000 Kms. ";
        }
        else
        {
            if (Convert.ToString(Session["myMantencion"]) == "1")
                LblTexto.Text = "Ha realizado su mantención de: 30 Días. ";
            else
                LblTexto.Text = "Ha realizado su mantención de: " + Convert.ToString(Session["myMantencion"]) + ".000 Millas ";
        }
        LblTexto.Text = LblTexto.Text + "en el concesionario (" + Convert.ToString(Session["Cliente_Numero"]).Trim() + ") " + Convert.ToString(Session["Cliente_Nombre"]).Trim() + ".";
        if (Convert.ToString(Session["IsSEQUOIA"]) == "N")
        {
            LblRevProx.Text = Convert.ToString(Rv.RevisionProxima) + ".000 Kms.";
        }
        else
        {
            LblRevProx.Text = Convert.ToString(Rv.RevisionProxima) + ".000 Millas";
        }
        LblCita.Text = Convert.ToString(Session["Cita"]);
        LblFecRevProx.Text = Convert.ToString(Rv.FecRevisionProxima.ToShortDateString());
        conexion1.Close();
        /* GRABO LAS MPP */
        if (Convert.ToString(Session["myMantencion"]) != "1")
        {
            //MPP MAF
            if (Session["prepagoContratados"] != null && Session["OrigenMantencion"].ToString() == "MAF")
            {
                if (Convert.ToInt32(Session["prepagoContratados"]) != 0)
                {
                    if (Session["prepagoContratados"].ToString() != Session["totalmantenciones"].ToString())
                    {
                        MantencionPrepago_Upd();
                    }
                }
            }
            // MPP TIENDA TOYOTA
            if (Session["prepagoContratadosTienda"] != null && Session["OrigenMantencion"].ToString() == "Tienda")
            {
                if (Convert.ToInt32(Session["prepagoContratadosTienda"]) != 0)
                {
                    if (Session["prepagoContratadosTienda"].ToString() != Session["totalmantencionestienda"].ToString())
                    {
                        MantencionPrepago_Upd();
                    }
                }
            }
            // MPP TIENDA TOYOTA
            if (Session["prepagoContratadosTienda"] != null && Session["OrigenMantencion"].ToString() == "Tienda Lexus")
            {
                if (Convert.ToInt32(Session["prepagoContratadosTienda"]) != 0)
                {
                    if (Session["prepagoContratadosTienda"].ToString() != Session["totalmantencionestienda"].ToString())
                    {
                        MantencionPrepago_Upd();
                    }
                }
            }
            // MPP REGALO MAF
            if (Session["prepagosContratadosRegMAF"] != null && Session["OrigenMantencion"].ToString() == "REGMAF")
            {
                if (Convert.ToInt32(Session["prepagosContratadosRegMAF"]) != 0)
                {
                    if (Session["prepagosContratadosRegMAF"].ToString() != Session["totalmantencionesRegMaf"].ToString())
                    {
                        MantencionPrepago_Upd();
                    }
                }
            }
            // MPP REGALO TOYOTA
            if (Session["prepagosContratadosRegTCL"] != null && Session["OrigenMantencion"].ToString() == "REGTCL")
            {
                if (Convert.ToInt32(Session["prepagosContratadosRegTCL"]) != 0)
                {
                    if (Session["prepagosContratadosRegTCL"].ToString() != Session["totalmantencionesRegTcl"].ToString())
                    {
                        MantencionPrepago_Upd();
                    }
                }
            }
            // MPP REGALO ATENCION COMERCIAL
            if (Session["prepagosContratadosRegAtCom"] != null && Session["OrigenMantencion"].ToString() == "ATCOM")
            {
                if (Convert.ToInt32(Session["prepagosContratadosRegAtCom"]) != 0)
                {
                    if (Session["prepagosContratadosRegAtCom"].ToString() != Session["totalmantencionesRegAtCom"].ToString())
                    {
                        MantencionPrepago_Upd();
                    }
                }
            }
            // MPP Flota
            if (Session["prepagosContratadosFlota"] != null && Session["OrigenMantencion"].ToString() == "MPP Flota")
            {
                if (Convert.ToInt32(Session["prepagosContratadosFlota"]) != 0)
                {
                    if (Session["prepagosContratadosFlota"].ToString() != Session["totalmantencionesFlota"].ToString())
                    {
                        MantencionPrepago_Upd();
                    }
                }
            }
        }
        #endregion
        #region Concurso App Mundo Toyota - Graba Cobro Premio
        if (Session["AppMundoToyota"].ToString() == "S")
        {
            SqlConnection conSQL = new SqlConnection(ConfigurationManager.ConnectionStrings["SAP_WEBConnectionString"].ConnectionString);
            try
            {
                SqlCommand updQRY = new SqlCommand();
                updQRY.Connection = conSQL;
                updQRY.CommandText = "Sp_Upd_App_Mundo_Toyota_Ganador_Cobro";
                updQRY.CommandType = CommandType.StoredProcedure;
                updQRY.Parameters.Add("@stock", SqlDbType.NVarChar).Value = Convert.ToString(Session["Session_Stock"]);
                updQRY.Parameters.Add("@tipoOt", SqlDbType.NVarChar).Value = "'M'";
                updQRY.Parameters.Add("@NroOT", SqlDbType.NVarChar).Value = Convert.ToString(Session["Session_Ort"]);
                conSQL.Open();
                updQRY.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                if (conSQL != null)
                    conSQL.Close();
                Response.Write("<font color=red size=2>(E)Favor enviar este error a vuestro supervisor (" + Page.Page.AppRelativeVirtualPath + ") <br><b>" + ex.Message + "</b><br><br><u>Información Adicional</u><br>" + ex.StackTrace + "</font><br><br>");
                Response.End();
            }
            finally
            {
                if (conSQL != null)
                    conSQL.Close();
            }
        }
        #endregion
        #region Graba ProgramaT10
        Boolean grabaActivacion = false;
        if (Session["Flujo_Programa_T10"].ToString() == "APROBACION AUTOMATICA")
        {
            grabaActivacion = true;
        }
        if (Session["Flujo_Programa_T10"].ToString() == "APROBACION MANUAL")
        {
            if (Session["estadoT10"] != null && Session["estadoT10"].ToString() == "Aprobar")
            {
                grabaActivacion = true;
            }
        }
        if (grabaActivacion)
        {
            #region Activa Programa T10
            SqlConnection conSQL = new SqlConnection(ConfigurationManager.ConnectionStrings["SAP_WEBConnectionString"].ConnectionString);
            try
            {
                conSQL.Open();
                SqlCommand insQRY = new SqlCommand();
                insQRY.Connection = conSQL;
                insQRY.CommandText = "Sp_Ins_ProgramaT10_Activacion";
                insQRY.CommandType = CommandType.StoredProcedure;
                insQRY.Parameters.Add("@Numero_Stock", SqlDbType.NVarChar).Value = Convert.ToString(Session["Session_Stock"]);
                insQRY.Parameters.Add("@Concesionario", SqlDbType.NVarChar).Value = Convert.ToString(Session["Cliente_Numero"]);
                insQRY.Parameters.Add("@Numero_Mantencion", SqlDbType.Int).Value = Convert.ToInt32(Session["myMantencion"]);
                insQRY.Parameters.Add("@CheckListPDF", SqlDbType.NText).Value = Convert.ToString(Session["checklistPDF"]);
                insQRY.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                string err = ex.Message + "<br>" + ex.StackTrace + "<br>" + ex.Source + "<br>" + ex.Data + "<br>" + ex.InnerException;
                if (conSQL != null)
                    conSQL.Close();
                Response.Write("<font color=red size=3>(A)Favor enviar este error a vuestro supervisor.<br> <b>" + Page.Page.AppRelativeVirtualPath + "<br>" + err + "</b></font>");
                Response.End();
            }
            finally
            {
                if (conSQL != null)
                    conSQL.Close();
            }
            #endregion
        }

        if (Session["Flujo_Programa_T10"].ToString() == "DESACTIVA PROGRAMA T10")
        {
            #region Desactiva Programa T10
            SqlConnection conSQL = new SqlConnection(ConfigurationManager.ConnectionStrings["SAP_WEBConnectionString"].ConnectionString);
            try
            {
                conSQL.Open();
                SqlCommand insQRY = new SqlCommand();
                insQRY.Connection = conSQL;
                insQRY.CommandText = "Sp_Upd_ProgramaT10_Desactivacion";
                insQRY.CommandType = CommandType.StoredProcedure;
                insQRY.Parameters.Add("@Numero_Stock", SqlDbType.NVarChar).Value = Convert.ToString(Session["Session_Stock"]);
                insQRY.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                string err = ex.Message + "<br>" + ex.StackTrace + "<br>" + ex.Source + "<br>" + ex.Data + "<br>" + ex.InnerException;
                if (conSQL != null)
                    conSQL.Close();
                Response.Write("<font color=red size=3>(A)Favor enviar este error a vuestro supervisor.<br> <b>" + Page.Page.AppRelativeVirtualPath + "<br>" + err + "</b></font>");
                Response.End();
            }
            finally
            {
                if (conSQL != null)
                    conSQL.Close();
            }
            #endregion
        }

        #endregion
    }
    protected void Button7_Click(object sender, EventArgs e)
    {

    }

    protected void MantencionPrepago_Upd()
    {
        int prepagoContratados = Convert.ToInt32(Session["prepagoContratados"].ToString());
        int totalmantenciones = Convert.ToInt32(Session["totalmantenciones"].ToString());
        int prepagoContratadosTienda = Convert.ToInt32(Session["prepagoContratadosTienda"].ToString());
        int totalmantencionestienda = Convert.ToInt32(Session["totalmantencionestienda"].ToString());
        //
        int prepagoContratadosREGMAF = Convert.ToInt32(Session["prepagosContratadosRegMAF"].ToString());
        int totalmantencionesREGMAF = Convert.ToInt32(Session["totalmantencionesRegMaf"].ToString());
        int prepagoContratadosREGTCL = Convert.ToInt32(Session["prepagosContratadosRegTCL"].ToString());
        int totalmantencionesREGTCL = Convert.ToInt32(Session["totalmantencionesRegTcl"].ToString());
        int prepagoContratadosATCOM = Convert.ToInt32(Session["prepagosContratadosRegAtCom"].ToString());
        int totalmantencionesATCOM = Convert.ToInt32(Session["totalmantencionesRegAtCom"].ToString());
        int prepagoContratadosFlota = Convert.ToInt32(Session["prepagosContratadosFlota"].ToString());
        int totalmantencionesFlota = Convert.ToInt32(Session["totalmantencionesFlota"].ToString());
        //
        int valor = Convert.ToInt32(Session["myMantencion"].ToString());
        int mant1 = 0;
        int mant2 = 0;
        int mant3 = 0;
        int mant4 = 0;
        int mant5 = 0;
        SqlConnection conexion1 = new SqlConnection(ConfigurationManager.ConnectionStrings["SAP_WEBConnectionString"].ConnectionString);
        if (Session["OrigenMantencion"].ToString() == "MAF")
        {
            SqlCommand updQRY = new SqlCommand();
            updQRY.Connection = conexion1;
            updQRY.CommandText = "Sp_Upd_Toyota_Avanza_Maestro_MPP";
            updQRY.CommandType = CommandType.StoredProcedure;
            updQRY.Parameters.Add("@stock", SqlDbType.NVarChar).Value = Session["Session_Stock"];
            switch (prepagoContratados)
            {
                case 1:
                    if (totalmantenciones == 0 && valor == 10)
                    {
                        mant1 = 1;
                        Session["Mantencion_Realizada"] = mant1;
                        updQRY.Parameters.Add("@total", SqlDbType.Int).Value = mant1;
                    }
                    break;
                case 2:
                    if (totalmantenciones == 0 && valor == 10)
                    {
                        mant2 = 1;
                        Session["Mantencion_Realizada"] = mant2;
                        updQRY.Parameters.Add("@total", SqlDbType.Int).Value = mant2;
                    }
                    else if (totalmantenciones == 1 && valor == 20)
                    {
                        mant2 = 2;
                        Session["Mantencion_Realizada"] = mant2;
                        updQRY.Parameters.Add("@total", SqlDbType.Int).Value = mant2;
                    }
                    break;
                case 3:
                    if (totalmantenciones == 0 && valor == 10)
                    {
                        mant3 = 1;
                        Session["Mantencion_Realizada"] = mant3;
                        updQRY.Parameters.Add("@total", SqlDbType.Int).Value = mant3;
                    }
                    else if (totalmantenciones == 1 && valor == 20)
                    {
                        mant3 = 2;
                        Session["Mantencion_Realizada"] = mant3;
                        updQRY.Parameters.Add("@total", SqlDbType.Int).Value = mant3;
                    }
                    else if (totalmantenciones == 2 && valor == 30)
                    {
                        mant3 = 3;
                        Session["Mantencion_Realizada"] = mant3;
                        updQRY.Parameters.Add("@total", SqlDbType.Int).Value = mant3;
                    }
                    break;
                case 4:
                    if (totalmantenciones == 0 && valor == 10)
                    {
                        mant4 = 1;
                        Session["Mantencion_Realizada"] = mant4;
                        updQRY.Parameters.Add("@total", SqlDbType.Int).Value = mant4;
                    }
                    if (totalmantenciones == 0 && valor == 20)
                    {
                        mant4 = 2;
                        Session["Mantencion_Realizada"] = mant4;
                        updQRY.Parameters.Add("@total", SqlDbType.Int).Value = mant4;
                    }
                    if (totalmantenciones == 0 && valor == 30)
                    {
                        mant4 = 3;
                        Session["Mantencion_Realizada"] = mant4;
                        updQRY.Parameters.Add("@total", SqlDbType.Int).Value = mant4;
                    }
                    if (totalmantenciones == 0 && valor == 40)
                    {
                        mant4 = 4;
                        Session["Mantencion_Realizada"] = mant4;
                        updQRY.Parameters.Add("@total", SqlDbType.Int).Value = mant4;
                    }
                    break;
                case 5:
                    if (totalmantenciones == 0 && valor == 10)
                    {
                        mant5 = 1;
                        Session["Mantencion_Realizada"] = mant5;
                        updQRY.Parameters.Add("@total", SqlDbType.Int).Value = mant5;
                    }
                    if (totalmantenciones == 0 && valor == 20)
                    {
                        mant5 = 2;
                        Session["Mantencion_Realizada"] = mant5;
                        updQRY.Parameters.Add("@total", SqlDbType.Int).Value = mant5;
                    }
                    if (totalmantenciones == 0 && valor == 30)
                    {
                        mant5 = 3;
                        Session["Mantencion_Realizada"] = mant5;
                        updQRY.Parameters.Add("@total", SqlDbType.Int).Value = mant5;
                    }
                    if (totalmantenciones == 0 && valor == 40)
                    {
                        mant5 = 4;
                        Session["Mantencion_Realizada"] = mant5;
                        updQRY.Parameters.Add("@total", SqlDbType.Int).Value = mant5;
                    }
                    if (totalmantenciones == 0 && valor == 50)
                    {
                        mant5 = 5;
                        Session["Mantencion_Realizada"] = mant5;
                        updQRY.Parameters.Add("@total", SqlDbType.Int).Value = mant5;
                    }
                    break;
            }
            try
            {
                conexion1.Open();
                updQRY.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                if (conexion1 != null)
                    conexion1.Close();
                //Response.Write("<font color=red size=2>(F)Favor enviar este error a vuestro supervisor (" + Page.Page.AppRelativeVirtualPath + ") <br><b>" + ex.Message + "</b><br><br><u>Información Adicional</u><br>" + ex.StackTrace + "</font><br><br>");
                //Response.End();
            }
            finally
            {
                if (conexion1 != null)
                    conexion1.Close();
            }
        }
        else if (Session["OrigenMantencion"].ToString() != "MAF" && Session["OrigenMantencion"].ToString() != "")
        {
            conexion1 = new SqlConnection(ConfigurationManager.ConnectionStrings["SAP_WEBConnectionString"].ConnectionString);
            try
            {
                SqlCommand updQRY = new SqlCommand();
                updQRY.Connection = conexion1;
                updQRY.CommandText = "Sp_Upd_Toyota_Avanza_Consulta_Vehiculo_Mantenciones_MPP";
                updQRY.CommandType = CommandType.StoredProcedure;
                updQRY.Parameters.Add("@stock", SqlDbType.NVarChar).Value = Session["Session_Stock"];
                updQRY.Parameters.Add("@mantencion", SqlDbType.NVarChar).Value = valor;
                updQRY.Parameters.Add("@origen_mpp", SqlDbType.NVarChar).Value = Session["OrigenMantencion"].ToString();
                conexion1.Open();
                updQRY.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                if (conexion1 != null)
                    conexion1.Close();
                Response.Write("<font color=red size=2>(G)Favor enviar este error a vuestro supervisor (" + Page.Page.AppRelativeVirtualPath + ") <br><b>" + ex.Message + "</b><br><br><u>Información Adicional</u><br>" + ex.StackTrace + "</font><br><br>");
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