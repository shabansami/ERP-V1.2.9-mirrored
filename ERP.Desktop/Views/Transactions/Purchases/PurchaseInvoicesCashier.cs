﻿using ERP.Desktop.DTOs;
using ERP.DAL;
using ERP.Desktop.Services;
using ERP.Desktop.Utilities;
using ERP.Desktop.Views._Main;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using static ERP.Web.Utilites.Lookups;
using static ERP.Desktop.Utilities.CommonMethods;
using ERP.Desktop.Services.Transactions;
using ERP.Web.DataTablesDS;
using PrintEngine.HTMLPrint;
using ERP.Desktop.ViewModels;
using ERP.Desktop.Views.Items;
using ERP.Desktop.Views.Transactions.PurchasesBack;
using ERP.Desktop.Views.Actors.Suppliers;

namespace ERP.Desktop.Views.Transactions.Purchases
{
    public partial class PurchaseInvoicesCashier : BaseForm
    {
        public ShiftsOffline currentShift;
        public PointOfSale currentPOS;
        Branch currentBranch;

        PurchaseInvoice currentInvoice;
        Item currentItem;
        List<ItemDetailsDT> CurrntInvoiceItems;


        readonly VTSaleEntities db;
        readonly PurchaseServices _services;

        //Main Info properities
        public string CashierPrinter { get; set; }


        //Fast Keys Values
        public int FKOpenNewOrder { get; set; }
        public int FKReturns { get; set; }
        public int FKItemsTable { get; set; }
        public int FKItemSearch { get; set; }
        public int FKSaveOrder { get; set; }


        public bool IsOpenFromShiftCloseFrom = false;




        Guid? StoreID = null;
        bool loading = false;
        public PurchaseInvoicesCashier() : base()
        {
            InitializeComponent();
            this.KeyPreview = true;
            TopLevel = true;
            //OVerride BaseForm value to enable maximizing in this form
            MaximizeBox = true;
            db = DBContext.UnitDbContext;
            _services = new PurchaseServices(db);
        }

        #region Load
        private void CashierSales_Load(object sender, EventArgs e)
        {
            if (LoadMainInfo() && LoadShiftInfo())
            {
                LoadFastKeysFromSetting();
                FillComboxs();
                LoadDefaultPOSDetails();

                CashierPrinter = Properties.Settings.Default.DefaultPrinter;

                ckFixedQuantity.Checked = Properties.Settings.Default.fixQuantity;
                ckMergeMatched.Checked = Properties.Settings.Default.MergeMatched;

                //اذا كانت الشاشة مفتوحة من شاشة غلق الوردية و نريد فتح كل فواتير الوردية
                if (IsOpenFromShiftCloseFrom)
                {
                    var form = new AllPurchaseInvoices(currentShift.Id, _services);
                    form.ShowDialog();
                    if (form.ReturnedInvoice != null)
                    {
                        LoadInvoice(form.ReturnedInvoice);
                    }
                    else
                    {
                        Close();
                    }
                }
                return;
            }
            this.Close();
        }

        private bool LoadShiftInfo()
        {
            if (currentShift == null)
                currentShift = db.ShiftsOfflines.FirstOrDefault(x => !x.IsDeleted && !x.IsClosed && x.PointOfSaleID == currentPOS.Id);
            if (currentShift == null)
            {
                AlrtMsgs.ShowMessageError("لا يوجد وردية مفتوحة الان");
                return false;
            }
            lblShiftNumber.Text = currentShift.ShiftNumber + "";
            lblEmployee.Text = UserServices.UserInfo.Name;
            return true;
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
            StoreID = currentPOS.StoreID;
            lblBranch.Text = currentBranch.Name;
            return true;
        }
        private void LoadFastKeysFromSetting()
        {
            FKOpenNewOrder = Properties.Settings.Default.FastKeyOpenOrder;
            FKReturns = Properties.Settings.Default.FaskKeyReturns;
            FKItemsTable = Properties.Settings.Default.FastKeyItemsTable;
            FKItemSearch = Properties.Settings.Default.FastKeyItemSearch;
            FKSaveOrder = Properties.Settings.Default.FastKeySaveOrder;

            lblFastOpenNewOrder.Text = CommonMethods.GetKeyEnum(FKOpenNewOrder) + " فتح فاتورة جديدة";
            lblFastReturns.Text = CommonMethods.GetKeyEnum(FKReturns) + " مرتجعات التوريد";
            lblFastItemsTable.Text = CommonMethods.GetKeyEnum(FKItemsTable) + " جدول الاصناف";
            lblFastItemSearch.Text = CommonMethods.GetKeyEnum(FKItemSearch) + " البحث عن صنف";
            lblFastSaveOrder.Text = CommonMethods.GetKeyEnum(FKSaveOrder) + " حفظ الفاتورة";
        }


