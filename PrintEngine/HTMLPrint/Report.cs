using PrintEngine.HTMLPrint.Printing;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using static PrintEngine.HTMLPrint.ReportClasses.EntityInformation;

namespace PrintEngine.HTMLPrint
{
    public class Report
    {
        #region Public Properties

        //Report Properties
        public DataTable ReportSource { get; set; }
        public List<Section> Sections { get; set; }
        public string ReportTitle { get; set; }
        public bool IncludeEntityInformation { get; set; } = true;
        public bool IncludeVtsFooter { get; set; } = true;
        public bool IncludeVtsHeaders { get; set; } = true;
        public bool HasPrintingNotes { get; set; }
        public List<Field> ReportFields { get; set; }
        public string ReportFont { get; set; }
        public bool IncludeBarcodeFont { get; set; } = false;
        public string BarcodeFontPath { get; set; }
        public int FontSize { get; set; }

        public string DocumentGenerated { get; set; }

        #endregion

        #region Private Properties

        private const string Newline = "\n";
        private int _iLevel;
        private readonly StringBuilder htmlContent;
        #endregion

        #region Constructor

        public Report()
        {
            htmlContent = new StringBuilder();
            Sections = new List<Section>();
            ReportFields = new List<Field>();
            ReportFont = "Arial";
        }

        #endregion

        #region Report Generation Methods

        /// <summary>
        /// Generates the table the given ReportSource.
        /// </summary>
        /// <returns>HTML String</returns>
        public void GenerateReport()
        {
            WriteStyle();
            WriteSections();
            DocumentGenerated = htmlContent.ToString();
            WriteHeadersAndFooters();
        }

        /// <summary>
        /// Writes CSS for the document and HTML title.
        /// </summary>
        private void WriteStyle()
        {

            //Set dir=rtl for RightToLeft Layout.
            // Set http-equiv content = 'IE=edge' to make the webcontrol use the Latest installed internet explorer (or edge) rendering engine
            // chrome=1 is to use the chrome rendering if instaled
            // This aims to support new html/css features.
            htmlContent.Append("<HTML dir='rtl'><HEAD> <meta http-equiv='X-UA-Compatible' content = 'IE=11' /><meta content='text/html'; charset='utf-8' http-equiv=Content-Type>" + Newline);

            //set Report title
            htmlContent.Append("<TITLE>" + ReportTitle + "</TITLE>" + Newline);

            //Set Report Font
            htmlContent.Append("<STYLE>" + (IncludeBarcodeFont ? @"@font-face { font-family: 'barcodefont'; src:local('3 of 9 Barcode'), url(" + BarcodeFontPath + ") format('truetype');}" : "") + "</STYLE>");

            //Set the css styling of the table
            htmlContent.Append("<STYLE>" + GetStyleFile() + "</STYLE>" + (FontSize > 0 ? "<style> .TableStyle td{font-size:" + FontSize + "px;}</style>" : "") + "</HEAD><BODY style='font-family:\"DIN Next LT Arabic\"'>" + Newline); //style='border:black; border-width:3px;border-style:solid;' 
            //add water mark
            //if (!string.IsNullOrEmpty(EntityLogoName))
            //    htmlContent.Append($"<img src='data:image/jpeg;base64,{EntityLogoName}' style='position:absolute;top:15%; right: 7%;opacity:0.2;' />");
           
            if (IncludeVtsHeaders)
                //initialize header
                htmlContent.Append($"<div class='row'><div><!--headerright--></div><div style='text-align: left;'><!--headerleft--></div></div>" + Newline);

            htmlContent.Append("<!--beforetitle-->");
            htmlContent.Append("<h1>" + ReportTitle + "</h1>");
            htmlContent.Append("<!--aftertitle-->");
        }

