using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.IO.Ports;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Collections.Generic;

namespace area_L
{   
    public partial class REG_IN_ITEM : Form
    {
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern bool GetDefaultCommConfig(string lpszName, IntPtr lpCC, ref uint lpdwSize);

        readonly SerialPort serialPortScanner = new SerialPort();
        
        DataTable gridTable = new DataTable();

        string prMode = "NO";   // 필드로 유지
        int grid_cnt = 0;       // 필드로 유지
        int strSEQ = 0;       // 필드로 유지
        int input_total_qty = 0;     // 필드로 유지
        private string _scannerBuf = "";

        string gd_cd_ = null;
        int exp_qty_ = 0;
        string order_num_return = "";
        string gd_cd_return = "";
        int exp_qty_return = 0;
        string strFac;

        private Panel pnl_select;
        private DataGridView fpExcel;


        // =========================
        // 입고등록grid 컬럼 폭 유지/저장/복원
        // =========================
        private readonly Dictionary<string, int> _inGridColWidth = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        private bool _inGridSuppressWidthEvents = false;
        private bool _inGridEventsHooked = false;

        private string InGridLayoutPath
        {
            get
            {
                // exe 폴더가 아니라 사용자 AppData 밑(권한/배포 안정성 좋음)
                // 예: C:\Users\...\AppData\Local\<회사>\<앱>\<버전>\REG_IN_ITEM_입고등록grid_widths.ini
                return Path.Combine(Application.UserAppDataPath, "REG_IN_ITEM_입고등록grid_widths.ini");
            }
        }

        private void HookInGridEvents()
        {
            if (_inGridEventsHooked) return;

            // 사용자가 컬럼 폭을 바꿀 때마다 메모리에 즉시 반영(리플리쉬 유지용)
            입고등록grid.ColumnWidthChanged += 입고등록grid_ColumnWidthChanged;

            _inGridEventsHooked = true;
        }

        private void 입고등록grid_ColumnWidthChanged(object sender, DataGridViewColumnEventArgs e)
        {
            if (_inGridSuppressWidthEvents) return;
            if (e?.Column == null) return;
            if (!e.Column.Visible) return;

            int w = e.Column.Width;
            if (w <= 0) return;

            _inGridColWidth[e.Column.Name] = w;
        }

        private void LoadInGridLayoutFromFile()
        {
            _inGridColWidth.Clear();

            try
            {
                var path = InGridLayoutPath;
                if (!File.Exists(path)) return;

                foreach (var line in File.ReadAllLines(path, Encoding.UTF8))
                {
                    var s = (line ?? "").Trim();
                    if (s.Length == 0) continue;
                    if (s.StartsWith("#")) continue;

                    int p = s.IndexOf('=');
                    if (p <= 0) continue;

                    string key = s.Substring(0, p).Trim();
                    string val = s.Substring(p + 1).Trim();

                    if (key.Length == 0) continue;

                    if (int.TryParse(val, out int w) && w > 0)
                        _inGridColWidth[key] = w;
                }
            }
            catch
            {
                // 파일 읽기 실패 시에도 UI는 정상 동작해야 하므로 조용히 무시
            }
        }

        private void ApplyInGridLayout()
        {
            if (_inGridColWidth.Count == 0) return;

            _inGridSuppressWidthEvents = true;
            try
            {
                foreach (DataGridViewColumn col in 입고등록grid.Columns)
                {
                    if (col == null) continue;
                    if (!col.Visible) continue;

                    if (_inGridColWidth.TryGetValue(col.Name, out int w) && w > 0)
                        col.Width = w;
                }
            }
            finally
            {
                _inGridSuppressWidthEvents = false;
            }
        }

        private void SaveInGridLayoutToFile()
        {
            try
            {
                // 현재 UI 상태를 dict에 반영
                foreach (DataGridViewColumn col in 입고등록grid.Columns)
                {
                    if (col == null) continue;
                    if (!col.Visible) continue;
                    if (col.Width <= 0) continue;

                    _inGridColWidth[col.Name] = col.Width;
                }

                var path = InGridLayoutPath;
                var dir = Path.GetDirectoryName(path);
                if (!string.IsNullOrEmpty(dir))
                    Directory.CreateDirectory(dir);

                var sb = new StringBuilder();
                sb.AppendLine("# REG_IN_ITEM 입고등록grid column widths");
                foreach (var kv in _inGridColWidth)
                    sb.Append(kv.Key).Append('=').Append(kv.Value).AppendLine();

                File.WriteAllText(path, sb.ToString(), Encoding.UTF8);
            }
            catch
            {
                // 저장 실패해도 종료는 되어야 하므로 조용히 무시
            }
        }


        public REG_IN_ITEM()
        {

            InitializeComponent();
            CreateSelectionPanel();


            serialPortScanner.DataReceived += SerialPortScanner_DataReceived;

            fpExcel.CellDoubleClick += FpExcel_CellDoubleClick;
        }

        private void FpExcel_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var row = fpExcel.Rows[e.RowIndex];

            order_num_return = Convert.ToString(row.Cells[0].Value);
            gd_cd_return = Convert.ToString(row.Cells[1].Value);
            int.TryParse(Convert.ToString(row.Cells[2].Value), out exp_qty_return);

            //MessageBox.Show($"부족분 List : 수주번호({order_num_return}) / 제품코드 ({gd_cd_return}) / 수량 ({exp_qty_return}) 중 {exp_qty_} 개 차감합니다.",
            //    "확인", MessageBoxButtons.OK, MessageBoxIcon.Information);

