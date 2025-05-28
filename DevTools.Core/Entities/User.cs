using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevTools.Core.Entities
{
    public class User : IdentityUser<Guid>
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public int ApiUsageLimit { get; set; } = 100;
        public int ApiUsageUsed { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual ICollection<CodeAnalysisSession> CodeAnalysisSessions { get; set; } = new List<CodeAnalysisSession>();
        public virtual ICollection<UserProject> UserProjects { get; set; } = new List<UserProject>();
    }
}
