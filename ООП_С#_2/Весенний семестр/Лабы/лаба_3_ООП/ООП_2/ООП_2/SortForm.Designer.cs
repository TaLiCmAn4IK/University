namespace ООП_2
{
    partial class SortForm
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.ComboBox comboSort1;
        private System.Windows.Forms.ComboBox comboSort2;
        private System.Windows.Forms.ListBox listBoxResults;
        private System.Windows.Forms.Button buttonSort;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.GroupBox groupBoxCriteria;
        private System.Windows.Forms.GroupBox groupBoxResults;
        private System.Windows.Forms.GroupBox groupBoxActions;
        private System.Windows.Forms.Label labelSort1;
        private System.Windows.Forms.Label labelSort2;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.comboSort1 = new System.Windows.Forms.ComboBox();
            this.comboSort2 = new System.Windows.Forms.ComboBox();
            this.listBoxResults = new System.Windows.Forms.ListBox();
            this.buttonSort = new System.Windows.Forms.Button();
            this.buttonSave = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.groupBoxCriteria = new System.Windows.Forms.GroupBox();
            this.groupBoxResults = new System.Windows.Forms.GroupBox();
            this.groupBoxActions = new System.Windows.Forms.GroupBox();
            this.labelSort1 = new System.Windows.Forms.Label();
            this.labelSort2 = new System.Windows.Forms.Label();
            this.groupBoxCriteria.SuspendLayout();
            this.groupBoxResults.SuspendLayout();
            this.groupBoxActions.SuspendLayout();
            this.SuspendLayout();

            // Form
            this.Text = "Сортировка авторов";
            this.ClientSize = new System.Drawing.Size(500, 450);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;

            // groupBoxCriteria
            this.groupBoxCriteria.Text = "Критерии сортировки";
            this.groupBoxCriteria.Location = new System.Drawing.Point(12, 12);
            this.groupBoxCriteria.Size = new System.Drawing.Size(476, 100);
            this.groupBoxCriteria.TabIndex = 0;

            // labelSort1
            this.labelSort1.Text = "Первый критерий:";
            this.labelSort1.Location = new System.Drawing.Point(10, 30);
            this.labelSort1.Size = new System.Drawing.Size(120, 23);
            this.labelSort1.TabIndex = 0;

            // comboSort1
            this.comboSort1.Location = new System.Drawing.Point(140, 27);
            this.comboSort1.Size = new System.Drawing.Size(200, 23);
            this.comboSort1.TabIndex = 1;
            this.comboSort1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;

            // labelSort2
            this.labelSort2.Text = "Второй критерий:";
            this.labelSort2.Location = new System.Drawing.Point(10, 60);
            this.labelSort2.Size = new System.Drawing.Size(120, 23);
            this.labelSort2.TabIndex = 2;

            // comboSort2
            this.comboSort2.Location = new System.Drawing.Point(140, 57);
            this.comboSort2.Size = new System.Drawing.Size(200, 23);
            this.comboSort2.TabIndex = 3;
            this.comboSort2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;

            // groupBoxResults
            this.groupBoxResults.Text = "Результаты";
            this.groupBoxResults.Location = new System.Drawing.Point(12, 118);
            this.groupBoxResults.Size = new System.Drawing.Size(476, 200);
            this.groupBoxResults.TabIndex = 1;

            // listBoxResults
            this.listBoxResults.Location = new System.Drawing.Point(6, 22);
            this.listBoxResults.Size = new System.Drawing.Size(464, 160);
            this.listBoxResults.TabIndex = 0;

            // groupBoxActions
            this.groupBoxActions.Text = "Действия";
            this.groupBoxActions.Location = new System.Drawing.Point(12, 324);
            this.groupBoxActions.Size = new System.Drawing.Size(476, 80);
            this.groupBoxActions.TabIndex = 2;

            // buttonSort
            this.buttonSort.Text = "Сортировать";
            this.buttonSort.Location = new System.Drawing.Point(100, 25);
            this.buttonSort.Size = new System.Drawing.Size(100, 30);
            this.buttonSort.TabIndex = 0;
            this.buttonSort.Click += new System.EventHandler(this.buttonSort_Click);

            // buttonSave
            this.buttonSave.Text = "Сохранить";
            this.buttonSave.Location = new System.Drawing.Point(210, 25);
            this.buttonSave.Size = new System.Drawing.Size(100, 30);
            this.buttonSave.TabIndex = 1;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);

            // buttonCancel
            this.buttonCancel.Text = "Отмена";
            this.buttonCancel.Location = new System.Drawing.Point(320, 25);
            this.buttonCancel.Size = new System.Drawing.Size(100, 30);
            this.buttonCancel.TabIndex = 2;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);

            // Добавляем элементы
            this.groupBoxCriteria.Controls.Add(this.labelSort1);
            this.groupBoxCriteria.Controls.Add(this.comboSort1);
            this.groupBoxCriteria.Controls.Add(this.labelSort2);
            this.groupBoxCriteria.Controls.Add(this.comboSort2);

            this.groupBoxResults.Controls.Add(this.listBoxResults);

            this.groupBoxActions.Controls.Add(this.buttonSort);
            this.groupBoxActions.Controls.Add(this.buttonSave);
            this.groupBoxActions.Controls.Add(this.buttonCancel);

            this.Controls.Add(this.groupBoxCriteria);
            this.Controls.Add(this.groupBoxResults);
            this.Controls.Add(this.groupBoxActions);

            this.groupBoxCriteria.ResumeLayout(false);
            this.groupBoxResults.ResumeLayout(false);
            this.groupBoxActions.ResumeLayout(false);
            this.ResumeLayout(false);
        }
    }
}