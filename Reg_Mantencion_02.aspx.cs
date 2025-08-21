using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Xml;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Globalization;
using System.Data.SqlTypes;
using OboutInc.Scheduler;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Drawing.Diagrams;
public partial class Registro_Mantenciones_Reg_Mantencion_02 : System.Web.UI.Page
{
    public string mantencionRealizada = "";
    public List<int> listaMantenciones = new List<int>();
    string raiz = "";
    protected void Page_Load(object sender, EventArgs e)
    {
        /*  control autentificacion */
        if (Session["usuario"] == null) Response.Redirect("../default.aspx");
        /**/
        raiz = Server.MapPath("~");
        VerificaSequoia();
        TxtFecReal.SelectedDate = DateTime.Now;
        if (!Page.IsPostBack)
        {
            Session["prepagoContratados"] = null;
            Session["totalmantenciones"] = null;

            Session["prepagoContratadosTienda"] = null;
            Session["totalmantencionestienda"] = null;

            Session["prepagosContratadosRegMAF"] = null;
            Session["totalmantencionesRegMaf"] = null;

            Session["prepagosContratadosRegTCL"] = null;
            Session["totalmantencionesRegTcl"] = null;

            Session["prepagosContratadosRegAtCom"] = null;
            Session["totalmantencionesRegAtCom"] = null;

            Session["prepagosContratadosFlota"] = null;
            Session["totalmantencionesFlota"] = null;

            mantencionesRealizadas(Session["Session_Stock"].ToString().Trim());
            comboMantencion();
            comboAsesor();
            B_Volver.PostBackUrl = "~/Registro_Mantenciones/Reg_Mantencion_01.aspx?rut=" + Convert.ToString(Session["Rut_Cliente_Final"]) + "&dv=" + Convert.ToString(Session["Dv_Cliente_Final"]) + "&stock=" + Convert.ToString(Session["Session_Stock"]);

            LblSinVenta.Visible = false;
            if (Session["Mensaje_SinRegistroVenta"] != null)
            {
                LblSinVenta.Text = Session["Mensaje_SinRegistroVenta"].ToString();
                LblSinVenta.Visible = true;
            }
            lblPisoInstaladoOK.Visible = false;
            TxtPisoInstaladoOK.Visible = false;
            TxtPisoInstaladoOK.Enabled = false;
        }
        Label4.Text = Convert.ToString(Session["Ud_Esta"]);
        TxtKM.Focus();
        Label7.Text = Convert.ToString(Session["Session_Stock"]);
        procesoStockCampana();
        procesoPrepago();
        habilitarAdjunto();
        #region Concurso App Mundo Toyota
        ConcursoAppMundoToyota(Convert.ToString(Session["Session_Stock"]));
        #endregion
        #region Afecto a Programa T10
        ValidaProgramaT10();
        #endregion
        
    }
    private void procesoStockCampana()
    {
        SqlConnection conexion1 = new SqlConnection(ConfigurationManager.ConnectionStrings["SAP_WEB_ENTRANAMIENTOConnectionString"].ConnectionString);
        try
        {
            LblCampana.Visible = false;
            SqlCommand selQRY = new SqlCommand();
            selQRY.Connection = conexion1;
            selQRY.CommandText = "Sp_Lst_OT_Campañas";
            selQRY.CommandType = CommandType.StoredProcedure;
            selQRY.Parameters.Add("@stock", SqlDbType.NVarChar).Value = Label7.Text;
            SqlDataAdapter adaptador1 = new SqlDataAdapter(selQRY);
            DataTable table = new DataTable();
            table = new DataTable("myVehCamp");
            adaptador1.Fill(table);
            if (table.Rows.Count > 0)
            {
                LblCampana.Visible = true;
                LblCampana.Text = "<div aling=center>Este stock esta en " + Convert.ToString(table.Rows.Count) + " campaña(s).";
            }
        }
        catch (Exception ex)
        {

            Response.Write("<div align=left><font color=red style='font-family:Arial;font-size:8pt;'><b><u>Error</u><br>" + Page.Page.AppRelativeVirtualPath + " - " + ex.Message + "</b><br><u>Detalle</u><br>" + ex.StackTrace + "</font><br></div>");
        }
        finally
        {
            if (conexion1 != null)
                conexion1.Close();
        }
    }
    private void habilitarAdjunto()
    {
        string origenPrepago = "";
        string descto_mpp = "";
        string precio_mpp = "";
        SqlConnection conexion1 = new SqlConnection(ConfigurationManager.ConnectionStrings["SAP_WEBConnectionString"].ConnectionString);
        try
        {
            SqlCommand selQRY = new SqlCommand();
            selQRY.Connection = conexion1;
            selQRY.CommandText = "Sp_lst_habilitar_Adjunto_MPP";
            selQRY.CommandType = CommandType.StoredProcedure;
            selQRY.Parameters.Add("@stock", SqlDbType.NVarChar).Value = Convert.ToString(Session["Session_Stock"]);
            selQRY.Parameters.Add("@mantencion", SqlDbType.Int).Value = Mantencion.SelectedValue;
            SqlDataAdapter adaptador1 = new SqlDataAdapter(selQRY);
            DataTable dt = new DataTable();
            dt = new DataTable("PrePago");
            adaptador1.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                origenPrepago = dt.Rows[0]["origen_prepago"].ToString();
                descto_mpp = dt.Rows[0]["dscto_mpp"].ToString();
                precio_mpp = dt.Rows[0]["precio_mpp"].ToString();
            }
        }
        catch (Exception ex)
        {
            Response.Write("<div align=left><font color=red style='font-family:Arial;font-size:8pt;'><b><u>Error</u><br>" + Page.Page.AppRelativeVirtualPath + " - " + ex.Message + "</b><br><u>Detalle</u><br>" + ex.StackTrace + "</font><br></div>");

        }
        finally
        {
            if (conexion1 != null)
                conexion1.Close();
        }
        
