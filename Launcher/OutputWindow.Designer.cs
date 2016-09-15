namespace Yal
{
    partial class OutputWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OutputWindow));
            this.listViewOutput = new System.Windows.Forms.ListView();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.SuspendLayout();
            // 
            // listViewOutput
            // 
            this.listViewOutput.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listViewOutput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewOutput.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listViewOutput.HideSelection = false;
            this.listViewOutput.LargeImageList = this.imageList1;
            this.listViewOutput.Location = new System.Drawing.Point(0, 0);
            this.listViewOutput.MultiSelect = false;
            this.listViewOutput.Name = "listViewOutput";
            this.listViewOutput.Size = new System.Drawing.Size(265, 300);
            this.listViewOutput.TabIndex = 0;
            this.listViewOutput.TileSize = new System.Drawing.Size(248, 30);
            this.listViewOutput.UseCompatibleStateImageBehavior = false;
            this.listViewOutput.View = System.Windows.Forms.View.Tile;
            this.listViewOutput.MouseClick += new System.Windows.Forms.MouseEventHandler(this.listViewOutput_MouseClick);
            this.listViewOutput.MouseEnter += new System.EventHandler(this.listViewOutput_MouseEnter);
            // 
            // imageList1
            // 
            this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.imageList1.ImageSize = new System.Drawing.Size(24, 24);
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // OutputWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(265, 300);
            this.Controls.Add(this.listViewOutput);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "OutputWindow";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Launcher output window";
            this.ResumeLayout(false);

        }

        #endregion

        internal System.Windows.Forms.ListView listViewOutput;
        internal System.Windows.Forms.ImageList imageList1;
    }
}