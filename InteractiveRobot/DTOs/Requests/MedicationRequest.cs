namespace InteractiveRobot.DTOs.Requests
{
    public class MedicationRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Dosage { get; set; }
        public string? Notes { get; set; }
    }
}
