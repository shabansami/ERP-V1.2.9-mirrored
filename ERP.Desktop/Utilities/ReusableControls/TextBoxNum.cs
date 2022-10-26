using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ERP.Desktop.Utilities.ReusableControls
{
    /*Created by Eng.Youssef*/
    public class TextBoxNum : TextBox
    {
        private double num;
        public bool VisibleDigits { get; set; }
        public int RoundDigits { get; set; }
        public double Num
        {
            get => num;
            set
            {
                num = Math.Round(value, RoundDigits);
                Text = VisibleDigits ? Math.Round(value, RoundDigits).ToString("0.000") : Math.Round(value, RoundDigits) + "";
            }
        }
        public TextBoxNum()
        {
            TextChanged += (s, e) =>
            {
                if (Text == null || Text.Length == 0)
                {
                    num = 0;
                    return;
                }
                if (double.TryParse(Text, out double n) && Math.Round(num, RoundDigits) != n)
                    num = n;
            };
            RoundDigits = 3;
            KeyPress += (s, e) => Validations.txtIsNumberOrDecimal(e, this);
        }
    }
}
