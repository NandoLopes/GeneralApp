using GeneralApp.Abstractions;
using PropertyChanged;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace GeneralApp.MVVM.Models
{
    [AddINotifyPropertyChangedInterface]
    public class TaskStock : TableData
    {
        [ForeignKey(typeof(MyTask))]
        public int TaskId { get; set; }

        [ManyToOne]
        public MyTask MyTask { get; set; }

        [ForeignKey(typeof(StockItem))]
        public int ItemId { get; set; }

        [ManyToOne]
        public StockItem Item { get; set; }

        [NotNull]
        public int Quantity { get; set; }
    }
}