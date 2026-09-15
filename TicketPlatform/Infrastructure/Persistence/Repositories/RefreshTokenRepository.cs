using Domain.Entities;
using Domain.Interfaces.IRepositories;
using Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class RefreshTokenRepository :IRefreshTokenRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly DbSet<RefreshToken> _refreshTokens;

        public RefreshTokenRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            _refreshTokens = dbContext.RefreshTokens;
        }
        public async ValueTask AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default) =>
            await _refreshTokens.AddAsync(refreshToken, cancellationToken);
        public async Task<RefreshToken?> GetRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default) =>
            await _refreshTokens.SingleOrDefaultAsync(x => x.Token == refreshToken, cancellationToken);
        public void Update(RefreshToken refreshToken) => _refreshTokens.Update(refreshToken);
        public void Delete(RefreshToken refreshToken) => _refreshTokens.Remove(refreshToken);
        public async Task DeleteRefreshTokensByUserIdAsync(Guid userId) => await _refreshTokens.Where(x => x.UserId == userId).ExecuteDeleteAsync();

    }
}
