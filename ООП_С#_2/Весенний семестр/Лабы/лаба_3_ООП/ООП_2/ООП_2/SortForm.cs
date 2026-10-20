using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using static ООП_2.FormLibrary;

namespace ООП_2
{
    public partial class SortForm : Form
    {
        private readonly List<Author> authors;
        public List<Author> SortedAuthors { get; private set; }

        public SortForm(List<Author> authorsList)
        {
            InitializeComponent();
            authors = authorsList;
            InitializeSortOptions();
        }

        private void InitializeSortOptions()
        {
            // Первый критерий
            comboSort1.Items.AddRange(new string[] {
                "По ФИО (А-Я)",
                "По ФИО (Я-А)",
                "По стране",
                "По ID (возрастание)",
                "По ID (убывание)"
            });
            comboSort1.SelectedIndex = 0;

            // Второй критерий
            comboSort2.Items.AddRange(new string[] {
                "Нет",
                "По ФИО (А-Я)",
                "По ФИО (Я-А)",
                "По стране",
                "По ID (возрастание)",
                "По ID (убывание)"
            });
            comboSort2.SelectedIndex = 0;
        }

        private void buttonSort_Click(object sender, EventArgs e)
        {
            try
            {
                var query = authors.AsEnumerable();

                // Применяем первую сортировку
                query = ApplySorting(query, comboSort1.SelectedItem.ToString());

                // Применяем вторую сортировку, если выбрана
                string secondSort = comboSort2.SelectedItem.ToString();
                if (secondSort != "Нет")
                {
                    query = ApplyThenBy(query, secondSort);
                }

                SortedAuthors = query.ToList();

                // Показываем результаты
                DisplayResults();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сортировке: {ex.Message}");
            }
        }

        private IEnumerable<Author> ApplySorting(IEnumerable<Author> source, string sortType)
        {
            switch (sortType)
            {
                case "По ФИО (А-Я)":
                    return source.OrderBy(a => a.FullName);
                case "По ФИО (Я-А)":
                    return source.OrderByDescending(a => a.FullName);
                case "По стране":
                    return source.OrderBy(a => a.Country);
                case "По ID (возрастание)":
                    return source.OrderBy(a => a.Id);
                case "По ID (убывание)":
                    return source.OrderByDescending(a => a.Id);
                default:
                    return source;
            }
        }

        private IEnumerable<Author> ApplyThenBy(IEnumerable<Author> source, string sortType)
        {
            var ordered = source as IOrderedEnumerable<Author>;
            if (ordered == null) return source;

            switch (sortType)
            {
                case "По ФИО (А-Я)":
                    return ordered.ThenBy(a => a.FullName);
                case "По ФИО (Я-А)":
                    return ordered.ThenByDescending(a => a.FullName);
                case "По стране":
                    return ordered.ThenBy(a => a.Country);
                case "По ID (возрастание)":
                    return ordered.ThenBy(a => a.Id);
                case "По ID (убывание)":
                    return ordered.ThenByDescending(a => a.Id);
                default:
                    return source;
            }
        }

        private void DisplayResults()
        {
            listBoxResults.DataSource = null;
            listBoxResults.DataSource = SortedAuthors;
            listBoxResults.DisplayMember = "ToString";
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}