using ERP.Desktop.DTOs;
using ERP.DAL;
using ERP.Desktop.Services;
using ERP.Desktop.Services.Items;
using ERP.Desktop.Utilities;
using ERP.Desktop.Views._Main;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using ERP.Desktop.Utilities;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static ERP.Desktop.Utilities.CommonMethods;
using ERP.Web.Utilites;

namespace ERP.Desktop.Views.Items
{
    public partial class ItemAddEdit : BaseForm
    {
        Guid editID = Guid.Empty;
        Guid editItemPriceID = Guid.Empty;
        VTSaleEntities db = DBContext.UnitDbContext;
        Item currentItem;
        public ItemAddEdit()
        {
            InitializeComponent();
            TryFillItemPrices();
        }
        public ItemAddEdit(Guid itemID)
        {
            InitializeComponent();
            editID = itemID;
        }

        #region Load and Fill
        private void ItemAddEdit_Load(object sender, EventArgs e)
        {
            Fill_Combobox();
            if (editID != Guid.Empty)
            {
                LoadItemForEdit(editID);
            }
        }
        void Fill_Combobox()
        {
            CommonMethods.FillComboBox(cmb_GroupBasics, db.Groups.Where(x => !x.IsDeleted && x.GroupTypeId == 1).ToList(), "Name", "Id");
            CommonMethods.FillComboBox(cmb_GroupSells, db.Groups.Where(x => !x.IsDeleted && x.GroupTypeId == 2).ToList(), "Name", "Id");
            CommonMethods.FillComboBox(cmb_ItemType, db.ItemTypes.Where(x => !x.IsDeleted).ToList(), "Name", "Id");
            CommonMethods.FillComboBox(cmb_Units, db.Units.Where(x => !x.IsDeleted).ToList(), "Name", "Id");
            CommonMethods.FillComboBox(cmbPricePolicies, db.PricingPolicies.Where(x => !x.IsDeleted).ToList(), "Name", "Id");
        }
        #endregion

        #region Clear form
        protected override void ClearForm()
        {
            btnCancelChanges.Visible = false;
            btnSaveChanges.Visible = false;
            btn_Add.Visible = true;
            txt_Name.Text = "";
            txt_SellPrice.Text = "0";
            txt_MinPrice.Text = "0.0";
            txt_MaxPrice.Text = "0.0";
            txt_ItemCode.Text = "";
            txt_ItemCode.Text = "";
            txt_TechnicalSpecifications.Text = "";
            currentItem = null;
            TryFillItemPrices();
        }
        #endregion

        #region Load Item for Editing
        void LoadItemForEdit(Guid id)
        {
            btnCancelChanges.Visible = true;
            btnSaveChanges.Visible = true;
            btn_Add.Visible = false;

            currentItem = db.Items.FirstOrDefault(x => x.Id == id);
            if (currentItem == null)
            {
                AlrtMsgs.ShowMessageError("خطأ في رقم الصنف");
                return;
            }

            cmb_GroupBasics.SelectedValue = currentItem.GroupBasicId ?? Guid.Empty;
            cmb_GroupSells.SelectedValue = currentItem.GroupSellId ?? Guid.Empty;
            cmb_ItemType.SelectedValue = currentItem.ItemTypeId ?? Guid.Empty;
            cmb_Units.SelectedValue = currentItem.UnitId ?? Guid.Empty;
            txt_Name.Text = currentItem.Name;
            txt_SellPrice.Text = currentItem.SellPrice + "";
            ckIsActive.Checked = currentItem.AvaliableToSell;
            txt_TechnicalSpecifications.Text = currentItem.TechnicalSpecifications;
            txt_Barcode.Text = currentItem.BarCode;
            txt_MinPrice.Text = (currentItem.MinPrice ?? 0) + "";
            txt_MaxPrice.Text = (currentItem.MaxPrice ?? 0) + "";
            txt_ItemCode.Text = currentItem.ItemCode + "";
            TryFillItemPrices();
        }
        #endregion

