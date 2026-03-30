using System;
using System.Drawing;
using System.IO;
using System.Media;
using System.Windows.Forms;

namespace area_L
{
    public partial class AlertForm : Form
    {
        string _soundPath;
        bool _playedOnce;
        SoundPlayer _player;

        public AlertForm()
        {
            InitializeComponent();
        }
        // 값을 세팅하고 폼을 표시. 사운드 경로는 저장만 하고, 실제 재생은 Shown에서 1회 수행
        public void ShowAlert(string orderNo, long requiredQty, long receivedQty, long shortageQty, string soundPath = null)
        {
            lblOrderNo.Text = orderNo;
            lblRequired.Text = requiredQty.ToString("#,0");
            lblReceived.Text = receivedQty.ToString("#,0");
            lblShortage.Text = shortageQty.ToString("#,0");

            _soundPath = soundPath;
            _playedOnce = false; // 새 값으로 열릴 때만 1회 재생

            Show();
            Activate();
        }

        // 폼이 실제로 화면에 나타나는 시점에 한 번만 재생
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            if (!_playedOnce && !string.IsNullOrWhiteSpace(_soundPath) && File.Exists(_soundPath))
            {
                try
                {
                    _player = new SoundPlayer(_soundPath);
                    _player.Play(); // 비동기 1회 재생
                }
                catch { /* 재생 실패는 무시 */ }
                _playedOnce = true;
            }
        }

        // 폼 종료 시 정리
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            try
            {
                _player?.Stop();
                _player?.Dispose();
                _player = null;
            }
            catch { }
            base.OnFormClosed(e);
        }

        private void btn_close_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
