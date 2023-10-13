using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Windows.Forms;

namespace admission_commision
{
    public partial class AdmissionCom : Form
    {
        public string userLogin = string.Empty;
        private readonly CheckAdmin _user;
        public AdmissionCom(CheckAdmin user)
        {
            _user = user;
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
        }
        private void button1_Click_1(object sender, EventArgs e)
        {
            EnterPro rf = new EnterPro(_user);
            rf.userLogin = _user.Login;
            this.Close();
            rf.Show();
            rf.Activate();
        }
        private void button2_Click_1(object sender, EventArgs e)
        {
            EducPrograms rf = new EducPrograms(_user);
            rf.userLogin = _user.Login;
            this.Close();
            rf.Show();
            rf.Activate();
        }
        private void button3_Click_1(object sender, EventArgs e)
        {
            AboutUs rf = new AboutUs(_user);
            rf.userLogin = _user.Login;
            this.Close();
            rf.Show();
            rf.Activate();
        }
        private void button4_Click(object sender, EventArgs e)
        {
            Entry form = (Entry)Application.OpenForms["entry"];
            if (form != null)
            {
                form.Show();
                this.Close();
            }
            else
                this.Close();
        }
    }
}

