using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PRETEST.UDFs;
using Oracle.ManagedDataAccess.Client;
using System.Text.RegularExpressions;
using System.IO;
using PRETEST.AppConfig;
using PRETEST.Database;
using PRETEST.SDriver;

namespace PRETEST
{
    public partial class FormMain
    {
        bool PRE_Arran()
        {
            string rst = string.Empty;
            string cmd = string.Empty;
            string regString = string.Empty;
            string TestName = string.Empty;
            string Filename = string.Empty;
            string compareString = string.Empty;
            int retry = 0;

            // 清空ADB工作, Đầu tiên đóng ADB
            RunAdbCmd("taskkill /f /im adb.exe /t", out rst, 1);

            SetMsg("確認ADB連線, Xác nhận kết nối ADB", UDF.COLOR.WORK);
            cmd = "adb devices";
            regString = @"^List of devices attached\s*$";
            try
            {
                retry:
                while (!RunAdbCmd(cmd, out rst))
                {
                    retry++;
                    if (retry > 30)
                    {
                        SetMsg("找不到機器, FAIL, Không tìm thấy máy", UDF.COLOR.FAIL);
                        return false;
                    }
                    SetMsg($"重新下指令:{retry}, Ra lệnh lại:{retry}", UDF.COLOR.WORK);
                    Delay(1000);
                }
                SetMsg(rst, UDF.COLOR.WORK);
                while (Regex.IsMatch(rst, regString))
                {
                    retry++;
                    if (retry > 30)
                    {
                        SetMsg("找不到機器, FAIL, Không tìm thấy máy", UDF.COLOR.FAIL);
                        return false;
                    }
                    SetMsg("連線失敗,重新一次, FAIL, Kết nối thất bại, thử lại lần nữa.", UDF.COLOR.FAIL);
                    Delay(100);
                    goto retry;
                }
                SetMsg("連線成功, PASS, Kết nối thành công.", UDF.COLOR.WORK);
            }
            catch (Exception e)
            {
                SetMsg(e.Message, UDF.COLOR.FAIL);
                return false;
            }

            SetMsg("設定工廠模式啟動,Kích hoạt chế độ nhà máy", UDF.COLOR.WORK);//@@@###
            cmd = "adb shell am startservice -n com.amtran.factory/.FactoryService --es Action SetFactoryMode --ez setMode 1";
            regString = @"Starting service: Intent \{ cmp=com.amtran.factory/.FactoryService \(has extras\) \}";
            try
            {
                while (!RunAdbCmd(cmd, out rst))
                {
                    retry++;
                    if (retry > 3)
                    {
                        SetMsg("找不到機器, FAIL, Không tìm thấy máy", UDF.COLOR.FAIL);
                        return false;
                    }
                    SetMsg($"重新下指令:{retry}, Ra lệnh lại:{retry}", UDF.COLOR.WORK);
                    Delay(1000);
                }
                if (Regex.IsMatch(rst, regString))
                {
                    SetMsg("指令成功, PASS, Lệnh chế độ nhà máy thành công", UDF.COLOR.WORK);
                    retry = 0;
                }
                else
                {
                    SetMsg("指令失敗, FAIL, Lệnh thất bại", UDF.COLOR.FAIL);
                    return false;
                }
            }
            catch (Exception e)
            {
                SetMsg(e.Message, UDF.COLOR.FAIL);
                return false;
            }

            SetMsg("取得工廠模式指令結果,Lấy kết quả lệnh chế độ nhà máy", UDF.COLOR.WORK);
            cmd = "adb shell am startservice -n com.amtran.factory/.FactoryService --es Action GetFactoryMode";
            regString = @"Starting service: Intent \{ cmp=com.amtran.factory/.FactoryService \(has extras\) \}";
            try
            {
                while (!RunAdbCmd(cmd, out rst))
                {
                    retry++;
                    if (retry > 3)
                    {
                        SetMsg("找不到機器, FAIL, Không tìm thấy máy", UDF.COLOR.FAIL);
                        return false;
                    }
                    SetMsg($"重新下指令:{retry}, Ra lệnh lại:{retry}", UDF.COLOR.WORK);
                    Delay(1000);
                }
                if (Regex.IsMatch(rst, regString))
                {
                    SetMsg("指令成功, PASS, Lệnh chế độ nhà máy thành công", UDF.COLOR.WORK);
                    retry = 0;
                }
                else
                {
                    SetMsg("指令失敗, FAIL, Lệnh thất bại", UDF.COLOR.FAIL);
                    return false;
                }
            }
            catch (Exception e)
            {
                SetMsg(e.Message, UDF.COLOR.FAIL);
                return false;
            }

            SetMsg("檢查工廠模式是否啟動,Kiểm tra xem chế độ nhà máy đã được kích hoạt hay chưa", UDF.COLOR.WORK);
            TestName = "Factory_Mode_1";
            Filename = "data_command_ack.txt";
            cmd = $"adb pull sdcard/Download/{Filename}";
            compareString = "1";
            try
            {
                if (File.Exists(Filename)) File.Delete(Filename);
                if (!run_cmd_and_check_link(rst, retry, cmd)) return false;
                if (!check_command_ack(TestName, Filename, compareString)) return false;
            }
            catch (Exception e)
            {
                SetMsg(e.Message, UDF.COLOR.FAIL);
                return false;
            }

            SetMsg("取得軟體版本號碼,Lấy số phiên bản phần mềm", UDF.COLOR.WORK);
            cmd = "adb shell am startservice -n com.amtran.factory/.FactoryService --es Action GetFWVersion";
            regString = @"Starting service: Intent \{ cmp=com.amtran.factory/.FactoryService \(has extras\) \}";
            try
            {
                while (!RunAdbCmd(cmd, out rst))
                {
                    retry++;
                    if (retry > 3)
                    {
                        SetMsg("找不到機器, FAIL, Không tìm thấy máy", UDF.COLOR.FAIL);
                        return false;
                    }
                    SetMsg($"重新下指令:{retry}, Ra lệnh lại:{retry}", UDF.COLOR.WORK);
                    Delay(1000);
                }
                if (Regex.IsMatch(rst, regString))
                {
                    SetMsg("指令成功, PASS, Lệnh chế độ nhà máy thành công", UDF.COLOR.WORK);
                }
                else
                {
                    SetMsg("指令失敗, FAIL, Lệnh thất bại", UDF.COLOR.FAIL);
                    return false;
                }
            }
            catch (Exception e)
            {
                SetMsg(e.Message, UDF.COLOR.FAIL);
                return false;
            }

            SetMsg("檢查軟體版本號碼,Kiểm tra số phiên bản phần mềm", UDF.COLOR.WORK);
            TestName = "Check_FW";
            Filename = "data_command_ack.txt";
            cmd = $"adb pull sdcard/Download/{Filename}";
            compareString = "NFA1.20240912.0826";
            //compareString = "NFA1.20250402.0225";
            try
            {
                if (File.Exists(Filename)) File.Delete(Filename);
                if (!run_cmd_and_check_link(rst, retry, cmd)) return false;
                if (!check_command_ack(TestName, Filename, compareString)) return false;
                if (textBoxFW.InvokeRequired)
                {
                    textBoxFW.Invoke(new Action(() =>
                    {
                        textBoxFW.Text = compareString; // 更新FW文本框
                    }));
                }
                else textBoxFW.Text = compareString;

            }
            catch (Exception e)
            {
                SetMsg(e.Message, UDF.COLOR.FAIL);
                return false;
            }

            SetMsg("設定多媒體音量 10,Đặt âm lượng đa phương tiện thành 10", UDF.COLOR.WORK);
            cmd = "adb shell am startservice -n com.amtran.factory/.FactoryService --es Action SetMediaVolume --ei mediaVolume 10";
            regString = @"Starting service: Intent \{ cmp=com.amtran.factory/.FactoryService \(has extras\) \}";
            try
            {
                while (!RunAdbCmd(cmd, out rst))
                {
                    retry++;
                    if (retry > 3)
                    {
                        SetMsg("找不到機器, FAIL, Không tìm thấy máy", UDF.COLOR.FAIL);
                        return false;
                    }
                    SetMsg($"重新下指令:{retry}, Ra lệnh lại:{retry}", UDF.COLOR.WORK);
                    Delay(1000);
                }
                if (Regex.IsMatch(rst, regString))
                {
                    SetMsg("指令成功, PASS, Lệnh chế độ nhà máy thành công", UDF.COLOR.WORK);
                    retry = 0;
                }
                else
                {
                    SetMsg("指令失敗, FAIL, Lệnh thất bại", UDF.COLOR.FAIL);
                    return false;
                }
            }
            catch (Exception e)
            {
                SetMsg(e.Message, UDF.COLOR.FAIL);
                return false;
            }

            SetMsg("檢查多媒體音量 10,Kiểm tra âm lượng đa phương tiện mức 10", UDF.COLOR.WORK);
            TestName = "Check_media_volume 10";
            Filename = "command_ack.txt";
            cmd = $"adb pull sdcard/Download/{Filename}";
            compareString = "10";
            try
            {
                if (File.Exists(Filename)) File.Delete(Filename);
                if (!run_cmd_and_check_link(rst, retry, cmd)) return false;
                if (!check_command_ack(TestName, Filename, compareString)) return false;
            }
            catch (Exception e)
            {
                SetMsg(e.Message, UDF.COLOR.FAIL);
                return false;
            }

            SetMsg("設定並檢查 Voc_Co2_Self Test,Thiết lập và kiểm tra Voc_Co2_Self Test", UDF.COLOR.WORK);
            cmd = "adb shell nf_sensors";
            regString = "voc_index:\\s+(\\S+)";
            try
            {
                while (!RunAdbCmd(cmd, out rst))
                {
                    retry++;
                    if (retry > 3)
                    {
                        SetMsg("找不到機器, FAIL, Không tìm thấy máy", UDF.COLOR.FAIL);
                        return false;
                    }
                    SetMsg($"重新下指令:{retry}, Ra lệnh lại:{retry}", UDF.COLOR.WORK);
                    Delay(1000);
                }
                Match match = Regex.Match(rst.Replace("\r\n", " ").Replace("\n", " "), regString);
                if (match.Success)
                {
                    if (double.TryParse(match.Groups[1].Value, out double value) && value > 0)
                    {
                        SetMsg($"Voc_Co2_Self Test 設定成功, PASS, Voc_Co2_SelfTest thiết lập thành công: {match.Groups[1].Value}", UDF.COLOR.WORK);
                        retry = 0;
                    }
                    else
                    {
                        SetMsg("Voc_Co2_Self Test 設定失敗, FAIL, Voc_Co2_SelfTest thiết lập thất bại", UDF.COLOR.FAIL);
                        return false;
                    }
                }
                else
                {
                    SetMsg("Voc_Co2_Self Test 設定失敗, FAIL, Voc_Co2_SelfTest thiết lập thất bại", UDF.COLOR.FAIL);
                    return false;
                }
            }
            catch (Exception e)
            {
                SetMsg(e.Message, UDF.COLOR.FAIL);
                return false;
            }

            SetMsg("設定序號,Thiết lập số sê-ri", UDF.COLOR.WORK);
            cmd = $"adb shell am startservice -n com.amtran.factory/.FactoryService --es Action SetSN --es serialNumber {textBoxSn.Text}";
            regString = @"Starting service: Intent \{ cmp=com.amtran.factory/.FactoryService \(has extras\) \}";
            try
            {
                while (!RunAdbCmd(cmd, out rst))
                {
                    retry++;
                    if (retry > 3)
                    {
                        SetMsg("找不到機器, FAIL, Không tìm thấy máy", UDF.COLOR.FAIL);
                        return false;
                    }
                    SetMsg($"重新下指令:{retry}, Ra lệnh lại:{retry}", UDF.COLOR.WORK);
                    Delay(1000);

                }
                if (Regex.IsMatch(rst, regString))
                {
                    SetMsg("指令成功, PASS, Lệnh chế độ nhà máy thành công", UDF.COLOR.WORK);
                    retry = 0;
                }
                else
                {
                    SetMsg("指令失敗, FAIL, Lệnh thất bại", UDF.COLOR.FAIL);
                    return false;
                }
            }
            catch (Exception e)
            {
                SetMsg(e.Message, UDF.COLOR.FAIL);
                return false;
            }

            SetMsg("檢查序號,Kiểm tra số sê-ri", UDF.COLOR.WORK);
            cmd = "adb shell am startservice -n com.amtran.factory/.FactoryService --es Action GetSN";
            regString = @"Starting service: Intent \{ cmp=com.amtran.factory/.FactoryService \(has extras\) \}";
            try
            {
                while (!RunAdbCmd(cmd, out rst))
                {
                    retry++;
                    if (retry > 3)
                    {
                        SetMsg("找不到機器, FAIL, Không tìm thấy máy", UDF.COLOR.FAIL);
                        return false;
                    }
                    SetMsg($"重新下指令:{retry}, Ra lệnh lại:{retry}", UDF.COLOR.WORK);
                    Delay(1000);
                }
                if (Regex.IsMatch(rst, regString))
                {
                    SetMsg("指令成功, PASS, Lệnh chế độ nhà máy thành công", UDF.COLOR.WORK);
                    retry = 0;
                }
                else
                {
                    SetMsg("指令失敗, FAIL, Lệnh thất bại", UDF.COLOR.FAIL);
                    return false;
                }
            }
            catch (Exception e)
            {
                SetMsg(e.Message, UDF.COLOR.FAIL);
                return false;
            }

            SetMsg($"檢查序號是否等於 {textBoxSn.Text}, Kiểm tra xem số sê-ri có trùng khớp không {textBoxSn.Text}", UDF.COLOR.WORK);
            TestName = "Check_SN";
            Filename = "data_command_ack.txt";
            cmd = $"adb pull sdcard/Download/{Filename}";
            compareString = textBoxSn.Text;
            try
            {
                if (File.Exists(Filename)) File.Delete(Filename);
                if (!run_cmd_and_check_link(rst, retry, cmd)) return false;
                if (!check_command_ack(TestName, Filename, compareString)) return false;
            }
            catch (Exception e)
            {
                SetMsg(e.Message, UDF.COLOR.FAIL);
                return false;
            }

            SetMsg("設定 MIC Vendor, Thiết lập nhà cung cấp MIC", UDF.COLOR.WORK);
            cmd = "adb shell \"su root sh -c 'echo merry > /proc/vendor/nf/inst_mic'\"";
            regString = "";
            try
            {
                while (!RunAdbCmd(cmd, out rst))
                {
                    retry++;
                    if (retry > 3)
                    {
                        SetMsg("找不到機器, FAIL, Không tìm thấy máy", UDF.COLOR.FAIL);
                        return false;
                    }
                    SetMsg($"重新下指令:{retry}, Ra lệnh lại:{retry}", UDF.COLOR.WORK);
                    Delay(1000);
                }
                if (Regex.IsMatch(rst, regString))
                {
                    SetMsg("指令成功, PASS, Lệnh chế độ nhà máy thành công", UDF.COLOR.WORK);
                    retry = 0;
                }
                else
                {
                    SetMsg("指令失敗, FAIL, Lệnh thất bại", UDF.COLOR.FAIL);
                    return false;
                }
            }
            catch (Exception e)
            {
                SetMsg(e.Message, UDF.COLOR.FAIL);
                return false;
            }

            SetMsg("檢查 MIC Vendor 是否為 merry, Kiểm tra nhà cung cấp MIC có phải là merry không", UDF.COLOR.WORK);
            TestName = "Check_MIC_Vendor_merry";
            Filename = "data_command_ack.txt";
            cmd = $"adb shell su root \"cat /proc/vendor/nf/inst_mic\" > {Filename}";
            compareString = "merry";
            try
            {
                if (File.Exists(Filename)) File.Delete(Filename);
                if (!run_cmd_and_check_link(rst, retry, cmd)) return false;
                if (!check_command_ack(TestName, Filename, compareString)) return false;
            }
            catch (Exception e)
            {
                SetMsg(e.Message, UDF.COLOR.FAIL);
                return false;
            }

            SetMsg("取得 Build date , Lấy ngày biên dịch", UDF.COLOR.WORK);
            cmd = "adb shell getprop ro.build.date.utc";
            regString = @"\d+";
            string buildDate = string.Empty;
            try
            {
                while (!RunAdbCmd(cmd, out rst))
                {
                    retry++;
                    if (retry > 3)
                    {
                        SetMsg("找不到機器, FAIL, Không tìm thấy máy", UDF.COLOR.FAIL);
                        return false;
                    }
                    SetMsg($"重新下指令:{retry}, Ra lệnh lại:{retry}", UDF.COLOR.WORK);
                    Delay(1000);
                }
                if (Regex.IsMatch(rst, regString))
                {
                    SetMsg("指令成功, PASS, Lệnh chế độ nhà máy thành công", UDF.COLOR.WORK);
                    buildDate = rst.Trim();
                    SetMsg($"Build Date: {buildDate}", UDF.COLOR.WORK);
                    retry = 0;
                }
                else
                {
                    SetMsg("指令失敗, FAIL, Lệnh thất bại", UDF.COLOR.FAIL);
                    return false;
                }
            }
            catch (Exception e)
            {
                SetMsg(e.Message, UDF.COLOR.FAIL);
                return false;
            }

            SetMsg("設定 Build date, Đặt ngày biên dịch", UDF.COLOR.WORK);
            cmd = $"adb shell \"su root sh -c 'echo {buildDate} > /proc/vendor/nf/fswtstamp'\"";
            regString = "";
            try
            {
                while (!RunAdbCmd(cmd, out rst))
                {
                    retry++;
                    if (retry > 3)
                    {
                        SetMsg("找不到機器, FAIL, Không tìm thấy máy", UDF.COLOR.FAIL);
                        return false;
                    }
                    SetMsg($"重新下指令:{retry}, Ra lệnh lại:{retry}", UDF.COLOR.WORK);
                    Delay(1000);
                }
                if (Regex.IsMatch(rst, regString))
                {
                    SetMsg("指令成功, PASS, Lệnh chế độ nhà máy thành công", UDF.COLOR.WORK);
                    retry = 0;
                }
                else
                {
                    SetMsg("指令失敗, FAIL, Lệnh thất bại", UDF.COLOR.FAIL);
                    return false;
                }
            }
            catch (Exception e)
            {
                SetMsg(e.Message, UDF.COLOR.FAIL);
                return false;
            }

            SetMsg("檢查 Build date 是否正確, Kiểm tra ngày biên dịch có chính xác không", UDF.COLOR.WORK);
            cmd = "adb shell su root cat /proc/vendor/nf/fswtstamp";
            regString = $"{buildDate}";
            try
            {
                while (!RunAdbCmd(cmd, out rst))
                {
                    retry++;
                    if (retry > 3)
                    {
                        SetMsg("找不到機器, FAIL, Không tìm thấy máy", UDF.COLOR.FAIL);
                        return false;
                    }
                    SetMsg($"重新下指令:{retry}, Ra lệnh lại:{retry}", UDF.COLOR.WORK);
                    Delay(1000);
                }
                if (Regex.IsMatch(rst, regString))
                {
                    SetMsg("指令成功, PASS, Lệnh chế độ nhà máy thành công", UDF.COLOR.WORK);
                    retry = 0;
                }
                else
                {
                    SetMsg("指令失敗, FAIL, Lệnh thất bại", UDF.COLOR.FAIL);
                    return false;
                }
            }
            catch (Exception e)
            {
                SetMsg(e.Message, UDF.COLOR.FAIL);
                return false;
            }

            SetMsg("清除 sdcard/Download 內的文件, Xóa các tệp trong sdcard/Download", UDF.COLOR.WORK);
            cmd = "adb shell rm sdcard/Download/*";
            regString = @"";
            try
            {
                while (!RunAdbCmd(cmd, out rst))
                {
                    retry++;
                    if (retry > 3)
                    {
                        SetMsg("找不到機器, FAIL, Không tìm thấy máy", UDF.COLOR.FAIL);
                        return false;
                    }
                    SetMsg($"重新下指令:{retry}, Ra lệnh lại:{retry}", UDF.COLOR.WORK);
                    Delay(1000);
                }
                if (Regex.IsMatch(rst, regString))
                {
                    SetMsg("指令成功, PASS, Lệnh chế độ nhà máy thành công", UDF.COLOR.WORK);
                    retry = 0;
                }
                else
                {
                    SetMsg("指令失敗, FAIL, Lệnh thất bại", UDF.COLOR.FAIL);
                    return false;
                }
            }
            catch (Exception e)
            {
                SetMsg(e.Message, UDF.COLOR.FAIL);
                return false;
            }

            SetMsg("取得藍芽MAC, Lấy địa chỉ MAC Bluetooth", UDF.COLOR.WORK);
            string btmac = FormatMac(GetMac(textBoxSn.Text, 0, "INSP_BARRAKEY_T").ToLower());
            //string btmac = "c4:63:fb:1b:de:ff";
            SetMsg($"BT mac: {btmac}", UDF.COLOR.WORK);

            SetMsg("設定藍芽MAC, Đặt địa chỉ MAC Bluetooth", UDF.COLOR.WORK);
            cmd = $"adb shell su 0 am startservice -n com.amtran.factory/.FactoryService --es Action SetBTMAC --es btMac {btmac}";
            regString = @"Starting service: Intent \{ cmp=com.amtran.factory/.FactoryService \(has extras\) \}";
            try
            {
                while (!RunAdbCmd(cmd, out rst))
                {
                    retry++;
                    if (retry > 3)
                    {
                        SetMsg("找不到機器, FAIL, Không tìm thấy máy", UDF.COLOR.FAIL);
                        return false;
                    }
                    SetMsg($"重新下指令:{retry}, Ra lệnh lại:{retry}", UDF.COLOR.WORK);
                    Delay(1000);
                }
                if (Regex.IsMatch(rst, regString))
                {
                    SetMsg("指令成功, PASS, Lệnh chế độ nhà máy thành công", UDF.COLOR.WORK);
                    retry = 0;
                }
                else
                {
                    SetMsg("指令失敗, FAIL, Lệnh thất bại", UDF.COLOR.FAIL);
                    return false;
                }
            }
            catch (Exception e)
            {
                SetMsg(e.Message, UDF.COLOR.FAIL);
                return false;
            }

            SetMsg("取得藍芽MAC檔案, Lấy tệp địa chỉ MAC Bluetooth", UDF.COLOR.WORK);
            cmd = "adb shell su 0 am startservice -n com.amtran.factory/.FactoryService --es Action GetBTMAC";
            regString = @"Starting service: Intent \{ cmp=com.amtran.factory/.FactoryService \(has extras\) \}";
            try
            {
                while (!RunAdbCmd(cmd, out rst))
                {
                    retry++;
                    if (retry > 3)
                    {
                        SetMsg("找不到機器, FAIL, Không tìm thấy máy", UDF.COLOR.FAIL);
                        return false;
                    }
                    SetMsg($"重新下指令:{retry}, Ra lệnh lại:{retry}", UDF.COLOR.WORK);
                    Delay(1000);
                }
                if (Regex.IsMatch(rst, regString))
                {
                    SetMsg("指令成功, PASS, Lệnh chế độ nhà máy thành công", UDF.COLOR.WORK);
                    retry = 0;
                }
                else
                {
                    SetMsg("指令失敗, FAIL, Lệnh thất bại", UDF.COLOR.FAIL);
                    return false;
                }
            }
            catch (Exception e)
            {
                SetMsg(e.Message, UDF.COLOR.FAIL);
                return false;
            }

            SetMsg("檢查藍芽MAC是否寫入成功, Kiểm tra xem địa chỉ MAC Bluetooth đã được ghi thành công hay chưa", UDF.COLOR.WORK);
            TestName = "Check_BT_MAC";
            Filename = "data_command_ack.txt";
            cmd = $"adb pull sdcard/Download/{Filename}";
            compareString = btmac;
            try
            {
                if (File.Exists(Filename)) File.Delete(Filename);
                if (!run_cmd_and_check_link(rst, retry, cmd)) return false;
                if (!check_command_ack(TestName, Filename, compareString)) return false;
                if (GlobalConfig.iRunMode == 1)
                {
                    if (!set_MAC_use_mode(textBoxSn.Text, UnformatMac(compareString), "1", "INSP_BARRAKEY_T")) return false; // 設定MAC使用模式為1, Đặt chế độ sử dụng MAC là 1
                }
                else
                {
                    SetMsg("非工廠模式, 不設定MAC使用模式, Không đặt chế độ sử dụng MAC Ethernet", UDF.COLOR.WORK);
                }
            }
            catch (Exception e)
            {
                SetMsg(e.Message, UDF.COLOR.FAIL);
                return false;
            }

            SetMsg("清除 sdcard/Download 內的文件, Xóa các tệp trong sdcard/Download", UDF.COLOR.WORK);
            cmd = "adb shell rm sdcard/Download/*";
            regString = @"";
            try
            {
                while (!RunAdbCmd(cmd, out rst))
                {
                    retry++;
                    if (retry > 3)
                    {
                        SetMsg("找不到機器, FAIL, Không tìm thấy máy", UDF.COLOR.FAIL);
                        return false;
                    }
                    SetMsg($"重新下指令:{retry}, Ra lệnh lại:{retry}", UDF.COLOR.WORK);
                    Delay(1000);
                }
                if (Regex.IsMatch(rst, regString))
                {
                    SetMsg("指令成功, PASS, Lệnh chế độ nhà máy thành công", UDF.COLOR.WORK);
                    retry = 0;
                }
                else
                {
                    SetMsg("指令失敗, FAIL, Lệnh thất bại", UDF.COLOR.FAIL);
                    return false;
                }
            }
            catch (Exception e)
            {
                SetMsg(e.Message, UDF.COLOR.FAIL);
                return false;
            }
            //@@@
            SetMsg("取得WiFiMAC, Lấy địa chỉ MAC WiFi", UDF.COLOR.WORK);
            string wifimac = FormatMac(GetMac(textBoxSn.Text, 2, "INSP_BARRAKEY_T").ToLower());
            //string wifimac = "c4:63:fb:1c:63:d0";
            SetMsg($"Wifi mac: {wifimac}", UDF.COLOR.WORK);

            SetMsg("設定WiFiMAC, Đặt địa chỉ MAC WiFi", UDF.COLOR.WORK);
            cmd = $"adb shell su 0 am startservice -n com.amtran.factory/.FactoryService --es Action SetWiFiMAC --es wifiMac {wifimac}";
            regString = @"Starting service: Intent \{ cmp=com.amtran.factory/.FactoryService \(has extras\) \}";
            try
            {
                while (!RunAdbCmd(cmd, out rst))
                {
                    retry++;
                    if (retry > 3)
                    {
                        SetMsg("找不到機器, FAIL, Không tìm thấy máy", UDF.COLOR.FAIL);
                        return false;
                    }
                    SetMsg($"重新下指令:{retry}, Ra lệnh lại:{retry}", UDF.COLOR.WORK);
                    Delay(1000);
                }
                if (Regex.IsMatch(rst, regString))
                {
                    SetMsg("指令成功, PASS, Lệnh chế độ nhà máy thành công", UDF.COLOR.WORK);
                    retry = 0;
                }
                else
                {
                    SetMsg("指令失敗, FAIL, Lệnh thất bại", UDF.COLOR.FAIL);
                    return false;
                }
            }
            catch (Exception e)
            {
                SetMsg(e.Message, UDF.COLOR.FAIL);
                return false;
            }

            SetMsg("取得WiFiMAC檔案, Lấy tệp địa chỉ MAC WiFi", UDF.COLOR.WORK);
            cmd = "adb shell su 0 am startservice -n com.amtran.factory/.FactoryService --es Action GetWiFiMAC";
            regString = @"Starting service: Intent \{ cmp=com.amtran.factory/.FactoryService \(has extras\) \}";
            try
            {
                while (!RunAdbCmd(cmd, out rst))
                {
                    retry++;
                    if (retry > 3)
                    {
                        SetMsg("找不到機器, FAIL, Không tìm thấy máy", UDF.COLOR.FAIL);
                        return false;
                    }
                    SetMsg($"重新下指令:{retry}, Ra lệnh lại:{retry}", UDF.COLOR.WORK);
                    Delay(1000);
                }
                if (Regex.IsMatch(rst, regString))
                {
                    SetMsg("指令成功, PASS, Lệnh chế độ nhà máy thành công", UDF.COLOR.WORK);
                    retry = 0;
                }
                else
                {
                    SetMsg("指令失敗, FAIL, Lệnh thất bại", UDF.COLOR.FAIL);
                    return false;
                }
            }
            catch (Exception e)
            {
                SetMsg(e.Message, UDF.COLOR.FAIL);
                return false;
            }

            SetMsg("檢查WiFiMAC是否寫入成功, Kiểm tra xem địa chỉ MAC WiFi đã được ghi thành công hay chưa", UDF.COLOR.WORK);
            TestName = "Check_WIFI_MAC";
            Filename = "data_command_ack.txt";
            cmd = $"adb pull sdcard/Download/{Filename}";
            compareString = wifimac;
            try
            {
                if (File.Exists(Filename)) File.Delete(Filename);
                if (!run_cmd_and_check_link(rst, retry, cmd)) return false;
                if (!check_command_ack(TestName, Filename, compareString)) return false;
                if (GlobalConfig.iRunMode == 1)
                {
                    if (!set_MAC_use_mode(textBoxSn.Text, UnformatMac(compareString), "1", "INSP_BARRAKEY_T")) return false; // 設定MAC使用模式為1, Đặt chế độ sử dụng MAC là 1
                }
                else
                {
                    SetMsg("非工廠模式, 不設定MAC使用模式, Không đặt chế độ sử dụng MAC Ethernet", UDF.COLOR.WORK);
                }
            }
            catch (Exception e)
            {
                SetMsg(e.Message, UDF.COLOR.FAIL);
                return false;
            }

            SetMsg("清除 sdcard/Download 內的文件, Xóa các tệp trong sdcard/Download", UDF.COLOR.WORK);
            cmd = "adb shell rm sdcard/Download/*";
            regString = @"";
            try
            {
                while (!RunAdbCmd(cmd, out rst))
                {
                    retry++;
                    if (retry > 3)
                    {
                        SetMsg("找不到機器, FAIL, Không tìm thấy máy", UDF.COLOR.FAIL);
                        return false;
                    }
                    SetMsg($"重新下指令:{retry}, Ra lệnh lại:{retry}", UDF.COLOR.WORK);
                    Delay(1000);
                }
                if (Regex.IsMatch(rst, regString))
                {
                    SetMsg("指令成功, PASS, Lệnh chế độ nhà máy thành công", UDF.COLOR.WORK);
                    retry = 0;
                }
                else
                {
                    SetMsg("指令失敗, FAIL, Lệnh thất bại", UDF.COLOR.FAIL);
                    return false;
                }
            }
            catch (Exception e)
            {
                SetMsg(e.Message, UDF.COLOR.FAIL);
                return false;
            }

            SetMsg("取得乙太網MAC, Lấy địa chỉ MAC Ethernet", UDF.COLOR.WORK);
            string ethmac = FormatMac(GetMac(textBoxSn.Text, 1, "INSP_BARRAKEY_T").ToLower());
            //string ethmac = "c4:63:fb:1c:4a:da";
            SetMsg($"Ethernet mac: {ethmac}", UDF.COLOR.WORK);

            SetMsg("設定乙太網MAC, Đặt địa chỉ MAC Ethernet", UDF.COLOR.WORK);
            cmd = $"adb shell su 0 am startservice -n com.amtran.factory/.FactoryService --es Action SetEthMAC --es ethMac {ethmac}";
            regString = @"Starting service: Intent \{ cmp=com.amtran.factory/.FactoryService \(has extras\) \}";
            try
            {
                while (!RunAdbCmd(cmd, out rst))
                {
                    retry++;
                    if (retry > 3)
                    {
                        SetMsg("找不到機器, FAIL, Không tìm thấy máy", UDF.COLOR.FAIL);
                        return false;
                    }
                    SetMsg($"重新下指令:{retry}, Ra lệnh lại:{retry}", UDF.COLOR.WORK);
                    Delay(1000);
                }
                if (Regex.IsMatch(rst, regString))
                {
                    SetMsg("指令成功, PASS, Lệnh chế độ nhà máy thành công", UDF.COLOR.WORK);
                    retry = 0;
                }
                else
                {
                    SetMsg("指令失敗, FAIL, Lệnh thất bại", UDF.COLOR.FAIL);
                    return false;
                }
            }
            catch (Exception e)
            {
                SetMsg(e.Message, UDF.COLOR.FAIL);
                return false;
            }

            SetMsg("取得乙太網MAC檔案, Lấy tệp địa chỉ MAC Ethernet", UDF.COLOR.WORK);
            cmd = "adb shell su 0 am startservice -n com.amtran.factory/.FactoryService --es Action GetEthMAC";
            regString = @"Starting service: Intent \{ cmp=com.amtran.factory/.FactoryService \(has extras\) \}";
            try
            {
                while (!RunAdbCmd(cmd, out rst))
                {
                    retry++;
                    if (retry > 3)
                    {
                        SetMsg("找不到機器, FAIL, Không tìm thấy máy", UDF.COLOR.FAIL);
                        return false;
                    }
                    SetMsg($"重新下指令:{retry}, Ra lệnh lại:{retry}", UDF.COLOR.WORK);
                    Delay(1000);
                }
                if (Regex.IsMatch(rst, regString))
                {
                    SetMsg("指令成功, PASS, Lệnh chế độ nhà máy thành công", UDF.COLOR.WORK);
                    retry = 0;
                }
                else
                {
                    SetMsg("指令失敗, FAIL, Lệnh thất bại", UDF.COLOR.FAIL);
                    return false;
                }
            }
            catch (Exception e)
            {
                SetMsg(e.Message, UDF.COLOR.FAIL);
                return false;
            }

            SetMsg("檢查乙太網MAC, Kiểm tra địa chỉ MAC Ethernet", UDF.COLOR.WORK);
            TestName = "Check_Ethernet_MAC";
            Filename = "data_command_ack.txt";
            cmd = $"adb pull sdcard/Download/{Filename}";
            compareString = ethmac;
            try
            {
                if (File.Exists(Filename)) File.Delete(Filename);
                if (!run_cmd_and_check_link(rst, retry, cmd)) return false;
                if (!check_command_ack(TestName, Filename, compareString)) return false;
                if (GlobalConfig.iRunMode == 1)
                {
                    if (!set_MAC_use_mode(textBoxSn.Text, UnformatMac(compareString), "1", "INSP_BARRAKEY_T")) return false; // 設定MAC使用模式為1, Đặt chế độ sử dụng MAC là 1
                }
                else
                {
                    SetMsg("非工廠模式, 不設定MAC使用模式, Không đặt chế độ sử dụng MAC Ethernet", UDF.COLOR.WORK);
                }
            }
            catch (Exception e)
            {
                SetMsg(e.Message, UDF.COLOR.FAIL);
                return false;
            }

            SetMsg("設定國家代碼 US, Đặt mã quốc gia US", UDF.COLOR.WORK);
            cmd = "adb shell am startservice -n com.amtran.factory/.FactoryService --es Action SetWiFiCountry --es countryCode US";
            regString = @"Starting service: Intent \{ cmp=com.amtran.factory/.FactoryService \(has extras\) \}";
            try
            {
                while (!RunAdbCmd(cmd, out rst))
                {
                    retry++;
                    if (retry > 3)
                    {
                        SetMsg("找不到機器, FAIL, Không tìm thấy máy", UDF.COLOR.FAIL);
                        return false;
                    }
                    SetMsg($"重新下指令:{retry}, Ra lệnh lại:{retry}", UDF.COLOR.WORK);
                    Delay(1000);
                }
                if (Regex.IsMatch(rst, regString))
                {
                    SetMsg("指令成功, PASS, Lệnh chế độ nhà máy thành công", UDF.COLOR.WORK);
                    retry = 0;
                }
                else
                {
                    SetMsg("指令失敗, FAIL, Lệnh thất bại", UDF.COLOR.FAIL);
                    return false;
                }
            }
            catch (Exception e)
            {
                SetMsg(e.Message, UDF.COLOR.FAIL);
                return false;
            }

            SetMsg("取得國家代碼, Lấy mã quốc gia", UDF.COLOR.WORK);
            cmd = "adb shell am startservice -n com.amtran.factory/.FactoryService --es Action GetWiFiCountry";
            regString = @"Starting service: Intent \{ cmp=com.amtran.factory/.FactoryService \(has extras\) \}";
            try
            {
                while (!RunAdbCmd(cmd, out rst))
                {
                    retry++;
                    if (retry > 3)
                    {
                        SetMsg("找不到機器, FAIL, Không tìm thấy máy", UDF.COLOR.FAIL);
                        return false;
                    }
                    SetMsg($"重新下指令:{retry}, Ra lệnh lại:{retry}", UDF.COLOR.WORK);
                    Delay(1000);
                }
                if (Regex.IsMatch(rst, regString))
                {
                    SetMsg("指令成功, PASS, Lệnh chế độ nhà máy thành công", UDF.COLOR.WORK);
                    retry = 0;
                }
                else
                {
                    SetMsg("指令失敗, FAIL, Lệnh thất bại", UDF.COLOR.FAIL);
                    return false;
                }
            }
            catch (Exception e)
            {
                SetMsg(e.Message, UDF.COLOR.FAIL);
                return false;
            }

            SetMsg("檢查國家代碼是否為 US, Kiểm tra mã quốc gia có phải là US không", UDF.COLOR.WORK);
            TestName = "Check_WIFI_Code_US";
            Filename = "data_command_ack.txt";
            cmd = $"adb pull sdcard/Download/{Filename}";
            compareString = "US";
            try
            {
                if (File.Exists(Filename)) File.Delete(Filename);
                if (!run_cmd_and_check_link(rst, retry, cmd)) return false;
                if (!check_command_ack(TestName, Filename, compareString)) return false;
            }
            catch (Exception e)
            {
                SetMsg(e.Message, UDF.COLOR.FAIL);
                return false;
            }

            SetMsg("啟動G sensor self test, Bắt đầu kiểm tra cảm biến G", UDF.COLOR.WORK);
            cmd = "adb shell am startservice -n com.amtran.factory/.FactoryService --es Action GSensorSelfTest";
            regString = @"Starting service: Intent \{ cmp=com.amtran.factory/.FactoryService \(has extras\) \}";
            try
            {
                while (!RunAdbCmd(cmd, out rst))
                {
                    retry++;
                    if (retry > 3)
                    {
                        SetMsg("找不到機器, FAIL, Không tìm thấy máy", UDF.COLOR.FAIL);
                        return false;
                    }
                    SetMsg($"重新下指令:{retry}, Ra lệnh lại:{retry}", UDF.COLOR.WORK);
                    Delay(1000);
                }
                if (Regex.IsMatch(rst, regString))
                {
                    SetMsg("指令成功, PASS, Lệnh chế độ nhà máy thành công", UDF.COLOR.WORK);
                    retry = 0;
                }
                else
                {
                    SetMsg("指令失敗, FAIL, Lệnh thất bại", UDF.COLOR.FAIL);
                    return false;
                }
            }
            catch (Exception e)
            {
                SetMsg(e.Message, UDF.COLOR.FAIL);
                return false;
            }

            SetMsg("檢查G sensor self test結果, Kiểm tra kết quả kiểm tra cảm biến G", UDF.COLOR.WORK);
            TestName = "Check_Gsensor";
            Filename = "command_ack.txt";
            cmd = $"adb pull sdcard/Download/{Filename}";
            compareString = "Ok";
            try
            {
                if (File.Exists(Filename)) File.Delete(Filename);
                if (!run_cmd_and_check_link(rst, retry, cmd)) return false;
                if (!check_command_ack(TestName, Filename, compareString)) return false;
            }
            catch (Exception e)
            {
                SetMsg(e.Message, UDF.COLOR.FAIL);
                return false;
            }

            SetMsg("清除 sdcard/Download 內的文件, Xóa các tệp trong sdcard/Download", UDF.COLOR.WORK);
            cmd = "adb shell rm sdcard/Download/*";
            regString = @"";
            try
            {
                while (!RunAdbCmd(cmd, out rst))
                {
                    retry++;
                    if (retry > 3)
                    {
                        SetMsg("找不到機器, FAIL, Không tìm thấy máy", UDF.COLOR.FAIL);
                        return false;
                    }
                    SetMsg($"重新下指令:{retry}, Ra lệnh lại:{retry}", UDF.COLOR.WORK);
                    Delay(1000);
                }
                if (Regex.IsMatch(rst, regString))
                {
                    SetMsg("指令成功, PASS, Lệnh chế độ nhà máy thành công", UDF.COLOR.WORK);
                    retry = 0;
                }
                else
                {
                    SetMsg("指令失敗, FAIL, Lệnh thất bại", UDF.COLOR.FAIL);
                    return false;
                }
            }
            catch (Exception e)
            {
                SetMsg(e.Message, UDF.COLOR.FAIL);
                return false;
            }

            SetMsg("回到歡迎畫面, Quay lại màn hình chào mừng", UDF.COLOR.WORK);
            cmd = "adb shell pm clear com.neatframe.home";
            regString = "Success";
            try
            {
                while (!RunAdbCmd(cmd, out rst))
                {
                    retry++;
                    if (retry > 3)
                    {
                        SetMsg("找不到機器, FAIL, Không tìm thấy máy", UDF.COLOR.FAIL);
                        return false;
                    }
                    SetMsg($"重新下指令:{retry}, Ra lệnh lại:{retry}", UDF.COLOR.WORK);
                    Delay(1000);
                }
                if (Regex.IsMatch(rst, regString))
                {
                    SetMsg("指令成功, PASS, Lệnh chế độ nhà máy thành công", UDF.COLOR.WORK);
                    retry = 0;
                }
                else
                {
                    SetMsg("指令失敗, FAIL, Lệnh thất bại", UDF.COLOR.FAIL);
                    return false;
                }
            }
            catch (Exception e)
            {
                SetMsg(e.Message, UDF.COLOR.FAIL);
                return false;
            }

            SetMsg("設定燒機模式啟動, Kích hoạt chế độ đốt cháy", UDF.COLOR.WORK);
            cmd = "adb shell am startservice -n com.amtran.factory/.FactoryService --es Action SetBurningMode --ez setMode 1";
            regString = @"Starting service: Intent \{ cmp=com.amtran.factory/.FactoryService \(has extras\) \}";
            try
            {
                while (!RunAdbCmd(cmd, out rst))
                {
                    retry++;
                    if (retry > 3)
                    {
                        SetMsg("找不到機器, FAIL, Không tìm thấy máy", UDF.COLOR.FAIL);
                        return false;
                    }
                    SetMsg($"重新下指令:{retry}, Ra lệnh lại:{retry}", UDF.COLOR.WORK);
                    Delay(1000);
                }
                if (Regex.IsMatch(rst, regString))
                {
                    SetMsg("指令成功, PASS, Lệnh chế độ nhà máy thành công", UDF.COLOR.WORK);
                    retry = 0;
                }
                else
                {
                    SetMsg("指令失敗, FAIL, Lệnh thất bại", UDF.COLOR.FAIL);
                    return false;
                }
            }
            catch (Exception e)
            {
                SetMsg(e.Message, UDF.COLOR.FAIL);
                return false;
            }

            SetMsg("檢查燒機模式是否啟動, Kiểm tra xem chế độ đốt cháy đã được kích hoạt hay chưa", UDF.COLOR.WORK);
            TestName = "Check_burn_in";
            Filename = "command_ack.txt";
            cmd = $"adb pull sdcard/Download/{Filename}";
            compareString = "1";
            try
            {
                if (File.Exists(Filename)) File.Delete(Filename);
                if (!run_cmd_and_check_link(rst, retry, cmd)) return false;
                if (!check_command_ack(TestName, Filename, compareString)) return false;
            }
            catch (Exception e)
            {
                SetMsg(e.Message, UDF.COLOR.FAIL);
                return false;
            }
            //------
            SetMsg("SetFactoryMode", UDF.COLOR.WORK);
            string ack;
            RunAdbCmd("adb shell am startservice -n com.amtran.factory/.FactoryService --es Action SetFactoryMode --ez setMode 1", out ack);
            //------
            // 關閉ADB, Đầu tiên đóng ADB
            RunAdbCmd("taskkill /f /im adb.exe /t", out rst, 1);
            return true;
        }
    }
}