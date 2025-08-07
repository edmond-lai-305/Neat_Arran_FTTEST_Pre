using Oracle.ManagedDataAccess.Client;
using PRETEST;
using PRETEST.AppConfig;
using PRETEST.Database;
using PRETEST.SDriver;
using PRETEST.UDFs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;


namespace PRETEST.UDFs
{
    public class API { }

}

namespace PRETEST
{
    public partial class FormMain
    {
        /// <summary>
        /// 延时函数,单位毫秒,精度 40毫秒
        /// </summary>
        /// <param name="milliSecond">要延时的毫秒数</param>
        private bool isDrawItemHooked = false;

        public int cmd_Delay;
        public static void Delay(int milliSecond)
        {
            if (milliSecond <= 0) return;
            int i = new int();
            int j = (int)(milliSecond / 40);
            do
            {
                i++;
                System.Windows.Forms.Application.DoEvents();
                Thread.Sleep(40);
            } while (i < j);
        }
        public DateTime GetComputerStartTime()
        {
            int result = Environment.TickCount & Int32.MaxValue;     //获取系统启动后运行的毫秒数
            TimeSpan m_WorkTimeTemp = new TimeSpan(Convert.ToInt64(Convert.ToInt64(result) * 10000));
            this.toolStripStatusLabel3.Text ="系统开机时间：" + Convert.ToString (m_WorkTimeTemp +"秒");
            DateTime startTime = System.DateTime.Now.AddDays(m_WorkTimeTemp.Days);
            startTime = startTime.AddHours(-m_WorkTimeTemp.Hours);
            startTime = startTime.AddMinutes(-m_WorkTimeTemp.Minutes);
            startTime = startTime.AddSeconds(-m_WorkTimeTemp.Seconds);
            //MessageBox.Show(startTime.ToString());
            return startTime;   //返回转换后的时间
        }
        /// <summary>
        /// 写log文件
        /// </summary>
        /// <param name="fileName">文件名(不包含路径)</param>
        /// <param name="text">文本信息</param>
        public static void WriteLog(string logpath, string text)
        {
            Delay(20);
            StreamWriter sWrite = new StreamWriter(logpath, true);
            sWrite.WriteLine(text);
            sWrite.Close();
        }
        /// <summary>
        /// 清空流程信息显示
        /// </summary>
        private void ClearMsg()
        {
            if(listBoxSetup.InvokeRequired)
            {
                listBoxSetup.BeginInvoke(new Action(() =>
                {
                    listBoxSetup.Items.Clear();
                }));
            }
            else
            {
                listBoxSetup.Items.Clear();
            }
            return;
        }
        /// <summary>
        /// 消息通知函数，更新界面提示
        /// </summary>
        /// <param name="msg">要显示的提示信息</param>
        /// <param name="col">自定义的工作颜色</param>
        private void SetMsg(string msg, UDF.COLOR col, int option = 1)
        {
            if (msg.Length > 16)
            {
                labelMsgtip.Font = new System.Drawing.Font("宋体", g_fHeightScaling * 18F, System.Drawing.FontStyle.Bold);
            }
            else
            {
                labelMsgtip.Font = new System.Drawing.Font("宋体", g_fHeightScaling * 30F, System.Drawing.FontStyle.Bold);
            }

            // 設定文字
            if (labelMsgtip.InvokeRequired)
            {
                labelMsgtip.Invoke(new Action(() =>
                {
                    labelMsgtip.Text = msg;
                    switch (col)
                    {
                        case UDF.COLOR.PASS:
                            labelMsgtip.BackColor = System.Drawing.Color.FromArgb(136, 255, 136);
                            break;
                        case UDF.COLOR.FAIL:
                            labelMsgtip.BackColor = System.Drawing.Color.FromArgb(255, 136, 136);
                            break;
                        case UDF.COLOR.WORK:
                            labelMsgtip.BackColor = System.Drawing.Color.FromArgb(128, 128, 255);
                            break;
                        case UDF.COLOR.WORK2:
                            labelMsgtip.BackColor = System.Drawing.Color.FromArgb(163, 73, 164);
                            break;
                        case UDF.COLOR.WARN:
                            labelMsgtip.BackColor = System.Drawing.Color.FromArgb(255, 201, 14);
                            break;
                        default:
                            break;
                    }
                }));
            }
            else
            {
                labelMsgtip.Text = msg;

                // 設定背景顏色
                switch (col)
                {
                    case UDF.COLOR.PASS:
                        labelMsgtip.BackColor = System.Drawing.Color.FromArgb(136, 255, 136);
                        break;
                    case UDF.COLOR.FAIL:
                        labelMsgtip.BackColor = System.Drawing.Color.FromArgb(255, 136, 136);
                        break;
                    case UDF.COLOR.WORK:
                        labelMsgtip.BackColor = System.Drawing.Color.FromArgb(128, 128, 255);
                        break;
                    case UDF.COLOR.WORK2:
                        labelMsgtip.BackColor = System.Drawing.Color.FromArgb(163, 73, 164);
                        break;
                    case UDF.COLOR.WARN:
                        labelMsgtip.BackColor = System.Drawing.Color.FromArgb(255, 201, 14);
                        break;
                    default:
                        break;
                }
            }

            // ListBox 設定
            if(listBoxSetup.InvokeRequired)
            {
                listBoxSetup.Invoke(new Action(() =>
                {
                    listBoxSetup.DrawMode = DrawMode.OwnerDrawFixed;
                    if (!isDrawItemHooked)
                    {
                        listBoxSetup.DrawItem -= listBoxSetup_DrawItem; // 避免事件重複
                        listBoxSetup.DrawItem += listBoxSetup_DrawItem;
                        isDrawItemHooked = true;
                    }
                    listBoxSetup.Items.Add(DateTime.Now.ToString("yyyy-MM-dd_HH:mm:ss") + "\t" + msg);
                    listBoxSetup.SelectedIndex = listBoxSetup.Items.Count - 1;
                }));
            }
            else
            {
                listBoxSetup.DrawMode = DrawMode.OwnerDrawFixed;
                if (!isDrawItemHooked)
                {
                    listBoxSetup.DrawItem -= listBoxSetup_DrawItem; // 避免事件重複
                    listBoxSetup.DrawItem += listBoxSetup_DrawItem;
                    isDrawItemHooked = true;
                }
                listBoxSetup.Items.Add(DateTime.Now.ToString("yyyy-MM-dd_HH:mm:ss") + "\t" + msg);
                listBoxSetup.SelectedIndex = listBoxSetup.Items.Count - 1;
            }

            if (option == 1) Savelog(msg);

            return;
        }
        private void listBoxSetup_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;

            string itemText = listBoxSetup.Items[e.Index].ToString();

            // 預設顏色
            Color backColor = e.BackColor;
            Color foreColor = e.ForeColor;

            if (itemText.Contains("PASS"))
            {
                backColor = Color.Green;
                foreColor = Color.White;
            }
            else if (itemText.Contains("FAIL"))
            {
                backColor = Color.Red;
                foreColor = Color.White;
            }
            else if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
            {
                // 如果選取狀態，使用系統 Highlight
                backColor = SystemColors.Highlight;
                foreColor = SystemColors.HighlightText;
            }

            // 畫背景
            using (SolidBrush backgroundBrush = new SolidBrush(backColor))
            {
                e.Graphics.FillRectangle(backgroundBrush, e.Bounds);
            }

            // 畫文字
            using (SolidBrush textBrush = new SolidBrush(foreColor))
            {
                e.Graphics.DrawString(itemText, e.Font, textBrush, e.Bounds);
            }

            // 畫框線
            e.DrawFocusRectangle();
        }
        private void Savelog(string msg,string state = "Normal")
        {
            string time = DateTime.Now.ToString("yyyy-MM-dd_HH:mm:ss");
            if (state == "FAIL")
            {
                WriteLog(GlobalConfig.sServerLogPath, $"{time}\t{textBoxSn.Text.Trim().ToUpper()}\tFAIL\tTestTime({g_iTimerCycle})");
                WriteLog(GlobalConfig.sLocalLogPath,  $"{time}\t{textBoxSn.Text.Trim().ToUpper()}\tFAIL\tTestTime({g_iTimerCycle})");
            }
            else if (state == "PASS")
            {
                WriteLog(GlobalConfig.sServerLogPath, $"{time}\t{textBoxSn.Text.Trim().ToUpper()}\tPASS\tTestTime({g_iTimerCycle})");
                WriteLog(GlobalConfig.sLocalLogPath,  $"{time}\t{textBoxSn.Text.Trim().ToUpper()}\tPASS\tTestTime({g_iTimerCycle})");
            }
            else 
            {
                WriteLog(GlobalConfig.sServerLogPath, $"{time}\t{textBoxSn.Text.Trim().ToUpper()}\tmsg({msg.Trim()})");
                WriteLog(GlobalConfig.sLocalLogPath,  $"{time}\t{textBoxSn.Text.Trim().ToUpper()}\tmsg({msg.Trim()})");
            }
        }
        /// <summary>
        /// 逻辑十六进制字符串 至 二进制命令转换
        /// 自动测算数据长度
        /// </summary>
        /// <param name="str">字符串命令</param>
        private void StrToCmd(string str)
        {
            string strBuf = string.Empty;
            int len = new int();
            int crc = new int();
            int tepmCRC = 0;

            strBuf = str.Replace(" ", "");
            len = Convert.ToInt32(strBuf.Length/2);

            for (int k = 1; k < len ; k++)
            {
                tepmCRC = tepmCRC + Convert.ToByte("0x" + strBuf.Substring(k * 2, 2), 16);
            }
            crc = 256 * (tepmCRC / 256 + 1) - tepmCRC;
            if (crc == 256)
            {
               crc = 0;
            }
            if (Convert.ToString(crc, 16).Length == 1)
            {
                strBuf = strBuf + "0" + Convert.ToString(crc, 16) + "FE";
            }
            else
            {
                strBuf = strBuf + Convert.ToString(crc, 16) + "FE";
            }
            for (int i = 0; i < len + 2; i++)
            {
                g_byCmdBuf[i] = Convert.ToByte("0x" + strBuf.Substring(i * 2, 2), 16);
            }
            return;
        }
        private void StrTo_bby_Cmd(string str)
        {
            string strBuf = string.Empty;
            int len = new int();
            int crc = new int();
            int tepmCRC = 0;

            strBuf = str.Replace(" ", "");
            len = Convert.ToInt32(strBuf.Length / 2);

            for (int k = 2; k < len; k++)
            {
                tepmCRC = tepmCRC + Convert.ToByte("0x" + strBuf.Substring(k * 2, 2), 16);
            }
            if (tepmCRC > 256)
            {
                crc = 256 * (tepmCRC / 256 + 1) - tepmCRC;
                if (crc == 256)
                {
                    crc = 0;
                }
            }
            else
            {
                crc = 256 - tepmCRC;
            }

            if (Convert.ToString(crc, 16).Length == 1)
            {
                strBuf = strBuf + "0" + Convert.ToString(crc, 16);
            }
            else
            {
                strBuf = strBuf + Convert.ToString(crc, 16);
            }
            for (int i = 0; i < len + 1; i++)
            {
                g_byCmdBuf[i] = Convert.ToByte("0x" + strBuf.Substring(i * 2, 2), 16);
            }
            return;
        }
        private void StrTo_TW_Cmd(string str)
        {
            string strBuf = string.Empty;
            int len = new int();

            strBuf = str.Replace(" ", "");
            len = Convert.ToInt32(strBuf.Length / 2);

            for (int i = 0; i < len; i++)
            {
                g_byCmdBuf[i] = Convert.ToByte("0x" + strBuf.Substring(i * 2, 2), 16);
            }
            return;
        }

