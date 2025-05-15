using IdentityManagementAPI.ModelResources;
using IdentityManagementAPI.Services.Abstracts;
using IdentityManagementAPI.Wrappers;

namespace IdentityManagementAPI.Endpoints;

public static class UsersEndpointMaps
{
    public static IEndpointRouteBuilder UsersEndpointBuilder(this IEndpointRouteBuilder builder)
    {
        builder.MapGet("/api/user",
            async (IKeycloackService service, CancellationToken cancellationToken) =>
            {
                var result = await service.GetUsers(cancellationToken);
                return result.ToIResult();
            }).RequireAuthorization();
        
        builder.MapGet("/api/user/get-by-email/{email}",
            async (IKeycloackService service, string email,  CancellationToken cancellationToken) =>
            {
                var result = await service.GetUserByEmail(email, cancellationToken);
                return result.ToIResult();
            }).RequireAuthorization();
        
        builder.MapGet("/api/user/get-by-username/{username}",
            async (IKeycloackService service, string username,  CancellationToken cancellationToken) =>
            {
                var result = await service.GetUserByUsername(username, cancellationToken);
                return result.ToIResult();
            }).RequireAuthorization();
        
        builder.MapGet("/api/user/get-by-id/{id}",
            async (IKeycloackService service, Guid id,  CancellationToken cancellationToken) =>
            {
                var result = await service.GetUserById(id, cancellationToken);
                return result.ToIResult();
            }).RequireAuthorization();
        
        builder.MapPut("/api/user", async (IKeycloackService service, UpdateUserModel request,
            CancellationToken cancellationToken) =>
        {
            var result = await service.UpdateUser(request, cancellationToken);
            return result.ToIResult();
        }).RequireAuthorization();
        
        builder.MapDelete("/api/user/{id}", async (IKeycloackService service, Guid id,
            CancellationToken cancellationToken) =>
        {
            var result = await service.DeleteUserById(id, cancellationToken);
            return result.ToIResult();
        }).RequireAuthorization();
        
        builder.MapGet("/api/user/role-mappings/{id}",
            async (IKeycloackService service, string id,  CancellationToken cancellationToken) =>
            {
                var result = await service.GetUserRoleMappings(id, cancellationToken);
                return result.ToIResult();
            }).RequireAuthorization();
        
        builder.MapGet("/api/user/role-mappings-realm-available/{id}",
            async (IKeycloackService service, string id,  CancellationToken cancellationToken) =>
            {
                var result = await service.GetUserRoleMappingsRealmAvaible(id, cancellationToken);
                return result.ToIResult();
            }).RequireAuthorization();
        
        return builder;
    }
}