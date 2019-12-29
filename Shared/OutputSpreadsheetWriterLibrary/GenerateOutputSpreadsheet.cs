using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OfficeOpenXml;
using VAA.Entities.VAAEntity;
using System.Diagnostics;

namespace OutputSpreadsheetWriterLibrary
{
    public class PrintRunData
    {
        public string MenuPDFFile;
        public string MenuFlight;
        public int MenuCount;
        public string Bundles;
        public int MenuClassId;
        public bool isTranslated6pp;
    }


    public class FlightScheduledata
    {
        public string FltNo;
        public string Origin;
        public string Dept;
        public string Dest;
        public string Arrival;
        public string M;
        public string Tu;
        public string W;
        public string Th;
        public string F;
        public string Sa;
        public string Su;
        public string EquipmentType;
        public string Effective;
        public string Discontinue;

    }



    /// <summary>
    ///generate excel output for Print run and Box ticket
    /// </summary>
    public class GenerateOutputSpreadsheet
    {

        public static void CreateFlightScheduleSpreadSheet(List<FlightScheduledata> data, string flightSchedulePath)
        {
            //// Set up columns
            var headerColumns = new Dictionary<string, int>();

            ////First validate all the columns in the xml file is present in spreadsheet or not?
            int icount = 1;
            BuildFlightScheduleHeader(headerColumns, icount);

            // Export data
            using (var package = new ExcelPackage())
            {
                var worksheetName = "Flight Schedule";
                var worksheet = package.Workbook.Worksheets.Add(worksheetName);
                flightScheduleReportWorksheetPreparation(worksheet, headerColumns, data);


                // Save file and return stream
                var fileName = Path.GetTempFileName();
                package.SaveAs(new FileInfo(fileName));

                var path = flightSchedulePath + @"\FlightSchedule" + DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss") + ".xlsx";

                SaveStreamToFile(path, new FileStream(fileName, FileMode.Open));

                Process.Start(path);

            }
        }
        public static void CreatePrintRunSpreadSheet(Dictionary<string, PrintRunData> data, long orderId, string lotNo, string printRunPath)
        {
            //// Set up columns
            var headerColumns = new Dictionary<string, int>();

            ////First validate all the columns in the xml file is present in spreadsheet or not?
            int icount = 1;
            BuildPrintRunHeader(headerColumns, icount);

            // Export data
            using (var package = new ExcelPackage())
            {
                var worksheetName = "UC MM";
                var searchString = "upper class menu";
                var worksheet = package.Workbook.Worksheets.Add(worksheetName);
                PrintRunReportWorksheetPreparation(worksheet, orderId, lotNo, headerColumns, data, searchString);

                worksheetName = "UC TEA";
                searchString = "upper class afternoon tea";
                worksheet = package.Workbook.Worksheets.Add(worksheetName);
                PrintRunReportWorksheetPreparation(worksheet, orderId, lotNo, headerColumns, data, searchString);

                worksheetName = "UC BRK";
                searchString = "upper class breakfast";
                worksheet = package.Workbook.Worksheets.Add(worksheetName);
                PrintRunReportWorksheetPreparation(worksheet, orderId, lotNo, headerColumns, data, searchString);

                worksheetName = "PREM EC";
                searchString = "premium economy main menu";
                worksheet = package.Workbook.Worksheets.Add(worksheetName);
                PrintRunReportWorksheetPreparation(worksheet, orderId, lotNo, headerColumns, data, searchString);

                worksheetName = "ECON";
                searchString = "economy main menu";
                worksheet = package.Workbook.Worksheets.Add(worksheetName);
                PrintRunReportWorksheetPreparation(worksheet, orderId, lotNo, headerColumns, data, searchString);

                // Save file and return stream
                var fileName = Path.GetTempFileName();
                package.SaveAs(new FileInfo(fileName));

                var path = printRunPath + @"\PrintRun_" + orderId + ".xlsx";

                SaveStreamToFile(path, new FileStream(fileName, FileMode.Open));
            }
        }

