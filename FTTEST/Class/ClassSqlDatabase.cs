using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace PRETEST
{
    public static class ClassSqlDatabase
    {
        static SqlConnection conn = new SqlConnection();

        public static bool Open()
        {
            conn.ConnectionString = @"Data Source=192.168.158.29;Initial Catalog=RAKEN_TE; User ID=te;Password=te";
            try
            {
                conn.Open();
            }
            catch (Exception e)
            {
                MessageBox.Show(null, e.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }
        public static void Close()
        {
            if (conn != null)
            {
                conn.Close();
            }
        }
        public static bool ServerExecute(string sql, out SqlDataReader reader)
        {
            reader = null;
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
                    MessageBox.Show(null, e.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            return true;
        }
        
        public static bool UpdateServer(string sql)
        {
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
                    MessageBox.Show(null, e.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            return true;
        }

        public enum DatabaseOpt {RAKEN_TE = 0, RAKEN_SFIS, NULL}

        public static DatabaseOpt DefaultDatabase
        {
            get {
                if (conn.Database == "RAKEN_TE")
                {
                    return DatabaseOpt.RAKEN_TE;
                }
                else if (conn.Database == "RAKEN_SFIS")
                {
                    return DatabaseOpt.RAKEN_SFIS;
                }
                else
                {
                    return DatabaseOpt.NULL;
                }
            }
            set {
                if (DatabaseOpt.RAKEN_SFIS == value)
                {
                    conn.ChangeDatabase("RAKEN_SFIS");
                }
                else if (DatabaseOpt.RAKEN_TE == value)
                {
                    conn.ChangeDatabase("RAKEN_TE");
                }
            }
        }
    }
}
