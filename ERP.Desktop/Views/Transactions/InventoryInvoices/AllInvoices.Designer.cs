namespace ERP.Desktop.Views.Transactions.InventoryInvoices
{
    partial class AllInvoices
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.dgvInvoices = new System.Windows.Forms.DataGridView();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnAddNew = new ERP.Desktop.Utilities.ReusableControls.DefaultButton();
            this.btnPrint = new ERP.Desktop.Utilities.ReusableControls.PrintButton();
            this.colID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colBranch = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colStore = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDeff = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colGuid = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colShow = new System.Windows.Forms.DataGridViewImageColumn();
            this.colDelete = new System.Windows.Forms.DataGridViewImageColumn();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvInvoices)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.dgvInvoices);
            this.groupBox1.Location = new System.Drawing.Point(7, 59);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(669, 308);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "الفواتير المسجلة";
            // 
            // dgvInvoices
            // 
            this.dgvInvoices.AllowUserToAddRows = false;
            this.dgvInvoices.AllowUserToDeleteRows = false;
            this.dgvInvoices.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvInvoices.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colID,
            this.colDate,
            this.colBranch,
            this.colStore,
            this.colDeff,
            this.colGuid,
            this.colShow,
            this.colDelete});
            this.dgvInvoices.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvInvoices.Location = new System.Drawing.Point(3, 19);
            this.dgvInvoices.Name = "dgvInvoices";
            this.dgvInvoices.ReadOnly = true;
            this.dgvInvoices.Size = new System.Drawing.Size(663, 286);
            this.dgvInvoices.TabIndex = 0;
            this.dgvInvoices.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvInvoices_CellClick);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnAddNew);
            this.groupBox2.Location = new System.Drawing.Point(7, -2);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(669, 55);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            // 
            // btnAddNew
            // 
            this.btnAddNew.BackColor = System.Drawing.Color.DeepSkyBlue;
            this.btnAddNew.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddNew.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.btnAddNew.ForeColor = System.Drawing.Color.White;
            this.btnAddNew.IconChar = FontAwesome.Sharp.IconChar.Plus;
            this.btnAddNew.IconColor = System.Drawing.Color.White;
            this.btnAddNew.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnAddNew.IconSize = 22;
            this.btnAddNew.Location = new System.Drawing.Point(6, 13);
            this.btnAddNew.Name = "btnAddNew";
            this.btnAddNew.Size = new System.Drawing.Size(203, 30);
            this.btnAddNew.TabIndex = 3;
            this.btnAddNew.Text = "اضافة فاتورة جرد جديدة";
            this.btnAddNew.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnAddNew.UseVisualStyleBackColor = false;
            this.btnAddNew.Click += new System.EventHandler(this.btnAddNew_Click);
            // 
            // btnPrint
            // 
            this.btnPrint.BackColor = System.Drawing.Color.Turquoise;
            this.btnPrint.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPrint.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.btnPrint.ForeColor = System.Drawing.Color.White;
            this.btnPrint.IconChar = FontAwesome.Sharp.IconChar.Print;
            this.btnPrint.IconColor = System.Drawing.Color.White;
            this.btnPrint.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnPrint.IconSize = 22;
            this.btnPrint.Location = new System.Drawing.Point(586, 373);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(90, 30);
            this.btnPrint.TabIndex = 2;
            this.btnPrint.Text = "طباعة";
            this.btnPrint.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnPrint.UseVisualStyleBackColor = false;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // colID
            // 
            this.colID.DataPropertyName = "Id";
            this.colID.HeaderText = "ID";
            this.colID.Name = "colID";
            this.colID.ReadOnly = true;
            this.colID.Visible = false;
            // 
            // colDate
            // 
            this.colDate.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colDate.DataPropertyName = "InvoiceDate";
            this.colDate.HeaderText = "تاريخ الفاتورة";
            this.colDate.Name = "colDate";
            this.colDate.ReadOnly = true;
            // 
            // colBranch
            // 
            this.colBranch.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.colBranch.DataPropertyName = "BranchName";
            this.colBranch.HeaderText = "الفرع";
            this.colBranch.Name = "colBranch";
            this.colBranch.ReadOnly = true;
            this.colBranch.Width = 59;
            // 
            // colStore
            // 
            this.colStore.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.colStore.DataPropertyName = "StoreName";
            this.colStore.HeaderText = "المخزن";
            this.colStore.Name = "colStore";
            this.colStore.ReadOnly = true;
            this.colStore.Width = 70;
            // 
            // colDeff
            // 
            this.colDeff.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.colDeff.DataPropertyName = "TotalDifferenceAmount";
            dataGridViewCellStyle1.Format = "N3";
            dataGridViewCellStyle1.NullValue = "0";
            this.colDeff.DefaultCellStyle = dataGridViewCellStyle1;
            this.colDeff.HeaderText = "اجمالى قيمة الفروق";
            this.colDeff.Name = "colDeff";
            this.colDeff.ReadOnly = true;
            this.colDeff.Width = 97;
            // 
            // colGuid
            // 
            this.colGuid.DataPropertyName = "InvoiceGuid";
            this.colGuid.HeaderText = "GUID";
            this.colGuid.Name = "colGuid";
            this.colGuid.ReadOnly = true;
            this.colGuid.Visible = false;
            // 
            // colShow
            // 
            this.colShow.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.colShow.HeaderText = "عرض";
            this.colShow.Image = global::ERP.Desktop.Properties.Resources.eye;
            this.colShow.Name = "colShow";
            this.colShow.ReadOnly = true;
            this.colShow.Width = 41;
            // 
            // colDelete
            // 
            this.colDelete.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.colDelete.HeaderText = "حذف";
            this.colDelete.Image = global::ERP.Desktop.Properties.Resources.trash;
            this.colDelete.Name = "colDelete";
            this.colDelete.ReadOnly = true;
            this.colDelete.Width = 40;
            // 
            // AllInvoices
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(684, 411);
            this.Controls.Add(this.btnPrint);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "AllInvoices";
            this.Text = "فواتير الجرد المسجلة";
            this.Load += new System.EventHandler(this.AllInvoices_Load);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvInvoices)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private Utilities.ReusableControls.PrintButton btnPrint;
        private System.Windows.Forms.DataGridView dgvInvoices;
        private Utilities.ReusableControls.DefaultButton btnAddNew;
        private System.Windows.Forms.DataGridViewTextBoxColumn colID;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn colBranch;
        private System.Windows.Forms.DataGridViewTextBoxColumn colStore;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDeff;
        private System.Windows.Forms.DataGridViewTextBoxColumn colGuid;
        private System.Windows.Forms.DataGridViewImageColumn colShow;
        private System.Windows.Forms.DataGridViewImageColumn colDelete;
    }
}