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
    public partial class INSERT_PALLET_INFO : Form
    {

        Rules rClass = null;

        public INSERT_PALLET_INFO(Rules rClass)
        {
            InitializeComponent();

            this.rClass = rClass;

            this.StartPosition = FormStartPosition.CenterScreen;

            this.tbx_Date.Text = DateTime.Now.ToString("yyMMdd");

            cbx_FACCD.Items.Add("A");
            cbx_FACCD.Items.Add("C");

            cbx_FACCD.SelectedIndex = 1;

            cbx_FACCD.SelectedIndexChanged += (s, e) => {  tbx_orderNo.Select(); };

            tbx_orderNo.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    processing();
                }
            };

            btn_Confirm.Click += (s,e) => { processing(); };

            tbx_orderNo.Select();
        }

        private void processing()
        {
            try
            {
                if (string.IsNullOrEmpty(tbx_orderNo.Text))
                {
                    MessageBox.Show("오더번호를 입력해 주십시오.", "입력", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    return;
                }

                rClass.Rulestr = $"{tbx_orderNo.Text}_{tbx_Date.Text}_{cbx_FACCD.Text}_";

                rClass.FacCd = cbx_FACCD.Text.Trim();

                rClass.OrderNo = tbx_orderNo.Text;

                rClass.Date = tbx_Date.Text;

                this.Close();
            }
            catch(Exception ex)
            {

            }
        }
    }
}
