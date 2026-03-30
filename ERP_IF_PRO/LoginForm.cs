using System;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using ERP_IF_PRO.Modules;

namespace ERP_IF_PRO
{
    public class LoginForm : XtraForm
    {
        // 관리자 비밀번호
        private const string ADMIN_PASSWORD = "erp@!#$";

        // 컨트롤 선언
        private PanelControl panelMain;
        private LabelControl lblTitle;
        private LabelControl lblSubTitle;
        private LabelControl lblUserType;
        private ComboBoxEdit cboUserType;
        private LabelControl lblPassword;
        private TextEdit txtPassword;
        private SimpleButton btnLogin;
        private SimpleButton btnCancel;

        // 테마 색상
        private static readonly Color PrimaryBlue = Color.FromArgb(27, 80, 145);
        private static readonly Color DarkBlue = Color.FromArgb(18, 55, 100);
        private static readonly Color AccentOrange = Color.FromArgb(245, 130, 32);

        /// <summary>
        /// 로그인 결과: true = 관리자, false = 일반 사용자
        /// </summary>
        public bool IsAdmin { get; private set; }

        public LoginForm()
        {
            InitControls();
            ApplyStyle();
        }

        private void InitControls()
        {
            // Form 설정
            this.Text = "INTEROJO - ERP Interface System";
            this.ClientSize = new Size(420, 320);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.AcceptButton = null;

            // 메인 패널
            panelMain = new PanelControl();
            panelMain.Dock = DockStyle.Fill;
            panelMain.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;

            // 타이틀
            lblTitle = new LabelControl();
            lblTitle.Text = "INTEROJO";
            lblTitle.Location = new Point(30, 25);
            lblTitle.AutoSizeMode = LabelAutoSizeMode.Default;

            // 서브 타이틀
            lblSubTitle = new LabelControl();
            lblSubTitle.Text = "ERP Interface System";
            lblSubTitle.Location = new Point(32, 70);
            lblSubTitle.AutoSizeMode = LabelAutoSizeMode.Default;

            // 사용자 유형 라벨
            lblUserType = new LabelControl();
            lblUserType.Text = "사용자 유형";
            lblUserType.Location = new Point(30, 120);
            lblUserType.AutoSizeMode = LabelAutoSizeMode.Default;

            // 사용자 유형 콤보박스
            cboUserType = new ComboBoxEdit();
            cboUserType.Location = new Point(30, 145);
            cboUserType.Size = new Size(360, 24);
            cboUserType.Properties.Items.AddRange(new string[] { "일반 사용자", "Admin" });
            cboUserType.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            cboUserType.SelectedIndex = 0;
            cboUserType.SelectedIndexChanged += CboUserType_SelectedIndexChanged;

            // 비밀번호 라벨
            lblPassword = new LabelControl();
            lblPassword.Text = "비밀번호";
            lblPassword.Location = new Point(30, 185);
            lblPassword.AutoSizeMode = LabelAutoSizeMode.Default;
            lblPassword.Visible = false;

            // 비밀번호 입력
            txtPassword = new TextEdit();
            txtPassword.Location = new Point(30, 210);
            txtPassword.Size = new Size(360, 24);
            txtPassword.Properties.PasswordChar = '●';
            txtPassword.Visible = false;
            txtPassword.KeyDown += TxtPassword_KeyDown;

            // 로그인 버튼
            btnLogin = new SimpleButton();
            btnLogin.Text = "로그인";
            btnLogin.Location = new Point(140, 265);
            btnLogin.Size = new Size(120, 36);
            btnLogin.Click += BtnLogin_Click;

            // 취소 버튼
            btnCancel = new SimpleButton();
            btnCancel.Text = "취소";
            btnCancel.Location = new Point(270, 265);
            btnCancel.Size = new Size(120, 36);
            btnCancel.Click += BtnCancel_Click;

            // 컨트롤 추가
            panelMain.Controls.Add(lblTitle);
            panelMain.Controls.Add(lblSubTitle);
            panelMain.Controls.Add(lblUserType);
            panelMain.Controls.Add(cboUserType);
            panelMain.Controls.Add(lblPassword);
            panelMain.Controls.Add(txtPassword);
            panelMain.Controls.Add(btnLogin);
            panelMain.Controls.Add(btnCancel);

            this.Controls.Add(panelMain);
        }