            string s = order_num_return;
            s = s?.Trim() ?? "";
            int p = s.IndexOf(' ');
            string orderNo = (p >= 0) ? s.Substring(0, p) : s;
            showAlertForm(orderNo, exp_qty_return.ToString(), exp_qty_.ToString());

            DbHelper.ExecuteNonQuery(
                "UPDATE isuf_stock SET qty = qty - @q WHERE gd_cd = @gd AND order_num = @ord",
                new SqlParameter("@q", exp_qty_),
                new SqlParameter("@gd", gd_cd_return ?? (object)DBNull.Value),
                new SqlParameter("@ord", order_num_return ?? (object)DBNull.Value)
            );         

            hide_pnl_select();
        }


        private void show_pnl_select()
        {
            if (pnl_select == null)
            {
                CreateSelectionPanel();
            }
            pnl_select.Visible = true;
            pnl_select.BringToFront();
        }
        private void hide_pnl_select()
        {
            if (pnl_select != null)
            {
                pnl_select.Visible = false;
            }
        }
        private static int TwipsToPx(int twips) => (int)Math.Round(twips * 96.0 / 1440.0);
        private void CreateSelectionPanel()
        {
            pnl_select = new Panel
            {
                Name = "pnl_select",
                Location = new Point(TwipsToPx(810), TwipsToPx(630)),
                Size = new Size(TwipsToPx(4785), TwipsToPx(4455)),
                Visible = false,
                TabIndex = 17,
                Font = new Font("굴림", 12f, FontStyle.Bold),
                BackColor = ColorTranslator.FromWin32(14215660),
                BorderStyle = BorderStyle.FixedSingle
            };

            fpExcel = new DataGridView
            {
                Name = "fpExcel",
                Location = new Point(TwipsToPx(90), TwipsToPx(90)),
                Size = new Size(TwipsToPx(4560), TwipsToPx(4215)),
                TabIndex = 18,
                TabStop = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ScrollBars = ScrollBars.Both,
                GridColor = SystemColors.ControlDark,
                BackgroundColor = SystemColors.Window,
                EnableHeadersVisualStyles = false
            };
            fpExcel.Font = new Font("굴림", 11.25f, FontStyle.Regular);
            fpExcel.RowTemplate.Height = TwipsToPx(300);

            while (fpExcel.Columns.Count < 11)
            {
                int idx = fpExcel.Columns.Count;
                fpExcel.Columns.Add("C" + idx, "");
            }

            fpExcel.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            fpExcel.ColumnHeadersDefaultCellStyle.Font = new Font("굴림", 11.25f, FontStyle.Bold);
            fpExcel.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            pnl_select.Controls.Add(fpExcel);
            this.Controls.Add(pnl_select);
            pnl_select.BringToFront();
        }
      
        private void cmd_Upload_Click(object sender, EventArgs e)
        {
            using (var f = new Form2()) f.ShowDialog(this);
        }

