namespace vibrance.GUI.common
{
    partial class VibranceGUI
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VibranceGUI));
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.projectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.backgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.checkBoxAutostart = new System.Windows.Forms.CheckBox();
            this.labelGraphicsAdapter = new System.Windows.Forms.Label();
            this.comboBoxGraphicsAdapter = new System.Windows.Forms.ComboBox();
            this.labelGraphicsAdapterRestart = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.checkBoxNeverChangeResolutions = new System.Windows.Forms.CheckBox();
            this.checkBoxPrimaryMonitorOnly = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.labelWindowsLevel = new System.Windows.Forms.Label();
            this.trackBarWindowsLevel = new System.Windows.Forms.TrackBar();
            this.statusLabel = new System.Windows.Forms.Label();
            this.observerStatusLabel = new System.Windows.Forms.Label();
            this.labelProduct = new System.Windows.Forms.Label();
            this.linkLabelProject = new System.Windows.Forms.LinkLabel();
            this.settingsBackgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.labelSubtitle = new System.Windows.Forms.Label();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.buttonProcessExplorer = new System.Windows.Forms.Button();
            this.buttonRemoveProgram = new System.Windows.Forms.Button();
            this.listApplications = new System.Windows.Forms.ListView();
            this.buttonAddProgram = new System.Windows.Forms.Button();
            this.contextMenuStrip.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarWindowsLevel)).BeginInit();
            this.groupBox5.SuspendLayout();
            this.SuspendLayout();
            // 
            // notifyIcon
            // 
            this.notifyIcon.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.notifyIcon.BalloonTipText = "Running in the notification area. Click to open vibranceGUIx.";
            this.notifyIcon.BalloonTipTitle = AppName;
            this.notifyIcon.ContextMenuStrip = this.contextMenuStrip;
            this.notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon.Icon")));
            this.notifyIcon.Text = AppName;
            this.notifyIcon.Visible = true;
            this.notifyIcon.MouseClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon_MouseClick);
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.projectToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.Size = new System.Drawing.Size(217, 48);
            this.contextMenuStrip.Text = "Vibrance Control";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Text = "Open vibranceGUIx";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            //
            // projectToolStripMenuItem
            //
            this.projectToolStripMenuItem.Name = "projectToolStripMenuItem";
            this.projectToolStripMenuItem.Text = "Project and help";
            this.projectToolStripMenuItem.Click += new System.EventHandler(this.projectToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("exitToolStripMenuItem.Image")));
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(216, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // backgroundWorker
            // 
            this.backgroundWorker.WorkerReportsProgress = true;
            this.backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker_DoWork);
            this.backgroundWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker_ProgressChanged);
            // 
            // checkBoxAutostart
            // 
            this.checkBoxAutostart.AutoSize = true;
            this.checkBoxAutostart.Location = new System.Drawing.Point(6, 19);
            this.checkBoxAutostart.Name = "checkBoxAutostart";
            this.checkBoxAutostart.Size = new System.Drawing.Size(131, 17);
            this.checkBoxAutostart.TabIndex = 8;
            this.checkBoxAutostart.Text = "Launch on boot (at Windows sign-in)";
            this.toolTip.SetToolTip(this.checkBoxAutostart, "Start minimized in the notification area when you sign in. Windows Task Manager can also disable startup apps.");
            this.checkBoxAutostart.UseVisualStyleBackColor = true;
            this.checkBoxAutostart.CheckedChanged += new System.EventHandler(this.checkBoxAutostart_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.labelGraphicsAdapter);
            this.groupBox1.Controls.Add(this.comboBoxGraphicsAdapter);
            this.groupBox1.Controls.Add(this.labelGraphicsAdapterRestart);
            this.groupBox1.Controls.Add(this.checkBoxNeverChangeResolutions);
            this.groupBox1.Controls.Add(this.checkBoxPrimaryMonitorOnly);
            this.groupBox1.Controls.Add(this.groupBox3);
            this.groupBox1.Controls.Add(this.checkBoxAutostart);
            this.groupBox1.Location = new System.Drawing.Point(13, 82);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(397, 143);
            this.groupBox1.TabIndex = 15;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Settings";
            //
            // GPU preference
            //
            this.labelGraphicsAdapter.AutoSize = true;
            this.labelGraphicsAdapter.Location = new System.Drawing.Point(196, 65);
            this.labelGraphicsAdapter.Text = "Graphics driver:";
            this.comboBoxGraphicsAdapter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxGraphicsAdapter.Items.AddRange(new object[] { "Auto (prefer NVIDIA)", "NVIDIA", "AMD Radeon" });
            this.comboBoxGraphicsAdapter.Location = new System.Drawing.Point(196, 83);
            this.comboBoxGraphicsAdapter.Name = "comboBoxGraphicsAdapter";
            this.comboBoxGraphicsAdapter.Size = new System.Drawing.Size(193, 21);
            this.comboBoxGraphicsAdapter.TabIndex = 17;
            this.comboBoxGraphicsAdapter.SelectedIndexChanged += new System.EventHandler(this.comboBoxGraphicsAdapter_SelectedIndexChanged);
            this.toolTip.SetToolTip(this.comboBoxGraphicsAdapter, "Auto prefers NVIDIA on systems with both NVIDIA and AMD GPUs. Only displays connected to the selected GPU can be controlled. Changes apply after restarting vibranceGUIx.");
            this.labelGraphicsAdapterRestart.Location = new System.Drawing.Point(196, 109);
            this.labelGraphicsAdapterRestart.Size = new System.Drawing.Size(193, 28);
            this.labelGraphicsAdapterRestart.Text = "Changes apply after restart.";
            // 
            // checkBoxNeverChangeResolutions
            // 
            this.checkBoxNeverChangeResolutions.AutoSize = true;
            this.checkBoxNeverChangeResolutions.Location = new System.Drawing.Point(163, 42);
            this.checkBoxNeverChangeResolutions.Name = "checkBoxNeverChangeResolutions";
            this.checkBoxNeverChangeResolutions.Size = new System.Drawing.Size(147, 17);
            this.checkBoxNeverChangeResolutions.TabIndex = 16;
            this.checkBoxNeverChangeResolutions.Text = "Never change resolutions";
            this.toolTip.SetToolTip(this.checkBoxNeverChangeResolutions, "When checking this, VibranceGUI will never change the resolution on any of your m" +
        "onitors.");
            this.checkBoxNeverChangeResolutions.UseVisualStyleBackColor = true;
            this.checkBoxNeverChangeResolutions.CheckedChanged += new System.EventHandler(this.checkBoxNeverChangeResolutions_CheckedChanged);
            // 
            // checkBoxPrimaryMonitorOnly
            // 
            this.checkBoxPrimaryMonitorOnly.AutoSize = true;
            this.checkBoxPrimaryMonitorOnly.Location = new System.Drawing.Point(6, 42);
            this.checkBoxPrimaryMonitorOnly.Name = "checkBoxPrimaryMonitorOnly";
            this.checkBoxPrimaryMonitorOnly.Size = new System.Drawing.Size(151, 17);
            this.checkBoxPrimaryMonitorOnly.TabIndex = 15;
            this.checkBoxPrimaryMonitorOnly.Text = "Primary monitor only";
            this.toolTip.SetToolTip(this.checkBoxPrimaryMonitorOnly, "When checking this, VibranceGUI will only change vibrance values on your primary " +
        "monitor.");
            this.checkBoxPrimaryMonitorOnly.UseVisualStyleBackColor = true;
            this.checkBoxPrimaryMonitorOnly.CheckedChanged += new System.EventHandler(this.checkBoxPrimaryMonitorOnly_CheckedChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.labelWindowsLevel);
            this.groupBox3.Controls.Add(this.trackBarWindowsLevel);
            this.groupBox3.Location = new System.Drawing.Point(7, 65);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(182, 72);
            this.groupBox3.TabIndex = 12;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Desktop vibrance";
            // 
            // labelWindowsLevel
            // 
            this.labelWindowsLevel.AutoSize = true;
            this.labelWindowsLevel.Location = new System.Drawing.Point(148, 22);
            this.labelWindowsLevel.Name = "labelWindowsLevel";
            this.labelWindowsLevel.Size = new System.Drawing.Size(0, 13);
            this.labelWindowsLevel.TabIndex = 1;
            // 
            // trackBarWindowsLevel
            // 
            this.trackBarWindowsLevel.Location = new System.Drawing.Point(15, 22);
            this.trackBarWindowsLevel.Maximum = 63;
            this.trackBarWindowsLevel.Name = "trackBarWindowsLevel";
            this.trackBarWindowsLevel.Size = new System.Drawing.Size(131, 45);
            this.trackBarWindowsLevel.TabIndex = 0;
            this.trackBarWindowsLevel.Scroll += new System.EventHandler(this.trackBarWindowsLevel_Scroll);
            // 
            // statusLabel
            // 
            this.statusLabel.AutoSize = false;
            this.statusLabel.Location = new System.Drawing.Point(60, 480);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(346, 36);
            this.statusLabel.TabIndex = 14;
            this.statusLabel.Text = "Initializing...";
            // 
            // observerStatusLabel
            // 
            this.observerStatusLabel.AutoSize = true;
            this.observerStatusLabel.Location = new System.Drawing.Point(12, 480);
            this.observerStatusLabel.Name = "observerStatusLabel";
            this.observerStatusLabel.Size = new System.Drawing.Size(87, 13);
            this.observerStatusLabel.TabIndex = 13;
            this.observerStatusLabel.Text = "Status:";
            // 
            // labelProduct
            // 
            this.labelProduct.AutoSize = true;
            this.labelProduct.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.labelProduct.Location = new System.Drawing.Point(10, 9);
            this.labelProduct.Name = "labelProduct";
            this.labelProduct.Text = AppName;
            // 
            // linkLabelProject
            // 
            this.linkLabelProject.AutoSize = true;
            this.linkLabelProject.Location = new System.Drawing.Point(13, 60);
            this.linkLabelProject.Name = "linkLabelProject";
            this.linkLabelProject.TabIndex = 0;
            this.linkLabelProject.TabStop = true;
            this.linkLabelProject.Text = "Project and help";
            this.toolTip.SetToolTip(this.linkLabelProject, ProjectLink);
            this.linkLabelProject.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelProject_LinkClicked);
            // 
            // settingsBackgroundWorker
            // 
            this.settingsBackgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.settingsBackgroundWorker_DoWork);
            this.settingsBackgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.settingsBackgroundWorker_RunWorkerCompleted);
            // 
            // labelSubtitle
            // 
            this.labelSubtitle.AutoSize = true;
            this.labelSubtitle.Location = new System.Drawing.Point(13, 41);
            this.labelSubtitle.Name = "labelSubtitle";
            this.labelSubtitle.Text = "Automatic vibrance for your games and desktop.";
            // 
            // toolTip
            // 
            this.toolTip.AutoPopDelay = 5000;
            this.toolTip.InitialDelay = 100;
            this.toolTip.IsBalloon = true;
            this.toolTip.ReshowDelay = 100;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.buttonProcessExplorer);
            this.groupBox5.Controls.Add(this.buttonRemoveProgram);
            this.groupBox5.Controls.Add(this.listApplications);
            this.groupBox5.Controls.Add(this.buttonAddProgram);
            this.groupBox5.Location = new System.Drawing.Point(13, 231);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(397, 227);
            this.groupBox5.TabIndex = 18;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Programs - double-click to edit";
            // 
            // buttonProcessExplorer
            // 
            this.buttonProcessExplorer.Location = new System.Drawing.Point(7, 20);
            this.buttonProcessExplorer.Name = "buttonProcessExplorer";
            this.buttonProcessExplorer.Size = new System.Drawing.Size(75, 23);
            this.buttonProcessExplorer.TabIndex = 3;
            this.buttonProcessExplorer.Text = "Add";
            this.buttonProcessExplorer.UseVisualStyleBackColor = true;
            this.buttonProcessExplorer.Click += new System.EventHandler(this.buttonProcessExplorer_Click);
            // 
            // buttonRemoveProgram
            // 
            this.buttonRemoveProgram.Location = new System.Drawing.Point(186, 20);
            this.buttonRemoveProgram.Name = "buttonRemoveProgram";
            this.buttonRemoveProgram.Size = new System.Drawing.Size(75, 23);
            this.buttonRemoveProgram.TabIndex = 2;
            this.buttonRemoveProgram.Text = "Remove";
            this.buttonRemoveProgram.UseVisualStyleBackColor = true;
            this.buttonRemoveProgram.Click += new System.EventHandler(this.buttonRemoveProgram_Click);
            // 
            // listApplications
            // 
            this.listApplications.Location = new System.Drawing.Point(7, 49);
            this.listApplications.Name = "listApplications";
            this.listApplications.Size = new System.Drawing.Size(384, 172);
            this.listApplications.TabIndex = 1;
            this.listApplications.UseCompatibleStateImageBehavior = false;
            this.listApplications.DoubleClick += new System.EventHandler(this.listApplications_DoubleClick);
            // 
            // buttonAddProgram
            // 
            this.buttonAddProgram.Location = new System.Drawing.Point(88, 20);
            this.buttonAddProgram.Name = "buttonAddProgram";
            this.buttonAddProgram.Size = new System.Drawing.Size(92, 23);
            this.buttonAddProgram.TabIndex = 0;
            this.buttonAddProgram.Text = "Add manually";
            this.buttonAddProgram.UseVisualStyleBackColor = true;
            this.buttonAddProgram.Click += new System.EventHandler(this.buttonAddProgram_Click);
            // 
            // VibranceGUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(419, 524);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.labelSubtitle);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.statusLabel);
            this.Controls.Add(this.observerStatusLabel);
            this.Controls.Add(this.labelProduct);
            this.Controls.Add(this.linkLabelProject);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "VibranceGUI";
            this.Text = AppName;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Shown += new System.EventHandler(this.Form1_Shown);
            this.Resize += new System.EventHandler(this.Form1_Resize);
            this.contextMenuStrip.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarWindowsLevel)).EndInit();
            this.groupBox5.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem projectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.ComponentModel.BackgroundWorker backgroundWorker;
        private System.Windows.Forms.CheckBox checkBoxAutostart;
        private System.Windows.Forms.Label labelGraphicsAdapter;
        private System.Windows.Forms.ComboBox comboBoxGraphicsAdapter;
        private System.Windows.Forms.Label labelGraphicsAdapterRestart;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label labelWindowsLevel;
        private System.Windows.Forms.TrackBar trackBarWindowsLevel;
        private System.Windows.Forms.Label statusLabel;
        private System.Windows.Forms.Label observerStatusLabel;
        private System.Windows.Forms.Label labelProduct;
        private System.Windows.Forms.LinkLabel linkLabelProject;
        private System.ComponentModel.BackgroundWorker settingsBackgroundWorker;
        private System.Windows.Forms.Label labelSubtitle;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.CheckBox checkBoxPrimaryMonitorOnly;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Button buttonRemoveProgram;
        private System.Windows.Forms.ListView listApplications;
        private System.Windows.Forms.Button buttonAddProgram;
        private System.Windows.Forms.Button buttonProcessExplorer;
        private System.Windows.Forms.CheckBox checkBoxNeverChangeResolutions;
    }
}

