using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ERP.DAL.Dtos;
using ERP.DAL.Utilites;
using ERP.Web.Utilites;
using static ERP.Web.Utilites.Lookups;

namespace ERP.Desktop.Services
{
    public static class UserServices
    {
        internal static UserInfo UserInfo { get; set; }
        public static UserInfo Login(string userName, string password)
        {
            var db = DBContext.UnitDbContext;
            {
                var passWord = VTSAuth.Encrypt(password);
                var sss = VTSAuth.Encrypt("1");
                UserInfo user = null;
                var userQ = db.Users.Where(x => !x.IsDeleted && x.UserName.Trim().ToLower() == userName.Trim().ToLower() && x.Pass == passWord);
                if (userQ.Count()>0)
                {
                    var usr = userQ.FirstOrDefault();
                    if (usr.Person != null)
                    {
                        if (usr.Person.Employees.Where(e => !e.IsDeleted).Any())
                        {
                            user = userQ.Select(x => new UserInfo
                            {
                                Name = x.Person.Name ,
                                PersonId = x.Person != null ? x.PersonId : null,
                                IsAdmin = x.IsAdmin,
                                RoleId = x.RoleId,
                                UserId = x.Id,
                                UserName = x.UserName,
                                IsActive = x.IsActive ? true : false,
                                EmployeeId = x.Person.Employees.Where(e => !e.IsDeleted /*&& e.PersonId == x.PersonId*/).FirstOrDefault().Id,
                                //BranchId = x.Person.Employees.Where(e => !e.IsDeleted).FirstOrDefault().BranchId,
                                StoreId = x.Person.Employees.Where(e => !e.IsDeleted).FirstOrDefault().EmployeeStores.FirstOrDefault().StoreId,
                            }).FirstOrDefault();
                        }
                    }
                    else if (usr.Person == null)
                        user = userQ.Select(x => new UserInfo
                        {
                            Name =  "مدير النظام",
                            PersonId = x.Person != null ? x.PersonId : null,
                            IsAdmin = x.IsAdmin,
                            RoleId = x.RoleId,
                            UserId = x.Id,
                            UserName = x.UserName,
                            IsActive = x.IsActive ? true : false
                        }).FirstOrDefault();
                }
                    
                if (user != null)
                {
                    //اسم المؤسسة الظاهر على الصفحة 
                    // الحصول على بيانات المؤسسة من الاعدادات
                    var generalSetting = db.GeneralSettings.Where(x => x.SType == (int)GeneralSettingTypeCl.EntityData).ToList();
                    var entityName = generalSetting.Where(x => x.Id == (int)GeneralSettingCl.EntityDataEntityDataName).FirstOrDefault().SValue;
                    user.EntityName = entityName;
                    return user;
                }
                else
                    return null; //اسم المستخدم او كلمة المرور خاطئة
            }
        }
    }
}
