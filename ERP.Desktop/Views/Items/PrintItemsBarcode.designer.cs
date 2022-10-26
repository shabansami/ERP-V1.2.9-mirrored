
namespace ERP.Desktop.Views.Items
{
    partial class PrintItemsBarcode
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cmbInvoices = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbSuppliers = new System.Windows.Forms.ComboBox();
            this.btnSearchTab1 = new FontAwesome.Sharp.IconButton();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.rdo_itemDetails = new System.Windows.Forms.RadioButton();
            this.rdo_itemOnly = new System.Windows.Forms.RadioButton();
            this.label4 = new System.Windows.Forms.Label();
            this.cmb_Groups = new System.Windows.Forms.ComboBox();
            this.btnSearchTab2 = new FontAwesome.Sharp.IconButton();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.dgvItems = new System.Windows.Forms.DataGridView();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.txt_printCount = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.ckDoubled = new System.Windows.Forms.CheckBox();
            this.cmbPrinters = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnPrint = new FontAwesome.Sharp.IconButton();
            this.btnSelectAll = new FontAwesome.Sharp.IconButton();
            this.Id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Choose = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Item_AName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SalePrice = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PrintCount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Barcode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ItemCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvItems)).BeginInit();
            this.groupBox6.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.RightToLeftLayout = true;
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(684, 89);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Location = new System.Drawing.Point(4, 25);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(676, 60);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "طباعة اصناف الفاتورة";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cmbInvoices);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.cmbSuppliers);
            this.groupBox1.Controls.Add(this.btnSearchTab1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(670, 54);
            this.groupBox1.TabIndex = 80;
            this.groupBox1.TabStop = false;
            // 
            // cmbInvoices
            // 
            this.cmbInvoices.FormattingEnabled = true;
            this.cmbInvoices.Location = new System.Drawing.Point(107, 18);
            this.cmbInvoices.Name = "cmbInvoices";
            this.cmbInvoices.Size = new System.Drawing.Size(200, 24);
            this.cmbInvoices.TabIndex = 83;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(313, 21);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(51, 16);
            this.label2.TabIndex = 82;
            this.label2.Text = "الفاتورة:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(587, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 16);
            this.label1.TabIndex = 81;
            this.label1.Text = "اسم المورد :";
            // 
            // cmbSuppliers
            // 
            this.cmbSuppliers.FormattingEnabled = true;
            this.cmbSuppliers.Location = new System.Drawing.Point(384, 18);
            this.cmbSuppliers.Name = "cmbSuppliers";
            this.cmbSuppliers.Size = new System.Drawing.Size(200, 24);
            this.cmbSuppliers.TabIndex = 80;
            this.cmbSuppliers.SelectedIndexChanged += new System.EventHandler(this.cmb_Supplier_SelectedIndexChanged);
            // 
            // btnSearchTab1
            // 
            this.btnSearchTab1.IconChar = FontAwesome.Sharp.IconChar.Search;
            this.btnSearchTab1.IconColor = System.Drawing.Color.Black;
            this.btnSearchTab1.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnSearchTab1.IconSize = 23;
            this.btnSearchTab1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSearchTab1.Location = new System.Drawing.Point(6, 14);
            this.btnSearchTab1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnSearchTab1.Name = "btnSearchTab1";
            this.btnSearchTab1.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnSearchTab1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btnSearchTab1.Size = new System.Drawing.Size(70, 30);
            this.btnSearchTab1.TabIndex = 79;
            this.btnSearchTab1.Text = "بحث";
            this.btnSearchTab1.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnSearchTab1.UseVisualStyleBackColor = true;
            this.btnSearchTab1.Click += new System.EventHandler(this.btnSearchTab1_Click);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.groupBox4);
            this.tabPage2.Location = new System.Drawing.Point(4, 25);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(676, 60);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "طباعة بالتصنيف";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.rdo_itemDetails);
            this.groupBox4.Controls.Add(this.rdo_itemOnly);
            this.groupBox4.Controls.Add(this.label4);
            this.groupBox4.Controls.Add(this.cmb_Groups);
            this.groupBox4.Controls.Add(this.btnSearchTab2);
            this.groupBox4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox4.Location = new System.Drawing.Point(3, 3);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(670, 54);
            this.groupBox4.TabIndex = 86;
            this.groupBox4.TabStop = false;
            // 
            // rdo_itemDetails
            // 
            this.rdo_itemDetails.AutoSize = true;
            this.rdo_itemDetails.Location = new System.Drawing.Point(111, 19);
            this.rdo_itemDetails.Name = "rdo_itemDetails";
            this.rdo_itemDetails.Size = new System.Drawing.Size(110, 20);
            this.rdo_itemDetails.TabIndex = 97;
            this.rdo_itemDetails.Text = "الصنف بتفاصيله";
            this.rdo_itemDetails.UseVisualStyleBackColor = true;
            // 
            // rdo_itemOnly
            // 
            this.rdo_itemOnly.AutoSize = true;
            this.rdo_itemOnly.Checked = true;
            this.rdo_itemOnly.Location = new System.Drawing.Point(227, 19);
            this.rdo_itemOnly.Name = "rdo_itemOnly";
            this.rdo_itemOnly.Size = new System.Drawing.Size(113, 20);
            this.rdo_itemOnly.TabIndex = 96;
            this.rdo_itemOnly.TabStop = true;
            this.rdo_itemOnly.Text = "اسم الصنف فقط";
            this.rdo_itemOnly.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(564, 21);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(99, 16);
            this.label4.TabIndex = 87;
            this.label4.Text = "مجموعة الصنف :";
            // 
            // cmb_Groups
            // 
            this.cmb_Groups.FormattingEnabled = true;
            this.cmb_Groups.Location = new System.Drawing.Point(358, 18);
            this.cmb_Groups.Name = "cmb_Groups";
            this.cmb_Groups.Size = new System.Drawing.Size(200, 24);
            this.cmb_Groups.TabIndex = 93;
            // 
            // btnSearchTab2
            // 
            this.btnSearchTab2.IconChar = FontAwesome.Sharp.IconChar.Search;
            this.btnSearchTab2.IconColor = System.Drawing.Color.Black;
            this.btnSearchTab2.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnSearchTab2.IconSize = 23;
            this.btnSearchTab2.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSearchTab2.Location = new System.Drawing.Point(6, 14);
            this.btnSearchTab2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnSearchTab2.Name = "btnSearchTab2";
            this.btnSearchTab2.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnSearchTab2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btnSearchTab2.Size = new System.Drawing.Size(70, 30);
            this.btnSearchTab2.TabIndex = 91;
            this.btnSearchTab2.Text = "بحث";
            this.btnSearchTab2.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnSearchTab2.UseVisualStyleBackColor = true;
            this.btnSearchTab2.Click += new System.EventHandler(this.btnFillDgvByGroup_Click);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.dgvItems);
            this.groupBox5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox5.Location = new System.Drawing.Point(0, 89);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(684, 332);
            this.groupBox5.TabIndex = 87;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "قائمة الأصناف";
            // 
            // dgvItems
            // 
            this.dgvItems.AllowUserToAddRows = false;
            this.dgvItems.AllowUserToDeleteRows = false;
            this.dgvItems.AllowUserToResizeColumns = false;
            this.dgvItems.AllowUserToResizeRows = false;
            this.dgvItems.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvItems.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Id,
            this.Choose,
            this.Item_AName,
            this.SalePrice,
            this.PrintCount,
            this.Barcode,
            this.ItemCode});
            this.dgvItems.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvItems.Location = new System.Drawing.Point(3, 19);
            this.dgvItems.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.dgvItems.Name = "dgvItems";
            this.dgvItems.Size = new System.Drawing.Size(678, 310);
            this.dgvItems.TabIndex = 86;
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.txt_printCount);
            this.groupBox6.Controls.Add(this.label5);
            this.groupBox6.Controls.Add(this.ckDoubled);
            this.groupBox6.Controls.Add(this.cmbPrinters);
            this.groupBox6.Controls.Add(this.label3);
            this.groupBox6.Controls.Add(this.btnPrint);
            this.groupBox6.Controls.Add(this.btnSelectAll);
            this.groupBox6.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.groupBox6.Location = new System.Drawing.Point(0, 421);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(684, 47);
            this.groupBox6.TabIndex = 88;
            this.groupBox6.TabStop = false;
            // 
            // txt_printCount
            // 
            this.txt_printCount.Location = new System.Drawing.Point(173, 15);
            this.txt_printCount.Name = "txt_printCount";
            this.txt_printCount.Size = new System.Drawing.Size(83, 23);
            this.txt_printCount.TabIndex = 86;
            this.txt_printCount.Text = "0";
            this.txt_printCount.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txt_printCount_KeyPress);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(254, 17);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(72, 16);
            this.label5.TabIndex = 85;
            this.label5.Text = "عدد النسخ:";
            // 
            // ckDoubled
            // 
            this.ckDoubled.AutoSize = true;
            this.ckDoubled.Location = new System.Drawing.Point(344, 13);
            this.ckDoubled.Name = "ckDoubled";
            this.ckDoubled.Size = new System.Drawing.Size(109, 20);
            this.ckDoubled.TabIndex = 84;
            this.ckDoubled.Text = "الطباعة مزدوجة";
            this.ckDoubled.UseVisualStyleBackColor = true;
            this.ckDoubled.Visible = false;
            // 
            // cmbPrinters
            // 
            this.cmbPrinters.FormattingEnabled = true;
            this.cmbPrinters.Location = new System.Drawing.Point(459, 14);
            this.cmbPrinters.Name = "cmbPrinters";
            this.cmbPrinters.Size = new System.Drawing.Size(167, 24);
            this.cmbPrinters.TabIndex = 83;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(623, 17);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(57, 16);
            this.label3.TabIndex = 82;
            this.label3.Text = "الطابعة : ";
            // 
            // btnPrint
            // 
            this.btnPrint.BackColor = System.Drawing.Color.WhiteSmoke;
            this.btnPrint.FlatAppearance.BorderColor = System.Drawing.SystemColors.ActiveBorder;
            this.btnPrint.FlatAppearance.BorderSize = 0;
            this.btnPrint.ForeColor = System.Drawing.Color.Black;
            this.btnPrint.IconChar = FontAwesome.Sharp.IconChar.Save;
            this.btnPrint.IconColor = System.Drawing.Color.Black;
            this.btnPrint.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnPrint.IconSize = 15;
            this.btnPrint.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPrint.Location = new System.Drawing.Point(3, 10);
            this.btnPrint.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(70, 30);
            this.btnPrint.TabIndex = 83;
            this.btnPrint.Text = "طباعة";
            this.btnPrint.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnPrint.UseVisualStyleBackColor = false;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // btnSelectAll
            // 
            this.btnSelectAll.BackColor = System.Drawing.Color.WhiteSmoke;
            this.btnSelectAll.FlatAppearance.BorderColor = System.Drawing.SystemColors.ActiveBorder;
            this.btnSelectAll.FlatAppearance.BorderSize = 0;
            this.btnSelectAll.ForeColor = System.Drawing.Color.Black;
            this.btnSelectAll.IconChar = FontAwesome.Sharp.IconChar.CheckSquare;
            this.btnSelectAll.IconColor = System.Drawing.Color.Black;
            this.btnSelectAll.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnSelectAll.IconSize = 15;
            this.btnSelectAll.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSelectAll.Location = new System.Drawing.Point(79, 10);
            this.btnSelectAll.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnSelectAll.Name = "btnSelectAll";
            this.btnSelectAll.Size = new System.Drawing.Size(88, 30);
            this.btnSelectAll.TabIndex = 82;
            this.btnSelectAll.Text = "تحديد الكل";
            this.btnSelectAll.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnSelectAll.UseVisualStyleBackColor = false;
            this.btnSelectAll.Click += new System.EventHandler(this.BtnCheckAllItems_Click);
            // 
            // Id
            // 
            this.Id.DataPropertyName = "ID";
            this.Id.HeaderText = "ID";
            this.Id.Name = "Id";
            this.Id.ReadOnly = true;
            this.Id.Visible = false;
            // 
            // Choose
            // 
            this.Choose.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.Choose.DataPropertyName = "Choose";
            this.Choose.HeaderText = "اختيار";
            this.Choose.Name = "Choose";
            this.Choose.Width = 44;
            // 
            // Item_AName
            // 
            this.Item_AName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Item_AName.DataPropertyName = "Name";
            this.Item_AName.HeaderText = "اسم الصنف";
            this.Item_AName.Name = "Item_AName";
            this.Item_AName.ReadOnly = true;
            // 
            // SalePrice
            // 
            this.SalePrice.DataPropertyName = "Price";
            this.SalePrice.HeaderText = "السعر";
            this.SalePrice.Name = "SalePrice";
            // 
            // PrintCount
            // 
            this.PrintCount.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.PrintCount.DataPropertyName = "Quantity";
            this.PrintCount.HeaderText = "عدد النسخ";
            this.PrintCount.Name = "PrintCount";
            this.PrintCount.Width = 92;
            // 
            // Barcode
            // 
            this.Barcode.DataPropertyName = "Barcode";
            this.Barcode.HeaderText = "Barcode";
            this.Barcode.Name = "Barcode";
            this.Barcode.Visible = false;
            // 
            // ItemCode
            // 
            this.ItemCode.DataPropertyName = "ItemCode";
            this.ItemCode.HeaderText = "ItemCode";
            this.ItemCode.Name = "ItemCode";
            this.ItemCode.Visible = false;
            // 
            // PrintItemsBarcode
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(684, 468);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox6);
            this.Controls.Add(this.tabControl1);
            this.Name = "PrintItemsBarcode";
            this.Tag = "124";
            this.Text = "طباعة الباركود";
            this.Load += new System.EventHandler(this.PrintItemsBarcode_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvItems)).EndInit();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private FontAwesome.Sharp.IconButton btnSearchTab1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox cmbInvoices;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbSuppliers;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.ComboBox cmb_Groups;
        private FontAwesome.Sharp.IconButton btnSearchTab2;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.DataGridView dgvItems;
        private System.Windows.Forms.GroupBox groupBox6;
        private FontAwesome.Sharp.IconButton btnPrint;
        private FontAwesome.Sharp.IconButton btnSelectAll;
        private System.Windows.Forms.ComboBox cmbPrinters;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox ckDoubled;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txt_printCount;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.RadioButton rdo_itemDetails;
        private System.Windows.Forms.RadioButton rdo_itemOnly;
        private System.Windows.Forms.DataGridViewTextBoxColumn Id;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Choose;
        private System.Windows.Forms.DataGridViewTextBoxColumn Item_AName;
        private System.Windows.Forms.DataGridViewTextBoxColumn SalePrice;
        private System.Windows.Forms.DataGridViewTextBoxColumn PrintCount;
        private System.Windows.Forms.DataGridViewTextBoxColumn Barcode;
        private System.Windows.Forms.DataGridViewTextBoxColumn ItemCode;
    }
}