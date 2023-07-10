using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;
using Access = Microsoft.Office.Interop.Access;
using Excel = Microsoft.Office.Interop.Excel;
using PowerPoint = Microsoft.Office.Interop.PowerPoint;
using Word = Microsoft.Office.Interop.Word;
using Microsoft.Office.Core;

namespace MvcAppHyLinedoBrasil.Models
{
    public class Office
    {
        public void AtualizaOfficeComMacro(string ferramenta, string arquivo, string nomeMacro)
        {
            //// Object for missing (or optional) arguments.
            //object oMissing = System.Reflection.Missing.Value;

            ////Switch based on the user selection.
            //switch (ferramenta)
            //{
            //    case "Access":
            //        // Create an instance of Microsoft Access, make it visible,
            //        // and open Db1.mdb.
            //        //Access.ApplicationClass oAccess = new Access.ApplicationClass();
            //        Access.Application oAccess = new Access.Application();
            //        oAccess.Visible = true;
            //        oAccess.OpenCurrentDatabase(arquivo, false, "");

            //        // Run the macros.
            //        RunMacro(oAccess, new Object[] { nomeMacro });
            //        //RunMacro(oAccess, new Object[]{"DoKbTestWithParameter","Hello from C# Client."});

            //        // Quit Access and clean up.
            //        oAccess.DoCmd.Quit(Access.AcQuitOption.acQuitSaveNone);
            //        System.Runtime.InteropServices.Marshal.ReleaseComObject(oAccess);
            //        oAccess = null;

            //        break;

            //    case "Excel":
            //        // Create an instance of Microsoft Excel, make it visible,
            //        // and open Book1.xls.
            //        Excel.Application oExcel = new Excel.Application();
            //        //oExcel.Visible = true;
            //        Excel.Workbooks oBooks = oExcel.Workbooks;
            //        Excel._Workbook oBook = null;
            //        oBook = oBooks.Open(arquivo, oMissing, oMissing,
            //            oMissing, oMissing, oMissing, oMissing, oMissing, oMissing,
            //            oMissing, oMissing, oMissing, oMissing, oMissing, oMissing);

            //        Excel._Worksheet worksheet = oBook.Worksheets["Plan2"];
            //        //worksheet = oBook.ActiveSheet;
            //        worksheet.Cells[1, 5] = "OBAAAAA";
            //        worksheet.Cells.EntireColumn.AutoFit();
            //        oBook.Save();

            //        // Run the macros.
            //        RunMacro(oExcel, new Object[] { "DoKbTest" });
            //        RunMacro(oExcel, new Object[]{"DoKbTestWithParameter",
            //               "Hello from C# Client."});

            //        // Quit Excel and clean up.
            //        oBook.Close(false, oMissing, oMissing);
            //        System.Runtime.InteropServices.Marshal.ReleaseComObject(oBook);
            //        oBook = null;
            //        System.Runtime.InteropServices.Marshal.ReleaseComObject(oBooks);
            //        oBooks = null;
            //        oExcel.Quit();
            //        System.Runtime.InteropServices.Marshal.ReleaseComObject(oExcel);
            //        oExcel = null;

            //        ////Response.AddHeader("Content-Length", new System.IO.FileInfo("c:\\AMD\\book1.xlsm").Length.ToString());
            //        //Response.AddHeader("Content-Disposition", "attachment; filename=book1.xlsm");
            //        ////Response.AddHeader("Content-Length", file.Length.ToString());
            //        //Response.ContentType = "application/vnd.ms-excel";
            //        //Response.TransmitFile("c:\\AMD\\book1.xlsm");
            //        ////Response.WriteFile("c:\\AMD\\book1.xlsm");

            //        break;

            //    case "Power Point":

            //        // Create an instance of PowerPoint, make it visible,
            //        // and open Pres1.ppt.
            //        PowerPoint.Application oPP = new PowerPoint.Application();
            //        oPP.Visible = MsoTriState.msoTrue;
            //        PowerPoint.Presentations oPresSet = oPP.Presentations;
            //        PowerPoint._Presentation oPres = oPresSet.Open("c:\\pres1.ppt",
            //            MsoTriState.msoFalse, MsoTriState.msoFalse,
            //            MsoTriState.msoTrue);

            //        // Run the macros.
            //        RunMacro(oPP, new Object[] { "'pres1.ppt'!DoKbTest" });
            //        RunMacro(oPP, new Object[]{"'pres1.ppt'!DoKbTestWithParameter",
            //               "Hello from C# Client."});

            //        // Quit PowerPoint and clean up.
            //        oPres.Close();
            //        System.Runtime.InteropServices.Marshal.ReleaseComObject(oPres);
            //        oPres = null;
            //        System.Runtime.InteropServices.Marshal.ReleaseComObject(oPresSet);
            //        oPresSet = null;
            //        oPP.Quit();
            //        System.Runtime.InteropServices.Marshal.ReleaseComObject(oPP);
            //        oPP = null;

            //        break;

            //    case "Word":

            //        // Create an instance of Word, make it visible,
            //        // and open Doc1.doc.
            //        Word.Application oWord = new Word.Application();
            //        oWord.Visible = true;
            //        Word.Documents oDocs = oWord.Documents;
            //        object oFile = "c:\\doc1.doc";

            //        // If the Microsoft Word 10.0 Object Library is referenced
            //        // use the following code.
            //        Word._Document oDoc = oDocs.Open(ref oFile, ref oMissing,
            //            ref oMissing, ref oMissing, ref oMissing, ref oMissing,
            //            ref oMissing, ref oMissing, ref oMissing, ref oMissing,
            //            ref oMissing, ref oMissing, ref oMissing, ref oMissing,
            //            ref oMissing);

            //        // If the Microsoft Word 11.0 Object Library is referenced comment
            //        // the previous line of code and uncomment the following code.
            //        //Word._Document oDoc = oDocs.Open(ref oFile, ref oMissing,
            //        //ref oMissing, ref oMissing, ref oMissing, ref oMissing,
            //        //ref oMissing, ref oMissing, ref oMissing, ref oMissing,
            //        //ref oMissing, ref oMissing, ref oMissing, ref oMissing,
            //        //ref oMissing, ref oMissing);

            //        // Run the macros.
            //        RunMacro(oWord, new Object[] { "DoKbTest" });
            //        RunMacro(oWord, new Object[]{"DoKbTestWithParameter",
            //               "Hello from C# Client."});

            //        // Quit Word and clean up.
            //        oDoc.Close(ref oMissing, ref oMissing, ref oMissing);
            //        System.Runtime.InteropServices.Marshal.ReleaseComObject(oDoc);
            //        oDoc = null;
            //        System.Runtime.InteropServices.Marshal.ReleaseComObject(oDocs);
            //        oDocs = null;
            //        oWord.Quit(ref oMissing, ref oMissing, ref oMissing);
            //        System.Runtime.InteropServices.Marshal.ReleaseComObject(oWord);
            //        oWord = null;

            //        break;

            //}

            //GC.Collect();   //Garbage collection.
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