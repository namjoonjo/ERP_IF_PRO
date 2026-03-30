using COMBINATION.Modules;
using DevExpress.Xpo.DB;
using DevExpress.XtraEditors.Filtering.Templates;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace COMBINATION
{
    public partial class PRODUCTION_ANALYSIS : Form
    {
        public Action<string> UpdateStatus { get; set; }

        private static string dbName = "ERP_2";

        MSSQL db = new MSSQL(dbName);

        SQLITE3 sdb = null;

        DataTable ErpData;

        CommonModule cm = new CommonModule();

        List<ProductionRecords> plist = new List<ProductionRecords>();

        private Dictionary<string, string> cboWorker = new Dictionary<string, string>();

        private Dictionary<string, string> cboKind = new Dictionary<string, string>();

        public PRODUCTION_ANALYSIS()
        {
            InitializeComponent();

            initControl();
        }

        private void initControl()
        {
            try
            {

                fn_ComboInit();

                fn_bindingRegiData();

                fn_bindingRegiHISTORYData();

                SetGridRowHeader(grid_State, 35, true);

                SetGridRowHeader(grid_State2, 35, true);
                 
                this.procTimer.Tick += (s, e) => { fn_StartProcessing(); };

                btn_Status.Click += (s, e) => { fn_StartTransfer(); };

                btn_Status.MouseEnter += (s, e) => { btn_Status.ForeColor = Color.Orange; };

                btn_Status.MouseLeave += (s, e) => { btn_Status.ForeColor = Color.Black; };

                btn_find.Click += (s, e) => { fn_GetFileRoute(); };

                cbx_Kind.SelectedIndexChanged += (s, e) => { fn_bindingRegiData(); };

                cbx_Worker.SelectedIndexChanged += (s, e) => { fn_bindingRegiData(); };

                StartDate.ValueChanged += (s, e) => { fn_bindingRegiData(); };

                this.FormClosing += (s, e) => { if (procTimer.Enabled) { procTimer.Stop(); } db.Close(); };

            }
            catch(Exception ex)
            {

            }
        }

        private void fn_GetFileRoute()
        {
            try
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Title = "SqlLite3 db file 찾기";
                ofd.Filter = "(*.db;) | *.db";

                DialogResult dr = ofd.ShowDialog();

                if (dr == DialogResult.OK)
                {
                    tbx_FileRoute.Text = ofd.FileName;

                    this.sdb = new SQLITE3($"Data Source={tbx_FileRoute.Text};");
                }
            }
            catch(Exception ex)
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
                    if (db.result.Rows.Count > 0)
                    {

                        DataRow[] drKind = db.result.Select($"KIND = 'COMBO_1'");

                        foreach(DataRow ddr in drKind)
                        {
                            string[] spliters = ddr["COMBO_STR"].ToString().Split('/');

                            if (!cboKind.ContainsKey(spliters[0])) cboKind.Add(spliters[0], spliters[1]);

                            cbx_Kind.Items.Add(spliters[0]);
                        }

                        DataRow[] drWorkers = db.result.Select($"KIND = 'COMBO_2'");

                        foreach (DataRow ddr in drWorkers)
                        {
                            string[] spliters = ddr["COMBO_STR"].ToString().Split('/');

                            if (!cboWorker.ContainsKey(spliters[0])) cboWorker.Add(spliters[0], spliters[1]);

                            cbx_Worker.Items.Add(spliters[0]);
                        }

                        cbx_Kind.SelectedIndex = cbx_Kind.Items.Count > 0 ? 0 : cbx_Kind.SelectedIndex;

                        cbx_Worker.SelectedIndex = cbx_Worker.Items.Count > 0 ? 0 : cbx_Worker.SelectedIndex;
                    }
                }
            }
            catch(Exception ex)
            {

            }
        }


        private void fn_StartTransfer()
        {
            try
            {
                if(btn_Status.Text.Equals("전송"))
                {
                    
                    if (MessageBox.Show("생산실적 이력이 전송됩니다. 진행하시겠습니까?","전송",MessageBoxButtons.YesNo,MessageBoxIcon.Question) == DialogResult.No) return;

                    if (!File.Exists(tbx_FileRoute.Text))
                    {
                        MessageBox.Show($"{tbx_FileRoute.Text}의 경로에 파일이 없습니다.\n확인부탁드립니다.", "전송", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                        return;
                    }

                    btn_Status.Text = "정지";

                    fn_SetEnable(false);

                    procTimer.Interval = Convert.ToInt32(CycleTime.Value) * 1000 * 60;

                    fn_StartProcessing();

                    procTimer.Start();

                    return;
                }

                if (MessageBox.Show("생산실적 이력전송이 진행중입니다. 멈추시겠습니까?", "전송취소", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    btn_Status.Text = "전송";

                    fn_SetEnable(true);

                    procTimer.Stop();

                    return;
                }


            }
            catch (Exception ex)
            {
                UpdateStatus?.Invoke($"fn_StartTransfer {ex.Message}");
            }

        }

        private void fn_SetEnable(bool flag)
        {
            try
            {
                this.StartDate.Enabled = flag;

                this.EndDate.Enabled = flag;

                this.tbx_FileRoute.Enabled = flag;

                this.cbx_Kind.Enabled = flag;

                this.cbx_Worker.Enabled = flag;

                this.btn_find.Enabled = flag;

                this.CycleTime.Enabled = flag;
            }
            catch(Exception ex)
            {

            }
        }

        private void fn_StartProcessing()
        {
            try
            {
                EndDate.Value = DateTime.Now;

                fn_bindingRegiData();

                plist.Clear();

                foreach (DataRow dr in ErpData.Rows)
                {
                    plist.Add(new ProductionRecords(dr["PR_DT"].ToString(), dr["LOT_NO"].ToString(), dr["GD_CD"].ToString(), dr["GD_NM"].ToString(), dr["PR_QTY"].ToString()));
                }

                if (this.sdb == null) this.sdb = new SQLITE3($"Data Source={tbx_FileRoute.Text};");

                this.sdb.ExecuteNonSqlForInsert(plist);

                if (!sdb.nState || !string.IsNullOrEmpty(sdb.sql_raise_error_msg))
                {
                    UpdateStatus?.Invoke($"fn_StartProcessing {sdb.sql_raise_error_msg}");
                }

                fn_AddHistory();

                if (!sdb.nState)
                {
                    if (procTimer.Enabled) procTimer.Stop();

                    btn_Status.Text = "전송";

                    UpdateStatus?.Invoke($"SQLite3 값을 넣는 도중 오류가 발생하였습니다.{sdb.sql_raise_error_msg}");
                }

            }
            catch (Exception ex) 
            {

            }
        }

        private void fn_AddHistory()
        {
            try
            {
                string strSql = $"{dbName}.dbo.ST_COMBI_PRODUCTION_ANALYSIS_HI_INS";

                db.Parameter("@EMP_NO", cboWorker[cbx_Worker.Text]);
                db.Parameter("@CYCLETIME_MIN", Convert.ToInt32(CycleTime.Value));
                db.Parameter("@FROMDATETODATE", $"{StartDate.Value} ~ {EndDate.Value}");

                db.ExecuteNonSql(strSql);

                if (!string.IsNullOrEmpty(db.sql_raise_error_msg))
                {
                    UpdateStatus?.Invoke(db.sql_raise_error_msg);
                }

                fn_bindingRegiHISTORYData();

            }
            catch (Exception ex)
            {

            }
        }

        private void fn_bindingRegiData()
        {
            try
            {
                if(DateTime.Now < StartDate.Value)
                {
                    MessageBox.Show("시작날짜가 현재날짜보다 클 수 없습니다.\n확인부탁드립니다.", "경고", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    StartDate.Value = DateTime.Now;

                    return;
                }


                string strSql = $"{dbName}.dbo.ST_COMBI_PRODUCTION_ANALYSIS_SEL";

                db.Parameter("@START_DATE", StartDate.Value.ToString("yyyy-MM-dd"));
                db.Parameter("@END_DATE", EndDate.Value.ToString("yyyy-MM-dd"));
                db.Parameter("@KIND", cboKind[cbx_Kind.Text]);

                db.ExecuteSql(strSql);

                if (db.nState)
                {
                    if (!string.IsNullOrEmpty(db.sql_raise_error_msg))
                    {
                        procTimer.Stop();

                        btn_Status.Text = "전송";
                        
                        UpdateStatus?.Invoke($"{db.sql_raise_error_msg}의 오류로 중단되었습니다.");

                        return;
                    }

                    grid_State.DataSource = db.result;

                    ErpData = db.result.Copy();

                    grid_State.Columns["PR_NO"].HeaderText = "실적번호";
                    grid_State.Columns["PR_DT"].HeaderText = "생산일자";
                    grid_State.Columns["LOT_NO"].HeaderText = "LotNo";
                    grid_State.Columns["GD_CD"].HeaderText = "품목코드";
                    grid_State.Columns["GD_NM"].HeaderText = "품명";
                    grid_State.Columns["PR_QTY"].HeaderText = "양품수량";
                    grid_State.Columns["SPEC"].HeaderText = "스펙";

                    grid_State.Columns["PR_NO"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    grid_State.Columns["PR_DT"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    grid_State.Columns["LOT_NO"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    grid_State.Columns["GD_CD"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    grid_State.Columns["GD_NM"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    grid_State.Columns["PR_QTY"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    grid_State.Columns["SPEC"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                }

            } 
            catch(Exception ex)
            {
              
            }
        }

        private void fn_bindingRegiHISTORYData()
        {
            try
            {
                string strSql = $"{dbName}.dbo.ST_COMBI_PRODUCTION_ANALYSIS_HI_SEL";

                db.ExecuteSql(strSql);

                if (db.nState)
                {
                    grid_State2.DataSource = db.result;

                    grid_State2.Columns["EMP_NO"].HeaderText = "작업자";
                    grid_State2.Columns["CYCLETIME_MIN"].HeaderText = "주기\n(분)";
                    grid_State2.Columns["FROMDATETODATE"].HeaderText = "생산실적\n시간범위";
                    grid_State2.Columns["IN_DT"].HeaderText = "이전시간";
     
                    grid_State2.Columns["EMP_NO"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    grid_State2.Columns["CYCLETIME_MIN"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    grid_State2.Columns["FROMDATETODATE"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    grid_State2.Columns["IN_DT"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
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
