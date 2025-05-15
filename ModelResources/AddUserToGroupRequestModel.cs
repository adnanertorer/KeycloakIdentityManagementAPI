namespace IdentityManagementAPI.ModelResources;

public class AddUserToGroupRequestModel
{
    public required string UserId { get; set; }
    public required string GroupId { get; set; }
}