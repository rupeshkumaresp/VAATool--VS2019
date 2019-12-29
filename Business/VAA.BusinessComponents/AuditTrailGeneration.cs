using System.Web;
using Elmah;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VAA.DataAccess;
using VAA.CommonComponents;
using VAA.Entities.VAAEntity;

namespace VAA.BusinessComponents
{
    /// <summary>
    /// Create Audit Trail report for menus
    /// </summary>
    public class AuditTrailGeneration
    {
        readonly CycleManagement _cycleManagement = new CycleManagement();
        readonly MenuManagement _menuManagement = new MenuManagement();
        readonly RouteManagement _routeManagement = new RouteManagement();
        readonly AccountManagement _accountManagement = new AccountManagement();

        public void GeneratePrintStausAudit(long cycleId, int classId, int menuTypeId)
        {
            try
            {
                var cycle = _cycleManagement.GetCycle(cycleId);
                // Set up columns
                var headerColumns = BuilderPrintStatusAuditHeader();
                var className = _menuManagement.GetClassShortName(classId);

                if (menuTypeId == -1)
                {
                    //all the menu - show each menu type as worksheet tab

                    using (var package = new ExcelPackage())
                    {
                        var menuTypes = _menuManagement.GetMenuTypeByClass(classId);

                        foreach (var menutype in menuTypes)
                        {
                            BuildPrintStatusAuditForMenuType(menutype, package, headerColumns, cycleId, classId);

                        }
                        // Save file and return stream
                        var fileName = Path.GetTempFileName();
                        package.SaveAs(new FileInfo(fileName));

                        var stream = new FileStream(fileName, FileMode.Open);

                        var directory = Directory.GetParent(HttpRuntime.AppDomainAppPath);
                        var parentDir = directory.Parent;


                        var path = parentDir.FullName + @"\PrintStatusReport\" + cycle.CycleName + "_" + className + "_PrintStatus.xlsx";
                        Helper.SaveStreamToFile(path, stream);

                    }
                }
                else
                {
                    using (var package = new ExcelPackage())
                    {
                        var menuType = _menuManagement.GetMenuTypeById(menuTypeId);
                        BuildPrintStatusAuditForMenuType(menuType, package, headerColumns, cycleId, classId);

                        // Save file and return stream
                        var fileName = Path.GetTempFileName();
                        package.SaveAs(new FileInfo(fileName));

                        var stream = new FileStream(fileName, FileMode.Open);

                        var directory = Directory.GetParent(HttpRuntime.AppDomainAppPath);
                        var parentDir = directory.Parent;

                        var path = parentDir.FullName + @"\PrintStatusReport\" + cycle.CycleName + "_" + className + "_" + menuType.DisplayName + "_PrintStatus.xlsx";
                        Helper.SaveStreamToFile(path, stream);

                    }
                }
            }
            catch (Exception ex)
            {
                //write to Elma
                ErrorSignal.FromCurrentContext().Raise(ex);
            }
        }

