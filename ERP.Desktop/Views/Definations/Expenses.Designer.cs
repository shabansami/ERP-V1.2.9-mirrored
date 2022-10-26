namespace ERP.Desktop.Views.Definations
{
    partial class Expenses
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
            this.dgv_Expens = new System.Windows.Forms.DataGridView();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btn_Cancel = new ERP.Desktop.Utilities.ReusableControls.DangerButton();
            this.btn_Update = new ERP.Desktop.Utilities.ReusableControls.SuccessButton();
            this.btn_Add = new ERP.Desktop.Utilities.ReusableControls.DefaultButton();
            this.label1 = new System.Windows.Forms.Label();
            this.txt_Expens = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.ExpensID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ExpensName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_Expens)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.Color.WhiteSmoke;
            this.groupBox2.Controls.Add(this.dgv_Expens);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.groupBox2.Location = new System.Drawing.Point(0, 84);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(334, 227);
            this.groupBox2.TabIndex = 182;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "انواع المصروفات";
            // 
            // dgv_Expens
            // 
            this.dgv_Expens.AllowUserToAddRows = false;
            this.dgv_Expens.AllowUserToDeleteRows = false;
            this.dgv_Expens.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgv_Expens.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_Expens.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ExpensID,
            this.ExpensName});
            this.dgv_Expens.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgv_Expens.Location = new System.Drawing.Point(3, 19);
            this.dgv_Expens.Margin = new System.Windows.Forms.Padding(3, 22, 3, 22);
            this.dgv_Expens.Name = "dgv_Expens";
            this.dgv_Expens.ReadOnly = true;
            this.dgv_Expens.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.dgv_Expens.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgv_Expens.RowTemplate.Height = 30;
            this.dgv_Expens.ShowEditingIcon = false;
            this.dgv_Expens.Size = new System.Drawing.Size(328, 205);
            this.dgv_Expens.TabIndex = 0;
            this.dgv_Expens.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgv_Expens_CellClick);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btn_Cancel);
            this.groupBox1.Controls.Add(this.btn_Update);
            this.groupBox1.Controls.Add(this.btn_Add);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.txt_Expens);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(334, 84);
            this.groupBox1.TabIndex = 203;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "نوع المصروف";
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
            this.btn_Cancel.Location = new System.Drawing.Point(12, 48);
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.Size = new System.Drawing.Size(80, 30);
            this.btn_Cancel.TabIndex = 208;
            this.btn_Cancel.Text = "رجوع";
            this.btn_Cancel.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btn_Cancel.UseVisualStyleBackColor = false;
            this.btn_Cancel.Visible = false;
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
            this.btn_Update.Location = new System.Drawing.Point(98, 48);
            this.btn_Update.Name = "btn_Update";
            this.btn_Update.Size = new System.Drawing.Size(80, 30);
            this.btn_Update.TabIndex = 207;
            this.btn_Update.Text = "حفظ";
            this.btn_Update.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btn_Update.UseVisualStyleBackColor = false;
            this.btn_Update.Visible = false;
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
            this.btn_Add.Location = new System.Drawing.Point(12, 48);
            this.btn_Add.Name = "btn_Add";
            this.btn_Add.Size = new System.Drawing.Size(80, 30);
            this.btn_Add.TabIndex = 206;
            this.btn_Add.Text = "إضافة";
            this.btn_Add.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btn_Add.UseVisualStyleBackColor = false;
            this.btn_Add.Click += new System.EventHandler(this.btn_Add_Click_1);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 9.75F);
            this.label1.ForeColor = System.Drawing.Color.Firebrick;
            this.label1.Location = new System.Drawing.Point(216, 20);
            this.label1.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(15, 16);
            this.label1.TabIndex = 205;
            this.label1.Text = "*";
            // 
            // txt_Expens
            // 
            this.txt_Expens.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_Expens.Location = new System.Drawing.Point(12, 17);
            this.txt_Expens.Margin = new System.Windows.Forms.Padding(3, 22, 3, 22);
            this.txt_Expens.Name = "txt_Expens";
            this.txt_Expens.Size = new System.Drawing.Size(196, 23);
            this.txt_Expens.TabIndex = 203;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(222, 20);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(88, 16);
            this.label3.TabIndex = 204;
            this.label3.Text = "نوع المصروف : ";
            // 
            // ExpensID
            // 
            this.ExpensID.DataPropertyName = "Id";
            this.ExpensID.HeaderText = "Id";
            this.ExpensID.Name = "ExpensID";
            this.ExpensID.ReadOnly = true;
            this.ExpensID.Visible = false;
            // 
            // ExpensName
            // 
            this.ExpensName.DataPropertyName = "Name";
            this.ExpensName.HeaderText = "نوع المصروف";
            this.ExpensName.Name = "ExpensName";
            this.ExpensName.ReadOnly = true;
            // 
            // Expenses
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(334, 311);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "Expenses";
            this.Text = "ادارة انواع المصروفات";
            this.Load += new System.EventHandler(this.Expenses_Load);
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgv_Expens)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.DataGridView dgv_Expens;
        private System.Windows.Forms.GroupBox groupBox1;
        private ERP.Desktop.Utilities.ReusableControls.DangerButton btn_Cancel;
        private ERP.Desktop.Utilities.ReusableControls.SuccessButton btn_Update;
        private ERP.Desktop.Utilities.ReusableControls.DefaultButton btn_Add;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txt_Expens;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DataGridViewTextBoxColumn ExpensID;
        private System.Windows.Forms.DataGridViewTextBoxColumn ExpensName;
    }
}