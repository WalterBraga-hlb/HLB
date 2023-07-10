<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Rastreabilidade.aspx.cs" Inherits="MvcAppHyLinedoBrasil.WebForms.Rastreabilidade"
    UICulture="pt" Culture="pt-BR" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Rastreabilidade dos Lotes</title>
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
            font-size: xx-small;
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
        function mnum(v) {
            v = v.replace(/\D/g, "");                                      //Remove tudo o que não é dígito
            return v;
        }
        function setDisableMask(o, v) {
            v_obj = o
            v_val = v
            setTimeout("disableValidation()", 1)
        }
        function disableValidation() {
            if (v_obj.value == v_val) {
                v_obj.value = '';
            }
        }
        function mtempo(v) {
            v = v.replace(/\D/g, "");                    //Remove tudo o que não é dígito
            v = v.replace(/(\d{1})(\d{2})(\d{2})/, "$1:$2.$3");
            return v;
        }
        function mhora(v) {
            v = v.replace(/\D/g, "");                    //Remove tudo o que não é dígito
            v = v.replace(/(\d{2})(\d)/, "$1:$2");
            return v;
        }
    </script>
</head>
<body style="background-color: #5c87b2; font-family: Verdana, Tahoma, Arial, Helvetica Neue, Helvetica, Sans-Serif;">
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" AsyncPostBackTimeout="0"
        EnableScriptLocalization="true" EnableScriptGlobalization="true">
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
                                    Text="RASTREABILIDADE DE LOTES"></asp:Label>
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
                                    onselectedindexchanged="ddlIncubatorios_SelectedIndexChanged" Height="16px">
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
                                <asp:Label ID="lblTituloPedidos" runat="server" Font-Bold="True" 
                                    Font-Size="Small" Font-Underline="False"
                                    Text="LISTA DE CLIENTES X PEDIDOS CHIC" CssClass="style66"></asp:Label>
                                <table align="center">
                                    <tr>
                                        <td class="style57">
                                            <asp:Label ID="lblLocalizarPedido" runat="server" CssClass="style61" Font-Size="XX-Small" 
                                                Text="Localizar:" style="font-family: Verdana; font-weight: 700"></asp:Label>
                                        </td>
                                        <td class="style22">
                                            &nbsp;
                                            <asp:TextBox ID="txtLocalizarPedido" runat="server" CssClass="style24" Width="193px" 
                                                Height="15px" style="font-family: Verdana"></asp:TextBox>
                                        </td>
                                        <td class="style62">
                                            <asp:DropDownList ID="ddlCampoPedido" runat="server" Height="21px" Style="font-weight: 700; font-family: Verdana; font-size: x-small;"
                                                Width="124px" CssClass="style60">
                                                <asp:ListItem>Nome</asp:ListItem>
                                                <asp:ListItem Value="UF">UF</asp:ListItem>
                                            </asp:DropDownList>
                                            &nbsp;
                                        </td>
                                        <td class="style39">
                                            <asp:Button ID="btnPesquisarPedido" runat="server" Font-Bold="True" Font-Size="XX-Small"
                                                Height="21px" OnClick="Button1_Click" Text="Pesquisar" Width="118px" 
                                                CssClass="style60" style="font-family: Verdana" />
                                            &nbsp;
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td class="style64">
                                <asp:GridView ID="gdvPedidos" runat="server" AllowSorting="True" 
                                    CellPadding="4" DataSourceID="PedidosAniPlan"
                                    ForeColor="#333333" GridLines="None" Style="font-size: xx-small; text-align: center; font-family: Tahoma;"
                                    Width="990px" AutoGenerateColumns="False" PageSize="5" 
                                    OnSelectedIndexChanged="GridView3_SelectedIndexChanged" 
                                    EmptyDataText="Não existe Pedidos!" 
                                    onrowdatabound="gdvPedidos_RowDataBound">
                                    <AlternatingRowStyle BackColor="White" />
                                    <Columns>
                                        <asp:CommandField ButtonType="Image" 
                                            SelectImageUrl="~/Content/images/Next_16x16.gif" 
                                            ShowSelectButton="True" />
                                        <asp:BoundField DataField="Empresa" HeaderText="Empresa" 
                                            SortExpression="Empresa" />
                                        <asp:BoundField DataField="Cód.Cliente" HeaderText="Cód.Cliente" 
                                            SortExpression="Cód.Cliente">
                                        </asp:BoundField>
                                        <asp:BoundField DataField="Nome do Cliente" HeaderText="Nome do Cliente" 
                                            SortExpression="Nome do Cliente" />
                                        <asp:BoundField DataField="UF" HeaderText="UF" SortExpression="UF" />
                                        <asp:BoundField DataField="Pedido" HeaderText="Pedido" 
                                            SortExpression="Pedido" />
                                        <asp:BoundField DataField="Linhagem" HeaderText="Linhagem" 
                                            SortExpression="Linhagem" />
                                        <asp:BoundField DataField="Qtde." HeaderText="Qtde." SortExpression="Qtde." DataFormatString="{0:N0}" />
                                        <asp:BoundField DataField="Carga" HeaderText="Carga" SortExpression="Carga" />
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
                                    ProviderName="<%$ ConnectionStrings:Oracle.ProviderName %>" SelectCommand="select 
  s.inv_comp &quot;Empresa&quot;,
  co.cust_no &quot;Cód.Cliente&quot;,
  c.name &quot;Nome do Cliente&quot;,
  c.state &quot;UF&quot;,
  co.orderno &quot;Pedido&quot;,
  v.flip &quot;Linhagem&quot;,
  SUM(b.quantity) &quot;Qtde.&quot;,
  (select max(b1.alt_desc) from ch_booked b1
       where co.site_key = b1.site_key and co.orderno = b1.orderno
       and b1.cal_date = :Data and b1.item = '362'
       and b1.location = :Geracao) &quot;Carga&quot;
from 
  ch_cust c,
  ch_orders co,
  ch_salesman s,
  ch_booked b,
  ch_items i,
  ch_vartabl v
where
  s.sl_code (+)= co.salesrep and 
  s.site_key (+)= co.site_key and
  co.site_key = c.site_key and
  co.cust_no = c.custno and
  co.orderno = b.orderno and
  b.item = i.item_no and
  b.site_key = i.site_key and
  v.site_key = i.site_key and
  v.variety = i.variety and
  i.form in ('HE','DV','DN','PM','PF') and
  (InStr(:Empresa,s.inv_comp) &gt; 0 or s.inv_comp is null) and
  b.cal_date =  :Data and
  b.location = Replace(:Geracao,'TB','AJ') and
  b.order_type = 'O' and c.custno not in ('0000178')
  and (
         (:Pesquisa ='0')
         or 
         (:Campo = 'Nome' and c.name like '%' ||UPPER(:Pesquisa)||'%')
         or
         (:Campo = 'UF' and c.state like '%'||UPPER(:Pesquisa)||'%')
        )
group by
  co.cust_no,
  c.name,
  c.state,
  co.orderno,
  s.inv_comp,
  co.site_key,
  v.flip
Order by
  c.name">
                                    <SelectParameters>
                                        <asp:ControlParameter ControlID="Calendar1" DbType="Date" DefaultValue="" Name="Data"
                                            PropertyName="SelectedDate" />
                                        <asp:ControlParameter ControlID="ddlIncubatorios" Name="Geracao" 
                                            PropertyName="SelectedValue" />
                                        <asp:SessionParameter Name="Empresa" SessionField="empresa" />
                                        <asp:ControlParameter ControlID="txtLocalizarPedido" Name="Pesquisa" 
                                            PropertyName="Text" />
                                        <asp:ControlParameter ControlID="ddlCampoPedido" Name="Campo" 
                                            PropertyName="SelectedValue" />
                                    </SelectParameters>
                                </asp:SqlDataSource>
                                <asp:SqlDataSource ID="PedidosAniPlan" runat="server" ConnectionString="<%$ ConnectionStrings:HLBAPPConnectionString %>" SelectCommand="select
	P.Empresa,
	P.CodigoCliente [Cód.Cliente],
	P.NomeCliente [Nome do Cliente],
	P.uf UF,
	P.CHICNum Pedido,
	P.LinhagemFLIP &quot;Linhagem&quot;,
	SUM(P.QtdeVendida + P.QtdeBonificada) [Qtde.],
	IIF(P.NumVeiculo = 0, '', 'Carga ' + convert(varchar,P.NumVeiculo)) [Carga]
from
	VU_Pedidos_Vendas_CHIC_Matrizes P
where
	CHARINDEX(P.Empresa,@Empresa) &gt; 0 and
                   P.QtdeVendida &gt; 0 and
	P.DataIncubacao = @Data and
	P.LocalNascimento = @Geracao and
	(
		(@Pesquisa = '0')
		or
		(@Campo = 'Nome' and P.NomeCliente like '%' + UPPER(@Pesquisa) + '%')
		or
		(@Campo = 'UF' and P.uf like '%' + UPPER(@Pesquisa) + '%')
	)
group by
	P.Empresa,
	P.CodigoCliente,
	P.NomeCliente,
	P.uf,
	P.CHICNum,
	P.LinhagemFLIP,
	P.NumVeiculo

Union

select
	P.Empresa,
	P.CodigoCliente [Cód.Cliente],
	P.NomeCliente [Nome do Cliente],
	P.uf UF,
	P.CHICNumReposicao Pedido,
	P.LinhagemFLIP &quot;Linhagem&quot;,
	SUM(P.QtdeReposicao) [Qtde.],
	IIF(P.NumVeiculo = 0, '', 'Carga ' + convert(varchar,P.NumVeiculo)) [Carga]
from
	VU_Pedidos_Vendas_CHIC_Matrizes P
where
	CHARINDEX(P.Empresa,@Empresa) &gt; 0 and
                   P.QtdeReposicao &gt; 0 and
	P.DataIncubacao = @Data and
	P.LocalNascimento = @Geracao and
	(
		(@Pesquisa = '0')
		or
		(@Campo = 'Nome' and P.NomeCliente like '%' + UPPER(@Pesquisa) + '%')
		or
		(@Campo = 'UF' and P.uf like '%' + UPPER(@Pesquisa) + '%')
	)
