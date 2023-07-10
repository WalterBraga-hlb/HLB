<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ProgDiarioTransp.aspx.cs" Inherits="MvcAppHyLinedoBrasil.WebForms.ProgDiarioTransp" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Programação Diária de Transportes e Direcionamento de Clientes</title>
    <%--<link href="../Content/icons/logo_hyline.ico" rel="Shortcut Icon" type="text/css" />--%>
    <style type="text/css">
        .panel
        {
            padding: 10px 10px 10px 10px;
            background-color: #fff;
            margin-bottom: 20px;
            _height: 1px; /* only IE6 applies CSS properties starting with an underscore */
        }
        .main
        {
            padding: 20px 30px 15px 30px;
            background-color: #fff;
            margin-bottom: 20px;
            _height: 1px; /* only IE6 applies CSS properties starting with an underscore */
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
            <div id="dvInfo" style="text-align: center; width: 98.5%; height:100%;" class="panel">
                <asp:Panel ID="Panel2" runat="server" Width="100%" HorizontalAlign="Center" 
                    Height="100%">
                    <table width="100%" style="height:100%">
                        <tr>
                            <td rowspan="3">
                                <a href="../Home/Index">
                                    <asp:Image ID="imgLogo" runat="server" 
                                    ImageUrl="../Content/images/logo.png" Height="48px" Width="121px" 
                                    BorderWidth="0px" />
                                </a>
                                <asp:Label ID="Label16" runat="server" Font-Bold="True" 
                                    Text="Selecione a data abaixo:" Font-Size="XX-Small"></asp:Label>
                                &nbsp;<asp:TextBox ID="txtDataProgramacao" runat="server" 
                                    ontextchanged="txtDataProgramacao_TextChanged" AutoPostBack="True" 
                                    Font-Bold="True" Width="75px"></asp:TextBox>
                                <asp:MaskedEditExtender ID="txtDataProgramacao_MaskedEditExtender" 
                                    runat="server" Century="2000" CultureAMPMPlaceholder="" 
                                    CultureCurrencySymbolPlaceholder="" CultureDateFormat="" 
                                    CultureDatePlaceholder="" CultureDecimalPlaceholder="" 
                                    CultureThousandsPlaceholder="" CultureTimePlaceholder="" Enabled="True" 
                                    Mask="99/99/9999" MaskType="Date" PromptCharacter=" " 
                                    TargetControlID="txtDataProgramacao">
                                </asp:MaskedEditExtender>
                                <asp:CalendarExtender ID="txtDataProgramacao_CalendarExtender" runat="server" 
                                    Enabled="True" Format="dd/MM/yyyy" TargetControlID="txtDataProgramacao">
                                </asp:CalendarExtender>
                                <br />
                                <asp:Label ID="Label18" runat="server" Font-Bold="True" 
                                    Text="Selecione a Empresa Transportadora:" Font-Size="XX-Small"></asp:Label>
                                &nbsp;<asp:DropDownList ID="ddlEmpresaTransportadora" runat="server" 
                                    AutoPostBack="True">
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label ID="Label5" runat="server" Font-Bold="True" Font-Size="Medium" Font-Underline="False"
                                    Text="PROGRAMAÇÃO DIÁRIA DE TRANSPORTES E DIRECIONAMENTO DE CLIENTES - " 
                                    style="font-size: xx-small"></asp:Label>
                                <asp:Label ID="lblDataNasc0" runat="server" Font-Bold="True" 
                                    Font-Size="Medium" Font-Underline="False" 
                                    Text="Data de Nascimento" style="font-size: xx-small"></asp:Label>
                                &nbsp;<asp:Label ID="lblDataNasc1" runat="server" Font-Bold="True" 
                                    Font-Size="Medium" Font-Underline="False" ForeColor="Red" 
                                    Text="Data de Nascimento" style="font-size: xx-small"></asp:Label>
                                <br />
                                <asp:Label ID="lblMensagem3" runat="server" 
                                    Style="font-weight: 700; color: #FF3300" Visible="False"></asp:Label>
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
                <asp:UpdateProgress ID="UpdateProgress1" runat="server" 
                    AssociatedUpdatePanelID="UpdatePanel1">
                    <ProgressTemplate>
                        <strong>CARREGANDO...</strong>&nbsp;&nbsp;
                        <asp:Image ID="Image1" runat="server" ImageUrl="~/Content/images/ajax-loading.gif" />
                    </ProgressTemplate>
                </asp:UpdateProgress>
                <asp:Panel ID="Panel1" runat="server" Width="100%" HorizontalAlign="Center" Style="text-align: left">
                    <asp:ImageButton ID="imgbRefresh" runat="server" 
                        ImageUrl="~/Content/images/Gnome-View-Refresh-32.png" 
                        onclick="imgbRefresh_Click" />
                    <asp:Label ID="lblAtualizarDados" runat="server" Text="Atualizar Dados">
                    </asp:Label>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                    <asp:ImageButton ID="imgbRefreshCHIC" runat="server" 
                        ImageUrl="~/Content/images/packages2.png" 
                        onclick="imgbRefreshCHIC_Click" />
                    <asp:Label ID="lblRefreshCHIC" runat="server" Text="Atualizar Dados de Transporte no CHIC">
                    </asp:Label>
                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                    <asp:ImageButton ID="imgbCopiaDadosManuais" runat="server" 
                        ImageUrl="~/Content/images/copy.png" 
                        onclick="imgbCopiaDadosManuais_Click" />
                    <asp:Label ID="lblCopiaDadosManuais" runat="server" Text="Copiar dados Manuais para o dia">
                    </asp:Label>
                    <asp:TextBox ID="txtDataProximoDia" runat="server" 
                        AutoPostBack="True" 
                        Font-Bold="True" Width="75px"></asp:TextBox>
                    <asp:MaskedEditExtender ID="mtxtDataProximoDia" 
                        runat="server" Century="2000" CultureAMPMPlaceholder="" 
                        CultureCurrencySymbolPlaceholder="" CultureDateFormat="" 
                        CultureDatePlaceholder="" CultureDecimalPlaceholder="" 
                        CultureThousandsPlaceholder="" CultureTimePlaceholder="" Enabled="True" 
                        Mask="99/99/9999" MaskType="Date" PromptCharacter=" " 
                        TargetControlID="txtDataProximoDia">
                    </asp:MaskedEditExtender>
                    <asp:CalendarExtender ID="etxtDataProximoDia" runat="server" 
                        Enabled="True" Format="dd/MM/yyyy" TargetControlID="txtDataProximoDia">
                    </asp:CalendarExtender>
                    <br />
                    <asp:Button ID="btnModoImpressao" runat="server" Text="Modo Impressão" 
                        onclick="btnModoImpressao_Click" Font-Size="6pt" Visible="False" />
                    <asp:Button ID="btnModoCompleto" runat="server" Text="Modo Completo" 
                        onclick="btnModoCompleto_Click" Font-Size="6pt" Visible="False" />
                    <asp:Button ID="btnGerarExcel" runat="server" Text="Gerar Excel" 
                        Font-Size="6pt" onclick="btnGerarExcel_Click" />
                    <asp:LinkButton ID="lkbDownload" runat="server" 
                        OnClick="lkbDownload_Click" Visible="False">Download</asp:LinkButton>
                    <asp:Label ID="lblMensagem2" runat="server" 
                        Style="font-weight: 700; color: #FF3300" Visible="False"></asp:Label>
                    
                    <asp:GridView ID="GridView1" runat="server" AllowPaging="True" 
                        AllowSorting="True" AutoGenerateColumns="False" CellPadding="4" 
                        DataKeyNames="ID" DataSourceID="ProgDiariaTranspPedidos" ForeColor="#333333" 
                        style="font-size: 6pt; text-align: center; margin-right: 39px;" 
                        Width="100%" onrowediting="GridView1_RowEditing" 
                        ShowHeaderWhenEmpty="True" onrowdatabound="GridView1_RowDataBound" 
                        PageSize="30" onrowdeleted="GridView1_RowDeleted" 
                        onrowdeleting="GridView1_RowDeleting" 
                        onselectedindexchanged="GridView1_SelectedIndexChanged" 
                        ondatabound="GridView1_DataBound" onrowcreated="GridView1_RowCreated">
                        <AlternatingRowStyle BackColor="White" />
                        <Columns>
                            <asp:TemplateField ShowHeader="False">
                                <EditItemTemplate>
                                    <asp:ImageButton ID="imgbSaveItem" runat="server" CausesValidation="True" 
                                        CommandName="Update" ImageUrl="~/Content/images/Sim.png" 
                                        onclick="imgbSaveItem_Click" Text="Update" />
                                    &nbsp;<asp:ImageButton ID="ImageButton2" runat="server" CausesValidation="False" 
                                        CommandName="Cancel" ImageUrl="~/Content/images/Nao.png" Text="Cancel" />
                                </EditItemTemplate>
                                <FooterTemplate>
                                    <asp:Button ID="btnAdd" runat="server" Text="+" 
                                        onclick="btnAdd_Click" />
                                    <asp:Button ID="btnCancel" runat="server" Text="X" 
                                        onclick="btnCancel_Click" />
                                </FooterTemplate>
                                <ItemTemplate>
                                    <asp:ImageButton ID="imgbEditItem" runat="server" CausesValidation="False" 
                                        CommandName="Edit" ImageUrl="~/Content/images/kjots.png" style="height: 16px" 
                                        Text="Edit" onclick="imgbEditItem_Click" />
                                    &nbsp;<asp:ImageButton ID="imgDelete" runat="server" CausesValidation="False" 
                                        CommandName="Select" ImageUrl="~/Content/images/button_cancel.png" 
                                        Text="Delete" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="ID" InsertVisible="False" SortExpression="ID">
                                <EditItemTemplate>
                                    <asp:Label ID="Label1" runat="server" Text='<%# Eval("ID") %>'></asp:Label>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="Label9" runat="server" Text='<%# Bind("ID") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="CHICNum" HeaderText="Nº CHIC" ReadOnly="True" 
                                SortExpression="CHICNum" />
                            <asp:TemplateField HeaderText="Cliente" SortExpression="NomeCliente">
                                <EditItemTemplate>
                                    <asp:Label ID="Label2" runat="server" Text='<%# Eval("NomeCliente") %>'></asp:Label>
                                </EditItemTemplate>
                                <FooterTemplate>
                                    <asp:TextBox ID="txtCliente" runat="server" ControlExtender="txtCliente"
                                        onkeyup="this.value = this.value.toUpperCase();"></asp:TextBox>
                                </FooterTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="Label10" runat="server" Text='<%# Bind("NomeCliente") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="CM.Nº" SortExpression="NumVeiculo">
                                <EditItemTemplate>
                                    <asp:DropDownList ID="ddlNumVeiculo" runat="server" 
                                        SelectedValue='<%# Eval("NumVeiculo") %>' 
                                        DataSourceID="ProgDiariaTranspVeiculosNumVeiculo" DataTextField="NumVeiculo" 
                                        DataValueField="NumVeiculo">
                                    </asp:DropDownList>
                                </EditItemTemplate>
                                <FooterTemplate>
                                    <asp:DropDownList ID="ddlNumVeiculo" runat="server" 
                                        DataSourceID="ProgDiariaTranspVeiculos" DataTextField="NumVeiculo" 
                                        DataValueField="NumVeiculo" >
                                        <asp:ListItem>0</asp:ListItem>
                                        <asp:ListItem>1</asp:ListItem>
                                        <asp:ListItem>2</asp:ListItem>
                                        <asp:ListItem>3</asp:ListItem>
                                        <asp:ListItem>4</asp:ListItem>
                                        <asp:ListItem>5</asp:ListItem>
                                        <asp:ListItem>6</asp:ListItem>
                                        <asp:ListItem>7</asp:ListItem>
                                        <asp:ListItem>8</asp:ListItem>
                                    </asp:DropDownList>
                                    <br />
                                </FooterTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="Label1" runat="server" Text='<%# Bind("NumVeiculo") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Or. Ent." SortExpression="Ordem">
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtOrdem" runat="server" Text='<%# Bind("Ordem") %>' 
                                        Width="15px"></asp:TextBox>
                                    <asp:MaskedEditExtender ID="txtOrdem_MaskedEditExtender" runat="server" 
                                        Century="2000" CultureAMPMPlaceholder="" CultureCurrencySymbolPlaceholder="" 
                                        CultureDateFormat="" CultureDatePlaceholder="" CultureDecimalPlaceholder="" 
                                        CultureThousandsPlaceholder="" CultureTimePlaceholder="" Enabled="True" 
                                        ErrorTooltipCssClass="MaskedEditError" ErrorTooltipEnabled="True" 
                                        Mask="9" MaskType="Number" TargetControlID="txtOrdem"
                                        PromptCharacter=" ">
                                    </asp:MaskedEditExtender>
                                    <asp:MaskedEditValidator ID="MaskedEditValidator1" runat="server" 
                                        InvalidValueMessage="Ordem Inválida!" 
                                        ControlExtender="txtOrdem_MaskedEditExtender" ControlToValidate="txtOrdem"
                                        PromptCharacter=" "></asp:MaskedEditValidator>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="Label17" runat="server" Text='<%# Bind("Ordem") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Quant." SortExpression="Quantidade">
                                <EditItemTemplate>
                                    <asp:Label ID="Label3" runat="server" 
                                        Text='<%# Eval("Quantidade", "{0:N0}") %>'></asp:Label>
                                </EditItemTemplate>
                                <FooterTemplate>
                                    <asp:TextBox ID="txtQtde" runat="server" Width="70px"></asp:TextBox>
                                    <asp:FilteredTextBoxExtender ID="filterQtde" runat="server" Enabled="True"
                                        FilterType="Numbers" TargetControlID="txtQtde" />
                                </FooterTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="Label11" runat="server" 
                                        Text='<%# Bind("Quantidade", "{0:N0}") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Qtde. Caixas" SortExpression="QuantidadeCaixa">
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtQuantidadeCaixa" runat="server" Text='<%# Bind("QuantidadeCaixa") %>'
                                        Width="45px"></asp:TextBox>
                                    <asp:FilteredTextBoxExtender ID="filterQuantidadeCaixa" runat="server" Enabled="True"
                                        FilterType="Numbers" TargetControlID="txtQuantidadeCaixa" />
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="Label18" runat="server" 
                                        Text='<%# Bind("QuantidadeCaixa") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Local Entrega" SortExpression="LocalEntrega">
                                <EditItemTemplate>
                                    <asp:Label ID="Label4" runat="server" Text='<%# Eval("LocalEntrega") %>'></asp:Label>
                                </EditItemTemplate>
                                <FooterTemplate>
                                    <asp:TextBox ID="txtLocalEntrega" runat="server"
                                        onkeyup="this.value = this.value.toUpperCase();"></asp:TextBox>
                                </FooterTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="Label12" runat="server" Text='<%# Bind("LocalEntrega") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="Produto" 
                                SortExpression="Produto" ReadOnly="True" />
                            <asp:TemplateField HeaderText="Linhagem" SortExpression="Linhagem">
                                <EditItemTemplate>
                                    <asp:Label ID="Label5" runat="server" Text='<%# Eval("Linhagem") %>'></asp:Label>
                                </EditItemTemplate>
                                <FooterTemplate>
                                    <asp:TextBox ID="txtLinhagem" runat="server"
                                        onkeyup="this.value = this.value.toUpperCase();" Width="50px"></asp:TextBox>
                                </FooterTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="Label13" runat="server" Text='<%# Bind("Linhagem") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Emb." SortExpression="Embalagem">
                                <EditItemTemplate>
                                    <asp:DropDownList ID="ddlEmbalagem" runat="server" 
                                        SelectedValue='<%# Bind("Embalagem") %>'>
                                        <asp:ListItem> </asp:ListItem>
                                        <asp:ListItem>PL</asp:ListItem>
                                        <asp:ListItem>PA</asp:ListItem>
                                        <asp:ListItem>CARR</asp:ListItem>
                                        <asp:ListItem>CX</asp:ListItem>
                                    </asp:DropDownList>
                                </EditItemTemplate>
                                <FooterTemplate>
                                    <asp:DropDownList ID="ddlEmbalagem" runat="server">
                                        <asp:ListItem> </asp:ListItem>
                                        <asp:ListItem>PL</asp:ListItem>
                                        <asp:ListItem>PA</asp:ListItem>
                                        <asp:ListItem>CARR</asp:ListItem>
                                        <asp:ListItem>CX</asp:ListItem>
                                    </asp:DropDownList>
                                </FooterTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="Label2" runat="server" Text='<%# Bind("Embalagem") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="Debicagem" HeaderText="Deb." ReadOnly="True" 
                                SortExpression="Debicagem" />
                            <asp:BoundField DataField="ValorTotal" DataFormatString="{0:N}" 
                                HeaderText="Valor Total" SortExpression="ValorTotal" ReadOnly="True" />
                            <asp:BoundField DataField="NFNum" HeaderText="Nº NF" SortExpression="NFNum" 
                                ReadOnly="True" />
                            <asp:TemplateField HeaderText="Nasc" SortExpression="LocalNascimento">
                                <EditItemTemplate>
                                    <asp:Label ID="Label7" runat="server" Text='<%# Eval("LocalNascimento") %>'></asp:Label>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="Label15" runat="server" Text='<%# Bind("LocalNascimento") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="TelefoneCliente" HeaderText="FONE" 
                                SortExpression="TelefoneCliente" ReadOnly="True" />
                            <asp:TemplateField HeaderText="Inicio Carreg. Inc. Esper." 
                                SortExpression="InicioCarregamentoEsperado">
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtInicioCarregEsperado" runat="server" 
                                        Text='<%# Bind("InicioCarregamentoEsperado") %>'
                                        Width="45px"></asp:TextBox>
                                    <asp:MaskedEditExtender ID="maskInicioCarregEsperado" runat="server" TargetControlID="txtInicioCarregEsperado"
                                        Mask="99:99" MessageValidatorTip="true" OnFocusCssClass="MaskedEditFocus" OnInvalidCssClass="MaskedEditError"
                                        MaskType="Time" AcceptAMPM="True" ErrorTooltipEnabled="True"
                                        PromptCharacter=" " ClearMaskOnLostFocus="true">
                                    </asp:MaskedEditExtender>
                                </EditItemTemplate>
                                <FooterTemplate>
                                    <asp:TextBox ID="txtInicioCarregEsperado" runat="server" Width="45px"></asp:TextBox>
                                    <asp:MaskedEditExtender ID="maskInicioCarregEsperado" runat="server" 
                                        AcceptAMPM="True" ErrorTooltipEnabled="True" Mask="99:99" MaskType="Time" 
                                        MessageValidatorTip="true" OnFocusCssClass="MaskedEditFocus" 
                                        OnInvalidCssClass="MaskedEditError" TargetControlID="txtInicioCarregEsperado"
                                        PromptCharacter=" ">
                                    </asp:MaskedEditExtender>
                                </FooterTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="Label3" runat="server" 
                                        Text='<%# Bind("InicioCarregamentoEsperado") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Data Entrega" SortExpression="DataEntrega">
                                <EditItemTemplate>
                                    <asp:Label ID="Label6" runat="server" 
                                        Text='<%# Eval("DataEntrega", "{0:d}") %>'></asp:Label>
                                </EditItemTemplate>
                                <FooterTemplate>
                                    <asp:TextBox ID="txtDataEntrega" runat="server" Width="80px"></asp:TextBox>
                                    <asp:MaskedEditExtender ID="txtDataEntrega_MaskedEditExtender" runat="server" 
                                        Century="2000" CultureAMPMPlaceholder="" CultureCurrencySymbolPlaceholder="" 
                                        CultureDateFormat="" CultureDatePlaceholder="" CultureDecimalPlaceholder="" 
                                        CultureThousandsPlaceholder="" CultureTimePlaceholder="" Enabled="True" 
                                        ErrorTooltipCssClass="MaskedEditError" ErrorTooltipEnabled="True" 
                                        Mask="99/99/9999" MaskType="Date" TargetControlID="txtDataEntrega"
                                        PromptCharacter=" " CultureName="pt-BR" ClearTextOnInvalid="True" MessageValidatorTip="False">
                                    </asp:MaskedEditExtender>
                                    <asp:CalendarExtender ID="txtDataEntrega_CalendarExtender" runat="server" 
                                        Enabled="True" TargetControlID="txtDataEntrega" Format="dd/MM/yyyy">
                                    </asp:CalendarExtender>
                                </FooterTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="Label14" runat="server" 
                                        Text='<%# Bind("DataEntrega", "{0:d}") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Cheg. Cliente Esper." 
                                SortExpression="ChegadaClienteEsperado">
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtChegadaClienteEsperado" runat="server" 
                                        Text='<%# Bind("ChegadaClienteEsperado") %>' Width="45px"></asp:TextBox>
                                    <asp:MaskedEditExtender ID="maskChegadaClienteEsperado" runat="server" 
                                        AcceptAMPM="True" ErrorTooltipEnabled="True" Mask="99:99" MaskType="Time" 
                                        MessageValidatorTip="true" OnFocusCssClass="MaskedEditFocus" 
                                        OnInvalidCssClass="MaskedEditError" TargetControlID="txtChegadaClienteEsperado"
                                        PromptCharacter=" ">
                                    </asp:MaskedEditExtender>
                                </EditItemTemplate>
                                <FooterTemplate>
                                    <asp:TextBox ID="txtChegadaClienteEsperado" runat="server" Width="45px"></asp:TextBox>
                                    <asp:MaskedEditExtender ID="maskChegadaClienteEsperado" runat="server" 
                                        AcceptAMPM="True" ErrorTooltipEnabled="True" Mask="99:99" MaskType="Time" 
                                        MessageValidatorTip="true" OnFocusCssClass="MaskedEditFocus" 
                                        OnInvalidCssClass="MaskedEditError" TargetControlID="txtChegadaClienteEsperado"
                                        PromptCharacter=" ">
                                    </asp:MaskedEditExtender>
                                </FooterTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="Label4" runat="server" 
                                        Text='<%# Bind("ChegadaClienteEsperado") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="KM" SortExpression="KM">
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtKM" runat="server" Text='<%# Bind("KM") %>'
                                        Width="45px"></asp:TextBox>
                                    <asp:FilteredTextBoxExtender ID="filterKM" runat="server" Enabled="True"
                                        FilterType="Numbers" TargetControlID="txtKM" />
                                </EditItemTemplate>
                                <FooterTemplate>
                                    <asp:TextBox ID="txtKM" runat="server" Width="45px"></asp:TextBox>
                                    <asp:FilteredTextBoxExtender ID="filterKM" runat="server" Enabled="True" 
                                        FilterType="Numbers" TargetControlID="txtKM" />
                                </FooterTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="Label5" runat="server" Text='<%# Bind("KM") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="NomeRepresentante" HeaderText="Repres." 
                                SortExpression="NomeRepresentante" ReadOnly="True" />
                            <asp:TemplateField HeaderText="Inicio Carreg. Inc. Real" 
                                SortExpression="InicioCarregamentoReal">
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtInicioCarregamentoReal" runat="server" 
                                        Text='<%# Bind("InicioCarregamentoReal") %>' Width="45px"></asp:TextBox>
                                    <asp:MaskedEditExtender ID="maskInicioCarregamentoReal" runat="server" 
                                        AcceptAMPM="True" ErrorTooltipEnabled="True" Mask="99:99" MaskType="Time" 
                                        MessageValidatorTip="true" OnFocusCssClass="MaskedEditFocus" 
                                        OnInvalidCssClass="MaskedEditError" TargetControlID="txtInicioCarregamentoReal"
                                        PromptCharacter=" ">
                                    </asp:MaskedEditExtender>
                                </EditItemTemplate>
                                <FooterTemplate>
                                    <asp:TextBox ID="txtInicioCarregamentoReal" runat="server" Width="45px"></asp:TextBox>
                                    <asp:MaskedEditExtender ID="maskInicioCarregamentoReal" runat="server" 
                                        AcceptAMPM="True" ErrorTooltipEnabled="True" Mask="99:99" MaskType="Time" 
                                        MessageValidatorTip="true" OnFocusCssClass="MaskedEditFocus" 
                                        OnInvalidCssClass="MaskedEditError" TargetControlID="txtInicioCarregamentoReal"
                                        PromptCharacter=" ">
                                    </asp:MaskedEditExtender>
                                </FooterTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="Label6" runat="server" 
                                        Text='<%# Bind("InicioCarregamentoReal") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Cheg. Cliente Real" 
                                SortExpression="ChegadaClienteReal">
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtChegadaClienteReal" runat="server" 
                                        Text='<%# Bind("ChegadaClienteReal") %>' Width="45px"></asp:TextBox>
                                    <asp:MaskedEditExtender ID="maskChegadaClienteReal" runat="server" 
                                        AcceptAMPM="True" ErrorTooltipEnabled="True" Mask="99:99" MaskType="Time" 
                                        MessageValidatorTip="true" OnFocusCssClass="MaskedEditFocus" 
                                        OnInvalidCssClass="MaskedEditError" TargetControlID="txtChegadaClienteReal"
                                        PromptCharacter=" ">
                                    </asp:MaskedEditExtender>
                                </EditItemTemplate>
                                <FooterTemplate>
                                    <asp:TextBox ID="txtChegadaClienteReal" runat="server" Width="45px"></asp:TextBox>
                                    <asp:MaskedEditExtender ID="maskChegadaClienteReal" runat="server" 
                                        AcceptAMPM="True" ErrorTooltipEnabled="True" Mask="99:99" MaskType="Time" 
                                        MessageValidatorTip="true" OnFocusCssClass="MaskedEditFocus" 
                                        OnInvalidCssClass="MaskedEditError" TargetControlID="txtChegadaClienteReal"
                                        PromptCharacter=" ">
                                    </asp:MaskedEditExtender>
                                </FooterTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="Label7" runat="server" Text='<%# Bind("ChegadaClienteReal") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Obs. Transp." SortExpression="Observacao">
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtObs" runat="server" Text='<%# Bind("Observacao") %>' 
                                        TextMode="MultiLine"></asp:TextBox>
                                </EditItemTemplate>
                                <FooterTemplate>
                                    <asp:TextBox ID="txtObs" runat="server" TextMode="MultiLine"></asp:TextBox>
                                </FooterTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="Label8" runat="server" Text='<%# Bind("Observacao") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="ObservacaoCHIC" HeaderText="Obs. CHIC" 
                                ReadOnly="True" SortExpression="ObservacaoCHIC" Visible="False" />
                            <asp:TemplateField HeaderText="Status">
                                <EditItemTemplate>
                                    <asp:Label ID="Label8" runat="server" Text='<%# Eval("Status") %>'></asp:Label>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="Label16" runat="server" Text='<%# Bind("Status") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <EditRowStyle BackColor="#2461BF" />
                        <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                        <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                        <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                        <RowStyle BackColor="#EFF3FB" Font-Size="6pt" />
                        <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                        <SortedAscendingCellStyle BackColor="#F5F7FB" />
                        <SortedAscendingHeaderStyle BackColor="#6D95E1" />
                        <SortedDescendingCellStyle BackColor="#E9EBEF" />
                        <SortedDescendingHeaderStyle BackColor="#4870BE" />
                    </asp:GridView>
                    <asp:SqlDataSource ID="ProgDiariaTranspPedidos" runat="server" 
                        ConnectionString="<%$ ConnectionStrings:HLBAPPConnectionString %>" 
                        DeleteCommand="DELETE FROM [Prog_Diaria_Transp_Pedidos] WHERE [ID] = @original_ID" 
                        InsertCommand="INSERT INTO [Prog_Diaria_Transp_Pedidos] ([DataProgramacao], [CodigoCliente], [NomeCliente], [NumVeiculo], [Quantidade], [LocalEntrega], [Produto], [Linhagem], [Embalagem], [ValorTotal], [NFEspecie], [NFSerie], [NFNum], [CHICNum], [LocalNascimento], [TelefoneCliente], [InicioCarregamentoEsperado], [DataEntrega], [ChegadaClienteEsperado], [KM], [CodigoRepresentante], [NomeRepresentante], [InicioCarregamentoReal], [ChegadaClienteReal], [Observacao], [Status], [ObservacaoCHIC], [Debicagem], [Ordem], [EmpresaTranportador], [Empresa], [QuantidadeCaixa]) VALUES (@DataProgramacao, @CodigoCliente, @NomeCliente, @NumVeiculo, @Quantidade, @LocalEntrega, @Produto, @Linhagem, @Embalagem, @ValorTotal, @NFEspecie, @NFSerie, @NFNum, @CHICNum, @LocalNascimento, @TelefoneCliente, @InicioCarregamentoEsperado, @DataEntrega, @ChegadaClienteEsperado, @KM, @CodigoRepresentante, @NomeRepresentante, @InicioCarregamentoReal, @ChegadaClienteReal, @Observacao, @Status, @ObservacaoCHIC, @Debicagem, @Ordem, @EmpresaTranportador, @Empresa, @QuantidadeCaixa)" 
                        OldValuesParameterFormatString="original_{0}" 
                        SelectCommand="SELECT * FROM [Prog_Diaria_Transp_Pedidos] WHERE ((([DataProgramacao] = @DataProgramacao) AND ([EmpresaTranportador] = @EmpresaTranportador)) or EmpresaTranportador = 'Branco')
ORDER BY [NumVeiculo], [Ordem]" 
                        
                        
                        
                        
                        
                        
                        
                        
                        
                        
                        UpdateCommand="UPDATE [Prog_Diaria_Transp_Pedidos] SET [DataProgramacao] = @DataProgramacao, [CodigoCliente] = @CodigoCliente, [NomeCliente] = @NomeCliente, [NumVeiculo] = @NumVeiculo, [Quantidade] = @Quantidade, [LocalEntrega] = @LocalEntrega, [Produto] = @Produto, [Linhagem] = @Linhagem, [Embalagem] = @Embalagem, [ValorTotal] = @ValorTotal, [NFEspecie] = @NFEspecie, [NFSerie] = @NFSerie, [NFNum] = @NFNum, [CHICNum] = @CHICNum, [LocalNascimento] = @LocalNascimento, [TelefoneCliente] = @TelefoneCliente, [InicioCarregamentoEsperado] = @InicioCarregamentoEsperado, [DataEntrega] = @DataEntrega, [ChegadaClienteEsperado] = @ChegadaClienteEsperado, [KM] = @KM, [CodigoRepresentante] = @CodigoRepresentante, [NomeRepresentante] = @NomeRepresentante, [InicioCarregamentoReal] = @InicioCarregamentoReal, [ChegadaClienteReal] = @ChegadaClienteReal, [Observacao] = @Observacao, [Status] = @Status, [ObservacaoCHIC] = @ObservacaoCHIC, [Debicagem] = @Debicagem, [Ordem] = @Ordem, [EmpresaTranportador] = @EmpresaTranportador, [Empresa] = @Empresa, [QuantidadeCaixa] = @QuantidadeCaixa WHERE [ID] = @original_ID">
                        <DeleteParameters>
                            <asp:Parameter Name="original_ID" Type="Int32" />
                        </DeleteParameters>
                        <InsertParameters>
                            <asp:Parameter Name="DataProgramacao" Type="DateTime" />
                            <asp:Parameter Name="CodigoCliente" Type="String" />
                            <asp:Parameter Name="NomeCliente" Type="String" />
                            <asp:Parameter Name="NumVeiculo" Type="Int32" />
                            <asp:Parameter Name="Quantidade" Type="Int32" />
                            <asp:Parameter Name="LocalEntrega" Type="String" />
                            <asp:Parameter Name="Produto" Type="String" />
                            <asp:Parameter Name="Linhagem" Type="String" />
                            <asp:Parameter Name="Embalagem" Type="String" />
                            <asp:Parameter Name="ValorTotal" Type="Decimal" />
                            <asp:Parameter Name="NFEspecie" Type="String" />
                            <asp:Parameter Name="NFSerie" Type="String" />
                            <asp:Parameter Name="NFNum" Type="String" />
                            <asp:Parameter Name="CHICNum" Type="String" />
                            <asp:Parameter Name="LocalNascimento" Type="String" />
                            <asp:Parameter Name="TelefoneCliente" Type="String" />
                            <asp:Parameter Name="InicioCarregamentoEsperado" Type="String" />
                            <asp:Parameter Name="DataEntrega" Type="DateTime" />
                            <asp:Parameter Name="ChegadaClienteEsperado" Type="String" />
                            <asp:Parameter Name="KM" Type="Int32" />
                            <asp:Parameter Name="CodigoRepresentante" Type="String" />
                            <asp:Parameter Name="NomeRepresentante" Type="String" />
                            <asp:Parameter Name="InicioCarregamentoReal" Type="String" />
                            <asp:Parameter Name="ChegadaClienteReal" Type="String" />
                            <asp:Parameter Name="Observacao" Type="String" />
                            <asp:Parameter Name="Status" Type="String" />
                            <asp:Parameter Name="ObservacaoCHIC" Type="String" />
                            <asp:Parameter Name="Debicagem" Type="String" />
                            <asp:Parameter Name="Ordem" Type="Int32" />
                            <asp:Parameter Name="EmpresaTranportador" Type="String" />
                            <asp:Parameter Name="Empresa" Type="String" />
                            <asp:Parameter Name="QuantidadeCaixa" Type="Int32" />
                        </InsertParameters>
                        <SelectParameters>
                            <asp:ControlParameter ControlID="txtDataProgramacao" Name="DataProgramacao" 
                                PropertyName="Text" Type="DateTime" />
                            <asp:ControlParameter ControlID="ddlEmpresaTransportadora" 
                                Name="EmpresaTranportador" PropertyName="SelectedValue" Type="String" />
                        </SelectParameters>
                        <UpdateParameters>
                            <asp:Parameter Name="DataProgramacao" Type="DateTime" />
                            <asp:Parameter Name="CodigoCliente" Type="String" />
                            <asp:Parameter Name="NomeCliente" Type="String" />
                            <asp:Parameter Name="NumVeiculo" Type="Int32" />
                            <asp:Parameter Name="Quantidade" Type="Int32" />
                            <asp:Parameter Name="LocalEntrega" Type="String" />
                            <asp:Parameter Name="Produto" Type="String" />
                            <asp:Parameter Name="Linhagem" Type="String" />
                            <asp:Parameter Name="Embalagem" Type="String" />
                            <asp:Parameter Name="ValorTotal" Type="Decimal" />
                            <asp:Parameter Name="NFEspecie" Type="String" />
                            <asp:Parameter Name="NFSerie" Type="String" />
                            <asp:Parameter Name="NFNum" Type="String" />
                            <asp:Parameter Name="CHICNum" Type="String" />
                            <asp:Parameter Name="LocalNascimento" Type="String" />
                            <asp:Parameter Name="TelefoneCliente" Type="String" />
                            <asp:Parameter Name="InicioCarregamentoEsperado" Type="String" />
                            <asp:Parameter Name="DataEntrega" Type="DateTime" />
                            <asp:Parameter Name="ChegadaClienteEsperado" Type="String" />
                            <asp:Parameter Name="KM" Type="Int32" />
                            <asp:Parameter Name="CodigoRepresentante" Type="String" />
                            <asp:Parameter Name="NomeRepresentante" Type="String" />
                            <asp:Parameter Name="InicioCarregamentoReal" Type="String" />
                            <asp:Parameter Name="ChegadaClienteReal" Type="String" />
                            <asp:Parameter Name="Observacao" Type="String" />
                            <asp:Parameter Name="Status" Type="String" />
                            <asp:Parameter Name="ObservacaoCHIC" Type="String" />
                            <asp:Parameter Name="Debicagem" Type="String" />
                            <asp:Parameter Name="Ordem" Type="Int32" />
                            <asp:Parameter Name="EmpresaTranportador" Type="String" />
                            <asp:Parameter Name="Empresa" Type="String" />
                            <asp:Parameter Name="QuantidadeCaixa" Type="Int32" />
                            <asp:Parameter Name="original_ID" Type="Int32" />
                        </UpdateParameters>
                    </asp:SqlDataSource>
                    <asp:SqlDataSource ID="ProgDiariaTranspVeiculosNumVeiculo" runat="server" 
                        ConnectionString="<%$ ConnectionStrings:HLBAPPConnectionString %>" 
                        DeleteCommand="DELETE FROM [Prog_Diaria_Transp_Veiculos] WHERE [ID] = @ID" 
                        
                        
                        InsertCommand="INSERT INTO [Prog_Diaria_Transp_Veiculos] ([DataProgramacao], [NumVeiculo], [Placa], [Motorista01], [Motorista02], [QuantidadeTotal], [QuantidadePorCaixa], [QunatidadeCaixa], [ValorTotal]) VALUES (@DataProgramacao, @NumVeiculo, @Placa, @Motorista01, @Motorista02, @QuantidadeTotal, @QuantidadePorCaixa, @QunatidadeCaixa, @ValorTotal)" SelectCommand="SELECT distinct NumVeiculo FROM [Prog_Diaria_Transp_Veiculos] WHERE (([DataProgramacao] = @DataProgramacao
and EmpresaTranportador = @EmpresaTranportador) or EmpresaTranportador = 'Branco')
ORDER BY [NumVeiculo]" 
                        
                        
                        UpdateCommand="UPDATE [Prog_Diaria_Transp_Veiculos] SET [DataProgramacao] = @DataProgramacao, [NumVeiculo] = @NumVeiculo, [Placa] = @Placa, [Motorista01] = @Motorista01, [Motorista02] = @Motorista02, [QuantidadeTotal] = @QuantidadeTotal, [QuantidadePorCaixa] = @QuantidadePorCaixa, [QunatidadeCaixa] = @QunatidadeCaixa, [ValorTotal] = @ValorTotal WHERE [ID] = @ID">
                        <DeleteParameters>
                            <asp:Parameter Name="ID" Type="Int32" />
                        </DeleteParameters>
                        <InsertParameters>
                            <asp:Parameter Name="DataProgramacao" Type="DateTime" />
                            <asp:Parameter Name="NumVeiculo" Type="Int32" />
                            <asp:Parameter Name="Placa" Type="String" />
                            <asp:Parameter Name="Motorista01" Type="String" />
                            <asp:Parameter Name="Motorista02" Type="String" />
                            <asp:Parameter Name="QuantidadeTotal" Type="Int32" />
                            <asp:Parameter Name="QuantidadePorCaixa" Type="Int32" />
                            <asp:Parameter Name="QunatidadeCaixa" Type="Int32" />
                            <asp:Parameter Name="ValorTotal" Type="Decimal" />
                        </InsertParameters>
                        <SelectParameters>
                            <asp:ControlParameter ControlID="txtDataProgramacao" Name="DataProgramacao" 
                                PropertyName="Text" Type="DateTime" />
                            <asp:ControlParameter ControlID="ddlEmpresaTransportadora" 
                                Name="EmpresaTranportador" PropertyName="SelectedValue" />
                        </SelectParameters>
                        <UpdateParameters>
                            <asp:Parameter Name="DataProgramacao" Type="DateTime" />
                            <asp:Parameter Name="NumVeiculo" Type="Int32" />
                            <asp:Parameter Name="Placa" Type="String" />
                            <asp:Parameter Name="Motorista01" Type="String" />
                            <asp:Parameter Name="Motorista02" Type="String" />
                            <asp:Parameter Name="QuantidadeTotal" Type="Int32" />
                            <asp:Parameter Name="QuantidadePorCaixa" Type="Int32" />
                            <asp:Parameter Name="QunatidadeCaixa" Type="Int32" />
                            <asp:Parameter Name="ValorTotal" Type="Decimal" />
                            <asp:Parameter Name="ID" Type="Int32" />
                        </UpdateParameters>
                    </asp:SqlDataSource>
                    <asp:ImageButton ID="imgbAdd" runat="server" 
                        ImageUrl="~/Content/images/add.png" onclick="imgbAdd_Click" />
                    &nbsp;
                    <asp:Label ID="lblAdd" runat="server" Text="Inserir Novo Item">
                    </asp:Label>
                    <br />
                    <br />
                    <asp:Label ID="Label15" runat="server" Font-Bold="True" 
                    Font-Size="XX-Small" Font-Underline="False" ForeColor="Red" 
                    Text="Horário p/ Carregar"></asp:Label>
                    <br />
                    <asp:Label ID="lblMensagem4" runat="server" Style="font-weight: 700; color: #FF3300" Visible="False"></asp:Label>
                    <asp:ListView ID="ListView1" runat="server" DataKeyNames="ID" 
                        DataSourceID="ProgDiariaTranspVeiculos" 
                        onitemupdated="ListView1_ItemUpdated" 
                        onitemdatabound="ListView1_ItemDataBound" style="font-size: 7pt;" 
                        onitemupdating="ListView1_ItemUpdating" OnItemEditing="ListView1_ItemEditing">
                        <AlternatingItemTemplate>
                            <td runat="server" style="background-color: #FFFFFF;color: #284775;"
                                align="center">
                                <table width="100%">
                                    <tr>
                                        <td style="border-bottom-style:solid; border-bottom-width:thin;">
                                            &nbsp;
                                            <asp:Label ID="NumVeiculoLabel" runat="server"
                                                Text='<%# Eval("NumVeiculo") %>' /></u>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="border-bottom-style:solid; border-bottom-width:thin;">
                                            &nbsp;
                                            <asp:Label ID="PlacaLabel" runat="server" Text='<%# Eval("Placa") %>' />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="border-bottom-style:solid; border-bottom-width:thin;">
                                            &nbsp;
                                            <asp:Label ID="Motorista01Label" runat="server" 
                                                Text='<%# Eval("Motorista01") %>' />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="border-bottom-style:solid; border-bottom-width:thin;">
                                            &nbsp;
                                            <asp:Label ID="Motorista02Label" runat="server" 
                                                Text='<%# Eval("Motorista02") %>' />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="border-bottom-style:solid; border-bottom-width:thin;">
                                            &nbsp;
                                            <asp:Label ID="QuantidadeTotalLabel" runat="server" 
                                                Text='<%# String.Format("{0:N0}", Eval("QuantidadeTotal")) %>' />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="border-bottom-style:solid; border-bottom-width:thin;">
                                            &nbsp;
                                            <asp:Label ID="QuantidadePorCaixaLabel" runat="server" 
                                                Text='<%# String.Format("{0:N0}", Eval("QuantidadePorCaixa")) %>' />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="border-bottom-style:solid; border-bottom-width:thin;">
                                            &nbsp;
                                            <asp:Label ID="QunatidadeCaixaLabel" runat="server" 
                                                Text='<%# String.Format("{0:N0}", Eval("QunatidadeCaixa")) %>' />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="border-bottom-style:solid; border-bottom-width:thin;">
                                            &nbsp;
                                            <asp:Label ID="ValorTotalLabel" runat="server" 
                                                Text='<%# String.Format("{0:N2}", Eval("ValorTotal")) %>' />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="border-bottom-style:solid; border-bottom-width:thin;">
                                            &nbsp;
                                            <asp:Label ID="DataEmbarqueLabel" runat="server" 
                                                Text='<%# Eval("DataEmbarque", "{0:d}") %>' />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="border-bottom-style:solid; border-bottom-width:thin;">
                                            &nbsp;
                                            <asp:Label ID="InicioCarregRealLabel" runat="server" 
                                                Text='<%# Eval("InicioCarregamentoReal") %>' />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="border-bottom-style:solid; border-bottom-width:thin;">
                                            &nbsp;
                                            <asp:Label ID="Label23" runat="server" 
                                                Text='<%# Eval("TerminoCarregamentoReal") %>' />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="border-bottom-style:solid; border-bottom-width:thin;">
                                            &nbsp;
                                            <asp:Label ID="lblValorKM" runat="server"
                                                Text='Valor KM' Font-Bold="True"  />
                                            <asp:Label ID="ValorKM" runat="server" 
                                                Text='<%# String.Format("{0:N2}", Eval("ValorKM")) %>' />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="border-bottom-style:solid; border-bottom-width:thin;">
                                            &nbsp;
                                            <asp:ImageButton ID="ImageButton1" runat="server" 
                                                ImageUrl="~/Content/images/kjots.png" CommandName="Edit" />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </AlternatingItemTemplate>
                        <EditItemTemplate>
                            <td runat="server" style="background-color: #999999;">
                                <asp:TextBox ID="DataProgramacaoLabel" runat="server" 
                                    Text='<%# Bind("DataProgramacao") %>' Visible="false" />
                                <table width="100%">
                                    <tr>
                                        <td>
                                            <asp:DropDownList ID="ddlNumVeiculo" runat="server" 
                                                    SelectedValue='<%# Bind("NumVeiculo") %>' Enabled="false">
                                                <asp:ListItem>0</asp:ListItem>
                                                <asp:ListItem>1</asp:ListItem>
                                                <asp:ListItem>2</asp:ListItem>
                                                <asp:ListItem>3</asp:ListItem>
                                                <asp:ListItem>4</asp:ListItem>
                                                <asp:ListItem>5</asp:ListItem>
                                                <asp:ListItem>6</asp:ListItem>
                                                <asp:ListItem>7</asp:ListItem>
                                                <asp:ListItem>8</asp:ListItem>
                                                <asp:ListItem>9</asp:ListItem>
                                                <asp:ListItem>10</asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:TextBox ID="PlacaTextBox" runat="server" Text='<%# Bind("Placa") %>' Width="80px" Visible="false" />
                                            <asp:MaskedEditExtender ID="maskExPlaca" runat="server" 
                                                TargetControlID="PlacaTextBox"
                                                Mask="AAA9C99" OnFocusCssClass="MaskedEditFocus" 
                                                OnInvalidCssClass="MaskedEditError"
                                                MessageValidatorTip="true" InputDirection="LeftToRight" 
                                                ClearTextOnInvalid="True" ClearMaskOnLostFocus="True"
                                                PromptCharacter=" ">
                                            </asp:MaskedEditExtender>
                                            <asp:DropDownList ID="ddlPlacas" runat="server" 
                                                
                                                onselectedindexchanged="ddlPlacasListView_SelectedIndexChanged" AutoPostBack="true">
                                                <asp:ListItem></asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:TextBox ID="Motorista01TextBox" runat="server" Visible="false" Text='<%# Bind("Motorista01") %>' />
                                            <asp:DropDownList ID="ddlMotorista01" runat="server" 
                                                
                                                onselectedindexchanged="ddlMotorista01ListView_SelectedIndexChanged" AutoPostBack="true">
                                                <asp:ListItem></asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:TextBox ID="Motorista02TextBox" runat="server" Visible="false" Text='<%# Bind("Motorista02") %>' />
                                            <asp:DropDownList ID="ddlMotorista02" runat="server" 
                                                
                                                onselectedindexchanged="ddlMotorista02ListView_SelectedIndexChanged" AutoPostBack="true">
                                                <asp:ListItem></asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:TextBox ID="QuantidadeTotalTextBox" runat="server" 
                                                Text='<%# Bind("QuantidadeTotal") %>' Enabled="false" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:TextBox ID="QuantidadePorCaixaTextBox" runat="server" 
                                                Text='<%# Bind("QuantidadePorCaixa") %>' />
                                            <asp:FilteredTextBoxExtender ID="ftxtQuantidadePorCaixa" runat="server" 
                                                TargetControlID="QuantidadePorCaixaTextBox" FilterType="Numbers">
                                            </asp:FilteredTextBoxExtender>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:TextBox ID="QunatidadeCaixaTextBox" runat="server" 
                                                Text='<%# Bind("QunatidadeCaixa") %>' Enabled="false" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:TextBox ID="ValorTotalTextBox" runat="server" 
                                                Text='<%# Bind("ValorTotal") %>' onkeyup = "mascara(this, mvalor);" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:TextBox ID="txtDataEmbarque" runat="server" Width="80px" Text='<%# Bind("DataEmbarque") %>'></asp:TextBox>
                                            <asp:MaskedEditExtender ID="txtDataEmbarque_MaskedEditExtender" runat="server" 
                                                Century="2000" CultureAMPMPlaceholder="" CultureCurrencySymbolPlaceholder="" 
                                                CultureDateFormat="" CultureDatePlaceholder="" CultureDecimalPlaceholder="" 
                                                CultureThousandsPlaceholder="" CultureTimePlaceholder="" Enabled="True" 
                                                ErrorTooltipCssClass="MaskedEditError" ErrorTooltipEnabled="True" 
                                                Mask="99/99/9999" MaskType="Date" TargetControlID="txtDataEmbarque"
                                                PromptCharacter=" " CultureName="pt-BR" ClearTextOnInvalid="True" MessageValidatorTip="False">
                                            </asp:MaskedEditExtender>
                                            <asp:CalendarExtender ID="txtDataEmbarque_CalendarExtender" runat="server" 
                                                Enabled="True" TargetControlID="txtDataEmbarque" Format="dd/MM/yyyy">
                                            </asp:CalendarExtender>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:TextBox ID="ValorKMTextBox" runat="server" 
                                                Text='<%# Bind("ValorKM") %>' onkeyup = "mascara(this, mvalor);" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:ImageButton ID="ImageButton3" runat="server"
                                                onclick="imgbUpdateVeiculoListView_Click"
                                                ImageUrl="~/Content/images/Sim.png" />
                                            <asp:ImageButton ID="ImageButton4" runat="server" CommandName="Cancel" 
                                                ImageUrl="~/Content/images/Nao.png" />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </EditItemTemplate>
                        <EmptyDataTemplate>
                            <table style="background-color: #FFFFFF;border-collapse: collapse;border-color: #999999;border-style:none;border-width:1px;">
                                <tr>
                                    <td>
                                        Não existem dados.</td>
                                </tr>
                            </table>
                        </EmptyDataTemplate>
                        <ItemTemplate>
                            <td runat="server" style="background-color: #D1DDF1;color: #333333; " align="center">
                                <table width="100%">
                                    <tr>
                                        <td style="border-bottom-style:solid; border-bottom-width:thin;">
                                            &nbsp;
                                            <asp:Label ID="lblNumVeiculo" runat="server"
                                                Text='Caminhão:' Font-Bold="True"  />
                                            <asp:Label ID="NumVeiculoLabel" runat="server"
                                                Text='<%# Eval("NumVeiculo") %>' />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="border-bottom-style:solid; border-bottom-width:thin;">
                                            &nbsp;
                                            <asp:Label ID="lblPlaca" runat="server"
                                                Text='Placa:' Font-Bold="True"  />
                                            <asp:Label ID="PlacaLabel" runat="server" Text='<%# Eval("Placa") %>' />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="border-bottom-style:solid; border-bottom-width:thin;">
                                            &nbsp;
                                            <asp:Label ID="lblMotorista01" runat="server"
                                                Text='Motorista 01:' Font-Bold="True" />
                                            <asp:Label ID="Motorista01Label" runat="server" 
                                                Text='<%# Eval("Motorista01") %>' />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="border-bottom-style:solid; border-bottom-width:thin;">
                                            &nbsp;
                                            <asp:Label ID="lblMotorista02" runat="server"
                                                Text='Motorista 02:' Font-Bold="True"  />
                                            <asp:Label ID="Motorista02Label" runat="server" 
                                                Text='<%# Eval("Motorista02") %>' />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="border-bottom-style:solid; border-bottom-width:thin;">
                                            &nbsp;
                                            <asp:Label ID="lblQtdeTotal" runat="server"
                                                Text='Qtde. Total:' Font-Bold="True"  />
                                            <asp:Label ID="QuantidadeTotalLabel" runat="server" 
                                                Text='<%# String.Format("{0:N0}", Eval("QuantidadeTotal")) %>' />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="border-bottom-style:solid; border-bottom-width:thin;">
                                            &nbsp;
                                            <asp:Label ID="lblQuantidadePorCaixa" runat="server"
                                                Text='Qtde. p/ Caixa:' Font-Bold="True"  />
                                            <asp:Label ID="QuantidadePorCaixaLabel" runat="server" 
                                                Text='<%# String.Format("{0:N0}", Eval("QuantidadePorCaixa")) %>' />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="border-bottom-style:solid; border-bottom-width:thin;">
                                            &nbsp;
                                            <asp:Label ID="lblQunatidadeCaixa" runat="server"
                                                Text='Qtde. de Caixa:' Font-Bold="True"  />
                                            <asp:Label ID="QunatidadeCaixaLabel" runat="server" 
                                                Text='<%# String.Format("{0:N0}", Eval("QunatidadeCaixa")) %>' />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="border-bottom-style:solid; border-bottom-width:thin;">
                                            &nbsp;
                                            <asp:Label ID="lblValorTotal" runat="server"
                                                Text='Despesas - R$:' Font-Bold="True"  />
                                            <asp:Label ID="ValorTotalLabel" runat="server" 
                                                Text='<%# String.Format("{0:N2}", Eval("ValorTotal")) %>' />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="border-bottom-style:solid; border-bottom-width:thin;">
                                            &nbsp;
                                            <asp:Label ID="lblDataEmbarque" runat="server"
                                                Text='Data Embarque' Font-Bold="True"  />
                                            <asp:Label ID="DataEmbarqueLabel" runat="server" 
                                                Text='<%# Eval("DataEmbarque", "{0:d}") %>' />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="border-bottom-style:solid; border-bottom-width:thin;">
                                            &nbsp;
                                            <asp:Label ID="lblInicioCarregReal" runat="server"
                                                Text='Início Carreg. Real' Font-Bold="True"  />
                                            <asp:Label ID="InicioCarregRealLabel" runat="server" 
                                                Text='<%# Eval("InicioCarregamentoReal") %>' />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="border-bottom-style:solid; border-bottom-width:thin;">
                                            &nbsp;
                                            <asp:Label ID="lblTerminoCarregReal" runat="server"
                                                Text='Término Carreg. Real' Font-Bold="True"  />
                                            <asp:Label ID="Label23" runat="server" 
                                                Text='<%# Eval("TerminoCarregamentoReal") %>' />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="border-bottom-style:solid; border-bottom-width:thin;">
                                            &nbsp;
                                            <asp:Label ID="lblValorKM" runat="server"
                                                Text='Valor KM' Font-Bold="True"  />
                                            <asp:Label ID="ValorKMLabel" runat="server" 
                                                Text='<%# String.Format("{0:N2}", Eval("ValorKM")) %>' />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="border-bottom-style:solid; border-bottom-width:thin;">
                                            &nbsp;
                                            <asp:ImageButton ID="ImageButton1" runat="server" 
                                                ImageUrl="~/Content/images/kjots.png" CommandName="Edit" />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </ItemTemplate>
                        <LayoutTemplate>
                            <table runat="server" border="1" width="100%" 
                                style="background-color: #FFFFFF;border-collapse: collapse;border-color: #999999;border-style:none;border-width:1px;font-family: Verdana, Arial, Helvetica, sans-serif; font-size:7pt;">
                                <tr ID="itemPlaceholderContainer" runat="server" >
                                    <td ID="itemPlaceholder" runat="server">
                                    </td>
                                </tr>
                            </table>
                            <div style="text-align: center;background-color: #5D7B9D;font-family: Verdana, Arial, Helvetica, sans-serif;color: #FFFFFF;">
                            </div>
                        </LayoutTemplate>
                        <SelectedItemTemplate>
                            <td runat="server" 
                                style="background-color: #E2DED6;font-weight: bold;color: #333333;">
                                Caminhão:
                                <asp:Label ID="NumVeiculoLabel" runat="server" 
                                    Text='<%# Eval("NumVeiculo") %>' />
                                <br />
                                <br />
                                Placa:
                                <asp:Label ID="PlacaLabel" runat="server" Text='<%# Eval("Placa") %>' />
                                <br />
                                <br />
                                Motorista 01:
                                <asp:Label ID="Motorista01Label" runat="server" 
                                    Text='<%# Eval("Motorista01") %>' />
                                <br />
                                <br />
                                Motorista 02:
                                <asp:Label ID="Motorista02Label" runat="server" 
                                    Text='<%# Eval("Motorista02") %>' />
                                <br />
                                <br />
                                Quantidade Total:
                                <asp:Label ID="QuantidadeTotalLabel" runat="server" 
                                    Text='<%# Eval("QuantidadeTotal") %>' />
                                <br />
                                <br />
                                Quantidade Por Caixa:
                                <asp:Label ID="QuantidadePorCaixaLabel" runat="server" 
                                    Text='<%# Eval("QuantidadePorCaixa") %>' />
                                <br />
                                <br />
                                Qunatidade de Caixa:
                                <asp:Label ID="QunatidadeCaixaLabel" runat="server" 
                                    Text='<%# Eval("QunatidadeCaixa") %>' />
                                <br />
                                <br />
                                Valor Total:
                                <asp:Label ID="ValorTotalLabel" runat="server" 
                                    Text='<%# Eval("ValorTotal") %>' />
                                <br />
                                <br />
                                Data Embarque:
                                <asp:Label ID="Label19" runat="server" 
                                    Text='<%# Eval("DataEmbarque", "{0:d}") %>' />
                                <br />
                                <asp:Button ID="EditButton" runat="server" CommandName="Edit" Text="Edit" />
                                <asp:Button ID="DeleteButton" runat="server" CommandName="Delete" 
                                    Text="Delete" />
                            </td>
                        </SelectedItemTemplate>
                    </asp:ListView>
                    <asp:SqlDataSource ID="ProgDiariaTranspVeiculos" runat="server" 
                        ConnectionString="<%$ ConnectionStrings:HLBAPPConnectionString %>" 
                        DeleteCommand="DELETE FROM [Prog_Diaria_Transp_Veiculos] WHERE [ID] = @ID" 
                        InsertCommand="INSERT INTO [Prog_Diaria_Transp_Veiculos] ([DataProgramacao], [NumVeiculo], [Placa], [Motorista01], [Motorista02], [QuantidadeTotal], [QuantidadePorCaixa], [QunatidadeCaixa], [ValorTotal], [DataEmbarque]) VALUES (@DataProgramacao, @NumVeiculo, @Placa, @Motorista01, @Motorista02, @QuantidadeTotal, @QuantidadePorCaixa, @QunatidadeCaixa, @ValorTotal, @DataEmbarque)" 
                        SelectCommand="SELECT * FROM [Prog_Diaria_Transp_Veiculos] WHERE ([DataProgramacao] = @DataProgramacao) 
