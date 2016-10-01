namespace YalProcessKiller
{
    partial class YalProcessKillerUC
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
            this.cbWarnBeforeKill = new System.Windows.Forms.CheckBox();
            this.cbGetProcessIcons = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // cbWarnBeforeKill
            // 
            this.cbWarnBeforeKill.AutoSize = true;
            this.cbWarnBeforeKill.Dock = System.Windows.Forms.DockStyle.Top;
            this.cbWarnBeforeKill.Location = new System.Drawing.Point(0, 0);
            this.cbWarnBeforeKill.Name = "cbWarnBeforeKill";
            this.cbWarnBeforeKill.Size = new System.Drawing.Size(246, 17);
            this.cbWarnBeforeKill.TabIndex = 0;
            this.cbWarnBeforeKill.Text = "Warn me before killing any process";
            this.cbWarnBeforeKill.UseVisualStyleBackColor = true;
            // 
            // cbGetProcessIcons
            // 
            this.cbGetProcessIcons.AutoSize = true;
            this.cbGetProcessIcons.Dock = System.Windows.Forms.DockStyle.Top;
            this.cbGetProcessIcons.Location = new System.Drawing.Point(0, 17);
            this.cbGetProcessIcons.Name = "cbGetProcessIcons";
            this.cbGetProcessIcons.Size = new System.Drawing.Size(246, 17);
            this.cbGetProcessIcons.TabIndex = 1;
            this.cbGetProcessIcons.Text = "Display process icons when available (slower)";
            this.cbGetProcessIcons.UseVisualStyleBackColor = true;
            // 
            // YalProcessKillerUC
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cbGetProcessIcons);
            this.Controls.Add(this.cbWarnBeforeKill);
            this.Name = "YalProcessKillerUC";
            this.Size = new System.Drawing.Size(246, 174);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox cbWarnBeforeKill;
        private System.Windows.Forms.CheckBox cbGetProcessIcons;
    }
}
