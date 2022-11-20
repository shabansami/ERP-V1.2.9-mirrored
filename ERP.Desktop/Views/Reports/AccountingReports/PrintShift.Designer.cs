namespace ERP.Desktop.Views.Reports.AccountingReports
{
    partial class PrintShift
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
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.dgv_shift = new System.Windows.Forms.DataGridView();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.dtpTo = new System.Windows.Forms.DateTimePicker();
            this.dtpFrom = new System.Windows.Forms.DateTimePicker();
            this.btnSearch = new ERP.Desktop.Utilities.ReusableControls.SuccessButton();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.ShiftNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CreatedOn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Person_Name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Person_ID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PointName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Shift_ID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.print = new System.Windows.Forms.DataGridViewButtonColumn();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_shift)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.dgv_shift);
            this.groupBox2.Location = new System.Drawing.Point(3, 67);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(633, 316);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "الورديات المغلقة";
            // 
            // dgv_shift
            // 
            this.dgv_shift.AllowUserToAddRows = false;
            this.dgv_shift.AllowUserToDeleteRows = false;
            this.dgv_shift.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgv_shift.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_shift.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ShiftNumber,
            this.CreatedOn,
            this.Person_Name,
            this.Person_ID,
            this.PointName,
            this.Shift_ID,
            this.print});
            this.dgv_shift.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgv_shift.Location = new System.Drawing.Point(3, 19);
            this.dgv_shift.Margin = new System.Windows.Forms.Padding(3, 22, 3, 22);
            this.dgv_shift.Name = "dgv_shift";
            this.dgv_shift.ReadOnly = true;
            this.dgv_shift.RowTemplate.Height = 30;
            this.dgv_shift.Size = new System.Drawing.Size(627, 294);
            this.dgv_shift.TabIndex = 3;
            this.dgv_shift.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgv_shift_CellContentClick);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.dtpTo);
            this.groupBox1.Controls.Add(this.dtpFrom);
            this.groupBox1.Controls.Add(this.btnSearch);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Location = new System.Drawing.Point(3, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(633, 61);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "معايير البحث";
            // 
            // dtpTo
            // 
            this.dtpTo.Location = new System.Drawing.Point(154, 22);
            this.dtpTo.Name = "dtpTo";
            this.dtpTo.Size = new System.Drawing.Size(193, 23);
            this.dtpTo.TabIndex = 3;
            // 
            // dtpFrom
            // 
            this.dtpFrom.Location = new System.Drawing.Point(392, 22);
            this.dtpFrom.Name = "dtpFrom";
            this.dtpFrom.Size = new System.Drawing.Size(193, 23);
            this.dtpFrom.TabIndex = 3;
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
            this.btnSearch.Location = new System.Drawing.Point(9, 18);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(115, 30);
            this.btnSearch.TabIndex = 2;
            this.btnSearch.Text = "بحث";
            this.btnSearch.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnSearch.UseVisualStyleBackColor = false;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(353, 25);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(36, 16);
            this.label7.TabIndex = 0;
            this.label7.Text = "الي :";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(591, 25);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(33, 16);
            this.label6.TabIndex = 0;
            this.label6.Text = "من :";
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
            // Shift_ID
            // 
            this.Shift_ID.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.Shift_ID.DataPropertyName = "ID";
            this.Shift_ID.HeaderText = "ID";
            this.Shift_ID.Name = "Shift_ID";
            this.Shift_ID.ReadOnly = true;
            this.Shift_ID.Visible = false;
            // 
            // print
            // 
            this.print.HeaderText = "طباعه";
            this.print.Name = "print";
            this.print.ReadOnly = true;
            this.print.Text = "طباعه";
            this.print.ToolTipText = "طباعه الوردية";
            this.print.UseColumnTextForButtonValue = true;
            // 
            // PrintShift
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(639, 386);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "PrintShift";
            this.Text = "طباعة وردية مغلقة";
            this.Load += new System.EventHandler(this.PrintShift_Load);
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgv_shift)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private Utilities.ReusableControls.SuccessButton btnSearch;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.DateTimePicker dtpTo;
        private System.Windows.Forms.DateTimePicker dtpFrom;
        private System.Windows.Forms.DataGridView dgv_shift;
        private System.Windows.Forms.DataGridViewTextBoxColumn ShiftNumber;
        private System.Windows.Forms.DataGridViewTextBoxColumn CreatedOn;
        private System.Windows.Forms.DataGridViewTextBoxColumn Person_Name;
        private System.Windows.Forms.DataGridViewTextBoxColumn Person_ID;
        private System.Windows.Forms.DataGridViewTextBoxColumn PointName;
        private System.Windows.Forms.DataGridViewTextBoxColumn Shift_ID;
        private System.Windows.Forms.DataGridViewButtonColumn print;
    }
}