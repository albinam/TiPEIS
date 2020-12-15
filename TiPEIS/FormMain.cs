using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TiPEIS
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
        }

        private void планСчетовToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormChartOfAccounts newForm = new FormChartOfAccounts();
            newForm.Show();
        }

        private void подразделенияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormSubdivision newForm = new FormSubdivision();
            newForm.Show();
        }

        private void сотрудникиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormEmployees newForm = new FormEmployees();
            newForm.Show();
        }

        private void видРасчетаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormTypeOfCalculation newForm = new FormTypeOfCalculation();
            newForm.Show();
        }

        private void журналПроводокToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormJournalOfOperations newForm = new FormJournalOfOperations();
            newForm.Show();
        }

        private void отчетToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormReport newForm = new FormReport();
            newForm.Show();
        }

    }
}
