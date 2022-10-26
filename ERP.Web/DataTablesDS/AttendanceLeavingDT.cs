using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Web.DataTablesDS
{
    public class AttendanceLeavingDT
    {
        public int DayDone { get; set; }//عدد الايام الصحيحة
        public int DelayDayNumber { get; set; } //عدد ايام التأخير
        public double DelayHourPenalty { get; set; }//عدد ساعات التاخير المخصومة
        public double DelayAmount { get; set; }//قيمة خصم التأخير 
        public int AbsenenceDayPenalty { get; set; }//عدد ايام الغياب 
        public Double AbsenceAmount { get; set; }//قيمة الغياب المخصومة
        public double OverTimeHour { get; set; }//عدد ساعات الافرتايم
        public double OverTimeAmount { get; set; }//قيمة الافرتايم
        public int VacationDays { get; set; } //ايام الاجازات الرسمية
        public bool Isvalid { get; set; }
    }
}