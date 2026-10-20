namespace ООП_2
{
    partial class FormLibrary
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            labelTitle = new Label();
            groupBoxBook = new GroupBox();
            numericPrice = new NumericUpDown();
            labelPrice = new Label();
            textPages = new TextBox();
            textFileSize = new TextBox();
            textTitle = new TextBox();
            comboGenre = new ComboBox();
            labelFileSize = new Label();
            labelPages = new Label();
            labelTitleBook = new Label();
            labelGenre = new Label();
            groupBoxAuthors = new GroupBox();
            groupBoxFees = new GroupBox();
            numericPercent = new NumericUpDown();
            labelTotalFee = new Label();
            buttonCalculate = new Button();
            labelPercent = new Label();
            textFeePerAuthor = new TextBox();
            textTotalFee = new TextBox();
            labelFeePerAuthor = new Label();
            groupBoxCurrentAuthor = new GroupBox();
            textCurrentAuthorID = new TextBox();
            textCurrentAuthorCountry = new TextBox();
            textCurrentAuthorName = new TextBox();
            labelCurrentAuthorID = new Label();
            labelCurrentAuthorCountry = new Label();
            labelCurrentAuthorName = new Label();
            listBoxAuthors = new ListBox();
            labelAuthorsList = new Label();
            buttonAddAuthor = new Button();
            buttonEditAuthor = new Button();
            buttonRemoveAuthor = new Button();
            panelBottom = new Panel();
            buttonLoad = new Button();
            buttonSave = new Button();
            buttonExit = new Button();
            errorProvider1 = new ErrorProvider(components);
            toolTip1 = new ToolTip(components);
            saveFileDialog1 = new SaveFileDialog();
            openFileDialog1 = new OpenFileDialog();
            groupBoxPublisher = new GroupBox();
            radioPublisherState = new RadioButton();
            radioPublisherPrivate = new RadioButton();
            numericPublisherYear = new NumericUpDown();
            comboPublisherCountry = new ComboBox();
            labelPublisherType = new Label();
            labelPublisherYear = new Label();
            textPublisherCity = new TextBox();
            labelPublisherCity = new Label();
            labelPublisherCountry = new Label();
            labelPublisherName = new Label();
            textPublisherName = new TextBox();
            groupBoxBook.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numericPrice).BeginInit();
            groupBoxAuthors.SuspendLayout();
            groupBoxFees.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numericPercent).BeginInit();
            groupBoxCurrentAuthor.SuspendLayout();
            panelBottom.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)errorProvider1).BeginInit();
            groupBoxPublisher.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numericPublisherYear).BeginInit();
            SuspendLayout();
            // 
            // labelTitle
            // 
            labelTitle.AutoSize = true;
            labelTitle.Font = new Font("Microsoft Sans Serif", 24F, FontStyle.Regular, GraphicsUnit.Point, 204);
            labelTitle.Location = new Point(350, 9);
            labelTitle.Name = "labelTitle";
            labelTitle.Size = new Size(494, 46);
            labelTitle.TabIndex = 0;
            labelTitle.Text = "Электронная библиотека";
            labelTitle.TextAlign = ContentAlignment.TopCenter;
            // 
            // groupBoxBook
            // 
            groupBoxBook.Controls.Add(numericPrice);
            groupBoxBook.Controls.Add(labelPrice);
            groupBoxBook.Controls.Add(textPages);
            groupBoxBook.Controls.Add(textFileSize);
            groupBoxBook.Controls.Add(textTitle);
            groupBoxBook.Controls.Add(comboGenre);
            groupBoxBook.Controls.Add(labelFileSize);
            groupBoxBook.Controls.Add(labelPages);
            groupBoxBook.Controls.Add(labelTitleBook);
            groupBoxBook.Controls.Add(labelGenre);
            groupBoxBook.Font = new Font("Microsoft Sans Serif", 13.8F, FontStyle.Regular, GraphicsUnit.Point, 204);
            groupBoxBook.Location = new Point(12, 78);
            groupBoxBook.Name = "groupBoxBook";
            groupBoxBook.Size = new Size(500, 340);
            groupBoxBook.TabIndex = 1;
            groupBoxBook.TabStop = false;
            groupBoxBook.Text = "Файл книги";
            // 
            // numericPrice
            // 
            numericPrice.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            numericPrice.Location = new Point(200, 200);
            numericPrice.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
            numericPrice.Name = "numericPrice";
            numericPrice.Size = new Size(150, 24);
            numericPrice.TabIndex = 4;
            // 
            // labelPrice
            // 
            labelPrice.AutoSize = true;
            labelPrice.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            labelPrice.Location = new Point(20, 202);
            labelPrice.Name = "labelPrice";
            labelPrice.Size = new Size(146, 18);
            labelPrice.TabIndex = 10;
            labelPrice.Text = "Стоимость доступа";
            // 
            // textPages
            // 
            textPages.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            textPages.Location = new Point(200, 155);
            textPages.Name = "textPages";
            textPages.Size = new Size(150, 24);
            textPages.TabIndex = 1;
            textPages.KeyPress += textPages_KeyPress;
            // 
            // textFileSize
            // 
            textFileSize.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            textFileSize.Location = new Point(200, 114);
            textFileSize.Name = "textFileSize";
            textFileSize.Size = new Size(150, 24);
            textFileSize.TabIndex = 5;
            textFileSize.KeyPress += textFileSize_KeyPress;
            // 
            // textTitle
            // 
            textTitle.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            textTitle.Location = new Point(200, 78);
            textTitle.Name = "textTitle";
            textTitle.Size = new Size(150, 24);
            textTitle.TabIndex = 4;
            // 
            // comboGenre
            // 
            comboGenre.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            comboGenre.FormattingEnabled = true;
            comboGenre.Location = new Point(200, 39);
            comboGenre.Name = "comboGenre";
            comboGenre.Size = new Size(150, 26);
            comboGenre.TabIndex = 3;
            // 
            // labelFileSize
            // 
            labelFileSize.AutoSize = true;
            labelFileSize.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            labelFileSize.Location = new Point(20, 117);
            labelFileSize.Name = "labelFileSize";
            labelFileSize.Size = new Size(111, 18);
            labelFileSize.TabIndex = 2;
            labelFileSize.Text = "Размер файла";
            // 
            // labelPages
            // 
            labelPages.AutoSize = true;
            labelPages.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            labelPages.Location = new Point(20, 158);
            labelPages.Name = "labelPages";
            labelPages.Size = new Size(117, 18);
            labelPages.TabIndex = 0;
            labelPages.Text = "Кол-во страниц";
            // 
            // labelTitleBook
            // 
            labelTitleBook.AutoSize = true;
            labelTitleBook.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            labelTitleBook.Location = new Point(20, 81);
            labelTitleBook.Name = "labelTitleBook";
            labelTitleBook.Size = new Size(75, 18);
            labelTitleBook.TabIndex = 1;
            labelTitleBook.Text = "Название";
            // 
            // labelGenre
            // 
            labelGenre.AutoSize = true;
            labelGenre.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            labelGenre.Location = new Point(20, 42);
            labelGenre.Name = "labelGenre";
            labelGenre.Size = new Size(46, 18);
            labelGenre.TabIndex = 0;
            labelGenre.Text = "Жанр";
            // 
            // groupBoxAuthors
            // 
            groupBoxAuthors.Controls.Add(groupBoxFees);
            groupBoxAuthors.Controls.Add(groupBoxCurrentAuthor);
            groupBoxAuthors.Controls.Add(listBoxAuthors);
            groupBoxAuthors.Controls.Add(labelAuthorsList);
            groupBoxAuthors.Controls.Add(buttonAddAuthor);
            groupBoxAuthors.Controls.Add(buttonEditAuthor);
            groupBoxAuthors.Controls.Add(buttonRemoveAuthor);
            groupBoxAuthors.Font = new Font("Microsoft Sans Serif", 13.8F);
            groupBoxAuthors.Location = new Point(530, 78);
            groupBoxAuthors.Name = "groupBoxAuthors";
            groupBoxAuthors.Size = new Size(550, 606);
            groupBoxAuthors.TabIndex = 2;
            groupBoxAuthors.TabStop = false;
            groupBoxAuthors.Text = "Авторы книги";
            // 
            // groupBoxFees
            // 
            groupBoxFees.Controls.Add(numericPercent);
            groupBoxFees.Controls.Add(labelTotalFee);
            groupBoxFees.Controls.Add(buttonCalculate);
            groupBoxFees.Controls.Add(labelPercent);
            groupBoxFees.Controls.Add(textFeePerAuthor);
            groupBoxFees.Controls.Add(textTotalFee);
            groupBoxFees.Controls.Add(labelFeePerAuthor);
            groupBoxFees.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            groupBoxFees.Location = new Point(20, 384);
            groupBoxFees.Name = "groupBoxFees";
            groupBoxFees.Size = new Size(510, 200);
            groupBoxFees.TabIndex = 23;
            groupBoxFees.TabStop = false;
            groupBoxFees.Text = "Отчисления авторам";
            // 
            // numericPercent
            // 
            numericPercent.Location = new Point(200, 40);
            numericPercent.Name = "numericPercent";
            numericPercent.Size = new Size(150, 24);
            numericPercent.TabIndex = 13;
            // 
            // labelTotalFee
            // 
            labelTotalFee.AutoSize = true;
            labelTotalFee.Location = new Point(15, 82);
            labelTotalFee.Name = "labelTotalFee";
            labelTotalFee.Size = new Size(112, 18);
            labelTotalFee.TabIndex = 10;
            labelTotalFee.Text = "Всего авторам";
            // 
            // buttonCalculate
            // 
            buttonCalculate.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            buttonCalculate.Location = new Point(380, 160);
            buttonCalculate.Name = "buttonCalculate";
            buttonCalculate.Size = new Size(120, 30);
            buttonCalculate.TabIndex = 12;
            buttonCalculate.Text = "Рассчитать";
            buttonCalculate.UseVisualStyleBackColor = true;
            buttonCalculate.Click += buttonCalculate_Click;
            // 
            // labelPercent
            // 
            labelPercent.AutoSize = true;
            labelPercent.Location = new Point(15, 42);
            labelPercent.Name = "labelPercent";
            labelPercent.Size = new Size(152, 18);
            labelPercent.TabIndex = 0;
            labelPercent.Text = "Процент отчисления";
            // 
            // textFeePerAuthor
            // 
            textFeePerAuthor.BackColor = SystemColors.Window;
            textFeePerAuthor.Location = new Point(200, 119);
            textFeePerAuthor.Name = "textFeePerAuthor";
            textFeePerAuthor.ReadOnly = true;
            textFeePerAuthor.Size = new Size(150, 24);
            textFeePerAuthor.TabIndex = 15;
            // 
            // textTotalFee
            // 
            textTotalFee.BackColor = SystemColors.Window;
            textTotalFee.Location = new Point(200, 79);
            textTotalFee.Name = "textTotalFee";
            textTotalFee.ReadOnly = true;
            textTotalFee.Size = new Size(150, 24);
            textTotalFee.TabIndex = 14;
            // 
            // labelFeePerAuthor
            // 
            labelFeePerAuthor.AutoSize = true;
            labelFeePerAuthor.Location = new Point(15, 122);
            labelFeePerAuthor.Name = "labelFeePerAuthor";
            labelFeePerAuthor.Size = new Size(133, 18);
            labelFeePerAuthor.TabIndex = 11;
            labelFeePerAuthor.Text = "На одного автора";
            // 
            // groupBoxCurrentAuthor
            // 
            groupBoxCurrentAuthor.Controls.Add(textCurrentAuthorID);
            groupBoxCurrentAuthor.Controls.Add(textCurrentAuthorCountry);
            groupBoxCurrentAuthor.Controls.Add(textCurrentAuthorName);
            groupBoxCurrentAuthor.Controls.Add(labelCurrentAuthorID);
            groupBoxCurrentAuthor.Controls.Add(labelCurrentAuthorCountry);
            groupBoxCurrentAuthor.Controls.Add(labelCurrentAuthorName);
            groupBoxCurrentAuthor.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            groupBoxCurrentAuthor.Location = new Point(20, 240);
            groupBoxCurrentAuthor.Name = "groupBoxCurrentAuthor";
            groupBoxCurrentAuthor.Size = new Size(510, 138);
            groupBoxCurrentAuthor.TabIndex = 22;
            groupBoxCurrentAuthor.TabStop = false;
            groupBoxCurrentAuthor.Text = "Текущий автор";
            // 
            // textCurrentAuthorID
            // 
            textCurrentAuthorID.BackColor = SystemColors.Window;
            textCurrentAuthorID.Location = new Point(200, 100);
            textCurrentAuthorID.Name = "textCurrentAuthorID";
            textCurrentAuthorID.ReadOnly = true;
            textCurrentAuthorID.Size = new Size(200, 24);
            textCurrentAuthorID.TabIndex = 23;
            // 
            // textCurrentAuthorCountry
            // 
            textCurrentAuthorCountry.BackColor = SystemColors.Window;
            textCurrentAuthorCountry.Location = new Point(200, 65);
            textCurrentAuthorCountry.Name = "textCurrentAuthorCountry";
            textCurrentAuthorCountry.ReadOnly = true;
            textCurrentAuthorCountry.Size = new Size(200, 24);
            textCurrentAuthorCountry.TabIndex = 22;
            // 
            // textCurrentAuthorName
            // 
            textCurrentAuthorName.BackColor = SystemColors.Window;
            textCurrentAuthorName.Location = new Point(200, 30);
            textCurrentAuthorName.Name = "textCurrentAuthorName";
            textCurrentAuthorName.ReadOnly = true;
            textCurrentAuthorName.Size = new Size(200, 24);
            textCurrentAuthorName.TabIndex = 21;
            // 
            // labelCurrentAuthorID
            // 
            labelCurrentAuthorID.AutoSize = true;
            labelCurrentAuthorID.Location = new Point(15, 105);
            labelCurrentAuthorID.Name = "labelCurrentAuthorID";
            labelCurrentAuthorID.Size = new Size(74, 18);
            labelCurrentAuthorID.TabIndex = 19;
            labelCurrentAuthorID.Text = "ID автора";
            // 
            // labelCurrentAuthorCountry
            // 
            labelCurrentAuthorCountry.AutoSize = true;
            labelCurrentAuthorCountry.Location = new Point(15, 70);
            labelCurrentAuthorCountry.Name = "labelCurrentAuthorCountry";
            labelCurrentAuthorCountry.Size = new Size(110, 18);
            labelCurrentAuthorCountry.TabIndex = 16;
            labelCurrentAuthorCountry.Text = "Страна автора";
            // 
            // labelCurrentAuthorName
            // 
            labelCurrentAuthorName.AutoSize = true;
            labelCurrentAuthorName.Location = new Point(15, 35);
            labelCurrentAuthorName.Name = "labelCurrentAuthorName";
            labelCurrentAuthorName.Size = new Size(44, 18);
            labelCurrentAuthorName.TabIndex = 20;
            labelCurrentAuthorName.Text = "ФИО";
            // 
            // listBoxAuthors
            // 
            listBoxAuthors.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            listBoxAuthors.FormattingEnabled = true;
            listBoxAuthors.ItemHeight = 18;
            listBoxAuthors.Location = new Point(20, 60);
            listBoxAuthors.Name = "listBoxAuthors";
            listBoxAuthors.Size = new Size(510, 130);
            listBoxAuthors.TabIndex = 18;
            // 
            // labelAuthorsList
            // 
            labelAuthorsList.AutoSize = true;
            labelAuthorsList.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            labelAuthorsList.Location = new Point(20, 35);
            labelAuthorsList.Name = "labelAuthorsList";
            labelAuthorsList.Size = new Size(125, 18);
            labelAuthorsList.TabIndex = 0;
            labelAuthorsList.Text = "Список авторов:";
            // 
            // buttonAddAuthor
            // 
            buttonAddAuthor.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            buttonAddAuthor.Location = new Point(199, 200);
            buttonAddAuthor.Name = "buttonAddAuthor";
            buttonAddAuthor.Size = new Size(100, 30);
            buttonAddAuthor.TabIndex = 19;
            buttonAddAuthor.Text = "Добавить";
            buttonAddAuthor.UseVisualStyleBackColor = true;
            buttonAddAuthor.Click += buttonAddAuthor_Click;
            // 
            // buttonEditAuthor
            // 
            buttonEditAuthor.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            buttonEditAuthor.Location = new Point(305, 200);
            buttonEditAuthor.Name = "buttonEditAuthor";
            buttonEditAuthor.Size = new Size(129, 30);
            buttonEditAuthor.TabIndex = 20;
            buttonEditAuthor.Text = "Редактировать";
            buttonEditAuthor.UseVisualStyleBackColor = true;
            buttonEditAuthor.Click += buttonEditAuthor_Click;
            // 
            // buttonRemoveAuthor
            // 
            buttonRemoveAuthor.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            buttonRemoveAuthor.Location = new Point(440, 200);
            buttonRemoveAuthor.Name = "buttonRemoveAuthor";
            buttonRemoveAuthor.Size = new Size(90, 30);
            buttonRemoveAuthor.TabIndex = 21;
            buttonRemoveAuthor.Text = "Удалить";
            buttonRemoveAuthor.UseVisualStyleBackColor = true;
            buttonRemoveAuthor.Click += buttonRemoveAuthor_Click;
            // 
            // panelBottom
            // 
            panelBottom.Controls.Add(buttonLoad);
            panelBottom.Controls.Add(buttonSave);
            panelBottom.Controls.Add(buttonExit);
            panelBottom.Dock = DockStyle.Bottom;
            panelBottom.Location = new Point(0, 690);
            panelBottom.Name = "panelBottom";
            panelBottom.Size = new Size(1100, 50);
            panelBottom.TabIndex = 3;
            // 
            // buttonLoad
            // 
            buttonLoad.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            buttonLoad.Location = new Point(740, 10);
            buttonLoad.Name = "buttonLoad";
            buttonLoad.Size = new Size(100, 30);
            buttonLoad.TabIndex = 18;
            buttonLoad.Text = "Загрузить";
            buttonLoad.UseVisualStyleBackColor = true;
            buttonLoad.Click += buttonLoad_Click;
            // 
            // buttonSave
            // 
            buttonSave.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            buttonSave.Location = new Point(850, 10);
            buttonSave.Name = "buttonSave";
            buttonSave.Size = new Size(100, 30);
            buttonSave.TabIndex = 17;
            buttonSave.Text = "Сохранить";
            buttonSave.UseVisualStyleBackColor = true;
            buttonSave.Click += buttonSave_Click;
            // 
            // buttonExit
            // 
            buttonExit.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            buttonExit.Location = new Point(960, 10);
            buttonExit.Name = "buttonExit";
            buttonExit.Size = new Size(100, 30);
            buttonExit.TabIndex = 16;
            buttonExit.Text = "Выход";
            buttonExit.UseVisualStyleBackColor = true;
            buttonExit.Click += buttonExit_Click;
            // 
            // errorProvider1
            // 
            errorProvider1.ContainerControl = this;
            // 
            // openFileDialog1
            // 
            openFileDialog1.FileName = "openFileDialog1";
            // 
            // groupBoxPublisher
            // 
            groupBoxPublisher.Controls.Add(radioPublisherState);
            groupBoxPublisher.Controls.Add(radioPublisherPrivate);
            groupBoxPublisher.Controls.Add(numericPublisherYear);
            groupBoxPublisher.Controls.Add(comboPublisherCountry);
            groupBoxPublisher.Controls.Add(labelPublisherType);
            groupBoxPublisher.Controls.Add(labelPublisherYear);
            groupBoxPublisher.Controls.Add(textPublisherCity);
            groupBoxPublisher.Controls.Add(labelPublisherCity);
            groupBoxPublisher.Controls.Add(labelPublisherCountry);
            groupBoxPublisher.Controls.Add(labelPublisherName);
            groupBoxPublisher.Controls.Add(textPublisherName);
            groupBoxPublisher.Font = new Font("Microsoft Sans Serif", 13.8F);
            groupBoxPublisher.Location = new Point(12, 424);
            groupBoxPublisher.Name = "groupBoxPublisher";
            groupBoxPublisher.Size = new Size(500, 260);
            groupBoxPublisher.TabIndex = 11;
            groupBoxPublisher.TabStop = false;
            groupBoxPublisher.Text = "Издательство";
            // 
            // radioPublisherState
            // 
            radioPublisherState.AutoSize = true;
            radioPublisherState.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            radioPublisherState.Location = new Point(293, 184);
            radioPublisherState.Name = "radioPublisherState";
            radioPublisherState.Size = new Size(151, 22);
            radioPublisherState.TabIndex = 16;
            radioPublisherState.TabStop = true;
            radioPublisherState.Text = "Государственное";
            radioPublisherState.UseVisualStyleBackColor = true;
            // 
            // radioPublisherPrivate
            // 
            radioPublisherPrivate.AutoSize = true;
            radioPublisherPrivate.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            radioPublisherPrivate.Location = new Point(200, 184);
            radioPublisherPrivate.Name = "radioPublisherPrivate";
            radioPublisherPrivate.Size = new Size(87, 22);
            radioPublisherPrivate.TabIndex = 15;
            radioPublisherPrivate.TabStop = true;
            radioPublisherPrivate.Text = "Частное";
            radioPublisherPrivate.UseVisualStyleBackColor = true;
            // 
            // numericPublisherYear
            // 
            numericPublisherYear.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            numericPublisherYear.Location = new Point(200, 147);
            numericPublisherYear.Maximum = new decimal(new int[] { 2026, 0, 0, 0 });
            numericPublisherYear.Minimum = new decimal(new int[] { 1400, 0, 0, 0 });
            numericPublisherYear.Name = "numericPublisherYear";
            numericPublisherYear.Size = new Size(150, 24);
            numericPublisherYear.TabIndex = 14;
            numericPublisherYear.Value = new decimal(new int[] { 2000, 0, 0, 0 });
            // 
            // comboPublisherCountry
            // 
            comboPublisherCountry.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            comboPublisherCountry.FormattingEnabled = true;
            comboPublisherCountry.Location = new Point(200, 72);
            comboPublisherCountry.Name = "comboPublisherCountry";
            comboPublisherCountry.Size = new Size(150, 26);
            comboPublisherCountry.TabIndex = 13;
            // 
            // labelPublisherType
            // 
            labelPublisherType.AutoSize = true;
            labelPublisherType.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            labelPublisherType.Location = new Point(20, 186);
            labelPublisherType.Name = "labelPublisherType";
            labelPublisherType.Size = new Size(33, 18);
            labelPublisherType.TabIndex = 12;
            labelPublisherType.Text = "Тип";
            // 
            // labelPublisherYear
            // 
            labelPublisherYear.AutoSize = true;
            labelPublisherYear.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            labelPublisherYear.Location = new Point(20, 149);
            labelPublisherYear.Name = "labelPublisherYear";
            labelPublisherYear.Size = new Size(111, 18);
            labelPublisherYear.TabIndex = 11;
            labelPublisherYear.Text = "Год написания";
            // 
            // textPublisherCity
            // 
            textPublisherCity.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            textPublisherCity.Location = new Point(200, 109);
            textPublisherCity.Name = "textPublisherCity";
            textPublisherCity.Size = new Size(150, 24);
            textPublisherCity.TabIndex = 10;
            // 
            // labelPublisherCity
            // 
            labelPublisherCity.AutoSize = true;
            labelPublisherCity.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            labelPublisherCity.Location = new Point(20, 112);
            labelPublisherCity.Name = "labelPublisherCity";
            labelPublisherCity.Size = new Size(52, 18);
            labelPublisherCity.TabIndex = 8;
            labelPublisherCity.Text = "Город";
            // 
            // labelPublisherCountry
            // 
            labelPublisherCountry.AutoSize = true;
            labelPublisherCountry.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            labelPublisherCountry.Location = new Point(20, 75);
            labelPublisherCountry.Name = "labelPublisherCountry";
            labelPublisherCountry.Size = new Size(58, 18);
            labelPublisherCountry.TabIndex = 7;
            labelPublisherCountry.Text = "Страна";
            // 
            // labelPublisherName
            // 
            labelPublisherName.AutoSize = true;
            labelPublisherName.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            labelPublisherName.Location = new Point(20, 38);
            labelPublisherName.Name = "labelPublisherName";
            labelPublisherName.Size = new Size(175, 18);
            labelPublisherName.TabIndex = 0;
            labelPublisherName.Text = "Название издательства";
            // 
            // textPublisherName
            // 
            textPublisherName.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            textPublisherName.Location = new Point(200, 35);
            textPublisherName.Name = "textPublisherName";
            textPublisherName.Size = new Size(150, 24);
            textPublisherName.TabIndex = 6;
            // 
            // FormLibrary
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1100, 740);
            Controls.Add(groupBoxPublisher);
            Controls.Add(groupBoxAuthors);
            Controls.Add(groupBoxBook);
            Controls.Add(panelBottom);
            Controls.Add(labelTitle);
            Name = "FormLibrary";
            Text = "Электронная библиотека";
            groupBoxBook.ResumeLayout(false);
            groupBoxBook.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numericPrice).EndInit();
            groupBoxAuthors.ResumeLayout(false);
            groupBoxAuthors.PerformLayout();
            groupBoxFees.ResumeLayout(false);
            groupBoxFees.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numericPercent).EndInit();
            groupBoxCurrentAuthor.ResumeLayout(false);
            groupBoxCurrentAuthor.PerformLayout();
            panelBottom.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)errorProvider1).EndInit();
            groupBoxPublisher.ResumeLayout(false);
            groupBoxPublisher.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numericPublisherYear).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label labelTitle;
        private GroupBox groupBoxBook;
        private GroupBox groupBoxPublisher;
        private GroupBox groupBoxAuthors;
        private Panel panelBottom;
        private Label labelGenre;
        private ComboBox comboGenre;
        private Label labelTitleBook;
        private TextBox textTitle;
        private Label labelFileSize;
        private TextBox textFileSize;
        private Label labelPages;
        private TextBox textPages;
        private Label labelPrice;
        private NumericUpDown numericPrice;
        private Label labelPublisherName;
        private TextBox textPublisherName;
        private Label labelPublisherCountry;
        private ComboBox comboPublisherCountry;
        private Label labelPublisherCity;
        private TextBox textPublisherCity;
        private Label labelPublisherYear;
        private NumericUpDown numericPublisherYear;
        private Label labelPublisherType;
        private RadioButton radioPublisherPrivate;
        private RadioButton radioPublisherState;
        private Label labelAuthorsList;
        private ListBox listBoxAuthors;
        private Button buttonAddAuthor;
        private Button buttonEditAuthor;
        private Button buttonRemoveAuthor;
        private GroupBox groupBoxCurrentAuthor;
        private Label labelCurrentAuthorName;
        private TextBox textCurrentAuthorName;
        private Label labelCurrentAuthorCountry;
        private TextBox textCurrentAuthorCountry;
        private Label labelCurrentAuthorID;
        private TextBox textCurrentAuthorID;
        private GroupBox groupBoxFees;
        private Label labelPercent;
        private NumericUpDown numericPercent;
        private Label labelTotalFee;
        private TextBox textTotalFee;
        private Label labelFeePerAuthor;
        private TextBox textFeePerAuthor;
        private Button buttonCalculate;
        private Button buttonSave;
        private Button buttonLoad;
        private Button buttonExit;
        private ErrorProvider errorProvider1;
        private ToolTip toolTip1;
        private SaveFileDialog saveFileDialog1;
        private OpenFileDialog openFileDialog1;
    }
}