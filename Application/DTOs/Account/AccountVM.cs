namespace Application.DTOs.Account
{
#pragma warning disable CS8618
    public class AccountVM
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? Phone { get; set; }
    }
}
