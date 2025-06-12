using DevTools.Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DevTools.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<UserController> _logger;

        public UserController(IAuthService authService, ILogger<UserController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        /// <summary>
        /// Get current user profile
        /// </summary>
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                var userId = GetUserId();
                var userInfo = await _authService.GetUserInfoAsync(userId);

                if (userInfo == null)
                    return NotFound("User not found");

                return Ok(userInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving profile for user {UserId}", GetUserId());
                return StatusCode(500, new { error = "An error occurred while retrieving user profile" });
            }
        }

        private Guid GetUserId()
        {
            var userIdClaim = User.FindFirst("userId")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                throw new UnauthorizedAccessException("Invalid user ID in token");
            }
            return userId;
        }
    }
}


