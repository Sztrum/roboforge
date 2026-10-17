using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RoboForge.Modules.Catalog.Application.Parts.Commands;
using RoboForge.Modules.Catalog.Application.Parts.Resources;
using RoboForge.Modules.Catalog.Domain.Parts.Enums;
using RoboForge.Modules.Catalog.Infrastructure.Database;
using RoboForge.Modules.Catalog.IntegrationEvents;
using RoboForge.Shared.Infrastructure.Outbox;

namespace RoboForge.Web.FunctionalTests.Shared;

public class OutboxPublishingTest(ApiWithPostgresAssemblyFixture fixture)
{
    [Fact]
    public async Task StorePart_WritesPartCreatedIntegrationEventToOutbox_ThatIsPublishedWithoutError()
    {
        using var client = fixture.CreateClient();
        var sensor = new StorePartCommand($"Czujnik outbox {Guid.NewGuid():N}", PartCategoryEnum.Sensor, 10m, 1, 5, null, null, null);
        var response = await client.PostAsJsonAsync("/api/parts", sensor, ApiJsonSerializerOptions.Web, TestContext.Current.CancellationToken);
        var storedSensor = (await response.Content.ReadFromJsonAsync<PartResource>(ApiJsonSerializerOptions.Web, TestContext.Current.CancellationToken))!;

        var publishedMessage = await EventuallyConsistentAssertion.WaitUntilAsync(
            () => FirstOutboxMessageAboutPartAsync(storedSensor.Id),
            message => message?.ProcessedOnUtc is not null,
            "the PartCreatedIntegrationEvent to be published");

        publishedMessage!.Type.ShouldStartWith(typeof(PartCreatedIntegrationEvent).FullName!);
        publishedMessage.Error.ShouldBeNull();
    }

    private async Task<OutboxMessage?> FirstOutboxMessageAboutPartAsync(Guid partId)
    {
        await using var scope = fixture.CreateServiceScope();
        var catalogDbContext = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();

        var partCreatedMessages = await catalogDbContext.Set<OutboxMessage>().AsNoTracking()
            .Where(message => message.Type.StartsWith(typeof(PartCreatedIntegrationEvent).FullName!))
            .ToListAsync(TestContext.Current.CancellationToken);

        return partCreatedMessages.FirstOrDefault(message => message.Content.Contains(partId.ToString(), StringComparison.Ordinal));
    }
}
