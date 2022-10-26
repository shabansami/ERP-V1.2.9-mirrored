using System;
using System.Drawing;
using System.Windows.Forms;

namespace PrintEngine.DataGridPrint.SupportingClasses
{
    /// <summary>
    /// Class for the ownerdraw event. Provide the caller with the cell data, the current
    /// graphics context and the location in which to draw the cell.
    /// </summary>
    public class DGVCellDrawingEventArgs : EventArgs
    {
        public Graphics g;
        public RectangleF DrawingBounds;
        public DataGridViewCellStyle CellStyle;
        public int row;
        public int column;
        public Boolean Handled;

        public DGVCellDrawingEventArgs(Graphics g, RectangleF bounds, DataGridViewCellStyle style,
            int row, int column)
            : base()
        {
            this.g = g;
            DrawingBounds = bounds;
            CellStyle = style;
            this.row = row;
            this.column = column;
            Handled = false;
        }
    }
}
