using GeneralApp.Abstractions;
using PropertyChanged;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace GeneralApp.MVVM.Models
{
    [AddINotifyPropertyChangedInterface]
    public class TaskProduct : TableData
    {
        [ForeignKey(typeof(MyTask))]
        public int TaskId { get; set; }

        [ManyToOne]
        public MyTask MyTask { get; set; }

        [ForeignKey(typeof(Product))]
        public int ProductId { get; set; }

        [ManyToOne]
        public Product Product { get; set; }

        [NotNull]
        public int Quantity { get; set; }
    }
}