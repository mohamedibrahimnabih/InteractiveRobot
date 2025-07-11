namespace InteractiveRobot.Models
{
    public class Diagnosis
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Child Relationship
        public int ChildId { get; set; }
        public Child? Child { get; set; }
    }
}