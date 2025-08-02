using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using PRETEST.UDFs;
using PRETEST.WebReference1;
using PRETEST.AppConfig;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Threading;

namespace PRETEST.SDriver
{
    public class SDriverX
    {
        static bool g_bRecvFlag = false;     //服务器返回消息开关
        static string g_sRecvMsg = "";    //服务器返回的消息
        public static UDF.ModelInfo g_modelInfo = new UDF.ModelInfo();   //查询的机种信息

        public static WebReference1.AVTC_webservice webService = new AVTC_webservice();
        #region SDriver 相关
        ///*++++++++++++++++++++++++++++++++++++++++++++++++++++
        //* SDriver 机种测试流程卡相关,查询或传递 PASS 信息
        //* --------------------------------------------------*/
        /// <summary>
        /// 初始化 SDriver
        /// </summary>
        /// <returns></returns>
        public static bool AVTCInit()
        {
            // AVTC webserver IP
            string host = "10.2.100.72"; 
            Ping pingSender = new Ping();

            try
            {
                PingReply reply = pingSender.Send(host, 1000); // timeout 1000ms

                if (reply.Status == IPStatus.Success)
                {
                    return true;
                    //Console.WriteLine($"Ping {host} 成功！");
                    //Console.WriteLine("地址: " + reply.Address.ToString());
                    //Console.WriteLine("往返時間: " + reply.RoundtripTime + " ms");
                    //Console.WriteLine("TTL: " + reply.Options.Ttl);
                    //Console.WriteLine("是否分段: " + reply.Options.DontFragment);
                    //Console.WriteLine("緩衝區大小: " + reply.Buffer.Length);
                }
                else
                {
                    //Console.WriteLine($"Ping {host}失敗：" + reply.Status);
                    return false;
                }
            }
            catch
            {
                return false;
            }
            
        }
        /// <summary>
        /// 站别查询函数,并返回机种的相关信息
        /// </summary>
        /// <param name="meseid">MESEID</param>
        /// <param name="sn">机台SN 序列号</param>
        /// <param name="user">用户名</param>
        /// <returns></returns>
        /// 
        public static bool STCK(string meseid, string sn, string user)
        {
            int iRetryTime = new int();
            UDF.ModelInfo modelInfoTmp = new UDF.ModelInfo();

            g_modelInfo = modelInfoTmp;
            g_bRecvFlag = false;

            if (GlobalConfig.iRunMode == 1)
            {
                iRetryTime = 0;
                do
                {
                    g_sRecvMsg = webService.CHK_MES_ROUTE(meseid, sn);
                    if (g_sRecvMsg != null)
                    {
                        if (Regex.IsMatch(g_sRecvMsg, "result:true"))
                        {
                            g_bRecvFlag = true;
                        }
                        else
                        {
                            modelInfoTmp.sErr_msg = g_sRecvMsg;
                            g_modelInfo = modelInfoTmp;
                            return false;
                        }
                    }
                    if (iRetryTime > 100) return false;
                    iRetryTime++;
                    Delay(100);
                } while (!g_bRecvFlag);

                modelInfoTmp.sSn = sn;
                modelInfoTmp.sMode =            Regex.Match(g_sRecvMsg, "\"sMode\":\"(\\S+)\",\"sTv_offline\"").Groups[1].Value;
                modelInfoTmp.sTv_offline =      Regex.Match(g_sRecvMsg, "\"sTv_offline\":\"(\\S+)\",\"sFinish_type\"").Groups[1].Value;
                modelInfoTmp.sFinish_type =     Regex.Match(g_sRecvMsg, "\"sFinish_type\":\"(\\S+)\",\"sProductname\"").Groups[1].Value;
                modelInfoTmp.sProductname =     Regex.Match(g_sRecvMsg, "\"sProductname\":\"(\\S+)\",\"sWord\"").Groups[1].Value;
                modelInfoTmp.sWord =            Regex.Match(g_sRecvMsg, "\"sWord\":\"(\\S+)\",\"sPart_no\"").Groups[1].Value;
                modelInfoTmp.sPart_no =         Regex.Match(g_sRecvMsg, "\"sPart_no\":\"(\\S+)\",\"sModel\"").Groups[1].Value;
                modelInfoTmp.sModel =           Regex.Match(g_sRecvMsg, "\"sModel\":\"(\\S+)\",\"sSimple_model\"").Groups[1].Value;
                modelInfoTmp.sSimple_model =    Regex.Match(g_sRecvMsg, "\"sSimple_model\":\"(\\S+)\",\"model_code\"").Groups[1].Value;
                modelInfoTmp.sRtn_cd =          Regex.Match(g_sRecvMsg, "\"sRtn_cd\":\"(\\S+)\",\"sErr_eng_msg\"").Groups[1].Value;
                modelInfoTmp.sErr_eng_msg =     Regex.Match(g_sRecvMsg, "\"sErr_eng_msg\":\"(\\S+)\",\"sErr_msg\"").Groups[1].Value;
                modelInfoTmp.sErr_msg =         Regex.Match(g_sRecvMsg, "\"sErr_msg\":\"(\\S+)\",\"sSn\"").Groups[1].Value;
                modelInfoTmp.sUseFlag = 0;
            }
            else
            {
                //以下都是run mode = 0測試使用
                modelInfoTmp.sSn = sn;
                //modelInfoTmp.sMode = "AUTO";
                //modelInfoTmp.sTv_offline = "N";
                //modelInfoTmp.sFinish_type = "FGD";
                //modelInfoTmp.sProductname = "P54E0779-TV-VN100001";
                //modelInfoTmp.sWord = "P54E0779-TV-VN1";
                //modelInfoTmp.sPart_no = "T55USTY22AO-.TY55013";
                //modelInfoTmp.sModel = "LED TV 55\"+48.5\" TY55-1 (Amlogic A311D2-B0D+AUO T550QVN10.0 + AUO P485IVN02.0) (US) (AVTC)";
                //modelInfoTmp.sSimple_model = null;
                //modelInfoTmp.sRtn_cd = "0";
                //modelInfoTmp.sErr_eng_msg = null;
                //modelInfoTmp.sErr_msg = null;
                //modelInfoTmp.sUseFlag = 0;
                modelInfoTmp.sMode = "";
                modelInfoTmp.sTv_offline = "";
                modelInfoTmp.sFinish_type = "";
                modelInfoTmp.sProductname = "";
                modelInfoTmp.sWord = "";
                modelInfoTmp.sPart_no = "";
                modelInfoTmp.sModel = "";
                modelInfoTmp.sSimple_model = null;
                modelInfoTmp.sRtn_cd = "0";
                modelInfoTmp.sErr_eng_msg = null;
                modelInfoTmp.sErr_msg = null;
                modelInfoTmp.sUseFlag = 0;
            }

            g_modelInfo = modelInfoTmp;
            if (modelInfoTmp.sPart_no == string.Empty) return false;

            return true;
        }

