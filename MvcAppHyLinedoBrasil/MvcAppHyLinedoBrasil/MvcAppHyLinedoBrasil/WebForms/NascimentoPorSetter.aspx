<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="NascimentoPorSetter.aspx.cs" Inherits="MvcAppHyLinedoBrasil.WebForms.NascimentoPorSetter" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Retirada / Nascimento de Pintos</title>
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
        .style73
        {
            height: 40px;
        }
    </style>
    <script language="javascript" type="text/javascript">
        function mascara(o, f) {
            v_obj = o
            v_fun = f
            setTimeout("execmascara()", 1)
        }
        function execmascara() {
            v_obj.value = v_fun(v_obj.value)
        }
        function mvalor(v) {
            v = v.replace(/\D/g, ""); //Remove tudo o que não é dígito
            v = v.replace(/(\d)(\d{8})$/, "$1.$2"); //coloca o ponto dos milhões
            v = v.replace(/(\d)(\d{5})$/, "$1.$2"); //coloca o ponto dos milhares

            v = v.replace(/(\d)(\d{2})$/, "$1,$2"); //coloca a virgula antes dos 2 últimos dígitos
            return v;
        }
    </script>
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
                                    Text="RETIRADA / NASCIMENTO DE PINTOS"></asp:Label>
                                <br />
                                <br />
                                <asp:Label ID="lblMensagem3" runat="server" 
                                    Style="font-weight: 700; color: #FF3300" Visible="False"></asp:Label>
                                <br />
                                <br />
                                <asp:Button ID="btnAtualizaFLIPAllPlanalto" runat="server" 
                                    onclick="btnAtualizaFLIPAllPlanalto_Click" 
                                    Text="Atualiza FLIP Total - Planalto" Visible="False" />
                            </td>
                            <td class="style58">
                                <asp:Label ID="Label9" runat="server" Text="Selecione o Incubatório:"  Style="font-size: xx-small;
                                    font-weight: 700"></asp:Label>
                                <br />
                                <asp:DropDownList ID="ddlIncubatorios" runat="server" AutoPostBack="True" 
                                    onselectedindexchanged="ddlIncubatorios_SelectedIndexChanged">
                                </asp:DropDownList>
                                <asp:SqlDataSource ID="IncsSqlDataSource" runat="server" 
                                    ConnectionString="<%$ ConnectionStrings:Oracle %>" 
                                    ProviderName="<%$ ConnectionStrings:Oracle.ProviderName %>" 
                                    SelectCommand="SELECT * FROM &quot;HATCHERY_CODES&quot;">
                                </asp:SqlDataSource>
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
                                                Height="21px" OnClick="Button1_Click" Text="Pesquisar" Width="118px" CssClass="style60" />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td style="text-align: center">
                                <table style="width: 100%; text-align: center;">
                                    <tr>
                                        <td align="center">
                                            <asp:Button ID="btnGerar" runat="server" 
                                                Text="GERAR CONTROLE DE INCUBAÇÃO / TRANSFERÊNCIA PARA ECLOSÃO"
                                                Style="font-weight: 700" 
                                                OnClick="btnGerar_Click" />      
                                            <asp:Button ID="btnGerar02" runat="server" 
                                                Text="GERAR CONTROLE DE ECLOSÃO / MÁQUINA / LOTE"
                                                Style="font-weight: 700" 
                                                OnClick="btnGerar02_Click" />      
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
                                        <asp:BoundField DataField="Incubadora" HeaderText="Incubadora" 
                                            SortExpression="Incubadora">
                                        </asp:BoundField>
                                        <asp:BoundField DataField="Nascedouro" HeaderText="Nascedouro" 
                                            SortExpression="Nascedouro" />
                                        <asp:BoundField DataField="Lote Completo" HeaderText="Lote Completo" 
                                            SortExpression="Lote Completo" />
                                        <asp:BoundField DataField="Lote" HeaderText="Lote" 
                                            SortExpression="Lote" />
                                        <asp:BoundField DataField="Linhagem" HeaderText="Linhagem" 
                                            SortExpression="Linhagem" />
                                        <asp:BoundField DataField="Idade Lote" HeaderText="Idade Lote" 
                                            SortExpression="Idade Lote" ReadOnly="True" />
                                        <asp:BoundField DataField="Class. Ovo" HeaderText="Class. Ovo" 
                                            SortExpression="Class. Ovo" />
                                        <asp:BoundField DataField="Ovos Incubados" HeaderText="Ovos Incubados" 
                                            ReadOnly="True" SortExpression="Ovos Incubados" DataFormatString="{0:N0}" />
                                        <asp:BoundField DataField="Digitado" HeaderText="Digitado" 
                                            SortExpression="Digitado" />
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
	Case When H.Hatch_loc = 'CH' Then 'Todas' Else H.Setter End Incubadora,
	Case When H.Hatch_loc = 'CH' Then 'Todos' Else H.Hatcher End Nascedouro,
	H.Flock_id [Lote Completo],
	H.NumLote Lote,
	H.Variety Linhagem,
	AVG(C.Age) [Idade Lote],
	H.ClassOvo [Class. Ovo],
	SUM(H.Qtde_Ovos_Transferidos) [Ovos Incubados],
                  Case When (select COUNT(1) from HATCHERY_FLOCK_SETTER_DATA T With(Nolock)
			 where H.Flock_id = T.Flock_id
			 and Case When H.Hatch_loc = 'CH' Then 'Todas' Else H.Setter End = T.Setter and H.Hatch_loc = T.Hatch_loc
			 and H.Set_date = T.Set_date and H.ClassOvo = T.ClassOvo) = 0 Then '' Else 'OK'                    End Digitado
from HLBAPP.dbo.HATCHERY_TRAN_DATA H With(Nolock)
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
	Case When H.Hatch_loc = 'CH' Then 'Todas' Else H.Setter End,
	Case When H.Hatch_loc = 'CH' Then 'Todos' Else H.Hatcher End,
	H.Flock_id,
	H.Variety,
	H.NumLote,
	H.ClassOvo
