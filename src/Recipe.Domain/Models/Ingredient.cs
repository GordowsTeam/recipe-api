﻿using Recipe.Core.Enums;

namespace Recipe.Core.Models;

public class Ingredient
{
    public string? Text { get; set; }//TODO: Change for Name
    public decimal Quantity { get; set; }
    public string? Measure { get; set; }
    public decimal Weight { get; set; }
    public string? Food { get; set; }
    public FoodCategory FoodCategory { get; set; }
    public string? FoodCategoryId { get; set; }
    public string? Image { get; set; }
}
