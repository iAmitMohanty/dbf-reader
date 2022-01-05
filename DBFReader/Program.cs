using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace DBFReader
{
    static class Program
    {
        private static Mutex mutex = null;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ReaderMaster());

            //Process.GetCurrentProcess().SingleInstance();
            //bool first = false;
            //mutex = new Mutex(true, Application.ProductName.ToString(), out first);
            //if (first)
            //{
            //    Application.EnableVisualStyles();
            //    Application.SetCompatibleTextRenderingDefault(false);
            //    Application.Run(new ReaderMaster());
            //}
            //else
            //{
            //    MessageBox.Show("Application " + Application.ProductName.ToString() + " already running");
            //}
        }

        public static void SingleInstance(this Process thisProcess)
        {
            foreach (Process proc in Process.GetProcessesByName(thisProcess.ProcessName))
            {
                if (proc.Id != thisProcess.Id)
                {
                    proc.Kill();
                }
            }
        }
    }
}
