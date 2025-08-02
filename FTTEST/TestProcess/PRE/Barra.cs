using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using PRETEST.UDFs;
using PRETEST.AppConfig;
using PRETEST.Database;
using PRETEST.SDriver;
using System.Data.OleDb;
using System.IO;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace PRETEST.TestProcess.PRE
{
    class Barra { }
}

namespace PRETEST
{
    public partial class FormMain
    {
        [DllImport("Kernel32.dll")]
        private static extern long GetPrivateProfileString(string section,string key,
             string def, StringBuilder retVal, int size, string filePath);
        public static string ReadIniFile(string section, string key, string filePath)
        {
            StringBuilder keyString = new StringBuilder(1024);
            string def = null;
            GetPrivateProfileString(section,key, def, keyString, 1024, filePath);
            return keyString.ToString().Trim();
        }
        public static string sinst_mic = string.Empty;
        public static string builddate2 = string.Empty;
        public static string builddate = string.Empty;
        bool PRE_Barra()
        {
            SetMsg(DateTime.Now.ToString(), UDF.COLOR.WORK);    //DEBUG

            if (SDriverX.g_modelInfo.sPart_no == "AXXUSNFB----.NBARB12")
            {
                if (WriteExtSn() == false) return false;
            }
            
            int VOL = Convert.ToInt16(wbc.iniFile.GetPrivateStringValue("VOL_barra", SDriverX.g_modelInfo.sPart_no, @"\\192.168.158.25\\te\\setup\\FTTEST\\FWVISON\\FW.ini"));
            if (VOL == 0)
            {
                VOL = Convert.ToInt16(wbc.iniFile.GetPrivateStringValue("VOL_barra", "VOL", @"\\192.168.158.25\\te\\setup\\FTTEST\\FWVISON\\FW.ini"));
                if (VOL == 0)
                {
                    VOL = 78;
                }
            }

            SetMsg("vol:"+VOL.ToString(), UDF.COLOR.WORK);
            //if (Barra_StartFactoryService() == false) return false;   //未更新指令
            if (Barra_EntryFactryMode() == false) return false;
            //if (Barra_AdbRoot() == false) return false; //未更新指令
            if (Barra_CheckFw() == false) return false;
            if (Barra_SetAudioVol(VOL) == false) return false;   //13 - 78
            //if (Barra_WriteEthMac() == false) return false; //未更新指令
            if (Barra_WriteEthMacWithExe() == false) return false; //未更新指令
            //if (Barra_CheckEthMac() == false) return false; //未更新指令
            if (Barra_WriteWifiMac() == false) return false;
            //if (Barra_CheckWifiMac() == false) return false; //未更新指令
            if (Barra_SetTvSn() == false) return false;
            if (Barra_GetTvSn() == false) return false;
            if (Barra_SetWifiCountryCode() == false) return false;
            if (Barra_SetVocCo2SelfTest() == false) return false;

            if (Barra_SetMicvendor() == false) return false;
            if (Barra_GetMicvendor() == false) return false;

            if (Barra_Setbuilddate() == false) return false;
            if (Barra_Getbuilddate() == false) return false;
            
            if (Barra_SetBiWifi() == false) return false;
            if (Barra_SetBurnInOn() == false) return false;

            if (UpdateBiTime() == false) return false;

            SetMsg(DateTime.Now.ToString(), UDF.COLOR.WORK);    //DEBUG

            return true;
        }
        bool PRE_Cardhu()
        {
            string ack = string.Empty;
            int VOL = Convert.ToInt16(wbc.iniFile.GetPrivateStringValue("VOL_CARDU_PRE", SDriverX.g_modelInfo.sPart_no, @"\\192.168.158.25\\te\\setup\\FTTEST\\FWVISON\\FW.ini"));
            if (VOL == 0)
            {
                VOL = Convert.ToInt16(wbc.iniFile.GetPrivateStringValue("VOL_CARDU_PRE", "VOL", @"\\192.168.158.25\\te\\setup\\FTTEST\\FWVISON\\FW.ini"));
                if (VOL == 0)
                {
                    VOL = 20;
                }
            }
            SetMsg("vol:"+VOL.ToString(), UDF.COLOR.WORK);
            SetMsg(DateTime.Now.ToString(), UDF.COLOR.WORK);    //DEBUG

            //if (Barra_StartFactoryService() == false) return false;   //未更新指令
            if (Barra_EntryFactryMode() == false) return false;
            //if (Barra_AdbRoot() == false) return false; //未更新指令
            if (GetScalerFwForCardhu() == false) return false;
            if (Barra_CheckFw() == false) return false;

            if (Barra_SetAudioVol(VOL) == false) return false;   //13 - 78
            //if (Barra_WriteEthMac() == false) return false; //未更新指令
            if (Barra_WriteEthMacWithExe() == false) return false; //未更新指令
            //if (Barra_CheckEthMac() == false) return false; //未更新指令
            if (Barra_WriteWifiMac() == false) return false;
            //if (Barra_CheckWifiMac() == false) return false; //未更新指令
            if (Barra_SetTvSn() == false) return false;
            if (Barra_GetTvSn() == false) return false;
            if (Barra_SetWifiCountryCode() == false) return false;
            if (Barra_SetVocCo2SelfTest() == false) return false;

            if (Cardhu_SetMicvendor() == false) return false;
            if (Cardhu_GetMicvendor() == false) return false;

            if (Barra_Setbuilddate() == false) return false;
            if (Barra_Getbuilddate() == false) return false;
            if (Barra_GetLightSensorValue() == false) return false;
            if (Barra_GetTmpSensorValue() == false) return false;


            if (Barra_SetBiWifi() == false) return false;
            if (Barra_SetBurnInOn() == false) return false;
            //set model index
            if (SetModelIndexForCardhu() == false) return false;
            //set bright max 
            RunAdbCmd("adb shell su root i2cset -f -y 9 0x37 0x51 0x86 0x4F 0x63 0x00 0x02 0x00 0x64 0xF3 i", out ack, 1);
            if (SDriverX.g_modelInfo.sPart_no.Substring(9, 3).ToUpper() == "EO1")
            {
                if (UpdateBiTime() == false) return false;
            }
            //if (UpdateBiTime() == false) return false;
            SetMsg(DateTime.Now.ToString(), UDF.COLOR.WORK);    //DEBUG

            return true;
        }
        bool FTA_Cardhu()
        {
            string ack = string.Empty;
           
            SetMsg(DateTime.Now.ToString(), UDF.COLOR.WORK);    //DEBUG
            int VOL = Convert.ToInt16(wbc.iniFile.GetPrivateStringValue("VOL_CARDU_FTA", SDriverX.g_modelInfo.sPart_no, @"\\192.168.158.25\\te\\setup\\FTTEST\\FWVISON\\FW.ini"));
            if (VOL == 0) {
                VOL = Convert.ToInt16(wbc.iniFile.GetPrivateStringValue("VOL_CARDU_FTA", "VOL", @"\\192.168.158.25\\te\\setup\\FTTEST\\FWVISON\\FW.ini"));
                if (VOL == 0)
                {
                    VOL = 78;
                }
            }
            SetMsg("vol:"+VOL.ToString(), UDF.COLOR.WORK);
            //if (Barra_StartFactoryService() == false) return false;   //未更新指令
            //if (Barra_EntryFactryMode() == false) return false;
            //if (Barra_AdbRoot() == false) return false; //未更新指令
            //if (GetScalerFwForCardhu() == false) return false;
            //if (Barra_CheckFw() == false) return false;
            if (Barra_SetAudioVol(VOL) == false) return false;   //13 - 78
            //if (Barra_WriteEthMac() == false) return false; //未更新指令
            //if (Barra_WriteEthMacWithExe() == false) return false; //未更新指令
            //if (Barra_CheckEthMac() == false) return false; //未更新指令
            //if (Barra_WriteWifiMac() == false) return false;
            //if (Barra_CheckWifiMac() == false) return false; //未更新指令
            //if (Barra_SetTvSn() == false) return false;
            //if (Barra_GetTvSn() == false) return false;
            if (Barra_SetWifiCountryCode() == false) return false;
            if (Barra_SetVocCo2SelfTest() == false) return false;
            if (Barra_GetLightSensorValue() == false) return false;
            //if (Cardhu_SetMicvendor() == false) return false;
            //if (Cardhu_GetMicvendor() == false) return false;

            //if (Barra_Setbuilddate() == false) return false;
            //if (Barra_Getbuilddate() == false) return false;

            if (Barra_SetBiWifi() == false) return false;
            if (Barra_SetBurnInOn() == false) return false;
            //set model index
            if (SetModelIndexForCardhu() == false) return false;
            //set bright max 
            RunAdbCmd("adb shell su root i2cset -f -y 9 0x37 0x51 0x86 0x4F 0x63 0x00 0x02 0x00 0x64 0xF3 i", out ack, 1);
            if (UpdateBiTime() == false) return false;
            SetMsg(DateTime.Now.ToString(), UDF.COLOR.WORK);    //DEBUG

            return true;
        }

