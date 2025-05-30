using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevTools.Application.DTOs.Auth
{
    public class AuthResponseDto
    {
        public bool Success { get; set; }
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
        public UserInfoDto? User { get; set; }
        public string? ErrorMessage { get; set; }
    }

    public class UserInfoDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public int ApiUsageLimit { get; set; }
        public int ApiUsageUsed { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
