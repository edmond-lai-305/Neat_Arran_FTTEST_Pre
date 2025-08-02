using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRETEST.Database
{
    public class Database
    {
        public delegate void ErrorsHandler(ErrorsEvent errors);
    }
    public class ErrorsEvent : EventArgs
    {
        public readonly string Message;

        public ErrorsEvent(string msg)
        {
            this.Message = msg;
        }
    }
}
