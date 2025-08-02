namespace PRETEST
{
    partial class FormSetting
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
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.comboBoxFactory = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.comboProgram = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textMESEID = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.comboI2C = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.checkBoxJES = new System.Windows.Forms.CheckBox();
            this.checkBoxMI = new System.Windows.Forms.CheckBox();
            this.textBoxVPG = new System.Windows.Forms.TextBox();
            this.comboBoxVPG = new System.Windows.Forms.ComboBox();
            this.checkBoxVPG = new System.Windows.Forms.CheckBox();
            this.textBoxIO = new System.Windows.Forms.TextBox();
            this.comboBoxIO = new System.Windows.Forms.ComboBox();
            this.checkBoxIO = new System.Windows.Forms.CheckBox();
            this.textBoxSick = new System.Windows.Forms.TextBox();
            this.comboBoxSick = new System.Windows.Forms.ComboBox();
            this.checkBoxSick = new System.Windows.Forms.CheckBox();
            this.textBoxTv = new System.Windows.Forms.TextBox();
            this.comboBoxTv = new System.Windows.Forms.ComboBox();
            this.checkBoxTv = new System.Windows.Forms.CheckBox();
            this.button_Exit = new System.Windows.Forms.Button();
            this.button_save = new System.Windows.Forms.Button();
            this.button_refresh = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // treeView1
            // 
            this.treeView1.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.treeView1.Location = new System.Drawing.Point(0, -1);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(537, 70);
            this.treeView1.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.comboBoxFactory);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.comboProgram);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.textMESEID);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.comboI2C);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox1.Location = new System.Drawing.Point(3, 75);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(248, 179);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Function";
            this.groupBox1.Enter += new System.EventHandler(this.groupBox1_Enter);
            // 
            // comboBoxFactory
            // 
            this.comboBoxFactory.FormattingEnabled = true;
            this.comboBoxFactory.Items.AddRange(new object[] {
            "R1",
            "R5"});
            this.comboBoxFactory.Location = new System.Drawing.Point(118, 20);
            this.comboBoxFactory.Name = "comboBoxFactory";
            this.comboBoxFactory.Size = new System.Drawing.Size(111, 22);
            this.comboBoxFactory.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.Location = new System.Drawing.Point(8, 26);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(61, 12);
            this.label4.TabIndex = 6;
            this.label4.Text = "Factory:";
            // 
            // comboProgram
            // 
            this.comboProgram.FormattingEnabled = true;
            this.comboProgram.Items.AddRange(new object[] {
            "GPTS",
            "ULPK",
            "Reset"});
            this.comboProgram.Location = new System.Drawing.Point(119, 129);
            this.comboProgram.Name = "comboProgram";
            this.comboProgram.Size = new System.Drawing.Size(111, 22);
            this.comboProgram.TabIndex = 5;
            this.comboProgram.SelectedIndexChanged += new System.EventHandler(this.comboProgram_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(10, 134);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(89, 12);
            this.label3.TabIndex = 4;
            this.label3.Text = "Auto_Program";
            this.label3.Click += new System.EventHandler(this.label3_Click);
            // 
            // textMESEID
            // 
            this.textMESEID.Location = new System.Drawing.Point(119, 92);
            this.textMESEID.Name = "textMESEID";
            this.textMESEID.Size = new System.Drawing.Size(110, 23);
            this.textMESEID.TabIndex = 3;
            this.textMESEID.TextChanged += new System.EventHandler(this.textMESEID_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(10, 97);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(54, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "MESEID:";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // comboI2C
            // 
            this.comboI2C.FormattingEnabled = true;
            this.comboI2C.Items.AddRange(new object[] {
            "&h340",
            "&h348",
            "&h350",
            "&h358"});
            this.comboI2C.Location = new System.Drawing.Point(119, 55);
            this.comboI2C.Name = "comboI2C";
            this.comboI2C.Size = new System.Drawing.Size(111, 22);
            this.comboI2C.TabIndex = 1;
            this.comboI2C.SelectedIndexChanged += new System.EventHandler(this.comboI2C_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(9, 61);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(103, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "I2C_Card_Addr.";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.checkBoxJES);
            this.groupBox2.Controls.Add(this.checkBoxMI);
            this.groupBox2.Controls.Add(this.textBoxVPG);
            this.groupBox2.Controls.Add(this.comboBoxVPG);
            this.groupBox2.Controls.Add(this.checkBoxVPG);
            this.groupBox2.Controls.Add(this.textBoxIO);
            this.groupBox2.Controls.Add(this.comboBoxIO);
            this.groupBox2.Controls.Add(this.checkBoxIO);
            this.groupBox2.Controls.Add(this.textBoxSick);
            this.groupBox2.Controls.Add(this.comboBoxSick);
            this.groupBox2.Controls.Add(this.checkBoxSick);
            this.groupBox2.Controls.Add(this.textBoxTv);
            this.groupBox2.Controls.Add(this.comboBoxTv);
            this.groupBox2.Controls.Add(this.checkBoxTv);
            this.groupBox2.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox2.Location = new System.Drawing.Point(257, 75);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(277, 179);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Com_Port";
            this.groupBox2.Enter += new System.EventHandler(this.groupBox2_Enter);
            // 
            // checkBoxJES
            // 
            this.checkBoxJES.AutoSize = true;
            this.checkBoxJES.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.checkBoxJES.Location = new System.Drawing.Point(6, 135);
            this.checkBoxJES.Name = "checkBoxJES";
            this.checkBoxJES.Size = new System.Drawing.Size(128, 16);
            this.checkBoxJES.TabIndex = 16;
            this.checkBoxJES.Text = "基恩士自动枪打勾";
            this.checkBoxJES.UseVisualStyleBackColor = true;
            // 
            // checkBoxMI
            // 
            this.checkBoxMI.AutoSize = true;
            this.checkBoxMI.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.checkBoxMI.Location = new System.Drawing.Point(6, 156);
            this.checkBoxMI.Name = "checkBoxMI";
            this.checkBoxMI.Size = new System.Drawing.Size(205, 16);
            this.checkBoxMI.TabIndex = 15;
            this.checkBoxMI.Text = "ON_COM: 自动侦测小米USB测试";
            this.checkBoxMI.UseVisualStyleBackColor = true;
            // 
            // textBoxVPG
            // 
            this.textBoxVPG.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.textBoxVPG.Location = new System.Drawing.Point(135, 108);
            this.textBoxVPG.Name = "textBoxVPG";
            this.textBoxVPG.Size = new System.Drawing.Size(136, 23);
            this.textBoxVPG.TabIndex = 13;
            // 
            // comboBoxVPG
            // 
            this.comboBoxVPG.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.comboBoxVPG.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxVPG.FormattingEnabled = true;
            this.comboBoxVPG.Items.AddRange(new object[] {
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
            "11",
            "12",
            "13",
            "14",
            "15",
            "16",
            "17",
            "18",
            "19",
            "20"});
            this.comboBoxVPG.Location = new System.Drawing.Point(79, 109);
            this.comboBoxVPG.Name = "comboBoxVPG";
            this.comboBoxVPG.Size = new System.Drawing.Size(50, 22);
            this.comboBoxVPG.TabIndex = 12;
            // 
            // checkBoxVPG
            // 
            this.checkBoxVPG.AutoSize = true;
            this.checkBoxVPG.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.checkBoxVPG.Location = new System.Drawing.Point(6, 113);
            this.checkBoxVPG.Name = "checkBoxVPG";
            this.checkBoxVPG.Size = new System.Drawing.Size(45, 16);
            this.checkBoxVPG.TabIndex = 14;
            this.checkBoxVPG.Text = "MHL";
            this.checkBoxVPG.UseVisualStyleBackColor = true;
            // 
            // textBoxIO
            // 
            this.textBoxIO.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.textBoxIO.Location = new System.Drawing.Point(135, 79);
            this.textBoxIO.Name = "textBoxIO";
            this.textBoxIO.Size = new System.Drawing.Size(136, 23);
            this.textBoxIO.TabIndex = 10;
            // 
            // comboBoxIO
            // 
            this.comboBoxIO.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.comboBoxIO.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxIO.FormattingEnabled = true;
            this.comboBoxIO.Items.AddRange(new object[] {
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
            "11",
            "12",
            "13",
            "14",
            "15",
            "16",
            "17",
            "18",
            "19",
            "20"});
            this.comboBoxIO.Location = new System.Drawing.Point(79, 80);
            this.comboBoxIO.Name = "comboBoxIO";
            this.comboBoxIO.Size = new System.Drawing.Size(50, 22);
            this.comboBoxIO.TabIndex = 9;
            // 
            // checkBoxIO
            // 
            this.checkBoxIO.AutoSize = true;
            this.checkBoxIO.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.checkBoxIO.Location = new System.Drawing.Point(6, 84);
            this.checkBoxIO.Name = "checkBoxIO";
            this.checkBoxIO.Size = new System.Drawing.Size(73, 16);
            this.checkBoxIO.TabIndex = 11;
            this.checkBoxIO.Text = "IO_Card";
            this.checkBoxIO.UseVisualStyleBackColor = true;
            // 
            // textBoxSick
            // 
            this.textBoxSick.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.textBoxSick.Location = new System.Drawing.Point(135, 50);
            this.textBoxSick.Name = "textBoxSick";
            this.textBoxSick.Size = new System.Drawing.Size(136, 23);
            this.textBoxSick.TabIndex = 7;
            // 
            // comboBoxSick
            // 
            this.comboBoxSick.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.comboBoxSick.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxSick.FormattingEnabled = true;
            this.comboBoxSick.Items.AddRange(new object[] {
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
            "11",
            "12",
            "13",
            "14",
            "15",
            "16",
            "17",
            "18",
            "19",
            "20"});
            this.comboBoxSick.Location = new System.Drawing.Point(79, 49);
            this.comboBoxSick.Name = "comboBoxSick";
            this.comboBoxSick.Size = new System.Drawing.Size(50, 22);
            this.comboBoxSick.TabIndex = 6;
            // 
            // checkBoxSick
            // 
            this.checkBoxSick.AutoSize = true;
            this.checkBoxSick.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.checkBoxSick.Location = new System.Drawing.Point(6, 55);
            this.checkBoxSick.Name = "checkBoxSick";
            this.checkBoxSick.Size = new System.Drawing.Size(52, 16);
            this.checkBoxSick.TabIndex = 8;
            this.checkBoxSick.Text = "Sick";
            this.checkBoxSick.UseVisualStyleBackColor = true;
            // 
            // textBoxTv
            // 
            this.textBoxTv.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.textBoxTv.Location = new System.Drawing.Point(135, 21);
            this.textBoxTv.Name = "textBoxTv";
            this.textBoxTv.Size = new System.Drawing.Size(136, 23);
            this.textBoxTv.TabIndex = 4;
            // 
            // comboBoxTv
            // 
            this.comboBoxTv.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.comboBoxTv.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxTv.FormattingEnabled = true;
            this.comboBoxTv.Items.AddRange(new object[] {
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
            "11",
            "12",
            "13",
            "14",
            "15",
            "16",
            "17",
            "18",
            "19",
            "20",
            "21",
            "22",
            "23",
            "24",
            "25",
            "26",
            "27",
            "28",
            "29",
            "30"});
            this.comboBoxTv.Location = new System.Drawing.Point(79, 21);
            this.comboBoxTv.Name = "comboBoxTv";
            this.comboBoxTv.Size = new System.Drawing.Size(50, 22);
            this.comboBoxTv.TabIndex = 3;
            this.comboBoxTv.SelectedIndexChanged += new System.EventHandler(this.comboBoxTv_SelectedIndexChanged);
            // 
            // checkBoxTv
            // 
            this.checkBoxTv.AutoSize = true;
            this.checkBoxTv.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.checkBoxTv.Location = new System.Drawing.Point(6, 26);
            this.checkBoxTv.Name = "checkBoxTv";
            this.checkBoxTv.Size = new System.Drawing.Size(38, 16);
            this.checkBoxTv.TabIndex = 5;
            this.checkBoxTv.Text = "TV";
            this.checkBoxTv.UseVisualStyleBackColor = true;
            this.checkBoxTv.CheckedChanged += new System.EventHandler(this.checkBoxTv_CheckedChanged);
            // 
            // button_Exit
            // 
            this.button_Exit.Image = global::PRETEST.Properties.Resources._3;
            this.button_Exit.Location = new System.Drawing.Point(153, 0);
            this.button_Exit.Name = "button_Exit";
            this.button_Exit.Size = new System.Drawing.Size(79, 69);
            this.button_Exit.TabIndex = 4;
            this.button_Exit.UseVisualStyleBackColor = true;
            this.button_Exit.Click += new System.EventHandler(this.button_Exit_Click);
            // 
            // button_save
            // 
            this.button_save.Image = global::PRETEST.Properties.Resources._2;
            this.button_save.Location = new System.Drawing.Point(77, 0);
            this.button_save.Name = "button_save";
            this.button_save.Size = new System.Drawing.Size(77, 69);
            this.button_save.TabIndex = 3;
            this.button_save.UseVisualStyleBackColor = true;
            this.button_save.Click += new System.EventHandler(this.button_save_Click);
            // 
            // button_refresh
            // 
            this.button_refresh.Image = global::PRETEST.Properties.Resources._1;
            this.button_refresh.Location = new System.Drawing.Point(0, 0);
            this.button_refresh.Name = "button_refresh";
            this.button_refresh.Size = new System.Drawing.Size(79, 69);
            this.button_refresh.TabIndex = 2;
            this.button_refresh.UseVisualStyleBackColor = true;
            this.button_refresh.Click += new System.EventHandler(this.button_refresh_Click);
            // 
            // FormSetting
            // 
            this.ClientSize = new System.Drawing.Size(539, 259);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.button_Exit);
            this.Controls.Add(this.button_save);
            this.Controls.Add(this.button_refresh);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.treeView1);
            this.Name = "FormSetting";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "设置";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormSetting_FormClosed);
            this.Load += new System.EventHandler(this.FormSetting_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        //private System.Windows.Forms.ImageList imageList1;
        //private System.Windows.Forms.HScrollBar hScrollBar1;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button button_refresh;
        private System.Windows.Forms.Button button_save;
        private System.Windows.Forms.Button button_Exit;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ComboBox comboI2C;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboProgram;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textMESEID;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboBoxFactory;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxVPG;
        private System.Windows.Forms.ComboBox comboBoxVPG;
        private System.Windows.Forms.CheckBox checkBoxVPG;
        private System.Windows.Forms.TextBox textBoxIO;
        private System.Windows.Forms.ComboBox comboBoxIO;
        private System.Windows.Forms.CheckBox checkBoxIO;
        private System.Windows.Forms.TextBox textBoxSick;
        private System.Windows.Forms.ComboBox comboBoxSick;
        private System.Windows.Forms.CheckBox checkBoxSick;
        private System.Windows.Forms.TextBox textBoxTv;
        private System.Windows.Forms.ComboBox comboBoxTv;
        private System.Windows.Forms.CheckBox checkBoxTv;
        private System.Windows.Forms.CheckBox checkBoxMI;
        private System.Windows.Forms.CheckBox checkBoxJES;
    }
}