order by
	1">
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
                                                <td colspan="2">
                                                    <asp:Label ID="lblIncubadora" runat="server" Text="Incubadora:" />
                                                </td>
                                                <td colspan="2">
                                                    <asp:Label ID="IncubadoraLabel1" runat="server" 
                                                        Text='<%# Eval("Incubadora") %>' />
                                                    <br />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="2">
                                                    <asp:Label ID="lblNascedouro" runat="server" Text="Nascedouro:" />
                                                </td>
                                                <td colspan="2">
                                                    <asp:Label ID="NascedouroLabel1" runat="server" 
                                                        Text='<%# Eval("Nascedouro") %>' />
                                                    <br />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="2">
                                                    <asp:Label ID="lblLoteCompleto" runat="server" Text="Lote Completo:" />
                                                </td>
                                                <td colspan="2">
                                                    <asp:Label ID="Flock_idLabel1" runat="server" 
                                                        Text='<%# Eval("Flock_id") %>' />
                                                    <br />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="2">
                                                    <asp:Label ID="lblNumLote" runat="server" Text="Nº Lote:" />
                                                </td>
                                                <td colspan="2">
                                                    <asp:Label ID="NumLoteLabel1" runat="server" 
                                                        Text='<%# Eval("NumLote") %>' />
                                                    <br />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="2">
                                                    <asp:Label ID="lblLinhagem" runat="server" Text="Linhagem:" />
                                                </td>
                                                <td colspan="2">
                                                    <asp:Label ID="VarietyLabel1" runat="server" 
                                                        Text='<%# Eval("Variety") %>' />
                                                    <br />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="2">
                                                    <asp:Label ID="lblIdadeLote" runat="server" Text="Idade Lote:" />
                                                </td>
                                                <td colspan="2">
                                                    <asp:Label ID="Idade_LoteLabel1" runat="server" 
                                                        Text='<%# Eval("Idade_Lote") %>' />
                                                    <br />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="2">
                                                    <asp:Label ID="lblClasOvo" runat="server" Text="Class. Ovo:" />
                                                </td>
                                                <td colspan="2">
                                                    <asp:Label ID="ClassOvoLabel1" runat="server" 
                                                        Text='<%# Eval("ClassOvo") %>' />
                                                    <br />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="2">
                                                    <asp:Label ID="lblOvosInc" runat="server" Text="Ovos Inc.:" />
                                                </td>
                                                <td colspan="2">
                                                    <asp:Label ID="Ovos_IncubadosLabel1" runat="server" 
                                                        Text='<%# Eval("Ovos_Incubados") %>' />
                                                    <br />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="4">
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="4">
                                                    <u><asp:Label ID="lblRetirada" runat="server" Text="RETIRADA" /></u>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="2">
                                                    <asp:Label ID="lblDataRetiradaReal" runat="server" Text="Data Retirada Real:" />
                                                </td>
                                                <td colspan="2">
                                                    <asp:TextBox ID="DataRetiradaRealTextBox" runat="server" 
                                                        Text='<%# Bind("DataRetiradaReal") %>'
                                                        Height="23px" Width="96px" />
                                                    <asp:CalendarExtender ID="DataRetiradaRealTextBox_CalendarExtender" runat="server" 
                                                        Enabled="True" Format="dd/MM/yyyy" TargetControlID="DataRetiradaRealTextBox" 
                                                        TodaysDateFormat="dd/MM/yyyy">
                                                    </asp:CalendarExtender>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblHoraRetirada01" runat="server" Text="Horario 1º Retirada:" />
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="Hora_01_RetiradaTextBox" runat="server" Height="23px" 
                                                        Text='<%# Bind("Hora_01_Retirada") %>'
                                                        Width="56px" />
                                                    <asp:MaskedEditExtender ID="MaskedEditExtender1" runat="server" 
                                                        TargetControlID="Hora_01_RetiradaTextBox"
                                                        Mask="99:99" MessageValidatorTip="true" OnFocusCssClass="MaskedEditFocus" 
                                                        OnInvalidCssClass="MaskedEditError"
                                                        MaskType="Time" AcceptAMPM="True" ErrorTooltipEnabled="True">
                                                    </asp:MaskedEditExtender>
                                                </td>
                                                <td>
                                                    <asp:Label ID="lblQtdeRetirada01" runat="server" Text="Qtde. 1ª Retirada:" />
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="Qtde_01_RetiradaTextBox" runat="server" Height="23px" 
                                                        Text='<%# Bind("Qtde_01_Retirada") %>'
                                                        Width="56px" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblHoraRetirada02" runat="server" Text="Horario 2º Retirada:" />
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="Hora_02_RetiradaTextBox" runat="server" Height="23px" 
                                                        Text='<%# Bind("Hora_02_Retirada") %>'
                                                        Width="56px" />
                                                    <asp:MaskedEditExtender ID="MaskedEditExtender2" runat="server" 
                                                        TargetControlID="Hora_02_RetiradaTextBox"
                                                        Mask="99:99" MessageValidatorTip="true" OnFocusCssClass="MaskedEditFocus" 
                                                        OnInvalidCssClass="MaskedEditError"
                                                        MaskType="Time" AcceptAMPM="True" ErrorTooltipEnabled="True">
                                                    </asp:MaskedEditExtender>
                                                </td>
                                                <td>
                                                    <asp:Label ID="lblQtdeRetirada02" runat="server" Text="Qtde. 2ª Retirada:" />
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="Qtde_02_RetiradaTextBox" runat="server" Height="23px" 
                                                        Text='<%# Bind("Qtde_02_Retirada") %>'
                                                        Width="56px" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="4">
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="4">
                                                    <u><asp:Label ID="lblDadosEclosao" runat="server" Text="DADOS DE ECLOSÃO" /></u>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblEliminadoSobras" runat="server" Text="Eliminado - Sobras:" />
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="EliminadoTextBox" runat="server" Height="23px" 
                                                        Text='<%# Bind("Eliminado") %>'
                                                        Width="56px" />
                                                </td>
                                                <td>
                                                    <asp:Label ID="lblRefugo" runat="server" Text="Refugo:" />
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="RefugoTextBox" runat="server" Height="23px" 
                                                        Text='<%# Bind("Refugo") %>' Width="56px" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblMacho" runat="server" Text="Macho:" />
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="MachoTextBox" runat="server" Height="23px" 
                                                        Text='<%# Bind("Macho") %>'
                                                        Width="56px" />
                                                </td>
                                                <td>
                                                    <asp:Label ID="lblPintosVendaveis" runat="server" Text="Pintos Vendáveis:" />
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="Pintos_VendaveisTextBox" runat="server" Height="23px" 
                                                        Text='<%# Bind("Pintos_Vendaveis") %>'
                                                        Width="56px" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblPinto3" runat="server" Text="Pinto de 3ª:" />
                                                <td>
                                                    <asp:TextBox ID="Pinto_TerceiraTextBox" runat="server" Height="23px" 
                                                        Text='<%# Bind("Pinto_Terceira") %>' Width="56px" />
                                                </td>
                                                <td>
                                                    <asp:Label ID="lblEliminadoCanc" runat="server" Text="Eliminado - Cancelamento:" />
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="EliminadoCancelamentoTextBox" runat="server" Height="23px" 
                                                        Text='<%# Bind("EliminadoCancelamento") %>' Width="56px" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="4">
                                                    <asp:Panel ID="pnlDadosNascimentoHYBR" runat="server">
                                                        <br />
                                                        <table style="width: 100%;">
                                                            <tr>
                                                                <td>
                                                                    <asp:Label ID="lblPeso" runat="server" Text="Peso (g):" />
                                                                <td>
                                                                    <asp:TextBox ID="txtPeso" runat="server" Height="23px" 
                                                                        onkeyup = "mascara(this, mvalor);"
                                                                        required="required"
                                                                        Text='<%# Bind("Peso") %>'
                                                                        Width="56px" />
                                                                </td>
                                                                <td>
                                                                    <asp:Label ID="lblUniformidade" runat="server" Text="Uniformidade (%):" />
                                                                </td>
                                                                <td>
                                                                    <asp:TextBox ID="txtUniformidade" runat="server" Height="23px" 
                                                                        onkeyup = "mascara(this, mvalor);"
                                                                        Text='<%# Bind("Uniformidade") %>'
                                                                        Width="56px" />
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </asp:Panel>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="4">
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="4">
                                                    <u><asp:Label ID="lblEmbrio" runat="server" Text="EMBRIODIAGNÓSTICO" /></u>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="4">
                                                    <asp:Panel ID="pnlEmbrioHYBRMachines" runat="server">
                                                        <br />
                                                        <table style="width: 100%;">
                                                            <tr>
                                                                <td>
                                                                    <asp:Label ID="lblIncubadoraEmbrio" runat="server" Text="Incubadora:" />
                                                                <td>
                                                                    <asp:TextBox ID="txtIncubadoraEmbrio" runat="server" Height="23px" 
                                                                        Text='<%# Bind("SetterEmbrio") %>' Width="56px" />
                                                                    <asp:MaskedEditExtender ID="mtxtIncubadoraEmbrio" runat="server" TargetControlID="txtIncubadoraEmbrio"
                                                                        Mask="A99" MessageValidatorTip="true" OnFocusCssClass="MaskedEditFocus" OnInvalidCssClass="MaskedEditError"
                                                                        MaskType="Number" ErrorTooltipEnabled="True" ClearMaskOnLostFocus="False">
                                                                    </asp:MaskedEditExtender>
                                                                </td>
                                                                <td>
                                                                    <asp:Label ID="lblNascedouroEmbrio" runat="server" Text="Nascedouro:" />
                                                                </td>
                                                                <td>
                                                                    <asp:TextBox ID="txtNascedouroEmbrio" runat="server" Height="23px" 
                                                                        Text='<%# Bind("HatcherEmbrio") %>' Width="56px" />
                                                                    <asp:MaskedEditExtender ID="mtxtNascedouroEmbrio" runat="server" TargetControlID="txtNascedouroEmbrio"
                                                                        Mask="A99" MessageValidatorTip="true" OnFocusCssClass="MaskedEditFocus" OnInvalidCssClass="MaskedEditError"
                                                                        MaskType="Number" ErrorTooltipEnabled="True" ClearMaskOnLostFocus="False">
                                                                    </asp:MaskedEditExtender>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </asp:Panel>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblAmostra" runat="server" Text="Amostra:" />
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="AmostraTextBox" runat="server" Height="23px" 
                                                        Text='<%# Bind("Amostra") %>'
                                                        Width="56px" />
                                                </td>
                                                <td>
                                                    <asp:Label ID="lblInfertil" runat="server" Text="Infértil:" />
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="InfertilTextBox" runat="server" Height="23px" 
                                                        Text='<%# Bind("Infertil") %>'
                                                        Width="56px" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblInicial0a3" runat="server" Text="Inicial (1-3):" />
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="Inicial0a3TextBox" runat="server" Height="23px" 
                                                        Text='<%# Bind("Inicial0a3") %>'
                                                        Width="56px" />
                                                </td>
                                                <td>
                                                    <asp:Label ID="lblInicial4a7" runat="server" Text="Inicial (4-7):" />
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="Inicial4a7TextBox" runat="server" Height="23px" 
                                                        Text='<%# Bind("Inicial4a7") %>'
                                                        Width="56px" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblMedia8a14" runat="server" Text="Média (8-14):" />
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="Media8a14TextBox" runat="server" Height="23px" 
                                                        Text='<%# Bind("Media8a14") %>'
                                                        Width="56px" />
                                                </td>
                                                <td>
                                                    <asp:Label ID="lblTardia15a18" runat="server" Text="Tardia (15-18):" />
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="Tardia15a18TextBox" runat="server" Height="23px" 
                                                        Text='<%# Bind("Tardia15a18") %>'
                                                        Width="56px" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblTardia19a21" runat="server" Text="Tardia (19-21):" />
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="Tardia19a21TextBox" runat="server" Height="23px" 
                                                        Text='<%# Bind("Tardia19a21") %>'
                                                        Width="56px" />
                                                </td>
                                                <td>
                                                    <asp:Label ID="lblContaminacaoBact" runat="server" Text="Contaminado:" />
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="ContaminacaoBacterianaTextBox" runat="server" Height="23px" 
                                                        Text='<%# Bind("ContaminacaoBacteriana") %>'
                                                        Width="56px" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblBicadoVivo" runat="server" Text="Bicado Vivo:" />
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="BicadoVivoTextBox" runat="server" Height="23px" 
                                                        Text='<%# Bind("BicadoVivo") %>'
                                                        Width="56px" />
                                                </td>
                                                <td>
                                                    <asp:Label ID="lblBicadoMorto" runat="server" Text="Bicado Morto:" />
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="BicadoMortoTextBox" runat="server" Height="23px" 
                                                        Text='<%# Bind("BicadoMorto") %>'
                                                        Width="56px" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblMaFormacaoCerebro" runat="server" Text="Má formação (Cérebro exposto):" />
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="MaFormacaoCerebroTextBox" runat="server" Height="23px" 
                                                        Text='<%# Bind("MaFormacaoCerebro") %>'
                                                        Width="56px" />
                                                </td>
                                                <td>
                                                    <asp:Label ID="lblMaFormacaoVisceras" runat="server" Text="Má formação (Vísceras expostas):" />
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="MaFormacaoViscerasTextBox" runat="server" Height="23px" 
                                                        Text='<%# Bind("MaFormacaoVisceras") %>'
                                                        Width="56px" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblMalPosicionado" runat="server" Text="Má Posição:" />
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="MalPosicionadoTextBox" runat="server" Height="23px" 
                                                        Text='<%# Bind("MalPosicionado") %>'
                                                        Width="56px" />
                                                </td>
                                                <td>
                                                    <asp:Label ID="lblHemorragico" runat="server" Text="Hemorrágico:" />
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="HemorragicoTextBox" runat="server" Height="23px" 
                                                        Text='<%# Bind("Hemorragico") %>'
                                                        Width="56px" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblFungo" runat="server" Text="Fungo:" />
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="FungoTextBox" runat="server" Height="23px" 
                                                        Text='<%# Bind("Fungo") %>'
                                                        Width="56px" />
                                                </td>
                                                <td>
                                                    <asp:Label ID="lblAnormalidade" runat="server" Text="Anormalidade:" />
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="AnormalidadeTextBox" runat="server" Height="23px" 
                                                        Text='<%# Bind("Anormalidade") %>'
                                                        Width="56px" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="4">
                                                    <asp:Panel ID="pnlEmbrioHYBRDados" runat="server">
                                                        <br />
                                                        <table style="width: 100%;">
                                                            <tr>
                                                                <td>
                                                                    <asp:Label ID="lblOvoVirado" runat="server" Text="Ovo Virado:" />
                                                                </td>
                                                                <td>
                                                                    <asp:TextBox ID="txtOvoVirado" runat="server" Height="23px" 
                                                                        Text='<%# Bind("OvoVirado") %>'
                                                                        Width="56px" />
                                                                </td>
                                                                <td>
                                                                    <asp:Label ID="lblQuebradoTrincado" runat="server" Text="Ovo Quebrado / Trincado:" />
                                                                </td>
                                                                <td>
                                                                    <asp:TextBox ID="txtQuebradoTrincado" runat="server" Height="23px" 
                                                                        Text='<%# Bind("QuebradoTrincado") %>'
                                                                        Width="56px" />
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td>
                                                                    <asp:Label ID="lblPerdaUmidade" runat="server" Text="% Perda de Umidade:" />
                                                                </td>
                                                                <td>
                                                                    <asp:TextBox ID="txtPerdaUmidade" runat="server" Height="23px" 
                                                                        onkeyup = "mascara(this, mvalor);"
                                                                        Text='<%# Bind("PerdaUmidade") %>'
                                                                        Width="56px" />
                                                                </td>
                                                                <td>
                                                                    <asp:Label ID="lblChickYeld" runat="server" Text="% Chick Yeld:" />
                                                                </td>
                                                                <td>
                                                                    <asp:TextBox ID="txtChickYeld" runat="server" Height="23px" 
                                                                        onkeyup = "mascara(this, mvalor);"
                                                                        Text='<%# Bind("ChickYeld") %>'
                                                                        Width="56px" />
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td>
                                                                    <asp:Label ID="lblTempCloaca" runat="server" Text="Temp. Cloaca:" />
                                                                </td>
                                                                <td>
                                                                    <asp:TextBox ID="txtTempCloaca" runat="server" Height="23px" 
                                                                        onkeyup = "mascara(this, mvalor);"
                                                                        Text='<%# Bind("TempCloaca") %>'
                                                                        Width="56px" />
                                                                </td>
                                                                <td>
                                                                    <asp:Label ID="lblQtdeNascidos" runat="server" Text="Qtde. Nascidos:" />
                                                                </td>
                                                                <td>
                                                                    <asp:TextBox ID="txtQtdeNascidos" runat="server" Height="23px" 
                                                                        Text='<%# Bind("QtdeNascidos") %>'
                                                                        Width="56px" />
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </asp:Panel>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                </td>
                                                <td>
                                                    <asp:LinkButton ID="UpdateButton" runat="server" CausesValidation="True"
                                                        Text="SALVAR" onclick="UpdateButton_Click" />
                                                </td>
                                            </tr>
                                        </table>
                                    </EditItemTemplate>
                                    <EditRowStyle BackColor="#FFCC66" Font-Bold="True" ForeColor="#663399" />
                                    <FooterStyle BackColor="#FFFFCC" ForeColor="#330099" />
                                    <HeaderStyle BackColor="#990000" Font-Bold="True" ForeColor="#FFFFCC" />
                                    <InsertItemTemplate>
                                        Incubadora:
                                        <asp:TextBox ID="IncubadoraTextBox" runat="server" 
                                            Text='<%# Bind("Incubadora") %>' />
                                        <br />
                                        Nascedouro:
                                        <asp:TextBox ID="NascedouroTextBox" runat="server" 
                                            Text='<%# Bind("Nascedouro") %>' />
                                        <br />
                                        Flock_id:
                                        <asp:TextBox ID="Flock_idTextBox" runat="server" 
                                            Text='<%# Bind("Flock_id") %>' />
                                        <br />
                                        NumLote:
                                        <asp:TextBox ID="NumLoteTextBox" runat="server" 
                                            Text='<%# Bind("NumLote") %>' />
                                        <br />
                                        Variety:
                                        <asp:TextBox ID="VarietyTextBox" runat="server" 
                                            Text='<%# Bind("Variety") %>' />
                                        <br />
                                        Idade_Lote:
                                        <asp:TextBox ID="Idade_LoteTextBox" runat="server" 
                                            Text='<%# Bind("Idade_Lote") %>' />
                                        <br />
                                        ClassOvo:
                                        <asp:TextBox ID="ClassOvoTextBox" runat="server" 
                                            Text='<%# Bind("ClassOvo") %>' />
                                        <br />
                                        Ovos_Incubados:
                                        <asp:TextBox ID="Ovos_IncubadosTextBox" runat="server" 
                                            Text='<%# Bind("Ovos_Incubados") %>' />
                                        <br />
                                        DataRetiradaReal:
                                        <asp:TextBox ID="DataRetiradaRealTextBox" runat="server" 
                                            Text='<%# Bind("DataRetiradaReal") %>' />
                                        <br />
                                        Hora_01_Retirada:
                                        <asp:TextBox ID="Hora_01_RetiradaTextBox" runat="server" 
                                            Text='<%# Bind("Hora_01_Retirada") %>' />
                                        <br />
                                        Qtde_01_Retirada:
                                        <asp:TextBox ID="Qtde_01_RetiradaTextBox" runat="server" 
                                            Text='<%# Bind("Qtde_01_Retirada") %>' />
                                        <br />
                                        Hora_02_Retirada:
                                        <asp:TextBox ID="Hora_02_RetiradaTextBox" runat="server" 
                                            Text='<%# Bind("Hora_02_Retirada") %>' />
                                        <br />
                                        Qtde_02_Retirada:
                                        <asp:TextBox ID="Qtde_02_RetiradaTextBox" runat="server" 
                                            Text='<%# Bind("Qtde_02_Retirada") %>' />
                                        <br />
                                        Eliminado:
                                        <asp:TextBox ID="EliminadoTextBox" runat="server" 
                                            Text='<%# Bind("Eliminado") %>' />
                                        <br />
                                        Morto:
                                        <asp:TextBox ID="MortoTextBox" runat="server" 
                                            Text='<%# Bind("Morto") %>' />
                                        <br />
                                        Macho:
                                        <asp:TextBox ID="MachoTextBox" runat="server" 
                                            Text='<%# Bind("Macho") %>' />
                                        <br />
                                        Pintos_Vendaveis:
                                        <asp:TextBox ID="Pintos_VendaveisTextBox" runat="server" 
                                            Text='<%# Bind("Pintos_Vendaveis") %>' />
                                        <br />
                                        Refugo:
                                        <asp:TextBox ID="RefugoTextBox" runat="server" 
                                            Text='<%# Bind("Refugo") %>' />
                                        <br />
                                        Pinto_Terceira:
                                        <asp:TextBox ID="Pinto_TerceiraTextBox" runat="server" 
                                            Text='<%# Bind("Pinto_Terceira") %>' />
                                        <br />
                                        Inicial0a3:
                                        <asp:TextBox ID="Inicial0a3TextBox" runat="server" 
                                            Text='<%# Bind("Inicial0a3") %>' />
                                        <br />
                                        Inicial4a7:
                                        <asp:TextBox ID="Inicial4a7TextBox" runat="server" 
                                            Text='<%# Bind("Inicial4a7") %>' />
                                        <br />
                                        Media8a14:
                                        <asp:TextBox ID="Media8a14TextBox" runat="server" 
                                            Text='<%# Bind("Media8a14") %>' />
                                        <br />
                                        Tardia15a18:
                                        <asp:TextBox ID="Tardia15a18TextBox" runat="server" 
                                            Text='<%# Bind("Tardia15a18") %>' />
                                        <br />
                                        Tardia19a21:
                                        <asp:TextBox ID="Tardia19a21TextBox" runat="server" 
                                            Text='<%# Bind("Tardia19a21") %>' />
                                        <br />
                                        BicadoVivo:
                                        <asp:TextBox ID="BicadoVivoTextBox" runat="server" 
                                            Text='<%# Bind("BicadoVivo") %>' />
                                        <br />
                                        BicadoMorto:
                                        <asp:TextBox ID="BicadoMortoTextBox" runat="server" 
                                            Text='<%# Bind("BicadoMorto") %>' />
                                        <br />
                                        ContaminacaoBacteriana:
                                        <asp:TextBox ID="ContaminacaoBacterianaTextBox" runat="server" 
                                            Text='<%# Bind("ContaminacaoBacteriana") %>' />
                                        <br />
                                        Fungo:
                                        <asp:TextBox ID="FungoTextBox" runat="server" 
                                            Text='<%# Bind("Fungo") %>' />
                                        <br />
                                        MalPosicionado:
                                        <asp:TextBox ID="MalPosicionadoTextBox" runat="server" 
                                            Text='<%# Bind("MalPosicionado") %>' />
                                        <br />
                                        MaFormacaoCerebro:
                                        <asp:TextBox ID="MaFormacaoCerebroTextBox" runat="server" 
                                            Text='<%# Bind("MaFormacaoCerebro") %>' />
                                        <br />
                                        MaFormacaoVisceras:
                                        <asp:TextBox ID="MaFormacaoViscerasTextBox" runat="server" 
                                            Text='<%# Bind("MaFormacaoVisceras") %>' />
                                        <br />
                                        Hemorragico:
                                        <asp:TextBox ID="HemorragicoTextBox" runat="server" 
                                            Text='<%# Bind("Hemorragico") %>' />
                                        <br />
                                        Anormalidade:
                                        <asp:TextBox ID="AnormalidadeTextBox" runat="server" 
                                            Text='<%# Bind("Anormalidade") %>' />
                                        <br />
                                        Infertil:
                                        <asp:TextBox ID="InfertilTextBox" runat="server" 
                                            Text='<%# Bind("Infertil") %>' />
                                        <br />
                                        Amostra:
                                        <asp:TextBox ID="AmostraTextBox" runat="server" Text='<%# Bind("Amostra") %>' />
                                        <br />
                                        EliminadoCancelamento:
                                        <asp:TextBox ID="EliminadoCancelamentoTextBox" runat="server" Text='<%# Bind("EliminadoCancelamento") %>' />
                                        <br />
                                        PerdaUmidade:
                                        <asp:TextBox ID="PerdaUmidadeTextBox" runat="server" Text='<%# Bind("PerdaUmidade") %>' />
                                        <br />
                                        ChickYeld:
                                        <asp:TextBox ID="ChickYeldTextBox" runat="server" Text='<%# Bind("ChickYeld") %>' />
                                        <br />
                                        TempCloaca:
                                        <asp:TextBox ID="TempCloacaTextBox" runat="server" Text='<%# Bind("TempCloaca") %>' />
                                        <br />
                                        OvoVirado:
                                        <asp:TextBox ID="OvoViradoTextBox" runat="server" Text='<%# Bind("OvoVirado") %>' />
                                        <br />
                                        QuebradoTrincado:
                                        <asp:TextBox ID="QuebradoTrincadoTextBox" runat="server" Text='<%# Bind("QuebradoTrincado") %>' />
                                        <br />
                                        SetterEmbrio:
                                        <asp:TextBox ID="SetterEmbrioTextBox" runat="server" Text='<%# Bind("SetterEmbrio") %>' />
                                        <br />
                                        HatcherEmbrio:
                                        <asp:TextBox ID="HatcherEmbrioTextBox" runat="server" Text='<%# Bind("HatcherEmbrio") %>' />
                                        <br />
                                        QtdeNascidos:
                                        <asp:TextBox ID="QtdeNascidosTextBox" runat="server" Text='<%# Bind("QtdeNascidos") %>' />
                                        <br />
                                        <asp:LinkButton ID="InsertButton" runat="server" CausesValidation="True" CommandName="Insert"
                                            Text="Inserir" />
                                        &nbsp;<asp:LinkButton ID="InsertCancelButton" runat="server" CausesValidation="False"
                                            CommandName="Cancel" Text="Cancelar" />
                                    </InsertItemTemplate>
                                    <ItemTemplate>
                                        Incubadora:
                                        <asp:Label ID="IncubadoraLabel" runat="server" 
                                            Text='<%# Bind("Incubadora") %>' />
                                        <br />
                                        Nascedouro:
                                        <asp:Label ID="NascedouroLabel" runat="server" 
                                            Text='<%# Bind("Nascedouro") %>' />
                                        <br />
                                        Flock_id:
                                        <asp:Label ID="Flock_idLabel" runat="server" 
                                            Text='<%# Bind("Flock_id") %>' />
                                        <br />
                                        NumLote:
                                        <asp:Label ID="NumLoteLabel" runat="server" Text='<%# Bind("NumLote") %>' />
                                        <br />
                                        Variety:
                                        <asp:Label ID="VarietyLabel" runat="server" Text='<%# Bind("Variety") %>' />
                                        <br />
                                        Idade_Lote:
                                        <asp:Label ID="Idade_LoteLabel" runat="server" 
                                            Text='<%# Bind("Idade_Lote") %>' />
                                        <br />
                                        ClassOvo:
                                        <asp:Label ID="ClassOvoLabel" runat="server" 
                                            Text='<%# Bind("ClassOvo") %>' />
                                        <br />
                                        Ovos_Incubados:
                                        <asp:Label ID="Ovos_IncubadosLabel" runat="server" 
                                            Text='<%# Bind("Ovos_Incubados") %>' />
                                        <br />
                                        DataRetiradaReal:
                                        <asp:Label ID="DataRetiradaRealLabel" runat="server" 
                                            Text='<%# Bind("DataRetiradaReal") %>' />
                                        <br />
                                        Hora_01_Retirada:
                                        <asp:Label ID="Hora_01_RetiradaLabel" runat="server" 
                                            Text='<%# Bind("Hora_01_Retirada") %>' />
                                        <br />
                                        Qtde_01_Retirada:
                                        <asp:Label ID="Qtde_01_RetiradaLabel" runat="server" 
                                            Text='<%# Bind("Qtde_01_Retirada") %>' />
                                        <br />
                                        Hora_02_Retirada:
                                        <asp:Label ID="Hora_02_RetiradaLabel" runat="server" 
                                            Text='<%# Bind("Hora_02_Retirada") %>' />
                                        <br />
                                        Qtde_02_Retirada:
                                        <asp:Label ID="Qtde_02_RetiradaLabel" runat="server" 
                                            Text='<%# Bind("Qtde_02_Retirada") %>' />
                                        <br />
                                        Eliminado:
                                        <asp:Label ID="EliminadoLabel" runat="server" 
                                            Text='<%# Bind("Eliminado") %>' />
                                        <br />
                                        Morto:
                                        <asp:Label ID="MortoLabel" runat="server" 
                                            Text='<%# Bind("Morto") %>' />
                                        <br />
                                        Macho:
                                        <asp:Label ID="MachoLabel" runat="server" 
                                            Text='<%# Bind("Macho") %>' />
                                        <br />
                                        Pintos_Vendaveis:
                                        <asp:Label ID="Pintos_VendaveisLabel" runat="server" 
                                            Text='<%# Bind("Pintos_Vendaveis") %>' />
                                        <br />
                                        Refugo:
                                        <asp:Label ID="RefugoLabel" runat="server" 
                                            Text='<%# Bind("Refugo") %>' />
                                        <br />
                                        Pinto_Terceira:
                                        <asp:Label ID="Pinto_TerceiraLabel" runat="server" 
                                            Text='<%# Bind("Pinto_Terceira") %>' />
                                        <br />
                                        Inicial0a3:
                                        <asp:Label ID="Inicial0a3Label" runat="server" 
                                            Text='<%# Bind("Inicial0a3") %>' />
                                        <br />
                                        Inicial4a7:
                                        <asp:Label ID="Inicial4a7Label" runat="server" 
                                            Text='<%# Bind("Inicial4a7") %>' />
                                        <br />
                                        Media8a14:
                                        <asp:Label ID="Media8a14Label" runat="server" Text='<%# Bind("Media8a14") %>' />
                                        <br />
                                        Tardia15a18:
                                        <asp:Label ID="Tardia15a18Label" runat="server" 
                                            Text='<%# Bind("Tardia15a18") %>' />
                                        <br />
                                        Tardia19a21:
                                        <asp:Label ID="Tardia19a21Label" runat="server" 
                                            Text='<%# Bind("Tardia19a21") %>' />
                                        <br />
                                        BicadoVivo:
                                        <asp:Label ID="BicadoVivoLabel" runat="server" 
                                            Text='<%# Bind("BicadoVivo") %>' />
                                        <br />
                                        BicadoMorto:
                                        <asp:Label ID="BicadoMortoLabel" runat="server" 
                                            Text='<%# Bind("BicadoMorto") %>' />
                                        <br />
                                        ContaminacaoBacteriana:
                                        <asp:Label ID="ContaminacaoBacterianaLabel" runat="server" 
                                            Text='<%# Bind("ContaminacaoBacteriana") %>' />
                                        <br />
                                        Fungo:
                                        <asp:Label ID="FungoLabel" runat="server" 
                                            Text='<%# Bind("Fungo") %>' />
                                        <br />
                                        MalPosicionado:
                                        <asp:Label ID="MalPosicionadoLabel" runat="server" 
                                            Text='<%# Bind("MalPosicionado") %>' />
                                        <br />
                                        MaFormacaoCerebro:
                                        <asp:Label ID="MaFormacaoCerebroLabel" runat="server" 
                                            Text='<%# Bind("MaFormacaoCerebro") %>' />
                                        <br />
                                        MaFormacaoVisceras:
                                        <asp:Label ID="MaFormacaoViscerasLabel" runat="server" 
                                            Text='<%# Bind("MaFormacaoVisceras") %>' />
                                        <br />
                                        Hemorragico:
                                        <asp:Label ID="HemorragicoLabel" runat="server" 
                                            Text='<%# Bind("Hemorragico") %>' />
                                        <br />
                                        Anormalidade:
                                        <asp:Label ID="AnormalidadeLabel" runat="server" 
                                            Text='<%# Bind("Anormalidade") %>' />
                                        <br />
                                        Infertil:
                                        <asp:Label ID="InfertilLabel" runat="server" Text='<%# Bind("Infertil") %>' />
                                        <br />
                                        Amostra:
                                        <asp:Label ID="AmostraLabel" runat="server" Text='<%# Bind("Amostra") %>' />
                                        <br />
                                        EliminadoCancelamento:
                                        <asp:Label ID="EliminadoCancelamentoLabel" runat="server" Text='<%# Bind("EliminadoCancelamento") %>' />
                                        <br />
                                        PerdaUmidade:
                                        <asp:Label ID="PerdaUmidadeLabel" runat="server" Text='<%# Bind("PerdaUmidade") %>' />
                                        <br />
                                        ChickYeld:
                                        <asp:Label ID="ChickYeldLabel" runat="server" Text='<%# Bind("ChickYeld") %>' />
                                        <br />
                                        TempCloaca:
                                        <asp:Label ID="TempCloacaLabel" runat="server" Text='<%# Bind("TempCloaca") %>' />
                                        <br />
                                        OvoVirado:
                                        <asp:Label ID="OvoViradoLabel" runat="server" Text='<%# Bind("OvoVirado") %>' />
                                        <br />
                                        QuebradoTrincado:
                                        <asp:Label ID="QuebradoTrincadoLabel" runat="server" Text='<%# Bind("QuebradoTrincado") %>' />
                                        <br />
                                        SetterEmbrio:
                                        <asp:Label ID="SetterEmbrioLabel" runat="server" Text='<%# Bind("SetterEmbrio") %>' />
                                        <br />
                                        HatcherEmbrio:
                                        <asp:Label ID="HatcherEmbrioLabel" runat="server" Text='<%# Bind("HatcherEmbrio") %>' />
                                        <br />
                                        QtdeNascidos:
                                        <asp:Label ID="QtdeNascidosLabel" runat="server" Text='<%# Bind("QtdeNascidos") %>' />
                                        <br />
                                        Peso:
                                        <asp:Label ID="PesoLabel" runat="server" Text='<%# Bind("Peso") %>' />
                                        <br />
                                        Uniformidade:
                                        <asp:Label ID="UniformidadeLabel" runat="server" Text='<%# Bind("Uniformidade") %>' />
                                        <br />
                                    </ItemTemplate>
                                    <PagerStyle BackColor="#FFFFCC" ForeColor="#330099" HorizontalAlign="Center" />
                                    <RowStyle BackColor="White" ForeColor="#330099" />
                                </asp:FormView>
                                <asp:SqlDataSource ID="HatchFormDataSource" runat="server" 
                                    ConnectionString="<%$ ConnectionStrings:HLBAPPConnectionString %>" 
                                    
                                    
                                    
                                    
                                    
                                    SelectCommand="select
	Case When Hatch_loc = 'CH' Then 'Todas' Else Setter End Incubadora,
	Case When Hatch_loc = 'CH' Then 'Todos' Else Hatcher End Nascedouro,
	Flock_id,
	NumLote,
	Variety,
	AVG(C.USERIdateLoteFLIP) Idade_Lote,
	ClassOvo,
	SUM(Qtde_Ovos_Transferidos) Ovos_Incubados,
	@DataRetiradaReal DataRetiradaReal,
	@Hora_01_Retirada Hora_01_Retirada, @Qtde_01_Retirada Qtde_01_Retirada,
	@Hora_02_Retirada Hora_02_Retirada, @Qtde_02_Retirada Qtde_02_Retirada,
	@Eliminado Eliminado, @Morto Morto, @Macho Macho, @Pintos_Vendaveis Pintos_Vendaveis,
	@Refugo Refugo, @Pinto_Terceira Pinto_Terceira, @Inicial0a3 Inicial0a3, @Inicial4a7 Inicial4a7, @Media8a14 Media8a14,
    @Tardia15a18 Tardia15a18, @Tardia19a21 Tardia19a21,
    @BicadoVivo BicadoVivo, @BicadoMorto BicadoMorto, @ContaminacaoBacteriana ContaminacaoBacteriana, @Fungo Fungo,
    @MalPosicionado MalPosicionado, @MaFormacaoCerebro MaFormacaoCerebro, @MaFormacaoVisceras MaFormacaoVisceras,
    @Hemorragico Hemorragico, @Anormalidade Anormalidade, @Infertil Infertil, @Amostra Amostra,
    @EliminadoCancelamento EliminadoCancelamento,
    @PerdaUmidade PerdaUmidade,
    @ChickYeld ChickYeld,
    @TempCloaca TempCloaca,
    @OvoVirado OvoVirado,
    @QuebradoTrincado QuebradoTrincado,
    @SetterEmbrio SetterEmbrio,
    @HatcherEmbrio HatcherEmbrio,
    @QtdeNascidos QtdeNascidos,
    @Peso Peso,
    @Uniformidade Uniformidade
