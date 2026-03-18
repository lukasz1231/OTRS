namespace otrs_backend.Responses
{
    public class UserProfileResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public DateTime BirthDate { get; set; }
        public string Bio { get; set; }
        public string AvatarUrl { get; set; }
        public List<string> Roles { get; set; }
        public List<string> Queues { get; set; }
    }
}