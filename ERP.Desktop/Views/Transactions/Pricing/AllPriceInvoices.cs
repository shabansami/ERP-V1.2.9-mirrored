using ERP.Desktop.Views._Main;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ERP.Desktop.Services.Transactions;
using System.Windows.Forms;
using ERP.Desktop.Utilities;
using ERP.DAL;
using System.Linq.Expressions;
using ERP.Desktop.ViewModels;

namespace ERP.Desktop.Views.Transactions.Pricing
{
    public partial class AllPriceInvoices : BaseForm
    {
        PriceInvoiceServices _services;
        public QuoteOrderSell ReturnedInvoice;
        TypeAssistant assistant;
        public AllPriceInvoices(PriceInvoiceServices saleServices)
        {
            InitializeComponent();
            assistant = new TypeAssistant();
            assistant.Idled += assistant_Idled;
            this._services = saleServices;
        }

        #region Load and fill
        private void AllInvoices_Load(object sender, EventArgs e)
        {
            DGVFill();
        }


        void DGVFill()
        {
            dgvAllinvoices.AutoGenerateColumns = false;
            dgvAllinvoices.DataSource = _services.GetAllInvoices();
        }
        #endregion

        #region Fill Invoice Details
        void FillInvoiceDetails(Guid invoiceID)
        {
            DGV_Details.AutoGenerateColumns = false;
            DGV_Details.DataSource = _services.GetInvoiceDetails(invoiceID);
        }
        #endregion

        #region Searching
        void assistant_Idled(object sender, EventArgs e)
        {
            this.Invoke(
            new MethodInvoker(() =>
            {
                //The method we want to delay
                Search();
            }));
        }
        void Search()
        {
            dgvAllinvoices.AutoGenerateColumns = false;
            dgvAllinvoices.DataSource = _services.GetAllItemsFilter(txtSearchText.Text);
            dgvAllinvoices.Refresh();
        }



        private void txtSearchText_TextChanged(object sender, EventArgs e)
        {
            assistant.TextChanged();
        }
        #endregion

        #region Select invoice to view details or open or show in Cashier Form
        private void dgvAllinvoices_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;
            if (Guid.TryParse(dgvAllinvoices.Rows[e.RowIndex].Cells[nameof(Invo_ID)].Value + "", out Guid id))
            {
                if (e.ColumnIndex == dgvAllinvoices.Columns[nameof(show)].Index)
                {
                    //To show the invoice in cashier form
                    var result = _services.GetInvoice(id);
                    if (!result.IsSuccessed)
                    {
                        AlrtMsgs.ShowMessageError(result.Message);
                        return;
                    }
                    ReturnedInvoice = result.Object;
                    Close();

                }
                else
                {
                    //to show invoice details in the same form
                    FillInvoiceDetails(id);
                }
            }
        }
        #endregion

    }
}
