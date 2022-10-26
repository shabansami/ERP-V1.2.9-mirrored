using ERP.Web.Identity;
using ERP.DAL;
using ERP.Web.Services;
using ERP.Web.Utilites;
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

    public class EmployeeGivingCustodyController : Controller
    {
        // GET: EmployeeGivingCustody
        VTSaleEntities db = new VTSaleEntities();
        VTSAuth auth = new VTSAuth();

        public ActionResult Index()
        {
            ViewBag.DepartmentId = new SelectList(db.Departments.Where(x => !x.IsDeleted), "Id", "Name");
            ViewBag.EmployeeId = new SelectList(new List<Employee>(), "Id", "Name");
            return View();
        }
        public ActionResult GetAll()
        {
            int? n = null;
            return Json(new
            {
                data = db.EmployeeGivingCustodies.Where(x => !x.IsDeleted).OrderBy(x => x.CreatedOn).OrderBy(x => x.CreatedOn).Select(x => new { Id = x.Id, IsApproval = x.IsApproval,  EmployeeName = x.Employee.Person.Name, Amount = x.Amount, CustodyDate = x.CustodyDate.ToString(),Notes=x.Notes, Actions = n, Num = n }).ToList()
            }, JsonRequestBehavior.AllowGet); ;

        }
        [HttpGet]
        public ActionResult CreateEdit()
        {
            // add
            IQueryable<Branch> branches = db.Branches.Where(x => !x.IsDeleted);
            ViewBag.BranchId = new SelectList(branches, "Id", "Name", 1);
            ViewBag.SafeId = new SelectList(db.Safes.Where(x => !x.IsDeleted && x.BranchId == branches.Select(y => y.Id).FirstOrDefault()), "Id", "Name", 1);
            ViewBag.BankAccountId = new SelectList(db.BankAccounts.Where(x => !x.IsDeleted).Select(x => new { Id = x.Id, AccountName = x.AccountName + " / " + x.Bank.Name }), "Id", "AccountName");

            ViewBag.DepartmentId = new SelectList(db.Departments.Where(x => !x.IsDeleted), "Id", "Name");
            ViewBag.EmployeeId = new SelectList(new List<Employee>(), "Id", "Name");
            ViewBag.LastRow = db.EmployeeGivingCustodies.Where(x => !x.IsDeleted).OrderByDescending(x => x.CreatedOn).FirstOrDefault();
            return View(new EmployeeGivingCustody() { CustodyDate = Utility.GetDateTime() });
            //}
        }


        [HttpPost]
        public JsonResult CreateEdit(EmployeeGivingCustody vm)
        {
            if (ModelState.IsValid)
            {
                if (vm.CustodyDate == null || vm.EmployeeId == null || vm.Amount == 0 )
                    return Json(new { isValid = false, message = "تأكد من ادخال بيانات صحيحة" });
                if (vm.BankAccountId == null && vm.SafeId == null)
                    return Json(new { isValid = false, message = "تأكد من اختيار طريقة دفع العهدة (بنكى-خزنة) بشكل صحيح" });

                if (TempData["userInfo"] != null)
                    auth = TempData["userInfo"] as VTSAuth;
                else
                    RedirectToAction("Login", "Default", Request.Url.AbsoluteUri.ToString());

                db.EmployeeGivingCustodies.Add(new EmployeeGivingCustody
                {
                    EmployeeId = vm.EmployeeId,
                    Amount = vm.Amount,
                    Notes = vm.Notes,
                    BranchId = vm.BranchId,
                    BankAccountId = vm.BankAccountId,
                    SafeId = vm.SafeId,
                    CustodyDate = vm.CustodyDate

                });
                //}
                if (db.SaveChanges(auth.CookieValues.UserId) > 0)
                {
                        return Json(new { isValid = true, message = "تم الاضافة بنجاح" });
                }
                else
                    return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });
            }
            else
                return Json(new { isValid = false, message = "تأكد من ادخال البيانات بشكل صحيح" });

        }

        public ActionResult Edit(string id)
        {
            Guid Id;

            if (!Guid.TryParse(id, out Id) || string.IsNullOrEmpty(id) || id == "undefined")
                return RedirectToAction("Index");

            TempData["model"] = Id;
            return RedirectToAction("CreateEdit");

        }
        [HttpPost]
        public ActionResult Delete(string id)
        {
            Guid Id;
            if (Guid.TryParse(id, out Id))
            {
                var model = db.EmployeeGivingCustodies.FirstOrDefault(x=>x.Id==Id);
                if (model != null)
                {
                    if (TempData["userInfo"] != null)
                        auth = TempData["userInfo"] as VTSAuth;
                    else
                        RedirectToAction("Login", "Default", Request.Url.AbsoluteUri.ToString());

                    model.IsDeleted = true;
                    db.Entry(model).State = EntityState.Modified;
                    if (db.SaveChanges(auth.CookieValues.UserId) > 0)
                        return Json(new { isValid = true, message = "تم الحذف بنجاح" });
                    else
                        return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });
                }
                else
                    return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });
            }
            else
                return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });

        }

        #region الاعتماد النهائى للعهدة  
        [HttpPost]
        public ActionResult ApprovalCustody(string id)
        {

            Guid Id;
            if (Guid.TryParse(id, out Id))
            {
                using (var context = new VTSaleEntities())
                {
                    using (DbContextTransaction transaction = context.Database.BeginTransaction())
                    {
                        try
                        {
                            var model = context.EmployeeGivingCustodies.Where(x => x.Id == Id).FirstOrDefault();
                            if (model != null)
                            {
                                if (TempData["userInfo"] != null)
                                    auth = TempData["userInfo"] as VTSAuth;
                                else
                                    RedirectToAction("Login", "Default", Request.Url.AbsoluteUri.ToString());

                                //اعتماد  العهدة 

                                //===================
                                List<GeneralSetting> generalSetting;
                                Employee employee;
                                //التأكد من عدم وجود حساب فرعى من الحسابات المستخدمة
                                if (GeneralDailyService.CheckGenralSettingHasValue((int)GeneralSettingTypeCl.AccountTree))
                                {
                                    // الحصول على حسابات من الاعدادات
                                    generalSetting = context.GeneralSettings.Where(x => x.SType == (int)GeneralSettingTypeCl.AccountTree).ToList();

                                    employee = context.Employees.Where(x => x.Id == model.EmployeeId).FirstOrDefault();
                                    ////التأكد من عدم وجود حساب فرعى من الحساب
                                    //if (AccountTreeService.CheckAccountTreeIdHasChilds(int.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeCustodyAccount).FirstOrDefault().SValue)))
                                    //    return Json(new { isValid = false, message = "حساب عهٌد الموظفين ليس بحساب فرعى" });

                                    //accountLoanTreeId = int.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeLoans).FirstOrDefault().SValue);
                                    //التأكد من انشاء حساب عهدة للموظف 
                                    AccountsTree accountTreeCustodyEmp;
                                    Guid? accountTreeCustodyEmpId;
                                    if (employee.Person.AccountTreeEmpCustodyId == null)
                                    {
                                        //تحديث بيانات الموظف فى جدول بيرسون
                                        var person = context.Persons.FirstOrDefault(x=>x.Id==employee.PersonId);
                                        //add as loan employee in account tree
                                        accountTreeCustodyEmp = InsertGeneralSettings<Person>.ReturnAccountTree(GeneralSettingCl.AccountTreeCustodyAccount, "عهدة الموظف : " + employee.Person.Name, AccountTreeSelectorTypesCl.Custody);
                                        context.AccountsTrees.Add(accountTreeCustodyEmp);
                                        context.SaveChanges(auth.CookieValues.UserId);
                                        accountTreeCustodyEmpId = accountTreeCustodyEmp.Id;

                                        person.AccountTreeEmpCustody = accountTreeCustodyEmp;
                                        context.Entry(person).State = EntityState.Modified;
                                        context.SaveChanges(auth.CookieValues.UserId);

                                    }
                                    else
                                    {
                                        //التأكد من عدم وجود حساب فرعى من حساب عهدة الموظف
                                        if (AccountTreeService.CheckAccountTreeIdHasChilds(employee.Person.AccountTreeEmpCustodyId))
                                            return Json(new { isValid = false, message = "حساب عهدة الموظف ليس بحساب فرعى" });
                                        accountTreeCustodyEmpId = employee.Person.AccountTreeEmpCustodyId;
                                    }

                                    //التأكد من عدم تكرار اعتماد القيد
                                    if (GeneralDailyService.GeneralDailaiyExists(model.Id, (int)TransactionsTypesCl.Custody))
                                        return Json(new { isValid = false, message = "تم الاعتماد مسبقا " });

                                    // من ح/  عهد الموظف
                                    var note = string.Empty;
                                    if (!string.IsNullOrEmpty(model.Notes))
                                        note = "_"+ model.Notes;

                                    context.GeneralDailies.Add(new GeneralDaily
                                    {
                                        AccountsTreeId = accountTreeCustodyEmpId,
                                        BranchId = model.BranchId,
                                        Debit = model.Amount,
                                        Notes = model.Notes,
                                        TransactionDate = model.CustodyDate,
                                        TransactionId = model.Id,
                                        TransactionTypeId = (int)TransactionsTypesCl.Custody
                                    });
                                    context.SaveChanges(auth.CookieValues.UserId);

                                    //الى ح/ النقدية
                                    context.GeneralDailies.Add(new GeneralDaily
                                    {
                                        AccountsTreeId = model.SafeId != null ? model.Safe.AccountsTreeId : model.BankAccount.AccountsTreeId,
                                        BranchId = model.BranchId,
                                        Credit = model.Amount,
                                        Notes = $"عهدة للموظف : {employee.Person.Name}{note}",
                                        TransactionDate = model.CustodyDate,
                                        TransactionId = model.Id,
                                        TransactionTypeId = (int)TransactionsTypesCl.Custody
                                    });
                                    context.SaveChanges(auth.CookieValues.UserId);

                                    //اعتماد العهدة
                                    model.IsApproval = true;
                                    context.Entry(model).State = EntityState.Modified;
                                    context.SaveChanges(auth.CookieValues.UserId);

                                    transaction.Commit();
                                    return Json(new { isValid = true, message = "تم الاعتماد بنجاح" });
                                }
                                else
                                    return Json(new { isValid = false, message = "يجب تعريف الأكواد الحسابية فى شاشة الاعدادات" });



                                //===============
                            }
                            else
                                return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });

                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });
                        }
                    }


                }
            }
            else
                return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });
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