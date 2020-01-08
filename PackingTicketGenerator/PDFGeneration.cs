using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using VAA.BusinessComponents;
using VAA.DataAccess;

namespace PDFProcessingVAA
{
    public partial class PDFGeneration : Form
    {
        public PDFGeneration()
        {
            InitializeComponent();
            cmbMenuType.SelectedIndex = 0;
            cmbMenuType.DropDownStyle = ComboBoxStyle.DropDownList;

        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {

            if (string.IsNullOrEmpty(txtBoxOrderId.Text))
            {
                MessageBox.Show("Please enter Order Id!");
                return;
            }

            btnGenerateOrderPDF.Enabled = false;

            lblStatus.Text = "In Progress, Please Wait...";
            int index = cmbMenuType.SelectedIndex;
            Thread thread = new Thread(() => GenerateOrderPDF(index));

            thread.Start();
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

        /// <summary>
        /// Generate the order PDF - Upper class chili doc is copied and then PDF generation is done on copy
        /// </summary>
        private void GenerateOrderPDF(int menuTypeSelectedIndex)
        {
            try
            {
                MessageBox.Show("Order PDF Generation is in progress, you will be notified once the PDF generation is complete!");

                this.InvokeEx(f => f.lblStatus.Visible = true);
                this.InvokeEx(f => f.btnGenerateOrderPDF.Enabled = false);

                var order = txtBoxOrderId.Text.Trim();

                long orderId = Convert.ToInt64(order);

                OrderManagement _orderManagement = new OrderManagement();
                MenuProcessor _menuProcessor = new MenuProcessor();

                var liveOrderId = _orderManagement.GetLiveOrderIdFromOrderId(orderId);
                var cycleId = _orderManagement.GetCycleIdOfLiveOrder(liveOrderId);


                //FOOD GUIDE
                if (menuTypeSelectedIndex == 1)
                {
                    _menuProcessor.GeneratePdfForOrder(cycleId, 1, 5, orderId);
                }
                //all PE class

                if (menuTypeSelectedIndex == 4)
                {
                    _menuProcessor.GeneratePdfForOrder(cycleId, 2, orderId);
                }
                //all Eco class
                if (menuTypeSelectedIndex == 5)
                {
                    _menuProcessor.GeneratePdfForOrder(cycleId, 3, orderId);
                }

                //BRK
                if (menuTypeSelectedIndex == 2)
                {
                    _menuProcessor.GeneratePdfForOrder(cycleId, 1, 3, orderId);
                }


                //MAIN MENU
                if (menuTypeSelectedIndex == 3)
                {
                    List<long> UCMainMenuList = new List<long>();
                    var liveOrderDetails = _orderManagement.GetOrderDetailsbyOrderId(orderId);

                    foreach (var ordeDetails in liveOrderDetails)
                    {
                        if (ordeDetails.MenuType == "Main Menu" && ordeDetails.ClassName == "Upper")
                        {
                            if (!UCMainMenuList.Contains(ordeDetails.MenuId))
                                UCMainMenuList.Add(ordeDetails.MenuId);
                        }

                    }

                    ChiliProcessor chili = new ChiliProcessor();

                    foreach (var menuId in UCMainMenuList)
                    {
                        _menuProcessor.RebuildFlightNumberLotNumberChiliVariableForMenu(menuId);

                        var newDOc = chili.CloneChiliDocument(menuId);

                        chili.UpdateChiliDocumentVariablesallowServerRendering(1, menuId, newDOc);

                        _menuProcessor.GeneratePdfForMenu(cycleId, menuId, orderId, newDOc);
                    }

                }

                if (menuTypeSelectedIndex == 0)
                {
                    //FOOD GUIDE
                    _menuProcessor.GeneratePdfForOrder(cycleId, 1, 5, orderId);

                    //all PE class
                    _menuProcessor.GeneratePdfForOrder(cycleId, 2, orderId);

                    //all Eco class
                    _menuProcessor.GeneratePdfForOrder(cycleId, 3, orderId);

                    //BRK
                    _menuProcessor.GeneratePdfForOrder(cycleId, 1, 3, orderId);



                    //MAIN MENU
                    List<long> UCMainMenuList = new List<long>();
                    var liveOrderDetails = _orderManagement.GetOrderDetailsbyOrderId(orderId);

                    foreach (var ordeDetails in liveOrderDetails)
                    {
                        if (ordeDetails.MenuType == "Main Menu" && ordeDetails.ClassName == "Upper")
                        {
                            if (!UCMainMenuList.Contains(ordeDetails.MenuId))
                                UCMainMenuList.Add(ordeDetails.MenuId);
                        }

                    }

                    ChiliProcessor chili = new ChiliProcessor();

                    foreach (var menuId in UCMainMenuList)
                    {
                        _menuProcessor.RebuildFlightNumberLotNumberChiliVariableForMenu(menuId);

                        var newDOc = chili.CloneChiliDocument(menuId);

                        chili.UpdateChiliDocumentVariablesallowServerRendering(1, menuId, newDOc);

                        _menuProcessor.GeneratePdfForMenu(cycleId, menuId, orderId, newDOc);
                    }


                }

                MessageBox.Show("OrderPDF Generation completed successfully!");
                this.InvokeEx(f => f.lblStatus.Text = "OrderPDF Generation completed successfully!");

                this.InvokeEx(f => f.btnGenerateOrderPDF.Enabled = true);
                this.InvokeEx(f => f.btnView.Visible = true);
                this.InvokeEx(f => f.btnView.Text = "View PDFs");


            }
            catch (Exception ex)
            {
                MessageBox.Show("Order PDF Generation falied, please contact development team!");
                this.InvokeEx(f => f.lblStatus.Text = "Order PDF generation Failed!");

                this.InvokeEx(f => f.btnGenerateOrderPDF.Enabled = true);


            }
        }

        private void btnView_Click(object sender, EventArgs e)
        {




            Process.Start("explorer.exe", @"\\192.168.16.208\Digital_Production\Virgin\Emma\MENU PDFS\" + txtBoxOrderId.Text);


        }

    }
}
