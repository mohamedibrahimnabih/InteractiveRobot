namespace InteractiveRobot.DTOs.Requests
{
    public class AddRatingRequest
    {
        public string DoctorId { get; set; } = string.Empty;
        public int Stars { get; set; } // 1 to 5
        public string? Comment { get; set; }
    }
}