        public void GenerateAuditTrail(long cycleId, int classId, int menuTypeId, string departure, string arrival)
        {
            try
            {
                var cycle = _cycleManagement.GetCycle(cycleId);
                // Set up columns
                var headerColumns = BuilderAuditHeader();
                var className = _menuManagement.GetClassShortName(classId);

                if (menuTypeId == -1)
                {
                    //all the menu - show each menu type as worksheet tab

                    if (arrival == "All" && departure == "All")
                    {
                        using (var package = new ExcelPackage())
                        {
                            var menuTypes = _menuManagement.GetMenuTypeByClass(classId);

                            foreach (var menutype in menuTypes)
                            {
                                BuildAuditForMenuType(menutype, package, headerColumns, cycleId, classId);

                            }
                            // Save file and return stream
                            var fileName = Path.GetTempFileName();
                            package.SaveAs(new FileInfo(fileName));

                            var stream = new FileStream(fileName, FileMode.Open);

                            var directory = Directory.GetParent(HttpRuntime.AppDomainAppPath);
                            var parentDir = directory.Parent;


                            var path = parentDir.FullName + @"\Audit\" + cycle.CycleName + "_" + className + "_Audit_AllRoutes.xlsx";
                            Helper.SaveStreamToFile(path, stream);

                        }
                    }
                    else
                    {
                        //rotue specific

                        if (arrival != "All" && departure != "All")
                        {

                            var routeId = _routeManagement.GetRouteId(departure, arrival);

                            using (var package = new ExcelPackage())
                            {
                                var menuTypes = _menuManagement.GetMenuTypeByClass(classId);

                                foreach (var menutype in menuTypes)
                                {
                                    BuildAuditForMenuTypeRouteBased(menutype, package, headerColumns, cycleId, classId, routeId);

                                }
                                // Save file and return stream
                                var fileName = Path.GetTempFileName();
                                package.SaveAs(new FileInfo(fileName));

                                var stream = new FileStream(fileName, FileMode.Open);

                                var directory = Directory.GetParent(HttpRuntime.AppDomainAppPath);
                                var parentDir = directory.Parent;


                                var path = parentDir.FullName + @"\Audit\" + cycle.CycleName + "_" + className + "_" + departure + "_" + arrival + "_Audit.xlsx";
                                Helper.SaveStreamToFile(path, stream);

                            }
                        }
                        else
                        {
                            //handle route where one location is all
                        }

                    }
                }
                else
                {
                    if (arrival == "All" && departure == "All")
                    {
                        using (var package = new ExcelPackage())
                        {
                            var menuType = _menuManagement.GetMenuTypeById(menuTypeId);
                            BuildAuditForMenuType(menuType, package, headerColumns, cycleId, classId);

                            // Save file and return stream
                            var fileName = Path.GetTempFileName();
                            package.SaveAs(new FileInfo(fileName));

                            var stream = new FileStream(fileName, FileMode.Open);

                            var directory = Directory.GetParent(HttpRuntime.AppDomainAppPath);
                            var parentDir = directory.Parent;

                            var path = parentDir.FullName + @"\Audit\" + cycle.CycleName + "_" + className + "_" + menuType.DisplayName + "_Audit_AllRoutes.xlsx";
                            Helper.SaveStreamToFile(path, stream);

                        }
                    }
                    else
                    {
                        //rotue specific

                        if (arrival != "All" && departure != "All")
                        {

                            var routeId = _routeManagement.GetRouteId(departure, arrival);

                            using (var package = new ExcelPackage())
                            {
                                var menuType = _menuManagement.GetMenuTypeById(menuTypeId);
                                BuildAuditForMenuTypeRouteBased(menuType, package, headerColumns, cycleId, classId, routeId);

                                // Save file and return stream
                                var fileName = Path.GetTempFileName();
                                package.SaveAs(new FileInfo(fileName));

                                var stream = new FileStream(fileName, FileMode.Open);

                                var directory = Directory.GetParent(HttpRuntime.AppDomainAppPath);
                                var parentDir = directory.Parent;

                                var path = parentDir.FullName + @"\Audit\" + cycle.CycleName + "_" + className + "_" + menuType.DisplayName + "_" + departure + "_" + arrival + "_Audit.xlsx";

                                Helper.SaveStreamToFile(path, stream);

                            }

                        }
                        else
                        {
                            //handle route where one location is all
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                //write to Elma
                ErrorSignal.FromCurrentContext().Raise(ex);
            }
        }

        private void BuildAuditForMenuTypeRouteBased(tMenuType menutype, ExcelPackage package, Dictionary<string, int> headerColumns, long cycleId, int classId, long routeId)
        {
            try
            {
                var menuTypeName = menutype.DisplayName;

                var worksheet = package.Workbook.Worksheets.Add(menuTypeName);

                // Write column headers
                foreach (var colKvp in headerColumns)
                {
                    if (colKvp.Value > 0)
                    {
                        worksheet.Cells[1, colKvp.Value].Value = colKvp.Key;
                        worksheet.Cells[1, colKvp.Value].Style.Font.Bold = true;
                        worksheet.Cells[1, colKvp.Value].AutoFitColumns();
                        worksheet.Cells[1, colKvp.Value].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells[1, colKvp.Value].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        worksheet.Cells[1, colKvp.Value].Style.Border.BorderAround(
                         OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                    }
                }

                // Write records
                var menus = _menuManagement.GetMenuByCycleClassAndMenutype(cycleId, classId, menutype.ID);

                int y = 2;
                int rowCount = 0;

                foreach (var menu in menus)
                {
                    var menunameParts = menu.MenuName.Split(new char[] { '/' });

                    var flights = "";

                    for (int i = 1; i < menunameParts.Length; i++)
                    {
                        flights += menunameParts[i];

                        if (i != menunameParts.Length - 1)
                            flights += ",";
                    }

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

                    if (!isRoute)
                        continue;


                    var menuHistory = _menuManagement.GetMenuHistory(menu.Id);

                    if (menuHistory == null)
                        continue;

                    foreach (var history in menuHistory)
                    {
                        worksheet.Cells[y + rowCount, headerColumns["MenuCode"]].Value = menu.MenuCode;
                        worksheet.Cells[y + rowCount, headerColumns["MenuCode"]].Style.WrapText = true;
                        worksheet.Cells[y + rowCount, headerColumns["MenuCode"]].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells[y + rowCount, headerColumns["MenuCode"]].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        worksheet.Cells[y + rowCount, headerColumns["MenuCode"]].Style.Border.BorderAround(
                         OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                        worksheet.Cells[y + rowCount, headerColumns["MenuName"]].Value = menunameParts[0];
                        worksheet.Cells[y + rowCount, headerColumns["MenuName"]].Style.WrapText = true;
                        worksheet.Cells[y + rowCount, headerColumns["MenuName"]].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells[y + rowCount, headerColumns["MenuName"]].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        worksheet.Cells[y + rowCount, headerColumns["MenuName"]].Style.Border.BorderAround(
                         OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                        worksheet.Cells[y + rowCount, headerColumns["Flights"]].Value = flights;
                        worksheet.Cells[y + rowCount, headerColumns["Flights"]].Style.WrapText = true;
                        worksheet.Cells[y + rowCount, headerColumns["Flights"]].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells[y + rowCount, headerColumns["Flights"]].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        worksheet.Cells[y + rowCount, headerColumns["Flights"]].Style.Border.BorderAround(
                         OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                        worksheet.Cells[y + rowCount, headerColumns["ActionTaken"]].Value = history.ActionTaken;
                        worksheet.Cells[y + rowCount, headerColumns["ActionTaken"]].Style.WrapText = true;
                        worksheet.Cells[y + rowCount, headerColumns["ActionTaken"]].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells[y + rowCount, headerColumns["ActionTaken"]].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        worksheet.Cells[y + rowCount, headerColumns["ActionTaken"]].Style.Border.BorderAround(
                         OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                        worksheet.Cells[y + rowCount, headerColumns["ModifiedAt"]].Value = Convert.ToDateTime(history.ModifiedAt).ToShortDateString();
                        worksheet.Cells[y + rowCount, headerColumns["ModifiedAt"]].Style.WrapText = true;
                        worksheet.Cells[y + rowCount, headerColumns["ModifiedAt"]].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells[y + rowCount, headerColumns["ModifiedAt"]].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        worksheet.Cells[y + rowCount, headerColumns["ModifiedAt"]].Style.Border.BorderAround(
                         OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                        var user = _accountManagement.GetUserById(Convert.ToInt32(history.ActionByUserID));
                        worksheet.Cells[y + rowCount, headerColumns["ActionByUser"]].Value = user.FirstName + " " + user.LastName;
                        worksheet.Cells[y + rowCount, headerColumns["ActionByUser"]].Style.WrapText = true;
                        worksheet.Cells[y + rowCount, headerColumns["ActionByUser"]].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells[y + rowCount, headerColumns["ActionByUser"]].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        worksheet.Cells[y + rowCount, headerColumns["ActionByUser"]].Style.Border.BorderAround(
                         OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                        rowCount++;
                    }
                }

                worksheet.Column(1).Width = 15;
                worksheet.Column(2).Width = 40;
                worksheet.Column(3).Width = 70;
                worksheet.Column(4).Width = 25;
                worksheet.Column(5).Width = 35;
                worksheet.Column(6).Width = 25;
            }
            catch (Exception ex)
            {
                //write to Elma
                ErrorSignal.FromCurrentContext().Raise(ex);
            }
        }

        private void BuildPrintStatusAuditForMenuType(tMenuType menutype, ExcelPackage package, Dictionary<string, int> headerColumns, long cycleId, int classId)
        {
            try
            {
                var menuTypeName = menutype.DisplayName;

                var worksheet = package.Workbook.Worksheets.Add(menuTypeName);

                // Write column headers
                foreach (var colKvp in headerColumns)
                {
                    if (colKvp.Value > 0)
                    {
                        worksheet.Cells[1, colKvp.Value].Value = colKvp.Key;
                        worksheet.Cells[1, colKvp.Value].Style.Font.Bold = true;
                        worksheet.Cells[1, colKvp.Value].AutoFitColumns();
                        worksheet.Cells[1, colKvp.Value].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells[1, colKvp.Value].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        worksheet.Cells[1, colKvp.Value].Style.Border.BorderAround(
                          OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                    }
                }

                // Write records
                var menus = _menuManagement.GetMenuByCycleClassAndMenutype(cycleId, classId, menutype.ID);

                int y = 2;
                int rowCount = 0;

                foreach (var menu in menus)
                {
                    var menuDetails = _menuManagement.GetMenuById(menu.Id);

                    var menunameParts = menuDetails.MenuName.Split(new char[] { '/' });

                    var flights = "";

                    for (int i = 1; i < menunameParts.Length; i++)
                    {
                        flights += menunameParts[i];

                        if (i != menunameParts.Length - 1)
                            flights += ",";
                    }
                    worksheet.Cells[y + rowCount, headerColumns["MenuCode"]].Value = menuDetails.MenuCode;
                    worksheet.Cells[y + rowCount, headerColumns["MenuCode"]].Style.WrapText = true;
                    worksheet.Cells[y + rowCount, headerColumns["MenuCode"]].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    worksheet.Cells[y + rowCount, headerColumns["MenuCode"]].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    worksheet.Cells[y + rowCount, headerColumns["MenuCode"]].Style.Border.BorderAround(
                         OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                    worksheet.Cells[y + rowCount, headerColumns["MenuName"]].Value = menunameParts[0];
                    worksheet.Cells[y + rowCount, headerColumns["MenuName"]].Style.WrapText = true;
                    worksheet.Cells[y + rowCount, headerColumns["MenuName"]].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    worksheet.Cells[y + rowCount, headerColumns["MenuName"]].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    worksheet.Cells[y + rowCount, headerColumns["MenuName"]].Style.Border.BorderAround(
                         OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                    worksheet.Cells[y + rowCount, headerColumns["Flights"]].Value = flights;
                    worksheet.Cells[y + rowCount, headerColumns["Flights"]].Style.WrapText = true;
                    worksheet.Cells[y + rowCount, headerColumns["Flights"]].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    worksheet.Cells[y + rowCount, headerColumns["Flights"]].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    worksheet.Cells[y + rowCount, headerColumns["Flights"]].Style.Border.BorderAround(
                         OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                    worksheet.Cells[y + rowCount, headerColumns["CurrentStatus"]].Value = menuDetails.ApprovalStatusName;
                    worksheet.Cells[y + rowCount, headerColumns["CurrentStatus"]].Style.WrapText = true;
                    worksheet.Cells[y + rowCount, headerColumns["CurrentStatus"]].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    worksheet.Cells[y + rowCount, headerColumns["CurrentStatus"]].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    worksheet.Cells[y + rowCount, headerColumns["CurrentStatus"]].Style.Border.BorderAround(
                         OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                    rowCount++;
                }

                worksheet.Column(1).Width = 15;
                worksheet.Column(2).Width = 40;
                worksheet.Column(3).Width = 70;
                worksheet.Column(4).Width = 25;
            }
            catch (Exception ex)
            {
                //write to Elma
                ErrorSignal.FromCurrentContext().Raise(ex);
            }
        }

        private void BuildAuditForMenuType(tMenuType menutype, ExcelPackage package, Dictionary<string, int> headerColumns, long cycleId, int classId)
        {
            try
            {
                var menuTypeName = menutype.DisplayName;

                var worksheet = package.Workbook.Worksheets.Add(menuTypeName);

                // Write column headers
                foreach (var colKvp in headerColumns)
                {
                    if (colKvp.Value > 0)
                    {
                        worksheet.Cells[1, colKvp.Value].Value = colKvp.Key;
                        worksheet.Cells[1, colKvp.Value].Style.Font.Bold = true;
                        worksheet.Cells[1, colKvp.Value].AutoFitColumns();
                        worksheet.Cells[1, colKvp.Value].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells[1, colKvp.Value].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        worksheet.Cells[1, colKvp.Value].Style.Border.BorderAround(
                         OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                    }
                }



                // Write records
                var menus = _menuManagement.GetMenuByCycleClassAndMenutype(cycleId, classId, menutype.ID);

                int y = 2;
                int rowCount = 0;

                foreach (var menu in menus)
                {
                    var menunameParts = menu.MenuName.Split(new char[] { '/' });

                    var flights = "";

                    for (int i = 1; i < menunameParts.Length; i++)
                    {
                        flights += menunameParts[i];

                        if (i != menunameParts.Length - 1)
                            flights += ",";
                    }

                    var menuHistory = _menuManagement.GetMenuHistory(menu.Id);

                    if (menuHistory == null)
                        continue;

                    foreach (var history in menuHistory)
                    {
                        worksheet.Cells[y + rowCount, headerColumns["MenuCode"]].Value = menu.MenuCode;
                        worksheet.Cells[y + rowCount, headerColumns["MenuCode"]].Style.WrapText = true;
                        worksheet.Cells[y + rowCount, headerColumns["MenuCode"]].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells[y + rowCount, headerColumns["MenuCode"]].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        worksheet.Cells[y + rowCount, headerColumns["MenuCode"]].Style.Border.BorderAround(
                         OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                        worksheet.Cells[y + rowCount, headerColumns["MenuName"]].Value = menunameParts[0];
                        worksheet.Cells[y + rowCount, headerColumns["MenuName"]].Style.WrapText = true;
                        worksheet.Cells[y + rowCount, headerColumns["MenuName"]].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells[y + rowCount, headerColumns["MenuName"]].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        worksheet.Cells[y + rowCount, headerColumns["MenuName"]].Style.Border.BorderAround(
                         OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                        worksheet.Cells[y + rowCount, headerColumns["Flights"]].Value = flights;
                        worksheet.Cells[y + rowCount, headerColumns["Flights"]].Style.WrapText = true;
                        worksheet.Cells[y + rowCount, headerColumns["Flights"]].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells[y + rowCount, headerColumns["Flights"]].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        worksheet.Cells[y + rowCount, headerColumns["Flights"]].Style.Border.BorderAround(
                         OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                        worksheet.Cells[y + rowCount, headerColumns["ActionTaken"]].Value = history.ActionTaken;
                        worksheet.Cells[y + rowCount, headerColumns["ActionTaken"]].Style.WrapText = true;
                        worksheet.Cells[y + rowCount, headerColumns["ActionTaken"]].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells[y + rowCount, headerColumns["ActionTaken"]].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        worksheet.Cells[y + rowCount, headerColumns["ActionTaken"]].Style.Border.BorderAround(
                         OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                        worksheet.Cells[y + rowCount, headerColumns["ModifiedAt"]].Value = Convert.ToDateTime(history.ModifiedAt).ToShortDateString();
                        worksheet.Cells[y + rowCount, headerColumns["ModifiedAt"]].Style.WrapText = true;
                        worksheet.Cells[y + rowCount, headerColumns["ModifiedAt"]].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells[y + rowCount, headerColumns["ModifiedAt"]].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        worksheet.Cells[y + rowCount, headerColumns["ModifiedAt"]].Style.Border.BorderAround(
                         OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                        var user = _accountManagement.GetUserById(Convert.ToInt32(history.ActionByUserID));
                        worksheet.Cells[y + rowCount, headerColumns["ActionByUser"]].Value = user.FirstName + " " + user.LastName;
                        worksheet.Cells[y + rowCount, headerColumns["ActionByUser"]].Style.WrapText = true;
                        worksheet.Cells[y + rowCount, headerColumns["ActionByUser"]].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells[y + rowCount, headerColumns["ActionByUser"]].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        worksheet.Cells[y + rowCount, headerColumns["ActionByUser"]].Style.Border.BorderAround(
                         OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                        rowCount++;
                    }
                }

                worksheet.Column(1).Width = 15;
                worksheet.Column(2).Width = 40;
                worksheet.Column(3).Width = 70;
                worksheet.Column(4).Width = 25;
                worksheet.Column(5).Width = 35;
                worksheet.Column(6).Width = 25;
            }
            catch (Exception ex)
            {
                //write to Elma
                ErrorSignal.FromCurrentContext().Raise(ex);
            }
        }

        private Dictionary<string, int> BuilderAuditHeader()
        {
            var headerColumns = new Dictionary<string, int>();
            headerColumns.Add("Cycle", 1);

            int x = 1;

            headerColumns.Add("MenuCode", x);
            x++;

            headerColumns.Add("MenuName", x);
            x++;

            headerColumns.Add("Flights", x);
            x++;


            headerColumns.Add("ModifiedAt", x);
            x++;

            headerColumns.Add("ActionTaken", x);
            x++;

            headerColumns.Add("ActionByUser", x);
            x++;
            return headerColumns;
        }

        private Dictionary<string, int> BuilderPrintStatusAuditHeader()
        {
            var headerColumns = new Dictionary<string, int>();
            headerColumns.Add("Cycle", 1);

            int x = 1;

            headerColumns.Add("MenuCode", x);
            x++;

            headerColumns.Add("MenuName", x);
            x++;

            headerColumns.Add("Flights", x);
            x++;

            headerColumns.Add("CurrentStatus", x);
            x++;

            return headerColumns;
        }

    }
}
