using GeneralApp.Abstractions;
using PropertyChanged;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace GeneralApp.MVVM.Models
{
    [AddINotifyPropertyChangedInterface]
    public class Product : TableData
    {
        [NotNull, MaxLength(30)]
        public string Name { get; set; }

        [ManyToMany(typeof(TaskProduct))]
        public List<MyTask>? MyTasks { get; set; }
    }
}
