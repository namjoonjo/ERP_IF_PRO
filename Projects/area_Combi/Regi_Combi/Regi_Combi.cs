using COMBINATION;
using COMBINATION.Label;
using COMBINATION.MixingLabel;
using COMBINATION.Modules;
#if REGI_COMBI_DLL
using ERP_IF_PRO.Modules;
#endif
using DevExpress.Pdf.Native.BouncyCastle.Utilities;
using DevExpress.Utils.Text;
using DevExpress.XtraEditors;
using DevExpress.XtraPrinting.BarCode;
using DevExpress.XtraReports.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;

namespace COMBINATION
{
    public partial class Regi_Combi : Form
    {
        public Action<string> UpdateStatus { get; set; }

        private static string dbName = "ERP_2";

        CommonModule cm = new CommonModule();

        MSSQL db = new MSSQL(dbName);

        private string MC_CD = string.Empty; private string prQtyStr = "0";

        public string PwStr = string.Empty; private string prLotNo = string.Empty;

        private Dictionary<string, string> cboWorker = new Dictionary<string, string>();

        private Dictionary<string, string> cboKind = new Dictionary<string, string>();

        PrintInfo pf = null;

        private DataRow[] drKind; private DataRow[] drWorkers; private DataRow[] drMeasures;

        private Excel.Application application = null;
         
        private Excel.Workbook workBook = null;

        private Excel.Worksheet workSheet = null;

        BarCodeControl barCodeControl = null;
        public Regi_Combi()
        {
            InitializeComponent();

            initControl();
        }

        private void initControl()
        {
            try
            {
                this.StartPosition = FormStartPosition.CenterScreen;

                this.FormBorderStyle = FormBorderStyle.FixedSingle;

                GetData();

                SetGridRowHeader(grid_State, 35, true);

                SetGridRowHeader(grid_State2, 35, false);

                fn_ComboInit();

                initControlValues();


                cbo입고기간.SelectedValueChanged += Cbo입고기간_SelectedValueChanged;

                grid_State.CellClick += (s, e) => { fn_DetailSel(false); };

                grid_State2.SelectionChanged += Grid_State2_SelectionChanged;

                this.FormClosing += Regi_Combi_FormClosing;

                this.txtLotNo.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) fn_txtLotNoKeypress(); };

                this.txt저울측정값.KeyDown += Txt저울측정값_KeyDown;

