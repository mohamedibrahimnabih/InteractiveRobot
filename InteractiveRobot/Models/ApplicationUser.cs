using Microsoft.AspNetCore.Identity;

namespace InteractiveRobot.Models
{
    public enum UserType
    {
        Parent,
        Doctor,
        Admin,
        SuperAdmin
    }

    public class ApplicationUser : IdentityUser
    {
        public UserType UserType { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsBanned { get; set; } = false;

        // Navigation
        public ICollection<Child>? Children { get; set; }
        public ICollection<DoctorRating>? DoctorRatings { get; set; }
        public ICollection<DoctorSpecialty>? DoctorSpecialties { get; set; }
    }
}
