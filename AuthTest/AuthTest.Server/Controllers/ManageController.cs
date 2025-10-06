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
                // ��������� ������
                if (model?.NewLoginData == null)
                {
                    return BadRequest(new
                    {
                        success = false,
                        error = "�������� ������ ������"
                    });
                }

                // ��������� ������������ �����
                if (string.IsNullOrEmpty(model.NewLoginData.Login))
                {
                    return BadRequest(new
                    {
                        success = false,
                        error = "����� ����������"
                    });
                }

                if (string.IsNullOrEmpty(model.NewLoginData.Password))
                {
                    return BadRequest(new
                    {
                        success = false,
                        error = "������ ����������"
                    });
                }

                if (string.IsNullOrEmpty(model.NewLoginData.SelectedRole))
                {
                    return BadRequest(new
                    {
                        success = false,
                        error = "���� �����������"
                    });
                }

                // �������� ����� ������
                if (model.NewLoginData.Password.Length < 6)
                {
                    return BadRequest(new
                    {
                        success = false,
                        error = "������ ������ ��������� �� ����� 6 ��������"
                    });
                }

                // �������� ���������� �������
                if (model.NewLoginData.Password != model.NewLoginData.ConfirmPassword)
                {
                    return BadRequest(new
                    {
                        success = false,
                        error = "������ �� ���������"
                    });
                }

                // �������� ������������ ������
                if (_dbcontext.������������.Any(u => u.�����.Equals(model.NewLoginData.Login)))
                {
                    return BadRequest(new
                    {
                        success = false,
                        error = "������������ � ����� ������� ��� ����������"
                    });
                }

                // �������� ������ ������������
                var newUser = new Users
                {
                    ����� = model.NewLoginData.Login,
                    ������ = Hasher.Hash(model.NewLoginData.Password),
                    ���� = model.NewLoginData.SelectedRole,
                    ��� = model.NewLoginData.FullName ?? string.Empty
                };

                _dbcontext.������������.Add(newUser);
                await _dbcontext.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    message = "����� ������������ ������ �������",
                    data = new
                    {
                        login = newUser.�����,
                        selectedRole = newUser.����,
                        fullName = newUser.���
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    error = "���������� ������ ������� ��� �������� ������������"
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
                    error = "������������ �� ������"
                });
            }

            var user = _dbcontext.������������
                .FirstOrDefault(u => u.�����.Equals(selectedExistingUser));

            if (user == null)
            {
                return NotFound(new
                {
                    success = false,
                    error = "������������ �� ������"
                });
            }

            var model = new
            {
                ExistingLoginData = new
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
                // ��������� ������
                if (model == null || model.ExistingLoginData == null)
                {
                    return BadRequest(new
                    {
                        success = false,
                        error = "�������� ������ ������"
                    });
                }

                if (string.IsNullOrEmpty(model.HiddenSelectedUser))
                {
                    return BadRequest(new
                    {
                        success = false,
                        error = "�� ������ ������������ ��� ���������"
                    });
                }

                if (string.IsNullOrEmpty(model.ExistingLoginData.Login))
                {
                    return BadRequest(new
                    {
                        success = false,
                        error = "����� ����������"
                    });
                }

                if (string.IsNullOrEmpty(model.ExistingLoginData.SelectedRole))
                {
                    return BadRequest(new
                    {
                        success = false,
                        error = "���� �����������"
                    });
                }

                // �������� ���������� �������
                if (!string.IsNullOrEmpty(model.ExistingLoginData.Password) &&
                    model.ExistingLoginData.Password != model.ExistingLoginData.ConfirmPassword)
                {
                    return BadRequest(new
                    {
                        success = false,
                        error = "������ �� ���������"
                    });
                }

                // ����� ������������
                var existingUser = _dbcontext.������������
                    .FirstOrDefault(u => u.�����.Equals(model.HiddenSelectedUser));

                if (existingUser == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        error = "������������ �� ������"
                    });
                }

                // �������� �� ������������ ������
                if (model.ExistingLoginData.Login != model.HiddenSelectedUser)
                {
                    var userWithSameLogin = _dbcontext.������������
                        .FirstOrDefault(u => u.�����.Equals(model.ExistingLoginData.Login) &&
                                           u.����� != model.HiddenSelectedUser);

                    if (userWithSameLogin != null)
                    {
                        return BadRequest(new
                        {
                            success = false,
                            error = "������������ � ����� ������� ��� ����������"
                        });
                    }
                }

                // ���������� ������
                existingUser.����� = model.ExistingLoginData.Login;
                existingUser.���� = model.ExistingLoginData.SelectedRole;
                existingUser.��� = model.ExistingLoginData.FullName;

                if (!string.IsNullOrEmpty(model.ExistingLoginData.Password))
                {
                    existingUser.������ = Hasher.Hash(model.ExistingLoginData.Password);
                }

                _dbcontext.������������.Update(existingUser);
                await _dbcontext.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    message = "������������ ������� �������",
                    data = new
                    {
                        login = existingUser.�����,
                        selectedRole = existingUser.����,
                        fullName = existingUser.���,
                        hiddenSelectedUser = existingUser.�����
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    error = "���������� ������ ������� ��� ���������� ������������"
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
                // ��������� ������������ �����
                if (string.IsNullOrEmpty(selectedExistingUser))
                {
                    return BadRequest(new
                    {
                        success = false,
                        error = "�� ������ ������������ ��� ��������"
                    });
                }

                // ����� ������������
                var existingUser = _dbcontext.������������
                    .FirstOrDefault(u => u.�����.Equals(selectedExistingUser));

                if (existingUser == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        error = "������������ �� ������"
                    });
                }

                // ��������: ������ ������� ������ ����
                var currentUserLogin = User.FindFirst(JwtRegisteredClaimNames.UniqueName)?.Value;
                if (existingUser.�����.Equals(currentUserLogin))
                {
                    return BadRequest(new
                    {
                        success = false,
                        error = "������ ������� ����������� �������"
                    });
                }

                // ��������: ������ ������� ���������� ��������������
                if (existingUser.���� == "�������������")
                {
                    var adminCount = _dbcontext.������������
                        .Count(u => u.���� == "�������������");

                    if (adminCount <= 1)
                    {
                        return BadRequest(new
                        {
                            success = false,
                            error = "������ ������� ���������� ��������������"
                        });
                    }
                }

                // �������� ������������
                _dbcontext.������������.Remove(existingUser);
                await _dbcontext.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    message = "������������ ������ �������",
                    data = new
                    {
                        deletedUser = existingUser.�����
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    error = "���������� ������ ������� ��� �������� ������������"
                });
            }
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