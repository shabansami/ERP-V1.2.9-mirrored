using ERP.Desktop.DTOs;
using ERP.DAL;
using ERP.Desktop.Services;
using ERP.Desktop.Utilities;
using ERP.Desktop.Views._Main;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using static ERP.Web.Utilites.Lookups;
using static ERP.Desktop.Utilities.CommonMethods;
using ERP.Desktop.Services.Transactions;
using ERP.Web.DataTablesDS;
using PrintEngine.HTMLPrint;
using ERP.Desktop.ViewModels;
using ERP.Desktop.Views.Items;

namespace ERP.Desktop.Views.Transactions.Pricing
{
    public partial class PriceInvoicesCashier : BaseForm
    {
        QuoteOrderSell currentInvoice;
        Item currentItem;
        List<ItemDetailsDT> CurrntInvoiceItems;


        readonly VTSaleEntities db;
        readonly PriceInvoiceServices _services;

        //Main Info properities
        public string CashierPrinter { get; set; }


        //Fast Keys Values
        public int FKOpenNewOrder { get; set; }
        public int FKItemsTable { get; set; }
        public int FKItemSearch { get; set; }
        public int FKSaveOrder { get; set; }


        public bool IsOpenFromShiftCloseFrom = false;

        bool loading = false;
        private PointOfSale currentPOS;
        private Branch currentBranch;

        public PriceInvoicesCashier() : base()
        {
            InitializeComponent();
            this.KeyPreview = true;
            TopLevel = true;
            //OVerride BaseForm value to enable maximizing in this form
            MaximizeBox = true;
            db = DBContext.UnitDbContext;
            _services = new PriceInvoiceServices(db);
        }

        #region Load
        private void CashierSales_Load(object sender, EventArgs e)
        {
            if (LoadMainInfo())
            {
                LoadFastKeysFromSetting();
                LoadDefaultPOSDetails();
                CashierPrinter = Properties.Settings.Default.DefaultPrinter;
                ckFixedQuantity.Checked = Properties.Settings.Default.fixQuantity;
                ckMergeMatched.Checked = Properties.Settings.Default.MergeMatched;
            }
            else
                Close();
        }


        private bool LoadMainInfo()
        {
            var posID = Properties.Settings.Default.PointOfSale;
            if (currentPOS == null)
                currentPOS = db.PointOfSales.FirstOrDefault(x => !x.IsDeleted && x.Id == posID);
            if (currentPOS == null)
            {
                AlrtMsgs.ShowMessageError("الرجاء تحديد نقطة البيع من إعدادات المظام");
                return false;
            }


            lblPOS.Text = currentPOS.Name;
            currentBranch = currentPOS.Store.Branch;
            lblBranch.Text = currentBranch.Name;
            return true;
        }

        private void LoadFastKeysFromSetting()
        {
            FKOpenNewOrder = Properties.Settings.Default.FastKeyOpenOrder;
            FKItemsTable = Properties.Settings.Default.FastKeyItemsTable;
            FKItemSearch = Properties.Settings.Default.FastKeyItemSearch;
            FKSaveOrder = Properties.Settings.Default.FastKeySaveOrder;

            lblFastOpenNewOrder.Text = CommonMethods.GetKeyEnum(FKOpenNewOrder) + " فتح فاتورة جديدة";
            lblFastItemsTable.Text = CommonMethods.GetKeyEnum(FKItemsTable) + " جدول الاصناف";
            lblFastItemSearch.Text = CommonMethods.GetKeyEnum(FKItemSearch) + " البحث عن صنف";
            lblFastSaveOrder.Text = CommonMethods.GetKeyEnum(FKSaveOrder) + " حفظ الفاتورة";
        }


        void LoadDefaultPOSDetails()
        {
            txtPrice.ReadOnly = !(txtPrice.Enabled = currentPOS.CanChangePrice);
            txtItemDiscountPerc.ReadOnly = !(txtItemDiscountPerc.Enabled = currentPOS.CanDiscount);
            txtItemDiscountValue.ReadOnly = !(txtItemDiscountValue.Enabled = currentPOS.CanDiscount);
        }
        #endregion

