using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using RAZER_C.Modules;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace RAZER_C
{
    public partial class ACCESSLOG : Form
    {
        public Action<string> UpdateStatus { get; set; }
        private static string dbName = "ERP_2";
        CommonModule cm = new CommonModule();
        MSSQL db = new MSSQL(dbName);

        public ACCESSLOG()
        {
            InitializeComponent();
            this.ControlBox = false;
            initControl();
        }

        private void initControl()
        {
            try
            {
                btn_Search.Click += (s, e) => fn_Search();

                // 초기 조회
                fn_Search();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "초기화 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 조회 (로그인 로그 + 메뉴 로그 동시 조회)
        /// </summary>
        private void fn_Search()
        {
            try
            {
                fn_SearchLoginLog();
                fn_SearchMenuLog();

                UpdateStatus?.Invoke("접속 기록 조회 완료");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "조회 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 로그인 접속 기록 조회
        /// </summary>
        private void fn_SearchLoginLog()
        {
            try
            {
                MSSQL dbLogin = new MSSQL(dbName);
                string strSql = dbName + ".dbo.ST_TB_ERP_IF_USER_LOGIN_LOG_SEL";
                dbLogin.ExecuteSql(strSql);

                if (dbLogin.nState)
                {
                    if (!string.IsNullOrEmpty(dbLogin.sql_raise_error_msg))
                    {
                        MessageBox.Show(dbLogin.sql_raise_error_msg, "로그인 로그 조회 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    gridControlLogin.DataSource = dbLogin.result;
                    fn_SetLoginGridColumns();

                    int cnt = dbLogin.result != null ? dbLogin.result.Rows.Count : 0;
                    lblLoginTitle.Text = $"▶ 로그인 접속 기록 ({cnt}건)";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "로그인 로그 조회 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 메뉴 접속 기록 조회
        /// </summary>
        private void fn_SearchMenuLog()
        {
            try
            {
                MSSQL dbMenu = new MSSQL(dbName);
                string strSql = dbName + ".dbo.ST_TB_ERP_IF_USER_MENU_LOG_SEL";
                dbMenu.ExecuteSql(strSql);

                if (dbMenu.nState)
                {
                    if (!string.IsNullOrEmpty(dbMenu.sql_raise_error_msg))
                    {
                        MessageBox.Show(dbMenu.sql_raise_error_msg, "메뉴 로그 조회 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    gridControlMenu.DataSource = dbMenu.result;
                    fn_SetMenuGridColumns();

                    int cnt = dbMenu.result != null ? dbMenu.result.Rows.Count : 0;
                    lblMenuTitle.Text = $"▶ 메뉴 접속 기록 ({cnt}건)";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "메뉴 로그 조회 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 로그인 로그 그리드 컬럼 설정
        /// </summary>
        private void fn_SetLoginGridColumns()
        {
            if (gridViewLogin.Columns.Count == 0) return;

            gridViewLogin.Columns["ID"].Caption = "No";
            gridViewLogin.Columns["ID"].Width = 60;
            gridViewLogin.Columns["ID"].VisibleIndex = 0;

            gridViewLogin.Columns["PC_IP"].Caption = "PC IP";
            gridViewLogin.Columns["PC_IP"].Width = 150;
            gridViewLogin.Columns["PC_IP"].VisibleIndex = 1;

            gridViewLogin.Columns["USER"].Caption = "사용자";
            gridViewLogin.Columns["USER"].Width = 120;
            gridViewLogin.Columns["USER"].VisibleIndex = 2;

            gridViewLogin.Columns["LOGIN_TIME"].Caption = "접속 일시";
            gridViewLogin.Columns["LOGIN_TIME"].Width = 180;
            gridViewLogin.Columns["LOGIN_TIME"].VisibleIndex = 3;
            gridViewLogin.Columns["LOGIN_TIME"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            gridViewLogin.Columns["LOGIN_TIME"].DisplayFormat.FormatString = "yyyy-MM-dd HH:mm:ss";

            // 정렬: 최근 접속 순
            gridViewLogin.Columns["ID"].SortOrder = DevExpress.Data.ColumnSortOrder.Descending;
        }

        /// <summary>
        /// 메뉴 로그 그리드 컬럼 설정
        /// </summary>
        private void fn_SetMenuGridColumns()
        {
            if (gridViewMenu.Columns.Count == 0) return;

            gridViewMenu.Columns["ID"].Caption = "No";
            gridViewMenu.Columns["ID"].Width = 60;
            gridViewMenu.Columns["ID"].VisibleIndex = 0;

            gridViewMenu.Columns["PC_IP"].Caption = "PC IP";
            gridViewMenu.Columns["PC_IP"].Width = 150;
            gridViewMenu.Columns["PC_IP"].VisibleIndex = 1;

            gridViewMenu.Columns["USER"].Caption = "사용자";
            gridViewMenu.Columns["USER"].Width = 100;
            gridViewMenu.Columns["USER"].VisibleIndex = 2;

            gridViewMenu.Columns["FORM_NAME"].Caption = "폼명";
            gridViewMenu.Columns["FORM_NAME"].Width = 200;
            gridViewMenu.Columns["FORM_NAME"].VisibleIndex = 3;

            gridViewMenu.Columns["MENU_NAME"].Caption = "메뉴명";
            gridViewMenu.Columns["MENU_NAME"].Width = 200;
            gridViewMenu.Columns["MENU_NAME"].VisibleIndex = 4;

            gridViewMenu.Columns["MENU_IN_TIME"].Caption = "접속 일시";
            gridViewMenu.Columns["MENU_IN_TIME"].Width = 180;
            gridViewMenu.Columns["MENU_IN_TIME"].VisibleIndex = 5;
            gridViewMenu.Columns["MENU_IN_TIME"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            gridViewMenu.Columns["MENU_IN_TIME"].DisplayFormat.FormatString = "yyyy-MM-dd HH:mm:ss";

            // 정렬: 최근 접속 순
            gridViewMenu.Columns["ID"].SortOrder = DevExpress.Data.ColumnSortOrder.Descending;
        }
    }
}
