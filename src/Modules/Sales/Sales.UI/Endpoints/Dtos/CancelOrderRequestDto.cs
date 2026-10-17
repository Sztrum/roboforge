namespace RoboForge.Modules.Sales.UI.Endpoints.Dtos;

/// <summary>The body of <c>POST /api/orders/{id}/cancel</c>.</summary>
public sealed record CancelOrderRequestDto(string Reason);
