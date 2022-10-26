using System;
using System.IO;
using System.Text;

namespace PrintEngine.DataGridPrint.SupportingClasses.Logging
{
    /// <summary>
    /// Do the actual log writing using setup info in Log Manager class
    /// </summary>
    class LogWriter
    {
        /// <summary>
        /// Create standard log file name with "our" name format
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private static String LogFileName(String name)
        {
            return String.Format("{0}_{1:yyyyMMdd}.Log", name, DateTime.Now);
        }

        /// <summary>
        /// Write the log entry to the file. Note that the log file is always flushed and closed. This
        /// will impact performance, but ensures that messages aren't lost
        /// </summary>
        /// <param name="from"></param>
        /// <param name="category"></param>
        /// <param name="msg"></param>
        /// <param name="path"></param>
        /// <param name="name"></param>
        public static void Write(String from, LogManager.Categories category, String msg, String path, String name)
        {
            StringBuilder line = new StringBuilder();
            line.Append(DateTime.Now.ToShortDateString().ToString());
            line.Append("-");
            line.Append(DateTime.Now.ToLongTimeString().ToString());
            line.Append(", ");
            line.Append(category.ToString().PadRight(6, ' '));
            line.Append(",");
            line.Append(from.PadRight(13, ' '));
            line.Append(",");
            line.Append(msg);
            StreamWriter w = new StreamWriter(path + "\\" + LogFileName(name), true);
            w.WriteLine(line.ToString());
            w.Flush();
            w.Close();
        }
    }
}
