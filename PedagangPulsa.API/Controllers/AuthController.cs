using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using PedagangPulsa.API.Database;
using PedagangPulsa.API.Database.Entity;
using PedagangPulsa.API.Model;
using PedagangPulsa.API.Service;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace PedagangPulsa.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly TokenService _tokenService; 
        private readonly HttpClient _httpClient;

        public AuthController(AppDbContext context, TokenService tokenService, HttpClient httpClient)
        {
            _context = context;
            _tokenService = tokenService; 
            _httpClient = httpClient;
        }

        [HttpGet("ip-info")]
        public async Task<IActionResult> GetIpInfo()
        {
            // Get the user's IP address from the request
            var userIpAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

            // Get the server IP address
            var serverIpAddress = Dns.GetHostEntry(Dns.GetHostName())
                .AddressList
                .FirstOrDefault(ip => ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)?
                .ToString();

            // Get the public IP address from ipify
            string publicIp = null;
            try
            {
                var response = await _httpClient.GetStringAsync("https://api.ipify.org/?format=json");
                var jsonResponse = JObject.Parse(response);
                publicIp = jsonResponse["ip"].ToString();
            }
            catch
            {
                publicIp = "Unable to retrieve public IP";
            }

            return Ok(new
            {
                UserIpAddress = userIpAddress,
                ServerIpAddress = serverIpAddress,
                PublicIpAddress = publicIp
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await _context.Users.SingleOrDefaultAsync(x => x.Username == model.Username);

            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
                return Unauthorized("Invalid username or password");

            var jwtToken = _tokenService.GenerateJwtToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Token = jwtToken,
                RefreshToken = refreshToken
            });
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenModel model)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.RefreshToken == model.RefreshToken);

            if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                return Unauthorized("Invalid or expired refresh token");

            var newJwtToken = _tokenService.GenerateJwtToken(user);
            var newRefreshToken = _tokenService.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Token = newJwtToken,
                RefreshToken = newRefreshToken
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Cek apakah username sudah terdaftar
            var existingUser = await _context.Users.SingleOrDefaultAsync(x => x.Username == model.Username);
            if (existingUser != null)
                return BadRequest("Username already exists");

            // Hash password menggunakan BCrypt
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);

            // Buat user baru
            var user = new User
            {
                Username = model.Username,
                PasswordHash = hashedPassword,
                NamaLengkap = model.NamaLengkap,
                Alamat = model.Alamat,
                NoWA = model.NoWA,
                Email = model.Email,
                Telegram = model.Telegram,
                PinTransaksi = model.PinTransaksi,
            };

            // Simpan user ke database
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok("User registered successfully");
        }

        // Endpoint untuk memeriksa apakah token valid
        [HttpGet("validate-token")]
        [Authorize] // Hanya bisa diakses oleh pengguna dengan token yang valid
        public IActionResult ValidateToken()
        {
            // Jika token valid, kembalikan OK
            var bearerToken = Request.Headers["Authorization"].ToString();
            var token = bearerToken.StartsWith("Bearer ") ? bearerToken.Substring(7) : bearerToken;

            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var username = HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;
            // Mendapatkan klaim expiration (exp)
            var expirationClaim = HttpContext.User.FindFirst("exp")?.Value;

            if (expirationClaim == null || userId == null || username == null)
            {
                return Unauthorized(new { Message = "Data tidak valid" });
            }
            // Konversi expiration time dari epoch ke DateTime
            var expirationTime = DateTimeOffset.FromUnixTimeSeconds(long.Parse(expirationClaim)).UtcDateTime;

            // Memeriksa apakah token sudah kedaluwarsa
            if (DateTime.UtcNow > expirationTime)
            {
                return Unauthorized(new { Message = "Sesi kadarluarsa" });
            }

            return Ok(new { 
                Message = "Token is valid",
                Id = userId,
                username = username,
                token = token
            });
        }


        // Endpoint untuk mengecek apakah token sudah kadaluarsa atau tidak
        [HttpPost("check-expiry")]
        public IActionResult CheckTokenExpiry([FromBody] string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return BadRequest("Token is required");

            var tokenHandler = new JwtSecurityTokenHandler();
            //var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]); // Secret key dari konfigurasi
            var key = Encoding.UTF8.GetBytes("your_jwt_secret_key_hereyour_jwt_secret_key_here"); // Secret key dari konfigurasi
            try
            {
                // Validasi token dan cek waktu kadaluarsa
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero // Tidak ada toleransi waktu tambahan untuk validasi waktu
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var expiryDate = jwtToken.ValidTo;

                if (expiryDate < DateTime.UtcNow)
                {
                    return Unauthorized(new { Message = "Token has expired" });
                }

                return Ok(new { Message = "Token is still valid", ExpiryDate = expiryDate });
            }
            catch (SecurityTokenExpiredException)
            {
                return Unauthorized(new { Message = "Token has expired" });
            }
            catch (Exception)
            {
                return BadRequest(new { Message = "Invalid token" });
            }
        }


    }

}
