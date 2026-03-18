using COMBINATION;
using ERP_IF_PRO.Modules;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace COMBINATION.Modules
{
    public partial class InputPw : Form
    {
        #region [1. 생성자 및 변수]

        CommonModule cm = new CommonModule();

        public Regi_Combi pfm;



        public InputPw(Regi_Combi pfm, object pfmTest, string text)
        {
            InitializeComponent();

            this.pfm = pfm;

            InitControl(text);
        }

        #endregion

        #region [2. 이벤트 함수]
        #endregion

        #region [3. 함수]

        private void InitControl(string text)
        {
            try
            {
                this.Text = text;

                this.StartPosition = FormStartPosition.CenterScreen;

                this.FormBorderStyle = FormBorderStyle.FixedSingle;

                this.MaximizeBox = false;

                this.MinimizeBox = false;

                this.ControlBox = false;

                this.Size = new System.Drawing.Size(400, 200);

                this.lb_Cmd.Text = "비밀번호를 입력하세요.";

                this.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) this.Close(); };

                this.btn_Confirm.Click += (s, e) => { fn_Confirmed(); };

                this.btn_Cancel.Click += (s, e) => { fn_Cancel();  };

                this.tx_pw_input.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) fn_Confirmed(); };

                this.tx_pw_input.KeyDown += (s, e) => { if (e.KeyCode == Keys.Escape) fn_Cancel(); };
            }
            catch (Exception ex)
            {
                cm.writeLog($"PWForm InitControl Error : {ex.Message}");
            }
        }

        private void fn_Cancel()
        {
            try
            {
                this.Close();

                if(pfm != null)
                {
                    this.pfm.chk_StockQty.Checked = false;

                    this.pfm.chk_byhand.Checked = false;
                }


            }
            catch(Exception ex)
            {

            }
        }


        private void fn_Confirmed()
        {
            try
            {

                if (string.IsNullOrEmpty(this.tx_pw_input.Text) || string.IsNullOrWhiteSpace(this.tx_pw_input.Text))
                {
                    MessageBox.Show("비밀번호를 입력하지 않았습니다.\n확인부탁드립니다.");

                    this.tx_pw_input.Focus();

                    return;
                }

                if(pfm != null)
                {
                    if (this.tx_pw_input.Text.Equals(pfm.PwStr))
                    {
                        pfm.txtLotNo.SelectAll();

                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("비밀번호가 일치하지 않습니다.");

                        this.tx_pw_input.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                cm.writeLog($"PWForm fn_Confirmed Error : {ex.Message}");
            }
        }

        #endregion
    }
}
