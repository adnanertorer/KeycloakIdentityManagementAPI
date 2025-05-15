using System.Text.Json.Serialization;

namespace IdentityManagementAPI.ModelResources;

public class RoleModel
{
    [JsonPropertyName("id")] public Guid Id { get; set; }

    [JsonPropertyName("name")] public string Name { get; set; } = null!;
}