namespace ERP.Desktop.Views.Reports.ProductReports
{
    partial class ItemBalances
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
            this.btnSearch = new ERP.Desktop.Utilities.ReusableControls.SuccessButton();
            this.cmbItemTypes = new System.Windows.Forms.ComboBox();
            this.cmbStores = new System.Windows.Forms.ComboBox();
            this.cmbBranchs = new System.Windows.Forms.ComboBox();
            this.cmbGroups = new System.Windows.Forms.ComboBox();
            this.txtBarCode = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtItemCode = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.dgvItemBalance = new System.Windows.Forms.DataGridView();
            this.ID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ItemName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Group = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ItemType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Store = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Balance = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ItemCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BarCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.printButton1 = new ERP.Desktop.Utilities.ReusableControls.PrintButton();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvItemBalance)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnSearch);
            this.groupBox1.Controls.Add(this.cmbItemTypes);
            this.groupBox1.Controls.Add(this.cmbStores);
            this.groupBox1.Controls.Add(this.cmbBranchs);
            this.groupBox1.Controls.Add(this.cmbGroups);
            this.groupBox1.Controls.Add(this.txtBarCode);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.txtItemCode);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.txtSearch);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(4, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(794, 117);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "معايير البحث";
            // 
            // btnSearch
            // 
            this.btnSearch.BackColor = System.Drawing.Color.Turquoise;
            this.btnSearch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSearch.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.btnSearch.ForeColor = System.Drawing.Color.White;
            this.btnSearch.IconChar = FontAwesome.Sharp.IconChar.Search;
            this.btnSearch.IconColor = System.Drawing.Color.White;
            this.btnSearch.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnSearch.IconSize = 22;
            this.btnSearch.Location = new System.Drawing.Point(56, 64);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(90, 30);
            this.btnSearch.TabIndex = 3;
            this.btnSearch.Text = "بحث";
            this.btnSearch.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnSearch.UseVisualStyleBackColor = false;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // cmbItemTypes
            // 
            this.cmbItemTypes.FormattingEnabled = true;
            this.cmbItemTypes.Location = new System.Drawing.Point(6, 22);
            this.cmbItemTypes.Name = "cmbItemTypes";
            this.cmbItemTypes.Size = new System.Drawing.Size(150, 24);
            this.cmbItemTypes.TabIndex = 2;
            // 
            // cmbStores
            // 
            this.cmbStores.FormattingEnabled = true;
            this.cmbStores.Location = new System.Drawing.Point(246, 78);
            this.cmbStores.Name = "cmbStores";
            this.cmbStores.Size = new System.Drawing.Size(150, 24);
            this.cmbStores.TabIndex = 2;
            // 
            // cmbBranchs
            // 
            this.cmbBranchs.FormattingEnabled = true;
            this.cmbBranchs.Location = new System.Drawing.Point(512, 78);
            this.cmbBranchs.Name = "cmbBranchs";
            this.cmbBranchs.Size = new System.Drawing.Size(150, 24);
            this.cmbBranchs.TabIndex = 2;
            this.cmbBranchs.SelectedIndexChanged += new System.EventHandler(this.cmbBranchs_SelectedIndexChanged);
            // 
            // cmbGroups
            // 
            this.cmbGroups.FormattingEnabled = true;
            this.cmbGroups.Location = new System.Drawing.Point(512, 48);
            this.cmbGroups.Name = "cmbGroups";
            this.cmbGroups.Size = new System.Drawing.Size(150, 24);
            this.cmbGroups.TabIndex = 2;
            // 
            // txtBarCode
            // 
            this.txtBarCode.Location = new System.Drawing.Point(246, 48);
            this.txtBarCode.Name = "txtBarCode";
            this.txtBarCode.Size = new System.Drawing.Size(150, 23);
            this.txtBarCode.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(398, 51);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(47, 16);
            this.label3.TabIndex = 0;
            this.label3.Text = "باركود :";
            // 
            // txtItemCode
            // 
            this.txtItemCode.Location = new System.Drawing.Point(246, 18);
            this.txtItemCode.Name = "txtItemCode";
            this.txtItemCode.Size = new System.Drawing.Size(150, 23);
            this.txtItemCode.TabIndex = 1;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(158, 27);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(72, 16);
            this.label6.TabIndex = 0;
            this.label6.Text = "نوع الصنف :";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(398, 83);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(54, 16);
            this.label8.TabIndex = 0;
            this.label8.Text = "المخزن :";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(398, 21);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(72, 16);
            this.label2.TabIndex = 0;
            this.label2.Text = "كود الصنف :";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(664, 83);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(43, 16);
            this.label7.TabIndex = 0;
            this.label7.Text = "الفرع :";
            // 
            // txtSearch
            // 
            this.txtSearch.Location = new System.Drawing.Point(512, 18);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(150, 23);
            this.txtSearch.TabIndex = 1;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(664, 53);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(126, 16);
            this.label4.TabIndex = 0;
            this.label4.Text = "المجموعة الاساسية :";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(664, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "بحث :";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.dgvItemBalance);
            this.groupBox2.Location = new System.Drawing.Point(4, 126);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(794, 300);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "ارصدة الاصناف";
            // 
            // dgvItemBalance
            // 
            this.dgvItemBalance.AllowUserToAddRows = false;
            this.dgvItemBalance.AllowUserToDeleteRows = false;
            this.dgvItemBalance.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvItemBalance.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ID,
            this.ItemName,
            this.Group,
            this.ItemType,
            this.Store,
            this.Balance,
            this.ItemCode,
            this.BarCode});
            this.dgvItemBalance.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvItemBalance.Location = new System.Drawing.Point(3, 19);
            this.dgvItemBalance.Name = "dgvItemBalance";
            this.dgvItemBalance.ReadOnly = true;
            this.dgvItemBalance.Size = new System.Drawing.Size(788, 278);
            this.dgvItemBalance.TabIndex = 0;
            // 
            // ID
            // 
            this.ID.DataPropertyName = "Id";
            this.ID.HeaderText = "ID";
            this.ID.Name = "ID";
            this.ID.ReadOnly = true;
            this.ID.Visible = false;
            // 
            // ItemName
            // 
            this.ItemName.DataPropertyName = "ItemName";
            this.ItemName.HeaderText = "الصنف";
            this.ItemName.Name = "ItemName";
            this.ItemName.ReadOnly = true;
            // 
            // Group
            // 
            this.Group.DataPropertyName = "GroupName";
            this.Group.HeaderText = "المجموعة";
            this.Group.Name = "Group";
            this.Group.ReadOnly = true;
            // 
            // ItemType
            // 
            this.ItemType.DataPropertyName = "ItemTypeName";
            this.ItemType.HeaderText = "نوع الصنف";
            this.ItemType.Name = "ItemType";
            this.ItemType.ReadOnly = true;
            // 
            // Store
            // 
            this.Store.DataPropertyName = "StoreName";
            this.Store.HeaderText = "المخزن";
            this.Store.Name = "Store";
            this.Store.ReadOnly = true;
            // 
            // Balance
            // 
            this.Balance.DataPropertyName = "Balance";
            this.Balance.HeaderText = "الرصيد";
            this.Balance.Name = "Balance";
            this.Balance.ReadOnly = true;
            // 
            // ItemCode
            // 
            this.ItemCode.DataPropertyName = "ItemCode";
            this.ItemCode.HeaderText = "كود الصنف";
            this.ItemCode.Name = "ItemCode";
            this.ItemCode.ReadOnly = true;
            // 
            // BarCode
            // 
            this.BarCode.DataPropertyName = "BarCode";
            this.BarCode.HeaderText = "الباركود";
            this.BarCode.Name = "BarCode";
            this.BarCode.ReadOnly = true;
            // 
            // printButton1
            // 
            this.printButton1.BackColor = System.Drawing.Color.Turquoise;
            this.printButton1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.printButton1.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.printButton1.ForeColor = System.Drawing.Color.White;
            this.printButton1.IconChar = FontAwesome.Sharp.IconChar.Print;
            this.printButton1.IconColor = System.Drawing.Color.White;
            this.printButton1.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.printButton1.IconSize = 22;
            this.printButton1.Location = new System.Drawing.Point(708, 432);
            this.printButton1.Name = "printButton1";
            this.printButton1.Size = new System.Drawing.Size(90, 30);
            this.printButton1.TabIndex = 2;
            this.printButton1.Text = "طباعة";
            this.printButton1.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.printButton1.UseVisualStyleBackColor = false;
            this.printButton1.Click += new System.EventHandler(this.printButton1_Click);
            // 
            // ItemBalances
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(804, 467);
            this.Controls.Add(this.printButton1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "ItemBalances";
            this.Text = "ارصدة الاصناف";
            this.Load += new System.EventHandler(this.ItemBalances_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvItemBalance)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtBarCode;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtItemCode;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cmbGroups;
        private Utilities.ReusableControls.SuccessButton btnSearch;
        private System.Windows.Forms.ComboBox cmbItemTypes;
        private System.Windows.Forms.ComboBox cmbStores;
        private System.Windows.Forms.ComboBox cmbBranchs;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.GroupBox groupBox2;
        private Utilities.ReusableControls.PrintButton printButton1;
        private System.Windows.Forms.DataGridView dgvItemBalance;
        private System.Windows.Forms.DataGridViewTextBoxColumn ID;
        private System.Windows.Forms.DataGridViewTextBoxColumn ItemName;
        private System.Windows.Forms.DataGridViewTextBoxColumn Group;
        private System.Windows.Forms.DataGridViewTextBoxColumn ItemType;
        private System.Windows.Forms.DataGridViewTextBoxColumn Store;
        private System.Windows.Forms.DataGridViewTextBoxColumn Balance;
        private System.Windows.Forms.DataGridViewTextBoxColumn ItemCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn BarCode;
    }
}