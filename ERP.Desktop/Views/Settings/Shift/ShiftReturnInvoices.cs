﻿using ERP.Desktop.Views._Main;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using ERP.Desktop.Utilities;
using ERP.Desktop.Services;
using System.Linq;
using ERP.Desktop.DTOs;
using System.Collections.Generic;
using static ERP.Desktop.Utilities.CommonMethods;
using ERP.DAL;
using ERP.Desktop.Services.Settings;
using ERP.Desktop.ViewModels;

namespace ERP.Desktop.Views.Settings.Shift
{
    public partial class ShiftReturnInvoices : BaseForm
    {
        ShiftServices _services;
        #region Constructors

        int id = 0;
        public ShiftReturnInvoices() : base()
        {
            InitializeComponent();
            _services = new ShiftServices();
        }
        #endregion

        #region Load
        private void ShiftInvoices_Load(object sender, EventArgs e)
        {
            FillDGV();
            dtpInvoiceDate.Value = DateTime.Now;
        }


        void FillDGV()
        {
            dgv_shiftInvoices.AutoGenerateColumns = false;
            //if(Properties.Settings.Default.ShiftId > 0)
            //    dgv_shiftInvoices.DataSource = _services.GetAllShiftReturnInvoices(Properties.Settings.Default.ShiftId);
            StyleDataGridViews(dgv_shiftInvoices);
        }


        private void dgv_shift_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
           
        }

        #endregion

        private void btnSearch_Click(object sender, EventArgs e)
        {
            //dgv_shiftInvoices.DataSource = _services.GetSomeShiftReturnInvoices(dtpInvoiceDate.Value);
        }

        private void btnAll_Click(object sender, EventArgs e)
        {
            //dgv_shiftInvoices.DataSource = _services.GetAllReturnInvoices();
        }
    }
}
