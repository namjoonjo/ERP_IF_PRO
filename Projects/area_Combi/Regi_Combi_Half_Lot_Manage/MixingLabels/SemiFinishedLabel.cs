using COMBINATION.Modules;
using DevExpress.XtraReports.UI;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;

namespace COMBINATION.MixingLabel
{
    public partial class SemiFinishedLabel : DevExpress.XtraReports.UI.XtraReport
    {
        public SemiFinishedLabel(PrintInfo pf)
        {
            InitializeComponent();

            this.MixingNo.Text = pf.getLotNo();

            this.MixingDate.Text = pf.getproDate();

            this.MixingValidate.Text = pf.getvaliDate();

            this.xrBarCode1.Text = pf.getLotNo();

            this.GD_NM.Text = pf.getGD_NM();

            this.lb_top.BackColor = this.lb_bottom.BackColor = this.lb_left.BackColor = this.lb_right.BackColor = Color.FromArgb(pf.getR(), pf.getG(), pf.getB());
        }

    }
}
