namespace Application.DTOs.Account
{
#pragma warning disable CS8618
    public class AccountChangePasswordRequest
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
