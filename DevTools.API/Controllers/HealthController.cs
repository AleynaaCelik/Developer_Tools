using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DevTools.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        /// <summary>
        /// Check API health status
        /// </summary>
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new
            {
                status = "healthy",
                timestamp = DateTime.UtcNow,
                environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production",
                version = "1.0.0"
            });
        }

        /// <summary>
        /// Get API version information
        /// </summary>
        [HttpGet("version")]
        public IActionResult GetVersion()
        {
            return Ok(new
            {
                version = "1.0.0",
                buildDate = DateTime.UtcNow.ToString("yyyy-MM-dd"),
                dotnetVersion = Environment.Version.ToString(),
                architecture = Environment.Is64BitProcess ? "x64" : "x86"
            });
        }
    }
}

