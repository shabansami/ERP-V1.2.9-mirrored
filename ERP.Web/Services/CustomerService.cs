using ERP.Web.DataTablesDS;
using ERP.DAL;
using ERP.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using static ERP.Web.Utilites.Lookups;

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

    }
}