        bool PRE_Barra_DEBUG()
        {
            SDriverX.g_modelInfo = new UDF.ModelInfo { sSn = "NB11941000022" };
            SetMsg(DateTime.Now.ToString(), UDF.COLOR.WORK);    //DEBUG

            //if (SDriverX.g_modelInfo.sPart_no == "AXXUSNFB----.NBARB12")
            //{
            //    if (WriteExtSn() == false) return false;
            //}

            //if (Barra_StartFactoryService() == false) return false;   //未更新指令
            if (Barra_EntryFactryMode() == false) return false;
            //if (Barra_AdbRoot() == false) return false; //未更新指令
            if (Barra_CheckFw() == false) return false;/*return false*/;
            if (Barra_SetAudioVol(13) == false) return false;
            //if (Barra_WriteEthMac() == false) return false; //未更新指令
            if (Barra_WriteEthMacWithExe() == false) return false; //未更新指令 ----
            //if (Barra_CheckEthMac() == false) return false; //未更新指令
            if (Barra_WriteWifiMac() == false) return false;
            //if (Barra_CheckWifiMac() == false) return false; //未更新指令
            if (Barra_SetTvSn() == false) return false;
            if (Barra_GetTvSn() == false) return false;
            if (Barra_SetWifiCountryCode() == false) return false;
            //if (Barra_SetVocCo2SelfTest() == false) return false;
            //if (Barra_SetBiWifi() == false) return false;
            if (Barra_SetBurnInOn() == false) return false;
            if (UpdateBiTime() == false) return false;
            SetMsg(DateTime.Now.ToString(), UDF.COLOR.WORK);    //DEBUG

            return true;
        }

        bool Barra_StartFactoryService()
        {
            string ack;
            int iRetryTime = new int();

            iRetryTime = 0;
        StartFacyService:
            SetMsg("Start factory services...", UDF.COLOR.WORK);
            RunAdbCmd("adb shell am startservice -n com.amtran.factory/.FactoryService", out ack);
            if (ResultCheck(ack, "Starting service") == false)
            {
                SetMsg("Err:" + ack, UDF.COLOR.FAIL);
                SetMsg("Start factory services fail!", UDF.COLOR.FAIL);
                if (iRetryTime > 2)
                    return false;
                else
                {
                    iRetryTime++;
                    goto StartFacyService;
                }
            }

            return true;
        }

        bool Barra_EntryFactryMode()
        {
            string ack;
            int iRetryTime = new int();

            iRetryTime = 0;
            FactMode:
            SetMsg("Factory mode on...", UDF.COLOR.WORK);
            RunAdbCmd("adb shell am startservice -n com.amtran.factory/.FactoryService --es Action SetFactoryMode --ez setMode 1", out ack, 1);
            RunAdbCmd(  "adb shell cat /sdcard/Download/command_ack.txt", out ack,0);
            if (ResultCheck(ack, "FactoryMode=1") == false)
            {
                SetMsg("Err:" + ack, UDF.COLOR.FAIL);
                SetMsg("Factory mode on fail!", UDF.COLOR.FAIL);
                if (iRetryTime > 2)
                    return false;
                else
                {
                    iRetryTime++;
                    goto FactMode;
                }
            }

            return true;
        }

        bool Barra_AdbRoot()
        {
            string ack;
            int iRetryTime = new int();

            iRetryTime = 0;
        AdbRoot:
            SetMsg("adb root...", UDF.COLOR.WORK);
            RunAdbCmd("adb root", out ack);
            if (ResultCheck(ack, "as root") == false)
            {
                SetMsg("Err:" + ack, UDF.COLOR.FAIL);
                SetMsg("adb root fail!", UDF.COLOR.FAIL);
                if (iRetryTime > 2)
                    return false;
                else
                {
                    iRetryTime++;
                    goto AdbRoot;
                }
            }
            return true;
        }

        bool Barra_CheckFw()
        {
            string ack, ack2, progid;
            int iRetryTime = new int();

            iRetryTime = 0;
        GetFw:
            SetMsg("Get fw version...", UDF.COLOR.WORK);
            RunAdbCmd("adb shell am startservice -n com.amtran.factory/.FactoryService --es Action GetFWVersion",
                "adb shell cat /sdcard/Download/data_command_ack.txt", out ack);
            if (ResultPick(ack, @"(?<=FWVersion=).*?(?=\r|\n)", out ack2) == false)
            {
                SetMsg("Err:" + ack, UDF.COLOR.FAIL);
                SetMsg("Get fw version fail!", UDF.COLOR.FAIL);
                if (iRetryTime > 2)
                    return false;
                else
                {
                    iRetryTime++;
                    goto GetFw;
                }
            }

            if ((g_iPtInitFlag & INIT_FW) == INIT_FW)  //如果已经有了首次查询了机种FW信息，则比较缓存数据不再联网查询
            {
                if ((ack2.IndexOf(g_sFwVer1) == -1) && (ack2.IndexOf(g_sFwVer2) == -1))   //机台 fw 与 数据库 fw1、fw2 均比较错误
                {
                    //FW 比对失败
                    SetMsg("FW比对错误！机台FW:" + ack2 + "\n数据库FW:" + g_sFwVer1 + "/" + g_sFwVer2, UDF.COLOR.FAIL);
                    return false;
                }
            }
            else //否则从数据库更新 FW信息，下一次不再联网查询，除非切换了新工单
            {
                if (QueryProgIdSql(out progid) == false)
                {
                    SetMsg("查询机种 progid fail", UDF.COLOR.FAIL);
                    return false;
                }
                else
                {
                    //g_iPtInitFlag = INIT_ZERO;  //工单切换后，这个 flag 会重置为 0，以通知其他部分更新信息，比如从数据库更新此工单应该卡的 FW 信息
                    if (CheckFwVerSql(progid, ack2) == false) return false;
                }
            }


            //Compare FW
            //SetMsg("Read FW:" + ack2, UDF.COLOR.WORK);
            //SetMsg("Compare FW:" + ack2, UDF.COLOR.WORK);
            //if (ack2 != GlobalConfig.sFw && ack2 != GlobalConfig.sFw2)
            //{
            //    SetMsg("Compare Fw fail, sepc:" + GlobalConfig.sFw + "/" + GlobalConfig.sFw2, UDF.COLOR.FAIL);
            //    return false;
            //}

            return true;
        }

        bool Barra_SetAudioVol(int vol = 6)
        {
            string ack;
            int iRetryTime = new int();

            iRetryTime = 0;
        FactMode:
            SetMsg("set media Volume...", UDF.COLOR.WORK);
            RunAdbCmd("adb shell am startservice -n com.amtran.factory/.FactoryService --es Action SetMediaVolume --ei mediaVolume " + vol.ToString(),
                "adb shell cat /sdcard/Download/command_ack.txt", out ack);
            if (ResultCheck(ack, "MediaVolume=" + vol.ToString()) == false)
            {
                SetMsg("Err:" + ack, UDF.COLOR.FAIL);
                SetMsg("set media Volume fail!", UDF.COLOR.FAIL);
                if (iRetryTime > 2)
                    return false;
                else
                {
                    iRetryTime++;
                    goto FactMode;
                }
            }

            return true;
        }