        public static bool INSERT_ROUTE_Webservice(string meseid, string sn, string user)
        {
            string msg = string.Empty;
            UDF.ModelInfo modelInfoTmp = new UDF.ModelInfo();

            g_modelInfo = modelInfoTmp;
            g_bRecvFlag = false;
            g_sRecvMsg = webService.INS_MES_STATION(meseid, sn, user, "N/A");

            if (CheckResult(g_sRecvMsg, "result:true") == false)
            {
                modelInfoTmp.sErr_msg = g_sRecvMsg;
                g_modelInfo = modelInfoTmp;
                return false;
            }
            return true;
        }

        public static bool CheckResult(string strSrouce, string pattern)
        {
            return Regex.Match(strSrouce, pattern).Success;
        }
        /// <summary>
        /// 机台序列号过站函数
        /// </summary>
        /// <param name="meseid">MESEID</param>
        /// <param name="sn">机台 SN 序列号</param>
        /// <param name="user">用户名</param>
        /// <param name="pf">'P' Or 'F'(PASS OR FAIL)</param>
        /// <param name="comment">注释</param>
        /// <returns></returns>
        public static bool WBER(string meseid, string sn, string user, string pf, string comment)
        {
            int iRetryTime = new int();
            //int i = new int();
            string msg = string.Empty;

            g_bRecvFlag = false;
            g_sRecvMsg = "";
            msg = "WBER EQP=" + meseid + " SET_SERIAL_NO=" + sn + " PF=" + pf + " USER=" + user + " TXN_DATE=(" + System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + ") COMMENT=(NoCMT)";
            //g_winsock.SDsendMessage(msg);
            iRetryTime = 0;
            do
            {
                if (iRetryTime > 100) return false;
                iRetryTime++;
                Delay(100);
            } while (!g_bRecvFlag);
            g_modelInfo.sMode = Regex.Match(g_sRecvMsg, @"(?<= MODE=).*(?= HOST_DATE=)").ToString().Trim();
            g_modelInfo.sProductname = Regex.Match(g_sRecvMsg, @"(?<= PRODUCTNAME=).*(?= RTN_CD=)").ToString().Trim();
            g_modelInfo.sRtn_cd = Regex.Match(g_sRecvMsg, @"(?<= RTN_CD=).*(?= ERR_ENG_MSG=)").ToString().Trim();
            g_modelInfo.sErr_eng_msg = Regex.Match(g_sRecvMsg, @"(?<= ERR_ENG_MSG=\().*(?=\) ERR_MSG=)").ToString().Trim();
            g_modelInfo.sErr_msg = Regex.Match(g_sRecvMsg, @"(?<= ERR_MSG=\().*(?=\))").ToString().Trim();
            if (g_modelInfo.sRtn_cd != "0") return false;

            return true;
        }
        /// <summary>
        /// 延时函数,单位毫秒,精度 40毫秒
        /// </summary>
        /// <param name="milliSecond">要延时的毫秒数</param>
        public static void Delay(int milliSecond)
        {
            if (milliSecond <= 0) return;
            int i = new int();
            int j = (int)(milliSecond / 40);
            do
            {
                i++;
                System.Windows.Forms.Application.DoEvents();
                System.Threading.Thread.Sleep(40);
            } while (i < j);
        }
        #endregion
    }
}
