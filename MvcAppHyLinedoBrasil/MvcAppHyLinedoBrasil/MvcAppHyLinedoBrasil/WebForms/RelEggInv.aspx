<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RelEggInv.aspx.cs" Inherits="MvcAppHyLinedoBrasil.WebForms.RelEggInv" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Relatório de Movimentação de Ovos</title>
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
        .style35
        {
            width: 189px;
            left: 50%;
            text-align: right;
            height: 259px;
        }
        .style36
        {
            width: 206px;
            text-align: center;
            height: 259px;
        }
        .style37
        {
            width: 177px;
            text-align: right;
            height: 259px;
        }
        .style38
        {
            height: 259px;
        }
    </style>
</head>
<body style="background-color: #5c87b2; font-family: Verdana, Tahoma, Arial, Helvetica Neue, Helvetica, Sans-Serif;">
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
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
                                Text="RELATÓRIO DE MOV. DE OVOS"></asp:Label>
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
                            <td class="style34">
                                <asp:Label ID="lblOrigem" runat="server" Text="Origem:" CssClass="style27"></asp:Label>
                            </td>
                            <td class="style29">
                                <asp:DropDownList ID="ddlOrigem" runat="server" CssClass="style26" Height="20px"
                                    Width="140px">
                                    <asp:ListItem>(Todas)</asp:ListItem>
                                    <asp:ListItem Value="CH">Incubatório Nova Granada</asp:ListItem>
                                    <asp:ListItem Value="TB">Incubatório Ajapi</asp:ListItem>
                                    <asp:ListItem Value="PH">Incubatório Avós</asp:ListItem>
                                </asp:DropDownList>
                            </td>
                            <td class="style33" style="text-align: right">
                                <asp:Label ID="lblLote" runat="server" CssClass="style27" Text="Lote:"></asp:Label>
                            </td>
                            <td>
                                <asp:DropDownList ID="ddlLotes" runat="server" CssClass="style26" Height="20px"
                                    Width="108px" DataSourceID="LotesSqlDataSource" DataTextField="FLOCK_ID" 
                                    DataValueField="FLOCK_ID">
                                    <asp:ListItem Value="(Todas)">(Todas)</asp:ListItem>
                                </asp:DropDownList>
                                <asp:SqlDataSource ID="LotesSqlDataSource" runat="server" 
                                    ConnectionString="<%$ ConnectionStrings:Oracle %>" 
                                    ProviderName="<%$ ConnectionStrings:Oracle.ProviderName %>" 
                                    
                                    SelectCommand="SELECT 1 Ordem, '(Todos)' Farm_ID,'(Todos)' FLOCK_ID from dual
Union
SELECT 2 Ordem, Farm_ID, FLOCK_ID FROM &quot;FLOCKS&quot; WHERE (&quot;ACTIVE&quot; = :ACTIVE) ORDER BY 1, &quot;FARM_ID&quot;, &quot;FLOCK_ID&quot;">
                                    <SelectParameters>
                                        <asp:Parameter DefaultValue="1" Name="ACTIVE" Type="Decimal" />
                                    </SelectParameters>
                                </asp:SqlDataSource>
                            </td>
                            <td class="style22">
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
                            <td class="style22">
                            </td>
                        </tr>
                        <tr>
                            <td class="style34">
                                <asp:Label ID="lblDataInicial" runat="server" Text="Data de Movimentação Inicial:" CssClass="style27"></asp:Label>
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
                                <asp:Label ID="lblDataFinal0" runat="server" CssClass="style27" Text="Data de Movimentação Final:"></asp:Label>
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
                            <td class="style35">
                                &nbsp;
                                <asp:Label ID="lblDataInicial0" runat="server" CssClass="style27" 
                                    Text="Data de Produção Inicial:"></asp:Label>
                            </td>
                            <td class="style36">
                                &nbsp;
                                <asp:Calendar ID="calDataProdInicial" runat="server" BackColor="White" 
                                    BorderColor="#3366CC" BorderWidth="1px" CellPadding="1" CssClass="style26" 
                                    DayNameFormat="Shortest" Font-Names="Verdana" Font-Size="8pt" 
                                    ForeColor="#003399" Height="160px" SelectedDate="11/07/2013 10:51:10" 
                                    Width="177px">
                                    <DayHeaderStyle BackColor="#99CCCC" ForeColor="#336666" Height="1px" />
                                    <NextPrevStyle Font-Size="8pt" ForeColor="#CCCCFF" />
                                    <OtherMonthDayStyle ForeColor="#999999" />
                                    <SelectedDayStyle BackColor="#009999" Font-Bold="True" ForeColor="#CCFF99" />
                                    <SelectorStyle BackColor="#99CCCC" ForeColor="#336666" />
                                    <TitleStyle BackColor="#003399" BorderColor="#3366CC" BorderWidth="1px" 
                                        Font-Bold="True" Font-Size="10pt" ForeColor="#CCCCFF" Height="25px" />
                                    <TodayDayStyle BackColor="#99CCCC" ForeColor="White" />
                                    <WeekendDayStyle BackColor="#CCCCFF" />
                                </asp:Calendar>
                            </td>
                            <td class="style37">
                                <asp:Label ID="lblDataFinal1" runat="server" CssClass="style27" 
                                    Text="Data de Produção Final:"></asp:Label>
                            </td>
                            <td class="style38">
                                <asp:Calendar ID="calDataProdFinal" runat="server" BackColor="White" 
                                    BorderColor="#3366CC" BorderWidth="1px" CellPadding="1" CssClass="style26" 
                                    DayNameFormat="Shortest" Font-Names="Verdana" Font-Size="8pt" 
                                    ForeColor="#003399" Height="150px" SelectedDate="11/07/2013 10:51:26" 
                                    Width="175px">
                                    <DayHeaderStyle BackColor="#99CCCC" ForeColor="#336666" Height="1px" />
                                    <NextPrevStyle Font-Size="8pt" ForeColor="#CCCCFF" />
                                    <OtherMonthDayStyle ForeColor="#999999" />
                                    <SelectedDayStyle BackColor="#009999" Font-Bold="True" ForeColor="#CCFF99" />
                                    <SelectorStyle BackColor="#99CCCC" ForeColor="#336666" />
                                    <TitleStyle BackColor="#003399" BorderColor="#3366CC" BorderWidth="1px" 
                                        Font-Bold="True" Font-Size="10pt" ForeColor="#CCCCFF" Height="25px" />
                                    <TodayDayStyle BackColor="#99CCCC" ForeColor="White" />
                                    <WeekendDayStyle BackColor="#CCCCFF" />
                                </asp:Calendar>
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
