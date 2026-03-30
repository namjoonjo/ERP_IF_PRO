using area_L.Modules;
using area_L;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraSplashScreen;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;

namespace area_L
{
    public partial class EXCEL_FOR_MATE : Form
    {
        private static string dbName = "ERP_2";

        MSSQL db = new MSSQL(dbName);

        DataTable ScanDatas = null; public DataRow ScanRow = null;

        CommonModule cm = new CommonModule();

        

        public EXCEL_FOR_MATE()
        {
            InitializeComponent();

            initControl();
        }

        private void initControl()
        {
            try 
            {
                int n = 0;

                fn_EmptyBindingToGrid();

                tbx_Barcode.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) fn_BarCodeProcessing(); };

                btn_Reset.Click += (s, e) => { fn_Reset(true); };

                gridView1.InitNewRow += GridView1_InitNewRow;

                btn_Excel.Click += (s, e) => {

                    GridToExportExcelforDevExpressGrid("재고이동등록(품목) 엑셀출력", string.Empty, this.gridView1);
                };

                tbx_Barcode.Select();
            }
            catch (Exception ex) 
            {

            }
        }

        private void GridView1_InitNewRow(object sender, DevExpress.XtraGrid.Views.Grid.InitNewRowEventArgs e)
        {
            try
            {

                DevExpress.XtraGrid.Views.Grid.GridView gv = sender as DevExpress.XtraGrid.Views.Grid.GridView;

                int ridx = gv.RowCount == 1 ? 0 : gv.RowCount - 1;

                gv.SetRowCellValue(e.RowHandle, "SEQ", ridx == 0 ? 1 : int.Parse(gv.GetRowCellDisplayText(ridx - 1, "SEQ")) + 1);

                gv.SetRowCellValue(e.RowHandle, "GD_CD", ScanRow["GD_CD"].ToString());

                gv.SetRowCellValue(e.RowHandle, "GD_NM", ScanRow["GD_NM"].ToString());

                gv.SetRowCellValue(e.RowHandle, "SPEC", ScanRow["SPEC"].ToString());

                gv.SetRowCellValue(e.RowHandle, "UNIT", ScanRow["UNIT"].ToString());

                gv.SetRowCellValue(e.RowHandle, "CHECKSHEETNO", ScanRow["CHECKSHEETNO"].ToString());

                gv.SetRowCellValue(e.RowHandle, "QTY", ScanRow["QTY"].ToString());

                gv.SetRowCellValue(e.RowHandle, "MV_QTY", ScanRow["MV_QTY"].ToString());

                gv.SetRowCellValue(e.RowHandle, "LOT_NO", ScanRow["LOT_NO"].ToString());

                gv.SetRowCellValue(e.RowHandle, "SCAN_BARCODE", ScanRow["SCAN_BARCODE"].ToString());

                gv.SetRowCellValue(e.RowHandle, "MEMO", ScanRow["MEMO"].ToString());

                gv.SetRowCellValue(e.RowHandle, "BTN_DEL", ScanRow["BTN_DEL"].ToString());

            }
            catch (Exception ex)
            {

            }
        }

        private void fn_EmptyBindingToGrid()
        {
            try
            {
                ScanDatas = new DataTable();

                ScanDatas.Columns.Add("SEQ");
                ScanDatas.Columns.Add("GD_CD");
                ScanDatas.Columns.Add("GD_NM");
                ScanDatas.Columns.Add("SPEC");
                ScanDatas.Columns.Add("UNIT");
                ScanDatas.Columns.Add("CHECKSHEETNO");
                ScanDatas.Columns.Add("QTY");
                ScanDatas.Columns.Add("MV_QTY");
                ScanDatas.Columns.Add("LOT_NO");
                ScanDatas.Columns.Add("SCAN_BARCODE");
                ScanDatas.Columns.Add("MEMO");
                ScanDatas.Columns.Add("BTN_DEL");

                gridControl1.DataSource = ScanDatas;

                gridView1.OptionsView.ShowIndicator = false;

                gridView1.OptionsView.ColumnHeaderAutoHeight = DevExpress.Utils.DefaultBoolean.True;

                gridView1.ColumnPanelRowHeight = 50;

                gridView1.Columns["SEQ"].Caption = "순번";
                gridView1.Columns["GD_CD"].Caption = "품목코드";
                gridView1.Columns["GD_NM"].Caption = "품명";
                gridView1.Columns["SPEC"].Caption = "규격";
                gridView1.Columns["UNIT"].Caption = "단위";
                gridView1.Columns["CHECKSHEETNO"].Caption = "CHECK SHEET NO";
                gridView1.Columns["QTY"].Caption = "수량";
                gridView1.Columns["MV_QTY"].Caption = "이동\n수량";
                gridView1.Columns["LOT_NO"].Caption = "LotNo";
                gridView1.Columns["SCAN_BARCODE"].Caption = "스캔된바코드";
                gridView1.Columns["MEMO"].Caption = "비고";
                gridView1.Columns["BTN_DEL"].Caption = "삭제";


                gridView1.Columns["SEQ"].OptionsColumn.ReadOnly = true;
                gridView1.Columns["GD_CD"].OptionsColumn.ReadOnly = true;
                gridView1.Columns["GD_NM"].OptionsColumn.ReadOnly = true;
                gridView1.Columns["SPEC"].OptionsColumn.ReadOnly = true;
                gridView1.Columns["SCAN_BARCODE"].OptionsColumn.ReadOnly = true;

                gridView1.Columns["SCAN_BARCODE"].Visible = false;


                gridView1.Columns["SEQ"].Width = 50;
                gridView1.Columns["GD_CD"].Width = 150;
                gridView1.Columns["GD_NM"].Width = 200;
                gridView1.Columns["SPEC"].Width = 50;
                gridView1.Columns["UNIT"].Width = 50;
                gridView1.Columns["CHECKSHEETNO"].Width = 120;
                gridView1.Columns["QTY"].Width = 50;
                gridView1.Columns["MV_QTY"].Width = 50;
                gridView1.Columns["LOT_NO"].Width = 100;

                gridView1.Columns["MEMO"].Width = 150;
                gridView1.Columns["BTN_DEL"].Width = 100;

                gridView1.Columns["SEQ"].AppearanceHeader.Font = new Font("Tahoma", 12, FontStyle.Bold);
                gridView1.Columns["GD_CD"].AppearanceHeader.Font = new Font("Tahoma", 12, FontStyle.Bold);
                gridView1.Columns["GD_NM"].AppearanceHeader.Font = new Font("Tahoma", 12, FontStyle.Bold);
                gridView1.Columns["SPEC"].AppearanceHeader.Font = new Font("Tahoma", 12, FontStyle.Bold);
                gridView1.Columns["UNIT"].AppearanceHeader.Font = new Font("Tahoma", 12, FontStyle.Bold);
                gridView1.Columns["CHECKSHEETNO"].AppearanceHeader.Font = new Font("Tahoma", 12, FontStyle.Bold);
                gridView1.Columns["QTY"].AppearanceHeader.Font = new Font("Tahoma", 12, FontStyle.Bold);
                gridView1.Columns["MV_QTY"].AppearanceHeader.Font = new Font("Tahoma", 12, FontStyle.Bold);
                gridView1.Columns["LOT_NO"].AppearanceHeader.Font = new Font("Tahoma", 12, FontStyle.Bold);
                gridView1.Columns["SCAN_BARCODE"].AppearanceHeader.Font = new Font("Tahoma", 12, FontStyle.Bold);
                gridView1.Columns["MEMO"].AppearanceHeader.Font = new Font("Tahoma", 12, FontStyle.Bold);
                gridView1.Columns["BTN_DEL"].AppearanceHeader.Font = new Font("Tahoma", 12, FontStyle.Bold);


                gridView1.Columns["BTN_DEL"].ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;

                RepositoryItemButtonEdit rbtn = new RepositoryItemButtonEdit();

                rbtn.Buttons.RemoveAt(0);

                EditorButton ebtn = new EditorButton();

                ebtn.Kind = ButtonPredefines.Delete;

                ebtn.Click += Rbtn_Click;

                rbtn.Buttons.Add(ebtn);

                rbtn.TextEditStyle = TextEditStyles.HideTextEditor;

                gridView1.Columns["BTN_DEL"].ColumnEdit = rbtn;

                RepositoryItemTextEdit repositoryItemTextEdit1 = new RepositoryItemTextEdit();

                repositoryItemTextEdit1.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;

                repositoryItemTextEdit1.Mask.EditMask = "n0";

                gridView1.Columns["QTY"].ColumnEdit = repositoryItemTextEdit1;

            }
            catch (Exception ex) { }
        }

        private void Rbtn_Click(object sender, EventArgs e)
        {
            try
            {
                int ridx = gridView1.FocusedRowHandle;

                int seq = Convert.ToInt32(gridView1.GetRowCellDisplayText(ridx, "SEQ"));

                for (int i = 0; i < gridView1.RowCount; i++)
                {
                    if (Convert.ToInt32(gridView1.GetRowCellValue(i, "SEQ")).Equals(seq))
                    {
                        gridView1.DeleteRow(ridx);

                        break;
                    }
                }

            }
            catch (Exception ex)
            {

            }
        }

        private void fn_BarCodeProcessing()
        {
            try
            {
                if (string.IsNullOrEmpty(tbx_Barcode.Text))
                {
                    MessageBox.Show("바코드 입력바랍니다.", "바코드 스캔", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    return;
                }

                string strSql = $"{dbName}.dbo.ST_EXCEL_FOR_MATE_BARCODE_PROCESSING_SEL";

                db.Parameter("@BAR_CD", tbx_Barcode.Text);

                if (!splashScreenManager1.IsSplashFormVisible) splashScreenManager1.ShowWaitForm();

                db.ExecuteSql(strSql);

                if (splashScreenManager1.IsSplashFormVisible) splashScreenManager1.CloseWaitForm();

                if (db.nState)
                {
                    if (string.IsNullOrEmpty(db.sql_raise_error_msg))
                    {
                        if(db.result.Rows.Count == 0)
                        {
                            MessageBox.Show("해당 바코드를 찾을 수 없습니다.\n확인부탁드립니다.", "바코드 스캔", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                            tbx_Barcode.SelectAll();

                            return;
                        }

                        ScanRow = db.result.Rows[0];

                        if (ScanRow["KIND"].ToString().Equals("UDI") && ScanRow["CHECKSHEETNO"].ToString().Equals(string.Empty))
                        {
                            INSERT_MATENO2 im2 = new INSERT_MATENO2(this, "UDI");

                            im2.ShowDialog();
                        }

                        if (ScanRow["KIND"].ToString().Equals("UDI"))
                        {
                            if (fn_ChkDuplicateBarcodes(tbx_Barcode.Text))
                            {
                                MessageBox.Show($"[{tbx_Barcode.Text}]는 이미 스캔된 바코드입니다.\n확인부탁드립니다.", "바코드 스캔", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                                return;
                            }
                        }

                        if (!ScanRow["GD_CD"].ToString().Equals(string.Empty) && ScanRow["KIND"].ToString().Equals("E"))
                        {
                            INSERT_MATENO2 im2 = new INSERT_MATENO2(this,"mate");

                            im2.ShowDialog();
                        }

                        gridView1.AddNewRow();

                        gridView1.UpdateCurrentRow();

                        tbx_gd_cd.Text = ScanRow["GD_CD"].ToString();

                        tbx_gd_nm.Text = ScanRow["GD_NM"].ToString();

                        tbx_lotno.Text = ScanRow["CHECKSHEETNO"].ToString();

                        tbx_mateno.Text = ScanRow["LOT_NO"].ToString();

                        tbx_ScanBarcode.Text = ScanRow["SCAN_BARCODE"].ToString();


                        switch (ScanRow["KIND"].ToString())
                        {
                            case "CS":

                                tbx_kind.Text = "CHECK-SHEET";

                                break;

                            case "UDI":

                                tbx_kind.Text = "UDI";

                                break;

                            case "E":

                                tbx_kind.Text = "물류(유통) 바코드 8자리";

                                break;
                        }

                        this.tbx_Barcode.Text = string.Empty;

        

                        return;
                    }
                    else if (db.sql_raise_error_msg.Equals("EXIST_ERR"))
                    {
                        MessageBox.Show("해당 창고에서 이미 등록된 체크시트입니다. \n\nERP - 2024 재고실사 데이터를 확인해주세요.", "바코드 스캔", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                        this.tbx_Barcode.SelectAll();

                        return;
                    }
                }

                MessageBox.Show($"{db.sql_raise_error_msg}\n\n정보전략팀에 문의하세요.", "바코드 스캔", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                return;
            }
            catch (Exception ex) { }
        }

        private bool fn_ChkDuplicateBarcodes(string barcode)
        {
            try
            {
                int rCnt = gridView1.RowCount;

                for (int i = 0; i < rCnt; i++)
                {
                    if (gridView1.GetRowCellDisplayText(i, "SCAN_BARCODE").Equals(barcode))
                    {
                        gridView1.SelectRow(i);

                        return true;
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                return true;
            }
        }

        private void fn_Reset(bool byUser)
        {
            try
            {
                if (byUser)
                {
                    DialogResult rs = MessageBox.Show("현재 스캔된 데이터가 지워집니다.\n초기화하시겠습니까?", "초기화", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (rs == DialogResult.No) return;
                }

                this.tbx_Barcode.Text = string.Empty;

                this.tbx_gd_cd.Text = string.Empty;

                this.tbx_gd_nm.Text = string.Empty;

                this.tbx_kind.Text = string.Empty;

                this.tbx_mateno.Text = string.Empty;

                this.tbx_lotno.Text = string.Empty;

                this.tbx_ScanBarcode.Text = string.Empty;

                for (int i = 0; i < gridView1.RowCount;) gridView1.DeleteRow(i);

                this.tbx_Barcode.Select();
            }
            catch (Exception ex)
            {

            }
        }

        public void GridToExportExcelforDevExpressGrid(string fileName, string kind, GridView dg)
        {
            try
            {
                bool IsExport = false;

                if (!splashScreenManager1.IsSplashFormVisible) splashScreenManager1.ShowWaitForm();

                Excel._Application excel = new Excel.Application();
                Excel._Workbook workbook = excel.Workbooks.Add(Type.Missing);
                Excel._Worksheet worksheet = null;

                worksheet = (Excel.Worksheet)workbook.ActiveSheet;

                int cellRowIndex = 1;
                int cellColumnIndex = 1;

                for (int col = 0; col < dg.Columns.Count - 1; col++)
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
                    for (int col = 0; col < dg.Columns.Count - 1; col++)
                    {
                        worksheet.Cells[cellRowIndex, cellColumnIndex] = dg.GetRowCellDisplayText(row, dg.Columns[col]);// dg.Rows[row].Cells[col].Value.ToString();
                        cellColumnIndex++;
                    }
                    cellColumnIndex = 1;
                    cellRowIndex++;
                }

                SaveFileDialog saveFileDialog = new SaveFileDialog();

                saveFileDialog.Title = "Save as Excel File";

                saveFileDialog.Filter = "Excel Files(2016)|*.xlsx";// "Excel Files(2003)|*.xls|Excel Files(2016)|*.xlsx";

                saveFileDialog.FileName = $"{fileName}_{kind}_{DateTime.Now.ToString("yyyyMMddhhmmss")}";

                if (splashScreenManager1.IsSplashFormVisible) splashScreenManager1.CloseWaitForm();

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
