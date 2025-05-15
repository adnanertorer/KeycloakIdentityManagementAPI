namespace IdentityManagementAPI.Wrappers;

public static class ResponseWrapperExtensions
{
    public static IResult ToIResult<T>(this Response<T> response){
        if(response.Succeeded)
            return Results.Ok(response);
        return Results.Problem(response.Message, statusCode: response.StatusCode);
    }
}