
namespace ERP.Desktop.Views.Transactions
{
    partial class OrderDetailsQuantity
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.txtNumber = new ERP.Desktop.Utilities.ReusableControls.TextBoxNum();
            this.SuspendLayout();
            // 
            // txtNumber
            // 
            this.txtNumber.Location = new System.Drawing.Point(12, 12);
            this.txtNumber.Name = "txtNumber";
            this.txtNumber.Num = 0D;
            this.txtNumber.Size = new System.Drawing.Size(155, 27);
            this.txtNumber.TabIndex = 0;
            this.txtNumber.Text = "0";
            this.txtNumber.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox1_KeyPress);
            // 
            // OrderDetailsQuantity
            // 
            this.ClientSize = new System.Drawing.Size(179, 57);
            this.Controls.Add(this.txtNumber);
            this.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OrderDetailsQuantity";
            this.Text = "الرجاء ادخال الكمية";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ERP.Desktop.Utilities.ReusableControls.TextBoxNum txtNumber;
    }
}
