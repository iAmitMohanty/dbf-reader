using DBFConverter;
using System;
using System.Data;
using System.Windows.Forms;

namespace DBFReader
{
    public partial class NewTable : Form
    {
        public DataTable dtColumn;
        public string fileName;
        public DbfVersion newFileVersion;

        public NewTable()
        {
            InitializeComponent();
        }

        private void NewTable_Load(object sender, EventArgs e)
        {
        }

        private DataTable CreateColumnTable()
        {
            DataTable dtNew = new DataTable();
            dtNew.Columns.Add("NAME", typeof(string));
            dtNew.Columns.Add("TYPE", typeof(string));
            dtNew.Columns.Add("LENGTH", typeof(string));
            dtNew.Columns.Add("DEC", typeof(string));
            return dtNew;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvTableNew.Rows.Count > 1)
                {
                    SaveFileDialog saveFileDialog = new SaveFileDialog
                    {
                        InitialDirectory = @"D:\",
                        Title = "Save DBF Files",
                        DefaultExt = "dbf",
                        Filter = "DBF Files(*.dbf)|*.dbf",
                        FilterIndex = 2,
                        RestoreDirectory = true
                    };
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        fileName = saveFileDialog.FileName;
                        GetNewFileVersion();
                        dtColumn = CreateColumnTable();
                        for (int i = 0; i < dgvTableNew.Rows.Count - 2; i++)
                        {
                            DataRow drCol = dtColumn.NewRow();
                            drCol["NAME"] = dgvTableNew.Rows[i].Cells[0].Value.ToString();
                            if (string.IsNullOrEmpty(dgvTableNew.Rows[i].Cells[1].FormattedValue.ToString()))
                            {
                                drCol["TYPE"] = "Character";
                            }
                            else
                            {
                                drCol["TYPE"] = dgvTableNew.Rows[i].Cells[1].Value.ToString();
                            }
                            if (string.IsNullOrEmpty(dgvTableNew.Rows[i].Cells[2].FormattedValue.ToString()))
                            {
                                drCol["LENGTH"] = "50";
                            }
                            else
                            {
                                drCol["LENGTH"] = dgvTableNew.Rows[i].Cells[2].Value.ToString();
                            }
                            drCol["DEC"] = "0";
                            dtColumn.Rows.Add(drCol);
                        }
                        this.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessageBox.Show("Error : - " + ex.Message, "Error", enumMessageIcon.Error);
            }
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void GetNewFileVersion()
        {
            if (rbtndBaseIII.Checked == true)
            {
                newFileVersion = DbfVersion.FoxBaseDBase3NoMemo;
            }
            else if (rbtndBaseIV.Checked == true)
            {
                newFileVersion = DbfVersion.dBase4SQLTableNoMemo;
            }
            else if (rbtnFoxpro2x.Checked == true)
            {
                newFileVersion = DbfVersion.FoxBase;
            }
            else if (rbtnVisualFoxpro.Checked == true)
            {
                newFileVersion = DbfVersion.VisualFoxPro;
            }
            else
            {
                newFileVersion = DbfVersion.VisualFoxProWithAutoIncrement;
            }
        }

        private void DgvTableNew_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            e.Control.KeyPress -= new KeyPressEventHandler(NumberColumn_KeyPress);
            if (dgvTableNew.CurrentCell.ColumnIndex == 2) //Desired Column
            {
                TextBox txtNumber = e.Control as TextBox;
                if (txtNumber != null)
                {
                    txtNumber.KeyPress += new KeyPressEventHandler(NumberColumn_KeyPress);
                }
            }
        }

        private void NumberColumn_KeyPress(object sender, KeyPressEventArgs e)
        {
            //allowed only numeric value  ex.10
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void DgvTableNew_RowValidating(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (e.RowIndex < dgvTableNew.Rows.Count - 1)
            {
                if (string.IsNullOrEmpty(dgvTableNew.Rows[e.RowIndex].Cells[0].FormattedValue.ToString()))
                {
                    ShowMessageBox.Show("Field Name cann't be empty");
                    return;
                }
                if (string.IsNullOrEmpty(dgvTableNew.Rows[e.RowIndex].Cells[1].FormattedValue.ToString()))
                {
                    ShowMessageBox.Show("Field Type cann't be empty");
                    return;
                }
                if (string.IsNullOrEmpty(dgvTableNew.Rows[e.RowIndex].Cells[2].FormattedValue.ToString()))
                {
                    ShowMessageBox.Show("Field Length cann't be empty");
                    return;
                }
            }
        }

        private void DgvTableNew_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvTableNew.CurrentCell.ColumnIndex == 0)
            {
                dgvTableNew.Rows[e.RowIndex].Cells[1].Value = "Character";
                dgvTableNew.Rows[e.RowIndex].Cells[2].ReadOnly = false;
                dgvTableNew.Rows[e.RowIndex].Cells[2].Value = 20;
            }
            if (dgvTableNew.CurrentCell.ColumnIndex == 1) //Desired Column
            {
                if (dgvTableNew.Rows[e.RowIndex].Cells[1].Value.ToString() == "Date")
                {
                    dgvTableNew.Rows[e.RowIndex].Cells[2].ReadOnly = true;
                    dgvTableNew.Rows[e.RowIndex].Cells[2].Value = 8;
                }
                if (dgvTableNew.Rows[e.RowIndex].Cells[1].Value.ToString() == "Logical")
                {
                    dgvTableNew.Rows[e.RowIndex].Cells[2].ReadOnly = true;
                    dgvTableNew.Rows[e.RowIndex].Cells[2].Value = 1;
                }
                if (dgvTableNew.Rows[e.RowIndex].Cells[1].Value.ToString() == "Numeric")
                {
                    dgvTableNew.Rows[e.RowIndex].Cells[2].ReadOnly = true;
                    dgvTableNew.Rows[e.RowIndex].Cells[2].Value = 20;
                }
                if (dgvTableNew.Rows[e.RowIndex].Cells[1].Value.ToString() == "Integer")
                {
                    dgvTableNew.Rows[e.RowIndex].Cells[2].ReadOnly = true;
                    dgvTableNew.Rows[e.RowIndex].Cells[2].Value = 10;
                }
                if (dgvTableNew.Rows[e.RowIndex].Cells[1].Value.ToString() == "Float")
                {
                    dgvTableNew.Rows[e.RowIndex].Cells[2].ReadOnly = true;
                    dgvTableNew.Rows[e.RowIndex].Cells[2].Value = 20;
                }
                if (dgvTableNew.Rows[e.RowIndex].Cells[1].Value.ToString() == "Character")
                {
                    dgvTableNew.Rows[e.RowIndex].Cells[2].ReadOnly = false;
                    dgvTableNew.Rows[e.RowIndex].Cells[2].Value = 20;
                }
            }
            if (int.Parse(dgvTableNew.Rows[e.RowIndex].Cells[2].Value.ToString()) >= 1024)
            {
                dgvTableNew.Rows[e.RowIndex].Cells[2].Value = 1024;
            }
        }
    }
}
