using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace admission_commision
{
    public partial class AboutUs : Form
    {
        public string userLogin = string.Empty;
        private readonly CheckAdmin _user;
        public AboutUs(CheckAdmin user)
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
            _user = user;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            AdmissionCom rf = new AdmissionCom(_user);
            rf.userLogin = _user.Login;
            this.Close();
            rf.Show();
            rf.Activate();
        }
    }
}