        #region Invoice Operations
        #region Create New Invoice
        private bool CreateNewInvoice()
        {

            var invoice = new QuoteOrderSell()
            {
                InvoiceDate = dtinvodate.Value,
                //InvoiceGuid = Guid.NewGuid()
            };
            var result = _services.CreateInvoice(invoice);
            if (!result.IsSuccessed)
            {
                AlrtMsgs.ShowMessageError(result.Message);
                return false;
            }
            currentInvoice = result.Object;
            return true;
        }
        #endregion

        #region LoadInvoice
        private void LoadInvoice(Guid invoiceID)
        {
            CurrntInvoiceItems = _services.GetInvoiceItems(invoiceID);
            var resultCalcTotals = _services.CalculateInvoiceTotals(invoiceID);
            if (!resultCalcTotals.IsSuccessed)
            {
                AlrtMsgs.ShowMessageError(resultCalcTotals.Message);
                if (resultCalcTotals.Object == null)
                    return;
            }
            currentInvoice = resultCalcTotals.Object;

            //finaly Load the invoice and it's details
            LoadInvoice(currentInvoice);
        }
        private void LoadInvoice(QuoteOrderSell invoice)
        {
            loading = true;
            ClearForm();
            if (invoice != null)
            {
                currentInvoice = invoice;
                txt_itemcount.Text = invoice.QuoteOrderSellDetails.Count(x => !x.IsDeleted) + "";
                txt_totalinvo.Text = invoice.TotalValue + "";

                txtInvoiceDiscountPers.Text = invoice.DiscountPercentage + "";

                txtInvoiceDiscounts.ReadOnly = invoice.DiscountPercentage > 0;

                txtInvoiceDiscounts.Text = invoice.InvoiceDiscount + "";
                txtInvoiceTotalDiscount.Text = invoice.InvoiceDiscount + "";

                dtinvodate.Value = invoice.InvoiceDate;

                txtInvoiceID.Text = invoice.InvoiceNumber + "";


                txtSalesTax.Text = invoice.SalesTax + "";
                txtProfitTax.Text = invoice.ProfitTax + "";

                txtInvoiceSafy.Text = invoice.Safy + "";
                LoadIteams();
            }
            loading = false;
        }
        void LoadIteams()
        {
            if (CurrntInvoiceItems == null)
            {
                if (currentInvoice == null)
                    return;
                else
                    CurrntInvoiceItems = _services.GetInvoiceItems(currentInvoice.Id);
            }

            DGItems.AutoGenerateColumns = false;
            DGItems.DataSource = CurrntInvoiceItems;
        }
        #endregion

        #region Update Invoice
        private void SaveInvoiceCalculationChanges()
        {
            if (loading)
                return;
            var result = _services.CalculateInvoiceTotals(currentInvoice.Id);
            if (!result.IsSuccessed)
            {
                AlrtMsgs.ShowMessageError(result.Message);
                if (result.Object == null)
                    return;
            }
            currentInvoice = result.Object;
            LoadInvoice(result.Object);
        }

        private void txtProfitTax_Leave(object sender, EventArgs e)
        {
            if (currentInvoice != null)
            {
                currentInvoice.ProfitTax = txtProfitTax.Num;
                SaveInvoiceCalculationChanges();
            }
        }

        private void txtSalesTax_Leave(object sender, EventArgs e)
        {
            if (currentInvoice != null)
            {
                currentInvoice.SalesTax = txtSalesTax.Num;
                SaveInvoiceCalculationChanges();
            }
        }

        private void txtInvoiceDiscountPers_Leave(object sender, EventArgs e)
        {
            if (currentInvoice != null)
            {
                currentInvoice.DiscountPercentage = txtInvoiceDiscountPers.Num;
                SaveInvoiceCalculationChanges();
            }
        }

        private void txtInvoiceDiscounts_Leave(object sender, EventArgs e)
        {
            if (currentInvoice != null && txtInvoiceDiscounts.Enabled)
            {
                currentInvoice.InvoiceDiscount = txtInvoiceDiscounts.Num;
                SaveInvoiceCalculationChanges();
            }
        }
        #endregion

        #region Save Invoice
        private void btnSaveInvoice_Click(object sender, EventArgs e)
        {
            btnSaveInvoice.Enabled = false;
            SaveInvoice();
            btnSaveInvoice.Enabled = true;
        }

