using System.ComponentModel.DataAnnotations;

namespace RegistrationService.Contracts
{
    public class ExchangeKeys
    {
        public string? UserId { get; set; }
        [RegularExpression(@"\b[0-9a-fA-F]+\b", ErrorMessage = "Invalid hex string.")]
        public string IdentityKey { get; set; }
        [RegularExpression(@"\b[0-9a-fA-F]+\b", ErrorMessage = "Invalid hex string.")]
        public string SignedPreKey { get; set; }
        [MinLength(200, ErrorMessage = "A minimum of 200 key bundles is required.")]
        public string[] OneTimePreKeys { get; set; }
        public string Signature { get; set; }
    }
}
