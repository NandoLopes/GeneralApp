using PropertyChanged;

namespace GeneralApp.MVVM.Models
{
    [AddINotifyPropertyChangedInterface]
    public class MyTask
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Completed { get; set; }
        public int CategoryId { get; set; }
        public string TaskColor { get; set; }
    }
}
