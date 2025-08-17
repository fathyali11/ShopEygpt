using Microsoft.ML.Data;

namespace Web.Entites.ViewModels.RecommendationSystem;

public class ProductRatingInput
{
    [LoadColumn(0)]
    public string UserId { get; set; } = string.Empty;

    [LoadColumn(1)]
    public int ProductId { get; set; }

    [LoadColumn(2)]
    public float Label { get; set; }
}

public class ProductPrediction
{
    public float Score { get; set; }
}