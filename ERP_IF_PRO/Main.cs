using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Navigation;
using DevExpress.XtraTab;
using DevExpress.XtraEditors;
using ERP_IF_PRO.Modules;

namespace ERP_IF_PRO
{
    public partial class Main : DevExpress.XtraEditors.XtraForm
    {
        // 테마 색상 상수
        private static readonly Color PrimaryBlue = Color.FromArgb(27, 80, 145);
        private static readonly Color DarkBlue = Color.FromArgb(18, 55, 100);
        private static readonly Color AccentOrange = Color.FromArgb(245, 130, 32);

        CommonModule cm = new CommonModule();
        FTPModule ftp = new FTPModule();
        DataTable dtMenu;

        // 관리자 여부 (LoginForm에서 설정)
        public bool IsAdmin { get; set; }

        // 현재 선택된 상위 메뉴 코드
        private string currentParentMenuCd = string.Empty;
        // 사이드 메뉴 토글 버튼 (클래스 레벨)
        private BarButtonItem btnToggleSideMenu;
        // 메뉴 검색 버튼 (클래스 레벨)
        private BarButtonItem btnSearchMenu;
        // panelHeader 원래 높이 (PANEL_YN 제어용)
        private int panelHeaderOriginalHeight = 0;

        public Main()
        {
            InitializeComponent();
            ApplyTheme();
            this.Load += Main_Load;
            this.timerClock.Tick += TimerClock_Tick;
            this.xtraTabControl.CloseButtonClick += XtraTabControl_CloseButtonClick;
            this.xtraTabControl.SelectedPageChanged += XtraTabControl_SelectedPageChanged;
            this.Resize += Main_Resize;
            this.FormClosing += Main_FormClosing;

            // 사이드 메뉴 토글 버튼 객체 미리 생성
            btnToggleSideMenu = null; // BuildTopMenu()에서 추가됨
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "프로그램을 종료하시겠습니까?",
                "종료 확인",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
            {
                e.Cancel = true;
            }
        }

        // ══════════════════════════════════════
        // 테마/디자인 적용 (디자이너에서 분리)
        // ══════════════════════════════════════
        private void ApplyTheme()
        {
            // Header
            panelHeader.Appearance.BackColor = PrimaryBlue;
            panelHeader.Appearance.Options.UseBackColor = true;

            lblLogo.Appearance.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblLogo.Appearance.ForeColor = Color.White;
            lblLogo.Appearance.Options.UseFont = true;
            lblLogo.Appearance.Options.UseForeColor = true;

            lblTitle.Appearance.Font = new Font("Segoe UI", 11F);
            lblTitle.Appearance.ForeColor = Color.FromArgb(180, 200, 230);
            lblTitle.Appearance.Options.UseFont = true;
            lblTitle.Appearance.Options.UseForeColor = true;

            // Bar Menu
            barMainMenu.Appearance.Font = new Font("맑은 고딕", 10F, FontStyle.Bold);
            barMainMenu.Appearance.Options.UseFont = true;
            barMainMenu.Appearance.BackColor = DarkBlue;
            barMainMenu.Appearance.Options.UseBackColor = true;
            barMainMenu.Appearance.ForeColor = Color.White;
            barMainMenu.Appearance.Options.UseForeColor = true;

            // Accordion (Side Menu)
            accordionMenu.Appearance.AccordionControl.BackColor = Color.FromArgb(235, 240, 248);
            accordionMenu.Appearance.AccordionControl.Options.UseBackColor = true;
            accordionMenu.Appearance.Group.Default.Font = new Font("맑은 고딕", 10F, FontStyle.Bold);
            accordionMenu.Appearance.Group.Default.Options.UseFont = true;
            accordionMenu.Appearance.Group.Default.ForeColor = PrimaryBlue;
            accordionMenu.Appearance.Group.Default.Options.UseForeColor = true;
            accordionMenu.Appearance.Item.Default.Font = new Font("맑은 고딕", 9.5F);
            accordionMenu.Appearance.Item.Default.Options.UseFont = true;
            accordionMenu.Appearance.Item.Hovered.BackColor = Color.FromArgb(210, 225, 245);
            accordionMenu.Appearance.Item.Hovered.Options.UseBackColor = true;
            accordionMenu.Appearance.Item.Pressed.BackColor = AccentOrange;
            accordionMenu.Appearance.Item.Pressed.ForeColor = Color.White;
            accordionMenu.Appearance.Item.Pressed.Options.UseBackColor = true;
            accordionMenu.Appearance.Item.Pressed.Options.UseForeColor = true;

            // Tab Control
            xtraTabControl.ClosePageButtonShowMode = DevExpress.XtraTab.ClosePageButtonShowMode.InAllTabPageHeaders;
            xtraTabControl.AppearancePage.Header.Font = new Font("맑은 고딕", 9.5F);
            xtraTabControl.AppearancePage.Header.Options.UseFont = true;
            xtraTabControl.AppearancePage.HeaderActive.Font = new Font("맑은 고딕", 9.5F, FontStyle.Bold);
            xtraTabControl.AppearancePage.HeaderActive.ForeColor = PrimaryBlue;
            xtraTabControl.AppearancePage.HeaderActive.Options.UseFont = true;
            xtraTabControl.AppearancePage.HeaderActive.Options.UseForeColor = true;

            // Status Bar
            panelStatus.Appearance.BackColor = DarkBlue;
            panelStatus.Appearance.Options.UseBackColor = true;

            lblStatus.Appearance.Font = new Font("맑은 고딕", 9F);
            lblStatus.Appearance.ForeColor = Color.White;
            lblStatus.Appearance.Options.UseFont = true;
            lblStatus.Appearance.Options.UseForeColor = true;

            lblUser.Appearance.Font = new Font("맑은 고딕", 9F);
            lblUser.Appearance.ForeColor = Color.FromArgb(180, 200, 230);
            lblUser.Appearance.Options.UseFont = true;
            lblUser.Appearance.Options.UseForeColor = true;

            lblIP.Appearance.Font = new Font("맑은 고딕", 9F);
            lblIP.Appearance.ForeColor = Color.FromArgb(180, 200, 230);
            lblIP.Appearance.Options.UseFont = true;
            lblIP.Appearance.Options.UseForeColor = true;

            lblDateTime.Appearance.Font = new Font("맑은 고딕", 9F);
            lblDateTime.Appearance.ForeColor = AccentOrange;
            lblDateTime.Appearance.Options.UseFont = true;
            lblDateTime.Appearance.Options.UseForeColor = true;
        }

