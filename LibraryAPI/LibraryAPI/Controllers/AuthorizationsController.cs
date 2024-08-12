using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using LibraryAPI.Data;
using LibraryAPI.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace LibraryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorizationsController : ControllerBase
    {
        private readonly ApplicationContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;


        public AuthorizationsController(ApplicationContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IConfiguration configuration)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;

        }


        [HttpPost("GoogleLogin")]
        public async Task<ActionResult> GoogleLogin(string provider)
        {
            if (provider == "google")
            {
                // Google ile giriş başlatma
                var properties = new AuthenticationProperties { RedirectUri = "/api/Login/GoogleResponse" };
                return Challenge(properties, GoogleDefaults.AuthenticationScheme);
            }
            else
            {
                return BadRequest("Geçersiz giriş sağlayıcı.");
            }
        }

        [HttpGet("GoogleResponse")]
        public async Task<ActionResult> GoogleResponse()
        {
            var authenticateResult = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

            if (!authenticateResult.Succeeded)
            {
                return Unauthorized("Google ile giriş başarısız.");
            }

            // Google'dan dönen kullanıcı bilgileri
            var externalClaims = authenticateResult.Principal.Claims;

            // Burada gerekli işlemleri yapabilirsiniz, örneğin kullanıcıyı veritabanınızda kontrol edebilir veya yeni bir hesap oluşturabilirsiniz.

            return Ok();
        }

        [HttpPost("Login")]
        public ActionResult Login(string userName, string password)
        {
            ApplicationUser applicationUser = _userManager.FindByNameAsync(userName).Result;

            if (applicationUser != null)
            {
                // Üyenin aktif olup olmadığını kontrol et
                if (!applicationUser.IsActive)
                {
                    return Unauthorized("Üye inaktif");
                }

                var signInResult = _userManager.CheckPasswordAsync(applicationUser, password).Result;
                if (signInResult)
                {
                    var token = GenerateJwtToken(applicationUser);
                    return Ok(new { Token = token });
                }
            }
            return Unauthorized();
        }




        private object GenerateJwtToken(ApplicationUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);

            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.UserName),
        // Diğer claim'leri ekleme ihtiyacınıza göre buraya ekleyebilirsiniz
    };

            // Kullanıcının rollerini ekleyin
            var userRoles = _userManager.GetRolesAsync(user).Result;
            foreach (var role in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // Kullanıcının diğer talep bilgilerini ekleyin
            var userClaims = _userManager.GetClaimsAsync(user).Result;
            claims.AddRange(userClaims);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(5), // Token geçerlilik süresi
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Audience = _configuration["Jwt:Audience"],
                Issuer = _configuration["Jwt:Issuer"]
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            // Token'i JSON formatında döndürmek için bir anonymous object kullanın
            return new
            {
                token = tokenHandler.WriteToken(token),
                expiration = token.ValidTo
            };
        }





        [HttpGet("Logout")]
        public ActionResult LogOut()
        {
            _signInManager.SignOutAsync();
            return Ok();
        }

        [HttpPost("ForgetPassword")]
        public ActionResult<string> ForgetPassword(string userName)
        {
            ApplicationUser applicationUser = _userManager.FindByNameAsync(userName).Result;

            string token = _userManager.GeneratePasswordResetTokenAsync(applicationUser).Result;
            System.Net.Mail.MailMessage mailMessage = new System.Net.Mail.MailMessage("abc", applicationUser.Email, "Şifre sıfırlama", token);

            //Bu kod, temel anlamda bir e - posta mesajını SMTP
            //sunucusu aracılığıyla göndermek için kullanılan
            //standart bir işlemi temsil eder.
            System.Net.Mail.SmtpClient smtpClient = new System.Net.Mail.SmtpClient("http://smtp.domain.com");
            smtpClient.Send(mailMessage);
            return token;
        }

        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(string userName, string token, string newPassword)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                return BadRequest(new { Errors = errors });
            }

            return Ok("Password reset successful.");
        }

    }
}
