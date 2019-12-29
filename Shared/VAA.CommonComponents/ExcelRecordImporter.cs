using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OfficeOpenXml;
using VAA.CommonComponents.Interfaces;

namespace VAA.CommonComponents
{
    /// <summary>
    /// Excel Importer using - EEPLUS
    /// </summary>
    public class ExcelRecordImporter : IRecordImporter
    {

        private string fileName;
        private Stream stream;
        private ExcelPackage package;
        private ExcelWorkbook workbook;

        public ExcelRecordImporter(string fileName)
        {
            this.fileName = fileName;
            package = new ExcelPackage(new FileInfo(fileName));
            workbook = package.Workbook;
        }

        public ExcelRecordImporter(Stream stream)
        {
            this.stream = stream;
            try
            {
                package = new ExcelPackage(stream);
            }
            catch (FormatException)
            {
                throw new VAAException("File is empty, corrupt or not an Excel 2010 file (.xlsx)");
            }
            workbook = package.Workbook;
        }

        public IEnumerable<string> GetDataSetNames()
        {
            return workbook.Worksheets.Select(worksheet => worksheet.Name);
        }

        public IEnumerable<Dictionary<string, string>> Import(string sheetName)
        {
            var worksheet = workbook.Worksheets[sheetName];
            var top = worksheet.Dimension.Start.Column;
            var bottom = worksheet.Dimension.End.Row;
            var left = worksheet.Dimension.Start.Row;
            var right = worksheet.Dimension.End.Column;
            var rows = new List<Dictionary<string, string>>();

            bool isEmpty = true;

            // Go through each non-header cell
            for (int y = top + 1; y <= bottom; y++)
            {
                var row = new Dictionary<string, string>();
                isEmpty = true;

                // Populate new row with data
                for (int x = left; x <= right; x++)
                {
                    var headerVal = (worksheet.Cells[top, x].Value ?? "").ToString().ToLower();
                    if (headerVal != "" && !row.ContainsKey(headerVal))
                    {
                        row[headerVal] = (worksheet.Cells[y, x].Value ?? "").ToString();

                        if (!string.IsNullOrEmpty(row[headerVal]))
                        {
                            isEmpty = false;
                        }
                    }
                }
                //if (!isEmpty)
                {
                    if ((row.ContainsKey("date") && row["date"] != "TOTAL_COUNT") || !row.ContainsKey("date"))
                        rows.Add(row);
                }
            }

            return rows;
        }

        public int TotalNumRows
        {
            get
            {
                int numRows = 0;
                int i = 0;

                foreach (var worksheet in workbook.Worksheets)
                {
                    i++;
                    if (i > 1)
                        continue;
                    numRows += worksheet.Dimension.End.Row - 1;
                }
                return numRows;
            }
        }
    }
}
