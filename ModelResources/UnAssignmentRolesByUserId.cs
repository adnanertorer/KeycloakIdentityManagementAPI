namespace IdentityManagementAPI.ModelResources;

public class UnAssignmentRolesByUserId
{
    public Guid Id { get; set; }
    public List<RoleModel>? Roles { get; set; } 
}