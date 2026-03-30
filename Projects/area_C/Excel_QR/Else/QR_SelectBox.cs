using DevExpress.XtraEditors.Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RAZER_C.Else
{
    public partial class QR_SelectBox : Form
    {
        Excel_QR pfm; DataTable Seldt;

        public QR_SelectBox(Excel_QR pfm, DataTable Seldt, string kind)
        {
            InitializeComponent();

            this.pfm = pfm; this.Seldt = Seldt;

            initControl(kind);
        }

        private void initControl(string kind)
        {
            try
            {
                this.StartPosition = FormStartPosition.CenterScreen;

                fn_BindingSelectData();

                btn_Confirm.Click += (s, e) => fn_Confirm(kind);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void fn_BindingSelectData()
        {
            try
            {
                this.gridControl1.DataSource = Seldt;

                gridView1.Columns["SEL"].Caption = "선택";
                gridView1.Columns["CUST_NM"].Caption = "거래처명";


                gridView1.Columns["SEL"].Width = 50;

                gridView1.Columns["CUST_NM"].Width = 150;


                //gridView1.Columns["INITI"].OptionsColumn.AllowEdit = false;
                gridView1.Columns["CUST_NM"].OptionsColumn.AllowEdit = false;


                RepositoryItemCheckEdit ri = new RepositoryItemCheckEdit();

                ri.ValueChecked = "True";

                ri.ValueUnchecked = "False";

                ri.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;

                ri.CheckBoxOptions.Style = DevExpress.XtraEditors.Controls.CheckBoxStyle.SvgCheckBox1;

                gridView1.Columns["SEL"].ColumnEdit = ri;

                gridView1.OptionsView.ShowIndicator = false;

                gridView1.OptionsView.ColumnHeaderAutoHeight = DevExpress.Utils.DefaultBoolean.True;

            }
            catch (Exception ex)
            {

            }
        }

        private void fn_Confirm(string kind)
        {
            try
            {
                DataRow[] dr = Seldt.Select("SEL = 'True'");
                if (dr.Length == 0)
                {
                    MessageBox.Show("선택된 데이터가 없습니다.");
                    return;
                }

                if (kind.Equals("TXT_INIT_KEYDOWN"))
                {
                    pfm.txt_CustName.Text = dr[0]["CUST_NM"].ToString();

                    pfm.txt_poNo.Text = dr[0]["PO_NO"].ToString();

                    pfm.txt_CustCD.Text = dr[0]["CS_CD"].ToString();
                }
                else
                {
                    pfm.txt_brandName.Text = dr[0]["BRAND_NM"].ToString();

                    pfm.txt_spec.Text = dr[0]["SPEC"].ToString();
                }

                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
