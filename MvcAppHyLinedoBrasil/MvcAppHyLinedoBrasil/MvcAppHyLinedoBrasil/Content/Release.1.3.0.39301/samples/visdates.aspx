<%@ Page Language="C#" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<script runat="server">

    protected void Page_Load(object sender, EventArgs e)
    {
        LoadChart(this.BalanceTable, "balances");
    }
    private void LoadChart(System.Data.DataTable table, string jsName)
    {
        Page.ClientScript.RegisterStartupScript(this.GetType(),
            jsName,
            string.Format("var {0} = {1};", jsName, new Bortosky.Google.Visualization.GoogleDataTable(table).GetJson()),
            true);
    }
    private System.Data.DataTable BalanceTable
    {
        get
        {
            var today = System.DateTime.Now;
            var rnd = new System.Random(today.Millisecond);
            var dt = new System.Data.DataTable();
            dt.Columns.Add("DATE", typeof(System.DateTime)).Caption = "Date";
            dt.Columns.Add("CHECKING", typeof(System.Decimal)).Caption = "Checking";
            dt.Columns.Add("SAVINGS", typeof(System.Decimal)).Caption = "Savings";
            for (var d = 0; d < 30; d++)
                dt.Rows.Add(new object[] { today.AddDays(-d), d * rnd.Next(500), d * rnd.Next(500) });
            Bortosky
                .Google
                .Visualization
                .GoogleDataTable
                .SetGoogleDateType(dt.Columns["Date"], Bortosky.Google.Visualization.GoogleDateType.Date);
            return dt;
        }
    }
    
</script>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>.NET Visualization Time Line</title>

    <script type="text/javascript" src="http://www.google.com/jsapi"></script>
    <script type="text/javascript">
        google.load("visualization", "1", { "packages": ["annotatedtimeline"] });
        function drawBalanceChart() {
            var data = new google.visualization.DataTable(balances, 0.5);
            var chart = new google.visualization.AnnotatedTimeLine(document.getElementById("divbalances"));
            chart.draw(data, { thickness: 1, displayExactValues: true });
        }
        google.setOnLoadCallback(drawBalanceChart);
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
        <h1>Annotated Time Line</h1>
        <div>
            <div id="divbalances" style="width: 500px; height: 300px;"></div>
        </div>
    </form>
</body>
</html>