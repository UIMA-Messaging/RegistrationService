using System.ComponentModel.DataAnnotations;
using IdentityService.Contracts;

namespace IdentityService.Keys
{
    public class Key
    {
        [Required]
        public DateTime CreationDate { get; }
        [Required]
        public string Value { get; }
        [Required]
        public KeyType Type { get; }
        public bool IsValid { get { return CreationDate > DateTime.Now; } }

        public Key(DateTime creationDate, string value, KeyType type)
        {
            CreationDate = creationDate;
            Value = value;
            Type = type;    
        }
    }
}