        /// <summary>
        /// Generates all section contents
        /// </summary>
        private void WriteSections()
        {
            if (Sections.Count == 0)
            {
                Section dummySection = new Section();
                dummySection.Level = 5;

                htmlContent.Append("<TABLE class='TableStyle'>" + Newline);

                WriteSectionDetail(null, "");

            }
            foreach (Section section in Sections)
            {
                _iLevel = 0;
                RecurseSections(section, "");
            }

            htmlContent.Append("<!--atlastrow-->" + Newline);
            htmlContent.Append("</TABLE>" + Newline);
            htmlContent.Append("<!--atfooter-->" + Newline);
            if (IncludeVtsFooter)
                htmlContent.Append($"<p style='margin-top : 0; padding-top:0; border-bottom-style:none;border-left-style:none;border-right-style:none;border-top-style:solid; border-width:2px;font-size:16px;text-align: center;'>تصميم وبرمجة VTS ت : 01065556289</p>" + Newline);
            htmlContent.Append("</BODY></HTML>");
        }

        /// <summary>
        /// Method to write Section data information
        /// </summary>
        /// <param name="section">the section details</param>
        /// <param name="criteria">the section selection criteria</param>
        private void WriteSectionDetail(Section section, string criteria)
        {
            if (section == null)
            {
                section = new Section();
            }

            if (ReportSource == null)
                return;

            //Draw DetailHeader
            htmlContent.Append("<thead><TR>" + Newline);
            foreach (Field field in ReportFields)
            {
                htmlContent.Append("<TH>" + field.HeaderName + "</TH>" + Newline);
            }

            htmlContent.Append("</TR></thead>" + Newline);

            //Draw Data
            if (criteria == null || criteria.Trim() == "")
                criteria = "";
            else
                criteria = criteria.Substring(3);


            foreach (DataRow dr in ReportSource.Select(criteria))
            {
                htmlContent.Append("<TR>" + Newline);
                foreach (Field field in ReportFields)
                {
                    htmlContent.Append(" <TD>" + dr[field.FieldName] + "</TD>" + Newline);
                }

                htmlContent.Append("</TR>" + Newline);

                if (HasPrintingNotes && dr["Notes"] != null && !string.IsNullOrEmpty(dr["Notes"].ToString()))
                {
                    htmlContent.Append("<TR><TD colspan='" + this.ReportFields.Count + $"'><b>ملاحظة </b>: {dr["Notes"]}</TD></TR>");
                }
            }

            //render ending of section
            if (section.SectionEnd != null && section.SectionEnd.Count > 0)
            {
                htmlContent.Append("</TABLE> <TABLE class = 'TableStyle tb'><tr>");

                foreach (var item in section.SectionEnd)
                {
                    htmlContent.Append($"<td><b>{item.title}</b></td>");
                    htmlContent.Append($"<td>{ReportSource.Select(criteria)[0][item.bindingName]}</td>");
                }

                htmlContent.Append("</tr>" + Newline);
            }


            htmlContent.Append("</Table>" + Newline);
        }


        /// <summary>
        /// A recursive function to write all the section headers, details and footer content.
        /// </summary>
        /// <param name="section">the section details</param>
        /// <param name="criteria">section data selection criteria</param>
        private void RecurseSections(Section section, string criteria)
        {
            _iLevel++;
            section.Level = _iLevel;
            ArrayList result = GetDistinctValues(ReportSource, section.GroupBy, criteria);

            foreach (object obj in result)
            {
                //Construct critiera string to select data for the current section
                string tcriteria = criteria + "and " + section.GroupBy + "='" + obj.ToString().Replace("'", "''") + "' ";
                WriteSectionHeader(section, obj.ToString());

                if (section.SubSection != null)
                {
                    RecurseSections(section.SubSection, tcriteria);
                    _iLevel--;
                }
                else
                {
                    WriteSectionDetail(section, tcriteria);
                }
            }
            if (section.Level < 2)
                htmlContent.Append("<TR><TD colspan='" + ReportFields.Count + "'>&nbsp;</TD></TR>");
        }

