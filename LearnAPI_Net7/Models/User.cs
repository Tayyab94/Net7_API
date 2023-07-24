namespace LearnAPI_Net7.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public bool isActive { get; set; }
        public string role { get; set; }
    }
}