from HLBAPP.dbo.HATCHERY_TRAN_DATA H With(Nolock)
left join Apolo10.dbo.CTRL_LOTE C With(Nolock) on 
	Substring(H.Flock_id,CHARINDEX('-',H.Flock_id)+1,len(H.Flock_id)-CHARINDEX('-',H.Flock_id)+1) = C.CtrlLoteNum and
	H.Lay_date = C.CtrlLoteDataValid
where Set_date = @SetDate and Hatch_loc = @Hatch_Loc
	and Flock_id = @Flock_id and Case When Hatch_loc = 'CH' Then 'Todas' Else Setter End = @Machine and ClassOvo = @ClassOvo and Case When Hatch_loc = 'CH' Then 'Todos' Else Hatcher End = @Hatcher
group by
	Hatch_loc,
	Set_date,
	Case When Hatch_loc = 'CH' Then 'Todas' Else Setter End,
	Case When Hatch_loc = 'CH' Then 'Todos' Else Hatcher End,
	Flock_id,
	Variety,
	NumLote,
	ClassOvo" ConflictDetection="CompareAllValues">
                                    <SelectParameters>
                                        <asp:Parameter DefaultValue="1988-01-01" Name="DataRetiradaReal" />
                                        <asp:Parameter DefaultValue="00:00" Name="Hora_01_Retirada" />
                                        <asp:Parameter DefaultValue="0" Name="Qtde_01_Retirada" />
                                        <asp:Parameter DefaultValue="00:00" Name="Hora_02_Retirada" />
                                        <asp:Parameter DefaultValue="0" Name="Qtde_02_Retirada" />
                                        <asp:Parameter DefaultValue="0" Name="Eliminado" />
                                        <asp:Parameter DefaultValue="0" Name="Morto" />
                                        <asp:Parameter DefaultValue="0" Name="Macho" />
                                        <asp:Parameter DefaultValue="0" Name="Pintos_Vendaveis" />
                                        <asp:Parameter DefaultValue="0" Name="Refugo" />
                                        <asp:Parameter DefaultValue="0" Name="Pinto_Terceira" />
                                        <asp:Parameter DefaultValue="0" Name="Inicial0a3" />
                                        <asp:Parameter DefaultValue="0" Name="Inicial4a7" />
                                        <asp:Parameter DefaultValue="0" Name="Media8a14" />
                                        <asp:Parameter DefaultValue="0" Name="Tardia15a18" />
                                        <asp:Parameter DefaultValue="0" Name="Tardia19a21" />
                                        <asp:Parameter DefaultValue="0" Name="BicadoVivo" />
                                        <asp:Parameter DefaultValue="0" Name="BicadoMorto" />
                                        <asp:Parameter DefaultValue="0" Name="ContaminacaoBacteriana" />
                                        <asp:Parameter DefaultValue="0" Name="Fungo" />
                                        <asp:Parameter DefaultValue="0" Name="MalPosicionado" />
                                        <asp:Parameter DefaultValue="0" Name="MaFormacaoCerebro" />
                                        <asp:Parameter DefaultValue="0" Name="MaFormacaoVisceras" />
                                        <asp:Parameter DefaultValue="0" Name="Hemorragico" />
                                        <asp:Parameter DefaultValue="0" Name="Anormalidade" />
                                        <asp:Parameter DefaultValue="0" Name="Infertil" />
                                        <asp:Parameter DefaultValue="0" Name="Amostra" />
                                        <asp:Parameter DefaultValue="0" Name="EliminadoCancelamento" />
                                        <asp:Parameter DefaultValue="0" Name="PerdaUmidade" />
                                        <asp:Parameter DefaultValue="0" Name="ChickYeld" />
                                        <asp:Parameter DefaultValue="0" Name="TempCloaca" />
                                        <asp:Parameter DefaultValue="0" Name="OvoVirado" />
                                        <asp:Parameter DefaultValue="0" Name="QuebradoTrincado" />
                                        <asp:Parameter DefaultValue="obs" Name="SetterEmbrio" />
                                        <asp:Parameter DefaultValue="obs" Name="HatcherEmbrio" />
                                        <asp:Parameter DefaultValue="0" Name="QtdeNascidos" />
                                        <asp:Parameter DefaultValue="0" Name="Peso" />
                                        <asp:Parameter DefaultValue="0" Name="Uniformidade" />
                                        <asp:ControlParameter ControlID="Calendar1" Name="SetDate" 
                                            PropertyName="SelectedDate" />
                                        <asp:ControlParameter ControlID="ddlIncubatorios" Name="Hatch_Loc" 
                                            PropertyName="SelectedValue" />
                                        <asp:Parameter DefaultValue="" Name="FLOCK_ID" />
                                        <asp:Parameter DefaultValue="" Name="Machine" />
                                        <asp:Parameter Name="ClassOvo" DefaultValue="" />
                                        <asp:Parameter Name="Hatcher" />
                                    </SelectParameters>
                                </asp:SqlDataSource>
                                <asp:Label ID="lblMensagem" runat="server" Style="font-weight: 700; color: #FF3300"
                                    Visible="False"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td class="style64" colspan="3">
                                <br />
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <table style="width: 100%;">
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
                                                Font-Underline="False" Text="SETTERS"></asp:Label>
                                            <asp:GridView ID="gvMaquinas" runat="server" AutoGenerateColumns="False" 
                                                CellPadding="4" DataSourceID="MaquinasSqlDataSource" ForeColor="#333333" 
                                                GridLines="None" style="font-size: xx-small" Width="100%">
                                                <AlternatingRowStyle BackColor="White" />
                                                <Columns>
                                                    <asp:BoundField DataField="Incubadora" HeaderText="Incubadora" 
                                                        SortExpression="Incubadora" />
                                                    <asp:BoundField DataField="1ª Ret." HeaderText="1ª Ret." ReadOnly="True" 
                                                        SortExpression="1ª Ret." />
                                                    <asp:BoundField DataField="2ª Ret." HeaderText="2ª Ret." ReadOnly="True" 
                                                        SortExpression="2ª Ret." />
                                                    <asp:BoundField DataField="Eliminados" HeaderText="Eliminados" ReadOnly="True" 
                                                        SortExpression="Eliminados" DataFormatString="{0:N0}" />
                                                    <asp:BoundField DataField="Mortos" HeaderText="Mortos" ReadOnly="True" 
                                                        SortExpression="Mortos" DataFormatString="{0:N0}" />
                                                    <asp:BoundField DataField="Machos" HeaderText="Machos" ReadOnly="True" 
                                                        SortExpression="Machos" DataFormatString="{0:N0}" />
                                                    <asp:BoundField DataField="Vendáveis" HeaderText="Vendáveis" ReadOnly="True" 
                                                        SortExpression="Vendáveis" DataFormatString="{0:N0}" />
                                                    <asp:BoundField DataField="Refugos" HeaderText="Refugos" ReadOnly="True" 
                                                        SortExpression="Refugos" DataFormatString="{0:N0}" />
                                                    <asp:BoundField DataField="3ª" HeaderText="3ª" ReadOnly="True" 
                                                        SortExpression="3ª" DataFormatString="{0:N0}" />
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
                                                ConnectionString="<%$ ConnectionStrings:LayoutDb %>" SelectCommand="select Setter Incubadora, SUM(Qtde_01_Retirada) [1ª Ret.], SUM(Qtde_02_Retirada) [2ª Ret.], SUM(Eliminado) Eliminados, SUM(Morto) Mortos, SUM(Macho) Machos, SUM(Pintos_Vendaveis) [Vendáveis], SUM(Refugo) Refugos, SUM(Pinto_Terceira) [3ª]
