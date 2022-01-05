namespace DBFReader
{
    partial class NewTable
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dgvTableNew = new System.Windows.Forms.DataGridView();
            this.FIELDNAME = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FIELDTYPE = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.FIELDLENGTH = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbtnVisualFoxpro = new System.Windows.Forms.RadioButton();
            this.rbtnFoxpro2x = new System.Windows.Forms.RadioButton();
            this.rbtndBaseIV = new System.Windows.Forms.RadioButton();
            this.rbtndBaseIII = new System.Windows.Forms.RadioButton();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTableNew)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvTableNew
            // 
            this.dgvTableNew.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dgvTableNew.ColumnHeadersHeight = 25;
            this.dgvTableNew.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvTableNew.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.FIELDNAME,
            this.FIELDTYPE,
            this.FIELDLENGTH});
            this.dgvTableNew.Location = new System.Drawing.Point(2, 28);
            this.dgvTableNew.Name = "dgvTableNew";
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvTableNew.RowHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvTableNew.RowHeadersWidth = 25;
            this.dgvTableNew.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgvTableNew.Size = new System.Drawing.Size(442, 369);
            this.dgvTableNew.TabIndex = 0;
            this.dgvTableNew.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.DgvTableNew_CellEndEdit);
            this.dgvTableNew.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.DgvTableNew_EditingControlShowing);
            this.dgvTableNew.RowValidating += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.DgvTableNew_RowValidating);
            // 
            // FIELDNAME
            // 
            this.FIELDNAME.HeaderText = "FIELD NAME";
            this.FIELDNAME.Name = "FIELDNAME";
            this.FIELDNAME.Width = 200;
            // 
            // FIELDTYPE
            // 
            this.FIELDTYPE.HeaderText = "FIELD TYPE";
            this.FIELDTYPE.Items.AddRange(new object[] {
            "Character",
            "Date",
            "Float",
            "Numeric",
            "Integer",
            "Logical"});
            this.FIELDTYPE.Name = "FIELDTYPE";
            this.FIELDTYPE.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.FIELDTYPE.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.FIELDTYPE.Width = 120;
            // 
            // FIELDLENGTH
            // 
            this.FIELDLENGTH.HeaderText = "LENGTH";
            this.FIELDLENGTH.MaxInputLength = 10;
            this.FIELDLENGTH.Name = "FIELDLENGTH";
            this.FIELDLENGTH.Width = 75;
            // 
            // btnSave
            // 
            this.btnSave.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSave.Location = new System.Drawing.Point(460, 275);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(101, 32);
            this.btnSave.TabIndex = 1;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.BtnSave_Click);
            // 
            // btnClose
            // 
            this.btnClose.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClose.Location = new System.Drawing.Point(460, 313);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(101, 32);
            this.btnClose.TabIndex = 2;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.BtnClose_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbtnVisualFoxpro);
            this.groupBox1.Controls.Add(this.rbtnFoxpro2x);
            this.groupBox1.Controls.Add(this.rbtndBaseIV);
            this.groupBox1.Controls.Add(this.rbtndBaseIII);
            this.groupBox1.Location = new System.Drawing.Point(447, 23);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(131, 140);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "File Type";
            // 
            // rbtnVisualFoxpro
            // 
            this.rbtnVisualFoxpro.AutoSize = true;
            this.rbtnVisualFoxpro.Location = new System.Drawing.Point(8, 109);
            this.rbtnVisualFoxpro.Name = "rbtnVisualFoxpro";
            this.rbtnVisualFoxpro.Size = new System.Drawing.Size(105, 21);
            this.rbtnVisualFoxpro.TabIndex = 3;
            this.rbtnVisualFoxpro.Text = "Visual Foxpro";
            this.rbtnVisualFoxpro.UseVisualStyleBackColor = true;
            // 
            // rbtnFoxpro2x
            // 
            this.rbtnFoxpro2x.AutoSize = true;
            this.rbtnFoxpro2x.Location = new System.Drawing.Point(8, 81);
            this.rbtnFoxpro2x.Name = "rbtnFoxpro2x";
            this.rbtnFoxpro2x.Size = new System.Drawing.Size(87, 21);
            this.rbtnFoxpro2x.TabIndex = 2;
            this.rbtnFoxpro2x.Text = "Foxpro 2.x";
            this.rbtnFoxpro2x.UseVisualStyleBackColor = true;
            // 
            // rbtndBaseIV
            // 
            this.rbtndBaseIV.AutoSize = true;
            this.rbtndBaseIV.Location = new System.Drawing.Point(8, 53);
            this.rbtndBaseIV.Name = "rbtndBaseIV";
            this.rbtndBaseIV.Size = new System.Drawing.Size(76, 21);
            this.rbtndBaseIV.TabIndex = 1;
            this.rbtndBaseIV.Text = "dBase IV";
            this.rbtndBaseIV.UseVisualStyleBackColor = true;
            // 
            // rbtndBaseIII
            // 
            this.rbtndBaseIII.AutoSize = true;
            this.rbtndBaseIII.Checked = true;
            this.rbtndBaseIII.Location = new System.Drawing.Point(8, 25);
            this.rbtndBaseIII.Name = "rbtndBaseIII";
            this.rbtndBaseIII.Size = new System.Drawing.Size(74, 21);
            this.rbtndBaseIII.TabIndex = 0;
            this.rbtndBaseIII.TabStop = true;
            this.rbtndBaseIII.Text = "dBase III";
            this.rbtndBaseIII.UseVisualStyleBackColor = true;
            // 
            // frmNewTable
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(580, 397);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.dgvTableNew);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "NewTable";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Table Structure - New";
            this.Load += new System.EventHandler(this.NewTable_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvTableNew)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvTableNew;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rbtnVisualFoxpro;
        private System.Windows.Forms.RadioButton rbtnFoxpro2x;
        private System.Windows.Forms.RadioButton rbtndBaseIV;
        private System.Windows.Forms.RadioButton rbtndBaseIII;
        private System.Windows.Forms.DataGridViewTextBoxColumn FIELDNAME;
        private System.Windows.Forms.DataGridViewComboBoxColumn FIELDTYPE;
        private System.Windows.Forms.DataGridViewTextBoxColumn FIELDLENGTH;
    }
}