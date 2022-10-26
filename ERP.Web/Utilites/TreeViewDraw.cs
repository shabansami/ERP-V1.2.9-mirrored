using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Web.Utilites
{
    public class TreeViewDraw
    {
        public string id { get; set; }
        public double total { get; set; }
        public string text { get; set; }
        public string icon { get; set; }
        public int? ParentId { get; set; }
        public List<TreeViewDraw> children { get; set; }
        public States state { get; set; }

    }

    public class States
    {
        public bool opened { get; set; }
        public bool disabled { get; set; }
        public bool selected { get; set; }
    }
}