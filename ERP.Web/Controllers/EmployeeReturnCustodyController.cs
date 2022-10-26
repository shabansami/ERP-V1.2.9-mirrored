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
using ERP.Web.ViewModels;

namespace ERP.Web.Controllers
{
    [Authorization]
    public class EmployeeReturnCustodyController : Controller
    {
        // GET: EmployeeReturnCustody
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
                data = db.EmployeeReturnCustodies.Where(x => !x.IsDeleted).OrderBy(x => x.CreatedOn).Select(x => new { Id = x.Id, IsApproval = x.IsApproval, AccountTreeId=x.ExpenseTypeAccountTreeId, ExpenseTypeName =x.ExpenseTypeAccountTree.AccountName, EmployeeName = x.Employee.Person.Name, Amount = x.Amount, ReturnDate = x.ReturnDate.ToString(), Notes = x.Notes, Actions = n, Num = n }).ToList()
            }, JsonRequestBehavior.AllowGet); ;

        }
        [HttpGet]
        public ActionResult CreateEdit()
        {
            // add
            //ViewBag.ExpenseTypeId = new SelectList(db.ExpenseTypes.Where(x => !x.IsDeleted), "Id", "Name");
            ViewBag.DepartmentId = new SelectList(db.Departments.Where(x => !x.IsDeleted), "Id", "Name");
            ViewBag.EmployeeId = new SelectList(new List<Employee>(), "Id", "Name");
            ViewBag.LastRow = db.EmployeeReturnCustodies.Where(x => !x.IsDeleted).OrderByDescending(x => x.CreatedOn).FirstOrDefault();
            return View(new EmployeeReturnCustody() { ReturnDate = Utility.GetDateTime() });
            //}
        }


        [HttpPost]
        public JsonResult CreateEdit(EmployeeReturnCustody vm)
        {
            if (ModelState.IsValid)
            {
                if (vm.ReturnDate == null|| vm.ExpenseTypeAccountTreeId == null || vm.EmployeeId == null || vm.EmployeeId == Guid.Empty || vm.Amount == 0)
                    return Json(new { isValid = false, message = "تأكد من ادخال بيانات صحيحة" });

                if (TempData["userInfo"] != null)
                    auth = TempData["userInfo"] as VTSAuth;
                else
                    RedirectToAction("Login", "Default", Request.Url.AbsoluteUri.ToString());
                //التأكد من ان الموظف تم تسجيل له عهدة اولا
                var empCustody = db.Employees.Where(x => x.Id == vm.EmployeeId).FirstOrDefault().Person;
                if (empCustody.AccountTreeEmpCustodyId == null)
                    return Json(new { isValid = false, message = "تأكد من تسجيل عهدة للموظف اولا" });


                db.EmployeeReturnCustodies.Add(new EmployeeReturnCustody
                {
                    ExpenseTypeAccountTreeId = vm.ExpenseTypeAccountTreeId,
                    EmployeeId = vm.EmployeeId,
                    Amount = vm.Amount,
                    Notes = vm.Notes,
                    ReturnDate = vm.ReturnDate

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
                var model = db.EmployeeReturnCustodies.FirstOrDefault(x=>x.Id==Id);
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

        #region الاعتماد النهائى لارجاع مصروف   
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
                            var model = context.EmployeeReturnCustodies.Where(x => x.Id == Id).FirstOrDefault();
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

                                    if (AccountTreeService.CheckAccountTreeIdHasChilds(model.ExpenseTypeAccountTreeId))
                                        return Json(new { isValid = false, message = "حساب المصروفات ليس بحساب فرعى" });

                                    //التأكد من انشاء حساب عهدة للموظف 
                                    Guid? accountTreeCustodyEmpId;
                                    if (employee.Person.AccountTreeEmpCustodyId == null)
                                        return Json(new { isValid = false, message = "تأكد من تسجيل عهدة للموظف اولا" });

                                    else
                                    {
                                        //التأكد من عدم وجود حساب فرعى من حساب عهدة الموظف
                                        if (AccountTreeService.CheckAccountTreeIdHasChilds(employee.Person.AccountTreeEmpCustodyId))
                                            return Json(new { isValid = false, message = "حساب عهدة الموظف ليس بحساب فرعى" });
                                        accountTreeCustodyEmpId = employee.Person.AccountTreeEmpCustodyId;
                                    }
                                    //التأكد من عدم تكرار اعتماد القيد
                                    if (GeneralDailyService.GeneralDailaiyExists(model.Id, (int)TransactionsTypesCl.ReturnCustody))
                                        return Json(new { isValid = false, message = "تم الاعتماد مسبقا " });


                                    //من حساب المصروف
                                    context.GeneralDailies.Add(new GeneralDaily
                                    {
                                        AccountsTreeId = model.ExpenseTypeAccountTreeId,
                                        BranchId = employee.BranchId,
                                        Debit = model.Amount,
                                        Notes = model.Notes,
                                        TransactionDate = model.ReturnDate,
                                        TransactionId = model.Id,
                                        TransactionTypeId = (int)TransactionsTypesCl.ReturnCustody
                                    });
                                    context.SaveChanges(auth.CookieValues.UserId);

                                    // الى ح/  عهد الموظف
                                    context.GeneralDailies.Add(new GeneralDaily
                                    {
                                        AccountsTreeId = accountTreeCustodyEmpId,
                                        BranchId = employee.BranchId,
                                        Credit = model.Amount,
                                        Notes = model.Notes,
                                        TransactionDate = model.ReturnDate,
                                        TransactionId = model.Id,
                                        TransactionTypeId = (int)TransactionsTypesCl.ReturnCustody
                                    });
                                    context.SaveChanges(auth.CookieValues.UserId);


                                    //اعتماد مصروف العهدة
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