
namespace Core.Services.SaveService
{
    public interface ISaveService<T> where T : class
    {
        void Save(T data);
        T Load();
        bool HasSave();
        void DeleteSave();
    }
}