        private static void PrintRunReportWorksheetPreparation(ExcelWorksheet worksheet, long orderId, string lotNo, Dictionary<string, int> headerColumns, Dictionary<string, PrintRunData> data, string searchString)
        {
            worksheet.Cells[1, 1].Value = "Order ID";
            worksheet.Cells[1, 1].Style.Font.Bold = true;
            worksheet.Cells[1, 2].Value = orderId;

            worksheet.Cells[1, 2].Style.HorizontalAlignment =
                OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            worksheet.Cells[1, 2].Style.VerticalAlignment =
                OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

            worksheet.Cells[2, 1].Value = "LOT NO";
            worksheet.Cells[2, 1].Style.Font.Bold = true;
            worksheet.Cells[2, 2].Value = lotNo;
            worksheet.Cells[2, 2].Style.HorizontalAlignment =
                OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            worksheet.Cells[2, 2].Style.VerticalAlignment =
                OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

            // Write column headers
            foreach (var colKvp in headerColumns)
            {
                if (colKvp.Value > 0)
                {
                    worksheet.Cells[4, colKvp.Value].Style.Border.BorderAround(
                        OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                    worksheet.Cells[4, colKvp.Value].Value = colKvp.Key;

                    worksheet.Cells[4, colKvp.Value].AutoFitColumns();

                    worksheet.Cells[4, colKvp.Value].Style.Font.Bold = true;
                    worksheet.Cells[4, colKvp.Value].AutoFitColumns();
                    worksheet.Cells[4, colKvp.Value].Style.HorizontalAlignment =
                        OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    worksheet.Cells[4, colKvp.Value].Style.VerticalAlignment =
                        OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                }
            }

            int y = 5;

            foreach (var key in data.Keys)
            {
                if (searchString == "economy main menu")
                {
                    if (!key.ToLower().Contains(searchString))
                        continue;

                    if (key.ToLower().Contains("premium"))
                        continue;
                }
                else
                {

                    if (!key.ToLower().Contains(searchString))
                        continue;
                }

                var printData = data[key];

                if (printData.isTranslated6pp)
                    continue;

                var menuDetailsArray = key.Split(new char[] { '-' });

                worksheet.Cells[y, 1].Value = key;
                if (menuDetailsArray.Length >= 2)
                    worksheet.Cells[y, 1].Value = menuDetailsArray[1];



                worksheet.Cells[y, 2].Value = printData.MenuFlight;
                worksheet.Cells[y, 3].Value = printData.MenuCount;

                var bundles = printData.Bundles;
                var bundleStr = "";

                //split based on commoa

                var bundleArray = bundles.Split(new char[] { ',' });
                Dictionary<int, int> bundleDict = new Dictionary<int, int>();

                for (int i = 0; i < bundleArray.Length; i++)
                {
                    if (string.IsNullOrEmpty(bundleArray[i]))
                        continue;

                    if (string.IsNullOrEmpty(bundleArray[i].Trim()))
                        continue;

                    var bundledata = bundleArray[i].Trim();
                    bundledata = bundledata.Replace("of", "");

                    var bundledataArray = bundledata.Split(new char[] { ' ' });

                    if (!bundleDict.ContainsKey(Convert.ToInt32(bundledataArray[1])))
                    {
                        bundleDict.Add(Convert.ToInt32(bundledataArray[1]), Convert.ToInt32(bundledataArray[0]));
                    }
                    else
                    {
                        var keyDict = Convert.ToInt32(bundledataArray[1]);
                        bundleDict[keyDict] += Convert.ToInt32(bundledataArray[0]);
                    }
                }

                foreach (var dicKey in bundleDict.Keys)
                {
                    bundleStr += bundleDict[dicKey] + " of " + dicKey + " / ";
                }

                worksheet.Cells[y, 4].Value = bundleStr;

                worksheet.Cells[y, 1].Style.Border.BorderAround(
                    OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                worksheet.Cells[y, 2].Style.Border.BorderAround(
                    OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                worksheet.Cells[y, 3].Style.Border.BorderAround(
                    OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                worksheet.Cells[y, 4].Style.Border.BorderAround(
                    OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                y++;
            }

            y++;

            if (searchString.Contains("economy main menu"))
            {
                worksheet.Cells[y, 1].Value = "Translated 6pp";
                worksheet.Cells[y, 1].Style.Font.Bold = true;
            }
            y++;

            foreach (var key in data.Keys)
            {
                if (searchString == "economy main menu")
                {
                    if (!key.ToLower().Contains(searchString))
                        continue;

                    if (key.ToLower().Contains("premium"))
                        continue;
                }
                else
                {

                    if (!key.ToLower().Contains(searchString))
                        continue;
                }

                var printData = data[key];

                if (!printData.isTranslated6pp)
                    continue;

                var menuDetailsArray = key.Split(new char[] { '-' });

                worksheet.Cells[y, 1].Value = key;
                if (menuDetailsArray.Length >= 2)
                    worksheet.Cells[y, 1].Value = menuDetailsArray[1];



                worksheet.Cells[y, 2].Value = printData.MenuFlight;
                worksheet.Cells[y, 3].Value = printData.MenuCount;

                var bundles = printData.Bundles;
                var bundleStr = "";

                //split based on commoa

                var bundleArray = bundles.Split(new char[] { ',' });
                Dictionary<int, int> bundleDict = new Dictionary<int, int>();

                for (int i = 0; i < bundleArray.Length; i++)
                {
                    if (string.IsNullOrEmpty(bundleArray[i]))
                        continue;

                    if (string.IsNullOrEmpty(bundleArray[i].Trim()))
                        continue;

                    var bundledata = bundleArray[i].Trim();
                    bundledata = bundledata.Replace("of", "");

                    var bundledataArray = bundledata.Split(new char[] { ' ' });

                    if (!bundleDict.ContainsKey(Convert.ToInt32(bundledataArray[1])))
                    {
                        bundleDict.Add(Convert.ToInt32(bundledataArray[1]), Convert.ToInt32(bundledataArray[0]));
                    }
                    else
                    {
                        var keyDict = Convert.ToInt32(bundledataArray[1]);
                        bundleDict[keyDict] += Convert.ToInt32(bundledataArray[0]);
                    }
                }

                foreach (var dicKey in bundleDict.Keys)
                {
                    bundleStr += bundleDict[dicKey] + " of " + dicKey + " / ";
                }

                worksheet.Cells[y, 4].Value = bundleStr;

                worksheet.Cells[y, 1].Style.Border.BorderAround(
                    OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                worksheet.Cells[y, 2].Style.Border.BorderAround(
                    OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                worksheet.Cells[y, 3].Style.Border.BorderAround(
                    OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                worksheet.Cells[y, 4].Style.Border.BorderAround(
                    OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                y++;
            }

            worksheet.Column(1).Width = 13;
            worksheet.Column(2).Width = 70;
            worksheet.Column(3).Width = 13;
            worksheet.Column(4).Width = 50;
        }

        private static void BuildPrintRunHeader(Dictionary<string, int> headerColumns, int icount)
        {
            headerColumns.Add("Filename", icount);
            icount++;

            headerColumns.Add("Flights", icount);
            icount++;

            headerColumns.Add("Quantity", icount);
            icount++;

            headerColumns.Add("Bundles", icount);
            icount++;
        }


        private static void flightScheduleReportWorksheetPreparation(ExcelWorksheet worksheet, Dictionary<string, int> headerColumns, List<FlightScheduledata> data)
        {

            // Write column headers
            foreach (var colKvp in headerColumns)
            {
                if (colKvp.Value > 0)
                {
                    worksheet.Cells[1, colKvp.Value].Style.Border.BorderAround(
                        OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                    worksheet.Cells[1, colKvp.Value].Value = colKvp.Key;

                    worksheet.Cells[1, colKvp.Value].AutoFitColumns();

                    worksheet.Cells[1, colKvp.Value].Style.Font.Bold = true;
                    worksheet.Cells[1, colKvp.Value].AutoFitColumns();
                    worksheet.Cells[1, colKvp.Value].Style.HorizontalAlignment =
                        OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    worksheet.Cells[1, colKvp.Value].Style.VerticalAlignment =
                        OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                }
            }

            int y = 2;

            foreach (var row in data)
            {

                int column = 1;

                string flt = row.FltNo.Trim().Replace("VS", "");

                int flightNum = Convert.ToInt16(flt);

                worksheet.Cells[y, column].Value = flightNum;
                worksheet.Cells[y, column].Style.Border.BorderAround(
                    OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                worksheet.Cells[y, column].Style.Border.BorderAround(
                    OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                worksheet.Cells[y, column].Style.Border.BorderAround(
                    OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                worksheet.Cells[y, column].Style.Border.BorderAround(
                    OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                column++;


                worksheet.Cells[y, column].Value = row.Origin;
                worksheet.Cells[y, column].Style.Border.BorderAround(
                    OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                worksheet.Cells[y, column].Style.Border.BorderAround(
                    OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                worksheet.Cells[y, column].Style.Border.BorderAround(
                    OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                worksheet.Cells[y, column].Style.Border.BorderAround(
                    OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                column++;


                worksheet.Cells[y, column].Value = row.Dept;
                worksheet.Cells[y, column].Style.Border.BorderAround(
                    OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                worksheet.Cells[y, column].Style.Border.BorderAround(
                    OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                worksheet.Cells[y, column].Style.Border.BorderAround(
                    OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                worksheet.Cells[y, column].Style.Border.BorderAround(
                    OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                column++;

                worksheet.Cells[y, column].Value = row.Dest;
                worksheet.Cells[y, column].Style.Border.BorderAround(
                    OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                worksheet.Cells[y, column].Style.Border.BorderAround(
                    OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                worksheet.Cells[y, column].Style.Border.BorderAround(
                    OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                worksheet.Cells[y, column].Style.Border.BorderAround(
                    OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                column++;


                worksheet.Cells[y, column].Value = row.Arrival;
                worksheet.Cells[y, column].Style.Border.BorderAround(
                    OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                worksheet.Cells[y, column].Style.Border.BorderAround(
                    OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                worksheet.Cells[y, column].Style.Border.BorderAround(
                    OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                worksheet.Cells[y, column].Style.Border.BorderAround(
                    OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                column++;

                worksheet.Cells[y, column].Value = row.M;
                worksheet.Cells[y, column].Style.Border.BorderAround(
                    OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                worksheet.Cells[y, column].Style.Border.BorderAround(
                    OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                worksheet.Cells[y, column].Style.Border.BorderAround(
                    OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                worksheet.Cells[y, column].Style.Border.BorderAround(
                    OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                column++;

                worksheet.Cells[y, column].Value = row.Tu;
                worksheet.Cells[y, column].Style.Border.BorderAround(
                    OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                worksheet.Cells[y, column].Style.Border.BorderAround(
                    OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                worksheet.Cells[y, column].Style.Border.BorderAround(
                    OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                worksheet.Cells[y, column].Style.Border.BorderAround(
                    OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                column++;

                worksheet.Cells[y, column].Value = row.W;
                worksheet.Cells[y, column].Style.Border.BorderAround(
                    OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                worksheet.Cells[y, column].Style.Border.BorderAround(
                    OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                worksheet.Cells[y, column].Style.Border.BorderAround(
                    OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                worksheet.Cells[y, column].Style.Border.BorderAround(
                    OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                column++;

                worksheet.Cells[y, column].Value = row.Th;
                worksheet.Cells[y, column].Style.Border.BorderAround(
                    OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                worksheet.Cells[y, column].Style.Border.BorderAround(
                    OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                worksheet.Cells[y, column].Style.Border.BorderAround(
                    OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                worksheet.Cells[y, column].Style.Border.BorderAround(
                    OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                column++;

                worksheet.Cells[y, column].Value = row.F;
                worksheet.Cells[y, column].Style.Border.BorderAround(
                    OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                worksheet.Cells[y, column].Style.Border.BorderAround(
                    OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                worksheet.Cells[y, column].Style.Border.BorderAround(
                    OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                worksheet.Cells[y, column].Style.Border.BorderAround(
                    OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                column++;

                worksheet.Cells[y, column].Value = row.Sa;
                worksheet.Cells[y, column].Style.Border.BorderAround(
                    OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                worksheet.Cells[y, column].Style.Border.BorderAround(
                    OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                worksheet.Cells[y, column].Style.Border.BorderAround(
                    OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                worksheet.Cells[y, column].Style.Border.BorderAround(
                    OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                column++;

                worksheet.Cells[y, column].Value = row.Su;
                worksheet.Cells[y, column].Style.Border.BorderAround(
                    OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                worksheet.Cells[y, column].Style.Border.BorderAround(
                    OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                worksheet.Cells[y, column].Style.Border.BorderAround(
                    OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                worksheet.Cells[y, column].Style.Border.BorderAround(
                    OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                column++;

                worksheet.Cells[y, column].Value = row.EquipmentType;
                worksheet.Cells[y, column].Style.Border.BorderAround(
                    OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                worksheet.Cells[y, column].Style.Border.BorderAround(
                    OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                worksheet.Cells[y, column].Style.Border.BorderAround(
                    OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                worksheet.Cells[y, column].Style.Border.BorderAround(
                    OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                column++;

                worksheet.Cells[y, column].Value = row.Effective;
                worksheet.Cells[y, column].Style.Border.BorderAround(
                    OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                worksheet.Cells[y, column].Style.Border.BorderAround(
                    OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                worksheet.Cells[y, column].Style.Border.BorderAround(
                    OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                worksheet.Cells[y, column].Style.Border.BorderAround(
                    OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                column++;

                worksheet.Cells[y, column].Value = row.Discontinue;
                worksheet.Cells[y, column].Style.Border.BorderAround(
                    OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                worksheet.Cells[y, column].Style.Border.BorderAround(
                    OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                worksheet.Cells[y, column].Style.Border.BorderAround(
                    OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                worksheet.Cells[y, column].Style.Border.BorderAround(
                    OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                column++;

                y++;
            }

        }

        private static void BuildFlightScheduleHeader(Dictionary<string, int> headerColumns, int icount)
        {
            headerColumns.Add("Flt No", icount);
            icount++;

            headerColumns.Add("Origin", icount);
            icount++;

            headerColumns.Add("Dept", icount);
            icount++;

            headerColumns.Add("Dest", icount);
            icount++;

            headerColumns.Add("Arrival", icount);
            icount++;

            headerColumns.Add("M", icount);
            icount++;

            headerColumns.Add("Tu", icount);
            icount++;

            headerColumns.Add("W", icount);
            icount++;

            headerColumns.Add("Th", icount);
            icount++;

            headerColumns.Add("F", icount);
            icount++;

            headerColumns.Add("Sa", icount);
            icount++;

            headerColumns.Add("Su", icount);
            icount++;

            headerColumns.Add("Equipment Type", icount);
            icount++;

            headerColumns.Add("Effective", icount);
            icount++;

            headerColumns.Add("Discontinue", icount);
            icount++;

        }


        public static void SaveStreamToFile(string fileFullPath, Stream stream)
        {
            if (stream.Length == 0) return;

            // Create a FileStream object to write a stream to a file
            using (FileStream fileStream = File.Create(fileFullPath, (int)stream.Length))
            {
                // Fill the bytes[] array with the stream data
                var bytesInStream = new byte[stream.Length];
                stream.Read(bytesInStream, 0, (int)bytesInStream.Length);

                // Use FileStream object to write to the specified file
                fileStream.Write(bytesInStream, 0, bytesInStream.Length);
            }
        }



        public static void CreateBoxTicketSpreadSheet(List<tBoxTicketData> boxTicketData, long orderId, string lotNo, string boxTicketPath)
        {
            //// Set up columns
            var headerColumns = new Dictionary<string, int>();

            ////First validate all the columns in the xml file is present in spreadsheet or not?
            int icount = 1;
            BuildBoxTicketHeader(headerColumns, icount);

            // Export data
            using (var package = new ExcelPackage())
            {

                var classNameSearch = "J";
                var shipToSearch = "LHR";

                if (ShipToPresent(boxTicketData, shipToSearch))
                {

                    BoxReportWorksheetPreparation(shipToSearch, classNameSearch, package, orderId, lotNo, headerColumns, boxTicketData);

                    classNameSearch = "W";

                    BoxReportWorksheetPreparation(shipToSearch, classNameSearch, package, orderId, lotNo, headerColumns, boxTicketData);

                    classNameSearch = "Y";

                    BoxReportWorksheetPreparation(shipToSearch, classNameSearch, package, orderId, lotNo, headerColumns, boxTicketData);
                }


                classNameSearch = "J";
                shipToSearch = "LGW";

                if (ShipToPresent(boxTicketData, shipToSearch))
                {

                    BoxReportWorksheetPreparation(shipToSearch, classNameSearch, package, orderId, lotNo, headerColumns, boxTicketData);

                    classNameSearch = "W";


                    BoxReportWorksheetPreparation(shipToSearch, classNameSearch, package, orderId, lotNo, headerColumns, boxTicketData);

                    classNameSearch = "Y";

                    BoxReportWorksheetPreparation(shipToSearch, classNameSearch, package, orderId, lotNo, headerColumns, boxTicketData);
                }

                classNameSearch = "J";
                shipToSearch = "MAN";
                if (ShipToPresent(boxTicketData, shipToSearch))
                {

                    BoxReportWorksheetPreparation(shipToSearch, classNameSearch, package, orderId, lotNo, headerColumns, boxTicketData);

                    classNameSearch = "W";

                    BoxReportWorksheetPreparation(shipToSearch, classNameSearch, package, orderId, lotNo, headerColumns, boxTicketData);

                    classNameSearch = "Y";

                    BoxReportWorksheetPreparation(shipToSearch, classNameSearch, package, orderId, lotNo, headerColumns, boxTicketData);
                }

                classNameSearch = "J";
                shipToSearch = "GLA";
                if (ShipToPresent(boxTicketData, shipToSearch))
                {

                    BoxReportWorksheetPreparation(shipToSearch, classNameSearch, package, orderId, lotNo, headerColumns, boxTicketData);

                    classNameSearch = "W";

                    BoxReportWorksheetPreparation(shipToSearch, classNameSearch, package, orderId, lotNo, headerColumns, boxTicketData);

                    classNameSearch = "Y";

                    BoxReportWorksheetPreparation(shipToSearch, classNameSearch, package, orderId, lotNo, headerColumns, boxTicketData);
                }

                classNameSearch = "J";
                shipToSearch = "EDI";
                if (ShipToPresent(boxTicketData, shipToSearch))
                {

                    BoxReportWorksheetPreparation(shipToSearch, classNameSearch, package, orderId, lotNo, headerColumns, boxTicketData);

                    classNameSearch = "W";

                    BoxReportWorksheetPreparation(shipToSearch, classNameSearch, package, orderId, lotNo, headerColumns, boxTicketData);

                    classNameSearch = "Y";


                    BoxReportWorksheetPreparation(shipToSearch, classNameSearch, package, orderId, lotNo, headerColumns, boxTicketData);

                }
                // Save file and return stream
                var fileName = Path.GetTempFileName();
                package.SaveAs(new FileInfo(fileName));


                SaveStreamToFile(boxTicketPath, new FileStream(fileName, FileMode.Open));

            }
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
        private static void BoxReportWorksheetPreparation(string shipToSearch, string classNameSearch, ExcelPackage package, long orderId, string lotNo, Dictionary<string, int> headerColumns, List<tBoxTicketData> boxTicketData)
        {
            var worksheet = package.Workbook.Worksheets.Add(shipToSearch + " " + classNameSearch);
            int? totalCount = 0;

            //worksheet.Cells[1, 1].Value = "Order ID";
            //worksheet.Cells[1, 1].Style.Font.Bold = true;
            //worksheet.Cells[1, 2].Value = orderId;
            //worksheet.Cells[1, 2].Style.HorizontalAlignment =
            //    OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            //worksheet.Cells[1, 2].Style.VerticalAlignment =
            //    OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

            //worksheet.Cells[2, 1].Value = "LOT NO";
            //worksheet.Cells[2, 1].Style.Font.Bold = true;
            //worksheet.Cells[2, 2].Value = lotNo;
            //worksheet.Cells[2, 2].Style.HorizontalAlignment =
            //    OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            //worksheet.Cells[2, 2].Style.VerticalAlignment =
            //    OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

            // Write column headers
            foreach (var colKvp in headerColumns)
            {
                if (colKvp.Value > 0)
                {
                    worksheet.Cells[1, colKvp.Value].Style.Border.BorderAround(
                        OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                    worksheet.Cells[1, colKvp.Value].Value = colKvp.Key;

                    worksheet.Cells[1, colKvp.Value].AutoFitColumns();

                    worksheet.Cells[1, colKvp.Value].Style.Font.Bold = true;
                    worksheet.Cells[1, colKvp.Value].AutoFitColumns();
                    worksheet.Cells[1, colKvp.Value].Style.HorizontalAlignment =
                        OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    worksheet.Cells[1, colKvp.Value].Style.VerticalAlignment =
                        OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                    if (colKvp.Value == 6)
                    {
                        worksheet.Cells[1, colKvp.Value].Style.Numberformat.Format = "dd-mm-yyyy";
                    }
                }
            }

            int y = 2;

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

                List<tBoxTicketData> alreadyWrittenInBound = new List<tBoxTicketData>();

                foreach (var dateBoxOutbound in dateBoxOutboundData)
                {
                    if (dateBoxOutbound == null)
                        continue;

                    //outbound write
                    totalCount += dateBoxOutbound.Count;
                    WriteOutboundBoxTicketRow(worksheet, y, dateBoxOutbound);
                    y++;

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
                            alreadyWrittenInBound.Add(inboundForOutbound);
                            totalCount += inboundForOutbound.Count;
                            WriteInBoundBoxTicketRow(worksheet, y, inboundForOutbound);
                            y++;
                        }
                    }
                }

                //check for any missed inbound which is purely inbound and no outbound associated

                foreach (var inbound in dataInBound)
                {
                    bool found = false;

                    foreach (var writtenData in alreadyWrittenInBound)
                    {
                        if (writtenData.Bound.ToLower() == "inbound")
                        {
                            if (inbound.Date == writtenData.Date && inbound.FlightNo == writtenData.FlightNo && inbound.Time == writtenData.Time && inbound.Route == writtenData.Route && inbound.Count == writtenData.Count)
                            {
                                found = true;
                                break;
                            }
                        }
                    }
                    if (!found)
                    {
                        totalCount += inbound.Count;
                        WriteInBoundBoxTicketRow(worksheet, y, inbound);
                        y++;
                    }
                }
                y++;

            }

            worksheet.Cells[y, 6].Value = "TOTAL_COUNT";
            worksheet.Cells[y, 6].Style.Border.BorderAround(
                OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);

            worksheet.Cells[y, 7].Value = totalCount;
            worksheet.Cells[y, 7].Style.Border.BorderAround(
                OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);



            worksheet.Column(6).Width = 15;
        }

        private static void WriteOutboundBoxTicketRow(ExcelWorksheet worksheet, int y, tBoxTicketData dateBoxOutbound)
        {
            worksheet.Cells[y, 1].Value = dateBoxOutbound.ClassName;
            worksheet.Cells[y, 1].Style.Border.BorderAround(
                OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);

            worksheet.Cells[y, 2].Value = dateBoxOutbound.ShipTo;
            worksheet.Cells[y, 2].Style.Border.BorderAround(
                OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);

            worksheet.Cells[y, 3].Value = dateBoxOutbound.FlightNo;
            worksheet.Cells[y, 3].Style.Border.BorderAround(
                OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);

            worksheet.Cells[y, 4].Value = dateBoxOutbound.Route;
            worksheet.Cells[y, 4].Style.Border.BorderAround(
                OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);

            worksheet.Cells[y, 5].Value = dateBoxOutbound.Time;
            worksheet.Cells[y, 5].Style.Border.BorderAround(
                OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);

            worksheet.Cells[y, 6].Style.Numberformat.Format = "dd-mm-yyyy";
            worksheet.Cells[y, 6].Value = dateBoxOutbound.Date;
            worksheet.Cells[y, 6].Style.Numberformat.Format = "dd-mm-yyyy";
            worksheet.Cells[y, 6].Style.Border.BorderAround(
                OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);

            worksheet.Cells[y, 7].Value = dateBoxOutbound.Count;
            worksheet.Cells[y, 7].Style.Border.BorderAround(
                OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);

            worksheet.Cells[y, 8].Value = dateBoxOutbound.Bound;
            worksheet.Cells[y, 8].Style.Border.BorderAround(
                OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);

            worksheet.Cells[y, 9].Value = dateBoxOutbound.MenuCode;
            worksheet.Cells[y, 9].Style.Border.BorderAround(
                OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);

            if (dateBoxOutbound.ClassName == "J")
                worksheet.Cells[y, 10].Value = dateBoxOutbound.TEAMenuCode;
            else
                worksheet.Cells[y, 10].Value = "";

            worksheet.Cells[y, 10].Style.Border.BorderAround(
                OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);

            if (dateBoxOutbound.ClassName == "J")
                worksheet.Cells[y, 11].Value = dateBoxOutbound.BRKMenuCode;
            else
                worksheet.Cells[y, 11].Value = "";
            worksheet.Cells[y, 11].Style.Border.BorderAround(
                OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);
        }

        private static void WriteInBoundBoxTicketRow(ExcelWorksheet worksheet, int y, tBoxTicketData inbound)
        {
            worksheet.Cells[y, 1].Value = inbound.ClassName;
            worksheet.Cells[y, 1].Style.Border.BorderAround(
                OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);

            worksheet.Cells[y, 2].Value = inbound.ShipTo;
            worksheet.Cells[y, 2].Style.Border.BorderAround(
                OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);

            worksheet.Cells[y, 3].Value = inbound.FlightNo;
            worksheet.Cells[y, 3].Style.Border.BorderAround(
                OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);

            worksheet.Cells[y, 4].Value = inbound.Route;
            worksheet.Cells[y, 4].Style.Border.BorderAround(
                OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);

            worksheet.Cells[y, 5].Value = inbound.Time;
            worksheet.Cells[y, 5].Style.Border.BorderAround(
                OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);

            worksheet.Cells[y, 6].Style.Numberformat.Format = "dd-mm-yyyy";

            var flightNum = inbound.FlightNo;

            if (flightNum == "VS251" || flightNum == "VS026" || flightNum == "VS207" || flightNum == "VS301" || flightNum == "VS401" || flightNum == "VS412" || flightNum == "VS450")
            {
                worksheet.Cells[y, 6].Value = Convert.ToDateTime(inbound.Date).AddDays(1);
            }
            else
                worksheet.Cells[y, 6].Value = inbound.Date;


            worksheet.Cells[y, 6].Style.Numberformat.Format = "dd-mm-yyyy";
            worksheet.Cells[y, 6].Style.Border.BorderAround(
                OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);

            worksheet.Cells[y, 7].Value = inbound.Count;
            worksheet.Cells[y, 7].Style.Border.BorderAround(
                OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);

            worksheet.Cells[y, 8].Value = inbound.Bound;
            worksheet.Cells[y, 8].Style.Border.BorderAround(
                OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);

            worksheet.Cells[y, 9].Value = inbound.MenuCode;
            worksheet.Cells[y, 9].Style.Border.BorderAround(
                OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);

            if (inbound.ClassName == "J")
                worksheet.Cells[y, 10].Value = inbound.TEAMenuCode;
            else
                worksheet.Cells[y, 10].Value = "";

            worksheet.Cells[y, 10].Style.Border.BorderAround(
                OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);

            if (inbound.ClassName == "J")
                worksheet.Cells[y, 11].Value = inbound.BRKMenuCode;
            else
                worksheet.Cells[y, 11].Value = "";
            worksheet.Cells[y, 11].Style.Border.BorderAround(
                OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);
        }

        private static void BuildBoxTicketHeader(Dictionary<string, int> headerColumns, int icount)
        {
            headerColumns.Add("Menu Class", icount);
            icount++;

            headerColumns.Add("Ship To", icount);
            icount++;

            headerColumns.Add("Flt No", icount);
            icount++;

            headerColumns.Add("Route", icount);
            icount++;

            headerColumns.Add("Dep. Time", icount);
            icount++;

            headerColumns.Add("Date", icount);
            icount++;

            headerColumns.Add("Menu Count", icount);
            icount++;

            headerColumns.Add("Bound", icount);
            icount++;

            headerColumns.Add("Main Menu", icount);
            icount++;

            headerColumns.Add("TEA Card", icount);
            icount++;

            headerColumns.Add("BRK Card", icount);
            icount++;

        }
    }
}
