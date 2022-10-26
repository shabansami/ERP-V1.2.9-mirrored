using System;
using System.Diagnostics;

namespace PrintEngine.DataGridPrint.SupportingClasses.Logging
{
    /// <summary>
    /// Setup and implements logs for internal logging
    /// </summary>
    class LogManager
    {
        /// <summary>
        /// Path to log file
        /// </summary>
        private String basepath;
        public String BasePath
        {
            get { return basepath; }
            set { basepath = value; }
        }

        /// <summary>
        /// Header for log file name
        /// </summary>
        private String logheader;
        public String LogNameHeader
        {
            get { return logheader; }
            set { logheader = value; }
        }

        private int useFrame = 1;

        /// <summary>
        /// Define logging message categories
        /// </summary>
        public enum Categories
        {
            Info = 1,
            Warning,
            Error,
            Exception
        }

        /// <summary>
        /// Constructor, allow user to override path and name of logging file
        /// </summary>
        /// <param name="userbasepath"></param>
        /// <param name="userlogname"></param>
        public LogManager(String userbasepath, String userlogname)
        {
            BasePath = String.IsNullOrEmpty(userbasepath) ? "." : userbasepath;
            LogNameHeader = String.IsNullOrEmpty(userlogname) ? "MsgLog" : userlogname;

            Log(Categories.Info, "********************* New Trace *********************");
        }

        /// <summary>
        /// Log a message, using the provided category
        /// </summary>
        /// <param name="category"></param>
        /// <param name="msg"></param>
        public void Log(Categories category, String msg)
        {
            // get call stack
            StackTrace stackTrace = new StackTrace();

            // get calling method name
            String caller = stackTrace.GetFrame(useFrame).GetMethod().Name;

            // log it
            LogWriter.Write(caller, category, msg, BasePath, LogNameHeader);

            // reset frame pointer
            useFrame = 1;
        }

        /// <summary>
        /// Log an informational message
        /// </summary>
        /// <param name="msg"></param>
        public void LogInfoMsg(String msg)
        {
            useFrame++; // bump up the stack frame pointer to skip this entry
            Log(Categories.Info, msg);
        }

        /// <summary>
        /// Log an error message
        /// </summary>
        /// <param name="msg"></param>
        public void LogErrorMsg(String msg)
        {
            useFrame++; // bump up the stack frame pointer to skip this entry
            Log(Categories.Error, msg);
        }

        /// <summary>
        /// Log an exception
        /// </summary>
        /// <param name="ex"></param>
        public void Log(Exception ex)
        {
            useFrame++; // bump up the stack frame pointer to skip this entry
            Log(Categories.Exception, String.Format("{0} from {1}", ex.Message, ex.Source));
        }
    }
}
