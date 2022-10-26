using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Web.Utilites
{
    public class DrawTree
    {
        private bool selectedTree = true;

        public Guid id { get; set; }
        public string title { get; set; }
        public Guid? ParentId { get; set; }
        public List<DrawTree> subs { get; set; }
        public bool SelectedTree { get => selectedTree; set 
            { 
                selectedTree = value;
                
            }
        }// عدم امكانية تحديد العناصر الفرعية المدخلة من شاشات خارجية مثل الخزن والعملاء
        public bool ShowAllLevel { get; set; } = false; //اظهار كل الحسابات والمستويات واعطاء خلفيه لاخر عنصر فرعي ف الشجرة
    }
}