﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ERP.DAL.Models;
using ERP.DAL.Utilites;

namespace ERP.DAL
{
    public partial class VTSaleEntities
    {
        #region Adding Lookups on Database
        public static bool ExcuteFirstInit()
        {
            try
            {
                using (var db = new VTSaleEntities())
                {
                    var user = new User
                    {
                        IsActive = true,
                        IsAdmin = true,
                        Pass = VTSAuth.Encrypt("P@ssw0rd"),//P@ssw0rd
                        UserName = "admin"
                    };
                    // add user
                    if (db.Users.Where(x => !x.IsDeleted).Count() == 0)
                    {
                        db.Users.Add(user);
                        db.SaveChanges(null);
                    }
                    else
                        user = db.Users.FirstOrDefault();


                    //اضافة صلاحية الادمن 
                    Role role = null;
                    if (db.Roles.Where(x => !x.IsDeleted).Count() == 0)
                    {
                        role = new Role { Name = "مدير النظام", IsAdmin = true };
                        db.Roles.Add(role);
                        db.SaveChanges(user.Id);

                        user.RoleId = role.Id;
                    }
                    else
                        role = db.Roles.Where(x => x.IsAdmin).FirstOrDefault();




                    //اضافة مجموعات الصفحات
                    if (db.Pages.Where(x => !x.IsDeleted).Count() == 0)
                        AddingPages(db, role.Id, user.Id);
                    // =========================
                    //                  


                    //اضافة صفحات الادمن 
                    //if (db.PagesRoles.Where(x => !x.IsDeleted).Count() == 0)
                    //{
                    //    var pages = db.Pages.Where(x => !x.IsDeleted && x.IsPage);
                    //    foreach (var item in pages)
                    //    {
                    //        db.PagesRoles.Add(new PagesRole
                    //        {
                    //            Page = item,
                    //            Role = role
                    //        });

                    //    }
                    //}

                    //add Week Day 
                    if (db.WeekDays.Where(x => !x.IsDeleted).Count() == 0)
                    {
                        db.WeekDays.Add(new WeekDay { Name = "الاثنين", Arrange = 3 });
                        db.WeekDays.Add(new WeekDay { Name = "الثلاثاء", Arrange = 4 });
                        db.WeekDays.Add(new WeekDay { Name = "الاربعاء", Arrange = 5 });
                        db.WeekDays.Add(new WeekDay { Name = "الخميس", Arrange = 6 });
                        db.WeekDays.Add(new WeekDay { Name = "الجمعة", Arrange = 7 });
                        db.WeekDays.Add(new WeekDay { Name = "السبت", Arrange = 1 });
                        db.WeekDays.Add(new WeekDay { Name = "الاحد", Arrange = 2 });
                    }
                    //add Gender
                    if (db.Genders.Where(x => !x.IsDeleted).Count() == 0)
                    {
                        db.Genders.Add(new Gender { Name = "ذكر" });
                        db.Genders.Add(new Gender { Name = "انثى" });
                    }

                    // add Person types انواع الاشخاص (مورد عميل - مود وعميل - موظف
                    if (db.PersonTypes.Where(x => !x.IsDeleted).Count() == 0)
                    {
                        db.PersonTypes.Add(new PersonType { Name = "مورد" });
                        db.PersonTypes.Add(new PersonType { Name = "عميل" });
                        db.PersonTypes.Add(new PersonType { Name = "مورد وعميل" });
                        db.PersonTypes.Add(new PersonType { Name = "موظف" });
                        db.PersonTypes.Add(new PersonType { Name = "مسئول عميل / مورد" });
                    }
                    // add social types انواع الاشخاص (اعزب - متزوج- مطلق - ارمل
                    if (db.SocialStatuses.Where(x => !x.IsDeleted).Count() == 0)
                    {
                        db.SocialStatuses.Add(new SocialStatus { Name = "اعزب" });
                        db.SocialStatuses.Add(new SocialStatus { Name = "متزوج" });
                        db.SocialStatuses.Add(new SocialStatus { Name = "مطلق" });
                        db.SocialStatuses.Add(new SocialStatus { Name = "ارمل" });
                    }

                    // add Group types انواع مجموعات الاصناف (اساسية - بيع)
                    if (db.GroupTypes.Where(x => !x.IsDeleted).Count() == 0)
                    {
                        db.GroupTypes.Add(new GroupType { Name = "مجموعة اسياسية" });
                        db.GroupTypes.Add(new GroupType { Name = "مجموعة بيع" });
                    }

                    //============== Account Tree ======================
                    // انواع الحساب فى الشجرة 
                    if (db.SelectorTypes.Where(x => !x.IsDeleted).Count() == 0)
                    {
                        db.SelectorTypes.Add(new SelectorType { Name = "حساب رئيسي" });
                        db.SelectorTypes.Add(new SelectorType { Name = "حساب فرعي" });
                        db.SelectorTypes.Add(new SelectorType { Name = "حساب تشغيلي " });
                       
                    }
                    //============== end ===============================



                    //============== Account Tree ======================
                    // انواع التوجيه فى الشجرة 
                    if (db.OrientationTypes.Where(x => !x.IsDeleted).Count() == 0)
                    {
                        db.OrientationTypes.Add(new OrientationTypes { Name = "المركز المالي" });
                        db.OrientationTypes.Add(new OrientationTypes { Name = "قائمة الدخل" });

                    }
                    //============== end ===============================
                    //============== General Setting ======================
                    //انواع الاعدادات العامة
                    // انواع الحساب فى الشجرة 
                    if (db.GeneralSettingTypes.Where(x => !x.IsDeleted).Count() == 0)
                    {
                        db.GeneralSettingTypes.Add(new GeneralSettingType { Name = "شجرة الحسابات" });
                        db.GeneralSettingTypes.Add(new GeneralSettingType { Name = "بيانات الجهة" });
                        db.GeneralSettingTypes.Add(new GeneralSettingType { Name = "طول الباركود" });
                        db.GeneralSettingTypes.Add(new GeneralSettingType { Name = "المخزن الافتراضى" });
                        db.GeneralSettingTypes.Add(new GeneralSettingType { Name = "التواريخ بداية ونهاية " });
                        db.GeneralSettingTypes.Add(new GeneralSettingType { Name = "رفع الملفات الى مركز التحميل" });
                        db.GeneralSettingTypes.Add(new GeneralSettingType { Name = "اعدادات اخرى" });
                    }

                    //الجنسيات
                    ExcuteNationaties(user.Id.ToString(), db);
                    // الاعدادات العامة 
                    db.SaveChanges(user.Id);
                    ExcuteAccountTreeAndGeneralSettings(user.Id.ToString(), db);
                    //AddingFunctionsToDataBase(db);
                    ExcuteUploadCenter(user.Id.ToString(), db);

                  


                  


                    // انواع طريقة السداد 
                    if (db.PaymentTypes.Where(x => !x.IsDeleted).Count() == 0)
                    {
                        db.PaymentTypes.Add(new PaymentType { Name = "نقدي" });
                        db.PaymentTypes.Add(new PaymentType { Name = "آجل" });
                        db.PaymentTypes.Add(new PaymentType { Name = "جزئي" });
                        db.PaymentTypes.Add(new PaymentType { Name = "تقسيط" });
                        db.PaymentTypes.Add(new PaymentType { Name = "محفظة بنكية" });
                        db.PaymentTypes.Add(new PaymentType { Name = "بطاقة بنكية(فيزا)" });
                    }
                    // add Transactions Types انواع مجموعات حركات المعاملات)
                    if (db.TransactionsTypes.Where(x => !x.IsDeleted).Count() == 0)
                    {

                        db.TransactionsTypes.Add(new TransactionsType { Name = "مشتريات" });
                        db.TransactionsTypes.Add(new TransactionsType { Name = "مبيعات" });
                        db.TransactionsTypes.Add(new TransactionsType { Name = "مرتجع مشتريات" });
                        db.TransactionsTypes.Add(new TransactionsType { Name = "مرتجع مبيعات" });
                        db.TransactionsTypes.Add(new TransactionsType { Name = "صرف نقدية" });
                        db.TransactionsTypes.Add(new TransactionsType { Name = "توريد (ادخال) نقدية" });
                        db.TransactionsTypes.Add(new TransactionsType { Name = "تسجيل مصروف" });
                        db.TransactionsTypes.Add(new TransactionsType { Name = "رصيد اول المدة اصناف" });
                        db.TransactionsTypes.Add(new TransactionsType { Name = "قيود يومية" });
                        db.TransactionsTypes.Add(new TransactionsType { Name = "تكاليف وامر الانتاج" });
                        db.TransactionsTypes.Add(new TransactionsType { Name = "صرف شيك لمورد" });
                        db.TransactionsTypes.Add(new TransactionsType { Name = "استلام شيك من عميل" });
                        db.TransactionsTypes.Add(new TransactionsType { Name = "سلف وقروض الموظفين" });
                        db.TransactionsTypes.Add(new TransactionsType { Name = "رواتب واجور الموظفين" });
                        db.TransactionsTypes.Add(new TransactionsType { Name = "عٌهد الموظفين" });
                        db.TransactionsTypes.Add(new TransactionsType { Name = "اصول ثابتة" });
                        db.TransactionsTypes.Add(new TransactionsType { Name = "ارجاع عهدة مصروفة" });
                        db.TransactionsTypes.Add(new TransactionsType { Name = "ارجاع عهدة نقدية" });
                        db.TransactionsTypes.Add(new TransactionsType { Name = "صيانة" });
                        db.TransactionsTypes.Add(new TransactionsType { Name = "اهلاك الاصول الثابتة" });
                        db.TransactionsTypes.Add(new TransactionsType { Name = "تسجيل إيراد" });
                        db.TransactionsTypes.Add(new TransactionsType { Name = "سند صرف" });
                        db.TransactionsTypes.Add(new TransactionsType { Name = "سند قبض" });
                        db.TransactionsTypes.Add(new TransactionsType { Name = "قسط" });
                        db.TransactionsTypes.Add(new TransactionsType { Name = "رصيد اول عملاء" });
                        db.TransactionsTypes.Add(new TransactionsType { Name = "رصيد اول موردين" });
                        db.TransactionsTypes.Add(new TransactionsType { Name = "رصيد اول اصول ثابتة" });
                        db.TransactionsTypes.Add(new TransactionsType { Name = "رصيد اول خزن" });
                        db.TransactionsTypes.Add(new TransactionsType { Name = "رصيد اول بنوك" });
                        db.TransactionsTypes.Add(new TransactionsType { Name = "رصيد اول اصول" });
                        db.TransactionsTypes.Add(new TransactionsType { Name = "رصيد اول حقوق ملكية" });
                        db.TransactionsTypes.Add(new TransactionsType { Name = "اذن استلام" });
                        db.TransactionsTypes.Add(new TransactionsType { Name = "اذن صرف" });
                        db.TransactionsTypes.Add(new TransactionsType { Name = "رصيد اول المدة" });
                    }
                    // add Cases حالات الفواتير)
                    if (db.Cases.Where(x => !x.IsDeleted).Count() == 0)
                    {
                        db.Cases.Add(new Case { Name = "تم تسجيل الفاتورة" });
                        db.Cases.Add(new Case { Name = "تم تعديل الفاتورة" });
                        db.Cases.Add(new Case { Name = "تم حذف الفاتورة" });
                        db.Cases.Add(new Case { Name = "تم اعتماد الفاتورة محاسبيا" });
                        db.Cases.Add(new Case { Name = "تم اعتماد الفاتورة مخزنيا" });
                        db.Cases.Add(new Case { Name = "تم ادخال كميات فعلية مختلفة عن الاصلية " });
                        db.Cases.Add(new Case { Name = "تم اعتماد الفاتورة بشكل نهائى " });
                        db.Cases.Add(new Case { Name = "تم تسجيل فاتورة مرتجع" });
                        db.Cases.Add(new Case { Name = "تم تعديل  فاتورة مرتجع" });
                        db.Cases.Add(new Case { Name = "تم حذف فاتورة مرتجع" });
                        db.Cases.Add(new Case { Name = "تم اعتماد فاتورة مرتجع محاسبيا" });
                        db.Cases.Add(new Case { Name = "تم اعتماد فاتورة مرتجع مخزنيا" });
                        db.Cases.Add(new Case { Name = "تم ادخال كميات فعلية مختلفة عن الاصلية " });
                        db.Cases.Add(new Case { Name = "تم اعتماد فاتورة المرتجع  بشكل نهائى " });
                        db.Cases.Add(new Case { Name = "تم ارجاع كامل الفاتورة" });
                        db.Cases.Add(new Case { Name = "تم ارجاع جزء من الفاتورة" });
                        db.Cases.Add(new Case { Name = "تم تسجيل فاتورة من خلال مندوب" });
                        db.Cases.Add(new Case { Name = "تم تعديل الفاتورة من خلال المندوب" });
                        db.Cases.Add(new Case { Name = "تم تسجيل فاتورة مرتجع من خلال مندوب" });
                        db.Cases.Add(new Case { Name = "تم تعديل فاتورة مرتجع من خلال المندوب" });
                        db.Cases.Add(new Case { Name = "تم فك الاعتماد للفاتورة" });
                        //db.Cases.Add(new Case { Name = "تم تسجيل فاتورة صيانة" });
                        //db.Cases.Add(new Case { Name = "تم تعديل فاتورة الصيانة" });
                        //db.Cases.Add(new Case { Name = "تم حذف فاتورة صيانة" });
                    }

                    // add MaintenanceCases حالات فواتير الصيانة)
                    if (db.MaintenanceCases.Where(x => !x.IsDeleted).Count() == 0)
                    {
                        db.MaintenanceCases.Add(new MaintenanceCas { Name = "قيد الانتظار" });
                        db.MaintenanceCases.Add(new MaintenanceCas { Name = "جارى الصيانة" });
                        db.MaintenanceCases.Add(new MaintenanceCas { Name = "تم الرفض" });
                        db.MaintenanceCases.Add(new MaintenanceCas { Name = "تم التنفيذ " });
                        db.MaintenanceCases.Add(new MaintenanceCas { Name = "تم التأجيل " });
                        db.MaintenanceCases.Add(new MaintenanceCas { Name = "تم الاعتماد المحاسبى ", ForAdmin = true });
                        db.MaintenanceCases.Add(new MaintenanceCas { Name = "تم الاعتماد المخزنى ", ForAdmin = true });
                        db.MaintenanceCases.Add(new MaintenanceCas { Name = "تم الاعتماد النهائى ", ForAdmin = true });
                    }
                    // Upload center type انواع مركز التحميل
                    if (db.UploadCenterTypes.Where(x => !x.IsDeleted).Count() == 0)
                    {
                        db.UploadCenterTypes.Add(new UploadCenterType { Name = "فاتورة التوريد" });
                        db.UploadCenterTypes.Add(new UploadCenterType { Name = "فاتورة مرتجع توريد" });
                        db.UploadCenterTypes.Add(new UploadCenterType { Name = "فاتورة بيع" });
                        db.UploadCenterTypes.Add(new UploadCenterType { Name = "فاتورة بيع" });
                        db.UploadCenterTypes.Add(new UploadCenterType { Name = "موظف" });
                        db.UploadCenterTypes.Add(new UploadCenterType { Name = "شيك بنكى " });
                        db.UploadCenterTypes.Add(new UploadCenterType { Name = "مورد" });
                        db.UploadCenterTypes.Add(new UploadCenterType { Name = "عميل" });
                        db.UploadCenterTypes.Add(new UploadCenterType { Name = "قسط" });
                        db.UploadCenterTypes.Add(new UploadCenterType { Name = "امر انتاج" });
                        db.UploadCenterTypes.Add(new UploadCenterType { Name = "قيود يومية" });
                    }


                    //طريقة احتساب تكلفة المنتج
                    if (db.ItemCostCalculations.Where(x => !x.IsDeleted).Count() == 0)
                    {
                        db.ItemCostCalculations.Add(new ItemCostCalculation { Name = "المتوسط المرجح" });
                        db.ItemCostCalculations.Add(new ItemCostCalculation { Name = "اخر سعر شراء" });
                        db.ItemCostCalculations.Add(new ItemCostCalculation { Name = "اعلى سعر شراء" });
                        db.ItemCostCalculations.Add(new ItemCostCalculation { Name = "اقل سعر شراء" });
                    }
                    //حالات سيريال الاصناف
                    if (db.SerialCases.Where(x => !x.IsDeleted).Count() == 0)
                    {
                        db.SerialCases.Add(new SerialCas { Name = "عملية بيع" });
                        db.SerialCases.Add(new SerialCas { Name = "عملية مرتجع بيع" });
                        db.SerialCases.Add(new SerialCas { Name = "صيانة" });
                        db.SerialCases.Add(new SerialCas { Name = "تم الصيانه وتسليمه للعميل" });
                        db.SerialCases.Add(new SerialCas { Name = "تم الصيانه وتحويله الى مخزن" });
                        db.SerialCases.Add(new SerialCas { Name = "تحويل مخزنى" });
                        db.SerialCases.Add(new SerialCas { Name = "عملية بيع قطع غيار" });
                    }
                    // نوع صرف الراتب للعقد يومى/اسبوعى/شهرى             
                    if (db.ContractSalaryTypes.Where(x => !x.IsDeleted).Count() == 0)
                    {
                        db.ContractSalaryTypes.Add(new ContractSalaryType { Name = "شهرى" });
                        db.ContractSalaryTypes.Add(new ContractSalaryType { Name = "اسبوعى" });
                        db.ContractSalaryTypes.Add(new ContractSalaryType { Name = "يومى" });
                        db.ContractSalaryTypes.Add(new ContractSalaryType { Name = "بالانتاج" });
                    }
                    // //نوع المنتج (مدخلات/مخرجات) مثلا مواد خام/منتج نهائى             
                    if (db.ProductionTypes.Where(x => !x.IsDeleted).Count() == 0)
                    {
                        db.ProductionTypes.Add(new ProductionType { Name = "مدخلات" });
                        db.ProductionTypes.Add(new ProductionType { Name = "مخرجات" });
                    }                 
                    // //حالات التحويلات المخزنية             
                    if (db.StoresTransferCases.Where(x => !x.IsDeleted).Count() == 0)
                    {
                        db.StoresTransferCases.Add(new StoresTransferCase { Name = "تم تسجيل التحويل المخزنى" });
                        db.StoresTransferCases.Add(new StoresTransferCase { Name = "تم تعديل التحويل المخزنى" });
                        db.StoresTransferCases.Add(new StoresTransferCase { Name = "تم حذف التحويل المخزنى" });
                        db.StoresTransferCases.Add(new StoresTransferCase { Name = "تم اعتماد التحويل مخزنيا" });
                        db.StoresTransferCases.Add(new StoresTransferCase { Name = "تم رفض التحويل مخزنيا" });
                        db.StoresTransferCases.Add(new StoresTransferCase { Name = "تم ادخال كميات فعلية مختلفة عن الاصلية " });
                        db.StoresTransferCases.Add(new StoresTransferCase { Name = "تم الاعتماد بشكل نهائى " });
                        db.StoresTransferCases.Add(new StoresTransferCase { Name = "فك اعتماد تحويل مخزنى " });
                    }
                    // //انواع العروض             
                    if (db.OfferTypes.Where(x => !x.IsDeleted).Count() == 0)
                    {
                        db.OfferTypes.Add(new OfferType { Name = "عرض على صنف او اكتر" });
                        db.OfferTypes.Add(new OfferType { Name = "عرض على فاتورة" });
                    }
                    // //حالات امر البيع              
                    if (db.OrderSellCases.Where(x => !x.IsDeleted).Count() == 0)
                    {
                        db.OrderSellCases.Add(new OrderSellCase { Name = "جارى التنفيذ" });
                        db.OrderSellCases.Add(new OrderSellCase { Name = "تم البيع" });
                        db.OrderSellCases.Add(new OrderSellCase { Name = "تم الانتاج" });
                        db.OrderSellCases.Add(new OrderSellCase { Name = "تم الانتهاء" });
                    }
                    db.SaveChanges(user.Id);


                    //اضافة الدولة والمدينة والمنطقة والفرع والمخازن
                    if (db.Countries.Where(x => !x.IsDeleted).Count() == 0)
                    {
                        db.Countries.Add(new Country
                        {
                            Name = "مصر",
                            Cities = new List<City>()
                            {
                                new City
                                {
                                    Name = "القاهرة",
                                    Areas = new List<Area>
                                    {
                                        new Area
                                        {
                                            Name = "القاهرة",
                                            Branches = new List<Branch>
                                            {
                                                new Branch
                                                {
                                                    Name = "الفرع الرئيسى",
                                                    Stores = new List<Store>
                                                    {
                                                        new Store { Name = "المخزن الرئيسى" },
                                                        new Store { Name = "مخزن التصنيع الافتراضى" },
                                                        new Store { Name = " مخزن تحت التصنيع" },
                                                        new Store { Name = " مخزن الصيانة" },
                                                        new Store { Name = " مخزن توالف الصيانة" }
                                                    },
                                                    Safes = new List<Safe>
                                                    {
                                                        new Safe
                                                        {
                                                            Name = "الخزنة الرئيسية",
                                                            AccountsTreeId = new Guid("00000133-1234-1234-1234-012345678910")
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    },
                                    Banks = new List<Bank>
                                    {
                                        new Bank
                                        {
                                            Name = "بنك مصر",
                                            BankAccounts = new List<BankAccount>
                                            {
                                                new BankAccount
                                                {
                                                    AccountName = "الحساب الرئيسيى للشركة",
                                                    AccountNo = "123456789",
                                                    AccountsTreeId = new Guid("00000134-1234-1234-1234-012345678910")
                                                }
                                            }
                                        },new Bank
                                        {
                                            Name = "المحافظ البنكية",
                                            BankAccounts = new List<BankAccount>
                                            {
                                                new BankAccount
                                                {
                                                    AccountName = "حساب فودافون كاش",
                                                    AccountNo = "12345678910",
                                                    AccountsTreeId = new Guid("00000135-1234-1234-1234-012345678910")
                                                }
                                            }
                                        },new Bank
                                        {
                                            Name = "البطاقات البنكية..فيزا",
                                            BankAccounts = new List<BankAccount>
                                            {
                                                new BankAccount
                                                {
                                                    AccountName = "بطاقة فيزا",
                                                    AccountNo = "1234567891011",
                                                    AccountsTreeId = new Guid("00000136-1234-1234-1234-012345678910")
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        });

                        db.SaveChanges(user.Id);
                    }

                    if (db.NotificationTypes.Where(x => !x.IsDeleted).Count() == 0)
                    {
                        db.NotificationTypes.Add(new NotificationType { Name = "مصروفات دورية" });
                        db.NotificationTypes.Add(new NotificationType { Name = "إيرادات دورية" });
                        db.NotificationTypes.Add(new NotificationType { Name = "موعد استحقاق فاتورة بيع" });
                        db.NotificationTypes.Add(new NotificationType { Name = "موعد استحقاق شيك من عميل" });
                        db.NotificationTypes.Add(new NotificationType { Name = "موعد استحقاق دفعة من فاتورة آجل" });
                        db.NotificationTypes.Add(new NotificationType { Name = "موعد استحقاق قسط من فاتورة تقسيط" });
                        db.NotificationTypes.Add(new NotificationType { Name = "موعد استحقاق شيك الى مورد" });
                        db.NotificationTypes.Add(new NotificationType { Name = "مهمة ادارية" });
                        db.NotificationTypes.Add(new NotificationType { Name = "موعد استحقاق فاتورة توريد " });
                        db.SaveChanges(user.Id);
                    }

                }
            }
            catch (Exception ex)
            {

                throw;
            }
            // first init project to create lookups


            return false;
        }
        #endregion
        #region UploadCenter
        public static void ExcuteUploadCenter(string UserID, VTSaleEntities db)
        {
            // Upload center type مجلدات الرئيسية لمركز التحميل

            var uploadCenters = $@"
INSERT [{Schema}].[UploadCenters] ([Id], [FileName], [Name], [ParentId], [IsFolder], [ReferenceGuid], [UploadCenterTypeId], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (N'cb9a06e2-e19f-4af1-9691-a80a74883134', NULL, N'فواتير التوريد', NULL, 1, NULL, NULL, CAST(N'2022-09-05T13:57:28.507' AS DateTime), N'{UserID}', NULL, NULL, 0, NULL, NULL)
INSERT [{Schema}].[UploadCenters] ([Id], [FileName], [Name], [ParentId], [IsFolder], [ReferenceGuid], [UploadCenterTypeId], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (N'bd58fb03-5f2e-4a7c-be30-c6397c25255e', NULL, N'فواتير مرتجع التوريد', NULL, 1, NULL, NULL, CAST(N'2022-09-05T13:57:28.507' AS DateTime), N'{UserID}', NULL, NULL, 0, NULL, NULL)
INSERT [{Schema}].[UploadCenters] ([Id], [FileName], [Name], [ParentId], [IsFolder], [ReferenceGuid], [UploadCenterTypeId], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (N'bbf4a785-0eea-48d5-85ba-7b0be50bf3a2', NULL, N'فواتير البيع', NULL, 1, NULL, NULL, CAST(N'2022-09-05T13:57:28.507' AS DateTime), N'{UserID}', NULL, NULL, 0, NULL, NULL)
INSERT [{Schema}].[UploadCenters] ([Id], [FileName], [Name], [ParentId], [IsFolder], [ReferenceGuid], [UploadCenterTypeId], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (N'a4eb5645-4d8e-4df0-bd9f-031f4a0796a5', NULL, N'فواتير مرتجع البيع', NULL, 1, NULL, NULL, CAST(N'2022-09-05T13:57:28.507' AS DateTime), N'{UserID}', NULL, NULL, 0, NULL, NULL)
INSERT [{Schema}].[UploadCenters] ([Id], [FileName], [Name], [ParentId], [IsFolder], [ReferenceGuid], [UploadCenterTypeId], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (N'0609f08a-55a0-427a-ae51-54f1ab286d1c', NULL, N'شئون الموظفين', NULL, 1, NULL, NULL, CAST(N'2022-09-05T13:57:28.507' AS DateTime), N'{UserID}', NULL, NULL, 0, NULL, NULL)
INSERT [{Schema}].[UploadCenters] ([Id], [FileName], [Name], [ParentId], [IsFolder], [ReferenceGuid], [UploadCenterTypeId], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (N'bb9f6e06-31dc-494e-901c-7aa7e92cf830', NULL, N'الشيكات البنكية', NULL, 1, NULL, NULL, CAST(N'2022-09-05T13:57:28.507' AS DateTime), N'{UserID}', NULL, NULL, 0, NULL, NULL)
INSERT [{Schema}].[UploadCenters] ([Id], [FileName], [Name], [ParentId], [IsFolder], [ReferenceGuid], [UploadCenterTypeId], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (N'b6439a80-2b5e-4e25-a361-9c83bed95d1f', NULL, N'الموردين', NULL, 1, NULL, NULL, CAST(N'2022-09-05T13:57:28.507' AS DateTime), N'{UserID}', NULL, NULL, 0, NULL, NULL)
INSERT [{Schema}].[UploadCenters] ([Id], [FileName], [Name], [ParentId], [IsFolder], [ReferenceGuid], [UploadCenterTypeId], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (N'88254c48-2952-45e0-ab3a-c139a4003819', NULL, N'العملاء', NULL, 1, NULL, NULL, CAST(N'2022-09-05T13:57:28.507' AS DateTime), N'{UserID}', NULL, NULL, 0, NULL, NULL)
INSERT [{Schema}].[UploadCenters] ([Id], [FileName], [Name], [ParentId], [IsFolder], [ReferenceGuid], [UploadCenterTypeId], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (N'72739a22-438b-4848-a4fc-9abb9ba32564', NULL, N'الاقساط', NULL, 1, NULL, NULL, CAST(N'2022-09-05T13:57:28.507' AS DateTime), N'{UserID}', NULL, NULL, 0, NULL, NULL)
INSERT [{Schema}].[UploadCenters] ([Id], [FileName], [Name], [ParentId], [IsFolder], [ReferenceGuid], [UploadCenterTypeId], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (N'72739a22-438b-4848-a4fc-9abb9ba32565', NULL, N'اوامر الانتاج', NULL, 1, NULL, NULL, CAST(N'2022-09-05T13:57:28.507' AS DateTime), N'{UserID}', NULL, NULL, 0, NULL, NULL)
INSERT [{Schema}].[UploadCenters] ([Id], [FileName], [Name], [ParentId], [IsFolder], [ReferenceGuid], [UploadCenterTypeId], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (N'72739a22-438b-4848-a4fc-9abb9ba32566', NULL, N'قيود اليومية', NULL, 1, NULL, NULL, CAST(N'2022-09-05T13:57:28.507' AS DateTime), N'{UserID}', NULL, NULL, 0, NULL, NULL)
";

            if (db.UploadCenterTypes.Where(x => !x.IsDeleted).Count() == 0)
            {
                db.Database.ExecuteSqlCommand(uploadCenters);
            }
        }
        #endregion
        #region Account Tree and General
        public static void ExcuteAccountTreeAndGeneralSettings(string UserID, VTSaleEntities db)
        {
            Console.WriteLine("User ID = " + UserID);
            var accountTreeInsertString = $@"



INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000001-1234-1234-1234-012345678910',1,N'الاصول',NULL,NULL,1,1,1,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)
INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000002-1234-1234-1234-012345678910',11,N'اصول طويلة الأجل',N'00000001-1234-1234-1234-012345678910',1,2,1, 1,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)
INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000003-1234-1234-1234-012345678910',112,N'اصول ثابتة ',N'00000002-1234-1234-1234-012345678910',2,3,1,1,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)
INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000004-1234-1234-1234-012345678910',1121,N'اراضي و مباني و إنشاءات',N'00000003-1234-1234-1234-012345678910',2,4,1,1,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)
INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000005-1234-1234-1234-012345678910',1122,N'آلات و معدات',N'00000003-1234-1234-1234-012345678910',2,4,1,1,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)
INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000006-1234-1234-1234-012345678910',1123,N'اثاث و مكاتب',N'00000003-1234-1234-1234-012345678910',2,4,1,1,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)
INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000007-1234-1234-1234-012345678910',1124,N'اجهزة كهربائية و تكييفات',N'00000003-1234-1234-1234-012345678910',2,4,1,1,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)
INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000008-1234-1234-1234-012345678910',1125,N'حاسبات و طابعات و أجهزة شبكات',N'00000003-1234-1234-1234-012345678910',2,4,1,1,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)
INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000009-1234-1234-1234-012345678910',1126,N'وسائل نقل و انتقال',N'00000003-1234-1234-1234-012345678910',2,4,1,1,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)




INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000010-1234-1234-1234-012345678910',113,N'أصول اخري طويلة الأجل',N'00000002-1234-1234-1234-012345678910',2,3,1,1,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)
INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000011-1234-1234-1234-012345678910',1131,N'نفقات ايرادات مؤجلة',N'00000010-1234-1234-1234-012345678910',2,4,1,1,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)
INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000012-1234-1234-1234-012345678910',1131000001,N'حملات اعلانية',N'00000011-1234-1234-1234-012345678910',3,5,0,1,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)
INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000013-1234-1234-1234-012345678910',1131000002,N'نفقات تأسيس',N'00000011-1234-1234-1234-012345678910',3,5,0,1,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)
INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000014-1234-1234-1234-012345678910',1132,N'مشروعات تحت التنفيذ',N'00000010-1234-1234-1234-012345678910',2,4,1,1,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)



INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000015-1234-1234-1234-012345678910',12,N'الأصول المتداولة',N'00000001-1234-1234-1234-012345678910',1,2,1,1,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)
INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000016-1234-1234-1234-012345678910',121,N'النقدية و البنوك',N'00000015-1234-1234-1234-012345678910',2,3,1,1,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)
INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000017-1234-1234-1234-012345678910',1211,N'النقدية',N'00000016-1234-1234-1234-012345678910',2,4,1,1,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)
INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000018-1234-1234-1234-012345678910',1212,N'البنك',N'00000016-1234-1234-1234-012345678910',2,4,1,1,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)


INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000019-1234-1234-1234-012345678910',122,N'المخزون',N'00000015-1234-1234-1234-012345678910',2,3,1,1,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)
INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000020-1234-1234-1234-012345678910',123,N'العملاء',N'00000015-1234-1234-1234-012345678910',2,3,1,1,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)
INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000021-1234-1234-1234-012345678910',124,N'اوراق القبض(أ.ق)',N'00000015-1234-1234-1234-012345678910',2,3,1,1,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)
INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000022-1234-1234-1234-012345678910',125,N'العهد',N'00000015-1234-1234-1234-012345678910',2,3,1,1,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)

INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000023-1234-1234-1234-012345678910',1251,N'عهد مستديمة',N'00000022-1234-1234-1234-012345678910',2,4,1,1,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)
INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000024-1234-1234-1234-012345678910',1252,N'عهد مؤقتة',N'00000022-1234-1234-1234-012345678910',2,4,1,1,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)

INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000025-1234-1234-1234-012345678910',126,N'الموظفين ',N'00000015-1234-1234-1234-012345678910',2,3,1,1,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)

INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000026-1234-1234-1234-012345678910',127,N'حسابات مدينة أخري ',N'00000015-1234-1234-1234-012345678910',2,3,1,1,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)
INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000027-1234-1234-1234-012345678910',1271,N'حسابات مدينة اخري/ تأمينات لدي الغير ',N'00000026-1234-1234-1234-012345678910',2,4,1,1,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)
INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000028-1234-1234-1234-012345678910',1272,N'حسابات مدينة اخري/ ايرادات مستحقة   ',N'00000026-1234-1234-1234-012345678910',2,4,1,1,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)
INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000029-1234-1234-1234-012345678910',1273,N'حسابات مدينة اخري/ م. مدفوعة مقدماً   ',N'00000026-1234-1234-1234-012345678910',2,4,1,1,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)
INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000030-1234-1234-1234-012345678910',1274,N'جاري فروع',N'00000026-1234-1234-1234-012345678910',2,4,1,1,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)









INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000031-1234-1234-1234-012345678910',2,N'الالتزامات و حقوق الملكية',NULL,NULL,1,1,1,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)

INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000032-1234-1234-1234-012345678910',21,N'التزامات ط.الأجل',N'00000031-1234-1234-1234-012345678910',1,2,1, 1,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)
INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000033-1234-1234-1234-012345678910',211,N'قروض ط.الأجل',N'00000032-1234-1234-1234-012345678910',2,3,1, 1,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)

INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000034-1234-1234-1234-012345678910',22,N'التزامات متداولة',N'00000031-1234-1234-1234-012345678910',2,2,1, 1,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)
INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000035-1234-1234-1234-012345678910',221,N'قروض ق.الاجل',N'00000034-1234-1234-1234-012345678910',2,3,1, 1,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)
INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000036-1234-1234-1234-012345678910',222,N'موردين',N'00000034-1234-1234-1234-012345678910',2,3,1, 1,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)
INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000037-1234-1234-1234-012345678910',223,N'أوراق دفع',N'00000034-1234-1234-1234-012345678910',2,3,1, 1,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)
INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000038-1234-1234-1234-012345678910',224,N'تأمينات لدي الغير ',N'00000034-1234-1234-1234-012345678910',2,3,1, 1,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)
INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000039-1234-1234-1234-012345678910',225,N'هيئة التأمينات الاجتماعية',N'00000034-1234-1234-1234-012345678910',2,3,1, 1,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)
INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000040-1234-1234-1234-012345678910',226,N'ض.القيمة المضافة ',N'00000034-1234-1234-1234-012345678910',2,3,1, 1,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)
INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000041-1234-1234-1234-012345678910',227,N'مصلحة الضرائب العامة ',N'00000034-1234-1234-1234-012345678910',2,3,1, 1,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)
INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000042-1234-1234-1234-012345678910',228,N'هيئة الاستثمار ',N'00000034-1234-1234-1234-012345678910',2,3,1, 1,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)
INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000043-1234-1234-1234-012345678910',229,N'جهاز تنمية المشروعات ',N'00000034-1234-1234-1234-012345678910',2,3,1, 1,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)


INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000044-1234-1234-1234-012345678910',23,N'حسابات دائنة أخري',N'00000031-1234-1234-1234-012345678910',1,2,1, 1,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)

INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000045-1234-1234-1234-012345678910',231,N'مصروفات مستحقة',N'00000044-1234-1234-1234-012345678910',2,3,1, 1,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)
INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000046-1234-1234-1234-012345678910',2311,N'مصروفات مستحقة/اجور',N'00000045-1234-1234-1234-012345678910',2,4,1, 1,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)
INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000047-1234-1234-1234-012345678910',2312,N'مصروفات مستحقة/فوائد',N'00000045-1234-1234-1234-012345678910',2,4,1, 1,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)
INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000048-1234-1234-1234-012345678910',2313,N'مصروفات مستحقة/ايجار',N'00000045-1234-1234-1234-012345678910',2,4,1, 1,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)
INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000049-1234-1234-1234-012345678910',2314,N'مصروفات مستحقة/ضرائب',N'00000045-1234-1234-1234-012345678910',2,4,1, 1,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)
INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000050-1234-1234-1234-012345678910',2315,N'مصروفات مستحقة/تأمينات',N'00000045-1234-1234-1234-012345678910',2,4,1, 1,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)



INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000051-1234-1234-1234-012345678910',232,N'ايرادات محصلة مقدماً ',N'00000044-1234-1234-1234-012345678910',2,3,1, 1,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)
INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000052-1234-1234-1234-012345678910',233,N'جاري رئيسي',N'00000044-1234-1234-1234-012345678910',2,3,1, 1,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)




INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000053-1234-1234-1234-012345678910',24,N'رأس المال و حقوق الملكية  ',N'00000031-1234-1234-1234-012345678910',1,2,1, 1,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)

INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000054-1234-1234-1234-012345678910',241,N' رأس المال المدفوع',N'00000053-1234-1234-1234-012345678910',2,3,1, 1,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)
INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000055-1234-1234-1234-012345678910',242,N' جاري شركاء',N'00000053-1234-1234-1234-012345678910',2,3,1, 1,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)
INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000056-1234-1234-1234-012345678910',243,N'ارباح وخسائر مرحلة',N'00000053-1234-1234-1234-012345678910',2,3,1, 1,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)
INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000057-1234-1234-1234-012345678910',2431,N'ارباح و خسائر مرحلة/فروع',N'00000056-1234-1234-1234-012345678910',2,4,1, 1,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)



INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000058-1234-1234-1234-012345678910',25,N'مجمع الاهلاك',N'00000031-1234-1234-1234-012345678910',1,2,1, 1,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)
INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000059-1234-1234-1234-012345678910',26,N'احتياطيات',N'00000031-1234-1234-1234-012345678910',1,2,1, 1,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)
INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000060-1234-1234-1234-012345678910',27,N'مخصصات بخلاف الاهلاك',N'00000031-1234-1234-1234-012345678910',1,2,1, 1,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)





INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000061-1234-1234-1234-012345678910',3,N'التكاليف و المصروفات',NULL,NULL,1,1,2,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)
INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000062-1234-1234-1234-012345678910',31,N'تكاليف انتاج',N'00000061-1234-1234-1234-012345678910',1,2,1, 2,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)

INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000063-1234-1234-1234-012345678910',311,N'تكلفة مبيعات ',N'00000062-1234-1234-1234-012345678910',2,3,1, 2,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)

INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000064-1234-1234-1234-012345678910',3111,N'تكلفة مبيعات/انتاج',N'00000063-1234-1234-1234-012345678910',2,4,1, 2,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)
INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000065-1234-1234-1234-012345678910',3112,N'تكلفة مبيعات/انحرافات تشغيل',N'00000063-1234-1234-1234-012345678910',2,4,1, 2,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)
INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000066-1234-1234-1234-012345678910',3113,N'تكلفة مبيعات/اختبارات انتاج',N'00000063-1234-1234-1234-012345678910',2,4,1, 2,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)
INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000067-1234-1234-1234-012345678910',3114,N'تكلفة مبيعات/اهلاك بضاعة',N'00000063-1234-1234-1234-012345678910',2,4,1, 2,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)
INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000068-1234-1234-1234-012345678910',3115,N'تكلفة مبيعات/تعبئة وتغليف',N'00000063-1234-1234-1234-012345678910',2,4,1, 2,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)



INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000069-1234-1234-1234-012345678910',312,N'مشتريات  ',N'00000062-1234-1234-1234-012345678910',2,3,1, 2,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)


INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000070-1234-1234-1234-012345678910',3121,N'مشتريات نقدية',N'00000069-1234-1234-1234-012345678910',2,4,1, 2,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)
INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000071-1234-1234-1234-012345678910',3122,N'مشتريات آجلة',N'00000069-1234-1234-1234-012345678910',2,4,1, 2,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)
INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000072-1234-1234-1234-012345678910',3123,N'خصم مسموح به',N'00000069-1234-1234-1234-012345678910',2,4,1, 2,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)
INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000073-1234-1234-1234-012345678910',3124,N'نقل مشتريات',N'00000069-1234-1234-1234-012345678910',2,4,1, 2,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)
INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000074-1234-1234-1234-012345678910',3125,N'عمولات و تخليص نشتريات',N'00000069-1234-1234-1234-012345678910',2,4,1, 2,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)
INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000075-1234-1234-1234-012345678910',3126,N'مردودات مبيعات',N'00000069-1234-1234-1234-012345678910',2,4,1, 2,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)


INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000076-1234-1234-1234-012345678910',313,N'اجور تشغيل و انتاج',N'00000062-1234-1234-1234-012345678910',2,3,1, 2,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)


INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000077-1234-1234-1234-012345678910',3131,N'مرتبات تشغيل وانتاج ',N'00000076-1234-1234-1234-012345678910',2,4,1, 2,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)
INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000078-1234-1234-1234-012345678910',3132,N'فروق مرتبات ',N'00000076-1234-1234-1234-012345678910',2,4,1, 2,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)
INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000079-1234-1234-1234-012345678910',3133,N'بدلات ',N'00000076-1234-1234-1234-012345678910',2,4,1, 2,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)
INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000080-1234-1234-1234-012345678910',3134,N'حوافز و مكافأت ',N'00000076-1234-1234-1234-012345678910',2,4,1, 2,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)
INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000081-1234-1234-1234-012345678910',3135,N'مزايا عينية ',N'00000076-1234-1234-1234-012345678910',2,4,1, 2,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)

INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000082-1234-1234-1234-012345678910',3135000001,N'مزايا عينية/ انتقالات ',N'00000081-1234-1234-1234-012345678910',3,5,0,2,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)
INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000083-1234-1234-1234-012345678910',3135000002,N'مزايا عينية/ ملابس عاملين ',N'00000081-1234-1234-1234-012345678910',3,5,0,2,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)
INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000084-1234-1234-1234-012345678910',3135000003,N'مزايا عينية/ علاج ',N'00000081-1234-1234-1234-012345678910',3,5,0,2,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)
INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000085-1234-1234-1234-012345678910',3135000004,N'مزايا عينية/ وجبات ',N'00000081-1234-1234-1234-012345678910',3,5,0,2,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)
INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000086-1234-1234-1234-012345678910',3135000005,N'مزايا عينية/ سكن ',N'00000081-1234-1234-1234-012345678910',3,5,0,2,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)



INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000087-1234-1234-1234-012345678910',314,N'وقود و قوي محركات للانتاج',N'00000062-1234-1234-1234-012345678910',2,3,1, 2,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)
INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000088-1234-1234-1234-012345678910',3141,N'كهرباء',N'00000087-1234-1234-1234-012345678910',2,4,1, 2,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)
INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000089-1234-1234-1234-012345678910',3142,N'غاز',N'00000087-1234-1234-1234-012345678910',2,4,1, 2,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)


INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000090-1234-1234-1234-012345678910',315,N'مستلزمات انتاج و سلعية',N'00000062-1234-1234-1234-012345678910',2,3,1, 2,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)
INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000091-1234-1234-1234-012345678910',3151,N'مستلزمات انتاج و سلعية//',N'00000090-1234-1234-1234-012345678910',2,4,1, 2,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)

INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000092-1234-1234-1234-012345678910',316,N'مصروفات الصيانة',N'00000062-1234-1234-1234-012345678910',2,3,1, 2,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)
INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000093-1234-1234-1234-012345678910',3161,N'مصروفات الصيانة / قطع غيار دورية',N'00000092-1234-1234-1234-012345678910',2,4,1, 2,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)
INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000094-1234-1234-1234-012345678910',3162,N'مصروفات الصيانة / زيوت و شحوم  ',N'00000092-1234-1234-1234-012345678910',2,4,1, 2,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)


INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000095-1234-1234-1234-012345678910',317,N'مصروفات الاهلاك',N'00000062-1234-1234-1234-012345678910',2,3,1, 2,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)


INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000096-1234-1234-1234-012345678910',32,N'مصروفات بيع و تسويق ',N'00000061-1234-1234-1234-012345678910',1,2,1, 2,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)
INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000097-1234-1234-1234-012345678910',321,N'دعاية و اعلان  ',N'00000096-1234-1234-1234-012345678910',2,3,1, 2,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)
INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000098-1234-1234-1234-012345678910',322,N'مصروفات تسويق  ',N'00000096-1234-1234-1234-012345678910',2,3,1, 2,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)
INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000100-1234-1234-1234-012345678910',323,N'عمولات بيع  ',N'00000096-1234-1234-1234-012345678910',2,3,1, 2,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)
INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000101-1234-1234-1234-012345678910',324,N'مطبوعات اعلانية  ',N'00000096-1234-1234-1234-012345678910',2,3,1, 2,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)



INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000102-1234-1234-1234-012345678910',33,N'مصروفات تمويلية',N'00000061-1234-1234-1234-012345678910',1,2,1, 2,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)
INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000103-1234-1234-1234-012345678910',331,N'فوائد و قروض',N'00000102-1234-1234-1234-012345678910',2,3,1, 2,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)



INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000104-1234-1234-1234-012345678910',34,N'مصروفات عمومية و ادارية',N'00000061-1234-1234-1234-012345678910',1,2,1, 2,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)

INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000105-1234-1234-1234-012345678910',341,N'اجور و مرتبات',N'00000104-1234-1234-1234-012345678910',2,3,1, 2,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)
INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000106-1234-1234-1234-012345678910',342,N'بدلات',N'00000104-1234-1234-1234-012345678910',2,3,1, 2,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)
INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000107-1234-1234-1234-012345678910',343,N'حوافز و مكافآت',N'00000104-1234-1234-1234-012345678910',2,3,1, 2,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)
INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000108-1234-1234-1234-012345678910',344,N'مزايا عينية',N'00000104-1234-1234-1234-012345678910',2,3,1, 2,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)


INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000109-1234-1234-1234-012345678910',3441,N'مزايا عينية/ انتقالات ',N'00000108-1234-1234-1234-012345678910',3,4,0,2,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)
INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000110-1234-1234-1234-012345678910',3442,N'مزايا عينية/ ملابس عاملين ',N'00000108-1234-1234-1234-012345678910',3,4,0,2,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)
INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000111-1234-1234-1234-012345678910',3443,N'مزايا عينية/ علاج ',N'00000108-1234-1234-1234-012345678910',3,4,0,2,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)
INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000112-1234-1234-1234-012345678910',3444,N'مزايا عينية/ وجبات ',N'00000108-1234-1234-1234-012345678910',3,4,0,2,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)
INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000113-1234-1234-1234-012345678910',3445,N'مزايا عينية/ سكن ',N'00000108-1234-1234-1234-012345678910',3,4,0,2,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)


INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000114-1234-1234-1234-012345678910',345,N'كهرباء ',N'00000104-1234-1234-1234-012345678910',2,3,1, 2,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)
INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000115-1234-1234-1234-012345678910',346,N'مياه ',N'00000104-1234-1234-1234-012345678910',2,3,1, 2,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)
INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000116-1234-1234-1234-012345678910',347,N'غاز ',N'00000104-1234-1234-1234-012345678910',2,3,1, 2,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)
INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000117-1234-1234-1234-012345678910',348,N'تأمينات اجتماعية ',N'00000104-1234-1234-1234-012345678910',2,3,1, 2,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)
INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000118-1234-1234-1234-012345678910',349,N'ادوات مكتبية و مطبوعات ',N'00000104-1234-1234-1234-012345678910',2,3,1, 2,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)


INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000119-1234-1234-1234-012345678910',35,N'ارباح و خسائر ',N'00000061-1234-1234-1234-012345678910',1,2,1, 2,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)


INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000120-1234-1234-1234-012345678910',4,N'الايرادات',NULL,NULL,1,1,2,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)

INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000121-1234-1234-1234-012345678910',41,N'مبيعات منتجات ',N'00000120-1234-1234-1234-012345678910',1,2,1, 2,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)
INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000122-1234-1234-1234-012345678910',411,N'مبيعات نقدية',N'00000121-1234-1234-1234-012345678910',2,3,1, 2,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)
INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000123-1234-1234-1234-012345678910',412,N'مبيعات آجلة',N'00000121-1234-1234-1234-012345678910',2,3,1, 2,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)
INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000124-1234-1234-1234-012345678910',413,N'مردودات مشتريات',N'00000121-1234-1234-1234-012345678910',2,3,1, 2,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)
INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000125-1234-1234-1234-012345678910',414,N'خصم مكتسب',N'00000121-1234-1234-1234-012345678910',2,3,1, 2,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)

INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000126-1234-1234-1234-012345678910',42,N'مبيعات خدمات ',N'00000120-1234-1234-1234-012345678910',1,2,1, 2,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)

INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000127-1234-1234-1234-012345678910',421,N'مبيعات نقدية',N'00000126-1234-1234-1234-012345678910',2,3,1, 2,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)
INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000128-1234-1234-1234-012345678910',422,N'مبيعات آجلة',N'00000126-1234-1234-1234-012345678910',2,3,1, 2,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)
INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000129-1234-1234-1234-012345678910',423,N'مردودات مشتريات',N'00000126-1234-1234-1234-012345678910',2,3,1, 2,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)
INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000130-1234-1234-1234-012345678910',424,N'خصم مكتسب',N'00000126-1234-1234-1234-012345678910',2,3,1, 2,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)

INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000131-1234-1234-1234-012345678910',43,N'ايرادات جزاءات  ',N'00000120-1234-1234-1234-012345678910',1,2,1, 2,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)
INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000132-1234-1234-1234-012345678910',44,N'ايرادات اخري  ',N'00000120-1234-1234-1234-012345678910',1,2,1, 2,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)
INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000133-1234-1234-1234-012345678910',1211000001,N'الخزينة الرئيسية',N'00000017-1234-1234-1234-012345678910',3,4,0, 1,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)
INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000134-1234-1234-1234-012345678910',1212000001,N'الحساب الرئيسيى للشركة ',N'00000018-1234-1234-1234-012345678910',3,4,0, 1,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)
INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000135-1234-1234-1234-012345678910',1212000002,N'فودافون كاش',N'00000018-1234-1234-1234-012345678910',3,4,0, 1,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)
INSERT [{Schema}].[AccountsTrees] ([Id], [AccountNumber], [AccountName], [ParentId], [TypeId], [AccountLevel], [SelectedTree],[OrientationTypes_Id], [DeletedOn], [CreatedBy], [CreatedOn],[ModifiedBy],[ModifiedOn],[IsDeleted]) VALUES (N'00000136-1234-1234-1234-012345678910',1212000003,N'بطاقة فيزا',N'00000018-1234-1234-1234-012345678910',3,4,0, 1,NULL,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),NULL,NULL,0)

";

            var genralSettings = $@"SET IDENTITY_INSERT [{Schema}].[GeneralSettings] ON;




INSERT [{Schema}].[GeneralSettings] ([Id], [SName], [SValue], [SType], [CreatedBy], [CreatedOn], [IsDeleted]) VALUES (1,N'حساب المشتريات',N'00000069-1234-1234-1234-012345678910',1,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),0)
INSERT [{Schema}].[GeneralSettings] ([Id], [SName], [SValue], [SType], [CreatedBy], [CreatedOn], [IsDeleted]) VALUES (2,N'حساب مبيعات ',N'00000121-1234-1234-1234-012345678910',1,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),0)
INSERT [{Schema}].[GeneralSettings] ([Id], [SName], [SValue], [SType], [CreatedBy], [CreatedOn], [IsDeleted]) VALUES (3,N'حساب الموردين',N'00000036-1234-1234-1234-012345678910',1,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),0)
INSERT [{Schema}].[GeneralSettings] ([Id], [SName], [SValue], [SType], [CreatedBy], [CreatedOn], [IsDeleted]) VALUES (4,N'حساب العملاء',N'00000020-1234-1234-1234-012345678910',1,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),0)
INSERT [{Schema}].[GeneralSettings] ([Id], [SName], [SValue], [SType], [CreatedBy], [CreatedOn], [IsDeleted]) VALUES (5,N'حساب مردودات المبيعات',N'00000075-1234-1234-1234-012345678910',1,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),0)
INSERT [{Schema}].[GeneralSettings] ([Id], [SName], [SValue], [SType], [CreatedBy], [CreatedOn], [IsDeleted]) VALUES (6,N'حساب مردودات المشتريات',N'00000128-1234-1234-1234-012345678910',1,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),0)
INSERT [{Schema}].[GeneralSettings] ([Id], [SName], [SValue], [SType], [CreatedBy], [CreatedOn], [IsDeleted]) VALUES (7,N'حساب الضريبة المضافة',N'00000040-1234-1234-1234-012345678910',1,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),0)
INSERT [{Schema}].[GeneralSettings] ([Id], [SName], [SValue], [SType], [CreatedBy], [CreatedOn], [IsDeleted]) VALUES (8,N'حساب الخصم المسموح به',N'00000072-1234-1234-1234-012345678910',1,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),0)
INSERT [{Schema}].[GeneralSettings] ([Id], [SName], [SValue], [SType], [CreatedBy], [CreatedOn], [IsDeleted]) VALUES (9,N'حساب الخصم المكتسب',N'00000124-1234-1234-1234-012345678910',1,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),0)
INSERT [{Schema}].[GeneralSettings] ([Id], [SName], [SValue], [SType], [CreatedBy], [CreatedOn], [IsDeleted]) VALUES (10,N'حساب الخزينة',N'00000017-1234-1234-1234-012345678910',1,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),0)
INSERT [{Schema}].[GeneralSettings] ([Id], [SName], [SValue], [SType], [CreatedBy], [CreatedOn], [IsDeleted]) VALUES (11,N'حساب البنك',N'00000018-1234-1234-1234-012345678910',1,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),0)
INSERT [{Schema}].[GeneralSettings] ([Id], [SName], [SValue], [SType], [CreatedBy], [CreatedOn], [IsDeleted]) VALUES (12,N'حساب رأس المال',N'00000054-1234-1234-1234-012345678910',1,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),0)
INSERT [{Schema}].[GeneralSettings] ([Id], [SName], [SValue], [SType], [CreatedBy], [CreatedOn], [IsDeleted]) VALUES (13,N'حساب المخزون',N'00000019-1234-1234-1234-012345678910',1,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),0)
INSERT [{Schema}].[GeneralSettings] ([Id], [SName], [SValue], [SType], [CreatedBy], [CreatedOn], [IsDeleted]) VALUES (14,N'حساب شيكات تحت التحصيل (اوراق قبض)',N'00000021-1234-1234-1234-012345678910',1,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),0)
INSERT [{Schema}].[GeneralSettings] ([Id], [SName], [SValue], [SType], [CreatedBy], [CreatedOn], [IsDeleted]) VALUES (15,N'اسم الجهة',N'في تي اس للحلول البرمجية ',2,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),0)
INSERT [{Schema}].[GeneralSettings] ([Id], [SName], [SValue], [SType], [CreatedBy], [CreatedOn], [IsDeleted]) VALUES (16,N'الاسم المختصر للجهة',NULL,2,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),0)
INSERT [{Schema}].[GeneralSettings] ([Id], [SName], [SValue], [SType], [CreatedBy], [CreatedOn], [IsDeleted]) VALUES (17,N'رقم التلفون 1',NULL,2,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),0)
INSERT [{Schema}].[GeneralSettings] ([Id], [SName], [SValue], [SType], [CreatedBy], [CreatedOn], [IsDeleted]) VALUES (18,N'رقم التلفون 2',NULL,2,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),0)
INSERT [{Schema}].[GeneralSettings] ([Id], [SName], [SValue], [SType], [CreatedBy], [CreatedOn], [IsDeleted]) VALUES (19,N'منطقة الجهة',NULL,2,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),0)
INSERT [{Schema}].[GeneralSettings] ([Id], [SName], [SValue], [SType], [CreatedBy], [CreatedOn], [IsDeleted]) VALUES (20,N'العنوان',NULL,2,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),0)
INSERT [{Schema}].[GeneralSettings] ([Id], [SName], [SValue], [SType], [CreatedBy], [CreatedOn], [IsDeleted]) VALUES (21,N'رقم السجل التجارى',NULL,2,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),0)
INSERT [{Schema}].[GeneralSettings] ([Id], [SName], [SValue], [SType], [CreatedBy], [CreatedOn], [IsDeleted]) VALUES (22,N'رقم السجل الضريبى',NULL,2,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),0)
INSERT [{Schema}].[GeneralSettings] ([Id], [SName], [SValue], [SType], [CreatedBy], [CreatedOn], [IsDeleted]) VALUES (23,N'الباركود يحتوى على حروف',N'False',3,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),0)
INSERT [{Schema}].[GeneralSettings] ([Id], [SName], [SValue], [SType], [CreatedBy], [CreatedOn], [IsDeleted]) VALUES (24,N'الباركود يحتوى على ارقام',N'False',3,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),0)
INSERT [{Schema}].[GeneralSettings] ([Id], [SName], [SValue], [SType], [CreatedBy], [CreatedOn], [IsDeleted]) VALUES (25,N'مخزن التصنيع الافتراضى',NULL,4,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),0)
INSERT [{Schema}].[GeneralSettings] ([Id], [SName], [SValue], [SType], [CreatedBy], [CreatedOn], [IsDeleted]) VALUES (26,N'المخزن الافتراضى لفواتير البيع والشراء',NULL,4,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),0)
INSERT [{Schema}].[GeneralSettings] ([Id], [SName], [SValue], [SType], [CreatedBy], [CreatedOn], [IsDeleted]) VALUES (27,N'مخزن تحت التصنيع الافتراضى',NULL,4,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),0)
INSERT [{Schema}].[GeneralSettings] ([Id], [SName], [SValue], [SType], [CreatedBy], [CreatedOn], [IsDeleted]) VALUES (28,N'تاريخ بداية السنة المالية',N'2023-01-01',5,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),0)
INSERT [{Schema}].[GeneralSettings] ([Id], [SName], [SValue], [SType], [CreatedBy], [CreatedOn], [IsDeleted]) VALUES (29,N'تاريخ نهاية السنة المالية',N'2023-12-31',5,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),0)
INSERT [{Schema}].[GeneralSettings] ([Id], [SName], [SValue], [SType], [CreatedBy], [CreatedOn], [IsDeleted]) VALUES (30,N'مجلد فواتير التوريد',N'CB9A06E2-E19F-4AF1-9691-A80A74883134',6,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),0)
INSERT [{Schema}].[GeneralSettings] ([Id], [SName], [SValue], [SType], [CreatedBy], [CreatedOn], [IsDeleted]) VALUES (31,N'مجلد فواتير مرتجع التوريد',N'BD58FB03-5F2E-4A7C-BE30-C6397C25255E',6,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),0)
INSERT [{Schema}].[GeneralSettings] ([Id], [SName], [SValue], [SType], [CreatedBy], [CreatedOn], [IsDeleted]) VALUES (32,N'مجلد فواتير البيع',N'BBF4A785-0EEA-48D5-85BA-7B0BE50BF3A2',6,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),0)
INSERT [{Schema}].[GeneralSettings] ([Id], [SName], [SValue], [SType], [CreatedBy], [CreatedOn], [IsDeleted]) VALUES (33,N'مجلد فواتير مرتجع البيع',N'A4EB5645-4D8E-4DF0-BD9F-031F4A0796A5',6,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),0)
INSERT [{Schema}].[GeneralSettings] ([Id], [SName], [SValue], [SType], [CreatedBy], [CreatedOn], [IsDeleted]) VALUES (34,N'مجلد الموظفين',N'0609F08A-55A0-427A-AE51-54F1AB286D1C',6,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),0)
INSERT [{Schema}].[GeneralSettings] ([Id], [SName], [SValue], [SType], [CreatedBy], [CreatedOn], [IsDeleted]) VALUES (35,N'مجلد شيك بنكى',N'BB9F6E06-31DC-494E-901C-7AA7E92CF830',6,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),0)
INSERT [{Schema}].[GeneralSettings] ([Id], [SName], [SValue], [SType], [CreatedBy], [CreatedOn], [IsDeleted]) VALUES (36,N'احتساب تكلفة المنتج',N'1',7,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),0)
INSERT [{Schema}].[GeneralSettings] ([Id], [SName], [SValue], [SType], [CreatedBy], [CreatedOn], [IsDeleted]) VALUES (37,N'حساب الرواتب والاجور',N'00000077-1234-1234-1234-012345678910',1,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),0)
INSERT [{Schema}].[GeneralSettings] ([Id], [SName], [SValue], [SType], [CreatedBy], [CreatedOn], [IsDeleted]) VALUES (38,N'حساب ذمم الموظفين',N'00000046-1234-1234-1234-012345678910',1,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),0)
INSERT [{Schema}].[GeneralSettings] ([Id], [SName], [SValue], [SType], [CreatedBy], [CreatedOn], [IsDeleted]) VALUES (39,N'حساب الايرادات المتنوعه -استقطاعات الموظفين',N'00000131-1234-1234-1234-012345678910',1,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),0)
INSERT [{Schema}].[GeneralSettings] ([Id], [SName], [SValue], [SType], [CreatedBy], [CreatedOn], [IsDeleted]) VALUES (40,N'عٌهد الموظفين',N'00000024-1234-1234-1234-012345678910',1,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),0)
INSERT [{Schema}].[GeneralSettings] ([Id], [SName], [SValue], [SType], [CreatedBy], [CreatedOn], [IsDeleted]) VALUES (41,N'مخزن الصيانة',NULL,4,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),0)
INSERT [{Schema}].[GeneralSettings] ([Id], [SName], [SValue], [SType], [CreatedBy], [CreatedOn], [IsDeleted]) VALUES (42,N'مخزن توالف الصيانة',NULL,4,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),0)
INSERT [{Schema}].[GeneralSettings] ([Id], [SName], [SValue], [SType], [CreatedBy], [CreatedOn], [IsDeleted]) VALUES (43,N'مسمى الاسكيما',N'{Schema}',2,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),0)
INSERT [{Schema}].[GeneralSettings] ([Id], [SName], [SValue], [SType], [CreatedBy], [CreatedOn], [IsDeleted]) VALUES (44,N'رقم الحماية الافلاين',NULL,2,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),0)
INSERT [{Schema}].[GeneralSettings] ([Id], [SName], [SValue], [SType], [CreatedBy], [CreatedOn], [IsDeleted]) VALUES (45,N'حساب مخصصات بخلاف الاهلاك   ',N'00000060-1234-1234-1234-012345678910',1,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),0)
INSERT [{Schema}].[GeneralSettings] ([Id], [SName], [SValue], [SType], [CreatedBy], [CreatedOn], [IsDeleted]) VALUES (46,N'قبول اضافة اصناف بدون رصيد فى فواتير البيع',N'0',7,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),0)
INSERT [{Schema}].[GeneralSettings] ([Id], [SName], [SValue], [SType], [CreatedBy], [CreatedOn], [IsDeleted]) VALUES (47,N'الاعتماد المباشر بعد حفظ فواتير البيع والتوريد',N'1',7,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),0)
INSERT [{Schema}].[GeneralSettings] ([Id], [SName], [SValue], [SType], [CreatedBy], [CreatedOn], [IsDeleted]) VALUES (48,N'اظهار تكلفة الصنف فى شاشة البيع',N'1',7,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),0)
INSERT [{Schema}].[GeneralSettings] ([Id], [SName], [SValue], [SType], [CreatedBy], [CreatedOn], [IsDeleted]) VALUES (49,N'المدة المسموح بها فى سداد قسط',N'5',7,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),0)
INSERT [{Schema}].[GeneralSettings] ([Id], [SName], [SValue], [SType], [CreatedBy], [CreatedOn], [IsDeleted]) VALUES (50,N'مجلد الموردين',N'B6439A80-2B5E-4E25-A361-9C83BED95D1F',6,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),0)
INSERT [{Schema}].[GeneralSettings] ([Id], [SName], [SValue], [SType], [CreatedBy], [CreatedOn], [IsDeleted]) VALUES (51,N'مجلد العملاء',N'88254C48-2952-45E0-AB3A-C139A4003819',6,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),0)
INSERT [{Schema}].[GeneralSettings] ([Id], [SName], [SValue], [SType], [CreatedBy], [CreatedOn], [IsDeleted]) VALUES (52,N'مجلد الاقساط',N'72739A22-438B-4848-A4FC-9ABB9BA32564',6,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),0)
INSERT [{Schema}].[GeneralSettings] ([Id], [SName], [SValue], [SType], [CreatedBy], [CreatedOn], [IsDeleted]) VALUES (53,N'حساب شيكات تحت التحصيل برسم السداد (اوراق دفع)',N'00000037-1234-1234-1234-012345678910',1,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),0)
INSERT [{Schema}].[GeneralSettings] ([Id], [SName], [SValue], [SType], [CreatedBy], [CreatedOn], [IsDeleted]) VALUES (54,N'قبول/رفض صرف نقدية بدون رصيد',N'1',7,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),0)
INSERT [{Schema}].[GeneralSettings] ([Id], [SName], [SValue], [SType], [CreatedBy], [CreatedOn], [IsDeleted]) VALUES (55,N'السماح بالبيع عند تخطى الحد الائتمانى للخطر',N'1',7,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),0)
INSERT [{Schema}].[GeneralSettings] ([Id], [SName], [SValue], [SType], [CreatedBy], [CreatedOn], [IsDeleted]) VALUES (56,N'نوع الجرد',N'1',7,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),0)
INSERT [{Schema}].[GeneralSettings] ([Id], [SName], [SValue], [SType], [CreatedBy], [CreatedOn], [IsDeleted]) VALUES (57,N'لوجو المؤسسة',NULL,2,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),0)
INSERT [{Schema}].[GeneralSettings] ([Id], [SName], [SValue], [SType], [CreatedBy], [CreatedOn], [IsDeleted]) VALUES (58,N'سطر الطباعه الاول اعلى',NULL,2,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),0)
INSERT [{Schema}].[GeneralSettings] ([Id], [SName], [SValue], [SType], [CreatedBy], [CreatedOn], [IsDeleted]) VALUES (59,N'سطر الطباعه الثانى اعلى',NULL,2,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),0)
INSERT [{Schema}].[GeneralSettings] ([Id], [SName], [SValue], [SType], [CreatedBy], [CreatedOn], [IsDeleted]) VALUES (60,N'سطر الطباعه الثالث اعلى',NULL,2,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),0)
INSERT [{Schema}].[GeneralSettings] ([Id], [SName], [SValue], [SType], [CreatedBy], [CreatedOn], [IsDeleted]) VALUES (61,N'سطر الطباعه الرابع اسفل',NULL,2,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),0)
INSERT [{Schema}].[GeneralSettings] ([Id], [SName], [SValue], [SType], [CreatedBy], [CreatedOn], [IsDeleted]) VALUES (62,N'سطر الطباعه الخامس اسفل',NULL,2,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),0)
INSERT [{Schema}].[GeneralSettings] ([Id], [SName], [SValue], [SType], [CreatedBy], [CreatedOn], [IsDeleted]) VALUES (63,N'حساب تكلفة بضاعه مباعه',NULL,1,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),0)
INSERT [{Schema}].[GeneralSettings] ([Id], [SName], [SValue], [SType], [CreatedBy], [CreatedOn], [IsDeleted]) VALUES (64,N'نسبة ضريبة القيمة المضافة',N'14',7,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),0)
INSERT [{Schema}].[GeneralSettings] ([Id], [SName], [SValue], [SType], [CreatedBy], [CreatedOn], [IsDeleted]) VALUES (65,N'نسبة ضريبة ارباح تجارية',N'1',7,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),0)
INSERT [{Schema}].[GeneralSettings] ([Id], [SName], [SValue], [SType], [CreatedBy], [CreatedOn], [IsDeleted]) VALUES (66,N'مجلد اوامر الانتاج',N'72739a22-438b-4848-a4fc-9abb9ba32565',6,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),0)
INSERT [{Schema}].[GeneralSettings] ([Id], [SName], [SValue], [SType], [CreatedBy], [CreatedOn], [IsDeleted]) VALUES (67,N'مجلد قيود اليومية',N'72739a22-438b-4848-a4fc-9abb9ba32566',6,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),0)
INSERT [{Schema}].[GeneralSettings] ([Id], [SName], [SValue], [SType], [CreatedBy], [CreatedOn], [IsDeleted]) VALUES (68,N'تاريخ بداية البحث',N'2023-01-01',5,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),0)
INSERT [{Schema}].[GeneralSettings] ([Id], [SName], [SValue], [SType], [CreatedBy], [CreatedOn], [IsDeleted]) VALUES (69,N'تاريخ نهاية البحث',N'2023-12-31',5,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),0)
INSERT [{Schema}].[GeneralSettings] ([Id], [SName], [SValue], [SType], [CreatedBy], [CreatedOn], [IsDeleted]) VALUES (70,N'ملاحظه عروض الاسعار بالعربية',N'',2,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),0)
INSERT [{Schema}].[GeneralSettings] ([Id], [SName], [SValue], [SType], [CreatedBy], [CreatedOn], [IsDeleted]) VALUES (71,N'ملاحظه عروض الاسعار بالانجليزية',N'',2,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),0)
INSERT [{Schema}].[GeneralSettings] ([Id], [SName], [SValue], [SType], [CreatedBy], [CreatedOn], [IsDeleted]) VALUES (72,N'حساب انحراف التشغيل فى حالة جرد المخزن',N'00000119-1234-1234-1234-012345678910',1,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),0)
INSERT [{Schema}].[GeneralSettings] ([Id], [SName], [SValue], [SType], [CreatedBy], [CreatedOn], [IsDeleted]) VALUES (73,N'سعر البيع يقبل صفر (الهدايا)',N'1',7,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),0)
INSERT [{Schema}].[GeneralSettings] ([Id], [SName], [SValue], [SType], [CreatedBy], [CreatedOn], [IsDeleted]) VALUES (74,N'السماح ببيع الصنف فى حالة سعر البيع اقل من تكلفته',N'1',7,N'{UserID}',CAST(N'2022-07-25T18:40:07.420' AS DateTime),0)





