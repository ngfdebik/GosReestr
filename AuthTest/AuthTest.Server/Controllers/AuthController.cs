// Controllers/AuthController.cs
using AuthTest.Server;
using EgrWebEntity.ModelTable;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AuthTest.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly DbContextTable _dbcontext;
        private readonly IJwtService _jwtService;

        public AuthController(IJwtService jwtService, DbContextTable dbcontext)
        {
            _jwtService = jwtService;
            _dbcontext = dbcontext;
        }

        [HttpPost("login")]
        public async Task<ActionResult<TokenResponse>> Login([FromBody] Credential credentials)
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
            /*
            // Создаём Claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.ФИО),
                new Claim(ClaimTypes.GivenName, user.Логин)
            };
            */
            //var person = _dbcontext.Пользователи.FirstOrDefault(x => x.Логин == username && Hasher.Verify(password, x.Пароль));

            var identity = GetIdentity(user);
            string userType = user.Роль switch
            {
                "Администратор" => "Администратор",
                "Пользователь" => "Пользователь",
                _ => throw new InvalidOperationException("Неизвестная роль")
            };

            if (identity == null)
            {
                return NotFound();
            }
            //var encodedJwt = JWT.getJWT(identity);
            
            //claims.Add(new Claim("UserType", userType));

            //var identity = new ClaimsIdentity(claims, "EGRCookieAuth");
            //var claimsPrincipal = new ClaimsPrincipal(identity);

            //await HttpContext.SignInAsync("EGRCookieAuth", claimsPrincipal);

            // Возвращаем URL для редиректа
            string redirectUrl = userType switch
            {
                "Администратор" => "/EGRAdmin",
                "Пользователь" => "/EGR",
                _ => "/"
            };

            bool isAdmin = userType switch
            {
                "Администратор" => true,
                "Пользователь" => false,
                _ => false
            };

            var tokens = _jwtService.GenerateTokens(user.Id.ToString(), user.Логин, new List<string> { user.Роль });

            return Ok(new { 
                access_token = tokens.AccessToken,
                refresh_token = tokens.RefreshToken,
                redirectTo = "/EGR",
                login = user.Логин,
                isAdmin
            });
        }

        [HttpPost("refresh")]
        public async Task<ActionResult<TokenResponse>> Refresh([FromBody] RefreshTokenRequest request)
        {
            try
            {
                // Получаем роли пользователя из базы данных
                var userId = GetUserIdFromToken(request.AccessToken); // Нужно реализовать этот метод
                var userRoles = await _dbcontext.Пользователи.FirstOrDefaultAsync(u => u.Id.ToString().Equals(userId));

                var tokens = _jwtService.RefreshTokens(request.AccessToken, request.RefreshToken, new List<string> { userRoles.Роль });

                return Ok(tokens);
            }
            catch (SecurityTokenException ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutRequest request)
        {
            // Здесь можно инвалидировать refresh token в базе данных
            // await _userService.RevokeRefreshTokenAsync(request.RefreshToken);

            return Ok(new { message = "Logged out successfully" });
        }

        [HttpPost("validate")]
        public IActionResult Validate([FromBody] ValidateTokenRequest request)
        {
            var principal = _jwtService.ValidateToken(request.Token);
            if (principal == null)
            {
                return Unauthorized("Invalid token");
            }

            return Ok(new
            {
                isValid = true,
                userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value
            });
        }

        private ClaimsIdentity? GetIdentity(Users person)
        {
            if (person != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, person.Логин),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, person.Роль)
                };
                ClaimsIdentity claimsIdentity =
                new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);
                return claimsIdentity;
            }

            // если пользователя не найдено
            return null;
        }

        private string GetUserIdFromToken(string token)
        {
            try
            {
                var principal = _jwtService.ValidateToken(token);
                return GetUserIdFromClaims(principal.Claims);
            }
            catch
            {
                return null;
            }
        }

        private string GetUserIdFromClaims(IEnumerable<Claim> claims)
        {
            return claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value
                ?? claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value
                ?? claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
        }

        public class Credential
        {
            [Required(ErrorMessage = "Не указан логин")]
            public string Login { get; set; } = string.Empty;

            [Required(ErrorMessage = "Не указан пароль")]
            public string Password { get; set; } = string.Empty;
        }
    }

    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class LogoutRequest
    {
        public string RefreshToken { get; set; }
    }

    public class ValidateTokenRequest
    {
        public string Token { get; set; }
    }
}