using DevExpress.Utils.Drawing.Helpers;
using DevExpress.XtraEditors.Repository;
using RAZER_C.Modules;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RAZER_C
{
    public partial class EXCEL_QR_MAPPER : Form
    {
        private static string dbName = "ERP_2";

        CommonModule cm = new CommonModule();

        MSSQL db = new MSSQL(dbName);

        DataRow PsDataRow;


        public EXCEL_QR_MAPPER()
        {
            InitializeComponent();

            initControl();
        }

        private void initControl()
        {
            try
            {
                tbx_emp_no.Select();

                tbx_emp_no.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) fn_FindEmpInfo(); };

                gridView1.InitNewRow += GridView1_InitNewRow;

                gridView1.CellValueChanged += GridView1_CellValueChanged;

                btn_Sel.Click += (s, e) => fn_BindingData();

                btn_Add.Click += (s,e) => gridView1.AddNewRow();

                btn_Del.Click += (s, e) => fn_Del();

                btn_Save.Click += (s, e) => fn_Save();

                tbx_BrandNM.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) fn_BindingData(); };

                tbx_CustNm.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) fn_BindingData(); };

                tbx_ColorNM.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) fn_BindingData(); };

                fn_BindingData();
            }
            catch(Exception ex)
            {

            }
        }


        private void GridView1_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                if (!gridView1.GetRowCellDisplayText(e.RowHandle, "SEL").Equals("Checked"))
                {
                   gridView1.SetRowCellValue(e.RowHandle, "SEL", "True");
                }
            }
            catch(Exception ex)
            {

            }
        }

        private void GridView1_InitNewRow(object sender, DevExpress.XtraGrid.Views.Grid.InitNewRowEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView gv = sender as DevExpress.XtraGrid.Views.Grid.GridView;

                int ridx = gv.RowCount == 1 ? 0 : gv.RowCount - 1;

                gv.SetRowCellValue(e.RowHandle, "SEL", "True");

                gv.SetRowCellValue(e.RowHandle, "ID", -1);

                gv.SetRowCellValue(e.RowHandle, "SEQ", ridx == 0 ? 1 : int.Parse(gv.GetRowCellDisplayText(ridx - 1, "SEQ")) + 1);

                gv.SetRowCellValue(e.RowHandle, "USE_FLAG", "Y");

                gv.SetRowCellValue(e.RowHandle, "CREATE_USER", tbx_emp_no.Text);


            }
            catch(Exception ex)
            {

            }
        }

        private void fn_FindEmpInfo()
        {
            try
            {
                if (string.IsNullOrEmpty(tbx_emp_no.Text))
                {
                    MessageBox.Show("사원번호를 입력해주십시오.", "사원번호 입력", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    return;
                }

                string strSql = $"{dbName}.dbo.ST_DANPLA_COLLECTOR_EMP_INFO_SEL";

                db.Parameter("@EMP_NO", tbx_emp_no.Text);

                db.ExecuteSql(strSql);

                if (db.nState)
                {
                    if (db.result.Rows.Count > 0)
                    {
                        PsDataRow = db.result.Rows[0];

                        lb_EmpName.Text = $"{PsDataRow["EMPNAME"].ToString()} / {PsDataRow["DEPTNAME"].ToString()}";

                        return;
                    }
                }

                MessageBox.Show($"{tbx_emp_no.Text}에 해당하는 사원정보가 없습니다.\n확인부탁드립니다.", "사원번호 입력", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                tbx_emp_no.Text = string.Empty;

                tbx_emp_no.SelectAll();

                return;
            }
            catch (Exception ex)
            {

            }
        }

        private void fn_Save()
        {
            try
            {
                if (MessageBox.Show("저장하시겠습니까?", "저장", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) return;

                if (string.IsNullOrEmpty(tbx_emp_no.Text))
                {
                    MessageBox.Show("사원번호를 입력해주십시오.", "사원번호 입력", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    tbx_emp_no.Select();

                    return;
                }

                string strSql = $"{dbName}.dbo.ST_EXCEL_QR_MAPPER_SAVE";

                DataTable dt = new DataTable();

                dt.Columns.Add("ID");
                dt.Columns.Add("CUST_NM");
                dt.Columns.Add("COLOR");
                dt.Columns.Add("BRAND_NM");
                dt.Columns.Add("SPEC");
                dt.Columns.Add("USE_FLAG");

                int cnt = gridView1.RowCount;

                for(int i=0;i<cnt;i++)
                {
                    if (string.IsNullOrEmpty(gridView1.GetRowCellDisplayText(i, "CUST_NM")) &&
                       string.IsNullOrEmpty(gridView1.GetRowCellDisplayText(i, "COLOR")) &&
                       string.IsNullOrEmpty(gridView1.GetRowCellDisplayText(i, "BRAND_NM")) &&
                       string.IsNullOrEmpty(gridView1.GetRowCellDisplayText(i, "SPEC"))) continue;

                    DataRow dr = dt.NewRow();
                    dr["ID"] = gridView1.GetRowCellDisplayText(i, "ID");
                    dr["CUST_NM"] = gridView1.GetRowCellDisplayText(i, "CUST_NM");
                    dr["COLOR"] = gridView1.GetRowCellDisplayText(i, "COLOR");
                    dr["BRAND_NM"] = gridView1.GetRowCellDisplayText(i, "BRAND_NM");
                    dr["SPEC"] = gridView1.GetRowCellDisplayText(i, "SPEC");
                    dr["USE_FLAG"] = gridView1.GetRowCellDisplayText(i, "USE_FLAG");
                    dt.Rows.Add(dr);
                }

                db.Parameter("@SV_XML",cm.DataTblToXML(dt));
                db.Parameter("@PS_CD", tbx_emp_no.Text);

                db.ExecuteNonSql(strSql);

                if (db.nState)
                {
                    if (!string.IsNullOrEmpty(db.sql_raise_error_msg))
                    {
                        MessageBox.Show($"{db.sql_raise_error_msg}\n정보전략팀에 문의 바랍니다.", "저장", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                        return;
                    }

                    MessageBox.Show($"저장되었습니다.", "저장", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    fn_BindingData();
                }
            }
            catch(Exception ex)
            {

            }
        }

        private void fn_Del()
        {
            try
            {
                if (MessageBox.Show("삭제하시겠습니까?", "삭제", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) return;

                DataTable deldt = new DataTable();

                deldt.Columns.Add("ID");

                int rCnt = gridView1.RowCount;

                for (int i = 0; i < rCnt; i++)
                {
                    if (gridView1.GetRowCellDisplayText(i, "SEL").Equals("Checked"))
                    {
                        DataRow dr = deldt.NewRow();

                        dr["ID"] = gridView1.GetRowCellDisplayText(i, "ID");

                        deldt.Rows.Add(dr);
                    }
                }

                if (deldt.Rows.Count > 0)
                {

                    string strSql = $"{dbName}.dbo.ST_EXCEL_QR_MAPPER_DEL";

                    db.Parameter("@DEL_XML", cm.DataTblToXML(deldt));

                    db.ExecuteNonSql(strSql);

                    if (db.nState)
                    {
                        if (string.IsNullOrEmpty(db.sql_raise_error_msg))
                        {
                            MessageBox.Show("삭제되었습니다.", "삭제", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                            fn_BindingData();
                        }
                    }
                    return;
                }

            }
            catch(Exception ex)
            {

            }
        }

        private void fn_BindingData()
        {
            try
            {
                string strSql = $"{dbName}.dbo.ST_EXCEL_QR_MAPPER_SEL";

                db.Parameter("@CUST_NM", tbx_CustNm.Text);
                db.Parameter("@COLOR", tbx_ColorNM.Text);
                db.Parameter("@BRAND_NM", tbx_BrandNM.Text);

                db.ExecuteSql(strSql);

                if (db.nState)
                {
                    gridControl1.DataSource = db.result;

                    gridView1.Columns["ID"].Visible = false;

                    gridView1.Columns["SEQ"].Caption = "순번";
                    gridView1.Columns["SEL"].Caption = "선택";
                    gridView1.Columns["CUST_NM"].Caption = "거래처명";
                    gridView1.Columns["COLOR"].Caption = "컬러";
                    gridView1.Columns["BRAND_NM"].Caption = "브랜드명";
                    gridView1.Columns["SPEC"].Caption = "SPEC";
                    gridView1.Columns["USE_FLAG"].Caption = "사용유무";
                    gridView1.Columns["CREATE_USER"].Caption = "생성자";
                    gridView1.Columns["CREATE_DT"].Caption = "생성시간";
                    gridView1.Columns["MODIFY_USER"].Caption = "수정자";
                    gridView1.Columns["MODIFY_DT"].Caption = "수정시간";

                    gridView1.Columns["SEQ"].Width = 50;
                    gridView1.Columns["SEL"].Width = 50;
                    gridView1.Columns["CUST_NM"].Width = 200;
                    gridView1.Columns["COLOR"].Width = 150;
                    gridView1.Columns["BRAND_NM"].Width = 300;
                    gridView1.Columns["SPEC"].Width = 180;
                    gridView1.Columns["USE_FLAG"].Width = 50;
                    gridView1.Columns["CREATE_USER"].Width = 80;
                    gridView1.Columns["CREATE_DT"].Width = 120;
                    gridView1.Columns["MODIFY_USER"].Width = 80;
                    gridView1.Columns["MODIFY_DT"].Width = 120;

                    gridView1.Columns["CREATE_USER"].OptionsColumn.AllowEdit = false;
                    gridView1.Columns["CREATE_DT"].OptionsColumn.AllowEdit = false;
                    gridView1.Columns["MODIFY_USER"].OptionsColumn.AllowEdit = false;
                    gridView1.Columns["MODIFY_DT"].OptionsColumn.AllowEdit = false;

                    RepositoryItemCheckEdit ri = new RepositoryItemCheckEdit();

                    ri.ValueChecked = "True";

                    ri.ValueUnchecked = "False";

                    ri.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;

                    ri.CheckBoxOptions.Style = DevExpress.XtraEditors.Controls.CheckBoxStyle.SvgCheckBox1;

                    gridView1.Columns["SEL"].ColumnEdit = ri;

                    RepositoryItemComboBox combo = new RepositoryItemComboBox();
                    combo.Items.Add("Y");
                    combo.Items.Add("N");

                    gridView1.Columns["USE_FLAG"].ColumnEdit = combo;


                    gridView1.OptionsBehavior.EditorShowMode = DevExpress.Utils.EditorShowMode.MouseDown;

                    gridView1.OptionsView.ShowIndicator = false;

                    gridView1.OptionsView.ColumnHeaderAutoHeight = DevExpress.Utils.DefaultBoolean.True;
                }

            }
            catch(Exception ex)
            {

            }
        }
    }
}
