using Oracle.ManagedDataAccess.Client;
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
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;
//using Oracle.ManagedDataAccess;

namespace PRETEST.TestProcess.RST
{
    class Telly
    {
    }
}

namespace PRETEST
{
    public partial class FormMain
    {
        public static Dictionary<string, string> Data { get; private set; }
        bool RST_Telly()
        {
            string rst = string.Empty;
            string cmd = string.Empty;
            int retry = 0;

            string checkResult = "FAIL";
            string failstring = string.Empty;
            string TestName = string.Empty;

            Data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            string[] testItem = {"LIGHT_SENSOR",
                                 "MOTOR",
                                 "TEST_VOLUME",
                                 "CAMERA_TEST",
                                 "RADAR",
                                 "MICROP_TEST",
                                 "CHECK_KEY",
                                 "READ_KEY",
                                 //"WHITE_FIELD",
                                 "DVB",
                                 "MEDIA",
                                 "LED_SCREEN_TEST",
                                 "BACK_LIGHT",
                                 "HDMI_IN",
                                 "OPEN_ADB",
                                 "WIFI",
                                 "WIFI_5G",
                                 "ETHERNET",
                                 "BLUETOOTH",
                                 "MICROP_TEST_OPEN_VALUE",
                                 "MICROP_TEST_CLOSE_VALUE",
                                 "WIFI_5G_RSSI_VALUE",
                                 "WIFI_24G_RSSI_VALUE",
                                 "BLUETOOTH_RSSI_VALUE",
                                 "AGING_TIME_VALUE",
                                 "CEC"};

            // 關閉ADB, Đầu tiên đóng ADB
            RunAdbCmd("taskkill /f /im adb.exe /t", out rst, 1);

            //清除測試結果
            try
            {
                if (File.Exists("command_ack.txt")) File.Delete("command_ack.txt");
                SetMsg("移除 command_ack.txt, Xóa file command_ack.txt", UDF.COLOR.WORK);
                cmd = "adb shell rm -rf /sdcard/Download/command_ack.txt";
                if (GlobalConfig.iRunMode == 0)
                {
                    run_cmd_and_check_link(rst, retry, cmd);
                }
                else
                {
                    if (!run_cmd_and_check_link(rst, retry, cmd)) return false;
                }
            }
            catch (Exception e)
            {
                SetMsg(e.Message, UDF.COLOR.FAIL);
                return false;
            }

            // 1. wifi 2.4g 和 5G會一起測試
            SetMsg("執行wifi 2.4g和5g, Chạy kiểm tra wifi 2.4g và 5g", UDF.COLOR.WORK);
            cmd = "adb shell dumpsys activity com.utsmta.app/com.nes.factorytest.ui.activity.Main2Activity startCaseWifi";
            TestName = "WiFI";
            try
            {
                if (GlobalConfig.iRunMode == 0)
                {
                    if (!run_cmd_and_check_link(rst, retry, cmd)) return false;
                    if (!check_command_ack(TestName, "command_ack.txt", "PASS")) return false;
                }
                else
                {
                    if (!run_cmd_and_check_link(rst, retry, cmd)) return false;
                    if (!check_command_ack(TestName,"command_ack.txt","PASS")) return false;
                }

            }
            catch (Exception e)
            {
                SetMsg(e.Message, UDF.COLOR.FAIL);
                return false;
            }

            //清除測試結果
            try
            {
                if (File.Exists("command_ack.txt")) File.Delete("command_ack.txt");
                SetMsg("移除 command_ack.txt, Xóa file command_ack.txt", UDF.COLOR.WORK);
                cmd = "adb shell rm -rf /sdcard/Download/command_ack.txt";
                
                if (GlobalConfig.iRunMode == 0)
                {
                    run_cmd_and_check_link(rst, retry, cmd);
                }
                else
                {
                    if (!run_cmd_and_check_link(rst, retry, cmd)) return false;
                }
            }
            catch (Exception e)
            {
                SetMsg(e.Message, UDF.COLOR.FAIL);
                return false;
            }

            // 2. ethernet 測試
            SetMsg("執行 Ethernet 測試, Thực thi kiểm tra Ethernet", UDF.COLOR.WORK);
            cmd = "adb shell dumpsys activity com.utsmta.app/com.nes.factorytest.ui.activity.Main2Activity startCaseEth";
            TestName = "Ethernet";
            try
            {
                if (GlobalConfig.iRunMode == 0)
                {
                    if (!run_cmd_and_check_link(rst, retry, cmd)) return false;
                    if (!check_command_ack(TestName, "command_ack.txt", "PASS")) return false;
                }
                else
                {
                    if (!run_cmd_and_check_link(rst, retry, cmd)) return false;
                    if (!check_command_ack(TestName)) return false;
                }
            }
            catch (Exception e)
            {
                SetMsg(e.Message, UDF.COLOR.FAIL);
                return false;
            }

            //清除測試結果
            try
            {
                if (File.Exists("command_ack.txt")) File.Delete("command_ack.txt");
                SetMsg("移除 command_ack.txt, Xóa file command_ack.txt", UDF.COLOR.WORK);
                cmd = "adb shell rm -rf /sdcard/Download/command_ack.txt";

                if (GlobalConfig.iRunMode == 0)
                {
                    run_cmd_and_check_link(rst, retry, cmd);
                }
                else
                {
                    if (!run_cmd_and_check_link(rst, retry, cmd)) return false;
                }
            }
            catch (Exception e)
            {
                SetMsg(e.Message, UDF.COLOR.FAIL);
                return false;
            }

            // 3. BT 測試
            SetMsg("執行 BT 測試, Thực thi kiểm tra BT", UDF.COLOR.WORK);
            cmd = "adb shell dumpsys activity com.utsmta.app/com.nes.factorytest.ui.activity.Main2Activity startCaseBt";
            TestName = "BT";
            try
            {
                if (GlobalConfig.iRunMode == 0)
                {
                    if (!run_cmd_and_check_link(rst, retry, cmd)) return false;
                    if (!check_command_ack(TestName, "command_ack.txt", "PASS")) return false;
                }
                else
                {
                    if (!run_cmd_and_check_link(rst, retry, cmd)) return false;
                    if (!check_command_ack(TestName)) return false;
                }
            }
            catch (Exception e)
            {
                SetMsg(e.Message, UDF.COLOR.FAIL);
                return false;
            }

            // 4. 備份產品測試結果
            SetMsg("備份產品測試結果, Sao lưu kết quả kiểm tra sản phẩm", UDF.COLOR.WORK);
            cmd = "adb shell cat /sdcard/app_result_rework.txt";
            string folderPath = $"\\{GlobalConfig.sLogServerIP}\\Logi\\reggie\\Telly\\LOG\\RST\\{DateTime.Now.ToString("yyyy_MM_dd")}";
            string filename = DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss") + "_" + textBoxSn.Text + "_app_result_rework.txt";
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
                //SetMsg(rst, UDF.COLOR.WORK);
                if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);
                File.WriteAllText(folderPath + @"\" + filename, rst);
                if (File.Exists(folderPath + @"\" + filename))
                { 
                    SetMsg($"備份產品測試結果成功, Sao lưu kết quả kiểm tra sản phẩm thành công: {folderPath}\\{filename}", UDF.COLOR.WORK);
                }
                folderPath = $@"D:\Telly\LOG\RST\{DateTime.Now.ToString("yyyy_MM_dd")}";
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }
                File.WriteAllText(folderPath + @"\" + filename, rst);
                if (File.Exists(folderPath + @"\" + filename))
                {
                    SetMsg($"備份產品測試結果成功, Sao lưu kết quả kiểm tra sản phẩm thành công: {folderPath}\\{filename}", UDF.COLOR.WORK);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show($"備份產品測試結果失敗, Sao lưu kết quả kiểm tra sản phẩm thất bại: {e.Message}");
                SetMsg(e.Message, UDF.COLOR.FAIL);
                return false;
            }

            // 5. 檢查前面所有測試站結果
            SetMsg("檢查產品所有測試結果, Kiểm tra tất cả kết quả thử nghiệm của sản phẩm", UDF.COLOR.WORK);
            try
            {
                foreach (string line in (File.ReadAllLines(folderPath + @"\" + filename)))
                {
                    var trimmed = line.Trim();
                    if (string.IsNullOrWhiteSpace(trimmed) || trimmed.StartsWith(";")) continue;

                    foreach (string item in trimmed.Split(' '))
                    {
                        if (item.Contains("="))
                        {
                            var parts = item.Split(new[] { '=' }, 2);
                            if (parts.Length == 2)
                            {
                                var key = parts[0].Trim();
                                var value = parts[1].Trim();
                                Data[key] = value;
                                //SetMsg($"解析: {key} = {value}", UDF.COLOR.WORK); // 可選，除錯用
                            }
                        }
                    }
                }

                foreach (string item in testItem)
                {
                    if (Regex.IsMatch(item, "VALUE")) continue; // 跳過VALUE項目
                    if (!Regex.IsMatch(rst, $"{item}=P"))
                    {
                        SetMsg($"{item} FAIL, 請回到該站重新測試, THẤT ​​BẠI , Vui lòng quay lại trang web này và kiểm tra lại", UDF.COLOR.FAIL);
                        checkResult = "FAIL";
                        failstring += $"{item},";
                        if (GlobalConfig.iRunMode == 1)
                        {
                            // 如果是線上模式, 不允許繼續
                            SetMsg("線上模式不允許繼續, Chế độ trực tuyến không cho phép tiếp tục", UDF.COLOR.FAIL);
                            return false;
                        }
                    }
                    else
                    {
                        checkResult = "PASS";
                        SetMsg($"{item} PASS, VƯỢT QUA", UDF.COLOR.WORK);
                    }
                }
            }
            catch (Exception e)
            {
                SetMsg(e.Message, UDF.COLOR.FAIL);
                return false;
            }

            // 6.  OTA 暫停72小時
            SetMsg("執行 OTA 暫停 72 小時, Tạm dừng OTA trong 72 giờ", UDF.COLOR.WORK);
            cmd = "adb shell am start -a android.intent.action.VIEW -d \"ttm://checkupdate?block=72\"";
            try
            {
                if (GlobalConfig.iRunMode == 0)
                {
                    run_cmd_and_check_link(rst, retry, cmd);
                }
                else
                {
                    if (!run_cmd_and_check_link(rst, retry, cmd)) return false;
                }
            }
            catch (Exception e)
            {
                SetMsg(e.Message, UDF.COLOR.FAIL);
                return false;
            }
            
            // 7. 改成出廠設定, 出貨FW , offline略過
            try
            {
                if (GlobalConfig.iRunMode == 1 && checkResult == "PASS")
                {
                    SetMsg("執行 Reset mode, Thực thi chế độ Reset", UDF.COLOR.WORK);
                    cmd = "adb shell dumpsys activity com.utsmta.app/com.nes.factorytest.ui.activity.Main2Activity reset";
                    run_cmd_and_check_link(rst, retry, cmd);

                    // 下完reset 要等一下看安卓機器人出現
                    SetMsg("等待25秒, Chờ 25 giây", UDF.COLOR.WORK);
                    Delay(25000);
                }
                else if (checkResult == "FAIL")
                {
                    SetMsg("不執行 Reset mode, Không thực hiện chế độ đặt lại", UDF.COLOR.WORK);
                    SetMsg($"錯誤項目, Mục bị lỗi", UDF.COLOR.WORK);
                    SetMsg($"{failstring.TrimEnd(',')}", UDF.COLOR.FAIL);
                    return false;
                }
                else
                {
                    SetMsg("不執行 Reset mode, Không thực hiện chế độ đặt lại", UDF.COLOR.WORK);
                }
            }
            catch (Exception e)
            {
                SetMsg(e.Message, UDF.COLOR.FAIL);
                return false;
            }
            
            // 8. 上傳Oracle
            SetMsg("上傳資料到Oracle INT_Telly_INFO, Tải dữ liệu lên Oracle INT_Telly_INFO", UDF.COLOR.WORK);
            string targetSN = textBoxSn.Text;
            
            try
            {
                foreach (string item in testItem)
                {
                    if (!Data.ContainsKey(item)) continue;
                    //if (!Data.TryGetValue(item,out string testvalue)) continue;

                    string updateSql = $@"
                                        UPDATE INS_TELLY_INFO
                                        SET {item} = :itemValue
                                        WHERE (LABEL_SN, INS_DATE) = (
                                            SELECT LABEL_SN, INS_DATE FROM (
                                                SELECT LABEL_SN, INS_DATE FROM INS_TELLY_INFO
                                                WHERE LABEL_SN = :sn
                                                ORDER BY INS_DATE DESC
                                            ) WHERE ROWNUM = 1
                                        )";
                    
                    string value = Data[item]?.Trim() ?? "";
                    int maxLength = 20;
                    if (item == "MICROP_TEST_OPEN_VALUE" || item == "MICROP_TEST_CLOSE_VALUE" || item == "AGING_TIME_VALUE")
                        maxLength = 100;
                    value = SafeTrim(value, maxLength);

                    using (OracleCommand oraCmd = new OracleCommand(updateSql, oraConn))
                    {
                        
                        oraCmd.Parameters.Add(new OracleParameter("itemValue", value));
                        oraCmd.Parameters.Add(new OracleParameter("sn", targetSN));

                        int rows = oraCmd.ExecuteNonQuery();
                        SetMsg($"更新欄位 {item}： {Data[item]} {rows} 筆", UDF.COLOR.WORK);
                    }
                }
            }
            catch (Exception e)
            {
                SetMsg(e.Message, UDF.COLOR.FAIL);
                return false;
            }
            SetMsg("系統已完成上傳 Oracle 資料, Hệ thống đã hoàn tất tải dữ liệu lên ORACLE", UDF.COLOR.WORK);

            // 關閉ADB, Đầu tiên đóng ADB
            RunAdbCmd("taskkill /f /im adb.exe /t", out rst, 1); 
            return true;
        }
    }
}