using System.Text.Json.Serialization;

namespace IdentityManagementAPI.ModelResources;

public class RoleRepresentationModel
{
    [JsonIgnore]
    public string? GroupId { get; set; }
    [JsonPropertyName("id")]
    public string? Id { get; set; }
    [JsonPropertyName("name")]
    public string? Name { get; set; }
    [JsonPropertyName("description")]
    public string? Description { get; set; }
    [JsonPropertyName("scopeParamRequired")]
    public bool? ScopeParamRequired { get; set; }
    [JsonPropertyName("composite")]
    public bool? Composite { get; set; }
    [JsonPropertyName("clientRole")]
    public bool? ClientRole { get; set; }
    [JsonPropertyName("containerId")]
    public string? ContainerId { get; set; }
    [JsonPropertyName("attributes")]
    public Dictionary<string, List<string>>? Attributes { get; set; }
}