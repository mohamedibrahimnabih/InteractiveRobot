namespace InteractiveRobot.Models
{
    public class DoctorSpecialty
    {
        public string DoctorId { get; set; } = string.Empty;
        public ApplicationUser? Doctor { get; set; }

        public int SpecialtyId { get; set; }
        public Specialty? Specialty { get; set; }
    }
}
