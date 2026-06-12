namespace LoyaltyPoints.Models
{
    public class User
    {
        //data structure
        public int Id { get; set; }

        public string Username { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public int Points { get; set; }
    }
}