group by
	P.Empresa,
	P.CodigoCliente,
	P.NomeCliente,
	P.uf,
	P.CHICNumReposicao,
	P.LinhagemFLIP,
	P.NumVeiculo">
                                    <SelectParameters>
                                        <asp:SessionParameter Name="Empresa" SessionField="empresa" />
                                        <asp:ControlParameter ControlID="Calendar1" DbType="Date" DefaultValue="" Name="Data" PropertyName="SelectedDate" />
                                        <asp:ControlParameter ControlID="ddlIncubatorios" Name="Geracao" PropertyName="SelectedValue" />
                                        <asp:ControlParameter ControlID="txtLocalizarPedido" Name="Pesquisa" PropertyName="Text" />
                                        <asp:ControlParameter ControlID="ddlCampoPedido" Name="Campo" PropertyName="SelectedValue" />
                                    </SelectParameters>
                                </asp:SqlDataSource>
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
                <asp:Panel ID="Panel3" runat="server" Width="1002px" HorizontalAlign="Center" Style="text-align: left">
                    <table style="width: 100%;">
                        <tr>
                            <td class="style64">
                                <asp:Label ID="lblPedidoSelecionado" runat="server" 
                                    style="font-weight: 700; color: #0000FF"></asp:Label>
                                <br />
                                <asp:Button ID="btnCancelar" runat="server" Text="Cancelar" 
                                    style="font-size: xx-small; font-weight: 700" 
                                    onclick="btnCancelar_Click" />
                                <asp:Button ID="btnSalvar" runat="server" onclick="btnSalvar_Click" 
                                    style="font-size: xx-small; font-weight: 700" Text="Salvar" />
                                <br />
                                <br />
                                <table style="width:100%;">
                                    <tr>
                                        <td>
                                            <asp:Label ID="lblPesoMedio" runat="server" CssClass="style61" 
                                                Text="Peso Médio:" Visible="False"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtPesoMedio" runat="server"
                                                onkeyup = "mascara(this, mvalor);" Visible="False"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:Label ID="lblUniformidadePerc" runat="server" CssClass="style61" 
                                                Text="% Uniformidade:" Visible="False"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtPercUniformidade" runat="server" 
                                                onkeyup="mascara(this, mvalor);" Visible="False"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Label ID="lblSaidaProgramada" runat="server" CssClass="style61" 
                                                Text="Saída Programada:"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtDataSaidaProgramada" runat="server" Width="64px" 
                                                onblur="setDisableMask(this, '  /  /    ');"></asp:TextBox>
                                            <asp:MaskedEditExtender ID="txtDataSaidaProgramada_MaskedEditExtender" 
                                                runat="server" Century="2000"
                                                CultureName="pt-BR"
                                                CultureThousandsPlaceholder="" CultureTimePlaceholder="" Enabled="True" 
                                                Mask="99/99/9999" MaskType="Date" PromptCharacter=" "
                                                UserDateFormat="DayMonthYear"
                                                InputDirection="LeftToRight"
                                                TargetControlID="txtDataSaidaProgramada" ClearMaskOnLostFocus="true">
                                            </asp:MaskedEditExtender>
                                            <asp:MaskedEditValidator ID="txtDataSaidaProgramada_MaskedEditValidator" runat="server" 
                                                InvalidValueMessage="Data Inválida!" 
                                                ControlExtender="txtDataSaidaProgramada_MaskedEditExtender" 
                                                ControlToValidate="txtDataSaidaProgramada"
                                                PromptCharacter=" " 
                                                style="font-size: xx-small; font-weight: 700; color: #FF0000"></asp:MaskedEditValidator>
                                            <asp:CalendarExtender ID="txtDataSaidaProgramada_CalendarExtender" runat="server" 
                                                Enabled="True" Format="dd/MM/yyyy" 
                                                TargetControlID="txtDataSaidaProgramada" >
                                            </asp:CalendarExtender>
                                            <asp:TextBox ID="txtHoraSaidaProgramada" runat="server" Width="35px"
                                                onkeyup="mascara(this, mhora);"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:Label ID="lblSaidaReal" runat="server" CssClass="style61" 
                                                Text="Saída Real:"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtDataSaidaReal" runat="server" Width="64px" 
                                                onblur="setDisableMask(this, '  /  /    ');"></asp:TextBox>
                                            <asp:MaskedEditExtender ID="txtDataSaidaReal_MaskedEditExtender" 
                                                runat="server" Century="2000"
                                                CultureName="pt-BR"
                                                CultureThousandsPlaceholder="" CultureTimePlaceholder="" Enabled="True" 
                                                Mask="99/99/9999" MaskType="Date" PromptCharacter=" "
                                                UserDateFormat="DayMonthYear"
                                                InputDirection="LeftToRight"
                                                TargetControlID="txtDataSaidaReal" ClearMaskOnLostFocus="true">
                                            </asp:MaskedEditExtender>
                                            <asp:MaskedEditValidator ID="txtDataSaidaReal_MaskedEditValidator" runat="server" 
                                                InvalidValueMessage="Data Inválida!" 
                                                ControlExtender="txtDataSaidaReal_MaskedEditExtender" 
                                                ControlToValidate="txtDataSaidaReal"
                                                PromptCharacter=" " 
                                                style="font-size: xx-small; font-weight: 700; color: #FF0000"></asp:MaskedEditValidator>
                                            <asp:CalendarExtender ID="txtDataSaidaReal_CalendarExtender" runat="server" 
                                                Enabled="True" Format="dd/MM/yyyy" 
                                                TargetControlID="txtDataSaidaReal" >
                                            </asp:CalendarExtender>
                                            <asp:TextBox ID="txtHoraSaidaReal" runat="server" Width="35px"
                                                onkeyup="mascara(this, mhora);"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Label ID="lblQtdeAmostra" runat="server" CssClass="style61" 
                                                Text="Qtde. Amostra:"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtQtdeAmostra" runat="server"
                                                onkeyup = "mascara(this, mnum);"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:Label ID="lblQtdeVacinada" runat="server" CssClass="style61" 
                                                Text="Qtde. Vacinada:"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtQtdeVacinada" runat="server"
                                                onkeyup = "mascara(this, mnum);"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Label ID="lblQtdePontoFioPretoUmbigo" runat="server" CssClass="style61" 
                                                Text="Qtde. Ponto e Fio Preto Umbigo:"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtQtdePontoFioPretoUmbigo" runat="server"
                                                onkeyup = "mascara(this, mnum);"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:Label ID="lblQtdePesTortosDedosCurvos" runat="server" CssClass="style61" 
                                                Text="Qtde. Pés Tortos / Dedos Curvos:"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtQtdePesTortosDedosCurvos" runat="server"
                                                onkeyup = "mascara(this, mnum);"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Label ID="lblQtdeErroContagem" runat="server" CssClass="style61" 
                                                Text="Qtde. Erro Contagem:"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtQtdeErroContagem" runat="server"
                                                onkeyup = "mascara(this, mnum);"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:Label ID="lblQtdeErroSexagem" runat="server" CssClass="style61" 
                                                Text="Qtde. Erro Sexagem:"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtQtdeErroSexagem" runat="server"
                                                onkeyup = "mascara(this, mnum);"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Label ID="lblQtdeErroSelecao" runat="server" CssClass="style61" 
                                                Text="Qtde. Erro Seleção:"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtQtdeErroSelecao" runat="server"
                                                onkeyup = "mascara(this, mnum);"></asp:TextBox>
                                        </td>
                                        <td>
                                        </td>
                                        <td>
                                        </td>
                                    </tr>
                                </table>
                                <br />
                                <asp:Panel ID="pnlRotulos" runat="server">
                                <asp:Label ID="lblRotulos" runat="server" Font-Bold="True" 
                                    Font-Size="Small" Font-Underline="False"
                                    Text="RÓTULOS"></asp:Label>
                                <br />
                                <asp:ImageButton ID="imgbAddRotulo" runat="server" 
                                    ImageUrl="~/Content/images/add.png" onclick="imgbAddRotulo_Click" />
                                <asp:Label ID="lblAddRotulo" runat="server" style="font-size: small" 
                                    Text="Inserir Novo Rótulo"></asp:Label>
                                <asp:GridView ID="gdvRotulos" runat="server" AllowPaging="True" 
                                    AllowSorting="True" AutoGenerateColumns="False" CellPadding="4" 
                                    DataKeyNames="ID" DataSourceID="RotulosSqlDataSource" ForeColor="#333333" 
                                    GridLines="None" PageSize="3" style="font-size: xx-small; text-align: center" 
                                    Width="757px" onrowdatabound="gdvRotulos_RowDataBound" 
                                        onrowupdated="gdvRotulos_RowUpdated">
                                    <AlternatingRowStyle BackColor="White" />
                                    <Columns>
                                        <asp:TemplateField ShowHeader="False">
                                            <EditItemTemplate>
                                                <asp:ImageButton ID="imgbSaveRotulo" runat="server" CausesValidation="True" 
                                                    CommandName="Update" ImageUrl="~/Content/images/Sim.png" Text="Update" />
                                                &nbsp;<asp:ImageButton ID="imgbCancelRotulo" runat="server" CausesValidation="False" 
                                                    CommandName="Cancel" ImageUrl="~/Content/images/Nao.png" Text="Cancel" />
                                            </EditItemTemplate>
                                            <FooterTemplate>
                                                <asp:ImageButton ID="imgbSaveRotulo" runat="server" CausesValidation="True" 
                                                    CommandName="Update" ImageUrl="~/Content/images/Sim.png" Text="Update" 
                                                    onclick="imgbSaveRotulo_Click" />
                                                <asp:ImageButton ID="imgbCancelRotulo" runat="server" CausesValidation="False" 
                                                    CommandName="Cancel" ImageUrl="~/Content/images/Nao.png" Text="Cancel" 
                                                    onclick="imgbCancelRotulo_Click" />
                                            </FooterTemplate>
                                            <ItemTemplate>
                                                <asp:ImageButton ID="imgbEdit" runat="server" CausesValidation="False" 
                                                    CommandName="Edit" ImageUrl="~/Content/images/kjots.png" Text="Edit" />
                                                &nbsp;<asp:ImageButton ID="imgbCancelEdit" runat="server" CausesValidation="False" 
                                                    CommandName="Delete" ImageUrl="~/Content/images/button_cancel.png" 
                                                    Text="Delete" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="ID" InsertVisible="False" SortExpression="ID">
                                            <EditItemTemplate>
                                                <asp:Label ID="Label1" runat="server" Text='<%# Eval("ID") %>'></asp:Label>
                                            </EditItemTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("ID") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Hatch_Loc" SortExpression="Hatch_Loc" 
                                            Visible="False">
                                            <EditItemTemplate>
                                                <asp:Label ID="Label2" runat="server" Text='<%# Eval("Hatch_Loc") %>'></asp:Label>
                                            </EditItemTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("Hatch_Loc") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Set_date" SortExpression="Set_date" 
                                            Visible="False">
                                            <EditItemTemplate>
                                                <asp:Label ID="Label3" runat="server" Text='<%# Eval("Set_date") %>'></asp:Label>
                                            </EditItemTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="Label3" runat="server" Text='<%# Bind("Set_date") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="OrderNoCHIC" SortExpression="OrderNoCHIC" 
                                            Visible="False">
                                            <EditItemTemplate>
                                                <asp:Label ID="Label4" runat="server" Text='<%# Eval("OrderNoCHIC") %>'></asp:Label>
                                            </EditItemTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="Label4" runat="server" Text='<%# Bind("OrderNoCHIC") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Variety" SortExpression="Variety" 
                                            Visible="False">
                                            <EditItemTemplate>
                                                <asp:Label ID="Label5" runat="server" Text='<%# Eval("Variety") %>'></asp:Label>
                                            </EditItemTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="Label8" runat="server" Text='<%# Bind("Variety") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Cor" SortExpression="Cor">
                                            <EditItemTemplate>
                                                <asp:DropDownList ID="ddlCores" runat="server" 
                                                    SelectedValue='<%# Bind("Cor") %>' style="text-align: center">
                                                    <asp:ListItem>AMARELO</asp:ListItem>
                                                    <asp:ListItem>AZUL ESCURO</asp:ListItem>
                                                    <asp:ListItem>AZUL CLARO</asp:ListItem>
                                                    <asp:ListItem>MARROM ESCURO</asp:ListItem>
                                                    <asp:ListItem>MARROM CLARO</asp:ListItem>
                                                    <asp:ListItem>ROSA</asp:ListItem>
                                                    <asp:ListItem>PRETO</asp:ListItem>
                                                    <asp:ListItem>SEM RÓTULO</asp:ListItem>
                                                    <asp:ListItem>ROXO</asp:ListItem>
                                                    <asp:ListItem>VERDE ESCURO</asp:ListItem>
                                                    <asp:ListItem>VERDE CLARO</asp:ListItem>
                                                    <asp:ListItem>VERMELHO</asp:ListItem>
                                                    <asp:ListItem>A</asp:ListItem>
                                                    <asp:ListItem>B</asp:ListItem>
                                                    <asp:ListItem>C</asp:ListItem>
                                                </asp:DropDownList>
                                            </EditItemTemplate>
                                            <FooterTemplate>
                                                <asp:DropDownList ID="ddlCores" runat="server" 
                                                    SelectedValue='<%# Bind("Cor") %>'>
                                                    <asp:ListItem>AMARELO</asp:ListItem>
                                                    <asp:ListItem>AZUL ESCURO</asp:ListItem>
                                                    <asp:ListItem>AZUL CLARO</asp:ListItem>
                                                    <asp:ListItem>MARROM ESCURO</asp:ListItem>
                                                    <asp:ListItem>MARROM CLARO</asp:ListItem>
                                                    <asp:ListItem>ROSA</asp:ListItem>
                                                    <asp:ListItem>PRETO</asp:ListItem>
                                                    <asp:ListItem>SEM RÓTULO</asp:ListItem>
                                                    <asp:ListItem>ROXO</asp:ListItem>
                                                    <asp:ListItem>VERDE ESCURO</asp:ListItem>
                                                    <asp:ListItem>VERDE CLARO</asp:ListItem>
                                                    <asp:ListItem>VERMELHO</asp:ListItem>
                                                    <asp:ListItem>A</asp:ListItem>
                                                    <asp:ListItem>B</asp:ListItem>
                                                    <asp:ListItem>C</asp:ListItem>
                                                </asp:DropDownList>
                                            </FooterTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="Label5" runat="server" Text='<%# Bind("Cor") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Peso Médio" SortExpression="PesoMedio">
                                            <EditItemTemplate>
                                                <asp:TextBox ID="txtPesoMedio" runat="server" Text='<%# Bind("PesoMedio") %>'
                                                    onkeyup = "mascara(this, mvalor);"></asp:TextBox>
                                            </EditItemTemplate>
                                            <FooterTemplate>
                                                <asp:TextBox ID="txtPesoMedio" runat="server" onkeyup="mascara(this, mvalor);" 
                                                    Text='<%# Bind("PesoMedio") %>'></asp:TextBox>
                                            </FooterTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="Label6" runat="server" Text='<%# Bind("PesoMedio") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Uniformidade" SortExpression="Uniformidade">
                                            <EditItemTemplate>
                                                <asp:TextBox ID="txtUniformidade" runat="server" 
                                                    Text='<%# Bind("Uniformidade") %>'
                                                    onkeyup="mascara(this, mvalor);"></asp:TextBox>
                                            </EditItemTemplate>
                                            <FooterTemplate>
                                                <asp:TextBox ID="txtUniformidade" runat="server" 
                                                    onkeyup="mascara(this, mvalor);" Text='<%# Bind("Uniformidade") %>'></asp:TextBox>
                                            </FooterTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="Label7" runat="server" Text='<%# Bind("Uniformidade") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                    <EditRowStyle BackColor="#2461BF" />
                                    <FooterStyle BackColor="#00CCFF" Font-Bold="True" ForeColor="White" />
                                    <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                    <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                                    <RowStyle BackColor="#EFF3FB" />
                                    <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                                    <SortedAscendingCellStyle BackColor="#F5F7FB" />
                                    <SortedAscendingHeaderStyle BackColor="#6D95E1" />
                                    <SortedDescendingCellStyle BackColor="#E9EBEF" />
                                    <SortedDescendingHeaderStyle BackColor="#4870BE" />
                                </asp:GridView>
                                <asp:SqlDataSource ID="RotulosSqlDataSource" runat="server" 
                                    ConnectionString="<%$ ConnectionStrings:HLBAPPConnectionString %>" 
                                    DeleteCommand="DELETE FROM [HATCHERY_ORDER_ROTULO_DATA] WHERE [ID] = @ID" 
                                    InsertCommand="INSERT INTO [HATCHERY_ORDER_ROTULO_DATA] ([Hatch_Loc], [Set_date], [OrderNoCHIC], [Cor], [PesoMedio], [Uniformidade]) VALUES (@Hatch_Loc, @Set_date, @OrderNoCHIC, @Cor, @PesoMedio, @Uniformidade)" 
                                    SelectCommand="SELECT * FROM [HATCHERY_ORDER_ROTULO_DATA] WHERE (([Hatch_Loc] = @Hatch_Loc) AND ([Set_date] = @Set_date) AND ([OrderNoCHIC] = @OrderNoCHIC) AND(Variety = @Linhagem or Variety is null)) or (OrderNoCHIC = 'Branco')" 
                                    
                                        
                                        UpdateCommand="UPDATE [HATCHERY_ORDER_ROTULO_DATA] SET [Hatch_Loc] = @Hatch_Loc, [Set_date] = @Set_date, [OrderNoCHIC] = @OrderNoCHIC, [Cor] = @Cor, [PesoMedio] = @PesoMedio, [Uniformidade] = @Uniformidade, [Variety] = @Linhagem WHERE [ID] = @ID">
                                    <DeleteParameters>
                                        <asp:Parameter Name="ID" Type="Int32" />
                                    </DeleteParameters>
                                    <InsertParameters>
                                        <asp:Parameter Name="Hatch_Loc" Type="String" />
                                        <asp:Parameter Name="Set_date" Type="DateTime" />
                                        <asp:Parameter Name="OrderNoCHIC" Type="String" />
                                        <asp:Parameter Name="Cor" Type="String" />
                                        <asp:Parameter Name="PesoMedio" Type="Decimal" />
                                        <asp:Parameter Name="Uniformidade" Type="Decimal" />
                                    </InsertParameters>
                                    <SelectParameters>
                                        <asp:ControlParameter ControlID="ddlIncubatorios" Name="Hatch_Loc" 
                                            PropertyName="SelectedValue" Type="String" />
                                        <asp:ControlParameter ControlID="Calendar1" Name="Set_date" 
                                            PropertyName="SelectedDate" Type="DateTime" />
                                        <asp:SessionParameter Name="OrderNoCHIC" SessionField="numPedidoSelecionado" 
                                            Type="String" />
                                        <asp:SessionParameter Name="Linhagem" SessionField="linhagemSelecionada" />
                                    </SelectParameters>
                                    <UpdateParameters>
                                        <asp:Parameter Name="Hatch_Loc" Type="String" />
                                        <asp:Parameter Name="Set_date" Type="DateTime" />
                                        <asp:Parameter Name="OrderNoCHIC" Type="String" />
                                        <asp:Parameter Name="Cor" Type="String" />
                                        <asp:Parameter Name="PesoMedio" Type="Decimal" />
                                        <asp:Parameter Name="Uniformidade" Type="Decimal" />
                                        <asp:Parameter Name="Linhagem" />
                                        <asp:Parameter Name="ID" Type="Int32" />
                                    </UpdateParameters>
                                </asp:SqlDataSource>
                                <br />
                                <br />
                                </asp:Panel>
                                <asp:Label ID="lblPintosNascidos" runat="server" Font-Bold="True" 
                                    Font-Size="Small" Font-Underline="False"
                                    Text="PINTOS NASCIDOS DISPONÍVEIS"></asp:Label>
                                <table align="center">
                                    <tr>
                                        <td class="style57">
                                            <asp:Label ID="lblLocalizarLotes" runat="server" CssClass="style61" 
                                                Font-Size="XX-Small" Text="Localizar:"></asp:Label>
                                        </td>
                                        <td class="style22">
                                            <asp:TextBox ID="txtLocalizarLotes" runat="server" CssClass="style24" 
                                                Width="193px" Height="15px"></asp:TextBox>
                                        </td>
                                        <td class="style62">
                                            <asp:DropDownList ID="ddlCamposLotes" runat="server" Height="21px" Style="font-weight: 700"
                                                Width="124px" CssClass="style60">
                                                <asp:ListItem>Linhagem</asp:ListItem>
                                                <asp:ListItem>Lote</asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                        <td class="style62">
                                            <asp:DropDownList ID="ddlClassOvos" runat="server" Height="21px" Style="font-weight: 700"
                                                Width="124px" CssClass="style60" AutoPostBack="True">
                                                <asp:ListItem Value="(Todos)">(Todos)</asp:ListItem>
                                                <asp:ListItem>T0</asp:ListItem>
                                                <asp:ListItem>T1</asp:ListItem>
                                                <asp:ListItem>T2</asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                        <td class="style39">
                                            <asp:Button ID="btnPesquisarLotes" runat="server" Font-Bold="True" Font-Size="XX-Small"
                                                Height="21px" OnClick="btnPesquisarLotes_Click" Text="Pesquisar" Width="118px" 
                                                CssClass="style60" />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td style="text-align: center">
                                <table style="width: 100%; text-align: center;">
                                    <tr>
                                        <td align="center">
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:GridView ID="gdvLotes" runat="server" AllowSorting="True" CellPadding="4" DataSourceID="SqlDataSource1"
                                    ForeColor="#333333" GridLines="None" Style="font-size: xx-small; text-align: center;"
                                    Width="758px" AutoGenerateColumns="False" OnSelectedIndexChanged="gdvLotes_SelectedIndexChanged"
                                    AllowPaging="True" PageSize="3">
                                    <AlternatingRowStyle BackColor="White" />
                                    <Columns>
                                        <asp:CommandField ButtonType="Image" 
                                            SelectImageUrl="~/Content/images/Next_16x16.gif" ShowSelectButton="True" />
                                        <asp:BoundField DataField="Data Incubação" DataFormatString="{0:d}" 
                                            HeaderText="Data Incubação" SortExpression="Data Incubação" />
                                        <asp:BoundField DataField="Lote Completo" HeaderText="Lote Completo" 
                                            SortExpression="Lote Completo" />
                                        <asp:BoundField DataField="Lote" HeaderText="Lote" SortExpression="Lote" />
                                        <asp:BoundField DataField="Linhagem" HeaderText="Linhagem" 
                                            SortExpression="Linhagem" />
                                        <asp:BoundField DataField="Class. Ovo" HeaderText="Class. Ovo" 
                                            SortExpression="Class. Ovo" />
                                        <asp:BoundField DataField="Pintos Vendáveis" HeaderText="Pintos Vendáveis" 
                                            ReadOnly="True" SortExpression="Pintos Vendáveis" 
                                            DataFormatString="{0:n0}" />
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
                                <asp:SqlDataSource ID="SqlDataSource1" runat="server" 
                                    ConnectionString="<%$ ConnectionStrings:HLBAPPConnectionString %>" SelectCommand="select
	Flock_id [Lote Completo],
	NumLote Lote,
	Variety Linhagem,
	ClassOvo [Class. Ovo],
	SUM(Pintos_Vendaveis) 
	-
	ISNULL((Select SUM(O.Qtde) from HATCHERY_ORDER_FLOCK_DATA O
		where H.Hatch_Loc = O.Hatch_Loc and H.Set_date = O.Set_date
		and H.Flock_id = O.Flock_id and H.ClassOvo = O.ClassOvo),0)
	[Pintos Vendáveis],
                   Set_date [Data Incubação]
