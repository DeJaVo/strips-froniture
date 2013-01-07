namespace UI
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.drawFurnituresDestCheckBox = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.furDestHeightCombo = new System.Windows.Forms.ComboBox();
            this.furDestWidthCombo = new System.Windows.Forms.ComboBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.furDestXCombo = new System.Windows.Forms.ComboBox();
            this.furDestYCombo = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.furStartHeightCombo = new System.Windows.Forms.ComboBox();
            this.furStartWidthCombo = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.furStartXCombo = new System.Windows.Forms.ComboBox();
            this.furStartYCombo = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.resetButton = new System.Windows.Forms.Button();
            this.createFurnitureButton = new System.Windows.Forms.Button();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.pauseButton = new System.Windows.Forms.Button();
            this.nextStepButton = new System.Windows.Forms.Button();
            this.runButton = new System.Windows.Forms.Button();
            this.operationsStack = new System.Windows.Forms.ListBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.drawFurnituresDestCheckBox);
            this.splitContainer1.Panel1.Controls.Add(this.groupBox2);
            this.splitContainer1.Panel1.Controls.Add(this.groupBox3);
            this.splitContainer1.Panel1.Controls.Add(this.resetButton);
            this.splitContainer1.Panel1.Controls.Add(this.createFurnitureButton);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(1362, 607);
            this.splitContainer1.SplitterDistance = 257;
            this.splitContainer1.TabIndex = 0;
            // 
            // drawFurnituresDestCheckBox
            // 
            this.drawFurnituresDestCheckBox.AutoSize = true;
            this.drawFurnituresDestCheckBox.Checked = true;
            this.drawFurnituresDestCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.drawFurnituresDestCheckBox.Location = new System.Drawing.Point(72, 523);
            this.drawFurnituresDestCheckBox.Name = "drawFurnituresDestCheckBox";
            this.drawFurnituresDestCheckBox.Size = new System.Drawing.Size(161, 17);
            this.drawFurnituresDestCheckBox.TabIndex = 24;
            this.drawFurnituresDestCheckBox.Text = "Draw Furnitures Destinations";
            this.drawFurnituresDestCheckBox.UseVisualStyleBackColor = true;
            this.drawFurnituresDestCheckBox.CheckedChanged += new System.EventHandler(this.drawFurnituresDestCheckBox_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.furDestHeightCombo);
            this.groupBox2.Controls.Add(this.furDestWidthCombo);
            this.groupBox2.Controls.Add(this.groupBox4);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Location = new System.Drawing.Point(15, 228);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(230, 208);
            this.groupBox2.TabIndex = 14;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "End";
            // 
            // furDestHeightCombo
            // 
            this.furDestHeightCombo.FormattingEnabled = true;
            this.furDestHeightCombo.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11"});
            this.furDestHeightCombo.Location = new System.Drawing.Point(82, 64);
            this.furDestHeightCombo.Name = "furDestHeightCombo";
            this.furDestHeightCombo.Size = new System.Drawing.Size(121, 21);
            this.furDestHeightCombo.TabIndex = 5;
            // 
            // furDestWidthCombo
            // 
            this.furDestWidthCombo.FormattingEnabled = true;
            this.furDestWidthCombo.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10"});
            this.furDestWidthCombo.Location = new System.Drawing.Point(82, 29);
            this.furDestWidthCombo.Name = "furDestWidthCombo";
            this.furDestWidthCombo.Size = new System.Drawing.Size(121, 21);
            this.furDestWidthCombo.TabIndex = 4;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.furDestXCombo);
            this.groupBox4.Controls.Add(this.furDestYCombo);
            this.groupBox4.Controls.Add(this.label5);
            this.groupBox4.Controls.Add(this.label6);
            this.groupBox4.Location = new System.Drawing.Point(22, 100);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(200, 100);
            this.groupBox4.TabIndex = 10;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Position";
            // 
            // furDestXCombo
            // 
            this.furDestXCombo.FormattingEnabled = true;
            this.furDestXCombo.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "12",
            "13",
            "14",
            "15",
            "16",
            "17",
            "18",
            "19",
            "20"});
            this.furDestXCombo.Location = new System.Drawing.Point(86, 59);
            this.furDestXCombo.Name = "furDestXCombo";
            this.furDestXCombo.Size = new System.Drawing.Size(108, 21);
            this.furDestXCombo.TabIndex = 7;
            // 
            // furDestYCombo
            // 
            this.furDestYCombo.FormattingEnabled = true;
            this.furDestYCombo.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11"});
            this.furDestYCombo.Location = new System.Drawing.Point(86, 24);
            this.furDestYCombo.Name = "furDestYCombo";
            this.furDestYCombo.Size = new System.Drawing.Size(108, 21);
            this.furDestYCombo.TabIndex = 6;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(19, 62);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(48, 13);
            this.label5.TabIndex = 3;
            this.label5.Text = "Column :";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(19, 27);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(35, 13);
            this.label6.TabIndex = 2;
            this.label6.Text = "Row :";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(19, 32);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(41, 13);
            this.label7.TabIndex = 0;
            this.label7.Text = "Width :";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(19, 67);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(44, 13);
            this.label8.TabIndex = 1;
            this.label8.Text = "Height :";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.furStartHeightCombo);
            this.groupBox3.Controls.Add(this.furStartWidthCombo);
            this.groupBox3.Controls.Add(this.groupBox1);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Location = new System.Drawing.Point(15, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(230, 210);
            this.groupBox3.TabIndex = 0;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Start";
            // 
            // furStartHeightCombo
            // 
            this.furStartHeightCombo.FormattingEnabled = true;
            this.furStartHeightCombo.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11"});
            this.furStartHeightCombo.Location = new System.Drawing.Point(76, 67);
            this.furStartHeightCombo.Name = "furStartHeightCombo";
            this.furStartHeightCombo.Size = new System.Drawing.Size(121, 21);
            this.furStartHeightCombo.TabIndex = 1;
            this.furStartHeightCombo.TextUpdate += new System.EventHandler(this.furStartHeightCombo_TextUpdate);
            // 
            // furStartWidthCombo
            // 
            this.furStartWidthCombo.FormattingEnabled = true;
            this.furStartWidthCombo.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10"});
            this.furStartWidthCombo.Location = new System.Drawing.Point(76, 29);
            this.furStartWidthCombo.Name = "furStartWidthCombo";
            this.furStartWidthCombo.Size = new System.Drawing.Size(121, 21);
            this.furStartWidthCombo.TabIndex = 0;
            this.furStartWidthCombo.TextUpdate += new System.EventHandler(this.furStartWidthCombo_TextUpdate);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.furStartXCombo);
            this.groupBox1.Controls.Add(this.furStartYCombo);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Location = new System.Drawing.Point(22, 100);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(200, 100);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Position";
            // 
            // furStartXCombo
            // 
            this.furStartXCombo.FormattingEnabled = true;
            this.furStartXCombo.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "12",
            "13",
            "14",
            "15",
            "16",
            "17",
            "18",
            "19",
            "20"});
            this.furStartXCombo.Location = new System.Drawing.Point(73, 59);
            this.furStartXCombo.Name = "furStartXCombo";
            this.furStartXCombo.Size = new System.Drawing.Size(108, 21);
            this.furStartXCombo.TabIndex = 3;
            // 
            // furStartYCombo
            // 
            this.furStartYCombo.FormattingEnabled = true;
            this.furStartYCombo.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11"});
            this.furStartYCombo.Location = new System.Drawing.Point(73, 19);
            this.furStartYCombo.Name = "furStartYCombo";
            this.furStartYCombo.Size = new System.Drawing.Size(108, 21);
            this.furStartYCombo.TabIndex = 2;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(19, 62);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(48, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Column :";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(19, 27);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Row :";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(19, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Width :";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(19, 67);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(44, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Height :";
            // 
            // resetButton
            // 
            this.resetButton.Location = new System.Drawing.Point(72, 482);
            this.resetButton.Name = "resetButton";
            this.resetButton.Size = new System.Drawing.Size(119, 23);
            this.resetButton.TabIndex = 23;
            this.resetButton.Text = "Reset";
            this.resetButton.UseVisualStyleBackColor = true;
            this.resetButton.Click += new System.EventHandler(this.resetButton_Click);
            // 
            // createFurnitureButton
            // 
            this.createFurnitureButton.Location = new System.Drawing.Point(72, 453);
            this.createFurnitureButton.Name = "createFurnitureButton";
            this.createFurnitureButton.Size = new System.Drawing.Size(119, 23);
            this.createFurnitureButton.TabIndex = 20;
            this.createFurnitureButton.Text = "Create Furniture";
            this.createFurnitureButton.UseVisualStyleBackColor = true;
            this.createFurnitureButton.Click += new System.EventHandler(this.createFurnitureButton_Click);
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.splitContainer3);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.operationsStack);
            this.splitContainer2.Size = new System.Drawing.Size(1101, 607);
            this.splitContainer2.SplitterDistance = 795;
            this.splitContainer2.TabIndex = 0;
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.Location = new System.Drawing.Point(0, 0);
            this.splitContainer3.Name = "splitContainer3";
            this.splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.pauseButton);
            this.splitContainer3.Panel2.Controls.Add(this.nextStepButton);
            this.splitContainer3.Panel2.Controls.Add(this.runButton);
            this.splitContainer3.Size = new System.Drawing.Size(795, 607);
            this.splitContainer3.SplitterDistance = 556;
            this.splitContainer3.TabIndex = 0;
            // 
            // pauseButton
            // 
            this.pauseButton.Enabled = false;
            this.pauseButton.Image = global::UI.Properties.Resources.pause;
            this.pauseButton.Location = new System.Drawing.Point(403, 5);
            this.pauseButton.Name = "pauseButton";
            this.pauseButton.Size = new System.Drawing.Size(35, 35);
            this.pauseButton.TabIndex = 5;
            this.pauseButton.UseVisualStyleBackColor = true;
            this.pauseButton.Click += new System.EventHandler(this.pauseButton_Click);
            // 
            // nextStepButton
            // 
            this.nextStepButton.Enabled = false;
            this.nextStepButton.Image = global::UI.Properties.Resources.next;
            this.nextStepButton.Location = new System.Drawing.Point(466, 5);
            this.nextStepButton.Name = "nextStepButton";
            this.nextStepButton.Size = new System.Drawing.Size(35, 35);
            this.nextStepButton.TabIndex = 4;
            this.nextStepButton.UseVisualStyleBackColor = true;
            this.nextStepButton.Click += new System.EventHandler(this.nextStepButton_Click);
            // 
            // runButton
            // 
            this.runButton.Enabled = false;
            this.runButton.Image = global::UI.Properties.Resources.play;
            this.runButton.Location = new System.Drawing.Point(344, 5);
            this.runButton.Name = "runButton";
            this.runButton.Size = new System.Drawing.Size(35, 35);
            this.runButton.TabIndex = 3;
            this.runButton.UseVisualStyleBackColor = true;
            this.runButton.Click += new System.EventHandler(this.runButton_Click);
            // 
            // operationsStack
            // 
            this.operationsStack.Dock = System.Windows.Forms.DockStyle.Fill;
            this.operationsStack.FormattingEnabled = true;
            this.operationsStack.Location = new System.Drawing.Point(0, 0);
            this.operationsStack.Name = "operationsStack";
            this.operationsStack.Size = new System.Drawing.Size(302, 607);
            this.operationsStack.TabIndex = 0;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1362, 607);
            this.Controls.Add(this.splitContainer1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "Furniture";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private System.Windows.Forms.Button pauseButton;
        private System.Windows.Forms.Button nextStepButton;
        private System.Windows.Forms.Button runButton;
        private System.Windows.Forms.ListBox operationsStack;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ComboBox furDestHeightCombo;
        private System.Windows.Forms.ComboBox furDestWidthCombo;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.ComboBox furDestXCombo;
        private System.Windows.Forms.ComboBox furDestYCombo;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ComboBox furStartHeightCombo;
        private System.Windows.Forms.ComboBox furStartWidthCombo;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox furStartXCombo;
        private System.Windows.Forms.ComboBox furStartYCombo;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button resetButton;
        private System.Windows.Forms.Button createFurnitureButton;
        private System.Windows.Forms.CheckBox drawFurnituresDestCheckBox;
    }
}

