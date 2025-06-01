using DevTools.Application.DTOs.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevTools.Application.Interfaces.Services
{
    public interface ICodeAnalysisService
    {
        Task<CodeAnalysisResponseDto> AnalyzeCodeAsync(Guid userId, CodeAnalysisRequestDto request);
        Task<CodeAnalysisResponseDto> ReviewCodeAsync(Guid userId, CodeReviewRequestDto request);
        Task<CodeAnalysisResponseDto> GenerateDocumentationAsync(Guid userId, DocumentationRequestDto request);
        Task<CodeAnalysisResponseDto> DetectBugsAsync(Guid userId, BugDetectionRequestDto request);
        Task<CodeAnalysisResponseDto> GenerateTestsAsync(Guid userId, TestGenerationRequestDto request);
        Task<bool> CheckUserLimitAsync(Guid userId);
        Task<IEnumerable<CodeAnalysisResponseDto>> GetUserAnalysisHistoryAsync(Guid userId, int? limit = null);
        Task<CodeAnalysisResponseDto?> GetAnalysisSessionAsync(Guid sessionId, Guid userId);
        Task<bool> DeleteAnalysisSessionAsync(Guid sessionId, Guid userId);
    }
}
