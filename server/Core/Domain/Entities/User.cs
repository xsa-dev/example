namespace WebApi.Core.Domain.Entities {
    public class User {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public bool isVerified { get; set; }
        public string VerificationCode { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
    }
}