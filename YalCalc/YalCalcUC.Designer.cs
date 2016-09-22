namespace YalCalc
{
    partial class YalCalcUC
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
            this.numericUpDownDecimalPlaces = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.cbReplaceClipboard = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownDecimalPlaces)).BeginInit();
            this.SuspendLayout();
            // 
            // numericUpDownDecimalPlaces
            // 
            this.numericUpDownDecimalPlaces.Location = new System.Drawing.Point(127, 18);
            this.numericUpDownDecimalPlaces.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.numericUpDownDecimalPlaces.Name = "numericUpDownDecimalPlaces";
            this.numericUpDownDecimalPlaces.Size = new System.Drawing.Size(57, 20);
            this.numericUpDownDecimalPlaces.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(0, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(124, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Maximum decimal places";
            // 
            // cbReplaceClipboard
            // 
            this.cbReplaceClipboard.AutoSize = true;
            this.cbReplaceClipboard.Dock = System.Windows.Forms.DockStyle.Top;
            this.cbReplaceClipboard.Location = new System.Drawing.Point(0, 0);
            this.cbReplaceClipboard.Name = "cbReplaceClipboard";
            this.cbReplaceClipboard.Size = new System.Drawing.Size(390, 17);
            this.cbReplaceClipboard.TabIndex = 2;
            this.cbReplaceClipboard.Text = "Copy result to clipboard if Enter is presses";
            this.cbReplaceClipboard.UseVisualStyleBackColor = true;
            // 
            // YalCalcUC
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cbReplaceClipboard);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.numericUpDownDecimalPlaces);
            this.Name = "YalCalcUC";
            this.Size = new System.Drawing.Size(390, 241);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownDecimalPlaces)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NumericUpDown numericUpDownDecimalPlaces;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox cbReplaceClipboard;
    }
}