        private void Mainform_Load(object sender, EventArgs e)
        {
            try
            {
                DbHelper.ConnectionString = "Data Source=192.168.2.5;Initial Catalog=ERP_2;User ID=interojo;Password=DB@$2022!;MultipleActiveResultSets=True;TrustServerCertificate=True";


                LoadInGridLayoutFromFile(); // <-- 추가 (폭 로드)

                Sheet_Setting();            // <-- 여기서 로드된 폭이 반영되도록

                HookInGridEvents();         // <-- 추가 (사용자 리사이즈 추적)
                

                EnumSerPorts();

                txtChk.Focus();


                strSEQ = 0;
                                
                lb_Totalstr.Text = "";


            }
            catch
            {
                MessageBox.Show("데이터베이스에 연결되지 못했습니다. 전산실로 연락주세요", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        void EnumSerPorts()
        {
            cmbSerialPorts.Items.Clear();
            foreach (var name in SerialPort.GetPortNames())
            {
                int n;
                var display = (name.StartsWith("COM", StringComparison.OrdinalIgnoreCase) &&
                               int.TryParse(name.Substring(3), out n))
                              ? "Com " + n
                              : name;
                cmbSerialPorts.Items.Add(display);
            }
            if (cmbSerialPorts.Items.Count > 0) cmbSerialPorts.SelectedIndex = 0;
        }
        private void cmbSerialPorts_Click(object sender, EventArgs e)
        {
            if (cmbSerialPorts.Items.Count == 0)
                return;

            int nPort = 0;
            var t = (cmbSerialPorts.Text ?? "").Trim();
            if (!string.IsNullOrEmpty(t))
            {
                t = t.ToUpperInvariant().Replace("COM", "").Trim();
                int.TryParse(t, out nPort);
            }

            if (serialPortScanner.IsOpen) serialPortScanner.Close();
            serialPortScanner.PortName = "COM" + nPort;
            serialPortScanner.BaudRate = 9600;
            serialPortScanner.Parity = Parity.None;
            serialPortScanner.DataBits = 8;
            serialPortScanner.StopBits = StopBits.One;
            serialPortScanner.Open();
            txtChk.Focus();
        }
        private void Mainform_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveInGridLayoutToFile(); // <-- 추가 (종료 시 저장)
            if (serialPortScanner.IsOpen) serialPortScanner.Close();
        }
        private void SerialPortScanner_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                var chunk = serialPortScanner.ReadExisting();
                if (string.IsNullOrEmpty(chunk)) return;
                _scannerBuf += chunk;
                int idx;
                while ((idx = _scannerBuf.IndexOfAny(new[] { '\n', '\r' })) >= 0)
                {
                    var line = _scannerBuf.Substring(0, idx).Trim();
                    _scannerBuf = _scannerBuf.Substring(idx + 1);
                    if (line.Length == 0) continue;
                    BeginInvoke((Action)(() => ProcessScannerBarcode(line)));
                }
            }
            catch { }
        }
        private void ProcessScannerBarcode(string barcode)
        {
            if (pnl_select.Visible)
            {
                MessageBox.Show("제품을 선택해주세요");
                return;
            }

            if (!serialPortScanner.IsOpen)
            {
                MessageBox.Show("[스케너] 포트 설정이 정확하지 않습니다.\r\n포트를 다시 한 번 확인하시기 바랍니다.", "포트 확인",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                if (string.IsNullOrWhiteSpace(barcode)) return;

                if (barcode == "<")
                {
                    prMode = "NO";
                    TryWriteSerial("Nothing Mode");
                    return;
                }
                if (barcode == "=")
                {
                    prMode = "INSERT";
                    TryWriteSerial("Insert Mode");
                    return;
                }
                if (barcode == ">")
                {
                    prMode = "NO";
                    TryWriteSerial("Nothing Mode");
                    return;
                }

                if (prMode != "INSERT")
                {
                    TryWriteSerial("Push the Button!!!");
                    return;
                }

                txtChk.Text = barcode;

                var sql = @"
SELECT 
  CASE A.STTS WHEN 'C' THEN '0' ELSE '1' END AS CHK,
  A.PR_NO AS IP_NO, A.PR_DT AS IP_DT, A.NEW_GDCD AS GD_CD, B.GD_NM, B.SPEC, B.UNIT_CD,
  A.LOT_NO,
  IP_QTY =
    (CASE WHEN ISNULL(C.sample_whcd_yn,'0') = '1' 
          THEN A.PR_QTY-ISNULL(A.NEW_SAMPLE_QTY,0)-ISNULL(D.NEW_SAMPLE_QTY,0) 
          ELSE A.PR_QTY END),
  MV_QTY =
    (CASE WHEN ISNULL(C.sample_whcd_yn,'0') = '1' 
          THEN A.PR_QTY-ISNULL(A.NEW_SAMPLE_QTY,0)-ISNULL(D.NEW_SAMPLE_QTY,0) 
          ELSE A.PR_QTY END),
  LOSS_QTY = 0,
  FA_CD = CASE LEFT(A.LOT_NO,1) WHEN 'BBB' THEN '02' ELSE '03' END,
  TMPS_GU = CASE LEFT(A.LOT_NO,1) WHEN 'BBB' THEN 'P003' ELSE 'W001' END,
  FMPS_GU = A.TWH_CD,
  A.SA_CD,
  MV_DT = CONVERT(date, GETDATE()),
  MV_NO = dbo.fnCodeNo('WI_JA1420', GETDATE()),
  A.PR_NO,
  GET_DT = GETDATE()
FROM PRTR1120 A
LEFT JOIN COMT1200 B ON A.NEW_GDCD = B.GD_CD
LEFT JOIN COMT1630 C ON B.SALE_CD = C.NM_CD
LEFT JOIN PRTR1120 D ON A.LOT_NO = D.LOT_NO AND D.GONG_CD = '55' AND D.STTS = 'C'
WHERE A.STTS = 'C'
  AND A.GONG_CD = '80'
  AND ISNULL(A.MV_YN,'N') <> 'Y'
  AND NOT EXISTS (SELECT 1 FROM JATR1230 WHERE A.PR_NO = PR_NO AND STTS = 'C')
  AND A.LOT_NO = @LOT
ORDER BY A.PR_NO, A.PR_DT";
                var dt = DbHelper.ExecuteDataTable(sql, new SqlParameter("@LOT", barcode));

                if (dt.Rows.Count == 0)
                {
                    TryWriteSerial("CheckSheet Not Found!");
                    lb_checksheet.BackColor = Color.Red;
                    txtChk.BackColor = Color.Red;
                    lb_Totalstr.Text = "※입고등록할 데이터없음";
                    lb_Totalstr.ForeColor = Color.Red;

                    for (int i = 0; i < 입고등록grid.Rows.Count; i++)
                    {
                        var cell = 입고등록grid.Rows[i].Cells.Count > 1 ? 입고등록grid.Rows[i].Cells[1] : null;
                        if (cell == null) continue;
                        var v = Convert.ToString(cell.Value)?.Trim();
                        if (!string.IsNullOrEmpty(v) && string.Equals(v, txtChk.Text.Trim(), StringComparison.OrdinalIgnoreCase))
                        {
                            for (int c = 0; c <= 9 && c < 입고등록grid.Columns.Count; c++)
                                입고등록grid.Rows[i].Cells[c].Style.BackColor = Color.Red;
                            lb_Totalstr.Text = "※이미 등록된 CHECK SHEET NO.";
                        }
                    }
                    return;
                }

                lb_checksheet.BackColor = Color.MistyRose;
                txtChk.BackColor = SystemColors.Window;
                lb_Totalstr.Text = "";
                lb_Totalstr.ForeColor = SystemColors.ControlText;

                for (int i = 0; i < 입고등록grid.Rows.Count; i++)
                    for (int c = 0; c <= 9 && c < 입고등록grid.Columns.Count; c++)
                        입고등록grid.Rows[i].Cells[c].Style.BackColor = SystemColors.Window;

                TryWriteSerial(barcode + "\r");

                var r0 = dt.Rows[0];
                var strLotNo = Convert.ToString(r0["LOT_NO"]);
                var strGDCD = Convert.ToString(r0["GD_CD"]);
                var strIPQTY = Convert.ToString(r0["IP_QTY"]);
                var strMVNO = Convert.ToString(r0["MV_NO"]);
                var strIPNO = Convert.ToString(r0["IP_NO"]);
                var strIPDT = Convert.ToString(r0["IP_DT"]);
                strFac = Convert.ToString(r0["FA_CD"]);
                var getDt = Convert.ToDateTime(r0["GET_DT"]).ToString("yyyy-MM-dd");

                DbHelper.ExecuteNonQuery(
                    @"INSERT INTO JATR1231 (MV_NO, MV_SEQ, GD_CD, MV_QTY, LOT_NO, RMV_QTY, LOSS_QTY, MPS_GU)
                  VALUES (@MV_NO, 1, @GD_CD, @MV_QTY, @LOT_NO, @RMV_QTY, 0, 'W001')",
                    new SqlParameter("@MV_NO", strMVNO),
                    new SqlParameter("@GD_CD", strGDCD),
                    new SqlParameter("@MV_QTY", strIPQTY),
                    new SqlParameter("@LOT_NO", strLotNo),
                    new SqlParameter("@RMV_QTY", strIPQTY)
                );

                var ps = string.IsNullOrWhiteSpace(txt_empno.Text) ? "admin" : txt_empno.Text.Trim();

                DbHelper.ExecuteNonQuery(
                    @"INSERT INTO JATR1230 (MV_NO, MV_DT, FMPS_GU, TMPS_GU, SA_CD, PS_CD, PL_CD, IN_DT, CL_DT, STTS, MV_GU,
                                        IP_NO, FA_CD, PR_DT, PR_NO)
                  VALUES (@MV_NO, GETDATE(), 'P002', 'W001', '01', @PS, NULL, GETDATE(), GETDATE(), 'C', '01',
                          @IP_NO, @FA_CD, @PR_DT, @PR_NO)",
                    new SqlParameter("@MV_NO", strMVNO),
                    new SqlParameter("@PS", ps),
                    new SqlParameter("@IP_NO", strIPNO),
                    new SqlParameter("@FA_CD", strFac),
                    new SqlParameter("@PR_DT", strIPDT),
                    new SqlParameter("@PR_NO", strIPNO)
                );

                
                DbHelper.ExecuteNonQuery("EXEC WI_PR1307_INSERT @MVNO, @DT",
                    new SqlParameter("@MVNO", strMVNO),
                    new SqlParameter("@DT", getDt)
                );

                var sql2 = @"
SELECT a.mv_no, b.mv_dt, b.pr_dt, b.ps_cd, ps_nm = ISNULL(c.emp_nm,B.PS_CD), e.pr_no,
       fa_cd = ISNULL(b.fa_cd,''), b.fmps_gu, b.tmps_gu, a.lot_no, a.gd_cd, d.gd_nm, d.spec, d.unit_cd, b.in_dt,
       a.rmv_qty, a.mv_qty, a.rmv_qty - a.mv_qty AS loss_qty, b.stts, b.in_dt, e.mate_no, a.rmv_qty,
       fmps_nm = (SELECT wh_nm FROM bcw100 WHERE use_yn = '1' AND wh_cd = b.fmps_gu),
       tmps_nm = (SELECT wh_nm FROM bcw100 WHERE use_yn = '1' AND wh_cd = b.tmps_gu)
FROM jatr1231 a
JOIN jatr1230 b ON a.mv_no = b.mv_no
LEFT JOIN hra100 c ON b.ps_cd = c.emp_no
LEFT JOIN comt1200 d ON a.gd_cd = d.gd_cd
LEFT JOIN PRTR1120 e ON b.pr_no = e.pr_no AND e.stts <> 'D'
WHERE b.stts <> 'D'
  AND b.stts = 'C'
  AND b.ip_no IS NOT NULL
  AND a.lot_no = @LOT
  AND a.mv_no  = @MV_NO
  AND b.ip_no  = @IP_NO
  AND b.pr_dt  = @PR_DT";
                var dt2 = DbHelper.ExecuteDataTable(sql2,
                    new SqlParameter("@LOT", strLotNo),
                    new SqlParameter("@MV_NO", strMVNO),
                    new SqlParameter("@IP_NO", strIPNO),
                    new SqlParameter("@PR_DT", strIPDT)
                );

                Sheet_Setting();
                grid_cnt++;
                strSEQ++;

                foreach (DataRow r in dt2.Rows)
                {
                    var row = 입고등록grid.Rows.Add();
                    입고등록grid.Rows[row].Cells[0].Value = strSEQ;
                    입고등록grid.Rows[row].Cells[1].Value = Convert.ToString(r["LOT_NO"]);
                    입고등록grid.Rows[row].Cells[2].Value = Convert.ToString(r["mate_no"]);
                    입고등록grid.Rows[row].Cells[3].Value = Convert.ToString(r["gd_cd"]);
                    입고등록grid.Rows[row].Cells[4].Value = Convert.ToString(r["gd_nm"]);
                    입고등록grid.Rows[row].Cells[5].Value = r["rmv_qty"];
                    입고등록grid.Rows[row].Cells[6].Value = r["mv_qty"];
                    입고등록grid.Rows[row].Cells[7].Value = Convert.ToString(r["tmps_nm"]);
                    입고등록grid.Rows[row].Cells[8].Value = Convert.ToString(r["fmps_nm"]);
                    입고등록grid.Rows[row].Cells[9].Value = r["in_dt"];
                    TryWriteSerial("[" + barcode + "][" + Convert.ToString(r["rmv_qty"]) + "]");
                }

                if (입고등록grid.Rows.Count > 0)
                    입고등록grid.FirstDisplayedScrollingRowIndex = 입고등록grid.Rows.Count - 1;

                txtChk.Clear();
                txtChk.Focus();

                int q = 0; int.TryParse(strIPQTY, out q);
                exp_qty_sub(strGDCD, q);
            }
            catch
            {
                try
                {
                    if (serialPortScanner.IsOpen)
                    {
                        serialPortScanner.DiscardInBuffer();
                        serialPortScanner.DiscardOutBuffer();
                    }
                }
                catch { }
            }
        }
        void TryWriteSerial(string s)
        {
            try { serialPortScanner.WriteLine(s); } catch { }
        }
        private void cmdExcel_Click(object sender, EventArgs e)
        {
            using (var dlg = new SaveFileDialog())
            {
                dlg.FileName = string.Empty;
                dlg.Filter = "Excel (*.xls)|*.xls";
                dlg.AddExtension = true;
                dlg.OverwritePrompt = true;
                if (dlg.ShowDialog(this) != DialogResult.OK) return;
                var path = dlg.FileName;
                if (!path.EndsWith(".xls", StringComparison.OrdinalIgnoreCase))
                    path += ".xls";
                try
                {
                    Cursor = Cursors.WaitCursor;
                    ExcelWriter.ExportDataGridView(입고등록grid, path);
                    Cursor = Cursors.Default;
                    MessageBox.Show($" [{path}] 로 저장이 되었습니다.", "입고등록 엑셀 저장",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    Cursor = Cursors.Default;
                    MessageBox.Show(
                        "오류번호 : " + ex.HResult + Environment.NewLine +
                        "오류내용 : " + ex.Message + Environment.NewLine + Environment.NewLine +
                        "혹시 해당이름의 파일이 열려있는지 확인하십시오!!" + Environment.NewLine +
                        "문제가 지속되면 개발자에게 문의하십시오",
                        "데이터베이스 연결 에러(cmdExcel_Click)",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void Mainform_Activated(object sender, EventArgs e)
        {
            
        }
        private void Sheet_Setting() // 입고등록grid 셋팅
        {
            var g = 입고등록grid;
            while (g.Columns.Count < 11) g.Columns.Add("C" + g.Columns.Count, "");
            int[] tw = { 650, 2000, 1000, 2400, 2000, 1000, 1000, 1000, 1000, 1800, 0 };

            try
            {
                for (int i = 0; i < tw.Length; i++)
                {
                    g.Columns[i].Visible = tw[i] != 0;

                    // 숨김 컬럼은 폭 의미 없음
                    if (tw[i] == 0) continue;

                    // 저장된 폭이 있으면 그걸 최우선 적용(리플리쉬 유지)
                    if (_inGridColWidth.TryGetValue(g.Columns[i].Name, out int savedW) && savedW > 0)
                        g.Columns[i].Width = savedW;
                    else
                        g.Columns[i].Width = TwipsToPx(tw[i]);
                }
            }
            finally
            {
                _inGridSuppressWidthEvents = false;
            }

            // 혹시 컬럼이 동적으로 추가/재생성되는 경우까지 커버
            ApplyInGridLayout();

            g.Columns[0].HeaderText = "순번";
            g.Columns[1].HeaderText = "CHECK SHEET NO.";
            g.Columns[2].HeaderText = "LOT NO.";
            g.Columns[3].HeaderText = "제품코드";
            g.Columns[4].HeaderText = "품명";
            g.Columns[5].HeaderText = "의뢰수량";
            g.Columns[6].HeaderText = "입고수량";
            g.Columns[7].HeaderText = "입고창고";
            g.Columns[8].HeaderText = "출고창고";
            g.Columns[9].HeaderText = "처리일시";

            var colRmv = g.Columns[5];
            colRmv.DefaultCellStyle.Format = "N0"; // 천단위 구분. 구분 없이 정수면 "0"
           
            colRmv.DefaultCellStyle.NullValue = "0";

            var colMv = g.Columns[6];
            colMv.DefaultCellStyle.Format = "N0";            
            colMv.DefaultCellStyle.NullValue = "0";


            for (int i = 0; i <= 8; i++) g.Columns[i].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            g.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            g.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            g.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            g.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            g.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            g.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            g.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            g.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            g.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            g.Columns[9].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            g.Columns[10].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            g.AutoGenerateColumns = false;
            g.AllowUserToAddRows = false;
            g.AllowUserToDeleteRows = false;
            g.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            g.RowHeadersVisible = false;
            g.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
        }

        private void txt_empno_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char)Keys.Enter) return;
            e.Handled = true;

            var empNo = txt_empno.Text?.Trim();
            if (string.IsNullOrEmpty(empNo))
            {
                txt_empno.Text = "admin";
                txt_empnm.Text = "admin";
                return;
            }

            var dt = DbHelper.ExecuteDataTable(
                "SELECT EMP_NO, EMP_NM FROM HRA100 WHERE EMP_NO = @NO",
                new SqlParameter("@NO", empNo)
            );

            if (dt.Rows.Count == 0)
            {
                txt_empno.Text = "admin";
                txt_empnm.Text = "admin";
            }
            else
            {
                txt_empno.Text = Convert.ToString(dt.Rows[0]["EMP_NO"]);
                txt_empnm.Text = Convert.ToString(dt.Rows[0]["EMP_NM"]);
            }
        }
        private void txtChk_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char)Keys.Enter) return;
            e.Handled = true;