        /// <summary>
        /// 逻辑十六进制字符串 至 二进制命令转换
        /// </summary>
        /// <param name="str">字符串命令</param>
        /// <param name="len">命令字节长度</param>
        private void StrToCmd(string str, int len)
        {
            string strBuf = string.Empty;
            strBuf = str.Replace(" ", "");
            for (int i = 0; i < len - 1; i++)
            {
                g_IOCmdBuf[i] = Convert.ToByte("0x" + strBuf.Substring(i * 2, 2), 16);
            }
            return;
        }
        /// <summary>
        /// 串口发送与接收函数
        /// </summary>
        /// <param name="cmd">包含指令的字节数组</param>
        /// <param name="len">命令字节长度</param>
        /// <param name="cycleDelay">延时参数,50毫秒的倍数</param>
        /// <param name="rtnLen">指定接收到的回传值字节长度</param>
        /// <param name="rtn">接收到的回传值数组</param>
        /// <returns></returns>
        private bool WriteRs232Command_XM(byte[] cmd, int len, int cycleDelay, int rtnLen, out byte[] rtn)
        {
            //textBoxAck.Text = "";   //清空 ACK 框内容
            rtn = null;
            //if (g_bCmdWorkFlag) return false;
            g_bCmdWorkFlag = true;
            g_bPortErr = false;

            int i = new int();
            //int j = new int();
            int retryCount = new int();
            byte[] cmdBuf = new byte[len];
            byte[] tmp = new byte[256];
            byte[] rtack = new byte[len];

            //if (serialPortTV.IsOpen == false)
            //{
            //    try
            //    {
            //        serialPortTV.Open();
            //    }
            //    catch (Exception e)
            //    {
            //        g_bCmdWorkFlag = false;
            //        g_bPortErr = true;
            //        MessageBox.Show(null, e.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //        return false;
            //    }
            //}

            for (i = 0; i < len; i++)
            {
                cmdBuf[i] = cmd[i];
            }
            //cmdBuf[len - 1] = 0;
            //j = 0;
            ////calc checksum
            //for (i = 0; i < len - 1; i++)
            //{
            //    j = j ^ (int)cmdBuf[i];
            //}
            //cmdBuf[len - 1] = (byte)j;

            g_sCmdRtnHexText = "CMD:";
            for (i = 0; i < len; i++)
            {
                g_sCmdRtnHexText = g_sCmdRtnHexText + string.Format(" {0:X02}", cmdBuf[i]);
            }
            //textBoxAck.Text = g_sCmdRtnHexText;   //输出到窗口界面 CMD/ACK 框;
            textBoxAck.AppendText("CMD:\r\n" + g_sCmdRtnHexText + "\r\n"); //cmd

            //serialPortTV.DiscardInBuffer();
            //serialPortTV.DiscardOutBuffer();
            //serialPortTV.WriteTimeout = 5000;
            //serialPortTV.ReadTimeout = 5000;
            //try
            //{
            //    serialPortTV.Write(cmdBuf, 0, len); //write cmd
            //}
            //catch (Exception e)
            //{
            //    g_bCmdWorkFlag = false;
            //    MessageBox.Show(null, "写入串口指令失败\n" + e.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    return false;
            //}

            
            System.Windows.Forms.Application.DoEvents();
            Thread.Sleep(20);

            try
            {
                //do
                //{
                //    retryCount += 1;
                //    if (retryCount > 100)
                //    {
                //        //serialPortTV.Write(cmd, 0, len); //write rtk cmd

                //        //if (serialPortTV.IsOpen == true) serialPortTV.Close();
                //        g_bCmdWorkFlag = false;
                //        return false;
                //    }
                //    System.Windows.Forms.Application.DoEvents(); //处理消息队列
                //    Thread.Sleep(cycleDelay);
                //    if (g_iCmdDelay != 0)
                //    {
                //        System.Windows.Forms.Application.DoEvents();
                //        Delay(g_iCmdDelay);
                //        g_iCmdDelay = 0;
                //    }
                //} while (serialPortTV.BytesToRead < rtnLen);
                //serialPortTV.Read(tmp, 0, rtnLen);  //get ack
            }
            catch (Exception e)
            {
                g_bCmdWorkFlag = false;
                MessageBox.Show(null, "Get ACK Fail\n" + e.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            rtn = tmp;
            for (i = 0; i < len; i++)
            {
                rtack[i] = tmp[i];
            }
            //serialPortTV.Write(rtack, 0, len); //write rtk cmd

            g_sCmdRtnHexText = "ACK:";
            for (i = 0; i < rtnLen; i++)
            {
                g_sCmdRtnHexText = g_sCmdRtnHexText + string.Format(" {0:X02}", tmp[i]);
            }
            g_sCmdRtnHexText = g_sCmdRtnHexText.Trim();

            //textBoxAck.Text = textBoxAck.Text + g_sCmdRtnHexText;   //输出到窗口界面 CMD/ACK 框
            textBoxAck.AppendText("ACK:\r\n" + g_sCmdRtnHexText + "\r\n");

            if (g_bRTACKFlag == true)
            {
                if ((tmp[0] != cmd[0]) || (tmp[2] != cmd[2]) || (tmp[3] != cmd[3]) || (tmp[11] != cmd[2]) || (tmp[12] != cmd[3]) || (tmp[13] != 1) || (tmp[14] != 0))
                {
                    g_bCmdWorkFlag = false;
                    return false;
                }
            }
            else
            {
                if ((tmp[0] != cmd[0]) || (tmp[2] != cmd[2]) || (tmp[3] != cmd[3]) || (tmp[11] != cmd[2]) || (tmp[12] != cmd[3]))
                {
                    g_bCmdWorkFlag = false;
                    return false;
                }
            }
            //if (serialPortTV.IsOpen) serialPortTV.Close();
            g_bCmdWorkFlag = false;
            return true;
        }
        private bool WriteRs232Command_XM_ACK(byte[] cmd, int len, int cycleDelay)
        {
            //if (g_bCmdWorkFlag) return false;
            g_bCmdWorkFlag = true;
            g_bPortErr = false;
            int i = new int();
            byte[] ComReceiveBuffer = new byte[500];
            byte[] bData = new byte[500];
            byte[] cmdBuf = new byte[len];
            byte[] tmp = new byte[256];
            byte[] rtack = new byte[len];
            byte[] btack = new byte[8];

            //if (serialPortTV.IsOpen == false)
            //{
            //    try
            //    {
            //        serialPortTV.Open();
            //    }
            //    catch (Exception e)
            //    {
            //        g_bCmdWorkFlag = false;
            //        g_bPortErr = true;
            //        MessageBox.Show(null, e.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //        return false;
            //    }
            //}
            //serialPortTV.DiscardInBuffer();
            //serialPortTV.DiscardOutBuffer();
            //serialPortTV.WriteTimeout = 5000;
            //serialPortTV.ReadTimeout = 5000;

            g_sCmdRtnHexText = "CMD:";
            for (i = 0; i < len; i++)
            {
                cmdBuf[i] = cmd[i];
                g_sCmdRtnHexText = g_sCmdRtnHexText + string.Format(" {0:X02}", cmdBuf[i]);
            }
            textBoxAck.AppendText("CMD:\r\n" + g_sCmdRtnHexText + "\r\n"); ////输出到窗口界面 CMD/ACK 框;
            System.Windows.Forms.Application.DoEvents();
            Thread.Sleep(20);

            //try
            //{
            //    serialPortTV.Write(cmdBuf, 0, len); //write cmd
            //}
            //catch (Exception e)
            //{
            //    g_bCmdWorkFlag = false;
            //    MessageBox.Show(null, "写入串口指令失败\n" + e.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    return false;
            //}

            Thread.Sleep(cycleDelay);
            return true;
        }
        private bool WriteRs232CommandMI(byte[] cmd, int len, int cycleDelay, int rtnLen, out byte[] rtn)
        {
            rtn = null;
            //if (g_bCmdWorkFlag) return false;
            g_bCmdWorkFlag = true;
            g_bPortErr = false;
            bool bCmdOK = false;
            int i = new int();
            int ii = new int();
            int j = new int();
            int Marki = new int();
            byte[] ComReceiveBuffer = new byte[500];
            byte[] bData = new byte[500];
            int nLenBuffer2 = new int();
            int retryCount = new int();
            byte[] cmdBuf = new byte[len];
            byte[] tmp = new byte[256];
            byte[] rtack = new byte[len];
            byte[] btack = new byte[10];

            //if (serialPortTV.IsOpen == false)
            //{
            //    try
            //    {
            //        serialPortTV.Open();
            //    }
            //    catch (Exception e)
            //    {
            //        g_bCmdWorkFlag = false;
            //        g_bPortErr = true;
            //        MessageBox.Show(null, e.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //        return false;
            //    }
            //}
            g_sCmdRtnHexText = "CMD:";
            for (i = 0; i < len; i++)
            {
                cmdBuf[i] = cmd[i];
                g_sCmdRtnHexText = g_sCmdRtnHexText + string.Format(" {0:X02}", cmdBuf[i]);
            }
            textBoxAck.AppendText("CMD:\r\n" + g_sCmdRtnHexText + "\r\n"); ////输出到窗口界面 CMD/ACK 框;
            //serialPortTV.DiscardInBuffer();
            //serialPortTV.DiscardOutBuffer();
            //serialPortTV.WriteTimeout = 5000;
            //serialPortTV.ReadTimeout = 5000;
            //try
            //{
            //    serialPortTV.Write(cmdBuf, 0, len); //write cmd
            //}
            //catch (Exception e)
            //{
            //    g_bCmdWorkFlag = false;
            //    MessageBox.Show(null, "写入串口指令失败\n" + e.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    return false;
            //}
            //i = Convert.ToInt32(comboBoxCmdDelay.SelectedItem.ToString());
            //Delay(i);
            System.Windows.Forms.Application.DoEvents();
            Thread.Sleep(5);

            try
            {
                do
                {
                    System.Windows.Forms.Application.DoEvents(); //处理消息队列
                    Thread.Sleep(cycleDelay);
                    retryCount += 1;
                    //if (retryCount > 100)
                    //{
                    //    serialPortTV.Write(cmd, 0, len); //write rtk cmd
                    //    if (serialPortTV.IsOpen == true) serialPortTV.Close();
                    //    g_bCmdWorkFlag = false;
                    //    return false;
                    //}
                    //int nLenBuffer = serialPortTV.BytesToRead;
                    //serialPortTV.Read(tmp, 0, rtnLen);  //get ack
                    //if (nLenBuffer > rtnLen)
                    //{
                    //    rtn = tmp;
                    //    for (i = 0; i < nLenBuffer; i++)
                    //    {
                    //        ComReceiveBuffer[i] = tmp[i];
                    //        nLenBuffer2++;
                    //    }
                    //}
                    if (nLenBuffer2 < rtnLen)
                    {
                        bCmdOK = false;
                    }
                    else
                    {
                        for (i = 0; i < nLenBuffer2; i++)
                        {
                            if (ComReceiveBuffer[i] == 121 && ComReceiveBuffer[i + 2] == cmd[2] && ComReceiveBuffer[i + 3] == cmd[3] && ComReceiveBuffer[i + 8] == 254)
                            {
                                for (j = 0; j < 10; j++)
                                {
                                    btack[j] = ComReceiveBuffer[j];
                                }
                                if (WriteRs232Command_XM_ACK(btack, 9, 20) == false)
                                {
                                    return false;
                                }
                                Marki = i;
                                bCmdOK = true;
                                goto CMDPASS; //退出for循环
                            }
                            else
                            {
                                bCmdOK = false;
                            }
                            ii = nLenBuffer2 - rtnLen;
                            if  (i >=ii)
                            {
                                bCmdOK = false;
                                continue; //退出for循环
                            }
                        }
                    }
                } 
                while (bCmdOK==true);
                //serialPortTV.Read(tmp, 0, rtnLen);  //get ack
            }
            catch (Exception e)
            {
                g_bCmdWorkFlag = false;
                //MessageBox.Show(null, "Get ACK Fail\n" + e.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                SetMsg("Get RETURE ACK ...." + e.Message, UDF.COLOR.FAIL );
                g_bCmdWorkFlag = false;
                //if (serialPortTV.IsOpen) serialPortTV.Close();
                return false;
            }
            //rtn = tmp;
            CMDPASS:

            for (i = 0; i < len; i++)
            {
                rtack[i] = tmp[i];
            }
            //serialPortTV.Write(rtack, 0, len); //write rtk cmd

            g_sCmdRtnHexText = "ACK:";
            for (i = 0; i < rtnLen; i++)
            {
                g_sCmdRtnHexText = g_sCmdRtnHexText + string.Format(" {0:X02}", tmp[i]);
            }
            g_sCmdRtnHexText = g_sCmdRtnHexText.Trim();

            //textBoxAck.Text = textBoxAck.Text + g_sCmdRtnHexText;   //输出到窗口界面 CMD/ACK 框
            textBoxAck.AppendText("ACK:\r\n" + g_sCmdRtnHexText + "\r\n");

            if (g_bRTACKFlag == true)
            {
                if ((tmp[0] != cmd[0]) || (tmp[2] != cmd[2]) || (tmp[3] != cmd[3]) || (tmp[11] != cmd[2]) || (tmp[12] != cmd[3]) || (tmp[13] != 1) || (tmp[14] != 0))
                {
                    g_bCmdWorkFlag = false;
                    //if(serialPortTV.IsOpen) serialPortTV.Close();
                    return false;
                }
            }
            else
            {
                if ((tmp[0] != cmd[0]) || (tmp[2] != cmd[2]) || (tmp[3] != cmd[3]) || (tmp[11] != cmd[2]) || (tmp[12] != cmd[3]))
                {
                    g_bCmdWorkFlag = false;
                    //if(serialPortTV.IsOpen) serialPortTV.Close();
                    return false;
                }
            }
            //if (serialPortTV.IsOpen) serialPortTV.Close();
            g_bCmdWorkFlag = false;
            return true;
        }
        private Boolean WriteRS232CommandBBY(byte[] btData, int nLen, int nByteDelay, int nRecLen, out byte[] VarReturn)
        {
            byte[] btACK = new byte[12];
            VarReturn = null;

            //if (serialPortTV.IsOpen == false)
            //{
            //    try
            //    {
            //        serialPortTV.Open();
            //    }
            //    catch (Exception err1)
            //    {
            //        MessageBox.Show(null, err1.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //        return false;
            //    }
            //}

            int i = new int();
            //int j = new int();
            int retryCount = 0;
            byte[] Tbyte = new byte[nLen];
            byte[] tmp = new byte[256];


            g_sCmdRtnHexText = "CMD:";
            for (i = 0; i < nLen; i++)
            {
                Tbyte[i] = btData[i];
                g_sCmdRtnHexText = g_sCmdRtnHexText + string.Format(" {0:X02}", btData[i]);
            }
            textBoxAck.AppendText("CMD:\r\n" + g_sCmdRtnHexText + "\r\n"); ////输出到窗口界面 CMD/ACK 框;

            //serialPortTV.DiscardInBuffer();
            //serialPortTV.DiscardOutBuffer();
            //serialPortTV.WriteTimeout = 2500;
            //serialPortTV.ReadTimeout = 5000;

            //try
            //{
            //    serialPortTV.Write(Tbyte, 0, nLen);
            //}
            //catch (Exception ex)
            //{
            //    SetMsg("发送指令失败，请检查通讯线。" + ex.Message, UDF.COLOR.FAIL);
            //    return false;
            //}
            Thread.Sleep(10);
            try
            {
                //do
                //{
                //    retryCount += 1;
                //    if (retryCount > 150)
                //    {
                //        return false;
                //    }
                //    Application.DoEvents();
                //    Thread.Sleep(nByteDelay);
                //    if (cmd_Delay != 0)
                //    {
                //        Delay(cmd_Delay);
                //        cmd_Delay = 0;
                //    }
                //} while (serialPortTV.BytesToRead < nRecLen);
                //serialPortTV.Read(tmp, 0, nRecLen);
            }
            catch (Exception err1)
            {
                MessageBox.Show(null, err1.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            VarReturn = tmp;

            g_sCmdRtnHexText = "ACK:";
            for (i = 0; i < nRecLen; i++)
            {
                g_sCmdRtnHexText = g_sCmdRtnHexText + string.Format(" {0:X02}", VarReturn[i]);
            }
            g_sCmdRtnHexText = g_sCmdRtnHexText.Trim();

            //textBoxAck.Text = textBoxAck.Text + g_sCmdRtnHexText;   //输出到窗口界面 CMD/ACK 框
            textBoxAck.AppendText("ACK:\r\n" + g_sCmdRtnHexText + "\r\n");
            
            if (g_bRTACKFlag == true)
            {
                //set: FF 33 0C 03 01 00 A1 00 00 00 00 4F 00   ack:  FF 33 15 03 A1 33 33 37 37 39 2F 33 30 36 36 30 30 30 30 30 4C
                if ((btData[0] != VarReturn[0]) || (btData[1] != VarReturn[1]) || (btData[3] != VarReturn[3]) || (VarReturn[4] != 1) || (VarReturn[5] != 0) || (btData[4] != VarReturn[6]))
                {
                    g_bCmdWorkFlag = false;
                    //if (serialPortTV.IsOpen) serialPortTV.Close();
                    return false;
                }
            }
            else
            {
                if ((btData[0] != VarReturn[0]) || (btData[1] != VarReturn[1]) || (btData[3] != VarReturn[3]))
                {
                    g_bCmdWorkFlag = false;
                    //if (serialPortTV.IsOpen) serialPortTV.Close();
                    return false;
                }
            }
            //if (serialPortTV.IsOpen) serialPortTV.Close();
            g_bCmdWorkFlag = false;
            return true;
        }
        private Boolean WriteRS232CommandMI (byte[] btData, int nLen, int nByteDelay, int nRecLen, out byte[] VarReturn)
        {
            byte[] btACK = new byte[9];
            VarReturn = null;

            //if (serialPortTV.IsOpen == false)
            //{
            //    try
            //    {
            //        serialPortTV.Open();
            //    }
            //    catch (Exception err1)
            //    {
            //        MessageBox.Show(null, err1.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //        return false;
            //    }
            //}

            int i = new int();
            //int j = new int();
            int retryCount = 0;
            byte[] Tbyte = new byte[nLen];
            byte[] tmp = new byte[256];


            g_sCmdRtnHexText = "CMD:";
            for (i = 0; i < nLen; i++)
            {
                Tbyte[i] = btData[i];
                g_sCmdRtnHexText = g_sCmdRtnHexText + string.Format(" {0:X02}", btData[i]);
            }
            textBoxAck.AppendText("CMD:\r\n" + g_sCmdRtnHexText + "\r\n"); ////输出到窗口界面 CMD/ACK 框;
            
            //serialPortTV.DiscardInBuffer();
            //serialPortTV.DiscardOutBuffer();
            //serialPortTV.WriteTimeout = 2500;
            //serialPortTV.ReadTimeout = 5000;

            //try
            //{
            //    serialPortTV.Write(Tbyte, 0, nLen);
            //}
            //catch (Exception ex)
            //{
            //    SetMsg("发送指令失败，请检查通讯线。" +ex.Message , UDF.COLOR.FAIL);
            //    return false;
            //}
            Thread.Sleep(10);
            try
            {
                //do
                //{
                //    retryCount += 1;
                //    if (retryCount > 150)
                //    {
                //        return false;
                //    }
                //    Application.DoEvents();
                //    Thread.Sleep(nByteDelay);
                //    if (cmd_Delay != 0)
                //    {
                //        Delay(cmd_Delay);
                //        cmd_Delay = 0;
                //    }
                //} while (serialPortTV.BytesToRead < nRecLen);
                //serialPortTV.Read(tmp, 0, nRecLen);
            }
            catch (Exception err1)
            {
                MessageBox.Show(null, err1.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            VarReturn = tmp;

            g_sCmdRtnHexText = "ACK:";
            for (i = 0; i < nRecLen; i++)
            {
                g_sCmdRtnHexText = g_sCmdRtnHexText + string.Format(" {0:X02}", VarReturn[i]);
            }
            g_sCmdRtnHexText = g_sCmdRtnHexText.Trim();

            //textBoxAck.Text = textBoxAck.Text + g_sCmdRtnHexText;   //输出到窗口界面 CMD/ACK 框
            textBoxAck.AppendText("ACK:\r\n" + g_sCmdRtnHexText + "\r\n");
            
            if (g_bRTACKFlag == true)
            {

                if ((tmp[0] != VarReturn[0]) || (tmp[2] != VarReturn[2]) || (tmp[3] != VarReturn[3]) || (VarReturn[11] != tmp[2]) || (VarReturn[12] != tmp[3]) || (VarReturn[13] != 1) || (VarReturn[14] != 0))
                {
                    g_bCmdWorkFlag = false;
                    //if (serialPortTV.IsOpen) serialPortTV.Close();
                    return false;
                }
                else
                {
                    for (i = 0; i < 9; i++)
                    {
                        btACK[i] = VarReturn[i];
                    }
                    if (WriteRs232CommandMIACK(btACK, 9, 10) == false)
                    {
                        SetMsg("SET REACK CMD FAIL", UDF.COLOR.FAIL);
                        return false;
                    }
                }
            }
            else
            {
                if ((tmp[0] != VarReturn[0]) || (tmp[2] != VarReturn[2]) || (tmp[3] != VarReturn[3]) || (VarReturn[11] != tmp[2]) || (VarReturn[12] != tmp[3]))
                {
                    g_bCmdWorkFlag = false;
                    //if (serialPortTV.IsOpen) serialPortTV.Close();
                    return false;
                }
                else
                {
                    for (i = 0; i < 9; i++)
                    {
                        btACK[i] = VarReturn[i];
                    }
                    if (WriteRs232CommandMIACK(btACK, 9, 10) == false)
                    {
                        SetMsg("SET REACK CMD FAIL", UDF.COLOR.FAIL);
                        return false;
                    }
                }
            }
            //if (serialPortTV.IsOpen) serialPortTV.Close();
            g_bCmdWorkFlag = false;
            return true;
        }
        private Boolean WriteRs232CommandMIACK(byte[] btData, int nLen, int nByteDelay)
        {
            //if (serialPortTV.IsOpen == false)
            //{
            //    try
            //    {
            //        serialPortTV.Open();
            //    }
            //    catch (Exception err1)
            //    {
            //        MessageBox.Show(null, err1.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //        return false;
            //    }
            //}

            int i = new int();
            //int j = new int();
            //int retryCount = 0;
            byte[] Tbyte = new byte[nLen];
            byte[] tmp = new byte[256];
            for (i = 0; i < nLen; i++)
            {
                Tbyte[i] = btData[i];
            }

            //serialPortTV.DiscardInBuffer();
            //serialPortTV.DiscardOutBuffer();
            //serialPortTV.WriteTimeout = 2500;
            //serialPortTV.ReadTimeout = 5000;

            //try
            //{
            //    serialPortTV.Write(Tbyte, 0, nLen);
            //}
            //catch (Exception err1)
            //{
            //    SetMsg("发送指令失败，请检查通讯线。"+ err1.Message , UDF.COLOR.FAIL);
            //    return false;
            //}
            return true;
        }
        private Boolean WriteRS232CommandTW(byte[] btData, int nLen, int nByteDelay, int nRecLen, out string ReACK)
        {
            ReACK ="";
            byte[] VarReturn = new byte[256];
            VarReturn = null;

            //if (serialPortTV.IsOpen == false)
            //{
            //    try
            //    {
            //        //serialPortTV.Open();
            //    }
            //    catch (Exception err1)
            //    {
            //        MessageBox.Show(null, err1.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //        return false ;
            //    }
            //}

            int i = new int();
            int retryCount = 0;
            byte[] Tbyte = new byte[nLen];
            byte[] tmp = new byte[256];


            g_sCmdRtnHexText = "CMD:";
            for (i = 0; i < nLen; i++)
            {
                Tbyte[i] = btData[i];
                g_sCmdRtnHexText = g_sCmdRtnHexText + string.Format(" {0:X02}", btData[i]);
            }
            textBoxAck.AppendText("CMD:\r\n" + g_sCmdRtnHexText + "\r\n"); ////输出到窗口界面 CMD/ACK 框;
            
            //serialPortTV.DiscardInBuffer();
            //serialPortTV.DiscardOutBuffer();
            //serialPortTV.WriteTimeout = 2500;
            //serialPortTV.ReadTimeout = 5000;

            //try
            //{
            //    serialPortTV.Write(Tbyte, 0, nLen);
            //}
            //catch (Exception ex)
            //{
            //    SetMsg("发送指令失败，请检查通讯线。" + ex.Message, UDF.COLOR.FAIL);
            //    return false;
            //}
            Thread.Sleep(10);
            try
            {
                //do
                //{
                //    retryCount += 1;
                //    if (retryCount > 150)
                //    {
                //        return false;
                //    }
                //    Application.DoEvents();
                //    Thread.Sleep(nByteDelay);
                //    if (cmd_Delay != 0)
                //    {
                //        Delay(cmd_Delay);
                //        cmd_Delay = 0;
                //    }
                //} while (serialPortTV.BytesToRead < nRecLen);
                //serialPortTV.Read(tmp, 0, nRecLen);
            }
            catch (Exception err1)
            {
                MessageBox.Show(null, err1.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            VarReturn = tmp;

            g_sCmdRtnHexText = "ACK:";
            for (i = 0; i < nRecLen; i++)
            {
                g_sCmdRtnHexText = g_sCmdRtnHexText + string.Format(" {0:X02}", VarReturn[i]);
            }
            g_sCmdRtnHexText = g_sCmdRtnHexText.Trim();

            //textBoxAck.Text = textBoxAck.Text + g_sCmdRtnHexText;   //输出到窗口界面 CMD/ACK 框
            textBoxAck.AppendText("ACK:\r\n" + g_sCmdRtnHexText + "\r\n");
            
            if (g_bRTACKFlag == true)
            {
                for (i = 0; i < nRecLen; i++)
                {
                    ReACK = ReACK + Convert.ToChar( VarReturn[i]);
                }
                //If InStr(1, ReACK, "OK") < 1 Then Exit Function
                if (ResultCheck(ReACK, "OK") == false)
                {
                    g_bCmdWorkFlag = false;
                    //if (serialPortTV.IsOpen) serialPortTV.Close();
                    return false;
                }
            }
            else
            {
                for (i = 0; i < nRecLen; i++)
                {
                    ReACK = ReACK + Convert.ToChar(VarReturn[i]);
                }
            }
            //if (serialPortTV.IsOpen) serialPortTV.Close();
            g_bCmdWorkFlag = false;
            return true;
        }
        private void  Write_IOCARD_OUT(string Sendcmd, int len, int cycleDelay, int rtnLen)
        {
            //textBoxAck.Text = "";   //清空 ACK 框内容
            int i = new int();
            byte[] cmdBuf = new byte[len];
            byte[] tmp = new byte[256];
            byte[] rtack = new byte[len];
            byte[] rtn = new byte[255];

            rtn = null;
            g_bPortErr = false;
            StrToCmd(Sendcmd, 7);

            //if (serialPortIOCard.IsOpen == false)
            //{
            //    try
            //    {
            //        serialPortIOCard.Open();
            //    }
            //    catch (Exception ex)
            //    {
            //        g_bCmdWorkFlag = false;
            //        g_bPortErr = true;
            //        //MessageBox.Show(null, e.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //        SetMsg("提示" + ex.Message, UDF.COLOR.FAIL);
            //        return;
            //    }
            //}
            for (i = 0; i < len; i++)
            {
                cmdBuf[i] = g_IOCmdBuf[i];
            }

            g_sCmdRtnHexText = "CMD:";
            for (i = 0; i < len; i++)
            {
                g_sCmdRtnHexText = g_sCmdRtnHexText + string.Format(" {0:X02}", cmdBuf[i]);
            }
            textBoxAck.Text = g_sCmdRtnHexText;   //输出到窗口界面 CMD/ACK 框;

            //serialPortIOCard.DiscardInBuffer();
            //serialPortIOCard.DiscardOutBuffer();
            //serialPortIOCard.WriteTimeout = 500;
            //serialPortIOCard.ReadTimeout = 500;

            //serialPortIOCard.Write(cmdBuf, 0, len); //write cmd
            //if (serialPortIOCard.IsOpen) serialPortIOCard.Close();
        }
        /// <summary>
        /// 如果网络上的程序比当前版本新,则自动更新程序
        /// </summary>
        private void AutoUpdate()
        {
            try
            {
                //网络位置定义
                const string sNetExePath = @"\\10.2.100.27\te\SETUP\PreTest\pre_new\PRETEST.exe";

                //获得当前程序信息
                Process cur = Process.GetCurrentProcess();
                FileInfo fi = new FileInfo(cur.MainModule.FileName);

                string MM = System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase.ToString();
                // MessageBox.Show(MM);
                MM = wbc.iniFile.Right(MM, 11).ToUpper();
                //获得网络位置程序信息
                if (MM != "PRETEST.EXE")
                {
                    return;
                }
                //获得网络位置程序信息
                FileInfo fi2 = new FileInfo(sNetExePath);

                //比较文件修改日期信息
                //如果网络上的文件比当前文件新,则...
                if (DateTime.Compare(fi2.LastWriteTime, fi.LastWriteTime) > 0)
                {
                    try
                    {
                        Process subPro = new Process();
                        subPro.StartInfo.UseShellExecute = true;
                        subPro.StartInfo.FileName = "cmd";
                        subPro.StartInfo.Arguments = " /q /c echo 请勿中断,正在更新程序... & ping -n 3 127.1 1>nul 2>nul & taskkill /pid "
                            + cur.Id + " 1>nul 2>nul & copy /y \""
                            + sNetExePath + "\" \""
                            + cur.MainModule.FileName
                            + "\" 1>nul 2>nul && start \"\" /Normal \""
                            + cur.MainModule.FileName + "\"";
                        subPro.StartInfo.CreateNoWindow =false ;
                        subPro.Start();
                    }
                    catch
                    {

                    }
                    finally
                    {
                        Close();
                    }
                }
            }
            catch
            {

            }
        }
        /// <summary>
        /// 重启程序
        /// </summary>
        private void Restart()
        {
            try
            {
                //获得当前程序信息
                Process cur = Process.GetCurrentProcess();
                FileInfo fi = new FileInfo(cur.MainModule.FileName);

                try
                {
                    Process subPro = new Process();
                    subPro.StartInfo.UseShellExecute = true;
                    subPro.StartInfo.FileName = "cmd";
                    subPro.StartInfo.Arguments = " /q /c echo 请勿中断,正在重启程序... & ping -n 3 127.1 1>nul 2>nul & taskkill /pid "
                        + cur.Id + " 1>nul 2>nul & start \"\" /max \""
                        + cur.MainModule.FileName + "\"";
                    subPro.StartInfo.CreateNoWindow = false;
                    subPro.Start();
                }
                catch
                {

                }
                finally
                {
                    Close();
                }
            }
            catch
            {

            }
        }
        /*++++++++++++++++++++++++++++++++++++++++++++++++++++
       * 串口设置函数
       * 例如：InitPort (TvPort, 2, "115200,N,8,1")
       * --------------------------------------------------*/
        /// <summary>
        /// 串口初始化函数
        /// </summary>
        /// <param name="SerPort">SerialPort</param>
        /// <param name="com">串口号</param>
        /// <param name="setting">串口设置</param>
        private void InitPort(System.IO.Ports.SerialPort SerPort, int com, string setting)
        {
            string strBuf = string.Empty;

            SerPort.PortName = "COM" + com; //设置 COM 口
            strBuf = Regex.Match(setting, @"^\d+(?=,)").ToString().Trim();
            if (strBuf != string.Empty)
            {
                SerPort.BaudRate = Convert.ToInt32(strBuf); //设置 BaudRate
            }
            strBuf = Regex.Match(setting, @"(?<=^\d+,)[a-z A-Z]+").ToString().Trim();
            if (strBuf != string.Empty) //设置 Parity
            {
                switch (strBuf)
                {
                    case "E":
                        SerPort.Parity = System.IO.Ports.Parity.Even;
                        break;
                    case "M":
                        SerPort.Parity = System.IO.Ports.Parity.Mark;
                        break;
                    case "N":
                        SerPort.Parity = System.IO.Ports.Parity.None;
                        break;
                    case "O":
                        SerPort.Parity = System.IO.Ports.Parity.Odd;
                        break;
                    case "S":
                        SerPort.Parity = System.IO.Ports.Parity.Space;
                        break;
                    default:
                        break;
                }
            }
            strBuf = Regex.Match(setting, @"(?<=[a-z A-Z]+,)\d+").ToString().Trim();
            if (strBuf != string.Empty)
            {
                SerPort.DataBits = Convert.ToInt32(strBuf); //设置 DataBits
            }
            strBuf = Regex.Match(setting, @"(?<=,)[0-9 \.]+$").ToString().Trim();
            if (strBuf != string.Empty) //设置 StopBits
            {
                switch (strBuf)
                {
                    case "0":
                        SerPort.StopBits = System.IO.Ports.StopBits.None;
                        break;
                    case "1":
                        SerPort.StopBits = System.IO.Ports.StopBits.One;
                        break;
                    case "1.5":
                        SerPort.StopBits = System.IO.Ports.StopBits.OnePointFive;
                        break;
                    case "2":
                        SerPort.StopBits = System.IO.Ports.StopBits.Two;
                        break;
                    default:
                        SerPort.StopBits = System.IO.Ports.StopBits.One;
                        break;
                }
            }
        }
        /// <summary>
        /// 根据配置文件初始化串口设置
        /// </summary>
        //void InitSerPort()
        //{
        //    MessageBox.Show("initSerPort");
            
        //    if (GlobalConfig.sTvEnable != "N")
        //    {

        //        //InitPort(serialPortTV, GlobalConfig.iTvPort, GlobalConfig.sTvSettings);
        //        //try
        //        //{
        //        //    toolStripStatusLabel1.Text = toolStripStatusLabel1.Text + "Com_TV--" + GlobalConfig.iTvPort;
        //        //    if (!serialPortTV.IsOpen)
        //        //    {
        //        //        serialPortTV.Open();
        //        //    }
        //        //}
        //        //catch (Exception ex)
        //        //{
        //        //    MessageBox.Show(ex.Message);
        //        //}
        //    }
        //    if (GlobalConfig.sGdmEnable != "N")
        //    {
        //        //InitPort(serialPortGDM, GlobalConfig.iGdmPort, GlobalConfig.sGdmSettings);
        //    }
        //    if (GlobalConfig.sIOCardEnable != "N")
        //    {
        //        //InitPort(serialPortIOCard, GlobalConfig.iIOCardPort, GlobalConfig.sIOCardSettings);
        //        //try
        //        //{
        //        //    if (!serialPortIOCard.IsOpen)
        //        //    {
        //        //        serialPortIOCard.Open();
        //        //        serialPortIOCard.ReceivedBytesThreshold = 8;
        //        //        toolStripStatusLabel1.Text = toolStripStatusLabel1.Text + ";Com_IOCard--" + GlobalConfig.iIOCardPort;
        //        //        IO_Card_Reset();
        //        //        this.timerIO_CARD.Enabled = true;
        //        //    }
        //        //}
        //        //catch (Exception ex)
        //        //{
        //        //    MessageBox.Show(ex.Message);
        //        //}
        //    }
        //    if (GlobalConfig.sMhlEnable != "N")
        //    {
        //        //InitPort(serialPortMHL, GlobalConfig.iMhlPort, "115200,N,8,1");
        //    }
        //    if (GlobalConfig.sDoorEnable != "N")
        //    {
        //        //InitPort(serialPortDoor, GlobalConfig.iDoorPort, "9600,N,8,1");
        //    }
        //}
        bool StrToSn(string strSrc, out string sn)
        {
            sn = string.Empty;

            if (ResultCheck(strSrc, @"\d+/\d+"))    //SN -> SN
            {
                sn = strSrc;
            }
            else if (ResultCheck(strSrc, @"S12.*"))  // MN -> SN
            {
                string sql = string.Empty;
                bool bErr = new bool();
                OleDbDataReader reader;

                sql = string.Format("select productname from consumable t where t.consumabletype = 'MBDMN' and t.consumablename =  '{0}'",
                    strSrc
                    );
                //bErr = Oracle.ServerExecute(sql, out reader);
                //if (bErr)
                //{
                //    reader.Read();
                //    if (reader.HasRows)
                //    {
                //        //已有数据
                //        sn = reader[0].ToString();
                //    }
                //    else
                //    {
                //        //没有数据
                //        SetMsg("MN索引失败，请扫 SN 作业.", UDF.COLOR.FAIL);
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

                if (sn.Length == 0)
                {
                    SetMsg("MN索引失败，请扫 SN 作业.", UDF.COLOR.FAIL);
                    return false;
                }
            }

            return true;
        }
        bool QueryMI_MAC()
        {
            string sql = string.Empty;
            bool bErr = new bool();
            //string SDriverX.g_modelInfo.sMn = string.Empty;
            OleDbDataReader reader;
            SetMsg("正在从数据库索引SN绑定的ETH MAC信息...", UDF.COLOR.WORK);
            if (g_bXM_RZ2L == true || g_bXM_RZ7L == true || g_bXM_RZ8L == true || g_bXM_RZ9L == true || g_bXM_RZAL == true || g_bXM_RZBL == true || g_bXM_RZCL == true)
            {
                sql = string.Format("select keycode,didkey from rknmgr.mikey_RZ2L where use_mode=0 and ssn='" + SDriverX.g_modelInfo.sSn + "'");
            }
            else if (g_bXM_RZ3L == true || g_bXM_RZ4L == true || g_bXM_RZ5L == true || g_bXM_RZ6L == true)
            {
                sql = string.Format("select keycode,didkey from rknmgr.mikey_RZ3L where use_mode=0 and ssn='" + SDriverX.g_modelInfo.sSn + "'");
            }
            else if (g_bXM_DZ5L == true || g_bXM_DZJL == true || g_bXM_DZ7L == true || g_bXM_DZ8L == true || g_bXM_DZHL == true || g_bXM_DZIL == true || g_bXM_DZLL == true || g_bXM_DZKL == true || g_bXM_DZNL == true || g_bXM_DZML == true || g_bXM_DZQL == true)
            {
                sql = string.Format("select keycode,didkey from rknmgr.mikey_DZXL where use_mode=0 and ssn='" + SDriverX.g_modelInfo.sSn + "'");
            }
            else if (g_bXM_DZ3L == true || g_bXM_DZ4L == true || g_bXM_DZ9L == true || g_bXM_DZCL == true || g_bXM_DZDL == true  || g_bXM_DZEL == true )
            {
                sql = string.Format("select keycode,didkey from rknmgr.mikey_DZ3L where use_mode=0 and ssn='" + SDriverX.g_modelInfo.sSn + "'");
            }
            else if (g_bXM_DZFL == true || g_bXM_DZGL == true )
            {
                sql = string.Format("select keycode,didkey from rknmgr.mikey_RZ1L where use_mode=0 and  ssn='" + SDriverX.g_modelInfo.sSn + "'");
            }
            else if (g_bXM_WZ2L==true || g_bXM_WZ3L == true || g_bXM_WZ4L == true || g_bXM_WZ5L == true || g_bXM_WZ6L == true || g_bXM_WZ7L == true)
            {
                sql = string.Format("select keycode,didkey from rknmgr.mikey_WZXL where use_mode=0 and  ssn='" + SDriverX.g_modelInfo.sSn + "'");
            }
            else 
            {
                SetMsg("MI ETH MAC数据库为定义，请联系TE...", UDF.COLOR.FAIL);
                return false;
            }
            this.textBoxAck.Text = textBoxAck.Text + sql;
            //bErr = Oracle.ServerExecute(sql, out reader);
            //if (bErr)
            //{
            //    reader.Read();
            //    if (reader.HasRows)
            //    {
            //        //已有数据
            //        SDriver.SDriverX.g_modelInfo.sEthMac = reader[0].ToString();
            //        SDriver.SDriverX.g_modelInfo.sDidKey = reader[1].ToString();
            //        if (SDriver.SDriverX.g_modelInfo.sDidKey == null)
            //        {
            //            SetMsg("DIDKEY资料未空，请联系TE...", UDF.COLOR.FAIL);
            //            return false;
            //        }
            //    }
            //    else
            //    {
            //        //没有数据
            //        SetMsg("没有从数据库检索到MN，请通知TE.", UDF.COLOR.FAIL);
            //        reader.Close();
            //        return false;
            //    }
            //    reader.Close();
            //}
            //else
            //{
            //    SetMsg("数据库无法查询SN对应的ETH MAC地址，请联系MES", UDF.COLOR.FAIL);
            //    return false;
            //}
            return true;
        }
        bool Insert_MI_MAC(int macindx)
        {
            DateTime dt1 = DateTime.Now;
            string sql = string.Empty;
            string strmac = string.Empty;
            bool bErr = new bool();
            string db = string.Empty;
            //string SDriverX.g_modelInfo.sMn = string.Empty;
            OleDbDataReader reader;
            if (macindx == 0 || macindx>2)
            {
                SetMsg("MI MAC标志位设定错误，请联系TE...", UDF.COLOR.FAIL);
                return false;
            }
            else if (macindx==1)
            {
                strmac = SDriver.SDriverX.g_modelInfo.sWifiMac;
            }
            else if (macindx == 2)
            {
                strmac = SDriver.SDriverX.g_modelInfo.sBtMac;
            }

            SetMsg("保存WIFI MAC信息...", UDF.COLOR.WORK);

            if (g_bXM_RZ2L == true || g_bXM_RZ7L == true || g_bXM_RZ8L == true || g_bXM_RZ9L == true || g_bXM_RZAL == true || g_bXM_RZBL == true || g_bXM_RZCL == true )
            {
                db = "rknmgr.mikey_RZ2L";
            }
            else if (g_bXM_RZ3L == true || g_bXM_RZ4L == true || g_bXM_RZ5L == true || g_bXM_RZ6L == true)
            {
                db = "rknmgr.mikey_RZ3L";
            }
            else if (g_bXM_DZ5L == true || g_bXM_DZJL == true || g_bXM_DZ7L == true || g_bXM_DZ8L == true || g_bXM_DZHL == true || g_bXM_DZIL == true || g_bXM_DZLL == true || g_bXM_DZKL == true || g_bXM_DZNL == true || g_bXM_DZML == true || g_bXM_DZQL == true)
            {
                db = "rknmgr.mikey_DZXL";
            }
            else if (g_bXM_DZ3L == true || g_bXM_DZ4L == true || g_bXM_DZ9L == true || g_bXM_DZCL == true || g_bXM_DZDL == true || g_bXM_DZEL == true)
            {
                db = "rknmgr.mikey_DZ3L";
            }
            else if (g_bXM_DZFL == true || g_bXM_DZGL == true)
            {
                db = "rknmgr.mikey_RZ1L";
            }
            else if (g_bXM_WZ2L == true || g_bXM_WZ3L == true || g_bXM_WZ4L == true || g_bXM_WZ5L == true || g_bXM_WZ6L == true || g_bXM_WZ7L == true)
            {
                db = "rknmgr.mikey_WZXL";
            }
            else
            {
                SetMsg("MI ETH MAC数据库为定义，请联系TE...", UDF.COLOR.FAIL);
                return false;
            }
            sql = string.Format("select *  from "+ db +" where use_mode='" + macindx + "' and keycode='" + strmac + "'");
            this.textBoxAck.Text = textBoxAck.Text + sql;
            //bErr = Oracle.ServerExecute(sql, out reader);
            //if (bErr==true)
            //{
            //    reader.Read();
            //    if (reader.HasRows)
            //    {
            //        //已有数据
            //        if (SDriverX.g_modelInfo.sSn != reader[1].ToString())
            //        {
            //            SetMsg("MAC地址重复...", UDF.COLOR.FAIL);
            //            return false;
            //        }
            //    }
            //    else
            //    {
            //        sql = string.Format("select * from " + db + " where use_mode='" + macindx + "' and ssn='" + SDriverX.g_modelInfo.sSn + "'");
            //        this.textBoxAck.Text = textBoxAck.Text + sql;
            //        bErr = Oracle.ServerExecute(sql, out reader);
            //        if (bErr == true)
            //        {
            //            reader.Read();
            //            if (reader.HasRows)
            //            { 
            //                sql = string.Format("update " + db + "  set keycode='" + strmac + "',use_flag=use_flag+1,use_date=to_date('" + dt1 + "', 'yyyy-mm-dd hh24:mi:ss')  where ssn='" + SDriverX.g_modelInfo.sSn + "' and use_mode='" + macindx + "'");
            //                this.textBoxAck.Text = textBoxAck.Text + sql;
            //                bErr = Oracle.UpdateServer(sql);
            //                if (bErr == false)
            //                {
            //                    SetMsg("更新数据库插入失败", UDF.COLOR.FAIL);
            //                    reader.Close();
            //                    return false;
            //                }
            //            }
            //            else
            //            {
            //                sql = string.Format("insert into " + db + " (keycode,ssn,use_flag,use_mode,use_date) values ('" + SDriverX.g_modelInfo.sWifiMac + "','" + SDriverX.g_modelInfo.sSn + "','1','" + macindx + "', sysdate)");
            //                this.textBoxAck.Text = textBoxAck.Text + sql;
            //                bErr = Oracle.UpdateServer(sql);
            //                if (bErr == false)
            //                {
            //                    SetMsg("数据库插入失败", UDF.COLOR.FAIL);
            //                    reader.Close();
            //                    return false;
            //                }
            //            }
            //        }

            //    }
            //    reader.Close();
            //}
            //else
            //{
            //    SetMsg("数据库查询失败", UDF.COLOR.FAIL);
            //    reader.Close();
            //    return false;
            //}
            return true;
        }
        bool Insert_FW_Info(string FW)
        {
            DateTime dt1 = DateTime.Now;
            string sql = string.Empty;
            string strmac = string.Empty;
            bool bErr = new bool();
            string db = string.Empty;
            //string SDriverX.g_modelInfo.sMn = string.Empty;
            OleDbDataReader reader;

            //SetMsg("MI MAC标志位设定错误，请联系TE...", UDF.COLOR.WORK);

            sql = string.Format("select fw_data  from rknmgr.insp_xiaomi_fw t where t.ssn='" + SDriverX.g_modelInfo.sSn + "'");
            this.textBoxAck.Text = textBoxAck.Text + sql;
            //bErr = Oracle.ServerExecute(sql, out reader);
            //if (bErr == true)
            //{ 
            //    reader.Read();
            //    if (reader.HasRows)
            //    {
            //        //已有数据
            //        if (FW != reader[0].ToString())
            //        {
            //            sql = string.Format("update rknmgr.insp_xiaomi_fw  set fw_data='" + FW + "',sysdat=to_date('" + dt1 + "', 'yyyy-mm-dd hh24:mi:ss')  where ssn='" + SDriverX.g_modelInfo.sSn + "'");
            //            this.textBoxAck.Text = textBoxAck.Text + sql;
            //            bErr = Oracle.UpdateServer(sql);
            //            if (bErr == false)
            //            {
            //                SetMsg("更新数据库插入失败", UDF.COLOR.FAIL);
            //                reader.Close();
            //                return false;
            //            }
            //        }
            //    }
            //    else
            //    {
            //        sql = string.Format("insert into rknmgr.insp_xiaomi_fw (ssn,model,workorder,fw_data,sysdat,remark) values ('" + SDriverX.g_modelInfo.sSn + "','" + SDriverX.g_modelInfo.sPart_no + "','" + SDriverX.g_modelInfo.sWord + "','" + FW + "',sysdate,'OK')");
            //        this.textBoxAck.Text = textBoxAck.Text + sql;
            //        bErr = Oracle.UpdateServer(sql);
            //        if (bErr == false)
            //        {
            //            SetMsg("数据库插入失败", UDF.COLOR.FAIL);
            //            reader.Close();
            //            return false;
            //        }
            //    }
            //}
            //else
            //{
            //    SetMsg("数据库查询失败", UDF.COLOR.FAIL);
            //    reader.Close();
            //    return false;
            //}
            //reader.Close();
            return true;
        }
        bool Insert_DEVICESN_Info(string dsn)
        {
            DateTime dt1 = DateTime.Now;
            string sql = string.Empty;
            string strmac = string.Empty;
            bool bErr = new bool();
            string db = string.Empty;
            //string SDriverX.g_modelInfo.sMn = string.Empty;
            OleDbDataReader reader;

            //SetMsg("MI MAC标志位设定错误，请联系TE...", UDF.COLOR.WORK);

            sql = string.Format("select  DSN from rknmgr.Insp_Bbysn t where t.TRID='" + SDriverX.g_modelInfo.sSn + "'");
            this.textBoxAck.Text = textBoxAck.Text + sql;
            //bErr = Oracle.ServerExecute(sql, out reader);
            //if (bErr == true)
            //{
            //    reader.Read();
            //    if (reader.HasRows)
            //    {
            //        //已有数据
            //        if (reader[0].ToString() != "")
            //        {
            //            if (dsn != reader[0].ToString())
            //            {
            //                sql = string.Format("update rknmgr.Insp_Bbysn set DSN='" + dsn + "',INDATETIME=to_date('" + dt1 + "', 'yyyy-mm-dd hh24:mi:ss')  where TRID='" + SDriverX.g_modelInfo.sSn + "'");
            //                this.textBoxAck.Text = textBoxAck.Text + sql;
            //                bErr = Oracle.UpdateServer(sql);
            //                if (bErr == false)
            //                {
            //                    SetMsg("更新数据库插入失败", UDF.COLOR.FAIL);
            //                    reader.Close();
            //                    return false;
            //                }
            //            }
            //        }
            //        else
            //        {
            //            sql = string.Format("insert into rknmgr.Insp_Bbysn (TRID,DSN,PN,WORKORDER,INDATETIME) values ('" + SDriverX.g_modelInfo.sSn + "','" + dsn + "','" + SDriverX.g_modelInfo.sPart_no + "','" + SDriverX.g_modelInfo.sWord + "',to_date('" + dt1 + "', 'yyyy-mm-dd hh24:mi:ss'))");
            //            this.textBoxAck.Text = textBoxAck.Text + sql;
            //            bErr = Oracle.UpdateServer(sql);
            //            if (bErr == false)
            //            {
            //                SetMsg("数据库插入失败", UDF.COLOR.FAIL);
            //                reader.Close();
            //                return false;
            //            }
            //        }
            //    }
            //    else
            //    {
            //        sql = string.Format("insert into rknmgr.Insp_Bbysn (TRID,DSN,PN,WORKORDER,INDATETIME) values ('" + SDriverX.g_modelInfo.sSn + "','" + dsn + "','" + SDriverX.g_modelInfo.sPart_no + "','" + SDriverX.g_modelInfo.sWord + "',to_date('" + dt1 + "', 'yyyy-mm-dd hh24:mi:ss'))");
            //        this.textBoxAck.Text = textBoxAck.Text + sql;
            //        bErr = Oracle.UpdateServer(sql);
            //        if (bErr == false)
            //        {
            //            SetMsg("数据库插入失败", UDF.COLOR.FAIL);
            //            reader.Close();
            //            return false;
            //        }
            //    }
            //}
            //else
            //{
            //    SetMsg("数据库查询失败", UDF.COLOR.FAIL);
            //    reader.Close();
            //    return false;
            //}
            //reader.Close();
            return true;
        }
        bool Insert_MAC_Info(string mactype,string mac)
        {
            DateTime dt1 = DateTime.Now;
            string sql = string.Empty;
            string strmac = string.Empty;
            bool bErr = new bool();
            string db = string.Empty;
            //string SDriverX.g_modelInfo.sMn = string.Empty;
            OleDbDataReader reader;

            //SetMsg("MI MAC标志位设定错误，请联系TE...", UDF.COLOR.WORK);

            //sql = string.Format("select key_code  from rknmgr.insp_onsum_key t where t.serial_number='" + SDriverX.g_modelInfo.sSn + "' and t.key_type='"+ mactype + "'");
            sql = string.Format("select key_code  from rknmgr.insp_onsum_key t where t.serial_number= '" + SDriverX.g_modelInfo.sSn + "' and t.key_type='" + mactype + "'");
            this.textBoxAck.Text = textBoxAck.Text + sql;
            //bErr = Oracle.ServerExecute(sql, out reader);
            //if (bErr == true)
            //{
            //    reader.Read();
            //    if (reader.HasRows)
            //    {
            //        //已有数据

            //        if (mac != reader[0].ToString())
            //        {
            //            sql = string.Format("update rknmgr.insp_onsum_key set key_code='" + mac + "',insert_date=to_date('" + dt1 + "', 'yyyy-mm-dd hh24:mi:ss')  where serial_number='" + SDriverX.g_modelInfo.sSn + "'and key_type='" + mactype + "'");
            //            this.textBoxAck.Text = textBoxAck.Text + sql;
            //            bErr = Oracle.UpdateServer(sql);
            //            if (bErr == false)
            //            {
            //                SetMsg("更新数据库插入失败", UDF.COLOR.FAIL);
            //                reader.Close();
            //                return false;
            //            }
            //        }
            //    }
            //    else
            //    {
            //        sql = string.Format("insert into rknmgr.insp_onsum_key (serial_number,key_code,key_type,insert_date,use_date,line,station) values ('" + SDriverX.g_modelInfo.sSn + "','" + mac + "','" + mactype + "',to_date('" + dt1 + "', 'yyyy-mm-dd hh24:mi:ss'),'','" + GlobalConfig.sMesEid.Substring(5, 3) + "','" + GlobalConfig.sMesEid.Substring(2, 3) + "')");
            //        this.textBoxAck.Text = textBoxAck.Text + sql;
            //        bErr = Oracle.UpdateServer(sql);
            //        if (bErr == false)
            //        {
            //            SetMsg("数据库插入失败", UDF.COLOR.FAIL);
            //            reader.Close();
            //            return false;
            //        }
            //    }
            //}
            //else
            //{
            //    SetMsg("数据库查询失败", UDF.COLOR.FAIL);
            //    reader.Close();
            //    return false;
            //}
            //reader.Close();
            return true;
        }
        bool QueryMN()
        {
            string sql = string.Empty;
            bool bErr = new bool();
            //string SDriverX.g_modelInfo.sMn = string.Empty;
            OleDbDataReader reader;
            SetMsg("正在从数据库索引SN绑定的MB MN信息...", UDF.COLOR.WORK);
            sql = string.Format("select consumablename from consumable t where t.consumabletype = 'MBD' and t.productname in (select PRODUCTNAME from product  where set_serial_no='" + SDriverX.g_modelInfo.sSn +"')");
            //bErr = Oracle.ServerExecute(sql, out reader);
            //if (bErr)
            //{ 
            //    reader.Read();
            //    if (reader.HasRows)
            //    {
            //        //已有数据
            //        SDriver.SDriverX.g_modelInfo.sMBMn = reader[0].ToString();
            //    }
            //    else
            //    {
            //        //没有数据
            //        SetMsg("没有从数据库检索到MN，请通知TE.", UDF.COLOR.FAIL);
            //        reader.Close();
            //        return false;
            //    }
            //    reader.Close();
            //}
            //else
            //{
            //    SetMsg("无法查询到MB MN信息", UDF.COLOR.FAIL);
            //    return false;
            //}

            SetMsg("正在从数据库索引SN绑定的TV MN信息...", UDF.COLOR.WORK);
            sql = string.Format("select mn_no from ccs_lbl_tseriallabel t where t.serial_bcd='" + SDriverX.g_modelInfo.sSn + "'");
            //bErr = Oracle.ServerExecute(sql, out reader);
            //if (bErr)
            //{
            //    reader.Read();
            //    if (reader.HasRows)
            //    {
            //        //已有数据
            //        SDriver.SDriverX.g_modelInfo.sMn = reader[0].ToString();
            //    }
            //    else
            //    {
            //        //没有数据
            //        SetMsg("没有从数据库检索到MN，请通知TE.", UDF.COLOR.FAIL);
            //        reader.Close();
            //        return false;
            //    }
            //    reader.Close();
            //}
            //else
            //{
            //    SetMsg("无法查询到TV MN信息", UDF.COLOR.FAIL);
            //    return false;
            //}
            return true;
         }
        bool SnMnCheck()
        {
            int retryTime = 0;
            Match match = null;

        CheckMn:
            SetMsg("Read and check mn...", UDF.COLOR.WORK);
            if (WriteRs232CommandS12(true, "MSTC_MNRead", 0) == false)
            {
                if (retryTime > 1)
                {
                    SetMsg("Write mn fail，请检查通讯线连接状况！", UDF.COLOR.FAIL);
                    return false;
                }
                retryTime++;
                goto CheckMn;
            }
            match = Regex.Match(g_sCmdRtnStrText, @"(?<=MSTC_R_MN_)[A-Za-z0-9]{1,}(?=\r\n)"); //old regular:(?<=MSTC_R_MN_)(\d{5}/\d{9})
            if (match.Success)
            {
                SetMsg("Read mn:" + match.Value, UDF.COLOR.WORK);

                //查询标签SN对应MN
                string sql = string.Empty;
                bool bErr = new bool();
                OleDbDataReader reader;

                SetMsg("正在从数据库索引SN绑定的MN信息...", UDF.COLOR.WORK);
                //先查询是否已申请
                sql = string.Format("select consumablename from consumable t where t.consumabletype = 'MBDMN' and t.productname = '{0}'",
                    SDriver.SDriverX.g_modelInfo.sSn
                    );
                //bErr = Oracle.ServerExecute(sql, out reader);
                //if (bErr)
                //{
                //    reader.Read();
                //    if (reader.HasRows)
                //    {
                //        //已有数据
                //        SDriver.SDriverX.g_modelInfo.sMn = reader[0].ToString();
                //    }
                //    else
                //    {
                //        //没有数据
                //        SetMsg("没有从数据库检索到MN，请通知TE.", UDF.COLOR.FAIL);
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

                //比对MN
                SetMsg("Check mn...", UDF.COLOR.WORK);
                if (match.Value != SDriver.SDriverX.g_modelInfo.sMn)
                {
                    SetMsg("MN 读取值与SN绑定的MN不符!可能是标签机台不符", UDF.COLOR.FAIL);
                    return false;
                }
            }
            else
            {
                SetMsg(@"Read mn fail,未匹配到MN,匹配规则：(?<=MSTC_R_MN_)(\d{5}/\d{9})", UDF.COLOR.FAIL);
                return false;
            }

            return true;
        }
        bool CheckTRID_ForNeatframeModel(string sn)
        {
            string sql = string.Empty;

            SDriverX.g_modelInfo = new UDF.ModelInfo
            {
                sSn = sn,
                sModelName = "Neatframe Model"
            };

            OleDbDataReader rdr = null;
            bool bErr = new bool();
            SetMsg("检查Reset站过站记录...", UDFs.UDF.COLOR.WORK);
            if (SDriverX.STCK("1AWBCY31", sn, "TestUser") == false)
            {
                if (SDriverX.g_modelInfo.sErr_msg.Contains("ReSet"))
                {
                    SetMsg(SDriverX.g_modelInfo.sErr_msg + ",请返回[Reset]站。", UDF.COLOR.FAIL);
                    return false;
                }
            }
            SetMsg("检查SN信息...", UDFs.UDF.COLOR.WORK);
            //bErr = Oracle.ServerExecute("select PRODUCTSPECNAME, PRODUCTREQUESTNAME from product where SET_SERIAL_NO = '" + sn + "'", out rdr);
            //if (bErr)
            //{
            //    rdr.Read();
            //    if (rdr.HasRows)
            //    {
            //        SDriverX.g_modelInfo.sPart_no = rdr[0].ToString();
            //        SDriverX.g_modelInfo.sWord = rdr[1].ToString();
            //        textBoxPn.Text = SDriverX.g_modelInfo.sPart_no;
            //    }
            //    else
            //    {
            //        SetMsg("SN信息为空！KPPN未作业，请返回[KPPN]站。", UDF.COLOR.FAIL);
            //        return false;
            //    }
            //}
            //else
            //{
            //    SetMsg("数据库操作失败！(检查网络连接或重启程序)", UDF.COLOR.FAIL);
            //    return false;
            //}

            
            //bErr = Oracle.ServerExecute("SELECT * FROM rknmgr.progdata where trid = '" + sn + "' and prog_sta='RST' ORDER BY PROG_DATE ", out rdr);
            //if (bErr)
            //{
            //    rdr.Read();
            //    if (rdr.HasRows)
            //    {

            //    }
            //    else
            //    {
            //        SetMsg("没有Reset站记录，请返回[Reset]站。", UDF.COLOR.FAIL);
            //        return false;
            //    }
            //}
            //else
            //{
            //    SetMsg("数据库操作失败！(检查网络连接或重启程序)", UDF.COLOR.FAIL);
            //    return false;
            //}

            return true;
        }
        /// <summary>
        /// 流程卡验证函数,验证当前机台序列号所在站别是否在本站
        /// </summary>
        /// <param name="sn"></param>
        /// <returns></returns>
        ///
        bool PreCheckTRID(string sn)
        {
            bool bFail =false;
            if (g_runMode >= UDF.RunMode.PRE && g_runMode <= UDF.RunMode.MIC && GlobalConfig.iRunMode == 1)
            {
                SetMsg("檢查MES, Kiểm tra trạm MES", UDF.COLOR.WORK);

                if (GlobalConfig.bNew_Mes == false)
                {
                    if (SDriverX.STCK(GlobalConfig.sMesEid, sn, "TestUser")==false)
                    {
                        bFail =true; 
                    }
                }

                if (bFail==false)
                {
                    if ((SDriverX.g_modelInfo.sPart_no != textBoxPn.Text) && (textBoxPn.Text != string.Empty))
                    {
                        g_iPtInitFlag = INIT_ZERO;
                    }
                    if (textBoxModel.InvokeRequired)
                    {
                        textBoxModel.Invoke(new Action(() => textBoxModel.Text = SDriver.SDriverX.g_modelInfo.sModel));
                    }
                    else 
                    {
                        textBoxModel.Text = SDriver.SDriverX.g_modelInfo.sModel;
                    }

                    if (textBoxPn.InvokeRequired)
                    {
                        textBoxPn.Invoke(new Action(() => textBoxPn.Text = SDriver.SDriverX.g_modelInfo.sPart_no));
                    }
                    else
                    {
                        textBoxPn.Text = SDriver.SDriverX.g_modelInfo.sPart_no;
                    }
                    
                    if (SDriver.SDriverX.g_modelInfo.sTv_offline == "Y")
                    {
                        SetMsg("流程卡验证失败！离线机台", UDF.COLOR.FAIL);
                        return false;
                    }
                }
                else
                {
                    SetMsg(SDriver.SDriverX.g_modelInfo.sErr_msg ?? "网络故障！未接收到 SDriver 服务器消息", UDF.COLOR.FAIL);
                    return false;
                }
            }
            else
            {
                SetMsg("不檢查MES, Không kiểm tra MES", UDF.COLOR.WORK);
                SDriver.SDriverX.g_modelInfo = new UDF.ModelInfo
                {
                    sSn = sn,
                    sModelName = "DEFAULT"
                };
            }

            return true;
        }
        bool Cycle
        {
            get
            {
                return timerCycle.Enabled;
            }

            set
            {
                if (!value)
                {
                    g_bWorkFlag = false;
                    g_sCmdRtnStrText = string.Empty;
                    g_iRcvPortLogNum = 0;

                    g_iTimerCycle = 0; //重置计时
                    textBoxSn.Enabled = true;   //解锁 SN 框
                    //buttonSetting.Enabled = true;   //解锁 设置按钮
                    buttonExit.Enabled = true;  //解锁 退出 按钮
                    //buttonGdmSend.Enabled = true;   //解锁 GDMSend 按钮
                    //checkBoxAutoRF.Enabled = true;  //解锁 自动RF
                    timerCycle.Enabled = value; //停止 Timer 计时
                    return;
                }
                g_bWorkFlag = true;
                textBoxSn.Enabled = false;  //锁定 SN 框
                //buttonSetting.Enabled = false;  //锁定 设置 按钮
                //buttonExit.Enabled = false; //锁定 退出 按钮
                //buttonGdmSend.Enabled = false;   //锁定 GDMSend 按钮
                //checkBoxAutoRF.Enabled = false;     //锁定 自动 RF
                timerCycle.Enabled = value; //激活 Timer 计时
            }
        }
        /// <summary>
        /// 初始化界面显示
        /// </summary>
        void InitUI()
        {
            //if ((g_bAutoScan == true))
            //{
            //    //Delay(1000);
            //    timertest.Enabled = true;
            //}
            //else
            //{
            //    timertest.Enabled = false;
            //}
            Cycle = true;   // 开始计时
            ClearMsg(); //清空步骤列表框
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() =>
                {
                    this.textBoxCycle.Text = "0";  
                    this.textBoxFW.Text = "";
                    this.textBoxUn.Text = "";
                    this.textBoxDC.Text = "";
                }));
            }
            else
            {
                this.textBoxCycle.Text = "0";   
                this.textBoxFW.Text = "";
                this.textBoxUn.Text = "";
                this.textBoxDC.Text = "";
            }
        }
        /// <summary>
        /// 重置界面显示
        /// </summary>
        void ResetUI()
        {
            Cycle = false;  //停止计时
            //重置焦点输入
            FalseReset();
            if ((g_bAutoScan == true))
            {
                Delay(500);
                timertest.Enabled = true;
                timerIO_CARD.Enabled =true;
            }
            else
            {
                timertest.Enabled = false;
                this.textBoxSn.Focus();
                this.textBoxSn.SelectAll();
            }
            return;
        }
        void FalseReset()
        {
            g_bXM_RZ3L = false;
            g_bXM_RZ3L = false;
            g_bXM_RZ4L = false;
            g_bXM_RZ5L = false;
            g_bXM_RZ6L = false;
            g_bXM_RZ7L = false;
            g_bXM_RZ8L = false;
            g_bXM_RZ9L = false;
            g_bXM_RZAL = false;
            g_bXM_RZBL = false;
            g_bXM_RZCL = false;
            g_bXM_DZ3L = false;
            g_bXM_DZ4L = false;
            g_bXM_DZ5L = false;
            g_bXM_DZJL = false;
            g_bXM_DZ7L = false;
            g_bXM_DZ8L = false;
            g_bXM_DZ9L = false;
            g_bXM_DZCL = false;
            g_bXM_DZDL = false;
            g_bXM_DZEL = false;
            g_bXM_DZFL = false;
            g_bXM_DZGL = false;
            g_bXM_DZHL = false;
            g_bXM_DZIL = false;

            g_bXM_DZLL = false;
            g_bXM_DZKL = false;
            g_bXM_DZNL = false;
            g_bXM_DZML = false;
            g_bXM_DZQL = false;

            g_bXM_WZ2L = false;
            g_bXM_WZ3L = false;
            g_bXM_WZ4L = false;
            g_bXM_WZ5L = false;
            g_bXM_WZ6L = false;
            g_bXM_WZ7L = false;
        }
        /// <summary>
        /// DataGridView 初始化
        /// Vizio Soundbar Style
        /// </summary>
        void VizioSoundbarDataStyleInit()
        {
            int iDataGridWidth = new int();

            dataGridViewTestData.RowHeadersVisible = false;
            dataGridViewTestData.Rows.Clear();
            dataGridViewTestData.Columns.Clear();

            iDataGridWidth = this.dataGridViewTestData.Width;
            dataGridViewTestData.Columns.Add("Col1", "序号");
            dataGridViewTestData.Columns["Col1"].Width = Convert.ToInt32(iDataGridWidth * 0.15f);
            dataGridViewTestData.Columns.Add("Col2", "机台 SN");
            dataGridViewTestData.Columns["Col2"].Width = Convert.ToInt32(iDataGridWidth * 0.25f);
            dataGridViewTestData.Columns.Add("Col3", "C-T");
            dataGridViewTestData.Columns["Col3"].Width = Convert.ToInt32(iDataGridWidth * 0.15f);
            dataGridViewTestData.Columns.Add("Col4", "测试时间");
            dataGridViewTestData.Columns["Col4"].Width = Convert.ToInt32(iDataGridWidth * 0.45f);

            g_complexConSetting.sDataGradView = this.dataGridViewTestData.RowHeadersWidth + ",";
            for (int i = 0; i < dataGridViewTestData.ColumnCount; i++)
            {
                g_complexConSetting.sDataGradView += (this.dataGridViewTestData.Columns[i].Width / g_fWidthScaling) + ",";
            }
        }
        /// <summary>
        /// 添加测试数据显示
        /// Vizio Soundbar Style
        /// </summary>
        /// <param name="sn">机台 SN</param>
        /// <param name="ct">测试耗时</param>
        /// 
        void AddDataVizioSoundbar(string sn, int ct)
        {
            int index = new int();

            if (this.dataGridViewTestData.RowCount > 200)
            {
                this.dataGridViewTestData.Rows.Clear();
            }
            index = this.dataGridViewTestData.Rows.Add();
            this.dataGridViewTestData.Rows[index].Cells[0].Value = dataGridViewTestData.RowCount - 1;
            this.dataGridViewTestData.Rows[index].Cells[1].Value = sn;
            this.dataGridViewTestData.Rows[index].Cells[2].Value = ct;
            this.dataGridViewTestData.Rows[index].Cells[3].Value = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            this.dataGridViewTestData.FirstDisplayedScrollingRowIndex = this.dataGridViewTestData.Rows[index].Index;
            this.dataGridViewTestData.Rows[index].Selected = true;
        }
        /// <summary>
        /// 查询程序工作状态
        /// </summary>
        bool IsWorking
        {
            get
            {
                return g_bCmdWorkFlag;
            }
        }
        void ShowTvPortMsg_Internal(string msg)
        {
            textBoxAck.AppendText(msg);
        }
        bool WriteRs232CommandS12(bool resetRtnFlag, string cmd, int cycleDelay)
        {
            if (g_bCmdWorkFlag)
            {
                return false;
            }
            g_bCmdWorkFlag = true;

            uint retryCount = new uint();
            g_bPortErr = false;


            //重置
            if (resetRtnFlag)
            {
                g_sCmdRtnStrText = String.Empty;
            }
            //尝试打开端口
            try
            {
                //if (!serialPortTV.IsOpen)
                //{
                //    serialPortTV.Open();
                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                g_bPortErr = true;
                g_bCmdWorkFlag = false;
                return false;
            }
            //初始化串口
            //serialPortTV.DiscardInBuffer();
            //serialPortTV.DiscardOutBuffer();

            //serialPortTV.WriteLine(cmd);    //发送命令

            Delay(40);

            //等待命令回传
            try
            {
                do
                {
                    retryCount++;
                    if (retryCount > 25 + cycleDelay)
                    {
                        g_bCmdWorkFlag = false;
                        return false;
                    }
                    System.Windows.Forms.Application.DoEvents(); //处理消息队列
                    Thread.Sleep(200);
                    System.Windows.Forms.Application.DoEvents();
                } while (Regex.Match(g_sCmdRtnStrText, @"root\@\(none\):/#").Success == false); //root\@\(none\):/#|MSTC_R_SystemInitDone|root\@mico:/#
            }
            catch (Exception ex)
            {
                g_bCmdWorkFlag = false;
                MessageBox.Show(null, "等待命令回传失败\n" + ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            g_bCmdWorkFlag = false;
            return true;
        }
        bool ExitFactMode_S12(bool resetRtnFlag, string cmd, int cycleDelay)
        {
            if (g_bCmdWorkFlag)
            {
                return false;
            }
            g_bCmdWorkFlag = true;

            uint retryCount = new uint();
            g_bPortErr = false;


            //重置
            if (resetRtnFlag)
            {
                g_sCmdRtnStrText = String.Empty;
            }
            //尝试打开端口
            try
            {
                //if (!serialPortTV.IsOpen)
                //{
                //    serialPortTV.Open();
                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                g_bPortErr = true;
                g_bCmdWorkFlag = false;
                return false;
            }
            //初始化串口
            //serialPortTV.DiscardInBuffer();
            //serialPortTV.DiscardOutBuffer();

            //serialPortTV.WriteLine(cmd);    //发送命令

            Delay(40);

            //等待命令回传
            try
            {
                do
                {
                    retryCount++;
                    if (retryCount > 25 + cycleDelay)
                    {
                        g_bCmdWorkFlag = false;
                        return false;
                    }
                    System.Windows.Forms.Application.DoEvents(); //处理消息队列
                    Thread.Sleep(200);
                    System.Windows.Forms.Application.DoEvents();
                } while (Regex.Match(g_sCmdRtnStrText, @"name: mac_bt, size 17").Success == false);
            }
            catch (Exception ex)
            {
                g_bCmdWorkFlag = false;
                MessageBox.Show(null, "等待命令回传失败\n" + ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            g_bCmdWorkFlag = false;
            return true;
        }
        bool EnterSleepModeS12()
        {
            if (g_bCmdWorkFlag)
            {
                return false;
            }
            g_bCmdWorkFlag = true;

            uint retryCount = new uint();
            g_bPortErr = false;


            //重置
            if (true)
            {
                g_sCmdRtnStrText = String.Empty;
            }
            g_bPortErr = false;
            //尝试打开端口
            try
            {
                //if (!serialPortTV.IsOpen)
                //{
                //    serialPortTV.Open();
                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                g_bPortErr = true;
                g_bCmdWorkFlag = false;
                return false;
            }
            //初始化串口
            //serialPortTV.DiscardInBuffer();
            //serialPortTV.DiscardOutBuffer();

            //serialPortTV.WriteLine("MSTC_SLP");    //发送命令

            Delay(40);

            //等待命令回传
            try
            {
                do
                {
                    retryCount++;
                    if (retryCount > 25)
                    {
                        g_bCmdWorkFlag = false;
                        return false;
                    }
                    System.Windows.Forms.Application.DoEvents(); //处理消息队列
                    Thread.Sleep(200);
                    System.Windows.Forms.Application.DoEvents();
                } while (Regex.Match(g_sCmdRtnStrText, @"enter suspend").Success == false);
            }
            catch (Exception ex)
            {
                g_bCmdWorkFlag = false;
                MessageBox.Show(null, "等待命令回传失败\n" + ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            g_bCmdWorkFlag = false;
            return true;
        }
        void ResetCmdRtnStrText()
        {
            g_sCmdRtnStrText = String.Empty;
        }
        bool ResultCheck(string pattern)
        {
            return Regex.Match(g_sCmdRtnStrText, pattern).Success;
        }
        bool ResultCheck(string strSrouce, string pattern)
        {
            return Regex.Match(strSrouce, pattern).Success;
        }
        bool ResultPick(string strSrouce, string pattern, out string target)
        {
            target = string.Empty;
            Match match = Regex.Match(strSrouce, pattern);
            if (match.Success)
                target = match.Value.ToString();
            else
                return false;

            return true;
        }
        bool WriteIOCard(string cmdStr, int delay)
        {
            if (g_bCmdWorkFlag) return false;
            g_bCmdWorkFlag = true;

            //定义变量
            String strBuf = cmdStr.Trim().Replace(" ", "");
            Byte[] byCmdBuf = new Byte[6];
            int i = 0, j = 0;

            if (strBuf.Length < 10)
            {
                //不合法的指令长度
                g_bCmdWorkFlag = false;
                return false;
            }

            j = 0;
            for (i = 0; i < 5; i++)
            {
                byCmdBuf[i] = Convert.ToByte("0x" + strBuf.Substring(i * 2, 2), 16);
                j += byCmdBuf[i];
            }
            byCmdBuf[5] = (Byte)(j & 0xff);

            g_bPortErr = false;
            //尝试打开串口
            try
            {
                //if (!serialPortIOCard.IsOpen)
                //{
                //    serialPortIOCard.Open();
                //}
                //serialPortIOCard.DiscardInBuffer();
                //serialPortIOCard.DiscardOutBuffer();
                //serialPortIOCard.ReadTimeout = 5000;
                //serialPortIOCard.WriteTimeout = 5000;
            }
            catch (Exception ex)
            {
                g_bPortErr = true;
                g_bCmdWorkFlag = false;
                // MessageBox.Show(null, ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                SetMsg("提示" + ex.Message, UDF.COLOR.FAIL);
                return false;
            }

            //serialPortIOCard.Write(byCmdBuf, 0, 6);

            if (delay != 0) Delay(delay);
            g_bCmdWorkFlag = false;
            return true;
        }
        bool GetCp310Power(out float power)
        {
            power = 0.0f;
            if (g_bCmdWorkFlag) return false;
            g_bCmdWorkFlag = true;

            //定义变量
            Byte[] byCmdBuf = new Byte[4] { 0x3F, 0x57, 0x3B, 0x0D };
            Byte[] byRtnBuf = new byte[16];
            String strBuf = string.Empty;
            int retryTime = new int();
            Match match;

            g_bPortErr = false;
            //尝试打开串口
            try
            {
                //if (!serialPortSick.IsOpen)
                //{
                //    serialPortSick.Open();
                //}
                //serialPortSick.DiscardInBuffer();
                //serialPortSick.DiscardOutBuffer();
                //serialPortSick.ReadTimeout = 5000;
                //serialPortSick.WriteTimeout = 5000;
            }
            catch (Exception ex)
            {
                g_bCmdWorkFlag = false;
                g_bPortErr = true;
                MessageBox.Show(null, ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }

            //serialPortSick.Write(byCmdBuf, 0, 4);

            //do
            //{
            //    if (retryTime > 20)
            //    {
            //        g_bCmdWorkFlag = false;
            //        return false;
            //    }
            //    retryTime++;
            //    Delay(200);
            //} while (serialPortSick.BytesToRead < 6);

            //serialPortSick.Read(byRtnBuf, 0, 16);
            strBuf = Encoding.Default.GetString(byRtnBuf);
            strBuf = strBuf.TrimEnd('\0').Replace("00.", "0.");
            match = Regex.Match(strBuf, @"\d{1,}\.\d{1,}");
            if (match.Success)
            {
                power = Convert.ToSingle(match.Value);
            }

            g_bCmdWorkFlag = false;
            return true;
        }
        bool WriteCommandToGdm(bool resetRtnFlag, string cmd, int cycleDelay)
        {
            if (g_bCmdWorkFlag) return false;
            g_bCmdWorkFlag = true;

            //定义变量
            //Byte[] byCmdBuf = new Byte[cmd.Length/2];
            Byte[] byRtnBuf = new byte[32];
            string strRtn = string.Empty;
            String strBuf = string.Empty;
            int retryTime = new int();

            g_bPortErr = false;
            //尝试打开串口
            try
            {
                //if (!serialPortGDM.IsOpen)
                //{
                //    serialPortGDM.Open();
                //}
                //serialPortGDM.DiscardInBuffer();
                //serialPortGDM.DiscardOutBuffer();
                //serialPortGDM.ReadTimeout = 5000;
                //serialPortGDM.WriteTimeout = 5000;
            }
            catch (Exception ex)
            {
                g_bPortErr = true;
                g_bCmdWorkFlag = false;
                MessageBox.Show(null, ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }

            if (resetRtnFlag) g_sGdmRtnText = String.Empty;

            //serialPortGDM.WriteLine(cmd);
            do
            {
                if (retryTime > 20 + cycleDelay)
                {
                    g_bCmdWorkFlag = false;
                    return false;
                }
                retryTime++;
                System.Windows.Forms.Application.DoEvents();
                Thread.Sleep(200);
                System.Windows.Forms.Application.DoEvents();
            } while (!Regex.Match(g_sGdmRtnText, @">\x0D\x0A").Success);

            g_bCmdWorkFlag = false;
            return true;
        }   //WriteCommandToGdm end
        bool OpenDoor()
        {
            if (g_bCmdWorkFlag) return false;
            g_bCmdWorkFlag = true;

            g_bPortErr = false;
            //尝试打开串口
            try
            {
                //if (!serialPortDoor.IsOpen) serialPortDoor.Open();
                //serialPortDoor.DiscardInBuffer();
                //serialPortDoor.DiscardOutBuffer();
                //serialPortDoor.ReadTimeout = 5000;
                //serialPortDoor.WriteTimeout = 5000;
            }
            catch (Exception ex)
            {
                g_bPortErr = true;
                g_bCmdWorkFlag = false;
                MessageBox.Show(null, ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }

            //serialPortDoor.Write("OPEN\r\n");

            g_bCmdWorkFlag = false;
            return true;
        }
        void Insert_ORALCEdatabase(string str)
        {
            //if (Oracle.UpdateServer("insert into rknmgr.progdata (trid,line_code,prog_info,prog_sta,okng) values ('" + SDriverX.g_modelInfo.sSn + "','" + GlobalConfig.sMesEid.Substring(5, 5) + "','"+ str + "','" + GlobalConfig.sMesEid.Substring(2, 3) + "', '"+ strOKNG +"')") == false)
            //{
            //    SetMsg("上传测试记录失败", UDF.COLOR.FAIL);
            //}
        }
        void Insert_BITIME()
        {
            //DateTime dt1 = DateTime.Now;

            string sql = string.Empty;
            string strmac = string.Empty;
            bool bErr = new bool();
            string db = string.Empty;

            GetOracletime();
            OleDbDataReader reader;
            sql = string.Format("select * from rknmgr.insp_bi_time t where t.trid='" + SDriverX.g_modelInfo.sSn + "'");
            //bErr = Oracle.ServerExecute(sql, out reader);
            //if (bErr)
            //{
            //    reader.Read();
            //    if (reader.HasRows)
            //    {
            //        //已有数据
            //        if (Oracle.UpdateServer("update rknmgr.insp_bi_time set BI_TIME=to_date('" + g_dOracle_time + "', 'yyyy-mm-dd hh24:mi:ss') where trid='" + SDriverX.g_modelInfo.sSn + "'") == false)
            //        {
            //            SetMsg("上传BI_TIME记录失败", UDF.COLOR.FAIL);
            //        }
            //    }
            //    else
            //    {
            //        //没有数据
            //        if (Oracle.UpdateServer("insert into rknmgr.insp_bi_time (trid,bi_time) values ('" + SDriverX.g_modelInfo.sSn + "',sysdate)") == false)
            //        {
            //            SetMsg("上传BI_TIME记录失败", UDF.COLOR.FAIL);
            //        }
            //    }
            //    reader.Close();
            //}
        }
        bool CloseDoor()
        {
            if (g_bCmdWorkFlag) return false;
            g_bCmdWorkFlag = true;

            int retryCount = new int();
            byte[] tmp = new byte[8];

            Array.Clear(tmp, 0, 8);
            g_bPortErr = false;
            //尝试打开串口
            //try
            //{
            //    if (!serialPortDoor.IsOpen) serialPortDoor.Open();
            //    serialPortDoor.DiscardInBuffer();
            //    serialPortDoor.DiscardOutBuffer();
            //    serialPortDoor.ReadTimeout = 5000;
            //    serialPortDoor.WriteTimeout = 5000;
            //}
            //catch (Exception ex)
            //{
            //    g_bPortErr = true;
            //    g_bCmdWorkFlag = false;
            //    MessageBox.Show(null, ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            //    return false;
            //}

            //serialPortDoor.Write("CLOSE\r\n");

            try
            {
                //do
                //{
                //    retryCount += 1;
                //    if (retryCount > 100)
                //    {
                //        g_bCmdWorkFlag = false;
                //        return false;
                //    }
                //    System.Windows.Forms.Application.DoEvents(); //处理消息队列
                //    Thread.Sleep(50);
                //    if (g_iCmdDelay != 0)
                //    {
                //        System.Windows.Forms.Application.DoEvents();
                //        Thread.Sleep(g_iCmdDelay);
                //        g_iCmdDelay = 0;
                //    }
                //} while (serialPortDoor.BytesToRead < 5);
                //serialPortDoor.Read(tmp, 0, 5);  //get ack
            }
            catch (Exception e)
            {
                g_bCmdWorkFlag = false;
                MessageBox.Show(null, "Get ACK Fail\n" + e.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (Encoding.Default.GetString(tmp).TrimEnd('\0') != "READY")
            {
                g_bCmdWorkFlag = false;
                return false;
            }

            g_bCmdWorkFlag = false;
            return true;
        }
        bool UpdataSpecFromServer()
        {
            String sStrBuf = string.Empty;

            //FW
            sStrBuf = GlobalConfig.ReadIniFile("TEST", "FW", @"\\192.168.158.25\te\setup\FTTEST\spec\config.ini");
            if (sStrBuf.Length == 0)
            {
                SetMsg(@"\\192.168.158.25\te\setup\FTTEST\spec\config.ini 未找到 FW 规格，请向 TE 报告此问题", UDF.COLOR.FAIL);
                return false;
            }
            else
            {
                GlobalConfig.sFw = sStrBuf.ToUpper().Replace(" ", "");
            }

            //FW2
            sStrBuf = GlobalConfig.ReadIniFile("TEST", "FW2", @"\\192.168.158.25\te\setup\FTTEST\spec\config.ini");
            if (sStrBuf.Length == 0)
            {
                SetMsg(@"\\192.168.158.25\te\setup\FTTEST\spec\config.ini 上未找到 FW2 规格，请向 TE 报告此问题", UDF.COLOR.FAIL);
                return false;
            }
            else
            {
                GlobalConfig.sFw2 = sStrBuf.ToUpper().Replace(" ", "");
            }

            g_iPtInitFlag |= INIT_GETTESTITEM;
            return true;
        }
        void InitGridFormatA()
        {
            int iDataGridWidth = new int();

            dataGridViewTestData.RowHeadersVisible = false;
            dataGridViewTestData.Rows.Clear();
            dataGridViewTestData.Columns.Clear();

            iDataGridWidth = this.dataGridViewTestData.Width;
            dataGridViewTestData.Columns.Add("Col1", "No.");
            dataGridViewTestData.Columns["Col1"].Width = Convert.ToInt32(iDataGridWidth * 0.10f);
            dataGridViewTestData.Columns.Add("Col2", "SN");
            dataGridViewTestData.Columns["Col2"].Width = Convert.ToInt32(iDataGridWidth * 0.25f);
            dataGridViewTestData.Columns.Add("Col3", "Item");
            dataGridViewTestData.Columns["Col3"].Width = Convert.ToInt32(iDataGridWidth * 0.20f);
            dataGridViewTestData.Columns.Add("Col4", "OK/NG");
            dataGridViewTestData.Columns["Col4"].Width = Convert.ToInt32(iDataGridWidth * 0.10f);
            dataGridViewTestData.Columns.Add("Col5", "Test time");
            dataGridViewTestData.Columns["Col5"].Width = Convert.ToInt32(iDataGridWidth * 0.40f);

            g_complexConSetting.sDataGradView = this.dataGridViewTestData.RowHeadersWidth + ",";
            for (int i = 0; i < dataGridViewTestData.ColumnCount; i++)
            {
                g_complexConSetting.sDataGradView += (this.dataGridViewTestData.Columns[i].Width / g_fWidthScaling) + ",";
            }
        }
        void AddDataA(string sn, string item, string flag)
        {
            int index = new int();

            if (this.dataGridViewTestData.RowCount > 200)
            {
                this.dataGridViewTestData.Rows.Clear();
            }
            index = this.dataGridViewTestData.Rows.Add();
            this.dataGridViewTestData.Rows[index].Cells[0].Value = dataGridViewTestData.RowCount - 1;
            this.dataGridViewTestData.Rows[index].Cells[1].Value = sn;
            this.dataGridViewTestData.Rows[index].Cells[2].Value = item;
            this.dataGridViewTestData.Rows[index].Cells[3].Value = flag;
            this.dataGridViewTestData.Rows[index].Cells[4].Value = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            this.dataGridViewTestData.FirstDisplayedScrollingRowIndex = this.dataGridViewTestData.Rows[index].Index;
            this.dataGridViewTestData.Rows[index].Selected = true;
        }
        void InitGridFormatB()
        {
            int iDataGridWidth = new int();

            dataGridViewTestData.RowHeadersVisible = false;
            dataGridViewTestData.Rows.Clear();
            dataGridViewTestData.Columns.Clear();

            iDataGridWidth = this.dataGridViewTestData.Width;
            dataGridViewTestData.Columns.Add("Col1", "No.");
            dataGridViewTestData.Columns["Col1"].Width = Convert.ToInt32(iDataGridWidth * 0.15f);
            dataGridViewTestData.Columns.Add("Col2", "SN");
            dataGridViewTestData.Columns["Col2"].Width = Convert.ToInt32(iDataGridWidth * 0.25f);
            dataGridViewTestData.Columns.Add("Col3", "C-T");
            dataGridViewTestData.Columns["Col3"].Width = Convert.ToInt32(iDataGridWidth * 0.15f);
            dataGridViewTestData.Columns.Add("Col4", "Test time");
            dataGridViewTestData.Columns["Col4"].Width = Convert.ToInt32(iDataGridWidth * 0.45f);

            g_complexConSetting.sDataGradView = this.dataGridViewTestData.RowHeadersWidth + ",";
            for (int i = 0; i < dataGridViewTestData.ColumnCount; i++)
            {
                g_complexConSetting.sDataGradView += (this.dataGridViewTestData.Columns[i].Width / g_fWidthScaling) + ",";
            }
        }
        void AddDataB(string sn, int ct)
        {
            int index = new int();

            if (this.dataGridViewTestData.InvokeRequired)
            {
                dataGridViewTestData.Invoke(new Action(() =>
                {
                    index = this.dataGridViewTestData.Rows.Add();
                    this.dataGridViewTestData.Rows[index].Cells[0].Value = dataGridViewTestData.RowCount - 1;
                    this.dataGridViewTestData.Rows[index].Cells[1].Value = sn;
                    this.dataGridViewTestData.Rows[index].Cells[2].Value = ct;
                    this.dataGridViewTestData.Rows[index].Cells[3].Value = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                    this.dataGridViewTestData.FirstDisplayedScrollingRowIndex = this.dataGridViewTestData.Rows[index].Index;
                    this.dataGridViewTestData.Rows[index].Selected = true;
                }));
            }
            else
            {
                index = this.dataGridViewTestData.Rows.Add();
                this.dataGridViewTestData.Rows[index].Cells[0].Value = dataGridViewTestData.RowCount - 1;
                this.dataGridViewTestData.Rows[index].Cells[1].Value = sn;
                this.dataGridViewTestData.Rows[index].Cells[2].Value = ct;
                this.dataGridViewTestData.Rows[index].Cells[3].Value = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                this.dataGridViewTestData.FirstDisplayedScrollingRowIndex = this.dataGridViewTestData.Rows[index].Index;
                this.dataGridViewTestData.Rows[index].Selected = true;
            }
        }
        bool RunAdbCmd(string cmd, out string ack, int skipOption = 0)
        {
            ack = string.Empty;
            try
            {
                if (textBoxAck.InvokeRequired)
                {
                    textBoxAck.BeginInvoke(new Action(() =>
                    {
                        textBoxAck.AppendText("CMD:\r\n" + cmd + "\r\n");
                    }));
                }
                else
                {
                    textBoxAck.AppendText("CMD:\r\n" + cmd + "\r\n"); //cmd
                }

                Process process = new Process();
                process.StartInfo.FileName = "cmd";
                process.StartInfo.Arguments = "/c "+ cmd; // "adb shell cat /sdcard/Download/command_ack.txt"
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.RedirectStandardOutput = true;
                
                process.Start();
                process.PriorityClass = ProcessPriorityClass.BelowNormal;
                process.WaitForExit();
                if ((skipOption & 0x01) == 0)
                {
                    string result = process.StandardOutput.ReadToEnd();
                    ack = result.Replace("\r\n"," ").Replace("\n"," ");
                    if (textBoxAck.InvokeRequired)
                    {
                        textBoxAck.BeginInvoke(new Action(() =>
                        {
                            textBoxAck.AppendText("ACK:\r\n" + result + "\r\n");
                        }));
                    }
                    else
                    {
                        textBoxAck.AppendText("ACK:\r\n" + result + "\r\n"); //cmd
                    }
                }
            }
            catch (Exception e)
            {
                SetMsg(e.Message, UDF.COLOR.FAIL);
                return false;
            }

            return true;
        }
        bool RunAdbCmd(string cmd, string query_ack, out string ack, int skipOption = 0)
        {
            ack = string.Empty;

            //CMD
            try
            {
                textBoxAck.AppendText("CMD:\r\n" + cmd + "\r\n"); //cmd

                Process process = new Process();
                process.StartInfo.FileName = "cmd";
                process.StartInfo.Arguments = " /c " + cmd; // " /c adb shell cat /sdcard/Download/command_ack.txt"
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.Start();
                Application.DoEvents();
                process.WaitForExit();
                if ((skipOption & 0x01) == 0)
                {
                    ack = process.StandardOutput.ReadToEnd();
                    textBoxAck.AppendText("ACK:\r\n" + ack + "\r\n");
                }
            }
            catch (Exception e)
            {
                SetMsg(e.Message, UDF.COLOR.FAIL);
                return false;
            }

            Delay(700);

            //QUERY ACK
            try
            {
                textBoxAck.AppendText("CMD:\r\n" + query_ack + "\r\n"); //cmd

                Process process = new Process();
                process.StartInfo.FileName = "cmd";
                process.StartInfo.Arguments = " /c " + query_ack; // " /c adb shell cat /sdcard/Download/command_ack.txt"
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.Start();
                Application.DoEvents();
                process.WaitForExit();
                if ((skipOption & 0x02) == 0)
                {
                    ack = process.StandardOutput.ReadToEnd();
                    textBoxAck.AppendText("ACK:\r\n" + ack + "\r\n");
                }
            }
            catch (Exception e)
            {
                SetMsg(e.Message, UDF.COLOR.FAIL);
                return false;
            }


            return true;
        }
        bool IO_Card_Output(string strPort,int idelay)
        {
            timerIO_CARD.Enabled = false;
            switch (strPort)
            {
                case "1":
                    Write_IOCARD_OUT("AA BB 01 01 01 68", 6, 20, 6);
                    Delay(idelay);
                    Write_IOCARD_OUT("AA BB 01 01 00 67", 6, 20, 6);
                    break;
                case "2":
                    Write_IOCARD_OUT("AA BB 01 02 01 69", 6, 20, 6);
                    Delay(idelay);
                    Write_IOCARD_OUT("AA BB 01 02 00 68", 6, 20, 6);
                    break;
                case "3":
                    Write_IOCARD_OUT("AA BB 01 03 01 6A", 6, 20, 6);
                    Delay(idelay);
                    Write_IOCARD_OUT("AA BB 01 03 00 69", 6, 20, 6);
                    break;
                case "4":
                    Write_IOCARD_OUT("AA BB 01 04 01 6B", 6, 20, 6);
                    Delay(idelay);
                    Write_IOCARD_OUT("AA BB 01 04 00 6A", 6, 20, 6);
                    break;
                default:
                    break;
            }
            return true;
        }
        bool IO_Card_Intput(string strPort, int idelay)
        {
            switch (strPort)
            {
                case "1":
                    Write_IOCARD_OUT("AA BB 01 01 01 68", 6, 20, 6);
                    Delay(idelay);
                    Write_IOCARD_OUT("AA BB 01 01 00 67", 6, 20, 6);
                    break;
                case "2":
                    Write_IOCARD_OUT("AA BB 01 02 01 69", 6, 20, 6);
                    Delay(idelay);
                    Write_IOCARD_OUT("AA BB 01 02 00 68", 6, 20, 6);
                    break;
                case "3":
                    Write_IOCARD_OUT("AA BB 01 03 01 6A", 6, 20, 6);
                    Delay(idelay);
                    Write_IOCARD_OUT("AA BB 01 03 00 69", 6, 20, 6);
                    break;
                case "4":
                    Write_IOCARD_OUT("AA BB 01 04 01 6B", 6, 20, 6);
                    Delay(idelay);
                    Write_IOCARD_OUT("AA BB 01 04 00 6A", 6, 20, 6);
                    break;
                default:
                    break;
            }
            return true;
        }
        void  IO_Card_Reset()
        {
            //AA BB 01 1C 01 83   关闭全部继电器 AA BB 01 DE 01 45   清除保存的继电器状态，上电默认全关
            Write_IOCARD_OUT("AA BB 01 1C 01 83", 6, 20, 6);
            Delay(50);
            Write_IOCARD_OUT("AA BB 01 DE 01 45", 6, 20, 6);
        }
        public static void GetOracletime()
        {
            string strOracle = string.Empty;
            OleDbDataReader reader;

            //sql = string.Format("select * from rknmgr.insp_bi_time t where t.trid='" + SDriverX.g_modelInfo.sSn + "'");
            //bErr = Oracle.ServerExecute(sql, out reader);
            //if (Oracle.ServerExecute("select sysdate from dual", out reader) == true)
            //{ 
            //    reader.Read();
            //    g_dOracle_time = Convert.ToDateTime(reader[0]);
            //    reader.Close();
            //}
        }
        public static void GetSqltime()
        {
            string strOracle = string.Empty;
            SqlDataReader reader = null;

            //sql = string.Format("select * from rknmgr.insp_bi_time t where t.trid='" + SDriverX.g_modelInfo.sSn + "'");
            //bErr = Oracle.ServerExecute(sql, out reader);
            Sql.DefaultDatabase = Sql.DatabaseOpt.RAKEN_SFIS;
            if (Sql.ServerExecute("select getdate() as date", out reader) == true)
            {
                try
                {
                    reader.Read();
                    g_dSql_time = Convert.ToDateTime(reader[0]);
                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(null, ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
        }
        private void Process_Exited(object sender, EventArgs e)
        {
            if (!g_bAdbProcessExited) g_bAdbProcessExited = true;
        }
        bool run_cmd_and_check_link(string rst, int retry, string cmd)
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
                Thread.Sleep(1000);
            }
            if (rst.Trim().Length > 24  || rst == "") return true;
            else return false;
        }
        bool check_command_ack(string test,string filename = "command_ack.txt",string compareString = "", string rst = default, int retry = 0)
        {
            SetMsg($"取得 {test} 測試結果, Thu thập kết quả kiểm tra wifi", UDF.COLOR.WORK);
            SetMsg($"{filename} 不存在, 請等候,{filename} không tồn tại, vui lòng chờ đợi", UDF.COLOR.WORK);
            while (!File.Exists(filename))
            {
                Thread.Sleep(1000);
                RunAdbCmd($"adb pull /sdcard/Download/{filename}", out rst);
                retry++;
                if (retry > 30) break;
            }

            if (retry > 30)
            {
                SetMsg($"{filename} 讀取失敗, FAIL, {filename} Đọc thất bại.", UDF.COLOR.FAIL);
                return false;
            }
            else { retry = 0; }

            SetMsg(File.ReadAllText(filename).Replace("\r\n"," ").Replace("\n"," "), UDF.COLOR.WORK);
            if (Regex.IsMatch(File.ReadAllText(filename),compareString))
            {
                SetMsg($"{test} 測試成功, PASS, Kiểm tra wifi thành công", UDF.COLOR.WORK);
            }
            else
            {
                SetMsg($"{test} 測試失敗, FAIL, Kiểm tra wifi thất bại", UDF.COLOR.FAIL);
                return false;
            }
            return true;
        }
        public static string GetMac(string sn, int useMode ,string tableName)//@@@
        {
            if (string.IsNullOrWhiteSpace(sn))
                throw new ArgumentException("SN 不可為空", nameof(sn));

            if (string.IsNullOrWhiteSpace(tableName))
                throw new ArgumentException("表格不可為空", nameof(sn));

            if (oraConn == null || oraConn.State != ConnectionState.Open)
                throw new InvalidOperationException("Oracle 連線尚未開啟");

            string sql = $@" SELECT KEYCODE
                            FROM (
                                SELECT KEYCODE
                                FROM {tableName}
                                WHERE SSN is NULL
                                  AND USE_MODE = :useMode
                                  AND USE_FLAG = 0
                                ORDER BY USE_DATE DESC, INSERT_DATE DESC
                            )
                            WHERE ROWNUM = 1";
            try
            {
                using (var cmd = new OracleCommand(sql, oraConn))
                {
                    cmd.BindByName = true; // 明確以參數名稱為準
                    cmd.Parameters.Add(":sn", OracleDbType.Varchar2).Value = sn;
                    cmd.Parameters.Add(":useMode", OracleDbType.Int32).Value = useMode;

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                            return reader["KEYCODE"]?.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                // 可視需要改為拋出或記錄錯誤
                Console.WriteLine("查詢 MAC 發生錯誤：" + ex.Message);
            }

            return null;
        }
        bool set_MAC_use_mode(string ssn, string mac , string useflag, string tableName)//@@@
        {
            if(oraConn == null || oraConn.State != ConnectionState.Open)
            {
                SetMsg("Oracle 連線尚未開啟", UDF.COLOR.FAIL);
                return false;
            }
            string sql = $@"UPDATE {tableName}
                            SET USE_FLAG = 1,
                                USE_DATE = :useDate,
                                SSN = :ssn
                            WHERE KEYCODE = :key";
            try
            {
                using (var cmd = new OracleCommand(sql, oraConn))
                {
                    cmd.BindByName = true; // 明確以參數名稱為準
                    cmd.Parameters.Add(":key", OracleDbType.Varchar2).Value = mac;
                    cmd.Parameters.Add(":useDate", OracleDbType.Date).Value = DateTime.Now;
                    cmd.Parameters.Add(":ssn", OracleDbType.Varchar2).Value = ssn;
                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        SetMsg($"MAC {mac} 已成功更新使用狀態為 {useflag}", UDF.COLOR.WORK);
                        return true;
                    }
                    else
                    {
                        SetMsg($"未找到 MAC {mac} 或已被使用", UDF.COLOR.FAIL);
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                SetMsg("更新 MAC 使用狀態時發生錯誤：" + ex.Message, UDF.COLOR.FAIL);
                return false;
            }
        }

        //=================================================================================
        public static string GetMac02(string sn, int useMode, string tableName)//@@@
        {
            // 參數驗證
            if (string.IsNullOrWhiteSpace(sn))
                throw new ArgumentException("SN 不可為空", nameof(sn));

            if (string.IsNullOrWhiteSpace(tableName))
                throw new ArgumentException("表格不可為空", nameof(tableName)); // 修正這裡

            if (oraConn == null || oraConn.State != ConnectionState.Open)
                throw new InvalidOperationException("Oracle 連線尚未開啟");

            string sql = $@" SELECT KEYCODE
                    FROM (
                        SELECT KEYCODE
                        FROM {tableName}
                        WHERE SSN is NULL
                          AND USE_MODE = :useMode
                          AND USE_FLAG = 0
                        ORDER BY USE_DATE DESC, INSERT_DATE DESC
                    )
                    WHERE ROWNUM = 1";

            try
            {
                using (var cmd = new OracleCommand(sql, oraConn))
                {
                    cmd.BindByName = true;
                    // 移除未使用的 :sn 參數
                    cmd.Parameters.Add(":useMode", OracleDbType.Int32).Value = useMode;

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                            return reader["KEYCODE"]?.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                // 可視需要改為拋出或記錄錯誤
                Console.WriteLine("查詢 MAC 發生錯誤：" + ex.Message);
            }

            return null;
        }


        /// <summary>
        /// 檢查指定SN的MAC地址是否存在並返回MAC地址
        /// </summary>
        /// <param name="sn">序列號</param>
        /// <param name="useMode">使用模式</param>
        /// <param name="tableName">表名</param>
        /// <param name="macAddress">返回的MAC地址</param>
        /// <returns>如果存在返回true，否則返回false</returns>
        public static bool IsMacExists(string sn, int useMode, string tableName, out string macAddress)
        {
            macAddress = null;

            try
            {
                // 參數驗證
                if (string.IsNullOrWhiteSpace(sn) || string.IsNullOrWhiteSpace(tableName))
                    return false;

                if (oraConn == null || oraConn.State != ConnectionState.Open)
                    return false;

                string sql = $@" SELECT KEYCODE
                        FROM {tableName}
                        WHERE SSN = :sn
                          AND USE_MODE = :useMode
                        ORDER BY USE_DATE DESC, INSERT_DATE DESC";

                using (var cmd = new OracleCommand(sql, oraConn))
                {
                    cmd.BindByName = true;
                    cmd.Parameters.Add(":sn", OracleDbType.Varchar2).Value = sn;
                    cmd.Parameters.Add(":useMode", OracleDbType.Int32).Value = useMode;

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            macAddress = reader["KEYCODE"]?.ToString();
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"檢查 MAC 存在性發生錯誤：{ex.Message}");
            }

            return false;

        }
        //===============================================================================
        public static string FormatMac(string mac)
        {
            if (string.IsNullOrWhiteSpace(mac) || mac.Length != 12)
                throw new ArgumentException("MAC 位址格式錯誤，必須是 12 個十六進位字元。");

            return string.Join(":", Enumerable.Range(0, 6)
                .Select(i => mac.Substring(i * 2, 2)));
        }
        public static string UnformatMac(string formattedMac)
        {
            if (string.IsNullOrWhiteSpace(formattedMac))
                throw new ArgumentException("輸入不得為空");

            // 移除所有非十六進位字元（:、- 等）
            string cleaned = new string(formattedMac
                .Where(c => Uri.IsHexDigit(c))
                .ToArray());

            if (cleaned.Length != 12)
                throw new ArgumentException("格式錯誤，還原後應為 12 碼");

            return cleaned.ToUpper();
        }
        string SafeTrim(string input, int maxBytes)
        {
            Encoding encoding = Encoding.UTF8;
            if (string.IsNullOrEmpty(input)) return "";

            while (encoding.GetByteCount(input) > maxBytes)
            {
                input = input.Substring(0, input.Length - 1);
            }
            return input;
        }
    }
}
