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
    class tw{ }
}

namespace PRETEST
{
    public partial class FormMain
    {
        //public static int iRetryTime = new int();
        //public static string strFW = string.Empty;
        //public static string strProgID = string.Empty;
        //public static string strTAG = string.Empty;
        //public static string strRestart = string.Empty;
        //public static int i = new int();
        //public static int iMacIndex = new int();
        //public static string strOKNG = string.Empty;
        public static string TempACK = String.Empty;
        public static string TempCMD = String.Empty;
        public static string strCMD = String.Empty;
        bool PRE_TW()
        {
            string strBuf = string.Empty;
            string strSN2 = string.Empty;
            iRetryTime = 0;
            SetMsg(DateTime.Now.ToString(), UDF.COLOR.WORK);    //DEBUG
            if (Check_TW_FW() == false) return false;
            if (set_modelname() == false) return false;
            if (Write_tw_tvsn() == false) return false;
            if (CHECK_ETH_Key() == false) return false;
            if (CHECK_WIFI_Key() == false) return false;
            if (CHECK_ESN_Key()==false) return false;
            //if (g_bAutoScan == true)
            //{
            //    ReSCAN2:
            //    SetMsg("第二次条码信息核对...", UDF.COLOR.WORK);
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
            if (SET_POWER_ONLY() == false) return false;
            SetMsg(DateTime.Now.ToString(), UDF.COLOR.WORK);    //DEBUG
            return true;
        }

