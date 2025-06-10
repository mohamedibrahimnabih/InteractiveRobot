using InteractiveRobot.Models;

namespace InteractiveRobot.DTOs.Request
{
    public class ApplicationUserRequest
    {
        public string Name { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
        public ApplicationUserType ApplicationUserType { get; set; }
    }
}
