
namespace ERP.Desktop.Views.Transactions.PurchasesBack
{
    partial class PurchaseBackInvoiceExpenses
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btn_addexpenses = new FontAwesome.Sharp.IconButton();
            this.btn_cancel = new FontAwesome.Sharp.IconButton();
            this.btn_add = new FontAwesome.Sharp.IconButton();
            this.txt_value = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cmb_expenses = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btn_save = new FontAwesome.Sharp.IconButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.dgvInvoiceIncomes = new System.Windows.Forms.DataGridView();
            this.Expense_ID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.exName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TypeID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Amount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.edit = new System.Windows.Forms.DataGridViewImageColumn();
            this.delete = new System.Windows.Forms.DataGridViewImageColumn();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.lbl_allval = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvInvoiceIncomes)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btn_addexpenses);
            this.groupBox1.Controls.Add(this.btn_cancel);
            this.groupBox1.Controls.Add(this.btn_add);
            this.groupBox1.Controls.Add(this.txt_value);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.cmb_expenses);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.btn_save);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox1.Size = new System.Drawing.Size(513, 90);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "الايراد";
            // 
            // btn_addexpenses
            // 
            this.btn_addexpenses.BackColor = System.Drawing.Color.Turquoise;
            this.btn_addexpenses.FlatAppearance.BorderColor = System.Drawing.SystemColors.ActiveBorder;
            this.btn_addexpenses.FlatAppearance.BorderSize = 0;
            this.btn_addexpenses.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_addexpenses.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_addexpenses.ForeColor = System.Drawing.Color.White;
            this.btn_addexpenses.IconChar = FontAwesome.Sharp.IconChar.Plus;
            this.btn_addexpenses.IconColor = System.Drawing.Color.White;
            this.btn_addexpenses.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btn_addexpenses.IconSize = 16;
            this.btn_addexpenses.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btn_addexpenses.Location = new System.Drawing.Point(236, 16);
            this.btn_addexpenses.Margin = new System.Windows.Forms.Padding(0);
            this.btn_addexpenses.Name = "btn_addexpenses";
            this.btn_addexpenses.Padding = new System.Windows.Forms.Padding(0, 5, 2, 0);
            this.btn_addexpenses.Size = new System.Drawing.Size(30, 24);
            this.btn_addexpenses.TabIndex = 79;
            this.btn_addexpenses.UseVisualStyleBackColor = false;
            // 
            // btn_cancel
            // 
            this.btn_cancel.BackColor = System.Drawing.Color.Crimson;
            this.btn_cancel.FlatAppearance.BorderColor = System.Drawing.SystemColors.ActiveBorder;
            this.btn_cancel.FlatAppearance.BorderSize = 0;
            this.btn_cancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_cancel.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_cancel.ForeColor = System.Drawing.Color.White;
            this.btn_cancel.IconChar = FontAwesome.Sharp.IconChar.Ban;
            this.btn_cancel.IconColor = System.Drawing.Color.White;
            this.btn_cancel.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btn_cancel.IconSize = 23;
            this.btn_cancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btn_cancel.Location = new System.Drawing.Point(8, 48);
            this.btn_cancel.Margin = new System.Windows.Forms.Padding(2);
            this.btn_cancel.Name = "btn_cancel";
            this.btn_cancel.Padding = new System.Windows.Forms.Padding(2);
            this.btn_cancel.Size = new System.Drawing.Size(70, 30);
            this.btn_cancel.TabIndex = 57;
            this.btn_cancel.Text = "الغاء";
            this.btn_cancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btn_cancel.UseVisualStyleBackColor = false;
            this.btn_cancel.Visible = false;
            this.btn_cancel.Click += new System.EventHandler(this.btn_cancel_Click);
            // 
            // btn_add
            // 
            this.btn_add.BackColor = System.Drawing.Color.DeepSkyBlue;
            this.btn_add.FlatAppearance.BorderColor = System.Drawing.SystemColors.ActiveBorder;
            this.btn_add.FlatAppearance.BorderSize = 0;
            this.btn_add.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_add.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_add.ForeColor = System.Drawing.Color.White;
            this.btn_add.IconChar = FontAwesome.Sharp.IconChar.Plus;
            this.btn_add.IconColor = System.Drawing.Color.White;
            this.btn_add.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btn_add.IconSize = 23;
            this.btn_add.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btn_add.Location = new System.Drawing.Point(8, 48);
            this.btn_add.Margin = new System.Windows.Forms.Padding(2);
            this.btn_add.Name = "btn_add";
            this.btn_add.Size = new System.Drawing.Size(70, 30);
            this.btn_add.TabIndex = 55;
            this.btn_add.Text = "اضافة";
            this.btn_add.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btn_add.UseVisualStyleBackColor = false;
            this.btn_add.Click += new System.EventHandler(this.btn_add_Click);
            // 
            // txt_value
            // 
            this.txt_value.Location = new System.Drawing.Point(8, 17);
            this.txt_value.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txt_value.Name = "txt_value";
            this.txt_value.Size = new System.Drawing.Size(150, 23);
            this.txt_value.TabIndex = 3;
            this.txt_value.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txt_value_KeyPress);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(164, 20);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(49, 16);
            this.label2.TabIndex = 2;
            this.label2.Text = "المبلغ :";
            // 
            // cmb_expenses
            // 
            this.cmb_expenses.FormattingEnabled = true;
            this.cmb_expenses.Location = new System.Drawing.Point(269, 16);
            this.cmb_expenses.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cmb_expenses.Name = "cmb_expenses";
            this.cmb_expenses.Size = new System.Drawing.Size(150, 24);
            this.cmb_expenses.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(419, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(84, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "نوع المصروف :";
            // 
            // btn_save
            // 
            this.btn_save.BackColor = System.Drawing.Color.Turquoise;
            this.btn_save.FlatAppearance.BorderColor = System.Drawing.SystemColors.ActiveBorder;
            this.btn_save.FlatAppearance.BorderSize = 0;
            this.btn_save.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_save.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_save.ForeColor = System.Drawing.Color.White;
            this.btn_save.IconChar = FontAwesome.Sharp.IconChar.Save;
            this.btn_save.IconColor = System.Drawing.Color.White;
            this.btn_save.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btn_save.IconSize = 23;
            this.btn_save.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btn_save.Location = new System.Drawing.Point(85, 48);
            this.btn_save.Margin = new System.Windows.Forms.Padding(2);
            this.btn_save.Name = "btn_save";
            this.btn_save.Padding = new System.Windows.Forms.Padding(2);
            this.btn_save.Size = new System.Drawing.Size(125, 30);
            this.btn_save.TabIndex = 56;
            this.btn_save.Text = "حفظ التعديلات";
            this.btn_save.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btn_save.UseVisualStyleBackColor = false;
            this.btn_save.Visible = false;
            this.btn_save.Click += new System.EventHandler(this.btn_save_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.dgvInvoiceIncomes);
            this.groupBox2.Controls.Add(this.panel1);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(0, 90);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox2.Size = new System.Drawing.Size(513, 221);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "مصروفات الفاتورة";
            // 
            // dgvInvoiceIncomes
            // 
            this.dgvInvoiceIncomes.AllowUserToAddRows = false;
            this.dgvInvoiceIncomes.AllowUserToDeleteRows = false;
            this.dgvInvoiceIncomes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvInvoiceIncomes.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Expense_ID,
            this.exName,
            this.TypeID,
            this.Amount,
            this.edit,
            this.delete});
            this.dgvInvoiceIncomes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvInvoiceIncomes.Location = new System.Drawing.Point(3, 20);
            this.dgvInvoiceIncomes.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.dgvInvoiceIncomes.Name = "dgvInvoiceIncomes";
            this.dgvInvoiceIncomes.ReadOnly = true;
            this.dgvInvoiceIncomes.RowHeadersWidth = 51;
            this.dgvInvoiceIncomes.Size = new System.Drawing.Size(507, 167);
            this.dgvInvoiceIncomes.TabIndex = 0;
            this.dgvInvoiceIncomes.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgExpensesInvoices_CellContentClick);
            // 
            // Expense_ID
            // 
            this.Expense_ID.DataPropertyName = "ID";
            this.Expense_ID.HeaderText = "Expense_ID";
            this.Expense_ID.MinimumWidth = 6;
            this.Expense_ID.Name = "Expense_ID";
            this.Expense_ID.ReadOnly = true;
            this.Expense_ID.Visible = false;
            this.Expense_ID.Width = 125;
            // 
            // exName
            // 
            this.exName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.exName.DataPropertyName = "TypeName";
            this.exName.HeaderText = "المصروف";
            this.exName.MinimumWidth = 6;
            this.exName.Name = "exName";
            this.exName.ReadOnly = true;
            // 
            // TypeID
            // 
            this.TypeID.DataPropertyName = "TypeID";
            this.TypeID.HeaderText = "Account_ID";
            this.TypeID.MinimumWidth = 6;
            this.TypeID.Name = "TypeID";
            this.TypeID.ReadOnly = true;
            this.TypeID.Visible = false;
            this.TypeID.Width = 125;
            // 
            // Amount
            // 
            this.Amount.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Amount.DataPropertyName = "Amount";
            this.Amount.HeaderText = "المبلغ";
            this.Amount.MinimumWidth = 6;
            this.Amount.Name = "Amount";
            this.Amount.ReadOnly = true;
            this.Amount.Width = 65;
            // 
            // edit
            // 
            this.edit.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.edit.HeaderText = "تعديل";
            this.edit.Image = global::ERP.Desktop.Properties.Resources.edit;
            this.edit.MinimumWidth = 6;
            this.edit.Name = "edit";
            this.edit.ReadOnly = true;
            this.edit.Width = 43;
            // 
            // delete
            // 
            this.delete.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.delete.HeaderText = "حذف";
            this.delete.Image = global::ERP.Desktop.Properties.Resources.delete;
            this.delete.MinimumWidth = 6;
            this.delete.Name = "delete";
            this.delete.ReadOnly = true;
            this.delete.Width = 40;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.lbl_allval);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(3, 187);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(507, 30);
            this.panel1.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(371, 7);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(116, 16);
            this.label3.TabIndex = 6;
            this.label3.Text = "اجمالي المصروفات :";
            // 
            // lbl_allval
            // 
            this.lbl_allval.AutoSize = true;
            this.lbl_allval.Location = new System.Drawing.Point(309, 7);
            this.lbl_allval.Name = "lbl_allval";
            this.lbl_allval.Size = new System.Drawing.Size(14, 16);
            this.lbl_allval.TabIndex = 3;
            this.lbl_allval.Text = "0";
            // 
            // PurchaseBackInvoiceExpenses
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(513, 311);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MinimizeBox = false;
            this.Name = "PurchaseBackInvoiceExpenses";
            this.Text = "مصروفات فاتورة مرتجع توريد";
            this.Load += new System.EventHandler(this.ExpensesInvoices_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvInvoiceIncomes)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txt_value;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmb_expenses;
        private System.Windows.Forms.Label label1;
        private FontAwesome.Sharp.IconButton btn_save;
        private FontAwesome.Sharp.IconButton btn_cancel;
        private FontAwesome.Sharp.IconButton btn_add;
        private System.Windows.Forms.GroupBox groupBox2;
        private FontAwesome.Sharp.IconButton btn_addexpenses;
        private System.Windows.Forms.Label lbl_allval;
        private System.Windows.Forms.DataGridView dgvInvoiceIncomes;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Expense_ID;
        private System.Windows.Forms.DataGridViewTextBoxColumn exName;
        private System.Windows.Forms.DataGridViewTextBoxColumn TypeID;
        private System.Windows.Forms.DataGridViewTextBoxColumn Amount;
        private System.Windows.Forms.DataGridViewImageColumn edit;
        private System.Windows.Forms.DataGridViewImageColumn delete;
    }
}