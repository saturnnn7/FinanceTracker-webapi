// Services/CurrencyService.cs
using System.Text.Json;
using FinanceTracker.Models.External;
using FinanceTracker.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace FinanceTracker.Services;

public class CurrencyService : ICurrencyService
{
    private readonly HttpClient   _httpClient;
    private readonly IMemoryCache _cache;
    private readonly IConfiguration _config;
    private readonly ILogger<CurrencyService> _logger;

    // Cache key prefix — to avoid conflicts with other caches
    private const string CacheKeyPrefix = "exchange_rates_";

    public CurrencyService(
        HttpClient httpClient,
        IMemoryCache cache,
        IConfiguration config,
        ILogger<CurrencyService> logger)
    {
        _httpClient = httpClient;
        _cache      = cache;
        _config     = config;
        _logger     = logger;
    }

    public async Task<decimal> ConvertAsync(
        decimal amount,
        string fromCurrency,
        string toCurrency,
        CancellationToken ct = default)
    {
        // Same currency—no conversion needed
        if (fromCurrency.Equals(toCurrency, StringComparison.OrdinalIgnoreCase))
            return amount;

        var rates = await GetRatesAsync(fromCurrency, ct);

        if (!rates.TryGetValue(toCurrency.ToUpper(), out var rate))
        {
            _logger.LogWarning(
                "Exchange rate not found for {From} -> {To}. Returning original amount.",
                fromCurrency, toCurrency);
            return amount;
        }

        return Math.Round(amount * rate, 2);
    }

    public async Task<Dictionary<string, decimal>> GetRatesAsync(
        string baseCurrency,
        CancellationToken ct = default)
    {
        var cacheKey = CacheKeyPrefix + baseCurrency.ToUpper();

        // Check the cache—if there is recent data, we don't make an API call
        if (_cache.TryGetValue(cacheKey, out Dictionary<string, decimal>? cached)
            && cached is not null)
        {
            _logger.LogDebug("Exchange rates for {Currency} served from cache.", baseCurrency);
            return cached;
        }

        // Let's move on to the external API
        return await FetchAndCacheRatesAsync(baseCurrency, cacheKey, ct);
    }

    // -------------------------------------------------------

    private async Task<Dictionary<string, decimal>> FetchAndCacheRatesAsync(
        string baseCurrency,
        string cacheKey,
        CancellationToken ct)
    {
        var baseUrl = _config["CurrencyApi:BaseUrl"]
            ?? "https://api.exchangerate-api.com/v4/latest/";

        var cacheDuration = int.Parse(
            _config["CurrencyApi:CacheDurationMinutes"] ?? "60");

        try
        {
            var url      = $"{baseUrl}{baseCurrency.ToUpper()}";
            var response = await _httpClient.GetAsync(url, ct);

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync(ct);
            var data = JsonSerializer.Deserialize<ExchangeRateResponse>(json);

            if (data?.Rates is null || !data.Rates.Any())
                return GetFallbackRates(baseCurrency);

            // We cache for N minutes
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(cacheDuration));

            _cache.Set(cacheKey, data.Rates, cacheOptions);

            _logger.LogInformation(
                "Exchange rates for {Currency} fetched and cached for {Minutes} minutes.",
                baseCurrency, cacheDuration);

            return data.Rates;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to fetch exchange rates for {Currency}. Using fallback rates.",
                baseCurrency);

            // If the API is unavailable, we return the fallback exchange rates
            return GetFallbackRates(baseCurrency);
        }
    }

    /// <summary>
    /// Fallback rates in case the API is unavailable.
    /// Approximate values are better than the app crashing.
    /// </summary>
    private static Dictionary<string, decimal> GetFallbackRates(string baseCurrency)
        => baseCurrency.ToUpper() switch
        {
            "PLN" => new() { ["USD"] = 0.25m, ["EUR"] = 0.23m, ["GBP"] = 0.20m, ["RUB"] = 23m,  ["PLN"] = 1m },
            "USD" => new() { ["PLN"] = 4.0m,  ["EUR"] = 0.92m, ["GBP"] = 0.79m, ["RUB"] = 92m,  ["USD"] = 1m },
            "EUR" => new() { ["PLN"] = 4.3m,  ["USD"] = 1.09m, ["GBP"] = 0.86m, ["RUB"] = 100m, ["EUR"] = 1m },
            _     => new() { [baseCurrency] = 1m }
        };
}