from HATCHERY_FLOCK_SETTER_DATA
where Hatch_loc =@Incubatorio and Set_date = @SetDate
group by Setter
order by Setter">
                                                <SelectParameters>
                                                    <asp:ControlParameter ControlID="ddlIncubatorios" Name="Incubatorio" 
                                                        PropertyName="SelectedValue" />
                                                    <asp:ControlParameter ControlID="Calendar1" Name="SetDate" 
                                                        PropertyName="SelectedDate" />
                                                </SelectParameters>
                                            </asp:SqlDataSource>
                                            <br />
                                            <asp:GridView ID="gvMaquinas0" runat="server" AutoGenerateColumns="False" 
                                                CellPadding="4" DataSourceID="MaquinasSqlDataSource0" ForeColor="#333333" 
                                                GridLines="None" style="font-size: xx-small" Width="100%">
                                                <AlternatingRowStyle BackColor="White" />
                                                <Columns>
                                                    <asp:BoundField DataField="Incubadora" HeaderText="Incubadora" 
                                                        SortExpression="Incubadora" />
                                                    <asp:BoundField DataField="Amostra" HeaderText="Amostra" ReadOnly="True" 
                                                        SortExpression="Amostra" DataFormatString="{0:N0}" />
                                                    <asp:BoundField DataField="Infértil" HeaderText="Infértil" ReadOnly="True" 
                                                        SortExpression="Infértil" DataFormatString="{0:N0}" />
                                                    <asp:BoundField DataField="Inicial (0-3)" HeaderText="Inicial (1-3)" ReadOnly="True" 
                                                        SortExpression="Inicial (0-3)" DataFormatString="{0:N0}" />
                                                    <asp:BoundField DataField="Inicial (4-7)" HeaderText="Inicial (4-7)" ReadOnly="True" 
                                                        SortExpression="Inicial (4-7)" DataFormatString="{0:N0}" />
                                                    <asp:BoundField DataField="Média (8-14)" HeaderText="Média (8-14)" 
                                                        ReadOnly="True" SortExpression="Média (8-14)" DataFormatString="{0:N0}" />
                                                    <asp:BoundField DataField="Tardia (15-18)" HeaderText="Tardia (15-18)" 
                                                        ReadOnly="True" SortExpression="Tardia (15-18)" DataFormatString="{0:N0}" />
                                                    <asp:BoundField DataField="Tardia (19-21)" 
                                                        HeaderText="Tardia (19-21)" ReadOnly="True" 
                                                        SortExpression="Tardia (19-21)" DataFormatString="{0:N0}" />
                                                    <asp:BoundField DataField="Hemorrágico" HeaderText="Hemorrágico" ReadOnly="True" 
                                                        SortExpression="Hemorrágico" DataFormatString="{0:N0}" />
                                                    <asp:BoundField DataField="Bicado Vivo" HeaderText="Bicado Vivo" 
                                                        ReadOnly="True" SortExpression="Bicado Vivo" DataFormatString="{0:N0}" />
                                                    <asp:BoundField DataField="Bicado Morto" HeaderText="Bicado Morto" 
                                                        ReadOnly="True" SortExpression="Bicado Morto" DataFormatString="{0:N0}" />
                                                    <asp:BoundField DataField="Contaminação Bacteriana" 
                                                        HeaderText="Contaminado" ReadOnly="True" 
                                                        SortExpression="Contaminação Bacteriana" DataFormatString="{0:N0}" />
                                                    <asp:BoundField DataField="Fungo" 
                                                        HeaderText="Fungo" ReadOnly="True" 
                                                        SortExpression="Fungo" />
                                                    <asp:BoundField DataField="Má Formação (Cérebro exposto)" 
                                                        HeaderText="Má Formação (Cérebro exposto)" ReadOnly="True" 
                                                        SortExpression="Má Formação (Cérebro exposto)" />
                                                    <asp:BoundField DataField="Má Formação (Vísceras expostas)" HeaderText="Má Formação (Vísceras expostas)" 
                                                        ReadOnly="True" SortExpression="Má Formação (Vísceras expostas)" />
                                                    <asp:BoundField DataField="Mal Posicionado" HeaderText="Má Posição" 
                                                        ReadOnly="True" SortExpression="Mal Posicionado" />
                                                    <asp:BoundField DataField="Anormalidade" HeaderText="Anormalidade" 
                                                        ReadOnly="True" SortExpression="Anormalidade" />
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
                                            <asp:SqlDataSource ID="MaquinasSqlDataSource0" runat="server" 
                                                ConnectionString="<%$ ConnectionStrings:LayoutDb %>" SelectCommand="select Setter Incubadora, SUM(Amostra) Amostra, SUM(Infertil) [Infértil],
	SUM(Inicial0a3) [Inicial (0-3)],
	SUM(Inicial4a7) [Inicial (4-7)],
	SUM(Media8a14) [Média (8-14)],
	SUM(Tardia15a18) [Tardia (15-18)],
	SUM(Tardia19a21) [Tardia (19-21)],
	SUM(Hemorragico) [Hemorrágico],
	SUM(BicadoVivo) [Bicado Vivo],
	SUM(BicadoMorto) [Bicado Morto],
	SUM(ContaminacaoBacteriana) [Contaminação Bacteriana],
	SUM(Fungo) [Fungo],
	SUM(MaFormacaoCerebro) [Má Formação (Cérebro exposto)],
	SUM(MaFormacaoVisceras) [Má Formação (Vísceras expostas)],
	SUM(MalPosicionado) [Mal Posicionado],
	SUM(Anormalidade) [Anormalidade]
