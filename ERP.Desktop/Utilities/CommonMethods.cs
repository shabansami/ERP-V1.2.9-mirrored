using ERP.Desktop.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ERP.Desktop.Utilities
{
    public static class CommonMethods
    {
        public static void Fill<T>(this ComboBox cmbo, List<T> list, string displayMem, string valueMem) where T : new()
        {
            FillComboBox(cmbo, list, displayMem, valueMem);
        }
        public static void FillComboBox<T>(ComboBox cmbo, List<T> list, string displayMem, string valueMem) where T : new()
        {
            if (cmbo == null)
                return;

            if(list == null)
                list = new List<T>();

            T defaultSelect = new T();
            var propVal = defaultSelect.GetType().GetProperty(valueMem);
            if (propVal != null)
            {
                propVal.SetValue(defaultSelect, default);
            }

            var propMem = defaultSelect.GetType().GetProperty(displayMem);
            if (propMem != null)
            {
                propMem.SetValue(defaultSelect, "--اختر--");

                //adding auto complete
                AutoCompleteStringCollection com = new AutoCompleteStringCollection();
                for (int i = 0; i < list.Count; i++)
                {
                    com.Add(propMem.GetValue(list[i]) + "");
                }

                // Running on the UI thread
                cmbo.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                cmbo.AutoCompleteSource = AutoCompleteSource.CustomSource;
                cmbo.AutoCompleteCustomSource = com;
            }

            list.Insert(0, defaultSelect);

            cmbo.DataSource = list;
            cmbo.DisplayMember = displayMem;
            cmbo.ValueMember = valueMem;
            if (list.Count == 2)
                cmbo.SelectedIndex = 0;
        }

        public static Keys GetKeyEnum(int key) => GetEnum<Keys>(key);
        public static T GetEnum<T>(int key) => (T)Enum.ToObject(typeof(T), key);

        public static DateTime TimeNow => DateTime.UtcNow.AddHours(2);

        public static void AutoComplateTextbox(TextBox Key)
        {
            AutoCompleteStringCollection com = new AutoCompleteStringCollection();
            var db = DBContext.UnitDbContext;
            var dt = db.Items.Where(x => x.IsDeleted == false).Select(x => x.Name.Trim()).ToList();

            for (int i = 0; i < dt.Count; i++)
            {
                com.Add(dt[i].ToString());
            }

            Key.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            Key.AutoCompleteSource = AutoCompleteSource.CustomSource;
            Key.AutoCompleteCustomSource = com;
        }

        public static List<string> GetPrinters()
        {
           List<string> printers = new List<string>();
            foreach (string printer in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
            {
                printers.Add(printer);
            }
            return printers;
        }


        public static string GenerateRandomNumber()
        {
            Random random = new Random();
            string r = "";
            for (int i = 1; i < 11; i++)
            {
                r += random.Next(0, 9).ToString();
            }
            return r;
        }
    }
}
