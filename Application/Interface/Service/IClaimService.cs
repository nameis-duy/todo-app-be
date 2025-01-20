namespace Application.Interface.Service
{
    public interface IClaimService
    {
        int GetCurrentUserId();
        string GetClaim(string claimType);
    }
}
