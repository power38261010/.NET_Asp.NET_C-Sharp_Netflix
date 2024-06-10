using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NetflixClone.Models;
using NetflixClone.Services;
using NetflixClone.Services.Contracts;
using System;
using System.Threading.Tasks;
using NetflixClone.Controllers.ModelRequest;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;

namespace NetflixClone.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController( IUserService userService, IAuthService authService, ILogger<AuthController> logger)
        {
            _userService = userService;
            _authService = authService;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AuthRequest credentials)
        {
            try
            {
                var user = await _userService.Authenticate(credentials.Username, credentials.PasswordHash);
                if (user == null)
                {
                    return BadRequest(new { Message = "User not found" });
                }

                var token = _authService.GenerateJwtToken(user);
                return Ok(new { Token = token, Profile = user });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error durante el inicio de sesión: {ex.Message}");
                return BadRequest(new { Message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User user)
        {

            try
            {
                var registeredUser = await _userService.Register(user);
                var token = _authService.GenerateJwtToken(registeredUser);
                return Ok(new { Token = token, Profile = registeredUser});
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error durante el registro: {ex.Message}");
                return BadRequest(new { ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost("validate-jwt")]
        public  IActionResult ValidateJWT([FromBody] TokenRequest request) {
            bool validated = _authService.ValidateJWTToken(request.Token);
            return Ok(new { isValidated = validated });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            try
            {
                await _authService.Logout(token);
                return Ok(new { Message = "Logged out successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
    }
}
