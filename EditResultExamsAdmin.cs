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
    public partial class EditResultExamsAdmin : Form
    {
        public string userLogin = string.Empty;
        private string user_id = string.Empty;
        public EditResultExamsAdmin()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
            textBox1.KeyPress += ValidateTextBox_KeyPress;
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        private void EditResultExamsAdmin_Load(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();

            List<string> exams = SQLClass.Select("SELECT name FROM exams ORDER BY id;");
            comboBox1.Items.AddRange(exams.ToArray());

            if (comboBox1.Items.Count > 0)
            {
                comboBox1.SelectedIndex = 0;
            }

            LoadApplicantsData();
            dataGridView1.CellClick += dataGridView1_CellClick;
            dataGridView2.SelectionChanged += dataGridView2_SelectionChanged;
        }
        private void dataGridView2_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView2.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dataGridView2.SelectedRows[0];
                string examName = selectedRow.Cells["Exam"].Value.ToString();
                string result = selectedRow.Cells["result"].Value.ToString();

                // Установка значений в TextBox и ComboBox
                textBox1.Text = result;
                comboBox1.SelectedItem = examName;
            }
        }
        private void LoadExamResults(string applicantId)
        {
            string query = @"
            SELECT ER.id, E.name AS Exam, ER.result
            FROM exam_results ER
            JOIN exams E ON ER.exam_id = E.id
            WHERE ER.applicant_id = @applicantId";

            MySqlCommand cmd = new MySqlCommand(query, SQLClass.conn);
            cmd.Parameters.AddWithValue("@applicantId", applicantId);
            MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);

            dataGridView2.DataSource = dt;

            dataGridView2.Columns["id"].Visible = false;
            dataGridView2.Columns["Exam"].HeaderText = "Экзамен";
            dataGridView2.Columns["result"].HeaderText = "Результат";
            dataGridView2.AllowUserToAddRows = false;
            dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }
        private void LoadApplicantsData()
        {
            string query = "SELECT id, surname, name, patronymic, user_id FROM applicants";
            LoadDataToDataGridView(query);
        }
        private void FilterApplicantsData(string filter)
        {
            string query = $"SELECT id, surname, name, patronymic, user_id FROM applicants WHERE surname LIKE '%{filter}%';";
            LoadDataToDataGridView(query);
        }
        private void LoadDataToDataGridView(string query)
        {
            MySqlCommand cmd = new MySqlCommand(query, SQLClass.conn);
            MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);

            dataGridView1.DataSource = dt;

            dataGridView1.Columns["surname"].HeaderText = "Фамилия";
            dataGridView1.Columns["name"].HeaderText = "Имя";
            dataGridView1.Columns["patronymic"].HeaderText = "Отчество";
            dataGridView1.Columns["id"].Visible = false;
            dataGridView1.Columns["user_id"].Visible = false;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow selectedRow = dataGridView1.Rows[e.RowIndex];

                var id = selectedRow.Cells["user_id"].Value.ToString();
                textBox1.Text = string.Empty;
                user_id = id;

                LoadExamResults(user_id);
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text) || string.IsNullOrEmpty(user_id))
            {
                MessageBox.Show("Вы не ввели все необходимые данные!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string result1 = textBox1.Text;
            string exam1Id = SQLClass.Select($"SELECT id FROM exams WHERE name = '{comboBox1.SelectedItem.ToString()}';")[0];
            string checkExam1 = $"SELECT COUNT(*) FROM exam_results WHERE exam_id = '{exam1Id}' AND applicant_id = '" + user_id + "'; ";

            int exam1Count = int.Parse(SQLClass.Select(checkExam1)[0]);


            if (exam1Count > 0)
            {
                MessageBox.Show("Это предмет уже имеет результаты! Обновите его вместо создания нового!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string query1 = $"INSERT INTO exam_results(result, exam_id, applicant_id, sel) VALUES('{result1}', '{exam1Id}', '{user_id}', 0);";
            MySqlCommand cmd1 = new MySqlCommand(query1, SQLClass.conn);

            if (cmd1.ExecuteNonQuery() == 1)
            {
                MessageBox.Show("Данные внесены!", "Успех");
                LoadExamResults(user_id);
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
        private void ValidateTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar);

            TextBox textBox = sender as TextBox;
            if (textBox != null)
            {
                int value;
                if (int.TryParse(textBox.Text + e.KeyChar, out value))
                {
                    if (value > 100)
                    {
                        e.Handled = true;
                    }
                }
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text) || string.IsNullOrEmpty(user_id) || dataGridView2.SelectedRows.Count <= 0)
            {
                MessageBox.Show("Вы не выбрали запись для обновления или не ввели все необходимые данные!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string exam_result_id = dataGridView2.SelectedRows[0].Cells["id"].Value.ToString();
            string result1 = textBox1.Text;
            string exam1Id = SQLClass.Select($"SELECT id FROM exams WHERE name = '{comboBox1.SelectedItem.ToString()}';")[0];
            string checkDuplicateQuery = $"SELECT COUNT(*) FROM exam_results WHERE exam_id = '{exam1Id}' AND applicant_id = '{user_id}' AND id <> '{exam_result_id}'; ";
            int duplicateCount = int.Parse(SQLClass.Select(checkDuplicateQuery)[0]);

            if (duplicateCount > 0)
            {
                MessageBox.Show("Запись с таким экзаменом уже существует для данного абитуриента!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string query1 = $"UPDATE exam_results SET result = '{result1}', exam_id = '{exam1Id}' WHERE id = '{exam_result_id}';";

            MySqlCommand cmd1 = new MySqlCommand(query1, SQLClass.conn);

            if (cmd1.ExecuteNonQuery() == 1)
            {
                MessageBox.Show("Результат экзамена обновлен!", "Успех");
                LoadExamResults(user_id);
            }
            else
            {
                MessageBox.Show("Ошибка, данные не обновлены!");
            }
        }
        private void button4_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(user_id) || dataGridView2.SelectedRows.Count <= 0)
            {
                MessageBox.Show("Вы не выбрали запись для удаления!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string exam_result_id = dataGridView2.SelectedRows[0].Cells["id"].Value.ToString();

            DialogResult dialogResult = MessageBox.Show("Вы уверены, что хотите удалить выбранный результат экзамена?", "Подтверждение удаления", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (dialogResult == DialogResult.Yes)
            {
                string query1 = $"DELETE FROM exam_results WHERE id = '{exam_result_id}';";

                MySqlCommand cmd1 = new MySqlCommand(query1, SQLClass.conn);

                if (cmd1.ExecuteNonQuery() == 1)
                {
                    MessageBox.Show("Результат экзамена удален!", "Успех");
                    LoadExamResults(user_id);
                }
                else
                {
                    MessageBox.Show("Ошибка, данные не удалены!");
                }
            }
        }
        private void searchButton_Click(object sender, EventArgs e)
        {
            string filter = searchTextBox.Text.Trim();
            FilterApplicantsData(filter);
        }
    }
}