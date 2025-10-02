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
                var user = await _dbcontext.������������.FirstOrDefaultAsync(u => u.����� == login);//User.FindFirst(ClaimTypes.GivenName)?.Value;

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
        [HttpPut("edit")]
        public async Task<ActionResult> EditUser([FromBody] UserEditRequest request)
        {
            try
            {
                // ��������� ������
                if (!ModelState.IsValid)
                {
                    return BadRequest(new 
                    {
                        success = false,
                        message = "������ ����� ���������"
                    });
                }

                var user = await _dbcontext.������������
                    .FirstOrDefaultAsync(u => u.�����.Equals(request.Login));

                if (string.IsNullOrEmpty(user.�����))
                {
                    return BadRequest(new 
                    {
                        success = false,
                        message = "������ ��������� ������������"
                    });
                }

                // �������� ���������� �������
                if (request.Password != request.ConfirmPassword)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "������ �� ���������"
                    });
                }

                //var user = await _dbcontext.������������
                //    .FirstOrDefaultAsync(u => u.�����.Equals(request.Login));

                if (user == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        sessage = "������������ �� ������"
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

                return Ok(new
                {
                    success = true,
                    message = "������������ ������� �������"
                });
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new 
                {
                    success = false,
                    message = "������ ���� ������ ��� ���������� ������������"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new 
                {
                    success = false,
                    message = "������ ��������� ������������"
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

    //public class ApiResponse
    //{
    //    public bool Success { get; set; }
    //    public string Message { get; set; }
    //}
}