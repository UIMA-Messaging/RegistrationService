using System.ComponentModel.DataAnnotations;
using IdentityService.Contracts;

namespace IdentityService.Keys
{
    public class Key
    {
        [Required]
        public string Value { get; set; }
        [Required]
        public DateTime CreationDate { get; set; }
        [Required]
        public DateTime ExpirationDate { get; set; }
        public bool IsValid { get { return ExpirationDate > DateTime.Now; } }
    }
}
