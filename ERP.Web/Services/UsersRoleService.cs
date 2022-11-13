using ERP.DAL;
using ERP.DAL.Dtos;
using ERP.DAL.Utilites;
using ERP.Web.Utilites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static ERP.Web.Utilites.Lookups;

namespace ERP.Web.Services
{
    public static class UsersRoleService
    {
        public static List<DrawSideBar> PagesLoad { get; set; }
        public static List<DrawSideBar> PagesUrl = null; //تستخدم يعرض الصفحات فى اداة البحث للقائمة الجانبية
        #region pages Sidebar
        public static List<DrawSideBar> GetPagesSideBar(Guid? roleId)
        {
            try
            {
                List<DrawSideBar> parentsDTO = new List<DrawSideBar>();
                PagesUrl = new List<DrawSideBar>();

                //List<Group> parents = new List<Group>();

                using (var db = new VTSaleEntities())
                {
                    var pageRole = db.PagesRoles.Where(x => !x.IsDeleted && x.RoleId == roleId).ToList();
                        var parents = db.Pages.Where(x => !x.IsDeleted/*&&pageRole.Any(pr=>pr.PageId==x.Id)*/).ToList();
                        parentsDTO = ChildrenGroupsOfSidBar(parents, null);
                        foreach (var item in parentsDTO)
                        {
                            var IsIn = false; //لاظهار او اخفاء العنصر فى القائمة الجانبية
                            if (item.children.Count() > 0)
                            {
                                //مستويين
                                //مستوى1
                                foreach (var item2 in item.children)
                                {
                                    //int pageId = int.Parse(item2.id);
                                    if (item2.children.Count() > 0)
                                    {
                                        var isIn2 = false;
                                        //مستوى2
                                        foreach (var item3 in item2.children)
                                        {
                                            //int pageId3 = int.Parse(item3.id);
                                            if (item3.IsPage && pageRole.Any(pr => pr.PageId == item3.PageId))
                                            {
                                                IsIn = true;
                                                isIn2 = true;
                                                item3.HasAccess = true;
                                            PagesUrl.Add(new DrawSideBar { text = item3.text, Url = item3.Url });
                                            }
                                           
                                        }
                                        if (!isIn2)
                                        {
                                            item2.ShowInSideBar = false;
                                        }
                                    }
                                    else
                                    {
                                        if (item2.IsPage && pageRole.Any(pr => pr.PageId == item2.PageId))
                                        {
                                            IsIn = true;
                                            item2.HasAccess = true;
                                        PagesUrl.Add(new DrawSideBar { text = item2.text, Url = item2.Url });

                                    }
                                }

                                }

                            }
                            if (!IsIn)
                            {
                                item.ShowInSideBar = false;
                            }
                        }

                        //if (parents.Count() > 150)
                        //{
                        //}
                        //else
                        //    parentsDTO = parents.Select(y => new TreeViewDraw
                        //    {
                        //        text = y.Name,
                        //        ParentId = y.ParentId,
                        //        Url = y.Url,
                        //        state = new States { selected = pageRole.Any(pr => pr.PageId == y.Id) ? true : false }
                        //    }).ToList();
                }
                return parentsDTO;


            }
            catch (Exception ex)
            {

                throw;
            }

        }
        static List<DrawSideBar> ChildrenGroupsOfSidBar(List<Page> pages, int? parentId)
        {
            try
            {
                var pagesTree = pages.Where(x => x.ParentId == parentId).OrderBy(x => x.OrderNum);

                return pagesTree.Select(y => new DrawSideBar
                {
                    PageId = y.Id,
                    text = y.Name,
                    ParentId = y.ParentId,
                    Icon = y.Icon,
                    Url = y.Url,
                    children = ChildrenGroupsOfSidBar(pages, y.Id),
                    IsPage = y.IsPage,
                }).ToList();

            }
            catch (Exception ex)
            {

                throw;
            }

        }
        #endregion

