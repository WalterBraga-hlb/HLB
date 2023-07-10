<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RelEstoqueOvos.aspx.cs" Inherits="MvcAppHyLinedoBrasil.WebForms.RelEstoqueOvos" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Relatório de Estoque de Ovos</title>
    <%--<link href="../Content/icons/logo_hyline.ico" rel="Shortcut Icon" type="text/css" />--%>
    <style type="text/css">
        .style14
        {
            width: 785px;
            height: 125px;
        }
        .style22
        {
            width: 387px;
            text-align: center;
            font-weight: 700;
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
        .style25
        {
            height: 48px;
        }
        .style26
        {
            font-size: xx-small;
            text-align: center;
        }
        .style27
        {
            font-size: xx-small;
            font-weight: bold;
        }
        .style29
        {
            width: 206px;
            text-align: center;
        }
        .style33
        {
            width: 177px;
            text-align: right;
        }
        .style34
        {
            width: 189px;
            left: 50%;
            text-align: right;
        }
    </style>
</head>
<body style="background-color: #5c87b2; font-family: Verdana, Tahoma, Arial, Helvetica Neue, Helvetica, Sans-Serif;">
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" AsyncPostBackTimeout="10000000">
    </asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div style="text-align: center;" class="panel">
                <table style="width: 1002px; height: 66px;">
                    <tr>
                        <td rowspan="2">
                            <a href="../Home/Index">
                            <asp:Image ID="Image2" runat="server" ImageUrl="~/Content/images/Logo_EW.png" /></a>
                        </td>
                    </tr>
                    <tr>
                        <td>
                        </td>
                        <td class="style14">
                            <asp:Label ID="Label5" runat="server" Font-Bold="True" Font-Size="XX-Large" Font-Underline="False"
                                Text="RELATÓRIO DE ESTOQUE DE OVOS"></asp:Label>
                        </td>
                    </tr>
                </table>
                <asp:Panel ID="Panel1" runat="server" Width="1003px" HorizontalAlign="Center">
                    <table align="center" style="height: 0px; width: 994px;">
                        <tr>
                            <td colspan="5" class="style25">
                                <asp:Label ID="lblTitulo" runat="server" Text="Informe os Parâmetros abaixo:" Style="font-weight: 700;
                                    text-decoration: underline;"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="5">
                                <asp:Label ID="lblmsg" runat="server" 
                                    Text="DADOS DISPONÍVEIS A PARTIR DE 01/06/2015!!!" Style="font-weight: 700;
                                    text-decoration: underline; color: #FF0000;"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                            </td>
                            <td>
                            </td>
                            <td>
                                <asp:Label ID="Label1" runat="server" Text="Incubatório:" CssClass="style27"></asp:Label>
                            </td>
                            <td>
                                <asp:DropDownList ID="ddlIncubatorio" runat="server" CssClass="style26" Height="20px"
                                    Width="180px">
                                    <asp:ListItem Value="PP">Matrizes - Nova Granada e Ajapi</asp:ListItem>
                                    <asp:ListItem Value="NM">Matrizes - Novo Mundo</asp:ListItem>
                                    <asp:ListItem Value="NMNF">Matrizes - Novo Mundo (Conf. NF)</asp:ListItem>
                                    <asp:ListItem Value="TB">Matrizes - Ajapi</asp:ListItem>
                                    <asp:ListItem Value="GP">Avós</asp:ListItem>
                                </asp:DropDownList>
                            </td>
                            <td>
                            </td>
                        </tr>
                        <tr>
                            <td class="style34">
                                <asp:Label ID="lblDataInicial" runat="server" Text="Data da Movimentação Inicial:" CssClass="style27"></asp:Label>
                            </td>
                            <td class="style29">
                                <asp:Calendar ID="calDataInicial" runat="server" BackColor="White" BorderColor="#3366CC"
                                    BorderWidth="1px" CellPadding="1" CssClass="style26" DayNameFormat="Shortest"
                                    Font-Names="Verdana" Font-Size="8pt" ForeColor="#003399" Height="128px" 
                                    Width="177px" SelectedDate="11/07/2013 10:51:10">
                                    <DayHeaderStyle BackColor="#99CCCC" ForeColor="#336666" Height="1px" />
                                    <NextPrevStyle Font-Size="8pt" ForeColor="#CCCCFF" />
                                    <OtherMonthDayStyle ForeColor="#999999" />
                                    <SelectedDayStyle BackColor="#009999" Font-Bold="True" ForeColor="#CCFF99" />
                                    <SelectorStyle BackColor="#99CCCC" ForeColor="#336666" />
                                    <TitleStyle BackColor="#003399" BorderColor="#3366CC" BorderWidth="1px" Font-Bold="True"
                                        Font-Size="10pt" ForeColor="#CCCCFF" Height="25px" />
                                    <TodayDayStyle BackColor="#99CCCC" ForeColor="White" />
                                    <WeekendDayStyle BackColor="#CCCCFF" />
                                </asp:Calendar>
                            </td>
                            <td class="style33">
                                <asp:Label ID="lblDataFinal0" runat="server" CssClass="style27" Text="Data da Movimentação Final:"></asp:Label>
                            </td>
                            <td>
                                <asp:Calendar ID="calDataFinal" runat="server" BackColor="White" BorderColor="#3366CC"
                                    BorderWidth="1px" CellPadding="1" CssClass="style26" DayNameFormat="Shortest"
                                    Font-Names="Verdana" Font-Size="8pt" ForeColor="#003399" Height="150px" 
                                    Width="175px" SelectedDate="11/07/2013 10:51:26">
                                    <DayHeaderStyle BackColor="#99CCCC" ForeColor="#336666" Height="1px" />
                                    <NextPrevStyle Font-Size="8pt" ForeColor="#CCCCFF" />
                                    <OtherMonthDayStyle ForeColor="#999999" />
                                    <SelectedDayStyle BackColor="#009999" Font-Bold="True" ForeColor="#CCFF99" />
                                    <SelectorStyle BackColor="#99CCCC" ForeColor="#336666" />
                                    <TitleStyle BackColor="#003399" BorderColor="#3366CC" BorderWidth="1px" Font-Bold="True"
                                        Font-Size="10pt" ForeColor="#CCCCFF" Height="25px" />
                                    <TodayDayStyle BackColor="#99CCCC" ForeColor="White" />
                                    <WeekendDayStyle BackColor="#CCCCFF" />
                                </asp:Calendar>
                            </td>
                            <td class="style22" rowspan="2">
                                <asp:Button ID="btnGerar" runat="server" Text="GERAR RELATÓRIO" Height="65px" Width="155px"
                                    Style="font-weight: 700" OnClick="btnGerar_Click" />
                                <asp:UpdateProgress ID="UpdateProgress1" runat="server">
                                    <ProgressTemplate>
                                        <br />
                                        <asp:Label ID="lblAguarde" runat="server" Text="AGUARDE..." Style="font-weight: 700"></asp:Label>
                                        <br />
                                        <img src="../Content/images/ajax-loading.gif" />
                                    </ProgressTemplate>
                                </asp:UpdateProgress>
                                <br />
                                <asp:LinkButton ID="lkbDownload" runat="server" OnClick="lkbDownload_Click" Visible="False">Download</asp:LinkButton>
                            </td>
                        </tr>
                        <tr>
                            <td class="style34">
                                &nbsp;
                            </td>
                            <td class="style29">
                                &nbsp;
                            </td>
                            <td class="style33">
                            </td>
                            <td>
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
    </form>
</body>
</html>