        #region Item Prices
        #region Load
        private void TryFillItemPrices()
        {
            if (currentItem == null || currentItem.Id == Guid.Empty)
            {
                gbPrices.Visible = false;
                this.Height = 310;
                return;
            }

            gbPrices.Visible = true;
            this.Height = 530;
            dgvPrices.AutoGenerateColumns = false;
            dgvPrices.DataSource = db.ItemPrices.Where(x => !x.IsDeleted && x.ItemId == currentItem.Id).Select(x => new { Id = x.Id, PolicyID = x.PricingPolicyId, Name = x.PricingPolicy.Name, Price = x.SellPrice }).ToList();
        }
        #endregion

        #region Add New
        private void btnAddPolicy_Click(object sender, EventArgs e)
        {
            if (currentItem == null || currentItem.Id == Guid.Empty)
            {
                AlrtMsgs.ShowMessageError("خطأ في رقم الصنف");
                return;
            }
            if (txtPolicyPrice.Num <= 0)
            {
                AlrtMsgs.EnterVaildData("السعر");
                txtPolicyPrice.Focus();
                return;
            }
            if (cmbPricePolicies.SelectedIndex < 1 || !Guid.TryParse(cmbPricePolicies.SelectedValue + "", out Guid policyID))
            {
                AlrtMsgs.ChooseVaildData("سياسة التسعير");
                cmbPricePolicies.Focus();
                cmbPricePolicies.DroppedDown = true;
                return;
            }

            if(currentItem.ItemPrices.Count(x=>!x.IsDeleted && x.PricingPolicyId == policyID) > 0)
            {
                AlrtMsgs.ShowMessageError("يوجد سعر لنفس سياسة التسعير بالفعل");
                return;
            }

            db.ItemPrices.Add(new ItemPrice()
            {
                SellPrice = txtPolicyPrice.Num,
                ItemId = currentItem.Id,
                PricingPolicyId = policyID,
            });
            db.SaveChanges(UserServices.UserInfo.UserId);
            AlrtMsgs.SaveSuccess();
            ClearItemPricesControls();
            TryFillItemPrices();
        }
        #endregion

        #region Clear
        void ClearItemPricesControls()
        {
            editItemPriceID = Guid.Empty;
            cmbPricePolicies.SelectedIndex = 0;
            txtPolicyPrice.Num = 0;
            btnAddPolicy.Visible = true;
            btnCancelPolcy.Visible = btnSavePolicyChanges.Visible = false;
        }
        #endregion

        #region Edit and Delete
        private void dgvPrices_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;
            var row = dgvPrices.Rows[e.RowIndex];
            if (e.ColumnIndex == dgvPrices.Columns[nameof(colEdit)].Index)
            {
                if (Guid.TryParse(row.Cells[nameof(colID)].Value + "", out Guid id))
                {
                    editItemPriceID = id;
                    txtPolicyPrice.Text = row.Cells[nameof(colPrice)].Value + "";
                    cmbPricePolicies.SelectedValue = int.Parse(row.Cells[nameof(policyID)].Value + "");
                    btnAddPolicy.Visible = false;
                    btnCancelPolcy.Visible = btnSavePolicyChanges.Visible = true;
                }
            }
            else if (e.ColumnIndex == dgvPrices.Columns[nameof(colDelete)].Index)
            {
                if (Guid.TryParse(row.Cells[nameof(colID)].Value + "", out Guid id))
                {
                    var itemPrice = db.ItemPrices.FirstOrDefault(x => x.Id == id);
                    if (itemPrice != null)
                    {
                        itemPrice.IsDeleted = true;
                        itemPrice.DeletedOn = TimeNow;
                        itemPrice.DeletedBy = UserServices.UserInfo.UserId;
                        db.SaveChanges(UserServices.UserInfo.UserId);
                        AlrtMsgs.DeleteSuccess();
                        TryFillItemPrices();
                    }
                }
            }
        }

