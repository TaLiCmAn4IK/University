using System.Windows.Input;

namespace WatchShop.Commands
{
    /// <summary>
    /// Пользовательские команды на основе RoutedUICommand.
    /// 
    /// RoutedUICommand отличается от обычной ICommand тем, что:
    ///  - имеет Text (отображаемое имя — для меню/тулбаров)
    ///  - имеет InputGestures (горячие клавиши) — встроенная привязка
    ///  - маршрутизируется по визуальному дереву (как RoutedEvent), поэтому
    ///    обработчики (CommandBinding) можно ставить на любом уровне UI.
    /// </summary>
    public static class AppCommands
    {
        /// <summary>
        /// Команда "О программе". Показывает диалог с информацией.
        /// Привязана к F1.
        /// </summary>
        public static readonly RoutedUICommand ShowAbout = new RoutedUICommand(
            text: "О программе",                    // отображаемый текст
            name: "ShowAbout",                       // ключ для XAML
            ownerType: typeof(AppCommands),
            inputGestures: new InputGestureCollection
            {
                new KeyGesture(Key.F1)               // горячая клавиша
            });

        /// <summary>
        /// Команда "Применить рекомендованную скидку 15%".
        /// Только для админа на форме редактирования. Привязана к Ctrl+D.
        /// </summary>
        public static readonly RoutedUICommand ApplyRecommendedDiscount = new RoutedUICommand(
            text: "Применить скидку 15%",
            name: "ApplyRecommendedDiscount",
            ownerType: typeof(AppCommands),
            inputGestures: new InputGestureCollection
            {
                new KeyGesture(Key.D, ModifierKeys.Control)
            });
    }
}
