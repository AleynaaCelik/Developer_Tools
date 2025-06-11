using DevTools.Application.DTOs.CodeAnalysis;
using DevTools.Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DevTools.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CodeAnalysisController : ControllerBase
    {
        private readonly ICodeAnalysisService _codeAnalysisService;
        private readonly ILogger<CodeAnalysisController> _logger;

        public CodeAnalysisController(
            ICodeAnalysisService codeAnalysisService,
            ILogger<CodeAnalysisController> logger)
        {
            _codeAnalysisService = codeAnalysisService;
            _logger = logger;
        }

        /// <summary>
        /// Perform general code analysis
        /// </summary>
        [HttpPost("analyze")]
        public async Task<ActionResult<CodeAnalysisResponseDto>> AnalyzeCode([FromBody] CodeAnalysisRequestDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var userId = GetUserId();
                var result = await _codeAnalysisService.AnalyzeCodeAsync(userId, request);

                if (!result.Success)
                    return BadRequest(result);

                _logger.LogInformation("Code analysis completed for user {UserId}, session {SessionId}", userId, result.SessionId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during code analysis for user {UserId}", GetUserId());
                return StatusCode(500, new { error = "An error occurred during code analysis" });
            }
        }

        /// <summary>
        /// Perform code review
        /// </summary>
        [HttpPost("review")]
        public async Task<ActionResult<CodeAnalysisResponseDto>> ReviewCode([FromBody] CodeReviewRequestDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var userId = GetUserId();
                var result = await _codeAnalysisService.ReviewCodeAsync(userId, request);

                if (!result.Success)
                    return BadRequest(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during code review for user {UserId}", GetUserId());
                return StatusCode(500, new { error = "An error occurred during code review" });
            }
        }

        /// <summary>
        /// Generate documentation
        /// </summary>
        [HttpPost("documentation")]
        public async Task<ActionResult<CodeAnalysisResponseDto>> GenerateDocumentation([FromBody] DocumentationRequestDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var userId = GetUserId();
                var result = await _codeAnalysisService.GenerateDocumentationAsync(userId, request);

                if (!result.Success)
                    return BadRequest(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during documentation generation for user {UserId}", GetUserId());
                return StatusCode(500, new { error = "An error occurred during documentation generation" });
            }
        }

        /// <summary>
        /// Detect bugs in code
        /// </summary>
        [HttpPost("bugs")]
        public async Task<ActionResult<CodeAnalysisResponseDto>> DetectBugs([FromBody] BugDetectionRequestDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var userId = GetUserId();
                var result = await _codeAnalysisService.DetectBugsAsync(userId, request);

                if (!result.Success)
                    return BadRequest(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during bug detection for user {UserId}", GetUserId());
                return StatusCode(500, new { error = "An error occurred during bug detection" });
            }
        }

        /// <summary>
        /// Generate test cases
        /// </summary>
        [HttpPost("tests")]
        public async Task<ActionResult<CodeAnalysisResponseDto>> GenerateTests([FromBody] TestGenerationRequestDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var userId = GetUserId();
                var result = await _codeAnalysisService.GenerateTestsAsync(userId, request);

                if (!result.Success)
                    return BadRequest(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during test generation for user {UserId}", GetUserId());
                return StatusCode(500, new { error = "An error occurred during test generation" });
            }
        }

        /// <summary>
        /// Get user's analysis history
        /// </summary>
        [HttpGet("history")]
        public async Task<ActionResult<IEnumerable<CodeAnalysisResponseDto>>> GetAnalysisHistory([FromQuery] int? limit = null)
        {
            try
            {
                var userId = GetUserId();
                var result = await _codeAnalysisService.GetUserAnalysisHistoryAsync(userId, limit);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving analysis history for user {UserId}", GetUserId());
                return StatusCode(500, new { error = "An error occurred while retrieving analysis history" });
            }
        }

        /// <summary>
        /// Get specific analysis session
        /// </summary>
        [HttpGet("{sessionId}")]
        public async Task<ActionResult<CodeAnalysisResponseDto>> GetAnalysisSession(Guid sessionId)
        {
            try
            {
                var userId = GetUserId();
                var result = await _codeAnalysisService.GetAnalysisSessionAsync(sessionId, userId);

                if (result == null)
                    return NotFound("Analysis session not found");

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving analysis session {SessionId} for user {UserId}", sessionId, GetUserId());
                return StatusCode(500, new { error = "An error occurred while retrieving analysis session" });
            }
        }

        /// <summary>
        /// Delete analysis session
        /// </summary>
        [HttpDelete("{sessionId}")]
        public async Task<IActionResult> DeleteAnalysisSession(Guid sessionId)
        {
            try
            {
                var userId = GetUserId();
                var result = await _codeAnalysisService.DeleteAnalysisSessionAsync(sessionId, userId);

                if (!result)
                    return NotFound("Analysis session not found");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting analysis session {SessionId} for user {UserId}", sessionId, GetUserId());
                return StatusCode(500, new { error = "An error occurred while deleting analysis session" });
            }
        }

        /// <summary>
        /// Check user API usage limits
        /// </summary>
        [HttpGet("usage")]
        public async Task<ActionResult> GetUsageInfo()
        {
            try
            {
                var userId = GetUserId();
                var canUse = await _codeAnalysisService.CheckUserLimitAsync(userId);

                return Ok(new { canUseService = canUse, userId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking usage limits for user {UserId}", GetUserId());
                return StatusCode(500, new { error = "An error occurred while checking usage limits" });
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

