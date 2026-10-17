namespace RoboForge.Modules.Production.Domain.Jobs;

/// <summary>The ID of the Sales order a job builds the robot for; Production knows nothing else about the order.</summary>
public readonly record struct SalesOrderId(Guid Value);
