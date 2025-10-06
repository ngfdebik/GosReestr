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
            return Ok(new
            {
                Roles = GetRolesSelectList(),
                ExistingUsers = GetExistingUsersSelectList()
            });
        }

        // POST: User/Create
        [HttpPost("Create")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromBody] CreateUserRequest req)
        {
            try
            {
                // ��������� ������
                //if (model?.NewLoginData == null)
                //{
                //    return BadRequest(new
                //    {
                //        success = false,
                //        error = "�������� ������ ������"
                //    });
                //}

                // ��������� ������������ �����
                if (string.IsNullOrEmpty(req.Login))
                {
                    return BadRequest(new
                    {
                        success = false,
                        error = "����� ����������"
                    });
                }

                if (string.IsNullOrEmpty(req.Password))
                {
                    return BadRequest(new
                    {
                        success = false,
                        error = "������ ����������"
                    });
                }

                if (string.IsNullOrEmpty(req.SelectedRole))
                {
                    return BadRequest(new
                    {
                        success = false,
                        error = "���� �����������"
                    });
                }

                // �������� ����� ������
                if (req.Password.Length < 6)
                {
                    return BadRequest(new
                    {
                        success = false,
                        error = "������ ������ ��������� �� ����� 6 ��������"
                    });
                }

                // �������� ���������� �������
                if (req.Password != req.ConfirmPassword)
                {
                    return BadRequest(new
                    {
                        success = false,
                        error = "������ �� ���������"
                    });
                }

                // �������� ������������ ������
                if (_dbcontext.������������.Any(u => u.�����.Equals(req.Login)))
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
                    ����� = req.Login,
                    ������ = Hasher.Hash(req.Password),
                    ���� = req.SelectedRole,
                    ��� = req.FullName ?? string.Empty
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
        public async Task<IActionResult> Update([FromBody] UpdateUserRequest req)
        {
            try
            {
                // ��������� ������
                if (req == null)
                {
                    return BadRequest(new
                    {
                        success = false,
                        error = "�������� ������ ������"
                    });
                }

                if (string.IsNullOrEmpty(req.HiddenSelectedUser))
                {
                    return BadRequest(new
                    {
                        success = false,
                        error = "�� ������ ������������ ��� ���������"
                    });
                }

                if (string.IsNullOrEmpty(req.Login))
                {
                    return BadRequest(new
                    {
                        success = false,
                        error = "����� ����������"
                    });
                }

                if (string.IsNullOrEmpty(req.SelectedRole))
                {
                    return BadRequest(new
                    {
                        success = false,
                        error = "���� �����������"
                    });
                }

                // �������� ���������� �������
                if (!string.IsNullOrEmpty(req.Password) &&
                    req.Password != req.ConfirmPassword)
                {
                    return BadRequest(new
                    {
                        success = false,
                        error = "������ �� ���������"
                    });
                }

                // ����� ������������
                var existingUser = _dbcontext.������������
                    .FirstOrDefault(u => u.�����.Equals(req.HiddenSelectedUser));

                if (existingUser == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        error = "������������ �� ������"
                    });
                }

                // �������� �� ������������ ������
                if (req.Login != req.HiddenSelectedUser)
                {
                    var userWithSameLogin = _dbcontext.������������
                        .FirstOrDefault(u => u.�����.Equals(req.Login) &&
                                           u.����� != req.HiddenSelectedUser);

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
                existingUser.����� = req.Login;
                existingUser.���� = req.SelectedRole;
                existingUser.��� = req.FullName;

                if (!string.IsNullOrEmpty(req.Password))
                {
                    existingUser.������ = Hasher.Hash(req.Password);
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
    //public class UserManageModel
    //{
    //    public FullCredential NewLoginData { get; set; } = new FullCredential();
    //    public FullCredential ExistingLoginData { get; set; } = new FullCredential();
    //    public List<SelectListItem> Roles { get; set; }
    //    public List<SelectListItem> ExistingUsers { get; set; }
    //    public string SelectedExistingUser { get; set; }
    //    public string HiddenSelectedUser { get; set; }

    //    [TempData]
    //    public string UserCreateStatusMessage { get; set; }

    //    [TempData]
    //    public string UserEditStatusMessage { get; set; }

    //    [TempData]
    //    public string MessageType { get; set; }
    //}
    public class UpdateUserRequest
    {
        public string HiddenSelectedUser { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string SelectedRole { get; set; }
        public string FullName { get; set; }
    }

    public class CreateUserRequest
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string SelectedRole { get; set; }
        public string FullName { get; set; }
    }

    // ������ ������ (����� ������� � ��������� ����)
    //public class FullCredential
    //{
    //    [Required(ErrorMessage = "����� ����������")]
    //    public string Login { get; set; }

    //    [Required(ErrorMessage = "������ ����������")]
    //    [DataType(DataType.Password)]
    //    public string Password { get; set; }

    //    [Required(ErrorMessage = "����������� ������")]
    //    [DataType(DataType.Password)]
    //    [Compare("Password", ErrorMessage = "������ �� ���������")]
    //    public string ConfirmPassword { get; set; }

    //    public string FullName { get; set; }

    //    [Required(ErrorMessage = "���� �����������")]
    //    public string SelectedRole { get; set; }
    //}
}