using GeneralApp.Abstractions;
using GeneralApp.Interfaces;
using GeneralApp.MVVM.Models.Helpers;
using GeneralApp.Shared;
using GeneralApp.Utilities;
using SQLite;
using SQLiteNetExtensions.Extensions;
using System.Linq.Expressions;

namespace GeneralApp.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T>
        where T : TableData, new()
    {
        SQLiteConnection connection;

        public BaseRepository()
        {
            connection =
                 new SQLiteConnection(Constants.DatabasePath,
                 Constants.Flags);
            connection.CreateTable<T>();
        }

        public GenericResponse<T> DeleteItem(T item)
        {
            try
            {
                connection.Delete(item, true);

                return new GenericResponse<T> { StatusMessage = $"Item deleted.", Result = item };
            }
            catch (Exception ex)
            {
                return Helpers.ErrorResponse<T>(ex.Message);
            }
        }

        public void Dispose()
        {
            connection.Close();
        }

        public GenericResponse<T> GetItem(int id)
        {
            try
            {
                var result = connection.Table<T>().FirstOrDefault(x => x.Id == id);

                return new GenericResponse<T> { Result = result };
            }
            catch (Exception ex)
            {
                return Helpers.ErrorResponse<T>(ex.Message);
            }
        }

        public GenericResponse<T> GetItemWithChildren(int id)
        {
            try
            {
                var result = connection.GetWithChildren<T>(id);

                return new GenericResponse<T> { Result = result };
            }
            catch (Exception ex)
            {
                return Helpers.ErrorResponse<T>(ex.Message);
            }
        }

        public GenericResponse<T> GetItem(Expression<Func<T, bool>> predicate)
        {
            try
            {
                var result = connection.Table<T>()
                     .Where(predicate).FirstOrDefault();

                return new GenericResponse<T> { Result = result };
            }
            catch (Exception ex)
            {
                return Helpers.ErrorResponse<T>(ex.Message);
            }
        }

        public GenericResponse<List<T>> GetItems()
        {
            try
            {
                var result = connection.Table<T>().ToList();

                return new GenericResponse<List<T>> { Result = result };
            }
            catch (Exception ex)
            {
                return Helpers.ErrorResponse<List<T>>(ex.Message);
            }
        }

        public GenericResponse<List<T>> GetItems(Expression<Func<T, bool>> predicate)
        {
            try
            {
                var result = connection.Table<T>().Where(predicate).ToList();

                return new GenericResponse<List<T>> { Result = result };
            }
            catch (Exception ex)
            {
                return Helpers.ErrorResponse<List<T>>(ex.Message);
            }
        }

        public async Task<GenericResponse<T>> SaveItem(T item)
        {
            try
            {
                int result;
                if (item.Id != 0)
                {
                    result = connection.Update(item);

                    return new GenericResponse<T> { StatusMessage = $"{result} row(s) updated", Result = item };
                }
                else
                {
                    result = connection.Insert(item);

                    return new GenericResponse<T> { StatusMessage = $"{result} row(s) updated", Result = item };
                }
            }
            catch (Exception ex)
            {
                return Helpers.ErrorResponse<T>(ex.Message);
            }
        }

        public GenericResponse<T> SaveItemWithChildren(T item, bool recursive = false)
        {
            connection.InsertWithChildren(item, recursive);

            try
            {
                if (item.Id != 0)
                {
                    connection.UpdateWithChildren(item);

                    return new GenericResponse<T> { StatusMessage = $"Update completed!", Result = item };
                }
                else
                {
                    connection.InsertWithChildren(item);

                    return new GenericResponse<T> { StatusMessage = $"Insert completed!", Result = item };
                }
            }
            catch (Exception ex)
            {
                return Helpers.ErrorResponse<T>(ex.Message);
            }
        }

        public GenericResponse<List<T>> GetItemsWithChildren()
        {
            try
            {
                var result = connection.GetAllWithChildren<T>().ToList();

                return new GenericResponse<List<T>> { Result = result };
            }
            catch (Exception ex)
            {
                return Helpers.ErrorResponse<List<T>>(ex.Message);
            }
        }
    }
}
