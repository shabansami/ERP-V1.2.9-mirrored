using ERP.Desktop.Utilities;
using ERP.DAL;
using System;
using System.Data;
using System.Windows.Forms;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using ERP.Desktop.Services;
using ERP.Desktop.Views._Main;
using System.Collections.Generic;
using ERP.Desktop.DTOs;
using ERP.Desktop.Services.Definations;
using ERP.Desktop.Services.Actors;
using static ERP.Web.Utilites.Lookups;
using ERP.Desktop.Services.Transactions;
using ERP.Desktop.Views.Items;
using ERP.Desktop.ViewModels;
using ERP.Web.Utilites;
using System.Drawing;

namespace ERP.Desktop.Views.Transactions.Sales
{
    public partial class ItemProductionFrm : BaseForm
    {
        readonly VTSaleEntities db;
        readonly SaleServices _services;
        Item currentItem;
        public ItemProductionVM itemProductions;
        Guid? customerID;
        TypeAssistant assistant;
        List<Person> people;
        public ItemProductionFrm(ItemProductionVM itemProds) : base()
        {
            InitializeComponent();
            customerID = itemProds.CustomerId;
            db = new VTSaleEntities();
            _services = new SaleServices(db);
            itemProductions = itemProds;
            assistant = new TypeAssistant();
            assistant.Idled += assistant_Idled;
        }

        private void rdo_newCustomer_CheckedChanged(object sender, EventArgs e)
        {
            if (rdo_newCustomer.Checked)
            {
                newCustomer();
                itemProductions = new ItemProductionVM();
                itemProductions.IsCustomerNew = true;
                this.Height = 460;
            }
        }

        private void rdo_existCustomer_CheckedChanged(object sender, EventArgs e)
        {
            if (rdo_existCustomer.Checked)
            {
                existCustomer();
                itemProductions = new ItemProductionVM();
                itemProductions.IsCustomerNew = false;
                FillDGV();
                this.Height = 585;
            }
        }

        private void ItemProductionFrm_Load(object sender, EventArgs e)
        {
            group_searchCustomer.Visible = false;
            group_itemProductionCust.Visible = false;
            if (customerID != null)
            {
                existCustomer();
                itemProductions.IsCustomerNew = false;
                itemProductions.CustomerId = customerID;
                FillDGV();
            }
            else
            {
                newCustomer();
                itemProductions.IsCustomerNew = true;
                txtName.Text = itemProductions.CustomerName;
                txtMob1.Text = itemProductions.CustomerMob;
                txtAddress.Text = itemProductions.CustomerAddress;
                txt_itemProduName.Text = itemProductions.ItemProductionName;
                BindDataGrid();
                txtName.Focus();
            }
            CommonMethods.AutoComplateTextbox(txt_itemName);

        }

        void newCustomer()
        {
            group_addNewCustomer.Visible = true;
            txtName.Focus();
            group_addItems.Visible = true;
            group_searchCustomer.Visible = false;
            group_itemProductionCust.Visible = false;
            group_Cancel.Visible = false;
            rdo_newCustomer.BackColor = Color.MediumAquamarine;
            rdo_existCustomer.BackColor = Color.LightCoral;
        }
        void existCustomer()
        {
            group_searchCustomer.Visible = true;
            group_itemProductionCust.Visible = true;
            group_Cancel.Visible = true;

            group_addNewCustomer.Visible = false;
            group_addItems.Visible = false;
            txt_searchCustNam.Focus();
            rdo_newCustomer.BackColor = Color.LightCoral;
            rdo_existCustomer.BackColor = Color.MediumAquamarine;

        }
        private void FillItemNameAndPrice(Item item)
        {
            if (item == null)
                return;

            txt_itemName.Text = item.Name;
            if (string.IsNullOrWhiteSpace(txtItemCode.Text))
                txtItemCode.Text = item.Id + "";
            //var prices = item.ItemPrices.Where(x => !x.IsDeleted).Select(x => new IDNameVM { ID = x.Id, Name = x.PricingPolicy.Name, Object = x.PricingPolicyId }).ToList();
            txtPrice.Text = item.SellPrice + "";
            if (currentItem == null)
                currentItem = item;

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

        private void txt_barcode_KeyDown(object sender, KeyEventArgs e)

        {
            if (txtItemCode.Text != "")
            {
                if (e.KeyData == Keys.Enter || e.KeyData == Keys.Tab)
                {
                    var result = _services.ItemCodeSearch(txtItemCode.Text);
                    if (result.IsSuccessed)
                    {
                        FillItemNameAndPrice(result.Object);
                        txt_quantity.Focus();

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
                    //ClearAddItemControls();
                }
            }
        }

        private void txtName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter || e.KeyData == Keys.Tab)
                txtMob1.Focus();

        }

