namespace RegistrationApi.Contracts
{
    public class RegisteredUser
    {
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public string Username { get; set; }
        public string? Image { get; set; }
        public DateTime? JoinedAt { get; set; }
        public DateTime? EditedAt { get; set; }
    }
}
