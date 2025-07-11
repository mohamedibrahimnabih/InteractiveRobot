using System.ComponentModel.DataAnnotations;

namespace InteractiveRobot.DTOs.Requests
{
    public class ResendEmailRequest
    {
        [Required]
        public string EmailOrUserName { get; set; } = string.Empty;
    }
}
