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
using System.Threading;

namespace PRETEST.TestProcess.PRE
{
    class xm_hw
    {
    }
}
namespace PRETEST
{
    public partial class FormMain
    {

        bool PRE_XM_HW()
        {
            string strBuf = string.Empty;
            string strSN2 = string.Empty;
            iRetryTime = 0;
            SetMsg(DateTime.Now.ToString(), UDF.COLOR.WORK);    //DEBUG

            if (Check_Abnormal_Restart() == false) return false;
            if (Check_MI_FW() == false) return false;
            if (Check_MI_TAG() == false) return false;
            if (Check_SetPattern_ON() == false) return false;
            if (Write_mi_tvmn() == false) return false;
            if (Write_mi_tvsn() == false) return false;
            if (Write_mi_eth_mac() == false) return false;
            if (Write_mi_didkey() == false) return false;
            if (Check_widevine_key_size() == false) return false;
            if (Check_hdcpkey_size() == false) return false;
            if (Check_netflix_key_verify() == false) return false;
            if (Check_playready_key_verify() == false) return false;
            if (Check_attestation_key_verify() == false) return false;
            
            if (Check_Panel_test() == false) return false;
            //if (Check_SetPatternON()==false) return false;
            if (Check_emmc_Lefttime() == false) return false;
            
            //if (g_bAutoScan == true)
            //{
            //    ReSCAN2:
            //    SetMsg("Second barcode information verification...", UDF.COLOR.WORK);
            //    if (Sick_scan_again(out strSN2) == false)
            //    {
            //        goto ReSCAN2;
            //    }
            //    else
            //    {
            //        if (strSN2 != SDriverX.g_modelInfo.sSn)
            //        {
            //            return false;
            //        }
            //    }
            //}
            if (Check_SetBurnInOn() == false) return false;
            
            Thread.Sleep(4000);
            if (SET_BT_STATUS_OFF() == false) return false;
            if (SET_BT_STATUS_OFF() == false) return false;
            SetMsg(DateTime.Now.ToString(), UDF.COLOR.WORK);    //DEBUG
            return true;
        }

        bool SetBurnInOn()
        {
            iRetryTime = 0;
            ReBIMODEON:
            SetMsg("SET AGING MODE ON....", UDF.COLOR.WORK);
            StrToCmd("79 00 14 80 00 00 01");
            g_bRTACKFlag = true;
            if (WriteRS232CommandMI(g_byCmdBuf, 9, 20, 19, out g_byCmdRtn) == false)
            {
                SetMsg("SET BI MODE ON CMD fail", UDF.COLOR.FAIL);
                if (iRetryTime > 2)
                    return false;
                else
                {
                    Thread.Sleep(200);
                    iRetryTime++;
                    goto ReBIMODEON;
                }
            }
            return true;
        }

