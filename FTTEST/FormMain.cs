using Oracle.ManagedDataAccess.Client;
using PRETEST.AppConfig;
using PRETEST.Database;
using PRETEST.SDriver;
using PRETEST.UDFs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using static PRETEST.UDFs.UDF;

namespace PRETEST 
{
    public partial class FormMain : Form
    {
        static OracleConnection oraConn = new OracleConnection();
        /*++++++++++++++++++++++++++++++++++++++++++++++++++++
        * 窗口自动调节相关
        * --------------------------------------------------*/
        public static int g_iFormWidth = new int();  //窗口宽度
        public static int g_iFormHeight = new int(); //窗口高度
        public static int g_iStatusStripHeight = new int(); //状态栏
        public static float g_fWidthScaling = 1.0f; //宽度缩放比例
        public static float g_fHeightScaling = 1.0f; //高度缩放比例
        public static UDF.ComplexConSetting g_complexConSetting = new UDF.ComplexConSetting();
        /*++++++++++++++++++++++++++++++++++++++++++++++++++++
        * 指令相关
        * --------------------------------------------------*/
        public static bool g_bRecvTvPortData = true;   //接收 Port 数据
        public static int g_iCmdDelay = new int();  //指令延时
        public static byte[] g_byCmdBuf = new byte[2048]; //命令缓冲区
        public static byte[] g_IOCmdBuf = new byte[2048]; //命令缓冲区
        public static byte[] g_byCmdRtn = new byte[4096]; //命令回传值
        public static byte[] g_byGdmRtn = new byte[512];   //GDM
        public static string g_sCmdRtnHexText = string.Empty;    //命令回传值16进制文本
        public static string g_sCmdRtnStrText = string.Empty;    //命令回传值-普通字符串文本
        public static string g_sCmdRtnCurStrText = string.Empty;   //命令回传－实时文本
        public static string g_sGdmRtnText = string.Empty; //GDM回传值
        public static bool g_bCmdWorkFlag = new bool(); //命令运行标志
        public static bool g_bWorkFlag = new bool(); //程序运行flag
        public static bool g_bRTACKFlag = new bool(); //程序运行flag
        public static bool g_bPortLogStart = new bool();   //端口log Start
        public static bool g_bPortLogStop = new bool();    //端口log Stop  
        public static bool g_bIO_Singal = new bool();    //到位信号的标志位
        public static bool g_bAutoScan = new bool();    //自动扫描的标志位
        public static bool g_bCheckusb_pass = new bool();    //三星机种USB的标志位
        public static bool g_bSj_check_ng = new bool();    // 视觉检测NG的标志位
        public static bool g_bSj_check_pass = new bool();    //视觉检测PASS的标志位

        public static string curBarcode_So = string.Empty;
        public static string curBarcode_PartNo = string.Empty;
        public static string curBarcode_Count = string.Empty;
        public static DateTime curBarcode_sTime;

        public static bool curBarcode_bRework = new bool();
        public static string curLine_MLine = string.Empty;
        public static string curLine_Usercode = string.Empty;

        public static bool timeGroup_DayWork = new bool();
        public static string timeGroup_BeginTime = string.Empty;  //BeginTime As String      '工作开始时间
        public static string timeGroup_OverTime = string.Empty; // OverTime As String     '工作结束时间
        public static string timeGroup_FreeTimeFirstStart = string.Empty;  
        public static string timeGroup_FreeTimeFirstEnd = string.Empty;

        public static string timeGroup_FreeTimeSecondStart = string.Empty;
        public static string timeGroup_FreeTimeSecondEnd = string.Empty;

        public static string timeGroup_EatTimeFirstStart = string.Empty;
        public static string timeGroup_EatTimeFirstEnd = string.Empty;

        public static string timeGroup_EatTimeSecondStart = string.Empty;
        public static string timeGroup_EatTimeSecondEnd = string.Empty;

        public static int timeGroup_FreeTimeFirstCount = new int();
        public static int timeGroup_FreeTimeSecondCount = new int();

        public static int timeGroup_EatTimeFirstCount = new int();
        public static int timeGroup_EatTimeSecondCount = new int();

        public static bool b_changeTime = new bool();
        public static string s_changeSo = string.Empty;
        public static int i_changeNum = new int(); 
        public static int Plan_Qty = new int();

        public static DateTime g_dOracle_time = g_dOracle_time.Date;
        public static DateTime g_dSql_time = g_dSql_time.Date;
        public static DateTime g_Computer_time;
        public static uint g_iRcvPortLogNum = new uint();  //端口接收数据次数


        public delegate void ShowTvPortMsgFunc(string msg);
        public static ShowTvPortMsgFunc ShowTvPortMsg;

        public static bool g_bXM_RZ2L = false;   //XM RX
        public static bool g_bXM_RZ3L = false;   // XM RX
        public static bool g_bXM_RZ4L = false;   //XM R
        public static bool g_bXM_RZ5L = false;   //XM R
        public static bool g_bXM_RZ6L = false;   //XM R
        public static bool g_bXM_RZ7L = false;   //XM
        public static bool g_bXM_RZ8L = false;   //XM RX
        public static bool g_bXM_RZ9L = false;   //XM RX
        public static bool g_bXM_RZAL = false;   //XM RX
        public static bool g_bXM_RZBL = false;   //XM RX
        public static bool g_bXM_RZCL = false;   //XM RX

        public static bool g_bXM_DZ3L = false;   //ES
        public static bool g_bXM_DZ4L = false;   //
        public static bool g_bXM_DZCL = false;   //
        public static bool g_bXM_DZDL = false;   //
        public static bool g_bXM_DZ9L = false;   //
        public static bool g_bXM_DZFL = false;   //
        public static bool g_bXM_DZGL = false;   //

        public static bool g_bXM_DZ5L = false;   //EA
        public static bool g_bXM_DZJL = false;   //
        public static bool g_bXM_DZ7L = false;   //
        public static bool g_bXM_DZ8L = false;   //
        public static bool g_bXM_DZEL = false;   //
        public static bool g_bXM_DZHL = false;   //
        public static bool g_bXM_DZIL = false;   //

        public static bool g_bXM_DZLL = false;   //A75 A75竞技版 DZKL
        public static bool g_bXM_DZKL = false;   //A75 A75 Pro DZNL
        public static bool g_bXM_DZNL = false;   //A75 HKC DZNL
        public static bool g_bXM_DZML = false;
        public static bool g_bXM_DZQL = false;

        public static bool g_bXM_WZ2L = false;   //EA
        public static bool g_bXM_WZ3L = false;   //
        public static bool g_bXM_WZ4L = false;   //
        public static bool g_bXM_WZ5L = false;   //
        public static bool g_bXM_WZ6L = false;   //
        public static bool g_bXM_WZ7L = false;   //


        /*++++++++++++++++++++++++++++++++++++++++++++++++++++
        * 其他
        * --------------------------------------------------*/
        public static bool g_bAdbProcessExited = new bool();  //ADB 进程退出标志
        static UDF.RunMode g_runMode = new UDF.RunMode();       //程序工作模式, 例如 RunMode.RST 或 RunMode.MHL
        public static bool g_bRecvFlag = false;     //服务器返回消息开关
        public static string g_sRecvMsg = "";    //服务器返回的消息
        public static string g_sFwVer1 = string.Empty;  //数据库中的 FW ver1
        public static string g_sFwVer2 = string.Empty;  //数据库中的 FW ver1
        public static string g_sTAG = string.Empty;  //数据库中的 FW ver1
        public static int g_iTimerCycle = new int();   //程序测试过程计时器
        public static bool g_bSubProFlag = new bool();  //子程序运行 flag
        public static bool g_bPortErr = new bool();    //端口错误
        public static bool g_bPrventPortErr = true;     //端口错误阻止标志,立即停止不再继续
        //public const string g_sVersion = "Ver: 2025-04-02.01";  //程序状态栏显示的版本
        public const string g_sVersion = "Ver: AVTC 2025-06-16.02";  //程序状态栏显示的版本
        public static string g_sAdcProgInfo = string.Empty;
        
        //NFA1.20200706.0027_163302100
        //NFA1.20210213.0033_255688573
        public string g_sArranFUDVer = "NFA1.20211012.0955_386672040";    //FW烧录工具VERSION
        public string g_sArranFUDVer2 = "NFA1.20211012.0955_386672040";    //FW烧录工具SECURE
        public string g_sArranFUDVer3 = "NFA1.20211012.0955_386672040";    //FW烧录工具NONSECURE
        //NFB1.20200706.0036_163305256
        //NFB1.20210213.0007_255688428
        public string g_sBarraFUDVer = "NFB1.20211012.1033_386672698";    //FW烧录工具VERSION
        public string FW_TEMP = "9351";
        //NFC1.20200909.0117_187561005
        //NFC1.20201119.0019_218193544
        //NFC1.20210213.0020_255688428
        public string g_sCardFUDVer = "NFC1.20210213.0020_255688428";    //FW烧录工具VERSION
        public string g_so = "";
        readonly static double[] g_dLightLuxSpec = new double[2] { 1d, 1000d };
        /*++++++++++++++++++++++++++++++++++++++++++++++++++++
        * 工单初始化相关
        * 
        * 通常情况下一个工单内的卡关的信息是一致的,
        * 所以应该只在工单切换初始时查询一次这些信息,而无需每次都进行查询
        * (例如,无需每次都要从数据库或其他网络上查询当前机种应该卡关的FW版本,测试规格,测试项目)
        * --------------------------------------------------*/
        static uint g_iPtInitFlag = new uint();    //工单初始化标志。工单切换后
        const uint INIT_ZERO = 0;   //工单初始化标志－重置
        const uint INIT_FW = 1; //FW 已下载
        const uint INIT_BTRSSI = 2; //BTRSSI 已下载
        const uint INIT_GETTESTITEM = 4;    //测试项 已下载
        const uint INIT_DATA_STYLE = 8;     //DataGridView 控件样式已重置

