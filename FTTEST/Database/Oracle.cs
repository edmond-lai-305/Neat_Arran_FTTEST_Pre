using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OleDb;

namespace PRETEST.Database
{
    public class Oracle
    {
        static OleDbConnection conn = new OleDbConnection();
        public static string err_msg = string.Empty;
        public static event Database.ErrorsHandler Errors;
        /// <summary>
        /// 初始化数据库连接,ConnectionString="Provider= MSDAORA.1;Data Source=RKMES;User ID=MESUSR;Password=musrtest1;Persist Security Info=True"
        /// </summary>
        /// <returns></returns>
        public static bool Open(string ConnectionString)
        {
            err_msg = string.Empty;
            /*
             Provider= MSDAORA.1;Data Source=RKMES;User ID=MESUSR;Password=musrtest1;Persist Security Info=True;
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
        public static bool ServerExecute(string sql, out OleDbDataReader reader)
        {
            reader = null;
            err_msg = string.Empty;
            using (OleDbCommand cmd = new OleDbCommand())
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
        /// <summary>
        /// 执行数据库更新操作,无返回值
        /// </summary>
        /// <param name="sql">SQL 语句</param>
        /// <returns></returns>
        public static bool UpdateServer(string sql)
        {
            err_msg = string.Empty;
            using (OleDbCommand cmd = new OleDbCommand())
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
    }
}
