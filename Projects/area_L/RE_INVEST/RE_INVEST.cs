using area_L.Modules;
using DevExpress.Export;
using DevExpress.Utils;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.UI;
using DevExpress.XtraSplashScreen;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace area_L
{
    public partial class RE_INVEST : Form
    {
        public static string dbName = "ERP_2";

        MSSQL db = new MSSQL(dbName);

        CommonModule cm = new CommonModule();

        private System.Data.DataTable empDt = null;

        public Action<string> UpdateStatus { get; set; }

        private readonly BindingList<ScanItem> _dataList = new BindingList<ScanItem>();
        private readonly BindingList<ScanItem> _totdataList = new BindingList<ScanItem>();

        private readonly BindingSource _bs = new BindingSource();
        private readonly BindingSource _totbs = new BindingSource();

        private int _dragStartRowHandle = GridControl.InvalidRowHandle;
        private bool _isDragging = false;

        public RE_INVEST()
        {
            InitializeComponent();

            InitControl();

            DbHelper.ConnectionString = "Data Source=192.168.2.5;Initial Catalog=ERP_2;User ID=interojo;Password=DB@$2022!;MultipleActiveResultSets=True;TrustServerCertificate=True";

            _dataList.AllowNew = true;
            _totdataList.AllowRemove = true;
            _totdataList.AllowNew = true;

            _totbs.DataSource = _totdataList;
            _bs.DataSource = _dataList;          // BindingSource에 연결

            gridControl1.DataSource = _bs;       // GridControl에 바인딩

            gridView1.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Bottom;
            gridView1.FocusedRowChanged += gridView1_FocusedRowChanged;

            gridView1.OptionsSelection.MultiSelectMode = GridMultiSelectMode.RowSelect;

            // 우클릭 메뉴 훅킹
            gridView1.PopupMenuShowing += GridView1_PopupMenuShowing;

            gridControl2.DataSource = _totdataList;
            gridView2.OptionsSelection.MultiSelectMode = GridMultiSelectMode.RowSelect;

            // 편집 막고 행 단위 선택
            //gridView1.OptionsBehavior.Editable = false;
            //gridView1.OptionsSelection.MultiSelect = true;
            //gridView1.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.RowSelect;
            //gridView1.OptionsSelection.EnableAppearanceFocusedCell = false;
            //gridView1.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFullFocus;

            //// ★ 여기만 남기고, OptionsClipboard.* 설정은 전부 제거
            //gridView1.KeyDown -= gridView1_KeyDown_PasteBarcodes;
            //gridView1.KeyDown += gridView1_KeyDown_PasteBarcodes;

        }

        private void InitControl()
        {
            ps_cd.Focus();
            ps_nm.Text = "<- 사번입력";

            fn_GetData();

            ps_cd.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) fn_GetPsNM(); };

            rt_no.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter && !rt_no.ReadOnly) fn_GetRtData_BufferOnly(); };



        }
        private void fn_GetData()
        {
            try
            {
                string strSql = $"{dbName}.dbo.ST_RE_INS_GET_EMPDATA";

                db.ExecuteSql(strSql);

                if (db.nState)
                {
                    if (db.result.Rows.Count > 0)
                    {
                        empDt = db.result;
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void fn_GetPsNM()
        {
            try
            {
                DataRow[] dr = empDt.Select($"EMP_NO = '{ps_cd.Text}'");

                if (dr.Length > 0)
                {
                    ps_nm.Text = dr[0]["EMP_NM"].ToString();

                    ps_cd.Text = ps_cd.Text.Trim();

                    rt_no.Focus();

                    if(ps_cd.Text == "180305")
                    {
                        ManagerMode(true);
                    }
                    else
                    {
                        ManagerMode(false);
                    }
                    string strSql = $"{dbName}.dbo.RE_HOUSE_SCAN_SEL";

                    db.ExecuteSql(strSql);

                    if (db.nState)
                    {
                        if (db.result == null)
                            return;

                        _totdataList.Clear();

                        foreach (DataRow row in db.result.Rows)
                        {
                            var item = new ScanItem
                            {
                                Barcode = row["Barcode"]?.ToString(),
                                Gtin = row["GTIN"]?.ToString(),
                                Gdcd = row["GDCD"]?.ToString(),
                                Qyt = row["QYT"]?.ToString(),
                                Mate = row["MATE"]?.ToString(),
                                Exprir = row["EXPRIR"]?.ToString(),   // "yyyy-MM-dd" 변환은 ScanItem에서 처리
                                Lotno = row["LOTNO"]?.ToString()
                            };
                            _totdataList.Add(item);
                        }
                        lb_totcount.Text = "전체 스캔 갯수 : " + _totdataList.Count.ToString();
                        InitDataControl();
                    }
                }
                else
                {
                    MessageBox.Show("사번이 존재하지 않습니다.\n확인부탁드립니다.", "사번조회", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    ps_nm.Text = string.Empty;

                    ps_cd.Focus();

                    ps_cd.SelectAll();

                    ps_nm.Text = "<- 사번입력";

                    return;
                }
            }
            catch (Exception ex)
            {

            }
        }
        private void gridView1_KeyDown_PasteBarcodes(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.V)
            {
                PasteBarcodesFromClipboardToBuffer();
                e.Handled = true;   // 기본 처리 막기
            }
        }

        // 클립보드의 엑셀 한 행 → 바코드 리스트 → fn_GetRtData_BufferOnly 재사용
        private void PasteBarcodesFromClipboardToBuffer()
        {
            if (!Clipboard.ContainsText())
                return;

            string text = Clipboard.GetText();
            if (string.IsNullOrWhiteSpace(text))
                return;

            // 탭 / 개행 / 콤마 / 세미콜론을 모두 구분자로 보고 바코드 추출
            string normalized = text.Replace("\r\n", "\n");
            var barcodes = normalized
                .Split(new[] { '\n', '\t', ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim())
                .Where(s => s.Length > 0)
                .ToList();

            if (barcodes.Count == 0)
                return;

            // 너무 많이 붙일 때 사용자에게 한 번 물어보는 것도 좋음
            if (barcodes.Count > 200)
            {
                if (MessageBox.Show(
                        $"{barcodes.Count}건을 한 번에 조회합니다. 계속하시겠습니까?",
                        "대량 조회",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question) != DialogResult.Yes)
                    return;
            }

            // 중복 바코드 제거 (이미 _dataList에 있는 것도 스킵)
            var distinct = barcodes
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Where(code => !_dataList.Any(x => x.Barcode == code))
                .ToList();

            if (distinct.Count == 0)
                return;

            // 그리드/바인딩 이벤트 묶기
            var view = gridView1;
            view.BeginDataUpdate();
            var oldRaise = _dataList.RaiseListChangedEvents;
            _dataList.RaiseListChangedEvents = false;

            try
            {
                int total = distinct.Count;
                int processed = 0;

                foreach (var code in distinct)
                {
                    rt_no.Text = code;
                    fn_GetRtData_BufferOnly();   // 기존 로직 그대로 재사용

                    processed++;

                    // 50건마다 한 번씩 진행 상황 표시 + UI 메시지 처리
                    if (processed % 50 == 0 || processed == total)
                    {
                        UpdateStatus?.Invoke($"붙여넣기 처리 중... {processed}/{total}"); //lb_Status2.Text = $"붙여넣기 처리 중... {processed}/{total}";

                        System.Windows.Forms.Application.DoEvents();  // 메시지 루프 처리 → "멈춘 것처럼" 보이지 않게
                    }
                }
                lb_scancount.Text = "스캔 갯수 : " + _dataList.Count.ToString();
                
                UpdateStatus?.Invoke($"붙여넣기 완료 ({processed}/{total})"); //lb_Status2.Text = $"붙여넣기 완료 ({processed}/{total})";
            }
            catch (Exception ex)
            {
                MessageBox.Show("붙여넣기 처리 중 오류가 발생했습니다.\n" + ex.Message,
                                "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // 바인딩 이벤트/그리드 갱신 재개
                _dataList.RaiseListChangedEvents = oldRaise;
                _dataList.ResetBindings();
                view.EndDataUpdate();
            }
        }


        private void ManagerMode(bool isManager)
        {
            if (isManager)
            {
                btn_excelexport.Visible = true;
                btn_gdcdexport.Visible= true;
                ConfigureRowSelection(gridView2, true);
                EnableDragRowSelection(gridView2);                
                gridView2.PopupMenuShowing += GridView2_PopupMenuShowing_Delete;               
                gridControl2.KeyDown += GridDeleteKeyHandler;
            }
            else
            {
                btn_excelexport.Visible = false;
                btn_gdcdexport.Visible = false;
                ConfigureRowSelection(gridView2, false);
                gridView2.PopupMenuShowing -= GridView2_PopupMenuShowing_Delete;
                gridControl2.KeyDown -= GridDeleteKeyHandler;
            }
        }
        private void fn_GetRtData_BufferOnly()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(ps_cd.Text))
                {
                    MessageBox.Show("승인자 ERP사번을 입력해주세요. 입력후 ENTER", "승인자 입력",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    ps_cd.Focus();
                    ps_nm.Text = "<- 사번입력";
                    return;
                }

                rt_no.Text = rt_no.Text.Trim();
                if (string.IsNullOrWhiteSpace(rt_no.Text))
                {
                    rt_no.Focus();
                    return;
                }
               
                if (rt_no.Text.Length >= 20)
                {
                    bool duplicate = _dataList.Any(x =>
       string.Equals(x.Barcode?.Trim(), rt_no.Text, StringComparison.Ordinal));

                    if (duplicate)
                    {
                        MessageBox.Show("동일한 UDI(Barcode)가 이미 스캔 목록에 있습니다.", "중복 항목",
                                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        rt_no.SelectAll();
                        rt_no.Focus();
                        return; // 이후 로직(추가/업로드 등) 중단
                    }
                    
                        duplicate = _totdataList.Any(x =>
       string.Equals(x.Barcode?.Trim(), rt_no.Text, StringComparison.Ordinal));

                    if (duplicate)
                    {
                        MessageBox.Show("동일한 UDI(Barcode)가 이미 전체 조사 목록에 있습니다.", "중복 항목",
                                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        rt_no.SelectAll();
                        rt_no.Focus();
                        return; // 이후 로직(추가/업로드 등) 중단
                    }

                }

                // 1) 정보 조회(있으면 채우고, 없으면 사용자 입력 폼)               
                ScanItem item = null;
                string gtin = string.Empty;
                string mate_no = string.Empty;
                string exprir = string.Empty;
                string lot21 = string.Empty;
                string lottoudi = string.Empty;
                string err = null;
                                
                if (rt_no.Text.Length >= 20)
                {
                    //UDI 바코드 인 경우
                    if (TryParseUdi_Fixed_01_10_17_21(rt_no.Text, out gtin, out mate_no, out exprir, out lot21, out err))
                    {
                        exprir = ExpToDateString(exprir);
                        var dtLot = DbHelper.ExecuteDataTable(    "SELECT dbo.fn_Udi21_ToLotString(@UDI) AS LotStr",    new SqlParameter("@UDI", rt_no.Text));

                        if (dtLot.Rows.Count > 0)
                        {
                            lottoudi = dtLot.Rows[0]["LotStr"].ToString();
                        }
                    }
                }
                
                if (err != null)
                {
                    MessageBox.Show(err);
                    return;
                }
                else
                {
                    string strSql = $"{dbName}.dbo.ST_RE_INVEST_SEL";
                    db.Parameter("@Param_scan_item", rt_no.Text);
                    db.Parameter("@pGTIN", gtin);
                    db.Parameter("@pLOT10", mate_no);
                    db.Parameter("@pExpDate", exprir);
                    db.Parameter("@pLOT21", lottoudi);
                    db.ExecuteSql(strSql);
                    if (db.nState)
                    {
                        if (db.result == null || db.result.Rows.Count == 0)
                        {
                            if (rt_no.Text.Length >= 20)
                            {
                                var dt = DbHelper.ExecuteDataTable("SELECT TOP 1  A.GD_CD_2, B.GD_NM FROM UDMT1010 AS A LEFT JOIN dbo.COMT1200 AS B  ON B.GD_CD = A.GD_CD_2 WHERE A.GTIN_NO = @GTIN", new SqlParameter("@GTIN", gtin));
                                string gdcdnm = string.Empty;
                                string gdnm = string.Empty;
                                if (dt.Rows.Count != 0)
                                {
                                    gdcdnm = dt.Rows[0]["GD_CD_2"].ToString();
                                    gdnm = dt.Rows[0]["GD_NM"].ToString();
                                }

                                item = new ScanItem { Barcode = rt_no.Text, Gtin = gtin, Mate = mate_no, Gdcd = gdcdnm, Lotno = mate_no.ToUpper(), Gdnm = gdnm, Exprir= exprir, Qyt = "1" };

                                if (gdcdnm == string.Empty)
                                {
                                    using (RE_INSERT_FORM insertForm = new RE_INSERT_FORM(item))
                                    {
                                        if (insertForm.ShowDialog() == DialogResult.OK)
                                        {
                                            item.Lotno = insertForm.mate_no.ToUpper();
                                            item.Gdcd = insertForm.gd_cd;
                                            item.Mate = insertForm.mate_no.ToUpper();
                                            // Exprir은 insertForm에서 item.Exprir 세팅되었다고 가정(yyyy-MM-dd 형태)
                                        }
                                        else
                                        {
                                            rt_no.Clear();
                                            rt_no.Focus();
                                            return;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                //물류 바코드 인 경우
                                item = new ScanItem { Barcode = rt_no.Text, Gtin = rt_no.Text, Qyt = "1" };

                                using (RE_INSERT_FORM insertForm = new RE_INSERT_FORM(item))
                                {
                                    if (insertForm.ShowDialog() == DialogResult.OK)
                                    {
                                        item.Lotno = insertForm.mate_no.ToUpper();
                                        item.Gdcd = insertForm.gd_cd;
                                        item.Mate = insertForm.mate_no.ToUpper();
                                        // Exprir은 insertForm에서 item.Exprir 세팅되었다고 가정(yyyy-MM-dd 형태)
                                    }
                                    else
                                    {
                                        rt_no.Clear();
                                        rt_no.Focus();
                                        return;
                                    }
                                }
                            }
                        }
                        else
                        {
                            // 조회 있음 → ScanItem 구성

                            var r = db.result.Rows[0];
                            item = new ScanItem
                            {
                                Barcode = rt_no.Text,
                                Gtin = r["GTIN"]?.ToString(),
                                Gdcd = r["GD_CD_2"]?.ToString(),
                                Qyt = "1",
                                Mate = r["MATE"]?.ToString().ToUpper(),
                                Exprir = r["EXPIR"]?.ToString(), // ScanItem에서 yyyy-MM-dd로 정규화됨
                                Lotno = r["LOTNO"]?.ToString(),
                                Gdnm = r["GD_NM"]?.ToString()
                            };

                            // 필수 값(예: Mate)이 없으면 사용자 입력 받기
                            if (string.IsNullOrWhiteSpace(item.Mate) || string.IsNullOrWhiteSpace(item.Lotno))
                            {
                                using (RE_INSERT_FORM insertForm = new RE_INSERT_FORM(item))
                                {
                                    if (insertForm.ShowDialog() == DialogResult.OK)
                                    {
                                        item.Lotno = insertForm.mate_no.ToUpper();
                                        item.Gdcd = insertForm.gd_cd;
                                        item.Qyt = "1";
                                        item.Mate = insertForm.mate_no.ToUpper();
                                    }
                                    else
                                    {
                                        rt_no.Clear();
                                        rt_no.Focus();
                                        return;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("조회 중 오류가 발생했습니다.");
                        return;
                    }
                }
                // 3) 버퍼에 추가 (그리드 자동 갱신)
                gridView1.BeginDataUpdate();
                try
                {

                    _dataList.Insert(0, item); // 0번째(맨 위)에 삽입
                }
                finally
                {
                    gridView1.EndDataUpdate();
                }

                // 화면 포커스/스크롤 맨 위로
                gridView1.FocusedRowHandle = 0;
                gridView1.MakeRowVisible(0, true);


                // 4) UI 업데이트
                lb_scancount.Text = "스캔 갯수 : " + _dataList.Count;
                SetDataControl(item.Barcode, item.Gtin, item.Qyt, item.Mate, item.Exprir, item.Lotno, item.Gdcd, item.Gdnm);

                // 5) 입력창 리셋
                rt_no.Clear();
                rt_no.Focus();
            }
            catch
            {
                // 필요 시 로깅
            }
        }
        private void SetDataControl(string barcode, string gtin, string qyt, string mate, string exprir, string lotno, string gdcd, string gdnm)
        {
            tb_barcode.Text = barcode;
            tb_gdcd.Text = gdcd;
            tb_gtin.Text = gtin;
            tb_gdnm.Text = qyt;
            tb_mate.Text = mate;
            tb_expir.Text = exprir;
            tb_lotno.Text = lotno;
            tb_gdnm.Text = gdnm; 
        }
        private void InitDataControl()
        {
            tb_barcode.Text = string.Empty;
            tb_gdcd.Text = string.Empty;
            tb_gtin.Text = string.Empty;
            tb_gdnm.Text = string.Empty;
            tb_mate.Text = string.Empty;
            tb_expir.Text = string.Empty;
            tb_lotno.Text = string.Empty;

            var col = gridView2.Columns["Gdnm"];   // FieldName 기준 (대소문자 주의)
            if (col != null)
            {
                col.Visible = false;               // 숨김
                col.VisibleIndex = -1;             // 혹시 모를 위치 고정
                col.OptionsColumn.ShowInCustomizationForm = false; // 사용자 커스터마이즈 창에서도 감춤(옵션)
            }
        }

        private void btn_initdata_Click(object sender, EventArgs e)
        {
            InitDataControl();
        }

        private void gridView1_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            // 현재 선택된 행의 데이터 가져오기
            var item = gridView1.GetFocusedRow() as ScanItem;

            if (item != null)
            {
                SetDataControl(item.Barcode, item.Gtin, item.Qyt, item.Mate, item.Exprir, item.Lotno, item.Gdcd, item.Gdnm);
            }
        }

        private void GridView1_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            if (e.MenuType != GridMenuType.Row) return; // 행 영역에서만 메뉴 노출

            var view = (GridView)sender;

            // 우클릭 위치의 행으로 포커스/선택 동기화(실수 방지)
            if (e.HitInfo.RowHandle >= 0 && !view.IsGroupRow(e.HitInfo.RowHandle))
            {
                view.FocusedRowHandle = e.HitInfo.RowHandle;
                if (!view.IsRowSelected(e.HitInfo.RowHandle))
                {
                    view.ClearSelection();
                    view.SelectRow(e.HitInfo.RowHandle);
                }
            }

            // 기본 메뉴를 모두 지우고 커스텀 메뉴만 사용(원하면 주석 처리해서 기본 메뉴 유지 가능)
            e.Menu.Items.Clear();

            // 삭제 메뉴 항목
            var miDelete = new DXMenuItem("삭제", (o, args) => DeleteSelectedRows(view))
            {
                // 아이콘이 필요하면 SVG 리소스를 연결하여 사용 가능
                // ImageOptions = { SvgImage = DevExpress.Utils.Svg.SvgImage.FromResources("YourNamespace.Resources.trash.svg", typeof(MainForm).Assembly) }
            };

            e.Menu.Items.Add(miDelete);
        }
        private void DeleteSelectedRows(GridView view)
        {
            // 선택된 행 핸들 수집(그룹/신규행 제외), 없으면 포커스 행 시도
            var selected = view.GetSelectedRows()
                               .Where(h => h >= 0 && !view.IsGroupRow(h) && !view.IsNewItemRow(h))
                               .Distinct()
                               .OrderByDescending(h => h)
                               .ToArray();

            if (selected.Length == 0 && view.FocusedRowHandle >= 0 && !view.IsGroupRow(view.FocusedRowHandle) && !view.IsNewItemRow(view.FocusedRowHandle))
                selected = new[] { view.FocusedRowHandle };

            if (selected.Length == 0) return; // 삭제할 실행 대상 없음

            // 확인 다이얼로그
            var message = (selected.Length == 1) ? "선택된 행을 삭제하시겠습니까?" : $"선택된 {selected.Length}개 행을 삭제하시겠습니까?";
            if (XtraMessageBox.Show(message, "삭제 확인", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            view.BeginDataUpdate();
            try
            {
                // 역순 삭제(핸들 무효화 방지)
                foreach (var handle in selected)
                {
                    view.DeleteRow(handle); // 데이터 소스(DataTable/BindingList 등)에서 제거 또는 삭제 표시

                    lb_scancount.Text = "스캔 갯수 : " + _dataList.Count;
                    InitDataControl();
                }
            }
            finally
            {
                view.EndDataUpdate();
            }

            // 참고:
            // - DataTable 바인딩 시: DeleteRow → DataRow.RowState = Deleted (어댑터 사용 시 Update 호출로 DB 반영)
            // - BindingList<T> 바인딩 시: 항목 제거

        }

        private void btn_upload_Click(object sender, EventArgs e)
        {
            if (_dataList.Count == 0)
            {
                MessageBox.Show("업로드할 데이터가 없습니다.");
                return;
            }

            btnUpload.Enabled = false; // 중복 클릭 방지
            try
            {
                string connStr = "Server=192.168.2.5;Database=ERP_2;UID = interojo; PWD = DB@$2022!;";

                using (var conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    using (var tran = conn.BeginTransaction())
                    {
                        try
                        {
                            foreach (var it in _dataList)
                            {
                                using (var cmd = new SqlCommand($"{dbName}.dbo.RE_HOUSE_SCAN_INS", conn, tran))
                                {
                                    cmd.CommandType = CommandType.StoredProcedure;

                                    // Exprir: yyyy-MM-dd 문자열 → DATE 변환
                                    DateTime expr;
                                    object exprParam = DBNull.Value;
                                    if (!string.IsNullOrWhiteSpace(it.Exprir) &&
                                        DateTime.TryParse(it.Exprir, out expr))
                                    {
                                        exprParam = expr.Date;
                                    }

                                    // Qyt: string → INT/DECIMAL 변환(프로시저가 NVARCHAR 받으면 그대로 전달 가능)
                                    // 여기서는 NVARCHAR로 넘기되, 비어있으면 "1" 기본값 예시
                                    string qtyStr = string.IsNullOrWhiteSpace(it.Qyt) ? "1" : it.Qyt;

                                    cmd.Parameters.AddWithValue("@Barcode", it.Barcode ?? (object)DBNull.Value);
                                    cmd.Parameters.AddWithValue("@Gtin", (object)(it.Gtin ?? (object)DBNull.Value));
                                    cmd.Parameters.AddWithValue("@Gdcd", (object)(it.Gdcd ?? (object)DBNull.Value));
                                    cmd.Parameters.AddWithValue("@Qyt", (object)qtyStr);
                                    cmd.Parameters.AddWithValue("@Mate", (object)(it.Mate ?? (object)DBNull.Value));
                                    cmd.Parameters.AddWithValue("@Exprir", exprParam);
                                    cmd.Parameters.AddWithValue("@Lotno", (object)(it.Lotno ?? (object)DBNull.Value));

                                    cmd.ExecuteNonQuery();
                                }
                            }

                            tran.Commit();
                        }
                        catch (Exception ex)
                        {
                            tran.Rollback();
                            MessageBox.Show("업로드 중 오류가 발생했습니다.\r\n" + ex.Message);
                            return;
                        }
                    }
                }

                // 성공 시 버퍼 비우고 UI 갱신
                gridView2.BeginUpdate();

                foreach (var item in _dataList)
                {
                    var copy = new ScanItem
                    {
                        Barcode = item.Barcode,
                        Gtin = item.Gtin,
                        Gdcd = item.Gdcd,
                        Qyt = item.Qyt,
                        Mate = item.Mate,
                        Exprir = item.Exprir,
                        Lotno = item.Lotno
                    };

                    _totdataList.Add(copy);
                }
                gridView2.EndUpdate();

                _dataList.Clear();
                lb_scancount.Text = "스캔 갯수 : 0";
                lb_totcount.Text = "전체 스캔된 갯수 : " + _totdataList.Count;
                InitDataControl();
                
                MessageBox.Show("업로드가 완료되었습니다.");
            }
            finally
            {
                btnUpload.Enabled = true;
            }
        }

        public static bool TryParseUdi_Fixed_01_10_17_21(
    string raw,
    out string gtin14,     // (01) 14자리 숫자
    out string lot4,       // (10) 4자리(멸균번호)
    out string expYYMMDD,  // (17) 6자리 숫자
    out string lot21,      // (21) 나머지 전체
    out string error)
        {
            gtin14 = null; lot4 = null; expYYMMDD = null; lot21 = null; error = null;
            if (string.IsNullOrWhiteSpace(raw)) { error = "입력이 비어 있습니다."; return false; }

            // 공백 제거 + 괄호 표기 정규화
            string s = new string(raw.Where(c => !char.IsWhiteSpace(c)).ToArray());
            s = s.Replace("(01)", "01").Replace("(10)", "10").Replace("(17)", "17").Replace("(21)", "21");
            const char GS = (char)29;

            int i = 0;

            // ---- (01) + 14자리 숫자 ----
            int p01 = s.IndexOf("01", i, StringComparison.Ordinal);
            if (p01 < 0) { error = "(01) 토큰을 찾지 못했습니다."; return false; }
            int start01 = p01 + 2;
            if (start01 + 14 > s.Length) { error = "(01) GTIN 길이가 부족합니다(14)."; return false; }

            string gtinCandidate = s.Substring(start01, 14);
            if (!gtinCandidate.All(char.IsDigit))
            {
                error = "(01) GTIN은 14자리 숫자여야 합니다.";
                return false;
            }
            gtin14 = gtinCandidate;
            i = start01 + 14;

            // ---- (10) + 4자리(고정, 앞의 첫 (10)만 인정) ----
            int p10 = s.IndexOf("10", i, StringComparison.Ordinal);
            if (p10 < 0) { error = "(10) 토큰을 찾지 못했습니다."; return false; }
            int start10 = p10 + 2;
            if (start10 + 4 > s.Length) { error = "(10) 멸균번호 길이가 부족합니다(4)."; return false; }
            // GS 포함 불가
            for (int k = 0; k < 4; k++)
                if (s[start10 + k] == GS) { error = "(10) 멸균번호에 GS 구분자가 포함되었습니다."; return false; }
            lot4 = s.Substring(start10, 4);
            i = start10 + 4;

            // ---- (17) + 6자리 숫자 ----
            int p17 = s.IndexOf("17", i, StringComparison.Ordinal);
            int start17 = i;
            if (p17 < 0)
            {
                //error = "(17) 토큰을 찾지 못했습니다.";
                //error = "";
                int testtemp = 0;
            }
            else
            {
                start17 = p17 + 2;
                if (start17 + 6 > s.Length) { error = "(17) 사용기간 길이가 부족합니다(6)."; return false; }

                string expCandidate = s.Substring(start17, 6);
                if (!expCandidate.All(char.IsDigit))
                {
                    error = "(17) 사용기간은 YYMMDD 6자리 숫자여야 합니다.";
                    return false;
                }
                expYYMMDD = expCandidate;
                i = start17 + 6;
            }
            // ---- (21) + 끝까지 ----
            int p21 = s.IndexOf("21", i, StringComparison.Ordinal);
            if (p21 < 0) { error = "(21) 토큰을 찾지 못했습니다."; return false; }
            int start21 = p21 + 2;
            int gsPos = s.IndexOf(GS, start21);
            lot21 = (gsPos < 0) ? s.Substring(start21) : s.Substring(start21, gsPos - start21);
            if (lot21.Length == 0) { error = "(21) LOTNO+추가번호가 비어 있습니다."; return false; }

            return true;
        }


        string ExpToDateString(string yymmdd)
        {
            if (string.IsNullOrEmpty(yymmdd) || yymmdd.Length != 6 || !yymmdd.All(char.IsDigit)) return null;
            int yy = int.Parse(yymmdd.Substring(0, 2));
            int mm = int.Parse(yymmdd.Substring(2, 2));
            int dd = int.Parse(yymmdd.Substring(4, 2));
            return new DateTime(2000 + yy, mm, dd).ToString("yyyy-MM-dd");
        }

        private void btn_excelexport_Click(object sender, EventArgs e)
        {
            var view = gridControl2.MainView as GridView;
            if (view == null || view.RowCount == 0)
            {
                MessageBox.Show("엑셀로 내보낼 데이터가 없습니다.");
                return;
            }

            using (var sfd = new SaveFileDialog
            {
                Title = "엑셀로 내보내기",
                Filter = "Excel Workbook (*.xlsx)|*.xlsx",
                FileName = $"Export_{DateTime.Now:yyyyMMdd_HHmm}.xlsx",
                OverwritePrompt = true
            })
            {
                if (sfd.ShowDialog() != DialogResult.OK) return;

                // 출력/레이아웃 옵션(필요 시 조정)
                view.OptionsPrint.AutoWidth = false;   // 열 너비 고정(가로 스크롤 유지)
                view.OptionsPrint.PrintHeader = true;
                // view.BestFitColumns();  // 컬럼 너비 자동 맞춤을 원하면 사용

                var opt = new XlsxExportOptionsEx
                {
                    // DataAware : 엑셀 친화(필터/정렬 가능, 숫자/날짜 타입 유지)
                    // WYSIWYG  : 화면 모양 그대로(머지/스타일 포함)
                    ExportType = ExportType.DataAware,
                    TextExportMode = TextExportMode.Value, // 숫자/날짜 그대로
                    AllowGrouping = DefaultBoolean.True,                    
                    ShowGridLines = true,
                    SheetName = "GridData",
                    // 선택 행만 내보내려면:
                    // ExportSelectedRowsOnly = true
                };

                // 실제 내보내기 (필터/정렬/그룹 상태를 반영)
                view.ExportToXlsx(sfd.FileName, opt);

                if (MessageBox.Show("내보내기가 완료되었습니다. 파일을 여시겠습니까?",
                                    "Export", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    System.Diagnostics.Process.Start(sfd.FileName);
                }
            }
        }
        
        private void btn_gdcdexport_Click(object sender, EventArgs e)
        {
            // 1) 서버 집계: GDCD별 QYT 합계
            var dtAgg = DbHelper.ExecuteDataTable(@"
        SELECT 
            Gdcd AS Gdcd,
            SUM(TRY_CONVERT(decimal(18,4), Qyt)) AS QytSum
        FROM dbo.RE_HOUSE_SCAN
        GROUP BY Gdcd
        ORDER BY Gdcd;
    ");

            if (dtAgg == null || dtAgg.Rows.Count == 0)
            {
                MessageBox.Show("내보낼 집계 데이터가 없습니다.");
                return;
            }

            // 2) 파일 저장 다이얼로그
            string filePath;
            using (var sfd = new SaveFileDialog
            {
                Title = "엑셀로 내보내기 (품목별 합계)",
                Filter = "Excel Workbook (*.xlsx)|*.xlsx",
                FileName = $"GDCD_Qty_Sum_{DateTime.Now:yyyyMMdd_HHmm}.xlsx",
                OverwritePrompt = true
            })
            {
                if (sfd.ShowDialog() != DialogResult.OK) return;
                filePath = sfd.FileName;
            }

            // 3) 화면 그리드와 무관한 임시 Grid에서만 Export
            using (var tempGrid = new GridControl())
            using (var tempView = new GridView(tempGrid))
            {
                tempGrid.MainView = tempView;
                tempGrid.ViewCollection.Add(tempView);

                tempGrid.BindingContext = new BindingContext();

                tempGrid.DataSource = dtAgg;

                tempGrid.ForceInitialize();
                tempView.PopulateColumns(dtAgg);

                // 컬럼 서식(엑셀에서도 숫자 타입 유지)
                var colQtySum = tempView.Columns["QytSum"] ?? tempView.Columns["QYTSUM"];
                if (colQtySum != null)
                {
                    colQtySum.Caption = "수량 합계";
                    colQtySum.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                    colQtySum.DisplayFormat.FormatString = "0";
                }
                var colGdcd = tempView.Columns["Gdcd"] ?? tempView.Columns["GDCD"];
                if (colGdcd != null) colGdcd.Caption = "품명";

                // 4) Excel Export (DataAware: 필터/정렬/숫자형 유지)
                var opt = new XlsxExportOptionsEx
                {
                    ExportType = ExportType.DataAware,
                    TextExportMode = TextExportMode.Value,
                    SheetName = "GDCD_Summary",
                    AllowGrouping = DevExpress.Utils.DefaultBoolean.False,
                    ShowGridLines = true
                };

                tempView.ExportToXlsx(filePath, opt);
            }

            // 5) 저장 후 열기 확인
            if (MessageBox.Show("내보내기가 완료되었습니다. 파일을 여시겠습니까?",
                                "Export", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
            {
                try { Process.Start(filePath); } catch { /* 필요 시 로깅 */ }
            }
        }
        private void ConfigureRowSelection(GridView view, bool multiSelect)
        {
            if (view == null) return;

            view.BeginUpdate();
            try
            {
                // 편집 완전 차단
                view.OptionsBehavior.Editable = false;
                view.OptionsBehavior.EditorShowMode = EditorShowMode.MouseDown; // 혹시 모를 에디터 오픈도 최소화

                // 셀 포커스 표시 제거 → 행 전체가 하이라이트
                view.OptionsSelection.EnableAppearanceFocusedCell = false;
                view.FocusRectStyle = DrawFocusRectStyle.RowFullFocus;

                // 선택 모드: 단일/다중 행
                view.OptionsSelection.MultiSelect = multiSelect;
                view.OptionsSelection.MultiSelectMode = GridMultiSelectMode.RowSelect;

                // Ctrl+A 전체 선택 핸들러: 다중일 때만 활성
                view.GridControl.KeyDown -= GridCtrlA_SelectAll;
                if (multiSelect)
                    view.GridControl.KeyDown += GridCtrlA_SelectAll;
            }
            finally
            {
                view.EndUpdate();
            }
        }

        private void GridCtrlA_SelectAll(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.A)
            {
                var gv = (sender as DevExpress.XtraGrid.GridControl)?.MainView as GridView;
                if (gv != null && gv.OptionsSelection.MultiSelect)
                {
                    gv.SelectAll();
                    e.Handled = true;
                }
            }
        }
        private void EnableDragRowSelection(GridView view)
        {
            // 중복 연결 방지
            view.MouseDown -= GridView_MouseDown_DragSelect;
            view.MouseMove -= GridView_MouseMove_DragSelect;
            view.MouseUp -= GridView_MouseUp_DragSelect;

            view.MouseDown += GridView_MouseDown_DragSelect;
            view.MouseMove += GridView_MouseMove_DragSelect;
            view.MouseUp += GridView_MouseUp_DragSelect;
        }

        private void GridView_MouseDown_DragSelect(object sender, MouseEventArgs e)
        {
            var view = sender as GridView;
            if (view == null || !view.OptionsSelection.MultiSelect) return;

            var hit = view.CalcHitInfo(e.Location);
            if (!hit.InRow) return;

            _isDragging = (e.Button == MouseButtons.Left);
            _dragStartRowHandle = hit.RowHandle;

            // 기본 클릭 선택 동작: Ctrl 미사용이라면 기존 선택 초기화 후 시작행만 선택
            if (_isDragging)
            {
                view.BeginSelection();
                try
                {
                    if (!ModifierKeys.HasFlag(Keys.Control))
                        view.ClearSelection();
                    view.SelectRow(_dragStartRowHandle);
                    view.FocusedRowHandle = _dragStartRowHandle;
                }
                finally
                {
                    view.EndSelection();
                }
            }
        }

        private void GridView_MouseMove_DragSelect(object sender, MouseEventArgs e)
        {
            if (!_isDragging) return;

            var view = sender as GridView;
            if (view == null || !view.OptionsSelection.MultiSelect) return;

            var hit = view.CalcHitInfo(e.Location);
            if (!hit.InRow) return;

            int current = hit.RowHandle;
            if (current == GridControl.InvalidRowHandle || _dragStartRowHandle == GridControl.InvalidRowHandle) return;

            int from = System.Math.Min(_dragStartRowHandle, current);
            int to = System.Math.Max(_dragStartRowHandle, current);

            view.BeginSelection();
            try
            {
                if (!ModifierKeys.HasFlag(Keys.Control))
                    view.ClearSelection();

                // 연속 범위 선택
                for (int r = from; r <= to; r++)
                    view.SelectRow(r);

                view.FocusedRowHandle = current;
            }
            finally
            {
                view.EndSelection();
            }
        }

        private void GridView_MouseUp_DragSelect(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            _isDragging = false;
            // 끝난 뒤 시작지점 초기화(선택은 유지)
            //_dragStartRowHandle = GridControl.InvalidRowHandle; // 필요시 주석 해제
        }
        
        private void GridView2_PopupMenuShowing_Delete(object sender, PopupMenuShowingEventArgs e)
        {
            if (e.MenuType != GridMenuType.Row) return;

            var view = (GridView)sender;

            // 우클릭한 위치의 행을 포커스/선택으로 맞춰줌(다중선택 유지: Ctrl 누르면 추가 선택)
            var hit = view.CalcHitInfo(e.Point);
            if (hit.InRow && !view.IsRowSelected(hit.RowHandle) && !ModifierKeys.HasFlag(Keys.Control))
            {
                view.ClearSelection();
                view.FocusedRowHandle = hit.RowHandle;
                view.SelectRow(hit.RowHandle);
            }

            // 메뉴 구성
            e.Menu.Items.Clear();

            e.Menu.Items.Add(new DXMenuItem(
                $"선택 행 삭제 ({view.SelectedRowsCount}개)",
                (o, args) => DeleteSelectedRows2(view)
            )
            { BeginGroup = true });
        }
        private void GridDeleteKeyHandler(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Delete) return;
            var view = gridControl2.MainView as GridView;
            if (view == null) return;

            DeleteSelectedRows2(view);
            e.Handled = true;
        }
        private void DeleteSelectedRows2(GridView view)
        {
            if (view == null || view.SelectedRowsCount <= 0) return;

            var barcodeCol = view.Columns
                .Cast<DevExpress.XtraGrid.Columns.GridColumn>()
                .FirstOrDefault(c => string.Equals(c.FieldName, "Barcode", StringComparison.OrdinalIgnoreCase));
            if (barcodeCol == null) { MessageBox.Show("Barcode 컬럼을 찾을 수 없습니다."); return; }

            var handles = view.GetSelectedRows();
            var barcodes = handles
                .Select(h => Convert.ToString(view.GetRowCellValue(h, barcodeCol))?.Trim())
                .Where(s => !string.IsNullOrEmpty(s))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
            if (barcodes.Count == 0) { MessageBox.Show("삭제할 Barcode가 없습니다."); return; }

            if (MessageBox.Show($"{barcodes.Count}개의 Barcode를 삭제하시겠습니까?",
                                "삭제 확인", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            try
            {
                // XML로 패킹(STRING_SPLIT 대체)
                string xml = "<x>" + string.Join("</x><x>",
                                  barcodes.Select(b => System.Security.SecurityElement.Escape(b))) + "</x>";

                var dt = DbHelper.ExecuteDataTable(@"
;WITH S AS (
    SELECT T.X.value('.','nvarchar(100)') AS Barcode
    FROM (SELECT CAST(@Xml AS xml) AS XmlData) AS A
    CROSS APPLY A.XmlData.nodes('/x') AS T(X)
)
DELETE H
FROM dbo.RE_HOUSE_SCAN AS H
JOIN S ON LTRIM(RTRIM(H.Barcode)) = LTRIM(RTRIM(S.Barcode));
SELECT @@ROWCOUNT AS RowsAffected;",
                    new SqlParameter("@Xml", xml));

                int affected = (dt != null && dt.Rows.Count > 0 && dt.Columns.Contains("RowsAffected"))
                    ? Convert.ToInt32(dt.Rows[0]["RowsAffected"])
                    : 0;

                // UI에서도 제거
                view.BeginUpdate();
                try
                {
                    foreach (var h in handles.OrderByDescending(x => x))
                        view.DeleteRow(h);
                }
                finally { view.EndUpdate(); }

                MessageBox.Show($"{affected}건 삭제됨");
            }
            catch (Exception ex)
            {
                MessageBox.Show("삭제 중 오류가 발생했습니다.\n" + ex.Message,
                                "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