from HATCHERY_FLOCK_SETTER_DATA
where Hatch_loc =@Incubatorio and Set_date = @SetDate
group by Setter
order by Setter">
                                                <SelectParameters>
                                                    <asp:ControlParameter ControlID="ddlIncubatorios" Name="Incubatorio" 
                                                        PropertyName="SelectedValue" />
                                                    <asp:ControlParameter ControlID="Calendar1" Name="SetDate" 
                                                        PropertyName="SelectedDate" />
                                                </SelectParameters>
                                            </asp:SqlDataSource>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="text-align: center">
                                        <asp:Label ID="Label12" runat="server" Font-Bold="True" Font-Size="Small" 
                                                Font-Underline="False" Text="LOTES"></asp:Label>
                                            <asp:GridView ID="gvLotes" runat="server" AutoGenerateColumns="False" 
                                                CellPadding="4" DataSourceID="LotesSqlDataSource" ForeColor="#333333" 
                                                GridLines="None" style="font-size: xx-small" Width="100%">
                                                <AlternatingRowStyle BackColor="White" />
                                                <Columns>
                                                    <asp:BoundField DataField="Lote" HeaderText="Lote" 
                                                        SortExpression="Lote" />
                                                    <asp:BoundField DataField="1ª Ret." HeaderText="1ª Ret." ReadOnly="True" 
                                                        SortExpression="1ª Ret." DataFormatString="{0:N0}" />
                                                    <asp:BoundField DataField="2ª Ret." HeaderText="2ª Ret." ReadOnly="True" 
                                                        SortExpression="2ª Ret." DataFormatString="{0:N0}" />
                                                    <asp:BoundField DataField="Eliminados" HeaderText="Eliminados" ReadOnly="True" 
                                                        SortExpression="Eliminados" DataFormatString="{0:N0}" />
                                                    <asp:BoundField DataField="Mortos" HeaderText="Mortos" ReadOnly="True" 
                                                        SortExpression="Mortos" DataFormatString="{0:N0}" />
                                                    <asp:BoundField DataField="Machos" HeaderText="Machos" ReadOnly="True" 
                                                        SortExpression="Machos" DataFormatString="{0:N0}" />
                                                    <asp:BoundField DataField="Vendáveis" HeaderText="Vendáveis" ReadOnly="True" 
                                                        SortExpression="Vendáveis" DataFormatString="{0:N0}" />
                                                    <asp:BoundField DataField="Refugos" HeaderText="Refugos" ReadOnly="True" 
                                                        SortExpression="Refugos" DataFormatString="{0:N0}" />
                                                    <asp:BoundField DataField="3ª" HeaderText="3ª" ReadOnly="True" 
                                                        SortExpression="3ª" DataFormatString="{0:N0}" />
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
                                                ConnectionString="<%$ ConnectionStrings:LayoutDb %>" SelectCommand="select NumLote Lote, SUM(Qtde_01_Retirada) [1ª Ret.], SUM(Qtde_02_Retirada) [2ª Ret.], SUM(Eliminado) Eliminados, SUM(Morto) Mortos, SUM(Macho) Machos, SUM(Pintos_Vendaveis) [Vendáveis], SUM(Refugo) Refugos, SUM(Pinto_Terceira) [3ª]
