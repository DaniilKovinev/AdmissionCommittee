﻿using MySql.Data.MySqlClient;
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
    public partial class EditFacultiesAdmin : Form
    {
        public string userLogin = string.Empty;
        public EditFacultiesAdmin()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
        }
        private void EditFacultiesAdmin_Load(object sender, EventArgs e)
        {
            LoadFacultiesData();
            dataGridView1.CellClick += dataGridView1_CellClick;
        }    
        private void LoadFacultiesData()
        {
            string query = "SELECT id, name_fac FROM faculties";
            MySqlCommand cmd = new MySqlCommand(query, SQLClass.conn);
            MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);

            dataGridView1.DataSource = dt;
            dataGridView1.Columns["name_fac"].HeaderText = "Факультет";
            dataGridView1.Columns["id"].Visible = false;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow selectedRow = dataGridView1.Rows[e.RowIndex];
                string name = selectedRow.Cells["name_fac"].Value.ToString();
                textBox1.Text = name;
            }
        }
        private bool IsNameExists(string name)
        {
            string query = $"SELECT COUNT(*) FROM faculties WHERE name_fac = '{name}'";
            MySqlCommand cmd = new MySqlCommand(query, SQLClass.conn);

            int count = Convert.ToInt32(cmd.ExecuteScalar());

            return count > 0;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            string newName = textBox1.Text;

            if (string.IsNullOrWhiteSpace(newName))
            {
                MessageBox.Show("Ошибка, поле для имени пустое!");
                return;
            }

            if (IsNameExists(newName))
            {
                MessageBox.Show("Ошибка, запись с таким именем уже существует!");
                return;
            }

            string query2 = $"INSERT INTO faculties (name_fac) VALUES ('{newName}')";
            MySqlCommand cmd = new MySqlCommand(query2, SQLClass.conn);

            if (cmd.ExecuteNonQuery() == 1)
            {
                LoadFacultiesData();
            }
            else
            {
                MessageBox.Show("Ошибка, данные не внесены!");
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {

            if (dataGridView1.SelectedRows.Count > 0)
            {
                string selectedId = dataGridView1.SelectedRows[0].Cells["id"].Value.ToString();
                string newName = textBox1.Text;

                if (string.IsNullOrWhiteSpace(newName))
                {
                    MessageBox.Show("Ошибка, поле для имени пустое!");
                    return;
                }

                if (IsNameExists(newName))
                {
                    MessageBox.Show("Ошибка, запись с таким именем уже существует!");
                    return;
                }

                string query3 = $"UPDATE faculties SET name_fac = '{newName}' WHERE id = {selectedId}";
                MySqlCommand cmd = new MySqlCommand(query3, SQLClass.conn);

                if (cmd.ExecuteNonQuery() == 1)
                {
                    LoadFacultiesData();
                }
                else
                {
                    MessageBox.Show("Ошибка, данные не внесены!");
                }
            }
            else
            {
                MessageBox.Show("Ошибка, не выбрана строка!");
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                string selectedId = dataGridView1.SelectedRows[0].Cells["id"].Value.ToString();

                string query4 = $"DELETE FROM faculties WHERE id = {selectedId}";
                MySqlCommand cmd = new MySqlCommand(query4, SQLClass.conn);

                if (cmd.ExecuteNonQuery() == 1)
                {
                    LoadFacultiesData();
                }
                else
                {
                    MessageBox.Show("Ошибка, данные не внесены!");
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
    }
}