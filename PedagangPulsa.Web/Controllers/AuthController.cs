using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PedagangPulsa.Web.Interface;
using System.Security.Claims;

namespace PedagangPulsa.Web.Controllers
{
    public class AuthController : BaseController
    {
        private IAuthService _authService;

        // Constructor Injection untuk menggunakan IAuthService
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // Method Index untuk menampilkan halaman login
        [HttpGet]
        public IActionResult Index()
        {
            return View(); // Mengembalikan View untuk login
        }

        // Method ActionLogin untuk memproses data login
        [HttpPost]
        public async Task<IActionResult> ActionLogin(string username, string password)
        {
            if (_authService.ValidateCredentials(username, password))
            {
                // Jika valid, buat identitas pengguna dan cookie autentikasi
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, username)
                };

                var claimsIdentity = new ClaimsIdentity(claims, "CookieAuth");
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                await HttpContext.SignInAsync("CookieAuth", claimsPrincipal);

                // Redirect ke halaman utama setelah login berhasil
                return RedirectToAction("Index", "Home");
            }
            else
            {
                // Jika gagal, tampilkan pesan error di halaman login
                ViewBag.ErrorMessage = "Invalid username or password.";
                return View("Index");
            }
        }

        [Authorize]
        [HttpGet]
        public IActionResult CekLogin()
        {
            ViewBag.Username = Username;
            return View(); // Mengembalikan View untuk login
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("CookieAuth"); // Gunakan skema autentikasi yang benar
            return RedirectToAction("Index", "Auth"); // Redirect ke halaman login
        } 

    }
}