            if (pnl_select.Visible)
            {
                MessageBox.Show("제품을 선택해주세요");
                return;
            }

            var SQL1 = @"
SELECT COUNT(A.MV_NO) AS CNT
FROM JATR1231 A
LEFT JOIN JATR1230 B ON A.MV_NO = B.MV_NO
where 1=1
  AND B.FMPS_GU = 'P002'
  AND B.TMPS_GU = 'W001'
  AND LOT_NO = @LOT";
            var dt1 = DbHelper.ExecuteDataTable(SQL1, new SqlParameter("@LOT", txtChk.Text.Trim()));
            if (dt1.Rows.Count > 0)
            {
                var cnt = Convert.ToInt32(dt1.Rows[0]["CNT"]);
                if (cnt > 0)
                {
                    lb_checksheet.BackColor = Color.Red;
                    txtChk.BackColor = Color.Red;
                    lb_Totalstr.Text = "※재고 이동이 등록된 CHECK SHEET NO.";
                    lb_Totalstr.ForeColor = Color.Red;
                    return;
                }
            }
            var SQL = @"
SELECT 
  CASE A.STTS WHEN 'C' THEN '0' ELSE '1' END AS CHK,
  A.PR_NO AS IP_NO, A.PR_DT AS IP_DT, A.NEW_GDCD AS GD_CD, B.GD_NM, B.SPEC, B.UNIT_CD,
  A.LOT_NO,
  IP_QTY =
    (CASE WHEN ISNULL(C.sample_whcd_yn,'0') = '1' 
          THEN A.PR_QTY-ISNULL(A.NEW_SAMPLE_QTY,0)-ISNULL(D.NEW_SAMPLE_QTY,0) 
          ELSE A.PR_QTY END),
  MV_QTY =
    (CASE WHEN ISNULL(C.sample_whcd_yn,'0') = '1' 
          THEN A.PR_QTY-ISNULL(A.NEW_SAMPLE_QTY,0)-ISNULL(D.NEW_SAMPLE_QTY,0) 
          ELSE A.PR_QTY END),
  LOSS_QTY = 0,
  FA_CD = CASE LEFT(A.LOT_NO,1) WHEN 'BBB' THEN '02' ELSE '03' END,
  TMPS_GU = CASE LEFT(A.LOT_NO,1) WHEN 'BBB' THEN 'P003' ELSE 'W001' END,
  FMPS_GU = A.TWH_CD,
  A.SA_CD,
  MV_DT = CONVERT(date, GETDATE()),
  MV_NO = dbo.fnCodeNo('WI_JA1420', GETDATE()),
  A.PR_NO,
  GET_DT = GETDATE()
FROM PRTR1120 A
LEFT JOIN COMT1200 B ON A.NEW_GDCD = B.GD_CD
LEFT JOIN COMT1630 C ON B.SALE_CD = C.NM_CD
LEFT JOIN PRTR1120 D ON A.LOT_NO = D.LOT_NO AND D.GONG_CD = '55' AND D.STTS = 'C'
WHERE A.STTS = 'C'
  AND A.GONG_CD = '80'
  AND ISNULL(A.MV_YN,'N') <> 'Y'
  AND NOT EXISTS (SELECT 1 FROM JATR1230 WHERE A.PR_NO = PR_NO AND STTS = 'C')
  AND A.LOT_NO = @LOT
ORDER BY A.PR_NO, A.PR_DT";
            var dt = DbHelper.ExecuteDataTable(SQL, new SqlParameter("@LOT", txtChk.Text.Trim()));