SET IDENTITY_INSERT [{Schema}].[GeneralSettings] OFF;";

            if (db.AccountsTrees.Where(x => !x.IsDeleted).Count() == 0)
            {
                Console.WriteLine("Adding account trees.....");

                db.Database.ExecuteSqlCommand(accountTreeInsertString);

                Console.WriteLine("Adding account trees completed!");
            }
            if (db.GeneralSettings.Where(x => !x.IsDeleted).Count() == 0)
            {
                Console.WriteLine("Adding general settings......");

                db.Database.ExecuteSqlCommand(genralSettings);

                Console.WriteLine("Adding general settings Completed!");
            }
        }
        #endregion

        #region Nationaties
        public static void ExcuteNationaties(string UserID, VTSaleEntities db)
        {
            var query = $"SET IDENTITY_INSERT [{Schema}].[Nationalities] ON ;INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (1, N'أندوري', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (2, N'إماراتي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (3, N'أفغانستاني', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (4, N'بربودي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (5, N'أنغويلي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (6, N'ألباني', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (7, N'أرميني', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (8, N'هولندي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (9, N'أنقولي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (10, N'أنتاركتيكي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (11, N'أرجنتيني', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (12, N'أمريكي سامواني', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (13, N'نمساوي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (14, N'أسترالي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (15, N'أوروبهيني', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (16, N'آلاندي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (17, N'أذربيجاني', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (18, N'بوسني/هرسكي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (19, N'بربادوسي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (20, N'بنغلاديشي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (21, N'بلجيكي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (22, N'بوركيني', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (23, N'بلغاري', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (24, N'بحريني', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (25, N'بورونيدي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (26, N'بنيني', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (27, N'سان بارتيلمي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (28, N'برمودي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (29, N'بروني', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (30, N'بوليفي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (31, N'برازيلي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (32, N'باهاميسي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (33, N'بوتاني', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (34, N'بوفيهي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (35, N'بوتسواني', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (36, N'روسي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (37, N'بيليزي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (38, N'كندي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (39, N'جزر كوكوس', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (40, N'أفريقي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (41, N'كونغي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (42, N'سويسري', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (43, N'ساحل العاج', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (44, N'جزر كوك', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (45, N'شيلي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (46, N'كاميروني', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (47, N'صيني', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (48, N'كولومبي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (49, N'كوستاريكي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (50, N'كوبي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (51, N'الرأس الأخضر', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (52, N'كوراساوي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (53, N'جزيرة عيد الميلاد', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (54, N'قبرصي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (55, N'تشيكي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (56, N'ألماني', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (57, N'جيبوتي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (58, N'دنماركي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (59, N'دومينيكي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (60, N'دومينيكي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (61, N'جزائري', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (62, N'إكوادوري', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (63, N'استوني', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (64, N'مصري', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (65, N'صحراوي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (66, N'إريتيري', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (67, N'إسباني', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (68, N'أثيوبي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (69, N'فنلندي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (70, N'فيجي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (71, N'فوكلاندي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (72, N'مايكرونيزيي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (73, N'جزر فارو', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (74, N'فرنسي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (75, N'غابوني', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (76, N'بريطاني', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (77, N'غرينادي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (78, N'جيورجي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (79, N'غويانا الفرنسية', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (80, N'غيرنزي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (81, N'غاني', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (82, N'جبل طارق', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (83, N'جرينلاندي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (84, N'غامبي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (85, N'غيني', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (86, N'جزر جوادلوب', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (87, N'غيني', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (88, N'يوناني', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (89, N'لمنطقة القطبية الجنوبية', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (90, N'غواتيمالي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (91, N'جوامي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (92, N'غيني', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (93, N'غياني', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (94, N'هونغ كونغي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (95, N'جزيرة هيرد وجزر ماكدونالد', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (96, N'هندوراسي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (97, N'كوراتي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (98, N'هايتي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (99, N'مجري', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (100, N'أندونيسيي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (101, N'إيرلندي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (102, N'إسرائيلي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (103, N'ماني', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (104, N'هندي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (105, N'إقليم المحيط الهندي البريطاني', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (106, N'عراقي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (107, N'إيراني', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (108, N'آيسلندي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (109, N'إيطالي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (110, N'جيرزي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (111, N'جمايكي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (112, N'أردني', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (113, N'ياباني', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (114, N'كيني', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (115, N'قيرغيزستاني', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (116, N'كمبودي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (117, N'كيريباتي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (118, N'جزر القمر', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (119, N'سانت كيتس ونيفس', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (120, N'كوري', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (121, N'كوري', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (122, N'كويتي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (123, N'كايماني', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (124, N'كازاخستاني', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (125, N'لاوسي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (126, N'لبناني', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (127, N'سان بيير وميكلوني', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (128, N'ليختنشتيني', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (129, N'سريلانكي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (130, N'ليبيري', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (131, N'ليوسيتي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (132, N'لتوانيي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (133, N'لوكسمبورغي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (134, N'لاتيفي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (135, N'ليبي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (136, N'مغربي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (137, N'مونيكي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (138, N'مولديفي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (139, N'الجبل الأسود', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (140, N'ساينت مارتني فرنسي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (141, N'مدغشقري', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (142, N'مارشالي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (143, N'مقدوني', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (144, N'مالي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (145, N'ميانماري', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (146, N'منغولي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (147, N'ماكاوي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (148, N'ماريني', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (149, N'مارتينيكي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (150, N'موريتانيي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (151, N'مونتسيراتي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (152, N'مالطي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (153, N'موريشيوسي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (154, N'مالديفي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (155, N'مالاوي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (156, N'مكسيكي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (157, N'ماليزي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (158, N'موزمبيقي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (159, N'ناميبي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (160, N'كاليدوني', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (161, N'نيجيري', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (162, N'نورفوليكي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (163, N'نيجيري', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (164, N'نيكاراجوي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (165, N'هولندي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (166, N'نرويجي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (167, N'نيبالي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (168, N'نوري', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (169, N'ني', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (170, N'نيوزيلندي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (171, N'عماني', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (172, N'بنمي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (173, N'بيري', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (174, N'بولينيزيي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (175, N'بابوي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (176, N'فلبيني', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (177, N'باكستاني', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (178, N'بولندي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (179, N'بيتكيرني', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (180, N'بورتي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (181, N'فلسطيني', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (182, N'برتغالي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (183, N'بالاوي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (184, N'بارغاوي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (185, N'قطري', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (186, N'ريونيوني', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (187, N'روماني', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (188, N'صربي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (189, N'روسي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (190, N'رواندا', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (191, N'سعودي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (192, N'جزر سليمان', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (193, N'سيشيلي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (194, N'سوداني', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (195, N'سويدي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (196, N'سنغافوري', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (197, N'هيلاني', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (198, N'سولفيني', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (199, N'سفالبارد ويان ماين', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (200, N'سولفاكي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (201, N'سيراليوني', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (202, N'ماريني', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (203, N'سنغالي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (204, N'صومالي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (205, N'سورينامي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (206, N'سوادني جنوبي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (207, N'ساو تومي وبرينسيبي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (208, N'سلفادوري', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (209, N'ساينت مارتني هولندي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (210, N'سوري', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (211, N'سوازيلندي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (212, N'جزر توركس وكايكوس', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (213, N'تشادي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (214, N'أراض فرنسية جنوبية وأنتارتيكية', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (215, N'توغي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (216, N'تايلندي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (217, N'طاجيكستاني', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (218, N'توكيلاوي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (219, N'تيموري', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (220, N'تركمانستاني', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (221, N'تونسي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (222, N'تونغي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (223, N'تركي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (224, N'ترينيداد وتوباغو', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (225, N'توفالي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (226, N'تايواني', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (227, N'تنزانيي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (228, N'أوكراني', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (229, N'أوغندي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (230, N'أمريكي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (231, N'أمريكي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (232, N'أورغواي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (233, N'أوزباكستاني', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (234, N'فاتيكاني', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (235, N'سانت فنسنت وجزر غرينادين', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (236, N'فنزويلي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (237, N'أمريكي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (238, N'فيتنامي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (239, N'فانواتي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (240, N'فوتوني', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (241, N'ساموي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (242, N'كوسيفي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (243, N'يمني', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (244, N'مايوتي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (245, N'أفريقي', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (246, N'زامبياني', NULL, NULL, NULL, NULL, 0, NULL, NULL);INSERT [{Schema}].[Nationalities] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (247, N'زمبابوي', NULL, NULL, NULL, NULL, 0, NULL, NULL);SET IDENTITY_INSERT [{Schema}].[Nationalities] OFF;";

            if (db.Nationalities.Where(x => !x.IsDeleted).Count() == 0)
            {
                db.Database.ExecuteSqlCommand(query);
            }

        }
        #endregion
        #region Balance Functions
        public static void AddingFunctionsToDataBase(VTSaleEntities db)
        {
            var f1 = $@"
CREATE FUNCTION [{Schema}].[GetBalance]
(
	-- Add the parameters for the function here
	@ItemId uniqueIdentifier,
	@StoreId uniqueIdentifier
)
RETURNS float
AS
BEGIN
declare @countSupply float -- اجمال عدد الصنف فى التوريد 
declare @countSell float -- اجمال عدد الصنف المباع
declare @countBackSupply  float -- اجمالى الصنف المرتجع للمورد
declare @countBackSell float -- اجمالى الصنف المرتجع من العميل
declare @countStoreTranFrom float --اجمالى الصنف الخارج من المخزن
declare @countStoreTranTo float --اجمالى الصنف الداخل الى المخزن
declare @countItemIntialBalance float --اجمالى الصنف فى رصيد اول المدة

declare @countProductionOrders float --اجمالى الصنف فى اوامر الانتاج
declare @countProductionOrderDetails float --اجمالى الصنف فى تصنيع اوامر الانتاج
declare @countProductionOrderReceipts float --اجمالى الصنف المنتج عند تسليمه الى مخزن الانتاج التام

declare @countMaintenances int --اجمالى الصنف فى مخازن الصيانة فواتير الصيانة
declare @countMaintenanceReceipts int --اجمالى الصنف التى تم استلامها ودخولها المخزن بعد الصيانة
declare @countSpareParts float -- اجمال عدد الصنف المباعه كقطع غيار 

declare @countInventory float -- اجمالى الصنف فى الجرد  

declare @count float

select @countSupply=sum(pd.Quantity) from [{Schema}].PurchaseInvoicesDetails pd INNER JOIN [{Schema}].PurchaseInvoices p ON p.Id=pd.PurchaseInvoiceId  where p.IsFinalApproval=1 AND p.IsDeleted=0 AND pd.IsDeleted=0 AND pd.ItemId=@itemId and  pd.storeId=@storeId -- التوريد
select @countSell=sum(sd.Quantity) from [{Schema}].SellInvoicesDetails sd INNER JOIN [{Schema}].SellInvoices s ON s.Id=sd.SellInvoiceId  where  s.IsFinalApproval=1 AND s.IsDeleted=0 AND sd.IsDeleted=0 AND sd.ItemId=@itemId and sd.storeId=@storeId -- المباع
select @countBackSupply=sum(pd.Quantity) from [{Schema}].PurchaseBackInvoicesDetails pd INNER JOIN [{Schema}].PurchaseBackInvoices p ON p.Id=pd.PurchaseBackInvoiceId  where  p.IsFinalApproval=1  AND p.IsDeleted=0 AND pd.IsDeleted=0 AND pd.ItemId=@itemId and  pd.storeId=@storeId -- مرتجع للمورد
select @countBackSell=sum(sd.Quantity) from [{Schema}].SellBackInvoicesDetails sd INNER JOIN [{Schema}].SellBackInvoices s ON s.Id=sd.SellBackInvoiceId  where s.IsFinalApproval=1 AND s.IsDeleted=0 AND sd.IsDeleted=0 AND sd.ItemId=@itemId and sd.storeId=@storeId -- مرتجع منن العميل
select @countStoreTranFrom=sum(StoresTransferDetails.Quantity) from [{Schema}].StoresTransfers inner join  [{Schema}].StoresTransferDetails on StoresTransfers.Id=StoresTransferDetails.StoresTransferId where StoresTransfers.IsDeleted=0 AND StoresTransferDetails.IsDeleted=0 AND StoresTransfers.StoreFromId=@storeId and StoresTransferDetails.ItemId=@itemId AND StoresTransfers.SaleMenStatus=1 AND StoresTransfers.SaleMenIsApproval=1 --)OR(StoresTransfers.SaleMenStatus=0 AND StoresTransfers.SaleMenIsApproval=0)) -- عدم احتساب اى كميات لم يعتمدها المندوب محوله إليه
select @countStoreTranTo=sum(StoresTransferDetails.Quantity) from [{Schema}].StoresTransfers inner join  [{Schema}].StoresTransferDetails on StoresTransfers.Id=StoresTransferDetails.StoresTransferId where StoresTransfers.IsDeleted=0 AND StoresTransferDetails.IsDeleted=0 AND StoresTransfers.StoreToId=@storeId and StoresTransferDetails.ItemId=@itemId AND StoresTransfers.SaleMenStatus=1 AND StoresTransfers.SaleMenIsApproval=1 --)OR(StoresTransfers.SaleMenStatus=0 AND StoresTransfers.SaleMenIsApproval=0)) -- عدم احتساب اى كميات لم يعتمدها المندوب محوله إليه
select @countItemIntialBalance=sum(i.Quantity) from [{Schema}].ItemIntialBalances i where i.IsDeleted=0 AND i.StoreId=@storeId and i.ItemId=@itemId AND i.IsApproval=1

