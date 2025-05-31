using DevTools.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevTools.Application.DTOs.CodeAnalysis
{
    public class CodeAnalysisRequestDto
    {
        [Required]
        public string Code { get; set; } = string.Empty;

        [Required]
        public ProgrammingLanguage Language { get; set; }

        [Required]
        [MaxLength(255)]
        public string FileName { get; set; } = string.Empty;

        [Required]
        public AnalysisType AnalysisType { get; set; }

        public Guid? ProjectId { get; set; }

        public Dictionary<string, object>? Options { get; set; }
    }

    public class CodeReviewRequestDto : CodeAnalysisRequestDto
    {
        public bool IncludePerformance { get; set; } = true;
        public bool IncludeSecurity { get; set; } = true;
        public bool IncludeCodeStyle { get; set; } = true;
        public bool IncludeOptimization { get; set; } = true;
        public bool IncludeArchitecture { get; set; } = true;
    }

    public class DocumentationRequestDto : CodeAnalysisRequestDto
    {
        public string DocumentationType { get; set; } = "inline"; // inline, readme, api-doc
        public bool IncludeExamples { get; set; } = true;
        public bool IncludeTypeDefinitions { get; set; } = true;
    }

    public class TestGenerationRequestDto : CodeAnalysisRequestDto
    {
        public string TestFramework { get; set; } = "xunit"; // xunit, nunit, jest, mocha
        public bool IncludeMocks { get; set; } = true;
        public bool IncludeIntegrationTests { get; set; } = false;
        public bool IncludeEdgeCases { get; set; } = true;
    }

    public class BugDetectionRequestDto : CodeAnalysisRequestDto
    {
        public bool CheckNullReferences { get; set; } = true;
        public bool CheckMemoryLeaks { get; set; } = true;
        public bool CheckLogicErrors { get; set; } = true;
        public bool CheckSecurityVulnerabilities { get; set; } = true;
    }
}
