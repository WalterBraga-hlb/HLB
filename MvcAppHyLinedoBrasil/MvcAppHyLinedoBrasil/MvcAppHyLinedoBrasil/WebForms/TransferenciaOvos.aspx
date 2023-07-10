<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TransferenciaOvos.aspx.cs" Inherits="MvcAppHyLinedoBrasil.WebForms.TransferenciaOvos" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Transferência de Ovos</title>
    <%--<link href="../Content/icons/logo_hyline.ico" rel="Shortcut Icon" type="text/css" />--%>
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
            font-weight: 700;
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
        .style72
        {
            width: 88px;
        }
        .style73
        {
            height: 40px;
        }
    </style>
</head>
<body style="background-color: #5c87b2; font-family: Verdana, Tahoma, Arial, Helvetica Neue, Helvetica, Sans-Serif;">
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" AsyncPostBackTimeout="0">
    </asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div style="text-align: center;" class="panel">
                <asp:Panel ID="Panel2" runat="server" Width="1002px" HorizontalAlign="Center">
                    <table style="width: 104%;">
                        <tr>
                            <td rowspan="3" class="style63">
                                <a href="../Home/Index">
                                    <%--<img alt="" class="style23" src="../Content/images/Logo.png" border="0" />--%>
                                    <asp:Image ID="Image2" runat="server" ImageUrl="../Content/images/Logo_EW.png" />
                                </a>
                                <asp:HyperLink ID="hlBackHome" runat="server" NavigateUrl="../Home/Index">Voltar para Home</asp:HyperLink>
                            </td>
                        </tr>
                        <tr>
                            <td class="style14">
                                <asp:Label ID="Label5" runat="server" Font-Bold="True" Font-Size="XX-Large" Font-Underline="False"
                                    Text="TRANSFERÊNCIA DE OVOS"></asp:Label>
                                <br />
                                <br />
                                <asp:Label ID="lblMensagem3" runat="server" 
                                    Style="font-weight: 700; color: #FF3300" Visible="False"></asp:Label>
                            </td>
                            <td class="style58">
                                <asp:Label ID="Label9" runat="server" Text="Selecione o Incubatório:"  Style="font-size: xx-small;
                                    font-weight: 700"></asp:Label>
                                <br />
                                <asp:DropDownList ID="ddlIncubatorios" runat="server" AutoPostBack="True" 
                                    onselectedindexchanged="ddlIncubatorios_SelectedIndexChanged">
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
                                
                                <br />
                                <asp:Label ID="Label3" runat="server" Font-Bold="True" Font-Size="Small" Font-Underline="False"
                                    Text="OVOS INCUBADOS"></asp:Label>
                                <table align="center">
                                    <tr>
                                        <td class="style57">
                                            <asp:Label ID="Label8" runat="server" CssClass="style61" Font-Size="XX-Small" Text="Localizar:"></asp:Label>
                                        </td>
                                        <td class="style22">
                                            <asp:TextBox ID="TextBox6" runat="server" CssClass="style24" Width="193px" Height="15px"></asp:TextBox>
                                        </td>
                                        <td class="style62">
                                            <asp:DropDownList ID="DropDownList1" runat="server" Height="21px" Style="font-weight: 700"
                                                Width="124px" CssClass="style60">
                                                <asp:ListItem>Linhagem</asp:ListItem>
                                                <asp:ListItem Value="Data de Produção">Data de Produção</asp:ListItem>
                                                <asp:ListItem>Lote</asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                        <td class="style62">
                                            <asp:DropDownList ID="ddlClassOvos" runat="server" Height="21px" Style="font-weight: 700"
                                                Width="124px" CssClass="style60" AutoPostBack="True">
                                                <asp:ListItem Value="T">(Todos)</asp:ListItem>
                                                <asp:ListItem>T0</asp:ListItem>
                                                <asp:ListItem>T1</asp:ListItem>
                                                <asp:ListItem>T2</asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                        <td class="style39">
                                            <asp:Button ID="btn_Pesquisar" runat="server" Font-Bold="True" Font-Size="XX-Small"
                                                Height="21px" Text="Pesquisar" Width="118px" CssClass="style60" />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td style="text-align: center">
                                <table style="width: 100%; text-align: center;">
                                    <tr>
                                        <td class="style72">
                                            <asp:Button ID="btnGerar" runat="server" 
                                                Text="GERAR RELATÓRIO"
                                                Style="font-weight: 700" 
                                                OnClick="btnGerar_Click" />      
                                            <br />
                                            <br />
                                            <asp:LinkButton ID="lbtnExportar" runat="server" onclick="lbtnExportar_Click" 
                                                style="font-size: medium; font-weight: 700; color: #000000" 
                                                Visible="False">Exportar</asp:LinkButton>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td class="style64">
                                <asp:GridView ID="GridView3" runat="server" AllowSorting="True" CellPadding="4" DataSourceID="EggInvDataSource"
                                    ForeColor="#333333" GridLines="None" Style="font-size: xx-small; text-align: center;"
                                    Width="756px" AutoGenerateColumns="False" OnSelectedIndexChanged="GridView3_SelectedIndexChanged"
                                    AllowPaging="True">
                                    <AlternatingRowStyle BackColor="White" />
                                    <Columns>
                                        <asp:CommandField ButtonType="Image" 
                                            SelectImageUrl="~/Content/images/Next_16x16.gif" ShowSelectButton="True" />
                                        <asp:BoundField DataField="Horário" HeaderText="Horário" 
                                            SortExpression="Horário">
                                        </asp:BoundField>
                                        <asp:BoundField DataField="Machine" HeaderText="Machine" 
                                            SortExpression="Machine" />
                                        <asp:BoundField DataField="Variety" HeaderText="Variety" 
                                            SortExpression="Variety" />
                                        <asp:BoundField DataField="Lote Completo" HeaderText="Lote Completo" 
                                            SortExpression="Lote Completo" />
                                        <asp:BoundField DataField="Lote" HeaderText="Lote" SortExpression="Lote" />
                                        <asp:BoundField DataField="Idade Lote" HeaderText="Idade Lote" 
                                            SortExpression="Idade Lote" />
                                        <asp:BoundField DataField="Class. Ovo" HeaderText="Class. Ovo" 
                                            SortExpression="Class. Ovo" />
                                        <asp:BoundField DataField="Qtde. Ovos" HeaderText="Qtde. Ovos"
                                            SortExpression="Qtde. Ovos" ReadOnly="True" DataFormatString="{0:N0}" />
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
                                <asp:SqlDataSource ID="EggInvDataSource" runat="server" 
                                    ConnectionString="<%$ ConnectionStrings:HLBAPPConnectionString %>" SelectCommand="select 
	MAX(H.Horario) [Horário],
	H.Machine,
	H.Flock_id [Lote Completo],
	H.Egg_key Lote,
                   H.Variety,
	Max(C.Age) [Idade Lote],
	H.ClassOvo [Class. Ovo],
	SUM(H.Eggs_rcvd)
	-
	ISNULL((select SUM(T.Qtde_Ovos_Transferidos) from HATCHERY_TRAN_DATA T With(Nolock)
	 where H.Flock_id = T.Flock_id --and H.Lay_date = T.Lay_date
	 and H.Machine = T.Setter and H.Hatch_loc = T.Hatch_loc
	 and H.Set_date = T.Set_date and H.ClassOvo = T.ClassOvo),0)
	[Qtde. Ovos]
