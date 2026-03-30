using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace area_L
{
    public class ScanItem : INotifyPropertyChanged
    {      
        private string _barcode;
        private string _gtin;
        private string _gdcd;
        private string _qyt;
        private string _mate;
        private string _exprir;
        private string _lotno;
        private string _gdnm;

        public string Barcode
        {
            get => _barcode;
            set { if (_barcode == value) return; _barcode = value; OnPropertyChanged(nameof(Barcode)); }
        }
        public string Gtin
        {
            get => _gtin;
            set { if (_gtin == value) return; _gtin = value; OnPropertyChanged(nameof(Gtin)); }
        }
        public string Gdcd
        {
            get => _gdcd;
            set { if (_gdcd == value) return; _gdcd = value; OnPropertyChanged(nameof(Gdcd)); }
        }

        public string Qyt
        {
            get => _qyt;
            set { if (_qyt == value) return; _qyt = value; OnPropertyChanged(nameof(Qyt)); }
        }
        public string Mate
        {
            get => _mate;
            set { if (_mate == value.ToUpper()) return; _mate = value.ToUpper(); OnPropertyChanged(nameof(Mate)); }
        }
        public string Exprir
        {
            get => _exprir;
            set
            {
                if (_exprir == value) return;

                if (value == string.Empty)
                {
                    _exprir = value;
                    return;
                }
                DateTime dt = DateTime.Parse(value);

                // 원하는 포맷으로 변환
                string onlyDate = dt.ToString("yyyy-MM-dd");
                _exprir = onlyDate;

                OnPropertyChanged(nameof(Exprir));
            }
        }
        public string Lotno
        {
            get => _lotno;
            set { if (_lotno == value) return; _lotno = value; OnPropertyChanged(nameof(Lotno)); }
        }
        public string Gdnm
        {
            get => _gdnm;
            set { if (_gdnm == value) return; _gdnm = value; }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string prop) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}
