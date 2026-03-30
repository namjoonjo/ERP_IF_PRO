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
    public partial class RE_INSERT_FORM : Form
    {
        public string rt_no = string.Empty;
        public string gd_cd = string.Empty;
        public string mate_no = string.Empty;

        public RE_INSERT_FORM(ScanItem emptyitem)
        {
            InitializeComponent();

            rt_no = emptyitem.Barcode;
            gd_cd = emptyitem.Gdcd;
            mate_no = emptyitem.Mate;
            InitControl();
        }

        public RE_INSERT_FORM(string rtno, string gdcd)
        {
            InitializeComponent();

            rt_no = rtno;
            gd_cd = gdcd;

            InitControl();
        }

        public void InitControl()
        {
            tx_BARCODE.Text = rt_no;
            tx_GDNM.Text = gd_cd;
            tx_MATE_NO.Text = mate_no;
        }

        private void btn_Confirm_Click(object sender, EventArgs e)
        {
            if (tx_MATE_NO.Text == string.Empty)
            {
                MessageBox.Show("멸균번호를 입력하세요.");
                return;
            }

            rt_no = tx_BARCODE.Text;
            gd_cd = tx_GDNM.Text;
            mate_no = tx_MATE_NO.Text;

            
            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void tx_MATE_NO_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                if(tx_MATE_NO.Text == string.Empty)
                {
                    MessageBox.Show("멸균번호를 입력하세요.");
                    return;
                }
                rt_no = tx_BARCODE.Text;
                gd_cd = tx_GDNM.Text;
                mate_no = tx_MATE_NO.Text;

                
                DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            // 아직 DialogResult가 정해지지 않았을 때만 Cancel로 세팅
            if (this.DialogResult == DialogResult.None)
            {
                this.DialogResult = DialogResult.Cancel;
            }
        }
    }
}
