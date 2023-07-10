<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RelDiarioProducaoAvos.aspx.cs"
    Inherits="MvcAppHyLinedoBrasil.WebForms.RelDiarioProducaoAvos" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Brazil-GP Weekly Follow-Up Report</title>
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
            text-align: right;
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
                                Text="BRAZIL-GP WEEKLY FOLLOW-UP REPORT"></asp:Label>
                        </td>
                    </tr>
                </table>
                <asp:Panel ID="Panel1" runat="server" Width="1003px" HorizontalAlign="Center">
                    <table align="center" style="height: 0px; width: 994px;">
                        <tr>
                            <td colspan="5" class="style25">
                                <asp:Label ID="lblTitulo" runat="server" Text="Enter the following parameters:" Style="font-weight: 700;
                                    text-decoration: underline;"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td style="border:1px solid #ddd;">
                                <asp:Label ID="lblGranja0" runat="server" CssClass="style27" 
                                    Text="Generation / Operation of Farms:"></asp:Label>
                            </td>
                            <td style="border:1px solid #ddd;">
                                <asp:DropDownList ID="ddlGranja" runat="server" CssClass="style26" 
                                    Height="20px" Width="108px">
                                    <asp:ListItem Value="GG">Grower</asp:ListItem>
                                    <asp:ListItem Value="GP">Production</asp:ListItem>
                                    <asp:ListItem>(All)</asp:ListItem>
                                </asp:DropDownList>
                                <br />
                            </td>
                            <td class="style33" style="text-align: right">
                                &nbsp;</td>
                            <td>
                                &nbsp;</td>
                            <td class="style22">
                            </td>
                        </tr>
                        <tr style="border:1px solid #ddd;">
                            <td style="border:1px solid #ddd;">
                                <asp:Label ID="lblFazenda" runat="server" CssClass="style27" Text="Farm:"></asp:Label>
                            </td>
                            <td style="border:1px solid #ddd;" colspan="4">
                                <asp:Panel ID="pnlFazendas01" runat="server" style="text-align: left">
                                    <asp:CheckBox ID="chkTodas" runat="server" AutoPostBack="True" 
                                        oncheckedchanged="chkTodas_CheckedChanged" style="font-size: xx-small" 
                                        Text="(All)" />
                                    <asp:CheckBoxList ID="chklFarms" runat="server" 
                                        DataSourceID="FarmsSqlDataSource" DataTextField="Nome" DataValueField="Codigo" 
                                        style="font-size: xx-small">
                                    </asp:CheckBoxList>
                                    <asp:SqlDataSource ID="FarmsSqlDataSource" runat="server" 
                                        ConnectionString="<%$ ConnectionStrings:Apolo10ConnectionString %>" SelectCommand="select EF.USERFLIPCod Codigo, EmpNome Nome from EMPRESA_FILIAL EF With(Nolock)
where EF.USERFLIPCod is not null and EF.USERFLIPCod &lt;&gt; ''
and 0 &lt; (select COUNT(1) from EMP_FIL_USUARIO EU With(Nolock)
			where EF.EmpCod = EU.EmpCod and EU.UsuCod = @Login)
and EF.USERTipoUnidadeFLIP = 'Granja' and EF.USERFLIPCod = 'SB'
Order by 1">
                                        <SelectParameters>
                                            <asp:SessionParameter Name="Login" SessionField="login" />
                                        </SelectParameters>
                                    </asp:SqlDataSource>
                                </asp:Panel>
                            </td>
                        </tr>
                        <tr>
                            <td style="border:1px solid #ddd;">
                                <asp:Label ID="lblDataInicial" runat="server" Text="Initial Production Date:" 
                                    CssClass="style27"></asp:Label>
                            </td>
                            <td style="border:1px solid #ddd;">
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
                            <td style="border:1px solid #ddd;">
                                <asp:Label ID="lblDataFinal0" runat="server" CssClass="style27" 
                                    Text="Final Production Date:"></asp:Label>
                            </td>
                            <td style="border:1px solid #ddd;">
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
                            <td style="border:1px solid #ddd;">
                                <asp:Button ID="btnGerar" runat="server" Text="GENERATE REPORT" Height="65px" Width="155px"
                                    Style="font-weight: 700" OnClick="btnGerar_Click" />
                                <asp:UpdateProgress ID="UpdateProgress1" runat="server">
                                    <ProgressTemplate>
                                        <br />
                                        <asp:Label ID="lblAguarde" runat="server" Text="LOADING..." 
                                            Style="font-weight: 700"></asp:Label>
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