        private void SaveInvoice()
        {
            if (currentInvoice == null)
            {
                MessageBox.Show("لايوجد فاتورة مفتوحة", "حفظ الفاتورة", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (DGItems.Rows.Count <= 0)
            {
                MessageBox.Show("الفاتورة لا تحتوي على أصناف", "حفظ الفاتورة", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (ckPrinting.Checked)
                PrintReport("فاتورة عرض اسعار");
            MessageBox.Show("تم حفظ الفاتورة", "حفظ الفاتورة", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            ClearForm();
        }

        #endregion
        #endregion

        #region Invoice Items Operations
        #region Search for item
        private void txt_code_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Down)
            {
                DGItems.Focus();
                return;
            }
            if (txtItemCode.Text != "")
            {
                if (e.KeyData == Keys.Enter || e.KeyData == Keys.Tab)
                {
                    var result = _services.ItemCodeSearch(txtItemCode.Text);
                    if (result.IsSuccessed)
                    {
                        FillItemNameAndPrice(result.Object);
                        currentItem = result.Object;
                    }
                    else
                    {
                        var dialogResult = MessageBox.Show("عفواً الصنف غير موجود\nفتح نافذة البحث عن صنف؟", "البحث عن صنف", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                        if (dialogResult == DialogResult.Yes)
                        {
                            OpenItemPopupSearch();
                        }
                    }
                }
                else
                {
                    ClearAddItemControls();
                }
            }
            else if (txtInvoiceID.Text != "" && e.KeyData == Keys.Enter)
            {
                if (MessageBox.Show("هل تريد حفظ الفاتورة؟", "حفظ الفاتورة", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    SaveInvoice();
                }
            }
        }
        private void OpenItemPopupSearch()
        {
            var form = new ItemSearch();
            form.ShowDialog();
            if (form.SelectedItem != null)
            {
                currentItem = db.Items.FirstOrDefault(x => x.Id == form.SelectedItem.ID);
                FillItemNameAndPrice(currentItem);
            }
        }
        #endregion

        #region New Item Criteria Changing
        private void FillItemNameAndPrice(Item item)
        {
            if (item == null)
                return;

            txtItemName.Text = item.Name;
            if (string.IsNullOrWhiteSpace(txtItemCode.Text))
                txtItemCode.Text = item.Id + "";
            var prices = item.ItemPrices.Where(x => !x.IsDeleted).Select(x => new IDNameVM { ID = x.Id, Name = x.PricingPolicy.Name, Object = x.PricingPolicyId }).ToList();
            FillComboBox(cmbItemPrices, prices, "Name", "Id");
            txtPrice.Text = item.SellPrice + "";
            if (currentItem == null)
                currentItem = item;


            if (ckFixedQuantity.Checked)
            {
                txtItemQuantity.Text = "1";
                AddItem();
                return;
            }


            if (cmbItemPrices.Items.Count > 1)
            {
                if (currentPOS.DefaultPricePolicyID != null)
                {
                    var pp = prices.FirstOrDefault(x => x.Object is int && (Guid)x.Object == currentPOS.DefaultPricePolicyID);
                    if (pp != null)
                    {
                        cmbItemPrices.SelectedValue = pp.ID;
                        if (!txtPrice.ReadOnly)
                        {
                            txtPrice.Focus();
                        }
                        else
                        {
                            txtItemQuantity.Focus();
                        }
                        return;
                    }
                }

                cmbItemPrices.Focus();
                return;
            }

            if (!txtPrice.ReadOnly)
            {
                txtPrice.Focus();
            }
            else
            {
                txtItemQuantity.Focus();
            }
        }
        private void cmbItemPrices_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbItemPrices.SelectedIndex > 0 && Guid.TryParse(cmbItemPrices.SelectedValue + "", out Guid id))
            {
                var price = db.ItemPrices.FirstOrDefault(x => !x.IsDeleted && x.Id == id);
                if (price != null && price.SellPrice != null)
                    txtPrice.Text = price.SellPrice + "";
            }
        }
        private void txtItemQuantity_TextChanged(object sender, EventArgs e)
        {
            if (currentItem != null && double.TryParse(txtItemQuantity.Text, out double quantity) && quantity > 0)
            {
                CalcAddItemTotals();
                return;
            }
            txtTotalItem.Text = string.Empty;
            txt_itemnet.Text = string.Empty;
        }

        void CalcAddItemTotals()
        {
            double quantity = 0, price = 0;
            double.TryParse(txtItemQuantity.Text, out quantity);
            double.TryParse(txtPrice.Text, out price);
            double total = quantity * price;
            txtTotalItem.Text = total.ToString();
            double.TryParse(txtItemDiscountPerc.Text, out double disperc);
            double discval;
            if (disperc > 0)
            {
                discval = total * disperc / 100;
                txtItemDiscountValue.Enabled = false;
                txtItemDiscountValue.Text = discval.ToString();
            }
            else
            {
                txtItemDiscountValue.Enabled = true;
                double.TryParse(txtItemDiscountValue.Text, out discval);
            }
            txt_itemnet.Text = (total - discval).ToString();
        }
        private void txtItemQuantity_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter || e.KeyData == Keys.Tab)
            {
                if (double.TryParse(txtItemQuantity.Text, out double quantity) && quantity > 0)
                {
                    if (txtItemDiscountPerc.Enabled)
                        txtItemDiscountPerc.Focus();
                    else if (txtItemDiscountValue.Enabled)
                        txtItemDiscountValue.Focus();
                    else
                    {
                        AddItem();
                    }
                }
                else
                {
                    MessageBox.Show("الرجاء ادخال كمية صحيحة", "الكمية المدخلة خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

        }
        private void txtItemDiscountPerc_TextChanged(object sender, EventArgs e)
        {
            CalcAddItemTotals();
        }
        private void txtItemDiscountPerc_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter || e.KeyData == Keys.Tab)
            {
                if (txtItemDiscountValue.Enabled)
                    txtItemDiscountValue.Focus();
                else
                    AddItem();
            }
        }
        private void txtItemDiscountValue_TextChanged(object sender, EventArgs e)
        {
            if (txtItemDiscountValue.Enabled)
                CalcAddItemTotals();
        }
        private void txtItemDiscountValue_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter || e.KeyData == Keys.Tab)
            {
                AddItem();
            }
        }
        #endregion

