using COMBINATION.Label;
using COMBINATION.MixingLabel;
using COMBINATION.Modules;
using DevExpress.XtraCharts;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraReports.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;


namespace COMBINATION
{
    public partial class Regi_Combi_Half_Lot_Manage : Form
    {
        public Action<string> UpdateStatus { get; set; }

        private static string dbName = "ERP_2";

        MSSQL db = new MSSQL(dbName);

        CommonModule cm = new CommonModule();

        private Excel.Application application = null;

        private Excel.Workbook workBook = null;

        private Excel.Worksheet workSheet = null;

        public Regi_Combi_Half_Lot_Manage()
        {
            InitializeComponent();

            initControl();
        }

        private void initControl()
        {
            try
            {
                tbx_gdcdnm.Enabled = false;

                gridView1.InitNewRow += GridView1_InitNewRow;

                gridView1.OptionsView.ShowIndicator = false;

                gridView2.OptionsView.ShowIndicator = false;

                gridView1.MouseDown += GridView1_MouseDown;

                gridView1.CellValueChanging += GridView_CellValueChanging;

                gridView2.CellValueChanging += GridView_CellValueChanging;

                btn_Add.Click += (s, e) => { gridView1.AddNewRow(); };

                btn_Sel.Click += (s, e) => { fn_MasterSel(); };

                btn_Save.Click += (s, e) => { fn_MasterSave(); };

                btn_Del.Click += (s, e) => { fn_MasterDel(); };

                btn_detailDel.Click += (s, e) => { fn_DetailDel(); };

                btn_CreateLot.Click += (s, e) => { fn_CreateLot(); };

                btn_detailSave.Click += (s, e) => { fn_DetailSave(); };

                btn_Print.Click += (s, e) => { fn_cmdReportClickforZebraQRBarcode(); };

                btn_Excel.Click += (s, e) => { cm.GridToExportExcelforDevExpressGrid($"{tbx_gdcdnm.Text.Replace("/", string.Empty)}", string.Empty, this.gridView2); };

                StartDate.ValueChanged += Date_ValueChanged;

                EndDate.ValueChanged += Date_ValueChanged;

                fn_MasterSel();
            }
            catch(Exception ex)
            {

            }
        }

        private void GridView_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                if (e.RowHandle < 0) return;

                GridView gv = sender as GridView;

