<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EFD_PisCofins_F130.aspx.cs" Inherits="MvcAppHyLinedoBrasil.WebForms.EFD_PisCofins_F130" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>EFD Pis Cofins - F130</title>
    <link href="../Content/icons/logo_hyline.ico" rel="Shortcut Icon" type="text/css" />
    <style type="text/css">
        .style4
        {
            font-weight: bold;
            text-align: left;
        }
        .style14
        {
            width: 785px;
            height: 125px;
        }
        .style20
        {
            width: 308px;
            left: 50%;
            text-align: right;
        }
        .style22
        {
            width: 274px;
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
            width: 270px;
            height: 104px;
        }
        .style24
        {
            font-weight: bold;
        }
        .style39
        {
            width: 293px;
        }
        .style41
        {
            width: 428px;
        }
        .style51
        {
            width: 274px;
        }
        .style52
        {
            text-align: right;
            width: 465px;
        }
        .style53
        {
            width: 354px;
        }
        .style54
        {
            width: 354px;
            text-align: left;
        }
        .style55
        {
            text-align: right;
            width: 428px;
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
                                <img alt="" class="style23" src="../Content/images/logo.png" border="0" /></a>
                            <asp:HyperLink ID="HyperLink1" runat="server" 
                                NavigateUrl="~/WebForms/EFD_PisCofins.aspx">Voltar</asp:HyperLink>
                        </td>
                    </tr>
                    <tr>
                        <td>
                        </td>
                        <td class="style14">
                            <asp:Label ID="Label5" runat="server" Font-Bold="True" Font-Size="XX-Large" Font-Underline="False"
                                Text="EFD PIS COFINS - BLOCO F130"></asp:Label></a>
                        </td>
                    </tr>
                </table>
                <asp:Panel ID="Panel1" runat="server" Width="1003px" HorizontalAlign="Center">
                    <table align="center">
                        <tr>
                            <td>
                                &nbsp;</td>
                            <td style="text-align: left" class="style22">
                                &nbsp;</td>
                            <td class="style39">
                                &nbsp;
                            </td>
                        </tr>
                        <tr>
                            <td class="style20">
                                <asp:Label ID="Label8" runat="server" CssClass="style4" Font-Size="Small" 
                                    Text="Cód.Reduzido: "></asp:Label>
                            </td>
                            <td class="style22">
                                <asp:TextBox ID="TextBox5" runat="server" CssClass="style24"></asp:TextBox>
                                &nbsp;
                            </td>
                            <td class="style39">
                                &nbsp;
                            </td>
                        </tr>
                        <tr>
                            <td class="style20">
                                <asp:Label ID="Label7" runat="server" CssClass="style4" Font-Size="Small" 
                                    Text="Descrição: "></asp:Label>
                            </td>
                            <td class="style22">
                                <asp:TextBox ID="TextBox6" runat="server" Width="250px" CssClass="style24"></asp:TextBox>
                            </td>
                            <td class="style39">
                                <asp:Button ID="btn_Pesquisar" runat="server" Font-Bold="True" 
                                    Font-Size="Medium" Height="72px" OnClick="Button1_Click" Text="Pesquisar" 
                                    Width="118px" />
                            </td>
                        </tr>
                        <tr>
                            <td class="style20">
                                <asp:Label ID="Label9" runat="server" CssClass="style4" Font-Size="Small" 
                                    Text="Valor: "></asp:Label>
                            </td>
                            <td class="style22">
                                <asp:TextBox ID="TextBox7" runat="server" CssClass="style24"></asp:TextBox>
                            </td>
                            <td class="style39">
                                &nbsp;
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
                <asp:Panel ID="Panel2" runat="server" Visible="False" 
                    Width="1218px">
                    <asp:Label ID="Label6" runat="server" Font-Bold="True" Font-Size="Large" Font-Underline="False"
                        Text="IMOBILIZADO"></asp:Label>
                    <br />
                    <br />
                    <table style="width: 100%;">
                        <tr>
                            <td class="style55" colspan="1" rowspan="1">
                                &nbsp;
                            </td>
                            <td colspan="1" class="style53">
                                <asp:Label ID="Label10" runat="server" Font-Size="Small" Text="Alteração por Lote"
                                    Style="font-weight: 700; text-decoration: underline"></asp:Label>
                            </td>
                            <td colspan="1">
                                &nbsp;
                            </td>
                        </tr>
                        <tr>
                            <td class="style41">
                                &nbsp;
                            </td>
                            <td class="style53">
                                &nbsp;
                            </td>
                            <td class="style25">
                                &nbsp;
                            </td>
                        </tr>
                        <tr>
                            <td class="style55">
                                &nbsp;
                                <asp:Label ID="Label11" runat="server" CssClass="style4" Font-Size="Small" 
                                    Text="Ano Mês Inicial de Recuperação: "></asp:Label>
                            </td>
                            <td class="style54">
                                <asp:TextBox ID="TextBox8" runat="server"></asp:TextBox>
                                <asp:Label ID="Label14" runat="server" Font-Bold="True" Font-Size="Smaller" 
                                    ForeColor="Red" Text="   Exemplo: 201301"></asp:Label>
                            </td>
                            <td>
                                &nbsp;
                            </td>
                        </tr>
                        <tr>
                            <td class="style55">
                                &nbsp;
                                </td>
                            <td class="style54">
                                &nbsp;</td>
                            <td style="text-align: left">
                                <asp:Button ID="Button1" runat="server" Height="22px" OnClick="Button1_Click1" Style="text-align: center"
                                    Text="Alterar" Width="106px" />
                                &nbsp;
                            </td>
                        </tr>
                        <tr>
                            <td class="style55">
                                &nbsp;
                                <asp:Label ID="Label13" runat="server" CssClass="style4" Font-Size="Small" 
                                    Text="Ano Mês Final de Recuperação: "></asp:Label>
                            </td>
                            <td class="style54">
                                <asp:TextBox ID="TextBox9" runat="server"></asp:TextBox>
                                <asp:Label ID="Label15" runat="server" Font-Bold="True" Font-Size="Smaller" 
                                    ForeColor="Red" Text="   Exemplo: 201301"></asp:Label>
                            </td>
                            <td>
                                &nbsp;
                            </td>
                        </tr>
                    </table>
                    <br />
                    <br />
                    <asp:Panel ID="Panel3" runat="server" Visible="True" ScrollBars="Horizontal" 
                    Width="1218px">
                    <asp:GridView ID="GridView1" runat="server" AllowPaging="True" AllowSorting="True"
                        AutoGenerateColumns="False" CellPadding="4" DataKeyNames="EmpCod,PatBemCodRed"
                        Font-Size="Small" ForeColor="#333333" GridLines="None" Width="3000px" 
                        DataSourceID="Imobilizado">
                        <AlternatingRowStyle BackColor="White" />
                        <Columns>
                            <asp:TemplateField>
                                <HeaderTemplate>
                                    <asp:CheckBox ID="CheckBox2" runat="server" AutoPostBack="True" 
                                        oncheckedchanged="CheckBox2_CheckedChanged" />
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:CheckBox ID="CheckBox1" runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:CommandField ButtonType="Image" 
                                CancelImageUrl="~/Content/images/button_cancel.png" 
                                EditImageUrl="~/Content/images/kjots.png" ShowEditButton="True" 
                                UpdateImageUrl="~/Content/images/apply.png" />
                            <asp:BoundField DataField="EmpCod" HeaderText="Empresa" ReadOnly="True" SortExpression="EmpCod" />
                            <asp:BoundField DataField="PatBemCodRed" HeaderText="Cód.Red." ReadOnly="True"
                                SortExpression="PatBemCodRed" />
                            <asp:BoundField DataField="PatBemNome" HeaderText="Descrição do Bem" ReadOnly="True"
                                SortExpression="PatBemNome" >
                            <ControlStyle Width="450px" />
                            <HeaderStyle Width="450px" />
                            <ItemStyle Width="450px" />
                            </asp:BoundField>
                            <asp:BoundField DataField="PatBemDataAquis" HeaderText="Data da Aquisição" 
                                ReadOnly="True" SortExpression="PatBemDataAquis" DataFormatString="{0:d}" />
                            <asp:BoundField DataField="ValorAquisicao" HeaderText="Valor da Aquisição" 
                                ReadOnly="True" SortExpression="ValorAquisicao" DataFormatString="{0:c}" />
                            <asp:TemplateField HeaderText="Ano Mês Inicio" 
                                SortExpression="PatBemAnoMesInicEfdPisCofins">
                                <EditItemTemplate>
                                    <asp:TextBox ID="TextBox1" runat="server" 
                                        Text='<%# Bind("PatBemAnoMesInicEfdPisCofins") %>' Width="83px"></asp:TextBox>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="Label1" runat="server" 
                                        Text='<%# Bind("PatBemAnoMesInicEfdPisCofins") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Ano Mês Fim" 
                                SortExpression="PatBemAnoMesFimEfdPisCofins">
                                <EditItemTemplate>
                                    <asp:TextBox ID="TextBox2" runat="server" 
                                        Text='<%# Bind("PatBemAnoMesFimEfdPisCofins") %>' Width="83px"></asp:TextBox>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="Label2" runat="server" 
                                        Text='<%# Bind("PatBemAnoMesFimEfdPisCofins") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="PatBemBaseCalcCOFINS" 
                                HeaderText="Base COFINS" SortExpression="PatBemBaseCalcCOFINS" 
                                DataFormatString="{0:c}" ReadOnly="True" />
                            <asp:BoundField DataField="PatBemBaseCalcPIS" HeaderText="Base PIS" 
                                ReadOnly="True" SortExpression="PatBemBaseCalcPIS" DataFormatString="{0:c}" />
                            <asp:BoundField DataField="PatBemValBaseIcms" DataFormatString="{0:c}" HeaderText="Base ICMS"
                                ReadOnly="True" SortExpression="PatBemValBaseIcms" />
                            <asp:BoundField DataField="PatBemQtdParcApropiar" 
                                HeaderText="Qtd.Parc.Apropiar" ReadOnly="True" 
                                SortExpression="PatBemQtdParcApropiar" />
                            <asp:BoundField DataField="PatBemValPIS" DataFormatString="{0:c}" 
                                HeaderText="Valor PIS" ReadOnly="True" SortExpression="PatBemValPIS" />
                            <asp:BoundField DataField="PatBemValCOFINS" DataFormatString="{0:c}" 
                                HeaderText="Valor COFINS" ReadOnly="True" SortExpression="PatBemValCOFINS" />
                            <asp:BoundField DataField="PatBemCst" HeaderText="CST" ReadOnly="True" 
                                SortExpression="PatBemCst" />
                            <asp:BoundField DataField="PatBemCodBaseCred" HeaderText="Cód.Base Cred." 
                                ReadOnly="True" SortExpression="PatBemCodBaseCred" />
                            <asp:BoundField DataField="PatBemIdentifBemIncorp" 
                                HeaderText="Ident.Bens Incorporados" ReadOnly="True" 
                                SortExpression="PatBemIdentifBemIncorp" />
                            <asp:BoundField DataField="PatBemOrigCred" HeaderText="Origem do Crédito" 
                                ReadOnly="True" SortExpression="PatBemOrigCred" />
                            <asp:BoundField DataField="PatBemAnoMesEfdPisCofins" HeaderText="Ano Mês" 
                                ReadOnly="True" SortExpression="PatBemAnoMesEfdPisCofins" />
                            <asp:BoundField DataField="PatBemIndicUtiliz" HeaderText="Inidic.Utilização" 
                                ReadOnly="True" SortExpression="PatBemIndicUtiliz" />
                            <asp:BoundField DataField="PatBemParcAquisDeduzCred" 
                                HeaderText="Parc.Aquisição Deduz Crédito" ReadOnly="True" 
                                SortExpression="PatBemParcAquisDeduzCred" />
                            <asp:BoundField DataField="PatBemIndicNumParc" HeaderText="Indic.Núm.Parc." 
                                ReadOnly="True" SortExpression="PatBemIndicNumParc" />
                            <asp:BoundField DataField="PatBemPercPIS" HeaderText="Perc.Pis" ReadOnly="True" 
                                SortExpression="PatBemPercPIS" />
                            <asp:BoundField DataField="PatBemPercCOFINS" HeaderText="Perc.Cofins" 
                                ReadOnly="True" SortExpression="PatBemPercCOFINS" />
                            <asp:BoundField DataField="Base_Calculo_ICMS" DataFormatString="{0:c}" 
                                HeaderText="Valor da Base de Cálculo" ReadOnly="True" 
                                SortExpression="Base_Calculo_ICMS" />
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
                    <br />
                    <asp:SqlDataSource ID="Imobilizado" runat="server" ConnectionString="<%$ ConnectionStrings:Apolo10ConnectionString %>"
                        SelectCommand="SELECT EmpCod, PatBemCodRed, PatBemNome, PatBemDataAquis, CASE WHEN B.PatBemValAquisEfdPisCofins IS NULL THEN B.PatBemValOrig ELSE B.PatBemValAquisEfdPisCofins END AS ValorAquisicao, PatBemBaseCalcCOFINS, PatBemBaseCalcPIS, PatBemValBaseIcms, PatBemQtdParcApropiar, PatBemValPIS, PatBemValCOFINS, PatBemCst, PatBemCodBaseCred, PatBemIdentifBemIncorp, PatBemOrigCred, PatBemAnoMesEfdPisCofins, PatBemIndicUtiliz, PatBemParcAquisDeduzCred, PatBemIndicNumParc, PatBemPercPIS, PatBemPercCOFINS, (PatBemValBaseIcms - PatBemParcAquisDeduzCred) / CASE WHEN PatBemQtdParcApropiar = 0 THEN 1 ELSE PatBemQtdParcApropiar END AS Base_Calculo_ICMS, PatBemAnoMesInicEfdPisCofins, PatBemAnoMesFimEfdPisCofins FROM PAT_BEM AS B WITH (nolock) WHERE (PatBemCodRed LIKE '%' + @CodRed + '%' or Charindex('*' + PatBemCodRed + '*',@CodRed) &gt; 0) AND (PatBemNome LIKE '%' + @Nome + '%') AND (CONVERT (varchar, PatBemValOrig) LIKE '%' + @Valor + '%') ORDER BY PatBemDataAquis, PatBemCodRed"
                        UpdateCommand="update PAT_BEM set PATBEMANOMESINICEFDPISCOFINS = @PATBEMANOMESINICEFDPISCOFINS, 
					PATBEMANOMESFIMEFDPISCOFINS = @PATBEMANOMESFIMEFDPISCOFINS
where EmpCod = @EMPCOD and PatBemCodRed = @PatBemCodRed">
                        <SelectParameters>
                            <asp:ControlParameter ControlID="TextBox5" Name="CodRed" PropertyName="Text" 
                                ConvertEmptyStringToNull="False" Type="String" DefaultValue="" />
                            <asp:ControlParameter ControlID="TextBox6" Name="Nome" PropertyName="Text" 
                                Type="String" ConvertEmptyStringToNull="False" DefaultValue="" />
                            <asp:ControlParameter ControlID="TextBox7" Name="Valor" PropertyName="Text"
                                Type="String" ConvertEmptyStringToNull="False" DefaultValue="" />
                        </SelectParameters>
                        <UpdateParameters>
                            <asp:Parameter Name="PATBEMANOMESINICEFDPISCOFINS" />
                            <asp:Parameter Name="PATBEMANOMESFIMEFDPISCOFINS" />
                            <asp:ControlParameter ControlID="GridView1" Name="EmpCod" 
                                PropertyName="SelectedValue" />
                            <asp:Parameter Name="PatBemCodRed" />
                        </UpdateParameters>
                    </asp:SqlDataSource>
                    </asp:Panel>
                    <br />
                    <asp:Button ID="btn_Nova_Pesquisa" runat="server" Text="Nova Pesquisa" Font-Bold="True"
                        OnClick="btn_Nova_Pesquisa_Click" />
                </asp:Panel>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>    
    </form>
</body>
</html>
