using DevTools.Application.DTOs.CodeAnalysis;
using DevTools.Application.Interfaces.External;
using DevTools.Core.Entities;
using DevTools.Core.Enums;
using DevTools.Core.Exceptions;
using DevTools.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevTools.Application.UseCases.CodeAnalysis
{
    public class AnalyzeCodeUseCase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOpenAIService _openAIService;

        public AnalyzeCodeUseCase(IUnitOfWork unitOfWork, IOpenAIService openAIService)
        {
            _unitOfWork = unitOfWork;
            _openAIService = openAIService;
        }

        public async Task<CodeAnalysisResponseDto> ExecuteAsync(Guid userId, CodeAnalysisRequestDto request)
        {
            // Check user limits
            var user = await _unitOfWork.Repository<User>().FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                throw new InvalidAnalysisRequestException("User not found");

            if (user.ApiUsageUsed >= user.ApiUsageLimit)
                throw new UserLimitExceededException();

            // Create analysis session
            var session = new CodeAnalysisSession
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                FileName = request.FileName,
                Language = request.Language,
                OriginalCode = request.Code,
                AnalysisType = request.AnalysisType,
                Status = AnalysisStatus.Processing,
                ProjectId = request.ProjectId
            };

            await _unitOfWork.CodeAnalysisSessions.AddAsync(session);

            try
            {
                // Call OpenAI service
                var analysisResult = await _openAIService.AnalyzeCodeAsync(
                    request.Code,
                    request.Language,
                    request.AnalysisType,
                    request.Options
                );

                // Estimate tokens and cost
                var tokens = await _openAIService.EstimateTokensAsync(request.Code + analysisResult);
                var cost = CalculateCost(tokens, request.AnalysisType);

                // Update session
                session.AnalysisResult = analysisResult;
                session.TokensUsed = tokens;
                session.Cost = cost;
                session.Status = AnalysisStatus.Completed;

                // Update user usage
                user.ApiUsageUsed += 1;
                user.UpdatedAt = DateTime.UtcNow;

                await _unitOfWork.SaveChangesAsync();

                // Parse and return result
                return new CodeAnalysisResponseDto
                {
                    SessionId = session.Id,
                    AnalysisType = request.AnalysisType,
                    Success = true,
                    Result = ParseAnalysisResult(analysisResult, request.AnalysisType),
                    TokensUsed = tokens,
                    Cost = cost,
                    Status = AnalysisStatus.Completed
                };
            }
            catch (Exception ex)
            {
                // Update session with error
                session.Status = AnalysisStatus.Failed;
                session.ErrorMessage = ex.Message;
                await _unitOfWork.SaveChangesAsync();

                throw new CodeAnalysisException($"Analysis failed: {ex.Message}", ex);
            }
        }

        private decimal CalculateCost(int tokens, AnalysisType analysisType)
        {
            // Different analysis types might have different costs
            var baseRate = analysisType switch
            {
                AnalysisType.Review => 0.03m,
                AnalysisType.Documentation => 0.02m,
                AnalysisType.BugDetection => 0.04m,
                AnalysisType.TestGeneration => 0.03m,
                _ => 0.03m
            };

            return (decimal)(tokens * (double)baseRate / 1000);
        }

        private CodeAnalysisResultDto ParseAnalysisResult(string result, AnalysisType analysisType)
        {
            // This would be a more sophisticated JSON parsing in real implementation
            return new CodeAnalysisResultDto
            {
                Summary = "Analysis completed successfully",
                Issues = new List<CodeIssueDto>(),
                Suggestions = new List<CodeSuggestionDto>(),
                ImprovedCode = analysisType == AnalysisType.Review ? result : null,
                GeneratedDocumentation = analysisType == AnalysisType.Documentation ? result : null,
                GeneratedTests = analysisType == AnalysisType.TestGeneration ? result : null,
                Statistics = new AnalysisStatisticsDto()
            };
        }
    }
}
