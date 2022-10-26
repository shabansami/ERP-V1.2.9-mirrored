using ERP.Desktop.Utilities.ReusableControls;
using System;
using System.Globalization;
using System.Windows.Forms;

namespace ERP.Desktop.Utilities
{
    //Created By eng:Shaban Sami
    public class Validations
    {
        /// <summary>
        /// Input number or decimal
        /// اجبار المستخدم على ادخال ارقام فقط صحيحة او عشرية
        /// </summary>
        /// <param name="e">Key Press Event Args</param>
        /// <param name="control">the Control</param>
        public static void txtIsNumberOrDecimal(KeyPressEventArgs e, Control control)
        {
            char decimalSepartor = Convert.ToChar(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != 8 && e.KeyChar != decimalSepartor)
                e.Handled = true;
            if (e.KeyChar == decimalSepartor && control.Text.Contains(decimalSepartor + ""))
                e.Handled = true;
        }

        // اجبار المستخدم على ادخال ارقام فقط صحيحة
        public static void txtIsNumberOnly(KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != 8)
                e.Handled = true;
        }
        // دالة لمعرفة هل المدخل رقم ام نص 
        public static bool isNumeric(string txt)
        {
            double num;
            return double.TryParse(txt, out num);
        }
        // اجبار المستخدم على عدم ادخال نصوص فارغة

        public static void txtstartnospace(object sender, KeyPressEventArgs e)
        {
            if ((sender as TextBox).SelectionStart == 0)
                e.Handled = (e.KeyChar == (char)Keys.Space);
            else
                e.Handled = false;
        }

        /// <summary>
        /// Allows letters, backspace and space in a textbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void AcceptAlphabeticOnly(object sender, KeyPressEventArgs e)
        {
            e.Handled = e.KeyChar != (char)Keys.Back && !char.IsSeparator(e.KeyChar) && !char.IsLetter(e.KeyChar);
        }

        /// <summary>
        /// Allows numbers, letters, backspace and space in a textbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void AcceptAlphaNumericOnly(object sender, KeyPressEventArgs e)
        {
            e.Handled = e.KeyChar != (char)Keys.Back && !char.IsSeparator(e.KeyChar) && !char.IsLetter(e.KeyChar) && !char.IsDigit(e.KeyChar);
        }
        public static bool ValidiateControls(params Control[] controls)
        {
            foreach (Control control in controls)
            {
                if(control is TextBoxNum)
                {
                    if(((TextBoxNum)control).Num < 0)
                    {
                        AlrtMsgs.EnterVaildData();
                        control.Focus();
                        return false;
                    }
                }
                else if (control is TextBox)
                {
                    if (string.IsNullOrWhiteSpace(control.Text))
                    {
                        AlrtMsgs.EnterVaildData();
                        control.Focus();
                        return false;
                    }
                }
                else if (control is ComboBox)
                {
                    var combo = (ComboBox)control;
                    if (combo.SelectedIndex < 1)
                    {
                        AlrtMsgs.ChooseVaildData();
                        combo.Focus();
                        return false;
                    }
                }
                else
                {
                    throw new Exception("The validiated control is not type of TextBox of ComboBox.\nControl Type : " + control.GetType().Name);
                }
            }
            return true;
        }
    }
}
