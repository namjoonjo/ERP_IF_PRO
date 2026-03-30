using COMBINATION.Modules;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace COMBINATION
{
    public partial class LotNoFIFO : Form
    {
  
        public LotNoFIFO(DataTable dt,string selectedLotNo)
        {
            InitializeComponent();

            initControl(dt,selectedLotNo);

            
        }

        private void initControl(DataTable dt, string selectedLotNo)
        {
            try
            {
                this.StartPosition = FormStartPosition.CenterScreen;

                SetGridRowHeader(grid_State, -1, true);
     
                grid_State.DataSource = dt;

                grid_State.Columns["IN_DT"].HeaderText = "입고일자";
                grid_State.Columns["ITM_CD"].HeaderText = "품목코드";
                grid_State.Columns["ITM_NM"].HeaderText = "품명";
                grid_State.Columns["MNG_NO"].HeaderText = "LotNo";
                grid_State.Columns["STOCK_QTY"].HeaderText = "현재고";
                grid_State.Columns["REMARK"].HeaderText = "비고";


                grid_State.Columns["IN_DT"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                grid_State.Columns["ITM_CD"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                grid_State.Columns["ITM_NM"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                grid_State.Columns["MNG_NO"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                grid_State.Columns["STOCK_QTY"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                grid_State.Columns["REMARK"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                grid_State.Columns["START_DT"].Visible = false;
                grid_State.Columns["END_DT"].Visible = false;

                grid_State.Rows[0].DefaultCellStyle.BackColor = Color.YellowGreen;
               
                grid_State.Rows[0].Cells["REMARK"].Value = "사용추천 LotNo";

                for (int i = 1; i < grid_State.Rows.Count; i++)
                {
                    DataGridViewRow dgr = grid_State.Rows[i];

                    if (dgr.Cells["MNG_NO"].Value.ToString().Equals(selectedLotNo))
                    {
                        dgr.Cells["REMARK"].Value = "입력한 LotNo";

                        fn_ChangeGridRowBackColor(i, Color.OrangeRed);
                    }
                    else dgr.Cells["REMARK"].Value = "-";
                }

                Statusbar.BackColor = Color.LightGray;

                Statusbar.Text = $"탐색기간 : {Convert.ToDateTime(dt.Rows[0]["START_DT"]).ToString("yyyy-MM-dd")} ~ {Convert.ToDateTime(dt.Rows[0]["END_DT"]).ToString("yyyy-MM-dd")}";


                grid_State.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) this.Close(); };
            }
            catch(Exception ex)
            {

            }
        }

        private void SetGridRowHeader(DataGridView dg, int rowHeight, bool readonlychk)
        {
            try
            {
                dg.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;

                dg.ColumnHeadersHeight = 40;

                dg.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                dg.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

                dg.EnableHeadersVisualStyles = false;

                dg.ColumnHeadersDefaultCellStyle.BackColor = Color.LightGray;

                dg.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                dg.RowHeadersVisible = false;

                dg.RowTemplate.Height = rowHeight != -1 ? rowHeight : dg.RowTemplate.Height; 

                dg.RowTemplate.Resizable = DataGridViewTriState.False;

                dg.AllowUserToAddRows = false;

                dg.ReadOnly = readonlychk;
            }
            catch (Exception ex)
            {

            }
        }

        private void fn_ChangeGridRowBackColor(int ridx, Color c)
        {
            try
            {
                for(int i=0;i<grid_State.Columns.Count;i++)
                {
                    grid_State.Rows[ridx].Cells[i].Style.BackColor = c;
                }
            }
            catch(Exception ex)
            {

            }
        }
    }
}
