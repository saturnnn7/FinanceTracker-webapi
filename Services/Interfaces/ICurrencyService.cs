namespace FinanceTracker.Services.Interfaces;

public interface ICurrencyService
{
    /// <summary>
    /// Converts an amount from one currency to another.
    /// Exchange rates are cached for 60 minutes.
    /// </summary>
    Task<decimal> ConvertAsync(
        decimal amount,
        string fromCurrency,
        string toCurrency,
        CancellationToken ct = default);

    /// <summary>
    /// Returns all exchange rates relative to the base currency.
    /// </summary>
    Task<Dictionary<string, decimal>> GetRatesAsync(
        string baseCurrency,
        CancellationToken ct = default);
}