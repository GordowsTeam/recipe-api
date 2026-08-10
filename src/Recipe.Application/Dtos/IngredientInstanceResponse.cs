using Recipe.Domain.Enums;

namespace Recipe.Application.Dtos;

public class IngredientInstanceResponse
{
    public required Guid Id { get; set; }
    public required Guid IngredientId { get; set; }
    public IngredientInstanceStatus Status { get; set; }
    public decimal Quantity { get; set; }
    public required string Unit { get; set; }
    public DateTime PurchasedAt { get; set; }
    public DateTime? FrozenAt { get; set; }
    public DateTime? DefrostStartedAt { get; set; }
    public DateTime? LastVerifiedAt { get; set; }
    public string? Notes { get; set; }
}
