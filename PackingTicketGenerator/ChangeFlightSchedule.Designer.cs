namespace PDFProcessingVAA
{
    partial class ChangeFlightSchedule
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnDownloadSchedule = new System.Windows.Forms.Button();
            this.groupBoxUpload = new System.Windows.Forms.GroupBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.txtBoxFileName = new System.Windows.Forms.TextBox();
            this.lblNewSchedule = new System.Windows.Forms.Label();
            this.btnUploadSchedule = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.groupBoxUpload.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnDownloadSchedule
            // 
            this.btnDownloadSchedule.Location = new System.Drawing.Point(103, 12);
            this.btnDownloadSchedule.Name = "btnDownloadSchedule";
            this.btnDownloadSchedule.Size = new System.Drawing.Size(162, 23);
            this.btnDownloadSchedule.TabIndex = 0;
            this.btnDownloadSchedule.Text = "Download Flight Schedule";
            this.btnDownloadSchedule.UseVisualStyleBackColor = true;
            this.btnDownloadSchedule.Click += new System.EventHandler(this.btnDownloadSchedule_Click);
            // 
            // groupBoxUpload
            // 
            this.groupBoxUpload.Controls.Add(this.btnBrowse);
            this.groupBoxUpload.Controls.Add(this.txtBoxFileName);
            this.groupBoxUpload.Controls.Add(this.lblNewSchedule);
            this.groupBoxUpload.Controls.Add(this.btnUploadSchedule);
            this.groupBoxUpload.Location = new System.Drawing.Point(12, 124);
            this.groupBoxUpload.Name = "groupBoxUpload";
            this.groupBoxUpload.Size = new System.Drawing.Size(389, 195);
            this.groupBoxUpload.TabIndex = 1;
            this.groupBoxUpload.TabStop = false;
            this.groupBoxUpload.Text = "Upload Modified/New Schedule";
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(322, 49);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(61, 23);
            this.btnBrowse.TabIndex = 5;
            this.btnBrowse.Text = "browse";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // txtBoxFileName
            // 
            this.txtBoxFileName.Location = new System.Drawing.Point(10, 51);
            this.txtBoxFileName.Name = "txtBoxFileName";
            this.txtBoxFileName.Size = new System.Drawing.Size(306, 20);
            this.txtBoxFileName.TabIndex = 4;
            // 
            // lblNewSchedule
            // 
            this.lblNewSchedule.AutoSize = true;
            this.lblNewSchedule.Location = new System.Drawing.Point(7, 35);
            this.lblNewSchedule.Name = "lblNewSchedule";
            this.lblNewSchedule.Size = new System.Drawing.Size(77, 13);
            this.lblNewSchedule.TabIndex = 3;
            this.lblNewSchedule.Text = "New Schedule";
            // 
            // btnUploadSchedule
            // 
            this.btnUploadSchedule.Location = new System.Drawing.Point(189, 152);
            this.btnUploadSchedule.Name = "btnUploadSchedule";
            this.btnUploadSchedule.Size = new System.Drawing.Size(162, 23);
            this.btnUploadSchedule.TabIndex = 2;
            this.btnUploadSchedule.Text = "Upload Flight Schedule";
            this.btnUploadSchedule.UseVisualStyleBackColor = true;
            this.btnUploadSchedule.Click += new System.EventHandler(this.btnUploadSchedule_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatus.ForeColor = System.Drawing.Color.Chocolate;
            this.lblStatus.Location = new System.Drawing.Point(99, 61);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(0, 20);
            this.lblStatus.TabIndex = 2;
            // 
            // ChangeFlightSchedule
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(426, 331);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.groupBoxUpload);
            this.Controls.Add(this.btnDownloadSchedule);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "ChangeFlightSchedule";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Change Flight Schedule";
            this.groupBoxUpload.ResumeLayout(false);
            this.groupBoxUpload.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnDownloadSchedule;
        private System.Windows.Forms.GroupBox groupBoxUpload;
        private System.Windows.Forms.Button btnUploadSchedule;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.TextBox txtBoxFileName;
        private System.Windows.Forms.Label lblNewSchedule;
        private System.Windows.Forms.Label lblStatus;
    }
}