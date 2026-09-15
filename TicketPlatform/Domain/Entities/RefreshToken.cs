namespace Domain.Entities
{
    public class RefreshToken:Entity
    {
        private RefreshToken(Guid id) : base(id) { }

        public string Token { get; private set; } = string.Empty;
        public DateTime ExpiresOn { get; private set; }
        public DateTime CreatedOn { get; private set; }
        public DateTime? RevokedOn { get; private set; }

        public bool IsExpired => DateTime.UtcNow >= ExpiresOn;
        public bool IsActive => RevokedOn == null && !IsExpired;

        public Guid UserId { get; private set; }    
    
        public static RefreshToken Create(Guid userId, DateTime expiresOn)
        {
            return new RefreshToken (Guid.NewGuid())
            {
                Token = Guid.NewGuid().ToString(),
                ExpiresOn = expiresOn,
                CreatedOn = DateTime.UtcNow,
                UserId = userId,
                RevokedOn = null
                
            };
        }
        public void Revoke()
        {
            RevokedOn = DateTime.UtcNow;
        }
        
    }
}
