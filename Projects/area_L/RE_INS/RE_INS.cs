using area_L;
using area_L.Modules;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Base.ViewInfo;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraSplashScreen;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace area_L
{
    public partial class RE_INS : Form
    {
        #region [1. 생성자 및 변수]

        public static string dbName = "ERP_TEST";

        MSSQL db = new MSSQL(dbName);
        
        CommonModule cm = new CommonModule();

        private Label lb_Status; private Label lb_Status2;

        private DataTable empDt = null;

        private Dictionary<int,int> hands = new Dictionary<int, int>();

        private int handIdx = -1;

        private int chkQty = 0;

        private int sucQty = 0;

        private DataTable BarCodeTbl = null;

        private DataTable rdt = null;

        private bool chkOX = false;

        private bool is_BarCode_UDI = false;

        private string fromLotNo = "";


        public RE_INS()
        {
            InitializeComponent();

            initControl();
        }

        enum barState
        {
            length_err = 0,
            err_880 = 1,
            isnumber_err = 2,
            none = 3,
            success = 4,
            chkHand = 5
        }

        #endregion


        #region [2. 이벤트 함수]

        private void Rbtn_Click(object sender, EventArgs e)
        {
            try
            {
                int ridx = gridView2.FocusedRowHandle;

                int Existseq = Convert.ToInt32(gridView2.GetRowCellDisplayText(ridx, "EXIST_SEQ"));

                for(int i = 0; i < gridView1.RowCount; i++)
                {
                    if (Convert.ToInt32(gridView1.GetRowCellValue(i, "SEQ")).Equals(Existseq) && Convert.ToInt32(gridView1.GetRowCellValue(i, "CHK_QTY")) > 0)
                    {
                        if(gridView2.GetRowCellDisplayText(ridx, "ERP_RETURN_INFO").Equals("수기"))
                        {
                            gridView1.SetRowCellValue(i, "CHK_QTY", 0);

                            gridView2.DeleteRow(ridx);

                            gridView1.SetRowCellValue(i, "RETURNBYHAND", "False");

                            break;
                        }
                        else
                        {
                            gridView1.SetRowCellValue(i, "CHK_QTY", (int)gridView1.GetRowCellValue(i, "CHK_QTY") - 1);

                            gridView2.DeleteRow(ridx);

                            break;
                        }

                    }
                }

                All_SCANQTY();
            }
            catch(Exception ex)
            {

            }
        }

        private void GridView2_InitNewRow(object sender, InitNewRowEventArgs e)
        {
            try
            {
                GridView gv = sender as GridView;

                int ridx = gv.RowCount == 1 ? 0 : gv.RowCount - 2;

                int newidx = gridView2.RowCount == 1 ? 1 : int.Parse(gv.GetRowCellDisplayText(ridx, "SCAN_SEQ")) + 1;

                //int ridx = gv.RowCount == 1 ? 0 : gv.RowCount - 1;

                //int newidx = ridx == 0 ? 1 : int.Parse(gv.GetRowCellDisplayText(ridx - 1, "SEQ")) + 1;

                if (handIdx == -1)
                {
                    int idx = chkOX ? gridView1.GetSelectedRows()[0] : 0;

                    gv.SetRowCellValue(e.RowHandle, "SCAN_SEQ", newidx);

                    gv.SetRowCellValue(e.RowHandle, "GD_NM_CD", gd_cd.Text.Substring(0, 4));

                    gv.SetRowCellValue(e.RowHandle, "GD_CD", gd_cd.Text);

                    gv.SetRowCellValue(e.RowHandle, "GD_NM", gd_nm.Text);

                    gv.SetRowCellValue(e.RowHandle, "DOS", spec1.Text);

                    gv.SetRowCellValue(e.RowHandle, "SCAN_QTY", 1);

                    gv.SetRowCellValue(e.RowHandle, "ERP_RETURN_INFO", chkOX ? "O" : "X");

                    gv.SetRowCellValue(e.RowHandle, "PRICE", gridView1.GetRowCellDisplayText(idx, "RT_PRI"));

                    gv.SetRowCellValue(e.RowHandle, "TYPE", gridView1.GetRowCellDisplayText(idx, "OD_GU"));

                    gv.SetRowCellValue(e.RowHandle, "DISCOUNTPERCENT", gridView1.GetRowCellDisplayText(idx, "DISCOUNT_RT"));

                    gv.SetRowCellValue(e.RowHandle, "NEW_GDCD", NEW_GDCD.Text);

                    gv.SetRowCellValue(e.RowHandle, "LOT_NO", fromLotNo);

                    gv.SetRowCellValue(e.RowHandle, "MATE_NO", string.Empty); // gv.SetRowCellValue(e.RowHandle, "MATE_NO", MATE_NO.Text);

                    gv.SetRowCellValue(e.RowHandle, "BarCode", barcode_nm.Text);

                    gv.SetRowCellValue(e.RowHandle, "EXIST_SEQ", gridView1.GetRowCellDisplayText(idx, "SEQ"));

                    gv.SetRowCellValue(e.RowHandle, "ORIGIN_GDCD", gridView1.GetRowCellDisplayText(idx, "GD_CD"));
                }
                else
                {
                    if(!hands.ContainsKey(handIdx)) hands.Add(handIdx, newidx);

                    gv.SetRowCellValue(e.RowHandle, "SCAN_SEQ", newidx);

                    gv.SetRowCellValue(e.RowHandle, "GD_NM_CD", gridView1.GetRowCellDisplayText(handIdx,"GD_CD").Substring(0,4));

                    gv.SetRowCellValue(e.RowHandle, "GD_CD", gridView1.GetRowCellDisplayText(handIdx, "GD_CD"));

                    gv.SetRowCellValue(e.RowHandle, "GD_NM", gridView1.GetRowCellDisplayText(handIdx, "GD_NM"));

                    gv.SetRowCellValue(e.RowHandle, "DOS", "-99.99");

                    gv.SetRowCellValue(e.RowHandle, "SCAN_QTY", gridView1.GetRowCellDisplayText(handIdx, "RT_QTY"));

                    gv.SetRowCellValue(e.RowHandle, "ERP_RETURN_INFO", "수기");

                    gv.SetRowCellValue(e.RowHandle, "PRICE", gridView1.GetRowCellDisplayText(handIdx, "RT_PRI"));

                    gv.SetRowCellValue(e.RowHandle, "TYPE", gridView1.GetRowCellDisplayText(handIdx, "OD_GU"));

                    gv.SetRowCellValue(e.RowHandle, "DISCOUNTPERCENT", gridView1.GetRowCellDisplayText(handIdx, "DISCOUNT_RT"));

                    gv.SetRowCellValue(e.RowHandle, "NEW_GDCD", string.Empty);

                    gv.SetRowCellValue(e.RowHandle, "LOT_NO", fromLotNo);

                    gv.SetRowCellValue(e.RowHandle, "MATE_NO", string.Empty);  // gv.SetRowCellValue(e.RowHandle, "MATE_NO", fromLotNo);

                    gv.SetRowCellValue(e.RowHandle, "BarCode", string.Empty);

                    gv.SetRowCellValue(e.RowHandle, "EXIST_SEQ", gridView1.GetRowCellDisplayText(handIdx, "SEQ"));

                    gv.SetRowCellValue(e.RowHandle, "ORIGIN_GDCD", gridView1.GetRowCellDisplayText(handIdx, "GD_CD"));

                    handIdx = -1;
                }

            }
            catch (Exception ex)
            {

            }
        }

        private void Ri_CheckedChanged(object sender, EventArgs e)
        {
            try
            {

                CheckEdit chkEdit = sender as CheckEdit;

                handIdx = gridView1.FocusedRowHandle;

                if (chkEdit.EditValue.ToString().Equals("True"))
                {
                    if(gridView1.GetRowCellDisplayText(handIdx,"GD_CD").Length >= 1 && !gridView1.GetRowCellDisplayText(handIdx, "GD_CD").Substring(0, 1).ToUpper().Equals("H"))
                    {
                        MessageBox.Show("H로 시작하는 품목이 아닙니다.\n확인부탁드립니다.", "수기검수", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        chkEdit.EditValue = "False";

                        return;
                    }

                    if (!gridView1.GetRowCellDisplayText(handIdx, "CHK_QTY").Equals("0"))
                    {
                        MessageBox.Show("이미 스캔된 바코드정보가 있습니다.\n확인부탁드립니다.", "수기검수", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        chkEdit.EditValue = "False";

                        return;
                    }

                    if (string.IsNullOrEmpty(gridView1.GetRowCellDisplayText(handIdx, "MATE_NO")))
                    {
                        INSERT_MATENO im = new INSERT_MATENO(this);

                        im.ShowDialog();

                        fromLotNo = MATE_NO.Text;
                    }
                    else fromLotNo = gridView1.GetRowCellDisplayText(handIdx, "MATE_NO");

                    gridView1.SetRowCellValue(handIdx, "CHK_QTY", gridView1.GetRowCellDisplayText(handIdx, "RT_QTY"));

                    gridView2.AddNewRow();

                    gridView2.UpdateCurrentRow();
                }
                else
                {
                    for (int i = 0; i < gridView2.RowCount; i++)
                    {
                        if (int.Parse(gridView2.GetRowCellDisplayText(i, "SCAN_SEQ")) == hands[handIdx])
                        {
                            gridView2.DeleteRow(i);

                            gridView1.SetRowCellValue(handIdx, "CHK_QTY", "0");

                            break;
                        }
                    }

                    hands.Remove(handIdx);

                    handIdx = -1;

                }

                All_SCANQTY();
            }
            catch (Exception ex)
            {

            }
        }

        private void GridView1_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            try
            {
                e.Appearance.BackColor = e.Column.Name.Equals("colCHK_QTY") ? Color.LightYellow : Color.White;
            }
            catch (Exception ex)
            {

            }
        }

        private void GridView2_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            try
            {
                e.Appearance.BackColor = e.Column.Name.Equals("colSCAN_QTY") ? Color.LightYellow : Color.White;
            }
            catch (Exception ex)
            {

            }
        }

        private void rt_no_RemovePlaceHolder(object sender, EventArgs e)
        {
            try
            {
                TextBox tx = sender as TextBox;

                if(tx.Text.Equals("2. 반품번호를 입력해주세요."))
                {
                    tx.ForeColor = Color.Black;

                    tx.Text = string.Empty;
                }
            }
            catch(Exception ex)
            {

            }
        }

        private void rt_no_GetPlaceHolder(object sender, EventArgs e)
        {
            try
            {
                TextBox tx = sender as TextBox;

                if (string.IsNullOrEmpty(tx.Text))
                {
                    tx.ForeColor = Color.DarkGray;

                    tx.Text = "2. 반품번호를 입력해주세요.";
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void ps_cd_RemovePlaceHolder(object sender, EventArgs e)
        {
            try
            {
                TextBox tx = sender as TextBox;

                if (tx.Text.Equals("1. 사번 입력."))
                {
                    tx.ForeColor = Color.Black;

                    tx.Text = string.Empty;
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void ps_cd_GetPlaceHolder(object sender, EventArgs e)
        {
            try
            {
                TextBox tx = sender as TextBox;

                if (string.IsNullOrEmpty(tx.Text))
                {
                    tx.ForeColor = Color.DarkGray;

                    tx.Text = "1. 사번 입력.";
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void barcode_RemovePlaceHolder(object sender, EventArgs e)
        {
            try
            {
                TextBox tx = sender as TextBox;

                if (tx.Text.Equals("3. 바코드를 스캔해 주십시오."))
                {
                    tx.ForeColor = Color.Black;

                    tx.Text = string.Empty;
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void barcode_GetPlaceHolder(object sender, EventArgs e)
        {
            try
            {
                TextBox tx = sender as TextBox;

                if (string.IsNullOrEmpty(tx.Text))
                {
                    tx.ForeColor = Color.DarkGray;

                    tx.Text = "3. 바코드를 스캔해 주십시오.";
                }
            }
            catch (Exception ex)
            {

            }
        }

        #endregion

        #region [3. 함수]


        private void initControl()
        {
            try
            {
                ps_cd.Focus();

                ps_nm.Text = "<- 사번입력";

                SetGridOptions(gridView1, 40);

                SetGridOptions(gridView2, 40);

                gridView1.RowCellStyle += GridView1_RowCellStyle;

                gridView2.RowCellStyle += GridView2_RowCellStyle;

                fn_GetData();

                ps_cd.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) fn_GetPsNM(); };

                rt_no.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter && !rt_no.ReadOnly) fn_GetRtData(); };

                barcode_txt.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) fn_barcodeExecute(); };

                fn_MakeBindingTbl();

                btn_Reset.Click += (s, e) => { fn_Reset(false); };

                btn_Confirm.Click += (s, e) => { fn_Confirm(); };

                gd_cd.ReadOnly           = true;
                MATE_NO.ReadOnly         = true;
                gd_nm.ReadOnly           = true;
                spec1.ReadOnly           = true;
                NEW_GDCD.ReadOnly        = true;
                barcode_nm.ReadOnly      = true;
                SALE_CD.ReadOnly         = true;
                tbx_nowdate.ReadOnly     = true;
                tbx_expire_date.ReadOnly = true;
                tbx_stts.ReadOnly        = true;

                
                gridView2.InitNewRow += GridView2_InitNewRow;

                ps_cd.Text = "1. 사번 입력.";

                ps_cd.ForeColor = Color.Gray;

                ps_cd.GotFocus += ps_cd_RemovePlaceHolder;

                ps_cd.LostFocus += ps_cd_GetPlaceHolder;

                rt_no.Text = "2. 반품번호를 입력해주세요.";

                rt_no.ForeColor = Color.Gray;

                rt_no.GotFocus += rt_no_RemovePlaceHolder;

                rt_no.LostFocus += rt_no_GetPlaceHolder;

                barcode_txt.Text = "3. 바코드를 스캔해 주십시오.";

                barcode_txt.ForeColor = Color.Gray;

                barcode_txt.GotFocus += barcode_RemovePlaceHolder;

                barcode_txt.LostFocus += barcode_GetPlaceHolder;

                ps_cd.Select();

            }
            catch(Exception ex)
            {

            }
        }

        private void fn_barcodeExecute()
        {
            try
            {
                //if (gridView1.DataSource == null || gridView1.RowCount == 0)
                //{
                //    MessageBox.Show("반품정보를 확인해주세요!", "반품정보 확인", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                //    barcode_txt.Text = string.Empty;

                //    rt_no.Select();

                //    return;
                //}

                barState bstate = fn_ChkBarcode(barcode_txt.Text);

                if (bstate != barState.success)
                {
                    switch (bstate)
                    {

                        case barState.err_880:

                            MessageBox.Show("바코드가 880으로 시작합니다.\n확인부탁드립니다.", "바코드", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                            break;

                        case barState.isnumber_err:

                            MessageBox.Show("바코드는 숫자여야 합니다.\n확인부탁드립니다.", "바코드", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                            break;
                    }

                    return;
                }

                MATE_NO.Text = string.Empty;

                is_BarCode_UDI = false;

                barcode_nm.Text = barcode_txt.Text.Trim();

                string strSql = $"{dbName}.dbo.ST_RE_INS_BARCODE_ACCESS_BEFORE";

                db.Parameter("@BAR_CD", barcode_nm.Text);

                db.ExecuteSql(strSql);

                if (db.nState)
                {
                    if(db.result.Rows.Count > 0)
                    {
                        DataRow dr = db.result.Rows[0];

                        gd_cd.Text = dr["GD_CD"].ToString();

                        gd_nm.Text = dr["GD_NM"].ToString();
                    }
                    else
                    {
                        MessageBox.Show("바코드 정보가 존재하지 않습니다.");

                        barcode_txt.ResetText();

                        return;
                    }
                }

                if (barcode_txt.TextLength >= 40 && barcode_txt.Text.ToString().Substring(0, 2).Equals("01"))
                {
                    if (fn_UDI_duplicate_chk())
                    {
                        MessageBox.Show($"이미 스캔된 UDI입니다.\n확인부탁드립니다.", "바코드 스캔", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                        barcode_txt.SelectAll();

                        return;
                    }

                    MATE_NO.Text = barcode_txt.Text.ToString().Substring(18, 4);

                    barcode_txt.Text = barcode_txt.Text.ToString().Substring(2, 14);

                    is_BarCode_UDI = true;
                }
                else
                {
                    INSERT_MATENO im = new INSERT_MATENO(this);

                    im.ShowDialog();
                }


                strSql = $"{dbName}.dbo.ST_RE_INS_BARCODE_ACCESS";

                db.Parameter("@BAR_CD", barcode_txt.Text.Trim());
                db.Parameter("@UDI_FULL", barcode_nm.Text.Trim());
                db.Parameter("@MATE_NO", MATE_NO.Text.Trim());

                db.ExecuteSql(strSql);

                if (db.nState)
                {
                    BarCodeTbl = db.result;
                }

                if (BarCodeTbl.Rows.Count == 0)
                {
                    MessageBox.Show("바코드 정보가 존재하지 않습니다.");

                    barcode_txt.ResetText();

                    return;
                }

                DataRow barRs = BarCodeTbl.Rows[0];

                gd_cd.Text = barRs["GD_CD"].ToString(); 

                gd_nm.Text = barRs["GD_NM"].ToString(); 

                spec1.Text = barRs["SPEC"].ToString();

                NEW_GDCD.Text = barRs["NEW_SALECD"].ToString();

                SALE_CD.Text = barRs["SALE_CD"].ToString();

                tbx_nowdate.Text = barRs["NOWTIME"].ToString();

                tbx_expire_date.Text = barRs["EX_PERIOD"].ToString();

                fromLotNo = barRs["LOT_NO"].ToString();

                if (barRs["DISCONTINUED_YN"].ToString().Equals("Y"))
                {
                    MessageBox.Show($"{barRs["GD_CD"].ToString()}는 단종된 제품입니다.\n확인부탁드립니다.", "바코드 스캔", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    return;
                }

                if (string.IsNullOrEmpty(fromLotNo))
                {
                    MessageBox.Show("멸균번호가 없습니다.\n확인부탁드립니다.", "바코드 스캔", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    return;
                }

                if (barRs["STTS"].ToString().Equals("Y"))
                {
                    tbx_stts.ForeColor = Color.YellowGreen;

                    tbx_stts.BackColor = Color.Teal;

                    tbx_stts.Text = "정상입니다.";

                    if (barRs["CHUL_GU_BUNRYU"].ToString().Equals("N")) MessageBox.Show("출고기한 초과입니다.\n확인부탁드립니다.", "메세지", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    barcode_txt.ResetText();

                    fn_Mapper();

                    All_SCANQTY();
                }
                else
                {
                    tbx_stts.ForeColor = Color.Red;

                    tbx_stts.BackColor = Color.Yellow;

                    tbx_stts.Text = "유효기간이 6개월 미만입니다.\n반품접수등록이 불가합니다.";

                    barcode_txt.ResetText();
                }
            }
            catch (Exception ex)
            {

            }
        }

        private bool fn_UDI_duplicate_chk()
        {
            try
            {
                for(int i = 0; i < gridView2.RowCount; i++)
                {
                    if (gridView2.GetRowCellDisplayText(i, "BarCode").Equals(barcode_nm.Text)) return true;
                }

                return false;
            }
            catch(Exception ex )
            {
                return false;
            }
        }

        private void fn_Mapper()
        {
            try
            {
                for (int i = 0; i < gridView1.RowCount; i++)
                {
                    if (NEW_GDCD.Text.Equals(gridView1.GetRowCellValue(i, "NEW_GDCD").ToString()))
                    {
                        cm.GridSelectedRowClear(gridView1, gridView1.GetSelectedRows().Length);

                        gridView1.SelectRow(i);

                        if (gridView1.GetRowCellDisplayText(i, "RT_QTY").Equals(gridView1.GetRowCellDisplayText(i, "CHK_QTY"))) continue;

                        else
                        {
                            gridView1.SetRowCellValue(i, "CHK_QTY", (int)gridView1.GetRowCellValue(i, "CHK_QTY") + 1);

                            fn_AddScanRow();

                            return;
                        }
                    }
                }

                MessageBox.Show("바코드 정보에 해당하는 반품정보가 없거나 체크수량과 반품수량이 같아진 품목입니다..\n확인부탁드립니다.", "바코드 입력", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                return;
            }
            catch (Exception ex)
            {
                return;
            }
        }

        private void fn_AddScanRow()
        {
            try
            {
                // sucQty : ERP반품정보 0
                // chkQty : ERP반품정보의 총 반품수량

                if (chkQty > sucQty)
                {
                    chkOX = true;

                    gridView2.AddNewRow();

                    gridView2.UpdateCurrentRow();

                    return;
                }
            }
            catch(Exception ex)
            {
            }
        }

        
        private void fn_MakeBindingTbl()
        {
            try
            {
                rdt = new DataTable();

                rdt.Columns.Add("SCAN_SEQ");

                rdt.Columns.Add("GD_NM_CD");

                rdt.Columns.Add("GD_CD");

                rdt.Columns.Add("GD_NM");

                rdt.Columns.Add("DOS");

                rdt.Columns.Add("SCAN_QTY");

                rdt.Columns.Add("ERP_RETURN_INFO");

                rdt.Columns.Add("PRICE");

                rdt.Columns.Add("TYPE");

                rdt.Columns.Add("DISCOUNTPERCENT");

                rdt.Columns.Add("NEW_GDCD");

                rdt.Columns.Add("LOT_NO"); // 20250721

                rdt.Columns.Add("MATE_NO");

                rdt.Columns.Add("BarCode");

                //rdt.Columns.Add("LOT_NO"); // 20250721

                rdt.Columns.Add("EXIST_SEQ");

                rdt.Columns.Add("btn_Del");

                rdt.Columns.Add("ORIGIN_GDCD");

                gridControl2.DataSource = rdt;

                gridView2.OptionsView.ColumnHeaderAutoHeight = DevExpress.Utils.DefaultBoolean.True;

                gridView2.Columns["SCAN_SEQ"].Caption = "스캔순번";
                gridView2.Columns["GD_NM_CD"].Caption = "제품명코드";
                gridView2.Columns["GD_CD"].Caption = "제품코드";
                gridView2.Columns["GD_NM"].Caption = "제품명";
                gridView2.Columns["DOS"].Caption = "도수";
                gridView2.Columns["SCAN_QTY"].Caption = "스캔수량";
                gridView2.Columns["ERP_RETURN_INFO"].Caption = "ERP 반품정보";
                gridView2.Columns["PRICE"].Caption = "단가";
                gridView2.Columns["TYPE"].Caption = "유형";
                gridView2.Columns["DISCOUNTPERCENT"].Caption = "할인율";
                gridView2.Columns["NEW_GDCD"].Caption = "뉴코드";
                gridView2.Columns["MATE_NO"].Caption = "멸균LOT";
                gridView2.Columns["btn_Del"].Caption = "삭제";

                gridView2.Columns["BarCode"].Visible = false;
                gridView2.Columns["EXIST_SEQ"].Visible = false;
                gridView2.Columns["ORIGIN_GDCD"].Visible = false;
                //gridView2.Columns["LOT_NO"].Visible = false; // 20250721

                gridView2.Columns["SCAN_SEQ"].OptionsColumn.ReadOnly = true;
                gridView2.Columns["GD_NM_CD"].OptionsColumn.ReadOnly = true;
                gridView2.Columns["GD_CD"].OptionsColumn.ReadOnly = true;
                gridView2.Columns["GD_NM"].OptionsColumn.ReadOnly = true;
                gridView2.Columns["DOS"].OptionsColumn.ReadOnly = true;
                gridView2.Columns["SCAN_QTY"].OptionsColumn.ReadOnly = true;
                gridView2.Columns["ERP_RETURN_INFO"].OptionsColumn.ReadOnly = true;
                gridView2.Columns["PRICE"].OptionsColumn.ReadOnly = true;
                gridView2.Columns["TYPE"].OptionsColumn.ReadOnly = true;
                gridView2.Columns["DISCOUNTPERCENT"].OptionsColumn.ReadOnly = true;
                gridView2.Columns["NEW_GDCD"].OptionsColumn.ReadOnly = true;
                gridView2.Columns["LOT_NO"].OptionsColumn.ReadOnly = true;

                gridView2.Columns["SCAN_SEQ"].Width = 50;
                gridView2.Columns["GD_NM_CD"].Width = 100;
                gridView2.Columns["GD_CD"].Width = 100;
                gridView2.Columns["GD_NM"].Width = 100;
                gridView2.Columns["DOS"].Width = 100;
                gridView2.Columns["SCAN_QTY"].Width = 50;
                gridView2.Columns["ERP_RETURN_INFO"].Width = 100;
                gridView2.Columns["PRICE"].Width = 100;
                gridView2.Columns["TYPE"].Width = 100;
                gridView2.Columns["DISCOUNTPERCENT"].Width = 100;
                gridView2.Columns["NEW_GDCD"].Width = 100;
                gridView2.Columns["MATE_NO"].Width = 100;
                gridView2.Columns["btn_Del"].Width = 100;

                gridView2.Columns["SCAN_SEQ"].AppearanceHeader.Font = new Font("Tahoma", 12, FontStyle.Bold);
                gridView2.Columns["GD_NM_CD"].AppearanceHeader.Font = new Font("Tahoma", 12, FontStyle.Bold);
                gridView2.Columns["GD_CD"].AppearanceHeader.Font = new Font("Tahoma", 12, FontStyle.Bold);
                gridView2.Columns["GD_NM"].AppearanceHeader.Font = new Font("Tahoma", 12, FontStyle.Bold);
                gridView2.Columns["DOS"].AppearanceHeader.Font = new Font("Tahoma", 12, FontStyle.Bold);
                gridView2.Columns["SCAN_QTY"].AppearanceHeader.Font = new Font("Tahoma", 12, FontStyle.Bold);
                gridView2.Columns["ERP_RETURN_INFO"].AppearanceHeader.Font = new Font("Tahoma", 12, FontStyle.Bold);
                gridView2.Columns["PRICE"].AppearanceHeader.Font = new Font("Tahoma", 12, FontStyle.Bold);
                gridView2.Columns["TYPE"].AppearanceHeader.Font = new Font("Tahoma", 12, FontStyle.Bold);
                gridView2.Columns["DISCOUNTPERCENT"].AppearanceHeader.Font = new Font("Tahoma", 12, FontStyle.Bold);
                gridView2.Columns["NEW_GDCD"].AppearanceHeader.Font = new Font("Tahoma", 12, FontStyle.Bold);
                gridView2.Columns["MATE_NO"].AppearanceHeader.Font = new Font("Tahoma", 12, FontStyle.Bold);
                gridView2.Columns["LOT_NO"].AppearanceHeader.Font = new Font("Tahoma", 12, FontStyle.Bold);
                gridView2.Columns["btn_Del"].AppearanceHeader.Font = new Font("Tahoma", 12, FontStyle.Bold);

                gridView2.Columns["btn_Del"].ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;

                RepositoryItemButtonEdit rbtn = new RepositoryItemButtonEdit();

                EditorButton ebtn = new EditorButton();

                ebtn.Kind = ButtonPredefines.Delete;

                ebtn.Click += Rbtn_Click;

                rbtn.Buttons.Add(ebtn);

                rbtn.TextEditStyle = TextEditStyles.HideTextEditor;

                gridView2.Columns["btn_Del"].ColumnEdit = rbtn;

            }
            catch(Exception ex)
            {
                
            }
        }



        private void fn_Reset(bool chkSave)
        {
            try
            {

                rt_no.Text = string.Empty;

                cust_cdnm.Text = string.Empty;

                barcode_txt.Text = string.Empty;

                gd_cd.Text = string.Empty;

                gd_nm.Text = string.Empty;

                spec1.Text = string.Empty;

                barcode_nm.Text = string.Empty;

                NEW_GDCD.Text = string.Empty;

                MATE_NO.Text = string.Empty;

                SALE_CD.Text = string.Empty;

                tbx_expire_date.Text = string.Empty;

                tbx_nowdate.Text = string.Empty;

                tbx_stts.ForeColor = Color.Black;

                tbx_stts.BackColor = Color.White;

                tbx_stts.Text = string.Empty;

                tbx_stts.ReadOnly = true;

                scan_all_qty.Text = "총 검수수량 : 0";

                chkExchange.Checked = false;

                rt_no.ReadOnly = false;

                chkQty = 0; handIdx = -1; sucQty = 0; hands.Clear();

                cm.fn_GridRowClear(gridView1, gridView1.RowCount);

                cm.fn_GridRowClear(gridView2, gridView2.RowCount);

                lb_Status.Text = lb_Status2.Text = string.Empty;

                //ps_nm.Text = chkSave ? ps_nm.Text : "<- 사번입력";

                //ps_cd.Text = chkSave ? ps_cd.Text : "1. 사번 입력.";

                ps_cd.ForeColor = chkSave ? Color.Black : Color.Gray;

                rt_no.Text = "2. 반품번호를 입력해주세요.";

                rt_no.ForeColor = Color.Gray;

                barcode_txt.Text = "3. 바코드를 스캔해 주십시오.";

                barcode_txt.ForeColor = Color.Gray;

                rt_no.Select();

            }
            catch (Exception ex)
            {
                
            }
        }

        private void fn_Confirm()
        {
            try
            {

                if (MessageBox.Show("스캔정보를 저장하시겠습니까?", "저장", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) return;

                if (!splashScreenManager1.IsSplashFormVisible) splashScreenManager1.ShowWaitForm();

                if (gridView2.RowCount == 0)
                {
                    MessageBox.Show("입력된 스캔정보가 없습니다.\n확인부탁드립니다.","저장",MessageBoxButtons.OK,MessageBoxIcon.Warning);

                    return;
                }
                
                int gRcnt2 = this.gridView2.RowCount;

                for (int i = 0; i < gRcnt2; i++)
                {
                    string gdcd = gridView2.GetRowCellDisplayText(i, "GD_CD");

                    if (string.IsNullOrEmpty(gdcd)) continue;

                    if (IsNumber(gdcd.Substring(0, 1)))
                    {
                        MessageBox.Show($"※잘못된 제품정보가 있습니다.\n스캔순번 : {gridView2.GetRowCellDisplayText(i,"SCAN_SEQ")}\n의 제품코드를 확인하세요.", "경고", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                        return;
                    }

                    // 수정일시 : 2025-12-18
                    // 물류팀 구미경 반장님 요청.
                    //if (!chkExchange.Checked)
                    //{
                    //    if(gridView2.GetRowCellDisplayText(i, "MATE_NO").Equals(string.Empty))
                    //    {
                    //        MessageBox.Show($"※교환이 아니기에 멸균번호가 필요합니다.\n멸균번호를 입력해주시기 바랍니다.", "경고", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    //        return;
                    //    }

                    //    if (!gridView2.GetRowCellDisplayText(i, "MATE_NO").Equals(string.Empty) && gridView2.GetRowCellDisplayText(i, "MATE_NO").Length != 4)
                    //    {
                    //        MessageBox.Show($"멸균번호가 4자리가 아닌것이 있습니다.\n확인부탁드립니다.", "경고", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    //        return;
                    //    }
                    //}
                }

                DataTable backupDt = new DataTable();

                backupDt.Columns.Add("RT_NO");
                backupDt.Columns.Add("SEQ");
                backupDt.Columns.Add("INSPECT_QTY");

                int gRcnt1 = this.gridView1.RowCount;

                for (int i = 0; i < gRcnt1; i++)
                {
                    DataRow dr = backupDt.NewRow();

                    dr["RT_NO"] = rt_no.Text;

                    dr["SEQ"] = gridView1.GetRowCellDisplayText(i, "SEQ");

                    dr["INSPECT_QTY"] = gridView1.GetRowCellDisplayText(i, "CHK_QTY");

                    backupDt.Rows.Add(dr);
                }

                DataTable newDt = new DataTable();

                newDt.Columns.Add("RT_NO");
                newDt.Columns.Add("SEQ");
                newDt.Columns.Add("GD_CD");
                newDt.Columns.Add("LOT_NO");
                newDt.Columns.Add("RT_QTY");
                newDt.Columns.Add("RT_PRI");
                newDt.Columns.Add("REMK");
                newDt.Columns.Add("OD_GU");
                newDt.Columns.Add("DISCOUNT_RT");
                newDt.Columns.Add("MATE_NO");
                newDt.Columns.Add("BARCODE_NM_TEXT");
                newDt.Columns.Add("ORIGIN_GDCD");
                newDt.Columns.Add("ORIGIN_SEQ");

                for (int i=0; i< gRcnt2; i++)
                {
                    DataRow dr = newDt.NewRow();

                    dr["RT_NO"] = rt_no.Text;

                    dr["SEQ"] = int.Parse(gridView2.GetRowCellDisplayText(i, "SCAN_SEQ"));

                    dr["GD_CD"] = gridView2.GetRowCellDisplayText(i, "GD_CD");

                    dr["LOT_NO"] = gridView2.GetRowCellDisplayText(i, "LOT_NO");

                    dr["RT_QTY"] = int.Parse(gridView2.GetRowCellDisplayText(i, "SCAN_QTY"));

                    dr["RT_PRI"] = gridView2.GetRowCellDisplayText(i, "PRICE");

                    dr["REMK"] = gridView2.GetRowCellDisplayText(i, "ERP_RETURN_INFO");

                    dr["OD_GU"] = gridView2.GetRowCellDisplayText(i, "TYPE");

                    dr["DISCOUNT_RT"] = gridView2.GetRowCellDisplayText(i, "DISCOUNTPERCENT");

                    dr["MATE_NO"] = gridView2.GetRowCellDisplayText(i, "MATE_NO");

                    dr["BARCODE_NM_TEXT"] = gridView2.GetRowCellDisplayText(i, "BarCode");

                    dr["ORIGIN_GDCD"] = gridView2.GetRowCellDisplayText(i, "ORIGIN_GDCD");

                    dr["ORIGIN_SEQ"] = gridView2.GetRowCellDisplayText(i, "EXIST_SEQ");

                    newDt.Rows.Add(dr);
                }

                string strSql = $"{dbName}.dbo.ST_RETURN_PRO_UPD";

                db.Parameter("@RT_NO", rt_no.Text);
                db.Parameter("@PS_CD", ps_cd.Text);

                string[] spliters = cust_cdnm.Text.Trim().Split('/');

                db.Parameter("@CUST_CD", spliters[0]);
                db.Parameter("@BACKUP", cm.DataTblToXML(backupDt));
                db.Parameter("@NEW_DATA", cm.DataTblToXML(newDt));

                db.ExecuteNonSql(strSql);

                if (splashScreenManager1.IsSplashFormVisible) splashScreenManager1.CloseWaitForm();

                if (db.nState)
                {
                    if (string.IsNullOrEmpty(db.sql_raise_error_msg))
                    {
                        fn_Reset(true);

                        return;
                    }
                    else
                    {
                        MessageBox.Show($"{db.sql_raise_error_msg} {rt_no.Text}", "저장", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                        return;
                    }
                }
                                
            }
            catch(Exception ex)
            {

            }
        }

        private barState fn_ChkBarcode(string barcodeTxt)
        {
            try
            {

                if (barcodeTxt.Substring(0, 3).Equals("880")) return barState.err_880;

                if (barcodeTxt.Length == 8 && !IsNumber(barcodeTxt.Substring(0, 8))) return barState.isnumber_err;

                return barState.success;
            }
            catch(Exception ex)
            {
                return barState.none;
            }
        }

        private bool IsNumber(string str)  //숫자인지 체크 Astra의 경우, OF22E이런식으로 시작 
        {

            try
            {
                char[] ch = str.Substring(0, 1).ToCharArray();

                if ((0x30 <= ch[0] && ch[0] <= 0x39))  
                {
                    return true;
                }

                return false;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        private void fn_GetRtData()
        {
            try
            {
                if (string.IsNullOrEmpty(ps_cd.Text))
                {
                    MessageBox.Show("승인자 ERP사번을 입력해주세요. 입력후 ENTER" ,"승인자 입력" ,MessageBoxButtons.OK ,MessageBoxIcon.Warning);

                    ps_cd.Focus();

                    ps_nm.Text = "<- 사번입력";

                    return;
                }

                rt_no.Text = rt_no.Text.Trim();

                string strSql = $"{dbName}.dbo.ST_RE_INS_RETURN_DATA";

                db.Parameter("@RT_NO", rt_no.Text);

                if(!splashScreenManager1.IsSplashFormVisible) splashScreenManager1.ShowWaitForm();

                db.ExecuteSql(strSql);

                if (splashScreenManager1.IsSplashFormVisible) splashScreenManager1.CloseWaitForm();

                if (db.nState)
                {
                    if(db.result.Rows.Count == 0)
                    {
                        MessageBox.Show("반품번호가 없거나 이미 검수한 반품입니다. 다시 확인해주세요!", "ERP 반품 정보", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                        rt_no.SelectAll();

                        return;
                    }

                    rt_no.ReadOnly = true;

                    gridControl1.DataSource = db.result;

                    gridView1.OptionsView.ColumnHeaderAutoHeight = DevExpress.Utils.DefaultBoolean.True;

                    foreach (DataRow rtqty in db.result.Rows) chkQty += int.Parse(rtqty["RT_QTY"].ToString());

                    DataRow dr = db.result.Rows[0];

                    lb_typeODGU.Text = dr["TITLE"].ToString();

                    chkExchange.Checked = dr["EXCHANGE_YN"].ToString().Equals("0") ? false : true;

                    cust_cdnm.Text = $"{dr["CS_CD"].ToString()} / {dr["CS_NM"].ToString()}"; 

                    gridView1.Columns["SEQ"].Caption = "순번";
                    gridView1.Columns["GD_CD"].Caption = "폼목코드";
                    gridView1.Columns["GD_NM"].Caption = "품명";
                    gridView1.Columns["RT_QTY"].Caption = "반품\n수량";
                    gridView1.Columns["CHK_QTY"].Caption = "체크\n수량";
                    gridView1.Columns["RETURNBYHAND"].Caption = "수기\n검수";
                    gridView1.Columns["RT_PRI"].Caption = "반품\n단가";
                    gridView1.Columns["TITLE"].Caption = "수주\n유형";
                    gridView1.Columns["DISCOUNT_RT"].Caption = "반품\n할인율";
                    gridView1.Columns["NEW_GDCD"].Caption = "뉴품목\n코드";

                    gridView1.Columns["CS_CD"].Visible = false;
                    gridView1.Columns["CS_NM"].Visible = false;
                    gridView1.Columns["EXCHANGE_YN"].Visible = false;
                    gridView1.Columns["OD_GU"].Visible = false;
                    gridView1.Columns["MATE_NO"].Visible = false;

                    gridView1.Columns["SEQ"].OptionsColumn.ReadOnly = true;
                    gridView1.Columns["GD_CD"].OptionsColumn.ReadOnly = true;
                    gridView1.Columns["GD_NM"].OptionsColumn.ReadOnly = true;
                    gridView1.Columns["RT_QTY"].OptionsColumn.ReadOnly = true;
                    gridView1.Columns["CHK_QTY"].OptionsColumn.ReadOnly = true;
                    gridView1.Columns["RT_PRI"].OptionsColumn.ReadOnly = true;
                    gridView1.Columns["TITLE"].OptionsColumn.ReadOnly = true;
                    gridView1.Columns["DISCOUNT_RT"].OptionsColumn.ReadOnly = true;
                    gridView1.Columns["NEW_GDCD"].OptionsColumn.ReadOnly = true;

                    RepositoryItemCheckEdit ri = new RepositoryItemCheckEdit();

                    ri.ValueChecked = "True";

                    ri.ValueUnchecked = "False";

                    ri.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;

                    ri.CheckBoxOptions.Style = DevExpress.XtraEditors.Controls.CheckBoxStyle.SvgCheckBox1;

                    ri.CheckedChanged += Ri_CheckedChanged;

                    gridView1.Columns["RETURNBYHAND"].ColumnEdit = ri;

                    gridView1.OptionsSelection.MultiSelect = true;

                    gridView1.Columns["SEQ"].Width = 50;
                    gridView1.Columns["GD_CD"].Width = 150;
                    gridView1.Columns["GD_NM"].Width = 300;
                    gridView1.Columns["RT_QTY"].Width = 50;
                    gridView1.Columns["CHK_QTY"].Width = 50;
                    gridView1.Columns["RETURNBYHAND"].Width = 30;
                    gridView1.Columns["RT_PRI"].Width = 100;
                    gridView1.Columns["OD_GU"].Width = 100;
                    gridView1.Columns["DISCOUNT_RT"].Width = 100;
                    gridView1.Columns["NEW_GDCD"].Width = 50;

                    gridView1.Columns["SEQ"].AppearanceHeader.Font = new Font("Tahoma", 12,FontStyle.Bold);
                    gridView1.Columns["GD_CD"].AppearanceHeader.Font = new Font("Tahoma", 12, FontStyle.Bold);
                    gridView1.Columns["GD_NM"].AppearanceHeader.Font = new Font("Tahoma", 12, FontStyle.Bold);
                    gridView1.Columns["RT_QTY"].AppearanceHeader.Font = new Font("Tahoma", 12, FontStyle.Bold);
                    gridView1.Columns["CHK_QTY"].AppearanceHeader.Font = new Font("Tahoma", 12, FontStyle.Bold);
                    gridView1.Columns["RETURNBYHAND"].AppearanceHeader.Font = new Font("Tahoma", 12, FontStyle.Bold);
                    gridView1.Columns["RT_PRI"].AppearanceHeader.Font = new Font("Tahoma", 12, FontStyle.Bold);
                    gridView1.Columns["OD_GU"].AppearanceHeader.Font = new Font("Tahoma", 12, FontStyle.Bold);
                    gridView1.Columns["DISCOUNT_RT"].AppearanceHeader.Font = new Font("Tahoma", 12, FontStyle.Bold);
                    gridView1.Columns["NEW_GDCD"].AppearanceHeader.Font = new Font("Tahoma", 12, FontStyle.Bold);
                    gridView1.Columns["TITLE"].AppearanceHeader.Font = new Font("Tahoma", 12, FontStyle.Bold);

                    lb_Status.Text = $"[ERP 반품정보] {db.result.Rows.Count} 행이 출력되었습니다.";

                    lb_Status2.Text = $"{DateTime.Now}";

                }

                barcode_txt.Focus();
            }
            catch(Exception ex )
            {

            }
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
            catch(Exception ex)
            {

            }
        }

        private void fn_GetPsNM()
        {
            try
            {
                DataRow[] dr = empDt.Select($"EMP_NO = '{ps_cd.Text}'");

                if(dr.Length > 0)
                {
                    ps_nm.Text = dr[0]["EMP_NM"].ToString();

                    ps_cd.Text = ps_cd.Text.Trim();

                    rt_no.Focus();
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
            catch(Exception ex)
            {

            }
        }

        private void All_SCANQTY()
        {
            try
            {
                int AllQty = 0;

                sucQty = 0;

                for (int i = 0; i < gridView2.RowCount; i++)
                {
                    AllQty += int.Parse(gridView2.GetRowCellDisplayText(i, "SCAN_QTY"));

                    sucQty += gridView2.GetRowCellDisplayText(i, "ERP_RETURN_INFO").Equals("O") || gridView2.GetRowCellDisplayText(i, "ERP_RETURN_INFO").Equals("수기") ? int.Parse(gridView2.GetRowCellDisplayText(i, "SCAN_QTY")) : 0;
                }

                scan_all_qty.Text = $"총 검수수량 : {AllQty}";
            }
            catch (Exception ex)
            {
                return;
            }
        }

        private void SetGridOptions(GridView dg, int rowHeight)
        {
            try
            {
                dg.OptionsView.ShowIndicator = false;

                dg.IndicatorWidth = 35;

                dg.ColumnPanelRowHeight = rowHeight;

                dg.OptionsView.ColumnAutoWidth = true;

                for (int i = 0; i < dg.Columns.Count; i++)
                {
                    dg.Columns[i].AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                }
            }
            catch (Exception ex)
            {

            }
        }
        #endregion


    }
}