        bool Barra_GetMAC(UDF.MAC_TYPE mac_type)
        {
            string sql = string.Empty;
            string sEthMac = string.Empty;
            string sWifiMac = string.Empty;
            string sBtMac = string.Empty;
            bool bErr = new bool();
            int flag = new int();
            OleDbDataReader reader;

            if (mac_type == UDF.MAC_TYPE.ETH)
            {
                #region ETH MAC
                SetMsg("正在从数据库检索 ETH MAC...", UDF.COLOR.WORK);
                //先查询是否已申请
                sql = string.Format("select * from rknmgr.insp_barrakey_t where use_mode = 0 and ssn = '{0}'",
                    SDriverX.g_modelInfo.sSn);
                //bErr = Oracle.ServerExecute(sql, out reader);
                //if (bErr)
                //{
                //    reader.Read();
                //    if (reader.HasRows)
                //    {
                //        //已有数据
                //        flag = 1;
                //        sEthMac = reader[0].ToString();
                //        SDriverX.g_modelInfo.sEthMac = reader[0].ToString().ToUpper();
                //        SDriverX.g_modelInfo.sUseFlag = Convert.ToInt32(reader[2]);
                //    }
                //    else
                //    {
                //        //没有数据
                //        flag = 2;
                //    }
                //    reader.Close();
                //}
                //else
                //{
                //    SetMsg("数据库操作失败", UDF.COLOR.FAIL);
                //    return false;
                //}

                if (flag == 2)
                {
                    //正常情况下是，打印标签时分配 ETH MAC的。未分配则提示报错
                    SetMsg("当前SN未分配ETH MAC，请联系MES或LABLE室", UDF.COLOR.FAIL);
                    return false;
                }

                //保留自申请功能
                //申请key并更新表
                //if (flag == 2)
                //{
                //    //如果之前没有申请Eth Mac,现在申请
                //    sql = "select * from rknmgr.insp_barrakey_t where use_mode = 0 and use_flag = 0 and rownum = 1";
                //    bErr = Oracle.ServerExecute(sql, out reader);
                //    if (bErr)
                //    {
                //        reader.Read();
                //        if (reader.HasRows)
                //        {
                //            //已有数据
                //            flag = 1;
                //            sEthMac = reader[0].ToString();
                //            SDriverX.g_modelInfo.sEthMac = StrToMac(reader[0].ToString()).ToUpper();
                //            SDriverX.g_modelInfo.sUseFlag = Convert.ToInt32(reader[2]);
                //        }
                //        else
                //        {
                //            //没有数据
                //            SetMsg("从数据库申请 ETH MAC fail，请联系TE", UDF.COLOR.FAIL);
                //            return false;
                //        }
                //        reader.Close();
                //    }
                //    else
                //    {
                //        SetMsg("数据库操作失败", UDF.COLOR.FAIL);
                //        return false;
                //    }

                //    //更新表
                //    sql = string.Format("update rknmgr.insp_barrakey_t set ssn = '{0}', use_flag = {1}, use_date = sysdate where keycode = '{2}'",
                //        SDriverX.g_modelInfo.sSn,
                //        (Convert.ToInt32(SDriverX.g_modelInfo.sUseFlag) + 1).ToString(),
                //        sEthMac);
                //    bErr = Oracle.UpdateServer(sql);
                //    if (!bErr)
                //    {
                //        SetMsg("数据库操作失败,更新数据库表失败", UDF.COLOR.FAIL);
                //        return false;
                //    }

                //    //检验数据库信息
                //    sql = string.Format("select * from rknmgr.insp_barrakey_t where use_mode = 0 and ssn = '{0}'",
                //           SDriverX.g_modelInfo.sSn
                //            );
                //    bErr = Oracle.ServerExecute(sql, out reader);
                //    if (bErr)
                //    {
                //        reader.Read();
                //        if (reader.HasRows)
                //        {
                //            //有数据
                //            if (reader[0].ToString() != sEthMac)
                //            {
                //                SetMsg(string.Format("数据库信息较验证错误,{0}/{1}", sEthMac, reader[0].ToString()), UDF.COLOR.FAIL);
                //                reader.Close();
                //                return false;
                //            }
                //        }
                //        else
                //        {
                //            //没有数据
                //            SetMsg("数据库信息更新失败", UDF.COLOR.FAIL);
                //            reader.Close();
                //            return false;
                //        }
                //        reader.Close();
                //    }
                //    else
                //    {
                //        SetMsg("数据库操作失败", UDF.COLOR.FAIL);
                //        return false;
                //    }
                //} //end if (flag == 2)
                #endregion END ETH MAC
            }   // end if (mac_type == UDF.MAC_TYPE.ETH)
            else if (mac_type == UDF.MAC_TYPE.BT)
            {
                #region BT MAC
                SetMsg("正在从数据库检索 BT MAC...", UDF.COLOR.WORK);
                //先查询是否已申请
                sql = string.Format("select * from rknmgr.insp_barrakey_t where use_mode = 1 and ssn = '{0}'",
                    SDriverX.g_modelInfo.sSn);
                //bErr = Oracle.ServerExecute(sql, out reader);
                //if (bErr)
                //{
                //    reader.Read();
                //    if (reader.HasRows)
                //    {
                //        //已有数据
                //        flag = 1;
                //        sBtMac = reader[0].ToString();
                //        SDriverX.g_modelInfo.sBtMac = reader[0].ToString().ToUpper();
                //        SDriverX.g_modelInfo.sUseFlag = Convert.ToInt32(reader[2]);
                //    }
                //    else
                //    {
                //        //没有数据
                //        flag = 2;
                //    }
                //    reader.Close();
                //}
                //else
                //{
                //    SetMsg("数据库操作失败", UDF.COLOR.FAIL);
                //    return false;
                //}

                //申请key并更新表
                if (flag == 2)
                {
                    //如果之前没有申请Eth Mac,现在申请
                    sql = "select * from rknmgr.insp_barrakey_t where use_mode = 1 and use_flag = 0 and rownum = 1";
                    //bErr = Oracle.ServerExecute(sql, out reader);
                    //if (bErr)
                    //{
                    //    reader.Read();
                    //    if (reader.HasRows)
                    //    {
                    //        //已有数据
                    //        flag = 1;
                    //        sBtMac = reader[0].ToString();
                    //        SDriverX.g_modelInfo.sBtMac = reader[0].ToString().ToUpper();
                    //    }
                    //    else
                    //    {
                    //        //没有数据
                    //        SetMsg("从数据库申请 BT MAC fail，请联系TE", UDF.COLOR.FAIL);
                    //        return false;
                    //    }
                    //    reader.Close();
                    //}
                    //else
                    //{
                    //    SetMsg("数据库操作失败", UDF.COLOR.FAIL);
                    //    return false;
                    //}

                    //更新表
                    sql = string.Format("update rknmgr.insp_barrakey_t set ssn = '{0}', use_flag = {1}, use_date = sysdate where keycode = '{2}'",
                        SDriverX.g_modelInfo.sSn,
                        (Convert.ToInt32(SDriverX.g_modelInfo.sUseFlag) + 1).ToString(),
                        sEthMac);
                    //bErr = Oracle.UpdateServer(sql);
                    //if (!bErr)
                    //{
                    //    SetMsg("数据库操作失败,更新数据库表失败", UDF.COLOR.FAIL);
                    //    return false;
                    //}

                    //检验数据库信息
                    sql = string.Format("select * from rknmgr.insp_barrakey_t where t.use_mode = 1 and t.ssn = '{0}'",
                           SDriverX.g_modelInfo.sSn
                            );
                    //bErr = Oracle.ServerExecute(sql, out reader);
                    //if (bErr)
                    //{
                    //    reader.Read();
                    //    if (reader.HasRows)
                    //    {
                    //        //有数据
                    //        if (reader[0].ToString() != sBtMac)
                    //        {
                    //            SetMsg(string.Format("数据库信息较验证错误,{0}/{1}", sBtMac, reader[0].ToString()), UDF.COLOR.FAIL);
                    //            reader.Close();
                    //            return false;
                    //        }
                    //    }
                    //    else
                    //    {
                    //        //没有数据
                    //        SetMsg("数据库信息更新失败", UDF.COLOR.FAIL);
                    //        reader.Close();
                    //        return false;
                    //    }
                    //    reader.Close();
                    //}
                    //else
                    //{
                    //    SetMsg("数据库操作失败", UDF.COLOR.FAIL);
                    //    return false;
                    //}
                } //end if (flag == 2)
                #endregion END BT MAC
            } // end else if (mac_type == UDF.MAC_TYPE.ETH)
            else if (mac_type == UDF.MAC_TYPE.WIFI)
            {
                #region WIFI MAC
                SetMsg("正在从数据库检索 WIFI MAC...", UDF.COLOR.WORK);
                //先查询是否已申请
                sql = string.Format("select * from rknmgr.insp_barrakey_t where use_mode = 2 and ssn = '{0}'",
                    SDriverX.g_modelInfo.sSn);
                //bErr = Oracle.ServerExecute(sql, out reader);
                //if (bErr)
                //{
                //    reader.Read();
                //    if (reader.HasRows)
                //    {
                //        //已有数据
                //        flag = 1;
                //        sWifiMac = reader[0].ToString();
                //        SDriverX.g_modelInfo.sWifiMac = reader[0].ToString().ToUpper();
                //        SDriverX.g_modelInfo.sUseFlag = Convert.ToInt32(reader[2]);
                //    }
                //    else
                //    {
                //        //没有数据
                //        flag = 2;
                //    }
                //    reader.Close();
                //}
                //else
                //{
                //    SetMsg("数据库操作失败(检索)", UDF.COLOR.FAIL);
                //    return false;
                //}

                //申请key并更新表
                if (flag == 2)
                {
                    //如果之前没有申请Wifi Mac,现在申请
                    sql = "select * from rknmgr.insp_barrakey_t where use_mode = 2 and use_flag = 0 and rownum = 1";
                    //bErr = Oracle.ServerExecute(sql, out reader);
                    //if (bErr)
                    //{
                    //    reader.Read();
                    //    if (reader.HasRows)
                    //    {
                    //        //已有数据
                    //        flag = 1;
                    //        sWifiMac = reader[0].ToString();
                    //        SDriverX.g_modelInfo.sWifiMac = reader[0].ToString().ToUpper();
                    //        SDriverX.g_modelInfo.sUseFlag = Convert.ToInt32(reader[2]);
                    //    }
                    //    else
                    //    {
                    //        //没有数据
                    //        SetMsg("从数据库申请 WIFI MAC fail，请联系TE", UDF.COLOR.FAIL);
                    //        return false;
                    //    }
                    //    reader.Close();
                    //}
                    //else
                    //{
                    //    SetMsg("数据库操作失败(申请Key)", UDF.COLOR.FAIL);
                    //    return false;
                    //}

                    //更新表
                    sql = string.Format("update rknmgr.insp_barrakey_t set ssn = '{0}', use_flag = {1}, use_date = sysdate where keycode = '{2}'",
                        SDriverX.g_modelInfo.sSn,
                        (Convert.ToInt32(SDriverX.g_modelInfo.sUseFlag) + 1).ToString(),
                        sWifiMac);
                    //bErr = Oracle.UpdateServer(sql);
                    //if (!bErr)
                    //{
                    //    SetMsg("数据库操作失败,更新数据库表失败", UDF.COLOR.FAIL);
                    //    return false;
                    //}

                    Delay(1000);
                    //检验数据库信息
                    sql = string.Format("select * from rknmgr.insp_barrakey_t where use_mode = 2 and ssn = '{0}'",
                           SDriverX.g_modelInfo.sSn
                            );
                    //bErr = Oracle.ServerExecute(sql, out reader);
                    //if (bErr)
                    //{
                    //    reader.Read();
                    //    if (reader.HasRows)
                    //    {
                    //        //有数据
                    //        if (reader[0].ToString() != sWifiMac)
                    //        {
                    //            SetMsg(string.Format("数据库信息较验证错误,{0}/{1}", sWifiMac, reader[0].ToString()), UDF.COLOR.FAIL);
                    //            reader.Close();
                    //            return false;
                    //        }
                    //    }
                    //    else
                    //    {
                    //        //没有数据
                    //        SetMsg("数据库信息更新失败", UDF.COLOR.FAIL);
                    //        reader.Close();
                    //        return false;
                    //    }
                    //    reader.Close();
                    //}
                    //else
                    //{
                    //    SetMsg("数据库操作失败(校验)", UDF.COLOR.FAIL);
                    //    return false;
                    //}
                } //end if (flag == 2)
                #endregion END WIFI MAC

            } // end else if (mac_type == UDF.MAC_TYPE.ETH)
            else
            {
                SetMsg("MAC TYPE参数错误，请联系TE工程师", UDF.COLOR.FAIL);
                return false;
            }

            return true;
        }

