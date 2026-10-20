using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Xml.Serialization;
using System.Text.Json;

namespace ООП_2
{
    public partial class FormLibrary : Form
    {
        // Простые классы данных прямо внутри формы
        public class Author
        {
            public int Id { get; set; }
            public string FullName { get; set; }
            public string Country { get; set; }

            public override string ToString()
            {
                return $"{FullName} ({Country})";
            }
        }

        public class Publisher
        {
            public string Name { get; set; }
            public string Country { get; set; }
            public string City { get; set; }
            public int FoundedYear { get; set; }
            public string Type { get; set; }
        }

        public class BookFile
        {
            public string Genre { get; set; }
            public string Title { get; set; }
            public double FileSize { get; set; }
            public int Pages { get; set; }
            public decimal Price { get; set; }
            public List<Author> Authors { get; set; } = new List<Author>();
            public Publisher PublisherInfo { get; set; }
            public decimal Percent { get; set; }
        }

        // Поля формы
        private List<Author> authors = new List<Author>();
        private int nextAuthorId = 1;

        public FormLibrary()
        {
            InitializeComponent();
            InitializeMyComponents();

            // ===== Файл книги =====
            toolTip1.SetToolTip(comboGenre, "Выберите жанр книги из списка");
            toolTip1.SetToolTip(textTitle, "Введите название книги");
            toolTip1.SetToolTip(textFileSize, "Введите размер файла в мегабайтах (например: 2.5)");
            toolTip1.SetToolTip(textPages, "Введите количество страниц (только цифры)");
            toolTip1.SetToolTip(numericPrice, "Укажите стоимость доступа к книге в рублях");

            // ===== Издательство =====
            toolTip1.SetToolTip(textPublisherName, "Введите название издательства");
            toolTip1.SetToolTip(comboPublisherCountry, "Выберите страну издательства");
            toolTip1.SetToolTip(textPublisherCity, "Введите город, где находится издательство");
            toolTip1.SetToolTip(numericPublisherYear, "Укажите год основания издательства (от 1400 до текущего года)");
            toolTip1.SetToolTip(radioPublisherPrivate, "Выберите, если издательство частное");
            toolTip1.SetToolTip(radioPublisherState, "Выберите, если издательство государственное");

            // ===== Авторы =====
            toolTip1.SetToolTip(listBoxAuthors, "Список добавленных авторов. Выберите автора для просмотра деталей");
            toolTip1.SetToolTip(buttonAddAuthor, "Добавить нового автора в список");
            toolTip1.SetToolTip(buttonEditAuthor, "Изменить данные выбранного автора");
            toolTip1.SetToolTip(buttonRemoveAuthor, "Удалить выбранного автора из списка");

            // ===== Текущий автор =====
            toolTip1.SetToolTip(textCurrentAuthorName, "ФИО автора, выбранного в списке выше.");
            toolTip1.SetToolTip(textCurrentAuthorCountry, "Страна автора, выбранного в списке выше.");
            toolTip1.SetToolTip(textCurrentAuthorID, "Уникальный идентификационный номер автора.");

            // ===== Отчисления =====
            toolTip1.SetToolTip(numericPercent, "Введите процент отчислений авторам от стоимости доступа (0-100)");
            toolTip1.SetToolTip(textTotalFee, "Общая сумма отчислений всем авторам (рассчитывается автоматически)");
            toolTip1.SetToolTip(textFeePerAuthor, "Сумма отчислений на одного автора (рассчитывается автоматически)");
            toolTip1.SetToolTip(buttonCalculate, "Нажмите для расчёта отчислений на основе введённых данных");

            // ===== Кнопки действий =====
            toolTip1.SetToolTip(buttonSave, "Сохранить данные книги в XML или JSON файл");
            toolTip1.SetToolTip(buttonLoad, "Загрузить данные книги из ранее сохранённого файла");
            toolTip1.SetToolTip(buttonExit, "Закрыть программу");
        }

        private void numericPrice_ValueChanged(object sender, EventArgs e)
        {
            // Можно автоматически пересчитывать при изменении цены
            // Например, очищать результаты расчёта
            textTotalFee.Clear();
            textFeePerAuthor.Clear();
        }
        private void InitializeMyComponents()
        {

            // Заполнение жанров
            comboGenre.Items.Clear();
            comboGenre.Items.AddRange(new string[] {
                "Роман", "Повесть", "Детектив", "Фантастика",
                "Фэнтези", "Научная", "Учебник", "Энциклопедия"
            });
            comboGenre.SelectedIndex = 0;

            // Заполнение стран для издательства
            comboPublisherCountry.Items.Clear();
            comboPublisherCountry.Items.AddRange(new string[] {
                "Россия", "Беларусь", "Украина", "США",
                "Великобритания", "Германия", "Франция"
            });
            comboPublisherCountry.SelectedIndex = 0;

            // Настройка года издательства
            numericPublisherYear.Minimum = 1400;
            numericPublisherYear.Maximum = DateTime.Now.Year;
            numericPublisherYear.Value = 2000;

            // По умолчанию частное издательство
            radioPublisherPrivate.Checked = true;

            // Обновление списка авторов
            UpdateAuthorsList();

            // Подписка на события
            listBoxAuthors.SelectedIndexChanged += ListBoxAuthors_SelectedIndexChanged;
        }

