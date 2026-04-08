using ServiceFlow.Class.Models;

namespace ServiceFlow.Class.Repositories
{
    public interface IRepository <T> where T : IEntity
    {
        Task<int> Create(T IEntity);
        Task Delete(int id);
        Task<ICollection<T>> GetAll();
        Task<T?> GetById(int id);
        Task Update(T entity);

    }
}
