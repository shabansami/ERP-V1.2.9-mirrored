using ERP.Desktop.Views._Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ERP.Desktop.Utilities.FormManager
{
    /// <summary>
    /// Handle the opening of forms, prevent opening the same form twice
    /// </summary>
    public static class FormManager
    {
        public static Form ParentForm;
        private readonly static Dictionary<Type, BaseForm> _forms = new Dictionary<Type, BaseForm>();

        public static T Show<T>(bool isAdmin = false, bool isTopLevel = false) where T : BaseForm, new()
        {
            var type = typeof(T);
            BaseForm f = null;
            if (_forms.TryGetValue(type, out f))
            {
                f.BringToFront();
            }
            else
            {
                f = new T();
                f.IsAdmin = isAdmin;
                f.TopLevel = isTopLevel;
                f.FormClosing += (s, e) => _forms.Remove(s.GetType());
                _forms.Add(type, f);
                if (!isTopLevel && ParentForm != null)
                {
                    f.MdiParent = ParentForm;
                }
                f.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
                    f.Show();
            }
            return (T)f;
        }
    }
}
