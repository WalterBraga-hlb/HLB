<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RelControleQualidadeCargas.aspx.cs" Inherits="MvcAppHyLinedoBrasil.WebForms.RelControleQualidadeCargas" %>

<%@ Register assembly="CrystalDecisions.Web, Version=13.0.4000.0, Culture=neutral, PublicKeyToken=692fbea5521e1304" namespace="CrystalDecisions.Web" tagprefix="CR" %>

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
                                    ReportSourceID="OrderData" ToolbarImagesFolderUrl="" ToolPanelWidth="200px" 
                                    Width="350px" ToolPanelView="None" EnableParameterPrompt="False" />
                                <CR:CrystalReportSource ID="OrderData" runat="server">
                                    <Report FileName="ControleQualidadeCargas.rpt">
                                        <DataSources>
                                            <CR:DataSourceRef DataSourceID="SqlDataSource1" 
                                                TableName="VU_REL_CONTROLE_CARGA_PINTOS" />
                                        </DataSources>
                                        <Parameters>
                                            <CR:ControlParameter ControlID="Label1" ConvertEmptyStringToNull="False" 
                                                DefaultValue="" Name="@pPedido" PropertyName="Text" ReportName="" />
                                            <CR:ControlParameter ControlID="Label2" ConvertEmptyStringToNull="False" 
                                                DefaultValue="" Name="@pDataNascimento" PropertyName="Text" ReportName="" />
                                        </Parameters>
                                    </Report>
                                </CR:CrystalReportSource>
                                <asp:Label ID="Label1" runat="server" Text="Label" Visible="False"></asp:Label>
                                <asp:Label ID="Label2" runat="server" Text="Label" Visible="False"></asp:Label>
                
                <asp:SqlDataSource ID="SqlDataSource2" runat="server" 
                    ConnectionString="<%$ ConnectionStrings:Oracle %>" 
                    ProviderName="<%$ ConnectionStrings:Oracle.ProviderName %>" 
                    SelectCommand="SELECT * FROM &quot;HATCHERY_VACC_DATA&quot; WHERE ((&quot;CUSTNO&quot; = :CUSTNO) AND (&quot;SET_DATE&quot; = :SET_DATE))">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="Label1" Name="CUSTNO" PropertyName="Text" 
                            Type="String" />
                        <asp:ControlParameter ControlID="Label2" Name="SET_DATE" PropertyName="Text" 
                            Type="DateTime" />
                    </SelectParameters>
                </asp:SqlDataSource>
                
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    </form>
</body>
</html>
