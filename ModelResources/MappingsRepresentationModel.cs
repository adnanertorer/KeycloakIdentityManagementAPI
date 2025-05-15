using System.Text.Json.Serialization;

namespace IdentityManagementAPI.ModelResources;


public class MappingsRepresentationModel
{
    [JsonPropertyName("realmMappings")]
    public List<RoleRepresentationModel>? RealmMappings { get; set; }
    [JsonPropertyName("clientMappings")]
    public Dictionary<string, List<ClientMappingsRepresentationModel>>? ClientMappings { get; set; }
}