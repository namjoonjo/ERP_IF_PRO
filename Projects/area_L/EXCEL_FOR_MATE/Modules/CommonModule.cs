
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;

namespace area_L.Modules
{
    /***********************
     * 
     * 1. 공통모듈 함수 입니다. 거의 모든 폼에 사용되니 폼 새로 생성시 전역변수에 선언하여 사용하기를 권장합니다.
     * 
     * 2. 참조가 걸려있지 않은 함수들(쓰지 않는 함수)은 삭제해도 무관합니다.
     * 
     ***********************/
    class CommonModule
    {
        public string DataTblToXML(System.Data.DataTable dt)
        {
            try
            {
                DataSet dsTemp = new DataSet("NewDataSet");

                dt.TableName = "nodes";

                dsTemp.Tables.Add(dt);

                return dsTemp.GetXml();
            }
            catch (Exception ex)
            {
                //writeLog($"CommonModule DataTblToXML Error : {ex.Message}");
            }

            return string.Empty;
        }

        public void ComboBoxBinding(System.Data.DataTable dtTbl, System.Windows.Forms.ComboBox combo, string code)
        {
            try
            {
                DataRow[] dr = dtTbl.Select($"KIND = '{code}'");

                foreach (DataRow ddr in dr)
                {
                    combo.Items.Add(ddr["COMBO_STR"].ToString());
                }
            }
            catch (Exception ex)
            {
                writeLog($"CommonModule ComboBoxBInding Error : {ex.Message}");
            }
        }

        public void ComboBoxBinding(System.Data.DataTable dtTbl, System.Windows.Forms.ComboBox combo)
        {
            try
            {
                foreach (DataRow dr in dtTbl.Rows)
                {
                    combo.Items.Add(dr["COMBO_STR"].ToString());
                }
            }
            catch (Exception ex)
            {
                writeLog($"CommonModule ComboBoxBInding Error : {ex.Message}");
            }
        }

        public void ComboBoxBinding(List<string> slist, System.Windows.Forms.ComboBox combo)
        {
            try
            {
                foreach (string str in slist)
                {
                    combo.Items.Add(str);
                }
            }
            catch (Exception ex)
            {
                writeLog($"CommonModule ComboBoxBInding Error : {ex.Message}");
            }
        }

        public void ComboBoxBinding(string[] slist, System.Windows.Forms.ComboBox combo)
        {
            try
            {
                foreach (string str in slist)
                {
                    combo.Items.Add(str);
                }
            }
            catch (Exception ex)
            {
                writeLog($"CommonModule ComboBoxBInding Error : {ex.Message}");
            }
        }



        // Right함수
        // vb의 right함수와 동일하게 구현함.
        public string Right(string str, int Length)
        {
            try
            {
                if (str.Length < Length)
                    Length = str.Length;

                return str.Substring(str.Length - Length, Length);
            }
            catch (Exception ex)
            {
                writeLog($"CommonModule Right Error : {ex.Message}");

                return string.Empty;
            }
        }


        public void fn_Txtwrite(string path, string content)
        {
            try
            {
                StreamWriter writer;
                writer = File.AppendText(path);
                writer.WriteLine(content);
                writer.Close();
            }
            catch (Exception ex)
            {
                writeLog($"CommonModule fn_Txtwrite Error : {ex.ToString()}");
            }
        }

        public string createLogFile()
        {
            try
            {
                string log_path = ConfigurationManager.AppSettings["logfilePath"].ToString();

                DirectoryInfo di = new DirectoryInfo(log_path);

                if (!di.Exists)
                {
                    di.Create();
                }

                return "ok";
            }
            catch (Exception ex)
            {
                return $"CommonModule createLogFile {ex.Message} {DateTime.Now.ToString("yyyy-MM-dd")}";
            }
        }

