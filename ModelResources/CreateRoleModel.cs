using System.Text.Json.Serialization;

namespace IdentityManagementAPI.ModelResources;

public class CreateRoleModel
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;
    [JsonPropertyName("description")]
    public string Description { get; set; } = null!;
}