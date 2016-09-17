namespace YalCommand
{
    partial class YalCommandUC
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
            this.components = new System.ComponentModel.Container();
            this.listViewEntries = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.txtCommand = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtTarget = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtParameters = new System.Windows.Forms.TextBox();
            this.cbxConfirm = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnAdd = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.copyCommandToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyTargetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyParametersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeEntryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // listViewEntries
            // 
            this.listViewEntries.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4});
            this.listViewEntries.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewEntries.FullRowSelect = true;
            this.listViewEntries.Location = new System.Drawing.Point(0, 0);
            this.listViewEntries.MultiSelect = false;
            this.listViewEntries.Name = "listViewEntries";
            this.listViewEntries.Size = new System.Drawing.Size(437, 182);
            this.listViewEntries.TabIndex = 0;
            this.listViewEntries.UseCompatibleStateImageBehavior = false;
            this.listViewEntries.View = System.Windows.Forms.View.Details;
            this.listViewEntries.MouseClick += new System.Windows.Forms.MouseEventHandler(this.listViewEntries_MouseClick);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Command";
            this.columnHeader1.Width = 74;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Target";
            this.columnHeader2.Width = 227;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Parameters";
            this.columnHeader3.Width = 65;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Confirm";
            this.columnHeader4.Width = 48;
            // 
            // txtCommand
            // 
            this.txtCommand.Location = new System.Drawing.Point(70, 6);
            this.txtCommand.Name = "txtCommand";
            this.txtCommand.Size = new System.Drawing.Size(66, 20);
            this.txtCommand.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Command";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(142, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Target";
            // 
            // txtTarget
            // 
            this.txtTarget.Location = new System.Drawing.Point(186, 5);
            this.txtTarget.Name = "txtTarget";
            this.txtTarget.Size = new System.Drawing.Size(150, 20);
            this.txtTarget.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 33);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(60, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Parameters";
            // 
            // txtParameters
            // 
            this.txtParameters.Location = new System.Drawing.Point(70, 29);
            this.txtParameters.Name = "txtParameters";
            this.txtParameters.Size = new System.Drawing.Size(66, 20);
            this.txtParameters.TabIndex = 6;
            // 
            // cbxConfirm
            // 
            this.cbxConfirm.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxConfirm.FormattingEnabled = true;
            this.cbxConfirm.Items.AddRange(new object[] {
            "True",
            "False"});
            this.cbxConfirm.Location = new System.Drawing.Point(262, 28);
            this.cbxConfirm.Name = "cbxConfirm";
            this.cbxConfirm.Size = new System.Drawing.Size(122, 21);
            this.cbxConfirm.TabIndex = 8;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(143, 32);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(113, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Confirm before running";
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(390, 16);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(34, 23);
            this.btnAdd.TabIndex = 10;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.btnBrowse);
            this.panel1.Controls.Add(this.btnAdd);
            this.panel1.Controls.Add(this.txtCommand);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.cbxConfirm);
            this.panel1.Controls.Add(this.txtTarget);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.txtParameters);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 182);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(437, 52);
            this.panel1.TabIndex = 11;
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(357, 4);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(28, 23);
            this.btnBrowse.TabIndex = 10;
            this.btnBrowse.Text = "...";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(339, 9);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(16, 13);
            this.label5.TabIndex = 11;
            this.label5.Text = "or";
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyCommandToolStripMenuItem,
            this.copyTargetToolStripMenuItem,
            this.copyParametersToolStripMenuItem,
            this.removeEntryToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(165, 92);
            // 
            // copyCommandToolStripMenuItem
            // 
            this.copyCommandToolStripMenuItem.Name = "copyCommandToolStripMenuItem";
            this.copyCommandToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.copyCommandToolStripMenuItem.Text = "Copy command";
            this.copyCommandToolStripMenuItem.Click += new System.EventHandler(this.copyCommandToolStripMenuItem_Click);
            // 
            // copyTargetToolStripMenuItem
            // 
            this.copyTargetToolStripMenuItem.Name = "copyTargetToolStripMenuItem";
            this.copyTargetToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.copyTargetToolStripMenuItem.Text = "Copy target";
            this.copyTargetToolStripMenuItem.Click += new System.EventHandler(this.copyTargetToolStripMenuItem_Click);
            // 
            // copyParametersToolStripMenuItem
            // 
            this.copyParametersToolStripMenuItem.Name = "copyParametersToolStripMenuItem";
            this.copyParametersToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.copyParametersToolStripMenuItem.Text = "Copy parameters";
            this.copyParametersToolStripMenuItem.Click += new System.EventHandler(this.copyParametersToolStripMenuItem_Click);
            // 
            // removeEntryToolStripMenuItem
            // 
            this.removeEntryToolStripMenuItem.Name = "removeEntryToolStripMenuItem";
            this.removeEntryToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.removeEntryToolStripMenuItem.Text = "Remove entry";
            this.removeEntryToolStripMenuItem.Click += new System.EventHandler(this.removeEntryToolStripMenuItem_Click);
            // 
            // YalCommandUC
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.listViewEntries);
            this.Controls.Add(this.panel1);
            this.Name = "YalCommandUC";
            this.Size = new System.Drawing.Size(437, 234);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView listViewEntries;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.TextBox txtCommand;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtTarget;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtParameters;
        private System.Windows.Forms.ComboBox cbxConfirm;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem copyCommandToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyTargetToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyParametersToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeEntryToolStripMenuItem;
    }
}
