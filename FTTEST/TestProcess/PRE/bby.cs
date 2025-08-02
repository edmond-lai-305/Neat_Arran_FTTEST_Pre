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
    class bby
    {
    }
}
namespace PRETEST
{
    public partial class FormMain
    {

        bool PRE_BBY()
        {
            string strBuf = string.Empty;
            string strSN2 = string.Empty;
            iRetryTime = 0;
            SetMsg(DateTime.Now.ToString(), UDF.COLOR.WORK);    //DEBUG

            if (Check_BBY_FW() == false) return false;
            if (get_device_id() == false) return false;
            if (Check_bby_projectid() == false) return false;
            if (Check_bby_modelname() == false) return false;
            if (set_bby_sn() == false) return false;
            if (check_bby_sn() == false) return false;
            if (get_bbby_eth_mac() == false) return false;
            if (get_bbby_wifi_mac() == false) return false;
            if (get_bbby_bt_mac() == false) return false;
            if (set_clear_keyflag() == false) return false;
            //if (g_bAutoScan == true)
            //{
            //    ReSCAN2:
            //    SetMsg("DOUBLE CHECK BARCODE SN...", UDF.COLOR.WORK);
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
            if (set_bby_aging_off() == false) return false;
            if (set_bby_aging_on() == false) return false;
            SetMsg(DateTime.Now.ToString(), UDF.COLOR.WORK);    //DEBUG
            return true;
        }

