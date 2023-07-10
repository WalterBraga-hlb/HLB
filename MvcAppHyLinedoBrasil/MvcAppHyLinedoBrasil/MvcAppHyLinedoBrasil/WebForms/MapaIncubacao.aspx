<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MapaIncubacao.aspx.cs" Inherits="MvcAppHyLinedoBrasil.WebForms.MapaIncubacao" %>

<%@ Register assembly="Microsoft.ReportViewer.WebForms, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a" namespace="Microsoft.Reporting.WebForms" tagprefix="rsweb" %>

<%@ Register assembly="CrystalDecisions.Web, Version=13.0.4000.0, Culture=neutral, PublicKeyToken=692fbea5521e1304" namespace="CrystalDecisions.Web" tagprefix="CR" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Mapa da Incubação</title>
    <link href="../Content/icons/logo_hyline.ico" rel="Shortcut Icon" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                
                <CR:CrystalReportViewer ID="CrystalReportViewer1" runat="server" 
                    AutoDataBind="True" 
                    GroupTreeImagesFolderUrl="" Height="50px" ReportSourceID="MapaIncubacaoSource" 
                    ReuseParameterValuesOnRefresh="True" ToolbarImagesFolderUrl="" 
                    ToolPanelView="None" ToolPanelWidth="200px" Width="350px" 
                    EnableParameterPrompt="False" EnableDatabaseLogonPrompt="False" />
                <CR:CrystalReportSource ID="MapaIncubacaoSource" runat="server" 
                    onload="MapaIncubacaoSource_Load">
                    <Report FileName="MapaIncubacaoCrystalReport.rpt">
                        <DataSources>
                            <CR:DataSourceRef DataSourceID="SqlDataSource1" TableName="MapaIncubacao;1" />
                            <CR:DataSourceRef DataSourceID="SqlDataSource2" 
                                ReportName="MapaIncubacaoDetalhado" TableName="HATCHERY_EGG_DATA" />
                        </DataSources>
                        <Parameters>
                            <CR:ControlParameter ControlID="Label1" ConvertEmptyStringToNull="False" 
                                DefaultValue="" Name="@pLocal" PropertyName="Text" ReportName="" />
                            <CR:ControlParameter ControlID="Label2" ConvertEmptyStringToNull="False" 
                                DefaultValue="" Name="@pSetDate" PropertyName="Text" ReportName="" />
                        </Parameters>
                    </Report>
                </CR:CrystalReportSource>
                <asp:SqlDataSource ID="SqlDataSource1" runat="server" 
                    ConnectionString="<%$ ConnectionStrings:LayoutDb %>" 
                    SelectCommand="MapaIncubacao" SelectCommandType="StoredProcedure">
                    <SelectParameters>
                        <asp:SessionParameter DefaultValue="" Name="pLocal" SessionField="hatchLoc" 
                            Type="String" />
                        <asp:SessionParameter Name="pSetDate" SessionField="setDate" Type="DateTime" />
                    </SelectParameters>
                </asp:SqlDataSource>
                <asp:SqlDataSource ID="SqlDataSource2" runat="server" 
                    ConnectionString="<%$ ConnectionStrings:LayoutDb %>" 
                    SelectCommand="SELECT * FROM [HATCHERY_EGG_DATA]"></asp:SqlDataSource>
                <asp:Label ID="Label1" runat="server" Text="Label" Visible="False"></asp:Label>
                <asp:Label ID="Label2" runat="server" Text="Label" Visible="False"></asp:Label>
            </ContentTemplate>
        </asp:UpdatePanel>
    
    </div>
    </form>
</body>
</html>
