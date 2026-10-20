using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace WatchShop.Controls
{
    /// <summary>
    /// Пользовательский элемент управления — звёздный рейтинг.
    /// Поддерживает дробные значения (0..MaxValue), кликабелен (если IsReadOnly=false),
    /// при наведении звёзды масштабируются (анимация в XAML).
    /// </summary>
    public partial class StarRating : UserControl
    {
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(double), typeof(StarRating),
                new FrameworkPropertyMetadata(0.0,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnValueChanged));

        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register(nameof(MaxValue), typeof(int), typeof(StarRating),
                new PropertyMetadata(5, OnMaxChanged));

        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register(nameof(IsReadOnly), typeof(bool), typeof(StarRating),
                new PropertyMetadata(false));

        /// <summary>Текущее значение рейтинга, может быть дробным (например, 4.5).</summary>
        public double Value
        {
            get => (double)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        /// <summary>Максимум звёзд (по умолчанию 5).</summary>
        public int MaxValue
        {
            get => (int)GetValue(MaxValueProperty);
            set => SetValue(MaxValueProperty, value);
        }

        /// <summary>Если true — клики игнорируются, рейтинг показывается только.</summary>
        public bool IsReadOnly
        {
            get => (bool)GetValue(IsReadOnlyProperty);
            set => SetValue(IsReadOnlyProperty, value);
        }

        /// <summary>Коллекция элементов-звёзд для биндинга в ItemsControl.</summary>
        public ObservableCollection<StarItem> StarItems { get; } = new();

        public StarRating()
        {
            InitializeComponent();
            RebuildStars();
        }
        // при изменении рейтинга → пересчитывает заливку звёзд
        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is StarRating sr) sr.RecalcFills();
        }
        // при изменении максимального количества звёзд → перестраивает коллекцию
        private static void OnMaxChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is StarRating sr) sr.RebuildStars();
        }
        // перестраивает коллекцию звёзд при изменении maxvalue
        private void RebuildStars()
        {
            StarItems.Clear();
            for (int i = 1; i <= MaxValue; i++)
                StarItems.Add(new StarItem { Index = i });
            RecalcFills();
        }
        // пересчитывает уровень заливки для каждой звезды на основе текущего value
        private void RecalcFills()
        {
            // Для каждой звезды считаем уровень заливки от 0 до 1.
            // Звезда i заполнена полностью если Value >= i,
            // частично если i-1 < Value < i, иначе пуста.
            foreach (var item in StarItems)
            {
                double fill = Value - (item.Index - 1);
                if (fill < 0) fill = 0;
                if (fill > 1) fill = 1;
                item.FillOffset = fill;
            }
        }

        private void StarButton_Click(object sender, RoutedEventArgs e)
        {
            if (IsReadOnly) return;
            if (sender is Button b && b.Tag is int idx)
            {
                Value = idx;
            }
        }
    }

    /// <summary>Модель одной звезды для DataTemplate.</summary>
    public class StarItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private int index;
        private double fillOffset;

        public int Index { get => index; set { index = value; OnPropertyChanged(); } }
        /// <summary>0..1 — насколько звезда заполнена слева направо.</summary>
        public double FillOffset { get => fillOffset; set { fillOffset = value; OnPropertyChanged(); } }

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name ?? ""));
    }
}
