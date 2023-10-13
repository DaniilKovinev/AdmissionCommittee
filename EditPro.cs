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
using System.Xml.Linq;

namespace admission_commision
{
    public partial class EditPro : Form
    {
        public string userLogin = string.Empty;
        public EditPro()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
            comboBox2.DropDownStyle = ComboBoxStyle.DropDownList;
        }
        private void EditPro_Load(object sender, EventArgs e)
        {
            List<string> id = SQLClass.Select("SELECT id FROM users WHERE login = '" + userLogin + "';");
            var user_id = id[0];

            List<string> applicantEntry = SQLClass.Select("SELECT * FROM applicants WHERE user_id = '" + user_id + "';");
            if (applicantEntry.Count == 0)
            {
                MessageBox.Show("Сначала внесите данные о себе!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            List<string> info = SQLClass.Select("SELECT surname, name, patronymic, citizenship, birthdate, address, gender, email FROM applicants WHERE user_Id = '" + user_id + "';");
            textBox1.Text = info[0];
            textBox2.Text = info[1];
            textBox3.Text = info[2];
            comboBox1.SelectedItem = info[3];
            comboBox2.SelectedItem = info[6];
            textBox5.Text = info[5];
            textBox6.Text = info[7];

            List<string> issueDate = SQLClass.Select("SELECT DATE_FORMAT(birthdate, '%d.%m.%Y') FROM applicants WHERE user_id = '" + user_id + "';");
            DateTime dateOfIssue = DateTime.Parse(issueDate[0]);
            dateTimePicker1.Value = dateOfIssue;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            List<string> id = SQLClass.Select("SELECT id FROM users WHERE login = '" + userLogin + "';");

            var surname = textBox1.Text;
            var name = textBox2.Text;
            var patronymic = textBox3.Text;
            var citizenship = comboBox1.SelectedItem;
            var birthdate = dateTimePicker1.Value.ToString("yyyy-MM-dd");
            var address = textBox5.Text;
            var gender = comboBox2.SelectedItem;
            var email = textBox6.Text;
            var user_id = id[0];

            if ((textBox1.Text.Equals("")) || (textBox2.Text.Equals("")))
                MessageBox.Show("Вы не ввели все необходимые данные!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                string query = $"UPDATE applicants SET surname = '{surname}', name = '{name}', patronymic = '{patronymic}', citizenship = '{citizenship}', birthdate = '{birthdate}', address = '{address}', gender = '{gender}', email = '{email}' WHERE user_id = '{user_id}';";

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
