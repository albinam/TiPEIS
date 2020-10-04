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

        public FormEmployees()
        {
            InitializeComponent();
        }

        private void FormEmployees_Load(object sender, EventArgs e)
        {
            string ConnectionString = @"Data Source=" + sPath +
";New=False;Version=3";
            String selectCommand = "Select * from Employees";
            selectTable(ConnectionString, selectCommand);
            String selectSubd = "SELECT idSubdivision, Name FROM Subdivision";
            selectCombo(ConnectionString, selectSubd, toolStripComboBox1, "Name",
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
            string ConnectionString = @"Data Source=" + sPath +
";New=False;Version=3";
            String selectCommand = "select MAX(idEmployees) from Employees";
            object maxValue = selectValue(ConnectionString, selectCommand);
            if (Convert.ToString(maxValue) == "")
                maxValue = 0;
            //вставка в таблицу Employees
            string pattern = @"\d{1,15}[.]\d{0,2}$";
            if (Regex.IsMatch(toolStripTextBox3.Text, pattern, RegexOptions.IgnoreCase))
            {
                string txtSQLQuery = "insert into Employees (idEmployees,FIO, PersonalInfo, Salary, Subdivision) values (" +
           (Convert.ToInt32(maxValue) + 1) + ", '" + toolStripTextBox1.Text + "', '" + toolStripTextBox2.Text + "','" + toolStripTextBox3.Text + "','" + toolStripComboBox1.Text + "')";
                ExecuteQuery(txtSQLQuery);
                //обновление dataGridView1
                selectCommand = "select * from Employees";
                refreshForm(ConnectionString, selectCommand);
                toolStripTextBox1.Text = "";
            }
            else
            {
                MessageBox.Show("Зарплата должна быть не более 15 символов и иметь не более 2-ух знаков после запятой");
            }
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
            selectCommand = "select * from Employees";
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
            string pattern = @"\d{1,15}[.]\d{0,2}$";
            if (Regex.IsMatch(toolStripTextBox3.Text, pattern, RegexOptions.IgnoreCase))
            {
                selectCommand = "update Employees set Salary='" + changeSalary + "' where idEmployees = " + valueId;
                changeValue(ConnectionString, selectCommand);
            }
            else
            {
                MessageBox.Show("Зарплата должна быть не более 15 символов и иметь не более 2-ух знаков после запятой");
            }
                string changeSubdivision = toolStripComboBox1.Text;
            selectCommand = "update Employees set Subdivision='" + changeSubdivision + "' where idEmployees = " + valueId;
            changeValue(ConnectionString, selectCommand);
            //обновление dataGridView1
            selectCommand = "select * from Employees";
            refreshForm(ConnectionString, selectCommand);
            toolStripTextBox1.Text = "";
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
    }
}
