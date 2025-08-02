using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PRETEST
{
    public partial class InputSn : Form
    {
        public string sSn = string.Empty;
        public InputSn()
        {
            InitializeComponent();
        }

        private void textBoxSn_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                if (textBoxSn.Text.Trim() == string.Empty) return;

                this.sSn = textBoxSn.Text.Trim().ToUpper();
                this.DialogResult = DialogResult.OK;
            }
        }
    }
}
