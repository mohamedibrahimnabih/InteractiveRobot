namespace InteractiveRobot.Models
{
    public class SuggestedGame
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Condition { get; set; } = string.Empty; // e.g ADHD, Autism
        public string Description { get; set; } = string.Empty;
    }
}
