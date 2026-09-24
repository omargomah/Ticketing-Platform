using Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.DataSeeding
{
    public static class AddSeedingDataExtensionsOfRoles
    {
        public static void AddSeedingDataOfRoles(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IdentityRole<Guid>>().HasData(
                new IdentityRole<Guid>
                {
                    Id = Guid.Parse("47248ad3-f771-4e70-bb3d-b911502de992"),
                    Name = UserRole.Admin.ToString(),
                    NormalizedName = UserRole.Admin.ToString().ToUpper(),
                    ConcurrencyStamp = "4963c7c6-172b-4c07-b590-b0add57e4802" 
                },
                new IdentityRole<Guid>   
                {
                    Id = Guid.Parse("69d005f7-a503-4a0f-b7f0-90d64f19c445"),
                    Name = UserRole.Attendee.ToString(),
                    NormalizedName = UserRole.Attendee.ToString().ToUpper(),
                    ConcurrencyStamp = "a3f88cf3-9694-4b9b-99cc-f23cecf92e0c"
                },
                new IdentityRole<Guid>
                {
                    Id = Guid.Parse("74ee2599-2a9e-42c5-8071-16cef0080b77"),
                    Name = UserRole.Organizer.ToString(),
                    NormalizedName = UserRole.Organizer.ToString().ToUpper(),
                    ConcurrencyStamp = "13914f17-c84e-418a-9109-fa2d5b96ed82"
                }
            );
        }
    }
}
