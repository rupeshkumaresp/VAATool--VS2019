using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VAA.BusinessComponents;
using VAA.DataAccess;

namespace PDFProcessingVAA
{
    public partial class UpdateMenuCode : Form
    {
        MenuManagement _menuManagement = new MenuManagement();
        MenuProcessor _menuProcessor = new MenuProcessor();

        public UpdateMenuCode()
        {
            InitializeComponent();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(richTextBoxMenucodes.Text))
                return;

              var menuCode = richTextBoxMenucodes.Text;

                var codes = menuCode.Split(new char[] { ',' });

                for (int i = 0; i < codes.Length; i++)
                {
                    if (string.IsNullOrEmpty(codes[i]))
                        continue;

                    var menudata = _menuManagement.GetMenuByMenuCode(codes[i].Trim());

                    _menuProcessor.RebuildFlightNumberLotNumberChiliVariableForMenu(menudata.Id);
                }

            MessageBox.Show("Menucode in chili document has been updated successfully");
     
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
