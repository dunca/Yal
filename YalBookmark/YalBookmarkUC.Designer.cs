namespace YalBookmark
{
    partial class YalBookmarkUC
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
            this.numericUpDownTruncate = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.cbOpenWithProvider = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTruncate)).BeginInit();
            this.SuspendLayout();
            // 
            // numericUpDownTruncate
            // 
            this.numericUpDownTruncate.Location = new System.Drawing.Point(258, 4);
            this.numericUpDownTruncate.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.numericUpDownTruncate.Name = "numericUpDownTruncate";
            this.numericUpDownTruncate.Size = new System.Drawing.Size(46, 20);
            this.numericUpDownTruncate.TabIndex = 0;
            this.numericUpDownTruncate.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(249, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Truncate bookmark names longer than (characters)";
            // 
            // cbOpenWithProvider
            // 
            this.cbOpenWithProvider.AutoSize = true;
            this.cbOpenWithProvider.Enabled = false;
            this.cbOpenWithProvider.Location = new System.Drawing.Point(3, 29);
            this.cbOpenWithProvider.Name = "cbOpenWithProvider";
            this.cbOpenWithProvider.Size = new System.Drawing.Size(248, 17);
            this.cbOpenWithProvider.TabIndex = 2;
            this.cbOpenWithProvider.Text = "Visit the bookmark via the browser that stores it";
            this.cbOpenWithProvider.UseVisualStyleBackColor = true;
            // 
            // YalBookmarkUC
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cbOpenWithProvider);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.numericUpDownTruncate);
            this.Name = "YalBookmarkUC";
            this.Size = new System.Drawing.Size(307, 182);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTruncate)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NumericUpDown numericUpDownTruncate;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox cbOpenWithProvider;
    }
}
