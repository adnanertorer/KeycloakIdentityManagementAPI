using IdentityManagementAPI.ModelResources;
using IdentityManagementAPI.Services.Abstracts;
using IdentityManagementAPI.Wrappers;

namespace IdentityManagementAPI.Endpoints;

public static class GroupEnpointMaps
{
    public static IEndpointRouteBuilder GroupEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.MapPost("/api/group/add", async (IKeycloackService service, GroupRepresentationModel request, CancellationToken cancellationToken) =>
        {
            var result = await service.AddGroup(request, cancellationToken);
            return result.ToIResult();
        }).RequireAuthorization();

        builder.MapPost("/api/group/get-groups", async (IKeycloackService service,
         GetGroupListByFilterRequest request, CancellationToken cancellationToken) =>
        {
            var result = await service.GetGroups(request, cancellationToken);
            return result.ToIResult();
        }).RequireAuthorization();

        builder.MapPost("/api/group/get-group-children", async (IKeycloackService service, 
        GetGroupChildrenListByFilterRequest request, CancellationToken cancellationToken) =>
        {
            var result = await service.GetGroupChildren(request.GroupId!, request, cancellationToken);
            return result.ToIResult();
        }).RequireAuthorization();

        builder.MapPost("/api/group/add-group-children", async (IKeycloackService service, 
        GroupRepresentationModel request, CancellationToken cancellationToken) =>
        {
            var result = await service.AddGroupChildren(request.GroupId!, request, cancellationToken);
            return result.ToIResult();
        }).RequireAuthorization();

        builder.MapGet("/api/group/{groupId}", async (IKeycloackService service,
        string groupId, CancellationToken cancellationToken) =>
        {
            var result = await service.GetGroup(groupId, cancellationToken);
            return result.ToIResult();
        }).RequireAuthorization();

        builder.MapGet("/api/group/group-management-permission/{groupId}", async (IKeycloackService service, 
        string groupId, CancellationToken cancellationToken) =>
        {
            var result = await service.GetGroupManagementPermission(groupId, cancellationToken);
            return result.ToIResult();
        }).RequireAuthorization();


        builder.MapPut("/api/group/update-group-management-permission", async (IKeycloackService service,
         ManagementPermissionReferenceModel request, CancellationToken cancellationToken) =>
        {
            var result = await service.UpdateGroupManagementPermission(request.GroupId, request, cancellationToken);
            return result.ToIResult();
        }).RequireAuthorization();

        builder.MapPost("/api/group/get-group-members", async (IKeycloackService service,
        GetGroupMembersByFilter request, CancellationToken cancellationToken) =>
        {
            var result = await service.GetGroupUsers(request.GroupId!, request, cancellationToken);
            return result.ToIResult();
        }).RequireAuthorization();

        builder.MapPut("/api/group/update-group", async (IKeycloackService service,
        GroupRepresentationModel request, CancellationToken cancellationToken) =>
       {
           var result = await service.UpdateGroup(request.GroupId!, request, cancellationToken);
           return result.ToIResult();
       }).RequireAuthorization();

        builder.MapPost("/api/group/add-user-to-group", async (IKeycloackService service,
        AddUserToGroupRequestModel request, CancellationToken cancellationToken) =>
       {
           var result = await service.AddUserToGroup(request.UserId, request.GroupId, cancellationToken);
           return result.ToIResult();
       }).RequireAuthorization();

        return builder;
    }
}