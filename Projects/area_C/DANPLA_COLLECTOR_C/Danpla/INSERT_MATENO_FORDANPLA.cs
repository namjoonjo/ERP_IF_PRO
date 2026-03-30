using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RAZER_C.Danpla
{
    public partial class INSERT_MATENO_FORDANPLA : Form
    {
        DANPLA_COLLECTOR_C pfm;

        public INSERT_MATENO_FORDANPLA(DANPLA_COLLECTOR_C pfm, string GD_CD, string GD_NM)
        {
            InitializeComponent();

            this.pfm = pfm;

            initControl(GD_CD, GD_NM);
        }

        private void initControl(string GD_CD, string GD_NM)
        {
            try
            {
                this.StartPosition = FormStartPosition.CenterScreen;

                this.ControlBox = this.ShowIcon = false;

                btn_Confirm.Click += (s, e) => { fn_sendMateNo(); };

                tx_MATE_NO.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) fn_sendMateNo(); };

                tx_GDCD.Text = GD_CD;

                tx_GDNM.Text = GD_NM;

                tx_GDCD.ReadOnly = tx_GDNM.ReadOnly = true;

                tx_MATE_NO.CharacterCasing = CharacterCasing.Upper;

                tx_MATE_NO.Select();
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

                pfm.tbx_result_MATE_NO.Text = tx_MATE_NO.Text;

                pfm.tbx_LotNo.Text = tx_MATE_NO.Text;

                this.Close();
            }
            catch (Exception ex)
            {

            }
        }
    }
}
