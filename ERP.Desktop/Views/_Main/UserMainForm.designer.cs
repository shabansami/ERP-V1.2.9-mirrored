
namespace ERP.Desktop.Views._Base.Main
{
    partial class UserMainForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UserMainForm));
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblPersonName = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusSpace1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel3 = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblUserRole = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusSpace2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.معلوماتالمركزالتعليميToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuItemShifts = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuItemSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.ادارهانواعالمصروفاتوالايراداتToolStripMenuItem = new FontAwesome.Sharp.IconMenuItem();
            this.ادارةنوعمصرفToolStripMenuItem = new FontAwesome.Sharp.IconMenuItem();
            this.ادارةنوعايرادToolStripMenuItem = new FontAwesome.Sharp.IconMenuItem();
            this.btnSystemSettings = new FontAwesome.Sharp.IconMenuItem();
            this.menussss = new System.Windows.Forms.ToolStripMenuItem();
            this.btnCustomers = new System.Windows.Forms.ToolStripMenuItem();
            this.btnSuppliers = new System.Windows.Forms.ToolStripMenuItem();
            this.التقاريرToolStripMenuItem = new FontAwesome.Sharp.IconMenuItem();
            this.btnItemsBalance = new FontAwesome.Sharp.IconMenuItem();
            this.btnSafeAccountings = new FontAwesome.Sharp.IconMenuItem();
            this.iconMenuItem5 = new FontAwesome.Sharp.IconMenuItem();
            this.iconMenuItem6 = new FontAwesome.Sharp.IconMenuItem();
            this.MenuItemLogout = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.iconMenuItem3 = new FontAwesome.Sharp.IconMenuItem();
            this.btnAddItem = new FontAwesome.Sharp.IconMenuItem();
            this.iconMenuItem4 = new FontAwesome.Sharp.IconMenuItem();
            this.btnItemOffers = new FontAwesome.Sharp.IconMenuItem();
            this.btnPrintBarcode = new FontAwesome.Sharp.IconMenuItem();
            this.btnItemSearchWithPrice = new FontAwesome.Sharp.IconMenuItem();
            this.iconMenuItem2 = new FontAwesome.Sharp.IconMenuItem();
            this.btnCustomersPayments = new FontAwesome.Sharp.IconMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.btnIncomes = new FontAwesome.Sharp.IconMenuItem();
            this.btnExpenses = new FontAwesome.Sharp.IconMenuItem();
            this.btnEmployeeLoan = new FontAwesome.Sharp.IconMenuItem();
            this.iconMenuItem1 = new FontAwesome.Sharp.IconMenuItem();
            this.btnSales = new FontAwesome.Sharp.IconMenuItem();
            this.btnSalesBack = new FontAwesome.Sharp.IconMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnPurchase = new FontAwesome.Sharp.IconMenuItem();
            this.btnPurchaseBack = new FontAwesome.Sharp.IconMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.btnPriceInvoices = new FontAwesome.Sharp.IconMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.btnInventoryItems = new FontAwesome.Sharp.IconMenuItem();
            this.btnNewInventoryInvoice = new FontAwesome.Sharp.IconMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.statusStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.BackColor = System.Drawing.Color.AliceBlue;
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.lblPersonName,
            this.toolStripStatusSpace1,
            this.toolStripStatusLabel3,
            this.lblUserRole,
            this.toolStripStatusSpace2});
            this.statusStrip1.Location = new System.Drawing.Point(0, 579);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(12, 0, 1, 0);
            this.statusStrip1.Size = new System.Drawing.Size(1019, 22);
            this.statusStrip1.TabIndex = 4;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.ForeColor = System.Drawing.Color.Black;
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(139, 17);
            this.toolStripStatusLabel1.Text = "تم تسجيل الدخول بواسطة: ";
            // 
            // lblPersonName
            // 
            this.lblPersonName.ForeColor = System.Drawing.Color.Black;
            this.lblPersonName.Name = "lblPersonName";
            this.lblPersonName.Size = new System.Drawing.Size(0, 17);
            // 
            // toolStripStatusSpace1
            // 
            this.toolStripStatusSpace1.ForeColor = System.Drawing.Color.Black;
            this.toolStripStatusSpace1.Name = "toolStripStatusSpace1";
            this.toolStripStatusSpace1.Size = new System.Drawing.Size(100, 17);
            this.toolStripStatusSpace1.Text = "                               ";
            // 
            // toolStripStatusLabel3
            // 
            this.toolStripStatusLabel3.ForeColor = System.Drawing.Color.Black;
            this.toolStripStatusLabel3.Name = "toolStripStatusLabel3";
            this.toolStripStatusLabel3.Size = new System.Drawing.Size(100, 17);
            this.toolStripStatusLabel3.Text = "صلاحية المستخدم: ";
            // 
            // lblUserRole
            // 
            this.lblUserRole.ForeColor = System.Drawing.Color.Black;
            this.lblUserRole.Name = "lblUserRole";
            this.lblUserRole.Size = new System.Drawing.Size(0, 17);
            // 
            // toolStripStatusSpace2
            // 
            this.toolStripStatusSpace2.ForeColor = System.Drawing.Color.Black;
            this.toolStripStatusSpace2.Name = "toolStripStatusSpace2";
            this.toolStripStatusSpace2.Size = new System.Drawing.Size(100, 17);
            this.toolStripStatusSpace2.Text = "                               ";
            // 
            // معلوماتالمركزالتعليميToolStripMenuItem
            // 
            this.معلوماتالمركزالتعليميToolStripMenuItem.BackColor = System.Drawing.Color.BlueViolet;
            this.معلوماتالمركزالتعليميToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuItemShifts,
            this.MenuItemSettings,
            this.ادارهانواعالمصروفاتوالايراداتToolStripMenuItem,
            this.btnSystemSettings});
            this.معلوماتالمركزالتعليميToolStripMenuItem.ForeColor = System.Drawing.SystemColors.ControlText;
            this.معلوماتالمركزالتعليميToolStripMenuItem.Image = global::ERP.Desktop.Properties.Resources.company_24px;
            this.معلوماتالمركزالتعليميToolStripMenuItem.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.معلوماتالمركزالتعليميToolStripMenuItem.Name = "معلوماتالمركزالتعليميToolStripMenuItem";
            this.معلوماتالمركزالتعليميToolStripMenuItem.Size = new System.Drawing.Size(117, 20);
            this.معلوماتالمركزالتعليميToolStripMenuItem.Text = "البيانات الاساسية";
            this.معلوماتالمركزالتعليميToolStripMenuItem.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // MenuItemShifts
            // 
            this.MenuItemShifts.BackColor = System.Drawing.Color.Teal;
            this.MenuItemShifts.ForeColor = System.Drawing.SystemColors.ControlText;
            this.MenuItemShifts.Image = global::ERP.Desktop.Properties.Resources.classroom_24px;
            this.MenuItemShifts.Name = "MenuItemShifts";
            this.MenuItemShifts.Size = new System.Drawing.Size(204, 22);
            this.MenuItemShifts.Tag = "Rooms";
            this.MenuItemShifts.Text = "الورديات";
            this.MenuItemShifts.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.MenuItemShifts.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.MenuItemShifts.Click += new System.EventHandler(this.MenuItemShifts_Click);
            // 
            // MenuItemSettings
            // 
            this.MenuItemSettings.BackColor = System.Drawing.Color.Teal;
            this.MenuItemSettings.Image = global::ERP.Desktop.Properties.Resources.franchise_24px;
            this.MenuItemSettings.Name = "MenuItemSettings";
            this.MenuItemSettings.Size = new System.Drawing.Size(204, 22);
            this.MenuItemSettings.Tag = "Settings";
            this.MenuItemSettings.Text = "نقاط البيع";
            this.MenuItemSettings.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.MenuItemSettings.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.MenuItemSettings.Click += new System.EventHandler(this.MenuItemSettings_Click);
            // 
            // ادارهانواعالمصروفاتوالايراداتToolStripMenuItem
            // 
            this.ادارهانواعالمصروفاتوالايراداتToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ادارةنوعمصرفToolStripMenuItem,
            this.ادارةنوعايرادToolStripMenuItem});
            this.ادارهانواعالمصروفاتوالايراداتToolStripMenuItem.IconChar = FontAwesome.Sharp.IconChar.MoneyBill;
            this.ادارهانواعالمصروفاتوالايراداتToolStripMenuItem.IconColor = System.Drawing.Color.Turquoise;
            this.ادارهانواعالمصروفاتوالايراداتToolStripMenuItem.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.ادارهانواعالمصروفاتوالايراداتToolStripMenuItem.Name = "ادارهانواعالمصروفاتوالايراداتToolStripMenuItem";
            this.ادارهانواعالمصروفاتوالايراداتToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
            this.ادارهانواعالمصروفاتوالايراداتToolStripMenuItem.Text = "انواع المصروفات والايرادات";
            // 
            // ادارةنوعمصرفToolStripMenuItem
            // 
            this.ادارةنوعمصرفToolStripMenuItem.IconChar = FontAwesome.Sharp.IconChar.MoneyCheck;
            this.ادارةنوعمصرفToolStripMenuItem.IconColor = System.Drawing.Color.Turquoise;
            this.ادارةنوعمصرفToolStripMenuItem.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.ادارةنوعمصرفToolStripMenuItem.Name = "ادارةنوعمصرفToolStripMenuItem";
            this.ادارةنوعمصرفToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.ادارةنوعمصرفToolStripMenuItem.Text = "ادارة نوع مصرف";
            this.ادارةنوعمصرفToolStripMenuItem.Click += new System.EventHandler(this.ادارةنوعمصرفToolStripMenuItem_Click);
            // 
            // ادارةنوعايرادToolStripMenuItem
            // 
            this.ادارةنوعايرادToolStripMenuItem.IconChar = FontAwesome.Sharp.IconChar.MoneyCheckAlt;
            this.ادارةنوعايرادToolStripMenuItem.IconColor = System.Drawing.Color.Turquoise;
            this.ادارةنوعايرادToolStripMenuItem.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.ادارةنوعايرادToolStripMenuItem.Name = "ادارةنوعايرادToolStripMenuItem";
            this.ادارةنوعايرادToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.ادارةنوعايرادToolStripMenuItem.Text = "ادارة نوع ايراد";
            this.ادارةنوعايرادToolStripMenuItem.Click += new System.EventHandler(this.ادارةنوعايرادToolStripMenuItem_Click);
            // 
            // btnSystemSettings
            // 
            this.btnSystemSettings.IconChar = FontAwesome.Sharp.IconChar.Cogs;
            this.btnSystemSettings.IconColor = System.Drawing.Color.DarkTurquoise;
            this.btnSystemSettings.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnSystemSettings.IconSize = 60;
            this.btnSystemSettings.Name = "btnSystemSettings";
            this.btnSystemSettings.Size = new System.Drawing.Size(204, 22);
            this.btnSystemSettings.Text = "إعدادات النظام";
            this.btnSystemSettings.Click += new System.EventHandler(this.btnSystemSettings_Click);
            // 
            // menussss
            // 
            this.menussss.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnCustomers,
            this.btnSuppliers});
            this.menussss.ForeColor = System.Drawing.SystemColors.ControlText;
            this.menussss.Image = global::ERP.Desktop.Properties.Resources.schoolboy_at_a_desk_24px;
            this.menussss.Name = "menussss";
            this.menussss.Size = new System.Drawing.Size(119, 20);
            this.menussss.Text = "العملاء والموردين";
            // 
            // btnCustomers
            // 
            this.btnCustomers.Image = global::ERP.Desktop.Properties.Resources.schoolboy_at_a_desk_24px;
            this.btnCustomers.Name = "btnCustomers";
            this.btnCustomers.Size = new System.Drawing.Size(141, 22);
            this.btnCustomers.Tag = "Instructors";
            this.btnCustomers.Text = "ادارة العملاء";
            this.btnCustomers.Click += new System.EventHandler(this.btnCustomers_Click);
            // 
            // btnSuppliers
            // 
            this.btnSuppliers.Image = global::ERP.Desktop.Properties.Resources.schoolboy_at_a_desk_24px;
            this.btnSuppliers.Name = "btnSuppliers";
            this.btnSuppliers.Size = new System.Drawing.Size(141, 22);
            this.btnSuppliers.Tag = "Instructors";
            this.btnSuppliers.Text = "ادارة الموردين";
            this.btnSuppliers.Click += new System.EventHandler(this.btnSuppliers_Click);
            // 
            // التقاريرToolStripMenuItem
            // 
            this.التقاريرToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnItemsBalance,
            this.btnSafeAccountings,
            this.iconMenuItem5,
            this.iconMenuItem6});
            this.التقاريرToolStripMenuItem.IconChar = FontAwesome.Sharp.IconChar.ListAlt;
            this.التقاريرToolStripMenuItem.IconColor = System.Drawing.Color.Turquoise;
            this.التقاريرToolStripMenuItem.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.التقاريرToolStripMenuItem.Name = "التقاريرToolStripMenuItem";
            this.التقاريرToolStripMenuItem.Size = new System.Drawing.Size(70, 20);
            this.التقاريرToolStripMenuItem.Text = "التقارير";
            // 
            // btnItemsBalance
            // 
            this.btnItemsBalance.IconChar = FontAwesome.Sharp.IconChar.List;
            this.btnItemsBalance.IconColor = System.Drawing.Color.Turquoise;
            this.btnItemsBalance.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnItemsBalance.Name = "btnItemsBalance";
            this.btnItemsBalance.Size = new System.Drawing.Size(194, 22);
            this.btnItemsBalance.Text = "تقرير ارصدة الاصناف";
            this.btnItemsBalance.Click += new System.EventHandler(this.btnItemsBalance_Click);
            // 
            // btnSafeAccountings
            // 
            this.btnSafeAccountings.IconChar = FontAwesome.Sharp.IconChar.BalanceScale;
            this.btnSafeAccountings.IconColor = System.Drawing.Color.Turquoise;
            this.btnSafeAccountings.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnSafeAccountings.Name = "btnSafeAccountings";
            this.btnSafeAccountings.Size = new System.Drawing.Size(194, 22);
            this.btnSafeAccountings.Text = "تقرير الحسابات المالية";
            this.btnSafeAccountings.Click += new System.EventHandler(this.btnSafeAccountings_Click);
            // 
            // iconMenuItem5
            // 
            this.iconMenuItem5.IconChar = FontAwesome.Sharp.IconChar.BalanceScale;
            this.iconMenuItem5.IconColor = System.Drawing.Color.Turquoise;
            this.iconMenuItem5.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.iconMenuItem5.Name = "iconMenuItem5";
            this.iconMenuItem5.Size = new System.Drawing.Size(194, 22);
            this.iconMenuItem5.Text = "كميات اصناف فواتير بيع ";
            this.iconMenuItem5.Click += new System.EventHandler(this.iconMenuItem5_Click);
            // 
            // iconMenuItem6
            // 
            this.iconMenuItem6.IconChar = FontAwesome.Sharp.IconChar.BalanceScale;
            this.iconMenuItem6.IconColor = System.Drawing.Color.Turquoise;
            this.iconMenuItem6.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.iconMenuItem6.Name = "iconMenuItem6";
            this.iconMenuItem6.Size = new System.Drawing.Size(194, 22);
            this.iconMenuItem6.Text = "تقرير الورديات المغلقة";
            this.iconMenuItem6.Click += new System.EventHandler(this.iconMenuItem6_Click);
            // 
            // MenuItemLogout
            // 
            this.MenuItemLogout.ForeColor = System.Drawing.SystemColors.ControlText;
            this.MenuItemLogout.Image = global::ERP.Desktop.Properties.Resources.shutdown_24px;
            this.MenuItemLogout.Name = "MenuItemLogout";
            this.MenuItemLogout.Size = new System.Drawing.Size(103, 20);
            this.MenuItemLogout.Text = "تسجيل الخروج";
            this.MenuItemLogout.Click += new System.EventHandler(this.MenuItemLogout_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.Color.AliceBlue;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.معلوماتالمركزالتعليميToolStripMenuItem,
            this.iconMenuItem3,
            this.menussss,
            this.iconMenuItem2,
            this.iconMenuItem1,
            this.التقاريرToolStripMenuItem,
            this.toolStripMenuItem1,
            this.MenuItemLogout});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(5, 2, 0, 2);
            this.menuStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.menuStrip1.Size = new System.Drawing.Size(1019, 24);
            this.menuStrip1.TabIndex = 3;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // iconMenuItem3
            // 
            this.iconMenuItem3.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnAddItem,
            this.iconMenuItem4,
            this.btnItemOffers,
            this.btnPrintBarcode,
            this.btnItemSearchWithPrice});
            this.iconMenuItem3.IconChar = FontAwesome.Sharp.IconChar.ObjectGroup;
            this.iconMenuItem3.IconColor = System.Drawing.Color.Turquoise;
            this.iconMenuItem3.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.iconMenuItem3.Name = "iconMenuItem3";
            this.iconMenuItem3.Size = new System.Drawing.Size(76, 20);
            this.iconMenuItem3.Text = "الاصناف";
            // 
            // btnAddItem
            // 
            this.btnAddItem.IconChar = FontAwesome.Sharp.IconChar.PlusCircle;
            this.btnAddItem.IconColor = System.Drawing.Color.Turquoise;
            this.btnAddItem.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnAddItem.Name = "btnAddItem";
            this.btnAddItem.Size = new System.Drawing.Size(202, 22);
            this.btnAddItem.Text = "اضافة صنف جديد";
            this.btnAddItem.Click += new System.EventHandler(this.btnAddItem_Click);
            // 
            // iconMenuItem4
            // 
            this.iconMenuItem4.IconChar = FontAwesome.Sharp.IconChar.ObjectGroup;
            this.iconMenuItem4.IconColor = System.Drawing.Color.Turquoise;
            this.iconMenuItem4.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.iconMenuItem4.Name = "iconMenuItem4";
            this.iconMenuItem4.Size = new System.Drawing.Size(202, 22);
            this.iconMenuItem4.Text = "قائمة الاصناف";
            this.iconMenuItem4.Click += new System.EventHandler(this.iconMenuItem4_Click);
            // 
            // btnItemOffers
            // 
            this.btnItemOffers.IconChar = FontAwesome.Sharp.IconChar.MoneyCheck;
            this.btnItemOffers.IconColor = System.Drawing.Color.Turquoise;
            this.btnItemOffers.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnItemOffers.Name = "btnItemOffers";
            this.btnItemOffers.Size = new System.Drawing.Size(202, 22);
            this.btnItemOffers.Text = "عروض الاسعار";
            // 
            // btnPrintBarcode
            // 
            this.btnPrintBarcode.IconChar = FontAwesome.Sharp.IconChar.Barcode;
            this.btnPrintBarcode.IconColor = System.Drawing.Color.Turquoise;
            this.btnPrintBarcode.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnPrintBarcode.Name = "btnPrintBarcode";
            this.btnPrintBarcode.Size = new System.Drawing.Size(202, 22);
            this.btnPrintBarcode.Text = "طباعة الباركود";
            this.btnPrintBarcode.Click += new System.EventHandler(this.btnPrintBarcode_Click);
            // 
            // btnItemSearchWithPrice
            // 
            this.btnItemSearchWithPrice.IconChar = FontAwesome.Sharp.IconChar.MoneyCheck;
            this.btnItemSearchWithPrice.IconColor = System.Drawing.Color.Turquoise;
            this.btnItemSearchWithPrice.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnItemSearchWithPrice.Name = "btnItemSearchWithPrice";
            this.btnItemSearchWithPrice.Size = new System.Drawing.Size(202, 22);
            this.btnItemSearchWithPrice.Text = "بحث سياسات اسعار صنف";
            this.btnItemSearchWithPrice.Click += new System.EventHandler(this.btnItemSearchWithPrice_Click);
            // 
            // iconMenuItem2
            // 
            this.iconMenuItem2.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnCustomersPayments,
            this.toolStripSeparator2,
            this.btnIncomes,
            this.btnExpenses,
            this.btnEmployeeLoan});
            this.iconMenuItem2.IconChar = FontAwesome.Sharp.IconChar.MoneyBillWaveAlt;
            this.iconMenuItem2.IconColor = System.Drawing.Color.Turquoise;
            this.iconMenuItem2.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.iconMenuItem2.Name = "iconMenuItem2";
            this.iconMenuItem2.Size = new System.Drawing.Size(140, 20);
            this.iconMenuItem2.Text = "الايرادات والمصروفات";
            // 
            // btnCustomersPayments
            // 
            this.btnCustomersPayments.IconChar = FontAwesome.Sharp.IconChar.MoneyBillWave;
            this.btnCustomersPayments.IconColor = System.Drawing.Color.Turquoise;
            this.btnCustomersPayments.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnCustomersPayments.Name = "btnCustomersPayments";
            this.btnCustomersPayments.Size = new System.Drawing.Size(178, 22);
            this.btnCustomersPayments.Text = "دفعة عميل";
            this.btnCustomersPayments.Click += new System.EventHandler(this.btnCustomersPayments_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(175, 6);
            // 
            // btnIncomes
            // 
            this.btnIncomes.IconChar = FontAwesome.Sharp.IconChar.SignInAlt;
            this.btnIncomes.IconColor = System.Drawing.Color.Turquoise;
            this.btnIncomes.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnIncomes.Name = "btnIncomes";
            this.btnIncomes.Size = new System.Drawing.Size(178, 22);
            this.btnIncomes.Text = "تسجيل ايرادات";
            this.btnIncomes.Click += new System.EventHandler(this.btnIncomes_Click);
            // 
            // btnExpenses
            // 
            this.btnExpenses.IconChar = FontAwesome.Sharp.IconChar.SignOutAlt;
            this.btnExpenses.IconColor = System.Drawing.Color.Turquoise;
            this.btnExpenses.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnExpenses.Name = "btnExpenses";
            this.btnExpenses.Size = new System.Drawing.Size(178, 22);
            this.btnExpenses.Text = "تسجيل مصروفات";
            this.btnExpenses.Click += new System.EventHandler(this.btnExpenses_Click);
            // 
            // btnEmployeeLoan
            // 
            this.btnEmployeeLoan.IconChar = FontAwesome.Sharp.IconChar.SignOutAlt;
            this.btnEmployeeLoan.IconColor = System.Drawing.Color.Turquoise;
            this.btnEmployeeLoan.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnEmployeeLoan.Name = "btnEmployeeLoan";
            this.btnEmployeeLoan.Size = new System.Drawing.Size(178, 22);
            this.btnEmployeeLoan.Text = "تسجيل سلفة لموظف";
            this.btnEmployeeLoan.Click += new System.EventHandler(this.btnEmployeeLoan_Click);
            // 
            // iconMenuItem1
            // 
            this.iconMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnSales,
            this.btnSalesBack,
            this.toolStripSeparator1,
            this.btnPurchase,
            this.btnPurchaseBack,
            this.toolStripSeparator3,
            this.btnPriceInvoices,
            this.toolStripSeparator4,
            this.btnInventoryItems,
            this.btnNewInventoryInvoice});
            this.iconMenuItem1.IconChar = FontAwesome.Sharp.IconChar.ObjectUngroup;
            this.iconMenuItem1.IconColor = System.Drawing.Color.Turquoise;
            this.iconMenuItem1.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.iconMenuItem1.Name = "iconMenuItem1";
            this.iconMenuItem1.Size = new System.Drawing.Size(84, 20);
            this.iconMenuItem1.Text = "المعاملات";
            // 
            // btnSales
            // 
            this.btnSales.IconChar = FontAwesome.Sharp.IconChar.Shopify;
            this.btnSales.IconColor = System.Drawing.Color.Turquoise;
            this.btnSales.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnSales.Name = "btnSales";
            this.btnSales.Size = new System.Drawing.Size(155, 22);
            this.btnSales.Text = "المبيعات";
            this.btnSales.Click += new System.EventHandler(this.btnSales_Click);
            // 
            // btnSalesBack
            // 
            this.btnSalesBack.IconChar = FontAwesome.Sharp.IconChar.Redo;
            this.btnSalesBack.IconColor = System.Drawing.Color.Turquoise;
            this.btnSalesBack.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnSalesBack.Name = "btnSalesBack";
            this.btnSalesBack.Size = new System.Drawing.Size(155, 22);
            this.btnSalesBack.Text = "مرتجع المبيعات";
            this.btnSalesBack.Click += new System.EventHandler(this.btnSalesBack_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(152, 6);
            // 
            // btnPurchase
            // 
            this.btnPurchase.IconChar = FontAwesome.Sharp.IconChar.ShoppingCart;
            this.btnPurchase.IconColor = System.Drawing.Color.Turquoise;
            this.btnPurchase.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnPurchase.Name = "btnPurchase";
            this.btnPurchase.Size = new System.Drawing.Size(155, 22);
            this.btnPurchase.Text = "التوريد";
            this.btnPurchase.Click += new System.EventHandler(this.btnPurchase_Click);
            // 
            // btnPurchaseBack
            // 
            this.btnPurchaseBack.IconChar = FontAwesome.Sharp.IconChar.Recycle;
            this.btnPurchaseBack.IconColor = System.Drawing.Color.Turquoise;
            this.btnPurchaseBack.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnPurchaseBack.Name = "btnPurchaseBack";
            this.btnPurchaseBack.Size = new System.Drawing.Size(155, 22);
            this.btnPurchaseBack.Text = "مرتجع توريد";
            this.btnPurchaseBack.Click += new System.EventHandler(this.btnPurchaseBack_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(152, 6);
            // 
            // btnPriceInvoices
            // 
            this.btnPriceInvoices.IconChar = FontAwesome.Sharp.IconChar.MoneyCheck;
            this.btnPriceInvoices.IconColor = System.Drawing.Color.Turquoise;
            this.btnPriceInvoices.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnPriceInvoices.Name = "btnPriceInvoices";
            this.btnPriceInvoices.Size = new System.Drawing.Size(155, 22);
            this.btnPriceInvoices.Text = "عروض الاسعار";
            this.btnPriceInvoices.Click += new System.EventHandler(this.btnPriceInvoices_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(152, 6);
            // 
            // btnInventoryItems
            // 
            this.btnInventoryItems.IconChar = FontAwesome.Sharp.IconChar.ObjectUngroup;
            this.btnInventoryItems.IconColor = System.Drawing.Color.Turquoise;
            this.btnInventoryItems.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnInventoryItems.Name = "btnInventoryItems";
            this.btnInventoryItems.Size = new System.Drawing.Size(155, 22);
            this.btnInventoryItems.Text = "فواتير الجرد";
            this.btnInventoryItems.Click += new System.EventHandler(this.btnInventoryItems_Click);
            // 
            // btnNewInventoryInvoice
            // 
            this.btnNewInventoryInvoice.IconChar = FontAwesome.Sharp.IconChar.ObjectGroup;
            this.btnNewInventoryInvoice.IconColor = System.Drawing.Color.Turquoise;
            this.btnNewInventoryInvoice.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnNewInventoryInvoice.Name = "btnNewInventoryInvoice";
            this.btnNewInventoryInvoice.Size = new System.Drawing.Size(155, 22);
            this.btnNewInventoryInvoice.Text = "فاتورة جرد جديدة";
            this.btnNewInventoryInvoice.Click += new System.EventHandler(this.btnNewInventoryInvoice_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Enabled = false;
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(22, 20);
            this.toolStripMenuItem1.Text = "|";
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 60000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // UserMainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.ClientSize = new System.Drawing.Size(1019, 601);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(2)))), ((int)(((byte)(29)))), ((int)(((byte)(73)))));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.IsMdiContainer = true;
            this.Name = "UserMainForm";
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "VTS";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.UserMainForm_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel lblPersonName;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusSpace1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel3;
        private System.Windows.Forms.ToolStripStatusLabel lblUserRole;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusSpace2;
        private System.Windows.Forms.ToolStripMenuItem معلوماتالمركزالتعليميToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem MenuItemShifts;
        private System.Windows.Forms.ToolStripMenuItem MenuItemSettings;
        private System.Windows.Forms.ToolStripMenuItem menussss;
        private System.Windows.Forms.ToolStripMenuItem btnCustomers;
        private FontAwesome.Sharp.IconMenuItem التقاريرToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem MenuItemLogout;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private FontAwesome.Sharp.IconMenuItem ادارهانواعالمصروفاتوالايراداتToolStripMenuItem;
        private FontAwesome.Sharp.IconMenuItem ادارةنوعمصرفToolStripMenuItem;
        private FontAwesome.Sharp.IconMenuItem ادارةنوعايرادToolStripMenuItem;
        private FontAwesome.Sharp.IconMenuItem btnSystemSettings;
        private FontAwesome.Sharp.IconMenuItem iconMenuItem1;
        private FontAwesome.Sharp.IconMenuItem btnSales;
        private FontAwesome.Sharp.IconMenuItem btnSalesBack;
        private System.Windows.Forms.ToolStripMenuItem btnSuppliers;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private FontAwesome.Sharp.IconMenuItem btnPurchase;
        private FontAwesome.Sharp.IconMenuItem btnPurchaseBack;
        private FontAwesome.Sharp.IconMenuItem iconMenuItem3;
        private FontAwesome.Sharp.IconMenuItem btnAddItem;
        private FontAwesome.Sharp.IconMenuItem iconMenuItem4;
        private FontAwesome.Sharp.IconMenuItem btnItemSearchWithPrice;
        private FontAwesome.Sharp.IconMenuItem iconMenuItem2;
        private FontAwesome.Sharp.IconMenuItem btnCustomersPayments;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private FontAwesome.Sharp.IconMenuItem btnIncomes;
        private FontAwesome.Sharp.IconMenuItem btnExpenses;
        private FontAwesome.Sharp.IconMenuItem btnItemsBalance;
        private FontAwesome.Sharp.IconMenuItem btnSafeAccountings;
        private System.Windows.Forms.Timer timer1;
        private FontAwesome.Sharp.IconMenuItem btnPrintBarcode;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private FontAwesome.Sharp.IconMenuItem btnPriceInvoices;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private FontAwesome.Sharp.IconMenuItem btnInventoryItems;
        private FontAwesome.Sharp.IconMenuItem btnNewInventoryInvoice;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private FontAwesome.Sharp.IconMenuItem btnItemOffers;
        private FontAwesome.Sharp.IconMenuItem iconMenuItem5;
        private FontAwesome.Sharp.IconMenuItem iconMenuItem6;
        private FontAwesome.Sharp.IconMenuItem btnEmployeeLoan;
    }
}