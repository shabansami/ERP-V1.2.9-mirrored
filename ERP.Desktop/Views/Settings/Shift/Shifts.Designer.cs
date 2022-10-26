namespace ERP.Desktop.Views.Settings.Shift
{
    partial class Shifts
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Shifts));
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.dgv_shift = new System.Windows.Forms.DataGridView();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cmbPOSs = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnAddEmployee = new FontAwesome.Sharp.IconButton();
            this.dtpShiftDate = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.btnAdd = new FontAwesome.Sharp.IconButton();
            this.cmbEmps = new System.Windows.Forms.ComboBox();
            this.lblcenter = new System.Windows.Forms.Label();
            this.Shift_ID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ShiftNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CreatedOn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Person_Name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Person_ID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PointName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.close = new System.Windows.Forms.DataGridViewButtonColumn();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_shift)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.Color.WhiteSmoke;
            this.groupBox2.Controls.Add(this.dgv_shift);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.groupBox2.Location = new System.Drawing.Point(3, 95);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(511, 277);
            this.groupBox2.TabIndex = 12;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "الورديات المفتوحة";
            // 
            // dgv_shift
            // 
            this.dgv_shift.AllowUserToAddRows = false;
            this.dgv_shift.AllowUserToDeleteRows = false;
            this.dgv_shift.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgv_shift.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_shift.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Shift_ID,
            this.ShiftNumber,
            this.CreatedOn,
            this.Person_Name,
            this.Person_ID,
            this.PointName,
            this.close});
            this.dgv_shift.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgv_shift.Location = new System.Drawing.Point(3, 19);
            this.dgv_shift.Margin = new System.Windows.Forms.Padding(3, 22, 3, 22);
            this.dgv_shift.Name = "dgv_shift";
            this.dgv_shift.ReadOnly = true;
            this.dgv_shift.RowTemplate.Height = 30;
            this.dgv_shift.Size = new System.Drawing.Size(505, 255);
            this.dgv_shift.TabIndex = 2;
            this.dgv_shift.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgv_shift_CellContentClick);
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.groupBox1.Controls.Add(this.cmbPOSs);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.btnAddEmployee);
            this.groupBox1.Controls.Add(this.dtpShiftDate);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.btnAdd);
            this.groupBox1.Controls.Add(this.cmbEmps);
            this.groupBox1.Controls.Add(this.lblcenter);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(3, 22, 3, 22);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(3, 22, 3, 22);
            this.groupBox1.Size = new System.Drawing.Size(511, 93);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "فتح وردية";
            // 
            // cmbPOSs
            // 
            this.cmbPOSs.FormattingEnabled = true;
            this.cmbPOSs.Items.AddRange(new object[] {
            "--اختر--",
            "نقطة 1",
            "نقطة 2",
            "نقطة 3",
            "نقطة 4",
            "نقطة 5",
            "نقطة 6",
            "نقطة 7",
            "نقطة 8",
            "نقطة 9",
            "نقطة 10"});
            this.cmbPOSs.Location = new System.Drawing.Point(12, 17);
            this.cmbPOSs.Name = "cmbPOSs";
            this.cmbPOSs.Size = new System.Drawing.Size(150, 24);
            this.cmbPOSs.TabIndex = 58;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(168, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 16);
            this.label1.TabIndex = 57;
            this.label1.Text = "نقطة البيع :";
            // 
            // btnAddEmployee
            // 
            this.btnAddEmployee.BackColor = System.Drawing.Color.Gray;
            this.btnAddEmployee.FlatAppearance.BorderColor = System.Drawing.SystemColors.ActiveBorder;
            this.btnAddEmployee.FlatAppearance.BorderSize = 0;
            this.btnAddEmployee.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddEmployee.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold);
            this.btnAddEmployee.ForeColor = System.Drawing.Color.White;
            this.btnAddEmployee.IconChar = FontAwesome.Sharp.IconChar.Plus;
            this.btnAddEmployee.IconColor = System.Drawing.Color.White;
            this.btnAddEmployee.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnAddEmployee.IconSize = 18;
            this.btnAddEmployee.Location = new System.Drawing.Point(236, 49);
            this.btnAddEmployee.Margin = new System.Windows.Forms.Padding(0);
            this.btnAddEmployee.Name = "btnAddEmployee";
            this.btnAddEmployee.Size = new System.Drawing.Size(33, 24);
            this.btnAddEmployee.TabIndex = 10;
            this.btnAddEmployee.UseVisualStyleBackColor = false;
            this.btnAddEmployee.Click += new System.EventHandler(this.btnAddEmployee_Click);
            // 
            // dtpShiftDate
            // 
            this.dtpShiftDate.CustomFormat = "yyyy/MM/dd";
            this.dtpShiftDate.Enabled = false;
            this.dtpShiftDate.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtpShiftDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpShiftDate.Location = new System.Drawing.Point(271, 18);
            this.dtpShiftDate.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.dtpShiftDate.Name = "dtpShiftDate";
            this.dtpShiftDate.RightToLeftLayout = true;
            this.dtpShiftDate.Size = new System.Drawing.Size(150, 23);
            this.dtpShiftDate.TabIndex = 8;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(427, 20);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(50, 16);
            this.label2.TabIndex = 56;
            this.label2.Text = "التاريخ :";
            // 
            // btnAdd
            // 
            this.btnAdd.BackColor = System.Drawing.Color.DeepSkyBlue;
            this.btnAdd.FlatAppearance.BorderColor = System.Drawing.SystemColors.ActiveBorder;
            this.btnAdd.FlatAppearance.BorderSize = 0;
            this.btnAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAdd.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAdd.ForeColor = System.Drawing.Color.White;
            this.btnAdd.IconChar = FontAwesome.Sharp.IconChar.Stopwatch;
            this.btnAdd.IconColor = System.Drawing.Color.White;
            this.btnAdd.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnAdd.IconSize = 23;
            this.btnAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAdd.Location = new System.Drawing.Point(12, 53);
            this.btnAdd.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnAdd.Size = new System.Drawing.Size(70, 30);
            this.btnAdd.TabIndex = 13;
            this.btnAdd.Text = "فتح";
            this.btnAdd.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnAdd.UseVisualStyleBackColor = false;
            this.btnAdd.Click += new System.EventHandler(this.btn_add_Click);
            // 
            // cmbEmps
            // 
            this.cmbEmps.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbEmps.FormattingEnabled = true;
            this.cmbEmps.Location = new System.Drawing.Point(271, 49);
            this.cmbEmps.Margin = new System.Windows.Forms.Padding(3, 1, 3, 1);
            this.cmbEmps.Name = "cmbEmps";
            this.cmbEmps.Size = new System.Drawing.Size(150, 24);
            this.cmbEmps.TabIndex = 9;
            // 
            // lblcenter
            // 
            this.lblcenter.AutoSize = true;
            this.lblcenter.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblcenter.Location = new System.Drawing.Point(427, 52);
            this.lblcenter.Name = "lblcenter";
            this.lblcenter.Size = new System.Drawing.Size(58, 16);
            this.lblcenter.TabIndex = 1;
            this.lblcenter.Text = "الموظف :";
            // 
            // Shift_ID
            // 
            this.Shift_ID.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.Shift_ID.DataPropertyName = "ID";
            this.Shift_ID.HeaderText = "ID";
            this.Shift_ID.Name = "Shift_ID";
            this.Shift_ID.ReadOnly = true;
            this.Shift_ID.Visible = false;
            // 
            // ShiftNumber
            // 
            this.ShiftNumber.DataPropertyName = "ShiftNumber";
            this.ShiftNumber.HeaderText = "رقم الوردية";
            this.ShiftNumber.Name = "ShiftNumber";
            this.ShiftNumber.ReadOnly = true;
            // 
            // CreatedOn
            // 
            this.CreatedOn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.CreatedOn.DataPropertyName = "Date";
            dataGridViewCellStyle1.Format = "yyyy/MM/dd";
            dataGridViewCellStyle1.NullValue = null;
            this.CreatedOn.DefaultCellStyle = dataGridViewCellStyle1;
            this.CreatedOn.HeaderText = "تاريخ الوردية";
            this.CreatedOn.Name = "CreatedOn";
            this.CreatedOn.ReadOnly = true;
            // 
            // Person_Name
            // 
            this.Person_Name.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.Person_Name.DataPropertyName = "EmpName";
            this.Person_Name.HeaderText = "الموظف";
            this.Person_Name.Name = "Person_Name";
            this.Person_Name.ReadOnly = true;
            this.Person_Name.Width = 150;
            // 
            // Person_ID
            // 
            this.Person_ID.DataPropertyName = "EmpID";
            this.Person_ID.HeaderText = "Person_ID";
            this.Person_ID.Name = "Person_ID";
            this.Person_ID.ReadOnly = true;
            this.Person_ID.Visible = false;
            // 
            // PointName
            // 
            this.PointName.DataPropertyName = "POSName";
            this.PointName.HeaderText = "نقطة البيع";
            this.PointName.Name = "PointName";
            this.PointName.ReadOnly = true;
            // 
            // close
            // 
            this.close.HeaderText = "غلق";
            this.close.Name = "close";
            this.close.ReadOnly = true;
            this.close.Text = "غلق";
            this.close.ToolTipText = "غلق الوردية";
            this.close.UseColumnTextForButtonValue = true;
            // 
            // Shifts
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(517, 375);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "Shifts";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Tag = 10;
            this.Text = "الورديات";
            this.Load += new System.EventHandler(this.Shifts_Load);
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgv_shift)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.DataGridView dgv_shift;
        private System.Windows.Forms.Label lblcenter;
        private System.Windows.Forms.ComboBox cmbEmps;
        private FontAwesome.Sharp.IconButton btnAdd;
        private System.Windows.Forms.DateTimePicker dtpShiftDate;
        private System.Windows.Forms.Label label2;
        private FontAwesome.Sharp.IconButton btnAddEmployee;
        private System.Windows.Forms.ComboBox cmbPOSs;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridViewTextBoxColumn PointOfSale;
        private System.Windows.Forms.DataGridViewTextBoxColumn Shift_ID;
        private System.Windows.Forms.DataGridViewTextBoxColumn ShiftNumber;
        private System.Windows.Forms.DataGridViewTextBoxColumn CreatedOn;
        private System.Windows.Forms.DataGridViewTextBoxColumn Person_Name;
        private System.Windows.Forms.DataGridViewTextBoxColumn Person_ID;
        private System.Windows.Forms.DataGridViewTextBoxColumn PointName;
        private System.Windows.Forms.DataGridViewButtonColumn close;
    }
}