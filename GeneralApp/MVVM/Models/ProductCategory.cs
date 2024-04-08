using GeneralApp.Abstractions;
using PropertyChanged;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace GeneralApp.MVVM.Models
{
    [AddINotifyPropertyChangedInterface]
    public class ProductCategory : TableData
    {
        [NotNull, MaxLength(30)]
        public string Name { get; set; }

        public string Color { get; set; }

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<StockItem>? StockItems { get; set; }

        [Ignore]
        public bool IsSelected { get; set; }

        [Ignore]
        public int StockCount {
            get { return (StockItems != null ? StockItems.Count : 0); }
        }
    }
}