        bool Check_BBY_FW()
        {
            //string progid;
            //Delay(100);
            ReFW:
            SetMsg("Check BBY FW Version", UDF.COLOR.WORK);
            StrTo_bby_Cmd("FF 33 06 03 12");
            g_bRTACKFlag = false; //SET true 指令判断回传值;GET false 指令不判断回传值
            strFW = string.Empty;
            //if (WriteRS232CommandBBY(g_byCmdBuf, 6, 50, 45, out g_byCmdRtn) == false)
            if (WriteRS232CommandBBY(g_byCmdBuf, 6, 50, 35, out g_byCmdRtn) == false)
            {
                if (iRetryTime > 2)
                {
                    SetMsg("Check FW Version CMD fail", UDF.COLOR.FAIL);
                    return false;
                }
                else
                {
                    iRetryTime++;
                    goto ReFW;
                }
            }
            else
            {
                if (g_byCmdRtn[2] < 34)
                {
                    SetMsg("get FW Version Length fail", UDF.COLOR.FAIL);
                    if (iRetryTime > 2)
                        return false;
                    else
                    {
                        iRetryTime++;
                        goto ReFW;
                    }
                }
                else
                {
                    for (i = 0; i < g_byCmdRtn[2]-6; i++)
                    {
                        strFW = strFW + Convert.ToChar(g_byCmdRtn[i + 5]);
                    }
                }
                //this.label8.Text = "FW:";
                this.textBoxFW.Text = strFW;
                if ((g_iPtInitFlag & INIT_FW) == INIT_FW)  //如果已经有了首次查询了机种FW信息，则比较缓存数据不再联网查询
                {
                    if ((strFW.IndexOf(g_sFwVer1) == -1) && (strFW.IndexOf(g_sFwVer2) == -1))   //机台 fw 与 数据库 fw1、fw2 均比较错误
                    {
                        //FW 比对失败
                        SetMsg("FW check fail！tv FW:" + strFW + "\n setting FW:" + g_sFwVer1 + "/" + g_sFwVer2, UDF.COLOR.FAIL);
                        return false;
                    }
                    else
                    {
                        SetMsg("get tv version： " + strFW, UDF.COLOR.WORK);
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
                        //        startindex = strFW.IndexOf(startstr);
                        //        if (startindex == -1)
                        //        {
                        //            tempFW = result;
                        //        }
                        //        string tmpstr = strFW.Substring(startindex + startstr.Length);
                        //        endindex = tmpstr.IndexOf(endstr);
                        //        if (endindex == -1)
                        //        {
                        //            tempFW = result;
                        //        }
                        //        tempFW = tmpstr.Remove(endindex);
                        //    }
                        //    catch (Exception ex)
                        //    {
                        //        MessageBox.Show(null, "错误信息", "失败" + ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        //    }
                        //    strFW = tempFW;

                        //}
                        Insert_FW_Info(strFW);
                    }
                }
                else //否则从数据库更新 FW信息，下一次不再联网查询，除非切换了新工单
                {
                    if (QueryProgIdSql(out strProgID) == false)
                    {
                        SetMsg("查询机种 progid fail", UDF.COLOR.FAIL);
                        return false;
                    }
                    else
                    {
                        //g_iPtInitFlag = INIT_ZERO;  //工单切换后，这个 flag 会重置为 0，以通知其他部分更新信息，比如从数据库更新此工单应该卡的 FW 信息
                        this.textBoxRC_SN.Text = strProgID;
                        if (CheckFwVerSql(strProgID, strFW) == false) return false;
                    }
                }
            }
            return true;
        }
        bool Check_bby_modelname()
        {
            ReFW:
            SetMsg("Check bby model name...", UDF.COLOR.WORK);
            StrTo_bby_Cmd("FF 33 06 03 DD");
            string strModel = string.Empty;
            g_bRTACKFlag = false; //SET true 指令判断回传值;GET false 指令不判断回传值
            if (WriteRS232CommandBBY(g_byCmdBuf, 6, 10, 18, out g_byCmdRtn) == false)
            {
                if (iRetryTime > 2)
                {
                    SetMsg("Check bby model CMD fail", UDF.COLOR.FAIL);
                    return false;
                }
                else
                {
                    iRetryTime++;
                    goto ReFW;
                }
            }
            else
            {
                if (g_byCmdRtn[2] < 8)
                {
                    SetMsg("Check bby model Length fail", UDF.COLOR.FAIL);
                    if (iRetryTime > 2)
                        return false;
                    else
                    {
                        iRetryTime++;
                        goto ReFW;
                    }
                }
                else
                {
                    for (i = 0; i < g_byCmdRtn[2] - 6; i++)
                    {
                        strModel = strModel + Convert.ToChar(g_byCmdRtn[i + 5]);
                    }
                }
                SetMsg("get model name: " + strModel, UDF.COLOR.WORK);
                if (SDriverX.g_modelInfo.sPart_no.Substring(0, 3).ToUpper() == "T40")
                {
                    if (strModel != "PN40-551-24U")
                    {
                        SetMsg("get model name fail: " + strModel, UDF.COLOR.FAIL);
                        return false;
                    }
                }
                else if (SDriverX.g_modelInfo.sPart_no.Substring(0, 3).ToUpper() == "T24")
                {
                    if (strModel != "PN24-551-24U")
                    {
                        SetMsg("get model name fail: " + strModel, UDF.COLOR.FAIL);
                        return false;
                    }
                }
                else
                {
                    SetMsg("this pn model name not define: " + SDriverX.g_modelInfo.sPart_no, UDF.COLOR.FAIL);
                    return false;
                }

            }
            return true;
        }
        bool Check_bby_projectid()
        {
            ReFW:
            SetMsg("Check bby projectid...", UDF.COLOR.WORK);
            StrTo_bby_Cmd("FF 33 06 03 62");  //FF 33 06 03 62
            g_bRTACKFlag = false; //SET true 指令判断回传值;GET false 指令不判断回传值
            strTAG = string.Empty;
            if (WriteRS232CommandBBY(g_byCmdBuf, 6, 10, 10, out g_byCmdRtn) == false)
            {
                if (iRetryTime > 2)
                {
                    SetMsg("get projectid CMD fail", UDF.COLOR.FAIL);
                    return false;
                }
                else
                {
                    iRetryTime++;
                    goto ReFW;
                }
            }
            else
            {
                if (g_byCmdRtn[2] != 10)
                {
                    SetMsg("get projectid Length fail", UDF.COLOR.FAIL);
                    if (iRetryTime > 2)
                        return false;
                    else
                    {
                        iRetryTime++;
                        goto ReFW;
                    }
                }
                else
                {
                    for (i = 0; i < g_byCmdRtn[2] - 6; i++)
                    {
                        strTAG = strTAG + Convert.ToChar(g_byCmdRtn[i + 5]);
                    }
                }
                strOKNG = "NA";
                Insert_ORALCEdatabase("pid: " + strTAG);
            }
            return true;
        }

