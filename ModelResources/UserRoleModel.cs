namespace IdentityManagementAPI.ModelResources;

public class UserRoleModel
{
    public Guid Id { get; set; }
    public List<RoleModel> Roles { get; set; } = default!;
}