from HATCHERY_FLOCK_SETTER_DATA
where Hatch_loc =@Incubatorio and Set_date = @SetDate
group by NumLote
order by NumLote">
                                                <SelectParameters>
                                                    <asp:ControlParameter ControlID="ddlIncubatorios" Name="Incubatorio" 
                                                        PropertyName="SelectedValue" />
                                                    <asp:ControlParameter ControlID="Calendar1" Name="SetDate" 
                                                        PropertyName="SelectedDate" />
                                                </SelectParameters>
                                            </asp:SqlDataSource>
                                            <br />
                                            <asp:GridView ID="gvLotes0" runat="server" AutoGenerateColumns="False" 
                                                CellPadding="4" DataSourceID="LotesSqlDataSource0" ForeColor="#333333" 
                                                GridLines="None" style="font-size: xx-small" Width="100%">
                                                <AlternatingRowStyle BackColor="White" />
                                                <Columns>
                                                    <asp:BoundField DataField="Lote" HeaderText="Lote" SortExpression="Lote" />
                                                    <asp:BoundField DataField="Amostra" HeaderText="Amostra" ReadOnly="True" 
                                                        SortExpression="Amostra" DataFormatString="{0:N0}" />
                                                    <asp:BoundField DataField="Infértil" HeaderText="Infértil" ReadOnly="True" 
                                                        SortExpression="Infértil" DataFormatString="{0:N0}" />
                                                    <asp:BoundField DataField="Inicial (0-3)" HeaderText="Inicial (1-3)" ReadOnly="True" 
                                                        SortExpression="Inicial (0-3)" DataFormatString="{0:N0}" />
                                                    <asp:BoundField DataField="Inicial (4-7)" HeaderText="Inicial (4-7)" ReadOnly="True" 
                                                        SortExpression="Inicial (4-7)" DataFormatString="{0:N0}" />
                                                    <asp:BoundField DataField="Média (8-14)" HeaderText="Média (8-14)" 
                                                        ReadOnly="True" SortExpression="Média (8-14)" DataFormatString="{0:N0}" />
                                                    <asp:BoundField DataField="Tardia (15-18)" HeaderText="Tardia (15-18)" 
                                                        ReadOnly="True" SortExpression="Tardia (15-18)" DataFormatString="{0:N0}" />
                                                    <asp:BoundField DataField="Tardia (19-21)" 
                                                        HeaderText="Tardia (19-21)" ReadOnly="True" 
                                                        SortExpression="Tardia (19-21)" DataFormatString="{0:N0}" />
                                                    <asp:BoundField DataField="Hemorrágico" HeaderText="Hemorrágico" ReadOnly="True" 
                                                        SortExpression="Hemorrágico" DataFormatString="{0:N0}" />
                                                    <asp:BoundField DataField="Bicado Vivo" HeaderText="Bicado Vivo" 
                                                        ReadOnly="True" SortExpression="Bicado Vivo" DataFormatString="{0:N0}" />
                                                    <asp:BoundField DataField="Bicado Morto" HeaderText="Bicado Morto" 
                                                        ReadOnly="True" SortExpression="Bicado Morto" DataFormatString="{0:N0}" />
                                                    <asp:BoundField DataField="Contaminação Bacteriana" 
                                                        HeaderText="Contaminado" ReadOnly="True" 
                                                        SortExpression="Contaminação Bacteriana" DataFormatString="{0:N0}" />
                                                    <asp:BoundField DataField="Fungo" 
                                                        HeaderText="Fungo" ReadOnly="True" 
                                                        SortExpression="Fungo" DataFormatString="{0:N0}" />
                                                    <asp:BoundField DataField="Má Formação (Cérebro exposto)" 
                                                        HeaderText="Má Formação (Cérebro exposto)" ReadOnly="True" 
                                                        SortExpression="Má Formação (Cérebro exposto)" DataFormatString="{0:N0}" />
                                                    <asp:BoundField DataField="Má Formação (Vísceras expostas)" HeaderText="Má Formação (Vísceras expostas)" 
                                                        ReadOnly="True" SortExpression="Má Formação (Vísceras expostas)" DataFormatString="{0:N0}" />
                                                    <asp:BoundField DataField="Mal Posicionado" HeaderText="Má Posição" 
                                                        ReadOnly="True" SortExpression="Mal Posicionado" DataFormatString="{0:N0}" />
                                                    <asp:BoundField DataField="Anormalidade" HeaderText="Anormalidade" 
                                                        ReadOnly="True" SortExpression="Anormalidade" DataFormatString="{0:N0}" />
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
                                            <asp:SqlDataSource ID="LotesSqlDataSource0" runat="server" 
                                                ConnectionString="<%$ ConnectionStrings:LayoutDb %>" SelectCommand="select NumLote Lote, SUM(Amostra) Amostra, SUM(Infertil) [Infértil],
	SUM(Inicial0a3) [Inicial (0-3)],
	SUM(Inicial4a7) [Inicial (4-7)],
	SUM(Media8a14) [Média (8-14)],
	SUM(Tardia15a18) [Tardia (15-18)],
	SUM(Tardia19a21) [Tardia (19-21)],
	SUM(Hemorragico) [Hemorrágico],
	SUM(BicadoVivo) [Bicado Vivo],
	SUM(BicadoMorto) [Bicado Morto],
	SUM(ContaminacaoBacteriana) [Contaminação Bacteriana],
	SUM(Fungo) [Fungo],
	SUM(MaFormacaoCerebro) [Má Formação (Cérebro exposto)],
	SUM(MaFormacaoVisceras) [Má Formação (Vísceras expostas)],
	SUM(MalPosicionado) [Mal Posicionado],
	SUM(Anormalidade) [Anormalidade]