            if (dt.Rows.Count == 0)
            {
                lb_checksheet.BackColor = Color.Red;
                txtChk.BackColor = Color.Red;
                lb_Totalstr.Text = "※입고등록할 데이터없음";
                lb_Totalstr.ForeColor = Color.Red;

                for (int i = 0; i < 입고등록grid.Rows.Count; i++)
                {
                    var cell = 입고등록grid.Rows[i].Cells.Count > 1 ? 입고등록grid.Rows[i].Cells[1] : null;
                    if (cell == null) continue;
                    var v = Convert.ToString(cell.Value)?.Trim();
                    if (!string.IsNullOrEmpty(v) && string.Equals(v, txtChk.Text.Trim(), StringComparison.OrdinalIgnoreCase))
                    {
                        for (int c = 0; c <= 9 && c < 입고등록grid.Columns.Count; c++)
                        {
                            if(입고등록grid.Rows[i].Cells[c].Style.BackColor != Color.MistyRose)
                                입고등록grid.Rows[i].Cells[c].Style.BackColor = Color.Red;
                        }
                        lb_Totalstr.Text = "※이미 등록된 CHECK SHEET NO.";
                    }
                }
                return;
            }

            lb_checksheet.BackColor = Color.MistyRose;
            txtChk.BackColor = SystemColors.Window;
            lb_Totalstr.Text = "";
            lb_Totalstr.ForeColor = SystemColors.ControlText;