from HLBAPP.dbo.HATCHERY_FLOCK_SETTER_DATA H With(Nolock)
where Set_date &gt;= @SetDate-2 and Set_date &lt;= @SetDate+1
        and Hatch_loc = @Incubatorio and
	(
        (@Pesquisa ='0')
        or
        (@Campo = 'Linhagem' and H.Variety like '%' + @Pesquisa + '%')
        or
        (@Campo = 'Lote' and H.Flock_id like '%' + @Pesquisa + '%')
    ) and (ClassOvo = @ClassOvo or @ClassOvo = '(Todos)')
group by
	Hatch_loc,
	Set_date,
	Flock_id,
	Variety,
	NumLote,
	ClassOvo
having
	(SUM(Pintos_Vendaveis) 
	-
	ISNULL((Select SUM(O.Qtde) from HATCHERY_ORDER_FLOCK_DATA O
		where H.Hatch_Loc = O.Hatch_Loc and H.Set_date = O.Set_date
		and H.Flock_id = O.Flock_id and H.ClassOvo = O.ClassOvo),0)) &gt; 0
order by
                  Set_Date,
	Flock_id">
                                    <SelectParameters>
                                        <asp:ControlParameter ControlID="Calendar1" Name="SetDate"
                                            PropertyName="SelectedDate" DefaultValue="" Type="DateTime" />
                                        <asp:ControlParameter ControlID="ddlIncubatorios" Name="Incubatorio" 
                                            PropertyName="SelectedValue" DefaultValue="" />
                                        <asp:ControlParameter ControlID="txtLocalizarLotes" Name="Pesquisa" PropertyName="Text" 
                                            DefaultValue="" />
                                        <asp:ControlParameter ControlID="ddlCamposLotes" Name="Campo" 
                                            PropertyName="SelectedValue" DefaultValue="" />
                                        <asp:ControlParameter ControlID="ddlClassOvos" Name="ClassOvo" 
                                            PropertyName="SelectedValue" DefaultValue="" />
                                    </SelectParameters>
                                </asp:SqlDataSource>
                            </td>
                            <td colspan="2">
                                <asp:FormView ID="frvLoteSelecionado" runat="server" CellPadding="4" 
                                    Height="163px" BackColor="White"
                                    BorderColor="#CC9966" BorderStyle="None" BorderWidth="1px"
                                    DataSourceID="HatchFormDataSource" GridLines="Both" Style="font-size: xx-small"
                                    OnDataBound="FormView1_DataBound" Width="228px" 
                                    OnItemUpdated="FormView1_ItemUpdated">
                                    <EditItemTemplate>
                                        <table style="width: 100%;">
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblDataIncubacaoTitulo" runat="server" 
                                                    Text="Data Incubação:"></asp:Label>
                                                </td>
                                                <td>
                                                    <asp:Label ID="lblDataIncubacao" runat="server" 
                                                    Text='<%# Eval("Set_date") %>'></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblLoteCompletoTitulo" runat="server" 
                                                    Text="Lote Completo:"></asp:Label>
                                                </td>
                                                <td>
                                                    <asp:Label ID="lblLoteCompleto" runat="server" 
                                                    Text='<%# Eval("LoteCompleto") %>'></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblLinhagemTitulo" runat="server" 
                                                    Text="Linhagem:"></asp:Label>
                                                </td>
                                                <td>
                                                    <asp:Label ID="lblLinhagem" runat="server" 
                                                    Text='<%# Eval("Linhagem") %>'></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblClassOvoTitulo" runat="server" 
                                                    Text="Tipo Ovo:"></asp:Label>
                                                </td>
                                                <td>
                                                    <asp:Label ID="lblClassOvo" runat="server" 
                                                    Text='<%# Eval("ClassOvo") %>'></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblQtde" runat="server" 
                                                    Text="Qtde.:"></asp:Label>
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="txtQtde" runat="server" 
                                                        Text='<%# Bind("Pintos_Vendaveis") %>' AutoPostBack="True" 
                                                        ontextchanged="txtQtde_TextChanged" onkeyup = "mascara(this, mnum);" />
                                                    <br />
                                                    <asp:Label ID="lblErroFrmLoteSelecionado" runat="server" Font-Bold="True" 
                                                        ForeColor="Red" Visible="False"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblRotulo" runat="server" 
                                                    Text="Rótulo:"></asp:Label>
                                                </td>
                                                <td>
                                                    <asp:DropDownList ID="ddlRotulo" runat="server" 
                                                        DataSourceID="RotulosSqlDataSource" DataTextField="Cor" DataValueField="Cor" 
                                                        SelectedValue='<%# Bind("Rotulo") %>'>
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblMascara" runat="server" Text="Máscara:"></asp:Label>
                                                </td>
                                                <td>
                                                    <asp:DropDownList ID="ddlMascara" runat="server" 
                                                        SelectedValue='<%# Bind("Mascara") %>'>
                                                        <asp:ListItem>Nenhuma</asp:ListItem>
                                                        <asp:ListItem>23/23</asp:ListItem>
                                                        <asp:ListItem>24/23</asp:ListItem>
                                                        <asp:ListItem>26/23</asp:ListItem>
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblPoderLampada" runat="server" Text="Poder Lâmpada:"></asp:Label>
                                                </td>
                                                <td>
                                                    <asp:DropDownList ID="ddlPoderLampada" runat="server" 
                                                        SelectedValue='<%# Bind("PoderLampada") %>'>
                                                        <asp:ListItem>Nenhum</asp:ListItem>
                                                        <asp:ListItem>38</asp:ListItem>
                                                        <asp:ListItem>39</asp:ListItem>
                                                        <asp:ListItem>40</asp:ListItem>
                                                        <asp:ListItem>41</asp:ListItem>
                                                        <asp:ListItem>42</asp:ListItem>
                                                        <asp:ListItem>44</asp:ListItem>
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                            <%--<tr>
                                                <td>
                                                    <asp:Label ID="lblPeso" runat="server" Text="Peso:"></asp:Label>
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="txtPeso" runat="server" Text='<%# Bind("Peso") %>' 
                                                        onkeyup = "mascara(this, mvalor);"/>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblUniformidade" runat="server" Text="Uniformidade:"></asp:Label>
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="txtUniformidade" runat="server" Text='<%# Bind("Uniformidade") %>' 
                                                        onkeyup = "mascara(this, mvalor);"/>
                                                </td>
                                            </tr>--%>
                                            <tr>
                                                <td colspan="2" style="text-align: center">
                                                    <asp:Label ID="lblTitleFalta" runat="server" Text="Caso ocorreu falta no lote, informar os campos abaixo:" style="text-decoration: underline; text-align: center; font-size: small"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblFaltaQtde" runat="server" Text="Qtde. que Faltou:"></asp:Label>
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="txtFaltaQtde" runat="server" Text='<%# Bind("FaltaQtde") %>' 
                                                        onkeyup = "mascara(this, mnum);"/>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblFaltaMotivo" runat="server" Text="Motivo da Falta:"></asp:Label>
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="txtFaltaMotivo" runat="server" Text='<%# Bind("FaltaMotivo") %>' Rows="5" TextMode="MultiLine" />
                                                    <asp:Label ID="lblErroFrmLoteSelecionadoMotivo" runat="server" Font-Bold="True" 
                                                        ForeColor="Red" Visible="False"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="2" style="text-align: center">
                                                    <asp:LinkButton ID="UpdateButton" runat="server" 
                                                        CausesValidation="True" 
                                                        Text="Salvar" style="text-align: center" onclick="UpdateButton_Click1" />
                                                    &nbsp;&nbsp;&nbsp;&nbsp;
                                                    <asp:LinkButton ID="CancelButton" runat="server" 
                                                        CausesValidation="True" 
                                                        Text="Cancelar" style="text-align: center" onclick="CancelButton_Click" />
                                                </td>
                                            </tr>
                                        </table>
                                    </EditItemTemplate>
                                    <EditRowStyle BackColor="#FFCC66" Font-Bold="True" ForeColor="#663399" />
                                    <FooterStyle BackColor="#FFFFCC" ForeColor="#330099" />
                                    <HeaderStyle BackColor="#990000" Font-Bold="True" ForeColor="#FFFFCC" />
                                    <InsertItemTemplate>
                                        LoteCompleto:
                                        <asp:TextBox ID="LoteCompletoTextBox" runat="server" 
                                            Text='<%# Bind("LoteCompleto") %>' />
                                        <br />
                                        Lote:
                                        <asp:TextBox ID="LoteTextBox" runat="server" 
                                            Text='<%# Bind("Lote") %>' />
                                        <br />
                                        Linhagem:
                                        <asp:TextBox ID="LinhagemTextBox" runat="server" 
                                            Text='<%# Bind("Linhagem") %>' />
                                        <br />
                                        ClassOvo:
                                        <asp:TextBox ID="ClassOvoTextBox" runat="server" 
                                            Text='<%# Bind("ClassOvo") %>' />
                                        <br />
                                        Pintos_Vendaveis:
                                        <asp:TextBox ID="Pintos_VendaveisTextBox" runat="server" 
                                            Text='<%# Bind("Pintos_Vendaveis") %>' />
                                        <br />
                                        Rotulo:
                                        <asp:TextBox ID="RotuloTextBox" runat="server" Text='<%# Bind("Rotulo") %>' />
                                        <br />
                                        <asp:LinkButton ID="InsertButton" runat="server" CausesValidation="True" CommandName="Insert"
                                            Text="Insert" />
                                        &nbsp;<asp:LinkButton ID="InsertCancelButton" runat="server" CausesValidation="False"
                                            CommandName="Cancel" Text="Cancel" />
                                    </InsertItemTemplate>
                                    <ItemTemplate>
                                        LoteCompleto:
                                        <asp:Label ID="LoteCompletoLabel" runat="server" 
                                            Text='<%# Bind("LoteCompleto") %>' />
                                        <br />
                                        Lote:
                                        <asp:Label ID="LoteLabel" runat="server" 
                                            Text='<%# Bind("Lote") %>' />
                                        <br />
                                        Linhagem:
                                        <asp:Label ID="LinhagemLabel" runat="server" 
                                            Text='<%# Bind("Linhagem") %>' />
                                        <br />
                                        ClassOvo:
                                        <asp:Label ID="ClassOvoLabel" runat="server" 
                                            Text='<%# Bind("ClassOvo") %>' />
                                        <br />
                                        Pintos_Vendaveis:
                                        <asp:Label ID="Pintos_VendaveisLabel" runat="server" 
                                            Text='<%# Bind("Pintos_Vendaveis") %>' />
                                        <br />
                                        Rotulo:
                                        <asp:Label ID="RotuloLabel" runat="server" Text='<%# Bind("Rotulo") %>' />
                                        <br />
                                        Máscara:
                                        <asp:Label ID="MascaraLabel" runat="server" Text='<%# Bind("Mascara") %>' />
                                        <br />
                                        Poder de Lâmpada:
                                        <asp:Label ID="PoderLampadaLabel" runat="server" Text='<%# Bind("PoderLampada") %>' />
                                        <br />
                                    </ItemTemplate>
                                    <PagerStyle BackColor="#FFFFCC" ForeColor="#330099" HorizontalAlign="Center" />
                                    <RowStyle BackColor="White" ForeColor="#330099" />
                                </asp:FormView>
                                <asp:SqlDataSource ID="HatchFormDataSource" runat="server" 
                                    ConnectionString="<%$ ConnectionStrings:HLBAPPConnectionString %>" 
                                    
                                    
                                    
                                    
                                    
                                    SelectCommand="select
                   Set_date,
	Flock_id LoteCompleto,
	NumLote Lote,
	Variety Linhagem,
	ClassOvo ClassOvo,
	SUM(Pintos_Vendaveis) 
	-
	ISNULL((Select SUM(O.Qtde) from HATCHERY_ORDER_FLOCK_DATA O
		where H.Hatch_Loc = O.Hatch_Loc and H.Set_date = O.Set_date
		and H.Flock_id = O.Flock_id and H.ClassOvo = O.ClassOvo),0)
	Pintos_Vendaveis,
                   '' Rotulo,
	'Nenhuma' Mascara,
	'Nenhum' PoderLampada,
                   0 FaltaQtde,
                   '' FaltaMotivo,
                   0 Peso,
                   0 Uniformidade
