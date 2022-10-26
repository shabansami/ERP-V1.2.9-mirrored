using ERP.DAL;
using ERP.Desktop.Services;
using ERP.Desktop.Services.Actors;
using ERP.Desktop.Utilities;
using ERP.Desktop.Views._Main;
using ERP.Web.Utilites;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static ERP.Web.Utilites.Lookups;

namespace ERP.Desktop.Views.Actors.Suppliers
{
    public partial class SupplierSelect : BaseForm
    {
        VTSaleEntities db = DBContext.UnitDbContext;
        Guid editingID;
        TypeAssistant assistant;
        readonly bool _isReturnSupplierID;

        List<Country> countries;
        List<Person> people;
        public Guid ReturnedSupplierID { get; set; }
        public bool Selected { get; set; }
        SupplierServices _services;

        public SupplierSelect(bool isReturnSupplier)
        {
            InitializeComponent();
            _isReturnSupplierID = isReturnSupplier;
            this.KeyPreview = true;
            assistant = new TypeAssistant();
            assistant.Idled += assistant_Idled;
            StyleDataGridViews(dgvSupplier);
            _services = new SupplierServices(db);
        }
        public SupplierSelect() : this(false)
        {

        }

        #region Load and Fill
        private void Customers_Load(object sender, EventArgs e)
        {
            FillDGV();
            FillCountries();
            txt_search.Focus();
        }

        void FillDGV()
        {
            people = db.Persons.Where(x => !x.IsDeleted && (x.PersonTypeId == (int)PersonTypeCl.Supplier || x.PersonTypeId == (int)PersonTypeCl.SupplierAndCustomer)).ToList();
            dgvSupplier.AutoGenerateColumns = false;
            dgvSupplier.DataSource = people;
        }

        private void FillCountries()
        {
            countries = db.Countries.Where(x => !x.IsDeleted).Include(x => x.Cities.Select(y => y.Areas)).ToList();
            CommonMethods.FillComboBox(cmbCountries, countries, "Name", "Id");
            cmbCountries.SelectedIndex = 0;
        }
        #endregion

        #region Add new 
        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                AlrtMsgs.EnterVaildData("اسم المورد");
                txtName.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(txtMob1.Text))
            {
                AlrtMsgs.EnterVaildData("رقم الهاتف");
                txtMob1.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(txtAddress.Text))
            {
                AlrtMsgs.EnterVaildData("العنوان");
                txtAddress.Focus();
                return;
            }
            if (cmbAreas.SelectedIndex < 1 || !Guid.TryParse(cmbAreas.SelectedValue + "", out Guid areaID))
            {
                AlrtMsgs.ChooseVaildData("المنطقة");
                cmbAreas.Focus();
                return;
            }

            var person = new Person()
            {
                Address = txtAddress.Text,
                AreaId = areaID,
                GenderId = rbGenderMale.Checked ? 1 : 2,
                Mob1 = txtMob1.Text,
                Mob2 = txtMob2.Text,
                Name = txtName.Text,
                IsActive = true,
                PersonTypeId = (int)(rbSupplier.Checked ? Lookups.PersonTypeCl.Supplier : Lookups.PersonTypeCl.SupplierAndCustomer)
            };

