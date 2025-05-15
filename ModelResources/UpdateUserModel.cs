using System.Text.Json.Serialization;

namespace IdentityManagementAPI.ModelResources;

public class UpdateUserModel
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = default!;

    [JsonPropertyName("userInfo")]
    public required UpdateUserInfo UserInfo { get; set; }
}

public class UpdateUserInfo
{
    [JsonPropertyName("firstName")]
    public string FirstName { get; set; } = null!;

    [JsonPropertyName("lastName")]
    public string LastName { get; set; } = null!;

    [JsonPropertyName("email")]
    public string Email { get; set; } = null!;

    [JsonPropertyName("emailVerified")]
    public bool EmailVerified { get; set; }

    [JsonPropertyName("enabled")]
    public bool Enabled { get; set; }
}