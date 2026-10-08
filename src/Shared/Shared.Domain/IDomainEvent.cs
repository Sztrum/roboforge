namespace RoboForge.Shared.Domain;

/// <summary>
/// Marks a fact that happened in the domain. Implementations are past-tense records,
/// e.g. <c>OrderConfirmed</c>.
/// </summary>
public interface IDomainEvent;
