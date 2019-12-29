using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using VAA.BusinessComponents;
using VAA.CommonComponents;
using VAA.DataAccess;
using PDFProcessingVAA;
using OfficeOpenXml;
using System.Diagnostics;
using OutputSpreadsheetWriterLibrary;

namespace PackingTicketGenerator
{
    public partial class Action : Form
    {
        /// <summary>
        /// Main action - VAA action items
        /// </summary>
        public Action()
        {
            InitializeComponent();
        }


        /// <summary>
        /// Generate Packing tickets PDFS
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGenerate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtBoxOrderId.Text))
            {
                MessageBox.Show("Please enter Order Id!");
                return;
            }

            OrderManagement orderManagement = new OrderManagement();

            bool boxticketDataReady = orderManagement.IsBoxTicketDataGenerated(Convert.ToInt64(txtBoxOrderId.Text));

            if (!boxticketDataReady)
            {
                MessageBox.Show("Box report must be generated before packing ticket pdf generation.");
                return;

            }

            btnGenerateOrderPDF.Enabled = false;
            btnGenerate.Enabled = false;
            btnPackingLabels.Enabled = false;
            btnAddFlights.Enabled = false;
            btnReset.Enabled = false;
            lblStatus.Text = "In Progress, Please Wait...";

            Thread thread = new Thread(new ThreadStart(GeneratePackingPdfs));
            thread.Start();

        }

        private void GeneratePackingPdfs()
        {
            try
            {

                MessageBox.Show("Packing Ticket Generation is in progress and you will be notified once process is complete!");

                this.InvokeEx(f => f.lblStatus.Visible = true);
                this.InvokeEx(f => f.btnGenerateOrderPDF.Enabled = false);
                this.InvokeEx(f => f.btnGenerate.Enabled = false);
                this.InvokeEx(f => f.btnPackingLabels.Enabled = false);
                this.InvokeEx(f => f.btnAddFlights.Enabled = false);
                this.InvokeEx(f => f.btnGeneratePackingData.Enabled = false);


                var order = txtBoxOrderId.Text.Trim();

                long orderId = Convert.ToInt64(order);

                PackingTicketProcessor packingTicket = new PackingTicketProcessor();

                packingTicket.CreateBoxTicketProofsAndTicketPDF(orderId);

                var message = EmailHelper.PackingTicketPdfGenerationCompleteEmailTemplate;

                message = EmailHelper.ConvertMail2(message, Convert.ToString(orderId), "\\[ORDERID\\]");

                ////send email
                SendEmailnotification("EMMA- Packing Ticket PDF Generation: " + order, message);

                MessageBox.Show("Packing Ticket PDF Generation completed successfully!");
                this.InvokeEx(f => f.lblStatus.Text = "Packing Ticket PDF Generation completed successfully!");

                this.InvokeEx(f => f.btnGenerateOrderPDF.Enabled = true);
                this.InvokeEx(f => f.btnGenerate.Enabled = true);
                this.InvokeEx(f => f.btnPackingLabels.Enabled = true);
                this.InvokeEx(f => f.btnAddFlights.Enabled = true);
                this.InvokeEx(f => f.btnView.Visible = true);
                this.InvokeEx(f => f.btnView.Text = "View Packing PDFs");
                this.InvokeEx(f => f.btnReset.Enabled = true);
                this.InvokeEx(f => f.btnGeneratePackingData.Enabled = true);


            }
            catch (Exception ex)
            {
                MessageBox.Show("Packing Ticket PDF Generation falied, please contact development team!");
                this.InvokeEx(f => f.lblStatus.Text = "Packing Ticket PDF Generation Failed!");
                this.InvokeEx(f => f.btnGenerateOrderPDF.Enabled = true);
                this.InvokeEx(f => f.btnGenerate.Enabled = true);
                this.InvokeEx(f => f.btnPackingLabels.Enabled = true);
                this.InvokeEx(f => f.btnAddFlights.Enabled = true);
                this.InvokeEx(f => f.btnReset.Enabled = true);
                this.InvokeEx(f => f.btnGeneratePackingData.Enabled = true);

            }
        }

        private void SendEmailnotification(string subject, string template)
        {
            //send email
            var packingPDFMessage = template;

            string notificationEmails = (System.Configuration.ConfigurationManager.AppSettings["NotificationEmails"]);

            var emails = notificationEmails.Split(new char[] { ';' });

            var allEmail = "";

            foreach (var email in emails)
            {
                if (!string.IsNullOrEmpty(email))
                {
                    EmailHelper.SendMail(email, "ESPAdmin@espcolour.co.uk", subject, packingPDFMessage);
                }
            }

            //EmailHelper.SendMail("R.Kumar@market-smart.co.uk", "ESPAdmin@espcolour.co.uk", subject, packingPDFMessage);
            //EmailHelper.SendMail("E.Sorrell@market-smart.co.uk", "ESPAdmin@espcolour.co.uk", subject, packingPDFMessage);
            //EmailHelper.SendMail("R.Lee@espcolour.co.uk", "ESPAdmin@espcolour.co.uk", subject, packingPDFMessage);
        }

        /// <summary>
        /// Show generate order PDF form - where you can choose menu type and geneate PDFs
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGenerateOrderPDF_Click(object sender, EventArgs e)
        {
            PDFGeneration pdfGenForm = new PDFGeneration();
            pdfGenForm.Show();
        }


        /// <summary>
        /// Generate packing labels
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPackingLabels_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtBoxOrderId.Text))
            {
                MessageBox.Show("Please enter Order Id!");
                return;
            }

            var BoxReportPath = @"\\nas1\Digital_Production\Virgin\BoxReport\BoxReport_" + txtBoxOrderId.Text + ".xlsx";

            if (!File.Exists(BoxReportPath))
            {
                MessageBox.Show("Box Report is not found at path: " + BoxReportPath + "." + " Please ensure Box report has been generated before generating packing labels.");
                return;

            }

            lblStatus.Text = "In Progress, Please Wait...";
            Thread thread = new Thread(new ThreadStart(GeneratePackingLabelExcels));
            thread.Start();


        }

        private void GeneratePackingLabelExcels()
        {
            MessageBox.Show("Packing Label Excel Generation is in progress and you will be notified once process is complete!");

            this.InvokeEx(f => f.btnGenerateOrderPDF.Enabled = false);
            this.InvokeEx(f => f.btnGenerate.Enabled = false);
            this.InvokeEx(f => f.btnPackingLabels.Enabled = false);
            this.InvokeEx(f => f.btnAddFlights.Enabled = false);
            this.InvokeEx(f => f.btnReset.Enabled = false);
            this.InvokeEx(f => f.btnGeneratePackingData.Enabled = false);


            this.InvokeEx(f => f.lblStatus.Visible = true);

            try
            {
                var order = txtBoxOrderId.Text.Trim();

                var BoxReportPath = @"\\nas1\Digital_Production\Virgin\BoxReport\BoxReport_" + txtBoxOrderId.Text + ".xlsx";

                ExcelRecordImporter importer = new ExcelRecordImporter(BoxReportPath);

                foreach (var dataSetName in importer.GetDataSetNames())
                {
                    var blankLabelName = "";

                    if (dataSetName == "LHR J")
                        blankLabelName = ProcessLHRJ(importer, dataSetName, blankLabelName);

                    if (dataSetName == "LHR W")
                        blankLabelName = ProcessLHRW(importer, dataSetName, blankLabelName);

                    if (dataSetName == "LHR Y")
                        blankLabelName = ProcessLHRY(importer, dataSetName, blankLabelName);

                    if (dataSetName == "LGW J")
                        blankLabelName = ProcessLGWJ(importer, dataSetName, blankLabelName);

                    if (dataSetName == "LGW W")
                        blankLabelName = ProcessLGWW(importer, dataSetName, blankLabelName);

                    if (dataSetName == "LGW Y")
                        blankLabelName = ProcessLGWY(importer, dataSetName, blankLabelName);

                    if (dataSetName == "MAN J")
                        blankLabelName = ProcessMANJ(importer, dataSetName, blankLabelName);
                    if (dataSetName == "MAN W")
                        blankLabelName = ProcessMANW(importer, dataSetName, blankLabelName);

                    if (dataSetName == "MAN Y")
                        blankLabelName = ProcessMANY(importer, dataSetName, blankLabelName);

                    if (dataSetName == "GLA J")
                        blankLabelName = ProcessGLAJ(importer, dataSetName, blankLabelName);

                    if (dataSetName == "GLA W")
                        blankLabelName = ProcessGLAW(importer, dataSetName, blankLabelName);

                    if (dataSetName == "GLA Y")
                        blankLabelName = ProcessGLAY(importer, dataSetName, blankLabelName);
                }

                MessageBox.Show("Packing Label Excel Generation completed successfully!");
                this.InvokeEx(f => f.lblStatus.Text = "Packing Label Excel Generation completed successfully!");

                this.InvokeEx(f => f.btnGenerateOrderPDF.Enabled = true);
                this.InvokeEx(f => f.btnGenerate.Enabled = true);
                this.InvokeEx(f => f.btnPackingLabels.Enabled = true);
                this.InvokeEx(f => f.btnGeneratePackingData.Enabled = true);
                this.InvokeEx(f => f.btnAddFlights.Enabled = true);
                this.InvokeEx(f => f.btnView.Visible = true);
                this.InvokeEx(f => f.btnView.Text = "View Packing Labels");
                this.InvokeEx(f => f.btnReset.Enabled = true);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Packing Label Excel Generation falied, please contact development team!");
                this.InvokeEx(f => f.lblStatus.Text = "Packing Label Excel Generation falied!");

                this.InvokeEx(f => f.btnGenerateOrderPDF.Enabled = true);
                this.InvokeEx(f => f.btnGeneratePackingData.Enabled = true);
                this.InvokeEx(f => f.btnGenerate.Enabled = true);
                this.InvokeEx(f => f.btnPackingLabels.Enabled = true);
                this.InvokeEx(f => f.btnAddFlights.Enabled = true);
                this.InvokeEx(f => f.btnReset.Enabled = true);
            }
        }

        private string ProcessLHRW(ExcelRecordImporter importer, string dataSetName, string blankLabelName)
        {
            OrderManagement orderManagement = new OrderManagement();

            var OrderId = txtBoxOrderId.Text;

            var LotNo = orderManagement.GetLotNoFromOrderId(Convert.ToInt64(txtBoxOrderId.Text));

            var importedBoxTicketRows = importer.Import(dataSetName);

            if (importedBoxTicketRows != null && importedBoxTicketRows.Any())
            {

                blankLabelName = "W MENU WK LHR";

                var blankLabelPath = @"\\nas1\Digital_Production\Virgin\PACKING LABELS BLANK TEMPLATE\" + blankLabelName + ".xlsx";

                var blankLabelPathOrder = @"\\nas1\Digital_Production\Virgin\Packing Label Output\" + OrderId;

                if (!Directory.Exists(blankLabelPathOrder))
                    Directory.CreateDirectory(blankLabelPathOrder);

                blankLabelPathOrder = blankLabelPathOrder + @"\" + blankLabelName + ".xlsx";

                File.Copy(blankLabelPath, blankLabelPathOrder, true);

                ExcelRecordImporter importerBlankLabel = new ExcelRecordImporter(blankLabelPath);

                int i = 1;


                FileInfo finfo = new FileInfo(blankLabelPathOrder);
                var lastDate = "";

                int rowCount = 0;

                int BoxLabelRow = 6;
                int colJumpFactor = 0;
                int RowJumpFactor = 0;

                foreach (var row in importedBoxTicketRows)
                {
                    rowCount++;
                    using (var excelPackage = new ExcelPackage(finfo))
                    {

                        ExcelWorksheet ws = excelPackage.Workbook.Worksheets["date" + i];


                        if (string.IsNullOrEmpty(row["date"]))
                        {
                            i++;
                            //ws.Name = lastDate;
                            excelPackage.Save();
                            BoxLabelRow = 6;
                            colJumpFactor = 0;
                            RowJumpFactor = 0;
                            continue;
                        }



                        var date = DateTime.FromOADate(Convert.ToDouble(row["date"]));

                        lastDate = date.ToShortDateString().Replace("/", "-");



                        ws.Cells["F1"].Value = OrderId;
                        ws.Cells["L1"].Value = OrderId;
                        ws.Cells["R1"].Value = OrderId;

                        ws.Cells["F2"].Value = LotNo;
                        ws.Cells["L2"].Value = LotNo;
                        ws.Cells["R2"].Value = LotNo;

                        ws.Cells["D3"].Value = "Date " + date.ToShortDateString();
                        ws.Cells["J3"].Value = "Date " + date.ToShortDateString();
                        ws.Cells["P3"].Value = "Date " + date.ToShortDateString();



                        ws.Cells[BoxLabelRow, 1 + colJumpFactor].Value = row["flt no"];
                        ws.Cells[BoxLabelRow, 2 + colJumpFactor].Value = row["route"];
                        ws.Cells[BoxLabelRow, 3 + colJumpFactor].Value = row["dep. time"];
                        ws.Cells[BoxLabelRow, 4 + colJumpFactor].Value = date.ToShortDateString();
                        ws.Cells[BoxLabelRow, 5 + colJumpFactor].Value = row["menu count"];
                        ws.Cells[BoxLabelRow, 6 + colJumpFactor].Value = row["bound"];

                        BoxLabelRow++;
                        RowJumpFactor++;


                        if (importedBoxTicketRows.Count() == rowCount)
                        {
                            //ws.Name = lastDate;
                            excelPackage.Save();
                            break;
                        }
                        if (BoxLabelRow >= 26)
                        {
                            colJumpFactor += 6;
                            BoxLabelRow = 6;
                            RowJumpFactor = 0;
                        }


                        if (RowJumpFactor == 2)
                        {
                            BoxLabelRow++;
                            RowJumpFactor = 0;
                        }




                        excelPackage.Save();


                    }


                }
            }
            return blankLabelName;
        }

        private string ProcessLHRJ(ExcelRecordImporter importer, string dataSetName, string blankLabelName)
        {
            OrderManagement orderManagement = new OrderManagement();

            var OrderId = txtBoxOrderId.Text;

            var LotNo = orderManagement.GetLotNoFromOrderId(Convert.ToInt64(txtBoxOrderId.Text));

            var importedBoxTicketRows = importer.Import(dataSetName);

            if (importedBoxTicketRows != null && importedBoxTicketRows.Any())
            {

                blankLabelName = "J MENU WK LHR";

                var blankLabelPath = @"\\nas1\Digital_Production\Virgin\PACKING LABELS BLANK TEMPLATE\" + blankLabelName + ".xlsx";

                var blankLabelPathOrder = @"\\nas1\Digital_Production\Virgin\Packing Label Output\" + OrderId;

                if (!Directory.Exists(blankLabelPathOrder))
                    Directory.CreateDirectory(blankLabelPathOrder);

                blankLabelPathOrder = blankLabelPathOrder + @"\" + blankLabelName + ".xlsx";

                File.Copy(blankLabelPath, blankLabelPathOrder, true);

                ExcelRecordImporter importerBlankLabel = new ExcelRecordImporter(blankLabelPath);


                int i = 1;


                FileInfo finfo = new FileInfo(blankLabelPathOrder);
                var lastDate = "";

                int rowCount = 0;

                int BoxLabelRow = 6;
                int colJumpFactor = 0;

                foreach (var row in importedBoxTicketRows)
                {
                    rowCount++;
                    using (var excelPackage = new ExcelPackage(finfo))
                    {

                        ExcelWorksheet ws = excelPackage.Workbook.Worksheets["date" + i];


                        if (string.IsNullOrEmpty(row["date"]))
                        {
                            i++;
                            // ws.Name = lastDate;
                            excelPackage.Save();
                            BoxLabelRow = 6;
                            colJumpFactor = 0;
                            continue;
                        }



                        var date = DateTime.FromOADate(Convert.ToDouble(row["date"]));

                        lastDate = date.ToShortDateString().Replace("/", "-");



                        ws.Cells["F1"].Value = OrderId;
                        ws.Cells["F2"].Value = LotNo;
                        ws.Cells["D3"].Value = "Date " + date.ToShortDateString();


                        ws.Cells["L1"].Value = OrderId;
                        ws.Cells["L2"].Value = LotNo;
                        ws.Cells["J3"].Value = "Date " + date.ToShortDateString();


                        ws.Cells["R1"].Value = OrderId;
                        ws.Cells["R2"].Value = LotNo;
                        ws.Cells["P3"].Value = "Date " + date.ToShortDateString();

                        ws.Cells["X1"].Value = OrderId;
                        ws.Cells["X2"].Value = LotNo;
                        ws.Cells["V3"].Value = "Date " + date.ToShortDateString();

                        ws.Cells["AD1"].Value = OrderId;
                        ws.Cells["AD2"].Value = LotNo;
                        ws.Cells["AB3"].Value = "Date " + date.ToShortDateString();

                        ws.Cells["AJ1"].Value = OrderId;
                        ws.Cells["AJ2"].Value = LotNo;
                        ws.Cells["AH3"].Value = "Date " + date.ToShortDateString();

                        ws.Cells["AP1"].Value = OrderId;
                        ws.Cells["AP2"].Value = LotNo;
                        ws.Cells["AN3"].Value = "Date " + date.ToShortDateString();

                        ws.Cells["AV1"].Value = OrderId;
                        ws.Cells["AV2"].Value = LotNo;
                        ws.Cells["AT3"].Value = "Date " + date.ToShortDateString();

                        ws.Cells["BB1"].Value = OrderId;
                        ws.Cells["BB2"].Value = LotNo;
                        ws.Cells["AZ3"].Value = "Date " + date.ToShortDateString();

                        ws.Cells["BH1"].Value = OrderId;
                        ws.Cells["BH2"].Value = LotNo;
                        ws.Cells["BF3"].Value = "Date " + date.ToShortDateString();

                        ws.Cells["BN1"].Value = OrderId;
                        ws.Cells["BN2"].Value = LotNo;
                        ws.Cells["BL3"].Value = "Date " + date.ToShortDateString();

                        ws.Cells[BoxLabelRow, 1 + colJumpFactor].Value = row["flt no"];
                        ws.Cells[BoxLabelRow, 2 + colJumpFactor].Value = row["route"];
                        ws.Cells[BoxLabelRow, 3 + colJumpFactor].Value = row["dep. time"];
                        ws.Cells[BoxLabelRow, 4 + colJumpFactor].Value = date.ToShortDateString();
                        ws.Cells[BoxLabelRow, 5 + colJumpFactor].Value = row["menu count"];
                        ws.Cells[BoxLabelRow, 6 + colJumpFactor].Value = row["bound"];

                        BoxLabelRow++;

                        if (importedBoxTicketRows.Count() == rowCount)
                        {
                            //ws.Name = lastDate;
                            excelPackage.Save();
                            break;
                        }

                        if (BoxLabelRow == 8 || BoxLabelRow == 11 || BoxLabelRow == 14)
                            BoxLabelRow++;

                        if (BoxLabelRow == 17)
                        {
                            colJumpFactor += 6;
                            BoxLabelRow = 6;
                        }



                        excelPackage.Save();


                    }


                }
            }
            return blankLabelName;
        }

        private string ProcessLHRY(ExcelRecordImporter importer, string dataSetName, string blankLabelName)
        {
            OrderManagement orderManagement = new OrderManagement();

            var OrderId = txtBoxOrderId.Text;

            var LotNo = orderManagement.GetLotNoFromOrderId(Convert.ToInt64(txtBoxOrderId.Text));

            var importedBoxTicketRows = importer.Import(dataSetName);

            if (importedBoxTicketRows != null && importedBoxTicketRows.Any())
            {

                blankLabelName = "Y MENU WK LHR";

                var blankLabelPath = @"\\nas1\Digital_Production\Virgin\PACKING LABELS BLANK TEMPLATE\" + blankLabelName + ".xlsx";

                var blankLabelPathOrder = @"\\nas1\Digital_Production\Virgin\Packing Label Output\" + OrderId;

                if (!Directory.Exists(blankLabelPathOrder))
                    Directory.CreateDirectory(blankLabelPathOrder);

                blankLabelPathOrder = blankLabelPathOrder + @"\" + blankLabelName + ".xlsx";

                File.Copy(blankLabelPath, blankLabelPathOrder, true);

                ExcelRecordImporter importerBlankLabel = new ExcelRecordImporter(blankLabelPath);

                int i = 1;


                FileInfo finfo = new FileInfo(blankLabelPathOrder);
                var lastDate = "";

                int rowCount = 0;

                int BoxLabelRow = 6;
                int colJumpFactor = 0;
                int countRow6 = 0;
                int countRow7 = 0;
                int countRow9 = 0;

                foreach (var row in importedBoxTicketRows)
                {
                    if (BoxLabelRow == 6)
                    {
                        var count = 0;

                        try
                        {
                            count = Convert.ToInt32(row["menu count"]);
                        }
                        catch { }

                        countRow6 = count;
                    }

                    if (BoxLabelRow == 7)
                    {
                        var count = 0;

                        try
                        {
                            count = Convert.ToInt32(row["menu count"]);
                        }
                        catch { }

                        countRow7 = count;
                    }

                    if (BoxLabelRow == 9)
                    {
                        var count = 0;

                        try
                        {
                            count = Convert.ToInt32(row["menu count"]);
                        }
                        catch { }

                        countRow9 = count;
                    }


                    if (BoxLabelRow == 9)
                    {
                        bool maxHold = false;

                        if (countRow6 >= 375 && countRow7 >= 375 && countRow9 >= 375)
                            maxHold = true;

                        if (maxHold)
                        {
                            colJumpFactor += 6;
                            BoxLabelRow = 6;
                        }
                    }

                    rowCount++;
                    using (var excelPackage = new ExcelPackage(finfo))
                    {

                        ExcelWorksheet ws = excelPackage.Workbook.Worksheets["date" + i];


                        if (string.IsNullOrEmpty(row["date"]))
                        {
                            i++;
                            //ws.Name = lastDate;
                            excelPackage.Save();
                            BoxLabelRow = 6;
                            colJumpFactor = 0;
                            continue;
                        }


                        var date = DateTime.FromOADate(Convert.ToDouble(row["date"]));

                        lastDate = date.ToShortDateString().Replace("/", "-");



                        ws.Cells["F1"].Value = OrderId;
                        ws.Cells["F2"].Value = LotNo;
                        ws.Cells["D3"].Value = "Date " + date.ToShortDateString();


                        ws.Cells["L1"].Value = OrderId;
                        ws.Cells["L2"].Value = LotNo;
                        ws.Cells["J3"].Value = "Date " + date.ToShortDateString();


                        ws.Cells["R1"].Value = OrderId;
                        ws.Cells["R2"].Value = LotNo;
                        ws.Cells["P3"].Value = "Date " + date.ToShortDateString();

                        ws.Cells["X1"].Value = OrderId;
                        ws.Cells["X2"].Value = LotNo;
                        ws.Cells["V3"].Value = "Date " + date.ToShortDateString();

                        ws.Cells["AD1"].Value = OrderId;
                        ws.Cells["AD2"].Value = LotNo;
                        ws.Cells["AB3"].Value = "Date " + date.ToShortDateString();

                        ws.Cells["AJ1"].Value = OrderId;
                        ws.Cells["AJ2"].Value = LotNo;
                        ws.Cells["AH3"].Value = "Date " + date.ToShortDateString();

                        ws.Cells["AP1"].Value = OrderId;
                        ws.Cells["AP2"].Value = LotNo;
                        ws.Cells["AN3"].Value = "Date " + date.ToShortDateString();

                        ws.Cells["AV1"].Value = OrderId;
                        ws.Cells["AV2"].Value = LotNo;
                        ws.Cells["AT3"].Value = "Date " + date.ToShortDateString();

                        ws.Cells["BB1"].Value = OrderId;
                        ws.Cells["BB2"].Value = LotNo;
                        ws.Cells["AZ3"].Value = "Date " + date.ToShortDateString();

                        ws.Cells["BH1"].Value = OrderId;
                        ws.Cells["BH2"].Value = LotNo;
                        ws.Cells["BF3"].Value = "Date " + date.ToShortDateString();

                        ws.Cells["BN1"].Value = OrderId;
                        ws.Cells["BN2"].Value = LotNo;
                        ws.Cells["BL3"].Value = "Date " + date.ToShortDateString();

                        ws.Cells[BoxLabelRow, 1 + colJumpFactor].Value = row["flt no"];
                        ws.Cells[BoxLabelRow, 2 + colJumpFactor].Value = row["route"];
                        ws.Cells[BoxLabelRow, 3 + colJumpFactor].Value = row["dep. time"];
                        ws.Cells[BoxLabelRow, 4 + colJumpFactor].Value = date.ToShortDateString();
                        ws.Cells[BoxLabelRow, 5 + colJumpFactor].Value = row["menu count"];
                        ws.Cells[BoxLabelRow, 6 + colJumpFactor].Value = row["bound"];

                        BoxLabelRow++;

                        if (importedBoxTicketRows.Count() == rowCount)
                        {
                            //ws.Name = lastDate;
                            excelPackage.Save();
                            break;
                        }


                        if (BoxLabelRow == 8)
                            BoxLabelRow++;

                        if (BoxLabelRow == 11)
                        {
                            colJumpFactor += 6;
                            BoxLabelRow = 6;
                        }



                        excelPackage.Save();


                    }


                }
            }
            return blankLabelName;
        }

        private string ProcessLGWJ(ExcelRecordImporter importer, string dataSetName, string blankLabelName)
        {
            OrderManagement orderManagement = new OrderManagement();

            var OrderId = txtBoxOrderId.Text;

            var LotNo = orderManagement.GetLotNoFromOrderId(Convert.ToInt64(txtBoxOrderId.Text));

            var importedBoxTicketRows = importer.Import(dataSetName);

            if (importedBoxTicketRows != null && importedBoxTicketRows.Any())
            {

                blankLabelName = "J MENU WK LGW";

                var blankLabelPath = @"\\nas1\Digital_Production\Virgin\PACKING LABELS BLANK TEMPLATE\" + blankLabelName + ".xlsx";

                var blankLabelPathOrder = @"\\nas1\Digital_Production\Virgin\Packing Label Output\" + OrderId;

                if (!Directory.Exists(blankLabelPathOrder))
                    Directory.CreateDirectory(blankLabelPathOrder);

                blankLabelPathOrder = blankLabelPathOrder + @"\" + blankLabelName + ".xlsx";

                File.Copy(blankLabelPath, blankLabelPathOrder, true);

                ExcelRecordImporter importerBlankLabel = new ExcelRecordImporter(blankLabelPath);


                int i = 1;


                FileInfo finfo = new FileInfo(blankLabelPathOrder);
                var lastDate = "";

                int rowCount = 0;

                int BoxLabelRow = 6;
                int colJumpFactor = 0;

                foreach (var row in importedBoxTicketRows)
                {
                    rowCount++;
                    using (var excelPackage = new ExcelPackage(finfo))
                    {

                        ExcelWorksheet ws = excelPackage.Workbook.Worksheets["date" + i];


                        if (string.IsNullOrEmpty(row["date"]))
                        {
                            i++;
                            ws.Name = lastDate;
                            excelPackage.Save();
                            BoxLabelRow = 6;
                            colJumpFactor = 0;
                            continue;
                        }



                        var date = DateTime.FromOADate(Convert.ToDouble(row["date"]));

                        lastDate = date.ToShortDateString().Replace("/", "-");



                        ws.Cells["F1"].Value = OrderId;
                        ws.Cells["F2"].Value = LotNo;
                        ws.Cells["D3"].Value = "Date " + date.ToShortDateString();


                        ws.Cells["L1"].Value = OrderId;
                        ws.Cells["L2"].Value = LotNo;
                        ws.Cells["J3"].Value = "Date " + date.ToShortDateString();


                        ws.Cells["R1"].Value = OrderId;
                        ws.Cells["R2"].Value = LotNo;
                        ws.Cells["P3"].Value = "Date " + date.ToShortDateString();

                        ws.Cells["X1"].Value = OrderId;
                        ws.Cells["X2"].Value = LotNo;
                        ws.Cells["V3"].Value = "Date " + date.ToShortDateString();

                        ws.Cells["AD1"].Value = OrderId;
                        ws.Cells["AD2"].Value = LotNo;
                        ws.Cells["AB3"].Value = "Date " + date.ToShortDateString();

                        ws.Cells["AJ1"].Value = OrderId;
                        ws.Cells["AJ2"].Value = LotNo;
                        ws.Cells["AH3"].Value = "Date " + date.ToShortDateString();

                        ws.Cells["AP1"].Value = OrderId;
                        ws.Cells["AP2"].Value = LotNo;
                        ws.Cells["AN3"].Value = "Date " + date.ToShortDateString();

                        ws.Cells["AV1"].Value = OrderId;
                        ws.Cells["AV2"].Value = LotNo;
                        ws.Cells["AT3"].Value = "Date " + date.ToShortDateString();

                        ws.Cells["BB1"].Value = OrderId;
                        ws.Cells["BB2"].Value = LotNo;
                        ws.Cells["AZ3"].Value = "Date " + date.ToShortDateString();

                        ws.Cells["BH1"].Value = OrderId;
                        ws.Cells["BH2"].Value = LotNo;
                        ws.Cells["BF3"].Value = "Date " + date.ToShortDateString();

                        ws.Cells["BN1"].Value = OrderId;
                        ws.Cells["BN2"].Value = LotNo;
                        ws.Cells["BL3"].Value = "Date " + date.ToShortDateString();

                        ws.Cells[BoxLabelRow, 1 + colJumpFactor].Value = row["flt no"];
                        ws.Cells[BoxLabelRow, 2 + colJumpFactor].Value = row["route"];
                        ws.Cells[BoxLabelRow, 3 + colJumpFactor].Value = row["dep. time"];
                        ws.Cells[BoxLabelRow, 4 + colJumpFactor].Value = date.ToShortDateString();
                        ws.Cells[BoxLabelRow, 5 + colJumpFactor].Value = row["menu count"];
                        ws.Cells[BoxLabelRow, 6 + colJumpFactor].Value = row["bound"];

                        BoxLabelRow++;

                        if (importedBoxTicketRows.Count() == rowCount)
                        {
                            ws.Name = lastDate;
                            excelPackage.Save();
                            break;
                        }


                        if (BoxLabelRow == 8 || BoxLabelRow == 11 || BoxLabelRow == 14)
                            BoxLabelRow++;

                        if (BoxLabelRow == 17)
                        {
                            colJumpFactor += 6;
                            BoxLabelRow = 6;
                        }



                        excelPackage.Save();


                    }


                }
            }
            return blankLabelName;
        }

        private string ProcessLGWY(ExcelRecordImporter importer, string dataSetName, string blankLabelName)
        {
            OrderManagement orderManagement = new OrderManagement();

            var OrderId = txtBoxOrderId.Text;

            var LotNo = orderManagement.GetLotNoFromOrderId(Convert.ToInt64(txtBoxOrderId.Text));

            var importedBoxTicketRows = importer.Import(dataSetName);

            if (importedBoxTicketRows != null && importedBoxTicketRows.Any())
            {

                blankLabelName = "Y MENU WK LGW";

                var blankLabelPath = @"\\nas1\Digital_Production\Virgin\PACKING LABELS BLANK TEMPLATE\" + blankLabelName + ".xlsx";

                var blankLabelPathOrder = @"\\nas1\Digital_Production\Virgin\Packing Label Output\" + OrderId;

                if (!Directory.Exists(blankLabelPathOrder))
                    Directory.CreateDirectory(blankLabelPathOrder);

                blankLabelPathOrder = blankLabelPathOrder + @"\" + blankLabelName + ".xlsx";

                File.Copy(blankLabelPath, blankLabelPathOrder, true);

                ExcelRecordImporter importerBlankLabel = new ExcelRecordImporter(blankLabelPath);

                int i = 1;


                FileInfo finfo = new FileInfo(blankLabelPathOrder);
                var lastDate = "";

                int rowCount = 0;

                int BoxLabelRow = 6;
                int colJumpFactor = 0;

                int countRow6 = 0;
                int countRow7 = 0;
                int countRow9 = 0;

                foreach (var row in importedBoxTicketRows)
                {
                    if (BoxLabelRow == 6)
                    {
                        var count = 0;

                        try
                        {
                            count = Convert.ToInt32(row["menu count"]);
                        }
                        catch { }

                        countRow6 = count;
                    }

                    if (BoxLabelRow == 7)
                    {
                        var count = 0;

                        try
                        {
                            count = Convert.ToInt32(row["menu count"]);
                        }
                        catch { }

                        countRow7 = count;
                    }

                    if (BoxLabelRow == 9)
                    {
                        var count = 0;

                        try
                        {
                            count = Convert.ToInt32(row["menu count"]);
                        }
                        catch { }

                        countRow9 = count;
                    }


                    if (BoxLabelRow == 9)
                    {
                        bool maxHold = false;

                        if (countRow6 >= 375 && countRow7 >= 375 && countRow9 >= 375)
                            maxHold = true;

                        if (maxHold)
                        {
                            colJumpFactor += 6;
                            BoxLabelRow = 6;
                        }
                    }

                    rowCount++;
                    using (var excelPackage = new ExcelPackage(finfo))
                    {

                        ExcelWorksheet ws = excelPackage.Workbook.Worksheets["date" + i];


                        if (string.IsNullOrEmpty(row["date"]))
                        {
                            i++;
                            ws.Name = lastDate;
                            excelPackage.Save();
                            BoxLabelRow = 6;
                            colJumpFactor = 0;
                            continue;
                        }



                        var date = DateTime.FromOADate(Convert.ToDouble(row["date"]));

                        lastDate = date.ToShortDateString().Replace("/", "-");



                        ws.Cells["F1"].Value = OrderId;
                        ws.Cells["F2"].Value = LotNo;
                        ws.Cells["D3"].Value = "Date " + date.ToShortDateString();


                        ws.Cells["L1"].Value = OrderId;
                        ws.Cells["L2"].Value = LotNo;
                        ws.Cells["J3"].Value = "Date " + date.ToShortDateString();


                        ws.Cells["R1"].Value = OrderId;
                        ws.Cells["R2"].Value = LotNo;
                        ws.Cells["P3"].Value = "Date " + date.ToShortDateString();

                        ws.Cells["X1"].Value = OrderId;
                        ws.Cells["X2"].Value = LotNo;
                        ws.Cells["V3"].Value = "Date " + date.ToShortDateString();

                        ws.Cells["AD1"].Value = OrderId;
                        ws.Cells["AD2"].Value = LotNo;
                        ws.Cells["AB3"].Value = "Date " + date.ToShortDateString();

                        ws.Cells["AJ1"].Value = OrderId;
                        ws.Cells["AJ2"].Value = LotNo;
                        ws.Cells["AH3"].Value = "Date " + date.ToShortDateString();

                        ws.Cells["AP1"].Value = OrderId;
                        ws.Cells["AP2"].Value = LotNo;
                        ws.Cells["AN3"].Value = "Date " + date.ToShortDateString();

                        ws.Cells["AV1"].Value = OrderId;
                        ws.Cells["AV2"].Value = LotNo;
                        ws.Cells["AT3"].Value = "Date " + date.ToShortDateString();

                        ws.Cells["BB1"].Value = OrderId;
                        ws.Cells["BB2"].Value = LotNo;
                        ws.Cells["AZ3"].Value = "Date " + date.ToShortDateString();

                        ws.Cells["BH1"].Value = OrderId;
                        ws.Cells["BH2"].Value = LotNo;
                        ws.Cells["BF3"].Value = "Date " + date.ToShortDateString();

                        ws.Cells["BN1"].Value = OrderId;
                        ws.Cells["BN2"].Value = LotNo;
                        ws.Cells["BL3"].Value = "Date " + date.ToShortDateString();

                        ws.Cells[BoxLabelRow, 1 + colJumpFactor].Value = row["flt no"];
                        ws.Cells[BoxLabelRow, 2 + colJumpFactor].Value = row["route"];
                        ws.Cells[BoxLabelRow, 3 + colJumpFactor].Value = row["dep. time"];
                        ws.Cells[BoxLabelRow, 4 + colJumpFactor].Value = date.ToShortDateString();
                        ws.Cells[BoxLabelRow, 5 + colJumpFactor].Value = row["menu count"];
                        ws.Cells[BoxLabelRow, 6 + colJumpFactor].Value = row["bound"];

                        BoxLabelRow++;

                        if (importedBoxTicketRows.Count() == rowCount)
                        {
                            ws.Name = lastDate;
                            excelPackage.Save();
                            break;
                        }

                        if (BoxLabelRow == 8)
                            BoxLabelRow++;

                        if (BoxLabelRow == 11)
                        {
                            colJumpFactor += 6;
                            BoxLabelRow = 6;
                        }



                        excelPackage.Save();


                    }


                }
            }
            return blankLabelName;
        }

        private string ProcessLGWW(ExcelRecordImporter importer, string dataSetName, string blankLabelName)
        {
            OrderManagement orderManagement = new OrderManagement();

            var OrderId = txtBoxOrderId.Text;

            var LotNo = orderManagement.GetLotNoFromOrderId(Convert.ToInt64(txtBoxOrderId.Text));

            var importedBoxTicketRows = importer.Import(dataSetName);

            if (importedBoxTicketRows != null && importedBoxTicketRows.Any())
            {

                blankLabelName = "W MENU WK LGW";

                var blankLabelPath = @"\\nas1\Digital_Production\Virgin\PACKING LABELS BLANK TEMPLATE\" + blankLabelName + ".xlsx";

                var blankLabelPathOrder = @"\\nas1\Digital_Production\Virgin\Packing Label Output\" + OrderId;

                if (!Directory.Exists(blankLabelPathOrder))
                    Directory.CreateDirectory(blankLabelPathOrder);

                blankLabelPathOrder = blankLabelPathOrder + @"\" + blankLabelName + ".xlsx";

                File.Copy(blankLabelPath, blankLabelPathOrder, true);

                ExcelRecordImporter importerBlankLabel = new ExcelRecordImporter(blankLabelPath);

                int i = 1;


                FileInfo finfo = new FileInfo(blankLabelPathOrder);
                var lastDate = "";

                int rowCount = 0;

                int BoxLabelRow = 6;
                int colJumpFactor = 0;
                int RowJumpFactor = 0;

                foreach (var row in importedBoxTicketRows)
                {
                    rowCount++;
                    using (var excelPackage = new ExcelPackage(finfo))
                    {

                        ExcelWorksheet ws = excelPackage.Workbook.Worksheets["date" + i];


                        if (string.IsNullOrEmpty(row["date"]))
                        {
                            i++;
                            ws.Name = lastDate;
                            excelPackage.Save();
                            BoxLabelRow = 6;
                            colJumpFactor = 0;
                            continue;
                        }


                        var date = DateTime.FromOADate(Convert.ToDouble(row["date"]));

                        lastDate = date.ToShortDateString().Replace("/", "-");



                        ws.Cells["F1"].Value = OrderId;
                        ws.Cells["F2"].Value = LotNo;
                        ws.Cells["D3"].Value = "Date " + date.ToShortDateString();


                        //ws.Cells["L1"].Value = OrderId;
                        //ws.Cells["L2"].Value = LotNo;
                        //ws.Cells["J3"].Value = "Date " + date.ToShortDateString();


                        ws.Cells[BoxLabelRow, 1 + colJumpFactor].Value = row["flt no"];
                        ws.Cells[BoxLabelRow, 2 + colJumpFactor].Value = row["route"];
                        ws.Cells[BoxLabelRow, 3 + colJumpFactor].Value = row["dep. time"];
                        ws.Cells[BoxLabelRow, 4 + colJumpFactor].Value = date.ToShortDateString();
                        ws.Cells[BoxLabelRow, 5 + colJumpFactor].Value = row["menu count"];
                        ws.Cells[BoxLabelRow, 6 + colJumpFactor].Value = row["bound"];

                        BoxLabelRow++;
                        RowJumpFactor++;


                        if (importedBoxTicketRows.Count() == rowCount)
                        {
                            ws.Name = lastDate;
                            excelPackage.Save();
                            break;
                        }


                        if (BoxLabelRow >= 41)
                        {
                            colJumpFactor += 6;
                            BoxLabelRow = 6;
                            RowJumpFactor = 0;
                        }


                        if (RowJumpFactor == 2)
                        {
                            BoxLabelRow++;
                            RowJumpFactor = 0;
                        }




                        excelPackage.Save();


                    }


                }
            }
            return blankLabelName;
        }

        private string ProcessMANJ(ExcelRecordImporter importer, string dataSetName, string blankLabelName)
        {
            OrderManagement orderManagement = new OrderManagement();

            var OrderId = txtBoxOrderId.Text;

            var LotNo = orderManagement.GetLotNoFromOrderId(Convert.ToInt64(txtBoxOrderId.Text));

            var importedBoxTicketRows = importer.Import(dataSetName);

            if (importedBoxTicketRows != null && importedBoxTicketRows.Any())
            {

                blankLabelName = "J MENU WK MAN";

                var blankLabelPath = @"\\nas1\Digital_Production\Virgin\PACKING LABELS BLANK TEMPLATE\" + blankLabelName + ".xlsx";

                var blankLabelPathOrder = @"\\nas1\Digital_Production\Virgin\Packing Label Output\" + OrderId;

                if (!Directory.Exists(blankLabelPathOrder))
                    Directory.CreateDirectory(blankLabelPathOrder);

                blankLabelPathOrder = blankLabelPathOrder + @"\" + blankLabelName + ".xlsx";

                File.Copy(blankLabelPath, blankLabelPathOrder, true);

                ExcelRecordImporter importerBlankLabel = new ExcelRecordImporter(blankLabelPath);


                int i = 1;


                FileInfo finfo = new FileInfo(blankLabelPathOrder);
                var lastDate = "";

                int rowCount = 0;

                int BoxLabelRow = 6;
                int colJumpFactor = 0;

                foreach (var row in importedBoxTicketRows)
                {
                    rowCount++;
                    using (var excelPackage = new ExcelPackage(finfo))
                    {

                        ExcelWorksheet ws = excelPackage.Workbook.Worksheets["date" + i];


                        if (string.IsNullOrEmpty(row["date"]))
                        {
                            i++;
                            ws.Name = lastDate;
                            excelPackage.Save();
                            BoxLabelRow = 6;
                            colJumpFactor = 0;
                            continue;
                        }



                        var date = DateTime.FromOADate(Convert.ToDouble(row["date"]));

                        lastDate = date.ToShortDateString().Replace("/", "-");



                        ws.Cells["F1"].Value = OrderId;
                        ws.Cells["F2"].Value = LotNo;
                        ws.Cells["D3"].Value = "Date " + date.ToShortDateString();


                        ws.Cells["L1"].Value = OrderId;
                        ws.Cells["L2"].Value = LotNo;
                        ws.Cells["J3"].Value = "Date " + date.ToShortDateString();


                        //ws.Cells["R1"].Value = OrderId;
                        //ws.Cells["R2"].Value = LotNo;
                        //ws.Cells["P3"].Value = "Date " + date.ToShortDateString();

                        //ws.Cells["X1"].Value = OrderId;
                        //ws.Cells["X2"].Value = LotNo;
                        //ws.Cells["V3"].Value = "Date " + date.ToShortDateString();

                        //ws.Cells["AD1"].Value = OrderId;
                        //ws.Cells["AD2"].Value = LotNo;
                        //ws.Cells["AB3"].Value = "Date " + date.ToShortDateString();

                        //ws.Cells["AJ1"].Value = OrderId;
                        //ws.Cells["AJ2"].Value = LotNo;
                        //ws.Cells["AH3"].Value = "Date " + date.ToShortDateString();

                        //ws.Cells["AP1"].Value = OrderId;
                        //ws.Cells["AP2"].Value = LotNo;
                        //ws.Cells["AN3"].Value = "Date " + date.ToShortDateString();

                        //ws.Cells["AV1"].Value = OrderId;
                        //ws.Cells["AV2"].Value = LotNo;
                        //ws.Cells["AT3"].Value = "Date " + date.ToShortDateString();

                        //ws.Cells["BB1"].Value = OrderId;
                        //ws.Cells["BB2"].Value = LotNo;
                        //ws.Cells["AZ3"].Value = "Date " + date.ToShortDateString();

                        //ws.Cells["BH1"].Value = OrderId;
                        //ws.Cells["BH2"].Value = LotNo;
                        //ws.Cells["BF3"].Value = "Date " + date.ToShortDateString();

                        //ws.Cells["BN1"].Value = OrderId;
                        //ws.Cells["BN2"].Value = LotNo;
                        //ws.Cells["BL3"].Value = "Date " + date.ToShortDateString();

                        ws.Cells[BoxLabelRow, 1 + colJumpFactor].Value = row["flt no"];
                        ws.Cells[BoxLabelRow, 2 + colJumpFactor].Value = row["route"];
                        ws.Cells[BoxLabelRow, 3 + colJumpFactor].Value = row["dep. time"];
                        ws.Cells[BoxLabelRow, 4 + colJumpFactor].Value = date.ToShortDateString();
                        ws.Cells[BoxLabelRow, 5 + colJumpFactor].Value = row["menu count"];
                        ws.Cells[BoxLabelRow, 6 + colJumpFactor].Value = row["bound"];

                        BoxLabelRow++;

                        if (importedBoxTicketRows.Count() == rowCount)
                        {
                            ws.Name = lastDate;
                            excelPackage.Save();
                            break;
                        }


                        if (BoxLabelRow == 8 || BoxLabelRow == 11 || BoxLabelRow == 14)
                            BoxLabelRow++;

                        if (BoxLabelRow == 17)
                        {
                            colJumpFactor += 6;
                            BoxLabelRow = 6;
                        }



                        excelPackage.Save();


                    }


                }
            }
            return blankLabelName;
        }

        private string ProcessMANW(ExcelRecordImporter importer, string dataSetName, string blankLabelName)
        {
            OrderManagement orderManagement = new OrderManagement();

            var OrderId = txtBoxOrderId.Text;

            var LotNo = orderManagement.GetLotNoFromOrderId(Convert.ToInt64(txtBoxOrderId.Text));

            var importedBoxTicketRows = importer.Import(dataSetName);

            if (importedBoxTicketRows != null && importedBoxTicketRows.Any())
            {

                blankLabelName = "W MENU WK MAN";

                var blankLabelPath = @"\\nas1\Digital_Production\Virgin\PACKING LABELS BLANK TEMPLATE\" + blankLabelName + ".xlsx";

                var blankLabelPathOrder = @"\\nas1\Digital_Production\Virgin\Packing Label Output\" + OrderId;

                if (!Directory.Exists(blankLabelPathOrder))
                    Directory.CreateDirectory(blankLabelPathOrder);

                blankLabelPathOrder = blankLabelPathOrder + @"\" + blankLabelName + ".xlsx";

                File.Copy(blankLabelPath, blankLabelPathOrder, true);

                ExcelRecordImporter importerBlankLabel = new ExcelRecordImporter(blankLabelPath);

                int i = 1;


                FileInfo finfo = new FileInfo(blankLabelPathOrder);
                var lastDate = "";

                int rowCount = 0;

                int BoxLabelRow = 6;
                int colJumpFactor = 0;
                int RowJumpFactor = 0;

                foreach (var row in importedBoxTicketRows)
                {
                    rowCount++;
                    using (var excelPackage = new ExcelPackage(finfo))
                    {

                        ExcelWorksheet ws = excelPackage.Workbook.Worksheets["date" + i];


                        if (string.IsNullOrEmpty(row["date"]))
                        {
                            i++;
                            ws.Name = lastDate;
                            excelPackage.Save();
                            BoxLabelRow = 6;
                            colJumpFactor = 0;
                            continue;
                        }


                        var date = DateTime.FromOADate(Convert.ToDouble(row["date"]));

                        lastDate = date.ToShortDateString().Replace("/", "-");



                        ws.Cells["F1"].Value = OrderId;
                        ws.Cells["F2"].Value = LotNo;
                        ws.Cells["D3"].Value = "Date " + date.ToShortDateString();


                        //ws.Cells["L1"].Value = OrderId;
                        //ws.Cells["L2"].Value = LotNo;
                        //ws.Cells["J3"].Value = "Date " + date.ToShortDateString();


                        //ws.Cells["R1"].Value = OrderId;
                        //ws.Cells["R2"].Value = LotNo;
                        //ws.Cells["P3"].Value = "Date " + date.ToShortDateString();

                        //ws.Cells["X1"].Value = OrderId;
                        //ws.Cells["X2"].Value = LotNo;
                        //ws.Cells["V3"].Value = "Date " + date.ToShortDateString();

                        //ws.Cells["AD1"].Value = OrderId;
                        //ws.Cells["AD2"].Value = LotNo;
                        //ws.Cells["AB3"].Value = "Date " + date.ToShortDateString();

                        //ws.Cells["AJ1"].Value = OrderId;
                        //ws.Cells["AJ2"].Value = LotNo;
                        //ws.Cells["AH3"].Value = "Date " + date.ToShortDateString();

                        //ws.Cells["AP1"].Value = OrderId;
                        //ws.Cells["AP2"].Value = LotNo;
                        //ws.Cells["AN3"].Value = "Date " + date.ToShortDateString();

                        //ws.Cells["AV1"].Value = OrderId;
                        //ws.Cells["AV2"].Value = LotNo;
                        //ws.Cells["AT3"].Value = "Date " + date.ToShortDateString();

                        //ws.Cells["BB1"].Value = OrderId;
                        //ws.Cells["BB2"].Value = LotNo;
                        //ws.Cells["AZ3"].Value = "Date " + date.ToShortDateString();

                        //ws.Cells["BH1"].Value = OrderId;
                        //ws.Cells["BH2"].Value = LotNo;
                        //ws.Cells["BF3"].Value = "Date " + date.ToShortDateString();

                        //ws.Cells["BN1"].Value = OrderId;
                        //ws.Cells["BN2"].Value = LotNo;
                        //ws.Cells["BL3"].Value = "Date " + date.ToShortDateString();

                        ws.Cells[BoxLabelRow, 1 + colJumpFactor].Value = row["flt no"];
                        ws.Cells[BoxLabelRow, 2 + colJumpFactor].Value = row["route"];
                        ws.Cells[BoxLabelRow, 3 + colJumpFactor].Value = row["dep. time"];
                        ws.Cells[BoxLabelRow, 4 + colJumpFactor].Value = date.ToShortDateString();
                        ws.Cells[BoxLabelRow, 5 + colJumpFactor].Value = row["menu count"];
                        ws.Cells[BoxLabelRow, 6 + colJumpFactor].Value = row["bound"];

                        BoxLabelRow++;
                        RowJumpFactor++;

                        if (importedBoxTicketRows.Count() == rowCount)
                        {
                            ws.Name = lastDate;
                            excelPackage.Save();
                            break;
                        }


                        if (BoxLabelRow >= 41)
                        {
                            colJumpFactor += 6;
                            BoxLabelRow = 6;
                            RowJumpFactor = 0;
                        }


                        if (RowJumpFactor == 2)
                        {
                            BoxLabelRow++;
                            RowJumpFactor = 0;
                        }


                        excelPackage.Save();


                    }


                }
            }
            return blankLabelName;
        }

        private string ProcessMANY(ExcelRecordImporter importer, string dataSetName, string blankLabelName)
        {
            OrderManagement orderManagement = new OrderManagement();

            var OrderId = txtBoxOrderId.Text;

            var LotNo = orderManagement.GetLotNoFromOrderId(Convert.ToInt64(txtBoxOrderId.Text));

            var importedBoxTicketRows = importer.Import(dataSetName);

            if (importedBoxTicketRows != null && importedBoxTicketRows.Any())
            {

                blankLabelName = "Y MENU WK MAN";

                var blankLabelPath = @"\\nas1\Digital_Production\Virgin\PACKING LABELS BLANK TEMPLATE\" + blankLabelName + ".xlsx";

                var blankLabelPathOrder = @"\\nas1\Digital_Production\Virgin\Packing Label Output\" + OrderId;

                if (!Directory.Exists(blankLabelPathOrder))
                    Directory.CreateDirectory(blankLabelPathOrder);

                blankLabelPathOrder = blankLabelPathOrder + @"\" + blankLabelName + ".xlsx";

                File.Copy(blankLabelPath, blankLabelPathOrder, true);

                ExcelRecordImporter importerBlankLabel = new ExcelRecordImporter(blankLabelPath);


                int i = 1;


                FileInfo finfo = new FileInfo(blankLabelPathOrder);
                var lastDate = "";

                int rowCount = 0;

                int BoxLabelRow = 6;
                int colJumpFactor = 0;
                int countRow6 = 0;
                int countRow7 = 0;
                int countRow9 = 0;

                foreach (var row in importedBoxTicketRows)
                {
                    if (BoxLabelRow == 6)
                    {
                        var count = 0;

                        try
                        {
                            count = Convert.ToInt32(row["menu count"]);
                        }
                        catch { }

                        countRow6 = count;
                    }

                    if (BoxLabelRow == 7)
                    {
                        var count = 0;

                        try
                        {
                            count = Convert.ToInt32(row["menu count"]);
                        }
                        catch { }

                        countRow7 = count;
                    }

                    if (BoxLabelRow == 9)
                    {
                        var count = 0;

                        try
                        {
                            count = Convert.ToInt32(row["menu count"]);
                        }
                        catch { }

                        countRow9 = count;
                    }


                    if (BoxLabelRow == 9)
                    {
                        bool maxHold = false;

                        if (countRow6 >= 375 && countRow7 >= 375 && countRow9 >= 375)
                            maxHold = true;

                        if (maxHold)
                        {
                            colJumpFactor += 6;
                            BoxLabelRow = 6;
                        }
                    }

                    rowCount++;
                    using (var excelPackage = new ExcelPackage(finfo))
                    {

                        ExcelWorksheet ws = excelPackage.Workbook.Worksheets["date" + i];


                        if (string.IsNullOrEmpty(row["date"]))
                        {
                            i++;
                            ws.Name = lastDate;
                            excelPackage.Save();
                            BoxLabelRow = 6;
                            colJumpFactor = 0;
                            continue;
                        }


                        var date = DateTime.FromOADate(Convert.ToDouble(row["date"]));

                        lastDate = date.ToShortDateString().Replace("/", "-");



                        ws.Cells["F1"].Value = OrderId;
                        ws.Cells["F2"].Value = LotNo;
                        ws.Cells["D3"].Value = "Date " + date.ToShortDateString();


                        ws.Cells["L1"].Value = OrderId;
                        ws.Cells["L2"].Value = LotNo;
                        ws.Cells["J3"].Value = "Date " + date.ToShortDateString();


                        //ws.Cells["R1"].Value = OrderId;
                        //ws.Cells["R2"].Value = LotNo;
                        //ws.Cells["P3"].Value = "Date " + date.ToShortDateString();

                        //ws.Cells["X1"].Value = OrderId;
                        //ws.Cells["X2"].Value = LotNo;
                        //ws.Cells["V3"].Value = "Date " + date.ToShortDateString();

                        //ws.Cells["AD1"].Value = OrderId;
                        //ws.Cells["AD2"].Value = LotNo;
                        //ws.Cells["AB3"].Value = "Date " + date.ToShortDateString();

                        //ws.Cells["AJ1"].Value = OrderId;
                        //ws.Cells["AJ2"].Value = LotNo;
                        //ws.Cells["AH3"].Value = "Date " + date.ToShortDateString();

                        //ws.Cells["AP1"].Value = OrderId;
                        //ws.Cells["AP2"].Value = LotNo;
                        //ws.Cells["AN3"].Value = "Date " + date.ToShortDateString();

                        //ws.Cells["AV1"].Value = OrderId;
                        //ws.Cells["AV2"].Value = LotNo;
                        //ws.Cells["AT3"].Value = "Date " + date.ToShortDateString();

                        //ws.Cells["BB1"].Value = OrderId;
                        //ws.Cells["BB2"].Value = LotNo;
                        //ws.Cells["AZ3"].Value = "Date " + date.ToShortDateString();

                        //ws.Cells["BH1"].Value = OrderId;
                        //ws.Cells["BH2"].Value = LotNo;
                        //ws.Cells["BF3"].Value = "Date " + date.ToShortDateString();

                        //ws.Cells["BN1"].Value = OrderId;
                        //ws.Cells["BN2"].Value = LotNo;
                        //ws.Cells["BL3"].Value = "Date " + date.ToShortDateString();

                        ws.Cells[BoxLabelRow, 1 + colJumpFactor].Value = row["flt no"];
                        ws.Cells[BoxLabelRow, 2 + colJumpFactor].Value = row["route"];
                        ws.Cells[BoxLabelRow, 3 + colJumpFactor].Value = row["dep. time"];
                        ws.Cells[BoxLabelRow, 4 + colJumpFactor].Value = date.ToShortDateString();
                        ws.Cells[BoxLabelRow, 5 + colJumpFactor].Value = row["menu count"];
                        ws.Cells[BoxLabelRow, 6 + colJumpFactor].Value = row["bound"];

                        BoxLabelRow++;

                        if (importedBoxTicketRows.Count() == rowCount)
                        {
                            ws.Name = lastDate;
                            excelPackage.Save();
                            break;
                        }


                        if (BoxLabelRow == 8)
                            BoxLabelRow++;

                        if (BoxLabelRow == 11)
                        {
                            colJumpFactor += 6;
                            BoxLabelRow = 6;
                        }



                        excelPackage.Save();


                    }


                }
            }
            return blankLabelName;
        }

        private string ProcessGLAJ(ExcelRecordImporter importer, string dataSetName, string blankLabelName)
        {
            OrderManagement orderManagement = new OrderManagement();

            var OrderId = txtBoxOrderId.Text;

            var LotNo = orderManagement.GetLotNoFromOrderId(Convert.ToInt64(txtBoxOrderId.Text));

            var importedBoxTicketRows = importer.Import(dataSetName);

            if (importedBoxTicketRows != null && importedBoxTicketRows.Any())
            {

                blankLabelName = "J MENU WK GLA";

                var blankLabelPath = @"\\nas1\Digital_Production\Virgin\PACKING LABELS BLANK TEMPLATE\" + blankLabelName + ".xlsx";

                var blankLabelPathOrder = @"\\nas1\Digital_Production\Virgin\Packing Label Output\" + OrderId;

                if (!Directory.Exists(blankLabelPathOrder))
                    Directory.CreateDirectory(blankLabelPathOrder);

                blankLabelPathOrder = blankLabelPathOrder + @"\" + blankLabelName + ".xlsx";

                File.Copy(blankLabelPath, blankLabelPathOrder, true);

                ExcelRecordImporter importerBlankLabel = new ExcelRecordImporter(blankLabelPath);

                int i = 1;


                FileInfo finfo = new FileInfo(blankLabelPathOrder);
                var lastDate = "";

                int rowCount = 0;

                int BoxLabelRow = 6;
                int colJumpFactor = 0;

                foreach (var row in importedBoxTicketRows)
                {
                    rowCount++;
                    using (var excelPackage = new ExcelPackage(finfo))
                    {

                        ExcelWorksheet ws = excelPackage.Workbook.Worksheets["date" + i];


                        if (string.IsNullOrEmpty(row["date"]))
                        {
                            i++;
                            ws.Name = lastDate;
                            excelPackage.Save();
                            BoxLabelRow = 6;
                            colJumpFactor = 0;
                            continue;
                        }



                        var date = DateTime.FromOADate(Convert.ToDouble(row["date"]));

                        lastDate = date.ToShortDateString().Replace("/", "-");



                        ws.Cells["F1"].Value = OrderId;
                        ws.Cells["F2"].Value = LotNo;
                        ws.Cells["D3"].Value = "Date " + date.ToShortDateString();


                        ws.Cells["L1"].Value = OrderId;
                        ws.Cells["L2"].Value = LotNo;
                        ws.Cells["J3"].Value = "Date " + date.ToShortDateString();


                        //ws.Cells["R1"].Value = OrderId;
                        //ws.Cells["R2"].Value = LotNo;
                        //ws.Cells["P3"].Value = "Date " + date.ToShortDateString();

                        //ws.Cells["X1"].Value = OrderId;
                        //ws.Cells["X2"].Value = LotNo;
                        //ws.Cells["V3"].Value = "Date " + date.ToShortDateString();

                        //ws.Cells["AD1"].Value = OrderId;
                        //ws.Cells["AD2"].Value = LotNo;
                        //ws.Cells["AB3"].Value = "Date " + date.ToShortDateString();

                        //ws.Cells["AJ1"].Value = OrderId;
                        //ws.Cells["AJ2"].Value = LotNo;
                        //ws.Cells["AH3"].Value = "Date " + date.ToShortDateString();

                        //ws.Cells["AP1"].Value = OrderId;
                        //ws.Cells["AP2"].Value = LotNo;
                        //ws.Cells["AN3"].Value = "Date " + date.ToShortDateString();

                        //ws.Cells["AV1"].Value = OrderId;
                        //ws.Cells["AV2"].Value = LotNo;
                        //ws.Cells["AT3"].Value = "Date " + date.ToShortDateString();

                        //ws.Cells["BB1"].Value = OrderId;
                        //ws.Cells["BB2"].Value = LotNo;
                        //ws.Cells["AZ3"].Value = "Date " + date.ToShortDateString();

                        //ws.Cells["BH1"].Value = OrderId;
                        //ws.Cells["BH2"].Value = LotNo;
                        //ws.Cells["BF3"].Value = "Date " + date.ToShortDateString();

                        //ws.Cells["BN1"].Value = OrderId;
                        //ws.Cells["BN2"].Value = LotNo;
                        //ws.Cells["BL3"].Value = "Date " + date.ToShortDateString();

                        ws.Cells[BoxLabelRow, 1 + colJumpFactor].Value = row["flt no"];
                        ws.Cells[BoxLabelRow, 2 + colJumpFactor].Value = row["route"];
                        ws.Cells[BoxLabelRow, 3 + colJumpFactor].Value = row["dep. time"];
                        ws.Cells[BoxLabelRow, 4 + colJumpFactor].Value = date.ToShortDateString();
                        ws.Cells[BoxLabelRow, 5 + colJumpFactor].Value = row["menu count"];
                        ws.Cells[BoxLabelRow, 6 + colJumpFactor].Value = row["bound"];

                        if (importedBoxTicketRows.Count() == rowCount)
                        {
                            ws.Name = lastDate;
                            excelPackage.Save();
                            break;
                        }

                        BoxLabelRow++;

                        if (BoxLabelRow == 8 || BoxLabelRow == 11 || BoxLabelRow == 14)
                            BoxLabelRow++;

                        if (BoxLabelRow == 17)
                        {
                            colJumpFactor += 6;
                            BoxLabelRow = 6;
                        }



                        excelPackage.Save();


                    }


                }
            }
            return blankLabelName;
        }

        private string ProcessGLAW(ExcelRecordImporter importer, string dataSetName, string blankLabelName)
        {
            OrderManagement orderManagement = new OrderManagement();

            var OrderId = txtBoxOrderId.Text;

            var LotNo = orderManagement.GetLotNoFromOrderId(Convert.ToInt64(txtBoxOrderId.Text));

            var importedBoxTicketRows = importer.Import(dataSetName);

            if (importedBoxTicketRows != null && importedBoxTicketRows.Any())
            {

                blankLabelName = "W MENU WK GLA";

                var blankLabelPath = @"\\nas1\Digital_Production\Virgin\PACKING LABELS BLANK TEMPLATE\" + blankLabelName + ".xlsx";

                var blankLabelPathOrder = @"\\nas1\Digital_Production\Virgin\Packing Label Output\" + OrderId;

                if (!Directory.Exists(blankLabelPathOrder))
                    Directory.CreateDirectory(blankLabelPathOrder);

                blankLabelPathOrder = blankLabelPathOrder + @"\" + blankLabelName + ".xlsx";

                File.Copy(blankLabelPath, blankLabelPathOrder, true);

                ExcelRecordImporter importerBlankLabel = new ExcelRecordImporter(blankLabelPath);

                int i = 1;


                FileInfo finfo = new FileInfo(blankLabelPathOrder);
                var lastDate = "";

                int rowCount = 0;

                int BoxLabelRow = 6;
                int colJumpFactor = 0;
                int RowJumpFactor = 0;

                foreach (var row in importedBoxTicketRows)
                {
                    rowCount++;
                    using (var excelPackage = new ExcelPackage(finfo))
                    {

                        ExcelWorksheet ws = excelPackage.Workbook.Worksheets["date" + i];


                        if (string.IsNullOrEmpty(row["date"]))
                        {
                            i++;
                            ws.Name = lastDate;
                            excelPackage.Save();
                            BoxLabelRow = 6;
                            colJumpFactor = 0;
                            RowJumpFactor = 0;
                            continue;
                        }


                        var date = DateTime.FromOADate(Convert.ToDouble(row["date"]));

                        lastDate = date.ToShortDateString().Replace("/", "-");



                        ws.Cells["F1"].Value = OrderId;
                        ws.Cells["F2"].Value = LotNo;
                        ws.Cells["D3"].Value = "Date " + date.ToShortDateString();


                        //ws.Cells["L1"].Value = OrderId;
                        //ws.Cells["L2"].Value = LotNo;
                        //ws.Cells["J3"].Value = "Date " + date.ToShortDateString();


                        //ws.Cells["R1"].Value = OrderId;
                        //ws.Cells["R2"].Value = LotNo;
                        //ws.Cells["P3"].Value = "Date " + date.ToShortDateString();

                        //ws.Cells["X1"].Value = OrderId;
                        //ws.Cells["X2"].Value = LotNo;
                        //ws.Cells["V3"].Value = "Date " + date.ToShortDateString();

                        //ws.Cells["AD1"].Value = OrderId;
                        //ws.Cells["AD2"].Value = LotNo;
                        //ws.Cells["AB3"].Value = "Date " + date.ToShortDateString();

                        //ws.Cells["AJ1"].Value = OrderId;
                        //ws.Cells["AJ2"].Value = LotNo;
                        //ws.Cells["AH3"].Value = "Date " + date.ToShortDateString();

                        //ws.Cells["AP1"].Value = OrderId;
                        //ws.Cells["AP2"].Value = LotNo;
                        //ws.Cells["AN3"].Value = "Date " + date.ToShortDateString();

                        //ws.Cells["AV1"].Value = OrderId;
                        //ws.Cells["AV2"].Value = LotNo;
                        //ws.Cells["AT3"].Value = "Date " + date.ToShortDateString();

                        //ws.Cells["BB1"].Value = OrderId;
                        //ws.Cells["BB2"].Value = LotNo;
                        //ws.Cells["AZ3"].Value = "Date " + date.ToShortDateString();

                        //ws.Cells["BH1"].Value = OrderId;
                        //ws.Cells["BH2"].Value = LotNo;
                        //ws.Cells["BF3"].Value = "Date " + date.ToShortDateString();

                        //ws.Cells["BN1"].Value = OrderId;
                        //ws.Cells["BN2"].Value = LotNo;
                        //ws.Cells["BL3"].Value = "Date " + date.ToShortDateString();

                        ws.Cells[BoxLabelRow, 1 + colJumpFactor].Value = row["flt no"];
                        ws.Cells[BoxLabelRow, 2 + colJumpFactor].Value = row["route"];
                        ws.Cells[BoxLabelRow, 3 + colJumpFactor].Value = row["dep. time"];
                        ws.Cells[BoxLabelRow, 4 + colJumpFactor].Value = date.ToShortDateString();
                        ws.Cells[BoxLabelRow, 5 + colJumpFactor].Value = row["menu count"];
                        ws.Cells[BoxLabelRow, 6 + colJumpFactor].Value = row["bound"];

                        BoxLabelRow++;
                        RowJumpFactor++;


                        if (importedBoxTicketRows.Count() == rowCount)
                        {
                            ws.Name = lastDate;
                            excelPackage.Save();
                            break;
                        }

                        if (BoxLabelRow >= 41)
                        {
                            colJumpFactor += 6;
                            BoxLabelRow = 6;
                            RowJumpFactor = 0;
                        }


                        if (RowJumpFactor == 2)
                        {
                            BoxLabelRow++;
                            RowJumpFactor = 0;
                        }



                        excelPackage.Save();


                    }


                }
            }
            return blankLabelName;
        }

        private string ProcessGLAY(ExcelRecordImporter importer, string dataSetName, string blankLabelName)
        {
            OrderManagement orderManagement = new OrderManagement();

            var OrderId = txtBoxOrderId.Text;

            var LotNo = orderManagement.GetLotNoFromOrderId(Convert.ToInt64(txtBoxOrderId.Text));

            var importedBoxTicketRows = importer.Import(dataSetName);

            if (importedBoxTicketRows != null && importedBoxTicketRows.Any())
            {

                blankLabelName = "Y MENU WK GLA";

                var blankLabelPath = @"\\nas1\Digital_Production\Virgin\PACKING LABELS BLANK TEMPLATE\" + blankLabelName + ".xlsx";

                var blankLabelPathOrder = @"\\nas1\Digital_Production\Virgin\Packing Label Output\" + OrderId;

                if (!Directory.Exists(blankLabelPathOrder))
                    Directory.CreateDirectory(blankLabelPathOrder);

                blankLabelPathOrder = blankLabelPathOrder + @"\" + blankLabelName + ".xlsx";

                File.Copy(blankLabelPath, blankLabelPathOrder, true);

                ExcelRecordImporter importerBlankLabel = new ExcelRecordImporter(blankLabelPath);

                int i = 1;


                FileInfo finfo = new FileInfo(blankLabelPathOrder);
                var lastDate = "";

                int rowCount = 0;

                int BoxLabelRow = 6;
                int colJumpFactor = 0;
                int countRow6 = 0;
                int countRow7 = 0;
                int countRow9 = 0;

                foreach (var row in importedBoxTicketRows)
                {
                    if (BoxLabelRow == 6)
                    {
                        var count = 0;

                        try
                        {
                            count = Convert.ToInt32(row["menu count"]);
                        }
                        catch { }

                        countRow6 = count;
                    }

                    if (BoxLabelRow == 7)
                    {
                        var count = 0;

                        try
                        {
                            count = Convert.ToInt32(row["menu count"]);
                        }
                        catch { }

                        countRow7 = count;
                    }

                    if (BoxLabelRow == 9)
                    {
                        var count = 0;

                        try
                        {
                            count = Convert.ToInt32(row["menu count"]);
                        }
                        catch { }

                        countRow9 = count;
                    }


                    if (BoxLabelRow == 9)
                    {
                        bool maxHold = false;

                        if (countRow6 >= 375 && countRow7 >= 375 && countRow9 >= 375)
                            maxHold = true;

                        if (maxHold)
                        {
                            colJumpFactor += 6;
                            BoxLabelRow = 6;
                        }
                    }

                    rowCount++;
                    using (var excelPackage = new ExcelPackage(finfo))
                    {

                        ExcelWorksheet ws = excelPackage.Workbook.Worksheets["date" + i];


                        if (string.IsNullOrEmpty(row["date"]))
                        {
                            i++;
                            ws.Name = lastDate;
                            excelPackage.Save();
                            BoxLabelRow = 6;
                            colJumpFactor = 0;
                            continue;
                        }



                        var date = DateTime.FromOADate(Convert.ToDouble(row["date"]));

                        lastDate = date.ToShortDateString().Replace("/", "-");



                        ws.Cells["F1"].Value = OrderId;
                        ws.Cells["F2"].Value = LotNo;
                        ws.Cells["D3"].Value = "Date " + date.ToShortDateString();


                        ws.Cells["L1"].Value = OrderId;
                        ws.Cells["L2"].Value = LotNo;
                        ws.Cells["J3"].Value = "Date " + date.ToShortDateString();


                        //ws.Cells["R1"].Value = OrderId;
                        //ws.Cells["R2"].Value = LotNo;
                        //ws.Cells["P3"].Value = "Date " + date.ToShortDateString();

                        //ws.Cells["X1"].Value = OrderId;
                        //ws.Cells["X2"].Value = LotNo;
                        //ws.Cells["V3"].Value = "Date " + date.ToShortDateString();

                        //ws.Cells["AD1"].Value = OrderId;
                        //ws.Cells["AD2"].Value = LotNo;
                        //ws.Cells["AB3"].Value = "Date " + date.ToShortDateString();

                        //ws.Cells["AJ1"].Value = OrderId;
                        //ws.Cells["AJ2"].Value = LotNo;
                        //ws.Cells["AH3"].Value = "Date " + date.ToShortDateString();

                        //ws.Cells["AP1"].Value = OrderId;
                        //ws.Cells["AP2"].Value = LotNo;
                        //ws.Cells["AN3"].Value = "Date " + date.ToShortDateString();

                        //ws.Cells["AV1"].Value = OrderId;
                        //ws.Cells["AV2"].Value = LotNo;
                        //ws.Cells["AT3"].Value = "Date " + date.ToShortDateString();

                        //ws.Cells["BB1"].Value = OrderId;
                        //ws.Cells["BB2"].Value = LotNo;
                        //ws.Cells["AZ3"].Value = "Date " + date.ToShortDateString();

                        //ws.Cells["BH1"].Value = OrderId;
                        //ws.Cells["BH2"].Value = LotNo;
                        //ws.Cells["BF3"].Value = "Date " + date.ToShortDateString();

                        //ws.Cells["BN1"].Value = OrderId;
                        //ws.Cells["BN2"].Value = LotNo;
                        //ws.Cells["BL3"].Value = "Date " + date.ToShortDateString();

                        ws.Cells[BoxLabelRow, 1 + colJumpFactor].Value = row["flt no"];
                        ws.Cells[BoxLabelRow, 2 + colJumpFactor].Value = row["route"];
                        ws.Cells[BoxLabelRow, 3 + colJumpFactor].Value = row["dep. time"];
                        ws.Cells[BoxLabelRow, 4 + colJumpFactor].Value = date.ToShortDateString();
                        ws.Cells[BoxLabelRow, 5 + colJumpFactor].Value = row["menu count"];
                        ws.Cells[BoxLabelRow, 6 + colJumpFactor].Value = row["bound"];

                        if (importedBoxTicketRows.Count() == rowCount)
                        {
                            ws.Name = lastDate;
                            excelPackage.Save();
                            break;
                        }

                        BoxLabelRow++;

                        if (BoxLabelRow == 8)
                            BoxLabelRow++;

                        if (BoxLabelRow == 11)
                        {
                            colJumpFactor += 6;
                            BoxLabelRow = 6;
                        }



                        excelPackage.Save();


                    }


                }
            }
            return blankLabelName;
        }

        private void btnGeneratePackingData_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtBoxOrderId.Text))
            {
                MessageBox.Show("Please enter Order Id!");
                return;
            }

            btnGeneratePackingData.Enabled = false;

            Thread thread = new Thread(new ThreadStart(GeneratePackingData));
            thread.Start();

        }

        private void GeneratePackingData()
        {
            MessageBox.Show("Packing data Generation is in progress and you will be notified once process is complete!");
            this.InvokeEx(f => f.lblStatus.Visible = true);
            this.InvokeEx(f => f.lblStatus.Text = "Box Report generation is in progress, please wait");

            this.InvokeEx(f => f.btnGenerateOrderPDF.Enabled = false);
            this.InvokeEx(f => f.btnGeneratePackingData.Enabled = false);

            this.InvokeEx(f => f.btnGenerate.Enabled = false);
            this.InvokeEx(f => f.btnPackingLabels.Enabled = false);
            this.InvokeEx(f => f.btnAddFlights.Enabled = false);
            this.InvokeEx(f => f.btnReset.Enabled = false);


            try
            {
                var OrderId = txtBoxOrderId.Text;

                if (!string.IsNullOrEmpty(OrderId))
                {
                    PackingTicketProcessor packingTicket = new PackingTicketProcessor();
                    packingTicket.CalculateBoxTicketData(Convert.ToInt64(OrderId));

                    //MessageBox.Show("Packing data generation is complete, please download the Box Ticket Excel from VAA site");
                    this.InvokeEx(f => f.lblStatus.Text = "Box Report Generation completed successfully!");

                    var orderManagement = new OrderManagement();

                    //get the order Id of current row
                    var lotNo = orderManagement.GetLotNoFromOrderId(Convert.ToInt64(OrderId));

                    var BoxReportPath = @"\\nas1\Digital_Production\Virgin\BoxReport\BoxReport_" + txtBoxOrderId.Text + ".xlsx";


                    var boxTicketData = orderManagement.GetBoxTicketData(Convert.ToInt64(OrderId));
                    GenerateOutputSpreadsheet.CreateBoxTicketSpreadSheet(boxTicketData, Convert.ToInt64(OrderId), lotNo, BoxReportPath);

                    Process.Start(BoxReportPath);

                    this.InvokeEx(f => f.lblStatus.Visible = true);
                    this.InvokeEx(f => f.btnView.Visible = false);
                    this.InvokeEx(f => f.btnGenerateOrderPDF.Enabled = true);
                    this.InvokeEx(f => f.btnGeneratePackingData.Enabled = true);
                    this.InvokeEx(f => f.btnGenerate.Enabled = true);
                    this.InvokeEx(f => f.btnPackingLabels.Enabled = true);
                    this.InvokeEx(f => f.btnAddFlights.Enabled = true);
                    this.InvokeEx(f => f.btnReset.Enabled = true);


                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Packing data generation is failed, please contact the developers");
                this.InvokeEx(f => f.lblStatus.Text = "Packing data Generation falied!");
                this.InvokeEx(f => f.btnView.Visible = false);
                this.InvokeEx(f => f.lblStatus.Visible = true);
                this.InvokeEx(f => f.btnGenerateOrderPDF.Enabled = true);
                this.InvokeEx(f => f.btnGenerate.Enabled = true);
                this.InvokeEx(f => f.btnPackingLabels.Enabled = true);
                this.InvokeEx(f => f.btnAddFlights.Enabled = true);
                this.InvokeEx(f => f.btnReset.Enabled = true);
                this.InvokeEx(f => f.btnGeneratePackingData.Enabled = true);
            }
        }

        private void btnAddFlights_Click(object sender, EventArgs e)
        {
            AddFlightsToMenu addToMenuForm = new AddFlightsToMenu();

            addToMenuForm.Show();

        }

        private void btnView_Click(object sender, EventArgs e)
        {
            var viewText = btnView.Text;


            if (viewText == "View Packing PDFs")
            {
                Process.Start("explorer.exe", @"\\nas1\Digital_Production\Virgin\Emma\BoxTickets\Output\");
            }

            if (viewText == "View PDFs")
            {
                Process.Start("explorer.exe", @"\\nas1\Digital_Production\Virgin\Emma\MENU PDFS\" + txtBoxOrderId.Text);
            }

            if (viewText == "View Packing Labels")
            {

                Process.Start("explorer.exe", @"\\nas1\Digital_Production\Virgin\Packing Label Output\" + txtBoxOrderId.Text);

            }

        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            txtBoxOrderId.Text = "";
            btnView.Visible = false;
            lblStatus.Text = "";
            btnGenerateOrderPDF.Enabled = true;
            btnGenerate.Enabled = true;
            btnPackingLabels.Enabled = true;
            btnAddFlights.Enabled = true;


        }

        private void BtnValidateSP_Click(object sender, EventArgs e)
        {

            frmValidateServicePlan validateSP = new frmValidateServicePlan();

            validateSP.Show();
        }

        /// <summary>
        /// show the flight schedule form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFlightSchedule_Click(object sender, EventArgs e)
        {
            ChangeFlightSchedule flightSchedulefrm = new ChangeFlightSchedule();

            flightSchedulefrm.Show();
        }

        private void btnUpdateMenuCode_Click(object sender, EventArgs e)
        {
            UpdateMenuCode updateMenuCode = new UpdateMenuCode();
            updateMenuCode.Show();
        }

        private void btnCloneMenusToNewCycle_Click(object sender, EventArgs e)
        {
            CloneMenus frmCloneMenus = new CloneMenus();
            frmCloneMenus.Show();
        }

        private void btnGenerateUnOrderMenuPDFs_Click(object sender, EventArgs e)
        {
            PDFGenerationByMenuCodes frmunOrderedMenus = new PDFGenerationByMenuCodes();
            frmunOrderedMenus.Show();
        }
    }

    public static class ISynchronizeInvokeExtensions
    {
        public static void InvokeEx<T>(this T @this, Action<T> action) where T : ISynchronizeInvoke
        {
            if (@this.InvokeRequired)
            {
                @this.Invoke(action, new object[] { @this });
            }
            else
            {
                action(@this);
            }
        }
    }
}