        #region Add Item to Invoice

        private void btnAddItem_Click(object sender, EventArgs e)
        {
            if (currentItem != null)
                AddItem();
        }
        private void AddItem()
        {

            if (currentItem == null)
                return;

            //check validation of invoice
            if (currentInvoice != null)
            {
                var result = _services.CheckValidInvoice(currentInvoice.Id);
                if (!result.IsSuccessed)
                {
                    AlrtMsgs.ShowMessageError(result.Message);
                    return;
                }
            }

            if (!double.TryParse(txtItemQuantity.Text, out double quantity))
            {
                AlrtMsgs.ShowMessageError("الرجاء تحديد الكمية بشكل صحيح");
                txtItemQuantity.Focus();
                return;
            }
            if (!double.TryParse(txtPrice.Text, out double price))
            {
                AlrtMsgs.ShowMessageError("الرجاء تحديد السعر بشكل صحيح");
                txtPrice.Focus();
                return;
            }
            //First open invoice if not exist
            if (currentInvoice == null)
            {
                if (!CreateNewInvoice()) //if creating new invoice not done successfully exit from this method
                    return;
            }
            double discount = txtItemDiscountPerc.Num > 0 ? quantity * price * txtItemDiscountPerc.Num / 100 : txtItemDiscountValue.Num;

            //second add the new item to invoice 
            var itemDetails = new QuoteOrderSellDetail()
            {
                Amount = quantity * price,
                Quantity = quantity,
                Price = price,
                ItemId = currentItem.Id,
                //ItemDiscount = discount,
                QuoteOrderSellId = currentInvoice.Id
            };

            var AllowSaleWithoutBalance = Properties.Settings.Default.AllowSaleWithoutBalance;
            var resultAddItem = _services.AddItemInInvoice(currentInvoice.Id, itemDetails, ckMergeMatched.Checked, !AllowSaleWithoutBalance);
            if (!resultAddItem.IsSuccessed)
            {
                AlrtMsgs.ShowMessageError(resultAddItem.Message);
                return;
            }
            CurrntInvoiceItems = resultAddItem.Object;
            ClearAddItemCodeAndControls();


            //therd calculate the invoice totals
            var resultCalcTotals = _services.CalculateInvoiceTotals(currentInvoice.Id);
            if (!resultCalcTotals.IsSuccessed)
            {
                AlrtMsgs.ShowMessageError(resultCalcTotals.Message);
                if (resultCalcTotals.Object == null)
                    return;
            }
            currentInvoice = resultCalcTotals.Object;

            //finaly Load the invoice and it's details
            LoadInvoice(currentInvoice);
        }
        #endregion