from HLBAPP.dbo.HATCHERY_EGG_DATA H With(Nolock)
left join HLBAPP.dbo.FLOCK_DATA C With(Nolock) on 
	H.Flock_id = C.Farm_ID+'-'+C.Flock_ID and
	H.Lay_date = C.Trx_Date
where H.Set_date = @SetDate and H.Hatch_loc = @Incubatorio and
	(
        (@Pesquisa ='0')
        or 
        (@Campo = 'Linhagem' and H.Variety like '%' + @Pesquisa + '%')
        or
		(@Campo = 'Data de Produção' and H.Lay_date = Case When @Pesquisa = @Pesquisa Then '1988-01-01' Else @Pesquisa End)
        or
        (@Campo = 'Lote' and H.Flock_id like '%' + @Pesquisa + '%')
    )
group by
	H.Hatch_loc,
	H.Set_date,
	H.Machine,
	H.Flock_id,
                   H.Variety,
	H.Egg_key,
	H.ClassOvo
having
    (SUM(H.Eggs_rcvd)
	-
	ISNULL((select SUM(T.Qtde_Ovos_Transferidos) from HATCHERY_TRAN_DATA T With(Nolock)
	 where H.Flock_id = T.Flock_id --and H.Lay_date = T.Lay_date
	 and H.Machine = T.Setter and H.Hatch_loc = T.Hatch_loc
	 and H.Set_date = T.Set_date and H.ClassOvo = T.ClassOvo),0)) &gt; 0
