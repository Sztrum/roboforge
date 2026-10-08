namespace RoboForge.Shared.Domain.UnitTests;

public class AggregateRootDomainEventsTest
{
    [Fact]
    public void BusinessMethod_WhenCalled_CollectsDomainEvent()
    {
        var aggregate = new TestAggregate(Guid.NewGuid());

        aggregate.DoSomething();

        aggregate.DomainEvents.ShouldHaveSingleItem().ShouldBeOfType<SomethingHappened>();
    }

    [Fact]
    public void ClearDomainEvents_WhenEventsCollected_RemovesAll()
    {
        var aggregate = new TestAggregate(Guid.NewGuid());
        aggregate.DoSomething();

        aggregate.ClearDomainEvents();

        aggregate.DomainEvents.ShouldBeEmpty();
    }

    private sealed record SomethingHappened : IDomainEvent;

    private sealed class TestAggregate(Guid id) : AggregateRoot<Guid>(id)
    {
        public void DoSomething() => AddDomainEvent(new SomethingHappened());
    }
}
