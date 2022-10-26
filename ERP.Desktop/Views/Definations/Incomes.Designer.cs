namespace ERP.Desktop.Views.Definations
{
    partial class Incomes
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
            this.dgvIncomes = new System.Windows.Forms.DataGridView();
            this.label1 = new System.Windows.Forms.Label();
            this.txt_Revenue = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btn_Cancel = new ERP.Desktop.Utilities.ReusableControls.DangerButton();
            this.btn_Update = new ERP.Desktop.Utilities.ReusableControls.SuccessButton();
            this.btn_Add = new ERP.Desktop.Utilities.ReusableControls.DefaultButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.RevID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.RevName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvIncomes)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.Color.WhiteSmoke;
            this.groupBox2.Controls.Add(this.dgvIncomes);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.groupBox2.Location = new System.Drawing.Point(0, 82);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(334, 229);
            this.groupBox2.TabIndex = 182;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "انواع الايرادات";
            // 
            // dgvIncomes
            // 
            this.dgvIncomes.AllowUserToAddRows = false;
            this.dgvIncomes.AllowUserToDeleteRows = false;
            this.dgvIncomes.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvIncomes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvIncomes.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.RevID,
            this.RevName});
            this.dgvIncomes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvIncomes.Location = new System.Drawing.Point(3, 19);
            this.dgvIncomes.Margin = new System.Windows.Forms.Padding(3, 22, 3, 22);
            this.dgvIncomes.Name = "dgvIncomes";
            this.dgvIncomes.ReadOnly = true;
            this.dgvIncomes.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.dgvIncomes.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgvIncomes.RowTemplate.Height = 30;
            this.dgvIncomes.ShowEditingIcon = false;
            this.dgvIncomes.Size = new System.Drawing.Size(328, 207);
            this.dgvIncomes.TabIndex = 0;
            this.dgvIncomes.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgv_Revenue_CellContentClick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 9.75F);
            this.label1.ForeColor = System.Drawing.Color.Firebrick;
            this.label1.Location = new System.Drawing.Point(243, 19);
            this.label1.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(15, 16);
            this.label1.TabIndex = 185;
            this.label1.Text = "*";
            // 
            // txt_Revenue
            // 
            this.txt_Revenue.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_Revenue.Location = new System.Drawing.Point(12, 16);
            this.txt_Revenue.Margin = new System.Windows.Forms.Padding(3, 22, 3, 22);
            this.txt_Revenue.Name = "txt_Revenue";
            this.txt_Revenue.Size = new System.Drawing.Size(223, 23);
            this.txt_Revenue.TabIndex = 183;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(255, 19);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(67, 16);
            this.label3.TabIndex = 184;
            this.label3.Text = "نوع الايراد :";
            // 
            // btn_Cancel
            // 
            this.btn_Cancel.BackColor = System.Drawing.Color.Crimson;
            this.btn_Cancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Cancel.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_Cancel.ForeColor = System.Drawing.Color.White;
            this.btn_Cancel.IconChar = FontAwesome.Sharp.IconChar.Times;
            this.btn_Cancel.IconColor = System.Drawing.Color.White;
            this.btn_Cancel.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btn_Cancel.IconSize = 22;
            this.btn_Cancel.Location = new System.Drawing.Point(12, 46);
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.Size = new System.Drawing.Size(80, 30);
            this.btn_Cancel.TabIndex = 211;
            this.btn_Cancel.Text = "رجوع";
            this.btn_Cancel.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btn_Cancel.UseVisualStyleBackColor = false;
            this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
            // 
            // btn_Update
            // 
            this.btn_Update.BackColor = System.Drawing.Color.DarkTurquoise;
            this.btn_Update.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Update.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_Update.ForeColor = System.Drawing.Color.White;
            this.btn_Update.IconChar = FontAwesome.Sharp.IconChar.Save;
            this.btn_Update.IconColor = System.Drawing.Color.White;
            this.btn_Update.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btn_Update.IconSize = 22;
            this.btn_Update.Location = new System.Drawing.Point(98, 46);
            this.btn_Update.Name = "btn_Update";
            this.btn_Update.Size = new System.Drawing.Size(80, 30);
            this.btn_Update.TabIndex = 210;
            this.btn_Update.Text = "حفظ";
            this.btn_Update.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btn_Update.UseVisualStyleBackColor = false;
            this.btn_Update.Click += new System.EventHandler(this.btn_Update_Click);
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
            this.btn_Add.Location = new System.Drawing.Point(12, 46);
            this.btn_Add.Name = "btn_Add";
            this.btn_Add.Size = new System.Drawing.Size(80, 30);
            this.btn_Add.TabIndex = 209;
            this.btn_Add.Text = "إضافة";
            this.btn_Add.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btn_Add.UseVisualStyleBackColor = false;
            this.btn_Add.Click += new System.EventHandler(this.button1_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.btn_Cancel);
            this.groupBox1.Controls.Add(this.btn_Update);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.btn_Add);
            this.groupBox1.Controls.Add(this.txt_Revenue);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(334, 82);
            this.groupBox1.TabIndex = 212;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "نوع الايراد";
            // 
            // RevID
            // 
            this.RevID.DataPropertyName = "Id";
            this.RevID.HeaderText = "Id";
            this.RevID.Name = "RevID";
            this.RevID.ReadOnly = true;
            this.RevID.Visible = false;
            // 
            // RevName
            // 
            this.RevName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.RevName.DataPropertyName = "Name";
            this.RevName.HeaderText = "نوع الايراد";
            this.RevName.Name = "RevName";
            this.RevName.ReadOnly = true;
            // 
            // Incomes
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(334, 311);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "Incomes";
            this.Text = "الايرادات";
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvIncomes)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.DataGridView dgvIncomes;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txt_Revenue;
        private System.Windows.Forms.Label label3;
        private ERP.Desktop.Utilities.ReusableControls.DangerButton btn_Cancel;
        private ERP.Desktop.Utilities.ReusableControls.SuccessButton btn_Update;
        private ERP.Desktop.Utilities.ReusableControls.DefaultButton btn_Add;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DataGridViewTextBoxColumn RevID;
        private System.Windows.Forms.DataGridViewTextBoxColumn RevName;
    }
}