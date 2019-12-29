namespace PDFProcessingVAA
{
    partial class frmValidateServicePlan
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
            this.groupBoxServicePlan = new System.Windows.Forms.GroupBox();
            this.richTextBoxResult = new System.Windows.Forms.RichTextBox();
            this.btnInputOpen = new System.Windows.Forms.Button();
            this.lblProcessing = new System.Windows.Forms.Label();
            this.btnValdiate = new System.Windows.Forms.Button();
            this.lblInput = new System.Windows.Forms.Label();
            this.txtBoxInput = new System.Windows.Forms.TextBox();
            this.btnClear = new System.Windows.Forms.Button();
            this.groupBoxServicePlan.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxServicePlan
            // 
            this.groupBoxServicePlan.Controls.Add(this.btnClear);
            this.groupBoxServicePlan.Controls.Add(this.richTextBoxResult);
            this.groupBoxServicePlan.Controls.Add(this.btnInputOpen);
            this.groupBoxServicePlan.Controls.Add(this.lblProcessing);
            this.groupBoxServicePlan.Controls.Add(this.btnValdiate);
            this.groupBoxServicePlan.Controls.Add(this.lblInput);
            this.groupBoxServicePlan.Controls.Add(this.txtBoxInput);
            this.groupBoxServicePlan.Location = new System.Drawing.Point(12, 21);
            this.groupBoxServicePlan.Name = "groupBoxServicePlan";
            this.groupBoxServicePlan.Size = new System.Drawing.Size(465, 363);
            this.groupBoxServicePlan.TabIndex = 8;
            this.groupBoxServicePlan.TabStop = false;
            this.groupBoxServicePlan.Text = "Validate Service Plans";
            // 
            // richTextBoxResult
            // 
            this.richTextBoxResult.Location = new System.Drawing.Point(7, 154);
            this.richTextBoxResult.Name = "richTextBoxResult";
            this.richTextBoxResult.Size = new System.Drawing.Size(452, 203);
            this.richTextBoxResult.TabIndex = 9;
            this.richTextBoxResult.Text = "";
            // 
            // btnInputOpen
            // 
            this.btnInputOpen.Location = new System.Drawing.Point(354, 55);
            this.btnInputOpen.Name = "btnInputOpen";
            this.btnInputOpen.Size = new System.Drawing.Size(96, 23);
            this.btnInputOpen.TabIndex = 8;
            this.btnInputOpen.Text = "Browse folder";
            this.btnInputOpen.UseVisualStyleBackColor = true;
            this.btnInputOpen.Click += new System.EventHandler(this.btnInputOpen_Click);
            // 
            // lblProcessing
            // 
            this.lblProcessing.AutoSize = true;
            this.lblProcessing.Location = new System.Drawing.Point(173, 138);
            this.lblProcessing.Name = "lblProcessing";
            this.lblProcessing.Size = new System.Drawing.Size(59, 13);
            this.lblProcessing.TabIndex = 7;
            this.lblProcessing.Text = "***status***";
            // 
            // btnValdiate
            // 
            this.btnValdiate.Location = new System.Drawing.Point(142, 98);
            this.btnValdiate.Name = "btnValdiate";
            this.btnValdiate.Size = new System.Drawing.Size(75, 23);
            this.btnValdiate.TabIndex = 0;
            this.btnValdiate.Text = "Validate";
            this.btnValdiate.UseVisualStyleBackColor = true;
            this.btnValdiate.Click += new System.EventHandler(this.btnValdiate_Click);
            // 
            // lblInput
            // 
            this.lblInput.AutoSize = true;
            this.lblInput.Location = new System.Drawing.Point(6, 32);
            this.lblInput.Name = "lblInput";
            this.lblInput.Size = new System.Drawing.Size(92, 13);
            this.lblInput.TabIndex = 3;
            this.lblInput.Text = "Service Plan Path";
            // 
            // txtBoxInput
            // 
            this.txtBoxInput.Location = new System.Drawing.Point(104, 29);
            this.txtBoxInput.Name = "txtBoxInput";
            this.txtBoxInput.Size = new System.Drawing.Size(346, 20);
            this.txtBoxInput.TabIndex = 4;
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(246, 98);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(75, 23);
            this.btnClear.TabIndex = 10;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // frmValidateServicePlan
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(489, 396);
            this.Controls.Add(this.groupBoxServicePlan);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "frmValidateServicePlan";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Validate Service Plan";
            this.groupBoxServicePlan.ResumeLayout(false);
            this.groupBoxServicePlan.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxServicePlan;
        private System.Windows.Forms.Button btnInputOpen;
        private System.Windows.Forms.Label lblProcessing;
        private System.Windows.Forms.Button btnValdiate;
        private System.Windows.Forms.Label lblInput;
        private System.Windows.Forms.TextBox txtBoxInput;
        private System.Windows.Forms.RichTextBox richTextBoxResult;
        private System.Windows.Forms.Button btnClear;
    }
}