using Aiursoft.Canon;
using Aiursoft.Scanner.Abstractions;
using Newtonsoft.Json;

namespace Aiursoft.Tracer.Services;

public class IpGeolocationService(
    CacheService cacheService,
    IHttpClientFactory httpClientFactory,
    ILogger<IpGeolocationService> logger) : IScopedDependency
{
    public Task<(string CountryName, string CountryCode)?> GetLocationAsync(string ip)
    {
        return cacheService.RunWithCache($"ip-location-info-{ip}", () => GetLocationFromServiceProviderAsync(ip));
    }

    private async Task<(string CountryName, string CountryCode)?> GetLocationFromServiceProviderAsync(string ip)
    {
        if (string.IsNullOrWhiteSpace(ip) || ip == "::1" || ip == "127.0.0.1")
        {
            // mock data:
            return ("China", "CN");
            //return null;
        }

        try
        {
            var client = httpClientFactory.CreateClient();
            // api.country.is is free and supports HTTPS.
            var response = await client.GetAsync($"https://api.country.is/{ip}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<IpApiResponse>(content);

                if (!string.IsNullOrWhiteSpace(result?.Country))
                {
                    try
                    {
                        var region = new System.Globalization.RegionInfo(result.Country);
                        return (region.DisplayName, result.Country);
                    }
                    catch
                    {
                        // In case the country code is invalid or not found
                        return (result.Country, result.Country);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to get location for IP: {IP}", ip);
        }

        return null;
    }
}

public class IpApiResponse
{
    [JsonProperty("ip")]
    public string? Ip { get; set; }

    [JsonProperty("country")]
    public string? Country { get; set; }
}
