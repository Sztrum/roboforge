namespace RoboForge.Modules.Production.Domain.Jobs.Enums;

/// <summary>The workshop stages, in the only order they may run (R9).</summary>
public enum ProductionStageEnum
{
    Printing,
    Assembly,
    QaTests,
    Packing,
    Shipped,
}