from HLBAPP.dbo.HATCHERY_FLOCK_SETTER_DATA H With(Nolock)
where Set_date = @SetDate and Hatch_loc = @Incubatorio
    and H.Flock_id = @FlockId
    and ClassOvo = @ClassOvo
group by
	Hatch_loc,
	Set_date,
	Flock_id,
	Variety,
	NumLote,
	ClassOvo
having
	(SUM(Pintos_Vendaveis) 
	-
	ISNULL((Select SUM(O.Qtde) from HATCHERY_ORDER_FLOCK_DATA O
		where H.Hatch_Loc = O.Hatch_Loc and H.Set_date = O.Set_date
		and H.Flock_id = O.Flock_id and H.ClassOvo = O.ClassOvo),0)) &gt; 0
order by
	Flock_id" ConflictDetection="CompareAllValues">
                                    <SelectParameters>
                                        <asp:Parameter Name="SetDate" />
                                        <asp:ControlParameter ControlID="ddlIncubatorios" Name="Incubatorio" 
                                            PropertyName="SelectedValue" DefaultValue="" />
                                        <asp:Parameter Name="FlockId" />
                                        <asp:Parameter Name="ClassOvo" />
                                    </SelectParameters>
                                </asp:SqlDataSource>
                                <asp:Label ID="lblMensagem" runat="server" Style="font-weight: 700; color: #FF3300"
                                    Visible="False"></asp:Label>
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
                <asp:Panel ID="Panel4" runat="server" Width="1002px" HorizontalAlign="Center" Style="text-align: left">
                    <table style="width: 100%;">
                        <tr>
                            <td class="style64">
                                <br />
                                <asp:Label ID="lblLotesRelacionados" runat="server" Font-Bold="True" 
                                    Font-Size="Small" Font-Underline="False"
                                    Text="LOTES RELACIONADOS"></asp:Label>
                                <table align="center">
                                    <tr>
                                        <td class="style57">
                                            <asp:Label ID="lblLocalizarLotesRelacionados" runat="server" CssClass="style61" 
                                                Font-Size="XX-Small" Text="Localizar:"></asp:Label>
                                        </td>
                                        <td class="style22">
                                            <asp:TextBox ID="txtPesquisaLotesRelacionados" runat="server" CssClass="style24" 
                                                Width="193px" Height="15px"></asp:TextBox>
                                        </td>
                                        <td class="style62">
                                            <asp:DropDownList ID="ddlCamposLotesRelacionados" runat="server" Height="21px" Style="font-weight: 700"
                                                Width="124px" CssClass="style60">
                                                <asp:ListItem>Linhagem</asp:ListItem>
                                                <asp:ListItem>Lote</asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                        <td class="style62">
                                            <asp:DropDownList ID="ddlTipoOvoLotesRelacionados" runat="server" Height="21px" Style="font-weight: 700"
                                                Width="124px" CssClass="style60" AutoPostBack="True">
                                                <asp:ListItem Value="(Todos)">(Todos)</asp:ListItem>
                                                <asp:ListItem>T0</asp:ListItem>
                                                <asp:ListItem>T1</asp:ListItem>
                                                <asp:ListItem>T2</asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                        <td class="style39">
                                            <asp:Button ID="btnPesquisarLotesRelacionados" runat="server" Font-Bold="True" Font-Size="XX-Small"
                                                Height="21px" Text="Pesquisar" Width="118px" 
                                                CssClass="style60" onclick="btnPesquisarLotesRelacionados_Click" />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td style="text-align: center">
                                <table style="width: 100%; text-align: center;">
                                    <tr>
                                        <td align="center">
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <asp:GridView ID="gdvLotesSelecionados" runat="server" AllowSorting="True" 
                                    CellPadding="4" DataSourceID="SqlDataSource2"
                                    ForeColor="#333333" GridLines="None" Style="font-size: xx-small; text-align: center; margin-right: 0px;"
                                    Width="758px" AutoGenerateColumns="False"
                                    AllowPaging="True" DataKeyNames="ID" 
                                    onrowediting="gdvLotesSelecionados_RowEditing" PageSize="3" 
                                    ShowHeaderWhenEmpty="True" onrowdeleted="gdvLotesSelecionados_RowDeleted">
                                    <AlternatingRowStyle BackColor="White" />
                                    <Columns>
                                        <asp:TemplateField ShowHeader="False">
                                            <EditItemTemplate>
                                                <asp:ImageButton ID="imgbSaveLoteRelacionado" runat="server" 
                                                    CausesValidation="True" Text="Update" ImageUrl="~/Content/images/Sim.png" 
                                                    onclick="imgbSaveLoteRelacionado_Click" />
                                                &nbsp;<asp:ImageButton ID="imgbCancelLoteRelacionado" runat="server" CausesValidation="False" 
                                                    CommandName="Cancel" Text="Cancel" ImageUrl="~/Content/images/Nao.png" />
                                            </EditItemTemplate>
                                            <ItemTemplate>
                                                <asp:ImageButton ID="imgEditLoteRelacionado" runat="server" 
                                                    CausesValidation="False" CommandName="Edit" 
                                                    ImageUrl="~/Content/images/kjots.png" Text="Edit" />
                                                &nbsp;<asp:ImageButton ID="imgbDeleteLoteSelecionado" runat="server" CausesValidation="False" 
                                                    CommandName="Delete" ImageUrl="~/Content/images/button_cancel.png" 
                                                    Text="Delete" onclick="imgbDeleteLoteSelecionado_Click" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="ID" HeaderText="ID" ReadOnly="True" 
                                            SortExpression="ID" />
                                        <asp:BoundField DataField="Hatch_Loc" HeaderText="Hatch_Loc" 
                                            SortExpression="Hatch_Loc" Visible="False" />
                                        <asp:BoundField DataField="Set_date" HeaderText="Data Incubação" 
                                            SortExpression="Set_date" DataFormatString="{0:d}" ReadOnly="True" />
                                        <asp:BoundField DataField="OrderNoCHIC" HeaderText="OrderNoCHIC" 
                                            SortExpression="OrderNoCHIC" Visible="False" />
                                        <asp:BoundField DataField="Flock_id" HeaderText="Lote Completo" ReadOnly="True" 
                                            SortExpression="Flock_id" />
                                        <asp:BoundField DataField="NumLote" HeaderText="Lote" ReadOnly="True" 
                                            SortExpression="NumLote" />
                                        <asp:BoundField DataField="Variety" HeaderText="Linhagem" 
                                            ReadOnly="True" SortExpression="Variety" />
                                        <asp:BoundField DataField="ClassOvo" HeaderText="Tipo Ovo" ReadOnly="True" 
                                            SortExpression="ClassOvo" />
                                        <asp:TemplateField HeaderText="Qtde." SortExpression="Qtde">
                                            <EditItemTemplate>
                                                <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("Qtde") %>' 
                                                    AutoPostBack="True" ontextchanged="TextBox1_TextChanged"
                                                    onkeyup = "mascara(this, mnum);"></asp:TextBox>
                                                <br />
                                                <asp:Label ID="lblMensagemQtdeLoteRelacionado" runat="server" Font-Bold="True" 
                                                    ForeColor="Red"></asp:Label>
                                            </EditItemTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("Qtde", "{0:N0}") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Rótulo" SortExpression="Rotulo">
                                            <EditItemTemplate>
                                                <asp:DropDownList ID="ddlRotulo" runat="server" 
                                                    DataSourceID="RotulosSqlDataSource" DataTextField="Cor" DataValueField="Cor" 
                                                    SelectedValue='<%# Bind("Rotulo") %>'>
                                                </asp:DropDownList>
                                            </EditItemTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("Rotulo") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Máscara - Debicagem" SortExpression="TIMascara">
                                            <EditItemTemplate>
                                                <asp:DropDownList ID="ddlMascara" runat="server" 
                                                    SelectedValue='<%# Bind("TIMascara") %>'>
                                                    <asp:ListItem>Nenhuma</asp:ListItem>
                                                    <asp:ListItem>23/23</asp:ListItem>
                                                    <asp:ListItem>24/23</asp:ListItem>
                                                    <asp:ListItem>26/23</asp:ListItem>
                                                </asp:DropDownList>
                                            </EditItemTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="lblMascara" runat="server" Text='<%# Bind("TIMascara") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Poder da Lâmpada - Debicagem" 
                                            SortExpression="TIPoderLampada">
                                            <EditItemTemplate>
                                                <asp:DropDownList ID="ddlPoderLampada" runat="server" 
                                                    SelectedValue='<%# Bind("TIPoderLampada") %>'>
                                                    <asp:ListItem>Nenhum</asp:ListItem>
                                                    <asp:ListItem>38</asp:ListItem>
                                                    <asp:ListItem>39</asp:ListItem>
                                                    <asp:ListItem>40</asp:ListItem>
                                                    <asp:ListItem>41</asp:ListItem>
                                                    <asp:ListItem>42</asp:ListItem>
                                                    <asp:ListItem>44</asp:ListItem>
                                                </asp:DropDownList>
                                            </EditItemTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="lblPoderLampada" runat="server" Text='<%# Bind("TIPoderLampada") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Qtde. Falta" SortExpression="FaltaQtde">
                                            <EditItemTemplate>
                                                <asp:TextBox ID="txtFaltaQtde" runat="server" Text='<%# Bind("FaltaQtde") %>' 
                                                    onkeyup = "mascara(this, mnum);"></asp:TextBox>
                                            </EditItemTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="lblFaltaQtde" runat="server" Text='<%# Bind("FaltaQtde", "{0:N0}") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Motivo da Falta" SortExpression="FaltaMotivo">
                                            <EditItemTemplate>
                                                <asp:TextBox ID="txtFaltaMotivo" runat="server" Text='<%# Bind("FaltaMotivo") %>' TextMode="MultiLine" Rows="5"></asp:TextBox>
                                                <asp:Label ID="lblErroFrmLoteSelecionadoMotivo" runat="server" Font-Bold="True" 
                                                        ForeColor="Red" Visible="False"></asp:Label>
                                            </EditItemTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="lblFaltaMotivo" runat="server" Text='<%# Bind("FaltaMotivo") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField ConvertEmptyStringToNull="False" HeaderText="Peso" SortExpression="Peso" Visible="False">
                                            <EditItemTemplate>
                                                <asp:TextBox ID="txtPeso" runat="server" Text='<%# Bind("Peso") %>' 
                                                    onkeyup = "mascara(this, mvalor);"></asp:TextBox>
                                            </EditItemTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="lblPeso" runat="server" Text='<%# Bind("Peso", "{0:N0}") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField ConvertEmptyStringToNull="False" HeaderText="Uniformidade" SortExpression="Uniformidade" Visible="False">
                                            <EditItemTemplate>
                                                <asp:TextBox ID="txtUniformidade" runat="server" Text='<%# Bind("Uniformidade") %>' 
                                                    onkeyup = "mascara(this, mvalor);"></asp:TextBox>
                                            </EditItemTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="lblUniformidade" runat="server" Text='<%# Bind("Uniformidade", "{0:N0}") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                    <EditRowStyle BackColor="#EFF3FB" />
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
                                <asp:SqlDataSource ID="SqlDataSource2" runat="server" 
                                    ConnectionString="<%$ ConnectionStrings:HLBAPPConnectionString %>" SelectCommand="SELECT * FROM [HATCHERY_ORDER_FLOCK_DATA] WHERE (([Hatch_Loc] = @Hatch_Loc)) and
    (
        (@Pesquisa ='0')
        or
        (@Campo = 'Linhagem' and Variety like '%' + @Pesquisa + '%')
        or
        (@Campo = 'Lote' and Flock_id like '%' + @Pesquisa + '%')
    ) and (ClassOvo = @ClassOvo or @ClassOvo = '(Todos)')
