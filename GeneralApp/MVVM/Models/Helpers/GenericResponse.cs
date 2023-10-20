namespace GeneralApp.MVVM.Models.Helpers
{
    public class GenericResponse<T>
    {
        public bool HasError { get; set; } = false;
        public string StatusMessage { get; set; } = string.Empty;
        public T Result { get; set; }
    }
}
