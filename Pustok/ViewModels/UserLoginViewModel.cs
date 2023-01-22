using Microsoft.Build.Framework;
using System.ComponentModel.DataAnnotations;
using RequiredAttribute = System.ComponentModel.DataAnnotations.RequiredAttribute;

namespace Pustok.ViewModels
{
    public class UserLoginViewModel
    {
        [Required]
        [StringLength(maximumLength: 30)]
        public string Username { get; set;}
        [Required]
        [StringLength(maximumLength: 30,MinimumLength =8),DataType(DataType.Password)]
        public string Password { get; set;}
    }
}
