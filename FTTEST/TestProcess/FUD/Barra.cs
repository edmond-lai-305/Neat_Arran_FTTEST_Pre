using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Windows.Forms;
using PRETEST.AppConfig;
using PRETEST.SDriver;

namespace PRETEST.TestProcess.FUD
{
    class Barra
    {
    }
}

namespace PRETEST
{
    public partial class FormMain
    {
        bool FUD_Barra()
        {
            if (CheckUpgradeToolsVersion_Barra() == false) return false;
            if (Barra_UpgradeFW_Batch() == false) return false;
            if (Barra_UpgradeFW() == false) return false;

            return true;
        }
        bool CheckUpgradeToolsVersion_Barra()
        {
            string text;
            string fudfw = "";
            string FLAG = "";
            //SDriverX.g_modelInfo.sWord;           
          
            FLAG = wbc.iniFile.GetPrivateStringValue("FW_USER_barra", "flag", @"\\192.168.158.25\\te\\setup\\FTTEST\\FWVISON\\FW.ini");
            if (FLAG == "1")
            {
                fudfw = wbc.iniFile.GetPrivateStringValue("FW_USER_barra", "FW", @"\\

\\te\\setup\\FTTEST\\FWVISON\\FW.ini");
                // FLAG = wbc.iniFile.GetPrivateStringValue("FW_USER", "flag", @"\\192.168.158.25\\te\\setup\\FTTEST\\FWVISON\\FW.ini");
                if (fudfw != "" && fudfw != "0")
                {
                    SetMsg("读取服务器路径1" + "\\192.168.158.25\\te\\setup\\FTTEST\\FWVISON\\FW.ini", UDFs.UDF.COLOR.WORK);
                    SetMsg("服务器:" + fudfw, UDFs.UDF.COLOR.WORK);
                    g_sBarraFUDVer = fudfw;
                }
                
            }
            FLAG = wbc.iniFile.GetPrivateStringValue("FW_USER", "flag", @"\\192.168.158.25\\te\\setup\\FTTEST\\FWVISON\\FW.ini");
            if (FLAG == "1")
            {
                fudfw = wbc.iniFile.GetPrivateStringValue("FW_USER", textBoxPn.Text, @"\\192.168.158.25\\te\\setup\\FTTEST\\FWVISON\\FW.ini");
                if (fudfw != "" && fudfw != "0")
                {
                    SetMsg("读取服务器路径1" + "\\192.168.158.25\\te\\setup\\FTTEST\\FWVISON\\FW.ini", UDFs.UDF.COLOR.WORK);
                    SetMsg("serv:" + fudfw, UDFs.UDF.COLOR.WORK);
                    g_sBarraFUDVer = fudfw;
                }
                //CHECK VER
                // g_sCardFUDVer = fudfw;
            }
            FLAG = wbc.iniFile.GetPrivateStringValue("FW_SO", "flag", @"\\192.168.158.25\\te\\setup\\FTTEST\\FWVISON\\FW.ini");
            if (FLAG == "1")
            {
                fudfw = wbc.iniFile.GetPrivateStringValue("FW_SO", SDriverX.g_modelInfo.sWord, @"\\192.168.158.25\\te\\setup\\FTTEST\\FWVISON\\FW.ini");
                // FLAG = wbc.iniFile.GetPrivateStringValue("FW_USER", "flag", @"\\192.168.158.25\\te\\setup\\FTTEST\\FWVISON\\FW.ini");
                if (fudfw != "" && fudfw != "0")
                {
                    g_sBarraFUDVer = fudfw;
                    SetMsg("读取服务器路径1" + "\\192.168.158.25\\te\\setup\\FTTEST\\FWVISON\\FW.ini", UDFs.UDF.COLOR.WORK);
                    SetMsg("serv:" + fudfw, UDFs.UDF.COLOR.WORK);
                }
            }

            if (fudfw != ""&& fudfw != "0")
            { g_sBarraFUDVer = fudfw; }
            SetMsg("Read user FW ver...", UDFs.UDF.COLOR.WORK);
            try
            {
                text = File.ReadAllText(GlobalConfig.sCmd3Path + "\\" + "version.txt");
                SetMsg("loc:" + text, UDFs.UDF.COLOR.WORK);
            }
            catch (Exception ex)
            {
                SetMsg("Read user FW ver Fail:" + ex.Message, UDFs.UDF.COLOR.FAIL);
                return false;
            }

            //CHECK VER
            if (ResultCheck(text, g_sBarraFUDVer) == false)
            {
                //SetMsg(string.Format("警告：FW版本错误，请向PE、TE核实。'{0}'<>'{1}'", text, g_sBarraFUDVer), UDFs.UDF.COLOR.FAIL);
                SetMsg("警告：FW版本错误，请向PE、TE核实。", UDFs.UDF.COLOR.FAIL);
                return false;
            }
            FW_TEMP = text;
            return true;
        }

        bool Barra_UpgradeFW()
        {
            string ack = string.Empty;
            int nRetryTime = 0;

            SetMsg("FW烧录即将开始...", UDFs.UDF.COLOR.WORK);
            SetMsg("烧录自动进行，请勿手动操作，以免错误", UDFs.UDF.COLOR.WARN);
            SetMsg("烧录时间过长，请耐心等候", UDFs.UDF.COLOR.WARN);

            Process process = new Process();
            process.StartInfo.FileName = "cmd";
            process.StartInfo.Arguments = " /c " + GlobalConfig.sCmd3Path + "\\" + "AndroidFlash.bat"; // " /c adb shell cat /sdcard/Download/command_ack.txt"
            process.StartInfo.UseShellExecute = true;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
            process.StartInfo.CreateNoWindow = false;
            process.Start();
            nRetryTime = 0;
            do
            {
                nRetryTime++;
                if (nRetryTime > 280)
                {
                    SetMsg("FW烧录失败，操作超时", UDFs.UDF.COLOR.FAIL);
                    return false;
                }
                Delay(1000);
            } while (process.HasExited == false);

            if (process.ExitCode != 0xff00)
            {
                SetMsg("退出代码错误," + process.ExitCode.ToString(), UDFs.UDF.COLOR.FAIL);
                return false;
            }

            SetMsg("烧录完成", UDFs.UDF.COLOR.WORK);

            return true;
        }
        bool Barra_UpgradeFW_Batch()
        {
            if (File.Exists(Application.StartupPath + @"\barra_fud.bat"))
            {
                string ack = string.Empty;
                SetMsg("Run barra default cmd...", UDFs.UDF.COLOR.WORK);
                RunAdbCmd(Application.StartupPath + @"\barra_fud.bat", out ack);
            }
            return true;
        }
    }
}