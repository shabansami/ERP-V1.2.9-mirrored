namespace ERP.Desktop.Views.Transactions.Sales
{
    partial class ItemProductionFrm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ItemProductionFrm));
            this.rdo_newCustomer = new System.Windows.Forms.RadioButton();
            this.rdo_existCustomer = new System.Windows.Forms.RadioButton();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.group_searchCustomer = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.txt_searchCustNam = new System.Windows.Forms.TextBox();
            this.dgv_search = new System.Windows.Forms.DataGridView();
            this.Person_ID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Person_Name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Mobil1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Mobil2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Cus_Address = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.addNew = new System.Windows.Forms.DataGridViewImageColumn();
            this.group_itemProductionCust = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.label3 = new System.Windows.Forms.Label();
            this.dgv_itemProductions = new System.Windows.Forms.DataGridView();
            this.ItemProductionId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CustomerName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ItemProductionName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CreatedOn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.show = new System.Windows.Forms.DataGridViewImageColumn();
            this.selectItem = new System.Windows.Forms.DataGridViewImageColumn();
            this.group_Cancel = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel6 = new System.Windows.Forms.TableLayoutPanel();
            this.dangerButton1 = new ERP.Desktop.Utilities.ReusableControls.DangerButton();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.btn_Cancel = new ERP.Desktop.Utilities.ReusableControls.DangerButton();
            this.btn_UpdateSaleInvoive = new ERP.Desktop.Utilities.ReusableControls.SuccessButton();
            this.txt_itemProduName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtItemCode = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.dgv_items = new System.Windows.Forms.DataGridView();
            this.ItemId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ItemBarcode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ItemName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Price = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Quantity = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Amount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.del = new System.Windows.Forms.DataGridViewImageColumn();
            this.btnAddItem = new ERP.Desktop.Utilities.ReusableControls.DefaultButton();
            this.txt_quantity = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.txt_itemName = new System.Windows.Forms.TextBox();
            this.txtPrice = new System.Windows.Forms.TextBox();
            this.group_addItems = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.label5 = new System.Windows.Forms.Label();
            this.txtAddress = new System.Windows.Forms.TextBox();
            this.txtName = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.txtMob1 = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.group_addNewCustomer = new System.Windows.Forms.GroupBox();
            this.groupBox3.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.group_searchCustomer.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_search)).BeginInit();
            this.group_itemProductionCust.SuspendLayout();
            this.tableLayoutPanel5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_itemProductions)).BeginInit();
            this.group_Cancel.SuspendLayout();
            this.tableLayoutPanel6.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_items)).BeginInit();
            this.group_addItems.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.group_addNewCustomer.SuspendLayout();
            this.SuspendLayout();
            // 
            // rdo_newCustomer
            // 
            this.rdo_newCustomer.AutoSize = true;
            this.rdo_newCustomer.BackColor = System.Drawing.Color.MediumAquamarine;
            this.rdo_newCustomer.CheckAlign = System.Drawing.ContentAlignment.TopCenter;
            this.rdo_newCustomer.Checked = true;
            this.rdo_newCustomer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rdo_newCustomer.Location = new System.Drawing.Point(504, 3);
            this.rdo_newCustomer.Name = "rdo_newCustomer";
            this.rdo_newCustomer.Size = new System.Drawing.Size(495, 40);
            this.rdo_newCustomer.TabIndex = 209;
            this.rdo_newCustomer.TabStop = true;
            this.rdo_newCustomer.Text = "عميل جديد";
            this.rdo_newCustomer.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.rdo_newCustomer.UseVisualStyleBackColor = false;
            this.rdo_newCustomer.CheckedChanged += new System.EventHandler(this.rdo_newCustomer_CheckedChanged);
            // 
            // rdo_existCustomer
            // 
            this.rdo_existCustomer.AutoSize = true;
            this.rdo_existCustomer.BackColor = System.Drawing.Color.LightCoral;
            this.rdo_existCustomer.CheckAlign = System.Drawing.ContentAlignment.TopCenter;
            this.rdo_existCustomer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rdo_existCustomer.Location = new System.Drawing.Point(3, 3);
            this.rdo_existCustomer.Name = "rdo_existCustomer";
            this.rdo_existCustomer.Size = new System.Drawing.Size(495, 40);
            this.rdo_existCustomer.TabIndex = 209;
            this.rdo_existCustomer.TabStop = true;
            this.rdo_existCustomer.Text = "عميل موجود مسبقا";
            this.rdo_existCustomer.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rdo_existCustomer.UseVisualStyleBackColor = false;
            this.rdo_existCustomer.CheckedChanged += new System.EventHandler(this.rdo_existCustomer_CheckedChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.tableLayoutPanel1);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox3.Location = new System.Drawing.Point(0, 0);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(1008, 65);
            this.groupBox3.TabIndex = 204;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "حالة العميل";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.rdo_newCustomer, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.rdo_existCustomer, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 19);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 46F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1002, 43);
            this.tableLayoutPanel1.TabIndex = 210;
            // 
            // group_searchCustomer
            // 
            this.group_searchCustomer.Controls.Add(this.tableLayoutPanel4);
            this.group_searchCustomer.Dock = System.Windows.Forms.DockStyle.Top;
            this.group_searchCustomer.Location = new System.Drawing.Point(0, 422);
            this.group_searchCustomer.Name = "group_searchCustomer";
            this.group_searchCustomer.Size = new System.Drawing.Size(1008, 240);
            this.group_searchCustomer.TabIndex = 212;
            this.group_searchCustomer.TabStop = false;
            this.group_searchCustomer.Text = "البحث عن عميل";
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 1;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.txt_searchCustNam, 0, 1);
            this.tableLayoutPanel4.Controls.Add(this.dgv_search, 0, 2);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(3, 19);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 3;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 23F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(1002, 218);
            this.tableLayoutPanel4.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Font = new System.Drawing.Font("Tahoma", 9.75F);
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(996, 23);
            this.label1.TabIndex = 131;
            this.label1.Text = "اسم العميل/رقم التليفون";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txt_searchCustNam
            // 
            this.txt_searchCustNam.BackColor = System.Drawing.SystemColors.Window;
            this.txt_searchCustNam.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txt_searchCustNam.Font = new System.Drawing.Font("Tahoma", 9.75F);
            this.txt_searchCustNam.Location = new System.Drawing.Point(3, 27);
            this.txt_searchCustNam.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txt_searchCustNam.Name = "txt_searchCustNam";
            this.txt_searchCustNam.Size = new System.Drawing.Size(996, 23);
            this.txt_searchCustNam.TabIndex = 132;
            this.txt_searchCustNam.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txt_searchCustNam.TextChanged += new System.EventHandler(this.txt_searchCustNam_TextChanged);
            // 
            // dgv_search
            // 
            this.dgv_search.AllowUserToAddRows = false;
            this.dgv_search.AllowUserToDeleteRows = false;
            this.dgv_search.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgv_search.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_search.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Person_ID,
            this.Person_Name,
            this.Mobil1,
            this.Mobil2,
            this.Cus_Address,
            this.addNew});
            this.dgv_search.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgv_search.Location = new System.Drawing.Point(3, 55);
            this.dgv_search.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.dgv_search.Name = "dgv_search";
            this.dgv_search.ReadOnly = true;
            this.dgv_search.Size = new System.Drawing.Size(996, 161);
            this.dgv_search.TabIndex = 133;
            this.dgv_search.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgv_search_CellClick);
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
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.Cus_Address.DefaultCellStyle = dataGridViewCellStyle1;
            this.Cus_Address.HeaderText = "العنوان";
            this.Cus_Address.Name = "Cus_Address";
            this.Cus_Address.ReadOnly = true;
            // 
            // addNew
            // 
            this.addNew.HeaderText = "انشاء توليفة جديدة";
            this.addNew.Image = global::ERP.Desktop.Properties.Resources.add;
            this.addNew.Name = "addNew";
            this.addNew.ReadOnly = true;
            this.addNew.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.addNew.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // group_itemProductionCust
            // 
            this.group_itemProductionCust.Controls.Add(this.tableLayoutPanel5);
            this.group_itemProductionCust.Dock = System.Windows.Forms.DockStyle.Top;
            this.group_itemProductionCust.Location = new System.Drawing.Point(0, 662);
            this.group_itemProductionCust.Name = "group_itemProductionCust";
            this.group_itemProductionCust.Size = new System.Drawing.Size(1008, 166);
            this.group_itemProductionCust.TabIndex = 213;
            this.group_itemProductionCust.TabStop = false;
            this.group_itemProductionCust.Text = "توليفات العميل";
            // 
            // tableLayoutPanel5
            // 
            this.tableLayoutPanel5.ColumnCount = 2;
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 70.19134F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 29.80866F));
            this.tableLayoutPanel5.Controls.Add(this.label3, 0, 0);
            this.tableLayoutPanel5.Controls.Add(this.dgv_itemProductions, 0, 1);
            this.tableLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel5.Location = new System.Drawing.Point(3, 19);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            this.tableLayoutPanel5.RowCount = 2;
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 13.88889F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 86.11111F));
            this.tableLayoutPanel5.Size = new System.Drawing.Size(1002, 144);
            this.tableLayoutPanel5.TabIndex = 0;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.tableLayoutPanel5.SetColumnSpan(this.label3, 2);
            this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label3.Location = new System.Drawing.Point(3, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(996, 20);
            this.label3.TabIndex = 0;
            this.label3.Text = "التوليفات المسجلة مسبقا للعميل ";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // dgv_itemProductions
            // 
            this.dgv_itemProductions.AllowUserToAddRows = false;
            this.dgv_itemProductions.AllowUserToDeleteRows = false;
            this.dgv_itemProductions.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgv_itemProductions.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_itemProductions.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ItemProductionId,
            this.CustomerName,
            this.ItemProductionName,
            this.CreatedOn,
            this.show,
            this.selectItem});
            this.tableLayoutPanel5.SetColumnSpan(this.dgv_itemProductions, 2);
            this.dgv_itemProductions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgv_itemProductions.Location = new System.Drawing.Point(3, 23);
            this.dgv_itemProductions.Name = "dgv_itemProductions";
            this.dgv_itemProductions.ReadOnly = true;
            this.dgv_itemProductions.Size = new System.Drawing.Size(996, 118);
            this.dgv_itemProductions.TabIndex = 2;
            this.dgv_itemProductions.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgv_itemProductions_CellClick);
            // 
            // ItemProductionId
            // 
            this.ItemProductionId.DataPropertyName = "ItemProductionId";
            this.ItemProductionId.HeaderText = "ItemProductionId";
            this.ItemProductionId.Name = "ItemProductionId";
            this.ItemProductionId.ReadOnly = true;
            this.ItemProductionId.Visible = false;
            // 
            // CustomerName
            // 
            this.CustomerName.DataPropertyName = "CustomerName";
            this.CustomerName.HeaderText = "اسم العميل";
            this.CustomerName.Name = "CustomerName";
            this.CustomerName.ReadOnly = true;
            // 
            // ItemProductionName
            // 
            this.ItemProductionName.DataPropertyName = "ItemProductionName";
            this.ItemProductionName.HeaderText = "مسمى التوليفة";
            this.ItemProductionName.Name = "ItemProductionName";
            this.ItemProductionName.ReadOnly = true;
            // 
            // CreatedOn
            // 
            this.CreatedOn.DataPropertyName = "CreatedOn";
            this.CreatedOn.HeaderText = "تاريخ الانشاء";
            this.CreatedOn.Name = "CreatedOn";
            this.CreatedOn.ReadOnly = true;
            // 
            // show
            // 
            this.show.HeaderText = "عرض الاصناف";
            this.show.Image = global::ERP.Desktop.Properties.Resources.eye;
            this.show.Name = "show";
            this.show.ReadOnly = true;
            // 
            // selectItem
            // 
            this.selectItem.HeaderText = "اختيار التوليفة";
            this.selectItem.Image = global::ERP.Desktop.Properties.Resources.checked_checkbox_24px;
            this.selectItem.Name = "selectItem";
            this.selectItem.ReadOnly = true;
            // 
            // group_Cancel
            // 
            this.group_Cancel.Controls.Add(this.tableLayoutPanel6);
            this.group_Cancel.Dock = System.Windows.Forms.DockStyle.Top;
            this.group_Cancel.Location = new System.Drawing.Point(0, 828);
            this.group_Cancel.Name = "group_Cancel";
            this.group_Cancel.Size = new System.Drawing.Size(1008, 76);
            this.group_Cancel.TabIndex = 214;
            this.group_Cancel.TabStop = false;
            // 
            // tableLayoutPanel6
            // 
            this.tableLayoutPanel6.ColumnCount = 3;
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 41.59021F));
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 58.40979F));
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            this.tableLayoutPanel6.Controls.Add(this.dangerButton1, 1, 0);
            this.tableLayoutPanel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel6.Location = new System.Drawing.Point(3, 19);
            this.tableLayoutPanel6.Name = "tableLayoutPanel6";
            this.tableLayoutPanel6.RowCount = 1;
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 57F));
            this.tableLayoutPanel6.Size = new System.Drawing.Size(1002, 54);
            this.tableLayoutPanel6.TabIndex = 0;
            // 
            // dangerButton1
            // 
            this.dangerButton1.BackColor = System.Drawing.Color.Crimson;
            this.dangerButton1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.dangerButton1.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dangerButton1.ForeColor = System.Drawing.Color.White;
            this.dangerButton1.IconChar = FontAwesome.Sharp.IconChar.Times;
            this.dangerButton1.IconColor = System.Drawing.Color.White;
            this.dangerButton1.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.dangerButton1.IconSize = 22;
            this.dangerButton1.Location = new System.Drawing.Point(390, 3);
            this.dangerButton1.Name = "dangerButton1";
            this.dangerButton1.Size = new System.Drawing.Size(208, 43);
            this.dangerButton1.TabIndex = 210;
            this.dangerButton1.Text = "الغاء ورجوع";
            this.dangerButton1.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.dangerButton1.UseVisualStyleBackColor = false;
            this.dangerButton1.Click += new System.EventHandler(this.dangerButton1_Click);
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 6;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 55.90829F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 44.09171F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 240F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 124F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 86F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 89F));
            this.tableLayoutPanel3.Controls.Add(this.btn_Cancel, 3, 3);
            this.tableLayoutPanel3.Controls.Add(this.btn_UpdateSaleInvoive, 0, 3);
            this.tableLayoutPanel3.Controls.Add(this.txt_itemProduName, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.label2, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.txtItemCode, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.label14, 2, 0);
            this.tableLayoutPanel3.Controls.Add(this.label15, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.dgv_items, 0, 2);
            this.tableLayoutPanel3.Controls.Add(this.btnAddItem, 5, 1);
            this.tableLayoutPanel3.Controls.Add(this.txt_quantity, 4, 1);
            this.tableLayoutPanel3.Controls.Add(this.label16, 4, 0);
            this.tableLayoutPanel3.Controls.Add(this.label7, 3, 0);
            this.tableLayoutPanel3.Controls.Add(this.txt_itemName, 2, 1);
            this.tableLayoutPanel3.Controls.Add(this.txtPrice, 3, 1);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 19);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 4;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 158F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 49F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(1002, 256);
            this.tableLayoutPanel3.TabIndex = 137;
            // 
            // btn_Cancel
            // 
            this.btn_Cancel.BackColor = System.Drawing.Color.Crimson;
            this.tableLayoutPanel3.SetColumnSpan(this.btn_Cancel, 3);
            this.btn_Cancel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_Cancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Cancel.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_Cancel.ForeColor = System.Drawing.Color.White;
            this.btn_Cancel.IconChar = FontAwesome.Sharp.IconChar.Times;
            this.btn_Cancel.IconColor = System.Drawing.Color.White;
            this.btn_Cancel.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btn_Cancel.IconSize = 22;
            this.btn_Cancel.Location = new System.Drawing.Point(3, 210);
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.Size = new System.Drawing.Size(294, 43);
            this.btn_Cancel.TabIndex = 209;
            this.btn_Cancel.Text = "الغاء ورجوع";
            this.btn_Cancel.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btn_Cancel.UseVisualStyleBackColor = false;
            this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
            // 
            // btn_UpdateSaleInvoive
            // 
            this.btn_UpdateSaleInvoive.BackColor = System.Drawing.Color.DarkTurquoise;
            this.tableLayoutPanel3.SetColumnSpan(this.btn_UpdateSaleInvoive, 3);
            this.btn_UpdateSaleInvoive.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_UpdateSaleInvoive.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_UpdateSaleInvoive.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_UpdateSaleInvoive.ForeColor = System.Drawing.Color.White;
            this.btn_UpdateSaleInvoive.IconChar = FontAwesome.Sharp.IconChar.Save;
            this.btn_UpdateSaleInvoive.IconColor = System.Drawing.Color.White;
            this.btn_UpdateSaleInvoive.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btn_UpdateSaleInvoive.IconSize = 22;
            this.btn_UpdateSaleInvoive.Location = new System.Drawing.Point(303, 210);
            this.btn_UpdateSaleInvoive.Name = "btn_UpdateSaleInvoive";
            this.btn_UpdateSaleInvoive.Size = new System.Drawing.Size(696, 43);
            this.btn_UpdateSaleInvoive.TabIndex = 208;
            this.btn_UpdateSaleInvoive.Text = "اضافة التوليفة الى فاتورة البيع";
            this.btn_UpdateSaleInvoive.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btn_UpdateSaleInvoive.UseVisualStyleBackColor = false;
            this.btn_UpdateSaleInvoive.Click += new System.EventHandler(this.btn_UpdateSaleInvoive_Click);
            // 
            // txt_itemProduName
            // 
            this.txt_itemProduName.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.txt_itemProduName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txt_itemProduName.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_itemProduName.Location = new System.Drawing.Point(747, 25);
            this.txt_itemProduName.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txt_itemProduName.Name = "txt_itemProduName";
            this.txt_itemProduName.Size = new System.Drawing.Size(252, 23);
            this.txt_itemProduName.TabIndex = 134;
            this.txt_itemProduName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txt_itemProduName_KeyDown);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Font = new System.Drawing.Font("Tahoma", 9.75F);
            this.label2.Location = new System.Drawing.Point(747, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(252, 21);
            this.label2.TabIndex = 129;
            this.label2.Text = "مسمى التوليفة";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txtItemCode
            // 
            this.txtItemCode.BackColor = System.Drawing.SystemColors.Window;
            this.txtItemCode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtItemCode.Font = new System.Drawing.Font("Tahoma", 9.75F);
            this.txtItemCode.Location = new System.Drawing.Point(543, 25);
            this.txtItemCode.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtItemCode.Name = "txtItemCode";
            this.txtItemCode.Size = new System.Drawing.Size(198, 23);
            this.txtItemCode.TabIndex = 127;
            this.txtItemCode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txt_barcode_KeyDown);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label14.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.Location = new System.Drawing.Point(303, 0);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(234, 21);
            this.label14.TabIndex = 133;
            this.label14.Text = "الصنف";
            this.label14.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label15.Font = new System.Drawing.Font("Tahoma", 9.75F);
            this.label15.Location = new System.Drawing.Point(543, 0);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(198, 21);
            this.label15.TabIndex = 130;
            this.label15.Text = "باركود/كود الصنف";
            this.label15.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // dgv_items
            // 
            this.dgv_items.AllowUserToAddRows = false;
            this.dgv_items.AllowUserToDeleteRows = false;
            this.dgv_items.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgv_items.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_items.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ItemId,
            this.ItemBarcode,
            this.ItemName,
            this.Price,
            this.Quantity,
            this.Amount,
            this.del});
            this.tableLayoutPanel3.SetColumnSpan(this.dgv_items, 6);
            this.dgv_items.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgv_items.Location = new System.Drawing.Point(3, 52);
            this.dgv_items.Name = "dgv_items";
            this.dgv_items.ReadOnly = true;
            this.dgv_items.Size = new System.Drawing.Size(996, 152);
            this.dgv_items.TabIndex = 213;
            this.dgv_items.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgv_items_CellClick);
            // 
            // ItemId
            // 
            this.ItemId.DataPropertyName = "ItemId";
            this.ItemId.HeaderText = "ItemId";
            this.ItemId.Name = "ItemId";
            this.ItemId.ReadOnly = true;
            this.ItemId.Visible = false;
            // 
            // ItemBarcode
            // 
            this.ItemBarcode.DataPropertyName = "ItemBarcode";
            this.ItemBarcode.HeaderText = "باركود/كود";
            this.ItemBarcode.Name = "ItemBarcode";
            this.ItemBarcode.ReadOnly = true;
            // 
            // ItemName
            // 
            this.ItemName.DataPropertyName = "ItemName";
            this.ItemName.HeaderText = "الصنف";
            this.ItemName.Name = "ItemName";
            this.ItemName.ReadOnly = true;
            // 
            // Price
            // 
            this.Price.DataPropertyName = "Price";
            this.Price.HeaderText = "السعر";
            this.Price.Name = "Price";
            this.Price.ReadOnly = true;
            // 
            // Quantity
            // 
            this.Quantity.DataPropertyName = "Quantity";
            this.Quantity.HeaderText = "الكمية";
            this.Quantity.Name = "Quantity";
            this.Quantity.ReadOnly = true;
            // 
            // Amount
            // 
            this.Amount.DataPropertyName = "Amount";
            this.Amount.HeaderText = "القيمة";
            this.Amount.Name = "Amount";
            this.Amount.ReadOnly = true;
            // 
            // del
            // 
            this.del.HeaderText = "حذف";
            this.del.Image = global::ERP.Desktop.Properties.Resources.trash;
            this.del.Name = "del";
            this.del.ReadOnly = true;
            this.del.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.del.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // btnAddItem
            // 
            this.btnAddItem.BackColor = System.Drawing.Color.DeepSkyBlue;
            this.btnAddItem.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnAddItem.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddItem.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.btnAddItem.ForeColor = System.Drawing.Color.White;
            this.btnAddItem.IconChar = FontAwesome.Sharp.IconChar.Plus;
            this.btnAddItem.IconColor = System.Drawing.Color.White;
            this.btnAddItem.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnAddItem.IconSize = 22;
            this.btnAddItem.Location = new System.Drawing.Point(3, 21);
            this.btnAddItem.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.btnAddItem.Name = "btnAddItem";
            this.btnAddItem.Size = new System.Drawing.Size(84, 28);
            this.btnAddItem.TabIndex = 212;
            this.btnAddItem.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnAddItem.UseVisualStyleBackColor = false;
            this.btnAddItem.Click += new System.EventHandler(this.btnAddItem_Click);
            // 
            // txt_quantity
            // 
            this.txt_quantity.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txt_quantity.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_quantity.Location = new System.Drawing.Point(93, 25);
            this.txt_quantity.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txt_quantity.Name = "txt_quantity";
            this.txt_quantity.Size = new System.Drawing.Size(80, 23);
            this.txt_quantity.TabIndex = 132;
            this.txt_quantity.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txt_quantity_KeyDown);
            this.txt_quantity.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txt_quantity_KeyPress);
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label16.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label16.Location = new System.Drawing.Point(93, 0);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(80, 21);
            this.label16.TabIndex = 133;
            this.label16.Text = "الكمية";
            this.label16.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label7.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(179, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(118, 21);
            this.label7.TabIndex = 133;
            this.label7.Text = "السعر";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txt_itemName
            // 
            this.txt_itemName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txt_itemName.Font = new System.Drawing.Font("Tahoma", 9.75F);
            this.txt_itemName.Location = new System.Drawing.Point(303, 25);
            this.txt_itemName.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txt_itemName.Name = "txt_itemName";
            this.txt_itemName.Size = new System.Drawing.Size(234, 23);
            this.txt_itemName.TabIndex = 128;
            this.txt_itemName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txt_itemName_KeyDown);
            // 
            // txtPrice
            // 
            this.txtPrice.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtPrice.Font = new System.Drawing.Font("Tahoma", 9.75F);
            this.txtPrice.Location = new System.Drawing.Point(179, 25);
            this.txtPrice.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtPrice.Name = "txtPrice";
            this.txtPrice.ReadOnly = true;
            this.txtPrice.Size = new System.Drawing.Size(118, 23);
            this.txtPrice.TabIndex = 214;
            // 
            // group_addItems
            // 
            this.group_addItems.Controls.Add(this.tableLayoutPanel3);
            this.group_addItems.Dock = System.Windows.Forms.DockStyle.Top;
            this.group_addItems.Location = new System.Drawing.Point(0, 144);
            this.group_addItems.Name = "group_addItems";
            this.group_addItems.Size = new System.Drawing.Size(1008, 278);
            this.group_addItems.TabIndex = 211;
            this.group_addItems.TabStop = false;
            this.group_addItems.Text = "اضافة صنف";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 3;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 462F));
            this.tableLayoutPanel2.Controls.Add(this.label5, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.txtAddress, 2, 1);
            this.tableLayoutPanel2.Controls.Add(this.txtName, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.label10, 2, 0);
            this.tableLayoutPanel2.Controls.Add(this.txtMob1, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.label4, 1, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 19);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(1002, 57);
            this.tableLayoutPanel2.TabIndex = 136;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label5.Font = new System.Drawing.Font("Tahoma", 9.75F);
            this.label5.Location = new System.Drawing.Point(735, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(264, 20);
            this.label5.TabIndex = 129;
            this.label5.Text = "اسم العميل ";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txtAddress
            // 
            this.txtAddress.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtAddress.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAddress.Location = new System.Drawing.Point(3, 24);
            this.txtAddress.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtAddress.Name = "txtAddress";
            this.txtAddress.Size = new System.Drawing.Size(456, 23);
            this.txtAddress.TabIndex = 132;
            this.txtAddress.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtAddress_KeyDown);
            // 
            // txtName
            // 
            this.txtName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtName.Font = new System.Drawing.Font("Tahoma", 9.75F);
            this.txtName.Location = new System.Drawing.Point(735, 24);
            this.txtName.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(264, 23);
            this.txtName.TabIndex = 127;
            this.txtName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtName_KeyDown);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label10.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(3, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(456, 20);
            this.label10.TabIndex = 133;
            this.label10.Text = "العنوان ";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txtMob1
            // 
            this.txtMob1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtMob1.Font = new System.Drawing.Font("Tahoma", 9.75F);
            this.txtMob1.Location = new System.Drawing.Point(465, 24);
            this.txtMob1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtMob1.MaxLength = 20;
            this.txtMob1.Name = "txtMob1";
            this.txtMob1.Size = new System.Drawing.Size(264, 23);
            this.txtMob1.TabIndex = 128;
            this.txtMob1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtMob1_KeyDown);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label4.Font = new System.Drawing.Font("Tahoma", 9.75F);
            this.label4.Location = new System.Drawing.Point(465, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(264, 20);
            this.label4.TabIndex = 130;
            this.label4.Text = "رقم الموبايل ";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // group_addNewCustomer
            // 
            this.group_addNewCustomer.Controls.Add(this.tableLayoutPanel2);
            this.group_addNewCustomer.Dock = System.Windows.Forms.DockStyle.Top;
            this.group_addNewCustomer.Location = new System.Drawing.Point(0, 65);
            this.group_addNewCustomer.Name = "group_addNewCustomer";
            this.group_addNewCustomer.Size = new System.Drawing.Size(1008, 79);
            this.group_addNewCustomer.TabIndex = 209;
            this.group_addNewCustomer.TabStop = false;
            this.group_addNewCustomer.Text = "اضافة عميل جديد";
            // 
            // ItemProductionFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1008, 421);
            this.Controls.Add(this.group_Cancel);
            this.Controls.Add(this.group_itemProductionCust);
            this.Controls.Add(this.group_searchCustomer);
            this.Controls.Add(this.group_addItems);
            this.Controls.Add(this.group_addNewCustomer);
            this.Controls.Add(this.groupBox3);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "ItemProductionFrm";
            this.Text = "التوليفات";
            this.Load += new System.EventHandler(this.ItemProductionFrm_Load);
            this.groupBox3.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.group_searchCustomer.ResumeLayout(false);
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_search)).EndInit();
            this.group_itemProductionCust.ResumeLayout(false);
            this.tableLayoutPanel5.ResumeLayout(false);
            this.tableLayoutPanel5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_itemProductions)).EndInit();
            this.group_Cancel.ResumeLayout(false);
            this.tableLayoutPanel6.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_items)).EndInit();
            this.group_addItems.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.group_addNewCustomer.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.RadioButton rdo_newCustomer;
        private System.Windows.Forms.RadioButton rdo_existCustomer;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.GroupBox group_searchCustomer;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.TextBox txt_searchCustNam;
        private System.Windows.Forms.GroupBox group_itemProductionCust;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DataGridView dgv_itemProductions;
        private System.Windows.Forms.GroupBox group_Cancel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel6;
        private Utilities.ReusableControls.DangerButton dangerButton1;
        private System.Windows.Forms.DataGridView dgv_search;
        private System.Windows.Forms.DataGridViewTextBoxColumn Person_ID;
        private System.Windows.Forms.DataGridViewTextBoxColumn Person_Name;
        private System.Windows.Forms.DataGridViewTextBoxColumn Mobil1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Mobil2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Cus_Address;
        private System.Windows.Forms.DataGridViewImageColumn addNew;
        private System.Windows.Forms.DataGridViewTextBoxColumn ItemProductionId;
        private System.Windows.Forms.DataGridViewTextBoxColumn CustomerName;
        private System.Windows.Forms.DataGridViewTextBoxColumn ItemProductionName;
        private System.Windows.Forms.DataGridViewTextBoxColumn CreatedOn;
        private System.Windows.Forms.DataGridViewImageColumn show;
        private System.Windows.Forms.DataGridViewImageColumn selectItem;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private Utilities.ReusableControls.DangerButton btn_Cancel;
        private Utilities.ReusableControls.SuccessButton btn_UpdateSaleInvoive;
        public System.Windows.Forms.TextBox txt_itemProduName;
        private System.Windows.Forms.Label label2;
        public System.Windows.Forms.TextBox txtItemCode;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.DataGridView dgv_items;
        private System.Windows.Forms.DataGridViewTextBoxColumn ItemId;
        private System.Windows.Forms.DataGridViewTextBoxColumn ItemBarcode;
        private System.Windows.Forms.DataGridViewTextBoxColumn ItemName;
        private System.Windows.Forms.DataGridViewTextBoxColumn Price;
        private System.Windows.Forms.DataGridViewTextBoxColumn Quantity;
        private System.Windows.Forms.DataGridViewTextBoxColumn Amount;
        private System.Windows.Forms.DataGridViewImageColumn del;
        private Utilities.ReusableControls.DefaultButton btnAddItem;
        public System.Windows.Forms.TextBox txt_quantity;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label7;
        public System.Windows.Forms.TextBox txt_itemName;
        public System.Windows.Forms.TextBox txtPrice;
        private System.Windows.Forms.GroupBox group_addItems;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label label5;
        public System.Windows.Forms.TextBox txtAddress;
        public System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label label10;
        public System.Windows.Forms.TextBox txtMob1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox group_addNewCustomer;
    }
}