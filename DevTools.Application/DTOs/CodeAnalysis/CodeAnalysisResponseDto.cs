using DevTools.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevTools.Application.DTOs.CodeAnalysis
{
    public class CodeAnalysisResponseDto
    {
        public Guid SessionId { get; set; }
        public AnalysisType AnalysisType { get; set; }
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public CodeAnalysisResultDto? Result { get; set; }
        public int TokensUsed { get; set; }
        public decimal Cost { get; set; }
        public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;
        public AnalysisStatus Status { get; set; }
    }

    public class CodeAnalysisResultDto
    {
        public string Summary { get; set; } = string.Empty;
        public List<CodeIssueDto> Issues { get; set; } = new();
        public List<CodeSuggestionDto> Suggestions { get; set; } = new();
        public string? ImprovedCode { get; set; }
        public string? GeneratedDocumentation { get; set; }
        public string? GeneratedTests { get; set; }
        public Dictionary<string, object> Metadata { get; set; } = new();
        public AnalysisStatisticsDto Statistics { get; set; } = new();
    }

    public class CodeIssueDto
    {
        public Guid Id { get; set; }
        public IssueType Type { get; set; }
        public IssueCategory Category { get; set; }
        public string Description { get; set; } = string.Empty;
        public int LineNumber { get; set; }
        public int ColumnNumber { get; set; }
        public IssueSeverity Severity { get; set; }
        public string? FixSuggestion { get; set; }
        public bool IsFixed { get; set; }
    }

    public class CodeSuggestionDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public SuggestionCategory Category { get; set; }
        public string? CodeExample { get; set; }
        public int Priority { get; set; }
        public bool IsApplied { get; set; }
    }

    public class AnalysisStatisticsDto
    {
        public int TotalIssues { get; set; }
        public int CriticalIssues { get; set; }
        public int HighSeverityIssues { get; set; }
        public int MediumSeverityIssues { get; set; }
        public int LowSeverityIssues { get; set; }
        public int TotalSuggestions { get; set; }
        public double QualityScore { get; set; } // 0-100
        public Dictionary<string, int> IssuesByCategory { get; set; } = new();
    }
}