        private void ApplyStyle()
        {
            // 패널 배경
            panelMain.Appearance.BackColor = Color.White;
            panelMain.Appearance.Options.UseBackColor = true;

            // 타이틀
            lblTitle.Appearance.Font = new Font("Segoe UI", 24F, FontStyle.Bold);
            lblTitle.Appearance.ForeColor = PrimaryBlue;
            lblTitle.Appearance.Options.UseFont = true;
            lblTitle.Appearance.Options.UseForeColor = true;

            // 서브 타이틀
            lblSubTitle.Appearance.Font = new Font("Segoe UI", 11F);
            lblSubTitle.Appearance.ForeColor = Color.FromArgb(120, 140, 170);
            lblSubTitle.Appearance.Options.UseFont = true;
            lblSubTitle.Appearance.Options.UseForeColor = true;

            // 사용자 유형 라벨
            lblUserType.Appearance.Font = new Font("맑은 고딕", 9.5F, FontStyle.Bold);
            lblUserType.Appearance.ForeColor = DarkBlue;
            lblUserType.Appearance.Options.UseFont = true;
            lblUserType.Appearance.Options.UseForeColor = true;

            // 비밀번호 라벨
            lblPassword.Appearance.Font = new Font("맑은 고딕", 9.5F, FontStyle.Bold);
            lblPassword.Appearance.ForeColor = DarkBlue;
            lblPassword.Appearance.Options.UseFont = true;
            lblPassword.Appearance.Options.UseForeColor = true;

            // 로그인 버튼
            btnLogin.Appearance.Font = new Font("맑은 고딕", 10F, FontStyle.Bold);
            btnLogin.Appearance.Options.UseFont = true;
            btnLogin.Appearance.BackColor = PrimaryBlue;
            btnLogin.Appearance.ForeColor = Color.White;
            btnLogin.Appearance.Options.UseBackColor = true;
            btnLogin.Appearance.Options.UseForeColor = true;

            // 취소 버튼
            btnCancel.Appearance.Font = new Font("맑은 고딕", 10F);
            btnCancel.Appearance.Options.UseFont = true;
        }

        private void CboUserType_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool isAdmin = cboUserType.SelectedIndex == 1;
            lblPassword.Visible = isAdmin;
            txtPassword.Visible = isAdmin;
            txtPassword.Text = string.Empty;

            if (isAdmin)
            {
                txtPassword.Focus();
            }
        }

        private void TxtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                BtnLogin_Click(sender, e);
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            if (cboUserType.SelectedIndex == 1) // Admin
            {
                if (txtPassword.Text == ADMIN_PASSWORD)
                {
                    IsAdmin = true;
                    InsertLoginLog("Admin");
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("비밀번호가 올바르지 않습니다.", "알림",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtPassword.Text = string.Empty;
                    txtPassword.Focus();
                }
            }
            else // 일반 사용자
            {
                IsAdmin = false;
                InsertLoginLog("일반");
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        /// <summary>
        /// 로그인 로그 INSERT
        /// </summary>
        private void InsertLoginLog(string userName)
        {
            try
            {
                string pcIp = GetLocalIPAddress();
                MSSQL db = new MSSQL("ERP_2");
                string strSql = "ERP_2.dbo.ST_TB_ERP_IF_USER_LOGIN_LOG_INS";
                db.Parameter("@PC_IP", pcIp);
                db.Parameter("@USER", userName);
                db.ExecuteNonSql(strSql);
            }
            catch { } // 로그 실패해도 로그인은 진행
        }

        /// <summary>
        /// 로컬 IP 주소 가져오기
        /// </summary>
        private string GetLocalIPAddress()
        {
            try
            {
                var host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                        return ip.ToString();
                }
                return "Unknown";
            }
            catch { return "Unknown"; }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
