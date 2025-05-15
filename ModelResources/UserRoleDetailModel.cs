using System.Text.Json.Serialization;

namespace IdentityManagementAPI.ModelResources;

public class UserRoleDetailModel
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }
    [JsonPropertyName("name")]
    public string Name { get; set; } = default!;
    [JsonPropertyName("composite")]
    public bool? Composite { get; set; }
    [JsonPropertyName("clientRole")]
    public bool? ClientRole { get; set; }
    [JsonPropertyName("containerId")]
    public string ContainerId { get; set; } = default!;
}