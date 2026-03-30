using System;
using System.Data;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace area_L
{
    public static class ExcelReader
    {
        public static DataTable LoadFirstSheet(string path)
        {
            var excel = Type.GetTypeFromProgID("Excel.Application");
            if (excel == null) throw new InvalidOperationException("Excel is not installed");
            dynamic app = Activator.CreateInstance(excel);
            try
            {
                dynamic wb = app.Workbooks.Open(path);
                dynamic ws = wb.Worksheets[1];
                var used = ws.UsedRange;
                object[,] values = used.Value2;
                var dt = new DataTable();
                int rows = values.GetLength(0);
                int cols = values.GetLength(1);
                for (int c = 1; c <= cols; c++) dt.Columns.Add("C" + c);
                for (int r = 2; r <= rows; r++)
                {
                    var row = dt.NewRow();
                    for (int c = 1; c <= cols; c++) row[c - 1] = values[r, c];
                    dt.Rows.Add(row);
                }
                wb.Close(false);
                return dt;
            }
            finally { app.Quit(); }
        }
    }

    public static class ExcelWriter
    {
        public static void ExportDataGridView(DataGridView grid, string path)
        {
            var excel = Type.GetTypeFromProgID("Excel.Application");
            if (excel == null) throw new InvalidOperationException("Excel is not installed");
            dynamic app = Activator.CreateInstance(excel);
            try
            {
                dynamic wb = app.Workbooks.Add();
                dynamic ws = wb.Worksheets[1];
                int col = 1;
                foreach (DataGridViewColumn c in grid.Columns)
                {
                    ws.Cells[1, col].Value2 = c.HeaderText;
                    col++;
                }
                int row = 2;
                foreach (DataGridViewRow r in grid.Rows)
                {
                    if (r.IsNewRow) continue;
                    for (int c = 0; c < grid.Columns.Count; c++)
                    {
                        ws.Cells[row, c + 1].Value2 = r.Cells[c].Value;
                    }
                    row++;
                }
                wb.SaveAs(path);
                wb.Close(true);
            }
            finally { app.Quit(); }
        }
    }
}