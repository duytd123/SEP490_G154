using SEP490_G154_Service.DTOs.MaLogin;

namespace SEP490_G154_Service.Interface
{
    public interface ILogin
    {
        Task<LoginResponseDTO> LoginAsync(LoginRequestDTO request);

        Task<object> RegisterAsync(CreateAcc newAcc, int defaultRoleId);

        Task<object> LoginWithGoogleAsync(GoogleLoginDTO request, int roleId);

        Task<object> LoginWithFacebookAsync(FacebookLoginDTO request, int roleId);

        Task<object> ForgotPasswordAsync(ForgotPasswordDTO request);

        Task<object> ResetPasswordWithTokenAsync(ResetPasswordWithTokenDTO request);
    }
}
