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
    public partial class AddPrivilege : Form
    {
        public string userLogin = string.Empty;
        public AddPrivilege()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            textBox1.ReadOnly = true;
            comboBox1.SelectedIndexChanged += ComboBox1_SelectedIndexChanged;
        }
        private void AddPrivelege_Load(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();

            List<string> privileges = SQLClass.Select(
                "SELECT benefits FROM privilege ORDER BY id;");
            comboBox1.Items.AddRange(privileges.ToArray());
            if (comboBox1.Items.Count > 0)
            {
                comboBox1.SelectedIndex = 0;
            }
        }
        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex > -1)
            {
                string privileges = comboBox1.SelectedItem.ToString();
                List<string> score = SQLClass.Select("SELECT scores FROM privilege WHERE benefits = '" + privileges + "';");
                if (score.Count > 0)
                {
                    textBox1.Text = score[0];
                }
                else
                {
                    textBox1.Clear();
                }
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            List<string> id = SQLClass.Select("SELECT id FROM privilege WHERE benefits = '" + comboBox1.SelectedItem + "' ");
            var privilege_Id = id[0];
            List<string> id2 = SQLClass.Select("SELECT id FROM users WHERE login = '" + userLogin + "';");
            var applicant_id = id2[0];

            List<string> applicantEntry = SQLClass.Select("SELECT * FROM applicants WHERE user_id = '" + applicant_id + "';");
            if (applicantEntry.Count == 0)
            {
                MessageBox.Show("Сначала внесите данные о себе!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if ((comboBox1.Text.Equals("")))
                MessageBox.Show("Вы не ввели все необходимые данные!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                List<string> existingEntry = SQLClass.Select($"SELECT * FROM privileged WHERE privilege_Id = '{privilege_Id}' AND applicant_id = '{applicant_id}';");
                if (existingEntry.Count > 0)
                {
                    MessageBox.Show("Запись с данной привилегией уже существует!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string query = $"INSERT INTO privileged(privilege_Id, applicant_id, sel) VALUES( '{privilege_Id}', '{applicant_id}', 0);";

                MySqlCommand cmd = new MySqlCommand(query, SQLClass.conn);

                if (cmd.ExecuteNonQuery() == 1)
                {
                    MessageBox.Show("Данные внесены!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    AddPrivilege newAddPrivilegeForm = new AddPrivilege(); 
                    newAddPrivilegeForm.userLogin = this.userLogin;

                    this.Close(); 
                    newAddPrivilegeForm.Show(); 
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

            string query = $"SELECT id, login, password, is_admin FROM users WHERE login = '{userLogin}'";

            MySqlCommand cmd2 = new MySqlCommand(query, SQLClass.conn);

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
