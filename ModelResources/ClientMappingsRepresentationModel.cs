using System.Text.Json.Serialization;

namespace IdentityManagementAPI.ModelResources;

public class ClientMappingsRepresentationModel
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }
    [JsonPropertyName("client")]
    public string? Client { get; set; }
    [JsonPropertyName("mappings")]
    public List<RoleRepresentationModel>? Mappings { get; set; }
}