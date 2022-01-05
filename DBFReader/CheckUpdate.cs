using System;
using System.Reflection;
using System.Windows.Forms;
using AutoUpdate;

namespace DBFReader
{
    partial class CheckUpdate : Form
    {
        public string PageTitle, CurrentVersion, NewerVirsion;
        public bool UpdateStatus = true;

        public CheckUpdate()
        {
            InitializeComponent();
        }

        public string AssemblyProduct
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyProductAttribute)attributes[0]).Product;
            }
        }

        private void CheckUpdate_Load(object sender, EventArgs e)
        {
            this.Text = String.Format(AssemblyProduct + ": {0}", PageTitle);
            this.labelProductName.Text = AssemblyProduct;
            this.labelVersion.Text = String.Format("{0}", CurrentVersion);
            this.labelNewVersion.Text = NewerVirsion;
            if (UpdateStatus == false)
            {
                btnYes.Hide();
                btnNo.Text = "OK";
            }
        }

        private void BtnYes_Click(object sender, EventArgs e)
        {
            try
            {
                AutoUpdater.DownloadUpdate();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, exception.GetType().ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnNo_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
