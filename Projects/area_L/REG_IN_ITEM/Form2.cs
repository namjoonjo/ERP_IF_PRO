using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace area_L
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
            BuildUI();
        }
        void BuildUI()
        {
            if (fpExcel.Columns.Count == 0)
            {
                fpExcel.AutoGenerateColumns = true;
                fpExcel.AllowUserToAddRows = false;
                fpExcel.AllowUserToDeleteRows = false;
                fpExcel.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            }
        }
        void search()
        {
            var g = fpExcel;
            g.DataSource = null;
            g.AllowUserToAddRows = false;

            while (g.Columns.Count < 3)
            {
                int idx = g.Columns.Count;
                g.Columns.Add("C" + idx, "C" + idx);
            }

            g.Rows.Clear();

            var dt = DbHelper.ExecuteDataTable(
                "SELECT order_num, gd_cd, qty FROM isuf_stock WHERE qty > 0"
            );

            foreach (DataRow r in dt.Rows)
            {
                int rowIndex = g.Rows.Add();
                g.Rows[rowIndex].Cells[0].Value = r["order_num"];
                g.Rows[rowIndex].Cells[1].Value = r["gd_cd"];
                g.Rows[rowIndex].Cells[2].Value = r["qty"];
            }
        }
        private void fpExcel_Click(object sender, EventArgs e)
        {

        }

        private void fpExcel_MouseDoubleClick(object sender, MouseEventArgs e)
        {

        }

        private void Form2_Load(object sender, EventArgs e)
        {
            Sheet_Setting();
        }

        private void Form2_Activated(object sender, EventArgs e)
        {

        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void cmd_Upload_Click(object sender, EventArgs e)
        {
            using (var dlg = new OpenFileDialog())
            {
                dlg.Filter = "엑셀파일|*.xlsx;*.xls";
                dlg.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;

                if (dlg.ShowDialog(this) != DialogResult.OK) return;

                try
                {
                    Cursor = Cursors.WaitCursor;

                    var dt = ExcelReader.LoadFirstSheet(dlg.FileName); // 1행 헤더, 2행부터 데이터

                    DbHelper.ExecuteNonQuery("DELETE FROM isuf_stock");

                    foreach (DataRow r in dt.Rows)
                    {
                        var orderNum = (r.ItemArray.Length > 0 ? Convert.ToString(r[0]) : null)?.Trim();
                        if (string.IsNullOrWhiteSpace(orderNum)) break; // 첫 열이 빈 행에서 종료

                        var gdCd = (r.ItemArray.Length > 1 ? Convert.ToString(r[1]) : null)?.Trim();

                        decimal qty = 0;
                        if (r.ItemArray.Length > 2)
                        {
                            var s = Convert.ToString(r[2])?.Trim();
                            decimal.TryParse(s, out qty);
                        }

                        DbHelper.ExecuteNonQuery(
                            "INSERT INTO isuf_stock (order_num, gd_cd, qty) VALUES (@o, @g, @q)",
                            new SqlParameter("@o", orderNum ?? (object)DBNull.Value),
                            new SqlParameter("@g", gdCd ?? (object)DBNull.Value),
                            new SqlParameter("@q", qty)
                        );
                    }

                    search();
                    MessageBox.Show("업로드가 완료되었습니다.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "업로드 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    Cursor = Cursors.Default;
                }
            }
        }

        private void cmd_Search_Click(object sender, EventArgs e)
        {
            search();
        }

        private void cmd_Reset_Click(object sender, EventArgs e)
        {
            DbHelper.ExecuteNonQuery("DELETE FROM isuf_stock");
            search();
        }

        private void cmd_excel_Click(object sender, EventArgs e)
        {
            using (var dlg = new SaveFileDialog())
            {
                dlg.FileName = string.Empty;
                dlg.Filter = "Excel (*.xlsx)|*.xlsx|Excel 97-2003 (*.xls)|*.xls|CSV (*.csv)|*.csv";
                dlg.AddExtension = true;

                if (dlg.ShowDialog(this) != DialogResult.OK) return;

                var path = dlg.FileName;
                var ext = System.IO.Path.GetExtension(path)?.ToUpperInvariant();

                if (string.IsNullOrEmpty(ext))
                {
                    path += ".xls";   // VB6 동작과 동일: 확장자 없으면 .xls 부여
                    ext = ".XLS";
                }

                try
                {
                    Cursor = Cursors.WaitCursor;

                    if (ext == ".CSV")
                    {
                        ExportGridToCsv(fpExcel, path);
                    }
                    else
                    {
                        // Excel COM(또는 대체 라이브러리) 기반 내보내기
                        ExcelWriter.ExportDataGridView(fpExcel, path);
                    }

                    Cursor = Cursors.Default;
                    MessageBox.Show($"[{path}] 로 저장이 되었습니다.", "입고등록 엑셀 저장",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    Cursor = Cursors.Default;
                    MessageBox.Show(
                        "오류번호 : " + ex.HResult + Environment.NewLine +
                        "오류내용 : " + ex.Message + Environment.NewLine + Environment.NewLine +
                        "혹시 해당이름의 파일이 열려있는지 확인하십시오!!" + Environment.NewLine +
                        "문제가 지속되면 개발자에게 문의하십시오",
                        "데이터베이스 연결 에러(cmd_excel_Click)",
                        MessageBoxButtons.OK, MessageBoxIcon.Error
                    );
                }
            }
        }
        private void ExportGridToCsv(DataGridView grid, string path)
        {
            using (var sw = new System.IO.StreamWriter(path, false, Encoding.UTF8))
            {
                // 헤더
                for (int c = 0; c < grid.Columns.Count; c++)
                {
                    if (!grid.Columns[c].Visible) continue;
                    sw.Write(grid.Columns[c].HeaderText);
                    if (c < grid.Columns.Count - 1) sw.Write(",");
                }
                sw.WriteLine();

                // 데이터
                foreach (DataGridViewRow r in grid.Rows)
                {
                    if (r.IsNewRow) continue;
                    for (int c = 0; c < grid.Columns.Count; c++)
                    {
                        if (!grid.Columns[c].Visible) continue;
                        var val = r.Cells[c].Value?.ToString() ?? string.Empty;
                        if (val.Contains("\"") || val.Contains(","))
                            val = "\"" + val.Replace("\"", "\"\"") + "\"";
                        sw.Write(val);
                        if (c < grid.Columns.Count - 1) sw.Write(",");
                    }
                    sw.WriteLine();
                }
            }
        }
        private void Sheet_Setting() // 입고등록 grid 셋팅
        {
            var g = fpExcel; // DataGridView

            // 최소 11개 컬럼 보장 (0 ~ 10)
            while (g.Columns.Count < 11)
            {
                int idx = g.Columns.Count;
                g.Columns.Add("C" + idx, "C" + idx);
            }

            Func<int, int> TwipsToPixels = tw => (int)Math.Round(tw * 96.0 / 1440.0);

            // 열 너비 (VB6 twips → pixel)
            g.Columns[0].Width = TwipsToPixels(1650); // 수주번호
            g.Columns[1].Width = TwipsToPixels(2000); // 제품코드
            g.Columns[2].Width = TwipsToPixels(1000); // 수량

            // 3~10번 열 숨김
            for (int i = 3; i <= 10; i++)
            {
                g.Columns[i].Visible = false;
                g.Columns[i].Width = 0;
                g.Columns[i].HeaderText = string.Empty;
            }

            // 헤더 정렬(0~2 열 가운데)
            for (int i = 0; i <= 2; i++)
            {
                g.Columns[i].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            // 셀 정렬(0~2 열 가운데)
            g.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            g.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            g.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // 헤더 텍스트
            g.Columns[0].HeaderText = "수주번호";
            g.Columns[1].HeaderText = "제품코드";
            g.Columns[2].HeaderText = "수량";

            // 편의 옵션(원 코드 의도에 맞춰 일반적으로 권장)
            g.AutoGenerateColumns = false;
            g.AllowUserToAddRows = false;
            g.AllowUserToDeleteRows = false;
            g.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }
    }
}
