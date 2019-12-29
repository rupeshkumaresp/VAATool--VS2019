using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Xml;
using Elmah;
using VAA.BusinessComponents.Interfaces;
using VAA.CommonComponents;
using VAA.DataAccess;
using System.IO;
using System.Web;
using VAA.DataAccess.Model;
using VAA.Entities.VAAEntity;
using System.Net;
using Aspose.Pdf;
using Aspose.Pdf.Text;
using OutputSpreadsheetWriterLibrary;

namespace VAA.BusinessComponents
{

    public class PackingTicketProcessor : IPackingTicket
    {
        readonly CycleManagement _cycleManagement = new CycleManagement();
        readonly MenuManagement _menuManagement = new MenuManagement();
        readonly OrderManagement _orderManagement = new OrderManagement();
        readonly RouteManagement _routeManagement = new RouteManagement();
        readonly BaseItemManagement _baseItemManagement = new BaseItemManagement();
        readonly AccountManagement _accountManagement = new AccountManagement();
        readonly ChiliProcessor chili = new ChiliProcessor();
        public int InstanceId = 1;
        public int PdfGenerationJobId = 1;
        static string pdfInputDestination = "";
        static string pdfOutputDestination = "";
        string EmmaPDFPathFolder = (System.Configuration.ConfigurationManager.AppSettings["EmmaPDFPathFolder"]);

        public void SetAsposeLicense()
        {
            Aspose.Pdf.License license = new Aspose.Pdf.License();
            // Set license
            var asposeLicensePath = ConfigurationManager.AppSettings["AsposeLicensePath"];

            license.SetLicense(asposeLicensePath);
        }

        public Dictionary<string, int> CalculatePrintRun(long orderId)
        {

            Dictionary<string, int> printRunCountDict = new Dictionary<string, int>();

            try
            {
                //get liveOrderId from orderId
                var liveOrderId = _orderManagement.GetLiveOrderIdFromOrderId(orderId);
                var liveOrderDetails = _orderManagement.GetLiveOrderDetails(liveOrderId);

                foreach (var orderDetails in liveOrderDetails)
                {
                    var menu = _menuManagement.GetMenuById(Convert.ToInt64(orderDetails.MenuId));
                    var menuTemplate = _menuManagement.GetMenuTemplate(menu.Id);

                    var fileName = GetPdfFileName(Convert.ToInt64(menu.CycleId), menu.Id, Convert.ToInt32(menuTemplate.TemplateID), Convert.ToInt32(menu.LanguageId));

                    if (!printRunCountDict.ContainsKey(fileName))
                        printRunCountDict.Add(fileName, Convert.ToInt32(orderDetails.Quantity));
                    else
                        printRunCountDict[fileName] += Convert.ToInt32(orderDetails.Quantity);
                }
            }
            catch (Exception ex)
            {
                //write to Elma
                ErrorSignal.FromCurrentContext().Raise(ex);
            }
            return printRunCountDict;
        }

        public Dictionary<string, PrintRunData> CalculatePrintRunData(long orderId)
        {

            Dictionary<string, PrintRunData> printRunBundleCountDict = new Dictionary<string, PrintRunData>();

            try
            {
                //get liveOrderId from orderId
                var liveOrderId = _orderManagement.GetLiveOrderIdFromOrderId(orderId);
                var liveOrderDetails = _orderManagement.GetLiveOrderDetails(liveOrderId);

                var order = _orderManagement.GetOrderById(orderId);
                var startDate = Convert.ToDateTime(order.OrderCycleStartDate);


                foreach (var orderDetails in liveOrderDetails)
                {
                    var menu = _menuManagement.GetMenuById(Convert.ToInt64(orderDetails.MenuId));
                    var menuTemplate = _menuManagement.GetMenuTemplate(menu.Id);

                    var fileName = GetPdfFileName(Convert.ToInt64(menu.CycleId), menu.Id, Convert.ToInt32(menuTemplate.TemplateID), Convert.ToInt32(menu.LanguageId));
                    var menunameParts = menu.MenuName.Split(new char[] { '/' });
                    var classID = _menuManagement.GetMenuClass(Convert.ToInt32(menu.MenuTypeId));

                    if (!printRunBundleCountDict.ContainsKey(fileName))
                    {

                        PrintRunData dataPrintRun = new PrintRunData();
                        dataPrintRun.MenuPDFFile = fileName;
                        dataPrintRun.MenuCount = Convert.ToInt32(orderDetails.Quantity);
                        dataPrintRun.MenuClassId = classID;

                        dataPrintRun.MenuFlight = orderDetails.FlightNo;

                        if ((menu.MenuTypeId == 10 || menu.MenuTypeId == 13))
                            dataPrintRun.isTranslated6pp = menu.LanguageId > 1;
                        else
                            dataPrintRun.isTranslated6pp = false;

                        printRunBundleCountDict.Add(fileName, dataPrintRun);
                    }
                    else
                    {
                        var dataPrun = printRunBundleCountDict[fileName];
                        dataPrun.MenuCount += Convert.ToInt32(orderDetails.Quantity);
                        dataPrun.MenuFlight = dataPrun.MenuFlight + "," + orderDetails.FlightNo;

                    }

                    //bundle calculation
                    var data = printRunBundleCountDict[fileName];
                    var flightNum = orderDetails.FlightNo;
                    var flightScheduleWednesday = _routeManagement.GetScheduleWednesday(flightNum, startDate);
                    var flightScheduleThursday = _routeManagement.GetScheduleThursday(flightNum, startDate.AddDays(1));
                    var flightScheduleFriday = _routeManagement.GetScheduleFriday(flightNum, startDate.AddDays(2));
                    var flightScheduleSaturday = _routeManagement.GetScheduleSaturday(flightNum, startDate.AddDays(3));
                    var flightScheduleSunday = _routeManagement.GetScheduleSunday(flightNum, startDate.AddDays(4));
                    var flightScheduleMonday = _routeManagement.GetScheduleMonday(flightNum, startDate.AddDays(5));
                    var flightScheduleTuesday = _routeManagement.GetScheduleTuesday(flightNum, startDate.AddDays(6));

                    //also see effective and discontinued dates
                    //loop thgourh the flight no records and see the days on which operatre to get the total day count
                    bool isWednesday = flightScheduleWednesday != null;
                    bool isThursday = flightScheduleThursday != null;
                    bool isFriday = flightScheduleFriday != null;
                    bool isSaturday = flightScheduleSaturday != null;
                    bool isSunday = flightScheduleSunday != null;
                    bool isMonday = flightScheduleMonday != null;
                    bool isTuesday = flightScheduleTuesday != null;

                    List<string> EquipmentTypes = new List<string>();

                    if (isWednesday)
                    {
                        if (!EquipmentTypes.Contains(flightScheduleWednesday.EquipmentType))
                            EquipmentTypes.Add(flightScheduleWednesday.EquipmentType);
                    }

                    if (isThursday)
                    {
                        if (!EquipmentTypes.Contains(flightScheduleThursday.EquipmentType))
                            EquipmentTypes.Add(flightScheduleThursday.EquipmentType);
                    }
                    if (isFriday)
                    {
                        if (!EquipmentTypes.Contains(flightScheduleFriday.EquipmentType))
                            EquipmentTypes.Add(flightScheduleFriday.EquipmentType);
                    }
                    if (isSaturday)
                    {
                        if (!EquipmentTypes.Contains(flightScheduleSaturday.EquipmentType))
                            EquipmentTypes.Add(flightScheduleSaturday.EquipmentType);
                    }
                    if (isSunday)
                    {
                        if (!EquipmentTypes.Contains(flightScheduleSunday.EquipmentType))
                            EquipmentTypes.Add(flightScheduleSunday.EquipmentType);
                    }
                    if (isMonday)
                    {
                        if (!EquipmentTypes.Contains(flightScheduleMonday.EquipmentType))
                            EquipmentTypes.Add(flightScheduleMonday.EquipmentType);
                    }
                    if (isTuesday)
                    {
                        if (!EquipmentTypes.Contains(flightScheduleTuesday.EquipmentType))
                            EquipmentTypes.Add(flightScheduleTuesday.EquipmentType);
                    }

                    List<int> EquipmentDays = new List<int>();

                    for (int i = 0; i < EquipmentTypes.Count; i++)
                    {
                        var days = 0;

                        if (isWednesday)
                        {
                            if (flightScheduleWednesday.EquipmentType == EquipmentTypes[i])
                                days++;
                        }

                        if (isThursday)
                        {
                            if (flightScheduleThursday.EquipmentType == EquipmentTypes[i])
                                days++;
                        }
                        if (isFriday)
                        {
                            if (flightScheduleFriday.EquipmentType == EquipmentTypes[i])
                                days++;
                        }
                        if (isSaturday)
                        {
                            if (flightScheduleSaturday.EquipmentType == EquipmentTypes[i])
                                days++;
                        }
                        if (isSunday)
                        {
                            if (flightScheduleSunday.EquipmentType == EquipmentTypes[i])
                                days++;
                        }
                        if (isMonday)
                        {
                            if (flightScheduleMonday.EquipmentType == EquipmentTypes[i])
                                days++;
                        }
                        if (isTuesday)
                        {
                            if (flightScheduleTuesday.EquipmentType == EquipmentTypes[i])
                                days++;
                        }
                        EquipmentDays.Add(days);

                    }
                    List<int> EquipmentQuantity = new List<int>();

                    for (int i = 0; i < EquipmentTypes.Count; i++)
                    {
                        int capacity = 0;
                        if (classID == 1)
                            capacity = _routeManagement.UpperClassCapacity(EquipmentTypes[i]);
                        if (classID == 2)
                            capacity = _routeManagement.PremiumEcoClassCapacity(EquipmentTypes[i]);
                        if (classID == 3)
                            capacity = _routeManagement.EcoClassCapacity(EquipmentTypes[i]);

                        EquipmentQuantity.Add(capacity);
                    }


                    for (int i = 0; i < EquipmentTypes.Count; i++)
                    {
                        if (EquipmentDays[i] != 0 && EquipmentQuantity[i] != 0)
                            data.Bundles = data.Bundles + Convert.ToString(EquipmentDays[i]) + "of " + Convert.ToString(EquipmentQuantity[i]) + ",";
                    }

                }

            }
            catch (Exception ex)
            {
                //write to Elma
                ErrorSignal.FromCurrentContext().Raise(ex);
            }
            return printRunBundleCountDict;
        }

