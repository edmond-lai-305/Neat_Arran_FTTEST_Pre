using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.IO;

namespace PRETEST.AppConfig
{
    public class GlobalConfig
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
            return keyString.ToString().Trim();
        }
        #endregion

        //从配置文件加载的变量
        #region 配置文件定义的变量
        // [LocalSetting]
        public static string sMesEid = string.Empty;
        public static string sLogServerIP = string.Empty;
        public static int iRunMode = new int();  //0 为离线模式，不上传过站信息。 1 上传过站信息
        public static string sTerminalId = string.Empty;
        public static string sFactory = string.Empty;
        public static bool bNew_Mes = false;
        public static string sModel = string.Empty;
        public static string sServerLogPath = string.Empty;
        public static string sLocalLogPath = string.Empty;

        // [ComPort]
        public static string sTvEnable = string.Empty;
        public static string sGdmEnable = string.Empty;
        public static string sIOCardEnable = string.Empty;
        public static string sSickEnable = string.Empty;
        public static string sJESEnable = string.Empty;
        public static string sMhlEnable = string.Empty;
        public static string sDoorEnable = string.Empty;
        public static int iTvPort = new int();
        public static int iGdmPort = new int();
        public static int iIOCardPort = new int();
        public static int iSickPort = new int();
        public static int iMhlPort = new int();
        public static int iDoorPort = new int();
        public static string sTvSettings = string.Empty;
        public static string sGdmSettings = string.Empty;
        public static string sIOCardSettings = string.Empty;
        public static string sSickSettings = string.Empty;

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
        public static string sPwd = string.Empty;
        public static string sUdpServer = string.Empty;
        public static string sDataServer = string.Empty;
        public static string sBtMac = string.Empty;
        public static string sHdcp = string.Empty;
        public static string sEthernet = string.Empty;
        public static string sFw = string.Empty;
        public static string sFw2 = string.Empty;
        public static string sDotFw = string.Empty;
        public static string sSbSn = string.Empty;
        public static string sBsPath = string.Empty;
        //public static string smicPath = string.Empty;
        //public static string sbuildPath = string.Empty;
        public static string sCmd1Path = string.Empty;
        public static string sCmd1PathFolder = string.Empty;
        public static string sCmd2Path = string.Empty;
        public static string sCmd3Path = string.Empty;
        public static string sCmd4Path = string.Empty;
        public static string sCmd5Path = string.Empty;
        public static string sCmd2PathFolder = string.Empty;
        public static string sAccessDbPath = string.Empty;
        public static string sCheckBut = string.Empty;  //新增
        public static int iBtRssiOffset = new int();
        public static string sBiWifiSsid = string.Empty;    //Burning WiFi SSID
        public static string sBiWifiPwd = string.Empty;     //Burning WiFi Password
        public static string sBiWifiGateway = string.Empty; //Burning WiFi Gateway
        #endregion
        /// <summary>
        /// 加载配置文件
        /// </summary>
        /// <returns></returns>
        static public bool LoadIniSettings()
        {
            int unCounter = new int();
            int unCounter2 = new int();
            bool err = true;
            string strBuf = string.Empty;
            string errTip = string.Empty;
            bNew_Mes = false;

            #region [LocalSettings]
            strBuf = ReadIniFile("LocalSettings", "MESEID", sIniPath);
            if (strBuf.Length != 0)
            {
                sMesEid = strBuf;
            }
            else
            {
                err = false;
                errTip += "●必选项 [LocalSettings]节 MESID 键值未定义\n";
            }
            strBuf = ReadIniFile("LocalSettings", "LogServerIP", sIniPath);
            if (strBuf.Length != 0)
            {
                sLogServerIP = strBuf;
            }
            else
            {
                err = false;
                errTip += "●必选项 [LocalSettings]节 MESID 键值未定义\n";
            }
            strBuf = ReadIniFile("LocalSettings", "NEW_MES", sIniPath);
            if (strBuf.ToString().ToUpper()=="Y")
            {
                bNew_Mes =true;
            }
            else
            {
                bNew_Mes = false;
            }
            strBuf = ReadIniFile("LocalSettings", "RunMode", sIniPath);
            if (strBuf.Length != 0)
            {
                iRunMode = Convert.ToInt32(strBuf);
                if (iRunMode != 0 && iRunMode != 1)
                {
                    err = false;
                    errTip += "●必选项 [LocalSettings]节 RunMode 键值设定错误\n";
                }
            }
            else
            {
                err = false;
                errTip += "●必选项 [LocalSettings]节 RunMode 键值未定义\n";
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
            }
            strBuf = ReadIniFile("LocalSettings", "Factory", sIniPath);
            if (strBuf.Length != 0)
            {
                sFactory = strBuf.ToUpper().Replace(" ", "");
                if (sFactory != "R1" && sFactory != "R5")
                {
                    errTip += "●必选项 [LocalSettings]节 Factory 键值设定错误\n";
                    err = false;
                }
            }
            else
            {
                errTip += "●必选项 [LocalSettings]节 Factory 键值未定义\n";
                err = false;
            }
            strBuf = ReadIniFile("LocalSettings", "Model", sIniPath);
            if (strBuf.Length != 0)
            {
                sModel = strBuf.ToUpper().Replace(" ", "");
                if (sFactory != "R1" && sFactory != "R5")
                {
                    errTip += "●必选项 [LocalSettings]节 Model 键值设定错误\n";
                    err = false;
                }
            }
            else
            {
                errTip += "●必选项 [LocalSettings]节 Model 键值未定义\n";
                err = false;
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
                errTip += "●必选项 [ComPort]节 TVEnable键值未定义\n";
               // err = false;
                sTvEnable = "N";
            }

            strBuf = ReadIniFile("ComPort", "GDMEnable", sIniPath);
            if (strBuf.Length != 0)
            {
                sGdmEnable = strBuf.Trim().ToUpper();
            }
            else
            {
                //errTip += "●必选项 [ComPort]节 GDMEnable 键值未定义\n";
                //err = false;
                sGdmEnable = "N";
            }
            strBuf = ReadIniFile("ComPort", "IOCardEnable", sIniPath);
            if (strBuf.Length != 0)
            {
                sIOCardEnable = strBuf.Trim().ToUpper();
            }
            else
            {
                errTip += "可选项 [ComPort]节 IOCardEnable 键值未定义\n";
                //err = false;
                sIOCardEnable = "N";
            }
            strBuf = ReadIniFile("ComPort", "SICKEnable", sIniPath);
            if (strBuf.Length != 0)
            {
                sSickEnable = strBuf.Trim().ToUpper();
            }
            else
            {
                errTip += "可选项 [ComPort]节 SICKEnable 键值未定义\n";
                //err = false;
                sSickEnable = "N";
            }

            strBuf = ReadIniFile("ComPort", "JESEnable", sIniPath);
            if (strBuf.Length != 0)
            {
                sJESEnable = strBuf.Trim().ToUpper();
            }
            else
            {
                errTip += "可选项 [ComPort]节 SICKEnable 键值未定义\n";
                //err = false;
                sJESEnable = "N";
            }

            strBuf = ReadIniFile("ComPort", "MHLEnable", sIniPath);
            if (strBuf.Length != 0)
            {
                sMhlEnable = strBuf.Trim().ToUpper();
            }
            else
            {
                errTip += "可选项 [ComPort]节 MHLEnable 键值未定义\n";
                //err = false;
                sMhlEnable = "N";
            }
            strBuf = ReadIniFile("ComPort", "DoorEnable", sIniPath);
            if (strBuf.Length != 0)
            {
                sDoorEnable = strBuf.Trim().ToUpper();
            }
            else
            {
                errTip += "可选项 [ComPort]节 DoorEnable 键值未定义\n";
                //err = false;
                sDoorEnable = "N";
            }
            strBuf = ReadIniFile("ComPort", "TVPort", sIniPath);
            if (strBuf.Length != 0)
            {
                iTvPort = Convert.ToInt32(strBuf);
                if (iTvPort < 1 || iTvPort > 30)
                {
                    errTip += "●必选项 [ComPort]节 TVPort 键值设定错误\n";
                    //err = false;
                }
            }
            else
            {
                errTip += "●必选项 [ComPort]节 TVPort 键值未定义\n";
                //err = false;
                iTvPort = 0;
            }
            strBuf = ReadIniFile("ComPort", "TVSettings", sIniPath);
            if (strBuf.Length != 0)
            {
                sTvSettings = strBuf.ToUpper().Replace(" ", "");
            }
            else
            {
                errTip += "●必选项 [ComPort]节 TVSettings 键值未定义\n";
               // err = false;
            }
            strBuf = ReadIniFile("ComPort", "GDMPort", sIniPath);
            if (strBuf.Length != 0)
            {
                iGdmPort = Convert.ToInt32(strBuf);
                if (iGdmPort < 1 || iGdmPort > 20)
                {
                    errTip += "可选项 [ComPort]节 GDMPort 键值设定错误\n";
                    //err = false;
                }
            }
            else
            {
                //errTip += "可选项 [ComPort]节 GDMPort 键值未定义\n";
                iGdmPort = 0;
                //err = false;
            }
            strBuf = ReadIniFile("ComPort", "GDMSettings", sIniPath);
            if (strBuf.Length != 0)
            {
                sGdmSettings = strBuf.ToUpper().Replace(" ", "");
            }
            else
            {
                //errTip += "可选项 [ComPort]节 GDMSettings 键值未定义\n";
                //err = false;
            }
            strBuf = ReadIniFile("ComPort", "IOCardPort", sIniPath);
            if (strBuf.Length != 0)
            {
                iIOCardPort = Convert.ToInt32(strBuf);
                if (iIOCardPort < 1 || iIOCardPort > 30)
                {
                    errTip += "可选项 [ComPort]节 IOCardPort 键值设定错误\n";
                    //err = false;
                }
            }
            else
            {
                errTip += "可选项 [ComPort]节 IOCardPort 键值未定义\n";
                iIOCardPort = 0;
                //err = false;
            }
            strBuf = ReadIniFile("ComPort", "IOCardSettings", sIniPath);
            if (strBuf.Length != 0)
            {
                sIOCardSettings = strBuf.ToUpper().Replace(" ", "");
            }
            else
            {
                errTip += "可选项 [ComPort]节 GDMSettings 键值未定义\n";
                //err = false;
            }
            strBuf = ReadIniFile("ComPort", "SICKPort", sIniPath);
            if (strBuf.Length != 0)
            {
                iSickPort = Convert.ToInt32(strBuf);
                if (iSickPort < 1 || iSickPort > 20)
                {
                    errTip += "可选项 [ComPort]节 SickPort 键值设定错误\n";
                    //err = false;
                }
            }
            else
            {
                errTip += "可选项 [ComPort]节 SickPort 键值未定义\n";
                iSickPort = 0;
                //err = false;
            }
            strBuf = ReadIniFile("ComPort", "SICKSettings", sIniPath);
            if (strBuf.Length != 0)
            {
                sSickSettings = strBuf.ToUpper().Replace(" ", "");
            }
            else
            {
                errTip += "可选项 [ComPort]节 CP310Settings 键值未定义\n";
                //err = false;
            }

            strBuf = ReadIniFile("ComPort", "MHLPort", sIniPath);
            if (strBuf.Length != 0)
            {
                iMhlPort = Convert.ToInt32(strBuf);
                if (iMhlPort < 1 || iMhlPort > 20)
                {
                    errTip += "可选项 [ComPort]节 MHLPort 键值设定错误\n";
                }
            }
            else
            {
                errTip += "可选项 [ComPort]节 MHLPort 键值未定义\n";
                //err = false;
            }

            strBuf = ReadIniFile("ComPort", "DoorPort", sIniPath);
            if (strBuf.Length != 0)
            {
                iDoorPort = Convert.ToInt32(strBuf);
                if (iDoorPort < 1 || iDoorPort > 20)
                {
                    errTip += "可选项 [ComPort]节 DoorPort 键值设定错误\n";
                }
            }
            else
            {
                errTip += "可选项 [ComPort]节 DoorPort 键值未定义\n";
                //err = false;
            }
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
                errTip += "可选项 [TEST]节 MHL 键值未定义\n";
                //err = false;
            }
            strBuf = ReadIniFile("TEST", "BTFW", sIniPath);
            if (strBuf.Length != 0)
            {
                sBtFw = strBuf.ToUpper().Replace(" ", "");
            }
            else
            {
                errTip += "可选项 [TEST]节 BTFW 键值未定义\n";
                //err = false;
            }
            strBuf = ReadIniFile("TEST", "SSID", sIniPath);
            if (strBuf.Length != 0)
            {
                sSsid = strBuf.Trim();
            }
            else
            {
                errTip += "可选项 [TEST]节 SSID 键值未定义\n";
                //err = false;
            }
            strBuf = ReadIniFile("TEST", "PWD", sIniPath);
            if (strBuf.Length != 0)
            {
                sPwd = strBuf.ToUpper().Replace(" ", "");
            }
            else
            {
                errTip += "可选项 [TEST]节 PWD 键值未定义\n";
                //err = false;
            }
            strBuf = ReadIniFile("TEST", "UDPSERVER", sIniPath);
            if (strBuf.Length != 0)
            {
                sUdpServer = strBuf.ToUpper().Replace(" ", "");
            }
            else
            {
                errTip += "可选项 [TEST]节 UDPSERVER 键值未定义\n";
                //err = false;
            }
            strBuf = ReadIniFile("TEST", "DATASERVER", sIniPath);
            if (strBuf.Length != 0)
            {
                sDataServer = strBuf.ToUpper().Replace(" ", "");
            }
            else
            {
                errTip += "可选项 [TEST]节 DATASERVER 键值未定义\n";
                //err = false;
            }
            strBuf = ReadIniFile("TEST", "BTMAC", sIniPath);
            if (strBuf.Length != 0)
            {
                sBtMac = strBuf.ToLower().Replace(" ", "");
            }
            else
            {
                errTip += "可选项 [TEST]节 BTMAC 键值未定义\n";
                //err = false;
            }
            strBuf = ReadIniFile("TEST", "HDCP", sIniPath);
            if (strBuf.Length != 0)
            {
                sHdcp = strBuf.ToUpper().Replace(" ", "");
            }
            else
            {
                errTip += "可选项 [TEST]节 HDCP 键值未定义\n";
                //err = false;
            }
            strBuf = ReadIniFile("TEST", "Ethernet", sIniPath);
            if (strBuf.Length != 0)
            {
                sEthernet = strBuf.ToUpper().Replace(" ", "");
            }
            else
            {
                errTip += "可选项 [TEST]节 Ethernet 键值未定义\n";
                //err = false;
            }
            //strBuf = ReadIniFile("TEST", "FW", sIniPath);
            //if (strBuf.Length != 0)
            //{
            //    sFw = strBuf.ToUpper().Replace(" ", "");
            //}
            //else
            //{
            //    errTip += "●必选项 [TEST]节 FW 键值未定义\n";
            //    err = false;
            //}
            strBuf = ReadIniFile("TEST", "DotFW", sIniPath);
            if (strBuf.Length != 0)
            {
                sDotFw = strBuf.ToUpper().Replace(" ", "");
            }
            else
            {
                errTip += "可选项 [TEST]节 DotFW 键值未定义\n";
                //err = false;
            }

            strBuf = ReadIniFile("TEST", "SBSN", sIniPath);
            if (strBuf.Length != 0)
            {
                sSbSn = strBuf.ToUpper().Replace(" ", "");
            }
            else
            {
                errTip += "可选项 [TEST]节 SBSN 键值未定义\n";
                //err = false;
            }

            strBuf = ReadIniFile("TEST", "BsPath", sIniPath);
            if (strBuf.Length != 0)
            {
                sBsPath = strBuf.Trim();
            }
            else
            {
                errTip += "可选项 [TEST]节 BsPath 键值未定义\n";
                //err = false;
            }

            //strBuf = ReadIniFile("TEST", "micPath", sIniPath);
            //if (strBuf.Length != 0)
            //{
            //    smicPath = strBuf.Trim();
            //}
            //else
            //{
            //    errTip += "可选项 [TEST]节 micPath 键值未定义\n";
            //    //err = false;
            //}

            //strBuf = ReadIniFile("TEST", "buildPath", sIniPath);
            //if (strBuf.Length != 0)
            //{
            //    sbuildPath = strBuf.Trim();
            //}
            //else
            //{
            //    errTip += "可选项 [TEST]节 micPath 键值未定义\n";
            //    //err = false;
            //}

            //strBuf = ReadIniFile("TEST", "Cmd1Path", sIniPath);
            //if (strBuf.Length != 0)
            //{
            //    sCmd1Path = strBuf.Trim();
            //    try
            //    {
            //        if (File.Exists(sCmd1Path) == true)
            //        {
            //            sCmd1PathFolder = Path.GetDirectoryName(sCmd1Path);
            //        }
            //        else
            //            sCmd1PathFolder = Path.GetDirectoryName(Directory.GetCurrentDirectory() + @"\" + sCmd1Path);

            //    }
            //    catch
            //    {
            //        sCmd1PathFolder = Path.GetDirectoryName(Directory.GetCurrentDirectory() + @"\" + sCmd1Path);
            //    }
            //}
            //else
            //{
            //    errTip += "可选项 [TEST]节 Cmd1Path 键值未定义\n";
            //    //err = false;
            //}

            //strBuf = ReadIniFile("TEST", "Cmd2Path", sIniPath);
            //if (strBuf.Length != 0)
            //{
            //    sCmd2Path = strBuf.Trim();
            //    try
            //    {
            //        if (File.Exists(sCmd2Path) == true)
            //        {
            //            sCmd2PathFolder = Path.GetDirectoryName(sCmd2Path);
            //        }
            //        else
            //            sCmd2PathFolder = Path.GetDirectoryName(Directory.GetCurrentDirectory() + @"\" + sCmd2Path);
            //    }
            //    catch
            //    {
            //        sCmd2PathFolder = Path.GetDirectoryName(Directory.GetCurrentDirectory() + @"\" + sCmd2Path);
            //    }
            //}
            //else
            //{
            //    //errTip += "可选项 [TEST]节 Cmd2Path 键值未定义\n";
            //    //err = false;
            //}
           // GlobalConfig.sCmd3Path；
            //strBuf = ReadIniFile("TEST", "Cmd3Path", sIniPath);
            //if (strBuf.Length != 0)
            //{
            //    sCmd3Path = strBuf.Trim();
            //    try
            //    {
            //        bool flag = false;
            //        try
            //        {
            //            if (File.Exists(sCmd3Path) == false)
            //            {
            //                throw new Exception();
            //            }
            //            flag = true;
            //        }
            //        catch { }
            //        try
            //        {
            //            if (Directory.Exists(sCmd3Path) == false)
            //            {
            //                throw new Exception();
            //            }
            //            flag = true;
            //        }
            //        catch { }
            //        if (!flag) throw new Exception();
            //    }
            //    catch
            //    {
            //        errTip += "可选项 [TEST]节 Cmd3Path 键值定义错误\n";
            //    }
            //}
            //else
            //{
            //    //errTip += "可选项 [TEST]节 Cmd3Path 键值未定义\n";
            //    //err = false;
            //}

            //strBuf = ReadIniFile("TEST", "Cmd4Path", sIniPath);
            //if (strBuf.Length != 0)
            //{
            //    sCmd4Path = strBuf.Trim();
            //    try
            //    {
            //        bool flag = false;
            //        try
            //        {
            //            if (File.Exists(sCmd4Path) == false)
            //            {
            //                throw new Exception();
            //            }
            //            flag = true;
            //        }
            //        catch { }
            //        try
            //        {
            //            if (Directory.Exists(sCmd4Path) == false)
            //            {
            //                throw new Exception();
            //            }
            //            flag = true;
            //        }
            //        catch { }
            //        if (!flag) throw new Exception();
            //    }
            //    catch
            //    {
            //        errTip += "可选项 [TEST]节 Cmd4Path 键值定义错误\n";
            //    }
            //}
            //else
            //{
            //    //errTip += "可选项 [TEST]节 Cmd4Path 键值未定义\n";
            //    //err = false;
            //}

            //strBuf = ReadIniFile("TEST", "Cmd5Path", sIniPath);
            //if (strBuf.Length != 0)
            //{
            //    sCmd5Path = strBuf.Trim();
            //    try
            //    {
            //        bool flag = false;
            //        try
            //        {
            //            if (File.Exists(sCmd5Path) == false)
            //            {
            //                throw new Exception();
            //            }
            //            flag = true;
            //        }
            //        catch { }
            //        try
            //        {
            //            if (Directory.Exists(sCmd5Path) == false)
            //            {
            //                throw new Exception();
            //            }
            //            flag = true;
            //        }
            //        catch { }
            //        if (!flag) throw new Exception();
            //    }
            //    catch
            //    {
            //        errTip += "可选项 [TEST]节 Cmd5Path 键值定义错误\n";
            //    }
            //}
            //else
            //{
            //    //errTip += "可选项 [TEST]节 Cmd5Path 键值未定义\n";
            //    //err = false;
            //}

            //strBuf = ReadIniFile("TEST", "AccessDbPath", sIniPath);
            //if (strBuf.Length != 0)
            //{
            //    sAccessDbPath = strBuf.Trim();
            //}
            //else
            //{
            //    errTip += "可选项 [TEST]节 AccessDbPath 键值未定义\n";
            //    //err = false;
            //}

            strBuf = ReadIniFile("TEST", "CheckBut", sIniPath);
            if (strBuf.Length != 0)
            {
                sCheckBut = strBuf.ToUpper().Trim();
            }
            else
            {
                errTip += "可选项 [TEST]节 CheckBut 键值未定义\n";
                sCheckBut = "N";
                //err = false;
            }

            strBuf = ReadIniFile("TEST", "BtRssiOffset", sIniPath);
            if (strBuf.Length != 0)
            {
                iBtRssiOffset = Convert.ToInt32(strBuf.Trim());
            }
            else
            {
                errTip += "可选项 [TEST]节 BtRssiOffset 键值未定义\n";
                iBtRssiOffset = 0;
                //err = false;
            }

            strBuf = ReadIniFile("TEST", "BiWifiSsid", sIniPath);
            if (strBuf.Length != 0)
            {
                sBiWifiSsid = strBuf.Trim();
            }
            else
            {
                errTip += "可选项 [TEST]节 BiWifiSsid 键值未定义\n";
                //err = false;
            }

            strBuf = ReadIniFile("TEST", "BiWifiPwd", sIniPath);
            if (strBuf.Length != 0)
            {
                sBiWifiPwd = strBuf.Trim();
            }
            else
            {
                errTip += "可选项 [TEST]节 BiWifiPwd 键值未定义\n";
                //err = false;
            }

            strBuf = ReadIniFile("TEST", "BiWifiGateway", sIniPath);
            if (strBuf.Length != 0)
            {
                sBiWifiGateway = strBuf.Trim();
            }
            else
            {
                errTip += "可选项 [TEST]节 BiWifiGateway 键值未定义\n";
                //err = false;
            }
            #endregion

            #region 如果配置文件有遗漏设置，报错提示
            if (errTip.Length != 0)
            {
                //测试
                unCounter = 0;
                unCounter2 = 0; //记录换行数
                do
                {
                    if (errTip[unCounter] == '\n')
                    {
                        ++unCounter2;
                        if (unCounter2 > 20)
                        {
                            errTip = errTip.Insert(unCounter, "\n...\n[更多错误信息已隐藏并未显示]" + "\n\n配置文件：" + sIniPath + "\0");
                            break;
                        }
                    }
                    ++unCounter;
                } while (unCounter < errTip.Length);

                if (!err) //err 为 false 时，阻断错误
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
    }
}
