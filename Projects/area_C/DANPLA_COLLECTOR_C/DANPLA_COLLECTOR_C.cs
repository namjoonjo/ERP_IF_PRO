using DevExpress.XtraEditors.Repository;
using DevExpress.XtraReports.UI;
using DevExpress.XtraSplashScreen;
using RAZER_C.Danpla;
using RAZER_C.Label;
using RAZER_C.Labels;
using RAZER_C.Modules;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RAZER_C
{
    public partial class DANPLA_COLLECTOR_C : Form
    {
        private static string dbName = "ERP_2";

        MSSQL db = new MSSQL(dbName);

        CommonModule cm = new CommonModule();

        Dictionary<string, string> comboDic = new Dictionary<string, string>();

        Dictionary<string, string> outputVals = new Dictionary<string, string>();

        private string selectedFac = "";

        DataRow PsDataRow = null; DataRow BarcodeScanRow = null;

        public DANPLA_COLLECTOR_C()
        {
            InitializeComponent();

            initControl();
        }

        private void initControl()
        {
            try
            {
                fn_Comboinit();

                cbx_FACCD.SelectedIndexChanged += Cbx_FACCD_SelectedIndexChanged;

                tbx_Empno.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) fn_FindEmpInfo(); };

                btn_Danpla_Sel.Click += (s, e) => { fn_Danpla_SEL(string.Empty); };

                btn_Danpla_Del.Click += (s, e) => { fn_Danpla_DEL(); };

                btn_Danpla_Add.Click += (s, e) => { fn_Danpla_Add(); };

                btn_BarCode_Scan.Click += (s, e) => { fn_BarCode_Scan(); };

                tbx_BarCode.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) fn_BarCode_Scan(); };

                btn_BarCode_Del.Click += (s, e) => { fn_BarCode_Del(); };

                gridView1.RowClick += GridView1_RowClick;

                gridView3.RowClick += GridView3_RowClick;

                btn_Pallet_Add.Click += (s, e) => { fn_Pallet_Add(); };

                btn_Pallet_Sel.Click += (s, e) => { fn_Pallet_SEL(string.Empty); };

                btn_Pallet_Del.Click += (s, e) => { fn_Pallet_DEL(); };

                btn_Danpla_Label_Print.Click += (s, e) => { fn_DanplaLabelPrint(); };

                btn_Danpla_Pallet_Print.Click += (s, e) => { fn_PalletLabelPrint(); };

                chkExport.CheckedChanged += (s, e) => { fn_Pallet_SEL(string.Empty); };

                btn_Scan_Save.Click += (s, e) => { fn_ScanSave(); };

                lb_RealQty.Text = $"스캔 총 수량: {gridView2.RowCount}";

                gridView2.InitNewRow += GridView2_InitNewRow;

                fn_Danpla_SEL(string.Empty);

                fn_Pallet_SEL(string.Empty);

                tbx_Empno.Select();

            }
            catch (Exception ex)
            {

            }
        }

        private void Cbx_FACCD_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                selectedFac = cbx_FACCD.Text.Substring(0, 1);

                fn_Danpla_SEL(string.Empty);

                fn_Pallet_SEL(string.Empty);
            }
            catch(Exception ed)
            {

            }
        }


        private void fn_Comboinit()
        {
            try
            {
                cbx_FACCD.Items.Add("A관");
                cbx_FACCD.Items.Add("C관");

                comboDic.Add("A관", "01");
                comboDic.Add("C관", "02");

                cbx_FACCD.SelectedIndex = 1;

                selectedFac = "C";

            }
            catch(Exception ex)
            {

            }
        }

        private void fn_ScanSave()
        {
            try
            {
                if (DialogResult.No == MessageBox.Show($"저장하시겠습니까?", "단프라 적재 데이터 저장", MessageBoxButtons.YesNo, MessageBoxIcon.Question)) return;

                if(Convert.ToInt32(realQty.Value) != gridView2.RowCount)
                {
                    MessageBox.Show("입력하신수량과 실제 스캔된 수량이 일치하지 않습니다.\n확인부탁드립니다.", "저장", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    realQty.Select();

                    return;
                }

                if (string.IsNullOrEmpty(tbx_Empno.Text))
                {
                    MessageBox.Show("사원번호를 입력해주십시오.", "사원번호 입력", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    tbx_Empno.Select();

                    return;
                }

                DataTable updDt = new DataTable();

                updDt.Columns.Add("DAN_NO");
                updDt.Columns.Add("BARCODE");
                updDt.Columns.Add("LOT_NO");
                updDt.Columns.Add("MATE_NO");
                updDt.Columns.Add("EXPIR_DT");
                updDt.Columns.Add("YY_CNT");

                int rowCnt = gridView2.RowCount;

                for (int i = 0; i < rowCnt; i++)
                {
                    DataRow dr = updDt.NewRow();

                    dr["DAN_NO"] = gridView2.GetRowCellDisplayText(i, "DAN_NO");
                    dr["BARCODE"] = gridView2.GetRowCellDisplayText(i, "BARCODE");
                    dr["LOT_NO"] = gridView2.GetRowCellDisplayText(i, "LOT_NO");
                    dr["MATE_NO"] = gridView2.GetRowCellDisplayText(i, "MATE_NO");
                    dr["EXPIR_DT"] = gridView2.GetRowCellDisplayText(i, "EXPIR");
                    dr["YY_CNT"] = gridView2.GetRowCellDisplayText(i, "YY_CNT");

                    updDt.Rows.Add(dr);
                }

                string strSql = $"{dbName}.dbo.ST_DANPLA_COLLECTOR_BARCODE_UPD_AC";

                db.Parameter("@DAN_NO", tbx_DanplaNo.Text);
                db.Parameter("@ALL_SCAN_QTY", gridView2.RowCount);
                db.Parameter("@BAR_DATA", cm.DataTblToXML(updDt));
                db.Parameter("@PS_CD", tbx_Empno.Text);

                db.ExecuteNonSql(strSql);

                if (db.nState)
                {
                    if (!string.IsNullOrEmpty(db.sql_raise_error_msg))
                    {
                        MessageBox.Show($"{db.sql_raise_error_msg}\n정보전략팀에 문의 바랍니다.", "저장", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                        return;
                    }

                    MessageBox.Show($"저장되었습니다.", "저장", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    fn_Danpla_SEL(tbx_DanplaNo.Text);

                    tbx_BarCode.Text = string.Empty;
                }

            }
            catch (Exception ex)
            {

            }
        }

        private void fn_BarCode_Del()
        {
            try
            {
                string msg = string.Empty;

                DataTable deldt = new DataTable();

                deldt.Columns.Add("DAN_NO");
                deldt.Columns.Add("BARCODE");

                int rCnt = gridView2.RowCount;

                for (int i =0;i< rCnt; i++)
                {
                    if (gridView2.GetRowCellDisplayText(i, "SEL").Equals("Checked"))
                    {
                        DataRow dr = deldt.NewRow();

                        dr["DAN_NO"] = gridView2.GetRowCellDisplayText(i, "DAN_NO");

                        dr["BARCODE"] = gridView2.GetRowCellDisplayText(i, "BARCODE");

                        msg += gridView2.GetRowCellDisplayText(i, "BARCODE") + "\n";

                        deldt.Rows.Add(dr);
                    }
                }

                if(deldt.Rows.Count > 0)
                {
                    if (MessageBox.Show(msg + "위의 바코드가 삭제됩니다.\n삭제하시겠습니까?", "삭제", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No) return;

                    string strSql = $"{dbName}.dbo.ST_DANPLA_COLLECTOR_BARCODE_DEL_AC";

                    db.Parameter("@DAN_NO", tbx_DanplaNo.Text);
                    db.Parameter("@DEL_VAL", cm.DataTblToXML(deldt));

                    db.ExecuteNonSql(strSql);

                    if (db.nState)
                    {
                        if (string.IsNullOrEmpty(db.sql_raise_error_msg))
                        {
                            MessageBox.Show("삭제되었습니다.", "삭제", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }

                    fn_Danpla_SEL(tbx_DanplaNo.Text);

                    return;
                }


                MessageBox.Show("삭제할 바코드를 선택하여 주십시오.", "바코드 삭제", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (Exception ex)
            {

            }
        }

        private void GridView2_InitNewRow(object sender, DevExpress.XtraGrid.Views.Grid.InitNewRowEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView gv = sender as DevExpress.XtraGrid.Views.Grid.GridView;

                int ridx = gv.RowCount == 1 ? 0 : gv.RowCount - 1;

                gv.SetRowCellValue(e.RowHandle, "SEL", "True");

                gv.SetRowCellValue(e.RowHandle, "SEQ", ridx == 0 ? 1 : int.Parse(gv.GetRowCellDisplayText(ridx - 1, "SEQ")) + 1);

                gv.SetRowCellValue(e.RowHandle, "DAN_NO", tbx_DanplaNo.Text);

                gv.SetRowCellValue(e.RowHandle, "BARCODE", tbx_BarCode.Text);

                gv.SetRowCellValue(e.RowHandle, "LOT_NO", BarcodeScanRow["LOT_NO"].ToString());

                gv.SetRowCellValue(e.RowHandle, "GD_CD", BarcodeScanRow["GD_CD"].ToString());

                gv.SetRowCellValue(e.RowHandle, "MATE_NO", BarcodeScanRow["MATE_NO"].ToString());

                gv.SetRowCellValue(e.RowHandle, "EXPIR", BarcodeScanRow["EXPIR"].ToString());

                gv.SetRowCellValue(e.RowHandle, "YY_CNT", BarcodeScanRow["YY_CNT"].ToString());
            }
            catch(Exception ex)
            {

            }
        }

        private void fn_BarCode_Scan()
        {
            try
            {
                if (string.IsNullOrEmpty(tbx_DanplaNo.Text))
                {
                    MessageBox.Show("선택된 단프라 번호가 없습니다.\n확인부탁드립니다.", "바코드 스캔", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    tbx_BarCode.SelectAll();

                    return;
                }

                if (string.IsNullOrEmpty(tbx_BarCode.Text))
                {
                    MessageBox.Show("바코드 스캔란이 비어있습니다.\n확인부탁드립니다.", "바코드 스캔", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    return;
                }


                if (tbx_BarCode.Text.Trim().Length > 20)
                {
                    for(int i = 0; i < gridView2.RowCount; i++)
                    {
                        string bData = gridView2.GetRowCellDisplayText(i, "BARCODE");

                        if (bData.Equals(tbx_BarCode.Text.Trim()))
                        {
                            MessageBox.Show("이미 스캔 완료된 바코드입니다.\n확인부탁드립니다.", "바코드 스캔", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                            tbx_BarCode.SelectAll();

                            return;
                        }
                    }
                }

                string strSql = string.Empty;

                if(tbx_BarCode.Text.Trim().Length < 20)
                {
                    tbx_BarCode.SelectAll();

                    return;
                }

                //if (tbx_BarCode.Text.Trim().Length < 20)
                //{
                //    strSql = $"{dbName}.dbo.ST_DANPLA_COLLECTOR_BARCODE_SIMPLE_INFO_SEL_AC";

                //    db.Parameter("@BARCODE", tbx_BarCode.Text);

                //    db.ExecuteSql(strSql);

                //    if (db.nState)
                //    {
                //        if (db.result.Rows.Count > 0)
                //        {
                //            DataRow dr = db.result.Rows[0];

                //            INSERT_MATENO_FORDANPLA imf = new INSERT_MATENO_FORDANPLA(this, dr["GD_CD"].ToString(), dr["NM_NM"].ToString());

                //            imf.ShowDialog();
                //        }
                //        else
                //        {
                //            MessageBox.Show($"[{tbx_BarCode.Text}] 해당 바코드를 찾을 수 없습니다.\n확인부탁드립니다.", "경고", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                //            return;
                //        }
                //    }
                //}
                //else
                //{
                //    tbx_result_MATE_NO.Text = tbx_BarCode.Text.Trim().Substring(18, 4);
                //}

                strSql = $"{dbName}.dbo.ST_DANPLA_COLLECTOR_BARCODE_DETAIL_SEL_AC";

                db.Parameter("@BARCODE", tbx_BarCode.Text);
                db.Parameter("@MATE_NO", tbx_result_MATE_NO.Text);

                db.ExecuteSql(strSql);

                if (db.nState)
                {
                    if (!string.IsNullOrEmpty(db.sql_raise_error_msg))
                    {
                        MessageBox.Show($"{db.sql_raise_error_msg}", "바코드 조회", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        return;
                    }

                    if (db.result.Rows.Count > 0)
                    {
                        BarcodeScanRow = db.result.Rows[0];

                        if (!fn_IsSameGDCD(BarcodeScanRow["GD_CD"].ToString()))
                        {
                            MessageBox.Show($"적재된 품목과 같은 품목이 아닙니다.\n\n적재된 품목 : {gridView2.GetRowCellDisplayText(0,"GD_CD")}\n새로 스캔된 품목: {BarcodeScanRow["GD_CD"].ToString()}", "스캔", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                            tbx_BarCode.SelectAll();

                            return;
                        }

                        if (BarcodeScanRow["SCANORNOT"].ToString().Equals("X"))
                        {
                            tbx_ScanResult.Text = "제한기간이 유효기간을 지났습니다.";

                            tbx_ScanResult.ForeColor = Color.Red;

                            tbx_ScanResult.BackColor = Color.Yellow;

                            MessageBox.Show(tbx_ScanResult.Text, "스캔", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                            tbx_BarCode.SelectAll();
                        }

                        if (BarcodeScanRow["CHUL_GU_BUNRYU"].ToString().Equals("N"))
                        {
                            tbx_CHULGO.Text = "출고제한기간을 초과하였습니다.";

                            tbx_CHULGO.ForeColor = Color.Red;

                            tbx_CHULGO.BackColor = Color.Yellow;

                            MessageBox.Show(tbx_CHULGO.Text, "스캔", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                            tbx_BarCode.SelectAll();
                        }

                        if (BarcodeScanRow["SCANORNOT"].ToString().Equals("O") && BarcodeScanRow["CHUL_GU_BUNRYU"].ToString().Equals("Y"))
                        {
                            tbx_ScanResult.Text = "정상입니다.";

                            tbx_ScanResult.ForeColor = Color.Yellow;

                            tbx_ScanResult.BackColor = Color.Teal;

                            tbx_CHULGO.Text = "정상입니다.";

                            tbx_CHULGO.ForeColor = Color.Yellow;

                            tbx_CHULGO.BackColor = Color.Teal;

                            tbx_GD_CD.Text = BarcodeScanRow["GD_CD"].ToString();

                            tbx_GD_NM.Text = BarcodeScanRow["GD_NM"].ToString();

                            tbx_LotNo.Text = BarcodeScanRow["LOT_NO"].ToString();

                            tbx_result_MATE_NO.Text = BarcodeScanRow["MATE_NO"].ToString();

                            tbx_EXPIRDT.Text = BarcodeScanRow["EXPIR"].ToString();

                            gridView2.AddNewRow();

                            gridView2.UpdateCurrentRow();

                            tbx_BarCode.SelectAll();
                        }

                        lb_RealQty.Text = $"스캔 총 수량 : {gridView2.RowCount}";

                        return;
                    }

                    tbx_GD_CD.Text = "-";

                    tbx_GD_NM.Text = "-";

                    tbx_LotNo.Text = "-";

                    tbx_EXPIRDT.Text = "-";

                    tbx_result_MATE_NO.Text = "-";

                    tbx_CHULGO.Text = "-";

                    tbx_ScanResult.Text = "해당 바코드의 기준정보를 가져올 수 없습니다.";

                    tbx_ScanResult.ForeColor = Color.Red;

                    tbx_ScanResult.BackColor = Color.Yellow;

                    MessageBox.Show(tbx_ScanResult.Text, "스캔", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    tbx_BarCode.SelectAll();

                    return;
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void fn_DanplaLabelPrint()
        {
            try
            {
                if (string.IsNullOrEmpty(tbx_DanplaNo.Text))
                {
                    MessageBox.Show($"단프라 번호가 선택되지 않았습니다.", "단프라 라벨 출력", MessageBoxButtons.OK, MessageBoxIcon.Question);

                    return;
                }

                if (DialogResult.No == MessageBox.Show($"[{gridView1.GetRowCellDisplayText(gridView1.FocusedRowHandle, "DAN_NO")}]을 출력 하시겠습니까?", "단프라 라벨 출력", MessageBoxButtons.YesNo, MessageBoxIcon.Question)) return;

                Danpla_Label xx = new Danpla_Label();

                xx.xrBarCode1.Text = $"{gridView1.GetRowCellDisplayText(gridView1.FocusedRowHandle, "DAN_NO")} {gridView1.GetRowCellDisplayText(gridView1.FocusedRowHandle, "GD_CD")} {gridView1.GetRowCellDisplayText(gridView1.FocusedRowHandle, "LOAD_QTY")}";

                xx.DAN_NO.Text = gridView1.GetRowCellDisplayText(gridView1.FocusedRowHandle, "DAN_NO");

                xx.GD_CD.Text = gridView1.GetRowCellDisplayText(gridView1.FocusedRowHandle, "GD_CD");

                xx.SELL_NM.Text = gridView1.GetRowCellDisplayText(gridView1.FocusedRowHandle, "GD_NM");

                xx.CNT.Text = gridView1.GetRowCellDisplayText(gridView1.FocusedRowHandle, "LOAD_QTY");

                using (ReportPrintTool printTool = new ReportPrintTool(xx))
                {
                    //printTool.ShowPreviewDialog();

                    printTool.Print();
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void fn_PalletLabelPrint()
        {
            try
            {
                if (string.IsNullOrEmpty(tbx_PalletNo.Text))
                {
                    MessageBox.Show("출력될 파레트번호가 조회되지 않았습니다.\n확인부탁드립니다.", "파레트 출력", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    return;
                }

                Pallet_Label xx = new Pallet_Label(tbx_PalletNo.Text);

                for (int i = 0; i < 2; i++)
                {
                    using (ReportPrintTool printTool = new ReportPrintTool(xx))
                    {
                        //printTool.ShowPreviewDialog();

                        printTool.Print();
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void GridView1_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
            try
            {
                if (e.RowHandle < 0) return;

                this.tbx_DanplaNo.Text = gridView1.GetRowCellDisplayText(e.RowHandle, "DAN_NO");

                fn_Barcode_Sel();
            }
            catch(Exception ex)
            {

            }
        }

        private void GridView3_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
            try
            {
                if (e.RowHandle < 0) return;

                this.tbx_PalletNo.Text = gridView3.GetRowCellDisplayText(e.RowHandle, "PALLET_NO");
            }
            catch (Exception ex)
            {

            }
        }


        private void fn_Pallet_DEL()
        {
            try
            {
                if (gridView3.SelectedRowsCount == 0)
                {
                    MessageBox.Show("선택된 파레트 번호가 없습니다.\n확인부탁드립니다.", "파레트 번호 삭제", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    return;
                }

                if (DialogResult.No == MessageBox.Show($"파레트 번호 : {tbx_PalletNo.Text}를 삭제하시겠습니까?", "파레트 번호 삭제", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)) return;

                string strSql = $"{dbName}.dbo.ST_DANPLA_COLLECTOR_PALLET_DEL_AC";

                db.Parameter("@PALLET_NO", tbx_PalletNo.Text);

                db.ExecuteNonSql(strSql);

                if (db.nState)
                {
                    MessageBox.Show("삭제되었습니다.", "파레트 번호 삭제", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    tbx_PalletNo.Text = string.Empty;

                    fn_Pallet_SEL(tbx_PalletNo.Text);

                    return;
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void fn_Pallet_Add()
        {
            try
            {
                if (DialogResult.No == MessageBox.Show($"파레트번호를 생성하시겠습니까?", "파레트 번호 생성", MessageBoxButtons.YesNo, MessageBoxIcon.Question)) return;

                if (string.IsNullOrEmpty(lb_EmpNo.Text))
                {
                    MessageBox.Show("사번을 입력하여 주십시오.", "경고", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    tbx_Empno.Select();

                    return;
                }

                Rules rClass = null;

                if (chkExport.Checked)
                {
                    rClass = new Rules();

                    INSERT_PALLET_INFO ipi = new INSERT_PALLET_INFO(rClass);

                    ipi.ShowDialog();
                }

                string strSql = $"{dbName}.dbo.ST_DANPLA_COLLECTOR_PALLET_INS_AC";

                db.Parameter("@FAC_CD", rClass != null ? rClass.FacCd : "C");
                db.Parameter("@PS_CD", tbx_Empno.Text);
                db.Parameter("@RULES", chkExport.Checked ? rClass.Rulestr : string.Empty);
                db.Parameter("@PALLET_NO", true, 50);

                db.ExecuteNonSql(strSql, outputVals);

                if (db.nState)
                {
                    if (string.IsNullOrEmpty(db.sql_raise_error_msg))
                    {
                        tbx_PalletNo.Text = outputVals["@PALLET_NO"];

                        fn_Pallet_SEL(tbx_PalletNo.Text);

                        return;
                    }
                }

                MessageBox.Show("단프라 번호가 생성되지 않았습니다.\n확인부탁드립니다.", "단프라 번호 추가 생성", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {

            }
        }


        private bool fn_IsSameGDCD(string strGDCD)
        {
            try
            {
                if (gridView2.RowCount == 0) return true;

                for(int i=0;i<gridView2.RowCount;i++)
                {
                    if (!gridView2.GetRowCellDisplayText(i, "GD_CD").Equals(strGDCD)) return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private void fn_Pallet_SEL(string PALLET_NO)
        {
            try
            {
                string strSql = $"{dbName}.dbo.ST_DANPLA_COLLECTOR_PALLET_SEL_AC";

                db.Parameter("@PALLET_NO", PALLET_NO);
                db.Parameter("@FAC_CD", selectedFac);
                db.Parameter("@CHK_ISEXPORT", Convert.ToInt32(chkExport.Checked));

                db.ExecuteSql(strSql);

                if (db.nState)
                {
                    gridControl3.DataSource = db.result;

                    gridView3.Columns["SEQ"].Caption = "순번";

                    gridView3.Columns["PALLET_NO"].Caption = "파레트 번호";

                    gridView3.Columns["IN_PSCD"].Caption = "생성자";

                    gridView3.Columns["IN_DT"].Caption = "생성시간";

                    gridView3.Columns["SEQ"].Width = 50;
                    gridView3.Columns["PALLET_NO"].Width = 300;
                    gridView3.Columns["IN_PSCD"].Width = 80;
                    gridView3.Columns["IN_DT"].Width = 100;

                    tbx_PalletNo.Text = gridView3.GetRowCellDisplayText(gridView3.GetSelectedRows()[0], "PALLET_NO");

                    gridView3.OptionsBehavior.Editable = false;

                    gridView3.OptionsBehavior.EditorShowMode = DevExpress.Utils.EditorShowMode.MouseDown;

                    gridView3.OptionsView.ShowIndicator = false;

                    gridView3.OptionsView.ColumnHeaderAutoHeight = DevExpress.Utils.DefaultBoolean.True;
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void fn_Danpla_Add()
        {
            try
            {
                if (DialogResult.No == MessageBox.Show("단프라번호를 추가로 생성하시겠습니까?", "단프라 추가 생성", MessageBoxButtons.YesNo, MessageBoxIcon.Question)) return;


                if (string.IsNullOrEmpty(lb_EmpNo.Text))
                {
                    MessageBox.Show("사원정보를 입력해 주십시오.", "단프라 번호 추가 생성", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    tbx_Empno.Focus();

                    return;
                }

                string strSql = $"{dbName}.dbo.ST_DANPLA_COLLECTOR_DANPLA_INS"; 

                db.Parameter("@FAC_CD", selectedFac);
                db.Parameter("@PS_CD", tbx_Empno.Text);
                db.Parameter("@DAN_NO", true, 50);

                db.ExecuteNonSql(strSql, outputVals);

                if (db.nState)
                {
                    if (string.IsNullOrEmpty(db.sql_raise_error_msg))
                    {
                        tbx_DanplaNo.Text = outputVals["@DAN_NO"];

                        fn_Danpla_SEL(tbx_DanplaNo.Text);

                        tbx_BarCode.Focus();

                        return;
                    }
                }

                MessageBox.Show("단프라 번호가 생성되지 않았습니다.\n확인부탁드립니다.", "단프라 번호 추가 생성", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                return;
            }
            catch (Exception ex)
            {

            }
        }

        private void fn_FindEmpInfo()
        {
            try
            {
                if (string.IsNullOrEmpty(tbx_Empno.Text))
                {
                    MessageBox.Show("사원번호를 입력해주십시오.", "사원번호 입력", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    return;
                }

                string strSql = $"{dbName}.dbo.ST_DANPLA_COLLECTOR_EMP_INFO_SEL";

                db.Parameter("@EMP_NO", tbx_Empno.Text);

                db.ExecuteSql(strSql);

                if (db.nState)
                {
                    if (db.result.Rows.Count > 0)
                    {
                        PsDataRow = db.result.Rows[0];

                        lb_EmpNo.Text = $"{PsDataRow["EMPNAME"].ToString()} / {PsDataRow["DEPTNAME"].ToString()}";

                        tbx_DanplaNo.Focus();

                        return;
                    }
                }

                MessageBox.Show($"{tbx_Empno.Text}에 해당하는 사원정보가 없습니다.\n확인부탁드립니다.", "사원번호 입력", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                lb_EmpNo.Text = string.Empty;

                tbx_Empno.SelectAll();

                return;
            }
            catch (Exception ex)
            {

            }
        }

        private void fn_Danpla_SEL(string DAN_NO)
        {
            try
            {
                string strSql = $"{dbName}.dbo.ST_DANPLA_COLLECTOR_DANPLA_SEL_AC"; // ERP_2

                db.Parameter("@DAN_NO", DAN_NO);
                db.Parameter("@FAC_CD", selectedFac);

                db.ExecuteSql(strSql);

                if (db.nState)
                {
                    gridControl1.DataSource = db.result;

                    gridView1.Columns["DAN_NO"].Caption = "단프라 번호";

                    gridView1.Columns["GD_CD"].Caption = "품목코드";

                    gridView1.Columns["GD_NM"].Caption = "품명";

                    gridView1.Columns["LOAD_ST"].Caption = "적재날짜";

                    gridView1.Columns["LOAD_QTY"].Caption = "단프라 적재 수량";

                    tbx_DanplaNo.Text = gridView1.GetRowCellDisplayText(gridView1.GetSelectedRows()[0], "DAN_NO");

                    gridView1.OptionsBehavior.Editable = false;

                    gridView1.OptionsBehavior.EditorShowMode = DevExpress.Utils.EditorShowMode.MouseDown;

                    gridView1.OptionsView.ShowIndicator = false;

                    gridView1.OptionsView.ColumnHeaderAutoHeight = DevExpress.Utils.DefaultBoolean.True;


                }

                fn_Barcode_Sel();

            }
            catch (Exception ex)
            {

            }
        }

        private void fn_Danpla_DEL()
        {
            try
            {
                if (gridView1.GetSelectedRows().Length == 0)
                {
                    MessageBox.Show("선택된 단프라 번호가 없습니다.\n확인부탁드립니다.", "단프라 번호 삭제", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    return;
                }


                string selectedDanplaNo = gridView1.GetRowCellDisplayText(gridView1.FocusedRowHandle, "DAN_NO");

                if (DialogResult.No == MessageBox.Show($"단프라 번호 : {selectedDanplaNo}를 삭제하시겠습니까?", "단프라 번호 삭제", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)) return;

                string strSql = $"{dbName}.dbo.ST_DANPLA_COLLECTOR_DANPLA_DEL"; 

                db.Parameter("@DAN_NO", selectedDanplaNo);

                db.ExecuteNonSql(strSql);

                if (db.nState)
                {
                    MessageBox.Show("삭제되었습니다.", "단프라 번호 삭제", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    tbx_DanplaNo.Text = string.Empty;

                    fn_Danpla_SEL(string.Empty);

                    return;
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void fn_Barcode_Sel()
        {
            try
            {
                string strSql = $"{dbName}.dbo.ST_DANPLA_COLLECTOR_BARCODE_SEL_AC";

                db.Parameter("@DAN_NO", tbx_DanplaNo.Text);

                if (!splashScreenManager1.IsSplashFormVisible) splashScreenManager1.ShowWaitForm();

                db.ExecuteSql(strSql);

                if (splashScreenManager1.IsSplashFormVisible) splashScreenManager1.CloseWaitForm();

                if (db.nState)
                {
                    gridControl2.DataSource = db.result;

                    gridView2.Columns["SEL"].Caption = "선택";

                    gridView2.Columns["SEQ"].Caption = "순번";

                    gridView2.Columns["DAN_NO"].Caption = "단프라번호";
                        
                    gridView2.Columns["BARCODE"].Caption = "바코드";
                        
                    gridView2.Columns["LOT_NO"].Caption = "LotNo";
                        
                    gridView2.Columns["GD_CD"].Caption = "품목코드";
                        
                    gridView2.Columns["MATE_NO"].Caption = "멸균번호"; 
                        
                    gridView2.Columns["EXPIR"].Caption = "유효기간";

                    gridView2.Columns["YY_CNT"].Visible = false;

                    gridView2.Columns["SEQ"].OptionsColumn.ReadOnly = true;
                    gridView2.Columns["DAN_NO"].OptionsColumn.ReadOnly = true;
                    gridView2.Columns["BARCODE"].OptionsColumn.ReadOnly = true;
                    gridView2.Columns["LOT_NO"].OptionsColumn.ReadOnly = true;
                    gridView2.Columns["GD_CD"].OptionsColumn.ReadOnly = true;
                    gridView2.Columns["MATE_NO"].OptionsColumn.ReadOnly = true;
                    gridView2.Columns["EXPIR"].OptionsColumn.ReadOnly = true;

                    gridView2.Columns["SEL"].Width = 50;
                    gridView2.Columns["SEQ"].Width = 50;
                    gridView2.Columns["DAN_NO"].Width = 150;
                    gridView2.Columns["BARCODE"].Width = 300;
                    gridView2.Columns["LOT_NO"].Width = 100;
                    gridView2.Columns["GD_CD"].Width = 100;
                    gridView2.Columns["MATE_NO"].Width = 70;
                    gridView2.Columns["EXPIR"].Width = 100;


                    RepositoryItemCheckEdit ri = new RepositoryItemCheckEdit();

                    ri.ValueChecked = "True";

                    ri.ValueUnchecked = "False";

                    ri.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;

                    ri.CheckBoxOptions.Style = DevExpress.XtraEditors.Controls.CheckBoxStyle.SvgCheckBox1;

                    gridView2.Columns["SEL"].ColumnEdit = ri;

                    gridView2.OptionsView.ShowIndicator = false;

                    lb_RealQty.Text = $"스캔 총 수량 : {gridView2.RowCount}";

                    tbx_BarCode.Select();
                }
            }
            catch (Exception ex)
            {

            }
        }

    }

    public class Rules
    {
        public string OrderNo { get; set; }

        public string Date { get; set; }

        public string FacCd { get; set; }

        public string Rulestr { get; set; }

    }
}
