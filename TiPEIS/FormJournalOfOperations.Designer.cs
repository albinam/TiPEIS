namespace TiPEIS
{
    partial class FormJournalOfOperations
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.buttonDelete = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.buttonCreate = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.buttonEntries = new System.Windows.Forms.Button();
            this.buttonEntriesOperation = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonDelete
            // 
            this.buttonDelete.Location = new System.Drawing.Point(749, 72);
            this.buttonDelete.Margin = new System.Windows.Forms.Padding(2);
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Size = new System.Drawing.Size(115, 24);
            this.buttonDelete.TabIndex = 12;
            this.buttonDelete.Text = "Удалить";
            this.buttonDelete.UseVisualStyleBackColor = true;
            this.buttonDelete.Click += new System.EventHandler(this.ButtonDelete_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(749, 43);
            this.button1.Margin = new System.Windows.Forms.Padding(2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(115, 25);
            this.button1.TabIndex = 11;
            this.button1.Text = "Редактировать";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.buttonEdit_Click);
            // 
            // buttonCreate
            // 
            this.buttonCreate.Location = new System.Drawing.Point(749, 14);
            this.buttonCreate.Margin = new System.Windows.Forms.Padding(2);
            this.buttonCreate.Name = "buttonCreate";
            this.buttonCreate.Size = new System.Drawing.Size(115, 25);
            this.buttonCreate.TabIndex = 10;
            this.buttonCreate.Text = "Создать";
            this.buttonCreate.UseVisualStyleBackColor = true;
            this.buttonCreate.Click += new System.EventHandler(this.buttonCreate_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(13, 14);
            this.dataGridView1.Margin = new System.Windows.Forms.Padding(2);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersWidth = 51;
            this.dataGridView1.Size = new System.Drawing.Size(732, 375);
            this.dataGridView1.TabIndex = 9;
            // 
            // buttonEntries
            // 
            this.buttonEntries.Location = new System.Drawing.Point(755, 109);
            this.buttonEntries.Name = "buttonEntries";
            this.buttonEntries.Size = new System.Drawing.Size(108, 38);
            this.buttonEntries.TabIndex = 13;
            this.buttonEntries.Text = "Посмотреть все проводки";
            this.buttonEntries.UseVisualStyleBackColor = true;
            this.buttonEntries.Click += new System.EventHandler(this.buttonEntries_Click);
            // 
            // buttonEntriesOperation
            // 
            this.buttonEntriesOperation.Location = new System.Drawing.Point(755, 162);
            this.buttonEntriesOperation.Name = "buttonEntriesOperation";
            this.buttonEntriesOperation.Size = new System.Drawing.Size(109, 54);
            this.buttonEntriesOperation.TabIndex = 14;
            this.buttonEntriesOperation.Text = "Посмотреть проводки по операции";
            this.buttonEntriesOperation.UseVisualStyleBackColor = true;
            this.buttonEntriesOperation.Click += new System.EventHandler(this.buttonEntriesOperation_Click);
            // 
            // FormJournalOfOperations
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(875, 450);
            this.Controls.Add(this.buttonEntriesOperation);
            this.Controls.Add(this.buttonEntries);
            this.Controls.Add(this.buttonDelete);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.buttonCreate);
            this.Controls.Add(this.dataGridView1);
            this.Name = "FormJournalOfOperations";
            this.Text = "Журнал операций";
            this.Load += new System.EventHandler(this.FormJournalOfOperations_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonDelete;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button buttonCreate;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button buttonEntries;
        private System.Windows.Forms.Button buttonEntriesOperation;
    }
}