        #region 窗口调节相关
        /*++++++++++++++++++++++++++++++++++++++++++++++++++++
        * 此块用于窗口自动调节相关
        * --------------------------------------------------*/
        /// <summary>
        /// 记录控件集初始信息
        /// </summary>
        /// <param name="cons">控件集</param>
        private void InitConTag(Control cons)
        {
            foreach (Control con in cons.Controls) //遍历控件集
            {
                con.Tag = con.Left + "," + con.Top + "," + con.Width + "," + con.Height + "," + con.Font.Size + "," + con.Font.Style;
                if (con.Controls.Count > 0) //处理子控件
                {
                    InitConTag(con);
                }
            }
        }
        /// <summary>
        /// 重新调整控件集显示
        /// </summary>
        /// <param name="widthScaling">窗口宽度缩放比例</param>
        /// <param name="heightScaling">窗口高度缩放比例</param>
        /// <param name="cons">控件集</param>
        private void ResizeCon(float widthScaling, float heightScaling, Control cons)
        {
            float fTmp = new float();

            foreach (Control con in cons.Controls) //遍历控件集
            {
                string[] conTag = con.Tag.ToString().Split(new char[] { ',' });
                fTmp = Convert.ToSingle(conTag[0]) * widthScaling;
                con.Left = (int)fTmp;
                fTmp = Convert.ToSingle(conTag[1]) * heightScaling;
                con.Top = (int)fTmp;
                fTmp = Convert.ToSingle(conTag[2]) * widthScaling;
                con.Width = (int)fTmp;
                fTmp = Convert.ToSingle(conTag[3]) * heightScaling;
                con.Height = (int)fTmp;
                fTmp = Convert.ToSingle(conTag[4]) * heightScaling;
                con.Font = new System.Drawing.Font("", (fTmp == 0) ? 0.1f : fTmp, (FontStyle)Enum.Parse(typeof(FontStyle), conTag[5]));
                if (con.Controls.Count > 0) //处理子控件
                {
                    ResizeCon(widthScaling, heightScaling, con);
                }
            }
        }
        /// <summary>
        /// 记录复杂控件初始信息
        /// </summary>
        private void InitComplexCon()
        {
            g_complexConSetting.sStatusStripVersion = this.statusStripVersion.Height + "," + this.toolStripStatusLabel1.Width + "," + this.toolStripStatusLabel2.Width;
        }
        /// <summary>
        /// 重新调整复杂控件显示
        /// </summary>
        /// <param name="widthScaling">窗口宽度绽放比例</param>
        private void ResetComplexCon(float widthScaling, float heightScaling)
        {
            float fTmp = new float();
            string[] conTag;

            if ((g_iPtInitFlag & INIT_DATA_STYLE) == INIT_DATA_STYLE)
            {
                conTag = g_complexConSetting.sDataGradView.Split(new char[] { ',' });
                fTmp = Convert.ToSingle(conTag[0]) * widthScaling;
                this.dataGridViewTestData.RowHeadersWidth = (int)fTmp;
                for (int i = 0; i < this.dataGridViewTestData.ColumnCount; i++)
                {
                    fTmp = Convert.ToSingle(conTag[i + 1]) * widthScaling;
                    this.dataGridViewTestData.Columns[i].Width = (int)fTmp;
                }
            }

            conTag = g_complexConSetting.sStatusStripVersion.Split(new char[] { ',' });
            fTmp = Convert.ToSingle(conTag[0]) * heightScaling;
            this.statusStripVersion.Height = (int)fTmp;
            fTmp = Convert.ToSingle(conTag[1]) * widthScaling;
            this.toolStripStatusLabel1.Width = (int)fTmp;
            fTmp = Convert.ToSingle(conTag[2]) * widthScaling;
            this.toolStripStatusLabel2.Width = (int)fTmp;
        }
        #endregion
        public FormMain()
        {
            InitializeComponent();
        }
        private void FormMain_Load(object sender, EventArgs e)
        {
            //解决 adb.exe bug
            string nul;
            RunAdbCmd("taskkill /f /im adb.exe /t", out nul, 1);

            //CreateLogPath(DateTime.Now.ToString("yyyy-MM-dd"));

            //初始化界面一
            toolStripStatusLabel2.Text = g_sVersion;
            dataGridViewTestData.RowHeadersVisible = false;

            if (GlobalConfig.LoadIniSettings() == false)
            {
                SetMsg("配置档错误, Lỗi tập tin cấu hình", UDF.COLOR.FAIL,0);
                MessageBox.Show("配置档错误 \nLỗi tập tin cấu hình");
                Application.Exit();
            }

            //更新測站模式
            if (GlobalConfig.sMesEid.Substring(2, 3) == "PRE")
            {
                g_runMode = UDF.RunMode.PRE;
                
                toolStripStatusLabel1.Text = "Welcome to use Pre-Test ";
            }
            else if (GlobalConfig.sMesEid.Substring(2, 3) == "RST")
            {
                g_runMode = UDF.RunMode.RST;

                label9.Text = "RST TEST";
                toolStripStatusLabel1.Text = "Welcome to use ReSet test project";
            }
            else if (GlobalConfig.sMesEid.Substring(2, 3) == "TV1")
            {
                g_runMode = UDF.RunMode.TV1;
                label9.Text = "TV1 TEST";
                toolStripStatusLabel1.Text = "Welcome to use TV1 test project";
            }
            else
            {
                SetMsg("不支援的 MESEID！ Không chính xác MESEID!", UDF.COLOR.FAIL,0);
                MessageBox.Show("不支援的 MESEID！\nKhông chính xác MESEID!");
                Application.Exit();
            }

            if (GlobalConfig.iRunMode != 1)
            {
                DialogResult UserSelect = MessageBox.Show(null, "用户选择了测试模式，不会上传过站信息，确定继续？\nNgười dùng đã chọn chế độ kiểm tra, sẽ không tải lên thông tin trạm, bạn có chắc chắn muốn tiếp tục?\n\nRunMode:0\t测试模式，不上传过站信息\nRunMode:0\tChế độ kiểm tra, không tải lên thông tin trạm.\nRunMode:1\t正常测试，上传过站信息\nRunMode:1\tKiểm tra bình thường, tải lên thông tin trạm.",
                            "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                if (UserSelect == DialogResult.No)
                {
                    Application.Exit();
                }
            }

            if (SDriverX.AVTCInit() == false)
            {
                MessageBox.Show(null, "连接 AVTC webserver 失败", "AVTC webserver 連線失败", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                SetMsg("连接 AVTC webserver 失败", UDF.COLOR.FAIL,0);
                Application.Exit();
            }
            SetMsg("連結 AVTC webserver PASS,Kết nối AVTC webserver PASS", UDF.COLOR.WORK,0);
            string connString = "User Id=RKNMGR;Password=6v7tqe-v;Data Source=(DESCRIPTION = (ADDRESS = (PROTOCOL = TCP)(HOST = 10.2.235.119)(PORT = 1521))(CONNECT_DATA = (SERVER = DEDICATED)(SERVICE_NAME = AVTCMES)))";
            try
            {
                oraConn.ConnectionString = connString;
                oraConn.Open();
            }
            catch
            {
                MessageBox.Show("LINK ORACLE SERVER FAIL");
                SetMsg("LINK ORACLE SERVER FAIL", UDF.COLOR.FAIL,0);
                Application.Exit();
            }

            SetMsg("Connect ORACLE Server 10.2.235.119 PASS", UDF.COLOR.WORK,0);
            SetMsg("Kết nối ORACLE Server 10.2.235.119 PASS", UDF.COLOR.WORK,0);
            SetMsg("請輸入SN, Vui lòng nhập SN.", UDF.COLOR.WORK,0);
            SetMsg(g_sVersion, UDF.COLOR.WORK,0);

            #region 初始化窗口界面二
            // 顯示meseid, 顯示程式啟動時間, 顯示runmode
            textBoxMeseid.Text = GlobalConfig.sMesEid;
            this.toolStripStatusLabel3.Text = "系统开机时间：" + DateTime.Now.ToString("yyyy-MM-dd_hh:mm:ss");
            this.Text= "FTTEST | RunMode = " + GlobalConfig.iRunMode;
            #endregion
        }
        private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            string nul;
            oraConn.Close();
            Sql.Close();
            RunAdbCmd("taskkill /f /im adb.exe /t", out nul, 1);
            Process.GetCurrentProcess().Kill();
        }
        private void FormMain_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized) return;

            //调整窗口大小
            if (g_iFormWidth == 0 || g_iFormHeight == 0) return;
            g_fWidthScaling = (float)this.ClientSize.Width / (float)g_iFormWidth;
            g_fHeightScaling = ((float)(this.ClientSize.Height)) / ((float)(g_iFormHeight));
            if (g_fWidthScaling < 0.2f || g_fHeightScaling < 0.2f) return;
            ResizeCon(g_fWidthScaling, g_fHeightScaling, this);
            ResetComplexCon(g_fWidthScaling, g_fHeightScaling);

        }
        private void buttonExit_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void textBoxSn_KeyPress(object sender, KeyPressEventArgs e)
        {
            string sn = string.Empty;
            string rst = "FAIL";
            //if (Regex.IsMatch(textBoxSn.Text, @"\W+"))
            //{
            //    ResetUI();
            //    return;
            //}
            if (e.KeyChar == 13)    //ENTER鍵
            {
                InitUI();

                //建立logPath
                if (!CreateLogPath(DateTime.Now.ToString("yyyy-MM-dd")))
                {
                    ResetUI();
                    return;
                }

                // 紀錄測試開始時間
                DateTime dStartTime = DateTime.Now;
                SetMsg($"MESEID:{GlobalConfig.sMesEid}", UDF.COLOR.WORK);
                SetMsg($"RunMode:{GlobalConfig.iRunMode}", UDF.COLOR.WORK);
                SetMsg($"Model:{GlobalConfig.sModel}", UDF.COLOR.WORK);
                SetMsg($"SN:{textBoxSn.Text.Trim().ToUpper()}", UDF.COLOR.WORK);

                // 檢查SN格式 , 正確就關閉輸入框
                sn = textBoxSn.Text.Trim().ToUpper();
                if (sn.Length == 0) return;
                textBoxSn.Enabled = false;
                
                // 檢查目前SN站別
                if (PreCheckTRID(sn) == false)
                {
                    //重置焦点输入
                    if (g_bAutoScan == false)
                    {
                        this.textBoxSn.Enabled = true;
                        this.textBoxSn.Focus();
                        this.textBoxSn.SelectAll();
                    }
                    //FW_TEMP = "";
                    ResetUI();
                    return;
                }

                // 測試流程開始 先初始化dataGridview
                if ((g_iPtInitFlag & INIT_DATA_STYLE) != INIT_DATA_STYLE)
                {
                    //添加 dataGridView 控件初始化代码
                    InitGridFormatB();
                    g_iPtInitFlag |= INIT_DATA_STYLE;
                }

                // 檢查meseid 判斷站別
                if (g_runMode == UDF.RunMode.PRE)
                {
                    if (PRE_MainTestProcess()) rst = "PASS";
                }
                else if (g_runMode == UDF.RunMode.RST)
                {
                    if (RST_MainTestProcess()) rst = "PASS";
                }
                else if (g_runMode == UDF.RunMode.TV1)
                {
                    if (TV1_MainTestProcess()) rst = "PASS";
                }
                else
                {
                    SetMsg("未维护的运行模式", UDF.COLOR.FAIL);
                    ResetUI();
                    return;
                }

                // Runmode 1:上傳MES, 0:不上傳MES
                if (g_runMode >= UDF.RunMode.PRE && g_runMode <= UDF.RunMode.MIC)
                {
                    if (GlobalConfig.iRunMode == 1)
                    {
                        if (g_runMode != UDF.RunMode.NULL && rst == "PASS")
                        {
                            SetMsg("上傳結果到MES, Gửi dữ liệu đến cơ sở dữ liệu MES", UDF.COLOR.WORK);

                            if (SDriverX.INSERT_ROUTE_Webservice(GlobalConfig.sMesEid, SDriverX.g_modelInfo.sSn, "TestUser") == false)
                            {
                                SetMsg("上傳結果到MES失敗, FAIL, Không thể tải kết quả lên hệ thống MES" + SDriverX.g_modelInfo.sErr_msg, UDF.COLOR.FAIL);
                                ResetUI();
                                return;
                            }
                        }
                        else
                        {
                            SetMsg("測試FAIL 不上傳過站訊息, Kiểm tra thất bại, không gửi thông tin qua trạm.", UDF.COLOR.WORK);
                        }
                    }
                    else if (GlobalConfig.iRunMode == 0)
                    {
                        SetMsg("測試模式不會上傳過站訊息, Chế độ kiểm tra sẽ không tải thông tin qua trạm lên", UDF.COLOR.WORK);
                    }
                    else
                    {
                        SetMsg("錯誤的執行模式, 上傳過站訊息設置 RunMode:1\r\n否則請設置為0 RunMode:0, Chế độ chạy không hợp lệ. Nếu cần gửi thông tin qua trạm, hãy đặt RunMode:1\r\nNgược lại, hãy đặt RunMode:0" ,UDF.COLOR.FAIL);
                        ResetUI();
                        return;
                    }
                }

                // 更新測試時間到textboxCycle
                DateTime dEndTime = DateTime.Now;
                int sec = Convert.ToInt32(DiffSeconds(dStartTime, dEndTime));   //将时间差转换为秒
                textBoxCycle.Text = Convert.ToString(sec);

                // 更新測試結果到 dataGridview
                AddDataB(sn, sec);

                // 更新測試結果到狀態欄
                if (rst == "PASS") SetMsg("PASS！請輸入下一個SN, Vui lòng nhập SN tiếp theo", UDF.COLOR.PASS,0);
                else SetMsg("FAIL！請輸入下一個SN, Vui lòng nhập SN tiếp theo", UDF.COLOR.FAIL,0);
                
                // 儲存測試log到server和local
                Savelog("", rst);
                ResetUI();
            }
            return;
        }
        private void timerCycle_Tick(object sender, EventArgs e)
        {
            g_iTimerCycle++;
            textBoxCycle.Text = g_iTimerCycle.ToString();
        }
        private void listBoxSetup_DoubleClick(object sender, EventArgs e)
        {
            if (listBoxSetup.SelectedIndex < 0) return;
            MessageBox.Show(listBoxSetup.Items[listBoxSetup.SelectedIndex].ToString());
        }
        private void buttonSetting_Click(object sender, EventArgs e)
        {
            FormSetting FRM = new FormSetting();
            FRM.Show();
        }
        private bool CreateLogPath(string time)
        {
            try
            {
                string filename = $"{DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss")}_{textBoxSn.Text.Trim().ToUpper()}.txt";
                GlobalConfig.sServerLogPath = $@"\\{GlobalConfig.sLogServerIP}\logi\reggie\{GlobalConfig.sModel}\LOG\{GlobalConfig.sMesEid.Substring(2, 3)}\{time}\{filename}";
                if (Directory.Exists($@"\\{GlobalConfig.sLogServerIP}\logi\reggie\{GlobalConfig.sModel}\LOG\{GlobalConfig.sMesEid.Substring(2, 3)}\{time}\") == false)
                {
                    Directory.CreateDirectory($@"\\{GlobalConfig.sLogServerIP}\logi\reggie\{GlobalConfig.sModel}\LOG\{GlobalConfig.sMesEid.Substring(2, 3)}\{time}");
                }
                GlobalConfig.sLocalLogPath = $@"D:\{GlobalConfig.sModel}\LOG\{GlobalConfig.sMesEid.Substring(2, 3)}\{time}\{filename}";
                if (Directory.Exists($@"D:\{GlobalConfig.sModel}\LOG\{GlobalConfig.sMesEid.Substring(2, 3)}\{time}") == false)
                {
                    Directory.CreateDirectory($@"D:\{GlobalConfig.sModel}\LOG\{GlobalConfig.sMesEid.Substring(2, 3)}\{time}");
                }
            }
            catch (Exception ex)
            {
                SetMsg(ex.ToString(), UDF.COLOR.WORK);
                return false;
            }
            return true;
        }

