<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ControleQualidadeCargasPintos.aspx.cs"
    Inherits="MvcAppHyLinedoBrasil.WebForms.ControleQualidadeCargasPintos" %>

<%@ Register assembly="CrystalDecisions.Web, Version=13.0.4000.0, Culture=neutral, PublicKeyToken=692fbea5521e1304" namespace="CrystalDecisions.Web" tagprefix="CR" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Composição dos Lotes dos Clientes</title>
    <%--<link href="../Content/icons/logo_hyline.ico" rel="Shortcut Icon" type="text/css" />--%>
    <style type="text/css">
        .style14
        {
            width: 1278px;
            height: 50px;
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
        .style58
        {
            height: 50px;
        }
        .style63
        {
            width: 216px;
        }
        .style66
        {
            font-family: Verdana;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePartialRendering="false">
    </asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div style="text-align: center;" class="panel">
                <asp:Panel ID="Panel2" runat="server" Width="1002px" HorizontalAlign="Center">
                    <table style="width: 104%;">
                        <tr>
                            <td rowspan="3" class="style63">
                                <a href="../Home/Index">
                                    <%--<img alt="" class="style23" src="../Content/images/logo.png" border="0" />--%>
                                    <asp:Image ID="Image2" runat="server" ImageUrl="~/Content/images/Logo_EW.png" /></a>
                                <br />
                                <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/WebForms/EFD_PisCofins.aspx"
                                    Style="font-size: small; font-family: Tahoma;">Voltar</asp:HyperLink>
                            </td>
                        </tr>
                        <tr>
                            <td class="style14">
                                <asp:Label ID="Label5" runat="server" Font-Bold="True" Font-Size="XX-Large" Font-Underline="False"
                                    Text="Composição dos Lotes dos Clientes" 
                                    style="font-family: Verdana"></asp:Label>
                            </td>
                            <td class="style58">
                                <asp:Label ID="Label1" runat="server" Text="Selecione a Geração:"  Style="font-size: xx-small;
                                    font-weight: 700; font-family: Verdana;"></asp:Label>
                                <br />
                                <asp:DropDownList ID="ddlIncubatorios" runat="server" AutoPostBack="True">
                                    <asp:ListItem Value="HYBRBRPG">Avós</asp:ListItem>
                                    <asp:ListItem Value="HYBRBRBR">Matrizes</asp:ListItem>
                                </asp:DropDownList>
                                <br />
                                <asp:Label ID="Label4" runat="server" Text="Seleciona a Data da Nascimento:" Style="font-size: xx-small;
                                    font-weight: 700; font-family: Verdana;"></asp:Label>
                                <asp:Calendar ID="Calendar1" runat="server" Font-Size="XX-Small" Height="32px" Width="233px"
                                    OnSelectionChanged="Calendar1_SelectionChanged" 
                                    style="font-family: Verdana" SelectedDate="2013-09-02">
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
                <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1"
                    OnDataBinding="UpdateProgress1_DataBinding" OnDisposed="UpdateProgress1_Disposed"
                    OnLoad="UpdateProgress1_Load" OnPreRender="UpdateProgress1_PreRender" OnUnload="UpdateProgress1_Unload">
                    <ProgressTemplate>
                        <span class="style66"><strong>CARREGANDO...</strong>&nbsp;&nbsp; </span>
                        <br class="style66" />
                        <asp:Image ID="Image1" runat="server" ImageUrl="~/Content/images/ajax-loading.gif" />
                    </ProgressTemplate>
                </asp:UpdateProgress>
                <asp:Panel ID="Panel1" runat="server" Width="1002px" HorizontalAlign="Center" Style="text-align: left">
                    <table style="width: 100%;">
                        <tr>
                            <td class="style64">
                                <asp:Label ID="Label3" runat="server" Font-Bold="True" Font-Size="Small" Font-Underline="False"
                                    Text="LISTA DE CLIENTES X PEDIDOS CHIC" CssClass="style66"></asp:Label>
                                <table align="center">
                                    <tr>
                                        <td class="style57">
                                            <asp:Label ID="Label8" runat="server" CssClass="style61" Font-Size="XX-Small" 
                                                Text="Localizar:" style="font-family: Verdana; font-weight: 700"></asp:Label>
                                        </td>
                                        <td class="style22">
                                            &nbsp;
                                            <asp:TextBox ID="TextBox6" runat="server" CssClass="style24" Width="193px" 
                                                Height="15px" style="font-family: Verdana"></asp:TextBox>
                                        </td>
                                        <td class="style62">
                                            <asp:DropDownList ID="DropDownList1" runat="server" Height="21px" Style="font-weight: 700; font-family: Verdana; font-size: x-small;"
                                                Width="124px" CssClass="style60">
                                                <asp:ListItem>Nome</asp:ListItem>
                                                <asp:ListItem Value="UF">UF</asp:ListItem>
                                            </asp:DropDownList>
                                            &nbsp;
                                        </td>
                                        <td class="style39">
                                            <asp:Button ID="btn_Pesquisar" runat="server" Font-Bold="True" Font-Size="XX-Small"
                                                Height="21px" OnClick="Button1_Click" Text="Pesquisar" Width="118px" 
                                                CssClass="style60" style="font-family: Verdana" />
                                            &nbsp;
                                        </td>
                                    </tr>
                                </table>
                                <br />
                                <asp:Label ID="Label9" runat="server" 
                                    style="color: #FF3300; font-family: Verdana; font-weight: 700; font-size: small" 
                                    Text="* PRIMEIRO SELECIONE A LINHA COM O ÍCONE DA SETA E DEPOIS CLIQUE NOS ÍCONES PARA GERAR O RELATÓRIO!"></asp:Label>
                                <br />
                                <asp:ImageButton ID="ImageButton1" runat="server" CausesValidation="False" 
                                    CommandName="Select" ImageUrl="~/Content/images/kjots.png" 
                                    onclick="ImageButton1_Click" Text="Select" Width="16px" />
                                <asp:Label ID="Label10" runat="server" 
                                    style="font-family: Verdana; font-size: xx-small; font-weight: 700;" 
                                    Text="Controle de Cargas de Pintos - Normal"></asp:Label>
                                <br />
                                <asp:ImageButton ID="ImageButton2" runat="server" CausesValidation="False" 
                                    CommandName="Select" ImageUrl="~/Content/images/aviso.png" 
                                    onclick="ImageButton2_Click" Text="Select" />
                                <asp:Label ID="Label11" runat="server" 
                                    style="font-family: Verdana; font-size: xx-small; font-weight: 700" 
                                    Text="Controle de Cargas de Pintos com Lotes Irmãos"></asp:Label>
                                <br />
                                <asp:ImageButton ID="imgFichaDescrLote" runat="server" CausesValidation="False" 
                                    CommandName="Select" ImageUrl="~/Content/images/kjots.png" 
                                    Text="Select" onclick="imgFichaDescrLote_Click" />
                                <asp:Label ID="lblFichaDescrLote" runat="server" 
                                    style="font-family: Verdana; font-size: xx-small; font-weight: 700" 
                                    Text="Ficha de Descrição do Lote"></asp:Label>
                                <br />
                                <asp:ImageButton ID="imgDadosLogger" runat="server" CausesValidation="False" 
                                    CommandName="Select" ImageUrl="~/Content/icons/excel.png" 
                                    onclick="imgDadosLogger_Click" Text="Select" />
                                <asp:Label ID="lblDadosLogger" runat="server" 
                                    style="font-family: Verdana; font-size: xx-small; font-weight: 700" 
                                    Text="Dados do Logger"></asp:Label>
                                &nbsp;<asp:LinkButton ID="lkbDownload" runat="server" OnClick="lkbDownload_Click" 
                                    Visible="False">Download</asp:LinkButton>
                            </td>
                        </tr>
                        <tr>
                            <td class="style64">
                                <asp:GridView ID="GridView3" runat="server" AllowSorting="True" CellPadding="4" DataSourceID="EggInvDataSource"
                                    ForeColor="#333333" GridLines="None" Style="font-size: xx-small; text-align: center; font-family: Tahoma;"
                                    Width="990px" AutoGenerateColumns="False" PageSize="5" 
                                    OnSelectedIndexChanged="GridView3_SelectedIndexChanged">
                                    <AlternatingRowStyle BackColor="White" />
                                    <Columns>
                                        <asp:CommandField ButtonType="Image" 
                                            SelectImageUrl="~/Content/images/Next_16x16.gif" SelectText="Selecionar Linha" 
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
  o.custno &quot;Cód.Cliente&quot;,
  c.name &quot;Nome do Cliente&quot;,
  c.state &quot;UF&quot;,
  o.orderno &quot;Pedido&quot;
from 
  order_data o, 
  ch_cust c,
  ch_orders co,
  ch_salesman s
where
  o.orderno = co.orderno and
  co.salesrep = s.sl_code(+) and 
  co.site_key = s.site_key(+) and
  co.site_key = c.site_key and
  o.custno = c.custno and
  InStr(:Empresa,s.inv_comp) &gt; 0 and
  (co.site_key = :Geracao) and 
  o.set_date + 21 = :Data
  and (
         (:Pesquisa ='0')
         or 
         (:Campo = 'Nome' and c.name like '%' ||UPPER(:Pesquisa)||'%')
         or
         (:Campo = 'UF' and c.state like '%'||UPPER(:Pesquisa)||'%')
        )
group by
  o.custno,
  c.name,
  c.state,
  o.orderno,
  s.inv_comp
Order by
  c.name">
                                    <SelectParameters>
                                        <asp:SessionParameter Name="Empresa" SessionField="empresa" />
                                        <asp:ControlParameter ControlID="ddlIncubatorios" Name="Geracao" 
                                            PropertyName="SelectedValue" />
                                        <asp:ControlParameter ControlID="Calendar1" DbType="Date" DefaultValue="" Name="Data"
                                            PropertyName="SelectedDate" />
                                        <asp:ControlParameter ControlID="TextBox6" Name="Pesquisa" 
                                            PropertyName="Text" />
                                        <asp:ControlParameter ControlID="DropDownList1" Name="Campo" 
                                            PropertyName="SelectedValue" />
                                    </SelectParameters>
                                </asp:SqlDataSource>
                                <asp:SqlDataSource ID="PedidosAniPlan" runat="server" ConnectionString="<%$ ConnectionStrings:HLBAPPConnectionString %>" SelectCommand="select
	P.Empresa,
	P.CodigoCliente [Cód.Cliente],
	P.NomeCliente [Nome do Cliente],
	P.uf UF,
	P.CHICNum Pedido
from
	VU_Pedidos_Vendas_CHIC_Matrizes P
where
	CHARINDEX(P.Empresa,@Empresa) &gt; 0 and
                   P.QtdeVendida &gt; 0 and
	P.DataIncubacao+21 = @Data and
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
	P.CHICNum

Union

select
	P.Empresa,
	P.CodigoCliente [Cód.Cliente],
	P.NomeCliente [Nome do Cliente],
	P.uf UF,
	P.CHICNumReposicao Pedido
from
	VU_Pedidos_Vendas_CHIC_Matrizes P
where
	CHARINDEX(P.Empresa,@Empresa) &gt; 0 and
                   P.QtdeReposicao &gt; 0 and
	P.DataIncubacao+21 = @Data and
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
	P.CHICNumReposicao">
                                    <SelectParameters>
                                        <asp:SessionParameter Name="Empresa" SessionField="empresa" />
                                        <asp:ControlParameter ControlID="Calendar1" DbType="Date" DefaultValue="" Name="Data" PropertyName="SelectedDate" />
                                        <asp:ControlParameter ControlID="TextBox6" Name="Pesquisa" PropertyName="Text" />
                                        <asp:ControlParameter ControlID="DropDownList1" Name="Campo" PropertyName="SelectedValue" />
                                    </SelectParameters>
                                </asp:SqlDataSource>
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
