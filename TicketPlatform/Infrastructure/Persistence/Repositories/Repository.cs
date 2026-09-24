using Domain.Entities;
using Domain.Interfaces.IRepositories;
using Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class Repository<T> : IRepository<T> where T : Entity
    {
        private readonly ApplicationDbContext _dbContext;
        protected readonly DbSet<T> _set ;

        public Repository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            _set = dbContext.Set<T>();
        }
        public async ValueTask AddAsync(T entity, CancellationToken cancellationToken = default) =>
            await _set.AddAsync(entity, cancellationToken);
        public void Update(T entity) => _set.Update(entity);
        public void Delete(T entity) => _set.Remove(entity);

        public Task<T?> GetAsync(Guid id, CancellationToken cancellationToken = default)
         => _set.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
    }
}
