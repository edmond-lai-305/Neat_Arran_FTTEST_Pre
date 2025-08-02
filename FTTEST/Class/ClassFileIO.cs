using System;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.IO;


namespace PRETEST
{
    static class ClassFileIO
    {
        public static string sIniPath = Application.StartupPath + "\\PRETEST.ini";

        #region read & write ini file
        [DllImport("Kernel32.dll")]
        public static extern bool WritePrivateProfileString(string lpAppName,
            string lpKeyName, string lpString, string lpFileName);
        [DllImport("Kernel32.dll")]
        private static extern long GetPrivateProfileString(string section, string key,
             string def, StringBuilder retVal, int size, string filePath);
        public static bool WriteIniFile(string section, string keyName, string keyString, string fileName)
        {
            bool err = WritePrivateProfileString(section, keyName, keyString, fileName);
            return err;
        }
        public static string ReadIniFile(string section, string key, string filePath)
        {
            StringBuilder keyString = new StringBuilder(1024);
            string def = null;
            GetPrivateProfileString(section, key, def, keyString, 1024, filePath);
            return keyString.ToString().ToUpper().Trim();
        }
        #endregion

        //从配置文件加载的变量
        #region 配置文件定义的变量
        // [LocalSetting]
        public static string sMesEid = string.Empty;
        public static int iRunMode = new int();  //0 为离线模式，不上传过站信息。 1 上传过站信息
        public static string sTerminalId = string.Empty;
        public static string sFactory = string.Empty;

        // [ComPort]
        public static string sTvEnable = string.Empty;
        public static string sGdmEnable = string.Empty;
        public static string sGdm2Enable = string.Empty;
        public static string sVpgEnable = string.Empty;
        public static string sMhlEnable = string.Empty;
        public static int iTvPort = new int();
        public static int iGdmPort = new int();
        public static int iGdm2Port = new int();
        public static int iVpgPort = new int();
        public static int iMhlPort = new int();
        public static string sTvSettings = string.Empty;
        public static string sGdmSettings = string.Empty;
        public static string sGdm2Settings = string.Empty;
        public static string sVpgSettings = string.Empty;

        // [Spec]
        public static int iGdmMax = new int();
        public static int iGdmMin = new int();
        public static string sWifiMac = null;
        public static int iRssiLimit = new int();
        public static int iBatteryUp = new int();
        public static int iBatteryDown = new int();

        // [I2CCard]
        public static string sBaseAddr = string.Empty;

