using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace admission_commision
{
    public partial class EditUsersAdmin : Form
    {
        public string userLogin = string.Empty;
        private string searchQuery = "";
        public EditUsersAdmin()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox2.DropDownStyle = ComboBoxStyle.DropDownList;
            LoadUsersData();
        }
        private void EditUsersAdmin_Load(object sender, EventArgs e)
        {
            LoadUsersData();
            dataGridView1.CellClick += dataGridView1_CellClick;
            comboBox2.SelectedIndex = 0;
        }
        private void LoadUsersData()
        {
            string query = searchQuery != "" ? searchQuery :
            "SELECT users.id, applicants.surname, applicants.name, applicants.patronymic, users.login, users.password, users.is_admin " +
            "FROM users " +
            "LEFT JOIN applicants " +
            "ON users.id = applicants.user_id";

            MySqlCommand cmd = new MySqlCommand(query, SQLClass.conn);
            MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);

            dataGridView1.DataSource = dt;

            dataGridView1.Columns["surname"].HeaderText = "Фамилия";
            dataGridView1.Columns["name"].HeaderText = "Имя";
            dataGridView1.Columns["patronymic"].HeaderText = "Отчество";
            dataGridView1.Columns["login"].HeaderText = "Логин";
            dataGridView1.Columns["password"].Visible = false;
            dataGridView1.Columns["is_admin"].HeaderText = "Администратор?";
            dataGridView1.Columns["id"].Visible = false;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            if (dataGridView1.Rows.Count > 0)
            {
                dataGridView1.ClearSelection();
                dataGridView1.CurrentCell = null;
            }
        }
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow selectedRow = dataGridView1.Rows[e.RowIndex];
                string login = selectedRow.Cells["login"].Value.ToString();
                string password = selectedRow.Cells["password"].Value.ToString();
                bool is_admin = Convert.ToBoolean(selectedRow.Cells["is_admin"].Value);
                textBox1.Text = login;
                textBox2.Text = password;
                comboBox1.SelectedIndex = (is_admin) ? comboBox1.FindStringExact("да") : comboBox1.FindStringExact("нет");
            }
        }
        private bool IsNameExists(string login)
        {
            string query = $"SELECT COUNT(*) FROM users WHERE login = '{login}'";
            MySqlCommand cmd = new MySqlCommand(query, SQLClass.conn);

            int count = Convert.ToInt32(cmd.ExecuteScalar());

            return count > 0;
        }
        private bool IsName1Exists(string login, int userId)
        {
            string query = $"SELECT COUNT(*) FROM users WHERE login = '{login}' AND id <> {userId}";
            MySqlCommand cmd = new MySqlCommand(query, SQLClass.conn);

            int count = Convert.ToInt32(cmd.ExecuteScalar());

            return count > 0;
        }
        private static string ComputeSha256Hash(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            string newLogin = textBox1.Text;
            string newPass = textBox2.Text;
            string selectedAdminText = comboBox1.SelectedItem.ToString();

            int newAdmin = (selectedAdminText.ToLower() == "да") ? 1 : 0;

            if (string.IsNullOrWhiteSpace(newLogin))
            {
                MessageBox.Show("Ошибка, поле для имени пустое!");
                return;
            }

            if (IsNameExists(newLogin))
            {
                MessageBox.Show("Ошибка, запись с таким логином уже существует!");
                return;
            }

            string hashedPassword = ComputeSha256Hash(newPass);

            string query2 = $"INSERT INTO users (login, password, is_admin) VALUES ('{newLogin}', '{hashedPassword}', '{newAdmin}')";
            MySqlCommand cmd = new MySqlCommand(query2, SQLClass.conn);

            if (cmd.ExecuteNonQuery() == 1)
            {
                LoadUsersData();
            }
            else
            {
                MessageBox.Show("Ошибка, данные не внесены!");
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                int selectedId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["id"].Value);
                string newLogin = textBox1.Text;
                string newPass = textBox2.Text;
                string selectedAdminText = comboBox1.SelectedItem.ToString();

                int newAdmin = (selectedAdminText.ToLower() == "да") ? 1 : 0;

                if (string.IsNullOrWhiteSpace(newLogin))
                {
                    MessageBox.Show("Ошибка, поле для имени пустое!");
                    return;
                }

                if (IsName1Exists(newLogin, selectedId))
                {
                    MessageBox.Show("Ошибка, запись с таким логином уже существует!");
                    return;
                }

                string hashedPassword = ComputeSha256Hash(newPass);

                string query3 = $"UPDATE users SET login = '{newLogin}', password = '{hashedPassword}', is_admin = '{newAdmin}' WHERE id = {selectedId}";
                MySqlCommand cmd = new MySqlCommand(query3, SQLClass.conn);

                if (cmd.ExecuteNonQuery() == 1)
                {
                    LoadUsersData();
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
            if (dataGridView1.SelectedRows.Count > 0)
            {
                string selectedId = dataGridView1.SelectedRows[0].Cells["id"].Value.ToString();

                string query4 = $"DELETE FROM users WHERE id = {selectedId}";
                MySqlCommand cmd = new MySqlCommand(query4, SQLClass.conn);

                if (cmd.ExecuteNonQuery() == 1)
                {
                    LoadUsersData();
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
        private void button5_Click(object sender, EventArgs e)
        {
            string searchTerm = searchTextBox.Text.Trim();


            string searchField = (comboBox2.SelectedItem.ToString() == "По Login") ? "login" : "surname";

            searchQuery = $"SELECT users.id, applicants.surname, applicants.name, applicants.patronymic, users.login, users.password, users.is_admin " +
                $"FROM users " +
                $"LEFT JOIN applicants " +
                $"ON users.id = applicants.user_id " +
                $"WHERE {searchField} LIKE '%{searchTerm}%'"; 

            LoadUsersData();
        }

    }
}

