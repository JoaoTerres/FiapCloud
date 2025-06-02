namespace FIapCloud.App.Dtos;

public class LoginUserResponse
{
    public string AccessToken { get; set; } = default!;
    public double ExpiresIn { get; set; } 
    public UserTokenResponse UserToken { get; set; } = default!;
}

public class UserTokenResponse
{
    public string Id { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Name { get; set; } = default!;
    public IEnumerable<ClaimResponse> Claims { get; set; } = default!;
}

public class ClaimResponse
{
    public string Value { get; set; } = default!;
    public string Type { get; set; } = default!;
}