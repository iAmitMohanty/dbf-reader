using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DBFReader
{
    public partial class GoTo : Form
    {
        public int currRecNo, totRecNo;

        public GoTo()
        {
            InitializeComponent();
        }

        private void GoTo_Load(object sender, EventArgs e)
        {
            lblRecord.Text = "Record Number (1 - " + totRecNo.ToString() + "):";
            txtRecord.Text = currRecNo.ToString();
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            currRecNo = int.Parse(txtRecord.Text);
            this.Close();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void TxtRecord_TextChanged(object sender, EventArgs e)
        {
            if (txtRecord.Text == string.Empty)
            {
                btnOk.Enabled = false;
            }
            else
            {
                btnOk.Enabled = true;
            }
        }

        private void TxtRecord_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar >= 48 && e.KeyChar <= 57 || e.KeyChar == 8)
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }
    }
}
