using Domain.Entities;
using Domain.Interfaces.IRepositories;
using Infrastructure.Persistence.Data;

namespace Infrastructure.Persistence.Repositories
{
    public class OrganizerRepository : Repository<Organizer>, IOrganizerRepository
    {
        public OrganizerRepository(ApplicationDbContext dbContext) : base(dbContext)
        {            
        }
    }
}
