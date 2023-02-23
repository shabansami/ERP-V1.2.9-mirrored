


namespace ERP.DAL
{
    using System;
    using System.Collections.Generic;
    using ERP.DAL.Models;

    public partial class OrientationTypes : BaseModelInt
    {
        public OrientationTypes()
        {
            this.AccountsTrees = new HashSet<AccountsTree>();
        }

        public string Name { get; set; }

        public virtual ICollection<AccountsTree> AccountsTrees { get; set; }
    }
}

