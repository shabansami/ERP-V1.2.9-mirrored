
namespace ERP.Desktop.Views.Items
{
    partial class ItemsList
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
            this.label1 = new System.Windows.Forms.Label();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.btnPrint = new ERP.Desktop.Utilities.ReusableControls.PrintButton();
            this.btnAdd = new ERP.Desktop.Utilities.ReusableControls.DefaultButton();
            this.btnSearch = new ERP.Desktop.Utilities.ReusableControls.DefaultButton();
            this.dgv_Items = new System.Windows.Forms.DataGridView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.ItemCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ItemTypeName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.GroupBasicName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.GroupSellName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.UnitName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SellPrice = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MinPrice = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MaxPrice = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.GroupBasicId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.GroupSellId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ItemTypeId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.UnitId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColBarCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AvailableToSell = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TechnicalSpecifications = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.edit1 = new System.Windows.Forms.DataGridViewImageColumn();
            this.delete1 = new System.Windows.Forms.DataGridViewImageColumn();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_Items)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 9.75F);
            this.label1.Location = new System.Drawing.Point(882, 25);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "بحث :";
            // 
            // txtSearch
            // 
            this.txtSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSearch.Font = new System.Drawing.Font("Tahoma", 9.75F);
            this.txtSearch.Location = new System.Drawing.Point(568, 22);
            this.txtSearch.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(306, 23);
            this.txtSearch.TabIndex = 0;
            this.txtSearch.TextChanged += new System.EventHandler(this.txt_Name_TextChanged);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.btnPrint);
            this.groupBox4.Controls.Add(this.btnAdd);
            this.groupBox4.Controls.Add(this.btnSearch);
            this.groupBox4.Controls.Add(this.dgv_Items);
            this.groupBox4.Controls.Add(this.label1);
            this.groupBox4.Controls.Add(this.txtSearch);
            this.groupBox4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox4.Location = new System.Drawing.Point(6, 6);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.groupBox4.Size = new System.Drawing.Size(928, 397);
            this.groupBox4.TabIndex = 0;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "قائمة الاصناف";
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
            this.btnPrint.Location = new System.Drawing.Point(6, 364);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(90, 30);
            this.btnPrint.TabIndex = 217;
            this.btnPrint.Text = "طباعة";
            this.btnPrint.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnPrint.UseVisualStyleBackColor = false;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.BackColor = System.Drawing.Color.DeepSkyBlue;
            this.btnAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAdd.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAdd.ForeColor = System.Drawing.Color.White;
            this.btnAdd.IconChar = FontAwesome.Sharp.IconChar.Plus;
            this.btnAdd.IconColor = System.Drawing.Color.White;
            this.btnAdd.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnAdd.IconSize = 22;
            this.btnAdd.Location = new System.Drawing.Point(6, 18);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(171, 30);
            this.btnAdd.TabIndex = 216;
            this.btnAdd.Text = "اضافة صنف جديد";
            this.btnAdd.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnAdd.UseVisualStyleBackColor = false;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnSearch
            // 
            this.btnSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSearch.BackColor = System.Drawing.Color.DarkTurquoise;
            this.btnSearch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSearch.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSearch.ForeColor = System.Drawing.Color.White;
            this.btnSearch.IconChar = FontAwesome.Sharp.IconChar.Search;
            this.btnSearch.IconColor = System.Drawing.Color.White;
            this.btnSearch.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnSearch.IconSize = 22;
            this.btnSearch.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSearch.Location = new System.Drawing.Point(470, 18);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(80, 30);
            this.btnSearch.TabIndex = 215;
            this.btnSearch.Text = "بحث";
            this.btnSearch.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnSearch.UseVisualStyleBackColor = false;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // dgv_Items
            // 
            this.dgv_Items.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.dgv_Items.AllowUserToAddRows = false;
            this.dgv_Items.AllowUserToDeleteRows = false;
            this.dgv_Items.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgv_Items.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgv_Items.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_Items.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ItemCode,
            this.ColName,
            this.ItemTypeName,
            this.GroupBasicName,
            this.GroupSellName,
            this.UnitName,
            this.SellPrice,
            this.MinPrice,
            this.MaxPrice,
            this.ColId,
            this.GroupBasicId,
            this.GroupSellId,
            this.ItemTypeId,
            this.UnitId,
            this.ColBarCode,
            this.AvailableToSell,
            this.TechnicalSpecifications,
            this.edit1,
            this.delete1});
            this.dgv_Items.Location = new System.Drawing.Point(3, 63);
            this.dgv_Items.Margin = new System.Windows.Forms.Padding(3, 22, 3, 22);
            this.dgv_Items.Name = "dgv_Items";
            this.dgv_Items.ReadOnly = true;
            this.dgv_Items.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.dgv_Items.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgv_Items.RowTemplate.Height = 30;
            this.dgv_Items.Size = new System.Drawing.Size(922, 296);
            this.dgv_Items.TabIndex = 214;
            this.dgv_Items.VirtualMode = true;
            this.dgv_Items.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgv_Items_CellClick);
            // 
            // panel1
            // 
            this.panel1.AutoSize = true;
            this.panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel1.Controls.Add(this.groupBox4);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(6);
            this.panel1.Size = new System.Drawing.Size(940, 409);
            this.panel1.TabIndex = 1;
            // 
            // ItemCode
            // 
            this.ItemCode.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.ItemCode.DataPropertyName = "ItemCode";
            this.ItemCode.HeaderText = "كود الصنف";
            this.ItemCode.Name = "ItemCode";
            this.ItemCode.ReadOnly = true;
            this.ItemCode.Width = 88;
            // 
            // ColName
            // 
            this.ColName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.ColName.DataPropertyName = "Name";
            this.ColName.FillWeight = 63.37091F;
            this.ColName.HeaderText = "اسم الصنف";
            this.ColName.MinimumWidth = 100;
            this.ColName.Name = "ColName";
            this.ColName.ReadOnly = true;
            // 
            // ItemTypeName
            // 
            this.ItemTypeName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.ItemTypeName.DataPropertyName = "ItemTypeName";
            this.ItemTypeName.HeaderText = "نوع الصنف";
            this.ItemTypeName.Name = "ItemTypeName";
            this.ItemTypeName.ReadOnly = true;
            this.ItemTypeName.Width = 88;
            // 
            // GroupBasicName
            // 
            this.GroupBasicName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.GroupBasicName.DataPropertyName = "GroupBasicName";
            this.GroupBasicName.FillWeight = 63.37091F;
            this.GroupBasicName.HeaderText = "المجموعة الاساسية";
            this.GroupBasicName.MinimumWidth = 100;
            this.GroupBasicName.Name = "GroupBasicName";
            this.GroupBasicName.ReadOnly = true;
            this.GroupBasicName.Width = 130;
            // 
            // GroupSellName
            // 
            this.GroupSellName.DataPropertyName = "GroupSellName";
            this.GroupSellName.FillWeight = 0.963658F;
            this.GroupSellName.HeaderText = "مجموعة البيع";
            this.GroupSellName.MinimumWidth = 100;
            this.GroupSellName.Name = "GroupSellName";
            this.GroupSellName.ReadOnly = true;
            this.GroupSellName.Visible = false;
            // 
            // UnitName
            // 
            this.UnitName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.UnitName.DataPropertyName = "UnitName";
            this.UnitName.FillWeight = 7.796569F;
            this.UnitName.HeaderText = "الوحدة";
            this.UnitName.MinimumWidth = 50;
            this.UnitName.Name = "UnitName";
            this.UnitName.ReadOnly = true;
            this.UnitName.Width = 67;
            // 
            // SellPrice
            // 
            this.SellPrice.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.SellPrice.DataPropertyName = "SellPrice";
            this.SellPrice.HeaderText = "سعر البيع";
            this.SellPrice.Name = "SellPrice";
            this.SellPrice.ReadOnly = true;
            this.SellPrice.Width = 79;
            // 
            // MinPrice
            // 
            this.MinPrice.DataPropertyName = "MinPrice";
            this.MinPrice.HeaderText = "اقل سعر";
            this.MinPrice.Name = "MinPrice";
            this.MinPrice.ReadOnly = true;
            this.MinPrice.Visible = false;
            // 
            // MaxPrice
            // 
            this.MaxPrice.DataPropertyName = "MaxPrice";
            this.MaxPrice.HeaderText = "اعلي سعر";
            this.MaxPrice.Name = "MaxPrice";
            this.MaxPrice.ReadOnly = true;
            this.MaxPrice.Visible = false;
            // 
            // ColId
            // 
            this.ColId.DataPropertyName = "Id";
            this.ColId.HeaderText = "Id";
            this.ColId.Name = "ColId";
            this.ColId.ReadOnly = true;
            this.ColId.Visible = false;
            // 
            // GroupBasicId
            // 
            this.GroupBasicId.DataPropertyName = "GroupBasicId";
            this.GroupBasicId.HeaderText = "GroupBasicId";
            this.GroupBasicId.Name = "GroupBasicId";
            this.GroupBasicId.ReadOnly = true;
            this.GroupBasicId.Visible = false;
            // 
            // GroupSellId
            // 
            this.GroupSellId.DataPropertyName = "GroupSellId";
            this.GroupSellId.HeaderText = "GroupSellId";
            this.GroupSellId.Name = "GroupSellId";
            this.GroupSellId.ReadOnly = true;
            this.GroupSellId.Visible = false;
            // 
            // ItemTypeId
            // 
            this.ItemTypeId.DataPropertyName = "ItemTypeId";
            this.ItemTypeId.HeaderText = "ItemTypeId";
            this.ItemTypeId.Name = "ItemTypeId";
            this.ItemTypeId.ReadOnly = true;
            this.ItemTypeId.Visible = false;
            // 
            // UnitId
            // 
            this.UnitId.DataPropertyName = "UnitId";
            this.UnitId.HeaderText = "UnitId";
            this.UnitId.Name = "UnitId";
            this.UnitId.ReadOnly = true;
            this.UnitId.Visible = false;
            // 
            // ColBarCode
            // 
            this.ColBarCode.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.ColBarCode.DataPropertyName = "BarCode";
            this.ColBarCode.HeaderText = "الباركود";
            this.ColBarCode.Name = "ColBarCode";
            this.ColBarCode.ReadOnly = true;
            this.ColBarCode.Width = 71;
            // 
            // AvailableToSell
            // 
            this.AvailableToSell.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.AvailableToSell.DataPropertyName = "AvailableToSell";
            this.AvailableToSell.HeaderText = "متوفر للبيع";
            this.AvailableToSell.Name = "AvailableToSell";
            this.AvailableToSell.ReadOnly = true;
            this.AvailableToSell.Width = 60;
            // 
            // TechnicalSpecifications
            // 
            this.TechnicalSpecifications.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.TechnicalSpecifications.DataPropertyName = "TechnicalSpecifications";
            this.TechnicalSpecifications.HeaderText = "الصفات الفنية";
            this.TechnicalSpecifications.Name = "TechnicalSpecifications";
            this.TechnicalSpecifications.ReadOnly = true;
            this.TechnicalSpecifications.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.TechnicalSpecifications.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.TechnicalSpecifications.Visible = false;
            this.TechnicalSpecifications.Width = 78;
            // 
            // edit1
            // 
            this.edit1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.edit1.HeaderText = "تعديل";
            this.edit1.Image = global::ERP.Desktop.Properties.Resources.edit;
            this.edit1.Name = "edit1";
            this.edit1.ReadOnly = true;
            this.edit1.Width = 43;
            // 
            // delete1
            // 
            this.delete1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.delete1.HeaderText = "حذف";
            this.delete1.Image = global::ERP.Desktop.Properties.Resources.trash;
            this.delete1.Name = "delete1";
            this.delete1.ReadOnly = true;
            this.delete1.Width = 40;
            // 
            // ItemsList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(940, 409);
            this.Controls.Add(this.panel1);
            this.MinimizeBox = false;
            this.Name = "ItemsList";
            this.ShowIcon = false;
            this.Text = "قائمة الاصناف";
            this.Load += new System.EventHandler(this.ItemsList_Load);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_Items)).EndInit();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.DataGridView dgv_Items;
        private ERP.Desktop.Utilities.ReusableControls.DefaultButton btnSearch;
        private ERP.Desktop.Utilities.ReusableControls.DefaultButton btnAdd;
        private Utilities.ReusableControls.PrintButton btnPrint;
        private System.Windows.Forms.DataGridViewTextBoxColumn ItemCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColName;
        private System.Windows.Forms.DataGridViewTextBoxColumn ItemTypeName;
        private System.Windows.Forms.DataGridViewTextBoxColumn GroupBasicName;
        private System.Windows.Forms.DataGridViewTextBoxColumn GroupSellName;
        private System.Windows.Forms.DataGridViewTextBoxColumn UnitName;
        private System.Windows.Forms.DataGridViewTextBoxColumn SellPrice;
        private System.Windows.Forms.DataGridViewTextBoxColumn MinPrice;
        private System.Windows.Forms.DataGridViewTextBoxColumn MaxPrice;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColId;
        private System.Windows.Forms.DataGridViewTextBoxColumn GroupBasicId;
        private System.Windows.Forms.DataGridViewTextBoxColumn GroupSellId;
        private System.Windows.Forms.DataGridViewTextBoxColumn ItemTypeId;
        private System.Windows.Forms.DataGridViewTextBoxColumn UnitId;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColBarCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn AvailableToSell;
        private System.Windows.Forms.DataGridViewTextBoxColumn TechnicalSpecifications;
        private System.Windows.Forms.DataGridViewImageColumn edit1;
        private System.Windows.Forms.DataGridViewImageColumn delete1;
    }
}