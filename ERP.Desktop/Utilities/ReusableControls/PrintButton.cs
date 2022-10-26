using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Desktop.Utilities.ReusableControls
{
    public class PrintButton : FontAwesome.Sharp.IconButton
    {
        public PrintButton()
        {
            Height = 30;
            Width = 90;
            Font = new System.Drawing.Font("tahoma", 10, System.Drawing.FontStyle.Bold);
            FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            ForeColor = Color.White;
            BackColor = Color.Turquoise;
            IconChar = FontAwesome.Sharp.IconChar.Print;
            IconColor = Color.White;
            IconSize = 22;
            Text = "طباعة";
            TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
        }
    }
}
