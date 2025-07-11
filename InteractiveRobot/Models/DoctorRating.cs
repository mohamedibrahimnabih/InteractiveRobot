namespace InteractiveRobot.Models
{
    public class DoctorRating
    {
        public int Id { get; set; }
        public string DoctorId { get; set; } = string.Empty;
        public ApplicationUser? Doctor { get; set; }

        public string ParentId { get; set; } = string.Empty;
        public ApplicationUser? Parent { get; set; }

        public int Stars { get; set; }
        public string? Comment { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow;
    }
}