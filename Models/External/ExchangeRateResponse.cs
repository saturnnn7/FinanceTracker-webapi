using System.Text.Json.Serialization;

namespace FinanceTracker.Models.External;

/// <summary>
/// Response template from exchangerate-api.com.
/// </summary>
public class ExchangeRateResponse
{
    [JsonPropertyName("base")]
    public string Base { get; set; } = string.Empty;

    [JsonPropertyName("rates")]
    public Dictionary<string, decimal> Rates { get; set; } = new();

    [JsonPropertyName("date")]
    public string Date { get; set; } = string.Empty;
}