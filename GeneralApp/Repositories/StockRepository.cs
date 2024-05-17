using GeneralApp.MVVM.Models;
using GeneralApp.MVVM.Models.Helpers;
using GeneralApp.Shared;
using SQLiteNetExtensions.Extensions;

namespace GeneralApp.Repositories
{
    public class StockRepository : BaseRepository<StockItem>
    {
        public GenericResponse<List<StockItem>> GetItemsWithChildrenByDatePredicate(Func<StockItem, bool> predicate)
        {
            try
            {
                var result = connection.GetAllWithChildren<StockItem>().OrderBy(x => x.ExpirationDate == null)
                                                                    .ThenBy(x => x.ExpirationDate)
                                                                    .Where(predicate).ToList();

                return new GenericResponse<List<StockItem>> { Result = result };
            }
            catch (Exception ex)
            {
                return Helpers.ErrorResponse<List<StockItem>>(ex.Message);
            }
        }
    }
}
