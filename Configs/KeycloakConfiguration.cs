namespace IdentityManagementAPI.Configs;

public class KeycloakConfiguration
{
    public string HostName { get; set; } = null!;
    public string ClientId { get; set; } = null!;
    public string Realm { get; set; } = null!;
    public string ClientSecret { get; set; } = null!;
    public string ClientUUID { get; set; } = null!;
}