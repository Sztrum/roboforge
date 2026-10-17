namespace RoboForge.Modules.Catalog.UI.Endpoints.Dtos;

/// <summary>The body of <c>PUT /api/parts/{id}/price</c>; the part ID comes from the route.</summary>
public sealed record ChangePartPriceRequestDto(decimal PriceAmount);
