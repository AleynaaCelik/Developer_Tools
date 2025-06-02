using DevTools.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevTools.Application.Interfaces.External
{
    public interface IOpenAIService
    {
        Task<string> AnalyzeCodeAsync(string code, ProgrammingLanguage language, AnalysisType analysisType, Dictionary<string, object>? options = null);
        Task<string> ReviewCodeAsync(string code, ProgrammingLanguage language, Dictionary<string, object>? options = null);
        Task<string> GenerateDocumentationAsync(string code, ProgrammingLanguage language, string documentationType = "inline", Dictionary<string, object>? options = null);
        Task<string> DetectBugsAsync(string code, ProgrammingLanguage language, Dictionary<string, object>? options = null);
        Task<string> GenerateTestsAsync(string code, ProgrammingLanguage language, string testFramework = "xunit", Dictionary<string, object>? options = null);
        Task<int> EstimateTokensAsync(string text);
        Task<bool> ValidateApiKeyAsync();
    }
}
