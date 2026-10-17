using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace RoboForge.Shared.Infrastructure.Persistence;

/// <summary>
/// Stores a value object, or a list of them, as one <c>jsonb</c> column. Used for data that is always read
/// and written together with its aggregate, such as the part snapshots of a configuration.
/// </summary>
public static class StoreAsJsonPropertyBuilderExtensions
{
    private static readonly JsonSerializerOptions JsonColumnSerializerOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() },
    };

    public static PropertyBuilder<T> StoreAsJson<T>(this PropertyBuilder<T> builder)
        where T : class =>
        builder
            .HasColumnType("jsonb")
            .HasConversion(
                value => JsonSerializer.Serialize(value, JsonColumnSerializerOptions),
                json => JsonSerializer.Deserialize<T>(json, JsonColumnSerializerOptions)!);

    /// <summary>
    /// Like <see cref="StoreAsJson{T}"/> for a list the aggregate changes in place; the comparer copies the
    /// list so EF Core notices added, removed and replaced items.
    /// </summary>
    public static PropertyBuilder<List<T>> StoreListAsJson<T>(this PropertyBuilder<List<T>> builder) =>
        builder
            .HasColumnType("jsonb")
            .HasConversion(
                list => JsonSerializer.Serialize(list, JsonColumnSerializerOptions),
                json => JsonSerializer.Deserialize<List<T>>(json, JsonColumnSerializerOptions)!,
                new ValueComparer<List<T>>(
                    (left, right) => left!.SequenceEqual(right!),
                    list => list.Aggregate(0, (hash, item) => HashCode.Combine(hash, item)),
                    list => list.ToList()));
}
