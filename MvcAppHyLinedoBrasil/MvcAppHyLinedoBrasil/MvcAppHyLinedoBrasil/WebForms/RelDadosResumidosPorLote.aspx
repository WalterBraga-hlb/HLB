<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RelDadosResumidosPorLote.aspx.cs" Inherits="MvcAppHyLinedoBrasil.WebForms.RelDadosResumidosPorLote" %>

<%@ Register assembly="CrystalDecisions.Web, Version=13.0.2000.0, Culture=neutral, PublicKeyToken=692fbea5521e1304" namespace="CrystalDecisions.Web" tagprefix="CR" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                
                <CR:CrystalReportViewer ID="CrystalReportViewer1" runat="server" 
                    AutoDataBind="True" GroupTreeImagesFolderUrl="" Height="50px" 
                    ReportSourceID="DadosResumidosPorLote" ToolbarImagesFolderUrl="" 
                    ToolPanelWidth="200px" Width="350px" />
                <CR:CrystalReportSource ID="DadosResumidosPorLote" runat="server">
                    <Report FileName="DadosResumidosPorLote.rpt">
        <DataSources>
            <CR:DataSourceRef DataSourceID="SqlDataSource1" 
                TableName="VW_DADOS_RESUMIDO_LOTES" />
        </DataSources>
                    </Report>
                </CR:CrystalReportSource>
                <asp:SqlDataSource ID="SqlDataSource1" runat="server" 
                    ConnectionString="<%$ ConnectionStrings:Oracle %>" 
                    ProviderName="<%$ ConnectionStrings:Oracle.ProviderName %>" 
                    SelectCommand="SELECT * FROM &quot;VW_DADOS_RESUMIDO_LOTES&quot;">
                </asp:SqlDataSource>
                
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    </form>
</body>
</html>
