using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace IdentityManagementAPI.ModelResources;

public class GetGroupChildrenListByFilterRequest
{
    [JsonIgnore]
    public string? GroupId { get; set; }
    
    [JsonPropertyName("briefRepresentation")]
    public bool? BriefRepresentation { get; set; }
    [JsonPropertyName("exact")]
    public bool? Exact { get; set; }
    [JsonPropertyName("first")]
    public int? First { get; set; }
    [JsonPropertyName("max")]
    public int? Max { get; set; }
    [JsonPropertyName("search")]
    public string? Search { get; set; }
}