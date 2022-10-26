using ERP.Web.DataTablesDS;
using ERP.DAL;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using static ERP.Web.Utilites.Lookups;
using ERP.Web.ViewModels;

namespace ERP.Web.Services
{
    public static class GeneralDailyService
    {
        #region  GeneralDaily 

        //التأكد من ادخال رقم الحسابات الرئيسية من شاشة الاعدادات
        public static bool CheckGenralSettingHasValue(int generalSettingTypeCl)
        {
            using (var db = new VTSaleEntities())
            {
                var accountTrees = db.GeneralSettings.Where(x => x.SType == generalSettingTypeCl);
                if (accountTrees.Count() > 0)
                {
                    if (accountTrees.Any(x => x.SValue == null))
                        return false;
                    else
                        return true;
                }
                else
                    return false;
            }
        }

        //البحث عن القيود 
        public static List<GeneralDayDto> SearchGeneralDailies(DateTime? dtFrom, DateTime? dtTo, Guid? transactionId, int? transactionTypeId, Guid? accountTreeId, bool isFirstInitPage)
        {
            if (isFirstInitPage)
                return new List<GeneralDayDto>();
            IQueryable<GeneralDaily> generalDailies = null;
            using (var db = new VTSaleEntities())
            {
                generalDailies = db.GeneralDailies.Where(x => !x.IsDeleted);
                if (dtFrom != null && dtTo != null)
                    generalDailies = generalDailies.Where(x => DbFunctions.TruncateTime(x.TransactionDate) >= dtFrom && DbFunctions.TruncateTime(x.TransactionDate) <= dtTo);

                if (accountTreeId != null)
                    generalDailies = generalDailies.Where(x => x.AccountsTreeId == accountTreeId);
                if (transactionId != null)
                    generalDailies = generalDailies.Where(x => x.TransactionId == transactionId);
                if (transactionTypeId != null)
                    generalDailies = generalDailies.Where(x => x.TransactionTypeId == transactionTypeId);
                return generalDailies.Select(x => new GeneralDayDto
                {
                    Id = x.Id,
                    TransactionId = x.TransactionId,
                    AccountsTreeId = x.AccountsTreeId,
                    AccountsTreeName = x.AccountsTree.AccountName,
                    AccountNumber = x.AccountsTree.AccountNumber.ToString(),
                    Credit = x.Credit,
                    Debit = x.Debit,
                    Notes = x.Notes,
                    TransactionDate = x.TransactionDate.ToString(),
                    TransactionTypeId = x.TransactionTypeId,
                    TransactionTypeName = x.TransactionsType.Name
                }).ToList();
            }
        }
        //كشف حساب فى القيود و التابعيين  
        public static GeneralDayAccountVM SearchAccountGeneralDailies(DateTime? dtFrom, DateTime? dtTo, Guid? accountTreeId, int? customerRelate = 0, bool ShowRptEn = false)
        {
            //ShowRptEn عرض التقرير بالانجليزية والافتراضى العربية
            GeneralDayAccountVM vm = new GeneralDayAccountVM();
            using (var db = new VTSaleEntities())
            {
                if (accountTreeId != null)
                    vm = GetGeneralDailies(dtFrom, dtTo, accountTreeId, false, null, ShowRptEn);
                //if (customerRelate==1)
                //{
                //    var customerParent = db.Persons.Where(x => x.AccountsTreeId == accountTreeId&&(x.PersonTypeId==(int)PersonTypeCl.Customer||x.PersonTypeId==(int)PersonTypeCl.SupplierAndCustomer)&&x.ParentId!=null).FirstOrDefault();
                //    if (customerParent!=null)
                //    {
                //    var customerChilds = db.Persons.Where(x => !x.IsDeleted && x.ParentId == customerParent.Id);
                //    foreach (var person in customerChilds)
                //    {
                //            vm.GeneralDalies.AddRange(GetGeneralDailies(dtFrom, dtTo, person.AccountsTreeId,true).GeneralDalies);
                //    }

                //    }
                //}
                return vm;
            }
        }


