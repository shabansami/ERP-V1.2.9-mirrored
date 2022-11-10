using ERP.Web.DataTablesDS;
using ERP.DAL;
using ERP.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using static ERP.Web.Utilites.Lookups;
using ERP.Web.Utilites;

namespace ERP.Web.Services
{
    public class CustomerService
    {
        //مديونية العميل او المورد
        public double GetPersonBalance(Guid? accountId)
        {
            IQueryable<GeneralDaily> generalDailies = null;
            using (var db = new VTSaleEntities())
            {
                generalDailies = db.GeneralDailies.Where(x => !x.IsDeleted);
                if (accountId != null)
                    generalDailies = generalDailies.Where(x => x.AccountsTreeId == accountId);
                else
                    return 0;
                double balance= generalDailies.Select(x=>x.Debit-x.Credit ).DefaultIfEmpty(0).Sum();
                return balance;
            }
        }

        #region تقارير فواتير العملاء

        public CustomerAccountVM GetCustomerAccount(Guid? customerId, DateTime? dtFrom, DateTime? dtTo, bool allTimes, bool isFinalApproval)
        {
            using (var db = new VTSaleEntities())
            {
                CustomerAccountVM customerAccountVm = new CustomerAccountVM();
                //فواتير البيع 
                var sell = db.SellInvoices.Where(x => !x.IsDeleted && x.CustomerId == customerId);
                if (isFinalApproval)
                    sell = sell.Where(x => x.IsFinalApproval);
                if (dtFrom != null && dtTo != null && !allTimes)
                    sell = sell.Where(x => DbFunctions.TruncateTime(x.InvoiceDate) >= dtFrom && DbFunctions.TruncateTime(x.InvoiceDate) <= dtTo);
                customerAccountVm.SellInvoices = sell.Select(x => new RptInvoiceDto
                {
                    Id = x.Id,
                    InvoiceNumber=x.InvoiceNumber,
                    InvoDate = x.InvoiceDate,
                    CustomerName = x.PersonCustomer.Name,
                    Amount = x.Safy,
                    ApprovalStatus = x.IsFinalApproval ? "معتمده" : "لم يتم اعتمادها بعد"
                }).ToList();
                customerAccountVm.TotalSellInvoices = customerAccountVm.SellInvoices.Sum(x => x.Amount);
                //فواتير مرتجع البيع 
                var sellBack = db.SellBackInvoices.Where(x => !x.IsDeleted && x.CustomerId == customerId);
                if (isFinalApproval)
                    sellBack = sellBack.Where(x => x.IsFinalApproval);
                if (dtFrom != null && dtTo != null && !allTimes)
                    sellBack = sellBack.Where(x => DbFunctions.TruncateTime(x.InvoiceDate) >= dtFrom && DbFunctions.TruncateTime(x.InvoiceDate) <= dtTo);
                customerAccountVm.SellBackInvoices = sellBack.Select(x => new RptInvoiceDto
                {
                    Id = x.Id,
                    InvoiceNumber=x.InvoiceNumber,
                    InvoDate = x.InvoiceDate,
                    CustomerName = x.PersonCustomer.Name,
                    Amount = x.Safy,
                    ApprovalStatus = x.IsFinalApproval ? "معتمده" : "لم يتم اعتمادها بعد"
                }).ToList();
                customerAccountVm.TotalSellBackInvoices = customerAccountVm.SellBackInvoices.Sum(x => x.Amount);
                // التحصيل النقدى 
                var customerPayment = db.CustomerPayments.Where(x => !x.IsDeleted && x.CustomerId == customerId);
                if (isFinalApproval)
                    customerPayment = customerPayment.Where(x => x.IsApproval);
                if (dtFrom != null && dtTo != null && !allTimes)
                    customerPayment = customerPayment.Where(x => DbFunctions.TruncateTime(x.PaymentDate) >= dtFrom && DbFunctions.TruncateTime(x.PaymentDate) <= dtTo);
                customerAccountVm.CustomerPayments = customerPayment.Select(x => new RptInvoiceDto
                {
                    Id = x.Id,
                    InvoDate = x.PaymentDate,
                    CustomerName = x.PersonCustomer.Name,
                    Amount = x.Amount,
                    ApprovalStatus = x.IsApproval ? "معتمده" : "لم يتم اعتمادها بعد"
                }).ToList();
                var totalVal = customerAccountVm.CustomerPayments.Sum(x => x.Amount);
                customerAccountVm.TotalCustomerPayments = totalVal;

                // الشيكات 
                var cheque = db.Cheques.Where(x => !x.IsDeleted && x.CustomerId == customerId);
                if (isFinalApproval)
                    cheque = cheque.Where(x => x.IsCollected || x.IsRefused);
                if (dtFrom != null && dtTo != null && !allTimes)
                    cheque = cheque.Where(x => DbFunctions.TruncateTime(x.CheckDate) >= dtFrom && DbFunctions.TruncateTime(x.CheckDate) <= dtTo);
                customerAccountVm.Cheques = cheque.Select(x => new RptInvoiceDto
                {
                    Id = x.Id,
                    InvoDate = x.CheckDate,
                    CustomerName = x.PersonCustomer.Name,
                    Amount = x.Amount,
                    ApprovalStatus = x.IsCollected ? "تم تحصيله" : x.IsRefused ? "تم رفضه/إرجاعه" : "تحت التحصيل"
                }).ToList();
                customerAccountVm.TotalCheques = customerAccountVm.Cheques.Sum(x => x.Amount);


                //الاقساط
                IQueryable<InstallmentSchedule> installmentSchedules = null;

                installmentSchedules = db.InstallmentSchedules.Where(x => !x.IsDeleted);
                if (customerId != null)
                    installmentSchedules = installmentSchedules.Where(x => x.Installment.SellInvoice.CustomerId == customerId || x.Installment.CustomerIntialBalance.PersonId == customerId);
                if (dtFrom != null && !allTimes)
                    installmentSchedules = installmentSchedules.Where(x => DbFunctions.TruncateTime(x.InstallmentDate) >= dtFrom);
                if (dtTo != null && !allTimes)
                    installmentSchedules = installmentSchedules.Where(x => DbFunctions.TruncateTime(x.InstallmentDate) <= dtTo);

                var paidInstallments = installmentSchedules;
                var paidNotInTimeInstallments = installmentSchedules;
                var notPaidInstallments = installmentSchedules;
                customerAccountVm.PaidInstallments = paidInstallments.Where(x => x.IsPayed && x.PaidInTime).Select(
                    x => new SellInvoiceInstallmentScheduleDto
                    {
                        InvoiceNum=x.Installment.SellInvoice!=null ? x.Installment.SellInvoice.InvoiceNumber:x.Installment.CustomerIntialBalance!=null?x.Installment.IntialCustomerId.ToString():null,
                        Amount = x.Amount,
                        ScheduleId = x.Id,
                        PaidInTime = x.PaidInTime,
                        InstallmentDate = x.InstallmentDate.ToString(),
                        PaymentDate = x.PaymentDate.ToString(),
                    }
                    ).ToList();
                customerAccountVm.TotalPaidInstallment = customerAccountVm.PaidInstallments.Sum(x => x.Amount);

                customerAccountVm.PaidNotInTimeInstallments = paidNotInTimeInstallments.Where(x => x.IsPayed && !x.PaidInTime).Select(
                     x => new SellInvoiceInstallmentScheduleDto
                     {
                         InvoiceNum = x.Installment.SellInvoice != null ? x.Installment.SellInvoice.InvoiceNumber : x.Installment.CustomerIntialBalance != null ? x.Installment.IntialCustomerId.ToString() : null,
                         Amount = x.Amount,
                         ScheduleId = x.Id,
                         PaidInTime = x.PaidInTime,
                         InstallmentDate = x.InstallmentDate.ToString(),
                         PaymentDate = x.PaymentDate.ToString(),
                     }
                     ).ToList();
                customerAccountVm.TotalPaidNotInTimeInstallment = customerAccountVm.PaidNotInTimeInstallments.Sum(x => x.Amount);

                customerAccountVm.NotPaidInstallments = notPaidInstallments.Where(x => !x.IsPayed).Select(
                     x => new SellInvoiceInstallmentScheduleDto
                     {
                         InvoiceNum = x.Installment.SellInvoice != null ? x.Installment.SellInvoice.InvoiceNumber : x.Installment.CustomerIntialBalance != null ? x.Installment.IntialCustomerId.ToString() : null,
                         ScheduleId = x.Id,
                         PaidInTime = x.PaidInTime,
                         Amount = x.Amount,
                         InstallmentDate = x.InstallmentDate.ToString(),
                         PaymentDate = x.PaymentDate.ToString(),
                     }
                     ).ToList();
                customerAccountVm.TotalNotPaidInstallment = customerAccountVm.NotPaidInstallments.Sum(x => x.Amount);

                return customerAccountVm;


            }
        }
        #endregion

