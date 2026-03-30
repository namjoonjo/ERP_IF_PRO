using DevExpress.Pdf.Native.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COMBINATION.Modules
{
    public class PrintInfo
    {
        private string LotNo;

        private string proDate;

        private string valiDate;

        private string GD_NM;

        private int cl_r; private int cl_g; private int cl_b;

        public void setLotNo(string LotNo) { this.LotNo = LotNo; }

        public void setproDate(string proDate) { this.proDate = proDate; }

        public void setvaliDate(string valiDate) { this.valiDate = valiDate; }

        public void setGD_NM(string GD_NM) { this.GD_NM = GD_NM; }

        public void setRGB(int r,int g,int b) { this.cl_r = r; this.cl_g = g; this.cl_b = b; }

        public string getLotNo() { return LotNo; }

        public string getproDate() { return proDate; }

        public string getvaliDate() { return valiDate; }

        public string getGD_NM() { return GD_NM; }

        public int getR() { return cl_r; }

        public int getG() { return cl_g; }

        public int getB() { return cl_b; }
    }
}
