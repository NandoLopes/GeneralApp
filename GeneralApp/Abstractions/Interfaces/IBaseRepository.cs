using GeneralApp.Abstractions;
using GeneralApp.MVVM.Models.Helpers;
using System.Linq.Expressions;

namespace GeneralApp.Interfaces
{
    public interface IBaseRepository<T> : IDisposable
        where T : TableData, new()
    {
        Task<GenericResponse<T>> SaveItem(T item);
        GenericResponse<T> SaveItemWithChildren(T item, bool recursive = false);
        GenericResponse<T> GetItem(int id);
        GenericResponse<T> GetItemWithChildren(int id);
        GenericResponse<T> GetItem(Expression<Func<T, bool>> predicate);
        GenericResponse<List<T>> GetItems();
        GenericResponse<List<T>> GetItems(Expression<Func<T, bool>> predicate);
        GenericResponse<List<T>> GetItemsWithChildren();
        GenericResponse<T> DeleteItem(T item);
    }
}
