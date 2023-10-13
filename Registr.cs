using admission_commision;
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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;


namespace admission_commision
{
    public partial class Registr : Form
    {
        public Registr()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
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
            string login = textBox1.Text;
            string pass = textBox2.Text;


            if (checkuser())
            {
                return;
            }

            string hashedPassword = ComputeSha256Hash(pass);
            string query = $"INSERT INTO users(login, password, is_admin) VALUES('{login}','{hashedPassword}', 0)";

            MySqlCommand cmd = new MySqlCommand(query, SQLClass.conn);
            if (cmd.ExecuteNonQuery() == 1)
            {
                MessageBox.Show("Аккаунт успешно создан!", "Успех");
                this.Close();
            }
            else
            {
                MessageBox.Show("Аккаунт не создан!");
            }
        }

        private Boolean checkuser()
        {
            string login = textBox1.Text;

            MySqlDataAdapter adapter = new MySqlDataAdapter();
            DataTable dt = new DataTable();
            string query = $"SELECT id, login FROM users WHERE login = '{login}'";

            MySqlCommand cmd = new MySqlCommand(query, SQLClass.conn);

            adapter.SelectCommand = cmd;
            adapter.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                MessageBox.Show("Пользователь уже существует!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return true;
            }
            else if (dt.Rows.Count == 0 && (textBox1.Text.Equals("")) || (textBox2.Text.Equals("")))
            {
                MessageBox.Show("Вы не ввели все необходимые данные!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return true;
            }
            else
            {
                return false;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Entry rf = new Entry();
            this.Close();
            rf.Show();
            rf.Activate();
        }
    }
}