select @countProductionOrders=ISNULL(sum(p.OrderQuantity),0)-ISNULL(sum(pr.ReceiptQuantity),0) from [{Schema}].ProductionOrders p LEFT JOIN [{Schema}].ProductionOrderReceipts pr On pr.ProductionOrderId=p.Id where p.IsDeleted=0 AND p.ProductionStoreId=@StoreId and p.FinalItemId=@itemId
select @countProductionOrderDetails=sum(pd.Quantity+pd.Quantitydamage) from [{Schema}].ProductionOrders p inner join  [{Schema}].ProductionOrderDetails pd on p.Id=pd.ProductionOrderId where p.IsDeleted=0 AND pd.IsDeleted=0 AND pd.StoreId=@StoreId and pd.ItemId=@itemId
select @countProductionOrderReceipts=sum(p.ReceiptQuantity) from [{Schema}].ProductionOrderReceipts p INNER JOIN [{Schema}].ProductionOrders po ON p.ProductionOrderId=po.Id where p.IsDeleted=0 AND p.FinalItemStoreId=@StoreId and po.FinalItemId=@itemId 

select @countMaintenances=count(ds.Id) from [{Schema}].Maintenances m inner join  [{Schema}].MaintenanceDetails ds on m.Id=ds.MaintenanceId where m.IsFinalApproval=1 AND m.IsDeleted=0 AND ds.IsDeleted=0 AND m.StoreId=@storeId and ds.ItemId=@itemId  AND m.StoreReceiptId is not null
select @countMaintenanceReceipts=count(ds.Id) from [{Schema}].Maintenances m inner join  [{Schema}].MaintenanceDetails ds on m.Id=ds.MaintenanceId where m.IsFinalApproval=1 AND m.IsDeleted=0 AND ds.IsDeleted=0 AND m.StoreReceiptId=@storeId and ds.ItemId=@itemId  
select @countSpareParts=sum(s.Quantity) from [{Schema}].Maintenances m INNER JOIN [{Schema}].MaintenanceDetails md ON m.Id=md.MaintenanceId INNER JOIN [{Schema}].MaintenanceSpareParts s ON md.Id=s.MaintenanceDetailId  where  m.IsFinalApproval=1 AND s.IsDeleted=0 AND md.IsDeleted=0 AND s.IsDeleted=0 AND s.ItemId=@itemId and s.storeId=@storeId -- قطع الغيار