        #region Update Item in Invoice
        private void DGItems_KeyDown(object sender, KeyEventArgs e)
        {
            if (currentInvoice == null)
                return;
            var index = -1;
            if (DGItems.SelectedRows.Count > 0)
            {
                var row = DGItems.SelectedRows[0];
                index = row.Index;
                double Quty = double.Parse(row.Cells[nameof(Quantity)].Value.ToString());

                if (e.KeyData == Keys.Enter || e.KeyData == Keys.Tab)
                    txtInvoiceDiscountPers.Focus();
                else if (e.KeyData == Keys.Add)
                {
                    Quty++;
                    UpdateItemQuantity(row, Quty);
                }
                else if (e.KeyData == Keys.Subtract)
                {
                    Quty--;
                    if (Quty > 0)
                    {
                        UpdateItemQuantity(row, Quty);
                    }
                    else
                    {
                        DeleteItem(row);
                    }
                }
                else if (e.KeyData == Keys.NumPad0 || e.KeyData == Keys.Delete)
                {
                    DialogResult deleteResult = MessageBox.Show("هل تريد حذف الصنف " + DGItems.SelectedRows[0].Cells["Item_AName"].Value.ToString() + " ؟", "حذف صنف؟", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (deleteResult == DialogResult.Yes)
                    {
                        DeleteItem(row);
                    }
                }
                else if (e.KeyData == Keys.Up)
                {
                    if (index == 0)
                    {
                        txtItemCode.Focus();
                    }
                }
                else if (e.KeyData == Keys.Multiply)
                {
                    OrderDetailsQuantity orderDetailsQuantity = new OrderDetailsQuantity();
                    orderDetailsQuantity.FormClosing += (ss, ee) =>
                    {
                        if (orderDetailsQuantity.Selected)
                            UpdateItemQuantity(row, orderDetailsQuantity.Number);
                    };
                    orderDetailsQuantity.ShowDialog();
                }
            }

            if (index > -1 && DGItems.Rows.Count > index)
            {
                for (int i = 0; i < DGItems.Rows.Count; i++)
                {
                    if (i == index)
                    {
                        DGItems.ClearSelection();
                        DGItems.CurrentCell = DGItems.Rows[i].Cells[1];
                        DGItems.Rows[i].Selected = true;
                    }
                }
            }
        }

        private void UpdateItemQuantity(DataGridViewRow row, double quantity)
        {
            if (Guid.TryParse(row.Cells[nameof(ID)].Value + "", out Guid id))
            {
                DtoResult result = _services.UpdateItemQuantity(id, quantity);
                if (result.IsSuccessed)
                {
                    LoadInvoice(currentInvoice.Id);
                    DGItems.Focus();
                }
                else
                {
                    AlrtMsgs.ShowMessageError(result.Message);
                }
            }
        }
        #endregion

        #region Remove Item from Invoice

        private void DeleteItem(DataGridViewRow row)
        {
            var result = _services.DeleteItem((Guid)row.Cells[nameof(ID)].Value);
            if (result.IsSuccessed)
            {
                LoadInvoice(currentInvoice.Id);
                DGItems.Focus();
            }
            else
            {
                AlrtMsgs.ShowMessageError(result.Message);
            }
        }
        #endregion
        #endregion

        #region Navigate between controls
        #region New item controls

        private void cmbItemPrices_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && cmbItemPrices.SelectedIndex > 0)
            {
                if (ckFixedQuantity.Checked)
                {
                    txtItemQuantity.Text = "1";
                    AddItem();
                    return;
                }

                if (txtPrice.ReadOnly || !txtPrice.Enabled)
                    txtItemQuantity.Focus();
                else
                    txtPrice.Focus();
            }
        }

