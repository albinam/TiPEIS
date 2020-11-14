using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiPEIS
{
    public class JournalEntries
    {
        private SQLiteConnection sql_con;
        private SQLiteCommand sql_cmd;
        private DataSet DS = new DataSet();
        private DataTable DT = new DataTable();
        private string sPath = "D:\\data\\SQLiteStudio-3.2.1\\SQLiteStudio\\db\\mybd.db";
        private string Id = null;
        private string DebitAccount;
        private string CreditAccount;
        private string SubcontoDt1;
        private string SubcontoKt1;
        private string SubcontoDt2;
        private string SubcontoKt2;
        private string Sum = null;
        private string Date;
        private string IdOperationsJournal;
        private string Count;
        private string TablePartId;


        public void addPostingJournal(string date, string IdJournalOperations, string TablePart, string Employees, string Type, string Sum, string Subdivision)
        {
            string ConnectionString = @"Data Source=" + sPath + ";New=False;Version=3";
            String selectCommand;
            this.IdOperationsJournal = IdJournalOperations;
            this.Count = "1";
            this.TablePartId = TablePart;
            selectCommand = "select MAX(idJournalEntries) from JournalEntries";
            string maxValue = selectValue(ConnectionString, selectCommand);
            if (maxValue == "")
                maxValue = "0";
            this.Id = (Convert.ToInt32(maxValue) + 1).ToString();
            this.Date = date;
            selectCommand = "select Type from TypeOfCalculation where idTypeOfCalculation=" + Type;
            object type = selectValue(ConnectionString, selectCommand);
            string OperationType = type.ToString();

            if (OperationType == "Начисление")
            {
                accrual(IdJournalOperations, Employees, Type, Sum, Subdivision);
            }
            if (OperationType == "Удержание")
            {
                withholding(IdJournalOperations, Employees, Type, Sum, Subdivision);
            }
            if (OperationType == "Выплата")
            {
                payout(IdJournalOperations, Employees, Type, Sum, Subdivision);
            }
        }
        public void /*Начисление*/ accrual(string IdJournalOperations, string Employees, string Type, string Sum, string Subdivision)
        {
            string ConnectionString = @"Data Source=" + sPath + ";New=False;Version=3";
            String selectCommand;
            #region Дебет Начисление
            selectCommand = "select Sum from TablePart where JournalOfOperations =" + IdJournalOperations + " And Employees=" + Employees;
            this.Sum = selectValue(ConnectionString, selectCommand);

            selectCommand = "select ChartOfAccounts from Subdivision where idSubdivision=" + Subdivision;
            this.DebitAccount = selectValue(ConnectionString, selectCommand);  

            selectCommand = "select NameSubdivision from Subdivision where idSubdivision=" + Subdivision;
            this.SubcontoDt1 = selectValue(ConnectionString, selectCommand);

            #endregion

            #region Кредит 70
            this.CreditAccount = "" + 70;

            selectCommand = "select FIO from Employees where idEmployees=" + Employees;
            this.SubcontoKt1 = selectValue(ConnectionString, selectCommand);

            selectCommand = "select NameSubdivision from Subdivision where idSubdivision=" + Subdivision;
            this.SubcontoKt1 = selectValue(ConnectionString, selectCommand);
            #endregion

            addRecord();
        }
        public void /*Удержание*/ withholding(string IdJournalOperations, string Employees, string Type, string Sum, string Subdivision)
        {
            string ConnectionString = @"Data Source=" + sPath + ";New=False;Version=3";
            String selectCommand;
            #region Дебет 70
            this.DebitAccount = "" + 70;

            selectCommand = "select FIO from Employees where idEmployees=" + Employees;
            this.SubcontoDt1 = selectValue(ConnectionString, selectCommand);
            selectCommand = "select NameSubdivision from Subdivision where idSubdivision=" + Subdivision;
            this.SubcontoDt2 = selectValue(ConnectionString, selectCommand);
            #endregion

            #region Кредит Удержание

            selectCommand = "select Name from TypeOfCalculation where idTypeOfCalculation =" + Type;
            string name = selectValue(ConnectionString, selectCommand);

            if (name == "НДФЛ")
                this.CreditAccount = "" + 68;
            if (name == "Алименты")
                this.CreditAccount = "" + 76;
            if (name == "Недостача")
                this.CreditAccount = "" + 94;

            selectCommand = "select Sum from TablePart where JournalOfOperations =" + IdJournalOperations + " And Employees=" + Employees;
            this.Sum = selectValue(ConnectionString, selectCommand);
            selectCommand = "select Name from TypeOfCalculation where idTypeOfCalculation =" + Type;
            this.SubcontoKt1 = selectValue(ConnectionString, selectCommand);
            selectCommand = "select Type from TypeOfCalculation where idTypeOfCalculation=" + Type;
            this.SubcontoKt2 = selectValue(ConnectionString, selectCommand);
            #endregion

            addRecord();
        }
        public void /*Выплаты*/ payout(string IdJournalOperations, string Employees, string Type, string Sum, string Subdivision)
        {
            string ConnectionString = @"Data Source=" + sPath + ";New=False;Version=3";
            String selectCommand;
            #region Дебет 70
            this.DebitAccount = "" + 70;

            selectCommand = "select FIO from Employees where idEmployees=" + Employees;
            this.SubcontoDt1 = selectValue(ConnectionString, selectCommand);

            selectCommand = "select NameSubdivision from Subdivision where idSubdivision=" + Subdivision;
            this.SubcontoDt2 = selectValue(ConnectionString, selectCommand);
            #endregion
            selectCommand = "select Sum from TablePart where JournalOfOperations =" + IdJournalOperations + " And Employees=" + Employees;
            this.Sum = selectValue(ConnectionString, selectCommand);

            #region Кредит 51
            this.CreditAccount = "" + 51;

            selectCommand = "select Name from TypeOfCalculation where idTypeOfCalculation =" + Type;
            this.SubcontoKt1 = selectValue(ConnectionString, selectCommand);
            #endregion

            addRecord();
        }

        public void addRecord()
        {
            string txtSQLQuery = "insert into JournalEntries (idJournalEntries,TablePart, Dt,Kt,SubkontoDt1,SubkontoKt1,SubkontoDt2,SubkontoKt2,Sum,Date,JournalOfOperations, Count) values (" +
           Id + ", " + TablePartId + ", "+ DebitAccount + ", " + CreditAccount + ", '" + SubcontoDt1 + "', '" + SubcontoKt1 + "', '"
           + SubcontoDt2 + "', '" + SubcontoKt2 + "', '" + Sum + "', '" + Date + "', " + IdOperationsJournal + ", " + Count + ")";
            ExecuteQuery(txtSQLQuery);
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

        public string selectValue(string ConnectionString, String selectCommand)
        {
            SQLiteConnection connect = new SQLiteConnection(ConnectionString);
            connect.Open();
            SQLiteCommand command = new SQLiteCommand(selectCommand, connect);
            SQLiteDataReader reader = command.ExecuteReader();
            string value = "";
            while (reader.Read())
            {
                value = reader[0].ToString();
            }
            connect.Close(); return value;
        }
      
    }
}
