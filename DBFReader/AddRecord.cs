using System;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace DBFReader
{
    public partial class AddRecord : Form
    {
        public DataTable dtColumn, dtData;
        public int maxUniqueId;
        public string filePath, fileName;
        public bool dbTableSts, addRecStatus = true;
        bool formClsSts = false;

        DateTimePicker oDateTimePicker;
        ComboBox oComboBox;

        public AddRecord()
        {
            InitializeComponent();
        }

        private void AddRecord_Load(object sender, EventArgs e)
        {
            DataRow dr = dtData.NewRow();
            dr[0] = maxUniqueId;
            for (int i = 1; i < dtColumn.Rows.Count; i++)
            {
                if (dtColumn.Rows[i]["TYPE"].ToString().ToLower() == "boolean")
                {
                    dr[i] = false;
                }
                else if (dtColumn.Rows[i]["TYPE"].ToString().ToLower() == "integer")
                {
                    dr[i] = 0;
                }
                else if (dtColumn.Rows[i]["TYPE"].ToString().ToLower() == "currency")
                {
                    dr[i] = 0.0000;
                }
                else if (dtColumn.Rows[i]["TYPE"].ToString().ToLower() == "numeric" || dtColumn.Rows[i]["TYPE"].ToString().ToLower() == "float")
                {
                    dr[i] = 0.00;
                }
                else if (dtColumn.Rows[i]["TYPE"].ToString().ToLower() == "decimal" || dtColumn.Rows[i]["TYPE"].ToString().ToLower() == "double")
                {
                    dr[i] = 0.00;
                }
                else if (dtColumn.Rows[i]["TYPE"].ToString().ToLower() == "datetime")
                {
                    dr[i] = DateTime.Now.ToString("dd/MMM/yyyy");
                }
                else
                {
                    dr[i] = "";
                }
            }
            dtData.Rows.Add(dr);
            dgvAddRec.DataSource = dtData;
            DataGridDesign();
        }

        private void DataGridDesign()
        {
            try
            {
                for (int i = 0; i < dtColumn.Rows.Count; i++)
                {
                    dgvAddRec.Columns[i].Name = dtColumn.Rows[i][0].ToString();
                    dgvAddRec.Columns[i].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    if (dtColumn.Rows[i]["TYPE"].ToString().ToLower() == "boolean" || dtColumn.Rows[i]["TYPE"].ToString().ToLower() == "logical")
                    {
                        dgvAddRec.Columns[i].Width = 70;
                        dgvAddRec.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    }
                    else if (dtColumn.Rows[i]["TYPE"].ToString().ToLower() == "numeric" || dtColumn.Rows[i]["TYPE"].ToString().ToLower() == "decimal" ||
                        dtColumn.Rows[i]["TYPE"].ToString().ToLower() == "integer")
                    {
                        dgvAddRec.Columns[i].Width = 75;
                        dgvAddRec.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    }
                    else if (dtColumn.Rows[i]["TYPE"].ToString().ToLower() == "float" || dtColumn.Rows[i]["TYPE"].ToString().ToLower() == "double")
                    {
                        dgvAddRec.Columns[i].Width = 75;
                        dgvAddRec.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                        dgvAddRec.Columns[i].DefaultCellStyle.Format = string.Format("0.00");
                    }
                    else if (dtColumn.Rows[i]["TYPE"].ToString().ToLower() == "currency")
                    {
                        dgvAddRec.Columns[i].Width = 90;
                        dgvAddRec.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                        dgvAddRec.Columns[i].DefaultCellStyle.Format = string.Format("0.0000");
                    }
                    else
                    {
                        dgvAddRec.Columns[i].Width = 100;
                        dgvAddRec.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                    }
                }
                dgvAddRec.AllowUserToAddRows = false;

                this.Text = "Add Records - " + dgvAddRec.Rows.Count;
            }
            catch (Exception ex)
            {
                Helper.WriteToFile("Error :" + ex.Message);
            }
        }

        private void DgvAddRec_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            for (int i = 0; i < dgvAddRec.Columns.Count; i++)
            {
                if (dgvAddRec.CurrentCell.ColumnIndex == i)
                {
                    if (dtColumn.Rows[i]["TYPE"].ToString() == "Numeric" || dtColumn.Rows[i]["TYPE"].ToString() == "Float" || dtColumn.Rows[i]["TYPE"].ToString() == "Currency")
                    {
                        e.Control.KeyPress -= new KeyPressEventHandler(NumericColumn_KeyPress);
                        TextBox txtNumber = e.Control as TextBox;
                        if (txtNumber != null)
                        {
                            txtNumber.KeyPress += new KeyPressEventHandler(NumericColumn_KeyPress);
                        }
                    }
                    else if (dtColumn.Rows[i]["TYPE"].ToString() == "Decimal" || dtColumn.Rows[i]["TYPE"].ToString() == "Double")
                    {
                        e.Control.KeyPress -= new KeyPressEventHandler(NumericColumn_KeyPress);
                        TextBox txtNumber = e.Control as TextBox;
                        if (txtNumber != null)
                        {
                            txtNumber.KeyPress += new KeyPressEventHandler(NumericColumn_KeyPress);
                        }
                    }
                    else if (dtColumn.Rows[i]["TYPE"].ToString() == "Integer")
                    {
                        e.Control.KeyPress -= new KeyPressEventHandler(NumberColumn_KeyPress);
                        TextBox txtNumber = e.Control as TextBox;
                        if (txtNumber != null)
                        {
                            txtNumber.KeyPress += new KeyPressEventHandler(NumberColumn_KeyPress);
                        }
                    }
                    else
                    {
                        e.Control.KeyPress -= new KeyPressEventHandler(CharacterColumn_KeyPress);
                        TextBox txtNumber = e.Control as TextBox;
                        if (txtNumber != null)
                        {
                            txtNumber.KeyPress += new KeyPressEventHandler(CharacterColumn_KeyPress);
                        }
                    }
                    break;
                }
            }
        }

        private void NumberColumn_KeyPress(object sender, KeyPressEventArgs e)
        {
            //allowed only numeric value  ex.10
            if (e.KeyChar >= 48 && e.KeyChar <= 57 || e.KeyChar == 8)
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void NumericColumn_KeyPress(object sender, KeyPressEventArgs e)
        {
            // allowed numeric and one dot  ex. 10.23
            if (e.KeyChar >= 48 && e.KeyChar <= 57 || e.KeyChar == 8 || e.KeyChar == 46)
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if (e.KeyChar == 46 && (sender as TextBox).Text.IndexOf('.') > -1)
            {
                e.Handled = true;
            }
        }

        private void CharacterColumn_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = false;
        }

        private void DateTimePicker_OnTextChange(object sender, EventArgs e)
        {
            dgvAddRec.CurrentCell.Value = oDateTimePicker.Text.ToString();
            oDateTimePicker.Visible = false;
        }

        private void DgvAddRec_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1)
            {
                if (oDateTimePicker != null)
                    oDateTimePicker.Visible = false;
                if (oComboBox != null)
                    oComboBox.Visible = false;
                if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
                {
                    // If any cell is clicked which is our date Column
                    if (dtColumn.Rows[e.ColumnIndex]["TYPE"].ToString().ToUpper() == "DATE" || dtColumn.Rows[e.ColumnIndex]["TYPE"].ToString().ToUpper() == "DATETIME")
                    {
                        oDateTimePicker = new DateTimePicker();
                        dgvAddRec.Controls.Add(oDateTimePicker);
                        oDateTimePicker.Visible = false;
                        oDateTimePicker.Format = DateTimePickerFormat.Short;
                        oDateTimePicker.TextChanged += new EventHandler(DateTimePicker_OnTextChange);
                        oDateTimePicker.Visible = true;
                        oDateTimePicker.BringToFront();
                        //It returns the retangular area that represents the Display area for a cell
                        Rectangle oRectangle = dgvAddRec.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true);
                        oDateTimePicker.Size = new Size(oRectangle.Width - 82, oRectangle.Height);
                        oDateTimePicker.Location = new Point(oRectangle.X + 82, oRectangle.Y);
                        oDateTimePicker.CloseUp += new EventHandler(DateTimePicker_CloseUp);
                    }
                    if (dtColumn.Rows[e.ColumnIndex]["TYPE"].ToString().ToUpper() == "LOGICAL" || dtColumn.Rows[e.ColumnIndex]["TYPE"].ToString().ToUpper() == "BOOLEAN")
                    {
                        dgvAddRec.Columns[e.ColumnIndex].ReadOnly = true;
                        string[] dataSource = { "FALSE", "TRUE" };
                        oComboBox = new ComboBox
                        {
                            DropDownStyle = ComboBoxStyle.DropDownList,
                            DataSource = dataSource.ToList()
                        };

                        dgvAddRec.Controls.Add(oComboBox);
                        oComboBox.Visible = false;
                        oComboBox.SelectedIndexChanged += new EventHandler(ComboBox_SelectedIndexChanged);
                        oComboBox.Visible = true;
                        oComboBox.SelectedIndex = 0;
                        //It returns the retangular area that represents the Display area for a cell
                        System.Drawing.Rectangle oRectangle = dgvAddRec.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true);
                        oComboBox.Size = new Size(oRectangle.Width, oRectangle.Height);
                        oComboBox.Location = new Point(oRectangle.X, oRectangle.Y);
                        oComboBox.DropDownClosed += new EventHandler(ComboBox_DropDownClosed);
                    }
                }
            }
        }

        private void DgvAddRec_KeyPress(object sender, KeyPressEventArgs e)
        {
            //if (e.KeyChar == 13 || e.KeyChar == 9)
            //{
            //    DataGridView ff = (DataGridView)sender;
            //    int RowIndex = ff.CurrentRow.Index;
            //    int ColumnIndex = ff.CurrentCell.ColumnIndex-1;
            //    if (RowIndex == dgvAddRec.Rows.Count - 1)
            //    {
            //        if (ColumnIndex == dgvAddRec.Columns.Count - 1)
            //        {
            //            if (IsGridColumnNull(RowIndex, dgvAddRec))
            //            {
            //                dgvAddRec.CurrentCell = dgvAddRec.Rows[RowIndex].Cells[0];
            //                return;
            //            }
            //            else
            //            {
            //                DataRow dr = dtData.NewRow();
            //                if (dtColumn.Rows[0]["TYPE"].ToString().ToLower() == "integer" || dtColumn.Rows[0]["TYPE"].ToString().ToLower() == "numeric" ||
            //                    dtColumn.Rows[0]["TYPE"].ToString().ToLower() == "decimal" || dtColumn.Rows[0]["TYPE"].ToString().ToLower() == "double" ||
            //                    dtColumn.Rows[0]["TYPE"].ToString().ToLower() == "float")
            //                {
            //                    dr[0] = int.Parse(dgvAddRec.Rows[RowIndex].Cells[0].FormattedValue.ToString()) + 1;
            //                }
            //                else
            //                {
            //                    dr[0] = "";
            //                }
            //                for (int i = 1; i < dtColumn.Rows.Count; i++)
            //                {
            //                    if (dtColumn.Rows[i]["TYPE"].ToString().ToLower() == "boolean" || dtColumn.Rows[i]["TYPE"].ToString().ToLower() == "logical")
            //                    {
            //                        dr[i] = false;
            //                    }
            //                    else if (dtColumn.Rows[i]["TYPE"].ToString().ToLower() == "integer")
            //                    {
            //                        dr[i] = 0;
            //                    }
            //                    else if (dtColumn.Rows[i]["TYPE"].ToString().ToLower() == "numeric" || dtColumn.Rows[i]["TYPE"].ToString().ToLower() == "float")
            //                    {
            //                        dr[i] = 0.00;
            //                    }
            //                    else if (dtColumn.Rows[i]["TYPE"].ToString().ToLower() == "decimal" || dtColumn.Rows[i]["TYPE"].ToString().ToLower() == "double")
            //                    {
            //                        dr[i] = 0.00;
            //                    }
            //                    else if (dtColumn.Rows[i]["TYPE"].ToString().ToLower() == "datetime" || dtColumn.Rows[i]["TYPE"].ToString().ToLower() == "date")
            //                    {
            //                        dr[i] = DateTime.Now.ToString("dd/MMM/yyyy");
            //                    }
            //                    else
            //                    {
            //                        dr[i] = "";
            //                    }
            //                }
            //                dtData.Rows.Add(dr);
            //                dgvAddRec.DataSource = dtData;
            //                dgvAddRec.CurrentCell = dgvAddRec.Rows[RowIndex + 1].Cells[0];
            //            }
            //        }
            //    }
            //}
        }

        private void DgvAddRec_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            //if (e.RowIndex == dgvAddRec.Rows.Count - 1)
            //{
            //    if (e.ColumnIndex == dgvAddRec.Columns.Count - 1)
            //    {
            //        if (IsGridColumnNull(e.RowIndex, dgvAddRec))
            //        {
            //            dgvAddRec.CurrentCell = dgvAddRec.Rows[e.RowIndex].Cells[0];
            //            return;
            //        }
            //        else
            //        {
            //            DataRow dr = dtData.NewRow();
            //            if (dtColumn.Rows[0]["TYPE"].ToString().ToLower() == "integer" || dtColumn.Rows[0]["TYPE"].ToString().ToLower() == "numeric" ||
            //                dtColumn.Rows[0]["TYPE"].ToString().ToLower() == "decimal" || dtColumn.Rows[0]["TYPE"].ToString().ToLower() == "double" ||
            //                dtColumn.Rows[0]["TYPE"].ToString().ToLower() == "float")
            //            {
            //                dr[0] = int.Parse(dgvAddRec.Rows[e.RowIndex].Cells[0].FormattedValue.ToString()) + 1;
            //            }
            //            else
            //            {
            //                dr[0] = "";
            //            }
            //            for (int i = 1; i < dtColumn.Rows.Count; i++)
            //            {
            //                if (dtColumn.Rows[i]["TYPE"].ToString().ToLower() == "boolean" || dtColumn.Rows[i]["TYPE"].ToString().ToLower() == "logical")
            //                {
            //                    dr[i] = false;
            //                }
            //                else if (dtColumn.Rows[i]["TYPE"].ToString().ToLower() == "integer")
            //                {
            //                    dr[i] = 0;
            //                }
            //                else if (dtColumn.Rows[i]["TYPE"].ToString().ToLower() == "numeric" || dtColumn.Rows[i]["TYPE"].ToString().ToLower() == "float")
            //                {
            //                    dr[i] = 0.00;
            //                }
            //                else if (dtColumn.Rows[i]["TYPE"].ToString().ToLower() == "decimal" || dtColumn.Rows[i]["TYPE"].ToString().ToLower() == "double")
            //                {
            //                    dr[i] = 0.00;
            //                }
            //                else if (dtColumn.Rows[i]["TYPE"].ToString().ToLower() == "datetime" || dtColumn.Rows[i]["TYPE"].ToString().ToLower() == "date")
            //                {
            //                    dr[i] = DateTime.Now.ToString("dd/MMM/yyyy");
            //                }
            //                else
            //                {
            //                    dr[i] = "";
            //                }
            //            }
            //            dtData.Rows.Add(dr);
            //            dgvAddRec.DataSource = dtData;
            //            dgvAddRec.CurrentCell = dgvAddRec.Rows[e.RowIndex + 1].Cells[0];
            //        }
            //    }
            //}
        }

        private void DgvAddRec_RowValidating(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (e.RowIndex < dgvAddRec.Rows.Count - 1)
            {
                for (int i = 0; i < dgvAddRec.Columns.Count; i++)
                {
                    if (string.IsNullOrEmpty(dgvAddRec.Rows[e.RowIndex].Cells[i].FormattedValue.ToString()))
                    {
                        ShowMessageBox.Show("Field " + dgvAddRec.Columns[i].HeaderText + " cann't be empty");
                        return;
                    }
                }
            }
        }

        private void DgvAddRec_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            if (oDateTimePicker != null)
                oDateTimePicker.Visible = false;
            if (oComboBox != null)
                oComboBox.Visible = false;
        }

        private void DgvAddRec_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
            if (oDateTimePicker != null)
                oDateTimePicker.Visible = false;
            if (oComboBox != null)
                oComboBox.Visible = false;
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            if (IsGridRowNull())
            {
                ShowMessageBox.Show(dgvAddRec.Columns[0].HeaderText + " shouldn't be blank.", "Error", enumMessageIcon.Error);
            }
            else
            {
                if (bgwAddRec.IsBusy != true)
                {
                    progressBar1.Visible = true; ;
                    progressBar1.Maximum = dgvAddRec.Rows.Count;
                    progressBar1.Minimum = 0;
                    btnAdd.Enabled = false;
                    btnImport.Enabled = false;
                    dgvAddRec.ReadOnly = true;
                    bgwAddRec.RunWorkerAsync();
                }
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            addRecStatus = false;
            this.Close();
        }

        private void BtnImport_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Select file",
                InitialDirectory = @"D:\",
                Filter = "CSV Files(*.csv)|*.csv|Excel Files(*.xls)|*.xls;*.xlsx",
                FilterIndex = 1,
                RestoreDirectory = true
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (Path.GetExtension(openFileDialog.FileName).ToLower() == ".csv" || Path.GetExtension(openFileDialog.FileName).ToLower() == ".xls" ||
                    Path.GetExtension(openFileDialog.FileName).ToLower() == ".xlsx")
                {
                    if (openFileDialog.FileName.Trim() != string.Empty)
                    {
                        if (Path.GetExtension(openFileDialog.FileName).ToLower() == ".csv")
                        {
                            try
                            {
                                DataTable dtCsv = GetDataFromCsv(openFileDialog.FileName);
                                if (dtCsv.Columns.Count == dtData.Columns.Count)
                                {
                                    dtData = dtCsv.Copy();
                                    dtData.AcceptChanges();
                                    dgvAddRec.DataSource = dtData;
                                    DataGridDesign();
                                    progressBar1.Visible = false;
                                }
                                else
                                {
                                    GetBlankRow();
                                    dgvAddRec.DataSource = dtData;
                                    DataGridDesign();
                                    progressBar1.Visible = false;
                                    ShowMessageBox.Show("No of column mismatch", "Error", enumMessageIcon.Error);
                                }
                            }
                            catch (Exception ex)
                            {
                                ShowMessageBox.Show(ex.Message.ToString(), "Error", enumMessageIcon.Error);
                            }
                        }
                        else
                        {
                            try
                            {
                                DataTable dtExcel = GetDataFromExcel(openFileDialog.FileName);
                                if (dtExcel.Columns.Count == dtData.Columns.Count)
                                {
                                    dtData = dtExcel.Copy();
                                    dtData.AcceptChanges();
                                    dgvAddRec.DataSource = dtData;
                                    DataGridDesign();
                                    progressBar1.Visible = false;
                                }
                                else
                                {
                                    GetBlankRow();
                                    dgvAddRec.DataSource = dtData;
                                    DataGridDesign();
                                    progressBar1.Visible = false;
                                    ShowMessageBox.Show("No of column mismatch", "Error", enumMessageIcon.Error);
                                }
                            }
                            catch (Exception ex)
                            {
                                ShowMessageBox.Show(ex.Message.ToString(), "Error", enumMessageIcon.Error);
                            }
                        }
                    }
                }
                else
                {
                    ShowMessageBox.Show("File format not supported !!!", "Info");
                }
            }
        }

        private void BgwAddRec_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                int cntStart = 0, numRes = 0;
                if (dbTableSts == true)
                {
                    cntStart = 1;
                }
                OleDbConnection oleDbConnection = new OleDbConnection("Provider=VFPOLEDB.1;Data Source=" + filePath);
                oleDbConnection.Open();
                for (int n = 0; n < dgvAddRec.Rows.Count; n++)
                {
                    if (bgwAddRec.CancellationPending)
                    {
                        e.Cancel = true;
                        return;
                    }
                    bgwAddRec.ReportProgress(n);
                    string columnNames = "";
                    string columnValues = "";
                    try
                    {
                        for (int i = cntStart; i < dtColumn.Rows.Count; i++)
                        {
                            if (i == dtColumn.Rows.Count - 1)
                            {
                                columnNames = columnNames + dtColumn.Rows[i][0].ToString();

                                if (dtColumn.Rows[i]["TYPE"].ToString().ToLower() == "boolean" || dtColumn.Rows[i]["TYPE"].ToString().ToLower() == "logical")
                                {
                                    columnValues = columnValues + "." + dgvAddRec.Rows[n].Cells[i].Value.ToString().Substring(0, 1) + ".";
                                }
                                else if (dtColumn.Rows[i]["TYPE"].ToString().ToLower() == "string")
                                {
                                    columnValues = columnValues + "'" + dgvAddRec.Rows[n].Cells[i].Value.ToString() + "'";
                                }
                                else if (dtColumn.Rows[i]["TYPE"].ToString().ToLower() == "character")
                                {
                                    columnValues = columnValues + "'" + dgvAddRec.Rows[n].Cells[i].Value.ToString() + "'";
                                }
                                else if (dtColumn.Rows[i]["TYPE"].ToString().ToLower() == "datetime" || dtColumn.Rows[i]["TYPE"].ToString().ToLower() == "date")
                                {
                                    columnValues = columnValues + "{d'" + DateTime.Parse(dgvAddRec.Rows[n].Cells[i].Value.ToString()).ToString("yyyy-MM-dd") + "'}";
                                }
                                else
                                {
                                    columnValues = columnValues + dgvAddRec.Rows[n].Cells[i].Value.ToString();
                                }
                            }
                            else
                            {
                                columnNames = columnNames + dtColumn.Rows[i][0].ToString() + ",";

                                if (dtColumn.Rows[i]["TYPE"].ToString().ToLower() == "boolean")
                                {
                                    columnValues = columnValues + "." + dgvAddRec.Rows[n].Cells[i].Value.ToString().Substring(0, 1) + ".,";
                                }
                                else if (dtColumn.Rows[i]["TYPE"].ToString().ToLower() == "string")
                                {
                                    columnValues = columnValues + "'" + dgvAddRec.Rows[n].Cells[i].Value.ToString() + "',";
                                }
                                else if (dtColumn.Rows[i]["TYPE"].ToString().ToLower() == "character")
                                {
                                    columnValues = columnValues + "'" + dgvAddRec.Rows[n].Cells[i].Value.ToString() + "',";
                                }
                                else if (dtColumn.Rows[i]["TYPE"].ToString().ToLower() == "datetime")
                                {
                                    columnValues = columnValues + "{d'" + DateTime.Parse(dgvAddRec.Rows[n].Cells[i].Value.ToString()).ToString("yyyy-MM-dd") + "'},";
                                }
                                else
                                {
                                    columnValues = columnValues + dgvAddRec.Rows[n].Cells[i].Value.ToString() + ",";
                                }
                            }
                        }

                        string sql = "Insert into " + fileName + " (" + columnNames + ") Values (" + columnValues + ")";
                        if (oleDbConnection.State == ConnectionState.Closed)
                            oleDbConnection.Open();
                        OleDbCommand oleDbCommand = new OleDbCommand(sql, oleDbConnection);
                        numRes = numRes + oleDbCommand.ExecuteNonQuery();
                    }
                    catch (Exception) { }
                }
                oleDbConnection.Close();
                if (numRes >= 1)
                {
                    formClsSts = true;
                    ShowMessageBox.Show(numRes.ToString() + " nos of record saved !", "Info", enumMessageIcon.Information);
                }
                else
                {
                    ShowMessageBox.Show("Please Try Again !", "Error", enumMessageIcon.Error);
                }
            }
            catch (Exception ex)
            {
                ShowMessageBox.Show("Error :- " + ex.Message, "Error", enumMessageIcon.Error);
            }
        }

        private void BgwAddRec_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
            decimal decProgress = (decimal.Parse(e.ProgressPercentage.ToString()) / decimal.Parse(dgvAddRec.Rows.Count.ToString())) * 100;
            this.Text = "Add Records - (" + e.ProgressPercentage + "/" + dgvAddRec.Rows.Count + ") " + decProgress.ToString("0") + "%";
        }

        private void BgwAddRec_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled) ShowMessageBox.Show("Operation was canceled", "Info", enumMessageIcon.Information);
            progressBar1.Visible = false;
            btnAdd.Enabled = true;
            btnImport.Enabled = true;
            if (formClsSts == true)
            {
                this.Close();
            }
        }

        private void AddRecord_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (bgwAddRec.IsBusy)
            {
                DialogResult dr = MessageBox.Show("Background process is running. Do ypu want to cancel it ?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (dr == DialogResult.Yes)
                {
                    if (bgwAddRec.WorkerSupportsCancellation == true)
                    {
                        bgwAddRec.CancelAsync();
                    }
                }
            }
        }

        void DateTimePicker_CloseUp(object sender, EventArgs e)
        {
            oDateTimePicker.Visible = false;
        }

        private void ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            dgvAddRec.CurrentCell.Value = oComboBox.Text.ToString();
            oComboBox.Visible = false;
        }

        void ComboBox_DropDownClosed(object sender, EventArgs e)
        {
            oComboBox.Visible = false;
        }

        public static DataTable GetDataFromCsv(string strFileName)
        {
            string[] str = File.ReadAllLines(strFileName);
            DataTable dt = new DataTable();
            string[] temp = str[0].Split(',');
            foreach (string t in temp)
            {
                dt.Columns.Add(t, typeof(string));
            }
            for (int i = 1; i < str.Length; i++)
            {
                string[] t = str[i].Split(',');
                dt.Rows.Add(t);
            }

            return dt;
        }

        public DataTable GetData(string strFileName)
        {
            string[] str = File.ReadAllLines(strFileName);
            DataTable dt = new DataTable();
            string[] temp = str[0].Split(',');
            foreach (string t in temp)
            {
                string tempstr = t;
                tempstr = tempstr.Trim('\"');
                dt.Columns.Add(tempstr, typeof(string));
            }
            for (int i = 1; i < str.Length; i++)
            {
                string[] t = str[i].Split(',');
                for (int j = 0; j < t.Length; j++)
                {
                    t[j] = t[j].Trim('\"');
                }
                dt.Rows.Add(t);
            }
            return dt;
        }

        public DataTable GetDataFromExcel(string strFileName)
        {
            DataTable dt = new DataTable();
            OleDbConnection oleDbConnection = new OleDbConnection(@"provider=Microsoft.ACE.OLEDB.12.0;Data Source='" + strFileName + "';Extended Properties=Excel 12.0;");
            OleDbDataAdapter oleDbDataAdapter = new OleDbDataAdapter("select * from [Sheet1$]", oleDbConnection);
            oleDbDataAdapter.Fill(dt);
            oleDbConnection.Close();
            return dt;
        }

        public void GetBlankRow()
        {
            dtData.Rows.Clear();
            DataRow dr = dtData.NewRow();
            dr[0] = maxUniqueId;
            for (int i = 1; i < dtColumn.Rows.Count; i++)
            {
                if (dtColumn.Rows[i]["TYPE"].ToString().ToLower() == "boolean")
                {
                    dr[i] = false;
                }
                else if (dtColumn.Rows[i]["TYPE"].ToString().ToLower() == "integer")
                {
                    dr[i] = 0;
                }
                else if (dtColumn.Rows[i][""].ToString().ToLower() == "currency")
                {
                    dr[i] = 0.0000;
                }
                else if (dtColumn.Rows[i]["TYPE"].ToString().ToLower() == "numeric" || dtColumn.Rows[i]["TYPE"].ToString().ToLower() == "float")
                {
                    dr[i] = 0.00;
                }
                else if (dtColumn.Rows[i]["TYPE"].ToString().ToLower() == "decimal" || dtColumn.Rows[i]["TYPE"].ToString().ToLower() == "double")
                {
                    dr[i] = 0.00;
                }
                else if (dtColumn.Rows[i]["TYPE"].ToString().ToLower() == "datetime")
                {
                    dr[i] = DateTime.Now.ToString("dd/MMM/yyyy");
                }
                else
                {
                    dr[i] = "";
                }
            }
            dtData.Rows.Add(dr);
        }

        private bool IsGridColumnNull(int RowIndex, DataGridView dgvGridView)
        {
            bool ReturnVal = false;
            for (int i = 0; i < dgvGridView.Columns.Count; i++)
            {
                if (string.IsNullOrEmpty(dgvGridView.Rows[RowIndex].Cells[i].Value.ToString()))
                {
                    ReturnVal = true;
                    break;
                }
            }
            return ReturnVal;
        }

        private bool IsGridRowNull()
        {
            bool ReturnVal = false;
            for (int n = 0; n < dgvAddRec.Rows.Count; n++)
            {
                if (string.IsNullOrEmpty(dgvAddRec.Rows[n].Cells[0].Value.ToString()))
                {
                    ReturnVal = true;
                    break;
                }
            }
            return ReturnVal;
        }

        public DataTable CompareMerge(DataTable dtOne)
        {
            DataTable dtReturn = dtData.Clone();

            DataRow drRow = dtReturn.NewRow();
            drRow[0] = maxUniqueId;
            for (int i = 1; i < dtColumn.Rows.Count; i++)
            {
                if (dtColumn.Rows[i]["TYPE"].ToString().ToLower() == "boolean")
                {
                    drRow[i] = false;
                }
                else if (dtColumn.Rows[i]["TYPE"].ToString().ToLower() == "integer")
                {
                    drRow[i] = int.Parse(dtOne.Rows[0][i].ToString());
                }
                else if (dtColumn.Rows[i]["TYPE"].ToString().ToLower() == "currency")
                {
                    drRow[i] = 0.0000;
                }
                else if (dtColumn.Rows[i]["TYPE"].ToString().ToLower() == "numeric" || dtColumn.Rows[i]["TYPE"].ToString().ToLower() == "float")
                {
                    drRow[i] = 0.00;
                }
                else if (dtColumn.Rows[i]["TYPE"].ToString().ToLower() == "decimal" || dtColumn.Rows[i]["TYPE"].ToString().ToLower() == "double")
                {
                    drRow[i] = 0.00;
                }
                else if (dtColumn.Rows[i]["TYPE"].ToString().ToLower() == "datetime")
                {
                    drRow[i] = DateTime.Now.ToString("dd/MMM/yyyy");
                }
                else
                {
                    drRow[i] = "";
                }
            }
            dtReturn.Rows.Add(drRow);

            return dtReturn;
        }
    }
}
