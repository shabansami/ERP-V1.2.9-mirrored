using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml.Serialization;
using VTSLicense.Core;
using VTSLicense.Core.Enums;

namespace VTSLicense.LicenseChecker.Logic
{
    public class LicenseFeatures : LicenseEntity
    {
        [DisplayName("Enable Feature 01")]
        [Category("License Options")]
        [XmlElement("EnableFeature01")]
        [ShowInLicenseInfo(true, "Enable Feature 01", ShowInLicenseInfoAttribute.FormatType.String)]
        public bool EnableFeature01 { get; set; }

        [DisplayName("Enable Feature 02")]
        [Category("License Options")]
        [XmlElement("EnableFeature02")]
        [ShowInLicenseInfo(true, "Enable Feature 02", ShowInLicenseInfoAttribute.FormatType.String)]
        public bool EnableFeature02 { get; set; }

        [DisplayName("Enable Feature 03")]
        [Category("License Options")]
        [XmlElement("EnableFeature03")]
        [ShowInLicenseInfo(true, "Enable Feature 03", ShowInLicenseInfoAttribute.FormatType.String)]
        public bool EnableFeature03 { get; set; }

        public LicenseFeatures()
        {
            //Initialize app name for the license
            //TODO:Change app name
            AppName = "VTS";
        }

        public override LicenseStatus ValidateAppName(out string validationMsg)
        {
            var licStatus = LicenseStatus.UNDEFINED;
            validationMsg = string.Empty;

            switch (Type)
            {
                case LicenseTypes.Single:
                    //For Single License, check whether UID is matched
                    if (UID == LicenseHandler.GenerateUID(AppName))
                    {
                        licStatus = LicenseStatus.VALID;
                    }
                    else
                    {
                        validationMsg = "The license is NOT for this copy!";
                        licStatus = LicenseStatus.INVALID;
                    }

                    break;
                case LicenseTypes.Volume:
                    //No UID checking for Volume License
                    licStatus = LicenseStatus.VALID;
                    break;

                case LicenseTypes.ExpirationDate:
                    if (UID == LicenseHandler.GenerateUID(AppName))
                    {
                        if (ExpireDateTime > DateTime.Now)
                            licStatus = LicenseStatus.VALID;
                        else
                        {
                            validationMsg = "This license expired!";
                            licStatus = LicenseStatus.INVALID;
                        }
                    }
                    else
                    {
                        validationMsg = "The license is NOT for this copy!";
                        licStatus = LicenseStatus.INVALID;
                    }
                    break;
                default:
                    validationMsg = "Invalid license";
                    licStatus = LicenseStatus.INVALID;
                    break;
            }

            return licStatus;
        }
    }
}