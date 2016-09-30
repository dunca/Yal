namespace Yal
{
    partial class Yal
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Yal));
            this.yalContextStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.showYalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.RebuildToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.timerAutoIndexing = new System.Windows.Forms.Timer(this.components);
            this.yalContextStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // yalContextStrip
            // 
            this.yalContextStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showYalToolStripMenuItem,
            this.optionsToolStripMenuItem,
            this.RebuildToolStripMenuItem,
            this.toolStripSeparator1,
            this.exitToolStripMenuItem});
            this.yalContextStrip.Name = "contextMenuStrip1";
            this.yalContextStrip.ShowImageMargin = false;
            this.yalContextStrip.Size = new System.Drawing.Size(140, 98);
            this.yalContextStrip.Opening += new System.ComponentModel.CancelEventHandler(this.yalContextStrip_Opening);
            // 
            // showYalToolStripMenuItem
            // 
            this.showYalToolStripMenuItem.Name = "showYalToolStripMenuItem";
            this.showYalToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.showYalToolStripMenuItem.Text = "Show Yal";
            this.showYalToolStripMenuItem.Visible = false;
            this.showYalToolStripMenuItem.Click += new System.EventHandler(this.showYalToolStripMenuItem_Click);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.optionsToolStripMenuItem.Text = "Options";
            this.optionsToolStripMenuItem.Click += new System.EventHandler(this.optionsToolStripMenuItem_Click);
            // 
            // RebuildToolStripMenuItem
            // 
            this.RebuildToolStripMenuItem.Name = "RebuildToolStripMenuItem";
            this.RebuildToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F5;
            this.RebuildToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.RebuildToolStripMenuItem.Text = "Rebuild index";
            this.RebuildToolStripMenuItem.Click += new System.EventHandler(this.rebuildToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(136, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // txtSearch
            // 
            this.txtSearch.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSearch.Location = new System.Drawing.Point(12, 8);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(241, 20);
            this.txtSearch.TabIndex = 0;
            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
            this.txtSearch.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSearch_KeyDown);
            // 
            // notifyIcon
            // 
            this.notifyIcon.ContextMenuStrip = this.yalContextStrip;
            this.notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon.Icon")));
            this.notifyIcon.Text = "Click to show YetAnotherLauncher";
            this.notifyIcon.Visible = true;
            this.notifyIcon.MouseClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon_MouseClick);
            // 
            // Yal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlDark;
            this.ClientSize = new System.Drawing.Size(265, 37);
            this.ContextMenuStrip = this.yalContextStrip;
            this.Controls.Add(this.txtSearch);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Yal";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Yal";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Yal_FormClosing);
            this.Load += new System.EventHandler(this.Yal_Load);
            this.Shown += new System.EventHandler(this.Yal_Shown);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Yal_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Yal_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Yal_MouseUp);
            this.Move += new System.EventHandler(this.Yal_Move);
            this.yalContextStrip.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ContextMenuStrip yalContextStrip;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem showYalToolStripMenuItem;
        public System.Windows.Forms.Timer timerAutoIndexing;
        private System.Windows.Forms.ToolStripMenuItem RebuildToolStripMenuItem;
        internal System.Windows.Forms.TextBox txtSearch;
    }
}

