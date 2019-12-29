using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using VAA.BusinessComponents;
using VAA.CommonComponents;
using VAA.DataAccess;

namespace PDFProcessingVAA
{
    public partial class CloneMenus : Form
    {
        CycleManagement _cycleManagement = new CycleManagement();
        MenuManagement _menuManagement = new MenuManagement();
        MenuProcessor _menuProcessor = new MenuProcessor();


        public CloneMenus()
        {
            InitializeComponent();
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {

            string fromCycle = cmdFromCycle.SelectedItem.ToString();
            string toCycle = cmbToCycle.SelectedItem.ToString();

            if (!string.IsNullOrEmpty(richTextBoxMenucodes.Text))
            {
                var menucodes = richTextBoxMenucodes.Text;

                Thread thread =
                           new Thread(
                             unused => CloneNowMenuBased(fromCycle, menucodes, toCycle)
                           );

                thread.Start();
            }
            else
            {
                string weekNo = cmbWeekNo.SelectedItem.ToString();
                
                Thread thread =
                              new Thread(
                                unused => CloneNowWeekBased(fromCycle, weekNo, toCycle)
                              );

                thread.Start();
            }

        }


        private void CloneNowMenuBased(string fromCycle, string menuCodes, string toCycle)
        {

            this.InvokeEx(f => f.lblStatus.Visible = true);
            this.InvokeEx(f => f.lblStatus.Text = "In Progress, please wait!");
            this.InvokeEx(f => f.btnSubmit.Enabled = false);

            var newClonedMenus = _menuManagement.CloneMenuCodesToNewCycle(fromCycle, menuCodes, toCycle);

            ChiliProcessor chili = new ChiliProcessor();

            foreach (var key in newClonedMenus.Keys)
            {

                var newMenuID = key;
                var oldMenuId = newClonedMenus[key];

                var newMenu = _menuManagement.GetMenuById(newMenuID);

                var menuCode = "MNU" + Helper.Get8Digits();
                menuCode = MenuProcessor.GenerateMenuCode(Convert.ToInt32(newMenu.MenuTypeId), menuCode);

                var newMenuCodeWeek = menuCode;

                //only food guide will not have _1

                if (newMenu.MenuTypeId != 5)
                    newMenuCodeWeek += "_1";

                _menuManagement.UpdateMenuCode(newMenuID, newMenuCodeWeek);

                var newDOc = chili.CloneChiliDocument(oldMenuId);

                var oldMenuTemplate = _menuManagement.GetMenuTemplate(oldMenuId);
                _menuManagement.UpdateMenuTemplate(newMenuID, oldMenuTemplate.TemplateID, newDOc);


                //apply cycle menucode chili variable in chii doc
                _menuProcessor.RebuildFlightNumberLotNumberChiliVariableForMenu(newMenuID);
            }

            this.InvokeEx(f => f.lblStatus.Visible = true);
            this.InvokeEx(f => f.lblStatus.Text = "Menu cloning has been completed successfully!");
            this.InvokeEx(f => f.btnSubmit.Enabled = true);

            MessageBox.Show("Menu cloning has been completed successfully!");

        }



        private void CloneNowWeekBased(string fromCycle, string weekNo, string toCycle)
        {

            this.InvokeEx(f => f.lblStatus.Visible = true);
            this.InvokeEx(f => f.lblStatus.Text = "In Progress, please wait!");
            this.InvokeEx(f => f.btnSubmit.Enabled = false);

            var newClonedMenus = _menuManagement.CloneMenusToNewCycle(fromCycle, weekNo, toCycle);

            ChiliProcessor chili = new ChiliProcessor();

            foreach (var key in newClonedMenus.Keys)
            {

                var newMenuID = key;
                var oldMenuId = newClonedMenus[key];

                var newMenu = _menuManagement.GetMenuById(newMenuID);

                var menuCode = "MNU" + Helper.Get8Digits();
                menuCode = MenuProcessor.GenerateMenuCode(Convert.ToInt32(newMenu.MenuTypeId), menuCode);

                var newMenuCodeWeek = menuCode;

                //only food guide will not have _1

                if (newMenu.MenuTypeId != 5)
                    newMenuCodeWeek += "_1";

                _menuManagement.UpdateMenuCode(newMenuID, newMenuCodeWeek);

                var newDOc = chili.CloneChiliDocument(oldMenuId);

                var oldMenuTemplate = _menuManagement.GetMenuTemplate(oldMenuId);
                _menuManagement.UpdateMenuTemplate(newMenuID, oldMenuTemplate.TemplateID, newDOc);


                //apply cycle menucode chili variable in chii doc
                _menuProcessor.RebuildFlightNumberLotNumberChiliVariableForMenu(newMenuID);
            }

            this.InvokeEx(f => f.lblStatus.Visible = true);
            this.InvokeEx(f => f.lblStatus.Text = "Menu cloning has been completed successfully!");
            this.InvokeEx(f => f.btnSubmit.Enabled = true);

            MessageBox.Show("Menu cloning has been completed successfully!");

        }

        private void CloneMenus_Load(object sender, EventArgs e)
        {
            BindCycles();
        }

        private void BindCycles()
        {
            cmbToCycle.Items.Clear();
            cmdFromCycle.Items.Clear();

            var cycles = _cycleManagement.GetCycles();

            foreach (var cycle in cycles)
            {
                cmbToCycle.Items.Add(cycle.CycleName);
                cmdFromCycle.Items.Add(cycle.CycleName);
            }

        }
    }
}
