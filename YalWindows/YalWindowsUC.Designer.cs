namespace YalWindows
{
    partial class YalWindowsUC
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.cbGetAppIcons = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // cbGetAppIcons
            // 
            this.cbGetAppIcons.AutoSize = true;
            this.cbGetAppIcons.Dock = System.Windows.Forms.DockStyle.Top;
            this.cbGetAppIcons.Location = new System.Drawing.Point(0, 0);
            this.cbGetAppIcons.Name = "cbGetAppIcons";
            this.cbGetAppIcons.Size = new System.Drawing.Size(299, 17);
            this.cbGetAppIcons.TabIndex = 0;
            this.cbGetAppIcons.Text = "Display application icons when available (slower)";
            this.cbGetAppIcons.UseVisualStyleBackColor = true;
            // 
            // YalWindowsUC
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cbGetAppIcons);
            this.Name = "YalWindowsUC";
            this.Size = new System.Drawing.Size(299, 151);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox cbGetAppIcons;
    }
}
