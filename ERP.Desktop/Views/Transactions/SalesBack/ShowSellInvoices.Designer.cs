namespace ERP.Desktop.Views.Transactions.SalesBack
{
    partial class ShowSellInvoices
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
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.dgv_SellInvoice = new System.Windows.Forms.DataGridView();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btn_Update = new ERP.Desktop.Utilities.ReusableControls.SuccessButton();
            this.btnSelectAll = new FontAwesome.Sharp.IconButton();
            this.txt_sellInvoiceId = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.Id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.selectItm = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.ItemName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Quantity = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Price = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Amount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ItemDiscount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SellInvoiceId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ItemId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_SellInvoice)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.Color.WhiteSmoke;
            this.groupBox2.Controls.Add(this.dgv_SellInvoice);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.groupBox2.Location = new System.Drawing.Point(0, 70);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(567, 241);
            this.groupBox2.TabIndex = 182;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "اصناف الفاتورة";
            // 
            // dgv_SellInvoice
            // 
            this.dgv_SellInvoice.AllowUserToAddRows = false;
            this.dgv_SellInvoice.AllowUserToDeleteRows = false;
            this.dgv_SellInvoice.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgv_SellInvoice.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_SellInvoice.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Id,
            this.selectItm,
            this.ItemName,
            this.Quantity,
            this.Price,
            this.Amount,
            this.ItemDiscount,
            this.SellInvoiceId,
            this.ItemId});
            this.dgv_SellInvoice.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgv_SellInvoice.Location = new System.Drawing.Point(3, 19);
            this.dgv_SellInvoice.Margin = new System.Windows.Forms.Padding(3, 22, 3, 22);
            this.dgv_SellInvoice.Name = "dgv_SellInvoice";
            this.dgv_SellInvoice.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.dgv_SellInvoice.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgv_SellInvoice.RowTemplate.Height = 30;
            this.dgv_SellInvoice.ShowEditingIcon = false;
            this.dgv_SellInvoice.Size = new System.Drawing.Size(561, 219);
            this.dgv_SellInvoice.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btn_Update);
            this.groupBox1.Controls.Add(this.btnSelectAll);
            this.groupBox1.Controls.Add(this.txt_sellInvoiceId);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(567, 64);
            this.groupBox1.TabIndex = 203;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "تفاصيل الفاتورة ";
            // 
            // btn_Update
            // 
            this.btn_Update.BackColor = System.Drawing.Color.DarkTurquoise;
            this.btn_Update.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Update.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_Update.ForeColor = System.Drawing.Color.White;
            this.btn_Update.IconChar = FontAwesome.Sharp.IconChar.Save;
            this.btn_Update.IconColor = System.Drawing.Color.White;
            this.btn_Update.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btn_Update.IconSize = 22;
            this.btn_Update.Location = new System.Drawing.Point(12, 24);
            this.btn_Update.Name = "btn_Update";
            this.btn_Update.Size = new System.Drawing.Size(197, 30);
            this.btn_Update.TabIndex = 208;
            this.btn_Update.Text = "اضافة الى فاتورة المرتجع";
            this.btn_Update.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btn_Update.UseVisualStyleBackColor = false;
            this.btn_Update.Click += new System.EventHandler(this.btn_Update_Click_1);
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
            this.btnSelectAll.Location = new System.Drawing.Point(467, 24);
            this.btnSelectAll.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnSelectAll.Name = "btnSelectAll";
            this.btnSelectAll.Size = new System.Drawing.Size(88, 30);
            this.btnSelectAll.TabIndex = 83;
            this.btnSelectAll.Text = "تحديد الكل";
            this.btnSelectAll.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnSelectAll.UseVisualStyleBackColor = false;
            this.btnSelectAll.Click += new System.EventHandler(this.btnSelectAll_Click);
            // 
            // txt_sellInvoiceId
            // 
            this.txt_sellInvoiceId.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_sellInvoiceId.Location = new System.Drawing.Point(215, 28);
            this.txt_sellInvoiceId.Margin = new System.Windows.Forms.Padding(3, 22, 3, 22);
            this.txt_sellInvoiceId.Name = "txt_sellInvoiceId";
            this.txt_sellInvoiceId.Size = new System.Drawing.Size(232, 23);
            this.txt_sellInvoiceId.TabIndex = 203;
            this.txt_sellInvoiceId.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txt_sellInvoiceId_KeyDown);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(264, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(138, 16);
            this.label3.TabIndex = 204;
            this.label3.Text = "رقم/باركود فاتورة البيع : ";
            // 
            // Id
            // 
            this.Id.DataPropertyName = "Id";
            this.Id.HeaderText = "Id";
            this.Id.Name = "Id";
            this.Id.Visible = false;
            // 
            // selectItm
            // 
            this.selectItm.DataPropertyName = "selectItm";
            this.selectItm.HeaderText = "تحديد";
            this.selectItm.Name = "selectItm";
            // 
            // ItemName
            // 
            this.ItemName.DataPropertyName = "ItemName";
            this.ItemName.HeaderText = "اسم الصنف";
            this.ItemName.Name = "ItemName";
            // 
            // Quantity
            // 
            this.Quantity.DataPropertyName = "Quantity";
            this.Quantity.HeaderText = "الكمية";
            this.Quantity.Name = "Quantity";
            // 
            // Price
            // 
            this.Price.DataPropertyName = "Price";
            this.Price.HeaderText = "السعر";
            this.Price.Name = "Price";
            // 
            // Amount
            // 
            this.Amount.DataPropertyName = "Amount";
            this.Amount.HeaderText = "القيمة";
            this.Amount.Name = "Amount";
            // 
            // ItemDiscount
            // 
            this.ItemDiscount.DataPropertyName = "ItemDiscount";
            this.ItemDiscount.HeaderText = "الخصم";
            this.ItemDiscount.Name = "ItemDiscount";
            // 
            // SellInvoiceId
            // 
            this.SellInvoiceId.DataPropertyName = "SellInvoiceId";
            this.SellInvoiceId.HeaderText = "SellInvoiceId";
            this.SellInvoiceId.Name = "SellInvoiceId";
            this.SellInvoiceId.Visible = false;
            // 
            // ItemId
            // 
            this.ItemId.DataPropertyName = "ItemId";
            this.ItemId.HeaderText = "ItemId";
            this.ItemId.Name = "ItemId";
            this.ItemId.Visible = false;
            // 
            // ShowSellInvoices
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(567, 311);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "ShowSellInvoices";
            this.Text = "تفاصيل فاتورة بيع ";
            this.Load += new System.EventHandler(this.ShowSellInvoices_Load);
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgv_SellInvoice)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.DataGridView dgv_SellInvoice;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txt_sellInvoiceId;
        private System.Windows.Forms.Label label3;
        private FontAwesome.Sharp.IconButton btnSelectAll;
        private Utilities.ReusableControls.SuccessButton btn_Update;
        private System.Windows.Forms.DataGridViewTextBoxColumn Id;
        private System.Windows.Forms.DataGridViewCheckBoxColumn selectItm;
        private System.Windows.Forms.DataGridViewTextBoxColumn ItemName;
        private System.Windows.Forms.DataGridViewTextBoxColumn Quantity;
        private System.Windows.Forms.DataGridViewTextBoxColumn Price;
        private System.Windows.Forms.DataGridViewTextBoxColumn Amount;
        private System.Windows.Forms.DataGridViewTextBoxColumn ItemDiscount;
        private System.Windows.Forms.DataGridViewTextBoxColumn SellInvoiceId;
        private System.Windows.Forms.DataGridViewTextBoxColumn ItemId;
    }
}