        bool set_bby_sn()
        {
            iRetryTime = 0;
            string strBuf = string.Empty;
            char[] chr = new char[1];
            ReWriteMISn:
            strBuf = "";
            SetMsg("set bby tv sn: " + SDriverX.g_modelInfo.sSn, UDF.COLOR.WORK);
            strBuf = "FF 33 16 03 5C 00";   //FF 33 0C 03 A1 31 32 33 34 35 36 
            for (int i = 0; i < 15; i++)
            {
                chr = SDriverX.g_modelInfo.sSn.Substring(i, 1).ToCharArray(0, 1);
                strBuf += string.Format(" {0:X02}", (byte)chr[0]);
            }
            StrTo_bby_Cmd(strBuf);
            g_bRTACKFlag = true;
            if (WriteRS232CommandBBY(g_byCmdBuf, 22, 10, 12, out g_byCmdRtn) == false)
            {
                SetMsg("set tv sn CMD fail", UDF.COLOR.FAIL);
                if (iRetryTime > 2)
                    return false;
                else
                {
                    Thread.Sleep(200);
                    iRetryTime++;
                    goto ReWriteMISn;
                }
            }
            return true;
        }

        bool check_bby_sn()
        {
            iRetryTime = 0;
            string strBuf = string.Empty;
            char[] chr = new char[1];
            iRetryTime = 0;
            ReReadMISn:
            SetMsg("check bby tv sn:" + SDriverX.g_modelInfo.sSn, UDF.COLOR.WORK);
            StrTo_bby_Cmd("FF 33 07 03 5D 00");//FF 33 06 03 A2
            g_bRTACKFlag = false;
            strBuf = string.Empty;
            if (WriteRS232CommandBBY(g_byCmdBuf, 7, 20, 22, out g_byCmdRtn) == false)
            {

                if (iRetryTime > 3)
                {
                    SetMsg("Read mi TVSN CMD fail", UDF.COLOR.FAIL);
                    return false;
                }
                else
                {
                    iRetryTime++;
                    goto ReReadMISn;
                }
            }
            else
            {
                if (g_byCmdRtn[2] != 22)
                {
                    
                    if (iRetryTime > 3)
                    {
                        SetMsg("Read bby tvsn length fail", UDF.COLOR.FAIL);
                        return false;
                    }                      
                    else
                    {
                        Thread.Sleep(200);
                        iRetryTime++;
                        goto ReReadMISn;
                    }
                }
                for (i = 0; i < 15; i++)
                {
                    strBuf = strBuf + Convert.ToChar(g_byCmdRtn[i + 6]);
                }
                if (strBuf != SDriverX.g_modelInfo.sSn)
                {
                    if (iRetryTime > 3)
                    {
                        SetMsg("读取 TVSN: " + strBuf + "与写入TVSN: " + SDriverX.g_modelInfo.sSn + "不一致", UDF.COLOR.FAIL);
                        return false;
                    }
                    else
                    {
                        iRetryTime++;
                        goto ReReadMISn;
                    }
                }
            }
            return true;
        }

