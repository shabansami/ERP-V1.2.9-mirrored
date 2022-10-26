using System.ComponentModel;

namespace VTSLicense.Core.Enums
{
    public enum LicenseTypes
    {
        [Description("Unknown")] Unknown = 0,
        [Description("Single")] Single = 1,
        [Description("Volume")] Volume = 2,
        [Description("ExpirationDate")] ExpirationDate = 3
    }
}