        public static GeneralDayAccountVM GetGeneralDailies(DateTime? dtFrom, DateTime? dtTo, Guid? accountTreeId, bool IsCustRelated = false, string BackGroundColor = null, bool ShowRptEn = false)
        {
            GeneralDayAccountVM vm = new GeneralDayAccountVM();
            IQueryable<GeneralDaily> generalDailies = null;
            IQueryable<GeneralDaily> lastDailies = null;
            using (var db = new VTSaleEntities())
            {
                generalDailies = db.GeneralDailies.Where(x => !x.IsDeleted);
                lastDailies = db.GeneralDailies.Where(x => !x.IsDeleted);
                if (dtFrom != null && dtTo != null)
                {
                    generalDailies = generalDailies.Where(x => DbFunctions.TruncateTime(x.TransactionDate) >= dtFrom && DbFunctions.TruncateTime(x.TransactionDate) <= dtTo);
                    var dFrom = dtFrom.Value.AddDays(-1);
                    lastDailies = lastDailies.Where(x => DbFunctions.TruncateTime(x.TransactionDate) <= dFrom.Date);
                }
                if (accountTreeId != null)
                {
                    generalDailies = generalDailies.Where(x => x.AccountsTreeId == accountTreeId);
                    lastDailies = lastDailies.Where(x => x.AccountsTreeId == accountTreeId);

                }
                var list = generalDailies.OrderBy(x=>x.TransactionDate).Select(x => new GeneralDayDto
                {
                    Id = x.Id,
                    TransactionId = x.TransactionId,
                    AccountsTreeId = x.AccountsTreeId,
                    AccountsTreeName = ShowRptEn ? x.AccountsTree.AccountNameEn : x.AccountsTree.AccountName,
                    AccountNumber = x.AccountsTree.AccountNumber.ToString(),
                    Credit = x.Credit,
                    Debit = x.Debit,
                    Notes = x.Notes,
                    TransactionDate = x.TransactionDate.ToString(),
                    TransactionTypeId = x.TransactionTypeId,
                    TransactionTypeName = x.TransactionsType.Name,
                    IsCustRelated = IsCustRelated
                }).ToList();
                double generalDailiesbalance = 0;
                foreach (var item in list)
                {
                    generalDailiesbalance += (item.Debit - item.Credit);
                    item.Balance = generalDailiesbalance;
                }
                vm.GeneralDalies = list;
                //احتساب الرصيد السابق 
                var last = lastDailies.ToList();
                var lastDebit = last.Sum(x => x.Debit); // اجمالى مدين السابق
                var lastCredit = last.Sum(x => x.Credit);//اجمالى الدائن سابق
                var balanceLast = lastDebit - lastCredit;//رصيد السابق 
                if (balanceLast > 0)
                    vm.LastBalanceDebit = balanceLast;
                else if (balanceLast < 0)
                    vm.LastBalanceCredit = balanceLast;

                var totalDebit = list.Sum(x => x.Debit);//اجمالى مدين عن الفترة 
                var totalCredit = list.Sum(x => x.Credit);//اجمالى دائن عن الفترة 
                vm.TotalDebit = totalDebit;
                vm.TotalCredit = totalCredit;
                double balance = totalDebit - totalCredit; //رصيد حركة الحساب خلال الفترة
                if (balance > 0)
                    vm.TotalBalanceDebit = balance;
                if (balance < 0)
                    vm.TotalBalanceCredit = balance;

                var safy = balanceLast + balance;//الرصيد النهائى للحساب حتى نهاية الفترة 
                if (safy > 0)
                    vm.SafyBalanceDebit = safy;
                else if (safy < 0)
                    vm.SafyBalanceCredit = safy;

                vm.AccountTreeId = accountTreeId;
                var account = db.AccountsTrees.Where(x => x.Id == accountTreeId).FirstOrDefault();
                if (account != null)
                {
                    vm.AccountNumber = account.AccountNumber;
                    vm.AccountsTreeName = ShowRptEn ? account.AccountNameEn : account.AccountName;
                    vm.ShowRptEn = ShowRptEn;
                }
                vm.dtFrom = dtFrom;
                vm.dtTo = dtTo;
                return vm;
            }
        }
        #endregion
        #region ارصدة موردين
        public static List<GeneralDayAccountVM> SearchBalanceSupplier(string PersonId, string CountryId, string CityId, string AreaId, string dtFrom, string dtTo, bool isCustomer)
        {
            List<GeneralDayAccountVM> vm = new List<GeneralDayAccountVM>();
            using (var db = new VTSaleEntities())
            {
                IQueryable<Person> persons = null;
                persons = db.Persons.Where(x => !x.IsDeleted);
                if (isCustomer)
                    persons = persons.Where(x => x.PersonTypeId == (int)PersonTypeCl.Customer || x.PersonTypeId == (int)PersonTypeCl.SupplierAndCustomer);
                else
                    persons = persons.Where(x => x.PersonTypeId == (int)PersonTypeCl.Supplier || x.PersonTypeId == (int)PersonTypeCl.SupplierAndCustomer);

                if (!string.IsNullOrEmpty(PersonId))
                {
                    if (Guid.TryParse(PersonId, out var personId))
                        persons = persons.Where(x => x.Id == personId);
                }

                if (!string.IsNullOrEmpty(AreaId))
                {
                    if (Guid.TryParse(AreaId, out var Area))
                    {
                        persons = persons.Where(x => x.AreaId == Area);
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(CountryId))
                    {
                        if (Guid.TryParse(CountryId, out var country))
                            persons = persons.Where(x => x.Area.City.CountryId == country);
                    }
                    if (!string.IsNullOrEmpty(CityId))
                    {
                        if (Guid.TryParse(CityId, out var City))
                            persons = persons.Where(x => x.Area.CityId == City);
                    }
                }

                DateTime dFrom = new DateTime();
                DateTime dTo = new DateTime();
                if (!string.IsNullOrEmpty(dtFrom))
                    DateTime.TryParse(dtFrom, out dFrom);
                if (!string.IsNullOrEmpty(dtTo))
                    DateTime.TryParse(dtTo, out dTo);

                var list = persons.ToList();
                if (list.Count() > 0)
                {
                    foreach (var person in list)
                    {
                        if (isCustomer)
                        {
                            if (person.AccountsTreeCustomerId != null)
                            {
                                var item = GetGeneralDailies(dFrom, dTo, person.AccountsTreeCustomerId, false);
                                item.AccountNumber = person.AccountsTreeCustomer.AccountNumber;
                                item.AccountsTreeName = person.AccountsTreeCustomer.AccountName;
                                item.CityName = person.Area.City.Name;
                                item.AreaName = person.Area.Name;
                                vm.Add(item);
                            }
                        }
                        else
                        {
                            if (person.AccountTreeSupplierId != null)
                            {
                                var item = GetGeneralDailies(dFrom, dTo, person.AccountTreeSupplierId, false);
                                item.AccountNumber = person.AccountsTreeSupplier.AccountNumber;
                                item.AccountsTreeName = person.AccountsTreeSupplier.AccountName;
                                item.CityName = person.Area.City.Name;
                                item.AreaName = person.Area.Name;
                                vm.Add(item);
                            }
                        }

                    }
                }

                return vm;
            }
        }

