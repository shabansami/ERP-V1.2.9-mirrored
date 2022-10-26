using ERP.Desktop.DTOs;
using ERP.DAL;
using ERP.Desktop.Utilities;
using ERP.Desktop.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Desktop.Services.Transactions
{
    public class CustomerPaymentServices
    {
        readonly VTSaleEntities db;

        public CustomerPaymentServices(VTSaleEntities db)
        {
            this.db = db;
        }


        public List<CustomerPaymentVm> GetAll()
        {
            return db.CustomerPayments.Where(x => !x.IsDeleted).Select(x => new CustomerPaymentVm()
            {
                Id = x.Id,
                EmployeeName = x.Employee.Person.Name,
                CustomerName = x.PersonCustomer.Name,
                IsApproval = x.IsApproval,
                IsApprovalStatus = x.IsApproval ? "معتمده" : "غير معتمده",
                InvoType = x.BySaleMen ? "مندوب" : "بدون مناديب",
                SafeName = x.Safe.Name,
                Amount = x.Amount,
                Notes = x.Notes,
                PaymentDate = x.PaymentDate + "",
                BranchName = x.Branch.Name
            }).ToList();
        }

        public DtoResultObj<CustomerPayment> AddNew(CustomerPayment customerPayment)
        {
            var result = new DtoResultObj<CustomerPayment>();
            if (customerPayment.CustomerId == null || customerPayment.Amount == 0 || customerPayment.BranchId == null || customerPayment.PaymentDate == null)
            {
                result.Message = "تأكد من ادخال بيانات صحيحة";
                return result;
            }
            if (customerPayment.SafeId == null)
            {
                result.Message = "تأكد من اختيار طريقة السداد بشكل صحيح";
                return result;
            }
            //فى حالة ادخال رقم فاتورة مبيعات يجب التأكد من صحة الرقم المدخلو وانه من ضمن فواتير المبيعات 
            if (customerPayment.SellInvoiceId != null)
            {
                if (int.TryParse(customerPayment.SellInvoiceId.ToString(), out var id))
                {
                    var sellIsExists = db.SellInvoices.Where(x => !x.IsDeleted && x.Id == customerPayment.SellInvoiceId).Count();
                    if (sellIsExists == 0)
                    {
                        result.Message = "خطأ .... رقم الفاتورة غير موجود فى فواتير البيع";
                        return result;
                    }
                }
                else
                {
                    result.Message = "تأكد من ادخال رقم فاتورة البيع بشكل صحيح ";
                    return result;
                }

            }
            customerPayment.CreatedBy = UserServices.UserInfo.UserId;
            customerPayment.CreatedOn = CommonMethods.TimeNow;
            db.CustomerPayments.Add(customerPayment);
            if (db.SaveChanges(UserServices.UserInfo.UserId) > 0)
            {
                result.IsSuccessed = true;
                result.Message = "تم الحفظ بنجاح";
                result.Object = customerPayment;
            }
            else
                result.Message = "حدث خطأ اثناء تنفيذ العملية";

            return result;
        }

        public DtoResultObj<CustomerPayment> Edit(CustomerPayment customerPayment)
        {
            var result = new DtoResultObj<CustomerPayment>();
            if (customerPayment.CustomerId == null || customerPayment.Amount == 0 || customerPayment.BranchId == null || customerPayment.PaymentDate == null)
            {
                result.Message = "تأكد من ادخال بيانات صحيحة";
                return result;
            }
            if (customerPayment.SafeId == null)
            {
                result.Message = "تأكد من اختيار طريقة السداد بشكل صحيح";
                return result;
            }
            //فى حالة ادخال رقم فاتورة مبيعات يجب التأكد من صحة الرقم المدخلو وانه من ضمن فواتير المبيعات 
            if (customerPayment.SellInvoiceId != null)
            {
                if (int.TryParse(customerPayment.SellInvoiceId.ToString(), out var id))
                {
                    var sellIsExists = db.SellInvoices.Where(x => !x.IsDeleted && x.Id == customerPayment.SellInvoiceId).Count();
                    if (sellIsExists == 0)
                    {
                        result.Message = "خطأ .... رقم الفاتورة غير موجود فى فواتير البيع";
                        return result;
                    }
                }
                else
                {
                    result.Message = "تأكد من ادخال رقم فاتورة البيع بشكل صحيح ";
                    return result;
                }

            }

            if (customerPayment.Id != Guid.Empty)
            {
                var model = db.CustomerPayments.Find(customerPayment.Id);
                if(model == null)
                {
                    result.Message = "خطأ في رقم الدفعة";
                    return result;
                }
                model.CustomerId = customerPayment.CustomerId;
                model.Amount = customerPayment.Amount;
                model.Notes = customerPayment.Notes;
                model.BySaleMen = customerPayment.BySaleMen;
                model.SafeId = customerPayment.SafeId;
                model.BranchId = customerPayment.BranchId;
                model.IsApproval = false;

                db.Entry(model).State = EntityState.Modified;

                if (db.SaveChanges(UserServices.UserInfo.UserId) > 0)
                {
                    result.IsSuccessed = true;
                    result.Message = "تم التعديل بنجاح";
                    result.Object = model;
                }
                else
                    result.Message = "حدث خطأ اثناء تنفيذ العملية";

            }
            else
            {
                result.Message = "خطأ في رقم الدفعة";
                return result;
            }

            return result;
        }

        public DtoResult Delete(Guid id)
        {
            var model = db.CustomerPayments.Find(id);
            if (model != null)
            {
                model.IsDeleted = true;
                db.Entry(model).State = EntityState.Modified;
                if (db.SaveChanges(UserServices.UserInfo.UserId) > 0)
                    return new DtoResult() { IsSuccessed = true, Message = "تم الحذف بنجاح" };
                else
                    return new DtoResult() { IsSuccessed = false, Message = "حدث خطأ اثناء تنفيذ العملية" };
            }
            else
                return new DtoResult() { IsSuccessed = false, Message = "حدث خطأ اثناء تنفيذ العملية" };
        }
    }
}
