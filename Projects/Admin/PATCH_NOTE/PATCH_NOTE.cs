using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace COMBINATION
{
    public partial class PATCH_NOTE : Form
    {
        public PATCH_NOTE()
        {
            InitializeComponent();

            initControl();
        }

        private void initControl()
        {
            try
            {
                patchBox.Text = "--25.10.15 Ver 1.0.0.79\n\n\n" +
                    "1. 약품 2D -> QR 바코드로 적용.(10.20일 적용예정. 기존(2D barcode)대로 원복완료.)\n\n" +
                    "2. '실시간 생산실적 이전'화면 추가." +
                    "\n\n\n\n" +
                    "--25.10.16 Ver 1.0.0.80\n\n\n" +
                    "1. QR바코드 적용 미정. 추후 다시 논의.\n\n" +
                    "2. 반제품 제조 내역 등록 및 관리 화면에 QR라벨로 나오도록 수정 완료." +
                    "\n\n\n" +
                    "--25.10.17 Ver 1.0.0.99\n\n\n" +
                    "1. 실시간 생산실적 이전 화면 데이터 이전되지 않는현상 수정." +
                    "\n\n\n" +
                    "--26.01.05 Ver 1.0.0.106\n\n\n" +
                    "1. CODEX 명명체계 반영.\n\n" +
                    "2. 배합실적등록(약품), 생산실적 및 조회(약품) 부분에 QR라벨 적용.";
            }
            catch (Exception ex)
            {
                
            }
        }
    }
}
