using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VAA.CommonComponents;

namespace PDFProcessingVAA
{
    public partial class frmValidateServicePlan : Form
    {
        public frmValidateServicePlan()
        {
            InitializeComponent();
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                this.Close();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void btnInputOpen_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();

            DialogResult result = folderBrowserDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                txtBoxInput.Text = folderBrowserDialog.SelectedPath;
            }
        }

        private void btnValdiate_Click(object sender, EventArgs e)
        {
            string[] files = Directory.GetFiles(txtBoxInput.Text);

            var resultFinal = string.Empty;

            foreach (var file in files)
            {
                //read each file

                //validate
                var result = string.Empty;

                Stream servicePlanstream = new MemoryStream(File.ReadAllBytes(file));

                var validInput = true;

                if (!Helper.IsValidExcelFormat(servicePlanstream))
                {
                    validInput = false;
                }

                if (!validInput)
                {
                    result += file + " : Invalid File format" + Environment.NewLine;
                    result += "-------------------------------------------------" + Environment.NewLine;
                }
                else
                {

                    using (var package = new ExcelPackage(servicePlanstream))
                    {
                        ExcelWorkbook workbook = package.Workbook;

                        ExcelWorksheet currentWorksheet = workbook.Worksheets.First();

                        int rows = currentWorksheet.Dimension.End.Row;

                        int columns = currentWorksheet.Dimension.End.Column;

                        for (int i = 2; i <= rows; i++)
                        {
                            for (int j = 1; j <= columns; j++)
                            {
                                ExcelRange theCell = currentWorksheet.Cells[i, j];

                                if (theCell.Style.Fill.PatternType == ExcelFillStyle.Solid && theCell.Style.Fill.BackgroundColor.Indexed == 8  && theCell.Style.Fill.PatternColor.Indexed==0)
                                {
                                    String getValue = theCell.Value.ToString();

                                    if (!string.IsNullOrEmpty(getValue))
                                    {
                                        if (getValue.ToUpper() != "NV")
                                        {
                                            result += "Invalid value in Cell (" + i + "," + j + ") - " + getValue + Environment.NewLine;
                                        }
                                    }
                                }
                            }
                        }
                    }


                    if (string.IsNullOrEmpty(result))
                        resultFinal += file + " : VALID" + Environment.NewLine;
                    else
                    {
                        resultFinal += file + " : INVALID" + Environment.NewLine;
                        resultFinal += result + Environment.NewLine;
                    }
                    resultFinal += "-------------------------------------------------" + Environment.NewLine;
                }


                //write to error log
            }

            richTextBoxResult.Text = resultFinal;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtBoxInput.Clear();
            lblProcessing.Text = "";
            richTextBoxResult.Clear();
        }
    }
}
