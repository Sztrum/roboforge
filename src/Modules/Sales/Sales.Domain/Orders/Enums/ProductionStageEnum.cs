namespace RoboForge.Modules.Sales.Domain.Orders.Enums;

/// <summary>Sales' copy of the workshop stages, so the customer can follow the order's progress.</summary>
public enum ProductionStageEnum
{
    Printing,
    Assembly,
    QaTests,
    Packing,
    Shipped,
}
