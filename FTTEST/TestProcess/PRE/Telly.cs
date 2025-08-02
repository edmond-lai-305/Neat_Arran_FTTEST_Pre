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
using System.Runtime.Remoting.Lifetime;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace PRETEST
{
    public partial class FormMain
    {
        bool PRE_Telly()
        {
            string rst = string.Empty;
            string cmd = string.Empty;
            int retry = 0;

            // 關閉ADB, Đầu tiên đóng ADB
            RunAdbCmd("taskkill /f /im adb.exe /t", out rst, 1);

            //2. 讀取FW, 比對FW版本
            SetMsg("讀取FW, Đọc FW", UDF.COLOR.WORK);
            cmd = "adb shell dumpsys activity com.utsmta.app/com.nes.factorytest.ui.activity.Main2Activity fwVersion";
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
                SetMsg($"FW: {rst}", UDF.COLOR.WORK);
                if (Regex.IsMatch(rst, $@".{FW_TEMP}."))
                {
                    SetMsg("FW 正確, FW chính xác", UDF.COLOR.WORK);
                }
                else
                {
                    SetMsg("FW 不正確, FW không chính xác", UDF.COLOR.FAIL);
                    return false;
                }

                //rst = "Oldsmobile-factory-1.5.1-8802-b36cdb6a71-TY55_1FB-user-25133.v11.1.8802release-keys (AOSP)"
                Match match = Regex.Match(rst, $@"(\S+{FW_TEMP})-");

                if (textBoxFW.InvokeRequired)
                {
                    textBoxFW.Invoke(new Action(() =>
                    {
                        textBoxFW.Text = match.Groups[1].ToString();
                    }));
                }
                else
                {
                    textBoxFW.Text = match.Groups[1].ToString();
                }
                SetMsg(match.Groups[1].ToString(), UDF.COLOR.WORK);
            }
            catch (Exception e)
            {
                SetMsg(e.Message, UDF.COLOR.FAIL);
                return false;
            }

            //2. 抓MAC
            SetMsg("讀取MAC, Đọc địa chỉ MAC", UDF.COLOR.WORK);
            cmd = "adb shell dumpsys activity com.utsmta.app/com.nes.factorytest.ui.activity.Main2Activity getEmac";
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
                Match match = Regex.Match(rst, @"\S+SUCCESS_(\S+)");
                if (textBoxDC.InvokeRequired)
                {
                    textBoxDC.Invoke(new Action(() =>
                    {
                        textBoxDC.Text = match.Groups[1].ToString();
                    }));
                }
                else
                {
                    textBoxDC.Text = match.Groups[1].ToString();
                }
                SetMsg(match.Groups[1].ToString(), UDF.COLOR.WORK);
            }
            catch (Exception e)
            {
                SetMsg(e.Message, UDF.COLOR.FAIL);
                return false;
            }

            //3. 讀取SN
            SetMsg("讀取SN, Đọc SN", UDF.COLOR.WORK);
            cmd = "adb shell dumpsys activity com.utsmta.app/com.nes.factorytest.ui.activity.Main2Activity getSerialNumber";
            string pcbasn = string.Empty;
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
                Match match = Regex.Match(rst, @"\S+SUCCESS_(\S+)");
                SetMsg(match.Groups[1].ToString(), UDF.COLOR.WORK);
                pcbasn = match.Groups[1].ToString();
            }
            catch (Exception e)
            {
                SetMsg(e.Message, UDF.COLOR.FAIL);
                return false;
            }

            //4. 開啟老化測試 
            SetMsg("開啟老化測試, Bắt đầu kiểm tra lão hóa", UDF.COLOR.WORK);
            cmd = "adb shell input keyevent 136";
            try
            {
                if (!run_cmd_and_check_link(rst, retry, cmd)) return false;
            }
            catch (Exception e)
            {
                SetMsg(e.Message, UDF.COLOR.FAIL);
                return false;
            }

            //5. 上傳資料Label_sn, pcba_sn, eth_mac , fw , test date 到Oracle , int_telly_info
            try
            {
                SetMsg("上傳資料到Oracle INT_Telly_INFO, Tải dữ liệu lên Oracle INT_Telly_INFO", UDF.COLOR.WORK);

                string insertSql = $"INSERT INTO INS_Telly_INFO (LABEL_SN, PCBA_SN, Ethernet_MAC, FW, INS_DATE) VALUES (:sn, :pcbasn, :Ethmac, :FW, :insDate)";
                OracleCommand insertCommand = new OracleCommand(insertSql, oraConn);
                insertCommand.Parameters.Add(new OracleParameter("sn", textBoxSn.Text));
                insertCommand.Parameters.Add(new OracleParameter("pcbasn", pcbasn));
                insertCommand.Parameters.Add(new OracleParameter("Ethmac", textBoxDC.Text));
                insertCommand.Parameters.Add(new OracleParameter("FW", textBoxFW.Text));
                insertCommand.Parameters.Add(new OracleParameter("insDate", DateTime.Now.ToString("yyyy/MM/dd_hh:mm:ss")));

                int rowsAffected = insertCommand.ExecuteNonQuery();
                SetMsg($"新增到 into INS_Telly_INFO資料庫成功, Đã thêm thành công vào cơ sở dữ liệu INS_Telly_INFO", UDF.COLOR.WORK);
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

