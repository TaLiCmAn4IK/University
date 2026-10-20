using System;
using System.Windows.Input;
using WatchShop.Commands;
using WatchShop.Models;

namespace WatchShop.ViewModels
{
    /// <summary>
    /// ViewModel для одного товара (аналог BookViewModel из лаб. 11).
    /// Связывает модель Watch с представлением и командами «Выдать» / «Забрать».
    /// </summary>
    public class WatchViewModel : ViewModelBase
    {
        private readonly Action<Watch>? _onStockChanged;
        private DelegateCommand? _giveItemCommand;
        private DelegateCommand? _getItemCommand;

        public WatchViewModel(Watch watch, Action<Watch>? onStockChanged = null)
        {
            Watch = watch ?? throw new ArgumentNullException(nameof(watch));
            _onStockChanged = onStockChanged;
            watch.PropertyChanged += (_, e) => ForwardPropertyChanged(e.PropertyName);
        }

        public Watch Watch { get; }

        public string ShortName
        {
            get => Watch.ShortName;
            set
            {
                if (Watch.ShortName == value) return;
                Watch.ShortName = value;
                OnPropertyChanged();
            }
        }

        public string Manufacturer
        {
            get => Watch.Manufacturer;
            set
            {
                if (Watch.Manufacturer == value) return;
                Watch.Manufacturer = value;
                OnPropertyChanged();
            }
        }

        public int Quantity
        {
            get => Watch.Quantity;
            set
            {
                if (Watch.Quantity == value) return;
                Watch.Quantity = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(AvailableText));
                OnPropertyChanged(nameof(InStockDisplay));
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public string CategoryDisplay => Watch.CategoryDisplay;
        public string InStockDisplay => Watch.InStockDisplay;
        public string AvailableText => Watch.AvailableText;
        public double Rating => Watch.Rating;
        public decimal Price => Watch.Price;
        public double Discount => Watch.Discount;
        public bool InStock => Watch.InStock;
        public byte[]? ImageData => Watch.ImageData;

        public ICommand GiveItemCommand =>
            _giveItemCommand ??= new DelegateCommand(GiveItem, CanGiveItem);

        public ICommand GetItemCommand =>
            _getItemCommand ??= new DelegateCommand(GetItem);

        private void GiveItem()
        {
            if (!CanGiveItem()) return;
            Quantity--;
            if (Quantity == 0)
                Watch.InStock = false;
            _onStockChanged?.Invoke(Watch);
        }

        private void GetItem()
        {
            Quantity++;
            if (!Watch.InStock)
                Watch.InStock = true;
            _onStockChanged?.Invoke(Watch);
        }

        private bool CanGiveItem() => Quantity > 0;

        public void RefreshDisplayProperties()
        {
            Watch.RefreshLocalization();
            OnPropertyChanged(nameof(CategoryDisplay));
            OnPropertyChanged(nameof(InStockDisplay));
            OnPropertyChanged(nameof(AvailableText));
        }

        private void ForwardPropertyChanged(string? propertyName)
        {
            if (string.IsNullOrEmpty(propertyName)) return;

            switch (propertyName)
            {
                case nameof(Watch.ShortName):
                    OnPropertyChanged(nameof(ShortName));
                    break;
                case nameof(Watch.Manufacturer):
                    OnPropertyChanged(nameof(Manufacturer));
                    break;
                case nameof(Watch.Quantity):
                    OnPropertyChanged(nameof(Quantity));
                    OnPropertyChanged(nameof(AvailableText));
                    CommandManager.InvalidateRequerySuggested();
                    break;
                case nameof(Watch.InStock):
                    OnPropertyChanged(nameof(InStock));
                    OnPropertyChanged(nameof(InStockDisplay));
                    break;
                case nameof(Watch.Category):
                    OnPropertyChanged(nameof(CategoryDisplay));
                    break;
                case nameof(Watch.Rating):
                    OnPropertyChanged(nameof(Rating));
                    break;
                case nameof(Watch.Price):
                    OnPropertyChanged(nameof(Price));
                    break;
                case nameof(Watch.Discount):
                    OnPropertyChanged(nameof(Discount));
                    break;
                case nameof(Watch.ImageData):
                    OnPropertyChanged(nameof(ImageData));
                    break;
            }
        }
    }
}