        #region Draw Tree View
        public static List<TreeViewDraw> GetPages(Guid? roleId)
        {
            try
            {
                List<TreeViewDraw> parentsDTO = new List<TreeViewDraw>();
                //List<Group> parents = new List<Group>();

                using (var db = new VTSaleEntities())
                {
                    var pageRole = db.PagesRoles.Where(x => !x.IsDeleted && x.RoleId == roleId);
                        var parents = db.Pages.Where(x => !x.IsDeleted).ToList();
                        parentsDTO = ChildrenGroupsOf(parents, null, pageRole);
                }
                return parentsDTO;


            }
            catch (Exception ex)
            {

                throw;
            }

        }
        static List<TreeViewDraw> ChildrenGroupsOf(List<Page> groups, int? parentId, IQueryable<PagesRole> pageRole)
        {
            try
            {
                var groupsTree = groups.Where(x => x.ParentId == parentId);

                return groupsTree.Select(y => new TreeViewDraw
                {
                    text = y.Name,
                    ParentId = y.ParentId,
                    id = y.Id.ToString(),
                    children = ChildrenGroupsOf(groups, y.Id, pageRole),
                    state = new States { selected = pageRole.Any(pr => pr.PageId == y.Id) ? true : false }
                }).ToList();

            }
            catch (Exception ex)
            {

                throw;
            }

        }

        #endregion
        //التأكد من بيانات الدخول
        public static UserInfo VaildLogin(string userName,string pass)
        {
            using (var db = new VTSaleEntities())
            {
                var passWord = VTSAuth.Encrypt(pass);
                var user = db.Users.Where(x => !x.IsDeleted && x.UserName.Trim().ToLower() == userName.Trim().ToLower() && x.Pass == passWord)
                    .Select(x => new UserInfo
                    {
                        Name = x.Person != null ? x.Person.Name : "مدير النظام",
                        PersonId = x.Person != null ? x.PersonId : null,
                        RoleId = x.RoleId,
                        UserId = x.Id,
                        IsAdmin = x.IsAdmin,
                        IsActive = x.IsActive,
                        UserName = x.UserName,
                        EmployeeId = x.Person.Employees.Where(e => !e.IsDeleted /*&& e.PersonId == x.PersonId*/).FirstOrDefault().Id,
                        //BranchId = x.Person.Employees.Where(e => !e.IsDeleted).FirstOrDefault().BranchId,
                        StoreId  = x.Person.Employees.Where(e => !e.IsDeleted).FirstOrDefault().StoreId,
                        //MyPages=x.PagesRoles.Where(e=>!e.IsDeleted).Select(p=>p.PageId).ToList()
                    })
                    .FirstOrDefault();
                if (user != null)
                {
                    var sessionKey = Guid.NewGuid().ToString().Replace("-",string.Empty)+Utility.GetDateTime().Minute+Utility.GetDateTime().Second;
                    var expirteDate = Utility.GetDateTime().AddHours(VTSAuth.cookieHours);
                    user.SessionKey = new SessionKey { key = sessionKey, ExpireDate = expirteDate };

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


//var userHasRow = db.Users.Where(x => !x.IsDeleted && x.UserName.Trim().ToLower() == userName.Trim().ToLower() && x.Pass == passWord);
//IQueryable<User> user = userHasRow;
//if (userHasRow.Count() > 0)
//{
//    var userr = new UserInfo();
//    var pageRoles = db.PagesRoles.Where(pr => !pr.IsDeleted && pr.RoleId == userHasRow.FirstOrDefault().RoleId);
//    user = user.Select(x => new UserInfo
//    {
//        Name = x.Person != null ? x.Person.Name : "مدير النظام",
//        PersonId = x.Person != null ? x.PersonId : null,
//        RoleId = x.RoleId,
//        UserId = x.Id,
//        UserName = x.UserName,
//        RolePages = db.PageCategories.Where(c => !c.IsDeleted)
//             .Select(p => new Pages { CategoryName = p.Page.PageCategory.Name, PageName = p.Page.Name, Url = p.Page.Url }).ToList()

//    }).FirstOrDefault();
