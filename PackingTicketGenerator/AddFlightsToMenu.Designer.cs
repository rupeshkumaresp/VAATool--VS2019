namespace PDFProcessingVAA
{
    partial class AddFlightsToMenu
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
            this.label1 = new System.Windows.Forms.Label();
            this.txtBoxMenuCode = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtBoxDepartureCode = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtBoxArrivalAirportCode = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtBoxFlightNumber = new System.Windows.Forms.TextBox();
            this.btnAddToMenu = new System.Windows.Forms.Button();
            this.cmbOperation = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(36, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "MenuCode";
            // 
            // txtBoxMenuCode
            // 
            this.txtBoxMenuCode.Location = new System.Drawing.Point(156, 26);
            this.txtBoxMenuCode.Name = "txtBoxMenuCode";
            this.txtBoxMenuCode.Size = new System.Drawing.Size(164, 20);
            this.txtBoxMenuCode.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(36, 67);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(114, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Departure airport Code";
            // 
            // txtBoxDepartureCode
            // 
            this.txtBoxDepartureCode.Location = new System.Drawing.Point(156, 64);
            this.txtBoxDepartureCode.Name = "txtBoxDepartureCode";
            this.txtBoxDepartureCode.Size = new System.Drawing.Size(164, 20);
            this.txtBoxDepartureCode.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(36, 103);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(96, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Arrival airport Code";
            // 
            // txtBoxArrivalAirportCode
            // 
            this.txtBoxArrivalAirportCode.Location = new System.Drawing.Point(156, 103);
            this.txtBoxArrivalAirportCode.Name = "txtBoxArrivalAirportCode";
            this.txtBoxArrivalAirportCode.Size = new System.Drawing.Size(164, 20);
            this.txtBoxArrivalAirportCode.TabIndex = 5;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(36, 138);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(72, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Flight Number";
            // 
            // txtBoxFlightNumber
            // 
            this.txtBoxFlightNumber.Location = new System.Drawing.Point(156, 138);
            this.txtBoxFlightNumber.Name = "txtBoxFlightNumber";
            this.txtBoxFlightNumber.Size = new System.Drawing.Size(164, 20);
            this.txtBoxFlightNumber.TabIndex = 7;
            // 
            // btnAddToMenu
            // 
            this.btnAddToMenu.Location = new System.Drawing.Point(169, 209);
            this.btnAddToMenu.Name = "btnAddToMenu";
            this.btnAddToMenu.Size = new System.Drawing.Size(102, 23);
            this.btnAddToMenu.TabIndex = 8;
            this.btnAddToMenu.Text = "Submit";
            this.btnAddToMenu.UseVisualStyleBackColor = true;
            this.btnAddToMenu.Click += new System.EventHandler(this.btnAddToMenu_Click);
            // 
            // cmbOperation
            // 
            this.cmbOperation.FormattingEnabled = true;
            this.cmbOperation.Items.AddRange(new object[] {
            "ADD FLIGHT",
            "REMOVE FLIGHT"});
            this.cmbOperation.Location = new System.Drawing.Point(156, 165);
            this.cmbOperation.Name = "cmbOperation";
            this.cmbOperation.Size = new System.Drawing.Size(164, 21);
            this.cmbOperation.TabIndex = 9;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(36, 173);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 13);
            this.label5.TabIndex = 10;
            this.label5.Text = "Operation";
            // 
            // AddFlightsToMenu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(422, 271);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.cmbOperation);
            this.Controls.Add(this.btnAddToMenu);
            this.Controls.Add(this.txtBoxFlightNumber);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtBoxArrivalAirportCode);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtBoxDepartureCode);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtBoxMenuCode);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AddFlightsToMenu";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Add/Remove Flight To Menu";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtBoxMenuCode;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtBoxDepartureCode;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtBoxArrivalAirportCode;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtBoxFlightNumber;
        private System.Windows.Forms.Button btnAddToMenu;
        private System.Windows.Forms.ComboBox cmbOperation;
        private System.Windows.Forms.Label label5;
    }
}