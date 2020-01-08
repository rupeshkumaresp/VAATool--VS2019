using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PackingTicketGenerator;
using VAA.DataAccess;

namespace PDFProcessingVAA
{
    public partial class Login : Form
    {
        AccountManagement _accountManagement = new AccountManagement();

        public Login()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            var userId = _accountManagement.LogIn(textBoxEmail.Text, VAA.CommonComponents.EncryptionHelper.Encrypt(txtboxPassword.Text));

            if (userId > 0)
            {
                _accountManagement.WriteToAdminTool(userId, "Login to the Admin Tool");
                this.Hide();
                PackingTicketGenerator.Action action = new PackingTicketGenerator.Action(userId);
                action.ShowDialog();
            }
            else
            {
                MessageBox.Show("Incorrect Email/Password!");
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
