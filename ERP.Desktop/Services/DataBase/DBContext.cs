using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ERP.DAL;

namespace ERP.Desktop.Services
{
    internal static class DBContext
    {
        static VTSaleEntities vtsDbContext = new VTSaleEntities();
        public static VTSaleEntities UnitDbContext 
        { 
            get 
            {
                if(vtsDbContext == null || vtsDbContext.IsDisposed)
                    vtsDbContext = new VTSaleEntities();
                return vtsDbContext; 
            } 
        }

        internal static VTSaleEntities CreateDBContext() => new VTSaleEntities();
    }
}
