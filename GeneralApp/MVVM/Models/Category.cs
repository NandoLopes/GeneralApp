using PropertyChanged;

namespace GeneralApp.MVVM.Models
{
    [AddINotifyPropertyChangedInterface]
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public int PendingTasks{ get; set; }
        public float Percentage { get; set; }
        public bool IsSelected { get; set; }
    }
}
