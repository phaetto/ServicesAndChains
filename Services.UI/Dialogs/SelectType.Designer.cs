namespace Services.UI.Dialogs
{
    partial class SelectType
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
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.typeComboBox = new System.Windows.Forms.ComboBox();
            this.autoIdCheckBox = new System.Windows.Forms.CheckBox();
            this.idTextBox = new System.Windows.Forms.TextBox();
            this.servedCheckBox = new System.Windows.Forms.CheckBox();
            this.servedHostTextBox = new System.Windows.Forms.TextBox();
            this.servedPortNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.parametersTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.modulesTextBox = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.servedPortNumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button2.Location = new System.Drawing.Point(408, 284);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 8;
            this.button2.Text = "Cancel";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button1.Location = new System.Drawing.Point(489, 284);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(108, 23);
            this.button1.TabIndex = 7;
            this.button1.Text = "Select Type";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 27.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 50);
            this.label1.TabIndex = 6;
            this.label1.Text = "Type";
            // 
            // typeComboBox
            // 
            this.typeComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.typeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.typeComboBox.FormattingEnabled = true;
            this.typeComboBox.Location = new System.Drawing.Point(12, 125);
            this.typeComboBox.Name = "typeComboBox";
            this.typeComboBox.Size = new System.Drawing.Size(585, 21);
            this.typeComboBox.Sorted = true;
            this.typeComboBox.TabIndex = 5;
            // 
            // autoIdCheckBox
            // 
            this.autoIdCheckBox.AutoSize = true;
            this.autoIdCheckBox.Checked = true;
            this.autoIdCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.autoIdCheckBox.Location = new System.Drawing.Point(12, 88);
            this.autoIdCheckBox.Name = "autoIdCheckBox";
            this.autoIdCheckBox.Size = new System.Drawing.Size(60, 17);
            this.autoIdCheckBox.TabIndex = 9;
            this.autoIdCheckBox.Text = "Auto Id";
            this.autoIdCheckBox.UseVisualStyleBackColor = true;
            this.autoIdCheckBox.CheckedChanged += new System.EventHandler(this.autoIdCheckBox_CheckedChanged);
            // 
            // idTextBox
            // 
            this.idTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.idTextBox.Enabled = false;
            this.idTextBox.Location = new System.Drawing.Point(78, 86);
            this.idTextBox.Name = "idTextBox";
            this.idTextBox.Size = new System.Drawing.Size(519, 20);
            this.idTextBox.TabIndex = 10;
            // 
            // servedCheckBox
            // 
            this.servedCheckBox.AutoSize = true;
            this.servedCheckBox.Location = new System.Drawing.Point(12, 249);
            this.servedCheckBox.Name = "servedCheckBox";
            this.servedCheckBox.Size = new System.Drawing.Size(111, 17);
            this.servedCheckBox.TabIndex = 11;
            this.servedCheckBox.Text = "Serve through tcp";
            this.servedCheckBox.UseVisualStyleBackColor = true;
            this.servedCheckBox.CheckedChanged += new System.EventHandler(this.servedCheckBox_CheckedChanged);
            // 
            // servedHostTextBox
            // 
            this.servedHostTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.servedHostTextBox.Enabled = false;
            this.servedHostTextBox.Location = new System.Drawing.Point(135, 246);
            this.servedHostTextBox.Name = "servedHostTextBox";
            this.servedHostTextBox.Size = new System.Drawing.Size(336, 20);
            this.servedHostTextBox.TabIndex = 12;
            this.servedHostTextBox.Text = "0.0.0.0";
            // 
            // servedPortNumericUpDown
            // 
            this.servedPortNumericUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.servedPortNumericUpDown.Enabled = false;
            this.servedPortNumericUpDown.Location = new System.Drawing.Point(477, 247);
            this.servedPortNumericUpDown.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.servedPortNumericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.servedPortNumericUpDown.Name = "servedPortNumericUpDown";
            this.servedPortNumericUpDown.Size = new System.Drawing.Size(120, 20);
            this.servedPortNumericUpDown.TabIndex = 13;
            this.servedPortNumericUpDown.Value = new decimal(new int[] {
            12001,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(18, 170);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(132, 13);
            this.label2.TabIndex = 14;
            this.label2.Text = "Parameters (JSON format):";
            // 
            // parametersTextBox
            // 
            this.parametersTextBox.Location = new System.Drawing.Point(177, 167);
            this.parametersTextBox.Name = "parametersTextBox";
            this.parametersTextBox.Size = new System.Drawing.Size(380, 20);
            this.parametersTextBox.TabIndex = 15;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(156, 162);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(15, 24);
            this.label3.TabIndex = 16;
            this.label3.Text = "[";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(563, 162);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(15, 24);
            this.label4.TabIndex = 17;
            this.label4.Text = "]";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(563, 202);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(15, 24);
            this.label5.TabIndex = 21;
            this.label5.Text = "]";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(156, 202);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(15, 24);
            this.label6.TabIndex = 20;
            this.label6.Text = "[";
            // 
            // modulesTextBox
            // 
            this.modulesTextBox.Location = new System.Drawing.Point(177, 207);
            this.modulesTextBox.Name = "modulesTextBox";
            this.modulesTextBox.Size = new System.Drawing.Size(380, 20);
            this.modulesTextBox.TabIndex = 19;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(18, 210);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(119, 13);
            this.label7.TabIndex = 18;
            this.label7.Text = "Modules (JSON format):";
            // 
            // SelectType
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(609, 322);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.modulesTextBox);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.parametersTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.servedPortNumericUpDown);
            this.Controls.Add(this.servedHostTextBox);
            this.Controls.Add(this.servedCheckBox);
            this.Controls.Add(this.idTextBox);
            this.Controls.Add(this.autoIdCheckBox);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.typeComboBox);
            this.Name = "SelectType";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "SelectType";
            this.Load += new System.EventHandler(this.SelectType_Load);
            ((System.ComponentModel.ISupportInitialize)(this.servedPortNumericUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox typeComboBox;
        private System.Windows.Forms.CheckBox autoIdCheckBox;
        private System.Windows.Forms.TextBox idTextBox;
        private System.Windows.Forms.CheckBox servedCheckBox;
        private System.Windows.Forms.TextBox servedHostTextBox;
        private System.Windows.Forms.NumericUpDown servedPortNumericUpDown;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox parametersTextBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox modulesTextBox;
        private System.Windows.Forms.Label label7;
    }
}