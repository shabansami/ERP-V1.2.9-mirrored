namespace ERP.Desktop.Views.Transactions.Outcoming
{
    partial class EmployeeLoans
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
            this.label15 = new System.Windows.Forms.Label();
            this.txt_AmountMonth = new ERP.Desktop.Utilities.ReusableControls.TextBoxNum();
            this.lbl_AmountMonth = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.txt_numberMonths = new ERP.Desktop.Utilities.ReusableControls.TextBoxNum();
            this.label14 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.cmbo_contractScheduling = new System.Windows.Forms.ComboBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.btnSave = new ERP.Desktop.Utilities.ReusableControls.SuccessButton();
            this.btnCancel = new ERP.Desktop.Utilities.ReusableControls.DangerButton();
            this.btnAdd = new ERP.Desktop.Utilities.ReusableControls.DefaultButton();
            this.txt_amount = new ERP.Desktop.Utilities.ReusableControls.TextBoxNum();
            this.txt_notes = new System.Windows.Forms.TextBox();
            this.dtp_Date = new System.Windows.Forms.DateTimePicker();
            this.cmbo_Department = new System.Windows.Forms.ComboBox();
            this.cmbo_employee = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.cmb_safes = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cmb_branches = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.dgv_loanEmployees = new System.Windows.Forms.DataGridView();
            this.ID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.EmployeeName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ContractSchedulingName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Amount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PaymentDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SafeName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Notes = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.IsApprovalStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colEdit = new System.Windows.Forms.DataGridViewImageColumn();
            this.colDelete = new System.Windows.Forms.DataGridViewImageColumn();
            this.btnPrint = new ERP.Desktop.Utilities.ReusableControls.PrintButton();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_loanEmployees)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label15);
            this.groupBox1.Controls.Add(this.txt_AmountMonth);
            this.groupBox1.Controls.Add(this.lbl_AmountMonth);
            this.groupBox1.Controls.Add(this.label13);
            this.groupBox1.Controls.Add(this.txt_numberMonths);
            this.groupBox1.Controls.Add(this.label14);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.cmbo_contractScheduling);
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.label19);
            this.groupBox1.Controls.Add(this.btnSave);
            this.groupBox1.Controls.Add(this.btnCancel);
            this.groupBox1.Controls.Add(this.btnAdd);
            this.groupBox1.Controls.Add(this.txt_amount);
            this.groupBox1.Controls.Add(this.txt_notes);
            this.groupBox1.Controls.Add(this.dtp_Date);
            this.groupBox1.Controls.Add(this.cmbo_Department);
            this.groupBox1.Controls.Add(this.cmbo_employee);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label16);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.cmb_safes);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.cmb_branches);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(5, 1);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(725, 192);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "البيانات الاساسية للسلفة";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Tahoma", 9.75F);
            this.label15.ForeColor = System.Drawing.Color.Firebrick;
            this.label15.Location = new System.Drawing.Point(90, 82);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(15, 16);
            this.label15.TabIndex = 83;
            this.label15.Text = "*";
            // 
            // txt_AmountMonth
            // 
            this.txt_AmountMonth.Location = new System.Drawing.Point(10, 75);
            this.txt_AmountMonth.Name = "txt_AmountMonth";
            this.txt_AmountMonth.Num = 0D;
            this.txt_AmountMonth.ReadOnly = true;
            this.txt_AmountMonth.RoundDigits = 3;
            this.txt_AmountMonth.Size = new System.Drawing.Size(75, 23);
            this.txt_AmountMonth.TabIndex = 82;
            this.txt_AmountMonth.Text = "0";
            this.txt_AmountMonth.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txt_AmountMonth.VisibleDigits = false;
            // 
            // lbl_AmountMonth
            // 
            this.lbl_AmountMonth.AutoSize = true;
            this.lbl_AmountMonth.Location = new System.Drawing.Point(97, 82);
            this.lbl_AmountMonth.Name = "lbl_AmountMonth";
            this.lbl_AmountMonth.Size = new System.Drawing.Size(92, 16);
            this.lbl_AmountMonth.TabIndex = 81;
            this.lbl_AmountMonth.Text = "قيمة كل قسط :";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Tahoma", 9.75F);
            this.label13.ForeColor = System.Drawing.Color.Firebrick;
            this.label13.Location = new System.Drawing.Point(92, 49);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(15, 16);
            this.label13.TabIndex = 80;
            this.label13.Text = "*";
            // 
            // txt_numberMonths
            // 
            this.txt_numberMonths.Location = new System.Drawing.Point(10, 46);
            this.txt_numberMonths.Name = "txt_numberMonths";
            this.txt_numberMonths.Num = 1D;
            this.txt_numberMonths.RoundDigits = 3;
            this.txt_numberMonths.Size = new System.Drawing.Size(75, 23);
            this.txt_numberMonths.TabIndex = 79;
            this.txt_numberMonths.Text = "1";
            this.txt_numberMonths.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txt_numberMonths.VisibleDigits = false;
            this.txt_numberMonths.TextChanged += new System.EventHandler(this.txt_numberMonths_TextChanged);
            this.txt_numberMonths.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txt_numberMonths_KeyPress);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(103, 49);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(76, 16);
            this.label14.TabIndex = 78;
            this.label14.Text = "عدد الشهور:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Tahoma", 9.75F);
            this.label5.ForeColor = System.Drawing.Color.Firebrick;
            this.label5.Location = new System.Drawing.Point(625, 84);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(15, 16);
            this.label5.TabIndex = 77;
            this.label5.Text = "*";
            // 
            // cmbo_contractScheduling
            // 
            this.cmbo_contractScheduling.FormattingEnabled = true;
            this.cmbo_contractScheduling.Location = new System.Drawing.Point(421, 80);
            this.cmbo_contractScheduling.Name = "cmbo_contractScheduling";
            this.cmbo_contractScheduling.Size = new System.Drawing.Size(204, 24);
            this.cmbo_contractScheduling.TabIndex = 76;
            this.cmbo_contractScheduling.SelectedIndexChanged += new System.EventHandler(this.cmbo_contractScheduling_SelectedIndexChanged);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(637, 84);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(84, 16);
            this.label12.TabIndex = 75;
            this.label12.Text = "بداية اول قسط";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Tahoma", 9.75F);
            this.label11.ForeColor = System.Drawing.Color.Firebrick;
            this.label11.Location = new System.Drawing.Point(89, 20);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(15, 16);
            this.label11.TabIndex = 74;
            this.label11.Text = "*";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Tahoma", 9.75F);
            this.label10.ForeColor = System.Drawing.Color.Firebrick;
            this.label10.Location = new System.Drawing.Point(343, 20);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(15, 16);
            this.label10.TabIndex = 74;
            this.label10.Text = "*";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Tahoma", 9.75F);
            this.label9.ForeColor = System.Drawing.Color.Firebrick;
            this.label9.Location = new System.Drawing.Point(343, 51);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(15, 16);
            this.label9.TabIndex = 74;
            this.label9.Text = "*";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Tahoma", 9.75F);
            this.label8.ForeColor = System.Drawing.Color.Firebrick;
            this.label8.Location = new System.Drawing.Point(659, 52);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(15, 16);
            this.label8.TabIndex = 74;
            this.label8.Text = "*";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Font = new System.Drawing.Font("Tahoma", 9.75F);
            this.label19.ForeColor = System.Drawing.Color.Firebrick;
            this.label19.Location = new System.Drawing.Point(344, 82);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(15, 16);
            this.label19.TabIndex = 74;
            this.label19.Text = "*";
            // 
            // btnSave
            // 
            this.btnSave.BackColor = System.Drawing.Color.Turquoise;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.btnSave.ForeColor = System.Drawing.Color.White;
            this.btnSave.IconChar = FontAwesome.Sharp.IconChar.Save;
            this.btnSave.IconColor = System.Drawing.Color.White;
            this.btnSave.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnSave.IconSize = 22;
            this.btnSave.Location = new System.Drawing.Point(105, 156);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(90, 30);
            this.btnSave.TabIndex = 7;
            this.btnSave.Text = "حفظ";
            this.btnSave.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Visible = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
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
            this.btnCancel.Location = new System.Drawing.Point(8, 156);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(90, 30);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "الغاء";
            this.btnCancel.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Visible = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
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
            this.btnAdd.Location = new System.Drawing.Point(7, 156);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(90, 30);
            this.btnAdd.TabIndex = 5;
            this.btnAdd.Text = "اضافة";
            this.btnAdd.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnAdd.UseVisualStyleBackColor = false;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // txt_amount
            // 
            this.txt_amount.Location = new System.Drawing.Point(10, 17);
            this.txt_amount.Name = "txt_amount";
            this.txt_amount.Num = 0D;
            this.txt_amount.RoundDigits = 3;
            this.txt_amount.Size = new System.Drawing.Size(75, 23);
            this.txt_amount.TabIndex = 4;
            this.txt_amount.Text = "0";
            this.txt_amount.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txt_amount.VisibleDigits = false;
            this.txt_amount.TextChanged += new System.EventHandler(this.txt_amount_TextChanged);
            this.txt_amount.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txt_amount_KeyPress);
            // 
            // txt_notes
            // 
            this.txt_notes.Location = new System.Drawing.Point(8, 113);
            this.txt_notes.Multiline = true;
            this.txt_notes.Name = "txt_notes";
            this.txt_notes.Size = new System.Drawing.Size(651, 40);
            this.txt_notes.TabIndex = 3;
            // 
            // dtp_Date
            // 
            this.dtp_Date.Location = new System.Drawing.Point(193, 79);
            this.dtp_Date.Name = "dtp_Date";
            this.dtp_Date.Size = new System.Drawing.Size(150, 23);
            this.dtp_Date.TabIndex = 2;
            // 
            // cmbo_Department
            // 
            this.cmbo_Department.FormattingEnabled = true;
            this.cmbo_Department.Location = new System.Drawing.Point(421, 20);
            this.cmbo_Department.Name = "cmbo_Department";
            this.cmbo_Department.Size = new System.Drawing.Size(204, 24);
            this.cmbo_Department.TabIndex = 1;
            this.cmbo_Department.SelectedIndexChanged += new System.EventHandler(this.cmbo_Department_SelectedIndexChanged);
            // 
            // cmbo_employee
            // 
            this.cmbo_employee.FormattingEnabled = true;
            this.cmbo_employee.Location = new System.Drawing.Point(421, 49);
            this.cmbo_employee.Name = "cmbo_employee";
            this.cmbo_employee.Size = new System.Drawing.Size(204, 24);
            this.cmbo_employee.TabIndex = 1;
            this.cmbo_employee.SelectedIndexChanged += new System.EventHandler(this.cmbo_employee_SelectedIndexChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(100, 20);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(49, 16);
            this.label6.TabIndex = 0;
            this.label6.Text = "المبلغ :";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(670, 24);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(38, 16);
            this.label16.TabIndex = 0;
            this.label16.Text = "الادارة";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(671, 52);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(49, 16);
            this.label3.TabIndex = 0;
            this.label3.Text = "الموظف";
            // 
            // cmb_safes
            // 
            this.cmb_safes.FormattingEnabled = true;
            this.cmb_safes.Location = new System.Drawing.Point(193, 47);
            this.cmb_safes.Name = "cmb_safes";
            this.cmb_safes.Size = new System.Drawing.Size(150, 24);
            this.cmb_safes.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(355, 51);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 16);
            this.label2.TabIndex = 0;
            this.label2.Text = "الخزينة :";
            // 
            // cmb_branches
            // 
            this.cmb_branches.FormattingEnabled = true;
            this.cmb_branches.Location = new System.Drawing.Point(193, 16);
            this.cmb_branches.Name = "cmb_branches";
            this.cmb_branches.Size = new System.Drawing.Size(150, 24);
            this.cmb_branches.TabIndex = 1;
            this.cmb_branches.SelectedIndexChanged += new System.EventHandler(this.cmbBranches_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(357, 82);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(50, 16);
            this.label4.TabIndex = 0;
            this.label4.Text = "التاريخ :";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(670, 124);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(46, 16);
            this.label7.TabIndex = 0;
            this.label7.Text = "البيان :";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(356, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "الفرع :";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.dgv_loanEmployees);
            this.groupBox2.Location = new System.Drawing.Point(5, 199);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(725, 259);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "السلف المسجلة";
            // 
            // dgv_loanEmployees
            // 
            this.dgv_loanEmployees.AllowUserToAddRows = false;
            this.dgv_loanEmployees.AllowUserToDeleteRows = false;
            this.dgv_loanEmployees.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_loanEmployees.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ID,
            this.EmployeeName,
            this.ContractSchedulingName,
            this.Amount,
            this.PaymentDate,
            this.SafeName,
            this.Notes,
            this.IsApprovalStatus,
            this.colEdit,
            this.colDelete});
            this.dgv_loanEmployees.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgv_loanEmployees.Location = new System.Drawing.Point(3, 19);
            this.dgv_loanEmployees.Name = "dgv_loanEmployees";
            this.dgv_loanEmployees.ReadOnly = true;
            this.dgv_loanEmployees.Size = new System.Drawing.Size(719, 237);
            this.dgv_loanEmployees.TabIndex = 0;
            this.dgv_loanEmployees.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvCustomerPayments_CellClick);
            // 
            // ID
            // 
            this.ID.DataPropertyName = "Id";
            this.ID.HeaderText = "ID";
            this.ID.Name = "ID";
            this.ID.ReadOnly = true;
            this.ID.Visible = false;
            // 
            // EmployeeName
            // 
            this.EmployeeName.DataPropertyName = "EmployeeName";
            this.EmployeeName.HeaderText = "اسم الموظف";
            this.EmployeeName.Name = "EmployeeName";
            this.EmployeeName.ReadOnly = true;
            // 
            // ContractSchedulingName
            // 
            this.ContractSchedulingName.DataPropertyName = "ContractSchedulingName";
            this.ContractSchedulingName.HeaderText = "الشهر";
            this.ContractSchedulingName.Name = "ContractSchedulingName";
            this.ContractSchedulingName.ReadOnly = true;
            // 
            // Amount
            // 
            this.Amount.DataPropertyName = "Amount";
            this.Amount.HeaderText = "قيمة السلفة";
            this.Amount.Name = "Amount";
            this.Amount.ReadOnly = true;
            // 
            // PaymentDate
            // 
            this.PaymentDate.DataPropertyName = "PaymentDate";
            this.PaymentDate.HeaderText = "تاريخ العملية";
            this.PaymentDate.Name = "PaymentDate";
            this.PaymentDate.ReadOnly = true;
            // 
            // SafeName
            // 
            this.SafeName.DataPropertyName = "SafeName";
            this.SafeName.HeaderText = "الخزينة";
            this.SafeName.Name = "SafeName";
            this.SafeName.ReadOnly = true;
            // 
            // Notes
            // 
            this.Notes.DataPropertyName = "Notes";
            this.Notes.HeaderText = "البيان";
            this.Notes.Name = "Notes";
            this.Notes.ReadOnly = true;
            // 
            // IsApprovalStatus
            // 
            this.IsApprovalStatus.DataPropertyName = "IsApprovalStatus";
            this.IsApprovalStatus.HeaderText = "معتمد";
            this.IsApprovalStatus.Name = "IsApprovalStatus";
            this.IsApprovalStatus.ReadOnly = true;
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
            this.btnPrint.Location = new System.Drawing.Point(637, 461);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(90, 30);
            this.btnPrint.TabIndex = 3;
            this.btnPrint.Text = "طباعة";
            this.btnPrint.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnPrint.UseVisualStyleBackColor = false;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // EmployeeLoans
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(734, 497);
            this.Controls.Add(this.btnPrint);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "EmployeeLoans";
            this.Text = "سلفة لموظف";
            this.Load += new System.EventHandler(this.EmployeeLoans_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgv_loanEmployees)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label19;
        private Utilities.ReusableControls.SuccessButton btnSave;
        private Utilities.ReusableControls.DangerButton btnCancel;
        private Utilities.ReusableControls.DefaultButton btnAdd;
        private Utilities.ReusableControls.TextBoxNum txt_amount;
        private System.Windows.Forms.TextBox txt_notes;
        private System.Windows.Forms.DateTimePicker dtp_Date;
        private System.Windows.Forms.ComboBox cmbo_employee;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cmb_safes;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmb_branches;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.DataGridView dgv_loanEmployees;
        private Utilities.ReusableControls.PrintButton btnPrint;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cmbo_contractScheduling;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label15;
        private Utilities.ReusableControls.TextBoxNum txt_AmountMonth;
        private System.Windows.Forms.Label lbl_AmountMonth;
        private System.Windows.Forms.Label label13;
        private Utilities.ReusableControls.TextBoxNum txt_numberMonths;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.ComboBox cmbo_Department;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.DataGridViewTextBoxColumn ID;
        private System.Windows.Forms.DataGridViewTextBoxColumn EmployeeName;
        private System.Windows.Forms.DataGridViewTextBoxColumn ContractSchedulingName;
        private System.Windows.Forms.DataGridViewTextBoxColumn Amount;
        private System.Windows.Forms.DataGridViewTextBoxColumn PaymentDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn SafeName;
        private System.Windows.Forms.DataGridViewTextBoxColumn Notes;
        private System.Windows.Forms.DataGridViewTextBoxColumn IsApprovalStatus;
        private System.Windows.Forms.DataGridViewImageColumn colEdit;
        private System.Windows.Forms.DataGridViewImageColumn colDelete;
    }
}