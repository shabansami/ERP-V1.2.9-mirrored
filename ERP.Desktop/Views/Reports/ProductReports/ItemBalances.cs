using ERP.DAL;
using ERP.Desktop.ViewModels;
using ERP.Desktop.Views._Main;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static ERP.Desktop.Utilities.CommonMethods;
using ERP.Desktop.Utilities;
using ERP.Desktop.Services.Reports.ItemReports;
using ERP.Desktop.DTOs;
using PrintEngine.HTMLPrint;
using ERP.Desktop.Services;
using ERP.Desktop.Services.Employees;

namespace ERP.Desktop.Views.Reports.ProductReports
{
    public partial class ItemBalances : BaseForm
    {
        VTSaleEntities db;
        BalanceService _service;
        List<ItemBalanceDto> items;
        public ItemBalances()
        {
            InitializeComponent();
            db = new VTSaleEntities();
            _service = new BalanceService(db);
        }

        #region Load and Fill
        private void ItemBalances_Load(object sender, EventArgs e)
        {
            FillCombos();
        }

        private void FillCombos()
        {
            var groups = db.Groups.Where(x => !x.IsDeleted).Select(x => new IDNameVM { ID = x.Id, Name = x.Name }).ToList();
            FillComboBox(cmbGroups, groups, "Name", "ID");

            var itemTypes = db.ItemTypes.Where(x => !x.IsDeleted).Select(x => new IDNameVM { Name = x.Name, ID = x.Id }).ToList();
            FillComboBox(cmbItemTypes, itemTypes, "Name", "ID");

            var branches = EmployeeService.GetBranchesByUser(UserServices.UserInfo);
            //var branches = db.Branches.Where(x => !x.IsDeleted).Select(x => new IDNameVM { Name = x.Name, ID = x.Id }).ToList();
            FillComboBox(cmbBranchs, branches, "Name", "ID");
        }


        #endregion

        #region Search
        private async void btnSearch_Click(object sender, EventArgs e)
        {
            btnSearch.Enabled = false;



            var groupID = cmbGroups.GetSelectedID(null);
            var itemTypeID = cmbItemTypes.GetSelectedID(null);
            var branchID = cmbBranchs.GetSelectedID(null);
            var storeID = cmbStores.GetSelectedID(null);
            items = new List<ItemBalanceDto>();
            await Task.Run(() =>
            {
                items = _service.SearchItemBalance(
                    txtItemCode.Text,
                    txtBarCode.Text,
                    groupID,
                    itemTypeID,
                    null,
                    branchID,
                    storeID,
                    false,
                    txtSearch.Text
                    );
            });
            dgvItemBalance.AutoGenerateColumns = false;
            dgvItemBalance.DataSource = items;
            btnSearch.Enabled = true;
        }
        #endregion

        #region Printing
        private void printButton1_Click(object sender, EventArgs e)
        {
            Report report = new Report();

            report.ReportTitle = "تقرير ارصدة الاصناف ";
            report.ReportSource = items.ToDataTable();

            report.ReportFields.Add(new Field("ItemName", "اسم الصنف"));
            report.ReportFields.Add(new Field("GroupName", "المجموعة"));
            report.ReportFields.Add(new Field("ItemTypeName", "نوع الصنف"));
            report.ReportFields.Add(new Field("StoreName", "المخزن"));
            report.ReportFields.Add(new Field("Balance", "الرصيد"));
            report.ReportFields.Add(new Field("ItemCode", "كود الصنف"));
            report.ReportFields.Add(new Field("BarCode", "الباركود"));

            report.FontSize = 12;
            //Generate the prepReport tables
            report.GenerateReport();
            report.ShowPrintPreviewDialog();
        }
        #endregion

        private void cmbBranchs_SelectedIndexChanged(object sender, EventArgs e)
        {
            var branchID = cmbBranchs.GetSelectedID(null);
            var stores = db.Stores.Where(x => !x.IsDeleted && x.BranchId == branchID).Select(x => new IDNameVM() { ID = x.Id, Name = x.Name }).ToList();
            FillComboBox(cmbStores, stores, "Name", "ID");
        }
    }
}