select @countInventory=sum(pd.DifferenceCount) from [{Schema}].InventoryInvoiceDetails pd INNER JOIN [{Schema}].InventoryInvoices p ON p.Id=pd.InventoryInvoiceId  where p.IsDeleted=0 AND pd.IsDeleted=0 AND pd.ItemId=@itemId and  p.storeId=@storeId -- الجرد

set @count =(ISNULL(@countSupply,0)+ISNULL(@countBackSell,0)+ISNULL(@countStoreTranTo,0)+ISNULL(@countItemIntialBalance,0)+ISNULL(@countProductionOrders,0)+ISNULL(@countProductionOrderReceipts,0)+ISNULL(@countMaintenances,0))-(ISNULL(@countSell,0)+ISNULL(@countBackSupply,0)+ISNULL(@countStoreTranFrom,0)+ISNULL(@countProductionOrderDetails,0)+ISNULL(@countMaintenanceReceipts,0)+ISNULL(@countSpareParts,0))+ISNULL(@countInventory,0)
	RETURN @count


END";
            var f2 = $@"
CREATE FUNCTION [{Schema}].[GetBalanceByBranchAllStores]
(
	-- Add the parameters for the function here
	@ItemId uniqueIdentifier,
	@BranchId uniqueIdentifier
)
RETURNS float
AS
BEGIN
declare @countSupply float -- اجمال عدد الصنف فى التوريد 
declare @countSell float -- اجمال عدد الصنف المباع
declare @countBackSupply  float -- اجمالى الصنف المرتجع للمورد
declare @countBackSell float -- اجمالى الصنف المرتجع من العميل
declare @countStoreTranFrom float --اجمالى الصنف الخارج من المخزن
declare @countStoreTranTo float --اجمالى الصنف الداخل الى المخزن
declare @countItemIntialBalance float --اجمالى الصنف فى رصيد اول المدة

declare @countProductionOrders float --اجمالى الصنف فى اوامر الانتاج
declare @countProductionOrderDetails float --اجمالى الصنف فى تصنيع اوامر الانتاج
declare @countProductionOrderReceipts float --اجمالى الصنف المنتج عند تسليمه الى مخزن الانتاج التام

declare @countMaintenances int --اجمالى الصنف فى مخازن الصيانة فواتير الصيانة
declare @countMaintenanceReceipts int --اجمالى الصنف التى تم استلامها ودخولها المخزن بعد الصيانة
declare @countSpareParts float -- اجمال عدد الصنف المباعه كقطع غيار 

declare @countInventory float -- اجمالى الصنف فى الجرد  

declare @count float

select @countSupply=sum(pd.Quantity) from [{Schema}].PurchaseInvoicesDetails pd INNER JOIN [{Schema}].PurchaseInvoices p ON p.Id=pd.PurchaseInvoiceId  where  p.IsFinalApproval=1 AND p.IsDeleted=0 AND pd.IsDeleted=0 AND pd.ItemId=@itemId AND p.BranchId=@BranchId-- التوريد
select @countSell=sum(sd.Quantity) from [{Schema}].SellInvoicesDetails sd INNER JOIN [{Schema}].SellInvoices s ON s.Id=sd.SellInvoiceId  where s.IsFinalApproval=1 AND s.IsDeleted=0 AND sd.IsDeleted=0 AND sd.ItemId=@itemId AND s.BranchId=@BranchId -- المباع
select @countBackSupply=sum(pd.Quantity) from [{Schema}].PurchaseBackInvoicesDetails pd INNER JOIN [{Schema}].PurchaseBackInvoices p ON p.Id=pd.PurchaseBackInvoiceId  where p.IsFinalApproval=1 AND p.IsDeleted=0 AND pd.IsDeleted=0 AND pd.ItemId=@itemId AND p.BranchId=@BranchId  -- مرتجع للمورد
select @countBackSell=sum(sd.Quantity) from [{Schema}].SellBackInvoicesDetails sd INNER JOIN [{Schema}].SellBackInvoices s ON s.Id=sd.SellBackInvoiceId  where s.IsFinalApproval=1 AND s.IsDeleted=0 AND sd.IsDeleted=0 AND sd.ItemId=@itemId and s.BranchId=@BranchId -- مرتجع منن العميل
select @countStoreTranFrom=sum(StoresTransferDetails.Quantity) from [{Schema}].StoresTransfers inner join  [{Schema}].StoresTransferDetails on StoresTransfers.Id=StoresTransferDetails.StoresTransferId INNER JOIN [{Schema}].Stores s ON s.Id=StoresTransfers.StoreFromId where StoresTransfers.IsDeleted=0 AND StoresTransferDetails.IsDeleted=0 AND s.BranchId=@BranchId and StoresTransferDetails.ItemId=@itemId AND StoresTransfers.SaleMenStatus=1 AND StoresTransfers.SaleMenIsApproval=1 --)OR(StoresTransfers.SaleMenStatus=0 AND StoresTransfers.SaleMenIsApproval=0)) -- عدم احتساب اى كميات لم يعتمدها المندوب محوله إليه
select @countStoreTranTo=sum(StoresTransferDetails.Quantity) from [{Schema}].StoresTransfers inner join  [{Schema}].StoresTransferDetails on StoresTransfers.Id=StoresTransferDetails.StoresTransferId INNER JOIN [{Schema}].Stores s ON s.Id=StoresTransfers.StoreToId where StoresTransfers.IsDeleted=0 AND StoresTransferDetails.IsDeleted=0 AND s.BranchId=@BranchId and StoresTransferDetails.ItemId=@itemId AND StoresTransfers.SaleMenStatus=1 AND StoresTransfers.SaleMenIsApproval=1 --)OR(StoresTransfers.SaleMenStatus=0 AND StoresTransfers.SaleMenIsApproval=0)) -- عدم احتساب اى كميات لم يعتمدها المندوب محوله إليه
select @countItemIntialBalance=sum(i.Quantity) from [{Schema}].ItemIntialBalances i where i.IsDeleted=0  and i.ItemId=@itemId AND i.IsApproval=1

select @countProductionOrders=ISNULL(sum(p.OrderQuantity),0)-ISNULL(sum(pr.ReceiptQuantity),0) from [{Schema}].ProductionOrders p LEFT JOIN [{Schema}].ProductionOrderReceipts pr On pr.ProductionOrderId=p.Id where p.IsDeleted=0 AND p.BranchId=@BranchId and p.FinalItemId=@itemId 
select @countProductionOrderDetails=sum(pd.Quantity+pd.Quantitydamage) from [{Schema}].ProductionOrders p inner join  [{Schema}].ProductionOrderDetails pd on p.Id=pd.ProductionOrderId where p.IsDeleted=0 AND pd.IsDeleted=0 AND p.BranchId=@BranchId and pd.ItemId=@itemId
select @countProductionOrderReceipts=sum(p.ReceiptQuantity) from [{Schema}].ProductionOrderReceipts p INNER JOIN [{Schema}].ProductionOrders po ON p.ProductionOrderId=po.Id INNER JOIN [{Schema}].Stores s ON s.Id=p.FinalItemStoreId where p.IsDeleted=0 AND s.BranchId=@BranchId and po.FinalItemId=@itemId 

select @countMaintenances=count(ds.Id) from [{Schema}].Maintenances m inner join  [{Schema}].MaintenanceDetails ds on m.Id=ds.MaintenanceId where m.IsFinalApproval=1 AND m.IsDeleted=0 AND ds.IsDeleted=0 AND m.BranchId=@BranchId and ds.ItemId=@itemId   AND m.StoreReceiptId is not null
select @countMaintenanceReceipts=count(ds.Id) from [{Schema}].Maintenances m inner join  [{Schema}].MaintenanceDetails ds on m.Id=ds.MaintenanceId where  m.IsFinalApproval=1 AND m.IsDeleted=0 AND ds.IsDeleted=0 AND m.BranchId=@BranchId and ds.ItemId=@itemId  
select @countSpareParts=sum(s.Quantity) from [{Schema}].Maintenances m INNER JOIN [{Schema}].MaintenanceDetails md ON m.Id=md.MaintenanceId INNER JOIN [{Schema}].MaintenanceSpareParts s ON md.Id=s.MaintenanceDetailId  where m.IsFinalApproval=1 AND s.IsDeleted=0 AND md.IsDeleted=0 AND s.IsDeleted=0 AND s.ItemId=@itemId and m.BranchId=@BranchId -- قطع الغيار

select @countInventory=sum(pd.DifferenceCount) from [{Schema}].InventoryInvoiceDetails pd INNER JOIN [{Schema}].InventoryInvoices p ON p.Id=pd.InventoryInvoiceId  where p.IsDeleted=0 AND pd.IsDeleted=0 AND pd.ItemId=@itemId and p.BranchId=@BranchId -- الجرد

