using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using static ООП_2.FormLibrary;

namespace ООП_2
{
    public partial class SearchConstructorForm : Form
    {
        private readonly List<Author> authors;
        private readonly BookFile currentBook;
        private readonly Publisher publisher;

        public List<SearchResultItem> SearchResults { get; private set; }

        public SearchConstructorForm(List<Author> authors, BookFile book, Publisher publisher)
        {
            InitializeComponent();
            this.authors = authors;
            this.currentBook = book;
            this.publisher = publisher;

            InitializeSearchOptions();
        }

        private void InitializeSearchOptions()
        {
            // Заполняем категории поиска
            comboCategory.Items.AddRange(new string[] { "Все", "Авторы", "Книги", "Издательства" });
            comboCategory.SelectedIndex = 0;

            // Заполняем поля для поиска
            comboField.Items.AddRange(new string[] {
                "Любое поле",
                "ФИО автора",
                "Страна автора",
                "ID автора",
                "Название книги",
                "Жанр книги",
                "Размер файла",
                "Количество страниц",
                "Стоимость доступа",
                "Название издательства",
                "Город издательства",
                "Страна издательства",
                "Год основания",
                "Тип издательства"
            });
            comboField.SelectedIndex = 0;

            // Заполняем типы поиска
            comboSearchType.Items.AddRange(new string[] {
                "Точное совпадение",
                "Содержит текст",
                "Регулярное выражение",
                "Больше чем",
                "Меньше чем",
                "В диапазоне"
            });
            comboSearchType.SelectedIndex = 0;

            // Обновляем видимость дополнительных полей
            UpdateSearchTypeVisibility();
        }

        private void comboSearchType_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateSearchTypeVisibility();
        }

        private void UpdateSearchTypeVisibility()
        {
            bool isRange = comboSearchType.SelectedItem?.ToString() == "В диапазоне";
            labelRangeTo.Visible = isRange;
            textRangeValue.Visible = isRange;

            // Для числовых полей можно добавить дополнительную валидацию
        }

        private void buttonSearch_Click(object sender, EventArgs e)
        {
            PerformSearch();
        }

