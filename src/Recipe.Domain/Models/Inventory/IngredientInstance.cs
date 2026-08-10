using Recipe.Domain.Enums;

namespace Recipe.Domain.Models.Inventory;

public class IngredientInstance
{
    public Guid Id { get; set; }

    /// <summary>Reference to the Ingredient (type) this instance is tracking, e.g. pollo, jícama, aguacate.</summary>
    public Guid IngredientId { get; set; }

    public string UserId { get; set; } = string.Empty;
    public IngredientInstanceStatus Status { get; set; }
    public decimal Quantity { get; set; }
    public string Unit { get; set; } = string.Empty;

    /// <summary>Base for calculating elapsed days.</summary>
    public DateTime PurchasedAt { get; set; }

    public DateTime? FrozenAt { get; set; }

    /// <summary>Base for calculating defrost ETA.</summary>
    public DateTime? DefrostStartedAt { get; set; }

    /// <summary>Base for triggering a verification prompt.</summary>
    public DateTime? LastVerifiedAt { get; set; }

    /// <summary>e.g. "paquete abierto", "ya cortado".</summary>
    public string? Notes { get; set; }
}
