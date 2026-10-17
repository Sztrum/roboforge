using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RoboForge.Modules.Production.Domain.Jobs;
using RoboForge.Modules.Production.Domain.Jobs.ValueObjects;
using RoboForge.Shared.Infrastructure.Persistence;

namespace RoboForge.Modules.Production.Infrastructure.Database.Configurations;

internal sealed class ProductionJobEntityTypeConfiguration : IEntityTypeConfiguration<ProductionJobAggregate>
{
    public void Configure(EntityTypeBuilder<ProductionJobAggregate> builder)
    {
        builder.ToTable("ProductionJobs");

        builder.HasKey(job => job.Id);
        builder.Property(job => job.Id)
            .HasConversion(jobId => jobId.Value, value => new ProductionJobId(value))
            .ValueGeneratedNever();

        builder.Property(job => job.OrderId).HasConversion(orderId => orderId.Value, value => new SalesOrderId(value));
        builder.HasIndex(job => job.OrderId).IsUnique();

        builder.Property(job => job.CurrentStage).HasConversion<string>().HasMaxLength(20);
        builder.HasIndex(job => job.CurrentStage);
        builder.Property(job => job.Status).HasConversion<string>().HasMaxLength(20);
        builder.Property(job => job.TrackingNumber).HasMaxLength(100);

        builder.Ignore(job => job.Parts);
        builder.Property<List<ProductionPart>>("_parts").HasColumnName("Parts").StoreListAsJson();

        builder.Ignore(job => job.CompletedStages);
        builder.Property<List<StageCompletion>>("_completedStages").HasColumnName("CompletedStages").StoreListAsJson();

        builder.Ignore(job => job.QaFailures);
        builder.Property<List<QaFailure>>("_qaFailures").HasColumnName("QaFailures").StoreListAsJson();

        builder.Ignore(job => job.DomainEvents);
    }
}
