﻿
namespace ERP.Desktop.Views.Transactions.Pricing
{
    partial class AllPriceInvoices
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.dgvAllinvoices = new System.Windows.Forms.DataGridView();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.DGV_Details = new System.Windows.Forms.DataGridView();
            this.ItemName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Final_Price = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.txtSearchText = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.Invo_ID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.InvoiceNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Date = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTotal = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSafy = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.show = new System.Windows.Forms.DataGridViewButtonColumn();
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAllinvoices)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DGV_Details)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 63.75909F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 36.24091F));
            this.tableLayoutPanel1.Controls.Add(this.groupBox1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.groupBox2, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.groupBox3, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(963, 530);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.dgvAllinvoices);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(352, 53);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(608, 474);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "فواتير مبيعات الوردية";
            // 
            // dgvAllinvoices
            // 
            this.dgvAllinvoices.AllowUserToAddRows = false;
            this.dgvAllinvoices.AllowUserToDeleteRows = false;
            this.dgvAllinvoices.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells;
            this.dgvAllinvoices.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvAllinvoices.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Invo_ID,
            this.InvoiceNumber,
            this.Date,
            this.colTotal,
            this.colSafy,
            this.show});
            this.dgvAllinvoices.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvAllinvoices.Location = new System.Drawing.Point(3, 19);
            this.dgvAllinvoices.Margin = new System.Windows.Forms.Padding(4);
            this.dgvAllinvoices.Name = "dgvAllinvoices";
            this.dgvAllinvoices.ReadOnly = true;
            this.dgvAllinvoices.Size = new System.Drawing.Size(602, 452);
            this.dgvAllinvoices.TabIndex = 7;
            this.dgvAllinvoices.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvAllinvoices_CellClick);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.DGV_Details);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(3, 53);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(343, 474);
            this.groupBox2.TabIndex = 10;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "اصناف الفاتورة";
            // 
            // DGV_Details
            // 
            this.DGV_Details.AllowUserToAddRows = false;
            this.DGV_Details.AllowUserToDeleteRows = false;
            this.DGV_Details.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells;
            this.DGV_Details.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DGV_Details.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ItemName,
            this.dataGridViewTextBoxColumn4,
            this.Final_Price});
            this.DGV_Details.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DGV_Details.Location = new System.Drawing.Point(3, 19);
            this.DGV_Details.Margin = new System.Windows.Forms.Padding(4);
            this.DGV_Details.Name = "DGV_Details";
            this.DGV_Details.ReadOnly = true;
            this.DGV_Details.Size = new System.Drawing.Size(337, 452);
            this.DGV_Details.TabIndex = 8;
            // 
            // ItemName
            // 
            this.ItemName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.ItemName.DataPropertyName = "ItemName";
            this.ItemName.HeaderText = "اسم الصنف";
            this.ItemName.MinimumWidth = 150;
            this.ItemName.Name = "ItemName";
            this.ItemName.ReadOnly = true;
            this.ItemName.Width = 150;
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dataGridViewTextBoxColumn4.DataPropertyName = "Quantity";
            this.dataGridViewTextBoxColumn4.HeaderText = "الكمية";
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            this.dataGridViewTextBoxColumn4.ReadOnly = true;
            this.dataGridViewTextBoxColumn4.Width = 66;
            // 
            // Final_Price
            // 
            this.Final_Price.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Final_Price.DataPropertyName = "Amount";
            this.Final_Price.HeaderText = "الصافى";
            this.Final_Price.Name = "Final_Price";
            this.Final_Price.ReadOnly = true;
            this.Final_Price.Width = 71;
            // 
            // groupBox3
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.groupBox3, 2);
            this.groupBox3.Controls.Add(this.txtSearchText);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox3.Location = new System.Drawing.Point(3, 3);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(957, 44);
            this.groupBox3.TabIndex = 11;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "معايير البحث";
            // 
            // txtSearchText
            // 
            this.txtSearchText.Location = new System.Drawing.Point(651, 16);
            this.txtSearchText.Name = "txtSearchText";
            this.txtSearchText.Size = new System.Drawing.Size(238, 23);
            this.txtSearchText.TabIndex = 1;
            this.txtSearchText.TextChanged += new System.EventHandler(this.txtSearchText_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(895, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(52, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "بحث بـ :";
            // 
            // Invo_ID
            // 
            this.Invo_ID.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Invo_ID.DataPropertyName = "Id";
            this.Invo_ID.HeaderText = "Id";
            this.Invo_ID.Name = "Invo_ID";
            this.Invo_ID.ReadOnly = true;
            this.Invo_ID.Visible = false;
            this.Invo_ID.Width = 43;
            // 
            // InvoiceNumber
            // 
            this.InvoiceNumber.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.InvoiceNumber.DataPropertyName = "InvoiceNumber";
            this.InvoiceNumber.HeaderText = "رقم الفاتورة";
            this.InvoiceNumber.Name = "InvoiceNumber";
            this.InvoiceNumber.ReadOnly = true;
            this.InvoiceNumber.Width = 94;
            // 
            // Date
            // 
            this.Date.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Date.DataPropertyName = "Date";
            this.Date.HeaderText = "تاريخ الفاتورة";
            this.Date.Name = "Date";
            this.Date.ReadOnly = true;
            // 
            // colTotal
            // 
            this.colTotal.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.colTotal.DataPropertyName = "Total";
            this.colTotal.HeaderText = "اجمالي الفاتورة";
            this.colTotal.Name = "colTotal";
            this.colTotal.ReadOnly = true;
            this.colTotal.Width = 116;
            // 
            // colSafy
            // 
            this.colSafy.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.colSafy.DataPropertyName = "Safy";
            this.colSafy.HeaderText = "صافي الفاتورة";
            this.colSafy.Name = "colSafy";
            this.colSafy.ReadOnly = true;
            this.colSafy.Width = 107;
            // 
            // show
            // 
            this.show.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.show.HeaderText = "عرض";
            this.show.Name = "show";
            this.show.ReadOnly = true;
            this.show.Text = "عرض";
            this.show.UseColumnTextForButtonValue = true;
            this.show.Width = 41;
            // 
            // AllPriceInvoices
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(963, 530);
            this.Controls.Add(this.tableLayoutPanel1);
            this.MinimizeBox = false;
            this.Name = "AllPriceInvoices";
            this.ShowIcon = false;
            this.Text = "فواتير عروض اسعار الوردية";
            this.Load += new System.EventHandler(this.AllInvoices_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvAllinvoices)).EndInit();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.DGV_Details)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        public System.Windows.Forms.DataGridView DGV_Details;
        public System.Windows.Forms.DataGridView dgvAllinvoices;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.DataGridViewTextBoxColumn ItemName;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private System.Windows.Forms.DataGridViewTextBoxColumn Final_Price;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtSearchText;
        private System.Windows.Forms.DataGridViewTextBoxColumn Invo_ID;
        private System.Windows.Forms.DataGridViewTextBoxColumn InvoiceNumber;
        private System.Windows.Forms.DataGridViewTextBoxColumn Date;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTotal;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSafy;
        private System.Windows.Forms.DataGridViewButtonColumn show;
    }
}