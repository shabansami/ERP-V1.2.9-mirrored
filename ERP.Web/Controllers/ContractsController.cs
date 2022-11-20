using Newtonsoft.Json;
using ERP.Web.DataTablesDS;
using ERP.Web.Identity;
using ERP.DAL;
using ERP.Web.Utilites;
using System;
using ERP.DAL.Utilites;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static ERP.Web.Utilites.Lookups;

namespace ERP.Web.Controllers
{
    [Authorization]

    public class ContractsController : Controller
    {
        // GET: Contracts
        VTSaleEntities db = new VTSaleEntities();
        VTSAuth auth => TempData["userInfo"] as VTSAuth;
        public static string DS_SalaryAddition { get; set; }
        public static string DS_SalaryPenalty { get; set; }
        public static string DS_DefinitionVacation { get; set; }

        #region ادارة عقود الموظفين
        public ActionResult Index()
        {
            ViewBag.EmployeeId = new SelectList(db.Employees.Where(x => !x.IsDeleted).Select(x => new { Id = x.Id, Name = x.Person.Name }), "Id", "Name");
            return View();
        }

        public ActionResult GetAll()
        {
            int? n = null;
            return Json(new
            {
                data = db.Contracts.Where(x => !x.IsDeleted).OrderBy(x => x.CreatedOn).Select(x => new { Id = x.Id, IsActive = x.IsActive, ConNum = x.Id, FromDate = x.FromDate.ToString().Substring(0, 7), ToDate = x.ToDate.ToString().Substring(0, 7), EmployeeName = x.Employee.Person.Name, ContractTypeName = x.ContractType.Name, Salary = x.Salary, IsApproval = x.IsApproval, IsActiveStatus = x.IsActive ? "العقد نشط" : "العقد غير نشط", EmpGuid = x.Employee.Id, typ = (int)UploalCenterTypeCl.Employee, Actions = n, Num = n }).ToList()
            }, JsonRequestBehavior.AllowGet);

        }
        #endregion

