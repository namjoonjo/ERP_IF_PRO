using COMBINATION.Modules;
using DevExpress.XtraReports.UI;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;

namespace COMBINATION.MixingLabel
{
    public partial class CODEX_MixingLabelforZebra : DevExpress.XtraReports.UI.XtraReport
    {
        public CODEX_MixingLabelforZebra(PrintInfo pf)
        {
            InitializeComponent();

            this.MixingNo.Text = pf.getLotNo();

            this.MixingValidate.Text = pf.getvaliDate();

            this.xrBarCode1.Text = pf.getLotNo();

            this.lb_top.BackColor = this.lb_bottom.BackColor = this.lb_left.BackColor = this.lb_right.BackColor = Color.FromArgb(pf.getR(), pf.getG(), pf.getB());
        }

    }
}
