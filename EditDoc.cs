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
    public partial class EditDoc : Form
    {
        public string userLogin = string.Empty;
        public EditDoc()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
            comboBox1.SelectedIndex = 0;
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
        }
        private void EditEducDoc_Load(object sender, EventArgs e)
        {
            List<string> id = SQLClass.Select("SELECT id FROM users WHERE login = '" + userLogin + "';");
            var applicant_id = id[0];

            List<string> info = SQLClass.Select("SELECT type, number, issued_by, applicant_id FROM documents WHERE applicant_id = '" + applicant_id + "';");

            if (info.Count >= 3) 
            {
                comboBox1.SelectedItem = info[0];
                textBox1.Text = info[1];
                textBox2.Text = info[2];

                List<string> issueDate = SQLClass.Select("SELECT DATE_FORMAT(date_issued, '%d.%m.%Y') FROM documents WHERE applicant_id = '" + applicant_id + "';");
                DateTime dateOfIssue = DateTime.Parse(issueDate[0]);
                dateTimePicker1.Value = dateOfIssue;
            }
            UpdateListboxRecords();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (listBoxRecords.SelectedIndex == -1)
            {
                MessageBox.Show("Пожалуйста, выберите запись из списка.", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            List<string> id = SQLClass.Select("SELECT id FROM users WHERE login = '" + userLogin + "';");

            var type = comboBox1.SelectedItem;
            var number = textBox1.Text;
            var issued_by = textBox2.Text;
            var date_issued = dateTimePicker1.Value.ToString("yyyy-MM-dd");
            var applicant_id = id[0];

            if ((textBox1.Text.Equals("")) || (textBox2.Text.Equals("")))
                MessageBox.Show("Вы не ввели все необходимые данные!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                string selectedRecordId = listBoxRecords.SelectedItem.ToString().Split(',')[0].Split(':')[1].Trim();
                List<string> matchingTypeRecords = SQLClass.Select($"SELECT id FROM documents WHERE applicant_id = '{applicant_id}' AND type = '{type}' AND id != '{selectedRecordId}';");

                if (matchingTypeRecords.Count > 0)
                {
                    MessageBox.Show("Запись с таким же названием типа документа уже существует!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                        string query = $"UPDATE documents SET type = '{type}', number = '{number}', issued_by = '{issued_by}', date_issued = '{date_issued}', applicant_id = '{applicant_id}' WHERE id = '{selectedRecordId}';";
                        MySqlCommand cmd = new MySqlCommand(query, SQLClass.conn);

                        if (cmd.ExecuteNonQuery() == 1)
                        {
                            MessageBox.Show("Данные внесены!", "Успех");

                            EditDoc newEditDocForm = new EditDoc();
                            newEditDocForm.userLogin = this.userLogin;
                            this.Close();
                            newEditDocForm.Show();
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
        private void listBoxRecords_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxRecords.SelectedIndex != -1)
            {
                List<string> id = SQLClass.Select("SELECT id FROM users WHERE login = '" + userLogin + "';");
                var applicant_id = id[0];
                string selectedRecordId = listBoxRecords.SelectedItem.ToString().Split(',')[0].Split(':')[1].Trim();
                List<string> selectedRecordInfo = SQLClass.Select("SELECT type, number, issued_by, DATE_FORMAT(date_issued, '%d.%m.%Y') as date_issued FROM documents WHERE id = '" + selectedRecordId + "';");

                comboBox1.SelectedItem = selectedRecordInfo[0];
                textBox1.Text = selectedRecordInfo[1];
                textBox2.Text = selectedRecordInfo[2];

                DateTime dateOfIssue = DateTime.Parse(selectedRecordInfo[3]);
                dateTimePicker1.Value = dateOfIssue;
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            if (listBoxRecords.SelectedIndex == -1)
            {
                MessageBox.Show("Пожалуйста, выберите запись из списка.", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string selectedRecordId = listBoxRecords.SelectedItem.ToString().Split(',')[0].Split(':')[1].Trim();

            string deleteQuery = $"DELETE FROM documents WHERE id = '{selectedRecordId}';";
            MySqlCommand deleteCmd = new MySqlCommand(deleteQuery, SQLClass.conn);

            if (deleteCmd.ExecuteNonQuery() == 1)
            {
                MessageBox.Show("Запись удалена!", "Успех");
            }
            else
            {
                MessageBox.Show("Ошибка, запись не удалена!");
            }
            UpdateListboxRecords();
        }
        private void UpdateListboxRecords()
        {
            List<string> id = SQLClass.Select("SELECT id FROM users WHERE login = '" + userLogin + "';");
            var applicant_id = id[0];

            List<string> records = SQLClass.Select("SELECT id, type, number, issued_by, DATE_FORMAT(date_issued, '%d.%m.%Y') as date_issued FROM documents WHERE applicant_id = '" + applicant_id + "';");
            listBoxRecords.Items.Clear();
            for (int i = 0; i < records.Count; i += 5)
            {
                listBoxRecords.Items.Add($"ID: {records[i]}, Тип: {records[i + 1]}, Номер: {records[i + 2]}, Выдан: {records[i + 3]}, Дата выдачи: {records[i + 4]}");
            }
        }
    }
}
