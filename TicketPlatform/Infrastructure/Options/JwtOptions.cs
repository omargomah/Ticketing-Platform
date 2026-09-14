namespace Infrastructure.Options
{
    public class JwtOptions
    {
        public string Audience { get; set; }
        public string Issuer { get; set; }
        public int AccessTokenExpireAfterMinutes { get; set; }
        public int RefreshTokenExpireAfterDays { get; set; }
        public string SecretKey { get; set; }
    }

}
