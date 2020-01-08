using OutputSpreadsheetWriterLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using VAA.BusinessComponents;

namespace PDFProcessingVAA
{
    public partial class ChangeFlightSchedule : Form
    {
        FlightScheduleEngine flightScheduleEngine = new FlightScheduleEngine();

        public ChangeFlightSchedule()
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

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            DialogResult result = openFileDialog.ShowDialog(); // Show the dialog.

            if (result == DialogResult.OK)
            {
                txtBoxFileName.Text = openFileDialog.FileName;
            }
        }

        private void btnDownloadSchedule_Click(object sender, EventArgs e)
        {
            Thread thread = new Thread(new ThreadStart(DownloadSchedule));
            thread.Start();

        }

        private void DownloadSchedule()
        {

            this.InvokeEx(f => f.lblStatus.Visible = true);
            this.InvokeEx(f => f.btnDownloadSchedule.Enabled = false);
            this.InvokeEx(f => f.groupBoxUpload.Enabled = false);
            this.InvokeEx(f => f.lblStatus.Text = "In Progress, please wait!");


            var schedule = flightScheduleEngine.GetFlightSchedule();

            GenerateOutputSpreadsheet.CreateFlightScheduleSpreadSheet(schedule, @"\\192.168.16.208\Digital_Production\Virgin\FlightSchedule");

            this.InvokeEx(f => f.btnDownloadSchedule.Enabled = true);
            this.InvokeEx(f => f.lblStatus.Visible = false);
            this.InvokeEx(f => f.groupBoxUpload.Enabled = true);
        }

        private void btnUploadSchedule_Click(object sender, EventArgs e)
        {
            Thread thread = new Thread(new ThreadStart(UploadSchedule));
          


            thread.Start();
        }


        private bool ValidateSchedule()
        {
            bool valid = true;
            FlightScheduleEngine scheduleEngine = new FlightScheduleEngine();

            //validate before upload
            Stream stream = System.IO.File.OpenRead(txtBoxFileName.Text);

            var missingEquipmentCode = scheduleEngine.ValidateSchedulePlan(stream);

            if (missingEquipmentCode.Count > 0)
            {
                //Loop with comma separated value
                var missingCode = string.Empty;
                for (int j = 0; j < missingEquipmentCode.Count; j++)
                {
                    missingCode += missingEquipmentCode[j] + ",";
                }


                MessageBox.Show("Invalid schedule - Missing Equipments: " + missingCode);                

                valid = false;
            }
            return valid;
        }

        private void UploadSchedule()
        {
            try
            {
                this.InvokeEx(f => f.btnDownloadSchedule.Enabled = false);
                this.InvokeEx(f => f.groupBoxUpload.Enabled = false);
                this.InvokeEx(f => f.lblStatus.Text = "In Progress, please wait!");


                bool valid = ValidateSchedule();

                if (!valid)
                {
                    this.InvokeEx(f => f.btnDownloadSchedule.Enabled = true);
                    this.InvokeEx(f => f.groupBoxUpload.Enabled = true);
                    this.InvokeEx(f => f.lblStatus.Visible = false);
                    return;
                }

                Stream stream = System.IO.File.OpenRead(txtBoxFileName.Text);
                flightScheduleEngine.UploadFlightSchedule(stream, true);

                MessageBox.Show("Upload completed successfully");
                this.InvokeEx(f => f.btnDownloadSchedule.Enabled = true);
                this.InvokeEx(f => f.groupBoxUpload.Enabled = true);
                this.InvokeEx(f => f.lblStatus.Visible = false);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Upload failed - please contact applaud");

                this.InvokeEx(f => f.btnDownloadSchedule.Enabled = true);
                this.InvokeEx(f => f.groupBoxUpload.Enabled = true);
                this.InvokeEx(f => f.lblStatus.Visible = false);
            }
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
