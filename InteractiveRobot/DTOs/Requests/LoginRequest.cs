using System.ComponentModel.DataAnnotations;

namespace InteractiveRobot.DTOs.Requests
{
    public class LoginRequest
    {
        [Required]
        public string EmailOrUserName { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; }
    }
}