        private void txtMob1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter || e.KeyData == Keys.Tab)
                txtAddress.Focus();

        }

        private void txtAddress_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter || e.KeyData == Keys.Tab)
                txt_itemProduName.Focus();

        }

        private void txt_quantity_KeyPress(object sender, KeyPressEventArgs e)
        {
            Validations.txtIsNumberOrDecimal(e, (Control)sender);
        }

        private void txt_quantity_KeyDown(object sender, KeyEventArgs e)

        {
            if (e.KeyData == Keys.Enter || e.KeyData == Keys.Tab)
            {
                AddItemProduction();
            }
        }

        private void AddItemProduction()
        {
            double quantity;
            if (!double.TryParse(txt_quantity.Text, out quantity))
            {
                AlrtMsgs.ShowMessageError("الرجاء تحديد الكمية بشكل صحيح");
                txt_quantity.Focus();
                return;
            }
            if (!double.TryParse(txtPrice.Text, out double price))
            {
                AlrtMsgs.ShowMessageError("الرجاء تحديد السعر بشكل صحيح");
                txtPrice.Focus();
                return;
            }
            if (currentItem == null)
            {
                AlrtMsgs.ShowMessageError("الرجاء تحديد الصنف بشكل صحيح");
                txtPrice.Focus();
                return;
            }
            if (itemProductions.itemProductionDetails.Where(x => x.ItemId == currentItem.Id && x.Quantity == quantity).Any())
            {
                AlrtMsgs.ShowMessageError("تم ادخال نفس الصنف بنفس الكمية سابقا");
                txtPrice.Focus();
                return;
            }

            var itemProductionDetails = new ItemProductionDetails
            {
                ItemBarcode = currentItem.BarCode,
                ItemName = currentItem.Name,
                Amount = quantity * price,
                Quantity = quantity,
                Price = price,
                ItemId = currentItem.Id,
            };
            itemProductions.itemProductionDetails.Add(itemProductionDetails);

            BindDataGrid();
            ClearTxt();
        }
        private void BindDataGrid()
        {
            dgv_items.AutoGenerateColumns = false;
            var source = new BindingSource(itemProductions.itemProductionDetails, null);
            dgv_items.DataSource = source;
        }
        private void btnAddItem_Click(object sender, EventArgs e)
        {
            AddItemProduction();
        }

        private void ClearTxt()
        {
            currentItem = null;
            txtItemCode.Text = "";
            txtPrice.Text = "";
            txt_itemName.Text = "";
            txtPrice.Text = "";
            txt_quantity.Text = "";
            txtItemCode.Focus();
        }

        private void txt_itemProduName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter || e.KeyData == Keys.Tab)
                txtItemCode.Focus();
        }

        private void dgv_items_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            var rowIndex = dgv_items.Rows[e.RowIndex].Index;


            if (e.ColumnIndex == dgv_items.Columns[nameof(del)].Index)
            {
                Delete(rowIndex);
            }
        }
        private void Delete(int id)
        {
            if (MessageBox.Show("هل تريد حذف الصنف ", "تنبيه", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
            {
                dgv_items.Rows.RemoveAt(id);
                itemProductions.itemProductionDetails = new List<ItemProductionDetails>();
                foreach (DataGridViewRow item in dgv_items.Rows)
                {
                    itemProductions.itemProductionDetails.Add(new ItemProductionDetails
                    {
                        Amount = double.Parse(item.Cells["Amount"].Value.ToString()),
                        ItemName = item.Cells["ItemName"].Value.ToString(),
                        ItemBarcode = item.Cells["ItemBarcode"].Value.ToString(),
                        ItemId = Guid.Parse(item.Cells["ItemId"].Value.ToString()),
                        Price = Convert.ToDouble(item.Cells["Price"].Value.ToString()),
                        Quantity = Convert.ToDouble(item.Cells["Quantity"].Value.ToString()),
                    });
                }
                BindDataGrid();
            }
        }

        private void btn_UpdateSaleInvoive_Click(object sender, EventArgs e)
        {
            if (rdo_newCustomer.Checked)
            {
                if (string.IsNullOrEmpty(txt_itemProduName.Text))
                {
                    AlrtMsgs.ShowMessageError("تأكد من ادخال مسمى التوليفة بشكل صحيح");
                    txtName.Focus();
                    return;
                }
                if (string.IsNullOrEmpty(txtName.Text) || string.IsNullOrEmpty(txtMob1.Text))
                {
                    AlrtMsgs.ShowMessageError("تأكد من ادخال اسم العميل ورقم التليفون بشكل صحيح");
                    txtName.Focus();
                    return;
                }
                if (itemProductions.itemProductionDetails.Count() == 0)
                {
                    AlrtMsgs.ShowMessageError("تأكد من ادخال اصناف التوليفة");
                    txtItemCode.Focus();
                    return;
                }
                itemProductions.CustomerName = txtName.Text;
                itemProductions.CustomerMob = txtMob1.Text;
                itemProductions.CustomerAddress = txtAddress.Text;
                itemProductions.ItemProductionName = txt_itemProduName.Text;

                if (db.Persons.Where(x => !x.IsDeleted && x.Mob1 == itemProductions.CustomerMob && (x.PersonTypeId == (int)Lookups.PersonTypeCl.Customer || x.PersonTypeId == (int)Lookups.PersonTypeCl.SupplierAndCustomer)).Count() > 0)
                {
                    AlrtMsgs.ShowMessageError("رقم الهاتف موجود مسبقا");
                    txtMob1.Focus();
                    return;
                }



                this.Close();
            }
            else if (rdo_existCustomer.Checked && !itemProductions.IsCustomerNew)
            {
                if (string.IsNullOrEmpty(txt_itemProduName.Text))
                {
                    AlrtMsgs.ShowMessageError("تأكد من ادخال مسمى التوليفة بشكل صحيح");
                    txtName.Focus();
                    return;
                }
                if (itemProductions.itemProductionDetails.Count() == 0)
                {
                    AlrtMsgs.ShowMessageError("تأكد من ادخال اصناف التوليفة");
                    txtItemCode.Focus();
                    return;
                }
                if (string.IsNullOrEmpty(txt_itemProduName.Text))
                {
                    AlrtMsgs.ShowMessageError("تأكد من ادخال مسمى التوليفة بشكل صحيح");
                    txtName.Focus();
                    return;
                }
                itemProductions.ItemProductionName = txt_itemProduName.Text;
                this.Close();
            }
        }

        void FillDGV()
        {
            people = db.Persons.Where(x => !x.IsDeleted && (x.PersonTypeId == (int)PersonTypeCl.Customer || x.PersonTypeId == (int)PersonTypeCl.SupplierAndCustomer)).ToList();
            dgv_search.AutoGenerateColumns = false;
            dgv_search.DataSource = people;
        }
        private void txt_searchCustNam_TextChanged(object sender, EventArgs e)
        {
            assistant.TextChanged();
        }
        void assistant_Idled(object sender, EventArgs e)
        {
            this.Invoke(
            new MethodInvoker(() =>
            {
                Search();
            }));
        }


        private void Search()
        {
            dgv_search.AutoGenerateColumns = false;
            if (txt_searchCustNam.TextLength > 2)
            {
                dgv_search.DataSource = people.Where(x => x.Name.Contains(txt_searchCustNam.Text) || x.Mob1.Contains(txt_searchCustNam.Text)).ToList();
            }
            else
            {
                dgv_search.DataSource = people;
            }
        }

        private void dgv_search_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            var senderGrid = (DataGridView)sender;
            Guid custID;
            var row = senderGrid.Rows[e.RowIndex];
            custID = Guid.Parse(row.Cells[nameof(Person_ID)].Value + "");

            //if (e.ColumnIndex == senderGrid.Columns[nameof(select)].Index && e.RowIndex >= 0)
            //{

            //}
            if (e.ColumnIndex == senderGrid.Columns[nameof(addNew)].Index && e.RowIndex >= 0)
            {
                var custmer = db.Persons.Find(custID);
                if (custmer != null)
                {
                    itemProductions.CustomerId = custmer.Id;
                    itemProductions.CustomerName = custmer.Name;
                    itemProductions.CustomerMob = custmer.Mob1;
                    itemProductions.CustomerAddress = custmer.Address;
                    this.Height = 795;
                    group_addItems.Visible = true;
                }
            }
            else
                ShowItemProduction(custID);
        }

        void ShowItemProduction(Guid customerId)
        {
            var list = db.ItemProductions.Where(x => !x.IsDeleted && x.CustomerId == customerId)
                .Select(x => new
                {
                    ItemProductionId = x.Id,
                    CustomerName = x.Person != null ? x.Person.Name : null,
                    ItemProductionName = x.Name,
                    CreatedOn = x.CreatedOn.ToString(),
                })
                .ToList();
            dgv_itemProductions.AutoGenerateColumns = false;
            dgv_itemProductions.DataSource = list;


        }

        private void dgv_itemProductions_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            var senderGrid = (DataGridView)sender;
            Guid ItemProductionID;
            var row = senderGrid.Rows[e.RowIndex];
            ItemProductionID = Guid.Parse(row.Cells[nameof(ItemProductionId)].Value + "");

            if (e.ColumnIndex == senderGrid.Columns[nameof(show)].Index && e.RowIndex >= 0)
            {
                var form = new ItemProdDetails(ItemProductionID);
                form.ShowDialog();
            }
            else
             if (e.ColumnIndex == senderGrid.Columns[nameof(selectItem)].Index && e.RowIndex >= 0)
            {
                var itemPro = db.ItemProductions.Where(x => x.Id == ItemProductionID).FirstOrDefault();
                var items = itemPro.ItemProductionDetails.Where(x => !x.IsDeleted).ToList();
                foreach (var item in items)
                {
                    double itemPrice = 0;
                    var itemdb = db.Items.Where(x => x.Id == item.ItemId).FirstOrDefault();
                    if (itemdb != null)
                        itemPrice = itemdb.SellPrice;
                    itemProductions.itemProductionDetails.Add(new ItemProductionDetails
                    {
                        ItemId = item.ItemId,
                        Amount = itemPrice * item.Quantity,
                        ItemBarcode = item.Item.BarCode,
                        ItemName = item.Item.Name,
                        Quantity = item.Quantity,
                        Price = itemPrice

                    });
                }
                if (string.IsNullOrEmpty(itemPro.CustomerId.ToString()))
                {
                    AlrtMsgs.ShowMessageError("تأكد من اختيار العميل بشكل صحيح");
                    dgv_search.Focus();
                    return;
                }
                if (itemProductions.itemProductionDetails.Count() == 0)
                {
                    AlrtMsgs.ShowMessageError("لا يوجد اصناف للتوليفة");
                    dgv_itemProductions.Focus();
                    return;
                }
                itemProductions.CustomerId = itemPro.CustomerId;
                itemProductions.CustomerName = itemPro.Person != null ? itemPro.Person.Name : null;
                itemProductions.CustomerMob = itemPro.Person != null ? itemPro.Person.Mob1 : null;
                itemProductions.CustomerAddress = itemPro.Person != null ? itemPro.Person.Address : null;
                //itemProductions.ItemProductionName = itemPro.Name;
                this.Close();
            }

        }

        private void dangerButton1_Click(object sender, EventArgs e)
        {
            this.Close();
            itemProductions = new ItemProductionVM();
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
            itemProductions = new ItemProductionVM();
            var t = 0;
        }

        private void txt_itemName_KeyDown(object sender, KeyEventArgs e)
        {
            if (txt_itemName.Text != "")
            {
                if (e.KeyData == Keys.Enter || e.KeyData == Keys.Tab)
                {
                    var result = _services.ItemNameSearch(txt_itemName.Text);
                    if (result.IsSuccessed)
                    {
                        FillItemNameAndPrice(result.Object);
                        txt_quantity.Focus();

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
                    //ClearAddItemControls();
                }
            }
        }
    }
}
