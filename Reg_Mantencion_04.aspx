<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Reg_Mantencion_04.aspx.cs" Inherits="Registro_Mantenciones_Reg_Mantencion_04" %>

<%@ Register TagPrefix="cl" TagName="Cliente" Src="~/Include/Tabla_Cliente_01.ascx" %>
<%@ Register TagPrefix="c2" TagName="Firma" Src="~/Include/firma.ascx" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xml:lang="es" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Registro Mantenciones - 04</title>

    <script type="text/javascript" language="javascript">
        function printPage() {
            var newWin = window.open('printer', 'window', 'letf=0,top=0,width=400,height=400,toolbar=0,scrollbars=0,status=0');
            var titleHTML = document.getElementById("printdiv").innerHTML;
            newWin.document.write(titleHTML);
            newWin.document.location.reload();
            newWin.print();

            newWin.close();
        }
    </script>

</head>
<body topmargin="0">
    <form id="form1" runat="server">
        <div style="text-align: center">
            <asp:Label ID="Label1" runat="server" Font-Bold="True" Font-Names="Arial" Font-Size="14pt"
                Font-Underline="True" Text="Registro de Mantenciones  (Paso 4)"></asp:Label><br />
            <asp:Label
                ID="Label4" runat="server" Font-Bold="True" Font-Names="Arial" Font-Size="7pt"
                ForeColor="#CC0000" Text="Label" Width="186px"></asp:Label><br />
            <br />
            <cl:Cliente ID="LinkId"
                runat="server"></cl:Cliente>
            <br />
            <br />
            <div align="center" id="printdiv">
                <div align="center">
                    <asp:Label ID="Label2" runat="server" Text="Felicitaciones" Width="300px" Font-Names="Arial" Font-Size="14pt" ForeColor="#000099"></asp:Label><br />
                    <asp:Label ID="Label3" runat="server" Text="Transacción Completa" Width="300px" Font-Names="Arial" Font-Size="14pt" Font-Underline="True" ForeColor="#000099"></asp:Label>
                    <br />
                </div>

                <table align="center" border="1" cellpadding="0" cellspacing="0" bordercolor="#000099" style="width: 460px">
                    <caption></caption>
                    <tr>
                        <th>
                            <asp:Label ID="lbldebug" runat="server" Font-Bold="True" Font-Names="Arial" Font-Size="10pt"></asp:Label>
                            </th>
                    </tr>
                    <tr bgcolor="#ccff99">
                        <td style="text-align: left; width: 357px; height: 28px;" valign="top">&nbsp;<asp:Label ID="Label5" runat="server" Font-Bold="True" Font-Names="Arial" Font-Size="10pt"
                            Text="Descripción de Transacción"></asp:Label></td>

                    </tr>
                    <tr>
                        <td style="height: 324px; text-align: left; width: 357px;" valign="top">
                            <br />
                            <asp:Label ID="Label6" runat="server" Font-Names="Arial" Font-Size="9pt" Text="El cliente que se individualiza a continuación:"
                                Width="351px"></asp:Label>
                            <br />
                            <br />
                            <table border="0">
                                <caption></caption>
                                <tr>
                                    <th></th>
                                </tr>
                                <tr>
                                    <td style="height: 16px; text-align: left" width="20%">
                                        <asp:Label ID="Label7" runat="server" Font-Names="Arial" Font-Size="9pt" Text="R.U.T."
                                            Width="26px"></asp:Label></td>
                                    <td style="height: 16px; text-align: left" width="80%">
                                        <asp:Label ID="LblRut" runat="server" Font-Names="Arial" Font-Size="9pt" Text="Rut"
                                            Width="440px"></asp:Label></td>
                                </tr>
                                <tr>
                                    <td style="text-align: left">
                                        <asp:Label ID="Label8" runat="server" Font-Names="Arial" Font-Size="9pt" Text="Nombre"
                                            Width="52px"></asp:Label></td>
                                    <td style="text-align: left">
                                        <asp:Label ID="LblNOm" runat="server" Font-Names="Arial" Font-Size="9pt" Text="Nombre"
                                            Width="440px"></asp:Label></td>
                                </tr>

                            </table>
                            <br />
                            <asp:Label ID="LblTexto" runat="server" Font-Names="Arial" Font-Size="9pt"
                                Text="Texto Generado dinamicamente" Width="455px"></asp:Label>

                            <br />
                            <table border="0" width="60%">
                                <caption></caption>
                                <tr>
                                    <th></th>
                                </tr>
                                <tr>
                                    <td style="text-align: left" height="40%">
                                        <asp:Label ID="Label11" runat="server" Font-Names="Arial" Font-Size="9pt" Text="Con código de garantía:"
                                            Width="135px"></asp:Label></td>
                                    <td style="width: 60%; text-align: left">
                                        <asp:Label ID="LblCodigo" runat="server" Font-Bold="True" Font-Names="Arial" Font-Size="9pt"
                                            Text="codigo garantia" Width="149px"></asp:Label></td>
                                </tr>
                            </table>

                            <br />
                            <table border="0" align="center" width="100%">
                                <caption></caption>
                                <tr>
                                    <th></th>
                                </tr>
                                <tr>
                                    <td colspan="2" style="height: 26px" valign="top">
                                        <asp:Label ID="Label13" runat="server" Font-Names="Arial" Font-Size="9pt" Text="Datos para la proxima mantención."
                                            Width="257px" Font-Underline="True"></asp:Label></td>

                                </tr>
                                <tr>
                                    <td style="width: 186px">
                                        <asp:Label ID="Label9" runat="server" Font-Names="Arial" Font-Size="9pt" Text="Número"></asp:Label></td>
                                    <td>
                                        <asp:Label ID="LblRevProx" runat="server" Font-Names="Arial" Font-Size="9pt"
                                            Text="revprox" Width="116px" Font-Bold="True"></asp:Label></td>
                                </tr>
                                <tr>
                                    <td style="width: 186px">
                                        <asp:Label ID="Label10" runat="server" Font-Names="Arial" Font-Size="9pt" Text="Fecha Aprox."></asp:Label></td>
                                    <td>
                                        <asp:Label ID="LblFecRevProx" runat="server" Font-Bold="True" Font-Names="Arial" Font-Size="9pt"
                                            Text="Fecrevprox" Width="118px"></asp:Label></td>
                                </tr>

                                <tr>
                                    <td style="width: 186px">
                                        <asp:Label ID="Label17" runat="server" Font-Names="Arial" Font-Size="9pt"
                                            Text="Cita"></asp:Label></td>
                                    <td>
                                        <asp:Label ID="LblCita" runat="server" Font-Bold="True" Font-Names="Arial" Font-Size="9pt"
                                            Text="Cita" Width="118px"></asp:Label></td>
                                </tr>

                            </table>


                            <br />
                            <asp:HyperLink ID="HyperLink1" runat="server" Font-Names="Arial" Font-Size="9pt"
                                Target="_blank" Font-Bold="True" Font-Underline="True" ForeColor="#CC0033">Ver Ficha</asp:HyperLink><br />
                            <br />
                            <asp:Label ID="Label16" runat="server" Font-Names="Arial" Font-Size="9pt" Font-Underline="True"
                                Text="*Guarde el codigo de garantia." Width="180px"></asp:Label><br />
                            <table border="0" width="100%" align="right">
                                <caption></caption>
                                <tr>
                                    <th></th>
                                </tr>
                                <tr>
                                    <td style="width: 362px; height: 144px;">
                                        <div align="right" style="text-align: right">
                                            <asp:Image ID="Image1" runat="server" Height="142px" ImageUrl="~/Imagenes/certifica.jpg"
                                                Width="160px" />
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </td>

                    </tr>
                </table>
            </div>

            <table align="center" width="460">
                <caption></caption>
                <tr>
                    <th></th>
                </tr>
                <tr>
                    <td style="height: 25px; text-align: center;">

                        <input id="Button4" onclick="printPage()" style="font-weight: bold; font-size: 8pt; font-family: Arial"
                            type="button" value="Imprimir Documento" size="10" />

                    </td>
                    <td style="height: 25px; text-align: center; width: 260px;">
                        <asp:Button ID="Button1" TabIndex="4" runat="server" Text="Nueva Mantención" Font-Size="8pt" Font-Names="Arial" Font-Bold="True" PostBackUrl="~/Registro_Mantenciones/Reg_Mantencion_01.aspx"></asp:Button></td>
                    <td style="height: 25px; text-align: center; width: 260px;">
                        <asp:Button ID="Button5" TabIndex="4" runat="server" Text="Nuevo Trab. Gral." Font-Size="8pt" Font-Names="Arial" Font-Bold="True" PostBackUrl="~/Registro_Trabajo_General/Reg_Trabajo_General_01.aspx"></asp:Button></td>
                    <td style="height: 25px; text-align: center; width: 260px;">
                        <asp:Button ID="Button7" TabIndex="4" runat="server" Text="Nuevo DYP"
                            Font-Size="8pt" Font-Names="Arial" Font-Bold="True"
                            PostBackUrl="~/Registro_DYP/Registro_DYP_01.aspx" OnClick="Button7_Click"></asp:Button></td>
                    <td style="height: 25px; text-align: center;">
                        <asp:Button ID="Button8" TabIndex="5" runat="server" Text="Menu Inicial" Font-Size="8pt" Font-Names="Arial" Font-Bold="True" CausesValidation="False" PostBackUrl="~/Default.aspx" Height="23px" Width="98px"></asp:Button></td>
                </tr>
            </table>


            <br />
            <c2:Firma ID="firma1" runat="server"></c2:Firma>
        </div>
    </form>
</body>
</html>
