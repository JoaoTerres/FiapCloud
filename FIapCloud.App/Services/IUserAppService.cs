using FIapCloud.App.Dtos;

namespace FIapCloud.App.Services;

public interface IUserAppService
{
    Task<Guid> CreateUserAsync(CreateUserRequest createUserRequest);
    Task<Guid> ChangeUserStatusAsync(Guid userId, ChangeUserStatusRequest changeUserStatusRequest);
    Task<LoginUserResponse> LoginUserAsync(LoginUserRequest loginRequest);

}
