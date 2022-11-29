using ERP.Web.DataTablesDS;
using ERP.DAL;
using ERP.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using static ERP.Web.Utilites.Lookups;
using ERP.DAL.Models;

namespace ERP.Web.Services
{
    public class SellInvoiceInstallmentService
    {
        //فواتير عملاء لهم اقساط فواتير بيع وارصدة
        public List<SellInvoiceInstallmentDto> GetSellInvoiceInstallments(DateTime? dtFrom, DateTime? dtTo, Guid? customerId, bool hasInstallments)
        {
            IQueryable<SellInvoice> sellInvoices = null;
            using (var db = new VTSaleEntities())
            {
                sellInvoices = db.SellInvoices.Where(x => !x.IsDeleted);
                if (hasInstallments)
                    sellInvoices = sellInvoices.Where(x => x.PaymentTypeId == (int)PaymentTypeCl.Installment && x.RemindValue > 0);
                else
                    sellInvoices = sellInvoices.Where(x => !x.Installments.Where(i => !i.IsDeleted).Any() && x.PaymentTypeId == (int)PaymentTypeCl.Installment && x.RemindValue > 0 || (x.PaymentTypeId == (int)PaymentTypeCl.Deferred && x.RemindValue > 0) || (x.PaymentTypeId == (int)PaymentTypeCl.Partial && x.RemindValue > 0));

                if (customerId != null)
                    sellInvoices = sellInvoices.Where(x => x.CustomerId == customerId);
                if (dtFrom != null)
                    sellInvoices = sellInvoices.Where(x => DbFunctions.TruncateTime(x.InvoiceDate) >= dtFrom);
                if (dtTo != null)
                    sellInvoices = sellInvoices.Where(x => DbFunctions.TruncateTime(x.InvoiceDate) <= dtTo);

                var list = sellInvoices.ToList().Select(
                    x => new SellInvoiceInstallmentDto
                    {
                        CustomerName = x.PersonCustomer != null ? x.PersonCustomer.Name : null,
                        InvoiceId = x.Id,
                        PaymentTypeId = x.PaymentTypeId,
                        PaymentTypeName = x.PaymentTypeId != null ? x.PaymentType.Name : null,
                        InvoiceNum = x.InvoiceNumber,
                        Safy = x.Safy,
                        PayedValue = x.PayedValue+ (x.CustomerPayments.Where(c => !c.IsDeleted).Sum(c => (double?)c.Amount ?? 0)),
                        RemindValue = x.RemindValue-(x.CustomerPayments.Where(c=>!c.IsDeleted).Sum(c=>(double?)c.Amount??0)),
                        InvoiceDate = x.InvoiceDate.ToString(),
                        AnySchedules = x.Installments.Where(i => !i.IsDeleted && i.InstallmentSchedules.Where(d => !d.IsDeleted).Any()).Any(),
                        StatusTxt = "فاتورة بيع",
                        IsSell = true

                    }
                    ).ToList();

                IQueryable<PersonIntialBalance> custIntialBalances = null;
                if (hasInstallments)
                {
                    custIntialBalances = db.PersonIntialBalances.Where(x => !x.IsDeleted && x.Installments.Where(i => !i.IsDeleted).Any());
                    if (customerId != null)
                        custIntialBalances = custIntialBalances.Where(x => x.PersonId == customerId && x.IsCustomer);
                    if (dtFrom != null)
                        custIntialBalances = custIntialBalances.Where(x => DbFunctions.TruncateTime(x.OperationDate) >= dtFrom);
                    if (dtTo != null)
                        custIntialBalances = custIntialBalances.Where(x => DbFunctions.TruncateTime(x.OperationDate) <= dtTo);

                    var list2 = custIntialBalances.Select(
                        x => new SellInvoiceInstallmentDto
                        {
                            CustomerName = x.Person != null ? x.Person.Name : null,
                            InvoiceId = x.Id,
                            //PaymentTypeId=x.PaymentTypeId,
                            //PaymentTypeName =x.PaymentTypeId!=null?x.PaymentType.Name:null,
                            InvoiceNum = x.Id.ToString(),
                            Safy = x.Amount,
                            PayedValue = 0,
                            RemindValue = x.Amount,
                            InvoiceDate = x.OperationDate.ToString(),
                            //InvoiceGuid = x.InvoiceGuid,
                            AnySchedules = x.Installments.Where(i => !i.IsDeleted && i.InstallmentSchedules.Where(d => !d.IsDeleted).Any()).Any(),
                            StatusTxt = "رصيد اول لعميل",
                            IsSell = false
                        }
                        ).ToList();
                    list.AddRange(list2);

                }
                return list;

            }
        }

        //اقساط تم تحصيلها فى موعدها /تم تحصيلها فى متأخرة عن موعدها 

