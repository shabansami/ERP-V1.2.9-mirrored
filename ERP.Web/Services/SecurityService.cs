using ERP.DAL;
using ERP.DAL.Utilites;
using ERP.Web.Utilites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Web;
using static ERP.Web.Utilites.Lookups;

namespace ERP.Web.Services
{
    public static class SecurityService
    {
        //التأكد من صلاحية رقم الحماية او لا 
        public static bool isSecured()
        {
            string pcSerial = getSerialID();
            string serialEncrypted = VTSAuth.Encrypt(pcSerial);
            using (var db=new VTSaleEntities())
            {
                var securityKey= db.GeneralSettings.Where(x => !x.IsDeleted && x.Id == (int)GeneralSettingCl.EntityDataSecurity).FirstOrDefault().SValue;
                if (securityKey == serialEncrypted)
                    return true;
                else
                    return false;
            }
        }
        public static string getSerialID()
        {
            string fullCode = string.Empty;
            fullCode += getProcessorID() + "n" + getMotherBoardID() + "n";

            return fullCode;
        }

        #region الحصول على سيريال البروسيسور والهارد والمازربورد

        static string getProcessorID()
        {
            string code = "";
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(
                           "select * from Win32_Processor");
            foreach (ManagementObject share in searcher.Get())
            {
                code = share["processorId"].ToString();
            }
            return code;
        }
        static string getMotherBoardID()
        {
            string code = "";
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(
                          "select * from Win32_BaseBoard");
            foreach (ManagementObject share in searcher.Get())
            {
                code = share["SerialNumber"].ToString();
            }
            return code;
        }

        static string getHardDiskID()
        {
            string code = "";
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(
                          "select * from Win32_DiskDrive");
            foreach (ManagementObject share in searcher.Get())
            {
                code = share["Signature"].ToString();
            }
            return code;
        }
        #endregion

    }
}