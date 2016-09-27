namespace YalClipboardHistory
{
    partial class YalClipboardHistoryUC
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
            this.spinMaxHistorySize = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.cbStoreInDb = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.spinMaxHistorySize)).BeginInit();
            this.SuspendLayout();
            // 
            // spinMaxHistorySize
            // 
            this.spinMaxHistorySize.Location = new System.Drawing.Point(192, 17);
            this.spinMaxHistorySize.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.spinMaxHistorySize.Name = "spinMaxHistorySize";
            this.spinMaxHistorySize.Size = new System.Drawing.Size(48, 20);
            this.spinMaxHistorySize.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(0, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(187, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Max number of items to keep in history";
            // 
            // cbStoreInDb
            // 
            this.cbStoreInDb.AutoSize = true;
            this.cbStoreInDb.Dock = System.Windows.Forms.DockStyle.Top;
            this.cbStoreInDb.Location = new System.Drawing.Point(0, 0);
            this.cbStoreInDb.Name = "cbStoreInDb";
            this.cbStoreInDb.Size = new System.Drawing.Size(354, 17);
            this.cbStoreInDb.TabIndex = 2;
            this.cbStoreInDb.Text = "Remember history between sessions";
            this.cbStoreInDb.UseVisualStyleBackColor = true;
            // 
            // YalClipboardHistoryUC
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cbStoreInDb);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.spinMaxHistorySize);
            this.Name = "YalClipboardHistoryUC";
            this.Size = new System.Drawing.Size(354, 150);
            ((System.ComponentModel.ISupportInitialize)(this.spinMaxHistorySize)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NumericUpDown spinMaxHistorySize;
        private System.Windows.Forms.Label label1;
        internal System.Windows.Forms.CheckBox cbStoreInDb;
    }
}
