using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ERP.Desktop.Utilities
{
    public static class Extensions
    {
        #region Null Saftey

        /// <summary>
        /// An extension method that converts the string to int or return defaultValue if null or not int
        /// </summary>
        /// <param name="s">the string to check</param>
        /// <param name="defaultToReturn">The defaultValue to return if not int</param>
        /// <returns>int number</returns>
        public static int ToInt(this string s, int defaultToReturn = 0)
        {
            int result;

            //If the string is null return the default value which is 0
            if (s == null)
                return defaultToReturn;

            //IF it's not null try parsing the string for int value
            else if (int.TryParse(s, out result))
                return result;

            //If it's not int return defult value
            else
                return defaultToReturn;
        }

        /// <summary>
        /// An extension method that converts the object to int or return defaultValue if null or not int
        /// </summary>
        /// <param name="s">the string to check</param>
        /// <param name="defaultToReturn">The defaultValue to return if not int</param>
        /// <returns>int number</returns>
        public static int ToInt(this object o, int defaultToReturn = 0)
        {
            int result;

            //If the string is null return the default value which is 0
            if (o == null)
                return defaultToReturn;

            //IF it's not null try parsing the string for int value
            else if (int.TryParse(o.ToString(), out result))
                return result;

            //If it's not int return defult value
            else
                return defaultToReturn;
        }

        /// <summary>
        /// Returns 0 if DataGridViewCell value is null, returns the value if not
        /// </summary>
        /// <param name="cell">The cell to check</param>
        /// <returns></returns>
        public static bool ToBool(this DataGridViewCell cell)
        {
            bool result;

            if (cell.Value == null)
                return false;
            else if (bool.TryParse(cell.Value.ToString(), out result))
                return result;
            else
                return false;
        }

        /// <summary>
        /// Converts a string to DateTime with null checks
        /// </summary>
        /// <param name="source">the string to convert</param>
        /// <returns></returns>
        public static string ToTime(this object o)
        {
            //If the string is null return the default value which is 0
            if (o == null)
                return "00";
            else
                return o.ToString();
        }

        #endregion

        #region Time Conversion

        /// <summary>
        /// Convert a timespan to 12hour format with arabic culture for designator
        /// ex: 03:50 ص
        /// </summary>
        /// <param name="timeSpan">The timespan to convert</param>
        /// <returns>Time in 12-hours format</returns>
        public static string ToShortTime(this TimeSpan timeSpan)
        {
            return new DateTime().Add(timeSpan).ToString("HH:mm tt", CultureInfo.CreateSpecificCulture("ar-EG"));
        }

        /// <summary>
        /// The same method as ToShortTime(this TimeSpan timeSpan)
        /// but with null saftey
        /// </summary>
        /// <param name="timeSpan"></param>
        /// <returns></returns>
        public static string ToShortTime(this TimeSpan? timeSpan)
        {
            return timeSpan == null ? string.Empty : timeSpan.Value.ToShortTime();
        }

        #endregion

        #region Date Utilities

        /// <summary>
        /// Gets the date of specific day in a date range
        /// ex: Today is Monday: 26/08/2019, next monday is :9/2/2019, etc
        /// </summary>
        /// <param name="from">The start date</param>
        /// <param name="to">The end date</param>
        /// <param name="day">The day in week</param>
        /// <returns>List of dates</returns>
        public static List<DateTime> GetWeekdayInRange(this DateTime from, DateTime to, int day)
        {
            const int daysInWeek = 7;

            var dayOfDate = (int)@from.DayOfWeek;

            var result = new List<DateTime>();
            var daysToAdd = (day - dayOfDate + daysInWeek) % daysInWeek;

            do
            {
                from = from.AddDays(daysToAdd);
                result.Add(from);
                daysToAdd = daysInWeek;
            } while (from < to);

            return result;
        }

        #endregion

        #region LINQ

        /// <summary>
        /// Select All columns and distinct them by specific column
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>
            (this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            var seenKeys = new HashSet<TKey>();
            foreach (var element in source)
                if (!seenKeys.Contains(keySelector(element)))
                {
                    seenKeys.Add(keySelector(element));
                    yield return element;
                }
        }

        /// <summary>
        /// Convert a list to a datatable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static DataTable ToDataTable<T>(this IList<T> data)
        {
            if(data.Count()==0)
                return new DataTable();
            var properties = TypeDescriptor.GetProperties(data.First().GetType());
            var table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);

            foreach (var item in data)
            {
                var row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }

            return table;
        }

        #endregion

        #region Controls
        public static int GetSelectedID(this ComboBox comboBox, int defaultValue = 0)
        {
            if (comboBox == null || comboBox.Items.Count == 0 || comboBox.SelectedIndex < 1)
                return defaultValue;
            if (int.TryParse(comboBox.SelectedValue + "", out int selectedID))
            {
                return selectedID;
            }
            return defaultValue;
        }
        public static Guid? GetSelectedID(this ComboBox comboBox, Guid? defaultValue = null)
        {
            if (comboBox == null || comboBox.Items.Count == 0 || comboBox.SelectedIndex < 1)
                return defaultValue;
            if (Guid.TryParse(comboBox.SelectedValue + "", out Guid selectedID))
            {
                return selectedID;
            }
            return defaultValue;
        }
        public static int? GetSelectedIntID(this ComboBox comboBox, int? defaultValue = null)
        {
            if (comboBox == null || comboBox.Items.Count == 0 || comboBox.SelectedIndex < 1)
                return defaultValue;
            if (int.TryParse(comboBox.SelectedValue + "", out int selectedID))
            {
                return selectedID;
            }
            return defaultValue;
        }

        #endregion


    }
}
