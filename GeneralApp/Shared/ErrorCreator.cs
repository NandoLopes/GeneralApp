using GeneralApp.MVVM.Models.Helpers;

namespace GeneralApp.Shared
{
    public static class Helpers
    {
        public static GenericResponse<T> ErrorResponse<T>(string exMessage) where T : class
        {
            return new GenericResponse<T> { HasError = true, StatusMessage = exMessage, Result = null };
        }
    }
}