and EmpresaTranportador = @EmpresaTranportador
ORDER BY [NumVeiculo]" 
                        
                        UpdateCommand="UPDATE [Prog_Diaria_Transp_Veiculos] SET [DataProgramacao] = @DataProgramacao, [NumVeiculo] = @NumVeiculo, [Placa] = @Placa, [EquipCodEstrVeiculo] = @EquipCodEstrVeiculo, [Motorista01] = @Motorista01, [EntCodMotorista01] = @EntCodMotorista01, [EntCodMotorista02] = @EntCodMotorista02, [Motorista02] = @Motorista02, [QuantidadeTotal] = @QuantidadeTotal, [QuantidadePorCaixa] = @QuantidadePorCaixa, [QunatidadeCaixa] = @QunatidadeCaixa, [ValorTotal] = @ValorTotal, [DataEmbarque] = @DataEmbarque WHERE [ID] = @ID">
                        <DeleteParameters>
                            <asp:Parameter Name="ID" Type="Int32" />
                        </DeleteParameters>
                        <InsertParameters>
                            <asp:Parameter Name="DataProgramacao" Type="DateTime" />
                            <asp:Parameter Name="NumVeiculo" Type="Int32" />
                            <asp:Parameter Name="Placa" Type="String" />
                            <asp:Parameter Name="Motorista01" Type="String" />
                            <asp:Parameter Name="Motorista02" Type="String" />
                            <asp:Parameter Name="QuantidadeTotal" Type="Int32" />
                            <asp:Parameter Name="QuantidadePorCaixa" Type="Int32" />
                            <asp:Parameter Name="QunatidadeCaixa" Type="Int32" />
                            <asp:Parameter Name="ValorTotal" Type="Decimal" />
                            <asp:Parameter Name="DataEmbarque" Type="DateTime" />
                        </InsertParameters>
                        <SelectParameters>
                            <asp:ControlParameter ControlID="txtDataProgramacao" Name="DataProgramacao" 
                                PropertyName="Text" Type="DateTime" />
                            <asp:ControlParameter ControlID="ddlEmpresaTransportadora" 
                                Name="EmpresaTranportador" PropertyName="SelectedValue" />
                        </SelectParameters>
                        <UpdateParameters>
                            <asp:Parameter Name="DataProgramacao" Type="DateTime" />
                            <asp:Parameter Name="NumVeiculo" Type="Int32" />
                            <asp:Parameter Name="Placa" Type="String" />
                            <asp:Parameter Name="EquipCodEstrVeiculo" Type="String" />
                            <asp:Parameter Name="Motorista01" Type="String" />
                            <asp:Parameter Name="EntCodMotorista01" Type="String" />
                            <asp:Parameter Name="Motorista02" Type="String" />
                            <asp:Parameter Name="EntCodMotorista02" Type="String" />
                            <asp:Parameter Name="QuantidadeTotal" Type="Int32" />
                            <asp:Parameter Name="QuantidadePorCaixa" Type="Int32" />
                            <asp:Parameter Name="QunatidadeCaixa" Type="Int32" />
                            <asp:Parameter Name="ValorTotal" Type="Decimal" />
                            <asp:Parameter Name="DataEmbarque" Type="DateTime" />
                            <asp:Parameter Name="ID" Type="Int32" />
                        </UpdateParameters>
                    </asp:SqlDataSource>
                    <asp:GridView ID="gdvVeiculos" runat="server" AllowPaging="True" 
                        AllowSorting="True" AutoGenerateColumns="False" CellPadding="4" 
                        DataKeyNames="ID" DataSourceID="ProgDiariaTranspVeiculos0" ForeColor="#333333" 
                        GridLines="None" style="font-size: xx-small; text-align: center;" 
                        ShowHeaderWhenEmpty="True" onrowdatabound="gdvVeiculos_RowDataBound" 
                        onrowediting="gdvVeiculos_RowEditing" 
                        onrowdeleted="gdvVeiculos_RowDeleted" onload="gdvVeiculos_Load" OnRowCancelingEdit="gdvVeiculos_RowCancelingEdit">
                        <AlternatingRowStyle BackColor="White" />
                        <Columns>
                            <asp:TemplateField ShowHeader="False">
                                <EditItemTemplate>
                                    <asp:ImageButton ID="imgbUpdateVeiculo" runat="server" CausesValidation="True" 
                                        ImageUrl="~/Content/images/Sim.png" Text="Update" 
                                        onclick="imgbUpdateVeiculo_Click" />
                                    &nbsp;<asp:ImageButton ID="ImageButton2" runat="server" CausesValidation="False" 
                                        CommandName="Cancel" ImageUrl="~/Content/images/Nao.png" Text="Cancel" />
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:ImageButton ID="ImageButton1" runat="server" CausesValidation="False" 
                                        CommandName="Edit" ImageUrl="~/Content/images/kjots.png" Text="Edit" />
                                    &nbsp;<asp:ImageButton ID="ImageButton2" runat="server" CausesValidation="False" 
                                        CommandName="Delete" ImageUrl="~/Content/images/button_cancel.png" 
                                        Text="Delete" />
                                </ItemTemplate>
                                <FooterTemplate>
                                    <asp:Button ID="btnAdd" runat="server" Text="+" 
                                        onclick="btnAddVeiculoNovo_Click" />
                                    <asp:Button ID="btnCancel" runat="server" Text="X" 
                                        onclick="btnCancelVeiculoNovo_Click" />
                                </FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="ID" InsertVisible="False" SortExpression="ID">
                                <EditItemTemplate>
                                    <asp:Label ID="Label1" runat="server" Text='<%# Eval("ID") %>'></asp:Label>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="Label9" runat="server" Text='<%# Bind("ID") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="DataProgramacao" 
                                SortExpression="DataProgramacao" Visible="False">
                                <EditItemTemplate>
                                    <asp:Label ID="Label2" runat="server" Text='<%# Eval("DataProgramacao") %>'></asp:Label>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="Label10" runat="server" Text='<%# Bind("DataProgramacao") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Nº Carga" SortExpression="NumVeiculo">
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtNumVeiculo" runat="server" Text='<%# Bind("NumVeiculo") %>' 
                                        Width="20px"></asp:TextBox>
                                    <asp:MaskedEditExtender ID="txtNumVeiculo_MaskedEditExtender" runat="server" 
                                        Century="2000" CultureAMPMPlaceholder="" CultureCurrencySymbolPlaceholder="" 
                                        CultureDateFormat="" CultureDatePlaceholder="" CultureDecimalPlaceholder="" 
                                        CultureThousandsPlaceholder="" CultureTimePlaceholder="" Enabled="True" 
                                        ErrorTooltipCssClass="MaskedEditError" ErrorTooltipEnabled="True" 
                                        Mask="99" MaskType="Number" TargetControlID="txtNumVeiculo"
                                        PromptCharacter=" ">
                                    </asp:MaskedEditExtender>
                                    <asp:MaskedEditValidator ID="MaskedEditValidator1" runat="server" 
                                        InvalidValueMessage="Nº da Carga Inválida!" 
                                        ControlExtender="txtNumVeiculo_MaskedEditExtender" ControlToValidate="txtNumVeiculo"
                                        PromptCharacter=" "></asp:MaskedEditValidator>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="Label1" runat="server" Text='<%# Bind("NumVeiculo") %>'></asp:Label>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <asp:TextBox ID="txtNumVeiculo" runat="server" Text='<%# Bind("NumVeiculo") %>' 
                                        Width="20px"></asp:TextBox>
                                    <asp:MaskedEditExtender ID="txtNumVeiculo_MaskedEditExtender" runat="server" 
                                        Century="2000" CultureAMPMPlaceholder="" CultureCurrencySymbolPlaceholder="" 
                                        CultureDateFormat="" CultureDatePlaceholder="" CultureDecimalPlaceholder="" 
                                        CultureThousandsPlaceholder="" CultureTimePlaceholder="" Enabled="True" 
                                        ErrorTooltipCssClass="MaskedEditError" ErrorTooltipEnabled="True" 
                                        Mask="99" MaskType="Number" TargetControlID="txtNumVeiculo"
                                        PromptCharacter=" ">
                                    </asp:MaskedEditExtender>
                                    <asp:MaskedEditValidator ID="MaskedEditValidator1" runat="server" 
                                        InvalidValueMessage="Nº da Carga Inválida!" 
                                        ControlExtender="txtNumVeiculo_MaskedEditExtender" ControlToValidate="txtNumVeiculo"
                                        PromptCharacter=" "></asp:MaskedEditValidator>
                                </FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Tranportadora" SortExpression="Tranportadora">
                                <EditItemTemplate>
                                    <asp:TextBox ID="TransportadoraTextBox" runat="server"
                                        onkeyup="this.value = this.value.toUpperCase();"
                                        Text='<%# Bind("Tranportadora") %>' Visible="False"></asp:TextBox>
                                    <asp:DropDownList ID="ddlTransportadoras" runat="server" AutoPostBack="True" 
                                        DataSourceID="TranportadorasSqlDataSource0" DataTextField="EntNomeFant" 
                                        DataValueField="EntCod" 
                                        onselectedindexchanged="ddlTransportadoras_SelectedIndexChanged" 
                                        SelectedValue='<%# Bind("EntCod") %>'>
                                    </asp:DropDownList>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="Label7" runat="server" Text='<%# Bind("Tranportadora") %>'></asp:Label>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <asp:TextBox ID="TransportadoraTextBox" runat="server"
                                        onkeyup="this.value = this.value.toUpperCase();"
                                        Text='<%# Bind("Tranportadora") %>' Visible="False"></asp:TextBox>
                                    <asp:DropDownList ID="ddlTransportadoras" runat="server" AutoPostBack="True" 
                                        DataSourceID="TranportadorasSqlDataSource0" DataTextField="EntNomeFant" 
                                        DataValueField="EntCod" 
                                        onselectedindexchanged="ddlTransportadoras_Footer_SelectedIndexChanged" 
                                        SelectedValue='<%# Bind("EntCod") %>'>
                                    </asp:DropDownList>
                                </FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Placa" SortExpression="Placa">
                                <EditItemTemplate>
                                    <asp:TextBox ID="PlacaTextBox" runat="server" Text='<%# Bind("Placa") %>' Visible="False" Width="80px" />
                                    <asp:MaskedEditExtender ID="maskExPlaca" runat="server" 
                                        TargetControlID="PlacaTextBox"
                                        Mask="AAA9C99" OnFocusCssClass="MaskedEditFocus" 
                                        OnInvalidCssClass="MaskedEditError"
                                        MessageValidatorTip="true" InputDirection="LeftToRight" 
                                        ClearTextOnInvalid="True" ClearMaskOnLostFocus="True"
                                        PromptCharacter=" ">
                                    </asp:MaskedEditExtender>
                                    <asp:DropDownList ID="ddlPlacas" runat="server" >
                                    </asp:DropDownList>
                                </EditItemTemplate>
                                <FooterTemplate>
                                    <asp:TextBox ID="PlacaTextBox" runat="server" Text='<%# Bind("Placa") %>' 
                                        Width="80px" Visible="False" />
                                    <asp:MaskedEditExtender ID="maskExPlaca" runat="server" 
                                        ClearMaskOnLostFocus="True" ClearTextOnInvalid="True" 
                                        InputDirection="LeftToRight" Mask="AAA9C99" MessageValidatorTip="true" 
                                        OnFocusCssClass="MaskedEditFocus" OnInvalidCssClass="MaskedEditError" 
                                        PromptCharacter=" " TargetControlID="PlacaTextBox">
                                    </asp:MaskedEditExtender>
                                    <asp:DropDownList ID="ddlPlacas" runat="server" SelectedValue='<%# Bind("Placa") %>'>
                                    </asp:DropDownList>
                                </FooterTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="Label2" runat="server" Text='<%# Bind("Placa") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Motorista 01" SortExpression="Motorista01">
                                <EditItemTemplate>
                                    <asp:TextBox ID="Motorista01TextBox" runat="server" Visible="False" 
                                        onkeyup="this.value = this.value.toUpperCase();"
                                        Text='<%# Bind("Motorista01") %>'></asp:TextBox>
                                    <asp:DropDownList ID="ddlMotorista01" runat="server">
                                    </asp:DropDownList>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="Label3" runat="server" Text='<%# Bind("Motorista01") %>'></asp:Label>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <asp:TextBox ID="Motorista01TextBox" runat="server" Visible="False" 
                                        onkeyup="this.value = this.value.toUpperCase();"
                                        Text='<%# Bind("Motorista01") %>'></asp:TextBox>
                                    <asp:DropDownList ID="ddlMotorista01" runat="server" SelectedValue='<%# Bind("Motorista01") %>'>
                                    </asp:DropDownList>
                                </FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Motorista 02" SortExpression="Motorista02">
                                <EditItemTemplate>
                                    <asp:TextBox ID="Motorista02TextBox" runat="server" Visible="False" 
                                        onkeyup="this.value = this.value.toUpperCase();"
                                        Text='<%# Bind("Motorista02") %>'></asp:TextBox>
                                    <asp:DropDownList ID="ddlMotorista02" runat="server">
                                    </asp:DropDownList>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="Label4" runat="server" Text='<%# Bind("Motorista02") %>'></asp:Label>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <asp:TextBox ID="Motorista02TextBox" runat="server" Visible="False" 
                                        onkeyup="this.value = this.value.toUpperCase();"
                                        Text='<%# Bind("Motorista02") %>'></asp:TextBox>
                                    <asp:DropDownList ID="ddlMotorista02" runat="server" SelectedValue='<%# Bind("Motorista02") %>'>
                                    </asp:DropDownList>
                                </FooterTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="QuantidadeTotal" HeaderText="Qtde. Total" 
                                ReadOnly="True" SortExpression="QuantidadeTotal" 
                                DataFormatString="{0:N0}" />
                            <asp:BoundField DataField="QuantidadePorCaixa" HeaderText="Qtde. p/ Caixa" 
                                ReadOnly="True" SortExpression="QuantidadePorCaixa" 
                                DataFormatString="{0:N0}" />
                            <asp:BoundField DataField="QunatidadeCaixa" HeaderText="Qtde. Caixa" 
                                ReadOnly="True" SortExpression="QunatidadeCaixa" 
                                DataFormatString="{0:N0}" />
                            <asp:BoundField DataField="ValorTotal" HeaderText="Valor Total Frete" 
                                ReadOnly="True" SortExpression="ValorTotal" />
                            <asp:BoundField DataField="EmpresaTranportador" 
                                HeaderText="EmpresaTranportador" ReadOnly="True" 
                                SortExpression="EmpresaTranportador" Visible="False" />
                            <asp:TemplateField HeaderText="Carreg. Prev." 
                                SortExpression="InicioCarregamentoEsperado">
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtInicioCarregEsperado" runat="server" 
                                        Text='<%# Bind("InicioCarregamentoEsperado") %>'
                                        Width="45px"></asp:TextBox>
                                    <asp:MaskedEditExtender ID="maskInicioCarregEsperado" runat="server" TargetControlID="txtInicioCarregEsperado"
                                        Mask="99:99" MessageValidatorTip="true" OnFocusCssClass="MaskedEditFocus" OnInvalidCssClass="MaskedEditError"
                                        MaskType="Time" AcceptAMPM="True" ErrorTooltipEnabled="True"
                                        PromptCharacter=" " ClearMaskOnLostFocus="true">
                                    </asp:MaskedEditExtender>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="Label5" runat="server" 
                                        Text='<%# Bind("InicioCarregamentoEsperado") %>'></asp:Label>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <asp:TextBox ID="txtInicioCarregEsperado" runat="server" 
                                        Text='<%# Bind("InicioCarregamentoEsperado") %>'
                                        Width="45px"></asp:TextBox>
                                    <asp:MaskedEditExtender ID="maskInicioCarregEsperado" runat="server" TargetControlID="txtInicioCarregEsperado"
                                        Mask="99:99" MessageValidatorTip="true" OnFocusCssClass="MaskedEditFocus" OnInvalidCssClass="MaskedEditError"
                                        MaskType="Time" AcceptAMPM="True" ErrorTooltipEnabled="True"
                                        PromptCharacter=" " ClearMaskOnLostFocus="true">
                                    </asp:MaskedEditExtender>
                                </FooterTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="InicioCarregamentoReal" HeaderText="Inicio Carreg. Real" ReadOnly="True" SortExpression="InicioCarregamentoReal" />
                            <asp:BoundField DataField="TerminoCarregamentoReal" HeaderText="Término Carreg. Real" ReadOnly="True" SortExpression="TerminoCarregamentoReal" />
                            <asp:TemplateField HeaderText="Ent. NF" SortExpression="HorarioEntregaNF">
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtHorarioEntregaNF" runat="server" 
                                        Text='<%# Bind("HorarioEntregaNF") %>'
                                        Width="45px"></asp:TextBox>
                                    <asp:MaskedEditExtender ID="maskHorarioEntregaNF" runat="server" TargetControlID="txtHorarioEntregaNF"
                                        Mask="99:99" MessageValidatorTip="true" OnFocusCssClass="MaskedEditFocus" OnInvalidCssClass="MaskedEditError"
                                        MaskType="Time" AcceptAMPM="True" ErrorTooltipEnabled="True"
                                        PromptCharacter=" " ClearMaskOnLostFocus="true">
                                    </asp:MaskedEditExtender>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="Label6" runat="server" Text='<%# Bind("HorarioEntregaNF") %>'></asp:Label>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <asp:TextBox ID="txtHorarioEntregaNF" runat="server" 
                                        Text='<%# Bind("HorarioEntregaNF") %>'
                                        Width="45px"></asp:TextBox>
                                    <asp:MaskedEditExtender ID="maskHorarioEntregaNF" runat="server" TargetControlID="txtHorarioEntregaNF"
                                        Mask="99:99" MessageValidatorTip="true" OnFocusCssClass="MaskedEditFocus" OnInvalidCssClass="MaskedEditError"
                                        MaskType="Time" AcceptAMPM="True" ErrorTooltipEnabled="True"
                                        PromptCharacter=" " ClearMaskOnLostFocus="true">
                                    </asp:MaskedEditExtender>
                                </FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Valor / KM" SortExpression="ValorKM">
                                <EditItemTemplate>
                                    <asp:TextBox ID="ValorKMTextBox" runat="server" 
                                        onkeyup = "mascara(this, mvalor);"
                                        Text='<%# Bind("ValorKM") %>' Width="50px"></asp:TextBox>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="Label8" runat="server" Text='<%# Bind("ValorKM", "{0:N2}") %>'></asp:Label>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <asp:TextBox ID="ValorKMTextBox" runat="server" 
                                        onkeyup = "mascara(this, mvalor);"
                                        Text='<%# Bind("ValorKM") %>' Width="50px"></asp:TextBox>
                                </FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Data Embarque" SortExpression="DataEmbarque">
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtDataEmbarque" runat="server" AutoPostBack="True" 
                                        Font-Bold="True" Width="75px" Text='<%# Bind("DataEmbarque", "{0:d}") %>'></asp:TextBox>
                                    <asp:MaskedEditExtender ID="txtDataEmbarque_MaskedEditExtender" 
                                        runat="server" Century="2000" CultureAMPMPlaceholder="" 
                                        CultureCurrencySymbolPlaceholder="" CultureDateFormat="" 
                                        CultureDatePlaceholder="" CultureDecimalPlaceholder="" 
                                        CultureThousandsPlaceholder="" CultureTimePlaceholder="" Enabled="True" 
                                        Mask="99/99/9999" MaskType="Date" PromptCharacter=" " 
                                        TargetControlID="txtDataEmbarque">
                                    </asp:MaskedEditExtender>
                                    <asp:CalendarExtender ID="txtDataEmbarque_CalendarExtender" runat="server" 
                                        Enabled="True" Format="dd/MM/yyyy" TargetControlID="txtDataEmbarque">
                                    </asp:CalendarExtender>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="Label11" runat="server" Text='<%# Bind("DataEmbarque", "{0:d}") %>'></asp:Label>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <asp:TextBox ID="txtDataEmbarque" runat="server" AutoPostBack="True" 
                                        Font-Bold="True" Width="75px" Text='<%# Bind("DataEmbarque", "{0:d}") %>'></asp:TextBox>
                                    <asp:MaskedEditExtender ID="txtDataEmbarque_MaskedEditExtender" 
                                        runat="server" Century="2000" CultureAMPMPlaceholder="" 
                                        CultureCurrencySymbolPlaceholder="" CultureDateFormat="" 
                                        CultureDatePlaceholder="" CultureDecimalPlaceholder="" 
                                        CultureThousandsPlaceholder="" CultureTimePlaceholder="" Enabled="True" 
                                        Mask="99/99/9999" MaskType="Date" PromptCharacter=" " 
                                        TargetControlID="txtDataEmbarque">
                                    </asp:MaskedEditExtender>
                                    <asp:CalendarExtender ID="txtDataEmbarque_CalendarExtender" runat="server" 
                                        Enabled="True" Format="dd/MM/yyyy" TargetControlID="txtDataEmbarque">
                                    </asp:CalendarExtender>
                                </FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Aeroporto Origem" 
                                SortExpression="AeroportoOrigem">
                                <EditItemTemplate>
                                    <asp:DropDownList ID="ddlAeroportoOrigem" runat="server" AutoPostBack="True" 
                                        DataSourceID="AeroportoOrigemSqlDataSource" DataTextField="AeropNome" 
                                        DataValueField="AeropNome" 
                                        SelectedValue='<%# Bind("AeroportoOrigem") %>'>
                                    </asp:DropDownList>
                                </EditItemTemplate>
                                <FooterTemplate>
                                    <asp:DropDownList ID="ddlAeroportoOrigem" runat="server" AutoPostBack="True" 
                                        DataSourceID="AeroportoOrigemSqlDataSource" DataTextField="AeropNome" 
                                        DataValueField="AeropNome" 
                                        SelectedValue='<%# Bind("AeroportoOrigem") %>'>
                                    </asp:DropDownList>
                                </FooterTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="Label12" runat="server" Text='<%# Bind("AeroportoOrigem") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Cheg. Aeroporto" 
                                SortExpression="HorarioChegadaAeroporto">
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtHorarioChegadaAeroporto" runat="server" 
                                        Text='<%# Bind("HorarioChegadaAeroporto") %>'
                                        Width="45px"></asp:TextBox>
                                    <asp:MaskedEditExtender ID="maskHorarioChegadaAeroporto" runat="server" TargetControlID="txtHorarioChegadaAeroporto"
                                        Mask="99:99" MessageValidatorTip="true" OnFocusCssClass="MaskedEditFocus" OnInvalidCssClass="MaskedEditError"
                                        MaskType="Time" AcceptAMPM="True" ErrorTooltipEnabled="True"
                                        PromptCharacter=" " ClearMaskOnLostFocus="true">
                                    </asp:MaskedEditExtender>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="Label13" runat="server" 
                                        Text='<%# Bind("HorarioChegadaAeroporto") %>'></asp:Label>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <asp:TextBox ID="txtHorarioChegadaAeroporto" runat="server" 
                                        Text='<%# Bind("HorarioChegadaAeroporto") %>'
                                        Width="45px"></asp:TextBox>
                                    <asp:MaskedEditExtender ID="maskHorarioChegadaAeroporto" runat="server" TargetControlID="txtHorarioChegadaAeroporto"
                                        Mask="99:99" MessageValidatorTip="true" OnFocusCssClass="MaskedEditFocus" OnInvalidCssClass="MaskedEditError"
                                        MaskType="Time" AcceptAMPM="True" ErrorTooltipEnabled="True"
                                        PromptCharacter=" " ClearMaskOnLostFocus="true">
                                    </asp:MaskedEditExtender>
                                </FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Despachante" SortExpression="Despachante">
                                <EditItemTemplate>
                                    <asp:DropDownList ID="ddlDespachante" runat="server" AutoPostBack="True" 
                                        SelectedValue='<%# Bind("Despachante") %>'>
                                        <asp:ListItem></asp:ListItem>
                                        <asp:ListItem>PALMA</asp:ListItem>
                                        <asp:ListItem>FERMAC</asp:ListItem>
                                    </asp:DropDownList>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="Label14" runat="server" Text='<%# Bind("Despachante") %>'></asp:Label>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <asp:DropDownList ID="ddlDespachante" runat="server" AutoPostBack="True" 
                                        SelectedValue='<%# Bind("Despachante") %>'>
                                        <asp:ListItem></asp:ListItem>
                                        <asp:ListItem>PALMA</asp:ListItem>
                                        <asp:ListItem>FERMAC</asp:ListItem>
                                    </asp:DropDownList>
                                </FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Data Inicio Vazio" SortExpression="DataInicioVazio">
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtDataInicioVazio" runat="server" AutoPostBack="True" 
                                        Font-Bold="True" Width="75px" Text='<%# Bind("DataInicioVazio", "{0:d}") %>'></asp:TextBox>
                                    <asp:MaskedEditExtender ID="txtDataInicioVazio_MaskedEditExtender" 
                                        runat="server" Century="2000" CultureAMPMPlaceholder="" 
                                        CultureCurrencySymbolPlaceholder="" CultureDateFormat="" 
                                        CultureDatePlaceholder="" CultureDecimalPlaceholder="" 
                                        CultureThousandsPlaceholder="" CultureTimePlaceholder="" Enabled="True" 
                                        Mask="99/99/9999" MaskType="Date" PromptCharacter=" " 
                                        TargetControlID="txtDataInicioVazio">
                                    </asp:MaskedEditExtender>
                                    <asp:CalendarExtender ID="txtDataInicioVazio_CalendarExtender" runat="server" 
                                        Enabled="True" Format="dd/MM/yyyy" TargetControlID="txtDataInicioVazio">
                                    </asp:CalendarExtender>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblDataInicio" runat="server" Text='<%# Bind("DataInicioVazio", "{0:d}") %>'></asp:Label>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <asp:TextBox ID="txtDataInicioVazio" runat="server" AutoPostBack="True" 
                                        Font-Bold="True" Width="75px" Text='<%# Bind("DataInicioVazio", "{0:d}") %>'></asp:TextBox>
                                    <asp:MaskedEditExtender ID="txtDataInicioVazio_MaskedEditExtender" 
                                        runat="server" Century="2000" CultureAMPMPlaceholder="" 
                                        CultureCurrencySymbolPlaceholder="" CultureDateFormat="" 
                                        CultureDatePlaceholder="" CultureDecimalPlaceholder="" 
                                        CultureThousandsPlaceholder="" CultureTimePlaceholder="" Enabled="True" 
                                        Mask="99/99/9999" MaskType="Date" PromptCharacter=" " 
                                        TargetControlID="txtDataInicioVazio">
                                    </asp:MaskedEditExtender>
                                    <asp:CalendarExtender ID="txtDataInicioVazio_CalendarExtender" runat="server" 
                                        Enabled="True" Format="dd/MM/yyyy" TargetControlID="txtDataInicioVazio">
                                    </asp:CalendarExtender>
                                </FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Odômetro na Data Embarque" SortExpression="OdometroVeiculoDataEmbarque">
                                <EditItemTemplate>
                                    <asp:TextBox ID="OdometroVeiculoDataEmbarqueTextBox" runat="server" onkeyup="mascara(this, mvalor);" Text='<%# Bind("OdometroVeiculoDataEmbarque") %>' Width="150px"></asp:TextBox>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="OdometroVeiculoDataEmbarqueLabel" runat="server" Text='<%# Bind("OdometroVeiculoDataEmbarque", "{0:N2}") %>'></asp:Label>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <asp:TextBox ID="OdometroVeiculoDataEmbarqueTextBox" runat="server" onkeyup="mascara(this, mvalor);" Text='<%# Bind("OdometroVeiculoDataEmbarque") %>' Width="150px"></asp:TextBox>
                                </FooterTemplate>
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
                    <asp:SqlDataSource ID="ProgDiariaTranspVeiculos0" runat="server" 
                        ConnectionString="<%$ ConnectionStrings:HLBAPPConnectionString %>" 
                        DeleteCommand="DELETE FROM [Prog_Diaria_Transp_Veiculos] WHERE [ID] = @ID" 
                        
                        
                        
                        
                        
                        InsertCommand="INSERT INTO [Prog_Diaria_Transp_Veiculos] ([DataProgramacao], [NumVeiculo], [Placa], [Motorista01], [Motorista02], [QuantidadeTotal], [QuantidadePorCaixa], [QunatidadeCaixa], [ValorTotal], [DataEmbarque], [AeroportoOrigem], [HorarioChegadaAeroporto], [Despachante], [DataInicioVazio]) VALUES (@DataProgramacao, @NumVeiculo, @Placa, @Motorista01, @Motorista02, @QuantidadeTotal, @QuantidadePorCaixa, @QunatidadeCaixa, @ValorTotal, @DataEmbarque, @AeroportoOrigem, @HorarioChegadaAeroporto, @Despachante, @DataInicioVazio)" 
                        SelectCommand="SELECT * FROM [Prog_Diaria_Transp_Veiculos] WHERE (([DataProgramacao] = @DataProgramacao
