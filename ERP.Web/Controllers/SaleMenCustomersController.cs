using Newtonsoft.Json;
using ERP.Web.DataTablesDS;
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

    public class SaleMenCustomersController : Controller
    {
        // GET: SaleMenCustomers
        VTSaleEntities db = new VTSaleEntities();
        VTSAuth auth = new VTSAuth();
        public static string DS { get; set; }

        public ActionResult Index()
        {
            //ViewBag.DepartmentId = new SelectList(db.Departments.Where(x => !x.IsDeleted), "Id", "Name");
            //ViewBag.EmployeeId = new SelectList(new List<Employee>(), "Id", "Name");
            return View();
        }
        public ActionResult GetAll()
        {
            int? n = null;
            return Json(new
            {
                data = db.SaleMenCustomers.OrderBy(x=>x.CreatedOn).Where(x => !x.IsDeleted).Select(x => new { EmployeeId = x.EmployeeId, EmployeeName = x.Employee.Person.Name,Actions = n, Num = n }).Distinct().ToList()
            }, JsonRequestBehavior.AllowGet); ;

        }
        #region wizard step 2 اضافة عملاء
        public ActionResult GetDSCustomers()
        {
            int? n = null;
            if (DS == null)
                return Json(new
                {
                    data = new SaleMenCustomerDT()
                }, JsonRequestBehavior.AllowGet);
            else
                return Json(new
                {
                    data = JsonConvert.DeserializeObject<List<SaleMenCustomerDT>>(DS)
                }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult addCustomer(SaleMenCustomerDT vm)
        {
            List<SaleMenCustomerDT> deDS = new List<SaleMenCustomerDT>();
            string customerName = "";
            if (vm.DT_Datasource != null)
                deDS = JsonConvert.DeserializeObject<List<SaleMenCustomerDT>>(vm.DT_Datasource);
            if (vm.CustomerId !=null)
            {
                if (deDS.Where(x => x.CustomerId == vm.CustomerId).Count() > 0)
                    return Json(new { isValid = false, msg = "اسم العميل موجود مسبقا " }, JsonRequestBehavior.AllowGet);
                customerName = db.Persons.Where(x => !x.IsDeleted && x.Id == vm.CustomerId).FirstOrDefault().Name;
            }
            else
                return Json(new { isValid = false, msg = "تأكد من اختيار عميل " }, JsonRequestBehavior.AllowGet);

            var newCustomer = new SaleMenCustomerDT { CustomerId = vm.CustomerId, CustomerName = customerName };
            deDS.Add(newCustomer);
            DS = JsonConvert.SerializeObject(deDS);
            return Json(new { isValid = true, msg = "تم اضافة العميل بنجاح " }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        [HttpGet]
        public ActionResult CreateEdit()
        {
            ViewBag.PersonCategoryId = new SelectList(db.PersonCategories.Where(x => !x.IsDeleted && x.IsCustomer), "Id", "Name");
            ViewBag.CustomerId = new SelectList(new List<Person>(), "Id", "Name");


            if (TempData["model"] != null) //edit
            {
                Guid Id;
                if (Guid.TryParse(TempData["model"].ToString(), out Id))
                {
                    var vm = db.SaleMenCustomers.Where(x =>!x.IsDeleted&& x.EmployeeId == Id);

                    List<SaleMenCustomerDT> saleMenCustomerDT = new List<SaleMenCustomerDT>();
                    var items = vm.Select(x => new
                                  SaleMenCustomerDT
                    {
                       CustomerId=x.CustomerId,
                       CustomerName=x.CustomerPerson.Name,
                      }).ToList();
                    DS = JsonConvert.SerializeObject(items);

                    var departMentId = vm.FirstOrDefault().Employee.DepartmentId;
                    var empId = vm.FirstOrDefault().EmployeeId;
                    ViewBag.DepartmentId = new SelectList(db.Departments.Where(x => !x.IsDeleted), "Id", "Name", departMentId);
                    ViewBag.EmployeeId = new SelectList(EmployeeService.GetSaleMens(departMentId), "Id", "Name", empId);
                    return View(new SaleMenCustomer() {Id=Id });
                }
                else
                    return RedirectToAction("Index");
            }
            else
            {                   // add
                DS = JsonConvert.SerializeObject(new List<SaleMenCustomerDT>());

                ViewBag.DepartmentId = new SelectList(db.Departments.Where(x => !x.IsDeleted), "Id", "Name");
                ViewBag.EmployeeId = new SelectList(new List<Employee>(), "Id", "Name");
                return View(new SaleMenCustomer() );
            }
        }


        [HttpPost]
        public JsonResult CreateEdit(SaleMenCustomer vm,string DT_DatasourceCustomers)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (TempData["userInfo"] != null)
                        auth = TempData["userInfo"] as VTSAuth;
                    else
                        RedirectToAction("Login", "Default", Request.Url.AbsoluteUri.ToString());


                    if (vm.EmployeeId == null )
                        return Json(new { isValid = false, message = "تأكد من ادخال بيانات صحيحة" });

                    //العملاء
                    List<SaleMenCustomerDT> customerDT = new List<SaleMenCustomerDT>();
                    List<SaleMenCustomer> saleMenCustomers = new List<SaleMenCustomer>();

                    if (DT_DatasourceCustomers != null)
                    {
                        customerDT = JsonConvert.DeserializeObject<List<SaleMenCustomerDT>>(DT_DatasourceCustomers);
                        if (customerDT.Count() == 0)
                            return Json(new { isValid = false, message = "تأكد من ادخال عميل واحد على الاقل" });
                        else
                        {
                            saleMenCustomers = customerDT.Select(x =>
                              new SaleMenCustomer
                              {
                                    EmployeeId=vm.EmployeeId,
                                    CustomerId=x.CustomerId
                              }).ToList();
                        }
                    }
                    else
                        return Json(new { isValid = false, message = "تأكد من ادخال عميل واحد على الاقل" });

                    var isInsert = false;

                    if (vm.Id != Guid.Empty)
                    {
                        //delete all privous customers 
                        var privousCustomers = db.SaleMenCustomers.Where(x => x.EmployeeId == vm.EmployeeId).ToList();
                        foreach (var item in privousCustomers)
                        {
                            item.IsDeleted = true;
                            db.Entry(item).State = EntityState.Modified;
                        }
                    }
                    else
                        isInsert = true;

                    db.SaleMenCustomers.AddRange(saleMenCustomers);

                    if (db.SaveChanges(auth.CookieValues.UserId) > 0)
                    {
                        if (isInsert)
                            return Json(new { isValid = true,  isInsert, message = "تم الاضافة بنجاح" });
                        else
                            return Json(new { isValid = true, isInsert, message = "تم التعديل بنجاح" });

                    }
                    else
                        return Json(new { isValid = false, isInsert, message = "حدث خطأ اثناء تنفيذ العملية" });
                }
                else
                    return Json(new { isValid = false, message = "تأكد من ادخال البيانات بشكل صحيح" });


            }
            catch (Exception ex)
            {
                return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });
            }
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
                var list = db.SaleMenCustomers.Where(x=>!x.IsDeleted&&x.EmployeeId==Id).ToList();
                if (list!=null)
                {
                    if (TempData["userInfo"] != null)
                        auth = TempData["userInfo"] as VTSAuth;
                    else
                        RedirectToAction("Login", "Default", Request.Url.AbsoluteUri.ToString());
                    foreach (var item in list)
                    {
                        item.IsDeleted = true;
                        db.Entry(item).State = EntityState.Modified;
                    }

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