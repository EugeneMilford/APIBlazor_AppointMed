using System.ComponentModel.DataAnnotations;

namespace AppointMed.API.Models.User
{
    public class UserDto : LoginUserDto
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public string Role { get; set; }
    }
}
