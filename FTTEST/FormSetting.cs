//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;

namespace PRETEST
{
    public partial class FormSetting : Form
    {
        public static string sIniPath = Application.StartupPath + "\\PRETEST.ini";
        private bool rewrite = new bool();
        public FormSetting()
        {
            InitializeComponent();
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void comboI2C_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textMESEID_TextChanged(object sender, EventArgs e)
        {

        }

        private void comboProgram_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void button_Exit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button_save_Click(object sender, EventArgs e)
        {
            //if (SpecCheck() == false) return;
            if (UpdateIniFile()==true)
            {
                rewrite = true;
                MessageBox.Show(null, "设置已保存，需要重新打开程序。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Application.Exit();
            }
            else
            {
                MessageBox.Show(null, "设置无法保存！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void button_refresh_Click(object sender, EventArgs e)
        {

        }
        bool SpecCheck()
        {
            int iErr = new int();
            bool bErr = new bool();
            string msg = string.Empty;
            string strBuf = string.Empty;
            bool[] port = new bool[20];

            strBuf = ClassFileIO.ReadIniFile ("LocalSettings", "MESEID", sIniPath);
            textMESEID.Text = strBuf;
            if (textMESEID.Text.ToUpper().Length != 8)
            {
                iErr++;
                msg += iErr + "：无效的 MESEID\n";
            }
            strBuf = "";
            strBuf = ClassFileIO.ReadIniFile("LocalSettings", "Factory", sIniPath);
            comboBoxFactory.Text = strBuf;
            if (comboBoxFactory.Text == "")
            {
                iErr++;
                msg += iErr + "：无效的 Factory\n";
            }
            strBuf = "";
            strBuf = ClassFileIO.ReadIniFile("I2CCard", "BaseAddr", sIniPath);
            comboI2C.Text = strBuf;
            if (comboI2C.Text == "")
            {
                iErr++;
                msg += iErr + "：无效的 I2CCard BaseAddr\n";
            }
            strBuf = "";
            //TV Port info
            strBuf = ClassFileIO.ReadIniFile("ComPort", "TVEnable", sIniPath);
            if  (strBuf.Trim().ToUpper()=="Y")
            {
                checkBoxTv.Checked = true;
            }
            else
            {
                checkBoxTv.Checked = false;
            }
            strBuf = "";

            strBuf = ClassFileIO.ReadIniFile("ComPort", "TVPort", sIniPath);
            if (strBuf!="")
            {
                if (Convert.ToInt16(strBuf)>0 || Convert.ToInt16(strBuf) < 30)
                {
                    comboBoxTv.Text = strBuf;
                }
                else
                {
                    iErr++;
                    msg += iErr + "： 无效的TV端口\n";
                }
            }
            strBuf = "";

            strBuf = ClassFileIO.ReadIniFile("ComPort", "TVSettings", sIniPath);
            if (strBuf != "")
            {
                textBoxTv.Text = strBuf;
            }
            //Sick info
            strBuf = ClassFileIO.ReadIniFile("ComPort", "SICKEnable", sIniPath);
            if (strBuf.Trim().ToUpper() == "Y")
            {
                checkBoxSick.Checked = true;
            }
            else
            {
                checkBoxSick.Checked = false;
            }

            //JES Sick info
            strBuf = ClassFileIO.ReadIniFile("ComPort", "JESEnable", sIniPath);
            if (strBuf.Trim().ToUpper() == "Y")
            {
                checkBoxJES.Checked = true;
            }
            else
            {
                checkBoxJES.Checked = false;
            }

            strBuf = "";

            strBuf = ClassFileIO.ReadIniFile("ComPort", "SICKPort", sIniPath);
            if (strBuf != "")
            {
                if (Convert.ToInt16(strBuf) > 0 || Convert.ToInt16(strBuf) < 30)
                {
                    comboBoxSick.Text = strBuf;
                }
                else
                {
                    iErr++;
                    msg += iErr + "： 无效的SICK枪端口\n";
                }
            }
            strBuf = "";

            strBuf = ClassFileIO.ReadIniFile("ComPort", "SICKSettings", sIniPath);
            if (strBuf != "")
            {
                textBoxSick.Text = strBuf;
            }
            //IO CARD info
            strBuf = ClassFileIO.ReadIniFile("ComPort", "IOCardEnable", sIniPath);
            if (strBuf.Trim().ToUpper() == "Y")
            {
                checkBoxIO.Checked = true;
            }
            else
            {
                checkBoxIO.Checked = false;
            }
            strBuf = "";

            strBuf = ClassFileIO.ReadIniFile("ComPort", "IOCardPort", sIniPath);
            if (strBuf != "")
            {
                if (Convert.ToInt16(strBuf) > 0 || Convert.ToInt16(strBuf) < 30)
                {
                    comboBoxIO.Text = strBuf;
                }
                else
                {
                    iErr++;
                    msg += iErr + "： 无效的TV端口\n";
                }
            }
            strBuf = "";

            strBuf = ClassFileIO.ReadIniFile("ComPort", "IOCardSettings", sIniPath);
            if (strBuf != "")
            {
                textBoxIO.Text = strBuf;
            }
            //MHL info
            strBuf = ClassFileIO.ReadIniFile("ComPort", "MHLEnable", sIniPath);
            if (strBuf.Trim().ToUpper() == "Y")
            {
                checkBoxVPG.Checked = true;
            }
            else
            {
                checkBoxVPG.Checked = false;
            }
            strBuf = "";

            strBuf = ClassFileIO.ReadIniFile("ComPort", "MHLPort", sIniPath);
            if (strBuf != "")
            {
                if (Convert.ToInt16(strBuf) > 0 || Convert.ToInt16(strBuf) < 30)
                {
                    comboBoxVPG.Text = strBuf;
                }
                else
                {
                    iErr++;
                    msg += iErr + "： 无效的TV端口\n";
                }
            }
            strBuf = "";

            strBuf = ClassFileIO.ReadIniFile("ComPort", "MHLSettings", sIniPath);
            if (strBuf != "")
            {
                textBoxVPG.Text = strBuf;
            }

            if (iErr != 0)
            {
                bErr = false;
                MessageBox.Show(null, msg, "提示");
            }
            else
            {
                bErr = true;
            }
            return bErr;
        }


        bool  UpdateIniFile()
        {
            //ClassFileIO.WriteIniFile("LocalSettings", "MESEID", (textMESEID.Text.ToUpper().Length != 8) ? "1ABUTY11" : textMESEID.Text.ToUpper(), ClassFileIO.sIniPath);
            //ClassFileIO.WriteIniFile("I2CCard", "BaseAddr", comboI2C.SelectedItem.ToString(), ClassFileIO.sIniPath);
            //ClassFileIO.WriteIniFile("LocalSettings", "Factory", comboBoxFactory.SelectedItem.ToString(), ClassFileIO.sIniPath);

            //ClassFileIO.WriteIniFile("ComPort", "TVPort", (comboBoxTv.SelectedIndex + 1).ToString(), ClassFileIO.sIniPath);
            //ClassFileIO.WriteIniFile("ComPort", "TVSettings", textBoxTv.Text.Trim().ToUpper() ?? "115200,N,8,1", ClassFileIO.sIniPath);

            //ClassFileIO.WriteIniFile("ComPort", "SICKPort", (comboBoxSick.SelectedIndex + 1).ToString(), ClassFileIO.sIniPath);
            //ClassFileIO.WriteIniFile("ComPort", "SICKSettings", textBoxSick.Text.Trim().ToUpper() ?? "9600,N,8,1", ClassFileIO.sIniPath);

            //ClassFileIO.WriteIniFile("ComPort", "IOCardPort", (comboBoxIO.SelectedIndex + 1).ToString(), ClassFileIO.sIniPath);
            //ClassFileIO.WriteIniFile("ComPort", "IOCardSettings", textBoxIO.Text.Trim().ToUpper() ?? "9600,N,8,1", ClassFileIO.sIniPath);

            //ClassFileIO.WriteIniFile("ComPort", "VPGPort", (comboBoxVPG.SelectedIndex + 1).ToString(), ClassFileIO.sIniPath);
            //ClassFileIO.WriteIniFile("ComPort", "VPGSettings", textBoxVPG.Text.Trim().ToUpper() ?? "9600,N,8,1", ClassFileIO.sIniPath);

            //ClassFileIO.WriteIniFile("ComPort", "MHLVPort", (comboBoxMhl.SelectedIndex + 1).ToString(), ClassFileIO.sIniPath);

            ClassFileIO.WriteIniFile("LocalSettings", "MESEID", textMESEID.Text , ClassFileIO.sIniPath);
            ClassFileIO.WriteIniFile("LocalSettings", "Factory", comboBoxFactory.Text, ClassFileIO.sIniPath);
            ClassFileIO.WriteIniFile("I2CCard", "BaseAddr", comboI2C.Text, ClassFileIO.sIniPath);

            if (checkBoxTv.Checked)
            {
                ClassFileIO.WriteIniFile("ComPort", "TVEnable", "Y", ClassFileIO.sIniPath);
                ClassFileIO.WriteIniFile("ComPort", "TVPort", comboBoxTv.Text , ClassFileIO.sIniPath);
                ClassFileIO.WriteIniFile("ComPort", "TVSettings", textBoxTv.Text, ClassFileIO.sIniPath);
            }
            else
            {
                ClassFileIO.WriteIniFile("ComPort", "TVEnable", "N", ClassFileIO.sIniPath);
            }
            if (checkBoxIO.Checked)
            {
                ClassFileIO.WriteIniFile("ComPort", "IOCardEnable", "Y", ClassFileIO.sIniPath);
                ClassFileIO.WriteIniFile("ComPort", "IOCardPort", comboBoxIO.Text, ClassFileIO.sIniPath);
                ClassFileIO.WriteIniFile("ComPort", "IOCardSettings", textBoxIO.Text, ClassFileIO.sIniPath);
            }
            else
            {
                ClassFileIO.WriteIniFile("ComPort", "IOCardEnable", "N", ClassFileIO.sIniPath);
            }
            if (checkBoxSick.Checked)
            {
                ClassFileIO.WriteIniFile("ComPort", "SICKEnable", "Y", ClassFileIO.sIniPath);
                ClassFileIO.WriteIniFile("ComPort", "SICKPort", comboBoxSick.Text, ClassFileIO.sIniPath);
                ClassFileIO.WriteIniFile("ComPort", "SICKSettings", textBoxSick.Text, ClassFileIO.sIniPath);
            }
            else
            {
                ClassFileIO.WriteIniFile("ComPort", "SICKEnable", "N", ClassFileIO.sIniPath);
            }

            if (checkBoxJES.Checked)
            {
                ClassFileIO.WriteIniFile("ComPort", "JESEnable", "Y", ClassFileIO.sIniPath);
            }
            else
            {
                ClassFileIO.WriteIniFile("ComPort", "JESEnable", "N", ClassFileIO.sIniPath);
            }

            if (checkBoxVPG.Checked)
            {
                ClassFileIO.WriteIniFile("ComPort", "MHLEnable", "Y", ClassFileIO.sIniPath);
                ClassFileIO.WriteIniFile("ComPort", "MHLPort", comboBoxVPG.Text , ClassFileIO.sIniPath);
                ClassFileIO.WriteIniFile("ComPort", "MHLSettings", textBoxVPG.Text, ClassFileIO.sIniPath);
            }
            else
            {
                ClassFileIO.WriteIniFile("ComPort", "MHLEnable", "N", ClassFileIO.sIniPath);
            }
            //if (MhlPort)
            //{
            //    ClassFileIO.WriteIniFile("ComPort", "MHLEnable", "Y", ClassFileIO.sIniPath);
            //}
            //else
            //{
            //    ClassFileIO.WriteIniFile("ComPort", "MHLEnable", "N", ClassFileIO.sIniPath);
            //}
            return true;
        }

        private void FormSetting_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (rewrite)
            {
                MessageBox.Show(null, "设置已保存，需要重新打开程序。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Application.Exit();
            }
        }

        private void checkBoxTv_CheckedChanged(object sender, EventArgs e)
        {
            
            checkBoxTv.Checked  = this.checkBoxTv.Checked;
        }

        private void FormSetting_Load(object sender, EventArgs e)
        {
            SpecCheck();
        }
        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void comboBoxTv_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
