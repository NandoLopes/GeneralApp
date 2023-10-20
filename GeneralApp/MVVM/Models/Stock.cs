using GeneralApp.Abstractions;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace GeneralApp.MVVM.Models
{
    public class Stock : TableData
    {
        [ForeignKey(typeof(Product))]
        public int ProductId { get; set; }

        [ManyToOne]
        public Product Product { get; set; }

        public DateTime? ExpirationDate { get; set; }

        [MaxLength(30)]
        public string Details { get; set; }
    }
}
