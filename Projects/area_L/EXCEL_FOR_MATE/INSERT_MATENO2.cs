using DevExpress.Pdf.Native.BouncyCastle.Asn1.Pkcs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace area_L
{
    public partial class INSERT_MATENO2 : Form
    {
        EXCEL_FOR_MATE pfm = null; string kind;

        public INSERT_MATENO2(EXCEL_FOR_MATE pfm, string kind)                 
        {
            InitializeComponent();

            this.pfm = pfm;

            this.kind = kind;

            initControl();
        }

        private void initControl()
        {
            try
            {
                this.StartPosition = FormStartPosition.CenterScreen;

                this.ControlBox = this.ShowIcon = false;

                btn_Confirm.Click += (s, e) => { fn_sendMateNo(); };

                tx_GDCD.Text = pfm.ScanRow["GD_CD"].ToString();

                tx_GDNM.Text = pfm.ScanRow["GD_NM"].ToString();

                tbx_LotNo.Text = pfm.ScanRow["CHECKSHEETNO"].ToString();

                tx_MATE_NO.Text = pfm.ScanRow["LOT_NO"].ToString();

                tx_MATE_NO.CharacterCasing = CharacterCasing.Upper;

                tx_GDCD.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) tx_GDNM.Select(); };

                tx_GDNM.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) tbx_LotNo.Select(); };

                tbx_LotNo.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) { tx_MATE_NO.Text = tbx_LotNo.Text; fn_sendMateNo(); } };

                //tx_MATE_NO.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) fn_sendMateNo(); };

                if (string.IsNullOrEmpty(pfm.ScanRow["CHECKSHEETNO"].ToString()))
                {
                    tbx_LotNo.Select();
                }
                else tx_MATE_NO.Select();
            }
            catch (Exception ex)
            {

            }
        }

        private void fn_sendMateNo()
        {
            try
            {
                if (string.IsNullOrEmpty(tx_MATE_NO.Text) || string.IsNullOrWhiteSpace(tx_MATE_NO.Text))
                {
                    MessageBox.Show("멸균넘버란이 비어있습니다.\n멸균넘버를 입력해주세요.", "멸균넘버입력", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    tx_MATE_NO.Focus();

                    return;
                }

                pfm.ScanRow["GD_CD"] = tx_GDCD.Text;

                pfm.ScanRow["GD_NM"] = tx_GDNM.Text;

                pfm.ScanRow["CHECKSHEETNO"] = kind.Equals("mate") ? tx_MATE_NO.Text : tbx_LotNo.Text;

                pfm.ScanRow["LOT_NO"] = tx_MATE_NO.Text;

                this.Close();
            }
            catch (Exception ex)
            {

            }
        }


    }
}
