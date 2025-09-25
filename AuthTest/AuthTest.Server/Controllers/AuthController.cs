// Controllers/AuthController.cs
using AuthTest.Server;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace AuthTest.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly DbContextTable _dbcontext;

        public AuthController(DbContextTable dbcontext)
        {
            _dbcontext = dbcontext;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Credential credentials)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    errors = ModelState.ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).FirstOrDefault() ?? ""
                    )
                });
            }

            var user = await _dbcontext.Пользователи
                .FirstOrDefaultAsync(u => u.Логин == credentials.Login);

            if (user == null)
            {
                return BadRequest(new
                {
                    errors = new { Login = "Неверно указан логин" }
                });
            }

            if (!Hasher.Verify(credentials.Password, user.Пароль))
            {
                return BadRequest(new
                {
                    errors = new { Password = "Неверный пароль" }
                });
            }

            // Создаём Claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.ФИО),
                new Claim(ClaimTypes.GivenName, user.Логин)
            };

            string userType = user.Роль switch
            {
                "Администратор" => "Администратор",
                "Пользователь" => "Пользователь",
                _ => throw new InvalidOperationException("Неизвестная роль")
            };

            claims.Add(new Claim("UserType", userType));

            var identity = new ClaimsIdentity(claims, "EGRCookieAuth");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync("EGRCookieAuth", claimsPrincipal);

            // Возвращаем URL для редиректа
            string redirectUrl = userType switch
            {
                "Администратор" => "/EGRAdmin",
                "Пользователь" => "/EGR",
                _ => "/"
            };

            return Ok(new { redirectTo = redirectUrl });
        }

        public class Credential
        {
            [Required(ErrorMessage = "Не указан логин")]
            public string Login { get; set; } = string.Empty;

            [Required(ErrorMessage = "Не указан пароль")]
            public string Password { get; set; } = string.Empty;
        }
    }
}