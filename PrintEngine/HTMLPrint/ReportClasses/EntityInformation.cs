namespace PrintEngine.HTMLPrint.ReportClasses
{
    /// <summary>
    /// A class that holds the information of the company (name , logo , headers to be inserted, ..)
    /// </summary>
    public class EntityInformation
    {
        public static string EntityName { get; set; }
        public static string HeaderLine1 { get; set; }
        public static string HeaderLine2 { get; set; }
        public static string FooterLine1 { get; set; }
        public static string FooterLine2 { get; set; }
        public static string EntityLogoName { get; set; }
    }
}
