using RoboForge.Modules.Catalog.Domain.Parts;

namespace RoboForge.Modules.Catalog.Domain.UnitTests.Parts;

public class PartIdCreationTest
{
    [Fact]
    public void New_CalledTwice_ReturnsDifferentIds()
    {
        var firstPartId = PartId.New();
        var secondPartId = PartId.New();

        firstPartId.ShouldNotBe(secondPartId);
    }

    [Fact]
    public void Equality_WithSameGuid_IsTrue()
    {
        var sharedGuid = Guid.NewGuid();

        new PartId(sharedGuid).ShouldBe(new PartId(sharedGuid));
    }
}