        bool Barra_WriteEthMac()
        {

            int iRetryTime = new int();
            string ack, ack2;
            string sEthMac = string.Empty;

            iRetryTime = 0;
        GetEthMac:
            SetMsg("Get Eth MAC...", UDF.COLOR.WORK);
            if (Barra_GetMAC(UDF.MAC_TYPE.ETH) == false)
            {
                if (iRetryTime > 2)
                    return false;
                else
                {
                    iRetryTime++;
                    goto GetEthMac;
                }
            }

            SetMsg("[Debug] 指定ETH MAC:C4:63:FB:00:11:2C", UDF.COLOR.WORK);
            SDriverX.g_modelInfo.sEthMac = "C4:63:FB:00:11:2C";

            if (SwitchMacStyle(SDriverX.g_modelInfo.sEthMac, UDF.MAC_STYLE.LC17, out sEthMac) == false)
            {
                SetMsg("Eth Mac检查失败！Eth Mac:" + SDriverX.g_modelInfo.sEthMac, UDF.COLOR.FAIL);
                return false;
            }

            iRetryTime = 0;
        WriteEthMac:
            SetMsg("Write Eth MAC...", UDF.COLOR.WORK);
            RunAdbCmd("adb shell am broadcast -a com.amtran.factory.SetEthMAC --es ethMac " + sEthMac,
                "adb shell cat /sys/class/net/eth0/address", out ack);
            if (ResultPick(ack, @"([a-z A-Z 0-9]{2}:){5}[a-z A-Z 0-9]{2}", out ack2) == false)
            {
                SetMsg("Err:" + ack, UDF.COLOR.FAIL);
                SetMsg("Write Eth MAC fail!", UDF.COLOR.FAIL);
                if (iRetryTime > 2)
                    return false;
                else
                {
                    iRetryTime++;
                    goto WriteEthMac;
                }
            }

            return true;
        }

        bool Barra_WriteEthMacWithExe()
        {
            int iRetryTime = new int();
            string ack;
            string sEthMac = string.Empty;

            iRetryTime = 0;
        GetEthMac:
            SetMsg("Get Eth MAC...", UDF.COLOR.WORK);
            if (Barra_GetMAC(UDF.MAC_TYPE.ETH) == false)
            {
                if (iRetryTime > 2)
                    return false;
                else
                {
                    iRetryTime++;
                    goto GetEthMac;
                }
            }

            if (SwitchMacStyle(SDriverX.g_modelInfo.sEthMac, UDF.MAC_STYLE.LC17, out sEthMac) == false)
            {
                SetMsg("Eth Mac检查失败！Eth Mac:" + SDriverX.g_modelInfo.sEthMac, UDF.COLOR.FAIL);
                return false;
            }
            //SetMsg("Push ETH FILE...", UDF.COLOR.WORK);

            //RunAdbCmd("pushRTL8125tool.bat", out ack, 0);
            //RunAdbCmd( Application.StartupPath + "\\WriteEthMac" + "\\pushRTL8125tool.bat", out ack, 0);

            Delay(500);
            iRetryTime = 0;
            WriteEthMac:
            SetMsg("Write Eth MAC...", UDF.COLOR.WORK);
            // RunAdbCmd(GlobalConfig.sCmd1Path + " " + sEthMac, out ack, 1);
            
            // RunAdbCmd("type " + Directory.GetCurrentDirectory() + @"\command_ack.txt", out ack, 1);
            RunAdbCmd(GlobalConfig.sCmd1Path + " " + sEthMac,
                "type " + Directory.GetCurrentDirectory() + @"\WriteEthMac"+ @"\command_ack.txt", out ack, 1);
            if (ResultCheck(ack, "true") == false)
            {
                SetMsg("Write Eth MAC fail!", UDF.COLOR.FAIL);
                if (iRetryTime > 1)
                    return false;
                else
                {
                    iRetryTime++;
                    goto WriteEthMac;
                }
            }

            return true;
        }

        bool Barra_CheckEthMac()
        {
            string ack, ack2;
            int iRetryTime = new int();

            iRetryTime = 0;
        GetMac:
            SetMsg("Get Eth MAC...", UDF.COLOR.WORK);
            RunAdbCmd("adb shell am broadcast -a com.amtran.factory.GetEthMAC",
                "adb shell cat /sdcard/Download/data_command_ack.txt", out ack);
            if (ResultPick(ack, @"(?<=MAC=).*?(?=\r|\n)", out ack2) == false)
            {
                SetMsg(ack, UDF.COLOR.FAIL);
                SetMsg("Get Eth MAC fail!", UDF.COLOR.FAIL);
                if (iRetryTime > 2)
                    return false;
                else
                {
                    iRetryTime++;
                    goto GetMac;
                }
            }
            SetMsg("Eth MAC:" + ack2, UDF.COLOR.WORK);

            return true;
        }

        bool Barra_WriteWifiMac()
        {
            string ack;
            string sWifiMac = string.Empty;
            int iRetryTime = new int();

            //if (Barra_AdbRoot() == false) return false;


            iRetryTime = 0;
        GetWifiMac:
            SetMsg("Get WiFi MAC...", UDF.COLOR.WORK);
            if (Barra_GetMAC(UDF.MAC_TYPE.WIFI) == false)
            {
                if (iRetryTime > 2)
                    return false;
                else
                {
                    iRetryTime++;
                    goto GetWifiMac;
                }
            }

            if (SwitchMacStyle(SDriverX.g_modelInfo.sWifiMac, UDF.MAC_STYLE.LC17, out sWifiMac) == false)
            {
                SetMsg("Eth Mac检查失败！WiFi Mac:" + SDriverX.g_modelInfo.sWifiMac, UDF.COLOR.FAIL);
                return false;
            }

            iRetryTime = 0;
        MacUpdate:
            SetMsg("adb shell mac_update...", UDF.COLOR.WORK);
            RunAdbCmd("adb shell su root mac_update " + sWifiMac, out ack);
            if (ResultCheck(ack, "completed|updated") == false)
            {
                SetMsg("Err:" + ack, UDF.COLOR.FAIL);
                SetMsg("adb shell mac_update fail!", UDF.COLOR.FAIL);
                if (iRetryTime > 2)
                    return false;
                else
                {
                    iRetryTime++;
                    goto MacUpdate;
                }
            }

            return true;
        }

        bool Barra_CheckWifiMac()
        {
            string ack, ack2;
            int iRetryTime = new int();

            iRetryTime = 0;
        GetMac:
            SetMsg("Get WiFi MAC...", UDF.COLOR.WORK);
            RunAdbCmd("adb shell am broadcast -a com.amtran.factory.GetWiFiMAC",
                "adb shell cat /sdcard/Download/data_command_ack.txt", out ack);
            if (ResultPick(ack, @"(?<=MAC=).*?(?=\r|\n)", out ack2) == false)
            {
                SetMsg("Err:" + ack, UDF.COLOR.FAIL);
                SetMsg("Get WiFi MAC fail!", UDF.COLOR.FAIL);
                if (iRetryTime > 2)
                    return false;
                else
                {
                    iRetryTime++;
                    goto GetMac;
                }
            }

            SetMsg("WiFi MAC:" + ack2, UDF.COLOR.WORK);

            return true;
        }

        bool Barra_SetTvSn()
        {
            string ack;
            int iRetryTime = new int();

            //if (Barra_AdbRoot() == false) return false;

            iRetryTime = 0;
        SetSn:
            SetMsg("Set sn...", UDF.COLOR.WORK);
            RunAdbCmd("adb shell su root nf_persist -s -k serialno -v " + SDriver.SDriverX.g_modelInfo.sSn, out ack);
            if (ResultCheck(ack, "Setting serialno=") == false)
            {
                SetMsg("Err:" + ack, UDF.COLOR.FAIL);
                SetMsg("Set sn fail!", UDF.COLOR.FAIL);
                if (iRetryTime > 2)
                    return false;
                else
                {
                    iRetryTime++;
                    goto SetSn;
                }
            }

            return true;
        }

