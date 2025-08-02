using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRETEST.UDFs
{
    public class UDF
    {
        public enum COLOR { PASS = 0, FAIL, WORK, WORK2, WARN };  //信息提示栏的背景色

        /// <summary>
        /// 机种信息
        /// </summary>
        public struct ModelInfo //机种信息类型
        {
            public string sMode;
            public string sTv_offline;
            public string sFinish_type;
            public string sProductname;
            public string sWord;
            public string sPart_no;
            public string sModel;
            public string sSimple_model;
            public string sRtn_cd;
            public string sErr_eng_msg;
            public string sErr_msg;
            public string sSn;

            public string sExtSn;
            //public string sMac;
            public string sBtRssi;
            public string sFw;
            public string sModelName;
            public string sNetSpeed;
            public string sMn;
            public string sMBMn;
            public string sEthMac;
            public string sWifiMac;
            public string sBtMac;
            public string sDidKey;
            public int sUseFlag;
            //public MicSplData micSplData;
        }

        /// <summary>
        /// 复杂控件设置
        /// </summary>
        public struct ComplexConSetting
        {
            public string sDataGradView;  //DataGradView
            public string sStatusStripVersion;  //statusStripVersion
        }

        public struct MicSplData
        {
            public float[] fDc;
            public float[] fNoi;
            public float[] fSig;
            public float[] fSnr;
            public float[] fThd;
            public float[,] fMag;
            public int[,] fPhs;
        }

        public enum RunMode { NULL = 0, PRE, RST, TV1 ,MIC };   //程序运行模式

        public enum Record { ADC = 0, HDP, PASS };

        public enum MAC_TYPE { ETH = 0, BT, WIFI };

        public enum MAC_STYLE {
            //U:up, L:low, C:colon, B:blank
            /// <summary>
            /// 长度12，大写。例如："C463FB000003"
            /// </summary>
            U12 = 0,
            /// <summary>
            /// 长度12，小写。例如："c463fb000003"
            /// </summary>
            L12,
            /// <summary>
            /// 长度17，大写，冒号间隔，例如："C4:63:FB:00:00:03"
            /// </summary>
            UC17,
            /// <summary>
            /// 长度17，小写，冒号间隔，例如："c4:63:fb:00:00:03"
            /// </summary>
            LC17,
            /// <summary>
            /// 长度17，大写，空格间隔，例如："C4 63 FB 00 00 03"
            /// </summary>
            UB17,
            /// <summary>
            /// 长度17，小写，空格间隔，例如："c4 63 fb 00 00 03"
            /// </summary>
            LB17

        };
        }
}
