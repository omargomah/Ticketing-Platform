using Domain.Entities;
using Domain.Interfaces.IRepositories;
using Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{

    public class RefreshTokenRepository :Repository<RefreshToken>, IRefreshTokenRepository
    {

        public RefreshTokenRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
        public async Task<RefreshToken?> GetRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default) =>
            await _set.SingleOrDefaultAsync(x => x.Token == refreshToken, cancellationToken);
        public async Task DeleteRefreshTokensByUserIdAsync(Guid userId) => await _set.Where(x => x.UserId == userId).ExecuteDeleteAsync();

    }
}
