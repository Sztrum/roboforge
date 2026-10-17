namespace RoboForge.Modules.Sales.Domain.Orders.Enums;

public enum OrderStatusEnum
{
    /// <summary>Placed from a valid configuration; waits for the workshop to confirm it.</summary>
    Placed,

    /// <summary>Confirmed; the workshop has started a production job for it.</summary>
    Confirmed,

    /// <summary>At least one production stage is completed.</summary>
    InProduction,

    Shipped,

    Cancelled,
}
