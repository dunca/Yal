namespace Yal
{
    partial class Options
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Options));
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPageGeneral = new System.Windows.Forms.TabPage();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.cbShowItemIcons = new System.Windows.Forms.CheckBox();
            this.spinMaxNameSize = new System.Windows.Forms.NumericUpDown();
            this.label10 = new System.Windows.Forms.Label();
            this.cbPluginItemsFirst = new System.Windows.Forms.CheckBox();
            this.cbExtensionInFileName = new System.Windows.Forms.CheckBox();
            this.label8 = new System.Windows.Forms.Label();
            this.spinSearchDelay = new System.Windows.Forms.NumericUpDown();
            this.spinMaxVisible = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.spinMaxItems = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.cbFuzzyMatchingPluginItems = new System.Windows.Forms.CheckBox();
            this.cbFuzzyMatching = new System.Windows.Forms.CheckBox();
            this.cbPluginSelectionsInHistory = new System.Windows.Forms.CheckBox();
            this.spinMaxHistorySize = new System.Windows.Forms.NumericUpDown();
            this.spinAutoIndexingInterval = new System.Windows.Forms.NumericUpDown();
            this.cbAutoIndexing = new System.Windows.Forms.CheckBox();
            this.cbMatchAnywhere = new System.Windows.Forms.CheckBox();
            this.label9 = new System.Windows.Forms.Label();
            this.cbAutostart = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.spinMaxPluginItems = new System.Windows.Forms.NumericUpDown();
            this.buttonPickColor = new System.Windows.Forms.Button();
            this.label11 = new System.Windows.Forms.Label();
            this.comboBoxHKKey = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.comboBoxHKMod = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cbMoveWithCtrl = new System.Windows.Forms.CheckBox();
            this.cbTopMost = new System.Windows.Forms.CheckBox();
            this.cbVAlignment = new System.Windows.Forms.CheckBox();
            this.cbHAlignment = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.trackBarOpacity = new System.Windows.Forms.TrackBar();
            this.tabPageIndexing = new System.Windows.Forms.TabPage();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.listBoxExcludedLocations = new System.Windows.Forms.ListBox();
            this.btnAddExcludedLocation = new System.Windows.Forms.Button();
            this.btnRemoveExcluded = new System.Windows.Forms.Button();
            this.btnRebuild = new System.Windows.Forms.Button();
            this.lblIndexStatus = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txtExtensions = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cbIncludeSubdirs = new System.Windows.Forms.CheckBox();
            this.listBoxLocations = new System.Windows.Forms.ListBox();
            this.btnAddLocation = new System.Windows.Forms.Button();
            this.btnRemoveLocation = new System.Windows.Forms.Button();
            this.tabPagePlugins = new System.Windows.Forms.TabPage();
            this.tabControlPlugins = new System.Windows.Forms.TabControl();
            this.tabPageAbout = new System.Windows.Forms.TabPage();
            this.lblActivationStatistics = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.btnApplyOptions = new System.Windows.Forms.Button();
            this.btnCancelOpt = new System.Windows.Forms.Button();
            this.btnRestoreDefaultSettings = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabPageGeneral.SuspendLayout();
            this.groupBox5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spinMaxNameSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinSearchDelay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinMaxVisible)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinMaxItems)).BeginInit();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spinMaxHistorySize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinAutoIndexingInterval)).BeginInit();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spinMaxPluginItems)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarOpacity)).BeginInit();
            this.tabPageIndexing.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabPagePlugins.SuspendLayout();
            this.tabPageAbout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPageGeneral);
            this.tabControl1.Controls.Add(this.tabPageIndexing);
            this.tabControl1.Controls.Add(this.tabPagePlugins);
            this.tabControl1.Controls.Add(this.tabPageAbout);
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(450, 390);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPageGeneral
            // 
            this.tabPageGeneral.Controls.Add(this.groupBox5);
            this.tabPageGeneral.Controls.Add(this.groupBox4);
            this.tabPageGeneral.Controls.Add(this.groupBox3);
            this.tabPageGeneral.Location = new System.Drawing.Point(4, 22);
            this.tabPageGeneral.Name = "tabPageGeneral";
            this.tabPageGeneral.Size = new System.Drawing.Size(442, 364);
            this.tabPageGeneral.TabIndex = 0;
            this.tabPageGeneral.Text = "General";
            this.tabPageGeneral.UseVisualStyleBackColor = true;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.cbShowItemIcons);
            this.groupBox5.Controls.Add(this.spinMaxNameSize);
            this.groupBox5.Controls.Add(this.label10);
            this.groupBox5.Controls.Add(this.cbPluginItemsFirst);
            this.groupBox5.Controls.Add(this.cbExtensionInFileName);
            this.groupBox5.Controls.Add(this.label8);
            this.groupBox5.Controls.Add(this.spinSearchDelay);
            this.groupBox5.Controls.Add(this.spinMaxVisible);
            this.groupBox5.Controls.Add(this.label7);
            this.groupBox5.Controls.Add(this.spinMaxItems);
            this.groupBox5.Controls.Add(this.label6);
            this.groupBox5.Location = new System.Drawing.Point(214, 193);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(218, 167);
            this.groupBox5.TabIndex = 6;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Output window";
            // 
            // cbShowItemIcons
            // 
            this.cbShowItemIcons.AutoSize = true;
            this.cbShowItemIcons.Location = new System.Drawing.Point(6, 102);
            this.cbShowItemIcons.Name = "cbShowItemIcons";
            this.cbShowItemIcons.Size = new System.Drawing.Size(149, 17);
            this.cbShowItemIcons.TabIndex = 22;
            this.cbShowItemIcons.Text = "Load and show item icons";
            this.cbShowItemIcons.UseVisualStyleBackColor = true;
            // 
            // spinMaxNameSize
            // 
            this.spinMaxNameSize.Location = new System.Drawing.Point(151, 58);
            this.spinMaxNameSize.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.spinMaxNameSize.Name = "spinMaxNameSize";
            this.spinMaxNameSize.Size = new System.Drawing.Size(61, 20);
            this.spinMaxNameSize.TabIndex = 20;
            this.spinMaxNameSize.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(6, 60);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(131, 13);
            this.label10.TabIndex = 21;
            this.label10.Text = "Trim long names to (chars)";
            // 
            // cbPluginItemsFirst
            // 
            this.cbPluginItemsFirst.AutoSize = true;
            this.cbPluginItemsFirst.Location = new System.Drawing.Point(6, 146);
            this.cbPluginItemsFirst.Name = "cbPluginItemsFirst";
            this.cbPluginItemsFirst.Size = new System.Drawing.Size(174, 17);
            this.cbPluginItemsFirst.TabIndex = 17;
            this.cbPluginItemsFirst.Text = "Plugin items have higher priority";
            this.cbPluginItemsFirst.UseVisualStyleBackColor = true;
            // 
            // cbExtensionInFileName
            // 
            this.cbExtensionInFileName.AutoSize = true;
            this.cbExtensionInFileName.Location = new System.Drawing.Point(6, 123);
            this.cbExtensionInFileName.Name = "cbExtensionInFileName";
            this.cbExtensionInFileName.Size = new System.Drawing.Size(167, 17);
            this.cbExtensionInFileName.TabIndex = 3;
            this.cbExtensionInFileName.Text = "Show extensions in file names";
            this.cbExtensionInFileName.UseVisualStyleBackColor = true;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 81);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(134, 13);
            this.label8.TabIndex = 11;
            this.label8.Text = "Search delay (milliseconds)";
            // 
            // spinSearchDelay
            // 
            this.spinSearchDelay.Location = new System.Drawing.Point(151, 79);
            this.spinSearchDelay.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.spinSearchDelay.Name = "spinSearchDelay";
            this.spinSearchDelay.Size = new System.Drawing.Size(61, 20);
            this.spinSearchDelay.TabIndex = 10;
            this.spinSearchDelay.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // spinMaxVisible
            // 
            this.spinMaxVisible.Location = new System.Drawing.Point(151, 14);
            this.spinMaxVisible.Maximum = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.spinMaxVisible.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.spinMaxVisible.Name = "spinMaxVisible";
            this.spinMaxVisible.Size = new System.Drawing.Size(61, 20);
            this.spinMaxVisible.TabIndex = 9;
            this.spinMaxVisible.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 18);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(86, 13);
            this.label7.TabIndex = 8;
            this.label7.Text = "Max visible items";
            // 
            // spinMaxItems
            // 
            this.spinMaxItems.Location = new System.Drawing.Point(151, 35);
            this.spinMaxItems.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.spinMaxItems.Name = "spinMaxItems";
            this.spinMaxItems.Size = new System.Drawing.Size(61, 20);
            this.spinMaxItems.TabIndex = 7;
            this.spinMaxItems.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.spinMaxItems.ValueChanged += new System.EventHandler(this.spinMaxItems_ValueChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 39);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(108, 13);
            this.label6.TabIndex = 5;
            this.label6.Text = "Total number of items";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.cbFuzzyMatchingPluginItems);
            this.groupBox4.Controls.Add(this.cbFuzzyMatching);
            this.groupBox4.Controls.Add(this.cbPluginSelectionsInHistory);
            this.groupBox4.Controls.Add(this.spinMaxHistorySize);
            this.groupBox4.Controls.Add(this.spinAutoIndexingInterval);
            this.groupBox4.Controls.Add(this.cbAutoIndexing);
            this.groupBox4.Controls.Add(this.cbMatchAnywhere);
            this.groupBox4.Controls.Add(this.label9);
            this.groupBox4.Controls.Add(this.cbAutostart);
            this.groupBox4.Location = new System.Drawing.Point(214, 5);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(219, 182);
            this.groupBox4.TabIndex = 1;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "System settings";
            // 
            // cbFuzzyMatchingPluginItems
            // 
            this.cbFuzzyMatchingPluginItems.AutoSize = true;
            this.cbFuzzyMatchingPluginItems.Location = new System.Drawing.Point(6, 89);
            this.cbFuzzyMatchingPluginItems.Name = "cbFuzzyMatchingPluginItems";
            this.cbFuzzyMatchingPluginItems.Size = new System.Drawing.Size(147, 17);
            this.cbFuzzyMatchingPluginItems.TabIndex = 17;
            this.cbFuzzyMatchingPluginItems.Text = "Fuzzily match plugin items";
            this.cbFuzzyMatchingPluginItems.UseVisualStyleBackColor = true;
            // 
            // cbFuzzyMatching
            // 
            this.cbFuzzyMatching.AutoSize = true;
            this.cbFuzzyMatching.Location = new System.Drawing.Point(6, 66);
            this.cbFuzzyMatching.Name = "cbFuzzyMatching";
            this.cbFuzzyMatching.Size = new System.Drawing.Size(139, 17);
            this.cbFuzzyMatching.TabIndex = 16;
            this.cbFuzzyMatching.Text = "Fuzzily match file names";
            this.cbFuzzyMatching.UseVisualStyleBackColor = true;
            // 
            // cbPluginSelectionsInHistory
            // 
            this.cbPluginSelectionsInHistory.AutoSize = true;
            this.cbPluginSelectionsInHistory.Location = new System.Drawing.Point(6, 43);
            this.cbPluginSelectionsInHistory.Name = "cbPluginSelectionsInHistory";
            this.cbPluginSelectionsInHistory.Size = new System.Drawing.Size(153, 17);
            this.cbPluginSelectionsInHistory.TabIndex = 15;
            this.cbPluginSelectionsInHistory.Text = "Keep plugin items in history";
            this.cbPluginSelectionsInHistory.UseVisualStyleBackColor = true;
            // 
            // spinMaxHistorySize
            // 
            this.spinMaxHistorySize.Location = new System.Drawing.Point(151, 154);
            this.spinMaxHistorySize.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.spinMaxHistorySize.Name = "spinMaxHistorySize";
            this.spinMaxHistorySize.Size = new System.Drawing.Size(61, 20);
            this.spinMaxHistorySize.TabIndex = 14;
            this.spinMaxHistorySize.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // spinAutoIndexingInterval
            // 
            this.spinAutoIndexingInterval.Location = new System.Drawing.Point(151, 133);
            this.spinAutoIndexingInterval.Maximum = new decimal(new int[] {
            300,
            0,
            0,
            0});
            this.spinAutoIndexingInterval.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.spinAutoIndexingInterval.Name = "spinAutoIndexingInterval";
            this.spinAutoIndexingInterval.Size = new System.Drawing.Size(61, 20);
            this.spinAutoIndexingInterval.TabIndex = 12;
            this.spinAutoIndexingInterval.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // cbAutoIndexing
            // 
            this.cbAutoIndexing.AutoSize = true;
            this.cbAutoIndexing.Location = new System.Drawing.Point(6, 135);
            this.cbAutoIndexing.Name = "cbAutoIndexing";
            this.cbAutoIndexing.Size = new System.Drawing.Size(135, 17);
            this.cbAutoIndexing.TabIndex = 12;
            this.cbAutoIndexing.Text = "Auto indexing (minutes)";
            this.cbAutoIndexing.UseVisualStyleBackColor = true;
            // 
            // cbMatchAnywhere
            // 
            this.cbMatchAnywhere.AutoSize = true;
            this.cbMatchAnywhere.Location = new System.Drawing.Point(6, 112);
            this.cbMatchAnywhere.Name = "cbMatchAnywhere";
            this.cbMatchAnywhere.Size = new System.Drawing.Size(172, 17);
            this.cbMatchAnywhere.TabIndex = 12;
            this.cbMatchAnywhere.Text = "Match anywhere in item names";
            this.cbMatchAnywhere.UseVisualStyleBackColor = true;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 158);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(141, 13);
            this.label9.TabIndex = 13;
            this.label9.Text = "Total items to keep in history";
            // 
            // cbAutostart
            // 
            this.cbAutostart.AutoSize = true;
            this.cbAutostart.Location = new System.Drawing.Point(6, 20);
            this.cbAutostart.Name = "cbAutostart";
            this.cbAutostart.Size = new System.Drawing.Size(144, 17);
            this.cbAutostart.TabIndex = 2;
            this.cbAutostart.Text = "Launch at system startup\r\n";
            this.cbAutostart.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.spinMaxPluginItems);
            this.groupBox3.Controls.Add(this.buttonPickColor);
            this.groupBox3.Controls.Add(this.label11);
            this.groupBox3.Controls.Add(this.comboBoxHKKey);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.comboBoxHKMod);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.cbMoveWithCtrl);
            this.groupBox3.Controls.Add(this.cbTopMost);
            this.groupBox3.Controls.Add(this.cbVAlignment);
            this.groupBox3.Controls.Add(this.cbHAlignment);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Controls.Add(this.trackBarOpacity);
            this.groupBox3.Location = new System.Drawing.Point(9, 6);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(200, 354);
            this.groupBox3.TabIndex = 0;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Look and feel";
            // 
            // spinMaxPluginItems
            // 
            this.spinMaxPluginItems.Location = new System.Drawing.Point(149, 241);
            this.spinMaxPluginItems.Maximum = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.spinMaxPluginItems.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.spinMaxPluginItems.Name = "spinMaxPluginItems";
            this.spinMaxPluginItems.Size = new System.Drawing.Size(61, 20);
            this.spinMaxPluginItems.TabIndex = 16;
            this.spinMaxPluginItems.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.spinMaxPluginItems.Visible = false;
            // 
            // buttonPickColor
            // 
            this.buttonPickColor.Location = new System.Drawing.Point(116, 48);
            this.buttonPickColor.Name = "buttonPickColor";
            this.buttonPickColor.Size = new System.Drawing.Size(75, 23);
            this.buttonPickColor.TabIndex = 11;
            this.buttonPickColor.Text = "Pick a color";
            this.buttonPickColor.UseVisualStyleBackColor = true;
            this.buttonPickColor.Click += new System.EventHandler(this.btnPickColor_Click);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(4, 245);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(85, 13);
            this.label11.TabIndex = 15;
            this.label11.Text = "Max plugin items";
            this.label11.Visible = false;
            // 
            // comboBoxHKKey
            // 
            this.comboBoxHKKey.Location = new System.Drawing.Point(135, 171);
            this.comboBoxHKKey.Name = "comboBoxHKKey";
            this.comboBoxHKKey.Size = new System.Drawing.Size(56, 21);
            this.comboBoxHKKey.TabIndex = 10;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(116, 175);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(13, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "+";
            // 
            // comboBoxHKMod
            // 
            this.comboBoxHKMod.Location = new System.Drawing.Point(52, 171);
            this.comboBoxHKMod.Name = "comboBoxHKMod";
            this.comboBoxHKMod.Size = new System.Drawing.Size(56, 21);
            this.comboBoxHKMod.TabIndex = 8;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(5, 175);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Hotkey";
            // 
            // cbMoveWithCtrl
            // 
            this.cbMoveWithCtrl.AutoSize = true;
            this.cbMoveWithCtrl.Location = new System.Drawing.Point(5, 153);
            this.cbMoveWithCtrl.Name = "cbMoveWithCtrl";
            this.cbMoveWithCtrl.Size = new System.Drawing.Size(184, 17);
            this.cbMoveWithCtrl.TabIndex = 6;
            this.cbMoveWithCtrl.Text = "Only move while CTRL is pressed";
            this.cbMoveWithCtrl.UseVisualStyleBackColor = true;
            // 
            // cbTopMost
            // 
            this.cbTopMost.AutoSize = true;
            this.cbTopMost.Location = new System.Drawing.Point(5, 87);
            this.cbTopMost.Name = "cbTopMost";
            this.cbTopMost.Size = new System.Drawing.Size(92, 17);
            this.cbTopMost.TabIndex = 5;
            this.cbTopMost.Text = "Always on top";
            this.cbTopMost.UseVisualStyleBackColor = true;
            // 
            // cbVAlignment
            // 
            this.cbVAlignment.AutoSize = true;
            this.cbVAlignment.Location = new System.Drawing.Point(5, 109);
            this.cbVAlignment.Name = "cbVAlignment";
            this.cbVAlignment.Size = new System.Drawing.Size(109, 17);
            this.cbVAlignment.TabIndex = 4;
            this.cbVAlignment.Text = "Vertical alignment";
            this.cbVAlignment.UseVisualStyleBackColor = true;
            // 
            // cbHAlignment
            // 
            this.cbHAlignment.AutoSize = true;
            this.cbHAlignment.Location = new System.Drawing.Point(5, 131);
            this.cbHAlignment.Name = "cbHAlignment";
            this.cbHAlignment.Size = new System.Drawing.Size(124, 17);
            this.cbHAlignment.TabIndex = 3;
            this.cbHAlignment.Text = "Horizontal  alignment";
            this.cbHAlignment.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(5, 19);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(43, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Opacity";
            // 
            // trackBarOpacity
            // 
            this.trackBarOpacity.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.trackBarOpacity.Location = new System.Drawing.Point(5, 37);
            this.trackBarOpacity.Minimum = 4;
            this.trackBarOpacity.Name = "trackBarOpacity";
            this.trackBarOpacity.Size = new System.Drawing.Size(91, 45);
            this.trackBarOpacity.TabIndex = 2;
            this.trackBarOpacity.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.trackBarOpacity.Value = 4;
            // 
            // tabPageIndexing
            // 
            this.tabPageIndexing.Controls.Add(this.groupBox6);
            this.tabPageIndexing.Controls.Add(this.btnRebuild);
            this.tabPageIndexing.Controls.Add(this.lblIndexStatus);
            this.tabPageIndexing.Controls.Add(this.label1);
            this.tabPageIndexing.Controls.Add(this.groupBox2);
            this.tabPageIndexing.Controls.Add(this.groupBox1);
            this.tabPageIndexing.Location = new System.Drawing.Point(4, 22);
            this.tabPageIndexing.Name = "tabPageIndexing";
            this.tabPageIndexing.Size = new System.Drawing.Size(442, 364);
            this.tabPageIndexing.TabIndex = 1;
            this.tabPageIndexing.Text = "Indexing";
            this.tabPageIndexing.UseVisualStyleBackColor = true;
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.listBoxExcludedLocations);
            this.groupBox6.Controls.Add(this.btnAddExcludedLocation);
            this.groupBox6.Controls.Add(this.btnRemoveExcluded);
            this.groupBox6.Location = new System.Drawing.Point(8, 162);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(426, 109);
            this.groupBox6.TabIndex = 8;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Folders to exclude";
            // 
            // listBoxExcludedLocations
            // 
            this.listBoxExcludedLocations.FormattingEnabled = true;
            this.listBoxExcludedLocations.HorizontalScrollbar = true;
            this.listBoxExcludedLocations.Location = new System.Drawing.Point(6, 19);
            this.listBoxExcludedLocations.Name = "listBoxExcludedLocations";
            this.listBoxExcludedLocations.Size = new System.Drawing.Size(333, 82);
            this.listBoxExcludedLocations.TabIndex = 4;
            // 
            // btnAddExcludedLocation
            // 
            this.btnAddExcludedLocation.Location = new System.Drawing.Point(345, 19);
            this.btnAddExcludedLocation.Name = "btnAddExcludedLocation";
            this.btnAddExcludedLocation.Size = new System.Drawing.Size(75, 23);
            this.btnAddExcludedLocation.TabIndex = 1;
            this.btnAddExcludedLocation.Text = "Add";
            this.btnAddExcludedLocation.UseVisualStyleBackColor = true;
            this.btnAddExcludedLocation.Click += new System.EventHandler(this.btnAddExcludedLocation_Click);
            // 
            // btnRemoveExcluded
            // 
            this.btnRemoveExcluded.Location = new System.Drawing.Point(345, 45);
            this.btnRemoveExcluded.Name = "btnRemoveExcluded";
            this.btnRemoveExcluded.Size = new System.Drawing.Size(75, 23);
            this.btnRemoveExcluded.TabIndex = 2;
            this.btnRemoveExcluded.Text = "Remove";
            this.btnRemoveExcluded.UseVisualStyleBackColor = true;
            this.btnRemoveExcluded.Click += new System.EventHandler(this.btnRemoveExcludedLocation_Click);
            // 
            // btnRebuild
            // 
            this.btnRebuild.Location = new System.Drawing.Point(353, 335);
            this.btnRebuild.Name = "btnRebuild";
            this.btnRebuild.Size = new System.Drawing.Size(75, 23);
            this.btnRebuild.TabIndex = 7;
            this.btnRebuild.Text = "Rebuild";
            this.btnRebuild.UseVisualStyleBackColor = true;
            this.btnRebuild.Click += new System.EventHandler(this.btnRebuild_Click);
            // 
            // lblIndexStatus
            // 
            this.lblIndexStatus.AutoSize = true;
            this.lblIndexStatus.Location = new System.Drawing.Point(76, 340);
            this.lblIndexStatus.Name = "lblIndexStatus";
            this.lblIndexStatus.Size = new System.Drawing.Size(10, 13);
            this.lblIndexStatus.TabIndex = 3;
            this.lblIndexStatus.Text = "-";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 340);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Last indexed:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.txtExtensions);
            this.groupBox2.Location = new System.Drawing.Point(8, 277);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(426, 45);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Extensions (comma separated)";
            // 
            // txtExtensions
            // 
            this.txtExtensions.Location = new System.Drawing.Point(6, 19);
            this.txtExtensions.Name = "txtExtensions";
            this.txtExtensions.Size = new System.Drawing.Size(414, 20);
            this.txtExtensions.TabIndex = 6;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cbIncludeSubdirs);
            this.groupBox1.Controls.Add(this.listBoxLocations);
            this.groupBox1.Controls.Add(this.btnAddLocation);
            this.groupBox1.Controls.Add(this.btnRemoveLocation);
            this.groupBox1.Location = new System.Drawing.Point(8, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(426, 150);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Folders to index";
            // 
            // cbIncludeSubdirs
            // 
            this.cbIncludeSubdirs.AutoSize = true;
            this.cbIncludeSubdirs.Location = new System.Drawing.Point(345, 70);
            this.cbIncludeSubdirs.Name = "cbIncludeSubdirs";
            this.cbIncludeSubdirs.Size = new System.Drawing.Size(76, 17);
            this.cbIncludeSubdirs.TabIndex = 5;
            this.cbIncludeSubdirs.Text = "Subfolders";
            this.cbIncludeSubdirs.UseVisualStyleBackColor = true;
            // 
            // listBoxLocations
            // 
            this.listBoxLocations.FormattingEnabled = true;
            this.listBoxLocations.HorizontalScrollbar = true;
            this.listBoxLocations.Location = new System.Drawing.Point(6, 18);
            this.listBoxLocations.Name = "listBoxLocations";
            this.listBoxLocations.Size = new System.Drawing.Size(333, 121);
            this.listBoxLocations.TabIndex = 4;
            // 
            // btnAddLocation
            // 
            this.btnAddLocation.Location = new System.Drawing.Point(345, 18);
            this.btnAddLocation.Name = "btnAddLocation";
            this.btnAddLocation.Size = new System.Drawing.Size(75, 23);
            this.btnAddLocation.TabIndex = 1;
            this.btnAddLocation.Text = "Add";
            this.btnAddLocation.UseVisualStyleBackColor = true;
            this.btnAddLocation.Click += new System.EventHandler(this.btnAddLocation_Click);
            // 
            // btnRemoveLocation
            // 
            this.btnRemoveLocation.Location = new System.Drawing.Point(345, 44);
            this.btnRemoveLocation.Name = "btnRemoveLocation";
            this.btnRemoveLocation.Size = new System.Drawing.Size(75, 23);
            this.btnRemoveLocation.TabIndex = 2;
            this.btnRemoveLocation.Text = "Remove";
            this.btnRemoveLocation.UseVisualStyleBackColor = true;
            this.btnRemoveLocation.Click += new System.EventHandler(this.btnRemoveLocation_Click);
            // 
            // tabPagePlugins
            // 
            this.tabPagePlugins.Controls.Add(this.tabControlPlugins);
            this.tabPagePlugins.Location = new System.Drawing.Point(4, 22);
            this.tabPagePlugins.Name = "tabPagePlugins";
            this.tabPagePlugins.Padding = new System.Windows.Forms.Padding(3);
            this.tabPagePlugins.Size = new System.Drawing.Size(442, 364);
            this.tabPagePlugins.TabIndex = 3;
            this.tabPagePlugins.Text = "Plugins";
            this.tabPagePlugins.UseVisualStyleBackColor = true;
            // 
            // tabControlPlugins
            // 
            this.tabControlPlugins.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlPlugins.Location = new System.Drawing.Point(3, 3);
            this.tabControlPlugins.Name = "tabControlPlugins";
            this.tabControlPlugins.SelectedIndex = 0;
            this.tabControlPlugins.Size = new System.Drawing.Size(436, 358);
            this.tabControlPlugins.TabIndex = 0;
            // 
            // tabPageAbout
            // 
            this.tabPageAbout.Controls.Add(this.lblActivationStatistics);
            this.tabPageAbout.Controls.Add(this.label5);
            this.tabPageAbout.Controls.Add(this.pictureBox1);
            this.tabPageAbout.Location = new System.Drawing.Point(4, 22);
            this.tabPageAbout.Name = "tabPageAbout";
            this.tabPageAbout.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageAbout.Size = new System.Drawing.Size(442, 364);
            this.tabPageAbout.TabIndex = 2;
            this.tabPageAbout.Text = "About";
            this.tabPageAbout.UseVisualStyleBackColor = true;
            // 
            // lblActivationStatistics
            // 
            this.lblActivationStatistics.AutoSize = true;
            this.lblActivationStatistics.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblActivationStatistics.Location = new System.Drawing.Point(3, 3);
            this.lblActivationStatistics.Name = "lblActivationStatistics";
            this.lblActivationStatistics.Size = new System.Drawing.Size(41, 13);
            this.lblActivationStatistics.TabIndex = 2;
            this.lblActivationStatistics.Text = "label12";
            // 
            // label5
            // 
            this.label5.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.label5.Location = new System.Drawing.Point(3, 348);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(436, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "YetAnotherLauncher 1.0";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Image = global::Yal.Properties.Resources.ICON;
            this.pictureBox1.Location = new System.Drawing.Point(3, 3);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(436, 358);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // btnApplyOptions
            // 
            this.btnApplyOptions.Location = new System.Drawing.Point(106, 392);
            this.btnApplyOptions.Name = "btnApplyOptions";
            this.btnApplyOptions.Size = new System.Drawing.Size(75, 23);
            this.btnApplyOptions.TabIndex = 12;
            this.btnApplyOptions.Text = "Apply";
            this.btnApplyOptions.UseVisualStyleBackColor = true;
            this.btnApplyOptions.Click += new System.EventHandler(this.btnApplyOptions_Click);
            // 
            // btnCancelOpt
            // 
            this.btnCancelOpt.Location = new System.Drawing.Point(187, 392);
            this.btnCancelOpt.Name = "btnCancelOpt";
            this.btnCancelOpt.Size = new System.Drawing.Size(75, 23);
            this.btnCancelOpt.TabIndex = 13;
            this.btnCancelOpt.Text = "Cancel";
            this.btnCancelOpt.UseVisualStyleBackColor = true;
            this.btnCancelOpt.Click += new System.EventHandler(this.btnCancelOpt_Click);
            // 
            // btnRestoreDefaultSettings
            // 
            this.btnRestoreDefaultSettings.Location = new System.Drawing.Point(268, 392);
            this.btnRestoreDefaultSettings.Name = "btnRestoreDefaultSettings";
            this.btnRestoreDefaultSettings.Size = new System.Drawing.Size(75, 23);
            this.btnRestoreDefaultSettings.TabIndex = 7;
            this.btnRestoreDefaultSettings.Text = "Defaults";
            this.btnRestoreDefaultSettings.UseVisualStyleBackColor = true;
            this.btnRestoreDefaultSettings.Click += new System.EventHandler(this.btnRestoreDefaultSettings_Click);
            // 
            // Options
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(448, 417);
            this.Controls.Add(this.btnRestoreDefaultSettings);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.btnCancelOpt);
            this.Controls.Add(this.btnApplyOptions);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Options";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "YetAnotherLauncher options";
            this.tabControl1.ResumeLayout(false);
            this.tabPageGeneral.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spinMaxNameSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinSearchDelay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinMaxVisible)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinMaxItems)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spinMaxHistorySize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinAutoIndexingInterval)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spinMaxPluginItems)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarOpacity)).EndInit();
            this.tabPageIndexing.ResumeLayout(false);
            this.tabPageIndexing.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabPagePlugins.ResumeLayout(false);
            this.tabPageAbout.ResumeLayout(false);
            this.tabPageAbout.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPageIndexing;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnRemoveLocation;
        private System.Windows.Forms.Button btnAddLocation;
        private System.Windows.Forms.ListBox listBoxLocations;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox txtExtensions;
        private System.Windows.Forms.CheckBox cbIncludeSubdirs;
        private System.Windows.Forms.Label lblIndexStatus;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnRebuild;
        private System.Windows.Forms.ColorDialog colorDialog1;
        private System.Windows.Forms.TabPage tabPageGeneral;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.CheckBox cbAutostart;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button buttonPickColor;
        private System.Windows.Forms.ComboBox comboBoxHKKey;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox comboBoxHKMod;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox cbMoveWithCtrl;
        private System.Windows.Forms.CheckBox cbTopMost;
        private System.Windows.Forms.CheckBox cbVAlignment;
        private System.Windows.Forms.CheckBox cbHAlignment;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TrackBar trackBarOpacity;
        private System.Windows.Forms.NumericUpDown spinAutoIndexingInterval;
        private System.Windows.Forms.CheckBox cbAutoIndexing;
        private System.Windows.Forms.Button btnApplyOptions;
        private System.Windows.Forms.Button btnCancelOpt;
        private System.Windows.Forms.CheckBox cbMatchAnywhere;
        private System.Windows.Forms.TabPage tabPageAbout;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown spinMaxHistorySize;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.ListBox listBoxExcludedLocations;
        private System.Windows.Forms.Button btnAddExcludedLocation;
        private System.Windows.Forms.Button btnRemoveExcluded;
        private System.Windows.Forms.TabPage tabPagePlugins;
        private System.Windows.Forms.TabControl tabControlPlugins;
        private System.Windows.Forms.CheckBox cbPluginSelectionsInHistory;
        private System.Windows.Forms.NumericUpDown spinMaxPluginItems;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.CheckBox cbFuzzyMatching;
        private System.Windows.Forms.CheckBox cbFuzzyMatchingPluginItems;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.NumericUpDown spinMaxNameSize;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.CheckBox cbPluginItemsFirst;
        private System.Windows.Forms.CheckBox cbExtensionInFileName;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.NumericUpDown spinSearchDelay;
        private System.Windows.Forms.NumericUpDown spinMaxVisible;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown spinMaxItems;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.CheckBox cbShowItemIcons;
        private System.Windows.Forms.Label lblActivationStatistics;
        private System.Windows.Forms.Button btnRestoreDefaultSettings;
    }
}