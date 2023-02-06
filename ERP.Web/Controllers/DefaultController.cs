using Newtonsoft.Json;
using ERP.DAL;
using ERP.Web.Services;
using ERP.Web.Utilites;
using System;using ERP.DAL.Utilites;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static ERP.Web.Utilites.Lookups;
using ERP.DAL.Dtos;
using System.Web.Security;
using ERP.Web.ViewModels;
//using ERP.Web.NewFolder2;
using Page = ERP.DAL.Page;
using PagesRole = ERP.DAL.PagesRole;

namespace ERP.Web.Controllers
{
    public class DefaultController : Controller
    {
        VTSaleEntities db = new VTSaleEntities();
        VTSAuth auth = new VTSAuth();


        // GET: Default
        public ActionResult Index()
        {
            return View();
        }

        #region Login
        public ActionResult Login(int? MsgBadLogin,string ErroeMsg)
        {
            //Lookups.ExcuteFirstInit();
            //var t = VTSAuth.Encrypt("p@ssw0rd");
            //add pages
            List<Page> pages = new List<Page>();
            Guid roleID = new Guid("BCE9127F-7C2E-42F9-BB93-0DD16F4CDBB1");
            Guid userId = new Guid("52883C22-F5C2-447C-BC17-D43FA0CF689C");
            //pages.Add(new Page() { Id = 383, ParentId = 33, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = " اضافة رصيد أول المدة للعملاء مجمع", OrderNum = 0, Url = "/CustomerIntials/CreateEditIntial", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            //pages.Add(new Page() { Id = 384, ParentId = 30, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = " اضافة رصيد أول المدة للموردين مجمع", OrderNum = 0, Url = "/SupplierIntials/CreateEditIntial", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            //pages.Add(new Page() { Id = 385, ParentId = 129, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "الاعتماد المخزنى ", OrderNum = 0, Url = "/StoresTransferApprovals/Index", OtherUrls = "/StoresTransferApprovals/ApprovalStore,/StoresTransferApprovals/ShowDetails,/StoresTransferApprovals/Refused", PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            //pages.Add(new Page() { Id = 386, ParentId = 129, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "الاعتماد النهائى ", OrderNum = 0, Url = "/StoresTransfers/ApprovalFinalInvoice", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            //pages.Add(new Page() { Id = 387, ParentId = 49, Icon = "menu-bullet menu-bullet-line", IsPage = false, Name = "تسجيل العروض", OrderNum = 0, Url = "", OtherUrls = null });
            //pages.Add(new Page() { Id = 388, ParentId = 387, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "اضافة عرض", OrderNum = 0, Url = "/Offers/CreateEdit", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            //pages.Add(new Page() { Id = 389, ParentId = 387, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "ادارة بيانات عرض", OrderNum = 0, Url = "/Offers/Index", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            //pages.Add(new Page() { Id = 390, ParentId = 88, Icon = "menu-bullet menu-bullet-line", IsPage = false, Name = "مدة استحقاق السداد", OrderNum = 0, Url = "", OtherUrls = null });
            //pages.Add(new Page() { Id = 391, ParentId = 390, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "اضافة مدة استحقاق", OrderNum = 0, Url = "/ContractCustomers/CreateEdit", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            //pages.Add(new Page() { Id = 392, ParentId = 390, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "ادارة مدة استحقاق", OrderNum = 0, Url = "/ContractCustomers/Index", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            //pages.Add(new Page() { Id = 393, ParentId = 66, Icon = "icon-md fas fa-coins ylow", IsPage = false, Name = "مدة استحقاق السداد", OrderNum = 0, Url = "", OtherUrls = null });
            //pages.Add(new Page() { Id = 394, ParentId = 393, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "اضافة مدة استحقاق", OrderNum = 0, Url = "/ContractSuppliers/CreateEdit", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            //pages.Add(new Page() { Id = 395, ParentId = 393, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "ادارة مدة استحقاق", OrderNum = 0, Url = "/ContractSuppliers/Index", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            //pages.Add(new Page() { Id = 396, ParentId = 49, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "تسجيل سياسات اسعار الموردين", OrderNum = 0, Url = "/ItemPriceSuppliers/CreateEdit", OtherUrls = "/ItemPriceSuppliers/AddItemPriceSupplier", PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            //pages.Add(new Page() { Id = 397, ParentId = 50, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "استعراض المجموعات", OrderNum = 0, Url = "/ItemGroups/ShowGroups", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            //pages.Add(new Page() { Id = 398, ParentId = 88, Icon = "menu-bullet menu-bullet-line", IsPage = false, Name = "استلام نقدية مخصصة لفاتورة", OrderNum = 0, Url = "", OtherUrls = null });
            //pages.Add(new Page() { Id = 399, ParentId = 398, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "اضافة دفعة مخصصة", OrderNum = 0, Url = "/SellInvoicePayments/CreateEdit", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            //pages.Add(new Page() { Id = 400, ParentId = 398, Icon = null, IsPage = true, Name = "ادارة دفعة مخصصة", OrderNum = 0, Url = "/SellInvoicePayments/Index", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            //pages.Add(new Page() { Id = 401, ParentId = 298, Icon = null, IsPage = true, Name = "تقرير أعمار الديون", OrderNum = 0, Url = "/RptDueInvoices/AgesDebt", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            //pages.Add(new Page() { Id = 402, ParentId = null, Icon = "icon-md fas fa-coins ylow", IsPage = false, Name = "عروض الاسعار وأوامر البيع", OrderNum = 3, Url = "", OtherUrls = null });
            //pages.Add(new Page() { Id = 403, ParentId = 402, Icon = null, IsPage = false, Name = "عروض الاسعار", OrderNum = 0, Url = "", OtherUrls = null });
            //pages.Add(new Page() { Id = 404, ParentId = 403, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "تسجيل عرض سعر", OrderNum = 0, Url = "/Quotes/CreateEdit", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            //pages.Add(new Page() { Id = 405, ParentId = 403, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "ادارة عرض سعر", OrderNum = 0, Url = "/Quotes/Index", OtherUrls = "/OrderSells/CreateEdit", PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            //pages.Add(new Page() { Id = 406, ParentId = 402, Icon = "menu-bullet menu-bullet-line", IsPage = false, Name = "أوامر البيع", OrderNum = 0, Url = "", OtherUrls = null });
            //pages.Add(new Page() { Id = 408, ParentId = 406, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "ادارة أوامر البيع", OrderNum = 0, Url = "/OrderSells/Index", OtherUrls = "/OrderSells/OrderForSell,/OrderSells/OrderForProduction", PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            //pages.Add(new Page() { Id = 409, ParentId = 285, Icon = null, IsPage = true, Name = "الاصناف المباعه خلال فترة", OrderNum = 0, Url = "/RptItems/SearchItemSell", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            //pages.Add(new Page() { Id = 410, ParentId = 117, Icon = null, IsPage = false, Name = "خطوط الانتاج", OrderNum = 0, Url = "", OtherUrls = null });
            //pages.Add(new Page() { Id = 411, ParentId = 410, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "تسجيل خط انتاج", OrderNum = 0, Url = "/ProductionLines/CreateEdit", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            //pages.Add(new Page() { Id = 412, ParentId = 410, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "ادارة خط انتاج", OrderNum = 0, Url = "/ProductionLines/Index", OtherUrls = "/ProductionLines/ShowDetails", PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            //pages.Add(new Page() { Id = 413, ParentId = 303, Icon = null, IsPage = true, Name = "تقرير موظف خط الانتاج", OrderNum = 0, Url = "/RptProductionLines/SearchProductionOrderEmployee", OtherUrls = "/ProductionOrders/ShowProductionOrder,/ProductionLines/ShowDetails", PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            //pages.Add(new Page() { Id = 414, ParentId = 303, Icon = null, IsPage = true, Name = " تقرير خطوط الانتاج", OrderNum = 1, Url = "/RptProductionLines/SearchProductionLine", OtherUrls = "/ProductionOrders/ShowProductionOrder,/ProductionLines/ShowDetails", PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            //pages.Add(new Page() { Id = 415, ParentId = 121, Icon = null, IsPage = true, Name = "اضافة أمر إنتاج مجمع", OrderNum = 0, Url = "/ProductionOrders/RegisterOrderComplex", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            //pages.Add(new Page() { Id = 416, ParentId = 285, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "كارت صنف بدون التكلفة", OrderNum = 0, Url = "/RptItemBalances/SearchItemBalanceNotMovement", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            //pages.Add(new Page() { Id = 417, ParentId = 48, Icon = "menu-bullet menu-bullet-line", IsPage = false, Name = "رصيد اول المدة", OrderNum = 1, Url = "", OtherUrls = null });
            //pages.Add(new Page() { Id = 418, ParentId = 417, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "اضافة رصيد اول المدة", OrderNum = 1, Url = "/AccountTreeIntialBalances/CreateEdit", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            //pages.Add(new Page() { Id = 419, ParentId = 417, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "ادارة رصيد اول المدة", OrderNum = 1, Url = "/AccountTreeIntialBalances/Index", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });

            //db.Pages.AddRange(pages);
            //db.SaveChanges(userId);
            //db.UploadCenterTypes.Add(new UploadCenterType { Name = "امر انتاج" });
            //db.ContractSalaryTypes.Add(new ContractSalaryType { Name = "بالانتاج" });
            ////db.SaveChanges(userId);
            //var pag = db.Pages.Where(x => x.Id == 412).FirstOrDefault();
            //pag.OtherUrls = "/ProductionLines/ShowDetails";
            //var pag2 = db.Pages.Where(x => x.Id == 334).FirstOrDefault();
            //pag2.IsDeleted = true;
            ////db.SaveChanges(userId);
            //int[] nums = { 75, 83, 113, 106, 405, 408 };
            //var pag3 = db.Pages.Where(x => nums.Contains(x.Id)).ToList();
            //pag3.ForEach(x => x.OtherUrls += ",/PrintInvoices/ShowPrintInvoice");
            //int[] nums2 = { 343, 346, 203 };
            //var pag4 = db.Pages.Where(x => nums2.Contains(x.Id)).ToList();
            //pag4.ForEach(x => x.OtherUrls += ",/PrintInvoices/PrintGeneralRecord");
            ////db.SaveChanges(userId);
            //var pag5 = db.Pages.Where(x => x.Id == 203).FirstOrDefault();
            //pag5.OtherUrls += ",/UploadCenterTypeFiles/Index,/UploadCenterTypeFiles/GetByInvoGuid,/UploadCenterTypeFiles/Delete";
            ////db.SaveChanges(userId);

            //db.TransactionsTypes.Add(new TransactionsType
            //{
            //    Name = "رصيد اول المدة"
            //});
            ////db.SaveChanges(userId);
            //var pag6 = db.Pages.Where(x => x.Id == 44).FirstOrDefault();
            //pag6.OtherUrls = "/ItemIntialBalances/ShowDetails";
            //db.SelectorTypes.Add(new SelectorType { Name = "مخزن جرد مستمر" });
            //db.SaveChanges(userId);

            //var genralSettings = $@"SET IDENTITY_INSERT [dbo].[GeneralSettings] ON;
            //INSERT [dbo].[GeneralSettings] ([Id], [SName], [SValue], [SType], [CreatedBy], [CreatedOn], [IsDeleted]) VALUES (65,N'نوع الجرد',N'1',7,N'{userId}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),0)
            //INSERT[dbo].[GeneralSettings] ([Id], [SName], [SValue], [SType], [CreatedBy], [CreatedOn], [IsDeleted]) VALUES(66, N'لوجو المؤسسة', NULL, 2, N'{userId}', CAST(N'2022-07-25T18:40:07.420' AS DateTime), 0)
            //INSERT[dbo].[GeneralSettings] ([Id], [SName], [SValue], [SType], [CreatedBy], [CreatedOn], [IsDeleted]) VALUES(67, N'سطر الطباعه الاول اعلى', NULL, 2, N'{userId}', CAST(N'2022-07-25T18:40:07.420' AS DateTime), 0)
            //INSERT[dbo].[GeneralSettings] ([Id], [SName], [SValue], [SType], [CreatedBy], [CreatedOn], [IsDeleted]) VALUES(68, N'سطر الطباعه الثانى اعلى', NULL, 2, N'{userId}', CAST(N'2022-07-25T18:40:07.420' AS DateTime), 0)
            //INSERT[dbo].[GeneralSettings] ([Id], [SName], [SValue], [SType], [CreatedBy], [CreatedOn], [IsDeleted]) VALUES(69, N'سطر الطباعه الثالث اعلى', NULL, 2, N'{userId}', CAST(N'2022-07-25T18:40:07.420' AS DateTime), 0)
            //INSERT[dbo].[GeneralSettings] ([Id], [SName], [SValue], [SType], [CreatedBy], [CreatedOn], [IsDeleted]) VALUES(70, N'سطر الطباعه الرابع اسفل', NULL, 2, N'{userId}', CAST(N'2022-07-25T18:40:07.420' AS DateTime), 0)
            //INSERT[dbo].[GeneralSettings] ([Id], [SName], [SValue], [SType], [CreatedBy], [CreatedOn], [IsDeleted]) VALUES(71, N'سطر الطباعه الخامس اسفل', NULL, 2, N'{userId}', CAST(N'2022-07-25T18:40:07.420' AS DateTime), 0)
            //INSERT[dbo].[GeneralSettings]([Id], [SName], [SValue], [SType], [CreatedBy], [CreatedOn], [IsDeleted]) VALUES(72, N'حساب تكلفة بضاعه مباعه مع الجرد المستمر', NULL, 1, N'{userId}', CAST(N'2022-07-25T18:40:07.420' AS DateTime), 0)
            //INSERT[dbo].[GeneralSettings]([Id], [SName], [SValue], [SType], [CreatedBy], [CreatedOn], [IsDeleted]) VALUES(73, N'نسبة ضريبة القيمة المضافة', N'14', 7, N'{userId}', CAST(N'2022-07-25T18:40:07.420' AS DateTime), 0)
            //INSERT[dbo].[GeneralSettings]([Id], [SName], [SValue], [SType], [CreatedBy], [CreatedOn], [IsDeleted]) VALUES(74, N'نسبة ضريبة ارباح تجارية', N'1', 7, N'{userId}', CAST(N'2022-07-25T18:40:07.420' AS DateTime), 0)
            //INSERT[dbo].[GeneralSettings]([Id], [SName], [SValue], [SType], [CreatedBy], [CreatedOn], [IsDeleted]) VALUES(75, N'مجلد اوامر الانتاج', N'72739a22-438b-4848-a4fc-9abb9ba32565', 6, N'{userId}', CAST(N'2022-07-25T18:40:07.420' AS DateTime), 0)
            //INSERT[dbo].[GeneralSettings]([Id], [SName], [SValue], [SType], [CreatedBy], [CreatedOn], [IsDeleted]) VALUES(76, N'مجلد قيود اليومية', N'72739a22-438b-4848-a4fc-9abb9ba32566', 6, N'{userId}', CAST(N'2022-07-25T18:40:07.420' AS DateTime), 0)
            //INSERT[dbo].[GeneralSettings]([Id], [SName], [SValue], [SType], [CreatedBy], [CreatedOn], [IsDeleted]) VALUES(77, N'تاريخ بداية البحث', N'2023-01-01', 5, N'{userId}', CAST(N'2022-07-25T18:40:07.420' AS DateTime), 0)
            //INSERT[dbo].[GeneralSettings]([Id], [SName], [SValue], [SType], [CreatedBy], [CreatedOn], [IsDeleted]) VALUES(78, N'تاريخ نهاية البحث', N'2023-12-31', 5, N'{userId}', CAST(N'2022-07-25T18:40:07.420' AS DateTime), 0)
            //INSERT[dbo].[GeneralSettings]([Id], [SName], [SValue], [SType], [CreatedBy], [CreatedOn], [IsDeleted]) VALUES(79, N'ملاحظه عروض الاسعار', N'', 2, N'{userId}', CAST(N'2022-07-25T18:40:07.420' AS DateTime), 0)
            //SET IDENTITY_INSERT[dbo].[GeneralSettings] OFF; ";
            //db.Database.ExecuteSqlCommand(genralSettings);
            //var uploadCenters = $@"
            //        INSERT [dbo].[UploadCenters] ([Id], [FileName], [Name], [ParentId], [IsFolder], [ReferenceGuid], [UploadCenterTypeId], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (N'72739a22-438b-4848-a4fc-9abb9ba32565', NULL, N'اوامر الانتاج', NULL, 1, NULL, NULL, CAST(N'2022-09-05T13:57:28.507' AS DateTime), N'{userId}', NULL, NULL, 0, NULL, NULL)
            //        INSERT [dbo].[UploadCenters] ([Id], [FileName], [Name], [ParentId], [IsFolder], [ReferenceGuid], [UploadCenterTypeId], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (N'72739a22-438b-4848-a4fc-9abb9ba32566', NULL, N'قيود اليومية', NULL, 1, NULL, NULL, CAST(N'2022-09-05T13:57:28.507' AS DateTime), N'{userId}', NULL, NULL, 0, NULL, NULL)
            //        ";
            //db.Database.ExecuteSqlCommand(uploadCenters);





            //ItemImportDataZumurada itemImportDataZumurada = new ItemImportDataZumurada();
            //itemImportDataZumurada.UpdateDataToVersion1_3();
            ////////itemImportDataZumurada.Excute2();
            ////ItemImportMohRamadan itemImportMohRamadan = new ItemImportMohRamadan();
            ////////itemImportMohRamadan.Excute();
            ////////FixInitialBalance fixInitialBalance = new FixInitialBalance();
            ////fixInitialBalance.Excute();
            //////itemImportMohRamadan.ExcuteV1_2();
            //using (var db = new VTSaleEntities())
            //{
            //    var items = db.Items.Where(x => !x.IsDeleted).Select(x=>new {Id=x.Id,Name=x.Name,UnitId=x.UnitId,UnitName=x.Unit.Name}).ToList();
            //    var itemssss = db.Items.Where(x => !x.IsDeleted);
            //    var units = db.Units.Where(x => !x.IsDeleted );
            //    var units2 = db.Units.Where(x => !x.IsDeleted ).ToList();
            //    var unitss = db.Items.Where(x => !x.IsDeleted&&units.Any(u=>u.Id==x.UnitId) ).ToList();
            //    var unitsssss = db.Units.Where(x => !x.IsDeleted&& !itemssss.Any(i=>i.UnitId==x.Id)).ToList();
            //    var unitsssss2 = db.Units.Where(x => !x.IsDeleted&& itemssss.Any(i=>i.UnitId==x.Id)).ToList();

            //    var unitsIte = db.ItemUnits.Where(x => !x.IsDeleted&& units.Any(y=>y.Id==x.UnitId)).ToList();
            //    var unitsIte2 = db.ItemUnits.Where(x => !x.IsDeleted&& !units.Any(y=>y.Id==x.UnitId)).ToList();
            //    var iteUnits = db.ItemUnits.Where(x => !x.IsDeleted);
            //    var iteUnitsss = db.ItemUnits.Where(x => !x.IsDeleted).ToList();
            //    var yy = db.Units.Where(x => !x.IsDeleted && iteUnits.Any(y => y.UnitId == x.Id)).ToList();
            //    var tt = units.ToList();
            //    //foreach (var unit in units.ToList())
            //    //{
            //    //    unit.IsDeleted = true;
            //    //}
            //}
            if (MsgBadLogin!=null)
                if(MsgBadLogin==1)
                ViewBag.MsgBadLogin = "انتهاء صلاحية الجلسة";
            if (MsgBadLogin == 2)
                ViewBag.MsgBadLogin = "حدث خطأ اثناء تنفيذ العملية " + " ErroeMsg= "+ ErroeMsg;
            if (MsgBadLogin == 3)
                ViewBag.MsgBadLogin = "غير مصرح لك الوصول لهذه الشاشة او انتهاء صلاحية الجلسة ";
            if (MsgBadLogin == 4)
                ViewBag.MsgBadLogin =  "تم تسجيل الدخول فى جهاز آخر";
            //تسجيل الدخول مباشرا فى حالة وجود كوكيز ومدته صالحة
            VTSAuth auth = new VTSAuth();
            if (auth.LoadDataFromCookies())
            {
                var roleId = auth.CookieValues.RoleId;
                //التأكد من عمد انتها صلاحية الجلسة
                if (Utility.GetDateTime() > auth.CookieValues.SessionKey.ExpireDate)
                    return View();
                else
                {
                    if (!VTSAuth.IsDemo)
                    {
                        using (var db = new VTSaleEntities())
                        {
                            var sessionDb = db.Users.Where(x => x.Id == auth.CookieValues.UserId).FirstOrDefault().SessionKey;
                            var sessionCookie = VTSAuth.Encrypt(JsonConvert.SerializeObject(auth.CookieValues.SessionKey));
                            if (sessionDb != sessionCookie)
                                return View();
                            return RedirectToAction("Index", "Home");
                        }
                    }else
                        return RedirectToAction("Index", "Home");

                }

            }
                    //if (TempData["MsgErrorActive"]!=null)
                    //    ViewBag.MsgBadLogin = TempData["MsgErrorActive"];
                    return View();
        }      
        [HttpPost]
        public ActionResult Login(string userName, string pass, string returnUrlVal)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(pass))
            {
                ViewBag.MsgBadLogin = "تأكد من ادخال اسم المستخدم وكلمة المرور";
                return View();
            }
            ////التأكد من صلاحية فتح الموقع فى حالة الافلاين
            //if (!SecurityService.isSecured())
            //{
            //    ViewBag.MsgBadLogin = "ليس لك صلاحية لاستخدام الموقع ";
            //    return View();
            //}
            UserInfo user = UsersRoleService.VaildLogin(userName, pass);
            if (user != null)
            {
                List<DrawSideBar> pages = new List<DrawSideBar>();
                var userDb = db.Users.FirstOrDefault(x=>x.Id==user.UserId);
                var roleId = userDb.RoleId;
                //Stopwatch stopwatch = new Stopwatch();
                //stopwatch.Start();
                //pages = UsersRoleService.GetPagesSideBar(roleId);
                //stopwatch.Stop();
                //refactor service -- remote 800 mili
                //remote 33 seconds,172 Mili
                //local 511 Mili
                //user.MyPages = pages.Select(x => x.PageId).ToList();
                auth.SaveToCookies(user);
                userDb.LastLogin = Utility.GetDateTime();
                userDb.SessionKey = VTSAuth.Encrypt(JsonConvert.SerializeObject(user.SessionKey));
                db.Entry(userDb).State = EntityState.Modified;
                db.SaveChanges(user.UserId);

                //UsersRoleService.PagesLoad = pages;
                //Session["pagesSession"] = pages;

                //Stopwatch stopwatch = new Stopwatch();
                //stopwatch.Start();
                //var roleId = 1;

                //UsersRoleService.GetPages(roleId);
                //stopwatch.Stop();

                //Stopwatch stopwatch3 = new Stopwatch();
                //stopwatch3.Start();
                //var pageRoles = db.PagesRoles.Where(x => !x.IsDeleted && x.RoleId == userDb.RoleId).GroupBy(pp => pp.Page.PageCatogryId);
                //var pages = pageRoles.Select(x => new SideBarPages
                //{
                //    CategoryName = x.Select(c => c.Page.PageCategory.Name).FirstOrDefault(),
                //    Pages = x.Select(p => new ListPages
                //    {
                //        PageName = p.Page.Name,
                //        Url = p.Page.Url
                //    }).ToList()
                //}).ToList();

                //stopwatch3.Stop();

                //Session["pagesSession"] = pages;
                if (!string.IsNullOrEmpty(returnUrlVal))
                    return Redirect(returnUrlVal);
                else
                    return RedirectToAction("Index", "Home");
            }
            else
                ViewBag.MsgBadLogin = "اسم المستخدم او كلمة المرور خاطئة";

