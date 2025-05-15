namespace IdentityManagementAPI.ModelResources;

public class RegisterRequestModel
{
    public string username { get; set; } = default!;
    public string firstName { get; set; } = default!;
    public string lastName { get; set; } = default!;
    public string email { get; set; } = default!;
    public bool enabled { get; set; }
    public bool emailVerified { get; set; }
    public long? Company_Id { get; set; }

    public List<CredentialModel> credentials { get; set; } = [];
}