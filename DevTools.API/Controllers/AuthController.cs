using DevTools.Application.DTOs.Auth;
using DevTools.Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DevTools.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        /// <summary>
        /// Register a new user
        /// </summary>
        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterRequestDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _authService.RegisterAsync(request);

                if (!result.Success)
                    return BadRequest(result);

                _logger.LogInformation("User registered successfully: {Email}", request.Email);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user registration: {Email}", request.Email);
                return StatusCode(500, new { error = "An error occurred during registration" });
            }
        }

        /// <summary>
        /// Login user
        /// </summary>
        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginRequestDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _authService.LoginAsync(request);

                if (!result.Success)
                    return Unauthorized(result);

                _logger.LogInformation("User logged in successfully: {Email}", request.Email);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user login: {Email}", request.Email);
                return StatusCode(500, new { error = "An error occurred during login" });
            }
        }

        /// <summary>
        /// Refresh JWT token
        /// </summary>
        [HttpPost("refresh")]
        public async Task<ActionResult<AuthResponseDto>> RefreshToken([FromBody] string refreshToken)
        {
            try
            {
                var result = await _authService.RefreshTokenAsync(refreshToken);

                if (!result.Success)
                    return Unauthorized(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during token refresh");
                return StatusCode(500, new { error = "An error occurred during token refresh" });
            }
        }

        /// <summary>
        /// Revoke refresh token
        /// </summary>
        [HttpPost("revoke")]
        public async Task<IActionResult> RevokeToken([FromBody] string token)
        {
            try
            {
                var result = await _authService.RevokeTokenAsync(token);
                return Ok(new { success = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during token revocation");
                return StatusCode(500, new { error = "An error occurred during token revocation" });
            }
        }
    }
}

