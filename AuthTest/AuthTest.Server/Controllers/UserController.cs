using AuthTest.Server;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace AuthTest.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize(Policy = "MustBeLoggedIn")]
    public class UserController : ControllerBase
    {
        private readonly DbContextTable _dbcontext;

        public UserController(DbContextTable dbcontext)
        {
            _dbcontext = dbcontext;
        }

        // GET: api/user/current
        [HttpGet("current/{login:required}")]
        public async Task<ActionResult<UserDto>> GetCurrentUser(string login)
        {
            try
            {
                var user = await _dbcontext.Пользователи.FirstOrDefaultAsync(u => u.Логин == login);//User.FindFirst(ClaimTypes.GivenName)?.Value;

                if (user == null)
                {
                    return NotFound("Пользователь не найден");
                }

                return Ok(new UserDto
                {
                    Login = user.Логин,
                    FullName = user.ФИО,
                    Role = user.Роль
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка сервера: {ex.Message}");
            }
        }

        // POST: api/user/edit
        [HttpPut("edit")]
        public async Task<ActionResult> EditUser([FromBody] UserEditRequest request)
        {
            try
            {
                // Валидация модели
                if (!ModelState.IsValid)
                {
                    return BadRequest(new 
                    {
                        success = false,
                        message = "Данные формы невалидны"
                    });
                }

                var user = await _dbcontext.Пользователи
                    .FirstOrDefaultAsync(u => u.Логин.Equals(request.Login));

                if (string.IsNullOrEmpty(user.Логин))
                {
                    return BadRequest(new 
                    {
                        success = false,
                        message = "Ошибка изменения пользователя"
                    });
                }

                // Проверка совпадения паролей
                if (request.Password != request.ConfirmPassword)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Пароли не совпадают"
                    });
                }

                //var user = await _dbcontext.Пользователи
                //    .FirstOrDefaultAsync(u => u.Логин.Equals(request.Login));

                if (user == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        sessage = "Пользователь не найден"
                    });
                }

                // Обновление данных
                if (!string.IsNullOrEmpty(request.FullName))
                {
                    user.ФИО = request.FullName;
                }

                if (!string.IsNullOrEmpty(request.Password))
                {
                    user.Пароль = Hasher.Hash(request.Password);
                }

                _dbcontext.Пользователи.Update(user);
                await _dbcontext.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    message = "Пользователь изменен успешно"
                });
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new 
                {
                    success = false,
                    message = "Ошибка базы данных при обновлении пользователя"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new 
                {
                    success = false,
                    message = "Ошибка изменения пользователя"
                });
            }
        }
    }

    // DTO модели
    public class UserDto
    {
        public string Login { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; }
    }

    public class UserEditRequest
    {
        [Required(ErrorMessage = "Логин обязателен")]
        public string Login { get; set; }

        public string Password { get; set; }

        public string ConfirmPassword { get; set; }

        public string FullName { get; set; }
    }

    //public class ApiResponse
    //{
    //    public bool Success { get; set; }
    //    public string Message { get; set; }
    //}
}