        // [TEST]
        public static string sMhl = string.Empty;
        public static string sBtFw = string.Empty;
        public static string sSsid = string.Empty;
        public static string sWifi = string.Empty;
        public static string sBt = string.Empty;
        public static string sHdcp = string.Empty;
        public static string sEthernet = string.Empty;
        public static string sFw = string.Empty;
        public static string sDotFw = string.Empty;
        public static string sSbSn = string.Empty;
        public static string sBsPath = string.Empty;
        #endregion
        static public bool LoadIniSettings()
        {
            bool err = true;
            string strBuf = string.Empty;
            string errTip = string.Empty;
            #region [LocalSettings]
            strBuf = ReadIniFile("LocalSettings", "MESEID", sIniPath);
            if (strBuf.Length != 0)
            {
                sMesEid = strBuf;
            }
            else
            {
                err = false;
                errTip += "必选项 [LocalSettings]节 MESID 键值未定义\n";
                //MessageBox.Show(null, "[LocalSettings]节 MESID 键值未定义\n\n" + "配置文件：" + iniPath, "配置文件错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            strBuf = ReadIniFile("LocalSettings", "RunMode", sIniPath);
            if (strBuf.Length != 0)
            {
                iRunMode = Convert.ToInt32(strBuf);
                if (iRunMode != 0 && iRunMode != 1)
                {
                    err = false;
                    errTip += "必选项 [LocalSettings]节 RunMode 键值设定错误\n";
                }
            }
            else
            {
                err = false;
                errTip += "必选项 [LocalSettings]节 RunMode 键值未定义\n";
                //MessageBox.Show(null, "[LocalSettings]节 RunMode 键值未定义\n\n" + "配置文件：" + iniPath, "配置文件错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            strBuf = ReadIniFile("LocalSettings", "TerminalID", sIniPath);
            if (strBuf.Length != 0)
            {
                sTerminalId = strBuf;
            }
            else
            {
                errTip += "可选项 [LocalSettings]节 TerminalID 键值未定义\n";
                //err = false;
                //MessageBox.Show(null, "[LocalSettings]节 TerminalID 键值未定义\n\n" + "配置文件：" + iniPath, "配置文件错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            strBuf = ReadIniFile("LocalSettings", "Factory", sIniPath);
            if (strBuf.Length != 0)
            {
                sFactory = strBuf.ToUpper().Replace(" ", "");
                if (sFactory != "R1" && sFactory != "R5")
                {
                    errTip += "必选项 [LocalSettings]节 Factory 键值设定错误\n";
                    err = false;
                }
            }
            else
            {
                errTip += "必选项 [LocalSettings]节 Factory 键值未定义\n";
                err = false;
                //MessageBox.Show(null, "[LocalSettings]节 Factory 键值未定义\n\n" + "配置文件：" + iniPath, "配置文件错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            #endregion
           #region [ComPort]
            strBuf = ReadIniFile("ComPort", "TVEnable", sIniPath);
            if (strBuf.Length != 0)
            {
                sTvEnable = strBuf.Trim().ToUpper();
            }
            else
            {
                errTip += "必选项 [ComPort]节 TVEnable 键值未定义\n";
                err = false;
                sTvEnable = "N";
                //MessageBox.Show(null, "[ComPort]节 TVPort 键值未定义\n\n" + "配置文件：" + iniPath, "配置文件错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            //strBuf = ReadIniFile("ComPort", "GDMEnable", sIniPath);
            //if (strBuf.Length != 0)
            //{
            //    sGdmEnable = strBuf.Trim().ToUpper();
            //}
            //else
            //{
            //    errTip += "必选项 [ComPort]节 GDMEnable 键值未定义\n";
            //    err = false;
            //    sGdmEnable = "N";
            //    //MessageBox.Show(null, "[ComPort]节 GDMEnable 键值未定义\n\n" + "配置文件：" + iniPath, "配置文件错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}

            //strBuf = ReadIniFile("ComPort", "GDM2Enable", sIniPath);
            //if (strBuf.Length != 0)
            //{
            //    sGdm2Enable = strBuf.Trim().ToUpper();
            //}
            //else
            //{
            //    errTip += "必选项 [ComPort]节 GDM2Enable 键值未定义\n";
            //    err = false;
            //    sGdm2Enable = "N";
            //    //MessageBox.Show(null, "[ComPort]节 GDM2Enable 键值未定义\n\n" + "配置文件：" + iniPath, "配置文件错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}

            strBuf = ReadIniFile("ComPort", "VPGEnable", sIniPath);
            if (strBuf.Length != 0)
            {
                sVpgEnable = strBuf.Trim().ToUpper();
            }
            else
            {
                errTip += "必选项 [ComPort]节 VPGEnable 键值未定义\n";
                err = false;
                sVpgEnable = "N";
                //MessageBox.Show(null, "[ComPort]节 VPGEnable 键值未定义\n\n" + "配置文件：" + iniPath, "配置文件错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            //strBuf = ReadIniFile("ComPort", "MHLEnable", sIniPath);
            //if (strBuf.Length != 0)
            //{
            //    sMhlEnable = strBuf.Trim().ToUpper();
            //}
            //else
            //{
            //    errTip += "必选项 [ComPort]节 MHLEnable 键值未定义\n";
            //    err = false;
            //    sMhlEnable = "N";
            //    //MessageBox.Show(null, "[ComPort]节 MHLEnable 键值未定义\n\n" + "配置文件：" + iniPath, "配置文件错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}

            strBuf = ReadIniFile("ComPort", "TVPort", sIniPath);
            if (strBuf.Length != 0)
            {
                iTvPort = Convert.ToInt32(strBuf);
                if (iTvPort < 1 || iTvPort > 20)
                {
                    errTip += "必选项 [ComPort]节 TVPort 键值设定错误\n";
                    err = false;
                }
            }
            else
            {
                errTip += "必选项 [ComPort]节 TVPort 键值未定义\n";
                err = false;
                iTvPort = 0;
                //MessageBox.Show(null, "[ComPort]节 TVPort 键值未定义\n\n" + "配置文件：" + iniPath, "配置文件错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            strBuf = ReadIniFile("ComPort", "TVSettings", sIniPath);
            if (strBuf.Length != 0)
            {
                sTvSettings = strBuf.ToUpper().Replace(" ","");
            }
            else
            {
                errTip += "必选项 [ComPort]节 TVSettings 键值未定义\n";
                err = false;
                //MessageBox.Show(null, "[ComPort]节 TVSettings 键值未定义\n\n" + "配置文件：" + iniPath, "配置文件错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            //strBuf = ReadIniFile("ComPort", "GDMPort", sIniPath);
            //if (strBuf.Length != 0)
            //{
            //    iGdmPort = Convert.ToInt32(strBuf);
            //    if (iGdmPort < 1 || iGdmPort > 20)
            //    {
            //        errTip += "必选项 [ComPort]节 GDMPort 键值设定错误\n";
            //        err = false;
            //    }
            //}
            //else
            //{
            //    errTip += "可选项 [ComPort]节 GDMPort 键值未定义\n";
            //    iGdmPort = 0;
            //    //err = false;
            //    //MessageBox.Show(null, "[ComPort]节 GDMPort 键值未定义\n\n" + "配置文件：" + iniPath, "配置文件错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}
            //strBuf = ReadIniFile("ComPort", "GDMSettings", sIniPath);
            //if (strBuf.Length != 0)
            //{
            //    sGdmSettings = strBuf.ToUpper().Replace(" ", "");
            //}
            //else
            //{
            //    errTip += "必选项 [ComPort]节 GDMSettings 键值未定义\n";
            //    err = false;
            //    //MessageBox.Show(null, "[ComPort]节 GDMSettings 键值未定义\n\n" + "配置文件：" + iniPath, "配置文件错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}
            //strBuf = ReadIniFile("ComPort", "GDM2Port", sIniPath);
            //if (strBuf.Length != 0)
            //{
            //    iGdm2Port = Convert.ToInt32(strBuf);
            //    if (iGdm2Port < 1 || iGdm2Port > 20)
            //    {
            //        errTip += "必选项 [ComPort]节 GDM2Port 键值设定错误\n";
            //        err = false;
            //    }
            //}
            //else
            //{
            //    errTip += "可选项 [ComPort]节 GDM2Port 键值未定义\n";
            //    iGdm2Port = 0;
            //    //err = false;
            //    //MessageBox.Show(null, "[ComPort]节 GDM2Port 键值未定义\n\n" + "配置文件：" + iniPath, "配置文件错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}
            //strBuf = ReadIniFile("ComPort", "GDM2Settings", sIniPath);
            //if (strBuf.Length != 0)
            //{
            //    sGdm2Settings = strBuf.ToUpper().Replace(" ", "");
            //}
            //else
            //{
            //    errTip += "必选项 [ComPort]节 GDMSettings 键值未定义\n";
            //    err = false;
            //    //MessageBox.Show(null, "[ComPort]节 GDM2Settings 键值未定义\n\n" + "配置文件：" + iniPath, "配置文件错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}
            strBuf = ReadIniFile("ComPort", "VPGPort", sIniPath);
            if (strBuf.Length != 0)
            {
                iVpgPort = Convert.ToInt32(strBuf);
                if (iVpgPort < 1 || iVpgPort > 20)
                {
                    errTip += "必选项 [ComPort]节 VPGPort 键值设定错误\n";
                    err = false;
                }
            }
            else
            {
                errTip += "可选项 [ComPort]节 VPGPort 键值未定义\n";
                iVpgPort = 0;
                //err = false;
                //MessageBox.Show(null, "[ComPort]节 VPGPort 键值未定义\n\n" + "配置文件：" + iniPath, "配置文件错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            strBuf = ReadIniFile("ComPort", "VPGSettings", sIniPath);
            if (strBuf.Length != 0)
            {
                sVpgSettings = strBuf.ToUpper().Replace(" ", "");
            }
            else
            {
                errTip += "可选项 [ComPort]节 VPGSettings 键值未定义\n";
                //err = false;
                //MessageBox.Show(null, "[ComPort]节 VPGSettings 键值未定义\n\n" + "配置文件：" + iniPath, "配置文件错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            //strBuf = ReadIniFile("ComPort", "MHLPort", sIniPath);
            //if (strBuf.Length != 0)
            //{
            //    iMhlPort = Convert.ToInt32(strBuf);
            //    if (iMhlPort < 1 || iMhlPort >20)
            //    {
            //        errTip += "可选项 [ComPort]节 MHLPort 键值设定错误\n";
            //        err = true;
            //    }
            //}
            //else
            //{
            //    errTip += "可选项 [ComPort]节 MHLPort 键值未定义\n";
            //    //err = false;
            //    //MessageBox.Show(null, "[ComPort]节 MHLPort 键值未定义\n\n" + "配置文件：" + iniPath, "配置文件错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}
            #endregion
            #region [Spec]
            strBuf = ReadIniFile("Spec", "GDMMax", sIniPath);
            if (strBuf.Length != 0)
            {
                iGdmMax = Convert.ToInt32(strBuf);
            }
            else
            {
                errTip += "可选项 [Spec]节 GDMMax 键值未定义\n";
                //err = false;
                //MessageBox.Show(null, "[Spec]节 GDMMax 键值未定义\n\n" + "配置文件：" + iniPath, "配置文件错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            strBuf = ReadIniFile("Spec", "GDMMin", sIniPath);
            if (strBuf.Length != 0)
            {
                iGdmMin = Convert.ToInt32(strBuf);
            }
            else
            {
                errTip += "可选项 [Spec]节 GDMMin 键值未定义\n";
                //err = false;
                //MessageBox.Show(null, "[Spec]节 GDMMin 键值未定义\n\n" + "配置文件：" + iniPath, "配置文件错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            strBuf = ReadIniFile("Spec", "WifiMac", sIniPath);
            if (strBuf.Length != 0)
            {
                sWifiMac = strBuf;
            }
            else
            {
                errTip += "可选项 [Spec]节 WifiMac 键值未定义\n";
                //err = false;
                //MessageBox.Show(null, "[Spec]节 WifiMac 键值未定义\n\n" + "配置文件：" + iniPath, "配置文件错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            strBuf = ReadIniFile("Spec", "RSSILimit", sIniPath);
            if (strBuf.Length != 0)
            {
                iRssiLimit = Convert.ToInt32(strBuf);
            }
            else
            {
                errTip += "可选项 [Spec]节 RSSILimit 键值未定义\n";
                //err = false;
                //MessageBox.Show(null, "[Spec]节 RSSILimit 键值未定义\n\n" + "配置文件：" + iniPath, "配置文件错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            #endregion
            #region [I2CCard]
            strBuf = ReadIniFile("I2CCard", "BaseAddr", sIniPath);
            if (strBuf.Length != 0)
            {
                sBaseAddr = strBuf.ToUpper().Replace(" ", "");
            }
            else
            {
                errTip += "可选项 [I2CCard]节 BaseAddr 键值未定义\n";
                //err = false;
                //MessageBox.Show(null, "[I2CCard]节 BaseAddr 键值未定义\n\n" + "配置文件：" + iniPath, "配置文件错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            #endregion
            #region [TEST]
            strBuf = ReadIniFile("TEST", "MHL", sIniPath);
            if (strBuf.Length != 0)
            {
                sMhl = strBuf.ToUpper().ToUpper().Replace(" ", "");
            }
            else
            {
                errTip += "必选项 [TEST]节 MHL 键值未定义\n";
                err = false;
                //MessageBox.Show(null, "[TEST]节 MHL 键值未定义\n\n" + "配置文件：" + iniPath, "配置文件错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            strBuf = ReadIniFile("TEST", "BTFW", sIniPath);
            if (strBuf.Length != 0)
            {
                sBtFw = strBuf.ToUpper().Replace(" ", "");
            }
            else
            {
                errTip += "必选项 [TEST]节 BTFW 键值未定义\n";
                err = false;
                //MessageBox.Show(null, "[TEST]节 BTFW 键值未定义\n\n" + "配置文件：" + iniPath, "配置文件错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            strBuf = ReadIniFile("TEST", "SSID", sIniPath);
            if (strBuf.Length != 0)
            {
                sSsid = strBuf.Trim();
            }
            else
            {
                errTip += "必选项 [TEST]节 SSID 键值未定义\n";
                err = false;
                //MessageBox.Show(null, "[TEST]节 SSID 键值未定义\n\n" + "配置文件：" + iniPath, "配置文件错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            strBuf = ReadIniFile("TEST", "Wifi", sIniPath);
            if (strBuf.Length != 0)
            {
                sWifi = strBuf.ToUpper().Replace(" ", "");
            }
            else
            {
                errTip += "必选项 [TEST]节 Wifi 键值未定义\n";
                err = false;
                //MessageBox.Show(null, "[TEST]节 Wifi 键值未定义\n\n" + "配置文件：" + iniPath, "配置文件错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            strBuf = ReadIniFile("TEST", "BT", sIniPath);
            if (strBuf.Length != 0)
            {
                sBt = strBuf.ToUpper().Replace(" ", "");
            }
            else
            {
                errTip += "必选项 [TEST]节 BT 键值未定义\n";
                err = false;
                //MessageBox.Show(null, "[TEST]节 BT 键值未定义\n\n" + "配置文件：" + iniPath, "配置文件错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            strBuf = ReadIniFile("TEST", "HDCP", sIniPath);
            if (strBuf.Length != 0)
            {
                sHdcp = strBuf.ToUpper().Replace(" ", "");
            }
            else
            {
                errTip += "必选项 [TEST]节 HDCP 键值未定义\n";
                err = false;
                //MessageBox.Show(null, "[TEST]节 HDCP 键值未定义\n\n" + "配置文件：" + iniPath, "配置文件错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            strBuf = ReadIniFile("TEST", "Ethernet", sIniPath);
            if (strBuf.Length != 0)
            {
                sEthernet = strBuf.ToUpper().Replace(" ", "");
            }
            else
            {
                errTip += "必选项 [TEST]节 Ethernet 键值未定义\n";
                err = false;
                //MessageBox.Show(null, "[TEST]节 Ethernet 键值未定义\n\n" + "配置文件：" + iniPath, "配置文件错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            strBuf = ReadIniFile("TEST", "FW", sIniPath);
            if (strBuf.Length != 0)
            {
                sFw = strBuf.ToUpper().Replace(" ", "");
            }
            else
            {
                errTip += "必选项 [TEST]节 FW 键值未定义\n";
                err = false;
                //MessageBox.Show(null, "[TEST]节 FW 键值未定义\n\n" + "配置文件：" + iniPath, "配置文件错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            strBuf = ReadIniFile("TEST", "DotFW", sIniPath);
            if (strBuf.Length != 0)
            {
                sDotFw = strBuf.ToUpper().Replace(" ", "");
            }
            else
            {
                errTip += "必选项 [TEST]节 DotFW 键值未定义\n";
                err = false;
                //MessageBox.Show(null, "[TEST]节 DotFW 键值未定义\n\n" + "配置文件：" + iniPath, "配置文件错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            strBuf = ReadIniFile("TEST", "SBSN", sIniPath);
            if (strBuf.Length != 0)
            {
                sSbSn = strBuf.ToUpper().Replace(" ", "");
            }
            else
            {
                errTip += "必选项 [TEST]节 SBSN 键值未定义\n";
                err = false;
                //MessageBox.Show(null, "[TEST]节 SBSN 键值未定义\n\n" + "配置文件：" + iniPath, "配置文件错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            strBuf = ReadIniFile("TEST", "BsPath", sIniPath);
            if (strBuf.Length != 0)
            {
                sBsPath = strBuf.ToUpper().Trim();
            }
            else
            {
                errTip += "必选项 [TEST]节 BsPath 键值未定义\n";
                err = false;
                //MessageBox.Show(null, "[TEST]节 BsPath 键值未定义\n\n" + "配置文件：" + iniPath, "配置文件错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            #endregion
            #region 如果配置文件有遗漏设置，报错提示
            if (errTip.Length != 0)
            {
                if (!err)
                {
                    strBuf = "配置文件: 必选项未设置，无法继续";
                }
                else
                {
                    strBuf = "配置文件: 请检查配置文件是否有误";
                }
                MessageBox.Show(null, errTip + "\n配置文件：" + sIniPath, strBuf, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            #endregion
            return err;
        }

        public static void WriteLog(string fileName, string text)
        {
            StreamWriter sWrite = new StreamWriter(Application.StartupPath + "\\" + fileName, true);
            sWrite.WriteLine(text);
            sWrite.Close();
        }
    }
}