        public string GeneratePdfFileNameForDownload(long cycleId, int classId, int menuTypeId, long routeId)
        {
            try
            {
                var directory = Directory.GetParent(HttpRuntime.AppDomainAppPath);
                var parentDir = directory.Parent;

                pdfInputDestination = EmmaPDFPathFolder + @"\MENU PDFS\";
                pdfOutputDestination = EmmaPDFPathFolder + @"\MENU PDFS\Output";


                if (menuTypeId > 0)
                {
                    var menus = _menuManagement.GetMenuByCycleClassAndMenutype(cycleId, classId, menuTypeId).ToList();

                    if (routeId > 0)
                    {
                        //find which menu is for this route
                        foreach (var menu in menus)
                        {

                            var menuForRoute = _menuManagement.GetRoutesByMenu(menu.Id);

                            bool isRoute = false;

                            foreach (var route in menuForRoute)
                            {
                                if (route.RouteId == routeId)
                                {
                                    isRoute = true;
                                    break;
                                }
                            }
                            if (isRoute)
                            {
                                try
                                {
                                    var menuTemplate = _menuManagement.GetMenuTemplate(menu.Id);
                                    return GetPdfFileName(cycleId, menu.Id, Convert.ToInt32(menuTemplate.TemplateID), Convert.ToInt32(menu.LanguageId));
                                }
                                catch
                                {
                                    //log the error
                                }
                            }


                        }

                    }

                }


            }
            catch
            {
                //failed
            }
            return "";

        }

        public string GetPdfFileName(long cycleId, long menuId, int templateId, int languageId)
        {
            var menu = _menuManagement.GetMenuById(menuId);

            var template = _menuManagement.GetTemplate(templateId);

            var languageName = _menuManagement.GetLanguage(languageId);

            var cycle = _cycleManagement.GetCycle(cycleId);

            var outputFileName = cycle.CycleName + "-" + menu.MenuCode + "-" + template.Name + "-" + languageName + ".pdf";

            return outputFileName;

        }

        public void CalculateBoxTicketData(long orderId)
        {
            try
            {
                //delete the boxticket data for this orderId
                _orderManagement.DeleteBoxTicketData(orderId);

                var liveOrderId = _orderManagement.GetLiveOrderIdFromOrderId(orderId);

                var liveOrderDetails = _orderManagement.GetLiveOrderDetails(liveOrderId);

                var order = _orderManagement.GetOrderById(orderId);

                var startDate = Convert.ToDateTime(order.OrderCycleStartDate);
                var endDate = Convert.ToDateTime(order.OrderCycleEndDate);

                List<tBoxTicketData> tempBoxData = new List<tBoxTicketData>();


                //Prepare data for all different shipping ports
                for (int i = 1; i <= 3; i++)
                {
                    var classId = i;

                    PackingTicketClassWise(orderId, liveOrderDetails, startDate, classId, "EDI", tempBoxData);
                    PackingTicketClassWise(orderId, liveOrderDetails, startDate, classId, "GLA", tempBoxData);
                    PackingTicketClassWise(orderId, liveOrderDetails, startDate, classId, "LGW", tempBoxData);
                    PackingTicketClassWise(orderId, liveOrderDetails, startDate, classId, "LHR", tempBoxData);
                    PackingTicketClassWise(orderId, liveOrderDetails, startDate, classId, "MAN", tempBoxData);
                }


                //Sort data base on flight no
                for (int i = 1; i <= 3; i++)
                {
                    var classId = i;

                    List<tBoxTicketData> tempEDI = new List<tBoxTicketData>();
                    List<tBoxTicketData> tempGLA = new List<tBoxTicketData>();
                    List<tBoxTicketData> tempLGW = new List<tBoxTicketData>();
                    List<tBoxTicketData> tempLHR = new List<tBoxTicketData>();
                    List<tBoxTicketData> tempMAN = new List<tBoxTicketData>();

                    for (int j = 0; j < tempBoxData.Count; j++)
                    {
                        //get all the data for EDI and sorted on flight no

                        if (tempBoxData[j].ShipTo == "EDI" && tempBoxData[j].ClassId == classId)
                            tempEDI.Add(tempBoxData[j]);

                        //get all the data for GLA and sorted on flight no
                        if (tempBoxData[j].ShipTo == "GLA" && tempBoxData[j].ClassId == classId)
                            tempGLA.Add(tempBoxData[j]);

                        //get all the data for LGW and sorted on flight no
                        if (tempBoxData[j].ShipTo == "LGW" && tempBoxData[j].ClassId == classId)
                            tempLGW.Add(tempBoxData[j]);

                        //get all the data for LHR and sorted on flight no
                        if (tempBoxData[j].ShipTo == "LHR" && tempBoxData[j].ClassId == classId)
                            tempLHR.Add(tempBoxData[j]);

                        //get all the data for MAN and sorted on flight no
                        if (tempBoxData[j].ShipTo == "MAN" && tempBoxData[j].ClassId == classId)
                            tempMAN.Add(tempBoxData[j]);

                    }

                    var ediSorted = tempEDI.OrderBy(e => e.FlightNo).ToList();
                    var glaSorted = tempGLA.OrderBy(e => e.FlightNo).ToList();
                    var lgwSorted = tempLGW.OrderBy(e => e.FlightNo).ToList();
                    var lhrSorted = tempLHR.OrderBy(e => e.FlightNo).ToList();
                    var manSorted = tempMAN.OrderBy(e => e.FlightNo).ToList();

                    for (int edi = 0; edi < ediSorted.Count; edi++)
                    {
                        _orderManagement.AddBoxTicketData(ediSorted[edi]);
                    }

                    for (int gla = 0; gla < glaSorted.Count; gla++)
                    {
                        _orderManagement.AddBoxTicketData(glaSorted[gla]);
                    }

                    for (int lgw = 0; lgw < lgwSorted.Count; lgw++)
                    {
                        _orderManagement.AddBoxTicketData(lgwSorted[lgw]);
                    }

                    for (int lhr = 0; lhr < lhrSorted.Count; lhr++)
                    {
                        _orderManagement.AddBoxTicketData(lhrSorted[lhr]);
                    }

                    for (int man = 0; man < manSorted.Count; man++)
                    {
                        _orderManagement.AddBoxTicketData(manSorted[man]);
                    }
                }

            }
            catch (Exception ex)
            {
                //write to Elma
                ErrorSignal.FromCurrentContext().Raise(ex);
            }
        }

