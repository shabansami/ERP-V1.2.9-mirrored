using System.Drawing.Printing;
using System.Linq;
using System.Management;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PrintEngine.HTMLPrint.Printing
{
    /// <summary>
    /// A class primarly used to print silently to a printer  
    /// </summary>
    internal static class PrintManagement
    {
        #region Print Dialogs

        /// <summary>
        /// Switch between the dialog options and show the chosen one
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="htmlDocument"></param>
        internal static void ShowChosenPrintDialog(PrintOperation operation, string htmlDocument)
        {
            if (Application.OpenForms.Count <= 0)
                return;

            //If not open make a new one
            Control.ControlCollection controls = Application.OpenForms[0].Controls;
            WebBrowser browser = new WebBrowser();
            browser.Name = "VTSBrowser";
            browser.ScriptErrorsSuppressed = true;
            browser.Visible = false;

            controls.Add((Control)browser);

            //Show the needed form when the document generation is done
            browser.DocumentCompleted += (s, e) =>
            {
                //Check if the user wants to open the Print Preview page
                switch (operation)
                {
                    case PrintOperation.Preview:
                        browser.ShowPrintPreviewDialog();
                        break;

                    case PrintOperation.ShowPrint:
                        browser.ShowPrintDialog();
                        break;
                    case PrintOperation.Setup:
                        browser.ShowPageSetupDialog();
                        break;
                    case PrintOperation.Save:
                        browser.ShowSaveAsDialog();
                        break;
                    default:
                        return;
                }

                //Dispose the browser after opening the dialog
                ((WebBrowser)s).Dispose();
            };

            //Set the current page of the browser to the html document we generated
            browser.DocumentText = htmlDocument;
        }


        #endregion

        #region Silent Print

        /// <summary>
        /// Print silently to specified printer
        /// </summary>
        /// <param name="chosenPrinter">The printer to print to</param>
        /// <param name="html">the html page to print</param>
        /// <param name="copies">Number of copies</param>
        internal static void Print(string chosenPrinter, string html, int copies = 1, bool isOpenSetting = false)
        {
            //Get installed Printers
            PrinterSettings.StringCollection installedPrinters = PrinterSettings.InstalledPrinters;

            //Loop through the lsit of printers and choose the passed printer name
            foreach (var printer in installedPrinters.Cast<string>().Where(chosenPrinter.Equals))
            {
                //Change the system defult printer, we can't print directly to a printer silently because of security concerns in windows
                SetDefaultPrinter(printer);

                //Instantitate a new webbrowser control
                var wb = new WebBrowser();
                wb.DocumentText = html;

                //The WebBrowser instance will continue to print to the previously defined printer even after the system default is altered.
                //To work around that, registering a handler to the PrintTemplateTeardown event by accessing the underlying ActiveX object
                //of the managed WebBrowser instance and waiting for the event to fire before printing further documents
                // Add refrence to Microsoft Internet Controls
                var wbax = (SHDocVw.WebBrowser)wb.ActiveXInstance;

                TaskCompletionSource<bool> printedTcs = null;
                SHDocVw.DWebBrowserEvents2_PrintTemplateTeardownEventHandler printTemplateTeardownHandler =
                    (p) => printedTcs.TrySetResult(true); // turn event into awaitable task

                printedTcs = new TaskCompletionSource<bool>();
                wbax.PrintTemplateTeardown += printTemplateTeardownHandler;

                try
                {
                    //WebBrowser requires an STA thread with an active message loop; by calling printedTcs.Task.Wait() from a form UI
                    //, we are blocking the STA thread and the application hangs waiting for an event that is never fired. 
                    //To fix that, we can use a Messege loop handler extension method, or simply call Application.DoEvents()
                    Application.DoEvents();
                    if (isOpenSetting)
                        wb.ShowPageSetupDialog();
                    //Loop through the number of copies requested for printing
                    for (int i = 0; i < copies; i++)
                    {
                        wb.Print();
                        printedTcs.Task.Wait();
                    }

                    // Allow some time for the last document to execute, then run what ever code you want
                    Thread.Sleep(2000);
                }

                finally
                {
                    //Remove the printTempleateTeardown that we used with the ActiveXInstane before
                    wbax.PrintTemplateTeardown -= printTemplateTeardownHandler;
                }

                //Disose the web browser control
                wb.Dispose();
            }

        }

        /// <summary>
        /// Change the default printer of the system to be able to print silently to it.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private static bool SetDefaultPrinter(string name)
        {
            // With credits to Austin Salonen <https://stackoverflow.com/users/4068/austin-salonen>
            // From: https://stackoverflow.com/a/714543/3258851
            // CC BY-SA 3.0
            using (ManagementObjectSearcher objectSearcher = new ManagementObjectSearcher("SELECT * FROM Win32_Printer"))
            {
                using (ManagementObjectCollection objectCollection = objectSearcher.Get())
                {
                    foreach (ManagementObject mo in objectCollection)
                    {
                        if (string.Compare(mo["Name"].ToString(), name, true) == 0)
                        {
                            mo.InvokeMethod("SetDefaultPrinter", null);
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        #endregion
    }
}