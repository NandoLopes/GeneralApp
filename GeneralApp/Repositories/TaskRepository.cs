using GeneralApp.MVVM.Models;
using GeneralApp.MVVM.Models.Helpers;
using GeneralApp.Shared;
using SQLiteNetExtensions.Extensions;

namespace GeneralApp.Repositories
{
    public class TaskRepository : BaseRepository<MyTask>
    {
        public GenericResponse<List<MyTask>> GetItemsWithChildrenByDatePredicate(Func<MyTask, bool> predicate)
        {
            try
            {
                var result = connection.GetAllWithChildren<MyTask>().OrderBy(x => x.DueDate == null)
                                                                    .ThenBy(x => x.DueDate)
                                                                    .ThenBy(x => x.Completed)
                                                                    .Where(predicate).ToList();

                return new GenericResponse<List<MyTask>> { Result = result };
            }
            catch (Exception ex)
            {
                return Helpers.ErrorResponse<List<MyTask>>(ex.Message);
            }
        }
    }
}
