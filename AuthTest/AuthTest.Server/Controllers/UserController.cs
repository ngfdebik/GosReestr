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
    [Authorize(Policy = "MustBeLoggedIn")]
    public class UserController : ControllerBase
    {
        private readonly DbContextTable _dbcontext;

        public UserController(DbContextTable dbcontext)
        {
            _dbcontext = dbcontext;
        }

        // GET: api/user/current
        [HttpGet("current")]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            try
            {
                var userLogin = User.FindFirst(ClaimTypes.GivenName)?.Value;

                if (string.IsNullOrEmpty(userLogin))
                {
                    return Unauthorized("������������ �� �����������");
                }

                var user = await _dbcontext.������������
                    .FirstOrDefaultAsync(u => u.�����.Equals(userLogin));

                if (user == null)
                {
                    return NotFound("������������ �� ������");
                }

                return Ok(new UserDto
                {
                    Login = user.�����,
                    FullName = user.���,
                    Role = user.����
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"������ �������: {ex.Message}");
            }
        }

        // POST: api/user/edit
        [HttpPost("edit")]
        public async Task<ActionResult<ApiResponse>> EditUser([FromBody] UserEditRequest request)
        {
            try
            {
                // ��������� ������
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse
                    {
                        Success = false,
                        Message = "������ ����� ���������"
                    });
                }

                var userLogin = User.FindFirst(ClaimTypes.GivenName)?.Value;

                if (string.IsNullOrEmpty(userLogin) || userLogin != request.Login)
                {
                    return BadRequest(new ApiResponse
                    {
                        Success = false,
                        Message = "������ ��������� ������������"
                    });
                }

                // �������� ���������� �������
                if (request.Password != request.ConfirmPassword)
                {
                    return BadRequest(new ApiResponse
                    {
                        Success = false,
                        Message = "������ �� ���������"
                    });
                }

                var user = await _dbcontext.������������
                    .FirstOrDefaultAsync(u => u.�����.Equals(request.Login));

                if (user == null)
                {
                    return NotFound(new ApiResponse
                    {
                        Success = false,
                        Message = "������������ �� ������"
                    });
                }

                // ���������� ������
                if (!string.IsNullOrEmpty(request.FullName))
                {
                    user.��� = request.FullName;
                }

                if (!string.IsNullOrEmpty(request.Password))
                {
                    user.������ = Hasher.Hash(request.Password);
                }

                _dbcontext.������������.Update(user);
                await _dbcontext.SaveChangesAsync();

                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "������������ ������� �������"
                });
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new ApiResponse
                {
                    Success = false,
                    Message = "������ ���� ������ ��� ���������� ������������"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse
                {
                    Success = false,
                    Message = "������ ��������� ������������"
                });
            }
        }
    }

    // DTO ������
    public class UserDto
    {
        public string Login { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; }
    }

    public class UserEditRequest
    {
        [Required(ErrorMessage = "����� ����������")]
        public string Login { get; set; }

        public string Password { get; set; }

        public string ConfirmPassword { get; set; }

        public string FullName { get; set; }
    }

    public class ApiResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}