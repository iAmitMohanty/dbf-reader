using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Data.OleDb;
using System.Drawing;
using Excel = Microsoft.Office.Interop.Excel;
using DBFConverter;
using AutoUpdate;
using System.Reflection;

namespace DBFReader
{
    public partial class ReaderMaster : Form
    {
        OpenFileDialog openFileDialog = new OpenFileDialog();
        private Dbf dbf;
        List<DbfRecord> listNewRecord;
        List<DbfField> listNewField;
        DbfVersion fileVersion;
        DataTable dtData, dtColumn, dtItems, dtCsvExport, dtPdfExport, dtExcelExport;
        string fileSize = "", newFileName = "", outputFileName = "";
        bool pdfExport, excelExport, csvExport, dataStatus, dbTableSts, bgReaderSts;
        string filePath, fileName, varSearch, orgFilePath;
        string[] outputCsv;
        int nxtRecNo;
        public int currRecNo, totRecNo;
        public bool caseSensetiveSts = false;
        public bool wholeWordSts = true;
        public bool fndRedord = false;

        DateTimePicker oDateTimePicker;
        ComboBox oComboBox;
        Loader loader = null;

        public ReaderMaster()
        {
            InitializeComponent();
        }

        private void ReaderMaster_Load(object sender, EventArgs e)
        {
            AutoUpdater.Start(Helper.checkPath);
            saveToolStripMenuItem.Enabled = false;
            findToolStripMenuItem.Enabled = false;
            findNextToolStripMenuItem.Enabled = false;
            replaceToolStripMenuItem.Enabled = false;
            gotoToolStripMenuItem.Enabled = false;
            skipToolStripMenuItem.Enabled = false;
            appendToolStripMenuItem.Enabled = false;
            deleteToolStripMenuItem.Enabled = false;
            deleteAllToolStripMenuItem.Enabled = false;

            // Double buffering can make DataGridView slow in remote desktop
            if (!System.Windows.Forms.SystemInformation.TerminalServerSession)
            {
                Type type = dgvReader.GetType();
                PropertyInfo propertyInfo = type.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
                propertyInfo.SetValue(dgvReader, true, null);
            }
        }

