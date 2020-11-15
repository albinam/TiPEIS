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
    public partial class FormJournalEntries : Form
    {
        private SQLiteConnection sql_con;
        private SQLiteCommand sql_cmd;
        private DataSet DS = new DataSet();
        private DataTable DT = new DataTable();
        private string sPath = "D:\\data\\SQLiteStudio-3.2.1\\SQLiteStudio\\db\\mybd.db";
        private int? ID = null;
        public FormJournalEntries(int? ID)
        {
            this.ID = ID;
            InitializeComponent();
        }
        private void FormJournalEntries_Load(object sender, EventArgs e)
        {
            string ConnectionString = @"Data Source=" + sPath +
        ";New=False;Version=3";
            String selectCommand;
            if (ID != null)
            {
                selectCommand = "Select Date,Sum,Dt,SubkontoDt1,SubkontoDt2,Kt,SubkontoKt1,SubkontoKt2 from JournalEntries where JournalOfOperations=" + ID;
                selectTable(ConnectionString, selectCommand);
                buttonSearch.Enabled = false;
                buttonClear.Enabled = false;
                dateTimePicker1.Enabled = false;
                dateTimePicker2.Enabled = false;
            }
        }
        private void ButtonSearch_Click(object sender, EventArgs e)
        {
            string ConnectionString = @"Data Source=" + sPath +
          ";New=False;Version=3";
            String selectCommand;
            if (ID == null)
            {
                selectCommand = "Select Date,Sum,Dt,SubkontoDt1,SubkontoDt2,Kt,SubkontoKt1,SubkontoKt2 from JournalEntries where Date between '" + dateTimePicker1.Value.ToShortDateString() + "' and '" + dateTimePicker2.Value.ToShortDateString() + "'";
                selectTable(ConnectionString, selectCommand);    
            }
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

        private void ButtonClear_Click(object sender, EventArgs e)
        {
            dataGridView1.Columns.Clear();
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
