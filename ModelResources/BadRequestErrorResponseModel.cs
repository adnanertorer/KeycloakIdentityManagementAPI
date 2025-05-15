using System.Text.Json.Serialization;

namespace IdentityManagementAPI.ModelResources;

public class BadRequestErrorResponseModel
{
    [JsonPropertyName("error")]
    public string Error { get; set; } = null!;
    [JsonPropertyName("error_description")]
    public string ErrorDescription { get; set; } = null!;
}