namespace IdentityManagementAPI.ModelResources;

public class CredentialModel
{
    public string type { get; set; } = null!;
    public string value { get; set; } = null!;
    public bool temporary { get; set; }
}