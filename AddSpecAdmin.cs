using MySql.Data.MySqlClient;
using System;
using System.Collections;
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
    public partial class AddSpecAdmin : Form
    {
        public AddSpecAdmin()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox2.DropDownStyle = ComboBoxStyle.DropDownList;
        }
        private void AddSpecAdmin_Load(object sender, EventArgs e)
        {
            LoadFaculties();
            comboBox1.SelectedIndexChanged += ComboBox1_SelectedIndexChanged;
        }
        private void LoadFaculties()
        {
            comboBox1.Items.Clear();
            comboBox2.Items.Clear();
            List<string> facult = SQLClass.Select("SELECT name_fac FROM faculties ORDER BY id;");
            comboBox1.Items.AddRange(facult.ToArray());
        }
        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox2.Items.Clear();
            string selectedFaculty = comboBox1.SelectedItem.ToString();
            List<string> dep = SQLClass.Select($"SELECT name_dep FROM departments WHERE faculty_id = (SELECT id FROM faculties WHERE name_fac='{selectedFaculty}') ORDER BY id;");
            comboBox2.Items.AddRange(dep.ToArray());
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem == null || comboBox2.SelectedItem == null || string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("Пожалуйста, заполните все поля!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            string name_sp = textBox1.Text;
            string fac = comboBox1.SelectedItem.ToString().Trim().ToLower();
            string dep = comboBox2.SelectedItem.ToString().Trim().ToLower();


            string searchFacQuery = $"SELECT id FROM faculties WHERE name_fac= '{fac}'";
            MySqlCommand searchFacCmd = new MySqlCommand(searchFacQuery, SQLClass.conn);
            object facIdObj = searchFacCmd.ExecuteScalar();
            int facId = facIdObj != null ? Convert.ToInt32(facIdObj) : -1;

            string searchDepQuery = $"SELECT id FROM departments WHERE name_dep= '{dep}'AND faculty_id = {facId}";
            MySqlCommand searchDepCmd = new MySqlCommand(searchDepQuery, SQLClass.conn);
            object depIdObj = searchDepCmd.ExecuteScalar();
            int depId = depIdObj != null ? Convert.ToInt32(depIdObj) : -1;

            string checkDuplicateQuery = $"SELECT COUNT(*) FROM specialties WHERE department_id = {depId} AND name_sp = '{name_sp}'";
            MySqlCommand checkDuplicateCmd = new MySqlCommand(checkDuplicateQuery, SQLClass.conn);
            int duplicateCount = Convert.ToInt32(checkDuplicateCmd.ExecuteScalar());

            if (duplicateCount > 0)
            {
                MessageBox.Show("Ошибка: Дубликат записи!");
                return;
            }

            string insertSpecialtyQuery = $"INSERT INTO specialties (name_sp, department_id) VALUES ('{name_sp}', {depId});";
            MySqlCommand insertSpecialtyCmd = new MySqlCommand(insertSpecialtyQuery, SQLClass.conn);

            try
            {
                int codeSpecializationResult = insertSpecialtyCmd.ExecuteNonQuery();

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
            EditSpecAdmin editSpecAdminForm = Owner as EditSpecAdmin;
            if (editSpecAdminForm != null)
            {
                editSpecAdminForm.RefreshData();
            }
            this.Close();
        }
    }
}