order by
	1,2,3,4,5">
                                    <SelectParameters>
                                        <asp:ControlParameter ControlID="Calendar1" Name="SetDate"
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
                                    BorderColor="#CC9966" BorderStyle="None" BorderWidth="1px"
                                    DataSourceID="HatchFormDataSource" GridLines="Both" Style="font-size: xx-small"
                                    Width="228px" ondatabound="FormView1_DataBound">
                                    <EditItemTemplate>
                                        <table style="width: 100%;">
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblMACHINE" runat="server" Text="Incubadora:" />
                                                </td>
                                                <td>
                                                    <asp:Label ID="MACHINELabel1" runat="server" Text='<%# Eval("Machine") %>' />
                                                    <br />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblVariety" runat="server" Text="Linhagem:" />
                                                </td>
                                                <td>
                                                    <asp:Label ID="VarietyLabel1" runat="server" Text='<%# Eval("Variety") %>' />
                                                    <br />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblFLOCK_ID" runat="server" Text="Lote Completo:" />
                                                </td>
                                                <td>
                                                    <asp:Label ID="FLOCK_IDLabel1" runat="server" Text='<%# Eval("FLOCK_ID") %>' />
                                                    <br />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblNumLote" runat="server" Text="Nº Lote:" />
                                                </td>
                                                <td>
                                                    <asp:Label ID="EGG_KEYLabel1" runat="server" Text='<%# Eval("EGG_KEY") %>' />
                                                    <br />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblLayDate" runat="server" Text="Maior Data Prd.:" />
                                                </td>
                                                <td>
                                                    <asp:Label ID="LAY_DATELabel1" runat="server" 
                                                        Text='<%# Eval("Lay_date", "{0:d}") %>' />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblClasOvo" runat="server" Text="Class. Ovo:" />
                                                </td>
                                                <td>
                                                    <asp:Label ID="ClassOvoLabel1" runat="server" 
                                                        Text='<%# Eval("ClassOvo") %>' />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblHatcher" runat="server" Text="Nascedouro:" />
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="MachineTextBox" runat="server" Text='<%# Bind("Hatcher") %>' Height="23px"
                                                        Width="56px" />
                                                    <asp:MaskedEditExtender ID="MaskedEditExtender2" runat="server" TargetControlID="MachineTextBox"
                                                        Mask="S-99" MessageValidatorTip="true" OnFocusCssClass="MaskedEditFocus" OnInvalidCssClass="MaskedEditError"
                                                        MaskType="Number" ErrorTooltipEnabled="True" AutoCompleteValue="S-" ClearMaskOnLostFocus="False">
                                                    </asp:MaskedEditExtender>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblQtdeOvos" runat="server" Text="Qtde.Ovos:" />
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="EGG_UNITSTextBox" runat="server" Text='<%# Bind("QtdeOvos") %>'
                                                        Height="23px" Width="56px" 
                                                        AutoPostBack="True" />                                                    
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblDataTransf" runat="server" Text="Data Transf.:" />
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="TransfDateTextBox" runat="server" Text='<%# Bind("Transf_date") %>'
                                                        Height="23px" Width="96px" />
                                                    <asp:CalendarExtender ID="TransfDateTextBox_CalendarExtender" runat="server" 
                                                        Enabled="True" Format="dd/MM/yyyy" TargetControlID="TransfDateTextBox" 
                                                        TodaysDateFormat="dd/MM/yyyy">
                                                    </asp:CalendarExtender>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblHorarioInicio" runat="server" Text="Hora Início:" />
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="HorarioTextBox" runat="server" Height="23px" Text='<%# Bind("Hora_Inicio") %>'
                                                        Width="56px" />
                                                    <asp:MaskedEditExtender ID="MaskedEditExtender1" runat="server" TargetControlID="HorarioTextBox"
                                                        Mask="99:99" MessageValidatorTip="true" OnFocusCssClass="MaskedEditFocus" OnInvalidCssClass="MaskedEditError"
                                                        MaskType="Time" AcceptAMPM="True" ErrorTooltipEnabled="True">
                                                    </asp:MaskedEditExtender>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblContaminadoTransf" runat="server" Text="Contaminado Transferência:" />
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="ContaminadoTransfTextBox" runat="server" Height="23px" Text='<%# Bind("Contaminado_Transferencia") %>'
                                                        Width="56px" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblContaminadoRodizio" runat="server" Text="Contaminado Rodízio:" />
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="ContaminadoRodizioTextBox" runat="server" Height="23px" Text='<%# Bind("Contaminado_Rodizio") %>'
                                                        Width="56px" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblBicados" runat="server" Text="Bicados:" />
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="BicadosTextBox" runat="server" Height="23px" Text='<%# Bind("Bicados") %>'
                                                        Width="56px" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblTransfTrincado" runat="server" Text="Trincados Transferência:" />
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="Trincados_TransferenciaTextBox" runat="server" Height="23px" Text='<%# Bind("Trincados_Transferencia") %>'
                                                        Width="56px" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblTransfRodizio" runat="server" Text="Trincados Rodízio:" />
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="Trincados_RodizioTextBox" runat="server" Height="23px" Text='<%# Bind("Trincados_Rodizio") %>'
                                                        Width="56px" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblGrudados" runat="server" Text="Nº Grudados:" />
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="Num_GrudadosTextBox" runat="server" Height="23px" Text='<%# Bind("Num_Grudados") %>'
                                                        Width="56px" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblPintosNascidos" runat="server" Text="Pintos Nascidos:" />
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="Pintos_NascidosTextBox" runat="server" Height="23px" Text='<%# Bind("Pintos_Nascidos") %>'
                                                        Width="56px" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblPerdidosTransf" runat="server" Text="Perdidos Transferência:" />
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="Perdidos_TransferenciaTextBox" runat="server" Height="23px" Text='<%# Bind("Perdidos_Transferencia") %>'
                                                        Width="56px" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblPerdidosRodizio" runat="server" Text="Perdidos Rodízio:" />
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="Perdidos_RodizioTextBox" runat="server" Height="23px" Text='<%# Bind("Perdidos_Rodizio") %>'
                                                        Width="56px" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblClaros" runat="server" Text="Claros:" />
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="ClarosTextBox" runat="server" Height="23px" 
                                                        Text='<%# Bind("Claros") %>'
                                                        Width="56px" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblHoraTermino" runat="server" Text="Hora Término:" />
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="Hora_TerminoTextBox" runat="server" Height="23px" Text='<%# Bind("Hora_Termino") %>'
                                                        Width="56px" />
                                                    <asp:MaskedEditExtender ID="MaskedEditExtender4" runat="server" TargetControlID="Hora_TerminoTextBox"
                                                        Mask="99:99" MessageValidatorTip="true" OnFocusCssClass="MaskedEditFocus" OnInvalidCssClass="MaskedEditError"
                                                        MaskType="Time" AcceptAMPM="True" ErrorTooltipEnabled="True">
                                                    </asp:MaskedEditExtender>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                </td>
                                                <td>
                                                    <asp:LinkButton ID="UpdateButton" runat="server" CausesValidation="True"
                                                        Text="TRANSFERIR" onclick="UpdateButton_Click" />
                                                </td>
                                            </tr>
                                        </table>
                                    </EditItemTemplate>
                                    <EditRowStyle BackColor="#FFCC66" Font-Bold="True" ForeColor="#663399" />
                                    <FooterStyle BackColor="#FFFFCC" ForeColor="#330099" />
                                    <HeaderStyle BackColor="#990000" Font-Bold="True" ForeColor="#FFFFCC" />
                                    <InsertItemTemplate>
                                        Hatch_loc:
                                        <asp:TextBox ID="Hatch_locTextBox" runat="server" 
                                            Text='<%# Bind("Hatch_loc") %>' />
                                        <br />
                                        Set_date:
                                        <asp:TextBox ID="Set_dateTextBox" runat="server" 
                                            Text='<%# Bind("Set_date") %>' />
                                        <br />
                                        Variety:
                                        <asp:TextBox ID="VarietyTextBox" runat="server" Text='<%# Bind("Variety") %>' />
                                        <br />
                                        Machine:
                                        <asp:TextBox ID="MachineTextBox" runat="server" 
                                            Text='<%# Bind("Machine") %>' />
                                        <br />
                                        Flock_id:
                                        <asp:TextBox ID="Flock_idTextBox" runat="server" 
                                            Text='<%# Bind("Flock_id") %>' />
                                        <br />
                                        Egg_key:
                                        <asp:TextBox ID="Egg_keyTextBox" runat="server" 
                                            Text='<%# Bind("Egg_key") %>' />
                                        <br />
                                        Lay_date:
                                        <asp:TextBox ID="Lay_dateTextBox" runat="server" 
                                            Text='<%# Bind("Lay_date") %>' />
                                        <br />
                                        ClassOvo:
                                        <asp:TextBox ID="ClassOvoTextBox" runat="server" 
                                            Text='<%# Bind("ClassOvo") %>' />
                                        <br />
                                        QtdeOvos:
                                        <asp:TextBox ID="QtdeOvosTextBox" runat="server" 
                                            Text='<%# Bind("QtdeOvos") %>' />
                                        <br />
                                        Transf_date:
                                        <asp:TextBox ID="Transf_dateTextBox" runat="server" 
                                            Text='<%# Bind("Transf_date") %>' />
                                        <br />
                                        Hatcher:
                                        <asp:TextBox ID="HatcherTextBox" runat="server" 
                                            Text='<%# Bind("Hatcher") %>' />
                                        <br />
                                        Hora_Inicio:
                                        <asp:TextBox ID="Hora_InicioTextBox" runat="server" 
                                            Text='<%# Bind("Hora_Inicio") %>' />
                                        <br />
                                        Contaminado_Transferencia:
                                        <asp:TextBox ID="Contaminado_TransferenciaTextBox" runat="server" 
                                            Text='<%# Bind("Contaminado_Transferencia") %>' />
                                        <br />
                                        Contaminado_Rodizio:
                                        <asp:TextBox ID="Contaminado_RodizioTextBox" runat="server" 
                                            Text='<%# Bind("Contaminado_Rodizio") %>' />
                                        <br />
                                        Bicados:
                                        <asp:TextBox ID="BicadosTextBox" runat="server" 
                                            Text='<%# Bind("Bicados") %>' />
                                        <br />
                                        Trincados_Transferencia:
                                        <asp:TextBox ID="Trincados_TransferenciaTextBox" runat="server" 
                                            Text='<%# Bind("Trincados_Transferencia") %>' />
                                        <br />
                                        Trincados_Rodizio:
                                        <asp:TextBox ID="Trincados_RodizioTextBox" runat="server" 
                                            Text='<%# Bind("Trincados_Rodizio") %>' />
                                        <br />
                                        Num_Grudados:
                                        <asp:TextBox ID="Num_GrudadosTextBox" runat="server" 
                                            Text='<%# Bind("Num_Grudados") %>' />
                                        <br />
                                        Pintos_Nascidos:
                                        <asp:TextBox ID="Pintos_NascidosTextBox" runat="server" 
                                            Text='<%# Bind("Pintos_Nascidos") %>' />
                                        <br />
                                        Claros:
                                        <asp:TextBox ID="ClarosTextBox" runat="server" 
                                            Text='<%# Bind("Claros") %>' />
                                        <br />
                                        Perdidos_Transferencia:
                                        <asp:TextBox ID="Perdidos_TransferenciaTextBox" runat="server" 
                                            Text='<%# Bind("Perdidos_Transferencia") %>' />
                                        <br />
                                        Perdidos_Rodizio:
                                        <asp:TextBox ID="Perdidos_RodizioTextBox" runat="server" 
                                            Text='<%# Bind("Perdidos_Rodizio") %>' />
                                        <br />
                                        Hora_Termino:
                                        <asp:TextBox ID="Hora_TerminoTextBox" runat="server" 
                                            Text='<%# Bind("Hora_Termino") %>' />
                                        <br />
                                        <asp:LinkButton ID="InsertButton" runat="server" CausesValidation="True" CommandName="Insert"
                                            Text="Insert" />
                                        &nbsp;<asp:LinkButton ID="InsertCancelButton" runat="server" CausesValidation="False"
                                            CommandName="Cancel" Text="Cancel" />
                                    </InsertItemTemplate>
                                    <ItemTemplate>
                                        Hatch_loc:
                                        <asp:Label ID="Hatch_locLabel" runat="server" Text='<%# Bind("Hatch_loc") %>' />
                                        <br />
                                        Set_date:
                                        <asp:Label ID="Set_dateLabel" runat="server" Text='<%# Bind("Set_date") %>' />
                                        <br />
                                        Variety:
                                        <asp:Label ID="VarietyLabel" runat="server" Text='<%# Bind("Variety") %>' />
                                        <br />
                                        Machine:
                                        <asp:Label ID="MachineLabel" runat="server" Text='<%# Bind("Machine") %>' />
                                        <br />
                                        Flock_id:
                                        <asp:Label ID="Flock_idLabel" runat="server" Text='<%# Bind("Flock_id") %>' />
                                        <br />
                                        Egg_key:
                                        <asp:Label ID="Egg_keyLabel" runat="server" Text='<%# Bind("Egg_key") %>' />
                                        <br />
                                        Lay_date:
                                        <asp:Label ID="Lay_dateLabel" runat="server" Text='<%# Bind("Lay_date") %>' />
                                        <br />
                                        ClassOvo:
                                        <asp:Label ID="ClassOvoLabel" runat="server" 
                                            Text='<%# Bind("ClassOvo") %>' />
                                        <br />
                                        QtdeOvos:
                                        <asp:Label ID="QtdeOvosLabel" runat="server" Text='<%# Bind("QtdeOvos") %>' />
                                        <br />
                                        Transf_date:
                                        <asp:Label ID="Transf_dateLabel" runat="server" 
                                            Text='<%# Bind("Transf_date") %>' />
                                        <br />
                                        Hatcher:
                                        <asp:Label ID="HatcherLabel" runat="server" 
                                            Text='<%# Bind("Hatcher") %>' />
                                        <br />
                                        Hora_Inicio:
                                        <asp:Label ID="Hora_InicioLabel" runat="server" 
                                            Text='<%# Bind("Hora_Inicio") %>' />
                                        <br />
                                        Contaminado_Transferencia:
                                        <asp:Label ID="Contaminado_TransferenciaLabel" runat="server" 
                                            Text='<%# Bind("Contaminado_Transferencia") %>' />
                                        <br />
                                        Contaminado_Rodizio:
                                        <asp:Label ID="Contaminado_RodizioLabel" runat="server" 
                                            Text='<%# Bind("Contaminado_Rodizio") %>' />
                                        <br />
                                        Bicados:
                                        <asp:Label ID="BicadosLabel" runat="server" 
                                            Text='<%# Bind("Bicados") %>' />
                                        <br />
                                        Trincados_Transferencia:
                                        <asp:Label ID="Trincados_TransferenciaLabel" runat="server" 
                                            Text='<%# Bind("Trincados_Transferencia") %>' />
                                        <br />
                                        Trincados_Rodizio:
                                        <asp:Label ID="Trincados_RodizioLabel" runat="server" 
                                            Text='<%# Bind("Trincados_Rodizio") %>' />
                                        <br />
                                        Num_Grudados:
                                        <asp:Label ID="Num_GrudadosLabel" runat="server" 
                                            Text='<%# Bind("Num_Grudados") %>' />
                                        <br />
                                        Pintos_Nascidos:
                                        <asp:Label ID="Pintos_NascidosLabel" runat="server" 
                                            Text='<%# Bind("Pintos_Nascidos") %>' />
                                        <br />
                                        Claros:
                                        <asp:Label ID="ClarosLabel" runat="server" 
                                            Text='<%# Bind("Claros") %>' />
                                        <br />
                                        Perdidos_Transferencia:
                                        <asp:Label ID="Perdidos_TransferenciaLabel" runat="server" 
                                            Text='<%# Bind("Perdidos_Transferencia") %>' />
                                        <br />
                                        Perdidos_Rodizio:
                                        <asp:Label ID="Perdidos_RodizioLabel" runat="server" 
                                            Text='<%# Bind("Perdidos_Rodizio") %>' />
                                        <br />
                                        Hora_Termino:
                                        <asp:Label ID="Hora_TerminoLabel" runat="server" 
                                            Text='<%# Bind("Hora_Termino") %>' />
                                        <br />
                                    </ItemTemplate>
                                    <PagerStyle BackColor="#FFFFCC" ForeColor="#330099" HorizontalAlign="Center" />
                                    <RowStyle BackColor="White" ForeColor="#330099" />
                                </asp:FormView>
                                <asp:SqlDataSource ID="HatchFormDataSource" runat="server" 
                                    ConnectionString="<%$ ConnectionStrings:HLBAPPConnectionString %>" 
                                    
                                    
                                    
                                    
                                    
                                    SelectCommand="select 
	Hatch_loc,
	Set_date,
                   Variety,
	Machine,
	Flock_id,
	Egg_key,
	Max(Lay_date) Lay_date,
	ClassOvo,
	SUM(Eggs_rcvd)
	-
	ISNULL((select SUM(T.Qtde_Ovos_Transferidos) from HATCHERY_TRAN_DATA T With(Nolock)
	 where H.Flock_id = T.Flock_id --and H.Lay_date = T.Lay_date
	 and H.Machine = T.Setter and H.Hatch_loc = T.Hatch_loc
	 and H.Set_date = T.Set_date and H.ClassOvo = T.ClassOvo),0)
	QtdeOvos,
	CONVERT(varchar(10),getdate(),103) Transf_date,
	'G01' Hatcher,
	'00:00' Hora_Inicio, '0' Contaminado_Transferencia, '0' Contaminado_Rodizio, '0' Bicados,
	'0' Trincados_Transferencia, '0' Trincados_Rodizio, '0' Num_Grudados, '0' Pintos_Nascidos,
                   '0' Claros,
	'0' Perdidos_Transferencia, '0' Perdidos_Rodizio, '00:00' Hora_Termino
