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
    class xm_hw_cs
    {
    }
}
namespace PRETEST
{
    public partial class FormMain
    {

        bool PRE_XM_HW_CS()
        {
            string strBuf = string.Empty;
            string strSN2 = string.Empty;
            iRetryTime = 0;
            SetMsg(DateTime.Now.ToString(), UDF.COLOR.WORK);    //DEBUG

            if (Check_Abnormal_Restart() == false) return false;
            if (Check_MI_FW() == false) return false;
            if (Check_MI_TAG() == false) return false;
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
            SetMsg(DateTime.Now.ToString(), UDF.COLOR.WORK);    //DEBUG
            return true;
        }
    }
}