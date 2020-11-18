using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TiPEIS
{
    public partial class FormJournalOfOperations : Form
    {
        private SQLiteConnection sql_con;
        private SQLiteCommand sql_cmd;
        private DataSet DS = new DataSet();
        private DataTable DT = new DataTable();
        private string sPath = "D:\\data\\SQLiteStudio-3.2.1\\SQLiteStudio\\db\\mybd.db";
        public FormJournalOfOperations()
        {
            InitializeComponent();
        }
        private void FormJournalOfOperations_Load(object sender, EventArgs e)
        {
            string ConnectionString = @"Data Source=" + sPath +
";New=False;Version=3";
            String selectCommand = "Select idJournalOfOperations, Date, Sum, Month, NameSubdivision, OperationType,Name, " +
                 "TypeOfCalc,Subdivision From JournalOfOperations Join Subdivision On Subdivision.idSubdivision=JournalOfOperations.Subdivision Join TypeOfCalculation On TypeOfCalculation.idTypeOfCalculation=JournalOfOperations.TypeOfCalc";
            selectTable(ConnectionString, selectCommand);
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
        private void buttonCreate_Click(object sender, EventArgs e)
        {
            Form formAddOperation = new FormAddOperation(null);
            formAddOperation.ShowDialog();
            string ConnectionString = @"Data Source=" + sPath + ";New=False;Version=3";
            String selectCommand = "Select idJournalOfOperations, Date, Sum, Month, NameSubdivision, OperationType,Name, " +
                  "TypeOfCalc,Subdivision From JournalOfOperations Join Subdivision On Subdivision.idSubdivision=JournalOfOperations.Subdivision Join TypeOfCalculation On TypeOfCalculation.idTypeOfCalculation=JournalOfOperations.TypeOfCalc";
            refreshForm(ConnectionString, selectCommand);
        }
        private void buttonEdit_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedCells[0].RowIndex >= 0)
            {
                int CurrentRow = dataGridView1.SelectedCells[0].RowIndex;
                Form formAddOperation = new FormAddOperation(Convert.ToInt32(dataGridView1[0, CurrentRow].Value.ToString()));
                formAddOperation.ShowDialog();
                string ConnectionString = @"Data Source=" + sPath + ";New=False;Version=3";
                String selectCommand = "Select idJournalOfOperations, Date, Sum, Month, NameSubdivision, OperationType,Name, " +
                   "TypeOfCalc,Subdivision From JournalOfOperations Join Subdivision On Subdivision.idSubdivision=JournalOfOperations.Subdivision Join TypeOfCalculation On TypeOfCalculation.idTypeOfCalculation=JournalOfOperations.TypeOfCalc";
                refreshForm(ConnectionString, selectCommand);
            }
        }
        private void ButtonDelete_Click(object sender, EventArgs e)
        {
            string ConnectionString = @"Data Source=" + sPath + ";New=False;Version=3";
            int CurrentRow = dataGridView1.SelectedCells[0].RowIndex;
            string valueId = dataGridView1[0, CurrentRow].Value.ToString();
            String deleteJournalOperation = "delete from JournalOfOperations where idJournalOfOperations=" + valueId;
            changeValue(ConnectionString, deleteJournalOperation);
            String deleteJournalEntries = "delete from JournalEntries where JournalOfOperations=" + valueId;
            changeValue(ConnectionString, deleteJournalEntries);
            String deleteMaterialsJournal = "delete from TablePart where JournalOfOperations=" + valueId;
            changeValue(ConnectionString, deleteMaterialsJournal);
            String selectCommand = "Select idJournalOfOperations, Date, Sum, Month, NameSubdivision, OperationType,Name, " +
                  "TypeOfCalc,Subdivision From JournalOfOperations Join Subdivision On Subdivision.idSubdivision=JournalOfOperations.Subdivision Join TypeOfCalculation On TypeOfCalculation.idTypeOfCalculation=JournalOfOperations.TypeOfCalc";
            selectTable(ConnectionString, selectCommand);
        }

        private void buttonEntries_Click(object sender, EventArgs e)
        {
            Form formJournalEntries = new FormJournalEntries(null);
            formJournalEntries.ShowDialog();          
        }

        private void buttonEntriesOperation_Click(object sender, EventArgs e)
        {
            int CurrentRow = dataGridView1.SelectedCells[0].RowIndex;
            Form formJournalEntries = new FormJournalEntries(Convert.ToInt32(dataGridView1[0, CurrentRow].Value.ToString()));
            formJournalEntries.ShowDialog();
        }
    }
}