from HLBAPP.dbo.HATCHERY_EGG_DATA H With(Nolock)
left join Apolo10.dbo.CTRL_LOTE C With(Nolock) on 
	Substring(H.Flock_id,CHARINDEX('-',H.Flock_id)+1,len(H.Flock_id)-CHARINDEX('-',H.Flock_id)+1) = C.CtrlLoteNum and
	H.Lay_date = C.CtrlLoteDataValid
where Set_date = @SetDate and Hatch_loc = @Hatch_Loc
	and Flock_id = @Flock_id and Machine = @Machine and ClassOvo = @ClassOvo
	and @Transf_date = CONVERT(datetime, '1988-01-01') and @Hatcher = 'G01'
	and @Horario_Inicio = '00:00' and @Contaminado_Transferencia = '0' and @Contaminado_Rodizio = '0' and @Bicados = '0'
	and @Trincados_Transferencia = '0' and @Trincados_Rodizio = '0' and @Num_Grudados = '0' and @Pintos_Nascidos = '0'
	and @Perdidos_Transferencia = '0' and @Perdidos_Rodizio = '0' and @Claros = '0' and @Hora_Termino = '00:00' 
group by
	Hatch_loc,
	Set_date,
	Machine,
	Flock_id,
	Egg_key,
	--C.USERIdateLoteFLIP,
	--Lay_date,
                   Variety,
	ClassOvo