        // 로그 기록 함수
        // Appconfig의 logfilePath에 저장된 경로의 메모장에 로그를 기록합니다.
        public void writeLog(string logStr)
        {
            try
            {
                string log_path = ConfigurationManager.AppSettings["logfilePath"].ToString();
                string fileName = $"{DateTime.Now.ToString("yyyy-MM-dd")}_syslog";
                string fileExtension = ".txt";

                string comPath = $"{log_path}\\{fileName}{fileExtension}";

                if (!File.Exists(comPath))
                {
                    File.WriteAllText(comPath, $"======= {fileName} 로그파일 입니다. =======\n");
                }

                using (StreamWriter wr = new StreamWriter(comPath))
                {
                    wr.WriteLine($"[{DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")}] {logStr}");
                }
            }
            catch (Exception ex)
            {
                //..로그 쓰기 오류
            }
        }


        public void RunExcel(string path)
        {
            try
            {
                ProcessStartInfo info = new ProcessStartInfo("excel.exe", path);
                Process.Start(info);
            }
            catch (Exception ex)
            {
                writeLog($"CommonModule RunExcel Error : ${ex.Message}");
            }
        }

        public string GetResourcePath()
        {

            try
            {
                string strAppPath = Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);

                string strFilePath = Path.Combine(strAppPath, "Resources");

                return  string.IsNullOrEmpty(strFilePath)  ? "" : strFilePath;
            }
            catch (Exception ex)
            {
                writeLog($"CommonModule GetResourceFileName Error : ${ex.Message}");
                return string.Empty;
            }

        }

        public void CreateQRFolder()
        {
            try
            {
                string filePath = "C:\\qrFiles";

                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }
            }
            catch (Exception ex)
            {
                writeLog($"CommonModule CreateQRFolder Error : ${ex.Message}");
            }
        }

        public bool CreateFolder(string filePath)
        {
            try
            {
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }

                return true;
            }
            catch (Exception ex)
            {
                writeLog($"CommonModule CreateQRFolder Error : ${ex.Message}");

                return false;
            }
        }

        // 레이저마킹 프로그램에서 문자열을 다시 재조합하는 함수입니다.
        public string reCombineStr(string[] arr, char spliter)
        {
            try
            {
                string rs = string.Empty;

                rs = arr[0];
                for (int i = 1; i < arr.Length; i++)
                {
                    rs += $"{spliter}{arr[i]}";
                }

                return rs;
            }
            catch (Exception ex)
            {
                writeLog($"CommonModule reCombineStr Error : ${ex.Message}");

                return string.Empty;
            }
        }

        public void CreateFileText()
        {
            try
            {

            }
            catch (Exception ex)
            {
                writeLog($"CommonModule CreateFile Error : ${ex.Message}");

            }
        }

        public bool LoadForm(TabControl tabControl, string formName, string menuName)
        {
            try
            {
                foreach (TabPage tb in tabControl.TabPages)
                {
                    if (tb.Name.Equals(formName))
                    {
                        tabControl.SelectedTab = tb;
                        return true;
                    }
                }

                var frm = (Form)Activator.CreateInstance(Type.GetType($"area_L.{formName}"));

                frm.TopLevel = false;

                frm.FormBorderStyle = FormBorderStyle.None;

                frm.Dock = DockStyle.Fill;

                frm.Visible = true;

                TabPage tp = new TabPage();
                tp.Name = formName;
                tp.Text = menuName;

                tp.Controls.Add(frm);

                tabControl.TabPages.Add(tp);

                tabControl.SelectedTab = tp;

                return true;

            }
            catch (Exception ex)
            {
                writeLog($"CommonModule CreateFile Error : ${ex.Message}");

                return false;
            }
        }

        public bool LoadForm(TabControl tabControl, string formName, string menuName, bool DockOrNot, float menuHeight)
        {
            try
            {
                foreach (TabPage tb in tabControl.TabPages)
                {
                    if (tb.Name.Equals(formName))
                    {
                        tabControl.SelectedTab = tb;
                        return true;
                    }
                }

                var frm = (Form)Activator.CreateInstance(Type.GetType($"area_L.{formName}"));

                frm.TopLevel = false;

                frm.FormBorderStyle = FormBorderStyle.None;

                frm.Dock = DockOrNot ? DockStyle.Fill : DockStyle.None;

                frm.Visible = true;

                frm.Location = new Point((Screen.PrimaryScreen.Bounds.Size.Width / 2) - (frm.Size.Width / 2), (Screen.PrimaryScreen.Bounds.Size.Height / 2) - (frm.Size.Height / 2) - ((int)menuHeight * 2));

                TabPage tp = new TabPage();
                tp.Name = formName;
                tp.Text = menuName;

                tp.Controls.Add(frm);

                tabControl.TabPages.Add(tp);

                tabControl.SelectedTab = tp;

                return true;

            }
            catch (Exception ex)
            {
                writeLog($"CommonModule CreateFile Error : ${ex.Message}");

                return false;
            }
        }

        public bool LoadForm(TabControl tabControl, Label lb_Status, Label lb_Status2, string formName, string menuName, bool DockOrNot, bool isMaximize, float menuHeight)
        {
            try
            {
                foreach (TabPage tb in tabControl.TabPages)
                {
                    if (tb.Name.Equals(formName))
                    {
                        tabControl.SelectedTab = tb;
                        return true;
                    }
                }

                var frm = (Form)Activator.CreateInstance(Type.GetType($"area_L.{formName}"));

                frm.TopLevel = false;

                frm.FormBorderStyle = FormBorderStyle.None;

                //frm.Dock = DockOrNot ? DockStyle.Fill : DockStyle.None;

                //frm.Location = new Point((tabControl.Size.Width / 2) - (frm.Size.Width / 2), (tabControl.Size.Height / 2) - (frm.Size.Height / 2) - ((int)menuHeight * 2));

                frm.Dock = DockStyle.Fill;

                frm.Show();

                TabPage tp = new TabPage();
                tp.Name = formName;
                tp.Text = menuName;

                tp.Controls.Add(frm);

                tabControl.TabPages.Add(tp);

                tabControl.SelectedTab = tp;

                frm.Focus();

                return true;

            }
            catch (Exception ex)
            {
                writeLog($"CommonModule CreateFile Error : ${ex.Message}");

                return false;
            }
        }

        //public bool LoadForm(Main pfm, TabControl tabControl, string formName, string menuName, bool DockOrNot, bool isMaximize, float menuHeight)
        //{
        //    try
        //    {
        //        foreach (TabPage tb in tabControl.TabPages)
        //        {
        //            if (tb.Name.Equals(formName))
        //            {
        //                tabControl.SelectedTab = tb;
        //                return true;
        //            }
        //        }

        //        var frm = (Form)Activator.CreateInstance(Type.GetType($"area_L.{formName}"));

        //        frm.TopLevel = false;

        //        frm.FormBorderStyle = FormBorderStyle.None;

        //        frm.Dock = DockOrNot ? DockStyle.Fill : DockStyle.None;

        //        frm.Visible = true;

        //        frm.Location = isMaximize ? new Point((Screen.PrimaryScreen.Bounds.Size.Width / 2) - (frm.Size.Width / 2), (Screen.PrimaryScreen.Bounds.Size.Height / 2) - (frm.Size.Height / 2) - ((int)menuHeight * 2)) : new Point(0, 0);

        //        TabPage tp = new TabPage();
        //        tp.Name = formName;
        //        tp.Text = menuName;

        //        tp.Controls.Add(frm);

        //        tabControl.TabPages.Add(tp);

        //        tabControl.SelectedTab = tp;

        //        return true;

        //    }
        //    catch (Exception ex)
        //    {
        //        writeLog($"CommonModule CreateFile Error : ${ex.Message}");

        //        return false;
        //    }
        //}

        public bool LoadForm(Form parentForm, TabControl tabControl, string formName, string menuName, bool DockOrNot, float menuHeight)
        {
            try
            {
                foreach (TabPage tb in tabControl.TabPages)
                {
                    if (tb.Name.Equals(formName))
                    {
                        tabControl.SelectedTab = tb;
                        return true;
                    }
                }

                var frm = (Form)Activator.CreateInstance(Type.GetType($"area_L.{formName}"));

                frm.TopLevel = false;

                frm.FormBorderStyle = FormBorderStyle.None;

                frm.Dock = DockOrNot ? DockStyle.Fill : DockStyle.None;

                frm.Visible = true;

                frm.Location = new Point((parentForm.Size.Width / 2) - (frm.Size.Width / 2), (parentForm.Size.Height / 2) - (frm.Size.Height / 2) - ((int)menuHeight * 2));

                TabPage tp = new TabPage();
                tp.Name = formName;
                tp.Text = menuName;

                tp.Controls.Add(frm);

                tabControl.TabPages.Add(tp);

                tabControl.SelectedTab = tp;

                return true;

            }
            catch (Exception ex)
            {
                writeLog($"CommonModule CreateFile Error : ${ex.Message}");

                return false;
            }
        }

        // 메모장에 텍스트를 쓰는 함수입니다.
        public void FileWrite(string FolderPath, string FileName, string content)
        {
            try
            {

                DirectoryInfo di = new DirectoryInfo(FolderPath);

                if (!di.Exists)
                {
                    di.Create();
                }

                string filePath = $"{FolderPath}\\{FileName}";

                using (var sw = new StreamWriter(filePath))
                {
                    sw.WriteLine(content);

                    sw.Close();
                }
            }
            catch (Exception ex)
            {
                writeLog($"CommonModule File_Clear Error : {ex.Message}");
            }
        }

        public void SetGridRowHeader(DataGridView dg, int rowHeight)
        {
            try
            {
                dg.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;

                dg.ColumnHeadersHeight = 40;

                dg.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

                dg.EnableHeadersVisualStyles = false;

                dg.ColumnHeadersDefaultCellStyle.BackColor = Color.LightGray;

                dg.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                dg.RowHeadersVisible = false;

                dg.RowTemplate.Height = rowHeight != -1 ? rowHeight : dg.RowTemplate.Height;

                dg.RowTemplate.Resizable = DataGridViewTriState.False;

                dg.AllowUserToAddRows = false;

                dg.ReadOnly = true;

                dg.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            }
            catch (Exception ex)
            {
                writeLog($"CommonModule SetGridRowHeader Error : {ex.Message}");
            }
        }


        public bool CheckImageExist(DataGridView dg)
        {
            try
            {
                foreach (DataGridViewRow row in dg.Rows)
                {
                    if (row.Cells[0].Value == null) continue;

                    return true;
                }

                return false;
            }
            catch(Exception ex)
            {
                writeLog($"CommonModule SetCheckImage Error : {ex.Message}");

                return false;
            }
        }

        public void SetGridCombo(DataGridView dg, string colName, string[] items, string value)
        {
            try
            {
                DataGridViewComboBoxCell cb = new DataGridViewComboBoxCell();

                cb.Value = value;
                
                foreach(string s in items)
                {
                    cb.Items.Add(s);
                }

                dg.Rows[dg.Rows.Count - 1].Cells[colName] = cb;
            }
            catch(Exception ex)
            {
                writeLog($"CommonModule SetCheckImage Error : {ex.Message}");
            }
        }

        public void releaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);

                obj = null;
            }
            catch (Exception e)
            {
                obj = null;
            }

            finally
            {
                GC.Collect();
            }
        }

        public void ReleaseExcelObject(object obj)
        {
            try
            {
                if (obj != null)
                {
                    Marshal.ReleaseComObject(obj);
                    obj = null;
                }
            }
            catch (Exception ex)
            {
                obj = null;
                throw ex;
            }
            finally
            {
                GC.Collect();
            }
        }

        public void GridToExportExcel(string fileName,string kind,DataGridView dg)
        {
            try
            {
                bool IsExport = false;

                Excel._Application excel = new Excel.Application();
                Excel._Workbook workbook = excel.Workbooks.Add(Type.Missing);
                Excel._Worksheet worksheet = null;

                worksheet = (Excel.Worksheet)workbook.ActiveSheet;

                int cellRowIndex = 1;
                int cellColumnIndex = 1;

                for (int col = 0; col < dg.Columns.Count; col++)
                {
                    if (cellRowIndex == 1)
                    {
                        worksheet.Cells[cellRowIndex, cellColumnIndex] = dg.Columns[col].HeaderText;
                    }
                    cellColumnIndex++;
                }

                cellColumnIndex = 1;
                cellRowIndex++;
                for (int row = 0; row < dg.Rows.Count - 1; row++)
                {
                    for (int col = 0; col < dg.Columns.Count; col++)
                    {
                        worksheet.Cells[cellRowIndex, cellColumnIndex] = dg.Rows[row].Cells[col].Value.ToString();
                        cellColumnIndex++;
                    }
                    cellColumnIndex = 1;
                    cellRowIndex++;
                }

                SaveFileDialog saveFileDialog = new SaveFileDialog();

                saveFileDialog.Title = "Save as Excel File";

                saveFileDialog.Filter = "Excel Files(2003)|*.xls|Excel Files(2007)|*.xlsx";

                saveFileDialog.FileName = $"{fileName}_{kind}_{DateTime.Now.ToString("yyyyMMddhhmmss")}";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    workbook.SaveAs(saveFileDialog.FileName);
                    IsExport = true;
                }
                if (IsExport)
                {
                    workbook.Close();
                    excel.Quit();
                    workbook = null;
                    excel = null;
                }
            }
            catch(Exception ex)
            {

            }
        }

        public void fn_GridRowClear(GridView dg,int rowcount)
        {
            try
            {
                for(int i=0;i<rowcount;i++)
                {
                    dg.DeleteRow(0);
                }
            }
            catch(Exception ex)
            {

            }
        }

        public void GridSelectedRowClear(GridView dg,int count)
        {
            try
            {
                for(int i = 0; i < count; i++)
                {
                    dg.UnselectRow(dg.GetSelectedRows()[0]);
                }

                
            }
            catch(Exception ex)
            {

            }
        }

        public void GridToExportExcelforDevExpressGrid(string fileName, string kind, GridView dg)
        {
            try
            {
                bool IsExport = false;

                Excel._Application excel = new Excel.Application();
                Excel._Workbook workbook = excel.Workbooks.Add(Type.Missing);
                Excel._Worksheet worksheet = null;

                worksheet = (Excel.Worksheet)workbook.ActiveSheet;

                int cellRowIndex = 1;
                int cellColumnIndex = 1;

                for (int col = 1; col < dg.Columns.Count; col++)
                {
                    if (cellRowIndex == 1)
                    {
                        worksheet.Cells[cellRowIndex, cellColumnIndex] = dg.Columns[col].Caption;
                    }
                    cellColumnIndex++;
                }

                cellColumnIndex = 1;
                cellRowIndex++;

                int gridRcnt = dg.RowCount;
                for (int row = 0; row < gridRcnt; row++)
                {
                    for (int col = 1; col < dg.Columns.Count; col++)
                    {
                        worksheet.Cells[cellRowIndex, cellColumnIndex] = dg.GetRowCellDisplayText(row, dg.Columns[col]);// dg.Rows[row].Cells[col].Value.ToString();
                        cellColumnIndex++;
                    }
                    cellColumnIndex = 1;
                    cellRowIndex++;
                }

                SaveFileDialog saveFileDialog = new SaveFileDialog();

                saveFileDialog.Title = "Save as Excel File";

                saveFileDialog.Filter = "Excel Files(2016)|*.xlsx";// "Excel Files(2003)|*.xls|Excel Files(2016)|*.xlsx";

                saveFileDialog.FileName = $"{fileName}_{kind}_{DateTime.Now.ToString("yyyyMMddhhmmss")}";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    workbook.SaveAs(saveFileDialog.FileName);
                    IsExport = true;
                }
                if (IsExport)
                {
                    workbook.Close();
                    excel.Quit();
                    workbook = null;
                    excel = null;
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