        public List<SellInvoiceInstallmentScheduleDto> GetInstallmentSchedules(bool isPaid, DateTime? dtFrom, DateTime? dtTo, Guid? customerId)
        {
            IQueryable<InstallmentSchedule> installmentSchedules = null;
            using (var db = new VTSaleEntities())
            {
                installmentSchedules = db.InstallmentSchedules.Where(x => !x.IsDeleted && x.IsPayed == isPaid);
                if (customerId != null)
                    installmentSchedules = installmentSchedules.Where(x => x.Installment.SellInvoice.CustomerId == customerId || x.Installment.CustomerIntialBalance.PersonId == customerId);
                if (dtFrom != null)
                    installmentSchedules = installmentSchedules.Where(x => DbFunctions.TruncateTime(x.InstallmentDate) >= dtFrom);
                if (dtTo != null)
                    installmentSchedules = installmentSchedules.Where(x => DbFunctions.TruncateTime(x.InstallmentDate) <= dtTo);

                //IQueryable<InstallmentSchedule> installmentSchedules2 = installmentSchedules;
                var list = installmentSchedules.ToList().OrderBy(x=>x.InstallmentDate).Select(
                    x => new SellInvoiceInstallmentScheduleDto
                    {
                        CustomerName = x.Installment.SellInvoice != null ? x.Installment.SellInvoice.PersonCustomer.Name : x.Installment.CustomerIntialBalance.Person.Name,
                        ScheduleId = x.Id,
                        InvoiceNum = x.Installment.SellInvoiceId != null ? x.Installment.SellInvoice.InvoiceNumber : x.Installment.CustomerIntialBalance.Id.ToString(),
                        InvoiceDate = x.Installment.SellInvoice != null ? x.Installment.SellInvoice.InvoiceDate.ToString() : x.Installment.CustomerIntialBalance.OperationDate.ToString(),
                        PaidInTime = x.PaidInTime,
                        InstallmentDate = x.InstallmentDate?.ToString("yyyy-MM-dd"),
                        PaymentDate = x.PaymentDate.ToString(),
                        Amount = x.Amount,
                        StatusTxt = x.Installment.SellInvoiceId != null ? "فاتورة بيع" : "رصيد اول لعميل",
                    }
                    ).ToList();
                //var list2 = installmentSchedules2.Select(
                //    x => new SellInvoiceInstallmentScheduleDto
                //    {
                //        CustomerName = x.Installment.PersonIntialBalance.Person != null ? x.Installment.PersonIntialBalance.Person.Name : null,
                //        Id = x.Id,
                //        InvoiceNum = x.Installment.PersonIntialBalance.Id,
                //        InvoiceDate = x.Installment.PersonIntialBalance.OperationDate.ToString(),
                //        ScheduleGuid = x.ScheduleGuid,
                //        PaidInTime = x.PaidInTime,
                //        InstallmentDate=x.InstallmentDate.ToString(),
                //        PaymentDate=x.PaymentDate.ToString(),
                //        Amount=x.Amount,
                //        StatusTxt = "رصيد اول لعميل",

                //    }
                //    ).ToList();

                return list;
            }
        }

        //اقساط لم يتم تحصيلها  تم تحصيلها فى موعدها /تم تحصيلها فى متأخرة عن موعدها 

