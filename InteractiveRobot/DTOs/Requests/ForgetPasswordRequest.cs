using System.ComponentModel.DataAnnotations;

namespace InteractiveRobot.DTOs.Requests
{
    public class ForgetPasswordRequest
    {
        [Required]
        public string EmailOrUserName { get; set; } = string.Empty;
    }
}