        /// <summary>
        /// Writes the section header information.
        /// </summary>
        /// <param name="section">The section details as Section object</param>
        /// <param name="sectionValue">section group field data</param>
        private void WriteSectionHeader(Section section, string sectionValue)
        {
            htmlContent.Append("<TABLE class='TableStyle'>" + Newline);

            htmlContent.Append("<thead>" + Newline);
            htmlContent.Append("<TR id='GroupingColumn'><TD colspan='" + ReportFields.Count + "'>");
            htmlContent.Append(section.TitlePrefix + sectionValue);
            htmlContent.Append("</TD></TR>" + Newline);
            htmlContent.Append("</thead>" + Newline);
        }

        private void WriteHeadersAndFooters()
        {
            //First check if the user wants to show the entity info
            if (!IncludeEntityInformation)
                return;

            //Check each string in EntityInfo class: headerLine1, headerLine2, footerLine1, footerLine2
            if (!string.IsNullOrEmpty(HeaderLine2))
            {
                AddHtmlLine($"<p>{HeaderLine2}</p>", StringLocation.AtHeader);
            }

            if (!string.IsNullOrEmpty(HeaderLine1))
            {
                AddHtmlLine($"<p>{HeaderLine1}</p>", StringLocation.AtHeader);
            }

            if (!string.IsNullOrEmpty(EntityLogoName))
            {
                AddHtmlLine($"<img src='data:image/jpeg;base64,{EntityLogoName}' alt='Logo' style='width:100%;'/>", StringLocation.AtHeader);
            }
            else
            {
                AddHtmlLine($"<h2>{EntityName}</h2>", StringLocation.AtHeader);
            }


            //two lines footer
            AddHtmlLine("<div class='row'><div>", StringLocation.AtFooter);


            if (!string.IsNullOrEmpty(FooterLine1))
            {
                AddHorizontalLine(StringLocation.AtFooter);

                AddHtmlLine($"<p style='margin-bottom : 0; padding-top:0;'>{FooterLine1}</p>", StringLocation.AtFooter);
            }

            if (!string.IsNullOrEmpty(FooterLine2))
            {
                AddHtmlLine($"<p style='margin-top : 0; padding-top:0;'>{FooterLine2}</p>", StringLocation.AtFooter);
            }

            AddHtmlLine("</div></div>", StringLocation.AtFooter);
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Reads the css file from the project embedded refrences and add it to the html string
        /// </summary>
        private string GetStyleFile()
        {
            var assembly = Assembly.GetExecutingAssembly();
            string resourceName = "PrintEngine.Resources.style.css";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        /// <summary>
        /// Method to get distinct values for the column in the report source dataset
        /// </summary>
        /// <param name="dataSet">report source dataset</param>
        /// <param name="columnName">Column name</param>
        /// <param name="criteria">Data selection criteria</param>
        /// <returns>List of distinct values</returns>
        private ArrayList GetDistinctValues(DataTable dataTable, string columnName, string criteria)
        {
            ArrayList distinctValues = new ArrayList();
            if (criteria == null || criteria.Trim() == "")
            {
                criteria = "";
            }
            else
            {
                criteria = criteria.Substring(3);
            }
            foreach (DataRow dr in dataTable.Select(criteria))
            {
                if (!distinctValues.Contains(dr[columnName].ToString()))
                {
                    distinctValues.Add(dr[columnName].ToString());
                }
            }
            return distinctValues;
        }

        #endregion

        #region Add Controls

        // Add an HTML line in any location in the report generated
        public void AddHtmlLine(string lineToAdd, StringLocation loc = StringLocation.AfterTitle)
        {
            switch (loc)
            {
                case StringLocation.BeforeTitle:
                    DocumentGenerated = DocumentGenerated.Replace("<!--beforetitle-->", lineToAdd + "<!--beforetitle-->");
                    break;
                case StringLocation.AfterTitle:
                    DocumentGenerated = DocumentGenerated.Replace("<!--aftertitle-->", lineToAdd + "<!--aftertitle-->");
                    break;
                case StringLocation.AtLastRow:
                    DocumentGenerated = DocumentGenerated.Replace("<!--atlastrow-->", lineToAdd + "<!--atlastrow-->");
                    break;
                case StringLocation.AtFooter:
                    DocumentGenerated = DocumentGenerated.Replace("<!--atfooter-->", lineToAdd + "<!--atfooter-->");
                    break;
                case StringLocation.AfterFooter:
                    DocumentGenerated = DocumentGenerated.Replace("<!--atfooter-->", "<!--atfooter-->" + lineToAdd);
                    break;
                case StringLocation.AtHeader:
                    DocumentGenerated = DocumentGenerated.Replace("<!--headerright-->", "<!--headerright-->" + lineToAdd);
                    break;
                case StringLocation.AtHeaderLeft:
                    DocumentGenerated = DocumentGenerated.Replace("<!--headerleft-->", lineToAdd + "<!--headerleft-->");
                    break;
                case StringLocation.AtHeaderRight:
                    DocumentGenerated = DocumentGenerated.Replace("<!--headerright-->", lineToAdd + "<!--headerright-->");
                    break;
            }
        }


        /// <summary>
        /// Add mutliple string fields in a div section with a border
        /// One field with be bold and the other one will be regular
        /// </summary>
        /// <param name="values">The values to enter</param>
        /// <param name="columns">number of columns to add</param>
        /// <param name="stringLocation">The location to add the strings</param>
        /// <param name="bordered">if bordered or not</param>
        public void AddStrings(IEnumerable<(string, string)> values, int columns = 3, StringLocation stringLocation = StringLocation.AtFooter, bool bordered = true, int fontsize = 8)
        {
            if (!values.Any())
                return;
            int i = columns;
            var html = $"<table style='{(bordered ? "border:2px solid black;" : "")} width:100%;text-align:right; table-layout:fixed;margin: 2px 0px !important;font-size:{fontsize}pt'>";
            foreach (var value in values)
            {
                if (i == columns)
                    html += "<tr>";

                html += $"<td><b>{value.Item1}{(string.IsNullOrEmpty(value.Item1) ? "" : ":")}</b></td><td>{value.Item2}</td>";

                for (int n = values.Count(); n < columns; n++)
                {
                    html += "<td></td><td></td>";
                }

                if (i == 1)
                {
                    html += "</tr>";
                    i = columns;
                    continue;
                }
                i--;
            }
            html += "</table>";
            AddHtmlLine(html, stringLocation);
        }

        public void AddLineBreak(StringLocation loc = StringLocation.AfterTitle) => AddHtmlLine("<br>", loc);

        public void AddHorizontalLine(StringLocation loc = StringLocation.AfterTitle) => AddHtmlLine("<hr>", loc);

        #endregion

        #region Printing Choices

        public void ShowPrintPreviewDialog()
        {
            PrintManagement.ShowChosenPrintDialog(PrintOperation.Preview, DocumentGenerated);
        }

        public void ShowPageSetupDialog()
        {
            PrintManagement.ShowChosenPrintDialog(PrintOperation.Setup, DocumentGenerated);
        }

        public void ShowPrintDialog()
        {
            PrintManagement.ShowChosenPrintDialog(PrintOperation.ShowPrint, DocumentGenerated);
        }

        public void ShowSaveAsDialog()
        {
            PrintManagement.ShowChosenPrintDialog(PrintOperation.Save, DocumentGenerated);
        }

        public void Print(string printerName, int copies = 1, bool isOpenSetting = false)
        {
            PrintManagement.Print(printerName, DocumentGenerated,  copies, isOpenSetting);
        }
        #endregion

    }
}