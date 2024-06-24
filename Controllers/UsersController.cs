using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetflixClone.Controllers.ModelRequest;
using NetflixClone.Models;
using NetflixClone.Services.Contracts;

namespace NetflixClone.Controllers {
    [Route("api/users")]
    [ApiController]
    public class UsersController (IUserService userService,IAuthService authService, ILogger<UsersController> logger) : Controller {
        private readonly IUserService _userService = userService;
        private readonly IAuthService _authService = authService;
        private readonly ILogger<UsersController> _logger= logger;

        [HttpGet("search")]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<ActionResult>  SearchUsers(
            string? searchTerm = null,
            string? role = null,
            DateTime? expirationDate = null,
            bool? isPaid = null,
            string? subscriptionType = null,
            int pageIndex = 1,
            int pageSize = 20) {
            try {
                var usernameSesion = HttpContext.User.Identity?.Name;
                var userSesion = await _userService.GetUserByUsername(usernameSesion);
                if (userSesion.Role != "super_admin" ) role = "client";

                var users = await _userService.Search(searchTerm, role, expirationDate, isPaid, subscriptionType, pageIndex, pageSize);
                return Ok(users);
            } catch (Exception ex) {
                _logger.LogError($"Error durante el uso de _userService.Search: {ex.Message}");

                return BadRequest(new { ex.Message });
            }
        }

        [HttpGet]
        [Authorize(Policy = "SuperAdminPolicy")]
        public async Task<ActionResult> GetAll() {
            try {

                    var users = await _userService.GetAll();
                    return Ok(users);

            } catch (Exception ex) {
                _logger.LogError($"Error durante el uso de _userService.GetUserById: {ex.Message}");

                return BadRequest(new { Message = ex.Message });
            }
        }


        [HttpGet("{id}")]
        public async Task<ActionResult> GetUserById(int id) {
            try {
                var username = HttpContext.User.Identity?.Name;
                var userSesion = (User?) await _userService.GetUserByUsername(username);
                if (userSesion.Id == id) {
                    var user = await _userService.GetUserById(id);
                    if (user == null) {
                        return NotFound();
                    }
                    var userSend = await _userService.GetUserDTO(user);
                    return Ok(userSend);
                }
                return BadRequest(new { Message = "No coincide el usuario al que editas" });
            } catch (Exception ex) {
                _logger.LogError($"Error durante el uso de _userService.GetUserById: {ex.Message}");

                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateUser(int id,[FromBody] UserRequest user) {
            try {
                var username = HttpContext.User.Identity?.Name;
                var userSesion = (User?) await _userService.GetUserByUsername(username);
                var userUpdate = (User?) await _userService.GetUserById(id);
                var validateUser = await _userService.Authenticate(username, user.PasswordHash);
                if ( userSesion == null || userUpdate == null || (user.PasswordHash != "" && validateUser == null)) return BadRequest(new { Message = "No coincide la contraseña a modificar" });
                if ( userSesion.Id == userUpdate.Id ) {
                    var Username = user.Username ?? null;
                    var PasswordHashNew = "";
                    if ( user.PasswordHashNew != "") PasswordHashNew = user.PasswordHashNew;
                    var Email = user.Email ?? null;
                    await _userService.UpdateUser(id,Username,PasswordHashNew,Email);
                    var updatedUser = await _userService.GetUserByUsername(user.Username);
                    var userSend = await _userService.GetUserDTO(updatedUser);
                    var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                    await _authService.Logout(token);
                    var newToken = _authService.GenerateJwtToken(userSend.Username, userSend.Role);
                    return Ok(new { Profile = userSend, Token = newToken });
                }
            return BadRequest(new { Message = "No coincide el user sesion respecto del user a modificar" });
            } catch (Exception ex) {
                _logger.LogError($"Error durante el uso de _userService.GetUserById: {ex.Message}");

                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "SuperAdminPolicy")]
        public async Task<ActionResult> DeleteUser(int id) {
            try {
                var queryProtected = await ProtectedAction (id);
                if (queryProtected != null) return queryProtected;

                await _userService.DeleteUser(id);
                return Ok(new {Message = "User Deleted"});
            } catch (Exception ex) {
                _logger.LogError($"Error durante el uso de _userService.DeleteUser: {ex.Message}");
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPut("softdelete/{id}")]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<ActionResult> SoftDeleteUser(int id) {
        try {

            var queryProtected = await ProtectedAction(id);
            _logger.LogWarning($"Error during _userService.UpdateUser in user softDeleted: {queryProtected}");
            if (queryProtected != null) return queryProtected;
            await _userService.UpdateRoleUser(id, null);
            return Ok(new { Message = "User Disabled" });
        } catch (Exception ex) {
            _logger.LogError($"Error during _userService.UpdateUser in user softDeleted: {ex.Message}");
            return BadRequest(new { Message = ex.Message });
        }
        }


        [HttpPut("up-user/{id}/{role}")]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<ActionResult> UpUser(int id, string role) {
            try {
                var queryProtected = await ProtectedAction (id);
            _logger.LogWarning($"Error during _userService.UpdateUser in user softDeleted: {queryProtected}");

                if (queryProtected != null) return queryProtected;
                await _userService.UpdateRoleUser(id,role);
                return Ok(new {Message = "Ususario activado"});
            } catch (Exception ex) {
                _logger.LogError($"Error durante el uso de _userService.UpdateUser in user softDeleted: {ex.Message}");

                return BadRequest(new { Message = ex.Message });
            }
        }

        public async Task<ActionResult?> ProtectedAction (int id) {
            try {
                var username = HttpContext.User.Identity?.Name;
                var userSesion = await _userService.GetUserByUsername(username);
                var existingUser = await _userService.GetUserById(id);
                if (existingUser == null || existingUser.Role == "super_admin") {
                    return NotFound();
                }
                if (userSesion.Role != "super_admin" && existingUser.Role == "admin") return BadRequest(new { Message = "No eres super_admin" });
                return null;
                } catch (Exception ex) {
                _logger.LogError($"Error durante el uso de _userService.GetUserByUsername in user ProtectedAction: {ex.Message}");

                return BadRequest(new { Message = ex.Message });
            }
        }
    }
}
