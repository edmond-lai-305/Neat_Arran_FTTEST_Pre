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
    class xm{ }
}

namespace PRETEST
{
    public partial class FormMain
    {
        //[DllImport("Kernel32.dll")]
        //private static extern long GetPrivateProfileString(string section, string key,
        //     string def, StringBuilder retVal, int size, string filePath);
        //public static string ReadIniFile(string section, string key, string filePath)
        //{
        //    StringBuilder keyString = new StringBuilder(1024);
        //    string def = null;
        //    GetPrivateProfileString(section, key, def, keyString, 1024, filePath);
        //    return keyString.ToString().Trim();
        //}
        //public static string sinst_mic1 = string.Empty;
        //public static string builddate2 = string.Empty;
        //public static string builddate = string.Empty;
        public static int iRetryTime = new int();
        public static string strFW = string.Empty;
        public static string strProgID = string.Empty;
        public static string strTAG = string.Empty;
        public static string strRestart = string.Empty;
        public static int i = new int();
        public static int iMacIndex = new int();
        public static string strOKNG = string.Empty;

        bool PRE_XM()
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
            //if (Check_SetBurnInOff() == false) return false;
            if (Write_mi_eth_mac() == false) return false;
            if (Write_mi_didkey() == false) return false;
            if(Check_widevine_key_size() == false) return false;
  
            if (Check_hdcpkey_size() == false) return false;
            //if (Check_wifi_mac() == false) return false;
            //if (Check_bt_mac() == false) return false;
            if (Check_Panel_test() == false) return false;
            if (Check_emmc_Lefttime() == false) return false;
            //if (g_bAutoScan ==true)
            //{
            //    ReSCAN2:
            //    SetMsg("第二次条码信息核对...", UDF.COLOR.WORK);
            //    if (Sick_scan_again(out strSN2)==false )
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
            SetMsg(DateTime.Now.ToString(), UDF.COLOR.WORK);    //DEBUG
            return true;
        }

        bool Check_MI_FW()
        {
            //string progid;
            //Delay(100);
            ReFW:
            SetMsg("Check MI FW Version", UDF.COLOR.WORK);
            StrToCmd("79 00 14 57 00 00 01");
            g_bRTACKFlag = false; //SET true 指令判断回传值;GET false 指令不判断回传值
            strFW = string.Empty;
            if (WriteRS232CommandMI(g_byCmdBuf, 9, 10, 29, out g_byCmdRtn) == false)
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
                if (g_byCmdRtn[13] < 3)
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
                    for (i = 0; i < g_byCmdRtn[13]; i++)
                    {
                        strFW = strFW + Convert.ToChar(g_byCmdRtn[i + 14]);
                    }
                }
                //this.label8.Text = "FW:";
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
                        SetMsg("获取机台版本： " + strFW, UDF.COLOR.WORK);
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
        bool Check_MI_TAG()
        {
            iRetryTime = 0;
            ReTAG:
            strTAG = "";
            SetMsg("Check MI TAG..", UDF.COLOR.WORK);
            StrToCmd("79 00 14 83 00 00 01");  //79 01 14 83 00 00 01
            g_bRTACKFlag = false; //SET true 指令判断回传值;GET false 指令不判断回传值
            strFW = string.Empty;
            if (WriteRS232CommandMI(g_byCmdBuf, 9, 20, 25, out g_byCmdRtn) == false)
            {
                if (iRetryTime > 2)
                {
                    SetMsg("Check TAG CMD fail", UDF.COLOR.FAIL);
                    return false;
                }
                else
                {
                    iRetryTime++;
                    goto ReTAG;
                }
            }
            else
            {
                if (g_byCmdRtn[13]==0)
                {
                   
                    if (iRetryTime > 2)
                    {
                        SetMsg("Check TAG CMD LENGTH fail", UDF.COLOR.FAIL);
                        return false;
                    }  
                    else
                    {
                        Thread.Sleep(200);
                        iRetryTime++;
                        goto ReTAG;
                    }
                }
                for (i = 0; i < g_byCmdRtn[13]; i++)
                {
                    strTAG = strTAG + Convert.ToChar(g_byCmdRtn[i + 14]);
                }
                if  (CheckTAGSql(strProgID, strTAG) == false) return false;
            }
            strOKNG = "OK";
            Insert_ORALCEdatabase(SDriverX.g_modelInfo.sPart_no +"_tag:" + strTAG);

            if (g_bXM_DZ3L == true || g_bXM_DZ4L == true || g_bXM_DZCL == true || g_bXM_DZDL == true || g_bXM_DZEL == true || g_bXM_DZFL == true || g_bXM_DZGL == true || g_bXM_RZ2L == true || g_bXM_RZ3L == true || g_bXM_RZ7L == true || g_bXM_RZ8L == true
                    || g_bXM_RZ9L == true || g_bXM_RZAL == true || g_bXM_RZBL == true || g_bXM_RZCL == true || g_bXM_DZLL == true || g_bXM_DZKL == true || g_bXM_DZNL == true || g_bXM_DZIL == true || g_bXM_DZHL == true || g_bXM_DZJL == true  || g_bXM_DZQL == true )
            {
                iRetryTime = 0;
                ReGetModel:
                string bootModel = string.Empty;
                strTAG = "";
                SetMsg("Check Boot MODEL...", UDF.COLOR.WORK);
                StrToCmd("79 00 14 3D 1C 67 65 74 70 72 6F 70 20 7C 20 67 72 65 70 20 72 6F 2E 62 6F 6F 74 2E 6D 6F 64 65 6C 00 01");  //79 01 14 83 00 00 01
                g_bRTACKFlag = false; //SET true 指令判断回传值;GET false 指令不判断回传值
                if (WriteRS232CommandMI(g_byCmdBuf, 37, 20, 50, out g_byCmdRtn) == false)
                {
                    if (iRetryTime > 2)
                    {
                        SetMsg("Check Boot MODEL CMD fail", UDF.COLOR.FAIL);
                        return false;
                    }
                    else
                    {
                        iRetryTime++;
                        goto ReTAG;
                    }
                }
                else
                {
                    if (g_byCmdRtn[13] <20)
                    {

                        if (iRetryTime > 2)
                        {
                            SetMsg("Check Boot MODEL CMD LENGTH fail", UDF.COLOR.FAIL);
                            return false;
                        }
                        else
                        {
                            Thread.Sleep(200);
                            iRetryTime++;
                            goto ReGetModel;
                        }
                    }
                    for (i = 0; i < g_byCmdRtn[13]; i++)
                    {
                        bootModel = bootModel + Convert.ToChar(g_byCmdRtn[i + 14]);
                    }
                    bootModel = bootModel.Replace("\0", "").ToUpper();
                    //bootModel = textBoxModel.Text;
                    strOKNG = "NA";
                    Insert_ORALCEdatabase("model:" + bootModel);
                    if (bootModel.IndexOf(textBoxModel.Text.Substring(3, 4)) == -1)
                    {
                        SetMsg("读取 Boot model: " + bootModel + "与实际Model: " + textBoxModel.Text.Substring(3, 4) + "不一致", UDF.COLOR.FAIL);
                        return false;
                    }
                }
            }
                
            return true;
        }
        bool Check_Abnormal_Restart()
        {
            iRetryTime = 0;
            RE_check_Abnormal:
            SetMsg("Check_Abnormal_Restart Times", UDF.COLOR.WORK);
            StrToCmd("79 00 14 51 00 00 01");  //79 01 14 51 00 00 01
            g_bRTACKFlag = false;
            strRestart = string.Empty;
            if (WriteRS232CommandMI(g_byCmdBuf, 9, 10, 25, out g_byCmdRtn) == false)
            {
                if (iRetryTime > 3)
                {
                    SetMsg("Check_Abnormal_Restart CMD fail", UDF.COLOR.FAIL);
                    return false;
                } 
                else
                {
                    Thread.Sleep(200);
                    iRetryTime++;
                    goto RE_check_Abnormal;
                }
            }
            else
            {
                try
                {
                    if (g_byCmdRtn[13] == 0)
                    {
                        SetMsg("Check_Abnormal_Restart length fail", UDF.COLOR.FAIL);
                        if (iRetryTime > 2)
                            return false;
                        else
                        {
                            Thread.Sleep(200);
                            iRetryTime++;
                            goto RE_check_Abnormal;
                        }
                    }

                    for (i = 0; i < g_byCmdRtn[13]; i++)
                    {
                        strRestart = strRestart + Convert.ToChar(g_byCmdRtn[i + 14]);
                    }
                    if ((g_byCmdRtn[16] > 48) || (g_byCmdRtn[18] > 49) || (g_byCmdRtn[20] >49) || (g_byCmdRtn[22] > 49) || (g_byCmdRtn[24] > 49))
                    {
                        SetMsg("abnormal restart counts:" + strRestart, UDF.COLOR.FAIL);
                        if (iRetryTime > 3)
                        {
                            strOKNG = "NG";
                            Insert_ORALCEdatabase("Abnormal_Restart:" + strRestart);
                            return false;
                        }
                        else
                        {
                            iRetryTime++;
                            goto RE_check_Abnormal;
                        }
                    }
                }
                catch (Exception err1)
                {
                    SetMsg("Error prompt:" + err1.Message, UDF.COLOR.FAIL);
                    return false;
                }
            }
            SetMsg("abnormal restart counts:" + strRestart, UDF.COLOR.WORK);
            return true;
        }

