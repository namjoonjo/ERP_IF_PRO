using DevExpress.XtraReports.UI;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;

namespace RAZER_C.Labels
{
    public partial class Pallet_Label : DevExpress.XtraReports.UI.XtraReport
    {
        public Pallet_Label(string PalletNo)
        {
            InitializeComponent();

            this.PalletNo.Text = PalletNo.ToUpper();

            this.Pallet_2D.Text = PalletNo;

            this.ShowPrintMarginsWarning = false;
        }

    }
}
