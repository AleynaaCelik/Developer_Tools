using DevTools.Application.Interfaces.External;
using DevTools.Core.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DevTools.Infrastructure.Services
{
    public class OpenAIService : IOpenAIService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<OpenAIService> _logger;
        private readonly string _apiKey;
        private readonly string _baseUrl = "https://api.openai.com/v1";

        public OpenAIService(HttpClient httpClient, IConfiguration configuration, ILogger<OpenAIService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
            _apiKey = _configuration["OpenAI:ApiKey"] ?? throw new InvalidOperationException("OpenAI API key not found");

            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
        }

        public async Task<string> AnalyzeCodeAsync(string code, ProgrammingLanguage language, AnalysisType analysisType, Dictionary<string, object>? options = null)
        {
            var prompt = analysisType switch
            {
                AnalysisType.Review => BuildCodeReviewPrompt(code, language, options),
                AnalysisType.Documentation => BuildDocumentationPrompt(code, language, options),
                AnalysisType.BugDetection => BuildBugDetectionPrompt(code, language, options),
                AnalysisType.TestGeneration => BuildTestGenerationPrompt(code, language, options),
                AnalysisType.Refactoring => BuildRefactoringPrompt(code, language, options),
                AnalysisType.Performance => BuildPerformancePrompt(code, language, options),
                AnalysisType.Security => BuildSecurityPrompt(code, language, options),
                _ => throw new ArgumentException($"Unknown analysis type: {analysisType}")
            };

            return await CallOpenAIAsync(prompt);
        }

        public async Task<string> ReviewCodeAsync(string code, ProgrammingLanguage language, Dictionary<string, object>? options = null)
        {
            var prompt = BuildCodeReviewPrompt(code, language, options);
            return await CallOpenAIAsync(prompt);
        }

        public async Task<string> GenerateDocumentationAsync(string code, ProgrammingLanguage language, string documentationType = "inline", Dictionary<string, object>? options = null)
        {
            var opts = options ?? new Dictionary<string, object>();
            opts["documentationType"] = documentationType;

            var prompt = BuildDocumentationPrompt(code, language, opts);
            return await CallOpenAIAsync(prompt);
        }

        public async Task<string> DetectBugsAsync(string code, ProgrammingLanguage language, Dictionary<string, object>? options = null)
        {
            var prompt = BuildBugDetectionPrompt(code, language, options);
            return await CallOpenAIAsync(prompt);
        }

        public async Task<string> GenerateTestsAsync(string code, ProgrammingLanguage language, string testFramework = "xunit", Dictionary<string, object>? options = null)
        {
            var opts = options ?? new Dictionary<string, object>();
            opts["testFramework"] = testFramework;

            var prompt = BuildTestGenerationPrompt(code, language, opts);
            return await CallOpenAIAsync(prompt);
        }

        public async Task<int> EstimateTokensAsync(string text)
        {
            // Simple token estimation (GPT-4 approximation)
            return (int)Math.Ceiling(text.Length / 4.0);
        }

        public async Task<bool> ValidateApiKeyAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/models");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        private async Task<string> CallOpenAIAsync(string prompt)
        {
            try
            {
                var model = _configuration["OpenAI:Model"] ?? "gpt-4";
                var maxTokens = int.Parse(_configuration["OpenAI:MaxTokens"] ?? "2000");
                var temperature = double.Parse(_configuration["OpenAI:Temperature"] ?? "0.3");

                var requestBody = new
                {
                    model = model,
                    messages = new[]
                    {
                        new { role = "system", content = "You are an expert software developer and code reviewer. Provide detailed, actionable feedback in JSON format when requested." },
                        new { role = "user", content = prompt }
                    },
                    max_tokens = maxTokens,
                    temperature = temperature
                };

                var json = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_baseUrl}/chat/completions", content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("OpenAI API error: {StatusCode} - {Content}", response.StatusCode, errorContent);
                    throw new HttpRequestException($"OpenAI API returned {response.StatusCode}: {errorContent}");
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<JsonElement>(responseContent);

                return result.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString() ?? "";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling OpenAI API");
                throw;
            }
        }

        private string BuildCodeReviewPrompt(string code, ProgrammingLanguage language, Dictionary<string, object>? options)
        {
            var languageStr = language.ToString().ToLowerInvariant();
            var includePerformance = options?.GetValueOrDefault("IncludePerformance", true) as bool? ?? true;
            var includeSecurity = options?.GetValueOrDefault("IncludeSecurity", true) as bool? ?? true;
            var includeCodeStyle = options?.GetValueOrDefault("IncludeCodeStyle", true) as bool? ?? true;
            var includeOptimization = options?.GetValueOrDefault("IncludeOptimization", true) as bool? ?? true;

            var focusAreas = new List<string>();
            if (includePerformance) focusAreas.Add("performance optimization");
            if (includeSecurity) focusAreas.Add("security vulnerabilities");
            if (includeCodeStyle) focusAreas.Add("code style and best practices");
            if (includeOptimization) focusAreas.Add("optimization opportunities");

            return $@"
Please review the following {languageStr} code and provide a comprehensive analysis focusing on: {string.Join(", ", focusAreas)}.

Please respond in JSON format with the following structure:
{{
    ""summary"": ""Brief summary of overall code quality and main findings"",
    ""qualityScore"": number (0-100),
    ""issues"": [
        {{
            ""type"": ""error|warning|info"",
            ""category"": ""performance|security|style|logic|memory|threading|exception|validation"",
            ""description"": ""Detailed issue description"",
            ""lineNumber"": number,
            ""columnNumber"": number,
            ""severity"": ""low|medium|high|critical"",
            ""fixSuggestion"": ""Specific suggestion on how to fix this issue""
        }}
    ],
    ""suggestions"": [
        {{
            ""title"": ""Improvement suggestion title"",
            ""description"": ""Detailed description of the improvement"",
            ""category"": ""performance|readability|maintainability|bestPractices|refactoring|testing|documentation"",
            ""codeExample"": ""Example code showing the improvement (if applicable)"",
            ""priority"": number (1-5, where 5 is highest priority)
        }}
    ],
    ""improvedCode"": ""Optimized version of the entire code with improvements applied (if applicable)""
}}

Code to review:
```{languageStr}
{code}
```";
        }

        private string BuildDocumentationPrompt(string code, ProgrammingLanguage language, Dictionary<string, object>? options)
        {
            var languageStr = language.ToString().ToLowerInvariant();
            var documentationType = options?.GetValueOrDefault("documentationType", "inline") as string ?? "inline";
            var includeExamples = options?.GetValueOrDefault("IncludeExamples", true) as bool? ?? true;

            return documentationType.ToLower() switch
            {
                "inline" => $@"
Generate comprehensive inline documentation for the following {languageStr} code.
Include:
- Function/method descriptions
- Parameter descriptions with types
- Return value descriptions
- Exception documentation
- {(includeExamples ? "Usage examples where appropriate" : "")}

Please provide the documented code with proper documentation comments for {languageStr}.

Code:
```{languageStr}
{code}
```",

                "readme" => $@"
Generate a comprehensive README.md file for the following {languageStr} code.
Include:
- Project title and description
- Installation/setup instructions
- Usage examples and API documentation
- Dependencies and requirements
- Contributing guidelines
- License information (if applicable)

Code:
```{languageStr}
{code}
```",

                "api-doc" => $@"
Generate API documentation for the following {languageStr} code.
Focus on:
- Endpoint descriptions and purposes
- Request/response formats and schemas
- HTTP status codes and error handling
- Authentication and authorization requirements
- Rate limiting and usage guidelines
- Example requests and responses

Code:
```{languageStr}
{code}
```",

                _ => throw new ArgumentException($"Unknown documentation type: {documentationType}")
            };
        }

        private string BuildBugDetectionPrompt(string code, ProgrammingLanguage language, Dictionary<string, object>? options)
        {
            var languageStr = language.ToString().ToLowerInvariant();

            return $@"
Analyze the following {languageStr} code for potential bugs, errors, and issues.
Look for:
- Null reference exceptions and null pointer dereferences
- Memory leaks and resource management issues
- Logic errors and incorrect algorithms
- Race conditions and threading issues
- Exception handling problems
- Input validation vulnerabilities
- Off-by-one errors and boundary conditions
- Type conversion issues

Please respond in JSON format:
{{
    ""summary"": ""Overall assessment of code stability and bug risk"",
    ""riskLevel"": ""low|medium|high|critical"",
    ""issues"": [
        {{
            ""type"": ""error|warning|info"",
            ""category"": ""null-reference|memory-leak|logic-error|race-condition|exception-handling|validation|boundary|type-conversion"",
            ""description"": ""Detailed description of the potential bug or issue"",
            ""lineNumber"": number,
            ""severity"": ""low|medium|high|critical"",
            ""fixSuggestion"": ""Specific recommendation to fix this issue"",
            ""codeExample"": ""Example of corrected code (if applicable)""
        }}
    ]
}}

Code to analyze:
```{languageStr}
{code}
```";
        }

        private string BuildTestGenerationPrompt(string code, ProgrammingLanguage language, Dictionary<string, object>? options)
        {
            var languageStr = language.ToString().ToLowerInvariant();
            var testFramework = options?.GetValueOrDefault("testFramework", "xunit") as string ?? "xunit";
            var includeMocks = options?.GetValueOrDefault("IncludeMocks", true) as bool? ?? true;
            var includeIntegrationTests = options?.GetValueOrDefault("IncludeIntegrationTests", false) as bool? ?? false;

            return $@"
Generate comprehensive test cases for the following {languageStr} code using {testFramework}.
{(includeMocks ? "Include mock objects and dependencies where appropriate." : "")}
{(includeIntegrationTests ? "Also provide integration test examples." : "")}

Requirements:
- Test all public methods and functions
- Include positive test cases (happy path)
- Include negative test cases (error conditions)
- Test edge cases and boundary conditions
- Follow AAA pattern (Arrange, Act, Assert) where applicable
- Use descriptive test method names
- Include proper setup and teardown if needed

Code to test:
```{languageStr}
{code}
```

Please provide complete, runnable test code with proper imports, setup, test methods, and assertions.";
        }

        private string BuildRefactoringPrompt(string code, ProgrammingLanguage language, Dictionary<string, object>? options)
        {
            var languageStr = language.ToString().ToLowerInvariant();

            return $@"
Refactor the following {languageStr} code to improve its structure, readability, and maintainability.
Focus on:
- Extracting methods/functions for better modularity
- Removing code duplication (DRY principle)
- Improving variable and method names
- Simplifying complex conditional logic
- Applying appropriate design patterns
- Improving error handling
- Enhancing code organization

Please provide the refactored code with explanations of changes made.

Original code:
```{languageStr}
{code}
```";
        }

        private string BuildPerformancePrompt(string code, ProgrammingLanguage language, Dictionary<string, object>? options)
        {
            var languageStr = language.ToString().ToLowerInvariant();

            return $@"
Analyze the following {languageStr} code for performance optimizations.
Look for:
- Algorithm efficiency improvements
- Memory usage optimization
- Database query optimization
- Loop optimization
- Caching opportunities
- Lazy loading possibilities
- Resource pooling opportunities
- Asynchronous operation improvements

Please provide specific performance improvement suggestions with optimized code examples.

Code to optimize:
```{languageStr}
{code}
```";
        }

        private string BuildSecurityPrompt(string code, ProgrammingLanguage language, Dictionary<string, object>? options)
        {
            var languageStr = language.ToString().ToLowerInvariant();

            return $@"
Perform a security analysis of the following {languageStr} code.
Check for:
- SQL injection vulnerabilities
- Cross-site scripting (XSS) vulnerabilities
- Authentication and authorization issues
- Input validation problems
- Sensitive data exposure
- Cryptographic weaknesses
- Path traversal vulnerabilities
- Denial of service vulnerabilities
- Insecure direct object references

Please respond in JSON format with detailed security findings and remediation suggestions.

Code to analyze:
```{languageStr}
{code}
```";
        }
    }
}