        // ══════════════════════════════════════
        // 사이드 메뉴 토글 버튼
        // ══════════════════════════════════════
        private void AddSideMenuToggleButton()
        {
            if (btnToggleSideMenu == null)
            {
                btnToggleSideMenu = new BarButtonItem();
                btnToggleSideMenu.Caption = "☰ 메뉴";
                btnToggleSideMenu.Alignment = BarItemLinkAlignment.Right;
                btnToggleSideMenu.ItemAppearance.Normal.Font = new Font("맑은 고딕", 10F, FontStyle.Bold);
                btnToggleSideMenu.ItemAppearance.Normal.Options.UseFont = true;
                btnToggleSideMenu.ItemAppearance.Normal.ForeColor = Color.White;
                btnToggleSideMenu.ItemAppearance.Normal.Options.UseForeColor = true;
                btnToggleSideMenu.ItemAppearance.Hovered.BackColor = AccentOrange;
                btnToggleSideMenu.ItemAppearance.Hovered.ForeColor = Color.White;
                btnToggleSideMenu.ItemAppearance.Hovered.Options.UseBackColor = true;
                btnToggleSideMenu.ItemAppearance.Hovered.Options.UseForeColor = true;
                btnToggleSideMenu.ItemClick += BtnToggleSideMenu_Click;
            }

            btnToggleSideMenu.Id = barManager.GetNewItemId();
            barManager.Items.Add(btnToggleSideMenu);
            barMainMenu.ItemLinks.Add(btnToggleSideMenu);
        }

        private void BtnToggleSideMenu_Click(object sender, ItemClickEventArgs e)
        {
            if (splitContainer.PanelVisibility == SplitPanelVisibility.Both)
                splitContainer.PanelVisibility = SplitPanelVisibility.Panel2;
            else
                splitContainer.PanelVisibility = SplitPanelVisibility.Both;
        }

        // ══════════════════════════════════════
        // 메뉴 검색 버튼
        // ══════════════════════════════════════
        private void AddSearchMenuButton()
        {
            if (btnSearchMenu == null)
            {
                btnSearchMenu = new BarButtonItem();
                btnSearchMenu.Caption = "🔍 메뉴검색";
                btnSearchMenu.Alignment = BarItemLinkAlignment.Right;
                btnSearchMenu.ItemAppearance.Normal.Font = new Font("맑은 고딕", 10F, FontStyle.Bold);
                btnSearchMenu.ItemAppearance.Normal.Options.UseFont = true;
                btnSearchMenu.ItemAppearance.Normal.ForeColor = Color.White;
                btnSearchMenu.ItemAppearance.Normal.Options.UseForeColor = true;
                btnSearchMenu.ItemAppearance.Hovered.BackColor = AccentOrange;
                btnSearchMenu.ItemAppearance.Hovered.ForeColor = Color.White;
                btnSearchMenu.ItemAppearance.Hovered.Options.UseBackColor = true;
                btnSearchMenu.ItemAppearance.Hovered.Options.UseForeColor = true;
                btnSearchMenu.ItemClick += BtnSearchMenu_Click;
            }

            btnSearchMenu.Id = barManager.GetNewItemId();
            barManager.Items.Add(btnSearchMenu);
            barMainMenu.ItemLinks.Add(btnSearchMenu);
        }

        private void BtnSearchMenu_Click(object sender, ItemClickEventArgs e)
        {
            ShowMenuSearchPopup();
        }

