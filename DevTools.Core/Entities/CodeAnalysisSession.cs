using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevTools.Core.Entities
{
    public class CodeAnalysisSession : BaseEntity
    {
        public Guid UserId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public ProgrammingLanguage Language { get; set; }
        public string OriginalCode { get; set; } = string.Empty;
        public string? AnalysisResult { get; set; }
        public AnalysisType AnalysisType { get; set; }
        public int TokensUsed { get; set; }
        public decimal Cost { get; set; }
        public AnalysisStatus Status { get; set; } = AnalysisStatus.Pending;
        public string? ErrorMessage { get; set; }
        public Guid? ProjectId { get; set; }

        // Navigation properties
        public virtual User User { get; set; } = null!;
        public virtual UserProject? Project { get; set; }
        public virtual ICollection<CodeIssue> Issues { get; set; } = new List<CodeIssue>();
        public virtual ICollection<CodeSuggestion> Suggestions { get; set; } = new List<CodeSuggestion>();
    }
}