        public SellInvoiceInstallmentScheduleCollectionVM GetInstallmentSchedulesCollections(Guid? invoiceGuid, Guid? custInitialId)
        {
            if (invoiceGuid == null && custInitialId == null)
                return new SellInvoiceInstallmentScheduleCollectionVM();
            SellInvoiceInstallmentScheduleCollectionVM scheduleCollection = new SellInvoiceInstallmentScheduleCollectionVM();
            IQueryable<InstallmentSchedule> installmentSchedules = null;
            using (var db = new VTSaleEntities())
            {
                if (invoiceGuid != null)
                    installmentSchedules = db.InstallmentSchedules.Where(x => !x.IsDeleted && x.Installment.SellInvoice.Id == invoiceGuid);
                else if (custInitialId != null)
                    installmentSchedules = db.InstallmentSchedules.Where(x => !x.IsDeleted && x.Installment.CustomerIntialBalance.Id == custInitialId);
                else
                    return new SellInvoiceInstallmentScheduleCollectionVM();

                var paidInstallments = installmentSchedules.OrderBy(x => x.InstallmentDate);
                var paidNotInTimeInstallments = installmentSchedules.OrderBy(x => x.InstallmentDate);
                var notPaidInstallments = installmentSchedules.OrderBy(x => x.InstallmentDate);
                scheduleCollection.PaidInstallments = paidInstallments.Where(x => x.IsPayed && x.PaidInTime).ToList().OrderBy(x=>x.InstallmentDate).Select(
                    x => new SellInvoiceInstallmentScheduleDto
                    {
                        Amount = x.Amount,
                        ScheduleId = x.Id,
                        PaidInTime = x.PaidInTime,
                        InstallmentDate = x.InstallmentDate?.ToString("yyyy-MM-dd"),
                        PaymentDate = x.PaymentDate.ToString(),
                    }
                    ).ToList();
                scheduleCollection.PaidNotInTimeInstallments = paidNotInTimeInstallments.Where(x => x.IsPayed && !x.PaidInTime).ToList().OrderBy(x => x.InstallmentDate).Select(
                     x => new SellInvoiceInstallmentScheduleDto
                     {
                         Amount = x.Amount,
                         ScheduleId = x.Id,
                         PaidInTime = x.PaidInTime,
                         InstallmentDate = x.InstallmentDate?.ToString("yyyy-MM-dd"),
                         PaymentDate = x.PaymentDate.ToString(),
                     }
                     ).ToList();
                scheduleCollection.NotPaidInstallments = notPaidInstallments.Where(x => !x.IsPayed).ToList().OrderBy(x => x.InstallmentDate).Select(
                     x => new SellInvoiceInstallmentScheduleDto
                     {
                         ScheduleId = x.Id,
                         PaidInTime = x.PaidInTime,
                         Amount = x.Amount,
                         InstallmentDate = x.InstallmentDate?.ToString("yyyy-MM-dd"),
                         PaymentDate = x.PaymentDate.ToString(),
                     }
                     ).ToList();

                //تفاصسل الفاتورة
                if (invoiceGuid != null)
                {
                    var invoice = db.SellInvoices.Where(x => x.Id == invoiceGuid).FirstOrDefault();
                    if (invoice != null)
                    {
                        scheduleCollection.SellInvoiceId = invoice.Id;
                        scheduleCollection.InvoiceNumber = invoice.InvoiceNumber;
                        scheduleCollection.CustomerName = invoice.PersonCustomer != null ? invoice.PersonCustomer.Name : null;
                        scheduleCollection.InvoiceDate = invoice.InvoiceDate;
                        scheduleCollection.Safy = invoice.Safy;
                        scheduleCollection.RemindValue = invoice.RemindValue;
                        scheduleCollection.TotalValue = invoice.Installments.Where(x => !x.IsDeleted).FirstOrDefault() != null ? invoice.Installments.Where(x => !x.IsDeleted).FirstOrDefault().TotalValue : 0;
                        scheduleCollection.Duration = invoice.Installments.Where(x => !x.IsDeleted).FirstOrDefault() != null ? invoice.Installments.Where(x => !x.IsDeleted).FirstOrDefault().Duration : 0;
                    }

                }
                else if (custInitialId != null)
                {
                    var invoice = db.PersonIntialBalances.Where(x => x.Id == custInitialId).FirstOrDefault();
                    if (invoice != null)
                    {
                        scheduleCollection.SellInvoiceId = invoice.Id;
                        scheduleCollection.InvoiceNumber = invoice.Id.ToString();
                        scheduleCollection.CustomerName = invoice.Person != null ? invoice.Person.Name : null;
                        scheduleCollection.InvoiceDate = invoice.OperationDate;
                        scheduleCollection.Safy = invoice.Amount;
                        scheduleCollection.RemindValue = invoice.Amount;
                        scheduleCollection.TotalValue = invoice.Installments.Where(x => !x.IsDeleted).FirstOrDefault() != null ? invoice.Installments.Where(x => !x.IsDeleted).FirstOrDefault().TotalValue : 0;
                        scheduleCollection.Duration = invoice.Installments.Where(x => !x.IsDeleted).FirstOrDefault() != null ? invoice.Installments.Where(x => !x.IsDeleted).FirstOrDefault().Duration : 0;
                    }
                }
                else
                    return new SellInvoiceInstallmentScheduleCollectionVM();

                return scheduleCollection;
            }
        }

        // اقساط عملاء لم يسددوا 
        public List<InstallmentNotPaidDto> InstallmentNotPaid()
        {
            List<InstallmentNotPaidDto> list = new List<InstallmentNotPaidDto>();
            using (var db = new VTSaleEntities())
            {
                IQueryable<InstallmentSchedule> installmentSchedules = null;
                installmentSchedules = db.InstallmentSchedules.Where(x => !x.IsDeleted && !x.IsPayed);
                var groupSchedules = installmentSchedules.ToList().Select(x => new
                {
                    CustomerName = x.Installment.SellInvoice != null ? x.Installment.SellInvoice.PersonCustomer.Name : x.Installment.CustomerIntialBalance.Person.Name,
                    Mob = x.Installment.SellInvoice != null ? x.Installment.SellInvoice.PersonCustomer.Mob1 : x.Installment.CustomerIntialBalance.Person.Mob1,
                    ScheduleAmount = x.Amount,
                }).GroupBy(x => new { x.CustomerName, x.Mob }).ToList();
                list = groupSchedules.Select(x => new InstallmentNotPaidDto
                {
                    CustomerName = x.FirstOrDefault().CustomerName,
                    Mob = x.FirstOrDefault().Mob,
                    SchedulesCount = x.Count(),
                    ScheduleAmount = x.FirstOrDefault().ScheduleAmount,
                    ScheduleTotalAmount = x.Sum(y => y.ScheduleAmount),
                    Actions = null,
                    Num = null
                }).ToList();
            }

            return list;
        }
    }
}