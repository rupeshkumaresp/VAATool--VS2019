﻿namespace PackingTicketGenerator
{
    partial class Action
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
            this.lblOrderId = new System.Windows.Forms.Label();
            this.txtBoxOrderId = new System.Windows.Forms.TextBox();
            this.btnGenerate = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.btnGenerateOrderPDF = new System.Windows.Forms.Button();
            this.btnPackingLabels = new System.Windows.Forms.Button();
            this.btnGeneratePackingData = new System.Windows.Forms.Button();
            this.btnAddFlights = new System.Windows.Forms.Button();
            this.btnView = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            this.grpBoxVAA = new System.Windows.Forms.GroupBox();
            this.btnGenerateUnOrderMenuPDFs = new System.Windows.Forms.Button();
            this.btnCloneMenusToNewCycle = new System.Windows.Forms.Button();
            this.btnUpdateMenuCode = new System.Windows.Forms.Button();
            this.btnFlightSchedule = new System.Windows.Forms.Button();
            this.BtnValidateSP = new System.Windows.Forms.Button();
            this.grpBoxVAA.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblOrderId
            // 
            this.lblOrderId.AutoSize = true;
            this.lblOrderId.Location = new System.Drawing.Point(70, 75);
            this.lblOrderId.Name = "lblOrderId";
            this.lblOrderId.Size = new System.Drawing.Size(42, 13);
            this.lblOrderId.TabIndex = 0;
            this.lblOrderId.Text = "OrderId";
            // 
            // txtBoxOrderId
            // 
            this.txtBoxOrderId.Location = new System.Drawing.Point(165, 75);
            this.txtBoxOrderId.Name = "txtBoxOrderId";
            this.txtBoxOrderId.Size = new System.Drawing.Size(163, 20);
            this.txtBoxOrderId.TabIndex = 1;
            // 
            // btnGenerate
            // 
            this.btnGenerate.Location = new System.Drawing.Point(114, 216);
            this.btnGenerate.Name = "btnGenerate";
            this.btnGenerate.Size = new System.Drawing.Size(163, 34);
            this.btnGenerate.TabIndex = 6;
            this.btnGenerate.Text = "Generate Packing PDFs";
            this.btnGenerate.UseVisualStyleBackColor = true;
            this.btnGenerate.Click += new System.EventHandler(this.btnGenerate_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatus.ForeColor = System.Drawing.Color.Green;
            this.lblStatus.Location = new System.Drawing.Point(181, 16);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(159, 13);
            this.lblStatus.TabIndex = 3;
            this.lblStatus.Text = "In Progress, Please Wait...";
            this.lblStatus.Visible = false;
            // 
            // btnGenerateOrderPDF
            // 
            this.btnGenerateOrderPDF.Location = new System.Drawing.Point(349, 282);
            this.btnGenerateOrderPDF.Name = "btnGenerateOrderPDF";
            this.btnGenerateOrderPDF.Size = new System.Drawing.Size(163, 37);
            this.btnGenerateOrderPDF.TabIndex = 3;
            this.btnGenerateOrderPDF.Text = "Generate Order MENU PDF";
            this.btnGenerateOrderPDF.UseVisualStyleBackColor = true;
            this.btnGenerateOrderPDF.Click += new System.EventHandler(this.btnGenerateOrderPDF_Click);
            // 
            // btnPackingLabels
            // 
            this.btnPackingLabels.Location = new System.Drawing.Point(349, 144);
            this.btnPackingLabels.Name = "btnPackingLabels";
            this.btnPackingLabels.Size = new System.Drawing.Size(163, 33);
            this.btnPackingLabels.TabIndex = 5;
            this.btnPackingLabels.Text = "Generate Packing Labels";
            this.btnPackingLabels.UseVisualStyleBackColor = true;
            this.btnPackingLabels.Click += new System.EventHandler(this.btnPackingLabels_Click);
            // 
            // btnGeneratePackingData
            // 
            this.btnGeneratePackingData.Location = new System.Drawing.Point(114, 144);
            this.btnGeneratePackingData.Name = "btnGeneratePackingData";
            this.btnGeneratePackingData.Size = new System.Drawing.Size(163, 33);
            this.btnGeneratePackingData.TabIndex = 4;
            this.btnGeneratePackingData.Text = "Generate Box Report";
            this.btnGeneratePackingData.UseVisualStyleBackColor = true;
            this.btnGeneratePackingData.Click += new System.EventHandler(this.btnGeneratePackingData_Click);
            // 
            // btnAddFlights
            // 
            this.btnAddFlights.Location = new System.Drawing.Point(349, 421);
            this.btnAddFlights.Name = "btnAddFlights";
            this.btnAddFlights.Size = new System.Drawing.Size(163, 34);
            this.btnAddFlights.TabIndex = 2;
            this.btnAddFlights.Text = "Add/Remove Flights in Menu";
            this.btnAddFlights.UseVisualStyleBackColor = true;
            this.btnAddFlights.Click += new System.EventHandler(this.btnAddFlights_Click);
            // 
            // btnView
            // 
            this.btnView.BackColor = System.Drawing.Color.Khaki;
            this.btnView.ForeColor = System.Drawing.Color.Black;
            this.btnView.Location = new System.Drawing.Point(368, 38);
            this.btnView.Name = "btnView";
            this.btnView.Size = new System.Drawing.Size(144, 23);
            this.btnView.TabIndex = 7;
            this.btnView.Text = "View";
            this.btnView.UseVisualStyleBackColor = false;
            this.btnView.Visible = false;
            this.btnView.Click += new System.EventHandler(this.btnView_Click);
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(114, 479);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(163, 29);
            this.btnReset.TabIndex = 7;
            this.btnReset.Text = "Reset";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // grpBoxVAA
            // 
            this.grpBoxVAA.Controls.Add(this.btnGenerateUnOrderMenuPDFs);
            this.grpBoxVAA.Controls.Add(this.btnCloneMenusToNewCycle);
            this.grpBoxVAA.Controls.Add(this.btnUpdateMenuCode);
            this.grpBoxVAA.Controls.Add(this.btnFlightSchedule);
            this.grpBoxVAA.Controls.Add(this.BtnValidateSP);
            this.grpBoxVAA.Controls.Add(this.lblStatus);
            this.grpBoxVAA.Controls.Add(this.btnReset);
            this.grpBoxVAA.Controls.Add(this.btnView);
            this.grpBoxVAA.Controls.Add(this.btnGenerate);
            this.grpBoxVAA.Controls.Add(this.btnPackingLabels);
            this.grpBoxVAA.Controls.Add(this.btnGeneratePackingData);
            this.grpBoxVAA.Controls.Add(this.btnAddFlights);
            this.grpBoxVAA.Controls.Add(this.lblOrderId);
            this.grpBoxVAA.Controls.Add(this.btnGenerateOrderPDF);
            this.grpBoxVAA.Controls.Add(this.txtBoxOrderId);
            this.grpBoxVAA.Location = new System.Drawing.Point(4, 4);
            this.grpBoxVAA.Name = "grpBoxVAA";
            this.grpBoxVAA.Size = new System.Drawing.Size(668, 645);
            this.grpBoxVAA.TabIndex = 8;
            this.grpBoxVAA.TabStop = false;
            this.grpBoxVAA.Text = "VAA Tasks";
            // 
            // btnGenerateUnOrderMenuPDFs
            // 
            this.btnGenerateUnOrderMenuPDFs.Location = new System.Drawing.Point(349, 350);
            this.btnGenerateUnOrderMenuPDFs.Name = "btnGenerateUnOrderMenuPDFs";
            this.btnGenerateUnOrderMenuPDFs.Size = new System.Drawing.Size(163, 32);
            this.btnGenerateUnOrderMenuPDFs.TabIndex = 12;
            this.btnGenerateUnOrderMenuPDFs.Text = "UnOrdered Menu PDFs";
            this.btnGenerateUnOrderMenuPDFs.UseVisualStyleBackColor = true;
            this.btnGenerateUnOrderMenuPDFs.Click += new System.EventHandler(this.btnGenerateUnOrderMenuPDFs_Click);
            // 
            // btnCloneMenusToNewCycle
            // 
            this.btnCloneMenusToNewCycle.Location = new System.Drawing.Point(114, 421);
            this.btnCloneMenusToNewCycle.Name = "btnCloneMenusToNewCycle";
            this.btnCloneMenusToNewCycle.Size = new System.Drawing.Size(163, 33);
            this.btnCloneMenusToNewCycle.TabIndex = 11;
            this.btnCloneMenusToNewCycle.Text = "Clone Menus to New Cycle";
            this.btnCloneMenusToNewCycle.UseVisualStyleBackColor = true;
            this.btnCloneMenusToNewCycle.Click += new System.EventHandler(this.btnCloneMenusToNewCycle_Click);
            // 
            // btnUpdateMenuCode
            // 
            this.btnUpdateMenuCode.Location = new System.Drawing.Point(114, 350);
            this.btnUpdateMenuCode.Name = "btnUpdateMenuCode";
            this.btnUpdateMenuCode.Size = new System.Drawing.Size(163, 32);
            this.btnUpdateMenuCode.TabIndex = 10;
            this.btnUpdateMenuCode.Text = "Update Menucode in Chili";
            this.btnUpdateMenuCode.UseVisualStyleBackColor = true;
            this.btnUpdateMenuCode.Click += new System.EventHandler(this.btnUpdateMenuCode_Click);
            // 
            // btnFlightSchedule
            // 
            this.btnFlightSchedule.Location = new System.Drawing.Point(114, 282);
            this.btnFlightSchedule.Name = "btnFlightSchedule";
            this.btnFlightSchedule.Size = new System.Drawing.Size(163, 35);
            this.btnFlightSchedule.TabIndex = 9;
            this.btnFlightSchedule.Text = "Change Flight Schedule";
            this.btnFlightSchedule.UseVisualStyleBackColor = true;
            this.btnFlightSchedule.Click += new System.EventHandler(this.btnFlightSchedule_Click);
            // 
            // BtnValidateSP
            // 
            this.BtnValidateSP.Location = new System.Drawing.Point(349, 216);
            this.BtnValidateSP.Name = "BtnValidateSP";
            this.BtnValidateSP.Size = new System.Drawing.Size(163, 34);
            this.BtnValidateSP.TabIndex = 8;
            this.BtnValidateSP.Text = "Validate Service Plan";
            this.BtnValidateSP.UseVisualStyleBackColor = true;
            this.BtnValidateSP.Click += new System.EventHandler(this.BtnValidateSP_Click);
            // 
            // Action
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(684, 661);
            this.Controls.Add(this.grpBoxVAA);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(700, 700);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(700, 700);
            this.Name = "Action";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Emma Management Tool";
            this.grpBoxVAA.ResumeLayout(false);
            this.grpBoxVAA.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblOrderId;
        private System.Windows.Forms.TextBox txtBoxOrderId;
        private System.Windows.Forms.Button btnGenerate;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Button btnGenerateOrderPDF;
        private System.Windows.Forms.Button btnPackingLabels;
        private System.Windows.Forms.Button btnGeneratePackingData;
        private System.Windows.Forms.Button btnAddFlights;
        private System.Windows.Forms.Button btnView;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.GroupBox grpBoxVAA;
        private System.Windows.Forms.Button BtnValidateSP;
        private System.Windows.Forms.Button btnFlightSchedule;
        private System.Windows.Forms.Button btnUpdateMenuCode;
        private System.Windows.Forms.Button btnCloneMenusToNewCycle;
        private System.Windows.Forms.Button btnGenerateUnOrderMenuPDFs;
    }
}