        private void btnSavePolicyChanges_Click(object sender, EventArgs e)
        {
            if (currentItem == null || currentItem.Id == Guid.Empty)
            {
                AlrtMsgs.ShowMessageError("خطأ في رقم الصنف");
                return;
            }
            if (txtPolicyPrice.Num <= 0)
            {
                AlrtMsgs.EnterVaildData("السعر");
                txtPolicyPrice.Focus();
                return;
            }
            if (cmbPricePolicies.SelectedIndex < 1 || !Guid.TryParse(cmbPricePolicies.SelectedValue + "", out Guid policyID))
            {
                AlrtMsgs.ChooseVaildData("سياسة التسعير");
                cmbPricePolicies.Focus();
                cmbPricePolicies.DroppedDown = true;
                return;
            }

            if (currentItem.ItemPrices.Count(x => !x.IsDeleted && x.PricingPolicyId == policyID && x.Id != editItemPriceID) > 0)
            {
                AlrtMsgs.ShowMessageError("يوجد سعر لنفس سياسة التسعير بالفعل");
                return;
            }

            if (editItemPriceID != Guid.Empty)
            {
                var itemPrice = db.ItemPrices.FirstOrDefault(x => !x.IsDeleted && x.Id == editItemPriceID);
                if (itemPrice != null)
                {
                    itemPrice.PricingPolicyId = policyID;
                    itemPrice.SellPrice = txtPolicyPrice.Num;
                    db.SaveChanges(UserServices.UserInfo.UserId);
                    AlrtMsgs.SaveSuccess();
                    ClearItemPricesControls();
                    TryFillItemPrices();
                }
            }
        }

        private void btnCancelPolcy_Click(object sender, EventArgs e)
        {
            ClearItemPricesControls();
        }
        #endregion
        #endregion

