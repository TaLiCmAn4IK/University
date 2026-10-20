using System.Windows;
using System.Windows.Controls;

namespace WatchShop.Controls
{
    /// <summary>
    /// Пользовательский элемент управления — ползунок скидки.
    /// Демонстрирует:
    /// 1) DependencyProperty со ValidateValueCallback (валидация значений) и
    ///    CoerceValueCallback (коррекция значений к диапазону 0..MaxDiscount).
    /// 2) RoutedEvent трёх типов маршрутизации: Direct, Bubbling, Tunneling.
    /// </summary>
    public partial class DiscountSlider : UserControl
    {
        //  DEPENDENCY PROPERTY 1: Discount — текущее значение скидки в процентах.
        //  Здесь демонстрируется и валидация, и коррекция.

        public static readonly DependencyProperty DiscountProperty =
            DependencyProperty.Register(
                nameof(Discount),
                typeof(double),
                typeof(DiscountSlider),
                new FrameworkPropertyMetadata(
                    defaultValue: 0.0,
                    flags: FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    propertyChangedCallback: OnDiscountChanged,
                    coerceValueCallback: CoerceDiscount),       // <-- коррекция
                validateValueCallback: ValidateDiscount);       // <-- валидация

        public double Discount
        {
            get => (double)GetValue(DiscountProperty);
            set => SetValue(DiscountProperty, value);
        }

        /// <summary>
        /// ValidateValueCallback — отбрасывает совсем неправильные значения
        /// (NaN, бесконечность, отрицательные числа). Если возвращает false,
        /// WPF выкинет исключение и не позволит установить такое значение.
        /// Вызывается ДО CoerceValueCallback.
        /// </summary>
        private static bool ValidateDiscount(object value)
        {
            if (value is double d)
            {
                if (double.IsNaN(d) || double.IsInfinity(d)) return false;
                if (d < 0) return false;
                return true;
            }
            return false;
        }

        /// <summary>
        /// CoerceValueCallback — мягко "подгоняет" значение в допустимый диапазон.
        /// Например, если кто-то поставит 150% — обрежем до MaxDiscount.
        /// Вызывается каждый раз ПОСЛЕ валидации, перед фактической установкой.
        /// </summary>
        private static object CoerceDiscount(DependencyObject d, object baseValue)
        {
            var ds = (DiscountSlider)d;
            double v = (double)baseValue;
            if (v < 0) v = 0;
            if (v > ds.MaxDiscount) v = ds.MaxDiscount;
            return v;
        }

        private static void OnDiscountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ds = (DiscountSlider)d;
            double oldVal = (double)e.OldValue;
            double newVal = (double)e.NewValue;

            // Поднимаем три события — пусть проект увидит разные стратегии маршрутизации.
            // Сначала Tunneling (PreviewDiscountChanged) — летит сверху вниз.
            ds.RaiseEvent(new RoutedPropertyChangedEventArgs<double>(oldVal, newVal,
                PreviewDiscountChangedEvent));

            // Потом Bubbling (DiscountChanged) — всплывает наверх.
            ds.RaiseEvent(new RoutedPropertyChangedEventArgs<double>(oldVal, newVal,
                DiscountChangedEvent));

            // И Direct — DiscountAppliedEvent, поднимается только если есть значимая скидка.
            if (newVal > 0 && oldVal == 0)
            {
                ds.RaiseEvent(new RoutedEventArgs(DiscountAppliedEvent));
            }
        }

        //  DEPENDENCY PROPERTY 2: MaxDiscount — верхняя граница скидки.
        //  При изменении переcчитывает Discount (чтобы не превышал новый максимум).

        public static readonly DependencyProperty MaxDiscountProperty =
            DependencyProperty.Register(
                nameof(MaxDiscount),
                typeof(double),
                typeof(DiscountSlider),
                new FrameworkPropertyMetadata(
                    defaultValue: 70.0,
                    propertyChangedCallback: OnMaxDiscountChanged,
                    coerceValueCallback: CoerceMaxDiscount),
                validateValueCallback: ValidateMaxDiscount);

        public double MaxDiscount
        {
            get => (double)GetValue(MaxDiscountProperty);
            set => SetValue(MaxDiscountProperty, value);
        }

        // не позволяем макс. меньше 1% или больше 100%
        private static bool ValidateMaxDiscount(object value)
            => value is double d && !double.IsNaN(d) && !double.IsInfinity(d);

        private static object CoerceMaxDiscount(DependencyObject d, object baseValue)
        {
            double v = (double)baseValue;
            if (v < 1) v = 1;
            if (v > 100) v = 100;
            return v;
        }

        private static void OnMaxDiscountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // Когда меняется верхняя граница, надо принудительно перепроверить Discount.
            // CoerceValue заставляет WPF снова вызвать CoerceDiscount у Discount-проперти.
            d.CoerceValue(DiscountProperty);
        }

        //  DEPENDENCY PROPERTY 3: SuggestedDiscount — рекомендуемая скидка
        //  (информативная, для подсветки, не влияет на Discount напрямую).

        public static readonly DependencyProperty SuggestedDiscountProperty =
            DependencyProperty.Register(
                nameof(SuggestedDiscount),
                typeof(double),
                typeof(DiscountSlider),
                new PropertyMetadata(0.0));

        public double SuggestedDiscount
        {
            get => (double)GetValue(SuggestedDiscountProperty);
            set => SetValue(SuggestedDiscountProperty, value);
        }

      
        //  ROUTED EVENT 1: PreviewDiscountChanged — стратегия Tunneling

        public static readonly RoutedEvent PreviewDiscountChangedEvent =
            EventManager.RegisterRoutedEvent( 
                "PreviewDiscountChanged",
                RoutingStrategy.Tunnel,                              
                typeof(RoutedPropertyChangedEventHandler<double>),  // тип делегата-обработчика
                typeof(DiscountSlider)); // класс-владелец события

        public event RoutedPropertyChangedEventHandler<double> PreviewDiscountChanged // clr-обёртка для туннельного события
        {
            add => AddHandler(PreviewDiscountChangedEvent, value);
            remove => RemoveHandler(PreviewDiscountChangedEvent, value);
        }

        //  ROUTED EVENT 2: DiscountChanged — стратегия Bubbling

        public static readonly RoutedEvent DiscountChangedEvent =
            EventManager.RegisterRoutedEvent(
                "DiscountChanged",
                RoutingStrategy.Bubble,                              
                typeof(RoutedPropertyChangedEventHandler<double>),
                typeof(DiscountSlider));

        public event RoutedPropertyChangedEventHandler<double> DiscountChanged
        {
            add => AddHandler(DiscountChangedEvent, value);
            remove => RemoveHandler(DiscountChangedEvent, value);
        }

        //  ROUTED EVENT 3: DiscountApplied — стратегия Direct

        public static readonly RoutedEvent DiscountAppliedEvent =
            EventManager.RegisterRoutedEvent(
                "DiscountApplied",
                RoutingStrategy.Direct,                                 
                typeof(RoutedEventHandler),
                typeof(DiscountSlider));

        public event RoutedEventHandler DiscountApplied
        {
            add => AddHandler(DiscountAppliedEvent, value);
            remove => RemoveHandler(DiscountAppliedEvent, value);
        }

        public DiscountSlider()
        {
            InitializeComponent();
        }
    }
}
