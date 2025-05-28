using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevTools.Core.Entities
{
    public class CodeIssue : BaseEntity
    {
        public Guid SessionId { get; set; }
        public IssueType Type { get; set; }
        public IssueCategory Category { get; set; }
        public string Description { get; set; } = string.Empty;
        public int LineNumber { get; set; }
        public int ColumnNumber { get; set; }
        public IssueSeverity Severity { get; set; }
        public string? FixSuggestion { get; set; }
        public bool IsFixed { get; set; } = false;

        // Navigation properties
        public virtual CodeAnalysisSession Session { get; set; } = null!;
    }
}
