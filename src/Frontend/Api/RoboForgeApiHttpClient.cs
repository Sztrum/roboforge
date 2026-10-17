using System.Net.Http.Json;
using System.Text.Json;
using RoboForge.Frontend.Api.Dtos;

namespace RoboForge.Frontend.Api;

/// <summary>Calls the RoboForge API. Rejected requests become <see cref="ApiRequestRejectedException"/>s.</summary>
public sealed class RoboForgeApiHttpClient(HttpClient httpClient)
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public Task<PartListDto> IndexPartsAsync(CancellationToken cancellationToken) =>
        GetAsync<PartListDto>("/api/parts", cancellationToken);

    public Task<ConfigurationDto> StoreConfigurationAsync(CancellationToken cancellationToken) =>
        SendAsync<ConfigurationDto>(HttpMethod.Post, "/api/configurations", body: null, cancellationToken);

    public Task<ConfigurationDto> ShowConfigurationAsync(Guid configurationId, CancellationToken cancellationToken) =>
        GetAsync<ConfigurationDto>($"/api/configurations/{configurationId}", cancellationToken);

    public Task<ConfigurationDto> AddPartAsync(Guid configurationId, Guid partId, CancellationToken cancellationToken) =>
        SendAsync<ConfigurationDto>(HttpMethod.Post, $"/api/configurations/{configurationId}/parts", new { partId }, cancellationToken);

    public Task<ConfigurationDto> RemovePartAsync(Guid configurationId, Guid partId, CancellationToken cancellationToken) =>
        SendAsync<ConfigurationDto>(HttpMethod.Delete, $"/api/configurations/{configurationId}/parts/{partId}", body: null, cancellationToken);

    public Task<QuoteDto> ShowQuoteAsync(Guid configurationId, CancellationToken cancellationToken) =>
        GetAsync<QuoteDto>($"/api/configurations/{configurationId}/quote", cancellationToken);

    public Task<OrderDto> PlaceOrderAsync(Guid configurationId, CancellationToken cancellationToken) =>
        SendAsync<OrderDto>(HttpMethod.Post, "/api/orders", new { configurationId }, cancellationToken);

    public Task<OrderListDto> IndexOrdersAsync(CancellationToken cancellationToken) =>
        GetAsync<OrderListDto>("/api/orders", cancellationToken);

    public Task<OrderDto> ShowOrderAsync(Guid orderId, CancellationToken cancellationToken) =>
        GetAsync<OrderDto>($"/api/orders/{orderId}", cancellationToken);

    public Task<OrderDto> ConfirmOrderAsync(Guid orderId, CancellationToken cancellationToken) =>
        SendAsync<OrderDto>(HttpMethod.Post, $"/api/orders/{orderId}/confirm", body: null, cancellationToken);

    public Task<OrderDto> CancelOrderAsync(Guid orderId, string reason, CancellationToken cancellationToken) =>
        SendAsync<OrderDto>(HttpMethod.Post, $"/api/orders/{orderId}/cancel", new { reason }, cancellationToken);

    public Task<ProductionJobListDto> IndexProductionJobsAsync(CancellationToken cancellationToken) =>
        GetAsync<ProductionJobListDto>("/api/production/jobs", cancellationToken);

    public Task<ProductionJobDto> CompleteStageAsync(Guid jobId, string stage, string completedBy, CancellationToken cancellationToken) =>
        SendAsync<ProductionJobDto>(HttpMethod.Post, $"/api/production/jobs/{jobId}/complete-stage", new { stage, completedBy }, cancellationToken);

    public Task<ProductionJobDto> FailQaAsync(Guid jobId, string reason, CancellationToken cancellationToken) =>
        SendAsync<ProductionJobDto>(HttpMethod.Post, $"/api/production/jobs/{jobId}/fail-qa", new { reason }, cancellationToken);

    public Task<ProductionJobDto> ReleaseFromHoldAsync(Guid jobId, CancellationToken cancellationToken) =>
        SendAsync<ProductionJobDto>(HttpMethod.Post, $"/api/production/jobs/{jobId}/release-hold", body: null, cancellationToken);

    public Task<ProductionJobDto> ShipAsync(Guid jobId, string trackingNumber, CancellationToken cancellationToken) =>
        SendAsync<ProductionJobDto>(HttpMethod.Post, $"/api/production/jobs/{jobId}/ship", new { trackingNumber }, cancellationToken);

    private Task<TResponse> GetAsync<TResponse>(string path, CancellationToken cancellationToken) =>
        SendAsync<TResponse>(HttpMethod.Get, path, body: null, cancellationToken);

    private async Task<TResponse> SendAsync<TResponse>(HttpMethod method, string path, object? body, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(method, path);

        if (body is not null)
        {
            request.Content = JsonContent.Create(body, options: JsonOptions);
        }

        using var response = await httpClient.SendAsync(request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw await ReadRejectionAsync(response, cancellationToken);
        }

        return (await response.Content.ReadFromJsonAsync<TResponse>(JsonOptions, cancellationToken))!;
    }

    private static async Task<ApiRequestRejectedException> ReadRejectionAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        try
        {
            using var problemDetails = JsonDocument.Parse(content);
            var root = problemDetails.RootElement;
            var message = ReadStringProperty(root, "detail") ?? ReadStringProperty(root, "title");
            string[] rules = root.TryGetProperty("rules", out var ruleIds)
                ? [.. ruleIds.EnumerateArray().Select(rule => rule.GetString()!)]
                : [];

            return new ApiRequestRejectedException(message ?? $"Błąd {(int)response.StatusCode}", rules);
        }
        catch (JsonException)
        {
            return new ApiRequestRejectedException($"Błąd {(int)response.StatusCode}", []);
        }
    }

    private static string? ReadStringProperty(JsonElement element, string propertyName) =>
        element.TryGetProperty(propertyName, out var property) ? property.GetString() : null;
}
