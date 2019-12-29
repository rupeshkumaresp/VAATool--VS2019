using Elmah;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Xml;
using VAA.BusinessComponents.Interfaces;
using VAA.CommonComponents;
using VAA.DataAccess;

namespace VAA.BusinessComponents
{
    /// <summary>
    ///  Menu processor Engine    
    /// </summary>
    public class MenuProcessor : IMenuProcessor
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
        string pathServerMenuPDFs = @"\\web2print\Emma_PDFs\MENU PDFS";
        string UserName = "graphite.rack";
        string Password = "123Grand?";
        string domain = "ESPC";


        #region Import Methods

        /// <summary>
        /// Validate the service plan and return the missing base items
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public List<string> ValidateServicePlanMenu(Stream stream)
        {
            List<string> missingServiceCode = new List<string>();

            try
            {
                ExcelRecordImporter importer = new ExcelRecordImporter(stream);

                int worksheet = 0;
                foreach (var dataSetName in importer.GetDataSetNames())
                {
                    if (worksheet == 0)
                    {
                        var importedRows = importer.Import(dataSetName);

                        if (importedRows != null && importedRows.Any())
                        {
                            foreach (var row in importedRows)
                            {
                                var serviceCode = Convert.ToString(row["service code"]);

                                if (row["service"] == "TEA" || row["service"] == "BRK" || row["service"] == "BRD" || row["service"] == "STA" || row["service"] == "ENT" || row["service"] == "DES" || row["service"] == "CHE" || row["service"] == "GRAZE")
                                {
                                    if (!string.IsNullOrEmpty(serviceCode))
                                    {
                                        var baseItem = _baseItemManagement.GetBaseItemList(serviceCode);
                                        if (baseItem.Count == 0)
                                        {
                                            missingServiceCode.Add(serviceCode);
                                        }
                                    }
                                }
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //write to Elma
                ErrorSignal.FromCurrentContext().Raise(ex);
            }
            return missingServiceCode;
        }

        public void ImportMenu(long cycleId, int classId, int menuTypeId, Stream stream, int userId)
        {
            ExcelRecordImporter importer = new ExcelRecordImporter(stream);
            try
            {
                int worksheet = 0;
                foreach (var dataSetName in importer.GetDataSetNames())
                {
                    if (worksheet == 0)
                    {
                        var importedRows = importer.Import(dataSetName);

                        if (importedRows != null && importedRows.Any())
                        {
                            var allRoutes = GetAllRoutesInUploadSheet(importedRows);

                            //correct the imported rows %
                            CleanImportedRowsData(importedRows, allRoutes);

                            var allDistinctMenu = GetAllDistinctMenu(importedRows, allRoutes, menuTypeId);

                            //create all the distinct menus
                            List<long> menuIdCollection = new List<long>();

                            //Build menu, assign routes and build menu items
                            GenerateDistinctMenuForUpload(allDistinctMenu, menuIdCollection, importedRows, cycleId, classId, menuTypeId, userId);

                            if (menuIdCollection.Count == 0)
                                return;

                            //Update MenuNames
                            UpdateMenuNames(menuIdCollection);

                            //create menu template
                            CreateMenuTemplate(menuIdCollection);

                            ////build chili document
                            CreateChiliDocumentForMenu(menuIdCollection);

                            ////apply chili variable - create all the chili variable collection and apply to chili document on the fly
                            CreateChiliVariableCollectionAndApply(menuIdCollection);

                            //write to Menu History Table
                            UpdateMenuHistory(menuIdCollection, userId, "Menu Created");
                        }
                    }
                    worksheet++;
                }
            }
            catch (Exception ex)
            {
                //write to Elma
                ErrorSignal.FromCurrentContext().Raise(ex);
            }
        }

        private void CleanImportedRowsData(IEnumerable<Dictionary<string, string>> importedRows, List<string> allRoutes)
        {
            try
            {
                foreach (var row in importedRows)
                {
                    for (int i = 0; i < allRoutes.Count; i++)
                    {
                        var menuItemVolume = Convert.ToString(row[allRoutes[i]]);

                        if (!string.IsNullOrEmpty(menuItemVolume) && menuItemVolume != "NV" /* && menuItemVolume.Contains("%")*/)
                            row[allRoutes[i]] = "100%";
                    }
                }
            }
            catch (Exception e)
            {
                //write to Elma
                ErrorSignal.FromCurrentContext().Raise(e);
            }
        }

        public void UpdateMenuNames(List<long> menuIdCollection)
        {
            for (int i = 0; i < menuIdCollection.Count; i++)
            {
                try
                {
                    var menuId = menuIdCollection[i];
                    _menuManagement.UpdateMenuNameBasedOnRoute(menuId);
                }
                catch (Exception e)
                {
                    //write to Elma
                    ErrorSignal.FromCurrentContext().Raise(e);
                }
            }
        }

        public void UpdateMenuHistory(List<long> menuIdCollection, int userId, string action)
        {
            for (int i = 0; i < menuIdCollection.Count; i++)
            {
                try
                {
                    var menuId = menuIdCollection[i];
                    _menuManagement.UpdateMenuHistory(menuId, userId, action);
                }
                catch (Exception e)
                {
                    //write to Elma
                    ErrorSignal.FromCurrentContext().Raise(e);
                }
            }
        }


        public void CreateMenuTemplate(List<long> menuIdCollection)
        {
            for (int i = 0; i < menuIdCollection.Count; i++)
            {
                try
                {
                    var menuId = menuIdCollection[i];
                    var menu = _menuManagement.GetMenuById(menuId);
                    var languageId = Convert.ToInt32(menu.LanguageId);
                    var menuTypeId = Convert.ToInt32(menu.MenuTypeId);

                    //assign languageId as these menu are only in english
                    if (menuTypeId == 2 || menuTypeId == 3 || menuTypeId == 4 || menuTypeId == 5 || menuTypeId == 6 || menuTypeId == 7)
                        languageId = 1;

                    var template = _menuManagement.GetTemplate(menuTypeId, languageId);

                    //menu template is already created for this for main manu - BRK, GRAZE, TEA stuff then do no create again
                    var menutemplate = _menuManagement.GetMenuTemplate(menuId);

                    if (menutemplate == null)
                    {
                        //get the template id for menu
                        int templateId = template.TemplateID;
                        _menuManagement.CreateMenuTemplate(menuId, templateId);
                    }
                }
                catch (Exception e)
                {
                    //write to Elma
                    ErrorSignal.FromCurrentContext().Raise(e);
                }
            }
        }


        /// <summary>
        /// GET ALL THE ROUTES IN THE UPLOAD SHEET
        /// </summary>
        /// <param name="importedRows"></param>
        /// <returns></returns>
        private static List<string> GetAllRoutesInUploadSheet(IEnumerable<Dictionary<string, string>> importedRows)
        {
            List<string> allRoutes = new List<string>();

            foreach (var importedRow in importedRows)
            {
                var keys = importedRow.Keys;
                int index = 1;

                foreach (var key in keys)
                {
                    if (index >= 8)
                        allRoutes.Add(key);
                    index++;
                }

                break;
            }

            return allRoutes;
        }

        /// <summary>
        /// GET ALL THE DISTINCT MENUS IN THE UPLOAD SHEET
        /// </summary>
        /// <param name="importedRows"></param>
        /// <param name="allRoutes"></param>
        /// <returns></returns>
        private List<string> GetAllDistinctMenu(IEnumerable<Dictionary<string, string>> importedRows, List<string> allRoutes, int menuTypeId)
        {
            Dictionary<string, string> routeFoodPercentageDictionary = allRoutes.ToDictionary(r => r, r => "");

            foreach (var row in importedRows)
            {
                if (menuTypeId == 1 || menuTypeId == 10 || menuTypeId == 13 || menuTypeId == 5 || menuTypeId == 6)
                {
                    if (row["service"] == "SNK" || row["service"] == "BRD" || row["service"] == "STA" || row["service"] == "ENT" || row["service"] == "DES" || row["service"] == "CHE" || row["service"] == "GRAZE")
                    {
                        foreach (var route in allRoutes)
                        {
                            string value = "";
                            row.TryGetValue(route, out value);
                            routeFoodPercentageDictionary[route] = routeFoodPercentageDictionary[route] + " " + value;
                        }
                    }

                    //for night cap turn down we need the SNK items
                    if (row["service"] == "SNK" && menuTypeId == 1)
                    {
                        foreach (var route in allRoutes)
                        {
                            string value = "";
                            row.TryGetValue(route, out value);
                            routeFoodPercentageDictionary[route] = routeFoodPercentageDictionary[route] + " " + value;
                        }
                    }
                }

                if (menuTypeId == 2)
                {
                    if (row["service"] == "TEA")
                    {
                        foreach (var route in allRoutes)
                        {
                            string value = "";
                            row.TryGetValue(route, out value);
                            routeFoodPercentageDictionary[route] = routeFoodPercentageDictionary[route] + " " + value;
                        }
                    }
                }

                if (menuTypeId == 3)
                {
                    if (row["service"] == "BRK")
                    {
                        foreach (var route in allRoutes)
                        {
                            string value = "";
                            row.TryGetValue(route, out value);
                            routeFoodPercentageDictionary[route] = routeFoodPercentageDictionary[route] + " " + value;
                        }
                    }
                }

            }

            var lookupinFoodDictionary = routeFoodPercentageDictionary.ToLookup(x => x.Value, x => x.Key).Where(x => x.Count() >= 1);

            List<string> distinctMenu = new List<string>();

            foreach (var item in lookupinFoodDictionary)
            {
                var keys = item.Aggregate("", (s, v) => s + ", " + v);

                if (keys.StartsWith(","))
                    keys = keys.Substring(2, keys.Length - 2);

                distinctMenu.Add(keys.ToUpper());
            }

            return distinctMenu;
        }


        /// <summary>
        /// CREATE ALL THE DISTINCT MENUS
        /// </summary>
        /// <param name="allDistinctMenu"></param>
        private void GenerateDistinctMenuForUpload(List<string> allDistinctMenu, List<long> menuIdCollection, IEnumerable<Dictionary<string, string>> importedRows, long cycleId, int classId, int menuTypeId, int userId)
        {
            for (int i = 0; i < allDistinctMenu.Count; i++)
            {
                try
                {
                    //split the first route and get all the menu items for this route column while looping in rows
                    var allRoutes = allDistinctMenu[i].Split(new char[] { ',' });

                    var firstRoute = allRoutes[0];
                    firstRoute = firstRoute.ToLower();

                    bool createTeaMenu = false;
                    bool createBreakfastMenu = false;

                    //if menu type is breakfast or afternoon tea
                    //check all the items and see if they have values or it is NV
                    //if NV then do not create this menu

                    if (menuTypeId == 2) //TEA
                    {
                        foreach (var row in importedRows)
                        {
                            if (row["service"] == "TEA")
                            {
                                var menuItemVolume = Convert.ToString(row[firstRoute]);

                                if (!string.IsNullOrEmpty(menuItemVolume) && menuItemVolume.Trim() != "NV")
                                {
                                    createTeaMenu = true;
                                    break;
                                }
                            }
                        }

                        if (!createTeaMenu)
                            continue;
                    }

                    if (menuTypeId == 3) //BREAKFAST
                    {
                        foreach (var row in importedRows)
                        {
                            if (row["service"] == "BRK")
                            {
                                var menuItemVolume = Convert.ToString(row[firstRoute]);

                                if (!string.IsNullOrEmpty(menuItemVolume) && menuItemVolume.Trim() != "NV")
                                {
                                    createBreakfastMenu = true;
                                    break;
                                }
                            }
                        }

                        if (!createBreakfastMenu)
                            continue;
                    }

                    var cycle = _cycleManagement.GetCycle(cycleId);

                    var menuname = cycle.ShortName + "_" + _menuManagement.GetClassShortName(classId) + "_" + _menuManagement.GetMenuTypeName(menuTypeId) /*+ "_M" + (i + 1).ToString()*/;

                    //3 digit - CHAR - 3 digit
                    var menuCode = "MNU" + Helper.Get8Digits();
                    menuCode = GenerateMenuCode(menuTypeId, menuCode);

                    int languageId = 1;

                    //since language based are uploaded seperately and these are only single entity so check for count
                    if (allDistinctMenu.Count == 1)
                        languageId = GetLanguageBasedOnRouteAndFlight(allDistinctMenu[i]);

                    if (menuTypeId == 5 || menuTypeId == 6 || menuTypeId == 7)
                        languageId = 1;

                    var menuId = _menuManagement.AddMenu(menuname, menuCode, menuTypeId, userId, cycleId, languageId);

                    menuIdCollection.Add(menuId);

                    SetRoutesForMenu(allDistinctMenu[i], menuId);

                    UploadMenuItems(allDistinctMenu[i], menuId, importedRows);
                }

                catch (Exception e)
                {
                    //write exception Elma
                    ErrorSignal.FromCurrentContext().Raise(e);
                }
            }

        }

        public static string GenerateMenuCode(int menuTypeId, string menuCode)
        {
            var definingChar = "D";

            //Upper Class  Afternoon Tea card
            if (menuTypeId == 2)
                definingChar = "A";
            //Upper Class Main Menu
            if (menuTypeId == 1)
                definingChar = "C";
            //Upper Class Breakfast card
            if (menuTypeId == 3)
                definingChar = "B";
            //Upper Class Wine cards
            if (menuTypeId == 4)
                definingChar = "W";
            //Upper Class Food Guide
            if (menuTypeId == 5)
                definingChar = "D";

            //Premium Economy Main Menu
            if (menuTypeId == 10)
                definingChar = "Y";
            //Economy Main Menu
            if (menuTypeId == 13)
                definingChar = "T";

            //Upper Class Allergen guides
            if (menuTypeId == 13)
                definingChar = "X";

            //Upper Class Special meals
            if (menuTypeId == 13)
                definingChar = "S";


            menuCode = Helper.Get3Digits() + definingChar + Helper.Get3Digits();
            return menuCode;
        }


        /// <summary>
        /// SET ALL THE ROUTES FOR THE MENU
        /// </summary>
        /// <param name="allDistinctMenu"></param>
        /// <param name="menuIdCollection"></param>
        private void SetRoutesForMenu(string allRoute, long menuId)
        {
            try
            {
                var routeCollection = allRoute.Split(new char[] { ',' });

                for (int index = 0; index < routeCollection.Length; index++)
                {
                    var routename = routeCollection[index];
                    var start = routename.IndexOf("(") + 1;
                    var length = routename.IndexOf(")") - routename.IndexOf("(") - 1;
                    var flightNo = routename.Substring(0, start - 1);

                    flightNo = flightNo.Trim();
                    routename = routename.Substring(start, length);
                    routename = routename.Replace(" ", "");

                    var ends = routename.Split(new char[] { '-' });
                    var departure = ends[0];
                    var arrival = ends[1];

                    var routeId = _routeManagement.GetRouteId(departure, arrival);

                    if (routeId > 0)
                    {
                        if (!flightNo.Contains("VS"))
                            flightNo = flightNo.Replace("V", "VS");

                        _menuManagement.AddRouteForMenu(menuId, routeId, flightNo);
                    }
                }
            }
            catch (Exception ex)
            {
                //write to Elma
                ErrorSignal.FromCurrentContext().Raise(ex);
            }
        }


        /// <summary>
        /// language based menus are not created, we need to create them and assign the routes as well
        /// DEL LHR - VS300 & VS301 Hindi; HKG LHR - VS207 & VS 206 TRAD CHN; PVG LHR VS251 & VS 250 SIM CHN
        /// </summary>
        /// <param name="route"></param>
        /// <returns></returns>
        private int GetLanguageBasedOnRouteAndFlight(string route)
        {
            int languageId = 1;

            try
            {
                if (route.Contains("DEL") && route.Contains("LHR") && (route.Contains("300") || route.Contains("301")))
                    languageId = 2;

                if (route.Contains("HKG") && route.Contains("LHR") && (route.Contains("207") || route.Contains("206")))
                    languageId = 4;

                if (route.Contains("PVG") && route.Contains("LHR") && (route.Contains("250") || route.Contains("251")))
                    languageId = 3;
            }
            catch (Exception ex)
            {
                //write to Elma
                ErrorSignal.FromCurrentContext().Raise(ex);
            }
            return languageId;
        }


        /// <summary>
        /// Uplaod menu items, only upload items for BRD, STA, ENT, DES and TEA
        /// </summary>
        /// <param name="allDistinctMenu"></param>
        /// <param name="menuIdCollection"></param>
        /// <param name="importedRows"></param>
        private void UploadMenuItems(string allDistinctMenu, long menuId, IEnumerable<Dictionary<string, string>> importedRows)
        {
            try
            {
                var menu = _menuManagement.GetMenuById(menuId);

                //split the first route and get all the menu items for this route column while looping in rows
                var allRoutes = allDistinctMenu.Split(new char[] { ',' });

                var firstRoute = allRoutes[0];
                firstRoute = firstRoute.ToLower();

                var menuTypeId = _menuManagement.GetMenuById(menuId).MenuTypeId;

                //for Main Menu - find the right version of main menu chili template
                if (menuTypeId == 1)
                {
                    bool isHavingTEA = false;
                    bool isHavingBRK = false;
                    bool isHavingExtraBite = false;
                    bool isHavingSNK = false;

                    foreach (var row in importedRows)
                    {
                        if (row["service"] == "TEA")
                        {
                            var menuItemVolume = Convert.ToString(row[firstRoute]);

                            if (menuItemVolume != "NV" && menuItemVolume.Contains("%"))
                            {
                                isHavingTEA = true;
                                break;
                            }
                        }
                    }


                    foreach (var row in importedRows)
                    {
                        if (row["service"] == "SNK")
                        {
                            var menuItemVolume = Convert.ToString(row[firstRoute]);

                            if (menuItemVolume != "NV" /*&& menuItemVolume.Contains("%")*/)
                            {
                                isHavingSNK = true;
                                break;
                            }
                        }
                    }

                    foreach (var row in importedRows)
                    {
                        if (row["service"] == "BRK")
                        {
                            var menuItemVolume = Convert.ToString(row[firstRoute]);

                            if (menuItemVolume != "NV" && menuItemVolume.Contains("%"))
                            {
                                isHavingBRK = true;
                                break;
                            }
                        }
                    }

                    foreach (var row in importedRows)
                    {
                        if (row["service"] == "GRAZE")
                        {
                            var menuItemVolume = Convert.ToString(row[firstRoute]);

                            if (menuItemVolume != "NV" && menuItemVolume.Contains("%"))
                            {
                                isHavingExtraBite = true;
                                break;
                            }
                        }

                    }

                    //TODO: update the menutemplate based on ishavingTea  isHavingBrk and isHavingExtraBite
                    //add template in database, hard code the below method params

                    if (!isHavingBRK && !isHavingTEA && isHavingExtraBite)
                    {
                        if (menu.LanguageId == 1)
                            _menuManagement.UpdateMenuTemplate(menuId, 1);

                        //TODO: set the template in database and uncomment the below code
                        if (menu.LanguageId == 2)
                            _menuManagement.UpdateMenuTemplate(menuId, 2);

                        if (menu.LanguageId == 3)
                            _menuManagement.UpdateMenuTemplate(menuId, 3);

                        if (menu.LanguageId == 4)
                            _menuManagement.UpdateMenuTemplate(menuId, 4);
                    }

                    if (isHavingBRK && !isHavingTEA && isHavingExtraBite)
                    {
                        if (menu.LanguageId == 1)
                            _menuManagement.UpdateMenuTemplate(menuId, 49);

                        if (menu.LanguageId == 2)
                            _menuManagement.UpdateMenuTemplate(menuId, 55);

                        if (menu.LanguageId == 3)
                            _menuManagement.UpdateMenuTemplate(menuId, 58);

                        if (menu.LanguageId == 4)
                            _menuManagement.UpdateMenuTemplate(menuId, 61);
                    }

                    if (isHavingBRK && !isHavingTEA && !isHavingExtraBite)
                    {
                        if (menu.LanguageId == 1)
                            _menuManagement.UpdateMenuTemplate(menuId, 50);

                        if (menu.LanguageId == 2)
                            _menuManagement.UpdateMenuTemplate(menuId, 56);

                        if (menu.LanguageId == 3)
                            _menuManagement.UpdateMenuTemplate(menuId, 59);

                        if (menu.LanguageId == 4)
                            _menuManagement.UpdateMenuTemplate(menuId, 62);
                    }

                    if (!isHavingBRK && isHavingTEA && !isHavingExtraBite)
                    {
                        if (menu.LanguageId == 1)
                            _menuManagement.UpdateMenuTemplate(menuId, 51);

                        if (menu.LanguageId == 2)
                            _menuManagement.UpdateMenuTemplate(menuId, 57);

                        if (menu.LanguageId == 3)
                            _menuManagement.UpdateMenuTemplate(menuId, 60);

                        if (menu.LanguageId == 4)
                            _menuManagement.UpdateMenuTemplate(menuId, 63);
                    }
                    if (!isHavingBRK && isHavingTEA && isHavingExtraBite)
                    {
                        if (menu.LanguageId == 1)
                            _menuManagement.UpdateMenuTemplate(menuId, 52);

                        //if (menu.LanguageId == 2)
                        //    _menuManagement.UpdateMenuTemplate(menuId, 53);

                        //if (menu.LanguageId == 3)
                        //    _menuManagement.UpdateMenuTemplate(menuId, 53);

                        //if (menu.LanguageId == 4)
                        //    _menuManagement.UpdateMenuTemplate(menuId, 53);
                    }

                    if (isHavingSNK && isHavingBRK)
                    {
                        if (menu.LanguageId == 1)
                            _menuManagement.UpdateMenuTemplate(menuId, 53);
                    }
                }

                foreach (var row in importedRows)
                {
                    //MAIN COURSE
                    if (menuTypeId == 1 || menuTypeId == 10 || menuTypeId == 13 || menuTypeId == 5 || menuTypeId == 6)
                    {

                        if (menu.LanguageId == 1)
                        {
                            if (row["service"] == "SNK" || row["service"] == "BRD" || row["service"] == "STA" || row["service"] == "ENT" || row["service"] == "DES" || row["service"] == "CHE" || row["service"] == "GRAZE")
                            {
                                var menuItemVolume = Convert.ToString(row[firstRoute]);

                                //for BRD, CHE consider the item even without %
                                if (row["service"] == "BRD" || row["service"] == "CHE" || row["service"] == "SNK")
                                {
                                    if (menuItemVolume != "NV")
                                    {
                                        var baseItemCode = Convert.ToString(row["service code"]);

                                        if (!string.IsNullOrEmpty(baseItemCode))
                                            _menuManagement.AddMenuItem(menuId, baseItemCode);
                                    }
                                }
                                else
                                {
                                    if (menuItemVolume != "NV" && menuItemVolume.Contains("%"))
                                    {
                                        var baseItemCode = Convert.ToString(row["service code"]);

                                        if (!string.IsNullOrEmpty(baseItemCode))
                                            _menuManagement.AddMenuItem(menuId, baseItemCode);
                                    }
                                }
                            }
                        }
                        else
                        {

                            //for language based menu, BRK, TEA are present in menu only so mark there as manu items
                            if (row["service"] == "BRK" || row["service"] == "TEA" || row["service"] == "BRD" || row["service"] == "STA" || row["service"] == "ENT" || row["service"] == "DES" || row["service"] == "CHE" || row["service"] == "GRAZE")
                            {
                                var menuItemVolume = Convert.ToString(row[firstRoute]);

                                //for BRD, CHE consider the item even without %
                                if (row["service"] == "BRD" || row["service"] == "CHE")
                                {
                                    if (menuItemVolume != "NV")
                                    {
                                        var baseItemCode = Convert.ToString(row["service code"]);

                                        if (!string.IsNullOrEmpty(baseItemCode))
                                            _menuManagement.AddMenuItem(menuId, baseItemCode);
                                    }
                                }
                                else
                                {
                                    if (menuItemVolume != "NV" && menuItemVolume.Contains("%"))
                                    {
                                        var baseItemCode = Convert.ToString(row["service code"]);

                                        if (!string.IsNullOrEmpty(baseItemCode))
                                            _menuManagement.AddMenuItem(menuId, baseItemCode);
                                    }
                                }
                            }
                        }
                    }


                    //This is tea menu
                    //TEA
                    if (menuTypeId == 2)
                    {
                        if (row["service"] == "TEA")
                        {
                            var menuItemVolume = Convert.ToString(row[firstRoute]);

                            if (menuItemVolume != "NV" && menuItemVolume.Contains("%"))
                            {
                                var baseItemCode = Convert.ToString(row["service code"]);

                                if (!string.IsNullOrEmpty(baseItemCode))
                                    _menuManagement.AddMenuItem(menuId, baseItemCode);
                            }
                        }
                    }

                    //BREAKFAST
                    if (menuTypeId == 3)
                    {
                        if (row["service"] == "BRK")
                        {
                            var menuItemVolume = Convert.ToString(row[firstRoute]);

                            if (menuItemVolume != "NV" && menuItemVolume.Contains("%"))
                            {
                                var baseItemCode = Convert.ToString(row["service code"]);

                                if (!string.IsNullOrEmpty(baseItemCode))
                                    _menuManagement.AddMenuItem(menuId, baseItemCode);
                            }
                        }
                    }

                    //OTHER Menu types are fixed so no need to save their menu items
                }
            }
            catch (Exception ex)
            {
                //write to Elma
                ErrorSignal.FromCurrentContext().Raise(ex);
            }

        }


        #endregion

        #region ChiliProcessing

        public void CreateChiliVariableCollectionAndApply(List<long> menuIdCollection)
        {

            for (int i = 0; i < menuIdCollection.Count; i++)
            {
                try
                {
                    CreateChiliVariableCollectionAndApplyForMenuId(menuIdCollection[i]);
                }
                catch (Exception e)
                {
                    //write to Elmah
                    ErrorSignal.FromCurrentContext().Raise(e);
                }
            }
        }


        public void CreateChiliVariableCollectionAndApplyForMenuId(long menuId)
        {
            try
            {
                var menu = _menuManagement.GetMenuById(menuId);
                var languageId = menu.LanguageId;

                Dictionary<string, string> values = new Dictionary<string, string>();

                BuildMenuCodeChiliVariable(menu, values);


                //build the chili variable collection

                var baseItemCollection = _menuManagement.GetAllMenuItems(menuId);

                List<string> distinctCategory = new List<string>();

                foreach (var baseItem in baseItemCollection)
                {
                    if (!distinctCategory.Contains(baseItem.CategoryCode))
                        distinctCategory.Add(baseItem.CategoryCode);
                }


                for (int iCount = 0; iCount < distinctCategory.Count; iCount++)
                {
                    var category = distinctCategory[iCount];

                    //get from database for that sertvice code

                    var menuItemCategory = _menuManagement.GetMenuItemCategory(category, 1);

                    //ENGLISH
                    if (menuItemCategory != null)
                    {
                        //ApplyLanguageBasedCategoryChiliVariable(values, category, menuItemCategory.CategoryName, menuItemCategory.Description, "");
                        ApplyLanguageBasedItemChiliVariable(values, baseItemCollection, category, "");
                    }

                    //HINDI
                    menuItemCategory = _menuManagement.GetMenuItemCategory(category, Convert.ToInt32(languageId));

                    if (languageId == 2)
                    {
                        ApplyLanguageBasedItemChiliVariable(values, baseItemCollection, category, "HIN_");

                        //if (menuItemCategory != null)
                        //    ApplyLanguageBasedCategoryChiliVariable(values, category, menuItemCategory.CategoryName, menuItemCategory.Description, "HIN_");
                    }

                    //SIMPLIFIED CHINESE 
                    if (languageId == 3)
                    {
                        ApplyLanguageBasedItemChiliVariable(values, baseItemCollection, category, "SIMCHN_");

                        //if (menuItemCategory != null)
                        //    ApplyLanguageBasedCategoryChiliVariable(values, category, menuItemCategory.CategoryName, menuItemCategory.Description, "SIMCHN_");
                    }

                    //TRADITIONAL CHINESE 
                    if (languageId == 4)
                    {
                        ApplyLanguageBasedItemChiliVariable(values, baseItemCollection, category, "TRADCHN_");

                        //if (menuItemCategory != null)
                        //    ApplyLanguageBasedCategoryChiliVariable(values, category, menuItemCategory.CategoryName, menuItemCategory.Description, "TRADCHN_");
                    }

                }

                try
                {
                    HandleNotAvailbleCategoryInServicePlan(values, distinctCategory, Convert.ToInt32(languageId));
                }
                catch (Exception e)
                {
                    //write to Elma
                    ErrorSignal.FromCurrentContext().Raise(e);
                }


                try
                {
                    chili.UpdateChiliDocumentVariables(InstanceId, menuId, values);
                }
                catch (Exception e)
                {
                    //write to Elma
                    ErrorSignal.FromCurrentContext().Raise(e);
                }
            }
            catch (Exception e)
            {
                //write to Elma
                ErrorSignal.FromCurrentContext().Raise(e);
            }
        }

        //just add the category and call the method HideMissingCategoryItems
        private static void HandleNotAvailbleCategoryInServicePlan(Dictionary<string, string> values, List<string> distinctCategory, int languageId)
        {
            try
            {
                HideMissingCategoryItems(values, distinctCategory, languageId, "BRD");
                HideMissingCategoryItems(values, distinctCategory, languageId, "CHE");
                HideMissingCategoryItems(values, distinctCategory, languageId, "GRAZE");
            }
            catch (Exception ex)
            {
                //write to Elma
                ErrorSignal.FromCurrentContext().Raise(ex);
            }

        }

        /// <summary>
        /// Hide three items for each missing category
        /// </summary>
        /// <param name="values"></param>
        /// <param name="distinctCategory"></param>
        /// <param name="languageId"></param>
        /// <param name="categoryCode"></param>
        private static void HideMissingCategoryItems(Dictionary<string, string> values, List<string> distinctCategory, int languageId, string categoryCode)
        {
            try
            {
                bool isPresent = false;

                if (distinctCategory.Contains(categoryCode))
                    isPresent = true;
                else
                    isPresent = false;

                if (!isPresent)
                {
                    for (int i = 1; i <= 3; i++)
                    {
                        var languagePrefix = "";
                        var category = categoryCode;
                        var counter = i;
                        var itemtitle = languagePrefix + category + "_ITEMTITLE1." + counter;
                        var itemsubtitle = languagePrefix + category + "_ITEMSUBTITLE1." + counter;
                        var itemdesc = languagePrefix + category + "_ITEMDESC1." + counter;
                        var itemsubdesc = languagePrefix + category + "_ITEMSUBDESC1." + counter;
                        var itemsattr = languagePrefix + category + "_ITEMATT1." + counter;

                        values.Add(itemtitle, "");
                        values.Add(itemsubtitle, "");
                        values.Add(itemdesc, "");
                        values.Add(itemsubdesc, "");
                        values.Add(itemsattr, "");
                    }

                }

                if (languageId == 2)
                {
                    var languagePrefix = "HIN_";

                    if (!isPresent)
                    {
                        for (int i = 1; i <= 3; i++)
                        {
                            var category = categoryCode;
                            var counter = i;
                            var itemtitle = languagePrefix + category + "_ITEMTITLE1." + counter;
                            var itemsubtitle = languagePrefix + category + "_ITEMSUBTITLE1." + counter;
                            var itemdesc = languagePrefix + category + "_ITEMDESC1." + counter;
                            var itemsubdesc = languagePrefix + category + "_ITEMSUBDESC1." + counter;
                            var itemsattr = languagePrefix + category + "_ITEMATT1." + counter;

                            values.Add(itemtitle, "");
                            values.Add(itemsubtitle, "");
                            values.Add(itemdesc, "");
                            values.Add(itemsubdesc, "");
                            values.Add(itemsattr, "");
                        }

                    }
                }

                if (languageId == 3)
                {
                    var languagePrefix = "SIMCHN_";

                    if (!isPresent)
                    {
                        for (int i = 1; i <= 3; i++)
                        {
                            var category = categoryCode;
                            var counter = i;
                            var itemtitle = languagePrefix + category + "_ITEMTITLE1." + counter;
                            var itemsubtitle = languagePrefix + category + "_ITEMSUBTITLE1." + counter;
                            var itemdesc = languagePrefix + category + "_ITEMDESC1." + counter;
                            var itemsubdesc = languagePrefix + category + "_ITEMSUBDESC1." + counter;
                            var itemsattr = languagePrefix + category + "_ITEMATT1." + counter;

                            values.Add(itemtitle, "");
                            values.Add(itemsubtitle, "");
                            values.Add(itemdesc, "");
                            values.Add(itemsubdesc, "");
                            values.Add(itemsattr, "");
                        }

                    }
                }

                if (languageId == 4)
                {
                    var languagePrefix = "TRADCHN_";

                    if (!isPresent)
                    {
                        for (int i = 1; i <= 3; i++)
                        {
                            var category = categoryCode;
                            var counter = i;
                            var itemtitle = languagePrefix + category + "_ITEMTITLE1." + counter;
                            var itemsubtitle = languagePrefix + category + "_ITEMSUBTITLE1." + counter;
                            var itemdesc = languagePrefix + category + "_ITEMDESC1." + counter;
                            var itemsubdesc = languagePrefix + category + "_ITEMSUBDESC1." + counter;
                            var itemsattr = languagePrefix + category + "_ITEMATT1." + counter;

                            values.Add(itemtitle, "");
                            values.Add(itemsubtitle, "");
                            values.Add(itemdesc, "");
                            values.Add(itemsubdesc, "");
                            values.Add(itemsattr, "");
                        }

                    }
                }
            }
            catch (Exception e)
            {
                //write to Elma
                ErrorSignal.FromCurrentContext().Raise(e);
            }
        }

        private void BuildMenuCodeChiliVariable(DataAccess.Model.MenuData menu, Dictionary<string, string> values)
        {

            try
            {
                var cycle = _cycleManagement.GetCycle(Convert.ToInt64(menu.CycleId));

                var menuNamePart = menu.MenuName.Split(new char[] { '/' });

                var flights = "";
                for (int icount = 1; icount < menuNamePart.Length; icount++)
                {
                    flights += menuNamePart[icount];

                    if (icount != menuNamePart.Length - 1)
                        flights += ", ";
                }

                values.Add("FLIGHTNUM", flights);
                values.Add("CYCLE", cycle.CycleName);
                values.Add("LOTNUM", menu.MenuCode);
            }
            catch (Exception e)
            {
                //write to Elma
                ErrorSignal.FromCurrentContext().Raise(e);
            }
        }

        private static void ApplyLanguageBasedCategoryChiliVariable(Dictionary<string, string> values, string category, string categorytitle, string categorydescription, string languagePrefix)
        {
            try
            {
                var cateogyTitleVariable = languagePrefix + category + "_CATEGORY_TITLE";
                var cateogyDescVariable = languagePrefix + category + "_CATEGORY_DESC";

                values.Add(cateogyTitleVariable, categorytitle);
                values.Add(cateogyDescVariable, categorydescription);
            }
            catch (Exception ex)
            {
                //write to Elma
                ErrorSignal.FromCurrentContext().Raise(ex);
            }
        }

        private static void ApplyLanguageBasedItemChiliVariable(Dictionary<string, string> values, List<DataAccess.Model.BaseItem> baseItemCollection, string category, string languagePrefix)
        {
            try
            {
                int counter = 1;
                foreach (var baseItem in baseItemCollection)
                {
                    if (baseItem.CategoryCode == category)
                    {
                        var itemtitle = languagePrefix + category + "_ITEMTITLE1." + counter;
                        var itemsubtitle = languagePrefix + category + "_ITEMSUBTITLE1." + counter;
                        var itemdesc = languagePrefix + category + "_ITEMDESC1." + counter;
                        var itemsubdesc = languagePrefix + category + "_ITEMSUBDESC1." + counter;
                        var itemsattr = languagePrefix + category + "_ITEMATT1." + counter;

                        values.Add(itemtitle, baseItem.BaseItemTitle);
                        values.Add(itemsubtitle, baseItem.BaseItemTitleDescription);
                        values.Add(itemdesc, baseItem.BaseItemDescription);
                        values.Add(itemsubdesc, baseItem.BaseItemSubDescription);
                        values.Add(itemsattr, baseItem.BaseItemAttributes);

                        counter++;
                    }
                }
            }
            catch (Exception e)
            {
                //write to Elma
                ErrorSignal.FromCurrentContext().Raise(e);
            }
        }

        private void CreateChiliDocumentForMenu(List<long> menuIdCollection)
        {
            for (int i = 0; i < menuIdCollection.Count; i++)
            {
                try
                {
                    CreateChiliDocumentForMenuId(menuIdCollection[i]);
                }
                catch (Exception e)
                {
                    //write to Elma
                    ErrorSignal.FromCurrentContext().Raise(e);
                }
            }
        }

        public void CreateChiliDocumentForMenuId(long menuId)
        {
            var menu = _menuManagement.GetMenuById(menuId);
            //var template = _menuManagement.GetTemplate(Convert.ToInt32(menu.MenuTypeId), Convert.ToInt32(menu.LanguageId));

            var menuTemplate = _menuManagement.GetMenuTemplate(menuId);

            try
            {
                var chilidoc = chili.CreateChiliDocumentForMenu(InstanceId, menuId, menuTemplate.TemplateID, Convert.ToString(menuId));

                _menuManagement.UpdateMenuTemplate(menuId, menuTemplate.TemplateID, chilidoc);

            }
            catch (Exception e)
            {
                //write to Elma
                ErrorSignal.FromCurrentContext().Raise(e);
            }
        }


        public void RebuildFlightNumberLotNumberChiliVariableForMenu(long menuId)
        {
            try
            {
                UpdateLotNoChiliVariable(menuId);
            }
            catch (Exception e)
            {
                //write to Elma
                ErrorSignal.FromCurrentContext().Raise(e);
            }

        }

        public void UpdateLotNoChiliVariable(long menuid)
        {
            try
            {
                var menu = _menuManagement.GetMenuById(menuid);
                Dictionary<string, string> values = new Dictionary<string, string>();
                values.Add("LOTNUM", menu.MenuCode);

                var menuNamePart = menu.MenuName.Split(new char[] { '/' });

                var flights = "";
                for (int icount = 1; icount < menuNamePart.Length; icount++)
                {
                    flights += menuNamePart[icount];

                    if (icount != menuNamePart.Length - 1)
                        flights += ", ";
                }

                values.Add("FLIGHTNUM", flights);

                var cycle = _cycleManagement.GetCycle(Convert.ToInt64(menu.CycleId));

                values.Add("CYCLE", cycle.CycleName);

                chili.UpdateChiliDocumentVariables(InstanceId, menuid, values);
            }
            catch { }

        }


        public void RebuildChiliDocumentForMenu(long menuId)
        {
            //Below code is not needed as template id assigned earlier is correct, jsut needs rebuilding
            /*
            bool isHavingBRK = false;
            bool isHavingTEA = false;
            bool isHavingExtraBite = false;

            var baseItemCollection = _menuManagement.GetAllMenuItems(menuId);

            foreach (var baseItem in baseItemCollection)
            {
                if (baseItem.CategoryCode == "BRK")
                    isHavingBRK = true;

                if (baseItem.CategoryCode == "TEA")
                    isHavingTEA = true;

                if (baseItem.CategoryCode == "GRAZE")
                    isHavingExtraBite = true;
            }

            if (!isHavingBRK && !isHavingTEA && isHavingExtraBite)
                _menuManagement.UpdateMenuTemplate(menuId, 49, "83d73ec8-7371-472e-a7a3-d1d25f6bc9fa");

            if (isHavingBRK && !isHavingTEA && isHavingExtraBite)
                _menuManagement.UpdateMenuTemplate(menuId, 50, "6f44757e-c299-4787-ba95-c350dea51747");

            if (isHavingBRK && !isHavingTEA && !isHavingExtraBite)
                _menuManagement.UpdateMenuTemplate(menuId, 51, "bc0c2b5f-5227-4bff-883a-6bc038ba1af3");

            if (!isHavingBRK && isHavingTEA && !isHavingExtraBite)
                _menuManagement.UpdateMenuTemplate(menuId, 52, "81c9e6a3-c7bc-4003-81b7-d755db57bf50");

            if (!isHavingBRK && isHavingTEA && isHavingExtraBite)
                _menuManagement.UpdateMenuTemplate(menuId, 53, "8c245578-48db-4d66-be38-af9298c928a7");

            */
            try
            {
                CreateChiliDocumentForMenuId(menuId);

                CreateChiliVariableCollectionAndApplyForMenuId(menuId);
            }
            catch (Exception e)
            {
                //write to Elma
                ErrorSignal.FromCurrentContext().Raise(e);
            }

        }
        public void CreateChiliDocumentForReOrderMenu(List<long> menuIdCollection)
        {
            for (int i = 0; i < menuIdCollection.Count; i++)
            {
                var menuId = menuIdCollection[i];

                var template = _menuManagement.GetMenuTemplate(menuId);

                try
                {
                    var chilidoc = chili.CreateChiliDocumentForReOrderMenu(InstanceId, menuId, template.ChiliDocumentID, Convert.ToString(menuId));

                    _menuManagement.UpdateMenuTemplate(menuId, template.TemplateID, chilidoc);

                    var menu = _menuManagement.GetMenuById(menuId);

                    Dictionary<string, string> values = new Dictionary<string, string>();
                    BuildMenuCodeChiliVariable(menu, values);

                    chili.UpdateChiliDocumentVariables(InstanceId, menuId, values);

                }
                catch (Exception e)
                {
                    //write to Elma
                    ErrorSignal.FromCurrentContext().Raise(e);
                }
            }
        }
        #endregion

        #region Generate PDF

        public void GeneratePdfForMenu(long cycleId, long menuId, long orderId, string chiliDocId)
        {
            if (orderId <= 0)
                return;


            pdfInputDestination = EmmaPDFPathFolder + @"\MENU PDFS";


            try
            {
                if (!Directory.Exists(pdfInputDestination + @"\" + orderId))
                    Directory.CreateDirectory(pdfInputDestination + @"\" + orderId);

            }
            catch { }



            var menu = _menuManagement.GetMenuById(menuId);


            var menuTemplate = _menuManagement.GetMenuTemplate(menuId);
            chili.AddPdfGenerationTaskForMenuTemplate(menuTemplate, chiliDocId);

            Generate(cycleId, PdfGenerationJobId, menuTemplate.MenuID, menuTemplate.TemplateID, Convert.ToInt32(menu.LanguageId));

            try
            {
                var fileName = GetPdfFileName(cycleId, menu.Id, menuTemplate.TemplateID, Convert.ToInt32(menu.LanguageId));
                var outfilePath = System.IO.Path.Combine(pdfInputDestination + @"\" + orderId + @"\" + "Upper Class Main Menu", fileName);

                try
                {
                    if (!Directory.Exists(pdfInputDestination + @"\" + orderId + @"\" + "Upper Class Main Menu"))
                        Directory.CreateDirectory(pdfInputDestination + @"\" + orderId + @"\" + "Upper Class Main Menu");

                }
                catch { }


                //copy the file to orderId PDF folder
                File.Copy(EmmaPDFPathFolder + @"\MENU PDFS\" + fileName, outfilePath, true);


                try
                {

                    using (new NetworkConnection(pathServerMenuPDFs, new NetworkCredential(UserName, Password, domain)))
                    {
                        File.Copy(EmmaPDFPathFolder + @"\MENU PDFS\" + fileName, pathServerMenuPDFs + @"\\" + fileName, true);
                    }
                }
                catch { }

                File.Delete(EmmaPDFPathFolder + @"\MENU PDFS\" + fileName);

            }
            catch { }

        }


        public void GeneratePdfForOrder(long cycleId, int classId, int menutypeId, long orderId)
        {
            try
            {
                if (orderId <= 0)
                    return;


                pdfInputDestination = EmmaPDFPathFolder + @"\MENU PDFS";


                try
                {
                    if (!Directory.Exists(pdfInputDestination + @"\" + orderId))
                        Directory.CreateDirectory(pdfInputDestination + @"\" + orderId);

                }
                catch { }

                var menuTypes = _menuManagement.GetMenuTypeByClass(classId);


                foreach (var menuType in menuTypes)
                {

                    if (menuType.ID != menutypeId)
                        continue;

                    var menus = _menuManagement.GetMenuByCycleClassAndMenutype(cycleId, classId, menuType.ID).ToList();

                    foreach (var menu in menus)
                    {
                        try
                        {
                            //menu is there in order or not?
                            var liveOrderDetails = _orderManagement.GetOrderDetailsbyOrderId(orderId);

                            bool menuFound = false;
                            foreach (var details in liveOrderDetails)
                            {
                                if (details != null && details.MenuCode == menu.MenuCode)
                                {
                                    menuFound = true;
                                    break;
                                }
                            }

                            //Process Fodo Guide even outside order
                            if (menuType.ID != 5)
                            {
                                if (!menuFound)
                                    continue;
                            }

                            var menuTemplate = _menuManagement.GetMenuTemplate(menu.Id);
                            //chili.AddPdfGenerationTaskForMenuTemplate(menuTemplate);

                            RebuildFlightNumberLotNumberChiliVariableForMenu(menu.Id);


                            var newDOc = chili.CloneChiliDocument(menu.Id);

                            chili.UpdateChiliDocumentVariablesallowServerRendering(1, menu.Id, newDOc);

                            chili.AddPdfGenerationTaskForMenuTemplate(menuTemplate, newDOc);

                            Generate(cycleId, PdfGenerationJobId, menuTemplate.MenuID, menuTemplate.TemplateID, Convert.ToInt32(menu.LanguageId));

                            //translated files are uploaded manually so copy this to order PDF folder
                            //if (menu.LanguageId != 1)
                            {
                                try
                                {
                                    var fileName = GetPdfFileName(cycleId, menu.Id, menuTemplate.TemplateID, Convert.ToInt32(menu.LanguageId));
                                    var outfilePath = System.IO.Path.Combine(pdfInputDestination + @"\" + orderId + @"\" + menuType.MenuType, fileName);

                                    try
                                    {
                                        if (!Directory.Exists(pdfInputDestination + @"\" + orderId + @"\" + menuType.MenuType))
                                            Directory.CreateDirectory(pdfInputDestination + @"\" + orderId + @"\" + menuType.MenuType);

                                    }
                                    catch { }


                                    //copy the file to orderId PDF folder
                                    File.Copy(EmmaPDFPathFolder + @"\MENU PDFS\" + fileName, outfilePath, true);

                                    //copy to web2print Emma folder as well for crew member site display

                                    try
                                    {

                                        using (new NetworkConnection(pathServerMenuPDFs, new NetworkCredential(UserName, Password, domain)))
                                        {
                                            File.Copy(EmmaPDFPathFolder + @"\MENU PDFS\" + fileName, pathServerMenuPDFs + @"\\" + fileName, true);
                                        }
                                    }
                                    catch { }


                                    File.Delete(EmmaPDFPathFolder + @"\MENU PDFS\" + fileName);

                                }
                                catch { }
                            }
                        }
                        catch
                        {
                            //log the error
                        }
                    }
                }


            }
            catch
            {
                //failed
            }
        }


        public void GeneratePdfForOrder(long cycleId, int classId, long orderId)
        {
            try
            {
                if (orderId <= 0)
                    return;


                pdfInputDestination = EmmaPDFPathFolder + @"\MENU PDFS";


                try
                {
                    if (!Directory.Exists(pdfInputDestination + @"\" + orderId))
                        Directory.CreateDirectory(pdfInputDestination + @"\" + orderId);

                }
                catch { }

                var menuTypes = _menuManagement.GetMenuTypeByClass(classId);


                foreach (var menuType in menuTypes)
                {
                    var menus = _menuManagement.GetMenuByCycleClassAndMenutype(cycleId, classId, menuType.ID).ToList();

                    foreach (var menu in menus)
                    {
                        try
                        {
                            //menu is there in order or not?
                            var liveOrderDetails = _orderManagement.GetOrderDetailsbyOrderId(orderId);

                            bool menuFound = false;
                            foreach (var details in liveOrderDetails)
                            {
                                if (details != null && details.MenuCode == menu.MenuCode)
                                {
                                    menuFound = true;
                                    break;
                                }
                            }

                            if (!menuFound)
                                continue;

                            var menuTemplate = _menuManagement.GetMenuTemplate(menu.Id);
                            chili.AddPdfGenerationTaskForMenuTemplate(menuTemplate);

                            Generate(cycleId, PdfGenerationJobId, menuTemplate.MenuID, menuTemplate.TemplateID, Convert.ToInt32(menu.LanguageId));

                            //translated files are uploaded manually so copy this to order PDF folder
                            //if (menu.LanguageId != 1)
                            {
                                try
                                {
                                    var fileName = GetPdfFileName(cycleId, menu.Id, menuTemplate.TemplateID, Convert.ToInt32(menu.LanguageId));
                                    var outfilePath = System.IO.Path.Combine(pdfInputDestination + @"\" + orderId + @"\" + menuType.MenuType, fileName);

                                    try
                                    {
                                        if (!Directory.Exists(pdfInputDestination + @"\" + orderId + @"\" + menuType.MenuType))
                                            Directory.CreateDirectory(pdfInputDestination + @"\" + orderId + @"\" + menuType.MenuType);

                                    }
                                    catch { }


                                    //copy the file to orderId PDF folder
                                    File.Copy(EmmaPDFPathFolder + @"\MENU PDFS\" + fileName, outfilePath, true);



                                    try
                                    {

                                        using (new NetworkConnection(pathServerMenuPDFs, new NetworkCredential(UserName, Password, domain)))
                                        {
                                            File.Copy(EmmaPDFPathFolder + @"\MENU PDFS\" + fileName, pathServerMenuPDFs + @"\\" + fileName, true);
                                        }
                                    }
                                    catch { }

                                    File.Delete(EmmaPDFPathFolder + @"\MENU PDFS\" + fileName);

                                }
                                catch { }
                            }
                        }
                        catch
                        {
                            //log the error
                        }
                    }
                }


            }
            catch
            {
                //failed
            }
        }


        /// <summary>
        /// MenutypeId =-1 means generate PDF for all menu types
        /// </summary>
        /// <param name="cycleId"></param>
        /// <param name="classId"></param>
        /// <param name="menuTypeId"></param>
        /// <param name="routeId"></param>
        /// 

        public void GeneratePdf(long cycleId, int classId, int menuTypeId, long routeId)
        {
            try
            {
                var recentOderDetails = _orderManagement.GetRecentOrderDetails();

                //var directory = Directory.GetParent(HttpRuntime.AppDomainAppPath);
                //var parentDir = directory.Parent;

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
                                    chili.AddPdfGenerationTaskForMenuTemplate(menuTemplate);
                                    var process = _orderManagement.IsMenuPresentInRecentOrderDetails(menu.Id, recentOderDetails);

                                    if (process)
                                        Generate(cycleId, PdfGenerationJobId, menuTemplate.MenuID, menuTemplate.TemplateID, Convert.ToInt32(menu.LanguageId));
                                }
                                catch
                                { }
                            }
                        }
                    }
                    else
                    {
                        foreach (var menu in menus)
                        {
                            try
                            {
                                var menuTemplate = _menuManagement.GetMenuTemplate(menu.Id);
                                chili.AddPdfGenerationTaskForMenuTemplate(menuTemplate);
                                //var process =  _orderManagement.IsMenuPresentInRecentOrderDetails(menu.Id, recentOderDetails);

                                //if (process)
                                Generate(cycleId, PdfGenerationJobId, menuTemplate.MenuID, menuTemplate.TemplateID, Convert.ToInt32(menu.LanguageId));
                            }
                            catch
                            { }
                        }
                    }
                }
                else
                {
                    var menuTypes = _menuManagement.GetMenuTypeByClass(classId);

                    foreach (var menuType in menuTypes)
                    {
                        var menus = _menuManagement.GetMenuByCycleClassAndMenutype(cycleId, classId, menuType.ID).ToList();

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
                                        chili.AddPdfGenerationTaskForMenuTemplate(menuTemplate);
                                        var process = _orderManagement.IsMenuPresentInRecentOrderDetails(menu.Id, recentOderDetails);

                                        if (process)
                                            Generate(cycleId, PdfGenerationJobId, menuTemplate.MenuID, menuTemplate.TemplateID, Convert.ToInt32(menu.LanguageId));
                                    }
                                    catch
                                    { }
                                }
                            }

                        }
                        else
                        {
                            foreach (var menu in menus)
                            {
                                try
                                {
                                    var menuTemplate = _menuManagement.GetMenuTemplate(menu.Id);
                                    chili.AddPdfGenerationTaskForMenuTemplate(menuTemplate);
                                    var process = _orderManagement.IsMenuPresentInRecentOrderDetails(menu.Id, recentOderDetails);

                                    if (process)
                                        Generate(cycleId, PdfGenerationJobId, menuTemplate.MenuID, menuTemplate.TemplateID, Convert.ToInt32(menu.LanguageId));

                                }
                                catch
                                { }
                            }
                        }
                    }

                }

            }
            catch
            { }
        }


        public void Generate(long cycleId, int jobID, long menuID, int templateID, int languageId)
        {
            //DO NOT DO PDF GENERATION FOR LANGUAGE OTHER THAN ENGLISH AS IT IS NOT SUPPORTED BY CHILI
            //if (languageId != 1)
            //  return;

            chili.Connect("VAA");

            var fileName = GetPdfFileName(cycleId, menuID, templateID, languageId);


            var filePath = System.IO.Path.Combine(pdfInputDestination, fileName);

            bool exists = System.IO.Directory.Exists(pdfInputDestination);

            int counter = 0;


            var task = chili.GetPdfGenerationTask(jobID, menuID);



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
                                //counter++;
                                task.Status = "TaskNotReady";
                                task.ChiliError = null;

                                //if (counter == 100)
                                //    loop=false;
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
                                chili.UpdateTask(task);

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

                    chili.UpdateTask(task);
                } while (loop);


            }
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

        public List<string> GeneratePdfFileNameForDownload(long cycleId, int classId, int menuTypeId, long routeId)
        {
            List<string> pdfFileNames = new List<string>();

            try
            {
                var recentOderDetails = _orderManagement.GetRecentOrderDetails();
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
                            var process = _orderManagement.IsMenuPresentInRecentOrderDetails(menu.Id, recentOderDetails);

                            if (!process)
                                continue;

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

                                    var filename = GetPdfFileName(cycleId, menu.Id, Convert.ToInt32(menuTemplate.TemplateID), Convert.ToInt32(menu.LanguageId));
                                    if (!pdfFileNames.Contains(filename))
                                        pdfFileNames.Add(filename);
                                }
                                catch
                                { }
                            }


                        }

                    }

                }


            }
            catch
            { }
            return pdfFileNames;
        }

        public string GeneratePdfFileNameForDownloadByMenuId(long menuId)
        {
            try
            {
                try
                {
                    var menu = _menuManagement.GetMenuById(menuId);

                    var menuTemplate = _menuManagement.GetMenuTemplate(menu.Id);
                    return GetPdfFileName(Convert.ToInt64(menu.CycleId), menu.Id, Convert.ToInt32(menuTemplate.TemplateID), Convert.ToInt32(menu.LanguageId));
                }
                catch
                {
                    //log the error
                }
            }
            catch
            {
                //failed
            }
            return "";

        }
        public void DownloadPdf(long cycleId, int classId, int menuTypeId, long routeId)
        {
            throw new System.NotImplementedException();
        }

        #endregion

        #region Quantity


        public void CalculateQuantity(long liveOrderId, DateTime fromDate, DateTime toDate)
        {
            //get liveorderdetails

            try
            {
                var liveOrderDetails = _orderManagement.GetLiveOrderDetails(liveOrderId);

                //loop through each liveorder details
                foreach (var orderDetails in liveOrderDetails)
                {
                    //for each flight number look for tflightschedule, there would be more than 1 records and get the equipment type
                    var flightNum = orderDetails.FlightNo;

                    if (!flightNum.Contains("VS"))
                        flightNum = flightNum.Replace("V", "VS");

                    var flightScheduleWednesday = _routeManagement.GetScheduleWednesday(flightNum, fromDate);
                    var flightScheduleThursday = _routeManagement.GetScheduleThursday(flightNum, fromDate.AddDays(1));
                    var flightScheduleFriday = _routeManagement.GetScheduleFriday(flightNum, fromDate.AddDays(2));
                    var flightScheduleSaturday = _routeManagement.GetScheduleSaturday(flightNum, fromDate.AddDays(3));
                    var flightScheduleSunday = _routeManagement.GetScheduleSunday(flightNum, fromDate.AddDays(4));
                    var flightScheduleMonday = _routeManagement.GetScheduleMonday(flightNum, fromDate.AddDays(5));
                    var flightScheduleTuesday = _routeManagement.GetScheduleTuesday(flightNum, fromDate.AddDays(6));

                    //also see effective and discontinued dates
                    //loop thgourh the flight no records and see the days on which operatre to get the total day count
                    bool isWednesday = flightScheduleWednesday != null;
                    bool isThursday = flightScheduleThursday != null;
                    bool isFriday = flightScheduleFriday != null;
                    bool isSaturday = flightScheduleSaturday != null;
                    bool isSunday = flightScheduleSunday != null;
                    bool isMonday = flightScheduleMonday != null;
                    bool isTuesday = flightScheduleTuesday != null;

                    var menu = _menuManagement.GetMenuById(Convert.ToInt64(orderDetails.MenuId));


                    var classId = _menuManagement.GetMenuClass(Convert.ToInt32(menu.MenuTypeId));

                    //look for tseatconfiguration for that equipment type
                    int totalQuantity = 0;

                    if (classId == 1)
                    {
                        if (isWednesday)
                            totalQuantity += _routeManagement.UpperClassCapacity(flightScheduleWednesday.EquipmentType);

                        if (isThursday)
                            totalQuantity += _routeManagement.UpperClassCapacity(flightScheduleThursday.EquipmentType);

                        if (isFriday)
                            totalQuantity += _routeManagement.UpperClassCapacity(flightScheduleFriday.EquipmentType);

                        if (isSaturday)
                            totalQuantity += _routeManagement.UpperClassCapacity(flightScheduleSaturday.EquipmentType);

                        if (isSunday)
                            totalQuantity += _routeManagement.UpperClassCapacity(flightScheduleSunday.EquipmentType);

                        if (isMonday)
                            totalQuantity += _routeManagement.UpperClassCapacity(flightScheduleMonday.EquipmentType);

                        if (isTuesday)
                            totalQuantity += _routeManagement.UpperClassCapacity(flightScheduleTuesday.EquipmentType);
                    }
                    if (classId == 2)
                    {
                        if (isWednesday)
                            totalQuantity += _routeManagement.PremiumEcoClassCapacity(flightScheduleWednesday.EquipmentType);

                        if (isThursday)
                            totalQuantity += _routeManagement.PremiumEcoClassCapacity(flightScheduleThursday.EquipmentType);

                        if (isFriday)
                            totalQuantity += _routeManagement.PremiumEcoClassCapacity(flightScheduleFriday.EquipmentType);

                        if (isSaturday)
                            totalQuantity += _routeManagement.PremiumEcoClassCapacity(flightScheduleSaturday.EquipmentType);

                        if (isSunday)
                            totalQuantity += _routeManagement.PremiumEcoClassCapacity(flightScheduleSunday.EquipmentType);

                        if (isMonday)
                            totalQuantity += _routeManagement.PremiumEcoClassCapacity(flightScheduleMonday.EquipmentType);

                        if (isTuesday)
                            totalQuantity += _routeManagement.PremiumEcoClassCapacity(flightScheduleTuesday.EquipmentType);
                    }
                    if (classId == 3)
                    {
                        if (isWednesday)
                            totalQuantity += _routeManagement.EcoClassCapacity(flightScheduleWednesday.EquipmentType);

                        if (isThursday)
                            totalQuantity += _routeManagement.EcoClassCapacity(flightScheduleThursday.EquipmentType);

                        if (isFriday)
                            totalQuantity += _routeManagement.EcoClassCapacity(flightScheduleFriday.EquipmentType);

                        if (isSaturday)
                            totalQuantity += _routeManagement.EcoClassCapacity(flightScheduleSaturday.EquipmentType);

                        if (isSunday)
                            totalQuantity += _routeManagement.EcoClassCapacity(flightScheduleSunday.EquipmentType);

                        if (isMonday)
                            totalQuantity += _routeManagement.EcoClassCapacity(flightScheduleMonday.EquipmentType);

                        if (isTuesday)
                            totalQuantity += _routeManagement.EcoClassCapacity(flightScheduleTuesday.EquipmentType);
                    }

                    //get the capacity
                    _orderManagement.UpdateQuantity(orderDetails.ID, totalQuantity);
                }
            }
            catch (Exception ex)
            {
                //write to Elma
                ErrorSignal.FromCurrentContext().Raise(ex);
            }

        }


        #endregion

    }
}

