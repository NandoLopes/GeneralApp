using GeneralApp.Abstractions;
using PropertyChanged;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace GeneralApp.MVVM.Models
{
    [AddINotifyPropertyChangedInterface]
    public class MyTask : TableData
    {
        [NotNull, MaxLength(30)]
        public string Name { get; set; }

        public bool Completed { get; set; } = false;

        [ForeignKey(typeof(TaskCategory)), NotNull]
        public int CategoryId { get; set; }

        [ManyToOne]
        public TaskCategory Category { get; set; }

        public string TaskColor { get; set; }
        public bool HasDueDate { get; set; }

        public DateTime? DueDate { get; set; }

        [ManyToMany(typeof(TaskItem),CascadeOperations = CascadeOperation.CascadeRead)]
        public List<StockItem>? Items { get; set; }

        [Ignore]
        public bool IsSelected { get; set; }
    }
}
