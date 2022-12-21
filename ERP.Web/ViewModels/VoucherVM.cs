using ERP.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ERP.Web.ViewModels
{
    public class VoucherVM
    {
        public Guid Id { get; set; }
        public Nullable<Guid> BranchId { get; set; }
        public Nullable<System.DateTime> VoucherDate { get; set; }
        public Nullable<Guid> AccountTreeId { get; set; } //الحساب الدائن فى حاالة سند صرف والحساب المدين فى حالة سند قبض
        public string Notes { get; set; }

        public Nullable<Guid> SelectedAccountTreeId { get; set; }
        public double InsertedAmount { get; set; }
        public string InsertedNotes { get; set; }

    }
    public class VoucherDetailDT
    {
        //public Guid GeneralRecordDetailId { get; set; }
        //public Guid GeneralRecordId { get; set; }
        public Nullable<Guid> AccountTreeIdDT { get; set; }
        public Nullable<long> AccountTreeNumDT { get; set; }
        public string AccountTreeNameDT { get; set; }
        //public Nullable<int> DebitCredit { get; set; }
        public double DebitAmount { get; set; }
        public double CreditAmount { get; set; }
        public string Notes { get; set; }
        public string Actions { get; set; }

    }
}