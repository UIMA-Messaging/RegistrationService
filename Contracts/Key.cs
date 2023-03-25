namespace RegistrationApi.Keys
{
    public class Key
    {
        public string Value { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public bool IsValid { get { return ExpirationDate > DateTime.Now; } }
    }
}
