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
    public partial class EditEge : Form
    {
        public string userLogin = string.Empty;
        public EditEge()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
            textBox1.KeyPress += ValidateTextBox_KeyPress;
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
        }
        private void EditEge_Load(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();

            List<string> exams = SQLClass.Select("SELECT name FROM exams ORDER BY id;");
            comboBox1.Items.AddRange(exams.ToArray());

            if (comboBox1.Items.Count > 0)
            {
                comboBox1.SelectedIndex = 0;
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

            List<string> id = SQLClass.Select("SELECT id FROM exams WHERE name = '" + comboBox1.SelectedItem + "' ");
            var exam_Id = id[0];
            List<string> id2 = SQLClass.Select("SELECT id FROM users WHERE login = '" + userLogin + "';");
            var applicant_id = id2[0];
            var result = textBox1.Text;

            string selectedRecordId = listBoxRecords.SelectedItem.ToString().Split(',')[0].Split(':')[1].Trim();
            List<string> existingEntry = SQLClass.Select($"SELECT * FROM ege_results WHERE exam_Id = '{exam_Id}' AND applicant_id = '{applicant_id}' AND id != '{selectedRecordId}';");

            if (existingEntry.Count > 0)
            {
                MessageBox.Show("Запись с данной привилегией уже существует!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                string query = $"UPDATE ege_results SET exam_Id = {exam_Id}, result = {result}, applicant_id = {applicant_id} WHERE id = {selectedRecordId};";
                MySqlCommand cmd = new MySqlCommand(query, SQLClass.conn);

                if (cmd.ExecuteNonQuery() == 1)
                {
                    MessageBox.Show("Данные внесены!", "Успех");

                    EditEge newForm = new EditEge();
                    newForm.userLogin = this.userLogin; 

                    this.Close(); 
                    newForm.Show(); 
                }
                else
                {
                    MessageBox.Show("Ошибка, данные не внесены!");
                }
            }
        }
        private void listBoxRecords_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxRecords.SelectedIndex != -1)
            {
                List<string> id2 = SQLClass.Select("SELECT id FROM users WHERE login = '" + userLogin + "';");
                var applicant_id = id2[0];
                string selectedRecordId = listBoxRecords.SelectedItem.ToString().Split(',')[0].Split(':')[1].Trim();
                List<string> selectedRecordInfo = SQLClass.Select("SELECT ege_results.id, exams.name, ege_results.result FROM exams INNER JOIN ege_results ON ege_results.exam_id = exams.id WHERE ege_results.applicant_id = '" + applicant_id + "' AND ege_results.id = '" + selectedRecordId + "';");

                if (selectedRecordInfo.Count > 0)
                {
                    comboBox1.SelectedItem = selectedRecordInfo[1];
                    textBox1.Text = selectedRecordInfo[2];
                }
                else
                {
                    comboBox1.SelectedIndex = -1;
                    textBox1.Text = "";
                }
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
            string deleteQuery = $"DELETE FROM ege_results WHERE id = '{selectedRecordId}';";
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

            List<string> records = SQLClass.Select("SELECT ege_results.id, exams.name, ege_results.result FROM exams INNER JOIN ege_results ON ege_results.exam_id = exams.id WHERE ege_results.applicant_id = " + applicant_id + ";");
            listBoxRecords.Items.Clear();
            for (int i = 0; i < records.Count; i += 3)
            {
                listBoxRecords.Items.Add($"ID: {records[i]}, Экзамен: {records[i + 1]}, Баллы: {records[i + 2]}");
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