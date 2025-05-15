using System.Text.Json.Serialization;

namespace IdentityManagementAPI.ModelResources;

public class ErrorResponseModel
{
    [JsonPropertyName("field")]
    public string Field { get; set; } = null!;
    [JsonPropertyName("errorMessage")]
    public string ErrorMessage { get; set; } = null!;
}