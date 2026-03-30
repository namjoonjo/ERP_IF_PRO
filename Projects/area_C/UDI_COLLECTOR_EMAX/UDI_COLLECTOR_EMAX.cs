using DevExpress.XtraBars.InternalItems;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Filtering.Templates;
using DevExpress.XtraGrid.Views.Grid;
using RAZER_C.Modules;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RAZER_C
{
    public partial class UDI_COLLECTOR_EMAX : Form
    {

        private static string dbName = "ERP_2";

        MSSQL db = new MSSQL(dbName);

        CommonModule cm = new CommonModule();

        DataTable detail = new DataTable();

        private Dictionary<string, string> fMap = new Dictionary<string, string>();

        private string MasterStr = string.Empty; private string SubLotStr = "-";

        public Action<string> UpdateStatus { get; set; }

        public UDI_COLLECTOR_EMAX()
        {
            InitializeComponent();

            initControl();
        }

        private void initControl()
        {
            try
            {
                cm.SetGridRowHeader(grid_Detail, -1, true);

                cm.SetGridRowHeader(grid_State, -1, true);

                cm.SetGridRowHeader(grid_State2, -1, true);

                tbx_Empno.Select();

                tbx_INITI.Enabled = false;

                tbx_Empno.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) fn_EmpSel(); };

                tbx_LotNo.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) fn_LotNoProcessing(); };

                tbx_UDI.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) fn_UDIProcessing(); };

                tbx_ReNo.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) fn_ReDataSel(); };

                tbx_INITI.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) fn_initiSel(); };

                btn_10.Click += Btn_Click;
                btn_11.Click += Btn_Click;
                btn_17.Click += Btn_Click;

                fn_Comboinit();

                detail.Columns.Add("SEQ");
                detail.Columns.Add("RE_SER");
                detail.Columns.Add("UDI");

                fn_DetailBinding();

                btn_DetailDel.Click += (s, e) => { fn_DetailDel(); };

                btn_Reset.Click += (s, e) => { fn_Reset(); };

                btn_Save.Click += (s, e) => { fn_Save(); };

                cbx_Kind.SelectedValueChanged += Cbx_Kind_SelectedValueChanged;

            }
            catch (Exception ex)
            {

            }
        }

        private void Cbx_Kind_SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {
                switch (cbx_Kind.Text)
                {
                    case "해외소프트렌즈":

                        tbx_INITI.Enabled = true;

                        tbx_INITI.Select();

                        btn_10.BackColor = Color.FromArgb(0, 192, 0);

                        btn_10.ForeColor = Color.Black;

                        btn_11.BackColor = Color.FromArgb(0, 192, 0);

                        btn_11.ForeColor = Color.Black;

                        btn_17.BackColor = Color.FromArgb(0, 192, 0);

                        btn_17.ForeColor = Color.Black;

                        break;

                    case "국내소프트렌즈":

                        tbx_INITI.Enabled = false;

                        tbx_LotNo.Select();

                        btn_10.BackColor = Color.FromArgb(0, 192, 0);

                        btn_10.ForeColor = Color.Black;

                        btn_11.BackColor = Color.FromArgb(0, 192, 0);

                        btn_11.ForeColor = Color.Black;

                        btn_17.BackColor = Color.FromArgb(0, 192, 0);

                        btn_17.ForeColor = Color.Black;

                        break;

                    case "국내하드렌즈":

                        tbx_INITI.Enabled = false;

                        tbx_LotNo.Select();

                        btn_10.BackColor = Color.Red;

                        btn_10.ForeColor = Color.Yellow;

                        btn_11.BackColor = Color.Red;

                        btn_11.ForeColor = Color.Yellow;

                        btn_17.BackColor = Color.Red;

                        btn_17.ForeColor = Color.Yellow;

                        break;
                }
            }
            catch(Exception ex)
            {

            }
        }

        private void fn_DetailBinding()
        {
            try
            {
                grid_Detail.DataSource = detail;

                grid_Detail.Columns["SEQ"].HeaderText = "순번";
                grid_Detail.Columns["RE_SER"].HeaderText = "레이저마킹 순번";
                grid_Detail.Columns["UDI"].HeaderText = "UDI";

                grid_Detail.Columns["SEQ"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                grid_Detail.Columns["RE_SER"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                grid_Detail.Columns["UDI"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            }
            catch(Exception ex)
            {

            }
        }

        private void fn_Save()
        {
            try
            {
                if (string.IsNullOrEmpty(tbx_Empno.Text))
                {
                    MessageBox.Show("사원정보를 입력해 주십시오.", "경고", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    tbx_Empno.Select();

                    return;
                }

                if (string.IsNullOrEmpty(tbx_LotNo.Text))
                {
                    MessageBox.Show("LotNo를 입력해 주십시오.", "경고", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    tbx_LotNo.Select();

                    return;
                }

                if(cbx_Kind.Text.Equals("해외소프트렌즈") && string.IsNullOrEmpty(tbx_INITI.Text))
                {
                    MessageBox.Show("이니셜 정보를 입력해주시기 바랍니다.", "경고", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    tbx_INITI.Select();

                    return;
                }


                if (detail.Rows.Count == 0)
                {
                    MessageBox.Show("스캔된 UDI정보가 없습니다.\n확인부탁드립니다.", "경고", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    return;
                }

                if (MessageBox.Show("UDI정보들을 저장하시겠습니까?", "저장", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) return;

                DataTable sdt = new DataTable();

                sdt.Columns.Add("RE_SEQ");
                sdt.Columns.Add("UDI");

                for(int i = 0; i < detail.Rows.Count; i++)
                {
                    DataRow nsdt = sdt.NewRow();

                    nsdt["RE_SEQ"] = i;

                    nsdt["UDI"] = detail.Rows[i]["UDI"].ToString();

                    sdt.Rows.Add(nsdt);
                }

                string strSql = $"{dbName}.dbo.ST_UDI_COLLECTOR_EMAX_SAVE";

                db.Parameter("@PS_CD", tbx_Empno.Text);
                db.Parameter("@GUBUN", cbx_Kind.Text);
                db.Parameter("@INITI", tbx_INITI.Text);
                db.Parameter("@LOT_NO", tbx_LotNo.Text);
                db.Parameter("@FAC_CD", fMap[cbx_Location.Text]);
                db.Parameter("@UDI_XML", cm.DataTblToXML(sdt));
                db.OutputParameter("@RE_NO");
               
                db.ExecuteNonSql(strSql);

                if (db.nState)
                {
                    if (string.IsNullOrEmpty(db.sql_raise_error_msg))
                    {
                        tbx_ReNo.Text = db.OutputParameterMapper["@RE_NO"];

                        MessageBox.Show("저장되었습니다.", "저장", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        fn_ReDataSel();

                        fn_Reset();

                        return;
                    }
                    else
                    {
                        MessageBox.Show($"{db.sql_raise_error_msg}", "에러", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        return;
                    }
                }
                
                
            }
            catch(Exception ex)
            {

            }
        }

        private void fn_initiSel()
        {
            try
            {
                if (string.IsNullOrEmpty(tbx_INITI.Text))
                {
                    MessageBox.Show("이니셜 정보를 입력해주시기 바랍니다.", "이니셜정보 입력", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    return;
                }

                tbx_INITI.Enabled = false;

                tbx_LotNo.Select();
            }
            catch(Exception ex)
            {

            }
        }

        private void fn_ReDataSel()
        {
            try
            {
                string strSql = $"{dbName}.dbo.ST_UDI_COLLECTOR_EMAX_RE_DATA_SEL";

                db.Parameter("@RE_NO", tbx_ReNo.Text);
                db.Parameter("@GUBUN", cbx_Kind.Text);

                db.ExecuteSql(strSql);

                if (db.nState)
                {
                    grid_State.DataSource = db.result;

                    grid_State.Columns["RE_NO"].HeaderText = "검수번호";
                    grid_State.Columns["RE_DT"].HeaderText = "검수일자";
                    grid_State.Columns["LOT_NO"].HeaderText = "LotNo";
                    grid_State.Columns["PS_CD"].HeaderText = "사원번호";

                    grid_State.Columns["RE_NO"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    grid_State.Columns["RE_DT"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    grid_State.Columns["LOT_NO"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    grid_State.Columns["PS_CD"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

                    fn_ReDataDetailSel();
                }
            }
            catch(Exception ex)
            {

            }
        }

        private void fn_ReDataDetailSel()
        {
            try
            {
                string strSql = $"{dbName}.dbo.ST_UDI_COLLECTOR_EMAX_RE_DATA_DETAIL_SEL";

                db.Parameter("@RE_NO", tbx_ReNo.Text);
                db.Parameter("@GUBUN", cbx_Kind.Text);

                db.ExecuteSql(strSql);

                if (db.nState)
                {
                    grid_State2.DataSource = db.result;

                    grid_State2.Columns["RE_NO"].HeaderText = "검수번호";
                    grid_State2.Columns["RE_SER"].HeaderText = "끝 순번 5자리";
                    grid_State2.Columns["UDI"].HeaderText = "UDI";

                    grid_State2.Columns["RE_NO"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    grid_State2.Columns["RE_SER"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    grid_State2.Columns["UDI"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                }
            }
            catch(Exception ex)
            {

            }
        }

        private void Btn_Click(object sender, EventArgs e)
        {
            try
            {
                Button btn = (Button)sender;

                if(btn.BackColor == Color.FromArgb(0, 192, 0))
                {
                    btn.BackColor = Color.Red;

                    btn.ForeColor = Color.Yellow;
                }
                else
                {
                    btn.BackColor = Color.FromArgb(0, 192, 0);

                    btn.ForeColor = Color.Black;
                }
            }
            catch(Exception ex)
            {
                
            }
        }

        private void fn_EmpSel()
        {
            try
            {
                if (string.IsNullOrEmpty(tbx_Empno.Text))
                {
                    MessageBox.Show("사원정보를 입력해 주십시오.", "경고", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    tbx_Empno.Select();

                    return;
                }

                string strSql = $"{dbName}.dbo.ST_UDI_COLLECTOR_EMAX_EMPDATA_SEL";

                db.Parameter("@EMP_NO", tbx_Empno.Text);

                db.ExecuteSql(strSql);

                if (db.nState)
                {
                    if(db.result.Rows.Count > 0)
                    {
                        DataRow rdr = db.result.Rows[0];

                        lb_Empnm.Text = $"{rdr["EMP_NM"].ToString()} / {rdr["DEPT_NM"].ToString()}";

                        tbx_LotNo.Select();

                        return;
                    }

                    MessageBox.Show("일치하는 사원정보가 없습니다.\n확인부탁드립니다.", "경고", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    return;
                }
            }
            catch(Exception ex)
            {

            }
        }

        private void fn_Comboinit()
        {
            try
            {
                string strSql = $"{dbName}.dbo.ST_UDI_COLLECTOR_COMBOBOX_DATA_SEL";

                db.Parameter("@F_NAME", this.Name);

                db.ExecuteSql(strSql);

                if (db.nState)
                {
                    cm.ComboBoxBinding(db.result, cbx_Kind, "UDI_KIND");

                    //cm.ComboBoxBinding(db.result, cbx_Location, "FAC_CD");

                    DataRow[] dr = db.result.Select($"KIND = 'FAC_CD'");

                    foreach (DataRow ddr in dr)
                    {
                        string[] spliters = ddr["COMBO_STR"].ToString().Split('/');

                        cbx_Location.Items.Add(spliters[0]);

                        fMap.Add(spliters[0], spliters[1]);
                    }

                    cbx_Kind.SelectedIndex = 0; cbx_Location.SelectedIndex = 1;
                }
            }
            catch(Exception ex)
            {

            }
        }

        private void fn_LotNoProcessing()
        {
            try
            {
                if (string.IsNullOrEmpty(lb_Empnm.Text))
                {
                    MessageBox.Show("사번을 입력해 주시기 바랍니다.", "LOT_NO 입력", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    tbx_LotNo.Text = string.Empty;

                    tbx_Empno.Select();

                    return;
                }

                if (string.IsNullOrEmpty(tbx_LotNo.Text))
                {
                    MessageBox.Show("LotNo를 입력해 주십시오.", "경고", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    tbx_LotNo.Select();

                    return;
                }

                tbx_LotNo.Text = tbx_LotNo.Text.Trim().ToUpper();

                if (!fn_ChkLotNo())
                {
                    MessageBox.Show("확인할 수 없는 Lot No입니다.\n확인부탁드립니다.", "LOT_NO 입력", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    tbx_LotNo.SelectAll();

                    return;
                }

                tbx_LotNo.Enabled = false;

                tbx_UDI.Select();
            }
            catch(Exception ex )
            {

            }
        }

        private bool fn_ChkLotNo()
        {
            try
            {
                string strSql = $"{dbName}.dbo.ST_UDI_COLLECTOR_EMAX_LOT_NO_SEL";

                db.Parameter("@LOT_NO", tbx_LotNo.Text);

                db.ExecuteSql(strSql);

                if (db.nState)
                {
                    if (db.result.Rows.Count > 0) return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private void fn_Reset()
        {
            try
            {
                detail.Rows.Clear();

                lb_DetailCnt.Text = $"검수 수량: {detail.Rows.Count}";

                fn_DetailBinding();

                MasterStr = string.Empty;

                tbx_INITI.Text = string.Empty;

                tbx_LotNo.Text = string.Empty;
                    
                tbx_UDI.Text = string.Empty;

                SubLotStr = "-";

                if (cbx_Kind.Text.Equals("해외소프트렌즈"))
                {
                    tbx_INITI.Enabled = true;

                    tbx_LotNo.Enabled = true;

                    tbx_INITI.Select();
                }
                else
                {
                    tbx_LotNo.Enabled = true;

                    tbx_LotNo.Select();
                }

            }
            catch(Exception ex)
            {
                
            }
        }

        private void fn_UDIProcessing()
        {
            try
            {
                if (string.IsNullOrEmpty(tbx_UDI.Text))
                {
                    MessageBox.Show("UDI 입력 칸이 비어있습니다.\n확인부탁드립니다.", "UDI 입력", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    tbx_UDI.Focus();

                    return;
                }

                if (string.IsNullOrEmpty(tbx_LotNo.Text))
                {
                    MessageBox.Show("LotNo 입력 칸이 비어있습니다.\n확인부탁드립니다.", "UDI 입력", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    tbx_LotNo.Focus();

                    return;
                }

                if (tbx_LotNo.Enabled)
                {
                    MessageBox.Show("검증된 LotNo가 아닙니다.\n확인부탁드립니다.", "UDI 입력", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    tbx_LotNo.Focus();

                    return;
                }

                tbx_UDI.Text = tbx_UDI.Text.Trim().ToUpper();

                if (tbx_UDI.Text.Length <= 8)
                {
                    MessageBox.Show("스캔된 LotNo가 UDI가 아닙니다.\n확인부탁드립니다.", "UDI 입력", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    tbx_LotNo.Focus();

                    return;
                }

                if (chkExistUDI())
                {
                    UpdateStatus?.Invoke("중복된 UDI가 있습니다.");

                    tbx_UDI.SelectAll();

                    return;
                }

                string dividedStrfromUDI = fn_DivideUDI();

                if (dividedStrfromUDI.Equals("-1"))
                {
                    MessageBox.Show("눌러진 버튼의 구분자와 실제 UDI 구분자가 다릅니다.\n확인부탁드립니다.", "UDI 입력", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    return;
                }

                if (!fn_ChkSubLot())
                {
                    MessageBox.Show($"체크시트와 UDI의 서브로트가 일치하지 않습니다.\n체크시트 : {tbx_LotNo.Text}\n서브로트 : {SubLotStr}", "UDI 입력", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    return;
                }

                if (detail.Rows.Count == 0) MasterStr = dividedStrfromUDI;
     
                if (!MasterStr.Equals(dividedStrfromUDI))
                {
                    MessageBox.Show("UDI 데이터는 끝 5자리를 제외하고 모두 동일하여야 합니다.\n확인부탁드립니다.", "경고", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    return;
                }

                fn_DetailAdd();

                tbx_UDI.SelectAll();
            }
            catch (Exception ex)
            {
                
            }
        }

        private bool fn_ChkSubLot()
        {
            try
            {
                if (SubLotStr.Equals("-"))
                {
                    MessageBox.Show("SUBLOT값을 구할 수 없습니다.\nUDI를 확인해주시기 바랍니다.", "경고", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    return false;
                }

                string strSql = $"{dbName}.dbo.ST_UDI_COLLECTOR_EMAX_SUBLOT_SEL";

                char[] fLotNo = tbx_LotNo.Text.Substring(0, 1).ToCharArray();

                string[] spliters = tbx_LotNo.Text.Split('-');

                if('A' <= fLotNo[0] && fLotNo[0] <= 'Z')
                {
                    db.Parameter("@LOT_YEAR", tbx_LotNo.Text.Substring(1, 4));
                    db.Parameter("@LOT_MON", tbx_LotNo.Text.Substring(5, 2));
                    db.Parameter("@LOT_DAY", tbx_LotNo.Text.Substring(7, 2));
                }
                else
                {
                    db.Parameter("@LOT_YEAR", tbx_LotNo.Text.Substring(0, 4));
                    db.Parameter("@LOT_MON", tbx_LotNo.Text.Substring(4, 2));
                    db.Parameter("@LOT_DAY", tbx_LotNo.Text.Substring(6, 2));
                }

                db.Parameter("@LOT_LAST", spliters[1]);
                db.Parameter("@UDI_SUBLOT1", SubLotStr.Substring(0, 3));
                db.Parameter("@UDI_SUBLOT2", cm.Right(SubLotStr, 3));

                db.ExecuteSql(strSql);

                if (db.nState)
                {
                    if(db.result.Rows.Count > 0) 
                    {
                        if (db.result.Rows[0]["RS"].ToString().Equals("OK")) return true;

                        else return false;
                    }
                }

                return false;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        

        private void fn_DetailAdd()
        {
            try
            {
                DataRow adddr = detail.NewRow();

                adddr["SEQ"] = detail.Rows.Count == 0 ? 0 : Convert.ToInt32(detail.Rows[detail.Rows.Count - 1]["SEQ"]) + 1;

                adddr["RE_SER"] = cm.Right(tbx_UDI.Text, 5);

                adddr["UDI"] = tbx_UDI.Text;

                detail.Rows.Add(adddr);

                fn_DetailBinding();

                lb_DetailCnt.Text = $"검수 수량: {detail.Rows.Count}";
            }
            catch(Exception ex)
            {

            }
        }

        private void fn_DetailDel()
        {
            try
            {
                if (grid_Detail.Rows.Count == 0 || grid_Detail.SelectedRows.Count == 0) return;

                foreach(DataRow dr in detail.Rows)
                {
                    if (dr["UDI"].ToString().Equals(grid_Detail.SelectedRows[0].Cells["UDI"].Value.ToString()))
                    {
                        detail.Rows.Remove(dr);

                        break;
                    }
                }

                lb_DetailCnt.Text = $"검수 수량: {detail.Rows.Count}";
            }
            catch(Exception ex)
            {

            }
        }

        private string fn_DivideUDI()
        {
            try
            {


                if(btn_10.BackColor == Color.FromArgb(0, 192, 0))
                {
                    if(btn_11.BackColor == Color.FromArgb(0, 192, 0))
                    {
                        if(btn_17.BackColor == Color.FromArgb(0, 192, 0))
                        {
                            if (!tbx_UDI.Text.Substring(0, 2).Equals("01") || !tbx_UDI.Text.Substring(16, 2).Equals("10") || !tbx_UDI.Text.Substring(22, 2).Equals("11") || !tbx_UDI.Text.Substring(30, 2).Equals("17")) return "-1";

                            SubLotStr = tbx_UDI.Text.Substring(40, 6);

                            //return tbx_UDI.Text.Substring(0, 38);
                        }

                        else
                        {
                            if (!tbx_UDI.Text.Substring(0, 2).Equals("01") || !tbx_UDI.Text.Substring(16, 2).Equals("10") || !tbx_UDI.Text.Substring(22, 2).Equals("11")) return "-1";

                            SubLotStr = tbx_UDI.Text.Substring(32, 6);

                            //return tbx_UDI.Text.Substring(0, 30);
                        }
                    }
                    else
                    {
                        if (btn_17.BackColor == Color.FromArgb(0, 192, 0))
                        {
                            if (!tbx_UDI.Text.Substring(0, 2).Equals("01") || !tbx_UDI.Text.Substring(16, 2).Equals("10") || !tbx_UDI.Text.Substring(22, 2).Equals("17")) return "-1";

                            SubLotStr = tbx_UDI.Text.Substring(32, 6);

                            //return tbx_UDI.Text.Substring(0, 30);
                        }

                        else
                        {
                            if (!tbx_UDI.Text.Substring(0, 2).Equals("01") || !tbx_UDI.Text.Substring(16, 2).Equals("10")) return "-1";

                            SubLotStr = tbx_UDI.Text.Substring(24, 6);

                            //return tbx_UDI.Text.Substring(0, 22);
                        }
                    }
                }
                else
                {
                    if (btn_11.BackColor == Color.FromArgb(0, 192, 0))
                    {
                        if (btn_17.BackColor == Color.FromArgb(0, 192, 0))
                        {
                            if (!tbx_UDI.Text.Substring(0, 2).Equals("01") || !tbx_UDI.Text.Substring(16, 2).Equals("11") || !tbx_UDI.Text.Substring(24, 2).Equals("17")) return "-1";

                            SubLotStr = tbx_UDI.Text.Substring(34, 6);

                            //return tbx_UDI.Text.Substring(0, 32);
                        }

                        else
                        {
                            if (!tbx_UDI.Text.Substring(0, 2).Equals("01") || !tbx_UDI.Text.Substring(16, 2).Equals("11")) return "-1";

                            SubLotStr = tbx_UDI.Text.Substring(26, 6);

                            //return tbx_UDI.Text.Substring(0, 24);
                        }
                    }
                    else
                    {
                        if (btn_17.BackColor == Color.FromArgb(0, 192, 0))
                        {
                            if (!tbx_UDI.Text.Substring(0, 2).Equals("01") || !tbx_UDI.Text.Substring(16, 2).Equals("17")) return "-1";

                            SubLotStr = tbx_UDI.Text.Substring(26, 6);

                            //return tbx_UDI.Text.Substring(0, 24);
                        }

                        else
                        {
                            if (!tbx_UDI.Text.Substring(0, 2).Equals("01")) return "-1";

                            SubLotStr = tbx_UDI.Text.Substring(18, 6);

                            //return tbx_UDI.Text.Substring(0, 16);
                        }
                    }
                }

                return tbx_UDI.Text.Substring(0, tbx_UDI.Text.Length - 8);
                
            }
            catch(Exception ex)
            {
                return "-1";
            }
        }

        private bool chkExistUDI()
        {
            try
            {
                if (grid_Detail.Rows.Count == 0) return false;

                foreach(DataRow dr in detail.Rows)
                {
                    if (dr["UDI"].ToString().Equals(tbx_UDI.Text)) return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
