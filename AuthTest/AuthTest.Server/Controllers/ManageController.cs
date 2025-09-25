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

            // �������� ������������ ������
            if (_dbcontext.������������.Any(u => u.�����.Equals(model.NewLoginData.Login)))
            {
                ModelState.AddModelError("NewLoginData.Login", "������������ � ����� ������� ��� ����������");
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
                ModelState.AddModelError("NewLoginData.ConfirmPassword", "������ �� ���������");
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
                ModelState.AddModelError("NewLoginData.SelectedRole", "��������� ������� ���� ������������");
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
                    ����� = model.NewLoginData.Login,
                    ������ = Hasher.Hash(model.NewLoginData.Password),
                    ���� = model.NewLoginData.SelectedRole,
                    ��� = model.NewLoginData.FullName
                };

                _dbcontext.������������.Add(newUser);
                await _dbcontext.SaveChangesAsync();

                TempData["UserCreateStatusMessage"] = "����� ������������ ������ �������";
                TempData["MessageType"] = "success";
            }
            catch (System.Exception)
            {
                TempData["UserCreateStatusMessage"] = "������ ��� �������� ������������";
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
                TempData["UserEditStatusMessage"] = "������������ �� ������";
                TempData["MessageType"] = "error";
                return RedirectToAction("Manage");
            }

            var user = _dbcontext.������������
                .FirstOrDefault(u => u.�����.Equals(selectedExistingUser));

            if (user == null)
            {
                TempData["UserEditStatusMessage"] = "������������ �� ������";
                TempData["MessageType"] = "error";
                return RedirectToAction("Manage");
            }

            var model = new UserManageModel
            {
                ExistingLoginData = new FullCredential
                {
                    Login = user.�����,
                    SelectedRole = user.����,
                    FullName = user.���
                },
                HiddenSelectedUser = user.�����,
                Roles = GetRolesSelectList(),
                ExistingUsers = GetExistingUsersSelectList(),
                SelectedExistingUser = user.�����
            };

            TempData["LoadedUser"] = user.�����;
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
                ModelState.AddModelError("ExistingLoginData.ConfirmPassword", "������ �� ���������");
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
                var existingUser = _dbcontext.������������
                    .FirstOrDefault(u => u.�����.Equals(model.HiddenSelectedUser));

                if (existingUser == null)
                {
                    TempData["UserEditStatusMessage"] = "������������ �� ������";
                    TempData["MessageType"] = "error";
                    return RedirectToAction("Manage");
                }

                existingUser.����� = model.ExistingLoginData.Login;
                existingUser.���� = model.ExistingLoginData.SelectedRole;
                existingUser.��� = model.ExistingLoginData.FullName;

                if (!string.IsNullOrEmpty(model.ExistingLoginData.Password))
                {
                    existingUser.������ = Hasher.Hash(model.ExistingLoginData.Password);
                }

                _dbcontext.������������.Update(existingUser);
                await _dbcontext.SaveChangesAsync();

                TempData["UserEditStatusMessage"] = "������������ ������� �������";
                TempData["MessageType"] = "success";
            }
            catch (System.Exception)
            {
                TempData["UserEditStatusMessage"] = "������ ��������� ������������";
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
                var existingUser = _dbcontext.������������
                    .FirstOrDefault(u => u.�����.Equals(model.HiddenSelectedUser));

                if (existingUser == null)
                {
                    TempData["UserEditStatusMessage"] = "������������ �� ������";
                    TempData["MessageType"] = "error";
                    return RedirectToAction("Manage");
                }

                _dbcontext.������������.Remove(existingUser);
                await _dbcontext.SaveChangesAsync();

                TempData["UserEditStatusMessage"] = "������������ ������ �������";
                TempData["MessageType"] = "success";
            }
            catch (System.Exception)
            {
                TempData["UserEditStatusMessage"] = "������ �������� ������������";
                TempData["MessageType"] = "error";
            }

            return RedirectToAction("Manage");
        }

        // ��������������� ������
        private List<SelectListItem> GetRolesSelectList()
        {
            return new List<SelectListItem>
            {
                new SelectListItem { Value = "�������������", Text = "�������������" },
                new SelectListItem { Value = "������������", Text = "������������" }
            };
        }

        private List<SelectListItem> GetExistingUsersSelectList()
        {
            var users = _dbcontext.������������.ToList();
            var selectList = new List<SelectListItem>();

            foreach (var user in users)
            {
                selectList.Add(new SelectListItem { Value = user.�����, Text = user.����� });
            }

            return selectList;
        }
    }

    // ������ �������������
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

    // ������ ������ (����� ������� � ��������� ����)
    public class FullCredential
    {
        [Required(ErrorMessage = "����� ����������")]
        public string Login { get; set; }

        [Required(ErrorMessage = "������ ����������")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "����������� ������")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "������ �� ���������")]
        public string ConfirmPassword { get; set; }

        public string FullName { get; set; }

        [Required(ErrorMessage = "���� �����������")]
        public string SelectedRole { get; set; }
    }
}