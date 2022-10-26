namespace ERP.Desktop.Views.Actors.Customers
{
    partial class CustomerSelect
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.rbCustomer = new System.Windows.Forms.RadioButton();
            this.rbCustAndSupplier = new System.Windows.Forms.RadioButton();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.rbGenderFemale = new System.Windows.Forms.RadioButton();
            this.rbGenderMale = new System.Windows.Forms.RadioButton();
            this.btnSaveChanges = new ERP.Desktop.Utilities.ReusableControls.SuccessButton();
            this.btnAddCustomer = new ERP.Desktop.Utilities.ReusableControls.DefaultButton();
            this.btnCancel = new ERP.Desktop.Utilities.ReusableControls.DangerButton();
            this.cmbCountries = new System.Windows.Forms.ComboBox();
            this.txtAddress = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.cmbAreas = new System.Windows.Forms.ComboBox();
            this.label22 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.cmbCities = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtMob2 = new System.Windows.Forms.TextBox();
            this.txtMob1 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.label26 = new System.Windows.Forms.Label();
            this.txt_search = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.dgv_customer = new System.Windows.Forms.DataGridView();
            this.Person_ID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Person_Name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Mobil1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Mobil2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Cus_Address = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.edit = new System.Windows.Forms.DataGridViewImageColumn();
            this.delete = new System.Windows.Forms.DataGridViewImageColumn();
            this.groupBox1.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_customer)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.groupBox1.Controls.Add(this.groupBox4);
            this.groupBox1.Controls.Add(this.groupBox3);
            this.groupBox1.Controls.Add(this.btnSaveChanges);
            this.groupBox1.Controls.Add(this.btnAddCustomer);
            this.groupBox1.Controls.Add(this.btnCancel);
            this.groupBox1.Controls.Add(this.cmbCountries);
            this.groupBox1.Controls.Add(this.txtAddress);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.label20);
            this.groupBox1.Controls.Add(this.label23);
            this.groupBox1.Controls.Add(this.label17);
            this.groupBox1.Controls.Add(this.cmbAreas);
            this.groupBox1.Controls.Add(this.label22);
            this.groupBox1.Controls.Add(this.label14);
            this.groupBox1.Controls.Add(this.cmbCities);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txtName);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.txtMob2);
            this.groupBox1.Controls.Add(this.txtMob1);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label19);
            this.groupBox1.Controls.Add(this.label26);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(3, 22, 3, 22);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(3, 22, 3, 22);
            this.groupBox1.Size = new System.Drawing.Size(854, 149);
            this.groupBox1.TabIndex = 12;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "تسجيل العميل";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.rbCustomer);
            this.groupBox4.Controls.Add(this.rbCustAndSupplier);
            this.groupBox4.Location = new System.Drawing.Point(281, 102);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(253, 44);
            this.groupBox4.TabIndex = 134;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "نوع العميل";
            // 
            // rbCustomer
            // 
            this.rbCustomer.AutoSize = true;
            this.rbCustomer.Checked = true;
            this.rbCustomer.Location = new System.Drawing.Point(191, 17);
            this.rbCustomer.Name = "rbCustomer";
            this.rbCustomer.Size = new System.Drawing.Size(55, 20);
            this.rbCustomer.TabIndex = 132;
            this.rbCustomer.TabStop = true;
            this.rbCustomer.Text = "عميل";
            this.rbCustomer.UseVisualStyleBackColor = true;
            // 
            // rbCustAndSupplier
            // 
            this.rbCustAndSupplier.AutoSize = true;
            this.rbCustAndSupplier.Location = new System.Drawing.Point(6, 17);
            this.rbCustAndSupplier.Name = "rbCustAndSupplier";
            this.rbCustAndSupplier.Size = new System.Drawing.Size(92, 20);
            this.rbCustAndSupplier.TabIndex = 132;
            this.rbCustAndSupplier.Text = "عميل و مورد";
            this.rbCustAndSupplier.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.rbGenderFemale);
            this.groupBox3.Controls.Add(this.rbGenderMale);
            this.groupBox3.Location = new System.Drawing.Point(598, 102);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(150, 44);
            this.groupBox3.TabIndex = 133;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "النوع";
            // 
            // rbGenderFemale
            // 
            this.rbGenderFemale.AutoSize = true;
            this.rbGenderFemale.Location = new System.Drawing.Point(6, 17);
            this.rbGenderFemale.Name = "rbGenderFemale";
            this.rbGenderFemale.Size = new System.Drawing.Size(50, 20);
            this.rbGenderFemale.TabIndex = 132;
            this.rbGenderFemale.Text = "انثي";
            this.rbGenderFemale.UseVisualStyleBackColor = true;
            // 
            // rbGenderMale
            // 
            this.rbGenderMale.AutoSize = true;
            this.rbGenderMale.Checked = true;
            this.rbGenderMale.Location = new System.Drawing.Point(101, 17);
            this.rbGenderMale.Name = "rbGenderMale";
            this.rbGenderMale.Size = new System.Drawing.Size(42, 20);
            this.rbGenderMale.TabIndex = 132;
            this.rbGenderMale.TabStop = true;
            this.rbGenderMale.Text = "ذكر";
            this.rbGenderMale.UseVisualStyleBackColor = true;
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
            this.btnSaveChanges.Location = new System.Drawing.Point(106, 107);
            this.btnSaveChanges.Name = "btnSaveChanges";
            this.btnSaveChanges.Size = new System.Drawing.Size(90, 30);
            this.btnSaveChanges.TabIndex = 131;
            this.btnSaveChanges.Text = "حفظ";
            this.btnSaveChanges.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnSaveChanges.UseVisualStyleBackColor = false;
            this.btnSaveChanges.Visible = false;
            this.btnSaveChanges.Click += new System.EventHandler(this.btnSaveChanges_Click);
            // 
            // btnAddCustomer
            // 
            this.btnAddCustomer.BackColor = System.Drawing.Color.DeepSkyBlue;
            this.btnAddCustomer.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddCustomer.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.btnAddCustomer.ForeColor = System.Drawing.Color.White;
            this.btnAddCustomer.IconChar = FontAwesome.Sharp.IconChar.Plus;
            this.btnAddCustomer.IconColor = System.Drawing.Color.White;
            this.btnAddCustomer.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnAddCustomer.IconSize = 22;
            this.btnAddCustomer.Location = new System.Drawing.Point(10, 107);
            this.btnAddCustomer.Name = "btnAddCustomer";
            this.btnAddCustomer.Size = new System.Drawing.Size(90, 30);
            this.btnAddCustomer.TabIndex = 130;
            this.btnAddCustomer.Text = "اضافة";
            this.btnAddCustomer.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnAddCustomer.UseVisualStyleBackColor = false;
            this.btnAddCustomer.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.Crimson;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.btnCancel.ForeColor = System.Drawing.Color.White;
            this.btnCancel.IconChar = FontAwesome.Sharp.IconChar.Times;
            this.btnCancel.IconColor = System.Drawing.Color.White;
            this.btnCancel.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnCancel.IconSize = 22;
            this.btnCancel.Location = new System.Drawing.Point(10, 107);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(90, 30);
            this.btnCancel.TabIndex = 129;
            this.btnCancel.Text = "الغاء";
            this.btnCancel.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Visible = false;
            this.btnCancel.Click += new System.EventHandler(this.btn_cancel_Click);
            // 
            // cmbCountries
            // 
            this.cmbCountries.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbCountries.FormattingEnabled = true;
            this.cmbCountries.Location = new System.Drawing.Point(598, 48);
            this.cmbCountries.Margin = new System.Windows.Forms.Padding(3, 1, 3, 1);
            this.cmbCountries.Name = "cmbCountries";
            this.cmbCountries.Size = new System.Drawing.Size(150, 24);
            this.cmbCountries.TabIndex = 116;
            this.cmbCountries.SelectedIndexChanged += new System.EventHandler(this.cmbCountries_SelectedIndexChanged);
            this.cmbCountries.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cmbCountries_KeyDown);
            // 
            // txtAddress
            // 
            this.txtAddress.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAddress.Location = new System.Drawing.Point(10, 77);
            this.txtAddress.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtAddress.Name = "txtAddress";
            this.txtAddress.Size = new System.Drawing.Size(738, 23);
            this.txtAddress.TabIndex = 112;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(760, 80);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(52, 16);
            this.label10.TabIndex = 113;
            this.label10.Text = "العنوان :";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label20.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label20.Location = new System.Drawing.Point(760, 52);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(40, 16);
            this.label20.TabIndex = 123;
            this.label20.Text = "البلد :";
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label23.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label23.Location = new System.Drawing.Point(444, 52);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(54, 16);
            this.label23.TabIndex = 125;
            this.label23.Text = "المدينة :";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label17.ForeColor = System.Drawing.Color.Firebrick;
            this.label17.Location = new System.Drawing.Point(161, 52);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(15, 16);
            this.label17.TabIndex = 128;
            this.label17.Text = "*";
            // 
            // cmbAreas
            // 
            this.cmbAreas.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbAreas.FormattingEnabled = true;
            this.cmbAreas.Location = new System.Drawing.Point(10, 48);
            this.cmbAreas.Margin = new System.Windows.Forms.Padding(3, 1, 3, 1);
            this.cmbAreas.Name = "cmbAreas";
            this.cmbAreas.Size = new System.Drawing.Size(150, 24);
            this.cmbAreas.TabIndex = 118;
            this.cmbAreas.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cmbAreas_KeyDown);
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label22.ForeColor = System.Drawing.Color.Firebrick;
            this.label22.Location = new System.Drawing.Point(433, 23);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(15, 16);
            this.label22.TabIndex = 126;
            this.label22.Text = "*";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.ForeColor = System.Drawing.Color.Firebrick;
            this.label14.Location = new System.Drawing.Point(748, 52);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(15, 16);
            this.label14.TabIndex = 124;
            this.label14.Text = "*";
            // 
            // cmbCities
            // 
            this.cmbCities.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbCities.FormattingEnabled = true;
            this.cmbCities.Location = new System.Drawing.Point(281, 48);
            this.cmbCities.Margin = new System.Windows.Forms.Padding(3, 1, 3, 1);
            this.cmbCities.Name = "cmbCities";
            this.cmbCities.Size = new System.Drawing.Size(150, 24);
            this.cmbCities.TabIndex = 117;
            this.cmbCities.SelectedIndexChanged += new System.EventHandler(this.cmbCities_SelectedIndexChanged);
            this.cmbCities.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cmbCities_KeyDown);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label2.Location = new System.Drawing.Point(172, 52);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 16);
            this.label2.TabIndex = 127;
            this.label2.Text = "المنطقة :";
            // 
            // txtName
            // 
            this.txtName.Font = new System.Drawing.Font("Tahoma", 9.75F);
            this.txtName.Location = new System.Drawing.Point(598, 19);
            this.txtName.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(150, 23);
            this.txtName.TabIndex = 4;
            this.txtName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txt_name_KeyDown);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Tahoma", 9.75F);
            this.label5.Location = new System.Drawing.Point(172, 22);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(92, 16);
            this.label5.TabIndex = 71;
            this.label5.Text = "رقم الموبايل 2 :";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Tahoma", 9.75F);
            this.label4.Location = new System.Drawing.Point(444, 22);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(92, 16);
            this.label4.TabIndex = 72;
            this.label4.Text = "رقم الموبايل 1 :";
            // 
            // txtMob2
            // 
            this.txtMob2.Font = new System.Drawing.Font("Tahoma", 9.75F);
            this.txtMob2.Location = new System.Drawing.Point(10, 19);
            this.txtMob2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtMob2.MaxLength = 20;
            this.txtMob2.Name = "txtMob2";
            this.txtMob2.Size = new System.Drawing.Size(150, 23);
            this.txtMob2.TabIndex = 11;
            this.txtMob2.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txt_mob2_KeyDown);
            this.txtMob2.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtNumbersOnly_KeyPress);
            // 
            // txtMob1
            // 
            this.txtMob1.Font = new System.Drawing.Font("Tahoma", 9.75F);
            this.txtMob1.Location = new System.Drawing.Point(281, 19);
            this.txtMob1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtMob1.MaxLength = 20;
            this.txtMob1.Name = "txtMob1";
            this.txtMob1.Size = new System.Drawing.Size(150, 23);
            this.txtMob1.TabIndex = 10;
            this.txtMob1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txt_mob1_KeyDown);
            this.txtMob1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtNumbersOnly_KeyPress);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Tahoma", 9.75F);
            this.label3.Location = new System.Drawing.Point(760, 22);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(80, 16);
            this.label3.TabIndex = 55;
            this.label3.Text = "اسم العميل :";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Font = new System.Drawing.Font("Tahoma", 9.75F);
            this.label19.ForeColor = System.Drawing.Color.Firebrick;
            this.label19.Location = new System.Drawing.Point(433, 52);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(15, 16);
            this.label19.TabIndex = 73;
            this.label19.Text = "*";
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Font = new System.Drawing.Font("Tahoma", 9.75F);
            this.label26.ForeColor = System.Drawing.Color.Firebrick;
            this.label26.Location = new System.Drawing.Point(748, 22);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(15, 16);
            this.label26.TabIndex = 111;
            this.label26.Text = "*";
            // 
            // txt_search
            // 
            this.txt_search.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_search.Location = new System.Drawing.Point(526, 18);
            this.txt_search.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txt_search.Name = "txt_search";
            this.txt_search.Size = new System.Drawing.Size(222, 23);
            this.txt_search.TabIndex = 0;
            this.txt_search.TextChanged += new System.EventHandler(this.txt_search_TextChanged);
            this.txt_search.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txt_search_KeyDown);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label13.Location = new System.Drawing.Point(760, 21);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(63, 16);
            this.label13.TabIndex = 101;
            this.label13.Text = "بحث عن :";
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.Color.WhiteSmoke;
            this.groupBox2.Controls.Add(this.dgv_customer);
            this.groupBox2.Controls.Add(this.txt_search);
            this.groupBox2.Controls.Add(this.label13);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.groupBox2.Location = new System.Drawing.Point(0, 148);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(3, 22, 3, 22);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(3, 22, 3, 22);
            this.groupBox2.Size = new System.Drawing.Size(854, 396);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "سجل العملاء";
            // 
            // dgv_customer
            // 
            this.dgv_customer.AllowUserToAddRows = false;
            this.dgv_customer.AllowUserToDeleteRows = false;
            this.dgv_customer.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_customer.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Person_ID,
            this.Person_Name,
            this.Mobil1,
            this.Mobil2,
            this.Cus_Address,
            this.edit,
            this.delete});
            this.dgv_customer.Location = new System.Drawing.Point(6, 49);
            this.dgv_customer.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.dgv_customer.Name = "dgv_customer";
            this.dgv_customer.ReadOnly = true;
            this.dgv_customer.Size = new System.Drawing.Size(841, 347);
            this.dgv_customer.TabIndex = 103;
            this.dgv_customer.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvCustomers_CellClick);
            this.dgv_customer.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgv_customer_CellDoubleClick);
            this.dgv_customer.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dgv_customer_KeyDown);
            // 
            // Person_ID
            // 
            this.Person_ID.DataPropertyName = "ID";
            this.Person_ID.HeaderText = "Person_ID";
            this.Person_ID.Name = "Person_ID";
            this.Person_ID.ReadOnly = true;
            this.Person_ID.Visible = false;
            // 
            // Person_Name
            // 
            this.Person_Name.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Person_Name.DataPropertyName = "Name";
            this.Person_Name.HeaderText = "الاسم";
            this.Person_Name.Name = "Person_Name";
            this.Person_Name.ReadOnly = true;
            // 
            // Mobil1
            // 
            this.Mobil1.DataPropertyName = "Mob1";
            this.Mobil1.HeaderText = "موبايل 1";
            this.Mobil1.Name = "Mobil1";
            this.Mobil1.ReadOnly = true;
            // 
            // Mobil2
            // 
            this.Mobil2.DataPropertyName = "Mob2";
            this.Mobil2.HeaderText = "موبايل 2";
            this.Mobil2.Name = "Mobil2";
            this.Mobil2.ReadOnly = true;
            // 
            // Cus_Address
            // 
            this.Cus_Address.DataPropertyName = "Address";
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.Cus_Address.DefaultCellStyle = dataGridViewCellStyle2;
            this.Cus_Address.HeaderText = "العنوان";
            this.Cus_Address.Name = "Cus_Address";
            this.Cus_Address.ReadOnly = true;
            this.Cus_Address.Width = 200;
            // 
            // edit
            // 
            this.edit.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.edit.HeaderText = "تعديل";
            this.edit.Image = global::ERP.Desktop.Properties.Resources.edit;
            this.edit.Name = "edit";
            this.edit.ReadOnly = true;
            this.edit.Width = 43;
            // 
            // delete
            // 
            this.delete.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.delete.HeaderText = "حذف";
            this.delete.Image = global::ERP.Desktop.Properties.Resources.trash;
            this.delete.Name = "delete";
            this.delete.ReadOnly = true;
            this.delete.Width = 40;
            // 
            // CustomerSelect
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(854, 544);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MinimizeBox = false;
            this.Name = "CustomerSelect";
            this.Tag = "40";
            this.Text = "العملاء";
            this.Load += new System.EventHandler(this.Customers_Load);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.DeliveryCustomer_KeyUp);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_customer)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        public System.Windows.Forms.TextBox txtMob2;
        public System.Windows.Forms.TextBox txtMob1;
        public System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.TextBox txt_search;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.DataGridView dgv_customer;
        public System.Windows.Forms.ComboBox cmbCountries;
        public System.Windows.Forms.Label label20;
        public System.Windows.Forms.Label label23;
        private System.Windows.Forms.Label label17;
        public System.Windows.Forms.ComboBox cmbAreas;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Label label14;
        public System.Windows.Forms.ComboBox cmbCities;
        public System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label10;
        private Utilities.ReusableControls.SuccessButton btnSaveChanges;
        private Utilities.ReusableControls.DangerButton btnCancel;
        private System.Windows.Forms.RadioButton rbGenderFemale;
        private System.Windows.Forms.RadioButton rbGenderMale;
        public System.Windows.Forms.TextBox txtAddress;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.RadioButton rbCustomer;
        private System.Windows.Forms.RadioButton rbCustAndSupplier;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Person_ID;
        private System.Windows.Forms.DataGridViewTextBoxColumn Person_Name;
        private System.Windows.Forms.DataGridViewTextBoxColumn Mobil1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Mobil2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Cus_Address;
        private System.Windows.Forms.DataGridViewImageColumn edit;
        private System.Windows.Forms.DataGridViewImageColumn delete;
        private Utilities.ReusableControls.DefaultButton btnAddCustomer;
    }
}