                gv.SetRowCellValue(e.RowHandle, "SEL", "True");
            }
            catch (Exception ex)
            {

            }
        }

        private void Date_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                GridView view = gridView1;

                view.PostEditor();
                view.UpdateCurrentRow();

                if (view.FocusedRowHandle >= 0 && view.IsDataRow(view.FocusedRowHandle))
                {
                    DataRow row = view.GetDataRow(view.FocusedRowHandle);

                    fn_DetailSel(row.Field<string>("GD_CD"));

                    tbx_gdcdnm.Text = $"{row["GD_CD"].ToString()} / {row["GD_NM"].ToString()}";
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void GridView1_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                var view = sender as GridView;
                GridHitInfo hit = view.CalcHitInfo(e.Location);

                if (hit.InRowCell)
                {
                    DataRow row = view.GetDataRow(hit.RowHandle);

                    if (row != null)
                    {
                        tbx_gdcdnm.Text = $"{row["GD_CD"].ToString()} / {row["GD_NM"].ToString()}";

                        fn_DetailSel(row["GD_CD"].ToString());
                    }
                }
            }
            catch(Exception ex )
            {

            }

        }


        private void GridView1_InitNewRow(object sender, DevExpress.XtraGrid.Views.Grid.InitNewRowEventArgs e)
        {
            try
            {
                GridView gv = sender as GridView;

                gv.SetRowCellValue(e.RowHandle, "SEL", "True");

            }
            catch(Exception ex)
            {

            }
        }


        private void fn_CreateLot()
        {
            try
            {

                if (MessageBox.Show($"[{tbx_gdcdnm.Text}]에 대한 Lot을 생성하시겠습니까?", "Lot부여", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) return;

                string strSql = $"{dbName}.dbo.ST_COMBI_SEMI_FINISHED_CREATE_LOT";

                GridView view = gridView1;

                view.PostEditor();
                view.UpdateCurrentRow();

                if (view.FocusedRowHandle >= 0 && view.IsDataRow(view.FocusedRowHandle))
                {
                    DataRow row = view.GetDataRow(view.FocusedRowHandle); 

                    if (string.IsNullOrEmpty(row.Field<string>("GD_CD")))
                    {
                        MessageBox.Show("Lot부여 규칙이 없습니다.\nLot생성이 불가능합니다.", "저장", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                        return;
                    }

                    db.Parameter("@LOT_RULE", row.Field<string>("LOT_RULE"));
                    db.Parameter("@GD_CD", row.Field<string>("GD_CD"));
                    db.Parameter("@GD_NM", row.Field<string>("GD_NM"));
                    db.Parameter("@DATE", madeDate.Value.ToString("yyyy-MM-dd"));

                    db.ExecuteNonSql(strSql);

                    if (db.nState)
                    {
                        if (string.IsNullOrEmpty(db.sql_raise_error_msg))
                        {
                            MessageBox.Show("Lot이 부여되었습니다.", "Lot부여", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            fn_DetailSel(row.Field<string>("GD_CD"));

                            return;
                        }
                    }

                    MessageBox.Show($"에러코드 : {db.sql_raise_error_msg}", "Lot부여", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

            }
            catch(Exception ex)
            {

            }
        }

        private void fn_DetailSel(string GD_CD)
        {
            try
            {

                string strSql = $"{dbName}.dbo.ST_COMBI_SEMI_FINISHED_DETAIL_SEL";

                db.Parameter("@GD_CD", GD_CD);
                db.Parameter("@START_DATE", StartDate.Value.ToString("yyyy-MM-dd"));
                db.Parameter("@END_DATE", EndDate.Value.ToString("yyyy-MM-dd"));

                db.ExecuteSql(strSql);

                if (db.nState)
                {
                    grid_State2.DataSource = db.result;

                    RepositoryItemCheckEdit ri = new RepositoryItemCheckEdit();

                    ri.ValueChecked = "True";

                    ri.ValueUnchecked = "False";

                    ri.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;

                    ri.CheckBoxOptions.Style = DevExpress.XtraEditors.Controls.CheckBoxStyle.SvgCheckBox1;

                    gridView2.Columns["SEL"].ColumnEdit = ri;

                    gridView2.Columns["GD_CD"].Visible = false;

                    gridView2.Columns["SEL"].Caption = "선택";
                    gridView2.Columns["GD_NM"].Caption = "품목명";
                    gridView2.Columns["LOT_NO"].Caption = "LotNo";
                    gridView2.Columns["IN_DT"].Caption = "제조날짜(내림차순)";
                    gridView2.Columns["SPEC"].Caption = "스펙(점도/색)";
                    gridView2.Columns["VAL_FLAG"].Caption = "적합 유/무";

                    gridView2.Columns["GD_NM"].OptionsColumn.ReadOnly = true;
                    gridView2.Columns["LOT_NO"].OptionsColumn.ReadOnly = true;

                    gridView2.Columns["SEL"].Width = 20;
                }  
            }
            catch(Exception ex)
            {

            }
        }

        private void fn_DetailDel()
        {
            try
            {
                if (MessageBox.Show("삭제하시겠습니까?", "삭제", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) return;

                string strSql = $"{dbName}.dbo.ST_COMBI_SEMI_FINISHED_DETAIL_DEL";

                int rowCount = gridView2.RowCount;

                DataTable delData = new DataTable();

                delData.Columns.Add("GD_CD");
                delData.Columns.Add("LOT_NO");

                for (int i = 0; i < rowCount; i++)
                {
                    DataRow gridviewDR = gridView2.GetDataRow(i);

                    if (gridviewDR == null || !gridviewDR["SEL"].ToString().Equals("True") || string.IsNullOrEmpty(gridView2.GetRowCellDisplayText(i, "GD_CD")) || string.IsNullOrEmpty(gridView2.GetRowCellDisplayText(i, "LOT_NO"))) continue;

                    DataRow dr = delData.NewRow();

                    dr["GD_CD"] = gridView2.GetRowCellDisplayText(i, "GD_CD");

                    dr["LOT_NO"] = gridView2.GetRowCellDisplayText(i, "LOT_NO");

                    delData.Rows.Add(dr);
                }

                db.Parameter("@DEL_DATA_STR", cm.DataTblToXML(delData));

                db.ExecuteNonSql(strSql);

                if (db.nState)
                {
                    if (string.IsNullOrEmpty(db.sql_raise_error_msg))
                    {
                        MessageBox.Show("삭제되었습니다.", "삭제", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        gridView1.PostEditor();
                        gridView1.UpdateCurrentRow();

                        fn_DetailSel(gridView1.GetDataRow(gridView1.FocusedRowHandle).Field<string>("GD_CD"));

                        return;
                    }
                }

                MessageBox.Show($"에러코드 : {db.sql_raise_error_msg}", "삭제", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {

            }
        }

        private void fn_MasterDel()
        {
            try
            {
                if (MessageBox.Show("삭제하시겠습니까?", "삭제", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) return;

                string strSql = $"{dbName}.dbo.ST_COMBI_SEMI_FINISHED_MASTER_DEL";

                int rowCount = gridView1.RowCount;

                DataTable delData = new DataTable();

                delData.Columns.Add("GD_CD");

                for (int i = 0; i < rowCount; i++)
                {
                    DataRow gridviewDR = gridView1.GetDataRow(i);

                    if (gridviewDR == null || !gridviewDR["SEL"].ToString().Equals("True") || string.IsNullOrEmpty(gridView1.GetRowCellDisplayText(i, "GD_CD"))) continue;

                    DataRow dr = delData.NewRow();

                    dr["GD_CD"] = gridView1.GetRowCellDisplayText(i, "GD_CD");

                    delData.Rows.Add(dr);
                }

                db.Parameter("@DEL_DATA_STR",cm.DataTblToXML(delData));

                db.ExecuteNonSql(strSql);

                if (db.nState)
                {
                    if (string.IsNullOrEmpty(db.sql_raise_error_msg))
                    {
                        MessageBox.Show("삭제되었습니다.", "삭제", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        fn_MasterSel();

                        return;
                    }
                }

                MessageBox.Show($"에러코드 : {db.sql_raise_error_msg}", "삭제", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch(Exception ex)
            {

            }
        }

        private void fn_DetailSave()
        {
            try 
            {
                
                if (MessageBox.Show("저장하시겠습니까?", "저장", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) return;

                string strSql = $"{dbName}.dbo.ST_COMBI_SEMI_FINISHED_DETAIL_UPD";

                DataTable newData = new DataTable();

                newData.Columns.Add("GD_CD");
                newData.Columns.Add("GD_NM");
                newData.Columns.Add("LOT_NO");
                newData.Columns.Add("IN_DT");
                newData.Columns.Add("SPEC");
                newData.Columns.Add("VAL_FLAG");

                int rowCount = gridView2.RowCount;

                for (int i = 0; i < rowCount; i++)
                {
                    DataRow gridviewDR = gridView2.GetDataRow(i);

                    if (gridviewDR == null || !gridviewDR["SEL"].ToString().Equals("True")) continue;

                    if (gridView2.GetRowCellValue(i, "GD_CD") == null || string.IsNullOrEmpty(gridView2.GetRowCellDisplayText(i, "GD_CD")))
                    {
                        MessageBox.Show("품목코드가 없습니다.", "저장", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                        return;
                    }

                    if (gridView2.GetRowCellValue(i, "LOT_NO") == null || string.IsNullOrEmpty(gridView2.GetRowCellDisplayText(i, "LOT_NO")))
                    {
                        MessageBox.Show("LotNo가 없습니다.", "저장", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                        return;
                    }

                    DataRow dr = newData.NewRow();

                    dr["GD_CD"] = gridView2.GetRowCellDisplayText(i, "GD_CD");

                    dr["GD_NM"] = gridView2.GetRowCellDisplayText(i, "GD_NM");

                    dr["LOT_NO"] = gridView2.GetRowCellDisplayText(i, "LOT_NO");

                    dr["IN_DT"] = gridView2.GetRowCellDisplayText(i, "IN_DT");

                    dr["SPEC"] = gridView2.GetRowCellDisplayText(i, "SPEC");

                    dr["VAL_FLAG"] = gridView2.GetRowCellDisplayText(i, "VAL_FLAG");

                    newData.Rows.Add(dr);
                }


                db.Parameter("@NEW_DATA_STR", cm.DataTblToXML(newData));

                db.ExecuteNonSql(strSql);

                if (db.nState)
                {
                    if (string.IsNullOrEmpty(db.sql_raise_error_msg))
                    {
                        MessageBox.Show("저장되었습니다.", "저장", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        if (gridView2.FocusedRowHandle >= 0 && gridView2.IsDataRow(gridView2.FocusedRowHandle))
                        {
                            DataRow row = gridView2.GetDataRow(gridView2.FocusedRowHandle);

                            fn_DetailSel(row.Field<string>("GD_CD"));

                        }

                        return;
                    }
                }

                MessageBox.Show($"에러코드 : {db.sql_raise_error_msg}", "저장", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch(Exception ex)
            {

            }
        }

        private void fn_cmdReportClickforZebra()
        {
            try
            {

                GridView view = gridView2;

                view.PostEditor();
                view.UpdateCurrentRow();

                if (view.FocusedRowHandle >= 0 && view.IsDataRow(view.FocusedRowHandle))
                {
                    DataRow row = view.GetDataRow(view.FocusedRowHandle);

                    tbx_gdcdnm.Text = $"{row["GD_CD"].ToString()} / {row["GD_NM"].ToString()}";

                    if (MessageBox.Show($"Report를 출력 하시겠습니까?\n\n약품번호 : {row.Field<string>("LOT_NO")}\n\n배합일시 : {row.Field<DateTime>("IN_DT").ToString("yyyy-MM-dd")}\n\n유통 기간 : " +
                    $"1년\n\n품  명 : {row.Field<string>("GD_NM")}", "출력", MessageBoxButtons.YesNo) == DialogResult.No) return;


                    PrintInfo pf = new PrintInfo();

                    pf.setLotNo(row["LOT_NO"].ToString());

                    pf.setproDate($"{row.Field<DateTime>("IN_DT").ToString("yyyy-MM-dd")}");

                    pf.setvaliDate($"1년");

                    pf.setRGB(255, 255, 255);

                    pf.setGD_NM($"{row.Field<string>("GD_NM")}");

                    SemiFinishedLabel xx = new SemiFinishedLabel(pf);

                    using (ReportPrintTool printTool = new ReportPrintTool(xx))
                    {
                        xx.ShowPrintMarginsWarning = false;

                        //printTool.ShowPreviewDialog();

                        printTool.Print();
                    }
                }

            }
            catch (Exception ex)
            {

            }

        }

        private void fn_cmdReportClickforZebraQRBarcode()
        {
            try
            {

                GridView view = gridView2;

                view.PostEditor();
                view.UpdateCurrentRow();

                if (view.FocusedRowHandle >= 0 && view.IsDataRow(view.FocusedRowHandle))
                {
                    DataRow row = view.GetDataRow(view.FocusedRowHandle);

                    tbx_gdcdnm.Text = $"{row["GD_CD"].ToString()} / {row["GD_NM"].ToString()}";

                    if (MessageBox.Show($"Report를 출력 하시겠습니까?\n\n약품번호 : {row.Field<string>("LOT_NO")}\n\n배합일시 : {row.Field<DateTime>("IN_DT").ToString("yyyy-MM-dd")}\n\n유통 기간 : " +
                    $"1년\n\n품  명 : {row.Field<string>("GD_NM")}", "출력", MessageBoxButtons.YesNo) == DialogResult.No) return;


                    PrintInfo pf = new PrintInfo();

                    pf.setLotNo(row["LOT_NO"].ToString());

                    pf.setproDate($"{row.Field<DateTime>("IN_DT").ToString("yyyy-MM-dd")}");

                    pf.setvaliDate($"1년");

                    pf.setRGB(255, 255, 255);

                    pf.setGD_NM($"{row.Field<string>("GD_NM")}");

                    CODEX_MixingLabelforZebra xx = new CODEX_MixingLabelforZebra(pf);

                    using (ReportPrintTool printTool = new ReportPrintTool(xx))
                    {
                        xx.ShowPrintMarginsWarning = false;

                        //printTool.ShowPreviewDialog();

                        printTool.Print();
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void fn_MasterSave()
        {
            try
            {
                if (MessageBox.Show("저장하시겠습니까?", "저장", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) return;

                int rowCount = gridView1.RowCount;

                string strSql = $"{dbName}.dbo.ST_COMBI_SEMI_FINISHED_MASTER_UPD";

                DataTable newData = new DataTable();

                newData.Columns.Add("GD_CD");
                newData.Columns.Add("GD_NM");
                newData.Columns.Add("LOT_RULE");
                newData.Columns.Add("STD_STR");

                for (int i = 0; i < rowCount; i++)
                {
                    DataRow gridviewDR = gridView1.GetDataRow(i);

                    if (gridviewDR == null || !gridviewDR["SEL"].ToString().Equals("True")) continue;

                    if(gridView1.GetRowCellValue(i,"GD_CD") == null || string.IsNullOrEmpty(gridView1.GetRowCellDisplayText(i, "GD_CD")))
                    {
                        MessageBox.Show("품목코드를 입력해주십시오.", "저장", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                        return;
                    }

                    if (gridView1.GetRowCellValue(i, "LOT_RULE") == null || string.IsNullOrEmpty(gridView1.GetRowCellDisplayText(i, "LOT_RULE")))
                    {
                        MessageBox.Show("Lot부여규칙을 입력해주십시오.", "저장", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                        return;
                    }

                    DataRow dr = newData.NewRow();

                    dr["GD_CD"] = gridView1.GetRowCellDisplayText(i, "GD_CD");

                    dr["GD_NM"] = gridView1.GetRowCellDisplayText(i, "GD_NM");

                    dr["LOT_RULE"] = gridView1.GetRowCellDisplayText(i, "LOT_RULE");

                    dr["STD_STR"] = gridView1.GetRowCellDisplayText(i, "STD_STR");

                    newData.Rows.Add(dr);
                }


                db.Parameter("@NEW_DATA_STR", cm.DataTblToXML(newData));

                db.ExecuteNonSql(strSql);

                if (db.nState)
                {
                    if (string.IsNullOrEmpty(db.sql_raise_error_msg))
                    {
                        MessageBox.Show("저장되었습니다.", "저장", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        fn_MasterSel();

                        return;
                    }
                }

                MessageBox.Show($"에러코드 : {db.sql_raise_error_msg}", "저장", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (Exception ex)
            {

            }
        }

        private void fn_MasterSel()
        {
            try
            {
                string strSql = $"{dbName}.dbo.ST_COMBI_SEMI_FINISHED_MASTER_SEL";

                UpdateStatus?.Invoke($"조회중입니다..잠시만 기다려주십시오.");

                //await Task.Run(() => db.ExecuteSql(strSql));

                db.ExecuteSql(strSql);

                if (db.nState)
                {
                    grid_State.DataSource = db.result;

                    RepositoryItemCheckEdit ri = new RepositoryItemCheckEdit();

                    ri.ValueChecked = "True";

                    ri.ValueUnchecked = "False";

                    ri.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;

                    ri.CheckBoxOptions.Style = DevExpress.XtraEditors.Controls.CheckBoxStyle.SvgCheckBox1;

                    gridView1.Columns["SEL"].ColumnEdit = ri;

                    gridView1.Columns["SEL"].Caption = "선택";
                    gridView1.Columns["GD_CD"].Caption = "제품코드";
                    gridView1.Columns["GD_NM"].Caption = "제품명";
                    gridView1.Columns["LOT_RULE"].Caption = "LOT규칙";
                    gridView1.Columns["STD_STR"].Caption = "기준";

                    gridView1.Columns["SEL"].Width = 20;

                    UpdateStatus?.Invoke($"[반제품 Lot 등록 부여 규칙] {db.result.Rows.Count} 행이 출력되었습니다.");

                    GridView view = gridView1;

                    view.PostEditor();
                    view.UpdateCurrentRow();

                    if (view.FocusedRowHandle >= 0 && view.IsDataRow(view.FocusedRowHandle))
                    {
                        DataRow row = view.GetDataRow(view.FocusedRowHandle);

                        fn_DetailSel(row.Field<string>("GD_CD"));

                        tbx_gdcdnm.Text = $"{row["GD_CD"].ToString()} / {row["GD_NM"].ToString()}";
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
