using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraTab;
using ERP_IF_PRO.Modules;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace RAZER_C
{
    public partial class COMBI_USER_MANAGE : Form
    {
        public Action<string> UpdateStatus { get; set; }
        private static string dbName = "ERP_2";
        CommonModule cm = new CommonModule();
        MSSQL db = new MSSQL(dbName);

        private DataTable dtUsers = null;
        private string currentKind = "COMBO_10";

        // 탭 ↔ KIND 매핑
        private string GetCurrentKind()
        {
            if (xtraTabProcess.SelectedTabPage == tabInk) return "COMBO_10";
            if (xtraTabProcess.SelectedTabPage == tabSemi) return "COMBO_11";
            if (xtraTabProcess.SelectedTabPage == tabWater) return "COMBO_12";
            return "COMBO_10";
        }

        private string GetCurrentTabName()
        {
            if (currentKind == "COMBO_10") return "잉크";
            if (currentKind == "COMBO_11") return "반제품";
            if (currentKind == "COMBO_12") return "용수";
            return "";
        }

        private string GetProcessSuffix()
        {
            if (currentKind == "COMBO_10") return "BC";
            if (currentKind == "COMBO_11") return "B";
            if (currentKind == "COMBO_12") return "BW";
            return "";
        }

        // 공정코드 → 한글 변환 (조회용)
        private string ProcessCodeToName(string code)
        {
            switch (code)
            {
                case "BC": return "잉크";
                case "B": return "반제품";
                case "BW": return "용수";
                default: return code;
            }
        }

        // 한글 → 공정코드 변환 (저장용)
        private string ProcessNameToCode(string name)
        {
            switch (name)
            {
                case "잉크": return "BC";
                case "반제품": return "B";
                case "용수": return "BW";
                default: return name;
            }
        }

        public COMBI_USER_MANAGE()
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
                btn_Add.Click += (s, e) => fn_Add();
                btn_Delete.Click += (s, e) => fn_Delete();
                btn_Save.Click += (s, e) => fn_Save();

                xtraTabProcess.SelectedPageChanged += XtraTabProcess_SelectedPageChanged;
                gridView1.CellValueChanged += GridView1_CellValueChanged;

                fn_Search();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "초기화 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void XtraTabProcess_SelectedPageChanged(object sender, TabPageChangedEventArgs e)
        {
            try
            {
                if (e.Page == null) return;
                currentKind = GetCurrentKind();
                fn_Search();
            }
            catch (Exception ex)
            {
                cm.writeLog($"TabChanged Error: {ex.Message}");
            }
        }

        private void GridView1_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                if (e.Column.FieldName != "CHK")
                {
                    if (!gridView1.GetRowCellDisplayText(e.RowHandle, "CHK").Equals("Checked"))
                    {
                        gridView1.SetRowCellValue(e.RowHandle, "CHK", "True");
                    }
                }
            }
            catch { }
        }

        /// <summary>
        /// DB에서 가져온 COMBO_STR을 이름/사번/공정 컬럼으로 분리한 DataTable 생성
        /// </summary>
        private DataTable BuildDisplayTable(DataTable dbResult)
        {
            DataTable displayDt = new DataTable();
            displayDt.Columns.Add("CHK", typeof(string));
            displayDt.Columns.Add("ORD_ID", typeof(int));
            displayDt.Columns.Add("USER_NAME", typeof(string));   // 이름
            displayDt.Columns.Add("USER_CODE", typeof(string));   // 사번
            displayDt.Columns.Add("PROCESS_TYPE", typeof(string)); // 공정구분
            displayDt.Columns.Add("USE_FLAG", typeof(string));

            if (dbResult == null) return displayDt;

            foreach (DataRow row in dbResult.Rows)
            {
                DataRow newRow = displayDt.NewRow();
                newRow["CHK"] = "False";
                newRow["ORD_ID"] = row["ORD_ID"] != DBNull.Value ? Convert.ToInt32(row["ORD_ID"]) : 0;
                newRow["USE_FLAG"] = row["USE_FLAG"] != DBNull.Value ? row["USE_FLAG"].ToString() : "Y";

                // COMBO_STR split: "이름/사번/공정구분"
                string comboStr = row["COMBO_STR"] != DBNull.Value ? row["COMBO_STR"].ToString() : "";
                string[] parts = comboStr.Split('/');

                newRow["USER_NAME"] = parts.Length > 0 ? parts[0] : "";
                newRow["USER_CODE"] = parts.Length > 1 ? parts[1] : "";
                string rawCode = parts.Length > 2 ? parts[2] : GetProcessSuffix();
                newRow["PROCESS_TYPE"] = ProcessCodeToName(rawCode);

                displayDt.Rows.Add(newRow);
            }

            return displayDt;
        }

        /// <summary>
        /// 조회
        /// </summary>
        private void fn_Search()
        {
            try
            {
                currentKind = GetCurrentKind();

                string strSql = dbName + ".dbo.ST_COMBOTBL_USER_SEL";
                db.Parameter("@F_NAME", "REGI_COMBI");
                db.Parameter("@KIND", currentKind);
                db.ExecuteSql(strSql);

                if (db.nState)
                {
                    if (!string.IsNullOrEmpty(db.sql_raise_error_msg))
                    {
                        MessageBox.Show(db.sql_raise_error_msg, "조회 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    dtUsers = BuildDisplayTable(db.result);
                    gridControl1.DataSource = dtUsers;
                    fn_SetGridColumns();

                    int cnt = dtUsers != null ? dtUsers.Rows.Count : 0;
                    UpdateStatus?.Invoke($"[{GetCurrentTabName()}] 조회 완료 ({cnt}건)");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "조회 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 그리드 컬럼 설정
        /// </summary>
        private void fn_SetGridColumns()
        {
            if (gridView1.Columns.Count == 0) return;

            // CHK
            gridView1.Columns["CHK"].Caption = "선택";
            gridView1.Columns["CHK"].Width = 50;
            gridView1.Columns["CHK"].VisibleIndex = 0;
            gridView1.Columns["CHK"].OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;

            // ORD_ID
            gridView1.Columns["ORD_ID"].Caption = "순서";
            gridView1.Columns["ORD_ID"].Width = 60;
            gridView1.Columns["ORD_ID"].VisibleIndex = 1;

            // USER_NAME
            gridView1.Columns["USER_NAME"].Caption = "이름";
            gridView1.Columns["USER_NAME"].Width = 120;
            gridView1.Columns["USER_NAME"].VisibleIndex = 2;

            // USER_CODE
            gridView1.Columns["USER_CODE"].Caption = "사번";
            gridView1.Columns["USER_CODE"].Width = 120;
            gridView1.Columns["USER_CODE"].VisibleIndex = 3;

            // PROCESS_TYPE
            gridView1.Columns["PROCESS_TYPE"].Caption = "공정구분";
            gridView1.Columns["PROCESS_TYPE"].Width = 80;
            gridView1.Columns["PROCESS_TYPE"].VisibleIndex = 4;
            gridView1.Columns["PROCESS_TYPE"].OptionsColumn.AllowEdit = false;

            // USE_FLAG
            gridView1.Columns["USE_FLAG"].Caption = "사용유무";
            gridView1.Columns["USE_FLAG"].Width = 80;
            gridView1.Columns["USE_FLAG"].VisibleIndex = 5;

            // CHK 체크박스
            RepositoryItemCheckEdit ri = new RepositoryItemCheckEdit();
            ri.ValueChecked = "True";
            ri.ValueUnchecked = "False";
            ri.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
            ri.CheckBoxOptions.Style = DevExpress.XtraEditors.Controls.CheckBoxStyle.SvgCheckBox1;
            gridView1.Columns["CHK"].ColumnEdit = ri;

            // USE_FLAG 콤보
            RepositoryItemComboBox comboUseFlag = new RepositoryItemComboBox();
            comboUseFlag.Items.Add("Y");
            comboUseFlag.Items.Add("N");
            gridView1.Columns["USE_FLAG"].ColumnEdit = comboUseFlag;

            // 그리드 옵션
            gridView1.OptionsView.ShowIndicator = false;
            gridView1.OptionsView.ShowGroupPanel = false;
            gridView1.OptionsView.ColumnHeaderAutoHeight = DevExpress.Utils.DefaultBoolean.True;
            gridView1.OptionsBehavior.EditorShowMode = DevExpress.Utils.EditorShowMode.MouseDown;
        }

        /// <summary>
        /// 추가
        /// </summary>
        private void fn_Add()
        {
            try
            {
                if (dtUsers == null)
                {
                    dtUsers = BuildDisplayTable(null);
                    gridControl1.DataSource = dtUsers;
                    fn_SetGridColumns();
                }

                // 다음 ORD_ID 계산
                int nextOrdId = 1;
                foreach (DataRow row in dtUsers.Rows)
                {
                    if (row.RowState != DataRowState.Deleted && row["ORD_ID"] != DBNull.Value)
                    {
                        int ordId = Convert.ToInt32(row["ORD_ID"]);
                        if (ordId >= nextOrdId) nextOrdId = ordId + 1;
                    }
                }

                DataRow newRow = dtUsers.NewRow();
                newRow["CHK"] = "True";
                newRow["ORD_ID"] = nextOrdId;
                newRow["USER_NAME"] = "";
                newRow["USER_CODE"] = "";
                newRow["PROCESS_TYPE"] = ProcessCodeToName(GetProcessSuffix());
                newRow["USE_FLAG"] = "Y";
                dtUsers.Rows.Add(newRow);

                gridView1.FocusedRowHandle = gridView1.RowCount - 1;
                UpdateStatus?.Invoke("새 사용자가 추가되었습니다. 이름과 사번을 입력하세요.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "추가 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 삭제
        /// </summary>
        private void fn_Delete()
        {
            try
            {
                gridView1.CloseEditor();
                gridView1.UpdateCurrentRow();

                if (dtUsers == null || dtUsers.Rows.Count == 0)
                {
                    MessageBox.Show("삭제할 데이터가 없습니다.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                DataTable delDt = new DataTable("ROW");
                delDt.Columns.Add("F_NAME", typeof(string));
                delDt.Columns.Add("KIND", typeof(string));
                delDt.Columns.Add("ORD_ID", typeof(string));

                for (int i = 0; i < gridView1.RowCount; i++)
                {
                    if (gridView1.GetRowCellDisplayText(i, "CHK").Equals("Checked"))
                    {
                        string ordId = gridView1.GetRowCellDisplayText(i, "ORD_ID");
                        if (!string.IsNullOrEmpty(ordId))
                        {
                            DataRow delRow = delDt.NewRow();
                            delRow["F_NAME"] = "REGI_COMBI";
                            delRow["KIND"] = currentKind;
                            delRow["ORD_ID"] = ordId;
                            delDt.Rows.Add(delRow);
                        }
                    }
                }

                if (delDt.Rows.Count == 0)
                {
                    bool hasNewChecked = false;
                    for (int i = gridView1.RowCount - 1; i >= 0; i--)
                    {
                        if (gridView1.GetRowCellDisplayText(i, "CHK").Equals("Checked"))
                        {
                            hasNewChecked = true;
                            gridView1.DeleteRow(i);
                        }
                    }

                    if (!hasNewChecked)
                    {
                        MessageBox.Show("삭제할 항목을 선택해주세요.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    UpdateStatus?.Invoke("삭제 완료");
                    return;
                }

                if (MessageBox.Show($"선택한 {delDt.Rows.Count}건을 삭제하시겠습니까?", "삭제 확인",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    return;

                string xmlData = cm.DataTblToXML(delDt);
                string strSql = dbName + ".dbo.ST_COMBOTBL_USER_DEL";
                db.Parameter("@XML_DATA", xmlData);
                db.ExecuteNonSql(strSql);

                if (db.nState)
                {
                    if (!string.IsNullOrEmpty(db.sql_raise_error_msg))
                    {
                        MessageBox.Show(db.sql_raise_error_msg, "삭제 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    UpdateStatus?.Invoke("삭제 완료");
                    fn_Search();
                }
                else
                {
                    MessageBox.Show("삭제 중 오류가 발생했습니다.", "삭제 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "삭제 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 저장 - 이름/사번/공정구분을 다시 COMBO_STR로 합쳐서 저장
        /// </summary>
        private void fn_Save()
        {
            try
            {
                gridView1.CloseEditor();
                gridView1.UpdateCurrentRow();

                if (dtUsers == null || dtUsers.Rows.Count == 0)
                {
                    MessageBox.Show("저장할 데이터가 없습니다.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                DataTable saveDt = new DataTable("ROW");
                saveDt.Columns.Add("F_NAME", typeof(string));
                saveDt.Columns.Add("KIND", typeof(string));
                saveDt.Columns.Add("ORD_ID", typeof(string));
                saveDt.Columns.Add("COMBO_STR", typeof(string));
                saveDt.Columns.Add("USE_FLAG", typeof(string));

                for (int i = 0; i < gridView1.RowCount; i++)
                {
                    if (!gridView1.GetRowCellDisplayText(i, "CHK").Equals("Checked")) continue;

                    string userName = gridView1.GetRowCellDisplayText(i, "USER_NAME").Trim();
                    string userCode = gridView1.GetRowCellDisplayText(i, "USER_CODE").Trim();
                    string processType = gridView1.GetRowCellDisplayText(i, "PROCESS_TYPE").Trim();

                    if (string.IsNullOrEmpty(userName))
                    {
                        MessageBox.Show($"{i + 1}번째 행: 이름을 입력해주세요.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        gridView1.FocusedRowHandle = i;
                        return;
                    }

                    if (string.IsNullOrEmpty(userCode))
                    {
                        MessageBox.Show($"{i + 1}번째 행: 사번을 입력해주세요.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        gridView1.FocusedRowHandle = i;
                        return;
                    }

                    // 이름/사번/공정구분 → COMBO_STR 조합 (한글→코드 변환)
                    string processCode = ProcessNameToCode(processType);
                    string comboStr = $"{userName}/{userCode}/{processCode}";

                    DataRow newRow = saveDt.NewRow();
                    newRow["F_NAME"] = "REGI_COMBI";
                    newRow["KIND"] = currentKind;
                    newRow["ORD_ID"] = gridView1.GetRowCellDisplayText(i, "ORD_ID");
                    newRow["COMBO_STR"] = comboStr;
                    newRow["USE_FLAG"] = gridView1.GetRowCellDisplayText(i, "USE_FLAG");
                    saveDt.Rows.Add(newRow);
                }

                if (saveDt.Rows.Count == 0)
                {
                    MessageBox.Show("저장할 데이터가 없습니다.\n변경된 행을 선택해주세요.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                string xmlData = cm.DataTblToXML(saveDt);
                string strSql = dbName + ".dbo.ST_COMBOTBL_USER_SAVE";
                db.Parameter("@XML_DATA", xmlData);
                db.ExecuteNonSql(strSql);

                if (db.nState)
                {
                    if (!string.IsNullOrEmpty(db.sql_raise_error_msg))
                    {
                        MessageBox.Show(db.sql_raise_error_msg, "저장 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    UpdateStatus?.Invoke("저장 완료");
                    fn_Search();
                }
                else
                {
                    MessageBox.Show("저장 중 오류가 발생했습니다.", "저장 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "저장 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