        #region تقرير اعمار الديون
        public List<AgesDebtDto> AgesDebt(Guid? branchId, Guid? customerId, bool? allCustomers)
        {
            IQueryable<SellInvoice> sellInvoices = null;
            List<AgesDebtDto> rpt = new List<AgesDebtDto>();
            //فواتير الاجل والجزئية 
            using (VTSaleEntities db=new VTSaleEntities())
            {
                sellInvoices = db.SellInvoices.Where(x => !x.IsDeleted &&x.IsFinalApproval&& (x.PaymentTypeId == (int)PaymentTypeCl.Deferred) || x.PaymentTypeId == (int)PaymentTypeCl.Partial);
                if (branchId!=null)
                    sellInvoices = sellInvoices.Where(x => x.BranchId == branchId);
                if (customerId!=null&&allCustomers==false)
                    sellInvoices = sellInvoices.Where(x => x.CustomerId == customerId);
                
                sellInvoices = sellInvoices.Where(x => x.Safy != x.SellInvoicePayments.Where(p => !p.IsDeleted).DefaultIfEmpty().Sum(p => p.Amount));
                var invoicesList = sellInvoices.ToList();
                foreach (var invoice in invoicesList)
                {
                    //مقارنة تاريخ اليوم مع تاريخ الاستحقاق لاحتساب عمر الدين 
                    var currentDate = Utility.GetDateTime();
                    var dueDate = invoice.DueDate;
                    if (dueDate!=null)
                    {
                        var ageDebt = new AgesDebtDto();
                        ageDebt.CustomerName = invoice.PersonCustomer?.Name;
                        ageDebt.InvoiceNumber = invoice.InvoiceNumber;
                        ageDebt.DebtAmount =Math.Round(invoice.Safy - invoice.SellInvoicePayments.Where(p => !p.IsDeleted).Sum(p => (double?)p.Amount??0),2,MidpointRounding.ToEven);
                        ageDebt.InvoiceDate = invoice.InvoiceDate.ToString("yyyy-MM-dd");
                        //ageDebt.TotalAmount = invoice.SellInvoicePayments.Where(p => !p.IsDeleted).DefaultIfEmpty().Sum(p => p.Amount);
                        ageDebt.AgeDebt = (currentDate.Date - dueDate.Value.Date).Days;
                        //عمر الدين بالسالب اى لم يحن استحقاقه بعد 
                        if (ageDebt.AgeDebt<0)
                            ageDebt.AgeDebtNotDue = ageDebt.DebtAmount;
                        //عمر الدين 30 يوم 
                        else if (ageDebt.AgeDebt>=0&& ageDebt.AgeDebt<=30)
                            ageDebt.AgeDebt30 =ageDebt.DebtAmount;
                        //عمر الدين 60 يوم 
                        else if (ageDebt.AgeDebt>=31&& ageDebt.AgeDebt<=60)
                            ageDebt.AgeDebt60 =ageDebt.DebtAmount;
                        //عمر الدين 90 يوم 
                        else if (ageDebt.AgeDebt>=61&& ageDebt.AgeDebt<=90)
                            ageDebt.AgeDebt90 =ageDebt.DebtAmount;
                        //عمر الدين 120 يوم 
                        else if (ageDebt.AgeDebt>=91&& ageDebt.AgeDebt<=120)
                            ageDebt.AgeDebt120 =ageDebt.DebtAmount;
                        //عمر الدين 150 يوم 
                        else if (ageDebt.AgeDebt>=121&& ageDebt.AgeDebt<=150)
                            ageDebt.AgeDebt150 =ageDebt.DebtAmount;
                        //عمر الدين 180 يوم 
                        else if (ageDebt.AgeDebt>=151&& ageDebt.AgeDebt<=180)
                            ageDebt.AgeDebt180 =ageDebt.DebtAmount;
                        //عمر الدين اكبر من 180 يوم 
                        else
                            ageDebt.AgeDebtOver180 = ageDebt.DebtAmount;
                        
                        rpt.Add(ageDebt);
                    }
                }

            }
            return rpt;
        }
        #endregion

    }
}