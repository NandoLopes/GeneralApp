using PropertyChanged;

namespace GeneralApp.MVVM.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public partial class ViewModelBase : IQueryAttributable
    {
        public string Title { get; set; }

        public virtual Task OnNavigatingTo(object? parameter)
           => Task.CompletedTask;

        public virtual Task OnNavigatedFrom(bool isForwardNavigation)
            => Task.CompletedTask;

        public virtual Task OnNavigatedTo()
            => Task.CompletedTask;

        public virtual void ApplyQueryAttributes(IDictionary<string, object> query)
        {

        }
    }
}
