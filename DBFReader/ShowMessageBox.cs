using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.IO;
using System.Collections.Generic;

namespace DBFReader
{
    public partial class ShowMessageBox : Form
    {
        public ShowMessageBox()
        {
            InitializeComponent();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Rectangle rectangle = this.ClientRectangle;
            LinearGradientBrush linearGradientBrush = new LinearGradientBrush(rectangle, Color.SkyBlue, Color.AliceBlue, 60);
            e.Graphics.FillRectangle(linearGradientBrush, rectangle);
            base.OnPaint(e);
        }

        private void SetMessage(string messageText)
        {
            int number = Math.Abs(messageText.Length / 50);
            if (number != 0)
                this.lblMessageText.Height = number * 25;
            this.lblMessageText.Text = messageText;
        }

        private void AddButton(enumMessageButton MessageButton)
        {
            switch (MessageButton)
            {
                case enumMessageButton.OK:
                    {
                        Button btnOk = new Button();
                        btnOk.Text = "OK";
                        btnOk.DialogResult = DialogResult.OK;
                        btnOk.FlatStyle = FlatStyle.Popup;
                        btnOk.FlatAppearance.BorderSize = 0;
                        btnOk.SetBounds(pnlShowMessage.ClientSize.Width - 80, 5, 75, 25);
                        pnlShowMessage.Controls.Add(btnOk);
                    }
                    break;
                case enumMessageButton.OKCancel:
                    {
                        Button btnOk = new Button();
                        btnOk.Text = "OK";
                        btnOk.DialogResult = DialogResult.OK;
                        btnOk.FlatStyle = FlatStyle.Popup;
                        btnOk.FlatAppearance.BorderSize = 0;
                        btnOk.SetBounds((pnlShowMessage.ClientSize.Width - 70), 5, 65, 25);
                        pnlShowMessage.Controls.Add(btnOk);

                        Button btnCancel = new Button();
                        btnCancel.Text = "Cancel";
                        btnCancel.DialogResult = DialogResult.Cancel;
                        btnCancel.FlatStyle = FlatStyle.Popup;
                        btnCancel.FlatAppearance.BorderSize = 0;
                        btnCancel.SetBounds((pnlShowMessage.ClientSize.Width - (btnOk.ClientSize.Width + 5 + 80)), 5, 75, 25);
                        pnlShowMessage.Controls.Add(btnCancel);

                    }
                    break;
                case enumMessageButton.YesNo:
                    {
                        Button btnNo = new Button();
                        btnNo.Text = "No";
                        btnNo.DialogResult = DialogResult.No;
                        btnNo.FlatStyle = FlatStyle.Popup;
                        btnNo.FlatAppearance.BorderSize = 0;
                        btnNo.SetBounds((pnlShowMessage.ClientSize.Width - 70), 5, 65, 25);
                        pnlShowMessage.Controls.Add(btnNo);

                        Button btnYes = new Button();
                        btnYes.Text = "Yes";
                        btnYes.DialogResult = DialogResult.Yes;
                        btnYes.FlatStyle = FlatStyle.Popup;
                        btnYes.FlatAppearance.BorderSize = 0;
                        btnYes.SetBounds((pnlShowMessage.ClientSize.Width - (btnNo.ClientSize.Width + 5 + 80)), 5, 75, 25);
                        pnlShowMessage.Controls.Add(btnYes);
                    }
                    break;
                case enumMessageButton.YesNoCancel:
                    {
                        Button btnCancel = new Button();
                        btnCancel.Text = "Cancel";
                        btnCancel.DialogResult = DialogResult.Cancel;
                        btnCancel.FlatStyle = FlatStyle.Popup;
                        btnCancel.FlatAppearance.BorderSize = 0;
                        btnCancel.SetBounds((pnlShowMessage.ClientSize.Width - 70), 5, 65, 25);
                        pnlShowMessage.Controls.Add(btnCancel);

                        Button btnNo = new Button();
                        btnNo.Text = "No";
                        btnNo.DialogResult = DialogResult.No;
                        btnNo.FlatStyle = FlatStyle.Popup;
                        btnNo.FlatAppearance.BorderSize = 0;
                        btnNo.SetBounds((pnlShowMessage.ClientSize.Width - (btnCancel.ClientSize.Width + 5 + 80)), 5, 75, 25);
                        pnlShowMessage.Controls.Add(btnNo);

                        Button btnYes = new Button();
                        btnYes.Text = "Yes";
                        btnYes.DialogResult = DialogResult.No;
                        btnYes.FlatStyle = FlatStyle.Popup;
                        btnYes.FlatAppearance.BorderSize = 0;
                        btnYes.SetBounds((pnlShowMessage.ClientSize.Width - (btnCancel.ClientSize.Width + btnNo.ClientSize.Width + 10 + 80)), 5, 75, 25);
                        pnlShowMessage.Controls.Add(btnYes);
                    }
                    break;
            }
        }