        private void PerformSearch()
        {
            SearchResults = new List<SearchResultItem>();
            string searchText = textSearchValue.Text.Trim();
            string rangeValue = textRangeValue.Text.Trim();
            string category = comboCategory.SelectedItem.ToString();
            string field = comboField.SelectedItem.ToString();
            string searchType = comboSearchType.SelectedItem.ToString();

            if (string.IsNullOrWhiteSpace(searchText) && searchType != "В диапазоне")
            {
                MessageBox.Show("Введите текст для поиска");
                return;
            }

            try
            {
                // Поиск по авторам
                if (category == "Все" || category == "Авторы")
                {
                    SearchInAuthors(searchText, rangeValue, field, searchType);
                }

                // Поиск по книге
                if (category == "Все" || category == "Книги")
                {
                    SearchInBook(searchText, rangeValue, field, searchType);
                }

                // Поиск по издательству
                if (category == "Все" || category == "Издательства")
                {
                    SearchInPublisher(searchText, rangeValue, field, searchType);
                }

                // Показываем результаты
                ShowResults();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при поиске: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SearchInAuthors(string searchText, string rangeValue, string field, string searchType)
        {
            var query = authors.AsEnumerable();

            // Фильтруем по полю
            if (field != "Любое поле")
            {
                switch (field)
                {
                    case "ФИО автора":
                        query = FilterByString(query, a => a.FullName, searchText, searchType, rangeValue);
                        break;
                    case "Страна автора":
                        query = FilterByString(query, a => a.Country, searchText, searchType, rangeValue);
                        break;
                    case "ID автора":
                        query = FilterByNumber(query, a => a.Id, searchText, searchType, rangeValue);
                        break;
                }
            }
            else
            {
                // Поиск по всем полям автора
                query = query.Where(a =>
                    IsMatch(a.FullName, searchText, searchType, rangeValue) ||
                    IsMatch(a.Country, searchText, searchType, rangeValue) ||
                    IsMatch(a.Id.ToString(), searchText, searchType, rangeValue));
            }

            foreach (var author in query)
            {
                SearchResults.Add(new SearchResultItem
                {
                    Category = "Автор",
                    FieldName = field,
                    Value = searchText,
                    DisplayText = author.ToString()
                });
            }
        }

        private void SearchInBook(string searchText, string rangeValue, string field, string searchType)
        {
            if (currentBook == null) return;

            bool match = false;

            switch (field)
            {
                case "Любое поле":
                    match = IsMatch(currentBook.Title, searchText, searchType, rangeValue) ||
                            IsMatch(currentBook.Genre, searchText, searchType, rangeValue) ||
                            IsMatch(currentBook.FileSize.ToString(), searchText, searchType, rangeValue) ||
                            IsMatch(currentBook.Pages.ToString(), searchText, searchType, rangeValue) ||
                            IsMatch(currentBook.Price.ToString(), searchText, searchType, rangeValue);
                    break;
                case "Название книги":
                    match = IsMatch(currentBook.Title, searchText, searchType, rangeValue);
                    break;
                case "Жанр книги":
                    match = IsMatch(currentBook.Genre, searchText, searchType, rangeValue);
                    break;
                case "Размер файла":
                    match = IsNumericMatch(currentBook.FileSize, searchText, searchType, rangeValue);
                    break;
                case "Количество страниц":
                    match = IsNumericMatch(currentBook.Pages, searchText, searchType, rangeValue);
                    break;
                case "Стоимость доступа":
                    match = IsNumericMatch((double)currentBook.Price, searchText, searchType, rangeValue);
                    break;
            }

            if (match)
            {
                SearchResults.Add(new SearchResultItem
                {
                    Category = "Книга",
                    FieldName = field,
                    Value = searchText,
                    DisplayText = $"{currentBook.Title} ({currentBook.Genre})"
                });
            }
        }

        private void SearchInPublisher(string searchText, string rangeValue, string field, string searchType)
        {
            if (publisher == null) return;

            bool match = false;

            switch (field)
            {
                case "Любое поле":
                    match = IsMatch(publisher.Name, searchText, searchType, rangeValue) ||
                            IsMatch(publisher.City, searchText, searchType, rangeValue) ||
                            IsMatch(publisher.Country, searchText, searchType, rangeValue) ||
                            IsMatch(publisher.FoundedYear.ToString(), searchText, searchType, rangeValue) ||
                            IsMatch(publisher.Type, searchText, searchType, rangeValue);
                    break;
                case "Название издательства":
                    match = IsMatch(publisher.Name, searchText, searchType, rangeValue);
                    break;
                case "Город издательства":
                    match = IsMatch(publisher.City, searchText, searchType, rangeValue);
                    break;
                case "Страна издательства":
                    match = IsMatch(publisher.Country, searchText, searchType, rangeValue);
                    break;
                case "Год основания":
                    match = IsNumericMatch(publisher.FoundedYear, searchText, searchType, rangeValue);
                    break;
                case "Тип издательства":
                    match = IsMatch(publisher.Type, searchText, searchType, rangeValue);
                    break;
            }

            if (match)
            {
                SearchResults.Add(new SearchResultItem
                {
                    Category = "Издательство",
                    FieldName = field,
                    Value = searchText,
                    DisplayText = publisher.Name
                });
            }
        }

        private IEnumerable<Author> FilterByString(IEnumerable<Author> source,
            Func<Author, string> selector, string searchText, string searchType, string rangeValue)
        {
            return source.Where(a => IsMatch(selector(a), searchText, searchType, rangeValue));
        }

        private IEnumerable<Author> FilterByNumber(IEnumerable<Author> source,
            Func<Author, int> selector, string searchText, string searchType, string rangeValue)
        {
            return source.Where(a => IsNumericMatch(selector(a), searchText, searchType, rangeValue));
        }

        private bool IsMatch(string value, string pattern, string searchType, string rangeValue)
        {
            if (string.IsNullOrEmpty(value)) return false;

            switch (searchType)
            {
                case "Точное совпадение":
                    return value.Equals(pattern, StringComparison.OrdinalIgnoreCase);

                case "Содержит текст":
                    return value.IndexOf(pattern, StringComparison.OrdinalIgnoreCase) >= 0;

                case "Регулярное выражение":
                    try
                    {
                        return Regex.IsMatch(value, pattern, RegexOptions.IgnoreCase);
                    }
                    catch
                    {
                        return false;
                    }

                default:
                    return false;
            }
        }

        private bool IsNumericMatch(double value, string pattern, string searchType, string rangeValue)
        {
            if (!double.TryParse(pattern, out double searchNum)) return false;

            switch (searchType)
            {
                case "Точное совпадение":
                    return Math.Abs(value - searchNum) < 0.001;

                case "Больше чем":
                    return value > searchNum;

                case "Меньше чем":
                    return value < searchNum;

                case "В диапазоне":
                    if (double.TryParse(rangeValue, out double rangeEnd))
                    {
                        return value >= searchNum && value <= rangeEnd;
                    }
                    return false;

                default:
                    return IsMatch(value.ToString(), pattern, searchType, rangeValue);
            }
        }

        private void ShowResults()
        {
            if (SearchResults.Count == 0)
            {
                MessageBox.Show("Ничего не найдено", "Результаты поиска",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                var resultsForm = new SearchResultsForm(SearchResults);
                resultsForm.ShowDialog();
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}