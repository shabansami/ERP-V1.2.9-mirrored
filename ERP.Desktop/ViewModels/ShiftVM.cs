using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Desktop.ViewModels
{
    public class ShiftVM
    {
        public Guid ID { get; set; }
        public DateTime Date { get; set; }
        public Guid EmpID { get; set; }
        public string EmpName { get; set; }
        public Guid POSID { get; set; }
        public string POSName { get; set; }
        public string ShiftNumber { get; set; }
    }
}
