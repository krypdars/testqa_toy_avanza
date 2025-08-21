using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.IO;
using System.Net;
using System.Globalization;
using System.Xml;

public partial class Registro_Mantenciones_Reg_Mantencion_03 : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        /*  control autentificacion */
        if (Session["usuario"] == null) Response.Redirect("../default.aspx");
        /**/
        Label4.Text = Convert.ToString(Session["Ud_Esta"]);
        if (Convert.ToString(Session["IsSEQUOIA"]) == "N")
        {
            if (Convert.ToString(Session["myMantencion"]) == "1")
                TxtRevision.Text = "30 Días";
            else
                TxtRevision.Text = Convert.ToString(Session["myMantencion"]) + ".000 Kms";
        }
        else
        {
            if (Convert.ToString(Session["myMantencion"]) == "1")
                TxtRevision.Text = "30 Días";
            else
            {
                Int32 vRev = Convert.ToInt32(Session["myMantencion"]) * 600;
                TxtRevision.Text = Convert.ToString(vRev) + " Millas";
            }
        }
        TxtFecReal.Text = Convert.ToString(Session["Session_FecMantecion"]);
        TxtKM.Text = Convert.ToString(Session["Session_KM"]);

        lblDebug.Text = Convert.ToString(Session["Session_KM"]);

        TxtOrt.Text = Convert.ToString(Session["Session_Ort"]);
        Label10.Text = Convert.ToString(Session["Session_Stock"]);
        if (Convert.ToString(Session["Session_Interno"]) == "0")
        {
            TxtInterno.Items[0].Selected = true;
        }
        if (Convert.ToString(Session["Session_Interno"]) == "1")
        {
            TxtInterno.Items[1].Selected = true;
        }
        TxtOrt.Focus();

        LblSinVenta.Visible = false;
        if (Session["Mensaje_SinRegistroVenta"] != null)
        {
            LblSinVenta.Text = Session["Mensaje_SinRegistroVenta"].ToString();
            LblSinVenta.Visible = true;
        }

        // se agrega este campo en el procedimiento de almacenado a causa de la incidencia 486, con fecha 04.07.2017 realizada
        // por fisla
        //TxtCita.Text = Convert.ToString(Session["Cita"]);

        if (Convert.ToString(Session["Cita"]) == "s")
        {
            TxtCita.Items[0].Selected = true;
        }
        if (Convert.ToString(Session["Cita"]) == "n")
        {
            TxtCita.Items[1].Selected = true;
        }
        //se agrega campo Domicilio Ticket#8218191
        if (Convert.ToString(Session["Servicio"]) == "s")
        {
            TxtServicio.Items[0].Selected = true;
        }
        if (Convert.ToString(Session["Servicio"]) == "n")
        {
            TxtServicio.Items[1].Selected = true;
        }
        if (Convert.ToInt32(Session["Session_Retiro"]) == 1)
            ChkRetiro.Checked = true;
        if (Convert.ToInt32(Session["Session_Entrega"]) == 1)
            ChkEntrega.Checked = true;

        if (Convert.ToString(Session["Session_ProveedorServicio"]) == "A")
        {
            TxtProveedor.Items[0].Selected = true;
        }
        if (Convert.ToString(Session["Session_ProveedorServicio"]) == "O")
        {
            TxtProveedor.Items[1].Selected = true;
        }
        /* Ticket#8218414 — RV: Requerimiento OT - Taller Móvil */
        if (Convert.ToString(Session["Taller_Movil"]) == "s")
        {
            TxtServMovil.Items[0].Selected = true;
        }
        if (Convert.ToString(Session["Taller_Movil"]) == "n")
        {
            TxtServMovil.Items[1].Selected = true;
        }
        //
        procesoStockCampana();
        /* Incidente Ticket#8204676 */
        //procesoPrepago();
        int MPPRestantes = 0;
        if (Session["mantencionesrestantes2"] != null)
        { MPPRestantes = Convert.ToInt32(Session["mantencionesrestantes2"].ToString()); }
        if (Convert.ToString(Session["myMantencion"]) != "1" && MPPRestantes > 0) procesoPrepago();

        //

        txtAsesor.Text = Session["Asesor"].ToString();

        chkLavadoSustentable.Checked = false;
        chkHibrido.Checked = false;
        if (Session["LavadoSustetable"].ToString() == "S") chkLavadoSustentable.Checked = true;
        if (Session["Hibrido"].ToString() == "S") chkHibrido.Checked = true;
        #region Concurso App Mundo Toyota
        ConcursoAppMundoToyota(Convert.ToString(Session["Session_Stock"]));
        if (Session["AppMundoToyota"].ToString() == "S") chkiAppMundoToyota.Checked = true;
        #endregion
        #region Piso Seguro
        chkAnclajePiso.Checked = false;
        lblPisoInstaladoOK.Visible = false;
        TxtPisoInstaladoOK.Visible = false;
        if (Session["AnclajePiso"].ToString() == "S")
        {
            chkAnclajePiso.Checked = true;
            TxtPisoInstaladoOK.Text = Session["PisoInstaladoOK"].ToString();
            lblPisoInstaladoOK.Visible = true;
            TxtPisoInstaladoOK.Visible = true;
        }
        #endregion
        #region Afecto a Programa T10
        rdEstado.SelectedValue= Session["estadoT10"].ToString().Trim();
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
            selQRY.Parameters.Add("@stock", SqlDbType.NVarChar).Value = Label10.Text;
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

    private void procesoPrepago()
    {
        SqlConnection conexion1 = new SqlConnection(ConfigurationManager.ConnectionStrings["SAP_WEBConnectionString"].ConnectionString);
        try
        {
            LblPrepago.Visible = false;
            SqlCommand selQRY = new SqlCommand();
            selQRY.Connection = conexion1;
            selQRY.CommandText = "Sp_Lst_OT_Prepago_Detalle";
            selQRY.CommandType = CommandType.StoredProcedure;
            selQRY.Parameters.Add("@stock", SqlDbType.NVarChar).Value = Convert.ToString(Session["myStock"]);
            SqlDataAdapter ad = new SqlDataAdapter(selQRY);
            DataTable dt = new DataTable();
            dt = new DataTable("PrePago");
            ad.Fill(dt);
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

                string avisoMPP = "<div><ul>";
                Int32 QtyTiposMPP = 0;
                // mantenciones MAF
                if (prepagosContratados > 0)
                {
                    int restante = prepagosContratados - totalmantenciones;
                    if (restante > 0)
                    {
                        QtyTiposMPP += 1;
                        avisoMPP += "<li> Mantenciones MAF: " + restante + "   <em style='color: #008000;'>adquiridas en el " + NbConcesionario + "</em></li>";
                        Label10.Visible = true;
                    }
                }
                // mantenciones Tienda
                if (prepagosContratadosTienda > 0)
                {
                    int restante = prepagosContratadosTienda - totalmantencionestienda;
                    if (restante > 0)
                    {
                        QtyTiposMPP += 1;
                        avisoMPP += "<li> Mantenciones Tienda: " + restante + "</li>";
                        Label10.Visible = true;
                    }
                }
                // mantenciones Regalo MAF
                if (prepagosContratadosRegMAF > 0)
                {
                    int restante = prepagosContratadosRegMAF - totalmantencionesRegMaf;
                    if (restante > 0)
                    {
                        QtyTiposMPP += 1;
                        avisoMPP += "<li> Mantenciones Regalo MAF: " + restante + "</li>";
                    }
                }
                // mantenciones Regalo TCL
                if (prepagosContratadosRegTCL > 0)
                {
                    int restante = prepagosContratadosRegTCL - totalmantencionesRegTcl;
                    if (restante > 0)
                    {
                        QtyTiposMPP += 1;
                        avisoMPP += "<li> Mantenciones Regalo Toyota: " + restante + "</li>";
                    }
                }
                // mantenciones Regalo Atencion Comercial
                if (prepagosContratadosRegAtCom > 0)
                {
                    int restante = prepagosContratadosRegAtCom - totalmantencionesRegAtCom;
                    if (restante > 0)
                    {
                        QtyTiposMPP += 1;
                        avisoMPP += "<li> Mantenciones Regalo Atencion Comercial (Toyota): " + restante + "</li>";
                    }
                }
                // mantenciones MPP Flota
                if (prepagosContratadosFlota > 0)
                {
                    int restante = prepagosContratadosFlota - totalmantencionesFlota;
                    if (restante > 0)
                    {
                        QtyTiposMPP += 1;
                        avisoMPP += "<li> Mantenciones MPP Flota: " + restante + "</li>";
                    }
                }
                avisoMPP += "</ul></div>";
                LblPrepago.Visible = false;
                if (QtyTiposMPP > 0)
                {
                    LblPrepago.Visible = true;
                    LblPrepago.Text = "<div aling=center>Este stock tiene <b>" + avisoMPP;
                }

                int mantencionesrestantes2 = prepagosContratados - totalmantenciones;
                int mantencionActual = totalmantenciones + mantencionesrestantes2;

                Session["mantencionesrestantes2"] = mantencionesrestantes2;

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

    protected void Button1_Click(object sender, EventArgs e)
    {

        Session["Session_Ort"] = Convert.ToString(TxtOrt.Text);
        Session["Session_KM"] = Convert.ToString(TxtKM.Text);
        Session["Session_Interno"] = Convert.ToString(TxtInterno.SelectedValue);
        Session["Session_FecMantecion"] = Convert.ToString(TxtFecReal.Text);

        Session["Cita"] = Convert.ToString(TxtCita.Text);

        if (ChkRetiro.Checked == true)
            Session["Session_Retiro"] = 1;
        else
            Session["Session_Retiro"] = 0;

        if (ChkEntrega.Checked == true)
            Session["Session_Entrega"] = 1;
        else
            Session["Session_Entrega"] = 0;

        Session["Session_ProveedorServicio"] = Convert.ToString(TxtProveedor.Text);
        /* Ticket#8218414 — RV: Requerimiento OT - Taller Móvil */
        Session["Taller_Movil"] = Convert.ToString(TxtServMovil.Text);
        Session["AppMundoToyota"] = "N";
        if (chkiAppMundoToyota.Checked) Session["AppMundoToyota"] = "S";
        //
        // Abril 2023 JMORAGA
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
        if (hdnFechaActivacionT10.Value.Trim() == "" && hdnFechaDesactivacionT10.Value.Trim() == "") // Aun no ha sido activado programa T10
        {
            Session["checklistPDF"] = hdnChecklistPDF.Value;
            Session["estadoT10"] = rdEstado.SelectedValue;
        }
        Session["desactivarProgramaT10"] = null;
        if (hdnDesactivarProgramaT10.Value == "S") Session["desactivarProgramaT10"] = "S";
        #endregion

        Response.Redirect("~/Registro_Mantenciones/Reg_Mantencion_03.aspx");

    }
    protected void TxtServicio_SelectedIndexChanged(object sender, EventArgs e)
    {

    }

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
            SqlDataAdapter adapt = new SqlDataAdapter(selQRY);
            DataTable dt = new DataTable();
            adapt.Fill(dt);
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
                        lblAppMundoToyota.Text = "*** GANADOR Concurso APP Mundo Toyota ***<br>*** Puede cobrar premio ***";
                    }
                    else
                    {
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

    #region Afecto a Programa T10
    protected void ValidaProgramaT10()
    {
        imgPrecacionFin.Visible = false;
        imgPrecaucionIni.Visible = false;
        lblMsgPrecaucion.Visible = false;
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
            selQRY.Parameters.Add("@Numero_Stock", SqlDbType.VarChar).Value = Label10.Text;
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
                if (hdnFechaDesactivacionT10.Value.Trim() == "")
                {
                    Session["Flujo_Programa_T10"] = "PROGRAMA T10 ACTIVO";
                    ProgramaT10Activado = true;
                }
                else 
                {
                    Session["Flujo_Programa_T10"] = "PROGRAMA T10 DESACTIVADO";
                }
                #region Crea panel para colocar reportes
                string strHTML = "";
                strHTML = strHTML + "<Table style='border: thin groove #C0C0C0' width='100%' cellpadding='3' cellspacing='3'>";
                strHTML = strHTML + "<tr>";
                strHTML = strHTML + "<td align='center' valign='middle' colspan='5' bgcolor= '#2563EB'>";
                strHTML = strHTML + "<h5><font color= 'White'><b>Informacion Activacion/Desactivacion</b></font></h5>";
                strHTML = strHTML + "</td>";
                strHTML = strHTML + "</tr>";
                strHTML = strHTML + "<tr>";
                strHTML = strHTML + "<td bgcolor= 'whitesmoke' align='center' valign='middle' width='20%'><b>Fecha<br>Activacion</b></td>";
                strHTML = strHTML + "<td bgcolor= 'whitesmoke' align='center' valign='middle' width='20%'><b>Concesionario</b></td>";
                strHTML = strHTML + "<td bgcolor= 'whitesmoke' align='center' valign='middle' width='20%'><b>Numero<br>Mantencion</b></td>";
                strHTML = strHTML + "<td bgcolor= 'whitesmoke' align='center' valign='middle' width='20%'><b>Fecha<br>Desactivacion</b></td>";
                strHTML = strHTML + "<td bgcolor= 'whitesmoke' align='center' valign='middle' width='20%'><b>Checklist<br>PDF</b></td>";
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
                strHTML = strHTML + "</tr>";
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
        lblProgramaT10.Visible = false;
        lblTitChecklist.Visible = false;
        lblTitEstadoT10.Visible = false;
        rdEstado.Visible = false;

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
        Int32 _años = Convert.ToInt32(hdnAntiguedadAños.Value);
        Int32 _meses = Convert.ToInt32(hdnAntiguedadMeses.Value);
        Int32 _dias = Convert.ToInt32(hdnAntiguedadDias.Value);
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
        if (!ProgramaT10Activado && !ProgramaT10Utilizado)
        {
            #region Valida Programa T10 por pimera vez
            if (Tiene_Antiguedad_Requerida || Kilometraje_valido)
            {
                Session["´Programa_T10"] = "X";
                lblProgramaT10.Visible = true;

                // Valida Mantenciones Faltantes
                Boolean Mantenciones_faltantes = Valida_Mantenciones_Faltantes();

                if (Mantenciones_faltantes)
                {
                    // Habilta subir PDF y Checkbox de Aprobar y Rechazar
                    lblTitChecklist.Visible = true;
                    lblTitEstadoT10.Visible = true;
                    rdEstado.Visible = true;
                    lblValidoT10.Text = "Afecto a programa T10 con aprobacion manual (Checklist)";
                    Session["Flujo_Programa_T10"] = "APROBACION MANUAL";
                }
                else
                {
                    // Aprobacion Automatica
                    lblTitChecklist.Visible = false;
                    lblTitEstadoT10.Visible = false;
                    rdEstado.Visible = false;
                    lblValidoT10.Text = "Afecto a programa T10 con aprobacion automatica";
                    Session["estadoT10"] = "Aprobar";
                    Session["Flujo_Programa_T10"] = "APROBACION AUTOMATICA";
                }
            }
            else
            {
                lblValidoT10.Text = "NO Valido para Programa T10";
                Session["Flujo_Programa_T10"] = "NO CALIFICA PARA PROGRAMA T10";
            }
            #endregion
        }
        else
        {
            if (ProgramaT10Activado)
            {
                lblValidoT10.Text = "PROGRAMA T10 Activo, ver cuadro siguiente";
                Session["Flujo_Programa_T10"] = "PROGRAMA T10 ACTIVO";
                #region valida nueva OT continua programa o lo desactiva
                imgPrecacionFin.Visible = false;
                imgPrecaucionIni.Visible = false;
                lblMsgPrecaucion.Visible = false;
                // Calcula Diferencia en meses entre ultima mantencion y hoy
                DateTime _fecUltMantencion = Convert.ToDateTime(hdnFechaUltimaMantencionSQL.Value);
                DateTime fechaInicio = _fecUltMantencion;
                DateTime fechaFin = DateTime.Now;
                int meses = Math.Abs((fechaFin.Year - fechaInicio.Year) * 12 + fechaFin.Month - fechaInicio.Month);
                // Calcula diferencia en kilometraje entre ultima mantencion y la que se esta ingresando
                Int32 Dif_Kilometraje = 0;
                if (TxtKM.Text.Trim() != "")
                {
                    Int32 _Ultimo_Kilometraje = Convert.ToInt32(hdnUltimoKilometrajeSQL.Value);
                    Int32 _Nuevo_Kilometraje = Convert.ToInt32(TxtKM.Text);
                    Dif_Kilometraje = _Nuevo_Kilometraje - _Ultimo_Kilometraje;
                }
                hdnDesactivarProgramaT10.Value = "N";
                if (meses > 13)
                {
                    imgPrecacionFin.Visible = true;
                    imgPrecaucionIni.Visible = true;
                    lblMsgPrecaucion.Visible = true;
                    lblMsgPrecaucion.Text = "Se desactivará Programa T10. Ultima Mantencion mas de 13 meses.";
                    hdnDesactivarProgramaT10.Value = "S";
                    Session["´Programa_T10"] = null;
                    Session["Flujo_Programa_T10"] = "DESACTIVA PROGRAMA T10";
                }
                else
                {
                    if (Dif_Kilometraje > 12000)
                    {
                        imgPrecacionFin.Visible = true;
                        imgPrecaucionIni.Visible = true;
                        lblMsgPrecaucion.Visible = true;
                        lblMsgPrecaucion.Text = "Se desactivará Programa T10.<br>Kilometraje transcurrido sobrepasa los 12.000 kms";
                        hdnDesactivarProgramaT10.Value = "S";
                        Session["´Programa_T10"] = null;
                        Session["Flujo_Programa_T10"] = "DESACTIVA PROGRAMA T10";
                    }
                }
                #endregion
            }
            else
            {
                if (ProgramaT10Utilizado)
                {
                    lblValidoT10.Text = "PROGRAMA T10 Inactivo, ver cuadro siguiente";
                    Session["Flujo_Programa_T10"] = "PROGRAMA T10 DESACTIVADO";
                }
            }
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
            selQRY.Parameters.Add("@stock", SqlDbType.VarChar).Value = Label10.Text;
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
                fventa = busca_Garantia(Label10.Text);
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
            selQRY.CommandText = "Sp_Lst_Vehiculos_Mantencion_Stock";
            selQRY.CommandType = CommandType.StoredProcedure;
            selQRY.Parameters.Add("@stock", SqlDbType.NVarChar).Value = Label10.Text;
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
            if (TxtRevision.Text.Trim() != "")
            {
                Int32 _mantenciones_Registradas = table.Rows.Count + 1;
                lblMantencionesRegistradas.Text = Convert.ToString(_mantenciones_Registradas);
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
        string raiz = "";
        string url = raiz + @"\\Archivos\ProgramaT10";
        string archivo = hdnChecklistPDF.Value;
        Response.Redirect(url + archivo);
    }
    #endregion
}
