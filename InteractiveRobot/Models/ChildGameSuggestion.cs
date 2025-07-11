namespace InteractiveRobot.Models
{
    public class ChildGameSuggestion
    {
        public int Id { get; set; }

        public int ChildId { get; set; }
        public Child? Child { get; set; }

        public int SuggestedGameId { get; set; }
        public SuggestedGame? SuggestedGame { get; set; }

        public DateTime SuggestedAt { get; set; } = DateTime.UtcNow;

        public string? DoctorId { get; set; }
        public ApplicationUser? Doctor { get; set; }
    }
}
