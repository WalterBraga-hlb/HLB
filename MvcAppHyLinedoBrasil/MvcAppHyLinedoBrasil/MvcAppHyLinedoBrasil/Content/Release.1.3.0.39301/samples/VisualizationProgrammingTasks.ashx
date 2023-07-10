<%@ WebHandler Language="C#" Class="VisualizationProgrammingTasks" %>

using System;
using System.Data;
using System.Web;

public class VisualizationProgrammingTasks : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "application/json";
        new Bortosky.Google.Visualization.GoogleDataTable(
            ProgrammingTable(context.Request.QueryString["funest"])).WriteJson(context.Response.OutputStream);
    }

    public bool IsReusable
    {
        get { return false; }
    }

    private static DataTable ProgrammingTable(string mostFunStyle)
    {
        var rand = new System.Random(DateTime.Now.Millisecond);
        var dt = new DataTable();
        dt.Columns.Add("STYLE", typeof(System.String)).Caption = "Programming Style";
        dt.Columns.Add("FUN", typeof(System.Int32)).Caption = "Fun";
        dt.Columns.Add("WORK", typeof(System.Int32)).Caption = "Work";
        foreach (var s in new[] { "Hand Coding", "Using the .NET Library", "Skipping Visualization" })
        {
            dt.Rows.Add(new object[] { s, rand.Next(0, 20 * (s.Equals(mostFunStyle) ? 10 : 1)), rand.Next(0, 20) });
        };
        return dt;
    }
}
