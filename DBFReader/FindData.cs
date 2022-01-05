using System;
using System.Data;
using System.Windows.Forms;

namespace DBFReader
{
    public partial class FindData : Form
    {
        public DataTable dtColumn, dtItems;
        public bool caseStatus = false;
        public bool wordStatus = true;
        public bool searchStatus = true;

        public FindData()
        {
            InitializeComponent();
        }

        private void FindData_Load(object sender, EventArgs e)
        {
            dtItems = new DataTable();
            dtItems.Columns.Add("ColumnName", typeof(string));
            dtItems.Columns.Add("ColumnId", typeof(int));
            for (int i = 0; i < dtColumn.Rows.Count; i++)
            {
                chkListBox.Items.Add(dtColumn.Rows[i]["NAME"].ToString().ToUpper(), true);
            }
            btnFind.Enabled = false;
            txtSearchFor.Select();
        }

        private void BtnCheck_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < chkListBox.Items.Count; i++)
            {
                chkListBox.SetItemChecked(i, true);
            }
        }

        private void BtnUncheck_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < chkListBox.Items.Count; i++)
            {
                chkListBox.SetItemChecked(i, false);
            }
        }

        private void BtnFind_Click(object sender, EventArgs e)
        {
            if (chkListBox.CheckedItems.Count > 0)
            {
                foreach (object item in chkListBox.CheckedItems)
                {
                    DataRow dr = dtItems.NewRow();
                    dr[0] = item.ToString();
                    dr[1] = chkListBox.Items.IndexOf(item);
                    dtItems.Rows.Add(dr);
                }
                this.Close();
            }
            else
            {
                ShowMessageBox.Show("Please Select Atleast One Field !", "Warning", enumMessageIcon.Warning);
            }
        }

        private void TxtSearchFor_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSearchFor.Text))
            {
                btnFind.Enabled = false;
            }
            else
            {
                btnFind.Enabled = true;
            }
        }

        private void ChkCase_CheckedChanged(object sender, EventArgs e)
        {
            if (chkCase.Checked == true)
            {
                caseStatus = true;
            }
            else
            {
                caseStatus = false;
            }
        }

        private void ChkWhole_CheckedChanged(object sender, EventArgs e)
        {
            if (chkWhole.Checked == true)
            {
                wordStatus = true;
            }
            else
            {
                wordStatus = false;
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            searchStatus = false;
            this.Close();
        }
    }
}
