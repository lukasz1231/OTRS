namespace otrs_backend.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateOnly BirthDate { get; set; }
        public string Bio { get; set; }
        public string AvatarUrl { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
    }
}
