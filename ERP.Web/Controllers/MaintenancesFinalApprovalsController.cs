using ERP.Web.Identity;
using ERP.DAL;
using ERP.Web.Services;
using ERP.Web.Utilites;
using ERP.Web.ViewModels;
using System;using ERP.DAL.Utilites;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static ERP.Web.Utilites.Lookups;

namespace ERP.Web.Controllers
{
    [Authorization]

    public class MaintenancesFinalApprovalsController : Controller
    {
        // GET: MaintenancesFinalApprovals
        VTSaleEntities db = new VTSaleEntities();
        VTSAuth auth => TempData["userInfo"] as VTSAuth;

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetAll()
        {
            int? n = null;
            return Json(new
            {
                data = db.Maintenances.Where(x => !x.IsDeleted && x.IsApprovalAccountant && (x.IsApprovalStore || x.StoreReceiptId == null)).OrderBy(x => x.CreatedOn).Select(x => new { Id = x.Id, InvoiceGuid = x.Id, InvoiceNum = x.InvoiceNumber, InvoiceDate = x.InvoiceDate.ToString(), CustomerName = x.Person.Name, Safy = x.Safy, InvoType = x.BySaleMen ? "مندوب" : "بدون مناديب", IsFinalApproval = x.IsFinalApproval, InvoFinal = x.IsFinalApproval ? "1" : "2", CaseName = x.MaintenanceCas != null ? x.MaintenanceCas.Name : "", Actions = n, Num = n }).ToList()
            }, JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        public ActionResult ApprovalInvoice(string invoGuid)
        {
            Guid Id;
            if (Guid.TryParse(invoGuid, out Id))
            {
                var model = db.Maintenances.Where(x => x.Id == Id).FirstOrDefault();
                if (model != null)
                {
                    //    //تسجيل القيود
                    // General Dailies
                    if (GeneralDailyService.CheckGenralSettingHasValue((int)GeneralSettingTypeCl.AccountTree))
                    {
                        //صافى قيمة الفاتورة= ( إجمالي قيمة المبيعات
                        //+ إجمالي قيمة الايرادات + قيمة الضريبة المضافة  - إجمالي خصومات الفاتورة - قيمة ض.أ.ت.ص )

                        double debit = 0;
                        double credit = 0;
                        Guid? safeBankAccountId;
                        if (model.SafeId != null)
                            safeBankAccountId = db.Safes.FirstOrDefault(x=>x.Id==model.SafeId).AccountsTreeId;
                        else if (model.BankAccountId != null)
                            safeBankAccountId = db.BankAccounts.FirstOrDefault(x=>x.Id==model.BankAccountId).AccountsTreeId;
                        else
                            return Json(new { isValid = false, message = "تأكد من تحديد طريقة الدفع من شاشة الاعتماد المحاسبى" });

                        var customer = db.Persons.Where(x => x.Id == model.CustomerId).FirstOrDefault();

                        // الحصول على حسابات من الاعدادات
                        var generalSetting = db.GeneralSettings.Where(x => x.SType == (int)GeneralSettingTypeCl.AccountTree).ToList();
                        //التأكد من عدم وجود حساب تشغيلى من الحساب
                        if (AccountTreeService.CheckAccountTreeIdHasChilds(Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeSalesAccount).FirstOrDefault().SValue)))
                            return Json(new { isValid = false, message = "حساب المبيعات ليس بحساب تشغيلى" });

                        if (AccountTreeService.CheckAccountTreeIdHasChilds(customer.AccountsTreeCustomerId))
                            return Json(new { isValid = false, message = "حساب العميل ليس بحساب تشغيلى" });

                        //if (AccountTreeService.CheckAccountTreeIdHasChilds(Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeMaintenance).FirstOrDefault().SValue)))
                        //    return Json(new { isValid = false, message = "حساب ايرادات الصيانة ليس بحساب تشغيلى" });

                        if (AccountTreeService.CheckAccountTreeIdHasChilds(Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeEarnedDiscount).FirstOrDefault().SValue)))
                            return Json(new { isValid = false, message = "حساب الخصومات ليس بحساب تشغيلى" });


                        // من ح/  العميل
                        db.GeneralDailies.Add(new GeneralDaily
                        {
                            AccountsTreeId = customer.AccountsTreeCustomerId,
                            BranchId = model.BranchId,
                            Debit = model.Safy,
                            Notes = $"فاتورة صيانة (قطع غيار)رقم : {model.InvoiceNumber}",
                            TransactionDate = model.InvoiceDate,
                            TransactionId = model.Id,
                            TransactionTypeId = (int)TransactionsTypesCl.Maintenance
                        });
                        //الى ح/ المبيعات
                        db.GeneralDailies.Add(new GeneralDaily
                        {
                            AccountsTreeId = Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeSalesAccount).FirstOrDefault().SValue),
                            BranchId = model.BranchId,
                            Credit = model.TotalSpareParts,
                            Notes = $"فاتورة صيانة (قطع غيار) رقم : {model.InvoiceNumber}",
                            TransactionDate = model.InvoiceDate,
                            TransactionId = model.Id,
                            TransactionTypeId = (int)TransactionsTypesCl.Maintenance
                        });
                        debit = debit + model.Safy;

