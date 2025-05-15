using System.Net;
using System.Text;
using System.Text.Json;
using IdentityManagementAPI.Configs;
using IdentityManagementAPI.ModelResources;
using IdentityManagementAPI.Services.Abstracts;
using IdentityManagementAPI.Wrappers;
using Microsoft.Extensions.Options;

namespace IdentityManagementAPI.Services.Concreate;

internal class KeycloackService(IOptions<KeycloakConfiguration> options, ICurrentUserService userService) : IKeycloackService
{
    public async Task<string> GetAccessToken(CancellationToken cancellationToken = default)
    {
        HttpClient client = new();

        string endpoint = $"{options.Value.HostName}/realms/{options.Value.Realm}/protocol/openid-connect/token";

        List<KeyValuePair<string, string>> data = [];
        KeyValuePair<string, string> grantType = new("grant_type", "client_credentials");
        KeyValuePair<string, string> clientId = new("client_id", options.Value.ClientId);
        KeyValuePair<string, string> clientSecret = new("client_secret", options.Value.ClientSecret);

        data.Add(grantType);
        data.Add(clientId);
        data.Add(clientSecret);

        var response = await client.PostAsync(endpoint, new FormUrlEncodedContent(data), cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            // Log error
            Console.WriteLine("Error occured while getting access token");
        }

        var responseStr = await response.Content.ReadAsStringAsync(cancellationToken);
        var result = JsonSerializer.Deserialize<GetAccessTokenResponseModel>(responseStr);

        return result!.AccessToken;
    }

    public async Task<Response<List<UserModel>>> GetUsers(CancellationToken cancellationToken = default)
    {
        HttpClient httpClient = new();

        var endpoint = $"{options.Value.HostName}/admin/realms/{options.Value.Realm}/users";

        if (!httpClient.DefaultRequestHeaders.Contains("Authorization"))
        {
            var token = await GetAccessToken(cancellationToken);
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        }

        var response = await httpClient.GetAsync(endpoint, cancellationToken);

        var responseResult = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return await ResponseError.ErrorResponseAsync<List<UserModel>>(response, cancellationToken);
        }

        var obj = JsonSerializer.Deserialize<List<UserModel>>(responseResult);

