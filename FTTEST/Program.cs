using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Threading;

namespace PRETEST
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            bool bCreateNew = new bool();

            using (Mutex mutex = new Mutex(true, Application.ProductName, out bCreateNew))
            {
                if (bCreateNew)
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new FormMain());
                }
                else
                {
                    MessageBox.Show(null, "程序已经运行了，无需再运行。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }
    }
}
