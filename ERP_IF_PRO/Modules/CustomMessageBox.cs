using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ERP_IF_PRO.Modules
{
    public partial class CustomMessageBox : Form
    {
        public CustomMessageBox(string Message,string Title)
        {
            InitializeComponent();

            lb_Message.Text = Message;

            this.Text = Title;

            this.StartPosition = FormStartPosition.CenterScreen;

            this.button1.Click += (s, e) => { this.Close(); };

            this.button1.KeyDown += (s, e) => { if (e.KeyCode == Keys.Escape) this.Close(); };
        }

        
    }
}
