using System.Text.Json.Serialization;

namespace IdentityManagementAPI.ModelResources;

public class UserRepresentationModel
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }
    [JsonPropertyName("usernama")]
    public string? Username { get; set; }
    [JsonPropertyName("firstName")]
    public string? FirstName { get; set; }
    [JsonPropertyName("lastName")]
    public string? LastName { get; set; }
    [JsonPropertyName("email")]
    public string? Email { get; set; }
    [JsonPropertyName("emailVerified")]
    public bool? EmailVerified { get; set; }
    [JsonPropertyName("attributes")]
    public Dictionary<string, List<string>>? Attributes { get; set; }
    [JsonPropertyName("self")]
    public string? Self { get; set; }
    [JsonPropertyName("origin")]
    public string? Origin { get; set; }
    [JsonPropertyName("createdTimestamp")]
    public long? CreatedTimestamp { get; set; }
    [JsonPropertyName("enabled")]
    public bool? Enabled { get; set; }
    [JsonPropertyName("totp")]
    public bool? TOTP { get; set; }
    [JsonPropertyName("federationLink")]
    public string? FederationLink { get; set; }
    [JsonPropertyName("serviceAccountClientId")]
    public string? ServiceAccountClientId { get; set; }
    [JsonPropertyName("realmRoles")]
    public List<string>? RealmRoles { get; set; }
    [JsonPropertyName("clientRoles")]
    public Dictionary<string, List<string>>? ClientRoles { get; set; }
    [JsonPropertyName("groups")]
    public List<string>? Groups { get; set; }
    [JsonPropertyName("access")]
    public bool? Access { get; set; }
}