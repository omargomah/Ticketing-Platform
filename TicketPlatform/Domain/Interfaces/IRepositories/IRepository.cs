using Domain.Entities;
namespace Domain.Interfaces.IRepositories
{
    public interface IRepository<T> where T : Entity 
    {
        ValueTask AddAsync(T entity, CancellationToken cancellationToken = default);
        Task<T?> GetAsync(Guid id, CancellationToken cancellationToken = default);
        void Update(T entity);
        void Delete(T entity);
    }
}
