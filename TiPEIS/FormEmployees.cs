using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TiPEIS
{
    public partial class FormEmployees : Form
    {
        private SQLiteConnection sql_con;
        private SQLiteCommand sql_cmd;
        private DataSet DS = new DataSet();
        private DataTable DT = new DataTable();
        private string sPath = "D:\\data\\SQLiteStudio-3.2.1\\SQLiteStudio\\db\\mybd.db";
        private int minLenght = 2;
        private int maxLenght = 50;
        public FormEmployees()
        {
            InitializeComponent();
        }

        private void FormEmployees_Load(object sender, EventArgs e)
        {
            string ConnectionString = @"Data Source=" + sPath +
";New=False;Version=3";
            String selectCommand = "Select idEmployees,FIO,PersonalInfo,Salary, NameSubdivision AS Subdivision  from Employees Join Subdivision On Subdivision.idSubdivision=Employees.Subdivision";
            selectTable(ConnectionString, selectCommand);
            String selectSubd = "SELECT idSubdivision, NameSubdivision FROM Subdivision";
            selectCombo(ConnectionString, selectSubd, toolStripComboBox1, "NameSubdivision",
"idSubdivision");
            toolStripComboBox1.SelectedIndex = -1;
        }
        //метод для отображения подразделений в комбобоксе
        public void selectCombo(string ConnectionString, String selectCommand,
ToolStripComboBox comboBox, string displayMember, string valueMember)
        {
            SQLiteConnection connect = new
           SQLiteConnection(ConnectionString);
            connect.Open();
            SQLiteDataAdapter dataAdapter = new
           SQLiteDataAdapter(selectCommand, connect);
            DataSet ds = new DataSet();
            dataAdapter.Fill(ds);
            comboBox.ComboBox.DataSource = ds.Tables[0];
            comboBox.ComboBox.DisplayMember = displayMember;
            comboBox.ComboBox.ValueMember = valueMember;
            connect.Close();
        }
        private void bindingNavigatorAddNewItem_Click(object sender, EventArgs e)
        {
            if (!(toolStripTextBox1.Text.Length > minLenght && toolStripTextBox1.Text.Length < maxLenght))
            {
                MessageBox.Show("Поле ФИО должно содержать не менее 3 и не более 50 символов");
                return;
            }
            if (!(toolStripTextBox2.Text.Length > minLenght && toolStripTextBox2.Text.Length < maxLenght))
            {
                MessageBox.Show("Поле Личные данные должно содержать не менее 3 и не более 50 символов");
                return;
            }
            if (toolStripComboBox1.Text == "")
            {
                MessageBox.Show("Выберите подразделение");
                return;
            }
            //вставка в таблицу Employees           
            if (toolStripTextBox3.Text.IndexOf('.') > 0)
            {
                if (toolStripTextBox3.Text.Substring(toolStripTextBox3.Text.IndexOf('.')).Length > 3)
                {
                    MessageBox.Show("Зарплата должна быть не более 15 символов и иметь не более 2-ух знаков после запятой");
                    return;
                }
            }
            toolStripTextBox3.Text = toolStripTextBox3.Text.Replace(",", ".");
            string ConnectionString = @"Data Source=" + sPath +
";New=False;Version=3";
            String selectCommand = "select MAX(idEmployees) from Employees";
            object maxValue = selectValue(ConnectionString, selectCommand);
            if (Convert.ToString(maxValue) == "")
                maxValue = 0;
            string txtSQLQuery = "insert into Employees (idEmployees,FIO, PersonalInfo, Salary, Subdivision) values (" +
       (Convert.ToInt32(maxValue) + 1) + ", '" + toolStripTextBox1.Text + "', '" + toolStripTextBox2.Text + "','" + toolStripTextBox3.Text + "','" + toolStripComboBox1.ComboBox.SelectedValue + "')";
            ExecuteQuery(txtSQLQuery);
            //обновление dataGridView1
            selectCommand = "Select idEmployees,FIO,PersonalInfo,Salary, NameSubdivision AS Subdivision  from Employees Join Subdivision On Subdivision.idSubdivision=Employees.Subdivision";
            refreshForm(ConnectionString, selectCommand);
        }
        public object selectValue(string ConnectionString, String selectCommand)
        {
            SQLiteConnection connect = new
           SQLiteConnection(ConnectionString);
            connect.Open();
            SQLiteCommand command = new SQLiteCommand(selectCommand,
    connect);
            SQLiteDataReader reader = command.ExecuteReader();
            object value = "";
            while (reader.Read())
            {
                value = reader[0];
            }
            connect.Close();
            return value;
        }
        private void ExecuteQuery(string txtQuery)
        {
            sql_con = new SQLiteConnection("Data Source=" + sPath +
           ";Version=3;New=False;Compress=True;");
            sql_con.Open();
            sql_cmd = sql_con.CreateCommand();
            sql_cmd.CommandText = txtQuery;
            sql_cmd.ExecuteNonQuery();
            sql_con.Close();
        }
        public void refreshForm(string ConnectionString, String selectCommand)
        {
            selectTable(ConnectionString, selectCommand);
            dataGridView1.Update();
            dataGridView1.Refresh();
            toolStripTextBox1.Text = "";
            toolStripTextBox2.Text = "";
            toolStripTextBox3.Text = "";
            toolStripComboBox1.SelectedIndex = -1;
        }
        public void selectTable(string ConnectionString, String selectCommand)
        {
            SQLiteConnection connect = new
           SQLiteConnection(ConnectionString);
            connect.Open();
            SQLiteDataAdapter dataAdapter = new
           SQLiteDataAdapter(selectCommand, connect);
            DataSet ds = new DataSet();
            dataAdapter.Fill(ds);
            dataGridView1.DataSource = ds;
            dataGridView1.DataMember = ds.Tables[0].ToString();
            connect.Close();
        }
        private void toolStripButtonDelete_Click(object sender, EventArgs e)
        {
            //выбрана строка CurrentRow
            int CurrentRow = dataGridView1.SelectedCells[0].RowIndex;
            //получить значение idEmployees выбранной строки
            string valueId = dataGridView1[0, CurrentRow].Value.ToString();
            String selectCommand = "delete from Employees where idEmployees=" + valueId;
            string ConnectionString = @"Data Source=" + sPath +
           ";New=False;Version=3";
            changeValue(ConnectionString, selectCommand);
            //обновление dataGridView1
            selectCommand = "Select idEmployees,FIO,PersonalInfo,Salary, NameSubdivision AS Subdivision  from Employees Join Subdivision On Subdivision.idSubdivision=Employees.Subdivision";
            refreshForm(ConnectionString, selectCommand);
            toolStripTextBox1.Text = "";
        }
        public void changeValue(string ConnectionString, String selectCommand)
        {
            SQLiteConnection connect = new
           SQLiteConnection(ConnectionString);
            connect.Open();
            SQLiteTransaction trans;
            SQLiteCommand cmd = new SQLiteCommand();
            trans = connect.BeginTransaction();
            cmd.Connection = connect;
            cmd.CommandText = selectCommand;
            cmd.ExecuteNonQuery();
            trans.Commit();
            connect.Close();
        }
        private void toolStripButtonEdit_Click(object sender, EventArgs e)
        {
            if (!(toolStripTextBox1.Text.Length > minLenght && toolStripTextBox1.Text.Length < maxLenght))
            {
                MessageBox.Show("Поле ФИО должно содержать не менее 3 и не более 50 символов");
                return;
            }
            if (!(toolStripTextBox2.Text.Length > minLenght && toolStripTextBox2.Text.Length < maxLenght))
            {
                MessageBox.Show("Поле Личные данные должно содержать не менее 3 и не более 50 символов");
                return;
            }
            if (toolStripComboBox1.Text == "")
            {
                MessageBox.Show("Выберите подразделение");
                return;
            }
            //вставка в таблицу Employees           
            if (toolStripTextBox3.Text.IndexOf('.') > 0)
            {
                if (toolStripTextBox3.Text.Substring(toolStripTextBox3.Text.IndexOf('.')).Length > 3)
                {
                    MessageBox.Show("Зарплата должна быть не более 15 символов и иметь не более 2-ух знаков после запятой");
                    return;
                }
            }
            toolStripTextBox3.Text = toolStripTextBox3.Text.Replace(",", ".");
            //выбрана строка CurrentRow
            int CurrentRow = dataGridView1.SelectedCells[0].RowIndex;
            //получить значение FIO выбранной строки
            string valueId = dataGridView1[0, CurrentRow].Value.ToString();
            string changeFIO = toolStripTextBox1.Text;
            //обновление Name
            String selectCommand = "update Employees set FIO='" + changeFIO + "' where idEmployees = " + valueId;
            string ConnectionString = @"Data Source=" + sPath +
       ";New=False;Version=3";
            changeValue(ConnectionString, selectCommand);
            string changePersonalInfo = toolStripTextBox2.Text;
            selectCommand = "update Employees set PersonalInfo='" + changePersonalInfo + "' where idEmployees = " + valueId;
            changeValue(ConnectionString, selectCommand);
            string changeSalary = toolStripTextBox3.Text;
            selectCommand = "update Employees set Salary='" + changeSalary + "' where idEmployees = " + valueId;
            changeValue(ConnectionString, selectCommand);
            string changeSubdivision = toolStripComboBox1.ComboBox.SelectedValue.ToString();
            selectCommand = "update Employees set Subdivision='" + changeSubdivision + "' where idEmployees = " + valueId;
            changeValue(ConnectionString, selectCommand);
            //обновление dataGridView1
            selectCommand = "Select idEmployees,FIO,PersonalInfo,Salary, NameSubdivision AS Subdivision  from Employees Join Subdivision On Subdivision.idSubdivision=Employees.Subdivision";
            refreshForm(ConnectionString, selectCommand);
          
        }
        private void dataGridView1_CellMouseClick(object sender,
    DataGridViewCellMouseEventArgs e)
        {
            //выбрана строка CurrentRow
            int CurrentRow = dataGridView1.SelectedCells[0].RowIndex;
            //получение значения
            string FIOId = dataGridView1[1, CurrentRow].Value.ToString();
            toolStripTextBox1.Text = FIOId;
            string PersonalInfoId = dataGridView1[2, CurrentRow].Value.ToString();
            toolStripTextBox2.Text = PersonalInfoId;
            string SalaryId = dataGridView1[3, CurrentRow].Value.ToString();
            toolStripTextBox3.Text = SalaryId;
            string SubdivisionId = dataGridView1[4, CurrentRow].Value.ToString();
            toolStripComboBox1.Text = SubdivisionId;
        }
        private void toolStripTextBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            char l = e.KeyChar;
            if ((l < '0' || l > '9') && l != '\b')
            {
                if (toolStripTextBox3.SelectionStart == 0)
                {
                    if (l == '.') e.Handled = true;
                }
                if (l != '.' || toolStripTextBox3.Text.IndexOf(".") != -1)
                {
                    e.Handled = true;
                }
            }
        }
    }
}
