using System;
using System.Data.OleDb;
using System.Windows.Forms;

namespace PRETEST
{
    class ClassOracleDatabase
    {
        static OleDbConnection conn = new OleDbConnection();
        public static bool open()
        {
            conn.ConnectionString = @"Provider= MSDAORA.1;Data Source=RKMES;User ID=MESUSR;Password=musrtest1;Persist Security Info=True;";
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

        public static void close()
        {
            if (conn != null)
            {
                conn.Close();
            }
        }

        public static bool serverExecute(string sql, out OleDbDataReader reader)
        {
            reader = null;
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
                    MessageBox.Show(null, e.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            return true;
        }
        public static bool updateServer(string sql)
        {
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
                    MessageBox.Show(null, e.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

            }
            return true;
        }

    }
}
