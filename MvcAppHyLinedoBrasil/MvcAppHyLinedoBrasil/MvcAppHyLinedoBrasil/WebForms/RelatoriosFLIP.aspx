<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RelatoriosFLIP.aspx.cs" Inherits="MvcAppHyLinedoBrasil.WebForms.RelatoriosFLIP" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Relatórios - FLIP</title>
    <%--<link href="../Content/icons/logo_hyline.ico" rel="Shortcut Icon" type="text/css" />--%>
    <style type="text/css">
        .style14
        {
            width: 785px;
            height: 125px;
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
        .style25
        {
            width: 511px;
            left: 50%;
            text-align: center;
            height: 54px;
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
                    <%--<img alt="" class="style23" src="../Content/images/logo.png" border="0" />--%>
                        <asp:Image ID="Image1" runat="server" 
                        ImageUrl="~/Content/images/Logo_EW.png" />
                        </a>
                    <asp:HyperLink ID="hlBackHome" runat="server" NavigateUrl="../Home/Index">Voltar para Home</asp:HyperLink>
                </td>
            </tr>
            <tr>
                <td>
                </td>
                <td class="style14">
                    <asp:Label ID="Label5" runat="server" Font-Bold="True" Font-Size="XX-Large" Font-Underline="False"
                        Text="RELATÓRIOS - FLIP"></asp:Label>
                </td>
            </tr>
        </table>
        <asp:Panel ID="Panel1" runat="server" Width="1003px" HorizontalAlign="Center">
            <table align="center" style="height: 0px; width: 994px;">
                <tr>
                    <td class="style25">
                        <asp:Image ID="imgRelDiarioCompleto" runat="server" ImageUrl="~/Content/images/kjots.png" />
                        &nbsp;
                        <asp:HyperLink ID="hplRelDiarioCompleto" runat="server" 
                            NavigateUrl="~/WebForms/RelDiarioProducao.aspx" 
                            style="font-family: Arial, Helvetica, sans-serif; font-weight: 700">Relatório Diário Completo</asp:HyperLink>
                    </td>
                    <td class="style25">
                        <asp:Image ID="imgControleQualidadeCargas" runat="server" ImageUrl="~/Content/images/kjots.png" />
                        &nbsp;
                        <asp:HyperLink ID="hplControleQualidadeCargas" runat="server" 
                            NavigateUrl="~/WebForms/ControleQualidadeCargasPintos.aspx" 
                            style="font-family: Arial, Helvetica, sans-serif; font-weight: 700">Composição dos Lotes dos Clientes</asp:HyperLink>
                        
                    </td>
                </tr>
                <tr>
                    <td class="style25">
                        
                        <asp:Image ID="imgRelDEOGranja" runat="server" EnableTheming="True" 
                            ImageUrl="~/Content/images/kjots.png" />
                        &nbsp;
                        <asp:HyperLink ID="hplRelDEOGranja" runat="server" 
                            NavigateUrl="~/WebForms/RelDEOGranja.aspx" 
                            style="font-family: Arial, Helvetica, sans-serif; font-weight: 700">Relatório de DEOS</asp:HyperLink>
                        
                    </td>
                    <td class="style25">                        
                        <asp:Image ID="imgRelEggInv" runat="server" ImageUrl="~/Content/images/kjots.png" />
                        &nbsp;
                        <asp:HyperLink ID="hplRelEggInv" runat="server" 
                            NavigateUrl="~/WebForms/RelEggInv.aspx" 
                            style="font-family: Arial, Helvetica, sans-serif; font-weight: 700">Relatório de Movimentações de Ovos</asp:HyperLink>
                        
                    </td>
                </tr>
                <tr>
                    <td class="style25">
                        <asp:Image ID="imgRelEstoqueOvos" runat="server" EnableTheming="True" 
                            ImageUrl="~/Content/images/kjots.png" />
                        &nbsp;
                        <asp:HyperLink ID="hplRelEstoqueOvos" runat="server" 
                            NavigateUrl="~/WebForms/RelEstoqueOvos.aspx" 
                            style="font-family: Arial, Helvetica, sans-serif; font-weight: 700">Relatório de Estoque de Ovos</asp:HyperLink>
                        
                    </td>
                    <td class="style25">                        
                        <asp:Image ID="imgRelConfIncWebXFlip" runat="server" ImageUrl="~/Content/images/kjots.png" />
                        &nbsp;
                        <asp:HyperLink ID="hplRelConfIncWebXFlip" runat="server" 
                            NavigateUrl="~/WebForms/RelConfIncWebXFlip.aspx" 
                            style="font-family: Arial, Helvetica, sans-serif; font-weight: 700">Relatórios de Conferência de Incubações</asp:HyperLink>
                        
                    </td>
                </tr>
                <tr>
                    <td class="style25">
                        
                        <asp:Image ID="imgRelRastreabilidade" runat="server" 
                            ImageUrl="~/Content/images/kjots.png" />
                        &nbsp;<asp:HyperLink ID="hplRelRastreabilidade" runat="server" 
                            NavigateUrl="~/WebForms/RelRastreabilidade.aspx" 
                            style="font-family: Arial, Helvetica, sans-serif; font-weight: 700">Relatório de Rastreabilidade</asp:HyperLink>
                        
                    </td>
                    <td class="style25">                        
                        <asp:Image ID="imgRelNascEmbrio" runat="server" ImageUrl="~/Content/images/kjots.png" />
                        &nbsp;
                        <asp:HyperLink ID="hplRelNascEmbrio" runat="server" 
                            NavigateUrl="~/WebForms/RelNascEmbrio.aspx" 
                            style="font-family: Arial, Helvetica, sans-serif; font-weight: 700">Relatório de Nascimento / Embriodiagnóstico</asp:HyperLink>
                        
                    </td>
                </tr>
            </table>
        </asp:Panel>
    </div>
    </form>
</body>
</html>
