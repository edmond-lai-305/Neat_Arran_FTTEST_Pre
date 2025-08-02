using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PRETEST.SDriver;
using PRETEST.UDFs;
using PRETEST.AppConfig;
using PRETEST.Database;
using System.Data.OleDb;
using System.Windows.Forms;

namespace PRETEST.TestProcess
{
    class TestProcess
    {
    }
}
namespace PRETEST
{
    public partial class FormMain
    {
        bool PRE_MainTestProcess()
        {
            switch (GlobalConfig.sModel.ToUpper())
            {
                case "TELLY":
                    if (PRE_Telly() == false) return false;
                    break;
                case "ARRAN":
                    if (PRE_Arran() == false) return false;
                    break;
            }
            return true;
        }

        bool RST_MainTestProcess()
        {
            if (RST_Telly() == false) return false;
            return true;
        }

        bool TV1_MainTestProcess()
        {
            if (TV1_Telly() == false) return false;
            return true;
        }
        
    }
}
