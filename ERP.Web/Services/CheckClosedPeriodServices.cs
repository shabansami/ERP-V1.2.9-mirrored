using ERP.Web.DataTablesDS;
using ERP.DAL;
using ERP.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using static ERP.Web.Utilites.Lookups;
using ERP.DAL.Dtos;
using System.Web.Mvc;

namespace ERP.Web.Services
{
    public class CheckClosedPeriodServices
    {
        //كل العملاء الساريه تعاقداتهم
        public bool IsINPeriod(string datein)
        {
            DateTime date;
            DateTime.TryParse(datein, out date);

            using (var db = new VTSaleEntities())
            {
                var FutureOpration = db.GeneralSettings.Where(x => x.Id == (int)GeneralSettingCl.FutureOpration).FirstOrDefault().SValue;
                if (FutureOpration == "0")
                {
                    if (date.Date == DateTime.Now.Date)
                    { return true;}
                    else
                    { return false; }
                }
                else
                {
                    var list = db.ClosingPeriods.Where(x => DbFunctions.TruncateTime(x.StartDate) >= date.Date && DbFunctions.TruncateTime(x.EndDate) <= date.Date);
                    list.ToList();
                    if (list.Count()>0)
                    {return true; }
                    else
                    { return false; }
                }

            }
        }


    }
}