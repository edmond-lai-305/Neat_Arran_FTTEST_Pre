using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Text;
using PRETEST.TestProcess.FUD;
//using System.Diagnostics;
//using System.Windows.Forms;
using PRETEST.AppConfig;
 

namespace PRETEST.TestProcess.FUD
{
    class FGA { }
}
namespace PRETEST
{

    public partial class FormMain
    {
        private static int g_pMainHwnd = 0;

        //private static int g_pTextHwnd = 0;
        [DllImport("User32.dll")]
        public static extern string GetWindowText(int hwnd,string lpClassName, int cch);

        [DllImport("User32.dll")]
        public static extern int FindWindow(string lpClassName, string lpWindowName);

        [DllImport("User32.dll")]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpClassName, string lpWindowName);

        [DllImport("User32.dll")]
        public static extern bool PostMessage(int hWnd, uint msg, uint wParam, uint lParam);

        [DllImport("User32.dll")]
        public static extern bool IsWindow(IntPtr hWnd);

        [DllImport("User32.dll")]
        public static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, int modifiers, Keys vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);
        [DllImport("TDXK.DLL")]
        private static extern int FindControl(int hwnd, string className, int index);

        [DllImport("TDXK.DLL")]
        private static extern bool ControlClick(int hwnd);
        [DllImport("TDXK.DLL")]
        private static extern bool SetControlTextForInput(int hwnd, string lpcszText, bool SendReturnKey);

        [DllImport("TDXK.DLL")]
        private static extern string GetControlTextForVB(int hwnd, string className, int index);

        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, string lParam);
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hwnd, int msg, int p1, int p2);
        [DllImport("user32.dll", EntryPoint = "SendMessageA")]
        public static extern int SendMessage(IntPtr hwnd, int wMsg, int wParam, Byte[] lParam);
        [DllImport("user32.dll")] static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, int wParam, StringBuilder lParam);
        public const int WM_GETTEXTLENGTH = 0x0E;
        public const int WM_GETTEXT = 0x0D;
        public const int VM_KEYDOWN = 0x100;
        public const int WM_KEYUP = 0x101;
        public const int VM_SYSKEYWOWN = 0x104;
        public const int VK_RETURN = 0xD;
        public const int WM_SETTEXT =0xC;
        public const int LB_GETCOUNT = 0x018B;
        public const int LB_GETTEXT = 0x0189;
        public bool FGA_TEST(  )
        {
            int retrytime = 0;
            int retrytime1 = 0;
            string strTEXT = "";
            int TextLen = 100;
            Byte[] byt = new Byte[TextLen];
            reopen:
            System.Threading.Thread.Sleep(5);
            //bool SendReturn = false;
            string lpClassName = "#32770";
            string lpWindowName = "RKDevTool v2.65";
            string result = "Reset Device Success";
            if(result =="0" )
            {
                result = "Reset Device Success";
            }
             
            if (lpClassName=="0"|| lpWindowName =="0")
            {
                 
                    lpClassName = "#32770";
                    lpWindowName = "RKDevTool v2.65";
                
            }
            reopen1:
            g_pMainHwnd = FindWindow(lpClassName, lpWindowName);
            //int hWndX4 = FindControl(g_pMainHwnd, "Edit", 4);
            //SetControlTextForInput(hWndX4, @"D:\Y3\ARRAN_FW_Release_NFA1.20211012.0955_386672040_EOL\neat_arran_flash_factory\fw\532182524_update-user-secure_NFA1.20220505.1717.img", SendReturn);
            if (g_pMainHwnd != 0)
            {
                SetMsg("wait LOADER Device...", UDFs.UDF.COLOR.WORK);
             wt:
                int hWndX1 = FindControl(g_pMainHwnd, "Static", 1);
                System.Threading.Thread.Sleep(50);
                SendMessage((IntPtr)hWndX1, WM_GETTEXT, TextLen + 1, byt);
                string str = Encoding.Default.GetString(byt).Replace("\0", "");
                strTEXT = str;
                int hWndX2 = FindControl(g_pMainHwnd, "Button", 8);
                if (strTEXT.Contains("LOADER"))
                {
                    Process[] pro = Process.GetProcessesByName("cmd");
                    for (int i = 0; i < pro.Length; i++)
                    {
                        pro[i].Kill();
                    }
                    SetMsg("Found One LOADER Device", UDFs.UDF.COLOR.WORK);
                    SetMsg("烧录中，请等待", UDFs.UDF.COLOR.WORK);
                    System.Threading.Thread.Sleep(50);
                    ControlClick(hWndX2);
                    //System.Threading.Thread.Sleep(50);
                    //ControlClick(hWndX2);
                }else
                { 
                    SetMsg("No Devices，请等待", UDFs.UDF.COLOR.WORK);
                    this.Refresh();
                    retrytime1++;
                    System.Threading.Thread.Sleep(500);
                    Application.DoEvents();
                    if (retrytime1 < 30)
                    { goto reopen1; }
                    if (retrytime1 < 100)
                    { goto wt; }
                    SetMsg("设备未连接或重启机台", UDFs.UDF.COLOR.FAIL);
                    //KillFudAllProcess();
                    return false;
                }
                retrytime1 = 0;
                int hWndX3 = FindControl(g_pMainHwnd, "ListBox", 1);
               // SetControlTextForInput(hWndX1, "12", SendReturn);
                //string m = "";
                //SendMessage((IntPtr)hWndX3, WM_GETTEXTLENGTH, 0, byt);
                //m = GetControlTextForVB(hWndX3, "ListBox", 1);
                //StringBuilder title = new StringBuilder(1);
                ////获取长度
                //Int32 Len = SendMessage((IntPtr)hWndX3, WM_GETTEXTLENGTH, 0, title).ToInt32();
                //StringBuilder DataStr = new StringBuilder(Len + 1);
                //SendMessage((IntPtr)hWndX3, WM_GETTEXT, DataStr.Capacity, DataStr);
                //Int32 Len = SendMessage((IntPtr)hWndX3, LB_GETCOUNT, 0, 0);
                //StringBuilder DataStr1 = new StringBuilder(256);
                //SendMessage((IntPtr)hWndX3, LB_GETTEXT, 0, DataStr1);
                //textBoxAck.AppendText(DataStr1.Replace("\0", "").Replace("\u0001","").Replace("\u0002", "") + "\r\n");
                Int32 Len_LAST=0;
                do
                {
                    System.Threading.Thread.Sleep(50);
                    retrytime++;
                    Application.DoEvents();                  
               
                    Int32 Len = SendMessage((IntPtr)hWndX3, LB_GETCOUNT, 0, 0);
                   
                    StringBuilder DataStr1 = new StringBuilder(256);
                    if (Len%2 ==0 &&(Len!= Len_LAST) || Len>14)
                    {
                        textBoxAck.Clear();
                        for (int I=0;I< Len;I++)
                        { SendMessage((IntPtr)hWndX3, LB_GETTEXT, I, DataStr1);
                            strTEXT = DataStr1.ToString().Replace("\0", "").Replace("\u0001", "").Replace("\u0002", "");
                            textBoxAck.AppendText(strTEXT + "\r\n");
                        }
                    }
                    Len_LAST = Len;
                    System.Threading.Thread.Sleep(500);
                    
                    if (strTEXT.ToUpper().Contains("NO DEVICE")|| strTEXT.ToUpper().Contains("FAIL"))
                    {
                         SetMsg("No Devices,通讯问题", UDFs.UDF.COLOR.WORK);
                        // FrmCT.Frm.setMsg("OGC TEST 不通讯 FAIL...");
                        // FrmCT.Frm.setMsg1("OGC TEST 不通讯 FAIL");
                        if (retrytime1 < 6)
                        {
                            retrytime1++;
                            goto reopen1;
                        }
                        return false;
                    }
                    if(strTEXT == "")
                    {
                        if (retrytime1 < 3)
                        {
                            retrytime1++;
                            goto reopen;
                        }
                    }
                }
                while (strTEXT != result);
                //return true;
            }
            else
            {
                //FrmCT.Frm.setMsg("厂商程序没有打开。");
               // FrmCT.Frm.setMsg1("厂商程序没有打开");
                 MessageBox.Show("确认fwtool程序有没有打开。");
                //if (retrytime < 20)
                //{
                //    retrytime++;
                //  //  SDriverX.Delay1(100);
                //    goto reopen;
                //}
               // Definition.PJ_Data.LampSFC = true;
                return false;
            }
            return true;
        }
       
    }
}
