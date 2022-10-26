using ERP.Desktop.Utilities;
using ERP.Desktop.Views._Main;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ERP.Desktop.Views.Transactions
{
    public partial class OrderDetailsQuantity : BaseForm
    {
        public int Number { get; set; }
        public bool Selected { get; set; }
        public OrderDetailsQuantity()
        {
            InitializeComponent();
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                if(txtNumber.Num > 0)
                {
                    Selected = true;
                    Number = (int)txtNumber.Num;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("الكمية غير صحيحة!","خطأ",MessageBoxButtons.OK,MessageBoxIcon.Error);
                }
            }
            else if (e.KeyChar == (char)Keys.Escape)
            {
                Selected = false;
                this.Close();
            }
            else
            {
                Validations.txtIsNumberOnly(e);
            }
        }
    }
}
