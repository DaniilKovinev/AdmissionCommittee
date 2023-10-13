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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace admission_commision
{
    public partial class EditAppAdmin : Form
    {
        public string userLogin = string.Empty;
        public EditAppAdmin()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox2.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox3.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox5.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox6.DropDownStyle = ComboBoxStyle.DropDownList;
            dataGridView2.SelectionChanged += DataGridView2_SelectionChanged;
            textBox1.ReadOnly = true;
        }
        private void EditApp_Load(object sender, EventArgs e)
        {
            LoadComboBoxData();
            comboBox1.SelectedIndexChanged += ComboBox1_SelectedIndexChanged;
            comboBox2.SelectedIndexChanged += ComboBox2_SelectedIndexChanged;
            comboBox3.SelectedIndexChanged += ComboBox3_SelectedIndexChanged;
            dataGridView1.CellClick += dataGridView1_CellClick;
            dataGridView3.CellClick += dataGridView3_CellClick;
            button4.Click += button4_Click;
            LoadDepartmentsData();
        }
        private void LoadComboBoxData()
        {
            comboBox1.Items.Clear();
            List<string> faculties = SQLClass.Select("SELECT name_fac FROM faculties ORDER BY id;");
            comboBox1.Items.AddRange(faculties.ToArray());

        }
        private void LoadDepartmentsData()
        {
            string selectedDepartment = comboBox2.SelectedItem?.ToString();
            string selectedSpecialty = comboBox3.SelectedItem?.ToString();

            if (selectedDepartment == null || selectedSpecialty == null)
            {
                return;
            }

            string query = $@"SELECT code_specialization.id, name_dep, name_sp, level, form, pay, budget_places, code_specialization.specialization_id, code_specialization.department_id
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
            dataGridView1.Columns["name_dep"].Visible = false;
            dataGridView1.Columns["id"].Visible = false;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }
        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox2.Items.Clear();
            comboBox3.Items.Clear();
            dataGridView1.DataSource = null;
            dataGridView2.DataSource = null;
            dataGridView3.DataSource = null;
            textBox1.Clear();

            string selectedFaculty = comboBox1.SelectedItem?.ToString();

            if (selectedFaculty != null)
            {
                List<string> departments = SQLClass.Select($"SELECT name_dep FROM departments WHERE faculty_id = (SELECT id FROM faculties WHERE name_fac = '{selectedFaculty}') ORDER BY id;");
                comboBox2.Items.AddRange(departments.ToArray());
            }
        }
        private void ComboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            dataGridView1.DataSource = null;
            dataGridView2.DataSource = null;
            dataGridView3.DataSource = null;
            comboBox3.Items.Clear();
            textBox1.Clear();

            string selectedDepartment = comboBox2.SelectedItem?.ToString();

            if (selectedDepartment != null)
            {
                List<string> specialties = SQLClass.Select($"SELECT name_sp FROM specialties WHERE department_id = (SELECT id FROM departments WHERE name_dep = '{selectedDepartment}') ORDER BY id;");
                comboBox3.Items.AddRange(specialties.ToArray());
            }
        }
        private void ComboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            dataGridView2.DataSource = null;
            dataGridView3.DataSource = null;
            textBox1.Clear();
            LoadDepartmentsData();
        }
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            LoadStudentApplicationsData();
        }
        private void LoadStudentApplicationsData()
        {
            if (dataGridView1.CurrentRow == null) return;

            int code_specialization_id = Convert.ToInt32(dataGridView1.CurrentRow.Cells["id"].Value);

            string query = $@"SELECT surname, name, patronymic, study_application.id, sum_results, decision, date_decision, date_issued, applicant_id
                              FROM study_application
                              INNER JOIN applicants ON study_application.applicant_id = applicants.user_id
                              WHERE code_specialization_id = {code_specialization_id};";

            MySqlCommand cmd = new MySqlCommand(query, SQLClass.conn);
            MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);

            dataGridView2.DataSource = dt;
            dataGridView2.Columns["surname"].HeaderText = "Фамилия";
            dataGridView2.Columns["name"].HeaderText = "Имя";
            dataGridView2.Columns["patronymic"].HeaderText = "Отчество";
            dataGridView2.Columns["sum_results"].HeaderText = "Cумма баллов";
            dataGridView2.Columns["decision"].HeaderText = "Решение";
            dataGridView2.Columns["date_decision"].HeaderText = "Дата решения";
            dataGridView2.Columns["date_issued"].HeaderText = "Дата заявления";
            dataGridView2.Columns["id"].Visible = false;
            dataGridView2.Columns["applicant_id"].Visible = false;
            dataGridView2.AllowUserToAddRows = false;
            dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }
        private string currentTable;
        private void LoadPrivilegeData()
        {
            if (dataGridView2.CurrentRow == null) return;

            int applicant_id = Convert.ToInt32(dataGridView2.CurrentRow.Cells["applicant_id"].Value);
            string query = $@"SELECT privileged.id, privilege.benefits, privilege.scores, privileged.sel
                              FROM privilege
                              INNER JOIN privileged ON privileged.privilege_id = privilege.id
                              WHERE privileged.applicant_id = {applicant_id};";

            MySqlCommand cmd = new MySqlCommand(query, SQLClass.conn);
            MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
            DataTable dt2 = new DataTable();
            adapter.Fill(dt2);

            dataGridView3.DataSource = dt2;

            dataGridView3.Columns["benefits"].HeaderText = "Привилегии";
            dataGridView3.Columns["scores"].HeaderText = "Баллы";
            dataGridView3.Columns["sel"].HeaderText = "Подтверждение";
            dataGridView3.Columns["id"].Visible = false;
            dataGridView3.AllowUserToAddRows = false;
            dataGridView3.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            currentTable = "Privilege";
        }
        private void LoadEgeData()
        {
            if (dataGridView2.CurrentRow == null) return;

            int applicant_id = Convert.ToInt32(dataGridView2.CurrentRow.Cells["applicant_id"].Value);
            string query = $@"SELECT ege_results.id, name, ege_results.result, ege_results.sel
                              FROM exams
                              INNER JOIN ege_results ON ege_results.exam_id = exams.id
                              WHERE ege_results.applicant_id = {applicant_id};";

            MySqlCommand cmd = new MySqlCommand(query, SQLClass.conn);
            MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
            DataTable dt2 = new DataTable();
            adapter.Fill(dt2);

            dataGridView3.DataSource = dt2;

            dataGridView3.Columns["name"].HeaderText = "Название";
            dataGridView3.Columns["result"].HeaderText = "Баллы";
            dataGridView3.Columns["sel"].HeaderText = "Подтверждение";
            dataGridView3.Columns["id"].Visible = false;
            dataGridView3.AllowUserToAddRows = false;
            dataGridView3.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            currentTable = "Ege";
        }
        private void LoadExamData()
        {
            if (dataGridView2.CurrentRow == null) return;

            int applicant_id = Convert.ToInt32(dataGridView2.CurrentRow.Cells["applicant_id"].Value);
            string query = $@"SELECT exam_results.id, name, exam_results.result, exam_results.sel
                              FROM exams
                              INNER JOIN exam_results ON exam_results.exam_id = exams.id
                              WHERE exam_results.applicant_id = {applicant_id};";

            MySqlCommand cmd = new MySqlCommand(query, SQLClass.conn);
            MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
            DataTable dt2 = new DataTable();
            adapter.Fill(dt2);

            dataGridView3.DataSource = dt2;

            dataGridView3.Columns["name"].HeaderText = "Название";
            dataGridView3.Columns["result"].HeaderText = "Баллы";
            dataGridView3.Columns["sel"].HeaderText = "Подтверждение";
            dataGridView3.Columns["id"].Visible = false;
            dataGridView3.AllowUserToAddRows = false;
            dataGridView3.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            currentTable = "Exam";
        }
        private void LoadDocData()
        {
            if (dataGridView2.CurrentRow == null) return;

            int applicant_id = Convert.ToInt32(dataGridView2.CurrentRow.Cells["applicant_id"].Value);
            string query = $@"SELECT type, number, issued_by, date_issued
                              FROM documents
                              WHERE applicant_id = {applicant_id};";

            MySqlCommand cmd = new MySqlCommand(query, SQLClass.conn);
            MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
            DataTable dt2 = new DataTable();
            adapter.Fill(dt2);

            dataGridView3.DataSource = dt2;

            dataGridView3.Columns["type"].HeaderText = "Тип";
            dataGridView3.Columns["number"].HeaderText = "Номер";
            dataGridView3.Columns["issued_by"].HeaderText = "Кем выдано";
            dataGridView3.Columns["date_issued"].HeaderText = "Когда выдано";
            dataGridView3.AllowUserToAddRows = false;
            dataGridView3.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            currentTable = "Doc";
        }
        private void LoadApplicantData()
        {
            if (dataGridView2.CurrentRow == null) return;

            int applicant_id = Convert.ToInt32(dataGridView2.CurrentRow.Cells["applicant_id"].Value);
            string query = $@"SELECT citizenship, birthdate, address, gender, email
                              FROM applicants
                              WHERE user_id = {applicant_id};";

            MySqlCommand cmd = new MySqlCommand(query, SQLClass.conn);
            MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
            DataTable dt2 = new DataTable();
            adapter.Fill(dt2);

            dataGridView3.DataSource = dt2;
            dataGridView3.Columns["citizenship"].HeaderText = "Страна";
            dataGridView3.Columns["birthdate"].HeaderText = "Дата рождения";
            dataGridView3.Columns["address"].HeaderText = "Адрес";
            dataGridView3.Columns["gender"].HeaderText = "Пол";
            dataGridView3.Columns["email"].HeaderText = "Email";
            dataGridView3.AllowUserToAddRows = false;
            dataGridView3.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            currentTable = "Applicant";
        }
        private void dataGridView3_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView3.CurrentRow != null)
            {
                if (currentTable == "Ege" || currentTable == "Exam" || currentTable == "Privilege")
                {
                    string selectValue = dataGridView3.CurrentRow.Cells["sel"].Value.ToString();
                    comboBox6.SelectedItem = selectValue == "True" ? "Зачислить баллы" : "Не зачислять баллы";
                }
            }
        }
        private void button4_Click(object sender, EventArgs e)
        {
                LoadPrivilegeData();
        }
        private void button5_Click(object sender, EventArgs e)
        {
            LoadEgeData();
        }
        private void button6_Click(object sender, EventArgs e)
        {
            LoadExamData();
        }
        private void button3_Click(object sender, EventArgs e)
        {
            LoadDocData();
        }
        private void button8_Click_1(object sender, EventArgs e)
        {
            LoadApplicantData();
        }
        private void DataGridView2_SelectionChanged(object sender, EventArgs e)
        {
            dataGridView3.DataSource = null;
            textBox1.Text = "";
            comboBox5.SelectedIndex = -1;
            comboBox6.SelectedIndex = -1;
        }
        private void button7_Click_1(object sender, EventArgs e)
        {
            if (currentTable == "Doc" || currentTable == "Applicant" || currentTable == "App")
            {
                MessageBox.Show("Ошибка, невозможно изменить данные для текущей таблицы!");
                return;
            }

            if (dataGridView3.SelectedRows.Count > 0)
            {
                int selectedId = Convert.ToInt32(dataGridView3.SelectedRows[0].Cells["id"].Value);
                string selectedAdminText = comboBox6.SelectedItem.ToString();
                int select = (selectedAdminText.ToLower() == "зачислить баллы") ? 1 : 0;

                string queryToUpdate;
                string curSelText = dataGridView3.SelectedRows[0].Cells["sel"].Value.ToString();
                int curSel = (curSelText.ToLower() == "true") ? 1 : 0;

                int applicant_id = Convert.ToInt32(dataGridView2.CurrentRow.Cells["applicant_id"].Value);
                int selectedSubjectsCount = GetSelectedSubjectsCount(applicant_id);

                if (currentTable != "Privilege" && select == 1 && curSel == 0 && selectedSubjectsCount > 2)
                {
                    MessageBox.Show("Ошибка, нельзя выбрать больше 3 предметов!");
                    return;
                }

                if (currentTable == "Ege")
                {
                    queryToUpdate = $"UPDATE ege_results SET sel = '{select}' WHERE id = {selectedId}";
                }
                else if (currentTable == "Privilege")
                {
                    queryToUpdate = $"UPDATE privileged SET sel = '{select}' WHERE id = {selectedId}";
                }
                else if (currentTable == "Exam")
                {
                    queryToUpdate = $"UPDATE exam_results SET sel = '{select}' WHERE id = {selectedId}";
                }
                else
                {
                    MessageBox.Show("Ошибка, таблица не определена!");
                    return;
                }

                MySqlCommand cmd = new MySqlCommand(queryToUpdate, SQLClass.conn);

                if (cmd.ExecuteNonQuery() == 1)
                {
                    if (currentTable == "Ege")
                    {
                        LoadEgeData();
                    }
                    else if (currentTable == "Privilege")
                    {
                        LoadPrivilegeData();
                    }
                    else if (currentTable == "Exam")
                    {
                        LoadExamData();
                    }
                }
                else
                {
                    MessageBox.Show("Ошибка, данные не внесены!");
                }
            }
            else
            {
                MessageBox.Show("Ошибка, не выбрана строка!");
            }
        }
        private void CalculateAndDisplaySum()
        {
            if (dataGridView2.CurrentRow == null)
            {
                MessageBox.Show("Ошибка, не выбрана строка!");
                return;
            }

            int applicant_id = Convert.ToInt32(dataGridView2.CurrentRow.Cells["applicant_id"].Value);

            using (MySqlConnection conn = new MySqlConnection(SQLClass.conn.ConnectionString))
            {
                conn.Open();

                string query = @"
            SELECT SUM(total_result) AS total_result
            FROM (
                SELECT SUM(er.result) AS total_result
                FROM exam_results er
                WHERE er.applicant_id = @applicantId
                AND er.sel = 1
                UNION ALL
                SELECT SUM(ege.result) AS total_result
                FROM ege_results ege
                WHERE ege.applicant_id = @applicantId
                AND ege.sel = 1
                UNION ALL
                SELECT SUM(p.scores) AS total_result
                FROM privilege p
                INNER JOIN privileged pr ON pr.privilege_id = p.id
                WHERE pr.applicant_id = @applicantId
                AND pr.sel = 1
            ) AS subquery;";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@applicantId", applicant_id);

                try
                {
                    object result = cmd.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        textBox1.Text = result.ToString();
                    }
                    else
                    {
                        textBox1.Text = "0";
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при выполнении запроса: {ex.Message}");
                }
            } 
        }
        private int GetSelectedSubjectsCount(int applicantId)
        {
            int count = 0;

            using (MySqlConnection conn = new MySqlConnection(SQLClass.conn.ConnectionString))
            {
                conn.Open();

                string query = @"
            SELECT COUNT(*) as count FROM (
                SELECT id FROM exam_results WHERE sel = 1 AND applicant_id = @applicantId
                UNION ALL
                SELECT id FROM ege_results WHERE sel = 1 AND applicant_id = @applicantId
            ) AS results";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@applicantId", applicantId);

                count = Convert.ToInt32(cmd.ExecuteScalar());
            }

            return count;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            CalculateAndDisplaySum();
        }
        private void button1_Click(object sender, EventArgs e)
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
        private void button9_Click(object sender, EventArgs e)
        {
            if (dataGridView2.CurrentRow == null)
            {
                MessageBox.Show("Ошибка, не выбрана строка с абитуриентом!");
                return;
            }

            if (comboBox5.SelectedItem == null || string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("Пожалуйста, заполните сначала поля с баллами и решением.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int id = Convert.ToInt32(dataGridView2.CurrentRow.Cells["id"].Value);
            string decision = comboBox5.SelectedItem.ToString();
            var recordingDate = DateTime.Today.ToString("yyyy-MM-dd");

            int scores;
            int.TryParse(textBox1.Text, out scores);

            string query = $"UPDATE study_application " +
                           $"SET decision = '{decision}',sum_results = '{scores}',date_decision = '{recordingDate}' " +
                           $"WHERE id = {id}";
            MySqlCommand cmd = new MySqlCommand(query, SQLClass.conn);

            if (cmd.ExecuteNonQuery() == 1)
            {
                LoadStudentApplicationsData();
            }
            else
            {
                MessageBox.Show("Ошибка, данные не внесены!");
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            if (dataGridView3.CurrentRow == null)
            {
                MessageBox.Show("Ошибка, нажмите на кнопку 'все заявления абитуриента' и выберите запись!");
                return;
            }

            int id = Convert.ToInt32(dataGridView3.CurrentRow.Cells["id"].Value);

            string query = $"DELETE FROM study_application WHERE id = {id}";

            MySqlCommand cmd = new MySqlCommand(query, SQLClass.conn);

            if (cmd.ExecuteNonQuery() == 1)
            {
                LoadStudentAllAppData();
            }
            else
            {
                MessageBox.Show("Ошибка, данные не внесены!");
            }
        }
        private void LoadStudentAllAppData()
        {
            int id = Convert.ToInt32(dataGridView2.CurrentRow.Cells["applicant_id"].Value);

            if (id > 0)
            {

                string query = $@"SELECT name_fac, name_dep, name_sp, form, study_application.id, sum_results, decision, date_decision, date_issued, applicant_id
                                  FROM study_application
                                  INNER JOIN applicants ON study_application.applicant_id = applicants.user_id
                                  INNER JOIN code_specialization ON study_application.code_specialization_id = code_specialization.id
                                  INNER JOIN specialties ON code_specialization.specialization_id = specialties.id
                                  INNER JOIN departments ON specialties.department_id = departments.id
                                  INNER JOIN faculties ON departments.faculty_id = faculties.id
                                  INNER JOIN studyForm ON code_specialization.studyForm_id = studyForm.id
                                  WHERE applicant_id = {id};";

                MySqlCommand cmd = new MySqlCommand(query, SQLClass.conn);
                MySqlDataAdapter adapter2 = new MySqlDataAdapter(cmd);
                DataTable dt2 = new DataTable();
                adapter2.Fill(dt2);

                dataGridView3.DataSource = dt2;
                dataGridView3.Columns["name_fac"].HeaderText = "Факультет";
                dataGridView3.Columns["name_dep"].HeaderText = "Кафедра";
                dataGridView3.Columns["name_sp"].HeaderText = "Специальность";
                dataGridView3.Columns["form"].HeaderText = "Форма обучения";
                dataGridView3.Columns["sum_results"].HeaderText = "Cумма баллов";
                dataGridView3.Columns["decision"].HeaderText = "Решение";
                dataGridView3.Columns["date_decision"].HeaderText = "Дата решения";
                dataGridView3.Columns["date_issued"].HeaderText = "Дата заявления";
                dataGridView3.Columns["id"].Visible = false;
                dataGridView3.Columns["applicant_id"].Visible = false;
                dataGridView3.AllowUserToAddRows = false;
                dataGridView3.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                currentTable = "App";
            }
            else
            {
                MessageBox.Show("Пользователь не найден");
            }
            if (dataGridView3.Rows.Count > 0)
            {
                dataGridView3.ClearSelection();
                dataGridView3.CurrentCell = null;
            }
        }
        private void button11_Click(object sender, EventArgs e)
        {
            if (dataGridView2.CurrentRow == null)
            {
                MessageBox.Show("Выберите запись с абитуриентом.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            LoadStudentAllAppData();
        }
    }
}
