using System.ComponentModel.DataAnnotations;

namespace UserService.Contracts
{
    public class ExchangeKeys
    {
        [RegularExpression(@"\b[0-9a-fA-F]+\b", ErrorMessage = "Invalid hex string.")]
        public string IdentityKey { get; }
        [RegularExpression(@"\b[0-9a-fA-F]+\b", ErrorMessage = "Invalid hex string.")]
        public string SignedPreKey { get; }
        [MinLength(200, ErrorMessage = "A minimum of 200 key bundles is required.")]
        [RegularExpression(@"\b[0-9a-fA-F]+\b", ErrorMessage = "Invalid hex string.")]
        public string[] OneTimePreKeys { get; }
    }
}