set @count =(ISNULL(@countSupply,0)+ISNULL(@countBackSell,0)+ISNULL(@countStoreTranTo,0)+ISNULL(@countItemIntialBalance,0)+ISNULL(@countProductionOrders,0)+ISNULL(@countProductionOrderReceipts,0)+ISNULL(@countMaintenances,0))-(ISNULL(@countSell,0)+ISNULL(@countBackSupply,0)+ISNULL(@countStoreTranFrom,0)+ISNULL(@countProductionOrderDetails,0)+ISNULL(@countMaintenanceReceipts,0)+ISNULL(@countSpareParts,0))+ISNULL(@countInventory,0)
	RETURN @count


END
";
            var f3 = $@"
CREATE FUNCTION [{Schema}].[GetBalanceByItemAllStores]
(
	-- Add the parameters for the function here
	@ItemId uniqueIdentifier
)
RETURNS float
AS
BEGIN
declare @countSupply int -- اجمال عدد الصنف فى التوريد 
declare @countSell int -- اجمال عدد الصنف المباع
declare @countBackSupply  int -- اجمالى الصنف المرتجع للمورد
declare @countBackSell int -- اجمالى الصنف المرتجع من العميل
--declare @countStoreTranFrom int --اجمالى الصنف الخارج من المخزن
--declare @countStoreTranTo int --اجمالى الصنف الداخل الى المخزن
declare @countItemIntialBalance int --اجمالى الصنف فى رصيد اول المدة

declare @countProductionOrders int --اجمالى الصنف فى اوامر الانتاج
declare @countProductionOrderDetails int --اجمالى الصنف فى تصنيع اوامر الانتاج
declare @countProductionOrderReceipts int --اجمالى الصنف المنتج عند تسليمه الى مخزن الانتاج التام

declare @countMaintenances int --اجمالى الصنف فى مخازن الصيانة فواتير الصيانة
declare @countMaintenanceReceipts int --اجمالى الصنف التى تم استلامها ودخولها المخزن بعد الصيانة
declare @countSpareParts int -- اجمال عدد الصنف المباعه كقطع غيار 

declare @countInventory int -- اجمالى الصنف فى الجرد  

declare @count int

select @countSupply=sum(pd.Quantity) from [{Schema}].PurchaseInvoicesDetails pd INNER JOIN [{Schema}].PurchaseInvoices p ON p.Id=pd.PurchaseInvoiceId  where p.IsApprovalAccountant=1 AND p.IsApprovalStore=1 AND p.IsFinalApproval=1 AND p.IsDeleted=0 AND pd.IsDeleted=0 AND pd.ItemId=@itemId -- التوريد
select @countSell=sum(sd.Quantity) from [{Schema}].SellInvoicesDetails sd INNER JOIN [{Schema}].SellInvoices s ON s.Id=sd.SellInvoiceId  where s.IsApprovalAccountant=1 AND s.IsApprovalStore=1 AND s.IsFinalApproval=1 AND s.IsDeleted=0 AND sd.IsDeleted=0 AND sd.ItemId=@itemId  -- المباع
select @countBackSupply=sum(pd.Quantity) from [{Schema}].PurchaseBackInvoicesDetails pd INNER JOIN [{Schema}].PurchaseBackInvoices p ON p.Id=pd.PurchaseBackInvoiceId  where p.IsApprovalAccountant=1 AND p.IsApprovalStore=1 AND p.IsDeleted=0 AND p.IsFinalApproval=1 AND pd.IsDeleted=0 AND pd.ItemId=@itemId   -- مرتجع للمورد
select @countBackSell=sum(sd.Quantity) from [{Schema}].SellBackInvoicesDetails sd INNER JOIN [{Schema}].SellBackInvoices s ON s.Id=sd.SellBackInvoiceId  where s.IsApprovalAccountant=1 AND s.IsApprovalStore=1 AND s.IsDeleted=0 AND s.IsFinalApproval=1 AND sd.IsDeleted=0 AND sd.ItemId=@itemId -- مرتجع منن العميل
--select @countStoreTranFrom=sum(StoresTransferDetails.Quantity) from [{Schema}].StoresTransfers inner join  [{Schema}].StoresTransferDetails on StoresTransfers.Id=StoresTransferDetails.StoresTransferId INNER JOIN [{Schema}].Stores s ON s.Id=StoresTransfers.StoreFromId where StoresTransfers.IsDeleted=0 AND StoresTransferDetails.IsDeleted=0 and StoresTransferDetails.ItemId=@itemId AND StoresTransfers.SaleMenStatus=1 AND StoresTransfers.SaleMenIsApproval=1--)OR(StoresTransfers.SaleMenStatus=0 AND StoresTransfers.SaleMenIsApproval=0)) -- عدم احتساب اى كميات لم يعتمدها المندوب محوله إليه
--select @countStoreTranTo=sum(StoresTransferDetails.Quantity) from [{Schema}].StoresTransfers inner join  [{Schema}].StoresTransferDetails on StoresTransfers.Id=StoresTransferDetails.StoresTransferId INNER JOIN [{Schema}].Stores s ON s.Id=StoresTransfers.StoreToId where StoresTransfers.IsDeleted=0 AND StoresTransferDetails.IsDeleted=0 AND  StoresTransferDetails.ItemId=@itemId AND StoresTransfers.SaleMenStatus=1 AND StoresTransfers.SaleMenIsApproval=1--)OR(StoresTransfers.SaleMenStatus=0 AND StoresTransfers.SaleMenIsApproval=0)) -- عدم احتساب اى كميات لم يعتمدها المندوب محوله إليه
select @countItemIntialBalance=sum(i.Quantity) from [{Schema}].ItemIntialBalances i where i.IsDeleted=0  and i.ItemId=@itemId AND i.IsApproval=1

select @countProductionOrders=ISNULL(sum(p.OrderQuantity),0)-ISNULL(sum(pr.ReceiptQuantity),0) from [{Schema}].ProductionOrders p LEFT JOIN [{Schema}].ProductionOrderReceipts pr On pr.ProductionOrderId=p.Id where p.IsDeleted=0 AND  p.FinalItemId=@itemId 
select @countProductionOrderDetails=sum(pd.Quantity+pd.Quantitydamage) from [{Schema}].ProductionOrders p inner join  [{Schema}].ProductionOrderDetails pd on p.Id=pd.ProductionOrderId where p.IsDeleted=0 AND pd.IsDeleted=0 AND  pd.ItemId=@itemId
select @countProductionOrderReceipts=sum(p.ReceiptQuantity) from [{Schema}].ProductionOrderReceipts p INNER JOIN [{Schema}].ProductionOrders po ON p.ProductionOrderId=po.Id  where p.IsDeleted=0 AND po.FinalItemId=@itemId 

select @countMaintenances=count(ds.Id) from [{Schema}].Maintenances m inner join  [{Schema}].MaintenanceDetails ds on m.Id=ds.MaintenanceId where m.IsFinalApproval=1 AND m.IsDeleted=0 AND ds.IsDeleted=0  and ds.ItemId=@itemId  AND m.StoreReceiptId is not null
select @countMaintenanceReceipts=count(ds.Id) from [{Schema}].Maintenances m inner join  [{Schema}].MaintenanceDetails ds on m.Id=ds.MaintenanceId where m.IsFinalApproval=1 AND m.IsDeleted=0 AND ds.IsDeleted=0 and ds.ItemId=@itemId  
select @countSpareParts=sum(s.Quantity) from [{Schema}].Maintenances m INNER JOIN [{Schema}].MaintenanceDetails md ON m.Id=md.MaintenanceId INNER JOIN [{Schema}].MaintenanceSpareParts s ON md.Id=s.MaintenanceDetailId  where  m.IsFinalApproval=1 AND s.IsDeleted=0 AND md.IsDeleted=0 AND s.IsDeleted=0 AND s.ItemId=@itemId -- قطع الغيار

select @countInventory=sum(pd.DifferenceCount) from [{Schema}].InventoryInvoiceDetails pd INNER JOIN [{Schema}].InventoryInvoices p ON p.Id=pd.InventoryInvoiceId  where p.IsDeleted=0 AND pd.IsDeleted=0 AND pd.ItemId=@itemId -- الجرد

set @count =(ISNULL(@countSupply,0)+ISNULL(@countBackSell,0)+ISNULL(@countItemIntialBalance,0)+ISNULL(@countProductionOrders,0)+ISNULL(@countProductionOrderReceipts,0)+ISNULL(@countMaintenances,0))-(ISNULL(@countSell,0)+ISNULL(@countBackSupply,0)+ISNULL(@countProductionOrderDetails,0)+ISNULL(@countMaintenanceReceipts,0)+ISNULL(@countSpareParts,0))+ISNULL(@countInventory,0)

	RETURN @count


END";
            var f4 = $@"
CREATE FUNCTION [{Schema}].[GetBalanceByProductionOrder]
(
	-- Add the parameters for the function here
	@ItemId uniqueIdentifier,
	@StoreId uniqueIdentifier,
	@ProductionOrderId uniqueIdentifier
)
RETURNS float
AS
BEGIN
declare @countSell float -- اجمال عدد الصنف المباع
declare @countBackSell float -- اجمالى الصنف المرتجع من العميل
declare @countStoreTranFrom float --اجمالى الصنف الخارج من المخزن
declare @countStoreTranTo float --اجمالى الصنف الداخل الى المخزن

declare @countProductionOrders float --اجمالى الصنف فى اوامر الانتاج
declare @countProductionOrderDetails float --اجمالى الصنف فى تصنيع اوامر الانتاج
declare @countProductionOrderReceipts float --اجمالى الصنف المنتج عند تسليمه الى مخزن الانتاج التام

declare @countMaintenances int --اجمالى الصنف فى مخازن الصيانة فواتير الصيانة
declare @countMaintenanceReceipts int --اجمالى الصنف التى تم استلامها ودخولها المخزن بعد الصيانة
declare @countSpareParts float -- اجمال عدد الصنف المباعه كقطع غيار 


declare @count float

--select @countSell=sum(sd.Quantity) from [{Schema}].SellInvoicesDetails sd INNER JOIN [{Schema}].SellInvoices s ON s.Id=sd.SellInvoiceId INNER JOIN [{Schema}].ItemSerials it ON it.ProductionOrderId=@ProductionOrderId  where s.IsApprovalAccountant=1 AND s.IsApprovalStore=1 AND s.IsDeleted=0 AND sd.IsDeleted=0 AND sd.ItemId=@itemId and sd.storeId=@storeId AND s.Id in (select h.ReferrenceId from CasesItemSerialHistories h) -- المباع

select @countSell=sum(sd.Quantity) from [{Schema}].SellInvoicesDetails sd INNER JOIN [{Schema}].SellInvoices s ON s.Id=sd.SellInvoiceId INNER JOIN  [{Schema}].ItemSerials ser ON ser.Id=sd.ItemSerialId  where s.IsFinalApproval=1 AND s.IsDeleted=0 AND sd.IsDeleted=0 AND sd.ItemId=@itemId and sd.storeId=@storeId AND ser.ProductionOrderId=@ProductionOrderId -- المباع
select @countBackSell=sum(sd.Quantity) from [{Schema}].SellBackInvoicesDetails sd INNER JOIN [{Schema}].SellBackInvoices s ON s.Id=sd.SellBackInvoiceId INNER JOIN [{Schema}].ItemSerials ser ON ser.Id=sd.ItemSerialId  where s.IsFinalApproval=1 AND s.IsDeleted=0 AND sd.IsDeleted=0 AND sd.ItemId=@itemId and sd.storeId=@storeId AND ser.ProductionOrderId=@ProductionOrderId-- مرتجع منن العميل
select @countStoreTranFrom=sum(StoresTransferDetails.Quantity) from [{Schema}].StoresTransfers inner join  [{Schema}].StoresTransferDetails on StoresTransfers.Id=StoresTransferDetails.StoresTransferId  where StoresTransfers.IsDeleted=0 AND StoresTransferDetails.ProductionOrderId=@ProductionOrderId  AND StoresTransferDetails.IsDeleted=0 AND StoresTransfers.StoreFromId=@storeId and StoresTransferDetails.ItemId=@itemId AND StoresTransfers.SaleMenStatus=1 AND StoresTransfers.SaleMenIsApproval=1--)OR(StoresTransfers.SaleMenStatus=0 AND StoresTransfers.SaleMenIsApproval=0)) -- عدم احتساب اى كميات لم يعتمدها المندوب محوله إليه
select @countStoreTranTo=sum(StoresTransferDetails.Quantity) from [{Schema}].StoresTransfers inner join  [{Schema}].StoresTransferDetails on StoresTransfers.Id=StoresTransferDetails.StoresTransferId  where StoresTransfers.IsDeleted=0 AND StoresTransferDetails.ProductionOrderId=@ProductionOrderId AND StoresTransferDetails.IsDeleted=0 AND StoresTransfers.StoreToId=@storeId and StoresTransferDetails.ItemId=@itemId AND StoresTransfers.SaleMenStatus=1 AND StoresTransfers.SaleMenIsApproval=1--)OR(StoresTransfers.SaleMenStatus=0 AND StoresTransfers.SaleMenIsApproval=0)) -- عدم احتساب اى كميات لم يعتمدها المندوب محوله إليه

select @countProductionOrders=ISNULL(sum(p.OrderQuantity),0)-ISNULL(sum(pr.ReceiptQuantity),0) from [{Schema}].ProductionOrders p LEFT JOIN [{Schema}].ProductionOrderReceipts pr On pr.ProductionOrderId=p.Id where p.IsDeleted=0 AND p.ProductionStoreId=@StoreId and p.FinalItemId=@itemId AND p.Id=@ProductionOrderId
select @countProductionOrderDetails=sum(pd.Quantity+pd.Quantitydamage) from [{Schema}].ProductionOrders p inner join  [{Schema}].ProductionOrderDetails pd on p.Id=pd.ProductionOrderId where p.IsDeleted=0 AND pd.IsDeleted=0 AND pd.StoreId=@StoreId and pd.ItemId=@itemId AND p.Id=@ProductionOrderId
select @countProductionOrderReceipts=sum(p.ReceiptQuantity) from [{Schema}].ProductionOrderReceipts p INNER JOIN [{Schema}].ProductionOrders po ON p.ProductionOrderId=po.Id where p.IsDeleted=0 AND p.FinalItemStoreId=@StoreId and po.FinalItemId=@itemId AND p.ProductionOrderId=@ProductionOrderId

select @countMaintenances=count(ds.Id) from [{Schema}].Maintenances m inner join  [{Schema}].MaintenanceDetails ds on m.Id=ds.MaintenanceId INNER JOIN [{Schema}].ItemSerials ser ON ds.ItemSerialId=ser.Id where m.IsFinalApproval=1 AND m.IsDeleted=0 AND ds.IsDeleted=0 AND ser.ProductionOrderId=@ProductionOrderId and m.StoreId=@storeId  and ds.ItemId=@itemId   AND m.StoreReceiptId is not null
select @countMaintenanceReceipts=count(ds.Id) from [{Schema}].Maintenances m inner join  [{Schema}].MaintenanceDetails ds on m.Id=ds.MaintenanceId INNER JOIN [{Schema}].ItemSerials ser ON ds.ItemSerialId=ser.Id where m.IsFinalApproval=1 AND m.IsDeleted=0  AND ser.ProductionOrderId=@ProductionOrderId AND ds.IsDeleted=0 AND m.StoreReceiptId=@storeId and ds.ItemId=@itemId  
select @countSpareParts=sum(s.Quantity) from [{Schema}].Maintenances m INNER JOIN [{Schema}].MaintenanceDetails md ON m.Id=md.MaintenanceId INNER JOIN [{Schema}].MaintenanceSpareParts s ON md.Id=s.MaintenanceDetailId  where  m.IsFinalApproval=1 AND s.IsDeleted=0 AND md.IsDeleted=0 AND s.IsDeleted=0 AND s.ProductionOrderId=@ProductionOrderId AND s.ItemId=@itemId and s.storeId=@storeId -- قطع الغيار

set @count =(ISNULL(@countBackSell,0)+ISNULL(@countStoreTranTo,0)+ISNULL(@countProductionOrders,0)+ISNULL(@countProductionOrderReceipts,0)+ISNULL(@countMaintenances,0))-(ISNULL(@countSell,0)+ISNULL(@countStoreTranFrom,0)+ISNULL(@countProductionOrderDetails,0)+ISNULL(@countMaintenanceReceipts,0)+ISNULL(@countSpareParts,0))

	RETURN @count


END
";
            var f5 = $@"
CREATE FUNCTION [{Schema}].[GetBalanceItemIntial]
(
	-- Add the parameters for the function here
	@ItemId uniqueIdentifier,
	@StoreId uniqueIdentifier
	)
RETURNS float
AS
BEGIN
declare @countSell float -- اجمال عدد الصنف المباع
declare @countBackSell float -- اجمالى الصنف المرتجع من العميل
declare @countStoreTranFrom float --اجمالى الصنف الخارج من المخزن
declare @countStoreTranTo float --اجمالى الصنف الداخل الى المخزن
declare @countItemIntialBalance float --اجمالى الصنف فى رصيد اول المدة

declare @countMaintenances int --اجمالى الصنف فى مخازن الصيانة فواتير الصيانة
declare @countMaintenanceReceipts int --اجمالى الصنف التى تم استلامها ودخولها المخزن بعد الصيانة
declare @countSpareParts float -- اجمال عدد الصنف المباعه كقطع غيار 

declare @count float

--select @countSell=sum(sd.Quantity) from [{Schema}].SellInvoicesDetails sd INNER JOIN [{Schema}].SellInvoices s ON s.Id=sd.SellInvoiceId INNER JOIN [{Schema}].ItemSerials it ON it.IsItemIntial=1  where s.IsApprovalAccountant=1 AND s.IsApprovalStore=1 AND s.IsDeleted=0 AND sd.IsDeleted=0 AND s.Id in (select h.ReferrenceId from CasesItemSerialHistories h) AND sd.ItemId=@itemId and sd.storeId=@storeId -- المباع 

select @countSell=sum(sd.Quantity) from [{Schema}].SellInvoicesDetails sd INNER JOIN [{Schema}].SellInvoices s ON s.Id=sd.SellInvoiceId  INNER JOIN  [{Schema}].ItemSerials ser ON ser.Id=sd.ItemSerialId  where  s.IsFinalApproval=1 AND s.IsDeleted=0 AND sd.IsDeleted=0 AND ser.IsItemIntial=1 AND sd.ItemId=@itemId and sd.storeId=@storeId -- المباع 
select @countBackSell=sum(sd.Quantity) from [{Schema}].SellBackInvoicesDetails sd INNER JOIN [{Schema}].SellBackInvoices s ON s.Id=sd.SellBackInvoiceId INNER JOIN [{Schema}].ItemSerials ser ON ser.Id=sd.ItemSerialId where s.IsFinalApproval=1 AND s.IsDeleted=0 AND sd.IsDeleted=0 AND sd.ItemId=@itemId and sd.storeId=@storeId AND ser.IsItemIntial=1 -- مرتجع منن العميل
select @countStoreTranFrom=sum(StoresTransferDetails.Quantity) from [{Schema}].StoresTransfers inner join  [{Schema}].StoresTransferDetails on StoresTransfers.Id=StoresTransferDetails.StoresTransferId AND StoresTransferDetails.IsItemIntial=1  where StoresTransfers.IsDeleted=0 AND StoresTransferDetails.IsDeleted=0 AND StoresTransfers.StoreFromId=@storeId and StoresTransferDetails.ItemId=@itemId AND StoresTransfers.SaleMenStatus=1 AND StoresTransfers.SaleMenIsApproval=1--)OR(StoresTransfers.SaleMenStatus=0 AND StoresTransfers.SaleMenIsApproval=0)) -- عدم احتساب اى كميات لم يعتمدها المندوب محوله إليه
select @countStoreTranTo=sum(StoresTransferDetails.Quantity) from [{Schema}].StoresTransfers inner join  [{Schema}].StoresTransferDetails on StoresTransfers.Id=StoresTransferDetails.StoresTransferId AND StoresTransferDetails.IsItemIntial=1 where StoresTransfers.IsDeleted=0 AND StoresTransferDetails.IsDeleted=0 AND StoresTransfers.StoreToId=@storeId and StoresTransferDetails.ItemId=@itemId AND StoresTransfers.SaleMenStatus=1 AND StoresTransfers.SaleMenIsApproval=1--)OR(StoresTransfers.SaleMenStatus=0 AND StoresTransfers.SaleMenIsApproval=0)) -- عدم احتساب اى كميات لم يعتمدها المندوب محوله إليه
select @countItemIntialBalance=sum(i.Quantity) from [{Schema}].ItemIntialBalances i where i.IsDeleted=0 AND i.StoreId=@storeId and i.ItemId=@itemId AND i.IsApproval=1

select @countMaintenances=count(ds.Id) from [{Schema}].Maintenances m inner join  [{Schema}].MaintenanceDetails ds on m.Id=ds.MaintenanceId INNER JOIN [{Schema}].ItemSerials ser ON ds.ItemSerialId=ser.Id where m.IsFinalApproval=1 AND m.IsDeleted=0 AND ds.IsDeleted=0 AND ser.IsItemIntial=1 and m.StoreId=@storeId  and ds.ItemId=@itemId AND m.StoreReceiptId is not null  
select @countMaintenanceReceipts=count(ds.Id) from [{Schema}].Maintenances m inner join  [{Schema}].MaintenanceDetails ds on m.Id=ds.MaintenanceId INNER JOIN [{Schema}].ItemSerials ser ON ds.ItemSerialId=ser.Id where m.IsFinalApproval=1 AND m.IsDeleted=0  AND ser.IsItemIntial=1 AND ds.IsDeleted=0 AND m.StoreReceiptId=@storeId and ds.ItemId=@itemId  
select @countSpareParts=sum(s.Quantity) from [{Schema}].Maintenances m INNER JOIN [{Schema}].MaintenanceDetails md ON m.Id=md.MaintenanceId INNER JOIN [{Schema}].MaintenanceSpareParts s ON md.Id=s.MaintenanceDetailId  where  m.IsFinalApproval=1 AND s.IsDeleted=0 AND md.IsDeleted=0 AND s.IsDeleted=0 AND s.IsItemIntial=1 AND s.ItemId=@itemId and s.storeId=@storeId -- قطع الغيار

set @count =(ISNULL(@countBackSell,0)+ISNULL(@countStoreTranTo,0)+ISNULL(@countItemIntialBalance,0)+ISNULL(@countMaintenances,0))-(ISNULL(@countSell,0)+ISNULL(@countStoreTranFrom,0)+ISNULL(@countMaintenanceReceipts,0)+ISNULL(@countSpareParts,0))

	RETURN @count


