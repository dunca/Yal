namespace YalPath
{
    partial class YalPathUC
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
            this.cbCopyPath = new System.Windows.Forms.CheckBox();
            this.cbOpenPath = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // cbCopyPath
            // 
            this.cbCopyPath.AutoSize = true;
            this.cbCopyPath.Location = new System.Drawing.Point(6, 19);
            this.cbCopyPath.Name = "cbCopyPath";
            this.cbCopyPath.Size = new System.Drawing.Size(147, 17);
            this.cbCopyPath.TabIndex = 0;
            this.cbCopyPath.Text = "Copy it\'s path to clipboard";
            this.cbCopyPath.UseVisualStyleBackColor = true;
            // 
            // cbOpenPath
            // 
            this.cbOpenPath.AutoSize = true;
            this.cbOpenPath.Location = new System.Drawing.Point(6, 42);
            this.cbOpenPath.Name = "cbOpenPath";
            this.cbOpenPath.Size = new System.Drawing.Size(231, 17);
            this.cbOpenPath.TabIndex = 1;
            this.cbOpenPath.Text = "Open file/directory in the default application";
            this.cbOpenPath.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cbOpenPath);
            this.groupBox1.Controls.Add(this.cbCopyPath);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(273, 65);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "When the Enter key is pressed on a selected item";
            // 
            // YalPathUC
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Name = "YalPathUC";
            this.Size = new System.Drawing.Size(273, 212);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckBox cbCopyPath;
        private System.Windows.Forms.CheckBox cbOpenPath;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}
