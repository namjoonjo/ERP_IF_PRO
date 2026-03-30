using DevExpress.Utils.Serializing.Helpers;
using DevExpress.XtraEditors;
using DevExpress.XtraPrinting.BarCode;

using DevExpress.XtraSplashScreen;
using RAZER_C.Else;
using RAZER_C.Modules;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;


namespace RAZER_C
{
    public partial class Excel_QR : Form
    {
        Excel.Application excelApp = null;

        Excel.Workbook workbook = null;

        BarCodeControl barCodeControl = null;
        CommonModule cm = new CommonModule();

        private static string dbName = "ERP_2";

        MSSQL db = new MSSQL(dbName);

        private bool chkCreateBtn = false;

        public Excel_QR()
        {
            InitializeComponent();

            this.ControlBox = false;

            //this.Load += Excel_QR_Load;

            this.btn_create_label.Click += btn_create_label_Click;

            this.FormClosing += Excel_QR_FormClosing;

            this.txt_init.KeyDown += txt_init_KeyDown;

            this.txt_ColorName.KeyDown += txt_Colorname_KeyDown;

            this.txt_out_box.KeyDown += txt_out_box_KeyDown;

            this.btn_reset.Click += btn_reset_Click;

            this.txt_init.Select();

            this.txt_poNo.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) txt_brandName.Select(); };

