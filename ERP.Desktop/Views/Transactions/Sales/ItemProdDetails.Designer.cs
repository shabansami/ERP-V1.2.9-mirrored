namespace ERP.Desktop.Views.Transactions.Sales
{
    partial class ItemProdDetails
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
            this.btn_Cancel = new ERP.Desktop.Utilities.ReusableControls.DangerButton();
            this.txt_customerName = new System.Windows.Forms.TextBox();
            this.txt_ItemProName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.dgv_Itemspro = new System.Windows.Forms.DataGridView();
            this.ExpensID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ItemName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Quantity = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_Itemspro)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btn_Cancel);
            this.groupBox1.Controls.Add(this.txt_customerName);
            this.groupBox1.Controls.Add(this.txt_ItemProName);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(395, 98);
            this.groupBox1.TabIndex = 203;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "تفاصيل التوليفة";
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
            this.btn_Cancel.Location = new System.Drawing.Point(12, 41);
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.Size = new System.Drawing.Size(80, 30);
            this.btn_Cancel.TabIndex = 208;
            this.btn_Cancel.Text = "رجوع";
            this.btn_Cancel.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btn_Cancel.UseVisualStyleBackColor = false;
            this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
            // 
            // txt_customerName
            // 
            this.txt_customerName.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_customerName.Location = new System.Drawing.Point(113, 65);
            this.txt_customerName.Margin = new System.Windows.Forms.Padding(3, 22, 3, 22);
            this.txt_customerName.Name = "txt_customerName";
            this.txt_customerName.ReadOnly = true;
            this.txt_customerName.Size = new System.Drawing.Size(173, 23);
            this.txt_customerName.TabIndex = 203;
            // 
            // txt_ItemProName
            // 
            this.txt_ItemProName.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_ItemProName.Location = new System.Drawing.Point(113, 24);
            this.txt_ItemProName.Margin = new System.Windows.Forms.Padding(3, 22, 3, 22);
            this.txt_ItemProName.Name = "txt_ItemProName";
            this.txt_ItemProName.ReadOnly = true;
            this.txt_ItemProName.Size = new System.Drawing.Size(173, 23);
            this.txt_ItemProName.TabIndex = 203;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(315, 68);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 16);
            this.label1.TabIndex = 204;
            this.label1.Text = "اسم العميل";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(292, 27);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(94, 16);
            this.label3.TabIndex = 204;
            this.label3.Text = "مسمى التوليفة";
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.Color.WhiteSmoke;
            this.groupBox2.Controls.Add(this.dgv_Itemspro);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.groupBox2.Location = new System.Drawing.Point(0, 98);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(395, 227);
            this.groupBox2.TabIndex = 182;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "الاصناف";
            // 
            // dgv_Itemspro
            // 
            this.dgv_Itemspro.AllowUserToAddRows = false;
            this.dgv_Itemspro.AllowUserToDeleteRows = false;
            this.dgv_Itemspro.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgv_Itemspro.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_Itemspro.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ExpensID,
            this.ItemName,
            this.Quantity});
            this.dgv_Itemspro.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgv_Itemspro.Location = new System.Drawing.Point(3, 19);
            this.dgv_Itemspro.Margin = new System.Windows.Forms.Padding(3, 22, 3, 22);
            this.dgv_Itemspro.Name = "dgv_Itemspro";
            this.dgv_Itemspro.ReadOnly = true;
            this.dgv_Itemspro.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.dgv_Itemspro.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgv_Itemspro.RowTemplate.Height = 30;
            this.dgv_Itemspro.ShowEditingIcon = false;
            this.dgv_Itemspro.Size = new System.Drawing.Size(389, 205);
            this.dgv_Itemspro.TabIndex = 0;
            // 
            // ExpensID
            // 
            this.ExpensID.DataPropertyName = "Id";
            this.ExpensID.HeaderText = "Id";
            this.ExpensID.Name = "ExpensID";
            this.ExpensID.ReadOnly = true;
            this.ExpensID.Visible = false;
            // 
            // ItemName
            // 
            this.ItemName.DataPropertyName = "ItemName";
            this.ItemName.HeaderText = "الصنف";
            this.ItemName.Name = "ItemName";
            this.ItemName.ReadOnly = true;
            // 
            // Quantity
            // 
            this.Quantity.DataPropertyName = "Quantity";
            this.Quantity.HeaderText = "الكمية";
            this.Quantity.Name = "Quantity";
            this.Quantity.ReadOnly = true;
            // 
            // ItemProdDetails
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(395, 325);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "ItemProdDetails";
            this.Text = "اصناف توليفة";
            this.Load += new System.EventHandler(this.ItemProdDetails_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgv_Itemspro)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.DataGridView dgv_Itemspro;
        private System.Windows.Forms.GroupBox groupBox1;
        private ERP.Desktop.Utilities.ReusableControls.DangerButton btn_Cancel;
        private System.Windows.Forms.TextBox txt_ItemProName;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DataGridViewTextBoxColumn ExpensID;
        private System.Windows.Forms.DataGridViewTextBoxColumn ItemName;
        private System.Windows.Forms.DataGridViewTextBoxColumn Quantity;
        private System.Windows.Forms.TextBox txt_customerName;
        private System.Windows.Forms.Label label1;
    }
}