        private void txtPrice_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                txtItemQuantity.Focus();
        }
        #endregion

        #region Invoice Controls

        private void txt_invodisperc_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter || e.KeyData == Keys.Tab)
            {
                txtInvoiceDiscounts.Focus();
            }

        }

        private void txt_invodisc_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyData == Keys.Enter || e.KeyData == Keys.Tab))
            {
                btnSaveInvoice.Focus();
            }
        }
        #endregion
        #endregion

        #region Clear Form
        protected override void ClearForm()
        {
            currentInvoice = null;
            CurrntInvoiceItems = null;
            ClearAddItemCodeAndControls();

            dtinvodate.Value = DateTime.Now;
            txtInvoiceDiscountPers.Text = txtInvoiceDiscounts.Text = txtInvoiceSafy.Text = txt_totalinvo.Text = "0";
            txtInvoiceID.Text = string.Empty;
            txt_itemcount.Text = string.Empty;
            txtProfitTax.Text = txtSalesTax.Text = txtInvoiceTotalDiscount.Text = "0";
            txtInvoiceTotalDiscount.Text = string.Empty;
            DGItems.DataSource = null;
            LoadDefaultPOSDetails();
            GC.Collect();
        }
        void ClearAddItemCodeAndControls()
        {
            txtItemCode.Text = string.Empty;
            txtItemCode.Focus();
            ClearAddItemControls();
        }
        private void ClearAddItemControls()
        {
            currentItem = null;
            txtItemName.Text = txtPrice.Text = txtItemQuantity.Text = txtTotalItem.Text = txt_itemnet.Text = string.Empty;
            txtItemDiscountValue.Text = txtItemDiscountPerc.Text = "0";
            cmbItemPrices.DataSource = null;
            GC.Collect();
        }
        #endregion

        #region Validations

        private void txtIsNumberOrDecimal(object sender, KeyPressEventArgs e)
        {
            Validations.txtIsNumberOrDecimal(e, (Control)sender);
        }
        private void txtIsNumber_KeyPress(object sender, KeyPressEventArgs e)
        {
            //Validations.txtIsNumberOnly(e);
        }

        #endregion

        #region Form Methods and Shortcuts
        private void timer1_Tick(object sender, EventArgs e)
        {
            lblCurrentTime.Text = DateTime.Now.ToString("التاريخ: yyyy/MM/dd \tالوقت: HH:mm:ss");
        }
        private void CashierSales_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == FKSaveOrder)
            {
                SaveInvoice();
            }
            else if (e.KeyValue == FKItemSearch)
            {
                OpenItemPopupSearch();
            }

            else if (e.KeyValue == FKItemsTable)
            {
                DGItems.Focus();
            }
            else if (e.KeyValue == FKOpenNewOrder)
            {
                ClearForm();
            }
            else if (e.KeyCode == Keys.Home)
            {
                txtItemCode.Focus();
            }
        }


        private void ckFixedQuantity_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.fixQuantity = ckFixedQuantity.Checked;
            Properties.Settings.Default.Save();
        }

        private void ckMerge_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.MergeMatched = ckMergeMatched.Checked;
            Properties.Settings.Default.Save();
        }

        private void CashierSales_FormClosing(object sender, FormClosingEventArgs e)
        {
        }

        private void btn_newinvo_Click(object sender, EventArgs e)
        {
            ClearForm();
        }
        private void lblFastOpenNewOrder_Click(object sender, EventArgs e)
        {
            ClearForm();
        }


        private void lblFastItemsTable_Click(object sender, EventArgs e)
        {
            DGItems.Focus();
        }

        private void lblFastItemSearch_Click(object sender, EventArgs e)
        {
            OpenItemPopupSearch();
        }

        private void lblFastSaveOrder_Click(object sender, EventArgs e)
        {
            SaveInvoice();
        }
        #endregion

        #region Printing

        public void PrintReport(string title = " فاتورة عرض اسعار")
        {
            if (currentInvoice != null)
            {
                //check if datatable is null or empty
                if (CurrntInvoiceItems == null || CurrntInvoiceItems.Count == 0)
                {
                    MessageBox.Show("لا يوجد بيانات للطباعة", "طباعة", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                else
                {
                    VTSEngineBillPrint(title);
                }
            }
        }
        void VTSEngineBillPrint(string title)
        {
            Report report = new Report();
            report.ReportSource = CurrntInvoiceItems.ToDataTable();

            report.ReportTitle = title;


            //Add prepReport fields to the prepReport object.
            report.ReportFields.Add(new Field("ItemName", "الصنف"));
            report.ReportFields.Add(new Field("Price", "السعر"));
            report.ReportFields.Add(new Field("Quantity", "الكمية"));
            report.ReportFields.Add(new Field("Amount", "الاجمالي"));
            if (CurrntInvoiceItems.Any(x => x.ItemDiscount > 0))
            {
                report.ReportFields.Add(new Field("ItemDiscount", "قيمة الخصم"));
                report.ReportFields.Add(new Field("Safy", "الصافي"));
            }

            report.FontSize = 12;
            //Generate the prepReport tables
            report.GenerateReport();


            var headerTexts = new List<(string, string)>();
            headerTexts.Add(("تاريخ الفاتورة", DateTime.Parse(dtinvodate.Value.ToString()).ToString("yyyy/MM/dd")));
            headerTexts.Add(("وقت الطلب", DateTime.Parse(dtinvodate.Value.ToString()).ToString("hh:mm tt")));
            headerTexts.Add(("نقطة البيع", " نقطة" + lblPOS.Text));
            headerTexts.Add(("رقم الفاتورة", txtInvoiceID.Text + ""));

            report.AddStrings(headerTexts, 2, StringLocation.AfterTitle, false);


            var list = new List<(string, string)>() { ("اجمالي الفاتورة", txt_totalinvo.Text + "") };
            if (txtProfitTax.Num > 0)
            {
                list.Add(("ضريبة الارباح", txtProfitTax.Num.ToString()));
            }
            if (txtSalesTax.Num > 0)
            {
                list.Add(("ضريبة المبيعات", txtSalesTax.Num.ToString()));
            }
            if (txtInvoiceDiscountPers.Num > 0)
            {
                list.Add(("نسبة الخصم", txtInvoiceDiscountPers.Text + " %"));
            }
            if (txtInvoiceDiscounts.Num > 0)
            {
                list.Add(("قيمة الخصم", txtInvoiceDiscounts.Text + ""));
            }
            if (txtInvoiceTotalDiscount.Num > 0)
            {
                list.Add(("اجمالي قيمة الخصم", txtInvoiceTotalDiscount.Text + ""));
            }
            if (float.Parse(txt_totalinvo.Text) != float.Parse(txtInvoiceSafy.Text))
            {
                list.Add(("صافي الفاتورة", txtInvoiceSafy.Text + ""));
            }
            report.AddStrings(list, 1, fontsize: 10);



            //prepReport.AddLineBreak(StringLocation.AfterTitle);
            report.AddHorizontalLine(StringLocation.AfterTitle);
            if (System.Drawing.Printing.PrinterSettings.InstalledPrinters.Cast<string>().Contains(CashierPrinter))
                report.Print(CashierPrinter);
            else
                report.ShowPrintPreviewDialog();
        }

        #endregion

        #region Permissions 

        public void GetFormPermissions()
        {
            //if (PermissionHandler.formList.Count != 0)
            //{
            //    var form = PermissionHandler.formList.Find(row => row.Form_ID == 44);

            //    AllowUserToView = form.AllowUserToView;
            //    AllowUserToAdd = form.AllowUserToAdd;
            //    AllowUserToEdit = form.AllowUserToEdit;
            //    AllowUserToDelete = form.AllowUserToDelete;
            //    AllowUserToPrint = form.AllowUserToPrint;
            //    AllowUserToConfirm = form.AllowUserToConfirm;
            //}
        }
        public void SetPermissionsToFormControls()
        {
            //#if DEBUG
            //            return;
            //#endif
            //            if (!AllowUserToAdd)
            //            {
            //                btnSaveInvoice.Enabled = false;
            //            }

            //            if (!AllowUserToEdit)
            //            {
            //                btnSaveInvoice.Enabled = false;
            //            }
            //            if (!AllowUserToPrint)
            //            {
            //                ckPrinting.Checked = ckPrinting.Enabled = false;
            //            }
            //        }


        }

        #endregion

        #region Subforms
        private void btnAllIncoives_Click(object sender, EventArgs e)
        {
            var form = new AllPriceInvoices(_services);
            form.ShowDialog();
            if (form.ReturnedInvoice != null)
            {
                LoadInvoice(form.ReturnedInvoice);
            }
        }
        #endregion

    }
}