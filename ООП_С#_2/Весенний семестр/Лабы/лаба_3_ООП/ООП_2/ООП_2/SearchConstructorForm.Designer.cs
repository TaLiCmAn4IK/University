namespace ООП_2
{
    partial class SearchConstructorForm
    {
        private System.ComponentModel.IContainer components = null;

        // Объявляем все элементы управления
        private System.Windows.Forms.ComboBox comboCategory;
        private System.Windows.Forms.ComboBox comboField;
        private System.Windows.Forms.ComboBox comboSearchType;
        private System.Windows.Forms.TextBox textSearchValue;
        private System.Windows.Forms.TextBox textRangeValue;
        private System.Windows.Forms.Label labelRangeTo;
        private System.Windows.Forms.Button buttonSearch;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.GroupBox groupBoxSearch;
        private System.Windows.Forms.GroupBox groupBoxOptions;
        private System.Windows.Forms.Label labelCategory;
        private System.Windows.Forms.Label labelField;
        private System.Windows.Forms.Label labelSearchType;
        private System.Windows.Forms.Label labelSearchValue;
        private System.Windows.Forms.Label labelRange;

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
            this.comboCategory = new System.Windows.Forms.ComboBox();
            this.comboField = new System.Windows.Forms.ComboBox();
            this.comboSearchType = new System.Windows.Forms.ComboBox();
            this.textSearchValue = new System.Windows.Forms.TextBox();
            this.textRangeValue = new System.Windows.Forms.TextBox();
            this.labelRangeTo = new System.Windows.Forms.Label();
            this.buttonSearch = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.groupBoxSearch = new System.Windows.Forms.GroupBox();
            this.groupBoxOptions = new System.Windows.Forms.GroupBox();
            this.labelCategory = new System.Windows.Forms.Label();
            this.labelField = new System.Windows.Forms.Label();
            this.labelSearchType = new System.Windows.Forms.Label();
            this.labelSearchValue = new System.Windows.Forms.Label();
            this.labelRange = new System.Windows.Forms.Label();
            this.groupBoxSearch.SuspendLayout();
            this.groupBoxOptions.SuspendLayout();
            this.SuspendLayout();

            // Form
            this.Text = "Конструктор поисковых запросов";
            this.ClientSize = new System.Drawing.Size(500, 400);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            // groupBoxSearch
            this.groupBoxSearch.Text = "Параметры поиска";
            this.groupBoxSearch.Location = new System.Drawing.Point(12, 12);
            this.groupBoxSearch.Size = new System.Drawing.Size(476, 300);
            this.groupBoxSearch.TabIndex = 0;

            // labelCategory
            this.labelCategory.Text = "Категория:";
            this.labelCategory.Location = new System.Drawing.Point(20, 30);
            this.labelCategory.Size = new System.Drawing.Size(100, 23);
            this.labelCategory.TabIndex = 0;

            // comboCategory
            this.comboCategory.Location = new System.Drawing.Point(130, 27);
            this.comboCategory.Size = new System.Drawing.Size(200, 23);
            this.comboCategory.TabIndex = 1;
            this.comboCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;

            // labelField
            this.labelField.Text = "Поле поиска:";
            this.labelField.Location = new System.Drawing.Point(20, 60);
            this.labelField.Size = new System.Drawing.Size(100, 23);
            this.labelField.TabIndex = 2;

            // comboField
            this.comboField.Location = new System.Drawing.Point(130, 57);
            this.comboField.Size = new System.Drawing.Size(200, 23);
            this.comboField.TabIndex = 3;
            this.comboField.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;

            // labelSearchType
            this.labelSearchType.Text = "Тип поиска:";
            this.labelSearchType.Location = new System.Drawing.Point(20, 90);
            this.labelSearchType.Size = new System.Drawing.Size(100, 23);
            this.labelSearchType.TabIndex = 4;

            // comboSearchType
            this.comboSearchType.Location = new System.Drawing.Point(130, 87);
            this.comboSearchType.Size = new System.Drawing.Size(200, 23);
            this.comboSearchType.TabIndex = 5;
            this.comboSearchType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboSearchType.SelectedIndexChanged += new System.EventHandler(this.comboSearchType_SelectedIndexChanged);

            // labelSearchValue
            this.labelSearchValue.Text = "Значение:";
            this.labelSearchValue.Location = new System.Drawing.Point(20, 120);
            this.labelSearchValue.Size = new System.Drawing.Size(100, 23);
            this.labelSearchValue.TabIndex = 6;

            // textSearchValue
            this.textSearchValue.Location = new System.Drawing.Point(130, 117);
            this.textSearchValue.Size = new System.Drawing.Size(200, 23);
            this.textSearchValue.TabIndex = 7;

            // labelRange
            this.labelRange.Text = "Диапазон до:";
            this.labelRange.Location = new System.Drawing.Point(20, 150);
            this.labelRange.Size = new System.Drawing.Size(100, 23);
            this.labelRange.TabIndex = 8;

            // textRangeValue
            this.textRangeValue.Location = new System.Drawing.Point(130, 147);
            this.textRangeValue.Size = new System.Drawing.Size(200, 23);
            this.textRangeValue.TabIndex = 9;

            // labelRangeTo
            this.labelRangeTo.Text = "(для диапазона)";
            this.labelRangeTo.Location = new System.Drawing.Point(340, 150);
            this.labelRangeTo.Size = new System.Drawing.Size(100, 23);
            this.labelRangeTo.TabIndex = 10;

            // groupBoxOptions
            this.groupBoxOptions.Text = "Действия";
            this.groupBoxOptions.Location = new System.Drawing.Point(12, 318);
            this.groupBoxOptions.Size = new System.Drawing.Size(476, 70);
            this.groupBoxOptions.TabIndex = 1;

            // buttonSearch
            this.buttonSearch.Text = "Поиск";
            this.buttonSearch.Location = new System.Drawing.Point(150, 25);
            this.buttonSearch.Size = new System.Drawing.Size(100, 30);
            this.buttonSearch.TabIndex = 0;
            this.buttonSearch.UseVisualStyleBackColor = true;
            this.buttonSearch.Click += new System.EventHandler(this.buttonSearch_Click);

            // buttonCancel
            this.buttonCancel.Text = "Отмена";
            this.buttonCancel.Location = new System.Drawing.Point(260, 25);
            this.buttonCancel.Size = new System.Drawing.Size(100, 30);
            this.buttonCancel.TabIndex = 1;
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);

            // Добавляем элементы в groupBoxSearch
            this.groupBoxSearch.Controls.Add(this.labelCategory);
            this.groupBoxSearch.Controls.Add(this.comboCategory);
            this.groupBoxSearch.Controls.Add(this.labelField);
            this.groupBoxSearch.Controls.Add(this.comboField);
            this.groupBoxSearch.Controls.Add(this.labelSearchType);
            this.groupBoxSearch.Controls.Add(this.comboSearchType);
            this.groupBoxSearch.Controls.Add(this.labelSearchValue);
            this.groupBoxSearch.Controls.Add(this.textSearchValue);
            this.groupBoxSearch.Controls.Add(this.labelRange);
            this.groupBoxSearch.Controls.Add(this.textRangeValue);
            this.groupBoxSearch.Controls.Add(this.labelRangeTo);

            // Добавляем элементы в groupBoxOptions
            this.groupBoxOptions.Controls.Add(this.buttonSearch);
            this.groupBoxOptions.Controls.Add(this.buttonCancel);

            // Добавляем groupBox на форму
            this.Controls.Add(this.groupBoxSearch);
            this.Controls.Add(this.groupBoxOptions);

            this.groupBoxSearch.ResumeLayout(false);
            this.groupBoxSearch.PerformLayout();
            this.groupBoxOptions.ResumeLayout(false);
            this.ResumeLayout(false);
        }
    }
}