        private void NewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewTable objFrm = new NewTable();
            objFrm.ShowDialog();
            dtColumn = objFrm.dtColumn;
            if (dtColumn == null)
            {
            }
            else if (dtColumn.Rows.Count > 0)
            {
                newFileName = objFrm.fileName;
                fileVersion = objFrm.newFileVersion;
                tsslFileName.Text = "File : New Dbf File";
                DataTable dtNew = CreateNewTable(dtColumn);
                dgvReader.DataSource = dtNew;
                dgvReader.AllowUserToAddRows = true;
                dgvReader.SelectionMode = DataGridViewSelectionMode.CellSelect;
                dataStatus = true;
                saveToolStripMenuItem.Enabled = true;
                findToolStripMenuItem.Enabled = true;
                findNextToolStripMenuItem.Enabled = true;
                replaceToolStripMenuItem.Enabled = true;
                gotoToolStripMenuItem.Enabled = true;
                skipToolStripMenuItem.Enabled = true;
            }
            else
            {
                ShowMessageBox.Show("No Table Created !!!");
            }
        }

        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog.Title = "Select file";
            //openFileDialog.InitialDirectory = @"D:\";
            openFileDialog.Filter = "DBF Files(*.dbf)|*.dbf|All Files(*.*)|*.*";
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (Path.GetExtension(openFileDialog.FileName).ToLower() == ".dbf")
                {
                    dtData = null; dtColumn = null;
                    dgvReader.AllowUserToAddRows = false;
                    tsslTotRecord.Text = "Total Record : 0";
                    tsslFileSize.Text = "0 KB";
                    dgvReader.DataSource = null;
                    orgFilePath = openFileDialog.FileName;
                    tsslFileName.Text = "File : " + Path.GetFileNameWithoutExtension(orgFilePath).ToUpper();
                    long fSize = new FileInfo(orgFilePath).Length;
                    fileSize = GetFileSize(fSize);
                    dataStatus = false;
                    filePath = Path.GetDirectoryName(orgFilePath);
                    fileName = Path.GetFileNameWithoutExtension(orgFilePath);
                    bwReader.RunWorkerAsync(orgFilePath);
                    loader = new Loader("Please Wait. Data is Loading...");
                    loader.ShowDialog();
                }
                else
                {
                    ShowMessageBox.Show("Choose DBF File !!!", "Info");
                }
            }
        }

        private void ClearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dtData = null; dtColumn = null;
            progressBar1.Visible = false;
            dgvReader.DataSource = null;
            tsslTotRecord.Text = "Total Record : 0";
            tsslFileSize.Text = "0 KB";
            filePath = ""; fileName = ""; varSearch = ""; orgFilePath = "";
            nxtRecNo = 0;
            currRecNo = 0;
            totRecNo = 0;
            tsslFileName.Text = "File : ";
            if (oDateTimePicker != null)
                oDateTimePicker.Visible = false;
            if (oComboBox != null)
                oComboBox.Visible = false;
            saveToolStripMenuItem.Enabled = false;
            findToolStripMenuItem.Enabled = false;
            findNextToolStripMenuItem.Enabled = false;
            replaceToolStripMenuItem.Enabled = false;
            gotoToolStripMenuItem.Enabled = false;
            skipToolStripMenuItem.Enabled = false;
            appendToolStripMenuItem.Enabled = false;
            deleteToolStripMenuItem.Enabled = false;
            deleteAllToolStripMenuItem.Enabled = false;
        }

        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataStatus == true)
            {
                WriteNewData();
            }
            else
            {
                //ShowMessageBox.Show(dbf.Write(orgFilePath, listNewRecord, listNewField, fileVersion));
            }
        }

        private void ExcelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dgvReader.Rows.Count > 0)
            {
                dtExcelExport = (DataTable)dgvReader.DataSource;
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "Excel (.xlsx)|  *.xlsx",
                    FileName = "Output.xlsx"
                };
                bool fileError = false;
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    outputFileName = saveFileDialog.FileName;
                    if (File.Exists(outputFileName))
                    {
                        try
                        {
                            File.Delete(outputFileName);
                        }
                        catch (IOException ex)
                        {
                            fileError = true;
                            ShowMessageBox.Show("It wasn't possible to write the data to the disk." + ex.Message);
                        }
                    }
                    if (!fileError)
                    {
                        tsProgressBar.Visible = true;
                        tsslProgPer.Visible = true;
                        csvToolStripMenuItem.Enabled = false;
                        excelToolStripMenuItem.Enabled = false;
                        pdfToolStripMenuItem.Enabled = false;
                        bwExcel.RunWorkerAsync();
                    }
                }
            }
            else
            {
                ShowMessageBox.Show("No Record To Export !!!", "Info");
            }
        }

        private void PdfToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dgvReader.Rows.Count > 0)
            {
                dtPdfExport = (DataTable)dgvReader.DataSource;
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "PDF (*.pdf)|*.pdf",
                    FileName = "Output.pdf"
                };
                bool fileError = false;
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    outputFileName = saveFileDialog.FileName;
                    if (File.Exists(outputFileName))
                    {
                        try
                        {
                            File.Delete(outputFileName);
                        }
                        catch (IOException ex)
                        {
                            fileError = true;
                            ShowMessageBox.Show("It wasn't possible to write the data to the disk." + ex.Message);
                        }
                    }
                    if (!fileError)
                    {
                        tsProgressBar.Visible = true;
                        tsslProgPer.Visible = true;
                        csvToolStripMenuItem.Enabled = false;
                        excelToolStripMenuItem.Enabled = false;
                        pdfToolStripMenuItem.Enabled = false;
                        bwPdf.RunWorkerAsync();
                    }
                }
            }
            else
            {
                ShowMessageBox.Show("No Record To Export !!!", "Info");
            }
        }

        private void CsvToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dgvReader.Rows.Count > 0)
            {
                dtCsvExport = (DataTable)dgvReader.DataSource;
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "CSV (*.csv)|*.csv",
                    FileName = "Output.csv"
                };
                bool fileError = false;
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    outputFileName = saveFileDialog.FileName;
                    if (File.Exists(outputFileName))
                    {
                        try
                        {
                            File.Delete(outputFileName);
                        }
                        catch (IOException ex)
                        {
                            fileError = true;
                            ShowMessageBox.Show("It wasn't possible to write the data to the disk." + ex.Message);
                        }
                    }
                    if (!fileError)
                    {
                        tsProgressBar.Visible = true;
                        tsslProgPer.Visible = true;
                        csvToolStripMenuItem.Enabled = false;
                        excelToolStripMenuItem.Enabled = false;
                        pdfToolStripMenuItem.Enabled = false;
                        bwCsv.RunWorkerAsync();
                    }
                }
            }
            else
            {
                ShowMessageBox.Show("No Record To Export !!!", "Info");
            }
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void FindToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FindData findData = new FindData
            {
                dtColumn = dtColumn
            };
            findData.ShowDialog();
            dtItems = findData.dtItems;
            varSearch = findData.txtSearchFor.Text;
            caseSensetiveSts = findData.caseStatus;
            wholeWordSts = findData.wordStatus;            
            if (findData.searchStatus)
            {
                bwFind.RunWorkerAsync();
                loader = new Loader("Please Wait. Searching for '" + varSearch + "'...");
                loader.ShowDialog();
            }
        }

        private void FindNextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(varSearch))
            {
                if (dtItems != null && dtItems.Rows.Count > 0)
                {
                    bwFindNxt.RunWorkerAsync();
                    loader = new Loader("Please Wait. Searching for next '" + varSearch + "'...");
                    loader.ShowDialog();
                }
            }
        }

        private void GotoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GoTo goTo = new GoTo
            {
                currRecNo = currRecNo,
                totRecNo = totRecNo
            };
            goTo.ShowDialog();
            currRecNo = goTo.currRecNo;
            if (currRecNo <= dgvReader.Rows.Count)
            {
                DataGridCalculation(currRecNo);
                dgvReader.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgvReader.CurrentCell = dgvReader.Rows[currRecNo - 1].Cells[0];
            }
            else
            {
                ShowMessageBox.Show("Record Number '" + currRecNo + "' Not Found !");
            }
        }

        private void SkipToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Skip skip = new Skip();
            skip.ShowDialog();
            currRecNo += skip.skipRecNo;
            while (currRecNo > totRecNo)
            {
                currRecNo -= totRecNo;
            }
            DataGridCalculation(currRecNo);
            dgvReader.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvReader.CurrentCell = dgvReader.Rows[currRecNo - 1].Cells[0];
        }

        private void RefreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(orgFilePath))
                if (!bwReader.IsBusy)
                    bwReader.RunWorkerAsync(orgFilePath);

            loader = new Loader("Please Wait. Refreshing the Data...");
            loader.ShowDialog();
        }

        private void AppendToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddRecord addRecord = new AddRecord
            {
                dtColumn = dtColumn,
                dbTableSts = dbTableSts,
                dtData = CreateNewTable(dtColumn),
                maxUniqueId = GetMaxUniqueId(),
                fileName = fileName,
                filePath = filePath
            };
            addRecord.ShowDialog();
            if (addRecord.addRecStatus == true)
                bwReader.RunWorkerAsync(orgFilePath);
        }

        private void DeleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("Do you want to delete the record " + dgvReader.SelectedRows[0].Cells[0].Value.ToString() + " ? ",
                "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dr == DialogResult.Yes)
            {
                string uniqueColumn = dgvReader.Columns[0].HeaderText;
                string uniqueColumnVal;
                if (dtColumn.Rows[0]["TYPE"].ToString().ToLower() == "boolean" || dtColumn.Rows[0]["TYPE"].ToString().ToLower() == "logical")
                {
                    uniqueColumnVal = "=." + dgvReader.SelectedRows[0].Cells[0].Value.ToString().Substring(0, 1) + ".";
                }
                else if (dtColumn.Rows[0]["TYPE"].ToString().ToLower() == "string" || dtColumn.Rows[0]["TYPE"].ToString().ToLower() == "character")
                {
                    uniqueColumnVal = "'" + dgvReader.SelectedRows[0].Cells[0].Value.ToString() + "'";
                }
                else if (dtColumn.Rows[0]["TYPE"].ToString().ToLower() == "datetime" || dtColumn.Rows[0]["TYPE"].ToString().ToLower() == "date")
                {
                    uniqueColumnVal = "{d'" + DateTime.Parse(dgvReader.SelectedRows[0].Cells[0].Value.ToString()).ToString("yyyy-MM-dd") + "'}";
                }
                else
                {
                    uniqueColumnVal = dgvReader.SelectedRows[0].Cells[0].Value.ToString();
                }

                OleDbConnection oleDbConnection;
                if (dbTableSts == true)
                {
                    oleDbConnection = new OleDbConnection("Provider=VFPOLEDB.1;Data Source=" + filePath);
                }
                else
                {
                    oleDbConnection = new OleDbConnection("Provider=VFPOLEDB.1;Data Source=" + filePath + ";");
                }
                oleDbConnection.Open();
                string sqlQuery = "Delete From " + fileName + " Where " + uniqueColumn + " = " + uniqueColumnVal;
                OleDbCommand oleDbCommand = new OleDbCommand(sqlQuery, oleDbConnection);
                int valRes = oleDbCommand.ExecuteNonQuery();
                oleDbConnection.Close();
                if (valRes >= 1)
                {
                    ShowMessageBox.Show("Data Deleted Successfully !", "Info", enumMessageIcon.Information);
                    bwReader.RunWorkerAsync(orgFilePath);
                }
                else
                {
                    ShowMessageBox.Show("Please Try Again !", "Error", enumMessageIcon.Error);
                }
            }
        }

        private void DeleteAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("Do you want to delete all record ? ",
                    "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dr == DialogResult.Yes)
            {
                OleDbConnection oleDbConnection = new OleDbConnection("Provider=VFPOLEDB.1;Data Source=" + filePath + ";");                
                oleDbConnection.Open();
                string sqlQuery = @"SET EXCLUSIVE ON
                                DELETE FROM " + fileName + @"
                                PACK";
                OleDbCommand oleDbCommand = new OleDbCommand(sqlQuery, oleDbConnection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "ExecScript"
                };
                oleDbCommand.Parameters.Add("myScript", OleDbType.Char).Value = sqlQuery;
                int valRes = oleDbCommand.ExecuteNonQuery();

                oleDbConnection.Close();
                if (valRes >= 1)
                {
                    bwReader.RunWorkerAsync(orgFilePath);
                    ShowMessageBox.Show("Data Deleted Successfully !", "Info", enumMessageIcon.Information);
                }
                else
                {
                    ShowMessageBox.Show("Please Try Again !", "Error", enumMessageIcon.Error);
                }
            }
        }

        private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutUs aboutUs = new AboutUs();
            aboutUs.ShowDialog();
        }

        private void CheckForUpdatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //AutoUpdater.CheckForUpdateEvent -= AutoUpdaterOnCheckForUpdateEvent;
            //AutoUpdater.CheckForUpdateEvent += AutoUpdaterOnCheckForUpdateEvent;
            AutoUpdater.Start(Helper.checkPath);
        }

        private void DgvReader_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1)
            {
                currRecNo = e.RowIndex + 1;
                DataGridCalculation(currRecNo);
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
                        if (dgvReader.CurrentCell.Value == null || string.IsNullOrEmpty(dgvReader.CurrentCell.Value.ToString()))
                            oDateTimePicker.Value = DateTime.Now;
                        else
                            oDateTimePicker.Value = DateTime.Parse(dgvReader.CurrentCell.Value.ToString());
                        dgvReader.Controls.Add(oDateTimePicker);
                        oDateTimePicker.Visible = false;
                        oDateTimePicker.Format = DateTimePickerFormat.Short;
                        oDateTimePicker.TextChanged += new EventHandler(DateTimePicker_OnTextChange);
                        oDateTimePicker.Visible = true;
                        oDateTimePicker.BringToFront();
                        //It returns the retangular area that represents the Display area for a cell
                        System.Drawing.Rectangle oRectangle = dgvReader.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true);
                        oDateTimePicker.Size = new Size(oRectangle.Width - 82, oRectangle.Height);
                        oDateTimePicker.Location = new Point(oRectangle.X + 82, oRectangle.Y);
                        oDateTimePicker.CloseUp += new EventHandler(DateTimePicker_CloseUp);
                    }
                    if (dtColumn.Rows[e.ColumnIndex]["TYPE"].ToString().ToUpper() == "LOGICAL")
                    {
                        dgvReader.Columns[e.ColumnIndex].ReadOnly = true;
                        string[] dataSource = { "FALSE", "TRUE" };
                        oComboBox = new ComboBox
                        {
                            DropDownStyle = ComboBoxStyle.DropDownList,
                            DataSource = dataSource.ToList()
                        };
                        dgvReader.Controls.Add(oComboBox);
                        oComboBox.Visible = false;
                        oComboBox.SelectedIndexChanged += new EventHandler(ComboBox_SelectedIndexChanged);
                        oComboBox.Visible = true;
                        oComboBox.SelectedIndex = 0;
                        //It returns the retangular area that represents the Display area for a cell
                        System.Drawing.Rectangle oRectangle = dgvReader.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true);
                        oComboBox.Size = new Size(oRectangle.Width, oRectangle.Height);
                        oComboBox.Location = new Point(oRectangle.X, oRectangle.Y);
                        oComboBox.DropDownClosed += new EventHandler(ComboBox_DropDownClosed);
                    }
                }
            }
        }

        private void DgvReader_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            //if (dataStatus != false)
            //{
            //    if (dgvReader.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null)
            //    {
            //        listNewRecord[e.RowIndex].Data[e.ColumnIndex] = dgvReader.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();

            //        //dbf.Write(orgFilePath, listNewRecord, (DbfField)listNewField[e.ColumnIndex], e.RowIndex, e.ColumnIndex);
            //    }
            //    else
            //    {
            //        listNewRecord[e.RowIndex].Data[e.ColumnIndex] = DBNull.Value;
            //    }

            //    //dbf.Write(orgFilePath, listNewRecord, listNewField, fileVersion);
            //}
            //else
            //{
            //    if (e.ColumnIndex >= 1)
            //    {
            //        Dictionary<string, string> columnNames = new Dictionary<string, string>();
            //        List<string> columnNames1 = new List<string>();
            //        List<string> columnValues = new List<string>();

            //        string colunmName = dgvReader.Columns[e.ColumnIndex].HeaderText;
            //        string columnValue = dgvReader.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString().Trim();
            //        string setCondition = "", whereCondition = "";
            //        for (int i = 0; i < dtColumn.Rows.Count; i++)
            //        {
            //            if (colunmName.ToLower() == dtColumn.Rows[i]["NAME"].ToString().ToLower())
            //            {
            //                if (dtColumn.Rows[i]["TYPE"].ToString().ToLower() == "boolean" || dtColumn.Rows[i]["TYPE"].ToString().ToLower() == "logical")
            //                {
            //                    if (string.IsNullOrWhiteSpace(columnValue))
            //                        setCondition = colunmName + "=.F.";
            //                    else
            //                        setCondition = colunmName + "=." + columnValue.Substring(0, 1) + ".";
            //                }
            //                else if (dtColumn.Rows[i]["TYPE"].ToString().ToLower() == "string" || dtColumn.Rows[i]["TYPE"].ToString().ToLower() == "character")
            //                {
            //                    if (string.IsNullOrWhiteSpace(columnValue))
            //                        setCondition = colunmName + "=''";
            //                    else
            //                        setCondition = colunmName + "='" + columnValue + "'";
            //                }
            //                else if (dtColumn.Rows[i]["TYPE"].ToString().ToLower() == "datetime" || dtColumn.Rows[i]["TYPE"].ToString().ToLower() == "date")
            //                {
            //                    if (string.IsNullOrWhiteSpace(columnValue))
            //                        setCondition = colunmName + "={.NULL.}";
            //                    else
            //                        setCondition = colunmName + "={d'" + DateTime.Parse(columnValue).ToString("yyyy-MM-dd") + "'}";
            //                }
            //                else
            //                {
            //                    if (string.IsNullOrWhiteSpace(columnValue))
            //                        setCondition = colunmName + "=0";
            //                    else
            //                        setCondition = colunmName + "=" + columnValue;
            //                }
            //                break;
            //            }
            //        }

            //        int numLoop = 0;
            //        for (int n = 0; n < dtColumn.Rows.Count; n++)
            //        {
            //            if (n != e.ColumnIndex)
            //            {
            //                if (numLoop == 0)
            //                {
            //                    if (dtColumn.Rows[n]["TYPE"].ToString().ToLower() == "boolean" || dtColumn.Rows[n]["TYPE"].ToString().ToLower() == "logical")
            //                    {
            //                        whereCondition = whereCondition + dtColumn.Rows[n]["NAME"].ToString() + "=";
            //                        if (string.IsNullOrWhiteSpace(dgvReader.Rows[e.RowIndex].Cells[n].Value.ToString()))
            //                            whereCondition = whereCondition + ".F.";
            //                        else
            //                            whereCondition = whereCondition + "." + dgvReader.Rows[e.RowIndex].Cells[n].Value.ToString().Substring(0, 1) + ".";
            //                    }
            //                    else if (dtColumn.Rows[n]["TYPE"].ToString().ToLower() == "string" || dtColumn.Rows[n]["TYPE"].ToString().ToLower() == "character")
            //                    {
            //                        whereCondition = whereCondition + dtColumn.Rows[n]["NAME"].ToString() + "=";
            //                        if (string.IsNullOrWhiteSpace(dgvReader.Rows[e.RowIndex].Cells[n].Value.ToString()))
            //                            whereCondition = whereCondition + "''";
            //                        else
            //                            whereCondition = whereCondition + "'" + dgvReader.Rows[e.RowIndex].Cells[n].Value.ToString().Trim() + "'";
            //                    }
            //                    else if (dtColumn.Rows[n]["TYPE"].ToString().ToLower() == "datetime" || dtColumn.Rows[n]["TYPE"].ToString().ToLower() == "date")
            //                    {
            //                        whereCondition = whereCondition + dtColumn.Rows[n]["NAME"].ToString() + "=";
            //                        if (string.IsNullOrWhiteSpace(dgvReader.Rows[e.RowIndex].Cells[n].Value.ToString()))
            //                            whereCondition = whereCondition + "{.NULL.}";
            //                        else
            //                            whereCondition = whereCondition + "{d'" + DateTime.Parse(dgvReader.Rows[e.RowIndex].Cells[n].Value.ToString().Trim()).ToString("yyyy-MM-dd") + "'}";
            //                    }
            //                    else
            //                    {
            //                        whereCondition = whereCondition + dtColumn.Rows[n]["NAME"].ToString() + "=";
            //                        if (string.IsNullOrWhiteSpace(dgvReader.Rows[e.RowIndex].Cells[n].Value.ToString()))
            //                            whereCondition = whereCondition + "0";
            //                        else
            //                            whereCondition = whereCondition + dgvReader.Rows[e.RowIndex].Cells[n].Value.ToString();
            //                    }
            //                }
            //                else
            //                {
            //                    if (dtColumn.Rows[n]["TYPE"].ToString().ToLower() == "boolean" || dtColumn.Rows[n]["TYPE"].ToString().ToLower() == "logical")
            //                    {
            //                        whereCondition = whereCondition + " AND " + dtColumn.Rows[n]["NAME"].ToString() + "=";
            //                        if (string.IsNullOrWhiteSpace(dgvReader.Rows[e.RowIndex].Cells[n].Value.ToString()))
            //                            whereCondition = whereCondition + ".F.";
            //                        else
            //                            whereCondition = whereCondition + "." + dgvReader.Rows[e.RowIndex].Cells[n].Value.ToString().Substring(0, 1) + ".";
            //                    }
            //                    else if (dtColumn.Rows[n]["TYPE"].ToString().ToLower() == "string" || dtColumn.Rows[n]["TYPE"].ToString().ToLower() == "character")
            //                    {
            //                        whereCondition = whereCondition + " AND " + dtColumn.Rows[n]["NAME"].ToString() + "=";
            //                        if (string.IsNullOrWhiteSpace(dgvReader.Rows[e.RowIndex].Cells[n].Value.ToString()))
            //                            whereCondition = whereCondition + "''";
            //                        else
            //                            whereCondition = whereCondition + "'" + dgvReader.Rows[e.RowIndex].Cells[n].Value.ToString().Trim() + "'";
            //                    }
            //                    else if (dtColumn.Rows[n]["TYPE"].ToString().ToLower() == "datetime" || dtColumn.Rows[n]["TYPE"].ToString().ToLower() == "date")
            //                    {
            //                        whereCondition = whereCondition + " AND " + dtColumn.Rows[n]["NAME"].ToString() + "=";
            //                        if (string.IsNullOrWhiteSpace(dgvReader.Rows[e.RowIndex].Cells[n].Value.ToString()))
            //                            whereCondition = whereCondition + "{.NULL.}";
            //                        else
            //                            whereCondition = whereCondition + "{d'" + DateTime.Parse(dgvReader.Rows[e.RowIndex].Cells[n].Value.ToString().Trim()).ToString("yyyy-MM-dd") + "'}";
            //                    }
            //                    else
            //                    {
            //                        whereCondition = whereCondition + " AND " + dtColumn.Rows[n]["NAME"].ToString() + "=";
            //                        if (string.IsNullOrWhiteSpace(dgvReader.Rows[e.RowIndex].Cells[n].Value.ToString()))
            //                            whereCondition = whereCondition + "0";
            //                        else
            //                            whereCondition = whereCondition + dgvReader.Rows[e.RowIndex].Cells[n].Value.ToString();
            //                    }
            //                }

            //                numLoop = numLoop + 1;
            //            }
            //        }

            //        OleDbConnection connection = new OleDbConnection("Provider=VFPOLEDB.1;Data Source=" + filePath);
            //        connection.Open();
            //        string sql = "Update " + fileName + " Set " + setCondition + " Where " + whereCondition;
            //        OleDbCommand cmd = new OleDbCommand(sql, connection);
            //        cmd.ExecuteNonQuery();
            //        connection.Close();
            //    }
            //}
        }

        private void DgvReader_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (dataStatus != false)
            {
                if (dgvReader.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null)
                {
                    listNewRecord[e.RowIndex].Data[e.ColumnIndex] = dgvReader.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();

                    //dbf.Write(orgFilePath, listNewRecord, (DbfField)listNewField[e.ColumnIndex], e.RowIndex, e.ColumnIndex);
                }
                else
                {
                    listNewRecord[e.RowIndex].Data[e.ColumnIndex] = DBNull.Value;
                }

                //dbf.Write(orgFilePath, listNewRecord, listNewField, fileVersion);
            }
            else
            {
                if (e.ColumnIndex >= 1)
                {
                    string colunmName = dgvReader.Columns[e.ColumnIndex].HeaderText;
                    string columnValue = dgvReader.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
                    string uniqueColumn = dgvReader.Columns[0].HeaderText;
                    string uniqueColumnVal;
                    if (dtColumn.Rows[0]["TYPE"].ToString().ToLower() == "boolean" || dtColumn.Rows[0]["TYPE"].ToString().ToLower() == "logical")
                    {
                        uniqueColumnVal = "=." + dgvReader.Rows[e.RowIndex].Cells[0].Value.ToString().Substring(0, 1) + ".";
                    }
                    else if (dtColumn.Rows[0]["TYPE"].ToString().ToLower() == "string" || dtColumn.Rows[0]["TYPE"].ToString().ToLower() == "character")
                    {
                        uniqueColumnVal = "'" + dgvReader.Rows[e.RowIndex].Cells[0].Value.ToString() + "'";
                    }
                    else if (dtColumn.Rows[0]["TYPE"].ToString().ToLower() == "datetime" || dtColumn.Rows[0]["TYPE"].ToString().ToLower() == "date")
                    {
                        uniqueColumnVal = "{d'" + DateTime.Parse(dgvReader.Rows[e.RowIndex].Cells[0].Value.ToString()).ToString("yyyy-MM-dd") + "'}";
                    }
                    else
                    {
                        uniqueColumnVal = dgvReader.Rows[e.RowIndex].Cells[0].Value.ToString();
                    }

                    string setCondition = "";
                    for (int i = 0; i < dtColumn.Rows.Count; i++)
                    {
                        if (colunmName.ToLower() == dtColumn.Rows[i]["NAME"].ToString().ToLower())
                        {
                            if (dtColumn.Rows[i]["TYPE"].ToString().ToLower() == "boolean" || dtColumn.Rows[i]["TYPE"].ToString().ToLower() == "logical")
                            {
                                setCondition = colunmName + "=." + columnValue.Substring(0, 1) + ".";
                            }
                            else if (dtColumn.Rows[i]["TYPE"].ToString().ToLower() == "string" || dtColumn.Rows[i]["TYPE"].ToString().ToLower() == "character")
                            {
                                setCondition = colunmName + "='" + columnValue + "'";
                            }
                            else if (dtColumn.Rows[i]["TYPE"].ToString().ToLower() == "datetime" || dtColumn.Rows[i]["TYPE"].ToString().ToLower() == "date")
                            {
                                setCondition = colunmName + "={d'" + DateTime.Parse(columnValue).ToString("yyyy-MM-dd") + "'}";
                            }
                            else
                            {
                                setCondition = colunmName + "=" + columnValue;
                            }
                            break;
                        }
                    }

                    OleDbConnection oleDbConnection = new OleDbConnection("Provider=VFPOLEDB.1;Data Source=" + filePath);
                    oleDbConnection.Open();
                    string sql = "Update " + fileName + " Set " + setCondition + " Where " + uniqueColumn + " = " + uniqueColumnVal;
                    OleDbCommand oleDbCommand = new OleDbCommand(sql, oleDbConnection);
                    oleDbCommand.ExecuteNonQuery();
                    oleDbConnection.Close();
                }
            }
        }

        private void DgvReader_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            for (int i = 0; i < dgvReader.Columns.Count; i++)
            {
                if (dgvReader.CurrentCell.ColumnIndex == i)
                {
                    if (dtColumn.Rows[i]["TYPE"].ToString() == "Numeric" || dtColumn.Rows[i]["TYPE"].ToString() == "Float" || dtColumn.Rows[i]["TYPE"].ToString() == "Currency")
                    {
                        e.Control.KeyPress -= new KeyPressEventHandler(NumericColumn_KeyPress);
                        if (e.Control is TextBox txtNumber)
                        {
                            txtNumber.KeyPress += new KeyPressEventHandler(NumericColumn_KeyPress);
                        }
                    }
                    else if (dtColumn.Rows[i]["TYPE"].ToString() == "Decimal" || dtColumn.Rows[i]["TYPE"].ToString() == "Double")
                    {
                        e.Control.KeyPress -= new KeyPressEventHandler(NumericColumn_KeyPress);
                        if (e.Control is TextBox txtNumber)
                        {
                            txtNumber.KeyPress += new KeyPressEventHandler(NumericColumn_KeyPress);
                        }
                    }
                    else if (dtColumn.Rows[i]["TYPE"].ToString() == "Integer")
                    {
                        e.Control.KeyPress -= new KeyPressEventHandler(NumberColumn_KeyPress);
                        if (e.Control is TextBox txtNumber)
                        {
                            txtNumber.KeyPress += new KeyPressEventHandler(NumberColumn_KeyPress);
                        }
                    }
                    else
                    {
                        e.Control.KeyPress -= new KeyPressEventHandler(CharacterColumn_KeyPress);
                        if (e.Control is TextBox txtNumber)
                        {
                            txtNumber.KeyPress += new KeyPressEventHandler(CharacterColumn_KeyPress);
                        }
                    }
                    break;
                }
            }
        }

        private void DgvReader_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            if (oDateTimePicker != null)
                oDateTimePicker.Visible = false;
            if (oComboBox != null)
                oComboBox.Visible = false;
        }

        private void DgvReader_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
            if (oDateTimePicker != null)
                oDateTimePicker.Visible = false;
            if (oComboBox != null)
                oComboBox.Visible = false;
        }

        private void BwReader_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                bgReaderSts = true;
                ReadData(e.Argument.ToString());
                dgvReader.Invoke(new Action(() => dgvReader.DataSource = dtData.Clone()));
                DataGridDesign();
                ChangeDateValue();
                dgvReader.Invoke(new Action(() => dgvReader.DataSource = dtData));
            }
            catch (Exception ex)
            {
                loader.Close();
                bgReaderSts = false;
                ShowMessageBox.Show("Error :- " + ex.Message, "Error", enumMessageIcon.Error);
            }
        }

        private void BwReader_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (bgReaderSts == false)
                {
                    ClearToolStripMenuItem_Click(null, null);
                }
                else
                {
                    currRecNo = 1;
                    DataGridCalculation(currRecNo);
                    totRecNo = dgvReader.Rows.Count;
                    saveToolStripMenuItem.Enabled = true;
                    findToolStripMenuItem.Enabled = true;
                    findNextToolStripMenuItem.Enabled = true;
                    gotoToolStripMenuItem.Enabled = true;
                    skipToolStripMenuItem.Enabled = true;
                    appendToolStripMenuItem.Enabled = true;
                    deleteToolStripMenuItem.Enabled = true;
                    deleteAllToolStripMenuItem.Enabled = true;
                    loader.Close();
                }
            }
            catch { loader.Close(); }
        }

        private void BwExcel_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                Excel.Application XcelApp = new Excel.Application();
                Excel._Workbook workbook = XcelApp.Workbooks.Add(Type.Missing);
                Excel._Worksheet worksheet = null;

                worksheet = workbook.Sheets["Sheet1"];
                worksheet = workbook.ActiveSheet;
                worksheet.Name = "Output";
                worksheet.Application.ActiveWindow.SplitRow = 1;
                worksheet.Application.ActiveWindow.FreezePanes = true;

                for (int i = 1; i < dtExcelExport.Columns.Count + 1; i++)
                {
                    worksheet.Cells[1, i] = dtExcelExport.Columns[i - 1].ColumnName;
                    worksheet.Cells[1, i].Font.NAME = "Calibri";
                    worksheet.Cells[1, i].Font.Bold = true;
                    worksheet.Cells[1, i].Interior.Color = Color.Wheat;
                    worksheet.Cells[1, i].Font.Size = 12;
                }

                int valProgress = 0;
                int maxVal = dtExcelExport.Rows.Count;
                for (int i = 0; i < dtExcelExport.Rows.Count; i++)
                {
                    for (int j = 0; j < dtExcelExport.Columns.Count; j++)
                    {
                        worksheet.Cells[i + 2, j + 1] = dtExcelExport.Rows[i][j].ToString();
                    }
                    int perProgress = (valProgress * 100) / maxVal;
                    bwExcel.ReportProgress(perProgress);
                    valProgress += 1;
                }

                worksheet.Columns.AutoFit();
                workbook.SaveAs(outputFileName);
                XcelApp.Quit();
                excelExport = true;
                ReleaseObject(worksheet);
                ReleaseObject(workbook);
                ReleaseObject(XcelApp);
            }
            catch (Exception ex)
            {
                excelExport = false;
                Helper.WriteToFile("Error :" + ex.Message);
            }
        }

        private void BwExcel_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            tsProgressBar.Value = e.ProgressPercentage;
            tsslProgPer.Text = "Exporting... " + e.ProgressPercentage.ToString() + " %";
        }

        private void BwExcel_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (excelExport == true)
            {
                try
                {
                    //File.WriteAllLines(outputFileName, outputCsv, Encoding.UTF8);
                    tsProgressBar.Value += 1;
                    tsslProgPer.Text = "100 %";
                    dtExcelExport = null;
                    ShowMessageBox.Show("Data Exported Successfully...", "Success");
                }
                catch (Exception ex)
                {
                    ShowMessageBox.Show(ex.Message);
                }
            }
            else
            {
                ShowMessageBox.Show("Please Try Again...", "Error", enumMessageIcon.Error);
            }
            tsProgressBar.Visible = false;
            tsslProgPer.Visible = false;
            tsslProgPer.Text = "";
            csvToolStripMenuItem.Enabled = true;
            excelToolStripMenuItem.Enabled = true;
            pdfToolStripMenuItem.Enabled = true;
        }

        private void BwPdf_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                PdfPTable pdfTable = new PdfPTable(dtPdfExport.Columns.Count);
                pdfTable.DefaultCell.Padding = 3;
                pdfTable.WidthPercentage = 100;
                pdfTable.HorizontalAlignment = Element.ALIGN_LEFT;

                foreach (DataColumn column in dtPdfExport.Columns)
                {
                    PdfPCell cell = new PdfPCell(new Phrase(column.ColumnName));
                    pdfTable.AddCell(cell);
                }

                int valProgress = 0;
                int maxVal = dtPdfExport.Rows.Count;
                foreach (DataRow row in dtPdfExport.Rows)
                {
                    foreach (var cell in row.ItemArray)
                    {
                        pdfTable.AddCell(cell.ToString());
                    }

                    int perProgress = (valProgress * 100) / maxVal;
                    bwPdf.ReportProgress(perProgress);
                    valProgress += 1;
                }

                using (FileStream stream = new FileStream(outputFileName, FileMode.Create))
                {
                    Document pdfDoc = new Document(PageSize.A4, 10f, 20f, 20f, 10f);
                    PdfWriter.GetInstance(pdfDoc, stream);
                    pdfDoc.Open();
                    pdfDoc.Add(pdfTable);
                    pdfDoc.Close();
                    stream.Close();
                }
                pdfExport = true;
            }
            catch (Exception ex)
            {
                pdfExport = false;
                Helper.WriteToFile("Error :" + ex.Message);
            }
        }

        private void BwPdf_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            tsProgressBar.Value = e.ProgressPercentage;
            tsslProgPer.Text = "Exporting... " + e.ProgressPercentage.ToString() + " %";
        }

        private void BwPdf_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (pdfExport == true)
            {
                tsProgressBar.Value += 1;
                tsslProgPer.Text = "100 %";
                dtPdfExport = null;
                ShowMessageBox.Show("Data Exported Successfully...", "Success");
            }
            else
            {
                ShowMessageBox.Show("Please Try Again...", "Error", enumMessageIcon.Error);
            }
            tsProgressBar.Visible = false;
            tsslProgPer.Visible = false;
            tsslProgPer.Text = "";
            csvToolStripMenuItem.Enabled = true;
            excelToolStripMenuItem.Enabled = true;
            pdfToolStripMenuItem.Enabled = true;
        }

        private void BwCsv_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                int columnCount = dtCsvExport.Columns.Count;
                string columnNames = "";
                outputCsv = new string[dtCsvExport.Rows.Count + 1];
                for (int i = 0; i < columnCount; i++)
                {
                    columnNames += dtCsvExport.Columns[i].ColumnName.ToString() + ",";
                }
                outputCsv[0] += columnNames;
                int valProgress = 0;
                int maxVal = dtCsvExport.Rows.Count;
                for (int i = 1; (i - 1) < dtCsvExport.Rows.Count; i++)
                {
                    for (int j = 0; j < columnCount; j++)
                    {
                        outputCsv[i] += dtCsvExport.Rows[i - 1][j].ToString() + ",";
                    }

                    int perProgress = (valProgress * 100) / maxVal;
                    bwCsv.ReportProgress(perProgress);
                    valProgress += 1;
                }
                csvExport = true;
            }
            catch
            {
                csvExport = false;
            }
        }

        private void BwCsv_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            tsProgressBar.Value = e.ProgressPercentage;
            tsslProgPer.Text = "Exporting... " + e.ProgressPercentage.ToString() + " %";
        }

        private void BwCsv_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (csvExport == true)
            {
                try
                {
                    File.WriteAllLines(outputFileName, outputCsv, Encoding.UTF8);
                    tsProgressBar.Value += 1;
                    tsslProgPer.Text = "100 %";
                    dtCsvExport = null;
                    ShowMessageBox.Show("Data Exported Successfully...", "Success");
                }
                catch (Exception ex)
                {
                    ShowMessageBox.Show(ex.Message);
                }
            }
            else
            {
                ShowMessageBox.Show("Please Try Again...", "Error", enumMessageIcon.Error);
            }
            tsProgressBar.Visible = false;
            tsslProgPer.Visible = false;
            tsslProgPer.Text = "";
            csvToolStripMenuItem.Enabled = true;
            excelToolStripMenuItem.Enabled = true;
            pdfToolStripMenuItem.Enabled = true;
        }

        private void BwFind_DoWork(object sender, DoWorkEventArgs e)
        {
            dgvReader.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            try
            {
                int firstCol;
                nxtRecNo = 0;
                foreach (DataGridViewRow row in dgvReader.Rows)
                {
                    nxtRecNo += 1;
                    for (int i = 0; i < dtItems.Rows.Count; i++)
                    {
                        firstCol = int.Parse(dtItems.Rows[i][1].ToString());
                        if (caseSensetiveSts == true)
                        {
                            if (row.Cells[firstCol].Value.ToString().Trim().Equals(varSearch))
                            {
                                row.Selected = true;
                                dgvReader.Invoke(new Action(() => dgvReader.CurrentCell = row.Cells[firstCol]));
                                fndRedord = true;
                                currRecNo = row.Index + 1;
                                DataGridCalculation(currRecNo);
                                break;
                            }
                        }
                        else
                        {
                            if (row.Cells[firstCol].Value.ToString().ToLower().Trim().Equals(varSearch.ToLower()))
                            {
                                row.Selected = true;
                                dgvReader.Invoke(new Action(() => dgvReader.CurrentCell = row.Cells[firstCol]));
                                fndRedord = true;
                                currRecNo = row.Index + 1;
                                DataGridCalculation(currRecNo);
                                return;
                            }
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                ShowMessageBox.Show("Error :- " + exc.Message, "Error", enumMessageIcon.Error);
            }
        }

        private void BwFind_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            loader.Close();
            if (fndRedord == false)
            {
                dgvReader.Rows[0].Selected = true;
                ShowMessageBox.Show("Search String '" + varSearch + "' Not Found !");
            }
        }

        private void BwFindNxt_DoWork(object sender, DoWorkEventArgs e)
        {
            fndRedord = false;
            try
            {
                int firstCol;
                dgvReader.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                for (int i = nxtRecNo; i < dgvReader.Rows.Count; i++)
                {
                    if (nxtRecNo == dgvReader.Rows.Count - 1)
                    {
                        nxtRecNo = 0;
                    }
                    else
                    {
                        nxtRecNo += 1;
                    }
                    for (int n = 0; n < dtItems.Rows.Count; n++)
                    {
                        firstCol = int.Parse(dtItems.Rows[n][1].ToString());
                        if (caseSensetiveSts == true)
                        {
                            if (dgvReader.Rows[i].Cells[firstCol].Value.ToString().Trim().Equals(varSearch))
                            {
                                dgvReader.Rows[i].Selected = true;
                                dgvReader.Invoke(new Action(() => dgvReader.CurrentCell = dgvReader.Rows[i].Cells[firstCol]));
                                fndRedord = true;
                                currRecNo = dgvReader.Rows[i].Index + 1;
                                DataGridCalculation(currRecNo);
                                break;
                            }
                        }
                        else
                        {
                            if (dgvReader.Rows[i].Cells[firstCol].Value.ToString().ToLower().Trim().Equals(varSearch.ToLower()))
                            {
                                dgvReader.Rows[i].Selected = true;
                                dgvReader.Invoke(new Action(() => dgvReader.CurrentCell = dgvReader.Rows[i].Cells[firstCol]));
                                fndRedord = true;
                                currRecNo = dgvReader.Rows[i].Index + 1;
                                DataGridCalculation(currRecNo);
                                return;
                            }
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                ShowMessageBox.Show("Error :- " + exc.Message, "Error", enumMessageIcon.Error);
            }
        }

        private void BwFindNxt_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            loader.Close();
            if (fndRedord == false)
            {
                ShowMessageBox.Show("No more record found for String '" + varSearch + "' !");
            }
        }

        public string GetFileSize(long fSize)
        {
            if (fSize >= 1048576)
            {
                return ((fSize / 1024f) / 1024f).ToString("0.00") + " MB";
            }
            else if (fSize >= 1024 && fSize < 1048576)
            {
                return (fSize / 1024f).ToString("0.00") + " KB";
            }
            else
            {
                return fSize.ToString() + " B";
            }
        }

        static DataTable ConvertListToDataTable(List<DbfRecord> listRecord, List<DbfField> listField)
        {
            // New table.
            DataTable table = new DataTable();

            // Add columns.
            for (int i = 0; i < listField.Count; i++)
            {
                if (listField[i].Type.ToString() == "Integer")
                {
                    table.Columns.Add(listField[i].Name.ToString(), typeof(int));
                }
                else if (listField[i].Type.ToString() == "Float")
                {
                    table.Columns.Add(listField[i].Name.ToString(), typeof(float));
                }
                else if (listField[i].Type.ToString() == "Date" || listField[i].Type.ToString() == "DateTime")
                {
                    table.Columns.Add(listField[i].Name.ToString(), typeof(DateTime));
                }
                else if (listField[i].Type.ToString() == "Logical")
                {
                    table.Columns.Add(listField[i].Name.ToString(), typeof(bool));
                }
                //else if (listField[i].Type.ToString() == "Currency")
                //{
                //    table.Columns.Add(listField[i].Name.ToString(), typeof(decimal));
                //}
                else
                {
                    table.Columns.Add(listField[i].Name.ToString(), typeof(string));
                }
            }

            // Add rows.
            for (int i = 0; i < listRecord.Count; i++)
            {
                DataRow dr = table.NewRow();
                for (int j = 0; j < listRecord[i].Data.Count; j++)
                {
                    if (listRecord[i].Data[j] == null || listRecord[i].Data[j].ToString() == "01/Jan/0001 12:00:00 AM")
                    {
                        dr[j] = DBNull.Value;
                    }
                    else
                    {
                        dr[j] = listRecord[i].Data[j];
                    }
                }
                table.Rows.Add(dr);
            }

            return table;
        }

        static DataTable GetColumnDataTable(List<DbfField> listField)
        {
            // New table.
            DataTable table = new DataTable();
            table.Columns.Add("NAME", typeof(string));
            table.Columns.Add("TYPE", typeof(string));
            table.Columns.Add("LENGTH", typeof(string));
            table.Columns.Add("DEC", typeof(string));

            // Add rows.
            for (int i = 0; i < listField.Count; i++)
            {
                DataRow dr = table.NewRow();
                dr[0] = listField[i].Name == null ? DBNull.Value : (object)listField[i].Name.ToString();
                dr[1] = listField[i].Type == null ? DBNull.Value : (object)listField[i].Type.ToString();
                dr[2] = listField[i].Length == null ? DBNull.Value : (object)listField[i].Length.ToString();
                dr[3] = listField[i].Length == null ? 0 : (object)listField[i].Precision.ToString();
                table.Rows.Add(dr);
            }

            return table;
        }

        static DataTable GetColumnDataTable()
        {
            // New table.
            DataTable table = new DataTable();
            table.Columns.Add("NAME", typeof(string));
            table.Columns.Add("TYPE", typeof(string));
            table.Columns.Add("TYPENO", typeof(string));
            table.Columns.Add("DEC", typeof(string));
            return table;
        }

        private void DataGridCalculation(int rowNo)
        {
            tsslTotRecord.Text = "Total Record : " + rowNo + "/" + dgvReader.Rows.Count;
            tsslFileSize.Text = fileSize;
            if (dgvReader.Rows.Count > 0)
                dgvReader.Columns[0].ReadOnly = true;
        }

        private DataTable CreateNewTable(DataTable dt)
        {
            DataTable dtNew = new DataTable();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dtNew.Columns.Add(dt.Rows[i][0].ToString(), typeof(string));
            }
            return dtNew;
        }

        private void DgvReader_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {

        }

        private void WriteNewData()
        {
            try
            {
                dbf = new Dbf();

                for (int i = 0; i < dtColumn.Rows.Count; i++)
                {
                    DbfField field;
                    if (dtColumn.Rows[i][1].ToString() == "Numeric")
                    {
                        field = new DbfField(dtColumn.Rows[i][0].ToString(), DbfFieldType.Numeric, byte.Parse(dtColumn.Rows[i][2].ToString()));
                    }
                    else if (dtColumn.Rows[i][1].ToString() == "Date")
                    {
                        field = new DbfField(dtColumn.Rows[i][0].ToString(), DbfFieldType.Date, 12);
                    }
                    else if (dtColumn.Rows[i][1].ToString() == "Logical")
                    {
                        field = new DbfField(dtColumn.Rows[i][0].ToString(), DbfFieldType.Logical, 12);
                    }
                    else if (dtColumn.Rows[i][1].ToString() == "Float")
                    {
                        field = new DbfField(dtColumn.Rows[i][0].ToString(), DbfFieldType.Float, byte.Parse(dtColumn.Rows[i][2].ToString()));
                    }
                    else if (dtColumn.Rows[i][1].ToString() == "Integer")
                    {
                        field = new DbfField(dtColumn.Rows[i][0].ToString(), DbfFieldType.Numeric, byte.Parse(dtColumn.Rows[i][2].ToString()));
                    }
                    else if (dtColumn.Rows[i][1].ToString() == "Memo")
                    {
                        field = new DbfField(dtColumn.Rows[i][0].ToString(), DbfFieldType.Memo, byte.Parse(dtColumn.Rows[i][2].ToString()));
                    }
                    else if (dtColumn.Rows[i][1].ToString() == "Picture")
                    {
                        field = new DbfField(dtColumn.Rows[i][0].ToString(), DbfFieldType.Picture, byte.Parse(dtColumn.Rows[i][2].ToString()));
                    }
                    else if (dtColumn.Rows[i][1].ToString() == "NullFlags")
                    {
                        field = new DbfField(dtColumn.Rows[i][0].ToString(), DbfFieldType.NullFlags, byte.Parse(dtColumn.Rows[i][2].ToString()));
                    }
                    else // For Character
                    {
                        field = new DbfField(dtColumn.Rows[i][0].ToString(), DbfFieldType.Character, byte.Parse(dtColumn.Rows[i][2].ToString()));
                    }

                    dbf.Fields.Add(field);
                }

                for (int i = 0; i < dgvReader.Rows.Count - 1; i++)
                {
                    DbfRecord dbfRecord = dbf.CreateRecord();
                    for (int j = 0; j < dgvReader.Columns.Count; j++)
                    {
                        dbfRecord.Data[j] = dgvReader.Rows[i].Cells[j].Value.ToString();
                    }
                }
                dbf.Write(newFileName, fileVersion);
                ShowMessageBox.Show("File Saved !!!");
            }
            catch (Exception ex)
            {
                ShowMessageBox.Show("Error :- " + ex.Message, "Error", enumMessageIcon.Error);
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
            dgvReader.CurrentCell.Value = oDateTimePicker.Text.ToString();
            oDateTimePicker.Visible = false;
        }

        void DateTimePicker_CloseUp(object sender, EventArgs e)
        {
            oDateTimePicker.Visible = false;
        }

        private void ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            dgvReader.CurrentCell.Value = oComboBox.Text.ToString();
            oComboBox.Visible = false;
        }

        void ComboBox_DropDownClosed(object sender, EventArgs e)
        {
            oComboBox.Visible = false;
        }

        private void ReadData(string rfileName)
        {
            OleDbConnection oleDbConnection = new OleDbConnection("Provider=VFPOLEDB.1;Data Source=" + filePath);
            oleDbConnection.Open();
            DataTable tables = oleDbConnection.GetSchema(System.Data.OleDb.OleDbMetaDataCollectionNames.Tables);
            dbTableSts = true;
            dtColumn = null;
            string fName = System.IO.Path.GetFileNameWithoutExtension(rfileName);
            foreach (System.Data.DataRow rowTables in tables.Rows)
            {
                if (rowTables["table_name"].ToString().ToUpper() == fName.ToUpper())
                {
                    DataTable columns = oleDbConnection.GetSchema(
                        System.Data.OleDb.OleDbMetaDataCollectionNames.Columns,
                        new String[] { null, null, rowTables["table_name"].ToString(), null });

                    dtColumn = GetColumnDataTable();
                    foreach (System.Data.DataRow rowColumns in columns.Rows)
                    {
                        DataRow dr = dtColumn.NewRow();
                        dr[0] = rowColumns["column_name"].ToString();
                        dr[1] = CustomOleDbType(int.Parse(rowColumns["data_type"].ToString()));
                        dr[2] = rowColumns["data_type"].ToString();
                        dr[3] = rowColumns["numeric_precision"].ToString();
                        dtColumn.Rows.Add(dr);
                    }
                }
            }

            if (dtColumn == null)
            {
                dbf = new Dbf();
                List<DbfField> listField = dbf.ReadColumns(rfileName);
                dtColumn = GetColumnDataTable(listField);
            }

            string sql = "SELECT * FROM " + fName + " WHERE NOT DELETED()";
            OleDbCommand oleDbCommand = new OleDbCommand(sql, oleDbConnection);
            OleDbDataAdapter oleDbDataAdapter = new OleDbDataAdapter(oleDbCommand);
            dtData = new DataTable();
            oleDbDataAdapter.Fill(dtData);
            oleDbConnection.Close();

            //if (dtColumn == null)
            //{
            //    dbTableSts = false;
            //    dbf = new Dbf();
            //    dtData = new DataTable();
            //    var dataRecords = dbf.Read(rfileName);
            //    List<DbfRecord> listRecord = dataRecords.Item1;
            //    List<DbfField> listField = dataRecords.Item2;
            //    fileVersion = dataRecords.Item3;
            //    dtColumn = GetColumnDataTable(listField);
            //    dtData = ConvertListToDataTable(listRecord, listField);
            //}
        }

        public string CustomOleDbType(int type)
        {
            string dataType;
            switch (type)
            {
                case 10:
                    dataType = "BigInt";
                    break;
                case 128:
                    dataType = "Byte";
                    break;
                case 11:
                    dataType = "Boolean";
                    break;
                case 8:
                    dataType = "String";
                    break;
                case 129:
                    dataType = "String";
                    break;
                case 6:
                    dataType = "Currency";
                    break;
                case 7:
                    dataType = "DateTime";
                    break;
                case 133:
                    dataType = "DateTime";
                    break;
                case 134:
                    dataType = "TimeSpan";
                    break;
                case 135:
                    dataType = "DateTime";
                    break;
                case 14:
                    dataType = "Decimal";
                    break;
                case 5:
                    dataType = "Double";
                    break;
                case 3:
                    dataType = "Integer";
                    break;
                case 201:
                    dataType = "String";
                    break;
                case 203:
                    dataType = "String";
                    break;
                case 204:
                    dataType = "Byte";
                    break;
                case 200:
                    dataType = "String";
                    break;
                case 139:
                    dataType = "Decimal";
                    break;
                case 202:
                    dataType = "String";
                    break;
                case 130:
                    dataType = "String";
                    break;
                case 131:
                    dataType = "Decimal";
                    break;
                case 64:
                    dataType = "DateTime";
                    break;

                default:
                    dataType = "";
                    break;
            }

            return dataType;
        }

        private int GetMaxUniqueId()
        {
            string tempMaxVal = "0";
            try
            {
                //var MaxID2 = dgvReader.Rows.Cast<DataGridViewRow>().Max(r => int.TryParse(r.Cells[0].Value.ToString(), out tempMaxVal) ? tempMaxVal : 0);
                tempMaxVal = (from DataGridViewRow row in dgvReader.Rows
                              where row.Cells[0].FormattedValue.ToString() != string.Empty
                              select Convert.ToInt32(row.Cells[0].FormattedValue)).Max().ToString();

                //int max = 0;
                //for (int i = 0; i <= dgvReader.Rows.Count - 1; i++)
                //{
                //    if (max < int.Parse(dgvReader.Rows[i].Cells[0].Value.ToString()))
                //    {
                //        max = int.Parse(dgvReader.Rows[i].Cells[0].Value.ToString());
                //    }
                //}

                //int[] columnData = new int[dgvReader.Rows.Count];
                //columnData = (from DataGridViewRow row in dgvReader.Rows
                //              where row.Cells[0].FormattedValue.ToString() != string.Empty
                //              select Convert.ToInt32(row.Cells[0].FormattedValue)).ToArray();
                //string tempMaxVal2 = columnData.Max().ToString();
            }
            catch
            {
                tempMaxVal = "";
            }
            if (IsDigits(tempMaxVal))
                return int.Parse(tempMaxVal) + 1;
            else
                return 0;
        }

        bool IsDigits(string strVal)
        {
            if (strVal == null || strVal == "") return false;

            for (int i = 0; i < strVal.Length; i++)
            {
                if ((strVal[i] ^ '0') > 9)
                    return false;
            }
            return true;
        }

        private void DataGridDesign()
        {
            try
            {
                for (int i = 0; i < dtColumn.Rows.Count; i++)
                {
                    dgvReader.Invoke(new Action(() => dgvReader.Columns[i].Name = dtColumn.Rows[i][0].ToString()));
                    dgvReader.Invoke(new Action(() => dgvReader.Columns[i].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter));
                    if (dtColumn.Rows[i]["TYPE"].ToString().ToLower() == "boolean" || dtColumn.Rows[i]["TYPE"].ToString().ToLower() == "logical")
                    {
                        dgvReader.Invoke(new Action(() => dgvReader.Columns[i].Width = 70));
                        dgvReader.Invoke(new Action(() => dgvReader.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter));
                    }
                    else if (dtColumn.Rows[i]["TYPE"].ToString().ToLower() == "numeric" || dtColumn.Rows[i]["TYPE"].ToString().ToLower() == "decimal" ||
                        dtColumn.Rows[i]["TYPE"].ToString().ToLower() == "integer")
                    {
                        dgvReader.Invoke(new Action(() => dgvReader.Columns[i].Width = 75));
                        dgvReader.Invoke(new Action(() => dgvReader.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight));
                    }
                    else if (dtColumn.Rows[i]["TYPE"].ToString().ToLower() == "float" || dtColumn.Rows[i]["TYPE"].ToString().ToLower() == "double")
                    {
                        dgvReader.Invoke(new Action(() => dgvReader.Columns[i].Width = 75));
                        dgvReader.Invoke(new Action(() => dgvReader.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight));
                        dgvReader.Invoke(new Action(() => dgvReader.Columns[i].DefaultCellStyle.Format = string.Format("0.00")));
                    }
                    else if (dtColumn.Rows[i]["TYPE"].ToString().ToLower() == "currency")
                    {
                        dgvReader.Invoke(new Action(() => dgvReader.Columns[i].Width = 90));
                        dgvReader.Invoke(new Action(() => dgvReader.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight));
                        dgvReader.Invoke(new Action(() => dgvReader.Columns[i].DefaultCellStyle.Format = string.Format("0.0000")));
                    }
                    else if (dtColumn.Rows[i]["TYPE"].ToString().ToLower() == "datetime" && dtColumn.Rows[i]["TYPENO"].ToString() == "135")
                    {
                        dgvReader.Invoke(new Action(() => dgvReader.Columns[i].Width = 150));
                        dgvReader.Invoke(new Action(() => dgvReader.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft));
                    }
                    else
                    {
                        dgvReader.Invoke(new Action(() => dgvReader.Columns[i].Width = 100));
                        dgvReader.Invoke(new Action(() => dgvReader.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft));
                    }
                }
                dgvReader.AllowUserToAddRows = false;
            }
            catch (Exception ex)
            {
                Helper.WriteToFile("Error :" + ex.Message);
            }
        }

        private void ChangeDateValue()
        {
            for (int i = 0; i < dtColumn.Rows.Count; i++)
            {
                if (dtColumn.Rows[i]["TYPE"].ToString().ToLower() == "date" || dtColumn.Rows[i]["TYPE"].ToString().ToLower() == "datetime")
                {
                    for (int n = 0; n < dtData.Rows.Count; n++)
                    {
                        try
                        {
                            if (dtData.Rows[n][i].ToString() != "" && DateTime.Parse(dtData.Rows[n][i].ToString()).ToString("yyyy-MM-dd") == "1899-12-30")
                            {
                                dtData.Rows[n][i] = DBNull.Value;
                            }
                        }
                        catch { }
                    }
                }
            }
        }

        private void AutoUpdaterOnCheckForUpdateEvent(UpdateInfoEventArgs args)
        {
            if (args != null)
            {
                CheckUpdate checkUpdate = new CheckUpdate
                {
                    CurrentVersion = string.Format("Current Version {0}", args.InstalledVersion)
                };
                if (args.IsUpdateAvailable)
                {
                    checkUpdate.NewerVirsion = string.Format("New Version {0} avilable." +
                        Environment.NewLine + "Do you want to update now ?", args.CurrentVersion);
                    checkUpdate.PageTitle = "Update Available";
                    checkUpdate.UpdateStatus = true;
                    checkUpdate.ShowDialog();
                }
                else
                {
                    checkUpdate.NewerVirsion = string.Format("There is no update avilable. \nPlease try again later.");
                    checkUpdate.PageTitle = "No update available";
                    checkUpdate.UpdateStatus = false;
                    checkUpdate.ShowDialog();
                }
            }
            else
            {
                MessageBox.Show(@"There is a problem reaching update server please check your internet connection and try again later.",
                       @"Update check failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ReleaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                obj = null;
                ShowMessageBox.Show("Exception Occured while releasing object " + ex.Message, "Error", enumMessageIcon.Error);
            }
            finally
            {
                GC.Collect();
            }
        }
    }
}
