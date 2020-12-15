using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Office.Interop.Excel;
using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using Ionic.Zip;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TiPEIS
{
    public partial class FormReport : Form
    {
        public SQLiteConnection sql_con;
        private SQLiteCommand sql_cmd;
        private DataSet DS = new DataSet();
        private System.Data.DataTable DT = new System.Data.DataTable();
        private string sPath = "D:\\data\\SQLiteStudio-3.2.1\\SQLiteStudio\\db\\mybd.db";

        public FormReport()
        {

            InitializeComponent();
        }

        private void FormReport_Load(object sender, EventArgs e)
        {

            comboBoxTypeOperation.Items.Add("Оборотно-сальдовая ведомость");
            comboBoxTypeOperation.Items.Add("Взаиморасчёт");
            comboBoxTypeOperation.Items.Add("Ведомость выплат");
            comboBoxTypeOperation.SelectedIndex = -1;

        }

        private void ComboBoxTypeOperation_SelectedIndexChanged(object sender, EventArgs e)
        {
            string ConnectionString = @"Data Source=" + sPath + ";New=False;Version=3";
            comboBoxAccount.Enabled = false;
            comboBoxSubdivision.Enabled = false;
            comboBoxEmployee.Enabled = false;
            dateTimePicker2.Enabled = true;
            String select = "";

            if (comboBoxTypeOperation.SelectedIndex == 0)
            {
                comboBoxAccount.Enabled = true;
                select = "Select Account, Description from ChartOfAccounts";
                selectCombo(ConnectionString, select, comboBoxAccount, "Account", "Description");
                comboBoxAccount.SelectedIndex = 1;
            }
            if (comboBoxTypeOperation.SelectedIndex == 1)
            {
                comboBoxSubdivision.SelectedIndex = -1;
                select = "Select NameSubdivision, idSubdivision from Subdivision";
                selectCombo(ConnectionString, select, comboBoxSubdivision, "NameSubdivision", "idSubdivision");
                comboBoxSubdivision.Enabled = true;
                comboBoxSubdivision.SelectedIndex = 1;
            }
            if (comboBoxTypeOperation.SelectedIndex == 2)
            {
                comboBoxSubdivision.SelectedIndex = -1;
                select = "Select NameSubdivision, idSubdivision from Subdivision";
                selectCombo(ConnectionString, select, comboBoxSubdivision, "NameSubdivision", "idSubdivision");
                dateTimePicker2.Enabled = false;
                comboBoxAccount.Enabled = false;
                comboBoxSubdivision.Enabled = true;
                comboBoxEmployee.Enabled = true;
            }
        }

        private void ComboBoxSubdivision_SelectedIndexChanged(object sender, EventArgs e)
        {
            string ConnectionString = @"Data Source=" + sPath + ";New=False;Version=3";
            if ((comboBoxSubdivision.SelectedIndex != -1) && (comboBoxSubdivision.Enabled == true))
            {

                comboBoxEmployee.Enabled = true;
                string select = "Select idEmployees, FIO, PersonalInfo, Subdivision from Employees where Subdivision ="
                    + comboBoxSubdivision.SelectedValue;
                selectCombo(ConnectionString, select, comboBoxEmployee, "FIO", "idEmployees");
                comboBoxEmployee.SelectedIndex = -1;
            }
        }

        public void selectCombo(string ConnectionString, String selectCommand, ComboBox comboBox, string displayMember, string valueMember)
        {
            SQLiteConnection connect = new SQLiteConnection(ConnectionString);
            connect.Open();
            SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(selectCommand, connect);
            DataSet ds = new DataSet();
            dataAdapter.Fill(ds);
            comboBox.DataSource = ds.Tables[0];
            comboBox.DisplayMember = displayMember;
            comboBox.ValueMember = valueMember;
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

        public void selectTable(string ConnectionString, String selectCommand)
        {
            SQLiteConnection connect = new SQLiteConnection(ConnectionString);
            connect.Open();
            SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(selectCommand, connect);
            DataSet ds = new DataSet();
            dataAdapter.Fill(ds);
            dataGridView1.DataSource = ds;
            dataGridView1.DataMember = ds.Tables[0].ToString();
            connect.Close();
        }

        private void buttonCreate_Click(object sender, EventArgs e)
        {
            string ConnectionString = @"Data Source=" + sPath + ";New=False;Version=3";
            String selectCommand = "";
            string dateFrom = dateTimePicker1.Value.ToString("yyyy-MM-dd");
            string dateTo = dateTimePicker2.Value.ToString("yyyy-MM-dd");
            string stock = comboBoxSubdivision.Text.ToString();

            int count = 0;
            double sum = 0;

            if (comboBoxTypeOperation.SelectedIndex == 0)
            {
                if (dateTimePicker1.Value.Date > dateTimePicker2.Value.Date)
                {
                    MessageBox.Show("Дата начала периода должна быть меньше дата конца периода");
                    return;
                }

                string account = comboBoxAccount.Text.ToString();
                string year1 = dateTimePicker1.Value.ToString("dd.mm.yy");
                string year2 = dateTimePicker2.Value.ToString("dd.mm.yy");
                selectCommand = "SELECT JournalOfOperations, JournalOfOperations.OperationType, " +
                    "ROUND(SUM(CASE WHEN JournalEntries.Date < '" + dateFrom + "'AND  Dt = '" + account + "' THEN JournalEntries.Sum ELSE 0 END),2), " +
                    "ROUND(SUM(CASE WHEN JournalEntries.Date < '" + dateFrom + "' AND Kt = '" + account + "' THEN JournalEntries.Sum ELSE 0 END),2), " +
                    "ROUND(SUM(CASE WHEN JournalEntries.Date >= '" + dateFrom + "' AND JournalEntries.Date <= '" + dateTo + "' AND  Dt = '" + account + "' THEN JournalEntries.Sum ELSE 0 END),2), " +
                    "ROUND(SUM(CASE WHEN JournalEntries.Date >= '" + dateFrom + "' AND JournalEntries.Date <= '" + dateTo + "' AND  Kt = '" + account + "' THEN JournalEntries.Sum ELSE 0 END),2), " +
                    "ROUND(SUM(CASE WHEN JournalEntries.Date < '" + dateTo + "' AND  Dt = '" + account + "' THEN JournalEntries.Sum ELSE 0 END),2), " +
                    "ROUND(SUM(CASE WHEN JournalEntries.Date < '" + dateTo + "' AND  Kt = '" + account + "' THEN JournalEntries.Sum ELSE 0 END),2)" +
                    "FROM JournalEntries JOIN JournalOfOperations ON JournalEntries.JournalOfOperations = JournalOfOperations.idJournalOfOperations WHERE JournalEntries.Date >= '" + dateFrom + "' and JournalEntries.Date <= '" + dateTo + "' AND JournalEntries.Dt='" + account + "' OR JournalEntries.Kt='" + account + "' GROUP BY JournalOfOperations";

                selectTable(ConnectionString, selectCommand);

                dataGridView1.Columns[0].HeaderCell.Value = "Код операции";
                dataGridView1.Columns[1].HeaderCell.Value = "Наименование операции";
                dataGridView1.Columns[2].HeaderCell.Value = "Начальный остаток дебет";
                dataGridView1.Columns[3].HeaderCell.Value = "Начальный остаток кредит";
                dataGridView1.Columns[4].HeaderCell.Value = "Дебетовый оборот";
                dataGridView1.Columns[5].HeaderCell.Value = "Кредитовый оборот";
                dataGridView1.Columns[6].HeaderCell.Value = "Конечный остаток дебет";
                dataGridView1.Columns[7].HeaderCell.Value = "Конечный остаток кредит";

                count = 0;
                sum = 0;
                while (count <= (Convert.ToInt32(dataGridView1.RowCount.ToString()) - 1))
                {
                    sum += Convert.ToDouble(dataGridView1[2, count].Value);
                    count++;
                }
                labelSum.Text = "" + Convert.ToString(sum);

                count = 0;
                sum = 0;
                while (count <= (Convert.ToInt32(dataGridView1.RowCount.ToString()) - 1))
                {
                    sum += Convert.ToDouble(dataGridView1[3, count].Value);
                    count++;
                }
                labelSum.Text = labelSum.Text + " " + Convert.ToString(sum);

                count = 0;
                sum = 0;
                while (count <= (Convert.ToInt32(dataGridView1.RowCount.ToString()) - 1))
                {
                    sum += Convert.ToDouble(dataGridView1[4, count].Value);
                    count++;
                }
                labelSum.Text = labelSum.Text + " " + Convert.ToString(sum);

                count = 0;
                sum = 0;
                while (count <= (Convert.ToInt32(dataGridView1.RowCount.ToString()) - 1))
                {
                    sum += Convert.ToDouble(dataGridView1[5, count].Value);
                    count++;
                }
                labelSum.Text = labelSum.Text + " " + Convert.ToString(sum);

                count = 0;
                sum = 0;
                while (count <= (Convert.ToInt32(dataGridView1.RowCount.ToString()) - 1))
                {
                    sum += Convert.ToDouble(dataGridView1[6, count].Value);
                    count++;
                }
                labelSum.Text = labelSum.Text + " " + Convert.ToString(sum);

                count = 0;
                sum = 0;
                while (count <= (Convert.ToInt32(dataGridView1.RowCount.ToString()) - 1))
                {
                    sum += Convert.ToDouble(dataGridView1[7, count].Value);
                    count++;
                }
                labelSum.Text = labelSum.Text + " " + Convert.ToString(sum);
            }

            if (comboBoxTypeOperation.SelectedIndex == 1)
            {
                if (comboBoxEmployee.SelectedIndex == -1)
                {
                    MessageBox.Show("Выберите сотрудника");
                    return;
                }
                if (comboBoxSubdivision.SelectedIndex == -1)
                {
                    MessageBox.Show("Выберите подразделение");
                    return;
                }
                selectCommand = "SELECT idEmployees, " +
                    "CASE WHEN JournalEntries.Dt LIKE '70' THEN SubkontoDt1 ELSE SubkontoKt1 END AS " +
                    "FIO, SUM(CASE WHEN JournalEntries.Kt LIKE '70' AND JournalEntries.Date >='" + dateFrom + "' AND JournalEntries.Date <= '" + dateTo + "' THEN JournalEntries.Sum ELSE 0 END), " +
                    "SUM(CASE WHEN JournalEntries.Dt LIKE '70' AND  JournalEntries.Kt NOT LIKE '51' " +
                    "AND  JournalEntries.SubkontoKt1 LIKE 'НДФЛ' AND JournalEntries.Date >='" + dateFrom + "' AND JournalEntries.Date <= '" + dateTo + "' THEN JournalEntries.Sum ELSE 0 END), " +
                    "SUM(CASE WHEN JournalEntries.Dt LIKE '70' AND  JournalEntries.Kt NOT LIKE '51' " +
                    "AND  JournalEntries.SubkontoKt1 NOT LIKE 'НДФЛ' AND JournalEntries.Date >='" + dateFrom + "' AND JournalEntries.Date <= '" + dateTo + "' THEN JournalEntries.Sum ELSE 0 END), " +
                    "SUM(CASE WHEN JournalEntries.Dt LIKE '70' AND  JournalEntries.Kt LIKE '51' AND JournalEntries.Date >='" + dateFrom + "' AND JournalEntries.Date <= '" + dateTo + "' " +
                    "THEN JournalEntries.Sum ELSE 0 END) ";

                if (comboBoxEmployee.SelectedIndex == -1)
                {

                    selectCommand = selectCommand + "FROM JournalEntries, Employees WHERE (JournalEntries.SubkontoDt1 = Employees.FIO OR " +
                    "JournalEntries.SubkontoKt1 = Employees.FIO) AND (Subdivision = '" + comboBoxSubdivision.SelectedValue + "')";
                }
                else
                {
                    selectCommand = selectCommand + "FROM JournalEntries, Employees WHERE (JournalEntries.SubkontoDt1 = Employees.FIO OR " +
                    "JournalEntries.SubkontoKt1 = Employees.FIO) AND (JournalEntries.SubkontoDt1 ='" + comboBoxEmployee.Text + "' OR " +
                    "JournalEntries.SubkontoKt1 ='" + comboBoxEmployee.Text + "')";
                }

                selectCommand = selectCommand + " GROUP BY FIO";
                selectTable(ConnectionString, selectCommand);
                dataGridView1.Columns[0].HeaderCell.Value = "Табельный номер";
                dataGridView1.Columns[1].HeaderCell.Value = "ФИО";
                dataGridView1.Columns[2].HeaderCell.Value = "Начислено";
                dataGridView1.Columns[3].HeaderCell.Value = "Удержано НДФЛ";
                dataGridView1.Columns[4].HeaderCell.Value = "Удержано другое";
                dataGridView1.Columns[5].HeaderCell.Value = "Выплачено";

                count = 0;
                sum = 0;
                while (count <= (Convert.ToInt32(dataGridView1.RowCount.ToString()) - 1))
                {
                    sum += Convert.ToDouble(dataGridView1[2, count].Value);
                    count++;
                }
                labelSum.Text = "" + Convert.ToString(sum);

                count = 0;
                sum = 0;
                while (count <= (Convert.ToInt32(dataGridView1.RowCount.ToString()) - 1))
                {
                    sum += Convert.ToDouble(dataGridView1[3, count].Value);
                    count++;
                }
                labelSum.Text = labelSum.Text + " " + Convert.ToString(sum);

                count = 0;
                sum = 0;
                while (count <= (Convert.ToInt32(dataGridView1.RowCount.ToString()) - 1))
                {
                    sum += Convert.ToDouble(dataGridView1[4, count].Value);
                    count++;
                }
                labelSum.Text = labelSum.Text + " " + Convert.ToString(sum);

                count = 0;
                sum = 0;
                while (count <= (Convert.ToInt32(dataGridView1.RowCount.ToString()) - 1))
                {
                    sum += Convert.ToDouble(dataGridView1[5, count].Value);
                    count++;
                }
                labelSum.Text = labelSum.Text + " " + Convert.ToString(sum);
            }
            if (comboBoxTypeOperation.SelectedIndex == 2)
            {
                if (comboBoxSubdivision.SelectedIndex == -1)
                {
                    MessageBox.Show("Выберите подразделение");
                    return;
                }
                string year = dateTimePicker1.Value.ToString("yyyy");
                selectCommand = "SELECT SubkontoDt1, " +
                    "SUM(CASE WHEN SUBSTR(JournalEntries.Date, 5, 4) LIKE '-01-' AND SUBSTR(JournalEntries.Date, 1, 4) ='" + year + "' THEN JournalEntries.Sum ELSE 0 END), " +
                    "SUM(CASE WHEN SUBSTR(JournalEntries.Date, 5, 4) LIKE '-02-' AND SUBSTR(JournalEntries.Date, 1, 4) ='" + year + "' THEN JournalEntries.Sum ELSE 0 END), " +
                    "SUM(CASE WHEN SUBSTR(JournalEntries.Date, 5, 4) LIKE '-03-' AND SUBSTR(JournalEntries.Date, 1, 4) ='" + year + "' THEN JournalEntries.Sum ELSE 0 END), " +
                    "SUM(CASE WHEN SUBSTR(JournalEntries.Date, 5, 4) LIKE '-04-' AND SUBSTR(JournalEntries.Date, 1, 4) ='" + year + "' THEN JournalEntries.Sum ELSE 0 END), " +
                    "SUM(CASE WHEN SUBSTR(JournalEntries.Date, 5, 4) LIKE '-05-' AND SUBSTR(JournalEntries.Date, 1, 4) ='" + year + "' THEN JournalEntries.Sum ELSE 0 END), " +
                    "SUM(CASE WHEN SUBSTR(JournalEntries.Date, 5, 4) LIKE '-06-' AND SUBSTR(JournalEntries.Date, 1, 4) ='" + year + "' THEN JournalEntries.Sum ELSE 0 END), " +
                    "SUM(CASE WHEN SUBSTR(JournalEntries.Date, 5, 4) LIKE '-07-' AND SUBSTR(JournalEntries.Date, 1, 4) ='" + year + "' THEN JournalEntries.Sum ELSE 0 END), " +
                    "SUM(CASE WHEN SUBSTR(JournalEntries.Date, 5, 4) LIKE '-08-' AND SUBSTR(JournalEntries.Date, 1, 4) ='" + year + "' THEN JournalEntries.Sum ELSE 0 END), " +
                    "SUM(CASE WHEN SUBSTR(JournalEntries.Date, 5, 4) LIKE '-09-' AND SUBSTR(JournalEntries.Date, 1, 4) ='" + year + "' THEN JournalEntries.Sum ELSE 0 END), " +
                    "SUM(CASE WHEN SUBSTR(JournalEntries.Date, 5, 4) LIKE '-10-' AND SUBSTR(JournalEntries.Date, 1, 4) ='" + year + "' THEN JournalEntries.Sum ELSE 0 END), " +
                    "SUM(CASE WHEN SUBSTR(JournalEntries.Date, 5, 4) LIKE '-11-' AND SUBSTR(JournalEntries.Date, 1, 4) ='" + year + "' THEN JournalEntries.Sum ELSE 0 END), " +
                    "SUM(CASE WHEN SUBSTR(JournalEntries.Date, 5, 4) LIKE '-12-' AND SUBSTR(JournalEntries.Date, 1, 4) ='" + year + "' THEN JournalEntries.Sum ELSE 0 END), " +
                    "SUM(CASE WHEN SUBSTR(JournalEntries.Date, 1, 4) LIKE '" + year + "' THEN JournalEntries.Sum ELSE 0 END) " +
                    "FROM JournalEntries WHERE SubkontoKt1 = 'Выплата' AND  SubkontoDt2= '" + comboBoxSubdivision.Text + "' GROUP BY SubkontoDt1";

                selectTable(ConnectionString, selectCommand);
                dataGridView1.Columns[0].HeaderCell.Value = "Сотрудник";
                dataGridView1.Columns[1].HeaderCell.Value = "Январь";
                dataGridView1.Columns[2].HeaderCell.Value = "Февраль";
                dataGridView1.Columns[3].HeaderCell.Value = "Март";
                dataGridView1.Columns[4].HeaderCell.Value = "Апрель";
                dataGridView1.Columns[5].HeaderCell.Value = "Май";
                dataGridView1.Columns[6].HeaderCell.Value = "Июнь";
                dataGridView1.Columns[7].HeaderCell.Value = "Июль";
                dataGridView1.Columns[8].HeaderCell.Value = "Август";
                dataGridView1.Columns[9].HeaderCell.Value = "Сентябрь";
                dataGridView1.Columns[10].HeaderCell.Value = "Октябрь";
                dataGridView1.Columns[11].HeaderCell.Value = "Ноябрь";
                dataGridView1.Columns[12].HeaderCell.Value = "Декабрь";
                dataGridView1.Columns[13].HeaderCell.Value = "Итого";

                foreach (DataGridViewColumn column in dataGridView1.Columns)
                {
                    column.Visible = true;
                    bool nulable = false;
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        if (dataGridView1.Rows[row.Index].Cells[column.Index].Value.ToString() == "0")
                        {
                            nulable = true;
                        }
                        else
                        {
                            nulable = false;
                            break;
                        }
                    }
                    if (nulable)
                    {
                        column.Visible = false;
                    }
                }
                count = 0;
                sum = 0;
                while (count <= (Convert.ToInt32(dataGridView1.RowCount.ToString()) - 1))
                {
                    sum += Convert.ToDouble(dataGridView1[13, count].Value);
                    count++;
                }
                labelSum.Text = "" + Convert.ToString(sum);

            }
            else if (comboBoxTypeOperation.SelectedIndex.ToString() == "-1")
            {
                MessageBox.Show("Выберите вид отчета");
            }
        }

        private void buttonPDF_Click(object sender, EventArgs e)
        {

            if (dateTimePicker1.Value.Date >= dateTimePicker2.Value.Date)
            {
                MessageBox.Show("Дата начала должна быть меньше даты окончания",
               "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = "doc|*.doc"
            };
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    savePDF(sfd.FileName);
                    MessageBox.Show("Выполнено", "Успех", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK,
                   MessageBoxIcon.Error);
                }
            }
        }
        public void savePDF(string FileName)
        {
            string FONT_LOCATION = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "arial.TTF"); //определяем В СИСТЕМЕ(чтобы не копировать файл) расположение шрифта arial.ttf
            BaseFont baseFont = BaseFont.CreateFont(FONT_LOCATION, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED); //создаем шрифт
            iTextSharp.text.Font fontParagraph = new iTextSharp.text.Font(baseFont, 17, iTextSharp.text.Font.NORMAL); //регистрируем + можно задать параметры для него(17 - размер, последний параметр - стиль)
            string title = "";
            if (comboBoxTypeOperation.SelectedItem.ToString() == "Оборотно-сальдовая ведомость")
            {
                title = "Оборотно-сальдовая ведомость по счёту " + comboBoxAccount.Text + " \"" + comboBoxAccount.SelectedValue + "\" с " + Convert.ToString(dateTimePicker1.Text) + " по " + Convert.ToString(dateTimePicker2.Text) + "\n\n";
            }
            if (comboBoxTypeOperation.SelectedItem.ToString() == "Взаиморасчёт")
            {
                title = "Ведомость взаиморасчёта с сотрудниками " + comboBoxSubdivision.Text + " " + comboBoxEmployee.Text + " с " + Convert.ToString(dateTimePicker1.Text) + " по " + Convert.ToString(dateTimePicker2.Text) + "\n\n";
            }
            if (comboBoxTypeOperation.SelectedItem.ToString() == "Ведомость выплат")
            {
                title = "Ведомость выплат за год " + dateTimePicker1.Value.ToString("yyyy") + "\n\n";
            }

            var phraseTitle = new Phrase(title,
            new iTextSharp.text.Font(baseFont, 18, iTextSharp.text.Font.BOLD));
            iTextSharp.text.Paragraph paragraph = new
           iTextSharp.text.Paragraph(phraseTitle)
            {
                Alignment = Element.ALIGN_CENTER,
                SpacingAfter = 12
            };

            PdfPTable table = new PdfPTable(dataGridView1.Columns.Count);

            for (int i = 0; i < dataGridView1.Columns.Count; i++)
            {
                table.AddCell(new Phrase(dataGridView1.Columns[i].HeaderCell.Value.ToString(), fontParagraph));
            }
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                for (int j = 0; j < dataGridView1.Columns.Count; j++)
                {
                    table.AddCell(new Phrase(dataGridView1.Rows[i].Cells[j].Value.ToString(), fontParagraph));
                }
            }
            PdfPTable table2 = new PdfPTable(dataGridView1.Columns.Count);
            String s = labelSum.Text;
            List<string> words = new List<string>();
            string[] sum = { "Итого:", "" };
            words.AddRange(sum);
            String[] words1 = s.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            words.AddRange(words1);
            for (int j = 0; j < words.Count; j++)
            {
                table2.AddCell(new Phrase(words[j], fontParagraph));
            }
            using (FileStream stream = new FileStream(FileName, FileMode.Create))
            {
                iTextSharp.text.Document pdfDoc = new iTextSharp.text.Document(PageSize.A2, 10f, 10f, 10f, 0f);
                PdfWriter.GetInstance(pdfDoc, stream);
                pdfDoc.Open();
                pdfDoc.Add(paragraph);
                pdfDoc.Add(table);
                pdfDoc.Add(table2);
                pdfDoc.Close();
                stream.Close();
            }
            string mailAddress = textBoxEmail.Text;
            if (!string.IsNullOrEmpty(mailAddress))
            {
                if (Regex.IsMatch(mailAddress, @"^(?("")(""[^""]+?""@)|(([0-9a-z]((\.(?!\.))|[-
!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9az][-\w]*[0-9a-z]*\.)+[a-z0-9]{2,17}))$"))
                {
                    MessageBox.Show("Неверный формат для электронной почты", "Ошибка",
                   MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                else
                {
                    SendEmailForClients(mailAddress, "Отчеты:", "", FileName);
                }
            }
        }
        public void saveRAR(string FileName)
        {
            using (ZipFile zip = new ZipFile())
            {
                savePDF(@"D:\универ\3 курс\тип эис\отчеты\ReportPdf.pdf");
                saveDoc(@"D:\универ\3 курс\тип эис\отчеты\ReportDoc.doc");
                saveXls(@"D:\универ\3 курс\тип эис\отчеты\ReportXls.xls");
                zip.AddDirectory(@"D:\универ\3 курс\тип эис\отчеты\");
                zip.Save(FileName);
            }
        }

        private void buttonSaveZIP_Click(object sender, EventArgs e)
        {
            if (dateTimePicker1.Value.Date >= dateTimePicker2.Value.Date)
            {
                MessageBox.Show("Дата начала должна быть меньше даты окончания",
               "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = "rar|*.rar"
            };
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    saveRAR(sfd.FileName);
                    MessageBox.Show("Выполнено", "Успех", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK,
                   MessageBoxIcon.Error);
                }
            }
        }
        public void saveDoc(string FileName)
        {
            var winword = new Microsoft.Office.Interop.Word.Application();
            try
            {
                object missing = System.Reflection.Missing.Value;
                //создаем документ
                Microsoft.Office.Interop.Word.Document document =
                winword.Documents.Add(ref missing, ref missing, ref missing, ref
               missing);
                //получаем ссылку на параграф
                var paragraph = document.Paragraphs.Add(missing);
                var range = paragraph.Range;
                string title = "";
                if (comboBoxTypeOperation.SelectedIndex == 0)
                {
                    title = "Оборотно-сальдовая ведомость по счёту " + comboBoxAccount.Text + " \"" + comboBoxAccount.SelectedValue + "\" с " + Convert.ToString(dateTimePicker1.Text) + " по " + Convert.ToString(dateTimePicker2.Text) + "";
                }
                if (comboBoxTypeOperation.SelectedIndex == 1)
                {
                    title = "Ведомость взаиморасчёта с сотрудниками " + comboBoxSubdivision.Text + " " + comboBoxEmployee.Text + " с " + Convert.ToString(dateTimePicker1.Text) + " по " + Convert.ToString(dateTimePicker2.Text) + "";
                }
                if (comboBoxTypeOperation.SelectedIndex == 2)
                {
                    title = "Ведомость выплат за год " + dateTimePicker1.Value.ToString("yyyy") + "";
                }
                //задаем текст
                range.Text = title;
                //задаем настройки шрифта
                var font = range.Font;
                font.Size = 16;
                font.Name = "Times New Roman";
                font.Bold = 1;
                //задаем настройки абзаца
                var paragraphFormat = range.ParagraphFormat;
                paragraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                paragraphFormat.LineSpacingRule = WdLineSpacing.wdLineSpaceSingle;
                paragraphFormat.SpaceAfter = 10;
                paragraphFormat.SpaceBefore = 0;
                //добавляем абзац в документ
                range.InsertParagraphAfter();
                //создаем таблицу
                var paragraphTable = document.Paragraphs.Add(Type.Missing);
                var rangeTable = paragraphTable.Range;

                int count = 0;
                for (int i = 0; i < dataGridView1.Columns.Count; ++i)
                {
                    if (dataGridView1.Columns[i].Visible == true)
                    {
                        count++;
                    }
                }
                var table = document.Tables.Add(rangeTable, dataGridView1.Rows.Count + 1, count, ref
        missing, ref missing);
                font = table.Range.Font;
                font.Size = 14;
                font.Name = "Times New Roman";
                var paragraphTableFormat = table.Range.ParagraphFormat;
                paragraphTableFormat.LineSpacingRule = WdLineSpacing.wdLineSpaceSingle;
                paragraphTableFormat.SpaceAfter = 0;
                paragraphTableFormat.SpaceBefore = 0;
                count = 0;
                for (int i = 0; i < dataGridView1.Columns.Count; ++i)
                {
                    if (dataGridView1.Columns[i].Visible == true)
                    {
                        table.Cell(1, count + 1).Range.Text = dataGridView1.Columns[i].HeaderCell.Value.ToString();
                        count++;
                    }
                }
                for (int i = 0; i < dataGridView1.Rows.Count; ++i)
                {
                    count = 0;
                    for (int j = 0; j < dataGridView1.Columns.Count; ++j)
                    {
                        if (dataGridView1.Columns[j].Visible == true)
                        {
                            table.Cell(i + 2, count + 1).Range.Text = dataGridView1.Rows[i].Cells[j].Value.ToString();
                            count++;
                        }
                    }
                }

                String s = labelSum.Text;
                List<string> words = new List<string>();
                string[] sum = { "Итого:", "" };
                words.AddRange(sum);
                String[] words1 = s.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                words.AddRange(words1);
                count = 0;
                for (int j = 0; j < words.Count; j++)
                {
                    table.Cell(dataGridView1.Rows.Count + 1, count + 1).Range.Text = words[j];
                    count++;
                }
                //задаем границы таблицы
                table.Borders.InsideLineStyle = WdLineStyle.wdLineStyleInset;
                table.Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;
                //сохраняем
                object fileFormat = WdSaveFormat.wdFormatXMLDocument;
                document.SaveAs(FileName, ref fileFormat, ref missing,
                ref missing, ref missing, ref missing, ref missing,
                ref missing, ref missing, ref missing, ref missing,
                ref missing, ref missing, ref missing, ref missing,
                ref missing);
                document.Close(ref missing, ref missing, ref missing);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                winword.Quit();
            }
            string mailAddress = textBoxEmail.Text;
            if (!string.IsNullOrEmpty(mailAddress))
            {
                if (Regex.IsMatch(mailAddress, @"^(?("")(""[^""]+?""@)|(([0-9a-z]((\.(?!\.))|[-
!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9az][-\w]*[0-9a-z]*\.)+[a-z0-9]{2,17}))$"))
                {
                    MessageBox.Show("Неверный формат для электронной почты", "Ошибка",
                   MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                else
                {
                    SendEmailForClients(mailAddress, "Отчеты:", "", FileName);
                }
            }
        }
        private void buttonToWord_Click(object sender, EventArgs e)
        {
            if (dateTimePicker1.Value.Date >= dateTimePicker2.Value.Date)
            {
                MessageBox.Show("Дата начала должна быть меньше даты окончания",
               "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = "doc|*.doc"
            };
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    saveDoc(sfd.FileName);
                    MessageBox.Show("Выполнено", "Успех", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK,
                   MessageBoxIcon.Error);
                }
            }
        }
        private void buttonSaveXls_Click(object sender, EventArgs e)
        {
            if (dateTimePicker1.Value.Date >= dateTimePicker2.Value.Date)
            {
                MessageBox.Show("Дата начала должна быть меньше даты окончания",
               "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = "xls|*.xls|xlsx|*.xlsx"
            };
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    saveXls(sfd.FileName);
                    MessageBox.Show("Выполнено", "Успех", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK,
                   MessageBoxIcon.Error);
                }
            }
        }

        public void saveXls(string FileName)
        {
            var excel = new Microsoft.Office.Interop.Excel.Application();
            try
            {
                if (File.Exists(FileName))
                {
                    excel.Workbooks.Open(FileName, Type.Missing, Type.Missing,
                   Type.Missing,
                    Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                   Type.Missing,
                    Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                   Type.Missing,
                    Type.Missing);
                }
                else
                {
                    excel.SheetsInNewWorkbook = 1;
                    excel.Workbooks.Add(Type.Missing);
                    excel.Workbooks[1].SaveAs(FileName, XlFileFormat.xlExcel8,
                    Type.Missing,
                     Type.Missing, false, false, XlSaveAsAccessMode.xlNoChange,
                    Type.Missing,
                     Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                }
                Sheets excelsheets = excel.Workbooks[1].Worksheets;

                var excelworksheet = (Worksheet)excelsheets.get_Item(1);
                excelworksheet.Cells.Clear();
                Microsoft.Office.Interop.Excel.Range excelcells = excelworksheet.get_Range("A1", "H1");
                excelcells.Merge(Type.Missing);
                excelcells.Font.Bold = true;
                string title = "";
                if (comboBoxTypeOperation.SelectedIndex == 0)
                {
                    title = "Оборотно-сальдовая ведомость по счёту " + comboBoxAccount.Text + " \"" + comboBoxAccount.SelectedValue + "\" с " + Convert.ToString(dateTimePicker1.Text) + " по " + Convert.ToString(dateTimePicker2.Text) + "";
                }
                if (comboBoxTypeOperation.SelectedIndex == 1)
                {
                    title = "Ведомость взаиморасчёта с сотрудниками " + comboBoxSubdivision.Text + " " + comboBoxEmployee.Text + " с " + Convert.ToString(dateTimePicker1.Text) + " по " + Convert.ToString(dateTimePicker2.Text) + "";
                }
                if (comboBoxTypeOperation.SelectedIndex == 2)
                {
                    title = "Ведомость выплат за год " + dateTimePicker1.Value.ToString("yyyy") + "";
                }
                excelcells.Value2 = title;
                excelcells.RowHeight = 40;
                excelcells.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                excelcells.VerticalAlignment = Microsoft.Office.Interop.Excel.XlVAlign.xlVAlignCenter;
                excelcells.Font.Name = "Times New Roman";
                excelcells.Font.Size = 14;

                for (int j = 0; j < dataGridView1.Columns.Count; j++)
                {
                    excelcells = excelworksheet.get_Range("A3", "A3");
                    excelcells = excelcells.get_Offset(0, j);
                    excelcells.ColumnWidth = 15;
                    excelcells.Value2 = dataGridView1.Columns[j].HeaderCell.Value.ToString();
                    excelcells.Font.Bold = true;
                }

                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    for (int j = 0; j < dataGridView1.Columns.Count; j++)
                    {
                        excelcells = excelworksheet.get_Range("A4", "A4");
                        excelcells = excelcells.get_Offset(i, j);
                        excelcells.ColumnWidth = 15;
                        excelcells.Value2 = dataGridView1.Rows[i].Cells[j].Value.ToString();
                    }
                }
                String s = labelSum.Text;
                List<string> words = new List<string>();
                string[] sum = { "Итого:", "" };
                words.AddRange(sum);
                String[] words1 = s.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                words.AddRange(words1);
                for (int j = 0; j < words.Count; j++)
                {
                    excelcells = excelworksheet.get_Range("A5", "A5");
                    excelcells = excelcells.get_Offset(dataGridView1.Rows.Count + 1, j);
                    excelcells.ColumnWidth = 25;
                    excelcells.Value2 = words[j].ToString();
                }
                excel.Workbooks[1].Save();
            }
            catch (Exception)
            {
            }
            finally
            {
                excel.Quit();
            }
            string mailAddress = textBoxEmail.Text;
            if (!string.IsNullOrEmpty(mailAddress))
            {
                if (Regex.IsMatch(mailAddress, @"^(?("")(""[^""]+?""@)|(([0-9a-z]((\.(?!\.))|[-
!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9az][-\w]*[0-9a-z]*\.)+[a-z0-9]{2,17}))$"))
                {
                    MessageBox.Show("Неверный формат для электронной почты", "Ошибка",
                   MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                else
                {
                    SendEmailForClients(mailAddress, "Отчеты:", "", FileName);
                }
            }
        }
        private void SendEmailForClients(string mailAddress, string subject, string text, string attachmentPath)
        {

            System.Net.Mail.MailMessage m = new System.Net.Mail.MailMessage();
            SmtpClient smtpClient = null;
            try
            {
                m.From = new MailAddress(ConfigurationManager.AppSettings["MailLogin"]);
                m.To.Add(new MailAddress(mailAddress));
                m.Subject = subject;
                m.Body = text;
                m.SubjectEncoding = System.Text.Encoding.UTF8;
                m.BodyEncoding = System.Text.Encoding.UTF8;
                m.Attachments.Add(new Attachment(attachmentPath));
                smtpClient = new SmtpClient("smtp.gmail.com", 587);
                smtpClient.UseDefaultCredentials = false;
                smtpClient.EnableSsl = true;
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpClient.Credentials = new NetworkCredential(
                    ConfigurationManager.AppSettings["MailLogin"],
                    ConfigurationManager.AppSettings["MailPassword"]
                    );
                smtpClient.Send(m);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                m = null;
                smtpClient = null;
            }
        }
    }
}

