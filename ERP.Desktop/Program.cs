using ERP.Desktop.Views._Base.Main;
using ERP.Desktop.Views._Main;
using ERP.Desktop.Views._Main.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using NLog;
using System.Drawing.Imaging;
using System.IO;
using System.Drawing;
using ERP.Web.Utilites;
using ERP.DAL.Utilites;

namespace ERP.Desktop
{
    internal static class Program
    {
        #region Form switching Methods

        //Make an Application-context to switch the main form of the application
        private static ApplicationContext _mainContext = new ApplicationContext();

        /// <summary>
        /// Set the current main form of the application that when the user closes it, closes the application
        /// </summary>
        /// <param name="mainForm"></param>
        public static void SetMainForm(Form mainForm)
        {
            _mainContext.MainForm = mainForm;
        }

        /// <summary>
        /// Show the MainForm
        /// </summary>
        public static void ShowMainForm()
        {
            _mainContext.MainForm.Show();
        }

        #endregion
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() 
        {
            if (!CheckTimeTamper())
                return;

            #region Setup NLog when not debug
#if DEBUG
#else
// Add the event handler for handling UI thread exceptions to the event.
            Application.ThreadException += (ss, e) => { Log(e.Exception); };
            // Set the unhandled exception mode to force all Windows Forms errors
            // to go through our handler.
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            // Add the event handler for handling non-UI thread exceptions to the event. 
            AppDomain.CurrentDomain.UnhandledException += (ss, e) => { Log((Exception)e.ExceptionObject); };

#endif
            #endregion

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            _mainContext.MainForm = new LoginScreen();
            Application.Run(_mainContext);
        }

        #region NLog
        /// <summary>
        /// log an exception error
        /// </summary>
        /// <param name="exception">exception</param>
        internal static void Log(Exception exception)
        {
            Rectangle bounds = Screen.GetBounds(Point.Empty);
            using (Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height))
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.CopyFromScreen(Point.Empty, Point.Empty, bounds.Size);
                }
                var path = Environment.CurrentDirectory + "/logs/" + DateTime.Now.ToString("yyyy-MM-dd");
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                bitmap.Save(path + DateTime.Now.ToString("/HH-mm-ss.fff") + ".jpg", ImageFormat.Jpeg);
            }
            LogManager.GetCurrentClassLogger().Error(exception);
            MessageBox.Show("لم تتم العملية \n الرجاء المحاولة مرة أخرى او التواصل مع فريق الدعم الفني 01020200504", "حدث خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign);

        }
        /// <summary>
        /// Log string with custom loglevel
        /// </summary>
        /// <param name="log">log string</param>
        /// <param name="logLevel">log level</param>
        internal static void Log(string log, LogLevel logLevel)
        {

            Rectangle bounds = Screen.GetBounds(Point.Empty);
            using (Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height))
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.CopyFromScreen(Point.Empty, Point.Empty, bounds.Size);
                }
                var path = Environment.CurrentDirectory + "/logs/" + DateTime.Now.ToString("yyyy-MM-dd");
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                bitmap.Save(path + DateTime.Now.ToString("/HH-mm-ss.fff") + ".jpg", ImageFormat.Jpeg);
            }
            LogManager.GetCurrentClassLogger().Log(logLevel, log);
            MessageBox.Show("لم تتم العملية \n الرجاء المحاولة مرة أخرى او التواصل مع فريق الدعم الفني 01020200504", "حدث خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign);

        }
        /// <summary>
        /// Log string with error logLevel
        /// </summary>
        /// <param name="log">log string</param>
        internal static void Log(string log)
        {

            Rectangle bounds = Screen.GetBounds(Point.Empty);
            using (Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height))
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.CopyFromScreen(Point.Empty, Point.Empty, bounds.Size);
                }
                var path = Environment.CurrentDirectory + "/logs/" + DateTime.Now.ToString("yyyy-MM-dd");
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                bitmap.Save(path + DateTime.Now.ToString("/HH-mm-ss.fff") + ".jpg", ImageFormat.Jpeg);
            }
            LogManager.GetCurrentClassLogger().Log(LogLevel.Error, log);
            MessageBox.Show("لم تتم العملية \n الرجاء المحاولة مرة أخرى او التواصل مع فريق الدعم الفني 01020200504", "حدث خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign);

        }
        #endregion

        #region Validiate Time tamper
        private static bool ValidiateTimeTamper()
        {
            //First Initialization
            if (string.IsNullOrWhiteSpace(Properties.Settings.Default.FirstInit))
            {
                Properties.Settings.Default.FirstInit = VTSAuth.Encrypt(DateTime.Now.ToString());
                Properties.Settings.Default.LastInit = VTSAuth.Encrypt(DateTime.Now.AddMilliseconds(500).ToString());
                Properties.Settings.Default.Save();
                return true;
            }

            //if not first time
            if (DateTime.TryParse(VTSAuth.Decrypt(Properties.Settings.Default.FirstInit), out DateTime firstInit))
            {
                if (DateTime.TryParse(VTSAuth.Decrypt(Properties.Settings.Default.LastInit), out DateTime lastiInit))
                {
                    //check if there is a 
                    if (lastiInit > DateTime.Now)
                    {
                        Properties.Settings.Default.LastInit = VTSAuth.Encrypt("not valid DateTime");
                        Properties.Settings.Default.Save();
                        return false;
                    }

                    Properties.Settings.Default.LastInit = VTSAuth.Encrypt(DateTime.Now.ToString());
                    Properties.Settings.Default.Save();
                    return true;
                }
            }
            return false;
        }
        public static bool CheckTimeTamper()
        {
            var result = ValidiateTimeTamper();
            if (!result)
            {
                MessageBox.Show("تم التلاعب في الوقت و التاريخ الرجاء الاتصال بالدعم الفني", "الوقت و التريخ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
            return result;
        }
        #endregion
    }
}
