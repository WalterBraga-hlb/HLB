using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcAppHylinedoBrasilMobile.Models;
using MvcAppHylinedoBrasilMobile.Models.CHICMobileDataSetTableAdapters;
using System.IO;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Excel = Microsoft.Office.Interop.Excel;
//using DHTMLX.Scheduler;
//using DHTMLX.Scheduler.Data;
//using DHTMLX.Common;
using System.Globalization;

namespace MvcAppHylinedoBrasilMobile.Controllers
{
    public class NavisionIntegrationAppController : Controller
    {
        #region Objects

        HLBAPPEntities hlbapp = new HLBAPPEntities();

        public static HLBAPPEntities hlbappStatic = new HLBAPPEntities();

        #endregion

        #region View Methods

        public ActionResult ListNavisionIntegration(string source)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            if (Session["HatcheryList"] == null)
                LoadHatcheryList();
            if (Session["DateTypeList"] == null)
                LoadDateTypeList();
            if (Session["CountryList"] == null)
                LoadCountryList();
            if (Session["initialDateSearchNav"] == null)
                LoadDateParams();
            if (Session["customerNameSearchNav"] == null)
                Session["customerNameSearchNav"] = "";
            if (Session["showDetailsSearchNav"] == null)
                Session["showDetailsSearchNav"] = false;
            if (Session["locSelectedNavInt"] == null)
                Session["locSelectedNavInt"] = "";
            if (Session["dateTypeSelectedNavInt"] == null)
                Session["dateTypeSelectedNavInt"] = 1;
            if (Session["countrySearchNav"] == null)
                Session["countrySearchNav"] = "";

            Session["sourceClick"] = "Index";

            List<String> listCountries = new List<string>();

            List<NavOrders> listNavOrders = SearchNavOrders(Session["locSelectedNavInt"].ToString(),
                Convert.ToInt16(Session["dateTypeSelectedNavInt"]), Convert.ToDateTime(Session["initialDateSearchNav"]),
                Convert.ToDateTime(Session["finalDateSearchNav"]), Session["customerNameSearchNav"].ToString(),
                Session["countrySearchNav"].ToString(), listCountries);

            return View("Index", listNavOrders);
        }

        public ActionResult LoadNavOrdersView(string Text, string DateType,
            DateTime initialDate, DateTime finalDate, string customerName, string country,
            FormCollection model)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            if (Session["sourceClick"].ToString() == "Index")
                Session["showDetailsSearchNav"] = model["showDetails"].Replace("false,true", "true");

            short dateType = Convert.ToInt16(DateType);

            string language = Session["language"].ToString();

            string typeAll = hlbapp.Languages
                .Where(w => w.Caption == "Tab_Nav_Order_DropDown_HatcheryList_All"
                    && w.Language == language)
                .FirstOrDefault().Text;

            string loc = "";
            if (Text != typeAll)
                loc = Text;

            string ctr = "";
            if (country != typeAll)
                ctr = country;

            Session["locSelectedNavInt"] = loc;
            UpdateHatcheryListSelected(loc);
            Session["dateTypeSelectedNavInt"] = dateType;
            UpdateDateTypeListSelected(dateType.ToString());
            Session["initialDateSearchNav"] = initialDate;
            Session["finalDateSearchNav"] = finalDate;
            Session["customerNameSearchNav"] = customerName;
            Session["countrySearchNav"] = ctr;

            List<String> listCountries = new List<string>();

            List<NavOrders> listNavOrders = SearchNavOrders(loc, dateType, initialDate, finalDate, customerName, ctr,
                listCountries);