having
    (SUM(Eggs_rcvd)
	-
	ISNULL((select SUM(T.Qtde_Ovos_Transferidos) from HATCHERY_TRAN_DATA T With(Nolock)
	 where H.Flock_id = T.Flock_id --and H.Lay_date = T.Lay_date
	 and H.Machine = T.Setter and H.Hatch_loc = T.Hatch_loc
	 and H.Set_date = T.Set_date and H.ClassOvo = T.ClassOvo),0)) &gt; 0
order by
	1,2,3,4,5">
                                    <SelectParameters>
                                        <asp:ControlParameter ControlID="Calendar1" Name="SetDate" 
                                            PropertyName="SelectedDate" />
                                        <asp:ControlParameter ControlID="ddlIncubatorios" Name="Hatch_Loc" 
                                            PropertyName="SelectedValue" />
                                        <asp:Parameter DefaultValue="" Name="FLOCK_ID" />
                                        <asp:Parameter DefaultValue="" Name="Machine" />
                                        <asp:Parameter Name="ClassOvo" />
                                        <asp:Parameter DefaultValue="1988-01-01" Name="Transf_date" />
                                        <asp:Parameter DefaultValue="G01" Name="Hatcher" />
                                        <asp:Parameter DefaultValue="00:00" Name="Horario_Inicio" />
                                        <asp:Parameter DefaultValue="0" Name="Contaminado_Transferencia" />
                                        <asp:Parameter DefaultValue="0" Name="Contaminado_Rodizio" />
                                        <asp:Parameter DefaultValue="0" Name="Bicados" />
                                        <asp:Parameter DefaultValue="0" Name="Trincados_Transferencia" />
                                        <asp:Parameter DefaultValue="0" Name="Trincados_Rodizio" />
                                        <asp:Parameter DefaultValue="0" Name="Num_Grudados" />
                                        <asp:Parameter DefaultValue="0" Name="Pintos_Nascidos" />
                                        <asp:Parameter DefaultValue="0" Name="Perdidos_Transferencia" />
                                        <asp:Parameter DefaultValue="0" Name="Perdidos_Rodizio" />
                                        <asp:Parameter DefaultValue="0" Name="Claros" />
                                        <asp:Parameter DefaultValue="00:00" Name="Hora_Termino" />
                                    </SelectParameters>
                                </asp:SqlDataSource>
                                <asp:Label ID="lblMensagem" runat="server" Style="font-weight: 700; color: #FF3300"
                                    Visible="False"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td class="style64" colspan="3">
                                <br />
                                <asp:Label ID="Label2" runat="server" Font-Bold="True" Font-Size="Small" Font-Underline="False"
                                    Text="OVOS TRANSFERIDOS"></asp:Label>
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
                                                <asp:ListItem>Lote</asp:ListItem>
                                                <asp:ListItem Value="Data de Produção">Data de Produção</asp:ListItem>
                                                <asp:ListItem>Incubadora</asp:ListItem>
                                                <asp:ListItem>Nascedouro</asp:ListItem>
                                            </asp:DropDownList>
                                            &nbsp;
                                        </td>
                                        <td class="style39" colspan="2">
                                            <asp:Button ID="Button2" runat="server" Font-Bold="True" Font-Size="XX-Small" Height="21px"
                                                Text="Pesquisar" Width="118px" CssClass="style60" />
                                            &nbsp;
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
                                    DataKeyNames="ID,Hatch_Loc,Set_date,Flock_id,NumLote,Lay_date,Setter,Hatcher,ClassOvo,Variety" 
                                    onrowdeleting="GridView1_RowDeleting" 
                                    onrowdeleted="GridView1_RowDeleted">
                                    <AlternatingRowStyle BackColor="White" />
                                    <Columns>
                                        <asp:TemplateField ShowHeader="False">
                                            <ItemTemplate>
                                                <asp:ImageButton ID="ibtnDeleteItemTranferido" runat="server" 
                                                    CausesValidation="False" CommandName="Delete" 
                                                    ImageUrl="~/Content/images/Nao.png" onclick="ibtnDeleteItemTranferido_Click" 
                                                    Text="Delete" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="ID" HeaderText="ID" 
                                            ReadOnly="True" SortExpression="ID" />
                                        <asp:BoundField DataField="Variety" HeaderText="Linhagem" ReadOnly="True" 
                                            SortExpression="Variety" />
                                        <asp:BoundField DataField="Flock_id" HeaderText="Lote Completo" 
                                            SortExpression="Flock_id" ReadOnly="True" />
                                        <asp:BoundField DataField="NumLote" HeaderText="Nº Lote" 
                                            SortExpression="NumLote" ReadOnly="True" />
                                        <asp:BoundField DataField="Lay_date" DataFormatString="{0:d}" 
                                            HeaderText="Maior Data Prd." SortExpression="Lay_date" ReadOnly="True" />
                                        <asp:BoundField DataField="Setter" HeaderText="Incubadora" 
                                            SortExpression="Setter" ReadOnly="True" />
                                        <asp:BoundField DataField="Hatcher" HeaderText="Nascedouro" 
                                            SortExpression="Hatcher" ReadOnly="True" />
                                        <asp:BoundField DataField="ClassOvo" HeaderText="Class. Ovo" 
                                            SortExpression="ClassOvo" ReadOnly="True" />
                                        <asp:BoundField DataField="Transf_date" DataFormatString="{0:d}" 
                                            HeaderText="Data Transf." SortExpression="Transf_date" />
                                        <asp:BoundField DataField="Qtde_Ovos_Transferidos" HeaderText="Ovos Transf." 
                                            SortExpression="Qtde_Ovos_Transferidos" />
                                        <asp:BoundField DataField="Hora_Inicio" HeaderText="Início" 
                                            SortExpression="Hora_Inicio" />
                                        <asp:BoundField DataField="Contaminado_Transferencia" 
                                            HeaderText="Cont. Transf." SortExpression="Contaminado_Transferencia" />
                                        <asp:BoundField DataField="Contaminado_Rodizio" HeaderText="Cont. Rodízio" 
                                            SortExpression="Contaminado_Rodizio" />
                                        <asp:BoundField DataField="Bicados" HeaderText="Bicados" 
                                            SortExpression="Bicados" />
                                        <asp:BoundField DataField="Trincados_Transferencia" 
                                            HeaderText="Trincados Transf." SortExpression="Trincados_Transferencia" />
                                        <asp:BoundField DataField="Trincados_Rodizio" HeaderText="Trincados Rodízio" 
                                            SortExpression="Trincados_Rodizio" />
                                        <asp:BoundField DataField="Num_Grudados" HeaderText="Grudados" 
                                            SortExpression="Num_Grudados" />
                                        <asp:BoundField DataField="Pintos_Nascidos" HeaderText="Nascidos" 
                                            SortExpression="Pintos_Nascidos" />
                                        <asp:BoundField DataField="Perdidos_Transferencia" 
                                            HeaderText="Perdidos Transf." SortExpression="Perdidos_Transferencia" />
                                        <asp:BoundField DataField="Perdidos_Rodizio" HeaderText="Perdidos Rodízio" 
                                            SortExpression="Perdidos_Rodizio" />
                                        <asp:BoundField DataField="Claros" HeaderText="Claros" 
                                            SortExpression="Claros" />
                                        <asp:BoundField DataField="Hora_Termino" HeaderText="Término" 
                                            SortExpression="Hora_Termino" />
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
                                <asp:SqlDataSource ID="HatchGridDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:HLBAPPConnectionString %>"
                                    
                                    SelectCommand="SELECT * FROM [HATCHERY_TRAN_DATA] WHERE 
