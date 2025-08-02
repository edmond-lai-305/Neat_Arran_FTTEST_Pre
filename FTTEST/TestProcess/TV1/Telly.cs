using PRETEST.AppConfig;
using PRETEST.Database;
using PRETEST.SDriver;
using PRETEST.UDFs;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PRETEST.TestProcess.TV1
{
    class Telly
    {
    }
}

namespace PRETEST
{
    public partial class FormMain
    {
        bool TV1_Telly()
        {
            string rst = string.Empty;
            string cmd = string.Empty;
            int retry = 0;

            // 關閉ADB, Đầu tiên đóng ADB
            RunAdbCmd("taskkill /f /im adb.exe /t", out rst, 1);

            //儲存MIC測試數據
            SetMsg("儲存 MIC 測試數據, Lưu dữ liệu kiểm tra MIC", UDF.COLOR.WORK);
            cmd = "adb shell cat /sdcard/Download/command_ack.txt";
            try
            {
                if (!RunAdbCmd(cmd, out rst))
                {
                    while (retry <= 3)
                    {
                        retry++;
                        SetMsg($"重新下指令:{retry}, Ra lệnh lại:{retry}", UDF.COLOR.WORK);
                        if (RunAdbCmd(cmd, out rst)) break;
                        SetMsg("RunAdbCmd 執行失敗, Thực thi thất bại", UDF.COLOR.FAIL);
                        return false;
                    }
                }
                if (Regex.IsMatch(rst, ".no devices/emulators found"))
                {
                    while (retry <= 3)
                    {
                        retry++;
                        SetMsg($"重新下指令:{retry}, Ra lệnh lại:{retry}", UDF.COLOR.WORK);
                        if (RunAdbCmd(cmd, out rst)) break;
                        SetMsg("找不到機器, Không tìm thấy máy", UDF.COLOR.FAIL);
                        return false;
                    }
                }

                SetMsg(rst, UDF.COLOR.WORK);
                
                string folderPath = $@"\\{GlobalConfig.sLogServerIP}\Logi\reggie\Telly\LOG\MIC\{DateTime.Now.ToString("yyyy_MM_dd")}";
                string filename = DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss") + "_" + textBoxSn.Text + ".txt";
                if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);
                SetMsg($"儲存檔案到 {folderPath} 路徑, Lưu tệp vào đường dẫn {folderPath}", UDF.COLOR.WORK);
                File.WriteAllText(folderPath + @"\" + filename, rst);
                folderPath = $@"D:\Telly\LOG\MIC\{DateTime.Now.ToString("yyyy_MM_dd")}";
                if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);
                
                File.WriteAllText(folderPath + @"\" + filename, rst);
                SetMsg($"儲存檔案到 {folderPath} 路徑, Lưu tệp vào đường dẫn {folderPath}", UDF.COLOR.WORK);
            }
            catch (Exception e)
            {
                SetMsg(e.Message, UDF.COLOR.FAIL);
                return false;
            }

            //清除測試結果
            //try
            //{
            //    if (File.Exists("command_ack.txt")) File.Delete("command_ack.txt");
            //    SetMsg("移除 command_ack.txt, Xóa file command_ack.txt", UDF.COLOR.WORK);
            //    cmd = "adb shell rm -rf /sdcard/Download/command_ack.txt";
            //    run_cmd_and_check_link(rst, retry, cmd);
            //}
            //catch (Exception e)
            //{
            //    SetMsg(e.Message, UDF.COLOR.FAIL);
            //    return false;
            //}
            
            //1. 讀key測試
            SetMsg("執行 Read key 測試, Kiểm tra đọc phím", UDF.COLOR.WORK);
            cmd = "adb shell dumpsys activity com.utsmta.app/com.nes.factorytest.ui.activity.Main2Activity startCaseReadKey";
            try
            {
                run_cmd_and_check_link(rst, retry, cmd);
                if (!check_command_ack("Read Key")) return false;
            }
            catch (Exception e)
            {
                SetMsg(e.Message, UDF.COLOR.FAIL);
                return false;
            }
            
            //清除測試結果
            //try
            //{
            //    if (File.Exists("command_ack.txt")) File.Delete("command_ack.txt");
            //    SetMsg("移除 command_ack.txt, Xóa file command_ack.txt", UDF.COLOR.WORK);
            //    cmd = "adb shell rm -rf /sdcard/Download/command_ack.txt";
            //    if(!run_cmd_and_check_link(rst, retry, cmd)) return false;
            //}
            //catch (Exception e)
            //{
            //    SetMsg(e.Message, UDF.COLOR.FAIL);
            //    return false;
            //}

            //2. 校驗key測試
            SetMsg("執行 Check key 測試, Kiểm tra đọc phím", UDF.COLOR.WORK);
            cmd = "adb shell dumpsys activity com.utsmta.app/com.nes.factorytest.ui.activity.Main2Activity startCaseCheckKey";
            try
            {
                run_cmd_and_check_link(rst, retry, cmd);
                if (!check_command_ack("Check Key")) return false;
            }
            catch (Exception e)
            {
                SetMsg(e.Message, UDF.COLOR.FAIL);
                return false;
            }
            
            // 關閉ADB, Đầu tiên đóng ADB
            RunAdbCmd("taskkill /f /im adb.exe /t", out rst, 1);
            return true;
        }
    }
}