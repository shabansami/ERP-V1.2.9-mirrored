using System.Collections.Generic;

namespace PrintEngine.HTMLPrint
{
    public class Section
    {
        #region Properties

        public string GroupBy;
        public string TitlePrefix;
        public bool IncludeFooter;
        public Section SubSection;
        public List<(string bindingName, string title)> SectionEnd;
        internal int Level;

        #endregion

        #region Constructors

        public Section()
        {
            SubSection = null;
        }

        public Section(string groupBy, string titlePrefix)
        {
            GroupBy = groupBy;
            TitlePrefix = titlePrefix;
            SubSection = null;
        }
        #endregion
    }
}