(([Hatch_Loc] = @Hatch_Loc) AND ([Set_date] = @Set_date) and
	(
        (@Pesquisa ='0')
        or 
        (@Campo = 'Linhagem' and Variety like '%' + @Pesquisa + '%')
        or
		(@Campo = 'Data de Produção' and Lay_date = Case When @Pesquisa = @Pesquisa Then '1988-01-01' Else @Pesquisa End)
        or
        (@Campo = 'Lote' and Flock_id like '%' + @Pesquisa + '%')
        or
        (@Campo = 'Incubadora' and Setter like '%' + @Pesquisa + '%')
        or
        (@Campo = 'Nascedouro' and Hatcher like '%' + @Pesquisa + '%')
    )
)" 
                                    UpdateCommand="UPDATE [HATCHERY_TRAN_DATA] SET [Hatch_Loc] = @Hatch_Loc, [Set_date] = @Set_date, [Flock_id] = @Flock_id, [NumLote] = @NumLote, [Lay_date] = @Lay_date, [Setter] = @Setter, [Hatcher] = @Hatcher, [ClassOvo] = @ClassOvo, [Transf_date] = @Transf_date, [Hora_Inicio] = @Hora_Inicio, [Contaminado_Transferencia] = @Contaminado_Transferencia, [Contaminado_Rodizio] = @Contaminado_Rodizio, [Bicados] = @Bicados, [Trincados_Transferencia] = @Trincados_Transferencia, [Trincados_Rodizio] = @Trincados_Rodizio, [Num_Grudados] = @Num_Grudados, [Pintos_Nascidos] = @Pintos_Nascidos, [Perdidos_Transferencia] = @Perdidos_Transferencia, [Perdidos_Rodizio] = @Perdidos_Rodizio, [Hora_Termino] = @Hora_Termino, [Qtde_Ovos_Transferidos] = @Qtde_Ovos_Transferidos WHERE [ID] = @ID" 
                                    DeleteCommand="DELETE FROM [HATCHERY_TRAN_DATA] WHERE [ID] = @ID" 
                                    InsertCommand="INSERT INTO [HATCHERY_TRAN_DATA] ([ID], [Hatch_Loc], [Set_date], [Flock_id], [NumLote], [Lay_date], [Setter], [Hatcher], [ClassOvo], [Transf_date], [Hora_Inicio], [Contaminado_Transferencia], [Contaminado_Rodizio], [Bicados], [Trincados_Transferencia], [Trincados_Rodizio], [Num_Grudados], [Pintos_Nascidos], [Perdidos_Transferencia], [Perdidos_Rodizio], [Hora_Termino], [Qtde_Ovos_Transferidos]) VALUES (@ID, @Hatch_Loc, @Set_date, @Flock_id, @NumLote, @Lay_date, @Setter, @Hatcher, @ClassOvo, @Transf_date, @Hora_Inicio, @Contaminado_Transferencia, @Contaminado_Rodizio, @Bicados, @Trincados_Transferencia, @Trincados_Rodizio, @Num_Grudados, @Pintos_Nascidos, @Perdidos_Transferencia, @Perdidos_Rodizio, @Hora_Termino, @Qtde_Ovos_Transferidos)">
                                    <DeleteParameters>
                                        <asp:Parameter Name="ID" Type="Int32" />
                                    </DeleteParameters>
                                    <InsertParameters>
                                        <asp:Parameter Name="ID" Type="Int32" />
                                        <asp:Parameter Name="Hatch_Loc" Type="String" />
                                        <asp:Parameter Name="Set_date" Type="DateTime" />
                                        <asp:Parameter Name="Flock_id" Type="String" />
                                        <asp:Parameter Name="NumLote" Type="String" />
                                        <asp:Parameter Name="Lay_date" Type="DateTime" />
                                        <asp:Parameter Name="Setter" Type="String" />
                                        <asp:Parameter Name="Hatcher" Type="String" />
                                        <asp:Parameter Name="ClassOvo" Type="String" />
                                        <asp:Parameter Name="Transf_date" Type="DateTime" />
                                        <asp:Parameter Name="Hora_Inicio" Type="String" />
                                        <asp:Parameter Name="Contaminado_Transferencia" Type="Int32" />
                                        <asp:Parameter Name="Contaminado_Rodizio" Type="Int32" />
                                        <asp:Parameter Name="Bicados" Type="Int32" />
                                        <asp:Parameter Name="Trincados_Transferencia" Type="Int32" />
                                        <asp:Parameter Name="Trincados_Rodizio" Type="Int32" />
                                        <asp:Parameter Name="Num_Grudados" Type="Int32" />
                                        <asp:Parameter Name="Pintos_Nascidos" Type="Int32" />
                                        <asp:Parameter Name="Perdidos_Transferencia" Type="Int32" />
                                        <asp:Parameter Name="Perdidos_Rodizio" Type="Int32" />
                                        <asp:Parameter Name="Hora_Termino" Type="String" />
                                        <asp:Parameter Name="Qtde_Ovos_Transferidos" Type="Int32" />
                                    </InsertParameters>
                                    <SelectParameters>
                                        <asp:ControlParameter ControlID="ddlIncubatorios" Name="Hatch_Loc" 
                                            PropertyName="SelectedValue" Type="String" />
                                        <asp:ControlParameter ControlID="Calendar1" Name="Set_date" 
                                            PropertyName="SelectedDate" Type="DateTime" />
                                        <asp:ControlParameter ControlID="TextBox1" Name="Pesquisa" PropertyName="Text" />
                                        <asp:ControlParameter ControlID="DropDownList2" Name="Campo" PropertyName="SelectedValue" />
                                    </SelectParameters>
                                    <UpdateParameters>
                                        <asp:Parameter Name="Hatch_Loc" Type="String" />
                                        <asp:Parameter Name="Set_date" Type="DateTime" />
                                        <asp:Parameter Name="Flock_id" Type="String" />
                                        <asp:Parameter Name="NumLote" Type="String" />
                                        <asp:Parameter Name="Lay_date" Type="DateTime" />
                                        <asp:Parameter Name="Setter" Type="String" />
                                        <asp:Parameter Name="Hatcher" Type="String" />
                                        <asp:Parameter Name="ClassOvo" Type="String" />
                                        <asp:Parameter Name="Transf_date" Type="DateTime" />
                                        <asp:Parameter Name="Hora_Inicio" Type="String" />
                                        <asp:Parameter Name="Contaminado_Transferencia" Type="Int32" />
                                        <asp:Parameter Name="Contaminado_Rodizio" Type="Int32" />
                                        <asp:Parameter Name="Bicados" Type="Int32" />
                                        <asp:Parameter Name="Trincados_Transferencia" Type="Int32" />
                                        <asp:Parameter Name="Trincados_Rodizio" Type="Int32" />
                                        <asp:Parameter Name="Num_Grudados" Type="Int32" />
                                        <asp:Parameter Name="Pintos_Nascidos" Type="Int32" />
                                        <asp:Parameter Name="Perdidos_Transferencia" Type="Int32" />
                                        <asp:Parameter Name="Perdidos_Rodizio" Type="Int32" />
                                        <asp:Parameter Name="Hora_Termino" Type="String" />
                                        <asp:Parameter Name="Qtde_Ovos_Transferidos" Type="Int32" />
                                        <asp:Parameter Name="ID" Type="Int32" />
                                    </UpdateParameters>
                                </asp:SqlDataSource>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <table style="width: 100%;">
                                    <tr>
                                        <td class="style71">
                                            <asp:Label ID="lblTotalOvosIncubados" runat="server" 
                                                Text="Total de Ovos Transferidos:" CssClass="style67"></asp:Label>
                                        </td>
                                        <td class="style70" style="text-align: center">
                                            &nbsp;
                                            <asp:Label ID="lblQtdeOvosIncubados" runat="server" style="text-align: center"></asp:Label>
                                        </td>
                                        <td style="text-align: center; font-weight: 700;">
                                            &nbsp;
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
                                        <td colspan="3" class="style73">
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
                                                Font-Underline="False" Text="NASCEDOUROS"></asp:Label>
                                            <asp:GridView ID="gvMaquinas" runat="server" AutoGenerateColumns="False" 
                                                CellPadding="4" DataSourceID="MaquinasSqlDataSource" ForeColor="#333333" 
                                                GridLines="None" style="font-size: xx-small" Width="100%">
                                                <AlternatingRowStyle BackColor="White" />
                                                <Columns>
                                                    <asp:BoundField DataField="Nascedouro" HeaderText="Nascedouro" 
                                                        SortExpression="Nascedouro" />
                                                    <asp:BoundField DataField="Qtde." HeaderText="Qtde." ReadOnly="True" 
                                                        SortExpression="Qtde." DataFormatString="{0:N0}" />
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
                                                ConnectionString="<%$ ConnectionStrings:LayoutDb %>" SelectCommand="select Hatcher Nascedouro, SUM(Qtde_Ovos_Transferidos) [Qtde.]
from HATCHERY_TRAN_DATA
where Hatch_loc =@Incubatorio and Set_date = @SetDate
group by Hatcher
order by Hatcher">
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
                                                    <asp:BoundField DataField="Qtde." HeaderText="Qtde." ReadOnly="True" 
                                                        SortExpression="Qtde." DataFormatString="{0:N0}" />
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
                                                ConnectionString="<%$ ConnectionStrings:LayoutDb %>" SelectCommand="select NumLote Lotes, SUM(Qtde_Ovos_Transferidos) [Qtde.]
from HATCHERY_TRAN_DATA
where Hatch_loc = @Incubatorio and Set_date = @SetDate
group by NumLote
order by NumLote">
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
                                                    <asp:BoundField DataField="Qtde." HeaderText="Qtde." ReadOnly="True" 
                                                        SortExpression="Qtde." DataFormatString="{0:N0}" />
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
                                                ConnectionString="<%$ ConnectionStrings:LayoutDb %>" SelectCommand="select Variety Linhagem, SUM(Qtde_Ovos_Transferidos) [Qtde.]
from HATCHERY_TRAN_DATA
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
