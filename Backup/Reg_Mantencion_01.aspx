<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Reg_Mantencion_01.aspx.cs" Inherits="Registro_Mantenciones_Reg_Mantencion_01" %>

<%@ Register TagPrefix="cl" TagName="Cliente" Src="~/Include/Tabla_Cliente_01.ascx" %>
<%@ Register TagPrefix="c2" TagName="Firma" Src="~/Include/firma.ascx" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xml:lang="es" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Registro Mantenciones - 01</title>
</head>
<body topmargin="0">
    <table width="800px" height="600px" bordercolor="#F3F3F3" border="1" cellpadding="0" cellspacing="0" align="center"
        background="../../Desarrollo/Seguridad%20Toyota%20Avanza/Imagenes/byFelipe/fondo2.jpg">
        <caption></caption>
        <tr>
            <th></th>
        </tr>
        <tr>
            <td valign="top">
                <form id="form1" runat="server">
                    <div style="text-align: center">
                        <asp:Label ID="Label1" runat="server" Font-Bold="True" Font-Names="Arial" Font-Size="12pt"
                            Font-Underline="True" Text="Registro de Mantenciones &lt;br&gt;&lt;font size=1&gt;(Paso 1)&lt;/font&gt;"></asp:Label><br />
                        <asp:Label
                            ID="Label4" runat="server" Font-Bold="True" Font-Names="Arial" Font-Size="7pt"
                            ForeColor="#CC0000" Text="Label" Width="186px"></asp:Label><br />
                        <br />
                        <cl:Cliente ID="LinkId"
                            runat="server"></cl:Cliente>
                        &nbsp;&nbsp;<br />
                        <br />
                        <table width="50%" align="center">
                            <caption></caption>
                            <tr>
                                <th></th>
                            </tr>
                            <tr>
                                <td style="height: 24px; text-align: left" width="20%">
                                    <asp:Label ID="Label2" runat="server" Text="R.U.T." Font-Names="Arial" Font-Size="8pt"></asp:Label>
                                </td>
                                <td style="height: 24px; text-align: left; font-size: 12pt; font-family: Times New Roman;">
                                    <asp:TextBox ID="TxtRut" MaxLength="10" TabIndex="1" runat="server" Font-Size="8pt" Font-Names="Arial" onblur="this.style.backgroundColor='#ffffff'" onfocus="this.style.backgroundColor='#ffff00'">
                                    </asp:TextBox><asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="TxtRut"
                                        Display="Dynamic" ErrorMessage="Rut es obligatorio.<br>" Font-Names="Arial" Font-Size="7pt"
                                        SetFocusOnError="True" Width="4px">*</asp:RequiredFieldValidator><asp:RegularExpressionValidator
                                            ID="RegularExpressionValidator1" runat="server" ControlToValidate="TxtRut" Display="Dynamic"
                                            ErrorMessage="Formato Rut no valido (Ej: 10981017-K).<br>" Font-Names="Arial"
                                            Font-Size="7pt" SetFocusOnError="True" ValidationExpression="^\d{7,8}[-][0-9kK]{1}$" Width="4px">*</asp:RegularExpressionValidator><asp:CustomValidator ID="MiValidadorRUT" runat="server" ControlToValidate="TxtRut" Display="Dynamic"
                                                ErrorMessage="Rut ingresado no es valido.<br>" Font-Names="Arial" Font-Size="7pt"
                                                OnServerValidate="ValidaRut2" Width="4px">*</asp:CustomValidator><asp:CustomValidator
                                                    ID="MiValidadorRUT2" runat="server" ControlToValidate="TxtRut" Display="Dynamic"
                                                    ErrorMessage="Rut ingresado no es valido.<br>" Font-Names="Arial" Font-Size="7pt"
                                                    OnServerValidate="ValidaRut3" Width="4px">*</asp:CustomValidator></td>
                            </tr>
                            <tr style="font-size: 12pt; font-family: Times New Roman">
                                <td style="text-align: left; height: 24px;">
                                    <asp:Label ID="Label3" runat="server" Text="N° Stock" Font-Names="Arial" Font-Size="8pt"></asp:Label></td>
                                <td style="text-align: left; height: 24px;">
                                    <asp:TextBox ID="TxtStock" runat="server" Font-Names="Arial" Font-Size="8pt" TabIndex="2" CausesValidation="True" MaxLength="10" onblur="this.style.backgroundColor='#ffffff';rellenar(this,this.value);" onfocus="this.style.backgroundColor='#ffff00'" AutoPostBack="True" OnTextChanged="TxtStock_TextChanged"></asp:TextBox><asp:RequiredFieldValidator
                                        ID="RequiredFieldValidator1" runat="server" ControlToValidate="TxtStock" Display="Dynamic"
                                        ErrorMessage="Stock es obligatorio.<br>" Font-Names="Arial" Font-Size="4pt"
                                        SetFocusOnError="True" Width="4px">*</asp:RequiredFieldValidator><asp:CustomValidator
                                            ID="ValidaStockReg" runat="server" ControlToValidate="TxtStock" Display="Dynamic" Font-Names="Arial" Font-Size="4pt"
                                            OnServerValidate="Valida_Stock2" SetFocusOnError="True" Width="4px">*</asp:CustomValidator></td>
                            </tr>
                            <tr style="font-size: 12pt; font-family: Times New Roman">
                                <td colspan="2" style="height: 56px">
                                    <!--spoiler ayuda-->

                                    <div align="center">
                                        <input type="button" class="boton" value="Ayuda" style="width: 75px; font-size: 10px; margin: 0px; padding: 0px;" onclick="if (this.parentNode.parentNode.getElementsByTagName('div')[1].getElementsByTagName('div')[0].style.display != '') { this.parentNode.parentNode.getElementsByTagName('div')[1].getElementsByTagName('div')[0].style.display = '';this.innerText = ''; this.value = 'Esconder'; } else { this.parentNode.parentNode.getElementsByTagName('div')[1].getElementsByTagName('div')[0].style.display = 'none'; this.innerText = ''; this.value = 'Ayuda'; }" />
                                    </div>
                                    <div class="alt2" style="margin: 1px; padding: 0px; border: 0px inset;">
                                        <div style="display: none;">
                                            <div style="margin: 20px; margin-top: 5px;">
                                                <table cellpadding="3" cellspacing="0" border="0" width="100%">
                                                    <caption></caption>
                                                    <tr>
                                                        <th></th>
                                                    </tr>
                                                    <tr>
                                                        <td class="alt2" style="border: 1px inset">
                                                            <asp:Label ID="LblAyuda" runat="server" Font-Names="Arial" Font-Size="8pt" ForeColor="#000066"
                                                                Height="100%" Text="AQUI VA LA AYUDA DINAMICA" Width="100%"></asp:Label>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </div>
                                        </div>
                                        <!--fin ayuda-->
                                        <br />
                                        <asp:Button ID="Button1" TabIndex="4" OnClick="Button1_Click1" runat="server" Text="Continuar >>>" Font-Size="9pt" Font-Names="Arial" Font-Bold="True"></asp:Button><asp:Button ID="Button3" TabIndex="5" runat="server" Text="Menu Inicial" Font-Size="9pt" Font-Names="Arial" Font-Bold="True" CausesValidation="False" PostBackUrl="~/Default.aspx"></asp:Button>
                                        <asp:TextBox ID="TxtInicio" runat="server" Visible="False" Width="1px" Font-Names="Arial"> M</asp:TextBox>
                                </td>
                            </tr>
                        </table>
                        <br />
                        <asp:ValidationSummary ID="valSumario" runat="server" DisplayMode="SingleParagraph"
                            Font-Names="Verdana" Font-Size="10pt" HeaderText="<div align=center><u>Se han encontrado los siguientes errores</u>: </div>"
                            ShowMessageBox="False" ShowSummary="True" />
                        <br />
                        <br />
                        <c2:Firma ID="firma1" runat="server"></c2:Firma>
                    </div>
                </form>
            </td>
        </tr>
    </table>
</body>
</html>
