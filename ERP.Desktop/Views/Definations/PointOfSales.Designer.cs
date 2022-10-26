namespace ERP.Desktop.Views.Definations
{
    partial class PointOfSales
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
            this.dgv_pos = new System.Windows.Forms.DataGridView();
            this.PointId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BranchName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SafeName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AccountName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BranchID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SafeID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AccountID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btn_edit = new System.Windows.Forms.DataGridViewImageColumn();
            this.btn_delete = new System.Windows.Forms.DataGridViewImageColumn();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.ckCanChangeItemPrice = new System.Windows.Forms.CheckBox();
            this.ckCanDiscount = new System.Windows.Forms.CheckBox();
            this.btnSaveChanges = new ERP.Desktop.Utilities.ReusableControls.SuccessButton();
            this.btnAdd = new ERP.Desktop.Utilities.ReusableControls.DefaultButton();
            this.btnCancelChanges = new ERP.Desktop.Utilities.ReusableControls.DangerButton();
            this.cmbo_BankCard = new System.Windows.Forms.ComboBox();
            this.cmbPricePolicies = new System.Windows.Forms.ComboBox();
            this.cmbSuppliers = new System.Windows.Forms.ComboBox();
            this.cmbo_BankWallet = new System.Windows.Forms.ComboBox();
            this.cmbPaymentTypes = new System.Windows.Forms.ComboBox();
            this.cmbCustomers = new System.Windows.Forms.ComboBox();
            this.cmb_safe = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.cmb_bankAccount = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.cmb_branchs = new System.Windows.Forms.ComboBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.txt_Point = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblarea = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.cmb_store = new System.Windows.Forms.ComboBox();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_pos)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.Color.WhiteSmoke;
            this.groupBox2.Controls.Add(this.dgv_pos);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.groupBox2.Location = new System.Drawing.Point(0, 228);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(689, 276);
            this.groupBox2.TabIndex = 182;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "نقاط البيع";
            // 
            // dgv_pos
            // 
            this.dgv_pos.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.dgv_pos.AllowUserToAddRows = false;
            this.dgv_pos.AllowUserToDeleteRows = false;
            this.dgv_pos.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgv_pos.CausesValidation = false;
            this.dgv_pos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_pos.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.PointId,
            this.colName,
            this.BranchName,
            this.SafeName,
            this.AccountName,
            this.BranchID,
            this.SafeID,
            this.AccountID,
            this.btn_edit,
            this.btn_delete});
            this.dgv_pos.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgv_pos.Location = new System.Drawing.Point(3, 19);
            this.dgv_pos.Margin = new System.Windows.Forms.Padding(3, 22, 3, 22);
            this.dgv_pos.Name = "dgv_pos";
            this.dgv_pos.ReadOnly = true;
            this.dgv_pos.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.dgv_pos.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgv_pos.RowTemplate.Height = 30;
            this.dgv_pos.ShowEditingIcon = false;
            this.dgv_pos.Size = new System.Drawing.Size(683, 254);
            this.dgv_pos.TabIndex = 0;
            this.dgv_pos.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgv_pos_CellClick);
            // 
            // PointId
            // 
            this.PointId.DataPropertyName = "Id";
            this.PointId.HeaderText = "Id";
            this.PointId.Name = "PointId";
            this.PointId.ReadOnly = true;
            this.PointId.Visible = false;
            // 
            // colName
            // 
            this.colName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colName.DataPropertyName = "Name";
            this.colName.FillWeight = 63.37091F;
            this.colName.HeaderText = " نقطة البيع";
            this.colName.MinimumWidth = 100;
            this.colName.Name = "colName";
            this.colName.ReadOnly = true;
            // 
            // BranchName
            // 
            this.BranchName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.BranchName.DataPropertyName = "BranchName";
            this.BranchName.FillWeight = 63.37091F;
            this.BranchName.HeaderText = "الفرع";
            this.BranchName.MinimumWidth = 100;
            this.BranchName.Name = "BranchName";
            this.BranchName.ReadOnly = true;
            // 
            // SafeName
            // 
            this.SafeName.DataPropertyName = "SafeName";
            this.SafeName.FillWeight = 0.963658F;
            this.SafeName.HeaderText = "الخزينة";
            this.SafeName.MinimumWidth = 100;
            this.SafeName.Name = "SafeName";
            this.SafeName.ReadOnly = true;
            // 
            // AccountName
            // 
            this.AccountName.DataPropertyName = "AccountName";
            this.AccountName.FillWeight = 7.796569F;
            this.AccountName.HeaderText = "الحساب البنكي";
            this.AccountName.MinimumWidth = 150;
            this.AccountName.Name = "AccountName";
            this.AccountName.ReadOnly = true;
            // 
            // BranchID
            // 
            this.BranchID.DataPropertyName = "BranchID";
            this.BranchID.HeaderText = "BranchID";
            this.BranchID.Name = "BranchID";
            this.BranchID.ReadOnly = true;
            this.BranchID.Visible = false;
            // 
            // SafeID
            // 
            this.SafeID.DataPropertyName = "SafeID";
            this.SafeID.HeaderText = "SafeID";
            this.SafeID.Name = "SafeID";
            this.SafeID.ReadOnly = true;
            this.SafeID.Visible = false;
            // 
            // AccountID
            // 
            this.AccountID.DataPropertyName = "AccountID";
            this.AccountID.HeaderText = "AccountID";
            this.AccountID.Name = "AccountID";
            this.AccountID.ReadOnly = true;
            this.AccountID.Visible = false;
            // 
            // btn_edit
            // 
            this.btn_edit.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.btn_edit.HeaderText = "تعديل";
            this.btn_edit.Image = global::ERP.Desktop.Properties.Resources.edit;
            this.btn_edit.MinimumWidth = 30;
            this.btn_edit.Name = "btn_edit";
            this.btn_edit.ReadOnly = true;
            this.btn_edit.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.btn_edit.Width = 62;
            // 
            // btn_delete
            // 
            this.btn_delete.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.btn_delete.HeaderText = "حذف";
            this.btn_delete.Image = global::ERP.Desktop.Properties.Resources.trash;
            this.btn_delete.MinimumWidth = 30;
            this.btn_delete.Name = "btn_delete";
            this.btn_delete.ReadOnly = true;
            this.btn_delete.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.btn_delete.Width = 59;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.ckCanChangeItemPrice);
            this.groupBox1.Controls.Add(this.ckCanDiscount);
            this.groupBox1.Controls.Add(this.btnSaveChanges);
            this.groupBox1.Controls.Add(this.btnAdd);
            this.groupBox1.Controls.Add(this.btnCancelChanges);
            this.groupBox1.Controls.Add(this.cmbo_BankCard);
            this.groupBox1.Controls.Add(this.cmbPricePolicies);
            this.groupBox1.Controls.Add(this.cmbSuppliers);
            this.groupBox1.Controls.Add(this.cmbo_BankWallet);
            this.groupBox1.Controls.Add(this.cmbPaymentTypes);
            this.groupBox1.Controls.Add(this.cmbCustomers);
            this.groupBox1.Controls.Add(this.cmb_store);
            this.groupBox1.Controls.Add(this.label15);
            this.groupBox1.Controls.Add(this.cmb_safe);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label14);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.cmb_bankAccount);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.cmb_branchs);
            this.groupBox1.Controls.Add(this.label13);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.txt_Point);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.lblarea);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(689, 228);
            this.groupBox1.TabIndex = 195;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "نقاط البيع";
            // 
            // ckCanChangeItemPrice
            // 
            this.ckCanChangeItemPrice.AutoSize = true;
            this.ckCanChangeItemPrice.Location = new System.Drawing.Point(292, 197);
            this.ckCanChangeItemPrice.Name = "ckCanChangeItemPrice";
            this.ckCanChangeItemPrice.Size = new System.Drawing.Size(146, 20);
            this.ckCanChangeItemPrice.TabIndex = 211;
            this.ckCanChangeItemPrice.Text = "امكانية تغير سعر صنف";
            this.ckCanChangeItemPrice.UseVisualStyleBackColor = true;
            // 
            // ckCanDiscount
            // 
            this.ckCanDiscount.AutoSize = true;
            this.ckCanDiscount.Location = new System.Drawing.Point(486, 197);
            this.ckCanDiscount.Name = "ckCanDiscount";
            this.ckCanDiscount.Size = new System.Drawing.Size(175, 20);
            this.ckCanDiscount.TabIndex = 211;
            this.ckCanDiscount.Text = "امكانية الخصم علي الفاتورة";
            this.ckCanDiscount.UseVisualStyleBackColor = true;
            // 
            // btnSaveChanges
            // 
            this.btnSaveChanges.BackColor = System.Drawing.Color.Turquoise;
            this.btnSaveChanges.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSaveChanges.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.btnSaveChanges.ForeColor = System.Drawing.Color.White;
            this.btnSaveChanges.IconChar = FontAwesome.Sharp.IconChar.Save;
            this.btnSaveChanges.IconColor = System.Drawing.Color.White;
            this.btnSaveChanges.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnSaveChanges.IconSize = 22;
            this.btnSaveChanges.Location = new System.Drawing.Point(114, 191);
            this.btnSaveChanges.Name = "btnSaveChanges";
            this.btnSaveChanges.Size = new System.Drawing.Size(90, 30);
            this.btnSaveChanges.TabIndex = 210;
            this.btnSaveChanges.Text = "حفظ";
            this.btnSaveChanges.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnSaveChanges.UseVisualStyleBackColor = false;
            this.btnSaveChanges.Visible = false;
            this.btnSaveChanges.Click += new System.EventHandler(this.btnSaveChanges_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.BackColor = System.Drawing.Color.DeepSkyBlue;
            this.btnAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAdd.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.btnAdd.ForeColor = System.Drawing.Color.White;
            this.btnAdd.IconChar = FontAwesome.Sharp.IconChar.Plus;
            this.btnAdd.IconColor = System.Drawing.Color.White;
            this.btnAdd.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnAdd.IconSize = 22;
            this.btnAdd.Location = new System.Drawing.Point(19, 191);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(90, 30);
            this.btnAdd.TabIndex = 209;
            this.btnAdd.Text = "اضافة";
            this.btnAdd.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnAdd.UseVisualStyleBackColor = false;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnCancelChanges
            // 
            this.btnCancelChanges.BackColor = System.Drawing.Color.Crimson;
            this.btnCancelChanges.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancelChanges.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.btnCancelChanges.ForeColor = System.Drawing.Color.White;
            this.btnCancelChanges.IconChar = FontAwesome.Sharp.IconChar.Times;
            this.btnCancelChanges.IconColor = System.Drawing.Color.White;
            this.btnCancelChanges.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnCancelChanges.IconSize = 22;
            this.btnCancelChanges.Location = new System.Drawing.Point(18, 191);
            this.btnCancelChanges.Name = "btnCancelChanges";
            this.btnCancelChanges.Size = new System.Drawing.Size(90, 30);
            this.btnCancelChanges.TabIndex = 208;
            this.btnCancelChanges.Text = "الغاء";
            this.btnCancelChanges.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnCancelChanges.UseVisualStyleBackColor = false;
            this.btnCancelChanges.Visible = false;
            this.btnCancelChanges.Click += new System.EventHandler(this.btnCancelChanges_Click);
            // 
            // cmbo_BankCard
            // 
            this.cmbo_BankCard.FormattingEnabled = true;
            this.cmbo_BankCard.Location = new System.Drawing.Point(11, 138);
            this.cmbo_BankCard.Name = "cmbo_BankCard";
            this.cmbo_BankCard.Size = new System.Drawing.Size(180, 24);
            this.cmbo_BankCard.TabIndex = 207;
            // 
            // cmbPricePolicies
            // 
            this.cmbPricePolicies.FormattingEnabled = true;
            this.cmbPricePolicies.Location = new System.Drawing.Point(11, 108);
            this.cmbPricePolicies.Name = "cmbPricePolicies";
            this.cmbPricePolicies.Size = new System.Drawing.Size(180, 24);
            this.cmbPricePolicies.TabIndex = 207;
            // 
            // cmbSuppliers
            // 
            this.cmbSuppliers.FormattingEnabled = true;
            this.cmbSuppliers.Location = new System.Drawing.Point(11, 79);
            this.cmbSuppliers.Name = "cmbSuppliers";
            this.cmbSuppliers.Size = new System.Drawing.Size(180, 24);
            this.cmbSuppliers.TabIndex = 207;
            // 
            // cmbo_BankWallet
            // 
            this.cmbo_BankWallet.FormattingEnabled = true;
            this.cmbo_BankWallet.Location = new System.Drawing.Point(358, 168);
            this.cmbo_BankWallet.Name = "cmbo_BankWallet";
            this.cmbo_BankWallet.Size = new System.Drawing.Size(180, 24);
            this.cmbo_BankWallet.TabIndex = 207;
            // 
            // cmbPaymentTypes
            // 
            this.cmbPaymentTypes.FormattingEnabled = true;
            this.cmbPaymentTypes.Location = new System.Drawing.Point(358, 138);
            this.cmbPaymentTypes.Name = "cmbPaymentTypes";
            this.cmbPaymentTypes.Size = new System.Drawing.Size(180, 24);
            this.cmbPaymentTypes.TabIndex = 207;
            // 
            // cmbCustomers
            // 
            this.cmbCustomers.FormattingEnabled = true;
            this.cmbCustomers.Location = new System.Drawing.Point(358, 109);
            this.cmbCustomers.Name = "cmbCustomers";
            this.cmbCustomers.Size = new System.Drawing.Size(180, 24);
            this.cmbCustomers.TabIndex = 207;
            // 
            // cmb_safe
            // 
            this.cmb_safe.FormattingEnabled = true;
            this.cmb_safe.Location = new System.Drawing.Point(358, 50);
            this.cmb_safe.Name = "cmb_safe";
            this.cmb_safe.Size = new System.Drawing.Size(180, 24);
            this.cmb_safe.TabIndex = 207;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Tahoma", 9.75F);
            this.label6.ForeColor = System.Drawing.Color.Firebrick;
            this.label6.Location = new System.Drawing.Point(538, 54);
            this.label6.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(15, 16);
            this.label6.TabIndex = 206;
            this.label6.Text = "*";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(550, 113);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(107, 16);
            this.label10.TabIndex = 205;
            this.label10.Text = "العميل الافتراضي :";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(550, 54);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(53, 16);
            this.label7.TabIndex = 205;
            this.label7.Text = "الخزينة :";
            // 
            // cmb_bankAccount
            // 
            this.cmb_bankAccount.FormattingEnabled = true;
            this.cmb_bankAccount.Location = new System.Drawing.Point(11, 21);
            this.cmb_bankAccount.Name = "cmb_bankAccount";
            this.cmb_bankAccount.Size = new System.Drawing.Size(180, 24);
            this.cmb_bankAccount.TabIndex = 204;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Tahoma", 9.75F);
            this.label4.ForeColor = System.Drawing.Color.Firebrick;
            this.label4.Location = new System.Drawing.Point(538, 25);
            this.label4.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(15, 16);
            this.label4.TabIndex = 203;
            this.label4.Text = "*";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(550, 25);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(43, 16);
            this.label5.TabIndex = 202;
            this.label5.Text = "الفرع :";
            // 
            // cmb_branchs
            // 
            this.cmb_branchs.FormattingEnabled = true;
            this.cmb_branchs.Location = new System.Drawing.Point(358, 21);
            this.cmb_branchs.Name = "cmb_branchs";
            this.cmb_branchs.Size = new System.Drawing.Size(180, 24);
            this.cmb_branchs.TabIndex = 201;
            this.cmb_branchs.SelectedIndexChanged += new System.EventHandler(this.cmb_branchs_SelectedIndexChanged);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.Location = new System.Drawing.Point(541, 172);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(149, 16);
            this.label13.TabIndex = 199;
            this.label13.Text = "المحفظة البنكية الافتراضية";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 9.75F);
            this.label1.ForeColor = System.Drawing.Color.Firebrick;
            this.label1.Location = new System.Drawing.Point(192, 54);
            this.label1.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(15, 16);
            this.label1.TabIndex = 200;
            this.label1.Text = "*";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(202, 142);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(148, 16);
            this.label12.TabIndex = 199;
            this.label12.Text = "البطاقة البنكية الافتراضية :";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(549, 142);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(125, 16);
            this.label9.TabIndex = 199;
            this.label9.Text = " نوع الدفع الافتراضي :";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(202, 112);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(153, 16);
            this.label11.TabIndex = 199;
            this.label11.Text = " سياسة السعر الافتراضية :";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(202, 83);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(108, 16);
            this.label8.TabIndex = 199;
            this.label8.Text = " المورد الافتراضي :";
            // 
            // txt_Point
            // 
            this.txt_Point.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_Point.Location = new System.Drawing.Point(11, 51);
            this.txt_Point.Margin = new System.Windows.Forms.Padding(3, 22, 3, 22);
            this.txt_Point.Name = "txt_Point";
            this.txt_Point.Size = new System.Drawing.Size(180, 23);
            this.txt_Point.TabIndex = 198;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(202, 54);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(105, 16);
            this.label3.TabIndex = 199;
            this.label3.Text = " اسم نقطة البيع :";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Tahoma", 9.75F);
            this.label2.ForeColor = System.Drawing.Color.Firebrick;
            this.label2.Location = new System.Drawing.Point(192, 25);
            this.label2.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(15, 16);
            this.label2.TabIndex = 197;
            this.label2.Text = "*";
            // 
            // lblarea
            // 
            this.lblarea.AutoSize = true;
            this.lblarea.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblarea.Location = new System.Drawing.Point(202, 25);
            this.lblarea.Name = "lblarea";
            this.lblarea.Size = new System.Drawing.Size(102, 16);
            this.lblarea.TabIndex = 196;
            this.lblarea.Text = "الحساب البنكي :";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.Location = new System.Drawing.Point(550, 84);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(110, 16);
            this.label14.TabIndex = 205;
            this.label14.Text = "المخزن الافتراضى :";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Tahoma", 9.75F);
            this.label15.ForeColor = System.Drawing.Color.Firebrick;
            this.label15.Location = new System.Drawing.Point(538, 84);
            this.label15.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(15, 16);
            this.label15.TabIndex = 206;
            this.label15.Text = "*";
            // 
            // cmb_store
            // 
            this.cmb_store.FormattingEnabled = true;
            this.cmb_store.Location = new System.Drawing.Point(358, 80);
            this.cmb_store.Name = "cmb_store";
            this.cmb_store.Size = new System.Drawing.Size(180, 24);
            this.cmb_store.TabIndex = 207;
            // 
            // PointOfSales
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(689, 504);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "PointOfSales";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "نقاط البيع";
            this.Load += new System.EventHandler(this.PointOfSales_Load);
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgv_pos)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.DataGridView dgv_pos;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox cmb_safe;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox cmb_bankAccount;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cmb_branchs;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txt_Point;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblarea;
        private System.Windows.Forms.DataGridViewTextBoxColumn PointId;
        private System.Windows.Forms.DataGridViewTextBoxColumn colName;
        private System.Windows.Forms.DataGridViewTextBoxColumn BranchName;
        private System.Windows.Forms.DataGridViewTextBoxColumn SafeName;
        private System.Windows.Forms.DataGridViewTextBoxColumn AccountName;
        private System.Windows.Forms.DataGridViewTextBoxColumn BranchID;
        private System.Windows.Forms.DataGridViewTextBoxColumn SafeID;
        private System.Windows.Forms.DataGridViewTextBoxColumn AccountID;
        private System.Windows.Forms.DataGridViewImageColumn btn_edit;
        private System.Windows.Forms.DataGridViewImageColumn btn_delete;
        private Utilities.ReusableControls.SuccessButton btnSaveChanges;
        private Utilities.ReusableControls.DefaultButton btnAdd;
        private Utilities.ReusableControls.DangerButton btnCancelChanges;
        private System.Windows.Forms.ComboBox cmbPaymentTypes;
        private System.Windows.Forms.ComboBox cmbCustomers;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.CheckBox ckCanChangeItemPrice;
        private System.Windows.Forms.CheckBox ckCanDiscount;
        private System.Windows.Forms.ComboBox cmbPricePolicies;
        private System.Windows.Forms.ComboBox cmbSuppliers;
        private System.Windows.Forms.ComboBox cmbo_BankCard;
        private System.Windows.Forms.ComboBox cmbo_BankWallet;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.ComboBox cmb_store;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label14;
    }
}