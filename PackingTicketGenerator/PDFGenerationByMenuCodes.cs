using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using VAA.BusinessComponents;
using VAA.DataAccess;

namespace PDFProcessingVAA
{
    public partial class PDFGenerationByMenuCodes : Form
    {
        MenuManagement _menuManagement = new MenuManagement();
        MenuProcessor _menuProcessor = new MenuProcessor();

        public PDFGenerationByMenuCodes()
        {
            InitializeComponent();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(richTextBoxMenucodes.Text))
                return;

            var menuCode = richTextBoxMenucodes.Text;

            lblStatus.Visible = true;

            Thread thread = new Thread(() => GeneratePDF(menuCode));

            thread.Start();


        }

        protected void GeneratePDF(string menuCode)
        {
            var codes = menuCode.Split(new char[] { ',' });

            ChiliProcessor chili = new ChiliProcessor();

            var tempPath = @"\\192.168.16.208\Digital_Production\Virgin\Emma\MENU PDFS\99999\";

            if (Directory.Exists(tempPath))
                Directory.Delete(tempPath, true);

            Directory.CreateDirectory(tempPath);

            this.InvokeEx(f => f.lblStatus.Text = "In Progress, Please Wait...");

            for (int i = 0; i < codes.Length; i++)
            {
                if (string.IsNullOrEmpty(codes[i]))
                    continue;

               // this.InvokeEx(f => f.lblStatus.Text = "PDFs generation in Progress for: " + codes[i]);


                var menudata = _menuManagement.GetMenuByMenuCode(codes[i].Trim());

                _menuProcessor.RebuildFlightNumberLotNumberChiliVariableForMenu(Convert.ToInt64(menudata.Id));

                var newDOc = chili.CloneChiliDocument(Convert.ToInt64(menudata.Id));

                chili.UpdateChiliDocumentVariablesallowServerRendering(1, Convert.ToInt64(menudata.Id), newDOc);

                _menuProcessor.GeneratePdfForMenu(Convert.ToInt64(menudata.CycleId), Convert.ToInt64(menudata.Id), 99999, newDOc);
            }

            this.InvokeEx(f => f.lblStatus.Text = "PDFs generated successfully!");

            Process.Start("explorer.exe", tempPath);


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

    }
}
