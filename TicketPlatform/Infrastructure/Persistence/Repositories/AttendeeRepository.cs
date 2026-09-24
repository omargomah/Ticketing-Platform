using Domain.Entities;
using Domain.Interfaces.IRepositories;
using Infrastructure.Persistence.Data;

namespace Infrastructure.Persistence.Repositories
{
    public class AttendeeRepository : Repository<Attendee>, IAttendeeRepository
    {
        public AttendeeRepository(ApplicationDbContext dbContext) : base(dbContext)
        {            
        }
    }
}
