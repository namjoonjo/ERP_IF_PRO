// ERP_IF_PRO.Modules 네임스페이스의 클래스를 COMBINATION.Modules에서 접근 가능하도록 브릿지
// Regi_Combi_Half_Lot_Manage.cs가 using COMBINATION.Modules만 사용하기 때문에 필요
using DevExpress.XtraGrid.Views.Grid;
using System;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;

namespace COMBINATION.Modules
{
    class MSSQL : ERP_IF_PRO.Modules.MSSQL
    {
        public MSSQL(string dbName) : base(dbName) { }
    }

    class CommonModule : ERP_IF_PRO.Modules.CommonModule
    {
        // ERP_IF_PRO CommonModule에서 주석 처리된 메서드를 복원
        public void GridToExportExcelforDevExpressGrid(string fileName, string kind, GridView dg)
        {
            try
            {
                bool IsExport = false;

                Excel._Application excel = new Excel.Application();
                Excel._Workbook workbook = excel.Workbooks.Add(Type.Missing);
                Excel._Worksheet worksheet = null;

                worksheet = (Excel.Worksheet)workbook.ActiveSheet;

                int cellRowIndex = 1;
                int cellColumnIndex = 1;

                for (int col = 1; col < dg.Columns.Count; col++)
                {
                    if (cellRowIndex == 1)
                    {
                        worksheet.Cells[cellRowIndex, cellColumnIndex] = dg.Columns[col].Caption;
                    }
                    cellColumnIndex++;
                }

                cellColumnIndex = 1;
                cellRowIndex++;

                int gridRcnt = dg.RowCount;
                for (int row = 0; row < gridRcnt; row++)
                {
                    for (int col = 1; col < dg.Columns.Count; col++)
                    {
                        worksheet.Cells[cellRowIndex, cellColumnIndex] = dg.GetRowCellDisplayText(row, dg.Columns[col]);
                        cellColumnIndex++;
                    }
                    cellColumnIndex = 1;
                    cellRowIndex++;
                }

                SaveFileDialog saveFileDialog = new SaveFileDialog();

                saveFileDialog.Title = "Save as Excel File";

                saveFileDialog.Filter = "Excel Files(2016)|*.xlsx";

                saveFileDialog.FileName = $"{fileName}_{kind}_{DateTime.Now.ToString("yyyyMMddhhmmss")}";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    workbook.SaveAs(saveFileDialog.FileName);
                    IsExport = true;
                }
                if (IsExport)
                {
                    workbook.Close();
                    excel.Quit();
                    workbook = null;
                    excel = null;
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
