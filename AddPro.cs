using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Forms;

namespace admission_commision
{
    public partial class AddPro : Form
    {
        public string userLogin = string.Empty;
        public AddPro()
        {
            
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
            comboBox2.DropDownStyle = ComboBoxStyle.DropDownList;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            List<string> id = SQLClass.Select("SELECT id FROM users WHERE login = '" + userLogin + "';");

            var surname = textBox1.Text;
            var name = textBox2.Text;
            var patronymic = textBox3.Text;
            var citizenship = comboBox1.SelectedItem;
            var birthdate = dateTimePicker1.Value.ToString("yyyy-MM-dd");
            var address = textBox4.Text;
            var gender = comboBox2.SelectedItem;
            var email = textBox6.Text;
            var user_id = id[0];

            if ((textBox1.Text.Equals("")) || (comboBox1.Text.Equals("")) || (comboBox2.Text.Equals("")) || (textBox2.Text.Equals("")) || (textBox3.Text.Equals("")))
                MessageBox.Show("Вы не ввели все необходимые данные!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                MySqlCommand checkCmd = new MySqlCommand($"SELECT 1 FROM applicants WHERE user_id = '{user_id}'", SQLClass.conn);
                using (var reader = checkCmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        MessageBox.Show("Запись уже существует!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
                string query = $"INSERT INTO applicants(surname, name, patronymic, citizenship, birthdate, address, gender, email, user_id) VALUES( '{surname}', '{name}', '{patronymic}', '{citizenship}','{birthdate}','{address}','{gender}','{email}','{user_id}');";

                MySqlCommand cmd = new MySqlCommand(query, SQLClass.conn);

                if (cmd.ExecuteNonQuery() == 1)
                {
                    MessageBox.Show("Данные внесены!", "Успех");

                    MySqlDataAdapter adapter = new MySqlDataAdapter();
                    DataTable dt = new DataTable();

                    string query2 = $"SELECT id, login, password, is_admin FROM users WHERE login = '{userLogin}'";

                    MySqlCommand cmd2 = new MySqlCommand(query2, SQLClass.conn);

                    adapter.SelectCommand = cmd2;
                    adapter.Fill(dt);

                    var user = new CheckAdmin(dt.Rows[0].ItemArray[1].ToString(), Convert.ToBoolean(dt.Rows[0].ItemArray[3]));

                    EnterPro rf = new EnterPro(user);
                    rf.userLogin = userLogin;
                    this.Close();
                    rf.Show();
                    rf.Activate();
                }
                else
                {
                    MessageBox.Show("Ошибка, данные не внесены!");
                }

            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            MySqlDataAdapter adapter = new MySqlDataAdapter();
            DataTable dt = new DataTable();

            string query2 = $"SELECT id, login, password, is_admin FROM users WHERE login = '{userLogin}'";

            MySqlCommand cmd2 = new MySqlCommand(query2, SQLClass.conn);

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
