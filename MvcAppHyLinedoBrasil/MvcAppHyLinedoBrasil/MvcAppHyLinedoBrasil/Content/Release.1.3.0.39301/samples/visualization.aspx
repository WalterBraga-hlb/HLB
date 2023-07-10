<%@ Page Language="C#" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<script runat="server">

    protected void Page_Load(object sender, EventArgs e)
    {
        var googleDataTable = new Bortosky.Google.Visualization.GoogleDataTable(this.ProgrammingTable);
        Page.ClientScript.RegisterStartupScript(
            this.GetType(), "vis", string.Format("var fundata = {0};", googleDataTable.GetJson()), true);
    }
    protected System.Data.DataTable ProgrammingTable
    {
        get // a DataTable filled in any way, for example:
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
</script>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>.NET Visualization Helper Sample</title>

    <script type="text/javascript" src="http://www.google.com/jsapi"></script>

    <script type="text/javascript">
        google.load("visualization", "1", { "packages": ["corechart"] });
        google.setOnLoadCallback(function () {
            var data = new google.visualization.DataTable(fundata, 0.5);
            var chart = new google.visualization.ColumnChart(document.getElementById("chart_div"));
            chart.draw(data, { title: "Visualization Satisfaction", hAxis: { title: "Programming method" }, vAxis: { title: "Units"} });
        });
    </script>

    <link href="../styles.css" rel="stylesheet" type="text/css" />

</head>
<body>
    <form id="form1" runat="server">
    <h1>Visualization Column Chart Sample</h1>
    <div id="chart_div" style="width: 700px; height: 300px;"></div>
    <p>See <a href="http://code.google.com/p/bortosky-google-visualization/">the open-source project site</a> for more information and to download the Visualization helper for .NET.</p>
    </form>
</body>
</html>
