using Domain.Entities;
using Microsoft.AspNetCore.Identity;
namespace Infrastructure.Identity
{
    public class AppUser:IdentityUser<Guid> 
    {
        private readonly List<RefreshToken> _refreshTokens = new List<RefreshToken>();
        public IReadOnlyCollection<RefreshToken> refreshTokens =>   _refreshTokens.AsReadOnly();
    }
}
