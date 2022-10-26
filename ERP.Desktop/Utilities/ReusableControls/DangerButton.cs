using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Desktop.Utilities.ReusableControls
{
    public class DangerButton : FontAwesome.Sharp.IconButton
    {
        public DangerButton()
        {
            Height = 30;
            Width = 90;
            Font = new System.Drawing.Font("tahoma", 10, System.Drawing.FontStyle.Bold);
            FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            ForeColor = Color.White;
            BackColor = Color.Crimson;
            IconChar = FontAwesome.Sharp.IconChar.Times;
            IconColor = Color.White;
            IconSize = 22;
            TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
        }
    }
}
