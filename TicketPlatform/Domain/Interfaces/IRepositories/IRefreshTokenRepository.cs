using System;
using Domain.Entities;
namespace Domain.Interfaces.IRepositories
{
    public interface IRefreshTokenRepository
    {
        ValueTask AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default);
        Task<RefreshToken?> GetRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
        void Update(RefreshToken refreshToken);
        void Delete(RefreshToken refreshToken);
        Task DeleteRefreshTokensByUserIdAsync(Guid userId);
    }
}
