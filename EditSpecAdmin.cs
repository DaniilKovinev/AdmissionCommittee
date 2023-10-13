using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace admission_commision
{
    public partial class EditSpecAdmin : Form
    {
        public string userLogin = string.Empty;
        private string selectedSpecId = string.Empty;
        private string selectedCodeId = string.Empty;

        public EditSpecAdmin()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox2.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox3.DropDownStyle = ComboBoxStyle.DropDownList;
            textBox2.KeyPress += TextBox2_KeyPress;
        }
        public void RefreshData()
        {
            ClearFormFields();
        }
        private void EditSpecAdmin_Load(object sender, EventArgs e)
        {
            LoadComboBoxData();
            comboBox1.SelectedIndexChanged += ComboBox1_SelectedIndexChanged;
            comboBox2.SelectedIndexChanged += ComboBox2_SelectedIndexChanged;
            comboBox3.SelectedIndexChanged += ComboBox3_SelectedIndexChanged;
            dataGridView1.CellClick += dataGridView1_CellClick;
            dataGridView2.CellClick += dataGridView2_CellClick;
            LoadDepartmentsData();
            LoadCodeData();
        }
        private void ClearFormFields()
        {
            dataGridView1.DataSource = null;
            dataGridView2.DataSource = null;
            comboBox2.SelectedIndex = -1;
            comboBox3.SelectedIndex = -1;
            textBox1.Clear();
            textBox2.Clear();
        }
        private void LoadComboBoxData()
        {
            comboBox1.Items.Clear();
            List<string> faculties = SQLClass.Select("SELECT name_fac FROM faculties ORDER BY id;");
            comboBox1.Items.AddRange(faculties.ToArray());

        }
        private void LoadDepartmentsData()
        {
            string selectedDepartment = comboBox2.SelectedItem?.ToString();
            string selectedSpecialty = comboBox3.SelectedItem?.ToString();

            if (selectedDepartment == null || selectedSpecialty == null)
            {
                return;
            }

            string query = $@"SELECT code_specialization.id, name_dep, name_sp, level, form, pay, budget_places, code_specialization.department_id, specialization_id, formPay_id
                              FROM code_specialization
                              INNER JOIN departments ON code_specialization.department_id = departments.id
                              INNER JOIN specialties ON code_specialization.specialization_id = specialties.id
                              INNER JOIN studyForm ON code_specialization.studyForm_id = studyForm.id
                              INNER JOIN studyLevel ON code_specialization.studyLevel_id = studyLevel.id
                              INNER JOIN formPay ON code_specialization.formPay_id = formPay.id
                              WHERE name_dep = '{selectedDepartment}'
                              AND name_sp = '{selectedSpecialty}';";

            MySqlCommand cmd = new MySqlCommand(query, SQLClass.conn);
            MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);

            dataGridView2.DataSource = dt;
            dataGridView2.Columns["name_sp"].HeaderText = "Специальность";
            dataGridView2.Columns["level"].HeaderText = "Форма подготовки";
            dataGridView2.Columns["form"].HeaderText = "Форма обучения";
            dataGridView2.Columns["pay"].HeaderText = "Оплата обучения";
            dataGridView2.Columns["budget_places"].HeaderText = "Бюджетные места";
            dataGridView2.Columns["specialization_id"].Visible = false;
            dataGridView2.Columns["department_id"].Visible = false;
            dataGridView2.Columns["name_dep"].Visible = false;
            dataGridView2.Columns["id"].Visible = false;
            dataGridView2.Columns["formPay_id"].Visible = false;
            dataGridView2.Columns["specialization_id"].Visible = false;
            dataGridView2.AllowUserToAddRows = false;
            dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            if (dataGridView2.Rows.Count > 0)
            {
                dataGridView2.ClearSelection();
                dataGridView2.CurrentCell = null;
            }
        }
        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox2.Items.Clear();
            dataGridView1.DataSource = null;
            dataGridView2.DataSource = null;
            comboBox3.SelectedIndex = -1;
            textBox1.Clear();
            textBox2.Clear();
            string selectedFaculty = comboBox1.SelectedItem?.ToString();

            if (selectedFaculty != null)
            {
                List<string> departments = SQLClass.Select($"SELECT name_dep FROM departments WHERE faculty_id = (SELECT id FROM faculties WHERE name_fac = '{selectedFaculty}') ORDER BY id;");
                comboBox2.Items.AddRange(departments.ToArray());
            }
        }
        private void ComboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox3.Items.Clear();
            dataGridView1.DataSource = null;
            dataGridView2.DataSource = null;
            textBox1.Clear();
            textBox2.Clear();
            string selectedDepartment = comboBox2.SelectedItem?.ToString();

            if (selectedDepartment != null)
            {
                List<string> specialties = SQLClass.Select($"SELECT name_sp FROM specialties WHERE department_id = (SELECT id FROM departments WHERE name_dep = '{selectedDepartment}') ORDER BY id;");
                comboBox3.Items.AddRange(specialties.ToArray());
            }
        }
        private void ComboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadDepartmentsData();
            LoadCodeData();
            if (comboBox3.SelectedItem != null)
            {
                textBox1.Text = comboBox3.SelectedItem.ToString(); 
            }

        }
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow selectedRow = dataGridView1.Rows[e.RowIndex];
                selectedSpecId = selectedRow.Cells["id"].Value.ToString();

            }
        }
        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow selectedRow = dataGridView2.Rows[e.RowIndex];
                selectedCodeId = selectedRow.Cells["id"].Value.ToString();
                string budget_places = selectedRow.Cells["budget_places"].Value.ToString();
                textBox2.Text = budget_places;

            }
        }
        private void LoadCodeData()
        {
            string selectedDepartment = comboBox2.SelectedItem?.ToString();
            string selectedSpecialty = comboBox3.SelectedItem?.ToString();

            if (selectedDepartment == null || selectedSpecialty == null)
            {
                return;
            }

            string query = $@"SELECT specialties.id, name_dep, name_sp
                              FROM specialties
                              INNER JOIN departments ON specialties.department_id = departments.id
                              WHERE name_dep = '{selectedDepartment}'
                              AND name_sp = '{selectedSpecialty}';";

            MySqlCommand cmd = new MySqlCommand(query, SQLClass.conn);
            MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);

            dataGridView1.DataSource = dt;
            dataGridView1.Columns["name_sp"].HeaderText = "Специальность";
            dataGridView1.Columns["name_dep"].HeaderText = "Кафедра";
            dataGridView1.Columns["id"].Visible = false;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
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
            AddSpecAdmin addSpecAdminForm = new AddSpecAdmin
            {
                Owner = this
            };
            addSpecAdminForm.ShowDialog();
        }
        private void button2_Click(object sender, EventArgs e)
        {

            if (dataGridView1.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];
                string name_sp = selectedRow.Cells["name_sp"].Value.ToString();
                string selectedSpId = selectedRow.Cells["specialization_id"].Value.ToString();
                string selectedDepartmentId = selectedRow.Cells["department_id"].Value.ToString();
                string name_spNew = textBox1.Text;

                string updateSpecialtiesQuery = $"UPDATE specialties SET name_sp = '{name_spNew}' WHERE id = {selectedSpId}";
                MySqlCommand specialtiesCmd = new MySqlCommand(updateSpecialtiesQuery, SQLClass.conn);



                string checkDuplicateQuery = $@"SELECT COUNT(*) 
                                                FROM code_specialization
                                                INNER JOIN specialties ON code_specialization.specialization_id = specialties.id
                                                WHERE specialization_id = {selectedSpId} 
                                                AND code_specialization.department_id = {selectedDepartmentId} 
                                                AND name_sp = '{name_spNew}'";

                MySqlCommand checkDuplicateCmd = new MySqlCommand(checkDuplicateQuery, SQLClass.conn);

                try
                {
                    int duplicateCount = Convert.ToInt32(checkDuplicateCmd.ExecuteScalar());

                    if (duplicateCount > 0)
                    {
                        MessageBox.Show("Ошибка: Дубликат записи!");
                        return;
                    }

                    int specialtiesResult = specialtiesCmd.ExecuteNonQuery();
                    if (specialtiesResult != 1)
                    {
                        MessageBox.Show("Ошибка при обновлении данных!");
                        return;
                    }

                    selectedRow.Cells["name_sp"].Value = name_spNew;

                    MessageBox.Show("Данные успешно обновлены!");
                    LoadDepartmentsData();
                    comboBox3.SelectedIndex = -1;
                    textBox1.Text = string.Empty;
                    textBox2.Text = string.Empty;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}");
                }
            }
        }
        private void button5_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox2.Text))
            {
                MessageBox.Show("Пожалуйста, заполните поле бюджетных мест", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (dataGridView2.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dataGridView2.SelectedRows[0];
                string selectedId = selectedRow.Cells["id"].Value.ToString();
                string formPay = selectedRow.Cells["formPay_id"].Value.ToString();
                int budgetPlaces;

                if (int.TryParse(textBox2.Text, out budgetPlaces) && budgetPlaces > 0)
                {
   
                    if (formPay == "4")
                    {
                        MessageBox.Show("Ошибка: Обновление записей с бюджетными местами больше 0 и выбранной оплатой обучения недопустимы!");
                        return;
                    }
                }

                string updateCodeSpecializationQuery = $"UPDATE code_specialization SET budget_places = '{budgetPlaces}' WHERE id = {selectedId}";
                MySqlCommand codeSpecializationCmd = new MySqlCommand(updateCodeSpecializationQuery, SQLClass.conn);

                try
                {
                    int codeSpecializationResult = codeSpecializationCmd.ExecuteNonQuery();
                    if (codeSpecializationResult != 1)
                    {
                        MessageBox.Show("Ошибка при обновлении данных!");
                        return;
                    }

                    MessageBox.Show("Данные успешно обновлены!");
                    LoadDepartmentsData();
                    textBox2.Text = string.Empty;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}");
                }
            }   
        }
        private void button3_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                    string query4 = $"DELETE FROM specialties WHERE id = {selectedSpecId}";
                    MySqlCommand cmd = new MySqlCommand(query4, SQLClass.conn);

                    if (cmd.ExecuteNonQuery() == 1)
                    {
                        LoadCodeData();
                        LoadDepartmentsData();
                        comboBox2.SelectedIndex = -1;
                        comboBox3.SelectedIndex = -1;
                        textBox1.Text = string.Empty;
                        textBox2.Text = string.Empty;
                    }
                    else
                    {
                        MessageBox.Show("Ошибка, данные не удалены!");
                    }
                }
            else
            {
                MessageBox.Show("Ошибка, не выбрана строка!");
            }
        }
        private void button4_Click(object sender, EventArgs e)
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
        private void button7_Click(object sender, EventArgs e)
        {
            AddDirectionAdmin addSpecAdminForm = new AddDirectionAdmin
            {
                Owner = this
            };
            addSpecAdminForm.ShowDialog();
        }
        private void button6_Click(object sender, EventArgs e)
        {
            if (dataGridView2.SelectedRows.Count > 0)
            {
                    string query4 = $"DELETE FROM code_specialization WHERE id = {selectedCodeId}";
                    MySqlCommand cmd = new MySqlCommand(query4, SQLClass.conn);

                    if (cmd.ExecuteNonQuery() == 1)
                    {
                        LoadDepartmentsData();
                        textBox2.Text = string.Empty;
                    }
                    else
                    {
                        MessageBox.Show("Ошибка, данные не удалены!");
                    }
                }
            else
            {
                MessageBox.Show("Ошибка, не выбрана строка!");
            }
        }
    }
}