        return Response<List<UserModel>>.Success(obj!, null, statusCode: (int)response.StatusCode);
    }

    public async Task<Response<GetAccessTokenResponseModel>> Login(LoginModel loginModel, CancellationToken cancellationToken = default)
    {
        Console.WriteLine(options.Value.HostName);
        HttpClient httpClient = new();
        var endpoint = $"{options.Value.HostName}/realms/{options.Value.Realm}/protocol/openid-connect/token";
        
        List<KeyValuePair<string, string>> data = [];
        KeyValuePair<string, string> grantType = new("grant_type", "password");
        KeyValuePair<string, string> clientId = new("client_id", options.Value.ClientId);
        KeyValuePair<string, string> clientSecret = new("client_secret", options.Value.ClientSecret);
        KeyValuePair<string, string> username = new("username", loginModel.username);
        KeyValuePair<string, string> password = new("password", loginModel.password);

        data.Add(grantType);
        data.Add(clientId);
        data.Add(clientSecret);
        data.Add(username);
        data.Add(password);

        var response = await httpClient.PostAsync(endpoint, new FormUrlEncodedContent(data), cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return await ResponseError.ErrorResponseAsync<GetAccessTokenResponseModel>(response, cancellationToken);
        }

        var responseStr = await response.Content.ReadAsStringAsync(cancellationToken);
        var result = JsonSerializer.Deserialize<GetAccessTokenResponseModel>(responseStr);
        return Response<GetAccessTokenResponseModel>.Success(result!, statusCode: (int)response.StatusCode);
    }

    public async Task<Response<StatusModel>> Register(RegisterModel registerModel, CancellationToken cancellationToken = default)
    {
        HttpClient httpClient = new();

        var endpoint = $"{options.Value.HostName}/admin/realms/{options.Value.Realm}/users";

        string token = await GetAccessToken(cancellationToken);

        httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

        var data = new RegisterRequestModel
        {
            username = registerModel.Username,
            firstName = registerModel.FirstName,
            lastName = registerModel.LastName,
            email = registerModel.Email,
            enabled = registerModel.Enabled,
            emailVerified = registerModel.EmailVerified,
            companyId = registerModel.CompanyId,
            credentials =
            [
                new() {
                    temporary = registerModel.Temporary,
                    type = "password",
                    value = registerModel.Password
                }
            ]
        };

        var json = JsonSerializer.Serialize(data);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await httpClient.PostAsync(endpoint, content, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return await ResponseError.ErrorResponseAsync<StatusModel>(response, cancellationToken);
        }
        return Response<StatusModel>.Success(new StatusModel { Status = 1 }, statusCode: (int)response.StatusCode);
    }

    public async Task<Response<List<UserModel>>> GetUserByEmail(string email, CancellationToken cancellationToken = default)
    {
        HttpClient httpClient = new();

        string endpoint = $"{options.Value.HostName}/admin/realms/{options.Value.Realm}/users?email={email}";

        if (!httpClient.DefaultRequestHeaders.Contains("Authorization"))
        {
            var token = await GetAccessToken(cancellationToken);
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        }

        var response = await httpClient.GetAsync(endpoint, cancellationToken);

        var responseResult = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return await ResponseError.ErrorResponseAsync<List<UserModel>>(response, cancellationToken);
        }

        var obj = JsonSerializer.Deserialize<List<UserModel>>(responseResult);
        return Response<List<UserModel>>.Success(obj ?? [], statusCode: (int)response.StatusCode);
    }

    public async Task<Response<List<UserModel>>> GetUserByUsername(string username, CancellationToken cancellationToken = default)
    {
        HttpClient httpClient = new();

        if (!httpClient.DefaultRequestHeaders.Contains("Authorization"))
        {
            var token = await GetAccessToken(cancellationToken);
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        }

        string endpoint = $"{options.Value.HostName}/admin/realms/{options.Value.Realm}/users?username={username}";

        var response = await httpClient.GetAsync(endpoint, cancellationToken);

        var responseResult = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return await ResponseError.ErrorResponseAsync<List<UserModel>>(response, cancellationToken);
        }

        var obj = JsonSerializer.Deserialize<List<UserModel>>(responseResult);
        return Response<List<UserModel>>.Success(obj ?? [], statusCode: (int)response.StatusCode);
    }

    public async Task<Response<UserModel>> GetUserById(Guid userId, CancellationToken cancellationToken = default)
    {
        HttpClient httpClient = new();

        if (!httpClient.DefaultRequestHeaders.Contains("Authorization"))
        {
            var token = await GetAccessToken(cancellationToken);
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        }

        string endpoint = $"{options.Value.HostName}/admin/realms/{options.Value.Realm}/users/{userId}";

        var response = await httpClient.GetAsync(endpoint, cancellationToken);

        var responseResult = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return await ResponseError.ErrorResponseAsync<UserModel>(response, cancellationToken);
        }

        var obj = JsonSerializer.Deserialize<UserModel>(responseResult);
        return Response<UserModel>.Success(obj!, statusCode: (int)response.StatusCode);
    }

    public async Task<Response<StatusModel>> UpdateUser(UpdateUserModel registerModel, CancellationToken cancellationToken = default)
    {
        HttpClient httpClient = new();

        if (!httpClient.DefaultRequestHeaders.Contains("Authorization"))
        {
            var token = await GetAccessToken(cancellationToken);
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        }

        var endpoint = $"{options.Value.HostName}/admin/realms/{options.Value.Realm}/users/{registerModel.Id}";

        var request = new HttpRequestMessage(HttpMethod.Put, endpoint);
        var json = JsonSerializer.Serialize(registerModel.UserInfo);
        request.Content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await httpClient.SendAsync(request, cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
            return Response<StatusModel>.Fail("data not found");

        if (!response.IsSuccessStatusCode)
        {
            return await ResponseError.ErrorResponseAsync<StatusModel>(response, cancellationToken);
        }

        return Response<StatusModel>.Success(new StatusModel { Status = 1 }, statusCode: (int)response.StatusCode);
    }

    public async Task<Response<StatusModel>> DeleteUserById(Guid userId, CancellationToken cancellationToken = default)
    {
        HttpClient httpClient = new();

        if (!httpClient.DefaultRequestHeaders.Contains("Authorization"))
        {
            var token = await GetAccessToken(cancellationToken);
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        }

        string endpoint = $"{options.Value.HostName}/admin/realms/{options.Value.Realm}/users/{userId}";

        var response = await httpClient.DeleteAsync(endpoint, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return await ResponseError.ErrorResponseAsync<StatusModel>(response, cancellationToken);
        }

        return Response<StatusModel>.Success(new StatusModel { Status = 1 }, statusCode: (int)response.StatusCode);
    }

    public async Task<Response<List<RoleModel>>> GetRoles(CancellationToken cancellationToken = default)
    {
        HttpClient httpClient = new();

        if (!httpClient.DefaultRequestHeaders.Contains("Authorization"))
        {
            var token = await GetAccessToken(cancellationToken);
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        }

        var endpoint = $"{options.Value.HostName}/admin/realms/{options.Value.Realm}/clients/{options.Value.ClientUUID}/roles";

        var response = await httpClient.GetAsync(endpoint, cancellationToken);

        var responseResult = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return await ResponseError.ErrorResponseAsync<List<RoleModel>>(response, cancellationToken);
        }

        var obj = JsonSerializer.Deserialize<List<RoleModel>>(responseResult);
        return Response<List<RoleModel>>.Success(obj ?? [], statusCode: (int)response.StatusCode);
    }

    public async Task<Response<List<RoleModel>>> GetRoleByName(string name, CancellationToken cancellationToken = default)
    {
        HttpClient httpClient = new();

        if (!httpClient.DefaultRequestHeaders.Contains("Authorization"))
        {
            var token = await GetAccessToken(cancellationToken);
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        }

        var endpoint = $"{options.Value.HostName}/admin/realms/{options.Value.Realm}/clients/{options.Value.ClientUUID}/roles/{name}";

        var response = await httpClient.GetAsync(endpoint, cancellationToken);

        var responseResult = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return await ResponseError.ErrorResponseAsync<List<RoleModel>>(response, cancellationToken);
        }

        var obj = JsonSerializer.Deserialize<List<RoleModel>>(responseResult);

        return Response<List<RoleModel>>.Success(obj ?? [], statusCode: (int)response.StatusCode);
    }

    public async Task<Response<StatusModel>> CreateRole(CreateRoleModel roleModel, CancellationToken cancellationToken = default)
    {
        HttpClient httpClient = new();

        if (!httpClient.DefaultRequestHeaders.Contains("Authorization"))
        {
            var token = await GetAccessToken(cancellationToken);
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        }

        var endpoint = $"{options.Value.HostName}/admin/realms/{options.Value.Realm}/clients/{options.Value.ClientUUID}/roles";

        var json = JsonSerializer.Serialize(roleModel);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await httpClient.PostAsync(endpoint, content, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return await ResponseError.ErrorResponseAsync<StatusModel>(response, cancellationToken);
        }

        return Response<StatusModel>.Success(new StatusModel { Status = 1 }, statusCode: (int)response.StatusCode);
    }

    public async Task<Response<StatusModel>> DeleteRoleByName(string name, CancellationToken cancellationToken = default)
    {
        HttpClient httpClient = new();

        if (!httpClient.DefaultRequestHeaders.Contains("Authorization"))
        {
            var token = await GetAccessToken(cancellationToken);
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        }

        var endpoint = $"{options.Value.HostName}/admin/realms/{options.Value.Realm}/clients/{options.Value.ClientUUID}/roles/{name}";

        var response = await httpClient.DeleteAsync(endpoint, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return await ResponseError.ErrorResponseAsync<StatusModel>(response, cancellationToken);
        }

        return Response<StatusModel>.Success(new StatusModel { Status = 1 }, statusCode: (int)response.StatusCode);
    }

    public async Task<Response<StatusModel>> AssignmentRolesByUserId(UserRoleModel userRoleModel, CancellationToken cancellationToken)
    {
        HttpClient httpClient = new();

        if (!httpClient.DefaultRequestHeaders.Contains("Authorization"))
        {
            var token = await GetAccessToken(cancellationToken);
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        }

        var endpoint = $"{options.Value.HostName}/admin/realms/{options.Value.Realm}/users/{userRoleModel.Id}/role-mappings/clients/{options.Value.ClientUUID}";

        var json = JsonSerializer.Serialize(userRoleModel.Roles);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await httpClient.PostAsync(endpoint, content, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return await ResponseError.ErrorResponseAsync<StatusModel>(response, cancellationToken);
        }

        return Response<StatusModel>.Success(new StatusModel { Status = 1 }, statusCode: (int)response.StatusCode);
    }

    public async Task<Response<StatusModel>> UnAssignmentRolesByUserId(UnAssignmentRolesByUserId userRoleModel, CancellationToken cancellationToken)
    {
        HttpClient httpClient = new();

        if (!httpClient.DefaultRequestHeaders.Contains("Authorization"))
        {
            var token = await GetAccessToken(cancellationToken);
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        }

        var endpoint = $"{options.Value.HostName}/admin/realms/{options.Value.Realm}/users/{userRoleModel.Id}/role-mappings/clients/{options.Value.ClientUUID}";

        var request = new HttpRequestMessage(HttpMethod.Delete, endpoint);

        var json = JsonSerializer.Serialize(userRoleModel.Roles);
        request.Content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await httpClient.SendAsync(request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return await ResponseError.ErrorResponseAsync<StatusModel>(response, cancellationToken);
        }

        return Response<StatusModel>.Success(new StatusModel { Status = 1 }, statusCode: (int)response.StatusCode);
    }

    public async Task<Response<List<UserRoleDetailModel>>> GetAllUserRolesByUserId(Guid id, CancellationToken cancellationToken = default)
    {
        HttpClient httpClient = new();

        if (!httpClient.DefaultRequestHeaders.Contains("Authorization"))
        {
            var token = await GetAccessToken(cancellationToken);
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        }

        var endpoint = $"{options.Value.HostName}/admin/realms/{options.Value.Realm}/users/{id}/role-mappings/clients/{options.Value.ClientUUID}";

        var response = await httpClient.GetAsync(endpoint, cancellationToken);

        var responseResult = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return await ResponseError.ErrorResponseAsync<List<UserRoleDetailModel>>(response, cancellationToken);
        }

        var obj = JsonSerializer.Deserialize<List<UserRoleDetailModel>>(responseResult);
        return Response<List<UserRoleDetailModel>>.Success(obj ?? [], statusCode: (int)response.StatusCode);
    }

    public async Task<Response<StatusModel>> Logout(LogoutModel model, CancellationToken cancellationToken = default)
    {
        HttpClient httpClient = new();

        string endpoint = $"{options.Value.HostName}/admin/realms/{options.Value.Realm}/users/{model.UserId}/logout";

        if (!httpClient.DefaultRequestHeaders.Contains("Authorization"))
        {
            var token = await GetAccessToken(cancellationToken);
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        }

        var response = await httpClient.PostAsync(endpoint, null, cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
            return Response<StatusModel>.Fail("data not found", statusCode: (int)response.StatusCode);

        if (!response.IsSuccessStatusCode)
        {
            return await ResponseError.ErrorResponseAsync<StatusModel>(response, cancellationToken);
        }

        return Response<StatusModel>.Success(new StatusModel { Status = 1 }, statusCode: (int)response.StatusCode);
    }

    public async Task<Response<StatusModel>> SendResetPasswordEmail(CancellationToken cancellationToken = default)
    {
        HttpClient httpClient = new();

        if (!httpClient.DefaultRequestHeaders.Contains("Authorization"))
        {
            var token = await GetAccessToken(cancellationToken);
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        }

        string endpoint = $"{options.Value.HostName}/admin/realms/{options.Value.Realm}/users/{userService.Id}/execute-actions-email";

        var response = await httpClient.PutAsync(endpoint, null, cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
            return Response<StatusModel>.Fail("data not found", statusCode: (int)response.StatusCode);

        if (!response.IsSuccessStatusCode)
        {
            return await ResponseError.ErrorResponseAsync<StatusModel>(response, cancellationToken);
        }

        return Response<StatusModel>.Success(new StatusModel { Status = 1 }, statusCode: (int)response.StatusCode);
    }

    public async Task<Response<StatusModel>> ResetPassword(ResetPasswordModel model, CancellationToken cancellationToken = default)
    {
        HttpClient httpClient = new();

        if (!httpClient.DefaultRequestHeaders.Contains("Authorization"))
        {
            var token = await GetAccessToken(cancellationToken);
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        }

        string endpoint = $"{options.Value.HostName}/admin/realms/{options.Value.Realm}/users/{userService.Id}/reset-password";

        var json = JsonSerializer.Serialize(model);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await httpClient.PutAsync(endpoint, content, cancellationToken);


        if (response.StatusCode == HttpStatusCode.NotFound)
            return Response<StatusModel>.Fail("data not found", statusCode: (int)response.StatusCode);

        if (!response.IsSuccessStatusCode)
        {
            return await ResponseError.ErrorResponseAsync<StatusModel>(response, cancellationToken);
        }

        return Response<StatusModel>.Success(new StatusModel { Status = 1 }, statusCode: (int)response.StatusCode);
    }

    public async Task<Response<GroupRepresentationModel>> AddGroup(GroupRepresentationModel model, CancellationToken cancellationToken = default)
    {
        HttpClient httpClient = new();

        if (!httpClient.DefaultRequestHeaders.Contains("Authorization"))
        {
            var token = await GetAccessToken(cancellationToken);
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        }

        var endpoint = $"{options.Value.HostName}/admin/realms/{options.Value.Realm}/groups";

        var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
        var json = JsonSerializer.Serialize(model);
        request.Content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await httpClient.SendAsync(request, cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)

            return Response<GroupRepresentationModel>.Fail("data not found", statusCode: (int)response.StatusCode);

        if (!response.IsSuccessStatusCode)
        {
            return await ResponseError.ErrorResponseAsync<GroupRepresentationModel>(response, cancellationToken);
        }

        return Response<GroupRepresentationModel>.Success(model, statusCode: (int)response.StatusCode);
    }

    public async Task<Response<List<GroupRepresentationModel>>> GetGroups(GetGroupListByFilterRequest getGroupListByFilterRequest, CancellationToken cancellationToken = default)
    {
        using HttpClient httpClient = new();


        if (!httpClient.DefaultRequestHeaders.Contains("Authorization"))
        {
            var token = await GetAccessToken(cancellationToken);
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        }

        var endpoint = $"{options.Value.HostName}/admin/realms/{options.Value.Realm}/groups";

        var queryParameters = new List<string>();

        if (!string.IsNullOrEmpty(getGroupListByFilterRequest.Search))
            queryParameters.Add($"search={Uri.EscapeDataString(getGroupListByFilterRequest.Search)}");
        if (getGroupListByFilterRequest.BriefRepresentation.HasValue)
            queryParameters.Add($"briefRepresentation={getGroupListByFilterRequest.BriefRepresentation.Value.ToString().ToLower()}");
        if (getGroupListByFilterRequest.Exact.HasValue)
            queryParameters.Add($"exact={getGroupListByFilterRequest.Exact.Value.ToString().ToLower()}");
        if (getGroupListByFilterRequest.First.HasValue)
            queryParameters.Add($"first={getGroupListByFilterRequest.First.Value}");
        if (getGroupListByFilterRequest.Max.HasValue)
            queryParameters.Add($"max={getGroupListByFilterRequest.Max.Value}");
        if (getGroupListByFilterRequest.PopulateHierarchy.HasValue)
            queryParameters.Add($"populateHierarchy={getGroupListByFilterRequest.PopulateHierarchy.Value.ToString().ToLower()}");

        if (queryParameters.Any())
            endpoint += "?" + string.Join("&", queryParameters);

        var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
        var response = await httpClient.SendAsync(request, cancellationToken);
        var responseResult = await response.Content.ReadAsStringAsync(cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
            return Response<List<GroupRepresentationModel>>.Fail("data not found", statusCode: (int)response.StatusCode);

        if (!response.IsSuccessStatusCode)
        {
            return await ResponseError.ErrorResponseAsync<List<GroupRepresentationModel>>(response, cancellationToken);
        }

        var obj = JsonSerializer.Deserialize<List<GroupRepresentationModel>>(responseResult);
        return Response<List<GroupRepresentationModel>>.Success(obj ?? [], statusCode: (int)response.StatusCode);
    }


    public async Task<Response<List<GroupRepresentationModel>>> GetGroupChildren(string groupId, GetGroupChildrenListByFilterRequest getGroupChildrenListByFilterRequest, CancellationToken cancellationToken = default)
    {
        HttpClient httpClient = new();

        if (!httpClient.DefaultRequestHeaders.Contains("Authorization"))
        {
            var token = await GetAccessToken(cancellationToken);
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        }

        var endpoint = $"{options.Value.HostName}/admin/realms/{options.Value.Realm}/groups/{groupId}/children";

        var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
        var json = JsonSerializer.Serialize(getGroupChildrenListByFilterRequest);
        request.Content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await httpClient.SendAsync(request, cancellationToken);

        var responseResult = await response.Content.ReadAsStringAsync(cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
            return Response<List<GroupRepresentationModel>>.Fail("data not found", statusCode: (int)response.StatusCode);

        if (!response.IsSuccessStatusCode)
        {
            return await ResponseError.ErrorResponseAsync<List<GroupRepresentationModel>>(response, cancellationToken);
        }

        var obj = JsonSerializer.Deserialize<List<GroupRepresentationModel>>(responseResult);

        return Response<List<GroupRepresentationModel>>.Success(obj ?? [], statusCode: (int)response.StatusCode);
    }

    public async Task<Response<GroupRepresentationModel>> AddGroupChildren(string groupId, GroupRepresentationModel model, CancellationToken cancellationToken = default)
    {
        HttpClient httpClient = new();

        if (!httpClient.DefaultRequestHeaders.Contains("Authorization"))
        {
            var token = await GetAccessToken(cancellationToken);
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        }

        var endpoint = $"{options.Value.HostName}/admin/realms/{options.Value.Realm}/groups/{groupId}/children";

        var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
        var json = JsonSerializer.Serialize(model);
        request.Content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await httpClient.SendAsync(request, cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
            return Response<GroupRepresentationModel>.Fail("data not found", statusCode: (int)response.StatusCode);

        if (!response.IsSuccessStatusCode)
        {
            return await ResponseError.ErrorResponseAsync<GroupRepresentationModel>(response, cancellationToken);
        }

        return Response<GroupRepresentationModel>.Success(model!, statusCode: (int)response.StatusCode);
    }

    public async Task<Response<GroupRepresentationModel>> GetGroup(string groupId, CancellationToken cancellationToken = default)
    {
        HttpClient httpClient = new();

        if (!httpClient.DefaultRequestHeaders.Contains("Authorization"))
        {
            var token = await GetAccessToken(cancellationToken);
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        }

        var endpoint = $"{options.Value.HostName}/admin/realms/{options.Value.Realm}/groups/{groupId}";

        var request = new HttpRequestMessage(HttpMethod.Get, endpoint);

        var response = await httpClient.SendAsync(request, cancellationToken);
        var responseResult = await response.Content.ReadAsStringAsync(cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
            return Response<GroupRepresentationModel>.Fail("data not found", statusCode: (int)response.StatusCode);

        if (!response.IsSuccessStatusCode)
        {
            return await ResponseError.ErrorResponseAsync<GroupRepresentationModel>(response, cancellationToken);
        }

        var model = JsonSerializer.Deserialize<GroupRepresentationModel>(responseResult);

        return Response<GroupRepresentationModel>.Success(model!, statusCode: (int)response.StatusCode);
    }

    public async Task<Response<ManagementPermissionReferenceModel>> GetGroupManagementPermission(string groupId, CancellationToken cancellationToken = default)
    {
        HttpClient httpClient = new();

        if (!httpClient.DefaultRequestHeaders.Contains("Authorization"))
        {
            var token = await GetAccessToken(cancellationToken);
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        }

        var endpoint = $"{options.Value.HostName}/admin/realms/{options.Value.Realm}/groups/{groupId}/management/permissions";

        var request = new HttpRequestMessage(HttpMethod.Get, endpoint);

        var response = await httpClient.SendAsync(request, cancellationToken);
        var responseResult = await response.Content.ReadAsStringAsync(cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
            return Response<ManagementPermissionReferenceModel>.Fail("data not found", statusCode: (int)response.StatusCode);

        if (!response.IsSuccessStatusCode)
        {
            return await ResponseError.ErrorResponseAsync<ManagementPermissionReferenceModel>(response, cancellationToken);
        }

        var model = JsonSerializer.Deserialize<ManagementPermissionReferenceModel>(responseResult);
        return Response<ManagementPermissionReferenceModel>.Success(model!, statusCode: (int)response.StatusCode);
    }

    public async Task<Response<ManagementPermissionReferenceModel>> UpdateGroupManagementPermission(string groupId, ManagementPermissionReferenceModel model, CancellationToken cancellationToken = default)
    {
        HttpClient httpClient = new();

        if (!httpClient.DefaultRequestHeaders.Contains("Authorization"))
        {
            var token = await GetAccessToken(cancellationToken);
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        }

        var endpoint = $"{options.Value.HostName}/admin/realms/{options.Value.Realm}/groups/{groupId}/management/permissions";

        var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
        var json = JsonSerializer.Serialize(model);
        request.Content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await httpClient.SendAsync(request, cancellationToken);
        var responseResult = await response.Content.ReadAsStringAsync(cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
            return Response<ManagementPermissionReferenceModel>.Fail("data not found", statusCode: (int)response.StatusCode);

        if (!response.IsSuccessStatusCode)
        {
            return await ResponseError.ErrorResponseAsync<ManagementPermissionReferenceModel>(response, cancellationToken);
        }

        var result = JsonSerializer.Deserialize<ManagementPermissionReferenceModel>(responseResult);

        return Response<ManagementPermissionReferenceModel>.Success(model!, statusCode: (int)response.StatusCode);
    }

    public async Task<Response<List<UserRepresentationModel>>> GetGroupUsers(string groupId, GetGroupMembersByFilter getGroupMembersByFilter, CancellationToken cancellationToken = default)
    {
        HttpClient httpClient = new();

        if (!httpClient.DefaultRequestHeaders.Contains("Authorization"))
        {
            var token = await GetAccessToken(cancellationToken);
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        }

        var endpoint = $"{options.Value.HostName}/admin/realms/{options.Value.Realm}/groups/{groupId}/members";

        var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
        var json = JsonSerializer.Serialize(getGroupMembersByFilter);
        request.Content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await httpClient.SendAsync(request, cancellationToken);
        var responseResult = await response.Content.ReadAsStringAsync(cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
            return Response<List<UserRepresentationModel>>.Fail("data not found", statusCode: (int)response.StatusCode);

        if (!response.IsSuccessStatusCode)
        {
            return await ResponseError.ErrorResponseAsync<List<UserRepresentationModel>>(response, cancellationToken);
        }

        var result = JsonSerializer.Deserialize<List<UserRepresentationModel>>(responseResult);

        return Response<List<UserRepresentationModel>>.Success(result ?? [], statusCode: (int)response.StatusCode);
    }

    public async Task<Response<GroupRepresentationModel>> UpdateGroup(string groupId, GroupRepresentationModel model, CancellationToken cancellationToken = default)
    {
        HttpClient httpClient = new();

        if (!httpClient.DefaultRequestHeaders.Contains("Authorization"))
        {
            var token = await GetAccessToken(cancellationToken);
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        }

        var endpoint = $"{options.Value.HostName}/admin/realms/{options.Value.Realm}/groups/{groupId}";

        var request = new HttpRequestMessage(HttpMethod.Put, endpoint);
        var json = JsonSerializer.Serialize(model);
        request.Content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await httpClient.SendAsync(request, cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
            return Response<GroupRepresentationModel>.Fail("data not found", statusCode: (int)response.StatusCode);

        if (!response.IsSuccessStatusCode)
        {
            return await ResponseError.ErrorResponseAsync<GroupRepresentationModel>(response, cancellationToken);
        }

        return Response<GroupRepresentationModel>.Success(model, statusCode: (int)response.StatusCode);
    }

    public async Task<Response<MappingsRepresentationModel>> GetGroupRoleMapping(string groupId, CancellationToken cancellationToken = default)
    {
        HttpClient httpClient = new();

        if (!httpClient.DefaultRequestHeaders.Contains("Authorization"))
        {
            var token = await GetAccessToken(cancellationToken);
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        }

        var endpoint = $"{options.Value.HostName}/admin/realms/{options.Value.Realm}/groups/{groupId}/role-mappings";

        var request = new HttpRequestMessage(HttpMethod.Get, endpoint);

        var response = await httpClient.SendAsync(request, cancellationToken);
        var responseResult = await response.Content.ReadAsStringAsync(cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
            return Response<MappingsRepresentationModel>.Fail("data not found", statusCode: (int)response.StatusCode);

        if (!response.IsSuccessStatusCode)
        {
            return await ResponseError.ErrorResponseAsync<MappingsRepresentationModel>(response, cancellationToken);
        }

        var result = JsonSerializer.Deserialize<MappingsRepresentationModel>(responseResult);

        return Response<MappingsRepresentationModel>.Success(result!, statusCode: (int)response.StatusCode);
    }

    public async Task<Response<List<RoleRepresentationModel>>> GetGroupRoleMappingRealmAvailable(string groupId, CancellationToken cancellationToken = default)
    {
        HttpClient httpClient = new();

        if (!httpClient.DefaultRequestHeaders.Contains("Authorization"))
        {
            var token = await GetAccessToken(cancellationToken);
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        }

        var endpoint = $"{options.Value.HostName}/admin/realms/{options.Value.Realm}/groups/{groupId}/role-mappings/realm/available";

        var request = new HttpRequestMessage(HttpMethod.Get, endpoint);

        var response = await httpClient.SendAsync(request, cancellationToken);
        var responseResult = await response.Content.ReadAsStringAsync(cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
            return Response<List<RoleRepresentationModel>>.Fail("data not found", statusCode: (int)response.StatusCode);

        if (!response.IsSuccessStatusCode)
        {
            return await ResponseError.ErrorResponseAsync<List<RoleRepresentationModel>>(response, cancellationToken);
        }

        var result = JsonSerializer.Deserialize<List<RoleRepresentationModel>>(responseResult);

        return Response<List<RoleRepresentationModel>>.Success(result ?? [], statusCode: (int)response.StatusCode);
    }

    public async Task<Response<List<RoleRepresentationModel>>> GetGroupRoleMappingRealmComposite(string groupId, CancellationToken cancellationToken = default)
    {
        HttpClient httpClient = new();

        if (!httpClient.DefaultRequestHeaders.Contains("Authorization"))
        {
            var token = await GetAccessToken(cancellationToken);
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        }

        var endpoint = $"{options.Value.HostName}/admin/realms/{options.Value.Realm}/groups/{groupId}/role-mappings/realm/composite";

        var request = new HttpRequestMessage(HttpMethod.Get, endpoint);

        var response = await httpClient.SendAsync(request, cancellationToken);
        var responseResult = await response.Content.ReadAsStringAsync(cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
            return Response<List<RoleRepresentationModel>>.Fail("data not found", statusCode: (int)response.StatusCode);

        if (!response.IsSuccessStatusCode)
        {
            return await ResponseError.ErrorResponseAsync<List<RoleRepresentationModel>>(response, cancellationToken);
        }

        var result = JsonSerializer.Deserialize<List<RoleRepresentationModel>>(responseResult);

        return Response<List<RoleRepresentationModel>>.Success(result ?? [], statusCode: (int)response.StatusCode);
    }

    public async Task<Response<List<RoleRepresentationModel>>> GetGroupRoleMappingRealm(string groupId, CancellationToken cancellationToken = default)
    {
        HttpClient httpClient = new();

        if (!httpClient.DefaultRequestHeaders.Contains("Authorization"))
        {
            var token = await GetAccessToken(cancellationToken);
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        }

        var endpoint = $"{options.Value.HostName}/admin/realms/{options.Value.Realm}/groups/{groupId}/role-mappings/realm";

        var request = new HttpRequestMessage(HttpMethod.Get, endpoint);

        var response = await httpClient.SendAsync(request, cancellationToken);
        var responseResult = await response.Content.ReadAsStringAsync(cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
            return Response<List<RoleRepresentationModel>>.Fail("data not found", statusCode: (int)response.StatusCode);

        if (!response.IsSuccessStatusCode)
        {
            return await ResponseError.ErrorResponseAsync<List<RoleRepresentationModel>>(response, cancellationToken);
        }

        var result = JsonSerializer.Deserialize<List<RoleRepresentationModel>>(responseResult);

        return Response<List<RoleRepresentationModel>>.Success(result ?? [], statusCode: (int)response.StatusCode);
    }

    public async Task<Response<StatusModel>> AddGroupRoleToMappingRealm(string groupId, RoleRepresentationModel model, CancellationToken cancellationToken = default)
    {
        HttpClient httpClient = new();

        if (!httpClient.DefaultRequestHeaders.Contains("Authorization"))
        {
            var token = await GetAccessToken(cancellationToken);
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        }

        var endpoint = $"{options.Value.HostName}/admin/realms/{options.Value.Realm}/groups/{groupId}/role-mappings/realm";

        var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
        var json = JsonSerializer.Serialize(model);
        request.Content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await httpClient.SendAsync(request, cancellationToken);
        var responseResult = await response.Content.ReadAsStringAsync(cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
            return Response<StatusModel>.Fail("data not found", statusCode: (int)response.StatusCode);

        if (!response.IsSuccessStatusCode)
        {
            return await ResponseError.ErrorResponseAsync<StatusModel>(response, cancellationToken);
        }

        return Response<StatusModel>.Success(new StatusModel { Status = 1 }, statusCode: (int)response.StatusCode);
    }

    public async Task<Response<List<MappingsRepresentationModel>>> GetUserRoleMappings(string userId, CancellationToken cancellationToken = default)
    {
        HttpClient httpClient = new();

        if (!httpClient.DefaultRequestHeaders.Contains("Authorization"))
        {
            var token = await GetAccessToken(cancellationToken);
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        }

        var endpoint = $"{options.Value.HostName}/admin/realms/{options.Value.Realm}/users/{userId}/role-mappings";

        var request = new HttpRequestMessage(HttpMethod.Get, endpoint);

        var response = await httpClient.SendAsync(request, cancellationToken);
        var responseResult = await response.Content.ReadAsStringAsync(cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
            return Response<List<MappingsRepresentationModel>>.Fail("data not found", statusCode: (int)response.StatusCode);

        if (!response.IsSuccessStatusCode)
        {
            return await ResponseError.ErrorResponseAsync<List<MappingsRepresentationModel>>(response, cancellationToken);
        }

        var result = JsonSerializer.Deserialize<List<MappingsRepresentationModel>>(responseResult);

        return Response<List<MappingsRepresentationModel>>.Success(result ?? [], statusCode: (int)response.StatusCode);
    }

    public async Task<Response<List<RoleRepresentationModel>>> GetUserRoleMappingsRealmAvaible(string userId, CancellationToken cancellationToken = default)
    {
        HttpClient httpClient = new();

        if (!httpClient.DefaultRequestHeaders.Contains("Authorization"))
        {
            var token = await GetAccessToken(cancellationToken);
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        }

        var endpoint = $"{options.Value.HostName}/admin/realms/{options.Value.Realm}/users/{userId}/role-mappings/realm/available";

        var request = new HttpRequestMessage(HttpMethod.Get, endpoint);

        var response = await httpClient.SendAsync(request, cancellationToken);
        var responseResult = await response.Content.ReadAsStringAsync(cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
            return Response<List<RoleRepresentationModel>>.Fail("data not found", statusCode: (int)response.StatusCode);

        if (!response.IsSuccessStatusCode)
        {
            return await ResponseError.ErrorResponseAsync<List<RoleRepresentationModel>>(response, cancellationToken);
        }

        var result = JsonSerializer.Deserialize<List<RoleRepresentationModel>>(responseResult);

        return Response<List<RoleRepresentationModel>>.Success(result ?? [], statusCode: (int)response.StatusCode);
    }

    public async Task<Response<StatusModel>> AddUserToGroup(string userId, string groupId, CancellationToken cancellationToken = default)
    {
        ///admin/realms/{realm}/users/{user-id}/groups/{groupId}
        HttpClient httpClient = new();

        if (!httpClient.DefaultRequestHeaders.Contains("Authorization"))
        {
            var token = await GetAccessToken(cancellationToken);
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        }

        var endpoint = $"{options.Value.HostName}/admin/realms/{options.Value.Realm}/users/{userId}/groups/{groupId}";

        var request = new HttpRequestMessage(HttpMethod.Put, endpoint);

        var response = await httpClient.SendAsync(request, cancellationToken);
        var responseResult = await response.Content.ReadAsStringAsync(cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
            return Response<StatusModel>.Fail("data not found", statusCode: (int)response.StatusCode);

        if (!response.IsSuccessStatusCode)
        {
            return await ResponseError.ErrorResponseAsync<StatusModel>(response, cancellationToken);
        }

        return Response<StatusModel>.Success(new StatusModel { Status = 1 }, statusCode: (int)response.StatusCode);
    }
}