        //private void serialPortIOCard_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        //{
        //    string str = string.Empty;
        //    g_sCmdRtnStrText = string.Empty;
        //    int i = 0;
        //    byte[] buf = new byte[256];
        //    try
        //    {
        //        Application.DoEvents();
        //        //Thread.Sleep(20);
        //        //int n = serialPortIOCard.BytesToRead;
        //        //serialPortIOCard.Read(buf, 0, n);                //1.缓存数据                           
        //        for (i = 0; i < 8; i++)  //for (i = 0; i < g_byCmdRtn[13]; i++)
        //        {
        //            str = Convert.ToString(buf[i], 16);// 数字转十六机制
        //            if (str.Length == 1)//如果我们获取的值是一位我们在前面补个0;
        //            {
        //                g_sCmdRtnStrText = g_sCmdRtnStrText + (" 0" + str);
        //            }
        //            else
        //            {
        //                g_sCmdRtnStrText = g_sCmdRtnStrText + (" " + str);//两个值就在前面加一个空格分隔 
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        //serialPort = new System.IO.Ports.SerialPort();
        //        MessageBox.Show(ex.Message);
        //        return;
        //    }
        //    if ((buf[0] != 204) || (buf[1] != 221) || (buf[2] != 1))
        //    {
        //        g_bSj_check_ng = false;
        //        g_bSj_check_pass = false;
        //        g_bCheckusb_pass = false;
        //        g_bIO_Singal = false;
        //        return;
        //    }
        //    if (buf[5] == 8)
        //    {
        //        g_bCheckusb_pass = true;  //三星机种USB的标志位  //IO CARD 到位第四个INPUT
        //    }
        //    else if (buf[5] == 4) //IO CARD 到位第三个INPUT
        //    {
        //        g_bSj_check_ng = true;
        //    }
        //    else if (buf[5] == 2 || buf[5] == 3)  //IO CARD 到位第二个INPUT
        //    {
        //        g_bSj_check_pass = true;
        //    }
        //    else if (buf[5] == 1) //IO CARD 到位第一个INPUT
        //    {
        //        g_bIO_Singal = true;
        //    }
        //    else
        //    {
        //        g_bSj_check_ng = false;
        //        g_bSj_check_pass = false;
        //        g_bCheckusb_pass = false;
        //        g_bIO_Singal = false;
        //    }
        //    //serialPortIOCard.DiscardInBuffer();//清除之前的缓存
        //}

        private void timerIO_CARD_Tick(object sender, EventArgs e)
        {
            //发指令实时侦测IO的INPUT信号

            this.textBoxAck.Text = this.textBoxAck.Text + "侦测IO信号cmd: AA BB 01 0D 01 74";
            Write_IOCARD_OUT("AA BB 01 0D 01 74", 6, 20, 6);
        }
        private void buttonSignal_Click(object sender, EventArgs e)
        {
            g_bIO_Singal = true;
        }
        private void buttonAutoScan_Click(object sender, EventArgs e)
        {
            g_bAutoScan = true;
            ClassFileIO.WriteIniFile("ComPort", "SICKEnable", "Y", ClassFileIO.sIniPath);
            timertest.Enabled = true;
            //InitUI();
            ResetUI();
        }
        private void buttonManScan_Click(object sender, EventArgs e)
        {
            g_bAutoScan = false;
            this.labelMsgtip.Text = "请输入标签序列号";
            this.labelMsgtip.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            ClassFileIO.WriteIniFile("ComPort", "SICKEnable", "N", ClassFileIO.sIniPath);
            timertest.Enabled = false;
            //InitUI();
            ResetUI();
        }

        //private void timertest_Tick(object sender, EventArgs e)
        //{
        //    //char[] keycode = new char[13];KeyPressEventArgs
        //    //char Key_Char = e.KeyChar;//判断按键的 Keychar  

        //    if ((g_bAutoScan == true))
        //    {
        //        textBoxSn.Enabled = false;
        //        this.labelMsgtip.Text = "Waiting for PLC signal";
        //        if (this.labelMsgtip.BackColor == System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128))))))
        //        {
        //            this.labelMsgtip.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
        //        }
        //        else if (this.labelMsgtip.BackColor == System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255))))))
        //        {
        //            this.labelMsgtip.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
        //        }
        //        else
        //        {
        //            this.labelMsgtip.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
        //        }
        //        timerIO_CARD.Enabled = true;
        //        if (g_bIO_Singal == true)
        //        {
        //            this.textBoxSn.Text = "";
        //            this.labelMsgtip.Text = "Automatic barcode scanning ...";
        //            timertest.Enabled = false;
        //            timerIO_CARD.Enabled = false;

        //            //if (Sick_scan() == false)
        //            //{
        //            //    timertest.Enabled = true;
        //            //    g_bIO_Singal = false;
        //            //    return;
        //            //}
        //            //else
        //            //{
        //            //    textBoxSn_KeyPress(null, new KeyPressEventArgs((char)13));
        //            //}
        //        }
        //    }
        //    else
        //    {
        //        textBoxSn.Enabled = true;
        //        timertest.Enabled = false;
        //    }
        //}
        //void Insert_Mes_qty(string str)
        //{
        //    string sql = string.Empty;
        //    OleDbDataReader reader;
        //    bool bErr = new bool();
        //    string m_Line = string.Empty;
        //    bool b_Rework = false;
        //    string buf = string.Empty;

        //    curLine_Usercode = "NA";
        //    curBarcode_So = SDriverX.g_modelInfo.sWord;
        //    curBarcode_PartNo = SDriverX.g_modelInfo.sPart_no;

        //    //if (isReworkSN(SDriverX.g_modelInfo.sSn) == true)
        //    //{
        //    //    b_Rework = true;
        //    //}
        //    //else
        //    //{
        //    //    b_Rework = false;
        //    //}

        //    //sql = string.Format("select AREANAME from machinespec where machinename ='" + GlobalConfig.sMesEid + "'");
        //    //bErr = Oracle.ServerExecute(sql, out reader);
        //    //if (bErr)
        //    //{
        //    //    reader.Read();
        //    //    if (reader.HasRows)
        //    //    {
        //    //        curLine_MLine = reader[0].ToString();
        //    //    }

        //    //}

        //    //sql = string.Format(" select productrequestname from producthistory where SET_SERIAL_NO='" + SDriverX.g_modelInfo.sSn + "' AND EVENTNAME ='MOVE'");
        //    //bErr = Oracle.ServerExecute(sql, out reader);
        //    //if (bErr)
        //    //{
        //    //    reader.Read();
        //    //    if (reader.HasRows)
        //    //    {
        //    //        buf = reader["productrequestname"].ToString();
        //    //        if (buf.Substring(0, 1).ToUpper() == "R")
        //    //        {
        //    //            b_Rework = true;
        //    //        }
        //    //    }

        //    //}
        //    if (b_Rework == false)
        //    {
        //        InsertQty_count();
        //        m_Line = curLine_MLine;
        //        Mpq(curBarcode_So, "FGD", curBarcode_PartNo, curBarcode_sTime, m_Line, curLine_Usercode);

        //        i_changeNum = i_changeNum + 1;
        //        if (s_changeSo != curBarcode_So || b_changeTime == true || i_changeNum == 10)
        //        {
        //            if (s_changeSo != curBarcode_So || b_changeTime == true)
        //            {
        //                i_changeNum = 1;
        //            }
        //            s_changeSo = curBarcode_So;