        private void PackingTicketClassWise(long orderId, List<tLiveOrderDetails> liveOrderDetails, DateTime startDate, int requestedClassId, string requestedShipto, List<tBoxTicketData> tempBoxData)
        {
            foreach (var orderDetails in liveOrderDetails)
            {
                //for each flight number count the 
                var menu = _menuManagement.GetMenuById(Convert.ToInt64(orderDetails.MenuId));

                //only main menu we need to process
                if (menu.MenuTypeId != 1 && menu.MenuTypeId != 10 && menu.MenuTypeId != 13)
                    continue;

                var classID = _menuManagement.GetMenuClass(Convert.ToInt32(menu.MenuTypeId));

                if (classID != requestedClassId)
                    continue;

                var className = "J";

                if (classID == 1)
                    className = "J";

                if (classID == 2)
                    className = "W";

                if (classID == 3)
                    className = "Y";

                var flightNum = orderDetails.FlightNo;
                var route = _routeManagement.GetRouteById(Convert.ToInt64(orderDetails.RouteId));
                var routename = route.DepartureAirportCode + "-" + route.ArrivalAirportCode;


                if (!flightNum.Contains("VS"))
                    flightNum = flightNum.Replace("V", "VS");

                var flightScheduleWednesday = _routeManagement.GetScheduleWednesday(flightNum, startDate);
                var flightScheduleThursday = _routeManagement.GetScheduleThursday(flightNum, startDate.AddDays(1));
                var flightScheduleFriday = _routeManagement.GetScheduleFriday(flightNum, startDate.AddDays(2));
                var flightScheduleSaturday = _routeManagement.GetScheduleSaturday(flightNum, startDate.AddDays(3));
                var flightScheduleSunday = _routeManagement.GetScheduleSunday(flightNum, startDate.AddDays(4));
                var flightScheduleMonday = _routeManagement.GetScheduleMonday(flightNum, startDate.AddDays(5));
                var flightScheduleTuesday = _routeManagement.GetScheduleTuesday(flightNum, startDate.AddDays(6));

                //also see effective and discontinued dates
                //loop thgourh the flight no records and see the days on which operatre to get the total day count
                bool isWednesday = flightScheduleWednesday != null;
                bool isThursday = flightScheduleThursday != null;
                bool isFriday = flightScheduleFriday != null;
                bool isSaturday = flightScheduleSaturday != null;
                bool isSunday = flightScheduleSunday != null;
                bool isMonday = flightScheduleMonday != null;
                bool isTuesday = flightScheduleTuesday != null;


                int capacity = 0;

                //look for tseatconfiguration for that equipment type

                var ShipTo = "LHR";

                if (routename.Contains("LGW"))
                    ShipTo = "LGW";

                if (routename.Contains("MAN"))
                    ShipTo = "MAN";

                if (routename.Contains("GLA"))
                    ShipTo = "GLA";

                if (routename.Contains("EDI"))
                    ShipTo = "EDI";

                if (routename.Contains("LHR"))
                    ShipTo = "LHR";

                if (ShipTo != requestedShipto)
                    continue;

                if (isWednesday)
                {
                    var date = startDate;
                    var departureTime = flightScheduleWednesday.DepartureTime;

                    if (classID == 1)
                        capacity = _routeManagement.UpperClassCapacity(flightScheduleWednesday.EquipmentType);
                    if (classID == 2)
                        capacity = _routeManagement.PremiumEcoClassCapacity(flightScheduleWednesday.EquipmentType);
                    if (classID == 3)
                        capacity = _routeManagement.EcoClassCapacity(flightScheduleWednesday.EquipmentType);

                    var count = capacity;

                    //create tboxticketdata object and save

                    var boxTicketData = new tBoxTicketData();
                    boxTicketData.FlightNo = flightNum;

                    boxTicketData.OrderId = orderId;
                    boxTicketData.Route = routename;

                    FlightNumberFix(flightNum, date, boxTicketData);

                    boxTicketData.Time = departureTime;
                    boxTicketData.Count = capacity;
                    boxTicketData.ShipTo = ShipTo;
                    boxTicketData.ClassId = classID;
                    boxTicketData.ClassName = className;
                    boxTicketData.Bound = flightScheduleWednesday.Bound;
                    boxTicketData.MenuCode = menu.MenuCode;
                    boxTicketData.RouteId = route.RouteId;

                    var brkMenu = string.Empty;
                    var teaMenu = string.Empty;

                    try
                    {
                        if (menu.MenuTypeId == 1)
                            brkMenu = _menuManagement.GetBreakfastMenuCodeFromMainMenuCode(menu.MenuCode, route.RouteId, flightNum, Convert.ToInt64(orderDetails.LiveOrderId));
                    }
                    catch { }

                    boxTicketData.BRKMenuCode = brkMenu;


                    try
                    {
                        if (menu.MenuTypeId == 1)
                            teaMenu = _menuManagement.GetTeaMenuCodeFromMainMenuCode(menu.MenuCode, route.RouteId, flightNum, Convert.ToInt64(orderDetails.LiveOrderId));
                    }
                    catch { }

                    boxTicketData.TEAMenuCode = teaMenu;

                    //_orderManagement.AddBoxTicketData(boxTicketData);
                    tempBoxData.Add(boxTicketData);
                }

                if (isThursday)
                {
                    var date = startDate.AddDays(1);
                    var departureTime = flightScheduleThursday.DepartureTime;

                    if (classID == 1)
                        capacity = _routeManagement.UpperClassCapacity(flightScheduleThursday.EquipmentType);
                    if (classID == 2)
                        capacity = _routeManagement.PremiumEcoClassCapacity(flightScheduleThursday.EquipmentType);
                    if (classID == 3)
                        capacity = _routeManagement.EcoClassCapacity(flightScheduleThursday.EquipmentType);

                    var count = capacity;

                    //create tboxticketdata object and save
                    var boxTicketData = new tBoxTicketData();
                    boxTicketData.FlightNo = flightNum;
                    boxTicketData.OrderId = orderId;
                    boxTicketData.Route = routename;

                    FlightNumberFix(flightNum, date, boxTicketData);

                    boxTicketData.Time = departureTime;
                    boxTicketData.Count = capacity;
                    boxTicketData.ShipTo = ShipTo;
                    boxTicketData.ClassId = classID;
                    boxTicketData.ClassName = className;
                    boxTicketData.Bound = flightScheduleThursday.Bound;
                    boxTicketData.MenuCode = menu.MenuCode;
                    boxTicketData.RouteId = route.RouteId;
                    var brkMenu = string.Empty;
                    var teaMenu = string.Empty;

                    try
                    {
                        if (menu.MenuTypeId == 1)
                            brkMenu = _menuManagement.GetBreakfastMenuCodeFromMainMenuCode(menu.MenuCode, route.RouteId, flightNum, Convert.ToInt64(orderDetails.LiveOrderId));
                    }
                    catch { }

                    boxTicketData.BRKMenuCode = brkMenu;


                    try
                    {
                        if (menu.MenuTypeId == 1)
                            teaMenu = _menuManagement.GetTeaMenuCodeFromMainMenuCode(menu.MenuCode, route.RouteId, flightNum, Convert.ToInt64(orderDetails.LiveOrderId));
                    }
                    catch { }

                    boxTicketData.TEAMenuCode = teaMenu;
                    //_orderManagement.AddBoxTicketData(boxTicketData);
                    tempBoxData.Add(boxTicketData);
                }

                if (isFriday)
                {
                    var date = startDate.AddDays(2);
                    var departureTime = flightScheduleFriday.DepartureTime;

                    if (classID == 1)
                        capacity = _routeManagement.UpperClassCapacity(flightScheduleFriday.EquipmentType);
                    if (classID == 2)
                        capacity = _routeManagement.PremiumEcoClassCapacity(flightScheduleFriday.EquipmentType);
                    if (classID == 3)
                        capacity = _routeManagement.EcoClassCapacity(flightScheduleFriday.EquipmentType);

                    var count = capacity;

                    //create tboxticketdata object and save
                    var boxTicketData = new tBoxTicketData();
                    boxTicketData.FlightNo = flightNum;
                    boxTicketData.OrderId = orderId;
                    boxTicketData.Route = routename;

                    FlightNumberFix(flightNum, date, boxTicketData);

                    boxTicketData.Time = departureTime;
                    boxTicketData.Count = capacity;
                    boxTicketData.ShipTo = ShipTo;
                    boxTicketData.ClassId = classID;
                    boxTicketData.ClassName = className;
                    boxTicketData.Bound = flightScheduleFriday.Bound;
                    boxTicketData.MenuCode = menu.MenuCode;
                    boxTicketData.RouteId = route.RouteId;
                    var brkMenu = string.Empty;
                    var teaMenu = string.Empty;

                    try
                    {
                        if (menu.MenuTypeId == 1)
                            brkMenu = _menuManagement.GetBreakfastMenuCodeFromMainMenuCode(menu.MenuCode, route.RouteId, flightNum, Convert.ToInt64(orderDetails.LiveOrderId));
                    }
                    catch { }

                    boxTicketData.BRKMenuCode = brkMenu;


                    try
                    {
                        if (menu.MenuTypeId == 1)
                            teaMenu = _menuManagement.GetTeaMenuCodeFromMainMenuCode(menu.MenuCode, route.RouteId, flightNum, Convert.ToInt64(orderDetails.LiveOrderId));
                    }
                    catch { }

                    boxTicketData.TEAMenuCode = teaMenu;
                    //_orderManagement.AddBoxTicketData(boxTicketData);
                    tempBoxData.Add(boxTicketData);
                }
                if (isSaturday)
                {
                    var date = startDate.AddDays(3);
                    var departureTime = flightScheduleSaturday.DepartureTime;

                    if (classID == 1)
                        capacity = _routeManagement.UpperClassCapacity(flightScheduleSaturday.EquipmentType);
                    if (classID == 2)
                        capacity = _routeManagement.PremiumEcoClassCapacity(flightScheduleSaturday.EquipmentType);
                    if (classID == 3)
                        capacity = _routeManagement.EcoClassCapacity(flightScheduleSaturday.EquipmentType);

                    var count = capacity;

                    //create tboxticketdata object and save
                    var boxTicketData = new tBoxTicketData();
                    boxTicketData.FlightNo = flightNum;
                    boxTicketData.OrderId = orderId;
                    boxTicketData.Route = routename;

                    FlightNumberFix(flightNum, date, boxTicketData);

                    boxTicketData.Time = departureTime;
                    boxTicketData.Count = capacity;
                    boxTicketData.ShipTo = ShipTo;
                    boxTicketData.ClassId = classID;
                    boxTicketData.ClassName = className;
                    boxTicketData.Bound = flightScheduleSaturday.Bound;
                    boxTicketData.MenuCode = menu.MenuCode;
                    boxTicketData.RouteId = route.RouteId;
                    var brkMenu = string.Empty;
                    var teaMenu = string.Empty;

                    try
                    {
                        if (menu.MenuTypeId == 1)
                            brkMenu = _menuManagement.GetBreakfastMenuCodeFromMainMenuCode(menu.MenuCode, route.RouteId, flightNum, Convert.ToInt64(orderDetails.LiveOrderId));
                    }
                    catch { }

                    boxTicketData.BRKMenuCode = brkMenu;


                    try
                    {
                        if (menu.MenuTypeId == 1)
                            teaMenu = _menuManagement.GetTeaMenuCodeFromMainMenuCode(menu.MenuCode, route.RouteId, flightNum, Convert.ToInt64(orderDetails.LiveOrderId));
                    }
                    catch { }

                    boxTicketData.TEAMenuCode = teaMenu;
                    //_orderManagement.AddBoxTicketData(boxTicketData);
                    tempBoxData.Add(boxTicketData);
                }
                if (isSunday)
                {
                    var date = startDate.AddDays(4);
                    var departureTime = flightScheduleSunday.DepartureTime;

                    if (classID == 1)
                        capacity = _routeManagement.UpperClassCapacity(flightScheduleSunday.EquipmentType);
                    if (classID == 2)
                        capacity = _routeManagement.PremiumEcoClassCapacity(flightScheduleSunday.EquipmentType);
                    if (classID == 3)
                        capacity = _routeManagement.EcoClassCapacity(flightScheduleSunday.EquipmentType);

                    var count = capacity;

                    //create tboxticketdata object and save
                    var boxTicketData = new tBoxTicketData();
                    boxTicketData.FlightNo = flightNum;
                    boxTicketData.OrderId = orderId;
                    boxTicketData.Route = routename;

                    FlightNumberFix(flightNum, date, boxTicketData);

                    boxTicketData.Time = departureTime;
                    boxTicketData.Count = capacity;
                    boxTicketData.ShipTo = ShipTo;
                    boxTicketData.ClassId = classID;
                    boxTicketData.ClassName = className;
                    boxTicketData.Bound = flightScheduleSunday.Bound;
                    boxTicketData.MenuCode = menu.MenuCode;
                    boxTicketData.RouteId = route.RouteId;
                    var brkMenu = string.Empty;
                    var teaMenu = string.Empty;

                    try
                    {
                        if (menu.MenuTypeId == 1)
                            brkMenu = _menuManagement.GetBreakfastMenuCodeFromMainMenuCode(menu.MenuCode, route.RouteId, flightNum, Convert.ToInt64(orderDetails.LiveOrderId));
                    }
                    catch { }

                    boxTicketData.BRKMenuCode = brkMenu;


                    try
                    {
                        if (menu.MenuTypeId == 1)
                            teaMenu = _menuManagement.GetTeaMenuCodeFromMainMenuCode(menu.MenuCode, route.RouteId, flightNum, Convert.ToInt64(orderDetails.LiveOrderId));
                    }
                    catch { }

                    boxTicketData.TEAMenuCode = teaMenu;
                    //_orderManagement.AddBoxTicketData(boxTicketData);
                    tempBoxData.Add(boxTicketData);
                }
                if (isMonday)
                {
                    var date = startDate.AddDays(5);
                    var departureTime = flightScheduleMonday.DepartureTime;

                    if (classID == 1)
                        capacity = _routeManagement.UpperClassCapacity(flightScheduleMonday.EquipmentType);
                    if (classID == 2)
                        capacity = _routeManagement.PremiumEcoClassCapacity(flightScheduleMonday.EquipmentType);
                    if (classID == 3)
                        capacity = _routeManagement.EcoClassCapacity(flightScheduleMonday.EquipmentType);

                    var count = capacity;

                    //create tboxticketdata object and save
                    var boxTicketData = new tBoxTicketData();
                    boxTicketData.FlightNo = flightNum;
                    boxTicketData.OrderId = orderId;
                    boxTicketData.Route = routename;

                    FlightNumberFix(flightNum, date, boxTicketData);

                    boxTicketData.Time = departureTime;
                    boxTicketData.Count = capacity;
                    boxTicketData.ShipTo = ShipTo;
                    boxTicketData.ClassId = classID;
                    boxTicketData.ClassName = className;
                    boxTicketData.Bound = flightScheduleMonday.Bound;
                    boxTicketData.MenuCode = menu.MenuCode;
                    boxTicketData.RouteId = route.RouteId;
                    var brkMenu = string.Empty;
                    var teaMenu = string.Empty;

                    try
                    {
                        if (menu.MenuTypeId == 1)
                            brkMenu = _menuManagement.GetBreakfastMenuCodeFromMainMenuCode(menu.MenuCode, route.RouteId, flightNum, Convert.ToInt64(orderDetails.LiveOrderId));
                    }
                    catch { }

                    boxTicketData.BRKMenuCode = brkMenu;


                    try
                    {
                        if (menu.MenuTypeId == 1)
                            teaMenu = _menuManagement.GetTeaMenuCodeFromMainMenuCode(menu.MenuCode, route.RouteId, flightNum, Convert.ToInt64(orderDetails.LiveOrderId));
                    }
                    catch { }

                    boxTicketData.TEAMenuCode = teaMenu;
                    //_orderManagement.AddBoxTicketData(boxTicketData);
                    tempBoxData.Add(boxTicketData);
                }
                if (isTuesday)
                {
                    var date = startDate.AddDays(6);
                    var departureTime = flightScheduleTuesday.DepartureTime;
                    if (classID == 1)
                        capacity = _routeManagement.UpperClassCapacity(flightScheduleTuesday.EquipmentType);
                    if (classID == 2)
                        capacity = _routeManagement.PremiumEcoClassCapacity(flightScheduleTuesday.EquipmentType);
                    if (classID == 3)
                        capacity = _routeManagement.EcoClassCapacity(flightScheduleTuesday.EquipmentType);

                    var count = capacity;

                    //create tboxticketdata object and save
                    var boxTicketData = new tBoxTicketData();
                    boxTicketData.FlightNo = flightNum;
                    boxTicketData.OrderId = orderId;
                    boxTicketData.Route = routename;

                    FlightNumberFix(flightNum, date, boxTicketData);

                    boxTicketData.Time = departureTime;
                    boxTicketData.Count = capacity;
                    boxTicketData.ShipTo = ShipTo;
                    boxTicketData.ClassId = classID;
                    boxTicketData.ClassName = className;
                    boxTicketData.Bound = flightScheduleTuesday.Bound;
                    boxTicketData.MenuCode = menu.MenuCode;
                    boxTicketData.RouteId = route.RouteId;
                    var brkMenu = string.Empty;
                    var teaMenu = string.Empty;

                    try
                    {
                        if (menu.MenuTypeId == 1)
                            brkMenu = _menuManagement.GetBreakfastMenuCodeFromMainMenuCode(menu.MenuCode, route.RouteId, flightNum, Convert.ToInt64(orderDetails.LiveOrderId));
                    }
                    catch { }

                    boxTicketData.BRKMenuCode = brkMenu;


                    try
                    {
                        if (menu.MenuTypeId == 1)
                            teaMenu = _menuManagement.GetTeaMenuCodeFromMainMenuCode(menu.MenuCode, route.RouteId, flightNum, Convert.ToInt64(orderDetails.LiveOrderId));
                    }
                    catch { }

                    boxTicketData.TEAMenuCode = teaMenu;
                    //_orderManagement.AddBoxTicketData(boxTicketData);
                    tempBoxData.Add(boxTicketData);
                }

            }
        }

