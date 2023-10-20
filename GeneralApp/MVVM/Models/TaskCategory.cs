using GeneralApp.Abstractions;
using PropertyChanged;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace GeneralApp.MVVM.Models
{
    [AddINotifyPropertyChangedInterface]
    public class TaskCategory : TableData
    {
        [NotNull, MaxLength(30)]
        public string Name { get; set; }

        public string Color { get; set; }

        public int PendingTasks{ get; set; }

        public float Percentage { get; set; }

        [Ignore]
        public bool IsSelected { get; set; }

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<MyTask>? MyTasks { get; set; }
    }
}
