namespace InteractiveRobot.Models
{
    public class PasswordResetCode
    {
        public int Id { get; set; }

        public string ApplicationUserId { get; set; } = string.Empty;
        public ApplicationUser? ApplicationUser { get; set; }

        public string Code { get; set; } = string.Empty;
        public DateTime ExpirationCode { get; set; }
    }
}
