namespace PedagangPulsa.Web.Interface
{
    public interface IAuthService
    {
        bool ValidateCredentials(string username, string password);

    }
}