            var r0 = dt.Rows[0];
            var strLotNo = Convert.ToString(r0["LOT_NO"]);
            var strGDCD = Convert.ToString(r0["GD_CD"]);
            var strIPQTY = Convert.ToString(r0["IP_QTY"]);
            var strMVNO = Convert.ToString(r0["MV_NO"]);
            var strIPNO = Convert.ToString(r0["IP_NO"]);
            var strIPDT = Convert.ToString(r0["IP_DT"]);
            strFac = Convert.ToString(r0["FA_CD"]);

            DbHelper.ExecuteNonQuery(
                @"INSERT INTO JATR1231 (MV_NO, MV_SEQ, GD_CD, MV_QTY, LOT_NO, RMV_QTY, LOSS_QTY, MPS_GU)
              VALUES (@MV_NO, 1, @GD_CD, @MV_QTY, @LOT_NO, @RMV_QTY, 0, 'W001')",
                new SqlParameter("@MV_NO", strMVNO),
                new SqlParameter("@GD_CD", strGDCD),
                new SqlParameter("@MV_QTY", strIPQTY),
                new SqlParameter("@LOT_NO", strLotNo),
                new SqlParameter("@RMV_QTY", strIPQTY)
            );

            var ps = string.IsNullOrWhiteSpace(txt_empno.Text) ? "admin" : txt_empno.Text.Trim();
            