and OrderNoCHIC = @NumPedido
and Variety = Replace(@Linhagem,'amp;','')" 
                                    DeleteCommand="DELETE FROM [HATCHERY_ORDER_FLOCK_DATA] WHERE [ID] = @ID" 
                                    InsertCommand="INSERT INTO [HATCHERY_ORDER_FLOCK_DATA] ([Hatch_Loc], [Set_date], [OrderNoCHIC], [Flock_id], [NumLote], [ClassOvo], [Qtde], [Variety]) VALUES (@Hatch_Loc, @Set_date, @OrderNoCHIC, @Flock_id, @NumLote, @ClassOvo, @Qtde, @Variety)" 
                                    
                                    
                                    
                                    UpdateCommand="UPDATE [HATCHERY_ORDER_FLOCK_DATA] SET [Hatch_Loc] = @Hatch_Loc, [Set_date] = @Set_date, [OrderNoCHIC] = @OrderNoCHIC, [Flock_id] = @Flock_id, [NumLote] = @NumLote, [ClassOvo] = @ClassOvo, [Qtde] = @Qtde, [Variety] = @Variety WHERE [ID] = @ID">
                                    <DeleteParameters>
                                        <asp:Parameter Name="ID" Type="Int32" />
                                    </DeleteParameters>
                                    <InsertParameters>
                                        <asp:Parameter Name="Hatch_Loc" Type="String" />
                                        <asp:Parameter Name="Set_date" Type="DateTime" />
                                        <asp:Parameter Name="OrderNoCHIC" Type="String" />
                                        <asp:Parameter Name="Flock_id" Type="String" />
                                        <asp:Parameter Name="NumLote" Type="String" />
                                        <asp:Parameter Name="ClassOvo" Type="String" />
                                        <asp:Parameter Name="Qtde" Type="Int32" />
                                        <asp:Parameter Name="Variety" Type="String" />
                                    </InsertParameters>
                                    <SelectParameters>
                                        <asp:ControlParameter ControlID="ddlIncubatorios" Name="Hatch_Loc" 
                                            PropertyName="SelectedValue" Type="String" />
                                        <asp:ControlParameter ControlID="txtPesquisaLotesRelacionados" Name="Pesquisa" 
                                            PropertyName="Text" />
                                        <asp:ControlParameter ControlID="ddlCamposLotesRelacionados" Name="Campo" 
                                            PropertyName="SelectedValue" />
                                        <asp:ControlParameter ControlID="ddlTipoOvoLotesRelacionados" Name="ClassOvo" 
                                            PropertyName="SelectedValue" />
                                        <asp:SessionParameter Name="NumPedido" 
                                            SessionField="Session[&quot;numPedidoSelecionado&quot;]" />
                                        <asp:SessionParameter DefaultValue="" Name="Linhagem" 
                                            SessionField="Session[&quot;linhagemSelecionada&quot;]" />
                                    </SelectParameters>
                                    <UpdateParameters>
                                        <asp:Parameter Name="Hatch_Loc" Type="String" />
                                        <asp:Parameter Name="Set_date" Type="DateTime" />
                                        <asp:Parameter Name="OrderNoCHIC" Type="String" />
                                        <asp:Parameter Name="Flock_id" Type="String" />
                                        <asp:Parameter Name="NumLote" Type="String" />
                                        <asp:Parameter Name="ClassOvo" Type="String" />
                                        <asp:Parameter Name="Qtde" Type="Int32" />
                                        <asp:Parameter Name="Variety" Type="String" />
                                        <asp:Parameter Name="ID" Type="Int32" />
                                    </UpdateParameters>
                                </asp:SqlDataSource>
                                <br />
                                <asp:Panel ID="pnlVacinas" runat="server"  Visible="False">
                                <asp:Label ID="lblVacinas" runat="server" Font-Bold="True" Font-Size="Small" 
                                    Font-Underline="False" Text="VACINAS"></asp:Label>
                                    <br />
                                <br />
                                <asp:ImageButton ID="imgbAdd" runat="server" 
                                    ImageUrl="~/Content/images/add.png" onclick="imgbAdd_Click" />
                                <asp:Label ID="lblAdd" runat="server" style="font-size: small" 
                                    Text="Inserir Nova Vacina"></asp:Label>
                                <asp:GridView ID="gdvVacinas" runat="server" AllowPaging="True" 
                                    AllowSorting="True" AutoGenerateColumns="False" CellPadding="4" 
                                    DataKeyNames="ID" DataSourceID="SqlDataSource3" ForeColor="#333333" 
                                    GridLines="None" onrowupdated="gdvVacinas_RowUpdated" PageSize="3" 
                                    ShowHeaderWhenEmpty="True" style="font-size: xx-small; text-align: center" 
                                    Width="758px" onrowdatabound="gdvVacinas_RowDataBound" OnRowCommand="gdvVacinas_RowCommand" OnRowUpdating="gdvVacinas_RowUpdating" OnRowCancelingEdit="gdvVacinas_RowCancelingEdit">
                                    <AlternatingRowStyle BackColor="White" />
                                    <Columns>
                                        <asp:TemplateField ShowHeader="False">
                                            <EditItemTemplate>
                                                <asp:ImageButton ID="imgbSaveVacina" runat="server" CausesValidation="True" 
                                                    CommandName="CustomUpdate" ImageUrl="~/Content/images/Sim.png" Text="Update" 
                                                    CommandArgument='<%# Eval("ID") %>' />
                                                &nbsp;<asp:ImageButton ID="imgbCancelVacina" runat="server" CausesValidation="False" 
                                                    CommandName="Cancel" ImageUrl="~/Content/images/Nao.png" Text="Cancel" />
                                            </EditItemTemplate>
                                            <FooterTemplate>
                                                <asp:ImageButton ID="imgbSaveVacina" runat="server" CausesValidation="True" 
                                                    ImageUrl="~/Content/images/Sim.png" onclick="imgbSaveVacina_Click" 
                                                    Text="Update" />
                                                <asp:ImageButton ID="imgbCancelVacina" runat="server" CausesValidation="False" 
                                                    CommandName="Cancel" ImageUrl="~/Content/images/Nao.png" 
                                                    onclick="imgbCancelVacina_Click" Text="Cancel" />
                                            </FooterTemplate>
                                            <ItemTemplate>
                                                <asp:ImageButton ID="imgbEdit" runat="server" CausesValidation="False" 
                                                    CommandName="Edit" ImageUrl="~/Content/images/kjots.png" Text="Edit" />
                                                &nbsp;<asp:ImageButton ID="imgbCancelEdit" runat="server" CausesValidation="False" 
                                                    CommandName="Delete" ImageUrl="~/Content/images/button_cancel.png" 
                                                    Text="Delete" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="ID" InsertVisible="False" SortExpression="ID">
                                            <EditItemTemplate>
                                                <asp:Label ID="Label1" runat="server" Text='<%# Eval("ID") %>'></asp:Label>
                                            </EditItemTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("ID") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Hatch_Loc" SortExpression="Hatch_Loc" 
                                            Visible="False">
                                            <EditItemTemplate>
                                                <asp:Label ID="Label2" runat="server" Text='<%# Eval("Hatch_Loc") %>'></asp:Label>
                                            </EditItemTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("Hatch_Loc") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Set_date" SortExpression="Set_date" 
                                            Visible="False">
                                            <EditItemTemplate>
                                                <asp:Label ID="Label3" runat="server" Text='<%# Eval("Set_date") %>'></asp:Label>
                                            </EditItemTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="Label3" runat="server" Text='<%# Bind("Set_date") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="OrderNoCHIC" SortExpression="OrderNoCHIC" 
                                            Visible="False">
                                            <EditItemTemplate>
                                                <asp:Label ID="Label4" runat="server" Text='<%# Eval("OrderNoCHIC") %>'></asp:Label>
                                            </EditItemTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="Label4" runat="server" Text='<%# Bind("OrderNoCHIC") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Vacina" SortExpression="Vacina">
                                            <EditItemTemplate>
                                                <asp:DropDownList ID="ddlVacinas" runat="server" 
                                                    DataSourceID="VacinasSqlDataSource" DataTextField="ProdNomeAlt1"
                                                    OnSelectedIndexChanged="ddlVacinas_SelectedIndexChanged"
                                                    DataValueField="ProdNomeAlt1" SelectedValue='<%# Bind("Vacina") %>'
                                                    AutoPostBack="true">
                                                </asp:DropDownList>
                                                <asp:SqlDataSource ID="VacinasSqlDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:Apolo10ConnectionString %>" SelectCommand="select distinct P.ProdNomeAlt1 from PRODUTO P With(Nolock)