        bool get_device_id()
        {
            iRetryTime = 0;
            string strBuf = string.Empty;
            ReCheckMAC:

            SetMsg("get device id ....", UDF.COLOR.WORK);
            StrTo_bby_Cmd("FF 33 06 03 51");
            g_bRTACKFlag = false;
            strBuf = string.Empty;
            if (WriteRS232CommandBBY(g_byCmdBuf, 6, 20, 22, out g_byCmdRtn) == false)
            {
                SetMsg("get device id cmd fail", UDF.COLOR.FAIL);
                if (iRetryTime > 3)
                    return false;
                else
                {
                    Thread.Sleep(500);
                    iRetryTime++;
                    goto ReCheckMAC;
                }
            }
            else
            {
                if (g_byCmdRtn[2] != 22)
                {
                    SetMsg("get device id fail", UDF.COLOR.FAIL);
                    if (iRetryTime > 3)
                        return false;
                    else
                    {
                        iRetryTime++;
                        goto ReCheckMAC;
                    }
                }

                for (i = 0; i < g_byCmdRtn[2] - 6; i++)
                {
                    strBuf = strBuf = strBuf + Convert.ToChar(g_byCmdRtn[i + 5]); ;
                }
                SDriverX.g_modelInfo.sDidKey = strBuf;
            }
            SetMsg("GET Device ID: " + SDriverX.g_modelInfo.sDidKey, UDF.COLOR.WORK);
            if (Insert_DEVICESN_Info(SDriverX.g_modelInfo.sDidKey) == false) return false;
            return true;
        }
        bool get_bbby_eth_mac()
        {
            iRetryTime = 0;
            string strBuf = string.Empty;
            ReCheckMAC:
            
            SetMsg("get bby eth mac ....", UDF.COLOR.WORK);
            StrTo_bby_Cmd("FF 33 07 03 5A 00");
            g_bRTACKFlag = false;
            strBuf = string.Empty;
            if (WriteRS232CommandBBY(g_byCmdBuf, 7, 20, 13, out g_byCmdRtn) == false)
            {
                SetMsg("CHECK SYSTEM ETH MAC CMD fail", UDF.COLOR.FAIL);
                if (iRetryTime > 3)
                    return false;
                else
                {
                    Thread.Sleep(500);
                    iRetryTime++;
                    goto ReCheckMAC;
                }
            }
            else
            {
                if (g_byCmdRtn[2] != 13)
                {
                    SetMsg("CHECK SYSTEM ETH MAC FAIL", UDF.COLOR.FAIL);
                    if (iRetryTime > 3)
                        return false;
                    else
                    {
                        iRetryTime++;
                        goto ReCheckMAC;
                    }
                }

                for (i = 0; i < g_byCmdRtn[2]-7; i++)
                {         
                    strBuf = strBuf+ string.Format("{0:X02}", g_byCmdRtn[i+6]);
                }
                SDriverX.g_modelInfo.sEthMac = strBuf;
                if (SDriverX.g_modelInfo.sEthMac == "000000000000")
                {
                    SetMsg("get eth mac fail " + SDriverX.g_modelInfo.sEthMac, UDF.COLOR.FAIL);
                    return false;
                }
            }
            SetMsg("GET ETH MAC: " + SDriverX.g_modelInfo.sEthMac, UDF.COLOR.WORK);
            if (Insert_MAC_Info("ETH_MAC", SDriverX.g_modelInfo.sEthMac)==false) return false;
            return true;
        }

