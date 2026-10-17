using System.Reflection;

namespace RoboForge.Shared.UI.Endpoints;

public static class MapEndpointGroupsWebApplicationExtensions
{
    /// <summary>
    /// Discovers all <see cref="IEndpointGroup"/> implementations in <paramref name="assembly"/>
    /// and registers each as a route group with a matching OpenAPI tag. The route prefix defaults
    /// to <c>/api/{ClassName}</c> but can be overridden via <see cref="IEndpointGroup.RoutePrefix"/>.
    /// The tag drops the naming convention's <c>Api</c> prefix and <c>EndpointGroup</c> suffix
    /// (<c>ApiPartEndpointGroup</c> → <c>Part</c>).
    /// </summary>
    public static WebApplication MapEndpointGroups(this WebApplication app, Assembly assembly)
    {
        var endpointGroupTypes = assembly.GetExportedTypes()
            .Where(t => t is { IsAbstract: false, IsInterface: false }
                     && t.IsAssignableTo(typeof(IEndpointGroup)));

        foreach (var type in endpointGroupTypes)
        {
            var groupName = type.Name;
            var routePrefix = type.GetProperty(nameof(IEndpointGroup.RoutePrefix))
                ?.GetValue(null) as string ?? $"/api/{groupName}";
            var group = app.MapGroup(routePrefix).WithTags(TagFromGroupName(groupName));
            type.GetMethod(nameof(IEndpointGroup.Map))!.Invoke(null, [group]);
        }

        return app;
    }

    private static string TagFromGroupName(string groupName)
    {
        const string ChannelPrefix = "Api";
        const string RoleSuffix = "EndpointGroup";

        var tag = groupName.EndsWith(RoleSuffix, StringComparison.Ordinal) ? groupName[..^RoleSuffix.Length] : groupName;

        return tag.StartsWith(ChannelPrefix, StringComparison.Ordinal) && tag.Length > ChannelPrefix.Length
            ? tag[ChannelPrefix.Length..]
            : tag;
    }
}