                        credit = credit + model.TotalSpareParts;


                        // الخصومات
                        if (model.TotalDiscount > 0)
                        {
                            db.GeneralDailies.Add(new GeneralDaily
                            {
                                AccountsTreeId = Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeEarnedDiscount).FirstOrDefault().SValue),
                                BranchId = model.BranchId,
                                Debit = model.TotalDiscount,
                                Notes = $"فاتورة صيانة رقم : {model.InvoiceNumber}",
                                TransactionDate = model.InvoiceDate,
                                TransactionId = model.Id,
                                TransactionTypeId = (int)TransactionsTypesCl.Maintenance
                            });
                            debit = debit + model.TotalDiscount;
                        }


                        //  المبلغ المدفوع الى حساب العميل (دائن
                        if (model.PayedValue > 0 && model.PaymentTypeId != (int)PaymentTypeCl.Deferred)
                        {
                            //  المبلغ المدفوع من حساب الخزينة (مدين
                            db.GeneralDailies.Add(new GeneralDaily
                            {
                                AccountsTreeId = safeBankAccountId,
                                BranchId = model.BranchId,
                                Debit = model.PayedValue,
                                Notes = $"فاتورة صيانة رقم : {model.InvoiceNumber}",
                                TransactionDate = model.InvoiceDate,
                                TransactionId = model.Id,
                                TransactionTypeId = (int)TransactionsTypesCl.Maintenance
                            });
                            debit = debit + model.PayedValue;


                            db.GeneralDailies.Add(new GeneralDaily
                            {
                                AccountsTreeId = customer.AccountsTreeCustomerId,
                                BranchId = model.BranchId,
                                Credit = model.PayedValue,
                                Notes = $"فاتورة صيانة رقم : {model.InvoiceNumber}",
                                TransactionDate = model.InvoiceDate,
                                TransactionId = model.Id,
                                TransactionTypeId = (int)TransactionsTypesCl.Maintenance
                            });
                            credit = credit + model.PayedValue;



                        }

                        // الايرادات (دائن
                        var maintenanceDetails = model.MaintenanceDetails.Where(x => !x.IsDeleted).ToList();
                        foreach (var items in maintenanceDetails)
                        {
                            //foreach (var item in items.MaintenanceIncomes.Where(x => !x.IsDeleted))
                            //{
                            //    db.GeneralDailies.Add(new GeneralDaily
                            //    {
                            //        AccountsTreeId = Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeMaintenance).FirstOrDefault().SValue),
                            //        BranchId = model.BranchId,
                            //        Credit = item.Amount,
                            //        Notes = $"فاتورة صيانة رقم : {model.InvoiceNumber}",
                            //        TransactionDate = model.InvoiceDate,
                            //        TransactionId = model.Id,
                            //        TransactionTypeId = (int)TransactionsTypesCl.Maintenance
                            //    });
                            //    credit = credit + item.Amount;
                            //}

                            if (items.ItemSerialId != null && items.ItemSerialId != null && items.ItemSerialId != Guid.Empty)
                            {
                                //update item serial case history
                                var serial = db.ItemSerials.FirstOrDefault(x=>x.Id==items.ItemSerialId);
                                if (serial != null)
                                {
                                    //تحديث حالة حقل فاتورة البيع للاصناف فى جدول السيريال بنل حتى يمكن بيعه مرة اخرى 
                                    int serialCase;
                                    if (model.StoreReceiptId != null)
                                    {
                                        serial.SellInvoiceId = null;
                                        serialCase = (int)SerialCaseCl.RepairedReceiptStore;
                                    }
                                    else
                                        serialCase = (int)SerialCaseCl.RepairedReceiptCustomer;

                                    serial.SerialCaseId = serialCase;
                                    db.Entry(serial).State = EntityState.Modified;
                                    //add item serial case history
                                    db.CasesItemSerialHistories.Add(new CasesItemSerialHistory
                                    {
                                        ItemSerialId = serial.Id,
                                        ReferrenceId = model.Id,
                                        SerialCaseId = serialCase
                                    });
                                }
                            }

                            //انشاء السيرالات نمبر الخاص بكل صنف 
                            var spareParts = items.MaintenanceSpareParts.Where(x => !x.IsDeleted).ToList();
                            foreach (var item in spareParts)
                            {
                                //تسجيل سيريال جديد لكل قطعة من الكمية المسجلة
                                for (int i = 1; i < item.Quantity + 1; i++)
                                {
                                    //التأكد من ان الصنف لم يكن له اى سيريال مسبقا 
                                    var itemSerial = db.ItemSerials.Where(x => !x.IsDeleted && x.Id == item.ItemSerialId).FirstOrDefault();
                                    if (itemSerial != null)
                                    {
                                        //السيريال موجود مسبقا
                                        itemSerial.MaintenanceId = model.Id;
                                        itemSerial.CurrentStoreId = null;
                                        itemSerial.SerialCaseId = (int)SerialCaseCl.SellSparePart;
                                        db.Entry(itemSerial).State = EntityState.Modified;
                                    }
                                    else
                                    {
                                    generate:
                                        var serial = GeneratBarcodes.GenerateRandomBarcode();
                                        var isExists = db.ItemSerials.Where(x => x.SerialNumber == serial).Any();
                                        if (isExists)
                                            goto generate;
                                        itemSerial = new ItemSerial
                                        {
                                            ItemId = item.ItemId,
                                            ProductionOrderId = item.ProductionOrderId,
                                            IsItemIntial = item.IsItemIntial ? true : false,
                                            SerialCaseId = (int)SerialCaseCl.SellSparePart,
                                            SerialNumber = serial,
                                            MaintenanceId = model.Id
                                        };


                                    }

                                    //add item serial case history
                                    db.CasesItemSerialHistories.Add(new CasesItemSerialHistory
                                    {
                                        ItemSerial = itemSerial,
                                        ReferrenceId = model.Id,
                                        SerialCaseId = (int)SerialCaseCl.SellSparePart
                                    });
                                    //تحديث سيريال الصنف الجديد 
                                    //item.ItemSerial = itemSerial;
                                    //db.Entry(item).State = EntityState.Modified;
                                }


                            }
                        }
                        //التاكد من توازن المعاملة 
                        if (debit == credit)
                        {

                            //حفظ القيود 
                            // اعتماد النهائى
                            model.IsFinalApproval = true;
                            model.MaintenanceCaseId = (int)MaintenanceCaseCl.ApprovalFinal;
                            //اضافة حالة الطلب 
                            db.MaintenanceCaseHistories.Add(new MaintenanceCaseHistory
                            {
                                Maintenance = model,
                                MaintenanceCaseId = (int)MaintenanceCaseCl.ApprovalFinal
                            });
                            db.Entry(model).State = EntityState.Modified;


                            //===========================
                            if (db.SaveChanges(auth.CookieValues.UserId) > 0)
                            {
                                return Json(new { isValid = true, message = "تم الاعتماد النهائى بنجاح" });
                            }
                            else
                                return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });
                        }
                        else
                        {
                            return Json(new { isValid = false, message = "قيد المعاملة غير موزوون" });
                        }

                    }
                    else
                        return Json(new { isValid = false, message = "يجب تعريف الأكواد الحسابية فى شاشة الاعدادات" });
                }
                else
                    return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });


            }
            else
                return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });
        }

        #region طباعة باركود الاصناف 
        public ActionResult PrintBarCode(Guid? invoGuid)
        {
            PrintBarcodeVM vm = new PrintBarcodeVM();
            if (invoGuid != null)
            {
                vm = db.Maintenances
                    .Where(x => x.Id == invoGuid)
                    .Select(x => new PrintBarcodeVM
                    {
                        Id = x.Id,
                        InvoiceNumber=x.InvoiceNumber,
                        InvoDate = x.InvoiceDate,
                        ItemSerial = x.ItemSerials.Where(y => !y.IsDeleted && x.Id == y.MaintenanceId)
                            .Select(i => new ItemSerialM
                            {
                                ItemName = i.Item.Name,
                                SerialNumber = i.SerialNumber
                            }).ToList()
                    }).FirstOrDefault();
                return View(vm);
            }
            else
                return RedirectToAction("Index");
        }
        #endregion

        //Releases unmanaged resources and optionally releases managed resources.
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

    }
}