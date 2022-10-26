
namespace ERP.Desktop.Views.Items
{
    partial class ItemSearch
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.DGItems = new System.Windows.Forms.DataGridView();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btn_search = new ERP.Desktop.Utilities.ReusableControls.SuccessButton();
            this.lblNotes = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.Item_ID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FastCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Item_AName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SalesPrice = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Group_Name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.group = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Item_Barcode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DGItems)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.groupBox2);
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(800, 450);
            this.panel1.TabIndex = 1;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.DGItems);
            this.groupBox2.Location = new System.Drawing.Point(12, 67);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(776, 373);
            this.groupBox2.TabIndex = 54;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "نتائج البحث";
            // 
            // DGItems
            // 
            this.DGItems.AllowUserToAddRows = false;
            this.DGItems.AllowUserToDeleteRows = false;
            this.DGItems.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.DGItems.BackgroundColor = System.Drawing.SystemColors.InactiveBorder;
            this.DGItems.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DGItems.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Item_ID,
            this.FastCode,
            this.Item_AName,
            this.SalesPrice,
            this.Group_Name,
            this.group,
            this.Item_Barcode});
            this.DGItems.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DGItems.Location = new System.Drawing.Point(3, 19);
            this.DGItems.Name = "DGItems";
            this.DGItems.ReadOnly = true;
            this.DGItems.Size = new System.Drawing.Size(770, 351);
            this.DGItems.TabIndex = 25;
            this.DGItems.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DGItems_CellDoubleClick);
            this.DGItems.KeyDown += new System.Windows.Forms.KeyEventHandler(this.DGItems_KeyDown);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btn_search);
            this.groupBox1.Controls.Add(this.lblNotes);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txtSearch);
            this.groupBox1.Location = new System.Drawing.Point(12, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(776, 58);
            this.groupBox1.TabIndex = 53;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "البحث ب";
            // 
            // btn_search
            // 
            this.btn_search.BackColor = System.Drawing.Color.Turquoise;
            this.btn_search.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_search.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.btn_search.ForeColor = System.Drawing.Color.White;
            this.btn_search.IconChar = FontAwesome.Sharp.IconChar.Search;
            this.btn_search.IconColor = System.Drawing.Color.White;
            this.btn_search.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btn_search.IconSize = 22;
            this.btn_search.Location = new System.Drawing.Point(323, 18);
            this.btn_search.Name = "btn_search";
            this.btn_search.Size = new System.Drawing.Size(90, 30);
            this.btn_search.TabIndex = 30;
            this.btn_search.Text = "بحث";
            this.btn_search.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btn_search.UseVisualStyleBackColor = false;
            this.btn_search.Click += new System.EventHandler(this.btn_search_Click);
            // 
            // lblNotes
            // 
            this.lblNotes.AutoSize = true;
            this.lblNotes.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNotes.Location = new System.Drawing.Point(6, 25);
            this.lblNotes.Name = "lblNotes";
            this.lblNotes.Size = new System.Drawing.Size(156, 16);
            this.lblNotes.TabIndex = 28;
            this.lblNotes.Text = "اضغط مرتين لأختيار الصنف";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(717, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 17);
            this.label2.TabIndex = 14;
            this.label2.Text = "الصنف :";
            // 
            // txtSearch
            // 
            this.txtSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSearch.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSearch.Location = new System.Drawing.Point(419, 19);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(296, 27);
            this.txtSearch.TabIndex = 16;
            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
            this.txtSearch.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSearch_KeyDown);
            // 
            // Item_ID
            // 
            this.Item_ID.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Item_ID.DataPropertyName = "ID";
            this.Item_ID.HeaderText = "ID";
            this.Item_ID.Name = "Item_ID";
            this.Item_ID.ReadOnly = true;
            this.Item_ID.Visible = false;
            this.Item_ID.Width = 44;
            // 
            // FastCode
            // 
            this.FastCode.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.FastCode.DataPropertyName = "ItemCode";
            this.FastCode.HeaderText = "كود الصنف";
            this.FastCode.Name = "FastCode";
            this.FastCode.ReadOnly = true;
            this.FastCode.Width = 88;
            // 
            // Item_AName
            // 
            this.Item_AName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Item_AName.DataPropertyName = "Name";
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.Item_AName.DefaultCellStyle = dataGridViewCellStyle1;
            this.Item_AName.HeaderText = "الصنف";
            this.Item_AName.Name = "Item_AName";
            this.Item_AName.ReadOnly = true;
            // 
            // SalesPrice
            // 
            this.SalesPrice.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.SalesPrice.DataPropertyName = "Price";
            this.SalesPrice.FillWeight = 60F;
            this.SalesPrice.HeaderText = "السعر";
            this.SalesPrice.Name = "SalesPrice";
            this.SalesPrice.ReadOnly = true;
            this.SalesPrice.Width = 64;
            // 
            // Group_Name
            // 
            this.Group_Name.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.Group_Name.DataPropertyName = "GroupBasicName";
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.Group_Name.DefaultCellStyle = dataGridViewCellStyle2;
            this.Group_Name.HeaderText = "المجموعة الاساسية";
            this.Group_Name.Name = "Group_Name";
            this.Group_Name.ReadOnly = true;
            this.Group_Name.Width = 130;
            // 
            // group
            // 
            this.group.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.group.DataPropertyName = "GroupSellName";
            this.group.HeaderText = "مجموعة البيع";
            this.group.Name = "group";
            this.group.ReadOnly = true;
            this.group.Visible = false;
            this.group.Width = 97;
            // 
            // Item_Barcode
            // 
            this.Item_Barcode.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Item_Barcode.DataPropertyName = "BarCode";
            this.Item_Barcode.HeaderText = "الباركود";
            this.Item_Barcode.Name = "Item_Barcode";
            this.Item_Barcode.ReadOnly = true;
            this.Item_Barcode.Width = 71;
            // 
            // ItemSearch
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MinimizeBox = false;
            this.Name = "ItemSearch";
            this.Tag = "20";
            this.Text = "بحث عن الأصناف";
            this.Load += new System.EventHandler(this.ItemsSearch_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ItemsSearch_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.ItemsSearch_KeyUp);
            this.panel1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.DGItems)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblNotes;
        private System.Windows.Forms.DataGridView DGItems;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private Utilities.ReusableControls.SuccessButton btn_search;
        private System.Windows.Forms.DataGridViewTextBoxColumn Item_ID;
        private System.Windows.Forms.DataGridViewTextBoxColumn FastCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn Item_AName;
        private System.Windows.Forms.DataGridViewTextBoxColumn SalesPrice;
        private System.Windows.Forms.DataGridViewTextBoxColumn Group_Name;
        private System.Windows.Forms.DataGridViewTextBoxColumn group;
        private System.Windows.Forms.DataGridViewTextBoxColumn Item_Barcode;
    }
}