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
    public partial class FormAddOperation : Form
    {
        private SQLiteConnection sql_con;
        private SQLiteCommand sql_cmd;
        private DataSet DS = new DataSet();
        private DataTable DT = new DataTable();
        private string sPath = "D:\\data\\SQLiteStudio-3.2.1\\SQLiteStudio\\db\\mybd.db";
        private int? ID = null;
        public FormAddOperation(int? ID)
        {
            this.ID = ID;
            InitializeComponent();
        }
        private void FormAddOperation_Load(object sender, EventArgs e)
        {
            string ConnectionString = @"Data Source=" + sPath + ";New=False;Version=3";
            String selectCommand;
            String selectSubd = "SELECT idSubdivision, Name FROM Subdivision";
            selectComboSubdivision(ConnectionString, selectSubd, toolStripComboBoxIdSubdivision, "Name",
"idSubdivision");
            String selectType = "Select idTypeOfCalculation,Name from TypeOfCalculation";
            selectComboTypeOfCalculation(ConnectionString, selectType, comboBoxTypeOfCalculation, "Name",
"idTypeOfCalculation");
            String selectEmployees = "Select idEmployees,FIO from Employees ";
            selectComboEmployees(ConnectionString, selectEmployees, toolStripComboBoxEmployees, "FIO",
"idEmployees");
            toolStripComboBoxEmployees.SelectedIndex = -1;
            toolStripComboBoxIdSubdivision.SelectedIndex = -1;
            comboBoxTypeOfCalculation.SelectedIndex = -1;
            dateTimePicker2.Format = DateTimePickerFormat.Custom;
            dateTimePicker2.CustomFormat = "MM/yyyy";
            ConnectionString = @"Data Source=" + sPath + ";New=False;Version=3";
            if (ID == null)
            {
                selectCommand = "select MAX(idJournalOfOperations) from JournalOfOperations";
                object maxValue = selectValue(ConnectionString, selectCommand);

                if (Convert.ToString(maxValue) == "")
                    maxValue = 0;
                textBoxNumber.Text = (Convert.ToInt32(maxValue) + 1).ToString();
            }
            else
            {
                textBoxNumber.Text = ID.ToString();
                selectCommand = "Select * from JournalOfOperations where idJournalOfOperations =" + textBoxNumber.Text;
                selectOperation(ConnectionString, selectCommand);
                selectCommand = "Select * from TablePart where idJournalOfOperations = " + textBoxNumber.Text;
                selectTable(ConnectionString, selectCommand);
            }
            getSum();
        }
        public void selectOperation(string ConnectionString, String selectCommand)
        {
            SQLiteConnection connect = new
           SQLiteConnection(ConnectionString);
            connect.Open();
            SQLiteDataAdapter dataAdapter = new
           SQLiteDataAdapter(selectCommand, connect);
            DataSet ds = new DataSet();
            dataAdapter.Fill(ds);
            dateTimePicker1.DataBindings.Add(new Binding("Text", ds.Tables[0], "Date", true));
            dateTimePicker2.DataBindings.Add(new Binding("Text", ds.Tables[0], "Month", true));
            toolStripComboBoxIdSubdivision.Text = ds.Tables[0].Rows[0].ItemArray[5].ToString();
            comboBoxTypeOfCalculation.Text = ds.Tables[0].Rows[0].ItemArray[4].ToString();
            connect.Close();
        }

        public object selectValue(string ConnectionString, String selectCommand)
        {
            SQLiteConnection connect = new SQLiteConnection(ConnectionString);
            connect.Open();
            SQLiteCommand command = new SQLiteCommand(selectCommand, connect);
            SQLiteDataReader reader = command.ExecuteReader();
            object value = "";
            while (reader.Read())
            {
                value = reader[0];
            }
            connect.Close(); return value;
        }


        public void getSum()
        {
            if (dataGridView1.Rows.Count != 0)
            {
                double Sum = 0;
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    Sum += Convert.ToDouble(dataGridView1.Rows[i].Cells[1].Value);
                }
                textBoxTotal.Text = Sum.ToString();
            }
        }
        public void selectComboSubdivision(string ConnectionString, String selectCommand,
       ComboBox comboBox, string displayMember, string valueMember)
        {
            SQLiteConnection connect = new
           SQLiteConnection(ConnectionString);
            connect.Open();
            SQLiteDataAdapter dataAdapter = new
           SQLiteDataAdapter(selectCommand, connect);
            DataSet ds = new DataSet();
            dataAdapter.Fill(ds);
            comboBox.DataSource = ds.Tables[0];
            comboBox.DisplayMember = displayMember;
            comboBox.ValueMember = valueMember;
            connect.Close();
        }
        public void selectComboTypeOfCalculation(string ConnectionString, String selectCommand,
  ComboBox comboBox, string displayMember, string valueMember)
        {
            SQLiteConnection connect = new
           SQLiteConnection(ConnectionString);
            connect.Open();
            SQLiteDataAdapter dataAdapter = new
           SQLiteDataAdapter(selectCommand, connect);
            DataSet ds = new DataSet();
            dataAdapter.Fill(ds);
            comboBox.DataSource = ds.Tables[0];
            comboBox.DisplayMember = displayMember;
            comboBox.ValueMember = valueMember;
            connect.Close();
        }
        public void selectComboEmployees(string ConnectionString, String selectCommand,
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
            selectCommand = "Select * from TablePart where idJournalOfOperations = " + textBoxNumber.Text;
            selectTable(ConnectionString, selectCommand);
            dataGridView1.Update();
            dataGridView1.Refresh();
            toolStripTextBoxSum.Clear();
            toolStripComboBoxEmployees.SelectedIndex = -1;
            getSum();
        }
        private void ToolStripButtonAdd_Click(object sender, EventArgs e)
        {
            string ConnectionString = @"Data Source=" + sPath +
    ";New=False;Version=3";
            String selectCommand;
            if (toolStripTextBoxSum.Text.IndexOf('.') > 0)
            {
                if (toolStripTextBoxSum.Text.Substring(toolStripTextBoxSum.Text.IndexOf('.')).Length > 3)
                {
                    MessageBox.Show("Сумма должна быть не более 15 символов и иметь не более 2-ух знаков после запятой");
                    return;
                }
            }
            toolStripTextBoxSum.Text = toolStripTextBoxSum.Text.Replace(",", ".");
            if (toolStripComboBoxEmployees.Text != "")
            {
                if (comboBoxTypeOfCalculation.Text != "")
                {
                    if (toolStripComboBoxIdSubdivision.Text != "")
                    {
                        selectCommand = "select Subdivision from Employees where idEmployees = " + toolStripComboBoxEmployees.ComboBox.SelectedValue.ToString();
                        object subd = selectValue(ConnectionString, selectCommand);
                        if (Convert.ToString(subd) == toolStripComboBoxIdSubdivision.Text)
                        {
                            if (toolStripTextBoxSum.Text != "" && toolStripTextBoxSum.Text != "0")
                            {
                                selectCommand = "select MAX(idTablePart) from TablePart";
                                object maxValue = selectValue(ConnectionString, selectCommand);
                                if (Convert.ToString(maxValue) == "")
                                    maxValue = 0;
                                string txtSQLQuery = "insert into TablePart (idTablePart, idEmployees, Sum, idJournalOfOperations) values (" +
                           (Convert.ToInt32(maxValue) + 1) + ", '" + toolStripComboBoxEmployees.Text + "','" + toolStripTextBoxSum.Text + "','" + textBoxNumber.Text + "')";
                                ExecuteQuery(txtSQLQuery);
                                //обновление dataGridView1
                                selectCommand = "select * from TablePart";
                                refreshForm(ConnectionString, selectCommand);
                            }
                            else
                            {
                                MessageBox.Show("Введите сумму");
                            }
                        }
                        else
                        {
                            MessageBox.Show("Данный сотрудник не относится к подразделению");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Выберите подразделение");
                    }
                }
                else
                {
                    MessageBox.Show("Выберите тип расчета");
                }
            }
            else
            {
                MessageBox.Show("Выберете сотрудника");
            }
        }
        private void dataGridView1_CellMouseClick(object sender,
    DataGridViewCellMouseEventArgs e)
        {
            //выбрана строка CurrentRow
            int CurrentRow = dataGridView1.SelectedCells[0].RowIndex;
            //получение значения
            string FIOId = dataGridView1[2, CurrentRow].Value.ToString();
            toolStripComboBoxEmployees.Text = FIOId;
            string SumId = dataGridView1[1, CurrentRow].Value.ToString();
            toolStripTextBoxSum.Text = SumId;
        }
        private void ToolStripButtonChange_Click(object sender, EventArgs e)
        {
            string ConnectionString = @"Data Source=" + sPath +
    ";New=False;Version=3";
            String selectCommand;
            if (toolStripTextBoxSum.Text.IndexOf('.') > 0)
            {
                if (toolStripTextBoxSum.Text.Substring(toolStripTextBoxSum.Text.IndexOf('.')).Length > 3)
                {
                    MessageBox.Show("Сумма должна быть не более 15 символов и иметь не более 2-ух знаков после запятой");
                    return;
                }
            }
            toolStripTextBoxSum.Text = toolStripTextBoxSum.Text.Replace(",", ".");
            if (toolStripComboBoxEmployees.Text != "")
            {
                if (comboBoxTypeOfCalculation.Text != "")
                {
                    if (toolStripComboBoxIdSubdivision.Text != "")
                    {
                        selectCommand = "select Subdivision from Employees where idEmployees = " + toolStripComboBoxEmployees.ComboBox.SelectedValue.ToString();
                        object subd = selectValue(ConnectionString, selectCommand);
                        if (Convert.ToString(subd) == toolStripComboBoxIdSubdivision.Text)
                        {
                            if (toolStripTextBoxSum.Text != "" && toolStripTextBoxSum.Text != "0")
                            {
                                //выбрана строка CurrentRow
                                int CurrentRow = dataGridView1.SelectedCells[0].RowIndex;
                                //получить значение FIO выбранной строки
                                string valueId = dataGridView1[0, CurrentRow].Value.ToString();
                                string changeFIO = toolStripComboBoxEmployees.Text;
                                //обновление Name
                                selectCommand = "update TablePart set idEmployees='" + changeFIO + "' where idTablePart = " + valueId;
                                changeValue(ConnectionString, selectCommand);
                                string changeSum = toolStripTextBoxSum.Text;
                                selectCommand = "update TablePart set Sum='" + changeSum + "' where idTablePart = " + valueId;
                                changeValue(ConnectionString, selectCommand);
                                //обновление dataGridView1
                                selectCommand = "select * from TablePart";
                                refreshForm(ConnectionString, selectCommand);
                            }
                            else
                            {
                                MessageBox.Show("Введите сумму");
                            }
                        }
                        else
                        {
                            MessageBox.Show("Данный сотрудник не относится к подразделению");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Выберите подразделение");
                    }
                }
                else
                {
                    MessageBox.Show("Выберите тип расчета");
                }
            }
            else
            {
                MessageBox.Show("Выберете сотрудника");
            }
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
        private void ButtonSave_Click(object sender, EventArgs e)
        {

            if (dateTimePicker1.Value != null && dateTimePicker2.Value != null)
            {
                if (dataGridView1.Rows.Count > 0)
                {
                    string ConnectionString = @"Data Source=" + sPath + ";New=False;Version=3";
                    string month = dateTimePicker2.Value.Month.ToString() + "." + dateTimePicker2.Value.Year.ToString();
                    textBoxTotal.Text = textBoxTotal.Text.Replace(",", ".");
                    //вставка в таблицу
                    if (ID == null)
                    {
                        string txtSQLQuery = "insert into JournalOfOperations (idJournalOfOperations, idSubdivision,idTypeOfCalculation, Date, Month, Sum) values ('" +
                       textBoxNumber.Text + "', '" + toolStripComboBoxIdSubdivision.Text + "','" + comboBoxTypeOfCalculation.Text + "','" + dateTimePicker1.Value.ToShortDateString() + "', '" + month + "', '" + textBoxTotal.Text + "')";
                        ID = Convert.ToInt32(textBoxNumber.Text);
                        ExecuteQuery(txtSQLQuery);
                        MessageBox.Show("Сохранение прошло успешно");
                    }
                    else
                    {
                        String selectCommand = "update JournalOfOperations set idSubdivision='" + toolStripComboBoxIdSubdivision.Text + "' where idJournalOfOperations=" + textBoxNumber.Text;
                        changeValue(ConnectionString, selectCommand);
                        selectCommand = "update JournalOfOperations set Date='" + dateTimePicker1.Value.ToShortDateString() + "' where idJournalOfOperations=" + textBoxNumber.Text;
                        changeValue(ConnectionString, selectCommand);
                        selectCommand = "update JournalOfOperations set Month='" + month + "' where idJournalOfOperations=" + textBoxNumber.Text;
                        changeValue(ConnectionString, selectCommand);
                        selectCommand = "update JournalOfOperations set Sum='" + textBoxTotal.Text + "' where idJournalOfOperations=" + textBoxNumber.Text;
                        changeValue(ConnectionString, selectCommand);
                        MessageBox.Show("Сохранение прошло успешно");
                    }
                }
                else
                {
                    MessageBox.Show("Заполните табличную часть");
                }
            }
            else
            {
                MessageBox.Show("Выберите дату операции и/или месяц расчета");
            }
        }
        private void toolStripButtonDelete_Click(object sender, EventArgs e)
        {
            //выбрана строка CurrentRow
            int CurrentRow = dataGridView1.SelectedCells[0].RowIndex;
            //получить значение idEmployees выбранной строки
            string valueId = dataGridView1[0, CurrentRow].Value.ToString();
            String selectCommand = "delete from TablePart where idTablePart=" + valueId;
            string ConnectionString = @"Data Source=" + sPath +
           ";New=False;Version=3";
            changeValue(ConnectionString, selectCommand);
            //обновление dataGridView1
            selectCommand = "select * from Employees";
            refreshForm(ConnectionString, selectCommand);
        }
        private void toolStripComboBoxEmployees_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxTypeOfCalculation.Text == "Начисление заработной платы")
            {
                string ConnectionString = @"Data Source=" + sPath + ";New=False;Version=3";
                if (toolStripComboBoxEmployees.ComboBox.SelectedValue != null)
                {
                    String idEmployee = toolStripComboBoxEmployees.ComboBox.SelectedValue.ToString();
                    String selectSalary = "Select Salary from Employees where idEmployees=" + idEmployee;
                    object Salary = selectValue(ConnectionString, selectSalary);

                    toolStripTextBoxSum.Enabled = false;
                    toolStripTextBoxSum.Text = Salary.ToString();
                }
            }         
            else
            {
                toolStripTextBoxSum.Text = "";
                toolStripTextBoxSum.Enabled = true;
            }
        }
        private void toolStripTextBoxSum_KeyPress(object sender, KeyPressEventArgs e)
        {
            char l = e.KeyChar;
            if ((l < '0' || l > '9') && l != '\b')
            {
                if (toolStripTextBoxSum.SelectionStart == 0)
                {
                    if (l == '.') e.Handled = true;
                }
                if (l != '.' || toolStripTextBoxSum.Text.IndexOf(".") != -1)
                {
                    e.Handled = true;
                }
            }
        }
    }
}

