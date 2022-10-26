using ERP.Desktop.Utilities.ReusableControls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ERP.Desktop.Views._Main
{
    public partial class BaseForm : Form
    {
        public bool IsAdmin { get; internal set; }

        public BaseForm()
        {
            //Design
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            RightToLeft = RightToLeft.Yes;
            RightToLeftLayout = true;
            AutoScaleMode = AutoScaleMode.None;
            Font = new Font("Tahoma", 9.75F, FontStyle.Regular, GraphicsUnit.Point, (byte)0);

            //Prevent maximizing child forms when double clicking the title bar
            MaximizeBox = false;
            TopLevel = true;
        }

        #region Keyboard Shortcuts

        /// <summary>
        /// A windows forms method that checks what key did the user press in this active form
        /// </summary>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            //Close the form when clicking the ESC button
            if (keyData == Keys.Escape)
            {
                Close();
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        #endregion

        #region Reports

        /// <summary>
        /// A methods that will be overriden in each form to impement the report printing
        /// </summary>
        /// <param name="isPreviewDialog"></param>
        protected virtual void PrintReport(bool isPreviewDialog = false)
        {
        }

        #endregion

        #region Styling

        public void StyleDataGridViews(params DataGridView[] dataGrids)
        {
            //Loop through the datagrids provided
            foreach (var dataGrid in dataGrids)
            {
                //We set the AutoSizeColumn to None and change it later becuase of a bug in DataGrid
                //AutoResizeColumns gives operation exception in some cases. This exception however is not reproducible in .Net 5
                dataGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
                dataGrid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(238, 239, 249);

                dataGrid.ColumnHeadersDefaultCellStyle.BackColor = Color.Teal;
                dataGrid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;

                dataGrid.DefaultCellStyle.SelectionBackColor = Color.LightBlue;
                dataGrid.DefaultCellStyle.SelectionForeColor = Color.Black;
                dataGrid.BackgroundColor = Color.FromArgb(238, 239, 249);

                //Change the style of RowHeader cells الرقم المسلسل

                dataGrid.TopLeftHeaderCell.Value = "م";
                dataGrid.TopLeftHeaderCell.Style.BackColor = Color.Teal;
                dataGrid.TopLeftHeaderCell.Style.ForeColor = Color.White;
                dataGrid.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
                dataGrid.RowHeadersDefaultCellStyle.BackColor = Color.Teal;

                dataGrid.BorderStyle = BorderStyle.None;
                dataGrid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;

                dataGrid.EnableHeadersVisualStyles = false;
                dataGrid.MultiSelect = false;

                //Draw sequence numbering
                if (dataGrid.RowHeadersVisible)
                    dataGrid.RowPostPaint += DataGrid_RowPostPaint;

                dataGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dataGrid.AutoGenerateColumns = false;
            }
        }

        /// <summary>
        /// Insert row sequence numbering inside of row header cell
        /// Faster way to apply instead of looping through the rows
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void DataGrid_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            var grid = sender as DataGridView;
            var rowIdx = (e.RowIndex + 1).ToString();

            var centerFormat = new StringFormat()
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            var headerBounds = new Rectangle(e.RowBounds.Right - 40, e.RowBounds.Top, grid.RowHeadersWidth,
                e.RowBounds.Height);
            e.Graphics.DrawString(rowIdx, Font, SystemBrushes.ControlText, headerBounds, centerFormat);
        }

        #endregion


        #region Cleaning Form
        protected virtual void ClearForm()
        {

        }

        protected void ClearControls(bool isSelectFirstComboItem,params Control[] controls)
        {
            foreach (var control in controls)
            {
                if (control is ComboBox)
                {
                    ComboBox comboBox = (ComboBox)control;
                    if (comboBox != null && comboBox.Items.Count > 0)
                        comboBox.SelectedIndex = comboBox.Items.Count > 1 && isSelectFirstComboItem ? 1 : 0;
                }
                else if (control is TextBoxNum)
                {
                    TextBoxNum textBoxNum = (TextBoxNum)control;
                    if (textBoxNum != null)
                        textBoxNum.Num = 0;
                }
                else if (control is TextBox)
                {
                    TextBox textBox = (TextBox)control;
                    if (textBox != null)
                        textBox.Text = "";
                }
                else if(control is DateTimePicker)
                {
                    DateTimePicker dateTimePicker = (DateTimePicker)control;
                    if(dateTimePicker != null)
                        dateTimePicker.Value = DateTime.Now;
                }
            }
        }
        #endregion

        #region Logging

        #endregion
    }
}
