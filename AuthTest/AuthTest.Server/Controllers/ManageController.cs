using EgrWebEntity.ModelTable;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using AuthTest.Server;

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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserManageModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Roles = GetRolesSelectList();
                model.ExistingUsers = GetExistingUsersSelectList();
                return Ok( new { 
                    text = "Manage", 
                    model
                });
            }

            // Проверка уникальности логина
            if (_dbcontext.Пользователи.Any(u => u.Логин.Equals(model.NewLoginData.Login)))
            {
                ModelState.AddModelError("NewLoginData.Login", "Пользователь с таким логином уже существует");
                model.Roles = GetRolesSelectList();
                model.ExistingUsers = GetExistingUsersSelectList();
                return Ok(new
                {
                    text = "Manage",
                    model
                });
            }

            if (model.NewLoginData.Password != model.NewLoginData.ConfirmPassword)
            {
                ModelState.AddModelError("NewLoginData.ConfirmPassword", "Пароли не совпадают");
                model.Roles = GetRolesSelectList();
                model.ExistingUsers = GetExistingUsersSelectList();
                return Ok(new
                {
                    text = "Manage",
                    model
                });
            }

            if (string.IsNullOrEmpty(model.NewLoginData.SelectedRole))
            {
                ModelState.AddModelError("NewLoginData.SelectedRole", "Требуется выбрать роль пользователя");
                model.Roles = GetRolesSelectList();
                model.ExistingUsers = GetExistingUsersSelectList();
                return Ok(new
                {
                    text = "Manage",
                    model
                });
            }

            try
            {
                var newUser = new Users
                {
                    Логин = model.NewLoginData.Login,
                    Пароль = Hasher.Hash(model.NewLoginData.Password),
                    Роль = model.NewLoginData.SelectedRole,
                    ФИО = model.NewLoginData.FullName
                };

                _dbcontext.Пользователи.Add(newUser);
                await _dbcontext.SaveChangesAsync();

                TempData["UserCreateStatusMessage"] = "Новый пользователь создан успешно";
                TempData["MessageType"] = "success";
            }
            catch (System.Exception)
            {
                TempData["UserCreateStatusMessage"] = "Ошибка при создании пользователя";
                TempData["MessageType"] = "error";
            }

            return RedirectToAction("Manage");
        }

        // GET: User/Load
        [HttpGet("Load")]
        public IActionResult Load(string selectedExistingUser)
        {
            if (string.IsNullOrEmpty(selectedExistingUser))
            {
                TempData["UserEditStatusMessage"] = "Пользователь не выбран";
                TempData["MessageType"] = "error";
                return RedirectToAction("Manage");
            }

            var user = _dbcontext.Пользователи
                .FirstOrDefault(u => u.Логин.Equals(selectedExistingUser));

            if (user == null)
            {
                TempData["UserEditStatusMessage"] = "Пользователь не найден";
                TempData["MessageType"] = "error";
                return RedirectToAction("Manage");
            }

            var model = new UserManageModel
            {
                ExistingLoginData = new FullCredential
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

            TempData["LoadedUser"] = user.Логин;
            return Ok(new
            {
                text = "Manage",
                model
            });
        }

        // POST: User/Update
        [HttpPut("Update")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(UserManageModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Roles = GetRolesSelectList();
                model.ExistingUsers = GetExistingUsersSelectList();
                return Ok(new
                {
                    text = "Manage",
                    model
                });
            }

            if (model.ExistingLoginData.Password != model.ExistingLoginData.ConfirmPassword)
            {
                ModelState.AddModelError("ExistingLoginData.ConfirmPassword", "Пароли не совпадают");
                model.Roles = GetRolesSelectList();
                model.ExistingUsers = GetExistingUsersSelectList();
                return Ok(new
                {
                    text = "Manage",
                    model
                });
            }

            try
            {
                var existingUser = _dbcontext.Пользователи
                    .FirstOrDefault(u => u.Логин.Equals(model.HiddenSelectedUser));

                if (existingUser == null)
                {
                    TempData["UserEditStatusMessage"] = "Пользователь не найден";
                    TempData["MessageType"] = "error";
                    return RedirectToAction("Manage");
                }

                existingUser.Логин = model.ExistingLoginData.Login;
                existingUser.Роль = model.ExistingLoginData.SelectedRole;
                existingUser.ФИО = model.ExistingLoginData.FullName;

                if (!string.IsNullOrEmpty(model.ExistingLoginData.Password))
                {
                    existingUser.Пароль = Hasher.Hash(model.ExistingLoginData.Password);
                }

                _dbcontext.Пользователи.Update(existingUser);
                await _dbcontext.SaveChangesAsync();

                TempData["UserEditStatusMessage"] = "Пользователь изменен успешно";
                TempData["MessageType"] = "success";
            }
            catch (System.Exception)
            {
                TempData["UserEditStatusMessage"] = "Ошибка изменения пользователя";
                TempData["MessageType"] = "error";
            }

            return RedirectToAction("Manage");
        }

        // POST: User/Delete
        [HttpDelete("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(UserManageModel model)
        {
            try
            {
                var existingUser = _dbcontext.Пользователи
                    .FirstOrDefault(u => u.Логин.Equals(model.HiddenSelectedUser));

                if (existingUser == null)
                {
                    TempData["UserEditStatusMessage"] = "Пользователь не найден";
                    TempData["MessageType"] = "error";
                    return RedirectToAction("Manage");
                }

                _dbcontext.Пользователи.Remove(existingUser);
                await _dbcontext.SaveChangesAsync();

                TempData["UserEditStatusMessage"] = "Пользователь удален успешно";
                TempData["MessageType"] = "success";
            }
            catch (System.Exception)
            {
                TempData["UserEditStatusMessage"] = "Ошибка удаления пользователя";
                TempData["MessageType"] = "error";
            }

            return RedirectToAction("Manage");
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