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
    ///
    /// Демонстрирует:
    ///  - DependencyProperty с ValidateValueCallback и CoerceValueCallback
    ///  - RoutedEvent (стратегия Bubbling)
    /// </summary>
    public partial class StarRating : UserControl
    {
        // ==========================================================================
        //  ValueProperty: текущее значение рейтинга. С валидацией и коррекцией.
        // ==========================================================================

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(
                nameof(Value),
                typeof(double),
                typeof(StarRating),
                new FrameworkPropertyMetadata(
                    defaultValue: 0.0,
                    flags: FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    propertyChangedCallback: OnValueChanged,
                    coerceValueCallback: CoerceValue),
                validateValueCallback: ValidateValue);

        /// <summary>Текущее значение рейтинга, может быть дробным (например, 4.5).</summary>
        public double Value
        {
            get => (double)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        /// <summary>
        /// Валидация: не пропускаем NaN/бесконечности и отрицательные числа.
        /// Если возвращает false — установка свойства завершится исключением.
        /// </summary>
        private static bool ValidateValue(object value)
        {
            if (value is double d)
                return !double.IsNaN(d) && !double.IsInfinity(d) && d >= 0;
            return false;
        }

        /// <summary>
        /// Коррекция: если кто-то задаст значение больше MaxValue — обрежем.
        /// Также округлим до полузвёзд (шаг 0.5), чтобы не было визуального мусора.
        /// </summary>
        private static object CoerceValue(DependencyObject d, object baseValue)
        {
            var sr = (StarRating)d;
            double v = (double)baseValue;
            if (v < 0) v = 0;
            if (v > sr.MaxValue) v = sr.MaxValue;
            v = System.Math.Round(v * 2) / 2.0;
            return v;
        }

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sr = (StarRating)d;
            sr.RecalcFills();

            // Поднимаем bubbling-событие — родитель сможет его поймать
            // (например, окно профиля показывает "Спасибо за оценку").
            sr.RaiseEvent(new RoutedPropertyChangedEventArgs<double>(
                (double)e.OldValue, (double)e.NewValue, ValueChangedEvent));
        }

        // ==========================================================================
        //  MaxValueProperty: с валидацией (1..10).
        // ==========================================================================

        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register(
                nameof(MaxValue),
                typeof(int),
                typeof(StarRating),
                new PropertyMetadata(5, OnMaxChanged),
                ValidateMaxValue);

        public int MaxValue
        {
            get => (int)GetValue(MaxValueProperty);
            set => SetValue(MaxValueProperty, value);
        }

        private static bool ValidateMaxValue(object value)
            => value is int i && i >= 1 && i <= 10;

        private static void OnMaxChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sr = (StarRating)d;
            sr.RebuildStars();
            // если Value > нового MaxValue — Coerce обрежет
            sr.CoerceValue(ValueProperty);
        }

        // ==========================================================================
        //  IsReadOnly — обычное DP без коррекции.
        // ==========================================================================

        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register(nameof(IsReadOnly), typeof(bool), typeof(StarRating),
                new PropertyMetadata(false));

        /// <summary>Если true — клики игнорируются, рейтинг показывается только.</summary>
        public bool IsReadOnly
        {
            get => (bool)GetValue(IsReadOnlyProperty);
            set => SetValue(IsReadOnlyProperty, value);
        }

        // ==========================================================================
        //  ROUTED EVENT: ValueChanged — стратегия Bubbling.
        //  Возникает при любом изменении Value, всплывает к корню окна.
        // ==========================================================================

        public static readonly RoutedEvent ValueChangedEvent =
            EventManager.RegisterRoutedEvent(
                "ValueChanged",
                RoutingStrategy.Bubble,
                typeof(RoutedPropertyChangedEventHandler<double>),
                typeof(StarRating));

        public event RoutedPropertyChangedEventHandler<double> ValueChanged
        {
            add => AddHandler(ValueChangedEvent, value);
            remove => RemoveHandler(ValueChangedEvent, value);
        }

        // ==========================================================================

        /// <summary>Коллекция элементов-звёзд для биндинга в ItemsControl.</summary>
        public ObservableCollection<StarItem> StarItems { get; } = new();

        public StarRating()
        {
            InitializeComponent();
            RebuildStars();
        }

        private void RebuildStars()
        {
            StarItems.Clear();
            for (int i = 1; i <= MaxValue; i++)
                StarItems.Add(new StarItem { Index = i });
            RecalcFills();
        }

        private void RecalcFills()
        {
            // Для каждой звезды считаем уровень заливки от 0 до 1.
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
