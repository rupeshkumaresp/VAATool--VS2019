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
    public partial class AddFlightsToMenu : Form
    {
        RouteManagement _routeManagement = new RouteManagement();
        MenuManagement _menuManagement = new MenuManagement();
        MenuProcessor _menuProcessor = new MenuProcessor();

        public AddFlightsToMenu()
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

        private void btnAddToMenu_Click(object sender, EventArgs e)
        {

            if (string.IsNullOrEmpty(txtBoxMenuCode.Text))
            {

                MessageBox.Show("Please enter Menu Code");
                return;

            }


            if (string.IsNullOrEmpty(txtBoxDepartureCode.Text))
            {

                MessageBox.Show("Please enter Departure Airport Code");
                return;

            }


            if (string.IsNullOrEmpty(txtBoxArrivalAirportCode.Text))
            {

                MessageBox.Show("Please enter Arrival Airport Code");
                return;

            }

            if (string.IsNullOrEmpty(txtBoxFlightNumber.Text))
            {

                MessageBox.Show("Please enter Flight Number");
                return;

            }

            if (!string.IsNullOrEmpty(txtBoxFlightNumber.Text))
            {

                if (!txtBoxFlightNumber.Text.Contains("VS"))
                {
                    MessageBox.Show("Invalid flight Number, Valid Flight Number format is VSXXX");
                    return;
                }

            }

            var menu = _menuManagement.GetMenuByMenuCode(txtBoxMenuCode.Text);

            if (menu == null)
            {
                MessageBox.Show("No such menu exists in database: " + txtBoxMenuCode.Text);
                return;
            }

            var routeId = _routeManagement.GetRouteId(txtBoxDepartureCode.Text, txtBoxArrivalAirportCode.Text);


            if (cmbOperation.SelectedItem == "ADD FLIGHT")
            {

                if (!menu.MenuName.Contains(txtBoxFlightNumber.Text))
                {

                    //Menu name has been changed
                    var newMenuName = menu.MenuName + "/" + txtBoxFlightNumber.Text.ToUpper();
                    _menuManagement.UpdateMenuName(menu.Id, newMenuName);

                    //now add/update to menuforRoute table

                    _menuManagement.AddRouteForMenu(menu.Id, routeId, txtBoxFlightNumber.Text.ToUpper());

                    //update the chili document for this flight number

                    _menuProcessor.RebuildFlightNumberLotNumberChiliVariableForMenu(menu.Id);

                    MessageBox.Show("Flight Added Successfully!");
                }
                else
                {
                    MessageBox.Show("Flight is already present in Menu!");
                }
            }


            if (cmbOperation.SelectedItem == "REMOVE FLIGHT")
            {
                if (menu.MenuName.Contains(txtBoxFlightNumber.Text))
                {
                    var oldMenuName = menu.MenuName;
                    var newMenuName = "";

                    var menuNamePart = menu.MenuName.Split(new char[] { '/' });

                    var flights = "";
                    for (int icount = 0; icount < menuNamePart.Length; icount++)
                    {
                        if (menuNamePart[icount] != txtBoxFlightNumber.Text.ToUpper())
                            newMenuName += menuNamePart[icount] + "/";
                    }

                    if (newMenuName.EndsWith("/"))
                        newMenuName = newMenuName.Substring(0, newMenuName.Length - 1);


                    _menuManagement.UpdateMenuName(menu.Id, newMenuName);

                    _menuManagement.RemoveRouteForMenu(menu.Id, routeId, txtBoxFlightNumber.Text.ToUpper());

                    //update the chili document for this flight number
                    _menuProcessor.RebuildFlightNumberLotNumberChiliVariableForMenu(menu.Id);

                    MessageBox.Show("Flight Removed Successfully!");



                }
                else
                {
                    MessageBox.Show("Flight doesn't seem to be present in Menu, please check with Applaud!");
                }
            }


        }
    }
}
