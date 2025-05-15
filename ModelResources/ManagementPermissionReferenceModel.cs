using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace IdentityManagementAPI.ModelResources;

public class ManagementPermissionReferenceModel
{
    [NotMapped]
    public string GroupId { get; set; } = null!;
    [JsonPropertyName("enabled")]
    public bool? Enabled { get; set; }
    [JsonPropertyName("resource")]
    public string? Resource { get; set; }
    [JsonPropertyName("scopePermissions")]
    public Dictionary<string, List<string>>? ScopePermissions { get; set; }
}