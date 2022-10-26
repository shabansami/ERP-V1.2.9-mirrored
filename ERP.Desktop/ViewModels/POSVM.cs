using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Desktop.ViewModels
{
    public class POSVM
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string BranchName { get; set; }
        public Guid? BranchID { get; set; }
        public string StoreName { get; set; }
        public Guid? StoreID { get; set; }
        public string AccountName { get; set; }
        public Guid? AccountID { get; set; }
        public string SafeName { get; set; }
        public Guid? SafeID { get; set; }

    }
}