        if (origenPrepago.Trim() != "")
        {
            Session["descto_mpp"] = descto_mpp;
            Session["precio_mpp"] = precio_mpp;
        }
        if (origenPrepago == "MAF")
        {
            Session["OrigenMantencion"] = "MAF";
            Session["MostrarBoton"] = "";
        }
        else if (origenPrepago == "Tienda Toyota")
        {
            
            Button1.Enabled = true;
            Session["OrigenMantencion"] = "Tienda";
            Session["MostrarBoton"] = "";

        }
        else if (origenPrepago == "Tienda Lexus")
        {
            Button1.Enabled = true;
            Session["OrigenMantencion"] = "Tienda Lexus";
            Session["MostrarBoton"] = "";
        }
        else if (origenPrepago == "Regalo MAF")
        {
            Button1.Enabled = true;
            Session["OrigenMantencion"] = "REGMAF";
            Session["MostrarBoton"] = "NoMostrar";
        }
        else if (origenPrepago == "Regalo Toyota")
        {
            Button1.Enabled = true;
            Session["OrigenMantencion"] = "REGTCL";
            Session["MostrarBoton"] = "NoMostrar";
        }
        else if (origenPrepago == "Regalo Atencion Comercial")
        {
            Button1.Enabled = true;
            Session["OrigenMantencion"] = "ATCOM";
            Session["MostrarBoton"] = "NoMostrar";
        }
        else if (origenPrepago == "MPP Flota")
        {
            Button1.Enabled = true;
            Session["OrigenMantencion"] = "MPP Flota";
            Session["MostrarBoton"] = "NoMostrar";
        }
        else
        {
            Button1.Enabled = true;
            Session["OrigenMantencion"] = "";
            Session["descto_mpp"] = "";
            Session["precio_mpp"] = "";
            Session["MostrarBoton"] = "NoMostrar";
        }

    }
    private void procesoPrepago()
    {
        InvokeServicioMAF(Convert.ToString(Session["Session_Stock"]));
        SqlConnection conexion1 = new SqlConnection(ConfigurationManager.ConnectionStrings["SAP_WEBConnectionString"].ConnectionString);
        try
        {
            LblPrepago.Visible = false;
            LblPrepago.Text = "";

            SqlCommand selQRY = new SqlCommand();
            selQRY.Connection = conexion1;
            selQRY.CommandText = "Sp_Lst_OT_Prepago_Detalle";
            selQRY.CommandType = CommandType.StoredProcedure;
            selQRY.Parameters.Add("@stock", SqlDbType.NVarChar).Value = Convert.ToString(Session["Session_Stock"]);
            SqlDataAdapter adaptador1 = new SqlDataAdapter(selQRY); DataTable dt = new DataTable();
            dt = new DataTable("PrePago");
            adaptador1.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                int prepagosContratados = Convert.ToInt32(dt.Rows[0]["prepagos"].ToString());
                int prepagosContratadosTienda = Convert.ToInt32(dt.Rows[0]["prepagos_tienda"].ToString());
                int prepagosContratadosRegMAF = Convert.ToInt32(dt.Rows[0]["prepagos_regmaf"].ToString());
                int prepagosContratadosRegTCL = Convert.ToInt32(dt.Rows[0]["prepagos_regtcl"].ToString());
                int prepagosContratadosRegAtCom = Convert.ToInt32(dt.Rows[0]["prepagos_regatcom"].ToString());
                int prepagosContratadosFlota = Convert.ToInt32(dt.Rows[0]["prepagos_flota"].ToString());

                int totalmantenciones = Convert.ToInt32(dt.Rows[0]["TotalMantenciones"].ToString());
                int totalmantencionestienda = Convert.ToInt32(dt.Rows[0]["TotalMantencionesTienda"].ToString());
                int totalmantencionesRegMaf = Convert.ToInt32(dt.Rows[0]["TotalMantencionesRegMaf"].ToString());
                int totalmantencionesRegTcl = Convert.ToInt32(dt.Rows[0]["TotalMantencionesRegTcl"].ToString());
                int totalmantencionesRegAtCom = Convert.ToInt32(dt.Rows[0]["TotalMantencionesRegAtCom"].ToString());
                int totalmantencionesFlota = Convert.ToInt32(dt.Rows[0]["TotalMantencionesFlota"].ToString());

                string NbConcesionario = dt.Rows[0]["grupo_concesionario"].ToString();
                string fecha = dt.Rows[0]["fecha"].ToString();
                string marca = dt.Rows[0]["Marca"].ToString();

                string avisoMPP = "<div><ul>";
                Int32 QtyTiposMPP = 0;
                // mantenciones MAF
                if (prepagosContratados > 0)
                {
                    int restante = prepagosContratados - totalmantenciones;
                    if (restante > 0)
                    {
                        QtyTiposMPP += 1;
                        avisoMPP += "<li> " + restante + " Mantencion(es) Prepagadas MAF <em style='color: #008000;'>adquiridas en el " + NbConcesionario + "</em></li>";
                    }
                }
                // mantenciones Tienda
                if (prepagosContratadosTienda > 0)
                {
                    int restante = prepagosContratadosTienda - totalmantencionestienda;
                    if (restante > 0)
                    {
                        QtyTiposMPP += 1;
                        if (marca == "TOY")
                        {
                            avisoMPP += "<li> " + restante + " Mantencion(es) Prepagadas Tienda Toyota</li>";
                        }
                        if (marca == "LEX")
                        {
                            avisoMPP += "<li> " + restante + " Mantencion(es) Prepagadas Tienda Lexus</li>";
                        }
                    }
                }
                // mantenciones Regalo MAF
                if (prepagosContratadosRegMAF > 0)
                {
                    int restante = prepagosContratadosRegMAF - totalmantencionesRegMaf;
                    if (restante > 0)
                    {
                        QtyTiposMPP += 1;
                        avisoMPP += "<li> " + restante + " Mantencion(es) Prepagadas Regalo MAF</li>";
                    }
                }
                // mantenciones Regalo TCL
                if (prepagosContratadosRegTCL > 0)
                {
                    int restante = prepagosContratadosRegTCL - totalmantencionesRegTcl;
                    if (restante > 0)
                    {
                        QtyTiposMPP += 1;
                        avisoMPP += "<li> " + restante + " Mantencion(es) Prepagadas Regalo Toyota</li>";
                    }
                }
                // mantenciones Regalo Atencion Comercial
                if (prepagosContratadosRegAtCom > 0)
                {
                    int restante = prepagosContratadosRegAtCom - totalmantencionesRegAtCom;
                    if (restante > 0)
                    {
                        QtyTiposMPP += 1;
                        avisoMPP += "<li> " + restante + " Mantencion(es) Prepagadas Regalo Atencion Comercial (Toyota)</li>";
                    }
                }
                // mantenciones MPP Flota
                if (prepagosContratadosFlota > 0)
                {
                    int restante = prepagosContratadosFlota - totalmantencionesFlota;
                    if (restante > 0)
                    {
                        QtyTiposMPP += 1;
                        avisoMPP += "<li> " + restante + " Mantencion(es) Prepagadas MPP Flota</li>";
                    }
                }
                avisoMPP += "</ul></div>";
                LblPrepago.Visible = false;
                if (QtyTiposMPP > 0)
                {
                    LblPrepago.Visible = true;
                    LblPrepago.Text = "<div aling=center>Este stock tiene <b>" + avisoMPP;
                }



                Session["prepagoContratados"] = prepagosContratados;
                Session["totalmantenciones"] = totalmantenciones;

                Session["prepagoContratadosTienda"] = prepagosContratadosTienda;
                Session["totalmantencionestienda"] = totalmantencionestienda;

                Session["prepagosContratadosRegMAF"] = prepagosContratadosRegMAF;
                Session["totalmantencionesRegMaf"] = totalmantencionesRegMaf;

                Session["prepagosContratadosRegTCL"] = prepagosContratadosRegTCL;
                Session["totalmantencionesRegTcl"] = totalmantencionesRegTcl;

                Session["prepagosContratadosRegAtCom"] = prepagosContratadosRegAtCom;
                Session["totalmantencionesRegAtCom"] = totalmantencionesRegAtCom;

                Session["prepagosContratadosFlota"] = prepagosContratadosFlota;
                Session["totalmantencionesFlota"] = totalmantencionesFlota;


                int contratados = prepagosContratados;
                contratados += prepagosContratadosTienda;
                contratados += prepagosContratadosRegAtCom;
                contratados += prepagosContratadosRegMAF;
                contratados += prepagosContratadosRegTCL;
                contratados += prepagosContratadosFlota;

                int consumidos = totalmantenciones;
                consumidos += totalmantencionesRegAtCom;
                consumidos += totalmantencionesRegMaf;
                consumidos += totalmantencionesRegTcl;
                consumidos += totalmantencionestienda;
                consumidos += totalmantencionesFlota;

                int mantencionesrestantes2 = contratados - consumidos;
                int mantencionActual = totalmantenciones + mantencionesrestantes2;

                Session["mantencionesrestantes2"] = mantencionesrestantes2;


                mantencionRealizada = Convert.ToString(mantencionActual); //Session["mantencionActual"] = mantencionActual;
            }
        }
        catch (Exception ex)
        {
            Response.Write("<div align=left><font color=red style='font-family:Arial;font-size:8pt;'><b><u>Error</u><br>" + Page.Page.AppRelativeVirtualPath + " - " + ex.Message + "</b><br><u>Detalle</u><br>" + ex.StackTrace + "</font><br></div>");

        }
        finally
        {
            if (conexion1 != null)
                conexion1.Close();
        }
    }
    protected void VerificaSequoia()
    {
        Session["IsSEQUOIA"] = "N";
        SqlConnection conexion1 = new SqlConnection(ConfigurationManager.ConnectionStrings["SAP_WEBConnectionString"].ConnectionString);
        try
        {
            SqlCommand selQRY = new SqlCommand();
            selQRY.Connection = conexion1;
            selQRY.CommandText = "Sp_Lst_Stock_Sequoia";
            selQRY.CommandType = CommandType.StoredProcedure;
            selQRY.Parameters.Add("@stock", SqlDbType.NVarChar).Value = Convert.ToString(Session["Session_Stock"]);
            SqlDataAdapter adaptador1 = new SqlDataAdapter(selQRY);
            DataTable table = new DataTable();
            table = new DataTable("myVeh");
            adaptador1.Fill(table);
            if (table.Rows.Count > 0)
            {
                Session["IsSEQUOIA"] = "S";
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
    private Boolean existeMantencion(int km)
    {
        Boolean existe = false;
        for (int i = 0; i < listaMantenciones.Count; i++)
        {
            if (km == listaMantenciones[i])
            {
                existe = true;
            }
        }
        return existe;
    }
    private void comboMantencion()
    {
        Mantencion.Items.Clear();
        ListItem item = new ListItem();
        item.Text = "Seleccione";
        item.Value = "0";
        Mantencion.Items.Add(item);
        List<List<string>> mant = mantenciones();

        if (Convert.ToString(Session["IsSEQUOIA"]) == "N")
        {
            item = new ListItem();
            item.Text = "30 Días";
            item.Value = "1";
            if (!existeMantencion(1))
            {
                Mantencion.Items.Add(item);
            }
            //
            Int32 aSumar = Convert.ToInt32(Session["MantenAsumar"]);
            if (aSumar == 0) aSumar = 5;
            int i = aSumar;
            while (i <= 1000)
            {
                item = new ListItem();
                item.Text = Convert.ToString(i) + ".000 Kms" + origenMantencion(mant, i);
                if (i == 1000)
                    item.Text = Convert.ToString(i).Substring(0, 1) + ".000" + ".000 Kms";
                item.Value = Convert.ToString(i);
                if (!existeMantencion(i))
                {
                    Mantencion.Items.Add(item);
                }
                i = i + aSumar;
            }
        }
        else
        {
            item = new ListItem();
            item.Text = "30 Días";
            item.Value = "1";
            if (!existeMantencion(1))
            {
                Mantencion.Items.Add(item);
            }
            Int32 aSumar = 6;
            int i = aSumar;
            while (i <= 1000)
            {
                item = new ListItem();
                item.Text = Convert.ToString(i) + ".000 Millas";
                item.Value = Convert.ToString(i);
                if (!existeMantencion(i))
                {
                    Mantencion.Items.Add(item);
                }
                i = i + aSumar;
            }
        }

    }
    private void comboAsesor()
    {
        ListItem item = new ListItem();
        item.Text = "Seleccione";
        item.Value = "0";
        Asesor.Items.Add(item);
        SqlConnection conexion1 = new SqlConnection(ConfigurationManager.ConnectionStrings["SAP_WEBConnectionString"].ConnectionString);
        try
        {
            SqlCommand selQRY = new SqlCommand();
            selQRY.CommandText = "Sp_Lst_Toyota_Avanza_Registro_OT_Asesores";
            selQRY.CommandType = CommandType.StoredProcedure;
            selQRY.Connection = conexion1;
            selQRY.Parameters.Add("@dealer", SqlDbType.NVarChar).Value = Session["Cliente_Numero"].ToString();
            SqlDataAdapter ad = new SqlDataAdapter(selQRY);
            DataTable dt = new DataTable();
            dt = new DataTable("Asesor");
            ad.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                for (Int32 k = 0; k <= dt.Rows.Count - 1; k++)
                {
                    item = new ListItem();
                    item.Text = dt.Rows[k]["nombre"].ToString().Trim() + " " + dt.Rows[k]["apellido"].ToString().Trim();
                    item.Value = dt.Rows[k]["idasesor"].ToString();
                    Asesor.Items.Add(item);
                }
            }
        }
        catch (Exception ex)
        {
            Response.Write("<div align=left><font color=red style='font-family:Arial;font-size:8pt;'><b><u>Error</u><br>" + Page.Page.AppRelativeVirtualPath + " - " + ex.Message + "</b><br><u>Detalle</u><br>" + ex.StackTrace + "</font><br></div>");

        }
        finally
        {
            if (conexion1 != null)
                conexion1.Close();
        }


    }
    private void mantencionesRealizadas(string stock)
    {
        SqlConnection conexion1 = new SqlConnection(ConfigurationManager.ConnectionStrings["SAP_WEBConnectionString"].ConnectionString);
        try
        {
            SqlCommand selQRY = new SqlCommand();
            selQRY.Connection = conexion1;
            selQRY.CommandText = "Sp_Lst_Toyota_Avanza_Registro_OT_Mantenciones_Realizadas";
            selQRY.CommandType = CommandType.StoredProcedure;
            selQRY.Parameters.Add("@stock", SqlDbType.NVarChar).Value = stock;
            SqlDataAdapter ad = new SqlDataAdapter(selQRY);
            DataTable dt = new DataTable();
            dt = new DataTable("PrePago");
            ad.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                for (Int32 k = 0; k <= dt.Rows.Count - 1; k++)
                {
                    listaMantenciones.Add(Convert.ToInt32(dt.Rows[k]["RevNumero"].ToString()));
                }
            }
        }
        catch (Exception ex)
        {
            Response.Write("<div align=left><font color=red style='font-family:Arial;font-size:8pt;'><b><u>Error</u><br>" + Page.Page.AppRelativeVirtualPath + " - " + ex.Message + "</b><br><u>Detalle</u><br>" + ex.StackTrace + "</font><br></div>");

        }
        finally
        {
            if (conexion1 != null)
                conexion1.Close();
        }
    }
    private List<List<string>> mantenciones()
    {
        List<List<string>> lista = new List<List<string>>();
        SqlConnection conexion1 = new SqlConnection(ConfigurationManager.ConnectionStrings["SAP_WEBConnectionString"].ConnectionString);
        try
        {
            SqlCommand selQRY = new SqlCommand();
            selQRY.Connection = conexion1;
            selQRY.CommandText = "Sp_Lst_OT_Prepago";
            selQRY.CommandType = CommandType.StoredProcedure;
            selQRY.Parameters.Add("@stock", SqlDbType.NVarChar).Value = Convert.ToString(Session["Session_Stock"]);
            SqlDataAdapter ad = new SqlDataAdapter(selQRY);
            DataTable dt = new DataTable();
            dt = new DataTable("PrePago");
            ad.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                for (Int32 k = 0; k <= dt.Rows.Count - 1; k++)
                {
                    List<string> dato = new List<string>();
                    dato.Add(dt.Rows[k]["origen_prepago"].ToString());
                    dato.Add(dt.Rows[k]["mantencion"].ToString());
                    lista.Add(dato);
                }
            }
        }
        catch (Exception ex)
        {
            Response.Write("<div align=left><font color=red style='font-family:Arial;font-size:8pt;'><b><u>Error</u><br>" + Page.Page.AppRelativeVirtualPath + " - " + ex.Message + "</b><br><u>Detalle</u><br>" + ex.StackTrace + "</font><br></div>");

        }
        finally
        {
            if (conexion1 != null)
                conexion1.Close();
        }

        return lista;
    }
    private string origenMantencion(List<List<string>> mantenciones, int km)
    {
        // Origen / KM
        foreach (List<string> dato in mantenciones)
        {
            if (Convert.ToInt32(dato[1].ToString()) == km)
            {
                return " - " + dato[0].ToString();
            }
        }
        return "";
    }
    protected void ValidaOrt(object source, ServerValidateEventArgs args)
    {

        if (Convert.ToString(args.Value).Trim().Length > 0)
        {
            SqlConnection conexion1 = new SqlConnection(ConfigurationManager.ConnectionStrings["SAP_WEBConnectionString"].ConnectionString);
            SqlCommand selQRY = new SqlCommand();
            selQRY.Connection = conexion1;
            selQRY.CommandText = "Sp_Lst_Vehiculos_Mantencion_Conce_ORT";
            selQRY.CommandType = CommandType.StoredProcedure;
            selQRY.Parameters.Add("@SAT", SqlDbType.NVarChar).Value = Convert.ToString(Session["Cliente_Numero"]);
            selQRY.Parameters.Add("@ORT", SqlDbType.NVarChar).Value = TxtOrt.Text.Trim();
            SqlDataAdapter ad = new SqlDataAdapter(selQRY);
            DataTable table = new DataTable();
            table = new DataTable("myORT");
            ad.Fill(table);

            if (table.Rows.Count > 0)
            {
                args.IsValid = false;
                validaORT1.ErrorMessage = "ORT N° " + Convert.ToString(args.Value) + ", fue registrada al Stock: " + Convert.ToString(table.Rows[0]["VehSto"]).Trim() + ".<br> ";
            }
            else
            {
                args.IsValid = true;
            }
        }
    }
    protected void Button1_Click(object sender, EventArgs e)
    {
        int MPPRestantes = 0;
        if (Session["mantencionesrestantes2"] != null)
        { MPPRestantes = Convert.ToInt32(Session["mantencionesrestantes2"].ToString()); }
        if (chkAnclajePiso.Checked)
        {
            if (TxtPisoInstaladoOK.Text.Trim().ToUpper() != "S" && TxtPisoInstaladoOK.Text.Trim().ToUpper() != "N")
            {
                lblMSG.Text = "Indico que Unidad tiene Anclaje Piso Seguor. Debe seleccionar con S o N si esta correctamente instalado.";
                return;
            }
        }
        #region validacion programa T10
        Boolean _afecto_programaT10 = false;
        if (Session["´Programa_T10"] != null)
        {
            if (Session["´Programa_T10"].ToString() == "X") _afecto_programaT10 = true;
            if (upChecklistPDF.Visible)
            {
                if (hdnChecklistPDF.Value.Trim() == "")
                {
                    lblMSG.Text = "[Programa T10] Por tener Mantenciones Faltantes se requiere subir Checklist en formato PDF.";
                    return;
                }
                if (rdEstado.SelectedIndex == -1)
                {
                    lblMSG.Text = "[Programa T10] Debe indicar si Aprueba o Rechaza ingreso a Programa T10.";
                    return;
                }
            }
            if (rdEstado.SelectedValue == "Rechazar")
            {
                if (cboMotivoRechazo.SelectedValue.Trim() == "")
                {
                    lblMSG.Text = "[Programa T10] Debe seleccionar Motivo de Rechazo.";
                    return;
                }
            }
        }
        #endregion
        if (ValidadorFecha.IsValid && validaORT1.IsValid && ValidaKM1.IsValid && Vmantencion0.IsValid && Vmantencion1.IsValid && Vmantencion2.IsValid && Vasesor0.IsValid == true)
        {
            checkPatente chkPantente = new checkPatente(Convert.ToString(Session["Session_Stock"]));
            string textoError = chkPantente.validar().Trim();

            if (textoError.Length > 0)
            {
                Response.Write(@"<script language='javascript'>alert('" + textoError.Trim() + "');</script>");
            }
            else
            {

                Session["myMantencion"] = Convert.ToString(Mantencion.SelectedValue);
                Session["Session_Ort"] = Convert.ToString(TxtOrt.Text);
                Session["Session_KM"] = Convert.ToString(TxtKM.Text);
                Session["Session_Interno"] = Convert.ToString(TxtInterno.SelectedValue);
                Session["Session_FecMantecion"] = Convert.ToString(TxtFecReal.SelectedDate.ToShortDateString());

                Session["Cita"] = Convert.ToString(TxtCita.Text);
                //se agrega campo Retiro/Entregq Domiclio Ticket#8218191
                Session["Servicio"] = Convert.ToString(TxtServicio.Text);
                if (ChkRetiro.Checked == true)
                    Session["Session_Retiro"] = 1;
                else
                    Session["Session_Retiro"] = 0;

                if (ChkEntrega.Checked == true)
                    Session["Session_Entrega"] = 1;
                else
                    Session["Session_Entrega"] = 0;

                Session["Session_ProveedorServicio"] = Convert.ToString(TxtProveedor.Text);

                Session["Taller_Movil"] = Convert.ToString(TxtServMovil.Text);

                Session["AsesorID"] = Convert.ToString(Asesor.SelectedValue);
                Session["Asesor"] = Convert.ToString(Asesor.SelectedItem.Text);
                Session["Arbol"] = "";
                Session["LavadoSustetable"] = "N";
                Session["Hibrido"] = "N";
                if (chkLavadoSustentable.Checked) Session["LavadoSustetable"] = "S";
                if (chkHibrido.Checked) Session["Hibrido"] = "S";
                Session["AppMundoToyota"] = "N";
                if (chkiAppMundoToyota.Checked) Session["AppMundoToyota"] = "S";

                if (chkAnclajePiso.Checked)
                {
                    Session["AnclajePiso"] = "S";
                    Session["PisoInstaladoOK"] = TxtPisoInstaladoOK.Text;
                }
                else
                {
                    Session["AnclajePiso"] = "N";
                    Session["PisoInstaladoOK"] = "";
                }

                #region Programa T10
                Session["checklistPDF"] = null;
                Session["estadoT10"] = null;
                Session["MotivoRechazo"] = cboMotivoRechazo.SelectedValue;
                if (Session["Flujo_Programa_T10"].ToString() == "APROBACION MANUAL")
                {
                    Session["checklistPDF"] = hdnChecklistPDF.Value;
                    Session["estadoT10"] = rdEstado.SelectedValue;
                }
                if (Session["Flujo_Programa_T10"].ToString() == "DESACTIVA PROGRAMA T10")
                {
                    Session["desactivarProgramaT10"] = null;
                    if (hdnDesactivarProgramaT10.Value == "S") Session["desactivarProgramaT10"] = "S";
                }
                #endregion

                Response.Redirect("~/Registro_Mantenciones/Reg_Mantencion_03.aspx");
            }
        }
    }
    protected void ValidaKM(object source, ServerValidateEventArgs args)
    {
        if (Convert.ToString(args.Value).Trim().Length > 0)
        {
            if (Session["Nivel_Usuario"].ToString() == "C") //solo concesionarios
            {
                SqlConnection conexion1 = new SqlConnection(ConfigurationManager.ConnectionStrings["SAP_WEBConnectionString"].ConnectionString);
                SqlCommand selQRY1 = new SqlCommand();
                selQRY1.Connection = conexion1;
                selQRY1.CommandText = "Sp_Lst_Vehiculos_Mantencion_Stock_KM";
                selQRY1.CommandType = CommandType.StoredProcedure;
                selQRY1.Parameters.Add("@stock", SqlDbType.NVarChar).Value = Convert.ToString(Session["Session_Stock"]);
                SqlDataAdapter ad1 = new SqlDataAdapter(selQRY1);
                DataTable table = new DataTable();
                table = new DataTable("myORT");
                ad1.Fill(table);
                //si trata de ingresar KM menor al maximo registrado para el stock, da error
                if (Convert.ToInt32(args.Value) < Convert.ToInt32(table.Rows[0]["Mayor"]))
                {
                    args.IsValid = false;

                    ValidaKM1.ErrorMessage = "KM menor a último(" + Convert.ToString(table.Rows[0]["Mayor"]) + ") registrado N° Stock.<br> ";

                }
                else
                {
                    args.IsValid = true;
                }
            }
            if (Session["Nivel_Usuario"].ToString() != "C") // excluye concesionarios
            {
                SqlConnection conexion1 = new SqlConnection(ConfigurationManager.ConnectionStrings["SAP_WEBConnectionString"].ConnectionString);
                SqlCommand selQRY2 = new SqlCommand();
                selQRY2.Connection = conexion1;
                selQRY2.CommandText = "Sp_Lst_Vehiculos_Mantencion_Stock_KM2";
                selQRY2.CommandType = CommandType.StoredProcedure;
                selQRY2.Parameters.Add("@stock", SqlDbType.NVarChar).Value = Convert.ToString(Session["Session_Stock"]);
                selQRY2.Parameters.Add("@kmsing", SqlDbType.NVarChar).Value = Convert.ToInt32(args.Value);
                SqlDataAdapter ad2 = new SqlDataAdapter(selQRY2);
                DataTable table = new DataTable();
                table = new DataTable("myORT");
                ad2.Fill(table);
                if (table.Rows.Count > 0)
                {
                    if (Convert.ToInt32(args.Value) < Convert.ToInt32(table.Rows[0]["Mayor"]))
                    {
                        args.IsValid = false;

                        ValidaKM1.ErrorMessage = "KM Fuera de Rango.<br> ";

                    }
                    else
                    {
                        args.IsValid = true;
                    }
                }
            }
            /* valida si ya existe una OT,DYP o Mantencion  para el mismo dia y N° Stock se indique el mismo Kilometraje */

            SqlConnection conSQL = new SqlConnection(ConfigurationManager.ConnectionStrings["SAP_WEBConnectionString"].ConnectionString);
            Session["Session_FecMantecion"] = Convert.ToString(TxtFecReal.SelectedDate.ToShortDateString());
            conSQL.Open();
            SqlCommand selQRY = new SqlCommand();
            selQRY.Connection = conSQL;
            selQRY.CommandText = "Sp_lst_Toyota_Avanza_Registro_OT_KMS_Cont";
            selQRY.CommandType = CommandType.StoredProcedure;
            selQRY.Parameters.Add("@stock", SqlDbType.NVarChar).Value = Convert.ToString(Session["Session_Stock"]);
            selQRY.Parameters.Add("@fecha", SqlDbType.NVarChar).Value = Convert.ToString(Session["Session_FecMantecion"]);
            SqlDataAdapter ad = new SqlDataAdapter(selQRY);
            int cant = Convert.ToInt32(selQRY.ExecuteScalar());
            if (cant > 0)
            {
                conSQL.Close();
                conSQL.Open();
                selQRY = new SqlCommand();
                selQRY.Connection = conSQL;
                selQRY.CommandText = "Sp_lst_Toyota_Avanza_Registro_OT_KMS";
                selQRY.CommandType = CommandType.StoredProcedure;
                selQRY.Parameters.Add("@stock", SqlDbType.NVarChar).Value = Convert.ToString(Session["Session_Stock"]);
                selQRY.Parameters.Add("@fecha", SqlDbType.NVarChar).Value = Convert.ToString(Session["Session_FecMantecion"]);
                ad = new SqlDataAdapter(selQRY);
                DataTable tbl = new DataTable();
                tbl = new DataTable("miTabla");
                ad.Fill(tbl);
                if (tbl.Rows.Count > 0)
                {
                    TxtKM.Text = tbl.Rows[0]["kms"].ToString();
                    lblMSG.Text = "Ya Existe un Trabajo General para el Stock y Grupo de trabajo Seleccionado, se tomará el Kilometraje ingresado de la primera Orden Ingresada ";
                    TxtKM.Enabled = false;
                    //swSeguir = false;
                }
            }
        }
    }
    protected void ValidaMantencion1(object source, ServerValidateEventArgs args)
    {
        if (Convert.ToString(args.Value).Trim() != "0" && Vmantencion0.IsValid == true)
        {
            SqlConnection conexion1 = new SqlConnection(ConfigurationManager.ConnectionStrings["SAP_WEBConnectionString"].ConnectionString);
            SqlCommand selQRY = new SqlCommand();
            selQRY.Connection = conexion1;
            selQRY.CommandText = "Sp_Lst_Vehiculos_Mantencion_One";
            selQRY.CommandType = CommandType.StoredProcedure;
            selQRY.Parameters.Add("@stock", SqlDbType.NVarChar).Value = Session["Session_Stock"].ToString();
            selQRY.Parameters.Add("@mantencion", SqlDbType.NVarChar).Value = Convert.ToString(args.Value);
            SqlDataAdapter adaptador1 = new SqlDataAdapter(selQRY);
            DataTable table = new DataTable();
            table = new DataTable("myMantencion");
            adaptador1.Fill(table);

            if (table.Rows.Count > 0)
            {
                args.IsValid = false;
            }
            else
            {
                args.IsValid = true;
            }
        }
    }
    protected void ValidaMantencion2(object source, ServerValidateEventArgs args)
    {
        if (Convert.ToString(args.Value).Trim() != "0" && Vmantencion0.IsValid == true && Vmantencion1.IsValid == true && Vasesor0.IsValid == true)
        {
            SqlConnection conexion1 = new SqlConnection(ConfigurationManager.ConnectionStrings["SAP_WEBConnectionString"].ConnectionString);

            SqlCommand selQRY = new SqlCommand();
            selQRY.Connection = conexion1;
            selQRY.CommandText = "Sp_Lst_Vehiculos_Mantencion_All";
            selQRY.CommandType = CommandType.StoredProcedure;
            selQRY.Parameters.Add("@stock", SqlDbType.NVarChar).Value = Session["Session_Stock"].ToString().Trim();
            SqlDataAdapter adaptador1 = new SqlDataAdapter(selQRY);

            DataTable table = new DataTable();
            table = new DataTable("myMantenciones");
            adaptador1.Fill(table);

            if (table.Rows.Count > 0)
            {
                //como me retorna las mantenciones en orden descendente tomo la primera y comparo
                Int32 lastMantencion = Convert.ToInt32(table.Rows[0]["RevNumero"]);
                lblUltimaMantencion.Text = table.Rows[0]["RevNumero"].ToString();
                Int32 selMantencion = Convert.ToInt32(args.Value);
                if (selMantencion < lastMantencion)
                {
                    if (Session["Nivel_Usuario"].ToString() == "C") //solo concesionarios
                    {
                        args.IsValid = false;
                        Vmantencion2.ErrorMessage = "Mantención seleccionada es inferior a ultima registrada. Favor contactar a personal Toyota.<br>(Ult.Mantención Registrada: " + Convert.ToString(lastMantencion) + ".000 Kms)";
                    }
                }
                else
                {
                    args.IsValid = true;
                }

            }
            else
            {
                args.IsValid = true;
            }
        }
    }
    protected void ValidaMantencion3(object source, ServerValidateEventArgs args)
    {
        //Response.Write(@"<script language='javascript'>alert('" + args + "');</script>");
        if (Convert.ToString(args.Value).Trim() != "0")
        {
            args.IsValid = true;
        }
        else
        {
            args.IsValid = false;
        }

    }
    protected void ValidaMantencion4(object source, ServerValidateEventArgs args)
    {
        Int32 _kilometraje = 0;
        if (Int32.TryParse(TxtKM.Text, out _kilometraje))
        {
            Int32 _Mant_seleccionada = Convert.ToInt32(Mantencion.SelectedValue) * 1000;
            Int32 _Rango_Kilometraje_desde = _Mant_seleccionada - 5000;
            Int32 _Rango_Kilometraje_hasta = _Mant_seleccionada + 5000;
            if (_kilometraje >= _Rango_Kilometraje_desde && _kilometraje <= _Rango_Kilometraje_hasta)
            {
                args.IsValid = true;
            }
            else
            {
                args.IsValid = false;
            }
        }
    }
    protected void ValidaAsesor(object source, ServerValidateEventArgs args)
    {
        //Response.Write(@"<script language='javascript'>alert('" + args + "');</script>");
        if (Convert.ToString(args.Value).Trim() != "0")
        {
            args.IsValid = true;
        }
        else
        {
            args.IsValid = false;
        }

    }
    protected void TxtFecReal_DataBinding1(object sender, EventArgs e)
    {

        //  DateTime fecha = (DateTime)sender;
    }
    protected void validaFechas(object source, ServerValidateEventArgs args)
    {
        args.IsValid = true;
        if (Convert.ToDateTime(TxtFecReal.SelectedDate) == Convert.ToDateTime("01-01-0001 0:00:00"))
        {
            args.IsValid = false;
            TxtFecReal.Focus();
        }
        else
        {
            if (Convert.ToDateTime(TxtFecReal.SelectedDate) > DateTime.Now)
            {
                args.IsValid = false;
                TxtFecReal.Focus();

            }
        }

    }
    protected void Mantencion_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (Mantencion.SelectedValue == "1")
        {
            Session["MostrarBoton"] = "NoMostrar";
            Button1.Enabled = true;
        }
    }
    protected void TxtServicio_SelectedIndexChanged(object sender, EventArgs e)
    {

        if (Convert.ToString(TxtServicio.Text) == "s")
        {
            ChkEntrega.Enabled = true;
            ChkRetiro.Enabled = true;
            TxtProveedor.Enabled = true;
            if (Mantencion.SelectedValue == "1")
            {
                Session["MostrarBoton"] = "NoMostrar";
                Button1.Enabled = true;
            }
        }
        else if (Convert.ToString(TxtServicio.Text) == "n")
        {
            ChkEntrega.Enabled = false;
            ChkRetiro.Enabled = false;
            ChkEntrega.Checked = false;
            ChkRetiro.Checked = false;
            TxtProveedor.Enabled = false;
            if (Mantencion.SelectedValue == "1")
            {
                Session["MostrarBoton"] = "NoMostrar";
                Button1.Enabled = true;
            }
        }
    }
    protected void TxtServMovil_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (TxtServMovil.SelectedItem.Value == "s")
        {
            TxtServicio.Enabled = false;
            TxtProveedor.Enabled = false;
            ChkEntrega.Enabled = false;
            ChkRetiro.Enabled = false;
            TxtServicio.Items[0].Selected = false;
            TxtProveedor.Items[0].Selected = false;
        }
        else if (TxtServMovil.SelectedItem.Value == "n")
        {
            TxtServicio.Enabled = true;
            TxtProveedor.Enabled = true;
            ChkEntrega.Enabled = true;
            ChkRetiro.Enabled = true;
            //Label10.Visible = false;
        }
    }
    // llamada aservicio MAF jmoraga 24.11.2022
    #region Servicio MAF
    // Noviembre 2022 JMORAGA
    // Carga MPP desde Servicio MAF
    protected void InvokeServicioMAF(string _stock)
    {
        try
        {
            string retorno = "";
            HttpWebRequest request = MAF_CreateSOAPWebRequest();
            XmlDocument SOAPReqBody = new XmlDocument();
            string Soap = "<?xml version='1.0' encoding='utf-8'?> " +
                            "<soap:Envelope xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xmlns:xsd='http://www.w3.org/2001/XMLSchema' xmlns:soap='http://schemas.xmlsoap.org/soap/envelope/'> " +
                            "  <soap:Body> " +
                            "    <Datos_Clientes_Por_Stock xmlns='http://tempuri.org/'> " +
                            "      <usuario>maf_tcl</usuario> " +
                            "      <clave>maf_tcl5020</clave> " +
                            "      <NumeroStock>" + _stock + "</NumeroStock> " +
                            "    </Datos_Clientes_Por_Stock> " +
                            "  </soap:Body> " +
                            "</soap:Envelope>";
            SOAPReqBody.LoadXml(@Soap);
            using (Stream stream = request.GetRequestStream())
            {
                SOAPReqBody.Save(stream);
            }
            using (WebResponse Serviceres = request.GetResponse())
            {
                using (StreamReader rd = new StreamReader
                    (Serviceres.GetResponseStream()))
                {
                    //reading stream    
                    string ServiceResult = rd.ReadToEnd();
                    retorno = MAF_LeerXML(ServiceResult, _stock);
                }
            }
        }
        catch (Exception ex)
        {
            string err = ex.Message;
        }
    }
    public HttpWebRequest MAF_CreateSOAPWebRequest()
    {
        string Url;
        Url = "https://web.mafchile.com/ws_tcl/service.asmx";
        HttpWebRequest Req = (HttpWebRequest)WebRequest.Create(Url);
        try
        {
            Req.Headers.Add(@"SOAPAction:http://tempuri.org/Datos_Clientes_Por_Stock");
            ICredentials credentials = new NetworkCredential("maf_tcl", "maf_tcl5020");
            Req.Credentials = credentials;
            Req.ContentType = "text/xml;charset=\"utf-8\"";
            Req.Accept = "text/xml";
            Req.Method = "POST";
        }
        catch (Exception ex)
        {
            string err = ex.Message;
        }
        return Req;
    }
    public string MAF_LeerXML(string _XML, string _stock)
    {
        Int32 ntrack = 0;
        string _10k = "";
        string _20k = "";
        string _30k = "";
        string _40k = "";
        string _50k = "";
        string mensaje = "";
        string aviso = "";
        ntrack += 1;
        XmlDocument xmldoc = new XmlDocument();
        xmldoc.LoadXml(_XML);
        ntrack += 1;
        XmlNodeList nodeList;
        ntrack += 1;
        nodeList = xmldoc.GetElementsByTagName("MENSAJE");
        foreach (XmlNode node in nodeList)
        {
            mensaje = node.InnerText;
        }
        ntrack += 1;
        nodeList = xmldoc.GetElementsByTagName("MANTENCION_PREPAGADA");
        foreach (XmlNode node in nodeList)
        {
            aviso = node.InnerText;
        }
        ntrack += 1;
        if (mensaje != "STOCK NO PERTENECE A CLIENTES DE MAF" && aviso == "S")
        {
            ntrack += 1;
            nodeList = xmldoc.GetElementsByTagName("MANTENCION_10K");
            foreach (XmlNode node in nodeList)
            {
                _10k = node.InnerText;
            }
            ntrack += 1;
            nodeList = xmldoc.GetElementsByTagName("MANTENCION_20K");
            foreach (XmlNode node in nodeList)
            {
                _20k = node.InnerText;
            }
            ntrack += 1;
            nodeList = xmldoc.GetElementsByTagName("MANTENCION_30K");
            foreach (XmlNode node in nodeList)
            {
                _30k = node.InnerText;
            }
            ntrack += 1;
            nodeList = xmldoc.GetElementsByTagName("MANTENCION_40K");
            foreach (XmlNode node in nodeList)
            {
                _40k = node.InnerText;
            }
            ntrack += 1;
            nodeList = xmldoc.GetElementsByTagName("MANTENCION_50K");
            foreach (XmlNode node in nodeList)
            {
                _50k = node.InnerText;
            }
            ntrack += 1;
            Int32 prepago1 = Convert.ToInt32(_10k);
            Int32 prepago2 = Convert.ToInt32(_20k);
            Int32 prepago3 = Convert.ToInt32(_30k);
            Int32 prepago4 = Convert.ToInt32(_40k);
            Int32 prepago5 = Convert.ToInt32(_50k);
            ntrack += 1;
            string NumStock = _stock;
            #region Busca Modelo
            ntrack += 1;
            SqlConnection conSQL = new SqlConnection(ConfigurationManager.ConnectionStrings["SAP_WEBConnectionString"].ConnectionString);
            try
            {
                ntrack += 1;
                string modelo = "";
                Int32 IdTipoPrePago = 0;

                SqlCommand selQRY = new SqlCommand();
                selQRY.Connection = conSQL;
                selQRY.CommandText = "Sp_Lst_Toyota_Avanza_Consulta_Vehiculo_Modelo_Version";
                selQRY.CommandType = CommandType.StoredProcedure;
                selQRY.Parameters.Add("@stock", SqlDbType.NVarChar).Value = _stock;
                SqlDataAdapter ad = new SqlDataAdapter(selQRY);

                DataTable table = new DataTable();
                table = new DataTable("xyz");
                ad.Fill(table);
                if (table.Rows.Count > 0)
                {
                    ntrack += 1;
                    modelo = table.Rows[0][0].ToString().Trim();
                    string[] cadena = modelo.Split('-');
                    modelo = cadena[0];
                    string codigo_version;
                    string[] cadena2 = table.Rows[0][1].ToString().Trim().Split(' ');
                    codigo_version = cadena2[0];
                    VerificarDatos(codigo_version, modelo);
                    ntrack += 1;

                    selQRY = new SqlCommand();
                    selQRY.Connection = conSQL;
                    selQRY.CommandText = "Sp_Lst_Toyota_Avanza_Consulta_Vehiculo_Modelo_MPP";
                    selQRY.CommandType = CommandType.StoredProcedure;
                    selQRY.Parameters.Add("@modelo", SqlDbType.NVarChar).Value = modelo;
                    conSQL.Open();
                    int cant = Convert.ToInt32(selQRY.ExecuteScalar());
                    if (cant > 0)
                    {
                        ntrack += 1;

                        selQRY = new SqlCommand();
                        selQRY.Connection = conSQL;
                        selQRY.CommandText = "Sp_Lst_Toyota_Avanza_Consulta_Vehiculo_Tipo_MPP";
                        selQRY.CommandType = CommandType.StoredProcedure;
                        selQRY.Parameters.Add("@modelo", SqlDbType.NVarChar).Value = modelo;
                        SqlDataAdapter adaptador2 = new SqlDataAdapter(selQRY);

                        DataTable tbl1 = new DataTable();
                        tbl1 = new DataTable("TipoPrepago");
                        adaptador2.Fill(tbl1);
                        if (tbl1.Rows.Count > 0)
                        {
                            ntrack += 1;
                            IdTipoPrePago = Convert.ToInt32(tbl1.Rows[0]["IdTipo_PrePago"].ToString());
                        }
                        ntrack += 1;
                        /* INSERT INTO A LA TABLA MANTENCION_PREPAGO */

                        SqlCommand insQRY = new SqlCommand();
                        insQRY.Connection = conSQL;
                        insQRY.CommandText = "Sp_Ins_Mantencion_Prepago";
                        insQRY.CommandType = CommandType.StoredProcedure;
                        insQRY.Parameters.Add("@Numero_Stock", SqlDbType.NVarChar).Value = NumStock;
                        insQRY.Parameters.Add("@IdTipo_PrePago", SqlDbType.Int).Value = IdTipoPrePago;
                        if (prepago1 > 0 && prepago2 > 0 && prepago3 > 0 && prepago4 > 0 && prepago5 > 0)
                        {
                            ntrack += 1;
                            insQRY.Parameters.Add("@Prepago1", SqlDbType.Int).Value = 1;
                            insQRY.Parameters.Add("@Prepago2", SqlDbType.Int).Value = 1;
                            insQRY.Parameters.Add("@Prepago3", SqlDbType.Int).Value = 1;
                            insQRY.Parameters.Add("@Prepago4", SqlDbType.Int).Value = 1;
                            insQRY.Parameters.Add("@Prepago5", SqlDbType.Int).Value = 1;
                        }
                        if (prepago1 > 0 && prepago2 > 0 && prepago3 > 0 && prepago4 > 0 && prepago5 == 0)
                        {
                            ntrack += 1;
                            insQRY.Parameters.Add("@Prepago1", SqlDbType.Int).Value = 1;
                            insQRY.Parameters.Add("@Prepago2", SqlDbType.Int).Value = 1;
                            insQRY.Parameters.Add("@Prepago3", SqlDbType.Int).Value = 1;
                            insQRY.Parameters.Add("@Prepago4", SqlDbType.Int).Value = 1;
                            insQRY.Parameters.Add("@Prepago5", SqlDbType.Int).Value = 0;
                        }
                        if (prepago1 > 0 && prepago2 > 0 && prepago3 > 0 && prepago4 == 0 & prepago5 == 0)
                        {
                            ntrack += 1;
                            insQRY.Parameters.Add("@Prepago1", SqlDbType.Int).Value = 1;
                            insQRY.Parameters.Add("@Prepago2", SqlDbType.Int).Value = 1;
                            insQRY.Parameters.Add("@Prepago3", SqlDbType.Int).Value = 1;
                            insQRY.Parameters.Add("@Prepago4", SqlDbType.Int).Value = 0;
                            insQRY.Parameters.Add("@Prepago5", SqlDbType.Int).Value = 0;
                        }
                        if (prepago1 > 0 && prepago2 > 0 && prepago3 == 0 && prepago4 == 0 & prepago5 == 0)
                        {
                            ntrack += 1;
                            insQRY.Parameters.Add("@Prepago1", SqlDbType.Int).Value = 1;
                            insQRY.Parameters.Add("@Prepago2", SqlDbType.Int).Value = 1;
                            insQRY.Parameters.Add("@Prepago3", SqlDbType.Int).Value = 0;
                            insQRY.Parameters.Add("@Prepago4", SqlDbType.Int).Value = 0;
                            insQRY.Parameters.Add("@Prepago5", SqlDbType.Int).Value = 0;
                        }
                        if (prepago1 > 0 && prepago2 == 0 && prepago3 == 0 && prepago4 == 0 && prepago5 == 0)
                        {
                            ntrack += 1;
                            insQRY.Parameters.Add("@Prepago1", SqlDbType.Int).Value = 1;
                            insQRY.Parameters.Add("@Prepago2", SqlDbType.Int).Value = 0;
                            insQRY.Parameters.Add("@Prepago3", SqlDbType.Int).Value = 0;
                            insQRY.Parameters.Add("@Prepago4", SqlDbType.Int).Value = 0;
                            insQRY.Parameters.Add("@Prepago5", SqlDbType.Int).Value = 0;
                        }
                        try
                        {
                            ntrack += 1;
                            insQRY.Parameters.Add("@valor_prepago1", SqlDbType.Int).Value = prepago1;
                            insQRY.Parameters.Add("@valor_prepago2", SqlDbType.Int).Value = prepago2;
                            insQRY.Parameters.Add("@valor_prepago3", SqlDbType.Int).Value = prepago3;
                            insQRY.Parameters.Add("@valor_prepago4", SqlDbType.Int).Value = prepago4;
                            insQRY.Parameters.Add("@valor_prepago5", SqlDbType.Int).Value = prepago5;
                            conSQL.Open();
                            insQRY.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {
                            if (conSQL != null)
                                conSQL.Close();
                        }
                        finally
                        {
                            if (conSQL != null)
                                conSQL.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (conSQL != null)
                    conSQL.Close();
            }
            finally
            {
                if (conSQL != null)
                    conSQL.Close();
            }
            #endregion
        }
        return mensaje;
    }
    protected void VerificarDatos(string codigo_version, string modelo)
    {
        SqlConnection conSQL2 = new SqlConnection(ConfigurationManager.ConnectionStrings["SAP_WEBConnectionString"].ConnectionString);
        try
        {
            conSQL2.Open();
            SqlCommand selQRY = new SqlCommand();
            selQRY.Connection = conSQL2;
            selQRY.CommandText = "Sp_Lst_Toyota_Avanza_Consulta_Vehiculo_Modelo_MPP";
            selQRY.CommandType = CommandType.StoredProcedure;
            selQRY.Parameters.Add("@modelo", SqlDbType.NVarChar).Value = modelo;
            int cant = Convert.ToInt32(selQRY.ExecuteScalar());
            if (cant == 0)
            {
                selQRY = new SqlCommand();
                selQRY.Connection = conSQL2;
                selQRY.CommandText = "Sp_Lst_Toyota_Avanza_Consulta_Vehiculo_Tipo_MPP_x_Version";
                selQRY.CommandType = CommandType.StoredProcedure;
                selQRY.Parameters.Add("@version", SqlDbType.NVarChar).Value = codigo_version;
                SqlDataAdapter adaptador2 = new SqlDataAdapter(selQRY);

                DataTable tbl1 = new DataTable();
                tbl1 = new DataTable("TipoPrepago");
                adaptador2.Fill(tbl1);
                int idtipo_prepago = 0;
                if (tbl1.Rows.Count == 0)
                {
                    // Insertar TIPO_PREPAGO
                    SqlCommand insQRY = new SqlCommand();
                    insQRY.Connection = conSQL2;
                    insQRY.CommandText = "Sp_Ins_Toyota_Avanza_Consulta_Vehiculo_Tipo_MPP";
                    insQRY.CommandType = CommandType.StoredProcedure;
                    insQRY.Parameters.Add("@modelo", SqlDbType.NVarChar).Value = codigo_version;
                    insQRY.ExecuteNonQuery();
                    // Obtener idtipo_prepago
                    selQRY = new SqlCommand();
                    selQRY.Connection = conSQL2;
                    selQRY.CommandText = "Sp_Lst_Toyota_Avanza_Consulta_Vehiculo_Tipo_MPP_x_Version";
                    selQRY.CommandType = CommandType.StoredProcedure;
                    selQRY.Parameters.Add("@version", SqlDbType.NVarChar).Value = codigo_version;
                    adaptador2 = new SqlDataAdapter(selQRY);
                    tbl1 = new DataTable();
                    tbl1 = new DataTable("TipoPrepago");
                    adaptador2.Fill(tbl1);
                    if (tbl1.Rows.Count > 0)
                    {
                        idtipo_prepago = Convert.ToInt32(tbl1.Rows[0]["IdTipo_PrePago"].ToString());
                    }
                }
                else
                {
                    idtipo_prepago = Convert.ToInt32(tbl1.Rows[0]["IdTipo_PrePago"].ToString());
                }
                // Inserta maestro_prepago
                for (int valor = 10; valor <= 50; valor = valor + 10)
                {
                    SqlCommand insQRY = new SqlCommand();
                    insQRY.Connection = conSQL2;
                    insQRY.CommandText = "Sp_Ins_Toyota_Avanza_Consulta_Vehiculo_Maestro_MPP";
                    insQRY.CommandType = CommandType.StoredProcedure;
                    insQRY.Parameters.Add("@idtipo_prepago", SqlDbType.Int).Value = idtipo_prepago;
                    insQRY.Parameters.Add("@valor", SqlDbType.Int).Value = valor;
                    insQRY.Parameters.Add("@codigo_version", SqlDbType.Int).Value = codigo_version;
                    insQRY.Parameters.Add("@modelo", SqlDbType.Int).Value = modelo;
                    conSQL2.Open();
                    insQRY.ExecuteNonQuery();
                }
            }
        }
        catch (Exception ex)
        {
            if (conSQL2 != null)
                conSQL2.Close();
        }
        finally
        {
            if (conSQL2 != null)
                conSQL2.Close();
        }
    }
    #endregion
    #region Concurso App Mundo Toyota
    // Noviembre 2022 JMORAGA
    // Concurso App Mundo Toyota
    protected void ConcursoAppMundoToyota(string _stock)
    {
        lblAppMundoToyota.Visible = false;
        lblEtiAppMundoToyota.Visible = false;
        chkiAppMundoToyota.Visible = false;
        chkiAppMundoToyota.Enabled = false;
        SqlConnection conSQL = new SqlConnection(ConfigurationManager.ConnectionStrings["SAP_WEBConnectionString"].ConnectionString);
        try
        {
            SqlCommand selQRY = new SqlCommand();
            selQRY.Connection = conSQL;
            selQRY.CommandText = "Sp_Lst_App_Mundo_Toyota_Ganador_One";
            selQRY.CommandType = CommandType.StoredProcedure;
            selQRY.Parameters.Add("@stock", SqlDbType.NVarChar).Value = _stock;
            SqlDataAdapter ad = new SqlDataAdapter(selQRY);
            DataTable dt = new DataTable();
            ad.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                lblAppMundoToyota.Visible = true;
                string TipoOT = dt.Rows[0]["Cobro_Premio_Tipo_OT"].ToString();
                string TipoCliente = dt.Rows[0]["tipo_cliente"].ToString().Trim();
                #region Cliente GOLD. Premio Mantencion o Repuestos
                if (TipoCliente == "GOLD")
                {
                    lblEtiAppMundoToyota.Visible = true;
                    chkiAppMundoToyota.Visible = true;
                    if (TipoOT.Trim() == "")
                    {
                        chkiAppMundoToyota.Enabled = true;
                        lblAppMundoToyota.Text = "*** GANADOR Concurso APP Mundo Toyota ***<br>*** Puede cobrar premio ***";
                    }
                    else
                    {
                        chkiAppMundoToyota.Enabled = false;
                        lblAppMundoToyota.Text = "*** GANADOR Concurso APP Mundo Toyota ***<br>*** Premio ya fue cobrado por cliente ***";
                    }
                }
                #endregion
                #region Cliente SILVER. Premio 4 Cambio Filtro Aceite
                if (TipoCliente == "SILVER")
                {
                    Int32 CAF_Disponibles = 4;
                    string CAF1 = dt.Rows[0]["SILVER_CAF1"].ToString();
                    string CAF2 = dt.Rows[0]["SILVER_CAF2"].ToString();
                    string CAF3 = dt.Rows[0]["SILVER_CAF3"].ToString();
                    string CAF4 = dt.Rows[0]["SILVER_CAF4"].ToString();
                    if (CAF1.Trim() != "") CAF_Disponibles = CAF_Disponibles - 1;
                    if (CAF2.Trim() != "") CAF_Disponibles = CAF_Disponibles - 1;
                    if (CAF3.Trim() != "") CAF_Disponibles = CAF_Disponibles - 1;
                    if (CAF4.Trim() != "") CAF_Disponibles = CAF_Disponibles - 1;
                    lblAppMundoToyota.Text = "*** GANADOR Concurso APP Mundo Toyota ***<br>*** Premio ya fue cobrado por cliente ***";
                    if (CAF_Disponibles > 0)
                    {
                        lblAppMundoToyota.Text = "*** GANADOR Concurso APP Mundo Toyota ***<br>*** Tiene Disponible " + CAF_Disponibles + " Cambio Filtro Aceite ***";
                    }
                }
                #endregion
            }
        }
        catch (Exception)
        {
            if (conSQL != null)
                conSQL.Close();
        }
        finally
        {
            if (conSQL != null)
                conSQL.Close();
        }
    }
    #endregion
    protected void chkAnclajePiso_CheckedChanged(object sender, EventArgs e)
    {
        if (chkAnclajePiso.Checked)
        {
            lblPisoInstaladoOK.Visible = true;
            TxtPisoInstaladoOK.Visible = true;
            TxtPisoInstaladoOK.Enabled = true;
        }
        else
        {
            lblPisoInstaladoOK.Visible = false;
            TxtPisoInstaladoOK.Visible = false;
            TxtPisoInstaladoOK.Enabled = false;
            TxtPisoInstaladoOK.SelectedIndex = -1;
        }
    }
    #region Afecto a Programa T10
    protected void ValidaProgramaT10()
    {
        imgPrecacionFin.Visible = false;
        imgPrecaucionIni.Visible = false;
        lblMsgPrecaucion.Visible = false;
        lblTitChecklist.Visible = false;
        upChecklistPDF.Visible = false;
        lblTitEstadoT10.Visible = false;
        rdEstado.Visible = false;
        btnCargarPDF.Visible = false;
        lblMotivoRechazo.Visible = false;
        cboMotivoRechazo.Visible = false;
        #region Obtiene Marca
        string marca = Obtiene_Marca(Label7.Text);
        #endregion
        switch (marca)
        {
            case "TOY":
            case "LEX":
                #region Programa TOYOTA 10 o LEXUS Unlimited
                string titulopROGRAMA = "";
                if (marca == "TOY") titulopROGRAMA = "PROGRAMA T10";
                if (marca == "LEX") titulopROGRAMA = "PROGRAMA LEXUS UNLIMITED";
                lblAlertaProgramaT10.Text = "AFECTO A " + titulopROGRAMA;
                lblCuadroProgramaT10.Text = titulopROGRAMA;
                Session["Flujo_Programa_T10"] = null;// Controla el flujo de un stock a traves del Programa T10

                #region Verifica si ya tiene activado ProgramaT10
                Boolean ProgramaT10Activado = false;
                Boolean ProgramaT10Utilizado = false;
                SqlConnection conSQL = new SqlConnection(ConfigurationManager.ConnectionStrings["SAP_WEBConnectionString"].ConnectionString);
                try
                {
                    SqlCommand selQRY = new SqlCommand();
                    selQRY.Connection = conSQL;
                    selQRY.CommandText = "Sp_Lst_Programa_T10_Stock";
                    selQRY.CommandType = CommandType.StoredProcedure;
                    selQRY.Parameters.Add("@Numero_Stock", SqlDbType.VarChar).Value = Label7.Text;
                    SqlDataAdapter adaptador1 = new SqlDataAdapter(selQRY);
                    DataTable table = new DataTable();
                    table = new DataTable("myVeh");
                    adaptador1.Fill(table);
                    if (table.Rows.Count > 0)
                    {
                        ProgramaT10Utilizado = true;
                        hdnFechaActivacionT10.Value = table.Rows[0]["Fecha_Activacion"].ToString();
                        hdnConcesionarioT10.Value = table.Rows[0]["Concesionario"].ToString();
                        hdnNumeroMantencionT10.Value = table.Rows[0]["Numero_Mantencion"].ToString();
                        hdnFechaDesactivacionT10.Value = table.Rows[0]["Fecha_Desactivacion"].ToString();
                        hdnChecklistPDF.Value = table.Rows[0]["CheckListPDF"].ToString();
                        hdnFechaRechazoProgramaT11.Value = table.Rows[0]["Fecha_Rechazo"].ToString();
                        if (hdnFechaDesactivacionT10.Value.Trim() == "")
                        {
                            Session["Flujo_Programa_T10"] = "PROGRAMA T10 ACTIVO";
                            ProgramaT10Activado = true;
                        }
                        else
                        {
                            Session["Flujo_Programa_T10"] = "PROGRAMA T10 DESACTIVADO";
                        }
                        if (hdnFechaRechazoProgramaT11.ToString() != "")
                        {
                            Session["Flujo_Programa_T10"] = "PROGRAMA T10 RECHAZADO";
                        }
                        #region Crea panel para colocar reportes
                        string strHTML = "";
                        strHTML = strHTML + "<Table style='border: thin groove #C0C0C0' width='100%' cellpadding='3' cellspacing='3'>";
                        strHTML = strHTML + "<tr>";
                        strHTML = strHTML + "<td align='center' valign='middle' colspan='6' bgcolor= '#2563EB'>";
                        strHTML = strHTML + "<h5><font color= 'White'><b>Informacion " + titulopROGRAMA + "</b></font></h5>";
                        strHTML = strHTML + "</td>";
                        strHTML = strHTML + "</tr>";
                        strHTML = strHTML + "<tr>";
                        strHTML = strHTML + "<td bgcolor= 'whitesmoke' align='center' valign='middle' width='20%'><b>Fecha<br>Activacion</b></td>";
                        strHTML = strHTML + "<td bgcolor= 'whitesmoke' align='center' valign='middle' width='20%'><b>Concesionario</b></td>";
                        strHTML = strHTML + "<td bgcolor= 'whitesmoke' align='center' valign='middle' width='20%'><b>Numero<br>Mantencion</b></td>";
                        strHTML = strHTML + "<td bgcolor= 'whitesmoke' align='center' valign='middle' width='20%'><b>Fecha<br>Desactivacion</b></td>";
                        strHTML = strHTML + "<td bgcolor= 'whitesmoke' align='center' valign='middle' width='20%'><b>Checklist<br>PDF</b></td>";
                        strHTML = strHTML + "<td bgcolor= 'whitesmoke' align='center' valign='middle' width='20%'><b>Fecha<br>Rechazo</b></td>";
                        strHTML = strHTML + "</tr>";
                        strHTML = strHTML + "<tr>";
                        strHTML = strHTML + "<td width='20%'>" + hdnFechaActivacionT10.Value + "</b></td>";
                        strHTML = strHTML + "<td width='20%'>" + hdnConcesionarioT10.Value + "</td>";
                        strHTML = strHTML + "<td width='20%'>" + hdnNumeroMantencionT10.Value + ".000 kms.</td>";
                        strHTML = strHTML + "<td width='20%'>" + hdnFechaDesactivacionT10.Value + "</td>";
                        strHTML = strHTML + "<td width='20%'>";
                        if (hdnChecklistPDF.Value.Trim() != "")
                        {
                            strHTML = strHTML + "<a href='../Archivos/ProgramaT10/" + hdnChecklistPDF.Value.Trim() + "' target='_blank'>";
                            strHTML = strHTML + "<img src='../imagenes/pdf.png' alt='Ver Checklist' width='30%'>";
                            strHTML = strHTML + "</a'>";
                        }
                        strHTML = strHTML + "</td>";
                        strHTML = strHTML + "<td width='20%'>" + hdnFechaRechazoProgramaT11.Value + "</td>"; strHTML = strHTML + "</tr>";
                        #endregion
                        #region Muestra panel 
                        strHTML = strHTML + "</Table>";
                        pnlActivacionT10.Controls.Add(new LiteralControl(strHTML));
                        #endregion
                    }
                }
                catch (Exception)
                {
                    if (conSQL != null)
                        conSQL.Close();
                }
                finally
                {
                    if (conSQL != null)
                        conSQL.Close();
                }
                #endregion
                Session["´Programa_T10"] = null;
                lblAlertaProgramaT10.Visible = false;
                lblTitChecklist.Visible = false;
                upChecklistPDF.Visible = false;
                lblTitEstadoT10.Visible = false;
                rdEstado.Visible = false;
                btnCargarPDF.Visible = false;
                lblMotivoRechazo.Visible = false;
                cboMotivoRechazo.Visible = false;

                // Obtiene y Valida Antiguedad desde Fecha Venta
                Boolean Tiene_Antiguedad_Requerida = Valida_Antiguedad();

                // Obtiene ultimo Kilometraje
                Int32 Ultimo_Kilometraje = Obtiene_Ultimo_Kilometraje();
                hdnUltimoKilometraje.Value = Ultimo_Kilometraje.ToString();
                lblUltimoKilometraje.Text = Ultimo_Kilometraje.ToString("N0") + " kms.";
                Boolean Kilometraje_valido = false;
                imgKilometraje.ImageUrl = imgNo.ImageUrl;
                if (Ultimo_Kilometraje > 100000 && Ultimo_Kilometraje <= 200000)
                {
                    Kilometraje_valido = true;
                    imgKilometraje.ImageUrl = imgSi.ImageUrl;
                }

                // Obtiene Ultima Mantencion
                Int32 Ultima_Mantencion = Obtiene_Ultima_Mantencion();
                hdnUltimaMantencion.Value = Ultima_Mantencion.ToString();
                lblUltimaMantencion.Text = Ultima_Mantencion.ToString() + ".000 kms.";

                // Excluye del programa si uno de los filtros es excedido
                Int32 _años = 0;
                Int32 _meses = 0;
                Int32 _dias = 0;
                if (hdnAntiguedadAños.Value.Trim() != "") _años = Convert.ToInt32(hdnAntiguedadAños.Value);
                if (hdnAntiguedadAños.Value.Trim() != "") _meses = Convert.ToInt32(hdnAntiguedadMeses.Value);
                if (hdnAntiguedadAños.Value.Trim() != "") _dias = Convert.ToInt32(hdnAntiguedadDias.Value);
                Boolean excede_años = false;
                if (_años >= 10)
                {
                    if (_años == 10 && (_meses > 0 || _dias > 0)) excede_años = true;
                    if (_años > 10) excede_años = true;
                }
                if (Ultimo_Kilometraje > 200000 || excede_años)
                {
                    Kilometraje_valido = false;
                    Tiene_Antiguedad_Requerida = false;
                }
                if (rdEstado.SelectedValue == "Rechazar")
                {
                    lblMotivoRechazo.Visible = true;
                    cboMotivoRechazo.Visible = true;
                }
                if (!ProgramaT10Activado && !ProgramaT10Utilizado)
                {
                    #region Valida Programa T10 por pimera vez
                    if (Tiene_Antiguedad_Requerida || Kilometraje_valido)
                    {
                        Session["´Programa_T10"] = "X";
                        lblAlertaProgramaT10.Visible = true;

                        // Valida Mantenciones Faltantes
                        Boolean Mantenciones_faltantes = Valida_Mantenciones_Faltantes();

                        if (Mantenciones_faltantes)
                        {
                            // Habilta subir PDF y Checkbox de Aprobar y Rechazar
                            lblTitChecklist.Visible = true;
                            upChecklistPDF.Visible = true;
                            lblTitEstadoT10.Visible = true;
                            rdEstado.Visible = true;
                            btnCargarPDF.Visible = true;
                            lblValidoT10.Text = "Afecto a " + titulopROGRAMA + " con aprobacion manual (Checklist)";
                            Session["Flujo_Programa_T10"] = "APROBACION MANUAL";
                        }
                        else
                        {
                            // Aprobacion Automatica
                            lblTitChecklist.Visible = false;
                            upChecklistPDF.Visible = false;
                            lblTitEstadoT10.Visible = false;
                            rdEstado.Visible = false;
                            btnCargarPDF.Visible = false;
                            lblValidoT10.Text = "Afecto a " + titulopROGRAMA + " con aprobacion automatica";
                            Session["estadoT10"] = "Aprobar";
                            Session["Flujo_Programa_T10"] = "APROBACION AUTOMATICA";
                        }
                    }
                    else
                    {
                        lblValidoT10.Text = "NO Valido para " + titulopROGRAMA + "";
                        Session["Flujo_Programa_T10"] = "NO CALIFICA PARA PROGRAMA T10";
                    }
                    #endregion
                }
                else
                {
                    if (ProgramaT10Activado && hdnFechaRechazoProgramaT11.Value == "")
                    {
                        lblValidoT10.Text = titulopROGRAMA + " Activo, ver cuadro siguiente";
                        Session["Flujo_Programa_T10"] = "PROGRAMA T10 ACTIVO";
                        #region valida nueva OT continua programa o lo desactiva
                        imgPrecacionFin.Visible = false;
                        imgPrecaucionIni.Visible = false;
                        lblMsgPrecaucion.Visible = false;
                        // Calcula Diferencia en meses entre ultima mantencion y hoy
                        DateTime _fecUltMantencion = Convert.ToDateTime(hdnFechaUltimaMantencionSQL.Value);
                        DateTime fechaInicio = _fecUltMantencion;
                        DateTime fechaFin = DateTime.Now;
                        int meses = ((fechaFin.Year - fechaInicio.Year) * 12) + fechaFin.Month - fechaInicio.Month;
                        DateTime fechaReferencia = fechaInicio.AddMonths(meses);
                        int dias = (fechaFin - fechaReferencia).Days;
                        if (dias < 0)
                        {
                            meses--;
                            fechaReferencia = fechaInicio.AddMonths(meses);
                            dias = (fechaFin - fechaReferencia).Days;
                        }

                        // Calcula diferencia en kilometraje entre ultima mantencion y la que se esta ingresando
                        Int32 Dif_Kilometraje = 0;
                        Int32 _Ultimo_Kilometraje = 0;
                        Int32 _Nuevo_Kilometraje = 0;
                        if (TxtKM.Text.Trim() != "")
                        {
                            _Ultimo_Kilometraje = Convert.ToInt32(hdnUltimoKilometrajeSQL.Value);
                            _Nuevo_Kilometraje = Convert.ToInt32(TxtKM.Text);
                            Dif_Kilometraje = _Nuevo_Kilometraje - _Ultimo_Kilometraje;
                        }
                        hdnDesactivarProgramaT10.Value = "N";
                        if ((meses == 13 && dias > 0) || meses > 13)
                        {
                            imgPrecacionFin.Visible = true;
                            imgPrecaucionIni.Visible = true;
                            lblMsgPrecaucion.Visible = true;
                            lblMsgPrecaucion.Text = "Se desactivará " + titulopROGRAMA + ".<br>Ultima Mantencion mas de 12 meses.";
                            hdnDesactivarProgramaT10.Value = "S";
                            Session["Flujo_Programa_T10"] = "DESACTIVA PROGRAMA T10";
                        }
                        else
                        {
                            if (Dif_Kilometraje > 12000)
                            {
                                imgPrecacionFin.Visible = true;
                                imgPrecaucionIni.Visible = true;
                                lblMsgPrecaucion.Visible = true;
                                lblMsgPrecaucion.Text = "Se desactivará " + titulopROGRAMA + ".<br>Kilometraje registrado supera al límite requerido";
                                hdnDesactivarProgramaT10.Value = "S";
                                Session["´Programa_T10"] = null;
                                Session["Flujo_Programa_T10"] = "DESACTIVA PROGRAMA T10";
                            }
                            else
                            {
                                if (_Nuevo_Kilometraje > 200000)
                                {
                                    imgPrecacionFin.Visible = true;
                                    imgPrecaucionIni.Visible = true;
                                    lblMsgPrecaucion.Visible = true;
                                    lblMsgPrecaucion.Text = "Se desactivará " + titulopROGRAMA + ".<br>Kilometraje registrado supera tope de Garantia (200.000 kms)";
                                    hdnDesactivarProgramaT10.Value = "S";
                                    Session["´Programa_T10"] = null;
                                    Session["Flujo_Programa_T10"] = "DESACTIVA PROGRAMA T10";
                                }
                                else
                                {
                                    if (excede_años)
                                    {
                                        imgPrecacionFin.Visible = true;
                                        imgPrecaucionIni.Visible = true;
                                        lblMsgPrecaucion.Visible = true;
                                        lblMsgPrecaucion.Text = "Se desactivará " + titulopROGRAMA + ".<br>Excede periodo de garantia de 10 años";
                                        hdnDesactivarProgramaT10.Value = "S";
                                        Session["´Programa_T10"] = null;
                                        Session["Flujo_Programa_T10"] = "DESACTIVA PROGRAMA T10";
                                    }
                                }
                            }
                        }
                        #endregion
                    }
                    else
                    {
                        if (ProgramaT10Utilizado && hdnFechaDesactivacionT10.Value != "")
                        {
                            lblValidoT10.Text = titulopROGRAMA + " Inactivo, ver cuadro siguiente";
                            Session["Flujo_Programa_T10"] = "PROGRAMA T10 DESACTIVADO";
                        }
                        if (ProgramaT10Utilizado && hdnFechaRechazoProgramaT11.Value != "")
                        {
                            lblValidoT10.Text = titulopROGRAMA + " Rechazado, ver cuadro siguiente";
                            Session["Flujo_Programa_T10"] = "PROGRAMA T10 RECHAZADO";
                        }
                    }
                }
                #endregion
                break;
            default:
                lblValidoT10.Text = "NO Valido para Programa T10 o Programa LEXUS Unlimited ";
                Session["Flujo_Programa_T10"] = "NO CALIFICA PARA PROGRAMA T10";
                break;
        }
    }
    protected Boolean Valida_Antiguedad()
    {
        lblFecVenta.Text = "";
        imgAntiguedad.ImageUrl = imgNo.ImageUrl;
        Boolean Antiguedad = false;
        //Obtiene Fecha Venta
        string fecha = Obtiene_Fecha_Venta();
        DateTime fecVenta;

        Boolean existeFecVenta = false;
        if (DateTime.TryParse(fecha, out fecVenta))
        {
            existeFecVenta = true;
        }
        else
        {
            if (fecha.Length == 8)
            {
                if (DateTime.TryParseExact(fecha, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out fecVenta))
                {
                    existeFecVenta = true;
                }
            }
        }
        if (existeFecVenta)
        {
            #region Graba Fecha Venta en tabla auxiliar
            Graba_Fecha_Venta(Label7.Text, fecVenta);
            #endregion
            lblFecVenta.Text = fecVenta.ToString("dd-MM-yyyy");
            Antiguedad = CalculaAntiguedad(fecVenta, DateTime.Now);
            if (Antiguedad) imgAntiguedad.ImageUrl = imgSi.ImageUrl;
        }
        return Antiguedad;
    }
    protected string Obtiene_Fecha_Venta()
    {
        string fventa = "";
        SqlConnection conSQL = new SqlConnection(ConfigurationManager.ConnectionStrings["SAP_WEBConnectionString"].ConnectionString);
        try
        {
            SqlCommand selQRY = new SqlCommand();
            selQRY.Connection = conSQL;
            selQRY.CommandText = "Sp_Lst_Vehiculo_Reg_Garantia_One";
            selQRY.CommandType = CommandType.StoredProcedure;
            selQRY.Parameters.Add("@stock", SqlDbType.VarChar).Value = Label7.Text;
            SqlDataAdapter adaptador1 = new SqlDataAdapter(selQRY);
            DataTable table = new DataTable();
            table = new DataTable("myVeh");
            adaptador1.Fill(table);

            if (table.Rows.Count > 0)
            {
                fventa = table.Rows[0]["garfecent"].ToString();
            }
            else
            {
                fventa = busca_Garantia(Label7.Text);
            }
        }
        catch (Exception)
        {
            if (conSQL != null)
                conSQL.Close();
        }
        finally
        {
            if (conSQL != null)
                conSQL.Close();
        }
        return fventa;
    }
    protected Boolean CalculaAntiguedad(DateTime fechaInicio, DateTime fechaFin)
    {
        Boolean Valido = false;
        int años = fechaFin.Year - fechaInicio.Year;
        int meses = fechaFin.Month - fechaInicio.Month;
        int dias = fechaFin.Day - fechaInicio.Day;

        if (dias < 0)
        {
            meses--; // Restamos un mes
            DateTime mesPrevio = fechaFin.AddMonths(-1);
            dias += DateTime.DaysInMonth(mesPrevio.Year, mesPrevio.Month);
        }
        if (meses < 0)
        {
            años--; // Restamos un año
            meses += 12;
        }

        lblAntiguedad.Text = años + " años , " + meses + " meses , " + dias + " dias.";

        hdnAntiguedadAños.Value = años.ToString();
        hdnAntiguedadMeses.Value = meses.ToString();
        hdnAntiguedadDias.Value = dias.ToString();

        if (años >= 5)
        {
            if (años < 10)
            {
                if (años == 5 && meses == 0 && dias > 0) Valido = true;
                if (años == 5 && meses > 0) Valido = true;
                if (años > 5) Valido = true;
            }
            else
            {
                if (años == 10 && meses == 0 && dias == 0) Valido = true;
            }
        }
        return Valido;
    }
    static string busca_Garantia(string _stock)
    {
        string mensaje = InvokeService(_stock);
        return mensaje;
    }
    static string InvokeService(string _stock)
    {
        string retorno = "";
        try
        {
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
            SOAPReqBody.LoadXml(@Soap);
            using (Stream stream = request.GetRequestStream())
            {
                SOAPReqBody.Save(stream);
            }
            using (WebResponse Serviceres = request.GetResponse())
            {
                using (StreamReader rd = new StreamReader
                    (Serviceres.GetResponseStream()))
                {
                    string ServiceResult = rd.ReadToEnd();
                    retorno = LeerXML(ServiceResult);
                }
            }
        }
        catch (WebException)
        {
        }
        return retorno;
    }
    static HttpWebRequest CreateSOAPWebRequest()
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
    static string LeerXML(string _XML)
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
                mensaje = "Not Ok";
            }
        }
        else if (nodeList.Count == 0)
        {
            nodeList = xmldoc.GetElementsByTagName("fechaFactura");
            foreach (XmlNode node in nodeList)
            {
                mensaje = "Ok";
                mensaje = node.InnerText;
            }
        }
        return mensaje;
    }
    protected Int32 Obtiene_Ultimo_Kilometraje()
    {
        Int32 Kilometraje = 0;

        SqlConnection conexion1 = new SqlConnection(ConfigurationManager.ConnectionStrings["SAP_WEBConnectionString"].ConnectionString);
        SqlCommand selQRY1 = new SqlCommand();
        selQRY1.Connection = conexion1;
        selQRY1.CommandText = "Sp_Lst_Vehiculos_Mantencion_Stock_KM";
        selQRY1.CommandType = CommandType.StoredProcedure;
        selQRY1.Parameters.Add("@stock", SqlDbType.NVarChar).Value = Convert.ToString(Session["Session_Stock"]);
        SqlDataAdapter ad1 = new SqlDataAdapter(selQRY1);
        DataTable table = new DataTable();
        table = new DataTable("myORT");
        ad1.Fill(table);
        if (table.Rows.Count > 0)
        {
            Kilometraje = Convert.ToInt32(table.Rows[0]["Mayor"]);
            hdnUltimoKilometrajeSQL.Value = table.Rows[0]["Mayor"].ToString();
        }

        if (TxtKM.Text.Trim() != "") Kilometraje = Convert.ToInt32(TxtKM.Text);

        decimal km = Kilometraje;
        decimal km2 = Math.Round(km / 10000, 1);
        Int32 Mantenciones_Requeridas = Convert.ToInt32(km2);
        lblMantencionesRequeridas.Text = Mantenciones_Requeridas.ToString();
        return Kilometraje;
    }
    protected Int32 Obtiene_Ultima_Mantencion()
    {
        Int32 Ultima = 0;
        SqlConnection conexion1 = new SqlConnection(ConfigurationManager.ConnectionStrings["SAP_WEBConnectionString"].ConnectionString);
        try
        {
            SqlCommand selQRY = new SqlCommand();
            selQRY.Connection = conexion1;
            selQRY.CommandText = "Sp_Lst_Vehiculos_Mantencion_para_T10";
            selQRY.CommandType = CommandType.StoredProcedure;
            selQRY.Parameters.Add("@stock", SqlDbType.NVarChar).Value = Label7.Text;
            SqlDataAdapter adaptador1 = new SqlDataAdapter(selQRY);
            DataTable table = new DataTable();
            table = new DataTable("myMantenciones");
            adaptador1.Fill(table);
            if (table.Rows.Count > 0)
            {
                Ultima = Convert.ToInt32(table.Rows[0]["RevNumero"]);
                hdnFechaUltimaMantencionSQL.Value = table.Rows[0]["RevFec"].ToString();
            }
            lblMantencionesRegistradas.Text = Convert.ToString(table.Rows.Count);
            if (Mantencion.SelectedIndex > 0)
            {
                Ultima = Convert.ToInt32(Mantencion.SelectedValue);
                if (Ultima > 0)
                {
                    Int32 _mantenciones_Registradas = table.Rows.Count + 1;
                    lblMantencionesRegistradas.Text = Convert.ToString(_mantenciones_Registradas);
                }
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
        return Ultima;
    }
    protected Boolean Valida_Mantenciones_Faltantes()
    {
        Boolean faltantes = false;
        imgMantencionesRequeridas.ImageUrl = imgSi.ImageUrl;
        Int32 _mantenciones_requeridas = 0;
        Int32 _mantenciones_registradas = 0;
        if (lblMantencionesRequeridas.Text.Trim() != "") _mantenciones_requeridas = Convert.ToInt32(lblMantencionesRequeridas.Text);
        if (lblMantencionesRegistradas.Text.Trim() != "") _mantenciones_registradas = Convert.ToInt32(lblMantencionesRegistradas.Text);

        #region Obtiene limite 50% mantenciones requeridas
        Int32 Minimo_Mantenciones_Requeridas = _mantenciones_requeridas / 2;
        #endregion

        if (_mantenciones_registradas >= _mantenciones_requeridas)
        {
            imgMantencionesRequeridas.ImageUrl = imgSi.ImageUrl;
            faltantes = false;
        }
        if (_mantenciones_registradas < _mantenciones_requeridas)
        {
            faltantes = true;
            if (_mantenciones_registradas >= Minimo_Mantenciones_Requeridas)
            {
                imgMantencionesRequeridas.ImageUrl = imgWarning.ImageUrl;
            }
            else
            {
                imgMantencionesRequeridas.ImageUrl = imgNo.ImageUrl;
            }
        }
        return faltantes;
    }
    protected void imgbtnVerPDF_Click(object sender, ImageClickEventArgs e)
    {
        string raiz = ""; // Server.MapPath("..");
        string url = raiz + @"\\Archivos\ProgramaT10";
        string archivo = hdnChecklistPDF.Value;
        Response.Redirect(url + archivo);
    }
    protected void btnCargarPDF_Click(object sender, EventArgs e)
    {
        if (upChecklistPDF.HasFile)
        {
            string fileName = upChecklistPDF.FileName;
            string fileExtension = System.IO.Path.GetExtension(fileName).ToLower();

            // Validar que el archivo sea un PDF
            if (fileExtension != ".pdf")
            {
                lblMSG.Text = "[Programa T10] El archivo debe estar en formato PDF.";
                return;
            }

            string savePath = raiz + "\\Archivos\\ProgramaT10\\";
            string pathToCheck = savePath + fileName;

            // Verificar si el archivo ya existe y eliminarlo si es necesario
            if (System.IO.File.Exists(pathToCheck))
            {
                System.IO.File.Delete(pathToCheck);
            }

            // Guardar el archivo
            savePath += fileName;
            upChecklistPDF.SaveAs(savePath);

            // Asignar el nombre del archivo a un campo oculto
            hdnChecklistPDF.Value = fileName;
            imgbtnVerPDF.Visible = true;
            lblMSG.Text = "[Programa T10] Archivo cargado exitosamente.";
        }
        else
        {
            lblMSG.Text = "[Programa T10] Por tener Mantenciones Faltantes se requiere subir Checklist en formato PDF.";
        }
    }
    protected void TxtKM_TextChanged(object sender, EventArgs e)
    {
        //#region llena combo de mantenciones
        //comboMantencion();
        //#endregion
        //#region obtiene rango mantenciones a mostrar segun kilometraje
        //int kilometraje = 0;
        //int desde = 0;
        //int hasta = 0;
        //int _mant = 0;
        //if (int.TryParse(TxtKM.Text, out kilometraje))
        //{
        //    Int32 _indexTope = Mantencion.Items.Count;
        //    Int32 _recorre = 1;
        //    while (_recorre < _indexTope)
        //    {
        //        _mant = Convert.ToInt32(Mantencion.Items[_recorre].Value) * 1000;
        //        if (kilometraje >= _mant) desde = _mant;
        //        if (kilometraje <= _mant && hasta == 0) hasta = _mant;
        //        _recorre += 1;
        //    }
        //}
        //#endregion
        //#region eliminacion de items fuera de rango
        //for (int i = Mantencion.Items.Count - 1; i > 0; i--)
        //{
        //    int valorItem = Convert.ToInt32(Mantencion.Items[i].Value) * 1000;
        //    if (valorItem < desde || valorItem > hasta)
        //    {
        //        Mantencion.Items.RemoveAt(i);
        //    }
        //}
        //#endregion
    }
    protected void Graba_Fecha_Venta(string stock, DateTime fecha)
    {
        SqlConnection conexion1 = new SqlConnection(ConfigurationManager.ConnectionStrings["SAP_WEBConnectionString"].ConnectionString);
        try
        {
            SqlCommand cmdQRY = new SqlCommand();
            cmdQRY.Connection = conexion1;
            cmdQRY.CommandText = "Sp_Ins_Programa_T10_Fecha_Venta";
            cmdQRY.CommandType = CommandType.StoredProcedure;
            cmdQRY.Parameters.Add("@Numero_Stock", SqlDbType.NVarChar).Value = stock;
            cmdQRY.Parameters.Add("@Fecha_Venta", SqlDbType.DateTime).Value = fecha;
            conexion1.Open();
            cmdQRY.ExecuteNonQuery();
        }
        catch (Exception)
        {
            if (conexion1 != null) conexion1.Close();
        }
        finally
        {
            if (conexion1 != null) conexion1.Close();
        }
    }
    protected string Obtiene_Marca(string stock)
    {
        string marca = "";
        SqlConnection conexion1 = new SqlConnection(ConfigurationManager.ConnectionStrings["SAP_WEBConnectionString"].ConnectionString);
        try
        {
            SqlCommand selQRY = new SqlCommand();
            selQRY.Connection = conexion1;
            selQRY.CommandText = "Sp_Lst_Vehiculo_Only_One";
            selQRY.CommandType = CommandType.StoredProcedure;
            selQRY.Parameters.Add("@stock", SqlDbType.NVarChar).Value = stock;
            SqlDataAdapter adaptador1 = new SqlDataAdapter(selQRY);
            DataTable table = new DataTable();
            table = new DataTable();
            adaptador1.Fill(table);
            if (table.Rows.Count > 0)
            {
                marca = table.Rows[0]["marca"].ToString();
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
        return marca;
    }
    #endregion

    protected void rdEstado_SelectedIndexChanged(object sender, EventArgs e)
    {
        lblMotivoRechazo.Visible = false;
        cboMotivoRechazo.Visible = false;
        if (rdEstado.SelectedValue == "Rechazar")
        {
            lblMotivoRechazo.Visible = true;
            cboMotivoRechazo.Visible = true;
        }
    }
}
