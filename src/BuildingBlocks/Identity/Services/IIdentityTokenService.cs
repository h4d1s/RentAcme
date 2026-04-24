namespace Identity.Services;

public interface IIdentityTokenService
{
    Task<string> GetValidTokenAsync();
}
