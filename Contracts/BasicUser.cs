using System.ComponentModel.DataAnnotations;

namespace UserService.Contracts
{
    public class BasicUser
    {
        [MinLength(5)]
        [MaxLength(30)]
        [RegularExpression(@"^[a-zA-Z0-9]+$")]
        public string DisplayName { get; set; }
        [Url]
        public string? Image { get; set; }
        public ExchangeKeys ExchangeKeys { get; set; }
    }
}
