<%@ Page Language="C#" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<script runat="server">

    protected void Page_Load(object sender, EventArgs e)
    {
        LoadChart(this.ProgrammingTable, "prog");
        LoadChart(this.RegionalSales, "sales");
    }
    private void LoadChart(System.Data.DataTable table, string jsName)
    {
        Page.ClientScript.RegisterStartupScript(
            this.GetType(),
            jsName, string.Format("var {0} = {1};", jsName, new Bortosky.Google.Visualization.GoogleDataTable(table).GetJson()), true);
    }
    private System.Data.DataTable ProgrammingTable
    {
        get
        {
            var dt = new System.Data.DataTable();
            dt.Columns.Add("STYLE", typeof(System.String)).Caption = "Programming Style";
            dt.Columns.Add("FUN", typeof(System.Int32)).Caption = "Fun";
            dt.Columns.Add("WORK", typeof(System.Int32)).Caption = "Work";
            dt.Rows.Add(new object[] { "Hand Coding", 30, 200 });
            dt.Rows.Add(new object[] { "Using the .NET Library", 300, 10 });
            dt.Rows.Add(new object[] { "Skipping Visualization", -50, 0 });
            return dt;
        }
    }
    private System.Data.DataTable RegionalSales
    {
        get
        {
            var dt = new System.Data.DataTable();
            dt.Columns.Add("REGION", typeof(System.String)).Caption = "Region";
            dt.Columns.Add("SALES", typeof(System.Int32)).Caption = "Annual Sales";
            dt.Rows.Add(new object[] { "North", 50000 });
            dt.Rows.Add(new object[] { "South", 150000 });
            dt.Rows.Add(new object[] { "East", 130000 });
            dt.Rows.Add(new object[] { "West", 120000 });
            return dt;
        }
    }
</script>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>.NET Visualization Two Visualizations Sample</title>

    <script type="text/javascript" src="http://www.google.com/jsapi"></script>
    <script type="text/javascript">
        google.load("visualization", "1", { "packages": ["corechart"] });
        function drawProgrammingChart() {
            var data = new google.visualization.DataTable(prog, 0.5);
            var chart = new google.visualization.ColumnChart(document.getElementById("divprog"));
            chart.draw(data, {
                title: "Visualization Satisfaction", hAxis: { title: "Programming method" }, vAxis: { title: "Units" }
            });
        }
        function drawSalesChart() {
            var data = new google.visualization.DataTable(sales, 0.5);
            var chart = new google.visualization.PieChart(document.getElementById("divsales"));
            chart.draw(data, {
                title: "Regional Sales", hAxis: { title: "Region" }, vAxis: { title: "2009 Sales" }
            });
        }
        function drawBothCharts() {
            drawProgrammingChart();
            drawSalesChart();
        }
        google.setOnLoadCallback(drawBothCharts);
    </script>
    <style type="text/css">
        body
        {
            font-family: Arial, Helvetica, sans-serif;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <h1>Column Chart</h1>
    <div>
        <div id="divprog" style="width: 750px; height: 350px;">
        </div>
    </div>
    <h1>Bar Chart</h1>    
    <div>
        <div id="divsales" style="width: 350px; height: 350px;">
        </div>
    </div>
    </form>
</body>
</html>