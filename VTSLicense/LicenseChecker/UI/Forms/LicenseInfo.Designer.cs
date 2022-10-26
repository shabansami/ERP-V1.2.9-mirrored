using VTSLicense.LicenseChecker.UI.UserControls;

namespace VTSLicense.LicenseChecker.UI.Forms
{
    partial class LicenseInfo
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LicenseInfo));
            this.licenseInfoControl = new LicenseInfoControl();
            this.SuspendLayout();
            // 
            // licenseInfoControl
            // 
            this.licenseInfoControl.AutoSize = true;
            this.licenseInfoControl.DateFormat = null;
            this.licenseInfoControl.DateTimeFormat = null;
            this.licenseInfoControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.licenseInfoControl.Location = new System.Drawing.Point(0, 0);
            this.licenseInfoControl.Name = "licenseInfoControl";
            this.licenseInfoControl.Size = new System.Drawing.Size(372, 375);
            this.licenseInfoControl.TabIndex = 0;
            // 
            // LicenseInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(372, 375);
            this.Controls.Add(this.licenseInfoControl);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LicenseInfo";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "LicenseInfo";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public LicenseInfoControl licenseInfoControl;
    }
}