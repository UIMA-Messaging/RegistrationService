using System.ComponentModel.DataAnnotations;
using IdentityService.Keys;

namespace IdentityService.Contracts
{
    public class KeyBundle
    {
        [Required]
        public Key IdentityKey { get; }
        [Required]
        public Key SignedPreKey { get; }
        [Required]
        [MinLength(200)]
        public Key[] OneTimePreKeys { get; }
    }
}
