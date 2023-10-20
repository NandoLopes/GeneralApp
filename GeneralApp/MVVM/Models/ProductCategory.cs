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

        [OneToMany]
        public List<Product> Products { get; set; }
    }
}