        /// <summary>
        /// 메뉴 검색 팝업 (실시간 자동완성 리스트)
        /// </summary>
        private void ShowMenuSearchPopup()
        {
            // 검색 대상 메뉴 목록 구성 (최상위 메뉴 제외, FORM_NAME이 있는 것만)
            var menuList = new System.Collections.Generic.List<string>();
            if (dtMenu != null)
            {
                DataRow[] allMenus = dtMenu.Select("P_ID IS NOT NULL AND USE_FLAG = 'Y'", "MENU_NAME");

                foreach (DataRow row in allMenus)
                {
                    // FORM_NAME이 없는 메뉴(그룹 메뉴)는 제외
                    string formName = row["FORM_NAME"] == DBNull.Value ? "" : row["FORM_NAME"].ToString().Trim();
                    if (string.IsNullOrEmpty(formName)) continue;

                    // 하위 메뉴가 있는 메뉴(중간 노드)는 제외
                    string menuId = row["ID"].ToString();
                    DataRow[] children = dtMenu.Select($"P_ID = {menuId}");
                    if (children.Length > 0) continue;

                    // 일반 사용자: 자신 또는 상위 메뉴가 ADMIN_YN='Y'이면 제외
                    if (!IsAdmin)
                    {
                        if (row["ADMIN_YN"].ToString() == "Y") continue;
                        if (IsParentAdminOnly(row["P_ID"].ToString())) continue;
                    }

                    string menuName = row["MENU_NAME"].ToString();
                    if (!string.IsNullOrEmpty(menuName) && !menuList.Contains(menuName))
                    {
                        menuList.Add(menuName);
                    }
                }
            }

            using (XtraForm searchForm = new XtraForm())
            {
                searchForm.Text = "메뉴 검색";
                searchForm.Size = new Size(450, 400);
                searchForm.StartPosition = FormStartPosition.CenterParent;
                searchForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                searchForm.MaximizeBox = false;
                searchForm.MinimizeBox = false;
                searchForm.Appearance.BackColor = Color.White;
                searchForm.Appearance.Options.UseBackColor = true;

                // 검색 아이콘 + 입력창 패널
                PanelControl pnlSearch = new PanelControl();
                pnlSearch.Location = new Point(15, 15);
                pnlSearch.Size = new Size(405, 36);
                pnlSearch.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
                pnlSearch.Appearance.BackColor = Color.FromArgb(245, 245, 245);
                pnlSearch.Appearance.Options.UseBackColor = true;

                LabelControl lblIcon = new LabelControl();
                lblIcon.Text = "🔍";
                lblIcon.Location = new Point(8, 7);
                lblIcon.Appearance.Font = new Font("Segoe UI", 12F);
                lblIcon.Appearance.Options.UseFont = true;

                TextEdit txtSearch = new TextEdit();
                txtSearch.Location = new Point(35, 5);
                txtSearch.Size = new Size(360, 26);
                txtSearch.Properties.Appearance.Font = new Font("맑은 고딕", 11F);
                txtSearch.Properties.Appearance.Options.UseFont = true;
                txtSearch.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
                txtSearch.Properties.NullValuePrompt = "메뉴명을 입력하세요...";
                txtSearch.Properties.NullValuePromptShowForEmptyValue = true;

                pnlSearch.Controls.Add(lblIcon);
                pnlSearch.Controls.Add(txtSearch);

                // 자동완성 결과 리스트
                ListBoxControl lstResults = new ListBoxControl();
                lstResults.Location = new Point(15, 58);
                lstResults.Size = new Size(405, 280);
                lstResults.Appearance.Font = new Font("맑은 고딕", 10F);
                lstResults.Appearance.Options.UseFont = true;
                lstResults.HotTrackItems = true;

                // 초기에 전체 목록 표시
                foreach (string menu in menuList)
                {
                    lstResults.Items.Add(menu);
                }

                // 안내 라벨 (검색 결과 없을 때)
                LabelControl lblNoResult = new LabelControl();
                lblNoResult.Text = "검색 결과가 없습니다.";
                lblNoResult.Location = new Point(150, 180);
                lblNoResult.Appearance.Font = new Font("맑은 고딕", 10F);
                lblNoResult.Appearance.ForeColor = Color.Gray;
                lblNoResult.Appearance.Options.UseFont = true;
                lblNoResult.Appearance.Options.UseForeColor = true;
                lblNoResult.Visible = false;

                // 하단 결과 수 표시
                LabelControl lblCount = new LabelControl();
                lblCount.Location = new Point(15, 343);
                lblCount.Appearance.Font = new Font("맑은 고딕", 8.5F);
                lblCount.Appearance.ForeColor = Color.Gray;
                lblCount.Appearance.Options.UseFont = true;
                lblCount.Appearance.Options.UseForeColor = true;
                lblCount.Text = $"총 {menuList.Count}개 메뉴";

                searchForm.Controls.AddRange(new Control[] { pnlSearch, lstResults, lblNoResult, lblCount });

                // 실시간 필터링
                txtSearch.EditValueChanged += (s, ev) =>
                {
                    string keyword = txtSearch.Text.Trim().ToLower();
                    lstResults.Items.Clear();

                    if (string.IsNullOrEmpty(keyword))
                    {
                        foreach (string menu in menuList)
                            lstResults.Items.Add(menu);
                    }
                    else
                    {
                        foreach (string menu in menuList)
                        {
                            if (menu.ToLower().Contains(keyword))
                                lstResults.Items.Add(menu);
                        }
                    }

                    lblNoResult.Visible = lstResults.Items.Count == 0;
                    lblCount.Text = $"검색 결과: {lstResults.Items.Count}개";
                };

                // 리스트 더블클릭 → 열기
                lstResults.DoubleClick += (s, ev) =>
                {
                    if (lstResults.SelectedItem != null)
                    {
                        searchForm.Tag = lstResults.SelectedItem.ToString();
                        searchForm.DialogResult = DialogResult.OK;
                    }
                };

                // Enter키 처리
                txtSearch.KeyDown += (s, ev) =>
                {
                    if (ev.KeyCode == Keys.Enter)
                    {
                        // 리스트에서 선택된 항목 또는 첫번째 항목 열기
                        if (lstResults.SelectedItem != null)
                        {
                            searchForm.Tag = lstResults.SelectedItem.ToString();
                            searchForm.DialogResult = DialogResult.OK;
                        }
                        else if (lstResults.Items.Count > 0)
                        {
                            searchForm.Tag = lstResults.Items[0].ToString();
                            searchForm.DialogResult = DialogResult.OK;
                        }
                        ev.Handled = true;
                        ev.SuppressKeyPress = true;
                    }
                    else if (ev.KeyCode == Keys.Down)
                    {
                        // 아래 화살표 → 리스트로 포커스 이동
                        if (lstResults.Items.Count > 0)
                        {
                            lstResults.Focus();
                            lstResults.SelectedIndex = 0;
                        }
                        ev.Handled = true;
                    }
                };

                // 리스트에서 Enter키
                lstResults.KeyDown += (s, ev) =>
                {
                    if (ev.KeyCode == Keys.Enter && lstResults.SelectedItem != null)
                    {
                        searchForm.Tag = lstResults.SelectedItem.ToString();
                        searchForm.DialogResult = DialogResult.OK;
                        ev.Handled = true;
                    }
                    else if (ev.KeyCode == Keys.Back || ev.KeyCode == Keys.Escape)
                    {
                        // 백스페이스나 ESC → 검색창으로 포커스 복귀
                        txtSearch.Focus();
                        if (ev.KeyCode == Keys.Escape)
                        {
                            searchForm.DialogResult = DialogResult.Cancel;
                        }
                        ev.Handled = true;
                    }
                };

                // 폼 열릴 때 검색창에 포커스
                searchForm.Shown += (s, ev) => txtSearch.Focus();

                if (searchForm.ShowDialog(this) == DialogResult.OK)
                {
                    string selectedMenu = searchForm.Tag?.ToString();
                    if (!string.IsNullOrEmpty(selectedMenu))
                    {
                        OpenMenuByName(selectedMenu);
                    }
                }
            }
        }

