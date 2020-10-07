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
    public partial class FormTypeOfCalculation : Form
    {
        private SQLiteConnection sql_con;
        private SQLiteCommand sql_cmd;
        private DataSet DS = new DataSet();
        private DataTable DT = new DataTable();
        private string sPath = "D:\\data\\SQLiteStudio-3.2.1\\SQLiteStudio\\db\\mybd.db";
        private int minLenght = 2;
        private int maxLenght = 50;

        public FormTypeOfCalculation()
        {
            InitializeComponent();
        }

        private void FormTypeOfCalculation_Load(object sender, EventArgs e)
        {
            string ConnectionString = @"Data Source=" + sPath +
";New=False;Version=3";
            String selectCommand = "Select * from TypeOfCalculation";
            selectTable(ConnectionString, selectCommand);
        }
        private void bindingNavigatorAddNewItem_Click(object sender, EventArgs e)
        {
            if (!(toolStripTextBox1.Text.Length > minLenght && toolStripTextBox1.Text.Length < maxLenght))
            {
                MessageBox.Show("Поле Название должно содержать не менее 3 и не более 50 символов");
                return;
            }
            if (!(toolStripTextBox2.Text.Length > minLenght && toolStripTextBox2.Text.Length < maxLenght))
            {
                MessageBox.Show("Поле Тип должно содержать не менее 3 и не более 50 символов");
                return;
            }
            if (toolStripTextBox3.Text.IndexOf('.') > 0)
            {
                if (toolStripTextBox3.Text.Substring(toolStripTextBox3.Text.IndexOf('.')).Length > 3)
                {
                    MessageBox.Show("Процент должен быть не более 15 символов и иметь не более 2-ух знаков после запятой");
                    return;
                }
            }
            string ConnectionString = @"Data Source=" + sPath +
";New=False;Version=3";
            String selectCommand = "select MAX(idTypeOfCalculation) from TypeOfCalculation";
            object maxValue = selectValue(ConnectionString, selectCommand);
            if (Convert.ToString(maxValue) == "")
                maxValue = 0;
            //вставка в таблицу Employee       
            string txtSQLQuery = "insert into TypeOfCalculation (idTypeOfCalculation,Name,Type,Percent) values (" +
       (Convert.ToInt32(maxValue) + 1) + ", '" + toolStripTextBox1.Text + "', '" + toolStripTextBox2.Text + "','" + toolStripTextBox3.Text + "')";
            ExecuteQuery(txtSQLQuery);
            //обновление dataGridView1
            selectCommand = "select * from TypeOfCalculation";
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
            String selectCommand = "delete from TypeOfCalculation where idTypeOfCalculation=" + valueId;
            string ConnectionString = @"Data Source=" + sPath +
           ";New=False;Version=3";
            changeValue(ConnectionString, selectCommand);
            //обновление dataGridView1
            selectCommand = "select * from TypeOfCalculation";
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
                MessageBox.Show("Поле Название должно содержать не менее 3 и не более 50 символов");
                return;
            }
            if (!(toolStripTextBox2.Text.Length > minLenght && toolStripTextBox2.Text.Length < maxLenght))
            {
                MessageBox.Show("Поле Тип должно содержать не менее 3 и не более 50 символов");
                return;
            }
            if (toolStripTextBox3.Text.IndexOf('.') > 0)
            {
                if (toolStripTextBox3.Text.Substring(toolStripTextBox3.Text.IndexOf('.')).Length > 3)
                {
                    MessageBox.Show("Процент должен быть не более 15 символов и иметь не более 2-ух знаков после запятой");
                    return;
                }
            }
            //выбрана строка CurrentRow
            int CurrentRow = dataGridView1.SelectedCells[0].RowIndex;
            //получить значение FIO выбранной строки
            string valueId = dataGridView1[0, CurrentRow].Value.ToString();
            string changeName = toolStripTextBox1.Text;
            //обновление Name
            String selectCommand = "update TypeOfCalculation set Name='" + changeName + "' where idTypeOfCalculation = " + valueId;
            string ConnectionString = @"Data Source=" + sPath +
       ";New=False;Version=3";
            changeValue(ConnectionString, selectCommand);
            string changeType = toolStripTextBox2.Text;
            selectCommand = "update TypeOfCalculation set Type='" + changeType + "' where idTypeOfCalculation = " + valueId;
            changeValue(ConnectionString, selectCommand);
            string changePercent = toolStripTextBox3.Text;
            string pattern = @"\d{1,15}[.]\d{0,2}$";
            selectCommand = "update TypeOfCalculation set Percent='" + changePercent + "' where idTypeOfCalculation = " + valueId;
            changeValue(ConnectionString, selectCommand);
            //обновление dataGridView1
            selectCommand = "select * from TypeOfCalculation";
            refreshForm(ConnectionString, selectCommand);
            toolStripTextBox1.Text = "";
        }
        private void dataGridView1_CellMouseClick(object sender,
DataGridViewCellMouseEventArgs e)
        {
            //выбрана строка CurrentRow
            int CurrentRow = dataGridView1.SelectedCells[0].RowIndex;
            //получение значения
            string NameId = dataGridView1[1, CurrentRow].Value.ToString();
            toolStripTextBox1.Text = NameId;
            string TypeId = dataGridView1[2, CurrentRow].Value.ToString();
            toolStripTextBox2.Text = TypeId;
            string PercentId = dataGridView1[3, CurrentRow].Value.ToString();
            toolStripTextBox3.Text = PercentId;
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