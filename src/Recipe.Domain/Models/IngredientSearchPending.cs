using Recipe.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Recipe.Core.Models
{
    public class IngredientSearchPending
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public required List<string> Ingredients { get; set; } = new();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public IngredientSearchStatus Status { get; set; } = IngredientSearchStatus.Unprocessed;
        public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;
        public string? Error { get; set; }
        public int RetryCount { get; set; } = 0;
    }
}
