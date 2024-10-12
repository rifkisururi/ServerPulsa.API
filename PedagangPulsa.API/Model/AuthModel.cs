using System.ComponentModel.DataAnnotations;

namespace PedagangPulsa.API.Model
{
    public class AuthModel
    {
    }
    public class LoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class TokenModel
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }

    public class RegisterModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string Password { get; set; }
        [Required]
        public string NamaLengkap { get; set; }
        [Required]
        public string Alamat { get; set; }
        [Required]
        public string NoWA { get; set; }
        [Required]
        public string Email { get; set; }
        public string Telegram { get; set; }
        [Required]
        [MinLength(6, ErrorMessage = "Pin Transaksi must be at least 6 characters long.")]
        public string PinTransaksi { get; set; } = "000000";
        
    }

}