        private static void FlightNumberFix(string flightNum, DateTime date, tBoxTicketData boxTicketData)
        {
            if (flightNum == "VS251" || flightNum == "VS026" || flightNum == "VS207" || flightNum == "VS301" || flightNum == "VS401" || flightNum == "VS412" || flightNum == "VS450")
            {
                boxTicketData.Date = date.AddDays(1);
            }
            else
                boxTicketData.Date = date;
        }

        private static bool ShipToPresent(List<tBoxTicketData> boxTicketData, string ShipTo)
        {
            bool found = false;

            foreach (var boxdata in boxTicketData)
            {
                if (boxdata.ShipTo == ShipTo)
                {
                    found = true;
                    break;
                }
            }

            return found;
        }

        public void CreateBoxTicketProofsAndTicketPDF(long orderId)
        {
            var sortedBoxTicketData = CreateBoxTicketProofs(orderId);
            CreateBoxTicketPDfs(orderId, sortedBoxTicketData);
        }

        public List<BoxTicketSortedData> CreateBoxTicketProofs(long orderId)
        {
            try
            {
                SetAsposeLicense();

                //first calculate the box ticket data in case if it is not calculated earlier

                var liveOrderId = _orderManagement.GetLiveOrderIdFromOrderId(orderId);

                //no need to calculate box ticket data as this is already calculated as part of box ticket
                //CalculateBoxTicketData(orderId);

                var boxTicketData = _orderManagement.GetBoxTicketData(orderId);

                if (boxTicketData.Count == 0)
                    return null;

                List<BoxTicketSortedData> SortedBoxTicketDataList = new List<BoxTicketSortedData>();

                //modify the boxticket data in the order we want before proof and pdf processing

                SetBoxTicketSortCriteria(orderId, boxTicketData, SortedBoxTicketDataList);

               // return SortedBoxTicketDataList;

                foreach (var boxTicket in SortedBoxTicketDataList)
                {

                    //create template
                    var boxTicketTempalte = _orderManagement.GetBoxTicketTemplate(boxTicket.ID);
                    if (boxTicketTempalte == null)
                        boxTicketTempalte = _orderManagement.CreateBoxTicketTemplate(boxTicket.ID, boxTicket.Bound);

                    ////apply chili variable - create all the chili variable collection and apply to chili document on the fly
                    Dictionary<string, string> values = new Dictionary<string, string>();

                    //Menu Class	Ship To	Flt No	Route	Dep. Time	Date	Menu Count	Bound
                    values.Add("MenuClass", boxTicket.ClassName);
                    values.Add("ShipTo", boxTicket.ShipTo);
                    values.Add("Route", boxTicket.Route);
                    values.Add("DepTime", boxTicket.Time);
                    values.Add("Date", Convert.ToDateTime(boxTicket.Date).ToString("dd-MMM-yyyy"));
                    values.Add("Bound", boxTicket.Bound);
                    values.Add("MenuCount", Convert.ToString(boxTicket.Count));
                    values.Add("LotNumber", _orderManagement.GetLotNoFromOrderId(orderId));

                    if (boxTicket.Bound.Contains("Outbound"))
                    {
                        values.Add("LoadingFlight", boxTicket.FlightNo);
                    }
                    else
                    {
                        //loading flight is flightno -1
                        var loadingFlight = boxTicket.FlightNo;
                        loadingFlight = loadingFlight.Replace("VS", "");
                        int loadingFlightNo = 0;

                        try
                        {
                            loadingFlightNo = Convert.ToInt32(loadingFlight);
                            loadingFlightNo = loadingFlightNo - 1;
                            var newLoadingFlight = loadingFlightNo.ToString().PadLeft(3, '0');

                            newLoadingFlight = "VS" + newLoadingFlight;
                            values.Add("LoadingFlight", newLoadingFlight);

                        }
                        catch { }
                    }

                    values.Add("FlightNumber", boxTicket.FlightNo);

                    //also add the menu code for various menus

                    if (boxTicket.ClassName.ToLower() == "j")
                    {
                        values.Add("J_MainMenuCode", boxTicket.MenuCode);
                        values.Add("J_BRKMenuCode", _menuManagement.GetBreakfastMenuCodeFromMainMenuCode(boxTicket.MenuCode, boxTicket.RouteId, boxTicket.FlightNo, liveOrderId));
                        values.Add("J_TEAMenuCode", _menuManagement.GetTeaMenuCodeFromMainMenuCode(boxTicket.MenuCode, boxTicket.RouteId, boxTicket.FlightNo, liveOrderId));

                        //wine - menu code
                        //values.Add("J_WineCard", _menuManagement.GetWineCardMenuCodeFromMainMenuCode(boxTicket.MenuCode, boxTicket.RouteId, boxTicket.FlightNo));
                        //values.Add("J_WineCard", "WINE CARD");

                    }
                    if (boxTicket.ClassName.ToLower() == "w")
                        values.Add("W_ClassMenuCode", boxTicket.MenuCode);

                    if (boxTicket.ClassName.ToLower() == "y")
                        values.Add("Y_ClassMenuCode", boxTicket.MenuCode);

                    //chili.UpdateChiliDocumentVariablesBoxTickets(InstanceId, boxTicket.ID, values);


                    string EmmaPDFPathFolder = (System.Configuration.ConfigurationManager.AppSettings["EmmaPDFPathFolder"]);

                    pdfInputDestination = EmmaPDFPathFolder + @"\BoxTickets\";

                    var input = "";

                    if (boxTicket.Bound.Contains("Outbound"))
                    {
                        input = EmmaPDFPathFolder + @"\BoxTickets\BaseTicket\Outbound.PDF";
                        ProcessOutBoundPackingTicket(boxTicket, input, values);
                    }
                    else
                    {
                        input = EmmaPDFPathFolder + @"\BoxTickets\BaseTicket\Inbound.PDF";
                        ProcessInBoundPackingTicket(boxTicket, input, values);
                    }

                 
                }

                return SortedBoxTicketDataList;
            }
            catch (Exception e)
            {
                //write to Elma
                ErrorSignal.FromCurrentContext().Raise(e);
                return new List<BoxTicketSortedData>();
            }

        }

