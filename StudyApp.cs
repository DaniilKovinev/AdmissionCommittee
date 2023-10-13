using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace admission_commision
{
    public partial class StudyApp : Form
    {
        public string userLogin = string.Empty;
        public StudyApp()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox2.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox3.DropDownStyle = ComboBoxStyle.DropDownList;
        }
        private void StudyApp_Load(object sender, EventArgs e)
        {
            LoadFaculties();
            comboBox1.SelectedIndexChanged += ComboBox1_SelectedIndexChanged;
            comboBox2.SelectedIndexChanged += ComboBox2_SelectedIndexChanged;
            comboBox3.SelectedIndexChanged += ComboBox3_SelectedIndexChanged;
            LoadDepartmentsData();
        }
        private void LoadFaculties()
        {
            comboBox1.Items.Clear();
            comboBox2.Items.Clear();
            comboBox3.Items.Clear();

            List<string> facult = SQLClass.Select("SELECT name_fac FROM faculties ORDER BY id;");
            comboBox1.Items.AddRange(facult.ToArray());
        }
        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            dataGridView1.DataSource = null;
            comboBox2.Items.Clear();
            comboBox3.Items.Clear();

            string selectedFaculty = comboBox1.SelectedItem.ToString();
            List<string> dep = SQLClass.Select($"SELECT name_dep FROM departments WHERE faculty_id = (SELECT id FROM faculties WHERE name_fac='{selectedFaculty}') ORDER BY id;");
            comboBox2.Items.AddRange(dep.ToArray());
        }
        private void ComboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            dataGridView1.DataSource = null;
            comboBox3.Items.Clear();

            string selectedDepartment = comboBox2.SelectedItem.ToString();
            List<string> spec = SQLClass.Select($"SELECT name_sp FROM specialties WHERE department_id = (SELECT id FROM departments WHERE name_dep='{selectedDepartment}') ORDER BY id;");
            comboBox3.Items.AddRange(spec.ToArray());
        }
        private void ComboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            dataGridView1.DataSource = null;
            LoadDepartmentsData();
        }
        private void LoadDepartmentsData()
        {
            string selectedDepartment = comboBox2.SelectedItem?.ToString();
            string selectedSpecialty = comboBox3.SelectedItem?.ToString();

            if (selectedDepartment == null || selectedSpecialty == null)
            {
                return;
            }

            string query = $@"SELECT code_specialization.id, name_dep, name_sp, level, form, pay, budget_places, code_specialization.specialization_id, code_specialization.department_id, studyForm_id, studyLevel_id, formPay_id
                              FROM code_specialization
                              INNER JOIN departments ON code_specialization.department_id = departments.id
                              INNER JOIN specialties ON code_specialization.specialization_id = specialties.id
                              INNER JOIN studyForm ON code_specialization.studyForm_id = studyForm.id
                              INNER JOIN studyLevel ON code_specialization.studyLevel_id = studyLevel.id
                              INNER JOIN formPay ON code_specialization.formPay_id = formPay.id
                              WHERE name_dep = '{selectedDepartment}'
                              AND name_sp = '{selectedSpecialty}';";

            MySqlCommand cmd = new MySqlCommand(query, SQLClass.conn);
            MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);

            dataGridView1.DataSource = dt;
            dataGridView1.Columns["name_sp"].HeaderText = "Специальность";
            dataGridView1.Columns["level"].HeaderText = "Форма подготовки";
            dataGridView1.Columns["form"].HeaderText = "Форма обучения";
            dataGridView1.Columns["pay"].HeaderText = "Оплата обучения";
            dataGridView1.Columns["budget_places"].HeaderText = "Бюджетные места";
            dataGridView1.Columns["specialization_id"].Visible = false;
            dataGridView1.Columns["department_id"].Visible = false;
            dataGridView1.Columns["studyForm_id"].Visible = false;
            dataGridView1.Columns["studyLevel_id"].Visible = false;
            dataGridView1.Columns["formPay_id"].Visible = false;
            dataGridView1.Columns["name_dep"].Visible = false;
            dataGridView1.Columns["id"].Visible = false;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }
        private void ComboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {


            string selectedDepartment = comboBox2.SelectedItem.ToString();
            string selectedSpecialty = comboBox3.SelectedItem.ToString();
            List<string> pay = SQLClass.Select
            ($@"SELECT pay
                FROM code_specialization
                INNER JOIN departments ON code_specialization.department_id = departments.id
                INNER JOIN specialties ON code_specialization.specialization_id = specialties.id
                INNER JOIN studyForm ON code_specialization.studyForm_id = studyForm.id
                INNER JOIN studyLevel ON code_specialization.studyLevel_id = studyLevel.id
                INNER JOIN formPay ON code_specialization.formPay_id = formPay.id
                WHERE name_dep = '{selectedDepartment}'
                AND name_sp = '{selectedSpecialty}'");

        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem == null || comboBox2.SelectedItem == null || comboBox3.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, заполните все поля перед подачей заявления.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (dataGridView1.CurrentRow == null)
            {
                MessageBox.Show("Ошибка, не выбрана строка с направлением обучения!");
                return;
            }

            List<string> id = SQLClass.Select("SELECT id FROM users WHERE login = '" + userLogin + "';");

            string selectedDepartment = comboBox2.SelectedItem.ToString();
            string selectedSpecialty = comboBox3.SelectedItem.ToString();
            int level = Convert.ToInt32(dataGridView1.CurrentRow.Cells["studyLevel_id"].Value);
            int form = Convert.ToInt32(dataGridView1.CurrentRow.Cells["studyForm_id"].Value);
            int pay = Convert.ToInt32(dataGridView1.CurrentRow.Cells["formPay_id"].Value);

            List<string> scodeId = SQLClass.Select
            ($@"SELECT code_specialization.id
                FROM code_specialization
                INNER JOIN departments ON code_specialization.department_id = departments.id
                INNER JOIN specialties ON code_specialization.specialization_id = specialties.id
                WHERE name_dep = '{selectedDepartment}'
                AND studyLevel_id = '{level}'
                AND studyForm_id = '{form}'
                AND name_sp = '{selectedSpecialty}'
                AND formPay_id = '{pay}'");


            var recordingDate = DateTime.Today.ToString("yyyy-MM-dd");
            var applicantId = id[0];
            var codeId = scodeId[0];

            List<string> applicantEntry = SQLClass.Select("SELECT * FROM applicants WHERE user_id = '" + applicantId + "';");
            if (applicantEntry.Count == 0)
            {
                MessageBox.Show("Сначала внесите данные о себе!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string countQuery = $@"SELECT COUNT(*) FROM study_application WHERE applicant_id = '{applicantId}';";
            List<string> countResult = SQLClass.Select(countQuery);

            if (int.Parse(countResult[0]) >= 3)
            {
                MessageBox.Show("Вы не можете подать больше трех заявлений.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            List<string> existingApplication = SQLClass.Select(
                 "SELECT id FROM study_application " +
                 $"WHERE applicant_id = '{applicantId}' AND code_specialization_id = '{codeId}';");

            if (existingApplication.Count > 0)
            {
                MessageBox.Show("Вы уже подали заявление на эту специальность.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string query = $"INSERT INTO study_application(date_issued, applicant_id, code_specialization_id) VALUES( '{recordingDate}', '{applicantId}', '{codeId}');";

            MySqlCommand cmd = new MySqlCommand(query, SQLClass.conn);

            if (cmd.ExecuteNonQuery() == 1)
            {
                MessageBox.Show("Данные внесены!", "Успех");

                MySqlDataAdapter adapter = new MySqlDataAdapter();
                DataTable dt = new DataTable();

                LoadDepartmentsData();
            }
            else
            {
                MessageBox.Show("Ошибка, данные не внесены!");
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            MySqlDataAdapter adapter = new MySqlDataAdapter();
            DataTable dt = new DataTable();

            string query5 = $"SELECT id, login, password, is_admin FROM users WHERE login = '{userLogin}'";

            MySqlCommand cmd2 = new MySqlCommand(query5, SQLClass.conn);

            adapter.SelectCommand = cmd2;
            adapter.Fill(dt);

            var user = new CheckAdmin(dt.Rows[0].ItemArray[1].ToString(), Convert.ToBoolean(dt.Rows[0].ItemArray[3]));

            EnterPro rf = new EnterPro(user);
            rf.userLogin = userLogin;
            this.Close();
            rf.Show();
            rf.Activate();
        }
    }
}
