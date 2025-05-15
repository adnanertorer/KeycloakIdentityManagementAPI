using System.Text.Json.Serialization;

namespace IdentityManagementAPI.ModelResources;

public class ResetPasswordModel
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = "password";
    [JsonPropertyName("temporary")]
    public bool Temporary { get; set; }
    [JsonPropertyName("value")]
    public string Value { get; set; } = null!;
}