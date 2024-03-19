using GeneralApp.Abstractions.Interfaces;
using PropertyChanged;

namespace GeneralApp.MVVM.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public partial class ViewModelBase
    {
        public string Title { get; set; }

        public virtual Task OnNavigatingTo(object? parameter)
           => Task.CompletedTask;

        public virtual Task OnNavigatedFrom(bool isForwardNavigation)
            => Task.CompletedTask;

        public virtual Task OnNavigatedTo()
            => Task.CompletedTask;

        public bool IsRefreshing { get; set; }

        public bool _canExecuteCommands { get; set; }
        public bool CanExecuteCommands() { return _canExecuteCommands; }
    }
}
