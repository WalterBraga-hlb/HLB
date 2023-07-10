<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Incubacao_Teste.aspx.cs" Inherits="MvcAppHyLinedoBrasil.WebForms.Incubacao_Teste" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Incubação</title>
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
        .upper-case
        {
            text-transform: uppercase;
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
                                <br />
                                <br />
                                <br />
                                <asp:LinkButton ID="lbMostrarInv" runat="server" onclick="lbMostrarInv_Click" 
                                    Visible="False">Mostrar Inventário</asp:LinkButton>
                                <br />
                                <asp:LinkButton ID="lbEsconderInv" runat="server" onclick="lbEsconderInv_Click" 
                                    Visible="False">Esconder Inventário</asp:LinkButton>
                            </td>
                        </tr>
                        <tr>
                            <td class="style14">
                                <asp:Label ID="Label5" runat="server" Font-Bold="True" Font-Size="XX-Large" Font-Underline="False"
                                    Text="INCUBAÇÃO"></asp:Label>
                                <br />
                                <br />
                                <asp:Label ID="lblMensagem3" runat="server" 
                                    Style="font-weight: 700; color: #FF3300" Visible="False"></asp:Label>
                                <br />
                                <br />
                                <asp:Button ID="btnRecalculaEstimativa" runat="server" 
                                    onclick="btnRecalculaEstimativa_Click" 
                                    Text="Calcula Media Ponderada Estimativa" Visible="False" />
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
                                
                                <asp:Button ID="btnEstoqueFuturo" runat="server" Font-Bold="True" 
                                    Font-Overline="False" Font-Strikeout="False" onclick="btnEstoqueFuturo_Click" 
                                    Text="Inserir Estoque Futuro" Visible="False" />
                                <br />
                                
                                <br />
                                <asp:Label ID="Label3" runat="server" Font-Bold="True" Font-Size="Small" Font-Underline="False"
                                    Text="INVENTÁRIO DE OVOS"></asp:Label>
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
                                                <asp:ListItem Value="LN">Linhagem</asp:ListItem>
                                                <asp:ListItem Value="DP">Data de Produção</asp:ListItem>
                                                <asp:ListItem Value="LT">Lote</asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                        <td class="style62">
                                            <asp:DropDownList ID="ddlClassOvos" runat="server" Height="21px" Style="font-weight: 700"
                                                Width="124px" CssClass="style60">
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
                                        <td align="left">
                                            <asp:RadioButtonList ID="rbListaExport" runat="server" AutoPostBack="True" 
                                                onselectedindexchanged="rbListaExport_SelectedIndexChanged" 
                                                style="font-size: xx-small;">
                                                <asp:ListItem>PDF</asp:ListItem>
                                                <asp:ListItem>Excel</asp:ListItem>
                                            </asp:RadioButtonList>
                                        </td>
                                        <td class="style72">
                                            <asp:HyperLink ID="hlExport" runat="server" Font-Bold="True" ForeColor="Black" 
                                                NavigateUrl="~/WebForms/MapaIncubacao.aspx" 
                                                style="font-size: medium; font-weight: 700" Target="_blank">Exportar</asp:HyperLink>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="2">
                                            <asp:Button ID="btnImportarEstqApolo" runat="server" 
                                                Text="Importa p/ Estoque APOLO" Font-Bold="True" Font-Overline="False" 
                                                Font-Strikeout="False" onclick="btnImportarEstqApolo_Click" 
                                                Visible="False" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="2">
                                            <asp:Button ID="btnDeletaImportacaoApolo" runat="server" 
                                                Text="Deleta Importação do Estoque APOLO" Font-Bold="True" Font-Overline="False" 
                                                Font-Strikeout="False" onclick="btnDeletaImportacaoApolo_Click" 
                                                Visible="False" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="font-weight: 700" colspan="2">
                                            <asp:Label ID="lblPerguntaConfirmaExclusaoImportacao" runat="server" 
                                                Text="CONFIRMA EXCLUSÃO DA IMPORTAÇÃO NO APOLO???" 
                                                style="font-weight: 700; color: #FF0000" Visible="False"></asp:Label>
                                            <br />
                                            <asp:LinkButton ID="lbtnSim" runat="server" onclick="lbtnSim_Click" 
                                                style="font-weight: 700; color: #FF9900" Visible="False">SIM</asp:LinkButton>
                                            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                            <asp:LinkButton ID="lbtnNao" runat="server" onclick="lbtnNao_Click" 
                                                style="font-weight: 700; color: #33CC33" Visible="False">NÃO</asp:LinkButton>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td style="text-align: center">
                                &nbsp;</td>
                        </tr>
                        <tr>
                            <td class="style64">
                                <asp:GridView ID="GridView3" runat="server" AllowSorting="True" CellPadding="4" DataSourceID="EggInvDataSource"
                                    ForeColor="#333333" GridLines="None" Style="font-size: xx-small; text-align: center;"
                                    Width="756px" AutoGenerateColumns="False" OnSelectedIndexChanged="GridView3_SelectedIndexChanged"
                                    AllowPaging="True">
                                    <AlternatingRowStyle BackColor="White" />
                                    <Columns>
                                        <asp:CommandField ButtonType="Image" SelectImageUrl="~/Content/images/Next_16x16.gif"
                                            ShowSelectButton="True" />
                                        <asp:BoundField DataField="Granja / Núcleo" HeaderText="Granja / Núcleo" SortExpression="Granja / Núcleo">
                                        </asp:BoundField>
                                        <asp:BoundField DataField="LOTE" HeaderText="LOTE" SortExpression="LOTE" />
                                        <asp:BoundField DataField="Idade Lote" HeaderText="Idade Lote" 
                                            SortExpression="Idade Lote" />
                                        <asp:BoundField DataField="LINHAGEM" HeaderText="LINHAGEM" SortExpression="LINHAGEM" />
                                        <asp:BoundField DataField="Data Prd." HeaderText="Data Prd." SortExpression="Data Prd."
                                            DataFormatString="{0:d}" />
                                        <asp:BoundField DataField="Idade do Ovo" HeaderText="Idade do Ovo" SortExpression="Idade do Ovo" />
                                        <asp:BoundField DataField="Qtde.Ovos" HeaderText="Qtde.Ovos" 
                                            SortExpression="Qtde.Ovos" DataFormatString="{0:N0}" />
                                        <asp:BoundField DataField="Class. Ovos" SortExpression="Class. Ovos" />
                                        <asp:BoundField DataField="% Eclo. Real D4" HeaderText="% Eclo. Real D4"
                                            SortExpression="% Eclo. Real D4" DataFormatString="{0:N2}" />
                                        <asp:BoundField DataField="% Eclo. Real Antepenúltima" HeaderText="% Eclo. Real Antepenúltima"
                                            SortExpression="% Eclo. Real Antepenúltima" DataFormatString="{0:N2}" />
                                        <asp:BoundField DataField="% Eclo. Real Penúltima" DataFormatString="{0:N2}" 
                                            HeaderText="% Eclo. Real Penúltima" SortExpression="% Eclo. Real Penúltima" />
                                        <asp:BoundField DataField="% Eclo. Real Última" DataFormatString="{0:N2}" 
                                            HeaderText="% Eclo. Real Última" SortExpression="% Eclo. Real Última" />
                                        <asp:BoundField DataField="Última Infertilidade 10 Dias" 
                                            DataFormatString="{0:N2}" HeaderText="Última Infertilidade 10 Dias" 
                                            SortExpression="Última Infertilidade 10 Dias" />
                                        <asp:BoundField DataField="Comentarios" HeaderText="Comentarios" 
                                            SortExpression="Comentarios" />
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
    CLLA.Local [Class. Ovos],
	CLLA.Nucleo [Granja / Núcleo],
	CLLA.LoteCompleto [Lote],
	CLLA.IdadeLote [Idade Lote],
	CLLA.Linhagem [Linhagem],
	CLLA.DataProducao [Data Prd.],
	Datediff(dd,CLLA.DataProducao,@Data) [Idade do Ovo],
    Convert(int,CLLA.Qtde) [Qtde.Ovos],
	0 [% Eclo. Real D4], 0 [% Eclo. Real Antepenúltima], 0 [% Eclo. Real Penúltima], 0 [% Eclo. Real Última],
	0 [Última Infertilidade 10 Dias],
    FD.Comentarios
from
	CTRL_LOTE_LOC_ARMAZ_WEB CLLA 
left join
	FLOCK_DATA FD on
		CLLA.LoteCompleto = FD.Flock_ID and CLLA.DataProducao = FD.Trx_Date
where
	(
        (@Pesquisa ='0')
        or 
        (@Campo = 'LN' and CLLA.Linhagem like '%' + @Pesquisa + '%')
        or
		(@Campo = 'DP' and Format(CLLA.DataProducao,'d',@Language) like '%' + @Pesquisa + '%')
        or
        (@Campo = 'LT' and CLLA.LoteCompleto like '%' + @Pesquisa + '%')
    ) 
	and 
	CLLA.Qtde &gt; 0 
	and 0 &lt; (select COUNT(1) from TIPO_CLASSFICACAO_OVO TCO
		where (
			(TCO.Unidade = @Incubatorio and TCO.CodigoTipo = CLLA.Local)
			or
			(TCO.CodigoTipo+'-'+@Incubatorio = CLLA.Local)
		)
		and TCO.AproveitamentoOvo = 'Incubável')
	and 0 &lt; (select COUNT(1) from LayoutDiarioExpedicaos LD
		where 
		(LD.TipoDEO = 'Classificação de Ovos' or LD.TipoDEO = 'Transf. Ovos Classificados'
					or (LD.TipoDEO = 'Transf. Ovos Incubáveis' and LD.Granja = 'PL' and LD.Incubatorio = 'NM'))
				and LD.Incubatorio = @Incubatorio
				and LD.TipoOvo = CLLA.Local
				and LD.LoteCompleto = CLLA.LoteCompleto
				and LD.DataProducao = CLLA.DataProducao)
	and CLLA.LoteCompleto not in ('VARIOS')
    --and ((CLLA.Local = @Incubatorio and 'NM' &lt;&gt; @Incubatorio)
    --   or ('NM' = @Incubatorio and CLLA.Local like @ClassOvo + '%'
    --        and CLLA.Local &lt;&gt; 'TB'))
and (CLLA.Local = @ClassOvo or @ClassOvo = 'T')

Union

select 
    CLLA.Local [Class. Ovos],
	CLLA.Nucleo [Granja / Núcleo],
	CLLA.LoteCompleto [Lote],
	CLLA.IdadeLote [Idade Lote],
	CLLA.Linhagem [Linhagem],
	CLLA.DataProducao [Data Prd.],
	Datediff(dd,CLLA.DataProducao,@Data) [Idade do Ovo],
    Convert(int,CLLA.Qtde) [Qtde.Ovos],
	0 [% Eclo. Real D4], 0 [% Eclo. Real Antepenúltima], 0 [% Eclo. Real Penúltima], 0 [% Eclo. Real Última],
	0 [Última Infertilidade 10 Dias],
    FD.Comentarios
from
	CTRL_LOTE_LOC_ARMAZ_WEB CLLA 
left join
	FLOCK_DATA FD on
		CLLA.LoteCompleto = FD.Flock_ID and CLLA.DataProducao = FD.Trx_Date
where
	(
        (@Pesquisa ='0')
        or 
        (@Campo = 'LN' and CLLA.Linhagem like '%' + @Pesquisa + '%')
        or
		(@Campo = 'DP' and Format(CLLA.DataProducao,'d',@Language) like '%' + @Pesquisa + '%')
        or
        (@Campo = 'LT' and CLLA.LoteCompleto like '%' + @Pesquisa + '%')
    ) 
	and 
	CLLA.Qtde &gt; 0 
	and 0 = (select COUNT(1) from TIPO_CLASSFICACAO_OVO TCO
				where TCO.Unidade = CLLA.Local)
	and CLLA.LoteCompleto not in ('VARIOS')
	and CLLA.Local = @Incubatorio
    --and ((CLLA.Local = @Incubatorio and 'NM' &lt;&gt; @Incubatorio)
    --   or ('NM' = @Incubatorio and CLLA.Local like @ClassOvo + '%'
    --        and CLLA.Local &lt;&gt; 'TB'))

order by
	2, 6">
                                    <SelectParameters>
                                        <asp:ControlParameter ControlID="Calendar1" DbType="Date" DefaultValue="" Name="Data"
                                            PropertyName="SelectedDate" />
                                        <asp:ControlParameter ControlID="TextBox6" Name="Pesquisa" PropertyName="Text" 
                                            DefaultValue="" />
                                        <asp:ControlParameter ControlID="DropDownList1" Name="Campo" 
                                            PropertyName="SelectedValue" DefaultValue="" />
                                        <asp:SessionParameter Name="Language" SessionField="Language" />
                                        <asp:ControlParameter ControlID="ddlIncubatorios" Name="Incubatorio" 
                                            PropertyName="SelectedValue" DefaultValue="" />
                                        <asp:ControlParameter ControlID="ddlClassOvos" Name="ClassOvo" 
                                            PropertyName="SelectedValue" />
                                    </SelectParameters>
                                </asp:SqlDataSource>
                                <asp:SqlDataSource ID="EggInvDataSource_Apolo" runat="server" 
                                    ConnectionString="<%$ ConnectionStrings:Apolo10ConnectionString %>" SelectCommand="select 
                  LA.USERCodigoFLIP [Class. Ovos],
	CL.USERGranjaNucleoFLIP [Granja / Núcleo],
	CL.CtrlLoteNum [Lote],
	CL.USERIdateLoteFLIP [Idade Lote],
	P.ProdNomeAlt1 [Linhagem],
	CL.CtrlLoteDataValid [Data Prd.],
	Datediff(dd,CL.CtrlLoteDataValid,@Data) [Idade do Ovo],
	--Convert(int,CLLA.CtrlLoteLocArmazQtdSaldo) - ISNULL(CLLA.USERQtdeIncNaoImportApolo,0) [Qtde.Ovos],
                   Convert(int,CLLA.CtrlLoteLocArmazQtdSaldo) [Qtde.Ovos],
	CL.USERPercMediaIncUlt4SemFLIP [Média Últ.4 Semanas (%)],
	Convert(int,(CL.USERPercMediaIncUlt4SemFLIP/100)*CLLA.CtrlLoteLocArmazQtdSaldo) [Qtde.Pint. Últ. 4 Semanas]
from
	CTRL_LOTE_LOC_ARMAZ CLLA 
inner join
	CTRL_LOTE CL on
		CLLA.EmpCod = CL.EmpCod and
		CLLA.ProdCodEstr = CL.ProdCodEstr and
		CLLA.CtrlLoteNum = CL.CtrlLoteNum and
		CLLA.CtrlLoteDataValid = CL.CtrlLoteDataValid and
		CL.CtrlLoteQtdSaldo &gt; 0 and 
		CL.USERGranjaNucleoFLIP is not null
inner join
	PRODUTO P on
		CLLA.ProdCodEstr = P.ProdCodEstr
inner join
	EMPRESA_FILIAL EF on
		CLLA.EmpCod = EF.EmpCod and
                                     EF.USERTipoUnidadeFLIP = 'Incubatório'
inner join
	LOC_ARMAZ LA on
		CLLA.LocArmazCodEstr = LA.LocArmazCodEstr and
                                     LA.USERTipoProduto = 'Ovos Incubáveis' and
		((LA.USERCodigoFLIP = @Incubatorio and 'NM' &lt;&gt; @Incubatorio)
                                       or ('NM' = @Incubatorio and LA.USERCodigoFLIP like @ClassOvo + '%'
                                             and LA.USERCodigoFLIP &lt;&gt; 'TB'))
where
	(
        (@Pesquisa ='0')
        or 
        (@Campo = 'Linhagem' and P.ProdNomeAlt1 like '%' + @Pesquisa + '%')
        or
		(@Campo = 'Data de Produção' and CL.CtrlLoteDataValid = Case When @Pesquisa = @Pesquisa Then '1988-01-01' Else @Pesquisa End)
        or
        (@Campo = 'Lote' and CL.CtrlLoteNum like '%' + @Pesquisa + '%')
    ) and CLLA.CtrlLoteLocArmazQtdSaldo &gt; 0
order by
	2, 6">
                                    <SelectParameters>
                                        <asp:ControlParameter ControlID="Calendar1" DbType="Date" DefaultValue="" Name="Data"
                                            PropertyName="SelectedDate" />
                                        <asp:ControlParameter ControlID="ddlIncubatorios" Name="Incubatorio" 
                                            PropertyName="SelectedValue" DefaultValue="" />
                                        <asp:ControlParameter ControlID="ddlClassOvos" Name="ClassOvo" 
                                            PropertyName="SelectedValue" />
                                        <asp:ControlParameter ControlID="TextBox6" Name="Pesquisa" PropertyName="Text" 
                                            DefaultValue="" />
                                        <asp:ControlParameter ControlID="DropDownList1" Name="Campo" 
                                            PropertyName="SelectedValue" DefaultValue="" />
                                    </SelectParameters>
                                </asp:SqlDataSource>
                                <asp:SqlDataSource ID="EggInvDataSource_FLIP" runat="server" 
                                    ConnectionString="<%$ ConnectionStrings:Oracle %>" 
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
                                        <asp:ControlParameter ControlID="Calendar1" DbType="Date" DefaultValue="" 
                                            Name="Data" PropertyName="SelectedDate" />
                                        <asp:ControlParameter ControlID="ddlIncubatorios" DefaultValue="" 
                                            Name="Incubatorio" PropertyName="SelectedValue" />
                                        <asp:ControlParameter ControlID="TextBox6" DefaultValue="" Name="Pesquisa" 
                                            PropertyName="Text" />
                                        <asp:ControlParameter ControlID="DropDownList1" DefaultValue="" Name="Campo" 
                                            PropertyName="SelectedValue" />
                                    </SelectParameters>
                                </asp:SqlDataSource>
                                <asp:SqlDataSource ID="EggInvDataSource_Antiga" runat="server" 
                                    ConnectionString="<%$ ConnectionStrings:Apolo114_TesteConnectionString %>" SelectCommand="select 
	CL.USERGranjaNucleoFLIP [Granja / Núcleo],
	CL.CtrlLoteNum [Lote],
	CL.USERIdateLoteFLIP [Idade Lote],
	P.ProdNomeAlt1 [Linhagem],
	CL.CtrlLoteDataValid [Data Prd.],
	Datediff(dd,CL.CtrlLoteDataValid,@Data) [Idade do Ovo],
	Convert(int,CLLA.CtrlLoteLocArmazQtdSaldo) [Qtde.Ovos],
	CL.USERPercMediaIncUlt4SemFLIP [Média Últ.4 Semanas (%)],
	Convert(int,(CL.USERPercMediaIncUlt4SemFLIP/100)*CLLA.CtrlLoteLocArmazQtdSaldo) [Qtde.Pint. Últ. 4 Semanas]
from
	CTRL_LOTE_LOC_ARMAZ CLLA 
inner join
	CTRL_LOTE CL on
		CLLA.EmpCod = CL.EmpCod and
		CLLA.ProdCodEstr = CL.ProdCodEstr and
		CLLA.CtrlLoteNum = CL.CtrlLoteNum and
		CLLA.CtrlLoteDataValid = CL.CtrlLoteDataValid and
		CL.CtrlLoteQtdSaldo &gt; 0 and 
		CL.USERGranjaNucleoFLIP is not null
inner join
	PRODUTO P on
		CLLA.ProdCodEstr = P.ProdCodEstr
inner join
	EMPRESA_FILIAL EF on
		CLLA.EmpCod = EF.EmpCod and
                                     EF.USERTipoUnidadeFLIP = 'Incubatório'
inner join
	LOC_ARMAZ LA on
		CLLA.LocArmazCodEstr = LA.LocArmazCodEstr and
                                     LA.USERTipoProduto = 'Ovos Incubáveis' and
		LA.USERCodigoFLIP in (Select LA2.USERLocalEstoqueIncub from LOC_ARMAZ LA2 
				     where LA2.USERCodigoFLIP = @Incubatorio)
where
	(
        (@Pesquisa ='0')
        or 
        (@Campo = 'Linhagem' and P.ProdNomeAlt1 like '%' + @Pesquisa + '%')
        or
		(@Campo = 'Data de Produção' and CL.CtrlLoteDataValid = Case When @Pesquisa = @Pesquisa Then '1988-01-01' Else @Pesquisa End)
        or
        (@Campo = 'Lote' and CL.CtrlLoteNum like '%' + @Pesquisa + '%')
    )
order by
	2, 5">
                                    <SelectParameters>
                                        <asp:ControlParameter ControlID="Calendar1" DbType="Date" DefaultValue="" 
                                            Name="Data" PropertyName="SelectedDate" />
                                        <asp:ControlParameter ControlID="ddlIncubatorios" DefaultValue="" 
                                            Name="Incubatorio" PropertyName="SelectedValue" />
                                        <asp:ControlParameter ControlID="TextBox6" DefaultValue="" Name="Pesquisa" 
                                            PropertyName="Text" />
                                        <asp:ControlParameter ControlID="DropDownList1" DefaultValue="" Name="Campo" 
                                            PropertyName="SelectedValue" />
                                    </SelectParameters>
                                </asp:SqlDataSource>
                                <br />
                            </td>
                            <td colspan="2">
                                <asp:FormView ID="FormView1" runat="server" CellPadding="4" Height="163px" BackColor="White"
                                    BorderColor="#CC9966" BorderStyle="None" BorderWidth="1px" DataKeyNames="FLOCK_ID,LAY_DATE,FARM_ID,LOCATION,COMPANY,REGION,HATCH_LOC,STATUS"
                                    DataSourceID="HatchFormDataSource" GridLines="Both" Style="font-size: xx-small"
                                    OnDataBound="FormView1_DataBound" Width="228px" OnItemUpdated="FormView1_ItemUpdated">
                                    <EditItemTemplate>
                                        <table style="width: 100%;">
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblFARM_ID" runat="server" Text="Granja:" />
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
                                                    <asp:Label ID="lblFLOCK_ID" runat="server" Text="Lote:" />
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
                                            <asp:Panel ID="pnlLayDateUnique" runat="server">
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblLAY_DATE" runat="server" Text="Data de Produção:" />
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
                                            </asp:Panel>
                                            <asp:Panel ID="pnlLayDateByPeriod" runat="server">
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblLayDateIni" runat="server" Text="Data de Produção Inicial:" />
                                                </td>
                                                <td>
                                                    <asp:Calendar ID="calLayDateIni" runat="server" BackColor="White" 
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
                                                    <asp:Label ID="lblLayDateFim" runat="server" Text="Data de Produção Final:" />
                                                </td>
                                                <td>
                                                    <asp:Calendar ID="calLayDateFim" runat="server" BackColor="White" 
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
                                            </asp:Panel>
                                            </tr>
                                            <tr>
                                                <td>
                                                    &nbsp;<asp:Label ID="lblSETTER" runat="server" Text="Incubadora:" />
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="MachineTextBox" runat="server" Text='<%# Bind("Machine") %>' Height="23px"
                                                        Width="56px" CssClass="upper-case" />
                                                    <asp:MaskedEditExtender ID="MaskedEditExtender2" runat="server" TargetControlID="MachineTextBox"
                                                        Mask="S-99" MessageValidatorTip="true" OnFocusCssClass="MaskedEditFocus" OnInvalidCssClass="MaskedEditError"
                                                        MaskType="Number" ErrorTooltipEnabled="True" AutoCompleteValue="S-" ClearMaskOnLostFocus="False">
                                                    </asp:MaskedEditExtender>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblEGGS_UNITS" runat="server" Text="Qtde.Ovos:" />
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="EGG_UNITSTextBox" runat="server" Text='<%# Bind("EGG_UNITS") %>'
                                                        Height="23px" Width="56px" OnTextChanged="EGG_UNITSTextBox_TextChanged" AutoPostBack="True" />                                                    
                                                    &nbsp;-
                                                    <asp:Label ID="Hatch_LocLabel" runat="server" Text='<%# Eval("Local") %>' />
                                                    <asp:DropDownList ID="ddlClasOvos" runat="server">
                                                        <asp:ListItem>T1</asp:ListItem>
                                                        <asp:ListItem>T0</asp:ListItem>
                                                        <asp:ListItem>T2</asp:ListItem>
                                                    </asp:DropDownList>
                                                    <br />
                                                    <asp:Label ID="lblMensagemEggUnits" runat="server" Style="font-weight: 700; color: #FF3300"
                                                        Visible="False"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblMediaEclosao" runat="server" Text="Média Eclosão:" />
&nbsp;</td>
                                                <td>
                                                    <asp:TextBox ID="MediaEclosaoTextBox" runat="server" Text='<%# Bind("MediaEclosao") %>'
                                                        Height="23px" Width="56px" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblHorario" runat="server" Text="Horário:" />
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="HorarioTextBox" runat="server" Height="40px" Text='<%# Bind("Horario") %>'
                                                        Width="56px" />
                                                    <asp:MaskedEditExtender ID="MaskedEditExtender1" runat="server" TargetControlID="HorarioTextBox"
                                                        Mask="99:99" MessageValidatorTip="true" OnFocusCssClass="MaskedEditFocus" OnInvalidCssClass="MaskedEditError"
                                                        MaskType="Time" AcceptAMPM="True" ErrorTooltipEnabled="True">
                                                    </asp:MaskedEditExtender>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblPosicao" runat="server" Text="Posição:" />
                                                    &nbsp;
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="PosicaoTextBox" runat="server" Height="23px" Text='<%# Bind("Posicao") %>'
                                                        Width="56px" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblBandejas" runat="server" Text="Bandejas:" />
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="BandejasTextBox" runat="server" Height="23px" Text='<%# Bind("Bandejas") %>'
                                                        Width="56px" Enabled="False" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="2">
                                                    <asp:Panel ID="pnlClasOvos" runat="server">
                                                        <br />
                                                        <table style="width: 100%;">
                                                            <tr>
                                                                <td colspan="2">
                                                                    <asp:Label ID="lblClasOvos" runat="server" Text="CLASSIFICAÇÃO DOS OVOS" 
                                                                        Font-Size="Small" Font-Bold="True" Font-Overline="False" 
                                                                        Font-Underline="True" />
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td>
                                                                    <asp:Label ID="lblOvosTrincados" runat="server" Text="Trincados:" />
                                                                </td>
                                                                <td>
                                                                    <asp:TextBox ID="txtOvosTrincados" runat="server" Text=""
                                                                        Height="23px" Width="56px" />      
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td>
                                                                    <asp:Label ID="lblOvosSujos" runat="server" Text="Sujos:" />
                                                                </td>
                                                                <td>
                                                                    <asp:TextBox ID="txtOvosSujos" runat="server" Text=""
                                                                        Height="23px" Width="56px" />      
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td>
                                                                    <asp:Label ID="lblOvosGrandes" runat="server" Text="Grandes:" />
                                                                </td>
                                                                <td>
                                                                    <asp:TextBox ID="txtOvosGrandes" runat="server" Text=""
                                                                        Height="23px" Width="56px" />      
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td>
                                                                    <asp:Label ID="lblOvosPequenos" runat="server" Text="Pequenos:" />
                                                                </td>
                                                                <td>
                                                                    <asp:TextBox ID="txtOvosPequenos" runat="server" Text=""
                                                                        Height="23px" Width="56px" />      
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td>
                                                                    <asp:Label ID="lblOvosQuebrados" runat="server" Text="Quebrados:" />
                                                                </td>
                                                                <td>
                                                                    <asp:TextBox ID="txtOvosQuebrados" runat="server" Text=""
                                                                        Height="23px" Width="56px" />      
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td>
                                                                    <asp:Label ID="lblOvosParaComercio" runat="server" Text="Comércio:" />
                                                                </td>
                                                                <td>
                                                                    <asp:TextBox ID="txtOvosParaComercio" runat="server" Text=""
                                                                        Height="23px" Width="56px" />      
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </asp:Panel>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="2">
                                                    <asp:Label ID="lblObservacao" runat="server" Text="Observação:" />
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
                                                    <asp:LinkButton ID="UpdateButton" runat="server" CausesValidation="True"
                                                        Text="INCUBAR" onclick="UpdateButton_Click" />
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
                                <asp:SqlDataSource ID="HatchFormDataSource" runat="server" 
                                    ConnectionString="<%$ ConnectionStrings:HLBAPPConnectionString %>" 
                                    
                                    
                                    
                                    
                                    
                                    
                                    SelectCommand="select 
	CLLA.Local Local,
	CLLA.LoteCompleto [FLOCK_ID],
	CLLA.DataProducao [LAY_DATE],
	Convert(int,CLLA.Qtde) [EGG_UNITS],
	CLLA.Nucleo [FARM_ID],
	CLLA.Local [LOCATION],
	'HYBR' [COMPANY], 
	'BR' [REGION], 
	CLLA.Local [HATCH_LOC], 
	'O' [STATUS],
	'S-01' [Machine], '00:00' [Horario], 
	'1' [Posicao], '0' [Bandejas], '0' [MediaEclosao], 'obs' [Observacao]
from
	CTRL_LOTE_LOC_ARMAZ_WEB CLLA 
WHERE 
	CLLA.Local = @ClassOvo and CLLA.LoteCompleto = @FLOCK_ID AND CLLA.DataProducao = @LAY_DATE and @Machine = 'S01' and @Horario = '00:00' and @Posicao = '1' and @Bandejas = '0' and @MediaEclosao = '0' and @Observacao = 'obs'">
                                    <SelectParameters>
                                        <asp:Parameter Name="ClassOvo" />
                                        <asp:Parameter DefaultValue="" Name="FLOCK_ID" />
                                        <asp:Parameter DbType="Date" DefaultValue="" Name="LAY_DATE" />
                                        <asp:Parameter DefaultValue="S01" Name="Machine" />
                                        <asp:Parameter DefaultValue="00:00" Name="Horario" />
                                        <asp:Parameter DefaultValue="1" Name="Posicao" />
                                        <asp:Parameter DefaultValue="0" Name="Bandejas" />
                                        <asp:Parameter DefaultValue="0" Name="MediaEclosao" />
                                        <asp:Parameter DefaultValue="obs" Name="observacao" />
                                    </SelectParameters>
                                </asp:SqlDataSource>
                                <asp:SqlDataSource ID="HatchFormDataSource_Apolo" runat="server" 
                                    ConnectionString="<%$ ConnectionStrings:Apolo10ConnectionString %>" 
                                    SelectCommand="select 
                   LA.USERCodigoFLIP Local,
                   CL.CtrlLoteNum [FLOCK_ID],
	CL.CtrlLoteDataValid [LAY_DATE],
	--Convert(int,CLLA.CtrlLoteLocArmazQtdSaldo) - ISNULL(CLLA.USERQtdeIncNaoImportApolo,0) [EGG_UNITS],
                   Convert(int,CLLA.CtrlLoteLocArmazQtdSaldo) [EGG_UNITS],
	CL.USERGranjaNucleoFLIP [FARM_ID],
                   LA.USERGeracaoFLIP [LOCATION],
                   'HYBR' [COMPANY], 
                   'BR' [REGION], 
                   EF.USERFLIPCod [HATCH_LOC], 
                   'O' [STATUS],
                   'S-01' [Machine], '00:00' [Horario], 
                   '1' [Posicao], '0' [Bandejas], '0' [MediaEclosao], 'obs' [Observacao]
from
	CTRL_LOTE_LOC_ARMAZ CLLA 
inner join
	CTRL_LOTE CL on
		CLLA.EmpCod = CL.EmpCod and
		CLLA.ProdCodEstr = CL.ProdCodEstr and
		CLLA.CtrlLoteNum = CL.CtrlLoteNum and
		CLLA.CtrlLoteDataValid = CL.CtrlLoteDataValid and
		CL.CtrlLoteQtdSaldo &gt; 0 and 
		CL.USERGranjaNucleoFLIP is not null
inner join
	PRODUTO P on
		CLLA.ProdCodEstr = P.ProdCodEstr
inner join
	EMPRESA_FILIAL EF on
		CLLA.EmpCod = EF.EmpCod and
		EF.USERTipoUnidadeFLIP = 'Incubatório'
inner join
                  LOC_ARMAZ LA on
		CLLA.LocArmazCodEstr = LA.LocArmazCodEstr and
                                     LA.USERTipoProduto = 'Ovos Incubáveis' and
		((LA.USERCodigoFLIP = @HATCH_LOC and 'NM' &lt;&gt; @HATCH_LOC)
                                       or ('NM' = @HATCH_LOC and LA.USERCodigoFLIP = @ClassOvo))
WHERE 
       ((CL.CtrlLoteNum = @FLOCK_ID) AND (CL.CtrlLoteDataValid = @LAY_DATE) and @Machine = 'S01' and @Horario = '00:00' and @Posicao = '1' and @Bandejas = '0' and @MediaEclosao = '0' and @Observacao = 'obs')">
                                    <SelectParameters>
                                        <asp:ControlParameter ControlID="ddlIncubatorios" DefaultValue="CH" 
                                            Name="HATCH_LOC" PropertyName="SelectedValue" />
                                        <asp:Parameter Name="ClassOvo" />
                                        <asp:Parameter DefaultValue="" Name="FLOCK_ID" />
                                        <asp:Parameter DbType="Date" DefaultValue="" Name="LAY_DATE" />
                                        <asp:Parameter DefaultValue="S01" Name="Machine" />
                                        <asp:Parameter DefaultValue="00:00" Name="Horario" />
                                        <asp:Parameter DefaultValue="1" Name="Posicao" />
                                        <asp:Parameter DefaultValue="0" Name="Bandejas" />
                                        <asp:Parameter DefaultValue="0" Name="MediaEclosao" />
                                        <asp:Parameter DefaultValue="obs" Name="observacao" />
                                    </SelectParameters>
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
                                                <asp:ListItem Value="LN">Linhagem</asp:ListItem>
                                                <asp:ListItem Value="DP">Data de Produção</asp:ListItem>
                                                <asp:ListItem Value="IN">Incubadora</asp:ListItem>
                                                <asp:ListItem Value="LT">Lote</asp:ListItem>
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
                                            <asp:Label ID="Label7" runat="server" Text=" para" CssClass="style61" 
                                                Font-Size="XX-Small"></asp:Label>
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
                                    <tr>
                                        <td colspan="5">
                                            <asp:Panel ID="pnlFiltroTipoEstoque" runat="server">
                                                <table>
                                                    <tr>
                                                        <td class="style65">
                                                            <asp:Label ID="lblFiltroTipoEstoque" runat="server" 
                                                                Text="Tipo de Estoque" CssClass="style61"
                                                                Font-Size="XX-Small"></asp:Label>
                                                        </td>
                                                        <td class="style65">
                                                            <asp:DropDownList ID="ddlTipoEstoque" runat="server" Height="21px" Style="font-weight: 700"
                                                                Width="124px" CssClass="style60" AutoPostBack="True">
                                                                <asp:ListItem>Todos</asp:ListItem>
                                                                <asp:ListItem>Estoque Real</asp:ListItem>
                                                                <asp:ListItem>Estoque Futuro</asp:ListItem>
                                                            </asp:DropDownList>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </asp:Panel>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td class="style59" colspan="3">
                            <asp:Panel ID="panelIncubacao" ScrollBars="Horizontal" runat="server">
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
                                    onrowediting="GridView1_RowEditing" 
                                    onrowdatabound="GridView1_RowDataBound">
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
                                        <asp:TemplateField HeaderText="ID" InsertVisible="False" SortExpression="ID">
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
                                        <asp:BoundField DataField="Import. FLIP" HeaderText="Import. FLIP" 
                                            SortExpression="Import. FLIP" ReadOnly="True" />
                                        <asp:TemplateField SortExpression="ClassOvo">
                                            <EditItemTemplate>
                                                <asp:TextBox ID="TextBox16" runat="server" Text='<%# Bind("ClassOvo") %>' 
                                                    Enabled="False"></asp:TextBox>
                                            </EditItemTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="Label17" runat="server" Text='<%# Bind("ClassOvo") %>'></asp:Label>
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
   ClassOvo,
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
   Usuario &quot;Usuário&quot;,
   ImportadoApolo &quot;Import. Apolo&quot;,
   ImportadoFLIP &quot;Import. FLIP&quot;
from hatchery_egg_data with(Nolock)
where hatch_loc = @Incubatorio
and set_date = @Data
and (
         @Pesquisa = '0'
         or 
         (@Campo = 'LN' and variety like '%'+@Pesquisa+'%')
         or
         (@Campo = 'DP' and Format(lay_date,'d',@Language) like '%' + @Pesquisa + '%')
         or
         (@Campo = 'IN' and machine like '%'+@Pesquisa+'%')
         or
         (@Campo = 'LT' and flock_id like '%'+@Pesquisa+'%')
        )
        and
        ((@TipoEstoque = 'Todos') or (@TipoEstoque = ImportadoApolo))
order by 2" UpdateCommand="UPDATE HATCHERY_EGG_DATA SET company = 'HYBR' where ID = @original_ID">
                                    <SelectParameters>
                                        <asp:ControlParameter ControlID="Calendar1" Name="Data" PropertyName="SelectedDate" />
                                        <asp:ControlParameter ControlID="ddlIncubatorios" Name="Incubatorio" 
                                            PropertyName="SelectedValue" />
                                        <asp:ControlParameter ControlID="TextBox1" Name="Pesquisa" PropertyName="Text" />
                                        <asp:ControlParameter ControlID="DropDownList2" Name="Campo" PropertyName="SelectedValue" />
                                        <asp:SessionParameter Name="Language" SessionField="Language" />
                                        <asp:ControlParameter ControlID="ddlTipoEstoque" Name="TipoEstoque" 
                                            PropertyName="SelectedValue" />
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
                                    <tr>
                                        <td colspan="3">
                                            <asp:Panel ID="pnlTabelaOvosClassificados" runat="server">
                                                <br />
                                                <br />
                                                <table style="width: 100%;">
                                                    <tr>
                                                        <td>
                                                            <asp:Label ID="lblOvosClassificados" runat="server" Font-Bold="True" Font-Size="Small" 
                                                                Font-Underline="False" Text="OVOS CLASSIFICADOS" 
                                                                style="text-decoration: underline"></asp:Label>
                                                            <br />
                                                            <asp:Label ID="lblMensagemOvosClass" runat="server" 
                                                                Style="font-weight: 700; color: #FF3300" Visible="False"></asp:Label>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <asp:GridView ID="gdvClasOvos" runat="server" CellPadding="4" ForeColor="#333333" 
                                                                GridLines="None" style="font-size: xx-small; text-align: center; " 
                                                                Width="100%" AutoGenerateColumns="False" 
                                                                DataSourceID="OvosClassificadosSqlDataSource" AllowPaging="True" 
                                                                AllowSorting="True" 
                                                                onselectedindexchanged="gdvClasOvos_SelectedIndexChanged">
                                                                <AlternatingRowStyle BackColor="White" />
                                                                <Columns>
                                                                    <asp:TemplateField ShowHeader="False">
                                                                        <EditItemTemplate>
                                                                            <asp:ImageButton ID="ibtnSaveClasOvo" runat="server" CausesValidation="True" 
                                                                                CommandName="Update" ImageUrl="~/Content/images/apply.png" 
                                                                                onclick="ibtnSaveClasOvo_Click" Text="V" />
                                                                            <asp:ImageButton ID="ibtnCancelClasOvo" runat="server" CausesValidation="False" 
                                                                                CommandName="Cancel" ImageUrl="~/Content/images/button_cancel.png" Text="X" />
                                                                        </EditItemTemplate>
                                                                        <ItemTemplate>
                                                                            <asp:ImageButton ID="ibtnUpdateClasOvo" runat="server" CausesValidation="False" 
                                                                                CommandName="Edit" ImageUrl="~/Content/images/kjots.png" Text="Edit" />
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>
                                                                    <asp:CommandField ButtonType="Image" SelectImageUrl="~/Content/images/Nao.png" 
                                                                        ShowSelectButton="True" />
                                                                    <asp:BoundField DataField="HatchLoc" HeaderText="HatchLoc" 
                                                                        SortExpression="HatchLoc" Visible="False" ReadOnly="True" />
                                                                    <asp:BoundField DataField="SetDate" HeaderText="SetDate" 
                                                                        SortExpression="SetDate" Visible="False" ReadOnly="True" />
                                                                    <asp:BoundField DataField="Variety" HeaderText="Linhagem" 
                                                                        SortExpression="Variety" ReadOnly="True" />
                                                                    <asp:TemplateField HeaderText="Lote Completo" SortExpression="FlockID">
                                                                        <EditItemTemplate>
                                                                            <asp:Label ID="lblLoteCompleto" runat="server" Text='<%# Eval("FlockID") %>'></asp:Label>
                                                                        </EditItemTemplate>
                                                                        <ItemTemplate>
                                                                            <asp:Label ID="lblLoteCompleto" runat="server" Text='<%# Bind("FlockID") %>'></asp:Label>
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>
                                                                    <asp:BoundField DataField="FlockNumber" HeaderText="Nº Lote" 
                                                                        SortExpression="FlockNumber" ReadOnly="True" />
                                                                    <asp:TemplateField HeaderText="Data Produção" SortExpression="LayDate">
                                                                        <EditItemTemplate>
                                                                            <asp:Label ID="lblDataProducao" runat="server" 
                                                                                Text='<%# Eval("LayDate", "{0:d}") %>'></asp:Label>
                                                                        </EditItemTemplate>
                                                                        <ItemTemplate>
                                                                            <asp:Label ID="lblDataProducao" runat="server" 
                                                                                Text='<%# Bind("LayDate", "{0:d}") %>'></asp:Label>
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="Trincados" SortExpression="CrackedEggs">
                                                                        <EditItemTemplate>
                                                                            <asp:TextBox ID="txtOvosTrincados" runat="server" 
                                                                                Text='<%# Eval("CrackedEggs", "{0:N0}") %>' Width="40px"></asp:TextBox>
                                                                        </EditItemTemplate>
                                                                        <ItemTemplate>
                                                                            <asp:Label ID="Label1" runat="server" 
                                                                                Text='<%# Bind("CrackedEggs", "{0:N0}") %>'></asp:Label>
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="Sujos" SortExpression="DirtyEggs">
                                                                        <EditItemTemplate>
                                                                            <asp:TextBox ID="txtOvosSujos" runat="server" 
                                                                                Text='<%# Eval("DirtyEggs", "{0:N0}") %>' Width="40px"></asp:TextBox>
                                                                        </EditItemTemplate>
                                                                        <ItemTemplate>
                                                                            <asp:Label ID="Label2" runat="server" Text='<%# Bind("DirtyEggs", "{0:N0}") %>'></asp:Label>
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="Grandes" SortExpression="BigEggs">
                                                                        <EditItemTemplate>
                                                                            <asp:TextBox ID="txtOvosGrandes" runat="server" 
                                                                                Text='<%# Eval("BigEggs", "{0:N0}") %>' Width="40px"></asp:TextBox>
                                                                        </EditItemTemplate>
                                                                        <ItemTemplate>
                                                                            <asp:Label ID="Label3" runat="server" Text='<%# Bind("BigEggs", "{0:N0}") %>'></asp:Label>
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="Pequenos" SortExpression="SmallEggs">
                                                                        <EditItemTemplate>
                                                                            <asp:TextBox ID="txtOvosPequenos" runat="server" Height="22px" 
                                                                                Text='<%# Eval("SmallEggs", "{0:N0}") %>' Width="40px"></asp:TextBox>
                                                                        </EditItemTemplate>
                                                                        <ItemTemplate>
                                                                            <asp:Label ID="Label4" runat="server" Text='<%# Bind("SmallEggs", "{0:N0}") %>'></asp:Label>
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="Quebrados" SortExpression="BrokenEggs">
                                                                        <EditItemTemplate>
                                                                            <asp:TextBox ID="txtOvosQuebrados" runat="server" Height="22px"
                                                                                Text='<%# Eval("BrokenEggs", "{0:N0}") %>' Width="40px"></asp:TextBox>
                                                                        </EditItemTemplate>
                                                                        <ItemTemplate>
                                                                            <asp:Label ID="Label5" runat="server" Text='<%# Bind("BrokenEggs", "{0:N0}") %>'></asp:Label>
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="Comércio" SortExpression="SalesEggs">
                                                                        <EditItemTemplate>
                                                                            <asp:TextBox ID="txtOvosComercio" runat="server" Height="22px"
                                                                                Text='<%# Eval("SalesEggs", "{0:N0}") %>' Width="40px"></asp:TextBox>
                                                                        </EditItemTemplate>
                                                                        <ItemTemplate>
                                                                            <asp:Label ID="Label6" runat="server" Text='<%# Bind("SalesEggs", "{0:N0}") %>'></asp:Label>
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
                                                            
                                                            <asp:SqlDataSource ID="OvosClassificadosSqlDataSource" runat="server" 
                                                                ConnectionString="<%$ ConnectionStrings:HLBAPPConnectionString %>" SelectCommand="select * from VU_Sorting_Eggs_By_Sett_Eggs_WEB
where HatchLoc = @Incubatorio and SetDate = @SetDate">
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
                                            </asp:Panel>
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