            var result = _services.AddNew(person);
            if (!result.IsSuccessed)
            {
                AlrtMsgs.ShowMessageError(result.Message);
                return;
            }
            else
            {
                AlrtMsgs.SaveSuccess();
            }
            if (_isReturnSupplierID)
            {
                ReturnedSupplierID = result.Object.Id;
                Selected = true;
                Close();
            }
            else
            {
                ClearForm();
                FillDGV();
            }
        }
        #endregion

        #region Edit and Delete
        private void dgvCustomers_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            var senderGrid = (DataGridView)sender;
            if (e.ColumnIndex == senderGrid.Columns[nameof(edit)].Index && e.RowIndex >= 0)
            {
                var row = senderGrid.Rows[e.RowIndex];
                editingID = Guid.Parse(row.Cells[nameof(Person_ID)].Value + "");

                var person = people.FirstOrDefault(x => x.Id == editingID);
                if (person != null)
                {
                    txtName.Text = person.Name;
                    txtMob1.Text = person.Mob1;
                    txtMob2.Text = person.Mob2;
                    txtAddress.Text = person.Address;
                    cmbCountries.SelectedValue = person.Area.City.CountryId;
                    cmbCities.SelectedValue = person.Area.CityId;
                    cmbAreas.SelectedValue = person.AreaId;
                    (person.GenderId == 1 ? rbGenderMale : rbGenderFemale).Checked = true;
                    (person.PersonTypeId == (int)Lookups.PersonTypeCl.Supplier ? rbSupplier : rbCustAndSupplier).Checked = true;
                    btnSaveChanges.Visible = true;
                    btnCancel.Visible = true;
                    btnAddCustomer.Visible = false;
                }
            }
            else if (e.ColumnIndex == senderGrid.Columns[nameof(delete)].Index && e.RowIndex >= 0)
            {
                if (AlrtMsgs.CheckDelete() == DialogResult.Yes)
                {
                    var row = senderGrid.Rows[e.RowIndex];
                    editingID = Guid.Parse(row.Cells[nameof(Person_ID)].Value + "");
                    var result = _services.Delete(editingID);
                    if (!result.IsSuccessed)
                    {
                        AlrtMsgs.ShowMessageError(result.Message);
                    }
                    else
                    {
                        AlrtMsgs.DeleteSuccess();
                        ClearForm();
                        FillDGV();
                    }
                }
            }
        }
        private void btnSaveChanges_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                AlrtMsgs.EnterVaildData("اسم المورد");
                txtName.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(txtMob1.Text))
            {
                AlrtMsgs.EnterVaildData("رقم الهاتف");
                txtMob1.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(txtAddress.Text))
            {
                AlrtMsgs.EnterVaildData("العنوان");
                txtAddress.Focus();
                return;
            }
            if (cmbAreas.SelectedIndex < 1 || !Guid.TryParse(cmbAreas.SelectedValue + "", out Guid areaID))
            {
                AlrtMsgs.ChooseVaildData("المنطقة");
                cmbAreas.Focus();
                return;
            }

            var person = new Person()
            {
                Id = editingID,
                Address = txtAddress.Text,
                AreaId = areaID,
                GenderId = rbGenderMale.Checked ? 1 : 2,
                Mob1 = txtMob1.Text,
                Mob2 = txtMob2.Text,
                Name = txtName.Text,
                PersonTypeId = (int)(rbSupplier.Checked ? Lookups.PersonTypeCl.Supplier : Lookups.PersonTypeCl.SupplierAndCustomer)
            };

            var result = _services.Update(person);
            if (!result.IsSuccessed)
            {
                AlrtMsgs.ShowMessageError(result.Message);
                return;
            }
            else
            {
                AlrtMsgs.SaveSuccess();
            }
            if (_isReturnSupplierID)
            {
                ReturnedSupplierID = result.Object.Id;
                Selected = true;
                Close();
            }
            else
            {
                ClearForm();
                FillDGV();
            }
        }

        private void btn_cancel_Click(object sender, EventArgs e)
        {
            ClearForm();
        }
        #endregion

        #region Clear Form
        protected override void ClearForm()
        {
            txtAddress.Text = txtMob1.Text = txtMob2.Text = txtName.Text = txt_search.Text = "";
            cmbCountries.SelectedValue = 1;
            btnCancel.Visible = btnSaveChanges.Visible = false;
            btnAddCustomer.Visible = true;
            ClearControls(false, cmbCountries, cmbCities, cmbAreas);
        }
        #endregion

        #region Country and city changes
        private void cmbCountries_SelectedIndexChanged(object sender, EventArgs e)
        {
            var countryID = cmbCountries.GetSelectedID(null);
            var cities = db.Cities.Where(x => !x.IsDeleted && x.CountryId == countryID).ToList();
            CommonMethods.FillComboBox(cmbCities, cities, "Name", "Id");
            cmbCities.SelectedIndex = 0;
        }

        private void cmbCities_SelectedIndexChanged(object sender, EventArgs e)
        {
            var cityID = cmbCities.GetSelectedID(null);
            var areas = db.Areas.Where(x => !x.IsDeleted && x.CityId == cityID).ToList();
            CommonMethods.FillComboBox(cmbAreas, areas, "Name", "Id");
            cmbAreas.SelectedIndex = 0;
        }
        #endregion

        #region Search

        private void txt_search_TextChanged(object sender, EventArgs e)
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
            dgvSupplier.AutoGenerateColumns = false;
            if (txt_search.TextLength > 2)
            {
                dgvSupplier.DataSource = people.Where(x => x.Name.Contains(txt_search.Text) || x.Mob1.Contains(txt_search.Text) || x.Mob2?.Contains(txt_search.Text) == true).ToList();
            }
            else
            {
                dgvSupplier.DataSource = people;
            }
        }



        #endregion

        #region Navigate between controls
        private void txt_name_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                txtMob1.Focus();
            }
        }
        private void txt_mob1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                txtMob2.Focus();
            }
        }

        private void txt_mob2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                cmbCountries.Focus();
                cmbCities.DroppedDown = true;
            }
        }

        private void cmbCountries_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                cmbCities.Focus();
                cmbCities.DroppedDown = true;
            }
        }

        private void cmbCities_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                cmbAreas.Focus();
                cmbAreas.DroppedDown = true;
            }
        }

        private void cmbAreas_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                txtAddress.Focus();
            }
        }

        private void txt_search_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Down)
            {
                dgvSupplier.Focus();
            }
        }

        #endregion

        #region Exit From Form
        private void DeliveryCustomer_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }
        #endregion

        #region Select Supplier from DataGridView
        private void dgv_customer_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (_isReturnSupplierID && e.RowIndex >= 0)
            {
                ReturnedSupplierID = Guid.Parse(dgvSupplier.Rows[e.RowIndex].Cells[nameof(Person_ID)].Value + "");
                Selected = true;
                this.Close();
            }
        }
        private void dgv_customer_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter && _isReturnSupplierID && dgvSupplier.CurrentRow != null)
            {
                ReturnedSupplierID = Guid.Parse(dgvSupplier.CurrentRow.Cells[nameof(Person_ID)].Value + "");
                Selected = true;
                this.Close();
            }
        }
        #endregion

        private void txtNumbersOnly_KeyPress(object sender, KeyPressEventArgs e)
        {
            Validations.txtIsNumberOnly(e);
        }
    }
}