        private void UpdateAuthorsList()
        {
            listBoxAuthors.DataSource = null;
            listBoxAuthors.DataSource = authors;
            listBoxAuthors.DisplayMember = "ToString";

            if (authors.Count > 0 && listBoxAuthors.SelectedIndex == -1)
                listBoxAuthors.SelectedIndex = 0;

            UpdateCurrentAuthorFields();
        }

        private void UpdateCurrentAuthorFields()
        {
            if (listBoxAuthors.SelectedItem is Author selected)
            {
                textCurrentAuthorName.Text = selected.FullName;
                textCurrentAuthorCountry.Text = selected.Country;
                textCurrentAuthorID.Text = selected.Id.ToString();
            }
            else
            {
                textCurrentAuthorName.Clear();
                textCurrentAuthorCountry.Clear();
                textCurrentAuthorID.Clear();
            }
        }

        private void ListBoxAuthors_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateCurrentAuthorFields();
        }

        private void buttonAddAuthor_Click(object sender, EventArgs e)
        {
            // Простой диалог добавления автора
            string name = Microsoft.VisualBasic.Interaction.InputBox(
                "Введите ФИО автора:", "Добавление автора", "");

            if (string.IsNullOrWhiteSpace(name)) return;

            string country = Microsoft.VisualBasic.Interaction.InputBox(
                "Введите страну автора:", "Добавление автора", "Россия");

            if (string.IsNullOrWhiteSpace(country)) return;

            authors.Add(new Author
            {
                Id = nextAuthorId++,
                FullName = name,
                Country = country
            });

            UpdateAuthorsList();
        }

        private void buttonEditAuthor_Click(object sender, EventArgs e)
        {
            if (listBoxAuthors.SelectedItem is Author selected)
            {
                string name = Microsoft.VisualBasic.Interaction.InputBox(
                    "Измените ФИО автора:", "Редактирование", selected.FullName);

                if (string.IsNullOrWhiteSpace(name)) return;

                string country = Microsoft.VisualBasic.Interaction.InputBox(
                    "Измените страну автора:", "Редактирование", selected.Country);

                if (string.IsNullOrWhiteSpace(country)) return;

                selected.FullName = name;
                selected.Country = country;

                UpdateAuthorsList();
                listBoxAuthors.SelectedItem = selected;
            }
            else
            {
                MessageBox.Show("Выберите автора для редактирования");
            }
        }

        private void buttonRemoveAuthor_Click(object sender, EventArgs e)
        {
            if (listBoxAuthors.SelectedItem is Author selected)
            {
                if (MessageBox.Show($"Удалить {selected.FullName}?", "Подтверждение",
                    MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    authors.Remove(selected);
                    UpdateAuthorsList();
                }
            }
            else
            {
                MessageBox.Show("Выберите автора для удаления");
            }
        }

        private bool ValidateInputs()
        {
            errorProvider1.Clear();
            bool hasError = false;

            if (string.IsNullOrWhiteSpace(textTitle.Text))
            {
                errorProvider1.SetError(textTitle, "Введите название");
                hasError = true;
            }

            if (!double.TryParse(textFileSize.Text, out double size) || size <= 0)
            {
                errorProvider1.SetError(textFileSize, "Введите корректный размер");
                hasError = true;
            }

            if (!int.TryParse(textPages.Text, out int pages) || pages <= 0)
            {
                errorProvider1.SetError(textPages, "Введите корректное кол-во страниц");
                hasError = true;
            }

            if (numericPrice.Value <= 0)
            {
                errorProvider1.SetError(numericPrice, "Цена должна быть > 0");
                hasError = true;
            }

            if (numericPercent.Value < 0 || numericPercent.Value > 100)
            {
                errorProvider1.SetError(numericPercent, "Процент от 0 до 100");
                hasError = true;
            }

            if (authors.Count == 0)
            {
                errorProvider1.SetError(listBoxAuthors, "Добавьте хотя бы одного автора");
                hasError = true;
            }

            if (string.IsNullOrWhiteSpace(textPublisherName.Text))
            {
                errorProvider1.SetError(textPublisherName, "Введите название издательства");
                hasError = true;
            }

            return !hasError;
        }

        private BookFile CreateBookFromInput()
        {
            return new BookFile
            {
                Genre = comboGenre.Text,
                Title = textTitle.Text,
                FileSize = double.Parse(textFileSize.Text),
                Pages = int.Parse(textPages.Text),
                Price = numericPrice.Value,
                Percent = numericPercent.Value,
                Authors = new List<Author>(authors),
                PublisherInfo = new Publisher
                {
                    Name = textPublisherName.Text,
                    Country = comboPublisherCountry.Text,
                    City = textPublisherCity.Text,
                    FoundedYear = (int)numericPublisherYear.Value,
                    Type = radioPublisherPrivate.Checked ? "Частное" : "Государственное"
                }
            };
        }

        private void buttonCalculate_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs()) return;