from HATCHERY_FLOCK_SETTER_DATA
where Hatch_loc =@Incubatorio and Set_date = @SetDate
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
                                    </tr>
                                    <tr>
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
                                                    <asp:BoundField DataField="1ª Ret." HeaderText="1ª Ret." ReadOnly="True" 
                                                        SortExpression="1ª Ret." DataFormatString="{0:N0}" />
                                                    <asp:BoundField DataField="2ª Ret." HeaderText="2ª Ret." ReadOnly="True" 
                                                        SortExpression="2ª Ret." DataFormatString="{0:N0}" />
                                                    <asp:BoundField DataField="Eliminados" HeaderText="Eliminados" ReadOnly="True" 
                                                        SortExpression="Eliminados" DataFormatString="{0:N0}" />
                                                    <asp:BoundField DataField="Mortos" HeaderText="Mortos" ReadOnly="True" 
                                                        SortExpression="Mortos" DataFormatString="{0:N0}" />
                                                    <asp:BoundField DataField="Machos" HeaderText="Machos" ReadOnly="True" 
                                                        SortExpression="Machos" DataFormatString="{0:N0}" />
                                                    <asp:BoundField DataField="Vendáveis" HeaderText="Vendáveis" ReadOnly="True" 
                                                        SortExpression="Vendáveis" DataFormatString="{0:N0}" />
                                                    <asp:BoundField DataField="Refugos" HeaderText="Refugos" ReadOnly="True" 
                                                        SortExpression="Refugos" DataFormatString="{0:N0}" />
                                                    <asp:BoundField DataField="3ª" HeaderText="3ª" ReadOnly="True" 
                                                        SortExpression="3ª" DataFormatString="{0:N0}" />
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
                                                ConnectionString="<%$ ConnectionStrings:LayoutDb %>" SelectCommand="select Variety Linhagem, SUM(Qtde_01_Retirada) [1ª Ret.], SUM(Qtde_02_Retirada) [2ª Ret.], SUM(Eliminado) Eliminados, SUM(Morto) Mortos, SUM(Macho) Machos, SUM(Pintos_Vendaveis) [Vendáveis], SUM(Refugo) Refugos, SUM(Pinto_Terceira) [3ª]
