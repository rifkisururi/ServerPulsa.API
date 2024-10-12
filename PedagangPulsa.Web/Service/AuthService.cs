using PedagangPulsa.Web.Interface;

namespace PedagangPulsa.Web.Service
{
    public class AuthService : IAuthService
    {
        // Contoh implementasi sederhana
        public bool ValidateCredentials(string username, string password)
        {
            // Validasi sederhana (di sini Anda bisa mengganti dengan validasi dari database)
            return username == "user" && password == "password";
        }
    }

}
