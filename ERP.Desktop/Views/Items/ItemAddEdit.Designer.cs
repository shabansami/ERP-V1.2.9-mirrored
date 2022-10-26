
namespace ERP.Desktop.Views.Items
{
    partial class ItemAddEdit
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
            this.ckIsActive = new System.Windows.Forms.CheckBox();
            this.txt_MinPrice = new System.Windows.Forms.TextBox();
            this.label26 = new System.Windows.Forms.Label();
            this.txt_SellPrice = new System.Windows.Forms.TextBox();
            this.label25 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.txt_Name = new System.Windows.Forms.TextBox();
            this.txt_ItemCode = new System.Windows.Forms.TextBox();
            this.label35 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.cmb_Units = new System.Windows.Forms.ComboBox();
            this.label18 = new System.Windows.Forms.Label();
            this.txt_Barcode = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.btnCancelChanges = new ERP.Desktop.Utilities.ReusableControls.DangerButton();
            this.btn_Add = new ERP.Desktop.Utilities.ReusableControls.DefaultButton();
            this.txt_TechnicalSpecifications = new System.Windows.Forms.TextBox();
            this.btnSaveChanges = new ERP.Desktop.Utilities.ReusableControls.SuccessButton();
            this.label7 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txt_MaxPrice = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.cmb_ItemType = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.cmb_GroupSells = new System.Windows.Forms.ComboBox();
            this.cmb_GroupBasics = new System.Windows.Forms.ComboBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.gbPrices = new System.Windows.Forms.GroupBox();
            this.dgvPrices = new System.Windows.Forms.DataGridView();
            this.colID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.policyID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPrice = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colEdit = new System.Windows.Forms.DataGridViewImageColumn();
            this.colDelete = new System.Windows.Forms.DataGridViewImageColumn();
            this.label10 = new System.Windows.Forms.Label();
            this.btnCancelPolcy = new ERP.Desktop.Utilities.ReusableControls.DangerButton();
            this.btnAddPolicy = new ERP.Desktop.Utilities.ReusableControls.DefaultButton();
            this.cmbPricePolicies = new System.Windows.Forms.ComboBox();
            this.txtPolicyPrice = new ERP.Desktop.Utilities.ReusableControls.TextBoxNum();
            this.btnSavePolicyChanges = new ERP.Desktop.Utilities.ReusableControls.SuccessButton();
            this.label13 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.groupBox4.SuspendLayout();
            this.panel1.SuspendLayout();
            this.gbPrices.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPrices)).BeginInit();
            this.SuspendLayout();
            // 
            // ckIsActive
            // 
            this.ckIsActive.AutoSize = true;
            this.ckIsActive.Checked = true;
            this.ckIsActive.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ckIsActive.Font = new System.Drawing.Font("Tahoma", 9.75F);
            this.ckIsActive.Location = new System.Drawing.Point(400, 224);
            this.ckIsActive.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.ckIsActive.Name = "ckIsActive";
            this.ckIsActive.Size = new System.Drawing.Size(85, 20);
            this.ckIsActive.TabIndex = 18;
            this.ckIsActive.Text = "متوفر للبيع";
            this.ckIsActive.UseVisualStyleBackColor = true;
            // 
            // txt_MinPrice
            // 
            this.txt_MinPrice.Font = new System.Drawing.Font("Tahoma", 9.75F);
            this.txt_MinPrice.Location = new System.Drawing.Point(303, 154);
            this.txt_MinPrice.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txt_MinPrice.Name = "txt_MinPrice";
            this.txt_MinPrice.Size = new System.Drawing.Size(182, 23);
            this.txt_MinPrice.TabIndex = 10;
            this.txt_MinPrice.Text = "0.0";
            this.txt_MinPrice.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txt_MinPrice.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txt_MinPrice_KeyPress);
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Font = new System.Drawing.Font("Tahoma", 9.75F);
            this.label26.Location = new System.Drawing.Point(200, 157);
            this.label26.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(73, 16);
            this.label26.TabIndex = 86;
            this.label26.Text = "اعلي سعر :";
            // 
            // txt_SellPrice
            // 
            this.txt_SellPrice.Font = new System.Drawing.Font("Tahoma", 9.75F);
            this.txt_SellPrice.Location = new System.Drawing.Point(9, 123);
            this.txt_SellPrice.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txt_SellPrice.Name = "txt_SellPrice";
            this.txt_SellPrice.Size = new System.Drawing.Size(182, 23);
            this.txt_SellPrice.TabIndex = 9;
            this.txt_SellPrice.Text = "0.0";
            this.txt_SellPrice.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txt_SellPrice.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txt_SellPrice_KeyPress);
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Font = new System.Drawing.Font("Tahoma", 9.75F);
            this.label25.ForeColor = System.Drawing.Color.Firebrick;
            this.label25.Location = new System.Drawing.Point(192, 126);
            this.label25.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(15, 16);
            this.label25.TabIndex = 95;
            this.label25.Text = "*";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 9.75F);
            this.label1.Location = new System.Drawing.Point(496, 27);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(79, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "اسم الصنف :";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Tahoma", 9.75F);
            this.label11.Location = new System.Drawing.Point(203, 126);
            this.label11.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(69, 16);
            this.label11.TabIndex = 87;
            this.label11.Text = "سعر البيع :";
            // 
            // txt_Name
            // 
            this.txt_Name.Font = new System.Drawing.Font("Tahoma", 9.75F);
            this.txt_Name.Location = new System.Drawing.Point(303, 24);
            this.txt_Name.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txt_Name.Name = "txt_Name";
            this.txt_Name.Size = new System.Drawing.Size(182, 23);
            this.txt_Name.TabIndex = 0;
            // 
            // txt_ItemCode
            // 
            this.txt_ItemCode.Font = new System.Drawing.Font("Tahoma", 9.75F);
            this.txt_ItemCode.Location = new System.Drawing.Point(303, 57);
            this.txt_ItemCode.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txt_ItemCode.Name = "txt_ItemCode";
            this.txt_ItemCode.Size = new System.Drawing.Size(182, 23);
            this.txt_ItemCode.TabIndex = 15;
            this.txt_ItemCode.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txt_ItemCode_KeyPress);
            // 
            // label35
            // 
            this.label35.AutoSize = true;
            this.label35.Font = new System.Drawing.Font("Tahoma", 9.75F);
            this.label35.Location = new System.Drawing.Point(496, 60);
            this.label35.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label35.Name = "label35";
            this.label35.Size = new System.Drawing.Size(72, 16);
            this.label35.TabIndex = 96;
            this.label35.Text = "كود الصنف :";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Tahoma", 9.75F);
            this.label15.Location = new System.Drawing.Point(201, 28);
            this.label15.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(51, 16);
            this.label15.TabIndex = 6;
            this.label15.Text = "الوحدة :";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Tahoma", 9.75F);
            this.label12.ForeColor = System.Drawing.Color.Firebrick;
            this.label12.Location = new System.Drawing.Point(191, 28);
            this.label12.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(15, 16);
            this.label12.TabIndex = 68;
            this.label12.Text = "*";
            // 
            // cmb_Units
            // 
            this.cmb_Units.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cmb_Units.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmb_Units.Font = new System.Drawing.Font("Tahoma", 9.75F);
            this.cmb_Units.FormattingEnabled = true;
            this.cmb_Units.Location = new System.Drawing.Point(9, 24);
            this.cmb_Units.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cmb_Units.Name = "cmb_Units";
            this.cmb_Units.Size = new System.Drawing.Size(182, 24);
            this.cmb_Units.TabIndex = 8;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Font = new System.Drawing.Font("Tahoma", 9.75F);
            this.label18.ForeColor = System.Drawing.Color.Firebrick;
            this.label18.Location = new System.Drawing.Point(485, 27);
            this.label18.Margin = new System.Windows.Forms.Padding(0);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(15, 16);
            this.label18.TabIndex = 67;
            this.label18.Text = "*";
            // 
            // txt_Barcode
            // 
            this.txt_Barcode.Font = new System.Drawing.Font("Tahoma", 9.75F);
            this.txt_Barcode.Location = new System.Drawing.Point(9, 57);
            this.txt_Barcode.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txt_Barcode.Name = "txt_Barcode";
            this.txt_Barcode.Size = new System.Drawing.Size(182, 23);
            this.txt_Barcode.TabIndex = 14;
            this.txt_Barcode.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Tahoma", 9.75F);
            this.label3.Location = new System.Drawing.Point(200, 60);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(55, 16);
            this.label3.TabIndex = 72;
            this.label3.Text = "الباركود :";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.label9);
            this.groupBox4.Controls.Add(this.label8);
            this.groupBox4.Controls.Add(this.btnCancelChanges);
            this.groupBox4.Controls.Add(this.btn_Add);
            this.groupBox4.Controls.Add(this.txt_TechnicalSpecifications);
            this.groupBox4.Controls.Add(this.txt_MinPrice);
            this.groupBox4.Controls.Add(this.btnSaveChanges);
            this.groupBox4.Controls.Add(this.label7);
            this.groupBox4.Controls.Add(this.label5);
            this.groupBox4.Controls.Add(this.txt_MaxPrice);
            this.groupBox4.Controls.Add(this.ckIsActive);
            this.groupBox4.Controls.Add(this.label6);
            this.groupBox4.Controls.Add(this.cmb_ItemType);
            this.groupBox4.Controls.Add(this.label2);
            this.groupBox4.Controls.Add(this.label4);
            this.groupBox4.Controls.Add(this.cmb_GroupSells);
            this.groupBox4.Controls.Add(this.cmb_GroupBasics);
            this.groupBox4.Controls.Add(this.label26);
            this.groupBox4.Controls.Add(this.txt_SellPrice);
            this.groupBox4.Controls.Add(this.label25);
            this.groupBox4.Controls.Add(this.label1);
            this.groupBox4.Controls.Add(this.label11);
            this.groupBox4.Controls.Add(this.txt_Name);
            this.groupBox4.Controls.Add(this.txt_ItemCode);
            this.groupBox4.Controls.Add(this.label35);
            this.groupBox4.Controls.Add(this.label15);
            this.groupBox4.Controls.Add(this.label12);
            this.groupBox4.Controls.Add(this.cmb_Units);
            this.groupBox4.Controls.Add(this.label18);
            this.groupBox4.Controls.Add(this.txt_Barcode);
            this.groupBox4.Controls.Add(this.label3);
            this.groupBox4.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox4.Location = new System.Drawing.Point(6, 6);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.groupBox4.Size = new System.Drawing.Size(631, 261);
            this.groupBox4.TabIndex = 0;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "بيانات الصنف الاساسية";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Tahoma", 9.75F);
            this.label9.Location = new System.Drawing.Point(200, 93);
            this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(90, 16);
            this.label9.TabIndex = 215;
            this.label9.Text = "مجموعة البيع :";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Tahoma", 9.75F);
            this.label8.Location = new System.Drawing.Point(496, 94);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(126, 16);
            this.label8.TabIndex = 213;
            this.label8.Text = "المجموعة الاساسية :";
            // 
            // btnCancelChanges
            // 
            this.btnCancelChanges.BackColor = System.Drawing.Color.Crimson;
            this.btnCancelChanges.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancelChanges.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancelChanges.ForeColor = System.Drawing.Color.White;
            this.btnCancelChanges.IconChar = FontAwesome.Sharp.IconChar.Times;
            this.btnCancelChanges.IconColor = System.Drawing.Color.White;
            this.btnCancelChanges.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnCancelChanges.IconSize = 22;
            this.btnCancelChanges.Location = new System.Drawing.Point(9, 222);
            this.btnCancelChanges.Name = "btnCancelChanges";
            this.btnCancelChanges.Size = new System.Drawing.Size(80, 30);
            this.btnCancelChanges.TabIndex = 212;
            this.btnCancelChanges.Text = "رجوع";
            this.btnCancelChanges.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnCancelChanges.UseVisualStyleBackColor = false;
            this.btnCancelChanges.Visible = false;
            this.btnCancelChanges.Click += new System.EventHandler(this.btnCancelChanges_Click);
            // 
            // btn_Add
            // 
            this.btn_Add.BackColor = System.Drawing.Color.DeepSkyBlue;
            this.btn_Add.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Add.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_Add.ForeColor = System.Drawing.Color.White;
            this.btn_Add.IconChar = FontAwesome.Sharp.IconChar.Plus;
            this.btn_Add.IconColor = System.Drawing.Color.White;
            this.btn_Add.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btn_Add.IconSize = 22;
            this.btn_Add.Location = new System.Drawing.Point(9, 222);
            this.btn_Add.Name = "btn_Add";
            this.btn_Add.Size = new System.Drawing.Size(80, 30);
            this.btn_Add.TabIndex = 210;
            this.btn_Add.Text = "إضافة";
            this.btn_Add.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btn_Add.UseVisualStyleBackColor = false;
            this.btn_Add.Click += new System.EventHandler(this.btn_Add_Click);
            // 
            // txt_TechnicalSpecifications
            // 
            this.txt_TechnicalSpecifications.Font = new System.Drawing.Font("Tahoma", 9.75F);
            this.txt_TechnicalSpecifications.Location = new System.Drawing.Point(9, 191);
            this.txt_TechnicalSpecifications.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txt_TechnicalSpecifications.Name = "txt_TechnicalSpecifications";
            this.txt_TechnicalSpecifications.Size = new System.Drawing.Size(476, 23);
            this.txt_TechnicalSpecifications.TabIndex = 107;
            // 
            // btnSaveChanges
            // 
            this.btnSaveChanges.BackColor = System.Drawing.Color.DarkTurquoise;
            this.btnSaveChanges.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSaveChanges.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSaveChanges.ForeColor = System.Drawing.Color.White;
            this.btnSaveChanges.IconChar = FontAwesome.Sharp.IconChar.Save;
            this.btnSaveChanges.IconColor = System.Drawing.Color.White;
            this.btnSaveChanges.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnSaveChanges.IconSize = 22;
            this.btnSaveChanges.Location = new System.Drawing.Point(95, 222);
            this.btnSaveChanges.Name = "btnSaveChanges";
            this.btnSaveChanges.Size = new System.Drawing.Size(80, 30);
            this.btnSaveChanges.TabIndex = 211;
            this.btnSaveChanges.Text = "حفظ";
            this.btnSaveChanges.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnSaveChanges.UseVisualStyleBackColor = false;
            this.btnSaveChanges.Visible = false;
            this.btnSaveChanges.Click += new System.EventHandler(this.btnSaveChanges_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Tahoma", 9.75F);
            this.label7.Location = new System.Drawing.Point(496, 157);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(61, 16);
            this.label7.TabIndex = 106;
            this.label7.Text = "اقل سعر :";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Tahoma", 9.75F);
            this.label5.Location = new System.Drawing.Point(496, 194);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(104, 16);
            this.label5.TabIndex = 108;
            this.label5.Text = "الموصفات الفنية :";
            // 
            // txt_MaxPrice
            // 
            this.txt_MaxPrice.Font = new System.Drawing.Font("Tahoma", 9.75F);
            this.txt_MaxPrice.Location = new System.Drawing.Point(9, 154);
            this.txt_MaxPrice.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txt_MaxPrice.Name = "txt_MaxPrice";
            this.txt_MaxPrice.Size = new System.Drawing.Size(182, 23);
            this.txt_MaxPrice.TabIndex = 105;
            this.txt_MaxPrice.Text = "0.0";
            this.txt_MaxPrice.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txt_MaxPrice.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txt_MaxPrice_KeyPress);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Tahoma", 9.75F);
            this.label6.ForeColor = System.Drawing.Color.Firebrick;
            this.label6.Location = new System.Drawing.Point(485, 94);
            this.label6.Margin = new System.Windows.Forms.Padding(0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(15, 16);
            this.label6.TabIndex = 104;
            this.label6.Text = "*";
            // 
            // cmb_ItemType
            // 
            this.cmb_ItemType.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cmb_ItemType.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmb_ItemType.Font = new System.Drawing.Font("Tahoma", 9.75F);
            this.cmb_ItemType.FormattingEnabled = true;
            this.cmb_ItemType.Location = new System.Drawing.Point(303, 124);
            this.cmb_ItemType.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cmb_ItemType.Name = "cmb_ItemType";
            this.cmb_ItemType.Size = new System.Drawing.Size(182, 24);
            this.cmb_ItemType.TabIndex = 102;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Tahoma", 9.75F);
            this.label2.ForeColor = System.Drawing.Color.Firebrick;
            this.label2.Location = new System.Drawing.Point(486, 128);
            this.label2.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(15, 16);
            this.label2.TabIndex = 103;
            this.label2.Text = "*";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Tahoma", 9.75F);
            this.label4.Location = new System.Drawing.Point(496, 128);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(72, 16);
            this.label4.TabIndex = 101;
            this.label4.Text = "نوع الصنف :";
            // 
            // cmb_GroupSells
            // 
            this.cmb_GroupSells.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cmb_GroupSells.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmb_GroupSells.Font = new System.Drawing.Font("Tahoma", 9.75F);
            this.cmb_GroupSells.FormattingEnabled = true;
            this.cmb_GroupSells.Location = new System.Drawing.Point(9, 89);
            this.cmb_GroupSells.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cmb_GroupSells.Name = "cmb_GroupSells";
            this.cmb_GroupSells.Size = new System.Drawing.Size(182, 24);
            this.cmb_GroupSells.TabIndex = 100;
            // 
            // cmb_GroupBasics
            // 
            this.cmb_GroupBasics.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cmb_GroupBasics.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmb_GroupBasics.Font = new System.Drawing.Font("Tahoma", 9.75F);
            this.cmb_GroupBasics.FormattingEnabled = true;
            this.cmb_GroupBasics.Location = new System.Drawing.Point(303, 90);
            this.cmb_GroupBasics.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cmb_GroupBasics.Name = "cmb_GroupBasics";
            this.cmb_GroupBasics.Size = new System.Drawing.Size(182, 24);
            this.cmb_GroupBasics.TabIndex = 98;
            // 
            // panel1
            // 
            this.panel1.AutoSize = true;
            this.panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel1.Controls.Add(this.gbPrices);
            this.panel1.Controls.Add(this.groupBox4);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(6);
            this.panel1.Size = new System.Drawing.Size(643, 491);
            this.panel1.TabIndex = 1;
            // 
            // gbPrices
            // 
            this.gbPrices.Controls.Add(this.dgvPrices);
            this.gbPrices.Controls.Add(this.label10);
            this.gbPrices.Controls.Add(this.btnCancelPolcy);
            this.gbPrices.Controls.Add(this.btnAddPolicy);
            this.gbPrices.Controls.Add(this.cmbPricePolicies);
            this.gbPrices.Controls.Add(this.txtPolicyPrice);
            this.gbPrices.Controls.Add(this.btnSavePolicyChanges);
            this.gbPrices.Controls.Add(this.label13);
            this.gbPrices.Controls.Add(this.label16);
            this.gbPrices.Controls.Add(this.label14);
            this.gbPrices.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbPrices.Location = new System.Drawing.Point(6, 267);
            this.gbPrices.Name = "gbPrices";
            this.gbPrices.Size = new System.Drawing.Size(631, 218);
            this.gbPrices.TabIndex = 213;
            this.gbPrices.TabStop = false;
            this.gbPrices.Text = "سياسات التسعير";
            // 
            // dgvPrices
            // 
            this.dgvPrices.AllowUserToAddRows = false;
            this.dgvPrices.AllowUserToDeleteRows = false;
            this.dgvPrices.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPrices.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colID,
            this.policyID,
            this.colName,
            this.colPrice,
            this.colEdit,
            this.colDelete});
            this.dgvPrices.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.dgvPrices.Location = new System.Drawing.Point(3, 77);
            this.dgvPrices.Margin = new System.Windows.Forms.Padding(5);
            this.dgvPrices.Name = "dgvPrices";
            this.dgvPrices.ReadOnly = true;
            this.dgvPrices.Size = new System.Drawing.Size(625, 138);
            this.dgvPrices.TabIndex = 0;
            this.dgvPrices.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvPrices_CellContentClick);
            // 
            // colID
            // 
            this.colID.DataPropertyName = "Id";
            this.colID.HeaderText = "id";
            this.colID.Name = "colID";
            this.colID.ReadOnly = true;
            this.colID.Visible = false;
            // 
            // policyID
            // 
            this.policyID.DataPropertyName = "PolicyID";
            this.policyID.HeaderText = "policyID";
            this.policyID.Name = "policyID";
            this.policyID.ReadOnly = true;
            this.policyID.Visible = false;
            // 
            // colName
            // 
            this.colName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colName.DataPropertyName = "Name";
            this.colName.HeaderText = "سياسة التسعير";
            this.colName.Name = "colName";
            this.colName.ReadOnly = true;
            // 
            // colPrice
            // 
            this.colPrice.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.colPrice.DataPropertyName = "Price";
            this.colPrice.HeaderText = "السعر";
            this.colPrice.Name = "colPrice";
            this.colPrice.ReadOnly = true;
            this.colPrice.Width = 64;
            // 
            // colEdit
            // 
            this.colEdit.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.colEdit.HeaderText = "تعديل";
            this.colEdit.Image = global::ERP.Desktop.Properties.Resources.edit;
            this.colEdit.Name = "colEdit";
            this.colEdit.ReadOnly = true;
            this.colEdit.Width = 43;
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
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Tahoma", 9.75F);
            this.label10.Location = new System.Drawing.Point(496, 20);
            this.label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(104, 16);
            this.label10.TabIndex = 213;
            this.label10.Text = "سياسة التسعير :";
            // 
            // btnCancelPolcy
            // 
            this.btnCancelPolcy.BackColor = System.Drawing.Color.Crimson;
            this.btnCancelPolcy.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancelPolcy.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancelPolcy.ForeColor = System.Drawing.Color.White;
            this.btnCancelPolcy.IconChar = FontAwesome.Sharp.IconChar.Times;
            this.btnCancelPolcy.IconColor = System.Drawing.Color.White;
            this.btnCancelPolcy.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnCancelPolcy.IconSize = 22;
            this.btnCancelPolcy.Location = new System.Drawing.Point(9, 45);
            this.btnCancelPolcy.Name = "btnCancelPolcy";
            this.btnCancelPolcy.Size = new System.Drawing.Size(80, 30);
            this.btnCancelPolcy.TabIndex = 212;
            this.btnCancelPolcy.Text = "رجوع";
            this.btnCancelPolcy.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnCancelPolcy.UseVisualStyleBackColor = false;
            this.btnCancelPolcy.Visible = false;
            this.btnCancelPolcy.Click += new System.EventHandler(this.btnCancelPolcy_Click);
            // 
            // btnAddPolicy
            // 
            this.btnAddPolicy.BackColor = System.Drawing.Color.DeepSkyBlue;
            this.btnAddPolicy.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddPolicy.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAddPolicy.ForeColor = System.Drawing.Color.White;
            this.btnAddPolicy.IconChar = FontAwesome.Sharp.IconChar.Plus;
            this.btnAddPolicy.IconColor = System.Drawing.Color.White;
            this.btnAddPolicy.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnAddPolicy.IconSize = 22;
            this.btnAddPolicy.Location = new System.Drawing.Point(9, 45);
            this.btnAddPolicy.Name = "btnAddPolicy";
            this.btnAddPolicy.Size = new System.Drawing.Size(80, 30);
            this.btnAddPolicy.TabIndex = 210;
            this.btnAddPolicy.Text = "إضافة";
            this.btnAddPolicy.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnAddPolicy.UseVisualStyleBackColor = false;
            this.btnAddPolicy.Click += new System.EventHandler(this.btnAddPolicy_Click);
            // 
            // cmbPricePolicies
            // 
            this.cmbPricePolicies.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cmbPricePolicies.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbPricePolicies.Font = new System.Drawing.Font("Tahoma", 9.75F);
            this.cmbPricePolicies.FormattingEnabled = true;
            this.cmbPricePolicies.Location = new System.Drawing.Point(303, 16);
            this.cmbPricePolicies.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cmbPricePolicies.Name = "cmbPricePolicies";
            this.cmbPricePolicies.Size = new System.Drawing.Size(182, 24);
            this.cmbPricePolicies.TabIndex = 98;
            // 
            // txtPolicyPrice
            // 
            this.txtPolicyPrice.Font = new System.Drawing.Font("Tahoma", 9.75F);
            this.txtPolicyPrice.Location = new System.Drawing.Point(9, 17);
            this.txtPolicyPrice.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtPolicyPrice.Name = "txtPolicyPrice";
            this.txtPolicyPrice.Num = 0D;
            this.txtPolicyPrice.RoundDigits = 3;
            this.txtPolicyPrice.Size = new System.Drawing.Size(182, 23);
            this.txtPolicyPrice.TabIndex = 10;
            this.txtPolicyPrice.Text = "0.0";
            this.txtPolicyPrice.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtPolicyPrice.VisibleDigits = false;
            this.txtPolicyPrice.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txt_MinPrice_KeyPress);
            // 
            // btnSavePolicyChanges
            // 
            this.btnSavePolicyChanges.BackColor = System.Drawing.Color.DarkTurquoise;
            this.btnSavePolicyChanges.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSavePolicyChanges.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSavePolicyChanges.ForeColor = System.Drawing.Color.White;
            this.btnSavePolicyChanges.IconChar = FontAwesome.Sharp.IconChar.Save;
            this.btnSavePolicyChanges.IconColor = System.Drawing.Color.White;
            this.btnSavePolicyChanges.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnSavePolicyChanges.IconSize = 22;
            this.btnSavePolicyChanges.Location = new System.Drawing.Point(95, 45);
            this.btnSavePolicyChanges.Name = "btnSavePolicyChanges";
            this.btnSavePolicyChanges.Size = new System.Drawing.Size(80, 30);
            this.btnSavePolicyChanges.TabIndex = 211;
            this.btnSavePolicyChanges.Text = "حفظ";
            this.btnSavePolicyChanges.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnSavePolicyChanges.UseVisualStyleBackColor = false;
            this.btnSavePolicyChanges.Visible = false;
            this.btnSavePolicyChanges.Click += new System.EventHandler(this.btnSavePolicyChanges_Click);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Tahoma", 9.75F);
            this.label13.Location = new System.Drawing.Point(202, 20);
            this.label13.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(61, 16);
            this.label13.TabIndex = 106;
            this.label13.Text = "اقل سعر :";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("Tahoma", 9.75F);
            this.label16.ForeColor = System.Drawing.Color.Firebrick;
            this.label16.Location = new System.Drawing.Point(486, 20);
            this.label16.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(15, 16);
            this.label16.TabIndex = 95;
            this.label16.Text = "*";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Tahoma", 9.75F);
            this.label14.ForeColor = System.Drawing.Color.Firebrick;
            this.label14.Location = new System.Drawing.Point(191, 20);
            this.label14.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(15, 16);
            this.label14.TabIndex = 95;
            this.label14.Text = "*";
            // 
            // ItemAddEdit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(643, 491);
            this.Controls.Add(this.panel1);
            this.MinimizeBox = false;
            this.Name = "ItemAddEdit";
            this.ShowIcon = false;
            this.Text = "ادارة الاصناف";
            this.Load += new System.EventHandler(this.ItemAddEdit_Load);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.gbPrices.ResumeLayout(false);
            this.gbPrices.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPrices)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        public System.Windows.Forms.CheckBox ckIsActive;
        public System.Windows.Forms.TextBox txt_MinPrice;
        private System.Windows.Forms.Label label26;
        public System.Windows.Forms.TextBox txt_SellPrice;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label11;
        public System.Windows.Forms.TextBox txt_Name;
        public System.Windows.Forms.TextBox txt_ItemCode;
        private System.Windows.Forms.Label label35;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label12;
        public System.Windows.Forms.ComboBox cmb_Units;
        private System.Windows.Forms.Label label18;
        public System.Windows.Forms.TextBox txt_Barcode;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Panel panel1;
        public System.Windows.Forms.ComboBox cmb_GroupSells;
        public System.Windows.Forms.ComboBox cmb_GroupBasics;
        public System.Windows.Forms.TextBox txt_MaxPrice;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        public System.Windows.Forms.ComboBox cmb_ItemType;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        public System.Windows.Forms.TextBox txt_TechnicalSpecifications;
        private System.Windows.Forms.Label label5;
        private ERP.Desktop.Utilities.ReusableControls.DangerButton btnCancelChanges;
        private ERP.Desktop.Utilities.ReusableControls.SuccessButton btnSaveChanges;
        private ERP.Desktop.Utilities.ReusableControls.DefaultButton btn_Add;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.GroupBox gbPrices;
        private System.Windows.Forms.DataGridView dgvPrices;
        private System.Windows.Forms.Label label10;
        private Utilities.ReusableControls.DangerButton btnCancelPolcy;
        private Utilities.ReusableControls.DefaultButton btnAddPolicy;
        public System.Windows.Forms.ComboBox cmbPricePolicies;
        private Utilities.ReusableControls.SuccessButton btnSavePolicyChanges;
        public Utilities.ReusableControls.TextBoxNum txtPolicyPrice;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.DataGridViewTextBoxColumn colID;
        private System.Windows.Forms.DataGridViewTextBoxColumn policyID;
        private System.Windows.Forms.DataGridViewTextBoxColumn colName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPrice;
        private System.Windows.Forms.DataGridViewImageColumn colEdit;
        private System.Windows.Forms.DataGridViewImageColumn colDelete;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label14;
    }
}