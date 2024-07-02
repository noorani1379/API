namespace API.Models.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; } = true;
        public ICollection<UserToken> UserTokens { get; set; }
    }

    public class UserToken
    {
        public int Id { get; set; }
        public string TokenHash { get; set; }
        public DateTime TokenExp { get; set; }
        public string MobileModel { get; set; }

        public User User { get; set; }
        public Guid UserId { get; set; }
    }
}