            DateTime prDt = DateTime.Parse(strIPDT);

            DbHelper.ExecuteNonQuery(
                @"INSERT INTO JATR1230 (MV_NO, MV_DT, FMPS_GU, TMPS_GU, SA_CD, PS_CD, PL_CD, IN_DT, CL_DT, STTS, MV_GU,
                                    IP_NO, FA_CD, PR_DT, PR_NO)
              VALUES (@MV_NO, GETDATE(), 'P002', 'W001', '01', @PS, NULL, GETDATE(), GETDATE(), 'C', '01',
                      @IP_NO, @FA_CD, @PR_DT, @PR_NO)",
                new SqlParameter("@MV_NO", strMVNO),
                new SqlParameter("@PS", ps),
                new SqlParameter("@IP_NO", strIPNO),
                new SqlParameter("@FA_CD", strFac),
                new SqlParameter("@PR_DT", prDt),
                new SqlParameter("@PR_NO", strIPNO)
            );

            string datenow = DateTime.Now.ToString("yyyy-MM-dd");
            DbHelper.ExecuteNonQuery("EXEC WI_PR1307_INSERT @MVNO, @DT",
                new SqlParameter("@MVNO", strMVNO),
                new SqlParameter("@DT", datenow)
            );

            var SQL2 = @"
SELECT a.mv_no, b.mv_dt, b.pr_dt, b.ps_cd, ps_nm = ISNULL(c.emp_nm,B.PS_CD), e.pr_no,
       fa_cd = ISNULL(b.fa_cd,''), b.fmps_gu, b.tmps_gu, a.lot_no, a.gd_cd, d.gd_nm, d.spec, d.unit_cd, b.in_dt,
       a.rmv_qty, a.mv_qty, a.rmv_qty - a.mv_qty AS loss_qty, b.stts, b.in_dt, e.mate_no, a.rmv_qty,
       fmps_nm = (SELECT wh_nm FROM bcw100 WHERE use_yn = '1' AND wh_cd = b.fmps_gu),
       tmps_nm = (SELECT wh_nm FROM bcw100 WHERE use_yn = '1' AND wh_cd = b.tmps_gu)