                this.txt지시수량.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) fn_txt지시수량KeyPress(); };

                this.cmd조건검색.Click += (s, e) => { fn_DataRefresh(false); };

                this.dtp입고일1.ValueChanged += (s, e) => { fn_DataRefresh(false); };

                this.dtp입고일2.ValueChanged += (s, e) => { fn_DataRefresh(false); };

                this.cmd저장2.Click += (s, e) => { fn_Save(); };

                this.cbo입고기간.SelectedIndexChanged += (s, e) => { fn_DataRefresh(false); };

                this.cbo_kind.SelectedIndexChanged += (s, e) => { fn_SetWorkers(); fn_DataRefresh(false); };

                this.cbo작업자.SelectedIndexChanged += (s, e) => { if (!string.IsNullOrEmpty(cbo작업자.Text)) txtLotNo.Select(); };

                this.chkFinYn.CheckedChanged += (s, e) => { fn_DataRefresh(false); };

                this.btn_Connect.Click += (s, e) => { fn_GetLink(); };

                this.btn_Print.Click += (s, e) => {

                    //if (fn_InputStock())
                    //{
                    //    //if (cbo_kind.Text.Equals("약품"))
                    //    //{
                    //    //    if (cboPrinter.SelectedIndex == 0) fn_cmdReportClickforEpsonQRBarcode(); else fn_cmdReportClickforZebraQRBarcode();
                    //    //}
                    //    //else
                    //    //{
                    //    //    if (cboPrinter.SelectedIndex == 0) fn_cmdReportClickforEpson2DBarcode(); else fn_cmdReportClickforZebra2DBarcode();
                    //    //}

                    //    if (cboPrinter.SelectedIndex == 0) fn_cmdReportClickforEpson2DBarcode(); else fn_cmdReportClickforZebra2DBarcode();
                    //}

                    if (cbo_kind.Text.Equals("약품"))
                    {
                        if (cboPrinter.SelectedIndex == 0) fn_cmdReportClickforEpsonQRBarcode(); else fn_cmdReportClickforZebraQRBarcode();
                    }
                    else
                    {
                        if (cboPrinter.SelectedIndex == 0) fn_cmdReportClickforEpson2DBarcode(); else fn_cmdReportClickforZebra2DBarcode();
                    }

                    //if (cboPrinter.SelectedIndex == 0) fn_cmdReportClickforEpson2DBarcode(); else fn_cmdReportClickforZebra2DBarcode();
                };

                this.chk_StockQty.CheckStateChanged += Chk_StockQty_CheckStateChanged;

                this.chk_byhand.CheckStateChanged += Chk_byhand_CheckStateChanged;

                this.grid_State2.DefaultCellStyle.Font = new Font("굴림", 15, FontStyle.Bold);

                txtLastData.ReadOnly = true;

                this.chkFinYn.Checked = false; 

                fn_DataRefresh(false);

                strQty.Value = 1;

                if(pf == null) pf = new PrintInfo();

                //this.strQty.Enabled = this.btn_Print.Enabled = false;

            }
            catch (Exception ex)
            {

            }
        }

        private void initControlValues()
        {
            try
            {
                cmbSerialPorts.Text = string.Empty;

                this.txt누적투입량.Text = "0";

                this.txt사업장.Text = "(주)인터로조";

                this.txt생산지시.ReadOnly = true;

                this.txt사업장.ReadOnly = true;

                this.txt품목코드.ReadOnly = true;

                this.txt품목명.ReadOnly = true;

                this.txt규격.ReadOnly = true;

                this.txt원재료코드.ReadOnly = true;

                this.txt원재료명.ReadOnly = true;

                this.txt배합Lot.ReadOnly = true;

                this.txt누적투입량.ReadOnly = true;

                this.txt투입량.ReadOnly = true;

                this.lb_SerialStatus.ForeColor = Color.Yellow;

                this.lb_SerialStatus.BackColor = Color.Red;

                this.lb_SerialStatus.Text = "UnLinked";
            }
            catch (Exception ex)
            {

            }
        }

        private bool fn_InputStock()
        {
            try
            {
                DataGridViewRow selectedRow = null;

                if (grid_State.Rows.Count == 0)
                {
                    DataGridViewRow dg = new DataGridViewRow();

                    dg.CreateCells(grid_State);

                    selectedRow = dg;

                    return false;
                }
                else selectedRow = grid_State.SelectedRows[0];

                if (!grid_State.SelectedRows[0].Cells["FIN_YN"].Value.ToString().Equals("완료"))
                {
                    MessageBox.Show("선택된 배합지시가 완료가 아닙니다.\n확인부탁드립니다.", "인쇄 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    return false;
                }

                if (cbo_kind.Text.Equals("용수")) return true;

                string strSql = $"{dbName}.dbo.ST_COMBI_STOCK_SEL_UPD_EMAX";

                db.Parameter("@GD_CD", txt품목코드.Text);
                db.Parameter("@DIV_CNT", Convert.ToInt32(strQty.Value));
                db.Parameter("@JOB_NO", txt생산지시.Text);
                db.Parameter("@JOB_QTY", selectedRow.Cells["PR_JOB_QTY"].Value.ToString());
                db.Parameter("@PR_DT", Convert.ToDateTime(selectedRow.Cells["PR_DT"].Value).ToString("yyyy-MM-dd"));
                db.Parameter("@PR_QTY", selectedRow.Cells["PR_QTY"].Value.ToString());
                db.Parameter("@FAC_CD", selectedRow.Cells["PR_FAC_CD"].Value.ToString());
                db.Parameter("@Y_day", Convert.ToDateTime(selectedRow.Cells["PR_Y_DAY"].Value.ToString()).ToString("yyyy-MM-dd"));
                db.Parameter("@COMBI_PR_NO", selectedRow.Cells["PR_NO"].Value.ToString());
                db.Parameter("@COMBI_LOT_NO", selectedRow.Cells["LOT_NO"].Value.ToString());
                db.Parameter("@XML_STOCK", cm.DataTblToXML(fn_makeStockTbl()));

                db.ExecuteNonSql(strSql);

                if (db.nState)
                {
                    if (string.IsNullOrEmpty(db.sql_raise_error_msg)) return true;

                    else return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        private DataTable fn_makeStockTbl()
        {
            try
            {
                DataTable rsDt = new DataTable();

                rsDt.Columns.Add("MA_CD");
                rsDt.Columns.Add("Y_HOUR");
                rsDt.Columns.Add("LOT_SEQ");
                rsDt.Columns.Add("DIV_CNT");

                for (int i = 1; i <= Convert.ToInt32(strQty.Value); i++)
                {
                    DataRow drr = rsDt.NewRow();

                    drr["MA_CD"] = (grid_State.SelectedRows.Count == 0 || grid_State.SelectedRows[0].Cells["PLAN_MCCD"].Value == null) ? string.Empty : grid_State.SelectedRows[0].Cells["PLAN_MCCD"].Value.ToString();

                    drr["Y_HOUR"] = cbo유효기간.Text;

                    drr["LOT_SEQ"] = i;

                    drr["DIV_CNT"] = Convert.ToInt32(strQty.Value);

                    rsDt.Rows.Add(drr);
                }

                return rsDt;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private void fn_Save()
        {
            try
            {
                DialogResult dir = MessageBox.Show($"저장하시겠습니까?", "저장", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (dir == DialogResult.No) return;

                prQtyStr = txt누적투입량.Text.Replace(",", string.Empty);

                foreach (DataGridViewRow row in grid_State2.Rows)
                {
                    if (string.IsNullOrEmpty(row.Cells["RAWLOT"].Value.ToString()))
                    {
                        MessageBox.Show($"{row.Cells["GD_NM"].Value.ToString()}의 원재료LOT가 입력되지 않았습니다.", "저장오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                        return;
                    }

                    if (string.IsNullOrEmpty(row.Cells["REALTUIPQTY"].Value.ToString()))
                    {
                        MessageBox.Show($"{row.Cells["GD_NM"].Value.ToString()}의 저울측정치값이 입력되지 않았습니다.", "저장오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                        return;
                    }
                }

                DataTable dt = new DataTable();

                dt.Columns.Add("PR_SEQ");
                dt.Columns.Add("GD_CD");
                dt.Columns.Add("SO_QTY");
                dt.Columns.Add("TUIP_QTY");
                dt.Columns.Add("LOT_NO");
                dt.Columns.Add("REMK");


                for (int i = 0; i < grid_State2.Rows.Count; i++)
                {
                    DataRow dr = dt.NewRow();

                    DataGridViewRow row = grid_State2.Rows[i];

                    dr["PR_SEQ"] = row.Cells["SEQ"].Value.ToString();

                    dr["GD_CD"] = row.Cells["JA_CD"].Value.ToString();

                    dr["SO_QTY"] = !string.IsNullOrEmpty(row.Cells["SO_QTY"].ToString()) ? row.Cells["SO_QTY"].Value.ToString() : "0";

                    dr["LOT_NO"] = row.Cells["RAWLOT"].Value.ToString();

                    // 합성/약품 이고, B109가 아닐 경우 투입량은 0, 비고에 실투입량을 기입함.
                    if (!fn_isRaw(row.Cells["JA_CD"].Value.ToString()) && !row.Cells["JA_CD"].Value.ToString().Equals("B109"))
                    {
                        dr["TUIP_QTY"] = "0";

                        dr["REMK"] = Convert.ToDouble(row.Cells["REALTUIPQTY"].Value).ToString();
                    }
                    // 원재료거나 B109일경우에만 투입량에 실투입량이 들어감.
                    else
                    {
                        dr["TUIP_QTY"] = !(row.Cells["REALTUIPQTY"].Value == null || string.IsNullOrEmpty(row.Cells["REALTUIPQTY"].Value.ToString())) ? Convert.ToDouble(row.Cells["REALTUIPQTY"].Value).ToString() : "0";

                        dr["REMK"] = string.Empty;
                    }

                    dt.Rows.Add(dr);
                }

                string y_day = string.Empty;

                if (txt품목코드.Text.Substring(0, 2).Equals("BW"))
                {
                    y_day = $"{dtp생산일자.Value.AddDays(Convert.ToDouble(cbo유효기간.Text) / 24).ToString("MM-dd")} {cbotime.Text.Substring(0, 2)}시";
                }
                else
                {
                    y_day = $"{dtp생산일자.Value.AddDays(Convert.ToDouble(cbo유효기간.Text) / 24).ToString("yyyy-MM-dd")}";
                }


                string strSql = dbName.Equals("ERP_2") ? $"{dbName}.dbo.ST_COMBI_PR_SAVE_EMAX" : $"{dbName}.dbo.ST_COMBI_PR_SAVE_EMAX_TEST";

                db.Parameter("@PR_DT", dtp생산일자.Value.ToString("yyyy-MM-dd"));
                db.Parameter("@PS_CD", cboWorker[cbo작업자.Text]);
                db.Parameter("@JOB_NO", txt생산지시.Text);
                db.Parameter("@GONG_CD", grid_State.SelectedRows[0].Cells["GONG_CD"].Value.ToString());
                db.Parameter("@MC_CD", MC_CD);
                db.Parameter("@JOB_QTY", txt지시수량.Text.Replace(",",string.Empty));
                db.Parameter("@PR_QTY", prQtyStr);
                db.Parameter("@REMK", strQty.Value.ToString());
                db.Parameter("@GD_CD", txt품목코드.Text);
                db.Parameter("@NEW_GDCD", txt품목코드.Text);
                db.Parameter("@PR_JOBNO", txt생산지시.Text);
                db.Parameter("@FAC_CD", cbo공장.SelectedIndex == 0 || cbo공장.SelectedIndex == 1 ? $"0{cbo공장.SelectedIndex + 1}" : "04");
                db.Parameter("@Y_day", y_day);
                db.Parameter("@TXTGDNM", txt품목명.Text);
                db.Parameter("@XML", cm.DataTblToXML(dt));
                db.Parameter("@DIV_CNT", Convert.ToInt32(strQty.Value));
                db.Parameter("@XML_STOCK", cm.DataTblToXML(fn_makeStockTbl()));
                db.OutputParameter("@COMBI_LOT_NO");
               
                db.ExecuteNonSql(strSql);

                if (db.nState)
                {
                    prLotNo = db.OutputParameterMapper["@COMBI_LOT_NO"];

                    MessageBox.Show(string.IsNullOrEmpty(db.sql_raise_error_msg) ? $"{db.OutputParameterMapper["@COMBI_LOT_NO"]}\n실적이 등록되었습니다.\n지시번호 : {txt생산지시.Text}" : db.sql_raise_error_msg, "저장", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    fn_DataRefresh(true);

                    return;
                }

                MessageBox.Show($"실적 등록에러가 발생하였습니다.\n에러코드 : {db.sql_raise_error_msg}", "저장", MessageBoxButtons.OK, MessageBoxIcon.Information);

                return;
            }
            catch (Exception ex)
            {

            }
        }

        private DataTable Fn_makeStockTbl()
        {
            try
            {
                DataTable rsDt = new DataTable();

                rsDt.Columns.Add("MA_CD");
                rsDt.Columns.Add("Y_HOUR");
                rsDt.Columns.Add("LOT_SEQ");
                rsDt.Columns.Add("DIV_CNT");

                for (int i = 1; i <= Convert.ToInt32(strQty.Value); i++)
                {
                    DataRow drr = rsDt.NewRow();

                    drr["MA_CD"] = (grid_State.SelectedRows.Count == 0 || grid_State.SelectedRows[0].Cells["PLAN_MCCD"].Value == null) ? string.Empty : grid_State.SelectedRows[0].Cells["PLAN_MCCD"].Value.ToString();

                    drr["Y_HOUR"] = cbo유효기간.Text;

                    drr["LOT_SEQ"] = i;

                    drr["DIV_CNT"] = Convert.ToInt32(strQty.Value);

                    rsDt.Rows.Add(drr);
                }

                return rsDt;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private void fn_cmdReportClickforZebra2DBarcode()
        {
            try
            {

                if (string.IsNullOrEmpty(strQty.Text) || strQty.Text.Equals("0"))
                {
                    MessageBox.Show("인쇄 매수 수량이 없거나 0입니다.", "인쇄 오류");

                    return;
                }

                if (!grid_State.SelectedRows[0].Cells["FIN_YN"].Value.ToString().Equals("완료"))
                {
                    MessageBox.Show("선택된 배합지시가 완료가 아닙니다.\n확인부탁드립니다.", "인쇄 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    return;
                }

                DateTime sDate = dtp생산일자.Value.AddDays(Convert.ToInt32(cbo유효기간.Text) / 24);

                if (MessageBox.Show($"Report를 출력 하시겠습니까?\n\n약품번호 : {txt배합Lot.Text}\n\n배합일시 : {dtp생산일자.Value}\n\n유통 기간 : " +
                                    $"{sDate.ToString("MM/dd")} / {cbotime.Text}\n\n품  명 : {txt품목명.Text} / {txt규격.Text}", "출력", MessageBoxButtons.YesNo) == DialogResult.No) return;

                if (pf == null) pf = new PrintInfo();

                pf.setLotNo(txt배합Lot.Text);

                pf.setproDate($"{dtp생산일자.Value.ToString("yyyy-MM-dd")} {cbotime.Text}");

                string strYdt = dtp생산일자.Value.AddDays(Convert.ToDouble(cbo유효기간.Text) / 24).ToString("MM/dd");

                pf.setvaliDate($"{strYdt} {cbotime.Text} 까지");

                pf.setRGB(Convert.ToInt32(grid_State.SelectedRows[0].Cells["COLOR_R"].Value), Convert.ToInt32(grid_State.SelectedRows[0].Cells["COLOR_G"].Value), Convert.ToInt32(grid_State.SelectedRows[0].Cells["COLOR_B"].Value));

                for (int i = 1; i <= Convert.ToInt32(strQty.Value); i++)
                {
                    pf.setGD_NM($"{txt품목명.Text} {txt규격.Text} {i}/{strQty.Text}");

                    MixingLabelforZebra xx = new MixingLabelforZebra(pf);

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

                if (string.IsNullOrEmpty(strQty.Text) || strQty.Text.Equals("0"))
                {
                    MessageBox.Show("인쇄 매수 수량이 없거나 0입니다.", "인쇄 오류");

                    return;
                }

                if (!grid_State.SelectedRows[0].Cells["FIN_YN"].Value.ToString().Equals("완료"))
                {
                    MessageBox.Show("선택된 배합지시가 완료가 아닙니다.\n확인부탁드립니다.", "인쇄 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    return;
                }

                DateTime sDate = dtp생산일자.Value.AddDays(Convert.ToInt32(cbo유효기간.Text) / 24);

                if (MessageBox.Show($"Report를 출력 하시겠습니까?\n\n약품번호 : {txt배합Lot.Text}\n\n배합일시 : {dtp생산일자.Value}\n\n유통 기간 : " +
                                    $"{sDate.ToString("MM/dd")} / {cbotime.Text}\n\n품  명 : {txt품목명.Text} / {txt규격.Text}", "출력", MessageBoxButtons.YesNo) == DialogResult.No) return;

                if (pf == null) pf = new PrintInfo();

                

                pf.setproDate($"{dtp생산일자.Value.ToString("yyyy-MM-dd")} {cbotime.Text}");

                string strYdt = dtp생산일자.Value.AddDays(Convert.ToDouble(cbo유효기간.Text) / 24).ToString("MM/dd");

                pf.setvaliDate($"{strYdt} {cbotime.Text} 까지");

                pf.setRGB(Convert.ToInt32(grid_State.SelectedRows[0].Cells["COLOR_R"].Value), Convert.ToInt32(grid_State.SelectedRows[0].Cells["COLOR_G"].Value), Convert.ToInt32(grid_State.SelectedRows[0].Cells["COLOR_B"].Value));

                for (int i = 1; i <= Convert.ToInt32(strQty.Value); i++)
                {
                    //pf.setGD_NM($"{txt품목명.Text} {txt규격.Text} {i}/{strQty.Text}");

                    pf.setLotNo($"{txt배합Lot.Text} {i}/{strQty.Text}");

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

        private void fn_cmdReportClickforEpson2DBarcode()
        {
            try
            {

                if (string.IsNullOrEmpty(strQty.Text) || strQty.Text.Equals("0"))
                {
                    MessageBox.Show("인쇄 매수 수량이 없거나 0입니다.", "인쇄 오류");

                    return;
                }

                if (!grid_State.SelectedRows[0].Cells["FIN_YN"].Value.ToString().Equals("완료"))
                {
                    MessageBox.Show("선택된 배합지시가 완료가 아닙니다.\n확인부탁드립니다.", "인쇄 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    return;
                }

                DateTime sDate = dtp생산일자.Value.AddDays(Convert.ToInt32(cbo유효기간.Text) / 24);

                if (MessageBox.Show($"Report를 출력 하시겠습니까?\n\n약품번호 : {txt배합Lot.Text}\n\n배합일시 : {dtp생산일자.Value}\n\n유통 기간 : " +
                                    $"{sDate.ToString("MM/dd")} / {cbotime.Text}\n\n품  명 : {txt품목명.Text} / {txt규격.Text}", "출력", MessageBoxButtons.YesNo) == DialogResult.No) return;


                if (application == null)
                {
                    application = new Excel.Application();

                }

                application.Visible = false;

                string FileName = $"{Application.StartupPath}\\MixingLabel\\MixingLabel.xlsx";

                if (!File.Exists(FileName)) return;

                workBook = application.Workbooks.Open(FileName);

                workSheet = workBook.Worksheets["sheet1"];


                int cl_r = Convert.ToInt32(grid_State.SelectedRows[0].Cells["COLOR_R"].Value);

                int cl_g = Convert.ToInt32(grid_State.SelectedRows[0].Cells["COLOR_G"].Value);

                int cl_b = Convert.ToInt32(grid_State.SelectedRows[0].Cells["COLOR_B"].Value);

                Excel.Range colorRange2 = null;
                Excel.Range colorRange4 = null;
                Excel.Range colorRange = null;
                Excel.Range colorRange3 = null;

                // x 가로 컬러
                for (int i = 1; i <= 5; i++)
                {
                    colorRange2 = workSheet.Cells[1, i + 6];

                    colorRange2.Interior.Color = Color.FromArgb(cl_r, cl_g, cl_b);
                }

                for (int i = 1; i <= 7; i++)
                {
                    colorRange4 = workSheet.Cells[13, i + 5];

                    colorRange4.Interior.Color = Color.FromArgb(cl_r, cl_g, cl_b);
                }

                // y 세로 컬러
                for (int i = 1; i <= 13; i++)
                {
                    colorRange = workSheet.Cells[i, 6];

                    colorRange.Interior.Color = Color.FromArgb(cl_r, cl_g, cl_b);
                }

                for (int i = 1; i <= 12; i++)
                {
                    colorRange3 = workSheet.Cells[i, 12];

                    colorRange3.Interior.Color = Color.FromArgb(cl_r, cl_g, cl_b);
                }


                workSheet.Cells[3, 8].Value = txt배합Lot.Text;

                workSheet.Cells[5, 8].Value = $"{dtp생산일자.Value.ToString("yyyy-MM-dd")} {cbotime.Text}";

                string strYdt = dtp생산일자.Value.AddDays(Convert.ToInt32(cbo유효기간.Text) / 24).ToString("MM/dd");

                workSheet.Cells[7, 8].Value = $"{strYdt} {cbotime.Text} 까지";

                for (int i = 1; i <= int.Parse(strQty.Text); i++)
                {
                    workSheet.Cells[9, 8].Value = $"{txt품목명.Text} {txt규격.Text} {i}/{strQty.Text}";

                    workSheet.PrintOutEx(Type.Missing, Type.Missing, 1, false, false, false, false);

                    //workSheet.PrintPreview(true);
                }

                workBook.Close(false);

                application.Quit();

                System.Runtime.InteropServices.Marshal.ReleaseComObject(workSheet.Cells[3, 8]);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(workSheet.Cells[5, 8]);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(workSheet.Cells[7, 8]);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(workSheet.Cells[9, 8]);

                System.Runtime.InteropServices.Marshal.ReleaseComObject(colorRange);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(colorRange2);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(colorRange3);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(colorRange4);
            }
            catch (Exception ex)
            {

            }
        }

        private void fn_cmdReportClickforEpsonQRBarcode()
        {
            try
            {

                if (string.IsNullOrEmpty(strQty.Text) || strQty.Text.Equals("0"))
                {
                    MessageBox.Show("인쇄 매수 수량이 없거나 0입니다.", "인쇄 오류");

                    return;
                }

                if (!grid_State.SelectedRows[0].Cells["FIN_YN"].Value.ToString().Equals("완료"))
                {
                    MessageBox.Show("선택된 배합지시가 완료가 아닙니다.\n확인부탁드립니다.", "인쇄 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    return;
                }

                DateTime sDate = dtp생산일자.Value.AddDays(Convert.ToInt32(cbo유효기간.Text) / 24);

                if (MessageBox.Show($"Report를 출력 하시겠습니까?\n\n약품번호 : {txt배합Lot.Text}\n\n배합일시 : {dtp생산일자.Value}\n\n유통 기간 : " +
                                    $"{sDate.ToString("MM/dd")} / {cbotime.Text}\n\n품  명 : {txt품목명.Text} / {txt규격.Text}", "출력", MessageBoxButtons.YesNo) == DialogResult.No) return;


                if (application == null)
                {
                    application = new Excel.Application();

                }

                application.Visible = false;

                string FileName = $"{Application.StartupPath}\\MixingLabel\\CODEX_MixingLabel.xlsx";

                if (!File.Exists(FileName)) return;

                workBook = application.Workbooks.Open(FileName);

                workSheet = workBook.Worksheets["sheet1"];

                barCodeControl = new BarCodeControl();
                barCodeControl.Visible = false;
                barCodeControl.ShowText = false;
                barCodeControl.Size = new Size(90, 90);
                barCodeControl.AutoModule = true;

                QRCodeGenerator symb = new QRCodeGenerator();
                symb.CompactionMode = QRCodeCompactionMode.Byte;
                symb.ErrorCorrectionLevel = QRCodeErrorCorrectionLevel.H;
                symb.Version = QRCodeVersion.AutoVersion;
                barCodeControl.Symbology = symb;


                int cl_r = Convert.ToInt32(grid_State.SelectedRows[0].Cells["COLOR_R"].Value);

                int cl_g = Convert.ToInt32(grid_State.SelectedRows[0].Cells["COLOR_G"].Value);

                int cl_b = Convert.ToInt32(grid_State.SelectedRows[0].Cells["COLOR_B"].Value);

                Excel.Range colorRange2 = null;
                Excel.Range colorRange4 = null;
                Excel.Range colorRange = null;
                Excel.Range colorRange3 = null;
                Excel.Range QRCoderange = null;

                // x 가로 컬러
                for (int i = 1; i <= 5; i++)
                {
                    colorRange2 = workSheet.Cells[1, i + 6];

                    colorRange2.Interior.Color = System.Drawing.Color.FromArgb(cl_r, cl_g, cl_b);
                }

                for (int i = 1; i <= 7; i++)
                {
                    colorRange4 = workSheet.Cells[13, i + 5];

                    colorRange4.Interior.Color = System.Drawing.Color.FromArgb(cl_r, cl_g, cl_b);
                }

                // y 세로 컬러
                for (int i = 1; i <= 13; i++)
                {
                    colorRange = workSheet.Cells[i, 6];

                    colorRange.Interior.Color = System.Drawing.Color.FromArgb(cl_r, cl_g, cl_b);
                }

                for (int i = 1; i <= 12; i++)
                {
                    colorRange3 = workSheet.Cells[i, 12];

                    colorRange3.Interior.Color = System.Drawing.Color.FromArgb(cl_r, cl_g, cl_b);
                }


                //workSheet.Cells[3, 8].Value = txt배합Lot.Text;

                //workSheet.Cells[5, 8].Value = $"{dtp생산일자.Value.ToString("yyyy-MM-dd")} {cbotime.Text}";

                string strYdt = dtp생산일자.Value.AddDays(Convert.ToInt32(cbo유효기간.Text) / 24).ToString("MM/dd");

                workSheet.Cells[5, 8].Value = $"{strYdt} {cbotime.Text} 까지";

                barCodeControl.Name = txt배합Lot.Text;
                barCodeControl.Text = txt배합Lot.Text;

                System.Drawing.Image image = null;

                while (image == null)
                {
                    image = barCodeControl.ExportToImage();
                    Console.WriteLine(image.ToString());
                }

                Clipboard.SetDataObject(image, true);

                QRCoderange = (Excel.Range)workSheet.Cells[7, 9];

                Thread.Sleep(50);  // 클립보드에 bitmap형식으로 담은 객체를 넣는 시간이 걸림

                workSheet.Paste(QRCoderange, (Bitmap)image);

                for (int i = 1; i <= int.Parse(strQty.Text); i++)
                {
                    //workSheet.Cells[9, 8].Value = $"{txt품목명.Text} {txt규격.Text} {i}/{strQty.Text}";

                    workSheet.Cells[3, 8].Value = $"{txt배합Lot.Text} {i}/{strQty.Text}";

                    workSheet.PrintOutEx(Type.Missing, Type.Missing, 1, false, false, false, false);

                    //workSheet.PrintPreview(true);
                }

                workBook.Close(false);

                application.Quit();

                System.Runtime.InteropServices.Marshal.ReleaseComObject(workSheet.Cells[3, 8]);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(workSheet.Cells[5, 8]);
                //System.Runtime.InteropServices.Marshal.ReleaseComObject(workSheet.Cells[7, 8]);
                //System.Runtime.InteropServices.Marshal.ReleaseComObject(workSheet.Cells[9, 8]);

                System.Runtime.InteropServices.Marshal.ReleaseComObject(colorRange);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(colorRange2);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(colorRange3);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(colorRange4);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(QRCoderange);
            }
            catch (Exception ex)
            {

            }
        }

        private void SerialPortLoad()
        {
            try
            {
                if (!serialPort1.IsOpen)
                {

                    switch (cboMC.Text.Trim())
                    {
                        case "매틀러토레도":

                            serialPort1.PortName = cmbSerialPorts.Text;
                            serialPort1.BaudRate = 9600;
                            serialPort1.DataBits = 8;
                            serialPort1.StopBits = StopBits.One;
                            serialPort1.Parity = Parity.None;
                            serialPort1.Handshake = Handshake.XOnXOff;

                            break;

                        case "AND저울":

                            serialPort1.PortName = cmbSerialPorts.Text;
                            serialPort1.BaudRate = 38400;
                            serialPort1.StopBits = StopBits.One;
                            serialPort1.Parity = Parity.Even;

                            break;

                        case "WANG저울":

                            serialPort1.PortName = cmbSerialPorts.Text;
                            serialPort1.BaudRate = 9600;
                            serialPort1.DataBits = 8;
                            serialPort1.StopBits = StopBits.One;
                            serialPort1.Parity = Parity.None;

                            break;

                    }

                    serialPort1.DataReceived += new SerialDataReceivedEventHandler(serialPort1_DataReceived);

                    serialPort1.Open();

                    cmbSerialPorts.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                cm.writeLog($"Regi_Combi_A SerialPortLoad Error : {ex.ToString()}");
            }
        }

        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)  //수신 이벤트가 발생하면 이 부분이 실행된다.
        {
            try
            {
                this.Invoke(new EventHandler(MySerialReceived));
            }
            catch (Exception ex)
            {
                cm.writeLog($"Regi_Combi_A serialPort1_DataReceived Error : {ex.ToString()}");
            }
        }

        private void MySerialReceived(object s, EventArgs e)
        {

            try
            {
                if (!fn_chkInputLotNo())
                {
                    MessageBox.Show("원재료LotNo를 입력해 주십시오.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    txt저울측정값.Text = string.Empty;

                    txtLotNo.Focus();

                    return;
                }

                // int ReceiveData = serialPort1.ReadByte();

                // string tmpStr = string.Format("{0:X2}", ReceiveData).Trim();

                //string tmpStr = serialPort1.ReadExisting().Trim();

                string tmpStr = serialPort1.ReadLine().ToString().Replace(" ", string.Empty);

                foreach (DataRow measuerDr in drMeasures)
                {
                    tmpStr = tmpStr.Contains(measuerDr["COM1"].ToString()) ? tmpStr.Replace(measuerDr["COM1"].ToString(), string.Empty) : tmpStr;
                }

                txt저울측정값.Text = tmpStr;

                fn_Processing();

            }
            catch (Exception ex)
            {
                cm.writeLog($"Regi_Combi_A MySerialReceived Error : {ex.ToString()}");
            }
        }

        private void fn_누적투입량()
        {
            try
            {
                double result = 0;

                foreach (DataGridViewRow row in grid_State2.Rows) result += (row.Cells[6].Value == null || string.IsNullOrEmpty(row.Cells[6].Value.ToString())) ? 0 : double.Parse(row.Cells[6].Value.ToString());

                txt누적투입량.Text = string.Format("{0:#,##0.00}", result);
            }
            catch (Exception ex)
            {

            }
        }

        private void fn_Processing()
        {
            try
            {
                txtLastData.Text = Convert.ToDouble(txt저울측정값.Text).ToString();

                if (double.Parse(grid_State2.SelectedRows[0].Cells["TUIP_QTY"].Value.ToString()) - 0.05 > double.Parse(txt저울측정값.Text) ||
                   double.Parse(grid_State2.SelectedRows[0].Cells["TUIP_QTY"].Value.ToString()) + 0.05 < double.Parse(txt저울측정값.Text))
                {
                    lbl판정.ForeColor = Color.Orange;
                    lbl판정.BackColor = Color.Red;

                    lbl판정.Text = string.Format("{0:#,##0.00}", double.Parse(txt저울측정값.Text) - double.Parse(grid_State2.SelectedRows[0].Cells["TUIP_QTY"].Value.ToString()));


                    txt저울측정값.Text = string.Empty;
                    txt저울측정값.Focus();
                }
                else
                {
                    grid_State2.SelectedRows[0].Cells["REALTUIPQTY"].Value = Convert.ToDouble(txt저울측정값.Text).ToString("0.00");

                    grid_State2.SelectedRows[0].Cells["RAWLOT"].Value = txtLotNo.Text;

                    fn_누적투입량();

                    if (grid_State2.Rows.Count > 0)
                    {
                        int selidx = grid_State2.SelectedRows[0].Index;

                        if (selidx + 1 != grid_State2.Rows.Count)
                        {
                            foreach (DataGridViewRow row in grid_State2.Rows)
                            {
                                row.Selected = false;
                            }

                            grid_State2.Rows[selidx + 1].Selected = true;
                        }
                    }

                    txt저울측정값.Text = string.Empty;

                    lbl판정.BackColor = Color.Blue;
                    lbl판정.ForeColor = Color.LightYellow;
                    lbl판정.Text = "Passed!";

                    txtLotNo.Select();
                }

                fn_누적투입량();
            }
            catch (Exception ex)
            {
                cm.writeLog($"REGI_COMBI_A fn_Processing Error : {ex.Message}");
            }
        }

        private bool fn_chkInputLotNo()
        {
            try
            {
                return !(string.IsNullOrEmpty(txt재고량.Text) || string.IsNullOrEmpty(txtLotNo.Text));
            }
            catch (Exception ex)
            {
                return true;
            }
        }

        private void fn_GetLink()
        {
            try
            {
                if (string.IsNullOrEmpty(cmbSerialPorts.Text) || cmbSerialPorts.Items.Count == 0)
                {
                    MessageBox.Show("시리얼 통신 연결이 불가합니다.\n확인부탁드립니다.", "시리얼통신 연결", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    return;
                }

                SerialPortLoad();

                this.lb_SerialStatus.ForeColor = Color.Blue;

                this.lb_SerialStatus.BackColor = Color.Teal;

                this.lb_SerialStatus.Text = "Linked";

                this.txtLotNo.Select();
            }
            catch (Exception ex)
            {
                cm.writeLog($"Regi_Combi_A fn_GetLink Error : {ex.ToString()}");
            }
        }

        private void fn_SetWorkers()
        {
            try
            {
                if (this.drWorkers == null || this.drWorkers.Length == 0) return;

                cbo작업자.Items.Clear();

                foreach (DataRow row in this.drWorkers)
                {
                    string[] spliters = row["COMBO_STR"].ToString().Split('/');

                    if (!spliters[2].Equals(cboKind[cbo_kind.Text])) continue;

                    cbo작업자.Items.Add(spliters[0]);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"fn_SetWorkers : {ex.Message}");
            }
        }


        private void fn_ComboInit()
        {
            try
            {
                //string strSql = $"{dbName}.dbo.ST_COMBOBOX_DATA_SEL";

                string strSql = "ERP_2.dbo.ST_COMBOBOX_DATA_SEL";

                db.Parameter("@F_NAME", this.Name);

                db.ExecuteSql(strSql);

                if (db.nState)
                {
                    if (db.result.Rows.Count > 0)
                    {
                        cm.ComboBoxBinding(db.result, cbo입고기간, "COMBO_1");

                        cm.ComboBoxBinding(db.result, cbo출고창고, "COMBO_2");

                        cm.ComboBoxBinding(db.result, cbo입고창고, "COMBO_3");

                        cm.ComboBoxBinding(db.result, cbo공장, "COMBO_4");

                        cm.ComboBoxBinding(db.result, cbotime, "COMBO_15");

                        cm.ComboBoxBinding(db.result, cboMC, "COMBO_8");

                        cm.ComboBoxBinding(db.result, cbo유효기간, "COMBO_5");

                        cm.ComboBoxBinding(db.result, cboPrinter, "COMBO_16");

                        DataRow[] PlanProcDr = db.result.Select($"KIND = 'COMBO_13'");

                        this.drWorkers = db.result.Select($"KIND IN ('COMBO_7','COMBO_10','COMBO_11','COMBO_12')");
                         
                        foreach (DataRow ddr in drWorkers)
                        {
                            string[] spliters = ddr["COMBO_STR"].ToString().Split('/');

                            if (!cboWorker.ContainsKey(spliters[0])) cboWorker.Add(spliters[0], spliters[1]);
                        }

                        drKind = db.result.Select($"KIND = 'COMBO_9'");

                        foreach (DataRow ddr in drKind)
                        {
                            string[] spliters = ddr["COMBO_STR"].ToString().Split('/');

                            cboKind.Add(spliters[0], spliters[1]);

                            cbo_kind.Items.Add(spliters[0]);
                        }

                        cbo입고기간.Text = "기간지정";

                        cbo_kind.SelectedIndex = cbo_kind.Items.Count > 0 ? 0 : cbo_kind.SelectedIndex;

                        cbo출고창고.SelectedIndex = cbo출고창고.Items.Count > 0 ? 0 : cbo출고창고.SelectedIndex;

                        cbo입고창고.SelectedIndex = cbo입고창고.Items.Count > 0 ? 0 : cbo입고창고.SelectedIndex;

                        cbo공장.SelectedIndex = cbo공장.Items.Count > 0 ? 1 : cbo공장.SelectedIndex;

                        cbotime.SelectedIndex = cbotime.Items.Count > 0 ? 1 : cbotime.SelectedIndex;

                        cboMC.SelectedIndex = cboMC.Items.Count > 0 ? 0 : cboMC.SelectedIndex;

                        cboPrinter.SelectedIndex = cboPrinter.Items.Count > 0 ? 1 : cboPrinter.SelectedIndex;

                        fn_SetWorkers();
                    }
                }

                cmbSerialPorts.DataSource = System.IO.Ports.SerialPort.GetPortNames();
            }
            catch (Exception ex)
            {

            }
        }

        private void Chk_byhand_CheckStateChanged(object sender, EventArgs e)
        {
            try
            {
                System.Windows.Forms.CheckBox cc = (System.Windows.Forms.CheckBox)sender;

                if (!cc.Checked) return;

                InputPw ip = new InputPw(this, null, "저울 측정값 수기 입력(배합지시)");

                ip.ShowDialog();
            }
            catch (Exception ex)
            {
            }
        }


        private void Chk_StockQty_CheckStateChanged(object sender, EventArgs e)
        {
            try
            {
                System.Windows.Forms.CheckBox cc = (System.Windows.Forms.CheckBox)sender;

                if (!cc.Checked) return;

                InputPw ip = new InputPw(this, null, "마이너스 재고 수량 넘어가기(배합지시)");

                ip.ShowDialog();
            }
            catch (Exception ex)
            {
                cm.writeLog($"REGI_COMBI_A Chk1_CheckStateChanged Error : {ex.Message}");
            }
        }

        private void fn_txt지시수량KeyPress()
        {
            try
            {
                foreach (DataGridViewRow dr in grid_State2.Rows)
                {
                    dr.Cells["TUIP_QTY"].Value = string.Format("{0:#,##0.00}", double.Parse(dr.Cells["SO_QTY"].Value.ToString()) * (double.Parse(txt지시수량.Text.Replace(",",string.Empty)) / (!cbo_kind.Text.Equals("약품") ? 1 : 100)));
                }

                txt투입량.Text = grid_State2.SelectedRows[0].Cells["TUIP_QTY"].Value.ToString();

                txtLotNo.Focus();

            }
            catch (Exception ex)
            {

            }
        }

        private void Txt저울측정값_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode != Keys.Enter) return;

                if (!fn_chkInputLotNo())
                {
                    MessageBox.Show("원재료LotNo를 입력해 주십시오.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    txt저울측정값.Text = string.Empty;

                    txtLotNo.Focus();

                    return;
                }

                if (!chk_byhand.Checked)
                {
                    MessageBox.Show("[측정값 수기 입력]을 체크하여 주십시오.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    txt저울측정값.Text = string.Empty;

                    txt저울측정값.Focus();

                    return;
                }

                if (string.IsNullOrEmpty(txt저울측정값.Text)) return;

                txt저울측정값.Text = txt저울측정값.Text.Trim();

                foreach (DataRow measuerDr in drMeasures)
                {
                    txt저울측정값.Text = txt저울측정값.Text.Contains(measuerDr["COM1"].ToString()) ? txt저울측정값.Text.Replace(measuerDr["COM1"].ToString(), string.Empty) : txt저울측정값.Text;
                }

                fn_Processing();
            }
            catch (Exception ex)
            {

            }
        }

        private void fn_txtLotNoKeypress()
        {
            try
            {
                if (string.IsNullOrEmpty(txt원재료코드.Text))
                {
                    MessageBox.Show("선택된 원재료코드가 없습니다.", "원재료LOT", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    return;
                }

                if (string.IsNullOrEmpty(cbo작업자.Text))
                {
                    MessageBox.Show("작업자를 선택하여 주십시오.", "원재료LOT", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    return;
                }

                //if (!this.lb_SerialStatus.Text.Equals("Linked"))
                //{
                //    MessageBox.Show("시리얼포트를 선택하여 연결해 주시기 바랍니다.", "시리얼포트 입력 요망", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                //    return;
                //}

                if (grid_State.SelectedRows[0].Cells["FIN_YN"].Value.ToString().Equals("완료"))
                {
                    MessageBox.Show("이미 실적이 등록된 지시입니다.\n확인부탁드립니다.", "원재료LOT", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    return;
                }

                fn_Chk_RegisterLotNoFIFO();

                string strSql = dbName.Equals("ERP_2") ? $"{dbName}.dbo.ST_COMBI_PRO_LOTNO_SEL" : $"{dbName}.dbo.ST_COMBI_PRO_LOTNO_SEL_TEST";

                db.Parameter("@ITM_CD", txt원재료코드.Text);

                db.Parameter("@MNG_NO", txtLotNo.Text);

                db.ExecuteSql(strSql);

                if (db.nState)
                {
                    if (db.result.Rows.Count > 0)
                    {
                        DataRow dr = db.result.Rows[0];

                        if (!dr["END_QTY"].ToString().Equals("-"))
                        {
                            if (Convert.ToDouble(dr["END_QTY"]) <= 0 && !chk_StockQty.Checked)
                            {
                                MessageBox.Show($"{txtLotNo.Text}의 원재료코드가 마이너스 혹은 0인 재고상태 입니다.\n[재고넘어가기]를 체크하여 주십시오.", "마이너스 재고상태", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                                txtLotNo.SelectAll();

                                return;
                            }
                        }

                        txt재고량.Text = dr["END_QTY"].ToString();

                        grid_State2.SelectedRows[0].Cells["RAWLOT"].Value = string.Empty;

                        txt저울측정값.Text = string.Empty;

                        txt저울측정값.Focus();

                        return;
                    }

                    MessageBox.Show("[ERP시스템]에서 해당제품의 LotNo를 검색할수 없습니다.", "전산실에 문의 바람", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    txtLotNo.Text = string.Empty;

                    grid_State2.SelectedRows[0].Cells[5].Value = string.Empty;

                    txt재고량.Text = string.Empty;

                    txtLotNo.Focus();

                }
            }
            catch (Exception ex)
            {

            }
        }

        private bool fn_isRaw(string GD_CD)
        {
            try
            {
                if (GD_CD.Substring(0, 1) == "B" && GD_CD.Length <= 5) return false;

                return true;
            }
            catch(Exception ex)
            {
                return true;
            }
        }

        private void fn_Chk_RegisterLotNoFIFO()
        {
            try
            {

                string strSql = dbName.Equals("ERP_2") ? $"{dbName}.dbo.ST_COMBI_PRO_LOTNO_FIFO_SEL" : $"{dbName}.dbo.ST_COMBI_PRO_LOTNO_FIFO_SEL_TEST";

                db.Parameter("@ITM_CD", txt원재료코드.Text);

                db.ExecuteSql(strSql);

                if (db.nState)
                {
                    DataRow rsdr = db.result.Rows[0];

                    if (!rsdr["MNG_NO"].ToString().Equals(txtLotNo.Text))
                    {
                        LotNoFIFO lf = new LotNoFIFO(db.result, txtLotNo.Text);

                        lf.ShowDialog();
                    }
                }

            }
            catch(Exception ex)
            {

            }
        }

        private void Regi_Combi_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (serialPort1.IsOpen)             // 시리얼포트가 열려 있으면
                {
                    serialPort1.Close();            // 시리얼포트 닫기

                    cmbSerialPorts.Enabled = true;  // COM포트설정 콤보박스 활성화
                }

                if (workSheet != null) Marshal.ReleaseComObject(workSheet);
                if (workBook != null) Marshal.ReleaseComObject(workBook);
                
                if (application != null)
                {
                    application.Quit();
                    Marshal.ReleaseComObject(application);
                }

                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            catch (Exception ex)
            {
                cm.writeLog($"Regi_Combi_A Regi_Combi_A_FormClosing Error : {ex.ToString()}");
            }
        }

        private void Grid_State2_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                DataGridView dg = sender as DataGridView;

                if (dg.SelectedRows.Count == 0) return;

                txtLotNo.Text = string.Empty;

                txt재고량.Text = string.Empty;

                txt원재료코드.Text = dg.SelectedRows[0].Cells["JA_CD"].Value.ToString();

                txt원재료명.Text = dg.SelectedRows[0].Cells["GD_NM"].Value.ToString();

                txt투입량.Text = dg.SelectedRows[0].Cells["TUIP_QTY"].Value.ToString();

                txtLotNo.Text = dg.SelectedRows[0].Cells["RAWLOT"].Value.ToString();

                txt재고량.Text = dg.SelectedRows[0].Cells["STOCK_QTY"].Value.ToString();

                txtLotNo.Focus();
            }
            catch (Exception ex)
            {

            }
        }

        private void Cbo입고기간_SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {
                switch (cbo입고기간.Text.ToString())
                {
                    case "전체":

                        dtp입고일1.Enabled = false;

                        dtp입고일2.Enabled = false;

                        break;

                    case "1개월":

                        dtp입고일1.Enabled = true;

                        dtp입고일2.Enabled = true;

                        dtp입고일2.Value = DateTime.Now;

                        dtp입고일1.Value = DateTime.Now.AddMonths(-1);

                        break;

                    case "2개월":

                        dtp입고일1.Enabled = true;

                        dtp입고일2.Enabled = true;

                        dtp입고일2.Value = DateTime.Now;

                        dtp입고일1.Value = DateTime.Now.AddMonths(-2);

                        break;

                    case "3개월":

                        dtp입고일1.Enabled = true;

                        dtp입고일2.Enabled = true;

                        dtp입고일2.Value = DateTime.Now;

                        dtp입고일1.Value = DateTime.Now.AddMonths(-3);

                        break;

                    case "6개월":

                        dtp입고일1.Enabled = true;

                        dtp입고일2.Enabled = true;

                        dtp입고일2.Value = DateTime.Now;

                        dtp입고일1.Value = DateTime.Now.AddMonths(-6);

                        break;

                    case "1년":

                        dtp입고일1.Enabled = true;

                        dtp입고일2.Enabled = true;

                        dtp입고일2.Value = DateTime.Now;

                        dtp입고일1.Value = DateTime.Now.AddMonths(-12);

                        break;

                    case "기간지정":

                        dtp입고일1.Enabled = true;

                        dtp입고일2.Enabled = true;

                        dtp입고일2.Value = DateTime.Now;

                        dtp입고일1.Value = DateTime.Now;

                        break;
                }

            }
            catch (Exception ex)
            {

            }
        }

        private void GetData()
        {
            try
            {
                //string strSql = $"{dbName}.dbo.ST_COMBI_COMMON_CODE_SEL_EMAX";

                string strSql = $"ERP_2.dbo.ST_COMBI_COMMON_CODE_SEL_EMAX";

                db.Parameter("@F_NAME", this.Name);

                db.ExecuteSql(strSql);

                drMeasures = db.result.Select($"KIND = 'MEASURE_VALUE'");

                DataRow[] pwdr = db.result.Select($"KIND = 'PW'");

                this.PwStr = pwdr[0]["COM1"].ToString();
            }
            catch (Exception ex)
            {
                cm.writeLog($"REGI_COMBI_A GetData Error : {ex.Message}");  // Exception 에러로 빠졌을시 로그쌓기
            }
        }

        private async void fn_DataRefresh(bool isRegiPro)
        {
            try
            {

                string strSql = dbName.Equals("ERP_2") ? $"{dbName}.dbo.ST_COMBI_PLANNO_SEL_EMAX" : $"{dbName}.dbo.ST_COMBI_PLANNO_SEL_EMAX_TEST";

                db.Parameter("@START_JOB_DT", dtp입고일1.Value.ToString("yyyyMMdd"));

                db.Parameter("@END_JOB_DT", dtp입고일2.Value.ToString("yyyyMMdd"));

                db.Parameter("@CHK_ALL", cbo입고기간.Text.Equals("전체") ? "Y" : "N");

                db.Parameter("@KIND", cboKind[cbo_kind.Text]);

                db.Parameter("@FINYN", chkFinYn.Checked ? 1 : 0);

                UpdateStatus?.Invoke("[배합실적등록] 조회중입니다..잠시만 기다려주십시오.");

                await Task.Run(() => db.ExecuteSql(strSql));

                if (db.nState)
                {
                    grid_State.DataSource = db.result;

                    grid_State.Columns["FIN_YN"].HeaderText = "완료\n여부";
                    grid_State.Columns["LOT_NO"].HeaderText = "LotNo";
                    grid_State.Columns["JOB_NO"].HeaderText = "생산지시\n번호";
                    grid_State.Columns["JOB_DT"].HeaderText = "지시\n일자";
                    grid_State.Columns["GD_NM"].HeaderText = "품명";
                    grid_State.Columns["JOB_QTY"].HeaderText = "지시\n수량";
                    grid_State.Columns["UNIT_CD"].HeaderText = "단위";
                    grid_State.Columns["MC_CD"].HeaderText = "사출기계";
                    grid_State.Columns["PLAN_MCCD"].HeaderText = "사출호기";

                    grid_State.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 11, FontStyle.Bold);

                    grid_State.Columns["FIN_YN"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    grid_State.Columns["LOT_NO"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    grid_State.Columns["JOB_NO"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    grid_State.Columns["JOB_DT"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    grid_State.Columns["GD_NM"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    grid_State.Columns["JOB_QTY"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    grid_State.Columns["UNIT_CD"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    grid_State.Columns["MC_CD"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    grid_State.Columns["PLAN_MCCD"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                    grid_State.Columns["GD_CD"].Visible = false;
                    grid_State.Columns["SPEC"].Visible = false;
                    grid_State.Columns["JOB_SEQ"].Visible = false;
                    grid_State.Columns["GONG_CD"].Visible = false;
                    grid_State.Columns["WA_CD"].Visible = false;
                    grid_State.Columns["WA_GU"].Visible = false;
                    grid_State.Columns["JOB_GU"].Visible = false;
                    grid_State.Columns["GONG_CD"].Visible = false;
                    grid_State.Columns["Y_HOUR"].Visible = false;
                    grid_State.Columns["PR_DT"].Visible = false;
                    grid_State.Columns["PR_JOB_QTY"].Visible = false;
                    grid_State.Columns["PR_QTY"].Visible = false;
                    grid_State.Columns["PR_FAC_CD"].Visible = false;
                    grid_State.Columns["PR_NO"].Visible = false;
                    grid_State.Columns["PR_Y_DAY"].Visible = false;
                    grid_State.Columns["COLOR_R"].Visible = false;
                    grid_State.Columns["COLOR_G"].Visible = false;
                    grid_State.Columns["COLOR_B"].Visible = false;

                    UpdateStatus?.Invoke($"[배합실적등록 - 생산지시] {db.result.Rows.Count} 행이 출력되었습니다.");

                    if (isRegiPro && !string.IsNullOrEmpty(prLotNo))
                    {
                        foreach(DataGridViewRow dgr in grid_State.Rows)
                        {
                            if (dgr.Cells["LOT_NO"].Value.ToString().Equals(prLotNo))
                            {
                                dgr.Selected = true;

                                break;
                            }
                        }
                    }

                    fn_DetailSel(isRegiPro);

                }
            }
            catch (Exception ex)
            {

            }
        }

        private bool fn_chkDuringCalculating()
        {
            try
            {
                if (grid_State2.Rows.Count == 0) return true;

                foreach (DataGridViewRow dgr in grid_State2.Rows)
                {
                    if (!string.IsNullOrEmpty(dgr.Cells["RAWLOT"].Value.ToString()) || !string.IsNullOrEmpty(dgr.Cells["REALTUIPQTY"].Value.ToString())) return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                return true;
            }
        }

        private void fn_TextClear()
        {
            try
            {
                txtLotNo.Text = string.Empty;

                txt저울측정값.Text = string.Empty;

                txt재고량.Text = string.Empty;

                txt배합Lot.Text = string.Empty;

                txt누적투입량.Text = "0";

                lbl판정.Text = string.Empty;

                lbl판정.BackColor = Color.FromArgb(224, 224, 224);
            }
            catch (Exception ex)
            {

            }
        }

        private void fn_DetailSel(bool isRegiPro)
        {
            try
            {
                DataGridViewRow selectedRow = null;

                if (grid_State.Rows.Count == 0)
                {
                    DataGridViewRow dg = new DataGridViewRow();

                    dg.CreateCells(grid_State);

                    selectedRow = dg;

                    return;
                }
                else selectedRow = grid_State.SelectedRows[0];

                if (!isRegiPro && !fn_chkDuringCalculating() && !selectedRow.Cells["JOB_NO"].Value.ToString().Equals(txt생산지시.Text))
                {
                    if (DialogResult.No == MessageBox.Show("입력하고 있는 저울값이 있습니다.\n다른 지시를 선택하시면 초기화됩니다.\n그래도 선택하시겠습니까?", "배합지시선택", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)) return;
                }

                fn_TextClear();

                txt배합Lot.Text = selectedRow.Cells["FIN_YN"].Value.ToString().Equals("완료") ? selectedRow.Cells["LOT_NO"].Value.ToString() : string.Empty;

                txt생산지시.Text = selectedRow.Cells["JOB_NO"].Value.ToString();

                txt품목코드.Text = selectedRow.Cells["GD_CD"].Value.ToString();

                txt품목명.Text = selectedRow.Cells["GD_NM"].Value.ToString();

                txt규격.Text = selectedRow.Cells["SPEC"].Value.ToString();

                //txt지시수량.Text = selectedRow.Cells["JOB_QTY"].Value.ToString().Replace(",", string.Empty); // string.Format("{0:#,##0.00}"

                txt지시수량.Text = string.Format("{0:#,##0.00}", selectedRow.Cells["JOB_QTY"].Value.ToString());

                //tbx유효기간.Text = selectedRow.Cells["Y_HOUR"].Value.ToString();

                if (selectedRow.Cells["Y_HOUR"].Value != null)
                {
                    switch (Convert.ToInt32(string.IsNullOrEmpty(selectedRow.Cells["Y_HOUR"].Value.ToString()) ? "0" : selectedRow.Cells["Y_HOUR"].Value) / 24)
                    {
                        case 2:

                            cbo유효기간.SelectedIndex = 0;

                            break;

                        case 3:

                            cbo유효기간.SelectedIndex = 1;

                            break;

                        case 4:

                            cbo유효기간.SelectedIndex = 2;

                            break;

                        case 10:

                            cbo유효기간.SelectedIndex = 3;

                            break;

                        default:

                            cbo유효기간.SelectedIndex = 0;

                            break;
                    }
                }

                MC_CD = selectedRow.Cells["MC_CD"].Value.ToString();

                string strSql = dbName.Equals("ERP_2") ? $"{dbName}.dbo.ST_COMBI_BOMQTY_SEL_EMAX" : $"{dbName}.dbo.ST_COMBI_BOMQTY_SEL_EMAX_TEST";

                db.Parameter("@GD_CD", selectedRow.Cells["GD_CD"].Value.ToString());

                db.Parameter("@GONG_CD", selectedRow.Cells["JOB_GU"].Value.ToString());

                db.Parameter("@QTY", txt지시수량.Text.Replace(",", string.Empty));

                db.ExecuteSql(strSql);

                if (db.nState)
                {
                    grid_State2.DataSource = db.result;

                    grid_State2.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                    grid_State2.Columns["SEQ"].HeaderText = "순번";
                    grid_State2.Columns["JA_CD"].HeaderText = "품목코드";
                    grid_State2.Columns["GD_NM"].HeaderText = "품명";
                    grid_State2.Columns["SO_QTY"].HeaderText = "소요량";
                    grid_State2.Columns["TUIP_QTY"].HeaderText = "투입량";
                    grid_State2.Columns["RAWLOT"].HeaderText = "원재료LOT";
                    grid_State2.Columns["REALTUIPQTY"].HeaderText = "실투입량";

                    grid_State2.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 20, FontStyle.Bold);

                    grid_State2.Columns["SEQ"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    grid_State2.Columns["JA_CD"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    grid_State2.Columns["GD_NM"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                    grid_State2.Columns["SO_QTY"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    grid_State2.Columns["TUIP_QTY"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    grid_State2.Columns["RAWLOT"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    grid_State2.Columns["REALTUIPQTY"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                    grid_State2.Columns["GD_NM"].Width = 140;

                    for (int i = 0; i < grid_State2.Columns.Count; i++)
                    {
                        grid_State2.Columns[i].ReadOnly = true;

                        grid_State2.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    }
                }

                grid_State2.Focus();

                txtLotNo.Focus();
            }
            catch (Exception ex)
            {

            }
        }

        public void SetGridRowHeader(DataGridView dg, int rowHeight, bool readonlychk)
        {
            try
            {
                dg.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;

                dg.ColumnHeadersHeight = 50;

                dg.EnableHeadersVisualStyles = false;

                dg.ColumnHeadersDefaultCellStyle.BackColor = Color.LightGray;

                dg.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                dg.RowHeadersVisible = false;

                dg.RowTemplate.Height = rowHeight != -1 ? rowHeight : dg.RowTemplate.Height;

                dg.RowTemplate.Resizable = DataGridViewTriState.False;

                dg.AllowUserToAddRows = false;

                dg.ReadOnly = readonlychk;

                dg.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            }
            catch (Exception ex)
            {

            }
        }
    }
}