END";


            string functionName = "GetBalanceItemIntial";
            bool result = CheckFunctionExist(db, Schema, functionName);

            if (!result)
            {
                Console.WriteLine("Balance Functions not found!\nAdding Balance Functions.....");
                db.Database.ExecuteSqlCommand(f1);
                Console.WriteLine("Function 1 added..");
                db.Database.ExecuteSqlCommand(f2);
                Console.WriteLine("Function 2 added..");
                db.Database.ExecuteSqlCommand(f3);
                Console.WriteLine("Function 3 added..");
                db.Database.ExecuteSqlCommand(f4);
                Console.WriteLine("Function 4 added..");
                db.Database.ExecuteSqlCommand(f5);
                Console.WriteLine("Function 5 added..");
                Console.WriteLine("Adding Balance Functions Conpleted!");
            }
        }

        private static bool CheckFunctionExist(VTSaleEntities db, string schema, string functionName)
        {
            return db.Database.SqlQuery<int>($@"IF EXISTS (SELECT *
           FROM   sys.objects
           WHERE  object_id = OBJECT_ID(N'[{schema}].[{functionName}]')
                  AND type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' ))
				  select 1
				  else 
				  select 0").FirstOrDefault() == 1;
        }
        #endregion

        #region Web App Pages links
        public static void AddingPages(VTSaleEntities db, Guid roleID, Guid userId)
        {
            List<Page> pages = new List<Page>();

            //هنا بنضيف الشاشات بشكل يدوي
            #region pages List
            pages.Add(new Page() { Id = 1, ParentId = null, Icon = "icon-md fas fa-list ylow", IsPage = false, Name = "التعريفات الاساسية", OrderNum = 1, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 2, ParentId = 1, Icon = "menu-bullet menu-bullet-line", IsPage = false, Name = "تعريف الدول", OrderNum = 0, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 3, ParentId = 2, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "اضافة دولة جديدة", OrderNum = 0, Url = "/Countries/CreateEdit", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 4, ParentId = 2, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "ادارة دولة", OrderNum = 0, Url = "/Countries/Index", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 5, ParentId = 1, Icon = "menu-bullet menu-bullet-line", IsPage = false, Name = "تعريف المحافظات", OrderNum = 0, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 6, ParentId = 5, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "اضافة محافظة جديدة", OrderNum = 0, Url = "/Cities/CreateEdit", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 7, ParentId = 5, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "ادارة بيانات محافظة", OrderNum = 0, Url = "/Cities/Index", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 8, ParentId = 1, Icon = "menu-bullet menu-bullet-line", IsPage = false, Name = "تعريف المدن", OrderNum = 0, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 9, ParentId = 8, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "اضافة مدينة جديدة", OrderNum = 0, Url = "/Areas/CreateEdit", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 10, ParentId = 8, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "ادارة بيانات مدينة", OrderNum = 0, Url = "/Areas/Index", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 11, ParentId = 1, Icon = "menu-bullet menu-bullet-line", IsPage = false, Name = "تعريف الفروع", OrderNum = 0, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 12, ParentId = 11, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "اضافة فرع جديد", OrderNum = 0, Url = "/Branches/CreateEdit", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 13, ParentId = 11, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "ادارة بيانات فرع", OrderNum = 0, Url = "/Branches/Index", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 14, ParentId = 1, Icon = "menu-bullet menu-bullet-line", IsPage = false, Name = "تعريف البنوك", OrderNum = 0, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 15, ParentId = 14, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "اضافة بنك جديد", OrderNum = 0, Url = "/Banks/CreateEdit", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 16, ParentId = 14, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "ادارة بيانات بنك", OrderNum = 0, Url = "/Banks/Index", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 17, ParentId = 1, Icon = "menu-bullet menu-bullet-line", IsPage = false, Name = "تعريف حسابات البنوك", OrderNum = 0, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 18, ParentId = 17, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "اضافة حساب بنكى", OrderNum = 0, Url = "/BanksAccounts/CreateEdit", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 19, ParentId = 17, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "ادارة بيانات حساب بنكى", OrderNum = 0, Url = "/BanksAccounts/Index", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 20, ParentId = 1, Icon = "menu-bullet menu-bullet-line", IsPage = false, Name = "تعريف الحاويات", OrderNum = 0, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 21, ParentId = 20, Icon = "icon-md fas fa-boxes ylow", IsPage = true, Name = "اضافة حاوية", OrderNum = 0, Url = "/Containers/CreateEdit", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 22, ParentId = 20, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "ادارة بيانات حاوية", OrderNum = 0, Url = "/Containers/Index", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 23, ParentId = 1, Icon = "menu-bullet menu-bullet-line", IsPage = false, Name = "تعريف سياسات الاسعار", OrderNum = 0, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 24, ParentId = 23, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "اضافة سياسة اسعار", OrderNum = 0, Url = "/PricingPolicies/CreateEdit", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 25, ParentId = 23, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "ادارة سياسة اسعار", OrderNum = 0, Url = "/PricingPolicies/Index", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 26, ParentId = 1, Icon = "menu-bullet menu-bullet-line", IsPage = false, Name = "تعريف الخزن", OrderNum = 0, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 27, ParentId = 26, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "اضافة خزنة جديدة", OrderNum = 0, Url = "/Safes/CreateEdit", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 28, ParentId = 26, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "ادارة بيانات خزنة", OrderNum = 0, Url = "/Safes/Index", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 29, ParentId = 1, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "الاعدادات العامة", OrderNum = 0, Url = "/GeneralSettings/Index", OtherUrls = "/GeneralSettings/CheckInventoryType", PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 30, ParentId = 48, Icon = "menu-bullet menu-bullet-line", IsPage = false, Name = "رصيد اول المدة للموردين", OrderNum = 0, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 31, ParentId = 30, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "اضافة رصيد اول المدة للمودرين", OrderNum = 0, Url = "/SupplierIntials/CreateEdit", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 32, ParentId = 30, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "ادارة رصيد اول المدة للمودرين", OrderNum = 0, Url = "/SupplierIntials/Index", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 33, ParentId = 48, Icon = "menu-bullet menu-bullet-line", IsPage = false, Name = "رصيد اول المدة للعملاء", OrderNum = 0, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 34, ParentId = 33, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "اضافة رصيد اول المدة للعملاء", OrderNum = 0, Url = "/CustomerIntials/CreateEdit", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 35, ParentId = 33, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "ادارة رصيد اول المدة للعملاء", OrderNum = 0, Url = "/CustomerIntials/Index", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 36, ParentId = 48, Icon = "menu-bullet menu-bullet-line", IsPage = false, Name = "رصيد اول المدة للبنوك", OrderNum = 0, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 37, ParentId = 36, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "اضافة رصيد اول المدة للبنوك", OrderNum = 0, Url = "/BankAccountIntialBalances/CreateEdit", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 38, ParentId = 36, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "ادارة رصيد اول المدة للبنوك", OrderNum = 0, Url = "/BankAccountIntialBalances/Index", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 39, ParentId = 48, Icon = "menu-bullet menu-bullet-line", IsPage = false, Name = "رصيد اول المدة للخزن", OrderNum = 0, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 40, ParentId = 39, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "اضافة رصيد اول المدة للخزن", OrderNum = 0, Url = "/SafeIntialBalances/CreateEdit", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 41, ParentId = 39, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "ادارة رصيد اول المدة للخزن", OrderNum = 0, Url = "/SafeIntialBalances/Index", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 42, ParentId = 48, Icon = "menu-bullet menu-bullet-line", IsPage = false, Name = "رصيد اول المدة للاصناف", OrderNum = 0, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 43, ParentId = 42, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "اضافة رصيد اول المدة للاصناف", OrderNum = 0, Url = "/ItemIntialBalances/CreateEdit", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 44, ParentId = 42, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "ادارة رصيد اول المدة للاصناف", OrderNum = 0, Url = "/ItemIntialBalances/Index", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 45, ParentId = 48, Icon = "menu-bullet menu-bullet-line", IsPage = false, Name = "رصيد اول المدة للأصول الثابتة", OrderNum = 0, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 46, ParentId = 45, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "اضافة رصيد اول المدة للأصول الثابتة", OrderNum = 0, Url = "/FixedAssetIntialBalances/CreateEdit", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 47, ParentId = 45, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "ادارة رصيد اول المدة للأصول الثابتة", OrderNum = 0, Url = "/FixedAssetIntialBalances/Index", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 48, ParentId = null, Icon = "icon-md fas fa-money-bill-alt ylow", IsPage = false, Name = "رصيد اول المدة", OrderNum = 3, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 49, ParentId = null, Icon = "icon-md fas fa-sitemap ylow", IsPage = false, Name = "الأصناف", OrderNum = 4, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 50, ParentId = 49, Icon = "menu-bullet menu-bullet-line", IsPage = false, Name = "تعريف مجموعات الاصناف", OrderNum = 0, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 51, ParentId = 50, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "اضافة مجموعة صنف", OrderNum = 0, Url = "/ItemGroups/CreateEdit", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 52, ParentId = 50, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "ادارة مجموعة صنف", OrderNum = 0, Url = "/ItemGroups/Index", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 53, ParentId = 49, Icon = "menu-bullet menu-bullet-line", IsPage = false, Name = "تعريف وحدات الاصناف", OrderNum = 0, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 54, ParentId = 53, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "اضافة وحدة صنف", OrderNum = 0, Url = "/Units/CreateEdit", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 55, ParentId = 53, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "ادارة وحدة صنف", OrderNum = 0, Url = "/Units/Index", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 56, ParentId = 49, Icon = "menu-bullet menu-bullet-line", IsPage = false, Name = "تعريف أنواع الاصناف", OrderNum = 0, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 57, ParentId = 56, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "اضافة نوع صنف", OrderNum = 0, Url = "/ItemTypes/CreateEdit", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 58, ParentId = 56, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "ادارة نوع صنف", OrderNum = 0, Url = "/ItemTypes/Index", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 59, ParentId = 49, Icon = "menu-bullet menu-bullet-line", IsPage = false, Name = "تعريف الاصناف", OrderNum = 0, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 60, ParentId = 59, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "اضافة صنف جديد", OrderNum = 0, Url = "/Items/CreateEdit", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 61, ParentId = 59, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "ادارة بيانات صنف", OrderNum = 0, Url = "/Items/Index", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 62, ParentId = null, Icon = "icon-md fas fa-ship ylow", IsPage = false, Name = "توليف صنف", OrderNum = 5, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 64, ParentId = 62, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "انشاء توليفه صنف", OrderNum = 0, Url = "/ItemProductions/CreateEdit", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 65, ParentId = 62, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "ادارة توليفه صنف", OrderNum = 0, Url = "/ItemProductions/Index", OtherUrls = "/ItemProductions/ShowItemProduction", PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 66, ParentId = null, Icon = "icon-md fas fa-user-cog ylow", IsPage = false, Name = "الموردين", OrderNum = 6, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 67, ParentId = 66, Icon = "menu-bullet menu-bullet-line", IsPage = false, Name = "تعريف مورد", OrderNum = 0, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 68, ParentId = 67, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "اضافة مورد جديد", OrderNum = 0, Url = "/Suppliers/CreateEdit", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 69, ParentId = 67, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "ادارة بيانات مورد", OrderNum = 0, Url = "/Suppliers/Index", OtherUrls = ",/UploadCenterTypeFiles/Index,/UploadCenterTypeFiles/GetByInvoGuid,/UploadCenterTypeFiles/Delete", PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 70, ParentId = 66, Icon = "menu-bullet menu-bullet-line", IsPage = false, Name = "صرف نقدى/بنكى لمورد", OrderNum = 0, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 71, ParentId = 70, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "اضافة عملية صرف لمورد", OrderNum = 0, Url = "/SuppliersPayments/CreateEdit", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 72, ParentId = 70, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "ادارة عملية صرف لمورد", OrderNum = 0, Url = "/SuppliersPayments/Index", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 73, ParentId = 79, Icon = "menu-bullet menu-bullet-line", IsPage = false, Name = "فواتير التوريد", OrderNum = 0, Url = "", OtherUrls = null, IsDeleted = true });
            pages.Add(new Page() { Id = 74, ParentId = 79, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "تسجيل فاتورة توريد", OrderNum = 0, Url = "/PurchaseInvoices/CreateEdit", OtherUrls = ",/UploadCenterTypeFiles/Index,/UploadCenterTypeFiles/GetByInvoGuid,/UploadCenterTypeFiles/Delete", PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 75, ParentId = 79, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "ادارة فواتير التوريد", OrderNum = 0, Url = "/PurchaseInvoices/Index", OtherUrls = ",/PurchaseInvoices/ShowHistory,/PurchaseInvoices/ShowPurchaseInvoice,/UploadCenterTypeFiles/Index,/UploadCenterTypeFiles/GetByInvoGuid,/UploadCenterTypeFiles/Delete,/PrintInvoices/ShowPrintInvoice", PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 76, ParentId = 79, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "الاعتماد المحاسبى لفاتورة التوريد", OrderNum = 0, Url = "/PurchaseInvoiceAccounting/Index", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 77, ParentId = 79, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "الاعتماد المخزنى لفاتورة التوريد", OrderNum = 0, Url = "/PurchaseInvoiceStores/Index", OtherUrls = "/PurchaseInvoiceStores/ApprovalStore", PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 78, ParentId = 79, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "الاعتماد النهائى لفاتورة التوريد", OrderNum = 0, Url = "/PurchaseInvoices/ApprovalFinalInvoice", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 79, ParentId = null, Icon = "icon-md fas fa-arrow-alt-circle-right ylow", IsPage = false, Name = "التوريدات", OrderNum = 7, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 80, ParentId = null, Icon = "icon-md fas fa-undo-alt ylow", IsPage = false, Name = "مرتجع التوريدات", OrderNum = 8, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 81, ParentId = 80, Icon = "menu-bullet menu-bullet-line", IsPage = false, Name = "فواتير مرتجع التوريد", OrderNum = 0, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 82, ParentId = 81, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "تسجيل فاتورة مرتجع توريد", OrderNum = 0, Url = "/PurchaseBackInvoices/CreateEdit", OtherUrls = ",/UploadCenterTypeFiles/Index,/UploadCenterTypeFiles/GetByInvoGuid,/UploadCenterTypeFiles/Delete", PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 83, ParentId = 81, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "ادارة فواتير مرتجع التوريد", OrderNum = 0, Url = "/PurchaseBackInvoices/Index", OtherUrls = ",/PurchaseBackInvoices/ShowHistory,/PurchaseBackInvoices/ShowPurchaseBackInvoice,/UploadCenterTypeFiles/Index,/UploadCenterTypeFiles/GetByInvoGuid,/UploadCenterTypeFiles/Delete,/PrintInvoices/ShowPrintInvoice", PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 84, ParentId = 81, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "الاعتماد المحاسبى لفاتورة مرتجع التوريد", OrderNum = 0, Url = "/PurchaseBackInvoiceAccounting/Index", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 85, ParentId = 81, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "الاعتماد المخزنى لفاتورة مرتجع التوريد", OrderNum = 0, Url = "/PurchaseBackInvoiceStores/Index", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 86, ParentId = 81, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "الاعتماد النهائى لفاتورة مرتجع التوريد", OrderNum = 0, Url = "/PurchaseBackInvoices/ApprovalFinalInvoice", OtherUrls = "/PurchaseBackInvoiceStores/ApprovalStore", PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 87, ParentId = 81, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "ارجاع كلى/جزئى لفاتورة توريد", OrderNum = 0,IsDeleted = true,Url = "/PurchaseFullBackInvoices/Index", OtherUrls = ",/UploadCenterTypeFiles/Index,/UploadCenterTypeFiles/GetByInvoGuid,/UploadCenterTypeFiles/Delete,/PurchaseFullBackInvoices/ReturnPatialInvoice", PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 88, ParentId = null, Icon = "icon-md fas fa-user-alt ylow", IsPage = false, Name = "العملاء", OrderNum = 9, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 89, ParentId = 88, Icon = "menu-bullet menu-bullet-line", IsPage = false, Name = "تعريف فئات العملاء", OrderNum = 0, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 90, ParentId = 89, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "اضافة فئة عملاء", OrderNum = 0, Url = "/CustomerCategories/CreateEdit", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 91, ParentId = 89, Icon = null, IsPage = true, Name = "ادارة فئة عملاء", OrderNum = 0, Url = "/CustomerCategories/Index", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 92, ParentId = 88, Icon = null, IsPage = false, Name = "تعريف العملاء", OrderNum = 0, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 93, ParentId = 92, Icon = null, IsPage = true, Name = "اضافة عميل", OrderNum = 0, Url = "/Customers/CreateEdit", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 94, ParentId = 92, Icon = null, IsPage = true, Name = "ادارة بيانات عميل", OrderNum = 0, Url = "/Customers/Index", OtherUrls = ",/UploadCenterTypeFiles/Index,/UploadCenterTypeFiles/GetByInvoGuid,/UploadCenterTypeFiles/Delete", PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 97, ParentId = 88, Icon = "menu-bullet menu-bullet-line", IsPage = false, Name = "استلام نقدية من عميل", OrderNum = 0, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 98, ParentId = 97, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "اضافة استلام نقدية", OrderNum = 0, Url = "/CustomersPayments/CreateEdit", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 99, ParentId = 97, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "ادارة استلام نقدية", OrderNum = 0, Url = "/CustomersPayments/Index", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 100, ParentId = 88, Icon = "menu-bullet menu-bullet-line", IsPage = false, Name = "استلام شيك من عميل", OrderNum = 0, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 101, ParentId = 100, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "اضافة استلام شيك ", OrderNum = 0, Url = "/CustomerCheques/CreateEdit", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 102, ParentId = 100, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "ادارة استلام شيك ", OrderNum = 0, Url = "/CustomerCheques/Index", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 103, ParentId = null, Icon = "icon-md fas fa-arrow-alt-circle-left ylow", IsPage = false, Name = "المبيعات", OrderNum = 10, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 104, ParentId = 103, Icon = "menu-bullet menu-bullet-line", IsPage = false, Name = "فواتير البيع ", OrderNum = 0, Url = "", OtherUrls = null, IsDeleted = true });
            pages.Add(new Page() { Id = 105, ParentId = 103, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "تسجيل فاتورة بيع جديدة", OrderNum = 0, Url = "/SellInvoices/CreateEdit", OtherUrls = ",/UploadCenterTypeFiles/Index,/UploadCenterTypeFiles/GetByInvoGuid,/UploadCenterTypeFiles/Delete", PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 106, ParentId = 103, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "إدارة فواتير البيع", OrderNum = 0, Url = "/SellInvoices/Index", OtherUrls = ",/SellInvoices/ShowHistory,/SellInvoices/ShowSellInvoice,/UploadCenterTypeFiles/Index,,/UploadCenterTypeFiles/GetByInvoGuid,/UploadCenterTypeFiles/Delete,/SellInvoices/ShowPrintSellInvoice,/PrintInvoices/ShowPrintInvoice", PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 107, ParentId = 103, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "الاعتماد المحاسبى لفاتورة البيع", OrderNum = 0, Url = "/SellInvoiceAccounting/Index", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 108, ParentId = 103, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "الاعتماد المخزنى لفاتورة البيع", OrderNum = 0, Url = "/SellInvoiceStores/Index", OtherUrls = "/SellInvoiceStores/ApprovalStore", PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 109, ParentId = 103, Icon = null, IsPage = true, Name = "الاعتماد النهائى لفاتورة البيع", OrderNum = 0, Url = "/SellInvoices/ApprovalFinalInvoice", OtherUrls = "/SellInvoices/PrintBarCode", PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 110, ParentId = null, Icon = "icon-md fas fa-undo ylow", IsPage = false, Name = "مرتجع المبيعات", OrderNum = 11, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 111, ParentId = 110, Icon = null, IsPage = false, Name = "فواتير مرتجع البيع", OrderNum = 0, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 112, ParentId = 111, Icon = null, IsPage = true, Name = "تسجيل فاتورة مرتجع بيع", OrderNum = 0, Url = "/SellBackInvoices/CreateEdit", OtherUrls = ",/UploadCenterTypeFiles/Index,/UploadCenterTypeFiles/GetByInvoGuid,/UploadCenterTypeFiles/Delete", PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 113, ParentId = 111, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "إدارة فواتير مرتجع البيع", OrderNum = 0, Url = "/SellBackInvoices/Index", OtherUrls = ",/SellBackInvoices/ShowHistory,/SellBackInvoices/ShowSellBackInvoice,/UploadCenterTypeFiles/Index,/UploadCenterTypeFiles/GetByInvoGuid,/UploadCenterTypeFiles/Delete,/PrintInvoices/ShowPrintInvoice", PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 114, ParentId = 111, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "الاعتماد المحاسبى لفاتورة مرتجع  البيع", OrderNum = 0, Url = "/SellBackInvoiceAccounting/Index", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 115, ParentId = 111, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "الاعتماد المخزنى لفاتورة مرتجع  البيع", OrderNum = 0, Url = "/SellBackInvoiceStores/Index", OtherUrls = "/SellBackInvoiceStores/ApprovalStore", PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 116, ParentId = 111, Icon = null, IsPage = true, Name = "الاعتماد النهائى لفاتورة مرتجع  البيع", OrderNum = 0, Url = "/SellBackInvoices/ApprovalFinalInvoice", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 117, ParentId = null, Icon = "icon-md fas fa-user-cog ylow", IsPage = false, Name = "اوامر الانتاج", OrderNum = 12, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 118, ParentId = 117, Icon = "menu-bullet menu-bullet-line", IsPage = false, Name = "تعريف ألوان اوامر الانتاج", OrderNum = 0, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 119, ParentId = 118, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "اضافة لون امر انتاج", OrderNum = 0, Url = "/ProductionOrderColors/CreateEdit", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 120, ParentId = 118, Icon = null, IsPage = true, Name = "ادارة لون امر انتاج", OrderNum = 0, Url = "/ProductionOrderColors/Index", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 121, ParentId = 117, Icon = null, IsPage = false, Name = "تعريف أمر إنتاج", OrderNum = 0, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 122, ParentId = 121, Icon = null, IsPage = true, Name = "اضافة أمر إنتاج", OrderNum = 0, Url = "/ProductionOrders/RegisterOrder", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 123, ParentId = 121, Icon = null, IsPage = true, Name = "ادارة أمر إنتاج", OrderNum = 0, Url = "/ProductionOrders/Index", OtherUrls = ",/ProductionOrders/ShowProductionOrder,/ProductionOrderDamages/Index", PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 124, ParentId = 121, Icon = "menu-bullet menu-bullet-line", IsPage = true,IsDeleted=true, Name = "استلام منتجات أمر إنتاج", OrderNum = 0, Url = "/ProductionOrderReceipts/Index", OtherUrls = ",/ProductionOrderReceipts/RegisterReceipts", PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 125, ParentId = null, Icon = "icon-md fas fa-warehouse ylow", IsPage = false, Name = "المخازن", OrderNum = 13, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 126, ParentId = 125, Icon = "menu-bullet menu-bullet-line", IsPage = false, Name = "تعريف المخازن", OrderNum = 0, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 127, ParentId = 126, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "اضافة مخزن جديد", OrderNum = 0, Url = "/Stores/CreateEdit", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 128, ParentId = 126, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "ادارة بيانات مخزن", OrderNum = 0, Url = "/Stores/Index", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 129, ParentId = 125, Icon = "menu-bullet menu-bullet-line", IsPage = false, Name = "التحويلات المخزنية", OrderNum = 0, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 130, ParentId = 129, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "تسجيل تحويل مخزنى ", OrderNum = 0, Url = "/StoresTransfers/CreateEdit", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 131, ParentId = 129, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "ادارة التحويلات بين المخازن", OrderNum = 0, Url = "/StoresTransfers/Index", OtherUrls = ",/StoresTransfers/ShowDetails,/StoresTransfers/ShowHistory", PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 132, ParentId = null, Icon = "icon-md fas fa-download ylow", IsPage = false, Name = "مركز التحميل", OrderNum = 14, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 133, ParentId = 132, Icon = null, IsPage = false, Name = "تعريف المجلدات", OrderNum = 0, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 134, ParentId = 133, Icon = null, IsPage = true, Name = "اضافة مجلد جديد", OrderNum = 0, Url = "/UploadCenterFolders/CreateEdit", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 135, ParentId = 133, Icon = null, IsPage = true, Name = "ادارة بيانات مجلد ", OrderNum = 0, Url = "/UploadCenterFolders/Index", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 136, ParentId = 132, Icon = "menu-bullet menu-bullet-line", IsPage = false, Name = "تعريف الملفات", OrderNum = 0, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 137, ParentId = 136, Icon = null, IsPage = true, Name = "اضافة ملف جديد", OrderNum = 0, Url = "/UploadCenterFiles/CreateEdit", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 138, ParentId = 136, Icon = null, IsPage = true, Name = "ادارة بيانات ملف ", OrderNum = 0, Url = "/UploadCenterFiles/Index", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 139, ParentId = null, Icon = "icon-md fas fa-users ylow", IsPage = false, Name = "الموارد البشرية", OrderNum = 15, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 140, ParentId = 139, Icon = null, IsPage = false, Name = "تعريف الادارات", OrderNum = 0, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 141, ParentId = 140, Icon = null, IsPage = true, Name = "اضافة ادارة جديدة", OrderNum = 0, Url = "/Departments/CreateEdit", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 142, ParentId = 140, Icon = null, IsPage = true, Name = "تعديل وحذف ادارة", OrderNum = 0, Url = "/Departments/Index", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 143, ParentId = 139, Icon = "menu-bullet menu-bullet-line", IsPage = false, Name = "تعريف الوظائف", OrderNum = 0, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 144, ParentId = 143, Icon = null, IsPage = true, Name = "اضافة وظيفة", OrderNum = 0, Url = "/Jobs/CreateEdit", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 145, ParentId = 143, Icon = null, IsPage = true, Name = "ادارة بيانات وظيفة", OrderNum = 0, Url = "/Jobs/Index", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 146, ParentId = 139, Icon = null, IsPage = false, Name = "تعريف الاجازات الرسمية للشركة", OrderNum = 0, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 147, ParentId = 146, Icon = null, IsPage = true, Name = "اضافة اجازة رسمية", OrderNum = 0, Url = "/VacationDays/CreateEdit", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 148, ParentId = 146, Icon = null, IsPage = true, Name = "ادارة اجازة رسمية", OrderNum = 0, Url = "/VacationDays/Index", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 149, ParentId = 139, Icon = null, IsPage = false, Name = "تعريف انواع التعاقدات", OrderNum = 0, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 150, ParentId = 149, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "اضافة نوع تعاقد", OrderNum = 0, Url = "/ContractTypes/CreateEdit", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 151, ParentId = 149, Icon = null, IsPage = true, Name = "ادارة نوع تعاقد", OrderNum = 0, Url = "/ContractTypes/Index", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 152, ParentId = 139, Icon = null, IsPage = false, Name = "تعريف انواع الاجازات", OrderNum = 0, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 153, ParentId = 152, Icon = null, IsPage = true, Name = "اضافة نوع اجازة", OrderNum = 0, Url = "/VacationTypes/CreateEdit", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 154, ParentId = 152, Icon = null, IsPage = true, Name = "ادارة نوع اجازة", OrderNum = 0, Url = "/VacationTypes/Index", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 155, ParentId = 139, Icon = null, IsPage = false, Name = "تعريف موظف", OrderNum = 0, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 156, ParentId = 155, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "اضافة موظف جديد", OrderNum = 0, Url = "/Employees/CreateEdit", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 157, ParentId = 155, Icon = null, IsPage = true, Name = "ادارة بيانات موظف", OrderNum = 0, Url = "/Employees/Index", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 158, ParentId = 139, Icon = null, IsPage = false, Name = "تعريف الفترات", OrderNum = 0, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 159, ParentId = 158, Icon = null, IsPage = true, Name = "اضافة فترة جديدة", OrderNum = 0, Url = "/WorkingPeriods/CreateEdit", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 160, ParentId = 158, Icon = null, IsPage = true, Name = "ادارة فترة ", OrderNum = 0, Url = "/WorkingPeriods/Index", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 161, ParentId = 139, Icon = null, IsPage = false, Name = "تعريف نظام الورديات", OrderNum = 0, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 162, ParentId = 161, Icon = null, IsPage = true, Name = "اضافة نظام وردية", OrderNum = 0, Url = "/Shifts/CreateEdit", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 163, ParentId = 161, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "ادارة نظام وردية", OrderNum = 0, Url = "/Shifts/Index", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 164, ParentId = 139, Icon = null, IsPage = false, Name = "تعريف أنواع الإضافات", OrderNum = 0, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 165, ParentId = 164, Icon = null, IsPage = true, Name = "اضافة نوع إضافة جديد", OrderNum = 0, Url = "/SalaryAdditionTypes/CreateEdit", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 166, ParentId = 164, Icon = null, IsPage = true, Name = "ادارة نوع إضافة ", OrderNum = 0, Url = "/SalaryAdditionTypes/Index", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 167, ParentId = 139, Icon = "menu-bullet menu-bullet-line", IsPage = false, Name = "تعريف الإضافات", OrderNum = 0, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 168, ParentId = 167, Icon = null, IsPage = true, Name = "تسجيل إضافة", OrderNum = 0, Url = "/SalaryAdditions/CreateEdit", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 169, ParentId = 167, Icon = null, IsPage = true, Name = "ادارة بيانات إضافة", OrderNum = 0, Url = "/SalaryAdditions/Index", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 170, ParentId = 139, Icon = null, IsPage = false, Name = "تعريف أنواع الخصومات", OrderNum = 0, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 171, ParentId = 170, Icon = null, IsPage = true, Name = "اضافة نوع خصم جديد", OrderNum = 0, Url = "/SalaryPenaltyTypes/CreateEdit", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 172, ParentId = 170, Icon = null, IsPage = true, Name = "ادارة نوع خصم ", OrderNum = 0, Url = "/SalaryPenaltyTypes/Index", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 173, ParentId = 139, Icon = null, IsPage = false, Name = "تعريف الخصومات", OrderNum = 0, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 174, ParentId = 173, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "تسجيل خصم جديد", OrderNum = 0, Url = "/SalaryPenalties/CreateEdit", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 175, ParentId = 173, Icon = null, IsPage = true, Name = "ادارة بيانات خصم ", OrderNum = 0, Url = "/SalaryPenalties/Index", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 176, ParentId = 139, Icon = null, IsPage = false, Name = "عقــود الموظفين", OrderNum = 0, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 177, ParentId = 176, Icon = null, IsPage = true, Name = "تسجيل عقد جديد لموظف", OrderNum = 0, Url = "/Contracts/CreateEdit", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 178, ParentId = 176, Icon = null, IsPage = true, Name = "ادارة بيانات عقد لموظف", OrderNum = 0, Url = "/Contracts/Index", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 179, ParentId = 139, Icon = null, IsPage = false, Name = "سلف/قروض الموظف", OrderNum = 0, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 180, ParentId = 179, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "تسجيل سلفه/قرض لموظف", OrderNum = 0, Url = "/ContractLoans/CreateEdit", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 181, ParentId = 179, Icon = null, IsPage = true, Name = "ادارة بيانات سلفه/قرض لموظف", OrderNum = 0, Url = "/ContractLoans/Index", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 182, ParentId = 139, Icon = null, IsPage = false, Name = "إضافات الموظفين", OrderNum = 0, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 183, ParentId = 182, Icon = null, IsPage = true, Name = "تسجيل إضافة لموظف", OrderNum = 0, Url = "/ContractSalaryAdditions/CreateEdit", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 184, ParentId = 182, Icon = null, IsPage = true, Name = "ادارة بيانات إضافة لموظف", OrderNum = 0, Url = "/ContractSalaryAdditions/Index", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 185, ParentId = 139, Icon = null, IsPage = false, Name = "خصومات الموظفين", OrderNum = 0, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 186, ParentId = 185, Icon = null, IsPage = true, Name = "تسجيل خصم على موظف", OrderNum = 0, Url = "/ContractSalaryPenalties/CreateEdit", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 187, ParentId = 185, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "ادارة بيانات خصم على موظف", OrderNum = 0, Url = "/ContractSalaryPenalties/Index", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 188, ParentId = 139, Icon = null, IsPage = false, Name = "حضور/انصراف الموظفين", OrderNum = 0, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 189, ParentId = 188, Icon = null, IsPage = true, Name = "تسجيل حضور/انصراف موظف", OrderNum = 0, Url = "/ContractAttendanceLeavings/CreateEdit", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 190, ParentId = 188, Icon = null, IsPage = true, Name = "ادارة حضور/انصراف موظف", OrderNum = 0, Url = "/ContractAttendanceLeavings/Index", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 191, ParentId = 139, Icon = "menu-bullet menu-bullet-line", IsPage = false, Name = "غياب الموظفين", OrderNum = 0, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 192, ParentId = 191, Icon = null, IsPage = true, Name = "تسجيل غياب موظف", OrderNum = 0, Url = "/ContractSchedulingAbsences/CreateEdit", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 193, ParentId = 139, Icon = null, IsPage = false, Name = "رواتب الموظفين", OrderNum = 0, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 194, ParentId = 193, Icon = null, IsPage = true, Name = "اعتماد راتب موظف", OrderNum = 0, Url = "/SalariesApproval/Index", OtherUrls = ",/SalariesApproval/CreateEdit", PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 195, ParentId = 193, Icon = null, IsPage = true, Name = "صرف رواتب الموظفين", OrderNum = 0, Url = "/SalariesPayed/Index", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 196, ParentId = null, Icon = "icon-md fas fa-file-invoice-dollar ylow", IsPage = false, Name = "دليل الحسابات", OrderNum = 16, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 197, ParentId = 196, Icon = null, IsPage = false, Name = "تسجيل الحسابات", OrderNum = 0, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 198, ParentId = 197, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "تسجيل حساب جديد", OrderNum = 0, Url = "/AccountsTrees/CreateEdit", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 199, ParentId = 197, Icon = null, IsPage = true, Name = "ادارة بيانات حساب", OrderNum = 0, Url = "/AccountsTrees/Index", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 200, ParentId = 197, Icon = null, IsPage = true, Name = "استعراض شجرة الحسابات", OrderNum = 0, Url = "/AccountsTrees/ShowAccounts", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 201, ParentId = 196, Icon = null, IsPage = false, Name = "القيود اليومية", OrderNum = 0, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 202, ParentId = 201, Icon = null, IsPage = true, Name = "تسجيل قيد يومية", OrderNum = 0, Url = "/GeneralRecords/CreateEdit", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 203, ParentId = 201, Icon = null, IsPage = true, Name = "ادارة قيد يومية", OrderNum = 0, Url = "/GeneralRecords/Index", OtherUrls = "/UploadCenterTypeFiles/Index,/UploadCenterTypeFiles/GetByInvoGuid,/UploadCenterTypeFiles/Delete,/PrintInvoices/PrintGeneralRecord", PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 204, ParentId = null, Icon = "icon-md fas fa-users-cog ylow", IsPage = false, Name = "الصلاحيات والمستخدمين", OrderNum = 17, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 205, ParentId = 204, Icon = null, IsPage = false, Name = "الصلاحيات", OrderNum = 0, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 206, ParentId = 205, Icon = null, IsPage = true, Name = "تسجيل صلاحية جديدة", OrderNum = 0, Url = "/Roles/CreateEdit", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 207, ParentId = 205, Icon = null, IsPage = true, Name = "ادارة بيانات صلاحية", OrderNum = 0, Url = "/Roles/Index", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 208, ParentId = 204, Icon = null, IsPage = false, Name = "اسناد الصلاحيات", OrderNum = 0, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 211, ParentId = 208, Icon = null, IsPage = true, Name = "اسناد صلاحية جديد", OrderNum = 0, Url = "/PagesRoles/AssignPages", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 212, ParentId = 208, Icon = null, IsPage = true, Name = "استعراض الصلاحيات ", OrderNum = 0, Url = "/PagesRoles/Index", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 217, ParentId = 204, Icon = null, IsPage = false, Name = "المستخدمين", OrderNum = 0, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 218, ParentId = 217, Icon = null, IsPage = true, Name = "تسجيل مستخدم جديد", OrderNum = 0, Url = "/UserRoles/CreateEdit", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 219, ParentId = 217, Icon = null, IsPage = true, Name = "ادارة بيانات مستخدم ", OrderNum = 0, Url = "/UserRoles/Index", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 220, ParentId = null, Icon = "icon-md fas fa-file-signature ylow", IsPage = false, Name = "التقارير", OrderNum = 19, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 221, ParentId = 220, Icon = null, IsPage = false, Name = "تقرير دليل الحسابات", OrderNum = 0, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 222, ParentId = 221, Icon = null, IsPage = true, Name = "ميزان المراجعة", OrderNum = 3, Url = "/AuditBalances/Index", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 223, ParentId = 221, Icon = null, IsPage = true, Name = "قائمة الدخل", OrderNum = 4, Url = "/IncomeLists/Index", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 224, ParentId = 221, Icon = null, IsPage = true, Name = "كشف حساب الاستاذ", OrderNum = 1, Url = "/RptAccountStatements/Search", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 225, ParentId = 221, Icon = null, IsPage = true, Name = "كشف حساب الاستاذ مجمع", OrderNum = 2, Url = "/RptAccountStatements/SearchMultiple", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 226, ParentId = 221, Icon = null, IsPage = true, Name = "بحث القيود", OrderNum = 0, Url = "/GeneralRecords/Search", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 227, ParentId = 220, Icon = null, IsPage = false, Name = "تقارير الفواتير", OrderNum = 0, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 228, ParentId = 285, Icon = null, IsPage = true, Name = "أرصده الأصناف", OrderNum = 0, Url = "/RptItemBalances/SearchItemBalance", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 229, ParentId = 227, Icon = null, IsPage = true, Name = "فواتير التوريد", OrderNum = 0, Url = "/RptPurchaseInvoices/SearchPurchases", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 230, ParentId = 227, Icon = null, IsPage = true, Name = "فواتير مرتجع التوريد", OrderNum = 0, Url = "/RptPurchaseInvoices/SearchBackPurchases", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 231, ParentId = 227, Icon = null, IsPage = true, Name = "فواتير البيع", OrderNum = 0, Url = "/RptSellInvoices/SearchSell", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 232, ParentId = 227, Icon = null, IsPage = true, Name = "فواتير مرتجع البيع ", OrderNum = 0, Url = "/RptSellInvoices/SearchBackSell", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 233, ParentId = 139, Icon = null, IsPage = false, Name = "تعريف نظام التأخير والغياب", OrderNum = 0, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 234, ParentId = 233, Icon = null, IsPage = true, Name = "تسجيل نظام التأخير والغياب", OrderNum = 0, Url = "/DelayAbsenceSystems/CreateEdit", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 235, ParentId = 233, Icon = null, IsPage = true, Name = "ادارة نظام التأخير والغياب", OrderNum = 0, Url = "/DelayAbsenceSystems/Index", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 236, ParentId = null, Icon = "icon-md fas fa-coins ylow", IsPage = false, Name = "المصروفات والايرادات", OrderNum = 2, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 237, ParentId = 236, Icon = null, IsPage = false, Name = "تعريف انواع المصروفات", OrderNum = 0, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 238, ParentId = 237, Icon = null, IsPage = true, Name = "اضافة نوع مصروف", OrderNum = 0, Url = "/ExpenseTypes/CreateEdit", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 239, ParentId = 237, Icon = null, IsPage = true, Name = "ادارة نوع مصروف ", OrderNum = 0, Url = "/ExpenseTypes/Index", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 240, ParentId = 236, Icon = "menu-bullet menu-bullet-line", IsPage = false, Name = "تعريف انواع الايرادات", OrderNum = 0, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 241, ParentId = 240, Icon = null, IsPage = true, Name = "اضافه نوع ايراد", OrderNum = 0, Url = "/IncomeTypes/CreateEdit", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 242, ParentId = 240, Icon = null, IsPage = true, Name = "ادارة نوع ايراد", OrderNum = 0, Url = "/IncomeTypes/Index", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 243, ParentId = 196, Icon = null, IsPage = false, Name = "تعريف الأصول الثابتة", OrderNum = 0, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 244, ParentId = 243, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "اضافة أصل ثابت", OrderNum = 0, Url = "/FixedAssets/CreateEdit", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 245, ParentId = 243, Icon = null, IsPage = true, Name = "ادارة بيانات أصل", OrderNum = 0, Url = "/FixedAssets/Index", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 246, ParentId = 139, Icon = "menu-bullet menu-bullet-line", IsPage = false, Name = "عــٌهد الموظفين", OrderNum = 0, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 247, ParentId = 246, Icon = null, IsPage = true, Name = "صرف عهدة لموظف", OrderNum = 0, Url = "/EmployeeGivingCustody/CreateEdit", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 248, ParentId = 246, Icon = null, IsPage = true, Name = "ادارة صرف عهدة", OrderNum = 0, Url = "/EmployeeGivingCustody/Index", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 249, ParentId = 139, Icon = "menu-bullet menu-bullet-line", IsPage = false, Name = "مصروفات عهدة موظف", OrderNum = 0, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 250, ParentId = 249, Icon = null, IsPage = true, Name = "تسجيل مصروف عهدة موظف", OrderNum = 0, Url = "/EmployeeReturnCustody/CreateEdit", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 251, ParentId = 249, Icon = null, IsPage = true, Name = "ادارة مصروف عهدة", OrderNum = 0, Url = "/EmployeeReturnCustody/Index", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 252, ParentId = 139, Icon = "menu-bullet menu-bullet-line", IsPage = false, Name = "ارجاع نقدى/بنكى لعهدة موظف", OrderNum = 0, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 253, ParentId = 252, Icon = null, IsPage = true, Name = "تسجيل ارجاع نقدى/بنكى", OrderNum = 0, Url = "/EmployeeReturnCashCustody/CreateEdit", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 254, ParentId = 252, Icon = null, IsPage = true, Name = "ادارة ارجاع نقدى/بنكى ", OrderNum = 0, Url = "/EmployeeReturnCashCustody/Index", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 255, ParentId = null, Icon = "icon-md fas fa-user-tag ylow", IsPage = false, Name = "المناديب", OrderNum = 18, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 256, ParentId = 255, Icon = null, IsPage = false, Name = "عملاء المناديب", OrderNum = 0, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 257, ParentId = 256, Icon = null, IsPage = true, Name = "تسجيل عملاء مندوب", OrderNum = 0, Url = "/SaleMenCustomers/CreateEdit", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 258, ParentId = 256, Icon = null, IsPage = true, Name = "ادارة عملاء مندوب", OrderNum = 0, Url = "/SaleMenCustomers/Index", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 259, ParentId = 255, Icon = "menu-bullet menu-bullet-line", IsPage = false, Name = "فواتير بيع المناديب", OrderNum = 0, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 260, ParentId = 259, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "تسجيل فاتورة بيع مندوب جديدة", OrderNum = 0, Url = "/SaleMenSellInvoices/CreateEdit", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 261, ParentId = 259, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "إدارة فواتير بيع المندوب", OrderNum = 0, Url = "/SaleMenSellInvoices/Index", OtherUrls = ",/SaleMenSellInvoices/ShowSaleMenSellInvoice", PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 262, ParentId = 255, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "تعريف مخازن المناديب", OrderNum = 0, Url = "/SaleMenStores/Index", OtherUrls = ",/SaleMenStores/CreateEdit,/SaleMenStores/ShowHistory", PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 263, ParentId = 255, Icon = "menu-bullet menu-bullet-line", IsPage = false, Name = "فواتير مرتجع بيع المناديب", OrderNum = 0, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 264, ParentId = 263, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "تسجيل فاتورة مرتجع بيع مندوب ", OrderNum = 0, Url = "/SaleMenSellBackInvoices/CreateEdit", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 265, ParentId = 263, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "إدارة فواتير مرتجع بيع المندوب", OrderNum = 0, Url = "/SaleMenSellBackInvoices/Index", OtherUrls = ",/SaleMenSellBackInvoices/ShowSaleMenSellBackInvoice", PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 266, ParentId = 255, Icon = "menu-bullet menu-bullet-line", IsPage = false, Name = "استلام نقدية من عميل", OrderNum = 0, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 267, ParentId = 266, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "اضافة استلام نقدية عميل من مندوب", OrderNum = 0, Url = "/SaleMenCustomerPayments/CreateEdit", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 268, ParentId = 49, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "تسجيل سياسات اسعار العملاء", OrderNum = 0, Url = "/ItemPrices/CreateEdit", OtherUrls = "/ItemPrices/AddItemPriceCustomer", PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 269, ParentId = null, Icon = "icon-md fas fa-cog ylow", IsPage = false, Name = "الصيانه", OrderNum = 20, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 270, ParentId = 269, Icon = null, IsPage = false, Name = "تعريف انواع الاعطال", OrderNum = 0, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 271, ParentId = 270, Icon = null, IsPage = true, Name = "تسجيل نوع عطل", OrderNum = 0, Url = "/MaintenProblemTypes/CreateEdit", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 272, ParentId = 270, Icon = null, IsPage = true, Name = "ادارة نوع عطل", OrderNum = 0, Url = "/MaintenProblemTypes/Index", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 273, ParentId = 266, Icon = null, IsPage = true, Name = "ادارة استلام نقدية عميل من مندوب", OrderNum = 0, Url = "/SaleMenCustomerPayments/Index", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 274, ParentId = 255, Icon = null, IsPage = false, Name = "استلام مندوب شيك من عميل", OrderNum = 0, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 275, ParentId = 274, Icon = null, IsPage = true, Name = "تسجيل استلام مندوب شيك ", OrderNum = 0, Url = "/SaleMenCustomerCheques/CreateEdit", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 276, ParentId = 274, Icon = null, IsPage = true, Name = "ادارة استلام مندوب شيك", OrderNum = 0, Url = "/SaleMenCustomerCheques/Index", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 277, ParentId = null, Icon = "icon-md fas fa-bell ylow", IsPage = false, Name = "الاشعارات", OrderNum = 21, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 278, ParentId = 277, Icon = "menu-bullet menu-bullet-line", IsPage = false, Name = "تعريف المصروفات الدورية", OrderNum = 0, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 279, ParentId = 278, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "اضافة مصروف دورى جديد", OrderNum = 0, Url = "/PeriodicExpenses/CreateEdit", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 280, ParentId = 278, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "ادارة مصروف دورى", OrderNum = 0, Url = "/PeriodicExpenses/Index", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 281, ParentId = 277, Icon = "menu-bullet menu-bullet-line", IsPage = false, Name = "تعريف الايرادات الدورية", OrderNum = 0, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 282, ParentId = 281, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "اضافة إيراد دورى جديد", OrderNum = 0, Url = "/PeriodicIncomes/CreateEdit", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 283, ParentId = 281, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "ادارة إيراد دورى", OrderNum = 0, Url = "/PeriodicIncomes/Index", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 284, ParentId = 277, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "استعراض الاشعارات", OrderNum = 0, Url = "/Notifications/Index", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 285, ParentId = 220, Icon = "menu-bullet menu-bullet-line", IsPage = false, Name = "تقارير الاصناف", OrderNum = 0, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 286, ParentId = 285, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "هوالك وتوالف اوامر الانتاج", OrderNum = 0, Url = "/RptQuantityDamages/Index", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 287, ParentId = 285, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "متوسط انتاج منتج", OrderNum = 0, Url = "/RptQuantityItemProductions/Index", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 288, ParentId = 255, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "اعتماد التحويلات المخزنية", OrderNum = 0, Url = "/SaleMenStoreTransfers/Index", OtherUrls = ",/SaleMenStoreTransfers/ShowDetails",IsDeleted=true, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 289, ParentId = 269, Icon = null, IsPage = false, Name = "فواتير الصيانة", OrderNum = 0, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 290, ParentId = 289, Icon = null, IsPage = true, Name = "تسجيل فاتورة صيانة", OrderNum = 0, Url = "/Maintenances/CreateEdit", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 291, ParentId = 289, Icon = null, IsPage = true, Name = "ادارة فاتورة صيانة", OrderNum = 0, Url = "/Maintenances/Index", OtherUrls = ",/Maintenances/ShowHistory,/Maintenances/ShowMaintenance,/MaintenanceItems/Index,/MaintenanceItems/SaveItemData", PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 292, ParentId = 289, Icon = null, IsPage = true, Name = "الاعتماد المحاسبى ", OrderNum = 0, Url = "/MaintenancesAccountants/Index", OtherUrls = "/MaintenancesAccountants/ApprovalAccountant,,/MaintenancesAccountants/ShowMaintenance", PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 293, ParentId = 289, Icon = null, IsPage = true, Name = "الاعتماد المخزنى", OrderNum = 0, Url = "/MaintenancesStores/Index", OtherUrls = "/MaintenancesStores/ShowMaintenanceStore", PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 294, ParentId = 289, Icon = null, IsPage = true, Name = "الاعتماد النهائى", OrderNum = 0, Url = "/MaintenancesFinalApprovals/Index", OtherUrls = "/MaintenancesFinalApprovals/PrintBarCode", PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 295, ParentId = 285, Icon = null, IsPage = true, Name = "بحث عن صنف بسيريال", OrderNum = 0, Url = "/RptItemBalances/SearchItemBySerial", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 296, ParentId = 220, Icon = null, IsPage = false, Name = "تقارير تفصيلية", OrderNum = 0, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 297, ParentId = 296, Icon = null, IsPage = true, Name = "تقرير مندوب", OrderNum = 0, Url = "/SaleMenAccounts/Index", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 298, ParentId = 220, Icon = null, IsPage = false, Name = "التقارير المالية", OrderNum = 0, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 299, ParentId = 298, Icon = null, IsPage = true, Name = "التحصيلات النقدية من العملاء", OrderNum = 0, Url = "/RptCustomerPayments/SearchPayments", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 300, ParentId = 298, Icon = null, IsPage = true, Name = "الشيكات من العملاء", OrderNum = 0, Url = "/RptCustomerPayments/SearchCheques", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 301, ParentId = 298, Icon = null, IsPage = true, Name = "متأخرات تحصيل بيع", OrderNum = 0, Url = "/RptDueInvoices/SearchDueSell", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 302, ParentId = 285, Icon = null, IsPage = true, Name = "حركة صنف", OrderNum = 0, Url = "/RptItemBalances/SearchItemAction", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 303, ParentId = 220, Icon = null, IsPage = false, Name = "تقارير الانتاج", OrderNum = 0, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 304, ParentId = 303, Icon = null, IsPage = true, Name = "تقرير اصناف الانتاج", OrderNum = 2, Url = "/RptProductionOrders/SearchProductionOrders", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 305, ParentId = 220, Icon = null, IsPage = false, Name = "تقارير الصيانة", OrderNum = 0, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 306, ParentId = 305, Icon = null, IsPage = true, Name = "فواتير الصيانة", OrderNum = 0, Url = "/RptMaintenances/SearchMaintenance", OtherUrls = ",/Maintenances/ShowHistory,/Maintenances/ShowMaintenance,", PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 307, ParentId = 305, Icon = null, IsPage = true, Name = "تقرير الاعطال ", OrderNum = 0, Url = "/RptMaintenances/SearchMaintenanceProblem", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 308, ParentId = 305, Icon = null, IsPage = true, Name = "احصاء الاعطال", OrderNum = 0, Url = "/RptMaintenances/MaintenanceProblemStatistics", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 309, ParentId = 191, Icon = null, IsPage = true, Name = "ادارة غياب موظف", OrderNum = 0, Url = "/ContractSchedulingAbsences/Index", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 310, ParentId = 221, Icon = null, IsPage = true, Name = "حساب المتاجرة", OrderNum = 6, Url = "/TradingAccounts/Index", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 311, ParentId = 221, Icon = null, IsPage = true, Name = "حساب ارباح وخسائر", OrderNum = 7, Url = "/ProfitLossAccounts/Index", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 312, ParentId = 236, Icon = null, IsPage = false, Name = "تعريف المصروفات", OrderNum = 0, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 313, ParentId = 312, Icon = null, IsPage = true, Name = "تسجيل مصروف", OrderNum = 0, Url = "/Expenses/CreateEdit", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 314, ParentId = 312, Icon = null, IsPage = true, Name = "ادارة مصروف", OrderNum = 0, Url = "/Expenses/Index", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 315, ParentId = 236, Icon = null, IsPage = false, Name = "تعريف الايرادات", OrderNum = 0, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 316, ParentId = 315, Icon = null, IsPage = true, Name = "تسجيل إيراد", OrderNum = 0, Url = "/Incomes/CreateEdit", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 317, ParentId = 315, Icon = null, IsPage = true, Name = "ادارة إيراد", OrderNum = 0, Url = "/Incomes/Index", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 318, ParentId = 285, Icon = null, IsPage = true, Name = "اصناف بلغت حد الطلب الامن", OrderNum = 0, Url = "/RptLimitItems/SearchItemSafety", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 319, ParentId = 285, Icon = null, IsPage = true, Name = "اصناف بلغت حد الطلب الخطر", OrderNum = 0, Url = "/RptLimitItems/SearchItemDanger", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 320, ParentId = 125, Icon = null, IsPage = false, Name = "جرد المخازن", OrderNum = 0, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 321, ParentId = 320, Icon = null, IsPage = true, Name = "تسجيل فاتورة جرد", OrderNum = 0, Url = "/InventoryInvoices/NewInventoryInvoice", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 322, ParentId = 320, Icon = null, IsPage = true, Name = "ادارة فاتورة جرد", OrderNum = 0, Url = "/InventoryInvoices/Index", OtherUrls = "/InventoryInvoices/ShowInventoryInvoice", PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 323, ParentId = 221, Icon = null, IsPage = true, Name = "المركز المالى", OrderNum = 5, Url = "/FinancialCenter/Index", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 324, ParentId = 48, Icon = "menu-bullet menu-bullet-line", IsPage = false, Name = "رصيد اول المدة للأصول", OrderNum = 0, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 325, ParentId = 324, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "اضافة رصيد اول المدة للأصول", OrderNum = 0, Url = "/AssetIntialBalances/CreateEdit", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 326, ParentId = 324, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "ادارة رصيد اول المدة للأصول", OrderNum = 0, Url = "/AssetIntialBalances/Index", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 327, ParentId = 48, Icon = "menu-bullet menu-bullet-line", IsPage = false, Name = "رصيد اول المدة لحقوق الملكية", OrderNum = 0, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 328, ParentId = 327, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "اضافة رصيد اول المدة لحقوق الملكية", OrderNum = 0, Url = "/PropertyRightsIntialBalances/CreateEdit", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 329, ParentId = 327, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "ادارة رصيد اول المدة لحقوق الملكية", OrderNum = 0, Url = "/PropertyRightsIntialBalances/Index", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            //pages.Add(new Page() { Id = 330, ParentId = 88, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "دفعات الفواتير الاجل", OrderNum = 0, Url = "/SellInvoicePayments/Index", OtherUrls = "/SellInvoicePayments/RegisterPyments", PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 331, ParentId = 332, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "انشاء أقساط/مبيعات آجله", OrderNum = 0, Url = "/SellInvoiceInstallments/Index", OtherUrls = "/SellInvoiceInstallments/RegisterInstallments ", PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 332, ParentId = null, Icon = "icon-md fas fa-cog ylow", IsPage = false, Name = "التقسيط", OrderNum = 11, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 333, ParentId = 332, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "حاسبة قسط", OrderNum = 0, Url = "/SellInvoiceInstallments/Calculate", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 334, ParentId = 201, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "تسجيل قيود يومية مركبة",IsDeleted=true, OrderNum = 0, Url = "/GeneralRecordComplex/CreateEdit", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 335, ParentId = 332, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "تحصيل اقساط", OrderNum = 0, Url = "/SellInvoiceInstallmentSchedules/Index", OtherUrls = "/SellInvoiceInstallmentSchedules/Paid,/UploadCenterTypeFiles/Index,/UploadCenterTypeFiles/GetByInvoGuid,/UploadCenterTypeFiles/Delete", PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 336, ParentId = 332, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "اقساط عميل", OrderNum = 0, Url = "/SellInvoiceHasInstallments/Index", OtherUrls = "/SellInvoiceHasInstallments/Installments", PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 337, ParentId = 296, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "تقرير مورد", OrderNum = 0, Url = "/SupplierAccounts/Index", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 338, ParentId = 296, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "تقرير عميل", OrderNum = 0, Url = "/CustomerAccounts/Index", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 339, ParentId = 332, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "عملاء لم يسددوا", OrderNum = 0, Url = "/SellInvoiceInstallmentNotPaid/Index", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 340, ParentId = null, Icon = "icon-md fas fa-coins ylow", IsPage = false, Name = "سندات الصرف والقبض", OrderNum = 3, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 341, ParentId = 340, Icon = null, IsPage = false, Name = "تعريف سند صرف", OrderNum = 0, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 342, ParentId = 341, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "اضافة سند صرف", OrderNum = 0, Url = "/VoucherPayments/CreateEdit", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 343, ParentId = 341, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "ادارة سند صرف", OrderNum = 0, Url = "/VoucherPayments/Index", OtherUrls = ",/PrintInvoices/PrintGeneralRecord", PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 344, ParentId = 340, Icon = "menu-bullet menu-bullet-line", IsPage = false, Name = "تعريف سند قبض", OrderNum = 0, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 345, ParentId = 344, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "اضافة سند قبض", OrderNum = 0, Url = "/VoucherReceipts/CreateEdit", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 346, ParentId = 344, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "ادارة سند قبض", OrderNum = 0, Url = "/VoucherReceipts/Index", OtherUrls = ",/PrintInvoices/PrintGeneralRecord", PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 347, ParentId = 49, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "تسجيل وحدات الاصناف", OrderNum = 0, Url = "/ItemUnits/CreateEdit", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 348, ParentId = 285, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "تقرير المخزون", OrderNum = 0, Url = "/RptItemStockBalances/Index", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 349, ParentId = 221, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "تقرير ارصدة الموردين", OrderNum = 8, Url = "/RptAccountStatements/BalanceSuppliers", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 350, ParentId = 221, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "تقرير ارصدة العملاء", OrderNum = 9, Url = "/RptAccountStatements/BalanceCustomers", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 351, ParentId = 285, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "حركة صنف 2", OrderNum = 0, Url = "/RptItemInvoices/SearchInvoices", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 352, ParentId = 66, Icon = "icon-md fas fa-coins ylow", IsPage = false, Name = "شيك الى مورد", OrderNum = 0, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 353, ParentId = 352, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "اضافة شيك الى مورد", OrderNum = 0, Url = "/SupplierCheques/CreateEdit", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 354, ParentId = 352, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "ادارة شيك الى مورد", OrderNum = 0, Url = "/SupplierCheques/Index", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 355, ParentId = 285, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "اصناف عينات وهدايا من مورد", OrderNum = 0, Url = "/RptGiftItems/SearchItemGiftPurchase", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 356, ParentId = 285, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "اصناف عينات وهدايا لعميل", OrderNum = 0, Url = "/RptGiftItems/SearchItemGiftSell", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 357, ParentId = 285, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "تقرير الاعلى والاقل مبيعا ", OrderNum = 0, Url = "/RptLimitItems/ItemsTopLowestSelling", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 358, ParentId = 125, Icon = "menu-bullet menu-bullet-line", IsPage = false, Name = "الهوالك", OrderNum = 0, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 359, ParentId = 358, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "تسجيل فاتورة هالك", OrderNum = 0, Url = "/DamageInvoices/NewDamageInvoice", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 360, ParentId = 358, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "ادارة فاتورة هالك", OrderNum = 0, Url = "/DamageInvoices/Index", OtherUrls = "/DamageInvoices/ShowDamageInvoice", PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 361, ParentId = 49, Icon = "menu-bullet menu-bullet-line", IsPage = false, Name = "تحديد سعر بيع الاصناف", OrderNum = 0, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 362, ParentId = 361, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "تسجيل سعر بيع", OrderNum = 0, Url = "/ItemCustomSellPrices/CreateEdit", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 363, ParentId = 361, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "ادارة سعر بيع", OrderNum = 0, Url = "/ItemCustomSellPrices/Index", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 364, ParentId = 277, Icon = "menu-bullet menu-bullet-line", IsPage = false, Name = "تعريف المهام الادارية", OrderNum = 0, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 365, ParentId = 364, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "اضافة مهمة ادارية جديدة", OrderNum = 0, Url = "/NotificationTasks/CreateEdit", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 366, ParentId = 364, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "ادارة مهمة ادارية", OrderNum = 0, Url = "/NotificationTasks/Index", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 367, ParentId = 66, Icon = "menu-bullet menu-bullet-line", IsPage = false, Name = "تعريف فئات الموردين", OrderNum = 0, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 368, ParentId = 367, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "اضافة فئة الموردين", OrderNum = 0, Url = "/SupplierCategories/CreateEdit", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 369, ParentId = 367, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "ادارة فئة الموردين", OrderNum = 0, Url = "/SupplierCategories/Index", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 370, ParentId = 1, Icon = "menu-bullet menu-bullet-line", IsPage = false, Name = "تعريف منطقة", OrderNum = 0, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 371, ParentId = 370, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "اضافة منطقة", OrderNum = 0, Url = "/Regions/CreateEdit", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 372, ParentId = 370, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "ادارة منطقة", OrderNum = 0, Url = "/Regions/Index", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 373, ParentId = 1, Icon = "menu-bullet menu-bullet-line", IsPage = false, Name = "تعريف حي", OrderNum = 0, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 374, ParentId = 373, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "اضافة حي", OrderNum = 0, Url = "/Districts/CreateEdit", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 375, ParentId = 373, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "ادارة حي", OrderNum = 0, Url = "/Districts/Index", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 376, ParentId = 125, Icon = "menu-bullet menu-bullet-line", IsPage = false, Name = "تعريف اذن استلام", OrderNum = 0, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 377, ParentId = 376, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "اضافة اذن استلام", OrderNum = 0, Url = "/StorePermissionsReceive/CreateEdit", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 378, ParentId = 376, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "ادارة اذن استلام", OrderNum = 0, Url = "/StorePermissionsReceive/Index", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 379, ParentId = 125, Icon = "menu-bullet menu-bullet-line", IsPage = false, Name = "تعريف اذن صرف", OrderNum = 0, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 380, ParentId = 379, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "اضافة اذن صرف", OrderNum = 0, Url = "/StorePermissionsLeave/CreateEdit", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 381, ParentId = 379, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "ادارة اذن صرف", OrderNum = 0, Url = "/StorePermissionsLeave/Index", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 382, ParentId = 285, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "كارت صنف بالتكلفة", OrderNum = 0, Url = "/RptItemBalances/SearchItemBalanceMovement", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 383, ParentId = 33, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = " اضافة رصيد أول المدة للعملاء مجمع", OrderNum = 0, Url = "/CustomerIntials/CreateEditIntial", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 384, ParentId = 30, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = " اضافة رصيد أول المدة للموردين مجمع", OrderNum = 0, Url = "/SupplierIntials/CreateEditIntial", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 385, ParentId = 129, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "الاعتماد المخزنى ", OrderNum = 0, Url = "/StoresTransferApprovals/Index", OtherUrls = "/StoresTransferApprovals/ApprovalStore,/StoresTransferApprovals/ShowDetails,/StoresTransferApprovals/Refused", PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 386, ParentId = 129, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "الاعتماد النهائى ", OrderNum = 0, Url = "/StoresTransfers/ApprovalFinalInvoice", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 387, ParentId = 49, Icon = "menu-bullet menu-bullet-line", IsPage = false, Name = "تسجيل العروض", OrderNum = 0, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 388, ParentId = 387, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "اضافة عرض", OrderNum = 0, Url = "/Offers/CreateEdit", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 389, ParentId = 387, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "ادارة بيانات عرض", OrderNum = 0, Url = "/Offers/Index", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 390, ParentId = 88, Icon = "menu-bullet menu-bullet-line", IsPage = false, Name = "مدة استحقاق السداد", OrderNum = 0, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 391, ParentId = 390, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "اضافة مدة استحقاق", OrderNum = 0, Url = "/ContractCustomers/CreateEdit", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 392, ParentId = 390, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "ادارة مدة استحقاق", OrderNum = 0, Url = "/ContractCustomers/Index", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 393, ParentId = 66, Icon = "icon-md fas fa-coins ylow", IsPage = false, Name = "مدة استحقاق السداد", OrderNum = 0, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 394, ParentId = 393, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "اضافة مدة استحقاق", OrderNum = 0, Url = "/ContractSuppliers/CreateEdit", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 395, ParentId = 393, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "ادارة مدة استحقاق", OrderNum = 0, Url = "/ContractSuppliers/Index", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 396, ParentId = 49, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "تسجيل سياسات اسعار الموردين", OrderNum = 0, Url = "/ItemPriceSuppliers/CreateEdit", OtherUrls = "/ItemPriceSuppliers/AddItemPriceSupplier", PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 397, ParentId = 50, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "استعراض المجموعات", OrderNum = 0, Url = "/ItemGroups/ShowGroups", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 398, ParentId = 88, Icon = "menu-bullet menu-bullet-line", IsPage = false, Name = "استلام نقدية مخصصة لفاتورة", OrderNum = 0, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 399, ParentId = 398, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "اضافة دفعة مخصصة", OrderNum = 0, Url = "/SellInvoicePayments/CreateEdit", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 400, ParentId = 398, Icon = null, IsPage = true, Name = "ادارة دفعة مخصصة", OrderNum = 0, Url = "/SellInvoicePayments/Index", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 401, ParentId = 298, Icon = null, IsPage = true, Name = "تقرير أعمار الديون", OrderNum = 0, Url = "/RptDueInvoices/AgesDebt", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 402, ParentId = null, Icon = "icon-md fas fa-coins ylow", IsPage = false, Name = "عروض الاسعار وأوامر البيع", OrderNum = 3, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 403, ParentId = 402, Icon = null, IsPage = false, Name = "عروض الاسعار", OrderNum = 0, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 404, ParentId = 403, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "تسجيل عرض سعر", OrderNum = 0, Url = "/Quotes/CreateEdit", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 405, ParentId = 403, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "ادارة عرض سعر", OrderNum = 0, Url = "/Quotes/Index", OtherUrls = "/OrderSells/CreateEdit,/PrintInvoices/ShowPrintInvoice", PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 406, ParentId = 402, Icon = "menu-bullet menu-bullet-line", IsPage = false, Name = "أوامر البيع", OrderNum = 0, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 408, ParentId = 406, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "ادارة أوامر البيع", OrderNum = 0, Url = "/OrderSells/Index", OtherUrls = "/OrderSells/OrderForSell,/OrderSells/OrderForProduction,/PrintInvoices/ShowPrintInvoice", PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 409, ParentId = 285, Icon = null, IsPage = true, Name = "الاصناف المباعه خلال فترة", OrderNum = 0, Url = "/RptItems/SearchItemSell", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 410, ParentId = 117, Icon = null, IsPage = false, Name = "خطوط الانتاج", OrderNum = 0, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 411, ParentId = 410, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "تسجيل خط انتاج", OrderNum = 0, Url = "/ProductionLines/CreateEdit", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 412, ParentId = 410, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "ادارة خط انتاج", OrderNum = 0, Url = "/ProductionLines/Index", OtherUrls = "/ProductionLines/ShowDetails", PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 413, ParentId = 303, Icon = null, IsPage = true, Name = "تقرير موظف خط الانتاج", OrderNum = 0, Url = "/RptProductionLines/SearchProductionOrderEmployee", OtherUrls = "/ProductionOrders/ShowProductionOrder,/ProductionLines/ShowDetails", PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 414, ParentId = 303, Icon = null, IsPage = true, Name = "تقرير خطوط الانتاج", OrderNum = 1, Url = "/RptProductionLines/SearchProductionLine", OtherUrls = "/ProductionOrders/ShowProductionOrder,/ProductionLines/ShowDetails", PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 415, ParentId = 121, Icon = null, IsPage = true, Name = "اضافة أمر إنتاج مجمع", OrderNum = 0, Url = "/ProductionOrders/RegisterOrderComplex", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 416, ParentId = 285, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "كارت صنف بدون التكلفة", OrderNum = 0, Url = "/RptItemBalances/SearchItemBalanceNotMovement", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 417, ParentId = 48, Icon = "menu-bullet menu-bullet-line", IsPage = false, Name = "رصيد اول المدة", OrderNum = 1, Url = "", OtherUrls = null });
            pages.Add(new Page() { Id = 418, ParentId = 417, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "اضافة رصيد اول المدة", OrderNum = 1, Url = "/AccountTreeIntialBalances/CreateEdit", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            pages.Add(new Page() { Id = 419, ParentId = 417, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "ادارة رصيد اول المدة", OrderNum = 1, Url = "/AccountTreeIntialBalances/Index", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });



            //INSERT[dbo].[Pages]([Id], [ParentId], [Name], [Url], [OtherUrls], [Icon], [IsPage], [OrderNum], [CreatedBy], [CreatedOn], [ModifiedBy], [ModifiedOn], [DeletedBy], [DeletedOn], [IsDeleted]) VALUES(387, 49, N'تسجيل العروض', N'', NULL, NULL, 0, 0, NULL, CAST(N'2021-11-27T09:21:29.837' AS DateTime), NULL, CAST(N'2021-11-27T09:21:29.837' AS DateTime), NULL, CAST(N'2021-11-27T09:21:29.837' AS DateTime), 1)
            //INSERT[dbo].[Pages]([Id], [ParentId], [Name], [Url], [OtherUrls], [Icon], [IsPage], [OrderNum], [CreatedBy], [CreatedOn], [ModifiedBy], [ModifiedOn], [DeletedBy], [DeletedOn], [IsDeleted]) VALUES(388, 387, N'اضافة عرض', N'/Offers/CreateEdit', NULL, NULL, 1, 0, NULL, CAST(N'2021-11-27T09:21:29.837' AS DateTime), NULL, CAST(N'2021-11-27T09:21:29.837' AS DateTime), NULL, CAST(N'2021-11-27T09:21:29.837' AS DateTime), 1)
            //INSERT[dbo].[Pages]([Id], [ParentId], [Name], [Url], [OtherUrls], [Icon], [IsPage], [OrderNum], [CreatedBy], [CreatedOn], [ModifiedBy], [ModifiedOn], [DeletedBy], [DeletedOn], [IsDeleted]) VALUES(389, 387, N'ادارة بيانات عرض', N'/Offers/Index', NULL, NULL, 1, 0, NULL, CAST(N'2021-11-27T09:21:29.837' AS DateTime), NULL, CAST(N'2021-11-27T09:21:29.837' AS DateTime), NULL, CAST(N'2021-11-27T09:21:29.837' AS DateTime), 1)

            //INSERT[dbo].[PagesRoles]([Id], [RoleId], [PageId], [CreatedBy], [CreatedOn], [ModifiedBy], [ModifiedOn], [DeletedBy], [DeletedOn], [IsDeleted]) VALUES('ABBA0D73-9081-4843-8B0A-FFB2FEB54307', 'B55F8D95-96DC-47A1-AD3A-98EB6CBFC8B1', 388, NULL, CAST(N'2022-04-18T03:16:15.153' AS DateTime), NULL, NULL, NULL, NULL, 0)
            //INSERT[dbo].[PagesRoles]([Id], [RoleId], [PageId], [CreatedBy], [CreatedOn], [ModifiedBy], [ModifiedOn], [DeletedBy], [DeletedOn], [IsDeleted]) VALUES('ABBA0D73-9081-4843-8B0A-FFB2FEB54317', 'B55F8D95-96DC-47A1-AD3A-98EB6CBFC8B1', 389, NULL, CAST(N'2022-04-18T03:16:15.153' AS DateTime), NULL, NULL, NULL, NULL, 0)
            //UPDATE [dbo].[Pages] SET [Name]=N'كارت صنف بالتكلفة' WHERE Id=382

            #endregion
            db.Pages.AddRange(pages);
            db.SaveChanges(userId);
            //if (db.Pages.Count() != pages.Count)
            //{
            //    var dbPages = db.Pages.ToList();
            //    foreach (var page in pages)
            //    {
            //        //بيدور علي الشاشات اللي مش موجودة و بيضفها
            //        var dbPage = dbPages.FirstOrDefault(x => x.Id == page.Id);
            //        if (dbPage == null)
            //        {
            //            db.Pages.Add(page);
            //        }

            //        //بيدور اذا كان الصفحة موجودة بس في حاجة مختلفة بيعدل علي اللي موجود
            //        else if (dbPage.Name != page.Name || dbPage.Icon != page.Icon || dbPage.IsPage != page.IsPage || dbPage.OrderNum != page.OrderNum
            //            || dbPage.OtherUrls != page.OtherUrls || dbPage.ParentId != page.ParentId || dbPage.Url != page.Url)
            //        {
            //            dbPage.IsPage = page.IsPage;
            //            dbPage.Icon = page.Icon;
            //            dbPage.Name = page.Name;
            //            dbPage.OrderNum = page.OrderNum;
            //            dbPage.OtherUrls = page.OtherUrls;
            //            dbPage.ParentId = page.ParentId;
            //            dbPage.Url = page.Url;
            //        }
            //    }
            //    db.SaveChanges(userId);
            //}
        }
        #endregion
    }
}
