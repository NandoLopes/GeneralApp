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
    }
}
