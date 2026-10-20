using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace ООП_2
{
    public partial class SearchResultsForm : Form
    {
        private List<SearchResultItem> results;

        public SearchResultsForm(List<SearchResultItem> searchResults)
        {
            InitializeComponent();
            results = searchResults;
            DisplayResults();
        }

        private void DisplayResults()
        {
            listBoxResults.DataSource = null;
            listBoxResults.DataSource = results;
            listBoxResults.DisplayMember = "DisplayText";

            labelCount.Text = $"Найдено: {results.Count}";
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            // Здесь можно добавить сохранение результатов
            MessageBox.Show("Функция сохранения результатов будет добавлена позже");
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void listBoxResults_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxResults.SelectedItem is SearchResultItem selected)
            {
                textDetails.Text = $"Категория: {selected.Category}\r\n" +
                                  $"Поле поиска: {selected.FieldName}\r\n" +
                                  $"Искомое значение: {selected.Value}\r\n" +
                                  $"Найдено: {selected.DisplayText}";
            }
        }
    }
}