using MySql.Data.MySqlClient;
using System.Data;
using System;
using System.Windows.Forms;

namespace admission_commision
{
    public partial class EducPrograms : Form
    {
        public string userLogin = string.Empty;
        private readonly CheckAdmin _user;
        public EducPrograms(CheckAdmin user)
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
            _user = user;
        }

        private void EducPrograms_Load(object sender, System.EventArgs e)
        {
            LoadStudentApplicationsData();
        }
        private void LoadStudentApplicationsData()
        {
            try
            {
                string query = $@"SELECT code_specialization.id, name_fac, name_dep, name_sp, level, form, pay, budget_places 
                          FROM code_specialization
                          INNER JOIN specialties ON code_specialization.specialization_id = specialties.id
                          INNER JOIN departments ON specialties.department_id = departments.id
                          INNER JOIN faculties ON departments.faculty_id = faculties.id
                          INNER JOIN studyForm ON code_specialization.studyForm_id = studyForm.id
                          INNER JOIN studyLevel ON code_specialization.studyLevel_id = studyLevel.id
                          INNER JOIN formPay ON code_specialization.formPay_id = formPay.id;";

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
                dataGridView1.Columns["budget_places"].HeaderText = "Бюджетные места";
                dataGridView1.Columns["id"].Visible = false;
                dataGridView1.AllowUserToAddRows = false;
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                if (dataGridView1.Rows.Count > 0)
                {
                    dataGridView1.ClearSelection();
                    dataGridView1.CurrentCell = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AdmissionCom rf = new AdmissionCom(_user);
            rf.userLogin =_user.Login;
            this.Close();
            rf.Show();
            rf.Activate();
        }
    }
}