FROM jatr1231 a
JOIN jatr1230 b ON a.mv_no = b.mv_no
LEFT JOIN hra100 c ON b.ps_cd = c.emp_no
LEFT JOIN comt1200 d ON a.gd_cd = d.gd_cd
LEFT JOIN PRTR1120 e ON b.pr_no = e.pr_no AND e.stts <> 'D'
WHERE b.stts <> 'D'
  AND b.stts = 'C'
  AND b.ip_no IS NOT NULL
  AND a.lot_no = @LOT
  AND a.mv_no  = @MV_NO
  AND b.ip_no  = @IP_NO
  AND b.pr_dt  = @PR_DT";
            var dt2 = DbHelper.ExecuteDataTable(SQL2,
                new SqlParameter("@LOT", strLotNo),
                new SqlParameter("@MV_NO", strMVNO),
                new SqlParameter("@IP_NO", strIPNO),
                new SqlParameter("@PR_DT", prDt)
            );

            Sheet_Setting();
            grid_cnt++;
            strSEQ++;

            foreach (DataRow r in dt2.Rows)
            {
                입고등록grid.Rows.Insert(0, new object[]
                    {
                        strSEQ,
                        Convert.ToString(r["LOT_NO"]),
                        Convert.ToString(r["mate_no"]),
                        Convert.ToString(r["gd_cd"]),
                        Convert.ToString(r["gd_nm"]),
                        (r["rmv_qty"] == DBNull.Value) ? 0 : (int)Math.Truncate(Convert.ToDecimal(r["rmv_qty"])),
                        (r["mv_qty"] == DBNull.Value) ? 0 : (int)Math.Truncate(Convert.ToDecimal(r["mv_qty"])),
                        Convert.ToString(r["tmps_nm"]),
                        Convert.ToString(r["fmps_nm"]),
                        r["in_dt"] // DateTime 또는 문자열 그대로
                    });

                input_total_qty += (r["mv_qty"] == DBNull.Value) ? 0 : (int)Math.Truncate(Convert.ToDecimal(r["mv_qty"]));


            }

            if (입고등록grid.Rows.Count > 0)
            {
                입고등록grid.ClearSelection();
                //입고등록grid.Rows[0].Selected = true;
                입고등록grid.FirstDisplayedScrollingRowIndex = 0;
            }
            
            lb_incountstr.Text = $"※총 입고수량 : {input_total_qty} 개";           

            int qty = (int)decimal.Truncate(decimal.Parse(strIPQTY, NumberStyles.Number, CultureInfo.InvariantCulture));
            if (exp_qty_sub(strGDCD, qty))
            {
                for (int c = 0; c <= 9 && c < 입고등록grid.Columns.Count; c++)
                    입고등록grid.Rows[0].Cells[c].Style.BackColor = Color.MistyRose;
            }

            txtChk.Clear();
            txtChk.Focus();
        }

        private bool exp_qty_sub(string gd_cd, int qty)
        {
            //var wavPath = PathCombine(AppDomain.CurrentDomain.BaseDirectory, "Error.wav");
            //using (var alert = new Form4())
            //{
            //    alert.ShowAlert("AA0892", 1000, 770, 230, wavPath);
            //}

            var dt = DbHelper.ExecuteDataTable(
                "SELECT * FROM isuf_stock WHERE qty > 0 AND gd_cd = @gd",
                new SqlParameter("@gd", gd_cd)
            );

            if (dt.Rows.Count == 0) return false;

            if (dt.Rows.Count > 1)
            {
                gd_cd_ = gd_cd;
                exp_qty_ = qty;

                order_num_return = "";
                gd_cd_return = "";
                exp_qty_return = 0;

                Sheet_Setting_sel();

                grid_view_sel();    
                
                show_pnl_select();
                return false;
            }
            else
            {
                var r = dt.Rows[0];

                //MessageBox.Show($"부족분 List : 수주번호({r["order_num"]}) / 제품코드 ({r["gd_cd"]}) / 수량 ({r["qty"]}) 중 {qty} 개 차감합니다.");
                string s = r["order_num"].ToString();
                s = s?.Trim() ?? "";
                int p = s.IndexOf(' ');
                string orderNo = (p >= 0) ? s.Substring(0, p) : s;
                showAlertForm(orderNo, r["qty"].ToString(),qty.ToString());


                DbHelper.ExecuteNonQuery(
                    "UPDATE isuf_stock SET qty = qty - @q WHERE gd_cd = @gd",
                    new SqlParameter("@q", qty),
                    new SqlParameter("@gd", gd_cd)
                );

                return true;
            }
        }
        
        private void grid_view_sel() // 입고등록grid 셋팅
        {
            var g = fpExcel;
            g.DataSource = null;
            g.Rows.Clear();

            while (g.Columns.Count < 3) g.Columns.Add("C" + g.Columns.Count, "");

            var dt = DbHelper.ExecuteDataTable(
                "SELECT order_num, gd_cd, qty FROM isuf_stock WHERE qty > 0 AND gd_cd = @gd",
                new SqlParameter("@gd", (object)(gd_cd_ ?? string.Empty))
            );

            foreach (DataRow r in dt.Rows)
            {
                int rowIndex = g.Rows.Add();
                g.Rows[rowIndex].Cells[0].Value = r["order_num"];
                g.Rows[rowIndex].Cells[1].Value = r["gd_cd"];
                g.Rows[rowIndex].Cells[2].Value = r["qty"];
            }
        }
        private void Sheet_Setting_sel() // 입고등록grid 셋팅
        {
            var g = fpExcel;
            while (g.Columns.Count < 11) g.Columns.Add("C" + g.Columns.Count, "");

            int[] twips = { 1650, 2000, 1000, 0, 0, 0, 0, 0, 0, 0, 0 };
            for (int i = 0; i < twips.Length; i++)
            {
                g.Columns[i].Width = TwipsToPx(twips[i]);
                g.Columns[i].Visible = twips[i] != 0;
            }

            g.Columns[0].HeaderText = "수주번호";
            g.Columns[1].HeaderText = "제품코드";
            g.Columns[2].HeaderText = "수량";
            for (int i = 3; i <= 10; i++) g.Columns[i].HeaderText = string.Empty;

            for (int i = 0; i <= 2; i++)
                g.Columns[i].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            g.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            g.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            g.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            g.AutoGenerateColumns = false;
            g.AllowUserToAddRows = false;
            g.AllowUserToDeleteRows = false;
            g.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            g.RowHeadersVisible = false;
        }

        private void showAlertForm(string ordernm, string required, string receive)
        {
            var f = new AlertForm();
            string appDir = Application.StartupPath;                // exe가 있는 폴더
            string wavPath = Path.Combine(appDir, "Error.wav");
            
            long req_val = long.Parse(required);
            long rec_val = long.Parse(receive);
            long diff_val = req_val - rec_val;

            f.ShowAlert(ordernm, req_val, rec_val, diff_val, wavPath);
        }

        private void label2_Click(object sender, EventArgs e)
        {
            showAlertForm("AA0092", "100", "50");
        }
    }
}
