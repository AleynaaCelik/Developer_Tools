using Atlassian.Jira;
using DevTools.Application.DTOs.Project;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DevTools.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectService _projectService;
        private readonly ILogger<ProjectController> _logger;

        public ProjectController(IProjectService projectService, ILogger<ProjectController> logger)
        {
            _projectService = projectService;
            _logger = logger;
        }

        /// <summary>
        /// Create a new project
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ProjectDto>> CreateProject([FromBody] CreateProjectRequestDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var userId = GetUserId();
                var result = await _projectService.CreateProjectAsync(userId, request);

                return CreatedAtAction(nameof(GetProject), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating project for user {UserId}", GetUserId());
                return StatusCode(500, new { error = "An error occurred while creating the project" });
            }
        }

        /// <summary>
        /// Get user's projects
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProjectDto>>> GetProjects([FromQuery] bool activeOnly = false)
        {
            try
            {
                var userId = GetUserId();
                var projects = activeOnly
                    ? await _projectService.GetActiveProjectsAsync(userId)
                    : await _projectService.GetUserProjectsAsync(userId);

                return Ok(projects);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving projects for user {UserId}", GetUserId());
                return StatusCode(500, new { error = "An error occurred while retrieving projects" });
            }
        }

        /// <summary>
        /// Get specific project
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ProjectDto>> GetProject(Guid id)
        {
            try
            {
                var userId = GetUserId();
                var project = await _projectService.GetProjectAsync(userId, id);

                if (project == null)
                    return NotFound("Project not found");

                return Ok(project);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving project {ProjectId} for user {UserId}", id, GetUserId());
                return StatusCode(500, new { error = "An error occurred while retrieving the project" });
            }
        }

        /// <summary>
        /// Update project
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<ProjectDto>> UpdateProject(Guid id, [FromBody] UpdateProjectRequestDto request)
        {
            try
            {
                if (id != request.Id)
                    return BadRequest("Project ID mismatch");

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var userId = GetUserId();
                var result = await _projectService.UpdateProjectAsync(userId, request);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating project {ProjectId} for user {UserId}", id, GetUserId());
                return StatusCode(500, new { error = "An error occurred while updating the project" });
            }
        }

        /// <summary>
        /// Delete project
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(Guid id)
        {
            try
            {
                var userId = GetUserId();
                var result = await _projectService.DeleteProjectAsync(userId, id);

                if (!result)
                    return NotFound("Project not found");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting project {ProjectId} for user {UserId}", id, GetUserId());
                return StatusCode(500, new { error = "An error occurred while deleting the project" });
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

