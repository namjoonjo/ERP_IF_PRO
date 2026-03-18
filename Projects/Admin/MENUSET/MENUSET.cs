using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Grid;
using RAZER_C.Modules;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace RAZER_C
{
    public partial class MENUSET : Form
    {

        public Action<string> UpdateStatus { get; set; }
        private static string dbName = "ERP_2";
        CommonModule cm = new CommonModule();
        MSSQL db = new MSSQL(dbName);

        private DataTable dtMenu = null;

        public MENUSET()
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

                // 데이터 변경 시 자동 체크 (CHK 컬럼 제외)
                gridView1.CellValueChanged += GridView1_CellValueChanged;

                // 초기 조회
                fn_Search();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "초기화 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 셀 값 변경 시 자동 체크
        /// </summary>
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
        /// 조회
        /// </summary>
        private void fn_Search()
        {
            try
            {
                string strSql = dbName + ".dbo.ST_TB_MENU_MST_SEL";
                db.ExecuteSql(strSql);

                if (db.nState)
                {
                    if (!string.IsNullOrEmpty(db.sql_raise_error_msg))
                    {
                        MessageBox.Show(db.sql_raise_error_msg, "조회 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    dtMenu = db.result;

                    // 선택 컬럼 추가 (string 타입 - EXCEL_QR_MAPPER 패턴)
                    if (!dtMenu.Columns.Contains("CHK"))
                    {
                        dtMenu.Columns.Add("CHK", typeof(string));
                        foreach (DataRow row in dtMenu.Rows)
                        {
                            row["CHK"] = "False";
                        }
                    }

                    gridControl1.DataSource = dtMenu;
                    fn_SetGridColumns();

                    UpdateStatus?.Invoke($"조회 완료 ({dtMenu.Rows.Count}건)");
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

            // 컬럼 순서 및 캡션 설정
            gridView1.Columns["CHK"].Caption = "선택";
            gridView1.Columns["CHK"].Width = 50;
            gridView1.Columns["CHK"].VisibleIndex = 0;
            gridView1.Columns["CHK"].OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;

            gridView1.Columns["ID"].Caption = "ID";
            gridView1.Columns["ID"].Width = 60;
            gridView1.Columns["ID"].VisibleIndex = 1;
            gridView1.Columns["ID"].OptionsColumn.AllowEdit = false;

            gridView1.Columns["P_ID"].Caption = "상위ID";
            gridView1.Columns["P_ID"].Width = 70;
            gridView1.Columns["P_ID"].VisibleIndex = 2;

            gridView1.Columns["GUBUN"].Caption = "구분";
            gridView1.Columns["GUBUN"].Width = 100;
            gridView1.Columns["GUBUN"].VisibleIndex = 3;

            gridView1.Columns["FORM_NAME"].Caption = "폼명";
            gridView1.Columns["FORM_NAME"].Width = 200;
            gridView1.Columns["FORM_NAME"].VisibleIndex = 4;

            gridView1.Columns["MENU_NAME"].Caption = "메뉴명";
            gridView1.Columns["MENU_NAME"].Width = 200;
            gridView1.Columns["MENU_NAME"].VisibleIndex = 5;

            gridView1.Columns["MENU_GROUP"].Caption = "메뉴그룹";
            gridView1.Columns["MENU_GROUP"].Width = 120;
            gridView1.Columns["MENU_GROUP"].VisibleIndex = 6;

            gridView1.Columns["ADMIN_YN"].Caption = "관리자";
            gridView1.Columns["ADMIN_YN"].Width = 60;
            gridView1.Columns["ADMIN_YN"].VisibleIndex = 7;

            gridView1.Columns["DOCK_OR_NOT"].Caption = "도킹";
            gridView1.Columns["DOCK_OR_NOT"].Width = 60;
            gridView1.Columns["DOCK_OR_NOT"].VisibleIndex = 8;

            gridView1.Columns["USE_FLAG"].Caption = "사용유무";
            gridView1.Columns["USE_FLAG"].Width = 70;
            gridView1.Columns["USE_FLAG"].VisibleIndex = 9;

            // CHK 컬럼 체크박스 에디터 (EXCEL_QR_MAPPER 패턴 그대로)
            RepositoryItemCheckEdit ri = new RepositoryItemCheckEdit();
            ri.ValueChecked = "True";
            ri.ValueUnchecked = "False";
            ri.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
            ri.CheckBoxOptions.Style = DevExpress.XtraEditors.Controls.CheckBoxStyle.SvgCheckBox1;
            gridView1.Columns["CHK"].ColumnEdit = ri;

            // USE_FLAG, ADMIN_YN, DOCK_OR_NOT 콤보박스
            RepositoryItemComboBox comboUseFlag = new RepositoryItemComboBox();
            comboUseFlag.Items.Add("Y");
            comboUseFlag.Items.Add("N");
            gridView1.Columns["USE_FLAG"].ColumnEdit = comboUseFlag;

            RepositoryItemComboBox comboAdminYn = new RepositoryItemComboBox();
            comboAdminYn.Items.Add("Y");
            comboAdminYn.Items.Add("N");
            gridView1.Columns["ADMIN_YN"].ColumnEdit = comboAdminYn;

            RepositoryItemComboBox comboDock = new RepositoryItemComboBox();
            comboDock.Items.Add("Y");
            comboDock.Items.Add("N");
            gridView1.Columns["DOCK_OR_NOT"].ColumnEdit = comboDock;

            // 그리드 옵션
            gridView1.OptionsView.ShowIndicator = false;
            gridView1.OptionsView.ShowGroupPanel = false;
            gridView1.OptionsView.ColumnHeaderAutoHeight = DevExpress.Utils.DefaultBoolean.True;
            gridView1.OptionsBehavior.EditorShowMode = DevExpress.Utils.EditorShowMode.MouseDown;
        }

        /// <summary>
        /// 추가 - 새 행 추가
        /// </summary>
        private void fn_Add()
        {
            try
            {
                if (dtMenu == null)
                {
                    dtMenu = new DataTable();
                    dtMenu.Columns.Add("CHK", typeof(string));
                    dtMenu.Columns.Add("ID", typeof(int));
                    dtMenu.Columns.Add("P_ID", typeof(int));
                    dtMenu.Columns.Add("GUBUN", typeof(string));
                    dtMenu.Columns.Add("FORM_NAME", typeof(string));
                    dtMenu.Columns.Add("MENU_NAME", typeof(string));
                    dtMenu.Columns.Add("MENU_GROUP", typeof(string));
                    dtMenu.Columns.Add("ADMIN_YN", typeof(string));
                    dtMenu.Columns.Add("DOCK_OR_NOT", typeof(string));
                    dtMenu.Columns.Add("USE_FLAG", typeof(string));
                    gridControl1.DataSource = dtMenu;
                    fn_SetGridColumns();
                }

                DataRow newRow = dtMenu.NewRow();
                newRow["CHK"] = "True";
                newRow["ID"] = DBNull.Value;
                newRow["P_ID"] = DBNull.Value;
                newRow["GUBUN"] = "";
                newRow["FORM_NAME"] = "";
                newRow["MENU_NAME"] = "";
                newRow["MENU_GROUP"] = "";
                newRow["ADMIN_YN"] = "N";
                newRow["DOCK_OR_NOT"] = "N";
                newRow["USE_FLAG"] = "Y";
                dtMenu.Rows.Add(newRow);

                // 새 행으로 포커스 이동
                gridView1.FocusedRowHandle = gridView1.RowCount - 1;

                UpdateStatus?.Invoke("새 행이 추가되었습니다");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "추가 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 선택 여부 확인 헬퍼
        /// </summary>
        private bool IsChecked(DataRow row)
        {
            if (row.RowState == DataRowState.Deleted) return false;
            if (row["CHK"] == DBNull.Value) return false;
            return row["CHK"].ToString().Equals("True", StringComparison.OrdinalIgnoreCase)
                || gridView1.GetRowCellDisplayText(dtMenu.Rows.IndexOf(row), "CHK").Equals("Checked");
        }

        /// <summary>
        /// 삭제 - 선택된 행 XML 일괄 삭제
        /// </summary>
        private void fn_Delete()
        {
            try
            {
                gridView1.CloseEditor();
                gridView1.UpdateCurrentRow();

                if (dtMenu == null || dtMenu.Rows.Count == 0)
                {
                    MessageBox.Show("삭제할 데이터가 없습니다.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // 선택된 행 확인
                DataTable delDt = new DataTable("ROW");
                delDt.Columns.Add("ID", typeof(string));

                for (int i = 0; i < gridView1.RowCount; i++)
                {
                    if (gridView1.GetRowCellDisplayText(i, "CHK").Equals("Checked"))
                    {
                        string id = gridView1.GetRowCellDisplayText(i, "ID");
                        if (!string.IsNullOrEmpty(id))
                        {
                            DataRow delRow = delDt.NewRow();
                            delRow["ID"] = id;
                            delDt.Rows.Add(delRow);
                        }
                    }
                }

                if (delDt.Rows.Count == 0)
                {
                    // 신규 행만 체크된 경우 그리드에서만 제거
                    bool hasNewChecked = false;
                    for (int i = gridView1.RowCount - 1; i >= 0; i--)
                    {
                        if (gridView1.GetRowCellDisplayText(i, "CHK").Equals("Checked"))
                        {
                            string id = gridView1.GetRowCellDisplayText(i, "ID");
                            if (string.IsNullOrEmpty(id))
                            {
                                hasNewChecked = true;
                                gridView1.DeleteRow(i);
                            }
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

                if (MessageBox.Show($"선택한 {delDt.Rows.Count}건을 삭제하시겠습니까?", "삭제 확인", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    return;

                string xmlData = cm.DataTblToXML(delDt);

                string strSql = dbName + ".dbo.ST_TB_MENU_MST_DEL";
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
        /// 저장 - 변경된 행 일괄 저장 (XML 전달)
        /// </summary>
        private void fn_Save()
        {
            try
            {
                gridView1.CloseEditor();
                gridView1.UpdateCurrentRow();

                if (dtMenu == null || dtMenu.Rows.Count == 0)
                {
                    MessageBox.Show("저장할 데이터가 없습니다.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // 저장용 DataTable 구성
                DataTable saveDt = new DataTable("ROW");
                saveDt.Columns.Add("ID", typeof(string));
                saveDt.Columns.Add("P_ID", typeof(string));
                saveDt.Columns.Add("GUBUN", typeof(string));
                saveDt.Columns.Add("FORM_NAME", typeof(string));
                saveDt.Columns.Add("MENU_NAME", typeof(string));
                saveDt.Columns.Add("MENU_GROUP", typeof(string));
                saveDt.Columns.Add("ADMIN_YN", typeof(string));
                saveDt.Columns.Add("DOCK_OR_NOT", typeof(string));
                saveDt.Columns.Add("USE_FLAG", typeof(string));

                for (int i = 0; i < gridView1.RowCount; i++)
                {
                    // 선택(CHK)된 행만 저장
                    if (!gridView1.GetRowCellDisplayText(i, "CHK").Equals("Checked")) continue;

                    DataRow newRow = saveDt.NewRow();
                    newRow["ID"] = gridView1.GetRowCellDisplayText(i, "ID");
                    newRow["P_ID"] = gridView1.GetRowCellDisplayText(i, "P_ID");
                    newRow["GUBUN"] = gridView1.GetRowCellDisplayText(i, "GUBUN");
                    newRow["FORM_NAME"] = gridView1.GetRowCellDisplayText(i, "FORM_NAME");
                    newRow["MENU_NAME"] = gridView1.GetRowCellDisplayText(i, "MENU_NAME");
                    newRow["MENU_GROUP"] = gridView1.GetRowCellDisplayText(i, "MENU_GROUP");
                    newRow["ADMIN_YN"] = gridView1.GetRowCellDisplayText(i, "ADMIN_YN");
                    newRow["DOCK_OR_NOT"] = gridView1.GetRowCellDisplayText(i, "DOCK_OR_NOT");
                    newRow["USE_FLAG"] = gridView1.GetRowCellDisplayText(i, "USE_FLAG");
                    saveDt.Rows.Add(newRow);
                }

                if (saveDt.Rows.Count == 0)
                {
                    MessageBox.Show("저장할 데이터가 없습니다.\n변경된 행을 선택해주세요.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                string xmlData = cm.DataTblToXML(saveDt);

                string strSql = dbName + ".dbo.ST_TB_MENU_MST_SAVE";
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
