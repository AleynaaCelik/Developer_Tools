using DevTools.Application.DTOs.CodeAnalysis;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevTools.Application.Validators
{
    public class CodeAnalysisRequestValidator : AbstractValidator<CodeAnalysisRequestDto>
    {
        public CodeAnalysisRequestValidator()
        {
            RuleFor(x => x.Code)
                .NotEmpty()
                .WithMessage("Code is required")
                .MaximumLength(100000)
                .WithMessage("Code cannot exceed 100,000 characters");

            RuleFor(x => x.Language)
                .IsInEnum()
                .WithMessage("Invalid programming language");

            RuleFor(x => x.FileName)
                .NotEmpty()
                .WithMessage("File name is required")
                .MaximumLength(255)
                .WithMessage("File name cannot exceed 255 characters");

            RuleFor(x => x.AnalysisType)
                .IsInEnum()
                .WithMessage("Invalid analysis type");
        }
    }

    public class CodeReviewRequestValidator : AbstractValidator<CodeReviewRequestDto>
    {
        public CodeReviewRequestValidator()
        {
            Include(new CodeAnalysisRequestValidator());
        }
    }
}
