<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Reg_Mantencion_03.aspx.cs" Inherits="Registro_Mantenciones_Reg_Mantencion_03" %>

<%@ Register TagPrefix="cl" TagName="Cliente" Src="~/Include/Tabla_Cliente_01.ascx" %>
<%@ Register TagPrefix="c2" TagName="Firma" Src="~/Include/firma.ascx" %>
<%@ Register TagPrefix="c3" TagName="StockClienteFinal" Src="~/Include/Stock_Cliente_Final_Resumen.ascx" %>
<%@ Register TagPrefix="c4" TagName="DatosClienteFinal" Src="~/Include/Datos_Cliente_FinalN.ascx" %>
<%@ Register Assembly="Enlasys.WebControls" Namespace="Enlasys.Web.UI.WebControls" TagPrefix="Enlasys" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xml:lang="es" xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Registro Mantenciones - 03</title>
</head>
<body topmargin="0">
    <form id="form1" runat="server">
        <div style="text-align: center">
            <asp:Label ID="Label1" runat="server" Font-Bold="True" Font-Names="Arial" Font-Size="14pt"
                Font-Underline="True" Text="Registro de Mantenciones <br><font size=1> (Paso 3)</font>"></asp:Label><br />
            <asp:Label
                ID="Label4" runat="server" Font-Bold="True" Font-Names="Arial" Font-Size="7pt"
                ForeColor="#CC0000" Text="Label" Width="186px"></asp:Label>
            <cl:Cliente ID="LinkId"
                runat="server"></cl:Cliente>
            <c3:StockClienteFinal ID="ClienteFinal1" runat="server"></c3:StockClienteFinal>
            &nbsp;
            <c4:DatosClienteFinal ID="datoscli1" runat="server" />
            <br />
            <div align="center">
                <asp:Label ID="LblCampana" runat="server" Font-Bold="True" Font-Names="Arial" Font-Size="12pt"
                    ForeColor="#990000" Width="80%"></asp:Label>
                <asp:Label ID="LblPrepago" runat="server" Font-Bold="True" Font-Names="Arial" Font-Size="12pt" ForeColor="#990000" Width="80%"></asp:Label>
                <br />
                <asp:Label ID="lblAppMundoToyota" runat="server" Font-Bold="True" Font-Names="Arial" Font-Size="12pt" ForeColor="#990000" Width="80%"></asp:Label>
            </div>
            <br />
            <asp:Label ID="LblSinVenta" runat="server" Font-Bold="True" Font-Names="Arial" Font-Size="11pt"
                Text="Label"></asp:Label>

            <table width="90%" border="1" bordercolor="#cc3333" cellpadding="0" cellspacing="0" align="center">
                <caption>
                    <asp:Label ID="lblDebug" runat="server" Text="Label"></asp:Label>
                </caption>
                <tr>
                    <th>
                                <asp:Label ID="lblProgramaT10" runat="server" BackColor="Red" Font-Bold="True" Font-Size="14pt" ForeColor="Yellow" Text="Afecto a Programa T10" Visible="False"></asp:Label>
                    </th>
                </tr>
                <tr>
                    <td colspan="2" style="text-align: left; height: 22px;">
                        <asp:Label ID="Label8" runat="server" Text="Datos de Revisión" Font-Bold="True" Font-Names="Arial" Font-Size="8pt" Font-Underline="True" ForeColor="Navy"></asp:Label></td>
                </tr>
                <tr>
                    <td style="text-align: center; height: 141px;" valign="top">
                        <table align="center" border="0" width="95%">
                            <tr>
                                <td style="text-align: center; vertical-align: top;">
                                    <table style="border: thin groove #000000; width: 95%;">
                                        <tr>
                                            <td colspan="4" bgcolor="#1E3A8A">
                                                <asp:Label ID="lblProgramaT11" runat="server" Font-Bold="True" Font-Size="12pt" Text="Programa T10" ForeColor="White"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td bgcolor="#2563EB">
                                                <asp:Label ID="Label28" runat="server" Font-Names="Arial" Font-Size="8pt" Text="Concepto" Font-Bold="True" ForeColor="White"></asp:Label></td>
                                            <td bgcolor="#2563EB">
                                                <asp:Label ID="Label30" runat="server" Font-Names="Arial" Font-Size="8pt" Text="Valor" Font-Bold="True" ForeColor="White"></asp:Label></td>
                                            <td bgcolor="#2563EB">
                                                <asp:Label ID="Label29" runat="server" Font-Names="Arial" Font-Size="8pt" Text="Condicion" Font-Bold="True" ForeColor="White"></asp:Label></td>
                                            <td bgcolor="#2563EB">
                                                <asp:Label ID="Label31" runat="server" Font-Names="Arial" Font-Size="8pt" Text="Valido" Font-Bold="True" ForeColor="White"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td bgcolor="#2563EB" style="text-align: left">
                                                <asp:Label ID="Label22" runat="server" Font-Names="Arial" Font-Size="8pt" Text="Fecha Venta" Font-Bold="True" ForeColor="White"></asp:Label></td>
                                            <td bgcolor="WhiteSmoke" style="vertical-align: middle; text-align: left">
                                                <asp:Label ID="lblFecVenta" runat="server" Font-Names="Arial" Font-Size="8pt"></asp:Label></td>
                                            <td bgcolor="WhiteSmoke" style="vertical-align: middle; text-align: left">&nbsp;</td>
                                            <td bgcolor="WhiteSmoke" style="vertical-align: middle; text-align: center"></td>
                                        </tr>
                                        <tr>
                                            <td bgcolor="#2563EB" style="text-align: left">
                                                <asp:Label ID="Label23" runat="server" Font-Names="Arial" Font-Size="8pt" Text="Antiguedad" Font-Bold="True" ForeColor="White"></asp:Label></td>
                                            <td bgcolor="WhiteSmoke" style="vertical-align: middle; text-align: left">
                                                <asp:Label ID="lblAntiguedad" runat="server" Font-Names="Arial" Font-Size="8pt"></asp:Label></td>
                                            <td bgcolor="WhiteSmoke" style="vertical-align: middle; text-align: left">
                                                <asp:Label ID="Label32" runat="server" Font-Names="Arial" Font-Size="8pt" Text="sobre 5 y hasta 10 años desde Fecha Venta" Font-Bold="True"></asp:Label></td>
                                            <td bgcolor="WhiteSmoke" style="vertical-align: middle; text-align: center">
                                                <asp:Image ID="imgAntiguedad" runat="server" Width="30%" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td bgcolor="#2563EB" style="text-align: left">
                                                <asp:Label ID="Label24" runat="server" Font-Names="Arial" Font-Size="8pt" Text="Kilometraje" Font-Bold="True" ForeColor="White"></asp:Label></td>
                                            <td bgcolor="WhiteSmoke" style="vertical-align: middle; text-align: left">
                                                <asp:Label ID="lblUltimoKilometraje" runat="server" Font-Names="Arial" Font-Size="8pt"></asp:Label></td>
                                            <td bgcolor="WhiteSmoke" style="vertical-align: middle; text-align: left">
                                                <asp:Label ID="Label33" runat="server" Font-Names="Arial" Font-Size="8pt" Text="sobre 100.000 y hasta 200.000" Font-Bold="True"></asp:Label></td>
                                            <td bgcolor="WhiteSmoke" style="vertical-align: middle; text-align: center">
                                                <asp:Image ID="imgKilometraje" runat="server" Width="30%" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td bgcolor="#2563EB" style="text-align: left">
                                                <asp:Label ID="Label25" runat="server" Font-Names="Arial" Font-Size="8pt" Text="Ultima Mantencion Registrada" Font-Bold="True" ForeColor="White"></asp:Label></td>
                                            <td bgcolor="WhiteSmoke" style="vertical-align: middle; text-align: left">
                                                <asp:Label ID="lblUltimaMantencion" runat="server" Font-Names="Arial" Font-Size="8pt"></asp:Label></td>
                                            <td bgcolor="WhiteSmoke" style="vertical-align: middle; text-align: left">&nbsp;</td>
                                            <td bgcolor="WhiteSmoke" style="vertical-align: middle; text-align: center">&nbsp;</td>
                                        </tr>
                                        <tr>
                                            <td bgcolor="#2563EB" style="text-align: left">
                                                <asp:Label ID="Label27" runat="server" Font-Names="Arial" Font-Size="8pt" Text="Mantenciones Requeridas" Font-Bold="True" ForeColor="White"></asp:Label></td>
                                            <td bgcolor="WhiteSmoke" style="vertical-align: middle; text-align: left">
                                                <asp:Label ID="lblMantencionesRequeridas" runat="server" Font-Names="Arial" Font-Size="8pt"></asp:Label></td>
                                            <td bgcolor="WhiteSmoke" style="vertical-align: middle; text-align: left">&nbsp;</td>
                                            <td bgcolor="WhiteSmoke" style="vertical-align: middle; text-align: center">&nbsp;</td>
                                        </tr>
                                        <tr>
                                            <td bgcolor="#2563EB" style="text-align: left">
                                                <asp:Label ID="Label26" runat="server" Font-Names="Arial" Font-Size="8pt" Text="Mantenciones Registradas" Font-Bold="True" ForeColor="White"></asp:Label></td>
                                            <td bgcolor="WhiteSmoke" style="vertical-align: middle; text-align: left">
                                                <asp:Label ID="lblMantencionesRegistradas" runat="server" Font-Names="Arial" Font-Size="8pt"></asp:Label></td>
                                            <td bgcolor="WhiteSmoke" style="vertical-align: middle; text-align: left">&nbsp;</td>
                                            <td bgcolor="WhiteSmoke" style="vertical-align: middle; text-align: center">
                                                <asp:Image ID="imgMantencionesRequeridas" runat="server" Width="30%" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td bgcolor="#2563EB" style="text-align: left">
                                                <asp:Label ID="lblTitChecklist" runat="server" Font-Names="Arial" Font-Size="8pt" Text="Checklist (PDF)" Font-Bold="True" Visible="False" ForeColor="White"></asp:Label></td>
                                            <td bgcolor="WhiteSmoke" colspan="3" style="vertical-align: middle; text-align: left">
                                                <table style="width: 100%;">
                                                    <tr>
                                                        <td>
                                                            &nbsp;</td>
                                                        <td>
                                                            &nbsp;</td>
                                                    </tr>
                                                </table>
                                                <asp:ImageButton ID="imgbtnVerPDF" runat="server" ImageUrl="~/Imagenes/pdf.png" Width="20%" Visible="False" OnClick="imgbtnVerPDF_Click" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td bgcolor="#2563EB" style="text-align: left">
                                                <asp:Label ID="lblTitEstadoT10" runat="server" Font-Names="Arial" Font-Size="8pt" Text="Estado T10" Font-Bold="True" Visible="False" ForeColor="White"></asp:Label></td>
                                            <td bgcolor="WhiteSmoke">
                                                <asp:RadioButtonList ID="rdEstado" runat="server" CellPadding="2" CellSpacing="2" RepeatDirection="Horizontal" Enabled="False">
                                                    <asp:ListItem>Aprobar</asp:ListItem>
                                                    <asp:ListItem>Rechazar</asp:ListItem>
                                                </asp:RadioButtonList>
                                            </td>
                                            <td bgcolor="WhiteSmoke">&nbsp;</td>
                                            <td bgcolor="WhiteSmoke" style="vertical-align: middle; text-align: center">&nbsp;</td>
                                        </tr>
                                        <tr>
                                            <td style="text-align: center; vertical-align: middle;" colspan="4">
                                                <asp:Label ID="lblValidoT10" runat="server" Font-Names="Arial" Font-Size="8pt" Font-Bold="True"></asp:Label></td>
                                        </tr>
                                        <tr>
                                            <td style="text-align: center; vertical-align: middle;" colspan="4">
                                                        <asp:Panel ID="pnlActivacionT10" runat="server">
                                                        </asp:Panel>
                                                    </td>
                                        </tr>
                                    </table>
                                            <table style="width: 100%;">
                                                <tr>
                                                    <td style="vertical-align: middle; text-align:right; width: 20%">
                                                        <asp:Image ID="imgPrecaucionIni" runat="server" ImageUrl="~/Imagenes/precaucion.png" Visible="False" Width="20%" />
                                                    </td>
                                                    <td style="vertical-align: middle; text-align: center; width: 60%">
                                                        <asp:Label ID="lblMsgPrecaucion" runat="server" Font-Names="Arial" Font-Size="10pt" Text="mensaje de precuacion" Width="100%" Font-Bold="True" ForeColor="Red"></asp:Label></td>
                                                    <td style="vertical-align: middle; text-align: left; width: 20%">
                                                        <asp:Image ID="imgPrecacionFin" runat="server" ImageUrl="~/Imagenes/precaucion.png" Visible="False" Width="20%" /></td>
                                                </tr>
                                            </table>
                                </td>
                                <td>
                                    <table align="center" border="0" width="75%">
                                        <caption></caption>
                                        <tr>
                                            <th></th>
                                        </tr>
                                        <tr>
                                            <td style="text-align: left; height: 24px;" bgcolor="#ccffcc" width="15%">
                                                <asp:Label ID="Label7" runat="server" Font-Names="Arial" Font-Size="8pt" Text="Stock"></asp:Label></td>
                                            <td style="text-align: left; height: 24px;" bgcolor="#ffffd9" width="40%">
                                                <asp:Label ID="Label10" runat="server" Font-Bold="True" Font-Names="Arial" Font-Size="8pt"
                                                    Text="Label" Width="118px"></asp:Label></td>
                                            <td style="text-align: left" bgcolor="#ccffcc" width="15%">
                                                <asp:Label ID="Label9" runat="server" Font-Names="Arial" Font-Size="8pt" Text="Fec. Real Servicio" Width="89px"></asp:Label></td>
                                            <td style="text-align: left" bgcolor="#ffffd9" width="40%">
                                                <asp:TextBox ID="TxtFecReal" runat="server" Font-Names="Arial" Font-Size="8pt" TabIndex="2"
                                                    Width="70px" Enabled="False"></asp:TextBox></td>

                                            <td rowspan="4"></td>

                                        </tr>
                                        <tr>
                                            <td style="text-align: left; height: 24px;" bgcolor="#ccffcc" width="15%">
                                                <asp:Label ID="Label2" runat="server" Text="Revisión" Font-Names="Arial" Font-Size="8pt"></asp:Label></td>
                                            <td style="text-align: left; height: 24px;" bgcolor="#ffffd9" width="40%">
                                                <asp:TextBox ID="TxtRevision" runat="server" Enabled="False" Font-Names="Arial" Font-Size="8pt"
                                                    TabIndex="2" Width="70px"></asp:TextBox></td>
                                            <td style="text-align: left; height: 24px;" bgcolor="#ccffcc" width="15%">
                                                <asp:Label ID="Label3" runat="server" Text="ORT" Font-Names="Arial" Font-Size="8pt"></asp:Label></td>
                                            <td style="text-align: left; height: 24px;" bgcolor="#ffffd9" width="40%">
                                                <asp:TextBox ID="TxtOrt" runat="server" Font-Names="Arial" Font-Size="8pt" TabIndex="1" Width="68px" Enabled="False"></asp:TextBox>
                                                &nbsp;
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="width: 51px; text-align: left; height: 24px;" bgcolor="#ccffcc">
                                                <asp:Label ID="Label5" runat="server" Text="Kilometros" Font-Names="Arial" Font-Size="8pt"></asp:Label></td>
                                            <td style="text-align: left; height: 24px; width: 125px;" bgcolor="#ffffd9">
                                                <asp:TextBox ID="TxtKM" runat="server" Font-Names="Arial" Font-Size="8pt" TabIndex="2" Width="70px" Enabled="False"></asp:TextBox></td>
                                            <td style="text-align: left; height: 24px; width: 36px;" bgcolor="#ccffcc">
                                                <asp:Label ID="Label6" runat="server" Text="Interno" Font-Names="Arial" Font-Size="8pt"></asp:Label></td>
                                            <td style="text-align: left; width: 134px; height: 24px;" bgcolor="#ffffd9">
                                                <asp:RadioButtonList ID="TxtInterno" runat="server" Font-Names="Arial" Font-Size="8pt" RepeatDirection="Horizontal" TabIndex="3" BorderWidth="0px" CellPadding="0" CellSpacing="0" Enabled="False">
                                                    <asp:ListItem Value="0">No</asp:ListItem>
                                                    <asp:ListItem Value="0">Si</asp:ListItem>
                                                </asp:RadioButtonList></td>
                                        </tr>


                                        <tr>
                                            <td style="width: 51px; text-align: left; height: 24px;" bgcolor="#ccffcc">
                                                <asp:Label ID="Label11" runat="server" Text="Cita" Font-Names="Arial"
                                                    Font-Size="8pt"></asp:Label></td>
                                            <td style="text-align: left; height: 24px; width: 125px;" bgcolor="#ffffd9">
                                                <asp:RadioButtonList ID="TxtCita" runat="server" Font-Names="Arial"
                                                    Font-Size="8pt" RepeatDirection="Horizontal" TabIndex="3" BorderWidth="0px"
                                                    CellPadding="0" CellSpacing="0" Enabled="False" Height="20px"
                                                    Style="margin-left: 0px" Width="104px">
                                                    <asp:ListItem Value="s">si</asp:ListItem>
                                                    <asp:ListItem Value="n">no</asp:ListItem>
                                                </asp:RadioButtonList></td>
                                            <td style="text-align: left; height: 24px; width: 36px;" bgcolor="#ccffcc">
                                                <asp:Label ID="Label16" runat="server" Text="¿Utilizó el Servicio de Taller Móvil? " Font-Names="Arial" Font-Size="8pt"></asp:Label>
                                            </td>
                                            <td style="text-align: left; height: 24px;" bgcolor="#ffffd9">
                                                <asp:RadioButtonList ID="TxtServMovil" runat="server" Font-Names="Arial"
                                                    Font-Size="8pt" RepeatDirection="Horizontal" TabIndex="6" BorderWidth="0px"
                                                    CellPadding="0" CellSpacing="0" Enabled="false">
                                                    <asp:ListItem Value="s">SI</asp:ListItem>
                                                    <asp:ListItem Value="n">NO</asp:ListItem>
                                                </asp:RadioButtonList>
                                            </td>

                                        </tr>
                                        <tr>
                                            <td style="text-align: left; height: 24px; width: 36px;" bgcolor="#ccffcc">
                                                <asp:Label ID="Label12" runat="server" Text="¿Utilizó algún servicio de retiro o entrega?" Font-Names="Arial" Font-Size="8pt"></asp:Label></td>
                                            <td style="text-align: left; width: 134px; height: 24px;" bgcolor="#ffffd9">
                                                <asp:RadioButtonList ID="TxtServicio" runat="server" Font-Names="Arial"
                                                    Font-Size="8pt" RepeatDirection="Horizontal" TabIndex="6" BorderWidth="0px"
                                                    CellPadding="0" CellSpacing="0" Enabled="False">
                                                    <asp:ListItem Value="s">SI</asp:ListItem>
                                                    <asp:ListItem Value="n">NO</asp:ListItem>
                                                </asp:RadioButtonList></td>
                                            <td style="width: 69px; text-align: left;" bgcolor="#ccffcc">
                                                <asp:Label ID="Label14" runat="server" Text="Retiro/Entrega a Domicilio" Font-Names="Arial"
                                                    Font-Size="8pt"></asp:Label></td>
                                            <td style="text-align: left; height: 24px;" bgcolor="#ffffd9">Retiro<asp:CheckBox ID="ChkRetiro" runat="server" Enabled="false" Font-Size="8pt" />
                                                Entrega<asp:CheckBox ID="ChkEntrega" runat="server" Enabled="false" Font-Size="8pt" />

                                            </td>

                                        </tr>
                                        <tr>
                                            <td style="text-align: left; height: 24px; width: 36px;" bgcolor="#ccffcc">
                                                <asp:Label ID="Label15" runat="server" Text="¿Qué proveedor hizo el servicio de retiro/entrega? " Font-Names="Arial" Font-Size="8pt"></asp:Label>
                                            </td>
                                            <td style="text-align: left; height: 24px;" bgcolor="#ffffd9">
                                                <asp:RadioButtonList ID="TxtProveedor" runat="server" Font-Names="Arial"
                                                    Font-Size="8pt" RepeatDirection="Horizontal" TabIndex="6" BorderWidth="0px"
                                                    CellPadding="0" CellSpacing="0" Enabled="False">
                                                    <asp:ListItem Value="A">Auxilia</asp:ListItem>
                                                    <asp:ListItem Value="O">Otros</asp:ListItem>
                                                </asp:RadioButtonList></td>
                                            <td style="text-align: left; height: 24px; width: 36px;" bgcolor="#ccffcc">
                                                <asp:Label ID="Label13" runat="server" Text="Nombre Asesor" Font-Names="Arial" Font-Size="8pt"></asp:Label>
                                            </td>
                                            <td style="text-align: left; height: 24px;" bgcolor="#ffffd9">
                                                <asp:TextBox ID="txtAsesor" runat="server" Font-Names="Arial" Font-Size="8pt" TabIndex="1" Width="130px" Enabled="False"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="text-align: left; height: 24px; width: 36px;" bgcolor="#ccffcc">
                                                <asp:Label ID="Label19" runat="server" Text="¿ Realizó servicio Airlife?" Font-Names="Arial" Font-Size="8pt"></asp:Label>
                                            </td>
                                            <td style="text-align: left; height: 24px;" bgcolor="#ffffd9">
                                                <asp:CheckBox ID="chkLavadoSustentable" runat="server" Text="Si" Enabled="False" />
                                            </td>
                                            <td style="text-align: left; width: 36px;" bgcolor="#ccffcc">&nbsp;</td>
                                            <td style="text-align: left;" bgcolor="#ffffd9">&nbsp;</td>
                                        </tr>
                                        <tr>
                                            <td style="text-align: left; height: 24px; width: 36px;" bgcolor="#ccffcc">
                                                <asp:Label ID="Label20" runat="server" Text="¿ Es Hibrido ?" Font-Names="Arial" Font-Size="8pt"></asp:Label>
                                            </td>
                                            <td style="text-align: left; height: 24px;" bgcolor="#ffffd9">
                                                <asp:CheckBox ID="chkHibrido" runat="server" Text="Si" Enabled="False" />
                                            </td>
                                            <td style="text-align: left; width: 36px;" bgcolor="#ccffcc">
                                                <asp:Label ID="lblEtiAppMundoToyota" runat="server" Text="APP MUNDO TOYOTA - Cliente Cobra Premio" Font-Names="Arial" Font-Size="8pt"></asp:Label>
                                            </td>
                                            <td style="text-align: left;" bgcolor="#ffffd9">
                                                <asp:CheckBox ID="chkiAppMundoToyota" runat="server" Text="Si" Enabled="False" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="text-align: left; height: 24px; width: 36px;" bgcolor="#ccffcc">
                                                <asp:Label ID="Label21" runat="server" Text="¿ Unidad posee anclaje de Piso Seguro? " Font-Names="Arial" Font-Size="8pt"></asp:Label>
                                            </td>
                                            <td style="text-align: left; height: 24px;" bgcolor="#ffffd9">
                                                <asp:CheckBox ID="chkAnclajePiso" runat="server" Text="Si" AutoPostBack="True" Enabled="False" />
                                            </td>
                                            <td style="text-align: left; width: 36px;" bgcolor="#ccffcc">
                                                <asp:Label ID="lblPisoInstaladoOK" runat="server" Text="¿ Piso correctamente instalado? " Font-Names="Arial" Font-Size="8pt"></asp:Label>
                                            </td>
                                            <td style="text-align: left;" bgcolor="#ffffd9">
                                                <asp:RadioButtonList ID="TxtPisoInstaladoOK" runat="server" Font-Names="Arial"
                                                    Font-Size="8pt" RepeatDirection="Horizontal" TabIndex="6" BorderWidth="0px"
                                                    CellPadding="0" CellSpacing="0" AutoPostBack="True"
                                                    OnSelectedIndexChanged="TxtServicio_SelectedIndexChanged" Enabled="False">
                                                    <asp:ListItem Value="s">SI</asp:ListItem>
                                                    <asp:ListItem Value="n">NO</asp:ListItem>
                                                </asp:RadioButtonList></td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
            <br />
            <input id="Button3" style="font-weight: bold; font-size: 9pt; font-family: Arial"
                type="button" value="<<< Atras" onclick="history.back();" />
            <asp:Button ID="Button1" runat="server" Font-Bold="True" Font-Names="Arial" Font-Size="9pt"
                TabIndex="14" Text="Confirmar >>>" ValidationGroup="grupoMantencion" PostBackUrl="~/Registro_Mantenciones/Reg_Mantencion_04.aspx" OnClick="Button1_Click" /><br />
            <br />
            <c2:Firma ID="firma1" runat="server"></c2:Firma>
                <asp:Image ID="imgSi" runat="server" ImageUrl="~/Imagenes/Semaforo_Verde.JPG" Visible="False" />
                <asp:Image ID="imgWarning" runat="server" ImageUrl="~/Imagenes/Semaforo_Amarillo.JPG" Visible="False" />
                <asp:Image ID="imgNo" runat="server" ImageUrl="~/Imagenes/Semaforo_Rojo.JPG" Visible="False" />
                <asp:HiddenField ID="hdnFecVenta" runat="server" />
                <asp:HiddenField ID="hdnUltimoKilometraje" runat="server" />
                <asp:HiddenField ID="hdnUltimaMantencion" runat="server" />
                <asp:HiddenField ID="hdnChecklistPDF" runat="server" />
                <asp:HiddenField ID="hdnAntiguedadAños" runat="server" />
                <asp:HiddenField ID="hdnAntiguedadMeses" runat="server" />
                <asp:HiddenField ID="hdnAntiguedadDias" runat="server" />
                <asp:HiddenField ID="hdnFechaActivacionT10" runat="server" />
                <asp:HiddenField ID="hdnConcesionarioT10" runat="server" />
                <asp:HiddenField ID="hdnNumeroMantencionT10" runat="server" />
                <asp:HiddenField ID="hdnFechaDesactivacionT10" runat="server" />
                <asp:HiddenField ID="hdnUltimoKilometrajeSQL" runat="server" />
                <asp:HiddenField ID="hdnFechaUltimaMantencionSQL" runat="server" />
                <asp:HiddenField ID="hdnDesactivarProgramaT10" runat="server" />
            &nbsp;
        </div>
    </form>
</body>
</html>
