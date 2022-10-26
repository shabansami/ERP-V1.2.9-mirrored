namespace PrintEngine.DataGridPrint.SupportingClasses
{
    /// <summary>
    /// Delegate for ownerdraw cells - allow the caller to provide drawing for the cell
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void CellOwnerDrawEventHandler(object sender, DGVCellDrawingEventArgs e);
}
