namespace IdentityManagementAPI.ModelResources;

public sealed record RegisterModel(string Username, string FirstName,
    string LastName, string Email, string Password, bool Enabled, bool EmailVerified,
    bool Temporary, long? CompanyId);