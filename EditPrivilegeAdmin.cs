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
    public partial class EditPrivilegeAdmin : Form
    {
        public string userLogin = string.Empty;
        public EditPrivilegeAdmin()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
            textBox2.KeyPress += TextBox2_KeyPress;
        }
        private void EditPrivilegeAdmin_Load(object sender, EventArgs e)
        {
            LoadPrivilegeData();
            dataGridView1.CellClick += dataGridView1_CellClick;
        }
        private void TextBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }
        private void LoadPrivilegeData()
        {
            string query = "SELECT id, benefits, scores FROM privilege";
            MySqlCommand cmd = new MySqlCommand(query, SQLClass.conn);
            MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);

            dataGridView1.DataSource = dt;

            dataGridView1.Columns["benefits"].HeaderText = "Привилегии";
            dataGridView1.Columns["scores"].HeaderText = "Баллы";
            dataGridView1.Columns["id"].Visible = false;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow selectedRow = dataGridView1.Rows[e.RowIndex];
                string benefits = selectedRow.Cells["benefits"].Value.ToString();
                string scores = selectedRow.Cells["scores"].Value.ToString();
                textBox1.Text = benefits;
                textBox2.Text = scores;
            }
        }
        private bool IsNameExists(string benefits)
        {
            string query = $"SELECT COUNT(*) FROM privilege WHERE benefits = '{benefits}'";
            MySqlCommand cmd = new MySqlCommand(query, SQLClass.conn);

            int count = Convert.ToInt32(cmd.ExecuteScalar());

            return count > 0;
        }
        private bool IsNameExists2(string benefits, int excludeId)
        {
            string query = $"SELECT COUNT(*) FROM privilege WHERE benefits = '{benefits}' AND id != {excludeId}";
            MySqlCommand cmd = new MySqlCommand(query, SQLClass.conn);

            int count = Convert.ToInt32(cmd.ExecuteScalar());

            return count > 0;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            string newName = textBox1.Text;
            string scores = textBox2.Text;

            if (string.IsNullOrWhiteSpace(newName))
            {
                MessageBox.Show("Ошибка, поле для имени пустое!");
                return;
            }

            if (string.IsNullOrWhiteSpace(scores))
            {
                MessageBox.Show("Ошибка, поле для баллов пустое!");
                return;
            }

            if (IsNameExists(newName))
            {
                MessageBox.Show("Ошибка, запись с таким именем уже существует!");
                return;
            }

            string query2 = $"INSERT INTO privilege (benefits, scores) VALUES ('{newName}', '{scores}')";
            MySqlCommand cmd = new MySqlCommand(query2, SQLClass.conn);

            if (cmd.ExecuteNonQuery() == 1)
            {
                LoadPrivilegeData();
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
                string selectedId = dataGridView1.SelectedRows[0].Cells["id"].Value.ToString();
                int selectedIdInt = Convert.ToInt32(selectedId);
                string newName = textBox1.Text;
                string scores = textBox2.Text;

                if (string.IsNullOrWhiteSpace(newName))
                {
                    MessageBox.Show("Ошибка, поле для имени пустое!");
                    return;
                }

                if (string.IsNullOrWhiteSpace(scores))
                {
                    MessageBox.Show("Ошибка, поле для баллов пустое!");
                    return;
                }

                if (IsNameExists2(newName, selectedIdInt)) 
                {
                    MessageBox.Show("Ошибка, запись с таким именем уже существует!");
                    return;
                }

                string query3 = $"UPDATE privilege SET benefits = '{newName}', scores = '{scores}' WHERE id = {selectedId}";
                MySqlCommand cmd = new MySqlCommand(query3, SQLClass.conn);

                if (cmd.ExecuteNonQuery() == 1)
                {
                    LoadPrivilegeData();
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

                string query4 = $"DELETE FROM privilege WHERE id = {selectedId}";
                MySqlCommand cmd = new MySqlCommand(query4, SQLClass.conn);

                if (cmd.ExecuteNonQuery() == 1)
                {
                    LoadPrivilegeData();
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
    }
}
