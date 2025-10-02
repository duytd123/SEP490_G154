namespace SEP490_G154_Service.DTOs.MaLogin
{
    public class ResetPasswordWithOtpDTO
    {
        public string Email { get; set; } = string.Empty;
        public string Otp { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}