        private static void ProcessInBoundPackingTicket(BoxTicketSortedData boxTicket, string input, Dictionary<string, string> values)
        {
            var result = pdfInputDestination + boxTicket.ID + ".pdf";

            var shortName = Path.GetFileNameWithoutExtension(result);
            var dir = Path.GetDirectoryName(result);

            // Open document
            Aspose.Pdf.Document pdfDocument = new Aspose.Pdf.Document(input);

            // Create TextAbsorber object to find all instances of the input search phrase
            TextFragmentAbsorber textFragmentAbsorber = new TextFragmentAbsorber("VAA MENU J CLASS");

            // Accept the absorber for all the pages
            pdfDocument.Pages.Accept(textFragmentAbsorber);

            // Get the extracted text fragments
            TextFragmentCollection textFragmentCollection = textFragmentAbsorber.TextFragments;

            // Loop through the fragments
            foreach (TextFragment textFragment in textFragmentCollection)
            {
                // Update text and other properties
                textFragment.Text = "VAA MENU " + boxTicket.ClassName.ToUpper() + " CLASS";

                //  textFragment.TextState.Font = FontRepository.FindFont("Helvetica-Bold");
                // textFragment.TextState.FontSize = 12;
            }


            // Create TextAbsorber object to find all instances of the input search phrase
            textFragmentAbsorber = new TextFragmentAbsorber("VS109");

            // Accept the absorber for all the pages
            pdfDocument.Pages.Accept(textFragmentAbsorber);

            // Get the extracted text fragments
            textFragmentCollection = textFragmentAbsorber.TextFragments;

            // Loop through the fragments
            foreach (TextFragment textFragment in textFragmentCollection)
            {
                // Update text and other properties
                textFragment.Text = values["LoadingFlight"];

                //textFragment.TextState.Font = FontRepository.FindFont("Helvetica-Bold");
                //textFragment.TextState.FontSize = 12;
            }

            // Create TextAbsorber object to find all instances of the input search phrase
            textFragmentAbsorber = new TextFragmentAbsorber("VS110");

            // Accept the absorber for all the pages
            pdfDocument.Pages.Accept(textFragmentAbsorber);

            // Get the extracted text fragments
            textFragmentCollection = textFragmentAbsorber.TextFragments;

            // Loop through the fragments
            foreach (TextFragment textFragment in textFragmentCollection)
            {
                // Update text and other properties
                textFragment.Text = values["FlightNumber"];

                //textFragment.TextState.Font = FontRepository.FindFont("Helvetica-Bold");
                //textFragment.TextState.FontSize = 12;
            }


            // Create TextAbsorber object to find all instances of the input search phrase
            textFragmentAbsorber = new TextFragmentAbsorber("20/01/2020");

            // Accept the absorber for all the pages
            pdfDocument.Pages.Accept(textFragmentAbsorber);

            // Get the extracted text fragments
            textFragmentCollection = textFragmentAbsorber.TextFragments;

            // Loop through the fragments
            foreach (TextFragment textFragment in textFragmentCollection)
            {
                // Update text and other properties
                textFragment.Text = values["Date"];

                textFragment.TextState.Font = FontRepository.FindFont("Helvetica-Bold");
                ////textFragment.TextState.FontSize = 12;
            }

            // Create TextAbsorber object to find all instances of the input search phrase
            textFragmentAbsorber = new TextFragmentAbsorber("ATL-MAN");

            // Accept the absorber for all the pages
            pdfDocument.Pages.Accept(textFragmentAbsorber);

            // Get the extracted text fragments
            textFragmentCollection = textFragmentAbsorber.TextFragments;

            // Loop through the fragments
            foreach (TextFragment textFragment in textFragmentCollection)
            {
                // Update text and other properties
                textFragment.Text = values["Route"];
            }


            // Create TextAbsorber object to find all instances of the input search phrase
            textFragmentAbsorber = new TextFragmentAbsorber("356L993");

            // Accept the absorber for all the pages
            pdfDocument.Pages.Accept(textFragmentAbsorber);

            // Get the extracted text fragments
            textFragmentCollection = textFragmentAbsorber.TextFragments;

            // Loop through the fragments
            foreach (TextFragment textFragment in textFragmentCollection)
            {
                // Update text and other properties
                textFragment.Text = values["LotNumber"];
            }


            // Create TextAbsorber object to find all instances of the input search phrase
            textFragmentAbsorber = new TextFragmentAbsorber("IN J MENU BOX");

            // Accept the absorber for all the pages
            pdfDocument.Pages.Accept(textFragmentAbsorber);

            // Get the extracted text fragments
            textFragmentCollection = textFragmentAbsorber.TextFragments;

            // Loop through the fragments
            foreach (TextFragment textFragment in textFragmentCollection)
            {
                // Update text and other properties
                textFragment.Text = "IN " + boxTicket.ClassName.ToUpper() + " MENU BOX";
            }


            if (boxTicket.ClassName == "W" || boxTicket.ClassName == "Y")
            {
                // Create TextAbsorber object to find all instances of the input search phrase
                textFragmentAbsorber = new TextFragmentAbsorber("BREAKFAST CARD: 259B324_20 X 31");

                // Accept the absorber for all the pages
                pdfDocument.Pages.Accept(textFragmentAbsorber);

                // Get the extracted text fragments
                textFragmentCollection = textFragmentAbsorber.TextFragments;

                // Loop through the fragments
                foreach (TextFragment textFragment in textFragmentCollection)
                {
                    // Update text and other properties
                    textFragment.Text = "";
                }


                if (boxTicket.ClassName == "W")
                {
                    // Create TextAbsorber object to find all instances of the input search phrase
                    textFragmentAbsorber = new TextFragmentAbsorber("410C688_20 X 31");

                    // Accept the absorber for all the pages
                    pdfDocument.Pages.Accept(textFragmentAbsorber);

                    // Get the extracted text fragments
                    textFragmentCollection = textFragmentAbsorber.TextFragments;

                    // Loop through the fragments
                    foreach (TextFragment textFragment in textFragmentCollection)
                    {
                        // Update text and other properties
                        textFragment.Text = values["W_ClassMenuCode"] + " X " + values["MenuCount"];
                    }

                    textFragmentAbsorber = new TextFragmentAbsorber("MAIN");

                    // Accept the absorber for all the pages
                    pdfDocument.Pages.Accept(textFragmentAbsorber);

                    // Get the extracted text fragments
                    textFragmentCollection = textFragmentAbsorber.TextFragments;

                    // Loop through the fragments
                    foreach (TextFragment textFragment in textFragmentCollection)
                    {
                        // Update text and other properties
                        textFragment.Text = "W CLASS";
                    }
                }


                if (boxTicket.ClassName == "Y")
                {
                    // Create TextAbsorber object to find all instances of the input search phrase
                    textFragmentAbsorber = new TextFragmentAbsorber("410C688_20 X 31");

                    // Accept the absorber for all the pages
                    pdfDocument.Pages.Accept(textFragmentAbsorber);

                    // Get the extracted text fragments
                    textFragmentCollection = textFragmentAbsorber.TextFragments;

                    // Loop through the fragments
                    foreach (TextFragment textFragment in textFragmentCollection)
                    {
                        // Update text and other properties
                        textFragment.Text = values["Y_ClassMenuCode"] + " X " + values["MenuCount"];
                    }

                    textFragmentAbsorber = new TextFragmentAbsorber("MAIN");

                    // Accept the absorber for all the pages
                    pdfDocument.Pages.Accept(textFragmentAbsorber);

                    // Get the extracted text fragments
                    textFragmentCollection = textFragmentAbsorber.TextFragments;

                    // Loop through the fragments
                    foreach (TextFragment textFragment in textFragmentCollection)
                    {
                        // Update text and other properties
                        textFragment.Text = "Y CLASS";
                    }
                }

            }
            else
            {



                // Create TextAbsorber object to find all instances of the input search phrase
                textFragmentAbsorber = new TextFragmentAbsorber("410C688_20 X 31");

                // Accept the absorber for all the pages
                pdfDocument.Pages.Accept(textFragmentAbsorber);

                // Get the extracted text fragments
                textFragmentCollection = textFragmentAbsorber.TextFragments;

                // Loop through the fragments
                foreach (TextFragment textFragment in textFragmentCollection)
                {
                    // Update text and other properties
                    textFragment.Text = values["J_MainMenuCode"] + " X " + values["MenuCount"];
                }


                if (string.IsNullOrEmpty(values["J_BRKMenuCode"]))
                {
                    // Create TextAbsorber object to find all instances of the input search phrase
                    textFragmentAbsorber = new TextFragmentAbsorber("BREAKFAST CARD: 259B324_20 X 31");

                    // Accept the absorber for all the pages
                    pdfDocument.Pages.Accept(textFragmentAbsorber);

                    // Get the extracted text fragments
                    textFragmentCollection = textFragmentAbsorber.TextFragments;

                    // Loop through the fragments
                    foreach (TextFragment textFragment in textFragmentCollection)
                    {
                        // Update text and other properties
                        textFragment.Text = "";
                    }
                }
                else
                {
                    // Create TextAbsorber object to find all instances of the input search phrase
                    textFragmentAbsorber = new TextFragmentAbsorber("259B324_20 X 31");

                    // Accept the absorber for all the pages
                    pdfDocument.Pages.Accept(textFragmentAbsorber);

                    // Get the extracted text fragments
                    textFragmentCollection = textFragmentAbsorber.TextFragments;

                    // Loop through the fragments
                    foreach (TextFragment textFragment in textFragmentCollection)
                    {
                        // Update text and other properties
                        textFragment.Text = values["J_BRKMenuCode"] + " X " + values["MenuCount"];
                    }
                }
            }
            // Save resulting PDF document.
            //pdfDocument.Pages[1].Rotate = Rotation.on90;

            pdfDocument.Save(result);
        }



