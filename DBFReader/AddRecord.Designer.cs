namespace DBFReader
{
    partial class AddRecord
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.dgvAddRec = new System.Windows.Forms.DataGridView();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnImport = new System.Windows.Forms.Button();
            this.bgwAddRec = new System.ComponentModel.BackgroundWorker();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAddRec)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvAddRec
            // 
            this.dgvAddRec.AllowUserToAddRows = false;
            this.dgvAddRec.AllowUserToDeleteRows = false;
            this.dgvAddRec.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dgvAddRec.ColumnHeadersHeight = 25;
            this.dgvAddRec.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvAddRec.Location = new System.Drawing.Point(1, 12);
            this.dgvAddRec.Name = "dgvAddRec";
            this.dgvAddRec.RowHeadersWidth = 25;
            this.dgvAddRec.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgvAddRec.Size = new System.Drawing.Size(872, 256);
            this.dgvAddRec.TabIndex = 0;
            this.dgvAddRec.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DgvAddRec_CellClick);
            this.dgvAddRec.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.DgvAddRec_CellEndEdit);
            this.dgvAddRec.CellLeave += new System.Windows.Forms.DataGridViewCellEventHandler(this.DgvAddRec_CellLeave);
            this.dgvAddRec.CellMouseLeave += new System.Windows.Forms.DataGridViewCellEventHandler(this.DgvAddRec_CellMouseLeave);
            this.dgvAddRec.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.DgvAddRec_EditingControlShowing);
            this.dgvAddRec.RowValidating += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.DgvAddRec_RowValidating);
            this.dgvAddRec.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.DgvAddRec_KeyPress);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(440, 278);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(84, 28);
            this.btnCancel.TabIndex = 9;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.BtnCancel_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(352, 278);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(84, 28);
            this.btnAdd.TabIndex = 8;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.BtnAdd_Click);
            // 
            // btnImport
            // 
            this.btnImport.Location = new System.Drawing.Point(789, 278);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(84, 28);
            this.btnImport.TabIndex = 10;
            this.btnImport.Text = "Import";
            this.btnImport.UseVisualStyleBackColor = true;
            this.btnImport.Click += new System.EventHandler(this.BtnImport_Click);
            // 
            // bgwAddRec
            // 
            this.bgwAddRec.WorkerReportsProgress = true;
            this.bgwAddRec.WorkerSupportsCancellation = true;
            this.bgwAddRec.DoWork += new System.ComponentModel.DoWorkEventHandler(this.BgwAddRec_DoWork);
            this.bgwAddRec.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.BgwAddRec_ProgressChanged);
            this.bgwAddRec.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.BgwAddRec_RunWorkerCompleted);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(0, 1);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(873, 11);
            this.progressBar1.TabIndex = 11;
            // 
            // frmAddRecord
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(874, 315);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.btnImport);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.dgvAddRec);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AddRecord";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Add Record(s)";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AddRecord_FormClosing);
            this.Load += new System.EventHandler(this.AddRecord_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvAddRec)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvAddRec;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnImport;
        private System.ComponentModel.BackgroundWorker bgwAddRec;
        private System.Windows.Forms.ProgressBar progressBar1;
    }
}