using DevExpress.XtraReports.UI;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;

namespace area_L.Labels
{
    public partial class Pallet_Label : DevExpress.XtraReports.UI.XtraReport
    {
        public Pallet_Label(string PalletNo)
        {
            InitializeComponent();

            this.PalletNo.Text = PalletNo;

            this.Pallet_2D.Text = PalletNo;

            this.ShowPrintMarginsWarning = false;
        }

    }
}
