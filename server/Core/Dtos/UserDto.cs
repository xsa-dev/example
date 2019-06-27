namespace WebApi.Core.Domain.Dtos {
    public class UserDto {
        // this is view model
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