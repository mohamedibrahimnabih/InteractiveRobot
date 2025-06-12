using System.ComponentModel.DataAnnotations;

namespace InteractiveRobot.DTOs.Requests
{
    public class RegisterRequest
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = null!;

        [Required]
        public string UserName { get; set; } = null!;
        [Required]
        public string Name { get; set; } = null!;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;

        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; } = null!;

        [Required]
        public ApplicationUserType ApplicationUserType { get; set; }
    }
}
