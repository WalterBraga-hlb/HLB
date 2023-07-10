using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Reflection;
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.Office.Core;
using Microsoft.Office.Interop.Excel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml;

namespace MvcAppHyLinedoBrasil.Controllers
{
    public class FormularioPedidosController : Controller
    {
        #region Objects

        #endregion

        //
        // GET: /FormularioPedidos/

        public bool VerificaSessao()
        {
            if (Session["usuario"] == null)
            {
                return true;
            }
            else
            {
                if (Session["usuario"].ToString() == "0")
                {
                    return true;
                }
            }

            return false;
        }

        public ActionResult Index()
        {
            if (VerificaSessao()) return RedirectToAction("LogOn", "Account");
            return View();
        }

        public FileResult BaixarFormulario()
        {
            string destino = "C:\\inetpub\\wwwroot\\Relatorios\\Planilha_Pedido_" + Session["empresaLayout"].ToString() + "_" + Session["login"].ToString() + Session.SessionID + ".xlsm";
            string destino2 = "C:\\inetpub\\wwwroot\\Relatorios\\Planilha_Pedido_" + Session["empresaLayout"].ToString() + "_" + Session["login"].ToString() + Session.SessionID + ".xlsx";

            string pesquisa = "*Planilha_Pedido_" + Session["empresaLayout"].ToString() + "_" + Session["login"].ToString() + "*.xlsm";

            string[] files = Directory.GetFiles("C:\\inetpub\\wwwroot\\Relatorios", pesquisa);

            foreach (var item in files)
            {
                System.IO.File.Delete(item);
            }

            //if (System.IO.File.Exists(destino))
            //{
            //    System.IO.File.Delete(destino);
            //}

            System.IO.File.Copy("C:\\inetpub\\wwwroot\\Relatorios\\Planilha_Pedido_" + Session["empresaLayout"].ToString() + ".xlsm", destino);

            // Object for missing (or optional) arguments.
            object oMissing = System.Reflection.Missing.Value;

            //Process[] P0, P1;
            //P0 = Process.GetProcessesByName("EXCEL");

            // Create an instance of Microsoft Excel, make it visible,
            // and open Book1.xls.
            Process[] P0, P1;
            P0 = Process.GetProcessesByName("Excel");

            Excel.Application oExcel = new Excel.Application();

            int I, J;
            P1 = Process.GetProcessesByName("Excel");
            I = 0;
            if (P1.Length > 1)
            {
                for (I = 0; I < P1.Length; I++)
                {
                    for (J = 0; J < P0.Length; J++)
                        if (P0[J].Id == P1[I].Id) break;
                    if (J == P0.Length) break;
                }
            }
            Process P = P1[I];

            //int I, J;
            //P1 = Process.GetProcessesByName("EXCEL");
            //I = 0;
            //if (P1.Length > 1)
            //{
            //    for (I = 0; I < P1.Length; I++)
            //    {
            //        for (J = 0; J < P0.Length; J++)
            //            if (P0[J].Id == P1[I].Id) break;
            //        if (J == P0.Length) break;
            //    }
            //}
            //Process P = P1[I];

            oExcel.Visible = true;
            Excel.Workbooks oBooks = oExcel.Workbooks;
            Excel._Workbook oBook = null;
            oBook = oBooks.Open(destino, oMissing, oMissing,
                oMissing, oMissing, oMissing, oMissing, oMissing, oMissing,
                //oMissing, oMissing, oMissing, oMissing, oMissing, XlCorruptLoad.xlRepairFile); // Quando abre arquivo corrompido
                oMissing, oMissing, oMissing, oMissing, oMissing, oMissing);

            Excel._Worksheet worksheet = (Excel._Worksheet)oBook.Worksheets["Pedido"];
            //worksheet = oBook.ActiveSheet;

            //if (ddlFazenda.SelectedValue == "(Todas)")
            //    worksheet.Cells[1, 6] = "";
            //else
            //    worksheet.Cells[1, 6] = ddlFazenda.SelectedValue;

            RunMacro(oExcel, new Object[] { "DesbloqueiaPlanilha" });
            //RunMacro(oExcel, new Object[]{"DoKbTestWithParameter","Hello from C# Client."});
            //System.Threading.Thread.Sleep(1000);

            //worksheet.Cells[1, 1] = Session["empresa"].ToString();
            //worksheet.Cells[56, 5] = Session["usuario"].ToString();
            if (Session["login"].ToString().IndexOf("@") > 0)
                worksheet.Cells[1, 1] = Session["login"].ToString();
            else
                worksheet.Cells[1, 1] = "";
            worksheet.Cells[1, 2] = Session["empresaLayout"].ToString();

            RunMacro(oExcel, new Object[] { "AtualizaTabelas" });
            //RunMacro(oExcel, new Object[]{"DoKbTestWithParameter","Hello from C# Client."});
            //System.Threading.Thread.Sleep(3000);
            //worksheet.Cells[2, 11] = calDataFinal.SelectedDate;

            Connections lista = oBook.Connections;

            foreach (Excel.WorkbookConnection item in lista)
            {
                item.OLEDBConnection.BackgroundQuery = false;
            }

            oBook.RefreshAll();

            RunMacro(oExcel, new Object[] { "AtualizaListas" });

            RunMacro(oExcel, new Object[] { "saveAsXlsx" });

            RunMacro(oExcel, new Object[] { "BloqueiaPlanilha" });
            //RunMacro(oExcel, new Object[]{"DoKbTestWithParameter","Hello from C# Client."});
            //System.Threading.Thread.Sleep(1000);

            //Excel._Worksheet worksheetEstq = (Excel._Worksheet)oBook.Worksheets["Estoque de Ovos"];
            //worksheetEstq.Cells[3, 7] = DateTime.Now;

            //worksheet.Cells.EntireColumn.AutoFit();
            //oBook.Save();

            // Quit Excel and clean up.
            oBook.Close(true, oMissing, oMissing);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(oBook);
            oBook = null;
            System.Runtime.InteropServices.Marshal.ReleaseComObject(oBooks);
            oBooks = null;
            oExcel.Quit();
            System.Runtime.InteropServices.Marshal.ReleaseComObject(oExcel);
            oExcel = null;

            P.Kill();

            GC.Collect();

            //FileStream stream = new FileStream(destino, FileMode.Open);

            //return File(stream, "application/vnd.ms-excel", "FormularioPedido.xlsm");

            return File(destino2, "Download", "FormularioPedido.xlsx");
        }

        private void RunMacro(object oApp, object[] oRunArgs)
        {
            oApp.GetType().InvokeMember("Run",
                System.Reflection.BindingFlags.Default |
                System.Reflection.BindingFlags.InvokeMethod,
                null, oApp, oRunArgs);
        }
    }
}
