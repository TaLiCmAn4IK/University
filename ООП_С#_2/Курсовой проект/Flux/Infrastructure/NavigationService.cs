using System;
using System.Collections.Generic;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Flux.Infrastructure;

public interface INavigationService : INotifyPropertyChanged
{
    ViewModelBase? CurrentViewModel { get; }
    bool CanGoBack { get; }

    /// <summary>Navigate, pushing the previous page onto the back stack.</summary>
    void NavigateTo<TViewModel>() where TViewModel : ViewModelBase;
    void NavigateTo(ViewModelBase vm);

    /// <summary>Navigate and clear the back stack (e.g., after login or logout).</summary>
    void NavigateRoot<TViewModel>() where TViewModel : ViewModelBase;
    void NavigateRoot(ViewModelBase vm);

    /// <summary>Pop the last page off the stack and display it. ничего не делает if stack is empty.</summary>
    void GoBack();
}

public class NavigationService : ObservableObject, INavigationService
{
    private readonly IServiceProvider _sp;
    private readonly Stack<ViewModelBase> _back = new();
    private ViewModelBase? _currentViewModel;

    public ViewModelBase? CurrentViewModel
    {
        get => _currentViewModel;
        private set
        {
            if (SetProperty(ref _currentViewModel, value))
                OnPropertyChanged(nameof(CanGoBack));
        }
    }

    public bool CanGoBack => _back.Count > 0;

    public NavigationService(IServiceProvider sp) { _sp = sp; }

    public void NavigateTo<TViewModel>() where TViewModel : ViewModelBase
        => NavigateTo((TViewModel)_sp.GetService(typeof(TViewModel))!);

    public void NavigateTo(ViewModelBase vm)
    {
        if (_currentViewModel != null) _back.Push(_currentViewModel);
        CurrentViewModel = vm;
        OnPropertyChanged(nameof(CanGoBack));
    }

    public void NavigateRoot<TViewModel>() where TViewModel : ViewModelBase
        => NavigateRoot((TViewModel)_sp.GetService(typeof(TViewModel))!);

    public void NavigateRoot(ViewModelBase vm)
    {
        _back.Clear();
        CurrentViewModel = vm;
        OnPropertyChanged(nameof(CanGoBack));
    }

    public void GoBack()
    {
        if (_back.Count == 0) return;
        CurrentViewModel = _back.Pop();
        OnPropertyChanged(nameof(CanGoBack));
    }
}