        #region Add new 
        private void btn_Add_Click(object sender, EventArgs e)
        {
            Guid.TryParse(cmb_GroupBasics.SelectedValue.ToString(), out Guid GBid);
            float.TryParse(txt_SellPrice.Text, out float SP);
            float.TryParse(txt_MinPrice.Text, out float MiP);
            float.TryParse(txt_MaxPrice.Text, out float MaP);


            Guid.TryParse(cmb_ItemType.SelectedValue.ToString(), out Guid ITid);
            Guid.TryParse(cmb_Units.SelectedValue.ToString(), out Guid Uid);

            Guid.TryParse(cmb_GroupSells.SelectedValue.ToString(), out Guid GSid);

            if (GBid == null || GBid == Guid.Empty)
            {
                AlrtMsgs.ChooseVaildData("مجموعة اساسية");
                cmb_GroupBasics.Focus();
                return;
            }
            if (ITid == null || ITid == Guid.Empty)
            {
                AlrtMsgs.ChooseVaildData(" نوع الصنف");
                cmb_ItemType.Focus();
                return;
            }
            if (Uid == null || Uid == Guid.Empty)
            {
                AlrtMsgs.ChooseVaildData(" الوحدة");
                cmb_Units.Focus();
                return;
            }
            if (string.IsNullOrEmpty(txt_Name.Text))
            {
                AlrtMsgs.ChooseVaildData(" اسم الصنف");
                txt_Name.Focus();
                return;
            }
            if (string.IsNullOrEmpty(txt_SellPrice.Text))
            {
                AlrtMsgs.ChooseVaildData(" سعر البيع");
                txt_SellPrice.Focus();
                return;
            }
            //if (string.IsNullOrEmpty(txt_ItemCode.Text))
            //{
            //    AlrtMsgs.ChooseVaildData("كود الصنف");
            //    txt_ItemCode.Focus();
            //    return;
            //}

            if (db.Items.Count(x => !x.IsDeleted && x.ItemCode + "" == txt_ItemCode.Text) > 0)
            {
                AlrtMsgs.ShowMessageError("كود الصنف موجود من قبل");
                txt_ItemCode.Focus();
                return;
            }
            if (db.Items.Count(x => !x.IsDeleted && x.BarCode == txt_Barcode.Text) > 0)
            {
                AlrtMsgs.ShowMessageError("باركود الصنف موجود من قبل");
                txt_Barcode.Focus();
                return;
            }
            string itemCode = null;
            if (string.IsNullOrEmpty(txt_ItemCode.Text))
            {
                var groubCode = db.Groups.Where(x => x.Id == GBid).FirstOrDefault();
                if (groubCode != null)
                    itemCode = groubCode.GroupCode + "-" + (db.Items.Where(x => x.GroupBasicId == GBid && !x.IsDeleted).Count() + 1).ToString();
            }
            else
            {
                itemCode = txt_ItemCode.Text;
            }

            string barcode;
            if (String.IsNullOrEmpty(txt_Barcode.Text))
                barcode = generateBarCode();
            else
                barcode = txt_Barcode.Text;
            var newItem = new Item
            {
                GroupBasicId = GBid,
                ItemTypeId = ITid,
                UnitId = Uid,
                Name = txt_Name.Text,
                SellPrice = SP,
                MinPrice = MiP,
                MaxPrice = MaP,
                ItemCode = itemCode,
                BarCode =  barcode,
                TechnicalSpecifications = txt_TechnicalSpecifications.Text,
                AvaliableToSell = ckIsActive.Checked,
                //CreatedOn = DateTime.Now,
                //CreatedBy = 1

            };
            if (GSid != Guid.Empty)
            {
                newItem.GroupSellId = GSid;
            }

            db.Items.Add(newItem);

            if (db.SaveChanges(UserServices.UserInfo.UserId) > 0)
            {
                AlrtMsgs.SaveSuccess("يمكنك اضافة سياسات التسعير");
                currentItem = newItem;
                btn_Add.Visible = false;
                btnSaveChanges.Visible = true;
                TryFillItemPrices();
            }
            else
            {
                AlrtMsgs.ErrorWhenExcute("حدث حطأ اثناء حفظ الصنف الجديد");
            }
        }
        string generateBarCode()
        {
        generate:
            var barcode = GeneratBarcodes.GenerateRandomBarcode();
            var isExistInItems = db.Items.Where(x => x.BarCode == barcode).Any();
            var isExistInItemSerials = db.ItemSerials.Where(x => x.SerialNumber == barcode).Any();
            if (isExistInItems)
                goto generate;
            else
                return barcode;
        }
        #endregion

        #region Validations
        private void txt_SellPrice_KeyPress(object sender, KeyPressEventArgs e)
        {
            Validations.txtIsNumberOrDecimal(e, txt_SellPrice);
        }

        private void txt_MinPrice_KeyPress(object sender, KeyPressEventArgs e)
        {
            Validations.txtIsNumberOrDecimal(e, txt_MinPrice);
        }

        private void txt_MaxPrice_KeyPress(object sender, KeyPressEventArgs e)
        {
            Validations.txtIsNumberOrDecimal(e, txt_MaxPrice);
        }

        private void txt_ItemCode_KeyPress(object sender, KeyPressEventArgs e)
        {
            Validations.txtIsNumberOrDecimal(e, txt_ItemCode);
        }
        #endregion

