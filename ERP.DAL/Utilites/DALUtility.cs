﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.DAL
{
    public class DALUtility
    {
        public static DateTime GetDateTime() => DateTime.UtcNow.AddHours(2);
    }
}
