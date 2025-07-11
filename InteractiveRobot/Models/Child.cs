namespace InteractiveRobot.Models
{
    public class Child
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Condition { get; set; } = string.Empty;

        // Parent Relationship
        public string ParentId { get; set; } = string.Empty;
        public ApplicationUser? Parent { get; set; }

        // Assigned Doctor
        public string? DoctorId { get; set; }
        public ApplicationUser? Doctor { get; set; }

        public ICollection<Diagnosis>? Diagnoses { get; set; } 
    }
}