namespace Services.UI
{
    partial class Main
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.redrawFromReportsTimer = new System.Windows.Forms.Timer(this.components);
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.serverToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.connectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.manageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.installToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.uninstallToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mainTreeView = new System.Windows.Forms.TreeView();
            this.serviceNamePanel = new System.Windows.Forms.Panel();
            this.button6 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.label14 = new System.Windows.Forms.Label();
            this.startWorkUnitButton = new System.Windows.Forms.Button();
            this.versionToStartNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.serviceInfoTextBox = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.serviceNameLabel = new System.Windows.Forms.Label();
            this.welcomePanel = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.serviceProcessPanel = new System.Windows.Forms.Panel();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.infoTabPage = new System.Windows.Forms.TabPage();
            this.changeVersionServiceEntryButton = new System.Windows.Forms.Button();
            this.cmdScriptTextBox = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.cloneButton = new System.Windows.Forms.Button();
            this.restartServiceButton = new System.Windows.Forms.Button();
            this.deleteServiceEntryButton = new System.Windows.Forms.Button();
            this.closeServiceButton = new System.Windows.Forms.Button();
            this.instanceStateLabel = new System.Windows.Forms.Label();
            this.instanceStartDataTextbox = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.instanceParametersTextBox = new System.Windows.Forms.TextBox();
            this.instanceHostLabel = new System.Windows.Forms.Label();
            this.instanceFilepathLabel = new System.Windows.Forms.Label();
            this.instanceIdLabel = new System.Windows.Forms.Label();
            this.instanceTypeLabel = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.timeStartedLabel = new System.Windows.Forms.Label();
            this.timeWorkingLabel = new System.Windows.Forms.Label();
            this.healthTabPage = new System.Windows.Forms.TabPage();
            this.errorsTextBox = new System.Windows.Forms.TextBox();
            this.totalReportedErrorsLabel = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.logTabPage = new System.Windows.Forms.TabPage();
            this.logTextBox = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.serviceProcessIdLabel = new System.Windows.Forms.Label();
            this.statusStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.serviceNamePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.versionToStartNumericUpDown)).BeginInit();
            this.welcomePanel.SuspendLayout();
            this.serviceProcessPanel.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.infoTabPage.SuspendLayout();
            this.healthTabPage.SuspendLayout();
            this.logTabPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // redrawFromReportsTimer
            // 
            this.redrawFromReportsTimer.Tick += new System.EventHandler(this.redrawFromReportsTimer_Tick);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel});
            this.statusStrip1.Location = new System.Drawing.Point(0, 546);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(998, 22);
            this.statusStrip1.TabIndex = 0;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel
            // 
            this.toolStripStatusLabel.Name = "toolStripStatusLabel";
            this.toolStripStatusLabel.Size = new System.Drawing.Size(12, 17);
            this.toolStripStatusLabel.Text = "-";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.serverToolStripMenuItem,
            this.manageToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(998, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // serverToolStripMenuItem
            // 
            this.serverToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.connectToolStripMenuItem});
            this.serverToolStripMenuItem.Name = "serverToolStripMenuItem";
            this.serverToolStripMenuItem.Size = new System.Drawing.Size(51, 20);
            this.serverToolStripMenuItem.Text = "Server";
            // 
            // connectToolStripMenuItem
            // 
            this.connectToolStripMenuItem.Name = "connectToolStripMenuItem";
            this.connectToolStripMenuItem.Size = new System.Drawing.Size(119, 22);
            this.connectToolStripMenuItem.Text = "Connect";
            this.connectToolStripMenuItem.Click += new System.EventHandler(this.connectToolStripMenuItem_Click);
            // 
            // manageToolStripMenuItem
            // 
            this.manageToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.installToolStripMenuItem,
            this.uninstallToolStripMenuItem});
            this.manageToolStripMenuItem.Name = "manageToolStripMenuItem";
            this.manageToolStripMenuItem.Size = new System.Drawing.Size(62, 20);
            this.manageToolStripMenuItem.Text = "Manage";
            // 
            // installToolStripMenuItem
            // 
            this.installToolStripMenuItem.Name = "installToolStripMenuItem";
            this.installToolStripMenuItem.Size = new System.Drawing.Size(120, 22);
            this.installToolStripMenuItem.Text = "Install";
            this.installToolStripMenuItem.Click += new System.EventHandler(this.installToolStripMenuItem_Click);
            // 
            // uninstallToolStripMenuItem
            // 
            this.uninstallToolStripMenuItem.Name = "uninstallToolStripMenuItem";
            this.uninstallToolStripMenuItem.Size = new System.Drawing.Size(120, 22);
            this.uninstallToolStripMenuItem.Text = "Uninstall";
            this.uninstallToolStripMenuItem.Click += new System.EventHandler(this.uninstallToolStripMenuItem_Click);
            // 
            // mainTreeView
            // 
            this.mainTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.mainTreeView.Location = new System.Drawing.Point(0, 27);
            this.mainTreeView.Name = "mainTreeView";
            this.mainTreeView.Size = new System.Drawing.Size(242, 516);
            this.mainTreeView.TabIndex = 2;
            // 
            // serviceNamePanel
            // 
            this.serviceNamePanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.serviceNamePanel.Controls.Add(this.button6);
            this.serviceNamePanel.Controls.Add(this.button5);
            this.serviceNamePanel.Controls.Add(this.button4);
            this.serviceNamePanel.Controls.Add(this.button3);
            this.serviceNamePanel.Controls.Add(this.label14);
            this.serviceNamePanel.Controls.Add(this.startWorkUnitButton);
            this.serviceNamePanel.Controls.Add(this.versionToStartNumericUpDown);
            this.serviceNamePanel.Controls.Add(this.serviceInfoTextBox);
            this.serviceNamePanel.Controls.Add(this.label13);
            this.serviceNamePanel.Controls.Add(this.serviceNameLabel);
            this.serviceNamePanel.Location = new System.Drawing.Point(248, 27);
            this.serviceNamePanel.Name = "serviceNamePanel";
            this.serviceNamePanel.Size = new System.Drawing.Size(750, 516);
            this.serviceNamePanel.TabIndex = 3;
            this.serviceNamePanel.Visible = false;
            // 
            // button6
            // 
            this.button6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button6.Enabled = false;
            this.button6.Location = new System.Drawing.Point(515, 249);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(231, 23);
            this.button6.TabIndex = 12;
            this.button6.Text = "Change version";
            this.button6.UseVisualStyleBackColor = true;
            // 
            // button5
            // 
            this.button5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button5.Enabled = false;
            this.button5.Location = new System.Drawing.Point(515, 220);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(231, 23);
            this.button5.TabIndex = 11;
            this.button5.Text = "Close all";
            this.button5.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            this.button4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button4.Enabled = false;
            this.button4.Location = new System.Drawing.Point(515, 191);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(231, 23);
            this.button4.TabIndex = 10;
            this.button4.Text = "Restart all";
            this.button4.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            this.button3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button3.Enabled = false;
            this.button3.Location = new System.Drawing.Point(515, 162);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(231, 23);
            this.button3.TabIndex = 9;
            this.button3.Text = "Start an updater";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // label14
            // 
            this.label14.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(515, 120);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(183, 13);
            this.label14.TabIndex = 7;
            this.label14.Text = "Start a new working unit with version:";
            // 
            // startWorkUnitButton
            // 
            this.startWorkUnitButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.startWorkUnitButton.Location = new System.Drawing.Point(671, 134);
            this.startWorkUnitButton.Name = "startWorkUnitButton";
            this.startWorkUnitButton.Size = new System.Drawing.Size(75, 23);
            this.startWorkUnitButton.TabIndex = 4;
            this.startWorkUnitButton.Text = "Start";
            this.startWorkUnitButton.UseVisualStyleBackColor = true;
            this.startWorkUnitButton.Click += new System.EventHandler(this.startWorkUnitButton_Click);
            // 
            // versionToStartNumericUpDown
            // 
            this.versionToStartNumericUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.versionToStartNumericUpDown.Location = new System.Drawing.Point(515, 136);
            this.versionToStartNumericUpDown.Name = "versionToStartNumericUpDown";
            this.versionToStartNumericUpDown.Size = new System.Drawing.Size(149, 20);
            this.versionToStartNumericUpDown.TabIndex = 3;
            // 
            // serviceInfoTextBox
            // 
            this.serviceInfoTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.serviceInfoTextBox.Location = new System.Drawing.Point(24, 107);
            this.serviceInfoTextBox.Multiline = true;
            this.serviceInfoTextBox.Name = "serviceInfoTextBox";
            this.serviceInfoTextBox.ReadOnly = true;
            this.serviceInfoTextBox.Size = new System.Drawing.Size(485, 402);
            this.serviceInfoTextBox.TabIndex = 2;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.Location = new System.Drawing.Point(19, 73);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(190, 30);
            this.label13.TabIndex = 1;
            this.label13.Text = "Info for this service";
            // 
            // serviceNameLabel
            // 
            this.serviceNameLabel.AutoSize = true;
            this.serviceNameLabel.Font = new System.Drawing.Font("Segoe UI", 27.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.serviceNameLabel.Location = new System.Drawing.Point(15, 14);
            this.serviceNameLabel.Name = "serviceNameLabel";
            this.serviceNameLabel.Size = new System.Drawing.Size(317, 50);
            this.serviceNameLabel.TabIndex = 0;
            this.serviceNameLabel.Text = "serviceNameLabel";
            // 
            // welcomePanel
            // 
            this.welcomePanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.welcomePanel.Controls.Add(this.label5);
            this.welcomePanel.Location = new System.Drawing.Point(248, 27);
            this.welcomePanel.Name = "welcomePanel";
            this.welcomePanel.Size = new System.Drawing.Size(750, 516);
            this.welcomePanel.TabIndex = 5;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Segoe UI", 27.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(15, 14);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(448, 50);
            this.label5.TabIndex = 0;
            this.label5.Text = "Welcome to admin viewer";
            // 
            // serviceProcessPanel
            // 
            this.serviceProcessPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.serviceProcessPanel.Controls.Add(this.tabControl1);
            this.serviceProcessPanel.Controls.Add(this.serviceProcessIdLabel);
            this.serviceProcessPanel.Location = new System.Drawing.Point(248, 27);
            this.serviceProcessPanel.Name = "serviceProcessPanel";
            this.serviceProcessPanel.Size = new System.Drawing.Size(750, 516);
            this.serviceProcessPanel.TabIndex = 5;
            this.serviceProcessPanel.Visible = false;
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.infoTabPage);
            this.tabControl1.Controls.Add(this.healthTabPage);
            this.tabControl1.Controls.Add(this.logTabPage);
            this.tabControl1.Location = new System.Drawing.Point(24, 67);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(726, 449);
            this.tabControl1.TabIndex = 1;
            // 
            // infoTabPage
            // 
            this.infoTabPage.Controls.Add(this.changeVersionServiceEntryButton);
            this.infoTabPage.Controls.Add(this.cmdScriptTextBox);
            this.infoTabPage.Controls.Add(this.label6);
            this.infoTabPage.Controls.Add(this.cloneButton);
            this.infoTabPage.Controls.Add(this.restartServiceButton);
            this.infoTabPage.Controls.Add(this.deleteServiceEntryButton);
            this.infoTabPage.Controls.Add(this.closeServiceButton);
            this.infoTabPage.Controls.Add(this.instanceStateLabel);
            this.infoTabPage.Controls.Add(this.instanceStartDataTextbox);
            this.infoTabPage.Controls.Add(this.label12);
            this.infoTabPage.Controls.Add(this.instanceParametersTextBox);
            this.infoTabPage.Controls.Add(this.instanceHostLabel);
            this.infoTabPage.Controls.Add(this.instanceFilepathLabel);
            this.infoTabPage.Controls.Add(this.instanceIdLabel);
            this.infoTabPage.Controls.Add(this.instanceTypeLabel);
            this.infoTabPage.Controls.Add(this.label11);
            this.infoTabPage.Controls.Add(this.label10);
            this.infoTabPage.Controls.Add(this.label9);
            this.infoTabPage.Controls.Add(this.label8);
            this.infoTabPage.Controls.Add(this.label1);
            this.infoTabPage.Controls.Add(this.label2);
            this.infoTabPage.Controls.Add(this.timeStartedLabel);
            this.infoTabPage.Controls.Add(this.timeWorkingLabel);
            this.infoTabPage.Location = new System.Drawing.Point(4, 22);
            this.infoTabPage.Name = "infoTabPage";
            this.infoTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.infoTabPage.Size = new System.Drawing.Size(718, 423);
            this.infoTabPage.TabIndex = 0;
            this.infoTabPage.Text = "Info";
            this.infoTabPage.UseVisualStyleBackColor = true;
            // 
            // changeVersionServiceEntryButton
            // 
            this.changeVersionServiceEntryButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.changeVersionServiceEntryButton.Location = new System.Drawing.Point(556, 379);
            this.changeVersionServiceEntryButton.Name = "changeVersionServiceEntryButton";
            this.changeVersionServiceEntryButton.Size = new System.Drawing.Size(159, 23);
            this.changeVersionServiceEntryButton.TabIndex = 30;
            this.changeVersionServiceEntryButton.Text = "Change version";
            this.changeVersionServiceEntryButton.UseVisualStyleBackColor = true;
            this.changeVersionServiceEntryButton.Click += new System.EventHandler(this.changeVersionServiceEntryButton_Click);
            // 
            // cmdScriptTextBox
            // 
            this.cmdScriptTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdScriptTextBox.Location = new System.Drawing.Point(128, 324);
            this.cmdScriptTextBox.Name = "cmdScriptTextBox";
            this.cmdScriptTextBox.ReadOnly = true;
            this.cmdScriptTextBox.Size = new System.Drawing.Size(582, 20);
            this.cmdScriptTextBox.TabIndex = 29;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(5, 327);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(59, 13);
            this.label6.TabIndex = 28;
            this.label6.Text = "Cmd script:";
            // 
            // cloneButton
            // 
            this.cloneButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cloneButton.Location = new System.Drawing.Point(556, 350);
            this.cloneButton.Name = "cloneButton";
            this.cloneButton.Size = new System.Drawing.Size(75, 23);
            this.cloneButton.TabIndex = 27;
            this.cloneButton.Text = "Clone";
            this.cloneButton.UseVisualStyleBackColor = true;
            this.cloneButton.Click += new System.EventHandler(this.cloneButton_Click);
            // 
            // restartServiceButton
            // 
            this.restartServiceButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.restartServiceButton.Location = new System.Drawing.Point(475, 350);
            this.restartServiceButton.Name = "restartServiceButton";
            this.restartServiceButton.Size = new System.Drawing.Size(75, 23);
            this.restartServiceButton.TabIndex = 26;
            this.restartServiceButton.Text = "Restart";
            this.restartServiceButton.UseVisualStyleBackColor = true;
            this.restartServiceButton.Click += new System.EventHandler(this.restartServiceButton_Click);
            // 
            // deleteServiceEntryButton
            // 
            this.deleteServiceEntryButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.deleteServiceEntryButton.Location = new System.Drawing.Point(394, 350);
            this.deleteServiceEntryButton.Name = "deleteServiceEntryButton";
            this.deleteServiceEntryButton.Size = new System.Drawing.Size(75, 23);
            this.deleteServiceEntryButton.TabIndex = 25;
            this.deleteServiceEntryButton.Text = "Delete";
            this.deleteServiceEntryButton.UseVisualStyleBackColor = true;
            this.deleteServiceEntryButton.Click += new System.EventHandler(this.deleteServiceEntryButton_Click);
            // 
            // closeServiceButton
            // 
            this.closeServiceButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.closeServiceButton.Location = new System.Drawing.Point(637, 350);
            this.closeServiceButton.Name = "closeServiceButton";
            this.closeServiceButton.Size = new System.Drawing.Size(75, 23);
            this.closeServiceButton.TabIndex = 24;
            this.closeServiceButton.Text = "Close";
            this.closeServiceButton.UseVisualStyleBackColor = true;
            this.closeServiceButton.Click += new System.EventHandler(this.closeServiceButton_Click);
            // 
            // instanceStateLabel
            // 
            this.instanceStateLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.instanceStateLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.instanceStateLabel.Location = new System.Drawing.Point(563, 14);
            this.instanceStateLabel.Name = "instanceStateLabel";
            this.instanceStateLabel.Size = new System.Drawing.Size(147, 20);
            this.instanceStateLabel.TabIndex = 23;
            this.instanceStateLabel.Text = "instanceStateLabel";
            this.instanceStateLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // instanceStartDataTextbox
            // 
            this.instanceStartDataTextbox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.instanceStartDataTextbox.Location = new System.Drawing.Point(128, 203);
            this.instanceStartDataTextbox.Multiline = true;
            this.instanceStartDataTextbox.Name = "instanceStartDataTextbox";
            this.instanceStartDataTextbox.ReadOnly = true;
            this.instanceStartDataTextbox.Size = new System.Drawing.Size(582, 115);
            this.instanceStartDataTextbox.TabIndex = 22;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(7, 206);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(68, 13);
            this.label12.TabIndex = 21;
            this.label12.Text = "Started data:";
            // 
            // instanceParametersTextBox
            // 
            this.instanceParametersTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.instanceParametersTextBox.Location = new System.Drawing.Point(130, 132);
            this.instanceParametersTextBox.Name = "instanceParametersTextBox";
            this.instanceParametersTextBox.ReadOnly = true;
            this.instanceParametersTextBox.Size = new System.Drawing.Size(582, 20);
            this.instanceParametersTextBox.TabIndex = 20;
            // 
            // instanceHostLabel
            // 
            this.instanceHostLabel.AutoSize = true;
            this.instanceHostLabel.Location = new System.Drawing.Point(127, 182);
            this.instanceHostLabel.Name = "instanceHostLabel";
            this.instanceHostLabel.Size = new System.Drawing.Size(10, 13);
            this.instanceHostLabel.TabIndex = 19;
            this.instanceHostLabel.Text = "-";
            // 
            // instanceFilepathLabel
            // 
            this.instanceFilepathLabel.AutoSize = true;
            this.instanceFilepathLabel.Location = new System.Drawing.Point(127, 160);
            this.instanceFilepathLabel.Name = "instanceFilepathLabel";
            this.instanceFilepathLabel.Size = new System.Drawing.Size(10, 13);
            this.instanceFilepathLabel.TabIndex = 18;
            this.instanceFilepathLabel.Text = "-";
            // 
            // instanceIdLabel
            // 
            this.instanceIdLabel.AutoSize = true;
            this.instanceIdLabel.Location = new System.Drawing.Point(127, 112);
            this.instanceIdLabel.Name = "instanceIdLabel";
            this.instanceIdLabel.Size = new System.Drawing.Size(10, 13);
            this.instanceIdLabel.TabIndex = 16;
            this.instanceIdLabel.Text = "-";
            // 
            // instanceTypeLabel
            // 
            this.instanceTypeLabel.AutoSize = true;
            this.instanceTypeLabel.Location = new System.Drawing.Point(127, 88);
            this.instanceTypeLabel.Name = "instanceTypeLabel";
            this.instanceTypeLabel.Size = new System.Drawing.Size(10, 13);
            this.instanceTypeLabel.TabIndex = 15;
            this.instanceTypeLabel.Text = "-";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(7, 182);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(72, 13);
            this.label11.TabIndex = 14;
            this.label11.Text = "Host address:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(7, 160);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(50, 13);
            this.label10.TabIndex = 13;
            this.label10.Text = "File path:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(7, 135);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(63, 13);
            this.label9.TabIndex = 12;
            this.label9.Text = "Parameters:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(7, 112);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(19, 13);
            this.label8.TabIndex = 11;
            this.label8.Text = "Id:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 88);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(34, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "Type:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 54);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Started on:";
            // 
            // timeStartedLabel
            // 
            this.timeStartedLabel.AutoSize = true;
            this.timeStartedLabel.Location = new System.Drawing.Point(127, 54);
            this.timeStartedLabel.Name = "timeStartedLabel";
            this.timeStartedLabel.Size = new System.Drawing.Size(86, 13);
            this.timeStartedLabel.TabIndex = 6;
            this.timeStartedLabel.Text = "timeStartedLabel";
            // 
            // timeWorkingLabel
            // 
            this.timeWorkingLabel.AutoSize = true;
            this.timeWorkingLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.timeWorkingLabel.Location = new System.Drawing.Point(6, 14);
            this.timeWorkingLabel.Name = "timeWorkingLabel";
            this.timeWorkingLabel.Size = new System.Drawing.Size(136, 20);
            this.timeWorkingLabel.TabIndex = 5;
            this.timeWorkingLabel.Text = "timeWorkingLabel";
            // 
            // healthTabPage
            // 
            this.healthTabPage.Controls.Add(this.errorsTextBox);
            this.healthTabPage.Controls.Add(this.totalReportedErrorsLabel);
            this.healthTabPage.Controls.Add(this.label4);
            this.healthTabPage.Controls.Add(this.label3);
            this.healthTabPage.Location = new System.Drawing.Point(4, 22);
            this.healthTabPage.Name = "healthTabPage";
            this.healthTabPage.Size = new System.Drawing.Size(718, 423);
            this.healthTabPage.TabIndex = 2;
            this.healthTabPage.Text = "Health";
            this.healthTabPage.UseVisualStyleBackColor = true;
            // 
            // errorsTextBox
            // 
            this.errorsTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.errorsTextBox.Location = new System.Drawing.Point(130, 60);
            this.errorsTextBox.Multiline = true;
            this.errorsTextBox.Name = "errorsTextBox";
            this.errorsTextBox.ReadOnly = true;
            this.errorsTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.errorsTextBox.Size = new System.Drawing.Size(585, 360);
            this.errorsTextBox.TabIndex = 13;
            // 
            // totalReportedErrorsLabel
            // 
            this.totalReportedErrorsLabel.AutoSize = true;
            this.totalReportedErrorsLabel.Location = new System.Drawing.Point(127, 15);
            this.totalReportedErrorsLabel.Name = "totalReportedErrorsLabel";
            this.totalReportedErrorsLabel.Size = new System.Drawing.Size(10, 13);
            this.totalReportedErrorsLabel.TabIndex = 12;
            this.totalReportedErrorsLabel.Text = "-";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 15);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(105, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "Total reported errors:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 63);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(49, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Error log:";
            // 
            // logTabPage
            // 
            this.logTabPage.Controls.Add(this.logTextBox);
            this.logTabPage.Controls.Add(this.label7);
            this.logTabPage.Location = new System.Drawing.Point(4, 22);
            this.logTabPage.Name = "logTabPage";
            this.logTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.logTabPage.Size = new System.Drawing.Size(718, 423);
            this.logTabPage.TabIndex = 1;
            this.logTabPage.Text = "Log";
            this.logTabPage.UseVisualStyleBackColor = true;
            // 
            // logTextBox
            // 
            this.logTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.logTextBox.Location = new System.Drawing.Point(127, 6);
            this.logTextBox.Multiline = true;
            this.logTextBox.Name = "logTextBox";
            this.logTextBox.ReadOnly = true;
            this.logTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.logTextBox.Size = new System.Drawing.Size(585, 414);
            this.logTextBox.TabIndex = 15;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(4, 9);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(119, 13);
            this.label7.TabIndex = 14;
            this.label7.Text = "Log entries from service";
            // 
            // serviceProcessIdLabel
            // 
            this.serviceProcessIdLabel.AutoSize = true;
            this.serviceProcessIdLabel.Font = new System.Drawing.Font("Segoe UI", 27.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.serviceProcessIdLabel.Location = new System.Drawing.Point(15, 14);
            this.serviceProcessIdLabel.Name = "serviceProcessIdLabel";
            this.serviceProcessIdLabel.Size = new System.Drawing.Size(375, 50);
            this.serviceProcessIdLabel.TabIndex = 0;
            this.serviceProcessIdLabel.Text = "serviceProcessIdLabel";
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(998, 568);
            this.Controls.Add(this.mainTreeView);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.serviceNamePanel);
            this.Controls.Add(this.welcomePanel);
            this.Controls.Add(this.serviceProcessPanel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Administration of services";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Main_FormClosed);
            this.Load += new System.EventHandler(this.Main_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.serviceNamePanel.ResumeLayout(false);
            this.serviceNamePanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.versionToStartNumericUpDown)).EndInit();
            this.welcomePanel.ResumeLayout(false);
            this.welcomePanel.PerformLayout();
            this.serviceProcessPanel.ResumeLayout(false);
            this.serviceProcessPanel.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.infoTabPage.ResumeLayout(false);
            this.infoTabPage.PerformLayout();
            this.healthTabPage.ResumeLayout(false);
            this.healthTabPage.PerformLayout();
            this.logTabPage.ResumeLayout(false);
            this.logTabPage.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer redrawFromReportsTimer;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem serverToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem connectToolStripMenuItem;
        private System.Windows.Forms.TreeView mainTreeView;
        private System.Windows.Forms.Panel serviceNamePanel;
        private System.Windows.Forms.Label serviceNameLabel;
        private System.Windows.Forms.Panel welcomePanel;
        private System.Windows.Forms.Panel serviceProcessPanel;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label serviceProcessIdLabel;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage infoTabPage;
        private System.Windows.Forms.TabPage logTabPage;
        private System.Windows.Forms.TabPage healthTabPage;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label timeStartedLabel;
        private System.Windows.Forms.Label timeWorkingLabel;
        private System.Windows.Forms.Label totalReportedErrorsLabel;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox errorsTextBox;
        private System.Windows.Forms.TextBox logTextBox;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label instanceHostLabel;
        private System.Windows.Forms.Label instanceFilepathLabel;
        private System.Windows.Forms.Label instanceIdLabel;
        private System.Windows.Forms.Label instanceTypeLabel;
        private System.Windows.Forms.TextBox instanceParametersTextBox;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox instanceStartDataTextbox;
        private System.Windows.Forms.Label instanceStateLabel;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox serviceInfoTextBox;
        private System.Windows.Forms.Button startWorkUnitButton;
        private System.Windows.Forms.NumericUpDown versionToStartNumericUpDown;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button closeServiceButton;
        private System.Windows.Forms.Button deleteServiceEntryButton;
        private System.Windows.Forms.Button restartServiceButton;
        private System.Windows.Forms.ToolStripMenuItem manageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem installToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem uninstallToolStripMenuItem;
        private System.Windows.Forms.Button cloneButton;
        private System.Windows.Forms.TextBox cmdScriptTextBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button changeVersionServiceEntryButton;
    }
}

