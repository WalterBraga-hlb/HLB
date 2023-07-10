<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EFD_PisCofins_Nfs_Saida.aspx.cs"
    Inherits="MvcAppHyLinedoBrasil.WebForms.AtualizaNF_EFD_PisCofins" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>EFD Pis Cofins - NFs Saida</title>
    <link href="../Content/icons/logo_hyline.ico" rel="Shortcut Icon" type="text/css" />
    <style type="text/css">
        .style3
        {
            width: 293px;
            text-align: center;
        }
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
        .style21
        {
            width: 308px;
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
        .style25
        {
            height: 22px;
        }
        .style27
        {
            height: 22px;
            width: 298px;
        }
        .style29
        {
            height: 22px;
            width: 596px;
        }
        .style30
        {
            width: 596px;
        }
        .style34
        {
            width: 144px;
            text-align: left;
        }
        .style37
        {
            width: 298px;
            text-align: right;
        }
        .style39
        {
            width: 293px;
        }
        .style40
        {
            width: 383px;
            text-align: right;
        }
        .style41
        {
            width: 383px;
            left: 50%;
            text-align: right;
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
        function mvalor4(v) {
            v = v.replace(/\D/g, ""); //Remove tudo o que não é dígito
            v = v.replace(/(\d)(\d{10})$/, "$1.$2"); //coloca o ponto dos milhões
            v = v.replace(/(\d)(\d{7})$/, "$1.$2"); //coloca o ponto dos milhares

            v = v.replace(/(\d)(\d{4})$/, "$1,$2"); //coloca a virgula antes dos 2 últimos dígitos
            return v;
        }
    </script>
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
                                Text="AJUSTE DE NFS - EFD PIS COFINS"></asp:Label></a>
                        </td>
                    </tr>
                </table>
                <asp:Panel ID="Panel1" runat="server" Width="1003px" HorizontalAlign="Center">
                    <table align="center">
                        <tr>
                            <td class="style41">
                                <asp:Label ID="Label1" runat="server" Text="Selecione a opção do Grupo de Empresas: "
                                    CssClass="style4" Font-Size="Small"></asp:Label>
                            </td>
                            <td style="text-align: left" class="style22">
                                <asp:DropDownList ID="DropDownList1" runat="server" Height="19px" Width="121px" CssClass="style4">
                                    <asp:ListItem Value="1">Hy-Line</asp:ListItem>
                                    <asp:ListItem Value="2">Lohmann</asp:ListItem>
                                    <asp:ListItem Value="3">H&amp;N</asp:ListItem>
                                    <asp:ListItem Value="4">Planalto</asp:ListItem>
                                    <asp:ListItem Value="5">Todas</asp:ListItem>
                                </asp:DropDownList>
                            </td>
                            <td class="style39">
                                &nbsp;
                            </td>
                        </tr>
                        <tr>
                            <td class="style41">
                                <asp:Label ID="Label8" runat="server" CssClass="style4" Font-Size="Small" Text="Nº Nota Fiscal: "></asp:Label>
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
                            <td class="style41">
                                <asp:Label ID="Label7" runat="server" CssClass="style4" Font-Size="Small" Text="Entidade: "></asp:Label>
                            </td>
                            <td class="style22">
                                <asp:TextBox ID="TextBox6" runat="server" Width="250px" CssClass="style24"></asp:TextBox>
                            </td>
                            <td class="style39">
                                &nbsp;
                            </td>
                        </tr>
                        <tr>
                            <td class="style41">
                                <asp:Label ID="Label9" runat="server" CssClass="style4" Font-Size="Small" Text="Descrição do Produto: "></asp:Label>
                            </td>
                            <td class="style22">
                                <asp:TextBox ID="TextBox7" runat="server" Width="250px" CssClass="style24"></asp:TextBox>
                            </td>
                            <td class="style39">
                                &nbsp;
                            </td>
                        </tr>
                        <tr>
                            <td class="style41">
                                &nbsp;
                            </td>
                            <td class="style22">
                                &nbsp;
                            </td>
                            <td class="style39">
                                &nbsp;
                            </td>
                        </tr>
                        <tr>
                            <td style="text-align: left">
                                <asp:Label ID="Label2" runat="server" Text="Data Inicial" CssClass="style4" Font-Size="Small"></asp:Label>
                            </td>
                            <td style="text-align: left" class="style22">
                                <asp:Label ID="Label3" runat="server" Text="Data Final" CssClass="style4" Font-Size="Small"></asp:Label>
                            </td>
                            <td class="style3">
                                &nbsp;
                            </td>
                        </tr>
                        <tr>
                            <td class="style40">
                                <asp:Calendar ID="Calendar1" runat="server" BackColor="White" BorderColor="#3366CC"
                                    BorderWidth="1px" CellPadding="1" DayNameFormat="Shortest" Font-Names="Verdana"
                                    Font-Size="8pt" ForeColor="#003399" Height="200px" Style="margin-top: 0px" Width="220px">
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
                            <td>
                                <asp:Calendar ID="Calendar2" runat="server" BackColor="White" BorderColor="#3366CC"
                                    BorderWidth="1px" CellPadding="1" DayNameFormat="Shortest" Font-Names="Verdana"
                                    Font-Size="8pt" ForeColor="#003399" Height="200px" Width="220px">
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
                            <td style="text-align: left" class="style39">
                                <asp:Button ID="btn_Pesquisar_C_D" runat="server" Font-Bold="True" Font-Size="Medium"
                                    Height="72px" Text="Blocos C / D" OnClick="Button1_Click" Width="118px" />
                                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                <asp:Button ID="btn_Pesquisar_C_D0" runat="server" Font-Bold="True" Font-Size="Medium"
                                    Height="72px" OnClick="btn_Pesquisar_C_D0_Click" Text="Outras NFs" Width="118px" />
                                <br />
                                <br />
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
                <br />
                <asp:UpdateProgress ID="UpdateProgress1" runat="server" 
                    AssociatedUpdatePanelID="UpdatePanel1">
                    <ProgressTemplate>
                        <strong>CARREGANDO...</strong>&nbsp;&nbsp;
                        <br />
                        <asp:Image ID="Image1" runat="server" ImageUrl="~/Content/images/ajax-loading.gif" />
                    </ProgressTemplate>
                </asp:UpdateProgress>
                <asp:Panel ID="Panel2" runat="server" Visible="False">
                    <asp:Label ID="Label6" runat="server" Font-Bold="True" Font-Size="Large" Font-Underline="False"
                        Text="BLOCOS C / D"></asp:Label>
                    <br />
                    <br />
                    <asp:Label ID="lblMensagem3" runat="server" Style="font-weight: 700; color: #FF3300" Visible="False"></asp:Label>
                    <table style="width: 114%;">
                        <tr>
                            <td class="style37" colspan="1" rowspan="1">
                                &nbsp;
                            </td>
                            <td colspan="1" class="style30">
                                <asp:Label ID="Label10" runat="server" Font-Size="Small" Text="Alteração por Lote"
                                    Style="font-weight: 700; text-decoration: underline"></asp:Label>
                            </td>
                            <td colspan="1">
                                &nbsp;
                            </td>
                        </tr>
                        <tr>
                            <td class="style27">
                                &nbsp;
                            </td>
                            <td class="style29">
                                &nbsp;
                            </td>
                            <td class="style25">
                                &nbsp;
                            </td>
                        </tr>
                        <tr>
                            <td class="style37">
                                &nbsp;
                                <asp:Label ID="Label11" runat="server" CssClass="style4" Font-Size="Small" Text="CST COFINS: "></asp:Label>
                            </td>
                            <td class="style34">
                                <asp:DropDownList ID="DropDownList3" runat="server" DataSourceID="CSTCofins" DataTextField="Descricao"
                                    DataValueField="ConfTribCod">
                                    <asp:ListItem Selected="True">(Nenhum)</asp:ListItem>
                                </asp:DropDownList>
                                <asp:SqlDataSource ID="CSTCofins" runat="server" ConnectionString="<%$ ConnectionStrings:Apolo10ConnectionString %>"
                                    SelectCommand="select ConfTribCod, ConfTribCod + ' - ' + ConfTribNome as Descricao from CONFIG_TRIBUT
where ConfTribTipo = 'COFINS'
order by 2"></asp:SqlDataSource>
                            </td>
                            <td>
                                &nbsp;
                            </td>
                        </tr>
                        <tr>
                            <td class="style37">
                                &nbsp;
                                <asp:Label ID="Label12" runat="server" CssClass="style4" Font-Size="Small" Text="CST PIS: "></asp:Label>
                            </td>
                            <td class="style34">
                                <asp:DropDownList ID="DropDownList4" runat="server" DataSourceID="CSTPis" DataTextField="Descricao"
                                    DataValueField="ConfTribCod" Height="27px" Width="571px">
                                    <asp:ListItem Selected="True">(Nenhum)</asp:ListItem>
                                </asp:DropDownList>
                                <asp:SqlDataSource ID="CSTPis" runat="server" ConnectionString="<%$ ConnectionStrings:Apolo10ConnectionString %>"
                                    SelectCommand="select ConfTribCod, ConfTribCod + ' - ' + ConfTribNome as Descricao from CONFIG_TRIBUT
where ConfTribTipo = 'PIS'
order by 2"></asp:SqlDataSource>
                            </td>
                            <td style="text-align: left">
                                <asp:Button ID="Button1" runat="server" Height="22px" OnClick="Button1_Click1" Style="text-align: center"
                                    Text="Alterar" Width="106px" />
                                &nbsp;
                            </td>
                        </tr>
                        <tr>
                            <td class="style37">
                                &nbsp;
                                <asp:Label ID="Label13" runat="server" CssClass="style4" Font-Size="Small" Text="Tipo EFD: "></asp:Label>
                            </td>
                            <td class="style34">
                                <asp:DropDownList ID="DropDownList6" runat="server" DataSourceID="EFD_BASE_CALC_CRED"
                                    DataTextField="Column1" DataValueField="EFDBaseCalcCredCod" Height="27px" Width="571px">
                                    <asp:ListItem Selected="True">(Nenhum)</asp:ListItem>
                                </asp:DropDownList>
                                <asp:SqlDataSource ID="EFD_BASE_CALC_CRED" runat="server" ConnectionString="<%$ ConnectionStrings:Apolo10ConnectionString %>"
                                    SelectCommand="select EFDBaseCalcCredCod, EFDBaseCalcCredCod + ' - ' + EFDBaseCalcCredDesc 
from EFD_BASE_CALC_CRED
order by 2"></asp:SqlDataSource>
                            </td>
                            <td>
                                &nbsp;
                            </td>
                        </tr>
                    </table>
                    <br />
                    <br />
                    <div style="overflow: auto; width: 100%">
                    <asp:GridView ID="GridView1" runat="server" AllowPaging="True" AllowSorting="True"
                        AutoGenerateColumns="False" CellPadding="4" DataKeyNames="EmpCod,FiscalNFChv,FiscalItNFSeq"
                        Font-Size="Small" ForeColor="#333333" GridLines="None" Width="1212px" OnRowUpdated="GridView1_RowUpdated"
                        OnRowUpdating="GridView1_RowUpdating">
                        <AlternatingRowStyle BackColor="White" />
                        <Columns>
                            <asp:TemplateField>
                                <HeaderTemplate>
                                    <asp:CheckBox ID="CheckBox1" runat="server" AutoPostBack="True" OnCheckedChanged="CheckBox1_CheckedChanged" />
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:CheckBox ID="CheckBox2" runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:CommandField ButtonType="Image" CancelImageUrl="~/Content/images/button_cancel.png"
                                EditImageUrl="~/Content/images/kjots.png" ShowEditButton="True" UpdateImageUrl="~/Content/images/apply.png" />
                            <asp:BoundField DataField="EmpCod" HeaderText="Empresa" ReadOnly="True" SortExpression="EmpCod" />
                            <asp:BoundField DataField="FiscalNFChv" HeaderText="Chave Fiscal" ReadOnly="True"
                                SortExpression="FiscalNFChv" />
                            <asp:BoundField DataField="FiscalItNFSeq" HeaderText="Sequencia" ReadOnly="True"
                                SortExpression="FiscalItNFSeq" />
                            <asp:BoundField DataField="FiscalNFEspec" HeaderText="Espécie" ReadOnly="True" SortExpression="FiscalNFEspec" />
                            <asp:BoundField DataField="FiscalNFSerie" HeaderText="Série" ReadOnly="True" SortExpression="FiscalNFSerie" />
                            <asp:BoundField DataField="FiscalNFNum" HeaderText="Número" ReadOnly="True" SortExpression="FiscalNFNum" />
                            <asp:BoundField DataField="FiscalNFIndMov" HeaderText="Tipo" ReadOnly="True" SortExpression="FiscalNFIndMov" />
                            <asp:TemplateField HeaderText="CST COFINS" SortExpression="FiscalItNFConfTribCofinsCod">
                                <EditItemTemplate>
                                    <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("FiscalItNFConfTribCofinsCod") %>'
                                        Width="33px"></asp:TextBox>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="Label1" runat="server" Text='<%# Bind("FiscalItNFConfTribCofinsCod") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="CST PIS" SortExpression="FiscalItNFConfTribPisCod">
                                <EditItemTemplate>
                                    <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("FiscalItNFConfTribPisCod") %>'
                                        Width="33px"></asp:TextBox>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="Label2" runat="server" Text='<%# Bind("FiscalItNFConfTribPisCod") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Tipo EFD" SortExpression="EFDBaseCalcCredCod">
                                <EditItemTemplate>
                                    <asp:TextBox ID="TextBox3" runat="server" Text='<%# Bind("EFDBaseCalcCredCod") %>'
                                        Width="33px"></asp:TextBox>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="Label3" runat="server" Text='<%# Bind("EFDBaseCalcCredCod") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Nat.Op." SortExpression="NatOpCodEstr">
                                <EditItemTemplate>
                                    <asp:TextBox ID="TextBox4" runat="server" Text='<%# Bind("NatOpCodEstr") %>' Width="53px"></asp:TextBox>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="Label4" runat="server" Text='<%# Bind("NatOpCodEstr") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="FiscalItNFNomeProd" HeaderText="Produto" ReadOnly="True"
                                SortExpression="FiscalItNFNomeProd" />
                            <asp:BoundField DataField="EntNome" HeaderText="Entidade" ReadOnly="True" SortExpression="EntNome" />
                            <asp:BoundField DataField="VALORCONTABIL" DataFormatString="{0:n2}" HeaderText="Valor"
                                ReadOnly="True" SortExpression="VALORCONTABIL" />
                            <asp:TemplateField HeaderText="Base PIS" SortExpression="FiscalItNFValBasePis">
                                <EditItemTemplate>
                                    <asp:TextBox ID="TextBox5" runat="server" 
                                        onkeyup = "mascara(this, mvalor);" Width="70px"
                                        Text='<%# Bind("FiscalItNFValBasePis") %>'></asp:TextBox>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="Label5" runat="server" 
                                        Text='<%# Bind("FiscalItNFValBasePis", "{0:n2}") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Aliq. PIS" SortExpression="FiscalItNFAliqPis">
                                <EditItemTemplate>
                                    <asp:TextBox ID="TextBox6" runat="server" 
                                        onkeyup = "mascara(this, mvalor4);" Width="70px"
                                        Text='<%# Bind("FiscalItNFAliqPis") %>'></asp:TextBox>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="Label6" runat="server" 
                                        Text='<%# Bind("FiscalItNFAliqPis", "{0:n2}") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Valor PIS" SortExpression="FiscalItNFValPis">
                                <EditItemTemplate>
                                    <asp:TextBox ID="TextBox7" runat="server" 
                                        onkeyup = "mascara(this, mvalor);" Width="70px"
                                        Text='<%# Bind("FiscalItNFValPis") %>'></asp:TextBox>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="Label7" runat="server" 
                                        Text='<%# Bind("FiscalItNFValPis", "{0:n2}") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Valor Recuperado PIS" 
                                SortExpression="FiscalItNFValPisRec">
                                <EditItemTemplate>
                                    <asp:TextBox ID="TextBox8" runat="server" 
                                        onkeyup = "mascara(this, mvalor);" Width="70px"
                                        Text='<%# Bind("FiscalItNFValPisRec") %>'></asp:TextBox>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="Label8" runat="server" 
                                        Text='<%# Bind("FiscalItNFValPisRec", "{0:n2}") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Base COFINS" 
                                SortExpression="FiscalItNFValBaseCofins">
                                <EditItemTemplate>
                                    <asp:TextBox ID="TextBox9" runat="server" 
                                        onkeyup = "mascara(this, mvalor);" Width="70px"
                                        Text='<%# Bind("FiscalItNFValBaseCofins") %>'></asp:TextBox>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="Label9" runat="server" 
                                        Text='<%# Bind("FiscalItNFValBaseCofins", "{0:n2}") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Aliq. COFINS" 
                                SortExpression="FiscalItNFAliqCofins">
                                <EditItemTemplate>
                                    <asp:TextBox ID="TextBox10" runat="server"
                                        onkeyup = "mascara(this, mvalor4);" Width="70px"
                                        Text='<%# Bind("FiscalItNFAliqCofins") %>'></asp:TextBox>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="Label10" runat="server" 
                                        Text='<%# Bind("FiscalItNFAliqCofins", "{0:n2}") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Valor COFINS" 
                                SortExpression="FiscalItNFValCofins">
                                <EditItemTemplate>
                                    <asp:TextBox ID="TextBox11" runat="server" 
                                        onkeyup = "mascara(this, mvalor);" Width="70px"
                                        Text='<%# Bind("FiscalItNFValCofins") %>'></asp:TextBox>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="Label11" runat="server" 
                                        Text='<%# Bind("FiscalItNFValCofins", "{0:n2}") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Valor Recuperado COFINS" 
                                SortExpression="FiscalItNFValCofinsRec">
                                <EditItemTemplate>
                                    <asp:TextBox ID="TextBox12" runat="server" 
                                        onkeyup = "mascara(this, mvalor);" Width="70px"
                                        Text='<%# Bind("FiscalItNFValCofinsRec") %>'></asp:TextBox>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="Label12" runat="server" 
                                        Text='<%# Bind("FiscalItNFValCofinsRec", "{0:n2}") %>'></asp:Label>
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
                    </div>
                    <br />
                    <asp:SqlDataSource ID="FiscalNFs" runat="server" ConnectionString="<%$ ConnectionStrings:Apolo10ConnectionString %>"
                        SelectCommand="USER_Consulta_EFD_PisCofins_Blocos_C_D" SelectCommandType="StoredProcedure"
                        UpdateCommand="UPDATE FISCAL_ITEM_NF SET 
FiscalItNFConfTribPisCod = CASE WHEN @FiscalItNFConfTribPisCod = '00' THEN NULL ELSE @FiscalItNFConfTribPisCod END, 
FiscalItNFConfTribCofinsCod = CASE WHEN @FiscalItNFConfTribCofinsCod = '00' THEN NULL ELSE @FiscalItNFConfTribCofinsCod END, 
NatOpCodEstr = @NatOpCodEstr, 
EFDBaseCalcCredCod = CASE WHEN @EFDBaseCalcCredCod = '00' THEN NULL ELSE @EFDBaseCalcCredCod END
WHERE (EmpCod = @EmpCod) AND (FiscalNFChv = @FiscalNFChv) AND (FiscalItNFSeq = @FiscalItNFSeq)">
                        <SelectParameters>
                            <asp:ControlParameter ControlID="DropDownList1" Name="Opcao" PropertyName="SelectedValue"
                                DefaultValue="" />
                            <asp:ControlParameter ControlID="Calendar1" Name="vDataIni" PropertyName="SelectedDate"
                                DefaultValue="" Type="String" />
                            <asp:ControlParameter ControlID="Calendar2" Name="vDataFinal" PropertyName="SelectedDate"
                                Type="String" />
                            <asp:ControlParameter ControlID="TextBox5" ConvertEmptyStringToNull="False" Name="NumeroNF"
                                PropertyName="Text" Type="String" />
                            <asp:ControlParameter ControlID="TextBox6" ConvertEmptyStringToNull="False" Name="Entidade"
                                PropertyName="Text" Type="String" />
                            <asp:ControlParameter ControlID="TextBox7" ConvertEmptyStringToNull="False" Name="Produto"
                                PropertyName="Text" Type="String" />
                        </SelectParameters>
                        <UpdateParameters>
                            <asp:ControlParameter ControlID="GridView1" Name="FiscalItNFConfTribPisCod" PropertyName="SelectedValue" />
                            <asp:ControlParameter ControlID="GridView1" Name="FiscalItNFConfTribCofinsCod" PropertyName="SelectedValue" />
                            <asp:ControlParameter ControlID="GridView1" Name="NatOpCodEstr" PropertyName="SelectedValue" />
                            <asp:ControlParameter ControlID="GridView1" Name="EFDBaseCalcCredCod" PropertyName="SelectedValue" />
                            <asp:ControlParameter ControlID="GridView1" Name="EmpCod" PropertyName="SelectedValue" />
                            <asp:ControlParameter ControlID="GridView1" Name="FiscalNFChv" PropertyName="SelectedValue" />
                            <asp:ControlParameter ControlID="GridView1" Name="FiscalItNFSeq" PropertyName="SelectedValue" />
                        </UpdateParameters>
                    </asp:SqlDataSource>
                    <asp:SqlDataSource ID="FiscalNFsGeral" runat="server" ConnectionString="<%$ ConnectionStrings:Apolo10ConnectionString %>"
                        SelectCommand="USER_Consulta_EFD_PisCofins_Nfs_Geral" SelectCommandType="StoredProcedure"
                        
                        UpdateCommand="UPDATE FISCAL_ITEM_NF SET 
FiscalItNFConfTribPisCod = CASE WHEN @FiscalItNFConfTribPisCod = '00' THEN NULL ELSE @FiscalItNFConfTribPisCod END, 
FiscalItNFConfTribCofinsCod = CASE WHEN @FiscalItNFConfTribCofinsCod = '00' THEN NULL ELSE @FiscalItNFConfTribCofinsCod END, 
NatOpCodEstr = @NatOpCodEstr, 
EFDBaseCalcCredCod = CASE WHEN @EFDBaseCalcCredCod = '00' THEN NULL ELSE @EFDBaseCalcCredCod END
WHERE (EmpCod = @EmpCod) AND (FiscalNFChv = @FiscalNFChv) AND (FiscalItNFSeq = @FiscalItNFSeq) and 1 = 5">
                        <SelectParameters>
                            <asp:ControlParameter ControlID="DropDownList1" Name="Opcao" PropertyName="SelectedValue"
                                DefaultValue="" />
                            <asp:ControlParameter ControlID="Calendar1" Name="vDataIni" PropertyName="SelectedDate"
                                DefaultValue="" Type="String" />
                            <asp:ControlParameter ControlID="Calendar2" Name="vDataFinal" PropertyName="SelectedDate"
                                Type="String" />
                            <asp:ControlParameter ControlID="TextBox5" ConvertEmptyStringToNull="False" Name="NumeroNF"
                                PropertyName="Text" Type="String" />
                            <asp:ControlParameter ControlID="TextBox6" ConvertEmptyStringToNull="False" Name="Entidade"
                                PropertyName="Text" Type="String" />
                            <asp:ControlParameter ControlID="TextBox7" ConvertEmptyStringToNull="False" Name="Produto"
                                PropertyName="Text" Type="String" />
                        </SelectParameters>
                        <UpdateParameters>
                            <asp:Parameter Name="FiscalItNFConfTribPisCod" />
                            <asp:Parameter Name="FiscalItNFConfTribCofinsCod" />
                            <asp:Parameter Name="NatOpCodEstr" />
                            <asp:Parameter Name="EFDBaseCalcCredCod" />
                            <asp:Parameter Name="EmpCod" />
                            <asp:Parameter Name="FiscalNFChv" />
                            <asp:Parameter Name="FiscalItNFSeq" />
                        </UpdateParameters>
                    </asp:SqlDataSource>
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
