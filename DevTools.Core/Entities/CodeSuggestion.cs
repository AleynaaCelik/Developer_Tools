using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevTools.Core.Entities
{
    public class CodeSuggestion : BaseEntity
    {
        public Guid SessionId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public SuggestionCategory Category { get; set; }
        public string? CodeExample { get; set; }
        public int Priority { get; set; } // 1-5
        public bool IsApplied { get; set; } = false;

        // Navigation properties
        public virtual CodeAnalysisSession Session { get; set; } = null!;
    }
}