and EmpresaTranportador = @EmpresaTranportador) or EmpresaTranportador = 'Branco')
ORDER BY [NumVeiculo]" 
                        
                        
                        
                        UpdateCommand="UPDATE [Prog_Diaria_Transp_Veiculos] SET [DataProgramacao] = @DataProgramacao, [NumVeiculo] = @NumVeiculo, [Placa] = @Placa, [Motorista01] = @Motorista01, [Motorista02] = @Motorista02, [QuantidadeTotal] = @QuantidadeTotal, [QuantidadePorCaixa] = @QuantidadePorCaixa, [QunatidadeCaixa] = @QunatidadeCaixa, [ValorTotal] = @ValorTotal, [DataEmbarque] = @DataEmbarque, [AeroportoOrigem] = @AeroportoOrigem, [HorarioChegadaAeroporto] = @HorarioChegadaAeroporto, [Despachante] = @Despachante, [DataInicioVazio] = @DataInicioVazio
WHERE [ID] = @ID">
                        <DeleteParameters>
                            <asp:Parameter Name="ID" Type="Int32" />
                        </DeleteParameters>
                        <InsertParameters>
                            <asp:Parameter Name="DataProgramacao" Type="DateTime" />
                            <asp:Parameter Name="NumVeiculo" Type="Int32" />
                            <asp:Parameter Name="Placa" Type="String" />
                            <asp:Parameter Name="Motorista01" Type="String" />
                            <asp:Parameter Name="Motorista02" Type="String" />
                            <asp:Parameter Name="QuantidadeTotal" Type="Int32" />
                            <asp:Parameter Name="QuantidadePorCaixa" Type="Int32" />
                            <asp:Parameter Name="QunatidadeCaixa" Type="Int32" />
                            <asp:Parameter Name="ValorTotal" Type="Decimal" />
                            <asp:Parameter Name="DataEmbarque" />
                            <asp:Parameter Name="AeroportoOrigem" />
                            <asp:Parameter Name="HorarioChegadaAeroporto" />
                            <asp:Parameter Name="Despachante" />
                            <asp:Parameter Name="DataInicioVazio" />
                        </InsertParameters>
                        <SelectParameters>
                            <asp:ControlParameter ControlID="txtDataProgramacao" Name="DataProgramacao" 
                                PropertyName="Text" Type="DateTime" />
                            <asp:ControlParameter ControlID="ddlEmpresaTransportadora" 
                                Name="EmpresaTranportador" PropertyName="SelectedValue" />
                        </SelectParameters>
                        <UpdateParameters>
                            <asp:Parameter Name="DataProgramacao" Type="DateTime" />
                            <asp:Parameter Name="NumVeiculo" Type="Int32" />
                            <asp:Parameter Name="Placa" Type="String" />
                            <asp:Parameter Name="Motorista01" Type="String" />
                            <asp:Parameter Name="Motorista02" Type="String" />
                            <asp:Parameter Name="QuantidadeTotal" Type="Int32" />
                            <asp:Parameter Name="QuantidadePorCaixa" Type="Int32" />
                            <asp:Parameter Name="QunatidadeCaixa" Type="Int32" />
                            <asp:Parameter Name="ValorTotal" Type="Decimal" />
                            <asp:Parameter Name="DataEmbarque" />
                            <asp:Parameter Name="AeroportoOrigem" />
                            <asp:Parameter Name="HorarioChegadaAeroporto" />
                            <asp:Parameter Name="Despachante" />
                            <asp:Parameter Name="DataInicioVazio" />
                            <asp:Parameter Name="ID" Type="Int32" />
                        </UpdateParameters>
                    </asp:SqlDataSource>
                    <asp:SqlDataSource ID="TranportadorasSqlDataSource0" runat="server" 
                        ConnectionString="<%$ ConnectionStrings:Apolo10ConnectionString %>" 
                        SelectCommand="
