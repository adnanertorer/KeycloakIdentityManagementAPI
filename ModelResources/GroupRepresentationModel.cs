using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace IdentityManagementAPI.ModelResources;

public class GroupRepresentationModel
{
    [JsonIgnore]
    public string? GroupId { get; set; }
    [JsonPropertyName("id")]
    public string? Id { get; set; }
    [JsonPropertyName("name")]
    public string? Name { get; set; }
    [JsonPropertyName("path")]
    public string? Path { get; set; }
    [JsonPropertyName("parentId")]
    public string? ParentId { get; set; }
    [JsonPropertyName("subGroupCount")]
    public long? SubGroupCount { get; set; }
    [JsonPropertyName("subGroups")]
    public List<GroupRepresentationModel>? SubGroups { get; set; }
    [JsonPropertyName("attributes")]
    public Dictionary<string, List<string>>? Attributes { get; set; }
    [JsonPropertyName("realmRoles")]
    public List<string>? RealmRoles { get; set; }
    [JsonPropertyName("clientRoles")]
    public Dictionary<string, List<string>>? ClientRoles { get; set; }
    [JsonPropertyName("access")]
    public Dictionary<string, bool>? Access { get; set; }
}