        private static void ProcessOutBoundPackingTicket(BoxTicketSortedData boxTicket, string input, Dictionary<string, string> values)
        {
            var result = pdfInputDestination + boxTicket.ID + ".pdf";

            var shortName = Path.GetFileNameWithoutExtension(result);
            var dir = Path.GetDirectoryName(result);

            // Open document
            Aspose.Pdf.Document pdfDocument = new Aspose.Pdf.Document(input);

            // Create TextAbsorber object to find all instances of the input search phrase
            TextFragmentAbsorber textFragmentAbsorber = new TextFragmentAbsorber("VAA MENU J CLASS");

            // Accept the absorber for all the pages
            pdfDocument.Pages.Accept(textFragmentAbsorber);

            // Get the extracted text fragments
            TextFragmentCollection textFragmentCollection = textFragmentAbsorber.TextFragments;

            // Loop through the fragments
            foreach (TextFragment textFragment in textFragmentCollection)
            {
                // Update text and other properties
                textFragment.Text = "VAA MENU " + boxTicket.ClassName.ToUpper() + " CLASS";

                //  textFragment.TextState.Font = FontRepository.FindFont("Helvetica-Bold");
                // textFragment.TextState.FontSize = 12;
            }


            // Create TextAbsorber object to find all instances of the input search phrase
            textFragmentAbsorber = new TextFragmentAbsorber("VS411");

            // Accept the absorber for all the pages
            pdfDocument.Pages.Accept(textFragmentAbsorber);

            // Get the extracted text fragments
            textFragmentCollection = textFragmentAbsorber.TextFragments;

            // Loop through the fragments
            foreach (TextFragment textFragment in textFragmentCollection)
            {
                // Update text and other properties
                textFragment.Text = values["LoadingFlight"];

                //textFragment.TextState.Font = FontRepository.FindFont("Helvetica-Bold");
                //textFragment.TextState.FontSize = 12;
            }


            // Create TextAbsorber object to find all instances of the input search phrase
            textFragmentAbsorber = new TextFragmentAbsorber("21/01/2020");

            // Accept the absorber for all the pages
            pdfDocument.Pages.Accept(textFragmentAbsorber);

            // Get the extracted text fragments
            textFragmentCollection = textFragmentAbsorber.TextFragments;

            // Loop through the fragments
            foreach (TextFragment textFragment in textFragmentCollection)
            {
                // Update text and other properties
                textFragment.Text = values["Date"];

                textFragment.TextState.Font = FontRepository.FindFont("Helvetica-Bold");
                ////textFragment.TextState.FontSize = 12;
            }

            // Create TextAbsorber object to find all instances of the input search phrase
            textFragmentAbsorber = new TextFragmentAbsorber("LHR-LOS");

            // Accept the absorber for all the pages
            pdfDocument.Pages.Accept(textFragmentAbsorber);

            // Get the extracted text fragments
            textFragmentCollection = textFragmentAbsorber.TextFragments;

            // Loop through the fragments
            foreach (TextFragment textFragment in textFragmentCollection)
            {
                // Update text and other properties
                textFragment.Text = values["Route"];
            }


            // Create TextAbsorber object to find all instances of the input search phrase
            textFragmentAbsorber = new TextFragmentAbsorber("356L993");

            // Accept the absorber for all the pages
            pdfDocument.Pages.Accept(textFragmentAbsorber);

            // Get the extracted text fragments
            textFragmentCollection = textFragmentAbsorber.TextFragments;

            // Loop through the fragments
            foreach (TextFragment textFragment in textFragmentCollection)
            {
                // Update text and other properties
                textFragment.Text = values["LotNumber"];
            }


            // Create TextAbsorber object to find all instances of the input search phrase
            textFragmentAbsorber = new TextFragmentAbsorber("IN J MENU BOX");

            // Accept the absorber for all the pages
            pdfDocument.Pages.Accept(textFragmentAbsorber);

            // Get the extracted text fragments
            textFragmentCollection = textFragmentAbsorber.TextFragments;

            // Loop through the fragments
            foreach (TextFragment textFragment in textFragmentCollection)
            {
                // Update text and other properties
                textFragment.Text = "IN " + boxTicket.ClassName.ToUpper() + " MENU BOX";
            }


            if (boxTicket.ClassName == "W" || boxTicket.ClassName == "Y")
            {
                // Create TextAbsorber object to find all instances of the input search phrase
                textFragmentAbsorber = new TextFragmentAbsorber("BREAKFAST CARD: 254B121_20 X 45");

                // Accept the absorber for all the pages
                pdfDocument.Pages.Accept(textFragmentAbsorber);

                // Get the extracted text fragments
                textFragmentCollection = textFragmentAbsorber.TextFragments;

                // Loop through the fragments
                foreach (TextFragment textFragment in textFragmentCollection)
                {
                    // Update text and other properties
                    textFragment.Text = "";
                }


                if (boxTicket.ClassName == "W")
                {
                    // Create TextAbsorber object to find all instances of the input search phrase
                    textFragmentAbsorber = new TextFragmentAbsorber("542C246_20 X 45");

                    // Accept the absorber for all the pages
                    pdfDocument.Pages.Accept(textFragmentAbsorber);

                    // Get the extracted text fragments
                    textFragmentCollection = textFragmentAbsorber.TextFragments;

                    // Loop through the fragments
                    foreach (TextFragment textFragment in textFragmentCollection)
                    {
                        // Update text and other properties
                        textFragment.Text = values["W_ClassMenuCode"] + " X " + values["MenuCount"];
                    }


                    textFragmentAbsorber = new TextFragmentAbsorber("MAIN");

                    // Accept the absorber for all the pages
                    pdfDocument.Pages.Accept(textFragmentAbsorber);

                    // Get the extracted text fragments
                    textFragmentCollection = textFragmentAbsorber.TextFragments;

                    // Loop through the fragments
                    foreach (TextFragment textFragment in textFragmentCollection)
                    {
                        // Update text and other properties
                        textFragment.Text = "W CLASS";
                    }

                }


                if (boxTicket.ClassName == "Y")
                {
                    // Create TextAbsorber object to find all instances of the input search phrase
                    textFragmentAbsorber = new TextFragmentAbsorber("542C246_20 X 45");

                    // Accept the absorber for all the pages
                    pdfDocument.Pages.Accept(textFragmentAbsorber);

                    // Get the extracted text fragments
                    textFragmentCollection = textFragmentAbsorber.TextFragments;

                    // Loop through the fragments
                    foreach (TextFragment textFragment in textFragmentCollection)
                    {
                        // Update text and other properties
                        textFragment.Text = values["Y_ClassMenuCode"] + " X " + values["MenuCount"];
                    }

                    textFragmentAbsorber = new TextFragmentAbsorber("MAIN");

                    // Accept the absorber for all the pages
                    pdfDocument.Pages.Accept(textFragmentAbsorber);

                    // Get the extracted text fragments
                    textFragmentCollection = textFragmentAbsorber.TextFragments;

                    // Loop through the fragments
                    foreach (TextFragment textFragment in textFragmentCollection)
                    {
                        // Update text and other properties
                        textFragment.Text = "Y CLASS";
                    }

                }

            }
            else
            {
                // Create TextAbsorber object to find all instances of the input search phrase
                textFragmentAbsorber = new TextFragmentAbsorber("542C246_20 X 45");

                // Accept the absorber for all the pages
                pdfDocument.Pages.Accept(textFragmentAbsorber);

                // Get the extracted text fragments
                textFragmentCollection = textFragmentAbsorber.TextFragments;

                // Loop through the fragments
                foreach (TextFragment textFragment in textFragmentCollection)
                {
                    // Update text and other properties
                    textFragment.Text = values["J_MainMenuCode"] + " X " + values["MenuCount"];
                }


                if (string.IsNullOrEmpty(values["J_BRKMenuCode"]))
                {
                    // Create TextAbsorber object to find all instances of the input search phrase
                    textFragmentAbsorber = new TextFragmentAbsorber("BREAKFAST CARD: 254B121_20 X 45");

                    // Accept the absorber for all the pages
                    pdfDocument.Pages.Accept(textFragmentAbsorber);

                    // Get the extracted text fragments
                    textFragmentCollection = textFragmentAbsorber.TextFragments;

                    // Loop through the fragments
                    foreach (TextFragment textFragment in textFragmentCollection)
                    {
                        // Update text and other properties
                        textFragment.Text = "";
                    }
                }
                else
                {
                    // Create TextAbsorber object to find all instances of the input search phrase
                    textFragmentAbsorber = new TextFragmentAbsorber("254B121_20 X 45");

                    // Accept the absorber for all the pages
                    pdfDocument.Pages.Accept(textFragmentAbsorber);

                    // Get the extracted text fragments
                    textFragmentCollection = textFragmentAbsorber.TextFragments;

                    // Loop through the fragments
                    foreach (TextFragment textFragment in textFragmentCollection)
                    {
                        // Update text and other properties
                        textFragment.Text = values["J_BRKMenuCode"] + " X " + values["MenuCount"];
                    }
                }
            }
            // Save resulting PDF document.
            //pdfDocument.Pages[1].Rotate = Rotation.on90;

            pdfDocument.Save(result);
        }


