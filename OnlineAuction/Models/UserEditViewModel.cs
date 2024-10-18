namespace OnlineAuction.Models
{
    public class UserEditViewModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        // Password fields (optional for editing user)
        public string? Password { get; set; }
        public string? ConfirmPassword { get; set; }
    }

}