        bool Check_TW_FW()
        {
            //string progid;
            ReFW:
            SetMsg("Check TW FW Version", UDF.COLOR.WORK);
            StrTo_TW_Cmd("61 76 20 30 30 20 30 30 0D");
            g_bRTACKFlag = true; //SET true 指令判断回传值;GET false 指令不判断回传值
            strFW = string.Empty;
            if (WriteRS232CommandTW(g_byCmdBuf, 9, 20, 16, out TempACK) == false)
            {
                SetMsg("Check FW Version CMD fail", UDF.COLOR.FAIL);
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
                strFW = TempACK.Substring(7, 8).Trim();
                this.label8.Text = "FW:";
                this.textBoxFW.Text = strFW;
                if ((g_iPtInitFlag & INIT_FW) == INIT_FW)  //如果已经有了首次查询了机种FW信息，则比较缓存数据不再联网查询
                {
                    if ((strFW.IndexOf(g_sFwVer1) == -1) && (strFW.IndexOf(g_sFwVer2) == -1))   //机台 fw 与 数据库 fw1、fw2 均比较错误
                    {
                        //FW 比对失败
                        SetMsg("FW比对错误！机台FW:" + strFW + "\n数据库FW:" + g_sFwVer1 + "/" + g_sFwVer2, UDF.COLOR.FAIL);
                        return false;
                    }
                    else
                    {
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
        bool set_modelname()
        {
            string strModel = string.Empty;
            int i = 0;
            TempCMD = "";
            strCMD = "";
            if (SDriverX.g_modelInfo.sPart_no.Substring(13, 3).ToUpper() == "50G")
            {
                strModel = "50G";
            }
            else if (SDriverX.g_modelInfo.sPart_no.Substring(13, 5).ToUpper() == "55TG2")
            {
                strModel = "55TG2";
            }
            else if (SDriverX.g_modelInfo.sPart_no.Substring(13, 5).ToUpper() == "50TG2")
            {
                strModel = "50TG2";
            }
            else if (SDriverX.g_modelInfo.sPart_no.Substring(13, 3).ToUpper() == "50T")
            {
                strModel = "50TG";
            }
            else if (SDriverX.g_modelInfo.sPart_no.Substring(13, 3).ToUpper() == "65G")
            {
                strModel = "65G";
            }
            else if (SDriverX.g_modelInfo.sPart_no.Substring(13, 3).ToUpper() == "75G")
            {
                strModel = "75G";
            }
            else if (SDriverX.g_modelInfo.sPart_no.Substring(13, 3).ToUpper() == "65T")
            {
                strModel = "65TG";
            }
            else if (SDriverX.g_modelInfo.sPart_no.Substring(13, 3).ToUpper() == "75T")
            {
                strModel = "75TG";
            }
            else if (SDriverX.g_modelInfo.sPart_no.Substring(13, 3).ToUpper() == "55T")
            {
                strModel = "55TG";
            }
            else if (SDriverX.g_modelInfo.sPart_no.Substring(13, 3).ToUpper() == "32G")
            {
                strModel = "32G";
            }
            else
            {
                SetMsg("MODEL Name参数程式上未设定，请联系TE..", UDF.COLOR.FAIL);
                return false;
            }
            //TempCMD = "";
            //SetMsg("SET MODEL NAME: " + strModel, UDF.COLOR.WORK);
            //strCMD = "sb" + strModel;
            //byte[] wdata = Encoding.Default.GetBytes(strCMD);

            //for (i = 0; i < wdata.Length; i++)
            //{
            //    TempCMD += wdata[i].ToString("x0");
            //}
            //TempCMD = "A087" + TempCMD + "0D";
            //StrTo_TW_Cmd(TempCMD);
            //g_bRTACKFlag = true; //SET true 指令判断回传值;GET false 指令不判断回传值
            //if (WriteRS232CommandTW(g_byCmdBuf, 8, 30, 2, out TempACK) == false)
            //{
            //    Application.DoEvents();
            //}

            ReGETModel:
            TempCMD = "";
            int rcklen = 0;
            SetMsg("CHECK MODEL NAME: " + strModel, UDF.COLOR.WORK);
            strCMD = "sb";
            byte[] rdata = Encoding.Default.GetBytes(strCMD);
            for (i = 0; i < rdata.Length; i++)
            {
                TempCMD += rdata[i].ToString("x0");
            }
            if (SDriverX.g_modelInfo.sPart_no.Substring(13, 5).ToUpper() == "55TG2" || SDriverX.g_modelInfo.sPart_no.Substring(13, 5).ToUpper() == "50TG2")
            {
                TempCMD = "A189" + TempCMD + "0D";
                rcklen = 5;
            }
            else if (SDriverX.g_modelInfo.sPart_no.Substring(13, 3).ToUpper() == "50T" || SDriverX.g_modelInfo.sPart_no.Substring(13, 3).ToUpper() == "65T" || SDriverX.g_modelInfo.sPart_no.Substring(13, 3).ToUpper() == "75T" || SDriverX.g_modelInfo.sPart_no.Substring(13, 3).ToUpper() == "55T")
            {
                TempCMD = "A188" + TempCMD + "0D";
                rcklen = 4;
            }
            else
            {

                TempCMD = "A187" + TempCMD + "0D";
                rcklen = 3;
            }
            StrTo_TW_Cmd(TempCMD);
            g_bRTACKFlag = false; //SET true 指令判断回传值;GET false 指令不判断回传值
            if (WriteRS232CommandTW(g_byCmdBuf, 5, 30, rcklen, out TempACK) == false)
            {

                if (iRetryTime > 2)
                {
                    SetMsg("CHECK MODEL NAME CMD fail", UDF.COLOR.FAIL);
                    return false;
                }
                else
                {
                    iRetryTime++;
                    goto ReGETModel;
                }
            }
            if (TempACK.Trim() != strModel)
            {
                SetMsg("读取Model Name: " + TempACK + " 与设定： " + strModel + "不符", UDF.COLOR.FAIL);
                return false;
            }
            return true;
        }
        bool Write_tw_tvsn()
        {
            TempCMD = "";
            SetMsg("SET TV SN..", UDF.COLOR.WORK);
            strCMD = "sa" + SDriverX.g_modelInfo.sSn;
            byte[] wdata = Encoding.Default.GetBytes(strCMD);
            for (i = 0; i < wdata.Length; i++)
            {
                TempCMD += wdata[i].ToString("x0");
            }
            TempCMD = "A090" + TempCMD + "0D";
            StrTo_TW_Cmd(TempCMD);
            g_bRTACKFlag = true; //SET true 指令判断回传值;GET false 指令不判断回传值

            if (WriteRS232CommandTW(g_byCmdBuf, 17, 20, 2, out TempACK) == false)
            {
                Application.DoEvents();
            }

            ReGETSN:
            TempCMD = "";
            TempACK = "";
            SetMsg("CHECK TV SN..", UDF.COLOR.WORK);
            strCMD = "sa";
            byte[] rdata = Encoding.Default.GetBytes(strCMD);
            for (i = 0; i < rdata.Length; i++)
            {
                TempCMD += rdata[i].ToString("x0");
            }
            TempCMD = "A190" + TempCMD + "0D";
            StrTo_TW_Cmd(TempCMD);
            g_bRTACKFlag = false; //SET true 指令判断回传值;GET false 指令不判断回传值
            if (WriteRS232CommandTW(g_byCmdBuf, 5, 20, 12, out TempACK) == false)
            {
                SetMsg("GET TV SN CMD fail", UDF.COLOR.FAIL);
                if (iRetryTime > 2)
                    return false;
                else
                {
                    iRetryTime++;
                    goto ReGETSN;
                }
            }
            if (TempACK.Trim() != SDriverX.g_modelInfo.sSn)
            {
                SetMsg("读取TV SN " + TempACK + " 与设定： " + SDriverX.g_modelInfo.sSn + "不符", UDF.COLOR.FAIL);
                return false;
            }
            return true;
        }

        bool CHECK_ESN_Key()
        {
            ReGETESN:
            TempCMD = "";
            TempACK = "";
            SetMsg("CHECK ESN KEY..", UDF.COLOR.WORK);
            strCMD = "as 10 10";
            byte[] rdata = Encoding.Default.GetBytes(strCMD); //字符串转十六机制
            for (i = 0; i < rdata.Length; i++)
            {
                TempCMD += rdata[i].ToString("x0");
            }
            TempCMD = TempCMD + "0D";
            StrTo_TW_Cmd(TempCMD);
            g_bRTACKFlag =true; //SET true 指令判断回传值;GET false 指令不判断回传值
            if (WriteRS232CommandTW(g_byCmdBuf, 9, 20, 3, out TempACK) == false)
            { 
                if (iRetryTime > 2)
                {
                    SetMsg("CHECK ESN KEY CMD fail", UDF.COLOR.FAIL);
                    return false;
                }  
                else
                {
                    iRetryTime++;
                    goto ReGETESN;
                }
            }
            return true;
        }
        bool CHECK_ETH_Key()
        {
            ReGETETH:
            TempCMD = "";
            TempACK = "";
            SetMsg("CHECK ETH MAC KEY..", UDF.COLOR.WORK);
            strCMD = "ai 70 70";
            byte[] rdata = Encoding.Default.GetBytes(strCMD);
            for (i = 0; i < rdata.Length; i++)
            {
                TempCMD += rdata[i].ToString("x0");
            }
            TempCMD = TempCMD + "0D";
            StrTo_TW_Cmd(TempCMD);
            g_bRTACKFlag = true ; //SET true 指令判断回传值;GET false 指令不判断回传值
            if (WriteRS232CommandTW(g_byCmdBuf, 9, 20, 20, out TempACK) == false)
            {
                if (iRetryTime > 2)
                {
                    SetMsg("CHECK ETH MAC FAIL", UDF.COLOR.FAIL);
                    return false;
                }
                else
                {
                    iRetryTime++;
                    goto ReGETETH;
                }
            }
            SDriverX.g_modelInfo.sEthMac =TempACK.Substring(7,12).Trim();
            Insert_MAC_Info("ETH_MAC", SDriverX.g_modelInfo.sEthMac);
            return true;
        }

        bool CHECK_WIFI_Key()
        {
            ReGETWifi:
            TempCMD = "";
            TempACK = "";
            SetMsg("CHECK WIFI MAC KEY..", UDF.COLOR.WORK);
            strCMD = "ai 74 74";
            byte[] rdata = Encoding.Default.GetBytes(strCMD);
            for (i = 0; i < rdata.Length; i++)
            {
                TempCMD += rdata[i].ToString("x0");
            }
            TempCMD = TempCMD + "0D";
            StrTo_TW_Cmd(TempCMD);
            g_bRTACKFlag = true; //SET true 指令判断回传值;GET false 指令不判断回传值
            if (WriteRS232CommandTW(g_byCmdBuf, 9, 20, 20, out TempACK) == false)
            {
                if (iRetryTime > 2)
                {
                    SetMsg("CHECK WIFI MAC FAIL", UDF.COLOR.FAIL);
                    return false;
                }
                else
                {
                    iRetryTime++;
                    goto ReGETWifi;
                }
            }
            SDriverX.g_modelInfo.sWifiMac = TempACK.Substring(7, 12).Trim();
            Insert_MAC_Info("WIFI_MAC", SDriverX.g_modelInfo.sWifiMac);
            return true;
        }

        bool SET_POWER_ONLY()
        {
            ReSETP:
            TempCMD = "";
            SetMsg("SET POWER ONLY..", UDF.COLOR.WORK);
            strCMD = "mc fe fe"; 
            byte[] wdata = Encoding.Default.GetBytes(strCMD);
            for (i = 0; i < wdata.Length; i++)
            {
                TempCMD += wdata[i].ToString("x0");
            }
            TempCMD =TempCMD + "0D";
            StrTo_TW_Cmd(TempCMD);
            g_bRTACKFlag = true; //SET true 指令判断回传值;GET false 指令不判断回传值

            if (WriteRS232CommandTW(g_byCmdBuf, 9, 20, 10, out TempACK) == false)
            {
                
                if (iRetryTime > 2)
                {
                    SetMsg("SET P ONLEY CMD fail", UDF.COLOR.FAIL);
                    return false;
                }
                else
                {
                    iRetryTime++;
                    goto ReSETP;
                }
            }
            return true;
        }
    }
}