        private void SetBoxTicketSortCriteria(long orderId, List<tBoxTicketData> boxTicketData, List<BoxTicketSortedData> SortedBoxTicketDataList)
        {
            var classNameSearch = "J";
            var shipToSearch = "LHR";

            if (ShipToPresent(boxTicketData, shipToSearch))
            {
                BoxReportSortDataPreparation(shipToSearch, classNameSearch, orderId, boxTicketData, SortedBoxTicketDataList);

                classNameSearch = "W";
                BoxReportSortDataPreparation(shipToSearch, classNameSearch, orderId, boxTicketData, SortedBoxTicketDataList);

                classNameSearch = "Y";
                BoxReportSortDataPreparation(shipToSearch, classNameSearch, orderId, boxTicketData, SortedBoxTicketDataList);
            }


            classNameSearch = "J";
            shipToSearch = "LGW";

            if (ShipToPresent(boxTicketData, shipToSearch))
            {
                BoxReportSortDataPreparation(shipToSearch, classNameSearch, orderId, boxTicketData, SortedBoxTicketDataList);

                classNameSearch = "W";
                BoxReportSortDataPreparation(shipToSearch, classNameSearch, orderId, boxTicketData, SortedBoxTicketDataList);

                classNameSearch = "Y";
                BoxReportSortDataPreparation(shipToSearch, classNameSearch, orderId, boxTicketData, SortedBoxTicketDataList);
            }

            classNameSearch = "J";
            shipToSearch = "MAN";
            if (ShipToPresent(boxTicketData, shipToSearch))
            {
                BoxReportSortDataPreparation(shipToSearch, classNameSearch, orderId, boxTicketData, SortedBoxTicketDataList);

                classNameSearch = "W";
                BoxReportSortDataPreparation(shipToSearch, classNameSearch, orderId, boxTicketData, SortedBoxTicketDataList);

                classNameSearch = "Y";
                BoxReportSortDataPreparation(shipToSearch, classNameSearch, orderId, boxTicketData, SortedBoxTicketDataList);
            }

            classNameSearch = "J";
            shipToSearch = "GLA";
            if (ShipToPresent(boxTicketData, shipToSearch))
            {
                BoxReportSortDataPreparation(shipToSearch, classNameSearch, orderId, boxTicketData, SortedBoxTicketDataList);

                classNameSearch = "W";
                BoxReportSortDataPreparation(shipToSearch, classNameSearch, orderId, boxTicketData, SortedBoxTicketDataList);

                classNameSearch = "Y";
                BoxReportSortDataPreparation(shipToSearch, classNameSearch, orderId, boxTicketData, SortedBoxTicketDataList);
            }

            classNameSearch = "J";
            shipToSearch = "EDI";
            if (ShipToPresent(boxTicketData, shipToSearch))
            {
                BoxReportSortDataPreparation(shipToSearch, classNameSearch, orderId, boxTicketData, SortedBoxTicketDataList);

                classNameSearch = "W";
                BoxReportSortDataPreparation(shipToSearch, classNameSearch, orderId, boxTicketData, SortedBoxTicketDataList);

                classNameSearch = "Y";
                BoxReportSortDataPreparation(shipToSearch, classNameSearch, orderId, boxTicketData, SortedBoxTicketDataList);

            }
        }

        private void BoxReportSortDataPreparation(string shipToSearch, string classNameSearch, long orderId, List<tBoxTicketData> boxTicketData, List<BoxTicketSortedData> SortedBoxTicketDataList)
        {

            List<tBoxTicketData> data = new List<tBoxTicketData>();

            List<DateTime?> OutboundDates = new List<DateTime?>();


            foreach (var boxTicket in boxTicketData)
            {
                if (!(boxTicket.ClassName == classNameSearch && boxTicket.ShipTo == shipToSearch))
                    continue;

                data.Add(boxTicket);
            }

            List<tBoxTicketData> dataOutBound = new List<tBoxTicketData>();

            foreach (var d in data)
            {
                if (d.Bound.ToLower() == "outbound")
                {
                    dataOutBound.Add(d);
                    if (!OutboundDates.Contains(d.Date))
                        OutboundDates.Add(d.Date);
                }
            }



            foreach (var date in OutboundDates)
            {
                var dateBoxOutboundData = dataOutBound.Where(d => d.Date == date).ToList().OrderBy(x => x.Time).ToList();

                List<tBoxTicketData> dataInBound = new List<tBoxTicketData>();

                foreach (var d in data)
                {
                    if (d.Bound.ToLower() == "inbound" && d.Date == date)
                        dataInBound.Add(d);
                }

                foreach (var dateBoxOutbound in dateBoxOutboundData)
                {
                    if (dateBoxOutbound == null)
                        continue;

                    BoxTicketSortedData sortDataOut = new BoxTicketSortedData();

                    //outbound write
                    sortDataOut.ID = dateBoxOutbound.ID;
                    sortDataOut.ClassName = dateBoxOutbound.ClassName;
                    sortDataOut.ShipTo = dateBoxOutbound.ShipTo;
                    sortDataOut.FlightNo = dateBoxOutbound.FlightNo;
                    sortDataOut.Route = dateBoxOutbound.Route;
                    sortDataOut.RouteId = dateBoxOutbound.RouteId;
                    sortDataOut.Time = dateBoxOutbound.Time;
                    sortDataOut.Date = dateBoxOutbound.Date;
                    sortDataOut.Count = dateBoxOutbound.Count;
                    sortDataOut.Bound = dateBoxOutbound.Bound;
                    sortDataOut.MenuCode = dateBoxOutbound.MenuCode;

                    if (dateBoxOutbound.ClassName == "J")
                        sortDataOut.TEAMenuCode = dateBoxOutbound.TEAMenuCode;
                    else
                        sortDataOut.TEAMenuCode = "";

                    if (dateBoxOutbound.ClassName == "J")
                        sortDataOut.BRKMenuCode = dateBoxOutbound.BRKMenuCode;
                    else
                        sortDataOut.BRKMenuCode = "";

                    SortedBoxTicketDataList.Add(sortDataOut);

                    //inbound write
                    var OutboundRoute = dateBoxOutbound.Route.Trim();

                    var OutboundRouteArray = OutboundRoute.Split(new char[] { '-' });

                    if (OutboundRouteArray.Length == 2)
                    {
                        var inboundRoute = OutboundRouteArray[1] + "-" + OutboundRouteArray[0];
                        var outboundFlight = dateBoxOutbound.FlightNo;
                        outboundFlight = outboundFlight.Replace("VS", "");

                        int InboundFlight = 0;

                        try
                        {
                            InboundFlight = Convert.ToInt16(outboundFlight) + 1;
                        }
                        catch { }

                        string InboundFlightName = "VS" + InboundFlight.ToString().PadLeft(3, '0');

                        var inboundForOutbound = (from d in data where d.Date == date && d.ClassName == dateBoxOutbound.ClassName && d.Route == inboundRoute && d.Bound == "Inbound" && d.FlightNo == InboundFlightName select d).FirstOrDefault();

                        if (inboundForOutbound != null)
                        {
                            BoxTicketSortedData sortDataIn = new BoxTicketSortedData();
                            sortDataIn.ID = inboundForOutbound.ID;
                            sortDataIn.ClassName = inboundForOutbound.ClassName;
                            sortDataIn.ShipTo = inboundForOutbound.ShipTo;
                            sortDataIn.FlightNo = inboundForOutbound.FlightNo;
                            sortDataIn.Route = inboundForOutbound.Route;
                            sortDataIn.RouteId = inboundForOutbound.RouteId;
                            sortDataIn.Time = inboundForOutbound.Time;
                            sortDataIn.Date = inboundForOutbound.Date;
                            sortDataIn.Count = inboundForOutbound.Count;
                            sortDataIn.Bound = inboundForOutbound.Bound;
                            sortDataIn.MenuCode = inboundForOutbound.MenuCode;

                            if (inboundForOutbound.ClassName == "J")
                                sortDataIn.TEAMenuCode = inboundForOutbound.TEAMenuCode;
                            else
                                sortDataIn.TEAMenuCode = "";

                            if (inboundForOutbound.ClassName == "J")
                                sortDataIn.BRKMenuCode = inboundForOutbound.BRKMenuCode;
                            else
                                sortDataIn.BRKMenuCode = "";

                            SortedBoxTicketDataList.Add(sortDataIn);
                        }
                    }
                }

                //check for any missed inbound which is purely inbound and no outbound associated

                foreach (var inbound in dataInBound)
                {
                    bool found = false;

                    foreach (var sortData in SortedBoxTicketDataList)
                    {
                        if (sortData.Bound.ToLower() == "inbound")
                        {
                            if (inbound.Date == sortData.Date && inbound.FlightNo == sortData.FlightNo && inbound.Time == sortData.Time && inbound.Route == sortData.Route && inbound.Count == sortData.Count)
                            {
                                found = true;
                                break;
                            }
                        }
                    }
                    if (!found)
                    {
                        //add this at the end
                        BoxTicketSortedData sortDataIn = new BoxTicketSortedData();
                        sortDataIn.ID = inbound.ID;
                        sortDataIn.ClassName = inbound.ClassName;
                        sortDataIn.ShipTo = inbound.ShipTo;
                        sortDataIn.FlightNo = inbound.FlightNo;
                        sortDataIn.Route = inbound.Route;
                        sortDataIn.RouteId = inbound.RouteId;
                        sortDataIn.Time = inbound.Time;
                        sortDataIn.Date = inbound.Date;
                        sortDataIn.Count = inbound.Count;
                        sortDataIn.Bound = inbound.Bound;
                        sortDataIn.MenuCode = inbound.MenuCode;

                        if (inbound.ClassName == "J")
                            sortDataIn.TEAMenuCode = inbound.TEAMenuCode;
                        else
                            sortDataIn.TEAMenuCode = "";

                        if (inbound.ClassName == "J")
                            sortDataIn.BRKMenuCode = inbound.BRKMenuCode;
                        else
                            sortDataIn.BRKMenuCode = "";

                        SortedBoxTicketDataList.Add(sortDataIn);
                    }

                }

            }
        }

