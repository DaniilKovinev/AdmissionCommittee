using MySql.Data.MySqlClient;
using Org.BouncyCastle.Ocsp;
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
    public partial class AddDirectionAdmin : Form
    {
        public AddDirectionAdmin()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox2.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox3.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox4.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox5.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox6.DropDownStyle = ComboBoxStyle.DropDownList;
            textBox2.KeyPress += TextBox2_KeyPress;
        }

        private void AddDirectionAdmin_Load(object sender, EventArgs e)
        {
            LoadFaculties();
            comboBox1.SelectedIndexChanged += ComboBox1_SelectedIndexChanged;
            comboBox2.SelectedIndexChanged += ComboBox2_SelectedIndexChanged;
            comboBox3.SelectedIndexChanged += ComboBox3_SelectedIndexChanged;
            comboBox4.SelectedIndexChanged += ComboBox4_SelectedIndexChanged;
            comboBox5.SelectedIndexChanged += ComboBox5_SelectedIndexChanged;
        }
        private void LoadFaculties()
        {
            comboBox1.Items.Clear();
            comboBox2.Items.Clear();
            comboBox3.Items.Clear();
            comboBox4.Items.Clear();
            comboBox5.Items.Clear();
            comboBox6.Items.Clear();

            List<string> facult = SQLClass.Select("SELECT name_fac FROM faculties ORDER BY id;");
            comboBox1.Items.AddRange(facult.ToArray());
        }

        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox2.Items.Clear();
            comboBox3.Items.Clear();
            comboBox4.Items.Clear();
            comboBox5.Items.Clear();
            comboBox6.Items.Clear();

            string selectedFaculty = comboBox1.SelectedItem.ToString();
            List<string> dep = SQLClass.Select($"SELECT name_dep FROM departments WHERE faculty_id = (SELECT id FROM faculties WHERE name_fac='{selectedFaculty}') ORDER BY id;");
            comboBox2.Items.AddRange(dep.ToArray());
        }

        private void ComboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox3.Items.Clear();
            comboBox4.Items.Clear();
            comboBox5.Items.Clear();
            comboBox6.Items.Clear();

            string selectedDepartment = comboBox2.SelectedItem.ToString();
            List<string> spec = SQLClass.Select($"SELECT name_sp FROM specialties WHERE department_id = (SELECT id FROM departments WHERE name_dep='{selectedDepartment}') ORDER BY id;");
            comboBox3.Items.AddRange(spec.ToArray());
        }
        private void ComboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox4.Items.Clear();
            comboBox5.Items.Clear();
            comboBox6.Items.Clear();

            List<string> level = SQLClass.Select
            ($@"SELECT level FROM studyLevel ORDER BY id;");

            comboBox4.Items.AddRange(level.ToArray());
        }
        private void ComboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox5.Items.Clear();
            comboBox6.Items.Clear();

            List<string> stform = SQLClass.Select($@"SELECT form FROM studyForm ORDER BY id;");
            comboBox5.Items.AddRange(stform.ToArray());
        }
        private void ComboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox6.Items.Clear();

            List<string> pay = SQLClass.Select ($@"SELECT pay FROM formPay ORDER BY id;");

            comboBox6.Items.AddRange(pay.ToArray());
        }
        private void TextBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != (char)Keys.Back)
            {
                e.Handled = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem == null || comboBox2.SelectedItem == null || comboBox4.SelectedItem == null || string.IsNullOrWhiteSpace(textBox2.Text))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            string name_sp = comboBox3.SelectedItem.ToString().Trim().ToLower();
            string fac = comboBox1.SelectedItem.ToString().Trim().ToLower();
            string dep = comboBox2.SelectedItem.ToString().Trim().ToLower();
            string form = comboBox5.SelectedItem.ToString().Trim().ToLower();
            string level = comboBox4.SelectedItem.ToString().Trim().ToLower();
            string pay = comboBox6.SelectedItem.ToString().Trim().ToLower();

            string searchFacQuery = $"SELECT id FROM faculties WHERE name_fac= '{fac}'";
            MySqlCommand searchFacCmd = new MySqlCommand(searchFacQuery, SQLClass.conn);
            object facIdObj = searchFacCmd.ExecuteScalar();
            int facId = facIdObj != null ? Convert.ToInt32(facIdObj) : -1;

            string searchDepQuery = $"SELECT id FROM departments WHERE name_dep= '{dep}'AND faculty_id = {facId}";
            MySqlCommand searchDepCmd = new MySqlCommand(searchDepQuery, SQLClass.conn);
            object depIdObj = searchDepCmd.ExecuteScalar();
            int depId = depIdObj != null ? Convert.ToInt32(depIdObj) : -1;

            string searchSpecialtyQuery = $"SELECT id FROM specialties WHERE name_sp = '{name_sp}' AND department_id = {depId}";
            MySqlCommand searchSpecialtyCmd = new MySqlCommand(searchSpecialtyQuery, SQLClass.conn);
            object specialtyIdObj = searchSpecialtyCmd.ExecuteScalar();
            int specialtyId = specialtyIdObj != null ? Convert.ToInt32(specialtyIdObj) : -1;

            string searchFormQuery = $"SELECT id FROM studyForm WHERE form = '{form}'";
            MySqlCommand searchFormCmd = new MySqlCommand(searchFormQuery, SQLClass.conn);
            object FormIdObj = searchFormCmd.ExecuteScalar();
            int FormId = FormIdObj != null ? Convert.ToInt32(FormIdObj) : -1;

            string searchLevelQuery = $"SELECT id FROM studyLevel WHERE level = '{level}'";
            MySqlCommand searchLevelCmd = new MySqlCommand(searchLevelQuery, SQLClass.conn);
            object levelIdObj = searchLevelCmd.ExecuteScalar();
            int levelId = levelIdObj != null ? Convert.ToInt32(levelIdObj) : -1;

            string searchPayQuery = $"SELECT id FROM formPay WHERE pay = '{pay}'";
            MySqlCommand searchPayCmd = new MySqlCommand(searchPayQuery, SQLClass.conn);
            object payIdObj = searchPayCmd.ExecuteScalar();
            int payId = payIdObj != null ? Convert.ToInt32(payIdObj) : -1;

            int budgetPlaces;
            if (int.TryParse(textBox2.Text, out budgetPlaces) && budgetPlaces > 0)
            {
                string studyFormPay = comboBox6.SelectedItem?.ToString().ToLower();
                if (studyFormPay == "платно")
                {
                    MessageBox.Show("Ошибка: Вставка записей с бюджетными местами больше 0 и выбранной оплатой обучения недопустимы!");
                    return;
                }
            }

            string checkDuplicateQuery = $"SELECT COUNT(*) FROM code_specialization WHERE department_id = {depId} AND specialization_id = {specialtyId} AND studyForm_id = {FormId} AND studyLevel_id = {levelId} AND formPay_id = {payId}";
            MySqlCommand checkDuplicateCmd = new MySqlCommand(checkDuplicateQuery, SQLClass.conn);
            int duplicateCount = Convert.ToInt32(checkDuplicateCmd.ExecuteScalar());

            if (duplicateCount > 0)
            {
                MessageBox.Show("Ошибка: Дубликат записи!");
                return;
            }


            string codeSpecializationInsertQuery = $"INSERT INTO code_specialization (budget_places, specialization_id, department_id, studyForm_id, studyLevel_id, formPay_id) VALUES ({budgetPlaces}, {specialtyId}, {depId}, {FormId}, {levelId}, {payId})";
            MySqlCommand codeSpecializationInsertCmd = new MySqlCommand(codeSpecializationInsertQuery, SQLClass.conn);

            try
            {
                int codeSpecializationResult = codeSpecializationInsertCmd.ExecuteNonQuery();

                if (codeSpecializationResult != 1)
                {
                    MessageBox.Show("Ошибка при внесении данных!");
                    return;
                }

                MessageBox.Show("Данные успешно внесены!");

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            {
                EditSpecAdmin editSpecAdminForm = Owner as EditSpecAdmin;
                if (editSpecAdminForm != null)
                {
                    editSpecAdminForm.RefreshData();
                }

                this.Close();
            }
        }
    }
}
