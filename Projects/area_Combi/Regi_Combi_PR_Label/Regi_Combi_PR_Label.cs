using COMBINATION.Label;
using COMBINATION.MixingLabel;
using COMBINATION.Modules;
using DevExpress.XtraEditors;
using DevExpress.XtraPrinting.BarCode;
using DevExpress.XtraReports.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;

namespace COMBINATION
{
    public partial class Regi_Combi_PR_Label : Form
    {
        public Action<string> UpdateStatus { get; set; }

        private static string dbName = "ERP_2";

        MSSQL db = new MSSQL(dbName);

        CommonModule cm = new CommonModule();

        private Dictionary<string, string> cboKind = new Dictionary<string, string>();

        PrintInfo pf = null;

        private Excel.Application application = null;

        private Excel.Workbook workBook = null;

        private Excel.Worksheet workSheet = null;

        BarCodeControl barCodeControl = null;

        public Regi_Combi_PR_Label()
        {
            InitializeComponent();

            initControl();
        }

        private void initControl()
        {
            try
            {
                fn_ComboInit();

                SetGridRowHeader(grid_State, 40, false);

                SetGridRowHeader(grid_State2, 40, true);

                btn_Search.Click += (s, e) => { fn_RegiDataBinding(); };

                this.btn_Print.Click += (s, e) => {

                    if (cbx_kind.Text.Equals("약품"))
                    {
                        if (cbx_PrintKind.SelectedIndex == 0) fn_cmdReportClickforEpsonQRBarcode(); else fn_cmdReportClickforZebraQRBarcode();
                    }
                    else
                    {
                        if (cbx_PrintKind.SelectedIndex == 0) fn_cmdReportClickforEpson2DBarcode(); else fn_cmdReportClickforZebra2DBarcode();
                    }

                    //if (cbx_PrintKind.SelectedIndex == 0) fn_cmdReportClickforEpson2DBarcode(); else fn_cmdReportClickforZebra2DBarcode();
                };

                this.dtpStart.ValueChanged += (s, e) => { fn_RegiDataBinding(); };

                this.dtpEnd.ValueChanged += (s, e) => { fn_RegiDataBinding(); };

                this.tbx_GDNM.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) fn_RegiDataBinding(); };

