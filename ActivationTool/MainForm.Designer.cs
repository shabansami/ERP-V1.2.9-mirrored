
using VTSLicense.LicenseChecker.UI.UserControls;

namespace ActivationTool
{
    partial class MainForm
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
            this.licenseSettingsControl1 = new LicenseSettingsControl();
            this.licenseStringContainer1 = new LicenseStringContainer();
            this.SuspendLayout();
            // 
            // licenseSettingsControl1
            // 
            this.licenseSettingsControl1.AllowVolumeLicense = true;
            this.licenseSettingsControl1.Dock = System.Windows.Forms.DockStyle.Left;
            this.licenseSettingsControl1.Location = new System.Drawing.Point(0, 0);
            this.licenseSettingsControl1.Name = "licenseSettingsControl1";
            this.licenseSettingsControl1.Size = new System.Drawing.Size(474, 538);
            this.licenseSettingsControl1.TabIndex = 0;
            this.licenseSettingsControl1.OnLicenseGenerated += new LicenseGeneratedHandler(this.licSettings_OnLicenseGenerated);
            // 
            // licenseStringContainer1
            // 
            this.licenseStringContainer1.Dock = System.Windows.Forms.DockStyle.Right;
            this.licenseStringContainer1.LicenseString = "";
            this.licenseStringContainer1.Location = new System.Drawing.Point(482, 0);
            this.licenseStringContainer1.Name = "licenseStringContainer1";
            this.licenseStringContainer1.Size = new System.Drawing.Size(348, 538);
            this.licenseStringContainer1.TabIndex = 1;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(830, 538);
            this.Controls.Add(this.licenseStringContainer1);
            this.Controls.Add(this.licenseSettingsControl1);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Activation Tool";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private LicenseSettingsControl licenseSettingsControl1;
        private LicenseStringContainer licenseStringContainer1;
    }
}

