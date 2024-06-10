using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetflixClone.Models;
using NetflixClone.Services.Contracts;

namespace NetflixClone.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController (IUserService userService, ILogger<UsersController> logger) : Controller
    {
        private readonly IUserService _userService = userService;
        private readonly ILogger<UsersController> _logger= logger;

        [HttpGet("search")]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<ActionResult>  SearchUsers(
            string? username = null,
            string? role = null,
            DateTime? expirationDate = null,
            bool? isPaid = null,
            string? subscriptionType = null,
            int pageIndex = 1,
            int pageSize = 10)
        {
            try
            {
                if (username != "super_admin" ) role = "client";

                var users = await _userService.Search(username, role, expirationDate, isPaid, subscriptionType, pageIndex, pageSize);
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error durante el uso de _userService.Search: {ex.Message}");

                return BadRequest(new { ex.Message });
            }
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<ActionResult> GetUserById(int id)
        {
            try {
                var user = await _userService.GetUserById(id);
                if (user == null)
                {
                    return NotFound();
                }
                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error durante el uso de _userService.GetUserById: {ex.Message}");

                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateUser(int id)
        {
            try {
                var username = HttpContext.User.Identity?.Name;
                var userSesion = (User?) await _userService.GetUserByUsername(username);
                var user = (User?) await _userService.GetUserById(id);
                if ( userSesion.Id == user.Id) {
                    await _userService.UpdateUser(user);
                    var updatedUser = await _userService.GetUserByUsername(user.Username);
                    return Ok(updatedUser);
                }
                return BadRequest(new { Message = "No coinciden registros" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error durante el uso de _userService.GetUserById: {ex.Message}");

                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<ActionResult> DeleteUser(int id)
        {
            try {
                var username = HttpContext.User.Identity?.Name;
                if (username == "super_admin") {
                var existingUser = await _userService.GetUserById(id);
                if (existingUser == null)
                {
                    return NotFound();
                }

                await _userService.DeleteUser(id);
                return Ok(new {Message = "User Deleted"});
                }
                return BadRequest(new { Message = "No eres super_admin" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error durante el uso de _userService.DeleteUser: {ex.Message}");

                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpDelete("/softdelete/{id}")]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<ActionResult> SoftDeleteUser(int id)
        {
            try {
                var username = HttpContext.User.Identity?.Name;
                var existingUser = await _userService.GetUserById(id);
                if (existingUser == null)
                {
                    return NotFound();
                }
                if (username != "super_admin" && existingUser.Role == "admin") return BadRequest(new { Message = "No eres super_admin" });
                existingUser.Role = null;
                await _userService.UpdateUser(existingUser);
                return Ok(new {Message = "User Disabled"});
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error durante el uso de _userService.UpdateUser in user softDeleted: {ex.Message}");

                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPut("/up-user/{id}/{role}")]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<ActionResult> UpUser(int id, string role)
        {
            try {
                var username = HttpContext.User.Identity?.Name;
                var existingUser = await _userService.GetUserById(id);
                if (existingUser == null)
                {
                    return NotFound();
                }
                if (username != "super_admin" && existingUser.Role == "admin") return BadRequest(new { Message = "No eres super_admin" });
                existingUser.Role = role;
                await _userService.UpdateUser(existingUser);
                return Ok(new {Message = "Ususario activado"});
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error durante el uso de _userService.UpdateUser in user softDeleted: {ex.Message}");

                return BadRequest(new { Message = ex.Message });
            }
        }
    }
}
