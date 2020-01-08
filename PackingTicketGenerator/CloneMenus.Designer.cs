namespace PDFProcessingVAA
{
    partial class CloneMenus
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
            this.lblFromCycle = new System.Windows.Forms.Label();
            this.cmdFromCycle = new System.Windows.Forms.ComboBox();
            this.lblWeekNo = new System.Windows.Forms.Label();
            this.cmbWeekNo = new System.Windows.Forms.ComboBox();
            this.lblToCycle = new System.Windows.Forms.Label();
            this.cmbToCycle = new System.Windows.Forms.ComboBox();
            this.btnSubmit = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.richTextBoxMenucodes = new System.Windows.Forms.RichTextBox();
            this.lblMenucodes = new System.Windows.Forms.Label();
            this.labelOR = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblFromCycle
            // 
            this.lblFromCycle.AutoSize = true;
            this.lblFromCycle.Location = new System.Drawing.Point(73, 41);
            this.lblFromCycle.Name = "lblFromCycle";
            this.lblFromCycle.Size = new System.Drawing.Size(121, 13);
            this.lblFromCycle.TabIndex = 0;
            this.lblFromCycle.Text = "Clone Menus from Cycle";
            // 
            // cmdFromCycle
            // 
            this.cmdFromCycle.FormattingEnabled = true;
            this.cmdFromCycle.Location = new System.Drawing.Point(254, 38);
            this.cmdFromCycle.Name = "cmdFromCycle";
            this.cmdFromCycle.Size = new System.Drawing.Size(244, 21);
            this.cmdFromCycle.TabIndex = 1;
            // 
            // lblWeekNo
            // 
            this.lblWeekNo.AutoSize = true;
            this.lblWeekNo.Location = new System.Drawing.Point(73, 129);
            this.lblWeekNo.Name = "lblWeekNo";
            this.lblWeekNo.Size = new System.Drawing.Size(76, 13);
            this.lblWeekNo.TabIndex = 2;
            this.lblWeekNo.Text = "Week Number";
            // 
            // cmbWeekNo
            // 
            this.cmbWeekNo.FormattingEnabled = true;
            this.cmbWeekNo.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12"});
            this.cmbWeekNo.Location = new System.Drawing.Point(254, 126);
            this.cmbWeekNo.Name = "cmbWeekNo";
            this.cmbWeekNo.Size = new System.Drawing.Size(244, 21);
            this.cmbWeekNo.TabIndex = 3;
            // 
            // lblToCycle
            // 
            this.lblToCycle.AutoSize = true;
            this.lblToCycle.Location = new System.Drawing.Point(73, 82);
            this.lblToCycle.Name = "lblToCycle";
            this.lblToCycle.Size = new System.Drawing.Size(114, 13);
            this.lblToCycle.TabIndex = 4;
            this.lblToCycle.Text = "Clone Menus To Cycle";
            // 
            // cmbToCycle
            // 
            this.cmbToCycle.FormattingEnabled = true;
            this.cmbToCycle.Location = new System.Drawing.Point(254, 82);
            this.cmbToCycle.Name = "cmbToCycle";
            this.cmbToCycle.Size = new System.Drawing.Size(244, 21);
            this.cmbToCycle.TabIndex = 5;
            // 
            // btnSubmit
            // 
            this.btnSubmit.Location = new System.Drawing.Point(311, 367);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(75, 23);
            this.btnSubmit.TabIndex = 6;
            this.btnSubmit.Text = "Submit";
            this.btnSubmit.UseVisualStyleBackColor = true;
            this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(251, 393);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(126, 13);
            this.lblStatus.TabIndex = 7;
            this.lblStatus.Text = "In Progress, Please Wait!";
            this.lblStatus.Visible = false;
            // 
            // richTextBoxMenucodes
            // 
            this.richTextBoxMenucodes.Location = new System.Drawing.Point(254, 194);
            this.richTextBoxMenucodes.Name = "richTextBoxMenucodes";
            this.richTextBoxMenucodes.Size = new System.Drawing.Size(354, 164);
            this.richTextBoxMenucodes.TabIndex = 8;
            this.richTextBoxMenucodes.Text = "";
            // 
            // lblMenucodes
            // 
            this.lblMenucodes.AutoSize = true;
            this.lblMenucodes.Location = new System.Drawing.Point(73, 197);
            this.lblMenucodes.Name = "lblMenucodes";
            this.lblMenucodes.Size = new System.Drawing.Size(153, 13);
            this.lblMenucodes.TabIndex = 9;
            this.lblMenucodes.Text = "Menucodes(comma seperated)";
            // 
            // labelOR
            // 
            this.labelOR.AutoSize = true;
            this.labelOR.Location = new System.Drawing.Point(212, 165);
            this.labelOR.Name = "labelOR";
            this.labelOR.Size = new System.Drawing.Size(23, 13);
            this.labelOR.TabIndex = 10;
            this.labelOR.Text = "OR";
            // 
            // CloneMenus
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(619, 430);
            this.Controls.Add(this.labelOR);
            this.Controls.Add(this.lblMenucodes);
            this.Controls.Add(this.richTextBoxMenucodes);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.btnSubmit);
            this.Controls.Add(this.cmbToCycle);
            this.Controls.Add(this.lblToCycle);
            this.Controls.Add(this.cmbWeekNo);
            this.Controls.Add(this.lblWeekNo);
            this.Controls.Add(this.cmdFromCycle);
            this.Controls.Add(this.lblFromCycle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "CloneMenus";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Clone Menus from one cycle to another cycle";
            this.Load += new System.EventHandler(this.CloneMenus_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblFromCycle;
        private System.Windows.Forms.ComboBox cmdFromCycle;
        private System.Windows.Forms.Label lblWeekNo;
        private System.Windows.Forms.ComboBox cmbWeekNo;
        private System.Windows.Forms.Label lblToCycle;
        private System.Windows.Forms.ComboBox cmbToCycle;
        private System.Windows.Forms.Button btnSubmit;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.RichTextBox richTextBoxMenucodes;
        private System.Windows.Forms.Label lblMenucodes;
        private System.Windows.Forms.Label labelOR;
    }
}