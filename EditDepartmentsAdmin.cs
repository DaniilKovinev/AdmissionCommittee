using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace admission_commision
{
    public partial class EditDepartmentsAdmin : Form
    {
        public string userLogin = string.Empty;
        private string selectedDepartmentId = string.Empty;
        public EditDepartmentsAdmin()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
        }
        private void EditDepartmentsAdmin_Load(object sender, EventArgs e)
        {
            LoadComboBoxData();
            dataGridView1.CellClick += dataGridView1_CellClick;
            comboBox1.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
        }
        private void LoadComboBoxData()
        {
            comboBox1.Items.Clear();
            List<string> faculties = SQLClass.Select("SELECT name_fac FROM faculties ORDER BY id;");
            comboBox1.Items.AddRange(faculties.ToArray());

            if (comboBox1.Items.Count > 0)
            {
                comboBox1.SelectedIndex = 0;
                LoadDepartmentsData(comboBox1.SelectedItem.ToString());
            }
        }
        private void LoadDepartmentsData(string selectedFaculty)
        {
            string query = $@"SELECT D.id, D.name_dep
                              FROM departments D
                              JOIN faculties F ON D.faculty_id = F.id
                              WHERE F.name_fac = '{selectedFaculty}'";

            MySqlCommand cmd = new MySqlCommand(query, SQLClass.conn);
            MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);

            dataGridView1.DataSource = dt;
            dataGridView1.Columns["name_dep"].HeaderText = "Кафедра";
            dataGridView1.Columns["id"].Visible = false;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedFaculty = comboBox1.SelectedItem.ToString();
            LoadDepartmentsData(selectedFaculty);
        }
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow selectedRow = dataGridView1.Rows[e.RowIndex];
                selectedDepartmentId = selectedRow.Cells["id"].Value.ToString();
                string name = selectedRow.Cells["name_dep"].Value.ToString();

                textBox1.Text = name;
            }
        }
        private bool IsNameExists(string name, int selectedFacultyId)
        {
            string query = $"SELECT COUNT(*) FROM departments WHERE name_dep = '{name}' AND faculty_id = {selectedFacultyId}";
            MySqlCommand cmd = new MySqlCommand(query, SQLClass.conn);

            int count = Convert.ToInt32(cmd.ExecuteScalar());

            return count > 0;
        }
        private int GetSelectedFacultyId(string selectedFacultyName)
        {
            string query = $"SELECT id FROM faculties WHERE name_fac = '{selectedFacultyName}'";
            MySqlCommand cmd = new MySqlCommand(query, SQLClass.conn);

            object result = cmd.ExecuteScalar();

            if (result != null)
            {
                return Convert.ToInt32(result);
            }
            else
            {
                return -1;
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            string newName = textBox1.Text;

            if (string.IsNullOrWhiteSpace(newName))
            {
                MessageBox.Show("Ошибка, поле для имени кафедры пустое!");
                return;
            }

            string selectedFaculty = comboBox1.SelectedItem.ToString();
            int selectedFacultyId = GetSelectedFacultyId(selectedFaculty);

            if (IsNameExists(newName, selectedFacultyId))
            {
                MessageBox.Show("Ошибка, запись с таким именем уже существует!");
                return;
            }

            string query2 = $"INSERT INTO departments (name_dep, faculty_id) VALUES ('{newName}', {selectedFacultyId})";
            MySqlCommand cmd = new MySqlCommand(query2, SQLClass.conn);

            if (cmd.ExecuteNonQuery() == 1)
            {
                LoadDepartmentsData(selectedFaculty);
            }
            else
            {
                MessageBox.Show("Ошибка, данные не внесены!");
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(selectedDepartmentId))
            {
                string newName = textBox1.Text;

                if (string.IsNullOrWhiteSpace(newName))
                {
                    MessageBox.Show("Ошибка, поле для имени кафедры пустое!");
                    return;
                }

                string selectedFaculty = comboBox1.SelectedItem.ToString();
                int selectedFacultyId = GetSelectedFacultyId(selectedFaculty);

                if (IsNameExists(newName, selectedFacultyId))
                {
                    MessageBox.Show("Ошибка, запись с таким именем уже существует!");
                    return;
                }

                string query3 = $"UPDATE departments SET name_dep = '{newName}' WHERE id = {selectedDepartmentId}";
                MySqlCommand cmd = new MySqlCommand(query3, SQLClass.conn);

                if (cmd.ExecuteNonQuery() == 1)
                {
                    LoadDepartmentsData(selectedFaculty);
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
        private void button3_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(selectedDepartmentId))
            {
                string query4 = $"DELETE FROM departments WHERE id = {selectedDepartmentId}";
                MySqlCommand cmd = new MySqlCommand(query4, SQLClass.conn);

                if (cmd.ExecuteNonQuery() == 1)
                {
                    string selectedFaculty = comboBox1.SelectedItem.ToString();
                    LoadDepartmentsData(selectedFaculty);
                }
                else
                {
                    MessageBox.Show("Ошибка, данные не удалены!");
                }
            }
            else
            {
                MessageBox.Show("Ошибка, не выбрана строка!");
            }
        }
        private void button4_Click(object sender, EventArgs e)
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