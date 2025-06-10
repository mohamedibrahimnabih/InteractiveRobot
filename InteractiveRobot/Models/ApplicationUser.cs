using Microsoft.AspNetCore.Identity;

namespace InteractiveRobot.Models
{
    public enum ApplicationUserType
    {
        Parent,
        Doctor
    }

    public class ApplicationUser : IdentityUser
    {
        public ApplicationUserType ApplicationUserType { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
