using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Web.Utilites
{
    public class DrawSideBar
    {
        public DrawSideBar()
        {
            children = new List<DrawSideBar>();
        }
        public int PageId { get; set; }
        public string Url { get; set; }
        public bool IsPage { get; set; }
        public bool HasAccess { get; set; }
        public bool ShowInSideBar { get; set; } = true;
        public string text { get; set; }
        public string Icon { get; set; }
        public int? ParentId { get; set; }
        public List<DrawSideBar> children { get; set; }
    }
}