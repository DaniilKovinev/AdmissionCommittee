using MySql.Data.MySqlClient;
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
    public partial class EnterPro : Form
    {
        public string userLogin = string.Empty;
        private readonly CheckAdmin _user;
        public EnterPro(CheckAdmin user)

        {
            _user = user;
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
            IsAdmin();
        }
        private void EnterPro_Load(object sender, EventArgs e)
        {
            LoadStudentApplicationsData();
        }
        private void LoadStudentApplicationsData()
        {
            string idQuery = $"SELECT id FROM users WHERE login = '{userLogin}'";
            MySqlCommand idCmd = new MySqlCommand(idQuery, SQLClass.conn);
            MySqlDataAdapter idAdapter = new MySqlDataAdapter(idCmd);
            DataTable idDt = new DataTable();
            idAdapter.Fill(idDt);

            if (idDt.Rows.Count > 0)
            {
                int userId = Convert.ToInt32(idDt.Rows[0]["id"]);

                string query = $@"SELECT name_fac, name_dep, name_sp, level, form, pay, date_issued, study_application.id, sum_results, decision, date_decision, applicant_id
                                  FROM study_application
                                  INNER JOIN applicants ON study_application.applicant_id = applicants.user_id
                                  INNER JOIN code_specialization ON study_application.code_specialization_id = code_specialization.id
                                  INNER JOIN specialties ON code_specialization.specialization_id = specialties.id
                                  INNER JOIN departments ON specialties.department_id = departments.id
                                  INNER JOIN faculties ON departments.faculty_id = faculties.id
                                  INNER JOIN studyForm ON code_specialization.studyForm_id = studyForm.id
                                  INNER JOIN studyLevel ON code_specialization.studyLevel_id = studyLevel.id
                                  INNER JOIN formPay ON code_specialization.formPay_id = formPay.id
                                  WHERE applicant_id = {userId};";

                MySqlCommand cmd = new MySqlCommand(query, SQLClass.conn);
                MySqlDataAdapter adapter2 = new MySqlDataAdapter(cmd);
                DataTable dt2 = new DataTable();
                adapter2.Fill(dt2);

                dataGridView1.DataSource = dt2;
                dataGridView1.Columns["name_fac"].HeaderText = "Факультет";
                dataGridView1.Columns["name_dep"].HeaderText = "Кафедра";
                dataGridView1.Columns["name_sp"].HeaderText = "Специальность";
                dataGridView1.Columns["level"].HeaderText = "Форма подготовки";
                dataGridView1.Columns["form"].HeaderText = "Форма обучения";
                dataGridView1.Columns["pay"].HeaderText = "Оплата обучения";
                dataGridView1.Columns["sum_results"].HeaderText = "Cумма баллов";
                dataGridView1.Columns["decision"].HeaderText = "Решение";
                dataGridView1.Columns["date_decision"].HeaderText = "Дата решения";
                dataGridView1.Columns["date_issued"].HeaderText = "Дата заявления";
                dataGridView1.Columns["id"].Visible = false;
                dataGridView1.Columns["applicant_id"].Visible = false;
                dataGridView1.AllowUserToAddRows = false;
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                if (dataGridView1.Rows.Count > 0)
                {
                    dataGridView1.ClearSelection();
                    dataGridView1.CurrentCell = null;
                }
            }
            else
            {
                MessageBox.Show("Пользователь не найден");
            }
        }
        private void IsAdmin()
        {
            button1.Enabled = !(_user.IsAdmin);
            button2.Enabled = !(_user.IsAdmin);
            button3.Enabled = !(_user.IsAdmin);
            button4.Enabled = !(_user.IsAdmin);
            button5.Enabled = !(_user.IsAdmin);
            button6.Enabled = !(_user.IsAdmin);
            button7.Enabled = !(_user.IsAdmin);
            button13.Enabled = !(_user.IsAdmin);
            button14.Enabled = !(_user.IsAdmin);

            button9.Enabled = _user.IsAdmin;
            button10.Enabled = _user.IsAdmin;
            button11.Enabled = _user.IsAdmin;
            button15.Enabled = _user.IsAdmin;
            button16.Enabled = _user.IsAdmin;
            button17.Enabled = _user.IsAdmin;
            button18.Enabled = _user.IsAdmin;
            button19.Enabled = _user.IsAdmin;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            AddPro rf = new AddPro();
            rf.userLogin = _user.Login;
            this.Close();
            rf.Show();
            rf.Activate();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            EditPro rf = new EditPro();
            rf.userLogin = _user.Login;
            this.Close();
            rf.Show();
            rf.Activate();
        }
        private void button3_Click(object sender, EventArgs e)
        {
            AddEge rf = new AddEge();
            rf.userLogin = _user.Login;
            this.Close();
            rf.Show();
            rf.Activate();
        }
        private void button4_Click(object sender, EventArgs e)
        {
            AddPrivilege rf = new AddPrivilege();
            rf.userLogin = _user.Login;
            this.Close();
            rf.Show();
            rf.Activate();
        }
        private void button5_Click(object sender, EventArgs e)
        {
            AddDoc rf = new AddDoc();
            rf.userLogin = _user.Login;
            this.Close();
            rf.Show();
            rf.Activate();
        }
        private void button6_Click(object sender, EventArgs e)
        {
            EditDoc rf = new EditDoc();
            rf.userLogin = _user.Login;
            this.Close();
            rf.Show();
            rf.Activate();
        }
        private void button14_Click(object sender, EventArgs e)
        {
            EditPrivilege rf = new EditPrivilege();
            rf.userLogin = _user.Login;
            this.Close();
            rf.Show();
            rf.Activate();
        }
        private void button13_Click(object sender, EventArgs e)
        {
            EditEge rf = new EditEge();
            rf.userLogin = _user.Login;
            this.Close();
            rf.Show();
            rf.Activate();
        }
        private void button7_Click(object sender, EventArgs e)
        {
            StudyApp rf = new StudyApp();
            rf.userLogin = _user.Login;
            this.Close();
            rf.Show();
            rf.Activate();
        }
        private void button15_Click(object sender, EventArgs e)
        {
            EditExamsAdmin rf = new EditExamsAdmin();
            rf.userLogin = _user.Login;
            this.Close();
            rf.Show();
            rf.Activate();
        }
        private void button16_Click(object sender, EventArgs e)
        {
            EditPrivilegeAdmin rf = new EditPrivilegeAdmin();
            rf.userLogin = _user.Login;
            this.Close();
            rf.Show();
            rf.Activate();
        }
        private void button11_Click(object sender, EventArgs e)
        {
            EditUsersAdmin rf = new EditUsersAdmin();
            rf.userLogin = _user.Login;
            this.Close();
            rf.Show();
            rf.Activate();
        }
        private void button17_Click(object sender, EventArgs e)
        {
            EditResultExamsAdmin rf = new EditResultExamsAdmin();
            rf.userLogin = _user.Login;
            this.Close();
            rf.Show();
            rf.Activate();
        }
        private void button9_Click(object sender, EventArgs e)
        {
            EditAppAdmin rf = new EditAppAdmin();
            rf.userLogin = _user.Login;
            this.Close();
            rf.Show();
            rf.Activate();
        }
        private void button18_Click(object sender, EventArgs e)
        {
            EditSpecAdmin rf = new EditSpecAdmin();
            rf.userLogin = _user.Login;
            this.Close();
            rf.Show();
            rf.Activate();
        }
        private void button19_Click(object sender, EventArgs e)
        {
            EditDepartmentsAdmin rf = new EditDepartmentsAdmin();
            rf.userLogin = _user.Login;
            this.Close();
            rf.Show();
            rf.Activate();
        }
        private void button10_Click(object sender, EventArgs e)
        {
          
            EditFacultiesAdmin rf = new EditFacultiesAdmin();
            rf.userLogin = _user.Login;
            this.Close();
            rf.Show();
            rf.Activate();
        }
        private void button12_Click(object sender, EventArgs e)
        {
            Entry form = (Entry)Application.OpenForms["Entry"];
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