        void FillComboxs()
        {
            var customers = db.Persons.Where(x => !x.IsDeleted && (x.PersonTypeId == (int)PersonTypeCl.Supplier || x.PersonTypeId == (int)PersonTypeCl.SupplierAndCustomer)).ToList();
            CommonMethods.FillComboBox(cmbSuppliers, customers, "Name", "Id");

            var paymentTypes = db.PaymentTypes.Where(x => !x.IsDeleted).ToList();
            CommonMethods.FillComboBox(cmb_paytype, paymentTypes, "Name", "Id");
        }


        void LoadDefaultPOSDetails()
        {
            if (currentPOS.DefaultSupplierID != null)
                cmbSuppliers.SelectedValue = currentPOS.DefaultSupplierID;

            if (currentPOS.DefaultPaymentID != null)
                cmb_paytype.SelectedValue = currentPOS.DefaultPaymentID;

            txtPrice.ReadOnly = !(txtPrice.Enabled = currentPOS.CanChangePrice);
            txtItemDiscountPerc.ReadOnly = !(txtItemDiscountPerc.Enabled = currentPOS.CanDiscount);
            txtItemDiscountValue.ReadOnly = !(txtItemDiscountValue.Enabled = currentPOS.CanDiscount);
        }
        #endregion

        #region Invoice Operations
        #region Create New Invoice
        private bool CreateNewInvoice()
        {
            if (!Guid.TryParse(cmbSuppliers.SelectedValue + "", out Guid supplierID) || supplierID == Guid.Empty)
            {
                AlrtMsgs.ShowMessageError("الرجاء تحديد المورد");
                cmbSuppliers.Focus();
                return false;
            }
            if (!int.TryParse(cmb_paytype.SelectedValue + "", out int paymentTypeID) || paymentTypeID < 1)
            {
                AlrtMsgs.ShowMessageError("الرجاء تحديد طريقة السداد");
                cmb_paytype.Focus();
                return false;
            }
            //تحديد الدفع فى الخزنة او محفظة او بطاقة الكترونية 
            Guid? safeId = null;
            Guid? bankId = null;
            if (paymentTypeID == (int)PaymentTypeCl.Cash || paymentTypeID == (int)PaymentTypeCl.Partial || paymentTypeID == (int)PaymentTypeCl.Installment)
                safeId = currentPOS.SafeID;
            if (paymentTypeID == (int)PaymentTypeCl.BankWallet)
                bankId = currentPOS.DefaultBankWalletId;
            if (paymentTypeID == (int)PaymentTypeCl.BankCard)
                bankId = currentPOS.DefaultBankCardId;

            var invoice = new PurchaseInvoice()
            {
                BankAccountId = bankId,
                BranchId = currentBranch.Id,
                SupplierId = supplierID,
                InvoiceDate = TimeNow,
                PaymentTypeId = paymentTypeID,
                SafeId = safeId,
                ShiftOfflineID = currentShift.Id
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
        private void LoadInvoice(PurchaseInvoice invoice)
        {
            loading = true;
            ClearForm();
            if (invoice != null)
            {
                currentInvoice = invoice;

                cmbSuppliers.SelectedValue = invoice.SupplierId;
                cmb_paytype.SelectedValue = invoice.PaymentTypeId;

                txt_itemcount.Text = invoice.PurchaseInvoicesDetails.Count(x => !x.IsDeleted) + "";
                txt_totalinvo.Text = invoice.TotalValue + "";

                txtInvoiceDiscountPers.Text = invoice.InvoiceDiscount / invoice.TotalValue * 100 + "";
                txtInvoiceDiscounts.Text = invoice.InvoiceDiscount + "";
                txtInvoiceTotalDiscount.Text = invoice.TotalDiscount + "";

                dtinvodate.Value = invoice.InvoiceDate;

                txtInvoiceID.Text = invoice.InvoiceNumber;

                txtInvoiceExpenses.Text = invoice.TotalExpenses + "";

                txtSalesTax.Text = invoice.SalesTax + "";
                txtProfitTax.Text = invoice.ProfitTax + "";

                txtPayed.Text = invoice.PayedValue + "";
                txtInvoiceRemained.Text = (invoice.Safy - invoice.PayedValue) + "";
                txtInvoiceSafy.Text = invoice.Safy + "";

                bool IsClosed = invoice.IsFinalApproval;
                ChangeTheEnabledOfControlControllers(!IsClosed);
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
            if (currentInvoice != null && !currentInvoice.IsFinalApproval)
            {
                currentInvoice.ProfitTax = txtProfitTax.Num;
                SaveInvoiceCalculationChanges();
            }
        }

        private void txtSalesTax_Leave(object sender, EventArgs e)
        {
            if (currentInvoice != null && !currentInvoice.IsFinalApproval)
            {
                currentInvoice.SalesTax = txtSalesTax.Num;
                SaveInvoiceCalculationChanges();
            }
        }

        private void txtInvoiceDiscountPers_Leave(object sender, EventArgs e)
        {
            if (currentInvoice != null && !currentInvoice.IsFinalApproval && double.TryParse(txt_totalinvo.Text, out double total))
            {
                txtInvoiceDiscounts.Num = txtInvoiceDiscountPers.Num * total / 100;
            }
        }

        private void txtInvoiceDiscounts_Leave(object sender, EventArgs e)
        {
            if (currentInvoice != null && !currentInvoice.IsFinalApproval && txtInvoiceDiscounts.Enabled)
            {
                currentInvoice.InvoiceDiscount = txtInvoiceDiscounts.Num;
                SaveInvoiceCalculationChanges();
            }
        }

        private void txtInvoicePaied_TextChanged(object sender, EventArgs e)
        {
            double paid = 0, total = 0;
            double.TryParse(txtPayed.Text, out paid);
            double.TryParse(txtInvoiceSafy.Text, out total);
            double rest = total - paid;
            txtInvoiceRemained.Text = Math.Round(rest, 2).ToString();
            if (rest == 0)
            {
                txtInvoiceRemained.BackColor = System.Drawing.SystemColors.Control;
            }
            else if (rest > 0)
            {
                txtInvoiceRemained.BackColor = Color.Crimson;
            }
            else if (rest < 0)
            {
                txtInvoiceRemained.BackColor = Color.SpringGreen;
            }
        }

        private void cmbPaymentTyps_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (currentInvoice != null && !currentInvoice.IsFinalApproval)
            {
                if (cmb_paytype.SelectedIndex == 1)
                {
                    txtPayed.Text = txtInvoiceSafy.Text;
                    txtInvoiceRemained.Text = "0";
                    currentInvoice.PaymentTypeId = (int)PaymentTypeCl.Cash;
                    currentInvoice.SafeId = currentPOS.SafeID;
                    currentInvoice.BankAccountId = null;
                }
                else if (cmb_paytype.SelectedIndex == 2)
                {
                    txtInvoiceRemained.Text = txtInvoiceSafy.Text;
                    txtPayed.Text = "0";
                    currentInvoice.PaymentTypeId = (int)PaymentTypeCl.Deferred;
                    currentInvoice.SafeId = null;
                    currentInvoice.BankAccountId = null;
                }
                else if (cmb_paytype.SelectedIndex == 3)
                {
                    currentInvoice.PaymentTypeId = (int)PaymentTypeCl.Partial;
                    currentInvoice.SafeId = currentPOS.SafeID;
                    currentInvoice.BankAccountId = null;
                }
                else if (cmb_paytype.SelectedIndex == 4)
                {
                    currentInvoice.PaymentTypeId = (int)PaymentTypeCl.Installment; //تقسيط
                    currentInvoice.SafeId = currentPOS.SafeID;
                    currentInvoice.BankAccountId = null;
                }
                else if (cmb_paytype.SelectedIndex == 5)
                {
                    txtPayed.Text = txtInvoiceSafy.Text;
                    txtInvoiceRemained.Text = "0";
                    currentInvoice.PaymentTypeId = (int)PaymentTypeCl.BankWallet; //محفظة بنكية
                    currentInvoice.BankAccountId = currentPOS.DefaultBankWalletId;
                    currentInvoice.SafeId = null;
                }
                else if (cmb_paytype.SelectedIndex == 6)
                {
                    txtPayed.Text = txtInvoiceSafy.Text;
                    txtInvoiceRemained.Text = "0";
                    currentInvoice.PaymentTypeId = (int)PaymentTypeCl.BankCard;//بطاقة بنكية
                    currentInvoice.BankAccountId = currentPOS.DefaultBankCardId;
                    currentInvoice.SafeId = null;
                }
                SaveInvoiceCalculationChanges();
            }
        }

        private void txt_invonet_TextChanged(object sender, EventArgs e)
        {
            if ((cmb_paytype.SelectedIndex) == 1)
            {
                txtPayed.Text = txtInvoiceSafy.Text;
                txtInvoiceRemained.Text = "0";
            }
            else if (cmb_paytype.SelectedIndex == 2)
            {
                txtInvoiceRemained.Text = txtInvoiceSafy.Text;
                txtPayed.Text = "0";
            }
        }

        private void txt_paied_Leave(object sender, EventArgs e)
        {
            if (currentInvoice != null && !currentInvoice.IsFinalApproval)
            {
                currentInvoice.PayedValue = txtPayed.Num;
                SaveInvoiceCalculationChanges();
            }
        }

        private void cmb_customer_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (loading)
                return;
            if (cmbSuppliers.SelectedIndex > 0 && currentInvoice != null && !currentInvoice.IsFinalApproval
                && Guid.TryParse(cmbSuppliers.SelectedValue + "", out Guid supplierID))
            {
                currentInvoice.SupplierId = supplierID;
                db.SaveChanges(UserServices.UserInfo.UserId);
            }
        }
        #endregion

        #region Save Invoice

        private void txt_paied_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                txt_paied_Leave(null, null);
                btnSaveInvoice_Click(null, null);
            }
        }
        private void btnSaveInvoice_Click(object sender, EventArgs e)
        {
            btnSaveInvoice.Enabled = false;
            if (currentInvoice == null || currentInvoice.IsFinalApproval)
            {
                MessageBox.Show("لايوجد فاتورة مفتوحة", "حفظ الفاتورة", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (DGItems.Rows.Count <= 0)
            {
                MessageBox.Show("الفاتورة لا تحتوي على أصناف", "حفظ الفاتورة", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var result = _services.CloseInvoice(currentInvoice);
            if (!result.IsSuccessed)
            {
                MessageBox.Show(result.Message, "حفظ الفاتورة", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (ckPrinting.Checked)
                PrintReport("فاتورة توريدات");
            MessageBox.Show("تم حفظ الفاتورة", "حفظ الفاتورة", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            ClearForm();
            btnSaveInvoice.Enabled = true;
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
                if (MessageBox.Show("هل تريد غلق الفاتورة؟", "غلق الفاتورة", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    txtPayed.Focus();
                    txtPayed.SelectAll();
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
                var result = _services.CheckValidInvoice(currentInvoice);
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
            var itemDetails = new PurchaseInvoicesDetail()
            {
                Amount = quantity * price,
                Quantity = quantity,
                Price = price,
                ItemId = currentItem.Id,
                PurchaseInvoiceId = currentInvoice.Id,
                ItemDiscount = discount,
                StoreId = StoreID
            };

            var resultAddItem = _services.AddItemInInvoice(currentInvoice.Id, itemDetails, ckMergeMatched.Checked);
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
            if (currentInvoice == null || currentInvoice.IsFinalApproval)
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
                txtPayed.Focus();
            }
        }
        #endregion
        #endregion

        #region From UI Changing
        void ChangeTheEnabledOfControlControllers(bool enabled)
        {
            txtSalesTax.Enabled = enabled;
            txtProfitTax.Enabled = enabled;
            dtinvodate.Enabled = enabled;
            cmbSuppliers.Enabled = enabled;
            cmb_paytype.Enabled = enabled;
            btnSearchForSupplier.Enabled = enabled;
            txtItemCode.Enabled = enabled;
            txtItemQuantity.Enabled = enabled;
            txtItemName.Enabled = enabled;
            txtPayed.Enabled = enabled;
            txtPayed.ReadOnly = cmb_paytype.SelectedIndex == 2;
            cmbItemPrices.Enabled = enabled;
            btnSaveInvoice.Enabled = enabled;
            txtPrice.Enabled = enabled && currentPOS.CanChangePrice;
            txtItemDiscountPerc.Enabled = txtItemDiscountValue.Enabled = enabled && currentPOS.CanDiscount;
            txtInvoiceDiscountPers.Enabled = enabled;
            txtInvoiceDiscounts.Enabled = enabled;
        }
        #endregion

        #region Clear Form
        protected override void ClearForm()
        {
            currentInvoice = null;
            CurrntInvoiceItems = null;
            ClearAddItemCodeAndControls();

            txtInvoiceRemained.BackColor = System.Drawing.SystemColors.Control;
            dtinvodate.Value = DateTime.Now;
            cmbSuppliers.SelectedIndex = 0;
            txtInvoiceDiscountPers.Text = txtInvoiceDiscounts.Text = txtInvoiceSafy.Text = txt_totalinvo.Text = "0";
            txtInvoiceID.Text = string.Empty;
            txt_itemcount.Text = string.Empty;
            txtProfitTax.Text = txtSalesTax.Text = txtPayed.Text = txtInvoiceRemained.Text = txtInvoiceTotalDiscount.Text = "0";
            txtInvoiceTotalDiscount.Text = string.Empty;
            DGItems.DataSource = null;
            ChangeTheEnabledOfControlControllers(true);
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

        private void CashierSales_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == FKSaveOrder)
            {
                txtPayed.Focus();
                txtPayed.SelectAll();
            }
            else if (e.KeyValue == FKItemSearch)
            {
                OpenItemPopupSearch();
            }

            else if (e.KeyValue == FKItemsTable)
            {
                DGItems.Focus();
            }
            else if (e.KeyValue == FKReturns)
            {
                var form = new PurchaseBackInvoicesCashier();
                form.ShowDialog();
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
        private void timer1_Tick(object sender, EventArgs e)
        {
            lblCurrentTime.Text = DateTime.Now.ToString("التاريخ: yyyy/MM/dd \tالوقت: HH:mm:ss");
        }


        private void lblFastOpenNewOrder_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void lblFastReturns_Click(object sender, EventArgs e)
        {
            var form = new PurchaseBackInvoicesCashier();
            form.ShowDialog();
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
            txtPayed.Focus();
            txtPayed.SelectAll();
        }
        #endregion

        #region Customer
        private void btnSearchForSupplier_Click(object sender, EventArgs e)
        {
            var form = new SupplierSelect(true);
            form.ShowDialog();
            if (form.Selected)
            {
                var customers = db.Persons.Where(x => !x.IsDeleted && (x.PersonTypeId == (int)PersonTypeCl.Supplier || x.PersonTypeId == (int)PersonTypeCl.SupplierAndCustomer)).ToList();
                FillComboBox(cmbSuppliers, customers, "Name", "Id");
                cmbSuppliers.SelectedValue = form.ReturnedSupplierID;
            }
        }
        #endregion

        #region Printing

        public void PrintReport(string title = " فاتورة توريدات")
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
            headerTexts.Add(("نوع الدفع", cmb_paytype.Text));
            headerTexts.Add(("رقم الوردية", lblShiftNumber.Text));
            headerTexts.Add(("الكاشير", lblEmployee.Text));
            headerTexts.Add(("رقم الفاتورة", txtInvoiceID.Text + ""));

            report.AddStrings(headerTexts, 2, StringLocation.AfterTitle, false);
            report.AddHtmlLine($"<p><b>المورد :</b> {cmbSuppliers.Text}</p>");

            if (currentInvoice.SupplierId != null && currentInvoice.PersonSupplier != null)
            {
                report.AddHtmlLine($"<p><b>تليفون :</b> {currentInvoice.PersonSupplier.Mob1}</p>");
            }


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

            if (float.Parse(txtPayed.Text) < float.Parse(txtInvoiceSafy.Text))
            {
                list.Add(("المدفوع", txtPayed.Text + ""));
                list.Add(("الباقي", txtInvoiceRemained.Text));
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

        #region SubForm
        private void btnExpenes_Click(object sender, EventArgs e)
        {
            if (currentInvoice != null)
            {
                var form = new PurchaseInvoiceExpenses(currentInvoice.Id);
                form.ShowDialog();
                if (form.IsChanged)
                {
                    SaveInvoiceCalculationChanges();
                }
            }
            else
            {
                AlrtMsgs.ShowMessageError("الرجاء فتح فاتورة اولاً");
            }
        }

        private void btnAllIncoives_Click(object sender, EventArgs e)
        {
            var form = new AllPurchaseInvoices(currentShift.Id, _services);
            form.ShowDialog();
            if (form.ReturnedInvoice != null)
            {
                LoadInvoice(form.ReturnedInvoice);
            }
        }
        #endregion
    }
}