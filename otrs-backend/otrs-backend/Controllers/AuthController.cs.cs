using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using otrs_backend.Data;
using otrs_backend.Models;
using otrs_backend.Requests;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace otrs_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly SmtpClient _smtpClient;

        public AuthController(AppDbContext context, IConfiguration configuration, SmtpClient smtpClient)
        {
            _context = context;
            _configuration = configuration;
            _smtpClient = smtpClient;
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _context.Users
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null)
            {
                return BadRequest("Nie znaleziono użytkownika.");
            }

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return BadRequest("Błędne hasło.");
            }

            string token = CreateToken(user);

            SetTokenCookie(token);

            var userData = new
            {
                user.Id,
                user.Name,
                user.Surname,
                user.Email,
                Roles = user.Roles.Select(r => r.Name).ToList()
            };

            return Ok(new { user = userData });
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] RegisterRequest request)
        {
            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            {
                return BadRequest("Użytkownik o podanym adresie email już istnieje.");
            }

            var names = request.Fullname.Split(' ', 2);
            string name = names[0];
            string surname = names.Length > 1 ? names[1] : "";

            var user = new User
            {
                Name = name,
                Surname = surname,
                Email = request.Email,
                Bio = "",
                AvatarUrl = "",
                BirthDate = DateTime.Now,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
            };

            var userRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "User");
            if (userRole == null)
            {
                userRole = new Role { Name = "User", Description = "Domyślna rola użytkownika" };
                _context.Roles.Add(userRole);
            }

            user.Roles.Add(userRole);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            string token = CreateToken(user);

            SetTokenCookie(token);

            var userData = new { user.Id, user.Name, user.Surname, user.Email, Roles = new[] { "User" } };

            return Ok(new { user = userData });
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null)
            {
                return Ok(new { message = "Jeśli e-mail istnieje w bazie, wysłano link do resetu hasła." });
            }

            var token = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
            user.PasswordResetToken = token;
            user.PasswordResetTokenExpiry = DateTime.UtcNow.AddHours(1);

            await _context.SaveChangesAsync();

            var resetLink = $"http://localhost:5173/reset-password?token={token}";

            var mailMessage = new System.Net.Mail.MailMessage
            {
                From = new System.Net.Mail.MailAddress("no-reply@otrs-hustle.com", "OTRS System"),
                Subject = "OTRS - Resetowanie Hasła"
            };

            var bodyStr = @"
                <!DOCTYPE html>
                <html lang=""pl"">
                <head>
                    <meta charset=""UTF-8"">
                    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                    <title>Reset hasła - OTRS</title>
                </head>
                <body style=""margin: 0; padding: 0; font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; background-color: #F0F2F4; color: #313D40;"">
                    <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0"" style=""background-color: #F0F2F4; padding: 40px 0;"">
                        <tr>
                            <td align=""center"">
                                <table width=""600"" cellpadding=""0"" cellspacing=""0"" border=""0"" style=""background-color: #ffffff; border-radius: 8px; overflow: hidden; box-shadow: 0 4px 10px rgba(0,0,0,0.05);"">
                                    <!-- Header -->
                                    <tr>
                                        <td align=""center"" style=""background-color: #3365A6; padding: 30px 0;"">
                                            <img src=""cid:logo"" alt=""OTRS Logo"" style=""max-height: 80px; display: block;"">
                                        </td>
                                    </tr>
                                    <!-- Content -->
                                    <tr>
                                        <td style=""padding: 40px 30px;"">
                                            <h2 style=""margin-top: 0; color: #313D40; font-size: 24px;"">Zresetuj swoje hasło</h2>
                                            <p style=""font-size: 16px; line-height: 1.6; color: #7D7E8C;"">Witaj <strong>{userName}</strong>,</p>
                                            <p style=""font-size: 16px; line-height: 1.6; color: #7D7E8C;"">Otrzymaliśmy prośbę o zresetowanie hasła dla Twojego konta.</p>
                                            <p style=""font-size: 16px; line-height: 1.6; color: #7D7E8C;"">Jeśli to nie Ty składałeś/-aś tę prośbę, zignoruj tę wiadomość.</p>
                                            <p style=""font-size: 16px; line-height: 1.6; color: #7D7E8C;"">Kliknij przycisk poniżej, aby ustawić nowe hasło (link jest ważny przez 1 godzinę):</p>
                                            
                                            <!-- Button -->
                                            <table width=""100%"" border=""0"" cellspacing=""0"" cellpadding=""0"" style=""margin: 30px 0;"">
                                                <tr>
                                                    <td align=""center"">
                                                        <a href=""{resetLink}"" style=""background-color: #3365A6; color: #ffffff; padding: 14px 28px; text-decoration: none; border-radius: 6px; font-size: 16px; font-weight: bold; display: inline-block;"">Zresetuj hasło</a>
                                                    </td>
                                                </tr>
                                            </table>

                                            <p style=""font-size: 14px; line-height: 1.5; color: #7D7E8C; border-top: 1px solid #e5e7eb; padding-top: 20px; margin-top: 30px;"">
                                                Jeśli przycisk nie działa, skopiuj i wklej ten link do przeglądarki:<br>
                                                <a href=""{resetLink}"" style=""color: #3365A6; word-break: break-all;"">{resetLink}</a>
                                            </p>
                                        </td>
                                    </tr>
                                    <!-- Footer -->
                                    <tr>
                                        <td align=""center"" style=""background-color: #F0F2F4; padding: 20px; font-size: 12px; color: #7392A7;"">
                                            <p style=""margin: 0;"">&copy; {year} OTRS System. Wszelkie prawa zastrzeżone.</p>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </body>
                </html>"
                .Replace("{userName}", user.Name)
                .Replace("{resetLink}", resetLink)
                .Replace("{year}", DateTime.UtcNow.Year.ToString());

            var htmlView = System.Net.Mail.AlternateView.CreateAlternateViewFromString(bodyStr, null, "text/html");

            var logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "HustleTrackLogo.png");
            if (System.IO.File.Exists(logoPath))
            {
                var logo = new System.Net.Mail.LinkedResource(logoPath, "image/png");
                logo.ContentId = "logo";
                htmlView.LinkedResources.Add(logo);
            }

            mailMessage.AlternateViews.Add(htmlView);
            mailMessage.To.Add(user.Email);

            await _smtpClient.SendMailAsync(mailMessage);

            return Ok(new { message = "Jeśli e-mail istnieje w bazie, wysłano link do resetu hasła.", devResetLink = resetLink });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u =>
                u.PasswordResetToken == request.Token &&
                u.PasswordResetTokenExpiry > DateTime.UtcNow);

            if (user == null)
            {
                return BadRequest("Token jest nieprawidłowy lub wygasł.");
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);

            user.PasswordResetToken = null;
            user.PasswordResetTokenExpiry = null;

            await _context.SaveChangesAsync();

            return Ok("Hasło zostało pomyślnie zmienione.");
        }

        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized("Brak poprawnych danych użytkownika.");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return Unauthorized("Użytkownik nie został znaleziony.");
            }

            if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
            {
                return BadRequest("Aktualne hasło jest nieprawidłowe.");
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            user.PasswordResetToken = null;
            user.PasswordResetTokenExpiry = null;

            await _context.SaveChangesAsync();

            return Ok("Hasło zostało pomyślnie zmienione.");
        }

        [HttpPost("logout")]
        [Authorize]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("jwt", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None
            });

            return Ok(new { message = "Wylogowano pomyślnie" });
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return Unauthorized(new { message = "Nieprawidłowy token." });
            }

            var user = await _context.Users
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return Unauthorized(new { message = "Użytkownik nie istnieje." });
            }

            var userData = new
            {
                Id = user.Id,
                Name = user.Name,
                Surname = user.Surname,
                Email = user.Email,
                Roles = user.Roles.Select(r => r.Name).ToList()
            };

            return Ok(new { user = userData });
        }

        private string CreateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, $"{user.Name} {user.Surname}")
            };

            foreach (var role in user.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.Name));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("JwtSettings:SecretKey").Value!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private void SetTokenCookie(string token)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(1),
                SameSite = SameSiteMode.None,
                Secure = true
            };

            Response.Cookies.Append("jwt", token, cookieOptions);
        }
    }
}