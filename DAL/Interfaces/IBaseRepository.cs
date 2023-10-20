namespace LostButFound.API.DAL.Interfaces
{
    public interface IBaseRepository<T>
    {
        Task<T> Get(int id);

        Task<bool> Create(T entity);

        Task<List<T>> Select();
        
        Task<bool> Delete(T entity);
    }
}
