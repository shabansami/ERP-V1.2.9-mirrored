using System;
using System.Threading;

namespace ERP.Desktop.Utilities
{
    /// <summary>
    /// Make a delay when typing in a text box to prevent stuttering when searching
    /// </summary>
    public class TypeAssistant
    {
        public event EventHandler Idled = delegate { };
        public int WaitingMilliSeconds { get; set; }
        Timer waitingTimer;

        /// <summary>
        /// Wait sometime before continuting the text changed event
        /// </summary>
        /// <param name="waitingMilliSeconds">The time to wait</param>
        public TypeAssistant(int waitingMilliSeconds = 500)
        {
            WaitingMilliSeconds = waitingMilliSeconds;
            waitingTimer = new Timer(p =>
            {
                Idled(this, EventArgs.Empty);
            });
        }

        public void TextChanged()
        {
            waitingTimer.Change(WaitingMilliSeconds, System.Threading.Timeout.Infinite);
        }
    }
}
