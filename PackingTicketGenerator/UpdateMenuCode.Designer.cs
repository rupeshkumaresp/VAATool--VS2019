namespace PDFProcessingVAA
{
    partial class UpdateMenuCode
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
            this.lblMenucodes = new System.Windows.Forms.Label();
            this.richTextBoxMenucodes = new System.Windows.Forms.RichTextBox();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.lblHeader = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblMenucodes
            // 
            this.lblMenucodes.AutoSize = true;
            this.lblMenucodes.Location = new System.Drawing.Point(13, 46);
            this.lblMenucodes.Name = "lblMenucodes";
            this.lblMenucodes.Size = new System.Drawing.Size(63, 13);
            this.lblMenucodes.TabIndex = 0;
            this.lblMenucodes.Text = "Menucodes";
            // 
            // richTextBoxMenucodes
            // 
            this.richTextBoxMenucodes.Location = new System.Drawing.Point(116, 46);
            this.richTextBoxMenucodes.Name = "richTextBoxMenucodes";
            this.richTextBoxMenucodes.Size = new System.Drawing.Size(354, 251);
            this.richTextBoxMenucodes.TabIndex = 1;
            this.richTextBoxMenucodes.Text = "";
            // 
            // btnUpdate
            // 
            this.btnUpdate.Location = new System.Drawing.Point(395, 326);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(75, 23);
            this.btnUpdate.TabIndex = 2;
            this.btnUpdate.Text = "Update";
            this.btnUpdate.UseVisualStyleBackColor = true;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // lblHeader
            // 
            this.lblHeader.AutoSize = true;
            this.lblHeader.Location = new System.Drawing.Point(12, 9);
            this.lblHeader.Name = "lblHeader";
            this.lblHeader.Size = new System.Drawing.Size(411, 13);
            this.lblHeader.TabIndex = 4;
            this.lblHeader.Text = "Please enter comma separated menu codes to update the menu code in chili template" +
    "";
            // 
            // UpdateMenuCode
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(522, 391);
            this.Controls.Add(this.lblHeader);
            this.Controls.Add(this.btnUpdate);
            this.Controls.Add(this.richTextBoxMenucodes);
            this.Controls.Add(this.lblMenucodes);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "UpdateMenuCode";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Update Menu Code";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblMenucodes;
        private System.Windows.Forms.RichTextBox richTextBoxMenucodes;
        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.Label lblHeader;
    }
}