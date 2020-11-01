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
            toolStripComboBoxEmployees.SelectedIndex = -1;
            comboBoxIdSubdivision.SelectedIndex = -1;
            comboBoxTypeOfCalculation.SelectedIndex = -1;
            comboBoxOperationType.SelectedIndex = -1;
            string ConnectionString = @"Data Source=" + sPath + ";New=False;Version=3";
            String selectCommand;
            String selectSubd = "SELECT idSubdivision, NameSubdivision FROM Subdivision";
            selectCombo(ConnectionString, selectSubd, comboBoxIdSubdivision, "NameSubdivision",
"idSubdivision");
            String selectType = "Select idTypeOfCalculation,Name from TypeOfCalculation";
            selectCombo(ConnectionString, selectType, comboBoxTypeOfCalculation, "Name",
"idTypeOfCalculation");
            String selectEmployees = "Select idEmployees,FIO from Employees ";
            selectComboEmployees(ConnectionString, selectEmployees, toolStripComboBoxEmployees, "FIO",
"idEmployees");
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
                comboBoxIdSubdivision.Enabled = false;
                comboBoxTypeOfCalculation.Enabled = false;
                comboBoxOperationType.Enabled = false;
                textBoxNumber.Text = ID.ToString();
                selectCommand = "Select idJournalOfOperations, Date, Sum, Month, NameSubdivision, OperationType,Name, " +
                "TypeOfCalc,Subdivision From JournalOfOperations Join Subdivision On Subdivision.idSubdivision=JournalOfOperations.Subdivision Join TypeOfCalculation On TypeOfCalculation.idTypeOfCalculation=JournalOfOperations.TypeOfCalc where idJournalOfOperations =" + textBoxNumber.Text;
                selectOperation(ConnectionString, selectCommand);
                selectCommand = "Select idTablePart, Sum, FIO, Employees  from TablePart Join Employees On Employees.idEmployees=TablePart.Employees  where JournalOfOperations = " + textBoxNumber.Text;
                selectTable(ConnectionString, selectCommand);
               
            }
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
            comboBoxIdSubdivision.SelectedValue = ds.Tables[0].Rows[0].ItemArray[8].ToString();
            comboBoxTypeOfCalculation.SelectedValue = ds.Tables[0].Rows[0].ItemArray[7].ToString();
            comboBoxOperationType.Text = ds.Tables[0].Rows[0].ItemArray[5].ToString();
            textBoxTotal.Text = ds.Tables[0].Rows[0].ItemArray[2].ToString();
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
        public void selectCombo(string ConnectionString, String selectCommand,
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
            comboBoxIdSubdivision.SelectedIndex = -1;
            comboBoxTypeOfCalculation.SelectedIndex = -1;
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
            toolStripComboBoxEmployees.SelectedIndex = -1;
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
            dataGridView1.Update();
            dataGridView1.Refresh();
            toolStripTextBoxSum.Clear();
            toolStripComboBoxEmployees.SelectedIndex = -1;
            if (dataGridView1.Rows.Count > 0)
            {
                comboBoxIdSubdivision.Enabled = false;
                comboBoxTypeOfCalculation.Enabled = false;
                comboBoxOperationType.Enabled = false;
                buttonAddAll.Enabled = false;
                getSum();
            }
            else
            {
                comboBoxIdSubdivision.Enabled = true;
                comboBoxTypeOfCalculation.Enabled = true;
                comboBoxOperationType.Enabled = true;
                buttonRefresh.Enabled = false;
                textBoxTotal.Clear();
            }
            getSum();
        }
        private void ToolStripButtonAdd_Click(object sender, EventArgs e)
        {
            string ConnectionString = @"Data Source=" + sPath +
    ";New=False;Version=3";
            String selectCommand;
            string month = dateTimePicker2.Value.Month.ToString() + "." + dateTimePicker2.Value.Year.ToString();
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
                    if (comboBoxIdSubdivision.Text != "")
                    {
                        if (comboBoxOperationType.Text != "")
                        {
                            selectCommand = "select Subdivision from Employees where idEmployees = " + toolStripComboBoxEmployees.ComboBox.SelectedValue.ToString();
                            object subd = selectValue(ConnectionString, selectCommand);
                            selectCommand = "select Employees from TablePart where Employees = " + toolStripComboBoxEmployees.ComboBox.SelectedValue.ToString() + " And JournalOfOperations=" + textBoxNumber.Text;
                            object empl = selectValue(ConnectionString, selectCommand);
                            if (empl.ToString() == "")
                            {
                                selectCommand = "select Type from TypeOfCalculation where idTypeOfCalculation = " + comboBoxTypeOfCalculation.SelectedValue.ToString();
                                object type = selectValue(ConnectionString, selectCommand);
                                if (Convert.ToString(type) == comboBoxOperationType.Text)
                                {
                                    if (Convert.ToString(subd) == comboBoxIdSubdivision.SelectedValue.ToString())
                                    {
                                        if (toolStripTextBoxSum.Text != "" && toolStripTextBoxSum.Text != "0")
                                        {
                                            object check = selectValue(ConnectionString, selectCommand);
                                            if (check.ToString() == "")
                                            {
                                                MessageBox.Show("Такой документ уже есть");
                                            }
                                            else
                                            {
                                                selectCommand = "select MAX(idTablePart) from TablePart";
                                                object maxValue = selectValue(ConnectionString, selectCommand);
                                                if (Convert.ToString(maxValue) == "")
                                                    maxValue = 0;
                                                string txtSQLQuery = "insert into TablePart (idTablePart, Employees, Sum, JournalOfOperations) values (" +
                                           (Convert.ToInt32(maxValue) + 1) + ", '" + toolStripComboBoxEmployees.ComboBox.SelectedValue.ToString() + "','" + toolStripTextBoxSum.Text + "','" + textBoxNumber.Text + "')";
                                                ExecuteQuery(txtSQLQuery);
                                                //обновление dataGridView1
                                                selectCommand = "Select idTablePart, Sum, FIO, Employees  from TablePart Join Employees On Employees.idEmployees = TablePart.Employees  where JournalOfOperations = " + textBoxNumber.Text;
                                                refreshForm(ConnectionString, selectCommand);
                                            }
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
                                    MessageBox.Show("Данный тип операции не соответствует виду расчета");
                                }
                            }
                            else
                            {
                                MessageBox.Show("Данный сотрудник уже есть в табличной части");
                            }
                        }
                        else
                        {
                            MessageBox.Show("Выберите тип операции");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Выберите подразделение");
                    }
                }
                else
                {
                    MessageBox.Show("Выберите вид расчета");
                }
            }
            else
            {
                MessageBox.Show("Выберите сотрудника");
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
            string month = dateTimePicker2.Value.Month.ToString() + "." + dateTimePicker2.Value.Year.ToString();
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
                if (toolStripTextBoxSum.Text != "" && toolStripTextBoxSum.Text != "0")
                {

                    //выбрана строка CurrentRow
                    int CurrentRow = dataGridView1.SelectedCells[0].RowIndex;
                    //получить значение FIO выбранной строки
                    string valueId = dataGridView1[0, CurrentRow].Value.ToString();
                    string changeFIO = toolStripComboBoxEmployees.ComboBox.SelectedValue.ToString();
                    //обновление Name
                    selectCommand = "update TablePart set Employees='" + changeFIO + "' where idTablePart = " + valueId;
                    changeValue(ConnectionString, selectCommand);
                    string changeSum = toolStripTextBoxSum.Text;
                    selectCommand = "update TablePart set Sum='" + changeSum + "' where idTablePart = " + valueId;
                    changeValue(ConnectionString, selectCommand);
                    //обновление dataGridView1
                    selectCommand = "Select idTablePart, Sum, FIO, Employees  from TablePart Join Employees On Employees.idEmployees = TablePart.Employees  where JournalOfOperations = " + textBoxNumber.Text;
                    refreshForm(ConnectionString, selectCommand);

                }
                else
                {
                    MessageBox.Show("Введите сумму");
                }
            }
            else
            {
                MessageBox.Show("Выберите сотрудника");
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
                string ConnectionString = @"Data Source=" + sPath +
    ";New=False;Version=3";
                string month = dateTimePicker2.Value.Month.ToString() + "." + dateTimePicker2.Value.Year.ToString();
                if (dataGridView1.Rows.Count > 0)
                {
                    textBoxTotal.Text = textBoxTotal.Text.Replace(",", ".");
                    //вставка в таблицу
                    if (ID == null)
                    {
                        string txtSQLQuery = "insert into JournalOfOperations (idJournalOfOperations, Subdivision,TypeOfCalc, Date, Month, Sum, OperationType) values ('" +
                       textBoxNumber.Text + "', '" + comboBoxIdSubdivision.SelectedValue.ToString() + "','" + comboBoxTypeOfCalculation.SelectedValue.ToString() + "','" + dateTimePicker1.Value.ToShortDateString() + "', '" + month + "', '" + textBoxTotal.Text + "', '" + comboBoxOperationType.Text + "')";
                        ID = Convert.ToInt32(textBoxNumber.Text);
                        ExecuteQuery(txtSQLQuery);
                        MessageBox.Show("Сохранение прошло успешно");
                    }
                    else
                    {
                        String selectCommand = "update JournalOfOperations set Subdivision='" + comboBoxIdSubdivision.SelectedValue.ToString() + "' where idJournalOfOperations=" + textBoxNumber.Text;
                        changeValue(ConnectionString, selectCommand);
                        selectCommand = "update JournalOfOperations set Date='" + dateTimePicker1.Value.ToShortDateString() + "' where idJournalOfOperations=" + textBoxNumber.Text;
                        changeValue(ConnectionString, selectCommand);
                        selectCommand = "update JournalOfOperations set TypeOfCalc='" + comboBoxTypeOfCalculation.SelectedValue.ToString() + "' where idJournalOfOperations=" + textBoxNumber.Text;
                        changeValue(ConnectionString, selectCommand);
                        selectCommand = "update JournalOfOperations set Month='" + month + "' where idJournalOfOperations=" + textBoxNumber.Text;
                        changeValue(ConnectionString, selectCommand);
                        selectCommand = "update JournalOfOperations set Sum='" + textBoxTotal.Text + "' where idJournalOfOperations=" + textBoxNumber.Text;
                        changeValue(ConnectionString, selectCommand);
                        selectCommand = "update JournalOfOperations set OperationType='" + comboBoxOperationType.Text + "' where idJournalOfOperations=" + textBoxNumber.Text;
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
            selectCommand = "Select idTablePart, Sum, FIO, Employees  from TablePart Join Employees On Employees.idEmployees = TablePart.Employees  where JournalOfOperations = " + textBoxNumber.Text;
            refreshForm(ConnectionString, selectCommand);
        }
        private void toolStripComboBoxEmployees_SelectedIndexChanged(object sender, EventArgs e)
        {

            string ConnectionString = @"Data Source=" + sPath + ";New=False;Version=3";
            if (toolStripComboBoxEmployees.ComboBox.SelectedIndex != -1 && comboBoxTypeOfCalculation.SelectedIndex != -1)
            {
                String idEmployee = toolStripComboBoxEmployees.ComboBox.SelectedValue.ToString();
                String selectSalary = "Select Salary from Employees where idEmployees=" + idEmployee;
                object Salary = selectValue(ConnectionString, selectSalary);
                String idPercent = comboBoxTypeOfCalculation.SelectedValue.ToString();
                String selectPercent = "Select Percent from TypeOfCalculation where idTypeOfCalculation=" + idPercent;
                object Percent = selectValue(ConnectionString, selectPercent);
                if (Convert.ToDouble(Percent) == 0)
                {
                    toolStripTextBoxSum.Text = Salary.ToString();
                }
                else
                {
                    double sum = Math.Round(Convert.ToDouble(Salary) * (Convert.ToDouble(Percent) / 100), 2);
                    toolStripTextBoxSum.Text = sum.ToString();
                }
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

        private void buttonAddAll_Click(object sender, EventArgs e)
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
            string month = dateTimePicker2.Value.Month.ToString() + "." + dateTimePicker2.Value.Year.ToString();
            toolStripTextBoxSum.Text = toolStripTextBoxSum.Text.Replace(",", ".");

            if (comboBoxTypeOfCalculation.Text != "")
            {
                if (comboBoxIdSubdivision.Text != "")
                {
                    if (comboBoxOperationType.Text != "")
                    {
                        selectCommand = "Select idJournalOfOperations from JournalOfOperations Where Subdivision=" + comboBoxIdSubdivision.SelectedValue.ToString() + " And Month=" + month + " And TypeOfCalc = " + comboBoxTypeOfCalculation.SelectedValue.ToString();
                        object check = selectValue(ConnectionString, selectCommand);
                        if (check.ToString() == "" && check == null)
                        {
                            MessageBox.Show("Такой документ уже есть");
                        }
                        else
                        {
                            selectCommand = "select Type from TypeOfCalculation where idTypeOfCalculation = " + comboBoxTypeOfCalculation.SelectedValue.ToString();
                            object type = selectValue(ConnectionString, selectCommand);
                            if (Convert.ToString(type) == comboBoxOperationType.Text)
                            {
                                selectCommand = "select COUNT(idEmployees) from Employees Where Subdivision=" + comboBoxIdSubdivision.SelectedValue.ToString();
                                object maxid = selectValue(ConnectionString, selectCommand);
                                if (Convert.ToInt32(maxid) != 0)
                                {
                                    selectCommand = "select MAX(idEmployees) from Employees Where Subdivision=" + comboBoxIdSubdivision.SelectedValue.ToString();
                                    object maxempl = selectValue(ConnectionString, selectCommand);
                                    List<int> empl = new List<int>();
                                    empl.Add(Convert.ToInt32(maxempl));
                                    for (int j = 0; j < Convert.ToInt32(maxid) - 1; j++)
                                    {
                                        selectCommand = "select idEmployees from Employees where Subdivision = " + comboBoxIdSubdivision.SelectedValue.ToString() + " And idEmployees<" + maxempl.ToString();
                                        object id = selectValue(ConnectionString, selectCommand);
                                        int i = Convert.ToInt32(id);
                                        empl.Add(i);
                                        maxempl = id;
                                    }
                                    foreach (var i in empl)
                                    {
                                        selectCommand = "select MAX(idTablePart) from TablePart";
                                        object maxValue = selectValue(ConnectionString, selectCommand);
                                        string sum;
                                        if (comboBoxOperationType.Text == "Выплата")
                                        {
                                            sum = getSumEmplPay(Convert.ToInt32(i)).ToString();
                                        }
                                        else
                                        {
                                            sum = getSumEmpl(Convert.ToInt32(i)).ToString();
                                        }
                                        string sumDouble = sum.Replace(",", ".");
                                        if (Convert.ToString(maxValue) == "")
                                            maxValue = 0;
                                        string txtSQLQuery = "insert into TablePart (idTablePart, Employees, Sum, JournalOfOperations) values (" +
                                   (Convert.ToInt32(maxValue) + 1) + ", '" + i.ToString() + "','" + sumDouble + "','" + textBoxNumber.Text + "')";
                                        ExecuteQuery(txtSQLQuery);
                                    }
                                    //обновление dataGridView1                                
                                    selectCommand = "Select idTablePart, Sum, FIO,Employees  from TablePart Join Employees On Employees.idEmployees = TablePart.Employees  where JournalOfOperations = " + textBoxNumber.Text;
                                    refreshForm(ConnectionString, selectCommand);
                                }
                                else
                                {
                                    MessageBox.Show("По данному подразделению нет сотрудников");
                                }
                            }
                            else
                            {
                                MessageBox.Show("Данный тип операции не соответствует виду расчета");
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Выберите тип операции");
                    }
                }
                else
                {
                    MessageBox.Show("Выберите подразделение");
                }
            }
            else
            {
                MessageBox.Show("Выберите вид расчета");
            }
        }
        public double getSumEmpl(int id)
        {
            string ConnectionString = @"Data Source=" + sPath + ";New=False;Version=3";
            String idEmployee = id.ToString();
            String selectSalary = "Select Salary from Employees where idEmployees=" + idEmployee;
            object Salary = selectValue(ConnectionString, selectSalary);
            String idPercent = comboBoxTypeOfCalculation.SelectedValue.ToString();
            String selectPercent = "Select Percent from TypeOfCalculation where idTypeOfCalculation=" + idPercent;
            object Percent = selectValue(ConnectionString, selectPercent);
            double sum;
            if (Convert.ToDouble(Percent) == 0)
            {
                sum = Convert.ToDouble(Salary);
                toolStripTextBoxSum.Text = sum.ToString();
            }
            else
            {
                sum = Math.Round(Convert.ToDouble(Salary) * (Convert.ToDouble(Percent) / 100), 2);
                toolStripTextBoxSum.Text = sum.ToString();
            }
            return sum;
        }
        public double getSumEmplPay(int id)
        {

            double sum = 0;
            string ConnectionString = @"Data Source=" + sPath + ";New=False;Version=3";
            String idEmployee = id.ToString();
            string month = dateTimePicker2.Value.Month.ToString() + "." + dateTimePicker2.Value.Year.ToString();
            // month = month.Remove(month.Length - 1);
            String selectOperation = "Select COUNT(idJournalOfOperations) from JournalOfOperations Where Subdivision=" + comboBoxIdSubdivision.SelectedValue.ToString() + " And CAST (Month as STRING) =" + month + " And OperationType = " + "'Удержание'";
            object opercount = selectValue(ConnectionString, selectOperation);
            if (Convert.ToInt32(opercount) != 0)
            {
                String selectOperationMax = "Select MAX(idJournalOfOperations) from JournalOfOperations Where Subdivision=" + comboBoxIdSubdivision.SelectedValue.ToString() + " And CAST (Month as STRING) =" + month + " And OperationType = " + "'Удержание'";
                object operMax = selectValue(ConnectionString, selectOperationMax);
                int count = Convert.ToInt32(opercount);
                List<int> oper = new List<int>();
                int opMax = Convert.ToInt32(operMax);
                oper.Add(opMax);
                for (int i = 0; i < count - 1; i++)
                {
                    String selectOper = "Select idJournalOfOperations from JournalOfOperations Where Subdivision=" + comboBoxIdSubdivision.SelectedValue.ToString() + " And CAST (Month as STRING) =" + month + " And idJournalOfOperations<" + opMax.ToString() + " And OperationType = " + "'Удержание'";
                    object op = selectValue(ConnectionString, selectOper);
                    int opCurr = Convert.ToInt32(op);
                    oper.Add(opCurr);
                    opMax = opCurr;
                }
                foreach (int i in oper)
                {
                    String selectOper = "Select idJournalOfOperations from JournalOfOperations Where idJournalOfOperations =" + i.ToString();
                    object op = selectValue(ConnectionString, selectOper);
                    String selectSum = "Select Sum from TablePart Where JournalOfOperations =" + i.ToString() + " And Employees =" + idEmployee;
                    object sumofretention = selectValue(ConnectionString, selectSum);
                    if (sumofretention.ToString() != "")
                        sum += Convert.ToDouble(sumofretention);

                }
                String selectSalary = "Select Salary from Employees where idEmployees=" + idEmployee;
                object Salary = selectValue(ConnectionString, selectSalary);
                double totalsum = Convert.ToDouble(Salary) - sum;
                return totalsum;
            }
            else
            {
                String selectSalary = "Select Salary from Employees where idEmployees=" + idEmployee;
                object Salary = selectValue(ConnectionString, selectSalary);
                double totalsum = Convert.ToDouble(Salary) - sum;
                return totalsum;
            }

        }
        private void comboBoxTypeOfCalculation_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxTypeOfCalculation.Text == "Оклад" || comboBoxTypeOfCalculation.Text == "НДФЛ" || comboBoxTypeOfCalculation.Text == "Выплата")
            {
                buttonAddAll.Enabled = true;
                toolStripComboBoxEmployees.Enabled = false;
                toolStripButtonAdd.Enabled = false;
                toolStripButtonChange.Enabled = false;
                toolStripTextBoxSum.Enabled = false;
            }
            else
            {
                buttonAddAll.Enabled = false;
                toolStripComboBoxEmployees.Enabled = true;
                toolStripButtonAdd.Enabled = true;
                toolStripButtonChange.Enabled = true;
                toolStripTextBoxSum.Enabled = true;
            }
        }

        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            if (comboBoxOperationType.Text == "Выплата")
            {

                textBoxNumber.Text = ID.ToString();
                int count = dataGridView1.Rows.Count;
                String selectCommand;
                string ConnectionString = @"Data Source=" + sPath + ";New=False;Version=3";
                for (int i = 0; i < count; i++)
                {
                    int idEmployee = Convert.ToInt32(dataGridView1.Rows[i].Cells[3].Value);
                    int valueId = Convert.ToInt32(dataGridView1.Rows[i].Cells[0].Value);
                    double sum = getSumEmplPay(idEmployee);
                    string s = sum.ToString().Replace(",", ".");
                    selectCommand = "update TablePart set Sum='" + s + "' where idTablePart = " + valueId;
                    changeValue(ConnectionString, selectCommand);
                }
                selectCommand = "Select idTablePart, Sum, FIO,Employees  from TablePart Join Employees On Employees.idEmployees = TablePart.Employees  where JournalOfOperations = " + textBoxNumber.Text;
                refreshForm(ConnectionString, selectCommand);
            }
            else
            {
                MessageBox.Show("Обновление доступно только для выплат");
            }
        }
    }
}

