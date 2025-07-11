namespace InteractiveRobot.Models
{
    public class Specialty
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public ICollection<DoctorSpecialty>? DoctorSpecialties { get; set; }
    }
}
