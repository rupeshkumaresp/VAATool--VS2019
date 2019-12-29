namespace PDFProcessingVAA
{
    partial class PDFGeneration
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
            this.lblMenuType = new System.Windows.Forms.Label();
            this.cmbMenuType = new System.Windows.Forms.ComboBox();
            this.btnGenerateOrderPDF = new System.Windows.Forms.Button();
            this.groupBoxPDF = new System.Windows.Forms.GroupBox();
            this.lblStatus = new System.Windows.Forms.Label();
            this.btnView = new System.Windows.Forms.Button();
            this.lblOrderId = new System.Windows.Forms.Label();
            this.txtBoxOrderId = new System.Windows.Forms.TextBox();
            this.groupBoxPDF.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblMenuType
            // 
            this.lblMenuType.AutoSize = true;
            this.lblMenuType.Location = new System.Drawing.Point(58, 171);
            this.lblMenuType.Name = "lblMenuType";
            this.lblMenuType.Size = new System.Drawing.Size(61, 13);
            this.lblMenuType.TabIndex = 0;
            this.lblMenuType.Text = "Menu Type";
            // 
            // cmbMenuType
            // 
            this.cmbMenuType.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbMenuType.FormattingEnabled = true;
            this.cmbMenuType.Items.AddRange(new object[] {
            "All",
            "Food Guide",
            "Upper Class Breakfast Card",
            "Upper Class Main menu",
            "Premium Economy Main menu",
            "Economy Main Menu"});
            this.cmbMenuType.Location = new System.Drawing.Point(144, 163);
            this.cmbMenuType.Name = "cmbMenuType";
            this.cmbMenuType.Size = new System.Drawing.Size(226, 24);
            this.cmbMenuType.TabIndex = 1;
            // 
            // btnGenerateOrderPDF
            // 
            this.btnGenerateOrderPDF.Location = new System.Drawing.Point(190, 208);
            this.btnGenerateOrderPDF.Name = "btnGenerateOrderPDF";
            this.btnGenerateOrderPDF.Size = new System.Drawing.Size(75, 23);
            this.btnGenerateOrderPDF.TabIndex = 2;
            this.btnGenerateOrderPDF.Text = "Generate";
            this.btnGenerateOrderPDF.UseVisualStyleBackColor = true;
            this.btnGenerateOrderPDF.Click += new System.EventHandler(this.btnGenerate_Click);
            // 
            // groupBoxPDF
            // 
            this.groupBoxPDF.Controls.Add(this.lblStatus);
            this.groupBoxPDF.Controls.Add(this.btnView);
            this.groupBoxPDF.Controls.Add(this.lblOrderId);
            this.groupBoxPDF.Controls.Add(this.txtBoxOrderId);
            this.groupBoxPDF.Controls.Add(this.lblMenuType);
            this.groupBoxPDF.Controls.Add(this.btnGenerateOrderPDF);
            this.groupBoxPDF.Controls.Add(this.cmbMenuType);
            this.groupBoxPDF.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBoxPDF.Location = new System.Drawing.Point(12, 13);
            this.groupBoxPDF.Name = "groupBoxPDF";
            this.groupBoxPDF.Size = new System.Drawing.Size(420, 268);
            this.groupBoxPDF.TabIndex = 3;
            this.groupBoxPDF.TabStop = false;
            this.groupBoxPDF.Text = "Select Menu Type and Generate PDF";
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatus.ForeColor = System.Drawing.Color.Green;
            this.lblStatus.Location = new System.Drawing.Point(58, 32);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(159, 13);
            this.lblStatus.TabIndex = 8;
            this.lblStatus.Text = "In Progress, Please Wait...";
            this.lblStatus.Visible = false;
            // 
            // btnView
            // 
            this.btnView.BackColor = System.Drawing.Color.Khaki;
            this.btnView.ForeColor = System.Drawing.Color.Black;
            this.btnView.Location = new System.Drawing.Point(250, 57);
            this.btnView.Name = "btnView";
            this.btnView.Size = new System.Drawing.Size(144, 23);
            this.btnView.TabIndex = 9;
            this.btnView.Text = "View";
            this.btnView.UseVisualStyleBackColor = false;
            this.btnView.Visible = false;
            this.btnView.Click += new System.EventHandler(this.btnView_Click);
            // 
            // lblOrderId
            // 
            this.lblOrderId.AutoSize = true;
            this.lblOrderId.Location = new System.Drawing.Point(58, 130);
            this.lblOrderId.Name = "lblOrderId";
            this.lblOrderId.Size = new System.Drawing.Size(42, 13);
            this.lblOrderId.TabIndex = 3;
            this.lblOrderId.Text = "OrderId";
            // 
            // txtBoxOrderId
            // 
            this.txtBoxOrderId.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBoxOrderId.Location = new System.Drawing.Point(144, 123);
            this.txtBoxOrderId.Name = "txtBoxOrderId";
            this.txtBoxOrderId.Size = new System.Drawing.Size(226, 23);
            this.txtBoxOrderId.TabIndex = 4;
            // 
            // PDFGeneration
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(444, 293);
            this.Controls.Add(this.groupBoxPDF);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "PDFGeneration";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Generation PDFs";
            this.groupBoxPDF.ResumeLayout(false);
            this.groupBoxPDF.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblMenuType;
        private System.Windows.Forms.ComboBox cmbMenuType;
        private System.Windows.Forms.Button btnGenerateOrderPDF;
        private System.Windows.Forms.GroupBox groupBoxPDF;
        private System.Windows.Forms.Label lblOrderId;
        private System.Windows.Forms.TextBox txtBoxOrderId;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Button btnView;
    }
}