        #region Save Edits
        private void btnSaveChanges_Click(object sender, EventArgs e)
        {
            Guid.TryParse(cmb_GroupBasics.SelectedValue.ToString(), out Guid GBid);
            float.TryParse(txt_SellPrice.Text, out float SP);
            float.TryParse(txt_MinPrice.Text, out float MiP);
            float.TryParse(txt_MaxPrice.Text, out float MaP);
            

            Guid.TryParse(cmb_ItemType.SelectedValue.ToString(), out Guid ITid);
            Guid.TryParse(cmb_Units.SelectedValue.ToString(), out Guid Uid);

            Guid.TryParse(cmb_GroupSells.SelectedValue.ToString(), out Guid GSid);

            if (GBid == null || GBid == Guid.Empty)
            {
                AlrtMsgs.ChooseVaildData("مجموعة اساسية");
                cmb_GroupBasics.Focus();
                return;
            }
            if (ITid == null || ITid == Guid.Empty)
            {
                AlrtMsgs.ChooseVaildData(" نوع الصنف");
                cmb_ItemType.Focus();
                return;
            }
            if (Uid == null || Uid == Guid.Empty)
            {
                AlrtMsgs.ChooseVaildData(" الوحدة");
                cmb_Units.Focus();
                return;
            }
            if (string.IsNullOrEmpty(txt_Name.Text))
            {
                AlrtMsgs.ChooseVaildData(" اسم الصنف");
                txt_Name.Focus();
                return;
            }
            if (string.IsNullOrEmpty(txt_SellPrice.Text))
            {
                AlrtMsgs.ChooseVaildData(" سعر البيع");
                txt_SellPrice.Focus();
                return;
            }

            //if (string.IsNullOrEmpty(txt_ItemCode.Text))
            //{
            //    AlrtMsgs.ChooseVaildData("كود الصنف");
            //    txt_ItemCode.Focus();
            //    return;
            //}

            if (db.Items.Count(x => !x.IsDeleted && x.ItemCode == txt_ItemCode.Text && x.Id != currentItem.Id) > 0)
            {
                AlrtMsgs.ShowMessageError("كود الصنف موجود من قبل");
                txt_ItemCode.Focus();
                return;
            }
            if (db.Items.Count(x => !x.IsDeleted && x.BarCode == txt_Barcode.Text && x.Id != currentItem.Id) > 0)
            {
                AlrtMsgs.ShowMessageError("باركود الصنف موجود من قبل");
                txt_Barcode.Focus();
                return;
            }
            string itemCode = null;
            if (string.IsNullOrEmpty(txt_ItemCode.Text))
            {
                var groubCode = db.Groups.Where(x => x.Id == GBid).FirstOrDefault();
                if (groubCode != null)
                    itemCode = groubCode.GroupCode + "-" + (db.Items.Where(x => x.GroupBasicId == GBid && !x.IsDeleted).Count() + 1).ToString();
            }
            else
            {
                itemCode = txt_ItemCode.Text;
            }

            string barcode;
            if (String.IsNullOrEmpty(txt_Barcode.Text))
                barcode = generateBarCode();
            else
                barcode = txt_Barcode.Text;

            currentItem.GroupBasicId = GBid;
            currentItem.GroupSellId = GSid;
            currentItem.ItemTypeId = ITid;
            currentItem.UnitId = Uid;
            currentItem.Name = txt_Name.Text;
            currentItem.SellPrice = SP;
            currentItem.MinPrice = MiP;
            currentItem.MaxPrice = MaP;
            currentItem.ItemCode = itemCode;
            currentItem.BarCode = barcode;
            currentItem.TechnicalSpecifications = txt_TechnicalSpecifications.Text;
            currentItem.AvaliableToSell = ckIsActive.Checked;
            //currentItem.ModifiedOn = DateTime.Now;
            //currentItem.ModifiedBy = 1;
            if (GSid != Guid.Empty)
            {
                currentItem.GroupSellId = GSid;
            }
            else
            {
                currentItem.GroupSellId = null;
            }
            db.SaveChanges(UserServices.UserInfo.UserId);
            AlrtMsgs.SaveSuccess();

            if (editID != Guid.Empty)//بعد حفظ التعديلات يتم اغلاق الشاشة اذا كانت مفتوحة لتعديل صنف
            {
                Close();
            }
        }
        #endregion

        #region Cancel Edits
        private void btnCancelChanges_Click(object sender, EventArgs e)
        {
            ClearForm();
            if (editID != Guid.Empty)//بعد الغاء التعديلات يتم اغلاق الشاشة اذا كانت مفتوحة لتعديل صنف
            {
                Close();
            }
        }
        #endregion
    }
}