        bool Barra_SetMicvendor()
        {
            string ack;

            if (textBoxPn.Text.Substring(textBoxPn.Text.Length - 3, 3) == "B19" || textBoxPn.Text.Substring(textBoxPn.Text.Length - 3, 3) == "B1A" || textBoxPn.Text.Substring(textBoxPn.Text.Length - 3, 3) == "B1B" || textBoxPn.Text.Substring(textBoxPn.Text.Length - 3, 3) == "B1C" || textBoxPn.Text.Substring(textBoxPn.Text.Length - 3, 3) == "B1D")
            {
                SetMsg("Set Mic vendor...", UDF.COLOR.WORK);
                RunAdbCmd("adb shell su root nf_persist -s -k inst_mic -v merry > command_ack.txt ", out ack);
        }
            return true;
        }
        bool Barra_GetMicvendor()
        {
            string ack;
            if (textBoxPn.Text.Substring(textBoxPn.Text.Length - 3, 3) == "B19" || textBoxPn.Text.Substring(textBoxPn.Text.Length - 3, 3) == "B1A" || textBoxPn.Text.Substring(textBoxPn.Text.Length - 3, 3) == "B1B" || textBoxPn.Text.Substring(textBoxPn.Text.Length - 3, 3) == "B1C" || textBoxPn.Text.Substring(textBoxPn.Text.Length - 3, 3) == "B1D")
            {
                SetMsg("Get Mic vendor...", UDF.COLOR.WORK);
                RunAdbCmd("adb shell su root nf_persist > data_command_ack.txt ", out ack);
                string s = Application.StartupPath + "\\data_command_ack.txt";
                string[] contents = File.ReadAllLines(s, Encoding.Default);
                for (int i = 0; i < contents.Length; i++)
                {
                    while (contents[i] == "inst_mic=merry")
                    {
                        return true;
                    }
                }
                SetMsg("Get build date fail...", UDF.COLOR.FAIL);
                return false;
            }
            return true;
        }

        bool Cardhu_SetMicvendor()
        {
            string ack;

            if (textBoxPn.Text == "M65WWNFB2EO1.CARDHU5" || textBoxPn.Text == "M65WWNFB2EO1.CARDHU6" || textBoxPn.Text == "M65WWNFB2AO1.CARDHU3" || textBoxPn.Text == "M65WWNFB2AO1.CARDHU4" || textBoxPn.Text == "M65WWNFB2EO1.CARDHUB" || textBoxPn.Text == "M65WWNFB2EO1.CARDHUC")
            {
                SetMsg("Set Mic vendor...", UDF.COLOR.WORK);
                RunAdbCmd("adb shell su root nf_persist -s -k inst_mic -v merry > command_ack.txt ", out ack);
        }
            return true;
        }
        bool Cardhu_GetMicvendor()
        {
            string ack;
            if (textBoxPn.Text == "M65WWNFB2EO1.CARDHU5" || textBoxPn.Text == "M65WWNFB2EO1.CARDHU6" || textBoxPn.Text == "M65WWNFB2AO1.CARDHU3" || textBoxPn.Text == "M65WWNFB2AO1.CARDHU4" || textBoxPn.Text == "M65WWNFB2EO1.CARDHUB" || textBoxPn.Text == "M65WWNFB2EO1.CARDHUC")
            {
                SetMsg("Get Mic vendor...", UDF.COLOR.WORK);
                RunAdbCmd("adb shell su root nf_persist > data_command_ack.txt ", out ack);
                string s = Application.StartupPath + "\\data_command_ack.txt";
                string[] contents = File.ReadAllLines(s, Encoding.Default);
                for (int i = 0; i < contents.Length; i++)
                {
                    while (contents[i] == "inst_mic=merry")
                    {
                        return true;
                    }
                }
                SetMsg("Get build date fail...", UDF.COLOR.FAIL);
                return false;
            }
            return true;
        }
        bool Barra_Setbuilddate()
        {
            string ack;
            RunAdbCmd("adb shell getprop ro.build.date.utc", out ack);
            Delay(2000);
            builddate = ack.Replace("\r\n", "");
            SetMsg(ack, UDF.COLOR.WORK);
            SetMsg("Set build date...", UDF.COLOR.WORK);
            RunAdbCmd("adb shell su root nf_persist -s -k fswtstamp -v "+ ack, out ack);
            return true;
        }
        bool Barra_Getbuilddate()
        {
            string ack;

            SetMsg("Get build date...", UDF.COLOR.WORK);
            RunAdbCmd("adb shell su root nf_persist > data_commaand_ack.txt ", out ack);
            string s = Application.StartupPath + "\\data_commaand_ack.txt";
            string[] contents = File.ReadAllLines(s, Encoding.Default);
            for (int i = 0; i < contents.Length; i++)
            {
                while (contents[i].Substring(0,10) == "fswtstamp=")
                {
                    if (contents[i].Substring(10, contents[i].Length - 10) == builddate)
                    {
                        return true;
                    }
                    
                }
            }
            SetMsg("Get build date fail...", UDF.COLOR.FAIL);
            return false;
        }

        bool Barra_GetTvSn()
        {
            string ack, ack2;
            int iRetryTime = new int();

            //if (Barra_AdbRoot() == false) return false;

            iRetryTime = 0;
        GetMac:
            SetMsg("Get TV sn...", UDF.COLOR.WORK);
            //RunAdbCmd("adb shell su root nf_persist", out ack);

            //新版 GET SN（须在第一次写完后，断上电）
            //RunAdbCmd("adb shell am startservice -n com.amtran.factory/.FactoryService --es Action GetSN",
            //    "adb shell cat /sdcard/Download/data_command_ack.txt", out ack);
            //if (ResultPick(ack, @"(?<=SN=).*?(?=\r|\n)", out ack2) == false)

            RunAdbCmd("adb shell su root nf_persist", out ack);
            if (ResultPick(ack, @"(?<=serialno=).*?(?=\r|\n)", out ack2) == false)
            {
                SetMsg("Err:" + ack, UDF.COLOR.FAIL);
                SetMsg("Get tv sn fail!", UDF.COLOR.FAIL);
                if (iRetryTime > 2)
                    return false;
                else
                {
                    iRetryTime++;
                    goto GetMac;
                }
            }
            SetMsg("TV sn:" + ack2, UDF.COLOR.WORK);
            SetMsg("Compare TV sn...", UDF.COLOR.WORK);
            if (ack2 != SDriverX.g_modelInfo.sSn)
            {
                SetMsg("Compare TV sn fail!", UDF.COLOR.FAIL);
                return false;
            }

            return true;
        }

        bool Barra_SetWifiCountryCode()
        {
            string ack;
            int iRetryTime = new int();

            //if (Barra_AdbRoot() == false) return false;

            iRetryTime = 0;
        SetWifiCountryCode:
            SetMsg("Set WiFi country code...", UDF.COLOR.WORK);
            RunAdbCmd("adb shell su root nf_persist -s -k countrycode -v US", out ack);
            if (ResultCheck(ack, "Setting countrycode=") == false)
            {
                SetMsg("Err:" + ack, UDF.COLOR.FAIL);
                SetMsg("Set WiFi country code fail!", UDF.COLOR.FAIL);
                if (iRetryTime > 2)
                    return false;
                else
                {
                    iRetryTime++;
                    goto SetWifiCountryCode;
                }
            }

            return true;
        }
        bool Barra_GetLightSensorValue()
        {
            string ack;
            int iRetryTime = new int();
            double lux = 0;

            iRetryTime = 0;
        GetSensorValue:
            SetMsg("Get light sensor value...", UDF.COLOR.WORK);

            
                RunAdbCmd("adb shell nf_sensors", out ack);

                if (ResultCheck(ack, "als:") == false)
                {
                    SetMsg("Err:" + ack, UDF.COLOR.FAIL);
                    SetMsg("Get light sensor value fail!", UDF.COLOR.FAIL);
                    if (iRetryTime > 2)
                        return false;
                    else
                    {
                        iRetryTime++;
                        goto GetSensorValue;
                    }
                }
                else
                {
                    int a = ack.IndexOf("als:");
                    ack = ack.Substring(a + 7, 4);
                    lux = Convert.ToDouble(ack);
                    if (lux < g_dLightLuxSpec[0] || lux > g_dLightLuxSpec[1])
                    {
                        SetMsg(string.Format("Lux值超规,当前值{0},规格{1}-{2}", lux, g_dLightLuxSpec[0], g_dLightLuxSpec[1]), UDF.COLOR.FAIL);
                        if (iRetryTime > 2)
                            return false;
                        else
                        {
                            iRetryTime++;
                            goto GetSensorValue;
                        }
                    }
                }

            SetMsg(string.Format("Lux PASS,当前值{0},规格{1}-{2}", lux, g_dLightLuxSpec[0], g_dLightLuxSpec[1]), UDF.COLOR.WORK);

            WriteLog("LightSensorLog.txt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss  ") + SDriverX.g_modelInfo.sSn + "\t\t" + ack.Replace("\r", " ").Replace("\n", ""));

