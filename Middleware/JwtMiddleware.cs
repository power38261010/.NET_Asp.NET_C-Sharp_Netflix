using Microsoft.AspNetCore.Http;
using NetflixClone.Services.Contracts;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NetflixClone.Middleware
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string[] _excludedPaths = { "/api/auth/login", "/api/auth/register", "/api/payments/ars", "/api/auth/validate-jwt" };
        private readonly ILogger<JwtMiddleware> _logger;

        public JwtMiddleware(RequestDelegate next, ILogger<JwtMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value;

            // Excluir rutas de registro y login
            if (_excludedPaths.Contains(path))
            {
                await _next(context);
                return;
            }

            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (token != null)
            {
                using (var scope = context.RequestServices.CreateScope())
                {
                    var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();

                    if (authService.ValidateJWTToken(token))
                    {
                        var userClaims = authService.GetUserClaimsFromToken(token);
                        var identity = new ClaimsIdentity(userClaims, "jwt");
                        context.User = new ClaimsPrincipal(identity);
                    }
                    else
                    {
                        _logger.LogWarning($"Token JWT no válido para la solicitud a: {context.Request.Path}");
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        return;
                    }
                }
            }
            else
            {
                _logger.LogWarning("No se proporcionó ningún token JWT en la solicitud.");
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }

            await _next(context);
        }
    }
}