        #endregion
        #region رصيد حساب
        public static Double GetAccountBalance(Guid? accountTreeId)
        {
            GeneralDayAccountVM vm = new GeneralDayAccountVM();
            IQueryable<GeneralDaily> generalDailies = null;
            using (var db = new VTSaleEntities())
            {
                generalDailies = db.GeneralDailies.Where(x => !x.IsDeleted);
                if (accountTreeId != null)
                    generalDailies = generalDailies.Where(x => x.AccountsTreeId == accountTreeId);
                else return 0;

                var list = generalDailies.Select(x => new GeneralDayDto
                {
                    //Id = x.Id,
                    //TransactionId = x.TransactionId,
                    //AccountsTreeId = x.AccountsTreeId,
                    //AccountsTreeName = x.AccountsTree.AccountName,
                    //AccountNumber = x.AccountsTree.AccountNumber.ToString(),
                    Credit = x.Credit,
                    Debit = x.Debit,
                    //Notes = x.Notes,
                    //TransactionDate = x.TransactionDate.ToString(),
                    //TransactionTypeId = x.TransactionTypeId,
                    //TransactionTypeName = x.TransactionsType.Name,
                    //IsCustRelated = IsCustRelated
                }).ToList();
                vm.GeneralDalies = list;

                var totalDebit = list.Sum(x => x.Debit);//اجمالى مدين عن الفترة 
                var totalCredit = list.Sum(x => x.Credit);//اجمالى دائن عن الفترة 
                vm.TotalDebit = totalDebit;
                vm.TotalCredit = totalCredit;
                double balance = totalDebit - totalCredit; //رصيد حركة الحساب خلال الفترة
                if (balance > 0)
                    vm.TotalBalanceDebit = balance;
                if (balance < 0)
                    vm.TotalBalanceCredit = balance;

                var safy = balance;//الرصيد النهائى للحساب حتى نهاية الفترة 
                if (safy > 0)
                    vm.SafyBalanceDebit = safy;
                else if (safy < 0)
                    vm.SafyBalanceCredit = safy;
                return vm.SafyBalanceDebit;
            }
        }

        #endregion
        #region ارصدة حسابات مختلفة لتقرير كشف حساب مجمع
        public static List<GeneralDayAccountVM> SearchBalanceMultiAccounts(List<Guid> accountTreeIds, DateTime? dtFrom, DateTime? dtTo)
        {
            List<GeneralDayAccountVM> vm = new List<GeneralDayAccountVM>();
            using (var db = new VTSaleEntities())
            {


                if (accountTreeIds.Count() > 0)
                {
                    foreach (var account in accountTreeIds)
                    {
                        var item = GetGeneralDailies(dtFrom, dtTo, account, false);
                        vm.Add(item);

                    }
                }

                return vm;
            }
        }

        #endregion

        #region التأكد من عدم تكرار القيد لنفس العملية
        //التأكد من عدم تكرار القيد لنفس العملية
        public static bool GeneralDailaiyExists(Guid? TransactionId, int? TransactionTypeId)
        {
            IQueryable<GeneralDaily> generalDay = null;
            using (var db = new VTSaleEntities())
            {
                generalDay = db.GeneralDailies.Where(x => !x.IsDeleted);
                if (TransactionId != null && TransactionId != Guid.Empty)
                    generalDay = generalDay.Where(x => x.TransactionId == TransactionId);
                else
                    return false;
                if (TransactionTypeId != null)
                    generalDay = generalDay.Where(x => x.TransactionTypeId == TransactionTypeId);
                else
                    return false;
                if (generalDay.Count() > 0)
                    return true;
                else
                    return false;
            }

        }
        #endregion
    }
}