using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Xml.Serialization;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace ООП_2
{
    public partial class FormLibrary : Form
    {
        // Классы данных с атрибутами валидации
        public class Author
        {
            [Required(ErrorMessage = "ID автора обязателен")]
            [Range(1, int.MaxValue, ErrorMessage = "ID должен быть положительным числом")]
            public int Id { get; set; }

            [Required(ErrorMessage = "ФИО автора обязательно")]
            [StringLength(100, MinimumLength = 2, ErrorMessage = "ФИО должно содержать от 2 до 100 символов")]
            [AuthorName(true, 2, ErrorMessage = "ФИО должно содержать минимум 2 слова (фамилия и имя)")]
            public string FullName { get; set; }

            [Required(ErrorMessage = "Страна автора обязательна")]
            [StringLength(50, ErrorMessage = "Название страны не должно превышать 50 символов")]
            [RegularExpression(@"^[а-яА-ЯёЁa-zA-Z\s\-]+$", ErrorMessage = "Страна может содержать только буквы, пробелы и дефисы")]
            public string Country { get; set; }

            public override string ToString()
            {
                return $"{FullName} ({Country})";
            }
        }

        public class Publisher
        {
            [Required(ErrorMessage = "Название издательства обязательно")]
            [StringLength(100, MinimumLength = 2, ErrorMessage = "Название должно содержать от 2 до 100 символов")]
            public string Name { get; set; }

            [Required(ErrorMessage = "Страна издательства обязательна")]
            [StringLength(50, ErrorMessage = "Название страны не должно превышать 50 символов")]
            public string Country { get; set; }

            [Required(ErrorMessage = "Город издательства обязателен")]
            [StringLength(50, ErrorMessage = "Название города не должно превышать 50 символов")]
            public string City { get; set; }

            [Required(ErrorMessage = "Год основания обязателен")]
            [Range(1400, 2026, ErrorMessage = "Год должен быть от 1400 до 2026")]
            public int FoundedYear { get; set; }

            [Required(ErrorMessage = "Тип издательства обязателен")]
            [RegularExpression("^(Частное|Государственное)$", ErrorMessage = "Тип должен быть 'Частное' или 'Государственное'")]
            public string Type { get; set; }
        }

        public class BookFile
        {
            [Required(ErrorMessage = "Жанр книги обязателен")]
            [StringLength(30, MinimumLength = 3, ErrorMessage = "Жанр должен содержать от 3 до 30 символов")]
            public string Genre { get; set; }

            [Required(ErrorMessage = "Название книги обязательно")]
            [StringLength(200, MinimumLength = 1, ErrorMessage = "Название должно содержать от 1 до 200 символов")]
            public string Title { get; set; }

            [Required(ErrorMessage = "Размер файла обязателен")]
            [Range(0.01, 999999.99, ErrorMessage = "Размер файла должен быть больше 0")]
            public double FileSize { get; set; }

            [Required(ErrorMessage = "Количество страниц обязательно")]
            [Range(1, 10000, ErrorMessage = "Количество страниц должно быть от 1 до 10000")]
            public int Pages { get; set; }

            [Required(ErrorMessage = "Стоимость доступа обязательна")]
            [Price(0.01, 100000, false, ErrorMessage = "Некорректная цена")]
            public decimal Price { get; set; }

            [Required(ErrorMessage = "Список авторов не может быть пустым")]
            [MinLength(1, ErrorMessage = "Добавьте хотя бы одного автора")]
            public List<Author> Authors { get; set; } = new List<Author>();

            public Publisher PublisherInfo { get; set; }

            [Required(ErrorMessage = "Процент отчислений обязателен")]
            [Range(0, 100, ErrorMessage = "Процент должен быть от 0 до 100")]
            public decimal Percent { get; set; }
        }

        // Поля формы
        private List<Author> authors = new List<Author>();
        private int nextAuthorId = 1;

        // Последние результаты для сохранения
        private List<SearchResultItem> lastSearchResults;
        private List<Author> lastSortedAuthors;

        // Для отслеживания состояния закрепления
        private bool _isToolStripDocked = true;

        // Таймер для обновления времени
        private System.Windows.Forms.Timer timer;

        public FormLibrary()
        {
            InitializeComponent();
            InitializeMyComponents();

            // Подписка на события меню
            this.exactSearchMenuItem.Click += ExactSearchMenuItem_Click;
            this.regexSearchMenuItem.Click += RegexSearchMenuItem_Click;
            this.rangeSearchMenuItem.Click += RangeSearchMenuItem_Click;
            this.advancedSearchMenuItem.Click += AdvancedSearchMenuItem_Click;
            this.sortByNameMenuItem.Click += SortByNameMenuItem_Click;
            this.sortByCountryMenuItem.Click += SortByCountryMenuItem_Click;
            this.sortByIDMenuItem.Click += SortByIDMenuItem_Click;
            this.sortByNameDescMenuItem.Click += SortByNameDescMenuItem_Click;
            this.sortAdvancedMenuItem.Click += SortAdvancedMenuItem_Click;
            this.saveXMLMenuItem.Click += SaveXMLMenuItem_Click;
            this.saveJSONMenuItem.Click += SaveJSONMenuItem_Click;
            this.aboutMenu.Click += AboutMenu_Click;

            // Подписки для панели инструментов
            this.toolStripButtonSearch.Click += (s, e) => {
                var book = ValidateInputsWithAttributes() ? CreateBookFromInput() : null;
                var searchForm = new SearchConstructorForm(authors, book, GetPublisherFromInput());
                searchForm.ShowDialog();
            };

            this.toolStripButtonSort.Click += (s, e) => {
                var sortForm = new SortForm(authors);
                if (sortForm.ShowDialog() == DialogResult.OK)
                {
                    lastSortedAuthors = sortForm.SortedAuthors;
                    ShowSortedResults();
                }
            };

            this.toolStripButtonClear.Click += ButtonClear_Click;
            this.toolStripButtonDelete.Click += buttonRemoveAuthor_Click;
            this.toolStripButtonBack.Click += ButtonBack_Click;
            this.toolStripButtonForward.Click += ButtonForward_Click;
            this.toolStripButtonToggleDock.Click += ToolStripButtonToggleDock_Click;

            // Настройка таймера для обновления времени
            timer = new System.Windows.Forms.Timer();
            timer.Interval = 1000;
            timer.Tick += Timer_Tick;
            timer.Start();

            // Устанавливаем начальные значения в строке состояния
            UpdateStatusStripObjects();
            SetStatusAction("Программа запущена");

            // ===== Подсказки для элементов формы =====
            toolTip1.SetToolTip(comboGenre, "Жанр книги (обязательно, от 3 до 30 символов)");
            toolTip1.SetToolTip(textTitle, "Название книги (обязательно, от 1 до 200 символов)");
            toolTip1.SetToolTip(textFileSize, "Размер файла в МБ (обязательно, больше 0, используйте запятую)");
            toolTip1.SetToolTip(textPages, "Количество страниц (обязательно, от 1 до 10000)");
            toolTip1.SetToolTip(numericPrice, "Стоимость доступа (обязательно, от 0.01 до 100000)");
            toolTip1.SetToolTip(textPublisherName, "Название издательства (обязательно, от 2 до 100 символов)");
            toolTip1.SetToolTip(comboPublisherCountry, "Страна издательства (обязательно)");
            toolTip1.SetToolTip(textPublisherCity, "Город издательства (обязательно, до 50 символов)");
            toolTip1.SetToolTip(numericPublisherYear, "Год основания (обязательно, от 1400 до 2026)");
            toolTip1.SetToolTip(listBoxAuthors, "Список авторов (должен содержать хотя бы одного автора)");
            toolTip1.SetToolTip(numericPercent, "Процент отчислений (обязательно, от 0 до 100)");
            toolTip1.SetToolTip(buttonSave, "Сохранить данные книги в XML или JSON файл");
            toolTip1.SetToolTip(buttonLoad, "Загрузить данные книги из файла");
            toolTip1.SetToolTip(buttonExit, "Закрыть программу");
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
            UpdateStatusStripObjects();
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
            string name = Microsoft.VisualBasic.Interaction.InputBox(
                "Введите ФИО автора (от 2 до 100 символов, только буквы):",
                "Добавление автора", "");

            if (string.IsNullOrWhiteSpace(name)) return;

            string country = Microsoft.VisualBasic.Interaction.InputBox(
                "Введите страну автора:",
                "Добавление автора", "Россия");

            if (string.IsNullOrWhiteSpace(country)) return;

            var tempAuthor = new Author
            {
                Id = nextAuthorId,
                FullName = name,
                Country = country
            };

            var errors = ValidateObject(tempAuthor);
            if (errors.Count > 0)
            {
                string errorMessage = "Ошибки валидации:\n" +
                    string.Join("\n", errors.Select(e => $"• {e.ErrorMessage}"));
                MessageBox.Show(errorMessage, "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            authors.Add(tempAuthor);
            nextAuthorId++;
            UpdateAuthorsList();
            SetStatusAction($"Автор {tempAuthor.FullName} добавлен");
        }

        private void buttonEditAuthor_Click(object sender, EventArgs e)
        {
            if (listBoxAuthors.SelectedItem is Author selected)
            {
                string name = Microsoft.VisualBasic.Interaction.InputBox(
                    "Измените ФИО автора:",
                    "Редактирование", selected.FullName);

                if (string.IsNullOrWhiteSpace(name)) return;

                string country = Microsoft.VisualBasic.Interaction.InputBox(
                    "Измените страну автора:",
                    "Редактирование", selected.Country);

                if (string.IsNullOrWhiteSpace(country)) return;

                var tempAuthor = new Author
                {
                    Id = selected.Id,
                    FullName = name,
                    Country = country
                };

                var errors = ValidateObject(tempAuthor);
                if (errors.Count > 0)
                {
                    string errorMessage = "Ошибки валидации:\n" +
                        string.Join("\n", errors.Select(e => $"• {e.ErrorMessage}"));
                    MessageBox.Show(errorMessage, "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                selected.FullName = name;
                selected.Country = country;

                UpdateAuthorsList();
                listBoxAuthors.SelectedItem = selected;
                SetStatusAction($"Автор {name} изменён");
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
                    string deletedName = selected.FullName;
                    authors.Remove(selected);
                    UpdateAuthorsList();
                    SetStatusAction($"Автор {deletedName} удалён");
                }
            }
            else
            {
                MessageBox.Show("Выберите автора для удаления");
            }
        }

        private BookFile CreateBookFromInput()
        {
            // Безопасный парсинг размера файла
            double fileSize = 0;
            string fileSizeText = textFileSize.Text.Trim();

            // Заменяем точку на запятую для русской локали
            fileSizeText = fileSizeText.Replace('.', ',');

            if (!double.TryParse(fileSizeText, out fileSize))
            {
                fileSize = 0;
            }

            // Безопасный парсинг страниц
            int pages = 0;
            if (!int.TryParse(textPages.Text, out pages))
            {
                pages = 0;
            }

            return new BookFile
            {
                Genre = comboGenre.Text,
                Title = textTitle.Text,
                FileSize = fileSize,
                Pages = pages,
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

        private Publisher GetPublisherFromInput()
        {
            return new Publisher
            {
                Name = textPublisherName.Text,
                Country = comboPublisherCountry.Text,
                City = textPublisherCity.Text,
                FoundedYear = (int)numericPublisherYear.Value,
                Type = radioPublisherPrivate.Checked ? "Частное" : "Государственное"
            };
        }

        // МЕТОДЫ ВАЛИДАЦИИ
        private List<ValidationResult> ValidateObject(object obj)
        {
            var results = new List<ValidationResult>();
            var context = new ValidationContext(obj);
            Validator.TryValidateObject(obj, context, results, true);
            return results;
        }

        private bool ValidateInputsWithAttributes()
        {
            errorProvider1.Clear();

            bool hasErrors = false;
            var allErrors = new List<string>();

            // Безопасный парсинг для валидации
            double fileSize = 0;
            string fileSizeText = textFileSize.Text.Trim().Replace('.', ',');
            double.TryParse(fileSizeText, out fileSize);

            int pages = 0;
            int.TryParse(textPages.Text, out pages);

            var book = new BookFile
            {
                Genre = comboGenre.Text,
                Title = textTitle.Text,
                FileSize = fileSize,
                Pages = pages,
                Price = numericPrice.Value,
                Authors = authors,
                Percent = numericPercent.Value,
                PublisherInfo = null
            };

            var bookErrors = ValidateObject(book);
            foreach (var error in bookErrors)
            {
                hasErrors = true;
                allErrors.Add($"Книга: {error.ErrorMessage}");

                foreach (var memberName in error.MemberNames)
                {
                    switch (memberName)
                    {
                        case nameof(BookFile.Genre):
                            errorProvider1.SetError(comboGenre, error.ErrorMessage);
                            break;
                        case nameof(BookFile.Title):
                            errorProvider1.SetError(textTitle, error.ErrorMessage);
                            break;
                        case nameof(BookFile.FileSize):
                            errorProvider1.SetError(textFileSize, error.ErrorMessage);
                            break;
                        case nameof(BookFile.Pages):
                            errorProvider1.SetError(textPages, error.ErrorMessage);
                            break;
                        case nameof(BookFile.Price):
                            errorProvider1.SetError(numericPrice, error.ErrorMessage);
                            break;
                        case nameof(BookFile.Percent):
                            errorProvider1.SetError(numericPercent, error.ErrorMessage);
                            break;
                        case nameof(BookFile.Authors):
                            errorProvider1.SetError(listBoxAuthors, error.ErrorMessage);
                            break;
                    }
                }
            }

            for (int i = 0; i < authors.Count; i++)
            {
                var authorErrors = ValidateObject(authors[i]);
                foreach (var error in authorErrors)
                {
                    hasErrors = true;
                    allErrors.Add($"Автор {i + 1}: {error.ErrorMessage}");

                    if (listBoxAuthors.SelectedIndex == i)
                    {
                        foreach (var memberName in error.MemberNames)
                        {
                            switch (memberName)
                            {
                                case nameof(Author.FullName):
                                    errorProvider1.SetError(textCurrentAuthorName, error.ErrorMessage);
                                    break;
                                case nameof(Author.Country):
                                    errorProvider1.SetError(textCurrentAuthorCountry, error.ErrorMessage);
                                    break;
                            }
                        }
                    }
                }
            }

            var publisher = new Publisher
            {
                Name = textPublisherName.Text,
                Country = comboPublisherCountry.Text,
                City = textPublisherCity.Text,
                FoundedYear = (int)numericPublisherYear.Value,
                Type = radioPublisherPrivate.Checked ? "Частное" : "Государственное"
            };

            var publisherErrors = ValidateObject(publisher);
            foreach (var error in publisherErrors)
            {
                hasErrors = true;
                allErrors.Add($"Издательство: {error.ErrorMessage}");

                foreach (var memberName in error.MemberNames)
                {
                    switch (memberName)
                    {
                        case nameof(Publisher.Name):
                            errorProvider1.SetError(textPublisherName, error.ErrorMessage);
                            break;
                        case nameof(Publisher.Country):
                            errorProvider1.SetError(comboPublisherCountry, error.ErrorMessage);
                            break;
                        case nameof(Publisher.City):
                            errorProvider1.SetError(textPublisherCity, error.ErrorMessage);
                            break;
                        case nameof(Publisher.FoundedYear):
                            errorProvider1.SetError(numericPublisherYear, error.ErrorMessage);
                            break;
                    }
                }
            }

            if (hasErrors && allErrors.Count > 0)
            {
                string errorMessage = "Обнаружены ошибки валидации:\n\n" + string.Join("\n", allErrors.Take(5));
                if (allErrors.Count > 5)
                    errorMessage += $"\n\n... и еще {allErrors.Count - 5} ошибок";

                MessageBox.Show(errorMessage, "Ошибки валидации",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            return !hasErrors;
        }

        // ОБРАБОТЧИКИ КНОПОК
        private void buttonCalculate_Click(object sender, EventArgs e)
        {
            if (!ValidateInputsWithAttributes()) return;

            var book = CreateBookFromInput();

            decimal totalFee = numericPrice.Value * (numericPercent.Value / 100m);
            decimal perAuthor = authors.Count > 0 ? totalFee / authors.Count : 0;

            textTotalFee.Text = totalFee.ToString("F2") + " ₽";
            textFeePerAuthor.Text = perAuthor.ToString("F2") + " ₽";

            MessageBox.Show($"Всего отчислений: {totalFee:F2} ₽\nНа автора: {perAuthor:F2} ₽");
            SetStatusAction("Отчисления рассчитаны");
            UpdateStatusStripObjects();
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            if (!ValidateInputsWithAttributes()) return;

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
                    SetStatusAction($"Данные сохранены в {Path.GetFileName(saveFileDialog1.FileName)}");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка: " + ex.Message);
                    SetStatusAction("Ошибка при сохранении");
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
                            var bookErrors = ValidateObject(book);
                            if (bookErrors.Count > 0)
                            {
                                string errorMessage = "Загруженный файл содержит некорректные данные:\n" +
                                    string.Join("\n", bookErrors.Select(e => $"• {e.ErrorMessage}"));

                                if (MessageBox.Show(errorMessage + "\n\nВсё равно загрузить?",
                                    "Предупреждение", MessageBoxButtons.YesNo,
                                    MessageBoxIcon.Warning) == DialogResult.No)
                                {
                                    return;
                                }
                            }

                            comboGenre.Text = book.Genre;
                            textTitle.Text = book.Title;

                            // Заменяем точку на запятую при отображении
                            textFileSize.Text = book.FileSize.ToString().Replace('.', ',');

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

                            buttonCalculate_Click(null, null);

                            MessageBox.Show("Загружено!");
                            SetStatusAction($"Данные загружены из {Path.GetFileName(dialog.FileName)}");
                            UpdateStatusStripObjects();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка: " + ex.Message);
                        SetStatusAction("Ошибка при загрузке");
                    }
                }
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

        // ОБРАБОТЧИКИ ПОИСКА
        private void ExactSearchMenuItem_Click(object sender, EventArgs e)
        {
            ShowSearchDialog("Точное совпадение");
        }

        private void RegexSearchMenuItem_Click(object sender, EventArgs e)
        {
            ShowSearchDialog("Регулярное выражение");
        }

        private void RangeSearchMenuItem_Click(object sender, EventArgs e)
        {
            ShowSearchDialog("В диапазоне");
        }

        private void AdvancedSearchMenuItem_Click(object sender, EventArgs e)
        {
            var book = ValidateInputsWithAttributes() ? CreateBookFromInput() : null;
            var searchForm = new SearchConstructorForm(authors, book, GetPublisherFromInput());
            searchForm.ShowDialog();

            if (searchForm.SearchResults != null && searchForm.SearchResults.Count > 0)
            {
                lastSearchResults = searchForm.SearchResults;
            }
        }

        private void ShowSearchDialog(string searchType)
        {
            string searchText = Microsoft.VisualBasic.Interaction.InputBox(
                $"Введите текст для поиска ({searchType}):",
                "Поиск", "");

            if (string.IsNullOrWhiteSpace(searchText)) return;

            string rangeValue = "";
            if (searchType == "В диапазоне")
            {
                rangeValue = Microsoft.VisualBasic.Interaction.InputBox(
                    "Введите верхнюю границу диапазона:",
                    "Диапазон", "");
            }

            PerformSimpleSearch(searchText, searchType, rangeValue);
        }

        private void PerformSimpleSearch(string searchText, string searchType, string rangeValue)
        {
            lastSearchResults = new List<SearchResultItem>();

            try
            {
                foreach (var author in authors)
                {
                    if (IsMatchSimple(author.FullName, searchText, searchType, rangeValue) ||
                        IsMatchSimple(author.Country, searchText, searchType, rangeValue) ||
                        IsMatchSimple(author.Id.ToString(), searchText, searchType, rangeValue))
                    {
                        lastSearchResults.Add(new SearchResultItem
                        {
                            Category = "Автор",
                            FieldName = "Любое поле",
                            Value = searchText,
                            DisplayText = author.ToString()
                        });
                    }
                }

                if (ValidateInputsWithAttributes())
                {
                    var book = CreateBookFromInput();
                    if (book != null)
                    {
                        if (IsMatchSimple(book.Title, searchText, searchType, rangeValue) ||
                            IsMatchSimple(book.Genre, searchText, searchType, rangeValue))
                        {
                            lastSearchResults.Add(new SearchResultItem
                            {
                                Category = "Книга",
                                FieldName = "Любое поле",
                                Value = searchText,
                                DisplayText = book.Title
                            });
                        }
                    }
                }

                var publisher = GetPublisherFromInput();
                if (publisher != null)
                {
                    if (IsMatchSimple(publisher.Name, searchText, searchType, rangeValue) ||
                        IsMatchSimple(publisher.City, searchText, searchType, rangeValue) ||
                        IsMatchSimple(publisher.Country, searchText, searchType, rangeValue))
                    {
                        lastSearchResults.Add(new SearchResultItem
                        {
                            Category = "Издательство",
                            FieldName = "Любое поле",
                            Value = searchText,
                            DisplayText = publisher.Name
                        });
                    }
                }

                if (lastSearchResults.Count > 0)
                {
                    var resultsForm = new SearchResultsForm(lastSearchResults);
                    resultsForm.ShowDialog();
                    SetStatusAction($"Найдено {lastSearchResults.Count} результатов");
                }
                else
                {
                    MessageBox.Show("Ничего не найдено");
                    SetStatusAction("Поиск не дал результатов");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при поиске: {ex.Message}");
                SetStatusAction("Ошибка при поиске");
            }
        }

        private bool IsMatchSimple(string value, string pattern, string searchType, string rangeValue)
        {
            if (string.IsNullOrEmpty(value)) return false;

            switch (searchType)
            {
                case "Точное совпадение":
                    return value.Equals(pattern, StringComparison.OrdinalIgnoreCase);

                case "Содержит текст":
                case "Регулярное выражение":
                    try
                    {
                        return Regex.IsMatch(value, pattern, RegexOptions.IgnoreCase);
                    }
                    catch
                    {
                        return value.IndexOf(pattern, StringComparison.OrdinalIgnoreCase) >= 0;
                    }

                default:
                    return false;
            }
        }

        // ОБРАБОТЧИКИ СОРТИРОВКИ
        private void SortByNameMenuItem_Click(object sender, EventArgs e)
        {
            lastSortedAuthors = authors.OrderBy(a => a.FullName).ToList();
            ShowSortedResults();
        }

        private void SortByCountryMenuItem_Click(object sender, EventArgs e)
        {
            lastSortedAuthors = authors.OrderBy(a => a.Country).ThenBy(a => a.FullName).ToList();
            ShowSortedResults();
        }

        private void SortByIDMenuItem_Click(object sender, EventArgs e)
        {
            lastSortedAuthors = authors.OrderBy(a => a.Id).ToList();
            ShowSortedResults();
        }

        private void SortByNameDescMenuItem_Click(object sender, EventArgs e)
        {
            lastSortedAuthors = authors.OrderByDescending(a => a.FullName).ToList();
            ShowSortedResults();
        }

        private void SortAdvancedMenuItem_Click(object sender, EventArgs e)
        {
            var sortForm = new SortForm(authors);
            if (sortForm.ShowDialog() == DialogResult.OK)
            {
                lastSortedAuthors = sortForm.SortedAuthors;
                ShowSortedResults();
            }
        }

        private void ShowSortedResults()
        {
            if (lastSortedAuthors != null && lastSortedAuthors.Count > 0)
            {
                string result = "Результаты сортировки:\n\n";
                foreach (var author in lastSortedAuthors)
                {
                    result += $"{author.Id}: {author.FullName} ({author.Country})\n";
                }

                SetStatusAction("Выполнена сортировка авторов");

                if (MessageBox.Show(result + "\n\nСохранить результаты?", "Сортировка",
                    MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    saveFileDialog1.Filter = "XML файлы|*.xml|JSON файлы|*.json";
                    saveFileDialog1.FileName = $"SortedAuthors_{DateTime.Now:yyyyMMddHHmmss}";

                    if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        SaveResults(saveFileDialog1.FileName, lastSortedAuthors.Cast<object>().ToList());
                    }
                }
            }
        }

        // ОБРАБОТЧИКИ СОХРАНЕНИЯ
        private void SaveXMLMenuItem_Click(object sender, EventArgs e)
        {
            SaveResultsToFile("xml");
        }

        private void SaveJSONMenuItem_Click(object sender, EventArgs e)
        {
            SaveResultsToFile("json");
        }

        private void SaveResultsToFile(string format)
        {
            if (lastSearchResults == null && lastSortedAuthors == null)
            {
                MessageBox.Show("Нет результатов для сохранения. Сначала выполните поиск или сортировку.");
                return;
            }

            saveFileDialog1.Filter = format == "xml" ? "XML файлы|*.xml" : "JSON файлы|*.json";
            saveFileDialog1.FileName = $"Results_{DateTime.Now:yyyyMMddHHmmss}.{format}";

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    object dataToSave = lastSearchResults != null ?
                        (object)lastSearchResults : lastSortedAuthors;

                    SaveResults(saveFileDialog1.FileName, dataToSave);
                    MessageBox.Show("Результаты сохранены!");
                    SetStatusAction($"Результаты сохранены в {Path.GetFileName(saveFileDialog1.FileName)}");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при сохранении: {ex.Message}");
                    SetStatusAction("Ошибка при сохранении результатов");
                }
            }
        }

        private void SaveResults(string fileName, object data)
        {
            if (fileName.EndsWith(".xml"))
            {
                var xml = new XmlSerializer(data.GetType());
                using (var fs = new FileStream(fileName, FileMode.Create))
                    xml.Serialize(fs, data);
            }
            else
            {
                string json = JsonSerializer.Serialize(data, new JsonSerializerOptions
                {
                    WriteIndented = true
                });
                File.WriteAllText(fileName, json);
            }
        }

        private void AboutMenu_Click(object sender, EventArgs e)
        {
            string version = "1.1.0";
            string developer = "Потребко Вадим Русланович"; 
            string group = "Группа 6-2";

            MessageBox.Show(
                $"Электронная библиотека\nВерсия: {version}\nРазработчик: {developer}\nГруппа: {group}\n\n© 2026",
                "О программе",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }

        private void ButtonClear_Click(object sender, EventArgs e)
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
                SetStatusAction("Форма очищена");
                UpdateStatusStripObjects();
            }
        }

        private void ButtonBack_Click(object sender, EventArgs e)
        {
            if (listBoxAuthors.Items.Count > 0)
            {
                int currentIndex = listBoxAuthors.SelectedIndex;
                if (currentIndex > 0)
                {
                    listBoxAuthors.SelectedIndex = currentIndex - 1;
                    SetStatusAction($"Переход к автору: {listBoxAuthors.SelectedItem}");
                }
                else
                {
                    MessageBox.Show("Это первый автор в списке", "Навигация",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void ButtonForward_Click(object sender, EventArgs e)
        {
            if (listBoxAuthors.Items.Count > 0)
            {
                int currentIndex = listBoxAuthors.SelectedIndex;
                if (currentIndex < listBoxAuthors.Items.Count - 1)
                {
                    listBoxAuthors.SelectedIndex = currentIndex + 1;
                    SetStatusAction($"Переход к автору: {listBoxAuthors.SelectedItem}");
                }
                else
                {
                    MessageBox.Show("Это последний автор в списке", "Навигация",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void ToolStripButtonToggleDock_Click(object sender, EventArgs e)
        {
            _isToolStripDocked = !_isToolStripDocked;

            if (_isToolStripDocked)
            {
                toolStrip.Dock = DockStyle.Top;
                toolStripButtonToggleDock.Text = "📌";
                toolStripButtonToggleDock.ToolTipText = "Открепить панель";
                SetStatusAction("Панель инструментов закреплена");
            }
            else
            {
                toolStrip.Dock = DockStyle.None;
                toolStripButtonToggleDock.Text = "⤢";
                toolStripButtonToggleDock.ToolTipText = "Закрепить панель";
                toolStrip.AllowDrop = true;
                toolStrip.GripStyle = ToolStripGripStyle.Visible;
                SetStatusAction("Панель инструментов откреплена");
            }
        }

        private void UpdateStatusStripObjects()
        {
            int authorCount = authors.Count;
            int bookCount = 0;
            if (!string.IsNullOrWhiteSpace(textTitle.Text) &&
                !string.IsNullOrWhiteSpace(textFileSize.Text) &&
                !string.IsNullOrWhiteSpace(textPages.Text))
            {
                bookCount = 1;
            }
            int publisherCount = !string.IsNullOrWhiteSpace(textPublisherName.Text) ? 1 : 0;

            toolStripStatusLabelObjects.Text = $"Авторов: {authorCount} | Книг: {bookCount} | Издательств: {publisherCount}";
        }

        private void SetStatusAction(string action)
        {
            toolStripStatusLabelAction.Text = $"Последнее действие: {action}";
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            toolStripStatusLabelDateTime.Text = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (_isToolStripDocked && toolStrip.Dock == DockStyle.Top)
            {
                toolStrip.Width = this.ClientSize.Width;
            }
        }
    }
}