            return true;
        }
        bool Barra_GetTmpSensorValue()
        {
            string ack;
            int iRetryTime = new int();
            //double temp, humidity;

            iRetryTime = 0;
            GetSensorValue:
            SetMsg("Get Temp & Humidity value...", UDF.COLOR.WORK);
            RunAdbCmd("adb shell am startservice -n com.amtran.factory/.FactoryService --es Action GetTempHumidity",
                "adb shell cat /sdcard/Download/data_command_ack.txt", out ack);
            if (ResultCheck(ack, "true") == false)
            {
                SetMsg("Err:" + ack, UDF.COLOR.FAIL);
                SetMsg("Get Temp & Humidity value fail!", UDF.COLOR.FAIL);
                if (iRetryTime > 2)
                    return false;
                else
                {
                    iRetryTime++;
                    goto GetSensorValue;
                }
            }

            return true;
        }
        bool Barra_SetVocCo2SelfTest()
        {
            string ack;
            int iRetryTime = new int();

            iRetryTime = 0;
        GetMac:
            SetMsg("VOC & CO2 self-test...", UDF.COLOR.WORK);
            RunAdbCmd("adb shell am startservice -n com.amtran.factory/.FactoryService --es Action VocCo2SelfTest",
                "adb shell cat /sdcard/Download/command_ack.txt", out ack);
            if (ResultCheck(ack, @"VocCo2SelfTest=OK") == false)
            {
                SetMsg("Err:" + ack, UDF.COLOR.FAIL);
                SetMsg("VOC & CO2 self-test fail!", UDF.COLOR.FAIL);
                if (iRetryTime > 2)
                    return false;
                else
                {
                    iRetryTime++;
                    goto GetMac;
                }
            }

            return true;
        }

        bool Barra_SetBiWifi()
        {
            string ack;
            int iRetryTime = new int();

            iRetryTime = 0;
        WifiConfig:
            SetMsg("Set Burning-WiFi config...", UDF.COLOR.WORK);
            RunAdbCmd("adb shell am startservice -n com.amtran.factory/.FactoryService --es Action SetBurningWiFiConfig --es WiFi_SSID " + GlobalConfig.sBiWifiSsid +
                " --es WiFi_IPaddress " + GlobalConfig.sBiWifiGateway +
                " --ei WiFi_Timeout 40 --ei WiFi_RSSI_Threshold -64 --ei WiFi_Total_Test_Count 3 --ei WiFi_Delay_Time 8",
                "adb shell cat /sdcard/Download/command_ack.txt", out ack);
            if (ResultCheck(ack, @"SSID=" + GlobalConfig.sBiWifiSsid) == false)
            {
                SetMsg("Err:" + ack, UDF.COLOR.FAIL);
                SetMsg("Set Burning-WiFi config fail!", UDF.COLOR.FAIL);
                if (iRetryTime > 2)
                    return false;
                else
                {
                    iRetryTime++;
                    goto WifiConfig;
                }
            }

            return true;
        }

        bool Barra_SetBurnInOn()
        {
            string ack;
            int iRetryTime = new int();

            //if (Barra_AdbRoot() == false) return false;

            iRetryTime = 0;
        SetWifiOn:
            SetMsg("Set burning mode on...", UDF.COLOR.WORK);
            RunAdbCmd("adb shell am startservice -n com.amtran.factory/.FactoryService --es Action SetBurningMode --ez setMode 1",
                "adb shell cat /sdcard/Download/command_ack.txt", out ack);
            if (ResultCheck(ack, "BurningMode=1") == false)
            {
                SetMsg("Err:" + ack, UDF.COLOR.FAIL);
                SetMsg("Set burning mode on fail!", UDF.COLOR.FAIL);
                if (iRetryTime > 2)
                    return false;
                else
                {
                    iRetryTime++;
                    goto SetWifiOn;
                }
            }

            return true;
        }

