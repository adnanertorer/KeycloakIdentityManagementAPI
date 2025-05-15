using System.Text.Json.Serialization;

namespace IdentityManagementAPI.ModelResources;

public class GetGroupListByFilterRequest
{
    [JsonPropertyName("briefRepresentation")]
    public bool? BriefRepresentation { get; set; }
    [JsonPropertyName("exact")]
    public bool? Exact { get; set; }
    [JsonPropertyName("first")]
    public int? First { get; set; }
    [JsonPropertyName("max")]
    public int? Max { get; set; }
    [JsonPropertyName("populateHierarchy")]
    public bool? PopulateHierarchy { get; set; }
    [JsonPropertyName("search")]
    public string? Search { get; set; }
    [JsonPropertyName("q")]
    public string? Query { get; set; }
}