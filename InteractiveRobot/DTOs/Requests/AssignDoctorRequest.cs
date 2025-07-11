namespace InteractiveRobot.DTOs.Requests
{
    public class AssignDoctorRequest
    {
        public int ChildId { get; set; }
        public string DoctorId { get; set; } = string.Empty;
    }
}