inner join PROD_GRUPO_SUBGRUPO PGS on P.ProdCodEstr = PGS.ProdCodEstr
inner join MARCA_PROD M With(Nolock) on P.MarcaProdCod = M.MarcaProdCod
where PGS.GrpProdCod = '041' and PGS.SubGrpProdCod in ('042','043')
and P.ProdNomeAlt1 &lt;&gt; ''
and 0 &lt; (select count(1) from PROD_UNID_MED U With(Nolock)
               where P.ProdCodEstr = U.ProdCodEstr
               and U.ProdUnidMedProduc = 'Sim')
order by 1"></asp:SqlDataSource>
                                            </EditItemTemplate>
                                            <FooterTemplate>
                                                <asp:DropDownList ID="ddlVacinas" runat="server" 
                                                    DataSourceID="VacinasSqlDataSource" DataTextField="ProdNomeAlt1" 
                                                    DataValueField="ProdNomeAlt1" OnSelectedIndexChanged="ddlVacinas_SelectedIndexChanged"
                                                    AutoPostBack="true">
                                                </asp:DropDownList>
                                                <asp:SqlDataSource ID="VacinasSqlDataSource" runat="server" 
                                                    ConnectionString="<%$ ConnectionStrings:Apolo10ConnectionString %>" SelectCommand="select distinct P.ProdNomeAlt1 from PRODUTO P With(Nolock)
inner join PROD_GRUPO_SUBGRUPO PGS on P.ProdCodEstr = PGS.ProdCodEstr
inner join MARCA_PROD M With(Nolock) on P.MarcaProdCod = M.MarcaProdCod
where PGS.GrpProdCod = '041' and PGS.SubGrpProdCod in ('042','043')
and P.ProdNomeAlt1 &lt;&gt; ''
and 0 &lt; (select count(1) from PROD_UNID_MED U With(Nolock)
               where P.ProdCodEstr = U.ProdCodEstr
               and U.ProdUnidMedProduc = 'Sim')
