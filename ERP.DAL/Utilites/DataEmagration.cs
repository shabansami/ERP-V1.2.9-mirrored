using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.DAL.Utilites
{
    internal class DataEmagration
    {
    }

    public class ReNamed : Attribute
    {
        public string OldName { get; }

        public ReNamed(string oldName)
        {
            OldName = oldName;
        }
    }
}