        //            Insert_Rate();
        //        }
        //    }
        //}
        void Insert_Rate()
        {
            string sql = string.Empty;
            SqlDataReader rdr = null;
            string Update_Item = string.Empty;
            string Update_Rate = string.Empty;
            string iPeriod = string.Empty;
            string sDateid = string.Empty;
            string sCurDateid = string.Empty;
            string sShift = string.Empty;           
            string diffqty = string.Empty;
            string sPart_no = string.Empty;
            string sStarttime = string.Empty;
            string Stand_Time = string.Empty;
            string iHour = string.Empty;
            string ipreHour = string.Empty;
            string sLine = string.Empty;
            string tDate = string.Empty;
            string sDate = string.Empty;
            DateTime sDatetime ;

            
            GetSqltime();
            sDatetime = g_dSql_time;
            getShiftPeriod(g_dSql_time, out sDate, out sShift);
            getQtyHour(curLine_MLine, curBarcode_sTime, out Update_Item, out Update_Rate, sShift, out iPeriod);

            //sDateid = Trim(curLine.MLine) & Format(getDayTimeStart(curBarcode.sTime, sShift), "yyyymmdd")
            //sCurDateid = Trim(curLine.MLine) & Format(curBarcode.sTime, "yyyymmdd")
            getDayTimeStart(g_dSql_time, sShift, out Stand_Time);
            sDateid = curLine_MLine + Convert.ToDateTime(Stand_Time).ToString("yyyyMMdd");
            sCurDateid = curLine_MLine + curBarcode_sTime.ToString("yyyyMMdd");
            iHour= sCurDateid + curBarcode_sTime.ToString("HH");
            if (curBarcode_sTime.ToString("HH")=="00")
            {
                ipreHour = curLine_MLine + curBarcode_sTime.AddHours(-1).ToString("yyyyMMdd") + curBarcode_sTime.AddHours(-1).ToString("HH");
            }
            else
            {
                ipreHour = sCurDateid + curBarcode_sTime.AddHours(-1).ToString("HH");
            }
            Sql.DefaultDatabase = Sql.DatabaseOpt.MES;
            if (Sql.ServerExecute("select DATE_ID from Qty_Rate with(nolock) where DATE_ID= '" + sDateid + "' and Mline= 'T300'", out rdr) ==true )
            {
                rdr.Read();
                if (rdr.HasRows == false)
                {
                    rdr.Close();
                    Sql.ServerExecute("insert into Qty_Rate(DATE_ID,MLINE) values ('" + sDateid + "','T300') ", out rdr);
                }
            }
            rdr.Close(); 
            RateShow(iHour, ipreHour);
            Sql.ServerExecute("UPDATE Qty_Rate SET " + Update_Rate + "=" + Plan_Qty + "   WHERE DATE_ID='" + sDateid + "' and Mline= 'T300'", out rdr);
            rdr.Close();
        }
        void RateShow(string sHour,string spreHour)
        {
            string sql = string.Empty;
            int sqlCount = 0;
            SqlDataReader rdr = null;

            string dsr = string.Empty;
            string[] startTime=new string [1000];
            DateTime [] startTimeRs = new DateTime[1000];
            string[] Part_No = new string[1000];
            string[] PartNoNum = new string[1000];
            long TimeSpace;
            int SpaceCount = 0;
            int j = 0;
            int k = 0,kk=0;
            int iShift = 0;
            int StdQty = 0;
            int iModelCount = 0;
            long ValidTime;
            Single Day_qty, Day_SumQty, temp_QTY;
            string temp_Station = string.Empty;
            string sLine = string.Empty;
            
            string timestr = string.Empty;
            string datestr = string.Empty;
            string CurTime = string.Empty;
            string dDiffTime = string.Empty;

            sLine = curLine_MLine;
            datestr= curBarcode_sTime.ToString("yyyy-MM-dd");
            CurTime = datestr+" "+ curBarcode_sTime.ToString("HH")+":59:59";
            j = 0;
            kk = 1;
            Day_qty =0;
            startTime[0] = CurTime;
            Sql.DefaultDatabase = Sql.DatabaseOpt.MES;
            if (Sql.ServerExecute("select * from mpq_new with(nolock) where (dayhour='" + sHour.Substring(2) + "' or dayhour='" + spreHour.Substring(2) + "') and line='" + sLine + "' and eline='" + curLine_Usercode + "' and station='T300' order by starttime Desc", out rdr) == true)
            {
                j = 1;
                while (rdr.Read())
                {
                    Part_No[j] = rdr["PartNo"].ToString().Trim();
                    PartNoNum[j] = rdr["quantity"].ToString().Trim();
                    startTimeRs[j] = Convert.ToDateTime(rdr["startTime"].ToString().Trim());
                    sqlCount = kk;
                    j += 1;
                    kk += 1;
                    if (j >= 1000)
                    {
                        break;
                    }
                }
                for (k = 1; k < kk; k++)
                {
                    if (sqlCount <= 1)
                    {
                        startTime[k] = curBarcode_sTime.ToString("yyyy-MM-dd") + " " + curBarcode_sTime.ToString("HH") + ":00:00";
                    }
                    else
                    {
                        if (sqlCount == k && Convert.ToDateTime(startTimeRs[k].ToString("yyyy-MM-dd HH:mm:ss")) > Convert.ToDateTime(curBarcode_sTime.ToString("yyyy-MM-dd") + " " + curBarcode_sTime.ToString("HH") + " :00:00"))
                        {
                            startTime[k] = curBarcode_sTime.ToString("yyyy-MM-dd") + " " + curBarcode_sTime.ToString("HH") + ":00:00";
                        }
                        else
                        {
                            startTime[k] = startTimeRs[k].ToString("yyyy-MM-dd HH:mm:ss");
                        }
                    }
                }
            }
            rdr.Close();
            SpaceCount = j - 1;
            for ( j = 1; j < SpaceCount+1; j++)
            {
                if (Convert.ToDateTime(startTime[j-1])<=Convert.ToDateTime(datestr + " " + curBarcode_sTime.ToString("HH") + ":00:00"))
                {
                    break;
                }
                temp_QTY = 0;
                if (j>1)
                {
                    //M32ARGGB2EQH.G32QCA1
                    if (Part_No[j].Substring(13,6)!= Part_No[j-1].Substring(13, 6))
                    {
                        if (Convert.ToInt32(PartNoNum[j])>=10 && Convert.ToInt32(PartNoNum[j-1]) >= 10)
                        {
                            GetModelCount(Part_No[j - 1], iShift,out iModelCount);
                            temp_QTY = iModelCount / (480 * 60) * 1800;
                        }
                    }
                }
                GetModelCount(Part_No[j], iShift, out iModelCount); //'获取各机种的标准产量
                StdQty = iModelCount;
                GetValidTime(startTime[j], startTime[j-1],out ValidTime);
                TimeSpace = ValidTime;

                //DbStdQty = StdQty;
                //Day_qty = Day_qty + (DbStdQty / (480 * 60) * TimeSpace) - mm.StrToInt(dDiffQTY);
                Day_SumQty = StdQty;
                Day_qty = Day_qty + (Day_SumQty / (480 * 60) * TimeSpace) - temp_QTY;
            }
            Plan_Qty =Convert.ToInt16 (Day_qty);
            //数组的清空，以备下一次 iShift 循环
            for (k=0;  k<j+1;  k++)
            {
                Part_No[k] = "";
                startTime[k] = "";
            }

        }
        void GetValidTime(string FirstTime,string LastTime,out long ValidTime)
        {
            long sumT = 0;
            string datestr = string.Empty;

            datestr = curBarcode_sTime.ToString("yyyy-MM-dd");
            if  (Convert.ToDateTime(FirstTime)< Convert.ToDateTime(datestr + " " + curBarcode_sTime.ToString("HH") + ":00:00"))
            {
                FirstTime = datestr + " " + curBarcode_sTime.ToString("HH") + ":00:00";
            }
            sumT = Convert.ToInt32(DiffSeconds(Convert.ToDateTime(FirstTime), Convert.ToDateTime(LastTime)));
            // 'Remove the First FreeTime.
            if (timeGroup_FreeTimeFirstStart != "" && timeGroup_FreeTimeFirstEnd == "")
            {
                if (Convert.ToDateTime(timeGroup_FreeTimeFirstStart) >= Convert.ToDateTime(FirstTime) && Convert.ToDateTime(timeGroup_FreeTimeFirstStart) < Convert.ToDateTime(LastTime))
                {
                    if (Convert.ToDateTime(timeGroup_FreeTimeFirstEnd) <= Convert.ToDateTime(LastTime))
                    {
                        sumT = sumT - timeGroup_FreeTimeFirstCount * 60;
                    }
                    else if (Convert.ToDateTime(timeGroup_FreeTimeFirstEnd) > Convert.ToDateTime(LastTime))
                    {
                        sumT = sumT - Convert.ToInt32(DiffSeconds(Convert.ToDateTime(timeGroup_FreeTimeFirstStart), Convert.ToDateTime(LastTime)));
                    }
                }
                else if (Convert.ToDateTime(timeGroup_FreeTimeFirstStart) < Convert.ToDateTime(FirstTime))
                {
                    if (Convert.ToDateTime(timeGroup_FreeTimeFirstEnd) > Convert.ToDateTime(FirstTime) && Convert.ToDateTime(timeGroup_FreeTimeFirstEnd) < Convert.ToDateTime(LastTime))
                    {
                        sumT = sumT - Convert.ToInt32(DiffSeconds(Convert.ToDateTime(FirstTime), Convert.ToDateTime(timeGroup_FreeTimeFirstEnd)));
                    }
                    else if (Convert.ToDateTime(timeGroup_FreeTimeFirstEnd) > Convert.ToDateTime(LastTime))
                    {
                        sumT = 0;
                    }
                }
            }
            //'Remove the second FreeTime.
            if (timeGroup_FreeTimeSecondStart != "" && timeGroup_FreeTimeSecondEnd != "")
            {
                if (Convert.ToDateTime(timeGroup_FreeTimeSecondStart) >= Convert.ToDateTime(FirstTime) && Convert.ToDateTime(timeGroup_FreeTimeSecondStart) < Convert.ToDateTime(LastTime))
                {
                    if (Convert.ToDateTime(timeGroup_FreeTimeSecondEnd) <= Convert.ToDateTime(LastTime))
                    {
                        sumT = sumT - timeGroup_FreeTimeSecondCount * 60;
                    }
                    else if (Convert.ToDateTime(timeGroup_FreeTimeSecondStart) > Convert.ToDateTime(LastTime))
                    {
                        sumT = sumT - Convert.ToInt32(DiffSeconds(Convert.ToDateTime(timeGroup_FreeTimeSecondStart), Convert.ToDateTime(LastTime)));
                    }
                }
                else if (Convert.ToDateTime(timeGroup_FreeTimeSecondStart) < Convert.ToDateTime(FirstTime))
                {
                    if (Convert.ToDateTime(timeGroup_FreeTimeSecondEnd) > Convert.ToDateTime(FirstTime) && Convert.ToDateTime(timeGroup_FreeTimeSecondEnd) < Convert.ToDateTime(LastTime))
                    {
                        sumT = sumT - Convert.ToInt32(DiffSeconds(Convert.ToDateTime(FirstTime), Convert.ToDateTime(timeGroup_FreeTimeSecondEnd)));
                    }
                    else if (Convert.ToDateTime(timeGroup_FreeTimeSecondEnd) > Convert.ToDateTime(LastTime))
                    {
                        sumT = 0;
                    }
                }
            }
            //'Remove First EatTime.
            if (timeGroup_EatTimeFirstStart != "" && timeGroup_EatTimeFirstEnd != "")
            { 
                if (Convert.ToDateTime(timeGroup_EatTimeFirstStart) >= Convert.ToDateTime(FirstTime) && Convert.ToDateTime(timeGroup_EatTimeFirstStart) < Convert.ToDateTime(LastTime))
                {
                    if (Convert.ToDateTime(timeGroup_EatTimeFirstEnd) <= Convert.ToDateTime(LastTime))
                    {
                        sumT = sumT - timeGroup_EatTimeFirstCount * 60;
                    }
                    else if (Convert.ToDateTime(timeGroup_EatTimeFirstEnd) > Convert.ToDateTime(LastTime))
                    {
                        sumT = sumT - Convert.ToInt32(DiffSeconds(Convert.ToDateTime(timeGroup_EatTimeFirstStart), Convert.ToDateTime(LastTime)));
                    }
                }
                else if (Convert.ToDateTime(timeGroup_EatTimeFirstStart) < Convert.ToDateTime(FirstTime))
                {
                    if (Convert.ToDateTime(timeGroup_EatTimeFirstEnd) > Convert.ToDateTime(FirstTime) && Convert.ToDateTime(timeGroup_EatTimeFirstEnd) < Convert.ToDateTime(LastTime))
                    {
                        sumT = sumT - Convert.ToInt32(DiffSeconds(Convert.ToDateTime(FirstTime), Convert.ToDateTime(timeGroup_EatTimeFirstEnd)));
                    }
                    else if (Convert.ToDateTime(timeGroup_EatTimeFirstEnd) > Convert.ToDateTime(LastTime))
                    {
                        sumT = 0;
                    }
                }
             }
            //'Remove Second EatTime
            if (timeGroup_EatTimeSecondStart != "" && timeGroup_EatTimeSecondEnd != "")
            {
                if (Convert.ToDateTime(timeGroup_EatTimeSecondStart) >= Convert.ToDateTime(FirstTime) && Convert.ToDateTime(timeGroup_EatTimeSecondStart) < Convert.ToDateTime(LastTime))
                {
                    if (Convert.ToDateTime(timeGroup_EatTimeSecondEnd) <= Convert.ToDateTime(LastTime))
                    {
                        sumT = sumT - timeGroup_EatTimeSecondCount * 60;
                    }
                    else if (Convert.ToDateTime(timeGroup_EatTimeSecondEnd) > Convert.ToDateTime(LastTime))
                    {
                        sumT = sumT - Convert.ToInt32(DiffSeconds(Convert.ToDateTime(timeGroup_EatTimeSecondStart), Convert.ToDateTime(LastTime)));
                    }
                }
                else if (Convert.ToDateTime(timeGroup_EatTimeSecondStart) < Convert.ToDateTime(FirstTime))
                {
                    if (Convert.ToDateTime(timeGroup_EatTimeSecondEnd) > Convert.ToDateTime(FirstTime) && Convert.ToDateTime(timeGroup_EatTimeSecondEnd) < Convert.ToDateTime(LastTime))
                    {
                        sumT = sumT - Convert.ToInt32(DiffSeconds(Convert.ToDateTime(FirstTime), Convert.ToDateTime(timeGroup_EatTimeSecondEnd)));
                    }
                    else if (Convert.ToDateTime(timeGroup_EatTimeSecondEnd) > Convert.ToDateTime(LastTime))
                    {
                        sumT = 0;
                    }
                }
            }
            ValidTime = sumT;
        }
        public double DiffSeconds(DateTime startTime, DateTime endTime)
        {
            TimeSpan secondSpan = new TimeSpan(endTime.Ticks - startTime.Ticks);
            return secondSpan.TotalSeconds;
        }
        public double DiffMinutes(DateTime startTime, DateTime endTime)
        {
            TimeSpan minuteSpan = new TimeSpan(endTime.Ticks - startTime.Ticks);
            return minuteSpan.TotalMinutes;
        }
        public double DiffHours(DateTime startTime, DateTime endTime)
        {
            TimeSpan HourSpan = new TimeSpan(endTime.Ticks - startTime.Ticks);
            return HourSpan.TotalHours;
        }
        void GetModelCount(string Part_No,int iShift,out int ModelCount)
        {
            string sql = string.Empty;
            SqlDataReader rdr = null;
            int dsr = 0;
            string sLine = string.Empty;

            sLine = curLine_MLine;

            Sql.DefaultDatabase = Sql.DatabaseOpt.MES;
            if (Sql.ServerExecute("select * from UPH with(nolock) where partno='" + Part_No +"' and linetype='" + sLine + "'", out rdr) ==true)
            {
                rdr.Read();
                if (rdr.HasRows == false)
                {
                    rdr.Close();
                    if (Sql.ServerExecute("select * from UPH with(nolock) where partno='" + Part_No + "' and linetype='Normal'", out rdr) == true)
                    {
                        rdr.Read();
                        if (rdr.HasRows == false)
                        {
                            MessageBox.Show(null, "该线别未设定此机种的UPH,请确认线别是否设定正确？", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            dsr = Convert.ToInt16(rdr[2].ToString());
                        }
                    }
                }
                else
                {
                    dsr = Convert.ToInt16(rdr[2].ToString());
                }
                rdr.Close();
            }
            ModelCount = dsr;
        }
        void InitT()
        {
            string timestr = string.Empty;
            string datestr = string.Empty;
            string CurTime = string.Empty;
            string sLine = string.Empty;

            string sql = string.Empty;
            SqlDataReader rdr = null;

            timeGroup_DayWork = false;
            timestr = curBarcode_sTime.ToString("HHmmss");
            datestr = curBarcode_sTime.ToString("yyyy-MM-dd");
            CurTime = curBarcode_sTime.ToString("HH:mm:ss");

            timeGroup_FreeTimeFirstCount = 0;
            timeGroup_FreeTimeSecondCount = 0;
            timeGroup_EatTimeFirstCount = 0;
            timeGroup_EatTimeSecondCount = 0;

            timeGroup_EatTimeFirstStart = "";
            timeGroup_EatTimeFirstEnd = "";
            timeGroup_EatTimeSecondStart = "";
            timeGroup_EatTimeSecondEnd = "";

            sLine = curLine_MLine;
            Sql.DefaultDatabase = Sql.DatabaseOpt.MES;
            if (Convert.ToInt32(timestr) >= 80000 && Convert.ToInt32(timestr) < 200000 )        //'8.00 Am - 8.00 Pm
            {
                if (Sql.ServerExecute("select * from LineRestTime with(nolock) where line = '" + sLine + "' and starttime < '200000' and starttime > '080000' order by period", out rdr) == true)
                {
                    while (rdr.Read())
                    {
                        //m = reader["FW_DATA"].ToString();
                        switch (rdr["period"].ToString())
                        {
                            case "3":  //10:00休息
                                timeGroup_FreeTimeFirstStart = datestr +" " + rdr[2].ToString().Trim().Substring(0, 2) + ":" + rdr[2].ToString().Trim().Substring(2, 2) + ":" + rdr[2].ToString().Trim().Substring(rdr[2].ToString().Length - 2, 2);
                                timeGroup_FreeTimeFirstEnd = datestr + " " + rdr[3].ToString().Trim().Substring(0, 2) + ":" + rdr[3].ToString().Trim().Substring(2, 2) + ":" + rdr[3].ToString().Trim().Substring(rdr[3].ToString().Length - 2, 2);
                                timeGroup_FreeTimeFirstCount = Convert.ToInt16(rdr[4].ToString().Trim());
                                break;
                            case "4":  //11:00 第一次吃饭开始
                                timeGroup_EatTimeFirstStart = datestr + " " + rdr[2].ToString().Trim().Substring(0, 2) + ":" + rdr[2].ToString().Trim().Substring(2, 2) + ":" + rdr[2].ToString().Trim().Substring(rdr[2].ToString().Length - 2, 2);
                                timeGroup_EatTimeFirstEnd = datestr + " " + rdr[3].ToString().Trim().Substring(0, 2) + ":" + rdr[3].ToString().Trim().Substring(2, 2) + ":" + rdr[3].ToString().Trim().Substring(rdr[3].ToString().Length - 2, 2);
                                timeGroup_EatTimeFirstCount = timeGroup_EatTimeFirstCount + Convert.ToInt16(rdr[4].ToString().Trim());
                                break;
                            case "5":
                                if (timeGroup_EatTimeFirstStart == "")
                                {
                                    timeGroup_EatTimeFirstStart = datestr + " " + rdr[2].ToString().Trim().Substring(0, 2) + ":" + rdr[2].ToString().Trim().Substring(2, 2) + ":" + rdr[2].ToString().Trim().Substring(rdr[2].ToString().Length - 2, 2);
                                }
                                timeGroup_EatTimeFirstEnd = datestr + " " + rdr[3].ToString().Trim().Substring(0, 2) + ":" + rdr[3].ToString().Trim().Substring(2, 2) + ":" + rdr[3].ToString().Trim().Substring(rdr[3].ToString().Length - 2, 2);
                                timeGroup_EatTimeFirstCount = timeGroup_FreeTimeFirstCount + Convert.ToInt16(rdr[4].ToString().Trim());
                                break;
                            case "8": // 15:00 下午休息
                                timeGroup_FreeTimeSecondStart = datestr + " " + rdr["startTime"].ToString().Trim().Substring(0, 2) + ":" + rdr["startTime"].ToString().Trim().Substring(2, 2) + ":" + rdr["startTime"].ToString().Trim().Substring(rdr["startTime"].ToString().Length - 2, 2);
                                timeGroup_FreeTimeSecondEnd = datestr + " " + rdr["endTime"].ToString().Trim().Substring(0, 2) + ":" + rdr["endTime"].ToString().Trim().Substring(2, 2) + ":" + rdr["endTime"].ToString().Trim().Substring(rdr["endTime"].ToString().Length - 2, 2);
                                timeGroup_FreeTimeSecondCount =Convert.ToInt16(rdr[4].ToString().Trim());
                                break;
                            case "9": //16:00 晚饭开始
                                timeGroup_EatTimeSecondStart = datestr + " " + rdr["startTime"].ToString().Trim().Substring(0, 2) + ":" + rdr["startTime"].ToString().Trim().Substring(2, 2) + ":" + rdr["startTime"].ToString().Trim().Substring(rdr["startTime"].ToString().Length - 2, 2);
                                timeGroup_EatTimeSecondEnd = datestr + " " + rdr["endTime"].ToString().Trim().Substring(0, 2) + ":" + rdr["endTime"].ToString().Trim().Substring(2, 2) + ":" + rdr["endTime"].ToString().Trim().Substring(rdr["endTime"].ToString().Length - 2, 2);
                                timeGroup_EatTimeSecondCount = timeGroup_EatTimeSecondCount + Convert.ToInt16(rdr["Length"].ToString().Trim());
                                break;
                            case "10":
                                if (timeGroup_EatTimeSecondStart == "")
                                {
                                    timeGroup_EatTimeSecondStart = datestr + " " + rdr["startTime"].ToString().Trim().Substring(0, 2) + ":" + rdr["startTime"].ToString().Trim().Substring(2, 2) + ":" + rdr["startTime"].ToString().Trim().Substring(rdr["startTime"].ToString().Length - 2, 2);
                                }
                                timeGroup_EatTimeSecondEnd= datestr + " " + rdr["endTime"].ToString().Trim().Substring(0, 2) + ":" + rdr["endTime"].ToString().Trim().Substring(2, 2) + ":" + rdr[3].ToString().Trim().Substring(rdr["endTime"].ToString().Length - 2, 2);
                                timeGroup_EatTimeSecondCount = timeGroup_EatTimeSecondCount + Convert.ToInt16(rdr["Length"].ToString().Trim());
                                break;
                            case "11":
                                //timeGroup_FreeTimeFirstStart = datestr + rdr[2].ToString().Trim().Substring(0, 2) + ":" + rdr[2].ToString().Trim().Substring(2, 2) + ":" + rdr[2].ToString().Trim().Substring(rdr[2].ToString().Length - 2, 2);
                                timeGroup_EatTimeSecondEnd = datestr + " " + rdr["endTime"].ToString().Trim().Substring(0, 2) + ":" + rdr["endTime"].ToString().Trim().Substring(2, 2) + ":" + rdr["endTime"].ToString().Trim().Substring(rdr["endTime"].ToString().Length - 2, 2);
                                timeGroup_EatTimeSecondCount = timeGroup_EatTimeSecondCount + Convert.ToInt16(rdr["Length"].ToString().Trim());
                                break;
                            default:
                                break;
                        }
                        //rdr.NextResult();
                    }
                }
            }
            else if (Convert.ToInt32(timestr) >= 200000 && Convert.ToInt32(timestr) < 235959)        //'8.00 Pm -11.59.59
            {
                if (Sql.ServerExecute("select * from LineRestTime with(nolock) where line = '" + sLine + "' and (starttime > '200000' or starttime < '080000') order by period", out rdr) == true)
                {
                    while (rdr.Read())
                    {
                        //m = reader["FW_DATA"].ToString();
                        switch (rdr["period"].ToString())
                        {
                            case "15": //22:00 夜班第一次休息
                                timeGroup_FreeTimeFirstStart = datestr + " " + rdr["startTime"].ToString().Trim().Substring(0, 2) + ":" + rdr["startTime"].ToString().Trim().Substring(2, 2) + ":" + rdr["startTime"].ToString().Trim().Substring(rdr["startTime"].ToString().Length - 2, 2);
                                timeGroup_FreeTimeFirstEnd = datestr + " " + rdr["endTime"].ToString().Trim().Substring(0, 2) + ":" + rdr["endTime"].ToString().Trim().Substring(2, 2) + ":" + rdr["endTime"].ToString().Trim().Substring(rdr["endTime"].ToString().Length - 2, 2);
                                timeGroup_FreeTimeFirstCount = Convert.ToInt16(rdr["Length"].ToString().Trim());
                                break;
                            case "16": //23:00 夜班第一次吃饭 

                                timeGroup_EatTimeFirstStart = datestr + " " + rdr[2].ToString().Trim().Substring(0, 2) + ":" + rdr[2].ToString().Trim().Substring(2, 2) + ":" + rdr[2].ToString().Trim().Substring(rdr[2].ToString().Length - 2, 2);
                                if (Convert.ToInt32(rdr["endTime"].ToString().Trim())- Convert.ToInt32(rdr["startTime"].ToString().Trim())>0)
                                {
                                    timeGroup_EatTimeFirstEnd = datestr  +" " + rdr[3].ToString().Trim().Substring(0, 2) + ":" + rdr[3].ToString().Trim().Substring(2, 2) + ":" + rdr[3].ToString().Trim().Substring(rdr[3].ToString().Length - 2, 2);
                                }
                                else
                                {
                                    timeGroup_EatTimeFirstEnd = curBarcode_sTime.AddDays(1).ToString("yyyy-MM-dd") +" "+ rdr[3].ToString().Trim().Substring(0, 2) + ":" + rdr[3].ToString().Trim().Substring(2, 2) + ":" + rdr[3].ToString().Trim().Substring(rdr[3].ToString().Length - 2, 2);
                                }
                                timeGroup_EatTimeFirstCount = timeGroup_EatTimeFirstCount + Convert.ToInt16(rdr["Length"].ToString().Trim());
                                break;
                            case "17": //00:00 夜班第一次吃饭
                                if (timeGroup_EatTimeFirstStart=="")
                                {
                                    timeGroup_EatTimeFirstStart = curBarcode_sTime.AddDays(1).ToString("yyyy-MM-dd") + " " + rdr[3].ToString().Trim().Substring(0, 2) + ":" + rdr[3].ToString().Trim().Substring(2, 2) + ":" + rdr[3].ToString().Trim().Substring(rdr[3].ToString().Length - 2, 2);
                                }
                                timeGroup_EatTimeFirstEnd = curBarcode_sTime.AddDays(1).ToString("yyyy-MM-dd") +" "+ rdr[3].ToString().Trim().Substring(0, 2) + ":" + rdr[3].ToString().Trim().Substring(2, 2) + ":" + rdr[3].ToString().Trim().Substring(rdr[3].ToString().Length - 2, 2);
                                timeGroup_EatTimeFirstCount = timeGroup_EatTimeFirstCount + Convert.ToInt16(rdr[4].ToString().Trim());
                                break;
                            case "20":  //3:00夜班第二次休息
                                timeGroup_FreeTimeSecondStart = curBarcode_sTime.AddDays(1).ToString("yyyy-MM-dd") +" " + rdr[2].ToString().Trim().Substring(0, 2) + ":" + rdr[2].ToString().Trim().Substring(2, 2) + ":" + rdr[2].ToString().Trim().Substring(rdr[2].ToString().Length - 2, 2);
                                timeGroup_FreeTimeSecondEnd = curBarcode_sTime.AddDays(1).ToString("yyyy-MM-dd") + " " + rdr["endTime"].ToString().Trim().Substring(0, 2) + ":" + rdr["endTime"].ToString().Trim().Substring(2, 2) + ":" + rdr["endTime"].ToString().Trim().Substring(rdr["endTime"].ToString().Length - 2, 2);
                                timeGroup_FreeTimeSecondCount =  Convert.ToInt16(rdr["Length"].ToString().Trim());
                                break;
                            case "21":  //4:00夜班早饭 
                                timeGroup_EatTimeSecondStart = curBarcode_sTime.AddDays(1).ToString("yyyy-MM-dd") + " " + rdr[2].ToString().Trim().Substring(0, 2) + ":" + rdr[2].ToString().Trim().Substring(2, 2) + ":" + rdr[2].ToString().Trim().Substring(rdr[2].ToString().Length - 2, 2);
                                timeGroup_EatTimeSecondEnd = curBarcode_sTime.AddDays(1).ToString("yyyy-MM-dd") + " " + rdr["endTime"].ToString().Trim().Substring(0, 2) + ":" + rdr["endTime"].ToString().Trim().Substring(2, 2) + ":" + rdr["endTime"].ToString().Trim().Substring(rdr["endTime"].ToString().Length - 2, 2);
                                timeGroup_EatTimeSecondCount = timeGroup_EatTimeSecondCount + Convert.ToInt16(rdr["Length"].ToString().Trim());
                                break;
                            case "22": // 5:00夜班早饭
                                if (timeGroup_EatTimeSecondStart=="")
                                {
                                    timeGroup_EatTimeSecondStart = curBarcode_sTime.AddDays(1).ToString("yyyy-MM-dd") + " " + rdr[2].ToString().Trim().Substring(0, 2) + ":" + rdr[2].ToString().Trim().Substring(2, 2) + ":" + rdr[2].ToString().Trim().Substring(rdr[2].ToString().Length - 2, 2);
                                }

                                timeGroup_EatTimeSecondEnd = curBarcode_sTime.AddDays(1).ToString("yyyy-MM-dd") + " " + rdr[3].ToString().Trim().Substring(0, 2) + ":" + rdr[3].ToString().Trim().Substring(2, 2) + ":" + rdr[3].ToString().Trim().Substring(rdr[3].ToString().Length - 2, 2);
                                timeGroup_EatTimeSecondCount = timeGroup_EatTimeSecondCount + Convert.ToInt16(rdr[4].ToString().Trim());
                                break;
                            case "23": //6:00夜班早饭
                                //timeGroup_FreeTimeFirstStart = datestr + rdr[2].ToString().Trim().Substring(0, 2) + ":" + rdr[2].ToString().Trim().Substring(2, 2) + ":" + rdr[2].ToString().Trim().Substring(rdr[2].ToString().Length - 2, 2);
                                timeGroup_EatTimeSecondEnd = curBarcode_sTime.AddDays(1).ToString("yyyy-MM-dd") + " " + rdr[3].ToString().Trim().Substring(0, 2) + ":" + rdr[3].ToString().Trim().Substring(2, 2) + ":" + rdr[3].ToString().Trim().Substring(rdr[3].ToString().Length - 2, 2);
                                timeGroup_EatTimeSecondCount = timeGroup_EatTimeSecondCount + Convert.ToInt16(rdr[4].ToString().Trim());
                                break;
                            default:
                                break;
                        }
                        //rdr.NextResult(); 
                    }
                }
            }
            else if (Convert.ToInt32(timestr) >= 0 && Convert.ToInt32(timestr) < 80000)        //'00.00 Am - 8.00 Am
            {
                if (Sql.ServerExecute("select * from LineRestTime with(nolock) where line = '" + sLine + "' and (starttime > '200000' or starttime < '080000') order by period", out rdr) == true)
                {
                    while (rdr.Read())
                    {
                        //m = reader["FW_DATA"].ToString();
                        switch (rdr["period"].ToString())
                        {
                            case "15":
                                timeGroup_FreeTimeFirstStart = curBarcode_sTime.AddDays(-1).ToString("yyyy-MM-dd")+" " + rdr["startTime"].ToString().Trim().Substring(0, 2) + ":" + rdr["startTime"].ToString().Trim().Substring(2, 2) + ":" + rdr["startTime"].ToString().Trim().Substring(rdr["startTime"].ToString().Length - 2, 2);
                                timeGroup_FreeTimeFirstEnd = curBarcode_sTime.AddDays(-1).ToString("yyyy-MM-dd") +" "+ rdr["endTime"].ToString().Trim().Substring(0, 2) + ":" + rdr["endTime"].ToString().Trim().Substring(2, 2) + ":" + rdr["endTime"].ToString().Trim().Substring(rdr["endTime"].ToString().Length - 2, 2);
                                timeGroup_FreeTimeFirstCount = Convert.ToInt16(rdr["Length"].ToString().Trim());
                                break;
                            case "16":
                                timeGroup_EatTimeFirstStart = curBarcode_sTime.AddDays(-1).ToString("yyyy-MM-dd") + " " + rdr[2].ToString().Trim().Substring(0, 2) + ":" + rdr["startTime"].ToString().Trim().Substring(2, 2) + ":" + rdr["startTime"].ToString().Trim().Substring(rdr["startTime"].ToString().Length - 2, 2);
                                if (Convert.ToInt32(rdr["endTime"].ToString().Trim()) - Convert.ToInt32(rdr["startTime"].ToString().Trim()) > 0)
                                {
                                    timeGroup_EatTimeFirstEnd = curBarcode_sTime.AddDays(-1).ToString("yyyy-MM-dd") + " " + rdr["endTime"].ToString().Trim().Substring(0, 2) + ":" + rdr["endTime"].ToString().Trim().Substring(2, 2) + ":" + rdr["endTime"].ToString().Trim().Substring(rdr["endTime"].ToString().Length - 2, 2);                                    
                                }
                                else
                                {
                                    timeGroup_EatTimeFirstEnd = datestr + " " + rdr["endTime"].ToString().Trim().Substring(0, 2) + ":" + rdr["endTime"].ToString().Trim().Substring(2, 2) + ":" + rdr["endTime"].ToString().Trim().Substring(rdr["endTime"].ToString().Length - 2, 2);
                                }
                                timeGroup_EatTimeFirstCount = timeGroup_EatTimeFirstCount + Convert.ToInt16(rdr[4].ToString().Trim());
                                break;
                            case "17":  // 00:00 夜班第一次吃饭 
                                if (timeGroup_EatTimeFirstStart=="")
                                {
                                    timeGroup_EatTimeFirstStart = datestr + " " + rdr["startTime"].ToString().Trim().Substring(0, 2) + ":" + rdr["startTime"].ToString().Trim().Substring(2, 2) + ":" + rdr["startTime"].ToString().Trim().Substring(rdr["startTime"].ToString().Length - 2, 2);
                                }
                                timeGroup_EatTimeFirstEnd = datestr + " " + rdr["endTime"].ToString().Trim().Substring(0, 2) + ":" + rdr["endTime"].ToString().Trim().Substring(2, 2) + ":" + rdr["endTime"].ToString().Trim().Substring(rdr["endTime"].ToString().Length - 2, 2);
                                timeGroup_EatTimeFirstCount = timeGroup_EatTimeFirstCount + Convert.ToInt16(rdr[4].ToString().Trim());
                                break;
                            case "20": // 3:00夜班第二次休息
                                timeGroup_FreeTimeSecondStart = datestr + " " + rdr["startTime"].ToString().Trim().Substring(0, 2) + ":" + rdr["startTime"].ToString().Trim().Substring(2, 2) + ":" + rdr["startTime"].ToString().Trim().Substring(rdr[2].ToString().Length - 2, 2);
                                timeGroup_FreeTimeSecondEnd = datestr + " " + rdr["endTime"].ToString().Trim().Substring(0, 2) + ":" + rdr["endTime"].ToString().Trim().Substring(2, 2) + ":" + rdr["endTime"].ToString().Trim().Substring(rdr["endTime"].ToString().Length - 2, 2);
                                timeGroup_FreeTimeSecondCount = Convert.ToInt16(rdr[4].ToString().Trim());
                                break;
                            case "21": // 4:00夜班早饭
                                timeGroup_EatTimeSecondStart = datestr + " " + rdr["startTime"].ToString().Trim().Substring(0, 2) + ":" + rdr["startTime"].ToString().Trim().Substring(2, 2) + ":" + rdr["startTime"].ToString().Trim().Substring(rdr["startTime"].ToString().Length - 2, 2);
                                timeGroup_EatTimeSecondEnd = datestr + " " + rdr["endTime"].ToString().Trim().Substring(0, 2) + ":" + rdr["endTime"].ToString().Trim().Substring(2, 2) + ":" + rdr["endTime"].ToString().Trim().Substring(rdr["endTime"].ToString().Length - 2, 2);
                                timeGroup_EatTimeSecondCount = timeGroup_EatTimeSecondCount + Convert.ToInt16(rdr[4].ToString().Trim());
                                break;
                            case "22": // 5:00夜班早饭
                                if (timeGroup_EatTimeSecondStart == "")
                                {
                                    timeGroup_EatTimeSecondStart = datestr + " " + rdr["startTime"].ToString().Trim().Substring(0, 2) + ":" + rdr["startTime"].ToString().Trim().Substring(2, 2) + ":" + rdr["startTime"].ToString().Trim().Substring(rdr["startTime"].ToString().Length - 2, 2);
                                }
                                timeGroup_EatTimeSecondEnd = datestr + " " + rdr[3].ToString().Trim().Substring(0, 2) + ":" + rdr[3].ToString().Trim().Substring(2, 2) + ":" + rdr[3].ToString().Trim().Substring(rdr[3].ToString().Length - 2, 2);
                                timeGroup_EatTimeSecondCount = timeGroup_EatTimeSecondCount + Convert.ToInt16(rdr[4].ToString().Trim());
                                break;
                            case "23": //6:00夜班早饭
                                //timeGroup_FreeTimeFirstStart = datestr + rdr[2].ToString().Trim().Substring(0, 2) + ":" + rdr[2].ToString().Trim().Substring(2, 2) + ":" + rdr[2].ToString().Trim().Substring(rdr[2].ToString().Length - 2, 2);
                                timeGroup_EatTimeSecondEnd = datestr + " " + rdr["endTime"].ToString().Trim().Substring(0, 2) + ":" + rdr["endTime"].ToString().Trim().Substring(2, 2) + ":" + rdr["endTime"].ToString().Trim().Substring(rdr["endTime"].ToString().Length - 2, 2);
                                timeGroup_EatTimeSecondCount = timeGroup_EatTimeSecondCount + Convert.ToInt16(rdr[4].ToString().Trim());
                                break;
                            default:
                                break;
                        }
                        //rdr.NextResult();
                    }
                }
            }
            rdr.Close(); 
        }
        void InsertQty_count()
        {
            string sql = string.Empty;
            string Update_Item = string.Empty;
            string Update_Rate = string.Empty;
            string iPeriod = string.Empty;
            string sDateid = string.Empty;
            string sShift = string.Empty;
    
            string diffqty = string.Empty;
            string sPart_no = string.Empty;
            string sStarttime = string.Empty;
            string sLine = string.Empty;
            string sDate = string.Empty;
            string tDate = string.Empty;
            string Stand_Time = string.Empty;
            SqlDataReader rdr = null;
            GetSqltime();
            curBarcode_sTime = g_dSql_time;
            getShiftPeriod(g_dSql_time, out sDate, out sShift);
            getQtyHour(curLine_MLine, curBarcode_sTime, out Update_Item, out Update_Rate, sShift, out iPeriod);
            getDayTimeStart(g_dSql_time, sShift, out Stand_Time);
            sDateid = curLine_MLine + Convert.ToDateTime(Stand_Time).ToString("yyyyMMdd");
            
            Sql.DefaultDatabase = Sql.DatabaseOpt.MES;
            if (Sql.ServerExecute("select DATE_ID from QTY_Count with(nolock) where DATE_ID= '" + sDateid + "' and Mline= 'T300'", out rdr) == true)
            {
                rdr.Read();
                if (rdr.HasRows == false)
                {
                    rdr.Close();
                    if (Sql.ServerExecute("insert into QTY_Count(DATE_ID,MLINE) values ('" + sDateid + "','T300')", out rdr) == false)
                    {
                        SetMsg("QTY_Count 数据库插入数据错误", UDF.COLOR.FAIL);
                        return;
                    }
                    rdr.Close();
                }
                else
                { 
                    rdr.Close();
                }
            }
            try
            {
                Sql.ServerExecute("UPDATE QTY_Count SET " + Update_Item + "=" + Update_Item + " + 1   WHERE DATE_ID='" + sDateid + "' and Mline= 'T300'", out rdr);
            }
            catch (Exception ex)
            {
                MessageBox.Show(null, ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            rdr.Close(); 

            try
            {
                if (Convert.ToInt16(iPeriod) >= 1 && Convert.ToInt16(iPeriod) <= 12)
                {
                    Sql.ServerExecute("UPDATE QTY_Count SET TOTAL_A= TOTAL_A+1,MODEL_A='" + curBarcode_PartNo.Substring(13, 7) + "' WHERE DATE_ID='" + sDateid + "' and Mline= 'T300'", out rdr);
                }
                else
                {
                    Sql.ServerExecute("UPDATE QTY_Count SET TOTAL_B= TOTAL_B+1,MODEL_B='" + curBarcode_PartNo.Substring(13, 7) + "' WHERE DATE_ID='" + sDateid + "' and Mline= 'T300'", out rdr);
                }
                rdr.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(null, ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        void Mpq(string sSO, string sType, string sPartNo, DateTime sDatetime, string sLine, string sEline)
        {
            string sql = string.Empty;
            SqlDataReader rdr = null;
            SqlDataReader rdr2 = null;
            string sUserModel = string.Empty;
            string sShiftPeriod = string.Empty;
            string sRework = string.Empty;
            string smark = string.Empty;
            string sDate = string.Empty;
            string sHour = string.Empty;
            string sHourid = string.Empty;
            string sDateid = string.Empty;
            string sMonthID = string.Empty;
            string sWeekID = string.Empty;
            string sQuarterID = string.Empty;
            string tempTime = string.Empty;
            int nQty = 0;

            b_changeTime = false;
      
            GetShift(out sShiftPeriod);
            getSOMark(sShiftPeriod, out sDate, out sHour,out smark);
            if  (SDriverX.g_modelInfo.sWord.Substring(0,1)=="R" )
            {
                sRework = "R";
            }
            else
            {
                sRework = "N";
            }
            Sql.DefaultDatabase = Sql.DatabaseOpt.MES;
            Sql.ServerExecute("select * from mpq_new with(nolock) where Dayhour='" + sHour + "' and ELine='" + sEline + "' and  Line='" + sLine + "' and  ShiftAB='" + sShiftPeriod + "' and Station ='T300' order by starttime desc", out rdr);
            rdr.Read();
            if (rdr.HasRows==true)
            {
                if (rdr[3].ToString().Trim() == curBarcode_So)
                {
                    tempTime = rdr[8].ToString().Trim();
                    nQty = Convert.ToInt16(rdr[1].ToString().Trim());
                    nQty = nQty + 1;
                    rdr.Close();
                    sql = "update mpq_new set Quantity='" + nQty + "', Endtime= '" + sDatetime + "' where  starttime='" + tempTime + "' and  so ='" + curBarcode_So + "'  and  partno='" + curBarcode_PartNo + "' and  Line='" + sLine + "'and Station ='T300'";
                    if (Sql.ServerExecute(sql, out rdr2)==false)
                    {
                        SetMsg("mpq_new 更新数据失败！", UDF.COLOR.FAIL);
                        return;
                    }
                    rdr2.Close();
                }
                else
                {
                    try
                    {
                        rdr.Close();
                        b_changeTime = true;
                        GetDateID(sDatetime, out sHourid, out sDateid, out sWeekID, out sMonthID, out sQuarterID);
                        Sql.ServerExecute("Insert mpq_new(Dayhour,Quantity,Line,ELine, PartNo, So, DateID, WeekID, MonthID, QuarterID,starttime,endtime,ShiftAB, Onot, Rework,station,type,Mark) values ( '" + sHourid + "', '1', '" + sLine + "','" + sEline + "','" + sPartNo + "','" + curBarcode_So + "'," +
                            "'" + sDate + "', '" + sWeekID + "','" + sMonthID + "','" + sQuarterID + "','" + sDatetime + "','" + sDatetime + "', '" + sShiftPeriod + "',  '" + smark + "', '" + sRework + "','T300', '" + sType + "','Y')", out rdr2);
                        rdr2.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(null, ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
            }
            else
            {
                try
                {
                    rdr.Close();
                    b_changeTime = true;
                    GetDateID(sDatetime, out sHourid, out sDateid, out sWeekID, out sMonthID, out sQuarterID);
                    Sql.ServerExecute("Insert mpq_new(Dayhour,Quantity,Line,ELine, PartNo, So, DateID, WeekID, MonthID, QuarterID,starttime,endtime,ShiftAB, Onot, Rework,station,type,Mark) values ( '" + sHourid + "', '1', '" + sLine + "','" + sEline + "','" + sPartNo + "','" + curBarcode_So + "'," +
                        "'" + sDate + "', '" + sWeekID + "','" + sMonthID + "','" + sQuarterID + "','" + sDatetime + "','" + sDatetime + "', '" + sShiftPeriod + "',  '" + smark + "', '" + sRework + "','T300', '" + sType + "','Y')", out rdr2);
                    rdr2.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(null, ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

            }
            rdr.Close();
        }
        void GetDateID(DateTime sDatetime,out string sHourid,out string sDateid,out string sWeekID,out string sMonthID,out string sQuarterID)
        {
            string sTime = string.Empty;
            string strDate = string.Empty;
            string sWeekID1 = string.Empty;
            string sTemp = string.Empty;
            int iQ = 0;
            DateTime tDate, tDate1;
            sTime = sDatetime.ToString("HH");
            sHourid = sDatetime.ToString("yyyyMMdd")+ sTime;
            if (Convert.ToInt16 (sTime)>=8)
            {
                tDate = sDatetime;
            }
            else
            {
                tDate = Convert.ToDateTime (sDatetime.AddDays(-1).ToString("yyyy-MM-dd"));
            }
            sDateid = tDate.ToString("yyyyMMdd");
            sWeekID = ((tDate.DayOfYear + Convert.ToInt32(Convert.ToDateTime(tDate.ToString("yyyy") + "/01" + "/01").DayOfWeek) - 1) / 7 + 1).ToString();
            if (Convert.ToInt32(tDate.DayOfWeek)+1>=5)
            {
                sWeekID = Convert.ToString(Convert.ToInt16(sWeekID) + 1);
            }

            if (sWeekID.Length >1 )
            {
                sWeekID = sDatetime.ToString("yyyy") + sWeekID;
            }
            else
            {
                sWeekID = sDatetime.ToString("yyyy") + "0" + sWeekID;
            }

            tDate1 = Convert.ToDateTime(sDatetime.AddMonths(1).ToString("yyyy-MM"));
            sWeekID1 = ((tDate1.DayOfYear + Convert.ToInt32(Convert.ToDateTime(tDate1.ToString("yyyy") + "/01" + "/01").DayOfWeek) - 1) / 7 + 1).ToString();
            if (Convert.ToInt32(tDate1.DayOfWeek)+1 >= 5)
            {
                sWeekID1 = Convert.ToString(Convert.ToInt16(sWeekID1) + 1);
            }
            sWeekID1 = tDate1.ToString("yyyy") + sWeekID1;
            if (sWeekID== sWeekID1)
            {
                sMonthID = tDate1.ToString("yyyyMM");
            }
            else
            {
                sMonthID = tDate.ToString("yyyyMM");
            }
            sTemp= sMonthID.Substring(sMonthID.Length - 2, 2);//Right 取2位
            //iQ = Convert.ToInt16(sMonthID.Substring(sMonthID.Length -2),2);  
            iQ = Convert.ToInt16(Convert.ToInt16(sTemp)- 1) / 3;
            iQ = iQ + 1;
            sQuarterID = sMonthID.Substring(0,4).ToString() + iQ.ToString();
        }
        public static int Weekday(DateTime dt, DayOfWeek startOfWeek)
        {
            return (dt.DayOfWeek - startOfWeek + 7) % 7;
        }
        void getShiftPeriod(DateTime sDatetime, out string sDate, out string sShift)
        {
            string sTime = string.Empty;
            string hh = string.Empty;
            string mm = string.Empty;

            sDate =g_dSql_time.ToString("yyyyMMdd");
            sTime = g_dSql_time.ToString("HHmmss");
            hh = g_dSql_time.ToString("HH");
            mm = g_dSql_time.ToString("mm");
            if (Convert.ToInt16(hh) >= 8 && Convert.ToInt16(hh) < 20)
            {
                sShift = "A";
            }
            else
            {
                sShift = "B";
            }
        }
        void getQtyHour(string sLine, DateTime sDatetime, out string Update_Item, out string Update_Rate, string sShiftAB, out string iPeriod)
        {
            string Stand_Time = string.Empty;
            long iHourdiff =0;
            long iiPeriod = 0;

            Update_Item = "";
            Update_Rate = "";
            getDayTimeStart(sDatetime,sShiftAB, out Stand_Time);

            //TimeSpan timeSpan= sDatetime- Convert.ToDateTime (Stand_Time);
            iHourdiff = Convert.ToInt32(DiffMinutes(Convert.ToDateTime(Stand_Time), Convert.ToDateTime(sDatetime)));
            if (iHourdiff<0)
            {
                if (sShiftAB=="A")
                {
                    Update_Item = "NO1_QTY";
                    Update_Rate = "NO1_PF";
                    iiPeriod = 1;
                }
                else
                { 
                    Update_Item = "NO13_QTY";
                    Update_Rate = "NO13_PF";
                    iiPeriod = 13;
                }
            }
            else if (iHourdiff < 60)   // '8:00~8:59
            {
                Update_Item = "NO1_QTY";
                Update_Rate = "NO1_PF";
                iiPeriod = 1;
            }
            else if (iHourdiff < 120 && iHourdiff>=60)  //9:00~09:59
            {
                Update_Item = "NO2_QTY";
                Update_Rate = "NO2_PF";
                iiPeriod = 2;
            }
            else if (iHourdiff < 180 && iHourdiff >=120)  //10:00~10:59
            {
                Update_Item = "NO3_QTY";
                Update_Rate = "NO3_PF";
                iiPeriod = 3;
            }
            else if (iHourdiff < 240 && iHourdiff >=180)  // 10:00~11:59
            {
                Update_Item = "NO4_QTY";
                Update_Rate = "NO4_PF";
                iiPeriod = 4;
            }
            else if (iHourdiff < 300 && iHourdiff >=240)  // 12:00~12:59
            {
                Update_Item = "NO5_QTY";
                Update_Rate = "NO5_PF";
                iiPeriod = 5;
            }
            else if (iHourdiff < 360 && iHourdiff >=300)  // 13:00~13:59
            {
                Update_Item = "NO6_QTY";
                Update_Rate = "NO6_PF";
                iiPeriod = 6;
            }
            else if (iHourdiff < 420 && iHourdiff >=360)  // 14:00~14:59
            {
                Update_Item = "NO7_QTY";
                Update_Rate = "NO7_PF";
                iiPeriod = 7;
            }
            else if (iHourdiff < 480 && iHourdiff >=420)  // 15:00~15:59
            {
                Update_Item = "NO8_QTY";
                Update_Rate = "NO8_PF";
                iiPeriod = 8;
            }
            else if (iHourdiff < 540 && iHourdiff >=480)  // 16:00~16:59
            {
                Update_Item = "NO9_QTY";
                Update_Rate = "NO9_PF";
                iiPeriod = 9;
            }
            else if (iHourdiff < 600 && iHourdiff >=540)  // 17:00~17:59
            {
                Update_Item = "NO10_QTY";
                Update_Rate = "NO10_PF";
                iiPeriod = 10;
            }
            else if (iHourdiff < 660 && iHourdiff >=600)  // 18:00~18:59
            {
                Update_Item = "NO11_QTY";
                Update_Rate = "NO11_PF";
                iiPeriod = 11;
            }
            else if (iHourdiff < 720 && iHourdiff >=660)   //19:00~19:59
            {
                Update_Item = "NO12_QTY";
                Update_Rate = "NO12_PF";
                iiPeriod = 12;
            }
            else if (iHourdiff < 780 && iHourdiff >=720)  //  20:00~20:59
            {
                if (sShiftAB == "A")
                {
                    Update_Item = "NO12_QTY";
                    Update_Rate = "NO12_PF";
                    iiPeriod = 12;
                }
                else
                {
                    Update_Item = "NO13_QTY";
                    Update_Rate = "NO13_PF";
                    iiPeriod = 13;
                }
            }
            else if (iHourdiff < 840 && iHourdiff >=780)  //  21:00~21:59
            {
                if (sShiftAB == "A")
                {
                    Update_Item = "NO12_QTY";
                    Update_Rate = "NO12_PF";
                    iiPeriod = 12;
                }
                else
                {
                    Update_Item = "NO14_QTY";
                    Update_Rate = "NO14_PF";
                    iiPeriod = 14;
                }
            }
            else if (iHourdiff < 900 && iHourdiff >=840)  //  22:00~22:59
            {
                if (sShiftAB == "A")
                {
                    Update_Item = "NO12_QTY";
                    Update_Rate = "NO12_PF";
                    iiPeriod = 12;
                }
                else
                {
                    Update_Item = "NO15_QTY";
                    Update_Rate = "NO15_PF";
                    iiPeriod = 15;
                }
            }
            else if (iHourdiff < 960 && iHourdiff >=900)  //  23:00~23:59
            {
                if (sShiftAB == "A")
                {
                    Update_Item = "NO12_QTY";
                    Update_Rate = "NO12_PF";
                    iiPeriod = 12;
                }
                else
                {
                    Update_Item = "NO16_QTY";
                    Update_Rate = "NO16_PF";
                    iiPeriod = 16;
                }
            }
            else if (iHourdiff < 1020 && iHourdiff >=960)   // 00:00~00:59
            {
                Update_Item = "NO17_QTY";
                Update_Rate = "NO17_PF";
                iiPeriod = 17;
            }
            else if (iHourdiff < 1080 && iHourdiff >=1020)   // 01:00~01:59
            {
                Update_Item = "NO18_QTY";
                Update_Rate = "NO18_PF";
                iiPeriod = 18;
            }
            else if (iHourdiff < 1140 && iHourdiff >=1080)   // 02:00~02:59
            {
                Update_Item = "NO19_QTY";
                Update_Rate = "NO19_PF";
                iiPeriod = 19;
            }
            else if (iHourdiff < 1200 && iHourdiff >=1140)   // 03:00~03:59
            {
                Update_Item = "NO20_QTY";
                Update_Rate = "NO20_PF";
                iiPeriod = 20;
            }
            else if (iHourdiff < 1260 && iHourdiff >=1200)   // 04:00~04:59
            {
                Update_Item = "NO21_QTY";
                Update_Rate = "NO21_PF";
                iiPeriod = 21;
            }
            else if (iHourdiff < 1320 && iHourdiff >=1260)   // 05:00~05:59
            {
                Update_Item = "NO22_QTY";
                Update_Rate = "NO22_PF";
                iiPeriod =22;
            }
            else if (iHourdiff < 1380 && iHourdiff >=1320)   // 06:00~06:59
            {
                Update_Item = "NO23_QTY";
                Update_Rate = "NO23_PF";
                iiPeriod = 23;
            }
            else if (iHourdiff < 1440 && iHourdiff >=1380)   // 07:00~07:59
            {
                Update_Item = "NO24_QTY";
                Update_Rate = "NO24_PF";
                iiPeriod = 24;
            }
            else if (iHourdiff > 1440)
            {
                Update_Item = "NO24_QTY";
                Update_Rate = "NO24_PF";
                iiPeriod = 24;
            }
            iPeriod = Convert.ToString (iiPeriod);
        }
        void getDayTimeStart(DateTime sDatetime,string sShiftAB,out string Stand_Time)
        {
            string sDate = string.Empty;
            string sHour = string.Empty;
            sHour = sDatetime.ToString("HH");
            if (Convert.ToInt16(sHour)>=0 && Convert.ToInt16(sHour) <8)
            {
                if  (sShiftAB=="A")
                {
                    sDate = sDatetime.ToString("yyyy-MM-dd") + " 08:00:00";
                }
                else
                {
                    sDate = sDatetime.AddDays(-1).ToString("yyyy-MM-dd") + " 08:00:00";
                }
            }
            else if (Convert.ToInt16(sHour) >= 8 && Convert.ToInt16(sHour) < 20)
            {
                if (sShiftAB == "B")
                {
                    if (Convert.ToInt16(sHour)<=12)
                    {
                        sDate = sDatetime.AddDays(-1).ToString("yyyy-MM-dd") + " 08:00:00";
                    }
                    else
                    {
                        sDate = sDatetime.ToString("yyyy-MM-dd") + " 08:00:00";
                    }
                }
                else
                {
                    sDate = sDatetime.ToString("yyyy-MM-dd") + " 08:00:00";
                }
            }
            else
            {
                sDate = sDatetime.ToString("yyyy-MM-dd") + " 08:00:00";
            }
            Stand_Time = sDate;
        }
        void GetShift(out string shift)
        {
            string sDatetime = string.Empty;
            string sDate = string.Empty;
            string iHour = string.Empty;
            GetSqltime();
            iHour = g_dSql_time.ToString("HH");
            if  (Convert.ToInt16(iHour) >= 8 && Convert.ToInt16(iHour) < 20)
            {
                shift = "A";
                sDate = g_dSql_time.ToString("yyyyMMdd");
            }
            else
            {
                shift = "B";
            }
        }
        void getSOMark(string sShift, out string sDate, out string sHour,out string smark)
        {
            DateTime sDatetime;
            string sCurDate = string.Empty;
            string sNormalShift = string.Empty;
            string iHour = string.Empty;
            sDatetime = curBarcode_sTime;
            sCurDate = sDatetime.ToString("yyyyMMdd");
            iHour = sDatetime.ToString("HH");
            if (sShift=="A")
            {
                if (Convert.ToInt16(iHour)>=0 && Convert.ToInt16(iHour) <17)
                {
                    smark = "ON";
                }
                else
                {
                    smark = "OFF";
                }
                sDate = sDatetime.ToString("yyyyMMdd");
            }
            else
            {
                if (Convert.ToInt16(iHour) >= 0 && Convert.ToInt16(iHour) < 5)
                {
                    smark = "ON";
                    sDate = sDatetime.AddDays(-1).ToString("yyyyMMdd");
                }
                else
                {
                    if (Convert.ToInt16(iHour) >= 5 && Convert.ToInt16(iHour) < 17)
                    {
                        smark = "OFF";
                        sDate = sDatetime.AddDays(-1).ToString("yyyyMMdd");
                    }
                    else
                    {
                        smark = "ON";
                        sDate = sDatetime.ToString("yyyyMMdd");
                    }
                }
            }
            iHour = sDatetime.ToString("HH");
            sHour = sCurDate + iHour;
        }

        private void textBoxSn_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
