using System;
using Domain.Entities;
namespace Domain.Interfaces.IRepositories
{
    public interface IRefreshTokenRepository : IRepository<RefreshToken>
    {
        Task<RefreshToken?> GetRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
        Task DeleteRefreshTokensByUserIdAsync(Guid userId);
    }
}
