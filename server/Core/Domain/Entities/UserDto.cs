namespace WebApi.Core.Domain.Entities {
    public class UserDto {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }

        public bool isVerified { get; set; }

        public string Password { get; set; }
        public string label {
            get {
                return Username;
            }
            set {

            }
        }
    }
}