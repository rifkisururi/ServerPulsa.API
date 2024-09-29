using Microsoft.IdentityModel.Tokens;
using PedagangPulsa.API.Database.Entity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace PedagangPulsa.API.Service
{
    public class TokenService
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TokenService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        public string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            string jwtKey = _configuration["Jwt:Key"];
            var key = Encoding.ASCII.GetBytes(jwtKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username)
                }),
                Expires = DateTime.UtcNow.AddMinutes(15),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }
        public bool ValidateTokenExpiration(ClaimsPrincipal user, out string? userId, out string? username, out string? token, out string? expirationClaim)
        {
            // Mengambil header Authorization dari request
            var bearerToken = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString();
            token = bearerToken.StartsWith("Bearer ") ? bearerToken.Substring(7) : bearerToken;

            userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            username = user.FindFirst(ClaimTypes.Name)?.Value;
            expirationClaim = user.FindFirst("exp")?.Value;

            // Memastikan bahwa klaim user dan expiration valid
            if (expirationClaim == null || userId == null || username == null)
            {
                return false;
            }

            // Konversi expiration time dari epoch ke DateTime
            var expirationTime = DateTimeOffset.FromUnixTimeSeconds(long.Parse(expirationClaim)).UtcDateTime;

            // Memeriksa apakah token sudah kedaluwarsa
            return DateTime.UtcNow <= expirationTime;
        }
    }

}
