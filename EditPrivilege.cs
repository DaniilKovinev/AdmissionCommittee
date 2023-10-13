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
    public partial class EditPrivilege : Form
    {
        public string userLogin = string.Empty;
        public EditPrivilege()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            textBox1.ReadOnly = true;
            comboBox1.SelectedIndexChanged += ComboBox1_SelectedIndexChanged;
        }
        private void EditPrivilege_Load(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();

            List<string> privileges = SQLClass.Select(
                "SELECT benefits FROM privilege ORDER BY id;");
            comboBox1.Items.AddRange(privileges.ToArray());

            if (comboBox1.Items.Count > 0)
            {
                comboBox1.SelectedIndex = 0; 
            }
            UpdateListboxRecords();
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
            if (listBoxRecords.SelectedIndex == -1)
            {
                MessageBox.Show("Пожалуйста, выберите запись из списка.", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            List<string> id = SQLClass.Select("SELECT id FROM privilege WHERE benefits = '" + comboBox1.SelectedItem + "' ");
            var privilege_Id = id[0];
            List<string> id2 = SQLClass.Select("SELECT id FROM users WHERE login = '" + userLogin + "';");
            var applicant_id = id2[0];

            string selectedRecordId = listBoxRecords.SelectedItem.ToString().Split(',')[0].Split(':')[1].Trim();

            List<string> existingEntry = SQLClass.Select($"SELECT * FROM privileged WHERE privilege_Id = '{privilege_Id}' AND applicant_id = '{applicant_id}' AND id != '{selectedRecordId}';");

            if (existingEntry.Count > 0)
            {
                MessageBox.Show("Запись с данной привилегией уже существует!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                string query = $"UPDATE privileged SET privilege_Id = {privilege_Id}, applicant_id = {applicant_id} WHERE id = {selectedRecordId};";
                MySqlCommand cmd = new MySqlCommand(query, SQLClass.conn);

                if (cmd.ExecuteNonQuery() == 1)
                {
                    MessageBox.Show("Данные внесены!", "Успех");

                    EditPrivilege newForm = new EditPrivilege();
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
                List<string> selectedRecordInfo = SQLClass.Select("SELECT privilege.benefits FROM privilege INNER JOIN privileged ON privileged.privilege_id = privilege.id WHERE privileged.applicant_id = '" + applicant_id + "' AND privileged.id = '" + selectedRecordId + "';");


                if (selectedRecordInfo.Count > 0)
                {
                    comboBox1.SelectedItem = selectedRecordInfo[0];
                }
                else
                {
                    comboBox1.SelectedIndex = -1;
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


            string deleteQuery = $"DELETE FROM privileged WHERE id = '{selectedRecordId}';";
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

            List<string> records = SQLClass.Select("SELECT privileged.id, privilege.benefits, privilege.scores FROM privilege INNER JOIN privileged ON privileged.privilege_id = privilege.id WHERE privileged.applicant_id = " + applicant_id + ";");
            listBoxRecords.Items.Clear();
            for (int i = 0; i < records.Count; i += 3)
            {
                listBoxRecords.Items.Add($"ID: {records[i]}, Привилегия: {records[i + 1]}, Баллы: {records[i + 2]}");
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
