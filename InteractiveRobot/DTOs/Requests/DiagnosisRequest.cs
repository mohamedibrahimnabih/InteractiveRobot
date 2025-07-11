namespace InteractiveRobot.DTOs.Requests
{
    public class DiagnosisRequest
    {
        public int ChildId { get; set; }
        public string Title { get; set; } = string.Empty;
    }
}
