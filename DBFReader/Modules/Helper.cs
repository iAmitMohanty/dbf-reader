using System;
using System.IO;

namespace DBFReader
{
    class Helper
    {
        // You can uese url like: http://mywebsite.com/checkupdates/checkupdate.xml
        public static string checkPath = @"E:\AMIT\Study\DBFReader\CheckUpdate\checkUpdate.xml";

        public static void WriteToFile(string text)
        {
            try
            {
                string logFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "DBFReader");
                string logFilePath = Path.Combine(logFolderPath, "ServiceLog-" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt");

                if (!Directory.Exists(logFolderPath))
                    Directory.CreateDirectory(logFolderPath);

                if (!File.Exists(logFilePath))
                    File.Create(logFilePath).Dispose();

                using (StreamWriter writer = new StreamWriter(logFilePath, true))
                {
                    writer.WriteLine(string.Format("{0} : " + text, DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt")));
                    writer.Close();
                }

                if (Directory.GetFiles(logFolderPath, "*.txt").Length > 30)
                {
                    string[] files = Directory.GetFiles(logFolderPath, "*.txt");
                    foreach (string file in files)
                    {
                        FileInfo info = new FileInfo(file);
                        info.Refresh();
                        if (info.CreationTime <= DateTime.Now.AddDays(-15))
                        {
                            info.Delete();
                        }
                    }
                }
            }
            catch
            {

            }
        }
    }
}
