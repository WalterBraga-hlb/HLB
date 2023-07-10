<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Incubacao.aspx.cs" Inherits="MvcAppHyLinedoBrasil.WebForms.Incubacao" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Incubação</title>
    <link href="../Content/icons/logo_hyline.ico" rel="Shortcut Icon" type="text/css" />
    <style type="text/css">
        .style14
        {
            width: 1278px;
            height: 50px;
        }
        .style22
        {
            width: 241px;
            text-align: left;
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
            width: 172px;
            height: 76px;
        }
        .style24
        {
            font-weight: bold;
            font-size: xx-small;
        }
        .style39
        {
            width: 192px;
        }
        .style57
        {
            width: 73px;
            left: 50%;
            text-align: right;
        }
        .style58
        {
            height: 50px;
        }
        .style59
        {
            width: 762px;
        }
        .style60
        {
            font-size: xx-small;
        }
        .style61
        {
            font-weight: bold;
            text-align: left;
            font-size: x-large;
        }
        .style62
        {
            width: 140px;
        }
        .style63
        {
            width: 216px;
        }
        .style64
        {
            width: 800px;
        }
        .style65
        {
            width: 164px;
            left: 50%;
            text-align: right;
        }
        .style67
        {
            font-weight: bold;
            text-decoration: underline;
        }
        .style68
        {
            width: 232px;
            text-align: right;
        }
        .style70
        {
            width: 305px;
            text-align: center;
        }
        .style71
        {
            width: 266px;
            text-align: right;
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
                <asp:Panel ID="Panel2" runat="server" Width="1002px" HorizontalAlign="Center">
                    <table style="width: 104%;">
                        <tr>
                            <td rowspan="3" class="style63">
                                <a href="../Home/Index">
                                    <img alt="" class="style23" src="../Content/images/logo.png" border="0" /></a>
                                <br />
                                <br />
                                <br />
                                <asp:LinkButton ID="lbMostrarInv" runat="server" onclick="lbMostrarInv_Click" 
                                    Visible="False">Mostrar Inventário</asp:LinkButton>
                                <br />
                                <asp:LinkButton ID="lbEsconderInv" runat="server" onclick="lbEsconderInv_Click">Esconder Inventário</asp:LinkButton>
                            </td>
                        </tr>
                        <tr>
                            <td class="style14">
                                <asp:Label ID="Label5" runat="server" Font-Bold="True" Font-Size="XX-Large" Font-Underline="False"
                                    Text="INCUBAÇÃO"></asp:Label>
                            </td>
                            <td class="style58">
                                <asp:Label ID="Label9" runat="server" Text="Selecione o Incubatório:"  Style="font-size: xx-small;
                                    font-weight: 700"></asp:Label>
                                <br />
                                <asp:DropDownList ID="ddlIncubatorios" runat="server" AutoPostBack="True" 
                                    onselectedindexchanged="ddlIncubatorios_SelectedIndexChanged">
                                    <asp:ListItem>CH</asp:ListItem>
                                    <asp:ListItem>TB</asp:ListItem>
                                </asp:DropDownList>
                                <br />
                                <asp:Label ID="Label4" runat="server" Text="Seleciona a Data da Incubação:" Style="font-size: xx-small;
                                    font-weight: 700"></asp:Label>
                                <asp:Calendar ID="Calendar1" runat="server" Font-Size="XX-Small" Height="32px" Width="233px"
                                    OnSelectionChanged="Calendar1_SelectionChanged">
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
                        </tr>
                    </table>
                </asp:Panel>
                <asp:UpdateProgress ID="UpdateProgress1" runat="server" 
                    AssociatedUpdatePanelID="UpdatePanel1">
                    <ProgressTemplate>
                        <strong>CARREGANDO...</strong>&nbsp;&nbsp;
                        <br />
                        <asp:Image ID="Image1" runat="server" ImageUrl="~/Content/images/ajax-loading.gif" />
                    </ProgressTemplate>
                </asp:UpdateProgress>
                <asp:Panel ID="Panel1" runat="server" Width="1002px" HorizontalAlign="Center" Style="text-align: left">
                    <table style="width: 100%;">
                        <tr>
                            <td class="style64">
                                
                                <asp:Label ID="Label3" runat="server" Font-Bold="True" Font-Size="Small" Font-Underline="False"
                                    Text="INVENTÁRIO DE OVOS"></asp:Label>
                                <table align="center">
                                    <tr>
                                        <td class="style57">
                                            <asp:Label ID="Label8" runat="server" CssClass="style61" Font-Size="XX-Small" Text="Localizar:"></asp:Label>
                                        </td>
                                        <td class="style22">
                                            &nbsp;
                                            <asp:TextBox ID="TextBox6" runat="server" CssClass="style24" Width="193px" Height="15px"></asp:TextBox>
                                        </td>
                                        <td class="style62">
                                            <asp:DropDownList ID="DropDownList1" runat="server" Height="21px" Style="font-weight: 700"
                                                Width="124px" CssClass="style60">
                                                <asp:ListItem>Linhagem</asp:ListItem>
                                                <asp:ListItem Value="Data de Produção">Data de Produção</asp:ListItem>
                                                <asp:ListItem>Lote</asp:ListItem>
                                            </asp:DropDownList>
                                            &nbsp;
                                        </td>
                                        <td class="style39">
                                            <asp:Button ID="btn_Pesquisar" runat="server" Font-Bold="True" Font-Size="XX-Small"
                                                Height="21px" OnClick="Button1_Click" Text="Pesquisar" Width="118px" CssClass="style60" />
                                            &nbsp;
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td style="text-align: center">
                                <asp:HyperLink ID="HyperLink2" runat="server" NavigateUrl="~/WebForms/MapaIncubacao.aspx"
                                    Target="_blank">Imprimir</asp:HyperLink>
                            </td>
                            <td style="text-align: center">
                                <asp:LinkButton ID="LinkButton1" runat="server" onclick="LinkButton1_Click">Inc.Estq.Futuro</asp:LinkButton>
                            </td>
                        </tr>
                        <tr>
                            <td class="style64">
                                <asp:GridView ID="GridView3" runat="server" AllowSorting="True" CellPadding="4" DataSourceID="EggInvDataSource"
                                    ForeColor="#333333" GridLines="None" Style="font-size: xx-small; text-align: center;"
                                    Width="756px" AutoGenerateColumns="False" OnSelectedIndexChanged="GridView3_SelectedIndexChanged"
                                    AllowPaging="True" onrowdatabound="GridView3_RowDataBound">
                                    <AlternatingRowStyle BackColor="White" />
                                    <Columns>
                                        <asp:CommandField ButtonType="Image" SelectImageUrl="~/Content/images/Next_16x16.gif"
                                            ShowSelectButton="True" />
                                        <asp:BoundField DataField="Granja / Núcleo" HeaderText="Granja / Núcleo" SortExpression="Granja / Núcleo">
                                        </asp:BoundField>
                                        <asp:BoundField DataField="LOTE" HeaderText="LOTE" SortExpression="LOTE" />
                                        <asp:BoundField DataField="Idade Lote" HeaderText="Idade Lote" 
                                            SortExpression="Idade Lote" />
                                        <asp:BoundField DataField="Nº Transporte" HeaderText="Nº Transporte" SortExpression="Nº Transporte" />
                                        <asp:BoundField DataField="LINHAGEM" HeaderText="LINHAGEM" SortExpression="LINHAGEM" />
                                        <asp:BoundField DataField="Data Prd." HeaderText="Data Prd." SortExpression="Data Prd."
                                            DataFormatString="{0:d}" />
                                        <asp:BoundField DataField="Idade do Ovo" HeaderText="Idade do Ovo" SortExpression="Idade do Ovo" />
                                        <asp:BoundField DataField="Qtde.Ovos" HeaderText="Qtde.Ovos" 
                                            SortExpression="Qtde.Ovos" DataFormatString="{0:N0}" />
                                        <asp:BoundField DataField="Média Últ.4 Semanas (%)" HeaderText="Média Últ.4 Semanas (%)"
                                            SortExpression="Média Últ.4 Semanas (%)" />
                                        <asp:BoundField DataField="Qtde.Pint. Últ. 4 Semanas" HeaderText="Qtde.Pint. Últ. 4 Semanas"
                                            SortExpression="Qtde.Pint. Últ. 4 Semanas" />
                                    </Columns>
                                    <EditRowStyle BackColor="#2461BF" />
                                    <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                    <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                    <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                                    <RowStyle BackColor="#EFF3FB" />
                                    <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                                    <SortedAscendingCellStyle BackColor="#F5F7FB" />
                                    <SortedAscendingHeaderStyle BackColor="#6D95E1" />
                                    <SortedDescendingCellStyle BackColor="#E9EBEF" />
                                    <SortedDescendingHeaderStyle BackColor="#4870BE" />
                                </asp:GridView>
                                <asp:SqlDataSource ID="EggInvDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:Oracle %>"
                                    ProviderName="<%$ ConnectionStrings:Oracle.ProviderName %>" SelectCommand="select e.farm_id &quot;Granja / Núcleo&quot;, 
       e.Flock_id Lote, 
       e.track_no &quot;Nº Transporte&quot;, 
       f.variety Linhagem, 
       e.lay_date &quot;Data Prd.&quot;,
       d.Age &quot;Idade Lote&quot;,
       (:Data - e.lay_date) &quot;Idade do Ovo&quot;, 
       e.egg_units &quot;Qtde.Ovos&quot;, 
       Round(AVG_LST4WK_HATCH(e.company,e.farm_id||'-'||e.flock_id),2) &quot;Média Últ.4 Semanas (%)&quot;,
       Round((AVG_LST4WK_HATCH(e.company,e.farm_id||'-'||e.flock_id)/100)*e.egg_units,0) &quot;Qtde.Pint. Últ. 4 Semanas&quot;
from egginv_data e, flocks f, Flock_Data d
where e.flock_key = f.flock_key
and f.flock_key = d.flock_key and e.lay_date = d.trx_date
and Status = 'O'
and hatch_loc = :Incubatorio
and (
         (:Pesquisa ='0')
         or 
         (:Campo = 'Linhagem' and f.variety like '%' ||:Pesquisa||'%')
         or
         (:Campo = 'Data de Produção' and e.lay_date = :Pesquisa)
         or
         (:Campo = 'Lote' and e.flock_id like '%'||:Pesquisa||'%')
        )
and f.active = 1
order by e.FLOCK_Id, e.lay_date">
                                    <SelectParameters>
                                        <asp:ControlParameter ControlID="Calendar1" DbType="Date" DefaultValue="" Name="Data"
                                            PropertyName="SelectedDate" />
                                        <asp:ControlParameter ControlID="ddlIncubatorios" Name="Incubatorio" 
                                            PropertyName="SelectedValue" DefaultValue="" />
                                        <asp:ControlParameter ControlID="TextBox6" Name="Pesquisa" PropertyName="Text" 
                                            DefaultValue="" />
                                        <asp:ControlParameter ControlID="DropDownList1" Name="Campo" 
                                            PropertyName="SelectedValue" DefaultValue="" />
                                    </SelectParameters>
                                </asp:SqlDataSource>
                            </td>
                            <td colspan="2">
                                <asp:FormView ID="FormView1" runat="server" CellPadding="4" Height="163px" BackColor="White"
                                    BorderColor="#CC9966" BorderStyle="None" BorderWidth="1px" DataKeyNames="FLOCK_ID,TRACK_NO,LAY_DATE,FARM_ID,LOCATION,COMPANY,REGION,HATCH_LOC,STATUS"
                                    DataSourceID="HatchFormDataSource" GridLines="Both" Style="font-size: xx-small"
                                    OnDataBound="FormView1_DataBound" Width="228px" OnItemUpdated="FormView1_ItemUpdated">
                                    <EditItemTemplate>
                                        <table style="width: 100%;">
                                            <tr>
                                                <td>
                                                    Granja:
                                                </td>
                                                <td>
                                                    <asp:Label ID="FARM_IDLabel1" runat="server" Text='<%# Eval("FARM_ID") %>' />
                                                    <br />
                                                    <asp:DropDownList ID="DropDownList4" runat="server" AutoPostBack="True" 
                                                        DataSourceID="FarmsSqlDataSource" DataTextField="FARM_ID" 
                                                        DataValueField="FARM_ID" Height="17px" Width="96px">
                                                    </asp:DropDownList>
                                                    <asp:SqlDataSource ID="FarmsSqlDataSource" runat="server" 
                                                        ConnectionString="<%$ ConnectionStrings:Oracle %>" 
                                                        ProviderName="<%$ ConnectionStrings:Oracle.ProviderName %>" 
                                                        
                                                        
                                                        
                                                        SelectCommand="SELECT FARM_ID FROM &quot;FLOCK_DATA&quot; WHERE LOCATION = 'PP' AND Active = 1 GROUP BY &quot;FARM_ID&quot; ORDER BY &quot;FARM_ID&quot;">
                                                    </asp:SqlDataSource>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    Lote:
                                                </td>
                                                <td>
                                                    <asp:Label ID="FLOCK_IDLabel1" runat="server" Text='<%# Eval("FLOCK_ID") %>' />
                                                    <br />
                                                    <asp:DropDownList ID="DropDownList3" runat="server" 
                                                        DataSourceID="FlocksSqlDataSource" DataTextField="FLOCK_ID" 
                                                        DataValueField="FLOCK_ID" Height="16px" Width="181px" AutoPostBack="True" 
                                                        onselectedindexchanged="DropDownList3_SelectedIndexChanged" 
                                                        ondatabound="DropDownList3_DataBound">
                                                    </asp:DropDownList>
                                                    <asp:SqlDataSource ID="FlocksSqlDataSource" runat="server" 
                                                        ConnectionString="<%$ ConnectionStrings:Oracle %>" 
                                                        ProviderName="<%$ ConnectionStrings:Oracle.ProviderName %>" 
                                                        
                                                        
                                                        
                                                        SelectCommand="SELECT * FROM &quot;FLOCKS&quot; WHERE ((&quot;FARM_ID&quot; = :FARM_ID AND ACTIVE = 1 AND LOCATION = 'PP')) ORDER BY &quot;FLOCK_ID&quot;">
                                                        <SelectParameters>
                                                            <asp:ControlParameter ControlID="DropDownList4" Name="FARM_ID" 
                                                                PropertyName="SelectedValue" />
                                                        </SelectParameters>
                                                    </asp:SqlDataSource>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    N. Transp.:
                                                </td>
                                                <td>
                                                    <asp:Label ID="TRACK_NOLabel1" runat="server" Text='<%# Eval("TRACK_NO") %>' />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    Data Prd.:
                                                </td>
                                                <td>
                                                    <asp:Label ID="LAY_DATELabel1" runat="server" Text='<%# Eval("LAY_DATE") %>' />
                                                    <asp:Calendar ID="Lay_DateCalendar" runat="server" BackColor="White" 
                                                        BorderColor="#3366CC" BorderWidth="1px" CellPadding="1" 
                                                        DayNameFormat="Shortest" Font-Names="Verdana" Font-Size="8pt" 
                                                        ForeColor="#003399" Height="105px" SelectedDate='<%# Bind("LAY_DATE") %>' 
                                                        Width="200px" onselectionchanged="Lay_DateCalendar_SelectionChanged">
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
                                            <tr>
                                                <td>
                                                    Incubadora:
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="MachineTextBox" runat="server" Text='<%# Bind("Machine") %>' Height="23px"
                                                        Width="56px" />
                                                    <asp:MaskedEditExtender ID="MaskedEditExtender2" runat="server" TargetControlID="MachineTextBox"
                                                        Mask="S-99" MessageValidatorTip="true" OnFocusCssClass="MaskedEditFocus" OnInvalidCssClass="MaskedEditError"
                                                        MaskType="Number" ErrorTooltipEnabled="True" AutoCompleteValue="S-" ClearMaskOnLostFocus="False">
                                                    </asp:MaskedEditExtender>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    Qtde.Ovos:
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="EGG_UNITSTextBox" runat="server" Text='<%# Bind("EGG_UNITS") %>'
                                                        Height="23px" Width="56px" OnTextChanged="EGG_UNITSTextBox_TextChanged" AutoPostBack="True" />                                                    
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    Média Eclosão:
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="MediaEclosaoTextBox" runat="server" Text='<%# Bind("MediaEclosao") %>'
                                                        Height="23px" Width="56px" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    Horário:
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="HorarioTextBox" runat="server" Height="23px" Text='<%# Bind("Horario") %>'
                                                        Width="56px" />
                                                    <asp:MaskedEditExtender ID="MaskedEditExtender1" runat="server" TargetControlID="HorarioTextBox"
                                                        Mask="99:99" MessageValidatorTip="true" OnFocusCssClass="MaskedEditFocus" OnInvalidCssClass="MaskedEditError"
                                                        MaskType="Time" AcceptAMPM="True" ErrorTooltipEnabled="True">
                                                    </asp:MaskedEditExtender>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    Posição:
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="PosicaoTextBox" runat="server" Height="23px" Text='<%# Bind("Posicao") %>'
                                                        Width="56px" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    Bandejas:
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="BandejasTextBox" runat="server" Height="23px" Text='<%# Bind("Bandejas") %>'
                                                        Width="56px" Enabled="False" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="2">
                                                    Observação:
                                                </td>
                                                
                                            </tr>
                                            <tr>
                                                <td colspan = "2">
                                                    <asp:TextBox ID="ObservacaoTextBox" runat="server" Height="86px" Text='<%# Bind("Observacao") %>'
                                                        Width="207px" TextMode="MultiLine" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                </td>
                                                <td>
                                                    <asp:LinkButton ID="UpdateButton" runat="server" CausesValidation="True" CommandName="Update"
                                                        Text="INCUBAR" />
                                                </td>
                                            </tr>
                                        </table>
                                    </EditItemTemplate>
                                    <EditRowStyle BackColor="#FFCC66" Font-Bold="True" ForeColor="#663399" />
                                    <FooterStyle BackColor="#FFFFCC" ForeColor="#330099" />
                                    <HeaderStyle BackColor="#990000" Font-Bold="True" ForeColor="#FFFFCC" />
                                    <InsertItemTemplate>
                                        FLOCK_ID:
                                        <asp:TextBox ID="FLOCK_IDTextBox" runat="server" Text='<%# Bind("FLOCK_ID") %>' />
                                        <br />
                                        TRACK_NO:
                                        <asp:TextBox ID="TRACK_NOTextBox" runat="server" Text='<%# Bind("TRACK_NO") %>' />
                                        <br />
                                        LAY_DATE:
                                        <asp:TextBox ID="LAY_DATETextBox" runat="server" Text='<%# Bind("LAY_DATE") %>' />
                                        <br />
                                        EGG_UNITS:
                                        <asp:TextBox ID="EGG_UNITSTextBox" runat="server" Text='<%# Bind("EGG_UNITS") %>' />
                                        <br />
                                        FARM_ID:
                                        <asp:TextBox ID="FARM_IDTextBox" runat="server" Text='<%# Bind("FARM_ID") %>' />
                                        <br />
                                        LOCATION:
                                        <asp:TextBox ID="LOCATIONTextBox" runat="server" Text='<%# Bind("LOCATION") %>' />
                                        <br />
                                        COMPANY:
                                        <asp:TextBox ID="COMPANYTextBox" runat="server" Text='<%# Bind("COMPANY") %>' />
                                        <br />
                                        REGION:
                                        <asp:TextBox ID="REGIONTextBox" runat="server" Text='<%# Bind("REGION") %>' />
                                        <br />
                                        HATCH_LOC:
                                        <asp:TextBox ID="HATCH_LOCTextBox" runat="server" Text='<%# Bind("HATCH_LOC") %>' />
                                        <br />
                                        STATUS:
                                        <asp:TextBox ID="STATUSTextBox" runat="server" Text='<%# Bind("STATUS") %>' />
                                        <br />
                                        <asp:LinkButton ID="InsertButton" runat="server" CausesValidation="True" CommandName="Insert"
                                            Text="Insert" />
                                        &nbsp;<asp:LinkButton ID="InsertCancelButton" runat="server" CausesValidation="False"
                                            CommandName="Cancel" Text="Cancel" />
                                    </InsertItemTemplate>
                                    <ItemTemplate>
                                        FLOCK_ID:
                                        <asp:Label ID="FLOCK_IDLabel" runat="server" Text='<%# Eval("FLOCK_ID") %>' />
                                        <br />
                                        TRACK_NO:
                                        <asp:Label ID="TRACK_NOLabel" runat="server" Text='<%# Eval("TRACK_NO") %>' />
                                        <br />
                                        LAY_DATE:
                                        <asp:Label ID="LAY_DATELabel" runat="server" Text='<%# Eval("LAY_DATE") %>' />
                                        <br />
                                        EGG_UNITS:
                                        <asp:Label ID="EGG_UNITSLabel" runat="server" Text='<%# Bind("EGG_UNITS") %>' />
                                        <br />
                                        FARM_ID:
                                        <asp:Label ID="FARM_IDLabel" runat="server" Text='<%# Eval("FARM_ID") %>' />
                                        <br />
                                        LOCATION:
                                        <asp:Label ID="LOCATIONLabel" runat="server" Text='<%# Eval("LOCATION") %>' />
                                        <br />
                                        COMPANY:
                                        <asp:Label ID="COMPANYLabel" runat="server" Text='<%# Eval("COMPANY") %>' />
                                        <br />
                                        REGION:
                                        <asp:Label ID="REGIONLabel" runat="server" Text='<%# Eval("REGION") %>' />
                                        <br />
                                        HATCH_LOC:
                                        <asp:Label ID="HATCH_LOCLabel" runat="server" Text='<%# Eval("HATCH_LOC") %>' />
                                        <br />
                                        STATUS:
                                        <asp:Label ID="STATUSLabel" runat="server" Text='<%# Eval("STATUS") %>' />
                                        &nbsp;&nbsp;
                                    </ItemTemplate>
                                    <PagerStyle BackColor="#FFFFCC" ForeColor="#330099" HorizontalAlign="Center" />
                                    <RowStyle BackColor="White" ForeColor="#330099" />
                                </asp:FormView>
                                <asp:SqlDataSource ID="HatchFormDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:Oracle %>"
                                    ProviderName="<%$ ConnectionStrings:Oracle.ProviderName %>" SelectCommand="SELECT &quot;FLOCK_ID&quot;, &quot;TRACK_NO&quot;, &quot;LAY_DATE&quot;, &quot;EGG_UNITS&quot;, &quot;FARM_ID&quot;, &quot;LOCATION&quot;, &quot;COMPANY&quot;, &quot;REGION&quot;, &quot;HATCH_LOC&quot;, &quot;STATUS&quot;, 'S-01' &quot;Machine&quot;, '00:00' &quot;Horario&quot;, '1' &quot;Posicao&quot;, '0' &quot;Bandejas&quot;, '0' &quot;MediaEclosao&quot;, 'obs' &quot;Observacao&quot; FROM &quot;EGGINV_DATA&quot; WHERE ((&quot;HATCH_LOC&quot; = :HATCH_LOC) AND (&quot;FLOCK_ID&quot; = :FLOCK_ID) AND (&quot;TRACK_NO&quot; = :TRACK_NO) AND (&quot;LAY_DATE&quot; = :LAY_DATE) AND (&quot;STATUS&quot; = :STATUS) and :Machine = 'S01' and :Horario = '00:00' and :Posicao = '1' and :Bandejas = '0' and :MediaEclosao = '0' and :Observacao = 'obs')"
                                    DeleteCommand="DELETE FROM &quot;EGGINV_DATA&quot; WHERE &quot;FLOCK_ID&quot; = :FLOCK_ID AND &quot;TRACK_NO&quot; = :TRACK_NO AND &quot;LAY_DATE&quot; = :LAY_DATE AND &quot;FARM_ID&quot; = :FARM_ID AND &quot;LOCATION&quot; = :LOCATION AND &quot;COMPANY&quot; = :COMPANY AND &quot;REGION&quot; = :REGION AND &quot;HATCH_LOC&quot; = :HATCH_LOC AND &quot;STATUS&quot; = :STATUS and :Machine = '1' and :Horario = '1' and :Posicao = '1' and :Bandejas = '1' and :MediaEclosao = '1' and :Observacao = 'obs'"
                                    InsertCommand="INSERT INTO hatchery_egg_data (COMPANY,REGION,Location,Set_Date,Hatch_Loc,Flock_Id,Lay_Date,Eggs_Rcvd,Machine,Track_No,Text_1,Num_1,Num_2, Num_3,Num_4)
VALUES('HYBR','BR','PP',sysdate,'CH',:FLOCK_ID,:LAY_DATE, :EGG_UNITS,:Machine,:TRACK_NO,:Horario,:Posicao,:Bandejas,:MediaEclosao,:Observacao)"
                                    
                                    
                                    
                                    
                                    UpdateCommand="UPDATE &quot;EGGINV_DATA&quot; SET &quot;EGG_UNITS&quot; = :EGG_UNITS WHERE &quot;FLOCK_ID&quot; = :FLOCK_ID AND &quot;TRACK_NO&quot; = :TRACK_NO AND &quot;LAY_DATE&quot; = :LAY_DATE AND &quot;FARM_ID&quot; = :FARM_ID AND &quot;LOCATION&quot; = :LOCATION AND &quot;COMPANY&quot; = :COMPANY AND &quot;REGION&quot; = :REGION AND &quot;HATCH_LOC&quot; = :HATCH_LOC AND &quot;STATUS&quot; = :STATUS and :Machine = '1' and :Horario = '1' and :Posicao = '1' and :Bandejas = '1' and :MediaEclosao = '1' and :Observacao = 'obs'">
                                    <DeleteParameters>
                                        <asp:Parameter Name="FLOCK_ID" Type="String" />
                                        <asp:Parameter Name="TRACK_NO" Type="String" />
                                        <asp:Parameter Name="LAY_DATE" Type="DateTime" />
                                        <asp:Parameter Name="FARM_ID" Type="String" />
                                        <asp:Parameter Name="LOCATION" Type="String" />
                                        <asp:Parameter Name="COMPANY" Type="String" />
                                        <asp:Parameter Name="REGION" Type="String" />
                                        <asp:Parameter Name="HATCH_LOC" Type="String" />
                                        <asp:Parameter Name="STATUS" Type="String" />
                                        <asp:Parameter Name="Machine" />
                                        <asp:Parameter Name="Horario" />
                                        <asp:Parameter Name="Posicao" />
                                        <asp:Parameter Name="Bandejas" />
                                        <asp:Parameter Name="MediaEclosao" />
                                        <asp:Parameter Name="Observacao" />
                                    </DeleteParameters>
                                    <InsertParameters>
                                        <asp:Parameter Name="FLOCK_ID" Type="String" />
                                        <asp:Parameter Name="LAY_DATE" Type="DateTime" />
                                        <asp:Parameter Name="EGG_UNITS" Type="Decimal" />
                                        <asp:Parameter Name="MACHINE" />
                                        <asp:Parameter Name="TRACK_NO" Type="String" />
                                        <asp:Parameter Name="Horario" />
                                        <asp:Parameter Name="Posicao" />
                                        <asp:Parameter Name="Bandejas" />
                                        <asp:Parameter Name="MediaEclosao" />
                                        <asp:Parameter Name="Observacao" />
                                    </InsertParameters>
                                    <SelectParameters>
                                        <asp:ControlParameter ControlID="ddlIncubatorios" DefaultValue="CH" 
                                            Name="HATCH_LOC" PropertyName="SelectedValue" />
                                        <asp:Parameter DefaultValue="" Name="FLOCK_ID" />
                                        <asp:Parameter DefaultValue="" Name="TRACK_NO" />
                                        <asp:Parameter DbType="Date" DefaultValue="" Name="LAY_DATE" />
                                        <asp:Parameter DefaultValue="O" Name="STATUS" />
                                        <asp:Parameter DefaultValue="S01" Name="Machine" />
                                        <asp:Parameter DefaultValue="00:00" Name="Horario" />
                                        <asp:Parameter DefaultValue="1" Name="Posicao" />
                                        <asp:Parameter DefaultValue="0" Name="Bandejas" />
                                        <asp:Parameter DefaultValue="0" Name="MediaEclosao" />
                                        <asp:Parameter DefaultValue="obs" Name="observacao" />
                                    </SelectParameters>
                                    <UpdateParameters>
                                        <asp:Parameter Name="EGG_UNITS" Type="Decimal" />
                                        <asp:Parameter Name="FLOCK_ID" Type="String" />
                                        <asp:Parameter Name="TRACK_NO" Type="String" />
                                        <asp:Parameter Name="LAY_DATE" Type="DateTime" />
                                        <asp:Parameter Name="FARM_ID" />
                                        <asp:Parameter Name="LOCATION" />
                                        <asp:Parameter Name="COMPANY" />
                                        <asp:Parameter Name="REGION" />
                                        <asp:Parameter Name="HATCH_LOC" />
                                        <asp:Parameter Name="STATUS" />
                                        <asp:Parameter Name="Machine" />
                                        <asp:Parameter Name="Horario" />
                                        <asp:Parameter Name="Posicao" />
                                        <asp:Parameter Name="Bandejas" />
                                        <asp:Parameter Name="MediaEclosao" />
                                        <asp:Parameter Name="Observacao" />
                                    </UpdateParameters>
                                </asp:SqlDataSource>
                                <asp:Label ID="lblMensagem" runat="server" Style="font-weight: 700; color: #FF3300"
                                    Visible="False"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td class="style64" colspan="3">
                                <asp:Label ID="Label2" runat="server" Font-Bold="True" Font-Size="Small" Font-Underline="False"
                                    Text="INCUBAÇÃO"></asp:Label>
                                <table align="center">
                                    <tr>
                                        <td class="style65">
                                            <asp:Label ID="Label1" runat="server" CssClass="style61" Font-Size="XX-Small" Text="Localizar:"></asp:Label>
                                        </td>
                                        <td class="style22">
                                            &nbsp;
                                            <asp:TextBox ID="TextBox1" runat="server" CssClass="style24" Width="193px" Height="15px"></asp:TextBox>
                                        </td>
                                        <td class="style62">
                                            <asp:DropDownList ID="DropDownList2" runat="server" Height="21px" Style="font-weight: 700"
                                                Width="124px" CssClass="style60">
                                                <asp:ListItem>Linhagem</asp:ListItem>
                                                <asp:ListItem Value="Data de Produção">Data de Produção</asp:ListItem>
                                                <asp:ListItem>Incubadora</asp:ListItem>
                                                <asp:ListItem>Lote</asp:ListItem>
                                            </asp:DropDownList>
                                            &nbsp;
                                        </td>
                                        <td class="style39" colspan="2">
                                            <asp:Button ID="Button2" runat="server" Font-Bold="True" Font-Size="XX-Small" Height="21px"
                                                OnClick="Button2_Click" Text="Pesquisar" Width="118px" CssClass="style60" />
                                            &nbsp;
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="style65">
                                            <asp:Label ID="Label6" runat="server" Text="Alterar Setter de " CssClass="style61"
                                                Font-Size="XX-Small"></asp:Label>
                                        </td>
                                        <td class="style22">
                                            &nbsp;
                                            <asp:TextBox ID="txt_SetterDe" runat="server" CssClass="style24" Width="88px" Height="15px"></asp:TextBox>
                                            <asp:MaskedEditExtender ID="MaskedEditExtender2" runat="server" TargetControlID="txt_SetterDe"
                                                        Mask="S-99" MessageValidatorTip="true" OnFocusCssClass="MaskedEditFocus" OnInvalidCssClass="MaskedEditError"
                                                        MaskType="Number" ErrorTooltipEnabled="True" AutoCompleteValue="S-" ClearMaskOnLostFocus="False">
                                                    </asp:MaskedEditExtender>
                                        </td>
                                        <td class="style57">
                                            <asp:Label ID="Label7" runat="server" Text=" para " CssClass="style61" Font-Size="XX-Small"></asp:Label>
                                        </td>
                                        <td class="style22">
                                            &nbsp;
                                            <asp:TextBox ID="txt_SetterPara" runat="server" CssClass="style24" Width="88px" Height="15px"></asp:TextBox>
                                            <asp:MaskedEditExtender ID="MaskedEditExtender3" runat="server" TargetControlID="txt_SetterPara"
                                                        Mask="S-99" MessageValidatorTip="true" OnFocusCssClass="MaskedEditFocus" OnInvalidCssClass="MaskedEditError"
                                                        MaskType="Number" ErrorTooltipEnabled="True" AutoCompleteValue="S-" ClearMaskOnLostFocus="False">
                                                    </asp:MaskedEditExtender>
                                        </td>
                                        <td class="style39">
                                            <asp:Button ID="btn_AtualizaSetter" runat="server" Text="Alterar Setter" Font-Bold="True"
                                                Font-Size="XX-Small" Height="21px" Width="118px" CssClass="style60" OnClick="btn_AtualizaSetter_Click" />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td class="style59" colspan="3">
                            <asp:Panel ID="panelIncubacao" ScrollBars="Horizontal" runat="server">
                                <br />
                                <asp:Label ID="lblMensagem2" runat="server" 
                                    Style="font-weight: 700; color: #FF3300" Visible="False"></asp:Label>
                                <br />
                                <asp:GridView ID="GridView1" runat="server" AllowPaging="True" AllowSorting="True"
                                    CellPadding="4" DataSourceID="HatchGridDataSource" ForeColor="#333333" GridLines="None"
                                    Style="font-size: xx-small; text-align: center;" Width="1031px" AutoGenerateColumns="False"
                                    Height="170px" PageSize="5" 
                                    OnSelectedIndexChanged="GridView1_SelectedIndexChanged" 
                                    ondatabound="GridView1_DataBound" 
                                    DataKeyNames="ID" 
                                    onrowediting="GridView1_RowEditing">
                                    <AlternatingRowStyle BackColor="White" />
                                    <Columns>
                                        <asp:TemplateField ShowHeader="False">
                                            <EditItemTemplate>
                                                <asp:ImageButton ID="ImageButton1" runat="server" CausesValidation="True" 
                                                    CommandName="Update" ImageUrl="~/Content/images/apply.png" 
                                                    onclick="ImageButton1_Click" Text="V" />
                                                &nbsp;<asp:ImageButton ID="ImageButton2" runat="server" CausesValidation="False" 
                                                    CommandName="Cancel" ImageUrl="~/Content/images/button_cancel.png" Text="X" />
                                            </EditItemTemplate>
                                            <ItemTemplate>
                                                <asp:ImageButton ID="ImageButton1" runat="server" CausesValidation="False" 
                                                    CommandName="Edit" ImageUrl="~/Content/images/kjots.png" Text="Edit" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:CommandField ButtonType="Image" SelectImageUrl="~/Content/images/Nao.png" ShowSelectButton="True" />
                                        <asp:TemplateField HeaderText="Linha" InsertVisible="False" SortExpression="ID">
                                            <EditItemTemplate>
                                                <asp:Label ID="Label1" runat="server" Text='<%# Eval("ID") %>'></asp:Label>
                                            </EditItemTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="Label15" runat="server" Text='<%# Bind("ID") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Incub." SortExpression="Incub">
                                            <EditItemTemplate>
                                                <asp:TextBox ID="TextBox1" runat="server" Enabled="False" 
                                                    Text='<%# Bind("Incub") %>' Width="38px"></asp:TextBox>
                                            </EditItemTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("Incub") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Granja / Núcleo / Lote" 
                                            SortExpression="Granja / Núcleo / Lote">
                                            <EditItemTemplate>
                                                <asp:TextBox ID="TextBox2" runat="server" Enabled="False" 
                                                    Text='<%# Bind("[Granja / Núcleo / Lote]") %>'></asp:TextBox>
                                            </EditItemTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="Label2" runat="server" 
                                                    Text='<%# Bind("[Granja / Núcleo / Lote]") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Idade Lote" SortExpression="Idade Lote">
                                            <EditItemTemplate>
                                                <asp:TextBox ID="TextBox15" runat="server" Enabled="False" 
                                                    Text='<%# Bind("[Idade Lote]") %>'></asp:TextBox>
                                            </EditItemTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="Label16" runat="server" Text='<%# Bind("[Idade Lote]") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Nº Transporte" SortExpression="Nº Transporte">
                                            <EditItemTemplate>
                                                <asp:TextBox ID="TextBox3" runat="server" Enabled="False" 
                                                    Text='<%# Bind("[Nº Transporte]") %>' Height="22px" 
                                                    style="text-align: center" Width="57px"></asp:TextBox>
                                            </EditItemTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="Label3" runat="server" Text='<%# Bind("[Nº Transporte]") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="LINHAGEM" SortExpression="LINHAGEM">
                                            <EditItemTemplate>
                                                <asp:TextBox ID="TextBox4" runat="server" Enabled="False" 
                                                    Text='<%# Bind("LINHAGEM") %>' Height="20px" Width="49px"></asp:TextBox>
                                            </EditItemTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="Label4" runat="server" Text='<%# Bind("LINHAGEM") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Data Prd." SortExpression="Data Prd">
                                            <EditItemTemplate>
                                                <asp:TextBox ID="TextBox5" runat="server" 
                                                    Text='<%# Bind("[Data Prd]") %>' Height="22px" style="text-align: center" 
                                                    Width="70px" AutoPostBack="True" ontextchanged="TextBox5_TextChanged"></asp:TextBox>
                                                <asp:MaskedEditExtender ID="MaskedEditExtender2" runat="server" TargetControlID="TextBox5"
                                                        Mask="99/99/9999" MessageValidatorTip="true" OnFocusCssClass="MaskedEditFocus" OnInvalidCssClass="MaskedEditError"
                                                        MaskType="Date" ErrorTooltipEnabled="True" ClearMaskOnLostFocus="False">
                                                    </asp:MaskedEditExtender>
                                            </EditItemTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="Label5" runat="server" 
                                                    Text='<%# Bind("[Data Prd]", "{0:d}") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Idade do Ovo" SortExpression="Idade do Ovo">
                                            <EditItemTemplate>
                                                <asp:TextBox ID="TextBox6" runat="server" Enabled="False" 
                                                    Text='<%# Bind("[Idade do Ovo]") %>' Height="22px" Width="26px"></asp:TextBox>
                                            </EditItemTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="Label6" runat="server" Text='<%# Bind("[Idade do Ovo]") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Qtde.Ovos" SortExpression="Qtde Ovos">
                                            <EditItemTemplate>
                                                <asp:TextBox ID="TextBox7" runat="server" Enabled="False" 
                                                    Text='<%# Bind("[Qtde Ovos]") %>' Height="22px" Width="74px"></asp:TextBox>
                                            </EditItemTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="Label7" runat="server" Text='<%# Bind("[Qtde Ovos]", "{0:N0}") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Estimativa (%) Eclosão" 
                                            SortExpression="Estimativa Eclosão">
                                            <EditItemTemplate>
                                                <asp:TextBox ID="TextBox10" runat="server" 
                                                    Text='<%# Bind("[Estimativa Eclosão]") %>' Height="22px" Width="52px"></asp:TextBox>
                                            </EditItemTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="Label10" runat="server" 
                                                    Text='<%# Bind("[Estimativa Eclosão]") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Qtde.Pint." SortExpression="Qtde Pint">
                                            <EditItemTemplate>
                                                <asp:TextBox ID="TextBox8" runat="server" Enabled="False" 
                                                    Text='<%# Bind("[Qtde Pint]") %>' Height="22px" Width="51px"></asp:TextBox>
                                            </EditItemTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="Label8" runat="server" 
                                                    Text='<%# Bind("[Qtde Pint]", "{0:N0}") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Horário" SortExpression="Horário">
                                            <EditItemTemplate>
                                                <asp:TextBox ID="TextBox9" runat="server" Text='<%# Bind("Horário") %>' 
                                                    Width="45px"></asp:TextBox>
                                                <asp:MaskedEditExtender ID="MaskedEditExtender1" runat="server" TargetControlID="TextBox9"
                                                    Mask="99:99" MessageValidatorTip="true" OnFocusCssClass="MaskedEditFocus" OnInvalidCssClass="MaskedEditError"
                                                    MaskType="Time" AcceptAMPM="True" ErrorTooltipEnabled="True">
                                                </asp:MaskedEditExtender>
                                            </EditItemTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="Label9" runat="server" Text='<%# Bind("Horário") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Posição" SortExpression="Posição">
                                            <EditItemTemplate>
                                                <asp:TextBox ID="TextBox11" runat="server" Enabled="False" 
                                                    Text='<%# Bind("Posição") %>' Height="22px" Width="22px"></asp:TextBox>
                                            </EditItemTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="Label11" runat="server" Text='<%# Bind("Posição") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Nº Bandejas" SortExpression="No Bandejas">
                                            <EditItemTemplate>
                                                <asp:TextBox ID="TextBox12" runat="server" Enabled="False" 
                                                    Text='<%# Bind("[No Bandejas]") %>' Height="22px" Width="29px"></asp:TextBox>
                                            </EditItemTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="Label12" runat="server" Text='<%# Bind("[No Bandejas]") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Observação" SortExpression="Observação">
                                            <EditItemTemplate>
                                                <asp:TextBox ID="TextBox13" runat="server" Text='<%# Bind("Observação") %>'></asp:TextBox>
                                            </EditItemTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="Label13" runat="server" Text='<%# Bind("Observação") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Usuário" SortExpression="Usuário">
                                            <EditItemTemplate>
                                                <asp:TextBox ID="TextBox14" runat="server" Enabled="False" 
                                                    Text='<%# Bind("Usuário") %>'></asp:TextBox>
                                            </EditItemTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="Label14" runat="server" Text='<%# Bind("Usuário") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                    <EditRowStyle BackColor="#2461BF" />
                                    <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                    <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                    <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                                    <RowStyle BackColor="#EFF3FB" />
                                    <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                                    <SortedAscendingCellStyle BackColor="#F5F7FB" />
                                    <SortedAscendingHeaderStyle BackColor="#6D95E1" />
                                    <SortedDescendingCellStyle BackColor="#E9EBEF" />
                                    <SortedDescendingHeaderStyle BackColor="#4870BE" />
                                </asp:GridView>
                                </asp:Panel>
                                <asp:SqlDataSource ID="HatchGridDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:LayoutDb %>"
                                    SelectCommand="select 
   ID,
   machine Incub,
   flock_id [Granja / Núcleo / Lote],
   age [Idade Lote],
   track_no [Nº Transporte],
   variety Linhagem,
   lay_date [Data Prd],
   DATEDIFF(dd,lay_date,@Data) [Idade do Ovo], 
   eggs_rcvd [Qtde Ovos],
   estimate [Estimativa Eclosão],
   Round( (Round(estimate,0,1)/100)*eggs_rcvd,0) &quot;Qtde Pint&quot;,
   Horario &quot;Horário&quot;, Posicao &quot;Posição&quot;, Bandejas &quot;No Bandejas&quot;,
   Observacao &quot;Observação&quot;,
   Usuario &quot;Usuário&quot;
from hatchery_egg_data with(Nolock)
where hatch_loc = @Incubatorio
and set_date = @Data
and (
         @Pesquisa = '0'
         or 
         (@Campo = 'Linhagem' and variety like '%'+@Pesquisa+'%')
         or
         (@Campo = 'Data de Produção' and lay_date = Case When @Pesquisa = @Pesquisa Then '1988-01-01' Else @Pesquisa End)
         or
         (@Campo = 'Incubadora' and machine like '%'+@Pesquisa+'%')
         or
         (@Campo = 'Lote' and flock_id like '%'+@Pesquisa+'%')
        )
order by 2" UpdateCommand="UPDATE HATCHERY_EGG_DATA SET company = 'HYBR' where ID = @original_ID">
                                    <SelectParameters>
                                        <asp:ControlParameter ControlID="Calendar1" Name="Data" PropertyName="SelectedDate" />
                                        <asp:ControlParameter ControlID="ddlIncubatorios" Name="Incubatorio" 
                                            PropertyName="SelectedValue" />
                                        <asp:ControlParameter ControlID="TextBox1" Name="Pesquisa" PropertyName="Text" />
                                        <asp:ControlParameter ControlID="DropDownList2" Name="Campo" PropertyName="SelectedValue" />
                                    </SelectParameters>
                                    <UpdateParameters>
                                        <asp:Parameter Name="original_ID" />
                                    </UpdateParameters>
                                </asp:SqlDataSource>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="3">
                                <table style="width: 100%;">
                                    <tr>
                                        <td class="style71">
                                            <asp:Label ID="lblTotalOvosIncubados" runat="server" 
                                                Text="Total de Ovos Incubados:" CssClass="style67"></asp:Label>
                                        </td>
                                        <td class="style70" style="text-align: center">
                                            &nbsp;
                                            <asp:Label ID="lblQtdeOvosIncubados" runat="server" style="text-align: center"></asp:Label>
                                        </td>
                                        <td style="text-align: center">
                                            &nbsp;
                                            <asp:Label ID="lblQtdeOvosIncubadosCx" runat="server" 
                                                style="text-align: center"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="style71">
                                            <asp:Label ID="lblMaquinasUtilizadas" runat="server" 
                                                Text="Máquinas Utilizadas:" CssClass="style67"></asp:Label>
                                        </td>
                                        <td class="style70" colspan="2">
                                            &nbsp;
                                            <asp:Label ID="lblMaquinas" runat="server" style="text-align: center"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr style="text-align: center">
                                        <td colspan="3">
                                            <br />
                                            &nbsp;
                                            <asp:Label ID="Label10" runat="server" Font-Bold="True" Font-Size="Small" 
                                                Font-Underline="False" Text="TABELAS DE CONFERÊNCIA" 
                                                style="text-decoration: underline"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="text-align: center">
                                        <asp:Label ID="Label11" runat="server" Font-Bold="True" Font-Size="Small" 
                                                Font-Underline="False" Text="SETTERS"></asp:Label>
                                            <asp:GridView ID="gvMaquinas" runat="server" AutoGenerateColumns="False" 
                                                CellPadding="4" DataSourceID="MaquinasSqlDataSource" ForeColor="#333333" 
                                                GridLines="None" style="font-size: xx-small" Width="100%">
                                                <AlternatingRowStyle BackColor="White" />
                                                <Columns>
                                                    <asp:BoundField DataField="Setters" HeaderText="Setters" 
                                                        SortExpression="Setters" />
                                                    <asp:BoundField DataField="Ovos Inc." HeaderText="Ovos Inc." ReadOnly="True" 
                                                        SortExpression="Ovos Inc." DataFormatString="{0:N0}" />
                                                    <asp:BoundField DataField="Bandejas" HeaderText="Bandejas" ReadOnly="True" 
                                                        SortExpression="Bandejas" DataFormatString="{0:N0}" />
                                                </Columns>
                                                <EditRowStyle BackColor="#2461BF" />
                                                <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                                <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                                <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                                                <RowStyle BackColor="#EFF3FB" />
                                                <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                                                <SortedAscendingCellStyle BackColor="#F5F7FB" />
                                                <SortedAscendingHeaderStyle BackColor="#6D95E1" />
                                                <SortedDescendingCellStyle BackColor="#E9EBEF" />
                                                <SortedDescendingHeaderStyle BackColor="#4870BE" />
                                            </asp:GridView>
                                            <asp:SqlDataSource ID="MaquinasSqlDataSource" runat="server" 
                                                ConnectionString="<%$ ConnectionStrings:LayoutDb %>" SelectCommand="select Machine Setters, SUM(Eggs_rcvd) [Ovos Inc.], SUM(Bandejas) [Bandejas] 
from HATCHERY_EGG_DATA
where Hatch_loc =@Incubatorio and Set_date = @SetDate
group by Machine
order by Machine">
                                                <SelectParameters>
                                                    <asp:ControlParameter ControlID="ddlIncubatorios" Name="Incubatorio" 
                                                        PropertyName="SelectedValue" />
                                                    <asp:ControlParameter ControlID="Calendar1" Name="SetDate" 
                                                        PropertyName="SelectedDate" />
                                                </SelectParameters>
                                            </asp:SqlDataSource>
                                        </td>
                                        <td style="text-align: center">
                                        <asp:Label ID="Label12" runat="server" Font-Bold="True" Font-Size="Small" 
                                                Font-Underline="False" Text="LOTES"></asp:Label>
                                            <asp:GridView ID="gvLotes" runat="server" AutoGenerateColumns="False" 
                                                CellPadding="4" DataSourceID="LotesSqlDataSource" ForeColor="#333333" 
                                                GridLines="None" style="font-size: xx-small" Width="100%">
                                                <AlternatingRowStyle BackColor="White" />
                                                <Columns>
                                                    <asp:BoundField DataField="Lotes" HeaderText="Lotes" 
                                                        SortExpression="Lotes" />
                                                    <asp:BoundField DataField="Ovos Inc." HeaderText="Ovos Inc." ReadOnly="True" 
                                                        SortExpression="Ovos Inc." DataFormatString="{0:N0}" />
                                                    <asp:BoundField DataField="Bandejas" HeaderText="Bandejas" ReadOnly="True" 
                                                        SortExpression="Bandejas" DataFormatString="{0:N0}" />
                                                </Columns>
                                                <EditRowStyle BackColor="#2461BF" />
                                                <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                                <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                                <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                                                <RowStyle BackColor="#EFF3FB" />
                                                <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                                                <SortedAscendingCellStyle BackColor="#F5F7FB" />
                                                <SortedAscendingHeaderStyle BackColor="#6D95E1" />
                                                <SortedDescendingCellStyle BackColor="#E9EBEF" />
                                                <SortedDescendingHeaderStyle BackColor="#4870BE" />
                                            </asp:GridView>
                                            <asp:SqlDataSource ID="LotesSqlDataSource" runat="server" 
                                                ConnectionString="<%$ ConnectionStrings:LayoutDb %>" SelectCommand="select Flock_id Lotes, SUM(Eggs_rcvd) [Ovos Inc.], SUM(Bandejas) [Bandejas] 
from HATCHERY_EGG_DATA
where Hatch_loc = @Incubatorio and Set_date = @SetDate
group by Flock_id
order by Flock_id">
                                                <SelectParameters>
                                                    <asp:ControlParameter ControlID="ddlIncubatorios" Name="Incubatorio" 
                                                        PropertyName="SelectedValue" />
                                                    <asp:ControlParameter ControlID="Calendar1" Name="SetDate" 
                                                        PropertyName="SelectedDate" />
                                                </SelectParameters>
                                            </asp:SqlDataSource>
                                        </td>
                                        <td style="text-align: center">
                                        <asp:Label ID="Label13" runat="server" Font-Bold="True" Font-Size="Small" 
                                                Font-Underline="False" Text="LINHAGENS"></asp:Label>
                                            <asp:GridView ID="gvLinhagens" runat="server" AutoGenerateColumns="False" 
                                                CellPadding="4" DataSourceID="LinhagensSqlDataSource" ForeColor="#333333" 
                                                GridLines="None" style="font-size: xx-small" Width="100%">
                                                <AlternatingRowStyle BackColor="White" />
                                                <Columns>
                                                    <asp:BoundField DataField="Linhagem" HeaderText="Linhagem" 
                                                        SortExpression="Linhagem" />
                                                    <asp:BoundField DataField="Ovos Inc." HeaderText="Ovos Inc." ReadOnly="True" 
                                                        SortExpression="Ovos Inc." DataFormatString="{0:N0}" />
                                                    <asp:BoundField DataField="Bandejas" HeaderText="Bandejas" ReadOnly="True" 
                                                        SortExpression="Bandejas" DataFormatString="{0:N0}" />
                                                </Columns>
                                                <EditRowStyle BackColor="#2461BF" />
                                                <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                                <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                                <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                                                <RowStyle BackColor="#EFF3FB" />
                                                <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                                                <SortedAscendingCellStyle BackColor="#F5F7FB" />
                                                <SortedAscendingHeaderStyle BackColor="#6D95E1" />
                                                <SortedDescendingCellStyle BackColor="#E9EBEF" />
                                                <SortedDescendingHeaderStyle BackColor="#4870BE" />
                                            </asp:GridView>
                                            <asp:SqlDataSource ID="LinhagensSqlDataSource" runat="server" 
                                                ConnectionString="<%$ ConnectionStrings:LayoutDb %>" SelectCommand="select Variety Linhagem, SUM(Eggs_rcvd) [Ovos Inc.], SUM(Bandejas) [Bandejas] 
from HATCHERY_EGG_DATA
where Hatch_loc = @Incubatorio and Set_date = @SetDate
group by Variety
order by Variety">
                                                <SelectParameters>
                                                    <asp:ControlParameter ControlID="ddlIncubatorios" Name="Incubatorio" 
                                                        PropertyName="SelectedValue" />
                                                    <asp:ControlParameter ControlID="Calendar1" Name="SetDate" 
                                                        PropertyName="SelectedDate" />
                                                </SelectParameters>
                                            </asp:SqlDataSource>
                                        </td>
                                    </tr>
                                </table>
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
