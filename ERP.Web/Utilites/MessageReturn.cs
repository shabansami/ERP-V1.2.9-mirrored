using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Web.Utilites
{
    public class MessageReturn
    {
        public string Message { get; set; }
        public bool IsValid { get; set; }
        public Object Object { get; set; }
    }
}