        bool get_bbby_wifi_mac()
        {
            iRetryTime = 0;
            string strBuf = string.Empty;
            ReCheckMAC:
            SetMsg("get bby wifi mac ....", UDF.COLOR.WORK);
            StrTo_bby_Cmd("FF 33 07 03 5A 01");
            g_bRTACKFlag = false;
            strBuf = string.Empty;
            if (WriteRS232CommandBBY(g_byCmdBuf, 7, 20, 13, out g_byCmdRtn) == false)
            {
                SetMsg("CHECK SYSTEM WIFI CMD fail", UDF.COLOR.FAIL);
                if (iRetryTime > 3)
                    return false;
                else
                {
                    Thread.Sleep(500);
                    iRetryTime++;
                    goto ReCheckMAC;
                }
            }
            else
            {
                if (g_byCmdRtn[2] != 13)
                {
                    SetMsg("CHECK SYSTEM ETH MAC FAIL", UDF.COLOR.FAIL);
                    if (iRetryTime > 3)
                        return false;
                    else
                    {
                        iRetryTime++;
                        goto ReCheckMAC;
                    }
                }

                for (i = 0; i < g_byCmdRtn[2] - 7; i++)
                {
                    strBuf = strBuf + string.Format("{0:X02}", g_byCmdRtn[i + 6]);
                }
                SDriverX.g_modelInfo.sWifiMac = strBuf;
                if (SDriverX.g_modelInfo.sWifiMac == "000000000000")
                {
                    SetMsg("get wifi mac fail " + SDriverX.g_modelInfo.sWifiMac, UDF.COLOR.FAIL);
                    return false;
                }
            }
            iMacIndex = 1;
            SetMsg("GET WIFI MAC: " + SDriverX.g_modelInfo.sWifiMac, UDF.COLOR.WORK);
            if (Insert_MAC_Info("WIFI_MAC", SDriverX.g_modelInfo.sWifiMac) == false) return false;
            return true;
        }
        bool get_bbby_bt_mac()
        {
            iRetryTime = 0;
            string strBuf = string.Empty;
            ReCheckMAC:
            SetMsg("get bby bt mac ....", UDF.COLOR.WORK);
            StrTo_bby_Cmd("FF 33 07 03 5A 02");
            g_bRTACKFlag = false;
            strBuf = string.Empty;
            if (WriteRS232CommandBBY(g_byCmdBuf, 7, 20, 13, out g_byCmdRtn) == false)
            {
                SetMsg("CHECK SYSTEM BT CMD fail", UDF.COLOR.FAIL);
                if (iRetryTime > 3)
                    return false;
                else
                {
                    Thread.Sleep(500);
                    iRetryTime++;
                    goto ReCheckMAC;
                }
            }
            else
            {
                if (g_byCmdRtn[2] != 13)
                {
                    SetMsg("CHECK SYSTEM BT MAC FAIL", UDF.COLOR.FAIL);
                    if (iRetryTime > 3)
                        return false;
                    else
                    {
                        iRetryTime++;
                        goto ReCheckMAC;
                    }
                }

                for (i = 0; i < g_byCmdRtn[2] - 7; i++)
                {
                    strBuf = strBuf + string.Format("{0:X02}", g_byCmdRtn[i + 6]);
                }
                SDriverX.g_modelInfo.sBtMac = strBuf;
                if (SDriverX.g_modelInfo.sBtMac == "000000000000")
                {
                    SetMsg("get bt mac fail " + SDriverX.g_modelInfo.sBtMac, UDF.COLOR.FAIL);
                    return false;
                }
            }
            iMacIndex = 1;
            SetMsg("GET BT MAC: " + SDriverX.g_modelInfo.sBtMac, UDF.COLOR.WORK);
            if (Insert_MAC_Info("BT_MAC", SDriverX.g_modelInfo.sBtMac) == false) return false;
            return true;
        }

        
        bool set_clear_keyflag()
        {
            iRetryTime = 0;
            string strBuf = string.Empty;
            char[] chr = new char[1];
            Reaging:
            strBuf = "";
            SetMsg("set clear keypad flag...", UDF.COLOR.WORK);
            StrTo_bby_Cmd("FF 33 06 03 E5");  //FF 33 06 03 E5
            g_bRTACKFlag = true;
            if (WriteRS232CommandBBY(g_byCmdBuf, 6, 20, 12, out g_byCmdRtn) == false)
            {
                SetMsg("set clear keypad flag CMD fail", UDF.COLOR.FAIL);
                if (iRetryTime > 2)
                    return false;
                else
                {
                    Thread.Sleep(200);
                    iRetryTime++;
                    goto Reaging;
                }
            }
            return true;
        }
        bool set_bby_aging_on()
        {
            iRetryTime = 0;
            string strBuf = string.Empty;
            char[] chr = new char[1];
            Reaging:
            strBuf = "";
            SetMsg("set aging on...", UDF.COLOR.WORK);
            StrTo_bby_Cmd("FF 33 06 03 C8");
            g_bRTACKFlag = true;
            if (WriteRS232CommandBBY(g_byCmdBuf, 6, 20, 12, out g_byCmdRtn) == false)
            {
                SetMsg("set aging on CMD fail", UDF.COLOR.FAIL);
                if (iRetryTime > 2)
                    return false;
                else
                {
                    Thread.Sleep(200);
                    iRetryTime++;
                    goto Reaging;
                }
            }
            return true;
        }

        bool set_bby_aging_off()
        {
            iRetryTime = 0;
            string strBuf = string.Empty;
            char[] chr = new char[1];
            Reaging:
            strBuf = "";
            SetMsg("set aging off...", UDF.COLOR.WORK);
            StrTo_bby_Cmd("FF 33 06 03 C9");
            g_bRTACKFlag =false;
            if (WriteRS232CommandBBY(g_byCmdBuf, 6, 20, 12, out g_byCmdRtn) == false)
            {
                SetMsg("set aging on CMD fail", UDF.COLOR.FAIL);
                if (iRetryTime > 2)
                    return false;
                else
                {
                    Thread.Sleep(200);
                    iRetryTime++;
                    goto Reaging;
                }
            }
            return true;
        }
    }
}