order by 1"></asp:SqlDataSource>
                                            </FooterTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="Label5" runat="server" Text='<%# Bind("Vacina") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Laboratorio" SortExpression="Laboratorio">
                                            <EditItemTemplate>
                                                <asp:Label ID="lblLaboratorio" runat="server" Text='<%# Bind("Laboratorio") %>'></asp:Label>
                                            </EditItemTemplate>
                                            <FooterTemplate>
                                                <asp:Label ID="lblLaboratorio" runat="server" Text='<%# Bind("Laboratorio") %>'></asp:Label>
                                            </FooterTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="Label6" runat="server" Text='<%# Bind("Laboratorio") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Partida" SortExpression="Partida">
                                            <EditItemTemplate>
                                                <asp:TextBox ID="txtPartida" runat="server" AutoPostBack="True" 
                                                    ontextchanged="txtPartida_TextChanged" Text='<%# Bind("Partida") %>' Width="64px"></asp:TextBox>
                                                <asp:MaskedEditExtender ID="txtPartida_MaskedEditExtender" runat="server" 
                                                    Century="2000" ClearMaskOnLostFocus="False" CultureAMPMPlaceholder="" 
                                                    CultureCurrencySymbolPlaceholder="" CultureDateFormat="" 
                                                    CultureDatePlaceholder="" CultureDecimalPlaceholder="" 
                                                    CultureThousandsPlaceholder="" CultureTimePlaceholder="" Enabled="True" 
                                                    Mask="999/99" PromptCharacter=" " TargetControlID="txtPartida">
                                                </asp:MaskedEditExtender>
                                                <br />
                                                <asp:Label ID="lblMsgPartida" runat="server" ForeColor="Red" 
                                                    style="font-weight: 700; color: #FFFF00;"></asp:Label>
                                            </EditItemTemplate>
                                            <FooterTemplate>
                                                <asp:TextBox ID="txtPartida" runat="server" AutoPostBack="True" Width="64px"></asp:TextBox>
                                                <asp:MaskedEditExtender ID="txtPartida_MaskedEditExtender" runat="server" 
                                                    Century="2000" ClearMaskOnLostFocus="False" CultureAMPMPlaceholder="" 
                                                    CultureCurrencySymbolPlaceholder="" CultureDateFormat="" 
                                                    CultureDatePlaceholder="" CultureDecimalPlaceholder="" 
                                                    CultureThousandsPlaceholder="" CultureTimePlaceholder="" Enabled="True" 
                                                    Mask="999/99" PromptCharacter=" " TargetControlID="txtPartida">
                                                </asp:MaskedEditExtender>
                                                <br />
                                                <asp:Label ID="lblMsgPartida" runat="server" ForeColor="Red"></asp:Label>
                                            </FooterTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="Label7" runat="server" Text='<%# Bind("Partida") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Fabricação" SortExpression="DataFabricacao">
                                            <EditItemTemplate>
                                                <asp:TextBox ID="txtDataFabricacao" runat="server" Text='<%# Bind("DataFabricacao") %>'
                                                    Width="70px" onblur="setDisableMask(this, '  /  /    ');"></asp:TextBox>
                                                <br />
                                                <asp:MaskedEditExtender ID="txtDataFabricacao_MaskedEditExtender" 
                                                    runat="server" Century="2000"
                                                    CultureName="pt-BR"
                                                    CultureThousandsPlaceholder="" CultureTimePlaceholder="" Enabled="True" 
                                                    Mask="99/99/9999" MaskType="Date" PromptCharacter=" "
                                                    UserDateFormat="DayMonthYear"
                                                    InputDirection="LeftToRight"
                                                    TargetControlID="txtDataFabricacao" ClearMaskOnLostFocus="true">
                                                </asp:MaskedEditExtender>
                                                <%--<asp:MaskedEditValidator ID="txtDataFabricacao_MaskedEditValidator" runat="server" 
                                                    InvalidValueMessage="Data Inválida!" 
                                                    ControlExtender="txtDataFabricacao_MaskedEditExtender" 
                                                    ControlToValidate="txtDataFabricacao"
                                                    PromptCharacter=" " 
                                                    style="font-size: xx-small; font-weight: 700; color: #FF0000"></asp:MaskedEditValidator>--%>
                                                <asp:CalendarExtender ID="txtDataFabricacao_CalendarExtender" runat="server" 
                                                    Enabled="True" Format="dd/MM/yyyy" 
                                                    TargetControlID="txtDataFabricacao" >
                                                </asp:CalendarExtender>
                                            </EditItemTemplate>
                                            <FooterTemplate>
                                                <asp:TextBox ID="txtDataFabricacao" runat="server"
                                                    Width="70px" onblur="setDisableMask(this, '  /  /    ');"></asp:TextBox>
                                                <br />
                                                <asp:MaskedEditExtender ID="txtDataFabricacao_MaskedEditExtender" 
                                                    runat="server" Century="2000"
                                                    CultureName="pt-BR"
                                                    CultureThousandsPlaceholder="" CultureTimePlaceholder="" Enabled="True" 
                                                    Mask="99/99/9999" MaskType="Date" PromptCharacter=" "
                                                    UserDateFormat="DayMonthYear"
                                                    InputDirection="LeftToRight"
                                                    TargetControlID="txtDataFabricacao" ClearMaskOnLostFocus="true">
                                                </asp:MaskedEditExtender>
                                                <asp:MaskedEditValidator ID="txtDataFabricacao_MaskedEditValidator" runat="server" 
                                                    InvalidValueMessage="Data Inválida!" 
                                                    ControlExtender="txtDataFabricacao_MaskedEditExtender" 
                                                    ControlToValidate="txtDataFabricacao"
                                                    PromptCharacter=" " 
                                                    style="font-size: xx-small; font-weight: 700; color: #FF0000"></asp:MaskedEditValidator>
                                                <asp:CalendarExtender ID="txtDataFabricacao_CalendarExtender" runat="server" 
                                                    Enabled="True" Format="dd/MM/yyyy" 
                                                    TargetControlID="txtDataFabricacao" >
                                                </asp:CalendarExtender>
                                            </FooterTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="lblDataFabricacao" runat="server" Text='<%# Bind("DataFabricacao", "{0:d}") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Validade" SortExpression="DataValidade">
                                            <EditItemTemplate>
                                                <asp:TextBox ID="txtDataValidade" runat="server" Text='<%# Bind("DataValidade", "{0:d}") %>'
                                                    Width="70px" onblur="setDisableMask(this, '  /  /    ');"></asp:TextBox>
                                                <br />
                                                <asp:MaskedEditExtender ID="txtDataValidade_MaskedEditExtender" 
                                                    runat="server" Century="2000"
                                                    CultureName="pt-BR"
                                                    CultureThousandsPlaceholder="" CultureTimePlaceholder="" Enabled="True" 
                                                    Mask="99/99/9999" MaskType="Date" PromptCharacter=" "
                                                    UserDateFormat="DayMonthYear"
                                                    InputDirection="LeftToRight"
                                                    TargetControlID="txtDataValidade" ClearMaskOnLostFocus="true">
                                                </asp:MaskedEditExtender>
                                                <asp:MaskedEditValidator ID="txtDataValidade_MaskedEditValidator" runat="server" 
                                                    InvalidValueMessage="Data Inválida!" 
                                                    ControlExtender="txtDataValidade_MaskedEditExtender" 
                                                    ControlToValidate="txtDataValidade"
                                                    PromptCharacter=" " 
                                                    style="font-size: xx-small; font-weight: 700; color: #FF0000"></asp:MaskedEditValidator>
                                                <asp:CalendarExtender ID="txtDataValidade_CalendarExtender" runat="server" 
                                                    Enabled="True" Format="dd/MM/yyyy" 
                                                    TargetControlID="txtDataValidade" >
                                                </asp:CalendarExtender>
                                            </EditItemTemplate>
                                            <FooterTemplate>
                                                <asp:TextBox ID="txtDataValidade" runat="server"
                                                    Width="70px" onblur="setDisableMask(this, '  /  /    ');"></asp:TextBox>
                                                <br />
                                                <asp:MaskedEditExtender ID="txtDataValidade_MaskedEditExtender" 
                                                    runat="server" Century="2000"
                                                    CultureName="pt-BR"
                                                    CultureThousandsPlaceholder="" CultureTimePlaceholder="" Enabled="True" 
                                                    Mask="99/99/9999" MaskType="Date" PromptCharacter=" "
                                                    UserDateFormat="DayMonthYear"
                                                    InputDirection="LeftToRight"
                                                    TargetControlID="txtDataValidade" ClearMaskOnLostFocus="true">
                                                </asp:MaskedEditExtender>
                                                <asp:MaskedEditValidator ID="txtDataValidade_MaskedEditValidator" runat="server" 
                                                    InvalidValueMessage="Data Inválida!" 
                                                    ControlExtender="txtDataValidade_MaskedEditExtender" 
                                                    ControlToValidate="txtDataValidade"
                                                    PromptCharacter=" " 
                                                    style="font-size: xx-small; font-weight: 700; color: #FF0000"></asp:MaskedEditValidator>
                                                <asp:CalendarExtender ID="txtDataValidade_CalendarExtender" runat="server" 
                                                    Enabled="True" Format="dd/MM/yyyy" 
                                                    TargetControlID="txtDataValidade" >
                                                </asp:CalendarExtender>
                                            </FooterTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="lblDataValidade" runat="server" Text='<%# Bind("DataValidade", "{0:d}") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Qtde. Ampolas Utilizadas" SortExpression="QtdeAmpolas">
                                            <EditItemTemplate>
                                                <asp:TextBox ID="txtQtdeAmpolas" runat="server" AutoPostBack="True" onkeyup="mascara(this, mnum);" ontextchanged="txtQtdeAmpolas_TextChanged" Text='<%# Bind("QtdeAmpolas") %>' Width="45px"></asp:TextBox>
                                                <br />
                                            </EditItemTemplate>
                                            <FooterTemplate>
                                                <asp:TextBox ID="txtQtdeAmpolas" runat="server" AutoPostBack="True" onkeyup="mascara(this, mnum);" ontextchanged="txtQtdeAmpolas_TextChanged" Text='<%# Bind("QtdeAmpolas") %>' Width="45px"></asp:TextBox>
                                                <br />
                                                <asp:Label ID="lblMensagemQtdeAmpolas" runat="server" Font-Bold="True" ForeColor="Red"></asp:Label>
                                            </FooterTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="lblQtdeAmpolas" runat="server" Text='<%# Bind("QtdeAmpolas", "{0:N0}") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Qtde. Doses Por Ampola" SortExpression="QtdeDosesPorAmpola">
                                            <EditItemTemplate>
                                                <asp:DropDownList ID="ddlQtdeDosesPorAmpola" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlQtdeDosesPorAmpola_SelectedIndexChanged">
                                                </asp:DropDownList>
                                            </EditItemTemplate>
                                            <FooterTemplate>
                                                <asp:DropDownList ID="ddlQtdeDosesPorAmpola" runat="server" SelectedValue='<%# Bind("QtdeDosesPorAmpola", "Número - {0:N0}") %>' AutoPostBack="True" OnSelectedIndexChanged="ddlQtdeDosesPorAmpola_SelectedIndexChanged">
                                                </asp:DropDownList>
                                            </FooterTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="lblQtdeDosesPorAmpola" runat="server" Text='<%# Bind("QtdeDosesPorAmpola", "{0:N0}") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Qtde. Doses Total" SortExpression="QtdeDosesTotal">
                                            <EditItemTemplate>
                                                <asp:Label ID="lblQtdeDosesTotal" runat="server" Text='<%# Bind("QtdeDosesTotal", "{0:N0}") %>' style="font-weight: 700"></asp:Label>
                                            </EditItemTemplate>
                                            <FooterTemplate>
                                                <asp:Label ID="lblQtdeDosesTotal" runat="server" Text='<%# Bind("QtdeDosesTotal", "{0:N0}") %>'></asp:Label>
                                            </FooterTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="lblQtdeDosesTotal" runat="server" Text='<%# Bind("QtdeDosesTotal", "{0:N0}") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Observações" SortExpression="Observacao">
                                            <EditItemTemplate>
                                                <asp:TextBox ID="txtObservacoes" runat="server" Rows="5" Text='<%# Bind("Observacao") %>' TextMode="MultiLine"></asp:TextBox>
                                                <br />
                                                <asp:Label ID="lblErroObservacoes" runat="server" Font-Bold="True" ForeColor="Red" Visible="False"></asp:Label>
                                            </EditItemTemplate>
                                            <FooterTemplate>
                                                <asp:TextBox ID="txtObservacoes" runat="server" Rows="5" Text='<%# Bind("Observacao") %>' TextMode="MultiLine"></asp:TextBox>
                                                <br />
                                                <asp:Label ID="lblErroObservacoes" runat="server" Font-Bold="True" ForeColor="Red" Visible="False"></asp:Label>
                                            </FooterTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="lblObservacao" runat="server" Text='<%# Bind("Observacao") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                    <EditRowStyle BackColor="#2461BF" />
                                    <FooterStyle BackColor="#00CCFF" Font-Bold="True" ForeColor="White" />
                                    <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                    <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                                    <RowStyle BackColor="#EFF3FB" />
                                    <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                                    <SortedAscendingCellStyle BackColor="#F5F7FB" />
                                    <SortedAscendingHeaderStyle BackColor="#6D95E1" />
                                    <SortedDescendingCellStyle BackColor="#E9EBEF" />
                                    <SortedDescendingHeaderStyle BackColor="#4870BE" />
                                </asp:GridView>
                                <asp:SqlDataSource ID="SqlDataSource3" runat="server" 
                                    ConnectionString="<%$ ConnectionStrings:HLBAPPConnectionString %>" 
                                    DeleteCommand="DELETE FROM [HATCHERY_ORDER_VACC_DATA] WHERE [ID] = @ID" 
                                    InsertCommand="INSERT INTO [HATCHERY_ORDER_VACC_DATA] ([Hatch_Loc], [Set_date], [OrderNoCHIC], [Vacina], [Laboratorio], [Partida], [DataFabricacao], [DataValidade]) VALUES (@Hatch_Loc, @Set_date, @OrderNoCHIC, @Vacina, @Laboratorio, @Partida, @DataFabricacao, @DataValidade)" 
                                    SelectCommand="SELECT *, ISNULL(QtdeAmpolas,0) * ISNULL(QtdeDosesPorAmpola,0) QtdeDosesTotal FROM [HATCHERY_ORDER_VACC_DATA] WHERE (([Hatch_Loc] = @Hatch_Loc) AND ([Set_date] = @Set_date) AND ([OrderNoCHIC] = @OrderNoCHIC) and (Variety = @Linhagem or Variety is null)) or (OrderNoCHIC = 'Branco')" 
                                    UpdateCommand="UPDATE [HATCHERY_ORDER_VACC_DATA] SET [Hatch_Loc] = @Hatch_Loc, [Set_date] = @Set_date, [OrderNoCHIC] = @OrderNoCHIC, [Vacina] = @Vacina, [Laboratorio] = @Laboratorio, [Partida] = @Partida,
