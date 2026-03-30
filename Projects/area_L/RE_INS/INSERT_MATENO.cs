using area_L;
using area_L.Modules;
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
    public partial class INSERT_MATENO : Form
    {

        #region [1. 생성자 및 변수]

        RE_INS pfm = null; 
        
        //RE_INS_BF pfm_y = null;

        public INSERT_MATENO(RE_INS pfm)
        {
            InitializeComponent();

            this.pfm = pfm;

            //this.pfm_y = pfm_y;

            initControl();
        }
        #endregion


        #region [3. 함수]

        private void initControl()
        {
            try
            {
                this.StartPosition = FormStartPosition.CenterScreen;

                this.ControlBox = this.ShowIcon = false;

                btn_Confirm.Click += (s, e) => { fn_sendMateNo(); };

                tx_MATE_NO.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) fn_sendMateNo(); };

                //tx_GDCD.Text = pfm == null ? pfm_y.gd_cd.Text : pfm.gd_cd.Text;

                //tx_GDNM.Text = pfm == null ? pfm_y.gd_nm.Text : pfm.gd_nm.Text;

                tx_GDCD.Text = pfm.gd_cd.Text;

                tx_GDNM.Text = pfm.gd_nm.Text;

                tx_GDCD.ReadOnly = tx_GDNM.ReadOnly = true;

                tx_MATE_NO.CharacterCasing = CharacterCasing.Upper;

                tx_MATE_NO.Select();
            }
            catch(Exception ex)
            {

            }
        }

        private void fn_sendMateNo()
        {
            try
            {
                if(string.IsNullOrEmpty(tx_MATE_NO.Text) || string.IsNullOrWhiteSpace(tx_MATE_NO.Text))
                {
                    MessageBox.Show("멸균넘버란이 비어있습니다.\n멸균넘버를 입력해주세요.", "멸균넘버입력", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    tx_MATE_NO.Focus();

                    return;
                }


                //if(pfm == null) pfm_y.MATE_NO.Text = tx_MATE_NO.Text;
                //else pfm.MATE_NO.Text = tx_MATE_NO.Text;

                pfm.MATE_NO.Text = tx_MATE_NO.Text.ToUpper();

                this.Close();
            }
            catch(Exception ex)
            {

            }
        }


        #endregion
    }
}
