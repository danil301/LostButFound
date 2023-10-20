using System.ComponentModel.DataAnnotations;

namespace LostButFound.API.Domian.ViewModels
{
    public class UserViewModel
    {
        [Required]
        public string FullName { get; set; }

        [Required]
        public string Login { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

    }
}
