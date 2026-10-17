using RoboForge.Shared.Domain.ValueObjects;

namespace RoboForge.Modules.Sales.Domain.Configurations.ValueObjects;

/// <summary>How the price of a robot is made up (R5).</summary>
public sealed record RobotPrice(Money PartsPrice, Money AssemblyPrice, Money Discount, Money TotalPrice);