select '' EntCod, '(Selecione uma Transportadora)' EntNomeFant, '' USERPlacaVeiculo, '' USEREmpresaControlaTransp
Union
select E.EntCod, E.EntNomeFant, UPPER(E1.USERPlacaVeiculo) USERPlacaVeiculo, E1.USEREmpresaControlaTransp from Apolo10.dbo.ENTIDADE E With(Nolock)
inner join Apolo10.dbo.ENTIDADE1 E1 With(Nolock) on
	E.EntCod = E1.EntCod
where E1.USERParticipaProgTranspWEB = 'Sim' and E1.USEREmpresaControlaTransp = @EmpresaTransp

Union

select Tranportadora, Tranportadora, UPPER(Max(Placa)), EmpresaTranportador from HLBAPP.dbo.Prog_Diaria_Transp_Veiculos
where EmpresaTranportador = @EmpresaTransp
and 0 = (select COUNT(1) from Apolo10.dbo.ENTIDADE E With(Nolock)
			inner join Apolo10.dbo.ENTIDADE1 E1 With(Nolock) on E.EntCod = E1.EntCod
			where E1.USERParticipaProgTranspWEB = 'Sim' 
			and E1.USEREmpresaControlaTransp = Prog_Diaria_Transp_Veiculos.EmpresaTranportador
			and E.EntNomeFant = Prog_Diaria_Transp_Veiculos.Tranportadora)

group by Tranportadora, EmpresaTranportador
order by 1">
                        <SelectParameters>
                            <asp:ControlParameter ControlID="ddlEmpresaTransportadora" Name="EmpresaTransp" 
                                PropertyName="SelectedValue" />
                        </SelectParameters>
                    </asp:SqlDataSource>
                    <asp:SqlDataSource ID="AeroportoOrigemSqlDataSource" runat="server" 
                        ConnectionString="<%$ ConnectionStrings:Apolo10ConnectionString %>" SelectCommand="select '' AeropNome
Union
select AeropNome from AEROPORTO A With(Nolock)
where 0 &lt; (select count(1) from CIDADE C
			where A.CidCod = C.CidCod
			and C.PaisSigla = 'BRA')
order by 1"></asp:SqlDataSource>
                    <br />
                    <asp:ImageButton ID="imgbAddVeiculo" runat="server" 
                        ImageUrl="~/Content/images/add.png" onclick="imgbAddVeiculo_Click" />
                    <asp:Label ID="lblAddVeiculo" runat="server" Text="Inserir Nova Carga"></asp:Label>
                </asp:Panel>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
    </form>
</body>
</html>