            if (Session["sourceClick"].ToString() == "Index")
                return View("Index", listNavOrders);
            else
                return View("RelationCHICWithNavision", listNavOrders);
        }

        public ActionResult RelationCHICWithNavisionIndex()
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            if (Session["HatcheryList"] == null)
                LoadHatcheryList();
            if (Session["DateTypeList"] == null)
                LoadDateTypeList();
            if (Session["initialDateSearchNav"] == null)
                LoadDateParams();
            if (Session["customerNameSearchNav"] == null)
                Session["customerNameSearchNav"] = "";
            if (Session["showDetailsSearchNav"] == null)
                Session["showDetailsSearchNav"] = false;
            if (Session["locSelectedNavInt"] == null)
                Session["locSelectedNavInt"] = "";
            if (Session["dateTypeSelectedNavInt"] == null)
                Session["dateTypeSelectedNavInt"] = 0;
            if (Session["countrySearchNav"] == null)
                Session["countrySearchNav"] = "";

            Session["sourceClick"] = "RelationCHICWithNavision";
            Session["showDetailsSearchNav"] = false;

            string loc = Session["locSelectedNavInt"].ToString();
            short dateType = Convert.ToInt16(Session["dateTypeSelectedNavInt"]);
            DateTime initialDate = Convert.ToDateTime(Session["initialDateSearchNav"]);
            DateTime finalDate = Convert.ToDateTime(Session["finalDateSearchNav"]);
            string customerName = Session["customerNameSearchNav"].ToString();
            string country = Session["countrySearchNav"].ToString();

            List<String> listCountries = new List<string>();

            List<NavOrders> listNavOrders = SearchNavOrders(loc, dateType, initialDate, finalDate, customerName, country,
                listCountries);

            return View("RelationCHICWithNavision", listNavOrders);
        }

        public ActionResult SaveRelationCHICWithNavision(FormCollection model)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            List<String> listCountries = new List<string>();

            List<NavOrders> listNavOrders = SearchNavOrders(Session["locSelectedNavInt"].ToString(),
                    Convert.ToInt16(Session["dateTypeSelectedNavInt"]), Convert.ToDateTime(Session["initialDateSearchNav"]),
                    Convert.ToDateTime(Session["finalDateSearchNav"]), Session["customerNameSearchNav"].ToString(),
                    Session["countrySearchNav"].ToString(), listCountries);

            try
            {
                string navNumber = model["OrderNumberNavision"];
                string usuario = Session["usuario"].ToString();

                //if (navNumber.Equals(""))
                //{
                //    ViewBag.Erro = MvcAppHylinedoBrasilMobile.Controllers.AccountMobileController
                //        .GetTextOnLanguage("LinkCHICWithNav_Warning_NavNumberEmpty",
                //            Session["language"].ToString());
                //    return View("RelationCHICWithNavision", listNavOrders);
                //}

                var fileIds = ("," + model["CHICorderNo"]).Split(',');

                var selectedIndices = model["OrderSelected"].Replace("true,false", "true")
                            .Split(',')
                            .Select((item, index) => new { item = item, index = index })
                            .Where(row => row.item == "true")
                            .Select(row => row.index).ToArray();

                if (selectedIndices.Count() > 0)
                {
                    foreach (var index in selectedIndices)
                    {
                        int fileId;
                        if (int.TryParse(fileIds[index], out fileId))
                        {
                            string id = fileId.ToString();
                            Nav_Orders linkCHICNav = hlbapp.Nav_Orders
                                .Where(w => w.OrderNumberCHIC == id)
                                .FirstOrDefault();

                            if (linkCHICNav == null)
                            {
                                linkCHICNav = new Nav_Orders();
                                linkCHICNav.OrderNumberCHIC = fileId.ToString();
                                linkCHICNav.OrderNumberNavision = navNumber;
                                linkCHICNav.usuario = usuario;
                                hlbapp.Nav_Orders.AddObject(linkCHICNav);
                            }
                            else
                            {
                                if (navNumber.Equals(""))
                                {
                                    linkCHICNav.usuario = usuario;
                                    hlbapp.SaveChanges();
                                    hlbapp.Nav_Orders.DeleteObject(linkCHICNav);
                                }
                                else
                                {
                                    linkCHICNav.usuario = usuario;
                                    linkCHICNav.OrderNumberNavision = navNumber;
                                }
                            }
                        }
                    }
                }
                else
                {
                    ViewBag.Erro = MvcAppHylinedoBrasilMobile.Controllers.AccountMobileController
                        .GetTextOnLanguage("LinkCHICWithNav_Warning_Select_Orders", 
                            Session["language"].ToString());
                    return View("RelationCHICWithNavision", listNavOrders);
                }

                hlbapp.SaveChanges();

                List<String> listCount = new List<string>();

                listNavOrders = SearchNavOrders(Session["locSelectedNavInt"].ToString(),
                    Convert.ToInt16(Session["dateTypeSelectedNavInt"]), Convert.ToDateTime(Session["initialDateSearchNav"]),
                    Convert.ToDateTime(Session["finalDateSearchNav"]), Session["customerNameSearchNav"].ToString(),
                    Session["countrySearchNav"].ToString(), listCount);
                
                Session["sourceClick"] = "Index";
                return View("Index", listNavOrders);
            }
            catch (Exception e)
            {
                string msg = "";
                if (e.InnerException != null)
                    msg = e.Message + " / " + e.InnerException.Message;
                else
                    msg = e.Message;
                ViewBag.Erro = "Error to link CHIC and Navision: " + msg;
                return View("RelationCHICWithNavision", listNavOrders);
            }
        }

        #region Imports Views

        public ActionResult ImportCHICExternalIndex()
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            return View("ImportCHICExternal");
        }

        [HttpPost]
        public ActionResult ImportCHICExternal(HttpPostedFileBase file)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            string caminho = @"C:\inetpub\wwwroot\Relatorios\ImportCHICExternal\ImportCHICExternal_"
                + Session["login"].ToString() + "_"
                + "_" + DateTime.Now.ToString("dd-MM-yyy")
                + "_" + DateTime.Now.ToString("mm-ss")
                + "_" + DateTime.Now.Millisecond
                + ".xls";

            file.SaveAs(caminho);
            caminho = VerificaFormatoArquivo(caminho);
            Stream arquivo = System.IO.File.Open(caminho, FileMode.Open);

            if (arquivo.Length > 0)
            {
                string retorno = ImportExcelCHICExternal(arquivo);

                if (retorno.Equals(""))
                {
                    ViewBag.erro = "";
                    ViewBag.fileName = MvcAppHylinedoBrasilMobile.Controllers.AccountMobileController
                        .GetTextOnLanguage("ImportCHICExternal_Message_Success_01",
                            Session["language"].ToString()) 
                            + Request.Files[0].FileName
                            + MvcAppHylinedoBrasilMobile.Controllers.AccountMobileController
                        .GetTextOnLanguage("ImportCHICExternal_Message_Success_02",
                            Session["language"].ToString());
                }
                else
                {
                    ViewBag.erro = retorno;
                    ViewBag.fileName = "";
                }
            }
            else
            {
                ViewBag.Erro = MvcAppHylinedoBrasilMobile.Controllers.AccountMobileController
                        .GetTextOnLanguage("ImportCHICExternal_Warning_Select_File",
                            Session["language"].ToString());
            }

            return View("ImportCHICExternal");
        }

        public ActionResult ImportActualOrdersIndex()
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            return View("ImportActualOrders");
        }

        [HttpPost]
        public ActionResult ImportActualOrders(HttpPostedFileBase file)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            string caminho = @"C:\inetpub\wwwroot\Relatorios\ImportActualOrders\ImportActualOrders_"
                + Session["login"].ToString() + "_"
                + "_" + DateTime.Now.ToString("dd-MM-yyy")
                + "_" + DateTime.Now.ToString("mm-ss")
                + "_" + DateTime.Now.Millisecond
                + ".xls";

            file.SaveAs(caminho);
            caminho = VerificaFormatoArquivo(caminho);
            Stream arquivo = System.IO.File.Open(caminho, FileMode.Open);

            if (arquivo.Length > 0)
            {
                string retorno = ImportExcelActualOrders(arquivo);

                if (retorno.Equals(""))
                {
                    ViewBag.erro = "";
                    ViewBag.fileName = MvcAppHylinedoBrasilMobile.Controllers.AccountMobileController
                        .GetTextOnLanguage("ImportCHICExternal_Message_Success_01",
                            Session["language"].ToString())
                            + Request.Files[0].FileName
                            + MvcAppHylinedoBrasilMobile.Controllers.AccountMobileController
                        .GetTextOnLanguage("ImportCHICExternal_Message_Success_02",
                            Session["language"].ToString());
                }
                else
                {
                    ViewBag.erro = retorno;
                    ViewBag.fileName = "";
                }
            }
            else
            {
                ViewBag.Erro = MvcAppHylinedoBrasilMobile.Controllers.AccountMobileController
                        .GetTextOnLanguage("ImportCHICExternal_Warning_Select_File",
                            Session["language"].ToString());
            }

            return View("ImportActualOrders");
        }

        public ActionResult ImportAgendaIndex()
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            return View("ImportAgenda");
        }

        [HttpPost]
        public ActionResult ImportAgenda(HttpPostedFileBase file)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            string caminho = @"C:\inetpub\wwwroot\Relatorios\ImportAgenda\ImportAgenda_"
                + Session["login"].ToString() + "_"
                + "_" + DateTime.Now.ToString("dd-MM-yyy")
                + "_" + DateTime.Now.ToString("mm-ss")
                + "_" + DateTime.Now.Millisecond
                + ".xls";

            file.SaveAs(caminho);
            caminho = VerificaFormatoArquivo(caminho);
            Stream arquivo = System.IO.File.Open(caminho, FileMode.Open);

            if (arquivo.Length > 0)
            {
                string retorno = ImportExcelAgenda(arquivo);

                if (retorno.Equals(""))
                {
                    ViewBag.erro = "";
                    ViewBag.fileName = MvcAppHylinedoBrasilMobile.Controllers.AccountMobileController
                        .GetTextOnLanguage("ImportCHICExternal_Message_Success_01",
                            Session["language"].ToString())
                            + Request.Files[0].FileName
                            + MvcAppHylinedoBrasilMobile.Controllers.AccountMobileController
                        .GetTextOnLanguage("ImportCHICExternal_Message_Success_02",
                            Session["language"].ToString());
                }
                else
                {
                    ViewBag.erro = retorno;
                    ViewBag.fileName = "";
                }
            }
            else
            {
                ViewBag.Erro = MvcAppHylinedoBrasilMobile.Controllers.AccountMobileController
                        .GetTextOnLanguage("ImportCHICExternal_Warning_Select_File",
                            Session["language"].ToString());
            }

            return View("ImportAgenda");
        }

        #endregion

        public ActionResult OrdersCalendar(DateTime firstDate, string source)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            //var scheduler = new DHXScheduler(this); //initializes dhtmlxScheduler
            //scheduler.LoadData = true;// allows loading data
            //scheduler.EnableDataprocessor = true;// enables DataProcessor in order to enable implementation CRUD operations
            //scheduler.Height = 500;
            //scheduler.InitialView = scheduler.Views[0].Name;
            //scheduler.Config.readonly_form = true;
            //return View(scheduler);

            //DateTime firstDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            DateTime lastDate = firstDate.AddMonths(1).AddDays(-1);

            if (Session["CountryList"] == null)
                LoadCountryList();
            if (Session["VisitorList"] == null)
                LoadVisitorList();
            if (Session["customerNameSearchNav"] == null)
                Session["customerNameSearchNav"] = "";

            Session["visitorSelectedNavInt"] = "";
            Session["initialDateSearchNav"] = firstDate;
            Session["finalDateSearchNav"] = lastDate;
            Session["showAgendaSearchNav"] = false;

            List<String> listCountries = new List<string>();

            var lista = SearchNavOrders("", 1, firstDate, lastDate, "", "", listCountries);

            //int semanaAnoFirstDate = CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(
            //            Convert.ToDateTime(firstDate),
            //            CalendarWeekRule.FirstFullWeek, DayOfWeek.Sunday);
            //if (semanaAnoFirstDate != 1 && firstDate.Month == 1)
            //    Session["CalendaryearOfMonth"] = firstDate.AddYears(-1).Year;
            //else
            //    Session["CalendaryearOfMonth"] = firstDate.Year;
            //if ((Session["CalendarFirstDateMonth"] == null) || (source == "filter"))
            Session["CalendarFirstDateMonth"] = firstDate;
            
            #region Lista Semanas do Ano p/ Exibir no Calendário

            List<int> listaSemanaAno = new List<int>();

            DateTime dateCount = firstDate;
            int semanaAnoAnterior = 0;

            while (dateCount <= lastDate)
            {
                int semanaAno = CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(
                        Convert.ToDateTime(dateCount),
                        CalendarWeekRule.FirstDay, DayOfWeek.Sunday);

                if (semanaAnoAnterior != semanaAno)
                {
                    listaSemanaAno.Add(semanaAno);
                }

                semanaAnoAnterior = semanaAno;

                dateCount = dateCount.AddDays(1);
            }

            Session["CalendarListWeeksOfYear"] = listaSemanaAno;

            #endregion

            return View(lista);
        }

        public ActionResult LoadNavOrdersCalendarView(FormCollection model, string Visitor)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            DateTime firstDate = Convert.ToDateTime(Session["initialDateSearchNav"]);
            DateTime lastDate = firstDate.AddMonths(1).AddDays(-1);

            string language = Session["language"].ToString();
            string typeAll = hlbapp.Languages
                .Where(w => w.Caption == "Tab_Nav_Order_DropDown_HatcheryList_All"
                    && w.Language == language)
                .FirstOrDefault().Text;

            if (Session["VisitorList"] == null)
                LoadVisitorList();
            else
            {
                if (Visitor != null)
                {
                    string loc = "";
                    if (Visitor != typeAll)
                        loc = Visitor;

                    UpdateVisitorListSelected(loc);
                    Session["visitorSelectedNavInt"] = loc;
                }
                else
                {
                    if (Session["visitorSelectedNavInt"] == null)
                        Session["visitorSelectedNavInt"] = "";
                }
            }

            if (model["showAgenda"] != null)
                Session["showAgendaSearchNav"] = model["showAgenda"].Replace("false,true", "true");

            if (Session["CountryList"] == null)
                LoadCountryList();
            if (model["customerName"] != null)
                Session["customerNameSearchNav"] = model["customerName"];
            else
                Session["customerNameSearchNav"] = "";

            Session["initialDateSearchNav"] = firstDate;
            Session["finalDateSearchNav"] = lastDate;

            String[] fileCountries = null;
            int[] selectedIndices = null;
            if (model.Count > 0)
            {
                fileCountries = ("," + model["name"]).Split(',');
                selectedIndices = model["countrySelect"].Replace("true,false", "true")
                                    .Split(',')
                                    .Select((item, index) => new { item = item, index = index })
                                    .Where(row => row.item == "true")
                                    .Select(row => row.index).ToArray();
                Session["countrySelectedList"] = fileCountries;
                Session["indexCountrySelectedList"] = selectedIndices;
            }
            else
            {
                fileCountries = (String[])Session["countrySelectedList"];
                selectedIndices = (int[])Session["indexCountrySelectedList"];
            }

            List<String> listCountries = new List<string>();

            List<String> listCountriesUpdSession = new List<string>();
            foreach (var index in selectedIndices)
            {
                listCountriesUpdSession.Add(fileCountries[index]);
            }

            UpdateCountryListSelected(listCountriesUpdSession);

            if (listCountriesUpdSession.Where(w => w == typeAll).Count() == 0)
            {
                foreach (var index in selectedIndices)
                {
                    listCountries.Add(fileCountries[index]);
                }
            }

            var lista = SearchNavOrders("", 1, firstDate, lastDate, Session["customerNameSearchNav"].ToString(),
                "", listCountries);

            Session["CalendarFirstDateMonth"] = firstDate;

            #region Lista Semanas do Ano p/ Exibir no Calendário

            List<int> listaSemanaAno = new List<int>();

            DateTime dateCount = firstDate;
            int semanaAnoAnterior = 0;

            while (dateCount <= lastDate)
            {
                int semanaAno = CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(
                        Convert.ToDateTime(dateCount),
                        CalendarWeekRule.FirstDay, DayOfWeek.Sunday);

                if (semanaAnoAnterior != semanaAno)
                {
                    listaSemanaAno.Add(semanaAno);
                }

                semanaAnoAnterior = semanaAno;

                dateCount = dateCount.AddDays(1);
            }

            Session["CalendarListWeeksOfYear"] = listaSemanaAno;

            #endregion

            return View("OrdersCalendar",lista);
        }

        public ActionResult DetailsOrder(string local, string orderNoCHIC, string orderNoNavision)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            DateTime firstDate = Convert.ToDateTime(Session["initialDateSearchNav"]);
            DateTime lastDate = Convert.ToDateTime(Session["finalDateSearchNav"]);

            List<String> listCountries = new List<string>();

            var lista = SearchNavOrders("", 1, firstDate, lastDate, "", "", listCountries);

            if (orderNoCHIC == null) orderNoCHIC = "";
            if (orderNoNavision == null) orderNoNavision = "";

            var listaFiltro = lista
                .Where(w => w.Location == local
                    && w.OrderNo == orderNoCHIC
                    && w.NavOrderNo == orderNoNavision)
                .ToList();

            Session["origemDetailsOrder"] = "Calendar";

            return View(listaFiltro);
        }

        public ActionResult OrdersAgenda()
        {
            if (Session["initialDateSearchNav"] == null)
                LoadDateParams();

            if (Session["VisitorList"] == null)
                LoadVisitorList();

            Session["visitorSelectedNavInt"] = "";

            DateTime initialDate = Convert.ToDateTime(Session["initialDateSearchNav"]);
            DateTime finalDate = Convert.ToDateTime(Session["finalDateSearchNav"]);

            Session["ListOrders"] = StaticSearchNavOrders(initialDate, finalDate);

            var lista = hlbapp.Nav_Agenda
                .Where(w => w.VisitDate >= initialDate && w.VisitDate <= finalDate)
                .ToList();

            return View("Agenda", lista);
        }

        public ActionResult LoadNavOrdersAgendaView(DateTime initialDate, DateTime finalDate)
        {
            string Visitor = "";

            string language = Session["language"].ToString();
            string typeAll = hlbapp.Languages
                .Where(w => w.Caption == "Tab_Nav_Order_DropDown_HatcheryList_All"
                    && w.Language == language)
                .FirstOrDefault().Text;

            if (Session["VisitorList"] == null)
                LoadVisitorList();
            else
            {
                if (Visitor != null)
                {
                    string loc = "";
                    if (Visitor != typeAll)
                        loc = Visitor;

                    UpdateVisitorListSelected(loc);
                    Session["visitorSelectedNavInt"] = loc;
                }
                else
                {
                    if (Session["visitorSelectedNavInt"] == null)
                        Session["visitorSelectedNavInt"] = "";
                }
            }

            if (initialDate != null)
                Session["initialDateSearchNav"] = initialDate;
            if (finalDate != null)
                Session["finalDateSearchNav"] = finalDate;

            Session["ListOrders"] = StaticSearchNavOrders(initialDate, finalDate);

            string vis = Session["visitorSelectedNavInt"].ToString();
            var lista = hlbapp.Nav_Agenda
                .Where(w => w.VisitDate >= initialDate && w.VisitDate <= finalDate
                    && (w.Visitor == vis || vis == ""))
                .ToList();

            return View("Agenda", lista);
        }

        public ActionResult DetailsOrderAgenda(string customer, DateTime hatchDate)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            List<String> listCountries = new List<string>();

            DateTime date = hatchDate.AddDays(-1);

            List<NavOrders> lista = SearchNavOrders("", 1, date, date, customer,
                "", listCountries);

            var listaFiltro = lista
                .Where(w => w.CustomerName == customer
                    && w.HatchDate == date)
                .ToList();

            Session["origemDetailsOrder"] = "Agenda";

            return View("DetailsOrder", listaFiltro);
        }

        //public ActionResult Data()
        //{//events for loading to scheduler
        //    List<CalendarEvents> lista = new List<CalendarEvents>();
        //    CalendarEvents calendarEvent = new CalendarEvents();
        //    calendarEvent.id = 1;
        //    calendarEvent.start_date = DateTime.Today;
        //    calendarEvent.end_date = DateTime.Today;
        //    calendarEvent.text = "Teste";
        //    lista.Add(calendarEvent);

        //    return new SchedulerAjaxData(lista);
        //}

        //public ActionResult Save(CalendarEvents updatedEvent, FormCollection formData)
        //{
        //    var action = new DataAction(formData);
        //    /*var context = new SampleDataContext();

        //    try
        //    {
        //        switch (action.Type)
        //        {
        //            case DataActionTypes.Insert: // your Insert logic
        //                context.Events.InsertOnSubmit(updatedEvent);
        //                break;
        //            case DataActionTypes.Delete: // your Delete logic
        //                updatedEvent = context.Events.SingleOrDefault(ev => ev.id == updatedEvent.id);
        //                context.Events.DeleteOnSubmit(updatedEvent);
        //                break;
        //            default:// "update" // your Update logic
        //                updatedEvent = context.Events.SingleOrDefault(
        //                ev => ev.id == updatedEvent.id);
        //                UpdateModel(updatedEvent);
        //                break;
        //        }
        //        context.SubmitChanges();
        //        action.TargetId = updatedEvent.id;
        //    }
        //    catch (Exception a)
        //    {
        //        action.Type = DataActionTypes.Error;
        //    }*/
        //    return (new AjaxSaveResponse(action));
        //}

        #endregion

        #region Search Methods

        public List<NavOrders> SearchNavOrders(string location, short dateType,
            DateTime initialDate, DateTime finalDate, string customerName, string country,
            List<String> listCountries)
        {
            List<NavOrders> listNavOrders = new List<NavOrders>();

            customerName = customerName.ToUpper();

            #region Search Orders in Brazil CHIC

            CHICMobileDataSet chic = new CHICMobileDataSet();

            bookedNavTableAdapter bookedNav = new bookedNavTableAdapter();
            CHICMobileDataSet.bookedNavDataTable bookedNavDataTable = new CHICMobileDataSet.bookedNavDataTable();

            string language = Session["language"].ToString();
            string countryCHICBrasil = "não existe";

            if (listCountries.Count == 0)
            {
                Country country1 = hlbapp.Country.Where(w => w.Language == language
                     && w.System == "CHIC Brazil" && w.Caption == country)
                     .FirstOrDefault();
                if (country1 != null)
                    countryCHICBrasil = country1.System_Name;

                if (country == "")
                    countryCHICBrasil = "";
            }
            else
            {
                countryCHICBrasil = "";
            }

            bookedNav.FillByNavOrders(bookedNavDataTable, dateType, initialDate, finalDate,
                dateType, initialDate, finalDate, dateType, initialDate, finalDate, location,
                location, countryCHICBrasil, countryCHICBrasil, customerName);

            foreach (var item in bookedNavDataTable)
            {
                string loc = "BR";

                string locationColor = hlbapp.NavLocations
                    .Where(w => w.Location == loc)
                    .FirstOrDefault().LocationColor;

                string chicOrderNo = Convert.ToInt32(item.orderno.Trim()).ToString();
                string navOrderNo = "";
                Nav_Orders navOrderR = hlbapp.Nav_Orders
                    .Where(w => w.OrderNumberCHIC == chicOrderNo)
                    .FirstOrDefault();

                if (navOrderR != null) navOrderNo = navOrderR.OrderNumberNavision.ToString();

                string description = "";
                if (item.alt_desc.Trim() != "")
                    description = item.alt_desc.Trim();
                else
                    description = item.item_desc.Trim();

                int SemanaAno = CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(item.cal_date.AddDays(21),
                    CalendarWeekRule.FirstDay, DayOfWeek.Sunday);

                Country countryObj = hlbapp.Country.Where(w => w.Language == language
                         && w.System == "CHIC Brazil" && w.System_Name == item.country.Trim())
                         .FirstOrDefault();

                string countryStr = "";
                if (countryObj != null)
                {
                    if (listCountries.Count == 0)
                    {
                        countryStr = countryObj.Caption;

                        NavOrders navOrder = new NavOrders
                        {
                            Location = loc,
                            LocationColor = locationColor,
                            CustomerName = item.name.Trim(),
                            Country = countryStr,
                            HatchDate = item.cal_date.AddDays(21),
                            SetDate = item.cal_date,
                            DeliveryDate = item.del_date,
                            OrderNo = item.orderno,
                            NavOrderNo = navOrderNo,
                            Line = item.variety,
                            Description = description,
                            Quantity = item.quantity,
                            NumeroSemanaAno = SemanaAno
                        };

                        listNavOrders.Add(navOrder);
                    }
                    else
                    {
                        foreach (var iCountry in listCountries)
                        {                        
                            if (iCountry == countryObj.Caption)
                            {
                                countryStr = countryObj.Caption;

                                NavOrders navOrder = new NavOrders
                                {
                                    Location = loc,
                                    LocationColor = locationColor,
                                    CustomerName = item.name.Trim(),
                                    Country = countryStr,
                                    HatchDate = item.cal_date.AddDays(21),
                                    SetDate = item.cal_date,
                                    DeliveryDate = item.del_date,
                                    OrderNo = item.orderno,
                                    NavOrderNo = navOrderNo,
                                    Line = item.variety,
                                    Description = description,
                                    Quantity = item.quantity,
                                    NumeroSemanaAno = SemanaAno
                                };

                                listNavOrders.Add(navOrder);
                            }
                        }
                    }
                }
            }

            #endregion

            #region Search Orders in Import Orders CHIC

            DateTime initialDateSet = initialDate.AddDays(-21);
            DateTime finalDateSet = finalDate.AddDays(-21);

            string countryCHICImports = "não existe";
            if (listCountries.Count == 0)
            {
                Country country2 = hlbapp.Country.Where(w => w.Language == language
                     && w.System == "CHIC Imports" && w.Caption == country)
                     .FirstOrDefault();
                if (country2 != null)
                    countryCHICImports = country2.System_Name;

                if (country == "")
                    countryCHICImports = "";
            }
            else
            {
                countryCHICImports = "";
            }

            var listImport = hlbapp.Nav_Orders_Import
                .Where(w => (w.location == location || location == "")
                    && (
                        (dateType == 0 && w.cal_date >= initialDate && w.cal_date <= finalDate)
                        ||
                        (dateType == 1 && w.hatch_date >= initialDate && w.hatch_date <= finalDate)
                        ||
                        (dateType == 2 && w.del_date >= initialDate && w.cal_date <= finalDate)
                       )
                    && (w.name.Contains(customerName) || customerName == "")
                    && (w.country == countryCHICImports || countryCHICImports == ""))
                .ToList();

            foreach (var item in listImport)
            {
                string loc = "";
                if (item.location.Trim().Equals("PH"))
                    loc = "BR";
                else
                    loc = item.location.Trim();

                string locationColor = hlbapp.NavLocations
                    .Where(w => w.Location == loc)
                    .FirstOrDefault().LocationColor;

                string chicOrderNo = item.orderno.ToString();
                string navOrderNo = "";
                Nav_Orders navOrderR = hlbapp.Nav_Orders
                    .Where(w => w.OrderNumberCHIC == chicOrderNo)
                    .FirstOrDefault();

                if (navOrderR != null) navOrderNo = navOrderR.OrderNumberNavision.ToString();

                string description = "";
                if (item.alt_desc.Trim() != "")
                    description = item.alt_desc;
                else
                    description = item.item_desc;

                int SemanaAno = CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(
                    Convert.ToDateTime(item.hatch_date),
                    CalendarWeekRule.FirstDay, DayOfWeek.Sunday);

                Country countryObj = hlbapp.Country.Where(w => w.Language == language
                     && w.System == "CHIC Imports" && w.System_Name == item.country.Trim())
                     .FirstOrDefault();

                string countryStr = "";
                if (countryObj != null)
                {
                    if (listCountries.Count == 0)
                    {
                        countryStr = countryObj.Caption;

                        NavOrders navOrder = new NavOrders
                        {
                            Location = loc,
                            LocationColor = locationColor,
                            CustomerName = item.name.Trim(),
                            Country = countryStr,
                            HatchDate = Convert.ToDateTime(item.hatch_date),
                            SetDate = Convert.ToDateTime(item.cal_date),
                            DeliveryDate = Convert.ToDateTime(item.del_date),
                            OrderNo = item.orderno.ToString(),
                            NavOrderNo = navOrderNo,
                            Description = description,
                            Line = item.variety,
                            Quantity = Convert.ToInt32(item.quantity),
                            NumeroSemanaAno = SemanaAno
                        };

                        listNavOrders.Add(navOrder);
                    }
                    else
                    {
                        foreach (var iCountry in listCountries)
                        {
                            if (iCountry == countryObj.Caption)
                            {
                                countryStr = countryObj.Caption;

                                NavOrders navOrder = new NavOrders
                                {
                                    Location = loc,
                                    LocationColor = locationColor,
                                    CustomerName = item.name.Trim(),
                                    Country = countryStr,
                                    HatchDate = Convert.ToDateTime(item.hatch_date),
                                    SetDate = Convert.ToDateTime(item.cal_date),
                                    DeliveryDate = Convert.ToDateTime(item.del_date),
                                    OrderNo = item.orderno.ToString(),
                                    NavOrderNo = navOrderNo,
                                    Description = description,
                                    Line = item.variety,
                                    Quantity = Convert.ToInt32(item.quantity),
                                    NumeroSemanaAno = SemanaAno
                                };

                                listNavOrders.Add(navOrder);
                            }
                        }
                    }
                }
            }

            /*string locationColorTeste = hlbapp.NavLocations
                    .Where(w => w.Location == "DO")
                    .FirstOrDefault().LocationColor;

            NavOrders navOrderTeste = new NavOrders
            {
                Location = "DO",
                LocationColor = locationColorTeste,
                CustomerName = "TESTE",
                Country = "DO",
                HatchDate = Convert.ToDateTime(DateTime.Today).AddDays(21),
                SetDate = Convert.ToDateTime(DateTime.Today),
                DeliveryDate = Convert.ToDateTime(DateTime.Today.AddDays(1)),
                OrderNo = "123",
                NavOrderNo = "",
                Line = "",
                Quantity = Convert.ToInt32(0)
            };

            listNavOrders.Add(navOrderTeste);

            locationColorTeste = hlbapp.NavLocations
                    .Where(w => w.Location == "CA")
                    .FirstOrDefault().LocationColor;

            NavOrders navOrderTeste2 = new NavOrders
            {
                Location = "CA",
                LocationColor = locationColorTeste,
                CustomerName = "TESTE",
                Country = "CA",
                HatchDate = Convert.ToDateTime(DateTime.Today).AddDays(21),
                SetDate = Convert.ToDateTime(DateTime.Today),
                DeliveryDate = Convert.ToDateTime(DateTime.Today.AddDays(1)),
                OrderNo = "123",
                NavOrderNo = "",
                Line = "",
                Quantity = Convert.ToInt32(0)
            };

            listNavOrders.Add(navOrderTeste2);*/

            #endregion

            #region Search Orders in Actual Orders Table

            initialDateSet = initialDate.AddDays(21);
            finalDateSet = finalDate.AddDays(21);

            string countryCHICActualOrders = "não existe";
            if (listCountries.Count == 0)
            {
                Country country3 = hlbapp.Country.Where(w => w.Language == language
                     && w.System == "Actual Orders" && w.Caption == country)
                     .FirstOrDefault();
                if (country3 != null)
                    countryCHICActualOrders = country3.System_Name;

                if (country == "")
                    countryCHICActualOrders = "";
            }
            else
            {
                countryCHICActualOrders = "";
            }

            var listActualOrders = hlbapp.Nav_Actual_Orders
                .Where(w => (hlbapp.NavLocations.Any(a => a.LocationNavision == w.LocationCode 
                                && a.Location == location || location == ""))
                    && (
                        (dateType == 0 && w.HatchDate >= initialDateSet && w.HatchDate <= finalDateSet)
                        ||
                        (dateType == 1 && w.HatchDate >= initialDate && w.HatchDate <= finalDate)
                        ||
                        (dateType == 2 && w.DelDate >= initialDate && w.DelDate <= finalDate)
                       )
                    && (w.Customer.Contains(customerName) || customerName == "")
                    && (w.Country == countryCHICActualOrders || countryCHICActualOrders == ""))
                .ToList();

            foreach (var item in listActualOrders)
            {
                NavLocations navLocation = hlbapp.NavLocations
                    .Where(w => w.LocationNavision == item.LocationCode)
                    .FirstOrDefault();

                string loc = "";
                if (navLocation.Location.Equals("PH"))
                    loc = "BR";
                else
                    loc = navLocation.Location;

                string locationColor = hlbapp.NavLocations
                    .Where(w => w.Location == loc)
                    .FirstOrDefault().LocationColor;

                string chicOrderNo = "";
                string navOrderNo = item.OrderNo;
                Nav_Orders navOrderR = hlbapp.Nav_Orders
                    .Where(w => w.OrderNumberNavision == navOrderNo)
                    .FirstOrDefault();

                if (navOrderR != null) chicOrderNo = navOrderR.OrderNumberCHIC;

                int SemanaAno = CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(
                    Convert.ToDateTime(item.HatchDate),
                    CalendarWeekRule.FirstDay, DayOfWeek.Sunday);

                string countryStr = "";
                Country countryObj = hlbapp.Country.Where(w => w.Language == language
                     && w.System == "Actual Orders" && w.System_Name == item.Country)
                     .FirstOrDefault();
                
                if (countryObj != null)
                {
                    if (listCountries.Count == 0)
                    {
                        countryStr = countryObj.Caption;

                        NavOrders navOrder = new NavOrders
                        {
                            Location = loc,
                            LocationColor = locationColor,
                            CustomerName = item.Customer,
                            Country = countryStr,
                            HatchDate = Convert.ToDateTime(item.HatchDate),
                            SetDate = Convert.ToDateTime(item.HatchDate).AddDays(-21),
                            DeliveryDate = Convert.ToDateTime(item.DelDate),
                            OrderNo = chicOrderNo,
                            NavOrderNo = item.OrderNo,
                            Line = item.Gender,
                            Description = item.Breed,
                            Quantity = Convert.ToInt32(item.Quantity),
                            NumeroSemanaAno = SemanaAno
                        };

                        listNavOrders.Add(navOrder);
                    }
                    else
                    {
                        foreach (var iCountry in listCountries)
                        {
                            if (iCountry == countryObj.Caption)
                            {
                                countryStr = countryObj.Caption;

                                NavOrders navOrder = new NavOrders
                                {
                                    Location = loc,
                                    LocationColor = locationColor,
                                    CustomerName = item.Customer,
                                    Country = countryStr,
                                    HatchDate = Convert.ToDateTime(item.HatchDate),
                                    SetDate = Convert.ToDateTime(item.HatchDate).AddDays(-21),
                                    DeliveryDate = Convert.ToDateTime(item.DelDate),
                                    OrderNo = chicOrderNo,
                                    NavOrderNo = item.OrderNo,
                                    Line = item.Gender,
                                    Description = item.Breed,
                                    Quantity = Convert.ToInt32(item.Quantity),
                                    NumeroSemanaAno = SemanaAno
                                };

                                listNavOrders.Add(navOrder);
                            }
                        }
                    }
                }
            }

            int semanaAnoAnterior = 0;
            int count = 1;
            foreach (var item in listNavOrders.OrderBy(o => o.NumeroSemanaAno).ToList())
            {
                if (semanaAnoAnterior != item.NumeroSemanaAno)
                {
                    item.NumeroSemanaMes = count;
                    count++;
                    semanaAnoAnterior = item.NumeroSemanaAno;
                }
            }

            #endregion

            if (dateType == 0)
                return listNavOrders.OrderBy(o => o.SetDate).ToList();
            else if (dateType == 1)
                return listNavOrders.OrderBy(o => o.HatchDate).ToList();
            else
                return listNavOrders.OrderBy(o => o.DeliveryDate).ToList();
        }

        public static List<NavOrders> StaticSearchNavOrders(DateTime initialDate, DateTime finalDate)
        {
            List<NavOrders> listNavOrders = new List<NavOrders>();

            HLBAPPEntities hlbapp = new HLBAPPEntities();

            #region Search Orders in Brazil CHIC

            CHICMobileDataSet chic = new CHICMobileDataSet();

            //custNavTableAdapter custNav = new custNavTableAdapter();
            //CHICMobileDataSet.custNavDataTable custNavDataTable = new CHICMobileDataSet.custNavDataTable();

            //custNav.Fill(custNavDataTable, initialDate, finalDate);

            bookedNavTableAdapter bookedNav = new bookedNavTableAdapter();
            CHICMobileDataSet.bookedNavDataTable bookedNavDataTable = new CHICMobileDataSet.bookedNavDataTable();

            bookedNav.FillByNavOrders(bookedNavDataTable, 1, initialDate, finalDate,
                1, initialDate, finalDate, 1, initialDate, finalDate, "",
                "", "", "", "");

            foreach (var item in bookedNavDataTable
                .GroupBy(g => new { g.name, g.cal_date })
                .Select(s => new { s.Key.name, s.Key.cal_date })
                .ToList())
            {
                string loc = "BR";

                string locationColor = hlbapp.NavLocations
                    .Where(w => w.Location == loc)
                    .FirstOrDefault().LocationColor;

                NavOrders navOrder = new NavOrders
                {
                    Location = loc,
                    LocationColor = locationColor,
                    CustomerName = item.name.Trim(),
                    HatchDate = item.cal_date.AddDays(22)
                };

                listNavOrders.Add(navOrder);
            }

            #endregion

            #region Search Orders in Import Orders CHIC

            var listImport = hlbapp.Nav_Orders_Import
                .Where(w => w.hatch_date >= initialDate && w.hatch_date <= finalDate)
                .GroupBy(g => new { g.location, g.name, g.hatch_date })
                .Select(s => new { s.Key.location, s.Key.name, s.Key.hatch_date })
                .ToList();

            foreach (var item in listImport)
            {
                string loc = "";
                if (item.location.Trim().Equals("PH"))
                    loc = "BR";
                else
                    loc = item.location.Trim();

                string locationColor = hlbapp.NavLocations
                    .Where(w => w.Location == loc)
                    .FirstOrDefault().LocationColor;

                NavOrders navOrder = new NavOrders
                {
                    Location = loc,
                    LocationColor = locationColor,
                    CustomerName = item.name.Trim(),
                    HatchDate = Convert.ToDateTime(item.hatch_date).AddDays(1)
                };

                listNavOrders.Add(navOrder);
            }

            #endregion

            #region Search Orders in Actual Orders Table

            var listActualOrders = hlbapp.Nav_Actual_Orders
                .Where(w => w.HatchDate >= initialDate && w.HatchDate <= finalDate)
                .GroupBy(g => new { g.LocationCode, g.Customer, g.HatchDate })
                .Select(s => new { s.Key.LocationCode, s.Key.Customer, s.Key.HatchDate })
                .ToList();

            foreach (var item in listActualOrders)
            {
                NavLocations navLocation = hlbapp.NavLocations
                    .Where(w => w.LocationNavision == item.LocationCode)
                    .FirstOrDefault();

                string loc = "";
                if (navLocation.Location.Equals("PH"))
                    loc = "BR";
                else
                    loc = navLocation.Location;

                string locationColor = hlbapp.NavLocations
                    .Where(w => w.Location == loc)
                    .FirstOrDefault().LocationColor;

                NavOrders navOrder = new NavOrders
                {
                    Location = loc,
                    LocationColor = locationColor,
                    CustomerName = item.Customer,
                    HatchDate = Convert.ToDateTime(item.HatchDate).AddDays(1)
                };

                listNavOrders.Add(navOrder);
            }

            #endregion

            return listNavOrders.OrderBy(o => o.CustomerName).ToList();
        }

        #endregion

        #region DropDown Methods

        public void LoadHatcheryList()
        {
            List<SelectListItem> items = new List<SelectListItem>();

            string language = Session["language"].ToString();

            string typeAll = hlbapp.Languages
                .Where(w => w.Caption == "Tab_Nav_Order_DropDown_HatcheryList_All"
                    && w.Language == language)
                .FirstOrDefault().Text;

            items.Add(new SelectListItem { Text = typeAll, Value = typeAll, Selected = true });
            items.Add(new SelectListItem { Text = "PH", Value = "PH", Selected = false });

            Session["HatcheryList"] = items;
        }

        public void UpdateHatcheryListSelected(string value)
        {
            List<SelectListItem> list = (List<SelectListItem>)Session["HatcheryList"];

            foreach (var item in list)
            {
                if (item.Value == value)
                {
                    item.Selected = true;
                }
                else
                {
                    item.Selected = false;
                }
            }

            Session["HatcheryList"] = list;
        }

        public void LoadDateTypeList()
        {
            List<SelectListItem> items = new List<SelectListItem>();

            string language = Session["language"].ToString();

            var list = hlbapp.Languages
                .Where(w => w.Caption == "Tab_Nav_Order_DropDown_DateTypeList"
                    && w.Language == language)
                .ToList();

            foreach (var item in list)
            {
                int index = list.FindIndex(i => i.Text == item.Text);
                if (index == 1)
                    items.Add(new SelectListItem { Text = item.Text, Value = index.ToString(), Selected = true });
                else
                    items.Add(new SelectListItem { Text = item.Text, Value = index.ToString(), Selected = false });
            }

            Session["DateTypeList"] = items;
        }

        public void UpdateDateTypeListSelected(string value)
        {
            List<SelectListItem> list = (List<SelectListItem>)Session["DateTypeList"];

            foreach (var item in list)
            {
                if (item.Value == value)
                {
                    item.Selected = true;
                }
                else
                {
                    item.Selected = false;
                }
            }

            Session["DateTypeList"] = list;
        }

        public void LoadCountryList()
        {
            List<SelectListItem> items = new List<SelectListItem>();

            string language = Session["language"].ToString();

            var list = hlbapp.Country
                .Where(w => w.Language == language)
                .GroupBy(g => new { g.Caption })
                .Select(s => new { s.Key.Caption })
                .ToList();

            string typeAll = hlbapp.Languages
                .Where(w => w.Caption == "Tab_Nav_Order_DropDown_HatcheryList_All"
                    && w.Language == language)
                .FirstOrDefault().Text;

            items.Add(new SelectListItem { Text = typeAll, Value = typeAll, Selected = true });

            foreach (var item in list)
            {
                items.Add(new SelectListItem { Text = item.Caption, Value = item.Caption, Selected = false });
            }

            Session["CountryList"] = items;
        }

        public void UpdateCountryListSelected(string value)
        {
            List<SelectListItem> list = (List<SelectListItem>)Session["CountryList"];

            foreach (var item in list)
            {
                if (item.Value == value)
                {
                    item.Selected = true;
                }
                else
                {
                    item.Selected = false;
                }
            }

            Session["CountryList"] = list;
        }

        public void UpdateCountryListSelected(List<String> listCount)
        {
            List<SelectListItem> list = (List<SelectListItem>)Session["CountryList"];

            foreach (var item in list)
            {
                String iCount = listCount.Where(w => w == item.Text).FirstOrDefault();
                if (item.Value == iCount)
                {
                    item.Selected = true;
                }
                else
                {
                    item.Selected = false;
                }
            }

            Session["CountryList"] = list;
        }

        public void LoadVisitorList()
        {
            List<SelectListItem> items = new List<SelectListItem>();

            string language = Session["language"].ToString();

            var list = hlbapp.Nav_Agenda
                .GroupBy(g => new { g.Visitor })
                .Select(s => new { s.Key.Visitor })
                .OrderBy(o => o.Visitor)
                .ToList();

            string typeAll = hlbapp.Languages
                .Where(w => w.Caption == "Tab_Nav_Order_DropDown_HatcheryList_All"
                    && w.Language == language)
                .FirstOrDefault().Text;

            items.Add(new SelectListItem { Text = typeAll, Value = typeAll, Selected = true });

            foreach (var item in list)
            {
                items.Add(new SelectListItem { Text = item.Visitor, Value = item.Visitor, Selected = false });
            }

            Session["VisitorList"] = items;
        }

        public void UpdateVisitorListSelected(string value)
        {
            List<SelectListItem> list = (List<SelectListItem>)Session["VisitorList"];

            foreach (var item in list)
            {
                if (item.Value == value)
                {
                    item.Selected = true;
                }
                else
                {
                    item.Selected = false;
                }
            }

            Session["VisitorList"] = list;
        }

        #endregion

        #region Others Methods

        public void LoadDateParams()
        {
            if (Session["initialDateSearchNav"] == null)
            {
                DateTime firstDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                Session["initialDateSearchNav"] = firstDate;
                Session["finalDateSearchNav"] = firstDate.AddMonths(1).AddDays(-1);
            }
        }

        public static string GetColorLocation(string location)
        {
            string result = "";
            HLBAPPEntities hlbapp = new HLBAPPEntities();

            NavLocations obj = hlbapp.NavLocations
                .Where(w => w.Location == location)
                .FirstOrDefault();

            if (obj != null)
                result = obj.LocationColor;

            return result;
        }

        public static string GetDetailsOrder(string location, string orderNo)
        {
            string retorno = "";



            return retorno;
        }

        public static String FromExcelTextBollean(Cell theCell, WorkbookPart wbPart)
        {
            string value = value = theCell.Descendants<CellValue>().First().Text;

            // If the cell represents an integer number, you are done. 
            // For dates, this code returns the serialized value that 
            // represents the date. The code handles strings and 
            // Booleans individually. For shared strings, the code 
            // looks up the corresponding value in the shared string 
            // table. For Booleans, the code converts the value into 
            // the words TRUE or FALSE.
            if (theCell.DataType != null)
            {
                switch (theCell.DataType.Value)
                {
                    case CellValues.SharedString:

                        // For shared strings, look up the value in the
                        // shared strings table.
                        var stringTable =
                            wbPart.GetPartsOfType<SharedStringTablePart>()
                            .FirstOrDefault();

                        // If the shared string table is missing, something 
                        // is wrong. Return the index that is in
                        // the cell. Otherwise, look up the correct text in 
                        // the table.
                        if (stringTable != null)
                        {
                            value =
                                stringTable.SharedStringTable
                                .ElementAt(int.Parse(value)).InnerText;
                        }
                        break;

                    case CellValues.Boolean:
                        switch (value)
                        {
                            case "0":
                                value = "FALSE";
                                break;
                            default:
                                value = "TRUE";
                                break;
                        }
                        break;
                }
            }

            return value;
        }

        public string VerificaFormatoArquivo(string caminho)
        {
            string formatoArquivo = Request.Files[0].ContentType;

            if (formatoArquivo.Equals("application/vnd.ms-excel"))
            {
                object oMissing = System.Reflection.Missing.Value;
                Excel.Application oExcel = new Excel.Application();

                oExcel.Visible = false;
                oExcel.DisplayAlerts = false;
                Excel.Workbooks oBooks = oExcel.Workbooks;
                Excel._Workbook oBook = null;
                oBook = oBooks.Open(caminho, false, oMissing,
                    oMissing, oMissing, oMissing, true, oMissing, oMissing,
                    //oMissing, oMissing, oMissing, oMissing, oMissing, XlCorruptLoad.xlRepairFile); // Quando abre arquivo corrompido
                    oMissing, false, oMissing, oMissing, oMissing, oMissing);

                caminho = caminho + "x";

                if (System.IO.File.Exists(caminho))
                {
                    System.IO.File.Delete(caminho);
                }

                oBook.SaveAs(caminho, Excel.XlFileFormat.xlOpenXMLWorkbook, System.Reflection.Missing.Value,
                    System.Reflection.Missing.Value, false, false, Excel.XlSaveAsAccessMode.xlNoChange,
                    Excel.XlSaveConflictResolution.xlOtherSessionChanges, false, System.Reflection.Missing.Value,
                    System.Reflection.Missing.Value, System.Reflection.Missing.Value);

                // Quit Excel and clean up.
                oBook.Close(true, oMissing, oMissing);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oBook);
                oBook = null;
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oBooks);
                oBooks = null;
                oExcel.Quit();
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oExcel);
                oExcel = null;

                //P.Kill();

                GC.Collect();
            }

            return caminho;
        }

        public static DateTime FromExcelSerialDate(int SerialDate)
        {
            if (SerialDate > 59) SerialDate -= 1; //Excel/Lotus 2/29/1900 bug   
            return new DateTime(1899, 12, 31).AddDays(SerialDate);
        }

        public static int GetWeekOfMonth(DateTime time)
        {
            DateTime first = new DateTime(time.Year, time.Month, 1);
            int weekTime = CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(
                    time, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
            int weekFirst = CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(
                    first, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
            return weekTime - weekFirst + 1;
        }

        public static DateTime FirstDateOfWeekISO8601(int year, int weekOfYear)
        {
            DateTime jan1 = new DateTime(year, 1, 1);
            int daysOffset = DayOfWeek.Thursday - jan1.DayOfWeek;

            DateTime firstThursday = jan1.AddDays(daysOffset);
            var cal = CultureInfo.CurrentCulture.Calendar;
            int firstWeek = cal.GetWeekOfYear(firstThursday, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);

            var weekNum = weekOfYear;
            if (firstWeek <= 1)
            {
                weekNum -= 1;
            }
            var result = firstThursday.AddDays(weekNum * 7);
            //return result.AddDays(-3).AddDays(6);
            return result.AddDays(-4);
        }

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

        #endregion

        #region Import Methods

        public string ImportExcelCHICExternal(Stream arquivo)
        {
            int erro = 0;
            try
            {
                string usuario = Session["usuario"].ToString();

                ViewBag.fileName = "Arquivo " + Request.Files[0].FileName + " importado com sucesso!";
                
                SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Open(arquivo, true);

                string relationshipId = spreadsheetDocument.WorkbookPart.Workbook.Descendants<Sheet>()
                                                    //.Where(s => s.Name == "CUSTS_ORD")
                                                    .First().Id;

                WorksheetPart worksheetPart = (WorksheetPart)spreadsheetDocument.WorkbookPart
                                                .GetPartById(relationshipId);

                SheetData sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();

                var lista = sheetData.Descendants<Row>().ToList();

                int insert = 0;
                foreach (var linha in lista)
                {
                    if (linha.RowIndex >= 2)
                    {
                        erro = (int)linha.RowIndex.Value;
                        if ((linha.Elements<Cell>()
                                .Where(c => c.CellReference.Value == "A" + linha.RowIndex)
                                .First().Descendants<CellValue>().Count() > 0))
                        {
                            if ((linha.Elements<Cell>()
                                    .Where(c => c.CellReference.Value == "A" + linha.RowIndex)
                                    .First().Descendants<CellValue>().FirstOrDefault().InnerText.Trim() != ""))
                            {
                                Cell celulaLocation = linha.Elements<Cell>()
                                            .Where(c => c.CellReference == "P" + linha.RowIndex).First();
                                string location = FromExcelTextBollean(celulaLocation, spreadsheetDocument.WorkbookPart);

                                Cell celulasalesman = linha.Elements<Cell>()
                                                .Where(c => c.CellReference == "AH" + linha.RowIndex).First();
                                string salesman = FromExcelTextBollean(celulasalesman, spreadsheetDocument.WorkbookPart);

                                if ((location != "PH") && (salesman.Trim() == "America"))
                                {
                                    #region Tratamento das Céluas para Variáveis

                                    Cell celulaHatchDate = linha.Elements<Cell>()
                                                .Where(c => c.CellReference == "A" + linha.RowIndex).First();
                                    string hatch_date = FromExcelTextBollean(celulaHatchDate, spreadsheetDocument.WorkbookPart);

                                    Cell celulaDelDate = linha.Elements<Cell>()
                                                .Where(c => c.CellReference == "B" + linha.RowIndex).First();
                                    string del_date = FromExcelTextBollean(celulaDelDate, spreadsheetDocument.WorkbookPart);

                                    if (del_date.Equals("  -   -"))
                                        del_date = "";

                                    string delivery = "";
                                    if (linha.Elements<Cell>()
                                            .Where(c => c.CellReference.Value == "C" + linha.RowIndex)
                                            .First().Descendants<CellValue>().Count() > 0)
                                    {
                                        if (linha.Elements<Cell>()
                                                .Where(c => c.CellReference.Value == "C" + linha.RowIndex)
                                                .First().Descendants<CellValue>().FirstOrDefault().InnerText.Trim() != "")
                                        {
                                            Cell celulaDelivery = linha.Elements<Cell>()
                                                .Where(c => c.CellReference == "C" + linha.RowIndex).First();
                                            delivery = FromExcelTextBollean(celulaDelivery, spreadsheetDocument.WorkbookPart);
                                        }
                                    }

                                    string farm_loc = "";
                                    if (linha.Elements<Cell>()
                                            .Where(c => c.CellReference.Value == "D" + linha.RowIndex)
                                            .First().Descendants<CellValue>().Count() > 0)
                                    {
                                        if (linha.Elements<Cell>()
                                                .Where(c => c.CellReference.Value == "D" + linha.RowIndex)
                                                .First().Descendants<CellValue>().FirstOrDefault().InnerText.Trim() != "")
                                        {
                                            Cell celulaDelivery = linha.Elements<Cell>()
                                                .Where(c => c.CellReference == "D" + linha.RowIndex).First();
                                            farm_loc = FromExcelTextBollean(celulaDelivery, spreadsheetDocument.WorkbookPart);
                                        }
                                    }

                                    string book_id = linha.Elements<Cell>()
                                        .Where(c => c.CellReference.Value == "E" + linha.RowIndex)
                                        .First().Descendants<CellValue>().FirstOrDefault().InnerText.Trim();

                                    Cell celulaCalDate = linha.Elements<Cell>()
                                                .Where(c => c.CellReference == "F" + linha.RowIndex).First();
                                    string cal_date = FromExcelTextBollean(celulaCalDate, spreadsheetDocument.WorkbookPart);

                                    Cell celulaCustomer = linha.Elements<Cell>()
                                                .Where(c => c.CellReference == "G" + linha.RowIndex).First();
                                    string customer = FromExcelTextBollean(celulaCustomer, spreadsheetDocument.WorkbookPart);

                                    Cell celulaItem = linha.Elements<Cell>()
                                                .Where(c => c.CellReference == "H" + linha.RowIndex).First();
                                    string item = FromExcelTextBollean(celulaItem, spreadsheetDocument.WorkbookPart);

                                    int quantity = 0;
                                    if (linha.Elements<Cell>()
                                            .Where(c => c.CellReference.Value == "I" + linha.RowIndex)
                                            .First().Descendants<CellValue>().Count() > 0)
                                    {
                                        if (linha.Elements<Cell>()
                                                .Where(c => c.CellReference.Value == "I" + linha.RowIndex)
                                                .First().Descendants<CellValue>().FirstOrDefault().InnerText.Trim() != "")
                                        {
                                            quantity = Convert.ToInt32(linha.Elements<Cell>().Where(c => c.CellReference == "I" + linha.RowIndex).First().Descendants<CellValue>().FirstOrDefault().Text);
                                        }
                                    }

                                    int price = 0;
                                    if (linha.Elements<Cell>()
                                            .Where(c => c.CellReference.Value == "J" + linha.RowIndex)
                                            .First().Descendants<CellValue>().Count() > 0)
                                    {
                                        if (linha.Elements<Cell>()
                                                .Where(c => c.CellReference.Value == "J" + linha.RowIndex)
                                                .First().Descendants<CellValue>().FirstOrDefault().InnerText.Trim() != "")
                                        {
                                            price = Convert.ToInt32(linha.Elements<Cell>().Where(c => c.CellReference == "J" + linha.RowIndex).First().Descendants<CellValue>().FirstOrDefault().Text);
                                        }
                                    }

                                    Cell celulaOrderNo = linha.Elements<Cell>()
                                                .Where(c => c.CellReference == "K" + linha.RowIndex).First();
                                    string orderno = FromExcelTextBollean(celulaOrderNo, spreadsheetDocument.WorkbookPart);

                                    Cell celulaOrderType = linha.Elements<Cell>()
                                                .Where(c => c.CellReference == "L" + linha.RowIndex).First();
                                    string orderType = FromExcelTextBollean(celulaOrderType, spreadsheetDocument.WorkbookPart);

                                    string comment_1 = "";
                                    if (linha.Elements<Cell>()
                                            .Where(c => c.CellReference.Value == "M" + linha.RowIndex)
                                            .First().Descendants<CellValue>().Count() > 0)
                                    {
                                        if (linha.Elements<Cell>()
                                                .Where(c => c.CellReference.Value == "M" + linha.RowIndex)
                                                .First().Descendants<CellValue>().FirstOrDefault().InnerText.Trim() != "")
                                        {
                                            Cell celula = linha.Elements<Cell>()
                                                .Where(c => c.CellReference == "M" + linha.RowIndex).First();
                                            comment_1 = FromExcelTextBollean(celula, spreadsheetDocument.WorkbookPart);
                                        }
                                    }

                                    string comment_2 = "";
                                    if (linha.Elements<Cell>()
                                            .Where(c => c.CellReference.Value == "N" + linha.RowIndex)
                                            .First().Descendants<CellValue>().Count() > 0)
                                    {
                                        if (linha.Elements<Cell>()
                                                .Where(c => c.CellReference.Value == "N" + linha.RowIndex)
                                                .First().Descendants<CellValue>().FirstOrDefault().InnerText.Trim() != "")
                                        {
                                            Cell celula = linha.Elements<Cell>()
                                                .Where(c => c.CellReference == "N" + linha.RowIndex).First();
                                            comment_2 = FromExcelTextBollean(celula, spreadsheetDocument.WorkbookPart);
                                        }
                                    }

                                    string comment_3 = "";
                                    if (linha.Elements<Cell>()
                                            .Where(c => c.CellReference.Value == "O" + linha.RowIndex)
                                            .First().Descendants<CellValue>().Count() > 0)
                                    {
                                        if (linha.Elements<Cell>()
                                                .Where(c => c.CellReference.Value == "O" + linha.RowIndex)
                                                .First().Descendants<CellValue>().FirstOrDefault().InnerText.Trim() != "")
                                        {
                                            Cell celula = linha.Elements<Cell>()
                                                .Where(c => c.CellReference == "O" + linha.RowIndex).First();
                                            comment_3 = FromExcelTextBollean(celula, spreadsheetDocument.WorkbookPart);
                                        }
                                    }

                                    string accountno = "";
                                    if (linha.Elements<Cell>()
                                            .Where(c => c.CellReference.Value == "Q" + linha.RowIndex)
                                            .First().Descendants<CellValue>().Count() > 0)
                                    {
                                        if (linha.Elements<Cell>()
                                                .Where(c => c.CellReference.Value == "Q" + linha.RowIndex)
                                                .First().Descendants<CellValue>().FirstOrDefault().InnerText.Trim() != "")
                                        {
                                            Cell celula = linha.Elements<Cell>()
                                                .Where(c => c.CellReference == "Q" + linha.RowIndex).First();
                                            accountno = FromExcelTextBollean(celula, spreadsheetDocument.WorkbookPart);
                                        }
                                    }

                                    string alt_desc = "";
                                    if (linha.Elements<Cell>()
                                            .Where(c => c.CellReference.Value == "R" + linha.RowIndex)
                                            .First().Descendants<CellValue>().Count() > 0)
                                    {
                                        if (linha.Elements<Cell>()
                                                .Where(c => c.CellReference.Value == "R" + linha.RowIndex)
                                                .First().Descendants<CellValue>().FirstOrDefault().InnerText.Trim() != "")
                                        {
                                            Cell celula = linha.Elements<Cell>()
                                                .Where(c => c.CellReference == "R" + linha.RowIndex).First();
                                            alt_desc = FromExcelTextBollean(celula, spreadsheetDocument.WorkbookPart);
                                        }
                                    }

                                    Cell celulaItemOrd = linha.Elements<Cell>()
                                                .Where(c => c.CellReference == "S" + linha.RowIndex).First();
                                    string item_Ord = FromExcelTextBollean(celulaItemOrd, spreadsheetDocument.WorkbookPart);

                                    Cell celulacreatdby = linha.Elements<Cell>()
                                                .Where(c => c.CellReference == "T" + linha.RowIndex).First();
                                    string creatdby = FromExcelTextBollean(celulacreatdby, spreadsheetDocument.WorkbookPart);

                                    Cell celuladatecrtd = linha.Elements<Cell>()
                                                .Where(c => c.CellReference == "U" + linha.RowIndex).First();
                                    string datecrtd = FromExcelTextBollean(celuladatecrtd, spreadsheetDocument.WorkbookPart);

                                    Cell celulamodifdby = linha.Elements<Cell>()
                                                .Where(c => c.CellReference == "V" + linha.RowIndex).First();
                                    string modifdby = FromExcelTextBollean(celulamodifdby, spreadsheetDocument.WorkbookPart);

                                    Cell celuladatemodi = linha.Elements<Cell>()
                                                .Where(c => c.CellReference == "W" + linha.RowIndex).First();
                                    string datemodi = FromExcelTextBollean(celuladatemodi, spreadsheetDocument.WorkbookPart);

                                    Cell celulaitm_ddate = linha.Elements<Cell>()
                                                .Where(c => c.CellReference == "X" + linha.RowIndex).First();
                                    string itm_ddate = FromExcelTextBollean(celulaitm_ddate, spreadsheetDocument.WorkbookPart);

                                    string vat = linha.Elements<Cell>()
                                        .Where(c => c.CellReference.Value == "Y" + linha.RowIndex)
                                        .First().Descendants<CellValue>().FirstOrDefault().InnerText.Trim();

                                    string salesrep = "";
                                    if (linha.Elements<Cell>()
                                            .Where(c => c.CellReference.Value == "Z" + linha.RowIndex)
                                            .First().Descendants<CellValue>().Count() > 0)
                                    {
                                        if (linha.Elements<Cell>()
                                                .Where(c => c.CellReference.Value == "Z" + linha.RowIndex)
                                                .First().Descendants<CellValue>().FirstOrDefault().InnerText.Trim() != "")
                                        {
                                            Cell celula = linha.Elements<Cell>()
                                                .Where(c => c.CellReference == "Z" + linha.RowIndex).First();
                                            salesrep = FromExcelTextBollean(celula, spreadsheetDocument.WorkbookPart);
                                        }
                                    }

                                    string bookkey = linha.Elements<Cell>()
                                        .Where(c => c.CellReference.Value == "AA" + linha.RowIndex)
                                        .First().Descendants<CellValue>().FirstOrDefault().InnerText.Trim();

                                    Cell celulaitem_desc = linha.Elements<Cell>()
                                                .Where(c => c.CellReference == "AB" + linha.RowIndex).First();
                                    string item_desc = FromExcelTextBollean(celulaitem_desc, spreadsheetDocument.WorkbookPart);

                                    Cell celulavariety = linha.Elements<Cell>()
                                                .Where(c => c.CellReference == "AC" + linha.RowIndex).First();
                                    string variety = FromExcelTextBollean(celulavariety, spreadsheetDocument.WorkbookPart);

                                    Cell celulaform = linha.Elements<Cell>()
                                                .Where(c => c.CellReference == "AD" + linha.RowIndex).First();
                                    string form = FromExcelTextBollean(celulaform, spreadsheetDocument.WorkbookPart);

                                    Cell celulaname = linha.Elements<Cell>()
                                                .Where(c => c.CellReference == "AE" + linha.RowIndex).First();
                                    string name = FromExcelTextBollean(celulaname, spreadsheetDocument.WorkbookPart);

                                    string contact_no = linha.Elements<Cell>()
                                        .Where(c => c.CellReference.Value == "AF" + linha.RowIndex)
                                        .First().Descendants<CellValue>().FirstOrDefault().InnerText.Trim();

                                    string shpname = "";
                                    if (linha.Elements<Cell>()
                                            .Where(c => c.CellReference.Value == "AG" + linha.RowIndex)
                                            .First().Descendants<CellValue>().Count() > 0)
                                    {
                                        if (linha.Elements<Cell>()
                                                .Where(c => c.CellReference.Value == "AG" + linha.RowIndex)
                                                .First().Descendants<CellValue>().FirstOrDefault().InnerText.Trim() != "")
                                        {
                                            Cell celula = linha.Elements<Cell>()
                                                .Where(c => c.CellReference == "AG" + linha.RowIndex).First();
                                            shpname = FromExcelTextBollean(celula, spreadsheetDocument.WorkbookPart);
                                        }
                                    }

                                    string account_no2 = "";
                                    if (linha.Elements<Cell>()
                                            .Where(c => c.CellReference.Value == "AI" + linha.RowIndex)
                                            .First().Descendants<CellValue>().Count() > 0)
                                    {
                                        if (linha.Elements<Cell>()
                                                .Where(c => c.CellReference.Value == "AI" + linha.RowIndex)
                                                .First().Descendants<CellValue>().FirstOrDefault().InnerText.Trim() != "")
                                        {
                                            Cell celula = linha.Elements<Cell>()
                                                .Where(c => c.CellReference == "AI" + linha.RowIndex).First();
                                            account_no2 = FromExcelTextBollean(celula, spreadsheetDocument.WorkbookPart);
                                        }
                                    }

                                    Cell celulasl_code = linha.Elements<Cell>()
                                                .Where(c => c.CellReference == "AJ" + linha.RowIndex).First();
                                    string sl_code = FromExcelTextBollean(celulasl_code, spreadsheetDocument.WorkbookPart);

                                    string street_1 = "";
                                    if (linha.Elements<Cell>()
                                            .Where(c => c.CellReference.Value == "AK" + linha.RowIndex)
                                            .First().Descendants<CellValue>().Count() > 0)
                                    {
                                        if (linha.Elements<Cell>()
                                                .Where(c => c.CellReference.Value == "AK" + linha.RowIndex)
                                                .First().Descendants<CellValue>().FirstOrDefault().InnerText.Trim() != "")
                                        {
                                            Cell celula = linha.Elements<Cell>()
                                                .Where(c => c.CellReference == "AK" + linha.RowIndex).First();
                                            street_1 = FromExcelTextBollean(celula, spreadsheetDocument.WorkbookPart);
                                        }
                                    }

                                    string street_2 = "";
                                    if (linha.Elements<Cell>()
                                            .Where(c => c.CellReference.Value == "AL" + linha.RowIndex)
                                            .First().Descendants<CellValue>().Count() > 0)
                                    {
                                        if (linha.Elements<Cell>()
                                                .Where(c => c.CellReference.Value == "AL" + linha.RowIndex)
                                                .First().Descendants<CellValue>().FirstOrDefault().InnerText.Trim() != "")
                                        {
                                            Cell celula = linha.Elements<Cell>()
                                                .Where(c => c.CellReference == "AL" + linha.RowIndex).First();
                                            street_2 = FromExcelTextBollean(celula, spreadsheetDocument.WorkbookPart);
                                        }
                                    }

                                    string city = "";
                                    if (linha.Elements<Cell>()
                                            .Where(c => c.CellReference.Value == "AM" + linha.RowIndex)
                                            .First().Descendants<CellValue>().Count() > 0)
                                    {
                                        if (linha.Elements<Cell>()
                                                .Where(c => c.CellReference.Value == "AM" + linha.RowIndex)
                                                .First().Descendants<CellValue>().FirstOrDefault().InnerText.Trim() != "")
                                        {
                                            Cell celula = linha.Elements<Cell>()
                                                .Where(c => c.CellReference == "AM" + linha.RowIndex).First();
                                            city = FromExcelTextBollean(celula, spreadsheetDocument.WorkbookPart);
                                        }
                                    }

                                    string state = "";
                                    if (linha.Elements<Cell>()
                                            .Where(c => c.CellReference.Value == "AN" + linha.RowIndex)
                                            .First().Descendants<CellValue>().Count() > 0)
                                    {
                                        if (linha.Elements<Cell>()
                                                .Where(c => c.CellReference.Value == "AN" + linha.RowIndex)
                                                .First().Descendants<CellValue>().FirstOrDefault().InnerText.Trim() != "")
                                        {
                                            Cell celula = linha.Elements<Cell>()
                                                .Where(c => c.CellReference == "AN" + linha.RowIndex).First();
                                            state = FromExcelTextBollean(celula, spreadsheetDocument.WorkbookPart);
                                        }
                                    }

                                    string zip = "";
                                    if (linha.Elements<Cell>()
                                            .Where(c => c.CellReference.Value == "AO" + linha.RowIndex)
                                            .First().Descendants<CellValue>().Count() > 0)
                                    {
                                        if (linha.Elements<Cell>()
                                                .Where(c => c.CellReference.Value == "AO" + linha.RowIndex)
                                                .First().Descendants<CellValue>().FirstOrDefault().InnerText.Trim() != "")
                                        {
                                            Cell celula = linha.Elements<Cell>()
                                                .Where(c => c.CellReference == "AO" + linha.RowIndex).First();
                                            zip = FromExcelTextBollean(celula, spreadsheetDocument.WorkbookPart);
                                        }
                                    }

                                    string country = "";
                                    if (linha.Elements<Cell>()
                                            .Where(c => c.CellReference.Value == "AP" + linha.RowIndex)
                                            .First().Descendants<CellValue>().Count() > 0)
                                    {
                                        if (linha.Elements<Cell>()
                                                .Where(c => c.CellReference.Value == "AP" + linha.RowIndex)
                                                .First().Descendants<CellValue>().FirstOrDefault().InnerText.Trim() != "")
                                        {
                                            Cell celula = linha.Elements<Cell>()
                                                .Where(c => c.CellReference == "AP" + linha.RowIndex).First();
                                            country = FromExcelTextBollean(celula, spreadsheetDocument.WorkbookPart);
                                        }
                                    }

                                    #endregion

                                    #region (DESATIVADO) Verifica se Existe o Pedido Inserido. Caso exista, será atualizado.

                                    /*int ordernoSearch = Convert.ToInt32(orderno);
                                    int bookidSearch = Convert.ToInt32(book_id);
                                    Nav_Orders_Import navOrderImport = hlbapp.Nav_Orders_Import
                                        .Where(w => w.orderno == ordernoSearch && w.location == location
                                            && w.book_id == bookidSearch)
                                        .FirstOrDefault();

                                    int insert = 0;
                                    if (navOrderImport == null)
                                    {
                                        insert = 1;
                                        navOrderImport = new Nav_Orders_Import();
                                    }*/

                                    #endregion

                                    #region Verifica se Existe o Pedido Inserido. Caso exista, será deletado.

                                    int ordernoSearch = Convert.ToInt32(orderno);
                                    int bookidSearch = Convert.ToInt32(book_id);
                                    var navOrderImportList = hlbapp.Nav_Orders_Import
                                        .Where(w => w.orderno == ordernoSearch && w.location == location)
                                        .ToList();

                                    if ((navOrderImportList.Count > 0) && (insert != ordernoSearch))
                                    {
                                        foreach (var itemNavOrderImport in navOrderImportList)
                                        {
                                            hlbapp.Nav_Orders_Import.DeleteObject(itemNavOrderImport);
                                        }

                                        hlbapp.SaveChanges();
                                        
                                        insert = ordernoSearch;
                                    }

                                    Nav_Orders_Import navOrderImport = new Nav_Orders_Import();

                                    #endregion

                                    #region Insere no Objeto

                                    navOrderImport.hatch_date = Convert.ToDateTime(hatch_date);
                                    if (del_date.Equals(""))
                                        navOrderImport.del_date = null;
                                    else
                                        navOrderImport.del_date = Convert.ToDateTime(del_date);
                                    navOrderImport.delivery = delivery;
                                    navOrderImport.farm_loc = farm_loc;
                                    navOrderImport.book_id = Convert.ToInt32(book_id);
                                    navOrderImport.cal_date = Convert.ToDateTime(cal_date);
                                    navOrderImport.customer = customer;
                                    navOrderImport.item = Convert.ToInt32(item);
                                    navOrderImport.quantity = quantity;
                                    navOrderImport.price = price;
                                    navOrderImport.orderno = Convert.ToInt32(orderno);
                                    navOrderImport.order_type = orderType;
                                    navOrderImport.comment_1 = comment_1;
                                    navOrderImport.comment_2 = comment_2;
                                    navOrderImport.comment_3 = comment_3;
                                    navOrderImport.location = location;
                                    navOrderImport.accountno = accountno;
                                    navOrderImport.alt_desc = alt_desc;
                                    navOrderImport.item_ord = item_Ord;
                                    navOrderImport.creatdby = creatdby;
                                    navOrderImport.datecrtd = Convert.ToDateTime(datecrtd);
                                    navOrderImport.modifdby = modifdby;
                                    navOrderImport.datemodi = Convert.ToDateTime(datemodi);
                                    navOrderImport.itm_ddate = itm_ddate;
                                    navOrderImport.vat = Convert.ToInt32(vat);
                                    navOrderImport.salesrep = salesrep;
                                    navOrderImport.bookkey = Convert.ToInt32(bookkey);
                                    navOrderImport.item_desc = item_desc;
                                    navOrderImport.variety = variety;
                                    navOrderImport.form = form;
                                    navOrderImport.name = name;
                                    navOrderImport.contact_no = Convert.ToInt32(contact_no);
                                    navOrderImport.shpname = shpname;
                                    navOrderImport.salesman = salesman;
                                    navOrderImport.account_no = account_no2;
                                    navOrderImport.sl_code = sl_code;
                                    navOrderImport.street_1 = street_1;
                                    navOrderImport.street_2 = street_2;
                                    navOrderImport.city = city;
                                    navOrderImport.state = state;
                                    navOrderImport.zip = zip;
                                    navOrderImport.country = country;
                                    navOrderImport.usuario = usuario;

                                    //if (insert == 1)
                                    hlbapp.Nav_Orders_Import.AddObject(navOrderImport);

                                    #endregion

                                }
                            }
                        }
                    }
                }

                arquivo.Close();

                hlbapp.SaveChanges();

                return "";
            }
            catch (Exception e)
            {
                arquivo.Close();
                return "Erro ao realizar a importação: " + e.Message;
            }
        }

        public string ImportExcelActualOrders(Stream arquivo)
        {
            int erro = 0;
            try
            {
                string usuario = Session["usuario"].ToString();

                ViewBag.fileName = "Arquivo " + Request.Files[0].FileName + " importado com sucesso!";

                SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Open(arquivo, true);

                string relationshipId = spreadsheetDocument.WorkbookPart.Workbook.Descendants<Sheet>()
                                                    .Where(s => s.Name == "Report")
                                                    .First().Id;

                WorksheetPart worksheetPart = (WorksheetPart)spreadsheetDocument.WorkbookPart
                                                .GetPartById(relationshipId);

                SheetData sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();

                var lista = sheetData.Descendants<Row>().ToList();

                string insert = "";
                foreach (var linha in lista)
                {
                    if (linha.RowIndex >= 2)
                    {
                        if (linha.RowIndex == 215)
                            erro = (int)linha.RowIndex.Value;

                        erro = (int)linha.RowIndex.Value;
                        if ((linha.Elements<Cell>()
                                .Where(c => c.CellReference.Value == "D" + linha.RowIndex)
                                .First().Descendants<CellValue>().Count() > 0))
                        {
                            if ((linha.Elements<Cell>()
                                    .Where(c => c.CellReference.Value == "D" + linha.RowIndex)
                                    .First().Descendants<CellValue>().FirstOrDefault().InnerText.Trim() != ""))
                            {
                                Cell celulaLocation = linha.Elements<Cell>()
                                            .Where(c => c.CellReference == "D" + linha.RowIndex).First();
                                string location = FromExcelTextBollean(celulaLocation, spreadsheetDocument.WorkbookPart);

                                if (location == "BES")
                                {
                                    #region Tratamento das Céluas para Variáveis

                                    Cell celulaWeek = linha.Elements<Cell>()
                                                .Where(c => c.CellReference == "A" + linha.RowIndex).First();
                                    string week = FromExcelTextBollean(celulaWeek, spreadsheetDocument.WorkbookPart);

                                    Cell celulaHatchDate = linha.Elements<Cell>()
                                                .Where(c => c.CellReference == "B" + linha.RowIndex).First();
                                    string hatch_date = FromExcelTextBollean(celulaHatchDate, spreadsheetDocument.WorkbookPart);

                                    DateTime? del_date = null;
                                    if (linha.Elements<Cell>()
                                            .Where(c => c.CellReference.Value == "C" + linha.RowIndex)
                                            .First().Descendants<CellValue>().Count() > 0)
                                    {
                                        if (linha.Elements<Cell>()
                                                .Where(c => c.CellReference.Value == "C" + linha.RowIndex)
                                                .First().Descendants<CellValue>().FirstOrDefault().InnerText.Trim() != "")
                                        {
                                            Cell celula = linha.Elements<Cell>()
                                                .Where(c => c.CellReference == "C" + linha.RowIndex).First();
                                            del_date = FromExcelSerialDate(Convert.ToInt32(celula.Descendants<CellValue>().First().Text));
                                        }
                                    }                                        

                                    Cell celulaOrderNo = linha.Elements<Cell>()
                                                .Where(c => c.CellReference == "E" + linha.RowIndex).First();
                                    string orderno = FromExcelTextBollean(celulaOrderNo, spreadsheetDocument.WorkbookPart);

                                    string country = "";
                                    if (linha.Elements<Cell>()
                                            .Where(c => c.CellReference.Value == "F" + linha.RowIndex)
                                            .First().Descendants<CellValue>().Count() > 0)
                                    {
                                        if (linha.Elements<Cell>()
                                                .Where(c => c.CellReference.Value == "F" + linha.RowIndex)
                                                .First().Descendants<CellValue>().FirstOrDefault().InnerText.Trim() != "")
                                        {
                                            Cell celula = linha.Elements<Cell>()
                                                .Where(c => c.CellReference == "F" + linha.RowIndex).First();
                                            country = FromExcelTextBollean(celula, spreadsheetDocument.WorkbookPart);
                                        }
                                    }

                                    Cell celulaCustomer = linha.Elements<Cell>()
                                                .Where(c => c.CellReference == "G" + linha.RowIndex).First();
                                    string customer = FromExcelTextBollean(celulaCustomer, spreadsheetDocument.WorkbookPart);

                                    Cell celulaClient = linha.Elements<Cell>()
                                                .Where(c => c.CellReference == "H" + linha.RowIndex).First();
                                    string client = FromExcelTextBollean(celulaClient, spreadsheetDocument.WorkbookPart);

                                    Cell celulaBreed = linha.Elements<Cell>()
                                                .Where(c => c.CellReference == "I" + linha.RowIndex).First();
                                    string breed = FromExcelTextBollean(celulaBreed, spreadsheetDocument.WorkbookPart);

                                    decimal price = 0;
                                    if (linha.Elements<Cell>()
                                            .Where(c => c.CellReference.Value == "J" + linha.RowIndex)
                                            .First().Descendants<CellValue>().Count() > 0)
                                    {
                                        if (linha.Elements<Cell>()
                                                .Where(c => c.CellReference.Value == "J" + linha.RowIndex)
                                                .First().Descendants<CellValue>().FirstOrDefault().InnerText.Trim() != "")
                                        {
                                            //price = Convert.ToInt32(linha.Elements<Cell>().Where(c => c.CellReference == "J" + linha.RowIndex).First().Descendants<CellValue>().FirstOrDefault().Text);
                                            price = Convert.ToDecimal(double.Parse(linha.Elements<Cell>().Where(c => c.CellReference == "J" + linha.RowIndex).First().Descendants<CellValue>().FirstOrDefault().Text.Replace(".", ",")));
                                        }
                                    }

                                    int quantity = 0;
                                    if (linha.Elements<Cell>()
                                            .Where(c => c.CellReference.Value == "K" + linha.RowIndex)
                                            .First().Descendants<CellValue>().Count() > 0)
                                    {
                                        if (linha.Elements<Cell>()
                                                .Where(c => c.CellReference.Value == "K" + linha.RowIndex)
                                                .First().Descendants<CellValue>().FirstOrDefault().InnerText.Trim() != "")
                                        {
                                            quantity = Convert.ToInt32(linha.Elements<Cell>()
                                                .Where(c => c.CellReference == "K" + linha.RowIndex).First()
                                                .Descendants<CellValue>().FirstOrDefault().Text);
                                        }
                                    }

                                    Cell celulaQuoteNo = linha.Elements<Cell>()
                                                .Where(c => c.CellReference == "L" + linha.RowIndex).First();
                                    string quoteNo = FromExcelTextBollean(celulaQuoteNo, spreadsheetDocument.WorkbookPart);

                                    Cell celulaGender = linha.Elements<Cell>()
                                                .Where(c => c.CellReference == "M" + linha.RowIndex).First();
                                    string gender = FromExcelTextBollean(celulaGender, spreadsheetDocument.WorkbookPart);

                                    #endregion

                                    #region (DESATIVADO) Verifica se Existe o Pedido Inserido. Caso exista, será atualizado.

                                    //Nav_Actual_Orders navActualOrder = hlbapp.Nav_Actual_Orders
                                    //    .Where(w => w.OrderNo == orderno && w.LocationCode == location
                                    //        && w.Gender == gender && w.Quantity == quantity)
                                    //    .FirstOrDefault();

                                    //int insert = 0;
                                    //if (navActualOrder == null)
                                    //{
                                    //    insert = 1;
                                    //    navActualOrder = new Nav_Actual_Orders();
                                    //}

                                    #endregion

                                    #region Verifica se Existe o Pedido Inserido. Caso exista, será deletado.

                                    var navActualOrderList = hlbapp.Nav_Actual_Orders
                                        .Where(w => w.OrderNo == orderno && w.LocationCode == location)
                                        .ToList();

                                    if ((navActualOrderList.Count > 0) && (insert != orderno))
                                    {
                                        foreach (var itemNavActualOrder in navActualOrderList)
                                        {
                                            hlbapp.Nav_Actual_Orders.DeleteObject(itemNavActualOrder);
                                        }

                                        hlbapp.SaveChanges();

                                        insert = orderno;
                                    }

                                    Nav_Actual_Orders navActualOrder = new Nav_Actual_Orders();

                                    #endregion

                                    #region Insere no Objeto

                                    navActualOrder.Week = week;
                                    navActualOrder.HatchDate = Convert.ToDateTime(hatch_date);
                                    navActualOrder.DelDate = del_date;
                                    navActualOrder.LocationCode = location;
                                    navActualOrder.OrderNo = orderno;
                                    navActualOrder.Country = country;
                                    navActualOrder.Customer = customer;
                                    navActualOrder.Client = client;
                                    navActualOrder.Breed = breed;
                                    navActualOrder.UnitPrice = price;
                                    navActualOrder.Quantity = quantity;
                                    navActualOrder.QuoteNo = quoteNo;
                                    navActualOrder.Gender = gender;
                                    navActualOrder.Usuario = usuario;

                                    //if (insert == 1)
                                    hlbapp.Nav_Actual_Orders.AddObject(navActualOrder);

                                    #endregion

                                }
                            }
                        }
                    }
                }

                arquivo.Close();

                hlbapp.SaveChanges();

                return "";
            }
            catch (Exception e)
            {
                arquivo.Close();
                return "Erro ao realizar a importação: " + e.Message;
            }
        }

        public string ImportExcelAgenda(Stream arquivo)
        {
            int erro = 0;
            try
            {
                string usuario = Session["usuario"].ToString();

                ViewBag.fileName = "Arquivo " + Request.Files[0].FileName + " importado com sucesso!";

                SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Open(arquivo, true);

                string relationshipId = spreadsheetDocument.WorkbookPart.Workbook.Descendants<Sheet>()
                    //.Where(s => s.Name == "CUSTS_ORD")
                                                    .First().Id;

                WorksheetPart worksheetPart = (WorksheetPart)spreadsheetDocument.WorkbookPart
                                                .GetPartById(relationshipId);

                SheetData sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();

                Row linhaVisitor = sheetData.Elements<Row>().Where(r => r.RowIndex == 2).First();

                var lista = sheetData.Descendants<Row>().ToList();

                foreach (var linha in lista)
                {
                    if (linha.RowIndex >= 3)
                    {
                        erro = (int)linha.RowIndex.Value;
                        if ((linha.Elements<Cell>()
                                .Where(c => c.CellReference.Value == "A" + linha.RowIndex)
                                .First().Descendants<CellValue>().Count() > 0))
                        {
                            if ((linha.Elements<Cell>()
                                    .Where(c => c.CellReference.Value == "A" + linha.RowIndex)
                                    .First().Descendants<CellValue>().FirstOrDefault().InnerText.Trim() != ""))
                            {
                                Cell celulaDate = linha.Elements<Cell>()
                                            .Where(c => c.CellReference == "A" + linha.RowIndex).First();
                                int dateInt = Convert.ToInt32(FromExcelTextBollean(celulaDate, 
                                    spreadsheetDocument.WorkbookPart));
                                DateTime date = FromExcelSerialDate(dateInt);

                                int SemanaAno = CultureInfo.InvariantCulture.Calendar
                                    .GetWeekOfYear(date,
                                    CalendarWeekRule.FirstDay, DayOfWeek.Sunday);

                                #region Coluna D - Thomas Calil

                                if (linha.Elements<Cell>()
                                        .Where(c => c.CellReference.Value == "D" + linha.RowIndex)
                                        .First().Descendants<CellValue>().Count() > 0)
                                {
                                    if (linha.Elements<Cell>()
                                            .Where(c => c.CellReference.Value == "D" + linha.RowIndex)
                                            .First().Descendants<CellValue>().FirstOrDefault().InnerText.Trim() != "")
                                    {
                                        Cell celulaVisitor = linhaVisitor.Elements<Cell>()
                                            .Where(c => c.CellReference == "D2").First();
                                        string visitor = FromExcelTextBollean(celulaVisitor, 
                                            spreadsheetDocument.WorkbookPart);

                                        Cell celulaDescription = linha.Elements<Cell>()
                                            .Where(c => c.CellReference == "D" + linha.RowIndex).First();
                                        string description = FromExcelTextBollean(celulaDescription, 
                                            spreadsheetDocument.WorkbookPart);

                                        Nav_Agenda navAgenda = hlbapp.Nav_Agenda
                                            .Where(w => w.VisitDate == date
                                                && w.Visitor == visitor)
                                            .FirstOrDefault();

                                        int insert = 0;
                                        if (navAgenda == null)
                                        {
                                            navAgenda = new Nav_Agenda();
                                            insert = 1;
                                        }

                                        navAgenda.VisitDate = date;
                                        navAgenda.Visitor = visitor;
                                        navAgenda.Description = description;
                                        navAgenda.YearWeek = SemanaAno;

                                        if (insert == 1)
                                            hlbapp.Nav_Agenda.AddObject(navAgenda);
                                    }
                                }

                                #endregion

                                #region Coluna E - Luciano Cosinet

                                if (linha.Elements<Cell>()
                                        .Where(c => c.CellReference.Value == "E" + linha.RowIndex)
                                        .First().Descendants<CellValue>().Count() > 0)
                                {
                                    if (linha.Elements<Cell>()
                                            .Where(c => c.CellReference.Value == "E" + linha.RowIndex)
                                            .First().Descendants<CellValue>().FirstOrDefault().InnerText.Trim() != "")
                                    {
                                        Cell celulaVisitor = linhaVisitor.Elements<Cell>()
                                            .Where(c => c.CellReference == "E2").First();
                                        string visitor = FromExcelTextBollean(celulaVisitor,
                                            spreadsheetDocument.WorkbookPart);

                                        Cell celulaDescription = linha.Elements<Cell>()
                                            .Where(c => c.CellReference == "E" + linha.RowIndex).First();
                                        string description = FromExcelTextBollean(celulaDescription,
                                            spreadsheetDocument.WorkbookPart);

                                        Nav_Agenda navAgenda = hlbapp.Nav_Agenda
                                            .Where(w => w.VisitDate == date
                                                && w.Visitor == visitor)
                                            .FirstOrDefault();

                                        int insert = 0;
                                        if (navAgenda == null)
                                        {
                                            navAgenda = new Nav_Agenda();
                                            insert = 1;
                                        }

                                        navAgenda.VisitDate = date;
                                        navAgenda.Visitor = visitor;
                                        navAgenda.Description = description;
                                        navAgenda.YearWeek = SemanaAno;

                                        if (insert == 1)
                                            hlbapp.Nav_Agenda.AddObject(navAgenda);
                                    }
                                }

                                #endregion

                                #region Coluna F - Matheus Alves

                                if (linha.Elements<Cell>()
                                        .Where(c => c.CellReference.Value == "F" + linha.RowIndex)
                                        .First().Descendants<CellValue>().Count() > 0)
                                {
                                    if (linha.Elements<Cell>()
                                            .Where(c => c.CellReference.Value == "F" + linha.RowIndex)
                                            .First().Descendants<CellValue>().FirstOrDefault().InnerText.Trim() != "")
                                    {
                                        Cell celulaVisitor = linhaVisitor.Elements<Cell>()
                                            .Where(c => c.CellReference == "F2").First();
                                        string visitor = FromExcelTextBollean(celulaVisitor,
                                            spreadsheetDocument.WorkbookPart);

                                        Cell celulaDescription = linha.Elements<Cell>()
                                            .Where(c => c.CellReference == "F" + linha.RowIndex).First();
                                        string description = FromExcelTextBollean(celulaDescription,
                                            spreadsheetDocument.WorkbookPart);

                                        Nav_Agenda navAgenda = hlbapp.Nav_Agenda
                                            .Where(w => w.VisitDate == date
                                                && w.Visitor == visitor)
                                            .FirstOrDefault();

                                        int insert = 0;
                                        if (navAgenda == null)
                                        {
                                            navAgenda = new Nav_Agenda();
                                            insert = 1;
                                        }

                                        navAgenda.VisitDate = date;
                                        navAgenda.Visitor = visitor;
                                        navAgenda.Description = description;
                                        navAgenda.YearWeek = SemanaAno;

                                        if (insert == 1)
                                            hlbapp.Nav_Agenda.AddObject(navAgenda);
                                    }
                                }

                                #endregion                                    

                                #region Coluna G - Paco Medina

                                if (linha.Elements<Cell>()
                                        .Where(c => c.CellReference.Value == "G" + linha.RowIndex)
                                        .First().Descendants<CellValue>().Count() > 0)
                                {
                                    if (linha.Elements<Cell>()
                                            .Where(c => c.CellReference.Value == "G" + linha.RowIndex)
                                            .First().Descendants<CellValue>().FirstOrDefault().InnerText.Trim() != "")
                                    {
                                        Cell celulaVisitor = linhaVisitor.Elements<Cell>()
                                            .Where(c => c.CellReference == "G2").First();
                                        string visitor = FromExcelTextBollean(celulaVisitor,
                                            spreadsheetDocument.WorkbookPart);

                                        Cell celulaDescription = linha.Elements<Cell>()
                                            .Where(c => c.CellReference == "G" + linha.RowIndex).First();
                                        string description = FromExcelTextBollean(celulaDescription,
                                            spreadsheetDocument.WorkbookPart);

                                        Nav_Agenda navAgenda = hlbapp.Nav_Agenda
                                            .Where(w => w.VisitDate == date
                                                && w.Visitor == visitor)
                                            .FirstOrDefault();

                                        int insert = 0;
                                        if (navAgenda == null)
                                        {
                                            navAgenda = new Nav_Agenda();
                                            insert = 1;
                                        }

                                        navAgenda.VisitDate = date;
                                        navAgenda.Visitor = visitor;
                                        navAgenda.Description = description;
                                        navAgenda.YearWeek = SemanaAno;

                                        if (insert == 1)
                                            hlbapp.Nav_Agenda.AddObject(navAgenda);
                                    }
                                }

                                #endregion

                                #region Coluna H - Ronald Trenchi

                                if (linha.Elements<Cell>()
                                        .Where(c => c.CellReference.Value == "H" + linha.RowIndex)
                                        .First().Descendants<CellValue>().Count() > 0)
                                {
                                    if (linha.Elements<Cell>()
                                            .Where(c => c.CellReference.Value == "H" + linha.RowIndex)
                                            .First().Descendants<CellValue>().FirstOrDefault().InnerText.Trim() != "")
                                    {
                                        Cell celulaVisitor = linhaVisitor.Elements<Cell>()
                                            .Where(c => c.CellReference == "H2").First();
                                        string visitor = FromExcelTextBollean(celulaVisitor,
                                            spreadsheetDocument.WorkbookPart);

                                        Cell celulaDescription = linha.Elements<Cell>()
                                            .Where(c => c.CellReference == "H" + linha.RowIndex).First();
                                        string description = FromExcelTextBollean(celulaDescription,
                                            spreadsheetDocument.WorkbookPart);

                                        Nav_Agenda navAgenda = hlbapp.Nav_Agenda
                                            .Where(w => w.VisitDate == date
                                                && w.Visitor == visitor)
                                            .FirstOrDefault();

                                        int insert = 0;
                                        if (navAgenda == null)
                                        {
                                            navAgenda = new Nav_Agenda();
                                            insert = 1;
                                        }

                                        navAgenda.VisitDate = date;
                                        navAgenda.Visitor = visitor;
                                        navAgenda.Description = description;
                                        navAgenda.YearWeek = SemanaAno;

                                        if (insert == 1)
                                            hlbapp.Nav_Agenda.AddObject(navAgenda);
                                    }
                                }

                                #endregion

                                #region Coluna I - Khalil Arar

                                if (linha.Elements<Cell>()
                                        .Where(c => c.CellReference.Value == "I" + linha.RowIndex)
                                        .First().Descendants<CellValue>().Count() > 0)
                                {
                                    if (linha.Elements<Cell>()
                                            .Where(c => c.CellReference.Value == "I" + linha.RowIndex)
                                            .First().Descendants<CellValue>().FirstOrDefault().InnerText.Trim() != "")
                                    {
                                        Cell celulaVisitor = linhaVisitor.Elements<Cell>()
                                            .Where(c => c.CellReference == "I2").First();
                                        string visitor = FromExcelTextBollean(celulaVisitor,
                                            spreadsheetDocument.WorkbookPart);

                                        Cell celulaDescription = linha.Elements<Cell>()
                                            .Where(c => c.CellReference == "I" + linha.RowIndex).First();
                                        string description = FromExcelTextBollean(celulaDescription,
                                            spreadsheetDocument.WorkbookPart);

                                        Nav_Agenda navAgenda = hlbapp.Nav_Agenda
                                            .Where(w => w.VisitDate == date
                                                && w.Visitor == visitor)
                                            .FirstOrDefault();

                                        int insert = 0;
                                        if (navAgenda == null)
                                        {
                                            navAgenda = new Nav_Agenda();
                                            insert = 1;
                                        }

                                        navAgenda.VisitDate = date;
                                        navAgenda.Visitor = visitor;
                                        navAgenda.Description = description;
                                        navAgenda.YearWeek = SemanaAno;

                                        if (insert == 1)
                                            hlbapp.Nav_Agenda.AddObject(navAgenda);
                                    }
                                }

                                #endregion

                                #region (DESATIVADA POR SAÍDA DO LUCIANO COUSINET) Coluna J - Rich

                                //if (linha.Elements<Cell>()
                                //        .Where(c => c.CellReference.Value == "J" + linha.RowIndex)
                                //        .First().Descendants<CellValue>().Count() > 0)
                                //{
                                //    if (linha.Elements<Cell>()
                                //            .Where(c => c.CellReference.Value == "J" + linha.RowIndex)
                                //            .First().Descendants<CellValue>().FirstOrDefault().InnerText.Trim() != "")
                                //    {
                                //        Cell celulaVisitor = linhaVisitor.Elements<Cell>()
                                //            .Where(c => c.CellReference == "J2").First();
                                //        string visitor = FromExcelTextBollean(celulaVisitor,
                                //            spreadsheetDocument.WorkbookPart);

                                //        Cell celulaDescription = linha.Elements<Cell>()
                                //            .Where(c => c.CellReference == "J" + linha.RowIndex).First();
                                //        string description = FromExcelTextBollean(celulaDescription,
                                //            spreadsheetDocument.WorkbookPart);

                                //        Nav_Agenda navAgenda = hlbapp.Nav_Agenda
                                //            .Where(w => w.VisitDate == date
                                //                && w.Visitor == visitor)
                                //            .FirstOrDefault();

                                //        int insert = 0;
                                //        if (navAgenda == null)
                                //        {
                                //            navAgenda = new Nav_Agenda();
                                //            insert = 1;
                                //        }

                                //        navAgenda.VisitDate = date;
                                //        navAgenda.Visitor = visitor;
                                //        navAgenda.Description = description;
                                //        navAgenda.YearWeek = SemanaAno;

                                //        if (insert == 1)
                                //            hlbapp.Nav_Agenda.AddObject(navAgenda);
                                //    }
                                //}

                                #endregion
                            }
                        }
                    }
                }

                arquivo.Close();

                hlbapp.SaveChanges();

                return "";
            }
            catch (Exception e)
            {
                arquivo.Close();
                return "Erro ao realizar a importação: " + e.Message;
            }
        }

        #endregion
    }
}
