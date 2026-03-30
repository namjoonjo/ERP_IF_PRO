using area_L.Modules;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
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

namespace area_L
{
    public partial class UDI_RE_STORE : Form
    {
        public DataRow ScanRow = null; DataTable ScanDatas = null;

        private static string dbName = "ERP_2";

        MSSQL db = new MSSQL(dbName);

        CommonModule cm = new CommonModule();

        public UDI_RE_STORE()
        {
            InitializeComponent();

            initControl();
        }

        private void initControl()
        {
            try
            {
                fn_EmptyBindingToGrid();

                tbx_worker.Select();

                tbx_worker.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) fn_WorkerChk(); };

                tbx_Barcode.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) fn_BarCodeProcessing(); };

                gridView1.InitNewRow += GridView1_InitNewRow;

                btn_Save.Click += (s,e) => { fn_Save(); };

                btn_Reset.Click += (s, e) => { fn_Reset(gridView1.RowCount != 0); };
            }
            catch(Exception ex)
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

                gv.SetRowCellValue(e.RowHandle, "GTIN_NO", ScanRow["GTIN_NO"].ToString());

                gv.SetRowCellValue(e.RowHandle, "LOT_NO", ScanRow["LOT_NO"].ToString());

                gv.SetRowCellValue(e.RowHandle, "MATE_NO", ScanRow["MATE_NO"].ToString());

                gv.SetRowCellValue(e.RowHandle, "SCAN_BARCODE", ScanRow["SCAN_BARCODE"].ToString());

            }
            catch (Exception ex)
            {

            }
        }

        private void fn_WorkerChk()
        {
            try
            {
                if (string.IsNullOrEmpty(tbx_worker.Text))
                {
                    MessageBox.Show("검수자(사번) 입력바랍니다.", "입력", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    return;
                }

                string strSql = $"{dbName}.dbo.ST_STOCKTAKING_EMP_SEL";

                db.Parameter("@EMP_NO", tbx_worker.Text);

                db.ExecuteSql(strSql);

                if (db.nState)
                {
                    if (db.result.Rows.Count > 0)
                    {
                        DataRow dr = db.result.Rows[0];

                        lb_empinfo.Text = $"검수자 : {dr["EMP_NM"].ToString()}";

                        tbx_Barcode.Select();

                        return;
                    }
                }

                MessageBox.Show("ERP에 등록되지 않은 사용자입니다.\n확인부탁드립니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                tbx_worker.SelectAll();

                lb_empinfo.Text = string.Empty;

                return;
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

                if (string.IsNullOrEmpty(tbx_worker.Text))
                {
                    MessageBox.Show("담당자 입력바랍니다.", "바코드 스캔", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    return;
                }

                if(tbx_Barcode.Text.Length < 20 || !tbx_Barcode.Text.Substring(0, 2).Equals("01"))
                {
                    MessageBox.Show("UDI 형태의 바코드만 스캔이 가능합니다", "바코드 스캔", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    return;
                }

                string strSql = $"{dbName}.dbo.UDI_RE_STORE_BARCODE_DETAIL_SEL";

                db.Parameter("@BAR_CD", tbx_Barcode.Text);


                if (!splashScreenManager1.IsSplashFormVisible) splashScreenManager1.ShowWaitForm();

                db.ExecuteSql(strSql);

                if (splashScreenManager1.IsSplashFormVisible) splashScreenManager1.CloseWaitForm();

                if (db.nState)
                {
                    if (string.IsNullOrEmpty(db.sql_raise_error_msg))
                    {
                        if (db.result.Rows.Count == 0)
                        {
                            MessageBox.Show("해당 바코드를 찾을 수 없습니다.\n확인부탁드립니다.", "바코드 스캔", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                            tbx_Barcode.SelectAll();

                            return;
                        }

                        this.ScanRow = db.result.Rows[0];

                        if (fn_ChkDuplicateBarcodes(tbx_Barcode.Text))
                        {
                            MessageBox.Show($"[{tbx_Barcode.Text}]는 이미 스캔된 바코드입니다.\n확인부탁드립니다.", "바코드 스캔", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                            return;
                        }

                        gridView1.AddNewRow();

                        gridView1.UpdateCurrentRow();

                        tbx_gd_cd.Text = ScanRow["GD_CD"].ToString();

                        tbx_gd_nm.Text = ScanRow["GD_NM"].ToString();

                        tbx_lotno.Text = ScanRow["LOT_NO"].ToString();

                        tbx_mateno.Text = ScanRow["MATE_NO"].ToString();

                        tbx_ScanBarcode.Text = ScanRow["SCAN_BARCODE"].ToString();

                        this.tbx_Barcode.Text = string.Empty;

                        return;
                    }
                    else 
                    {
                        MessageBox.Show($"{db.sql_raise_error_msg}. \n\n확인부탁드립니다.", "바코드 스캔", MessageBoxButtons.OK, MessageBoxIcon.Warning);

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

        private void fn_EmptyBindingToGrid()
        {
            try
            {
                ScanDatas = new DataTable();

                ScanDatas.Columns.Add("SEQ");
                ScanDatas.Columns.Add("GD_CD");
                ScanDatas.Columns.Add("GD_NM");
                ScanDatas.Columns.Add("GTIN_NO");
                ScanDatas.Columns.Add("LOT_NO");
                ScanDatas.Columns.Add("MATE_NO");
                ScanDatas.Columns.Add("SCAN_BARCODE");

                gridControl1.DataSource = ScanDatas;

                gridView1.OptionsView.ColumnHeaderAutoHeight = DevExpress.Utils.DefaultBoolean.True;

                gridView1.OptionsView.ShowIndicator = false;

                gridView1.Columns["SEQ"].Caption = "순번";
                gridView1.Columns["GD_CD"].Caption = "품목코드";
                gridView1.Columns["GD_NM"].Caption = "품명";
                gridView1.Columns["GTIN_NO"].Caption = "GTIN";
                gridView1.Columns["LOT_NO"].Caption = "LotNo";
                gridView1.Columns["MATE_NO"].Caption = "멸균번호";
                gridView1.Columns["SCAN_BARCODE"].Caption = "스캔된바코드";

                gridView1.Columns["SEQ"].OptionsColumn.ReadOnly = true;
                gridView1.Columns["GD_CD"].OptionsColumn.ReadOnly = true;
                gridView1.Columns["GD_NM"].OptionsColumn.ReadOnly = true;
                gridView1.Columns["GTIN_NO"].OptionsColumn.ReadOnly = true;
                gridView1.Columns["LOT_NO"].OptionsColumn.ReadOnly = true;
                gridView1.Columns["MATE_NO"].OptionsColumn.ReadOnly = true;
                gridView1.Columns["SCAN_BARCODE"].OptionsColumn.ReadOnly = true;

                gridView1.Columns["SEQ"].Width = 50;
                gridView1.Columns["GD_CD"].Width = 200;
                gridView1.Columns["GD_NM"].Width = 250;
                gridView1.Columns["GTIN_NO"].Width = 130;
                gridView1.Columns["LOT_NO"].Width = 130;
                gridView1.Columns["MATE_NO"].Width = 100;
                gridView1.Columns["SCAN_BARCODE"].Width = 300;

                gridView1.Columns["SEQ"].AppearanceHeader.Font = new Font("Tahoma", 12, FontStyle.Bold);
                gridView1.Columns["GD_CD"].AppearanceHeader.Font = new Font("Tahoma", 12, FontStyle.Bold);
                gridView1.Columns["GD_NM"].AppearanceHeader.Font = new Font("Tahoma", 12, FontStyle.Bold);
                gridView1.Columns["GTIN_NO"].AppearanceHeader.Font = new Font("Tahoma", 12, FontStyle.Bold);
                gridView1.Columns["LOT_NO"].AppearanceHeader.Font = new Font("Tahoma", 12, FontStyle.Bold);
                gridView1.Columns["MATE_NO"].AppearanceHeader.Font = new Font("Tahoma", 12, FontStyle.Bold);
                gridView1.Columns["SCAN_BARCODE"].AppearanceHeader.Font = new Font("Tahoma", 10, FontStyle.Bold);

            }
            catch (Exception ex) { }
        }

        private void fn_Save()
        {
            try
            {
                if (MessageBox.Show("저장하시겠습니까?", "저장", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) return;

                int rCnt = gridView1.RowCount;

                if (rCnt == 0)
                {
                    MessageBox.Show("스캔된 바코드가 없습니다.\n확인부탁드립니다.", "저장", MessageBoxButtons.OK, MessageBoxIcon.Question);

                    return;
                }

                if (string.IsNullOrEmpty(lb_empinfo.Text) || string.IsNullOrEmpty(tbx_worker.Text))
                {
                    MessageBox.Show("검수자를 입력해 주십시오.", "저장", MessageBoxButtons.OK, MessageBoxIcon.Question);

                    tbx_worker.Select();

                    return;
                }

                DataTable newdt = new DataTable();

                newdt.Columns.Add("GTIN_NO");
                newdt.Columns.Add("PS_CD");
                newdt.Columns.Add("SCAN_BARCODE");
                newdt.Columns.Add("MATE_NO");

                for (int i = 0; i < rCnt; i++)
                {
                    DataRow dr = newdt.NewRow();

                    dr["GTIN_NO"] = gridView1.GetRowCellDisplayText(i, "GTIN_NO");

                    dr["PS_CD"] = tbx_worker.Text.Trim();

                    dr["SCAN_BARCODE"] = gridView1.GetRowCellDisplayText(i, "SCAN_BARCODE");

                    dr["MATE_NO"] = gridView1.GetRowCellDisplayText(i, "MATE_NO");

                    newdt.Rows.Add(dr);
                }

                string strSql = $"{dbName}.dbo.UDI_RE_STORE_SAVE";

                db.Parameter("@XML_VAL", cm.DataTblToXML(newdt));

                if (!splashScreenManager1.IsSplashFormVisible) splashScreenManager1.ShowWaitForm();

                db.ExecuteNonSql(strSql);

                if (splashScreenManager1.IsSplashFormVisible) splashScreenManager1.CloseWaitForm();

                if (db.nState)
                {
                    if (string.IsNullOrEmpty(db.sql_raise_error_msg))
                    {
                        MessageBox.Show("저장이 완료되었습니다.", "저장", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        fn_Reset(false);

                        return;
                    }
                }

                MessageBox.Show($"{db.sql_raise_error_msg}\n\n정보전략팀에 문의하시기 바랍니다.", "ERP전송", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                return;
            }
            catch (Exception ex) { }
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

    }
}
