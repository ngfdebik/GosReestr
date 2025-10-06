using EgrWebEntity.ModelTable;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using AuthTest.Server;
using System.IdentityModel.Tokens.Jwt;

namespace AuthTest.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "MustBeAdmin")]
    public class ManageController : Controller
    {
        private readonly DbContextTable _dbcontext;

        public ManageController(DbContextTable dbcontext)
        {
            _dbcontext = dbcontext;
        }

        // GET: User/Manage
        [HttpGet("Manage")]
        public IActionResult Manage()
        {
            var model = new UserManageModel
            {
                Roles = GetRolesSelectList(),
                ExistingUsers = GetExistingUsersSelectList()
            };

            return Ok(model);
        }

        // POST: User/Create
        [HttpPost("Create")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromBody] UserManageModel model)
        {
            try
            {
                // Валидация модели
                if (model?.NewLoginData == null)
                {
                    return BadRequest(new
                    {
                        success = false,
                        error = "Неверный формат данных"
                    });
                }

                // Валидация обязательных полей
                if (string.IsNullOrEmpty(model.NewLoginData.Login))
                {
                    return BadRequest(new
                    {
                        success = false,
                        error = "Логин обязателен"
                    });
                }

                if (string.IsNullOrEmpty(model.NewLoginData.Password))
                {
                    return BadRequest(new
                    {
                        success = false,
                        error = "Пароль обязателен"
                    });
                }

                if (string.IsNullOrEmpty(model.NewLoginData.SelectedRole))
                {
                    return BadRequest(new
                    {
                        success = false,
                        error = "Роль обязательна"
                    });
                }

                // Проверка длины пароля
                if (model.NewLoginData.Password.Length < 6)
                {
                    return BadRequest(new
                    {
                        success = false,
                        error = "Пароль должен содержать не менее 6 символов"
                    });
                }

                // Проверка совпадения паролей
                if (model.NewLoginData.Password != model.NewLoginData.ConfirmPassword)
                {
                    return BadRequest(new
                    {
                        success = false,
                        error = "Пароли не совпадают"
                    });
                }

                // Проверка уникальности логина
                if (_dbcontext.Пользователи.Any(u => u.Логин.Equals(model.NewLoginData.Login)))
                {
                    return BadRequest(new
                    {
                        success = false,
                        error = "Пользователь с таким логином уже существует"
                    });
                }

                // Создание нового пользователя
                var newUser = new Users
                {
                    Логин = model.NewLoginData.Login,
                    Пароль = Hasher.Hash(model.NewLoginData.Password),
                    Роль = model.NewLoginData.SelectedRole,
                    ФИО = model.NewLoginData.FullName ?? string.Empty
                };

                _dbcontext.Пользователи.Add(newUser);
                await _dbcontext.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    message = "Новый пользователь создан успешно",
                    data = new
                    {
                        login = newUser.Логин,
                        selectedRole = newUser.Роль,
                        fullName = newUser.ФИО
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    error = "Внутренняя ошибка сервера при создании пользователя"
                });
            }
        }

        // GET: User/Load
        [HttpGet("Load/{selectedExistingUser:required}")]
        public IActionResult Load(string selectedExistingUser)
        {
            if (string.IsNullOrEmpty(selectedExistingUser))
            {
                return BadRequest(new
                {
                    success = false,
                    error = "Пользователь не выбран"
                });
            }

            var user = _dbcontext.Пользователи
                .FirstOrDefault(u => u.Логин.Equals(selectedExistingUser));

            if (user == null)
            {
                return NotFound(new
                {
                    success = false,
                    error = "Пользователь не найден"
                });
            }

            var model = new
            {
                ExistingLoginData = new
                {
                    Login = user.Логин,
                    SelectedRole = user.Роль,
                    FullName = user.ФИО
                },
                HiddenSelectedUser = user.Логин,
                Roles = GetRolesSelectList(),
                ExistingUsers = GetExistingUsersSelectList(),
                SelectedExistingUser = user.Логин
            };

            return Ok(new
            {
                success = true,
                data = model
            });
        }

        // POST: User/Update
        [HttpPut("Update")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Update([FromBody] UserManageModel model)
        {
            try
            {
                // Валидация модели
                if (model == null || model.ExistingLoginData == null)
                {
                    return BadRequest(new
                    {
                        success = false,
                        error = "Неверный формат данных"
                    });
                }

                if (string.IsNullOrEmpty(model.HiddenSelectedUser))
                {
                    return BadRequest(new
                    {
                        success = false,
                        error = "Не указан пользователь для изменения"
                    });
                }

                if (string.IsNullOrEmpty(model.ExistingLoginData.Login))
                {
                    return BadRequest(new
                    {
                        success = false,
                        error = "Логин обязателен"
                    });
                }

                if (string.IsNullOrEmpty(model.ExistingLoginData.SelectedRole))
                {
                    return BadRequest(new
                    {
                        success = false,
                        error = "Роль обязательна"
                    });
                }

                // Проверка совпадения паролей
                if (!string.IsNullOrEmpty(model.ExistingLoginData.Password) &&
                    model.ExistingLoginData.Password != model.ExistingLoginData.ConfirmPassword)
                {
                    return BadRequest(new
                    {
                        success = false,
                        error = "Пароли не совпадают"
                    });
                }

                // Поиск пользователя
                var existingUser = _dbcontext.Пользователи
                    .FirstOrDefault(u => u.Логин.Equals(model.HiddenSelectedUser));

                if (existingUser == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        error = "Пользователь не найден"
                    });
                }

                // Проверка на уникальность логина
                if (model.ExistingLoginData.Login != model.HiddenSelectedUser)
                {
                    var userWithSameLogin = _dbcontext.Пользователи
                        .FirstOrDefault(u => u.Логин.Equals(model.ExistingLoginData.Login) &&
                                           u.Логин != model.HiddenSelectedUser);

                    if (userWithSameLogin != null)
                    {
                        return BadRequest(new
                        {
                            success = false,
                            error = "Пользователь с таким логином уже существует"
                        });
                    }
                }

                // Обновление данных
                existingUser.Логин = model.ExistingLoginData.Login;
                existingUser.Роль = model.ExistingLoginData.SelectedRole;
                existingUser.ФИО = model.ExistingLoginData.FullName;

                if (!string.IsNullOrEmpty(model.ExistingLoginData.Password))
                {
                    existingUser.Пароль = Hasher.Hash(model.ExistingLoginData.Password);
                }

                _dbcontext.Пользователи.Update(existingUser);
                await _dbcontext.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    message = "Пользователь изменен успешно",
                    data = new
                    {
                        login = existingUser.Логин,
                        selectedRole = existingUser.Роль,
                        fullName = existingUser.ФИО,
                        hiddenSelectedUser = existingUser.Логин
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    error = "Внутренняя ошибка сервера при обновлении пользователя"
                });
            }
        }

        // POST: User/Delete
        //[ValidateAntiForgeryToken]
        [HttpDelete("delete/{selectedExistingUser:required}")]
        public async Task<IActionResult> Delete(string selectedExistingUser)
        {
            try
            {
                // Валидация обязательных полей
                if (string.IsNullOrEmpty(selectedExistingUser))
                {
                    return BadRequest(new
                    {
                        success = false,
                        error = "Не указан пользователь для удаления"
                    });
                }

                // Поиск пользователя
                var existingUser = _dbcontext.Пользователи
                    .FirstOrDefault(u => u.Логин.Equals(selectedExistingUser));

                if (existingUser == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        error = "Пользователь не найден"
                    });
                }

                // Проверка: нельзя удалить самого себя
                var currentUserLogin = User.FindFirst(JwtRegisteredClaimNames.UniqueName)?.Value;
                if (existingUser.Логин.Equals(currentUserLogin))
                {
                    return BadRequest(new
                    {
                        success = false,
                        error = "Нельзя удалить собственный аккаунт"
                    });
                }

                // Проверка: нельзя удалить последнего администратора
                if (existingUser.Роль == "Администратор")
                {
                    var adminCount = _dbcontext.Пользователи
                        .Count(u => u.Роль == "Администратор");

                    if (adminCount <= 1)
                    {
                        return BadRequest(new
                        {
                            success = false,
                            error = "Нельзя удалить последнего администратора"
                        });
                    }
                }

                // Удаление пользователя
                _dbcontext.Пользователи.Remove(existingUser);
                await _dbcontext.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    message = "Пользователь удален успешно",
                    data = new
                    {
                        deletedUser = existingUser.Логин
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    error = "Внутренняя ошибка сервера при удалении пользователя"
                });
            }
        }

        // Вспомогательные методы
        private List<SelectListItem> GetRolesSelectList()
        {
            return new List<SelectListItem>
            {
                new SelectListItem { Value = "Администратор", Text = "Администратор" },
                new SelectListItem { Value = "Пользователь", Text = "Пользователь" }
            };
        }

        private List<SelectListItem> GetExistingUsersSelectList()
        {
            var users = _dbcontext.Пользователи.ToList();
            var selectList = new List<SelectListItem>();

            foreach (var user in users)
            {
                selectList.Add(new SelectListItem { Value = user.Логин, Text = user.Логин });
            }

            return selectList;
        }
    }

    // Модель представления
    public class UserManageModel
    {
        public FullCredential NewLoginData { get; set; } = new FullCredential();
        public FullCredential ExistingLoginData { get; set; } = new FullCredential();
        public List<SelectListItem> Roles { get; set; }
        public List<SelectListItem> ExistingUsers { get; set; }
        public string SelectedExistingUser { get; set; }
        public string HiddenSelectedUser { get; set; }

        [TempData]
        public string UserCreateStatusMessage { get; set; }

        [TempData]
        public string UserEditStatusMessage { get; set; }

        [TempData]
        public string MessageType { get; set; }
    }

    // Модель данных (можно вынести в отдельный файл)
    public class FullCredential
    {
        [Required(ErrorMessage = "Логин обязателен")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Пароль обязателен")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Подтвердите пароль")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        public string ConfirmPassword { get; set; }

        public string FullName { get; set; }

        [Required(ErrorMessage = "Роль обязательна")]
        public string SelectedRole { get; set; }
    }
}