            return View();
        }

        #endregion

        #region LogOut
        public ActionResult LogOut()
        {
            auth.ClearCookies();
            //UsersRoleService.PagesLoad = null;
            //Session["pagesSession"] = null;
            return View("Login");
        }

        #endregion
        #region Active/Un Offline Mode
        // تفعيل الموقع افلاين
        [HttpPost]
        public ActionResult Activation(string activePass)
        {
            try
            {
                if (string.IsNullOrEmpty(activePass))
                {
                    TempData["MsgErrorActive"] = "رقم التفعيل خاطئ";
                    return RedirectToAction("Login");
                }
                string hashedValue = "F854C93C2F3F8D34257DD9B07024FB4B";

                var hashedString = VTSAuth.CreateMD5(activePass);
                if (hashedString == hashedValue)
                {
                    string pcSerial = SecurityService.getSerialID();
                    string serialEncrypted = VTSAuth.Encrypt(pcSerial);
                    var model = db.GeneralSettings.Where(x => !x.IsDeleted && x.Id == (int)GeneralSettingCl.EntityDataSecurity).FirstOrDefault();
                    model.SValue = serialEncrypted;
                    db.Entry(model).State = EntityState.Modified;
                    if (db.SaveChanges(null) > 0)
                    {
                        TempData["MsgSuccessActive"] = "تم التفعيل بنجاح";
                        return RedirectToAction("Login");
                    }
                    else
                    {
                        TempData["MsgErrorActive"] = "حدث خطأ اثناء تنفيذ العملية";
                        return RedirectToAction("Login");
                    }
                }
                else
                {
                    TempData["MsgErrorActive"] = "رقم التفعيل خاطئ";
                    return RedirectToAction("Login");
                }
            }
            catch (Exception ex)
            {
                {
                    TempData["MsgErrorActive"] = ex.Message;
                    return RedirectToAction("Login");
                }
            }

        }

        #endregion
        public ActionResult Error(string ErroeMsg)
        {
            //auth.ClearCookies();
            ViewBag.MsgBadLogin = ErroeMsg;
            return View();
        }

        //List<ListPages> GetPages(int catId, IQueryable<PagesRole> pageRoless)
        //{
        //    return db.Pages.Where(p => !p.IsDeleted && p.PageCatogryId == catId && pageRoless.Any(pr => pr.PageId == p.Id)).Select(p => new ListPages
        //    {
        //        PageName = p.Name,
        //        Url = p.Url
        //    }).ToList();
        //}
        //bool HasPages(int catId, IQueryable<PagesRole> pageRolee)
        //{
        //    return db.Pages.Where(p => !p.IsDeleted && pageRolee.Any(pr => pr.PageId == p.Id)).Any();
        //}

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