using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace PRETEST.Database
{
    public class Sql
    {
        static SqlConnection conn = new SqlConnection();
        public static string err_msg = string.Empty;
        public static event Database.ErrorsHandler Errors;
        /// <summary>
        /// 初始化数据库连接,ConnectionString="Data Source=192.168.158.29;Initial Catalog=RAKEN_TE; User ID=te;Password=te"
        /// </summary>
        /// <returns></returns>
        public static bool Open(string ConnectionString)
        {
            err_msg = string.Empty;
            /*
             Data Source=192.168.158.29;Initial Catalog=RAKEN_TE; User ID=te;Password=te
             */
            try
            {
                conn.ConnectionString = ConnectionString;
                conn.Open();
            }
            catch (Exception e)
            {
                err_msg = e.Message;
                Errors?.Invoke(new ErrorsEvent(e.Message));
                return false;
            }
            return true;
        }
        /// <summary>
        /// 关闭数据库连接
        /// </summary>
        public static void Close()
        {
            if (conn != null)
            {
                conn.Close();
            }
        }
        /// <summary>
        /// 执行数据库查询操作,并返回结果
        /// </summary>
        /// <param name="sql">SQL 语句</param>
        /// <param name="reader">结果集</param>
        /// <returns></returns>
        public static bool ServerExecute(string sql, out SqlDataReader reader)
        {
            reader = null;
            err_msg = string.Empty;
            using (SqlCommand cmd = new SqlCommand())
            {
                try
                {
                    cmd.Connection = conn;
                    cmd.CommandText = sql;
                    reader = cmd.ExecuteReader();
                    cmd.Dispose();
                }
                catch (Exception e)
                {
                    err_msg = e.Message;
                    Errors?.Invoke(new ErrorsEvent(e.Message));
                    return false;
                }
            }
            return true;
        }
        public static bool ServerExecuteX(string sql, out SqlDataReader reader,out int sqlCount)
        {
            reader = null;
            err_msg = string.Empty;
            sqlCount = 0;
            using (SqlCommand cmd = new SqlCommand())
            {
                try
                {
                    cmd.Connection = conn;
                    cmd.CommandText = sql;
                    reader = cmd.ExecuteReader();
                    //sqlCount=cmd.ExecuteNonQuery();
                    cmd.Dispose();
                }
                catch (Exception e)
                {
                    err_msg = e.Message;
                    Errors?.Invoke(new ErrorsEvent(e.Message));
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// 执行数据库更新操作,无返回值
        /// </summary>
        /// <param name="sql">SQL 语句</param>
        /// <returns></returns>
        public static bool UpdateServer(string sql)
        {
            err_msg = string.Empty;
            using (SqlCommand cmd = new SqlCommand())
            {
                try
                {
                    cmd.Connection = conn;
                    cmd.CommandText = sql;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                }
                catch (Exception e)
                {
                    err_msg = e.Message;
                    Errors?.Invoke(new ErrorsEvent(e.Message));
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// 限制可供使用的数据库
        /// </summary>
        public enum DatabaseOpt { RAKEN_TE = 0, RAKEN_SFIS, MES, NULL }
        /// <summary>
        /// 查询或设置默认数据库
        /// </summary>
        public static DatabaseOpt DefaultDatabase
        {
            get
            {
                if (conn.Database == "RAKEN_TE")
                {
                    return DatabaseOpt.RAKEN_TE;
                }
                else if (conn.Database == "RAKEN_SFIS")
                {
                    return DatabaseOpt.RAKEN_SFIS;
                }
                else if (conn.Database == "MES")
                {
                    return DatabaseOpt.MES;
                }
                else
                {
                    return DatabaseOpt.NULL;
                }
            }
            set
            {
                if (DatabaseOpt.RAKEN_SFIS == value)
                {
                    conn.ChangeDatabase("RAKEN_SFIS");
                }
                else if (DatabaseOpt.RAKEN_TE == value)
                {
                    conn.ChangeDatabase("RAKEN_TE");
                }
                else if (DatabaseOpt.MES == value)
                {
                    conn.ChangeDatabase("MES");
                }
            }
        }
    }
}