from HATCHERY_FLOCK_SETTER_DATA
where Hatch_loc =@Incubatorio and Set_date = @SetDate
group by Variety
order by Variety">
                                                <SelectParameters>
                                                    <asp:ControlParameter ControlID="ddlIncubatorios" Name="Incubatorio" 
                                                        PropertyName="SelectedValue" />
                                                    <asp:ControlParameter ControlID="Calendar1" Name="SetDate" 
                                                        PropertyName="SelectedDate" />
                                                </SelectParameters>
                                            </asp:SqlDataSource>
                                            <br />
                                            <asp:GridView ID="gvLinhagens0" runat="server" AutoGenerateColumns="False" 
                                                CellPadding="4" DataSourceID="LinhagensSqlDataSource0" ForeColor="#333333" 
                                                GridLines="None" style="font-size: xx-small" Width="100%">
                                                <AlternatingRowStyle BackColor="White" />
                                                <Columns>
                                                    <asp:BoundField DataField="Linhagem" HeaderText="Linhagem" 
                                                        SortExpression="Linhagem" />
                                                    <asp:BoundField DataField="Amostra" HeaderText="Amostra" ReadOnly="True" 
                                                        SortExpression="Amostra" DataFormatString="{0:N0}" />
                                                    <asp:BoundField DataField="Infértil" HeaderText="Infértil" ReadOnly="True" 
                                                        SortExpression="Infértil" DataFormatString="{0:N0}" />
                                                    <asp:BoundField DataField="Inicial (0-3)" HeaderText="Inicial (1-3)" ReadOnly="True" 
                                                        SortExpression="Inicial (0-3)" DataFormatString="{0:N0}" />
                                                    <asp:BoundField DataField="Inicial (4-7)" HeaderText="Inicial (4-7)" ReadOnly="True" 
                                                        SortExpression="Inicial (4-7)" DataFormatString="{0:N0}" />
                                                    <asp:BoundField DataField="Média (8-14)" HeaderText="Média (8-14)" 
                                                        ReadOnly="True" SortExpression="Média (8-14)" DataFormatString="{0:N0}" />
                                                    <asp:BoundField DataField="Tardia (15-18)" HeaderText="Tardia (15-18)" 
                                                        ReadOnly="True" SortExpression="Tardia (15-18)" DataFormatString="{0:N0}" />
                                                    <asp:BoundField DataField="Tardia (19-21)" 
                                                        HeaderText="Tardia (19-21)" ReadOnly="True" 
                                                        SortExpression="Tardia (19-21)" DataFormatString="{0:N0}" />
                                                    <asp:BoundField DataField="Hemorrágico" HeaderText="Hemorrágico" ReadOnly="True" 
                                                        SortExpression="Hemorrágico" DataFormatString="{0:N0}" />
                                                    <asp:BoundField DataField="Bicado Vivo" HeaderText="Bicado Vivo" 
                                                        ReadOnly="True" SortExpression="Bicado Vivo" DataFormatString="{0:N0}" />
                                                    <asp:BoundField DataField="Bicado Morto" HeaderText="Bicado Morto" 
                                                        ReadOnly="True" SortExpression="Bicado Morto" DataFormatString="{0:N0}" />
                                                    <asp:BoundField DataField="Contaminação Bacteriana" 
                                                        HeaderText="Contaminado" ReadOnly="True" 
                                                        SortExpression="Contaminação Bacteriana" DataFormatString="{0:N0}" />
                                                    <asp:BoundField DataField="Fungo" 
                                                        HeaderText="Fungo" ReadOnly="True" 
                                                        SortExpression="Fungo" DataFormatString="{0:N0}" />
                                                    <asp:BoundField DataField="Má Formação (Cérebro exposto)" 
                                                        HeaderText="Má Formação (Cérebro exposto)" ReadOnly="True" 
                                                        SortExpression="Má Formação (Cérebro exposto)" DataFormatString="{0:N0}" />
                                                    <asp:BoundField DataField="Má Formação (Vísceras expostas)" HeaderText="Má Formação (Vísceras expostas)" 
                                                        ReadOnly="True" SortExpression="Má Formação (Vísceras expostas)" DataFormatString="{0:N0}" />
                                                    <asp:BoundField DataField="Mal Posicionado" HeaderText="Má Posição" 
                                                        ReadOnly="True" SortExpression="Mal Posicionado" DataFormatString="{0:N0}" />
                                                    <asp:BoundField DataField="Anormalidade" HeaderText="Anormalidade" 
                                                        ReadOnly="True" SortExpression="Anormalidade" DataFormatString="{0:N0}" />
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
                                            <asp:SqlDataSource ID="LinhagensSqlDataSource0" runat="server" 
                                                ConnectionString="<%$ ConnectionStrings:LayoutDb %>" SelectCommand="select Variety Linhagem, SUM(Amostra) Amostra,SUM(Infertil) [Infértil],
	SUM(Inicial0a3) [Inicial (0-3)],
	SUM(Inicial4a7) [Inicial (4-7)],
	SUM(Media8a14) [Média (8-14)],
	SUM(Tardia15a18) [Tardia (15-18)],
	SUM(Tardia19a21) [Tardia (19-21)],
	SUM(Hemorragico) [Hemorrágico],
	SUM(BicadoVivo) [Bicado Vivo],
	SUM(BicadoMorto) [Bicado Morto],
	SUM(ContaminacaoBacteriana) [Contaminação Bacteriana],
	SUM(Fungo) [Fungo],
	SUM(MaFormacaoCerebro) [Má Formação (Cérebro exposto)],
	SUM(MaFormacaoVisceras) [Má Formação (Vísceras expostas)],
	SUM(MalPosicionado) [Mal Posicionado],
	SUM(Anormalidade) [Anormalidade]
from HATCHERY_FLOCK_SETTER_DATA
where Hatch_loc =@Incubatorio and Set_date = @SetDate
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