[DataFabricacao] = convert(datetime, @DataFabricacao,103), [DataValidade] = convert(datetime, @DataValidade,103), Variety = @Linhagem, QtdeAmpolas = @QtdeAmpolas, QtdeDosesPorAmpola = @QtdeDosesPorAmpola, Observacao = @Observacao
WHERE [ID] = @ID">
                                    <DeleteParameters>
                                        <asp:Parameter Name="ID" Type="Int32" />
                                    </DeleteParameters>
                                    <InsertParameters>
                                        <asp:Parameter Name="Hatch_Loc" Type="String" />
                                        <asp:Parameter Name="Set_date" Type="DateTime" />
                                        <asp:Parameter Name="OrderNoCHIC" Type="String" />
                                        <asp:Parameter Name="Vacina" Type="String" />
                                        <asp:Parameter Name="Laboratorio" Type="String" />
                                        <asp:Parameter Name="Partida" Type="String" />
                                        <asp:Parameter Name="DataFabricacao" />
                                        <asp:Parameter Name="DataValidade" />
                                    </InsertParameters>
                                    <SelectParameters>
                                        <asp:ControlParameter ControlID="ddlIncubatorios" Name="Hatch_Loc" 
                                            PropertyName="SelectedValue" Type="String" />
                                        <asp:ControlParameter ControlID="Calendar1" Name="Set_date" 
                                            PropertyName="SelectedDate" Type="DateTime" />
                                        <asp:SessionParameter Name="OrderNoCHIC" SessionField="numPedidoSelecionado" 
                                            Type="String" />
                                        <asp:SessionParameter Name="Linhagem" SessionField="linhagemSelecionada" />
                                    </SelectParameters>
                                    <UpdateParameters>
                                        <asp:Parameter Name="Hatch_Loc" Type="String" />
                                        <asp:Parameter Name="Set_date" Type="DateTime" />
                                        <asp:Parameter Name="OrderNoCHIC" Type="String" />
                                        <asp:Parameter Name="Vacina" Type="String" />
                                        <asp:Parameter Name="Laboratorio" Type="String" />
                                        <asp:Parameter Name="Partida" Type="String" />
                                        <asp:Parameter Name="DataFabricacao" />
                                        <asp:Parameter Name="DataValidade" />
                                        <asp:Parameter Name="Linhagem" />
                                        <asp:Parameter Name="QtdeAmpolas" />
                                        <asp:Parameter Name="QtdeDosesPorAmpola" />
                                        <asp:Parameter Name="Observacao" />
                                        <asp:Parameter Name="ID" Type="Int32" />
                                    </UpdateParameters>
                                </asp:SqlDataSource>
                                <asp:Label ID="lblMsgVerificaVacinas" runat="server" Style="font-weight: 700; color: #FF3300" Visible="False"></asp:Label>
                                <br />
                                <br />
                                </asp:Panel>
                                <asp:Panel ID="pnlInformacoesAdicionais" runat="server" Visible="False">    
                                <table style="width:76%;">
                                    <tr>
                                        <td>
                                            <asp:Label ID="lblObservacao" runat="server" CssClass="style61" 
                                                Text="Observação:"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtObservacao" runat="server" TextMode="MultiLine" 
                                                Height="67px" Width="569px"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <br />
                                            <asp:Label ID="lblRepInspFinal" runat="server" CssClass="style61" 
                                                Text="Responsável p/ Inspeção Final:"></asp:Label>
                                            <br />
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtRepInspFinal" runat="server" Width="560px"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <br />
                                            <asp:Label ID="lblRepExpedicaoCarga" runat="server" CssClass="style61" 
                                                Text="Responsável p/ Expedição da Carga:"></asp:Label>
                                            <br />
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtRepExpedicaoCarga" runat="server" Width="560px"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <br />
                                            <asp:Label ID="lblRespLiberacaoVeiculo" runat="server" CssClass="style61" 
                                                Text="Responsável p/ Liberação do Veículo:"></asp:Label>
                                            <br />
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtRespLiberacaoVeiculo" runat="server" Width="560px"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <br />
                                            <asp:Label ID="lblRNC" runat="server" CssClass="style61" 
                                                Text="R.N.C.:"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:DropDownList ID="ddlRNC" runat="server" AutoPostBack="True" 
                                                onselectedindexchanged="ddlRNC_SelectedIndexChanged">
                                                <asp:ListItem>(Nenhum)</asp:ListItem>
                                                <asp:ListItem>ATRASO DE LIBERAÇÃO</asp:ListItem>
                                                <asp:ListItem>EFICIÊNCIA DE VACINAÇÃO</asp:ListItem>
                                                <asp:ListItem>ERRO DE CONTAGEM</asp:ListItem>
                                                <asp:ListItem>ERRO DE SELEÇÃO</asp:ListItem>
                                                <asp:ListItem>ERRO DE SEXAGEM</asp:ListItem>
                                                <asp:ListItem>TEMPO MÁXIMO DE PERMANÊNCIA</asp:ListItem>
                                                <asp:ListItem>TRANSPORTE</asp:ListItem>
                                            </asp:DropDownList>
                                            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                            <asp:Label ID="lblNumeroRNC" runat="server" CssClass="style61" 
                                                Text="Nº R.N.C.:" Visible="False"></asp:Label>
                                            &nbsp;
                                            <asp:TextBox ID="txtNumeroRNC" runat="server" Visible="False"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <br />
                                            <asp:Label ID="lblDisposicaoRNC" runat="server" CssClass="style61" 
                                                Text="Disposição da R.N.C.:"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:DropDownList ID="ddlDisposicaoRNC" runat="server">
                                                <asp:ListItem>(Nenhum)</asp:ListItem>
                                                <asp:ListItem>ADIANTAMENTO DE NASCIMENTO</asp:ListItem>
                                                <asp:ListItem>ALTERAÇÃO DO CLIENTE PROGRAMADO</asp:ListItem>
                                                <asp:ListItem>ALTERAÇÃO DO HORÁRIO DE EXPEDIÇÃO</asp:ListItem>
                                                <asp:ListItem>ATRASO DA TRANSPORTADORA</asp:ListItem>
                                                <asp:ListItem>ATRASO DE NASCIMENTO</asp:ListItem>
                                                <asp:ListItem>ATRASO NO FATURAMENTO</asp:ListItem>
                                                <asp:ListItem>ERRO NO PLANEJAMENTO ESTRATÉGICO</asp:ListItem>
                                                <asp:ListItem>FALHA NA CONTAGEM DA MÁQUINA DE VACINAÇÃO</asp:ListItem>
                                                <asp:ListItem>FALTA DE ATENÇÃO DOS COLABORADORES</asp:ListItem>
                                                <asp:ListItem>INTERVALO ETÁRIO ELEVADO</asp:ListItem>
                                                <asp:ListItem>MISTURA DE LOTES</asp:ListItem>
                                                <asp:ListItem>MISTURA DE TIPOS DE PINTOS DIFERENTES</asp:ListItem>
                                                <asp:ListItem>NOVATAS EM TREINAMENTO</asp:ListItem>
                                                <asp:ListItem>PROBLEMA COM CAMINHÃO TRANSPORTADOR</asp:ListItem>
                                                <asp:ListItem>QTDE ALÉM DA CAPACIDADE DE TRABALHO</asp:ListItem>
                                                <asp:ListItem>QUEDA NA ECLOSÃO PREVISTA</asp:ListItem>
                                                <asp:ListItem>REGULAGEM DA MÁQUINA DE VACINAÇÃO</asp:ListItem>
                                                <asp:ListItem>SOLICITAÇÃO DE ALTERAÇÃO PELO CLIENTE</asp:ListItem>
                                                <asp:ListItem>USO DE PINTOS DO DIA ANTERIOR (SOBRA)</asp:ListItem>
                                                <asp:ListItem>USO DE PINTOS DO DIA SEGUINTE (PUXADO)</asp:ListItem>
                                                <asp:ListItem>UTILIZAÇÃO PINTOS 2ª RETIRADA</asp:ListItem>
                                                <asp:ListItem>UTILIZAÇÃO PINTOS DA 1ª E 2ª RETIRADAS</asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                </table>
                                </asp:Panel>
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
