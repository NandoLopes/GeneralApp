using GeneralApp.Abstractions;
using PropertyChanged;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace GeneralApp.MVVM.Models
{
    [AddINotifyPropertyChangedInterface]
    public class StockItem : TableData
    {
        [Unique, NotNull, MaxLength(30)]
        public string Product { get; set; }

        [ForeignKey(typeof(ProductCategory))]
        public int ProductCategoryId { get; set; }

        [ManyToOne]
        public ProductCategory ProductCategory { get; set; }

        public bool Expires { get; set; } = false;

        public DateTime? ExpirationDate { get; set; }

        [NotNull]
        public int Quantity { get; set; }

        [ManyToMany(typeof(TaskStock), CascadeOperations = CascadeOperation.CascadeRead)]
        public List<MyTask>? MyTasks { get; set; }
    }
}
