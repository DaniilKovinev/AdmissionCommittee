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
    public partial class AddEge : Form
    {
        public string userLogin = string.Empty;
        public AddEge()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
            textBox1.KeyPress += ValidateTextBox_KeyPress;
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
        }
        private void AddEge_Load(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();

            List<string> exams = SQLClass.Select("SELECT name FROM exams ORDER BY id;");
            comboBox1.Items.AddRange(exams.ToArray());

            if (comboBox1.Items.Count > 0)
            {
                comboBox1.SelectedIndex = 0;
            }  
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("Вы не ввели все необходимые данные!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            List<string> id = SQLClass.Select("SELECT id FROM users WHERE login = '" + userLogin + "';");

            var result1 = textBox1.Text; 
            var applicant_id = id[0];

            List<string> applicantEntry = SQLClass.Select("SELECT * FROM applicants WHERE user_id = '" + applicant_id + "';");
            if (applicantEntry.Count == 0)
            {
                MessageBox.Show("Сначала внесите данные о себе!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string exam1Id = SQLClass.Select($"SELECT id FROM exams WHERE name = '{comboBox1.SelectedItem.ToString()}';")[0];
            string checkExam1 = $"SELECT COUNT(*) FROM ege_results WHERE exam_id = '{exam1Id}' AND applicant_id = '{applicant_id}';";


            int exam1Count = int.Parse(SQLClass.Select(checkExam1)[0]);


            if (exam1Count > 0)
            {
                MessageBox.Show("Это предмет уже имеет результаты! Обновите его вместо создания нового!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string query1 = $"INSERT INTO ege_results(result, exam_id, applicant_id, sel) VALUES('{result1}', '{exam1Id}', '{applicant_id}', 0);";

            MySqlCommand cmd1 = new MySqlCommand(query1, SQLClass.conn);


            if (cmd1.ExecuteNonQuery() == 1)
            {
                MessageBox.Show("Данные внесены!", "Успех");

                AddEge newAddEgeForm = new AddEge();
                newAddEgeForm.userLogin = this.userLogin;

                this.Close();
                newAddEgeForm.Show();
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
    }
}


