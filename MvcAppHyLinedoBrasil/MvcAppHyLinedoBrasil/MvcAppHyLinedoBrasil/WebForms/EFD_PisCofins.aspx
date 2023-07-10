<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EFD_PisCofins.aspx.cs"
    Inherits="MvcAppHyLinedoBrasil.WebForms.EFD_PisCofins" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Ajuste - EFD Pis Cofins</title>
    <link href="../Content/icons/logo_hyline.ico" rel="Shortcut Icon" type="text/css" />
    <style type="text/css">
        .style14
        {
            width: 785px;
            height: 125px;
        }
        .style20
        {
            width: 915px;
            left: 50%;
            text-align: center;
        }
        .style22
        {
            width: 387px;
            text-align: center;
        }
        .panel
        {
            padding: 30px 30px 15px 30px;
            background-color: #fff;
            margin-bottom: 30px;
            _height: 1px; /* only IE6 applies CSS properties starting with an underscore */
        }
        .main
        {
            padding: 30px 30px 15px 30px;
            background-color: #fff;
            margin-bottom: 30px;
            _height: 1px; /* only IE6 applies CSS properties starting with an underscore */
        }
        .style23
        {
            width: 270px;
            height: 104px;
        }
        </style>
</head>
<body style="background-color: #5c87b2; font-family: Verdana, Tahoma, Arial, Helvetica Neue, Helvetica, Sans-Serif;">
    <form id="form1" runat="server">
    <div style="text-align: center;" class="panel">
        <table style="width: 1002px; height: 66px;">
            <tr>
                <td rowspan="2">
                    <a href="../Home/Index">
                        <img alt="" class="style23" src="../Content/images/logo.png" border="0" /></a>
                </td>
            </tr>
            <tr>
                <td>
                </td>
                <td class="style14">
                    <asp:Label ID="Label5" runat="server" Font-Bold="True" Font-Size="XX-Large" Font-Underline="False"
                        Text="AJUSTE - EFD PIS COFINS"></asp:Label>
                </td>
            </tr>
        </table>
        <asp:Panel ID="Panel1" runat="server" Width="1003px" HorizontalAlign="Center">
            <table align="center" style="height: 235px; width: 994px;">
                <tr>
                    <td class="style20">
                        <asp:HyperLink ID="HyperLink1" runat="server" 
                            NavigateUrl="~/WebForms/EFD_PisCofins_Nfs_Saida.aspx" 
                            style="font-weight: 700; font-size: xx-large">BLOCO C / D</asp:HyperLink>
                    </td>
                    <td class="style22">
                        <asp:HyperLink ID="HyperLink2" runat="server" 
                            style="font-weight: 700; font-size: xx-large" 
                            NavigateUrl="~/WebForms/EFD_PisCofins_F130.aspx">BLOCO F130</asp:HyperLink>
                    </td>
                </tr>
            </table>
        </asp:Panel>
    </div>
    </form>
</body>
</html>
