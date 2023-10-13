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
    public partial class AddDoc : Form
    {
        public string userLogin = string.Empty;
        public AddDoc()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
            comboBox1.SelectedIndex = 0;
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            List<string> id = SQLClass.Select("SELECT id FROM users WHERE login = '" + userLogin + "';");

            var type = comboBox1.SelectedItem;
            var number = textBox1.Text;
            var issued_by = textBox2.Text;
            var date_issued = dateTimePicker1.Value.ToString("yyyy-MM-dd");
            var applicant_id = id[0];

            List<string> applicantEntry = SQLClass.Select("SELECT * FROM applicants WHERE user_id = '" + applicant_id + "';");
            if (applicantEntry.Count == 0)
            {
                MessageBox.Show("Сначала внесите данные о себе!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if ((textBox1.Text.Equals("")) || (textBox2.Text.Equals("")) || (comboBox1.Text.Equals("")))
                MessageBox.Show("Вы не ввели все необходимые данные!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                List<string> existingDocument = SQLClass.Select($"SELECT * FROM documents WHERE type = '{type}' AND applicant_id = '{applicant_id}';");

                if (existingDocument.Count > 0)
                {
                    MessageBox.Show("Документ с таким названием уже существует!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    string query = $"INSERT INTO documents(type, number, issued_by, date_issued, applicant_id) VALUES( '{type}', '{number}', '{issued_by}','{date_issued}','{applicant_id}');";

                    MySqlCommand cmd = new MySqlCommand(query, SQLClass.conn);

                    if (cmd.ExecuteNonQuery() == 1)
                    {
                        MessageBox.Show("Данные внесены!", "Успех");

                        AddDoc newAddDocForm = new AddDoc();
                        newAddDocForm.userLogin = this.userLogin;

                        this.Close();
                        newAddDocForm.Show();
                    }
                    else
                    {
                        MessageBox.Show("Ошибка, данные не внесены!");
                    }
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