        bool Write_mi_tvsn()
        {
            iRetryTime = 0;
            string strBuf = string.Empty;
            char[] chr = new char[1];
            ReWriteMISn:
            strBuf = "";
            SetMsg("WRITE MI TV SN:" + SDriverX.g_modelInfo.sSn, UDF.COLOR.WORK);
            strBuf = "79 00 14 74 11";
            for (int i = 0; i < 18; i++)
            {
                if (SDriverX.g_modelInfo.sSn.Substring(i, 1) == "/")
                    continue;
                else
                {
                    chr = SDriverX.g_modelInfo.sSn.Substring(i, 1).ToCharArray(0, 1);
                    strBuf += string.Format(" {0:X02}", (byte)chr[0]);
                }
            }
            strBuf = strBuf + " 00 01";
            StrToCmd(strBuf);
            g_bRTACKFlag = true;
            strRestart = string.Empty;
            if (WriteRS232CommandMI(g_byCmdBuf, 26, 20, 19, out g_byCmdRtn) == false)
            {
                if (iRetryTime > 3)
                {
                    SetMsg("Write TVSN CMD fail", UDF.COLOR.FAIL);
                    return false;
                } 
                else
                {
                    iRetryTime++;
                    goto ReWriteMISn;
                }
            }
            iRetryTime = 0;
            ReReadMISn:
            SetMsg("CHECK MI TV SN:" + SDriverX.g_modelInfo.sSn, UDF.COLOR.WORK);
            StrToCmd("79 00 14 75 00 00 01");//79 01 14 75 00 00 01
            g_bRTACKFlag = false;
            strBuf = string.Empty;
            if (WriteRS232CommandMI(g_byCmdBuf, 9, 20, 32, out g_byCmdRtn) == false)
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
                if (g_byCmdRtn[13]!=17)
                {
                    SetMsg("Read MI TVSN LENGTH fail", UDF.COLOR.FAIL);
                    if (iRetryTime > 3)
                        return false;
                    else
                    {
                        Thread.Sleep(200);
                        iRetryTime++;
                        goto ReReadMISn;
                    }
                }
                for (i = 0; i < g_byCmdRtn[13]; i++)
                {
                    strBuf = strBuf + Convert.ToChar(g_byCmdRtn[i + 14]);
                }
                strBuf = strBuf.Substring(0, 5) + "/" + strBuf.Substring(5, 12);
                if (strBuf != SDriverX.g_modelInfo.sSn)
                {
                    if (iRetryTime > 3)
                    {
                        SetMsg("Read TVSN: " + strBuf + " and Write TVSN: " + SDriverX.g_modelInfo.sSn + " not Match", UDF.COLOR.FAIL);
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

        bool Write_mi_tvmn()
        {
            if (QueryMN() == false) return false;
            iRetryTime = 0;
            string strBuf = string.Empty;
            char[] chr = new char[1];
            ReCheckMBMN:
            strBuf = "";
            SetMsg("CHECK MI MB MN :" + SDriverX.g_modelInfo.sMBMn, UDF.COLOR.WORK);
            StrToCmd("79 00 14 73 00 00 01");//79 01 14 73 00 00 01
            g_bRTACKFlag = false;
            strBuf = string.Empty;
            if (WriteRS232CommandMI(g_byCmdBuf, 9, 30, 36, out g_byCmdRtn) == false)
            {
                if (iRetryTime > 3)
                {
                    SetMsg("Read MB MN CMD fail", UDF.COLOR.FAIL);
                    return false;
                }
                else
                {
                    Thread.Sleep(300);
                    iRetryTime++;
                    goto ReCheckMBMN;
                }
            }
            else
            {
                if (g_byCmdRtn[13]!=21)
                {
                    SetMsg("Read MI MB MN LENGTH fail", UDF.COLOR.FAIL);
                    if (iRetryTime > 3)
                        return false;
                    else
                    {
                        Thread.Sleep(200); 
                        iRetryTime++;
                        goto ReCheckMBMN;
                    }
                }
                for (i = 0; i < g_byCmdRtn[13]; i++)
                {
                    strBuf = strBuf + Convert.ToChar(g_byCmdRtn[i + 14]);
                }
                if (strBuf != SDriverX.g_modelInfo.sMBMn)
                {
                    if (iRetryTime > 1)
                    {
                        SetMsg("Read MB MN: " + strBuf + " and Mes system MB MN: " + SDriverX.g_modelInfo.sMBMn + " Not Match", UDF.COLOR.FAIL);
                        return false;
                    }
                    else
                    {
                        iRetryTime++;
                        goto ReCheckMBMN;
                    }
                }
            }
            Thread.Sleep(100);
            iRetryTime = 0;
            ReWriteTVMN:
            strBuf = "";
            SetMsg("WRTE MI TV MN :" + SDriverX.g_modelInfo.sMn, UDF.COLOR.WORK);
            strBuf = "79 00 14 76 15";
            for (int i = 0; i < 21; i++)
            {
                chr = SDriverX.g_modelInfo.sMn.Substring(i, 1).ToCharArray(0, 1);
                strBuf += string.Format(" {0:X02}", (byte)chr[0]);
            }
            strBuf = strBuf + " 00 01";
            StrToCmd(strBuf);
            g_bRTACKFlag = true;
            strRestart = string.Empty;
            if (WriteRS232CommandMI(g_byCmdBuf, 30, 20, 19, out g_byCmdRtn) == false)
            {
                if (iRetryTime > 3)
                {
                    SetMsg("Write TV MN CMD fail", UDF.COLOR.FAIL);
                    return false;
                } 
                else
                {
                    iRetryTime++;
                    goto ReWriteTVMN;
                }
            }
            Thread.Sleep(300);

            ReReadTVSMN:
            strBuf = "";
            SetMsg("CHECK MI TV MN:" + SDriverX.g_modelInfo.sMn, UDF.COLOR.WORK);
            StrToCmd("79 00 14 77 00 00 01");//79 01 14 77 00 00 01
            g_bRTACKFlag = false;
            strBuf = string.Empty;
            if (WriteRS232CommandMI(g_byCmdBuf, 9, 20, 36, out g_byCmdRtn) == false)
            {
                if (iRetryTime > 3)
                {
                    SetMsg("Read MI TV MN CMD fail", UDF.COLOR.FAIL);
                    return false;
                }
                else
                {
                    Thread.Sleep(500);
                    iRetryTime++;
                    goto ReReadTVSMN;
                }
            }
            else
            {
                if (g_byCmdRtn [13]!=21)
                {
                    SetMsg("Read MI TV MN LENGTH fail", UDF.COLOR.FAIL);
                    if (iRetryTime > 3)
                        return false;
                    else
                    {
                        Thread.Sleep(200);
                        iRetryTime++;
                        goto ReReadTVSMN;
                    }
                }
                for (i = 0; i < g_byCmdRtn[13]; i++)
                {
                    strBuf = strBuf + Convert.ToChar(g_byCmdRtn[i + 14]);
                }
                if (strBuf != SDriverX.g_modelInfo.sMn)
                {
                    if (iRetryTime > 3)
                    {
                        SetMsg("Read TVSN: " + strBuf + " and Wrtie TVSN: " + SDriverX.g_modelInfo.sMn + " Not Match", UDF.COLOR.FAIL);
                        return false;
                    }
                    else
                    {
                        iRetryTime++;
                        goto ReReadTVSMN;
                    }
                }
            }
            return true;
        }

        bool Write_mi_eth_mac()
        {
            if (QueryMI_MAC() == false) return false;
            iRetryTime = 0;
            string strBuf = string.Empty;
            string tempMac = string.Empty;
            char[] chr = new char[1];
            ReWriteETH_MAC:
            tempMac = "";
            SetMsg("WRITE MI ETH MAC:" + SDriverX.g_modelInfo.sEthMac, UDF.COLOR.WORK);
            strBuf = "79 00 14 7A 11";
            for (int i = 0; i < 6; i++)
            {
                if (i == 5)
                {
                    tempMac = tempMac + SDriverX.g_modelInfo.sEthMac.Substring(i * 2, 2);
                }
                else
                {
                    tempMac = tempMac + SDriverX.g_modelInfo.sEthMac.Substring(i * 2, 2) + ":";
                }
            }
            for (int i = 0; i < 17; i++)
            {
                chr = tempMac.Substring(i, 1).ToCharArray(0, 1);
                strBuf += string.Format(" {0:X02}", (byte)chr[0]);
            }
            strBuf = strBuf + " 00 01";
            this.textBoxDC.Text = tempMac;
            StrToCmd(strBuf);
            g_bRTACKFlag = true;
            if (WriteRS232CommandMI(g_byCmdBuf, 26, 20, 19, out g_byCmdRtn) == false)
            {
                if (iRetryTime > 3)
                {
                    SetMsg("Write ETH MAC CMD fail", UDF.COLOR.FAIL);
                    return false;
                }
                else
                {
                    iRetryTime++;
                    goto ReWriteETH_MAC;
                }
            }
            Delay(200);
            ReReadEthMac:
            strBuf = "";
            SetMsg("CHECK ETH MAC...." , UDF.COLOR.WORK);
            StrToCmd("79 00 14 7B 00 00 01");//79 01 14 7B 00 00 01
            g_bRTACKFlag = false;
            strBuf = string.Empty;
            if (WriteRS232CommandMI(g_byCmdBuf, 9, 20, 35, out g_byCmdRtn) == false)
            {
                SetMsg("Read ETH MAC CMD fail", UDF.COLOR.FAIL);
                if (iRetryTime > 3)
                    return false;
                else
                {
                    iRetryTime++;
                    goto ReReadEthMac;
                }
            }
            else
            {
                if (g_byCmdRtn[13]!=17)
                {
                    if (iRetryTime > 3)
                    {
                        SetMsg("Read ETH MAC LENGTH fail", UDF.COLOR.FAIL);
                        return false;
                    }
                    else
                    {
                        Thread.Sleep(300);
                        iRetryTime++;
                        goto ReReadEthMac;
                    }
                }
                for (i = 0; i < g_byCmdRtn[13]; i++)
                {
                    strBuf = strBuf + Convert.ToChar(g_byCmdRtn[i + 14]);
                }
                strBuf = strBuf.ToUpper().Trim().Replace(":", "");
                if (strBuf != SDriverX.g_modelInfo.sEthMac)
                {
                    if (iRetryTime > 3)
                    {
                        SetMsg("Read ETH MAC: " + strBuf + " and Write ETH MAC: " + SDriverX.g_modelInfo.sEthMac + " not Match", UDF.COLOR.FAIL);
                        return false;
                    }
                    else
                    {
                        iRetryTime++;
                        goto ReReadEthMac;
                    }
                }
            }
            return true;
        }

        bool Write_mi_didkey()
        {
            iRetryTime = 0;
            string strBuf = string.Empty;
            string tempMac = string.Empty;
            char[] chr = new char[1];
            ReWriteDidkey:
            strBuf = "";
            SetMsg("WRITE MI DID KEY:" + SDriverX.g_modelInfo.sDidKey, UDF.COLOR.WORK);
            strBuf = "79 00 14 9E 27";
            if (SDriverX.g_modelInfo.sDidKey.Length != 39)
            {
                SetMsg("DIDKEY Lenght Fail :" + SDriverX.g_modelInfo.sDidKey + " Please Connect TE", UDF.COLOR.WORK);
                return false;
            }
            for (int i = 0; i < 39; i++)
            {
                chr = SDriverX.g_modelInfo.sDidKey.Substring(i, 1).ToCharArray(0, 1);
                strBuf += string.Format(" {0:X02}", (byte)chr[0]);
            }
            strBuf = strBuf + " 00 01";
            StrToCmd(strBuf);
            g_bRTACKFlag = true;
            strRestart = string.Empty;
            if (WriteRS232CommandMI(g_byCmdBuf, 48, 20, 19, out g_byCmdRtn) == false)
            {
                if (iRetryTime > 2)
                {
                    SetMsg("Write DIDKEY CMD fail", UDF.COLOR.FAIL);
                    return false;
                }
                else
                {
                    Thread.Sleep(500);
                    iRetryTime++;
                    goto ReWriteDidkey;
                }
            }
            Application.DoEvents();
            Thread.Sleep(200);
            iRetryTime = 0;
            ReReadDidkey:
            strBuf = "";
            SetMsg("CHECK DID KEY....", UDF.COLOR.WORK);
            StrToCmd("79 00 14 9D 00 00 01");//79 01 14 9D 00 00 01
            g_bRTACKFlag = false;
            strBuf = string.Empty;
            if (WriteRS232CommandMI(g_byCmdBuf, 9, 40, 56, out g_byCmdRtn) == false)
            {
                
                if (iRetryTime > 2)
                {
                    SetMsg("Read DIDKEY CMD fail", UDF.COLOR.FAIL);
                    return false;
                }
                else
                {
                    Thread.Sleep(500);
                    iRetryTime++;
                    goto ReReadDidkey;
                }
            }
            else
            {
                if (g_byCmdRtn[13]==0)
                {
                    SetMsg("Read DIDKEY Length fail", UDF.COLOR.FAIL);
                    if (iRetryTime > 2)
                        return false;
                    else
                    {
                        Thread.Sleep(300);
                        iRetryTime++;
                        goto ReReadDidkey;
                    }
                }
                for (i = 0; i < g_byCmdRtn[13]; i++)
                {
                    strBuf = strBuf + Convert.ToChar(g_byCmdRtn[i + 14]);
                }

                if (strBuf != SDriverX.g_modelInfo.sDidKey)
                {
                    if (iRetryTime > 2)
                    {
                        SetMsg("Read DIDKEY: " + strBuf + " and Write DIDKEY: " + SDriverX.g_modelInfo.sDidKey + " not match", UDF.COLOR.FAIL);
                        return false;
                    }
                    else
                    {
                        iRetryTime++;
                        goto ReReadDidkey;
                    }
                }
            }
            return true;
        }
        bool Check_widevine_key_size()
        {
            iRetryTime = 0;
            string strBuf = string.Empty;
            ReReadWDkey:
            strBuf = "";
            SetMsg("CHECK Widevine key size....", UDF.COLOR.WORK);
            StrToCmd("79 00 14 6D 00 00 01");//79 01 14 6D 00 00 01
            g_bRTACKFlag = false;
            strBuf = string.Empty;
            if (WriteRS232CommandMI(g_byCmdBuf, 9, 25, 21, out g_byCmdRtn) == false)
            {
                SetMsg("Read Widevine key size CMD fail", UDF.COLOR.FAIL);
                if (iRetryTime > 3)
                    return false;
                else
                {
                    Thread.Sleep(500);
                    iRetryTime++;
                    goto ReReadWDkey;
                }
            }
            else
            {
                try
                {
                    if (g_byCmdRtn[13] == 0)
                    {
                        SetMsg("Read Widevine key lenght fail", UDF.COLOR.FAIL);
                        if (iRetryTime > 3)
                            return false;
                        else
                        {
                            Thread.Sleep(200);
                            iRetryTime++;
                            goto ReReadWDkey;
                        }
                    }
                    for (i = 0; i < g_byCmdRtn[13]; i++)
                    {
                        strBuf = strBuf + Convert.ToChar(g_byCmdRtn[i + 14]);
                    }

                    if (g_bXM_DZ3L==true || g_bXM_DZ4L == true || g_bXM_DZCL == true || g_bXM_DZDL == true || g_bXM_DZEL == true || g_bXM_DZFL == true || g_bXM_DZGL == true ||  g_bXM_RZ2L == true   || g_bXM_RZ3L == true || g_bXM_RZ7L == true || g_bXM_RZ8L == true 
                        || g_bXM_RZ9L == true || g_bXM_RZAL == true || g_bXM_RZBL == true || g_bXM_RZCL == true || g_bXM_WZ2L == true || g_bXM_WZ3L == true || g_bXM_WZ4L == true || g_bXM_WZ5L == true || g_bXM_WZ6L == true || g_bXM_WZ7L == true)
                    {
                        if (Convert.ToInt16(strBuf)!=260)
                        {
                            SetMsg("Raed Widevinekey length：" + strBuf + " and key setting length: 260 not match", UDF.COLOR.FAIL);
                            return false;
                        }
                    }
                    else if (g_bXM_DZ5L == true || g_bXM_DZJL == true || g_bXM_DZ7L == true || g_bXM_DZ8L == true || g_bXM_DZ9L == true || g_bXM_RZ4L == true || g_bXM_RZ5L == true || g_bXM_RZ6L == true  || g_bXM_DZHL == true || g_bXM_DZIL == true || g_bXM_DZLL == true
                        || g_bXM_DZKL == true || g_bXM_DZNL == true || g_bXM_DZML == true || g_bXM_DZQL == true )
                    {
                        if (Convert.ToInt16(strBuf) != 128)
                        {
                            SetMsg("Raed Widevinekey length：" + strBuf + " and key setting length : 128 not match", UDF.COLOR.FAIL);
                            return false;
                        }
                    }
                    else
                    {
                        SetMsg("This Model Widevinekey length not setting，Please Connect TE", UDF.COLOR.FAIL);
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    SetMsg("Read Widevine key ack fail" + ex.Message, UDF.COLOR.WORK);
                    if (iRetryTime > 3)
                        return false;
                    else
                    {
                        Thread.Sleep(500);
                        iRetryTime++;
                        goto ReReadWDkey;
                    }
                }
            }
            SetMsg("Widevine key size: " + strBuf, UDF.COLOR.WORK);
            return true;
        }
        //Check_hdcpkey_size

        bool Check_hdcpkey_size()
        {
            iRetryTime = 0;
            string strBuf = string.Empty;
            ReReadHDCP14key:
            strBuf = "";
            SetMsg("CHECK hdcp1.4 key size....", UDF.COLOR.WORK);
            StrToCmd("79 00 11 3D 00 00 01");//79 01 14 6D 00 00 01
            g_bRTACKFlag = false;
            strBuf = string.Empty;
            if (WriteRS232CommandMI(g_byCmdBuf, 9, 25, 21, out g_byCmdRtn) == false)
            {
                SetMsg("Read hdcp1.4 key size CMD fail", UDF.COLOR.FAIL);
                if (iRetryTime > 3)
                    return false;
                else
                {
                    Thread.Sleep(500);
                    iRetryTime++;
                    goto ReReadHDCP14key;
                }
            }
            else
            {
                if (g_byCmdRtn[13]==0)
                {
                    SetMsg("Read hdcp1.4 key Length fail", UDF.COLOR.FAIL);
                    if (iRetryTime > 3)
                        return false;
                    else
                    {
                        iRetryTime++;
                        goto ReReadHDCP14key;
                    }
                }
                try
                {
                    for (i = 0; i < g_byCmdRtn[13]; i++)
                    {
                        strBuf = strBuf + Convert.ToChar(g_byCmdRtn[i + 14]);
                    }

                    if (g_bXM_DZ3L == true || g_bXM_DZ4L == true || g_bXM_DZCL == true || g_bXM_DZDL == true || g_bXM_DZEL == true || g_bXM_DZFL == true || g_bXM_DZGL == true || g_bXM_RZ2L == true || g_bXM_RZ3L == true || g_bXM_RZ7L == true 
                        || g_bXM_RZ8L == true || g_bXM_RZ9L == true || g_bXM_RZAL == true || g_bXM_RZBL == true || g_bXM_RZCL == true  || g_bXM_WZ2L == true || g_bXM_WZ3L == true || g_bXM_WZ4L == true || g_bXM_WZ5L == true || g_bXM_WZ6L == true || g_bXM_WZ7L == true)
                    {
                        if (Convert.ToInt16(strBuf) != 304)
                        {
                            SetMsg("Read hdcp1.4 key Length ：" + strBuf + " and key Length: 304 not match", UDF.COLOR.FAIL);
                            return false;
                        }
                    }
                    else if (g_bXM_DZ5L == true || g_bXM_DZJL == true || g_bXM_DZ7L == true || g_bXM_DZ8L == true  || g_bXM_RZ4L == true || g_bXM_RZ5L == true || g_bXM_RZ6L == true   || g_bXM_DZHL == true || g_bXM_DZIL == true || g_bXM_DZLL == true
                        || g_bXM_DZKL == true || g_bXM_DZNL == true || g_bXM_DZML == true || g_bXM_DZQL == true )
                    {
                        if (Convert.ToInt16(strBuf) != 328)
                        {
                            SetMsg("Read hdcp1.4 key Length：" + strBuf + " and key Length:: 328  not match", UDF.COLOR.FAIL);
                            return false;
                        }
                    }
                    else if (g_bXM_DZ9L == true)  //'T982 烧录key长度是708，解密出来长度 544
                    {
                        if (Convert.ToInt16(strBuf) != 544)
                        {
                            SetMsg("Read hdcp1.4 key Length ：" + strBuf + "and setting key Length: 544 Not Match", UDF.COLOR.FAIL);
                            return false;
                        }
                    }
                    else
                    {
                        SetMsg("This hdcp1.4 key Length not setting，Please Connet TE", UDF.COLOR.FAIL);
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    SetMsg("Read hdcp1.4 key FAIL." + ex.Message, UDF.COLOR.FAIL);
                    return false;
                }
                SetMsg("hdcp1.4 key size: " + strBuf, UDF.COLOR.WORK);

                iRetryTime = 0;
                ReReadHDCP22key:
                strBuf = "";
                SetMsg("CHECK hdcp2.2 key size....", UDF.COLOR.WORK);
                StrToCmd("79 00 11 3E 00 00 01");//79 01 14 3E 00 00 01
                g_bRTACKFlag = false;
                strBuf = string.Empty;
                if (WriteRS232CommandMI(g_byCmdBuf, 9, 20, 21, out g_byCmdRtn) == false)
                {
                    SetMsg("Read hdcp2.2 key size CMD fail", UDF.COLOR.FAIL);
                    if (iRetryTime > 2)
                        return false;
                    else
                    {
                        Thread.Sleep(500);
                        iRetryTime++;
                        goto ReReadHDCP22key;
                    }
                }
                else
                {
                    if (g_byCmdRtn[13]==0 )
                    {
                        SetMsg("Read hdcp2.2 key length CMD fail", UDF.COLOR.FAIL);
                        if (iRetryTime > 2)
                            return false;
                        else
                        {
                            iRetryTime++;
                            goto ReReadHDCP22key;
                        }
                    }
                    try
                    {
                        for (i = 0; i < g_byCmdRtn[13]; i++)
                        {
                            strBuf = strBuf + Convert.ToChar(g_byCmdRtn[i + 14]);
                        }

                        if (g_bXM_DZ3L == true || g_bXM_DZ4L == true || g_bXM_DZCL == true || g_bXM_DZDL == true || g_bXM_DZEL == true || g_bXM_DZFL == true || g_bXM_DZGL == true || g_bXM_RZ2L == true || g_bXM_RZ3L == true || g_bXM_RZ7L == true 
                            || g_bXM_RZ8L == true || g_bXM_RZ9L == true || g_bXM_RZAL == true || g_bXM_RZBL == true | g_bXM_RZCL == true || g_bXM_WZ2L == true || g_bXM_WZ3L == true || g_bXM_WZ4L == true || g_bXM_WZ5L == true || g_bXM_WZ6L == true || g_bXM_WZ7L == true )
                        {
                            if (Convert.ToInt16(strBuf) != 1044)
                            {
                                SetMsg("Read hdcp2.2 key length ：" + strBuf + " setting key length : 1044 Not Match", UDF.COLOR.FAIL);
                                return false;
                            }
                        }
                        else if (g_bXM_DZ5L == true || g_bXM_DZJL == true || g_bXM_DZ7L == true || g_bXM_DZ8L == true  || g_bXM_RZ4L == true || g_bXM_RZ5L == true || g_bXM_RZ6L == true || g_bXM_DZHL == true || g_bXM_DZIL == true || g_bXM_DZLL == true
                            || g_bXM_DZKL == true || g_bXM_DZNL == true || g_bXM_DZML == true || g_bXM_DZQL == true )
                        {
                            if (Convert.ToInt16(strBuf) != 2080)
                            {
                                SetMsg("Read hdcp2.2 key length：" + strBuf + " setting  key length: 2080 Not Match", UDF.COLOR.FAIL);
                                return false;
                            }
                        }
                        else if (g_bXM_DZ9L == true)   //'T982 HDCP22 KEY长度长度 2302  解密出来是 1056
                        {
                            if (Convert.ToInt16(strBuf) != 1056)
                            {
                                SetMsg("Read hdcp2.2 key length：" + strBuf + " and setting key length: 1056 Not Match", UDF.COLOR.FAIL);
                                return false;
                            }
                        }
                        else
                        {
                            SetMsg("This Model hdcp2.2 key length not setting ，Please connect TE", UDF.COLOR.FAIL);
                            return false;
                        }
                    }
                    catch (Exception ex)
                    {
                        SetMsg("Read hdcp2.2 key Fail." + ex.Message, UDF.COLOR.FAIL);
                        return false;
                    }
                }
            }
            SetMsg("hdcp2.2 key size: " + strBuf, UDF.COLOR.WORK);
            return true;
        }

        bool Check_wifi_mac()
        {
            iRetryTime = 0;
            string strBuf = string.Empty;
            ReCheckWifiMAC:
            SetMsg("CHECK SYSTEM WIFI MAC....", UDF.COLOR.WORK);
            StrToCmd("79 00 14 89 00 00 01");//79 01 14 89 00 00 01
            g_bRTACKFlag = false;
            strBuf = string.Empty;
            if (WriteRS232CommandMI(g_byCmdBuf, 9, 50, 36, out g_byCmdRtn) == false)
            {
                SetMsg("CHECK SYSTEM WIFI CMD fail", UDF.COLOR.FAIL);
                if (iRetryTime > 3)
                    return false;
                else
                {
                    Thread.Sleep(500);
                    iRetryTime++;
                    goto ReCheckWifiMAC;
                }
            }
            else
            {
                if (g_byCmdRtn[13] != 17)
                {
                    SetMsg("CHECK SYSTEM WIFI MAC 长度错误", UDF.COLOR.FAIL);
                    if (iRetryTime > 3)
                        return false;
                    else
                    {
                        iRetryTime++;
                        goto ReCheckWifiMAC;
                    }
                }
                
                for (i = 0; i < g_byCmdRtn[13]; i++)
                {
                    strBuf = strBuf + Convert.ToChar(g_byCmdRtn[i + 14]);
                }
                SDriverX.g_modelInfo.sWifiMac= strBuf.Trim().Replace(":", "");
            }
            iMacIndex = 1;
            SetMsg("GET WIFI MAC: " + SDriverX.g_modelInfo.sWifiMac, UDF.COLOR.WORK);
            if (Insert_MI_MAC(iMacIndex)==false) return false;
            return true;
        }

        bool Check_bt_mac()
        {
            iRetryTime = 0;
            string strBuf = string.Empty;
            ReOpenBT:
            strBuf = "";
            SetMsg("SET BT STATUS ON....", UDF.COLOR.WORK);
            StrToCmd("79 00 11 2E 01 30 00 01"); //79 00 11 2E 01 30 00 01
            g_bRTACKFlag = true;
            strRestart = string.Empty;
            if (WriteRS232CommandMI(g_byCmdBuf,10, 20, 19, out g_byCmdRtn) == false)
            {
                SetMsg("set open bt cmd fail", UDF.COLOR.FAIL);
                if (iRetryTime > 2)
                    return false;
                else
                {
                    Thread.Sleep(500);
                    iRetryTime++;
                    goto ReOpenBT;
                }
            }
            Delay(500);
            ReCheckBTMAC:
            SetMsg("CHECK SYSTEM BT MAC....", UDF.COLOR.WORK);
            StrToCmd("79 00 14 40 00 00 01");//79 01 14 40 00 00 01
            g_bRTACKFlag = false;
            strBuf = string.Empty;
            if (WriteRS232CommandMI(g_byCmdBuf, 9, 20, 36, out g_byCmdRtn) == false)
            {
                SetMsg("CHECK SYSTEM BT CMD fail", UDF.COLOR.FAIL);
                if (iRetryTime > 3)
                    return false;
                else
                {
                    iRetryTime++;
                    goto ReCheckBTMAC;
                }
            }
            else
            {
                if (g_byCmdRtn[13] != 17)
                {
                    SetMsg("CHECK SYSTEM BT MAC Length fail", UDF.COLOR.FAIL);
                    if (iRetryTime > 3)
                        return false;
                    else
                    {
                        iRetryTime++;
                        goto ReCheckBTMAC;
                    }
                }

                for (i = 0; i < g_byCmdRtn[13]; i++)
                {
                    strBuf = strBuf + Convert.ToChar(g_byCmdRtn[i + 14]);
                }
                SDriverX.g_modelInfo.sBtMac = strBuf.Trim().Replace(":", "");
            }
            iMacIndex = 2;
            SetMsg("GET BT MAC: " + SDriverX.g_modelInfo.sBtMac, UDF.COLOR.WORK);
            if (Insert_MI_MAC(iMacIndex) == false) return false;
            return true;
        }

        bool Check_SetBurnInOn()
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
        bool Check_emmc_Lefttime()
        {
            iRetryTime = 0;
            string strBuf = string.Empty;
            ReCheckLeft:
            strBuf = "";
            SetMsg("CHECK EMMC LEFTTIME....", UDF.COLOR.WORK);
            StrToCmd("79 01 14 A4 00 00 01"); //79 01 14 A4 00 00 01 46 FE
            g_bRTACKFlag =false ;
            strRestart = string.Empty;
            if (WriteRS232CommandMI(g_byCmdBuf, 9, 20, 24, out g_byCmdRtn) == false)
            {
                SetMsg("CHECK EMMC LEFTTIME cmd fail", UDF.COLOR.FAIL);
                if (iRetryTime > 2)
                    return false;
                else
                {
                    Thread.Sleep(500);
                    iRetryTime++;
                    goto ReCheckLeft;
                }
            }
            else
            {
                if (g_byCmdRtn[13]==0 )
                {
                    SetMsg("CHECK EMMC LEFTTIME Length fail", UDF.COLOR.FAIL);
                    if (iRetryTime > 2)
                        return false;
                    else
                    {
                        iRetryTime++;
                        goto ReCheckLeft;
                    }
                }
                for (i = 0; i < g_byCmdRtn[13]; i++)
                {
                    strBuf = strBuf + Convert.ToChar(g_byCmdRtn[i + 14]);
                }
                switch (strBuf)
                {
                    case "0x01/0x00":
                        break;
                    case "0x01 0x00":
                        break;
                    case "0x01|0x00":
                        break;
                    case "0x00/0x01":
                        break;
                    case "0x00 0x01":
                        break;
                    case "0x00|0x01":
                        break;
                    case "0x01/0x01":
                        break;
                    case "0x01 0x01":
                        break;
                    case "0x01|0x01":
                        break;
                    default:
                        SetMsg("CHECK EMMC LEFTTIME FIAL " + strBuf , UDF.COLOR.FAIL);
                        return false;
                }
            }
            return true;
        }
        bool Check_SetBurnInOff()
        {
            iRetryTime = 0;
            ReBIMODEOff:
            SetMsg("SET AGING MODE OFF....", UDF.COLOR.WORK);
            StrToCmd("79 00 14 81 00 00 01");
            g_bRTACKFlag = true;
            strRestart = string.Empty;
            if (WriteRS232CommandMI(g_byCmdBuf, 9, 30, 19, out g_byCmdRtn) == false)
            {
                SetMsg("SET BI MODE OFF CMD fail", UDF.COLOR.FAIL);
                if (iRetryTime > 2)
                    return false;
                else
                {
                    Thread.Sleep(500);
                    iRetryTime++;
                    goto ReBIMODEOff;
                }
            }
            Delay(200); 
            return true;
        }

        bool Check_SetPattern_ON()
        {
            iRetryTime = 0;
            string strBuf = string.Empty; 
            strBuf = ClassFileIO.ReadIniFile("Function", "SJ_TEST", ClassFileIO.sIniPath);
            if (strBuf.Trim().ToUpper()!="Y")
            {
                return true;
            }
            RePTN_ON:
            SetMsg("SET PATTERN ON....", UDF.COLOR.WORK);
            StrToCmd("79 00 12 25 0B 32 35 35 3A 32 35 35 3A 32 35 35 00 01");
            g_bRTACKFlag = true;
            strRestart = string.Empty;
            if (WriteRS232CommandMI(g_byCmdBuf, 20, 30, 19, out g_byCmdRtn) == false)
            {
                SetMsg("SET PATTERN ON CMD fail", UDF.COLOR.FAIL);
                if (iRetryTime > 2)
                    return false;
                else
                {
                    Thread.Sleep(200);
                    iRetryTime++;
                    goto RePTN_ON;
                }
            }
            //Delay(100);
            IO_Card_Output("3", 200);
            return true;
        }

        bool Check_SetPatternON()
        {
            iRetryTime = 0;
            RePTN_ON:
            SetMsg("SET PATTERN ON....", UDF.COLOR.WORK);
            StrToCmd("79 00 12 25 0B 32 35 35 3A 32 35 35 3A 32 35 35 00 01");
            g_bRTACKFlag = true;
            strRestart = string.Empty;
            if (WriteRS232CommandMI(g_byCmdBuf, 20, 30, 19, out g_byCmdRtn) == false)
            {
                SetMsg("SET PATTERN ON CMD fail", UDF.COLOR.FAIL);
                if (iRetryTime > 2)
                    return false;
                else
                {
                    Thread.Sleep(200);
                    iRetryTime++;
                    goto RePTN_ON;
                }
            }
            return true;
        }

        bool Check_Panel_test()
        {
            string strBuf = string.Empty;
            string strMessage = string.Empty;
            Autosjtest:
            strBuf = ClassFileIO.ReadIniFile("Function", "SJ_TEST", ClassFileIO.sIniPath);
            if (strBuf.Trim().ToUpper() == "Y" && g_bAutoScan==true )
            {
                if (SJ_TEST() == false)
                {
                    strMessage = "请确认当前白画面是否正常？";
                    DialogResult Dr;
                    Dr = (MessageBox.Show(strMessage, "提示", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question));

                    if (Dr == DialogResult.Yes)
                    {
                        Application.DoEvents();
                    }
                    else if (Dr == DialogResult.No )
                    {
                        goto Autosjtest;
                    }
                    else
                        return false;
                }
                else
                {
                    SetMsg("画面自动检查PASS....", UDF.COLOR.PASS);
                    Check_SetPattern_OFF();
                }
            }
            return true;
        }

        bool SJ_TEST()
        {
            iRetryTime = 0;
            g_bSj_check_ng = false;
            g_bSj_check_pass = false;
            timerIO_CARD.Enabled = true;
            Re_SJ:
            SetMsg("摄像头全白画面检查....", UDF.COLOR.WORK);
            if (g_bSj_check_ng==true)
            {
                SetMsg("摄像头全白画面检查显示NG....", UDF.COLOR.FAIL);
                return false;
            }
            if (g_bSj_check_pass == false)
            {
                Application.DoEvents();
                Thread.Sleep(200);
                if (iRetryTime >30)
                {
                    SetMsg("摄像头全白画面检查超时....", UDF.COLOR.FAIL);
                    return false;
                }
                else
                {
                    iRetryTime++;
                    goto Re_SJ;
                }
            }
            return true;
        }
        bool Check_SetPattern_OFF()
        {
            iRetryTime = 0;
            RePTN_OFF:
            SetMsg("SET PATTERN OFF....", UDF.COLOR.WORK);
            StrToCmd("79 00 12 2B 00 00 01");
            g_bRTACKFlag = true;
            strRestart = string.Empty;
            if (WriteRS232CommandMI(g_byCmdBuf, 9, 20, 19, out g_byCmdRtn) == false)
            {
                SetMsg("SET PATTERN OFF CMD fail", UDF.COLOR.FAIL);
                if (iRetryTime > 2)
                    return false;
                else
                {
                    Thread.Sleep(500);
                    iRetryTime++;
                    goto RePTN_OFF;
                }
            }
            Delay(100);
            return true;
        }
    }
}