        bool UpdateBiTime()
        {
            string sql = string.Empty;
            bool bErr = false;
            SqlDataReader rdr;

            //SDriverX.g_modelInfo = new UDF.ModelInfo { sSn = "55ME00123456789" }; //DEBUG

            SetMsg("insert burning time...", UDF.COLOR.WORK);

            sql = string.Format("select * from BI_TIME where trid = '{0}'", SDriverX.g_modelInfo.sSn);
            bErr = Database.Sql.ServerExecute(sql, out rdr);
            if (bErr)
            {
                rdr.Read();
                if (rdr.HasRows)
                {
                    //已有记录
                    if (rdr.IsClosed == false) rdr.Close();
                    sql = string.Format("update BI_TIME  set bi_time = '{0}' where trid = '{1}'",
                        DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"),
                        SDriverX.g_modelInfo.sSn);
                    if (Database.Sql.UpdateServer(sql) == false)
                    {
                        SetMsg("Update bi time fail!数据库操作失败(update...)", UDF.COLOR.FAIL);
                        return false;
                    }
                }
                else
                {
                    //无记录
                    if (rdr.IsClosed == false) rdr.Close();
                    sql = string.Format("insert into  BI_TIME (trid, bi_time) values('{0}','{1}')",
                            SDriverX.g_modelInfo.sSn,
                            DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
                    if (Sql.UpdateServer(sql) == false)
                    {
                        SetMsg("Update bi time fail!数据库操作失败(insert...)", UDF.COLOR.FAIL);
                        return false;
                    }
                }
            }
            else
            {
                SetMsg("Update bi time fail!数据库操作失败(query...)", UDF.COLOR.FAIL);
                return false;
            }

            return true;
        }

        bool WriteExtSn()
        {
            string ack, ack2, sql;
            int iRetryTime = new int();
            bool bErr = new bool();
            int flag = new int();
            OleDbDataReader reader;


            SetMsg("获取特别版SN...", UDF.COLOR.WORK);

            //查询数据库
            SetMsg("正在从数据库查询已绑定的特别版SN...", UDF.COLOR.WORK);
            //先查询是否已申请
            sql = string.Format("select lcm_lv from rknmgr.insp_neatframe_data where trid = '{0}'", SDriverX.g_modelInfo.sSn);
            //bErr = Oracle.ServerExecute(sql, out reader);
            //if (bErr)
            //{
            //    reader.Read();
            //    if (reader.HasRows)
            //    {
            //        //已有数据
            //        if (reader[0].ToString().Trim() == string.Empty)
            //        {
            //            //为空
            //            flag = 3;
            //        }
            //        else
            //        {
            //            flag = 1;
            //            SDriverX.g_modelInfo.sExtSn = reader[0].ToString().ToUpper();
            //        }
            //    }
            //    else
            //    {
            //        //没有数据
            //        flag = 2;
            //    }
            //    reader.Close();
            //}
            //else
            //{
            //    SetMsg("数据库操作失败(Query)", UDF.COLOR.FAIL);
            //    return false;
            //}

            iRetryTime = 0;
        GetExtSn:
            SetMsg("请输入特别版SN", UDF.COLOR.WORK);
            if (flag == 2 || flag == 3)
            {
                InputSn inputSn = new InputSn();
                DialogResult result = inputSn.ShowDialog(this);
                if (result == DialogResult.OK)
                {
                    if (inputSn.sSn.Length > 3)
                    {
                        SetMsg("未输入正确的特别版SN", UDF.COLOR.FAIL);
                        if (iRetryTime > 1)
                            return false;
                        else
                        {
                            iRetryTime++;
                            goto GetExtSn;
                        }
                    }
                    else
                    {
                        SDriverX.g_modelInfo.sExtSn = inputSn.sSn;
                    }
                }
                else
                {
                    SetMsg("未输入正确的特别版SN", UDF.COLOR.FAIL);
                    if (iRetryTime > 1)
                        return false;
                    else
                    {
                        iRetryTime++;
                        goto GetExtSn;
                    }
                }

                //上传网络信息
                if (flag == 2)
                {
                    sql = string.Format("insert into rknmgr.insp_neatframe_data (trid, lcm_lv, datein) values('{0}', '{1}', sysdate)",
                            SDriverX.g_modelInfo.sSn,
                            SDriverX.g_modelInfo.sExtSn);
                    //bErr = Oracle.UpdateServer(sql);
                    //if (!bErr)
                    //{
                    //    SetMsg("数据库操作失败,插入数据库表失败", UDF.COLOR.FAIL);
                    //    return false;
                    //}
                }
                else if (flag == 3)
                {
                    sql = string.Format("update rknmgr.insp_neatframe_data set lcm_lv = '{0}', datein = sysdate where trid = '{1}'",
                            SDriverX.g_modelInfo.sExtSn,
                            SDriverX.g_modelInfo.sSn);
                    //bErr = Oracle.UpdateServer(sql);
                    //if (!bErr)
                    //{
                    //    SetMsg("数据库操作失败,更新数据库表失败(update)", UDF.COLOR.FAIL);
                    //    return false;
                    //}
                }

                //较验信息
                Delay(1000);
                //检验数据库信息
                sql = string.Format("select lcm_lv from rknmgr.insp_neatframe_data where trid = '{0}'", SDriverX.g_modelInfo.sSn);
                //bErr = Oracle.ServerExecute(sql, out reader);
                //if (bErr)
                //{
                //    reader.Read();
                //    if (reader.HasRows)
                //    {
                //        //有数据
                //        if (reader[0].ToString().ToUpper() != SDriverX.g_modelInfo.sExtSn)
                //        {
                //            SetMsg(string.Format("数据库信息较验证错误,{0}/{1}", SDriverX.g_modelInfo.sExtSn, reader[0].ToString().ToUpper()), UDF.COLOR.FAIL);
                //            return false;
                //        }
                //    }
                //    else
                //    {
                //        //没有数据
                //        SetMsg("数据库信息更新失败(Inset)", UDF.COLOR.FAIL);
                //        return false;
                //    }
                //    reader.Close();
                //}
                //else
                //{
                //    SetMsg("数据库操作失败(校验)", UDF.COLOR.FAIL);
                //    return false;
                //}
            }

            iRetryTime = 0;
        WriteExtSn:
            SetMsg("写入特别版SN", UDF.COLOR.WORK);
            RunAdbCmd("adb shell su root nf_persist -s -k edition -v launch_" + SDriverX.g_modelInfo.sExtSn,
                "adb shell su root nf_persist edition", out ack);
            if (ResultPick(ack, @"(?<=edition=launch_).*?(?=\r|\n)", out ack2) == false)
            {
                SetMsg("Err:" + ack, UDF.COLOR.FAIL);
                SetMsg("Read edition fail!", UDF.COLOR.FAIL);
                if (iRetryTime > 2)
                    return false;
                else
                {
                    iRetryTime++;
                    goto WriteExtSn;
                }
            }

            return true;
        }

        bool SwitchMacStyle(string macSource, UDF.MAC_STYLE mac_style, out string macTarget)
        {
            macTarget = string.Empty;
            string target = string.Empty;
            string[] MAC = { string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty };

            if (ResultPick(macSource, @"([a-z A-Z 0-9]{2}([\x20:])?){5}[a-z A-Z 0-9]{2}", out target) == true)
            {
                target = target.Replace(" ", "");
                target = target.Replace(":", "");
                target = target.Trim().ToUpper();
                if (target.Length != 12)
                {
                    SetMsg("MAC匹配错误，RAW MAC:" + macSource, UDF.COLOR.FAIL);
                    return false;
                }
                else
                {
                    MAC[0] = target.Substring(0, 2);
                    MAC[1] = target.Substring(2, 2);
                    MAC[2] = target.Substring(4, 2);
                    MAC[3] = target.Substring(6, 2);
                    MAC[4] = target.Substring(8, 2);
                    MAC[5] = target.Substring(10, 2);
                }
            }
            else
            {
                SetMsg("MAC匹配错误，RAW MAC:" + macSource, UDF.COLOR.FAIL);
                return false;
            }

            switch (mac_style)
            {
                case UDF.MAC_STYLE.U12:
                    macTarget = target;
                    break;
                case UDF.MAC_STYLE.L12:
                    macTarget = target.ToLower();
                    break;
                case UDF.MAC_STYLE.UC17:
                    macTarget = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", MAC[0], MAC[1], MAC[2], MAC[3], MAC[4], MAC[5]);
                    break;
                case UDF.MAC_STYLE.LC17:
                    macTarget = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", MAC[0], MAC[1], MAC[2], MAC[3], MAC[4], MAC[5]).ToLower();
                    break;
                case UDF.MAC_STYLE.UB17:
                    macTarget = string.Format("{0} {1} {2} {3} {4} {5}", MAC[0], MAC[1], MAC[2], MAC[3], MAC[4], MAC[5]);
                    break;
                case UDF.MAC_STYLE.LB17:
                    macTarget = string.Format("{0} {1} {2} {3} {4} {5}", MAC[0], MAC[1], MAC[2], MAC[3], MAC[4], MAC[5]).ToLower();
                    break;
                default:
                    macTarget = target;
                    break;
            }
            return true;
        }

        bool QueryMI_Model()
        {
            string Mitype = string.Empty; 
            SqlDataReader rdr = null;
            Sql.DefaultDatabase = Sql.DatabaseOpt.RAKEN_SFIS;
            SetMsg("正在从数据库查询MI MODEL信息...", UDF.COLOR.WORK);
            //this.label8.Text  = "MI_Model";
            Sql.ServerExecute("select ULPK from keyspec where ssn= '" + SDriverX.g_modelInfo.sPart_no + "'", out rdr);
            rdr.Read();
            if (rdr.HasRows)
            {
                Mitype = rdr[0].ToString();
                rdr.Close();

            }
            else
            {
                //rdr.Close();
                Sql.DefaultDatabase = Sql.DatabaseOpt.RAKEN_TE;
                MessageBox.Show(null, "在查询数据存储的 FW 信息途中失败，没有查询到对应的 ProgID", "查询数据失败");
                return false;
            }
            Mitype = Mitype.Trim();
            this.textBoxModel.Text = Mitype;
            this.label7.Text = "PROGID:";
            switch (Mitype)
            {
                // 以下MODEL为小米XM系列
                case "XM_RZ2L":
                    g_bXM_RZ2L=true ;
                    break;
                case "XM_RZ3L":
                    g_bXM_RZ3L = true;
                    break;
                case "XM_RZ4L":
                    g_bXM_RZ4L = true;
                    break;
                case "XM_RZ5L":
                    g_bXM_RZ5L = true;
                    break;
                case "XM_RZ6L":
                    g_bXM_RZ6L = true;
                    break;
                case "XM_RZ7L":
                    g_bXM_RZ7L = true;
                    break;
                case "XM_RZ8L":
                    g_bXM_RZ8L = true;
                    break;
                case "XM_RZ9L":
                    g_bXM_RZ9L = true;
                    break;
                case "XM_RZAL":
                    g_bXM_RZAL = true;
                    break;
                case "XM_RZBL":
                    g_bXM_RZBL = true;
                    break;
                case "XM_RZCL":
                    g_bXM_RZCL = true;
                    break;
                // 以下MODEL为小米ES系列
                case "XM_DZ3L":
                    g_bXM_DZ3L = true;
                    break;
                case "XM_DZ4L":
                    g_bXM_DZ4L = true;
                    break;
                case "XM_DZ9L":
                    g_bXM_DZ9L = true;
                    break;
                case "XM_DZCL":
                    g_bXM_DZCL = true;
                    break;
                case "XM_DZDL":
                    g_bXM_DZDL = true;
                    break;
                case "XM_DZFL":
                    g_bXM_DZFL = true;
                    break;
                case "XM_DZGL":
                    g_bXM_DZGL = true;
                    break;
                // 以下MODEL为小米EA系列
                case "XM_DZ5L":
                    g_bXM_DZ5L = true;
                    break;
                case "XM_DZJL":
                    g_bXM_DZJL = true;
                    break;
                case "XM_DZ7L":
                    g_bXM_DZ7L = true;
                    break;
                case "XM_DZ8L":
                    g_bXM_DZ8L = true;
                    break;
                case "XM_DZEL":
                    g_bXM_DZEL = true;
                    break;
                case "XM_DZHL":
                    g_bXM_DZHL = true;
                    break;
                case "XM_DZIL":
                    g_bXM_DZIL = true;
                    break;
                // 以下MODEL为小米 W系列
                case "XM_WZ2L":
                    g_bXM_WZ2L = true;
                    break;
                case "XM_WZ3L":
                    g_bXM_WZ3L = true;
                    break;
                case "XM_WZ4L":
                    g_bXM_WZ4L = true;
                    break;
                case "XM_WZ5L":
                    g_bXM_WZ5L = true;
                    break;
                case "XM_WZ6L":
                    g_bXM_WZ6L = true;
                    break;
                case "XM_WZ7L":
                    g_bXM_WZ7L = true;
                    break;
                //// 以下MODEL为小米 A75系列
                case "XM_DZLL":
                    g_bXM_DZLL = true;
                    break;
                case "XM_DZKL":
                    g_bXM_DZKL = true;
                    break;
                case "XM_DZNL":
                    g_bXM_DZNL = true;
                    break;
                case "XM_DZML":
                    g_bXM_DZML = true;
                    break;
                case "XM_DZQL":
                    g_bXM_DZQL = true;
                    break;
                default:
                    SetMsg("MI MODEL信息程式上未设定，请联系TE...", UDF.COLOR.FAIL );
                    //break;
                    return false; 
            }
            return true;
        }

        bool QueryProgIdSql(out string progid)
        {
            progid = string.Empty;
            SqlDataReader rdr = null;
            Sql.DefaultDatabase = Sql.DatabaseOpt.RAKEN_SFIS;
            Sql.ServerExecute("select progid from RAKEN_SFIS..ModelList where pn = '" + SDriverX.g_modelInfo.sPart_no + "' and ForProcess =  'WBC' and (forline = 'ALL' or forline = '') order by forline desc", out rdr);
            rdr.Read();
            if (rdr.HasRows)
            {
                progid = rdr[0].ToString();
                rdr.Close();

            }
            else
            {
                //rdr.Close();
                Sql.DefaultDatabase = Sql.DatabaseOpt.RAKEN_TE;
                MessageBox.Show(null, "在查询数据存储的 FW 信息途中失败，没有查询到对应的 ProgID", "查询数据失败");
                return false;
            }
            return true;
        }

        bool CheckFwVerSql(string progid, string fw)
        {
            string fw1 = string.Empty;
            string fw2 = string.Empty;
            SqlDataReader rdr = null;
            Sql.DefaultDatabase = Sql.DatabaseOpt.RAKEN_SFIS;
            Sql.ServerExecute("select paravalue from RAKEN_SFIS..ProgParaDetail where progid='" + progid + "' and cmdid='65' and (paraid='3' OR paraid='4')", out rdr);
            rdr.Read();
            if (rdr[0].ToString() == string.Empty)
            {
                SetMsg("警告：数据库 FW_1 为空", UDF.COLOR.WORK);
            }
            else
            {
                fw1 = rdr[0].ToString().Replace(" ", "");
                g_sFwVer1 = fw1;
            }
            rdr.Read();
            if (rdr[0].ToString() == string.Empty)
            {
                SetMsg("警告：数据库 FW_2 为空", UDF.COLOR.WORK);
            }
            else
            {
                fw2 = rdr[0].ToString().Replace(" ", "");
                g_sFwVer2 = fw2;
            }
            rdr.Close();
            Sql.DefaultDatabase = Sql.DatabaseOpt.RAKEN_TE;
            if (fw1 == fw2 && fw1 == string.Empty)
            {
                MessageBox.Show(null, "未查询到数据库存储的 FW 信息", "查询数据失败", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            g_iPtInitFlag = INIT_FW;
            //this.textBoxUn.Text = g_sFwVer1 + @"/" + g_sFwVer2;
            if ((fw.IndexOf(fw1) == -1) && (fw.IndexOf(fw2) == -1))
            {
                SetMsg("FW比对失败！机台FW:" + fw + "\n数据库FW:" + fw1 + "/" + fw2, UDF.COLOR.FAIL);
                return false;
            }
            else
            {
                //if (SDriverX.g_modelInfo.sPart_no.Substring(5, 2).ToUpper() == "BB")
                //{
                //    string tempFW = "";
                //    //去数字
                //    //var result1 = Regex.Replace(fw, @"[^0-9]+", "");
                //    //string startstr = "aml-nxtgen_";
                //    string startstr = "sprint_";
                //    string endstr = "sdy";
                //    string result = string.Empty;
                //    int startindex, endindex;
                //    try
                //    {
                //        startindex = fw.IndexOf(startstr);
                //        if (startindex == -1)
                //        {
                //            tempFW = result;
                //        }
                //        string tmpstr = fw.Substring(startindex + startstr.Length);
                //        endindex = tmpstr.IndexOf(endstr);
                //        if (endindex == -1)
                //        {
                //            tempFW = result;
                //        }
                //        tempFW = tmpstr.Remove(endindex);
                //    }
                //    catch (Exception ex)
                //    {
                //        MessageBox.Show(null, "fail message", "FAIL" + ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //    }
                //    fw = tempFW;

                //}
                Insert_FW_Info(fw);
            }
            return true;
        }
        bool CheckTAGSql(string progid, string r_tag)
        {
            string tag = string.Empty;
            SqlDataReader rdr = null;
            Sql.DefaultDatabase = Sql.DatabaseOpt.RAKEN_SFIS;
            Sql.ServerExecute("select paravalue from RAKEN_SFIS..ProgParaDetail where progid='" + progid + "' and cmdid='142' and paraid='3'" , out rdr);
            rdr.Read();
            if (rdr[0].ToString() == string.Empty)
            {
                SetMsg("警告：数据库 FW_1 为空", UDF.COLOR.WORK);
            }
            else
            {
                tag = rdr[0].ToString().Replace(" ", "");
                g_sTAG = tag;
            }
            rdr.Close();
            Sql.DefaultDatabase = Sql.DatabaseOpt.RAKEN_TE;
            if (tag == string.Empty)
            {
                MessageBox.Show(null, "未查询到数据库存储的 FW 信息", "查询数据失败", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            this.label8.Text = "TAG:";
            this.textBoxUn.Text = g_sTAG;
            if (r_tag.IndexOf(tag) == -1)
            {
                SetMsg("TAG比对失败！读取机台TAG:" + r_tag + "\n数据库TAG设定:" + tag , UDF.COLOR.FAIL);
                strOKNG = "NG";
                Insert_ORALCEdatabase(SDriverX.g_modelInfo.sPart_no + "_tag:" + r_tag);
                return false;
            }
            return true;
        }
        bool SetModelIndexForCardhu()
        {
            string tmp = null;

            SetMsg("Set model index...", UDF.COLOR.WORK);

            if (SDriver.SDriverX.g_modelInfo.sPart_no.Length < 12)
            {
                SetMsg("Set model index fail (query pn fail)", UDF.COLOR.FAIL);
                return false;
            }
            else if (SDriver.SDriverX.g_modelInfo.sPart_no.Length == 12)
            {
                if (SDriver.SDriverX.g_modelInfo.sPart_no == "356506020300")
                {
                    RunAdbCmd("adb shell su root i2cset -f -y 9 0x37 0x51 0x86 0x4F 0x63 0x00 0x17 0x00 0x02 0x80 i", out tmp, 1);

                }
                else
                {
                    SetMsg("料号未维护请通知TE/PE", UDF.COLOR.FAIL);
                    return false;
                }
            }
            else
            {
                try
                {
                    string type = SDriver.SDriverX.g_modelInfo.sPart_no.Substring(9, 1);
                    string type2 = SDriver.SDriverX.g_modelInfo.sPart_no.Substring(13, 7);
                    string ack = "";

                    if (type == "E")
                    {
                        if (type2 == "CARDHU1" || type2 == "CARDHU2")
                            RunAdbCmd("adb shell su root i2cset -f -y 9 0x37 0x51 0x86 0x4F 0x63 0x00 0x17 0x00 0x00 0x82 i", out ack, 1);
                        else if (type2 == "CARDHU3" || type2 == "CARDHU4" || type2 == "CARDHU6" || type2 == "CARDHU5" || type2 == "CARDHU8" || type2 == "CARDHUC" || type2 == "CARDHUB" || type2 == "CARDHU7" || type2 == "CARDHU5")
                            RunAdbCmd("adb shell su root i2cset -f -y 9 0x37 0x51 0x86 0x4F 0x63 0x00 0x17 0x00 0x01 0x83 i", out ack, 1);
                        else
                        {
                            SetMsg("料号未维护，请联系TE/PE。", UDF.COLOR.FAIL);
                            return false;
                        }
                    }
                    else if (type == "A")
                    {
                        RunAdbCmd("adb shell su root i2cset -f -y 9 0x37 0x51 0x86 0x4F 0x63 0x00 0x17 0x00 0x02 0x80 i", out ack, 1);
                    }
                    else
                    {
                        SetMsg("料号未维护，请联系TE/PE。", UDF.COLOR.FAIL);
                        return false;
                    }
                }
                catch
                {
                    SetMsg("Set model index fail", UDF.COLOR.FAIL);
                    return false;
                }
            }

            return true;
        }

        bool GetScalerFwForCardhu()
        {
            string ack;
            //string tmp = null;
            string strFw = "", FLAG;
            FLAG = wbc.iniFile.GetPrivateStringValue("FW_PN", "flag", @"\\192.168.158.25\\te\\setup\\FTTEST\\FWVISON\\FW.ini");
            if (FLAG == "1")
            {
                strFw = wbc.iniFile.GetPrivateStringValue("FW_PN", SDriver.SDriverX.g_modelInfo.sPart_no, @"\\192.168.158.25\\te\\setup\\FTTEST\\FWVISON\\FW.ini");
                if (strFw != "0" || strFw != "")
                {
                    strFw = wbc.iniFile.GetPrivateStringValue("FW_PN", SDriver.SDriverX.g_modelInfo.sPart_no, @"\\192.168.158.25\\te\\setup\\FTTEST\\FWVISON\\FW.ini");
                }
                else
                {
                    SetMsg("\\192.168.158.25\\te\\setup\\FTTEST\\FWVISON\\FW.ini，服务器没有维护料号FW", UDF.COLOR.FAIL);
                    return false;
                }
            }
            else
            {
                FLAG = wbc.iniFile.GetPrivateStringValue("FW", "flag", @"\\192.168.158.25\\te\\setup\\FTTEST\\FWVISON\\FW.ini");
                if (FLAG == "1")
                {
                    strFw = wbc.iniFile.GetPrivateStringValue("FW", "FW", @"\\192.168.158.25\\te\\setup\\FTTEST\\FWVISON\\FW.ini");
                    if (strFw =="0"|| strFw =="")
                    {
                        SetMsg("\\192.168.158.25\\te\\setup\\FTTEST\\FWVISON\\FW.ini，服务器没有维护FW", UDF.COLOR.FAIL);
                        return false;
                    }
                }
                else
                {
                    SetMsg("\\192.168.158.25\\te\\setup\\FTTEST\\FWVISON\\FW.ini，服务器没有启动FW防呆；", UDF.COLOR.FAIL);
                    return false;

                }
            }
                SetMsg("Get scaler fw version...", UDF.COLOR.WORK);
                RunAdbCmd(@"adb shell su root cat /sys/devices/soc/c1b7000.i2c/i2c-9/9-0037/fw_version", out ack);
                if (ResultCheck(ack, strFw) == false)
                {
                    SetMsg("Check scaler fw fail!规格:" + strFw + " 读取值" + ack, UDF.COLOR.FAIL);
                    return false;
                }

                return true;
            }
        }
    
}

