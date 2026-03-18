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

        public Main()
        {
            InitializeComponent();
            ApplyTheme();
            this.Load += Main_Load;
            this.timerClock.Tick += TimerClock_Tick;
            this.xtraTabControl.CloseButtonClick += XtraTabControl_CloseButtonClick;
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
        // Form Load
        // ══════════════════════════════════════
        private void Main_Load(object sender, EventArgs e)
        {
            try
            {
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

                // 토글 버튼을 메뉴 우측에 항시 추가
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

                            AccordionControlElement item = new AccordionControlElement();
                            item.Text = childMenuNm;
                            item.Tag = childFormNm + "|" + childMenuNm;
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
                        item.Tag = formNm + "|" + menuNm;
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

                if (string.IsNullOrEmpty(formName)) return;

                LoadFormToTab(formName, menuName);

                // 폼을 열면 사이드 패널 숨김
                splitContainer.PanelVisibility = SplitPanelVisibility.Panel2;
            }
            catch (Exception ex)
            {
                cm.writeLog($"SubMenuItem_Click Error: {ex.Message}");
            }
        }

        // ══════════════════════════════════════
        // 폼을 탭에 로드 (FTP → DLL 다운로드 → 동적 로딩)
        // ══════════════════════════════════════
        private void LoadFormToTab(string formName, string menuName)
        {
            try
            {
                // 이미 열려있는 탭인지 확인
                foreach (XtraTabPage page in xtraTabControl.TabPages)
                {
                    if (page.Name == formName)
                    {
                        xtraTabControl.SelectedTabPage = page;
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
                frm.Dock = DockStyle.Fill;
                frm.Visible = true;

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
                tabPage.Controls.Add(frm);

                xtraTabControl.TabPages.Add(tabPage);
                xtraTabControl.SelectedTabPage = tabPage;

                lblStatus.Text = $"현재: {menuName}";
            }
            catch (Exception ex)
            {
                cm.writeLog($"LoadFormToTab Error: {ex.Message}");
                MessageBox.Show($"폼 로드 중 오류가 발생했습니다.\n{ex.Message}", "오류",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblStatus.Text = "Ready";
            }
            finally
            {
                Cursor = Cursors.Default;
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
    }
}