            decimal totalFee = numericPrice.Value * (numericPercent.Value / 100m);
            decimal perAuthor = authors.Count > 0 ? totalFee / authors.Count : 0;

            textTotalFee.Text = totalFee.ToString("F2") + " ₽";
            textFeePerAuthor.Text = perAuthor.ToString("F2") + " ₽";

            MessageBox.Show($"Всего отчислений: {totalFee:F2} ₽\nНа автора: {perAuthor:F2} ₽");
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs()) return;

            var book = CreateBookFromInput();

            saveFileDialog1.Filter = "XML файлы|*.xml|JSON файлы|*.json";
            saveFileDialog1.FileName = $"{book.Title}_{DateTime.Now:yyyyMMdd}";

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if (saveFileDialog1.FileName.EndsWith(".xml"))
                    {
                        var xml = new XmlSerializer(typeof(BookFile));
                        using (var fs = new FileStream(saveFileDialog1.FileName, FileMode.Create))
                            xml.Serialize(fs, book);
                    }
                    else
                    {
                        string json = JsonSerializer.Serialize(book, new JsonSerializerOptions
                        {
                            WriteIndented = true
                        });
                        File.WriteAllText(saveFileDialog1.FileName, json);
                    }

                    MessageBox.Show("Сохранено!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка: " + ex.Message);
                }
            }
        }

        private void buttonLoad_Click(object sender, EventArgs e)
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.Filter = "XML файлы|*.xml|JSON файлы|*.json";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        BookFile book = null;

                        if (dialog.FileName.EndsWith(".xml"))
                        {
                            var xml = new XmlSerializer(typeof(BookFile));
                            using (var fs = new FileStream(dialog.FileName, FileMode.Open))
                                book = (BookFile)xml.Deserialize(fs);
                        }
                        else
                        {
                            string json = File.ReadAllText(dialog.FileName);
                            book = JsonSerializer.Deserialize<BookFile>(json);
                        }

                        if (book != null)
                        {
                            // Заполняем форму
                            comboGenre.Text = book.Genre;
                            textTitle.Text = book.Title;
                            textFileSize.Text = book.FileSize.ToString();
                            textPages.Text = book.Pages.ToString();
                            numericPrice.Value = book.Price;
                            numericPercent.Value = book.Percent;

                            authors.Clear();
                            authors.AddRange(book.Authors);
                            nextAuthorId = (authors.Count > 0 ? authors.Max(a => a.Id) : 0) + 1;
                            UpdateAuthorsList();

                            if (book.PublisherInfo != null)
                            {
                                textPublisherName.Text = book.PublisherInfo.Name;
                                comboPublisherCountry.Text = book.PublisherInfo.Country;
                                textPublisherCity.Text = book.PublisherInfo.City;
                                numericPublisherYear.Value = book.PublisherInfo.FoundedYear;
                                radioPublisherPrivate.Checked = book.PublisherInfo.Type == "Частное";
                                radioPublisherState.Checked = book.PublisherInfo.Type == "Государственное";
                            }

                            // Пересчитываем отчисления
                            buttonCalculate_Click(null, null);

                            MessageBox.Show("Загружено!");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка: " + ex.Message);
                    }
                }
            }
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Очистить все поля?", "Подтверждение",
                MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                comboGenre.SelectedIndex = 0;
                textTitle.Clear();
                textFileSize.Clear();
                textPages.Clear();
                numericPrice.Value = 0;

                textPublisherName.Clear();
                comboPublisherCountry.SelectedIndex = 0;
                textPublisherCity.Clear();
                numericPublisherYear.Value = 2000;
                radioPublisherPrivate.Checked = true;

                authors.Clear();
                UpdateAuthorsList();

                numericPercent.Value = 0;
                textTotalFee.Clear();
                textFeePerAuthor.Clear();

                errorProvider1.Clear();
            }
        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        // Валидация ввода
        private void textFileSize_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != ',')
                e.Handled = true;
        }

        private void textPages_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                e.Handled = true;
        }

        private void textYear_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                e.Handled = true;
        }
    }
}