        public void CreateBoxTicketPDfs(long orderId, List<BoxTicketSortedData> sortedBoxTicketData)
        {

            
            pdfInputDestination = EmmaPDFPathFolder + @"\BoxTickets\";
            pdfOutputDestination = EmmaPDFPathFolder + @"\BoxTickets\Output";
            List<byte[]> filesByte = new List<byte[]>();



            foreach (var data in sortedBoxTicketData)
            {
                if (!string.IsNullOrEmpty(data.ID.ToString()))
                {
                    var filePath = System.IO.Path.Combine(pdfInputDestination, data.ID.ToString() + ".pdf");

                    if (File.Exists(filePath))
                    {
                        // TODO catch missing file
                        var thisFileBytes = System.IO.File.ReadAllBytes(filePath);
                        for (int i = 0; i < 1; i++)
                        {
                            filesByte.Add(thisFileBytes);
                        }
                    }

                }
            }


            var outputFileName = orderId + ".pdf";
            System.IO.File.WriteAllBytes(Path.Combine(pdfOutputDestination, outputFileName), PdfMerger.MergeFiles(filesByte));


        }

        public void GenerateSinglePackingTicketPDFs(int jobID, long boxTicketID)
        {
            try
            {
                chili.Connect("VAA");

                pdfInputDestination = EmmaPDFPathFolder + @"\BoxTickets\";
                pdfOutputDestination = EmmaPDFPathFolder + @"\BoxTickets\Output";


                var fileName = boxTicketID.ToString() + ".pdf";
                var filePath = System.IO.Path.Combine(pdfInputDestination, fileName);

                var task = chili.GetPdfGenerationTaskPackingTicket(jobID, boxTicketID);

                if (task != null)
                {

                    var loop = true;
                    do
                    {

                        switch (task.Status)
                        {

                            //-----------------------------------------
                            // Ready - start export
                            //-----------------------------------------

                            case "Ready":
                                // TODO load export settings from job
                                var exportSettings = "<item name=\"Print Ready\" id=\"39ab3751-cd08-486d-8bba-6e1adbfd9aa7\" relativePath=\"\" outputSplitPages=\"1\" layoutID=\"\" createAllPages=\"True\" pageRangeStart=\"1\" includeAnnotations=\"False\" includeLinks=\"False\" includeGuides=\"False\" includeTextRangeBorder=\"True\" includePageMargins=\"True\" pdfTitle=\"\" embedFonts=\"True\" fastWebView=\"False\" allowModifyDocument=\"True\" allowPrinting=\"True\" pdfAuthor=\"ESP Web2Print\" pdfCreator=\"MergeIt\" includeFrameBorder=\"True\" userPassword=\"\" ownerPassword=\"\" pdfSubject=\"\" pdfKeywords=\"\" watermarkText=\"\" pdfLayers=\"False\" imageQuality=\"original\" createSingleFile=\"True\" createSpreads=\"False\" serverOutputLocation=\"\" includeCropMarks=\"True\" includeBleedMarks=\"True\" includeImages=\"True\" includeNonPrintingLayers=\"False\" slugLeft=\"5 mm\" slugTop=\"5 mm\" slugRight=\"5 mm\" slugBottom=\"5 mm\" bleedRight=\"6 mm\" bleedTop=\"6 mm\" bleedLeft=\"6 mm\" useDocumentBleed=\"False\" useDocumentSlug=\"True\" optimizationOptions=\"\" includeGrid=\"True\" preflight_overrideDocumentSettings=\"False\" preflight_minOutputResolution=\"72\" preflight_minResizePercentage=\"70\" preflight_maxResizePercentage=\"120\" includeBleed=\"True\" dataSourceCreate=\"False\" dataSourceIncludeBackgroundLayers=\"False\" dataSourceCreateBackgroundPDF=\"True\" dataSourceRowsPerPDF=\"1\" dataSourceMaxRows=\"-1\" includeAdOverlays=\"False\" includeSectionBreaks=\"False\" dontDeleteExistingDirectory=\"False\" collateOutputWidth=\"210mm\" collateNumRows=\"3\" collateNumCols=\"3\" collateOutputHeight=\"297mm\" collateColumnWidth=\"50mm\" collateStartX=\"10mm\" collateStartY=\"10mm\" collateMarginX=\"10mm\" allowExtractContent=\"True\" collateMarginY=\"10mm\" collateOutput=\"False\" collateDrawPageBorder=\"False\" collateIncludeFileHeader=\"False\" includePageLabels=\"False\" includeFrameInset=\"True\" includeBaseLineGrid=\"True\" missingEditPlaceHolder=\"False\" missingAdPlaceHolder=\"False\" missingAdPlaceHolderColor=\"#FF00FF\" missingAdSizePlaceHolder=\"False\" missingAdSizePlaceHolderColor=\"#FF00FF\" rgbSwatches=\"False\" dropshadowQuality=\"150\" missingEditPlaceHolderColor=\"#FF00FF\" annotationBorderColor=\"#FF0000\" annotationFillColor=\"#FFFFFF\" annotationOpacity=\"50\" linkBorderColor=\"#0000FF\" dropshadowTextQuality=\"150\" bleedBottom=\"6 mm\" barWidthReduction=\"0 mm\" includeSlug=\"True\" markOffset=\"3 mm\" markWidth=\"0.5pt\" dataSourceEngine=\"editor_cli\" dataSourceNumConcurrent=\"3\" dataSourceUnspecifiedContentType=\"variable_data\" dataSourceIncludeGenerationLog=\"False\" dataSourceUnspecifiedPageContentType=\"variable_data\" outputIntentRegistryName=\"\" outputIntentConditionIdentifier=\"\" outputIntent=\"\" pdfStandard=\"\" pdfVersion=\"4\" debugVtContent=\"False\" watermarkType=\"\" watermarkPdfAssetID=\"\" watermarkPdfAnchor=\"top_left\" pageRangeEnd=\"999999\" watermarkPdfSize=\"original\" convertBlacks=\"False\" convertAnyK100=\"True\" convertSystemBlack=\"True\" convert0_0_0_100=\"True\" convertBlackToC=\"63\" convertBlackToK=\"100\" convertBlackToY=\"51\" convertBlackToM=\"52\" debugDropShadowsWithoutBlur=\"False\"><pdfvt_metaDataConfigItems/></item>";
                                var createPdfResult = new XmlDocument();
                                createPdfResult.LoadXml(chili.WebService.DocumentCreatePDF(chili.ApiKey, task.ChiliDocumentId, exportSettings, 5));
                                var taskID = createPdfResult.DocumentElement.GetAttribute("id");
                                task.ChiliTaskId = taskID;
                                task.Status = "TaskStarted";
                                break;

                            //-----------------------------------------
                            // Task started - awaiting update
                            //-----------------------------------------

                            case "TaskStarted":
                            case "TaskNotReady":

                                // Check task status
                                var getTaskStatusResult = new XmlDocument();
                                getTaskStatusResult.LoadXml(chili.WebService.TaskGetStatus(chili.ApiKey, task.ChiliTaskId));
                                if (getTaskStatusResult.DocumentElement.GetAttribute("finished") == "True")
                                {
                                    var resultXml = new XmlDocument();
                                    var resultText = getTaskStatusResult.DocumentElement.GetAttribute("result");
                                    if (!string.IsNullOrEmpty(resultText))
                                    {
                                        resultXml.LoadXml(resultText);
                                        var url = resultXml.DocumentElement.GetAttribute("url");
                                        task.Status = "ReadyToDownload";
                                        task.ChiliPdfurl = url;
                                        task.ChiliError = null;

                                    }
                                    else
                                    {
                                        var errorMessageText = getTaskStatusResult.DocumentElement.GetAttribute("errorMessage");
                                        task.Status = "Error";
                                        task.ChiliError = errorMessageText;
                                    }
                                }
                                else if (getTaskStatusResult.DocumentElement.GetAttribute("found") == "false")
                                {
                                    task.Status = "Error";
                                    task.ChiliError = "Task not found. Please retry";
                                }
                                else
                                {
                                    task.Status = "TaskNotReady";
                                    task.ChiliError = null;
                                }
                                break;

                            //-----------------------------------------
                            // PDF generated - download it
                            //-----------------------------------------

                            case "ReadyToDownload":
                                var wc = new WebClient();
                                try
                                {
                                    wc.DownloadFile(task.ChiliPdfurl, filePath);
                                }
                                catch { }
                                // TODO catch 404 etc
                                task.Status = "Downloaded";
                                task.LocalPdfFile = fileName;
                                task.ChiliError = null;
                                break;

                            //-----------------------------------------
                            // PDF downloaded
                            //-----------------------------------------

                            case "Downloaded":
                                if (System.IO.File.Exists(filePath))
                                {
                                    //task.Status = "Done";
                                    //task.ChiliError = null;
                                    task.Status = "Done";
                                    task.ChiliError = null;
                                    task.LocalPdfFile = fileName;
                                    loop = false;
                                    chili.UpdateTaskPackingTicket(task);

                                }
                                else
                                {
                                    task.Status = "ReadyToDownload";
                                    task.ChiliError = "PDF not found locally; downloading again";
                                    //loop = true;
                                }
                                break;

                            //-----------------------------------------
                            // Error - need to start again
                            //-----------------------------------------

                            case "Error":
                                task.Status = "Ready";
                                task.ChiliError = null;
                                break;
                        }

                        chili.UpdateTaskPackingTicket(task);
                    } while (loop);


                }
            }
            catch (Exception e)
            {
                //write to Elma
                ErrorSignal.FromCurrentContext().Raise(e);
            }
        }

    }

}