        public Boolean SET_BT_STATUS_OFF()
        {
            int reback = 0;
            StrToCmd("79 00 11 2E 01 31 00 01");  //79 00 11 2E 01 30 00 01
            g_bRTACKFlag = true;
            BTSET:
            SetMsg("SET CLOSE BT...", UDF.COLOR.WORK);
            if (WriteRS232CommandMI(g_byCmdBuf, 10, 20, 19, out g_byCmdRtn) == false)
            {
                if (reback < 3)
                {
                    reback++;
                    goto BTSET;
                }
                else
                {
                    SetMsg("SET CLOSE BT CMD Fail!", UDF.COLOR.FAIL);
                    return false;
                }
            }
            else
            {
                Delay(1000);
                StrToCmd("79 00 14 1C 00 00 01");  //79 00 11 2E 01 30 00 01
                g_bRTACKFlag =false;
                BTGET:
                SetMsg("GET BT STATUS...", UDF.COLOR.WORK);
                if (WriteRS232CommandMI(g_byCmdBuf, 9, 20, 19, out g_byCmdRtn) == false)
                {
                    if (reback < 3)
                    {
                        reback++;
                        goto BTGET;
                    }
                    else
                    {
                        SetMsg("GET CLOSE BT STATUS CMD Fail!", UDF.COLOR.FAIL);
                        return false;
                    }
                }
                else
                {
                    if (g_byCmdRtn[13] != 1)
                    {
                        SetMsg("GET CLOSE BT STATUS CMD fail", UDF.COLOR.FAIL);
                        if (iRetryTime > 3)
                            return false;
                        else
                        {
                            iRetryTime++;
                            goto BTGET;
                        }
                    }
                    string strBuf = string.Empty;
                    for (int i = 0; i < 4; i++)
                    {
                        strBuf = strBuf + " " + g_byCmdRtn[i + 12];
                    }
                    SetMsg("GET BT STATUS ACK " + strBuf, UDF.COLOR.WORK);
                    if (g_byCmdRtn[14]!=1)
                    {
                        SetMsg("GET CLOSE BT STATUS CMD fail", UDF.COLOR.FAIL);
                        if (iRetryTime > 2)
                            return false;
                        else
                        {
                            iRetryTime++;
                            goto BTSET;
                        }
                    }
                }
            }
            return true;
        }
        bool SetBurnInOff()
        {
            iRetryTime = 0;
            ReBIMODEON:
            SetMsg("SET AGING MODE OFF....", UDF.COLOR.WORK);
            StrToCmd("79 00 14 81 00 00 01");
            g_bRTACKFlag = true;
            if (WriteRS232CommandMI(g_byCmdBuf, 9, 20, 19, out g_byCmdRtn) == false)
            {
                SetMsg("SET BI MODE ON CMD fail", UDF.COLOR.FAIL);
                if (iRetryTime > 2)
                    return false;
                else
                {
                    Thread.Sleep(200);
                    iRetryTime++;
                    goto ReBIMODEON;
                }
            }
            return true;
        }
        bool Check_netflix_key_verify()
        {
            iRetryTime = 0;
            Reverify:
            SetMsg("Check netflix key verify....", UDF.COLOR.WORK);
            StrToCmd("79 00 12 80 00 00 01");
            g_bRTACKFlag = true;
            if (WriteRS232CommandMI(g_byCmdBuf, 9, 20, 19, out g_byCmdRtn) == false)
            {
                SetMsg("netflix key verify ack fail", UDF.COLOR.FAIL);
                if (iRetryTime > 2)
                    return false;
                else
                {
                    Thread.Sleep(200);
                    iRetryTime++;
                    goto Reverify;
                }
            }
            return true;
        }

        bool Check_playready_key_verify()
        {
            iRetryTime = 0;
            Reverify:
            SetMsg("Check playready key verify....", UDF.COLOR.WORK);
            StrToCmd("79 00 14 17 00 00 01");
            g_bRTACKFlag = true;
            if (WriteRS232CommandMI(g_byCmdBuf, 9, 20, 19, out g_byCmdRtn) == false)
            {
                SetMsg("playready key verify ack fail", UDF.COLOR.FAIL);
                if (iRetryTime > 2)
                    return false;
                else
                {
                    Thread.Sleep(200);
                    iRetryTime++;
                    goto Reverify;
                }
            }
            return true;
        }

        bool Check_attestation_key_verify()
        {
            iRetryTime = 0;
            Reverify:
            SetMsg("Check attestation key verify....", UDF.COLOR.WORK);
            StrToCmd("79 00 14 19 00 00 01");
            g_bRTACKFlag = true;
            if (WriteRS232CommandMI(g_byCmdBuf, 9, 20, 19, out g_byCmdRtn) == false)
            {
                SetMsg("attestation key verify ack fail", UDF.COLOR.FAIL);
                if (iRetryTime > 2)
                    return false;
                else
                {
                    Thread.Sleep(200);
                    iRetryTime++;
                    goto Reverify;
                }
            }
            return true;
        }
    }
}