        /// <summary>
        /// 메뉴명으로 폼 열기
        /// </summary>
        private void OpenMenuByName(string menuName)
        {
            try
            {
                if (dtMenu == null) return;

                // MENU_NAME으로 메뉴 행 검색
                string filter = IsAdmin
                    ? $"MENU_NAME = '{menuName.Replace("'", "''")}' AND FORM_NAME IS NOT NULL AND FORM_NAME <> ''"
                    : $"MENU_NAME = '{menuName.Replace("'", "''")}' AND FORM_NAME IS NOT NULL AND FORM_NAME <> '' AND ADMIN_YN = 'N'";

                DataRow[] foundRows = dtMenu.Select(filter);

                if (foundRows.Length == 0)
                {
                    MessageBox.Show($"'{menuName}' 메뉴를 찾을 수 없습니다.", "알림",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                DataRow row = foundRows[0];
                string formName = row["FORM_NAME"].ToString();
                string panelYn = dtMenu.Columns.Contains("PANEL_YN") ? row["PANEL_YN"].ToString() : "Y";
                string passwordYn = dtMenu.Columns.Contains("PASSWORD_YN") ? row["PASSWORD_YN"].ToString() : "N";
                string password = dtMenu.Columns.Contains("PASSWORD") ? row["PASSWORD"].ToString() : "";
                string dockOrNot = dtMenu.Columns.Contains("DOCK_OR_NOT") ? row["DOCK_OR_NOT"].ToString() : "Y";

                // 비밀번호 체크
                if (passwordYn == "Y" && !string.IsNullOrEmpty(password))
                {
                    bool alreadyOpen = false;
                    foreach (XtraTabPage page in xtraTabControl.TabPages)
                    {
                        if (page.Name == formName) { alreadyOpen = true; break; }
                    }

                    if (!alreadyOpen && !CheckMenuPassword(menuName, password))
                    {
                        return;
                    }
                }

                LoadFormToTab(formName, menuName, panelYn, dockOrNot);
                splitContainer.PanelVisibility = SplitPanelVisibility.Panel2;
            }
            catch (Exception ex)
            {
                cm.writeLog($"OpenMenuByName Error: {ex.Message}");
            }
        }

        // ══════════════════════════════════════
        // Form Load
        // ══════════════════════════════════════
        private void Main_Load(object sender, EventArgs e)
        {
            try
            {
                // panelHeader 원래 높이 저장
                panelHeaderOriginalHeight = panelHeader.Height;

                // DLL 캐시 정리 (이전 세션의 잔여 파일)
                ftp.CleanCache();

                // 상태바 정보 설정
                lblUser.Text = IsAdmin ? "사용자: Admin" : "사용자: 일반";
                lblIP.Text = $"IP: {cm.GetLocalIPAddress()}";
                lblDateTime.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                lblStatus.Text = "메뉴 로딩 중...";

                // DB에서 메뉴 데이터 로드
                LoadMenuData();

                // 상위 메뉴 (ToolMenuStrip) 구성
                BuildTopMenu();

                // 초기 안내 탭 추가
                AddWelcomeTab();

                lblStatus.Text = "Ready";

                // 상태바 위치 재배치
                RepositionStatusLabels();
            }
            catch (Exception ex)
            {
                cm.writeLog($"Main_Load Error: {ex.Message}");
                lblStatus.Text = "초기화 오류 발생";
            }
        }

        // ══════════════════════════════════════
        // 메뉴 데이터 로드 (MSSQL)
        // ══════════════════════════════════════
        private void LoadMenuData()
        {
            try
            {
                // 1. MSSQL 객체 선언
                MSSQL db = new MSSQL("ERP_2");

                // 2. strSql 지정
                string strSql = "ERP_2.dbo.ST_NEW_MENU_SEL";

                // 3. 파라미터 설정 (필요시 db.Parameter("매개변수명", "값"))
                db.Parameter("@GUBUN", "COMBI");

                // 4. SELECT 후 ExecuteSql 실행
                db.ExecuteSql(strSql);

                // 5. db.result에 결과값
                if (db.result != null && db.result.Rows.Count > 0)
                {
                    dtMenu = db.result;
                }
                else
                {
                    dtMenu = null;
                    MessageBox.Show("메뉴 데이터를 불러올 수 없습니다.", "알림",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                cm.writeLog($"LoadMenuData Error: {ex.Message}");
                dtMenu = null;
            }
        }

        // ══════════════════════════════════════
        // 상위 메뉴 구성 (BarManager - ToolMenuStrip)
        // ══════════════════════════════════════
        private void BuildTopMenu()
        {
            try
            {
                barMainMenu.ItemLinks.Clear();
                barManager.Items.Clear();

                if (dtMenu == null) return;

                // 상위 메뉴 필터 (P_ID가 NULL인 최상위 항목)
                DataRow[] topMenus = GetTopLevelMenus();

                foreach (DataRow row in topMenus)
                {
                    string menuId = row["ID"].ToString();
                    string menuNm = row["MENU_NAME"].ToString();

                    BarButtonItem btnItem = new BarButtonItem();
                    btnItem.Caption = $"  {menuNm}  ";
                    btnItem.Tag = menuId;
                    btnItem.Id = barManager.GetNewItemId();
                    btnItem.ItemAppearance.Normal.Font = new Font("맑은 고딕", 10F, FontStyle.Bold);
                    btnItem.ItemAppearance.Normal.Options.UseFont = true;
                    btnItem.ItemAppearance.Normal.ForeColor = Color.White;
                    btnItem.ItemAppearance.Normal.Options.UseForeColor = true;
                    btnItem.ItemAppearance.Hovered.BackColor = AccentOrange;
                    btnItem.ItemAppearance.Hovered.ForeColor = Color.White;
                    btnItem.ItemAppearance.Hovered.Options.UseBackColor = true;
                    btnItem.ItemAppearance.Hovered.Options.UseForeColor = true;
                    btnItem.ItemAppearance.Pressed.BackColor = AccentOrange;
                    btnItem.ItemAppearance.Pressed.ForeColor = Color.White;
                    btnItem.ItemAppearance.Pressed.Options.UseBackColor = true;
                    btnItem.ItemAppearance.Pressed.Options.UseForeColor = true;

                    btnItem.ItemClick += TopMenuButton_ItemClick;

                    barManager.Items.Add(btnItem);
                    barMainMenu.ItemLinks.Add(btnItem);
                }

                // 우측 버튼들 추가 (검색 먼저, 토글 나중에 → 화면에서 토글이 더 오른쪽)
                AddSearchMenuButton();
                AddSideMenuToggleButton();
            }
            catch (Exception ex)
            {
                cm.writeLog($"BuildTopMenu Error: {ex.Message}");
            }
        }

        private void TopMenuButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                string parentMenuCd = e.Item.Tag?.ToString();
                if (string.IsNullOrEmpty(parentMenuCd)) return;

                currentParentMenuCd = parentMenuCd;
                BuildSideMenu(parentMenuCd);

                // 상위 메뉴 클릭 시 사이드 패널 표시
                splitContainer.PanelVisibility = SplitPanelVisibility.Both;

                lblStatus.Text = $"메뉴: {e.Item.Caption.Trim()}";
            }
            catch (Exception ex)
            {
                cm.writeLog($"TopMenuButton_ItemClick Error: {ex.Message}");
            }
        }

        // ══════════════════════════════════════
        // 사이드 메뉴 구성 (AccordionControl)
        // ══════════════════════════════════════
        private void BuildSideMenu(string parentMenuCd)
        {
            try
            {
                accordionMenu.Elements.Clear();

                if (dtMenu == null) return;

                DataRow[] subMenus = GetChildMenus(parentMenuCd);

                if (subMenus.Length == 0)
                {
                    AccordionControlElement emptyElement = new AccordionControlElement();
                    emptyElement.Text = "등록된 하위 메뉴가 없습니다.";
                    emptyElement.Style = ElementStyle.Item;
                    emptyElement.Enabled = false;
                    accordionMenu.Elements.Add(emptyElement);
                    return;
                }

                // 하위 메뉴가 있는지 확인하여 그룹/아이템 분기
                foreach (DataRow row in subMenus)
                {
                    string menuId = row["ID"].ToString();
                    string menuNm = row["MENU_NAME"].ToString();
                    string formNm = row["FORM_NAME"].ToString();
                    string panelYn = dtMenu.Columns.Contains("PANEL_YN") ? row["PANEL_YN"].ToString() : "Y";
                    string passwordYn = dtMenu.Columns.Contains("PASSWORD_YN") ? row["PASSWORD_YN"].ToString() : "N";
                    string password = dtMenu.Columns.Contains("PASSWORD") ? row["PASSWORD"].ToString() : "";
                    string dockOrNot = dtMenu.Columns.Contains("DOCK_OR_NOT") ? row["DOCK_OR_NOT"].ToString() : "Y";

                    // 이 메뉴의 하위 메뉴가 있는지 확인
                    DataRow[] childItems = GetChildMenus(menuId);

                    if (childItems.Length > 0)
                    {
                        // 그룹으로 생성 (하위 메뉴 있음)
                        AccordionControlElement group = new AccordionControlElement();
                        group.Text = menuNm;
                        group.Style = ElementStyle.Group;
                        group.Expanded = true;

                        foreach (DataRow childRow in childItems)
                        {
                            string childMenuNm = childRow["MENU_NAME"].ToString();
                            string childFormNm = childRow["FORM_NAME"].ToString();
                            string childPanelYn = dtMenu.Columns.Contains("PANEL_YN") ? childRow["PANEL_YN"].ToString() : "Y";
                            string childPasswordYn = dtMenu.Columns.Contains("PASSWORD_YN") ? childRow["PASSWORD_YN"].ToString() : "N";
                            string childPassword = dtMenu.Columns.Contains("PASSWORD") ? childRow["PASSWORD"].ToString() : "";
                            string childDockOrNot = dtMenu.Columns.Contains("DOCK_OR_NOT") ? childRow["DOCK_OR_NOT"].ToString() : "Y";

                            AccordionControlElement item = new AccordionControlElement();
                            item.Text = childMenuNm;
                            item.Tag = childFormNm + "|" + childMenuNm + "|" + childPanelYn + "|" + childPasswordYn + "|" + childPassword + "|" + childDockOrNot;
                            item.Style = ElementStyle.Item;
                            item.Click += SubMenuItem_Click;
                            group.Elements.Add(item);
                        }

                        accordionMenu.Elements.Add(group);
                    }
                    else
                    {
                        // 아이템으로 직접 생성 (하위 메뉴 없음)
                        AccordionControlElement item = new AccordionControlElement();
                        item.Text = menuNm;
                        item.Tag = formNm + "|" + menuNm + "|" + panelYn + "|" + passwordYn + "|" + password + "|" + dockOrNot;
                        item.Style = ElementStyle.Item;
                        item.Click += SubMenuItem_Click;
                        accordionMenu.Elements.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                cm.writeLog($"BuildSideMenu Error: {ex.Message}");
            }
        }

        // ══════════════════════════════════════
        // 서브메뉴 클릭 → 폼 로드 (XtraTabControl)
        // ══════════════════════════════════════
        private void SubMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                var element = sender as AccordionControlElement;
                if (element?.Tag == null) return;

                string[] tagParts = element.Tag.ToString().Split('|');
                string formName = tagParts[0];
                string menuName = tagParts.Length > 1 ? tagParts[1] : formName;
                string panelYn = tagParts.Length > 2 ? tagParts[2] : "Y";
                string passwordYn = tagParts.Length > 3 ? tagParts[3] : "N";
                string password = tagParts.Length > 4 ? tagParts[4] : "";
                string dockOrNot = tagParts.Length > 5 ? tagParts[5] : "Y";

                if (string.IsNullOrEmpty(formName)) return;

                // 비밀번호 체크
                if (passwordYn == "Y" && !string.IsNullOrEmpty(password))
                {
                    // 이미 열려있는 탭이면 비밀번호 다시 안 물어봄
                    bool alreadyOpen = false;
                    foreach (XtraTabPage page in xtraTabControl.TabPages)
                    {
                        if (page.Name == formName) { alreadyOpen = true; break; }
                    }

                    if (!alreadyOpen && !CheckMenuPassword(menuName, password))
                    {
                        return; // 비밀번호 불일치 → 폼 열지 않음
                    }
                }

                LoadFormToTab(formName, menuName, panelYn, dockOrNot);

                // 폼을 열면 사이드 패널 숨김
                splitContainer.PanelVisibility = SplitPanelVisibility.Panel2;
            }
            catch (Exception ex)
            {
                cm.writeLog($"SubMenuItem_Click Error: {ex.Message}");
            }
        }

        /// <summary>
        /// 메뉴 비밀번호 확인 다이얼로그
        /// </summary>
        private bool CheckMenuPassword(string menuName, string correctPassword)
        {
            using (XtraForm pwdForm = new XtraForm())
            {
                pwdForm.Text = "비밀번호 확인";
                pwdForm.Size = new Size(350, 180);
                pwdForm.StartPosition = FormStartPosition.CenterParent;
                pwdForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                pwdForm.MaximizeBox = false;
                pwdForm.MinimizeBox = false;

                LabelControl lblMsg = new LabelControl();
                lblMsg.Text = $"'{menuName}' 접근 비밀번호를 입력하세요.";
                lblMsg.Location = new Point(20, 20);
                lblMsg.AutoSizeMode = LabelAutoSizeMode.Default;
                lblMsg.Appearance.Font = new Font("맑은 고딕", 9.5F);
                lblMsg.Appearance.Options.UseFont = true;

                TextEdit txtPwd = new TextEdit();
                txtPwd.Location = new Point(20, 50);
                txtPwd.Size = new Size(295, 28);
                txtPwd.Properties.PasswordChar = '●';
                txtPwd.Properties.Appearance.Font = new Font("맑은 고딕", 10F);
                txtPwd.Properties.Appearance.Options.UseFont = true;

                SimpleButton btnOk = new SimpleButton();
                btnOk.Text = "확인";
                btnOk.Location = new Point(120, 95);
                btnOk.Size = new Size(90, 32);
                btnOk.DialogResult = DialogResult.OK;

                SimpleButton btnCancel = new SimpleButton();
                btnCancel.Text = "취소";
                btnCancel.Location = new Point(225, 95);
                btnCancel.Size = new Size(90, 32);
                btnCancel.DialogResult = DialogResult.Cancel;

                pwdForm.Controls.AddRange(new Control[] { lblMsg, txtPwd, btnOk, btnCancel });
                pwdForm.AcceptButton = btnOk;
                pwdForm.CancelButton = btnCancel;

                // Enter키로 확인
                txtPwd.KeyDown += (s, ev) =>
                {
                    if (ev.KeyCode == Keys.Enter)
                    {
                        btnOk.PerformClick();
                        ev.Handled = true;
                        ev.SuppressKeyPress = true;
                    }
                };

                if (pwdForm.ShowDialog(this) == DialogResult.OK)
                {
                    if (txtPwd.Text == correctPassword)
                    {
                        return true;
                    }
                    else
                    {
                        MessageBox.Show("비밀번호가 일치하지 않습니다.", "알림",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                }

                return false; // 취소
            }
        }

        // ══════════════════════════════════════
        // 폼을 탭에 로드 (FTP → DLL 다운로드 → 동적 로딩)
        // ══════════════════════════════════════
        private void LoadFormToTab(string formName, string menuName, string panelYn = "Y", string dockOrNot = "Y")
        {
            try
            {
                // 이미 열려있는 탭인지 확인
                foreach (XtraTabPage page in xtraTabControl.TabPages)
                {
                    if (page.Name == formName)
                    {
                        xtraTabControl.SelectedTabPage = page;
                        // 탭 전환 시에도 PANEL_YN 적용
                        ApplyPanelHeaderVisibility(panelYn);
                        lblStatus.Text = $"현재: {menuName}";
                        return;
                    }
                }

                // 로딩 상태 표시
                lblStatus.Text = $"다운로드 중: {menuName}...";
                Cursor = Cursors.WaitCursor;
                Application.DoEvents();

                // 1. FTP에서 DLL 다운로드
                string dllFileName = $"{formName}.dll";
                string localDllPath = ftp.DownloadDll(dllFileName);

                if (string.IsNullOrEmpty(localDllPath) || !File.Exists(localDllPath))
                {
                    Cursor = Cursors.Default;
                    MessageBox.Show($"'{formName}' DLL을 다운로드할 수 없습니다.\nFTP 서버를 확인하세요.",
                        "알림", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    lblStatus.Text = "Ready";
                    return;
                }

                // 2. byte[]로 어셈블리 로드 (파일 잠금 방지)
                byte[] dllBytes = File.ReadAllBytes(localDllPath);
                Assembly asm = Assembly.Load(dllBytes);

                // 3. 폼 타입 검색 (네임스페이스 포함)
                Type formType = asm.GetType($"ERP_IF_PRO.{formName}");

                // fallback: 네임스페이스가 다를 수 있으므로 이름으로 검색
                if (formType == null)
                {
                    foreach (Type t in asm.GetExportedTypes())
                    {
                        if (t.Name == formName && typeof(Form).IsAssignableFrom(t))
                        {
                            formType = t;
                            break;
                        }
                    }
                }

                if (formType == null)
                {
                    Cursor = Cursors.Default;
                    MessageBox.Show($"'{formName}' 폼 타입을 DLL에서 찾을 수 없습니다.",
                        "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    lblStatus.Text = "Ready";
                    return;
                }

                // 4. 폼 인스턴스 생성 및 탭에 추가
                Form frm = (Form)Activator.CreateInstance(formType);
                frm.TopLevel = false;
                frm.FormBorderStyle = FormBorderStyle.None;

                if (dockOrNot == "Y")
                {
                    frm.Dock = DockStyle.Fill;
                }
                else
                {
                    frm.Dock = DockStyle.None;
                    frm.StartPosition = FormStartPosition.Manual;
                }

                frm.Visible = true;
                frm.Tag = dockOrNot; // 센터링용 플래그 저장

                // 5. DLL 안에 UpdateStatus 프로퍼티가 있으면 연결
                var statusProp = formType.GetProperty("UpdateStatus");
                if (statusProp != null && statusProp.PropertyType == typeof(Action<string>))
                {
                    statusProp.SetValue(frm, new Action<string>(text =>
                    {
                        if (lblStatus.InvokeRequired)
                            lblStatus.Invoke(new Action(() => lblStatus.Text = text));
                        else
                            lblStatus.Text = text;
                    }));
                }

                XtraTabPage tabPage = new XtraTabPage();
                tabPage.Name = formName;
                tabPage.Text = menuName;
                tabPage.Tooltip = formName;
                tabPage.Tag = panelYn; // PANEL_YN 정보 저장
                tabPage.Controls.Add(frm);

                // DOCK_OR_NOT = N인 경우 탭 페이지 중앙 정렬
                if (dockOrNot != "Y")
                {
                    tabPage.Resize += (s, ev) =>
                    {
                        CenterFormInTab(frm, tabPage);
                    };
                }

                xtraTabControl.TabPages.Add(tabPage);
                xtraTabControl.SelectedTabPage = tabPage;

                // 추가 직후 중앙 정렬 (크기가 잡힌 후)
                if (dockOrNot != "Y")
                {
                    CenterFormInTab(frm, tabPage);
                }

                // PANEL_YN에 따라 panelHeader 표시/숨김
                ApplyPanelHeaderVisibility(panelYn);

                // 메뉴 접속 로그 INSERT
                InsertMenuLog(formName, menuName);

                lblStatus.Text = $"현재: {menuName}";
            }
            catch (Exception ex)
            {
                // InnerException이 있으면 실제 원인을 표시
                string errorDetail = ex.InnerException != null
                    ? $"{ex.Message}\n\n원인: {ex.InnerException.Message}"
                    : ex.Message;

                cm.writeLog($"LoadFormToTab Error: {errorDetail}");
                MessageBox.Show($"폼 로드 중 오류가 발생했습니다.\n{errorDetail}", "오류",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblStatus.Text = "Ready";
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        // ══════════════════════════════════════
        // PANEL_YN에 따른 panelHeader 표시/숨김
        // ══════════════════════════════════════
        private void ApplyPanelHeaderVisibility(string panelYn)
        {
            if (panelYn == "N")
                panelHeader.Height = 0;
            else
                panelHeader.Height = panelHeaderOriginalHeight;
        }

        private void XtraTabControl_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            try
            {
                if (e.Page == null) return;

                // Welcome 탭이면 panelHeader 보임
                if (e.Page.Name == "Welcome")
                {
                    panelHeader.Height = panelHeaderOriginalHeight;
                    return;
                }

                // 탭에 저장된 PANEL_YN 정보로 panelHeader 제어
                string panelYn = e.Page.Tag?.ToString() ?? "Y";
                ApplyPanelHeaderVisibility(panelYn);
            }
            catch (Exception ex)
            {
                cm.writeLog($"SelectedPageChanged Error: {ex.Message}");
            }
        }

        // ══════════════════════════════════════
        // 탭 닫기 버튼 이벤트
        // ══════════════════════════════════════
        private void XtraTabControl_CloseButtonClick(object sender, EventArgs e)
        {
            try
            {
                XtraTabPage page = xtraTabControl.SelectedTabPage;
                if (page == null || page.Name == "Welcome") return;

                string menuName = page.Text;

                // 종료 확인 메시지
                DialogResult result = MessageBox.Show(
                    $"{menuName}을(를) 종료하시겠습니까?",
                    "확인",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result != DialogResult.Yes) return;

                // 폼 리소스 정리
                foreach (Control ctrl in page.Controls)
                {
                    if (ctrl is Form frm)
                    {
                        frm.Close();
                        frm.Dispose();
                    }
                }
                xtraTabControl.TabPages.Remove(page);
                page.Dispose();
            }
            catch (Exception ex)
            {
                cm.writeLog($"Tab Close Error: {ex.Message}");
            }
        }

        // ══════════════════════════════════════
        // 시계 타이머
        // ══════════════════════════════════════
        private void TimerClock_Tick(object sender, EventArgs e)
        {
            lblDateTime.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        // ══════════════════════════════════════
        // Resize 이벤트 - 상태바 라벨 위치 조정
        // ══════════════════════════════════════
        private void Main_Resize(object sender, EventArgs e)
        {
            RepositionStatusLabels();
        }

        private void RepositionStatusLabels()
        {
            try
            {
                int rightMargin = 15;
                int y = 5;

                lblDateTime.Location = new Point(this.ClientSize.Width - lblDateTime.Width - rightMargin, y);
                lblIP.Location = new Point(lblDateTime.Location.X - lblIP.Width - 20, y);
                lblUser.Location = new Point(lblIP.Location.X - lblUser.Width - 20, y);
            }
            catch { }
        }

        // ══════════════════════════════════════
        // Welcome 탭 추가
        // ══════════════════════════════════════
        private void AddWelcomeTab()
        {
            try
            {
                XtraTabPage welcomePage = new XtraTabPage();
                welcomePage.Name = "Welcome";
                welcomePage.Text = "Home";
                welcomePage.ShowCloseButton = DevExpress.Utils.DefaultBoolean.False;

                PanelControl welcomePanel = new PanelControl();
                welcomePanel.Dock = DockStyle.Fill;
                welcomePanel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
                welcomePanel.Appearance.BackColor = Color.White;
                welcomePanel.Appearance.Options.UseBackColor = true;

                LabelControl lblWelcome = new LabelControl();
                lblWelcome.Text = "INTEROJO";
                lblWelcome.Appearance.Font = new Font("Segoe UI", 36F, FontStyle.Bold);
                lblWelcome.Appearance.ForeColor = PrimaryBlue;
                lblWelcome.Appearance.Options.UseFont = true;
                lblWelcome.Appearance.Options.UseForeColor = true;
                lblWelcome.AutoSizeMode = LabelAutoSizeMode.Default;

                LabelControl lblWelcomeSub = new LabelControl();
                lblWelcomeSub.Text = "ERP Interface System";
                lblWelcomeSub.Appearance.Font = new Font("Segoe UI", 14F, FontStyle.Regular);
                lblWelcomeSub.Appearance.ForeColor = Color.FromArgb(120, 140, 170);
                lblWelcomeSub.Appearance.Options.UseFont = true;
                lblWelcomeSub.Appearance.Options.UseForeColor = true;
                lblWelcomeSub.AutoSizeMode = LabelAutoSizeMode.Default;

                LabelControl lblGuide = new LabelControl();
                lblGuide.Text = "상단 메뉴를 클릭하여 시작하세요.";
                lblGuide.Appearance.Font = new Font("맑은 고딕", 10F);
                lblGuide.Appearance.ForeColor = AccentOrange;
                lblGuide.Appearance.Options.UseFont = true;
                lblGuide.Appearance.Options.UseForeColor = true;
                lblGuide.AutoSizeMode = LabelAutoSizeMode.Default;

                // 중앙 배치를 위한 패널
                Panel centerPanel = new Panel();
                centerPanel.Size = new Size(400, 200);
                centerPanel.BackColor = Color.Transparent;
                centerPanel.Controls.Add(lblGuide);
                centerPanel.Controls.Add(lblWelcomeSub);
                centerPanel.Controls.Add(lblWelcome);

                lblWelcome.Location = new Point(20, 20);
                lblWelcomeSub.Location = new Point(24, 80);
                lblGuide.Location = new Point(24, 120);

                welcomePanel.Controls.Add(centerPanel);

                // 중앙 배치 이벤트
                welcomePanel.Resize += (s, ev) =>
                {
                    centerPanel.Location = new Point(
                        (welcomePanel.Width - centerPanel.Width) / 2,
                        (welcomePanel.Height - centerPanel.Height) / 2
                    );
                };

                welcomePage.Controls.Add(welcomePanel);
                xtraTabControl.TabPages.Add(welcomePage);
                xtraTabControl.SelectedTabPage = welcomePage;
            }
            catch (Exception ex)
            {
                cm.writeLog($"AddWelcomeTab Error: {ex.Message}");
            }
        }

        // ══════════════════════════════════════
        // DOCK_OR_NOT = N인 폼을 탭 중앙에 배치
        // ══════════════════════════════════════
        private void CenterFormInTab(Form frm, XtraTabPage tabPage)
        {
            try
            {
                int x = Math.Max(0, (tabPage.ClientSize.Width - frm.Width) / 2);
                int y = Math.Max(0, (tabPage.ClientSize.Height - frm.Height) / 2);
                frm.Location = new Point(x, y);
            }
            catch { }
        }

        // 메뉴 접속 로그 INSERT
        // ══════════════════════════════════════
        private void InsertMenuLog(string formName, string menuName)
        {
            try
            {
                MSSQL db = new MSSQL("ERP_2");
                string strSql = "ERP_2.dbo.ST_TB_ERP_IF_USER_MENU_LOG_INS";
                db.Parameter("@PC_IP", cm.GetLocalIPAddress());
                db.Parameter("@USER", IsAdmin ? "Admin" : "일반");
                db.Parameter("@FORM_NAME", formName);
                db.Parameter("@MENU_NAME", menuName);
                db.ExecuteNonSql(strSql);
            }
            catch { } // 로그 실패해도 폼 로딩은 진행
        }

        // ══════════════════════════════════════
        // 유틸리티: 메뉴 데이터 접근 헬퍼
        // ══════════════════════════════════════

        /// <summary>
        /// 최상위 메뉴 (P_ID가 NULL인 메뉴) 조회
        /// </summary>
        private DataRow[] GetTopLevelMenus()
        {
            if (dtMenu == null) return new DataRow[0];

            // Admin이면 전체, 일반 사용자면 ADMIN_YN = 'N'만
            if (IsAdmin)
                return dtMenu.Select("P_ID IS NULL", "ID");
            else
                return dtMenu.Select("P_ID IS NULL AND ADMIN_YN = 'N'", "ID");
        }

        /// <summary>
        /// 하위 메뉴 조회 (P_ID = 부모ID)
        /// </summary>
        private DataRow[] GetChildMenus(string parentId)
        {
            if (dtMenu == null || string.IsNullOrEmpty(parentId)) return new DataRow[0];

            if (IsAdmin)
                return dtMenu.Select($"P_ID = {parentId}", "ID");
            else
                return dtMenu.Select($"P_ID = {parentId} AND ADMIN_YN = 'N'", "ID");
        }

        /// <summary>
        /// 상위 메뉴 중 ADMIN_YN='Y'인 것이 있는지 재귀 체크
        /// </summary>
        private bool IsParentAdminOnly(string parentId)
        {
            if (dtMenu == null || string.IsNullOrEmpty(parentId)) return false;

            DataRow[] parentRows = dtMenu.Select($"ID = {parentId}");
            if (parentRows.Length == 0) return false;

            DataRow parent = parentRows[0];
            if (parent["ADMIN_YN"].ToString() == "Y") return true;

            // 최상위까지 재귀
            string grandParentId = parent["P_ID"] == DBNull.Value ? null : parent["P_ID"].ToString();
            if (!string.IsNullOrEmpty(grandParentId))
            {
                return IsParentAdminOnly(grandParentId);
            }

            return false;
        }
    }
}
