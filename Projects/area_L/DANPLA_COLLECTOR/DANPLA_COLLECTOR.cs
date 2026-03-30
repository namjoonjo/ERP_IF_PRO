using area_L.Labels;
using area_L.Modules;
using DevExpress.Xpo.DB;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraReports.UI;
using DevExpress.XtraSplashScreen;
using area_L.Modules;
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
    public partial class DANPLA_COLLECTOR : Form
    {
        private static string dbName = "ERP_2";

        MSSQL db = new MSSQL(dbName);

        CommonModule cm = new CommonModule();

        Dictionary<string, string> comboDic = new Dictionary<string, string>();

        Dictionary<string, string> outputVals = new Dictionary<string, string>();

        DataTable BarCode = new DataTable();

        DataTable Pallet = new DataTable();

        DataRow PsDataRow = null;

        private string kind = string.Empty;

        public DANPLA_COLLECTOR()
        {
            InitializeComponent();

            initControl();
        }

        private void initControl()
        {
            try
            {

                cm.SetGridRowHeader(grid_Danpla, -1);

                cm.SetGridRowHeader(grid_BarCode, -1);

                cm.SetGridRowHeader(grid_Pallet, -1);

                BarCode.Columns.Add("SEQ");
                BarCode.Columns.Add("DAN_NO");
                BarCode.Columns.Add("BARCODE");
                BarCode.Columns.Add("LOT_NO");
                BarCode.Columns.Add("GD_CD");
                BarCode.Columns.Add("MATE_NO");
                BarCode.Columns.Add("EXPIR");
                BarCode.Columns.Add("YY_CNT");

                Pallet.Columns.Add("SEQ");
                Pallet.Columns.Add("PALLET_NO");

                tbx_Empno.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) fn_FindEmpInfo(); };

                btn_Danpla_Sel.Click += (s, e) => { fn_Danpla_SEL(); };

                btn_Danpla_Del.Click += (s, e) => { fn_Danpla_DEL(); };

                btn_Danpla_Add.Click += (s, e) => { fn_Danpla_Add(); };

                tbx_DanplaNo.KeyDown += (s, e) => { if(e.KeyCode == Keys.Enter) fn_Danpla_SEL(); };

                tbx_PalletNo.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) fn_Pallet_SEL(); };

                btn_BarCode_Scan.Click += (s, e) => { fn_BarCode_Scan(); };

                tbx_BarCode.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) fn_BarCode_Scan(); };

                btn_BarCode_Del.Click += (s, e) => { fn_BarCode_Del(); };

                grid_Danpla.CellContentClick += (s, e) => { this.tbx_DanplaNo.Text = grid_Danpla.SelectedRows[0].Cells["DAN_NO"].Value.ToString(); fn_Barcode_Sel(); };

                btn_Pallet_Add.Click += (s, e) => { fn_Pallet_Add(); };

                btn_Pallet_Sel.Click += (s, e) => { fn_Pallet_SEL(); };

                btn_Pallet_Del.Click += (s, e) => { fn_Pallet_DEL(); };

                btn_Danpla_Label_Print.Click += (s, e) => { fn_DanplaLabelPrint(); };

                btn_Danpla_Pallet_Print.Click += (s, e) => { fn_PalletLabelPrint(); };

                btn_Scan_Save.Click += (s, e) => { fn_ScanSave(); };

                lb_RealQty.Text = $"스캔 총 수량: {fn_AllScanQty()}";

                tbx_Empno.Select();

            }
            catch (Exception ex)
            {

            }
        }

        private void fn_ScanSave()
        {
            try
            {
                if (DialogResult.No == MessageBox.Show($"저장하시겠습니까?", "단프라 적재 데이터 저장", MessageBoxButtons.YesNo, MessageBoxIcon.Question)) return;

                if (string.IsNullOrEmpty(tbx_Empno.Text))
                {
                    MessageBox.Show("사원번호를 입력해주십시오.", "사원번호 입력", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    return;
                }

                string strSql = $"{dbName}.dbo.ST_DANPLA_COLLECTOR_BARCODE_UPD";

                db.Parameter("@DAN_NO", grid_Danpla.SelectedRows[0].Cells["DAN_NO"].Value.ToString());
                db.Parameter("@ALL_SCAN_QTY", Convert.ToInt32(fn_AllScanQty()));
                db.Parameter("@BAR_DATA", BarCode.Rows.Count == 0 ? string.Empty : cm.DataTblToXML(BarCode.Copy()));
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

                    fn_Danpla_SEL();

                    tbx_BarCode.Text = string.Empty;
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
                if(grid_Danpla.SelectedRows.Count == 0)
                {
                    MessageBox.Show($"단프라 번호가 선택되지 않았습니다.", "단프라 라벨 출력", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    return;
                }

                DataGridViewRow selectedRow = grid_Danpla.SelectedRows[0];

                if (DialogResult.No == MessageBox.Show($"[{selectedRow.Cells["DAN_NO"].Value}]을 출력 하시겠습니까?","단프라 라벨 출력",MessageBoxButtons.YesNo,MessageBoxIcon.Question)) return;

                Danpla_Label xx = new Danpla_Label();

                xx.xrBarCode1.Text = selectedRow.Cells["DAN_NO"].Value.ToString();

                xx.DAN_NO.Text = selectedRow.Cells["DAN_NO"].Value.ToString();

                xx.GD_CD.Text = selectedRow.Cells["GD_CD"].Value.ToString();

                xx.SELL_NM.Text = selectedRow.Cells["GD_NM"].Value.ToString();

                xx.CNT.Text = selectedRow.Cells["LOAD_QTY"].Value.ToString();

                using (ReportPrintTool printTool = new ReportPrintTool(xx))
                {
                    //printTool.ShowPreviewDialog();

                    printTool.Print();
                }
            }
            catch(Exception ex)
            {

            }
        }

        private void fn_PalletLabelPrint()
        {
            try
            {
                if (Pallet.Rows.Count == 0)
                {
                    MessageBox.Show("출력될 파레트번호가 조회되지 않았습니다.\n확인부탁드립니다.", "파레트 출력", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    return;
                }

                DataGridViewRow selectedRow = grid_Pallet.SelectedRows[0];

                Pallet_Label xx = new Pallet_Label(selectedRow.Cells["PALLET_NO"].Value.ToString());

                for(int i = 0; i < Convert.ToInt32(inputQty_Pallet.Value); i++)
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


       

        private void fn_BarCode_Scan()
        {
            try
            {
                if(grid_Danpla.SelectedRows.Count == 0)
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

                if(tbx_BarCode.Text.Trim().Length > 20)
                {
                    foreach (DataGridViewRow dgr in grid_BarCode.Rows)
                    {
                        if (dgr.Cells["BARCODE"].Value.ToString().Equals(tbx_BarCode.Text.Trim()))
                        {
                            MessageBox.Show("이미 스캔 완료된 바코드입니다.\n확인부탁드립니다.", "바코드 스캔", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                            tbx_BarCode.SelectAll();

                            return;
                        }
                    }
                }

                string strSql = $"{dbName}.dbo.ST_DANPLA_COLLECTOR_BARCODE_DETAIL_SEL";
                
                db.Parameter("@BARCODE", tbx_BarCode.Text);
                db.Parameter("@MATE_NO", string.Empty);

                db.ExecuteSql(strSql);

                if (db.nState)
                {
                    if(db.result.Rows.Count > 0)
                    {
                        DataRow dr = db.result.Rows[0];

                        if (!fn_IsSameGDCD(dr["GD_CD"].ToString()))
                        {
                            //MessageBox.Show($"적재된 품목과 같은 품목이 아닙니다.\n\n적재된 품목 : {BarCode.Rows[0]["GD_CD"].ToString()}\n새로 스캔된 품목: {dr["GD_CD"].ToString()}", "스캔", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                            new CustomMessageBox($"적재된 품목과 같은 품목이 아닙니다.\n\n적재된 품목 : {BarCode.Rows[0]["GD_CD"].ToString()}\n새로 스캔된 품목: {dr["GD_CD"].ToString()}", "스캔").ShowDialog();

                            tbx_BarCode.SelectAll();

                            return;
                        }

                        DataRow barcodeDataRow = BarCode.NewRow();

                        int max = 0;
                        foreach (DataRow bdr in BarCode.Rows) 
                        {
                            if (max < Convert.ToInt32(bdr["SEQ"])) max = Convert.ToInt32(bdr["SEQ"]);
                        }

                        barcodeDataRow["SEQ"] = BarCode.Rows.Count == 0 ? 1 : max + 1;

                        barcodeDataRow["DAN_NO"] = grid_Danpla.SelectedRows[0].Cells["DAN_NO"].Value.ToString();

                        barcodeDataRow["BARCODE"] = tbx_BarCode.Text;

                        barcodeDataRow["LOT_NO"] = tbx_LotNo.Text = dr["LOT_NO"].ToString();

                        barcodeDataRow["MATE_NO"] = tbx_result_MATE_NO.Text = dr["MATE_NO"].ToString();

                        tbx_EXPIRDT.Text = dr["EXPIR"].ToString();

                        barcodeDataRow["EXPIR"] = tbx_BarCode.Text.Length >= 20 ? dr["EXPIR"] : DBNull.Value;

                        barcodeDataRow["GD_CD"]  = tbx_GD_CD.Text = dr["GD_CD"].ToString();

                        barcodeDataRow["YY_CNT"] = tbx_BarCode.Text.Length >= 20 ? string.Empty : dr["YY_CNT"].ToString();

                        tbx_GD_NM.Text = dr["GD_NM"].ToString();

                        tbx_LIMITDT.Text = dr["LIMIT_DT"].ToString();

                        if (dr["SCANORNOT"].ToString().Equals("X"))
                        {
                            tbx_ScanResult.Text = "제한기간이 유효기간을 지났습니다.";

                            tbx_ScanResult.ForeColor = Color.Red;

                            tbx_ScanResult.BackColor = Color.Yellow;

                            new CustomMessageBox(tbx_ScanResult.Text, "스캔").ShowDialog();

                            tbx_BarCode.SelectAll();
                        }
                        else
                        {
                            tbx_ScanResult.Text = "정상입니다.";

                            tbx_ScanResult.ForeColor = Color.Yellow;

                            tbx_ScanResult.BackColor = Color.Teal;

                            BarCode.Rows.Add(barcodeDataRow);

                            tbx_BarCode.SelectAll();
                        }

                        grid_BarCode.DataSource = BarCode;

                        grid_BarCode.Columns["SEQ"].HeaderText = "순번";

                        grid_BarCode.Columns["DAN_NO"].HeaderText = "단프라번호";

                        grid_BarCode.Columns["BARCODE"].HeaderText = "바코드";

                        grid_BarCode.Columns["LOT_NO"].HeaderText = "LotNo";

                        grid_BarCode.Columns["MATE_NO"].HeaderText = "멸균번호";

                        grid_BarCode.Columns["EXPIR"].HeaderText = "만료일자";

                        grid_BarCode.ClearSelection();

                        grid_BarCode.Rows[grid_BarCode.Rows.Count - 1].Selected = true;

                        grid_BarCode.FirstDisplayedScrollingRowIndex = grid_BarCode.SelectedRows[0].Index;

                        lb_RealQty.Text = $"스캔 총 수량 : {fn_AllScanQty()}";

                        return;
                    }

                    tbx_GD_CD.Text = "-";

                    tbx_GD_NM.Text = "-";

                    tbx_LotNo.Text = "-";

                    tbx_EXPIRDT.Text = "-";

                    tbx_result_MATE_NO.Text = "-";

                    tbx_LIMITDT.Text = "-";

                    tbx_ScanResult.Text = "해당 바코드의 기준정보를 가져올 수 없습니다.";

                    tbx_ScanResult.ForeColor = Color.Red;

                    tbx_ScanResult.BackColor = Color.Yellow;

                    new CustomMessageBox(tbx_ScanResult.Text, "스캔").ShowDialog();

                    tbx_BarCode.SelectAll();

                    return;
                }
            }
            catch (Exception ex)
            {

            }
        }

        private bool fn_IsSameGDCD(string strGDCD)
        {
            try
            {
                if (BarCode.Rows.Count == 0) return true;

                foreach (DataRow row in BarCode.Rows) 
                {
                    if (!row["GD_CD"].ToString().Equals(strGDCD)) return false;
                }

                return true;
            }
            catch (Exception ex) 
            {
                return false;
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

                string strSql = $"{dbName}.dbo.ST_DANPLA_COLLECTOR_DANPLA_INS"; // ERP_2

                db.Parameter("@FAC_CD", "L");
                db.Parameter("@PS_CD", tbx_Empno.Text);
                db.Parameter("@DAN_NO", true, 50);

                 db.ExecuteNonSql(strSql, outputVals);

                if (db.nState)
                {
                    if (string.IsNullOrEmpty(db.sql_raise_error_msg))
                    {
                        tbx_DanplaNo.Text = outputVals["@DAN_NO"];

                        fn_Danpla_SEL();

                        tbx_BarCode.Focus();

                        return;
                    }
                }

                MessageBox.Show("단프라 번호가 생성되지 않았습니다.\n확인부탁드립니다.", "단프라 번호 추가 생성", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                return;
            }
            catch(Exception ex)
            {

            }
        }

        private void fn_Danpla_DEL()
        {
            try
            {
                if(grid_Danpla.SelectedRows.Count == 0)
                {
                    MessageBox.Show("선택된 단프라 번호가 없습니다.\n확인부탁드립니다.", "단프라 번호 삭제", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    return;
                }

                if (DialogResult.No == MessageBox.Show($"단프라 번호 : {grid_Danpla.SelectedRows[0].Cells["DAN_NO"].Value.ToString()}를 삭제하시겠습니까?", "단프라 번호 삭제", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)) return;

                string strSql = $"{dbName}.dbo.ST_DANPLA_COLLECTOR_DANPLA_DEL"; // ERP_2

                db.Parameter("@DAN_NO", grid_Danpla.SelectedRows[0].Cells["DAN_NO"].Value.ToString());

                db.ExecuteNonSql(strSql);

                if (db.nState)
                {
                    MessageBox.Show("삭제되었습니다.", "단프라 번호 삭제", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    tbx_DanplaNo.Text = string.Empty;

                    fn_Danpla_SEL();

                    return;
                }
            }
            catch(Exception ex)
            {

            }
        }


        private void fn_Danpla_SEL()
        {
            try
            {
                string strSql = $"{dbName}.dbo.ST_DANPLA_COLLECTOR_DANPLA_SEL"; // ERP_2

                db.Parameter("@DAN_NO", tbx_DanplaNo.Text);

                db.ExecuteSql(strSql);

                if (db.nState)
                {
                    this.grid_Danpla.DataSource = db.result;

                    grid_Danpla.Columns["DAN_NO"].HeaderText = "단프라 번호";

                    grid_Danpla.Columns["GD_CD"].HeaderText = "품목코드";

                    grid_Danpla.Columns["GD_NM"].HeaderText = "품명";

                    grid_Danpla.Columns["LOAD_ST"].HeaderText = "적재날짜";

                    grid_Danpla.Columns["LOAD_QTY"].HeaderText = "단프라 적재 수량";

                    tbx_DanplaNo.Text = grid_Danpla.SelectedRows[0].Cells["DAN_NO"].Value.ToString();
                }

                fn_Barcode_Sel();

            }
            catch(Exception ex)
            {

            }
        }

        private void fn_Pallet_DEL()
        {
            try
            {
                if (grid_Pallet.SelectedRows.Count == 0)
                {
                    MessageBox.Show("선택된 파레트 번호가 없습니다.\n확인부탁드립니다.", "파레트 번호 삭제", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    return;
                }

                if (DialogResult.No == MessageBox.Show($"파레트 번호 : {grid_Pallet.SelectedRows[0].Cells["PALLET_NO"].Value.ToString()}를 삭제하시겠습니까?", "파레트 번호 삭제", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)) return;

                string strSql = $"{dbName}.dbo.ST_DANPLA_COLLECTOR_PALLET_DEL";

                db.Parameter("@PALLET_NO", grid_Pallet.SelectedRows[0].Cells["PALLET_NO"].Value.ToString());

                db.ExecuteNonSql(strSql);

                if (db.nState)
                {
                    MessageBox.Show("삭제되었습니다.", "파레트 번호 삭제", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    tbx_PalletNo.Text = string.Empty;

                    fn_Pallet_SEL();

                    return;
                }
            }
            catch(Exception ex)
            {

            }
        }

        private void fn_Pallet_SEL()
        {
            try
            {
                string strSql = $"{dbName}.dbo.ST_DANPLA_COLLECTOR_PALLET_SEL";

                db.Parameter("@PALLET_NO", tbx_PalletNo.Text);

                db.ExecuteSql(strSql);

                if (db.nState)
                {
                    this.grid_Pallet.DataSource = this.Pallet = db.result;

                    grid_Pallet.Columns["SEQ"].HeaderText = "순번";

                    grid_Pallet.Columns["PALLET_NO"].HeaderText = "파레트 번호";

                    grid_Pallet.Columns["IN_PSCD"].HeaderText = "생성자";

                    grid_Pallet.Columns["IN_DT"].HeaderText = "생성시간";
                }
            }
            catch(Exception ex)
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

                string strSql = $"{dbName}.dbo.ST_DANPLA_COLLECTOR_PALLET_INS";

                db.Parameter("@FAC_CD", "L");
                db.Parameter("@PS_CD", tbx_Empno.Text);
                db.Parameter("@PALLET_NO", true, 50);

                db.ExecuteNonSql(strSql, outputVals);

                if (db.nState)
                {
                    if (string.IsNullOrEmpty(db.sql_raise_error_msg))
                    {
                        tbx_PalletNo.Text = outputVals["@PALLET_NO"];

                        fn_Pallet_SEL();

                        return;
                    }
                }

                MessageBox.Show("단프라 번호가 생성되지 않았습니다.\n확인부탁드립니다.", "단프라 번호 추가 생성", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {

            }
        }



        private void fn_BarCode_Del()
        {
            try
            {
                if (grid_Danpla.SelectedRows[0] == null)
                {
                    MessageBox.Show("선택된 단프라 번호가 없습니다.\n단프라 번호를 선택하여주시기 바랍니다.", "바코드 삭제", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    return;
                }

                if (grid_BarCode.SelectedRows[0] == null)
                {
                    MessageBox.Show("선택된 바코드가 없습니다.\n바코드를 선택하여주시기 바랍니다.", "바코드 삭제", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    return;
                }

                foreach(DataRow dr in BarCode.Rows)
                {
                    if (dr["BARCODE"].ToString().Equals(grid_BarCode.SelectedRows[0].Cells["BARCODE"].Value.ToString()))
                    {
                        BarCode.Rows.Remove(dr);

                        grid_BarCode.DataSource = BarCode;

                        lb_RealQty.Text = $"스캔 총 수량 : {fn_AllScanQty()}";

                        return;
                    }
                }
                
            }
            catch(Exception ex)
            {

            }
        }

        private void fn_Barcode_Sel()
        {
            try
            {
                string strSql = $"{dbName}.dbo.ST_DANPLA_COLLECTOR_BARCODE_SEL";

                db.Parameter("@DAN_NO", tbx_DanplaNo.Text);

                if (!splashScreenManager1.IsSplashFormVisible) splashScreenManager1.ShowWaitForm();

                db.ExecuteSql(strSql);

                if (splashScreenManager1.IsSplashFormVisible) splashScreenManager1.CloseWaitForm();

                if (db.nState)
                {
                    BarCode = db.result;

                    grid_BarCode.DataSource = BarCode;

                    grid_BarCode.Columns["SEQ"].HeaderText = "순번";

                    grid_BarCode.Columns["DAN_NO"].HeaderText = "단프라번호";

                    grid_BarCode.Columns["BARCODE"].HeaderText = "바코드";

                    grid_BarCode.Columns["LOT_NO"].HeaderText = "LotNo";

                    grid_BarCode.Columns["GD_CD"].HeaderText = "품목코드";

                    grid_BarCode.Columns["MATE_NO"].HeaderText = "멸균번호";

                    grid_BarCode.Columns["EXPIR"].HeaderText = "유효기간";

                    grid_BarCode.Columns["YY_CNT"].Visible = false;

                    lb_RealQty.Text = $"스캔 총 수량 : {fn_AllScanQty()}";

                    tbx_BarCode.Select();
                }
            }
            catch(Exception ex)
            {

            }
        }

        private string fn_AllScanQty()
        {
            try
            {
                int cnt = 0;

                for (int i = 0; i < grid_BarCode.Rows.Count; i++) cnt++;

                return cnt.ToString();
            }
            catch(Exception ex)
            {
                return "0";
            }
        }

        
    }
}