        private void AddIconImage(enumMessageIcon MessageIcon)
        {
            switch (MessageIcon)
            {
                case enumMessageIcon.Error:
                    pictureBox1.Image = imageList1.Images["Error.png"];
                    break;
                case enumMessageIcon.Information:
                    pictureBox1.Image = imageList1.Images["Info.png"];
                    break;
                case enumMessageIcon.Question:
                    pictureBox1.Image = imageList1.Images["Question.png"];
                    break;
                case enumMessageIcon.Warning:
                    pictureBox1.Image = imageList1.Images["Warning.png"];
                    break;
            }
        }

        #region Overloaded Show message to display message box.

        internal static void Show(string messageText)
        {
            ShowMessageBox showMessageBox = new ShowMessageBox();
            showMessageBox.SetMessage(messageText);
            showMessageBox.AddIconImage(enumMessageIcon.Information);
            showMessageBox.AddButton(enumMessageButton.OK);
            showMessageBox.Text = "DBF Reader : Information";
            showMessageBox.lblHeader.Text = "Info";
            showMessageBox.ShowDialog();
            showMessageBox.BringToFront();
        }

        internal static void Show(string messageText, string messageTitle)
        {
            ShowMessageBox showMessageBox = new ShowMessageBox();
            showMessageBox.Text = "DBF Reader : " + messageTitle;
            showMessageBox.SetMessage(messageText);
            showMessageBox.AddIconImage(enumMessageIcon.Information);
            showMessageBox.AddButton(enumMessageButton.OK);
            showMessageBox.lblHeader.Text = messageTitle;
            showMessageBox.ShowDialog();
            showMessageBox.BringToFront();
        }

        internal static void Show(string messageText, string messageTitle, enumMessageIcon messageIcon)
        {
            ShowMessageBox showMessageBox = new ShowMessageBox();
            showMessageBox.SetMessage(messageText);
            showMessageBox.Text = "DBF Reader : " + messageTitle;
            showMessageBox.AddIconImage(messageIcon);
            showMessageBox.AddButton(enumMessageButton.OK);
            showMessageBox.lblHeader.Text = messageTitle;
            showMessageBox.ShowDialog();
            showMessageBox.BringToFront();
        }

        internal static void Show(string messageText, string messageTitle, enumMessageIcon messageIcon, enumMessageButton messageButton)
        {
            ShowMessageBox showMessageBox = new ShowMessageBox();
            showMessageBox.SetMessage(messageText);
            showMessageBox.Text = "DBF Reader : " + messageTitle;
            showMessageBox.AddIconImage(messageIcon);
            showMessageBox.AddButton(messageButton);
            showMessageBox.lblHeader.Text = messageTitle;
            showMessageBox.ShowDialog();
            showMessageBox.BringToFront();
        }

        #endregion

        private void LblMessageText_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(lblMessageText.Text);
        }
    }

    #region constant defiend in form of enumration which is used in showMessage class.

    internal enum enumMessageIcon
    {
        Error,
        Warning,
        Information,
        Question,
    }

    internal enum enumMessageButton
    {
        OK,
        YesNo,
        YesNoCancel,
        OKCancel
    }

    #endregion
}