        #region wizard step 2 اضافة عناصر  البدلات 
        public ActionResult GetDSSalaryAddition()
        {
            int? n = null;
            if (DS_SalaryAddition == null)
                return Json(new
                {
                    data = new ContractSalaryAdditionsDT()
                }, JsonRequestBehavior.AllowGet);
            else
                return Json(new
                {
                    data = JsonConvert.DeserializeObject<List<ContractSalaryAdditionsDT>>(DS_SalaryAddition)
                }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddSalaryAddition(ContractSalaryAdditionsDT vm)
        {
            List<ContractSalaryAdditionsDT> deDS = new List<ContractSalaryAdditionsDT>();
            string salaryAdditionTypeName = "";
            string salaryAdditionName = "";

            if (vm.DT_Datasource != null)
                deDS = JsonConvert.DeserializeObject<List<ContractSalaryAdditionsDT>>(vm.DT_Datasource);
            if (vm.SalaryAdditionId != null)
            {
                if (deDS.Where(x => x.SalaryAdditionId == vm.SalaryAdditionId).Count() > 0)
                    return Json(new { isValid = false, msg = "البدل تم اضافته مسبقا " }, JsonRequestBehavior.AllowGet);
                var salaryAddition = db.SalaryAdditions.Where(x => x.Id == vm.SalaryAdditionId).FirstOrDefault();
                salaryAdditionTypeName = salaryAddition.SalaryAdditionType.Name;
                salaryAdditionName = salaryAddition.Name;
            }
            else
                return Json(new { isValid = false, msg = "تأكد من اختيار البدل " }, JsonRequestBehavior.AllowGet);

            var newSalaryAddition = new ContractSalaryAdditionsDT { SalaryAdditionId = vm.SalaryAdditionId, SalaryAdditionName = salaryAdditionName, SalaryAdditionTypeName = salaryAdditionTypeName, SalaryAdditionAmount = vm.SalaryAdditionAmount, SalaryAdditionNotes = vm.SalaryAdditionNotes };
            deDS.Add(newSalaryAddition);
            DS_SalaryAddition = JsonConvert.SerializeObject(deDS);
            return Json(new { isValid = true, msg = "تم تسجيل البدل بنجاح ", totalAmount = deDS.Sum(x => x.SalaryAdditionAmount) }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region wizard step 3 تسجيل الخصومات للعقد
        public ActionResult GetDSSalaryPenalty()
        {
            int? n = null;
            if (DS_SalaryPenalty == null)
                return Json(new
                {
                    data = new ContractSalaryPenaltyDT()
                }, JsonRequestBehavior.AllowGet);
            else
                return Json(new
                {
                    data = JsonConvert.DeserializeObject<List<ContractSalaryPenaltyDT>>(DS_SalaryPenalty)
                }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddSalaryPenalty(ContractSalaryPenaltyDT vm)
        {
            List<ContractSalaryPenaltyDT> deDS = new List<ContractSalaryPenaltyDT>();
            string salaryPenaltyTypeName = "";
            string salaryPenaltyName = "";
            if (vm.DT_Datasource != null)
                deDS = JsonConvert.DeserializeObject<List<ContractSalaryPenaltyDT>>(vm.DT_Datasource);
            if (vm.SalaryPenaltyId != null)
            {
                if (deDS.Where(x => x.SalaryPenaltyId == vm.SalaryPenaltyId).Count() > 0)
                    return Json(new { isValid = false, msg = "الخصم تم اضافته مسبقا " }, JsonRequestBehavior.AllowGet);
                var salaryPenalty = db.SalaryPenalties.Where(x => x.Id == vm.SalaryPenaltyId).FirstOrDefault();
                salaryPenaltyTypeName = salaryPenalty.SalaryPenaltyType.Name;
                salaryPenaltyName = salaryPenalty.Name;
            }
            else
                return Json(new { isValid = false, msg = "تأكد من اختيار الخصم " }, JsonRequestBehavior.AllowGet);

            var newSalaryPenalty = new ContractSalaryPenaltyDT { SalaryPenaltyId = vm.SalaryPenaltyId, SalaryPenaltyName = salaryPenaltyName, SalaryPenaltyTypeName = salaryPenaltyTypeName, SalaryPenaltyAmount = vm.SalaryPenaltyAmount, SalaryPenaltyNotes = vm.SalaryPenaltyNotes };
            deDS.Add(newSalaryPenalty);
            DS_SalaryPenalty = JsonConvert.SerializeObject(deDS);
            return Json(new { isValid = true, msg = "تم تسجيل الخصم بنجاح ", totalPenalty = deDS.Sum(x => x.SalaryPenaltyAmount) }, JsonRequestBehavior.AllowGet);
        }

        #endregion
        #region wizard step 4 تسجيل الاجازات للعقد
        public ActionResult GetDSDefinitionVacations()
        {
            int? n = null;
            if (DS_DefinitionVacation == null)
                return Json(new
                {
                    data = new ContractDefinitionVacationDT()
                }, JsonRequestBehavior.AllowGet);
            else
                return Json(new
                {
                    data = JsonConvert.DeserializeObject<List<ContractDefinitionVacationDT>>(DS_DefinitionVacation)
                }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddDefinitionVacation(ContractDefinitionVacationDT vm)
        {
            List<ContractDefinitionVacationDT> deDS = new List<ContractDefinitionVacationDT>();
            string VacationTypeName = "";
            if (vm.DT_Datasource != null)
                deDS = JsonConvert.DeserializeObject<List<ContractDefinitionVacationDT>>(vm.DT_Datasource);
            if (vm.VacationTypeId != null)
            {
                if (deDS.Where(x => x.VacationTypeId == vm.VacationTypeId).Count() > 0)
                    return Json(new { isValid = false, msg = "الاجازة تم اضافتها مسبقا " }, JsonRequestBehavior.AllowGet);
                var definitionVacation = db.VacationTypes.Where(x => x.Id == vm.VacationTypeId).FirstOrDefault();
                VacationTypeName = definitionVacation.Name;
            }
            else
                return Json(new { isValid = false, msg = "تأكد من اختيار الاجازة " }, JsonRequestBehavior.AllowGet);

            var newDefinitionVacation = new ContractDefinitionVacationDT { VacationTypeId = vm.VacationTypeId, VacationTypeName = VacationTypeName, DayNumber = vm.DayNumber };
            deDS.Add(newDefinitionVacation);
            DS_DefinitionVacation = JsonConvert.SerializeObject(deDS);
            return Json(new { isValid = true, msg = "تم تسجيل الاجازة بنجاح ", totalDefinitionVacation = deDS.Sum(x => x.DayNumber) }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region تسجيل عقد لموظف وتعديلها وحذفها
        [HttpGet]
        public ActionResult CreateEdit()
        {

            ViewBag.SalaryAdditionTypeId = new SelectList(db.SalaryAdditionTypes.Where(x => !x.IsDeleted), "Id", "Name");
            ViewBag.SalaryAdditionId = new SelectList(new List<SalaryAddition>(), "Id", "Name");

            ViewBag.SalaryPenaltyTypeId = new SelectList(db.SalaryPenaltyTypes.Where(x => !x.IsDeleted), "Id", "Name");
            ViewBag.SalaryPenaltyId = new SelectList(new List<SalaryPenalty>(), "Id", "Name");

            ViewBag.VacationTypeId = new SelectList(db.VacationTypes.Where(x => !x.IsDeleted), "Id", "Name");

            if (TempData["model"] != null) //edit
            {
                Guid guId;
                if (Guid.TryParse(TempData["model"].ToString(), out guId))
                {
                    var vm = db.Contracts.Where(x => x.Id == guId).FirstOrDefault();

                    List<ContractSalaryAdditionsDT> salaryAdditionsDTs = new List<ContractSalaryAdditionsDT>();
                    var salaryAdditions = db.ContractSalaryAdditions.Where(x => !x.IsDeleted && x.ContractId == vm.Id).Select(item => new
                                  ContractSalaryAdditionsDT
                    {
                        SalaryAdditionAmount = item.Amount,
                        SalaryAdditionId = item.SalaryAdditionId,
                        SalaryAdditionName = item.SalaryAddition.Name,
                        SalaryAdditionTypeName = item.SalaryAddition.SalaryAdditionType.Name,
                        SalaryAdditionNotes = item.Notes
                    }).ToList();
                    DS_SalaryAddition = JsonConvert.SerializeObject(salaryAdditions);

                    var salaryPenalts = db.ContractSalaryPenalties.Where(x => !x.IsDeleted && x.ContractId == vm.Id).Select(s => new
                                 ContractSalaryPenaltyDT
                    {
                        SalaryPenaltyId = s.SalaryPenaltyId,
                        SalaryPenaltyName = s.SalaryPenalty.Name,
                        SalaryPenaltyAmount = s.Amount,
                        SalaryPenaltyNotes = s.Notes,
                        SalaryPenaltyTypeName = s.SalaryPenalty.SalaryPenaltyType.Name
                    }).ToList();
                    DS_SalaryPenalty = JsonConvert.SerializeObject(salaryPenalts);

                    var definitionVacations = db.ContractDefinitionVacations.Where(x => !x.IsDeleted && x.ContractId == vm.Id).Select(s => new
                                 ContractDefinitionVacationDT
                    {
                        VacationTypeId = s.VacationTypeId,
                        VacationTypeName = s.VacationType.Name,
                        DayNumber = s.DayNumber
                    }).ToList();
                    DS_DefinitionVacation = JsonConvert.SerializeObject(definitionVacations);

                    var departMentId = vm.Employee.DepartmentId;
                    ViewBag.DepartmentId = new SelectList(db.Departments.Where(x => !x.IsDeleted), "Id", "Name", departMentId);
                    ViewBag.EmployeeId = new SelectList(db.Employees.Where(x => !x.IsDeleted && x.DepartmentId == departMentId).Select(p => new { Id = p.Id, Name = p.Person.Name }), "Id", "Name", vm.EmployeeId);
                    ViewBag.ContractTypeId = new SelectList(db.ContractTypes.Where(x => !x.IsDeleted), "Id", "Name", vm.ContractTypeId);
                    ViewBag.ContractSalaryTypeId = new SelectList(db.ContractSalaryTypes.Where(x => !x.IsDeleted), "Id", "Name", vm.ContractSalaryTypeId);
                    return View(vm);
                }
                else
                    return RedirectToAction("Index");
            }
            else
            {
                // add

                DS_SalaryAddition = JsonConvert.SerializeObject(new List<ContractSalaryAdditionsDT>());
                DS_SalaryPenalty = JsonConvert.SerializeObject(new List<ContractSalaryPenaltyDT>());
                DS_DefinitionVacation = JsonConvert.SerializeObject(new List<ContractDefinitionVacationDT>());

                ViewBag.DepartmentId = new SelectList(db.Departments.Where(x => !x.IsDeleted), "Id", "Name");
                ViewBag.EmployeeId = new SelectList(new List<Employee>(), "Id", "Name");
                ViewBag.ContractTypeId = new SelectList(db.ContractTypes.Where(x => !x.IsDeleted), "Id", "Name");
                ViewBag.ContractSalaryTypeId = new SelectList(db.ContractSalaryTypes.Where(x => !x.IsDeleted), "Id", "Name", 1);

                var vm = new Contract();
                vm.FromDate = new DateTime(Utility.GetDateTime().Year, Utility.GetDateTime().Month, 1);
                vm.NumberMonths = 12;
                vm.OverTime = 1;
                return View(vm);
            }
        }
        [HttpPost]
        public JsonResult CreateEdit(Contract vm, string DT_DatasourceSalaryAdditions, string DT_DatasourceSalaryPenaltys, string DT_DatasourceDefinitionVacations)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (vm.EmployeeId == null || vm.ContractTypeId == null || vm.FromDate == null || vm.NumberMonths == 0 || vm.NumberMonths == 0)
                        return Json(new { isValid = false, message = "تأكد من ادخال بيانات صحيحة" });

                    //البدلات
                    List<ContractSalaryAdditionsDT> salaryAdditionsDT = new List<ContractSalaryAdditionsDT>();
                    List<ContractSalaryAddition> salaryAdditions = new List<ContractSalaryAddition>();

                    if (DT_DatasourceSalaryAdditions != null)
                    {
                        salaryAdditionsDT = JsonConvert.DeserializeObject<List<ContractSalaryAdditionsDT>>(DT_DatasourceSalaryAdditions);
                        salaryAdditions = salaryAdditionsDT.Select(x =>
                              new ContractSalaryAddition
                              {
                                  ContractId = vm.Id,
                                  SalaryAdditionId = x.SalaryAdditionId,
                                  IsEveryMonth = true,
                                  Notes = x.SalaryAdditionNotes,
                                  Amount = x.SalaryAdditionAmount,
                                  AmountPayed = x.SalaryAdditionAmount
                              }).ToList();
                    }

                    //الخصومات
                    List<ContractSalaryPenaltyDT> salaryPenaltyDT = new List<ContractSalaryPenaltyDT>();
                    List<ContractSalaryPenalty> salaryPenalties = new List<ContractSalaryPenalty>();

                    if (DT_DatasourceSalaryPenaltys != null)
                    {
                        salaryPenaltyDT = JsonConvert.DeserializeObject<List<ContractSalaryPenaltyDT>>(DT_DatasourceSalaryPenaltys);
                        salaryPenalties = salaryPenaltyDT.Select(x =>
                              new ContractSalaryPenalty
                              {
                                  ContractId = vm.Id,
                                  SalaryPenaltyId = x.SalaryPenaltyId,
                                  IsEveryMonth = true,
                                  Notes = x.SalaryPenaltyNotes,
                                  Amount = x.SalaryPenaltyAmount,
                                  AmountPayed = x.SalaryPenaltyAmount
                              }).ToList();
                    }
                    //الاجازات
                    List<ContractDefinitionVacationDT> definitionVacationDT = new List<ContractDefinitionVacationDT>();
                    List<ContractDefinitionVacation> definitionVacations = new List<ContractDefinitionVacation>();

                    if (DT_DatasourceDefinitionVacations != null)
                    {
                        definitionVacationDT = JsonConvert.DeserializeObject<List<ContractDefinitionVacationDT>>(DT_DatasourceDefinitionVacations);
                        definitionVacations = definitionVacationDT.Select(x =>
                              new ContractDefinitionVacation
                              {
                                  ContractId = vm.Id,
                                  VacationTypeId = x.VacationTypeId,
                                  DayNumber = x.DayNumber
                              }).ToList();
                    }



                    var isInsert = false;
                    Contract model = null;
                    if (vm.Id != Guid.Empty)
                        model = db.Contracts.FirstOrDefault(x => x.Id == vm.Id);
                    else
                    {
                        model = vm;
                        model.Employee = db.Employees.FirstOrDefault(x => x.Id == vm.EmployeeId);
                    }

                    //صافى قيمة الراتب= ( الراتب الاساسى + إجمالي قيمة الدبلات -اجمالى قيمة الخصومات )
                    model.TotalSalaryAddition = salaryAdditionsDT.Sum(x => x.SalaryAdditionAmount);
                    model.TotalSalaryPenalties = salaryPenaltyDT.Sum(x => x.SalaryPenaltyAmount);
                    model.TotalDefinitionVacations = definitionVacationDT.Sum(x => x.DayNumber);

                    if (model.ContractSalaryTypeId != (int)ContractSalaryTypeCl.Monthly)
                        model.ToDate = vm.FromDate.Value.AddMonths(vm.NumberMonths);
                    else
                        model.ToDate = vm.FromDate.Value.AddMonths(vm.NumberMonths - 1);

                    if (vm.Id != Guid.Empty)
                    {
                        //delete all privous salary addition 
                        var privousSalaryAdditions = db.ContractSalaryAdditions.Where(x => x.ContractId == vm.Id).ToList();
                        foreach (var item in privousSalaryAdditions)
                        {
                            item.IsDeleted = true;
                            db.Entry(item).State = EntityState.Modified;
                        }
                        //delete all privous salary penalty
                        var privousSalaryPenalties = db.ContractSalaryPenalties.Where(x => x.ContractId == vm.Id).ToList();
                        foreach (var penalty in privousSalaryPenalties)
                        {
                            penalty.IsDeleted = true;
                            db.Entry(penalty).State = EntityState.Modified;
                        }
                        //delete all privous salary penalty
                        var privousDefinitionVactions = db.ContractDefinitionVacations.Where(x => x.ContractId == vm.Id).ToList();
                        foreach (var vaction in privousDefinitionVactions)
                        {
                            vaction.IsDeleted = true;
                            db.Entry(vaction).State = EntityState.Modified;
                        }

                        // Update Contract
                        model.ContractTypeId = vm.ContractTypeId;
                        model.ContractSalaryTypeId = vm.ContractSalaryTypeId;
                        model.EmployeeId = vm.EmployeeId;
                        model.FromDate = vm.FromDate;
                        model.Salary = vm.Salary;
                        model.OverTime = vm.OverTime;
                        model.NumberMonths = vm.NumberMonths;
                        db.Entry(model).State = EntityState.Modified;
                        db.ContractSalaryAdditions.AddRange(salaryAdditions);
                        db.ContractSalaryPenalties.AddRange(salaryPenalties);
                        db.ContractDefinitionVacations.AddRange(definitionVacations);
                    }
                    else
                    {

                        isInsert = true;
                        model.ContractSalaryAdditions = salaryAdditions;
                        model.ContractSalaryPenalties = salaryPenalties;
                        model.ContractDefinitionVacations = definitionVacations;

                        db.Contracts.Add(model);
                    }
                    if (db.SaveChanges(auth.CookieValues.UserId) > 0)
                    {
                        if (isInsert)
                            return Json(new { isValid = true, typ = (int)UploalCenterTypeCl.Employee, refGid = model.Employee.Id, isInsert, message = "تم الاضافة بنجاح" });
                        else
                            return Json(new { isValid = true, typ = (int)UploalCenterTypeCl.Employee, refGid = model.Employee.Id, isInsert, message = "تم التعديل بنجاح" });

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
        public ActionResult Edit(string conGuid)
        {
            Guid GuId;

            if (!Guid.TryParse(conGuid, out GuId) || string.IsNullOrEmpty(conGuid) || conGuid == "undefined")
                return RedirectToAction("Index");

            TempData["model"] = GuId;
            return RedirectToAction("CreateEdit");

        }
        [HttpPost]
        public ActionResult Delete(string conGuid)
        {
            Guid Id;
            if (Guid.TryParse(conGuid, out Id))
            {
                var model = db.Contracts.Where(x => x.Id == Id).FirstOrDefault();
                if (model != null)
                {
                    model.IsDeleted = true;
                    db.Entry(model).State = EntityState.Modified;

                    var salaryAdditions = db.ContractSalaryAdditions.Where(x => x.ContractId == model.Id).ToList();
                    foreach (var item in salaryAdditions)
                    {
                        item.IsDeleted = true;
                        db.Entry(item).State = EntityState.Modified;
                    }

                    var salaryPenalties = db.ContractSalaryPenalties.Where(x => x.ContractId == model.Id).ToList();
                    foreach (var item in salaryPenalties)
                    {
                        item.IsDeleted = true;
                        db.Entry(item).State = EntityState.Modified;
                    }

                    var defintionVacations = db.ContractDefinitionVacations.Where(x => x.ContractId == model.Id).ToList();
                    foreach (var item in defintionVacations)
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
        #endregion

        #region الاعتماد النهائى للعقد 
        [HttpPost]
        public ActionResult ApprovalContract(string conGuid)
        {
            try
            {
                Guid Id;
                if (Guid.TryParse(conGuid, out Id))
                {
                    var model = db.Contracts.Where(x => x.Id == Id).FirstOrDefault();
                    if (model != null)
                    {
                        //الغاء تنشيط اى عقد اخر بخلاف المحدد
                        var contracts = db.Contracts.Where(x => !x.IsDeleted && x.EmployeeId == model.EmployeeId && x.Id != model.Id).ToList();
                        foreach (var item in contracts)
                        {
                            item.IsActive = false;
                            db.Entry(item).State = EntityState.Modified;
                        }
                        //اعتماد وتنشيط العقد الحالى
                        model.IsApproval = true;
                        model.IsActive = true;
                        db.Entry(model).State = EntityState.Modified;

                        //جدولة اشهر العقد
                        //الشهرى
                        if (model.ContractSalaryTypeId == (int)ContractSalaryTypeCl.Monthly)
                        {
                            for (int i = 0; i < model.NumberMonths; i++)
                            {
                                var monthYear = model.FromDate.Value.AddMonths(i);
                                db.ContractSchedulings.Add(new ContractScheduling
                                {
                                    ContractId = model.Id,
                                    MonthYear = monthYear,
                                    ToDate = monthYear.AddMonths(1).AddDays(-1),
                                    Name = $"شهر : {monthYear.ToString("yyyy-MM-dd").Substring(0, 7)}"
                                });
                            }
                        }
                        //الاسبوعى
                        if (model.ContractSalaryTypeId == (int)ContractSalaryTypeCl.Weekly)
                        {
                            //int weekNumbers = (int)Math.Abs(Math.Round((model.FromDate - model.ToDate).Value.TotalDays)); //عدد الاسابيع
                            int days = (int)Math.Abs(Math.Round((model.ToDate - model.FromDate).Value.TotalDays)); //عدد الايام
                            //var days2 = (model.ToDate-model.FromDate).Value.TotalDays; //عدد الايام
                            //var days22 = (model.FromDate- model.ToDate).Value.TotalDays; //عدد الايام
                            //var days232 = (model.FromDate.Value.AddMonths(model.NumberMonths)- model.FromDate).Value.TotalDays; //عدد الايام
                            int weekNumbers = (int)Math.Abs(Math.Round((model.ToDate - model.FromDate).Value.TotalDays)) / 7; //عدد الاسابيع
                            var dayDIV = days % 7;// عدد الايام المتبيقية فى اخر اسبوع 

                            DateTime dtfFrom = model.FromDate.Value;
                            DateTime dtfTo = model.FromDate.Value.AddDays(6);
                            string nameWeek = $"الاسبوع رقم 1 من {dtfFrom.ToString("yyyy-MM-dd")} ==> الى {dtfTo.ToString("yyyy-MM-dd")}";
                            List<Tuple<DateTime, DateTime, string>> dateTimes = new List<Tuple<DateTime, DateTime, string>>();
                            dateTimes.Add(new Tuple<DateTime, DateTime, string>(dtfFrom, dtfTo, nameWeek));
                            for (int i = 1; i <= weekNumbers; i++)
                            {
                                db.ContractSchedulings.Add(new ContractScheduling
                                {
                                    ContractId = model.Id,
                                    MonthYear = dtfFrom,
                                    ToDate = dtfTo,
                                    Name = nameWeek
                                });
                                if (i != weekNumbers)
                                {
                                    dtfFrom = dtfTo.AddDays(1);
                                    dtfTo = dtfFrom.AddDays(6);
                                    nameWeek = $"الاسبوع رقم {i + 1} من {dtfFrom.ToString("yyyy-MM-dd")} ==> الى {dtfTo.ToString("yyyy-MM-dd")}";
                                    dateTimes.Add(new Tuple<DateTime, DateTime, string>(dtfFrom, dtfTo, nameWeek));
                                }
                            }
                            //فى حالة وجود ايام متبقية لاخر اسبوع
                            if (dayDIV > 0)
                            {
                                dtfFrom = dtfTo.AddDays(1);
                                dtfTo = dtfFrom.AddDays(dayDIV - 1);
                                nameWeek = $"الاسبوع رقم {weekNumbers + 1} من {dtfFrom.ToString("yyyy-MM-dd")} ==> الى {dtfTo.ToString("yyyy-MM-dd")}";

                                dateTimes.Add(new Tuple<DateTime, DateTime, string>(dtfFrom, dtfTo, nameWeek));
                                db.ContractSchedulings.Add(new ContractScheduling
                                {
                                    ContractId = model.Id,
                                    MonthYear = dtfFrom,
                                    ToDate = dtfTo,
                                    Name = nameWeek
                                });
                            }

                        }

                        if (db.SaveChanges(auth.CookieValues.UserId) > 0)
                            return Json(new { isValid = true, message = "تم الاعتماد بنجاح" });
                        else
                            return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });

                    }
                    else
                        return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });


                }
                else
                    return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });
            }
            catch (Exception ex)
            {

                return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });
            }
        }


        #endregion

        #region الغاء اعتماد العقد
        [HttpPost]
        public ActionResult UnApprovalContract(string conGuid)
        {
            try
            {
                Guid Id;
                if (Guid.TryParse(conGuid, out Id))
                {
                    var model = db.Contracts.Where(x => x.Id == Id).FirstOrDefault();
                    if (model != null)
                    {
                        // الغاء اعتماد وتنشيط العقد الحالى 
                        model.IsApproval = false;
                        model.IsActive = false;
                        db.Entry(model).State = EntityState.Modified;

                        if (db.SaveChanges(auth.CookieValues.UserId) > 0)
                            return Json(new { isValid = true, message = "تم االغاء الاعتماد بنجاح" });
                        else
                            return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });

                    }
                    else
                        return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });


                }
                else
                    return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });
            }
            catch (Exception ex)
            {

                return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });
            }
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