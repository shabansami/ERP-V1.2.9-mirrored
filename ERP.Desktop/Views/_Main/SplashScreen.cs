using System.Threading;
using System.Windows.Forms;

namespace ERP.Desktop.Views._Main
{
    /// <Author>
    /// By: Mohamed Ali
    /// </Author>
    public partial class SplashScreen : Form
    {
        //Delegate for cross thread call to close
        private delegate void CloseDelegate();

        //The type of form to be displayed as the splash screen.
        private static SplashScreen splashForm;

        static public void ShowSplashScreen()
        {
            // Make sure it is only launched once.    
            if (splashForm != null) return;
            splashForm = new SplashScreen();
            Thread thread = new Thread(new ThreadStart(SplashScreen.ShowForm));
            thread.IsBackground = true;
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        static private void ShowForm()
        {
            if (splashForm != null) Application.Run(splashForm);
        }

        static public void CloseSplashScreen()
        {
            splashForm?.Invoke(new CloseDelegate(SplashScreen.CloseFormInternal));
        }

        static private void CloseFormInternal()
        {
            if (splashForm != null)
            {
                splashForm.Close();
                splashForm = null;
            };
        }

        public SplashScreen()
        {
            InitializeComponent();
        }
    } 
}
