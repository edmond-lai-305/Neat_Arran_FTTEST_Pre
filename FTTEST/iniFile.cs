using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace wbc
{
    class iniFile
    {
        //************************************

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string lpAppName, string lpKeyName, string lpDefault, StringBuilder lpReturnedString, int nSize, string lpFileName);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern uint GetPrivateProfileSection(string lpAppName, IntPtr lpReturnedString, uint nSize, string lpFileName);


        private static string ReadString(string section, string key, string def, string filePath)
        {
            StringBuilder temp = new StringBuilder(1024);

            try
            {
                GetPrivateProfileString(section, key, def, temp, 1024, filePath);
            }
            catch
            { }
            if (temp.ToString() == "") temp.AppendLine("0");

            return temp.ToString().Trim().Replace("\r", "").Replace("\n", "");
        }
        /// <summary>  
        /// 根据section取所有key  
        /// </summary>  
        /// <param name="section"></param>  
        /// <param name="filePath"></param>  
        /// <returns></returns>  
        public static string[] ReadIniAllKeys(string section, string filePath)
        {
            UInt32 MAX_BUFFER = 32767;

            string[] items = new string[0];

            IntPtr pReturnedString = Marshal.AllocCoTaskMem((int)MAX_BUFFER * sizeof(char));

            UInt32 bytesReturned = GetPrivateProfileSection(section, pReturnedString, MAX_BUFFER, filePath);

            if (!(bytesReturned == MAX_BUFFER - 2) || (bytesReturned == 0))
            {
                string returnedString = Marshal.PtrToStringAuto(pReturnedString, (int)bytesReturned);

                items = returnedString.Split(new char[] { '\0' }, StringSplitOptions.RemoveEmptyEntries);
            }

            Marshal.FreeCoTaskMem(pReturnedString);

            return items;
        }
        /// <summary>
        /// 得到某个节点下面所有的key和value组合
        /// </summary>
        /// <param name="section"></param>
        /// <param name="keys"></param>
        /// <param name="values"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static int GetAllKeyValues(string section, out string[] keys, out string[] values, string path)
        {
            UInt32 MAX_BUFFER = 32767;
            //byte[] b = new byte[65535];
            IntPtr pReturnedString = Marshal.AllocCoTaskMem((int)MAX_BUFFER * sizeof(char));

            UInt32 bytesReturned = GetPrivateProfileSection(section, pReturnedString, MAX_BUFFER, path);
            //string s = System.Text.Encoding.Default.GetString(pReturnedString);
            string s = Marshal.PtrToStringAuto(pReturnedString, (int)bytesReturned);
            string[] tmp = s.Split((char)0);
            ArrayList result = new ArrayList();
            foreach (string r in tmp)
            {
                if (r != string.Empty)
                    result.Add(r);
            }
            keys = new string[result.Count];
            values = new string[result.Count];
            for (int i = 0; i < result.Count; i++)
            {
                string[] item = result[i].ToString().Split(new char[] { '=' });
                if (item.Length == 2)
                {
                    keys[i] = item[0].Trim();
                    values[i] = item[1].Trim();
                }
                else if (item.Length == 1)
                {
                    keys[i] = item[0].Trim();
                    values[i] = "";
                }
                else if (item.Length == 0)
                {
                    keys[i] = "";
                    values[i] = "";
                }
            }

            return 0;
        }


        /// <summary>  
        /// 根据section，key取值  ReadIniKeys
        /// </summary>  
        /// <param name="section"></param>  
        /// <param name="keys"></param>  
        /// <param name="filePath">ini文件路径</param>  
        /// <returns></returns>  
        public static string GetPrivateStringValue(string section, string keys, string filePath)
        {

            string a = Left(filePath, 2).ToString();
            if (a == "\\")
            {
             // string ServerName = GetPrivateStringValueSFC("DBServer", "ServerName", Definition.SpecFilePath + @"\ServerPath\DBinfo.ini");
              //  if (ServerName == "0")
                {
                    //FrmBase.ShowError();
                }
            }
            return ReadString(section, keys, "", filePath);
        }

        public static string GetPrivateStringValueSFC(string section, string keys, string filePath)
        {
            return ReadString(section, keys, "", filePath);
        }
        /// <summary>  
        /// 保存ini  
        /// </summary>  
        /// <param name="section"></param>  
        /// <param name="key"></param>  
        /// <param name="value"></param>  
        /// <param name="filePath">ini文件路径</param>  
        public static void WriteIniKeys(string section, string key, string value, string filePath)
        {
            WritePrivateProfileString(section, key, value, filePath);
        }


        public static void WritePrivateProfileStringByKeyName(string lpApplicationName, string lpKeyName, string lpString, string lplFileName)
        {
            WriteIniKeys(lpApplicationName, lpKeyName, lpString, lplFileName);
        }

        public static string Left(string s, int l)
        {
            if (s.Length > l)
            {
                return s.Substring(0, l);
            }
            else
            {
                return s;
            }
        }
        public static string Right(string s, int l)
        {
            if (s.Length > l)
            {
                return s.Substring(s.Length - l, l);
            }
            else
            {
                return s;
            }
        }
        public static string Mid(string sSource, int iStart, int iLength)
        {
            iStart = iStart - 1;
            int iStartPoint = iStart > sSource.Length ? sSource.Length : iStart;
            return sSource.Substring(iStartPoint, iStartPoint + iLength > sSource.Length ? sSource.Length - iStartPoint : iLength);
        }
        public static string FmtStr(string s)
        {
            return s.ToUpper().Trim().Replace("\r", "").Replace("\n", "").ToString();
        }
        public static bool IsNumeric(string s)
        {
            Regex reg = new Regex("^[0-9]+$");
            if (reg.IsMatch(s) == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static void WriteCIMlog(string TempSTR)
        {
            string TempPath;// As String

            TempPath = AppDomain.CurrentDomain.BaseDirectory + "\\cimlog.csv";   // .Path + "\cimlog.csv"
            //Open TempPath For Append As #1
            //    Print #1, TempSTR
            //Close #1
            FileStream fs = new FileStream(TempPath, FileMode.Append);
            //实例化一个StreamWriter-->与fs相关联  
            StreamWriter sw = new StreamWriter(fs);
            //开始写入  
            sw.WriteLine(TempSTR);
            //清空缓冲区  
            sw.Flush();
            //关闭流  
            sw.Close();
            fs.Close();
        }


        public static void WriteTestLog(string TempSTR)
        {
            string TempPath;
            TempPath = AppDomain.CurrentDomain.BaseDirectory + "\\TestLog.txt";
            FileStream fs = new FileStream(TempPath, FileMode.Append);
            StreamWriter sw = new StreamWriter(fs);
            sw.WriteLine(TempSTR);
            sw.Flush();
            sw.Close();
            fs.Close();
        }
        //将MAC写入TXT文本中


        public static void WriteMACLog(string TempSTR)
        {
            string TempPath;
            TempPath = AppDomain.CurrentDomain.BaseDirectory + "\\MACLog.txt";
            FileStream fs = new FileStream(TempPath, FileMode.Append);
            StreamWriter sw = new StreamWriter(fs);
            sw.WriteLine(TempSTR);
            sw.Flush();
            sw.Close();
            fs.Close();
        }
        public static void WriteFile0(string TempSTR, string FileName)
        {
            string TempPath;// As String
            try
            {
                TempPath = FileName;
                FileStream fs = new FileStream(TempPath, FileMode.Append);
                StreamWriter sw = new StreamWriter(fs);
                sw.WriteLine(TempSTR);
                sw.Flush();
                sw.Close();
                fs.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }



        public static void WriteFile(string TempSTR, string FileName)
        {
            //string TempPath;// As String
            //TempPath = "";
            //TempPath = FileName;
            ////if (Directory.Exists(TempPath)==false)
            ////{
            ////    Directory.CreateDirectory(TempPath);
            ////}
            ////TempPath = TempPath + "\\\\" + Definition.PJ_Data.Model;
            ////if (Directory.Exists(TempPath ) == false)
            ////{
            ////    Directory.CreateDirectory(TempPath);
            ////}
            ////TempPath = TempPath + "\\\\" + Definition.PJ_Data.PN;
            ////if (Directory.Exists(TempPath) == false)
            ////{
            ////    Directory.CreateDirectory(TempPath);
            ////}
            //TempPath = TempPath + "\\\\" + Definition.PJ_Data.Model + "\\\\" + Definition.PJ_Data.PN + "\\\\" + Definition.PJ_Data.ID + "\\\\";
            //if (Directory.Exists(TempPath) == false)
            //{
            //    Directory.CreateDirectory(TempPath);
            //}
            ////    TempPath = TempPath + "\\\\" + Definition.PJ_Data.SN + ".txt";
            ////if (File.Exists(TempPath)==false)
            ////{
            //// File.Create(TempPath);
            ////File.OpenWrite(TempPath);
            //// File.Exists(TempPath);
            ////    }
            //TempPath = TempPath + Definition.PJ_Data.SN + "-" + DateTime.Now.ToString("yyyyMMdd-HHmmss") + ".txt";
            //FileStream fs = new FileStream(TempPath, FileMode.Append);
            //StreamWriter sw = new StreamWriter(fs);

            //sw.WriteLine(TempSTR);
            //sw.Flush();
            //sw.Close();
            //fs.Close();
        }
        public static void WriteFile1(string TempSTR, string FileName)
        {
            string TempPath;// As String
            TempPath = "";
            TempPath = FileName;

            //TempPath = TempPath + "\\\\" + Definition.PJ_Data.Model + "\\\\" + Definition.PJ_Data.ID + "\\\\";
            //if (Directory.Exists(TempPath) == false)
            //{
            //    Directory.CreateDirectory(TempPath);
            //}

            //TempPath = TempPath + Definition.PJ_Data.ID + "-" + DateTime.Now.ToString("yyyyMMdd-HHmmss") + ".txt";
            //FileStream fs = new FileStream(TempPath, FileMode.Append);
            //StreamWriter sw = new StreamWriter(fs);

            //sw.WriteLine(TempSTR);
            //sw.Flush();
            //sw.Close();
            //fs.Close();
        }
    }
}