                this.tbx_GDCDNM.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) fn_RegiDataBinding(); };

                this.cbx_kind.SelectedIndexChanged += (s, e) => { fn_RegiDataBinding(); };

                this.tbx_GDNM.Select();

                this.FormClosing += Regi_Combi_PR_Label_FormClosing;

                grid_State.CellClick += (s, e) => { fn_RegiDataDetailBinding(false); };

                fn_RegiDataBinding();
            }
            catch(Exception ex)
            {

            }
        }

        private void Regi_Combi_PR_Label_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
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
            catch(Exception ex )
            {

            }
        }

        private void fn_ComboInit()
        {
            try
            {
                string strSql = $"{dbName}.dbo.ST_COMBOBOX_DATA_SEL";

                db.Parameter("@F_NAME", this.Name);

                db.ExecuteSql(strSql);

                if (db.nState)
                {
                    if(db.result.Rows.Count > 0)
                    {
                        cm.ComboBoxBinding(db.result, cbx_kind, "COMBO_9",cboKind);

                        cm.ComboBoxBinding(db.result, cbx_PrintKind, "COMBO_16");

                        cbx_kind.SelectedIndex = cbx_kind.Items.Count > 0 ? 3 : cbx_kind.SelectedIndex;

                        cbx_PrintKind.SelectedIndex = cbx_PrintKind.Items.Count > 0 ? 1 : cbx_PrintKind.SelectedIndex;
                    }
                }

            }
            catch(Exception ex)
            {

            }
        }

        private async void fn_RegiDataBinding()
        {
            try
            {
                string strSql = $"{dbName}.dbo.ST_COMBI_PR_LABEL_SEL";

                db.Parameter("@START_DATE",dtpStart.Value.ToString("yyyy-MM-dd"));
                db.Parameter("@END_DATE", dtpEnd.Value.ToString("yyyy-MM-dd"));
                db.Parameter("@KIND", cboKind[cbx_kind.Text]);
                db.Parameter("@GD_CD", tbx_GDCDNM.Text);
                db.Parameter("@GD_NM", tbx_GDNM.Text);

                UpdateStatus?.Invoke($"조회중입니다..잠시만 기다려주십시오.");

                await Task.Run(() => db.ExecuteSql(strSql));

                if (db.nState)
                {
                    grid_State.DataSource = db.result;

                    grid_State.Columns["PR_DT"].HeaderText = "생산\n일자";
                    grid_State.Columns["LABEL_DT"].HeaderText = "라벨출력\n일자";
                    grid_State.Columns["LOT_NO"].HeaderText = "LotNo";
                    grid_State.Columns["GD_CD"].HeaderText = "품목코드";
                    grid_State.Columns["GD_NM"].HeaderText = "품명";
                    grid_State.Columns["PR_QTY"].HeaderText = "양품수량";

                    grid_State.Columns["PR_NO"].Visible = false;
                    grid_State.Columns["SPEC"].Visible = false;
                    grid_State.Columns["EXP_HR"].Visible = false;
                    grid_State.Columns["FROM_TIME"].Visible = false;
                    grid_State.Columns["COLOR_R"].Visible = false;
                    grid_State.Columns["COLOR_G"].Visible = false;
                    grid_State.Columns["COLOR_B"].Visible = false;

                    grid_State.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 11, FontStyle.Bold);

                    grid_State.Columns["PR_DT"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    grid_State.Columns["LOT_NO"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    grid_State.Columns["GD_CD"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    grid_State.Columns["GD_NM"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    grid_State.Columns["PR_QTY"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

                    grid_State.Columns["PR_DT"].ReadOnly = true;
                    grid_State.Columns["GD_CD"].ReadOnly = true;
                    grid_State.Columns["GD_NM"].ReadOnly = true;
                    grid_State.Columns["PR_QTY"].ReadOnly = true;

                    UpdateStatus?.Invoke($"[생산실적목록] {db.result.Rows.Count}행, ");

                    if (db.result.Rows.Count > 0)
                    {
                        grid_State.Rows[0].Selected = true;

                        fn_RegiDataDetailBinding(true);
                    }
                }
            }
            catch(Exception ex )
            {

            }
        }

        private void fn_RegiDataDetailBinding(bool fromGridStateOrNot)
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

                string strSql = $"{dbName}.dbo.ST_COMBI_PR_LABEL_DETAIL_SEL";

                db.Parameter("@PR_NO", selectedRow.Cells["PR_NO"].Value.ToString());
                db.Parameter("@KIND", cboKind[cbx_kind.Text]);

                db.ExecuteSql(strSql);

                if (db.nState)
                {
                    grid_State2.DataSource = db.result;

                    grid_State2.Columns["PR_SEQ"].HeaderText = "순번";
                    grid_State2.Columns["R_GD_CD"].HeaderText = "배합\n품목코드";
                    grid_State2.Columns["GD_CD"].HeaderText = "원재료\n품목코드";
                    grid_State2.Columns["SO_QTY"].HeaderText = "품명";
                    grid_State2.Columns["TUIP_QTY"].HeaderText = "양품수량";
                    grid_State2.Columns["LOT_NO"].HeaderText = "원재료\nLotNo";
                    grid_State2.Columns["REMK"].HeaderText = "비고";

                    grid_State2.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 11, FontStyle.Bold);

                    grid_State2.Columns["PR_SEQ"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    grid_State2.Columns["R_GD_CD"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    grid_State2.Columns["GD_CD"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    grid_State2.Columns["SO_QTY"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    grid_State2.Columns["TUIP_QTY"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    grid_State2.Columns["LOT_NO"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    grid_State2.Columns["REMK"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

                    UpdateStatus?.Invoke($"[투입품목] {db.result.Rows.Count}행이 출력되었습니다.");
                }
            }
            catch(Exception ex)
            {

            }
        }

        private void fn_cmdReportClickforEpson2DBarcode()
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

                if (string.IsNullOrEmpty(strQty.Text) || strQty.Text.Equals("0"))
                {
                    MessageBox.Show("인쇄 매수 수량이 없거나 0입니다.", "인쇄 오류");

                    return;
                }

                if (selectedRow.Cells["LOT_NO"].Value == null || string.IsNullOrEmpty(selectedRow.Cells["LOT_NO"].Value.ToString()))
                {
                    MessageBox.Show("출력하려는 LotNo가 비어있습니다.\n확인부탁드립니다.", "인쇄 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    return;
                }

                DateTime sDate = Convert.ToDateTime(selectedRow.Cells["LABEL_DT"].Value).AddDays(Convert.ToInt32(selectedRow.Cells["EXP_HR"].Value) / 24);

                if (MessageBox.Show($"Report를 출력 하시겠습니까?\n\n약품번호 : {selectedRow.Cells["LOT_NO"].Value}\n\n배합일시 : {selectedRow.Cells["LABEL_DT"].Value}\n\n유통 기간 : " +
                                    $"{sDate.ToString("MM/dd")} / {selectedRow.Cells["FROM_TIME"].Value}\n\n품  명 : {selectedRow.Cells["GD_NM"].Value} / {selectedRow.Cells["SPEC"].Value}", "출력", MessageBoxButtons.YesNo) == DialogResult.No) return;


                if (application == null)
                {
                    application = new Excel.Application();

                }

                application.Visible = false;

                string FileName = $"{Application.StartupPath}\\MixingLabel\\MixingLabel.xlsx";

                if (!File.Exists(FileName)) return;

                workBook = application.Workbooks.Open(FileName);

                workSheet = workBook.Worksheets["sheet1"];


                int cl_r = Convert.ToInt32(selectedRow.Cells["COLOR_R"].Value);

                int cl_g = Convert.ToInt32(selectedRow.Cells["COLOR_G"].Value);

                int cl_b = Convert.ToInt32(selectedRow.Cells["COLOR_B"].Value);

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


                workSheet.Cells[3, 8].Value = selectedRow.Cells["LOT_NO"].Value.ToString();

                workSheet.Cells[5, 8].Value = $"{Convert.ToDateTime(selectedRow.Cells["LABEL_DT"].Value).ToString("yyyy-MM-dd")} {selectedRow.Cells["FROM_TIME"].Value}";

                string strYdt = Convert.ToDateTime(selectedRow.Cells["LABEL_DT"].Value).AddDays(Convert.ToInt32(selectedRow.Cells["EXP_HR"].Value) / 24).ToString("MM/dd");

                workSheet.Cells[7, 8].Value = $"{strYdt} {selectedRow.Cells["FROM_TIME"].Value} 까지";

                for (int i = 1; i <= int.Parse(strQty.Text); i++)
                {
                    workSheet.Cells[9, 8].Value = $"{selectedRow.Cells["GD_NM"].Value} {selectedRow.Cells["SPEC"].Value} {i}/{strQty.Text}";

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
                DataGridViewRow selectedRow = null;

                if (grid_State.Rows.Count == 0)
                {
                    DataGridViewRow dg = new DataGridViewRow();

                    dg.CreateCells(grid_State);

                    selectedRow = dg;

                    return;
                }
                else selectedRow = grid_State.SelectedRows[0];

                if (string.IsNullOrEmpty(strQty.Text) || strQty.Text.Equals("0"))
                {
                    MessageBox.Show("인쇄 매수 수량이 없거나 0입니다.", "인쇄 오류");

                    return;
                }

                DateTime sDate = Convert.ToDateTime(selectedRow.Cells["LABEL_DT"].Value).AddDays(Convert.ToInt32(selectedRow.Cells["EXP_HR"].Value) / 24);

                if (MessageBox.Show($"Report를 출력 하시겠습니까?\n\n약품번호 : {selectedRow.Cells["LOT_NO"].Value}\n\n배합일시 : {selectedRow.Cells["LABEL_DT"].Value}\n\n유통 기간 : " +
                                    $"{sDate.ToString("MM/dd")} / {selectedRow.Cells["FROM_TIME"].Value}\n\n품  명 : {selectedRow.Cells["GD_NM"].Value} / {selectedRow.Cells["SPEC"].Value}", "출력", MessageBoxButtons.YesNo) == DialogResult.No) return;


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


                workSheet.Cells[3, 8].Value = selectedRow.Cells["LOT_NO"].Value.ToString();

                string strYdt = Convert.ToDateTime(selectedRow.Cells["LABEL_DT"].Value).AddDays(Convert.ToInt32(selectedRow.Cells["EXP_HR"].Value) / 24).ToString("MM/dd");

                workSheet.Cells[5, 8].Value = $"{strYdt} {selectedRow.Cells["FROM_TIME"].Value} 까지";

                barCodeControl.Name = selectedRow.Cells["LOT_NO"].Value.ToString();
                barCodeControl.Text = selectedRow.Cells["LOT_NO"].Value.ToString();

                System.Drawing.Image image = null;

                while (image == null)
                {
                    image = barCodeControl.ExportToImage();
                }

                Clipboard.SetDataObject(image, true);

                QRCoderange = (Excel.Range)workSheet.Cells[7, 9];

                Thread.Sleep(50);  // 클립보드에 bitmap형식으로 담은 객체를 넣는 시간이 걸림

                workSheet.Paste(QRCoderange, (Bitmap)image);

                for (int i = 1; i <= int.Parse(strQty.Text); i++)
                {
                    //workSheet.Cells[9, 8].Value = $"{txt품목명.Text} {txt규격.Text} {i}/{strQty.Text}";

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

        private void fn_cmdReportClickforZebraQRBarcode()
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

                if (string.IsNullOrEmpty(strQty.Text) || strQty.Text.Equals("0"))
                {
                    MessageBox.Show("인쇄 매수 수량이 없거나 0입니다.", "인쇄 오류");

                    return;
                }

                DateTime sDate = Convert.ToDateTime(selectedRow.Cells["LABEL_DT"].Value).AddDays(Convert.ToInt32(selectedRow.Cells["EXP_HR"].Value) / 24);

                if (MessageBox.Show($"Report를 출력 하시겠습니까?\n\n약품번호 : {selectedRow.Cells["LOT_NO"].Value}\n\n배합일시 : {selectedRow.Cells["LABEL_DT"].Value}\n\n유통 기간 : " +
                                    $"{sDate.ToString("MM/dd")} / {selectedRow.Cells["FROM_TIME"].Value}\n\n품  명 : {selectedRow.Cells["GD_NM"].Value} / {selectedRow.Cells["SPEC"].Value}", "출력", MessageBoxButtons.YesNo) == DialogResult.No) return;

                if (pf == null) pf = new PrintInfo();

                pf.setLotNo(selectedRow.Cells["LOT_NO"].Value.ToString());

                pf.setproDate($"{Convert.ToDateTime(selectedRow.Cells["LABEL_DT"].Value).ToString("yyyy-MM-dd")} {selectedRow.Cells["FROM_TIME"].Value}");

                string strYdt = Convert.ToDateTime(selectedRow.Cells["LABEL_DT"].Value).AddDays(Convert.ToInt32(selectedRow.Cells["EXP_HR"].Value) / 24).ToString("MM/dd");

                pf.setvaliDate($"{strYdt} {selectedRow.Cells["FROM_TIME"].Value} 까지");

                pf.setRGB(Convert.ToInt32(selectedRow.Cells["COLOR_R"].Value), Convert.ToInt32(selectedRow.Cells["COLOR_G"].Value), Convert.ToInt32(selectedRow.Cells["COLOR_B"].Value));

                for (int i = 1; i <= Convert.ToInt32(strQty.Value); i++)
                {
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

        private void fn_cmdReportClickforZebra2DBarcode()
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

                if (string.IsNullOrEmpty(strQty.Text) || strQty.Text.Equals("0"))
                {
                    MessageBox.Show("인쇄 매수 수량이 없거나 0입니다.", "인쇄 오류");

                    return;
                }

                if (selectedRow.Cells["LOT_NO"].Value == null || string.IsNullOrEmpty(selectedRow.Cells["LOT_NO"].Value.ToString()))
                {
                    MessageBox.Show("출력하려는 LotNo가 비어있습니다.\n확인부탁드립니다.", "인쇄 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    return;
                }

                DateTime sDate = Convert.ToDateTime(selectedRow.Cells["LABEL_DT"].Value).AddDays(Convert.ToInt32(selectedRow.Cells["EXP_HR"].Value) / 24);

                if (MessageBox.Show($"Report를 출력 하시겠습니까?\n\n약품번호 : {selectedRow.Cells["LOT_NO"].Value}\n\n배합일시 : {selectedRow.Cells["LABEL_DT"].Value}\n\n유통 기간 : " +
                                    $"{sDate.ToString("MM/dd")} / {selectedRow.Cells["FROM_TIME"].Value}\n\n품  명 : {selectedRow.Cells["GD_NM"].Value} / {selectedRow.Cells["SPEC"].Value}", "출력", MessageBoxButtons.YesNo) == DialogResult.No) return;


                if (pf == null) pf = new PrintInfo();

                pf.setLotNo(selectedRow.Cells["LOT_NO"].Value.ToString());

                pf.setproDate($"{Convert.ToDateTime(selectedRow.Cells["LABEL_DT"].Value).ToString("yyyy-MM-dd")} {selectedRow.Cells["FROM_TIME"].Value}");

                string strYdt = Convert.ToDateTime(selectedRow.Cells["LABEL_DT"].Value).AddDays(Convert.ToInt32(selectedRow.Cells["EXP_HR"].Value) / 24).ToString("MM/dd");

                pf.setvaliDate($"{strYdt} {selectedRow.Cells["FROM_TIME"].Value} 까지");

                pf.setRGB(Convert.ToInt32(selectedRow.Cells["COLOR_R"].Value), Convert.ToInt32(selectedRow.Cells["COLOR_G"].Value), Convert.ToInt32(selectedRow.Cells["COLOR_B"].Value));

                for (int i = 1; i <= Convert.ToInt32(strQty.Value); i++)
                {
                    pf.setGD_NM($"{selectedRow.Cells["GD_NM"].Value} {selectedRow.Cells["SPEC"].Value} {i}/{strQty.Text}");

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