            this.txt_brandName.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) txt_ColorName.Select(); };

            this.txt_spec.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) txt_out_box.Select(); };

            this.txt_CustName.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) txt_poNo.Select(); };

            this.tbx_totBoxCnt.KeyDown += Tbx_totBoxCnt_KeyDown;

            this.dateTimePicker1.Value = DateTime.Now.AddMonths(-3);

            this.dateTimePicker2.Value = DateTime.Now;

        }

        private void Tbx_totBoxCnt_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode != Keys.Enter) return;

                for (int i = 0; i < tbx_totBoxCnt.Text.Length; i++)
                {
                    if (!(tbx_totBoxCnt.Text[i] >= '0' && tbx_totBoxCnt.Text[i] <= '9'))
                    {
                        MessageBox.Show("숫자만 입력 가능합니다.", "입력", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                        tbx_totBoxCnt.SelectAll();

                        return;
                    }
                }

                btn_create_label.Focus();
            }
            catch (Exception ex)
            {

            }
        }

        private void Excel_QR_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (chkCreateBtn)
                {
                    //if (workSheet != null) Marshal.ReleaseComObject(workSheet);
                    if (workbook != null) Marshal.ReleaseComObject(workbook);

                    if (excelApp != null)
                    {
                        excelApp.Quit();
                        Marshal.ReleaseComObject(excelApp);
                    }

                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void Excel_QR_Load(object sender, EventArgs e)
        {
            try
            {
                txt_init.Focus();
                this.ActiveControl = txt_init;
                if (!splashScreenManager1.IsSplashFormVisible) splashScreenManager1.ShowWaitForm();

                splashScreenManager1.SetWaitFormCaption("Excel_QR을 여는중입니다..");

                splashScreenManager1.SetWaitFormDescription("잠시만 기다려 주십시오..");

                excelApp = new Excel.Application(); // Excel Application 시작

                barCodeControl = new BarCodeControl();
                barCodeControl.Visible = false;
                barCodeControl.ShowText = false;
                barCodeControl.Size = new Size(80, 80);
                barCodeControl.AutoModule = true;

                QRCodeGenerator symb = new QRCodeGenerator();
                symb.CompactionMode = QRCodeCompactionMode.Byte;
                symb.ErrorCorrectionLevel = QRCodeErrorCorrectionLevel.H;
                symb.Version = QRCodeVersion.AutoVersion;
                barCodeControl.Symbology = symb;

                if (splashScreenManager1.IsSplashFormVisible) splashScreenManager1.CloseWaitForm();
            }
            catch (Exception ex)
            {

            }
        }

        private void btn_create_label_Click(object sender, EventArgs e)
        {
            try
            {
                if (!splashScreenManager1.IsSplashFormVisible) splashScreenManager1.ShowWaitForm();

                splashScreenManager1.SetWaitFormCaption("Excel_QR 작업중..");

                splashScreenManager1.SetWaitFormDescription("엑셀 파일을 생성하고 있습니다..");

                chkCreateBtn = true;

                excelApp = new Excel.Application(); // Excel Application 시작
                excelApp.DisplayAlerts = false;

                barCodeControl = new BarCodeControl();
                barCodeControl.Visible = false;
                barCodeControl.ShowText = false;
                barCodeControl.Size = new Size(75, 75);
                barCodeControl.AutoModule = true;

                QRCodeGenerator symb = new QRCodeGenerator();
                symb.CompactionMode = QRCodeCompactionMode.Byte;
                symb.ErrorCorrectionLevel = QRCodeErrorCorrectionLevel.H;
                symb.Version = QRCodeVersion.AutoVersion;
                barCodeControl.Symbology = symb;

                string SavePath = @"C:\패킹리스트_멀티바코드";

                if (!Directory.Exists(SavePath))
                {
                    Directory.CreateDirectory(SavePath);
                }

                //string directoryPath = AppDomain.CurrentDomain.BaseDirectory;

                string directoryPath = Application.StartupPath;

                string originFileName = "QR_EXCEL_SAMPLE2.xlsx";

                string fileName = $"{txt_init.Text}_{txt_CustName.Text}_{txt_out_box.Text}_{DateTime.Now.ToString("yyyyMMddhhmmss")}.xlsx";

                File.Copy(Path.Combine(directoryPath, originFileName), Path.Combine(SavePath, fileName));

                workbook = excelApp.Workbooks.Open(Path.Combine(SavePath, fileName));

                //string ssql;

                //ssql = " select cust_code, lens, sp_power, lot_no, expir, sum(pack), replace(barcode_2, CHAR(13),'') from Packing_list_M ";
                //ssql = ssql + " where p_order = '" + txt_init.Text + "' ";
                //ssql = ssql + " and out_box = '" + txt_out_box.Text + "' ";
                //ssql = ssql + " and lens = '" + txt_lens_name.Text + "' ";
                //ssql = ssql + " and stts <> 'D' ";
                //ssql = ssql + " group by cust_code, lens, sp_power, lot_no, expir, replace(barcode_2, CHAR(13),'') ";

                //System.Data.DataTable dp_dt = null;
                //db = new Class_DB();
                //db.ConnentDB();
                //dp_dt = db.GetDBtable(ssql);

                string strSql = $"{dbName}.dbo.ST_EXCEL_QR_PACKING_LIST_M_SEL";

                db.Parameter("@P_ORDER", txt_init.Text);
                db.Parameter("@OUT_BOX", txt_out_box.Text);
                db.Parameter("@LENS", txt_ColorName.Text);
                db.Parameter("@START_DT", dateTimePicker1.Value.ToString("yyyy-MM-dd"));
                db.Parameter("@TO_DT", dateTimePicker2.Value.ToString("yyyy-MM-dd"));


                db.ExecuteSql(strSql);

                DataTable dp_dt = db.result;

                if (dp_dt.Rows.Count < 1)
                {
                    if (splashScreenManager1.IsSplashFormVisible) splashScreenManager1.CloseWaitForm();

                    MessageBox.Show("해당데이터가 없습니다. ", "확인", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    txt_init.Focus();

                    txt_init.SelectAll();

                    return;
                }

                int sheetCount = dp_dt.Rows.Count % 10 == 0 ? dp_dt.Rows.Count / 10 : (dp_dt.Rows.Count / 10) + 1;
                int[] rowCursur = new int[sheetCount];
                int rsRow = dp_dt.Rows.Count;
                int cnt = 0;
                int sheetSum = 0;
                int resultSum = 0;
                int outboxCnt = Convert.ToInt32(txt_out_box.Text);

                for (int i = 0; i < sheetCount; i++)
                {
                    rowCursur[i] = rsRow / 10 > 0 ? 10 : rsRow;

                    rsRow -= 10;
                }

                var wstemp = (Excel.Worksheet)workbook.Sheets["originSheets"];



                for (int i = 0; i < sheetCount - 1; i++)
                {
                    wstemp.Copy((Excel.Worksheet)workbook.Sheets[1]);
                }

                //((Excel.Worksheet)workbook.Sheets[1]).Delete();


                // k + i * outboxCnt + 1


                for (int i = 0; i < sheetCount; i++)
                {
                    //cnt = 0;
                    //sheetSum = 0;

                    //Excel.Worksheet ws = (Excel.Worksheet)workbook.Sheets[k + outboxCnt * i + 1];

                    Excel.Worksheet ws = (Excel.Worksheet)workbook.Sheets[i + 1];

                    ws.Cells[2, 3] = txt_CustName.Text;

                    //ws.Cells[1, 3] = k + outboxCnt * i + 1;
                    ws.Cells[1, 3] = txt_out_box.Text;

                    ws.Cells[1, 4] = $"/ {tbx_totBoxCnt.Text}";
                    ws.Cells[3, 3] = txt_poNo.Text;
                    ws.Cells[4, 3] = txt_brandName.Text;
                    ws.Cells[5, 3] = txt_ColorName.Text;
                    ws.Cells[6, 3] = txt_spec.Text;

                    for (int j = 0; j < rowCursur[i]; j++)
                    {
                        // Cells[y,x]
                        ws.Cells[j + 9, 1] = txt_out_box.Text;
                        //ws.Cells[j + 9, 1] = k + outboxCnt * i + 1;
                        ws.Cells[j + 9, 3] = dp_dt.Rows[cnt][2].ToString();
                        ws.Cells[j + 9, 4] = dp_dt.Rows[cnt][3].ToString();
                        ws.Cells[j + 9, 5] = dp_dt.Rows[cnt][4].ToString().Replace("/", "-");
                        ws.Cells[j + 9, 6] = dp_dt.Rows[cnt][5].ToString();

                        resultSum += Convert.ToInt32(dp_dt.Rows[cnt][5]);

                        sheetSum += Convert.ToInt32(dp_dt.Rows[cnt][5]);

                        fn_CreateQR(barCodeControl, ws, j + 9, 7, dp_dt.Rows[cnt][6].ToString());

                        cnt++;
                    }

                    //ws.Cells[19, 5] = "Sum : ";
                    //ws.Cells[19, 6] = sheetSum;

                }

                Excel.Worksheet wstemp2 = workbook.Sheets[sheetCount];

                wstemp2.Cells[19, 5] = "Total : ";
                wstemp2.Cells[19, 6] = resultSum;

                //wstemp2.Cells[19, 3] = "Total : ";
                //wstemp2.Cells[19, 4] = resultSum;

                if (splashScreenManager1.IsSplashFormVisible) splashScreenManager1.CloseWaitForm();


                excelApp.Visible = true;

                txt_out_box.Text = "";
                txt_out_box.Focus();
            }
            catch (Exception ex)
            {
                if (splashScreenManager1.IsSplashFormVisible) splashScreenManager1.CloseWaitForm();
            }

        }

        //public bool fn_CreateQR(BarCodeControl barCodeControl, Excel.Worksheet workSheet, int rowLoc, int colLoc, string data)
        //{
        //    try
        //    {
        //        barCodeControl.Name = data;
        //        barCodeControl.Text = data;

        //        Image image = null;

        //        while (image == null)
        //        {
        //            image = barCodeControl.ExportToImage();
        //            Console.WriteLine(image.ToString());
        //        }

        //        Clipboard.SetDataObject(image, true);

        //        Excel.Range range = (Excel.Range)workSheet.Cells[rowLoc, colLoc];

        //        Thread.Sleep(50);  // 클립보드에 bitmap형식으로 담은 객체를 넣는 시간이 걸림

        //        workSheet.Paste(range, (Bitmap)image);

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {

        //        return false;
        //    }
        //}

        public bool fn_CreateQR(BarCodeControl barCodeControl, Excel.Worksheet workSheet, int rowLoc, int colLoc, string data)
        {
            string tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".png");

            try
            {
                barCodeControl.Name = data;
                barCodeControl.Text = data;

                using (Image img = barCodeControl.ExportToImage())
                using (Bitmap bmp = new Bitmap(img))
                {
                    bmp.Save(tempPath, System.Drawing.Imaging.ImageFormat.Png);
                }

                Excel.Range cell = (Excel.Range)workSheet.Cells[rowLoc, colLoc];

                float left = (float)(double)cell.Left;
                float top = (float)(double)cell.Top;
                float width = (float)(double)cell.Width;
                float height = (float)(double)cell.Height;

                workSheet.Shapes.AddPicture(
                    tempPath,
                    Microsoft.Office.Core.MsoTriState.msoFalse,
                    Microsoft.Office.Core.MsoTriState.msoTrue,
                    left,
                    top,
                    width,
                    height
                );

                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                if (File.Exists(tempPath))
                    File.Delete(tempPath);
            }
        }



        private void txt_init_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {

                    string strSql = $"{dbName}.dbo.ST_EXCEL_QR_PACKING_LIST_M_DATA2_SEL";

                    //ssql = " select * from Packing_list_M where p_order = '" + txt_init.Text + "' ";

                    db.Parameter("@KIND", "TXT_INIT_KEYDOWN");
                    db.Parameter("@INITI", txt_init.Text);
                    db.Parameter("@OUT_BOX", txt_out_box.Text);
                    db.Parameter("@COLOR_NM", txt_ColorName.Text);
                    db.Parameter("@BRAND_NM", txt_brandName.Text);

                    db.ExecuteSql(strSql);

                    if (db.nState)
                    {
                        if (string.IsNullOrEmpty(db.sql_raise_error_msg))
                        {
                            if (db.result.Rows.Count > 1)
                            {
                                QR_SelectBox sb = new QR_SelectBox(this, db.result, "TXT_INIT_KEYDOWN");

                                sb.ShowDialog();

                                this.txt_ColorName.Select();

                            }
                            else if (db.result.Rows.Count == 1)
                            {
                                DataRow dr = db.result.Rows[0];

                                txt_CustName.Text = dr["CUST_NM"].ToString();

                                txt_CustCD.Text = dr["CS_CD"].ToString();

                                txt_poNo.Text = dr["PO_NO"].ToString();

                                this.txt_ColorName.Select();
                            }
                            else
                            {
                                MessageBox.Show($"{txt_init.Text}에 대한 정보가 없습니다.\n이니셜번호를 확인부탁드립니다.", "이니셜번호 입력", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                                this.txt_init.SelectAll();

                                return;
                            }
                        }
                        else
                        {
                            MessageBox.Show($"{db.sql_raise_error_msg}", "이니셜번호 입력", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                            this.txt_init.SelectAll();

                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void txt_Colorname_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    //ssql = " select * from Packing_list_M where lens = '" + txt_lens_name.Text + "' and p_order = '" + txt_init.Text + "' ";

                    string strSql = $"{dbName}.dbo.ST_EXCEL_QR_PACKING_LIST_M_DATA2_SEL";

                    db.Parameter("@KIND", "TXT_LENS_NAME_KEYDOWN");
                    db.Parameter("@INITI", txt_init.Text);
                    db.Parameter("@OUT_BOX", txt_out_box.Text);
                    db.Parameter("@COLOR_NM", txt_ColorName.Text);
                    db.Parameter("@BRAND_NM", string.Empty);

                    db.ExecuteSql(strSql);



                    if (db.nState)
                    {
                        if (string.IsNullOrEmpty(db.sql_raise_error_msg))
                        {
                            if (db.result.Rows.Count > 1)
                            {
                                QR_SelectBox sb = new QR_SelectBox(this, db.result, "TXT_LENS_NAME_KEYDOWN");

                                sb.ShowDialog();

                                this.txt_out_box.Select();

                            }
                            else if (db.result.Rows.Count == 1)
                            {
                                DataRow dr = db.result.Rows[0];

                                txt_brandName.Text = dr["BRAND_NM"].ToString();

                                txt_spec.Text = dr["SPEC"].ToString();

                                this.txt_out_box.Select();
                            }
                            else
                            {
                                MessageBox.Show($"{txt_init.Text}에 대한 정보가 없습니다.\n이니셜번호를 확인부탁드립니다.", "이니셜번호 입력", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                                this.txt_ColorName.SelectAll();

                                return;
                            }
                        }
                        else
                        {
                            MessageBox.Show($"{db.sql_raise_error_msg}", "컬러명 입력", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                            this.txt_init.SelectAll();

                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void txt_out_box_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {

                    for (int i = 0; i < txt_out_box.Text.Length; i++)
                    {
                        if (!(txt_out_box.Text[i] >= '0' && txt_out_box.Text[i] <= '9'))
                        {
                            MessageBox.Show("숫자만 입력 가능합니다.", "입력", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                            txt_out_box.SelectAll();

                            return;
                        }
                    }

                    string strSql = $"{dbName}.dbo.ST_EXCEL_QR_PACKING_LIST_M_DATA2_SEL";

                    //ssql = " select * from Packing_list_M where lens = '" + txt_lens_name.Text + "' and p_order = '" + txt_init.Text + "' and out_box = '" + txt_out_box.Text + "' ";

                    db.Parameter("@KIND", "TXT_OUT_BOX_KEYDOWN");
                    db.Parameter("@INITI", txt_init.Text);
                    db.Parameter("@OUT_BOX", txt_out_box.Text);
                    db.Parameter("@COLOR_NM", txt_ColorName.Text);
                    db.Parameter("@BRAND_NM", txt_brandName.Text);


                    db.ExecuteSql(strSql);

                    if (db.nState)
                    {
                        if (!string.IsNullOrEmpty(db.sql_raise_error_msg))
                        {
                            MessageBox.Show($"{db.sql_raise_error_msg}", "아웃박스 수량 입력", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void btn_reset_Click(object sender, EventArgs e)
        {
            try
            {
                txt_init.Text = string.Empty;
                txt_CustName.Text = string.Empty;
                txt_out_box.Text = string.Empty;
                txt_poNo.Text = string.Empty;
                txt_brandName.Text = string.Empty;
                txt_ColorName.Text = string.Empty;
                txt_spec.Text = string.Empty;

                txt_init.Select();
            }
            catch (Exception ex)
            {

            }
        }
    }
}
