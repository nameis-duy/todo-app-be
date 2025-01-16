namespace Domain.Entity
{
#pragma warning disable CS8618
    public class Account
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? Phone { get; set; }
        //
        public ICollection<Tasks> Tasks { get; set; }
    }
}
