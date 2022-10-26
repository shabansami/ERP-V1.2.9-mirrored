using ERP.Web.Identity;
using ERP.DAL;
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

    public class ContractSchedulingAbsencesController : Controller
    {
        // GET: ContractSchedulingAbsences
        VTSaleEntities db = new VTSaleEntities();
        VTSAuth auth = new VTSAuth();

        public ActionResult Index()
        {
            ViewBag.DepartmentId = new SelectList(db.Departments.Where(x => !x.IsDeleted), "Id", "Name");
            ViewBag.EmployeeId = new SelectList(new List<Employee>(), "Id", "Name");
            ViewBag.ContractSchedulingId = new SelectList(new List<ContractScheduling>(), "Id", "Name");
            return View();
        }
        public ActionResult GetAll()
        {
            int? n = null;
            return Json(new
            {
                data = db.ContractSchedulingAbsences.Where(x => !x.IsDeleted).OrderBy(x => x.CreatedOn).Select(x => new { Id = x.Id,Name=x.ContractScheduling.Name, IsPayed = x.IsPayed, IsPenalty = x.IsPenalty ? "بخصم" : "بدون خصم", EmployeeName = x.ContractScheduling.Contract.Employee.Person.Name, ContractSchedulingName = x.ContractScheduling.MonthYear.ToString().Substring(0, 7), FromDate = x.FromDate.ToString(), ToDate = x.ToDate.ToString(), AbsenceDayNumber = x.IsPenalty?x.PenaltyNumber: x.AbsenceDayNumber, Actions = n, Num = n }).ToList()
        }, JsonRequestBehavior.AllowGet); ;

        }
        [HttpGet]
        public ActionResult CreateEdit()
        {
        // add
            ViewBag.VacationTypeId = new SelectList(db.VacationTypes.Where(x => !x.IsDeleted), "Id", "Name");

            ViewBag.DepartmentId = new SelectList(db.Departments.Where(x => !x.IsDeleted), "Id", "Name");
            ViewBag.EmployeeId = new SelectList(new List<Employee>(), "Id", "Name");
            ViewBag.ContractSchedulingId = new SelectList(new List<ContractScheduling>(), "Id", "Name");
            ViewBag.LastRow = db.ContractSchedulingAbsences.Where(x => !x.IsDeleted).OrderByDescending(x => x.CreatedOn).FirstOrDefault();
            return View(new ContractSchedulingAbsence() { IsPenalty =true, FromDate = Utility.GetDateTime(),ToDate = Utility.GetDateTime() });
            //}
        }

        [HttpPost]
        public JsonResult CreateEdit(ContractSchedulingAbsence vm,string balanceDays,string txtPenaltyNumber)
        {
            if (ModelState.IsValid)
            {
                if (vm.ContractSchedulingId == null || vm.FromDate ==null|| vm.ToDate ==null || vm.AbsenceDayNumber == 0 )
                    return Json(new { isValid = false, message = "تأكد من ادخال بيانات صحيحة" });
                if (txtPenaltyNumber == "0" && vm.VacationTypeId == null)
                    return Json(new { isValid = false, message = "تأكد من اختيار حالة الغياب بشكل صحيح" });

                if (TempData["userInfo"] != null)
                    auth = TempData["userInfo"] as VTSAuth;
                else
                    RedirectToAction("Login", "Default", Request.Url.AbsoluteUri.ToString());

                //التأكد من وقوع ايام الغياب فى نطاق الشهر/الاسبوع
                var contractScheduling = db.ContractSchedulings.Where(x => x.Id == vm.ContractSchedulingId).FirstOrDefault();
                // فى حالة الشهري/الاسبوعى
                var contractSalaryTpe = contractScheduling.Contract.ContractSalaryTypeId == (int)ContractSalaryTypeCl.Monthly ? "الشهر" : "الاسبوع";
                    if (!(vm.FromDate >= contractScheduling.MonthYear && vm.ToDate <= contractScheduling.ToDate))
                        return Json(new { isValid = false, message = $" التاريخ من/الى لا يقع فى نطاق {contractSalaryTpe} " });

                var newSchedulingAbsence = new ContractSchedulingAbsence
                {
                    ContractSchedulingId = vm.ContractSchedulingId,
                    FromDate = vm.FromDate,
                    ToDate = vm.ToDate,
                    AbsenceDayNumber = vm.AbsenceDayNumber,
                };
                if (vm.IsPenalty)
                {
                        if(txtPenaltyNumber == "0"||!double.TryParse(txtPenaltyNumber,out var yy))
                            return Json(new { isValid = false, message = "تأكد من ادخال عدد ايام الغياب" });
                        newSchedulingAbsence.PenaltyNumber = double.Parse(txtPenaltyNumber);
                        newSchedulingAbsence.IsPenalty = true;
                }
                else
                {
                    if (vm.VacationTypeId != null)
                    {
                        if(int.TryParse(balanceDays,out var tt))
                        {
                            if (int.Parse(balanceDays) < vm.AbsenceDayNumber)
                                return Json(new { isValid = false, message = "عدد ايام الغياب اكبر من الرصيد المتاح " });
                            newSchedulingAbsence.VacationTypeId = vm.VacationTypeId;
                        }
                        else
                            return Json(new { isValid = false, message = "عدد ايام الغياب اكبر من الرصيد المتاح " });



                    }
                    else
                        return Json(new { isValid = false, message = "تأكد من اختيار نوع الاجازة" });
                }

                db.ContractSchedulingAbsences.Add(newSchedulingAbsence);
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
                var model = db.ContractSchedulingAbsences.FirstOrDefault(x=>x.Id==Id);
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

        #region الاعتماد النهائى للغياب  
        //[HttpPost]
        //public ActionResult ApprovalContractSchedulingAbsence(string id)
        //{
        //    int Id;
        //    if (int.TryParse(id, out Id))
        //    {
        //        var model = db.ContractSchedulingAbsences.FirstOrDefault(x=>x.Id==Id);
        //        if (model != null)
        //        {
        //            if (TempData["userInfo"] != null)
        //                auth = TempData["userInfo"] as VTSAuth;
        //            else
        //                RedirectToAction("Login", "Default", Request.Url.AbsoluteUri.ToString());

        //            model.IsApproval = true;
        //            db.Entry(model).State = EntityState.Modified;
        //            if (db.SaveChanges(auth.CookieValues.UserId) > 0)
        //                return Json(new { isValid = true, message = "تم الاعتماد بنجاح" });
        //            else
        //                return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });
        //        }
        //        else
        //            return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });
        //    }
        //    else
        //        return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });
        //}




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