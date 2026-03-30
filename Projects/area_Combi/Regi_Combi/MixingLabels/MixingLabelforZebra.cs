using COMBINATION.Modules;
#if !REGI_COMBI_DLL
using DevExpress.PerformanceTests.PerfomanceMonitor;
#endif
using DevExpress.XtraReports.UI;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;

namespace COMBINATION.Label
{
    public partial class MixingLabelforZebra : DevExpress.XtraReports.UI.XtraReport
    {
        public MixingLabelforZebra(PrintInfo pf)
        {
            InitializeComponent();

            this.MixingNo.Text = pf.getLotNo();

            this.MixingDate.Text = pf.getproDate();

            this.MixingValidate.Text = pf.getvaliDate();

            this.xrBarCode1.Text = pf.getLotNo();

            this.GD_NM.Text = pf.getGD_NM();

            this.lb_top.BackColor = this.lb_bottom.BackColor = this.lb_left.BackColor = this.lb_right.BackColor = Color.FromArgb(pf.getR(),pf.getG(),pf.getB());
        }

    }
}
